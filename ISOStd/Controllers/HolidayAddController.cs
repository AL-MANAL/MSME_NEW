using ISOStd.Filters;
using ISOStd.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class HolidayAddController : Controller
    {
        // GET: HolidayAdd
        private clsGlobal objGlobaldata = new clsGlobal();

        public HolidayAddController()
        {
            ViewBag.Menutype = "LeaveMgmt";
            ViewBag.SubMenutype = "Holiday";
        }

        // GET: Certificates

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Holiday(string chkAll, string SearchText, int? page, string year)
        {
            HolidayAddModelsList obj = new HolidayAddModelsList();
            obj.HolidayList = new List<HolidayAddModel>();

            try
            {
                ViewBag.Year = objGlobaldata.GetDropdownList("Years");
                string sSqlstmt = "select Id_holiday,Name,Frdate,Todate from t_holiday";
                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;
                    if (year != null && year != "")
                    {
                        ViewBag.YearVal = year;
                        sSearchtext = sSearchtext + "  where '" + year + "'=year(Frdate) or '" + year + "'=year(Todate)";
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Name asc";
                DataSet dsHolList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsHolList.Tables.Count > 0 && dsHolList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsHolList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            HolidayAddModel objCertificate = new HolidayAddModel
                            {
                                Id_holiday = (dsHolList.Tables[0].Rows[i]["Id_holiday"].ToString()),
                                Name = dsHolList.Tables[0].Rows[i]["Name"].ToString(),
                            };
                            DateTime dtDocDate;
                            if (dsHolList.Tables[0].Rows[i]["Frdate"].ToString() != ""
                               && DateTime.TryParse(dsHolList.Tables[0].Rows[i]["Frdate"].ToString(), out dtDocDate))
                            {
                                objCertificate.Frdate = dtDocDate;
                            }
                            if (dsHolList.Tables[0].Rows[i]["Todate"].ToString() != ""
                               && DateTime.TryParse(dsHolList.Tables[0].Rows[i]["Todate"].ToString(), out dtDocDate))
                            {
                                objCertificate.Todate = dtDocDate;
                            }
                            obj.HolidayList.Add(objCertificate);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in HolidayAdd: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HolidayAdd: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(obj.HolidayList.ToList().ToPagedList(page ?? 1, 10));
        }

        [AllowAnonymous]
        public ActionResult AddHoliday(HolidayAddModel obj, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                if (DateTime.TryParse(form["Frdate"], out dateValue) == true)
                {
                    obj.Frdate = dateValue;
                }

                if (DateTime.TryParse(form["Todate"], out dateValue) == true)
                {
                    obj.Todate = dateValue;
                }

                if (obj.FunAddHoliday(obj))
                {
                    TempData["Successdata"] = "Added Holiday details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHoliday: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Holiday");
        }

        [AllowAnonymous]
        public JsonResult HolidayDelete(FormCollection form)
        {
            try
            {
                if (form["Id_holiday"] != null && form["Id_holiday"] != "")
                {
                    HolidayAddModel cert = new HolidayAddModel();
                    string sId_holiday = form["Id_holiday"];

                    if (cert.FunDeleteCertificate(sId_holiday))
                    {
                        TempData["Successdata"] = "Holiday details deleted successfully";
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
                    TempData["alertdata"] = "Access Denied";
                    return Json("Access Denied");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HolidayDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public ActionResult HolidayEdit()
        {
            HolidayAddModel obj = new HolidayAddModel();
            try
            {
                if (Request.QueryString["Id_holiday"] != null && Request.QueryString["Id_holiday"] != "")
                {
                    string sId_holiday = Request.QueryString["Id_holiday"];

                    string sSqlstmt = "select Id_holiday,Name,Frdate,Todate from t_holiday where Id_holiday='" + sId_holiday + "'";
                    DataSet dsHolitList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsHolitList.Tables.Count > 0 && dsHolitList.Tables[0].Rows.Count > 0)
                    {
                        obj = new HolidayAddModel
                        {
                            Id_holiday = (dsHolitList.Tables[0].Rows[0]["Id_holiday"].ToString()),
                            Name = dsHolitList.Tables[0].Rows[0]["Name"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsHolitList.Tables[0].Rows[0]["Frdate"].ToString() != ""
                           && DateTime.TryParse(dsHolitList.Tables[0].Rows[0]["Frdate"].ToString(), out dtDocDate))
                        {
                            obj.Frdate = dtDocDate;
                        }
                        if (dsHolitList.Tables[0].Rows[0]["Todate"].ToString() != ""
                          && DateTime.TryParse(dsHolitList.Tables[0].Rows[0]["Todate"].ToString(), out dtDocDate))
                        {
                            obj.Todate = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Holiday ID cannot be Null or empty";
                        return RedirectToAction("Holiday");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Holiday ID cannot be Null or empty";
                    return RedirectToAction("Holiday");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HolidayEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("Certificate");
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HolidayEdit(HolidayAddModel obj, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                if (DateTime.TryParse(form["Frdate"], out dateValue) == true)
                {
                    obj.Frdate = dateValue;
                }
                if (DateTime.TryParse(form["Todate"], out dateValue) == true)
                {
                    obj.Todate = dateValue;
                }

                if (obj.FunUpdateHolidady(obj))
                {
                    TempData["Successdata"] = "Holiday details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HolidayEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("Holiday");
        }
    }
}