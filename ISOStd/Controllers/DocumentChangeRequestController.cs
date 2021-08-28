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
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class DocumentChangeRequestController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        static List<string> objDocumentNameList = new List<string>();
        public DocumentChangeRequestController()
        {
           
            ViewBag.Menutype = "Documents";
            ViewBag.SubMenutype = "DocumentChangeRequest";
        }
         
        public ActionResult AddDocumentChangeRequest()
        {

            return InitializeDocumentChangeRequest();
        }
         
        public ActionResult InitializeDocumentChangeRequest()
        {
            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.Name = objUser.firstname;
                ViewBag.DeptName = objGlobaldata.GetDeptNameById(objUser.DeptID);
                ViewBag.Designation = objUser.Designation;
                ViewBag.DocumentRef = objGlobaldata.GetDocumentRefList();
                ViewBag.DocumentName = objGlobaldata.GetDocumentNameList();            
                ViewBag.Emplist = objGlobaldata.GetApprover();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitializeDocumentChangeRequest: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddDocumentChangeRequest(DocumentChangeRequestModels objDocuments, FormCollection form, HttpPostedFileBase upload)
        {
            try {
                objDocuments.ApprovedBy = form["ApprovedBy"];

                if (upload != null && upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/IMSDocs"), Path.GetFileName(upload.FileName));
                        string sFilename = Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        upload.SaveAs(sFilepath + "/" + sFilename);
                        objDocuments.upload = "~/DataUpload/MgmtDocs/IMSDocs/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddDocumentChangeRequest-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objDocuments.FunAddDocumentChangeRequest(objDocuments))
                {
                    TempData["Successdata"] = "Added Document details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDocumentChangeRequest: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DocumentChangeRequestList");
        }

        [HttpPost]
        public ActionResult FunGetDocNameByID(string idMgmt)
        {
            DocumentChangeRequestModels obj = new DocumentChangeRequestModels();
            string DocName = obj.GetDocNameByRef(idMgmt);
            return Json(DocName);
        }
                 
        [HttpPost]
        public JsonResult GetDocumentNameList(string DocName)
        {
            UserCredentials objUser = new UserCredentials();
            objUser = objGlobaldata.GetCurrentUserSession();
            string DeptName = objGlobaldata.GetDeptNameById(objUser.DeptID);


            objDocumentNameList = objGlobaldata.GetDocumentNameListboxList(objUser.DeptID, DeptName);

            List<string> lstFilteredList = objDocumentNameList.FindAll(d => d.StartsWith(DocName.ToUpper()));

            return Json(lstFilteredList);
        }
         
        [HttpPost]
        public ActionResult FunGetDocRefByDocName(string DocName)
        {
            string DocRef = "";
            try
            {
                DocumentChangeRequestModels obj = new DocumentChangeRequestModels();
                DocRef = obj.GetDocRefByName(DocName);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetDocRefByDocName: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(DocRef);
        }
                 
         [AllowAnonymous]
         public ActionResult DocumentChangeRequestList(FormCollection form, string SearchText, string Approvestatus, string branch_name)
         {
             ViewBag.SubMenutype = "DocumentChangeRequest";
             ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus2");
             DocumentChangeRequestModelsList objDocumentList = new DocumentChangeRequestModelsList();
             objDocumentList.DocumentChangeList = new List<DocumentChangeRequestModels>();

             DocumentChangeRequestModels objDocuments = new DocumentChangeRequestModels();

             try
             {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select Id,DocRef,DocName,Changes,RequestedBy,ApprovedBy,ChangeRequestDate,(case when ApproveStatus='1' then 'Approved' when ApproveStatus='2' then 'Rejected' when ApproveStatus='0' then 'Not Approved' end) as ApproveStatus,"
                 + "(case when ChangeStatus='1' then 'Completed' else 'Pending' end) as ChangeStatus,Approvers,Rejector,upload,IssueNo,RevNo,DocDate from t_documentchangerequest where Active=1";
             
                 string sSearchtext = "";
                

                 if (SearchText != null && SearchText != "")
                 {
                     ViewBag.SearchText = SearchText;
                     sSearchtext = sSearchtext + "  and (DocRef ='" + SearchText + "' or DocRef like '" + SearchText + "%' or DocRef like '%" + SearchText + "%' or DocName='" + SearchText
                        + "' or DocName like '" + SearchText + "%' or DocName like '%" + SearchText + "%')";
                 }

                 
                 if (Approvestatus != null && Approvestatus != "All" && Approvestatus != "")
                 {
                     ViewBag.ApprovestatusVal = Approvestatus;
                     if (sSearchtext != "")
                     {
                         sSearchtext = sSearchtext + " and (ApproveStatus ='" + Approvestatus + "')";
                     }
                     else
                     {
                         sSearchtext = sSearchtext + " and (ApproveStatus ='" + Approvestatus + "')";
                     }
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
                sSqlstmt = sSqlstmt + sSearchtext + " order by ChangeRequestDate desc";

                 DataSet dsDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                 if (dsDocumentsList.Tables.Count > 0 && dsDocumentsList.Tables[0].Rows.Count > 0)
                 {
                     
                         
                     for (int i = 0; i < dsDocumentsList.Tables[0].Rows.Count; i++)
                     {
                         DateTime dtDocDate = Convert.ToDateTime(dsDocumentsList.Tables[0].Rows[i]["ChangeRequestDate"].ToString());
                       
                         try
                         {
                             DocumentChangeRequestModels objDocumentsModels = new DocumentChangeRequestModels
                             {
                                 Id = Convert.ToInt16(dsDocumentsList.Tables[0].Rows[i]["Id"].ToString()),
                                 DocRef =objGlobaldata.GetDocRefByID(dsDocumentsList.Tables[0].Rows[i]["DocRef"].ToString()),
                                 DocName =objGlobaldata.GetDocNameByID(dsDocumentsList.Tables[0].Rows[i]["DocName"].ToString()),
                                 Changes = dsDocumentsList.Tables[0].Rows[i]["Changes"].ToString(),
                                 RequestedBy =objGlobaldata.GetEmpHrNameById( dsDocumentsList.Tables[0].Rows[i]["RequestedBy"].ToString()),
                                 ApprovedBy =objGlobaldata.GetMultiHrEmpNameById( dsDocumentsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                 ApproveStatus = dsDocumentsList.Tables[0].Rows[i]["ApproveStatus"].ToString(),
                                 ChangeStatus =dsDocumentsList.Tables[0].Rows[i]["ChangeStatus"].ToString(),
                                 Approvers = dsDocumentsList.Tables[0].Rows[i]["Approvers"].ToString(),
                                 Rejector = dsDocumentsList.Tables[0].Rows[i]["Rejector"].ToString(),
                                 upload = dsDocumentsList.Tables[0].Rows[i]["upload"].ToString(),
                                 IssueNo = (dsDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString()),
                                 RevNo = (dsDocumentsList.Tables[0].Rows[i]["RevNo"].ToString()),
                             };

                             if (dsDocumentsList.Tables[0].Rows[i]["ChangeRequestDate"].ToString() != ""
                                 && DateTime.TryParse(dsDocumentsList.Tables[0].Rows[i]["ChangeRequestDate"].ToString(), out dtDocDate))
                             {
                                 objDocumentsModels.ChangeRequestDate = dtDocDate;
                             }
                             if (dsDocumentsList.Tables[0].Rows[i]["DocDate"].ToString() != ""
                                 && DateTime.TryParse(dsDocumentsList.Tables[0].Rows[i]["DocDate"].ToString(), out dtDocDate))
                             {
                                 objDocumentsModels.DocDate = dtDocDate;
                             }
                             objDocumentList.DocumentChangeList.Add(objDocumentsModels);
                         }
                         catch (Exception ex)
                         {
                            objGlobaldata.AddFunctionalLog("Exception in DocumentChangeRequestList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                     }

                 }
             }
             catch (Exception ex)
             {
                 objGlobaldata.AddFunctionalLog("Exception in DocumentChangeRequestList: " + ex.ToString());
                 TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
             }
             return View(objDocumentList.DocumentChangeList.ToList());
         }

        [AllowAnonymous]
        public JsonResult DocumentChangeRequestListSearch(FormCollection form, string SearchText, string Approvestatus, string branch_name)
        {
            ViewBag.SubMenutype = "DocumentChangeRequest";
            ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus2");
            DocumentChangeRequestModelsList objDocumentList = new DocumentChangeRequestModelsList();
            objDocumentList.DocumentChangeList = new List<DocumentChangeRequestModels>();

            DocumentChangeRequestModels objDocuments = new DocumentChangeRequestModels();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select Id,DocRef,DocName,Changes,RequestedBy,ApprovedBy,ChangeRequestDate,(case when ApproveStatus='1' then 'Approved' when ApproveStatus='2' then 'Rejected' when ApproveStatus='0' then 'Not Approved' end) as ApproveStatus,"
                + "(case when ChangeStatus='1' then 'Completed' else 'Pending' end) as ChangeStatus,Approvers,Rejector,upload,IssueNo,RevNo,DocDate from t_documentchangerequest where Active=1";

                string sSearchtext = "";


                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + "  and (DocRef ='" + SearchText + "' or DocRef like '" + SearchText + "%' or DocRef like '%" + SearchText + "%' or DocName='" + SearchText
                       + "' or DocName like '" + SearchText + "%' or DocName like '%" + SearchText + "%')";
                }


                if (Approvestatus != null && Approvestatus != "All" && Approvestatus != "")
                {
                    ViewBag.ApprovestatusVal = Approvestatus;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (ApproveStatus ='" + Approvestatus + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and (ApproveStatus ='" + Approvestatus + "')";
                    }
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

                sSqlstmt = sSqlstmt + sSearchtext + " order by ChangeRequestDate desc";

                DataSet dsDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsDocumentsList.Tables.Count > 0 && dsDocumentsList.Tables[0].Rows.Count > 0)
                {

                  

                    for (int i = 0; i < dsDocumentsList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtDocDate = Convert.ToDateTime(dsDocumentsList.Tables[0].Rows[i]["ChangeRequestDate"].ToString());

                        try
                        {
                            DocumentChangeRequestModels objDocumentsModels = new DocumentChangeRequestModels
                            {
                                Id = Convert.ToInt16(dsDocumentsList.Tables[0].Rows[i]["Id"].ToString()),
                                DocRef = objGlobaldata.GetDocRefByID(dsDocumentsList.Tables[0].Rows[i]["DocRef"].ToString()),
                                DocName = objGlobaldata.GetDocNameByID(dsDocumentsList.Tables[0].Rows[i]["DocName"].ToString()),
                                Changes = dsDocumentsList.Tables[0].Rows[i]["Changes"].ToString(),
                                RequestedBy = objGlobaldata.GetEmpHrNameById(dsDocumentsList.Tables[0].Rows[i]["RequestedBy"].ToString()),
                                ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsDocumentsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                ApproveStatus = dsDocumentsList.Tables[0].Rows[i]["ApproveStatus"].ToString(),
                                ChangeStatus = dsDocumentsList.Tables[0].Rows[i]["ChangeStatus"].ToString(),
                                Approvers = dsDocumentsList.Tables[0].Rows[i]["Approvers"].ToString(),
                                Rejector = dsDocumentsList.Tables[0].Rows[i]["Rejector"].ToString(),
                                upload = dsDocumentsList.Tables[0].Rows[i]["upload"].ToString(),
                                IssueNo = (dsDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString()),
                                RevNo = (dsDocumentsList.Tables[0].Rows[i]["RevNo"].ToString()),
                            };

                            if (dsDocumentsList.Tables[0].Rows[i]["ChangeRequestDate"].ToString() != ""
                                && DateTime.TryParse(dsDocumentsList.Tables[0].Rows[i]["ChangeRequestDate"].ToString(), out dtDocDate))
                            {
                                objDocumentsModels.ChangeRequestDate = dtDocDate;
                            }
                            if (dsDocumentsList.Tables[0].Rows[i]["DocDate"].ToString() != ""
                                && DateTime.TryParse(dsDocumentsList.Tables[0].Rows[i]["DocDate"].ToString(), out dtDocDate))
                            {
                                objDocumentsModels.DocDate = dtDocDate;
                            }
                            objDocumentList.DocumentChangeList.Add(objDocumentsModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DocumentChangeRequestListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocumentChangeRequestListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
         public ActionResult DocumentChangesApproveReject(string DocID, int iStatus, string PendingFlg, string Doccomments)
         {
             try
             {
                 DocumentChangeRequestModels objDocModels = new DocumentChangeRequestModels();
                
                     string sStatus = "";
                     if (iStatus == 0)
                     {
                         sStatus = "Pending";
                     }
                     else if (iStatus == 1)
                     {
                         sStatus = "Approved";

                     }
                     else if (iStatus == 2)
                     {
                         sStatus = "Rejected";

                     }
                     if (objDocModels.FunDocumentChangesRequestApproveOrReject(DocID, iStatus, Doccomments))
                     {
                         //TempData["Successdata"] = "Training " + sStatus;
                         TempData["Successdata"] = "Document Change request" + sStatus;
                     }
                     else
                     {
                         TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                     }
                 
                

             }
             catch (Exception ex)
             {
                 objGlobaldata.AddFunctionalLog("Exception in DocumentChangesApproveReject: " + ex.ToString());
                 TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
             }

             if (PendingFlg != null && PendingFlg == "true")
             {
                 return RedirectToAction("ListPendingForApproval", "Dashboard");
             }
             else
             {
                 return RedirectToAction("DocumentChangeRequestList");
             }
         }
                    
         [AllowAnonymous]
         public ActionResult DocumentChangeRequestEdit()
         {
             ViewBag.SubMenutype = "DocumentChangeRequest";
             DocumentChangeRequestModels objDocumentChangeRequestModels = new DocumentChangeRequestModels();

             try
             {
                 UserCredentials objUser = new UserCredentials();
                
                     objUser = objGlobaldata.GetCurrentUserSession();
                 
                 ViewBag.Name = objUser.firstname;
                 ViewBag.DeptName = objGlobaldata.GetDeptNameById(objUser.DeptID);
                 ViewBag.Designation = objUser.Designation;
                 ViewBag.DocumentRef = objGlobaldata.GetDocumentRefList();
                 ViewBag.DocumentName = objGlobaldata.GetDocumentNameList();
               //  ViewBag.IssueNo = objGlobaldata.GetDocumentIssueNoList();
                // ViewBag.RevNo = objGlobaldata.GetDocumentRevNoList();
                 ViewBag.EmpLists = objGlobaldata.GetApprover();
                 DateTime dtDocDate;
                 if (Request.QueryString["Ids"] != null && Request.QueryString["Ids"] != "")
                 {
                     string Id = Request.QueryString["Ids"];

                     string sSqlstmt = "select Id,DocRef,DocName,Changes,RequestedBy,ApprovedBy,ChangeRequestDate,upload,IssueNo,RevNo,DocDate from t_documentchangerequest where Id='" + Id + "'";

                     DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);


                     if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                     {
                         if (objUser.empid == dsMgmtDocumentsList.Tables[0].Rows[0]["RequestedBy"].ToString())
                         {
                             objDocumentChangeRequestModels = new DocumentChangeRequestModels
                             {
                                 Id = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["Id"].ToString()),
                                 DocRef =(dsMgmtDocumentsList.Tables[0].Rows[0]["DocRef"].ToString()),
                                 DocName = dsMgmtDocumentsList.Tables[0].Rows[0]["DocName"].ToString(),
                                 Changes = dsMgmtDocumentsList.Tables[0].Rows[0]["Changes"].ToString(),
                                 RequestedBy =objGlobaldata.GetEmpHrNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["RequestedBy"].ToString()),
                                 ApprovedBy = (dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                                 ChangeRequestDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[0]["ChangeRequestDate"].ToString()),
                                 upload = dsMgmtDocumentsList.Tables[0].Rows[0]["upload"].ToString(),
                                 IssueNo = dsMgmtDocumentsList.Tables[0].Rows[0]["IssueNo"].ToString(),
                                 RevNo = dsMgmtDocumentsList.Tables[0].Rows[0]["RevNo"].ToString(),
                             };
                             if (dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString() != ""
                                && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString(), out dtDocDate))
                             {
                                 objDocumentChangeRequestModels.DocDate = dtDocDate;
                             }
                         }
                         else
                         {
                             ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                             TempData["alertdata"] = "Access Denied";
                             return RedirectToAction("DocumentChangeRequestList");
                         }
                     }
                     else
                     {
                         ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                         TempData["alertdata"] = "Document Id cannot be Null or empty";
                         return RedirectToAction("DocumentChangeRequestList");
                     }
                 }
                 else
                 {
                     ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                     TempData["alertdata"] = "Document Id cannot be Null or empty";
                     return RedirectToAction("DocumentChangeRequestList");
                 }
             }
             catch (Exception ex)
             {
                 objGlobaldata.AddFunctionalLog("Exception in DocumentChangeRequestEdit: " + ex.ToString());
                 TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                 return RedirectToAction("DocumentChangeRequestList");
             }

             return View(objDocumentChangeRequestModels);

         }
                 
         [HttpPost]
         [ValidateAntiForgeryToken]
         [AllowAnonymous]
         [ValidateInput(false)]
         public ActionResult DocumentChangeRequestEdit(DocumentChangeRequestModels objManagement, FormCollection form, HttpPostedFileBase upload)
         {
             try
             {

                 objManagement.ApprovedBy = form["ApprovedBy"];

                if (upload != null && upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/IMSDocs"), Path.GetFileName(upload.FileName));
                        string sFilename = Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        upload.SaveAs(sFilepath + "/" + sFilename);
                        objManagement.upload = "~/DataUpload/MgmtDocs/IMSDocs/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in DocumentChangeRequestEdit-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objManagement.FunUpdateChangeRequest(objManagement))
                 {
                     TempData["Successdata"] = "Document Change request details updated successfully";
                     //objManagement.SendDocumentChangeRequestmail(objManagement.Id, "Document Change Request for Approval");
                 }
                 else
                 {
                     TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                 }
             }
             catch (Exception ex)
             {
                 objGlobaldata.AddFunctionalLog("Exception in DocumentChangeRequestEdit: " + ex.ToString());
                 TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                 return RedirectToAction("DocumentChangeRequestList");
             }
             return RedirectToAction("DocumentChangeRequestList");
         }
                 
         [HttpPost]
         public JsonResult FunCheckStatus(string DocRef)
         {
             DocumentChangeRequestModels objMgmtDocuments = new DocumentChangeRequestModels();
             bool status = false;
             if (DocRef != null)
             {
                 status = objMgmtDocuments.GetDocumentStatus(DocRef);
             }

             return Json(status);
         }
                 
         [HttpPost]
         public JsonResult FunCheckDocRefStatus(string idMgmt)
         {
             DocumentChangeRequestModels objMgmtDocuments = new DocumentChangeRequestModels();
             bool status = false;
             if (idMgmt != null)
             {
                 status = objMgmtDocuments.GetDocRefStatus(idMgmt);
             }

             return Json(status);
         }
                 
         [HttpPost]
         public JsonResult FunCheckDocNameStatus(string idMgmt)
         {
             DocumentChangeRequestModels objMgmtDocuments = new DocumentChangeRequestModels();
             bool status = false;
             if (idMgmt != null)
             {
                 status = objMgmtDocuments.GetDocNameStatus(idMgmt);
             }

             return Json(status);
         }
        
        public JsonResult FungetDocDetail(string DocName)
        {
            DocumentChangeRequestModels objMgmtDocuments = new DocumentChangeRequestModels();
            if (DocName != "")
            {
                string sql = "select IssueNo,RevNo from t_mgmt_documents where idMgmt= '" + DocName + "'";
                DataSet dsData = objGlobaldata.Getdetails(sql);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    objMgmtDocuments = new DocumentChangeRequestModels()
                    {
                        IssueNo = dsData.Tables[0].Rows[0]["IssueNo"].ToString(),
                        RevNo = dsData.Tables[0].Rows[0]["RevNo"].ToString(),
                    };
                }
            }
            return Json(objMgmtDocuments);
        }

        [AllowAnonymous]
         public JsonResult ChangeDocumenttDocDelete(FormCollection form)
         {
             try
             {
                    if (form["Id"] != null && form["Id"] != "")
                         {
                             DocumentChangeRequestModels Doc = new DocumentChangeRequestModels();
                             string sId = form["Id"];


                             if (Doc.FunDeleteChangeMgmtDoc(sId))
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
                             TempData["alertdata"] = "Change Management Id cannot be Null or empty";
                             return Json("Failed");
                         }
                   
                 
             }
             catch (Exception ex)
             {
                 objGlobaldata.AddFunctionalLog("Exception in ChangeDocumenttDocDelete: " + ex.ToString());
                 TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
             }
             return Json("Failed");
         }
                 
         [AllowAnonymous]
         public ActionResult DocumentChangeRequestDetails()
         {
             DocumentChangeRequestModels objDocumentsModels = new DocumentChangeRequestModels();
          
             try
             {
                 if (Request.QueryString["Idmgmt"] != null && Request.QueryString["Idmgmt"] != "")
                 {
                     string Id = Request.QueryString["Idmgmt"];


                     string sSqlstmt = "select Id,DocRef,DocName,Changes,RequestedBy,ApprovedBy,ChangeRequestDate,(case when ApproveStatus='1' then 'Approved' when ApproveStatus='2' then 'Rejected' when ApproveStatus='0' then 'Not Approved' end) as ApproveStatus,ApprovedDate,"
                     + "(case when ChangeStatus='1' then 'Completed' else 'Pending' end) as ChangeStatus,Approvers,Rejector,upload,IssueNo,RevNo,DocDate from t_documentchangerequest where Id='" + Id + "'";

                  DataSet dsDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                  if (dsDocumentsList.Tables.Count > 0 && dsDocumentsList.Tables[0].Rows.Count > 0)
                  {
                      for (int i = 0; i < dsDocumentsList.Tables[0].Rows.Count; i++)
                      {
                          objDocumentsModels = new DocumentChangeRequestModels
                          {
                              Id = Convert.ToInt16(dsDocumentsList.Tables[0].Rows[i]["Id"].ToString()),
                              DocRef =objGlobaldata.GetDocRefByID(dsDocumentsList.Tables[0].Rows[i]["DocRef"].ToString()),
                              DocName =objGlobaldata.GetDocNameByID(dsDocumentsList.Tables[0].Rows[i]["DocName"].ToString()),
                              Changes = dsDocumentsList.Tables[0].Rows[i]["Changes"].ToString(),
                              RequestedBy = objGlobaldata.GetEmpHrNameById(dsDocumentsList.Tables[0].Rows[i]["RequestedBy"].ToString()),
                              ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsDocumentsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                              ApproveStatus = dsDocumentsList.Tables[0].Rows[i]["ApproveStatus"].ToString(),
                              ChangeStatus = dsDocumentsList.Tables[0].Rows[i]["ChangeStatus"].ToString(),
                              Approvers = dsDocumentsList.Tables[0].Rows[i]["Approvers"].ToString(),
                              Rejector = dsDocumentsList.Tables[0].Rows[i]["Rejector"].ToString(),
                              upload = dsDocumentsList.Tables[0].Rows[i]["upload"].ToString(),
                              IssueNo = (dsDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString()),
                              RevNo = (dsDocumentsList.Tables[0].Rows[i]["RevNo"].ToString()),

                          };
                          DateTime dtDocDate;
                          if (dsDocumentsList.Tables[0].Rows[i]["ChangeRequestDate"].ToString() != ""
                              && DateTime.TryParse(dsDocumentsList.Tables[0].Rows[i]["ChangeRequestDate"].ToString(), out dtDocDate))
                          {
                              objDocumentsModels.ChangeRequestDate = dtDocDate;
                          }

                          if (dsDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString() != ""
                              && DateTime.TryParse(dsDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString(), out dtDocDate))
                          {
                              objDocumentsModels.ApprovedDate = dtDocDate;
                          }
                          if (dsDocumentsList.Tables[0].Rows[i]["DocDate"].ToString() != ""
                            && DateTime.TryParse(dsDocumentsList.Tables[0].Rows[i]["DocDate"].ToString(), out dtDocDate))
                          {
                              objDocumentsModels.DocDate = dtDocDate;
                          }
                          DataSet dsChangeRequest = objGlobaldata.GetDocumentChangeRequestCommentdetails(Id);
                          ViewBag.CommentDetails = dsChangeRequest;

                      }
                  }
                  else
                  {
                      TempData["alertdata"] = "No Data exists";
                      return RedirectToAction("ChangeManagementList");
                  }
                 }
                 else
                 {
                     TempData["alertdata"] = "Id cannot be null";
                     return RedirectToAction("ChangeManagementList");
                 }
             }
             catch (Exception ex)
             {
                 objGlobaldata.AddFunctionalLog("Exception in DocumentChangeRequestDetails: " + ex.ToString());
                 TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
             }
             return View(objDocumentsModels);

         }
                 
         [AllowAnonymous]
         public ActionResult DocumentChangeRequestInfo(int id)
         {
             DocumentChangeRequestModels objDocumentsModels = new DocumentChangeRequestModels();
             try
             {
                    string idChange = Convert.ToString(id);
                     string sSqlstmt = "select Id,DocRef,DocName,Changes,RequestedBy,ApprovedBy,ChangeRequestDate,(case when ApproveStatus='1' then 'Approved' when ApproveStatus='2' then 'Rejected' when ApproveStatus='0' then 'Not Approved' end) as ApproveStatus,ApprovedDate,"
                     + "(case when ChangeStatus='1' then 'Completed' else 'Pending' end) as ChangeStatus,Approvers,Rejector,upload,IssueNo,RevNo,DocDate from t_documentchangerequest where Id='" + id + "'";

                     DataSet dsDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                     if (dsDocumentsList.Tables.Count > 0 && dsDocumentsList.Tables[0].Rows.Count > 0)
                     {
                         for (int i = 0; i < dsDocumentsList.Tables[0].Rows.Count; i++)
                         {
                             objDocumentsModels = new DocumentChangeRequestModels
                             {
                                 Id = Convert.ToInt16(dsDocumentsList.Tables[0].Rows[i]["Id"].ToString()),
                                 DocRef = objGlobaldata.GetDocRefByID(dsDocumentsList.Tables[0].Rows[i]["DocRef"].ToString()),
                                 DocName = objGlobaldata.GetDocNameByID(dsDocumentsList.Tables[0].Rows[i]["DocName"].ToString()),
                                 Changes = dsDocumentsList.Tables[0].Rows[i]["Changes"].ToString(),
                                 RequestedBy = objGlobaldata.GetEmpHrNameById(dsDocumentsList.Tables[0].Rows[i]["RequestedBy"].ToString()),
                                 ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsDocumentsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                 ApproveStatus = dsDocumentsList.Tables[0].Rows[i]["ApproveStatus"].ToString(),
                                 ChangeStatus = dsDocumentsList.Tables[0].Rows[i]["ChangeStatus"].ToString(),
                                 Approvers = dsDocumentsList.Tables[0].Rows[i]["Approvers"].ToString(),
                                 Rejector = dsDocumentsList.Tables[0].Rows[i]["Rejector"].ToString(),
                                 upload = dsDocumentsList.Tables[0].Rows[i]["upload"].ToString(),
                                 IssueNo = dsDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString(),
                                 RevNo = dsDocumentsList.Tables[0].Rows[i]["RevNo"].ToString(),

                             };
                             DateTime dtDocDate;
                             if (dsDocumentsList.Tables[0].Rows[i]["ChangeRequestDate"].ToString() != ""
                                 && DateTime.TryParse(dsDocumentsList.Tables[0].Rows[i]["ChangeRequestDate"].ToString(), out dtDocDate))
                             {
                                 objDocumentsModels.ChangeRequestDate = dtDocDate;
                             }

                             if (dsDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString() != ""
                                 && DateTime.TryParse(dsDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString(), out dtDocDate))
                             {
                                 objDocumentsModels.ApprovedDate = dtDocDate;
                             }
                             if (dsDocumentsList.Tables[0].Rows[i]["DocDate"].ToString() != ""
                                && DateTime.TryParse(dsDocumentsList.Tables[0].Rows[i]["DocDate"].ToString(), out dtDocDate))
                             {
                                 objDocumentsModels.DocDate = dtDocDate;
                             }
                             DataSet dsChangeRequest = objGlobaldata.GetDocumentChangeRequestCommentdetails(idChange);
                             ViewBag.CommentDetails = dsChangeRequest;
                         }
                     }
                     else
                     {
                         TempData["alertdata"] = "No Data exists";
                         return RedirectToAction("ChangeManagementList");
                     }
             }
             catch (Exception ex)
             {
                 objGlobaldata.AddFunctionalLog("Exception in DocumentChangeRequestDetails: " + ex.ToString());
                 TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
             }
             return View(objDocumentsModels);

         }

        public JsonResult DocumentChangesApproveRejectNoty(string DocID, int iStatus, string PendingFlg, string Doccomments)
         {
             try
             {
                 DocumentChangeRequestModels objDocModels = new DocumentChangeRequestModels();

                 string sStatus = "";
                 if (iStatus == 0)
                 {
                     sStatus = "Pending";
                 }
                 else if (iStatus == 1)
                 {
                     sStatus = "Approved";

                 }
                 else if (iStatus == 2)
                 {
                     sStatus = "Rejected";

                 }
                 if (objDocModels.FunDocumentChangesRequestApproveOrReject(DocID, iStatus, Doccomments))
                 {
                     //TempData["Successdata"] = "Training " + sStatus;
                     return Json("Success" + iStatus);
                 }
                 else
                 {
                     return Json("Failed");
                 }
             }
             catch (Exception ex)
             {
                 objGlobaldata.AddFunctionalLog("Exception in DocumentChangesApproveReject: " + ex.ToString());
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