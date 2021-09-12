using ISOStd.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    public class DocumentReviewController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public DocumentReviewController()
        {
            ViewBag.Menutype = "Documents";
            ViewBag.SubMenutype = "DocumentReview";
        }

        [AllowAnonymous]
        public ActionResult AddDocumentReview()
        {
            DocumentReviewModels objReview = new DocumentReviewModels();
            try
            {
                objReview.division = objGlobaldata.GetCurrentUserSession().division;
               
                //ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DocLevel = objGlobaldata.GetDocLevelsList();
                ViewBag.Frequency = objReview.GetDocReviewFrequencyList();
                ViewBag.Criteria = objReview.GetDocReviewCriteriaList();
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDocumentReview: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objReview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDocumentReview(DocumentReviewModels objReview, FormCollection form)
        {
            try
            {              
              
                objReview.approvedby = form["approvedby"];
              
                DateTime dateValue;
                if (DateTime.TryParse(form["review_date"], out dateValue) == true)
                {
                    objReview.review_date = dateValue;
                }                               

                if (objReview.FunAddDocReview(objReview))
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
                objGlobaldata.AddFunctionalLog("Exception in AddDocumentReview: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("DocumentReviewList");
        }


        [AllowAnonymous]
        public JsonResult DocReviewDelete(FormCollection form)
        {
            try
            {
                if (form["id_doc_review"] != null && form["id_doc_review"] != "")
                {
                    DocumentReviewModels Doc = new DocumentReviewModels();
                    string sid_doc_review = form["id_doc_review"];

                    if (Doc.FunDeleteDocReview(sid_doc_review))
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
                objGlobaldata.AddFunctionalLog("Exception in DocReviewDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult DocumentReviewList(FormCollection form, string branch_name)
        {
            DocumentReviewModelsList objRList = new DocumentReviewModelsList();
            objRList.ReviewList = new List<DocumentReviewModels>();

            DocumentReviewModels objReview = new DocumentReviewModels();
            try
            {
                string sSqlstmt = "select id_doc_review,review_date, doc_level, doc_type, frequency, " +
                    "criteria, approvedby, division from t_document_review where active=1";

                string sSearchtext = "";

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', division)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by doc_type";

                DataSet dsReviewList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsReviewList.Tables.Count > 0 && dsReviewList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReviewList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objReview = new DocumentReviewModels
                            {
                                id_doc_review = dsReviewList.Tables[0].Rows[i]["id_doc_review"].ToString(),
                                doc_level = objGlobaldata.GetDocumentLevelbyId(dsReviewList.Tables[0].Rows[i]["doc_level"].ToString()),
                                doc_type = objGlobaldata.GetDocumentTypebyId(dsReviewList.Tables[0].Rows[i]["doc_type"].ToString()),
                                frequency = objReview.GetDocReviewFrequencyById(dsReviewList.Tables[0].Rows[i]["frequency"].ToString()),
                                criteria = objReview.GetDocReviewCriteriaById(dsReviewList.Tables[0].Rows[i]["criteria"].ToString()),
                                approvedby = objGlobaldata.GetMultiHrEmpNameById(dsReviewList.Tables[0].Rows[i]["approvedby"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsReviewList.Tables[0].Rows[i]["division"].ToString()),                               
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsReviewList.Tables[0].Rows[i]["review_date"].ToString(), out dateValue))
                            {
                                objReview.review_date = dateValue;
                            }                           
                            objRList.ReviewList.Add(objReview);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DocumentReviewList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocumentReviewList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRList.ReviewList.ToList());
        }

        [AllowAnonymous]
        public ActionResult DocumentReviewEdit()
        {
           DocumentReviewModels objReview = new DocumentReviewModels();
            try
            {
                string sid_doc_review = Request.QueryString["id_doc_review"];
                if (sid_doc_review != null && sid_doc_review != "")
                {
                    string sSqlstmt = "select id_doc_review,review_date, doc_level, doc_type, frequency, " +
                        "criteria, approvedby, division from t_document_review where id_doc_review='"+ sid_doc_review + "'";
                                      
                    DataSet dsReviewList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsReviewList.Tables.Count > 0 && dsReviewList.Tables[0].Rows.Count > 0)
                    {
                        try
                            {
                                objReview = new DocumentReviewModels
                                {
                                    id_doc_review = dsReviewList.Tables[0].Rows[0]["id_doc_review"].ToString(),
                                    doc_level = objGlobaldata.GetDocumentLevelbyId(dsReviewList.Tables[0].Rows[0]["doc_level"].ToString()),
                                    doc_type = objGlobaldata.GetDocumentTypebyId(dsReviewList.Tables[0].Rows[0]["doc_type"].ToString()),
                                    frequency = /*objReview.GetDocReviewFrequencyById*/(dsReviewList.Tables[0].Rows[0]["frequency"].ToString()),
                                    criteria = /*objReview.GetDocReviewCriteriaById*/(dsReviewList.Tables[0].Rows[0]["criteria"].ToString()),
                                    approvedby = /*objGlobaldata.GetMultiHrEmpNameById*/(dsReviewList.Tables[0].Rows[0]["approvedby"].ToString()),
                                    division = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsReviewList.Tables[0].Rows[0]["division"].ToString()),
                                };

                                DateTime dateValue;
                                if (DateTime.TryParse(dsReviewList.Tables[0].Rows[0]["review_date"].ToString(), out dateValue))
                                {
                                    objReview.review_date = dateValue;
                                }
                            ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                            ViewBag.EmpLists = objGlobaldata.GetHrEmpListByDivision(dsReviewList.Tables[0].Rows[0]["division"].ToString());
                            ViewBag.Frequency = objReview.GetDocReviewFrequencyList();
                            ViewBag.Criteria = objReview.GetDocReviewCriteriaList();

                        }
                        catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in DocumentReviewEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                    }
                    return View(objReview);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocumentReviewEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DocumentReviewList");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DocumentReviewEdit(DocumentReviewModels objReview, FormCollection form)
        {
            try
            {
                objReview.approvedby = form["approvedby"];

                DateTime dateValue;
                if (DateTime.TryParse(form["review_date"], out dateValue) == true)
                {
                    objReview.review_date = dateValue;
                }

                if (objReview.FunUpdateDocReview(objReview))
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
                objGlobaldata.AddFunctionalLog("Exception in DocumentReviewEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DocumentReviewList");
        }

        [AllowAnonymous]
        public ActionResult DocumentReviewDetails(FormCollection form)
        {
            DocumentReviewModels objReview = new DocumentReviewModels();
            try
            {
                string sid_doc_review = Request.QueryString["id_doc_review"];
                if (sid_doc_review != null && sid_doc_review != "")
                {
                    string sSqlstmt = "select id_doc_review,review_date, doc_level, doc_type, frequency, " +
                        "criteria, approvedby, division, (case when approve_status='1' then 'Approved' else 'Not Approved' end) as approve_status," +
                        " approve_status as approve_statusId from t_document_review where id_doc_review='" + sid_doc_review + "'";

                    DataSet dsReviewList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsReviewList.Tables.Count > 0 && dsReviewList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objReview = new DocumentReviewModels
                            {
                                id_doc_review = dsReviewList.Tables[0].Rows[0]["id_doc_review"].ToString(),
                                doc_level = objGlobaldata.GetDocumentLevelbyId(dsReviewList.Tables[0].Rows[0]["doc_level"].ToString()),
                                doc_type = objGlobaldata.GetDocumentTypebyId(dsReviewList.Tables[0].Rows[0]["doc_type"].ToString()),
                                frequency = objReview.GetDocReviewFrequencyById(dsReviewList.Tables[0].Rows[0]["frequency"].ToString()),
                                criteria = objReview.GetDocReviewCriteriaById(dsReviewList.Tables[0].Rows[0]["criteria"].ToString()),
                                approvedby = objGlobaldata.GetMultiHrEmpNameById(dsReviewList.Tables[0].Rows[0]["approvedby"].ToString()),
                                approvedbyId = (dsReviewList.Tables[0].Rows[0]["approvedby"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsReviewList.Tables[0].Rows[0]["division"].ToString()),
                                approve_status = dsReviewList.Tables[0].Rows[0]["approve_status"].ToString(),
                                approve_statusId = dsReviewList.Tables[0].Rows[0]["approve_statusId"].ToString(),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsReviewList.Tables[0].Rows[0]["review_date"].ToString(), out dateValue))
                            {
                                objReview.review_date = dateValue;
                            }
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DocumentReviewEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    return View(objReview);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocumentReviewDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DocumentReviewList");
        }


        [AllowAnonymous]
        public ActionResult DocumentLogList(FormCollection form, string branch_name)
        {
            ViewBag.SubMenutype = "DocumentReviewLog";
            DocumentReviewModelsList objRList = new DocumentReviewModelsList();
            objRList.ReviewList = new List<DocumentReviewModels>();

            DocumentReviewModels objReview = new DocumentReviewModels();
            try
            {
                string sSqlstmt = "select id_doc_review,review_date, doc_level, doc_type, frequency, " +
                    "criteria, b.approvedby, a.division,DocRef,DocName,IssueNo,RevNo,DocDate from t_document_review a,t_mgmt_documents b" +
                    " where a.active=1 and b.Status=1 and doc_level=DocLevels and doc_type=Doctype";

                string sSearchtext = "";

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', division)";
                }               
                sSqlstmt = sSqlstmt + sSearchtext + " order by doc_type";

                DataSet dsReviewList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsReviewList.Tables.Count > 0 && dsReviewList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReviewList.Tables[0].Rows.Count; i++)
                    {
                         try
                        {
                            objReview = new DocumentReviewModels
                            {
                                id_doc_review =dsReviewList.Tables[0].Rows[i]["id_doc_review"].ToString(),
                                doc_level = objGlobaldata.GetDocumentLevelbyId(dsReviewList.Tables[0].Rows[i]["doc_level"].ToString()),
                                doc_type = objGlobaldata.GetDocumentTypebyId(dsReviewList.Tables[0].Rows[i]["doc_type"].ToString()),
                                frequency = objReview.GetDocReviewFrequencyById(dsReviewList.Tables[0].Rows[i]["frequency"].ToString()),
                                criteria = objReview.GetDocReviewCriteriaById(dsReviewList.Tables[0].Rows[i]["criteria"].ToString()),
                                approvedby = objGlobaldata.GetMultiHrEmpNameById(dsReviewList.Tables[0].Rows[i]["approvedby"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsReviewList.Tables[0].Rows[i]["division"].ToString()),
                                DocRef = dsReviewList.Tables[0].Rows[i]["DocRef"].ToString(),
                                DocName = dsReviewList.Tables[0].Rows[i]["DocName"].ToString(),
                                IssueNo = dsReviewList.Tables[0].Rows[i]["IssueNo"].ToString(),
                                RevNo = dsReviewList.Tables[0].Rows[i]["RevNo"].ToString(),

                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsReviewList.Tables[0].Rows[i]["review_date"].ToString(), out dateValue))
                            {
                                objReview.review_date = dateValue;
                            }

                            if (DateTime.TryParse(dsReviewList.Tables[0].Rows[i]["DocDate"].ToString(), out dateValue))
                            {
                                objReview.DocDate = dateValue;
                            }

                            string sFrequency = objReview.GetDocReviewFrequencyById(dsReviewList.Tables[0].Rows[i]["frequency"].ToString());
                                if (sFrequency == "Semi Annually")
                                {
                                    if (DateTime.TryParse(dsReviewList.Tables[0].Rows[i]["DocDate"].ToString(), out dateValue))
                                    {
                                      objReview.next_review_date = objReview.DocDate.AddMonths(6);                                       
                                    }
                                }
                            else if (sFrequency == "Annually")
                            {
                                if (DateTime.TryParse(dsReviewList.Tables[0].Rows[i]["DocDate"].ToString(), out dateValue))
                                {
                                    objReview.next_review_date = objReview.DocDate.AddMonths(12);
                                }
                            }
                            else if (sFrequency == "Once every two years")
                            {
                                if (DateTime.TryParse(dsReviewList.Tables[0].Rows[i]["DocDate"].ToString(), out dateValue))
                                {
                                    objReview.next_review_date = objReview.DocDate.AddMonths(24);
                                }
                            }

                            objRList.ReviewList.Add(objReview);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DocumentLogList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocumentLogList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRList.ReviewList.ToList());
        }

        [AllowAnonymous]
        public ActionResult DocReviewFreqApproveOrReject(string id_doc_review, int iStatus, string PendingFlg)
        {
            try
            {
                DocumentReviewModels objReview = new DocumentReviewModels();              
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
                if (objReview.FunDocReviewFreApproveOrReject(id_doc_review, iStatus))
                {
                    TempData["Successdata"] = "Document Review Frequency" + sStatus + " successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocReviewFreqApproveOrReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                string path = Request.CurrentExecutionFilePath;
                string[] url = path.Split('/');
                //var controller = url[1];
                if (url[1] == "DocumentReview" || url[2] == "DocumentReview")
                {
                    return RedirectToAction("DocumentReviewList", "DocumentReview");
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


        public JsonResult FunGetExistDoc(string DocLevels,string DocType)
        {
            try
            {
                if (DocLevels != "" && DocType != "" && DocType != null)
                {
                    string Exist = "true";
                    string stmt = "Select id_doc_review from t_document_review" +
                        " where doc_level='" + DocLevels + "' and doc_type ='" + DocType + "' and active=1";
                    DataSet dsDoc = objGlobaldata.Getdetails(stmt);
                    if(dsDoc.Tables[0].Rows != null && dsDoc.Tables[0].Rows.Count >0)
                    {
                        Exist = dsDoc.Tables[0].Rows[0]["id_doc_review"].ToString();
                        return Json("true");
                    }                    
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exceptiion in FunGetExistDoc", ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json ("false");
        }
    }
}