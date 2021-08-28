using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.IO;
using System.Data;
using PagedList;
using PagedList.Mvc;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class MgmtExternalDocumentController : Controller
    {

        clsGlobal objGlobaldata = new clsGlobal();

        public MgmtExternalDocumentController()
        {
            ViewBag.Menutype = "Documents";
            ViewBag.SubMenutype = "External";
        }

        //
        // GET: /MgmtExternalDocument/
          
        public ActionResult Index()
        {
            return View();
        }

        // GET: /MgmtExternalDocument/AddMgmtExternalDocument
        
        [AllowAnonymous]
        public ActionResult AddMgmtExternalDocument()
        {
            return InitilizeAddMgmtExternalDocument();
        }          
         
        private ActionResult InitilizeAddMgmtExternalDocument()
        {
            MgmtExternalDocumentModels MgmtModel = new MgmtExternalDocumentModels();
            try
            {
                MgmtModel.branch = objGlobaldata.GetCurrentUserSession().division;
                MgmtModel.DeptId = objGlobaldata.GetCurrentUserSession().DeptID;
                MgmtModel.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.SubMenutype = "External";
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(MgmtModel.branch, MgmtModel.DeptId, MgmtModel.Location);
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(MgmtModel.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(MgmtModel.branch);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddMgmtExternalDocument: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(MgmtModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMgmtExternalDocument(MgmtExternalDocumentModels objMgmtExternalDocumentModels, FormCollection form, HttpPostedFileBase SoftCopy_Path)
        {
            try
            {
                objMgmtExternalDocumentModels.DeptId = form["DeptId"];
                objMgmtExternalDocumentModels.CustodyOf = form["CustodyOfVal"];
                objMgmtExternalDocumentModels.Person_Responsible = form["Person_Responsible"];
                objMgmtExternalDocumentModels.NotificationPeriod = form["NotificationPeriod"];
                objMgmtExternalDocumentModels.NotificationValue = form["NotificationValue"];
                objMgmtExternalDocumentModels.branch = form["branch"];
                objMgmtExternalDocumentModels.Location = form["Location"];
                if (objMgmtExternalDocumentModels.branch == "" || objMgmtExternalDocumentModels.branch == null)
                {
                    objMgmtExternalDocumentModels.branch = objGlobaldata.GetCurrentUserSession().division;
                }
                int Notificationval = 1;

                if (objMgmtExternalDocumentModels.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objMgmtExternalDocumentModels.NotificationValue = "0";
                }
                else if (objMgmtExternalDocumentModels.NotificationPeriod == "Weeks" && int.TryParse(objMgmtExternalDocumentModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objMgmtExternalDocumentModels.NotificationPeriod == "Months" && int.TryParse(objMgmtExternalDocumentModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objMgmtExternalDocumentModels.NotificationDays = Notificationval;
                DateTime dateValue;

                if (DateTime.TryParse(form["DocDate"], out dateValue) == true)
                {
                    objMgmtExternalDocumentModels.DocDate = dateValue;
                }
                if (DateTime.TryParse(form["Eve_Date"], out dateValue) == true)
                {
                    objMgmtExternalDocumentModels.Eve_Date = dateValue;
                }

                if (SoftCopy_Path != null && SoftCopy_Path.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/IMSDocs"), Path.GetFileName(SoftCopy_Path.FileName));
                        string sFilename = objMgmtExternalDocumentModels.DocName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        SoftCopy_Path.SaveAs(sFilepath + "/"  + sFilename);
                        objMgmtExternalDocumentModels.SoftCopy_Path = "~/DataUpload/MgmtDocs/IMSDocs/" + sFilename;
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


                if (objMgmtExternalDocumentModels.FunAddMgmtExternalDocument(objMgmtExternalDocumentModels))
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
                objGlobaldata.AddFunctionalLog("Exception in AddMgmtExternalDocument: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MgmtExternalDocumentList");
        }


        // GET: /MgmtExternalDocument/MgmtExternalDocumentList
          
        [AllowAnonymous]
        public ActionResult MgmtExternalDocumentList(string SearchText, string DeptId, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "External";

            MgmtExternalDocumentModelsList objMgmtExternalDocumentModelsList = new MgmtExternalDocumentModelsList();
            objMgmtExternalDocumentModelsList.lstMgmtExternalDocument = new List<MgmtExternalDocumentModels>();
            ViewBag.DeptId = objGlobaldata.GetDepartmentWithIdListbox();

            try
            {
                string sSqlstmt = "select Mgmt_doc_External_Id, DocRef, DocName, Doc_Origin, IssueNo, RevNo, DocDate, DeptId, CustodyOf, SoftCopy_Path, MethodOf_Updating,"
                        + "Updated_Thru, Person_Responsible, Remarks, UploadedBy, UploadedDate,Eve_Date,Location,branch from t_mgmt_doc_external where Active=1";
                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = " (DocName ='" + SearchText + "' or DocName like %'" + SearchText + "%')";
                }

                if (sSearchtext != "")
                {
                    sSqlstmt = sSqlstmt + " and";
                }

                if (DeptId != null && DeptId != "Select")
                {
                    ViewBag.DeptIdVal = DeptId;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (DeptId ='" + DeptId + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and (DeptId ='" + DeptId + "')";
                    }
                }

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

                sSqlstmt = sSqlstmt + sSearchtext + " order by DocName asc";

                DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                {
                                         
                    
                    for (int i = 0; i < dsMgmtDocumentsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            MgmtExternalDocumentModels objMgmtDocumentsModels = new MgmtExternalDocumentModels
                            {
                                Mgmt_doc_External_Id = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["Mgmt_doc_External_Id"].ToString()),
                                DeptId = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["DeptId"].ToString()),
                                DocRef = (dsMgmtDocumentsList.Tables[0].Rows[i]["DocRef"].ToString()),
                                DocName = dsMgmtDocumentsList.Tables[0].Rows[i]["DocName"].ToString(),
                                Doc_Origin = (dsMgmtDocumentsList.Tables[0].Rows[i]["Doc_Origin"].ToString()),
                                IssueNo = dsMgmtDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString(),
                                RevNo = dsMgmtDocumentsList.Tables[0].Rows[i]["RevNo"].ToString(),
                                DocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["DocDate"].ToString()),
                                CustodyOf = objGlobaldata.GetEmpHrNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["CustodyOf"].ToString()),
                                SoftCopy_Path = (dsMgmtDocumentsList.Tables[0].Rows[i]["SoftCopy_Path"].ToString()),
                                MethodOf_Updating = (dsMgmtDocumentsList.Tables[0].Rows[i]["MethodOf_Updating"].ToString()),
                                Updated_Thru = (dsMgmtDocumentsList.Tables[0].Rows[i]["Updated_Thru"].ToString()),
                                Person_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Person_Responsible"].ToString()),
                                Remarks = dsMgmtDocumentsList.Tables[0].Rows[i]["Remarks"].ToString(),
                                UploadedDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["UploadedDate"].ToString()),
                                UploadedBy = objGlobaldata.GetEmpHrNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["UploadedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsMgmtDocumentsList.Tables[0].Rows[i]["Location"].ToString())
                            };

                            DateTime dtDocDate;
                            if (dsMgmtDocumentsList.Tables[0].Rows[i]["Eve_Date"].ToString() != ""
                               && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[i]["Eve_Date"].ToString(), out dtDocDate))
                            {
                                objMgmtDocumentsModels.Eve_Date = dtDocDate;
                            }

                            objMgmtExternalDocumentModelsList.lstMgmtExternalDocument.Add(objMgmtDocumentsModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in MgmtExternalDocumentList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtExternalDocumentList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objMgmtExternalDocumentModelsList.lstMgmtExternalDocument.ToList());
        }

        [AllowAnonymous]
        public JsonResult MgmtExternalDocumentListSearch(string SearchText, string DeptId, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "External";

            MgmtExternalDocumentModelsList objMgmtExternalDocumentModelsList = new MgmtExternalDocumentModelsList();
            objMgmtExternalDocumentModelsList.lstMgmtExternalDocument = new List<MgmtExternalDocumentModels>();
            ViewBag.DeptId = objGlobaldata.GetDepartmentWithIdListbox();

            try
            {
                string sSqlstmt = "select Mgmt_doc_External_Id, DocRef, DocName, Doc_Origin, IssueNo, RevNo, DocDate, DeptId, CustodyOf, SoftCopy_Path, MethodOf_Updating,"
                        + "Updated_Thru, Person_Responsible, Remarks, UploadedBy, UploadedDate,Eve_Date,branch,Location from t_mgmt_doc_external where Active=1";
                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = " (DocName ='" + SearchText + "' or DocName like %'" + SearchText + "%')";
                }

                if (sSearchtext != "")
                {
                    sSqlstmt = sSqlstmt + " and";
                }

                if (DeptId != null && DeptId != "Select")
                {
                    ViewBag.DeptIdVal = DeptId;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (DeptId ='" + DeptId + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and (DeptId ='" + DeptId + "')";
                    }
                }

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

                sSqlstmt = sSqlstmt + sSearchtext + " order by DocName asc";

                DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                {

                   
                    for (int i = 0; i < dsMgmtDocumentsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            MgmtExternalDocumentModels objMgmtDocumentsModels = new MgmtExternalDocumentModels
                            {
                                Mgmt_doc_External_Id = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["Mgmt_doc_External_Id"].ToString()),
                                DeptId = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["DeptId"].ToString()),
                                DocRef = (dsMgmtDocumentsList.Tables[0].Rows[i]["DocRef"].ToString()),
                                DocName = dsMgmtDocumentsList.Tables[0].Rows[i]["DocName"].ToString(),
                                Doc_Origin = (dsMgmtDocumentsList.Tables[0].Rows[i]["Doc_Origin"].ToString()),
                                IssueNo = dsMgmtDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString(),
                                RevNo = dsMgmtDocumentsList.Tables[0].Rows[i]["RevNo"].ToString(),
                                DocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["DocDate"].ToString()),
                                CustodyOf = objGlobaldata.GetEmpHrNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["CustodyOf"].ToString()),
                                SoftCopy_Path = (dsMgmtDocumentsList.Tables[0].Rows[i]["SoftCopy_Path"].ToString()),
                                MethodOf_Updating = (dsMgmtDocumentsList.Tables[0].Rows[i]["MethodOf_Updating"].ToString()),
                                Updated_Thru = (dsMgmtDocumentsList.Tables[0].Rows[i]["Updated_Thru"].ToString()),
                                Person_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Person_Responsible"].ToString()),
                                Remarks = dsMgmtDocumentsList.Tables[0].Rows[i]["Remarks"].ToString(),
                                UploadedDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["UploadedDate"].ToString()),
                                UploadedBy = objGlobaldata.GetEmpHrNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["UploadedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsMgmtDocumentsList.Tables[0].Rows[i]["Location"].ToString())

                            };

                            DateTime dtDocDate;
                            if (dsMgmtDocumentsList.Tables[0].Rows[i]["Eve_Date"].ToString() != ""
                               && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[i]["Eve_Date"].ToString(), out dtDocDate))
                            {
                                objMgmtDocumentsModels.Eve_Date = dtDocDate;
                            }

                            objMgmtExternalDocumentModelsList.lstMgmtExternalDocument.Add(objMgmtDocumentsModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in MgmtExternalDocumentListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtExternalDocumentListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }


        public ActionResult MgmtExternalDocumentHistory(int? page)
        {
            ViewBag.SubMenutype = "External";

            MgmtExternalDocumentModelsList objMgmtExternalDocumentModelsList = new MgmtExternalDocumentModelsList();
            objMgmtExternalDocumentModelsList.lstMgmtExternalDocument = new List<MgmtExternalDocumentModels>();
            ViewBag.DeptId = objGlobaldata.GetDepartmentWithIdListbox();

            try
            {
                if (Request.QueryString["Mgmt_doc_External_Id"] != null && Request.QueryString["Mgmt_doc_External_Id"] != "")
                {
                    string sMgmt_doc_External_Id = Request.QueryString["Mgmt_doc_External_Id"];
                    string sSqlstmt = "select Mgmt_doc_External_Id, DocRef, DocName, Doc_Origin, IssueNo, RevNo, DocDate, DeptId, CustodyOf, SoftCopy_Path, MethodOf_Updating,"
                        + "Updated_Thru, Person_Responsible, Remarks, UploadedBy, UploadedDate,Eve_Date from t_mgmt_doc_external_trans where Mgmt_doc_External_Id=" + sMgmt_doc_External_Id;


                    //sSqlstmt = sSqlstmt + " order by DocName asc";

                    DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                    {

                        
                        for (int i = 0; i < dsMgmtDocumentsList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                MgmtExternalDocumentModels objMgmtDocumentsModels = new MgmtExternalDocumentModels
                                {
                                    Mgmt_doc_External_Id = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[i]["Mgmt_doc_External_Id"].ToString()),
                                    DeptId = objGlobaldata.GetDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["DeptId"].ToString()),
                                    DocRef = (dsMgmtDocumentsList.Tables[0].Rows[i]["DocRef"].ToString()),
                                    DocName = dsMgmtDocumentsList.Tables[0].Rows[i]["DocName"].ToString(),
                                    Doc_Origin = (dsMgmtDocumentsList.Tables[0].Rows[i]["Doc_Origin"].ToString()),
                                    IssueNo = dsMgmtDocumentsList.Tables[0].Rows[i]["IssueNo"].ToString(),
                                    RevNo = dsMgmtDocumentsList.Tables[0].Rows[i]["RevNo"].ToString(),
                                    DocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["DocDate"].ToString()),
                                    CustodyOf = objGlobaldata.GetEmpHrNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["CustodyOf"].ToString()),
                                    SoftCopy_Path = (dsMgmtDocumentsList.Tables[0].Rows[i]["SoftCopy_Path"].ToString()),
                                    MethodOf_Updating = (dsMgmtDocumentsList.Tables[0].Rows[i]["MethodOf_Updating"].ToString()),
                                    Updated_Thru = (dsMgmtDocumentsList.Tables[0].Rows[i]["Updated_Thru"].ToString()),
                                    Person_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["Person_Responsible"].ToString()),
                                    Remarks = dsMgmtDocumentsList.Tables[0].Rows[i]["Remarks"].ToString(),
                                    UploadedDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[i]["UploadedDate"].ToString()),
                                    UploadedBy = objGlobaldata.GetEmpHrNameById(dsMgmtDocumentsList.Tables[0].Rows[i]["UploadedBy"].ToString())
                                };

                                DateTime dtDocDate;
                                if (dsMgmtDocumentsList.Tables[0].Rows[i]["Eve_Date"].ToString() != ""
                                   && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[i]["Eve_Date"].ToString(), out dtDocDate))
                                {
                                    objMgmtDocumentsModels.Eve_Date = dtDocDate;
                                }

                                objMgmtExternalDocumentModelsList.lstMgmtExternalDocument.Add(objMgmtDocumentsModels);
                            }

                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in MgmtExternalDocumentHistory: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtExternalDocumentHistory: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objMgmtExternalDocumentModelsList.lstMgmtExternalDocument.ToList());
        }


        // GET: /MgmtExternalDocument/MgmtExternalDocumentDetails

        [AllowAnonymous]
        public ActionResult MgmtExternalDocumentDetails()
        {
            ViewBag.SubMenutype = "External";
            MgmtExternalDocumentModels objMgmtDocumentsModels = new MgmtExternalDocumentModels();

            try
            {
                if (Request.QueryString["Mgmt_doc_External_Id"] != null && Request.QueryString["Mgmt_doc_External_Id"] != "")
                {
                    string sMgmt_doc_External_Id = Request.QueryString["Mgmt_doc_External_Id"];
                    string sSqlstmt = "select Mgmt_doc_External_Id, DocRef, DocName, Doc_Origin, IssueNo, RevNo, DocDate, DeptId, CustodyOf, SoftCopy_Path, MethodOf_Updating,"
                            + "Updated_Thru, Person_Responsible, Remarks, UploadedBy, UploadedDate, Eve_Date,NotificationDays,NotificationPeriod,NotificationValue,branch,Location from t_mgmt_doc_external where Mgmt_doc_External_Id='"
                            + sMgmt_doc_External_Id + "'";

                    DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                    {
                        objMgmtDocumentsModels = new MgmtExternalDocumentModels
                        {
                            Mgmt_doc_External_Id = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["Mgmt_doc_External_Id"].ToString()),
                            DeptId = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["DeptId"].ToString()),
                            DocRef = (dsMgmtDocumentsList.Tables[0].Rows[0]["DocRef"].ToString()),
                            DocName = dsMgmtDocumentsList.Tables[0].Rows[0]["DocName"].ToString(),
                            Doc_Origin = (dsMgmtDocumentsList.Tables[0].Rows[0]["Doc_Origin"].ToString()),
                            IssueNo = dsMgmtDocumentsList.Tables[0].Rows[0]["IssueNo"].ToString(),
                            RevNo = dsMgmtDocumentsList.Tables[0].Rows[0]["RevNo"].ToString(),
                            DocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString()),
                            CustodyOf = objGlobaldata.GetEmpHrNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["CustodyOf"].ToString()),
                            SoftCopy_Path = (dsMgmtDocumentsList.Tables[0].Rows[0]["SoftCopy_Path"].ToString()),
                            MethodOf_Updating = (dsMgmtDocumentsList.Tables[0].Rows[0]["MethodOf_Updating"].ToString()),
                            Updated_Thru = (dsMgmtDocumentsList.Tables[0].Rows[0]["Updated_Thru"].ToString()),
                            Person_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["Person_Responsible"].ToString()),
                            Remarks = dsMgmtDocumentsList.Tables[0].Rows[0]["Remarks"].ToString(),
                            UploadedDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[0]["UploadedDate"].ToString()),
                            UploadedBy = objGlobaldata.GetEmpHrNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["UploadedBy"].ToString()),
                            NotificationPeriod = dsMgmtDocumentsList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsMgmtDocumentsList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsMgmtDocumentsList.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsMgmtDocumentsList.Tables[0].Rows[0]["Eve_Date"].ToString() != ""
                           && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[0]["Eve_Date"].ToString(), out dtDocDate))
                        {
                            objMgmtDocumentsModels.Eve_Date = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("MgmtExternalDocumentList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Document Id cannot be Null or empty";
                    return RedirectToAction("MgmtExternalDocumentList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtExternalDocumentDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objMgmtDocumentsModels);
        }
          
        [AllowAnonymous]
        public ActionResult MgmtExternalDocumentInfo(int id)
        {
            ViewBag.SubMenutype = "External";
            MgmtExternalDocumentModels objMgmtDocumentsModels = new MgmtExternalDocumentModels();

            try
            {
                    string sSqlstmt = "select Mgmt_doc_External_Id, DocRef, DocName, Doc_Origin, IssueNo, RevNo, DocDate, DeptId, CustodyOf, SoftCopy_Path, MethodOf_Updating,"
                            + "Updated_Thru, Person_Responsible, Remarks, UploadedBy, UploadedDate, Eve_Date,NotificationDays,NotificationPeriod,NotificationValue,branch,Location from t_mgmt_doc_external where Mgmt_doc_External_Id='"
                            + id + "'";

                    DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                    {
                        objMgmtDocumentsModels = new MgmtExternalDocumentModels
                        {
                            Mgmt_doc_External_Id = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["Mgmt_doc_External_Id"].ToString()),
                            DeptId = objGlobaldata.GetMultiDeptNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["DeptId"].ToString()),
                            DocRef = (dsMgmtDocumentsList.Tables[0].Rows[0]["DocRef"].ToString()),
                            DocName = dsMgmtDocumentsList.Tables[0].Rows[0]["DocName"].ToString(),
                            Doc_Origin = (dsMgmtDocumentsList.Tables[0].Rows[0]["Doc_Origin"].ToString()),
                            IssueNo = dsMgmtDocumentsList.Tables[0].Rows[0]["IssueNo"].ToString(),
                            RevNo = dsMgmtDocumentsList.Tables[0].Rows[0]["RevNo"].ToString(),
                            DocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString()),
                            CustodyOf = objGlobaldata.GetEmpHrNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["CustodyOf"].ToString()),
                            SoftCopy_Path = (dsMgmtDocumentsList.Tables[0].Rows[0]["SoftCopy_Path"].ToString()),
                            MethodOf_Updating = (dsMgmtDocumentsList.Tables[0].Rows[0]["MethodOf_Updating"].ToString()),
                            Updated_Thru = (dsMgmtDocumentsList.Tables[0].Rows[0]["Updated_Thru"].ToString()),
                            Person_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["Person_Responsible"].ToString()),
                            Remarks = dsMgmtDocumentsList.Tables[0].Rows[0]["Remarks"].ToString(),
                            UploadedDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[0]["UploadedDate"].ToString()),
                            UploadedBy = objGlobaldata.GetEmpHrNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["UploadedBy"].ToString()),
                            NotificationPeriod = dsMgmtDocumentsList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsMgmtDocumentsList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsMgmtDocumentsList.Tables[0].Rows[0]["Location"].ToString()),
                        };

                      DateTime dtDocDate;
                      if (dsMgmtDocumentsList.Tables[0].Rows[0]["Eve_Date"].ToString() != ""
                           && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[0]["Eve_Date"].ToString(), out dtDocDate))
                      {
                        objMgmtDocumentsModels.Eve_Date = dtDocDate;
                      }

                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("MgmtExternalDocumentList");
                    }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtExternalDocumentDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objMgmtDocumentsModels);
        }
        // GET: /MgmtExternalDocument/MgmtExternalDocumentEdit
          
        [AllowAnonymous]
        public ActionResult MgmtExternalDocumentEdit()
        {
            ViewBag.SubMenutype = "External";
            MgmtExternalDocumentModels objMgmtDocumentsModels = new MgmtExternalDocumentModels();
            try
            {
                if (Request.QueryString["Mgmt_doc_External_Id"] != null && Request.QueryString["Mgmt_doc_External_Id"] != "")
                {
                    string sMgmt_doc_External_Id = Request.QueryString["Mgmt_doc_External_Id"];
                    string sSqlstmt = "select Mgmt_doc_External_Id, DocRef, DocName, Doc_Origin, IssueNo, RevNo, DocDate, DeptId, CustodyOf, SoftCopy_Path, MethodOf_Updating,"
                            + "Updated_Thru, Person_Responsible, Remarks, UploadedBy, UploadedDate, Eve_Date,NotificationDays,NotificationPeriod,NotificationValue,branch,Location from t_mgmt_doc_external where Mgmt_doc_External_Id='"
                            + sMgmt_doc_External_Id + "'";

                    DataSet dsMgmtDocumentsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsMgmtDocumentsList.Tables.Count > 0 && dsMgmtDocumentsList.Tables[0].Rows.Count > 0)
                    {

                        objMgmtDocumentsModels = new MgmtExternalDocumentModels
                        {
                            Mgmt_doc_External_Id = Convert.ToInt16(dsMgmtDocumentsList.Tables[0].Rows[0]["Mgmt_doc_External_Id"].ToString()),
                            DeptId = (dsMgmtDocumentsList.Tables[0].Rows[0]["DeptId"].ToString()),
                            DocRef = (dsMgmtDocumentsList.Tables[0].Rows[0]["DocRef"].ToString()),
                            DocName = dsMgmtDocumentsList.Tables[0].Rows[0]["DocName"].ToString(),
                            Doc_Origin = (dsMgmtDocumentsList.Tables[0].Rows[0]["Doc_Origin"].ToString()),
                            IssueNo = dsMgmtDocumentsList.Tables[0].Rows[0]["IssueNo"].ToString(),
                            RevNo = dsMgmtDocumentsList.Tables[0].Rows[0]["RevNo"].ToString(),
                            DocDate = Convert.ToDateTime(dsMgmtDocumentsList.Tables[0].Rows[0]["DocDate"].ToString()),
                            CustodyOf = (dsMgmtDocumentsList.Tables[0].Rows[0]["CustodyOf"].ToString()),
                            SoftCopy_Path = (dsMgmtDocumentsList.Tables[0].Rows[0]["SoftCopy_Path"].ToString()),
                            MethodOf_Updating = (dsMgmtDocumentsList.Tables[0].Rows[0]["MethodOf_Updating"].ToString()),
                            Updated_Thru = (dsMgmtDocumentsList.Tables[0].Rows[0]["Updated_Thru"].ToString()),
                            Person_Responsible = dsMgmtDocumentsList.Tables[0].Rows[0]["Person_Responsible"].ToString(),
                            Remarks = dsMgmtDocumentsList.Tables[0].Rows[0]["Remarks"].ToString(),
                            NotificationPeriod = dsMgmtDocumentsList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsMgmtDocumentsList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            branch = dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString(),
                            Location = (dsMgmtDocumentsList.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        ViewBag.CustodyOfVal = objGlobaldata.GetEmpHrNameById(objMgmtDocumentsModels.CustodyOf);
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsMgmtDocumentsList.Tables[0].Rows[0]["branch"].ToString(), dsMgmtDocumentsList.Tables[0].Rows[0]["DeptId"].ToString(), dsMgmtDocumentsList.Tables[0].Rows[0]["Location"].ToString());
                        ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        
                        DateTime dtDocDate;
                        if (dsMgmtDocumentsList.Tables[0].Rows[0]["Eve_Date"].ToString() != ""
                             && DateTime.TryParse(dsMgmtDocumentsList.Tables[0].Rows[0]["Eve_Date"].ToString(), out dtDocDate))
                        {
                            objMgmtDocumentsModels.Eve_Date = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("MgmtExternalDocumentList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Document Id cannot be Null or empty";
                    return RedirectToAction("MgmtExternalDocumentList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtExternalDocumentEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
           
            return View(objMgmtDocumentsModels);
        }


        // POST: /MgmtExternalDocument/MgmtExternalDocumentEdit
          
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MgmtExternalDocumentEdit(MgmtExternalDocumentModels objMgmtExternalDocumentModels, FormCollection form, HttpPostedFileBase SoftCopy_Path)
        {
            try
            {
                objMgmtExternalDocumentModels.DeptId = form["DeptId"];
                objMgmtExternalDocumentModels.CustodyOf = form["CustodyOfVal"];
                objMgmtExternalDocumentModels.Person_Responsible = form["Person_Responsible"];
                objMgmtExternalDocumentModels.branch = form["branch"];
                objMgmtExternalDocumentModels.Location = form["Location"];
                if (objMgmtExternalDocumentModels.branch == "" || objMgmtExternalDocumentModels.branch == null)
                {
                    objMgmtExternalDocumentModels.branch = objGlobaldata.GetCurrentUserSession().division;
                }

                if (form["Mgmt_doc_External_Id"] != null)
                {
                    objMgmtExternalDocumentModels.Mgmt_doc_External_Id = Convert.ToInt16(form["Mgmt_doc_External_Id"]);
                }

                DateTime dateValue;

                if (DateTime.TryParse(form["DocDate"], out dateValue) == true)
                {
                    objMgmtExternalDocumentModels.DocDate = dateValue;
                }

                if (DateTime.TryParse(form["Eve_Date"], out dateValue) == true)
                {
                    objMgmtExternalDocumentModels.Eve_Date = dateValue;
                }
                int Notificationval = 1;

                if (objMgmtExternalDocumentModels.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objMgmtExternalDocumentModels.NotificationValue = "0";
                }
                else if (objMgmtExternalDocumentModels.NotificationPeriod == "Weeks" && int.TryParse(objMgmtExternalDocumentModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objMgmtExternalDocumentModels.NotificationPeriod == "Months" && int.TryParse(objMgmtExternalDocumentModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objMgmtExternalDocumentModels.NotificationDays = Notificationval;

                if (SoftCopy_Path != null && SoftCopy_Path.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/IMSDocs"), Path.GetFileName(SoftCopy_Path.FileName));
                        string sFilename = objMgmtExternalDocumentModels.DocName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath) ;
                        string sFilepath = Path.GetDirectoryName(spath);

                        SoftCopy_Path.SaveAs(sFilepath + "/" + sFilename);
                        objMgmtExternalDocumentModels.SoftCopy_Path = "~/DataUpload/MgmtDocs/IMSDocs/" + sFilename;
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


                if (objMgmtExternalDocumentModels.FunUpdateMgmtExternalDocument(objMgmtExternalDocumentModels))
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
                objGlobaldata.AddFunctionalLog("Exception in MgmtExternalDocumentEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MgmtExternalDocumentList");
        }


        [AllowAnonymous]
        public JsonResult MgmtExternalDocDelete(FormCollection form)
        {
            try
            {

                if (form["Mgmt_doc_External_Id"] != null && form["Mgmt_doc_External_Id"] != "")
                {

                    MgmtExternalDocumentModels Doc = new MgmtExternalDocumentModels();
                    string sMgmt_doc_External_Id = form["Mgmt_doc_External_Id"];


                    if (Doc.FunDeleteExternalMgmtDoc(sMgmt_doc_External_Id))
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
                    TempData["alertdata"] = "ExternalDoc Id cannot be Null or empty";
                    return Json("Failed");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MgmtExternalDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public ActionResult FunGetDeptHeadList(string DeptId)
        {
            MultiSelectList lstEmp = objGlobaldata.GetDeptHeadList(DeptId);
            return Json(lstEmp);
        }

        [HttpPost]
        public JsonResult doesDocNameExist(string DocName)
        {
            MgmtExternalDocumentModels objMgmtExtDocuments = new MgmtExternalDocumentModels();
            var user = objMgmtExtDocuments.CheckForDocNameExists(DocName);

            return Json(user);
        }

        [HttpPost]
        public JsonResult doesDocRefExist(string DocRef)
        {
            MgmtExternalDocumentModels objMgmtExtDocuments = new MgmtExternalDocumentModels();
            var user = objMgmtExtDocuments.CheckForDocRefExists(DocRef);

            return Json(user);
        }

    }
}
