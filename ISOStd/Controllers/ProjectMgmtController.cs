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
    public class ProjectMgmtController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();
        private static List<string> objCompanySupplierList = new List<string>();

        public ActionResult ProjectList(int? page)
        {
            ProjectMgmtList lst = new ProjectMgmtList();
            lst.lstPrj = new List<ProjectMgmtModels>();
            try
            {
                string SqlStmt = "select id_projectmgmt,ProjectNo,ProjectName,Location,Customer,ProjectManager,ProjectDocs,"
                    + "ProjectStatus,StartDate,PlannedEndDate,ActualEndDate,Remarks,Team from t_projectmgmt where Active=1";

                DataSet dsProject = objGlobaldata.Getdetails(SqlStmt);

                if (dsProject.Tables.Count > 0 && dsProject.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsProject.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            ProjectMgmtModels objPrj = new ProjectMgmtModels
                            {
                                id_projectmgmt = Convert.ToInt32(dsProject.Tables[0].Rows[i]["id_projectmgmt"].ToString()),
                                ProjectNo = dsProject.Tables[0].Rows[i]["ProjectNo"].ToString(),
                                ProjectName = dsProject.Tables[0].Rows[i]["ProjectName"].ToString(),
                                Location = objGlobaldata.GetCompanyBranchNameById(dsProject.Tables[0].Rows[i]["Location"].ToString()),
                                Customer = dsProject.Tables[0].Rows[i]["Customer"].ToString(),
                                ProjectManager = objGlobaldata.GetEmpHrNameById(dsProject.Tables[0].Rows[i]["ProjectManager"].ToString()),
                                ProjectDocs = dsProject.Tables[0].Rows[i]["ProjectDocs"].ToString(),
                                ProjectStatus = objGlobaldata.GetDropdownitemById(dsProject.Tables[0].Rows[i]["ProjectStatus"].ToString()),
                                Remarks = dsProject.Tables[0].Rows[i]["Remarks"].ToString(),
                                Team = objGlobaldata.GetMultiHrEmpNameById(dsProject.Tables[0].Rows[i]["Team"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsProject.Tables[0].Rows[i]["StartDate"].ToString(), out dtValue))
                            {
                                objPrj.StartDate = dtValue;
                            }

                            if (DateTime.TryParse(dsProject.Tables[0].Rows[i]["PlannedEndDate"].ToString(), out dtValue))
                            {
                                objPrj.PlannedEndDate = dtValue;
                            }

                            if (DateTime.TryParse(dsProject.Tables[0].Rows[i]["ActualEndDate"].ToString(), out dtValue))
                            {
                                objPrj.ActualEndDate = dtValue;
                            }

                            lst.lstPrj.Add(objPrj);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ProjectList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ProjectList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(lst.lstPrj.ToList().ToPagedList(page ?? 1, 40));
        }

        [AllowAnonymous]
        public ActionResult AddDesignReview()
        {
            try
            {
                ViewBag.Dicipline = objGlobaldata.GetDropdownList("Project Dicipline");
                ViewBag.Reviewer = objGlobaldata.GetReviewer();
                ViewBag.Approver = objGlobaldata.GetApprover();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.ProjectStatus = objGlobaldata.GetDropdownList("Project Status");
                ViewBag.Company = objGlobaldata.GetCompanyBranchListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDesignReview: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDesignReview(ProjectMgmtModels objDesign, FormCollection form, IEnumerable<HttpPostedFileBase> ProjectDocs)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                ProjectMgmtList lstDesign = new ProjectMgmtList();
                lstDesign.lstPrj = new List<ProjectMgmtModels>();

                objDesign.Team = form["Team"];
                objDesign.ProjectManager = form["ProjectManager"];
                objDesign.Location = form["Location"];

                if (ProjectDocs != null && files.ContentLength > 0)
                {
                    objDesign.ProjectDocs = "";
                    foreach (var file in ProjectDocs)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                            string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objDesign.ProjectDocs = objDesign.ProjectDocs + "," + "~/DataUpload/MgmtDocs/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objDesign.ProjectDocs = objDesign.ProjectDocs.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    ProjectMgmtModels objDesignlst = new ProjectMgmtModels();
                    DateTime dateValues;
                    objDesignlst.Dicipline = form["Dicipline" + i];
                    objDesignlst.IntRevno = Convert.ToInt32(form["IntRevno" + i]);
                    objDesignlst.DrawingNumber = form["DrawingNumber" + i];
                    objDesignlst.DrawingNumber = objDesignlst.DrawingNumber.ToUpper();
                    objDesignlst.ReviewedBy = form["ReviewedBy" + i];
                    objDesignlst.ApprovedBy = form["ApprovedBy" + i];
                    objDesignlst.Upload = form["Upload" + i];
                    objDesignlst.Upload = objDesignlst.Upload.TrimEnd(',');
                    if (DateTime.TryParse(form["CustApprDate" + i], out dateValues) == true)
                    {
                        objDesignlst.CustApprDate = dateValues;
                    }
                    if (DateTime.TryParse(form["DgnSntDate" + i], out dateValues) == true)
                    {
                        objDesignlst.DgnSntDate = dateValues;
                    }
                    objDesignlst.CustFeedback = form["CustFeedback" + i];
                    objDesignlst.RevNo = form["RevNo" + i];
                    objDesignlst.CustApprStatus = form["CustApprStatus" + i];
                    lstDesign.lstPrj.Add(objDesignlst);
                }

                if (objDesign.FunAddProjectDetails(lstDesign, objDesign))
                {
                    TempData["Successdata"] = "Added Project details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DesignReview: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ProjectList");
        }

        public ActionResult ProjectDesignProcessEdit()
        {
            try
            {
                ViewBag.ProjectStatus = objGlobaldata.GetDropdownList("Project Status");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Dicipline = objGlobaldata.GetDropdownList("Project Dicipline");
                ViewBag.Reviewer = objGlobaldata.GetReviewer();
                ViewBag.Approver = objGlobaldata.GetApprover();
                ViewBag.Company = objGlobaldata.GetCompanyBranchListbox();
                ProjectMgmtModels objProj = new ProjectMgmtModels();
                if (Request.QueryString["id_projectmgmt"] != null && Request.QueryString["id_projectmgmt"] != "")
                {
                    string sid_projectmgmt = Request.QueryString["id_projectmgmt"];
                    string sqlStmt = "select id_projectmgmt,ProjectNo,ProjectName,Location,Customer,ProjectManager,Team,ProjectDocs,ProjectStatus,"
                    + "StartDate,PlannedEndDate,ActualEndDate,Remarks from t_projectmgmt where id_projectmgmt='" + sid_projectmgmt + "'";
                    DataSet dsProject = objGlobaldata.Getdetails(sqlStmt);
                    if (dsProject.Tables.Count > 0 && dsProject.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objProj = new ProjectMgmtModels
                            {
                                id_projectmgmt = Convert.ToInt32(dsProject.Tables[0].Rows[0]["id_projectmgmt"].ToString()),
                                ProjectNo = dsProject.Tables[0].Rows[0]["ProjectNo"].ToString(),
                                ProjectName = dsProject.Tables[0].Rows[0]["ProjectName"].ToString(),
                                Location = objGlobaldata.GetCompanyBranchNameById(dsProject.Tables[0].Rows[0]["Location"].ToString()),
                                Customer = dsProject.Tables[0].Rows[0]["Customer"].ToString(),
                                ProjectManager = objGlobaldata.GetEmpHrNameById(dsProject.Tables[0].Rows[0]["ProjectManager"].ToString()),
                                Team = objGlobaldata.GetMultiHrEmpNameById(dsProject.Tables[0].Rows[0]["Team"].ToString()),
                                ProjectDocs = dsProject.Tables[0].Rows[0]["ProjectDocs"].ToString(),
                                ProjectStatus = objGlobaldata.GetDropdownitemById(dsProject.Tables[0].Rows[0]["ProjectStatus"].ToString()),
                                Remarks = dsProject.Tables[0].Rows[0]["Remarks"].ToString(),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsProject.Tables[0].Rows[0]["StartDate"].ToString(), out dtValue))
                            {
                                objProj.StartDate = dtValue;
                            }
                            if (DateTime.TryParse(dsProject.Tables[0].Rows[0]["PlannedEndDate"].ToString(), out dtValue))
                            {
                                objProj.PlannedEndDate = dtValue;
                            }
                            if (DateTime.TryParse(dsProject.Tables[0].Rows[0]["ActualEndDate"].ToString(), out dtValue))
                            {
                                objProj.ActualEndDate = dtValue;
                            }

                            string SqlStmt = "select id_projectdesign,id_projectmgmt,Dicipline,IntRevno,DrawingNumber,Upload,ReviewedBy,ApprovedBy,"
                            + "CustApprDate,DgnSntDate,CustFeedback,RevNo,CustApprStatus from t_projectdesign where id_projectmgmt='" + objProj.id_projectmgmt + "'";
                            ViewBag.ProjectDesignDetails = objGlobaldata.Getdetails(SqlStmt);

                            return View(objProj);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ProjectEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("ProjectList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("ProjectList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ProjectDesignProcessEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ProjectList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectDesignProcessEdit(FormCollection form, IEnumerable<HttpPostedFileBase> ProjectDocs)
        {
            ProjectMgmtModels objPro = new ProjectMgmtModels();
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                objPro.id_projectmgmt = Convert.ToInt32(form["id_projectmgmt"]);
                objPro.ProjectNo = form["ProjectNo"];
                objPro.ProjectName = form["ProjectName"];
                objPro.Location = form["Location"];
                objPro.Customer = form["Customer"];
                objPro.ProjectManager = form["ProjectManager"];
                objPro.ProjectStatus = form["ProjectStatus"];
                objPro.Remarks = form["Remarks"];
                objPro.Team = form["Team"];

                string QCDelete = Request.Form["QCDocsValselectall"];

                DateTime dateValue;

                if (DateTime.TryParse(form["StartDate"], out dateValue) == true)
                {
                    objPro.StartDate = dateValue;
                }

                if (DateTime.TryParse(form["PlannedEndDate"], out dateValue) == true)
                {
                    objPro.PlannedEndDate = dateValue;
                }

                if (DateTime.TryParse(form["ActualEndDate"], out dateValue) == true)
                {
                    objPro.ActualEndDate = dateValue;
                }

                if (ProjectDocs != null && files.ContentLength > 0)
                {
                    objPro.ProjectDocs = "";
                    foreach (var file in ProjectDocs)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                            string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objPro.ProjectDocs = objPro.ProjectDocs + "," + "~/DataUpload/MgmtDocs/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ProjectDesignProcessEdit-upload: " + ex.ToString());
                        }
                    }
                    objPro.ProjectDocs = objPro.ProjectDocs.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objPro.ProjectDocs = objPro.ProjectDocs + "," + form["QCDocsVal"];
                    objPro.ProjectDocs = objPro.ProjectDocs.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objPro.ProjectDocs = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objPro.ProjectDocs = null;
                }
                if (objPro.FunUpdateProjectDetails(objPro))
                {
                    TempData["Successdata"] = "Updated Project details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ProjectDesignProcessEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ProjectDesignProcessEdit", new { id_projectmgmt = objPro.id_projectmgmt });
        }

        [AllowAnonymous]
        public ActionResult ProjectDetails()
        {
            ProjectMgmtModels objProj = new ProjectMgmtModels();
            try
            {
                if (Request.QueryString["id_projectmgmt"] != null && Request.QueryString["id_projectmgmt"] != "")
                {
                    string sid_projectmgmt = Request.QueryString["id_projectmgmt"];
                    string sqlStmt = "select id_projectmgmt,ProjectNo,ProjectName,Location,Customer,ProjectManager,Team,ProjectDocs,ProjectStatus,"
                   + "StartDate,PlannedEndDate,ActualEndDate,Remarks from t_projectmgmt where id_projectmgmt='" + sid_projectmgmt + "'";
                    DataSet dsProject = objGlobaldata.Getdetails(sqlStmt);
                    if (dsProject.Tables.Count > 0 && dsProject.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objProj = new ProjectMgmtModels
                            {
                                id_projectmgmt = Convert.ToInt32(dsProject.Tables[0].Rows[0]["id_projectmgmt"].ToString()),
                                ProjectNo = dsProject.Tables[0].Rows[0]["ProjectNo"].ToString(),
                                ProjectName = dsProject.Tables[0].Rows[0]["ProjectName"].ToString(),
                                Location = objGlobaldata.GetCompanyBranchNameById(dsProject.Tables[0].Rows[0]["Location"].ToString()),
                                Customer = dsProject.Tables[0].Rows[0]["Customer"].ToString(),
                                ProjectManager = objGlobaldata.GetEmpHrNameById(dsProject.Tables[0].Rows[0]["ProjectManager"].ToString()),
                                Team = objGlobaldata.GetMultiHrEmpNameById(dsProject.Tables[0].Rows[0]["Team"].ToString()),
                                ProjectDocs = dsProject.Tables[0].Rows[0]["ProjectDocs"].ToString(),
                                ProjectStatus = objGlobaldata.GetDropdownitemById(dsProject.Tables[0].Rows[0]["ProjectStatus"].ToString()),
                                Remarks = dsProject.Tables[0].Rows[0]["Remarks"].ToString(),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsProject.Tables[0].Rows[0]["StartDate"].ToString(), out dtValue))
                            {
                                objProj.StartDate = dtValue;
                            }
                            if (DateTime.TryParse(dsProject.Tables[0].Rows[0]["PlannedEndDate"].ToString(), out dtValue))
                            {
                                objProj.PlannedEndDate = dtValue;
                            }
                            if (DateTime.TryParse(dsProject.Tables[0].Rows[0]["ActualEndDate"].ToString(), out dtValue))
                            {
                                objProj.ActualEndDate = dtValue;
                            }

                            string SqlStmt = "select id_projectdesign,id_projectmgmt,Dicipline,IntRevno,DrawingNumber,Upload,ReviewedBy,ApprovedBy,"
                            + "ApprovedDate,ApproverComment,(case when ApproveStatus='1' then 'Approved' else 'Not Approved' end) as ApproveStatus,"
                            + "ReviewedDate,ReviewerComment,(case when ReviewStatus='1' then 'Reviewed' else 'Not Reviewed' end) as ReviewStatus,"
                            + "CustApprDate,DgnSntDate,CustFeedback,RevNo,CustApprStatus from t_projectdesign where id_projectmgmt='" + objProj.id_projectmgmt + "'";
                            ViewBag.ProjectDesignDetails = objGlobaldata.Getdetails(SqlStmt);

                            return View(objProj);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ProjectDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("ProjectList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Objectives Id cannot be null";
                    return RedirectToAction("ProjectList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ProjectDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ProjectList");
        }

        public ActionResult DesignProcessPlanUpdate(ProjectMgmtModels objPrjModels, FormCollection form, IEnumerable<HttpPostedFileBase> Upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                if (Request["button"].Equals("Save"))
                {
                    if (Upload != null && files.ContentLength > 0)
                    {
                        objPrjModels.Upload = "";
                        foreach (var file in Upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objPrjModels.Upload = objPrjModels.Upload + "," + "~/DataUpload/MgmtDocs/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = "ERROR:" + ex.Message.ToString();
                            }
                        }
                        objPrjModels.Upload = objPrjModels.Upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                    if (objPrjModels.DesignReviewPlanAdd(objPrjModels, form))
                    {
                        TempData["Successdata"] = "Added Design Process successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    if (Upload != null && files.ContentLength > 0)
                    {
                        objPrjModels.Upload = "";
                        foreach (var file in Upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objPrjModels.Upload = objPrjModels.Upload + "," + "~/DataUpload/MgmtDocs/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = "ERROR:" + ex.Message.ToString();
                            }
                        }
                        objPrjModels.Upload = objPrjModels.Upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                    DateTime dateValues;

                    objPrjModels.Dicipline = form["Dicipline"];
                    objPrjModels.DrawingNumber = form["DrawingNumber"].ToUpper();
                    objPrjModels.IntRevno = Convert.ToInt32(form["IntRevno"]);
                    objPrjModels.ReviewedBy = form["ReviewedBy"];
                    objPrjModels.ApprovedBy = form["ApprovedBy"];
                    if (DateTime.TryParse(form["CustApprDate"], out dateValues) == true)
                    {
                        objPrjModels.CustApprDate = dateValues;
                    }
                    if (DateTime.TryParse(form["DgnSntDate"], out dateValues) == true)
                    {
                        objPrjModels.DgnSntDate = dateValues;
                    }
                    if (objPrjModels.FunUpdateDesignReviewProcess(objPrjModels))
                    {
                        TempData["Successdata"] = "Design Process updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                return RedirectToAction("ProjectDesignProcessEdit", new { id_projectmgmt = objPrjModels.id_projectmgmt });
                //return objObjectivesModels.FunUpdateObjectivesPlan(objObjectivesModels);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DesignProcessPlanUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ProjectList");
        }

        [AllowAnonymous]
        public ActionResult ProjectDesignReview(string sid_projectdesign, string sid_projectmgmt, int iStatus, string projectcomments)
        {
            try
            {
                ProjectMgmtModels objPrjModels = new ProjectMgmtModels();

                string sStatus = "";
                if (iStatus == 1)
                {
                    sStatus = "Reviewed";
                }

                if (objPrjModels.FunProjectDesignReview(sid_projectdesign, sid_projectmgmt, iStatus, projectcomments))
                {
                    TempData["Successdata"] = "Design" + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ProjectDesignReview: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ListPendingForReview", "Dashboard");
        }

        [AllowAnonymous]
        public ActionResult ProjectDesignApprove(string sid_projectdesign, string sid_projectmgmt, int iStatus, string ProjDesgncomments)
        {
            try
            {
                ProjectMgmtModels objPrjModels = new ProjectMgmtModels();

                string sStatus = "";
                if (iStatus == 1)
                {
                    sStatus = "Approved";
                }

                if (objPrjModels.FunProjectDesignApprove(sid_projectdesign, sid_projectmgmt, iStatus, ProjDesgncomments))
                {
                    TempData["Successdata"] = "Design" + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ProjectDesignApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ListPendingForApproval", "Dashboard");
        }

        public ActionResult ProjectDesignHistory(int? page)
        {
            ProjectMgmtList lst = new ProjectMgmtList();
            lst.lstPrj = new List<ProjectMgmtModels>();
            try
            {
                if (Request.QueryString["id_projectdesign"] != null && Request.QueryString["id_projectdesign"] != "")
                {
                    string sid_projectdesign = Request.QueryString["id_projectdesign"];
                    string sSqlstmt = "select Dicipline,IntRevno,DrawingNumber,Upload,ReviewedBy,ReviewedDate,ReviewerComment,"
                        + "(case when ReviewStatus='1' then 'Reviewed' else 'Not Reviewed' end) as ReviewStatus,ApprovedBy,"
                      + "ApprovedDate,ApproverComment,(case when ApproveStatus='1' then 'Approved' else 'Not Approved' end) "
                      + "as ApproveStatus,CustApprDate,DgnSntDate,CustFeedback,RevNo,CustApprStatus from t_projectdesign_history "
                      + "where id_projectdesign='" + sid_projectdesign + "'";
                    DataSet dsProject = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsProject.Tables.Count > 0 && dsProject.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsProject.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                ProjectMgmtModels objPrj = new ProjectMgmtModels
                                {
                                    Dicipline = objGlobaldata.GetDropdownitemById(dsProject.Tables[0].Rows[i]["Dicipline"].ToString()),
                                    IntRevno = Convert.ToInt32(dsProject.Tables[0].Rows[i]["IntRevno"].ToString()),
                                    DrawingNumber = dsProject.Tables[0].Rows[i]["DrawingNumber"].ToString(),
                                    Upload = dsProject.Tables[0].Rows[i]["Upload"].ToString(),
                                    ReviewedBy = objGlobaldata.GetEmpHrNameById(dsProject.Tables[0].Rows[i]["ReviewedBy"].ToString()),
                                    ReviewerComment = dsProject.Tables[0].Rows[i]["ReviewerComment"].ToString(),
                                    ReviewStatus = dsProject.Tables[0].Rows[i]["ReviewStatus"].ToString(),
                                    ApprovedBy = objGlobaldata.GetEmpHrNameById(dsProject.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                    ApproverComment = dsProject.Tables[0].Rows[i]["ApproverComment"].ToString(),
                                    ApproveStatus = dsProject.Tables[0].Rows[i]["ApproveStatus"].ToString(),
                                    CustFeedback = dsProject.Tables[0].Rows[i]["CustFeedback"].ToString(),
                                    RevNo = dsProject.Tables[0].Rows[i]["RevNo"].ToString(),
                                    CustApprStatus = objGlobaldata.GetDropdownitemById(dsProject.Tables[0].Rows[i]["CustApprStatus"].ToString()),
                                };

                                DateTime dtValue;
                                if (DateTime.TryParse(dsProject.Tables[0].Rows[i]["ReviewedDate"].ToString(), out dtValue))
                                {
                                    objPrj.ReviewedDate = dtValue;
                                }
                                if (DateTime.TryParse(dsProject.Tables[0].Rows[i]["ApprovedDate"].ToString(), out dtValue))
                                {
                                    objPrj.ApprovedDate = dtValue;
                                }
                                if (DateTime.TryParse(dsProject.Tables[0].Rows[i]["CustApprDate"].ToString(), out dtValue))
                                {
                                    objPrj.CustApprDate = dtValue;
                                }
                                if (DateTime.TryParse(dsProject.Tables[0].Rows[i]["DgnSntDate"].ToString(), out dtValue))
                                {
                                    objPrj.DgnSntDate = dtValue;
                                }
                                lst.lstPrj.Add(objPrj);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in ProjectDesignHistory: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ProjectDesignHistory: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(lst.lstPrj.ToList().ToPagedList(page ?? 1, 40));
        }

        [HttpPost]
        public JsonResult GetCustomerList(string Customer)
        {
            objCompanySupplierList = objGlobaldata.GetCustomerListboxList();

            List<string> lstFilteredList = objCompanySupplierList.FindAll(d => d.StartsWith(Customer.ToUpper()));

            return Json(lstFilteredList);
        }

        [HttpPost]
        public JsonResult UploadDocument()
        {
            if (Request.Files.Count > 0)
            {
                string sFile = "", sDrawFile = "";
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                    string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                    file.SaveAs(sFilepath + "/" + "DrawingDesign" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                    sDrawFile = "~/DataUpload/MgmtDocs/" + "DrawingDesign" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename + ",";
                    sFile = sFile + sDrawFile;
                }
                return Json(sFile);
            }
            else
            {
                return Json("");
            }
        }

        //[HttpPost]
        //public JsonResult FunGetReviewerEmpID(string Reviewer)
        //{
        //    string sReviewer = ""; ;
        //    if (Reviewer != null)
        //    {
        //        sReviewer = objGlobaldata.GetEmployeeIdByName(Reviewer);
        //    }

        //    return Json(sReviewer);
        //}
        //[HttpPost]
        //public JsonResult FunGetApproverEmpID(string Approver)
        //{
        //    string sApprover = ""; ;
        //    if (Approver != null)
        //    {
        //        sApprover = objGlobaldata.GetEmployeeIdByName(Approver);
        //    }
        //    return Json(sApprover);
        //}

        [HttpPost]
        public JsonResult FunGetDiciplineID(string Dicipline)
        {
            string sDicipline = ""; ;
            if (Dicipline != null)
            {
                sDicipline = objGlobaldata.GetDiciplineIdByName(Dicipline);
            }
            return Json(sDicipline);
        }

        [HttpPost]
        public JsonResult FunGetProjectStatusByID(string CustApprStatus)
        {
            string sCustApprStatus = ""; ;
            if (CustApprStatus != null)
            {
                sCustApprStatus = objGlobaldata.GetProjectStatusIdByName(CustApprStatus);
            }
            return Json(sCustApprStatus);
        }

        [AllowAnonymous]
        public JsonResult ProjectDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_projectmgmt"] != null && form["id_projectmgmt"] != "")
                {
                    ProjectMgmtModels Doc = new ProjectMgmtModels();
                    string sid_projectmgmt = form["id_projectmgmt"];

                    if (Doc.FunDeleteProjectDoc(sid_projectmgmt))
                    {
                        TempData["Successdata"] = "Project deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in ProjectDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }
}