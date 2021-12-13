using ISOStd.Filters;
using ISOStd.Models;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class ManHourController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public ManHourController()
        {
            ViewBag.Menutype = "Human Resources";
            ViewBag.SubMenutype = "ManHour";
        }

        public ActionResult AddManPower()
        {
            try
            {
                ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
                ViewBag.Month = objGlobaldata.GetConstantValue("Months");
                ViewBag.Year = objGlobaldata.GetDropdownList("Years");
                ViewBag.FuelUnit = objGlobaldata.GetDropdownList("Fuel Measurement Unit");
                ViewBag.WaterUnit = objGlobaldata.GetDropdownList("Water Measurement Unit");
                ViewBag.PaperUnit = objGlobaldata.GetDropdownList("Paper Measurement Unit");
                ViewBag.SolidUnit = objGlobaldata.GetDropdownList("Quantiy of Solid Unit");
                ViewBag.Reviewer = objGlobaldata.GetReviewer();
                ViewBag.Preparer = objGlobaldata.GetPreparer();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddManPower: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddManPower(ManHourModels objManHr, FormCollection form)
        {
            try
            {
                if (objManHr.FunAddManHour(objManHr))
                {
                    TempData["Successdata"] = "Added ManPower details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddManPower: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ManPowerList");
        }

        public ActionResult ManPowerList(int? page, string branch_name)
        {
            ManHourModelsList objList = new ManHourModelsList();
            objList.ManHrList = new List<ManHourModels>();

            ManHourModels objManHr = new ManHourModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT id_man_hour,project, location,t_month,t_year,man_power,man_hour,man_hour_ot,restrict_days," +
                    "fuel_consump,fuelunit,water_consump,waterunit,paper_consump,paperunit,electricity_consump,qty_solid,solidunit,preparedby,reviewedby,comments  from t_man_hour where active='1'";

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
                DataSet dsManHrList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsManHrList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsManHrList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objManHr = new ManHourModels
                            {
                                id_man_hour = dsManHrList.Tables[0].Rows[i]["id_man_hour"].ToString(),
                                location = objGlobaldata.GetCompanyBranchNameById(dsManHrList.Tables[0].Rows[i]["location"].ToString()),
                                project = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["project"].ToString()),
                                man_power = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["man_power"].ToString()),
                                man_hour = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["man_hour"].ToString()),
                                t_month = dsManHrList.Tables[0].Rows[i]["t_month"].ToString(),
                                t_year = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["t_year"].ToString()),
                                man_hour_ot = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["man_hour_ot"].ToString()),
                                restrict_days = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["restrict_days"].ToString()),
                                fuel_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["fuel_consump"].ToString()),
                                water_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["water_consump"].ToString()),
                                paper_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["paper_consump"].ToString()),
                                electricity_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["electricity_consump"].ToString()),
                                qty_solid = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["qty_solid"].ToString()),
                                fuelunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["fuelunit"].ToString()),
                                waterunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["waterunit"].ToString()),
                                paperunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["paperunit"].ToString()),
                                solidunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["solidunit"].ToString()),
                                preparedby = objGlobaldata.GetEmpHrNameById(dsManHrList.Tables[0].Rows[i]["preparedby"].ToString()),
                                reviewedby = objGlobaldata.GetEmpHrNameById(dsManHrList.Tables[0].Rows[i]["reviewedby"].ToString()),
                                comments = dsManHrList.Tables[0].Rows[i]["comments"].ToString(),
                            };

                            objList.ManHrList.Add(objManHr);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ManPowerList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ManPowerList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objList.ManHrList.ToList().ToPagedList(page ?? 1, 40));
        }

        public JsonResult ManPowerListSearch(int? page, string branch_name)
        {
            ManHourModelsList objList = new ManHourModelsList();
            objList.ManHrList = new List<ManHourModels>();

            ManHourModels objManHr = new ManHourModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT id_man_hour,project, location,t_month,t_year,man_power,man_hour,man_hour_ot,restrict_days," +
                    "fuel_consump,fuelunit,water_consump,waterunit,paper_consump,paperunit,electricity_consump,qty_solid,solidunit,preparedby,reviewedby,comments  from t_man_hour where active='1'";

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
                DataSet dsManHrList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsManHrList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsManHrList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objManHr = new ManHourModels
                            {
                                id_man_hour = dsManHrList.Tables[0].Rows[i]["id_man_hour"].ToString(),
                                location = objGlobaldata.GetCompanyBranchNameById(dsManHrList.Tables[0].Rows[i]["location"].ToString()),
                                project = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["project"].ToString()),
                                man_power = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["man_power"].ToString()),
                                man_hour = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["man_hour"].ToString()),
                                t_month = dsManHrList.Tables[0].Rows[i]["t_month"].ToString(),
                                t_year = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["t_year"].ToString()),
                                man_hour_ot = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["man_hour_ot"].ToString()),
                                restrict_days = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["restrict_days"].ToString()),
                                fuel_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["fuel_consump"].ToString()),
                                water_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["water_consump"].ToString()),
                                paper_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["paper_consump"].ToString()),
                                electricity_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["electricity_consump"].ToString()),
                                qty_solid = Convert.ToInt32(dsManHrList.Tables[0].Rows[i]["qty_solid"].ToString()),
                                fuelunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["fuelunit"].ToString()),
                                waterunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["waterunit"].ToString()),
                                paperunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["paperunit"].ToString()),
                                solidunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[i]["solidunit"].ToString()),
                                preparedby = objGlobaldata.GetEmpHrNameById(dsManHrList.Tables[0].Rows[i]["preparedby"].ToString()),
                                reviewedby = objGlobaldata.GetEmpHrNameById(dsManHrList.Tables[0].Rows[i]["reviewedby"].ToString()),
                                comments = dsManHrList.Tables[0].Rows[i]["comments"].ToString(),
                            };

                            objList.ManHrList.Add(objManHr);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ManPowerListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ManPowerListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        public ActionResult ManPowerEdit()
        {
            ManHourModels objManHr = new ManHourModels();
            try
            {
                ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
                ViewBag.Month = objGlobaldata.GetConstantValue("Months");
                ViewBag.Year = objGlobaldata.GetDropdownList("Years");
                ViewBag.FuelUnit = objGlobaldata.GetDropdownList("Fuel Measurement Unit");
                ViewBag.WaterUnit = objGlobaldata.GetDropdownList("Water Measurement Unit");
                ViewBag.PaperUnit = objGlobaldata.GetDropdownList("Paper Measurement Unit");
                ViewBag.SolidUnit = objGlobaldata.GetDropdownList("Quantiy of Solid Unit");
                ViewBag.Reviewer = objGlobaldata.GetReviewer();
                ViewBag.Preparer = objGlobaldata.GetPreparer();

                if (Request.QueryString["id_man_hour"] != null && Request.QueryString["id_man_hour"] != "")
                {
                    string sid_man_hour = Request.QueryString["id_man_hour"];

                    string sSqlstmt = "SELECT id_man_hour,project, location,t_month,t_year,man_power,man_hour,man_hour_ot,restrict_days," +
                    "fuel_consump,fuelunit,water_consump,waterunit,paper_consump,paperunit,electricity_consump,qty_solid,solidunit,preparedby,reviewedby,comments  from t_man_hour where active='1' and  id_man_hour = '" + sid_man_hour + "'";

                    DataSet dsManHrList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsManHrList.Tables.Count > 0 && dsManHrList.Tables[0].Rows.Count > 0)
                    {
                        objManHr = new ManHourModels
                        {
                            id_man_hour = dsManHrList.Tables[0].Rows[0]["id_man_hour"].ToString(),
                            location = (dsManHrList.Tables[0].Rows[0]["location"].ToString()),
                            project =/* objGlobaldata.GetDropdownitemById*/(dsManHrList.Tables[0].Rows[0]["project"].ToString()),
                            man_power = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_power"].ToString()),
                            man_hour = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_hour"].ToString()),
                            t_month = dsManHrList.Tables[0].Rows[0]["t_month"].ToString(),
                            t_year = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["t_year"].ToString()),
                            man_hour_ot = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_hour_ot"].ToString()),
                            restrict_days = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["restrict_days"].ToString()),
                            fuel_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["fuel_consump"].ToString()),
                            water_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["water_consump"].ToString()),
                            paper_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["paper_consump"].ToString()),
                            electricity_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["electricity_consump"].ToString()),
                            qty_solid = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["qty_solid"].ToString()),
                            fuelunit = dsManHrList.Tables[0].Rows[0]["fuelunit"].ToString(),
                            waterunit = dsManHrList.Tables[0].Rows[0]["waterunit"].ToString(),
                            paperunit = dsManHrList.Tables[0].Rows[0]["paperunit"].ToString(),
                            solidunit = dsManHrList.Tables[0].Rows[0]["solidunit"].ToString(),
                            preparedby = dsManHrList.Tables[0].Rows[0]["preparedby"].ToString(),
                            reviewedby = dsManHrList.Tables[0].Rows[0]["reviewedby"].ToString(),
                            comments = dsManHrList.Tables[0].Rows[0]["comments"].ToString(),
                        };

                        return View(objManHr);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("ManPowerList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Man hour Id cannot be null";
                    return RedirectToAction("ManPowerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ManPowerEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ManPowerList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManPowerEdit(ManHourModels objManHr, FormCollection form)
        {
            try
            {
                if (objManHr.FunUpdateManHour(objManHr))
                {
                    TempData["Successdata"] = "Updated ManPower details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ManPowerEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ManPowerList");
        }

        public ActionResult ManPowerDetails()
        {
            ManHourModels objManHr = new ManHourModels();
            try
            {
                if (Request.QueryString["id_man_hour"] != null && Request.QueryString["id_man_hour"] != "")
                {
                    string sid_man_hour = Request.QueryString["id_man_hour"];

                    string sSqlstmt = "SELECT id_man_hour,project, location,t_month,t_year,man_power,man_hour,man_hour_ot,restrict_days," +
                    "fuel_consump,fuelunit,water_consump,waterunit,paper_consump,paperunit,electricity_consump,qty_solid,solidunit,preparedby,reviewedby,comments  from t_man_hour where active='1' and  id_man_hour = '" + sid_man_hour + "'";

                    DataSet dsManHrList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsManHrList.Tables.Count > 0 && dsManHrList.Tables[0].Rows.Count > 0)
                    {
                        objManHr = new ManHourModels
                        {
                            id_man_hour = dsManHrList.Tables[0].Rows[0]["id_man_hour"].ToString(),
                            location = (dsManHrList.Tables[0].Rows[0]["location"].ToString()),
                            project = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["project"].ToString()),
                            man_power = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_power"].ToString()),
                            man_hour = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_hour"].ToString()),
                            t_month = dsManHrList.Tables[0].Rows[0]["t_month"].ToString(),
                            t_year = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["t_year"].ToString()),
                            man_hour_ot = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_hour_ot"].ToString()),
                            restrict_days = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["restrict_days"].ToString()),
                            fuel_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["fuel_consump"].ToString()),
                            water_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["water_consump"].ToString()),
                            paper_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["paper_consump"].ToString()),
                            electricity_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["electricity_consump"].ToString()),
                            qty_solid = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["qty_solid"].ToString()),
                            fuelunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["fuelunit"].ToString()),
                            waterunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["waterunit"].ToString()),
                            paperunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["paperunit"].ToString()),
                            solidunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["solidunit"].ToString()),
                            preparedby = objGlobaldata.GetEmpHrNameById(dsManHrList.Tables[0].Rows[0]["preparedby"].ToString()),
                            reviewedby = objGlobaldata.GetEmpHrNameById(dsManHrList.Tables[0].Rows[0]["reviewedby"].ToString()),
                            comments = dsManHrList.Tables[0].Rows[0]["comments"].ToString(),
                        };

                        return View(objManHr);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("ManPowerList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ManHour Id cannot be null";
                    return RedirectToAction("ManPowerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ManPowerDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ManPowerList");
        }

        public ActionResult ManPowerInfo(int id)
        {
            ManHourModels objManHr = new ManHourModels();
            try
            {
                string sSqlstmt = "SELECT id_man_hour,project, location,t_month,t_year,man_power,man_hour,man_hour_ot,restrict_days," +
              "fuel_consump,fuelunit,water_consump,waterunit,paper_consump,paperunit,electricity_consump,qty_solid,solidunit,preparedby,reviewedby,comments  from t_man_hour where active='1' and  id_man_hour= '" + id + "'";

                DataSet dsManHrList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsManHrList.Tables.Count > 0 && dsManHrList.Tables[0].Rows.Count > 0)
                {
                    objManHr = new ManHourModels
                    {
                        id_man_hour = dsManHrList.Tables[0].Rows[0]["id_man_hour"].ToString(),
                        location = (dsManHrList.Tables[0].Rows[0]["location"].ToString()),
                        project = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["project"].ToString()),
                        man_power = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_power"].ToString()),
                        man_hour = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_hour"].ToString()),
                        t_month = dsManHrList.Tables[0].Rows[0]["t_month"].ToString(),
                        t_year = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["t_year"].ToString()),
                        man_hour_ot = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_hour_ot"].ToString()),
                        restrict_days = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["restrict_days"].ToString()),
                        fuel_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["fuel_consump"].ToString()),
                        water_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["water_consump"].ToString()),
                        paper_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["paper_consump"].ToString()),
                        electricity_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["electricity_consump"].ToString()),
                        qty_solid = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["qty_solid"].ToString()),
                        fuelunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["fuelunit"].ToString()),
                        waterunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["waterunit"].ToString()),
                        paperunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["paperunit"].ToString()),
                        solidunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["solidunit"].ToString()),
                        preparedby = objGlobaldata.GetEmpHrNameById(dsManHrList.Tables[0].Rows[0]["preparedby"].ToString()),
                        reviewedby = objGlobaldata.GetEmpHrNameById(dsManHrList.Tables[0].Rows[0]["reviewedby"].ToString()),
                        comments = dsManHrList.Tables[0].Rows[0]["comments"].ToString(),
                    };

                    return View(objManHr);
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("ManPowerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ManPowerInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ManPowerList");
        }

        [AllowAnonymous]
        public JsonResult ManPowerDelete(FormCollection form)
        {
            try
            {
                if (form["id_man_hour"] != null && form["id_man_hour"] != "")
                {
                    ManHourModels Doc = new ManHourModels();
                    string sid_man_hour = form["id_man_hour"];

                    if (Doc.FunDeleteManHour(sid_man_hour))
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
                    TempData["alertdata"] = "Man Hour Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ManPowerDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult ManPowerReportToPDF(FormCollection form)
        {
            try
            {
                ManHourModels objManHr = new ManHourModels();
                string sid_man_hour = form["id_man_hour"];

                if (sid_man_hour != null && sid_man_hour != "")
                {
                    string sSqlstmt = "SELECT id_man_hour,project, location,t_month,t_year,man_power,man_hour,man_hour_ot,restrict_days," +
                    "fuel_consump,fuelunit,water_consump,waterunit,paper_consump,paperunit,electricity_consump,qty_solid,solidunit,preparedby,reviewedby,comments  from t_man_hour where active='1' and  id_man_hour = '" + sid_man_hour + "'";

                    DataSet dsManHrList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsManHrList.Tables.Count > 0 && dsManHrList.Tables[0].Rows.Count > 0)
                    {
                        objManHr = new ManHourModels
                        {
                            id_man_hour = dsManHrList.Tables[0].Rows[0]["id_man_hour"].ToString(),
                            location = (dsManHrList.Tables[0].Rows[0]["location"].ToString()),
                            project = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["project"].ToString()),
                            man_power = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_power"].ToString()),
                            man_hour = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_hour"].ToString()),
                            t_month = dsManHrList.Tables[0].Rows[0]["t_month"].ToString(),
                            t_year = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["t_year"].ToString()),
                            man_hour_ot = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["man_hour_ot"].ToString()),
                            restrict_days = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["restrict_days"].ToString()),
                            fuel_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["fuel_consump"].ToString()),
                            water_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["water_consump"].ToString()),
                            paper_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["paper_consump"].ToString()),
                            electricity_consump = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["electricity_consump"].ToString()),
                            qty_solid = Convert.ToInt32(dsManHrList.Tables[0].Rows[0]["qty_solid"].ToString()),
                            fuelunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["fuelunit"].ToString()),
                            waterunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["waterunit"].ToString()),
                            paperunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["paperunit"].ToString()),
                            solidunit = objGlobaldata.GetDropdownitemById(dsManHrList.Tables[0].Rows[0]["solidunit"].ToString()),
                            preparedby = objGlobaldata.GetEmpHrNameById(dsManHrList.Tables[0].Rows[0]["preparedby"].ToString()),
                            reviewedby = objGlobaldata.GetEmpHrNameById(dsManHrList.Tables[0].Rows[0]["reviewedby"].ToString()),
                            comments = dsManHrList.Tables[0].Rows[0]["comments"].ToString(),
                        };

                        ViewBag.Manpower = objManHr;
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("ManPowerList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Man hour Id cannot be null";
                    return RedirectToAction("ManPowerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ManPowerReportToPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("ManpowerPDF")
            {
                //FileName = "ManPowerReportPDF.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public JsonResult FunGetTotalEmp()
        {
            string count = "";
            try
            {
                string sSqlstmt = "select count(*)  as empcount from t_hr_employee where emp_status=1";
                DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    count = dsData.Tables[0].Rows[0]["empcount"].ToString();
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetIncidentDesc: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(count);
        }
    }
}