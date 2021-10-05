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
using Microsoft.Reporting.WebForms;
using ISOStd.Filters;
using Rotativa;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class LegalRegisterController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();


        public LegalRegisterController()
        {
            ViewBag.Menutype = "LegalRegister";
            ViewBag.SubMenutype = "Compliance";
        }

        //
        // GET: /LegalRegister/

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AddCompliance()
        {
            LegalRegisterModel ObjMdl = new LegalRegisterModel();
            try
            {
                ViewBag.SubMenutype = "compliance";
                ObjMdl.branch = objGlobaldata.GetCurrentUserSession().division;
                ObjMdl.deptid = objGlobaldata.GetCurrentUserSession().DeptID;
                ObjMdl.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Compliance = objGlobaldata.GetDropdownList("Compliance");
                ViewBag.ISOStds = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(ObjMdl.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(ObjMdl.branch);
                ViewBag.LawRelevantTo= objGlobaldata.GetDropdownList("Law Relevant To");
                ViewBag.LawIssuedBy = objGlobaldata.GetDropdownList("Law Issued By");
                ViewBag.LawIssueAuthority = objGlobaldata.GetDropdownList("Law Issue Authority");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Frequency = objGlobaldata.GetDropdownList("Compliance Frequency");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCompliance: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(ObjMdl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCompliance(LegalRegisterModel objComp, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objComp.Isostd = form["Isostd"];
                objComp.branch = form["branch"];
                objComp.deptid = form["deptid"];
                objComp.Location = form["Location"];
                if (objComp.branch == null || objComp.branch == "")
                {
                    objComp.branch = objGlobaldata.GetCurrentUserSession().division;
                }
                DateTime dateValue;

                if (DateTime.TryParse(form["Eve_date"], out dateValue) == true)
                {
                    objComp.Eve_date = dateValue;
                }
                if (DateTime.TryParse(form["nexteval_date"], out dateValue) == true)
                {
                    objComp.nexteval_date = dateValue;
                }

                //notified_to
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objComp.notified_to = form["nempno " + i] + "," + objComp.notified_to;
                    }
                }
                if (objComp.notified_to != null)
                {
                    objComp.notified_to = objComp.notified_to.Trim(',');
                }

                HttpPostedFileBase files = Request.Files[0];
                if (upload != null && files.ContentLength > 0)
                {
                    objComp.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(file.FileName));
                            string sFilename = "Compliance" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objComp.upload = objComp.upload + "," + "~/DataUpload/MgmtDocs/LegalReg/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddCompliance-upload: " + ex.ToString());

                        }
                    }
                    objComp.upload = objComp.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objComp.FunAddCompliance(objComp))
                {
                    TempData["Successdata"] = "Added Compliance details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCompliance: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ComplianceList");
        }

        [AllowAnonymous]
        public ActionResult ComplianceList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Compliance";

            LegalRegisterModelsList objCom = new LegalRegisterModelsList();
            objCom.LegalRegisterMList = new List<LegalRegisterModel>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_law,lawNo,Isostd,lawTitle,deptid,compliance,upload,url," +
                    "Eve_date,Revision_Date,Revision_No,branch,Location,law_issue_authority,law_issued_by,law_relevant_to,frequency_eval,notified_to from t_compliance_obligation where Active=1";
            
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by lawNo";
                DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                {                   

                    for (int i = 0; i < dsComplList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            LegalRegisterModel objLegalModels = new LegalRegisterModel
                            {
                                id_law = (dsComplList.Tables[0].Rows[i]["id_law"].ToString()),
                                lawNo = dsComplList.Tables[0].Rows[i]["lawNo"].ToString(),
                                Isostd = objGlobaldata.GetIsoStdDescriptionById(dsComplList.Tables[0].Rows[i]["Isostd"].ToString()),
                                lawTitle = dsComplList.Tables[0].Rows[i]["lawTitle"].ToString(),
                                deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[i]["deptid"].ToString()),
                                compliance = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[i]["compliance"].ToString()),
                                url = dsComplList.Tables[0].Rows[i]["url"].ToString(),
                                Revision_No = dsComplList.Tables[0].Rows[i]["Revision_No"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[i]["Location"].ToString()),
                                law_issue_authority = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[i]["law_issue_authority"].ToString()),
                                law_issued_by = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[i]["law_issued_by"].ToString()),
                                law_relevant_to = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[i]["law_relevant_to"].ToString()),
                                frequency_eval = dsComplList.Tables[0].Rows[i]["frequency_eval"].ToString(),
                                notified_to = objGlobaldata.GetMultiHrEmpNameById(dsComplList.Tables[0].Rows[i]["notified_to"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsComplList.Tables[0].Rows[i]["Eve_date"].ToString() != ""
                               && DateTime.TryParse(dsComplList.Tables[0].Rows[i]["Eve_date"].ToString(), out dtDocDate))
                            {
                                objLegalModels.Eve_date = dtDocDate;
                            }

                            //if (dsComplList.Tables[0].Rows[i]["Revision_Date"].ToString() != ""
                            //   && DateTime.TryParse(dsComplList.Tables[0].Rows[i]["Revision_Date"].ToString(), out dtDocDate))
                            //{
                            //    objLegalModels.Revision_Date = dtDocDate;
                            //}

                            //if(objLegalModels.frequency_eval == "Quarterly")
                            //{
                            //    objLegalModels.Revision_Date = objLegalModels.Eve_date.AddMonths(3);
                            //}
                            //else if(objLegalModels.frequency_eval == "Semi Annually")
                            //{
                            //    objLegalModels.Revision_Date = objLegalModels.Eve_date.AddMonths(6);
                            //}
                            //if (objLegalModels.frequency_eval == "Yearly")
                            //{
                            //    objLegalModels.Revision_Date = objLegalModels.Eve_date.AddMonths(12);
                            //}
                            objCom.LegalRegisterMList.Add(objLegalModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ComplianceList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCom.LegalRegisterMList.ToList());
        }

       
        [AllowAnonymous]
        public ActionResult ComplianceEdit()
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();

            UserCredentials objUser = new UserCredentials();

            objUser = objGlobaldata.GetCurrentUserSession();

            //ViewBag.Role = objGlobaldata.GetRoleName(objUser.role);
            // ViewBag.CurrentEmpName = objUser.firstname;

            try
            {
                if (Request.QueryString["id_law"] != null && Request.QueryString["id_law"] != "")
                {
                    string sid_law = Request.QueryString["id_law"];
                    string sSqlstmt = "select id_law,lawNo,Isostd,lawTitle,deptid,compliance,upload," +
                        "url,Eve_date,Revision_date,Revision_No,requirement,description,nexteval_date,branch," +
                        "Location,law_issue_authority,law_issued_by,law_relevant_to,frequency_eval,notified_to,frequency_review from t_compliance_obligation"
                    + " where id_law='" + sid_law + "'";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {
                        objComp = new LegalRegisterModel
                        {
                            id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                            lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                            Isostd = (dsComplList.Tables[0].Rows[0]["Isostd"].ToString()),
                            lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            deptid = (dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                            upload = (dsComplList.Tables[0].Rows[0]["upload"].ToString()),
                            compliance = (dsComplList.Tables[0].Rows[0]["compliance"].ToString()),
                            url = dsComplList.Tables[0].Rows[0]["url"].ToString(),
                            Revision_No = dsComplList.Tables[0].Rows[0]["Revision_No"].ToString(),
                            requirement = dsComplList.Tables[0].Rows[0]["requirement"].ToString(),
                            description = dsComplList.Tables[0].Rows[0]["description"].ToString(),
                            branch = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                            Location =/* objGlobaldata.GetDivisionLocationById*/(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                            law_issue_authority =/* objGlobaldata.GetDropdownitemById*/(dsComplList.Tables[0].Rows[0]["law_issue_authority"].ToString()),
                            law_issued_by = /*objGlobaldata.GetDropdownitemById*/(dsComplList.Tables[0].Rows[0]["law_issued_by"].ToString()),
                            law_relevant_to = /*objGlobaldata.GetDropdownitemById*/(dsComplList.Tables[0].Rows[0]["law_relevant_to"].ToString()),
                            frequency_eval = dsComplList.Tables[0].Rows[0]["frequency_eval"].ToString(),
                            notified_to = /*objGlobaldata.GetMultiHrEmpNameById*/(dsComplList.Tables[0].Rows[0]["notified_to"].ToString()),
                            frequency_review = dsComplList.Tables[0].Rows[0]["frequency_review"].ToString(),
                        };
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsComplList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsComplList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Compliance = objGlobaldata.GetDropdownList("Compliance");
                        ViewBag.ISOStds = objGlobaldata.GetAllIsoStdListbox();                       
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.LawRelevantTo = objGlobaldata.GetDropdownList("Law Relevant To");
                        ViewBag.LawIssuedBy = objGlobaldata.GetDropdownList("Law Issued By");
                        ViewBag.LawIssueAuthority = objGlobaldata.GetDropdownList("Law Issue Authority");
                       // ViewBag.FrequencyEval = objGlobaldata.GetConstantValue("Issue Frequency Evaluation");
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.Frequency = objGlobaldata.GetDropdownList("Compliance Frequency");

                        DateTime dtDocDate;
                        if (dsComplList.Tables[0].Rows[0]["Eve_date"].ToString() != ""
                           && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_date"].ToString(), out dtDocDate))
                        {
                            objComp.Eve_date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["nexteval_date"].ToString() != ""
                         && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["nexteval_date"].ToString(), out dtDocDate))
                        {
                            objComp.nexteval_date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString() != ""
                          && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString(), out dtDocDate))
                        {
                            objComp.Revision_Date = dtDocDate;
                        }

                        if (dsComplList.Tables[0].Rows[0]["notified_to"].ToString() != "")
                        {
                            ViewBag.notified_Array = (dsComplList.Tables[0].Rows[0]["notified_to"].ToString()).Split(',');
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("ComplianceList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("ComplianceList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }
            return View(objComp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ComplianceEdit(LegalRegisterModel objComp, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objComp.Isostd = form["Isostd"];
                objComp.deptid = form["deptid"];
                objComp.Location = form["Location"];
                objComp.branch = form["branch"];
                if (objComp.branch == null || objComp.branch == "")
                {
                    objComp.branch = objGlobaldata.GetCurrentUserSession().division;
                }
                DateTime dateValue;

                if (DateTime.TryParse(form["Eve_date"], out dateValue) == true)
                {
                    objComp.Eve_date = dateValue;
                }

                if (DateTime.TryParse(form["Revision_Date"], out dateValue) == true)
                {
                    objComp.Revision_Date = dateValue;
                }

                //notified_to
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objComp.notified_to = form["nempno " + i] + "," + objComp.notified_to;
                    }
                }
                if (objComp.notified_to != null)
                {
                    objComp.notified_to = objComp.notified_to.Trim(',');
                }

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (upload != null && files.ContentLength > 0)
                {
                    objComp.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(file.FileName));
                            string sFilename = "Enquiry" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objComp.upload = objComp.upload + "," + "~/DataUpload/MgmtDocs/CRM/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in EnquiryEdit-upload: " + ex.ToString());

                        }
                    }
                    objComp.upload = objComp.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objComp.upload = objComp.upload + "," + form["QCDocsVal"];
                    objComp.upload = objComp.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objComp.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objComp.upload = null;
                }
                if (objComp.FunUpdateCompliance(objComp))
                {
                    TempData["Successdata"] = "Updated Compliance details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ComplianceList");
        }

        [AllowAnonymous]
        public ActionResult ComplianceDetails()
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();

            try
            {

                if (Request.QueryString["id_law"] != null && Request.QueryString["id_law"] != "")
                {
                    string sid_law = Request.QueryString["id_law"];
                    string sSqlstmt = "select id_law,lawNo,Isostd,lawTitle,deptid,compliance,upload,url,Eve_date," +
                        "Revision_Date,Revision_No,requirement,description,nexteval_date,branch," +
                        "Location,law_issue_authority,law_issued_by,law_relevant_to,frequency_eval,notified_to,frequency_review,ammend_date,ammend_detail,amend_review_date,amend_review_by,amend_review_detail,amend_actions,amend_notified_to from t_compliance_obligation"
                    + " where id_law='" + sid_law + "'";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {

                        objComp = new LegalRegisterModel
                        {
                            id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                            lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                            Isostd = objGlobaldata.GetIsoStdDescriptionById(dsComplList.Tables[0].Rows[0]["Isostd"].ToString()),
                            lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                            compliance = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["compliance"].ToString()),
                            upload = (dsComplList.Tables[0].Rows[0]["upload"].ToString()),
                            url = dsComplList.Tables[0].Rows[0]["url"].ToString(),
                            Revision_No = dsComplList.Tables[0].Rows[0]["Revision_No"].ToString(),
                            requirement = dsComplList.Tables[0].Rows[0]["requirement"].ToString(),
                            description = dsComplList.Tables[0].Rows[0]["description"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                            Location =objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                            law_issue_authority = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["law_issue_authority"].ToString()),
                            law_issued_by = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["law_issued_by"].ToString()),
                            law_relevant_to = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["law_relevant_to"].ToString()),
                            frequency_eval = dsComplList.Tables[0].Rows[0]["frequency_eval"].ToString(),
                            notified_to = objGlobaldata.GetMultiHrEmpNameById(dsComplList.Tables[0].Rows[0]["notified_to"].ToString()),
                            frequency_review =objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["frequency_review"].ToString()),

                            ammend_detail = dsComplList.Tables[0].Rows[0]["ammend_detail"].ToString(),
                            amend_review_by = objGlobaldata.GetMultiHrEmpNameById(dsComplList.Tables[0].Rows[0]["amend_review_by"].ToString()),
                            amend_review_detail = dsComplList.Tables[0].Rows[0]["amend_review_detail"].ToString(),
                            amend_actions = dsComplList.Tables[0].Rows[0]["amend_actions"].ToString(),
                            amend_notified_to = objGlobaldata.GetMultiHrEmpNameById(dsComplList.Tables[0].Rows[0]["amend_notified_to"].ToString()),
                        };

                        DateTime dtDocDate;
                        if (dsComplList.Tables[0].Rows[0]["Eve_date"].ToString() != ""
                           && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_date"].ToString(), out dtDocDate))
                        {
                            objComp.Eve_date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["nexteval_date"].ToString() != ""
                          && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["nexteval_date"].ToString(), out dtDocDate))
                        {
                            objComp.nexteval_date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString() != ""
                          && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString(), out dtDocDate))
                        {
                            objComp.Revision_Date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["ammend_date"].ToString() != ""
                          && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["ammend_date"].ToString(), out dtDocDate))
                        {
                            objComp.ammend_date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["amend_review_date"].ToString() != ""
                        && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["amend_review_date"].ToString(), out dtDocDate))
                        {
                            objComp.amend_review_date = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("ComplianceList");
                    }

                    LegalRegisterModelsList objList = new LegalRegisterModelsList();
                    objList.LegalRegisterMList = new List<LegalRegisterModel>();

                    string sSqlstmt1 = "select id_article,article_date,article_no,article_detail,compliance_status,article_desc," +
                    "action_taken,target_date,person_resp,article_upload,action_status,pending_reason,status_updatedon,article_recordno,frequency_eval,report_date from t_compliance_obligation_article"
                    + " where id_law='" + sid_law + "' and article_active=1";
                    DataSet dsarticlelList = objGlobaldata.Getdetails(sSqlstmt1);

                    if (dsarticlelList.Tables.Count > 0 && dsarticlelList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsarticlelList.Tables[0].Rows.Count; i++)
                        {
                            LegalRegisterModel objLeg = new LegalRegisterModel
                            {
                                id_article = dsarticlelList.Tables[0].Rows[i]["id_article"].ToString(),
                                article_no = dsarticlelList.Tables[0].Rows[i]["article_no"].ToString(),
                                article_detail = (dsarticlelList.Tables[0].Rows[i]["article_detail"].ToString()),
                                compliance_status = objGlobaldata.GetDropdownitemById(dsarticlelList.Tables[0].Rows[i]["compliance_status"].ToString()),
                                article_desc = dsarticlelList.Tables[0].Rows[i]["article_desc"].ToString(),
                                action_taken = (dsarticlelList.Tables[0].Rows[i]["action_taken"].ToString()),
                                person_resp = objGlobaldata.GetMultiHrEmpNameById(dsarticlelList.Tables[0].Rows[i]["person_resp"].ToString()),
                                article_upload = dsarticlelList.Tables[0].Rows[i]["article_upload"].ToString(),
                                action_status = objGlobaldata.GetDropdownitemById(dsarticlelList.Tables[0].Rows[i]["action_status"].ToString()),
                                pending_reason = dsarticlelList.Tables[0].Rows[i]["pending_reason"].ToString(),
                                article_recordno = dsarticlelList.Tables[0].Rows[i]["article_recordno"].ToString(),
                                frequency_eval = dsarticlelList.Tables[0].Rows[i]["frequency_eval"].ToString(),

                               
                            };

                            DateTime dtDocDate;
                            if (dsarticlelList.Tables[0].Rows[i]["article_date"].ToString() != ""
                               && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["article_date"].ToString(), out dtDocDate))
                            {
                                objLeg.article_date = dtDocDate;
                            }

                            if (dsarticlelList.Tables[0].Rows[i]["target_date"].ToString() != ""
                             && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["target_date"].ToString(), out dtDocDate))
                            {
                                objLeg.target_date = dtDocDate;
                            }
                            if (dsarticlelList.Tables[0].Rows[i]["report_date"].ToString() != ""
                            && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["report_date"].ToString(), out dtDocDate))
                            {
                                objLeg.report_date = dtDocDate;
                            }

                            if (dsarticlelList.Tables[0].Rows[i]["status_updatedon"].ToString() != ""
                            && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["status_updatedon"].ToString(), out dtDocDate))
                            {
                                objLeg.status_updatedon = dtDocDate;
                            }

                          
                            objList.LegalRegisterMList.Add(objLeg);
                        }
                    }

                    ViewBag.Law_article = objList;

                    //---------------------Ammendment--------------

                    //LegalRegisterModelsList objList1 = new LegalRegisterModelsList();
                    //objList1.LegalRegisterMList = new List<LegalRegisterModel>();

                    //string sSqlstmt11 = "select id_ammendment,ammend_date,ammend_detail from t_compliance_obligation_ammendment"
                    //+ " where id_law='" + sid_law + "' and ammend_active=1";

                    //DataSet dsammend = objGlobaldata.Getdetails(sSqlstmt11);

                    //if (dsammend.Tables.Count > 0 && dsammend.Tables[0].Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < dsarticlelList.Tables[0].Rows.Count; i++)
                    //    {
                    //        LegalRegisterModel objLeg1 = new LegalRegisterModel
                    //        {
                    //            id_ammendment = dsammend.Tables[0].Rows[i]["id_ammendment"].ToString(),
                    //            ammend_detail = dsammend.Tables[0].Rows[i]["ammend_detail"].ToString(),
                    //        };

                    //        DateTime dtDocDate;

                    //        if (dsammend.Tables[0].Rows[i]["ammend_date"].ToString() != ""
                    //           && DateTime.TryParse(dsammend.Tables[0].Rows[i]["ammend_date"].ToString(), out dtDocDate))
                    //        {
                    //            objLeg1.ammend_date = dtDocDate;
                    //        }

                    //        objList1.LegalRegisterMList.Add(objLeg1);
                    //    }
                    //}

                    //ViewBag.Ammendment = objList1;
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("ComplianceList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }
            return View(objComp);
        }

        [AllowAnonymous]
        public ActionResult CompliancePDF()
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();

            try
            {               
                if (Request.QueryString["id_law"] != null && Request.QueryString["id_law"] != "")
                {
                    //---------------start Main -------------
                    string sid_law = Request.QueryString["id_law"];
                    string sSqlstmt = "select id_law,lawNo,Isostd,lawTitle,deptid,compliance,upload,url,Eve_date," +
                        "Revision_Date,Revision_No,requirement,description,nexteval_date,branch," +
                        "Location,law_issue_authority,law_issued_by,law_relevant_to,frequency_eval,notified_to from t_compliance_obligation"
                    + " where id_law='" + sid_law + "'";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {

                        objComp = new LegalRegisterModel
                        {
                            id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                            lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                            Isostd = objGlobaldata.GetIsoStdDescriptionById(dsComplList.Tables[0].Rows[0]["Isostd"].ToString()),
                            lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                            compliance = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["compliance"].ToString()),
                            upload = (dsComplList.Tables[0].Rows[0]["upload"].ToString()),
                            url = dsComplList.Tables[0].Rows[0]["url"].ToString(),
                            Revision_No = dsComplList.Tables[0].Rows[0]["Revision_No"].ToString(),
                            requirement = dsComplList.Tables[0].Rows[0]["requirement"].ToString(),
                            description = dsComplList.Tables[0].Rows[0]["description"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                            law_issue_authority = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["law_issue_authority"].ToString()),
                            law_issued_by = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["law_issued_by"].ToString()),
                            law_relevant_to = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["law_relevant_to"].ToString()),
                            frequency_eval = dsComplList.Tables[0].Rows[0]["frequency_eval"].ToString(),
                            notified_to = objGlobaldata.GetMultiHrEmpNameById(dsComplList.Tables[0].Rows[0]["notified_to"].ToString()),

                        };

                        DateTime dtDocDate;
                        if (dsComplList.Tables[0].Rows[0]["Eve_date"].ToString() != ""
                           && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_date"].ToString(), out dtDocDate))
                        {
                            objComp.Eve_date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["nexteval_date"].ToString() != ""
                          && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["nexteval_date"].ToString(), out dtDocDate))
                        {
                            objComp.nexteval_date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString() != ""
                          && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString(), out dtDocDate))
                        {
                            objComp.Revision_Date = dtDocDate;
                        }                       
                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("ComplianceList");
                    }
                    ViewBag.ObjMainList = objComp;

                    CompanyModels objCompany = new CompanyModels();
                    dsComplList = objCompany.GetCompanyDetailsForReport(dsComplList);
                    dsComplList = objGlobaldata.GetReportDetails(dsComplList, "", objGlobaldata.GetCurrentUserSession().empid, "LEGAL REGISTER REPORT");

                    ViewBag.CompanyInfo = dsComplList;
                    //---------------End Main -------------

                    LegalRegisterModelsList objList = new LegalRegisterModelsList();
                    objList.LegalRegisterMList = new List<LegalRegisterModel>();

                    string sSqlstmt1 = "select id_article,article_date,article_no,article_detail,compliance_status,article_desc," +
                    "action_taken,target_date,person_resp,article_upload,action_status,pending_reason,status_updatedon,article_recordno,frequency_eval from t_compliance_obligation_article"
                    + " where id_law='" + sid_law + "' and article_active=1";
                    DataSet dsarticlelList = objGlobaldata.Getdetails(sSqlstmt1);

                    if (dsarticlelList.Tables.Count > 0 && dsarticlelList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsarticlelList.Tables[0].Rows.Count; i++)
                        {
                            LegalRegisterModel objLeg = new LegalRegisterModel
                            {
                                id_article = dsarticlelList.Tables[0].Rows[i]["id_article"].ToString(),
                                article_no = dsarticlelList.Tables[0].Rows[i]["article_no"].ToString(),
                                article_detail = (dsarticlelList.Tables[0].Rows[i]["article_detail"].ToString()),
                                compliance_status = objGlobaldata.GetDropdownitemById(dsarticlelList.Tables[0].Rows[i]["compliance_status"].ToString()),
                                article_desc = dsarticlelList.Tables[0].Rows[i]["article_desc"].ToString(),
                                action_taken = (dsarticlelList.Tables[0].Rows[i]["action_taken"].ToString()),
                                person_resp = objGlobaldata.GetMultiHrEmpNameById(dsarticlelList.Tables[0].Rows[i]["person_resp"].ToString()),
                                article_upload = dsarticlelList.Tables[0].Rows[i]["article_upload"].ToString(),
                                action_status = objGlobaldata.GetDropdownitemById(dsarticlelList.Tables[0].Rows[i]["action_status"].ToString()),
                                pending_reason = dsarticlelList.Tables[0].Rows[i]["pending_reason"].ToString(),

                                article_recordno = dsarticlelList.Tables[0].Rows[i]["article_recordno"].ToString(),
                                frequency_eval = dsarticlelList.Tables[0].Rows[i]["frequency_eval"].ToString(),
                            };

                            DateTime dtDocDate;
                            if (dsarticlelList.Tables[0].Rows[i]["article_date"].ToString() != ""
                               && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["article_date"].ToString(), out dtDocDate))
                            {
                                objLeg.article_date = dtDocDate;
                            }

                            if (dsarticlelList.Tables[0].Rows[i]["target_date"].ToString() != ""
                             && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["target_date"].ToString(), out dtDocDate))
                            {
                                objLeg.target_date = dtDocDate;
                            }

                            if (dsarticlelList.Tables[0].Rows[i]["status_updatedon"].ToString() != ""
                            && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["status_updatedon"].ToString(), out dtDocDate))
                            {
                                objLeg.status_updatedon = dtDocDate;
                            }
                            objList.LegalRegisterMList.Add(objLeg);
                        }
                    }

                    ViewBag.Law_article = objList;

                    //----------------

                    LegalRegisterModelsList objList1 = new LegalRegisterModelsList();
                    objList1.LegalRegisterMList = new List<LegalRegisterModel>();

                    string sSqlstmt11 = "select id_ammendment,ammend_date,ammend_detail from t_compliance_obligation_ammendment"
                    + " where id_law='" + sid_law + "' and ammend_active=1";

                    DataSet dsammend = objGlobaldata.Getdetails(sSqlstmt11);

                    if (dsammend.Tables.Count > 0 && dsammend.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsarticlelList.Tables[0].Rows.Count; i++)
                        {
                            LegalRegisterModel objLeg1 = new LegalRegisterModel
                            {
                                id_ammendment = dsammend.Tables[0].Rows[i]["id_ammendment"].ToString(),
                                ammend_detail = dsammend.Tables[0].Rows[i]["ammend_detail"].ToString(),
                            };

                            DateTime dtDocDate;

                            if (dsammend.Tables[0].Rows[i]["ammend_date"].ToString() != ""
                               && DateTime.TryParse(dsammend.Tables[0].Rows[i]["ammend_date"].ToString(), out dtDocDate))
                            {
                                objLeg1.ammend_date = dtDocDate;
                            }

                            objList1.LegalRegisterMList.Add(objLeg1);
                        }
                    }

                    ViewBag.Ammendment = objList1;
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("ComplianceList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CompliancePDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("CompliancePDF")
            {
                FileName = "CompliancePDF.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }


        [AllowAnonymous]
        public ActionResult ComplianceInfo(int id)
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();

            try
            {
                string sSqlstmt = "select id_law,lawNo,Isostd,lawTitle,deptid,compliance,upload,url,Eve_date," +
                    "Revision_Date,Revision_No,branch,Location,law_issue_authority,law_issued_by,law_relevant_to,frequency_eval,notified_to from t_compliance_obligation"
                + " where id_law='" + id + "'";

                DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);
                
                if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                {

                    objComp = new LegalRegisterModel
                    {
                        id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                        lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                        Isostd = objGlobaldata.GetIsoStdDescriptionById(dsComplList.Tables[0].Rows[0]["Isostd"].ToString()),
                        lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                        deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                        compliance = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["compliance"].ToString()),
                        upload = (dsComplList.Tables[0].Rows[0]["upload"].ToString()),
                        url = dsComplList.Tables[0].Rows[0]["url"].ToString(),
                        Revision_No = dsComplList.Tables[0].Rows[0]["Revision_No"].ToString(),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                        Location = objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                        law_issue_authority = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["law_issue_authority"].ToString()),
                        law_issued_by = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["law_issued_by"].ToString()),
                        law_relevant_to = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["law_relevant_to"].ToString()),
                        frequency_eval = dsComplList.Tables[0].Rows[0]["frequency_eval"].ToString(),
                        notified_to = objGlobaldata.GetMultiHrEmpNameById(dsComplList.Tables[0].Rows[0]["notified_to"].ToString()),

                    };
                    DateTime dtDocDate;
                    if (dsComplList.Tables[0].Rows[0]["Eve_date"].ToString() != ""
                       && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_date"].ToString(), out dtDocDate))
                    {
                        objComp.Eve_date = dtDocDate;
                    }
                    if (dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString() != ""
                       && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString(), out dtDocDate))
                    {
                        objComp.Revision_Date = dtDocDate;
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("ComplianceList");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }
            return View(objComp);
        }

        [AllowAnonymous]
        public JsonResult ComplianceDocDelete(FormCollection form)
        {
            try
            {
                  if (form["id_law"] != null && form["id_law"] != "")
                    {

                        LegalRegisterModel Doc = new LegalRegisterModel();
                        string sid_law = form["id_law"];
                    
                        if (Doc.FunDeleteComplianceDoc(sid_law))
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
                        TempData["alertdata"] = "Compliance Id cannot be Null or empty";
                        return Json("Failed");
                    }
               

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //----------------- Law - Articles-----------------
        [AllowAnonymous]
        public ActionResult Law_ArticleEdit()
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();
            try
            {
                string sid_law = Request.QueryString["id_law"];
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.FrequencyEval = objGlobaldata.GetConstantValue("Issue Frequency Evaluation");

                if (Request.QueryString["id_law"] != null && Request.QueryString["id_law"] != "")
                {
                    //t_compliance_obligation
                    string sSqlstmt = "select id_law,lawNo,lawTitle,deptid,Eve_date,branch,Location,article_notified_to from t_compliance_obligation"
                    + " where id_law='" + sid_law + "' and active=1";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {
                        objComp = new LegalRegisterModel
                        {
                            id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                            lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                            lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                            article_notified_to= dsComplList.Tables[0].Rows[0]["article_notified_to"].ToString()
                        };                       

                        DateTime dtDocDate;
                        if (dsComplList.Tables[0].Rows[0]["Eve_date"].ToString() != ""
                           && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_date"].ToString(), out dtDocDate))
                        {
                            objComp.Eve_date = dtDocDate;
                        }

                        if (dsComplList.Tables[0].Rows[0]["article_notified_to"].ToString() != "")
                        {
                            ViewBag.notified_Array = (dsComplList.Tables[0].Rows[0]["article_notified_to"].ToString()).Split(',');
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("ComplianceList");
                    }

                    //t_compliance_obligation_article

                    LegalRegisterModelsList objList = new LegalRegisterModelsList();
                    objList.LegalRegisterMList = new List<LegalRegisterModel>();

                    string sSqlstmt1 = "select id_article,article_date,article_no,article_detail,article_recordno,frequency_eval from t_compliance_obligation_article"
                    + " where id_law='" + sid_law + "' and article_active=1";
                    DataSet dsarticlelList = objGlobaldata.Getdetails(sSqlstmt1);

                    if (dsarticlelList.Tables.Count > 0 && dsarticlelList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsarticlelList.Tables[0].Rows.Count; i++)
                        {
                            LegalRegisterModel objLeg = new LegalRegisterModel
                            {
                                id_article = dsarticlelList.Tables[0].Rows[i]["id_article"].ToString(),
                                article_no = dsarticlelList.Tables[0].Rows[i]["article_no"].ToString(),
                                article_detail = (dsarticlelList.Tables[0].Rows[i]["article_detail"].ToString()),
                                article_recordno = dsarticlelList.Tables[0].Rows[i]["article_recordno"].ToString(),
                                frequency_eval = (dsarticlelList.Tables[0].Rows[i]["frequency_eval"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsarticlelList.Tables[0].Rows[i]["article_date"].ToString() != ""
                               && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["article_date"].ToString(), out dtDocDate))
                            {
                                objLeg.article_date = dtDocDate;
                            }

                            objList.LegalRegisterMList.Add(objLeg);
                        }                     
                    }

                    ViewBag.Law_article = objList;
                }
                    else
                    {
                       TempData["alertdata"] = "ID cannot be Null or empty";
                       return RedirectToAction("ComplianceList");
                    }                
            }  
 
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Law_ArticleEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }
            return View(objComp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Law_ArticleEdit(LegalRegisterModel objComp, FormCollection form)
        {
            try
            {
               
                //notified_to
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objComp.article_notified_to = form["nempno " + i] + "," + objComp.article_notified_to;
                    }
                }
                if (objComp.article_notified_to != null)
                {
                    objComp.article_notified_to = objComp.article_notified_to.Trim(',');
                }

                LegalRegisterModelsList objList = new LegalRegisterModelsList();
                objList.LegalRegisterMList = new List<LegalRegisterModel>();

                int iCnts = 0;
                DateTime dateValue1;
                if (form["itemcount"] != null && form["itemcount"] != "" && int.TryParse(form["itemcount"], out iCnts))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                    {
                        if (form["article_no " + i] != null || form["article_no " + i] != "")
                        {
                            LegalRegisterModel objLegal = new LegalRegisterModel
                            {
                                id_article = form["id_article " + i],
                                article_no = form["article_no " + i],
                                article_detail = form["article_detail " + i],
                                article_recordno = form["article_recordno " + i],
                                frequency_eval = form["frequency_eval " + i],
                            };
                            if (DateTime.TryParse(form["article_date " + i], out dateValue1) == true)
                            {
                                objLegal.article_date = dateValue1;
                            }
                            objList.LegalRegisterMList.Add(objLegal);
                        }
                    }
                }

                if (objComp.FunUpdateArticle(objComp,objList))
                {
                    TempData["Successdata"] = "Updated Law-Article details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Law_ArticleEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ComplianceList");
        }

        [AllowAnonymous]
        public ActionResult Law_ArticleDetail()
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();

            try
            {
                string sid_law = Request.QueryString["id_law"];
                if (Request.QueryString["id_law"] != null && Request.QueryString["id_law"] != "")
                {
                    //t_compliance_obligation
                    string sSqlstmt = "select id_law,lawNo,lawTitle,deptid,Eve_date,branch,Location,article_notified_to from t_compliance_obligation"
                    + " where id_law='" + sid_law + "' and active=1";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {
                        objComp = new LegalRegisterModel
                        {
                            id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                            lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                            lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                            article_notified_to = objGlobaldata.GetMultiHrEmpNameById(dsComplList.Tables[0].Rows[0]["article_notified_to"].ToString())
                        };

                        DateTime dtDocDate;
                        if (dsComplList.Tables[0].Rows[0]["Eve_date"].ToString() != ""
                           && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_date"].ToString(), out dtDocDate))
                        {
                            objComp.Eve_date = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("ComplianceList");
                    }

                    //t_compliance_obligation_article

                    LegalRegisterModelsList objList = new LegalRegisterModelsList();
                    objList.LegalRegisterMList = new List<LegalRegisterModel>();

                    string sSqlstmt1 = "select id_article,article_date,article_no,article_detail,article_recordno,frequency_eval from t_compliance_obligation_article"
                    + " where id_law='" + sid_law + "' and article_active=1";
                    DataSet dsarticlelList = objGlobaldata.Getdetails(sSqlstmt1);

                    if (dsarticlelList.Tables.Count > 0 && dsarticlelList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsarticlelList.Tables[0].Rows.Count; i++)
                        {
                            LegalRegisterModel objLeg = new LegalRegisterModel
                            {
                                id_article = dsarticlelList.Tables[0].Rows[i]["id_article"].ToString(),
                                article_no = dsarticlelList.Tables[0].Rows[i]["article_no"].ToString(),
                                article_detail = (dsarticlelList.Tables[0].Rows[i]["article_detail"].ToString()),
                                article_recordno = dsarticlelList.Tables[0].Rows[i]["article_recordno"].ToString(),
                                frequency_eval = (dsarticlelList.Tables[0].Rows[i]["frequency_eval"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsarticlelList.Tables[0].Rows[i]["article_date"].ToString() != ""
                               && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["article_date"].ToString(), out dtDocDate))
                            {
                                objLeg.article_date = dtDocDate;
                            }

                            objList.LegalRegisterMList.Add(objLeg);
                        }
                    }

                    ViewBag.Law_article = objList;
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("ComplianceList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Law_ArticleDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }
            return View(objComp);
        }

        [AllowAnonymous]
        public ActionResult ComplianceEvaluationEdit()
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();
            try
            {
                string sid_law = Request.QueryString["id_law"];
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.ComplianceStatus = objGlobaldata.GetDropdownList("Legal Register Compliance Status");

                if (Request.QueryString["id_law"] != null && Request.QueryString["id_law"] != "")
                {
                    //t_compliance_obligation
                    string sSqlstmt = "select id_law,lawNo,lawTitle,deptid,Eve_date,branch,Location,article_notified_to from t_compliance_obligation"
                    + " where id_law='" + sid_law + "' and active=1";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {
                        objComp = new LegalRegisterModel
                        {
                            id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                            lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                            lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                          //  article_notified_to = dsComplList.Tables[0].Rows[0]["article_notified_to"].ToString()
                        };                      

                        DateTime dtDocDate;
                        if (dsComplList.Tables[0].Rows[0]["Eve_date"].ToString() != ""
                           && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_date"].ToString(), out dtDocDate))
                        {
                            objComp.Eve_date = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("ComplianceList");
                    }

                    //t_compliance_obligation_article

                    LegalRegisterModelsList objList = new LegalRegisterModelsList();
                    objList.LegalRegisterMList = new List<LegalRegisterModel>();

                    string sSqlstmt1 = "select id_article,article_date,article_no,article_detail,compliance_status,article_desc," +
                    "action_taken,target_date,person_resp,article_upload,report_date from t_compliance_obligation_article"
                    + " where id_law='" + sid_law + "' and article_active=1";
                    DataSet dsarticlelList = objGlobaldata.Getdetails(sSqlstmt1);

                    if (dsarticlelList.Tables.Count > 0 && dsarticlelList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsarticlelList.Tables[0].Rows.Count; i++)
                        {
                            LegalRegisterModel objLeg = new LegalRegisterModel
                            {
                                id_article = dsarticlelList.Tables[0].Rows[i]["id_article"].ToString(),
                                article_no = dsarticlelList.Tables[0].Rows[i]["article_no"].ToString(),
                                article_detail = (dsarticlelList.Tables[0].Rows[i]["article_detail"].ToString()),
                                compliance_status = dsarticlelList.Tables[0].Rows[i]["compliance_status"].ToString(),
                                article_desc = dsarticlelList.Tables[0].Rows[i]["article_desc"].ToString(),
                                action_taken = (dsarticlelList.Tables[0].Rows[i]["action_taken"].ToString()),
                                person_resp = dsarticlelList.Tables[0].Rows[i]["person_resp"].ToString(),
                                article_upload = dsarticlelList.Tables[0].Rows[i]["article_upload"].ToString(),                              
                            };

                            DateTime dtDocDate;
                            if (dsarticlelList.Tables[0].Rows[i]["article_date"].ToString() != ""
                               && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["article_date"].ToString(), out dtDocDate))
                            {
                                objLeg.article_date = dtDocDate;
                            }

                            if (dsarticlelList.Tables[0].Rows[i]["target_date"].ToString() != ""
                             && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["target_date"].ToString(), out dtDocDate))
                            {
                                objLeg.target_date = dtDocDate;
                            }
                            if (dsarticlelList.Tables[0].Rows[i]["report_date"].ToString() != ""
                             && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["report_date"].ToString(), out dtDocDate))
                            {
                                objLeg.report_date = dtDocDate;
                            }
                            objList.LegalRegisterMList.Add(objLeg);
                        }
                    }

                    ViewBag.Law_article = objList;
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("ComplianceList");
                }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }
            return View(objComp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ComplianceEvaluationEdit(FormCollection form)
        {
            try
            {
                LegalRegisterModel objComp = new LegalRegisterModel();               
               

                LegalRegisterModelsList objList = new LegalRegisterModelsList();
                objList.LegalRegisterMList = new List<LegalRegisterModel>();

                int iCnts = 0;
                DateTime dateValue1;
                if (form["itemcount"] != null && form["itemcount"] != "" && int.TryParse(form["itemcount"], out iCnts))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                    {
                        if (form["article_no " + i] != null || form["article_no " + i] != "")
                        {
                            LegalRegisterModel objLegal = new LegalRegisterModel
                            {
                                id_article = form["id_article " + i],
                                article_no = form["article_no " + i],
                                article_detail = form["article_detail " + i],
                                compliance_status = form["compliance_status " + i],
                                article_desc = form["article_desc " + i],
                                action_taken = form["action_taken " + i],
                                person_resp = form["person_resp " + i]
                            };
                            if (form["article_upload" + i] != null)
                            {
                                objLegal.article_upload = form["article_upload" + i];
                            }
                            if (DateTime.TryParse(form["article_date " + i], out dateValue1) == true)
                            {
                                objLegal.article_date = dateValue1;
                            }
                            if (DateTime.TryParse(form["target_date " + i], out dateValue1) == true)
                            {
                                objLegal.target_date = dateValue1;
                            }
                            if (DateTime.TryParse(form["report_date " + i], out dateValue1) == true)
                            {
                                objLegal.report_date = dateValue1;
                            }
                            objList.LegalRegisterMList.Add(objLegal);
                        }
                    }
                }

                if (objComp.FunUpdateComplianceEvaluation(objList))
                {
                    TempData["Successdata"] = "Updated Compliance Evaluation successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ComplianceList");
        }

        [AllowAnonymous]
        public ActionResult ComplianceStatusEdit()
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();
            try
            {
                string sid_law = Request.QueryString["id_law"];
                ViewBag.ActionStatus = objGlobaldata.GetDropdownList("Legal Register Action Status");

                if (Request.QueryString["id_law"] != null && Request.QueryString["id_law"] != "")
                {
                    //t_compliance_obligation
                    string sSqlstmt = "select id_law,lawNo,lawTitle,deptid,Eve_date,branch,Location,article_notified_to from t_compliance_obligation"
                    + " where id_law='" + sid_law + "' and active=1";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {
                        objComp = new LegalRegisterModel
                        {
                            id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                            lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                            lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                            //  article_notified_to = dsComplList.Tables[0].Rows[0]["article_notified_to"].ToString()
                        };

                        DateTime dtDocDate;
                        if (dsComplList.Tables[0].Rows[0]["Eve_date"].ToString() != ""
                           && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_date"].ToString(), out dtDocDate))
                        {
                            objComp.Eve_date = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("ComplianceList");
                    }

                    //t_compliance_obligation_article

                    LegalRegisterModelsList objList = new LegalRegisterModelsList();
                    objList.LegalRegisterMList = new List<LegalRegisterModel>();

                    string Compliance_Status_notcomplied = objGlobaldata.GetComplianceStausIdByName("Not complied");
                    string sSqlstmt1 = "select id_article,article_date,article_no,article_detail,action_status,pending_reason,status_updatedon from t_compliance_obligation_article"
                    + " where id_law='" + sid_law + "' and article_active=1 and compliance_status ='"+ Compliance_Status_notcomplied + "'";

                    DataSet dsarticlelList = objGlobaldata.Getdetails(sSqlstmt1);

                    if (dsarticlelList.Tables.Count > 0 && dsarticlelList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsarticlelList.Tables[0].Rows.Count; i++)
                        {
                            LegalRegisterModel objLeg = new LegalRegisterModel
                            {
                                id_article = dsarticlelList.Tables[0].Rows[i]["id_article"].ToString(),
                                article_no = dsarticlelList.Tables[0].Rows[i]["article_no"].ToString(),
                                article_detail = (dsarticlelList.Tables[0].Rows[i]["article_detail"].ToString()),
                                action_status = dsarticlelList.Tables[0].Rows[i]["action_status"].ToString(),
                                pending_reason = dsarticlelList.Tables[0].Rows[i]["pending_reason"].ToString(),
                            };

                            DateTime dtDocDate;

                            if (dsarticlelList.Tables[0].Rows[i]["article_date"].ToString() != ""
                               && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["article_date"].ToString(), out dtDocDate))
                            {
                                objLeg.article_date = dtDocDate;
                            }

                            if (dsarticlelList.Tables[0].Rows[i]["status_updatedon"].ToString() != ""
                             && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["status_updatedon"].ToString(), out dtDocDate))
                            {
                                objLeg.status_updatedon = dtDocDate;
                            }

                            objList.LegalRegisterMList.Add(objLeg);
                        }
                    }

                    ViewBag.Law_article = objList;
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("ComplianceList");
                }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceStatusEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }
            return View(objComp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ComplianceStatusEdit(FormCollection form)
        {
            try
            {
                LegalRegisterModel objComp = new LegalRegisterModel();


                LegalRegisterModelsList objList = new LegalRegisterModelsList();
                objList.LegalRegisterMList = new List<LegalRegisterModel>();

                int iCnts = 0;
                DateTime dateValue1;
                if (form["itemcount"] != null && form["itemcount"] != "" && int.TryParse(form["itemcount"], out iCnts))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                    {
                        if (form["article_no " + i] != null || form["article_no " + i] != "")
                        {
                            LegalRegisterModel objLegal = new LegalRegisterModel
                            {
                                id_article = form["id_article " + i],
                                //article_no = form["article_no " + i],
                                //article_detail = form["article_detail " + i],
                                action_status = form["action_status " + i],                               
                                pending_reason = form["pending_reason " + i]
                            };
                           
                            if (DateTime.TryParse(form["status_updatedon " + i], out dateValue1) == true)
                            {
                                objLegal.status_updatedon = dateValue1;
                            }

                            objList.LegalRegisterMList.Add(objLegal);
                        }
                    }
                }

                if (objComp.FunUpdateComplianceStatus(objList))
                {
                    TempData["Successdata"] = "Updated Compliance Evaluation successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceStatusEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ComplianceList");
        }


        [AllowAnonymous]
        public JsonResult ComplianceArticleDelete(FormCollection form)
        {
            try
            {

                if (form["id_article"] != null && form["id_article"] != "")
                {
                    LegalRegisterModel Doc = new LegalRegisterModel();
                    string sid_article = form["id_article"];


                    if (Doc.FunDeleteComplianceArticle(sid_article))
                    {
                        TempData["Successdata"] = "Compliance article deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in ComplianceArticleDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //------------ End Law - Articles ------------------

        //------------------Start Ammendment-----------------------------
        [AllowAnonymous]
        public ActionResult AmmendmentEdit()
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();
            try
            {
                string sid_law = Request.QueryString["id_law"];
                if (Request.QueryString["id_law"] != null && Request.QueryString["id_law"] != "")
                {
                    //t_compliance_obligation
                    string sSqlstmt = "select id_law,lawNo,lawTitle,deptid,Eve_date,branch,Location,article_notified_to,ammend_date,ammend_detail,amend_review_date,amend_review_by,amend_review_detail,amend_actions,amend_notified_to from t_compliance_obligation"
                    + " where id_law='" + sid_law + "' and active=1";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {
                        objComp = new LegalRegisterModel
                        {
                            id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                            lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                            lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                            //  article_notified_to = dsComplList.Tables[0].Rows[0]["article_notified_to"].ToString()
                            ammend_detail = dsComplList.Tables[0].Rows[0]["ammend_detail"].ToString(),
                            amend_review_by = dsComplList.Tables[0].Rows[0]["amend_review_by"].ToString(),
                            amend_review_detail = dsComplList.Tables[0].Rows[0]["amend_review_detail"].ToString(),
                            amend_actions = dsComplList.Tables[0].Rows[0]["amend_actions"].ToString(),
                            amend_notified_to = dsComplList.Tables[0].Rows[0]["amend_notified_to"].ToString(),

                        };
                        if (dsComplList.Tables[0].Rows[0]["amend_notified_to"].ToString() != "")
                        {
                            ViewBag.NotifiedToArray = (dsComplList.Tables[0].Rows[0]["amend_notified_to"].ToString()).Split(',');
                        }
                        if (objComp.amend_review_by == "")
                        {
                            objComp.amend_review_by = objGlobaldata.GetCurrentUserSession().empid;
                        }
                        DateTime dtDocDate;
                        if (dsComplList.Tables[0].Rows[0]["Eve_date"].ToString() != ""
                           && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_date"].ToString(), out dtDocDate))
                        {
                            objComp.Eve_date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["ammend_date"].ToString() != ""
                          && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["ammend_date"].ToString(), out dtDocDate))
                        {
                            objComp.ammend_date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["amend_review_date"].ToString() != ""
                          && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["amend_review_date"].ToString(), out dtDocDate))
                        {
                            objComp.amend_review_date = dtDocDate;
                        }
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("ComplianceList");
                    }

                   

                    //LegalRegisterModelsList objList = new LegalRegisterModelsList();
                    //objList.LegalRegisterMList = new List<LegalRegisterModel>();

                    //string sSqlstmt1 = "select id_ammendment,ammend_date,ammend_detail from t_compliance_obligation_ammendment"
                    //+ " where id_law='" + sid_law + "' and ammend_active=1";

                    //DataSet dsarticlelList = objGlobaldata.Getdetails(sSqlstmt1);

                    //if (dsarticlelList.Tables.Count > 0 && dsarticlelList.Tables[0].Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < dsarticlelList.Tables[0].Rows.Count; i++)
                    //    {
                    //        LegalRegisterModel objLeg = new LegalRegisterModel
                    //        {
                    //            id_ammendment = dsarticlelList.Tables[0].Rows[i]["id_ammendment"].ToString(),
                    //            ammend_detail = dsarticlelList.Tables[0].Rows[i]["ammend_detail"].ToString(),
                    //        };

                    //        DateTime dtDocDate;

                    //        if (dsarticlelList.Tables[0].Rows[i]["ammend_date"].ToString() != ""
                    //           && DateTime.TryParse(dsarticlelList.Tables[0].Rows[i]["ammend_date"].ToString(), out dtDocDate))
                    //        {
                    //            objLeg.ammend_date = dtDocDate;
                    //        }                           

                    //        objList.LegalRegisterMList.Add(objLeg);
                    //    }
                    //}

                    //ViewBag.Ammendment = objList;
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("ComplianceList");
                }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AmmendmentEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }
            return View(objComp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AmmendmentEdit(FormCollection form, LegalRegisterModel objComp)
        {
            try
            {
                objComp.amend_review_by = form["amend_review_by"];
                
                DateTime dateValue;
                if (DateTime.TryParse(form["ammend_date"], out dateValue) == true)
                {
                    objComp.ammend_date = dateValue;
                }
                if (DateTime.TryParse(form["amend_review_date"], out dateValue) == true)
                {
                    objComp.amend_review_date = dateValue;
                }

                //notified to
                for (int i = 0; i < Convert.ToInt16(form["notified_cnt"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objComp.amend_notified_to = objComp.amend_notified_to + "," + form["empno " + i];
                    }
                }
                if (objComp.amend_notified_to != null)
                {
                    objComp.amend_notified_to = objComp.amend_notified_to.Trim(',');
                }

                //LegalRegisterModel objComp = new LegalRegisterModel();
                //objComp.id_law = form["id_law"];

                //LegalRegisterModelsList objList = new LegalRegisterModelsList();
                //objList.LegalRegisterMList = new List<LegalRegisterModel>();

                //int iCnts = 0;
                //DateTime dateValue1;
                //if (form["itemcount"] != null && form["itemcount"] != "" && int.TryParse(form["itemcount"], out iCnts))
                //{
                //    for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                //    {
                //        if (form["ammend_detail " + i] != null || form["ammend_detail " + i] != "")
                //        {
                //            LegalRegisterModel objLegal = new LegalRegisterModel
                //            {
                //                id_ammendment = form["id_ammendment " + i],
                //                ammend_detail = form["ammend_detail " + i]
                //            };

                //            if (DateTime.TryParse(form["ammend_date " + i], out dateValue1) == true)
                //            {
                //                objLegal.ammend_date = dateValue1;
                //            }

                //            objList.LegalRegisterMList.Add(objLegal);
                //        }
                //    }
                //}

                if (objComp.FunUpdateAmmendment(objComp))
                {
                    TempData["Successdata"] = "Updated Ammendment successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AmmendmentEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ComplianceList");
        }



        [AllowAnonymous]
        public JsonResult ComplianceAmmendmentDelete(FormCollection form)
        {
            try
            {

                if (form["id_ammendment"] != null && form["id_ammendment"] != "")
                {
                    LegalRegisterModel Doc = new LegalRegisterModel();
                    string sid_ammendment = form["id_ammendment"];


                    if (Doc.FunDeleteComplianceAmmendment(sid_ammendment))
                    {
                        TempData["Successdata"] = "Compliance Ammendment deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in ComplianceAmmendmentDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        //----------------End Ammendment-------------------------------

        // GET: /LegalRegister/AddLegalRegister

        [AllowAnonymous]
        public ActionResult AddLegalRegister()
        {
            return InitilizeAddLegalRegister();
        }

        // GET: /LegalRegister/AddLegalRegister

        private ActionResult InitilizeAddLegalRegister()
        {
            try
            {

                ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.frequency_of_evaluation = objGlobaldata.GetConstantValue("frequency_of_evaluation");
                ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                ViewBag.applicable = objGlobaldata.GetConstantValue("applicable");
                ViewBag.activeStatus = objGlobaldata.GetConstantValue("activeStatus");
                ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                ViewBag.deptlist = objGlobaldata.GetDepartmentList();
                ViewBag.Status_Obj_Eval = objGlobaldata.GetConstantValue("Status_Obj_Eval");
                ViewBag.DeptHead = objGlobaldata.GetDeptHeadList("");
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Empposition = objGlobaldata.GetEmpDesignationByHrEmpId("");
                ViewBag.ApproverList = objGlobaldata.GetApprover();
                ViewBag.ApproverList = objGlobaldata.GetApprover();
                ViewBag.ReviewerList = objGlobaldata.GetReviewer();//role=1+active+internal
                ViewBag.applicable = objGlobaldata.GetConstantValue("applicable");

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddLegalRegister: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        //Returns only Department Head list List
        // GET: /LegalRegister/FunAttendeesList

        public ActionResult FunGetDeptEmpList(string DeptId)
        {
            MultiSelectList lstEmp = objGlobaldata.GetHrEmpListByDept(DeptId);
            return Json(lstEmp);
        }

        //Returns only Objective reference List
        // GET: /LegalRegister/GetLegalRegisterRefList

        public ActionResult GetLegalRegisterRefList(string DeptId)
        {
            LegalRegisterModel objLegalRegister = new LegalRegisterModel();
            List<string> lstObjectivesList = objLegalRegister.GetLegalRegisterRef(DeptId);
            return Json(lstObjectivesList);
        }

        [AllowAnonymous]
        public JsonResult LegalRegisterDocDelete(FormCollection form)
        {
            try
            {
             
                    if (form["LegalRequirement_Id"] != null && form["LegalRequirement_Id"] != "")
                    {
                        LegalRegisterModel Doc = new LegalRegisterModel();
                        string sLegalRequirement_Id = form["LegalRequirement_Id"];


                        if (Doc.FunDeleteLegalRequirementsDoc(sLegalRequirement_Id))
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
                        TempData["alertdata"] = "Legal Register Id cannot be Null or empty";
                        return Json("Failed");
                    }
                

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult doesObjRefExist(string lawNo)
        {
            LegalRegisterModel objLegalRegister = new LegalRegisterModel();
            var Objective = objLegalRegister.checkObjRefExists(lawNo);

            return Json(Objective);
        }

        //Returns only Department Head list List
        // GET: /LegalRegister/GetLegalRequirementsDetails

        public ActionResult FunGetLegalRequirementsDetails(string LegalRequirement_Id)
        {
            try
            {
                LegalRegisterModel objLegalRegister = new LegalRegisterModel();

                DataSet dsObjectives = objLegalRegister.GetLegalRequirementsDetails(LegalRequirement_Id);

                if (dsObjectives.Tables.Count > 0 && dsObjectives.Tables[0].Rows.Count > 0)
                {
                    return Json(new
                    {
                        lawNo = dsObjectives.Tables[0].Rows[0]["lawNo"].ToString(),
                        lawTitle = dsObjectives.Tables[0].Rows[0]["lawTitle"].ToString(),
                        origin_of_requirement = dsObjectives.Tables[0].Rows[0]["origin_of_requirement"].ToString(),
                        document_storage_location = (dsObjectives.Tables[0].Rows[0]["document_storage_location"].ToString()),
                        frequency_of_evaluation = (dsObjectives.Tables[0].Rows[0]["frequency_of_evaluation"].ToString()),
                        activeStatus = (dsObjectives.Tables[0].Rows[0]["activeStatus"].ToString()),
                        updatedOn = objGlobaldata.GetCurrentUserSession().ToString(),
                        updatedByName = dsObjectives.Tables[0].Rows[0]["updatedByName"].ToString(),
                        reviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectives.Tables[0].Rows[0]["reviewedBy"].ToString()),
                        approvedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectives.Tables[0].Rows[0]["approvedBy"].ToString()),
                        reviewstatus = (dsObjectives.Tables[0].Rows[0]["reviewstatus"].ToString()),
                        approvestatus = (dsObjectives.Tables[0].Rows[0]["approvestatus"].ToString())
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetLegalRequirementsDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }


        // POST: /LegalRegister/AddLegalRegister

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddLegalRegister(LegalRegisterModel objLegalRegister, FormCollection form, HttpPostedFileBase upload)
        {
            TempData["Successdata"] = "Added Objective details suhail";
            try
            {
                if (objLegalRegister != null)
                {

                    objLegalRegister.lawNo = form["lawNo"];
                    objLegalRegister.lawTitle = form["lawTitle"];
                    objLegalRegister.origin_of_requirement = form["origin_of_requirement"];
                    objLegalRegister.document_storage_location = form["document_storage_location"];
                    objLegalRegister.frequency_of_evaluation = form["frequency_of_evaluation"];
                    objLegalRegister.activeStatus = form["activeStatus"];
                    objLegalRegister.reviewedBy = form["reviewedBy"];
                    objLegalRegister.updatedByName = form["updatedByName"];
                    objLegalRegister.approvedBy = form["approvedBy"];

                    if (upload != null && upload.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(upload.FileName));
                            string sFilename = "LegalRegister" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            upload.SaveAs(sFilepath + "/" + sFilename);
                            objLegalRegister.upload = "~/DataUpload/MgmtDocs/LegalReg/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddLegalRegister-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    DateTime dateValue;

                    if (DateTime.TryParse(form["initialdevelopmentdate"], out dateValue) == true)
                    {
                        objLegalRegister.initialdevelopmentdate = dateValue;
                    }
                    if (DateTime.TryParse(form["updatedOn"], out dateValue) == true)
                    {
                        objLegalRegister.updatedOn = dateValue;
                    }


                    LegalRegisterModelsList objObjectivesModelsList = new LegalRegisterModelsList();
                    objObjectivesModelsList.LegalRegisterMList = new List<LegalRegisterModel>();

                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        LegalRegisterModel objObjectivesModels = new LegalRegisterModel();

                        DateTime dateValues;

                        if (DateTime.TryParse(form["updatedDate" + i], out dateValue) == true)
                        {
                            objObjectivesModels.updatedDate = dateValue;
                        }

                        objObjectivesModels.article = form["article" + i];
                        objObjectivesModels.requirements = form["requirements" + i];
                        objObjectivesModels.applicable = form["applicable" + i];
                        objObjectivesModels.nonapplicablejustify = form["nonapplicablejustify" + i];
                        // objObjectivesModels.updatedBynametrans = form["updatedBynametrans" + i];
                        objObjectivesModels.updatedByposition = form["updatedByposition" + i];
                        //  objObjectivesModels.updatedByDept = form["updatedByDept" + i];
                        objObjectivesModels.updatedBynametrans = form["updatedBynametrans" + i];
                        objObjectivesModels.updatedByDept = form["updatedByDept" + i];
                        //objObjectivesModels.updatedBynametrans = objGlobaldata.GetCurrentUserSession().empid;
                        //objObjectivesModels.updatedByDept = objGlobaldata.GetDeptNameById("");
                        objObjectivesModels.reference = form["reference" + i];
                        objObjectivesModels.monitoring = form["monitoring" + i];
                        objObjectivesModelsList.LegalRegisterMList.Add(objObjectivesModels);
                    }

                    if (objLegalRegister.FunAddLegalRegister(objLegalRegister, objObjectivesModelsList))
                    {
                        TempData["Successdata"] = "Added Legal Register details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLegalRegister: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("LegalRegisterList");
        }


        [HttpPost]
        public JsonResult doesLawNumberExist(string lawNo)
        {
            var Valid = true;
            if (lawNo != null)
            {
                LegalRegisterModel objLegalRegisterModel = new LegalRegisterModel();
                Valid = objLegalRegisterModel.checkLawNumberExists(lawNo);
            }

            return Json(Valid);
        }

        public bool checkLawNumberExists(string lawNo)
        {
            string sSqlstmt = "select LegalRequirement_Id from t_legalregister where lawNo='" + lawNo + "'";
            DataSet dsEmp = objGlobaldata.Getdetails(sSqlstmt);
            if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
            {
                return false;
            }
            return true;
        }



        // GET: /LegalRegister/LegalRegisterApprove

        [AllowAnonymous]
        public ActionResult LegalRegisterApprove(string approvedBy, string PendingFlg)
        {
            try
            {
                LegalRegisterModel objLegalRegister = new LegalRegisterModel();
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().empid;

                if (objLegalRegister.FunUpdateLegalRegisterApproval(user, approvedBy))
                {
                    TempData["Successdata"] = "LegalRegister Apporved successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                //  }
                //   else
                //  {
                //        TempData["alertdata"] = "Access Denied";
                //  }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("LegalRegisterList");
            }
        }




     

        //========================   reviewed===================


        [AllowAnonymous]
        public ActionResult LegalRegisterReviewed(string LegalRequirement_Id, string PendingFlg, string lawNo)
        {
            try
            {
                LegalRegisterModel objLegalRegister = new LegalRegisterModel();
                //string filename = Path.GetFileName(Document);
                //FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                //if (objGlobaldata.GetRoleName(objGlobaldata.GetCurrentUserSession().role) == "Reviewer")
                //{
                if (objLegalRegister.FunUpdateLegalRegisterReview(LegalRequirement_Id, lawNo))
                {
                    TempData["Successdata"] = "Document Reviewed successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                //}
                //else
                //{
                //    TempData["alertdata"] = "Access Denied";
                //}

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterReviewed: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForReview", "Dashboard");
            }
            else
            {
                return RedirectToAction("LegalRegisterList");
            }
        }



        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase Action_Plan1 = Request.Files;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "ActionPlan" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/LegalReg/" + "ActionPlan" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);


            }

            return Json("Failed");
        }



        // GET: /LegalRegister/LegalRegisterList

        [AllowAnonymous]
        public ActionResult LegalRegisterList(string SearchText, string frequency_of_evaluation, string approvestatus, int? page)
        {
            // string sid_requirements = Request.QueryString["id_requirements"];
            LegalRegisterModelsList objObjectivesModelsList = new LegalRegisterModelsList();
            objObjectivesModelsList.LegalRegisterMList = new List<LegalRegisterModel>();

            try
            {
                ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                //ViewBag.id_requirements = sid_requirements;
                ViewBag.frequency_of_evaluation = objGlobaldata.GetConstantValue("frequency_of_evaluation");
                ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                ViewBag.activeStatus = objGlobaldata.GetConstantValue("activeStatus");
                ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select LegalRequirement_Id, lawNo, lawTitle,initialdevelopmentdate, origin_of_requirement,document_storage_location, frequency_of_evaluation,"
                + " activeStatus,updatedOn, updatedByName,approvedBy,reviewedBy,"
                    + " (case when approvestatus='1' then 'Approved' else 'Not Approved' end) as approvestatus ,(case when reviewstatus='1' then 'Reviewed' else 'Not Reviewed' end) as reviewstatus from t_legalregister ";//where activeStatus=1
                string sSearchtext = "";



                UserCredentials objUser = new UserCredentials();

                objUser = objGlobaldata.GetCurrentUserSession();

                string sRolename = objGlobaldata.GetRoleName(objUser.role);
                ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                ViewBag.Role = sRolename;
                ViewBag.CurrentEmpName = objUser.firstname;
                ViewBag.approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");
                ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");

                if (sRolename.ToLower() == "Preparer".ToLower())
                {
                    sSqlstmt = sSqlstmt + " where approvestatus=1 or updatedByName='" + objUser.empid + "'";
                }

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = " (lawNo ='" + SearchText + "' or lawNo like '" + SearchText + "%' or Dept in (select DeptId from t_departments where "
                        + "approvedBy='" + SearchText + "' or approvedBy like '" + SearchText + "%'))";
                }

                if (sRolename.ToLower() != "Preparer".ToLower() && sSearchtext != "")
                {
                    sSearchtext = " where " + sSearchtext;
                }
                else if (sRolename.ToLower() == "Preparer".ToLower() && sSearchtext != "")
                {
                    sSearchtext = " and " + sSearchtext;
                }

                if (approvestatus != null && approvestatus != "All" && approvestatus != "")
                {
                    ViewBag.ApprovestatusVal = approvestatus;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (approvestatus ='" + approvestatus + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " where (approvestatus ='" + approvestatus + "')";
                    }
                }


                if (frequency_of_evaluation != null && frequency_of_evaluation != "Select")
                {
                    ViewBag.frequency_of_evaluationText = frequency_of_evaluation;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (frequency_of_evaluation ='" + frequency_of_evaluation + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " where (frequency_of_evaluation ='" + frequency_of_evaluation + "')";
                    }
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by lawNo asc";

                DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                {

                     ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        
                        ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                   

                    for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            LegalRegisterModel objObjectivesModels = new LegalRegisterModel
                            {
                                LegalRequirement_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["LegalRequirement_Id"].ToString()),
                                lawNo = dsObjectivesModelsList.Tables[0].Rows[i]["lawNo"].ToString(),
                                lawTitle = dsObjectivesModelsList.Tables[0].Rows[i]["lawTitle"].ToString(),
                                origin_of_requirement = dsObjectivesModelsList.Tables[0].Rows[i]["origin_of_requirement"].ToString(),
                                document_storage_location = dsObjectivesModelsList.Tables[0].Rows[i]["document_storage_location"].ToString(),
                                frequency_of_evaluation = dsObjectivesModelsList.Tables[0].Rows[i]["frequency_of_evaluation"].ToString(),
                                activeStatus = dsObjectivesModelsList.Tables[0].Rows[i]["activeStatus"].ToString(),
                                updatedByName = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[i]["updatedByName"].ToString()),
                                /*here*/
                                reviewedBy = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[i]["reviewedBy"].ToString()),
                                /*here*/
                                approvedBy = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[i]["approvedBy"].ToString()),
                                reviewstatus = dsObjectivesModelsList.Tables[0].Rows[i]["reviewstatus"].ToString(),
                                approvestatus = dsObjectivesModelsList.Tables[0].Rows[i]["approvestatus"].ToString()
                            };
                            DateTime dtDocDate = new DateTime();
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString() != ""
                                && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.initialdevelopmentdate = dtDocDate;
                            }
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString() != ""
                               && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.updatedOn = dtDocDate;
                            }
                            objObjectivesModelsList.LegalRegisterMList.Add(objObjectivesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in LegalRegisterList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objObjectivesModelsList.LegalRegisterMList.ToList().ToPagedList(page ?? 1, 40));
        }

        // GET: /LegalRegister/LegalRegisterDetails

        [AllowAnonymous]
        public ActionResult LegalRegisterDetails()
        {
            LegalRegisterModel objObjectivesModels = new LegalRegisterModel();
            try
            {
                if (Request.QueryString["LegalRequirement_Id"] != null && Request.QueryString["LegalRequirement_Id"] != "")
                {
                    string sLegalRequirement_Id = Request.QueryString["LegalRequirement_Id"];

                    ViewBag.UserRole = objGlobaldata.GetRoleName(objGlobaldata.GetCurrentUserSession().role);

                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    string sSqlstmt = "select LegalRequirement_Id, lawNo,lawTitle, initialdevelopmentdate, origin_of_requirement,document_storage_location ,frequency_of_evaluation ,activeStatus, updatedByName,updatedOn,reviewedBy, approvedBy,"
                        + " (case when approvestatus='1' then 'Approved' else 'Not Approved' end) as approvestatus,(case when reviewstatus='1' then 'Reviewed' else 'Not Reviewed' end) as reviewstatus, updatedByName,upload"
                        + "   from t_legalregister where LegalRequirement_Id='" + sLegalRequirement_Id + "'";


                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {


                        objObjectivesModels = new LegalRegisterModel
                        {
                            LegalRequirement_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["LegalRequirement_Id"].ToString()),
                            lawNo = dsObjectivesModelsList.Tables[0].Rows[0]["lawNo"].ToString(),
                            lawTitle = dsObjectivesModelsList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            origin_of_requirement = dsObjectivesModelsList.Tables[0].Rows[0]["origin_of_requirement"].ToString(),
                            document_storage_location = dsObjectivesModelsList.Tables[0].Rows[0]["document_storage_location"].ToString(),
                            frequency_of_evaluation = dsObjectivesModelsList.Tables[0].Rows[0]["frequency_of_evaluation"].ToString(),
                            activeStatus = dsObjectivesModelsList.Tables[0].Rows[0]["activeStatus"].ToString(),
                            updatedByName = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[0]["updatedByName"].ToString()),
                            reviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[0]["reviewedBy"].ToString()),
                            approvedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[0]["approvedBy"].ToString()),
                            reviewstatus = dsObjectivesModelsList.Tables[0].Rows[0]["reviewstatus"].ToString(),
                            approvestatus = dsObjectivesModelsList.Tables[0].Rows[0]["approvestatus"].ToString(),
                            upload = dsObjectivesModelsList.Tables[0].Rows[0]["upload"].ToString()
                        };

                        DateTime dtDocDate = new DateTime();
                        if (dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString() != ""
                            && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString(), out dtDocDate))
                        {
                            objObjectivesModels.initialdevelopmentdate = dtDocDate;
                        }
                        if (dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString() != ""
                        && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString(), out dtDocDate))
                        {
                            objObjectivesModels.updatedOn = dtDocDate;
                        }

                        sSqlstmt = "select id_requirements, LegalRequirement_Id, article, requirements, applicable, nonapplicablejustify, updatedBynametrans,"
                                    + "updatedByposition, updatedByDept,updatedDate,reference,monitoring from t_legalrequirementstrans where LegalRequirement_Id='" + objObjectivesModels.LegalRequirement_Id
                                    + "' order by LegalRequirement_Id desc";
                        ViewBag.FindingsDetails = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        ViewBag.updatedBynametrans = objGlobaldata.GetEmpHrNameById("updatedBynametrans");
                        ViewBag.updatedByDept = objGlobaldata.GetDeptNameById("updatedByDept");
                        ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                        ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");

                        //DataSet dsCommnets = objGlobaldata.Getdetails(sSqlstmt);
                        //ViewBag.dsComments = dsCommnets;
                        //   ViewBag.lawNo = objObjectivesModels.checkObjRefExists(dsObjectivesModelsList.Tables[0].Rows[0]["lawTitle"].ToString());
                        ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                        ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
                        ViewBag.frequency_of_evaluation = objGlobaldata.GetConstantValue("frequency_of_evaluation");
                        ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                        ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                        ViewBag.applicable = objGlobaldata.GetConstantValue("applicable");
                        ViewBag.activeStatus = objGlobaldata.GetConstantValue("activeStatus");
                        ViewBag.DeptHead = objGlobaldata.GetDeptHeadList("");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        // ViewBag.DeptEmpLists = objGlobaldata.GetMultiEmployeeList(' ', "", dsObjectivesModelsList.Tables[0].Rows[0]["updatedByDept"].ToString());
                        ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                        ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                        ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("LegalRegisterList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "LegalRegister Id cannot be null";
                    return RedirectToAction("LegalRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objObjectivesModels);

        }

        // POST: /LegalRegister/LegalRegisterDetails
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LegalRegisterDetails(LegalRegisterModel objObjectivesModels, FormCollection form)
        //{
        //    try
        //    {
        //        if (form["LegalRequirement_Id"] != null && form["LegalRequirement_Id"] != "")
        //        {
        //            //string sLegalRequirement_Id = form["LegalRequirement_Id"], sComments = form["Comment"];

        //            //if (objObjectivesModels.FunAddComments(objGlobaldata.GetCurrentUserSession().empid, sComments, sLegalRequirement_Id))
        //            //{
        //            //    TempData["Successdata"] = "Document Comments details added successfully";
        //            //    return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = sLegalRequirement_Id });
        //            //}
        //            //else
        //            //{
        //            //    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //objGlobaldata.AddFunctionalLog("Exception in LegalRegisterDetails: " + ex.ToString());
        //        //TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
        //    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox('I');
        //    return RedirectToAction("LegalRegisterList");
        //}

        // GET: /LegalRegister/LegalRegisterEdit

        [AllowAnonymous]
        public ActionResult LegalRegisterEdit()
        {
            LegalRegisterModel objObjectivesModels = new LegalRegisterModel();
            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();

                if (Request.QueryString["LegalRequirement_Id"] != null)
                {
                    string sLegalRequirement_Id = Request.QueryString["LegalRequirement_Id"];

                    string sSqlstmt = "select LegalRequirement_Id,   lawNo, lawTitle,initialdevelopmentdate, origin_of_requirement,document_storage_location, frequency_of_evaluation,"
                + " activeStatus,updatedOn, updatedByName,approvedBy,reviewedBy,"
                    + "  approvestatus , reviewstatus,upload from t_legalregister   where LegalRequirement_Id='" + sLegalRequirement_Id + "'";

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        //if (objUser.empid == dsObjectivesModelsList.Tables[0].Rows[0]["updatedByName"].ToString()
                        //    && dsObjectivesModelsList.Tables[0].Rows[0]["approvestatus"].ToString() == "0")
                        {
                            objObjectivesModels = new LegalRegisterModel
                            {
                                LegalRequirement_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["LegalRequirement_Id"].ToString()),
                                lawNo = dsObjectivesModelsList.Tables[0].Rows[0]["lawNo"].ToString(),
                                lawTitle = dsObjectivesModelsList.Tables[0].Rows[0]["lawTitle"].ToString(),
                                origin_of_requirement = dsObjectivesModelsList.Tables[0].Rows[0]["origin_of_requirement"].ToString(),
                                document_storage_location = dsObjectivesModelsList.Tables[0].Rows[0]["document_storage_location"].ToString(),
                                frequency_of_evaluation = dsObjectivesModelsList.Tables[0].Rows[0]["frequency_of_evaluation"].ToString(),
                                activeStatus = dsObjectivesModelsList.Tables[0].Rows[0]["activeStatus"].ToString(),
                                updatedByName = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[0]["updatedByName"].ToString()),
                                reviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[0]["reviewedBy"].ToString()),
                                approvedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[0]["approvedBy"].ToString()),
                                reviewstatus = dsObjectivesModelsList.Tables[0].Rows[0]["reviewstatus"].ToString(),
                                approvestatus = dsObjectivesModelsList.Tables[0].Rows[0]["approvestatus"].ToString(),
                                upload = dsObjectivesModelsList.Tables[0].Rows[0]["upload"].ToString()
                            };

                            DateTime dtDocDate = new DateTime();
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString() != ""
                                && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.initialdevelopmentdate = dtDocDate;
                            }
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString() != ""
                            && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.updatedOn = dtDocDate;
                            }

                            sSqlstmt = "select id_requirements, LegalRequirement_Id, article, requirements, applicable, nonapplicablejustify,"
                                            + "updatedDate, updatedBynametrans,updatedByposition,updatedByDept,reference,monitoring from t_legalrequirementstrans where LegalRequirement_Id='" + objObjectivesModels.LegalRequirement_Id
                                            + "' order by id_requirements desc";
                            ViewBag.FindingsDetails = objGlobaldata.Getdetails(sSqlstmt);
                            ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                            ViewBag.lawNo = objObjectivesModels.GetLegalRegisterRef(dsObjectivesModelsList.Tables[0].Rows[0]["lawNo"].ToString());
                            ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                            ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
                            ViewBag.frequency_of_evaluation = objGlobaldata.GetConstantValue("frequency_of_evaluation");
                            ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                            ViewBag.applicable = objGlobaldata.GetConstantValue("applicable");
                            ViewBag.activeStatus = objGlobaldata.GetConstantValue("activeStatus");
                            ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                            ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                            ViewBag.deptlist = objGlobaldata.GetDepartmentList();
                            ViewBag.DeptHead = objGlobaldata.GetDeptHeadList("");
                            ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                            // ViewBag.DeptEmpLists = objGlobaldata.GetMultiEmployeeList(' ', "", dsObjectivesModelsList.Tables[0].Rows[0]["Dept"].ToString());
                            ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                            ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                            ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                            ViewBag.applicable = objGlobaldata.GetConstantValue("applicable");
                            //   ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                            return View(objObjectivesModels);
                            //  return RedirectToAction("LegalRegisterList");
                        }
                        //else
                        //{
                        //    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                        //    TempData["alertdata"] = "Access Denied";
                        //    return RedirectToAction("LegalRegisterList");
                        //}
                    }
                    else
                    {
                        ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                        ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                        ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("LegalRegisterList");

                    }
                }
                else
                {

                    ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                    ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                    TempData["alertdata"] = "LegalRegister Id cannot be Null or empty";
                    return RedirectToAction("LegalRegisterList");
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("LegalRegisterList");

        }

        // POST: /LegalRegister/LegalRegisterEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LegalRegisterEdit(LegalRegisterModel objLegalRegister, FormCollection form, HttpPostedFileBase upload)
        {
            objLegalRegister.article = form["article"];
            objLegalRegister.requirements = form["requirements"];
            objLegalRegister.applicable = form["applicable"];
            objLegalRegister.nonapplicablejustify = form["nonapplicablejustify"];
            objLegalRegister.updatedByposition = form["updatedByposition"];
            objLegalRegister.updatedBynametrans = form["updatedBynametrans"];
            objLegalRegister.updatedByDept = form["updatedByDept"];
            if (upload != null && upload.ContentLength > 0)
            {
                try
                {
                    string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(upload.FileName));
                    string sFilename = "LegalRegister" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                    string sFilepath = Path.GetDirectoryName(spath);

                    upload.SaveAs(sFilepath + "/" + sFilename);
                    objLegalRegister.upload = "~/DataUpload/MgmtDocs/LegalReg/" + sFilename;
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in AddLegalRegister-upload: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            if (objLegalRegister.FunUpdateLegalRegister(objLegalRegister))
            {
                TempData["Successdata"] = "Legal Register details updated successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("LegalRegisterList"); /*new { LegalRequirement_Id = objLegalRegister.LegalRequirement_Id });*/



        }



        [AllowAnonymous]
        public bool LegalRegisterPlanAdd(LegalRegisterModel objObjectivesModels, FormCollection form)
        {
            try
            {
                LegalRegisterModelsList objLegalRegisterModelsList = new LegalRegisterModelsList();
                objLegalRegisterModelsList.LegalRegisterMList = new List<LegalRegisterModel>();

                DateTime dateValue;

                if (DateTime.TryParse(form["updatedDate"], out dateValue) == true)
                {
                    objObjectivesModels.updatedDate = dateValue;
                }

                objObjectivesModels.applicable = form["applicable"];

                objLegalRegisterModelsList.LegalRegisterMList.Add(objObjectivesModels);

                return objObjectivesModels.FunAddLegalRegisterTrans(objLegalRegisterModelsList, Convert.ToInt16(objObjectivesModels.LegalRequirement_Id));
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //POST: /LegalRegister/ObjectivePlanUpdate

        [AllowAnonymous]
        public ActionResult LegalRegisterPlanUpdate(LegalRegisterModel objObjectivesModels, FormCollection form)
        {
            try
            {


                if (Request["button"].Equals("Save"))
                {


                    if (LegalRegisterPlanAdd(objObjectivesModels, form))
                    {
                        TempData["Successdata"] = "Added Legal Regsiter  successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {



                    DateTime dateValue;

                    if (DateTime.TryParse(form["updatedDate"], out dateValue) == true)
                    {
                        objObjectivesModels.updatedDate = dateValue;
                    }

                    objObjectivesModels.article = form["article"];
                    objObjectivesModels.requirements = form["requirements"];
                    objObjectivesModels.applicable = form["applicable"];
                    objObjectivesModels.nonapplicablejustify = form["nonapplicablejustify"];
                    objObjectivesModels.updatedBynametrans = form["updatedBynametrans"];
                    objObjectivesModels.updatedByposition = form["updatedByposition"];
                    objObjectivesModels.updatedByDept = form["updatedByDept"];
                    objObjectivesModels.approvedBy = form["approvedBy"];


                    //ExternalAuditModels objExternalAuditModels = new ExternalAuditModels();

                    if (objObjectivesModels.FunUpdateLegisterRegisterPlan(objObjectivesModels))
                    {
                        TempData["Successdata"] = "LegalRegister plan updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                return RedirectToAction("LegalRegisterEdit", new { LegalRequirement_Id = objObjectivesModels.LegalRequirement_Id });

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterPlanUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("LegalRegisterList");
        }
        //POST: /LegalRegister/ObjectivePlanAdd

        [AllowAnonymous]
        public bool LegalRegisterPlan(LegalRegisterModel objObjectivesModels, FormCollection form)
        {

            LegalRegisterModelsList objObjectivesModelsList = new LegalRegisterModelsList();
            objObjectivesModelsList.LegalRegisterMList = new List<LegalRegisterModel>();

            DateTime dateValue;

            if (DateTime.TryParse(form["updatedDate"], out dateValue) == true)
            {
                objObjectivesModels.updatedDate = dateValue;
            }
            objObjectivesModelsList.LegalRegisterMList.Add(objObjectivesModels);

            return objObjectivesModels.FunAddLegalRegisterTrans(objObjectivesModelsList, Convert.ToInt16(objObjectivesModels.LegalRequirement_Id));


        }





        //
        // GET: /LegalRegister/AddLegalRegisterEvaluation

        [AllowAnonymous]
        public ActionResult AddLegalRegisterEvaluation()
        {
            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();

                if (Request.QueryString["id_requirements"] != null)
                {
                    string sid_requirements = Request.QueryString["id_requirements"];
                    ViewBag.id_requirements = sid_requirements;
                    DataSet dsObjectives_Id = objGlobaldata.Getdetails("select LegalRequirement_Id, updatedDate,updatedBynametrans from t_legalrequirementstrans where id_requirements='"
                        + sid_requirements + "'");
                    if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.activeStatus = objGlobaldata.GetConstantValue("comply");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        ViewBag.LegalRequirement_Id = dsObjectives_Id.Tables[0].Rows[0]["LegalRequirement_Id"].ToString();
                        ViewBag.updatedBynametrans = dsObjectives_Id.Tables[0].Rows[0]["updatedBynametrans"].ToString();
                        ViewBag.updatedDate = Convert.ToDateTime(dsObjectives_Id.Tables[0].Rows[0]["updatedDate"].ToString()).ToString("dd/MM/yyyy");
                    }
                }
                else
                {

                    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                    TempData["alertdata"] = "Legal Register Id cannot be Null or empty";
                    return RedirectToAction("LegalRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLegalRegisterEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }


        //
        // POST: /LegalRegister/AddLegalRegisterEvaluation

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLegalRegisterEvaluation(LegalRegisterModel objObjectivesModels, FormCollection form, HttpPostedFileBase Evidence)
        {
            try
            {
                DateTime dateValue;

                if (DateTime.TryParse(form["LegalRegister_Eval_On"], out dateValue) == true)
                {
                    objObjectivesModels.LegalRegister_Eval_On = dateValue;
                }

                if (DateTime.TryParse(form["Duedate"], out dateValue) == true)
                {
                    objObjectivesModels.Duedate = dateValue;
                }
                //  objObjectivesModels.id_requirements = Convert.ToInt32(form["id_requirements"]).ToString();

                if (Evidence != null && Evidence.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(Evidence.FileName));
                        string sFilename = "Evidence" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                        Evidence.SaveAs(sFilepath + "/" + sFilename);
                        objObjectivesModels.Evidence = "~/DataUpload/MgmtDocs/LegalReg/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                        ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
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

                if (objObjectivesModels.FunAddLegalRegisterEvaluation(objObjectivesModels))
                {
                    TempData["Successdata"] = "Added Legal Register Evaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLegalRegisterEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            ViewBag.comply = objGlobaldata.GetConstantValue("comply");
            ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
            ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();


            return RedirectToAction("LegalRegisterEvaluationList", new { id_requirements = objObjectivesModels.id_requirements });
        }


        // GET: /LegalRegister/ObjectiveEvaluationList

        [AllowAnonymous]
        public ActionResult LegalRegisterEvaluationList()
        {
            LegalRegisterModelsList objObjectivesModelsList = new LegalRegisterModelsList();
            objObjectivesModelsList.LegalRegisterMList = new List<LegalRegisterModel>();

            //ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox('I');
            //  ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox('I');
            try
            {
                ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                if (Request.QueryString["id_requirements"] != null)
                {
                    string sid_requirements = Request.QueryString["id_requirements"];
                    string sSqlstmt = "select legalregister_evaluation_Id, id_requirements, LegalRegister_Eval_On, Duedate, comply, yes_comply_reason, no_comply_reason, Evidence,Comply_action,personel_responsible "
                                        + "from t_legalregister_evaluation where id_requirements='" + sid_requirements + "'";
                    ViewBag.id_requirements = sid_requirements;
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                            //{
                            //    DateTime LegalRegister_Eval_On = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[i]["dtlegalregister_evaluation_Id"].ToString());
                            //    DateTime dtDuedate= Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[i]["Duedate"].ToString());
                            ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        try
                        {
                            LegalRegisterModel objObjectivesModels = new LegalRegisterModel
                            {
                                legalregister_evaluation_Id = Convert.ToInt32(dsObjectivesModelsList.Tables[0].Rows[0]["legalregister_evaluation_Id"].ToString()),
                                id_requirements = Convert.ToInt32(dsObjectivesModelsList.Tables[0].Rows[0]["id_requirements"].ToString()),
                                comply = dsObjectivesModelsList.Tables[0].Rows[0]["comply"].ToString(),
                                yes_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["yes_comply_reason"].ToString(),
                                no_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["no_comply_reason"].ToString(),
                                Evidence = dsObjectivesModelsList.Tables[0].Rows[0]["Evidence"].ToString(),
                                Comply_action = dsObjectivesModelsList.Tables[0].Rows[0]["Comply_action"].ToString(),
                                personel_responsible = dsObjectivesModelsList.Tables[0].Rows[0]["personel_responsible"].ToString(),

                            };
                            DateTime dtDocDate = new DateTime();
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["LegalRegister_Eval_On"].ToString() != ""
                                && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["LegalRegister_Eval_On"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.LegalRegister_Eval_On = dtDocDate;
                            }
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["Duedate"].ToString() != ""
                               && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["Duedate"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.Duedate = dtDocDate;
                            }

                            objObjectivesModelsList.LegalRegisterMList.Add(objObjectivesModels);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (sid_requirements != "")
                    {
                        DataSet dsObjectives_Id = objGlobaldata.Getdetails("select LegalRequirement_Id from t_legalrequirementstrans where id_requirements='" + sid_requirements + "'");
                        if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.LegalRequirement_Id = dsObjectives_Id.Tables[0].Rows[0]["LegalRequirement_Id"].ToString();
                        }


                        else if (Request.QueryString["LegalRequirement_Id"] != null)
                        {

                            return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                        }
                        else
                        {
                            TempData["alertdata"] = "No data exists";
                            return RedirectToAction("LegalRegisterList");
                        }
                    }

                    else if (Request.QueryString["LegalRequirement_Id"] != null)
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                    }
                }
                else
                {
                    TempData["alertdata"] = "LegalRegister Trans Id cannot be null";
                    return RedirectToAction("LegalRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objObjectivesModelsList.LegalRegisterMList.ToList());
        }

        //
        // GET: /LegalRegister/ObjectiveEvaluationDetails

        [AllowAnonymous]
        public ActionResult LegalRegisterEvaluationDetails()
        {
            LegalRegisterModel objObjectivesModels = new LegalRegisterModel();
            UserCredentials objUser = new UserCredentials();
            objUser = objGlobaldata.GetCurrentUserSession();

            try
            {
                ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                if (Request.QueryString["id_requirements"] != null && Request.QueryString["id_requirements"] != "")
                {
                    string sid_requirements = Request.QueryString["id_requirements"];

                    string sSqlstmt = "select legalregister_evaluation_Id, id_requirements, LegalRegister_Eval_On, Duedate, comply, yes_comply_reason, no_comply_reason, Evidence ,Comply_action,personel_responsible"
                                        + "from t_legalregister_evaluation where id_requirements=" + sid_requirements;

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    if (dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtObj_Eval_On = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["LegalRegister_Eval_On"].ToString());
                        DateTime dtDuedate = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["Duedate"].ToString());

                        objObjectivesModels = new LegalRegisterModel
                        {
                            legalregister_evaluation_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["legalregister_evaluation_Id"].ToString()),
                            id_requirements = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["id_requirements"].ToString()),
                            LegalRegister_Eval_On = dtObj_Eval_On,
                            Duedate = dtDuedate,
                            comply = dsObjectivesModelsList.Tables[0].Rows[0]["comply"].ToString(),
                            yes_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["yes_comply_reason"].ToString(),
                            no_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["no_comply_reason"].ToString(),
                            Evidence = dsObjectivesModelsList.Tables[0].Rows[0]["Evidence"].ToString(),
                            Comply_action = dsObjectivesModelsList.Tables[0].Rows[0]["Comply_action"].ToString(),
                            personel_responsible = dsObjectivesModelsList.Tables[0].Rows[0]["personel_responsible"].ToString(),
                        };

                        DataSet dsObjectives_Id = objGlobaldata.Getdetails("select LegalRequirement_Id from t_legalrequirementstrans where id_requirements='"
                            + objObjectivesModels.id_requirements + "'");
                        ViewBag.LegalRequirement_Id = dsObjectives_Id.Tables[0].Rows[0]["LegalRequirement_Id"].ToString();
                    }
                    else if (Request.QueryString["LegalRequirement_Id"] != null)
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                    }
                    else
                    {
                        return RedirectToAction("AddLegalRegisterEvaluation", new { id_requirements = sid_requirements });
                    }
                }
                else if (Request.QueryString["LegalRequirement_Id"] != null)
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                }
                else
                {
                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("LegalRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objObjectivesModels);

        }

        //
        // GET: /LegalRegister/ObjectiveEvaluationEdit

        [AllowAnonymous]
        public ActionResult LegalRegisterEvaluationEdit()
        {
            LegalRegisterModel objObjectivesModels = new LegalRegisterModel();


            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                if (Request.QueryString["legalregister_evaluation_Id"] != null && Request.QueryString["legalregister_evaluation_Id"] != "")
                {
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    string slegalregister_evaluation_Id = Request.QueryString["legalregister_evaluation_Id"];

                    string sSqlstmt = "select legalregister_evaluation_Id, id_requirements, LegalRegister_Eval_On, Duedate, comply, yes_comply_reason, no_comply_reason, Evidence, Comply_action  ,personel_responsible "
                                        + "from t_legalregister_evaluation where legalregister_evaluation_Id='" + slegalregister_evaluation_Id + "'";

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtLegalRegister_Eval_On = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["LegalRegister_Eval_On"].ToString());
                        DateTime dtDuedate = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["Duedate"].ToString());
                        ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        objObjectivesModels = new LegalRegisterModel
                        {
                            legalregister_evaluation_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["legalregister_evaluation_Id"].ToString()),
                            id_requirements = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["id_requirements"].ToString()),
                            LegalRegister_Eval_On = dtLegalRegister_Eval_On,
                            Duedate = dtDuedate,
                            comply = dsObjectivesModelsList.Tables[0].Rows[0]["comply"].ToString(),
                            yes_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["yes_comply_reason"].ToString(),
                            no_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["no_comply_reason"].ToString(),
                            Evidence = dsObjectivesModelsList.Tables[0].Rows[0]["Evidence"].ToString(),
                            Comply_action = dsObjectivesModelsList.Tables[0].Rows[0]["Comply_action"].ToString(),
                            personel_responsible = dsObjectivesModelsList.Tables[0].Rows[0]["personel_responsible"].ToString(),
                        };
                        ViewBag.id_requirements = objObjectivesModels.id_requirements;
                        ViewBag.legalregister_evaluation_Id = objObjectivesModels.legalregister_evaluation_Id;
                        DataSet dsObjectives_Id = objGlobaldata.Getdetails("select LegalRequirement_Id,updatedDate from t_legalrequirementstrans where id_requirements='"
                            + objObjectivesModels.id_requirements + "'");

                        if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                        {

                            objObjectivesModels.updatedDate = Convert.ToDateTime(dsObjectives_Id.Tables[0].Rows[0]["updatedDate"].ToString());
                        }
                    }
                    else if (Request.QueryString["LegalRequirement_Id"] != null)
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("LegalRegisterList");
                    }
                }
                else if (Request.QueryString["LegalRequirement_Id"] != null)
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("LegalRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            ViewBag.comply = objGlobaldata.GetConstantValue("comply");

            return View(objObjectivesModels);
        }


        //
        // POST: /LegalRegister/ObjectiveEvaluationEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LegalRegisterEvaluationEdit(LegalRegisterModel objObjectivesModels, FormCollection form, HttpPostedFileBase Evidence)
        {
            try
            {
                DateTime dateValue;

                if (DateTime.TryParse(form["LegalRegister_Eval_On"], out dateValue) == true)
                {
                    objObjectivesModels.LegalRegister_Eval_On = dateValue;
                }
                if (DateTime.TryParse(form["Duedate"], out dateValue) == true)
                {
                    objObjectivesModels.Duedate = dateValue;
                }
                objObjectivesModels.legalregister_evaluation_Id = Convert.ToInt16(form["legalregister_evaluation_Id"]);
                objObjectivesModels.id_requirements = Convert.ToInt16(form["id_requirements"]);

                objObjectivesModels.updatedByName = objGlobaldata.GetCurrentUserSession().empid;





                if (Evidence != null && Evidence.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(Evidence.FileName));
                        string sFilename = "Evidence" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Evidence.SaveAs(sFilepath + "/" + sFilename);
                        objObjectivesModels.Evidence = "~/DataUpload/MgmtDocs/LegalReg/" + sFilename;
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

                if (objObjectivesModels.FunUpdateLegalRegisterEvaluation(objObjectivesModels))
                {
                    TempData["Successdata"] = "Legal Register Evaluation details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("LegalRegisterEvaluationList", new { id_requirements = objObjectivesModels.id_requirements });
        }
        public JsonResult LegalRegisterApproveNoty(string approvedBy, string PendingFlg)
        {
            try
            {
                LegalRegisterModel objLegalRegister = new LegalRegisterModel();
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().empid;

                if (objLegalRegister.FunUpdateLegalRegisterApproval(user, approvedBy))
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }
                //  }
                //   else
                //  {
                //        TempData["alertdata"] = "Access Denied";
                //  }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterApprove: " + ex.ToString());
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

        public JsonResult LegalRegisterReviewedNoty(string LegalRequirement_Id, string PendingFlg, string lawNo)
        {
            try
            {
                LegalRegisterModel objLegalRegister = new LegalRegisterModel();
                //string filename = Path.GetFileName(Document);
                //FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                //if (objGlobaldata.GetRoleName(objGlobaldata.GetCurrentUserSession().role) == "Reviewer")
                //{
                if (objLegalRegister.FunUpdateLegalRegisterReview(LegalRequirement_Id, lawNo))
                {
                    return Json("Success");
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
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterReviewed: " + ex.ToString());
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
