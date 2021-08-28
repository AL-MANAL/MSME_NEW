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
    public class PortalAccessUserLogController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public PortalAccessUserLogController()
        {
            ViewBag.Menutype = "Protal";
            ViewBag.SubMenutype = "Access_UserLog";
        }


        //---------------Start Portal Accer User Log --------------------

        [AllowAnonymous]
        public ActionResult AddAccessUserLog()
        {
            PortalAccessUserLogModels objPortal = new PortalAccessUserLogModels();
            try
            {
                ViewBag.SubMenutype = "Access_UserLog";
                objPortal.division = objGlobaldata.GetCurrentUserSession().division;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.AccessUserName = objGlobaldata.GetPortalAccessUserNameList();
                ViewBag.PortalCategory = objGlobaldata.GetDropdownList("Portal Category");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.PortalName = objGlobaldata.GetDropdownList("Portal Name");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAccessUserLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPortal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAccessUserLog(PortalAccessUserLogModels objPortal, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
               // objPortal.master_div = form["master_div"];              

                DateTime dateValue;
                if (DateTime.TryParse(form["date_access"], out dateValue) == true)
                {
                    objPortal.date_access = dateValue;
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
                            objGlobaldata.AddFunctionalLog("Exception in AddAccessUserLog-upload: " + ex.ToString());
                        }
                    }
                    objPortal.upload = objPortal.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objPortal.FunAddAccessUserLog(objPortal))
                {
                    TempData["Successdata"] = "Added Portal Access UserLog Master details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAccessUserLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AccessUserLogList");
        }

        [AllowAnonymous]
        public ActionResult AccessUserLogList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Access_UserLog";

            PortalAccessModelsList objPortalList = new PortalAccessModelsList();
            objPortalList.AccessList = new List<PortalAccessUserLogModels>();

            PortalAccessUserLogModels objParties = new PortalAccessUserLogModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_access_userlog,portal_name,portal_access_username,date_access,activity_performed,requested_by," +
                    "request_apporvedby,remarks,upload,division,loggedby,portal_category from t_portal_access_userlog where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', division)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "order by id_access_userlog desc";
                DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsPortalList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            PortalAccessUserLogModels objPartiesModels = new PortalAccessUserLogModels
                            {
                                id_access_userlog = (dsPortalList.Tables[0].Rows[i]["id_access_userlog"].ToString()),
                                portal_name = objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[i]["portal_name"].ToString()),
                                portal_category = objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[i]["portal_category"].ToString()),
                                portal_access_username = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["portal_access_username"].ToString()),
                                activity_performed = dsPortalList.Tables[0].Rows[i]["activity_performed"].ToString(),
                                requested_by = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["requested_by"].ToString()),
                                request_apporvedby = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["request_apporvedby"].ToString()),
                                remarks = (dsPortalList.Tables[0].Rows[i]["remarks"].ToString()),
                                upload = dsPortalList.Tables[0].Rows[i]["upload"].ToString(),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsPortalList.Tables[0].Rows[i]["division"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsPortalList.Tables[0].Rows[i]["date_access"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[i]["date_access"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.date_access = dtDocDate;
                            }
                            
                            objPortalList.AccessList.Add(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AccessUserLogList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessUserLogList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPortalList.AccessList.ToList());
        }

        [AllowAnonymous]
        public ActionResult AccessUserLogEdit()
        {
            ViewBag.SubMenutype = "Access_UserLog";

            //PortalAccessModelsList objPortalList = new PortalAccessModelsList();
            //objPortalList.AccessList = new List<PortalAccessUserLogModels>();

            PortalAccessUserLogModels objParties = new PortalAccessUserLogModels();
            try
            {
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.AccessUserName = objGlobaldata.GetPortalAccessUserNameList();
                ViewBag.PortalName = objGlobaldata.GetDropdownList("Portal Name"); 
                ViewBag.PortalCategory= objGlobaldata.GetDropdownList("Portal Category");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                string sid_access_userlog = Request.QueryString["id_access_userlog"];
                if (sid_access_userlog != null && sid_access_userlog != "")
                { 
                
                string sSqlstmt = "select id_access_userlog,portal_name,portal_access_username,date_access,activity_performed,requested_by," +
                    "request_apporvedby,remarks,upload,division,loggedby,portal_category from t_portal_access_userlog where id_access_userlog= '" + sid_access_userlog + "'";

                    DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        PortalAccessUserLogModels objPartiesModels = new PortalAccessUserLogModels
                        {
                            id_access_userlog = (dsPortalList.Tables[0].Rows[0]["id_access_userlog"].ToString()),
                            portal_name = (dsPortalList.Tables[0].Rows[0]["portal_name"].ToString()),
                            portal_access_username = /*objGlobaldata.GetMultiHrEmpNameById*/(dsPortalList.Tables[0].Rows[0]["portal_access_username"].ToString()),
                            activity_performed = dsPortalList.Tables[0].Rows[0]["activity_performed"].ToString(),
                            requested_by =/* objGlobaldata.GetMultiHrEmpNameById*/(dsPortalList.Tables[0].Rows[0]["requested_by"].ToString()),
                            request_apporvedby = /*objGlobaldata.GetMultiHrEmpNameById*/(dsPortalList.Tables[0].Rows[0]["request_apporvedby"].ToString()),
                            remarks = (dsPortalList.Tables[0].Rows[0]["remarks"].ToString()),
                            upload = dsPortalList.Tables[0].Rows[0]["upload"].ToString(),
                            division =/* objGlobaldata.GetMultiCompanyBranchNameById*/(dsPortalList.Tables[0].Rows[0]["division"].ToString()),
                            portal_category = (dsPortalList.Tables[0].Rows[0]["portal_category"].ToString()),
                        };

                        DateTime dtDocDate;
                        if (dsPortalList.Tables[0].Rows[0]["date_access"].ToString() != ""
                           && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["date_access"].ToString(), out dtDocDate))
                        {
                            objPartiesModels.date_access = dtDocDate;
                        }

                        return View(objPartiesModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AccessUserLogEdit: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                }               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessUserLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AccessUserLogList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccessUserLogEdit(PortalAccessUserLogModels objPortal, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objPortal.division = form["division"];              

                DateTime dateValue;
                if (DateTime.TryParse(form["date_access"], out dateValue) == true)
                {
                    objPortal.date_access = dateValue;
                }

                string QCDelete = Request.Form["QCDocsValselectall"];
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
                            objGlobaldata.AddFunctionalLog("Exception in AddAccessUserLog-upload: " + ex.ToString());
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
                if (objPortal.FunUpdatePortalAccess(objPortal))
                {
                    TempData["Successdata"] = "Updated Portal Access UserLog Master details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessUserLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AccessUserLogList");
        }

        [AllowAnonymous]
        public ActionResult AccessUserLogDetails()
        {
            ViewBag.SubMenutype = "Access_UserLog";

            //PortalAccessModelsList objPortalList = new PortalAccessModelsList();
            //objPortalList.AccessList = new List<PortalAccessUserLogModels>();

            PortalAccessUserLogModels objParties = new PortalAccessUserLogModels();
            try
            {
                //ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                //ViewBag.AccessUserName = objGlobaldata.GetPortalAccessUserNameList();
                //ViewBag.PortalName = objGlobaldata.GetDropdownList("Portal Category");
                //ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                string sid_access_userlog = Request.QueryString["id_access_userlog"];
                if (sid_access_userlog != null && sid_access_userlog != "")
                {

                    string sSqlstmt = "select id_access_userlog,portal_name,portal_access_username,date_access,activity_performed,requested_by," +
                        "request_apporvedby,remarks,upload,division,portal_category from t_portal_access_userlog where id_access_userlog= '" + sid_access_userlog + "'";

                    DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            PortalAccessUserLogModels objPartiesModels = new PortalAccessUserLogModels
                            {
                                id_access_userlog = (dsPortalList.Tables[0].Rows[0]["id_access_userlog"].ToString()),
                                portal_name = objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[0]["portal_name"].ToString()),
                                portal_category = objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[0]["portal_category"].ToString()),
                                portal_access_username = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["portal_access_username"].ToString()),
                                activity_performed = dsPortalList.Tables[0].Rows[0]["activity_performed"].ToString(),
                                requested_by =objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["requested_by"].ToString()),
                                request_apporvedby = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["request_apporvedby"].ToString()),
                                remarks = (dsPortalList.Tables[0].Rows[0]["remarks"].ToString()),
                                upload = dsPortalList.Tables[0].Rows[0]["upload"].ToString(),
                                division =objGlobaldata.GetMultiCompanyBranchNameById(dsPortalList.Tables[0].Rows[0]["division"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsPortalList.Tables[0].Rows[0]["date_access"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["date_access"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.date_access = dtDocDate;
                            }
                            return View(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AccessUserLogDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessUserLogDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AccessUserLogList");
        }

        [AllowAnonymous]
        public JsonResult PortalUserLogDelete(FormCollection form)
        {
            try
            {

                if (form["id_access_userlog"] != null && form["id_access_userlog"] != "")
                {

                    PortalAccessUserLogModels Doc = new PortalAccessUserLogModels();
                    string sid_access_userlog = form["id_access_userlog"];


                    if (Doc.FunDelAccessUserLog(sid_access_userlog))
                    {
                        TempData["Successdata"] = "Portal User Log deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in PortalUserLogDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //---------------End Portal Accer User Log --------------------


        //---------------Start Portal Name--------------------
        [AllowAnonymous]
        public ActionResult AddProtalName()
        {
            PortalAccessUserLogModels objPortal = new PortalAccessUserLogModels();
            try
            {
                //ViewBag.PoralName = objPortal.GetPortalNameListBox();
                ViewBag.dsItems = objGlobaldata.Getdetails("select id_portal_ministry_name, portal_name, ministry_name, ministry_url from t_portal_ministry_name where active=1 order by portal_name asc");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddProtalName: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPortal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProtalName(PortalAccessUserLogModels objModels, FormCollection form)
        {
            try
            {
                if (objModels.FunAddPortalName(objModels))
                {
                    TempData["Successdata"] = "Added Portal Name details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddProtalName: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AddProtalName");
        }

        [HttpPost]
        public JsonResult GetPortalItems(string Header_id)
        {
            PortalAccessUserLogModels objPortal = new PortalAccessUserLogModels();
            MultiSelectList lstItems = objPortal.GetPortalDetailsList();

            return Json(lstItems);
        }

        [AllowAnonymous]
        public JsonResult UpdatePortalName(string id_portal_ministry_name, string portal_name, string ministry_name, string ministry_url)
        {
            PortalAccessUserLogModels objModels = new PortalAccessUserLogModels();

            try
            {
                objModels.id_portal_ministry_name = id_portal_ministry_name;
                objModels.portal_name = portal_name;
                objModels.ministry_name = ministry_name;
                objModels.ministry_url = ministry_url;

                if (objModels.FunUpdatePoratlName(objModels))
                {
                    TempData["Successdata"] = "Updated Portal details successfully";
                    return Json("Update Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UpdatePortalName: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Update Failed");
        }



        [HttpPost]
        public JsonResult DeletePortalName(string id_portal_ministry_name)
        {
            if (id_portal_ministry_name != "")
            {
                PortalAccessUserLogModels objModels = new PortalAccessUserLogModels();
                try
                {
                    if (objModels.FunDeletePortalName(id_portal_ministry_name))
                    {
                        TempData["Successdata"] = "Deleted Portal details successfully";
                        return Json("Deleted Successfully");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in DeletePortalName: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
          return Json("Delete Failed, Null Id");
        }

        //---------------End Portal Name--------------------
    }
}