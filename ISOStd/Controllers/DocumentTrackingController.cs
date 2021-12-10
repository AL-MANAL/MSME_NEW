using ISOStd.Filters;
using ISOStd.Models;
using PagedList;
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
    public class DocumentTrackingController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public DocumentTrackingController()
        {
            ViewBag.Menutype = "Documents";
            ViewBag.SubMenutype = "DocumentTracking";
        }

        public ActionResult AddDocTracking()
        {
            DocumentTrackingModels objDoc = new DocumentTrackingModels();
            try
            {
                objDoc.branch = objGlobaldata.GetCurrentUserSession().division;
                objDoc.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objDoc.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.DocType = objGlobaldata.GetDropdownList("Legal DocType");
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objDoc.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objDoc.branch);
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(objDoc.branch, objDoc.Department, objDoc.Location);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDocTracking: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objDoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDocTracking(DocumentTrackingModels objDoc, FormCollection form)
        {
            try
            {
                objDoc.Logged_by = objGlobaldata.GetCurrentUserSession().empid;
                //objDoc.branch = form["branch"];
                //if(objDoc.branch == null || objDoc.branch == "")
                //{
                //    objDoc.branch = objGlobaldata.GetCurrentUserSession().division;
                //}

                DocumentTrackingModelsList DocTrackList = new DocumentTrackingModelsList();
                DocTrackList.DocList = new List<DocumentTrackingModels>();
                DateTime dateValue;
                int c = Convert.ToInt16(form["itemcnt"]);
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    DocumentTrackingModels objDocTrack = new DocumentTrackingModels();
                    objDocTrack.branch = form["branch" + i];
                    if (objDocTrack.branch == null || objDocTrack.branch == "")
                    {
                        objDocTrack.branch = objGlobaldata.GetCurrentUserSession().division;
                    }
                    objDocTrack.Location = form["Location" + i];
                    objDocTrack.Department = form["Department" + i];
                    objDocTrack.doctype = form["doctype" + i];
                    objDocTrack.docname = form["docname" + i];
                    if (DateTime.TryParse(form["issue_date" + i], out dateValue) == true)
                    {
                        objDocTrack.issue_date = dateValue;
                    }
                    if (DateTime.TryParse(form["exp_date" + i], out dateValue) == true)
                    {
                        objDocTrack.exp_date = dateValue;
                    }
                    objDocTrack.issue_autority = form["issue_autority" + i];
                    objDocTrack.NotificationPerson = form["NotificationPerson" + i];
                    objDocTrack.upload = form["upload" + i];
                    objDocTrack.NotificationPeriod = form["NotificationPeriod" + i];
                    objDocTrack.NotificationValue = form["NotificationValue" + i];
                    int Notificationval = 1;

                    if (objDocTrack.NotificationPeriod == "None")
                    {
                        Notificationval = 0;
                        objDocTrack.NotificationValue = "0";
                    }
                    else if (objDocTrack.NotificationPeriod == "Weeks" && int.TryParse(objDoc.NotificationValue, out Notificationval))
                    {
                        Notificationval = 7 * Notificationval;
                    }
                    else if (objDocTrack.NotificationPeriod == "Months" && int.TryParse(objDoc.NotificationValue, out Notificationval))
                    {
                        Notificationval = 30 * Notificationval;
                    }
                    objDocTrack.NotificationDays = Notificationval;
                    DocTrackList.DocList.Add(objDocTrack);
                }

                if (objDoc.FunAddDocTracking(objDoc, DocTrackList))
                {
                    TempData["Successdata"] = "Document added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDocTracking: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DocTrackingList");
        }

        [AllowAnonymous]
        public ActionResult DocTrackingList(string branch_name)
        {
            DocumentTrackingModelsList objDocList = new DocumentTrackingModelsList();
            objDocList.DocList = new List<DocumentTrackingModels>();

            try
            {
                string sSearchtext = "";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_document_tracking,doctype,docname,issue_autority,issue_date,exp_date,NotificationPerson," +
                    "NotificationPeriod,NotificationValue,Logged_by,upload,branch,Department,Location from t_document_tracking where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by doctype asc";

                DataSet dsDocList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsDocList.Tables.Count > 0 && dsDocList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsDocList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            DocumentTrackingModels objTracModels = new DocumentTrackingModels
                            {
                                id_document_tracking = Convert.ToInt16(dsDocList.Tables[0].Rows[i]["id_document_tracking"].ToString()),
                                doctype = objGlobaldata.GetDropdownitemById(dsDocList.Tables[0].Rows[i]["doctype"].ToString()),
                                docname = dsDocList.Tables[0].Rows[i]["docname"].ToString(),
                                issue_autority = dsDocList.Tables[0].Rows[i]["issue_autority"].ToString(),
                                NotificationPerson = objGlobaldata.GetMultiHrEmpNameById(dsDocList.Tables[0].Rows[i]["NotificationPerson"].ToString()),
                                upload = (dsDocList.Tables[0].Rows[i]["upload"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsDocList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsDocList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsDocList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsDocList.Tables[0].Rows[i]["issue_date"].ToString(), out dtValue))
                            {
                                objTracModels.issue_date = dtValue;
                            }
                            if (DateTime.TryParse(dsDocList.Tables[0].Rows[i]["exp_date"].ToString(), out dtValue))
                            {
                                objTracModels.exp_date = dtValue;
                            }

                            objDocList.DocList.Add(objTracModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DocTrackingList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocTrackingList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objDocList.DocList.ToList());
        }

        [AllowAnonymous]
        public JsonResult DocTrackingListSearch(string branch_name)
        {
            DocumentTrackingModelsList objDocList = new DocumentTrackingModelsList();
            objDocList.DocList = new List<DocumentTrackingModels>();

            try
            {
                string sSearchtext = "";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_document_tracking,doctype,docname,issue_autority,issue_date,exp_date,NotificationPerson," +
                    "NotificationPeriod,NotificationValue,Logged_by,upload,branch,Department,Location from t_document_tracking where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by doctype asc";

                DataSet dsDocList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsDocList.Tables.Count > 0 && dsDocList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsDocList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            DocumentTrackingModels objTracModels = new DocumentTrackingModels
                            {
                                id_document_tracking = Convert.ToInt16(dsDocList.Tables[0].Rows[i]["id_document_tracking"].ToString()),
                                doctype = objGlobaldata.GetDropdownitemById(dsDocList.Tables[0].Rows[i]["doctype"].ToString()),
                                docname = dsDocList.Tables[0].Rows[i]["docname"].ToString(),
                                issue_autority = dsDocList.Tables[0].Rows[i]["issue_autority"].ToString(),
                                NotificationPerson = objGlobaldata.GetMultiHrEmpNameById(dsDocList.Tables[0].Rows[i]["NotificationPerson"].ToString()),
                                upload = (dsDocList.Tables[0].Rows[i]["upload"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsDocList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsDocList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsDocList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsDocList.Tables[0].Rows[i]["issue_date"].ToString(), out dtValue))
                            {
                                objTracModels.issue_date = dtValue;
                            }
                            if (DateTime.TryParse(dsDocList.Tables[0].Rows[i]["exp_date"].ToString(), out dtValue))
                            {
                                objTracModels.exp_date = dtValue;
                            }

                            objDocList.DocList.Add(objTracModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DocTrackingListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocTrackingListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult DocTrackingEdit()
        {
            DocumentTrackingModels objTracModels = new DocumentTrackingModels();
            try
            {
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DocType = objGlobaldata.GetDropdownList("Legal DocType");
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                if (Request.QueryString["id_document_tracking"] != null && Request.QueryString["id_document_tracking"] != "")
                {
                    string sid_document_tracking = Request.QueryString["id_document_tracking"];

                    string sSqlstmt = "select id_document_tracking,doctype,docname,issue_autority,issue_date,exp_date,NotificationPerson,upload," +
                   "NotificationPeriod,NotificationValue,Logged_by,branch,Department,Location from t_document_tracking where id_document_tracking='" + sid_document_tracking + "'";

                    DataSet dsDocList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsDocList.Tables.Count > 0 && dsDocList.Tables[0].Rows.Count > 0)
                    {
                        objTracModels = new DocumentTrackingModels
                        {
                            id_document_tracking = Convert.ToInt16(dsDocList.Tables[0].Rows[0]["id_document_tracking"].ToString()),
                            doctype = objGlobaldata.GetDropdownitemById(dsDocList.Tables[0].Rows[0]["doctype"].ToString()),
                            docname = dsDocList.Tables[0].Rows[0]["docname"].ToString(),
                            issue_autority = dsDocList.Tables[0].Rows[0]["issue_autority"].ToString(),
                            NotificationPerson = objGlobaldata.GetMultiHrEmpNameById(dsDocList.Tables[0].Rows[0]["NotificationPerson"].ToString()),
                            upload = (dsDocList.Tables[0].Rows[0]["upload"].ToString()),
                            NotificationPeriod = dsDocList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsDocList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            branch = dsDocList.Tables[0].Rows[0]["branch"].ToString(),
                            Department = (dsDocList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = (dsDocList.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsDocList.Tables[0].Rows[0]["issue_date"].ToString(), out dtValue))
                        {
                            objTracModels.issue_date = dtValue;
                        }
                        if (DateTime.TryParse(dsDocList.Tables[0].Rows[0]["exp_date"].ToString(), out dtValue))
                        {
                            objTracModels.exp_date = dtValue;
                        }
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsDocList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsDocList.Tables[0].Rows[0]["branch"].ToString());
                        //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsDocList.Tables[0].Rows[0]["branch"].ToString(), dsDocList.Tables[0].Rows[0]["Department"].ToString(), dsDocList.Tables[0].Rows[0]["Location"].ToString());
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("DocTrackingList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("DocTrackingList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocTrackingEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("DocTrackingList");
            }
            return View(objTracModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DocTrackingEdit(DocumentTrackingModels objDocTrack, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objDocTrack.doctype = form["doctype"];
                objDocTrack.docname = form["docname"];
                objDocTrack.Department = form["Department"];
                objDocTrack.Location = form["Location"];
                objDocTrack.branch = form["branch"];
                if (objDocTrack.branch == null || objDocTrack.branch == "")
                {
                    objDocTrack.branch = objGlobaldata.GetCurrentUserSession().division;
                }
                DateTime dateValue;
                if (DateTime.TryParse(form["issue_date"], out dateValue) == true)
                {
                    objDocTrack.issue_date = dateValue;
                }
                if (DateTime.TryParse(form["exp_date"], out dateValue) == true)
                {
                    objDocTrack.exp_date = dateValue;
                }
                objDocTrack.issue_autority = form["issue_autority"];
                objDocTrack.NotificationPerson = form["NotificationPerson"];

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (upload != null && files.ContentLength > 0)
                {
                    objDocTrack.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/DocTracking"), Path.GetFileName(file.FileName));
                            string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objDocTrack.upload = objDocTrack.upload + "," + "~/DataUpload/MgmtDocs/DocTracking/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objDocTrack.upload = objDocTrack.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objDocTrack.upload = objDocTrack.upload + "," + form["QCDocsVal"];
                    objDocTrack.upload = objDocTrack.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    //objDocTrack.upload = null;
                    TempData["alertdata"] = "Error,No Files Were Selected";
                    return RedirectToAction("DocTrackingList");
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    //objDocTrack.upload = null;
                    TempData["alertdata"] = "Error,No Files Were Selected";
                    return RedirectToAction("DocTrackingList");
                }

                int Notificationval = 1;

                if (objDocTrack.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objDocTrack.NotificationValue = "0";
                }
                else if (objDocTrack.NotificationPeriod == "Weeks" && int.TryParse(objDocTrack.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objDocTrack.NotificationPeriod == "Months" && int.TryParse(objDocTrack.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objDocTrack.NotificationDays = Notificationval;

                if (objDocTrack.FunUpdateDocTracking(objDocTrack))
                {
                    TempData["Successdata"] = "Document Tracking is updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocTrackingEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("DocTrackingList");
            }

            return RedirectToAction("DocTrackingList");
        }

        [AllowAnonymous]
        public ActionResult DocTrackingsHistoryList(string SearchText, string id_document_tracking, int? page)
        {
            DocumentTrackingModelsList objDocList = new DocumentTrackingModelsList();
            objDocList.DocList = new List<DocumentTrackingModels>();

            try
            {
                string sSqlstmt = "select id_document_tracking_trans,id_document_tracking,doctype,docname,issue_autority,issue_date,exp_date,NotificationPerson," +
                    "NotificationPeriod,NotificationValue,Logged_by,upload from t_document_tracking_trans where id_document_tracking='" + id_document_tracking + "'";

                DataSet dsDocList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsDocList.Tables.Count > 0 && dsDocList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsDocList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            DocumentTrackingModels objTracModels = new DocumentTrackingModels
                            {
                                id_document_tracking_trans = Convert.ToInt16(dsDocList.Tables[0].Rows[i]["id_document_tracking_trans"].ToString()),
                                id_document_tracking = Convert.ToInt16(dsDocList.Tables[0].Rows[i]["id_document_tracking"].ToString()),
                                doctype = objGlobaldata.GetDropdownitemById(dsDocList.Tables[0].Rows[i]["doctype"].ToString()),
                                docname = dsDocList.Tables[0].Rows[i]["docname"].ToString(),
                                issue_autority = dsDocList.Tables[0].Rows[i]["issue_autority"].ToString(),
                                NotificationPerson = objGlobaldata.GetMultiHrEmpNameById(dsDocList.Tables[0].Rows[i]["NotificationPerson"].ToString()),
                                upload = (dsDocList.Tables[0].Rows[i]["upload"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsDocList.Tables[0].Rows[i]["issue_date"].ToString(), out dtValue))
                            {
                                objTracModels.issue_date = dtValue;
                            }
                            if (DateTime.TryParse(dsDocList.Tables[0].Rows[i]["exp_date"].ToString(), out dtValue))
                            {
                                objTracModels.exp_date = dtValue;
                            }

                            objDocList.DocList.Add(objTracModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DocTrackingList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocTrackingList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            ViewBag.id_document_tracking = id_document_tracking;

            return View(objDocList.DocList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult DocTrackingDetail()
        {
            DocumentTrackingModels objTracModels = new DocumentTrackingModels();
            try
            {
                if (Request.QueryString["id_document_tracking"] != null && Request.QueryString["id_document_tracking"] != "")
                {
                    string sid_document_tracking = Request.QueryString["id_document_tracking"];

                    string sSqlstmt = "select id_document_tracking,doctype,docname,issue_autority,issue_date,exp_date,NotificationPerson,upload," +
                   "NotificationPeriod,NotificationValue,Logged_by,branch,Department,Location from t_document_tracking where id_document_tracking='" + sid_document_tracking + "'";

                    DataSet dsDocList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsDocList.Tables.Count > 0 && dsDocList.Tables[0].Rows.Count > 0)
                    {
                        objTracModels = new DocumentTrackingModels
                        {
                            id_document_tracking = Convert.ToInt16(dsDocList.Tables[0].Rows[0]["id_document_tracking"].ToString()),
                            doctype = objGlobaldata.GetDropdownitemById(dsDocList.Tables[0].Rows[0]["doctype"].ToString()),
                            docname = dsDocList.Tables[0].Rows[0]["docname"].ToString(),
                            issue_autority = dsDocList.Tables[0].Rows[0]["issue_autority"].ToString(),
                            NotificationPerson = objGlobaldata.GetMultiHrEmpNameById(dsDocList.Tables[0].Rows[0]["NotificationPerson"].ToString()),
                            upload = (dsDocList.Tables[0].Rows[0]["upload"].ToString()),
                            NotificationPeriod = dsDocList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsDocList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsDocList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsDocList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsDocList.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsDocList.Tables[0].Rows[0]["issue_date"].ToString(), out dtValue))
                        {
                            objTracModels.issue_date = dtValue;
                        }
                        if (DateTime.TryParse(dsDocList.Tables[0].Rows[0]["exp_date"].ToString(), out dtValue))
                        {
                            objTracModels.exp_date = dtValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("DocTrackingList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("DocTrackingList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocTrackingDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("DocTrackingList");
            }
            return View(objTracModels);
        }

        [AllowAnonymous]
        public ActionResult DocTrackingInfo(int id)
        {
            DocumentTrackingModels objTracModels = new DocumentTrackingModels();
            try
            {
                string sSqlstmt = "select id_document_tracking,doctype,docname,issue_autority,issue_date,exp_date,NotificationPerson,upload," +
                 "NotificationPeriod,NotificationValue,Logged_by,branch,Department,Location from t_document_tracking where id_document_tracking='" + id + "'";

                DataSet dsDocList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsDocList.Tables.Count > 0 && dsDocList.Tables[0].Rows.Count > 0)
                {
                    objTracModels = new DocumentTrackingModels
                    {
                        id_document_tracking = Convert.ToInt16(dsDocList.Tables[0].Rows[0]["id_document_tracking"].ToString()),
                        doctype = objGlobaldata.GetDropdownitemById(dsDocList.Tables[0].Rows[0]["doctype"].ToString()),
                        docname = dsDocList.Tables[0].Rows[0]["docname"].ToString(),
                        issue_autority = dsDocList.Tables[0].Rows[0]["issue_autority"].ToString(),
                        NotificationPerson = objGlobaldata.GetMultiHrEmpNameById(dsDocList.Tables[0].Rows[0]["NotificationPerson"].ToString()),
                        upload = (dsDocList.Tables[0].Rows[0]["upload"].ToString()),
                        NotificationPeriod = dsDocList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                        NotificationValue = dsDocList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsDocList.Tables[0].Rows[0]["branch"].ToString()),
                        Department = objGlobaldata.GetMultiDeptNameById(dsDocList.Tables[0].Rows[0]["Department"].ToString()),
                        Location = objGlobaldata.GetDivisionLocationById(dsDocList.Tables[0].Rows[0]["Location"].ToString()),
                    };

                    DateTime dtValue;
                    if (DateTime.TryParse(dsDocList.Tables[0].Rows[0]["issue_date"].ToString(), out dtValue))
                    {
                        objTracModels.issue_date = dtValue;
                    }
                    if (DateTime.TryParse(dsDocList.Tables[0].Rows[0]["exp_date"].ToString(), out dtValue))
                    {
                        objTracModels.exp_date = dtValue;
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("DocTrackingList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocTrackingInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("DocTrackingList");
            }
            return View(objTracModels);
        }

        [AllowAnonymous]
        public JsonResult DocTrackingDelete(FormCollection form)
        {
            try
            {
                if (form["id_document_tracking"] != null && form["id_document_tracking"] != "")
                {
                    DocumentTrackingModels Doc = new DocumentTrackingModels();
                    string sid_document_tracking = form["id_document_tracking"];
                    if (Doc.FunDeleteDocTracking(sid_document_tracking))
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
                objGlobaldata.AddFunctionalLog("Exception in DocTrackingDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase upload = Request.Files;
            DocumentTrackingModels obj = new DocumentTrackingModels();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/DocTracking"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + sFilename);
                //return Json("~/DataUpload/MgmtDocs/Surveillance/" + "Surveillance" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                obj.upload = obj.upload + "," + "~/DataUpload/MgmtDocs/DocTracking/" + sFilename;
            }
            obj.upload = obj.upload.Trim(',');
            return Json(obj.upload);
        }
    }
}