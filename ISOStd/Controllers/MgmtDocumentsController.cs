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
    public class MgmtDocumentsController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public MgmtDocumentsController()
        {
            ViewBag.Menutype = "Documents";
            ViewBag.SubMenutype = "MgmtDocuments";
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AddMgmtDocuments()
        {
            ViewBag.SubMenutype = "MgmtDocuments";
            return InitilizeAddMgmtDocuments();
        }

        [AllowAnonymous]
        public ActionResult Mgmtvideo()
        {
            return View();
        }

        private ActionResult InitilizeAddMgmtDocuments()
        {
            MgmtDocumentsModels objmgmt = new MgmtDocumentsModels();
            try
            {
                objmgmt.branch = objGlobaldata.GetCurrentUserSession().division;
                objmgmt.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objmgmt.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.PreparerList = objGlobaldata.GetGEmpListbymultiBranch(objmgmt.branch);
                ViewBag.ReviewerList = objGlobaldata.GetReviewer();
                ViewBag.ApproverList = objGlobaldata.GetApprover();
                //ViewBag.PreparerList = objGlobaldata.GetGEmpListBymulitBDL(objmgmt.branch, objmgmt.Department, objmgmt.Location);
                //ViewBag.ReviewerList = objGlobaldata.GetGRoleList(objmgmt.branch, objmgmt.Department, objmgmt.Location, "Reviewer");
                //ViewBag.ApproverList = objGlobaldata.GetGRoleList(objmgmt.branch, objmgmt.Department, objmgmt.Location, "Approver");
                //// ViewBag.DeptList = objGlobaldata.GetDepartmentWithIdListbox();
                ViewBag.DocLevels = objGlobaldata.GetDocLevelsList();
                // ViewBag.Doctype = objGlobaldata.GetConstantValue("Doctype");

                //ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList("");
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objmgmt.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objmgmt.branch);
                ViewBag.Views = objGlobaldata.GetViewsListbox();
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.DCRDoc = objGlobaldata.GetDCRDocumentList(objmgmt.branch, objmgmt.Location);

                //ViewBag.ISOStds = objGlobaldata.GetAllIsoStdListbox();

                DropdownMultiItemsList dplist = new DropdownMultiItemsList();
                dplist.DropdownList = new List<DropdownMultiItems>();
                try
                {
                    DropdownMultiItems Isostd = new DropdownMultiItems();
                    DataSet dsEmp = objGlobaldata.Getdetails("select StdId, IsoName,Descriptions from t_isostandards where Active=1");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                        {
                            Isostd = new DropdownMultiItems()
                            {
                                Id = dsEmp.Tables[0].Rows[i]["StdId"].ToString(),
                                Name = dsEmp.Tables[0].Rows[i]["IsoName"].ToString(),
                                Desc = dsEmp.Tables[0].Rows[i]["Descriptions"].ToString()
                            };
                            dplist.DropdownList.Add(Isostd);
                        }
                        ViewBag.ISOStds = dplist;
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in InitilizeAddMgmtDocuments: " + ex.ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddMgmtDocuments: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objmgmt);
        }

        [HttpPost]
        public JsonResult doesDocNameExist(string DocName)
        {
            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
            var user = objMgmtDocuments.CheckForDocNameExists(DocName);

            return Json(user);
        }

        [HttpPost]
        public JsonResult doesDocRefExist(string DocRef)
        {
            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
            var user = objMgmtDocuments.CheckForDocRefExists(DocRef);

            return Json(user);
        }

        public ActionResult FunISOClauseList(string ISOStdId)
        {
            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
            MultiSelectList lstEmp = objMgmtDocuments.GetMultiISOClauseList(ISOStdId);
            return Json(lstEmp);
        }

        [HttpPost]
        public JsonResult FunCheckRevisionNo(string idMgmt)
        {
            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
            var RevNo = "";
            if (idMgmt != null)
            {
                RevNo = objMgmtDocuments.GetRevNoHistory(idMgmt);
            }

            return Json(RevNo);
        }

        [HttpPost]
        public JsonResult FunCheckIssueNo(string idMgmt)
        {
            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
            var IssueNo = "";
            if (idMgmt != null)
            {
                IssueNo = objMgmtDocuments.GetIssueNoHistory(idMgmt);
            }

            return Json(IssueNo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMgmtDocuments(MgmtDocumentsModels objMgmtDocuments, FormCollection form, HttpPostedFileBase fileUploader)
        {
            string sDocName = form["DocName"];
            //if (objMgmtDocuments != null && objMgmtDocuments.CheckForDuplicate(sDocName, objMgmtDocuments.DocRef))
            try
            {
                objMgmtDocuments.Dept = form["Dept"];
                objMgmtDocuments.DocLevels = form["DocLevels"];
                objMgmtDocuments.DocName = sDocName;
                objMgmtDocuments.DocRef = objMgmtDocuments.DocRef;
                objMgmtDocuments.ISOStds = form["ISOStds"];
                objMgmtDocuments.Doctype = form["Doctype"];
                objMgmtDocuments.PreparedBy = form["PreparedBy"];
                objMgmtDocuments.ReviewedBy = form["ReviewedBy"];
                objMgmtDocuments.ApprovedBy = form["ApprovedBy"];
                objMgmtDocuments.IssueNo = form["IssueNo"];
                objMgmtDocuments.AppClauses = form["AppClauses"];
                objMgmtDocuments.Department = form["Department"];
                objMgmtDocuments.Doctype = form["Doctype"];
                objMgmtDocuments.view_by = form["view_by"];
                objMgmtDocuments.branch = form["branch"];
                objMgmtDocuments.Location = form["Location"];
                objMgmtDocuments.dcr_no = form["dcr_no"];
                string supload = form["upload"];

                if (objMgmtDocuments.branch == null || objMgmtDocuments.branch == "")
                {
                    objMgmtDocuments.branch = objGlobaldata.GetCurrentUserSession().division;
                }
                DateTime dateValue;

                if (DateTime.TryParse(form["DocDate"], out dateValue) == true)
                {
                    objMgmtDocuments.DocDate = dateValue;
                }

                if (supload != null && supload != "")
                {
                    objMgmtDocuments.DocUploadPath = supload;
                }
                else if (fileUploader != null && fileUploader.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/IMSDocs"), Path.GetFileName(fileUploader.FileName));
                        string sFilename = objMgmtDocuments.DocName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        fileUploader.SaveAs(sFilepath + "/" + sFilename);
                        objMgmtDocuments.DocUploadPath = "~/DataUpload/MgmtDocs/IMSDocs/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddMgmtDocuments-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objMgmtDocuments.FunAddMgmtDocuments(objMgmtDocuments, fileUploader))
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
                objGlobaldata.AddFunctionalLog("Exception in AddMgmtDocuments: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MgmtDocumentsList");
        }

        [AllowAnonymous]
        public JsonResult MgmtDocDelete(FormCollection form)
        {
            try
            {
                if (form["idMgmt"] != null && form["idMgmt"] != "")
                {
                    MgmtDocumentsModels Doc = new MgmtDocumentsModels();
                    string sidMgmt = form["idMgmt"];

                    if (Doc.FunDeleteMgmtDoc(sidMgmt))
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
                    TempData["alertdata"] = "MgmtDoc Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult AnnexureDelete(FormCollection form)
        {
            try
            {
                if (form["idAnnexure"] != null && form["idAnnexure"] != "")
                {
                    MgmtDocumentsModels Doc = new MgmtDocumentsModels();
                    string sidAnnexure = form["idAnnexure"];

                    if (Doc.FunDeleteAnnexure(sidAnnexure))
                    {
                        TempData["Successdata"] = "Annexure details deleted successfully";
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
                    TempData["alertdata"] = "Annexure ID cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AnnexureDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult MgmtHistoricalDocumentsList(FormCollection form, int? page)
        {
            ViewBag.SubMenutype = "MgmtDocuments";

            MgmtDocumentsModelsList objMgmtDocumentsList = new MgmtDocumentsModelsList();
            objMgmtDocumentsList.MgmtDocumentsMList = new List<MgmtDocumentsModels>();

            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();

            try
            {
                string sSqlstmt = "select idMgmt,Company, DocLevels, Doctype, ISOStds, AppClauses, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate,"
                    + " DocUploadPath, ApprovedBy, (case when Approved_Status='1' then 'Approved' else 'Not Approved' end) as Approved_Status,(case when Reviewed_Status='1' then 'Reviewed' else 'Not Reviewed' end) as Reviewed_Status, ApprovedDate, UploadedBy "
                    + " from t_mgmt_documents where status=1";

                UserCredentials objUser = new UserCredentials();

                objUser = objGlobaldata.GetCurrentUserSession();

                string sRolename = objGlobaldata.GetRoleName(objUser.role);
                string sCompany = objGlobaldata.GetCompanyBranchNameById(objUser.division);
                ViewBag.Role = sRolename;
                ViewBag.CurrentEmpName = objUser.firstname;
                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");
                ViewBag.Company = objGlobaldata.GetCompanyBranchListbox();

                //searching the company name
                objMgmtDocuments.Company = form["Company"];
                string sSCompany = objGlobaldata.GetCompanyBranchNameById(form["Company"]);

                sSqlstmt = sSqlstmt + " order by DocLevels, idmgmt desc";

                DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsMgmtDocumentsList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["DocDate"].ToString());
                        string sDept = "";
                        if (dsMgmtDocumentsList.Tables[0].Rows[i]["Dept"].ToString() != "")
                        {
                            sDept = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Dept"].ToString());
                        }
                        try
                        {
                            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels
                            {
                                idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["idMgmt"].ToString()),
                                Dept = sDept,
                                Company = objGlobaldata.GetCompanyBranchNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Company"].ToString()),
                                DocLevels = (dsMgmtDocumentsList.Tables[0].Rows[i]["DocLevels"].ToString()),
                                Doctype = dsMgmtDocumentsList.Tables[0].Rows[i]["Doctype"].ToString(),
                                ISOStds = objGlobaldata.GetIsoStdNamebyId(dsMgmtDocumentsList.Tables[0].Rows[i]["ISOStds"].ToString()),
                                AppClauses = objMgmtDocuments.GetMultiISOClauseNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["AppClauses"].ToString()),
                                DocRef = dsMgmtDocumentsList.Tables[0].Rows[i]["DocRef"].ToString(),
                                DocName = dsMgmtDocumentsList.Tables[0].Rows[i]["DocName"].ToString(),
                                IssueNo = dsMgmtDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString(),
                                RevNo = dsMgmtDocumentsList.Tables[0].Rows[i]["RevNo"].ToString(),
                                DocDate = dtDocDate,
                                PreparedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["PreparedBy"].ToString()),
                                ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["ReviewedBy"].ToString()),
                                DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                                ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                Approved_Status = dsMgmtDocumentsList.Tables[0].Rows[i]["Approved_Status"].ToString(),
                                Reviewed_Status = dsMgmtDocumentsList.Tables[0].Rows[i]["Reviewed_Status"].ToString(),
                                UploadedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["UploadedBy"].ToString())
                            };

                            if (dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString() != ""
                                && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString(), out dtDocDate))
                            {
                                objMgmtDocumentsModels.ApprovedDate = dtDocDate;
                            }

                            objMgmtDocumentsList.MgmtDocumentsMList.Add(objMgmtDocumentsModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in MgmtHistoricalDocumentsList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtHistoricalDocumentsList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objMgmtDocumentsList.MgmtDocumentsMList.ToList().ToPagedList(page ?? 1, 1000));
        }

        // GET: /MgmtDocuments/MgmtDocumentsList

        [AllowAnonymous]
        public ActionResult MgmtDocumentsList(FormCollection form, string SearchText, string Approvestatus, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "MgmtDocuments";

            MgmtDocumentsModelsList objMgmtDocumentsList = new MgmtDocumentsModelsList();
            objMgmtDocumentsList.MgmtDocumentsMList = new List<MgmtDocumentsModels>();

            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
            try
            {
                //ViewBag.View = Request.QueryString["View"];

                //String DocumentType = objMgmtDocuments.GetDocumentTypeIdbyName("Policy");
                //String DocumentType1 = objMgmtDocuments.GetDocumentTypeIdbyName("Manual");
                //String DocumentType2 = objMgmtDocuments.GetDocumentTypeIdbyName("Procedure");
                //String DocumentType3 = objMgmtDocuments.GetDocumentTypeIdbyName("Forms");
                //String DocumentType4 = objMgmtDocuments.GetDocumentTypeIdbyName("Work Instructions");

                string sSqlstmt = "select idMgmt,Department, DocLevels, Doctype, ISOStds, AppClauses, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate,"
                    + " DocUploadPath, ApprovedBy, (case when Approved_Status='1' then 'Approved' when Approved_Status='2' then 'Approve Rejected' else 'Not Approved' end) as Approved_Status," +
                    "(case when Reviewed_Status='1' then 'Reviewed' when Reviewed_Status='2' then 'Review Rejected' else 'Not Reviewed' end) as Reviewed_Status, ApprovedDate, UploadedBy,view_by,branch,Location,Reviewers,Approvers,"
                    + "Reviewed_Status as Reviewed_StatusId,Approved_Status as Approved_StatusId from t_mgmt_documents where status=1";

                string sSearchtext = "";/*, sApproveStatus = "", DepartName = "ALL";*/

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();

                string sDepartment = objGlobaldata.GetDeptNameById(objUser.DeptID);//retrieving employee deptid who logged in

                ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);

                ViewBag.CurrentEmpName = objUser.firstname;
                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");

                ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();

                //searching the company name
                objMgmtDocuments.Department = form["Department"];//retrieving dept id
                string sSDepartment = objGlobaldata.GetMultiDeptNameById(form["Department"]);//retrieving name by id

                objMgmtDocuments.Doctype = form["Doctype"];
                string sSDocumentType = objGlobaldata.GetDocumentTypebyId(form["Doctype"]);

                //fetching deptid of Department="ALL"
                //string AllDeptId = objGlobaldata.GetDeptIDByName(DepartName.ToLower());

                if (objMgmtDocuments.Department == null)
                {
                    objMgmtDocuments.Department = "";
                }

                if (Approvestatus == null)
                {
                    Approvestatus = "";
                }

                if (objMgmtDocuments.Doctype == null)
                {
                    objMgmtDocuments.Doctype = "";
                }

                if (!(objGlobaldata.GetMultiRolesNameById(objUser.role).Contains("Admin")))
                {
                    sSearchtext = sSearchtext + " and (find_in_set('" + objUser.DeptID + "', view_by) or find_in_set('0', view_by))";
                }

                //sSqlstmt = sSqlstmt + sSearchtext + " order by Doctype";
                sSqlstmt = sSqlstmt + sSearchtext + " order by idMgmt desc";

                DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsMgmtDocumentsList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["DocDate"].ToString());
                        //string sDept = "";
                        //if (dsMgmtDocumentsList.Tables[0].Rows[i]["Dept"].ToString() != "")
                        //{
                        //    sDept = objGlobaldata.GetDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Dept"].ToString());
                        //}
                        try
                        {
                            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels
                            {
                                idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["idMgmt"].ToString()),

                                Department = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Department"].ToString()),
                                DocLevels = objGlobaldata.GetDocumentLevelbyId(dsMgmtDocumentsList.Tables[0].Rows[i]["DocLevels"].ToString()),
                                Doctype = objGlobaldata.GetDocumentTypebyId(dsMgmtDocumentsList.Tables[0].Rows[i]["Doctype"].ToString()),
                                ISOStds = objGlobaldata.GetIsoStdNamebyId(dsMgmtDocumentsList.Tables[0].Rows[i]["ISOStds"].ToString()),
                                AppClauses = objMgmtDocuments.GetMultiISOClauseNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["AppClauses"].ToString()),
                                DocRef = dsMgmtDocumentsList.Tables[0].Rows[i]["DocRef"].ToString(),
                                DocName = dsMgmtDocumentsList.Tables[0].Rows[i]["DocName"].ToString(),
                                IssueNo = dsMgmtDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString(),
                                RevNo = dsMgmtDocumentsList.Tables[0].Rows[i]["RevNo"].ToString(),
                                DocDate = dtDocDate,
                                PreparedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["PreparedBy"].ToString()),
                                ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["ReviewedBy"].ToString()),
                                DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                                ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                Approved_Status = dsMgmtDocumentsList.Tables[0].Rows[i]["Approved_Status"].ToString(),
                                Reviewed_Status = dsMgmtDocumentsList.Tables[0].Rows[i]["Reviewed_Status"].ToString(),
                                UploadedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["UploadedBy"].ToString()),
                                DocRetention = objGlobaldata.GetDropdownitemById(dsMgmtDocumentsList.Tables[0].Rows[i]["Doctype"].ToString()),
                                view_by = objGlobaldata.GetViewDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["view_by"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsMgmtDocumentsList.Tables[0].Rows[i]["Location"].ToString()),
                                Reviewers = dsMgmtDocumentsList.Tables[0].Rows[i]["Reviewers"].ToString(),
                                Approvers = dsMgmtDocumentsList.Tables[0].Rows[i]["Approvers"].ToString(),
                                Approved_StatusId = dsMgmtDocumentsList.Tables[0].Rows[i]["Approved_StatusId"].ToString(),
                                Reviewed_StatusId = dsMgmtDocumentsList.Tables[0].Rows[i]["Reviewed_StatusId"].ToString(),
                            };
                            if (objMgmtDocumentsModels.Reviewers != "0")
                            {
                                string sReviewer = objMgmtDocumentsModels.Reviewers;
                                string sReviewedPersons = sReviewer.Substring(2);
                                objMgmtDocumentsModels.Reviewers = objGlobaldata.GetMultiHrEmpNameById(sReviewedPersons);
                            }
                            if (objMgmtDocumentsModels.Approvers != "0")
                            {
                                string sApprovers = objMgmtDocumentsModels.Approvers;
                                string sApproveredPersons = sApprovers.Substring(2);
                                objMgmtDocumentsModels.Approvers = objGlobaldata.GetMultiHrEmpNameById(sApproveredPersons);
                            }

                            if (dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString() != ""
                                && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString(), out dtDocDate))
                            {
                                objMgmtDocumentsModels.ApprovedDate = dtDocDate;
                            }

                            objMgmtDocumentsList.MgmtDocumentsMList.Add(objMgmtDocumentsModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objMgmtDocumentsList.MgmtDocumentsMList.ToList());
        }

        //[AllowAnonymous]
        //public JsonResult MgmtDocumentsListSearchs(FormCollection form, string SearchText, string Approvestatus, int? page, string branch_name, string View)
        //{
        //    ViewBag.SubMenutype = "MgmtDocuments";

        //    MgmtDocumentsModelsList objMgmtDocumentsList = new MgmtDocumentsModelsList();
        //    objMgmtDocumentsList.MgmtDocumentsMList = new List<MgmtDocumentsModels>();

        //    MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
        //    try
        //    {
        //        //ViewBag.View = Request.QueryString["View"];

        //        //String DocumentType = objMgmtDocuments.GetDocumentTypeIdbyName("Policy");
        //        //String DocumentType1 = objMgmtDocuments.GetDocumentTypeIdbyName("Manual");
        //        //String DocumentType2 = objMgmtDocuments.GetDocumentTypeIdbyName("Procedure");
        //        //String DocumentType3 = objMgmtDocuments.GetDocumentTypeIdbyName("Forms");
        //        //String DocumentType4 = objMgmtDocuments.GetDocumentTypeIdbyName("Work Instructions");

        //        string sSqlstmt = "select idMgmt,Department, DocLevels, Doctype, ISOStds, AppClauses, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate,"
        //            + " DocUploadPath, ApprovedBy, (case when Approved_Status='1' then 'Approved' when Approved_Status='2' then 'Approval Rejected' else 'Not Approved' end) as Approved_Status," +
        //            "(case when Reviewed_Status='1' then 'Reviewed' when Reviewed_Status='2' then 'Review Rejected' else 'Not Reviewed' end) as Reviewed_Status, ApprovedDate, UploadedBy,branch,Location "
        //            + " from t_mgmt_documents where status=1";

        //        string sSearchtext = "";

        //        string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
        //        string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
        //        ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

        //        if (branch_name != null && branch_name != "")
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
        //            ViewBag.Branch_name = branch_name;
        //        }
        //        else
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
        //        }

        //        UserCredentials objUser = new UserCredentials();
        //        objUser = objGlobaldata.GetCurrentUserSession();

        //        string sDepartment = objGlobaldata.GetDeptNameById(objUser.DeptID);//retrieving employee deptid who logged in

        //        ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);

        //        ViewBag.CurrentEmpName = objUser.firstname;
        //        ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");

        //        ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();

        //        //searching the company name
        //        objMgmtDocuments.Department = form["Department"];//retrieving dept id
        //        string sSDepartment = objGlobaldata.GetMultiDeptNameById(form["Department"]);//retrieving name by id

        //        objMgmtDocuments.Doctype = form["Doctype"];
        //        string sSDocumentType = objGlobaldata.GetDocumentTypebyId(form["Doctype"]);

        //        //fetching deptid of Department="ALL"
        //        //string AllDeptId = objGlobaldata.GetDeptIDByName(DepartName.ToLower());

        //        if (objMgmtDocuments.Department == null)
        //        {
        //            objMgmtDocuments.Department = "";
        //        }

        //        if (Approvestatus == null)
        //        {
        //            Approvestatus = "";
        //        }

        //        if (objMgmtDocuments.Doctype == null)
        //        {
        //            objMgmtDocuments.Doctype = "";
        //        }

        //        if (!(objGlobaldata.GetMultiRolesNameById(objUser.role).Contains("Admin")))
        //        {
        //            sSearchtext = sSearchtext + " and (find_in_set('" + objUser.DeptID + "', view_by) or find_in_set('0', view_by))";
        //            //sSearchtext = sSearchtext + " and (FIND_IN_SET(Department ,'" + objGlobaldata.GetCurrentUserSession().DeptID + "'))";
        //        }

        //       sSqlstmt = sSqlstmt + sSearchtext + " order by Doctype";

        //        DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
        //        if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsMgmtDocumentsList.Tables[0].Rows.Count; i++)
        //            {
        //                DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["DocDate"].ToString());
        //                //string sDept = "";
        //                //if (dsMgmtDocumentsList.Tables[0].Rows[i]["Dept"].ToString() != "")
        //                //{
        //                //    sDept = objGlobaldata.GetDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Dept"].ToString());
        //                //}
        //                try
        //                {
        //                    MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels
        //                    {
        //                        idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["idMgmt"].ToString()),

        //                        Department = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Department"].ToString()),
        //                        DocLevels = objGlobaldata.GetDocumentLevelbyId(dsMgmtDocumentsList.Tables[0].Rows[i]["DocLevels"].ToString()),
        //                        Doctype = objGlobaldata.GetDocumentTypebyId(dsMgmtDocumentsList.Tables[0].Rows[i]["Doctype"].ToString()),
        //                        ISOStds = objGlobaldata.GetIsoStdNamebyId(dsMgmtDocumentsList.Tables[0].Rows[i]["ISOStds"].ToString()),
        //                        AppClauses = objMgmtDocuments.GetMultiISOClauseNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["AppClauses"].ToString()),
        //                        DocRef = dsMgmtDocumentsList.Tables[0].Rows[i]["DocRef"].ToString(),
        //                        DocName = dsMgmtDocumentsList.Tables[0].Rows[i]["DocName"].ToString(),
        //                        IssueNo = dsMgmtDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString(),
        //                        RevNo = dsMgmtDocumentsList.Tables[0].Rows[i]["RevNo"].ToString(),
        //                        DocDate = dtDocDate,
        //                        PreparedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["PreparedBy"].ToString()),
        //                        ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["ReviewedBy"].ToString()),
        //                        DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
        //                        ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
        //                        Approved_Status = dsMgmtDocumentsList.Tables[0].Rows[i]["Approved_Status"].ToString(),
        //                        Reviewed_Status = dsMgmtDocumentsList.Tables[0].Rows[i]["Reviewed_Status"].ToString(),
        //                        UploadedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["UploadedBy"].ToString()),
        //                        DocRetention = objGlobaldata.GetDropdownitemById(dsMgmtDocumentsList.Tables[0].Rows[i]["Doctype"].ToString()),
        //                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["branch"].ToString()),
        //                        Location = objGlobaldata.GetDivisionLocationById(dsMgmtDocumentsList.Tables[0].Rows[i]["Location"].ToString()),
        //                    };

        //                    if (dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString() != ""
        //                        && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString(), out dtDocDate))
        //                    {
        //                        objMgmtDocumentsModels.ApprovedDate = dtDocDate;
        //                    }

        //                    objMgmtDocumentsList.MgmtDocumentsMList.Add(objMgmtDocumentsModels);
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsList: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsList: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json("Success");
        //}

        //[AllowAnonymous]
        //public ActionResult MgmtDocumentsListSearch(FormCollection form, string SearchText, string Approvestatus, int? page)
        //{
        //    ViewBag.SubMenutype = "MgmtDocuments";

        //    MgmtDocumentsModelsList objMgmtDocumentsList = new MgmtDocumentsModelsList();
        //    objMgmtDocumentsList.MgmtDocumentsMList = new List<MgmtDocumentsModels>();

        //    MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();

        //    //ViewBag.View = Request.QueryString["View"];

        //    //String DocumentType = objMgmtDocuments.GetDocumentTypeIdbyName("Policy");
        //    //String DocumentType1 = objMgmtDocuments.GetDocumentTypeIdbyName("Manual");
        //    //String DocumentType2 = objMgmtDocuments.GetDocumentTypeIdbyName("Procedure");
        //    //String DocumentType3 = objMgmtDocuments.GetDocumentTypeIdbyName("Forms");
        //    //String DocumentType4 = objMgmtDocuments.GetDocumentTypeIdbyName("Work Instructions");

        //    try
        //    {
        //        string sSqlstmt = "select idMgmt,Department, DocLevels, Doctype, ISOStds, AppClauses, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate,"
        //            + " DocUploadPath, ApprovedBy, (case when Approved_Status='1' then 'Approved' when Approved_Status='2' then 'Approved Rejected' else 'Not Approved' end) as Approved_Status," +
        //            "(case when Reviewed_Status='1' then 'Reviewed' when Reviewed_Status='2' then 'Review Rejected' else 'Not Reviewed' end) as Reviewed_Status, ApprovedDate, UploadedBy,view_by,Location"
        //            + " from t_mgmt_documents where status=1";

        //        string sSearchtext = "", sApproveStatus = "", DepartName = "ALL";

        //        UserCredentials objUser = new UserCredentials();
        //        objUser = objGlobaldata.GetCurrentUserSession();

        //        string sDepartment = objGlobaldata.GetDeptNameById(objUser.DeptID);//retrieving employee deptid who logged in

        //        ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);

        //        ViewBag.CurrentEmpName = objUser.firstname;
        //        ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");

        //        ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();

        //        //searching the company name
        //        objMgmtDocuments.Department = form["Department"];//retrieving dept id
        //        string sSDepartment = objGlobaldata.GetMultiDeptNameById(form["Department"]);//retrieving name by id

        //        objMgmtDocuments.Doctype = form["Doctype"];
        //        string sSDocumentType = objGlobaldata.GetDocumentTypebyId(form["Doctype"]);

        //        //fetching deptid of Department="ALL"
        //        string AllDeptId = objGlobaldata.GetDeptIDByName(DepartName.ToLower());

        //        if (objMgmtDocuments.Department == null)
        //        {
        //            objMgmtDocuments.Department = "";
        //        }

        //        if (Approvestatus == null)
        //        {
        //            Approvestatus = "";
        //        }

        //        if (objMgmtDocuments.Doctype == null)
        //        {
        //            objMgmtDocuments.Doctype = "";
        //        }

        //        if (!(objGlobaldata.GetMultiRolesNameById(objUser.role).Contains("Admin")))
        //        {
        //            sSearchtext = sSearchtext + " and (Department='" + objUser.DeptID + "' or Department='" + AllDeptId + "')";
        //            //sSearchtext = sSearchtext + " and (FIND_IN_SET(Department ,'" + objGlobaldata.GetCurrentUserSession().DeptID + "'))";
        //        }

        //        //if (objMgmtDocuments.Department != null && sSDepartment != "ALL")

        //        sSqlstmt = sSqlstmt + sSearchtext + " order by DocLevels asc, DocRef asc";

        //        DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
        //        if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsMgmtDocumentsList.Tables[0].Rows.Count; i++)
        //            {
        //                DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["DocDate"].ToString());
        //                //string sDept = "";
        //                //if (dsMgmtDocumentsList.Tables[0].Rows[i]["Dept"].ToString() != "")
        //                //{
        //                //    sDept = objGlobaldata.GetDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Dept"].ToString());
        //                //}
        //                try
        //                {
        //                    MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels
        //                    {
        //                        idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["idMgmt"].ToString()),

        //                        Department = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Department"].ToString()),
        //                        DocLevels = objGlobaldata.GetDocumentLevelbyId(dsMgmtDocumentsList.Tables[0].Rows[i]["DocLevels"].ToString()),
        //                        Doctype = objGlobaldata.GetDocumentTypebyId(dsMgmtDocumentsList.Tables[0].Rows[i]["Doctype"].ToString()),
        //                        ISOStds = objGlobaldata.GetIsoStdNamebyId(dsMgmtDocumentsList.Tables[0].Rows[i]["ISOStds"].ToString()),
        //                        AppClauses = objMgmtDocuments.GetMultiISOClauseNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["AppClauses"].ToString()),
        //                        DocRef = dsMgmtDocumentsList.Tables[0].Rows[i]["DocRef"].ToString(),
        //                        DocName = dsMgmtDocumentsList.Tables[0].Rows[i]["DocName"].ToString(),
        //                        IssueNo = dsMgmtDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString(),
        //                        RevNo = dsMgmtDocumentsList.Tables[0].Rows[i]["RevNo"].ToString(),
        //                        DocDate = dtDocDate,
        //                        PreparedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["PreparedBy"].ToString()),
        //                        ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["ReviewedBy"].ToString()),
        //                        DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
        //                        ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
        //                        Approved_Status = dsMgmtDocumentsList.Tables[0].Rows[i]["Approved_Status"].ToString(),
        //                        Reviewed_Status = dsMgmtDocumentsList.Tables[0].Rows[i]["Reviewed_Status"].ToString(),
        //                        UploadedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["UploadedBy"].ToString()),
        //                        view_by = objGlobaldata.GetViewDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["view_by"].ToString()),
        //                        Location = objGlobaldata.GetDivisionLocationById(dsMgmtDocumentsList.Tables[0].Rows[i]["Location"].ToString()),
        //                    };

        //                    if (dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString() != ""
        //                        && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedDate"].ToString(), out dtDocDate))
        //                    {
        //                        objMgmtDocumentsModels.ApprovedDate = dtDocDate;
        //                    }

        //                    objMgmtDocumentsList.MgmtDocumentsMList.Add(objMgmtDocumentsModels);
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsListSearch: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsListSearch: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View(objMgmtDocumentsList.MgmtDocumentsMList.ToList().ToPagedList(page ?? 1, 1000));
        //}

        [AllowAnonymous]
        public ActionResult MgmtDocumentsApproveReject(string idMgmt, int iStatus, string PendingFlg, string Document, string DocName, string DocRef)
        {
            try
            {
                MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
                //string filename = Path.GetFileName(Document);
                //FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                string filename = "";
                System.IO.FileStream fsSource = null;

                if (Document != null && Document != "")
                {
                    if (Document.Contains(","))
                    {
                        string[] filearray = Document.Split(',');
                        fsSource = new FileStream(Server.MapPath(filearray[0]), FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                    else
                    {
                        filename = Path.GetFileName(Document);
                        fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                    }
                }
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
                if (objMgmtDocuments.FunMgmtDocumentApproveOrReject(idMgmt, iStatus, fsSource, filename, DocName, DocRef))
                {
                    TempData["Successdata"] = "Internal Document is " + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            if (PendingFlg != null && PendingFlg == "true")
            {
                string path = Request.CurrentExecutionFilePath;
                string[] url = path.Split('/');
                //var controller = url[1];
                if (url[1] == "MgmtDocuments" || url[2] == "MgmtDocuments")
                {
                    return RedirectToAction("MgmtDocumentsList", "MgmtDocuments");
                }
                else
                {
                    return RedirectToAction("ListPendingForApproval", "Dashboard");
                }
            }
            else
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
        }

        [AllowAnonymous]
        public ActionResult MgmtDocumentsReviewedApproveOrReject(string idMgmt, int iStatus, string PendingFlg, string Document, string DocName, string DocRef)
        {
            try
            {
                MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
                //string filename = Path.GetFileName(Document);
                //FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                string filename = "";
                System.IO.FileStream fsSource = null;

                if (Document != null && Document != "")
                {
                    if (Document.Contains(","))
                    {
                        string[] filearray = Document.Split(',');
                        fsSource = new FileStream(Server.MapPath(filearray[0]), FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                    else
                    {
                        filename = Path.GetFileName(Document);
                        fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                    }
                }
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
                if (objMgmtDocuments.FunMgmtDocumentsReviewApproveOrReject(idMgmt, iStatus, fsSource, filename, DocName, DocRef))
                {
                    TempData["Successdata"] = "Document Review " + sStatus + " successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                string path = Request.CurrentExecutionFilePath;
                string[] url = path.Split('/');
                //var controller = url[1];
                if (url[1] == "MgmtDocuments" || url[2] == "MgmtDocuments")
                {
                    return RedirectToAction("MgmtDocumentsList", "MgmtDocuments");
                }
                else
                {
                    return RedirectToAction("ListPendingForReview", "Dashboard");
                }
            }
            else
            {
                return RedirectToAction("MgmtDocumentsList");
            }
        }

        // GET: /MgmtDocuments/MgmtDocumentsHistoryList

        [AllowAnonymous]
        public ActionResult MgmtDocumentsHistoryList(string SearchText, string idMgmt, int? page)
        {
            ViewBag.SubMenutype = "MgmtDocuments";
            MgmtDocumentsModelsList objMgmtDocumentsList = new MgmtDocumentsModelsList();
            objMgmtDocumentsList.MgmtDocumentsMList = new List<MgmtDocumentsModels>();

            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();

            //idMgmt = Request.QueryString["idMgmt"];

            try
            {
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                string sSqlstmt = "select idMgmtTrans, idMgmt, DocLevels, Doctype, ISOStds, AppClauses, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy,"
                            + "DocDate, DocUploadPath, TransDate, ApprovedBy, Approved_Status, ApprovedDate, UploadedBy from t_mgmt_documents_trans where idMgmt='"
                            + idMgmt + "'";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSqlstmt = sSqlstmt + " and (DocRef ='" + SearchText + "' or DocRef like '" + SearchText + "%' or DocName='" + SearchText
                        + "' or DocName like '" + SearchText + "%')";
                }

                sSqlstmt = sSqlstmt + " order by DocLevels, idmgmt desc";
                DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsMgmtDocumentsList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["DocDate"].ToString());
                        //DateTime dtTransDate;

                        //if (dsMgmtDocumentsList.Tables[0].Rows[i]["TransDate"].ToString() != null)
                        //{
                        //    dtTransDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["TransDate"].ToString());
                        //}
                        //string sDept = "";
                        //if (dsMgmtDocumentsList.Tables[0].Rows[i]["Dept"].ToString() != "")
                        //{
                        //    sDept = objGlobaldata.GetDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Dept"].ToString());
                        //}
                        try
                        {
                            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels
                            {
                                idMgmtTrans = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["idMgmtTrans"].ToString()),
                                idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["idMgmt"].ToString()),

                                DocLevels = (dsMgmtDocumentsList.Tables[0].Rows[i]["DocLevels"].ToString()),
                                Doctype = dsMgmtDocumentsList.Tables[0].Rows[i]["Doctype"].ToString(),
                                ISOStds = objGlobaldata.GetIsoStdNamebyId(dsMgmtDocumentsList.Tables[0].Rows[i]["ISOStds"].ToString()),
                                AppClauses = objMgmtDocuments.GetMultiISOClauseNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["AppClauses"].ToString()),
                                DocRef = dsMgmtDocumentsList.Tables[0].Rows[i]["DocRef"].ToString(),
                                DocName = dsMgmtDocumentsList.Tables[0].Rows[i]["DocName"].ToString(),
                                IssueNo = dsMgmtDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString(),
                                RevNo = dsMgmtDocumentsList.Tables[0].Rows[i]["RevNo"].ToString(),
                                DocDate = dtDocDate,
                                PreparedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["PreparedBy"].ToString()),
                                ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["ReviewedBy"].ToString()),
                                DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                                ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                UploadedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["UploadedBy"].ToString())
                            };

                            if (dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedDate"].ToString() != ""
                                && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedDate"].ToString(), out dtDocDate))
                            {
                                objMgmtDocumentsModels.ApprovedDate = dtDocDate;
                            }

                            if (dsMgmtDocumentsList.Tables[0].Rows[0]["TransDate"].ToString() != ""
                                && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[0]["TransDate"].ToString(), out dtDocDate))
                            {
                                objMgmtDocumentsModels.TransDate = dtDocDate;
                            }

                            objMgmtDocumentsList.MgmtDocumentsMList.Add(objMgmtDocumentsModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsHistoryList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsHistoryList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            ViewBag.idMgmt = idMgmt;

            return View(objMgmtDocumentsList.MgmtDocumentsMList.ToList());
        }

        // GET: /MgmtDocuments/MgmtDocumentsDetails

        [AllowAnonymous]
        public ActionResult MgmtDocumentsDetails()
        {
            ViewBag.SubMenutype = "MgmtDocuments";
            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels();
            UserCredentials objUser = new UserCredentials();
            objUser = objGlobaldata.GetCurrentUserSession();
            ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);
            try
            {
                if (Request.QueryString["idMgmt"] != null && Request.QueryString["idMgmt"] != "")
                {
                    string idMgmt = Request.QueryString["idMgmt"];
                    ViewBag.idMgmt = idMgmt;

                    ViewBag.UserRole = objGlobaldata.GetRoleName(objGlobaldata.GetCurrentUserSession().role);

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                    string sSqlstmt = "select idMgmt, DocLevels,Department, Doctype, ISOStds, AppClauses, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate,"
                        + " DocUploadPath, ApprovedBy, (case when Approved_Status='1' then 'Approved' when Approved_Status='2' then 'Rejected' else 'Not Approved' end) as Approved_Status,"
                        + "(case when Reviewed_Status = '1' then 'Reviewed' when Reviewed_Status = '2' then 'Review Rejected' else 'Not Reviewed' end) as Reviewed_Status, ApprovedDate,ReviewedDate, "
                        + "UploadedBy,branch,Location,Reviewers,Approvers,Reviewed_Status as Reviewed_StatusId,Approved_Status as Approved_StatusId,dcr_no,view_by from t_mgmt_documents where idmgmt='" + idMgmt + "'";

                    DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString());

                        //string sDept = "";
                        //if (dsMgmtDocumentsList.Tables[0].Rows[0]["Department"].ToString() != "")
                        //{
                        //    sDept = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["Department"].ToString());
                        //}
                        objMgmtDocumentsModels = new MgmtDocumentsModels
                        {
                            idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["idMgmt"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["Department"].ToString()),
                            DocLevels = objGlobaldata.GetDocumentLevelbyId(dsMgmtDocumentsList.Tables[0].Rows[0]["DocLevels"].ToString()),
                            Doctype = objGlobaldata.GetDocumentTypebyId(dsMgmtDocumentsList.Tables[0].Rows[0]["Doctype"].ToString()),
                            ISOStds = objGlobaldata.GetIsoStdNamebyId(dsMgmtDocumentsList.Tables[0].Rows[0]["ISOStds"].ToString()),
                            AppClauses = objMgmtDocumentsModels.GetMultiISOClauseNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["AppClauses"].ToString()),
                            DocRef = dsMgmtDocumentsList.Tables[0].Rows[0]["DocRef"].ToString(),
                            DocName = dsMgmtDocumentsList.Tables[0].Rows[0]["DocName"].ToString(),
                            IssueNo = dsMgmtDocumentsList.Tables[0].Rows[0]["IssueNo"].ToString(),
                            RevNo = dsMgmtDocumentsList.Tables[0].Rows[0]["RevNo"].ToString(),
                            DocDate = dtDocDate,
                            PreparedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["PreparedBy"].ToString()),
                            ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                            DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            ReviewedById = (dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                            ApprovedById = (dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            Reviewed_Status = dsMgmtDocumentsList.Tables[0].Rows[0]["Reviewed_Status"].ToString(),
                            Approved_Status = dsMgmtDocumentsList.Tables[0].Rows[0]["Approved_Status"].ToString(),
                            UploadedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["UploadedBy"].ToString()),
                            DocRetention = objGlobaldata.GetDropdownitemById(dsMgmtDocumentsList.Tables[0].Rows[0]["Doctype"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsMgmtDocumentsList.Tables[0].Rows[0]["Location"].ToString()),
                            Reviewers = dsMgmtDocumentsList.Tables[0].Rows[0]["Reviewers"].ToString(),
                            Approvers = dsMgmtDocumentsList.Tables[0].Rows[0]["Approvers"].ToString(),
                            Reviewed_StatusId = dsMgmtDocumentsList.Tables[0].Rows[0]["Reviewed_StatusId"].ToString(),
                            Approved_StatusId = dsMgmtDocumentsList.Tables[0].Rows[0]["Approved_StatusId"].ToString(),
                            dcr_no = objGlobaldata.GetDCRNOById(dsMgmtDocumentsList.Tables[0].Rows[0]["dcr_no"].ToString()),
                            view_by = objGlobaldata.GetViewDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["view_by"].ToString()),
                        };
                        if (objMgmtDocumentsModels.Reviewers != "0")
                        {
                            string sReviewer = objMgmtDocumentsModels.Reviewers;
                            string sReviewedPersons = sReviewer.Substring(2);
                            objMgmtDocumentsModels.Reviewers = objGlobaldata.GetMultiHrEmpNameById(sReviewedPersons);
                        }
                        if (objMgmtDocumentsModels.Approvers != "0")
                        {
                            string sApprovers = objMgmtDocumentsModels.Approvers;
                            string sApproveredPersons = sApprovers.Substring(2);
                            objMgmtDocumentsModels.Approvers = objGlobaldata.GetMultiHrEmpNameById(sApproveredPersons);
                        }
                        sSqlstmt = "select  CommentsId, idMgmt, emp_firstname, Comments, CommentDate "
                            + " from t_mgmt_docs_comments as tmdc, t_hr_employee as emp where tmdc.empId = emp.emp_no and idMgmt='" + idMgmt + "'";
                        DataSet dsCommnets = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.dsComments = dsCommnets;

                        if (dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedDate"].ToString() != ""
                            && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedDate"].ToString(), out dtDocDate))
                        {
                            objMgmtDocumentsModels.ApprovedDate = dtDocDate;
                        }

                        if (dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedDate"].ToString() != ""
                            && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedDate"].ToString(), out dtDocDate))
                        {
                            objMgmtDocumentsModels.ReviewedDate = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("MgmtDocumentsList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Document Id cannot be Null or empty";
                    return RedirectToAction("MgmtDocumentsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objMgmtDocumentsModels);
        }

        [AllowAnonymous]
        public ActionResult MgmtDocumentsInfo(int id)
        {
            ViewBag.SubMenutype = "MgmtDocuments";
            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels();
            UserCredentials objUser = new UserCredentials();
            try
            {
                string sSqlstmt = "select idMgmt, DocLevels,Dept, Doctype, ISOStds, AppClauses, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate,"
                    + " DocUploadPath, ApprovedBy, (case when Approved_Status='1' then 'Approved' when Approved_Status='2' then 'Rejected' else 'Not Approved' end) as Approved_Status, ApprovedDate,Department,ReviewedDate, "
                    + "UploadedBy,branch,Location,dcr_no from t_mgmt_documents where idmgmt='" + id + "'";

                DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                {
                    objMgmtDocumentsModels = new MgmtDocumentsModels
                    {
                        idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["idMgmt"].ToString()),
                        DocLevels = objGlobaldata.GetDocumentLevelbyId(dsMgmtDocumentsList.Tables[0].Rows[0]["DocLevels"].ToString()),
                        Doctype = objGlobaldata.GetDocumentTypebyId(dsMgmtDocumentsList.Tables[0].Rows[0]["Doctype"].ToString()),
                        ISOStds = objGlobaldata.GetIsoStdNamebyId(dsMgmtDocumentsList.Tables[0].Rows[0]["ISOStds"].ToString()),
                        AppClauses = objMgmtDocumentsModels.GetMultiISOClauseNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["AppClauses"].ToString()),
                        DocRef = dsMgmtDocumentsList.Tables[0].Rows[0]["DocRef"].ToString(),
                        DocName = dsMgmtDocumentsList.Tables[0].Rows[0]["DocName"].ToString(),
                        IssueNo = dsMgmtDocumentsList.Tables[0].Rows[0]["IssueNo"].ToString(),
                        RevNo = dsMgmtDocumentsList.Tables[0].Rows[0]["RevNo"].ToString(),
                        PreparedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["PreparedBy"].ToString()),
                        ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                        DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                        ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                        Approved_Status = dsMgmtDocumentsList.Tables[0].Rows[0]["Approved_Status"].ToString(),
                        UploadedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["UploadedBy"].ToString()),
                        Department = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["Department"].ToString()),
                        DocRetention = objGlobaldata.GetDropdownitemById(dsMgmtDocumentsList.Tables[0].Rows[0]["Doctype"].ToString()),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString()),
                        Location = objGlobaldata.GetDivisionLocationById(dsMgmtDocumentsList.Tables[0].Rows[0]["Location"].ToString()),
                        dcr_no = objGlobaldata.GetDCRNOById(dsMgmtDocumentsList.Tables[0].Rows[0]["dcr_no"].ToString()),
                    };
                    DateTime dtDocDate;
                    if (dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString() != ""
                        && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString(), out dtDocDate))
                    {
                        objMgmtDocumentsModels.DocDate = dtDocDate;
                    }
                    if (dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedDate"].ToString() != ""
                        && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedDate"].ToString(), out dtDocDate))
                    {
                        objMgmtDocumentsModels.ApprovedDate = dtDocDate;
                    }
                    if (dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedDate"].ToString() != ""
                      && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedDate"].ToString(), out dtDocDate))
                    {
                        objMgmtDocumentsModels.ReviewedDate = dtDocDate;
                    }
                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("MgmtDocumentsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objMgmtDocumentsModels);
        }

        // POST: /MgmtDocuments/MgmtDocumentsDetails

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MgmtDocumentsDetails(MgmtDocumentsModels objMgmtDocumentsModels, FormCollection form)
        {
            try
            {
                if (form["idMgmt"] != null && form["idMgmt"] != "")
                {
                    string idMgmt = form["idMgmt"], sComments = form["Comment"], User = "";

                    User = objGlobaldata.GetCurrentUserSession().empid;

                    if (objMgmtDocumentsModels.FunAddComments(User, sComments, idMgmt))
                    {
                        TempData["Successdata"] = "Document Comments details added successfully";
                        return RedirectToAction("MgmtDocumentsDetails", new { idMgmt = idMgmt });
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("MgmtDocumentsList");
        }

        // GET: /MgmtDocuments/MgmtDocumentsEdit

        [AllowAnonymous]
        public ActionResult MgmtDocumentsEdit()
        {
            ViewBag.SubMenutype = "MgmtDocuments";
            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels();

            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                if (Request.QueryString["idMgmt"] != null && Request.QueryString["idMgmt"] != "")
                {
                    string idMgmt = Request.QueryString["idMgmt"];

                    ViewBag.idMgmt = idMgmt;

                    // ViewBag.PreparerList = objGlobaldata.GetHrEmployeeListbox();//all internal active employees
                    ViewBag.ReviewerList = objGlobaldata.GetReviewer();//role=1+active+internal
                    ViewBag.ApproverList = objGlobaldata.GetApprover();//role=2

                    // ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox('I');
                    //ViewBag.DeptList = objGlobaldata.GetDepartmentWithIdListbox();
                    ViewBag.DocLevels = objGlobaldata.GetDocLevelsList();
                    ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList("");
                    //ViewBag.CompanyName = objGlobaldata.GetCompanyBranchListbox();
                    // ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();
                    ViewBag.Views = objGlobaldata.GetViewsListbox();
                    ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();

                    //ViewBag.ISOStds = objGlobaldata.GetAllIsoStdListbox();

                    DropdownMultiItemsList dplist = new DropdownMultiItemsList();
                    dplist.DropdownList = new List<DropdownMultiItems>();
                    try
                    {
                        DropdownMultiItems Isostd = new DropdownMultiItems();
                        DataSet dsEmp = objGlobaldata.Getdetails("select StdId, IsoName,Descriptions from t_isostandards where Active=1");
                        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                            {
                                Isostd = new DropdownMultiItems()
                                {
                                    Id = dsEmp.Tables[0].Rows[i]["StdId"].ToString(),
                                    Name = dsEmp.Tables[0].Rows[i]["IsoName"].ToString(),
                                    Desc = dsEmp.Tables[0].Rows[i]["Descriptions"].ToString()
                                };
                                dplist.DropdownList.Add(Isostd);
                            }
                            ViewBag.ISOStds = dplist;
                        }
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsEdit: " + ex.ToString());
                    }

                    string sSqlstmt = "select idMgmt, DocLevels, Dept,Department, Doctype, ISOStds, AppClauses, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate, "
                        + "DocUploadPath, ApprovedBy, Approved_Status, UploadedBy,view_by,branch,Location,dcr_no from t_mgmt_documents where idmgmt='" + idMgmt + "'";

                    DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString());
                        string sDept = "";
                        if (dsMgmtDocumentsList.Tables[0].Rows[0]["Dept"].ToString() != "")
                        {
                            sDept = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["Dept"].ToString());
                        }

                        objMgmtDocumentsModels = new MgmtDocumentsModels
                        {
                            idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["idMgmt"].ToString()),
                            Dept = sDept,
                            DocLevels = (dsMgmtDocumentsList.Tables[0].Rows[0]["DocLevels"].ToString()),
                            Doctype = /*objGlobaldata.GetDocumentTypebyId*/(dsMgmtDocumentsList.Tables[0].Rows[0]["Doctype"].ToString()),
                            ISOStds =/* objGlobaldata.GetIsoStdDescriptionById*/(dsMgmtDocumentsList.Tables[0].Rows[0]["ISOStds"].ToString()),
                            AppClauses = /*objMgmtDocumentsModels.GetMultiISOClauseNameById*/(dsMgmtDocumentsList.Tables[0].Rows[0]["AppClauses"].ToString()),
                            DocRef = dsMgmtDocumentsList.Tables[0].Rows[0]["DocRef"].ToString(),
                            DocName = dsMgmtDocumentsList.Tables[0].Rows[0]["DocName"].ToString(),
                            IssueNo = dsMgmtDocumentsList.Tables[0].Rows[0]["IssueNo"].ToString(),
                            RevNo = dsMgmtDocumentsList.Tables[0].Rows[0]["RevNo"].ToString(),
                            DocDate = dtDocDate,
                            PreparedBy = (dsMgmtDocumentsList.Tables[0].Rows[0]["PreparedBy"].ToString()),
                            ReviewedBy = /*objGlobaldata.GetEmpHrNameById*/(dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                            DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            ApprovedBy = /*objGlobaldata.GetEmpHrNameById*/(dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            Department =/* objGlobaldata.GetMultiDeptNameById*/(dsMgmtDocumentsList.Tables[0].Rows[0]["Department"].ToString()),
                            view_by = (dsMgmtDocumentsList.Tables[0].Rows[0]["view_by"].ToString()),
                            branch = (dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = /*objGlobaldata.GetDivisionLocationById*/(dsMgmtDocumentsList.Tables[0].Rows[0]["Location"].ToString()),
                            dcr_no = (dsMgmtDocumentsList.Tables[0].Rows[0]["dcr_no"].ToString()),
                        };
                        ViewBag.AppClauses = objMgmtDocumentsModels.GetMultiISOClauseList(dsMgmtDocumentsList.Tables[0].Rows[0]["ISOStds"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.DocType = objGlobaldata.GetDocTypeListbyDocLevels(dsMgmtDocumentsList.Tables[0].Rows[0]["DocLevels"].ToString());
                        ViewBag.DCRDoc = objGlobaldata.GetDCRDocumentList(dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString(), dsMgmtDocumentsList.Tables[0].Rows[0]["Location"].ToString());
                        ViewBag.PreparerList = objGlobaldata.GetGEmpListbymultiBranch(dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString());
                    }
                    else
                    {
                        ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                        TempData["alertdata"] = "Document Id cannot be Null or empty";
                        return RedirectToAction("MgmtDocumentsList");
                    }
                }
                else
                {
                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                    TempData["alertdata"] = "Document Id cannot be Null or empty";
                    return RedirectToAction("MgmtDocumentsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("MgmtDocumentsList");
            }
            return View(objMgmtDocumentsModels);
        }

        // POST: /MgmtDocuments/MgmtDocumentsEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MgmtDocumentsEdit(MgmtDocumentsModels objMgmtDocumentsModels, FormCollection form, HttpPostedFileBase file)
        {
            string OldIssueNo = objMgmtDocumentsModels.IssueNo;
            string OldRevNo = objMgmtDocumentsModels.RevNo;
            string sDocName = form["DocName"], sDocRef = form["DocRef"];
            //if (objMgmtDocumentsModels != null && objMgmtDocumentsModels.CheckForDuplicate(sDocName, sDocRef))
            try
            {
                objMgmtDocumentsModels.DocName = sDocName;
                objMgmtDocumentsModels.DocLevels = form["DocLevels"];
                objMgmtDocumentsModels.DocRef = sDocRef;
                objMgmtDocumentsModels.Doctype = form["Doctype"];
                objMgmtDocumentsModels.ISOStds = form["ISOStds"];
                objMgmtDocumentsModels.PreparedBy = form["PreparedBy"];
                objMgmtDocumentsModels.ReviewedBy = form["ReviewedBy"];
                objMgmtDocumentsModels.ApprovedBy = form["ApprovedBy"];
                objMgmtDocumentsModels.IssueNo = form["IssueNoNew"];
                objMgmtDocumentsModels.RevNo = (form["RevNoNew"]);
                objMgmtDocumentsModels.view_by = (form["view_by"]);
                objMgmtDocumentsModels.UploadedBy = objGlobaldata.GetCurrentUserSession().empid;

                objMgmtDocumentsModels.AppClauses = form["AppClauses"];
                objMgmtDocumentsModels.Location = form["Location"];
                objMgmtDocumentsModels.Department = form["Department"];
                objMgmtDocumentsModels.idMgmt = Convert.ToInt16(form["idMgmt"]);
                //objMgmtDocumentsModels.branch = form["branch"];

                string supload = form["upload"];

                if (objMgmtDocumentsModels.branch == null || objMgmtDocumentsModels.branch == "")
                {
                    objMgmtDocumentsModels.branch = objGlobaldata.GetCurrentUserSession().division;
                }
                DateTime dateValue;

                if (DateTime.TryParse(form["DocDate"], out dateValue) == true)
                {
                    objMgmtDocumentsModels.DocDate = dateValue;
                }

                if (supload != null && supload != "")
                {
                    objMgmtDocumentsModels.DocUploadPath = supload;
                }
                else if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/IMSDocs"), Path.GetFileName(file.FileName));
                        string sFilename = objMgmtDocumentsModels.DocName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objMgmtDocumentsModels.DocUploadPath = "~/DataUpload/MgmtDocs/IMSDocs/" + sFilename;
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

                if (objMgmtDocumentsModels.FunUpdateMgmtDocuments(objMgmtDocumentsModels, file, OldIssueNo, OldRevNo))
                {
                    TempData["Successdata"] = "Document details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("MgmtDocumentsList");
        }

        // GET: /MgmtDocuments/AddAnnexure

        [AllowAnonymous]
        public ActionResult AddAnnexure()
        {
            return InitilizeAddAnnexure();
        }

        // GET: /MgmtDocuments/AddMgmtDocuments

        private ActionResult InitilizeAddAnnexure()
        {
            try
            {
                ViewBag.SubMenutype = "MgmtDocuments";
                int iidMgmt = 0;
                int.TryParse(Request.QueryString["idMgmt"], out iidMgmt);
                ViewBag.idMgmt = iidMgmt;
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels();

                ViewBag.DocType = objGlobaldata.GetDocumentTypebyId(objMgmtDocumentsModels.GetDocName(iidMgmt));
                objGlobaldata.GetDeptHeadList("");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddAnnexure: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View();
        }

        // POST: /MgmtDocuments/AddAnnexure

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAnnexure(MgmtDocumentsModels objMgmtDocuments, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                if (objMgmtDocuments != null)
                {
                    objMgmtDocuments.idMgmt = Convert.ToInt16(form["idMgmt"]);

                    DateTime dateValue;

                    if (DateTime.TryParse(form["DocDate"], out dateValue) == true)
                    {
                        objMgmtDocuments.DocDate = dateValue;
                    }

                    objMgmtDocuments.PreparedBy = form["PreparedBy"];
                    objMgmtDocuments.ReviewedBy = form["ReviewedBy"];
                    objMgmtDocuments.ApprovedBy = form["ApprovedBy"];

                    if (file != null && file.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/IMSDocs"), Path.GetFileName(file.FileName));
                            string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + objMgmtDocuments.DocName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                            objMgmtDocuments.DocUploadPath = "~/DataUpload/MgmtDocs/IMSDocs/" + objMgmtDocuments.DocName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename;
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

                    if (objMgmtDocuments.FunAddAnnexure(objMgmtDocuments, file))
                    {
                        TempData["Successdata"] = "Added Annexure details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAnnexure: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AnnexureList", new { idMgmt = objMgmtDocuments.idMgmt });
        }

        // GET: /MgmtDocuments/AnnexureList

        [AllowAnonymous]
        public ActionResult AnnexureList(int? page)
        {
            ViewBag.SubMenutype = "MgmtDocuments";
            MgmtDocumentsModelsList objMgmtDocumentsList = new MgmtDocumentsModelsList();
            objMgmtDocumentsList.MgmtDocumentsMList = new List<MgmtDocumentsModels>();

            try
            {
                int iMgmtId = 0;
                int.TryParse(Request.QueryString["idMgmt"], out iMgmtId);

                if (iMgmtId > 0)
                {
                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                    string sSqlstmt = "select idAnnexure, MgmtId, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate, DocUploadPath, ApprovedBy, ApprovedStatus "
                        + "from t_mgmt_annexure where Status=1 and MgmtId='" + iMgmtId + "'";

                    DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsMgmtDocumentsList.Tables[0].Rows.Count; i++)
                        {
                            DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["DocDate"].ToString());

                            try
                            {
                                MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels
                                {
                                    idAnnexure = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["idAnnexure"].ToString()),
                                    idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["MgmtId"].ToString()),
                                    DocRef = dsMgmtDocumentsList.Tables[0].Rows[i]["DocRef"].ToString(),
                                    DocName = dsMgmtDocumentsList.Tables[0].Rows[i]["DocName"].ToString(),
                                    IssueNo = dsMgmtDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString(),
                                    RevNo = dsMgmtDocumentsList.Tables[0].Rows[i]["RevNo"].ToString(),
                                    DocDate = dtDocDate,
                                    PreparedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["PreparedBy"].ToString()),
                                    // ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                                    DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                                    ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                                    ApprovedStatus = dsMgmtDocumentsList.Tables[0].Rows[i]["ApprovedStatus"].ToString()
                                };

                                objMgmtDocumentsList.MgmtDocumentsMList.Add(objMgmtDocumentsModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AnnexureList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }

                    ViewBag.idMgmt = iMgmtId;
                    MgmtDocumentsModels objMgmtDoc = new MgmtDocumentsModels();
                    ViewBag.GetDocType = objGlobaldata.GetDocumentTypebyId(objMgmtDoc.GetDocumentTypeIdbyMgtId(Convert.ToInt16(iMgmtId).ToString()));

                    return View(objMgmtDocumentsList.MgmtDocumentsMList.ToList());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AnnexureList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("MgmtDocumentsList");
        }

        // GET: /MgmtDocuments/AnnexureHistoryList

        [AllowAnonymous]
        public ActionResult AnnexureHistoryList()
        {
            ViewBag.SubMenutype = "MgmtDocuments";
            MgmtDocumentsModelsList objMgmtDocumentsList = new MgmtDocumentsModelsList();
            objMgmtDocumentsList.MgmtDocumentsMList = new List<MgmtDocumentsModels>();

            try
            {
                int idAnnexure = 0;
                int.TryParse(Request.QueryString["idAnnexure"], out idAnnexure);

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                string sSqlstmt = "select idAnnexTrans, idAnnexure, MgmtId, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate, DocUploadPath, TransDate, ApprovedBy "
                                        + " from t_mgmt_annexure_trans where idAnnexure='" + idAnnexure + "'";

                DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsMgmtDocumentsList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["DocDate"].ToString());
                        DateTime dtTransDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["TransDate"].ToString());

                        try
                        {
                            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels
                            {
                                idAnnexTrans = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["idAnnexTrans"].ToString()),
                                idAnnexure = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["idAnnexure"].ToString()),
                                idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["MgmtId"].ToString()),
                                DocRef = dsMgmtDocumentsList.Tables[0].Rows[i]["DocRef"].ToString(),
                                DocName = dsMgmtDocumentsList.Tables[0].Rows[i]["DocName"].ToString(),
                                IssueNo = dsMgmtDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString(),
                                RevNo = dsMgmtDocumentsList.Tables[0].Rows[i]["RevNo"].ToString(),
                                DocDate = dtDocDate,
                                PreparedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["PreparedBy"].ToString()),
                                //ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                                DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                                TransDate = dtTransDate,
                                ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedBy"].ToString())
                            };

                            ViewBag.GetDocType = objGlobaldata.GetDocumentTypebyId(objMgmtDocumentsModels.GetDocumentTypeIdbyMgtId(Convert.ToInt16(objMgmtDocumentsModels.idMgmt).ToString()));
                            /* objGlobaldata.GetDocumentTypebyId(objMgmtDocumentsModels.GetDocName(objMgmtDocumentsModels.idMgmt));*/
                            objMgmtDocumentsList.MgmtDocumentsMList.Add(objMgmtDocumentsModels);
                            ViewBag.idMgmt = objMgmtDocumentsModels.idMgmt;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AnnexureHistoryList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AnnexureHistoryList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objMgmtDocumentsList.MgmtDocumentsMList.ToList());
        }

        // GET: /MgmtDocuments/AnnexureDetails

        [AllowAnonymous]
        public ActionResult AnnexureDetails()
        {
            ViewBag.SubMenutype = "MgmtDocuments";
            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels();

            try
            {
                if (Request.QueryString["idAnnexure"] != null && Request.QueryString["idAnnexure"] != "")
                {
                    string idAnnexure = Request.QueryString["idAnnexure"];

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                    string sSqlstmt = "select idAnnexure, MgmtId, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate, DocUploadPath ,ApprovedBy "
                        + "from t_mgmt_annexure where idAnnexure='" + idAnnexure + "'";

                    DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString());

                        objMgmtDocumentsModels = new MgmtDocumentsModels
                        {
                            idAnnexure = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["idAnnexure"].ToString()),
                            idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["MgmtId"].ToString()),
                            DocRef = dsMgmtDocumentsList.Tables[0].Rows[0]["DocRef"].ToString(),
                            DocName = dsMgmtDocumentsList.Tables[0].Rows[0]["DocName"].ToString(),
                            IssueNo = dsMgmtDocumentsList.Tables[0].Rows[0]["IssueNo"].ToString(),
                            RevNo = dsMgmtDocumentsList.Tables[0].Rows[0]["RevNo"].ToString(),
                            DocDate = dtDocDate,
                            PreparedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["PreparedBy"].ToString()),
                            ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                            DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedBy"].ToString())
                        };
                        ViewBag.idMgmt = objMgmtDocumentsModels.idMgmt;
                        ViewBag.GetDocType = objGlobaldata.GetDocumentTypebyId(objMgmtDocumentsModels.GetDocumentTypeIdbyMgtId(Convert.ToInt16(objMgmtDocumentsModels.idMgmt).ToString()));
                        /*objGlobaldata.GetDocumentTypebyId(objMgmtDocumentsModels.GetDocName(objMgmtDocumentsModels.idMgmt));*/

                        return View(objMgmtDocumentsModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "Annexure Id cannot be Null or empty";
                        return RedirectToAction("AnnexureList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Annexure Id cannot be Null or empty";
                    return RedirectToAction("AnnexureList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AnnexureDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AnnexureList");
        }

        [AllowAnonymous]
        public ActionResult AnnexureInfo(int id)
        {
            ViewBag.SubMenutype = "MgmtDocuments";
            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels();

            try
            {
                string sSqlstmt = "select idAnnexure, MgmtId, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate, DocUploadPath ,ApprovedBy "
                    + "from t_mgmt_annexure where idAnnexure='" + id + "'";

                DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                {
                    DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString());

                    objMgmtDocumentsModels = new MgmtDocumentsModels
                    {
                        idAnnexure = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["idAnnexure"].ToString()),
                        idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["MgmtId"].ToString()),
                        DocRef = dsMgmtDocumentsList.Tables[0].Rows[0]["DocRef"].ToString(),
                        DocName = dsMgmtDocumentsList.Tables[0].Rows[0]["DocName"].ToString(),
                        IssueNo = dsMgmtDocumentsList.Tables[0].Rows[0]["IssueNo"].ToString(),
                        RevNo = dsMgmtDocumentsList.Tables[0].Rows[0]["RevNo"].ToString(),
                        DocDate = dtDocDate,
                        PreparedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["PreparedBy"].ToString()),
                        ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                        ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                        DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[0]["DocUploadPath"].ToString()
                    };

                    return View(objMgmtDocumentsModels);
                }
                else
                {
                    TempData["alertdata"] = "Annexure Id cannot be Null or empty";
                    return RedirectToAction("AnnexureList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AnnexureDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AnnexureList");
        }

        [AllowAnonymous]
        public ActionResult AnnexureEdit()
        {
            ViewBag.SubMenutype = "MgmtDocuments";
            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels();

            try
            {
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                if (Request.QueryString["idAnnexure"] != null && Request.QueryString["idAnnexure"] != "")
                {
                    string idAnnexure = Request.QueryString["idAnnexure"];

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                    string sSqlstmt = "select idAnnexure, MgmtId, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate, DocUploadPath, ApprovedBy "
                        + " from t_mgmt_annexure where idAnnexure='" + idAnnexure + "'";

                    DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString());

                        objMgmtDocumentsModels = new MgmtDocumentsModels
                        {
                            idAnnexure = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["idAnnexure"].ToString()),
                            idMgmt = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["MgmtId"].ToString()),
                            DocRef = dsMgmtDocumentsList.Tables[0].Rows[0]["DocRef"].ToString(),
                            DocName = dsMgmtDocumentsList.Tables[0].Rows[0]["DocName"].ToString(),
                            IssueNo = dsMgmtDocumentsList.Tables[0].Rows[0]["IssueNo"].ToString(),
                            RevNo = dsMgmtDocumentsList.Tables[0].Rows[0]["RevNo"].ToString(),
                            DocDate = dtDocDate,
                            PreparedBy = (dsMgmtDocumentsList.Tables[0].Rows[0]["PreparedBy"].ToString()),
                            ReviewedBy = (dsMgmtDocumentsList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            DocUploadPath = dsMgmtDocumentsList.Tables[0].Rows[0]["DocUploadPath"].ToString()
                        };
                        ViewBag.GetDocType = objGlobaldata.GetDocumentTypebyId(objMgmtDocumentsModels.GetDocumentTypeIdbyMgtId(Convert.ToInt16(objMgmtDocumentsModels.idMgmt).ToString())); /*objGlobaldata.GetDocumentTypebyId(objMgmtDocumentsModels.GetDocName(objMgmtDocumentsModels.idMgmt));*/
                    }
                    else
                    {
                        TempData["alertdata"] = "Annexure Id cannot be Null or empty";
                        return RedirectToAction("AnnexureList");
                    }

                    ViewBag.idAnnexure = objMgmtDocumentsModels.idAnnexure;
                    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();

                    return View(objMgmtDocumentsModels);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AnnexureEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MgmtDocumentsList");
        }

        // POST: /MgmtDocuments/AnnexureEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AnnexureEdit(MgmtDocumentsModels objMgmtDocumentsModels, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                int idAnnexure = 0;
                int.TryParse(form["idAnnexure"], out idAnnexure);
                objMgmtDocumentsModels.idAnnexure = idAnnexure;

                DateTime dateValue;

                if (DateTime.TryParse(form["DocDate"], out dateValue) == true)
                {
                    objMgmtDocumentsModels.DocDate = dateValue;
                }

                objMgmtDocumentsModels.PreparedBy = form["PreparedBy"];
                // objMgmtDocumentsModels.ReviewedBy = form["ReviewedBy"];
                objMgmtDocumentsModels.ApprovedBy = form["ApprovedBy"];
                objMgmtDocumentsModels.IssueNo = form["IssueNo"];

                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/IMSDocs"), Path.GetFileName(file.FileName));
                        string sFilename = objMgmtDocumentsModels.DocName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objMgmtDocumentsModels.DocUploadPath = "~/DataUpload/MgmtDocs/IMSDocs/" + sFilename;
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

                if (objMgmtDocumentsModels.FunUpdateAnnexure(objMgmtDocumentsModels))
                {
                    TempData["Successdata"] = "Annexure details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AnnexureEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AnnexureList", new { idMgmt = objMgmtDocumentsModels.idMgmt });
        }

        [AllowAnonymous]
        public ActionResult AnnexureApproval(string idAnnexure, string PendingFlg, string Document)
        {
            try
            {
                MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
                //string filename = Path.GetFileName(Document);
                //FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                string filename = "";
                System.IO.FileStream fsSource = null;

                if (Document != null && Document != "")
                {
                    if (Document.Contains(","))
                    {
                        string[] filearray = Document.Split(',');

                        //string fullfile = "";
                        //for (int ii=0; ii< filearray.Length; ii++)
                        //{
                        //     filename = Path.GetFileName(filearray[ii]);
                        //}
                        fsSource = new FileStream(Server.MapPath(filearray[0]), FileMode.Open, FileAccess.Read);
                    }
                    else
                    {
                        filename = Path.GetFileName(Document);
                        fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                    }
                }

                if (objMgmtDocuments.FunUpdateAnnexureReview(idAnnexure, fsSource, filename))
                {
                    TempData["Successdata"] = "Document Reviewed successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AnnexureApproval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                //return RedirectToAction("ListPendingForReview", "Dashboard");
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("MgmtDocumentsList");
            }
        }

        public JsonResult MgmtDocumentsApproveNoty(string idMgmt, int iStatus, string PendingFlg, string Document, string DocName, string DocRef)
        {
            try
            {
                MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
                //string filename = Path.GetFileName(Document);
                //FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                string filename = "";
                System.IO.FileStream fsSource = null;

                if (Document != null && Document != "")
                {
                    if (Document.Contains(","))
                    {
                        string[] filearray = Document.Split(',');

                        //string fullfile = "";
                        //for (int ii=0; ii< filearray.Length; ii++)
                        //{
                        //     filename = Path.GetFileName(filearray[ii]);
                        //}
                        fsSource = new FileStream(Server.MapPath(filearray[0]), FileMode.Open, FileAccess.Read);
                    }
                    else
                    {
                        filename = Path.GetFileName(Document);
                        fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                    }
                }

                if (objMgmtDocuments.FunMgmtDocumentApproveOrReject(idMgmt, iStatus, fsSource, filename, DocName, DocRef))
                {
                    return Json("Success" + iStatus);
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
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsApprove: " + ex.ToString());
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

        public JsonResult AnnexureApprovalNoty(string idAnnexure, string PendingFlg, string Document)
        {
            try
            {
                MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
                //string filename = Path.GetFileName(Document);
                //FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);

                string filename = "";
                System.IO.FileStream fsSource = null;

                if (Document != null && Document != "")
                {
                    if (Document.Contains(","))
                    {
                        string[] filearray = Document.Split(',');

                        //string fullfile = "";
                        //for (int ii=0; ii< filearray.Length; ii++)
                        //{
                        //     filename = Path.GetFileName(filearray[ii]);
                        //}
                        fsSource = new FileStream(Server.MapPath(filearray[0]), FileMode.Open, FileAccess.Read);
                    }
                    else
                    {
                        filename = Path.GetFileName(Document);
                        fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                    }
                }

                if (objMgmtDocuments.FunUpdateAnnexureReview(idAnnexure, fsSource, filename))
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
                objGlobaldata.AddFunctionalLog("Exception in AnnexureApproval: " + ex.ToString());
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                //return RedirectToAction("ListPendingForReview", "Dashboard");
                return Json("Success");
            }
            else
            {
                return Json("Failed");
            }
        }

        public JsonResult MgmtDocumentsReviewedNoty(string idMgmt, string PendingFlg, string Document, int iStatus, string DocName, string DocRef)
        {
            try
            {
                MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
                string filename = "";
                System.IO.FileStream fsSource = null;

                if (Document != null && Document != "")
                {
                    if (Document.Contains(","))
                    {
                        string[] filearray = Document.Split(',');

                        //string fullfile = "";
                        //for (int ii=0; ii< filearray.Length; ii++)
                        //{
                        //     filename = Path.GetFileName(filearray[ii]);
                        //}
                        fsSource = new FileStream(Server.MapPath(filearray[0]), FileMode.Open, FileAccess.Read);
                    }
                    else
                    {
                        filename = Path.GetFileName(Document);
                        fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                    }
                }

                if (objMgmtDocuments.FunMgmtDocumentsReviewApproveOrReject(idMgmt, iStatus, fsSource, filename, DocName, DocRef))
                {
                    return Json("Success" + iStatus);
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsApprove: " + ex.ToString());
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

        public JsonResult FunGetDocTypebyDocLevels(string DocLevels)
        {
            try
            {
                MultiSelectList DocTypeList = objGlobaldata.GetDocTypeListbyDocLevels(DocLevels);
                return Json(DocTypeList);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetDocTypebyDocLevels: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }

        public JsonResult FunGetExistDCRNo(string dcr_no)
        {
            try
            {
                DocumentCreateRequestModels objmodel = new DocumentCreateRequestModels();
                string stmt = "Select id_doc_request,dcr_no,upload from t_document_create_request where id_doc_request = '" + dcr_no + "' and active=1";
                DataSet dsDCR = objGlobaldata.Getdetails(stmt);

                if (dsDCR.Tables.Count > 0 && dsDCR.Tables[0].Rows.Count > 0)
                {
                    objmodel.id_doc_request = dsDCR.Tables[0].Rows[0]["id_doc_request"].ToString();
                    objmodel.dcr_no = dsDCR.Tables[0].Rows[0]["dcr_no"].ToString();
                    objmodel.upload = dsDCR.Tables[0].Rows[0]["upload"].ToString();
                }
                return Json(objmodel);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetExistDCRNo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }

        public JsonResult FunGetDCRList(string branch, string location)
        {
            try
            {
                MultiSelectList DCRList = objGlobaldata.GetDCRDocumentList(branch, location);
                return Json(DCRList);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetDCRList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }
    }
}