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
    public class NearMissController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public NearMissController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "NearMiss";
        }

        [AllowAnonymous]
        public ActionResult AddNearMiss()
        {
            try
            {
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Reviewer = objGlobaldata.GetHrQHSEEmployeeListbox();
                ViewBag.NearmissCause = objGlobaldata.GetDropdownList("Nearmiss Causes");
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
                ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddNearMiss: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddNearMiss(NearMissModels objNearMiss, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objNearMiss.reported_by = form["reported_by"];
                objNearMiss.reviewed_by = form["reviewed_by"];
                objNearMiss.causes = form["causes"];

                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;
                if (form["incident_date"] != null && DateTime.TryParse(form["incident_date"], out dateValue) == true)
                {
                    objNearMiss.incident_date = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objNearMiss.incident_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (DateTime.TryParse(form["reported_date"], out dateValue) == true)
                {
                    objNearMiss.reported_date = dateValue;
                }

                if (upload != null && files.ContentLength > 0)
                {
                    objNearMiss.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "NearMiss" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objNearMiss.upload = objNearMiss.upload + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddNearMiss-upload: " + ex.ToString());
                        }
                    }
                    objNearMiss.upload = objNearMiss.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objNearMiss.FunAddNearMissDetails(objNearMiss))
                {
                    TempData["Successdata"] = "Added Near Miss details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddNearMiss: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("NearMissList");
        }

        [AllowAnonymous]
        public ActionResult NearMissList(string branch_name)
        {
            NearMissModelsList objNearMiss = new NearMissModelsList();
            objNearMiss.lstNearMiss = new List<NearMissModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_nearmiss,incident_date,reported_date,report_no,reported_by,reported_by_position,reported_by_dept,"
                    + "location,description,upload,effect_incident,reviewed_by,causes,project from t_nearmiss where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by incident_date desc";
                DataSet dsNearMissList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsNearMissList.Tables.Count > 0 && dsNearMissList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNearMissList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            NearMissModels objNearMissModel = new NearMissModels
                            {
                                id_nearmiss = Convert.ToInt16(dsNearMissList.Tables[0].Rows[i]["id_nearmiss"].ToString()),
                                report_no = (dsNearMissList.Tables[0].Rows[i]["report_no"].ToString()),
                                reported_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[i]["reported_by"].ToString()),
                                reported_by_position = (dsNearMissList.Tables[0].Rows[i]["reported_by_position"].ToString()),
                                reported_by_dept = (dsNearMissList.Tables[0].Rows[i]["reported_by_dept"].ToString()),
                                location = objGlobaldata.GetCompanyBranchNameById(dsNearMissList.Tables[0].Rows[i]["location"].ToString()),
                                description = (dsNearMissList.Tables[0].Rows[i]["description"].ToString()),
                                upload = (dsNearMissList.Tables[0].Rows[i]["upload"].ToString()),
                                effect_incident = (dsNearMissList.Tables[0].Rows[i]["effect_incident"].ToString()),
                                reviewed_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[i]["reviewed_by"].ToString()),
                                causes = objGlobaldata.GetDropdownitemById(dsNearMissList.Tables[0].Rows[i]["causes"].ToString()),
                                project = objGlobaldata.GetDropdownitemById(dsNearMissList.Tables[0].Rows[i]["project"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsNearMissList.Tables[0].Rows[i]["incident_date"].ToString(), out dtValue))
                            {
                                objNearMissModel.incident_date = dtValue;
                            }
                            if (DateTime.TryParse(dsNearMissList.Tables[0].Rows[i]["reported_date"].ToString(), out dtValue))
                            {
                                objNearMissModel.reported_date = dtValue;
                            }
                            objNearMiss.lstNearMiss.Add(objNearMissModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in NearMissList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NearMissList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNearMiss.lstNearMiss.ToList());
        }

        [AllowAnonymous]
        public JsonResult NearMissListSearch(string branch_name)
        {
            NearMissModelsList objNearMiss = new NearMissModelsList();
            objNearMiss.lstNearMiss = new List<NearMissModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_nearmiss,incident_date,reported_date,report_no,reported_by,reported_by_position,reported_by_dept,"
                    + "location,description,upload,effect_incident,reviewed_by,causes,project from t_nearmiss where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by incident_date desc";
                DataSet dsNearMissList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsNearMissList.Tables.Count > 0 && dsNearMissList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNearMissList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            NearMissModels objNearMissModel = new NearMissModels
                            {
                                id_nearmiss = Convert.ToInt16(dsNearMissList.Tables[0].Rows[i]["id_nearmiss"].ToString()),
                                report_no = (dsNearMissList.Tables[0].Rows[i]["report_no"].ToString()),
                                reported_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[i]["reported_by"].ToString()),
                                reported_by_position = (dsNearMissList.Tables[0].Rows[i]["reported_by_position"].ToString()),
                                reported_by_dept = (dsNearMissList.Tables[0].Rows[i]["reported_by_dept"].ToString()),
                                location = objGlobaldata.GetCompanyBranchNameById(dsNearMissList.Tables[0].Rows[i]["location"].ToString()),
                                description = (dsNearMissList.Tables[0].Rows[i]["description"].ToString()),
                                upload = (dsNearMissList.Tables[0].Rows[i]["upload"].ToString()),
                                effect_incident = (dsNearMissList.Tables[0].Rows[i]["effect_incident"].ToString()),
                                reviewed_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[i]["reviewed_by"].ToString()),
                                causes = objGlobaldata.GetDropdownitemById(dsNearMissList.Tables[0].Rows[i]["causes"].ToString()),
                                project = objGlobaldata.GetDropdownitemById(dsNearMissList.Tables[0].Rows[i]["project"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsNearMissList.Tables[0].Rows[i]["incident_date"].ToString(), out dtValue))
                            {
                                objNearMissModel.incident_date = dtValue;
                            }
                            if (DateTime.TryParse(dsNearMissList.Tables[0].Rows[i]["reported_date"].ToString(), out dtValue))
                            {
                                objNearMissModel.reported_date = dtValue;
                            }
                            objNearMiss.lstNearMiss.Add(objNearMissModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in NearMissListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NearMissListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult NearMissEdit()
        {
            NearMissModels objNearMiss = new NearMissModels();

            try
            {
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Reviewer = objGlobaldata.GetHrQHSEEmployeeListbox();
                ViewBag.NearmissCause = objGlobaldata.GetDropdownList("Nearmiss Causes");
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
                ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();

                if (Request.QueryString["id_nearmiss"] != null && Request.QueryString["id_nearmiss"] != "")
                {
                    string sid_nearmiss = Request.QueryString["id_nearmiss"];

                    string sSqlstmt = "select id_nearmiss,incident_date,reported_date,report_no,reported_by,reported_by_position,reported_by_dept,"
                    + "location,description,upload,effect_incident,reviewed_by,causes,project from t_nearmiss where id_nearmiss='" + sid_nearmiss + "'";

                    DataSet dsNearMissList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNearMissList.Tables.Count > 0 && dsNearMissList.Tables[0].Rows.Count > 0)
                    {
                        objNearMiss = new NearMissModels
                        {
                            id_nearmiss = Convert.ToInt16(dsNearMissList.Tables[0].Rows[0]["id_nearmiss"].ToString()),
                            report_no = (dsNearMissList.Tables[0].Rows[0]["report_no"].ToString()),
                            reported_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[0]["reported_by"].ToString()),
                            reported_by_position = (dsNearMissList.Tables[0].Rows[0]["reported_by_position"].ToString()),
                            reported_by_dept = (dsNearMissList.Tables[0].Rows[0]["reported_by_dept"].ToString()),
                            location = objGlobaldata.GetCompanyBranchNameById(dsNearMissList.Tables[0].Rows[0]["location"].ToString()),
                            description = (dsNearMissList.Tables[0].Rows[0]["description"].ToString()),
                            upload = (dsNearMissList.Tables[0].Rows[0]["upload"].ToString()),
                            effect_incident = (dsNearMissList.Tables[0].Rows[0]["effect_incident"].ToString()),
                            reviewed_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            causes = objGlobaldata.GetDropdownitemById(dsNearMissList.Tables[0].Rows[0]["causes"].ToString()),
                            project = objGlobaldata.GetDropdownitemById(dsNearMissList.Tables[0].Rows[0]["project"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsNearMissList.Tables[0].Rows[0]["incident_date"].ToString(), out dtValue))
                        {
                            objNearMiss.incident_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNearMissList.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                        {
                            objNearMiss.reported_date = dtValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Naermiss ID cannot be Null or empty";
                        return RedirectToAction("NearMissList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Naermiss ID cannot be Null or empty";
                    return RedirectToAction("NearMissList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NearMissEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("NearMissList");
            }
            return View(objNearMiss);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult NearMissEdit(NearMissModels objNearMiss, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objNearMiss.reported_by = form["reported_by"];
                objNearMiss.reviewed_by = form["reviewed_by"];
                objNearMiss.causes = form["causes"];
                string QCDelete = Request.Form["QCDocsValselectall"];
                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;
                if (form["incident_date"] != null && DateTime.TryParse(form["incident_date"], out dateValue) == true)
                {
                    objNearMiss.incident_date = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objNearMiss.incident_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (DateTime.TryParse(form["reported_date"], out dateValue) == true)
                {
                    objNearMiss.reported_date = dateValue;
                }

                if (upload != null && files.ContentLength > 0)
                {
                    objNearMiss.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "NearMiss" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objNearMiss.upload = objNearMiss.upload + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddNearMiss-upload: " + ex.ToString());
                        }
                    }
                    objNearMiss.upload = objNearMiss.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objNearMiss.upload = objNearMiss.upload + "," + form["QCDocsVal"];
                    objNearMiss.upload = objNearMiss.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objNearMiss.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objNearMiss.upload = null;
                }
                if (objNearMiss.FunUpdateNearMissDetails(objNearMiss))
                {
                    TempData["Successdata"] = "Updated Near Miss details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NearMissEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("NearMissList");
        }

        [AllowAnonymous]
        public JsonResult NearMissDelete(FormCollection form)
        {
            try
            {
                if (form["id_nearmiss"] != null && form["id_nearmiss"] != "")
                {
                    NearMissModels Doc = new NearMissModels();
                    string sid_nearmiss = form["id_nearmiss"];

                    if (Doc.FunDeleteNearMiss(sid_nearmiss))
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
                    TempData["alertdata"] = "Near Miss Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NearMissDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult GetEmpDetail(string empid)
        {
            EmployeeModels objEmpModels = new EmployeeModels();
            try
            {
                string sqlstmt = "select Designation, Dept_Id as DeptID from t_hr_employee e,t_departments t where e.Dept_Id=t.DeptId and emp_no='" + empid + "'";
                DataSet dsEmp = objGlobaldata.Getdetails(sqlstmt);

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        objEmpModels = new EmployeeModels
                        {
                            Designation = dsEmp.Tables[0].Rows[0]["Designation"].ToString(),
                            DeptID = objGlobaldata.GetDeptNameById(dsEmp.Tables[0].Rows[0]["DeptID"].ToString()),
                        };
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in GetEmpDetail: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssuesList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(objEmpModels);
        }

        [AllowAnonymous]
        public ActionResult ActionNearMiss()
        {
            NearMissModels objNearMiss = new NearMissModels();
            try
            {
                ViewBag.Hazard = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.Verifier = objGlobaldata.GetHrQHSEEmployeeListbox();
                ViewBag.Reviewer = objGlobaldata.GetDeptHeadList();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                if (Request.QueryString["id_nearmiss"] != null && Request.QueryString["id_nearmiss"] != "")
                {
                    string sid_nearmiss = Request.QueryString["id_nearmiss"];

                    string sSqlstmt = "select id_nearmiss from t_nearmiss where id_nearmiss='" + sid_nearmiss + "'";
                    DataSet dsNearMissList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNearMissList.Tables.Count > 0 && dsNearMissList.Tables[0].Rows.Count > 0)
                    {
                        objNearMiss = new NearMissModels
                        {
                            id_nearmiss = Convert.ToInt32(dsNearMissList.Tables[0].Rows[0]["id_nearmiss"].ToString()),
                        };
                    }
                }
                else
                {
                    TempData["Successdata"] = "ID cannot be null";
                    return RedirectToAction("NearMissList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionNearMiss: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNearMiss);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ActionNearMiss(NearMissModels objNearMiss, FormCollection form)
        {
            try
            {
                objNearMiss.verified_by = form["verified_by"];
                objNearMiss.reviewed_by = form["reviewed_by"];

                if (objNearMiss.FunAddNearMissActionDetails(objNearMiss))
                {
                    TempData["Successdata"] = "Added Near Miss Action details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionNearMiss: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("NearMissActionList", new { objNearMiss.id_nearmiss });
        }

        [AllowAnonymous]
        public ActionResult NearMissActionList()
        {
            NearMissModelsList objNearMiss = new NearMissModelsList();
            objNearMiss.lstNearMiss = new List<NearMissModels>();
            try
            {
                if (Request.QueryString["id_nearmiss"] != null && Request.QueryString["id_nearmiss"] != "")
                {
                    string sid_nearmiss = Request.QueryString["id_nearmiss"];
                    string sSqlstmt = "select id_nearmiss_action,id_nearmiss,correction,action,hazard,verified_by,reviewed_by"
                    + " from t_nearmiss_action where id_nearmiss='" + sid_nearmiss + "' and Active=1";
                    DataSet dsNearMissList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNearMissList.Tables.Count > 0 && dsNearMissList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsNearMissList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                NearMissModels objNearMissModel = new NearMissModels
                                {
                                    id_nearmiss_action = Convert.ToInt16(dsNearMissList.Tables[0].Rows[i]["id_nearmiss_action"].ToString()),
                                    id_nearmiss = Convert.ToInt16(dsNearMissList.Tables[0].Rows[i]["id_nearmiss"].ToString()),
                                    correction = (dsNearMissList.Tables[0].Rows[i]["correction"].ToString()),
                                    action = (dsNearMissList.Tables[0].Rows[i]["action"].ToString()),
                                    hazard = (dsNearMissList.Tables[0].Rows[i]["hazard"].ToString()),
                                    verified_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[i]["verified_by"].ToString()),
                                    reviewed_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[i]["reviewed_by"].ToString()),
                                };
                                objNearMiss.lstNearMiss.Add(objNearMissModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in NearMissActionList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id Cannot be null";
                    return RedirectToAction("NearMissList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NearMissActionList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNearMiss.lstNearMiss.ToList());
        }

        [AllowAnonymous]
        public JsonResult NearMissActionDelete(FormCollection form)
        {
            try
            {
                if (form["id_nearmiss_action"] != null && form["id_nearmiss_action"] != "")
                {
                    NearMissModels Doc = new NearMissModels();
                    string sid_nearmiss_action = form["id_nearmiss_action"];

                    if (Doc.FunDeleteNearMissAction(sid_nearmiss_action))
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
                    TempData["alertdata"] = "Near Miss Action Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NearMissActionDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult NearMissActionEdit()
        {
            NearMissModels objNearMiss = new NearMissModels();

            try
            {
                ViewBag.Hazard = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.Verifier = objGlobaldata.GetHrQHSEEmployeeListbox();
                ViewBag.Reviewer = objGlobaldata.GetDeptHeadList();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                if (Request.QueryString["id_nearmiss_action"] != null && Request.QueryString["id_nearmiss_action"] != "")
                {
                    string sid_nearmiss_action = Request.QueryString["id_nearmiss_action"];

                    string sSqlstmt = "select id_nearmiss_action,id_nearmiss,correction,action,hazard,verified_by,reviewed_by from t_nearmiss_action where id_nearmiss_action='" + sid_nearmiss_action + "'";

                    DataSet dsNearMissList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNearMissList.Tables.Count > 0 && dsNearMissList.Tables[0].Rows.Count > 0)
                    {
                        objNearMiss = new NearMissModels
                        {
                            id_nearmiss_action = Convert.ToInt16(dsNearMissList.Tables[0].Rows[0]["id_nearmiss_action"].ToString()),
                            id_nearmiss = Convert.ToInt16(dsNearMissList.Tables[0].Rows[0]["id_nearmiss"].ToString()),
                            correction = (dsNearMissList.Tables[0].Rows[0]["correction"].ToString()),
                            action = (dsNearMissList.Tables[0].Rows[0]["action"].ToString()),
                            hazard = (dsNearMissList.Tables[0].Rows[0]["hazard"].ToString()),
                            verified_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[0]["verified_by"].ToString()),
                            reviewed_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                        };
                    }
                    else
                    {
                        TempData["alertdata"] = "Naermiss ID cannot be Null or empty";
                        return RedirectToAction("NearMissList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Naermiss ID cannot be Null or empty";
                    return RedirectToAction("NearMissList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NearMissActionEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("NearMissList");
            }
            return View(objNearMiss);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult NearMissActionEdit(NearMissModels objNearMiss, FormCollection form)
        {
            try
            {
                objNearMiss.verified_by = form["verified_by"];
                objNearMiss.reviewed_by = form["reviewed_by"];

                if (objNearMiss.FunUpdateNearMissActionDetails(objNearMiss))
                {
                    TempData["Successdata"] = "Added Near Miss Action details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionNearMiss: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("NearMissActionList", new { objNearMiss.id_nearmiss });
        }

        [AllowAnonymous]
        public ActionResult NearMissDetails()
        {
            NearMissModels objNearMiss = new NearMissModels();

            try
            {
                if (Request.QueryString["id_nearmiss"] != null && Request.QueryString["id_nearmiss"] != "")
                {
                    string sid_nearmiss = Request.QueryString["id_nearmiss"];
                    string sSqlstmt = "select id_nearmiss,incident_date,reported_date,report_no,reported_by,reported_by_position,reported_by_dept,"
                     + "location,description,upload,effect_incident,reviewed_by,causes,project from t_nearmiss where id_nearmiss='" + sid_nearmiss + "'";

                    DataSet dsNearMissList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNearMissList.Tables.Count > 0 && dsNearMissList.Tables[0].Rows.Count > 0)
                    {
                        objNearMiss = new NearMissModels
                        {
                            id_nearmiss = Convert.ToInt16(dsNearMissList.Tables[0].Rows[0]["id_nearmiss"].ToString()),
                            report_no = (dsNearMissList.Tables[0].Rows[0]["report_no"].ToString()),
                            reported_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[0]["reported_by"].ToString()),
                            reported_by_position = (dsNearMissList.Tables[0].Rows[0]["reported_by_position"].ToString()),
                            reported_by_dept = (dsNearMissList.Tables[0].Rows[0]["reported_by_dept"].ToString()),
                            location = objGlobaldata.GetCompanyBranchNameById(dsNearMissList.Tables[0].Rows[0]["location"].ToString()),
                            description = (dsNearMissList.Tables[0].Rows[0]["description"].ToString()),
                            upload = (dsNearMissList.Tables[0].Rows[0]["upload"].ToString()),
                            effect_incident = (dsNearMissList.Tables[0].Rows[0]["effect_incident"].ToString()),
                            reviewed_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            causes = objGlobaldata.GetDropdownitemById(dsNearMissList.Tables[0].Rows[0]["causes"].ToString()),
                            project = objGlobaldata.GetDropdownitemById(dsNearMissList.Tables[0].Rows[0]["project"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsNearMissList.Tables[0].Rows[0]["incident_date"].ToString(), out dtValue))
                        {
                            objNearMiss.incident_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNearMissList.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                        {
                            objNearMiss.reported_date = dtValue;
                        }
                    }

                    string SqlStmt = "select correction,action,hazard,verified_by,tt.reviewed_by from t_nearmiss t,t_nearmiss_action tt"
                    + " where t.id_nearmiss=tt.id_nearmiss and t.id_nearmiss='" + sid_nearmiss + "' and tt.Active=1";
                    DataSet dsNearMiss = objGlobaldata.Getdetails(SqlStmt);
                    ViewBag.ActionDetails = dsNearMiss;
                }
                else
                {
                    TempData["alertdata"] = "NearMiss Id cannot be Null or empty";
                    return RedirectToAction("NearMissList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NearMissDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNearMiss);
        }

        [AllowAnonymous]
        public ActionResult NearMissInfo(int id)
        {
            NearMissModels objNearMiss = new NearMissModels();

            try
            {
                string sSqlstmt = "select id_nearmiss,incident_date,reported_date,report_no,reported_by,reported_by_position,reported_by_dept,"
                 + "location,description,upload,effect_incident,reviewed_by,causes,project from t_nearmiss where id_nearmiss='" + id + "'";

                DataSet dsNearMissList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNearMissList.Tables.Count > 0 && dsNearMissList.Tables[0].Rows.Count > 0)
                {
                    objNearMiss = new NearMissModels
                    {
                        id_nearmiss = Convert.ToInt16(dsNearMissList.Tables[0].Rows[0]["id_nearmiss"].ToString()),
                        report_no = (dsNearMissList.Tables[0].Rows[0]["report_no"].ToString()),
                        reported_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[0]["reported_by"].ToString()),
                        reported_by_position = (dsNearMissList.Tables[0].Rows[0]["reported_by_position"].ToString()),
                        reported_by_dept = (dsNearMissList.Tables[0].Rows[0]["reported_by_dept"].ToString()),
                        location = objGlobaldata.GetCompanyBranchNameById(dsNearMissList.Tables[0].Rows[0]["location"].ToString()),
                        description = (dsNearMissList.Tables[0].Rows[0]["description"].ToString()),
                        upload = (dsNearMissList.Tables[0].Rows[0]["upload"].ToString()),
                        effect_incident = (dsNearMissList.Tables[0].Rows[0]["effect_incident"].ToString()),
                        reviewed_by = objGlobaldata.GetEmpHrNameById(dsNearMissList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                        causes = objGlobaldata.GetDropdownitemById(dsNearMissList.Tables[0].Rows[0]["causes"].ToString()),
                        project = objGlobaldata.GetDropdownitemById(dsNearMissList.Tables[0].Rows[0]["project"].ToString()),
                    };
                    DateTime dtValue;
                    if (DateTime.TryParse(dsNearMissList.Tables[0].Rows[0]["incident_date"].ToString(), out dtValue))
                    {
                        objNearMiss.incident_date = dtValue;
                    }
                    if (DateTime.TryParse(dsNearMissList.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                    {
                        objNearMiss.reported_date = dtValue;
                    }
                }

                string SqlStmt = "select correction,action,hazard,verified_by,tt.reviewed_by from t_nearmiss t,t_nearmiss_action tt"
                + " where t.id_nearmiss=tt.id_nearmiss and t.id_nearmiss='" + id + "' and tt.Active=1";
                DataSet dsNearMiss = objGlobaldata.Getdetails(SqlStmt);
                ViewBag.ActionDetails = dsNearMiss;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NearMissDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNearMiss);
        }

        [HttpPost]
        public JsonResult FunGetReportNo(string Location)
        {
            DataSet dsData; string RepNo = "";

            dsData = objGlobaldata.GetReportNo("NMR", "", Location);
            if (dsData != null && dsData.Tables.Count > 0)
            {
                RepNo = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
            }
            return Json(RepNo);
        }
    }
}