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
    public class FireEquipInspectionController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();
        private FireEquipInspectionModels objFireEquip = new FireEquipInspectionModels();

        public FireEquipInspectionController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "FireEquipInspection";
        }

        public ActionResult FireEquipInspectionAdd()
        {
            return InitilizeFireEquip();
        }

        private ActionResult InitilizeFireEquip()
        {
            try
            {
                ViewBag.FireLocation = objGlobaldata.GetDropdownList("FireEquip-Location");
                ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.FireType = objFireEquip.GetFireEquipType();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeFireEquip: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FireEquipInspectionAdd(FireEquipInspectionModels objFireEquip, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                DateTime dateValue;

                if (form["Fire_DateTime"] != null && DateTime.TryParse(form["Fire_DateTime"], out dateValue) == true)
                {
                    objFireEquip.Fire_DateTime = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objFireEquip.Fire_DateTime = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }

                if (DateTime.TryParse(form["FireNext_DateTime"], out dateValue) == true)
                {
                    objFireEquip.FireNext_DateTime = dateValue;
                }
                HttpPostedFileBase files = Request.Files[0];

                if (upload != null && files.ContentLength > 0)
                {
                    objFireEquip.upload = "";
                    foreach (var document in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/FireInsp"), Path.GetFileName(document.FileName));
                            string sFilename = "FireInsp" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            document.SaveAs(sFilepath + "/" + sFilename);
                            objFireEquip.upload = objFireEquip.upload + "," + "~/DataUpload/MgmtDocs/FireInsp/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in FireEquipInspectionAdd-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objFireEquip.upload = objFireEquip.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objFireEquip.FunAddFireEquip(objFireEquip))
                {
                    TempData["Successdata"] = "Added Fire Equiqment Inspection details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddFireEquipInspection: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("FireEquipInspectionList");
        }

        public ActionResult FireEquipInspectionList(string branch_name)
        {
            ViewBag.SubMenutype = "FireEquipInspection";
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

            FireEquipInspectionModelsList objFireEquipList = new FireEquipInspectionModelsList();
            objFireEquipList.FireEquipList = new List<FireEquipInspectionModels>();

            FireEquipInspectionModels objType = new FireEquipInspectionModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT id_FireEquip,Fire_DateTime, Location, FireNext_DateTime,Smoke_Detector, Smoke_Detector_Remarks, " +
                    "Fire_Alarm, Fire_Alarm_Remarks, Fire_Water_Pumps,Fire_Water_Pumps_Remarks,Fire_Box_No,Fire_Box_No_Location,Fire_Box_No_Type_Remarks,Fire_Box_No_Type,Fire_Box_No_Remarks," +
                    "Fire_Hydrants,Fire_Hydrants_Remarks,Project,ReportNo,upload from t_fireequip_inspection where Active='1'";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "";

                DataSet dsFireEquipInspectionList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsFireEquipInspectionList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsFireEquipInspectionList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtDailyDate = Convert.ToDateTime(dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_DateTime"].ToString());
                        DateTime dtValue;

                        try
                        {
                            FireEquipInspectionModels objFireInspectionModels = new FireEquipInspectionModels
                            {
                                id_FireEquip = dsFireEquipInspectionList.Tables[0].Rows[i]["id_FireEquip"].ToString(),
                                Location = objGlobaldata.GetCompanyBranchNameById(dsFireEquipInspectionList.Tables[0].Rows[i]["Location"].ToString()),
                                Fire_DateTime = dtDailyDate,
                                Smoke_Detector = dsFireEquipInspectionList.Tables[0].Rows[i]["Smoke_Detector"].ToString(),
                                Smoke_Detector_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Smoke_Detector_Remarks"].ToString(),
                                Fire_Alarm = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Alarm"].ToString(),
                                Fire_Alarm_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Alarm_Remarks"].ToString(),
                                Fire_Water_Pumps = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Water_Pumps"].ToString(),
                                Fire_Water_Pumps_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Water_Pumps_Remarks"].ToString(),
                                Fire_Box_No_Location = objGlobaldata.GetDropdownitemById(dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Box_No_Location"].ToString()),
                                Fire_Box_No_Type = objFireEquip.GetFireEquipTypeById(dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Box_No_Type"].ToString()),
                                Fire_Box_No = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Box_No"].ToString(),
                                Fire_Box_No_Type_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Box_No_Type_Remarks"].ToString(),
                                Fire_Box_No_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Box_No_Remarks"].ToString(),
                                Fire_Hydrants = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Hydrants"].ToString(),
                                Fire_Hydrants_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Hydrants_Remarks"].ToString(),
                                Project = objGlobaldata.GetDropdownitemById(dsFireEquipInspectionList.Tables[0].Rows[i]["Project"].ToString()),
                                ReportNo = dsFireEquipInspectionList.Tables[0].Rows[i]["ReportNo"].ToString(),
                                upload = dsFireEquipInspectionList.Tables[0].Rows[i]["upload"].ToString(),
                            };
                            if (DateTime.TryParse(dsFireEquipInspectionList.Tables[0].Rows[i]["FireNext_DateTime"].ToString(), out dtValue))
                            {
                                objFireInspectionModels.FireNext_DateTime = dtValue;
                            }
                            objFireEquipList.FireEquipList.Add(objFireInspectionModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in FireEquipInspectionList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FireEquipInspectionList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objFireEquipList.FireEquipList.ToList());
        }

        public JsonResult FireEquipInspectionListSearch(string branch_name)
        {
            ViewBag.SubMenutype = "FireEquipInspection";
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

            FireEquipInspectionModelsList objFireEquipList = new FireEquipInspectionModelsList();
            objFireEquipList.FireEquipList = new List<FireEquipInspectionModels>();

            FireEquipInspectionModels objType = new FireEquipInspectionModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT id_FireEquip,Fire_DateTime, Location, FireNext_DateTime,Smoke_Detector, Smoke_Detector_Remarks, " +
                    "Fire_Alarm, Fire_Alarm_Remarks, Fire_Water_Pumps,Fire_Water_Pumps_Remarks,Fire_Box_No,Fire_Box_No_Location,Fire_Box_No_Type_Remarks,Fire_Box_No_Type,Fire_Box_No_Remarks," +
                    "Fire_Hydrants,Fire_Hydrants_Remarks,Project,ReportNo,upload from t_fireequip_inspection where Active='1'";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "";

                DataSet dsFireEquipInspectionList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsFireEquipInspectionList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsFireEquipInspectionList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtDailyDate = Convert.ToDateTime(dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_DateTime"].ToString());
                        DateTime dtValue;

                        try
                        {
                            FireEquipInspectionModels objFireInspectionModels = new FireEquipInspectionModels
                            {
                                id_FireEquip = dsFireEquipInspectionList.Tables[0].Rows[i]["id_FireEquip"].ToString(),
                                Location = objGlobaldata.GetCompanyBranchNameById(dsFireEquipInspectionList.Tables[0].Rows[i]["Location"].ToString()),
                                Fire_DateTime = dtDailyDate,
                                Smoke_Detector = dsFireEquipInspectionList.Tables[0].Rows[i]["Smoke_Detector"].ToString(),
                                Smoke_Detector_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Smoke_Detector_Remarks"].ToString(),
                                Fire_Alarm = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Alarm"].ToString(),
                                Fire_Alarm_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Alarm_Remarks"].ToString(),
                                Fire_Water_Pumps = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Water_Pumps"].ToString(),
                                Fire_Water_Pumps_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Water_Pumps_Remarks"].ToString(),
                                Fire_Box_No_Location = objGlobaldata.GetDropdownitemById(dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Box_No_Location"].ToString()),
                                Fire_Box_No_Type = objFireEquip.GetFireEquipTypeById(dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Box_No_Type"].ToString()),
                                Fire_Box_No = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Box_No"].ToString(),
                                Fire_Box_No_Type_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Box_No_Type_Remarks"].ToString(),
                                Fire_Box_No_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Box_No_Remarks"].ToString(),
                                Fire_Hydrants = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Hydrants"].ToString(),
                                Fire_Hydrants_Remarks = dsFireEquipInspectionList.Tables[0].Rows[i]["Fire_Hydrants_Remarks"].ToString(),
                                Project = objGlobaldata.GetDropdownitemById(dsFireEquipInspectionList.Tables[0].Rows[i]["Project"].ToString()),
                                ReportNo = dsFireEquipInspectionList.Tables[0].Rows[i]["ReportNo"].ToString(),
                                upload = dsFireEquipInspectionList.Tables[0].Rows[i]["upload"].ToString(),
                            };
                            if (DateTime.TryParse(dsFireEquipInspectionList.Tables[0].Rows[i]["FireNext_DateTime"].ToString(), out dtValue))
                            {
                                objFireInspectionModels.FireNext_DateTime = dtValue;
                            }
                            objFireEquipList.FireEquipList.Add(objFireInspectionModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in FireEquipInspectionListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FireEquipInspectionListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        public ActionResult FireEquipInspectionDetails()
        {
            ViewBag.SubMenutype = "FireEquipInspection";
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
            FireEquipInspectionModels objComp = new FireEquipInspectionModels();

            try
            {
                if (Request.QueryString["id_FireEquip"] != null && Request.QueryString["id_FireEquip"] != "")
                {
                    string sid_FireEquip = Request.QueryString["id_FireEquip"];
                    string sSqlstmt = "SELECT id_FireEquip,Fire_DateTime, Location, FireNext_DateTime,Smoke_Detector, Smoke_Detector_Remarks, Fire_Alarm, Fire_Alarm_Remarks," +
                        " Fire_Water_Pumps,Fire_Water_Pumps_Remarks,Fire_Box_No,Fire_Box_No_Location,Fire_Box_No_Type_Remarks,Fire_Box_No_Type,Fire_Box_No_Remarks,Fire_Hydrants," +
                        "Fire_Hydrants_Remarks,Project,ReportNo,upload from t_fireequip_inspection where id_FireEquip=" + sid_FireEquip;

                    DataSet dsFireEquipInspectionList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsFireEquipInspectionList.Tables.Count > 0 && dsFireEquipInspectionList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDailyDate = Convert.ToDateTime(dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_DateTime"].ToString());
                        DateTime dtValue;

                        objComp = new FireEquipInspectionModels
                        {
                            id_FireEquip = dsFireEquipInspectionList.Tables[0].Rows[0]["id_FireEquip"].ToString(),
                            Location = objGlobaldata.GetCompanyBranchNameById(dsFireEquipInspectionList.Tables[0].Rows[0]["Location"].ToString()),
                            Fire_DateTime = dtDailyDate,
                            Smoke_Detector = dsFireEquipInspectionList.Tables[0].Rows[0]["Smoke_Detector"].ToString(),
                            Smoke_Detector_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Smoke_Detector_Remarks"].ToString(),
                            Fire_Alarm = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Alarm"].ToString(),
                            Fire_Alarm_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Alarm_Remarks"].ToString(),
                            Fire_Water_Pumps = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Water_Pumps"].ToString(),
                            Fire_Water_Pumps_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Water_Pumps_Remarks"].ToString(),
                            Fire_Box_No_Location = objGlobaldata.GetDropdownitemById(dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Location"].ToString()),
                            Fire_Box_No_Type = objFireEquip.GetFireEquipTypeById(dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Type"].ToString()),
                            Fire_Box_No = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No"].ToString(),
                            Fire_Box_No_Type_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Type_Remarks"].ToString(),
                            Fire_Box_No_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Remarks"].ToString(),
                            Fire_Hydrants = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Hydrants"].ToString(),
                            Fire_Hydrants_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Hydrants_Remarks"].ToString(),
                            Project = objGlobaldata.GetDropdownitemById(dsFireEquipInspectionList.Tables[0].Rows[0]["Project"].ToString()),
                            ReportNo = dsFireEquipInspectionList.Tables[0].Rows[0]["ReportNo"].ToString(),
                            upload = dsFireEquipInspectionList.Tables[0].Rows[0]["upload"].ToString(),
                        };
                        if (DateTime.TryParse(dsFireEquipInspectionList.Tables[0].Rows[0]["FireNext_DateTime"].ToString(), out dtValue))
                        {
                            objComp.FireNext_DateTime = dtValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("FireEquipInspectionList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("FireEquipInspectionList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FireEquipInspectionDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("FireEquipInspectionList");
            }
            return View(objComp);
        }

        public ActionResult FireEquipInspectionInfo(int id)
        {
            FireEquipInspectionModels objComp = new FireEquipInspectionModels();

            try
            {
                string sSqlstmt = "SELECT id_FireEquip,Fire_DateTime, Location, FireNext_DateTime,Smoke_Detector, Smoke_Detector_Remarks, Fire_Alarm, Fire_Alarm_Remarks, Fire_Water_Pumps,Fire_Water_Pumps_Remarks," +
                    "Fire_Box_No,Fire_Box_No_Location,Fire_Box_No_Type_Remarks,Fire_Box_No_Type,Fire_Box_No_Remarks,Fire_Hydrants,Fire_Hydrants_Remarks,Project,ReportNo,upload from t_fireequip_inspection where id_FireEquip='" + id + "'";

                DataSet dsFireEquipInspectionList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsFireEquipInspectionList.Tables.Count > 0 && dsFireEquipInspectionList.Tables[0].Rows.Count > 0)
                {
                    DateTime dtDailyDate = Convert.ToDateTime(dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_DateTime"].ToString());
                    DateTime dtValue;

                    objComp = new FireEquipInspectionModels
                    {
                        id_FireEquip = dsFireEquipInspectionList.Tables[0].Rows[0]["id_FireEquip"].ToString(),
                        Location = objGlobaldata.GetCompanyBranchNameById(dsFireEquipInspectionList.Tables[0].Rows[0]["Location"].ToString()),
                        Fire_DateTime = dtDailyDate,
                        Smoke_Detector = dsFireEquipInspectionList.Tables[0].Rows[0]["Smoke_Detector"].ToString(),
                        Smoke_Detector_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Smoke_Detector_Remarks"].ToString(),
                        Fire_Alarm = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Alarm"].ToString(),
                        Fire_Alarm_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Alarm_Remarks"].ToString(),
                        Fire_Water_Pumps = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Water_Pumps"].ToString(),
                        Fire_Water_Pumps_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Water_Pumps_Remarks"].ToString(),
                        Fire_Box_No_Location = objGlobaldata.GetDropdownitemById(dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Location"].ToString()),
                        Fire_Box_No_Type = objFireEquip.GetFireEquipTypeById(dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Type"].ToString()),
                        Fire_Box_No = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No"].ToString(),
                        Fire_Box_No_Type_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Type_Remarks"].ToString(),
                        Fire_Box_No_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Remarks"].ToString(),
                        Fire_Hydrants = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Hydrants"].ToString(),
                        Fire_Hydrants_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Hydrants_Remarks"].ToString(),
                        Project = objGlobaldata.GetDropdownitemById(dsFireEquipInspectionList.Tables[0].Rows[0]["Project"].ToString()),
                        ReportNo = dsFireEquipInspectionList.Tables[0].Rows[0]["ReportNo"].ToString(),
                        upload = dsFireEquipInspectionList.Tables[0].Rows[0]["upload"].ToString(),
                    };
                    if (DateTime.TryParse(dsFireEquipInspectionList.Tables[0].Rows[0]["FireNext_DateTime"].ToString(), out dtValue))
                    {
                        objComp.FireNext_DateTime = dtValue;
                    }
                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("FireEquipInspectionList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FireEquipInspectionInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objComp);
        }

        [AllowAnonymous]
        public JsonResult FireEquipInspectionDelete(FormCollection form)
        {
            try
            {
                if (form["id_FireEquip"] != null && form["id_FireEquip"] != "")
                {
                    FireEquipInspectionModels Doc = new FireEquipInspectionModels();
                    string sid_FireEquip = form["id_FireEquip"];

                    if (Doc.FunDeleteFireEquipInspection(sid_FireEquip))
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
                    TempData["alertdata"] = "Fire Equip inspection Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception inFireEquipInspectionDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public ActionResult FireEquipInspectionEdit()
        {
            ViewBag.SubMenutype = "FireEquipInspection";
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

            FireEquipInspectionModels objComp = new FireEquipInspectionModels();
            try
            {
                if (Request.QueryString["id_FireEquip"] != null && Request.QueryString["id_FireEquip"] != "")
                {
                    string sid_FireEquip = Request.QueryString["id_FireEquip"];
                    string sSqlstmt = "SELECT id_FireEquip,Fire_DateTime, Location, FireNext_DateTime,Smoke_Detector, Smoke_Detector_Remarks, Fire_Alarm, Fire_Alarm_Remarks," +
                        " Fire_Water_Pumps,Fire_Water_Pumps_Remarks,Fire_Box_No,Fire_Box_No_Location,Fire_Box_No_Type_Remarks,Fire_Box_No_Type,Fire_Box_No_Remarks,Fire_Hydrants,Fire_Hydrants_Remarks,Project,ReportNo,upload" +
                        " from t_fireequip_inspection where id_FireEquip=" + sid_FireEquip;

                    DataSet dsFireEquipInspectionList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsFireEquipInspectionList.Tables.Count > 0 && dsFireEquipInspectionList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDailyDate = Convert.ToDateTime(dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_DateTime"].ToString());
                        DateTime dtValue;

                        objComp = new FireEquipInspectionModels
                        {
                            id_FireEquip = dsFireEquipInspectionList.Tables[0].Rows[0]["id_FireEquip"].ToString(),
                            Location = objGlobaldata.GetCompanyBranchNameById(dsFireEquipInspectionList.Tables[0].Rows[0]["Location"].ToString()),
                            Fire_DateTime = dtDailyDate,
                            Smoke_Detector = dsFireEquipInspectionList.Tables[0].Rows[0]["Smoke_Detector"].ToString(),
                            Smoke_Detector_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Smoke_Detector_Remarks"].ToString(),
                            Fire_Alarm = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Alarm"].ToString(),
                            Fire_Alarm_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Alarm_Remarks"].ToString(),
                            Fire_Water_Pumps = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Water_Pumps"].ToString(),
                            Fire_Water_Pumps_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Water_Pumps_Remarks"].ToString(),
                            Fire_Box_No_Location = objGlobaldata.GetDropdownitemById(dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Location"].ToString()),
                            Fire_Box_No_Type = objFireEquip.GetFireEquipTypeById(dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Type"].ToString()),
                            Fire_Box_No = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No"].ToString(),
                            Fire_Box_No_Type_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Type_Remarks"].ToString(),
                            Fire_Box_No_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Box_No_Remarks"].ToString(),
                            Fire_Hydrants = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Hydrants"].ToString(),
                            Fire_Hydrants_Remarks = dsFireEquipInspectionList.Tables[0].Rows[0]["Fire_Hydrants_Remarks"].ToString(),
                            Project = objGlobaldata.GetDropdownitemById(dsFireEquipInspectionList.Tables[0].Rows[0]["Project"].ToString()),
                            ReportNo = dsFireEquipInspectionList.Tables[0].Rows[0]["ReportNo"].ToString(),
                            upload = dsFireEquipInspectionList.Tables[0].Rows[0]["upload"].ToString(),
                        };
                        if (DateTime.TryParse(dsFireEquipInspectionList.Tables[0].Rows[0]["FireNext_DateTime"].ToString(), out dtValue))
                        {
                            objComp.FireNext_DateTime = dtValue;
                        }

                        InitilizeFireEquip();

                        return View(objComp);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("FireEquipInspectionList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Fire Equip Inspection Id cannot be null";
                    return RedirectToAction("FireEquipInspectionList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FireEquipInspectionEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("FireEquipInspectionList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FireEquipInspectionEdit(FireEquipInspectionModels objFireInspection, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                //objFireInspection.id_FireEquip = form["id_FireEquip"];
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                DateTime dateValue;

                if (form["Fire_DateTime"] != null && DateTime.TryParse(form["Fire_DateTime"], out dateValue) == true)
                {
                    objFireInspection.Fire_DateTime = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objFireInspection.Fire_DateTime = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (DateTime.TryParse(form["FireNext_DateTime"], out dateValue) == true)
                {
                    objFireInspection.FireNext_DateTime = dateValue;
                }
                objFireInspection.upload = "";
                if (upload != null && files.ContentLength > 0)
                {
                    foreach (var document in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/FireInsp"), Path.GetFileName(document.FileName));
                            string sFilename = "FireInsp" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            document.SaveAs(sFilepath + "/" + sFilename);
                            objFireInspection.upload = objFireInspection.upload + "," + "~/DataUpload/MgmtDocs/FireInsp/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in FireEquipInspectionEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objFireInspection.upload = objFireInspection.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objFireInspection.upload = objFireInspection.upload + "," + form["QCDocsVal"];
                    objFireInspection.upload = objFireInspection.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objFireInspection.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objFireInspection.upload = null;
                }
                if (objFireInspection.FunUpdateFireEquip(objFireInspection))
                {
                    TempData["Successdata"] = "Updated Fire Equip Inspection details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DailyInspectionEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("FireEquipInspectionList");
        }

        [HttpPost]
        public JsonResult FunGetReportNo(string Location)
        {
            DataSet dsData; string RepNo = "";

            dsData = objGlobaldata.GetReportNo("FIRE", "", Location);
            if (dsData != null && dsData.Tables.Count > 0)
            {
                RepNo = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
            }
            return Json(RepNo);
        }
    }
}