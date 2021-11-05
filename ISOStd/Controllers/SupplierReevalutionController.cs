using ISOStd.Filters;
using ISOStd.Models;
using Rotativa;
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
    public class SupplierReevalutionController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        public SupplierReevalutionController()
        {
            ViewBag.Menutype = "Suppliers";
            ViewBag.SubMenutype = "SupplierReevalution";
        }

        [AllowAnonymous]
        public ActionResult AddSupplierReevalution()
        {
            SupplierReevalutionModels objMdl = new SupplierReevalutionModels();
            try
            {
                Initilization();
                objMdl.branch = objGlobaldata.GetCurrentUserSession().division;
                objMdl.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objMdl.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objMdl.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objMdl.branch);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSupplierReevalution: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objMdl);
        }

        public void Initilization()
        {
            ViewBag.SubMenutype = "SupplierReevalution";

            SurveyModels objSurvey = new SurveyModels();
            ViewBag.SurveyQuestions = objSurvey.GetSurveyTypeListbox("Supplier Reevaluation");
            ViewBag.SurveyRating = objSurvey.GetSurveyRating(objSurvey.getSurveyIDByName("Supplier Reevaluation"));
            ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            ViewBag.Supplier = objGlobaldata.GetSupplierList();
            ViewBag.Iso = objGlobaldata.GetAllIsoStdListbox();
            ViewBag.Division = objGlobaldata.GetCompanyBranchListbox();
            ViewBag.Year = objGlobaldata.GetDropdownList("Years");
            ViewBag.DeptHead = objGlobaldata.GetDeptHeadList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSupplierReevalution(SupplierReevalutionModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> audit_upload, IEnumerable<HttpPostedFileBase> visit_upload)
        {
            try
            {
                objModel.certification = form["certification"];
                objModel.branch = form["branch"];
                objModel.Department = form["Department"];
                objModel.Location = form["Location"];

                DateTime dateValue;

                //if (DateTime.TryParse(form["perf_review_year"], out dateValue) == true)
                //{
                //    objModel.perf_review_year = dateValue;
                //}
                if (DateTime.TryParse(form["audit_date"], out dateValue) == true)
                {
                    objModel.audit_date = dateValue;
                }

                if (DateTime.TryParse(form["date_visited"], out dateValue) == true)
                {
                    objModel.date_visited = dateValue;
                }

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase files = Request.Files[0];
                    if (audit_upload != null && files.ContentLength > 0)
                    {
                        objModel.audit_upload = "";
                        foreach (var file in audit_upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(file.FileName));
                                string sFilename = "Reeval" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objModel.audit_upload = objModel.audit_upload + "," + "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddSupplierReevalution-upload: " + ex.ToString());
                            }
                        }
                        objModel.audit_upload = objModel.audit_upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                }
                if (Request.Files.Count > 0)
                { 
                    HttpPostedFileBase files1 = Request.Files[0];
                if (visit_upload != null && files1.ContentLength > 0)
                {
                    objModel.visit_upload = "";
                    foreach (var file in visit_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(file.FileName));
                            string sFilename = "Reeval" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.visit_upload = objModel.visit_upload + "," + "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddSupplierReevalution-upload: " + ex.ToString());
                        }
                    }
                    objModel.visit_upload = objModel.visit_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
            }

                SupplierReevalutionModelsList objList = new SupplierReevalutionModelsList();
                objList.EvalList = new List<SupplierReevalutionModels>();

                for(int i= 0; i< Convert.ToInt16(form["itemcnt"]);i++)
                {
                    SupplierReevalutionModels objElements = new SupplierReevalutionModels();
                    if(form["cust_name" + i] != "")
                    { 
                        objElements.cust_name = form["cust_name" + i];
                        objElements.complaints = form["complaints" + i];
                        objElements.description_complaint = form["description_complaint" + i];
                        objElements.ref_no_complaint = form["ref_no_complaint" + i];
                    }

                    if (DateTime.TryParse(form["date_reevaluation" + i], out dateValue) == true)
                    {
                        objElements.date_reevaluation = dateValue;
                    }
                    objList.EvalList.Add(objElements);
                }

                SupplierReevalutionModelsList objQuestList = new SupplierReevalutionModelsList();
                objQuestList.EvalList = new List<SupplierReevalutionModels>();

                SurveyModels objSurvey = new SurveyModels();
                MultiSelectList SurveyQuestions = objSurvey.GetSurveyTypeListbox("Supplier Reevaluation");

                foreach (var item in SurveyQuestions)
                {
                    SupplierReevalutionModels objElements = new SupplierReevalutionModels();

                    objElements.SQId = form["Question " + item.Value];
                    objElements.SQ_OptionsId = objModel.GetFunRatingId(form["SQ_OptionsId " + item.Value]);

                    objQuestList.EvalList.Add(objElements);
                }

                if (objModel.FunAddSupplierReevalution(objModel, objList, objQuestList))
                {
                    TempData["Successdata"] = "Added Reevaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSupplierReevalution: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SupplierReevalutionList");
        }

        [AllowAnonymous]
        public ActionResult SupplierReevalutionList(string SearchText, int? page, string branch_name)
        {
            SupplierReevalutionModelsList ObjList = new SupplierReevalutionModelsList();
            ObjList.EvalList = new List<SupplierReevalutionModels>();

            try
            {
                ViewBag.SubMenutype = "SupplierReevalution";

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_reevaluation,perf_review_year,supplier,certification,other_certification,supp_required," +
                    "auditor_name,audit_upload,audit_date,supp_complaint,issatisfactory,anycomplaint,ishandled,remark,visited,visit_upload,date_visited," +
                    "visited_by,recommanded,approved,recommanded_to,approved_to,branch,Department,Location from t_supplier_reevaluation where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + "";
                DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsSupplier.Tables.Count > 0 && i < dsSupplier.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        SupplierReevalutionModels objSupplier = new SupplierReevalutionModels
                        {
                            id_reevaluation = dsSupplier.Tables[0].Rows[i]["id_reevaluation"].ToString(),
                            supplier = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[i]["supplier"].ToString()),
                            certification = objGlobaldata.GetIsoStdNameById(dsSupplier.Tables[0].Rows[i]["certification"].ToString()),
                            other_certification = dsSupplier.Tables[0].Rows[i]["other_certification"].ToString(),
                            supp_required = dsSupplier.Tables[0].Rows[i]["supp_required"].ToString(),
                            auditor_name = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["auditor_name"].ToString()),
                            audit_upload = dsSupplier.Tables[0].Rows[i]["audit_upload"].ToString(),
                            supp_complaint = dsSupplier.Tables[0].Rows[i]["supp_complaint"].ToString(),
                            issatisfactory = dsSupplier.Tables[0].Rows[i]["issatisfactory"].ToString(),
                            anycomplaint = dsSupplier.Tables[0].Rows[i]["anycomplaint"].ToString(),
                            ishandled = dsSupplier.Tables[0].Rows[i]["ishandled"].ToString(),
                            remark = dsSupplier.Tables[0].Rows[i]["remark"].ToString(),
                            visited = dsSupplier.Tables[0].Rows[i]["visited"].ToString(),
                            visit_upload = dsSupplier.Tables[0].Rows[i]["visit_upload"].ToString(),
                            visited_by = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["visited_by"].ToString()),
                            recommanded = dsSupplier.Tables[0].Rows[i]["recommanded"].ToString(),
                            approved = dsSupplier.Tables[0].Rows[i]["approved"].ToString(),
                            recommanded_to = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["recommanded_to"].ToString()),
                            approved_to = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["approved_to"].ToString()),
                            perf_review_year = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["perf_review_year"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[i]["Location"].ToString()),
                        };

                        DateTime dtValue;
                        //if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["perf_review_year"].ToString(), out dtValue))
                        //{
                        //    objSupplier.perf_review_year = dtValue;
                        //}
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["audit_date"].ToString(), out dtValue))
                        {
                            objSupplier.audit_date = dtValue;
                        }
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["date_visited"].ToString(), out dtValue))
                        {
                            objSupplier.date_visited = dtValue;
                        }

                        ObjList.EvalList.Add(objSupplier);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(ObjList.EvalList.ToList());
        }
        
        [AllowAnonymous]
        public ActionResult SupplierReevalutionListSearch(string SearchText, int? page, string branch_name)
        {
            SupplierReevalutionModelsList ObjList = new SupplierReevalutionModelsList();
            ObjList.EvalList = new List<SupplierReevalutionModels>();

            try
            {
                ViewBag.SubMenutype = "SupplierReevalution";

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_reevaluation,perf_review_year,supplier,certification,other_certification,supp_required," +
                    "auditor_name,audit_upload,audit_date,supp_complaint,issatisfactory,anycomplaint,ishandled,remark,visited,visit_upload,date_visited," +
                    "visited_by,recommanded,approved,recommanded_to,approved_to,branch,Department,Location from t_supplier_reevaluation where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + "";
                DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsSupplier.Tables.Count > 0 && i < dsSupplier.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        SupplierReevalutionModels objSupplier = new SupplierReevalutionModels
                        {
                            id_reevaluation = dsSupplier.Tables[0].Rows[i]["id_reevaluation"].ToString(),
                            supplier = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[i]["supplier"].ToString()),
                            certification = objGlobaldata.GetIsoStdNameById(dsSupplier.Tables[0].Rows[i]["certification"].ToString()),
                            other_certification = dsSupplier.Tables[0].Rows[i]["other_certification"].ToString(),
                            supp_required = dsSupplier.Tables[0].Rows[i]["supp_required"].ToString(),
                            auditor_name = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["auditor_name"].ToString()),
                            audit_upload = dsSupplier.Tables[0].Rows[i]["audit_upload"].ToString(),
                            supp_complaint = dsSupplier.Tables[0].Rows[i]["supp_complaint"].ToString(),
                            issatisfactory = dsSupplier.Tables[0].Rows[i]["issatisfactory"].ToString(),
                            anycomplaint = dsSupplier.Tables[0].Rows[i]["anycomplaint"].ToString(),
                            ishandled = dsSupplier.Tables[0].Rows[i]["ishandled"].ToString(),
                            remark = dsSupplier.Tables[0].Rows[i]["remark"].ToString(),
                            visited = dsSupplier.Tables[0].Rows[i]["visited"].ToString(),
                            visit_upload = dsSupplier.Tables[0].Rows[i]["visit_upload"].ToString(),
                            visited_by = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["visited_by"].ToString()),
                            recommanded = dsSupplier.Tables[0].Rows[i]["recommanded"].ToString(),
                            approved = dsSupplier.Tables[0].Rows[i]["approved"].ToString(),
                            recommanded_to = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["recommanded_to"].ToString()),
                            approved_to = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["approved_to"].ToString()),
                            perf_review_year = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["perf_review_year"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[i]["Location"].ToString()),

                        };

                        DateTime dtValue;
                        //if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["perf_review_year"].ToString(), out dtValue))
                        //{
                        //    objSupplier.perf_review_year = dtValue;
                        //}
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["audit_date"].ToString(), out dtValue))
                        {
                            objSupplier.audit_date = dtValue;
                        }
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["date_visited"].ToString(), out dtValue))
                        {
                            objSupplier.date_visited = dtValue;
                        }

                        ObjList.EvalList.Add(objSupplier);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("success");
        }

        [AllowAnonymous]
        public JsonResult SupplierReevalutionDel(FormCollection form)
        {
            try
            {
                if (form["id_reevaluation"] != null && form["id_reevaluation"] != "")
                {
                    SupplierReevalutionModels Doc = new SupplierReevalutionModels();
                    string sid_reevaluation = form["id_reevaluation"];
                    
                    if (Doc.FunDeleteSupplierReevalution(sid_reevaluation))
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
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionDel: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult SupplierReevalutionRecommend(string id_reevaluation, int iStatus, string PendingFlg)
        {
            try
            {
                SupplierReevalutionModels objMdl = new SupplierReevalutionModels();

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

                if (objMdl.FunUpdateReevaluationRecommend(id_reevaluation, iStatus))
                {
                    TempData["Successdata"] = "SupplierReevalutionRecommend is " + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForReview", "Dashboard");
            }
            else
            {
                return RedirectToAction("ListPendingForReview", "Dashboard");
            }
        }

        public JsonResult SupplierReevalutionRecommendNoty(string id_reevaluation, int iStatus, string PendingFlg)
        {
            try
            {
                SupplierReevalutionModels objMdl = new SupplierReevalutionModels();

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

                if (objMdl.FunUpdateReevaluationRecommend(id_reevaluation, iStatus))
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
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionRecommendNoty: " + ex.ToString());
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
        
        [AllowAnonymous]
        public ActionResult SupplierReevalutionApprove(string id_reevaluation, int iStatus, string PendingFlg)
        {
            try
            {
                SupplierReevalutionModels objMdl = new SupplierReevalutionModels();
               
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

                if (objMdl.FunUpdateReevaluationApprvReject(id_reevaluation, iStatus))
                {
                    TempData["Successdata"] = "SupplierReevalutionApprove is " + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }              

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionApprove: " + ex.ToString());
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

        public JsonResult SupplierReevalutionApproveNoty(string id_reevaluation, int iStatus, string PendingFlg)
        {
            try
            {
                SupplierReevalutionModels objMdl = new SupplierReevalutionModels();
                               
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

                if (objMdl.FunUpdateReevaluationApprvReject(id_reevaluation, iStatus))
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
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionApproveNoty: " + ex.ToString());
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

        //Questions

        [AllowAnonymous]
        public ActionResult AddSuppplierReevaluationQuestions()
        {
            SurveyModels objSurvey = new SurveyModels();
            try
            {
                ViewBag.SubMenutype = "SupplierReevalution";
                //ViewBag.dsSurvey = objSurvey.GetSurveyTypeListbox();
                ViewBag.Survey_Type = objSurvey.getSurveyIDByName("Supplier Performance Survey");

                //if (Request.QueryString["Survey_TypeId"] != null && Request.QueryString["Survey_TypeId"] != "")
                //{
                //    ViewBag.Survey_TypeId = Request.QueryString["Survey_TypeId"];
                //    ViewBag.dsSurveyQuestions = objSurvey.GetSurveyQuestionsListbox(Request.QueryString["Survey_TypeId"]);
                //    ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(Request.QueryString["Survey_TypeId"]);
                //}
                //if (Request.QueryString["Survey_TypeId1"] != null && Request.QueryString["Survey_TypeId1"] != "")
                //{
                //    ViewBag.Survey_TypeId = Request.QueryString["Survey_TypeId1"];
                //    ViewBag.dsSurveyQuestions = objSurvey.GetSurveyQuestionsListbox(Request.QueryString["Survey_TypeId1"]);
                //    ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(Request.QueryString["Survey_TypeId1"]);
                //}
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSuppplierReevaluationQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objSurvey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSuppplierReevaluationQuestions(SurveyModels objSurveyModels, FormCollection form)
        {
            try
            {
                SurveyModels objSurvey = new SurveyModels();
                ViewBag.dsSurvey = objSurvey.GetSurveyTypeListbox();
                               
                ViewBag.Survey_Type = objSurveyModels.Survey_TypeId;
                ViewBag.Survey_TypeId = objSurvey.getSurveyIDByName(objSurveyModels.Survey_TypeId);
                objSurveyModels.Survey_TypeId1 = objSurvey.getSurveyIDByName(objSurveyModels.Survey_TypeId);
                if (objSurveyModels.Questions != "")
                {

                    if (objSurveyModels.FunAddSurvey(objSurveyModels))
                    {
                        TempData["Successdata"] = "Added Supplier Performance Questions successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id is required.";
                }
                ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(objSurveyModels.Survey_TypeId);
                ViewBag.dsSurveyQuestions = objSurvey.GetSurveyQuestionsListbox(objSurveyModels.Survey_TypeId);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSuppplierReevaluationQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objSurveyModels);
        }

        [AllowAnonymous]
        public ActionResult SuppReevalQuestionsDelete(string SQId)
        {

            ViewBag.SubMenutype = "SupplierReevalution";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunDeleteSurveyQuestions(SQId))
                {
                    TempData["Successdata"] = "Supplier Performance details deleted successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppReevalQuestionsDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddSuppplierReevaluationQuestions", new { Survey_TypeId = objSurveyModels.GetSurveyTypeIdByQuestionId(SQId) });
        }

        [AllowAnonymous]
        public ActionResult SuppReevalQuestionsDelete1(string SQId)
        {

            ViewBag.SubMenutype = "SupplierReevalution";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunDeleteSurveyQuestions(SQId))
                {
                    TempData["Successdata"] = "Survey details deleted successfully";
                    return Json("Success");

                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppReevalQuestionsDelete1: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult SuppReevalQuestionUpdate(string SQId, string Questions)
        {

            ViewBag.SubMenutype = "SupplierReevalution";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunUpdateSurveyQuestions(SQId, Questions))
                {
                    TempData["Successdata"] = "Survey details updated successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppReevalQuestionUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");

        }

        public JsonResult FunGetsupplierInfo(string supplier_id)
        {
            SupplierModels objSupp = new SupplierModels();
            string sql = "Select SupplierCode,SupplyScope from t_supplier where SupplierID='" + supplier_id + "'and active=1";
            DataSet dsSupp = objGlobaldata.Getdetails(sql);
            if(dsSupp.Tables.Count>0 && dsSupp.Tables[0].Rows.Count>0)
            {
                 objSupp = new SupplierModels
                {
                    SupplierCode = dsSupp.Tables[0].Rows[0]["SupplierCode"].ToString(),
                    SupplyScope = dsSupp.Tables[0].Rows[0]["SupplyScope"].ToString()
                };
            }
            return Json(objSupp);
        }

        public ActionResult GetRating()
        {
            try
            {
                string sql = "select RatingOptions,Weightage from t_survey_type a,t_surveyquestion_options b where a.Survey_TypeId=b.Survey_TypeId and a.TypeName='Supplier Reevaluation'";
                DataSet dsRating = objGlobaldata.Getdetails(sql);
                ViewBag.dsRating = dsRating;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetRating: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        //Edit

        [AllowAnonymous]
        public ActionResult SupplierReevalutionEdit()
        {
            SupplierReevalutionModels objSupplier = new SupplierReevalutionModels();
          
            try
            {
                ViewBag.SubMenutype = "SupplierReevalution";

                if (Request.QueryString["id_reevaluation"] != "" && Request.QueryString["id_reevaluation"] != null)
                {
                    string sid_reevaluation = Request.QueryString["id_reevaluation"];
                    string sSqlstmt = "select id_reevaluation,perf_review_year,supplier,certification,other_certification,supp_required," +
                        "auditor_name,audit_upload,audit_date,supp_complaint,issatisfactory,anycomplaint,ishandled,remark,visited,visit_upload,date_visited," +
                        "visited_by,recommanded,approved,recommanded_to,approved_to,branch,Department,Location from t_supplier_reevaluation where id_reevaluation= '" + sid_reevaluation + "'";

                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    try                        
                    {
                        if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                        {
                            objSupplier = new SupplierReevalutionModels
                            {
                                id_reevaluation = dsSupplier.Tables[0].Rows[0]["id_reevaluation"].ToString(),
                                supplier = /*objGlobaldata.GetSupplierNameById*/(dsSupplier.Tables[0].Rows[0]["supplier"].ToString()),
                                certification = /*objGlobaldata.GetIsoStdNameById*/(dsSupplier.Tables[0].Rows[0]["certification"].ToString()),
                                other_certification = dsSupplier.Tables[0].Rows[0]["other_certification"].ToString(),
                                supp_required = dsSupplier.Tables[0].Rows[0]["supp_required"].ToString(),
                                auditor_name = /*objGlobaldata.GetEmpHrNameById*/(dsSupplier.Tables[0].Rows[0]["auditor_name"].ToString()),
                                audit_upload = dsSupplier.Tables[0].Rows[0]["audit_upload"].ToString(),
                                supp_complaint = dsSupplier.Tables[0].Rows[0]["supp_complaint"].ToString(),
                                issatisfactory = dsSupplier.Tables[0].Rows[0]["issatisfactory"].ToString(),
                                anycomplaint = dsSupplier.Tables[0].Rows[0]["anycomplaint"].ToString(),
                                ishandled = dsSupplier.Tables[0].Rows[0]["ishandled"].ToString(),
                                remark = dsSupplier.Tables[0].Rows[0]["remark"].ToString(),
                                visited = dsSupplier.Tables[0].Rows[0]["visited"].ToString(),
                                visit_upload = dsSupplier.Tables[0].Rows[0]["visit_upload"].ToString(),
                                visited_by = /*objGlobaldata.GetEmpHrNameById*/(dsSupplier.Tables[0].Rows[0]["visited_by"].ToString()),
                                recommanded = dsSupplier.Tables[0].Rows[0]["recommanded"].ToString(),
                                approved = dsSupplier.Tables[0].Rows[0]["approved"].ToString(),
                                recommanded_to = /*objGlobaldata.GetEmpHrNameById*/(dsSupplier.Tables[0].Rows[0]["recommanded_to"].ToString()),
                                approved_to = /*objGlobaldata.GetEmpHrNameById*/(dsSupplier.Tables[0].Rows[0]["approved_to"].ToString()),
                                perf_review_year = /*objGlobaldata.GetDropdownitemById*/(dsSupplier.Tables[0].Rows[0]["perf_review_year"].ToString()),
                                branch = (dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                                Department = (dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                                Location = (dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                            };

                            DateTime dtValue;
                            //if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["perf_review_year"].ToString(), out dtValue))
                            //{
                            //    objSupplier.perf_review_year = dtValue;
                            //}
                            if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["audit_date"].ToString(), out dtValue))
                            {
                                objSupplier.audit_date = dtValue;
                            }
                            if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["date_visited"].ToString(), out dtValue))
                            {
                                objSupplier.date_visited = dtValue;
                            }
                            ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsSupplier.Tables[0].Rows[0]["branch"].ToString());
                            ViewBag.Department = objGlobaldata.GetDepartmentList1(dsSupplier.Tables[0].Rows[0]["branch"].ToString());
                            ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();

                        }

                        SupplierReevalutionModelsList ObjList = new SupplierReevalutionModelsList();
                        ObjList.EvalList = new List<SupplierReevalutionModels>();

                        string Sql = "Select id_reevaluation_trans,id_reevaluation,date_reevaluation,cust_name,complaints," +
                            "description_complaint,ref_no_complaint from t_supplier_reevaluation_trans where id_reevaluation = '"+ sid_reevaluation + "'";
                        DataSet dsSuppTrans = objGlobaldata.Getdetails(Sql);

                        if (dsSuppTrans.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsSuppTrans.Tables[0].Rows.Count; i++)
                            { 
                                try
                                {
                                    SupplierReevalutionModels objSuppTrans = new SupplierReevalutionModels
                                    {
                                        id_reevaluation = dsSuppTrans.Tables[0].Rows[i]["id_reevaluation"].ToString(),
                                        cust_name = dsSuppTrans.Tables[0].Rows[i]["cust_name"].ToString(),
                                        complaints = dsSuppTrans.Tables[0].Rows[i]["complaints"].ToString(),
                                        description_complaint = dsSuppTrans.Tables[0].Rows[i]["description_complaint"].ToString(),
                                        ref_no_complaint = dsSuppTrans.Tables[0].Rows[i]["ref_no_complaint"].ToString(),
                                    };
                                    DateTime dtValue;
                                    if (DateTime.TryParse(dsSuppTrans.Tables[0].Rows[i]["date_reevaluation"].ToString(), out dtValue))
                                    {
                                        objSuppTrans.date_reevaluation = dtValue;
                                    }
                                    ObjList.EvalList.Add(objSuppTrans);
                                }
                                catch (Exception Ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit:" + Ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }
                        ViewBag.ObjTransList=ObjList;

                        string Sqlstmt = "Select id_reevaluation_quest,id_reevaluation,SQId,SQ_OptionsId from t_supplier_reevaluation_quest where id_reevaluation = '" + sid_reevaluation + "'";
                        DataSet dsSuppQuest = objGlobaldata.Getdetails(Sqlstmt);

                        SupplierReevalutionModelsList ObjQuestList = new SupplierReevalutionModelsList();
                        ObjQuestList.EvalList = new List<SupplierReevalutionModels>();

                        Dictionary<string, string> dicQuestElements = new Dictionary<string, string>();

                        if (dsSuppQuest.Tables.Count > 0 && dsSuppQuest.Tables[0].Rows.Count > 0)
                        {
                            for (int i=0; i< dsSuppQuest.Tables[0].Rows.Count; i++)
                            {
                                try {

                                    SupplierReevalutionModels objSuppQuest = new SupplierReevalutionModels()
                                    {
                                        id_reevaluation = dsSuppQuest.Tables[0].Rows[i]["id_reevaluation"].ToString(),
                                        SQId = dsSuppQuest.Tables[0].Rows[i]["SQId"].ToString(),
                                        SQ_OptionsId = dsSuppQuest.Tables[0].Rows[i]["SQ_OptionsId"].ToString(),
                                    };
                                    dicQuestElements.Add(dsSuppQuest.Tables[0].Rows[i]["SQId"].ToString(), dsSuppQuest.Tables[0].Rows[i]["SQ_OptionsId"].ToString());
                                    ObjQuestList.EvalList.Add(objSuppQuest);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }                                
                            }
                        }
                        ViewBag.ObjQuestList = ObjQuestList;
                        ViewBag.dicQuestElements = dicQuestElements;

                        Initilization();
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objSupplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierReevalutionEdit(SupplierReevalutionModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> audit_upload, IEnumerable<HttpPostedFileBase> visit_upload)
        {
            try
            {
                objModel.certification = form["certification"];
                objModel.branch = form["branch"];
                objModel.Department = form["Department"];
                objModel.Location = form["Location"];

                DateTime dateValue;

                //if (DateTime.TryParse(form["perf_review_year"], out dateValue) == true)
                //{
                //    objModel.perf_review_year = dateValue;
                //}
                if (DateTime.TryParse(form["audit_date"], out dateValue) == true)
                {
                    objModel.audit_date = dateValue;
                }

                if (DateTime.TryParse(form["date_visited"], out dateValue) == true)
                {
                    objModel.date_visited = dateValue;
                }
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase files = Request.Files[0];
                    
                    if (audit_upload != null && files.ContentLength > 0)
                    {
                        objModel.audit_upload = "";
                        foreach (var file in audit_upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(file.FileName));
                                string sFilename = "Reeval" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objModel.audit_upload = objModel.audit_upload + "," + "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit-upload: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                        objModel.audit_upload = objModel.audit_upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                }

                string QCDelete = Request.Form["QCDocsValselectall"];

                   if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                    {
                        objModel.audit_upload = objModel.audit_upload + "," + form["QCDocsVal"];
                        objModel.audit_upload = objModel.audit_upload.Trim(',');
                    }
                    else if (form["QCDocsVal"] == null && QCDelete != null && Request.Files.Count == 0)
                    {
                        objModel.audit_upload = null;
                    }
                    else if (form["QCDocsVal"] == null && Request.Files.Count == 0)
                    {
                        objModel.audit_upload = null;
                    }


                if (Request.Files.Count > 0)
                {

                    HttpPostedFileBase files1 = Request.Files[0];
                    
                    if (visit_upload != null && files1.ContentLength > 0)
                    {
                        objModel.visit_upload = "";
                        foreach (var file in visit_upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(file.FileName));
                                string sFilename = "Reeval" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objModel.visit_upload = objModel.visit_upload + "," + "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit-upload: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                        objModel.visit_upload = objModel.visit_upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                }

                string QCDelete1 = Request.Form["QCDocsValselectall1"];

                if (form["QCDocsVal1"] != null && form["QCDocsVal1"] != "")
                {
                    objModel.visit_upload = objModel.visit_upload + "," + form["QCDocsVal1"];
                    objModel.visit_upload = objModel.visit_upload.Trim(',');
                }
                else if (form["QCDocsVal1"] == null && QCDelete1 != null && Request.Files.Count == 0)
                {
                    objModel.visit_upload = null;
                }
                else if (form["QCDocsVal1"] == null && Request.Files.Count == 0)
                {
                    objModel.visit_upload = null;
                }
            

                SupplierReevalutionModelsList objList = new SupplierReevalutionModelsList();
                objList.EvalList = new List<SupplierReevalutionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    SupplierReevalutionModels objElements = new SupplierReevalutionModels();
                    if (form["cust_name " + i] != "" && form["cust_name " + i] != null)
                    {
                        objElements.cust_name = form["cust_name " + i];
                        objElements.complaints = form["complaints " + i];
                        objElements.description_complaint = form["description_complaint " + i];
                        objElements.ref_no_complaint = form["ref_no_complaint " + i];
                    }

                    if (DateTime.TryParse(form["date_reevaluation " + i], out dateValue) == true)
                    {
                        objElements.date_reevaluation = dateValue;
                    }
                    objList.EvalList.Add(objElements);
                }

                SupplierReevalutionModelsList objQuestList = new SupplierReevalutionModelsList();
                objQuestList.EvalList = new List<SupplierReevalutionModels>();

                SurveyModels objSurvey = new SurveyModels();
                MultiSelectList SurveyQuestions = objSurvey.GetSurveyTypeListbox("Supplier Reevaluation");

                foreach (var item in SurveyQuestions)
                {
                    SupplierReevalutionModels objElements = new SupplierReevalutionModels();

                    objElements.SQId = form["Question " + item.Value];
                    objElements.SQ_OptionsId = objModel.GetFunRatingId(form["SQ_OptionsId " + item.Value]);

                    objQuestList.EvalList.Add(objElements);
                }

                if (objModel.FunUpdateSupplierReevalution(objModel, objList, objQuestList))
                {
                    TempData["Successdata"] = "Updated Reevaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SupplierReevalutionList");
        }

        public JsonResult FunGetApproverList(string Emp_id)
        {
            EmployeeModels emp = new EmployeeModels();
            DataSet dsEmp = objGlobaldata.Getdetails("Select EvaluatedBy from t_hr_employee where emp_no = '" + Emp_id + "'");
            if(dsEmp.Tables.Count >0 && dsEmp.Tables[0].Rows.Count>0)
            {
                emp.EmpID = dsEmp.Tables[0].Rows[0]["EvaluatedBy"].ToString();
                emp.CompEmpId = objGlobaldata.GetEmpHrNameById(dsEmp.Tables[0].Rows[0]["EvaluatedBy"].ToString());
            }
            return Json(emp);
        }

        [AllowAnonymous]
        public ActionResult SupplierReevalutionDetails()
        {
            SupplierReevalutionModels objSupplier = new SupplierReevalutionModels();

            try
            {
                ViewBag.SubMenutype = "SupplierReevalution";

                if (Request.QueryString["id_reevaluation"] != "" && Request.QueryString["id_reevaluation"] != null)
                {
                    string sid_reevaluation = Request.QueryString["id_reevaluation"];
                    string sSqlstmt = "select id_reevaluation,perf_review_year,supplier,certification,other_certification,supp_required," +
                        "auditor_name,audit_upload,audit_date,supp_complaint,issatisfactory,anycomplaint,ishandled,remark,visited,visit_upload,date_visited," +
                        "visited_by,recommanded,approved,recommanded_to,approved_to,branch,Department,Location from t_supplier_reevaluation where id_reevaluation= '" + sid_reevaluation + "'";

                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    try
                    {
                        if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                        {
                            objSupplier = new SupplierReevalutionModels
                            {
                                id_reevaluation = dsSupplier.Tables[0].Rows[0]["id_reevaluation"].ToString(),
                                supplier = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[0]["supplier"].ToString()),
                                certification = objGlobaldata.GetIsoStdNameById(dsSupplier.Tables[0].Rows[0]["certification"].ToString()),
                                other_certification = dsSupplier.Tables[0].Rows[0]["other_certification"].ToString(),
                                supp_required = dsSupplier.Tables[0].Rows[0]["supp_required"].ToString(),
                                auditor_name = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["auditor_name"].ToString()),
                                audit_upload = dsSupplier.Tables[0].Rows[0]["audit_upload"].ToString(),
                                supp_complaint = dsSupplier.Tables[0].Rows[0]["supp_complaint"].ToString(),
                                issatisfactory = dsSupplier.Tables[0].Rows[0]["issatisfactory"].ToString(),
                                anycomplaint = dsSupplier.Tables[0].Rows[0]["anycomplaint"].ToString(),
                                ishandled = dsSupplier.Tables[0].Rows[0]["ishandled"].ToString(),
                                remark = dsSupplier.Tables[0].Rows[0]["remark"].ToString(),
                                visited = dsSupplier.Tables[0].Rows[0]["visited"].ToString(),
                                visit_upload = dsSupplier.Tables[0].Rows[0]["visit_upload"].ToString(),
                                visited_by = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["visited_by"].ToString()),
                                recommanded = dsSupplier.Tables[0].Rows[0]["recommanded"].ToString(),
                                approved = dsSupplier.Tables[0].Rows[0]["approved"].ToString(),
                                recommanded_to = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["recommanded_to"].ToString()),
                                approved_to = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["approved_to"].ToString()),
                                perf_review_year = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["perf_review_year"].ToString()),
                                supp_code= objGlobaldata.GetSupplierCodeBySupplierId(dsSupplier.Tables[0].Rows[0]["supplier"].ToString()),
                                supp_scope= objGlobaldata.GetSupplierScopeBySupplierId(dsSupplier.Tables[0].Rows[0]["supplier"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                            };

                            DateTime dtValue;
                            //if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["perf_review_year"].ToString(), out dtValue))
                            //{
                            //    objSupplier.perf_review_year = dtValue;
                            //}
                            if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["audit_date"].ToString(), out dtValue))
                            {
                                objSupplier.audit_date = dtValue;
                            }
                            if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["date_visited"].ToString(), out dtValue))
                            {
                                objSupplier.date_visited = dtValue;
                            }
                        }

                        SupplierReevalutionModelsList ObjList = new SupplierReevalutionModelsList();
                        ObjList.EvalList = new List<SupplierReevalutionModels>();

                        string Sql = "Select id_reevaluation_trans,id_reevaluation,date_reevaluation,cust_name,complaints," +
                            "description_complaint,ref_no_complaint from t_supplier_reevaluation_trans where id_reevaluation = '" + sid_reevaluation + "'";
                        DataSet dsSuppTrans = objGlobaldata.Getdetails(Sql);

                        if (dsSuppTrans.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsSuppTrans.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    SupplierReevalutionModels objSuppTrans = new SupplierReevalutionModels
                                    {
                                        id_reevaluation = dsSuppTrans.Tables[0].Rows[i]["id_reevaluation"].ToString(),
                                        cust_name = dsSuppTrans.Tables[0].Rows[i]["cust_name"].ToString(),
                                        complaints = dsSuppTrans.Tables[0].Rows[i]["complaints"].ToString(),
                                        description_complaint = dsSuppTrans.Tables[0].Rows[i]["description_complaint"].ToString(),
                                        ref_no_complaint = dsSuppTrans.Tables[0].Rows[i]["ref_no_complaint"].ToString(),
                                    };
                                    DateTime dtValue;
                                    if (DateTime.TryParse(dsSuppTrans.Tables[0].Rows[i]["date_reevaluation"].ToString(), out dtValue))
                                    {
                                        objSuppTrans.date_reevaluation = dtValue;
                                    }
                                    ObjList.EvalList.Add(objSuppTrans);
                                }
                                catch (Exception Ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit:" + Ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }
                        ViewBag.ObjTransList = ObjList;

                        string Sqlstmt = "Select id_reevaluation_quest,id_reevaluation,SQId,SQ_OptionsId from t_supplier_reevaluation_quest where id_reevaluation = '" + sid_reevaluation + "'";
                        DataSet dsSuppQuest = objGlobaldata.Getdetails(Sqlstmt);

                        SurveyModels survey = new SurveyModels();
                        SupplierReevalutionModelsList ObjQuestList = new SupplierReevalutionModelsList();
                        ObjQuestList.EvalList = new List<SupplierReevalutionModels>();

                        //Dictionary<string, string> dicQuestElements = new Dictionary<string, string>();

                        if (dsSuppQuest.Tables.Count > 0 && dsSuppQuest.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsSuppQuest.Tables[0].Rows.Count; i++)
                            {
                                try
                                {

                                    SupplierReevalutionModels objSuppQuest = new SupplierReevalutionModels()
                                    {
                                        id_reevaluation = dsSuppQuest.Tables[0].Rows[i]["id_reevaluation"].ToString(),
                                        SQId = survey.GetSurveyQuestionNameById(dsSuppQuest.Tables[0].Rows[i]["SQId"].ToString()),
                                        SQ_OptionsId = survey.GetSurveyRatingNameById(dsSuppQuest.Tables[0].Rows[i]["SQ_OptionsId"].ToString()),
                                    };
                                    //dicQuestElements.Add(dsSuppQuest.Tables[0].Rows[i]["SQId"].ToString(), dsSuppQuest.Tables[0].Rows[i]["SQ_OptionsId"].ToString());
                                    ObjQuestList.EvalList.Add(objSuppQuest);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }
                        ViewBag.ObjQuestList = ObjQuestList;
                       // ViewBag.dicQuestElements = dicQuestElements;
                     
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objSupplier);
        }

        [AllowAnonymous]
        public ActionResult SupplierReevalutionInfo(int id)
        {
            SupplierReevalutionModels objSupplier = new SupplierReevalutionModels();

            try
            {
                ViewBag.SubMenutype = "SupplierReevalution";

                if (id > 0)
                {
                    string sSqlstmt = "select id_reevaluation,perf_review_year,supplier,certification,other_certification,supp_required," +
                        "auditor_name,audit_upload,audit_date,supp_complaint,issatisfactory,anycomplaint,ishandled,remark,visited,visit_upload,date_visited," +
                        "visited_by,recommanded,approved,recommanded_to,approved_to,branch,Department,Location from t_supplier_reevaluation where id_reevaluation= '" + id + "'";

                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    try
                    {
                        if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                        {
                            objSupplier = new SupplierReevalutionModels
                            {
                                id_reevaluation = dsSupplier.Tables[0].Rows[0]["id_reevaluation"].ToString(),
                                supplier = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[0]["supplier"].ToString()),
                                certification = objGlobaldata.GetIsoStdNameById(dsSupplier.Tables[0].Rows[0]["certification"].ToString()),
                                other_certification = dsSupplier.Tables[0].Rows[0]["other_certification"].ToString(),
                                supp_required = dsSupplier.Tables[0].Rows[0]["supp_required"].ToString(),
                                auditor_name = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["auditor_name"].ToString()),
                                audit_upload = dsSupplier.Tables[0].Rows[0]["audit_upload"].ToString(),
                                supp_complaint = dsSupplier.Tables[0].Rows[0]["supp_complaint"].ToString(),
                                issatisfactory = dsSupplier.Tables[0].Rows[0]["issatisfactory"].ToString(),
                                anycomplaint = dsSupplier.Tables[0].Rows[0]["anycomplaint"].ToString(),
                                ishandled = dsSupplier.Tables[0].Rows[0]["ishandled"].ToString(),
                                remark = dsSupplier.Tables[0].Rows[0]["remark"].ToString(),
                                visited = dsSupplier.Tables[0].Rows[0]["visited"].ToString(),
                                visit_upload = dsSupplier.Tables[0].Rows[0]["visit_upload"].ToString(),
                                visited_by = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["visited_by"].ToString()),
                                recommanded = dsSupplier.Tables[0].Rows[0]["recommanded"].ToString(),
                                approved = dsSupplier.Tables[0].Rows[0]["approved"].ToString(),
                                recommanded_to = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["recommanded_to"].ToString()),
                                approved_to = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["approved_to"].ToString()),
                                perf_review_year = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["perf_review_year"].ToString()),
                                supp_code = objGlobaldata.GetSupplierCodeBySupplierId(dsSupplier.Tables[0].Rows[0]["supplier"].ToString()),
                                supp_scope = objGlobaldata.GetSupplierScopeBySupplierId(dsSupplier.Tables[0].Rows[0]["supplier"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                            };

                            DateTime dtValue;
                            //if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["perf_review_year"].ToString(), out dtValue))
                            //{
                            //    objSupplier.perf_review_year = dtValue;
                            //}
                            if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["audit_date"].ToString(), out dtValue))
                            {
                                objSupplier.audit_date = dtValue;
                            }
                            if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["date_visited"].ToString(), out dtValue))
                            {
                                objSupplier.date_visited = dtValue;
                            }
                        }

                        SupplierReevalutionModelsList ObjList = new SupplierReevalutionModelsList();
                        ObjList.EvalList = new List<SupplierReevalutionModels>();

                        string Sql = "Select id_reevaluation_trans,id_reevaluation,date_reevaluation,cust_name,complaints," +
                            "description_complaint,ref_no_complaint from t_supplier_reevaluation_trans where id_reevaluation = '" + id + "'";
                        DataSet dsSuppTrans = objGlobaldata.Getdetails(Sql);

                        if (dsSuppTrans.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsSuppTrans.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    SupplierReevalutionModels objSuppTrans = new SupplierReevalutionModels
                                    {
                                        id_reevaluation = dsSuppTrans.Tables[0].Rows[i]["id_reevaluation"].ToString(),
                                        cust_name = dsSuppTrans.Tables[0].Rows[i]["cust_name"].ToString(),
                                        complaints = dsSuppTrans.Tables[0].Rows[i]["complaints"].ToString(),
                                        description_complaint = dsSuppTrans.Tables[0].Rows[i]["description_complaint"].ToString(),
                                        ref_no_complaint = dsSuppTrans.Tables[0].Rows[i]["ref_no_complaint"].ToString(),
                                    };
                                    DateTime dtValue;
                                    if (DateTime.TryParse(dsSuppTrans.Tables[0].Rows[i]["date_reevaluation"].ToString(), out dtValue))
                                    {
                                        objSuppTrans.date_reevaluation = dtValue;
                                    }
                                    ObjList.EvalList.Add(objSuppTrans);
                                }
                                catch (Exception Ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit:" + Ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }
                        ViewBag.ObjTransList = ObjList;

                        string Sqlstmt = "Select id_reevaluation_quest,id_reevaluation,SQId,SQ_OptionsId from t_supplier_reevaluation_quest where id_reevaluation = '" + id + "'";
                        DataSet dsSuppQuest = objGlobaldata.Getdetails(Sqlstmt);

                        SurveyModels survey = new SurveyModels();
                        SupplierReevalutionModelsList ObjQuestList = new SupplierReevalutionModelsList();
                        ObjQuestList.EvalList = new List<SupplierReevalutionModels>();

                        //Dictionary<string, string> dicQuestElements = new Dictionary<string, string>();

                        if (dsSuppQuest.Tables.Count > 0 && dsSuppQuest.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsSuppQuest.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    SupplierReevalutionModels objSuppQuest = new SupplierReevalutionModels()
                                    {
                                        id_reevaluation = dsSuppQuest.Tables[0].Rows[i]["id_reevaluation"].ToString(),
                                        SQId = survey.GetSurveyQuestionNameById(dsSuppQuest.Tables[0].Rows[i]["SQId"].ToString()),
                                        SQ_OptionsId = survey.GetSurveyRatingNameById(dsSuppQuest.Tables[0].Rows[i]["SQ_OptionsId"].ToString()),
                                    };
                                    //dicQuestElements.Add(dsSuppQuest.Tables[0].Rows[i]["SQId"].ToString(), dsSuppQuest.Tables[0].Rows[i]["SQ_OptionsId"].ToString());
                                    ObjQuestList.EvalList.Add(objSuppQuest);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }
                        ViewBag.ObjQuestList = ObjQuestList;
                        // ViewBag.dicQuestElements = dicQuestElements;

                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objSupplier);
        }

        [AllowAnonymous]
        public ActionResult SupplierReevalutionPDF(FormCollection form)
        {
            SupplierReevalutionModels objSupplier = new SupplierReevalutionModels();
            try
            {
                ViewBag.SubMenutype = "SupplierReevalution";

                if (form["id_reevaluation"] != "" && form["id_reevaluation"] != null)
                {
                    string sid_reevaluation = form["id_reevaluation"];
                    string sSqlstmt = "select id_reevaluation,perf_review_year,supplier,certification,other_certification,supp_required," +
                        "auditor_name,audit_upload,audit_date,supp_complaint,issatisfactory,anycomplaint,ishandled,remark,visited,visit_upload,date_visited," +
                        "visited_by,recommanded,approved,recommanded_to,approved_to,branch,Department,Location from t_supplier_reevaluation where id_reevaluation= '" + sid_reevaluation + "'";

                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    try
                    {
                        if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                        {
                            objSupplier = new SupplierReevalutionModels
                            {
                                id_reevaluation = dsSupplier.Tables[0].Rows[0]["id_reevaluation"].ToString(),
                                supplier = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[0]["supplier"].ToString()),
                                certification = objGlobaldata.GetIsoStdNameById(dsSupplier.Tables[0].Rows[0]["certification"].ToString()),
                                other_certification = dsSupplier.Tables[0].Rows[0]["other_certification"].ToString(),
                                supp_required = dsSupplier.Tables[0].Rows[0]["supp_required"].ToString(),
                                auditor_name = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["auditor_name"].ToString()),
                                audit_upload = dsSupplier.Tables[0].Rows[0]["audit_upload"].ToString(),
                                supp_complaint = dsSupplier.Tables[0].Rows[0]["supp_complaint"].ToString(),
                                issatisfactory = dsSupplier.Tables[0].Rows[0]["issatisfactory"].ToString(),
                                anycomplaint = dsSupplier.Tables[0].Rows[0]["anycomplaint"].ToString(),
                                ishandled = dsSupplier.Tables[0].Rows[0]["ishandled"].ToString(),
                                remark = dsSupplier.Tables[0].Rows[0]["remark"].ToString(),
                                visited = dsSupplier.Tables[0].Rows[0]["visited"].ToString(),
                                visit_upload = dsSupplier.Tables[0].Rows[0]["visit_upload"].ToString(),
                                visited_by = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["visited_by"].ToString()),
                                recommanded = dsSupplier.Tables[0].Rows[0]["recommanded"].ToString(),
                                approved = dsSupplier.Tables[0].Rows[0]["approved"].ToString(),
                                recommanded_to = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["recommanded_to"].ToString()),
                                approved_to = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["approved_to"].ToString()),
                                perf_review_year = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["perf_review_year"].ToString()),
                                supp_code = objGlobaldata.GetSupplierCodeBySupplierId(dsSupplier.Tables[0].Rows[0]["supplier"].ToString()),
                                supp_scope = objGlobaldata.GetSupplierScopeBySupplierId(dsSupplier.Tables[0].Rows[0]["supplier"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[0]["Location"].ToString()),

                            };

                            DateTime dtValue;
                            //if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["perf_review_year"].ToString(), out dtValue))
                            //{
                            //    objSupplier.perf_review_year = dtValue;
                            //}
                            if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["audit_date"].ToString(), out dtValue))
                            {
                                objSupplier.audit_date = dtValue;
                            }
                            if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["date_visited"].ToString(), out dtValue))
                            {
                                objSupplier.date_visited = dtValue;
                            }
                            CompanyModels objCompany = new CompanyModels();
                            dsSupplier = objCompany.GetCompanyDetailsForReport(dsSupplier);

                            string loggedby = objGlobaldata.GetCurrentUserSession().empid;
                            dsSupplier = objGlobaldata.GetReportDetails(dsSupplier, objSupplier.id_reevaluation, loggedby, "SUPPLIER REEVALUATION REPORT");
                            ViewBag.CompanyInfo = dsSupplier;

                        }
                        ViewBag.Supplier = objSupplier;
                        //SupplierReevalutionModelsList ObjList = new SupplierReevalutionModelsList();
                        //ObjList.EvalList = new List<SupplierReevalutionModels>();

                        string Sql = "Select id_reevaluation_trans,id_reevaluation,date_reevaluation,cust_name,complaints," +
                            "description_complaint,ref_no_complaint from t_supplier_reevaluation_trans where id_reevaluation = '" + sid_reevaluation + "'";
                        ViewBag.ObjTransList = objGlobaldata.Getdetails(Sql);

                        //if (dsSuppTrans.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                        //{
                        //    for (int i = 0; i < dsSuppTrans.Tables[0].Rows.Count; i++)
                        //    {
                        //        try
                        //        {
                        //            SupplierReevalutionModels objSuppTrans = new SupplierReevalutionModels
                        //            {
                        //                id_reevaluation = dsSuppTrans.Tables[0].Rows[i]["id_reevaluation"].ToString(),
                        //                cust_name = dsSuppTrans.Tables[0].Rows[i]["cust_name"].ToString(),
                        //                complaints = dsSuppTrans.Tables[0].Rows[i]["complaints"].ToString(),
                        //                description_complaint = dsSuppTrans.Tables[0].Rows[i]["description_complaint"].ToString(),
                        //                ref_no_complaint = dsSuppTrans.Tables[0].Rows[i]["ref_no_complaint"].ToString(),
                        //            };
                        //            DateTime dtValue;
                        //            if (DateTime.TryParse(dsSuppTrans.Tables[0].Rows[i]["date_reevaluation"].ToString(), out dtValue))
                        //            {
                        //                objSuppTrans.date_reevaluation = dtValue;
                        //            }
                        //            ObjList.EvalList.Add(objSuppTrans);
                        //        }
                        //        catch (Exception Ex)
                        //        {
                        //            objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit:" + Ex.ToString());
                        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        //        }
                        //    }
                        //}
                        //ViewBag.ObjTransList = ObjList;

                        string Sqlstmt = "Select id_reevaluation_quest,id_reevaluation,SQId,SQ_OptionsId from t_supplier_reevaluation_quest where id_reevaluation = '" + sid_reevaluation + "'";
                        ViewBag.ObjQuestList = objGlobaldata.Getdetails(Sqlstmt);

                        //SurveyModels survey = new SurveyModels();
                        //SupplierReevalutionModelsList ObjQuestList = new SupplierReevalutionModelsList();
                        //ObjQuestList.EvalList = new List<SupplierReevalutionModels>();

                        //Dictionary<string, string> dicQuestElements = new Dictionary<string, string>();

                        //if (dsSuppQuest.Tables.Count > 0 && dsSuppQuest.Tables[0].Rows.Count > 0)
                        //{
                        //    for (int i = 0; i < dsSuppQuest.Tables[0].Rows.Count; i++)
                        //    {
                        //        try
                        //        {

                        //            SupplierReevalutionModels objSuppQuest = new SupplierReevalutionModels()
                        //            {
                        //                id_reevaluation = dsSuppQuest.Tables[0].Rows[i]["id_reevaluation"].ToString(),
                        //                SQId = survey.GetSurveyQuestionNameById(dsSuppQuest.Tables[0].Rows[i]["SQId"].ToString()),
                        //                SQ_OptionsId = survey.GetSurveyRatingNameById(dsSuppQuest.Tables[0].Rows[i]["SQ_OptionsId"].ToString()),
                        //            };
                        //            //dicQuestElements.Add(dsSuppQuest.Tables[0].Rows[i]["SQId"].ToString(), dsSuppQuest.Tables[0].Rows[i]["SQ_OptionsId"].ToString());
                        //            ObjQuestList.EvalList.Add(objSuppQuest);
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionEdit: " + ex.ToString());
                        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        //        }
                        //    }
                        //}
                        //ViewBag.ObjQuestList = ObjQuestList;
                        // ViewBag.dicQuestElements = dicQuestElements;

                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierReevalutionPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("SupplierReevalutionPDF")
            {
                //FileName = "Reevaluation.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }
    }
}