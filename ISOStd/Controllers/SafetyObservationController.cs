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
using Rotativa;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class SafetyObservationController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        SafetyObservationModels objSafeObs = new SafetyObservationModels();

        public SafetyObservationController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "SafetyObservation";
        }


        public ActionResult AddSafetyObservation()
        {
            try
            {
                ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
                ViewBag.ObservationType = objGlobaldata.GetDropdownList("Safety Observation Type");
                ViewBag.Observer = objGlobaldata.GetDropdownList("Safety Observation Observer");
                ViewBag.Status = objGlobaldata.GetDropdownList("Safety Observation Status");
                ViewBag.EmpList= objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Dept = objGlobaldata.GetDepartmentListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSafetyObservation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSafetyObservation(SafetyObservationModels objSafeObs, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;

                if (form["observation_date"] != null && DateTime.TryParse(form["observation_date"], out dateValue) == true)
                {
                    objSafeObs.observation_date = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objSafeObs.observation_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (upload != null && files.ContentLength > 0)
                {
                    objSafeObs.upload = "";
                    foreach (var file in upload)
                    {

                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Safety"), Path.GetFileName(file.FileName));
                            string sFilename = "Safety" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objSafeObs.upload = objSafeObs.upload + "," + "~/DataUpload/MgmtDocs/Safety/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddSafetyObservation-upload: " + ex.ToString());
                        }
                    }
                    objSafeObs.upload = objSafeObs.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objSafeObs.FunAddObservation(objSafeObs))
                {
                    TempData["Successdata"] = "Added Safety Observation Card details successfully with Permit No'" + objSafeObs.report_no + "'";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSafetyObservation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SafetyObservationList");
        }

        public ActionResult SafetyObservationList(string branch_name)
        {
            //ViewBag.SubMenutype = "FireEquipInspection";
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

            SafetyObservationModelsList objSafetyObsList = new SafetyObservationModelsList();
            objSafetyObsList.SafetyList = new List<SafetyObservationModels>();

            SafetyObservationModels objSafty = new SafetyObservationModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT id_safety_observation,observation_date, type_observation, desc_observation,action_taken, observed_by, " +
                    "reported_by, project, location,report_no,status,comments,dept,resp_person,target_date from t_safety_observation where active='1'";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsSaftyObservationList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsSaftyObservationList.Tables.Count > 0)
                {
                   

                    for (int i = 0; i < dsSaftyObservationList.Tables[0].Rows.Count; i++)
                    {
                       // DateTime dtDailyDate = Convert.ToDateTime(dsSaftyObservationList.Tables[0].Rows[i]["observation_date"].ToString());
                        DateTime dtValue;

                        try
                        {
                            objSafty = new SafetyObservationModels
                            {
                                id_safety_observation = dsSaftyObservationList.Tables[0].Rows[i]["id_safety_observation"].ToString(),
                                location = /*objGlobaldata.GetCompanyBranchNameById*/(dsSaftyObservationList.Tables[0].Rows[i]["location"].ToString()),
                                // observation_date = dtDailyDate,
                                type_observation = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[i]["type_observation"].ToString()),
                                desc_observation = dsSaftyObservationList.Tables[0].Rows[i]["desc_observation"].ToString(),
                                action_taken = dsSaftyObservationList.Tables[0].Rows[i]["action_taken"].ToString(),
                                observed_by = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[i]["observed_by"].ToString()),
                                reported_by = dsSaftyObservationList.Tables[0].Rows[i]["reported_by"].ToString(),
                                status = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[i]["status"].ToString()),
                                comments = dsSaftyObservationList.Tables[0].Rows[i]["comments"].ToString(),
                                project = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[i]["project"].ToString()),
                                report_no = dsSaftyObservationList.Tables[0].Rows[i]["report_no"].ToString(),
                                dept = objGlobaldata.GetDeptNameById(dsSaftyObservationList.Tables[0].Rows[i]["dept"].ToString()),
                                resp_person = objGlobaldata.GetMultiHrEmpNameById(dsSaftyObservationList.Tables[0].Rows[i]["resp_person"].ToString()),
                            };
                            if (DateTime.TryParse(dsSaftyObservationList.Tables[0].Rows[i]["observation_date"].ToString(), out dtValue))
                            {
                                objSafty.observation_date = dtValue;
                            }
                            if (DateTime.TryParse(dsSaftyObservationList.Tables[0].Rows[i]["target_date"].ToString(), out dtValue))
                            {
                                objSafty.target_date = dtValue;
                            }
                            objSafetyObsList.SafetyList.Add(objSafty);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SafetyObservationList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyObservationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objSafetyObsList.SafetyList.ToList());
        }

        public JsonResult SafetyObservationListSearch(string branch_name)
        {
            //ViewBag.SubMenutype = "FireEquipInspection";
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

            SafetyObservationModelsList objSafetyObsList = new SafetyObservationModelsList();
            objSafetyObsList.SafetyList = new List<SafetyObservationModels>();

            SafetyObservationModels objSafty = new SafetyObservationModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT id_safety_observation,observation_date, type_observation, desc_observation,action_taken, observed_by, " +
                    "reported_by, project, location,report_no,status,comments,dept,resp_person,target_date from t_safety_observation where active='1'";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsSaftyObservationList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsSaftyObservationList.Tables.Count > 0)
                {
                

                    for (int i = 0; i < dsSaftyObservationList.Tables[0].Rows.Count; i++)
                    {
                        // DateTime dtDailyDate = Convert.ToDateTime(dsSaftyObservationList.Tables[0].Rows[i]["observation_date"].ToString());
                        DateTime dtValue;

                        try
                        {
                            objSafty = new SafetyObservationModels
                            {
                                id_safety_observation = dsSaftyObservationList.Tables[0].Rows[i]["id_safety_observation"].ToString(),
                                location = /*objGlobaldata.GetCompanyBranchNameById*/(dsSaftyObservationList.Tables[0].Rows[i]["location"].ToString()),
                                // observation_date = dtDailyDate,
                                type_observation = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[i]["type_observation"].ToString()),
                                desc_observation = dsSaftyObservationList.Tables[0].Rows[i]["desc_observation"].ToString(),
                                action_taken = dsSaftyObservationList.Tables[0].Rows[i]["action_taken"].ToString(),
                                observed_by = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[i]["observed_by"].ToString()),
                                reported_by = dsSaftyObservationList.Tables[0].Rows[i]["reported_by"].ToString(),
                                status = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[i]["status"].ToString()),
                                comments = dsSaftyObservationList.Tables[0].Rows[i]["comments"].ToString(),
                                project = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[i]["project"].ToString()),
                                report_no = dsSaftyObservationList.Tables[0].Rows[i]["report_no"].ToString(),
                                dept = objGlobaldata.GetDeptNameById(dsSaftyObservationList.Tables[0].Rows[i]["dept"].ToString()),
                                resp_person = objGlobaldata.GetMultiHrEmpNameById(dsSaftyObservationList.Tables[0].Rows[i]["resp_person"].ToString()),
                            };
                            if (DateTime.TryParse(dsSaftyObservationList.Tables[0].Rows[i]["observation_date"].ToString(), out dtValue))
                            {
                                objSafty.observation_date = dtValue;
                            }
                            objSafetyObsList.SafetyList.Add(objSafty);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SafetyObservationListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyObservationListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        public ActionResult SafetyObservationEdit()
        {
            //ViewBag.SubMenutype = "FireEquipInspection";
            ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
            ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
            ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
            ViewBag.ObservationType = objGlobaldata.GetDropdownList("Safety Observation Type");
            ViewBag.Observer = objGlobaldata.GetDropdownList("Safety Observation Observer");
            ViewBag.Status = objGlobaldata.GetDropdownList("Safety Observation Status");
            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            ViewBag.Dept = objGlobaldata.GetDepartmentListbox();

            SafetyObservationModels objSafty = new SafetyObservationModels();
            try
            {
                if (Request.QueryString["id_safety_observation"] != null && Request.QueryString["id_safety_observation"] != "")
                {
                    string sid_safety_observation = Request.QueryString["id_safety_observation"];
                    
                    string sSqlstmt = "SELECT id_safety_observation,observation_date, type_observation, desc_observation,action_taken, observed_by, " +
                    "reported_by, project, location,report_no,status,comments,upload,dept,resp_person,target_date from t_safety_observation where id_safety_observation = '" + sid_safety_observation + "'";

                    DataSet dsSaftyObservationList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSaftyObservationList.Tables.Count > 0 && dsSaftyObservationList.Tables[0].Rows.Count > 0)
                    {
                        // DateTime dtDailyDate = Convert.ToDateTime(dsSaftyObservationList.Tables[0].Rows[0]["observation_date"].ToString());
                        DateTime dtValue;

                        objSafty = new SafetyObservationModels
                        {
                            id_safety_observation = dsSaftyObservationList.Tables[0].Rows[0]["id_safety_observation"].ToString(),
                            location = /*objGlobaldata.GetCompanyBranchNameById*/(dsSaftyObservationList.Tables[0].Rows[0]["location"].ToString()),
                            // observation_date = dtDailyDate,
                            type_observation = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["type_observation"].ToString()),
                            desc_observation = dsSaftyObservationList.Tables[0].Rows[0]["desc_observation"].ToString(),
                            action_taken = dsSaftyObservationList.Tables[0].Rows[0]["action_taken"].ToString(),
                            observed_by = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["observed_by"].ToString()),
                            reported_by = dsSaftyObservationList.Tables[0].Rows[0]["reported_by"].ToString(),
                            status = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["status"].ToString()),
                            comments = dsSaftyObservationList.Tables[0].Rows[0]["comments"].ToString(),
                            project = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["project"].ToString()),
                            report_no = dsSaftyObservationList.Tables[0].Rows[0]["report_no"].ToString(),
                            upload = dsSaftyObservationList.Tables[0].Rows[0]["upload"].ToString(),
                            dept = /*objGlobaldata.GetDeptNameById*/(dsSaftyObservationList.Tables[0].Rows[0]["dept"].ToString()),
                            resp_person = /*objGlobaldata.GetMultiHrEmpNameById*/(dsSaftyObservationList.Tables[0].Rows[0]["resp_person"].ToString()),
                        };
                        if (DateTime.TryParse(dsSaftyObservationList.Tables[0].Rows[0]["observation_date"].ToString(), out dtValue))
                        {
                            objSafty.observation_date = dtValue;
                        }
                        if (DateTime.TryParse(dsSaftyObservationList.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                        {
                            objSafty.target_date = dtValue;
                        }

                        return View(objSafty);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("SafetyObservationList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Safety Observation Id cannot be null";
                    return RedirectToAction("SafetyObservationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyObservationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SafetyObservationList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SafetyObservationEdit(SafetyObservationModels objSafty, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];
                DateTime dateValue;

                if (form["observation_date"] != null && DateTime.TryParse(form["observation_date"], out dateValue) == true)
                {
                    objSafty.observation_date = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objSafty.observation_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (upload != null && files.ContentLength > 0)
                {
                    objSafty.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Safety"), Path.GetFileName(file.FileName));
                            string sFilename = "Safety" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objSafty.upload = objSafty.upload + "," + "~/DataUpload/MgmtDocs/Safety/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SafetyObservationEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objSafty.upload = objSafty.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objSafty.upload = objSafty.upload + "," + form["QCDocsVal"];
                    objSafty.upload = objSafty.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objSafty.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objSafty.upload = null;
                }
                if (objSafty.FunUpdateObservation(objSafty))
                {
                    TempData["Successdata"] = "Updated Safety Observation Card details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyObservationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SafetyObservationList");
        }

        public ActionResult SafetyObservationDetails()
        {
            // ViewBag.SubMenutype = "FireEquipInspection";
            SafetyObservationModels objSafty = new SafetyObservationModels();

            try
            {
                if (Request.QueryString["id_safety_observation"] != null && Request.QueryString["id_safety_observation"] != "")
                {
                    string sid_safety_observation = Request.QueryString["id_safety_observation"];

                    string sSqlstmt = "SELECT id_safety_observation,observation_date, type_observation, desc_observation,action_taken, observed_by, " +
                    "reported_by, project, location,report_no,status,comments,upload,dept,resp_person,target_date from t_safety_observation where id_safety_observation = '" + sid_safety_observation + "'";

                    DataSet dsSaftyObservationList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSaftyObservationList.Tables.Count > 0 && dsSaftyObservationList.Tables[0].Rows.Count > 0)
                    {
                        // DateTime dtDailyDate = Convert.ToDateTime(dsSaftyObservationList.Tables[0].Rows[0]["observation_date"].ToString());
                        DateTime dtValue;

                        objSafty = new SafetyObservationModels
                        {
                            id_safety_observation = dsSaftyObservationList.Tables[0].Rows[0]["id_safety_observation"].ToString(),
                            location =/* objGlobaldata.GetCompanyBranchNameById*/(dsSaftyObservationList.Tables[0].Rows[0]["location"].ToString()),
                            // observation_date = dtDailyDate,
                            type_observation = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["type_observation"].ToString()),
                            desc_observation = dsSaftyObservationList.Tables[0].Rows[0]["desc_observation"].ToString(),
                            action_taken = dsSaftyObservationList.Tables[0].Rows[0]["action_taken"].ToString(),
                            observed_by = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["observed_by"].ToString()),
                            reported_by = dsSaftyObservationList.Tables[0].Rows[0]["reported_by"].ToString(),
                            status = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["status"].ToString()),
                            comments = dsSaftyObservationList.Tables[0].Rows[0]["comments"].ToString(),
                            project = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["project"].ToString()),
                            report_no = dsSaftyObservationList.Tables[0].Rows[0]["report_no"].ToString(),
                            upload = dsSaftyObservationList.Tables[0].Rows[0]["upload"].ToString(),
                            dept = objGlobaldata.GetDeptNameById(dsSaftyObservationList.Tables[0].Rows[0]["dept"].ToString()),
                            resp_person = objGlobaldata.GetMultiHrEmpNameById(dsSaftyObservationList.Tables[0].Rows[0]["resp_person"].ToString()),
                        };
                        if (DateTime.TryParse(dsSaftyObservationList.Tables[0].Rows[0]["observation_date"].ToString(), out dtValue))
                        {
                            objSafty.observation_date = dtValue;
                        }
                        if (DateTime.TryParse(dsSaftyObservationList.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                        {
                            objSafty.target_date = dtValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("SafetyObservationList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("SafetyObservationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyObservationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("SafetyObservationList");
            }
            return View(objSafty);
        }

        public ActionResult SafetyObservationInfo(int id)
        {
            SafetyObservationModels objSafty = new SafetyObservationModels();
            try
            {
                    string sSqlstmt = "SELECT id_safety_observation,observation_date, type_observation, desc_observation,action_taken, observed_by, " +
                    "reported_by, project, location,report_no,status,comments,dept,resp_person,target_date from t_safety_observation where id_safety_observation = '" + id + "'";

                    DataSet dsSaftyObservationList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSaftyObservationList.Tables.Count > 0 && dsSaftyObservationList.Tables[0].Rows.Count > 0)
                    {
                        // DateTime dtDailyDate = Convert.ToDateTime(dsSaftyObservationList.Tables[0].Rows[0]["observation_date"].ToString());
                        DateTime dtValue;

                        objSafty = new SafetyObservationModels
                        {
                            id_safety_observation = dsSaftyObservationList.Tables[0].Rows[0]["id_safety_observation"].ToString(),
                            location = /*objGlobaldata.GetCompanyBranchNameById*/(dsSaftyObservationList.Tables[0].Rows[0]["location"].ToString()),
                            // observation_date = dtDailyDate,
                            type_observation = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["type_observation"].ToString()),
                            desc_observation = dsSaftyObservationList.Tables[0].Rows[0]["desc_observation"].ToString(),
                            action_taken = dsSaftyObservationList.Tables[0].Rows[0]["action_taken"].ToString(),
                            observed_by = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["observed_by"].ToString()),
                            reported_by = dsSaftyObservationList.Tables[0].Rows[0]["reported_by"].ToString(),
                            status = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["status"].ToString()),
                            comments = dsSaftyObservationList.Tables[0].Rows[0]["comments"].ToString(),
                            project = objGlobaldata.GetDropdownitemById(dsSaftyObservationList.Tables[0].Rows[0]["project"].ToString()),
                            report_no = dsSaftyObservationList.Tables[0].Rows[0]["report_no"].ToString(),
                            dept = objGlobaldata.GetDeptNameById(dsSaftyObservationList.Tables[0].Rows[0]["dept"].ToString()),
                            resp_person = objGlobaldata.GetMultiHrEmpNameById(dsSaftyObservationList.Tables[0].Rows[0]["resp_person"].ToString()),
                        };
                        if (DateTime.TryParse(dsSaftyObservationList.Tables[0].Rows[0]["observation_date"].ToString(), out dtValue))
                        {
                            objSafty.observation_date = dtValue;
                        }
                    if (DateTime.TryParse(dsSaftyObservationList.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                    {
                        objSafty.target_date = dtValue;
                    }

                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("SafetyObservationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyObservationInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objSafty);
        }

        [AllowAnonymous]
        public JsonResult SafetyObservationDelete(FormCollection form)
        {
            try
            {
                   if (form["id_safety_observation"] != null && form["id_safety_observation"] != "")
                    {

                        SafetyObservationModels Doc = new SafetyObservationModels();
                        string sid_safety_observation = form["id_safety_observation"];

                        if (Doc.FunDeleteObservation(sid_safety_observation))
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
                        TempData["alertdata"] = "Safety Observation Card Id cannot be Null or empty";
                        return Json("Failed");
                    }
               

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyObservationDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult FunGetReportNo(string Location)
        {

            DataSet dsData; string RepNo = "";

            dsData = objGlobaldata.GetReportNo("SOC","", Location);
            if (dsData != null && dsData.Tables.Count > 0)
            {
                RepNo = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
            }
            return Json(RepNo);

        }

    }
}