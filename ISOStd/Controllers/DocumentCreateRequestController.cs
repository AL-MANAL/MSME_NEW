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
    public class DocumentCreateRequestController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public DocumentCreateRequestController()
        {
            ViewBag.Menutype = "Documents";
            ViewBag.SubMenutype = "CreateRequest";
        }

        [AllowAnonymous]
        public ActionResult AddDocCreateRequest()
        {
            DocumentCreateRequestModels objRequest = new DocumentCreateRequestModels();
            try
            {
                ViewBag.SubMenutype = "CreateRequest";
                
                objRequest.division = objGlobaldata.GetCurrentUserSession().division;
                objRequest.department = objGlobaldata.GetCurrentUserSession().DeptID;
                objRequest.location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
               // ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
               // ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList();
               // ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentList1(objRequest.division);
                ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(objRequest.division);

                ViewBag.IMSGroup = objGlobaldata.GetIMSGroupRole();
               // ViewBag.IMSGroup = objGlobaldata.GetGRolelikeList(objRequest.division, objRequest.department, objRequest.location, "%IMS% Rep%");

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDocCreateRequest: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRequest);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddDocCreateRequest(DocumentCreateRequestModels objRequest, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                //objRequest.division = form["division"];                

                DateTime dateValue;
                if (DateTime.TryParse(form["date_request"], out dateValue) == true)
                {
                    objRequest.date_request = dateValue;
                }
               
                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objRequest.checkedby = objRequest.checkedby + "," + form["empno " + i];
                    }
                }
                if (objRequest.checkedby != null)
                {
                    objRequest.checkedby = objRequest.checkedby.Trim(',');
                }
                HttpPostedFileBase files = Request.Files[0];
                if (upload != null && files.ContentLength > 0)
                {
                    objRequest.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/DocCreateRequest"), Path.GetFileName(file.FileName));
                            string sFilename = "DocCreateRequest" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objRequest.upload = objRequest.upload + "," + "~/DataUpload/MgmtDocs/DocCreateRequest/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddDocCreateRequest-upload: " + ex.ToString());
                        }
                    }
                    objRequest.upload = objRequest.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objRequest.FunAddDocCreateRequest(objRequest))
                {
                    TempData["Successdata"] = "Added Document Create Request details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDocCreateRequest: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DocCreateRequestList");
        }

        [AllowAnonymous]
        public ActionResult DocCreateRequestList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "CreateRequest";

            DocCreateRequestModelsList objPortalList = new DocCreateRequestModelsList();
            objPortalList.RequestList = new List<DocumentCreateRequestModels>();

            DocumentCreateRequestModels objRequest = new DocumentCreateRequestModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_doc_request,dcr_no,date_request,division,`department`,location,reason,upload,checkedby,doc_status as doc_statusId," +
                       "case when doc_status = '0' then 'Pending' when doc_status = '1' then 'Checked by Department Head' when doc_status = '2' then 'Approved' when doc_status = '3' then 'Rejected' end" +
                       " as doc_status from t_document_create_request where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', division)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by id_doc_request desc";
                
                DataSet dsCreateList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsCreateList.Tables.Count > 0 && dsCreateList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCreateList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            DocumentCreateRequestModels objPartiesModels = new DocumentCreateRequestModels
                            {
                                id_doc_request = dsCreateList.Tables[0].Rows[i]["id_doc_request"].ToString(),
                                dcr_no = dsCreateList.Tables[0].Rows[i]["dcr_no"].ToString(),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsCreateList.Tables[0].Rows[i]["division"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsCreateList.Tables[0].Rows[i]["department"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsCreateList.Tables[0].Rows[i]["location"].ToString()),
                                reason = (dsCreateList.Tables[0].Rows[i]["reason"].ToString()),
                                upload = dsCreateList.Tables[0].Rows[i]["upload"].ToString(),
                                checkedby = objGlobaldata.GetMultiHrEmpNameById(dsCreateList.Tables[0].Rows[i]["checkedby"].ToString()),
                                doc_status = dsCreateList.Tables[0].Rows[i]["doc_status"].ToString(),
                                doc_statusId = dsCreateList.Tables[0].Rows[i]["doc_status"].ToString(),
                            };

                            DateTime dtDocDate;
                            if (dsCreateList.Tables[0].Rows[i]["date_request"].ToString() != ""
                               && DateTime.TryParse(dsCreateList.Tables[0].Rows[i]["date_request"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.date_request = dtDocDate;
                            }
                           
                            objPortalList.RequestList.Add(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPortalList.RequestList.ToList());
        }
        
        [AllowAnonymous]
        public ActionResult DocCreateRequestEdit()
        {
            ViewBag.SubMenutype = "CreateRequest";

            DocCreateRequestModelsList objPortalList = new DocCreateRequestModelsList();
            objPortalList.RequestList = new List<DocumentCreateRequestModels>();

            DocumentCreateRequestModels objRequest = new DocumentCreateRequestModels();
            try
            {
                string sid_doc_request = Request.QueryString["id_doc_request"];

                if (sid_doc_request != null && sid_doc_request != "")
                {
                    string sSqlstmt = "select id_doc_request,dcr_no,date_request,division,`department`,location,reason,upload,checkedby,doc_status as doc_statusId," +
                           "case when doc_status = '0' then 'Pending' when doc_status = '1' then 'Checked by Department Head' when doc_status = '2' then 'Approved' when doc_status = '3' then 'Rejected' end" +
                           " as doc_status from t_document_create_request where Active=1 and id_doc_request= '" + sid_doc_request + "'";

                    DataSet dsCreateList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCreateList.Tables.Count > 0 && dsCreateList.Tables[0].Rows.Count > 0)
                    {
                        try
                            {
                                 objRequest = new DocumentCreateRequestModels
                                {
                                    id_doc_request = dsCreateList.Tables[0].Rows[0]["id_doc_request"].ToString(),
                                    dcr_no = dsCreateList.Tables[0].Rows[0]["dcr_no"].ToString(),
                                    division = (dsCreateList.Tables[0].Rows[0]["division"].ToString()),
                                    department =(dsCreateList.Tables[0].Rows[0]["department"].ToString()),
                                     location = (dsCreateList.Tables[0].Rows[0]["location"].ToString()),
                                     reason = (dsCreateList.Tables[0].Rows[0]["reason"].ToString()),
                                    upload = dsCreateList.Tables[0].Rows[0]["upload"].ToString(),
                                    checkedby =/* objGlobaldata.GetMultiHrEmpNameById*/(dsCreateList.Tables[0].Rows[0]["checkedby"].ToString()),
                                    doc_status = dsCreateList.Tables[0].Rows[0]["doc_status"].ToString(),
                                    doc_statusId = dsCreateList.Tables[0].Rows[0]["doc_status"].ToString(),
                                };

                                DateTime dtDocDate;
                                if (dsCreateList.Tables[0].Rows[0]["date_request"].ToString() != ""
                                   && DateTime.TryParse(dsCreateList.Tables[0].Rows[0]["date_request"].ToString(), out dtDocDate))
                                {
                                    objRequest.date_request = dtDocDate;
                                }
                            ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                            ViewBag.Department = objGlobaldata.GetDepartmentList1(dsCreateList.Tables[0].Rows[0]["division"].ToString());
                            ViewBag.location = objGlobaldata.GetLocationbyMultiDivision(dsCreateList.Tables[0].Rows[0]["division"].ToString());
                            //ViewBag.IMSGroup = objGlobaldata.GetGRolelikeList(dsCreateList.Tables[0].Rows[0]["division"].ToString(), dsCreateList.Tables[0].Rows[0]["department"].ToString(), dsCreateList.Tables[0].Rows[0]["location"].ToString(), "%IMS%Rep%");
                            ViewBag.IMSGroup = objGlobaldata.GetIMSGroupRole();

                            objPortalList.RequestList.Add(objRequest);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                     }

                    return View(objRequest);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DocCreateRequestList");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult DocCreateRequestEdit(DocumentCreateRequestModels objRequest, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objRequest.reason = form["reason"];

                //DateTime dateValue;
                //if (DateTime.TryParse(form["date_request"], out dateValue) == true)
                //{
                //    objRequest.date_request = dateValue;
                //}

                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objRequest.checkedby = objRequest.checkedby + "," + form["empno " + i];
                    }
                }
                if (objRequest.checkedby != null)
                {
                    objRequest.checkedby = objRequest.checkedby.Trim(',');
                }
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (upload != null && files.ContentLength > 0)
                {
                    objRequest.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/DocCreateRequest"), Path.GetFileName(file.FileName));
                            string sFilename = "DocCreateRequest" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objRequest.upload = objRequest.upload + "," + "~/DataUpload/MgmtDocs/DocCreateRequest/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddDocCreateRequest-upload: " + ex.ToString());
                        }
                    }
                    objRequest.upload = objRequest.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objRequest.upload = objRequest.upload + "," + form["QCDocsVal"];
                    objRequest.upload = objRequest.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objRequest.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objRequest.upload = null;
                }

                if (objRequest.FunUpdateDocCreateRequest(objRequest))
                {
                    TempData["Successdata"] = "Added Document Create Request details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DocCreateRequestList");
        }

        [AllowAnonymous]
        public ActionResult DocCreateRequestDetails()
        {
            ViewBag.SubMenutype = "CreateRequest";

            DocCreateRequestModelsList objPortalList = new DocCreateRequestModelsList();
            objPortalList.RequestList = new List<DocumentCreateRequestModels>();

            DocumentCreateRequestModels objRequest = new DocumentCreateRequestModels();
            try
            {
                string sid_doc_request = Request.QueryString["id_doc_request"];

                if (sid_doc_request != null && sid_doc_request != "")
                {
                    string sSqlstmt = "select id_doc_request,dcr_no,date_request,division,`department`,location,reason,upload,checkedby,doc_status as doc_statusId," +
                           "case when doc_status = '0' then 'Pending' when doc_status = '1' then 'Checked by Department Head' when doc_status = '2' then 'Approved' when doc_status = '3' then 'Rejected' end" +
                           " as doc_status,checklist_id,agreed,comments,doc_control,doc_level,doc_title,serial_no,new_doc_ref," +
                           "checkedby_approve_date,controller_approve_date from t_document_create_request where Active=1 " +
                           "and id_doc_request= '" + sid_doc_request + "'";

                    DataSet dsCreateList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCreateList.Tables.Count > 0 && dsCreateList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objRequest = new DocumentCreateRequestModels
                            {
                                id_doc_request = dsCreateList.Tables[0].Rows[0]["id_doc_request"].ToString(),
                                dcr_no = dsCreateList.Tables[0].Rows[0]["dcr_no"].ToString(),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsCreateList.Tables[0].Rows[0]["division"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsCreateList.Tables[0].Rows[0]["department"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsCreateList.Tables[0].Rows[0]["location"].ToString()),
                                reason = (dsCreateList.Tables[0].Rows[0]["reason"].ToString()),
                                upload = dsCreateList.Tables[0].Rows[0]["upload"].ToString(),
                                checkedby = objGlobaldata.GetMultiHrEmpNameById(dsCreateList.Tables[0].Rows[0]["checkedby"].ToString()),
                                doc_status = dsCreateList.Tables[0].Rows[0]["doc_status"].ToString(),
                                doc_statusId = dsCreateList.Tables[0].Rows[0]["doc_statusId"].ToString(),
                                checklist_id = objGlobaldata.GetDCRChecklistById(dsCreateList.Tables[0].Rows[0]["checklist_id"].ToString()),
                                agreed = dsCreateList.Tables[0].Rows[0]["agreed"].ToString(),
                                comments = dsCreateList.Tables[0].Rows[0]["comments"].ToString(),
                                doc_control = objGlobaldata.GetMultiHrEmpNameById(dsCreateList.Tables[0].Rows[0]["doc_control"].ToString()),
                                //doc_level = objGlobaldata.GetDocLevelNameByID(dsCreateList.Tables[0].Rows[0]["doc_level"].ToString()),
                                //doc_title = dsCreateList.Tables[0].Rows[0]["doc_title"].ToString(),
                                serial_no = dsCreateList.Tables[0].Rows[0]["serial_no"].ToString(),
                                new_doc_ref = dsCreateList.Tables[0].Rows[0]["new_doc_ref"].ToString(),             
                            };

                            DateTime dtDocDate;
                            if (dsCreateList.Tables[0].Rows[0]["date_request"].ToString() != ""
                               && DateTime.TryParse(dsCreateList.Tables[0].Rows[0]["date_request"].ToString(), out dtDocDate))
                            {
                                objRequest.date_request = dtDocDate;
                            }
                            if (dsCreateList.Tables[0].Rows[0]["checkedby_approve_date"].ToString() != ""
                              && DateTime.TryParse(dsCreateList.Tables[0].Rows[0]["checkedby_approve_date"].ToString(), out dtDocDate))
                            {
                                objRequest.checkedby_approve_date = dtDocDate;
                            }
                            if (dsCreateList.Tables[0].Rows[0]["controller_approve_date"].ToString() != ""
                              && DateTime.TryParse(dsCreateList.Tables[0].Rows[0]["controller_approve_date"].ToString(), out dtDocDate))
                            {
                                objRequest.controller_approve_date = dtDocDate;
                            }
                          
                            objPortalList.RequestList.Add(objRequest);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    return View(objRequest);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DocCreateRequestList");
        }

        [AllowAnonymous]
        public ActionResult DCRApporveDetails(string id_doc_request)
        {
            ViewBag.SubMenutype = "CreateRequest";
            DocumentCreateRequestModels objRequest = new DocumentCreateRequestModels();

          if (id_doc_request != null && id_doc_request != "")
           { 
             try
              {
                   // ViewBag.CheckList = objGlobaldata.GetDCRChecklist();
                    ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                    // ViewBag.DocLevel = objGlobaldata.GetDocLevel();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbyDesignation("%As%manager%HSE%");
                    //ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbyDesignation("Assistant manager HSE");

                    string sSqlstmt = "select id_doc_request,dcr_no,date_request,division,`department`,location,reason,upload,checkedby,doc_status as doc_statusId," +
                       "case when doc_status = '0' then 'Pending' when doc_status = '1' then 'Checked by Department Head' when doc_status = '2' then 'Approved' when doc_status = '3' then 'Rejected' end" +
                       " as doc_status,checklist_id,agreed,comments,doc_control from t_document_create_request where Active=1 and id_doc_request= '" + id_doc_request + "'order by id_doc_request desc";

                DataSet dsCreateList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsCreateList.Tables.Count > 0 && dsCreateList.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        objRequest = new DocumentCreateRequestModels
                        {
                            id_doc_request = dsCreateList.Tables[0].Rows[0]["id_doc_request"].ToString(),
                            dcr_no = dsCreateList.Tables[0].Rows[0]["dcr_no"].ToString(),
                            division = objGlobaldata.GetMultiCompanyBranchNameById(dsCreateList.Tables[0].Rows[0]["division"].ToString()),
                            department = objGlobaldata.GetMultiDeptNameById(dsCreateList.Tables[0].Rows[0]["department"].ToString()),
                            divisionId = (dsCreateList.Tables[0].Rows[0]["division"].ToString()),
                            departmentId = (dsCreateList.Tables[0].Rows[0]["department"].ToString()),
                            location =objGlobaldata.GetDivisionLocationById(dsCreateList.Tables[0].Rows[0]["location"].ToString()),
                            reason = (dsCreateList.Tables[0].Rows[0]["reason"].ToString()),
                            upload = dsCreateList.Tables[0].Rows[0]["upload"].ToString(),
                            checkedby = objGlobaldata.GetMultiHrEmpNameById(dsCreateList.Tables[0].Rows[0]["checkedby"].ToString()),
                            doc_status = dsCreateList.Tables[0].Rows[0]["doc_status"].ToString(),
                            doc_statusId = dsCreateList.Tables[0].Rows[0]["doc_statusId"].ToString(),
                            checklist_id = objGlobaldata.GetDCRChecklistById(dsCreateList.Tables[0].Rows[0]["checklist_id"].ToString()),
                            agreed = dsCreateList.Tables[0].Rows[0]["agreed"].ToString(),
                            comments = dsCreateList.Tables[0].Rows[0]["comments"].ToString(),
                            doc_control = dsCreateList.Tables[0].Rows[0]["doc_control"].ToString(),
                            doc_controlName = objGlobaldata.GetMultiHrEmpNameById(dsCreateList.Tables[0].Rows[0]["doc_control"].ToString()),
                        };

                        DateTime dtDocDate;
                        if (dsCreateList.Tables[0].Rows[0]["date_request"].ToString() != ""
                           && DateTime.TryParse(dsCreateList.Tables[0].Rows[0]["date_request"].ToString(), out dtDocDate))
                        {
                           objRequest.date_request = dtDocDate;
                        }
                           ViewBag.Department = objGlobaldata.GetDepartmentList1(dsCreateList.Tables[0].Rows[0]["division"].ToString());
                           ViewBag.Location = objGlobaldata.GetDivisionLocationList(dsCreateList.Tables[0].Rows[0]["location"].ToString());
                           //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsCreateList.Tables[0].Rows[0]["division"].ToString(), dsCreateList.Tables[0].Rows[0]["department"].ToString(), dsCreateList.Tables[0].Rows[0]["location"].ToString());
                        }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                    return View(objRequest);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
           }
            return RedirectToAction("ListPendingForApproval", "Dashboard");
        }

        [AllowAnonymous]
        public JsonResult DocCreateRequestDelete(FormCollection form)
        {
            try
            {
                if (form["id_doc_request"] != null && form["id_doc_request"] != "")
                {
                    DocumentCreateRequestModels Doc = new DocumentCreateRequestModels();

                    string sid_doc_request = form["id_doc_request"];

                    if (Doc.FunDelDocCreateRequest(sid_doc_request))
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
                objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public ActionResult DCRCheckecbyApporveDetails(DocumentCreateRequestModels objRequest, FormCollection form)
        {
            try
            {
                objRequest.doc_status = form["doc_status_checkedby"];
                objRequest.checklist_id = objGlobaldata.GetChklistIdByRef(form["checklist_id"]);
                string stat = "";

                if (objRequest.FunAddDCRCheckedbyApprove(objRequest))
                {
                    if (objRequest.doc_status == "1")
                    {
                        stat = "Approved";
                    }
                    else if (objRequest.doc_status == "3")
                    {
                        stat = "Rejected";
                    }
                    TempData["Successdata"] = "Document Create Request " +stat+" successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DCRCheckecbyApporveDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult DCRControllerApporveDetails(DocumentCreateRequestModels objRequest, FormCollection form)
        {
            try
            {
                objRequest.doc_status = form["doc_status_conroller"];
                string stat = "";
                if (objRequest.FunAddDCRControllerApprove(objRequest))
                {
                    if(objRequest.doc_status == "2")
                    {
                        stat = "Approved";
                    }
                    else if (objRequest.doc_status == "3")
                    {
                        stat = "Rejected";
                    }
                    TempData["Successdata"] = "Document Create Request "+stat+" successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DCRControllerApporveDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }


        //From Layout
        public JsonResult DocCreateRequestApproveNoty()
        {
            return Json("Success");
        }

        //[AllowAnonymous]
        //public JsonResult GetDocLevelGroup(string DocLevelId)
        //{
        //    return Json(objGlobaldata.GetDocLevelGroupType(DocLevelId));
        //}

        //[AllowAnonymous]
        //public JsonResult GetDocLevelTeam(string DocLevelId)
        //{
        //    return Json(objGlobaldata.GetDocLevelTeamType(DocLevelId));
        //}

        public JsonResult FunGetEmpDetails(string semp_no)
        {

            NCModels objModels = new NCModels();
            try
            {
                string sSqlstmt = "select emp_no,emp_id,division,Emp_work_location,Dept_Id,EmailId,MobileNo from t_hr_employee where emp_no = '" + semp_no + "'";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    objModels = new NCModels
                    {
                        emp_id = (dsList.Tables[0].Rows[0]["emp_id"].ToString()),
                        empname = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["emp_no"].ToString()),
                       // division = objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[0]["division"].ToString()),
                       // location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[0]["Emp_work_location"].ToString()),
                        department = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["Dept_Id"].ToString()),
                        EmailId = (dsList.Tables[0].Rows[0]["EmailId"].ToString()),
                        MobileNo = (dsList.Tables[0].Rows[0]["MobileNo"].ToString()),
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

        //Checklist Generation

        public ActionResult GenerateDCRChecklist()
        {
            ViewBag.SubMenutype = "chklist";
            string sid_doc_request = Request.QueryString["id_doc_request"];

            DCRChecklistModels ObjCheck = new DCRChecklistModels();      
            try
            {
                ViewBag.DCRQuestions = ObjCheck.GetDCRQuestionList();
                ViewBag.DCRSection = ObjCheck.GetDCRSectionList();
                ViewBag.Revision = objGlobaldata.GetDropdownList("Revision Type");
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");

             
                 if (sid_doc_request != "" && sid_doc_request != null)
                {                    
                    ViewBag.dcrf_no = objGlobaldata.GetDCRNoByDocId(sid_doc_request);
                    ViewBag.checklistRef = "ChkList-" + ViewBag.dcrf_no;
                }
                if(ViewBag.checklistRef != "")
                {
                    if (objGlobaldata.checkDCRChkListRefExists(ViewBag.checklistRef))
                    {
                        return View(ObjCheck);
                    }                         
                }
            
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateDCRChecklist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DCRApporveDetails", "DocumentCreateRequest", new { id_doc_request = sid_doc_request });
      }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateDCRChecklist(DCRChecklistModels ObjCheck, FormCollection form)
        {
            try
            {

                //DateTime dateValue;
                //if (DateTime.TryParse(form["Inspection_date"], out dateValue) == true)
                //{
                //    objInspChecklist.Inspection_date = dateValue;
                //}                

                DCRChecklistModelsList ObjModelList = new DCRChecklistModelsList();
                ObjModelList.DCRChkList = new List <DCRChecklistModels> ();

                MultiSelectList DCRQuestions = ObjCheck.GetDCRQuestionList();

                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in DCRQuestions)
                {
                    if (i <= Convert.ToInt16(form["itmctn"]))
                    {
                        DCRChecklistModels objElements = new DCRChecklistModels();
                        objElements.id_questions = form["id_questions " + item.Value];
                        objElements.ratings = form["ratings " + item.Value];
                        objElements.comments = form["comments " + i];
                        ObjModelList.DCRChkList.Add(objElements);
                    }
                    i++;
                }

                if (ObjCheck.FunAddDCRChecklist(ObjCheck, ObjModelList))
                {
                    TempData["Successdata"] = "DCR Created Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateDCRChecklist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DCRChecklistList");
        }

        [AllowAnonymous]
        public ActionResult DCRChecklistList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "chklist";

            DCRChecklistModelsList objchkList = new DCRChecklistModelsList();
            objchkList.DCRChkList = new List<DCRChecklistModels>();

            DCRChecklistModels objchklist = new DCRChecklistModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
               // string sSearchtext = "";

                string sSqlstmt = "select id_chklist,checklistRef,revision,doc_title,dcrf_no,memo_ref," +
                    "logged_by from t_document_create_request_chklist where Active=1";

                //if (branch_name != null && branch_name != "")
                //{
                //    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', division)";
                //    ViewBag.Branch_name = branch_name;
                //}
                //else
                //{
                //    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', division)";
                //}
                //sSqlstmt = sSqlstmt + sSearchtext + " order by id_chklist desc";

                DataSet dsCreateList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsCreateList.Tables.Count > 0 && dsCreateList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCreateList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                           objchklist = new DCRChecklistModels
                           {
                                id_chklist = dsCreateList.Tables[0].Rows[i]["id_chklist"].ToString(),
                                checklistRef = dsCreateList.Tables[0].Rows[i]["checklistRef"].ToString(),
                                revision = objGlobaldata.GetDropdownitemById(dsCreateList.Tables[0].Rows[i]["revision"].ToString()),
                                doc_title = dsCreateList.Tables[0].Rows[i]["doc_title"].ToString(),
                                dcrf_no = dsCreateList.Tables[0].Rows[i]["dcrf_no"].ToString(),
                                memo_ref = dsCreateList.Tables[0].Rows[i]["memo_ref"].ToString(),
                                logged_by = objGlobaldata.GetMultiHrEmpNameById(dsCreateList.Tables[0].Rows[i]["logged_by"].ToString()),
                           };

                            //DateTime dtDocDate;
                            //if (dsCreateList.Tables[0].Rows[i]["date_request"].ToString() != ""
                            //   && DateTime.TryParse(dsCreateList.Tables[0].Rows[i]["date_request"].ToString(), out dtDocDate))
                            //{
                            //    objchklist.date_request = dtDocDate;
                            //}

                            objchkList.DCRChkList.Add(objchklist);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DCRChecklistList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objchkList.DCRChkList.ToList());
        }

        [AllowAnonymous]
        public ActionResult DCRChecklistEdit()
        {
            ViewBag.SubMenutype = "chklist";

            DCRChecklistModelsList objchkList = new DCRChecklistModelsList();
            objchkList.DCRChkList = new List<DCRChecklistModels>();
           
            try
            {
                string sid_chklist = Request.QueryString["id_chklist"];

                if (sid_chklist != null && sid_chklist != "")
                {
                    string sSqlstmt = "select id_chklist,checklistRef,revision,doc_title,dcrf_no,memo_ref," +
                        "logged_by from t_document_create_request_chklist where Active=1 and id_chklist = '"+ sid_chklist+"'";

                    DataSet dsCreateList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCreateList.Tables.Count > 0 && dsCreateList.Tables[0].Rows.Count > 0)
                    {
                        try
                            {
                                DCRChecklistModels objchklist = new DCRChecklistModels
                                {
                                    id_chklist = dsCreateList.Tables[0].Rows[0]["id_chklist"].ToString(),
                                    checklistRef = dsCreateList.Tables[0].Rows[0]["checklistRef"].ToString(),
                                    revision = /*objGlobaldata.GetDropdownitemById*/(dsCreateList.Tables[0].Rows[0]["revision"].ToString()),
                                    doc_title = dsCreateList.Tables[0].Rows[0]["doc_title"].ToString(),
                                    dcrf_no = dsCreateList.Tables[0].Rows[0]["dcrf_no"].ToString(),
                                    memo_ref = dsCreateList.Tables[0].Rows[0]["memo_ref"].ToString(),
                                    logged_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsCreateList.Tables[0].Rows[0]["logged_by"].ToString()),
                                };

                                string sqlstmt1 = "select id_chklist_trans,id_chklist,id_questions,ratings,comments from " +
                                    "t_document_create_request_chklist_trans where id_chklist = '"+ sid_chklist + "'";
                                DataSet dsChklist = objGlobaldata.Getdetails(sqlstmt1);

                            //Dictionary<string, string> dicDCRElement = new Dictionary<string, string>();
                            //if (dsChklist.Tables.Count > 0 && dsChklist.Tables[0].Rows.Count > 0)
                            //{                               
                            //    for (int i = 0; dsChklist.Tables.Count > 0 && i < dsChklist.Tables[0].Rows.Count; i++)
                            //    {
                            //        dicDCRElement.Add(dsChklist.Tables[0].Rows[i]["id_questions"].ToString(), dsChklist.Tables[0].Rows[i]["ratings"].ToString());
                            //    }
                            //}

                                ViewBag.dsChklist = dsChklist;
                            //    ViewBag.dicDCRElement = dicDCRElement;
                                ViewBag.DCRQuestions = objchklist.GetDCRQuestionList();
                                ViewBag.DCRSection = objchklist.GetDCRSectionList();
                                ViewBag.Revision = objGlobaldata.GetDropdownList("Revision Type");
                                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");

                            return View(objchklist);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DCRChecklistEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DCRChecklistList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DCRChecklistEdit(DCRChecklistModels ObjCheck, FormCollection form)
        {
            try
            {
                DCRChecklistModelsList ObjModelList = new DCRChecklistModelsList();
                ObjModelList.DCRChkList = new List<DCRChecklistModels>();

                MultiSelectList DCRQuestions = ObjCheck.GetDCRQuestionList();

                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in DCRQuestions)
                {
                    if (i <= Convert.ToInt16(form["itmctn"]))
                    {
                        DCRChecklistModels objElements = new DCRChecklistModels();
                        objElements.id_chklist_trans = form["id_chklist_trans " + item.Value];
                        objElements.id_questions = form["id_questions " + item.Value];
                        objElements.ratings = form["ratings " + item.Value];
                        objElements.comments = form["comments " + i];
                        ObjModelList.DCRChkList.Add(objElements);
                    }
                    i++;
                }

                if (ObjCheck.FunUpdateDCRChecklist(ObjCheck, ObjModelList))
                {
                    TempData["Successdata"] = "DCR Updated Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DCRChecklistEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DCRChecklistList");
        }

        [AllowAnonymous]
        public ActionResult DCRChecklistDetails()
        {
            ViewBag.SubMenutype = "chklist";

            DCRChecklistModelsList objchkList = new DCRChecklistModelsList();
            objchkList.DCRChkList = new List<DCRChecklistModels>();

            try
            {
                string sid_chklist = Request.QueryString["id_chklist"];

                if (sid_chklist != null && sid_chklist != "")
                {
                    string sSqlstmt = "select id_chklist,checklistRef,revision,doc_title,dcrf_no,memo_ref," +
                        "logged_by from t_document_create_request_chklist where Active=1 and id_chklist = '" + sid_chklist + "'";

                    DataSet dsCreateList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCreateList.Tables.Count > 0 && dsCreateList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            DCRChecklistModels objchklist = new DCRChecklistModels
                            {
                                id_chklist = dsCreateList.Tables[0].Rows[0]["id_chklist"].ToString(),
                                checklistRef = dsCreateList.Tables[0].Rows[0]["checklistRef"].ToString(),
                                revision = objGlobaldata.GetDropdownitemById(dsCreateList.Tables[0].Rows[0]["revision"].ToString()),
                                doc_title = dsCreateList.Tables[0].Rows[0]["doc_title"].ToString(),
                                dcrf_no = dsCreateList.Tables[0].Rows[0]["dcrf_no"].ToString(),
                                memo_ref = dsCreateList.Tables[0].Rows[0]["memo_ref"].ToString(),
                                logged_by = objGlobaldata.GetMultiHrEmpNameById(dsCreateList.Tables[0].Rows[0]["logged_by"].ToString()),
                            };

                            string sqlstmt1 = "select id_chklist_trans,id_chklist,id_questions,ratings,comments from " +
                                "t_document_create_request_chklist_trans where id_chklist = '" + sid_chklist + "'";
                            DataSet dsChklist = objGlobaldata.Getdetails(sqlstmt1);
                            for (int i = 0; dsChklist.Tables.Count > 0 && i < dsChklist.Tables[0].Rows.Count; i++)
                            {
                                DCRChecklistModels objElements = new DCRChecklistModels
                                {

                                    id_chklist = dsChklist.Tables[0].Rows[i]["id_chklist"].ToString(),
                                    id_questions = objchklist.GetDCRQuestionNameById(dsChklist.Tables[0].Rows[i]["id_questions"].ToString()),
                                    ratings = dsChklist.Tables[0].Rows[i]["ratings"].ToString(),
                                    comments = dsChklist.Tables[0].Rows[i]["comments"].ToString(),
                                };
                                objchkList.DCRChkList.Add(objElements);
                            }

                            ViewBag.ChkList = objchkList;
                            ViewBag.DCRQuestions = objchklist.GetDCRQuestionList();
                            ViewBag.DCRSection = objchklist.GetDCRSectionList();
                            //ViewBag.Revision = objGlobaldata.GetRevisionTypeList();
                            //ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");

                            return View(objchklist);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DocCreateRequestList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DCRChecklistDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DCRChecklistList");
        }

        [AllowAnonymous]
        public ActionResult DCRChecklistInfo(string chklist)
        {
            ViewBag.SubMenutype = "chklist";

            DCRChecklistModels objchklist = new DCRChecklistModels();
            
            DCRChecklistModelsList objchkList = new DCRChecklistModelsList();
            objchkList.DCRChkList = new List<DCRChecklistModels>();

            try
            {
               if (chklist != "")
                {
                    string id = objGlobaldata.GetChklistIdByRef(chklist);
                    string sSqlstmt = "select id_chklist,checklistRef,revision,doc_title,dcrf_no,memo_ref," +
                        "logged_by from t_document_create_request_chklist where Active=1 and id_chklist = '" + id + "'";

                    DataSet dsCreateList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCreateList.Tables.Count > 0 && dsCreateList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                             objchklist = new DCRChecklistModels
                            {
                                id_chklist = dsCreateList.Tables[0].Rows[0]["id_chklist"].ToString(),
                                checklistRef = dsCreateList.Tables[0].Rows[0]["checklistRef"].ToString(),
                                revision = objGlobaldata.GetDropdownitemById(dsCreateList.Tables[0].Rows[0]["revision"].ToString()),
                                doc_title = dsCreateList.Tables[0].Rows[0]["doc_title"].ToString(),
                                dcrf_no = dsCreateList.Tables[0].Rows[0]["dcrf_no"].ToString(),
                                memo_ref = dsCreateList.Tables[0].Rows[0]["memo_ref"].ToString(),
                                logged_by = objGlobaldata.GetMultiHrEmpNameById(dsCreateList.Tables[0].Rows[0]["logged_by"].ToString()),
                            };

                            string sqlstmt1 = "select id_chklist_trans,id_chklist,id_questions,ratings,comments from " +
                                "t_document_create_request_chklist_trans where id_chklist = '" + id + "'";
                            DataSet dsChklist = objGlobaldata.Getdetails(sqlstmt1);
                            for (int i = 0; dsChklist.Tables.Count > 0 && i < dsChklist.Tables[0].Rows.Count; i++)
                            {
                                DCRChecklistModels objElements = new DCRChecklistModels
                                {

                                    id_chklist = dsChklist.Tables[0].Rows[i]["id_chklist"].ToString(),
                                    id_questions = objchklist.GetDCRQuestionNameById(dsChklist.Tables[0].Rows[i]["id_questions"].ToString()),
                                    ratings = dsChklist.Tables[0].Rows[i]["ratings"].ToString(),
                                    comments = dsChklist.Tables[0].Rows[i]["comments"].ToString(),
                                };
                                objchkList.DCRChkList.Add(objElements);
                            }

                            ViewBag.ChkList = objchkList;
                            ViewBag.DCRQuestions = objchklist.GetDCRQuestionList();
                            ViewBag.DCRSection = objchklist.GetDCRSectionList();
                            //ViewBag.Revision = objGlobaldata.GetRevisionTypeList();
                            //ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");

                          
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DCRChecklistInfo: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    return View(objchklist);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DCRChecklistInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [AllowAnonymous]
        public JsonResult DCRChecklistDelete(FormCollection form)
        {
            try
            {
                if (form["id_chklist"] != null && form["id_chklist"] != "")
                {
                    DCRChecklistModels Doc = new DCRChecklistModels();

                    string sid_chklist = form["id_chklist"];

                    if (Doc.FunDeleteDCRChecklist(sid_chklist))
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
                objGlobaldata.AddFunctionalLog("Exception in DCRChecklistDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //public JsonResult FunGetCheckList()
        // {           
        //     try
        //     {
        //         ViewBag.CheckList = objGlobaldata.GetDCRChecklist();
        //         return Json(ViewBag.CheckList);
        //     }
        //     catch (Exception ex)
        //     {
        //         objGlobaldata.AddFunctionalLog("Exception in FunGetEmpDetails: " + ex.ToString());
        //         TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //     }
        //     return Json("");
        // }

       public JsonResult FunGetDocRefNo(string Direct,string Grp)
        {
            try
            {
                string DocRef = "";
                string sCompany = "";
                DataSet DsCompany = objGlobaldata.Getdetails("select CompanyName from t_companyinfo;");
                if (DsCompany.Tables.Count > 0 && DsCompany.Tables[0].Rows.Count > 0)
                {
                    sCompany = DsCompany.Tables[0].Rows[0]["CompanyName"].ToString();
                }
                if (Direct !="" && Grp != "")
                {
                    string dir = objGlobaldata.GetBranchShortNameByID(Direct);
                    string grp = objGlobaldata.GetDeptNameById(Grp);
                    DocRef = sCompany + "/" + dir + "/" + grp ;
                }
                return Json(DocRef);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetDocRefNo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }

        public JsonResult GetDCRChecklistByRef(string id_doc_request)
        {
            try
            {
                string chklist = objGlobaldata.GetDCRNoByDocId(id_doc_request);
                string CheckList = objGlobaldata.GetDCRChecklistByRef(chklist);
                return Json(CheckList);               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetDCRChecklistByRef: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }
        
    }
}