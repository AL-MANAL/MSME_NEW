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
    public class HazardNotificationController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();
        //HazardNotificationModels objHazard = new HazardNotificationModels();

        public HazardNotificationController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "HazardNotification";
        }

        public ActionResult AddHazardNotification()
        {
            HazardNotificationModels objHazard = new HazardNotificationModels();
            try
            {
                objHazard.branch = objGlobaldata.GetCurrentUserSession().division;
                objHazard.dept = objGlobaldata.GetCurrentUserSession().DeptID;
                objHazard.location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objHazard.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objHazard.branch);
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.CMR = objGlobaldata.GetCMR();
                ViewBag.DMR = objGlobaldata.GetDMR();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.ActivityType = objGlobaldata.GetDropdownList("Hazard Notification Activity Type");
                ViewBag.RiskLevel = objGlobaldata.GetDropdownList("Hazard Notification Risk Level");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHazardNotification: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objHazard);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHazardNotification(HazardNotificationModels objHazard, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                //ViewBag.ActivityType = objGlobaldata.GetHazardActivityTypeList();
                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;

                if (form["notify_date"] != null && DateTime.TryParse(form["notify_date"], out dateValue) == true)
                {
                    objHazard.notify_date = dateValue;
                }
                objHazard.activity_type = form["activity_type"];
                objHazard.branch = form["branch"];
                objHazard.dept = form["dept"];
                objHazard.location = form["location"];
                //if(ViewBag.ActivityType != null)
                //{
                //    foreach (string item in ViewBag.ActivityType)
                //    {
                //        if (form["chk" + item] != null)
                //        {
                //            objHazard.activity_type = objHazard.activity_type + item + ",";
                //        }
                //    }
                //}
                //if (objHazard.activity_type != null && objHazard.activity_type != "")
                //{
                //    objHazard.activity_type = objHazard.activity_type.TrimEnd(',');
                //}
                if (upload != null && files.ContentLength > 0)
                {
                    objHazard.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "Hazard" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objHazard.upload = objHazard.upload + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddHazardNotification-upload: " + ex.ToString());
                        }
                    }
                    objHazard.upload = objHazard.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objHazard.FunAddHazardNotify(objHazard))
                {
                    objHazard.SendHazardNotifyEmailbyDMR("Hazard Nofification Email");
                    TempData["Successdata"] = "Added Hazard notification details successfully with Permit No'" + objHazard.control_no + "'";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHazardNotification: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HazardNotificationList");
        }

        public ActionResult HazardNotificationList(string branch_name)
        {
            //ViewBag.SubMenutype = "FireEquipInspection";
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

            HazardNotificationModelsList objList = new HazardNotificationModelsList();
            objList.NofifyList = new List<HazardNotificationModels>();

            HazardNotificationModels objHazard = new HazardNotificationModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT id_notification,control_no, observed_by, reported_by,notify_date, dept,branch,location,activity_type,description, " +
                    "hazard_aspect, condition_impact, affected_env,upload,reviewed_by,level_risk,work_stopped,control_measure,incorporated_HIRA,incorporated_by,Hazard_loc from t_hazard_notification where active='1'";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsHazardList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsHazardList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsHazardList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objHazard = new HazardNotificationModels
                            {
                                id_notification = dsHazardList.Tables[0].Rows[i]["id_notification"].ToString(),
                                control_no = dsHazardList.Tables[0].Rows[i]["control_no"].ToString(),
                                observed_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[i]["observed_by"].ToString()),
                                reported_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[i]["reported_by"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardList.Tables[0].Rows[i]["branch"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsHazardList.Tables[0].Rows[i]["dept"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsHazardList.Tables[0].Rows[i]["location"].ToString()),
                                activity_type = objGlobaldata.GetDropdownitemById(dsHazardList.Tables[0].Rows[i]["activity_type"].ToString()),
                                description = dsHazardList.Tables[0].Rows[i]["description"].ToString(),
                                hazard_aspect = dsHazardList.Tables[0].Rows[i]["hazard_aspect"].ToString(),
                                condition_impact = dsHazardList.Tables[0].Rows[i]["condition_impact"].ToString(),
                                affected_env = dsHazardList.Tables[0].Rows[i]["affected_env"].ToString(),
                                upload = dsHazardList.Tables[0].Rows[i]["upload"].ToString(),
                                reviewed_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[i]["reviewed_by"].ToString()),
                                level_risk = dsHazardList.Tables[0].Rows[i]["level_risk"].ToString(),
                                work_stopped = dsHazardList.Tables[0].Rows[i]["work_stopped"].ToString(),
                                control_measure = dsHazardList.Tables[0].Rows[i]["control_measure"].ToString(),
                                incorporated_HIRA = dsHazardList.Tables[0].Rows[i]["incorporated_HIRA"].ToString(),
                                incorporated_by = dsHazardList.Tables[0].Rows[i]["incorporated_by"].ToString(),
                                Hazard_loc = dsHazardList.Tables[0].Rows[i]["Hazard_loc"].ToString(),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsHazardList.Tables[0].Rows[i]["notify_date"].ToString(), out dtValue))
                            {
                                objHazard.notify_date = dtValue;
                            }

                            objList.NofifyList.Add(objHazard);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in HazardNotificationList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardNotificationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objList.NofifyList.ToList());
        }

        public JsonResult HazardNotificationListSearch(string branch_name)
        {
            //ViewBag.SubMenutype = "FireEquipInspection";
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

            HazardNotificationModelsList objList = new HazardNotificationModelsList();
            objList.NofifyList = new List<HazardNotificationModels>();

            HazardNotificationModels objHazard = new HazardNotificationModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT id_notification,control_no, observed_by, reported_by,notify_date, dept,branch,location,activity_type,description, " +
                    "hazard_aspect, condition_impact, affected_env,upload,reviewed_by,level_risk,work_stopped,control_measure,incorporated_HIRA,incorporated_by,Hazard_loc from t_hazard_notification where active='1'";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsHazardList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsHazardList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsHazardList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objHazard = new HazardNotificationModels
                            {
                                id_notification = dsHazardList.Tables[0].Rows[i]["id_notification"].ToString(),
                                control_no = dsHazardList.Tables[0].Rows[i]["control_no"].ToString(),
                                observed_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[i]["observed_by"].ToString()),
                                reported_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[i]["reported_by"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardList.Tables[0].Rows[i]["branch"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsHazardList.Tables[0].Rows[i]["dept"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsHazardList.Tables[0].Rows[i]["location"].ToString()),
                                activity_type = objGlobaldata.GetDropdownitemById(dsHazardList.Tables[0].Rows[i]["activity_type"].ToString()),
                                description = dsHazardList.Tables[0].Rows[i]["description"].ToString(),
                                hazard_aspect = dsHazardList.Tables[0].Rows[i]["hazard_aspect"].ToString(),
                                condition_impact = dsHazardList.Tables[0].Rows[i]["condition_impact"].ToString(),
                                affected_env = dsHazardList.Tables[0].Rows[i]["affected_env"].ToString(),
                                upload = dsHazardList.Tables[0].Rows[i]["upload"].ToString(),
                                reviewed_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[i]["reviewed_by"].ToString()),
                                level_risk = dsHazardList.Tables[0].Rows[i]["level_risk"].ToString(),
                                work_stopped = dsHazardList.Tables[0].Rows[i]["work_stopped"].ToString(),
                                control_measure = dsHazardList.Tables[0].Rows[i]["control_measure"].ToString(),
                                incorporated_HIRA = dsHazardList.Tables[0].Rows[i]["incorporated_HIRA"].ToString(),
                                incorporated_by = dsHazardList.Tables[0].Rows[i]["incorporated_by"].ToString(),
                                Hazard_loc = dsHazardList.Tables[0].Rows[i]["Hazard_loc"].ToString(),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsHazardList.Tables[0].Rows[i]["notify_date"].ToString(), out dtValue))
                            {
                                objHazard.notify_date = dtValue;
                            }

                            objList.NofifyList.Add(objHazard);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in HazardNotificationListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardNotificationListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        public ActionResult HazardNotificationEdit()
        {
            //ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
            //ViewBag.Department = objGlobaldata.GetDepartmentListbox();
            //ViewBag.Division = objGlobaldata.GetDivision();
            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            ViewBag.CMR = objGlobaldata.GetCMR();
            ViewBag.DMR = objGlobaldata.GetDMR();
            ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
            ViewBag.ActivityType = objGlobaldata.GetDropdownList("Hazard Notification Activity Type");
            ViewBag.RiskLevel = objGlobaldata.GetDropdownList("Hazard Notification Risk Level");

            HazardNotificationModels objHazard = new HazardNotificationModels();
            try
            {
                if (Request.QueryString["id_notification"] != null && Request.QueryString["id_notification"] != "")
                {
                    string sid_notification = Request.QueryString["id_notification"];

                    string sSqlstmt = "SELECT id_notification,control_no, observed_by, reported_by,notify_date, dept,branch,location,activity_type,description, " +
                    "hazard_aspect, condition_impact, affected_env,upload,reviewed_by,level_risk,work_stopped,control_measure,incorporated_HIRA,Hazard_loc from t_hazard_notification " +
                    "where id_notification = '" + sid_notification + "'";

                    DataSet dsHazardList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsHazardList.Tables.Count > 0 && dsHazardList.Tables[0].Rows.Count > 0)
                    {
                        // DateTime dtDailyDate = Convert.ToDateTime(dsHazardList.Tables[0].Rows[0]["observation_date"].ToString());
                        DateTime dtValue;

                        objHazard = new HazardNotificationModels
                        {
                            id_notification = dsHazardList.Tables[0].Rows[0]["id_notification"].ToString(),
                            control_no = dsHazardList.Tables[0].Rows[0]["control_no"].ToString(),
                            observed_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()),
                            reported_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString()),
                            branch = (dsHazardList.Tables[0].Rows[0]["branch"].ToString()),
                            dept = (dsHazardList.Tables[0].Rows[0]["dept"].ToString()),
                            location = (dsHazardList.Tables[0].Rows[0]["location"].ToString()),
                            activity_type =/* objGlobaldata.GetDropdownitemById*/(dsHazardList.Tables[0].Rows[0]["activity_type"].ToString()),
                            description = dsHazardList.Tables[0].Rows[0]["description"].ToString(),
                            hazard_aspect = dsHazardList.Tables[0].Rows[0]["hazard_aspect"].ToString(),
                            condition_impact = dsHazardList.Tables[0].Rows[0]["condition_impact"].ToString(),
                            affected_env = dsHazardList.Tables[0].Rows[0]["affected_env"].ToString(),
                            upload = dsHazardList.Tables[0].Rows[0]["upload"].ToString(),
                            reviewed_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsHazardList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            Hazard_loc = dsHazardList.Tables[0].Rows[0]["Hazard_loc"].ToString(),
                        };
                        if (DateTime.TryParse(dsHazardList.Tables[0].Rows[0]["notify_date"].ToString(), out dtValue))
                        {
                            objHazard.notify_date = dtValue;
                        }
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsHazardList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsHazardList.Tables[0].Rows[0]["branch"].ToString());

                        return View(objHazard);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("HazardNotificationList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Hazard Notification Id cannot be null";
                    return RedirectToAction("HazardNotificationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardNotificationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HazardNotificationList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HazardNotificationEdit(HazardNotificationModels objHazard, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];
                DateTime dateValue;

                if (form["notify_date"] != null && DateTime.TryParse(form["notify_date"], out dateValue) == true)
                {
                    objHazard.notify_date = dateValue;
                }

                objHazard.activity_type = form["activity_type"];
                objHazard.branch = form["branch"];
                objHazard.dept = form["dept"];
                objHazard.location = form["location"];

                if (upload != null && files.ContentLength > 0)
                {
                    objHazard.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "Hazard" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objHazard.upload = objHazard.upload + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in HazardNotificationEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objHazard.upload = objHazard.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objHazard.upload = objHazard.upload + "," + form["QCDocsVal"];
                    objHazard.upload = objHazard.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objHazard.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objHazard.upload = null;
                }
                if (objHazard.FunUpdateHazardNotify(objHazard))
                {
                    TempData["Successdata"] = "Updated  Hazard Notification details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardNotificationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HazardNotificationList");
        }

        public ActionResult HazardNotificationDMREdit()
        {
            // ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
            // ViewBag.Department = objGlobaldata.GetDepartmentListbox();
            // ViewBag.Division = objGlobaldata.GetDivision();
            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            //  ViewBag.CMR = objGlobaldata.GetCMR();
            //  ViewBag.DMR = objGlobaldata.GetDMR();
            ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
            ViewBag.RiskLevel = objGlobaldata.GetDropdownList("Hazard Notification Risk Level");

            HazardNotificationModels objHazard = new HazardNotificationModels();
            try
            {
                if (Request.QueryString["id_notification"] != null && Request.QueryString["id_notification"] != "")
                {
                    string sid_notification = Request.QueryString["id_notification"];

                    string sSqlstmt = "SELECT id_notification,control_no, observed_by, reported_by,notify_date, dept,branch,location,activity_type,description, " +
                    "hazard_aspect, condition_impact, affected_env,upload,reviewed_by,level_risk,work_stopped,control_measure,incorporated_HIRA,incorporated_by," +
                    "incorporated_date,cmr_name,dept_head from t_hazard_notification " +
                    "where id_notification = '" + sid_notification + "'";

                    DataSet dsHazardList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsHazardList.Tables.Count > 0 && dsHazardList.Tables[0].Rows.Count > 0)
                    {
                        // DateTime dtDailyDate = Convert.ToDateTime(dsHazardList.Tables[0].Rows[0]["observation_date"].ToString());
                        DateTime dtValue;

                        objHazard = new HazardNotificationModels
                        {
                            id_notification = dsHazardList.Tables[0].Rows[0]["id_notification"].ToString(),
                            //control_no = dsHazardList.Tables[0].Rows[0]["control_no"].ToString(),
                            //observed_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()),
                            //reported_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString()),
                            //activity_type =/* objGlobaldata.GetDropdownitemById*/(dsHazardList.Tables[0].Rows[0]["activity_type"].ToString()),
                            //description = dsHazardList.Tables[0].Rows[0]["description"].ToString(),
                            //hazard_aspect = dsHazardList.Tables[0].Rows[0]["hazard_aspect"].ToString(),
                            //condition_impact = dsHazardList.Tables[0].Rows[0]["condition_impact"].ToString(),
                            //affected_env = dsHazardList.Tables[0].Rows[0]["affected_env"].ToString(),
                            //upload = dsHazardList.Tables[0].Rows[0]["upload"].ToString(),
                            //reviewed_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsHazardList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            level_risk = dsHazardList.Tables[0].Rows[0]["level_risk"].ToString(),
                            work_stopped = dsHazardList.Tables[0].Rows[0]["work_stopped"].ToString(),
                            control_measure = dsHazardList.Tables[0].Rows[0]["control_measure"].ToString(),
                            incorporated_HIRA = dsHazardList.Tables[0].Rows[0]["incorporated_HIRA"].ToString(),
                            incorporated_by = dsHazardList.Tables[0].Rows[0]["incorporated_by"].ToString(),
                            dept_head = dsHazardList.Tables[0].Rows[0]["dept_head"].ToString(),
                            cmr_name = dsHazardList.Tables[0].Rows[0]["cmr_name"].ToString(),
                        };
                        if (DateTime.TryParse(dsHazardList.Tables[0].Rows[0]["incorporated_date"].ToString(), out dtValue))
                        {
                            objHazard.incorporated_date = dtValue;
                        }
                        return View(objHazard);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("HazardNotificationList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Hazard Notification Id cannot be null";
                    return RedirectToAction("HazardNotificationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardNotificationDMREdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HazardNotificationList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HazardNotificationDMREdit(HazardNotificationModels objHazard, FormCollection form)
        {
            try
            {
                DateTime dateValue;

                if (form["incorporated_date"] != null && DateTime.TryParse(form["incorporated_date"], out dateValue) == true)
                {
                    objHazard.incorporated_date = dateValue;
                }

                if (objHazard.FunUpdateHazardNotifyDMR(objHazard))
                {
                    objHazard.SendHazardNotifyEmailFromDMR(objHazard.id_notification, "Hazard Notification");
                    TempData["Successdata"] = "Updated  Hazard Notification details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardNotificationDMREdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HazardNotificationList");
        }

        public ActionResult HazardNotificationDetails()
        {
            HazardNotificationModels objHazard = new HazardNotificationModels();
            try
            {
                if (Request.QueryString["id_notification"] != null && Request.QueryString["id_notification"] != "")
                {
                    string sid_notification = Request.QueryString["id_notification"];

                    string sSqlstmt = "SELECT id_notification,control_no, observed_by, reported_by,notify_date, dept,branch,location,activity_type,description, " +
                    "hazard_aspect, condition_impact, affected_env,upload,reviewed_by,level_risk,work_stopped,control_measure,incorporated_HIRA," +
                    "incorporated_by,incorporated_date,dept_head,cmr_name,Hazard_loc from t_hazard_notification " +
                    "where id_notification = '" + sid_notification + "'";

                    DataSet dsHazardList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsHazardList.Tables.Count > 0 && dsHazardList.Tables[0].Rows.Count > 0)
                    {
                        // DateTime dtDailyDate = Convert.ToDateTime(dsHazardList.Tables[0].Rows[0]["observation_date"].ToString());
                        DateTime dtValue;

                        objHazard = new HazardNotificationModels
                        {
                            id_notification = dsHazardList.Tables[0].Rows[0]["id_notification"].ToString(),
                            control_no = dsHazardList.Tables[0].Rows[0]["control_no"].ToString(),
                            observed_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()),
                            reported_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardList.Tables[0].Rows[0]["branch"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsHazardList.Tables[0].Rows[0]["dept"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsHazardList.Tables[0].Rows[0]["location"].ToString()),
                            activity_type = objGlobaldata.GetDropdownitemById(dsHazardList.Tables[0].Rows[0]["activity_type"].ToString()),
                            description = dsHazardList.Tables[0].Rows[0]["description"].ToString(),
                            hazard_aspect = dsHazardList.Tables[0].Rows[0]["hazard_aspect"].ToString(),
                            condition_impact = dsHazardList.Tables[0].Rows[0]["condition_impact"].ToString(),
                            affected_env = dsHazardList.Tables[0].Rows[0]["affected_env"].ToString(),
                            upload = dsHazardList.Tables[0].Rows[0]["upload"].ToString(),
                            reviewed_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            level_risk = objGlobaldata.GetDropdownitemById(dsHazardList.Tables[0].Rows[0]["level_risk"].ToString()),
                            work_stopped = dsHazardList.Tables[0].Rows[0]["work_stopped"].ToString(),
                            control_measure = dsHazardList.Tables[0].Rows[0]["control_measure"].ToString(),
                            incorporated_HIRA = dsHazardList.Tables[0].Rows[0]["incorporated_HIRA"].ToString(),
                            incorporated_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["incorporated_by"].ToString()),
                            dept_head = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["dept_head"].ToString()),
                            cmr_name = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["cmr_name"].ToString()),
                            Hazard_loc = dsHazardList.Tables[0].Rows[0]["Hazard_loc"].ToString(),
                        };
                        if (DateTime.TryParse(dsHazardList.Tables[0].Rows[0]["notify_date"].ToString(), out dtValue))
                        {
                            objHazard.notify_date = dtValue;
                        }
                        if (DateTime.TryParse(dsHazardList.Tables[0].Rows[0]["incorporated_date"].ToString(), out dtValue))
                        {
                            objHazard.incorporated_date = dtValue;
                        }
                        return View(objHazard);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("HazardNotificationList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Hazard Notification Id cannot be null";
                    return RedirectToAction("HazardNotificationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardNotificationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HazardNotificationList");
        }

        public ActionResult HazardNotificationPDF(FormCollection form)
        {
            CompanyModels objCompany = new CompanyModels();
            HazardNotificationModels objHazard = new HazardNotificationModels();
            try
            {
                if (form["id_notification"] != null && form["id_notification"] != "")
                {
                    string sid_notification = form["id_notification"];

                    string sSqlstmt = "SELECT id_notification,control_no, observed_by, reported_by,notify_date, dept,branch,location,activity_type,description, " +
                    "hazard_aspect, condition_impact, affected_env,upload,reviewed_by,level_risk,work_stopped,control_measure,incorporated_HIRA," +
                    "incorporated_by,incorporated_date,dept_head,cmr_name,Hazard_loc,logged_by from t_hazard_notification " +
                    "where id_notification = '" + sid_notification + "'";

                    DataSet dsHazardList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsHazardList.Tables.Count > 0 && dsHazardList.Tables[0].Rows.Count > 0)
                    {
                        // DateTime dtDailyDate = Convert.ToDateTime(dsHazardList.Tables[0].Rows[0]["observation_date"].ToString());
                        DateTime dtValue;

                        objHazard = new HazardNotificationModels
                        {
                            id_notification = dsHazardList.Tables[0].Rows[0]["id_notification"].ToString(),
                            control_no = dsHazardList.Tables[0].Rows[0]["control_no"].ToString(),
                            observed_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()),
                            reported_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardList.Tables[0].Rows[0]["branch"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsHazardList.Tables[0].Rows[0]["dept"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsHazardList.Tables[0].Rows[0]["location"].ToString()),
                            activity_type = objGlobaldata.GetDropdownitemById(dsHazardList.Tables[0].Rows[0]["activity_type"].ToString()),
                            description = dsHazardList.Tables[0].Rows[0]["description"].ToString(),
                            hazard_aspect = dsHazardList.Tables[0].Rows[0]["hazard_aspect"].ToString(),
                            condition_impact = dsHazardList.Tables[0].Rows[0]["condition_impact"].ToString(),
                            affected_env = dsHazardList.Tables[0].Rows[0]["affected_env"].ToString(),
                            upload = dsHazardList.Tables[0].Rows[0]["upload"].ToString(),
                            reviewed_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            level_risk = objGlobaldata.GetDropdownitemById(dsHazardList.Tables[0].Rows[0]["level_risk"].ToString()),
                            work_stopped = dsHazardList.Tables[0].Rows[0]["work_stopped"].ToString(),
                            control_measure = dsHazardList.Tables[0].Rows[0]["control_measure"].ToString(),
                            incorporated_HIRA = dsHazardList.Tables[0].Rows[0]["incorporated_HIRA"].ToString(),
                            incorporated_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["incorporated_by"].ToString()),
                            dept_head = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["dept_head"].ToString()),
                            cmr_name = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["cmr_name"].ToString()),
                            Hazard_loc = dsHazardList.Tables[0].Rows[0]["Hazard_loc"].ToString(),
                            logged_by = dsHazardList.Tables[0].Rows[0]["logged_by"].ToString(),
                        };
                        if (DateTime.TryParse(dsHazardList.Tables[0].Rows[0]["notify_date"].ToString(), out dtValue))
                        {
                            objHazard.notify_date = dtValue;
                        }
                        if (DateTime.TryParse(dsHazardList.Tables[0].Rows[0]["incorporated_date"].ToString(), out dtValue))
                        {
                            objHazard.incorporated_date = dtValue;
                        }
                        ViewBag.Hazard = objHazard;
                        dsHazardList = objCompany.GetCompanyDetailsForReport(dsHazardList);
                        dsHazardList = objGlobaldata.GetReportDetails(dsHazardList, objHazard.control_no, dsHazardList.Tables[0].Rows[0]["logged_by"].ToString(), "HSE HAZARD/ASPECT NOTIFICATION");

                        ViewBag.CompanyInfo = dsHazardList;
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("HazardNotificationList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Hazard Notification Id cannot be null";
                    return RedirectToAction("HazardNotificationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardNotificationPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            //string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\" --margin-bottom \"18\" ";

            return new ViewAsPdf("HazardNotificationPDF")
            {
                //FileName = "HazardNotificationPDF.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        public ActionResult HazardNotificationInfo(int id)
        {
            HazardNotificationModels objHazard = new HazardNotificationModels();
            try
            {
                string sSqlstmt = "SELECT id_notification,control_no, observed_by, reported_by,notify_date, dept,branch,location,activity_type,description, " +
                "hazard_aspect, condition_impact, affected_env,upload,reviewed_by,level_risk,work_stopped,control_measure,incorporated_HIRA," +
                "incorporated_by,incorporated_date,Hazard_loc from t_hazard_notification " +
                "where id_notification = '" + id + "'";

                DataSet dsHazardList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsHazardList.Tables.Count > 0 && dsHazardList.Tables[0].Rows.Count > 0)
                {
                    // DateTime dtDailyDate = Convert.ToDateTime(dsHazardList.Tables[0].Rows[0]["observation_date"].ToString());
                    DateTime dtValue;

                    objHazard = new HazardNotificationModels
                    {
                        id_notification = dsHazardList.Tables[0].Rows[0]["id_notification"].ToString(),
                        control_no = dsHazardList.Tables[0].Rows[0]["control_no"].ToString(),
                        observed_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()),
                        reported_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString()),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardList.Tables[0].Rows[0]["branch"].ToString()),
                        dept = objGlobaldata.GetMultiDeptNameById(dsHazardList.Tables[0].Rows[0]["dept"].ToString()),
                        location = objGlobaldata.GetDivisionLocationById(dsHazardList.Tables[0].Rows[0]["location"].ToString()),
                        activity_type = objGlobaldata.GetDropdownitemById(dsHazardList.Tables[0].Rows[0]["activity_type"].ToString()),
                        description = dsHazardList.Tables[0].Rows[0]["description"].ToString(),
                        hazard_aspect = dsHazardList.Tables[0].Rows[0]["hazard_aspect"].ToString(),
                        condition_impact = dsHazardList.Tables[0].Rows[0]["condition_impact"].ToString(),
                        affected_env = dsHazardList.Tables[0].Rows[0]["affected_env"].ToString(),
                        upload = dsHazardList.Tables[0].Rows[0]["upload"].ToString(),
                        reviewed_by = objGlobaldata.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                        level_risk = dsHazardList.Tables[0].Rows[0]["level_risk"].ToString(),
                        work_stopped = dsHazardList.Tables[0].Rows[0]["work_stopped"].ToString(),
                        control_measure = dsHazardList.Tables[0].Rows[0]["control_measure"].ToString(),
                        incorporated_HIRA = dsHazardList.Tables[0].Rows[0]["incorporated_HIRA"].ToString(),
                        incorporated_by = dsHazardList.Tables[0].Rows[0]["incorporated_by"].ToString(),
                        Hazard_loc = dsHazardList.Tables[0].Rows[0]["Hazard_loc"].ToString(),
                    };
                    if (DateTime.TryParse(dsHazardList.Tables[0].Rows[0]["notify_date"].ToString(), out dtValue))
                    {
                        objHazard.notify_date = dtValue;
                    }
                    if (DateTime.TryParse(dsHazardList.Tables[0].Rows[0]["incorporated_date"].ToString(), out dtValue))
                    {
                        objHazard.incorporated_date = dtValue;
                    }
                    return View(objHazard);
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("HazardNotificationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardNotificationInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HazardNotificationList");
        }

        [AllowAnonymous]
        public JsonResult HazardNotificationDelete(FormCollection form)
        {
            try
            {
                if (form["id_notification"] != null && form["id_notification"] != "")
                {
                    HazardNotificationModels Doc = new HazardNotificationModels();
                    string sid_notification = form["id_notification"];

                    if (Doc.FunDeleteHazardNotify(sid_notification))
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
                    TempData["alertdata"] = "Hazard Notification Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardNotificationDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }
}