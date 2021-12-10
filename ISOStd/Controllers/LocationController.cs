using ISOStd.Models;
using System;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    public class LocationController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public LocationController()
        {
            ViewBag.Menutype = "Settings";
        }

        [AllowAnonymous]
        public ActionResult AddLocations(string id_country = "", string area_type = "")
        {
            LocationModels objModel = new LocationModels();
            try
            {
                ViewBag.SubMenutype = "Locations";
                //MultiSelectList CountryList = objGlobaldata.GetCountryListbox();
                //ViewBag.CountryHeader = CountryList;//GetDepartmentList();
                ViewBag.CountryHeader = objGlobaldata.GetCountryListbox();
                // ViewBag.AreaName = objGlobaldata.GetAreaTypeNameList();
                if (id_country != null && id_country != "" && area_type != "" && area_type != null)
                {
                    // ViewBag.dsCountry = objGlobaldata.Getdetails("select id_country, country_name from t_country where id_country='" + id_country + "' order by id_country asc");
                    ViewBag.dsLocations = objGlobaldata.Getdetails("select id_location,id_country, location_name from t_location where id_country='" + id_country + "' and area_type='" + area_type + "' and active= 1 order by id_country asc");
                    ViewBag.Country = id_country;
                    // ViewBag.Area = area_type;
                }
                else if (id_country != null && id_country != "")
                {
                    // ViewBag.dsCountry = objGlobaldata.Getdetails("select id_country, country_name from t_country where id_country='" + id_country + "' order by id_country asc");
                    ViewBag.dsLocations = objGlobaldata.Getdetails("select id_location,id_country, location_name from t_location where id_country='" + id_country + "' and active= 1 order by id_country asc");
                    ViewBag.Country = id_country;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLocations: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLocations(LocationModels objModels, FormCollection form)
        {
            try
            {
                if (objModels.FunAddLocations(objModels))
                {
                    TempData["Successdata"] = "Added Location details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLocations: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddLocations", new { id_country = objModels.id_country, area_type = objModels.area_type });
        }

        [AllowAnonymous]
        public JsonResult UpdateLocation(string id_location, string location_name)
        {
            LocationModels objModels = new LocationModels();
            try
            {
                objModels.id_location = id_location;
                objModels.location_name = location_name;

                if (objModels.FunUpdateLocations(objModels))
                {
                    TempData["Successdata"] = "Updated Location details successfully";
                    return Json("Update Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UpdateLocations: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Update Failed");
        }

        [HttpPost]
        public JsonResult DeleteLocation(FormCollection form)
        {
            string id_location = form["id_location"];

            if (id_location != "")
            {
                LocationModels objModels = new LocationModels();
                try
                {
                    if (objModels.FunDeleteLocations(id_location))
                    {
                        TempData["Successdata"] = "Deleted Location details successfully";
                        return Json("Deleted Successfully");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in DeleteLocation: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            return Json("Delete Failed, Null Id");
        }

        [HttpPost]
        public JsonResult GetLocations(string id_country)
        {
            MultiSelectList LocList = objGlobaldata.GetLocationListWithCountryId(id_country);

            return Json(LocList);
        }

        [HttpPost]
        public JsonResult GetLocationsWithArea(string id_country, string area_type)
        {
            //MultiSelectList LocList = new MultiSelectList("");

            if (area_type != "" && area_type != null)
            {
                MultiSelectList LocList = objGlobaldata.GetLocationListWithCountrynAreaId(id_country, area_type);
                return Json(LocList);
            }
            else
            {
                MultiSelectList LocList = objGlobaldata.GetLocationListWithCountryId(id_country);
                return Json(LocList);
            }
        }

        public ActionResult FunGetAreaTypeNameList()
        {
            MultiSelectList AreaList = objGlobaldata.GetAreaTypeNameList();
            return Json(AreaList);
        }
    }
}