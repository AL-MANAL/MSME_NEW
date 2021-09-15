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
using System.Security.Principal;
using ISOStd.Filters;


namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class IssuesController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public IssuesController()
        {
            ViewBag.Menutype = "PartiesIssues";
            ViewBag.SubMenutype = "Issues";
        }
         
        [AllowAnonymous]
        public ActionResult AddIssues()
        {
            IssuesModels objIssues = new IssuesModels();
            try
            {

                ViewBag.User = User.Identity.Name;
                objIssues.branch = objGlobaldata.GetCurrentUserSession().division;
                objIssues.Deptid = objGlobaldata.GetCurrentUserSession().DeptID;
                objIssues.Location = objGlobaldata.GetCurrentUserSession().Work_Location;
                //WindowsIdentity user;
                //user = System.Web.HttpContext.Current.Request.LogonUserIdentity;
                //ViewBag.User=user.Name;
                //ViewBag.User = System.Web.HttpContext.Current.User.Identity.Name;
                ViewBag.SubMenutype = "Issues";
                ViewBag.IssueType = objGlobaldata.GetConstantValue("IssueType");
                ViewBag.Impact = objGlobaldata.GetConstantValue("IssueImpact");
                ViewBag.ISOStds = objGlobaldata.GetAllIsoStdListbox();
               // ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                ViewBag.IssueCategory = objGlobaldata.GetDropdownList("Issue Category Type");
                ViewBag.IssueEffect = objGlobaldata.GetDropdownList("Issue Effect Type");
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objIssues.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objIssues.branch);
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Repet_Issue = objGlobaldata.GetAllIssueList();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddIssues: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objIssues);
        }
                 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddIssues(IssuesModels objIssues, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                objIssues.Issue = form["Issue"];
                objIssues.IssueType = form["IssueType"];
                objIssues.Impact = form["Impact"];
                objIssues.Isostd = form["Isostd"];
                objIssues.ImpactDesc = form["ImpactDesc"];
                objIssues.Issue_Category = form["Issue_Category"];
                objIssues.branch = form["branch"];
                objIssues.Deptid = form["Deptid"];
                objIssues.Location = form["Location"];
                objIssues.reporting_to = form["reporting_to"];
                objIssues.notified_to = form["notified_to"];
                objIssues.Effect = form["Effect"];
                objIssues.Impact_detail = form["Impact_detail"];

                DateTime dateValue;
                if (DateTime.TryParse(form["issue_date"], out dateValue) == true)
                {
                    objIssues.issue_date = dateValue;
                }

                //notified_to
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objIssues.notified_to = form["nempno " + i] + "," + objIssues.notified_to;
                    }
                }
                if (objIssues.notified_to != null)
                {
                    objIssues.notified_to = objIssues.notified_to.Trim(',');
                }

                //Reported By
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objIssues.reporting_to = objIssues.reporting_to + "," + form["empno " + i];
                    }
                }
                if (objIssues.reporting_to != null)
                {
                    objIssues.reporting_to = objIssues.reporting_to.Trim(',');
                }

                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Issues"), Path.GetFileName(file.FileName));
                        string sFilename = "Issue" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objIssues.Evidence = "~/DataUpload/MgmtDocs/Issues/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in file-upload: " + ex.ToString());
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objIssues.FunAddIssues(objIssues, file))
                {
                    TempData["Successdata"] = "Added Issues details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddIssues: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("IssuesList");
        }
        
        [AllowAnonymous]
        public ActionResult IssuesList(FormCollection form,int? page, string branch_name)
        {
             ViewBag.SubMenutype = "Issues";

             IssuesModelsList objIssueList = new IssuesModelsList();
             objIssueList.IssueList = new List<IssuesModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
                ViewBag.IssueCategory = objGlobaldata.GetDropdownList("Issue Category Type");
                string sSqlstmt = "select Issue_refno,id_issue,Issue,IssueType,Impact,Isostd,Evidence," +
                    "ImpactDesc,Effect,Deptid,Issue_Category,branch,Location,issue_date,reporting_to,notified_to from t_issues where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_issue desc";
                DataSet dsIssueList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsIssueList.Tables.Count > 0 && dsIssueList.Tables[0].Rows.Count > 0)
                {  
                    for (int i = 0; i < dsIssueList.Tables[0].Rows.Count; i++)
                    {  
                        try
                        {
                            IssuesModels objIssueModels = new IssuesModels
                            {
                                id_issue = Convert.ToInt16(dsIssueList.Tables[0].Rows[i]["id_issue"].ToString()),
                                Issue_refno = dsIssueList.Tables[0].Rows[i]["Issue_refno"].ToString(),
                                Issue = dsIssueList.Tables[0].Rows[i]["Issue"].ToString(),
                                IssueType = dsIssueList.Tables[0].Rows[i]["IssueType"].ToString(),
                                Impact = dsIssueList.Tables[0].Rows[i]["Impact"].ToString(),
                                Isostd = objGlobaldata.GetIsoStdDescriptionById(dsIssueList.Tables[0].Rows[i]["Isostd"].ToString()),
                                Evidence = dsIssueList.Tables[0].Rows[i]["Evidence"].ToString(),
                                ImpactDesc = dsIssueList.Tables[0].Rows[i]["ImpactDesc"].ToString(),
                                Effect = objGlobaldata.GetDropdownitemById(dsIssueList.Tables[0].Rows[i]["Effect"].ToString()),
                                Deptid =objGlobaldata.GetMultiDeptNameById(dsIssueList.Tables[0].Rows[i]["Deptid"].ToString()),
                                Issue_Category = objGlobaldata.GetDropdownitemById(dsIssueList.Tables[0].Rows[i]["Issue_Category"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsIssueList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsIssueList.Tables[0].Rows[i]["Location"].ToString()),
                                reporting_to = objGlobaldata.GetMultiHrEmpNameById(dsIssueList.Tables[0].Rows[i]["reporting_to"].ToString()),
                                notified_to = objGlobaldata.GetMultiHrEmpNameById(dsIssueList.Tables[0].Rows[i]["notified_to"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsIssueList.Tables[0].Rows[i]["issue_date"].ToString(), out dtValue))
                            {
                                objIssueModels.issue_date = dtValue;
                            }
                            objIssueList.IssueList.Add(objIssueModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in IssuesList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                }
       
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssuesList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objIssueList.IssueList.ToList());
        }

        //[AllowAnonymous]
        //public JsonResult IssuesListSearch(FormCollection form, int? page, string branch_name)
        //{
        //    ViewBag.SubMenutype = "Issues";

        //    IssuesModelsList objIssueList = new IssuesModelsList();
        //    objIssueList.IssueList = new List<IssuesModels>();

        //    try
        //    {
        //        string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
        //        string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
        //        ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
        //        string sSearchtext = "";
        //        ViewBag.IssueCategory = objGlobaldata.GetDropdownList("Issue Category Type");
        //        string sSqlstmt = "select Issue_refno,id_issue,Issue,IssueType,Impact,Isostd,Evidence,ImpactDesc," +
        //            "Effect,Deptid,Issue_Category,branch,Location,issue_date,reporting_to,notified_to  from t_issues where Active=1";

        //        if (branch_name != null && branch_name != "")
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
        //            ViewBag.Branch_name = branch_name;
        //        }
        //        else
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
        //        }

        //        sSqlstmt = sSqlstmt + sSearchtext + " order by id_issue desc";
        //        DataSet dsIssueList = objGlobaldata.Getdetails(sSqlstmt);

        //        if (dsIssueList.Tables.Count > 0 && dsIssueList.Tables[0].Rows.Count > 0)
        //        {                  
        //            for (int i = 0; i < dsIssueList.Tables[0].Rows.Count; i++)
        //            {
        //                try
        //                {
        //                    IssuesModels objIssueModels = new IssuesModels
        //                    {
        //                        id_issue = Convert.ToInt16(dsIssueList.Tables[0].Rows[i]["id_issue"].ToString()),
        //                        Issue_refno = dsIssueList.Tables[0].Rows[i]["Issue_refno"].ToString(),
        //                        Issue = dsIssueList.Tables[0].Rows[i]["Issue"].ToString(),
        //                        IssueType = dsIssueList.Tables[0].Rows[i]["IssueType"].ToString(),
        //                        Impact = dsIssueList.Tables[0].Rows[i]["Impact"].ToString(),
        //                        Isostd = objGlobaldata.GetIsoStdDescriptionById(dsIssueList.Tables[0].Rows[i]["Isostd"].ToString()),
        //                        Evidence = dsIssueList.Tables[0].Rows[i]["Evidence"].ToString(),
        //                        ImpactDesc = dsIssueList.Tables[0].Rows[i]["ImpactDesc"].ToString(),
        //                        Effect = dsIssueList.Tables[0].Rows[i]["Effect"].ToString(),
        //                        Deptid = objGlobaldata.GetMultiDeptNameById(dsIssueList.Tables[0].Rows[i]["Deptid"].ToString()),
        //                        Issue_Category = objGlobaldata.GetDropdownitemById(dsIssueList.Tables[0].Rows[i]["Issue_Category"].ToString()),
        //                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsIssueList.Tables[0].Rows[i]["branch"].ToString()),
        //                        Location = objGlobaldata.GetDivisionLocationById(dsIssueList.Tables[0].Rows[i]["Location"].ToString()),
        //                        reporting_to = objGlobaldata.GetMultiHrEmpNameById(dsIssueList.Tables[0].Rows[i]["reporting_to"].ToString()),
        //                        notified_to = objGlobaldata.GetMultiHrEmpNameById(dsIssueList.Tables[0].Rows[i]["notified_to"].ToString()),
        //                    };

        //                    objIssueList.IssueList.Add(objIssueModels);
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobaldata.AddFunctionalLog("Exception in IssuesListSearch: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in IssuesListSearch: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json("Success");
        //}
               
        [AllowAnonymous]
        public ActionResult IssuesInfo(int id)
        {
            ViewBag.SubMenutype = "Issues";

            IssuesModelsList objIssueList = new IssuesModelsList();
            objIssueList.IssueList = new List<IssuesModels>();

            try
            {
                string sSqlstmt = "select Issue_refno,id_issue,Issue,IssueType,Impact,Isostd,Evidence," +
                    "ImpactDesc,Effect,Deptid,Issue_Category,branch,Location,issue_date,reporting_to,notified_to,Impact_detail,Repet_Issue  from t_issues where Active=1"
                + " and id_issue='"+id+"' order by id_issue desc";
                DataSet dsIssueList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsIssueList.Tables.Count > 0 && dsIssueList.Tables[0].Rows.Count > 0)
                {    try
                        {
                            IssuesModels objIssueModels = new IssuesModels
                            {
                                id_issue = Convert.ToInt16(dsIssueList.Tables[0].Rows[0]["id_issue"].ToString()),
                                Issue = dsIssueList.Tables[0].Rows[0]["Issue"].ToString(),
                                IssueType = dsIssueList.Tables[0].Rows[0]["IssueType"].ToString(),
                                Impact = dsIssueList.Tables[0].Rows[0]["Impact"].ToString(),
                                Isostd = objGlobaldata.GetIsoStdDescriptionById(dsIssueList.Tables[0].Rows[0]["Isostd"].ToString()),
                                Evidence = dsIssueList.Tables[0].Rows[0]["Evidence"].ToString(),
                                ImpactDesc = dsIssueList.Tables[0].Rows[0]["ImpactDesc"].ToString(),
                                Effect = objGlobaldata.GetDropdownitemById(dsIssueList.Tables[0].Rows[0]["Effect"].ToString()),
                                Issue_refno = dsIssueList.Tables[0].Rows[0]["Issue_refno"].ToString(),
                                Deptid = objGlobaldata.GetMultiDeptNameById(dsIssueList.Tables[0].Rows[0]["Deptid"].ToString()),
                                Issue_Category = objGlobaldata.GetDropdownitemById(dsIssueList.Tables[0].Rows[0]["Issue_Category"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsIssueList.Tables[0].Rows[0]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsIssueList.Tables[0].Rows[0]["Location"].ToString()),
                                reporting_to = objGlobaldata.GetMultiHrEmpNameById(dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString()),
                                notified_to = objGlobaldata.GetMultiHrEmpNameById(dsIssueList.Tables[0].Rows[0]["notified_to"].ToString()),
                                Impact_detail= objGlobaldata.GetDropdownitemById(dsIssueList.Tables[0].Rows[0]["Impact_detail"].ToString()),
                                Repet_Issue= objGlobaldata.GetIssueRefnobyId(dsIssueList.Tables[0].Rows[0]["Repet_Issue"].ToString()),
                                Repet_Issue_detail = objGlobaldata.GetIssueRefnobyId(dsIssueList.Tables[0].Rows[0]["Repet_Issue"].ToString())
                            };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsIssueList.Tables[0].Rows[0]["issue_date"].ToString(), out dtValue))
                        {
                            objIssueModels.issue_date = dtValue;
                        }
                        objIssueList.IssueList.Add(objIssueModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in IssuesList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssuesList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objIssueList.IssueList.ToList());
        }
                 
        [AllowAnonymous]
        public JsonResult IssueDocDelete(FormCollection form)
        {
            try
            {
                  
                        if (form["id_issue"] != null && form["id_issue"] != "")
                        {

                            IssuesModels Doc = new IssuesModels();
                            string sid_issue = form["id_issue"];


                            if (Doc.FunDeleteIssueDoc(sid_issue))
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
                            TempData["alertdata"] = "Issue Id cannot be Null or empty";
                            return Json("Failed");
                        }                  
                   
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
                 
        [AllowAnonymous]
        public ActionResult IssueEdit()
        {
            ViewBag.SubMenutype = "Issues";
            IssuesModels objIssueModels = new IssuesModels();

            try
            {
               // ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                ViewBag.IssueType = objGlobaldata.GetConstantValue("IssueType");
                ViewBag.Impact = objGlobaldata.GetConstantValue("IssueImpact");
                ViewBag.ISOStds = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.IssueCategory = objGlobaldata.GetDropdownList("Issue Category Type");
                ViewBag.IssueEffect = objGlobaldata.GetDropdownList("Issue Effect Type");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Repet_Issue = objGlobaldata.GetAllIssueList();

                if (Request.QueryString["id_issue"] != null && Request.QueryString["id_issue"] != "")
                {
                    string id_issue = Request.QueryString["id_issue"];

                    ViewBag.id_issue = id_issue;

                    string sSqlstmt = "select Issue_refno,id_issue,Issue,IssueType,Impact,Isostd,Evidence,ImpactDesc," +
                        "Effect,Deptid,Issue_Category,branch,Location,issue_date,reporting_to,notified_to,Impact_detail,Repet_Issue from t_issues where id_issue='" + id_issue + "'";

                    DataSet dsIssueList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsIssueList.Tables.Count > 0 && dsIssueList.Tables[0].Rows.Count > 0)
                    {

                        objIssueModels = new IssuesModels
                        {
                            id_issue = Convert.ToInt16(dsIssueList.Tables[0].Rows[0]["id_issue"].ToString()),
                            Issue = dsIssueList.Tables[0].Rows[0]["Issue"].ToString(),
                            IssueType = dsIssueList.Tables[0].Rows[0]["IssueType"].ToString(),
                            Impact = dsIssueList.Tables[0].Rows[0]["Impact"].ToString(),
                            Isostd = objGlobaldata.GetIsoStdDescriptionById(dsIssueList.Tables[0].Rows[0]["Isostd"].ToString()),
                            Evidence = dsIssueList.Tables[0].Rows[0]["Evidence"].ToString(),
                            ImpactDesc = dsIssueList.Tables[0].Rows[0]["ImpactDesc"].ToString(),
                            Effect = dsIssueList.Tables[0].Rows[0]["Effect"].ToString(),
                            Issue_refno = dsIssueList.Tables[0].Rows[0]["Issue_refno"].ToString(),
                            Deptid = objGlobaldata.GetMultiDeptNameById(dsIssueList.Tables[0].Rows[0]["Deptid"].ToString()),
                            Issue_Category = objGlobaldata.GetDropdownitemById(dsIssueList.Tables[0].Rows[0]["Issue_Category"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsIssueList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsIssueList.Tables[0].Rows[0]["Location"].ToString()),
                            reporting_to = (dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString()),
                            notified_to = (dsIssueList.Tables[0].Rows[0]["notified_to"].ToString()),
                            Impact_detail= (dsIssueList.Tables[0].Rows[0]["Impact_detail"].ToString()),
                            Repet_Issue= (dsIssueList.Tables[0].Rows[0]["Repet_Issue"].ToString())
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsIssueList.Tables[0].Rows[0]["issue_date"].ToString(), out dtValue))
                        {
                            objIssueModels.issue_date = dtValue;
                        }
                        if (dsIssueList.Tables[0].Rows[0]["Impact"].ToString().ToLower() == "low")
                        {
                            ViewBag.ImpactDetails = objGlobaldata.GetDropdownList("Issue Impact Low");
                        }
                        else if (dsIssueList.Tables[0].Rows[0]["Impact"].ToString().ToLower() == "moderate")
                        {
                            ViewBag.ImpactDetails = objGlobaldata.GetDropdownList("Issue Impact Moderate");
                        }
                        else if (dsIssueList.Tables[0].Rows[0]["Impact"].ToString().ToLower() == "high")
                        {
                            ViewBag.ImpactDetails = objGlobaldata.GetDropdownList("Issue Impact High");
                        }
                        else if (dsIssueList.Tables[0].Rows[0]["Impact"].ToString().ToLower() == "extreme")
                        {
                            ViewBag.ImpactDetails = objGlobaldata.GetDropdownList("Issue Impact Extreme");
                        }



                        if (dsIssueList.Tables[0].Rows[0]["notified_to"].ToString() != "")
                        {
                            ViewBag.notified_Array = (dsIssueList.Tables[0].Rows[0]["notified_to"].ToString()).Split(',');
                        }

                        if (dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString() != "")
                        {
                            ViewBag.ReportedByArray = (dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString()).Split(',');
                        }
                        //    ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsIssueList.Tables[0].Rows[0]["branch"].ToString());
                        //    ViewBag.Department = objGlobaldata.GetDepartmentList1(dsIssueList.Tables[0].Rows[0]["branch"].ToString());
                        //    ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                    }
                    else
                    {
                        TempData["alertdata"] = "issueId cannot be Null or empty";
                        return RedirectToAction("IssuesList");
                    }
                }
                else
                {                  
                    TempData["alertdata"] = "issueId cannot be Null or empty";
                    return RedirectToAction("IssuesList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("IssuesList");
            }
            return View(objIssueModels);
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IssueEdit(IssuesModels objIssues, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                objIssues.Issue = form["Issue"];
                objIssues.IssueType = form["IssueType"];
                objIssues.Impact = form["Impact"];
                objIssues.Isostd = form["Isostd"];
                objIssues.ImpactDesc = form["ImpactDesc"];
                objIssues.Issue_Category = form["Issue_Category"];
                objIssues.reporting_to = form["reporting_to"];
                objIssues.notified_to = form["notified_to"];
                objIssues.Effect = form["Effect"];
                objIssues.Impact_detail = form["Impact_detail"];
                //  objIssues.branch = form["branch"];
                //   objIssues.Deptid = form["Deptid"];
                // objIssues.Location = form["Location"];
                //  objIssues.Issue_refno = form["Issue_refno"];

                DateTime dateValue;
                if (DateTime.TryParse(form["issue_date"], out dateValue) == true)
                {
                    objIssues.issue_date = dateValue;
                }

                //notified_to
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objIssues.notified_to = form["nempno " + i] + "," + objIssues.notified_to;
                    }
                }
                if (objIssues.notified_to != null)
                {
                    objIssues.notified_to = objIssues.notified_to.Trim(',');
                }

                //Reported By
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objIssues.reporting_to = objIssues.reporting_to + "," + form["empno " + i];
                    }
                }
                if (objIssues.reporting_to != null)
                {
                    objIssues.reporting_to = objIssues.reporting_to.Trim(',');
                }


                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Issues"), Path.GetFileName(file.FileName));
                        string sFilename = "Issue" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objIssues.Evidence = "~/DataUpload/MgmtDocs/Issues/" + sFilename;
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

                if (objIssues.FunUpdateIssues(objIssues, file))
                {
                    TempData["Successdata"] = "Issues details Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("IssuesList");
            }
            return RedirectToAction("IssuesList");     
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult doesIssueRefNoExist(string Issue_refno)
        {
            IssuesModels objIssueDocuments = new IssuesModels();
            var user = objIssueDocuments.CheckForIssueRefNoExists(Issue_refno);

            return Json(user);
        }

        public JsonResult FunGetIssueImpactTypeList(string Impact_type)
        {           
            if (Impact_type != null && Impact_type != "")
            {
                if(Impact_type.ToLower() == "low")
                {
                    MultiSelectList ImpactList = objGlobaldata.GetDropdownList("Issue Impact Low");
                    return Json(ImpactList);
                }
                else if (Impact_type.ToLower() == "moderate")
                {
                    MultiSelectList ImpactList = objGlobaldata.GetDropdownList("Issue Impact Moderate");
                    return Json(ImpactList);
                }
                else if (Impact_type.ToLower() == "high")
                {
                    MultiSelectList ImpactList = objGlobaldata.GetDropdownList("Issue Impact High");
                    return Json(ImpactList);
                }
                else if (Impact_type.ToLower() == "extreme")
                {
                    MultiSelectList ImpactList = objGlobaldata.GetDropdownList("Issue Impact Extreme");
                    return Json(ImpactList);
                }
            }
            return Json ("");
           
        }

        public JsonResult FunGetIssueDetailbyRefNo(string Issue_id)
        {
            try
            {                
                if(Issue_id != "")
                {
                    string Detail = "";
                    string Sqlstmt= "Select Issue from t_issues where id_issue = '"+ Issue_id + "'";
                    DataSet dsIssue = objGlobaldata.Getdetails(Sqlstmt);
                    if(dsIssue.Tables.Count>0 && dsIssue.Tables[0].Rows.Count>0)
                    {
                        Detail = dsIssue.Tables[0].Rows[0]["Issue"].ToString();
                    }
                    return Json(Detail);
                }               
            }
            catch(Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetIssueDetailbyRefNo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }

    }
}