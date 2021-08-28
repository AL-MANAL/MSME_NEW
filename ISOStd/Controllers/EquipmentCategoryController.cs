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

namespace ISOStd.Controllers
{
    public class EquipmentCategoryController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        static List<string> objEmployeeList = new List<string>();
        
        [AllowAnonymous]
        public ActionResult AddCategory()
        {
            try
            {
                   ViewBag.Category = objGlobaldata.getCategories();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCategory: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

         
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(EquipmentCategoryModels objCat, FormCollection form)
        {
            try
            {
                objCat.Category = form["Category"];
                if (objCat.FunAddCategory(objCat))
                {
                    TempData["Successdata"] = "Category added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCategory: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CategoryList");
        }

         
        [AllowAnonymous]
        public ActionResult CategoryList(int? page)
        {
            EquipmentCategoryModelsList lstCatModel = new EquipmentCategoryModelsList();
            lstCatModel.lstCat = new List<EquipmentCategoryModels>();

            try
            {
             
                string sSqlstmt = "select id_cat,Category,Cat_name from t_equipment_category where Active=1";
                DataSet dsCatList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCatList.Tables.Count > 0 && dsCatList.Tables[0].Rows.Count > 0)
                {
                  
                        if (objGlobaldata.GetCurrentUserSession().DeleteFlag.ToLower() == "y")
                        {
                            ViewBag.DeleteFlg = true;
                        }
                    
                    for (int i = 0; i < dsCatList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EquipmentCategoryModels objCategory = new EquipmentCategoryModels
                            {
                                id_cat =Convert.ToInt32(dsCatList.Tables[0].Rows[i]["id_cat"].ToString()),
                                Category =objGlobaldata.getCategoriesByID(dsCatList.Tables[0].Rows[i]["Category"].ToString()),
                                Cat_name = dsCatList.Tables[0].Rows[i]["Cat_name"].ToString(),
                         
                            };
                            lstCatModel.lstCat.Add(objCategory);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CategoryList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CategoryList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(lstCatModel.lstCat.ToList().ToPagedList(page ?? 1, 10000));

        }

         
        [AllowAnonymous]
        public JsonResult CategoryDelete(FormCollection form)
        {
            try
            {
               
                    if (objGlobaldata.GetCurrentUserSession().DeleteFlag.ToLower() == "y")
                    {
                        if (form["id_cat"] != null && form["id_cat"] != "")
                        {

                            EquipmentCategoryModels Doc = new EquipmentCategoryModels();
                            string sid_cat = form["id_cat"];


                            if (Doc.FunDeleteCategory(sid_cat))
                            {
                                TempData["Successdata"] = "Category deleted successfully";
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
                            TempData["alertdata"] = "Category Id cannot be Null or empty";
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
                objGlobaldata.AddFunctionalLog("Exception in CategoryDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

         
        [HttpPost]
        public JsonResult doesCategoryNameExist(string Cat_name)
        {
            EquipmentCategoryModels obj = new EquipmentCategoryModels();
            var user = obj.CheckForCategoryNameExists(Cat_name);

            return Json(user);
        }

         
        [AllowAnonymous]
        public ActionResult AddVehicleDetails()
        {
            try
            {
                EquipmentDetailModels objEqup = new EquipmentDetailModels();
                ViewBag.Category = objGlobaldata.getCategories();
                ViewBag.Vehicle = objEqup.getVehicleList();
                ViewBag.VehicleStatus = objGlobaldata.getVehicleStatusList();
                ViewBag.Equipment = objEqup.getEquipmentList();
                ViewBag.Emirates = objGlobaldata.GetConstantValue("Emirates");
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.TrailerType = objGlobaldata.getTrailerTypeList();
                ViewBag.Company = objGlobaldata.getTrailerCompanyList();
                ViewBag.Suspension = objGlobaldata.getSuspensionTypeList();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEquipmentDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

         
        [AllowAnonymous]
        public ActionResult AddEquipmentDetails()
        {
            try
            {
                EquipmentDetailModels objEqup=new EquipmentDetailModels();
                ViewBag.Category = objGlobaldata.getCategories();
                ViewBag.Vehicle = objEqup.getVehicleList();
                ViewBag.VehicleStatus = objGlobaldata.getVehicleStatusList();
                ViewBag.Equipment = objEqup.getEquipmentList();
                ViewBag.Emirates = objGlobaldata.GetConstantValue("Emirates");
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.TrailerType = objGlobaldata.getTrailerTypeList();
                ViewBag.Company = objGlobaldata.getTrailerCompanyList();
                ViewBag.Suspension = objGlobaldata.getSuspensionTypeList();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEquipmentDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

         
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddEquipmentDetails(EquipmentDetailModels objEqp, FormCollection form)
        {
            try
            {
                   
                objEqp.trailertype = form["trailertype"];
                objEqp.company = form["company"];
                objEqp.suspensiontype = form["suspensiontype"];
                objEqp.Cat_name = form["Cat_name"];
                

                DateTime dateValue;
               
              
                if (DateTime.TryParse(form["passing_date"], out dateValue) == true)
                {
                    objEqp.passing_date = dateValue;
                }
                if (DateTime.TryParse(form["reg_date"], out dateValue) == true)
                {
                    objEqp.reg_date = dateValue;
                }
                if (DateTime.TryParse(form["exp_date"], out dateValue) == true)
                {
                    objEqp.exp_date = dateValue;
                }
               
               
                if (objEqp.FunAddEquipmentDetails(objEqp))
                {
                    TempData["Successdata"] = "Equipment details added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEquipmentDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            
            return RedirectToAction("TrailerList");
        }

         
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddVehicleDetails(EquipmentDetailModels objEqp, FormCollection form,
             IEnumerable<HttpPostedFileBase> vehicle_upload, HttpPostedFileBase civildefence_upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
              
                objEqp.status = form["status"];
                objEqp.Cat_name = form["Cat_name"];

                string sCurrentDriver = objGlobaldata.GetEmpHrIDByName(form["driver"]);
                objEqp.driver = sCurrentDriver;
                DateTime dateValue;
                if (DateTime.TryParse(form["vehicle_renewal_date"], out dateValue) == true)
                {
                    objEqp.vehicle_renewal_date = dateValue;
                }
                if (DateTime.TryParse(form["vehicle_renewal_due_date"], out dateValue) == true)
                {
                    objEqp.vehicle_renewal_due_date = dateValue;
                }
                if (DateTime.TryParse(form["card_expdate"], out dateValue) == true)
                {
                    objEqp.card_expdate = dateValue;
                }
                if (DateTime.TryParse(form["civildefence_expiry"], out dateValue) == true)
                {
                    objEqp.civildefence_expiry = dateValue;
                }
                if (civildefence_upload != null && civildefence_upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(civildefence_upload.FileName));
                        string sFilename = "Civildefence" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        civildefence_upload.SaveAs(sFilepath + "/" + sFilename);
                        objEqp.civildefence_upload = "~/DataUpload/MgmtDocs/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in civildefence-upload: " + ex.ToString());
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (vehicle_upload != null && files.ContentLength > 0)
                {
                    objEqp.vehicle_upload = "";
                    foreach (var file in vehicle_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                            string sFilename = "VehicleCard" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objEqp.vehicle_upload = objEqp.vehicle_upload + "," + "~/DataUpload/MgmtDocs/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in vehicle-upload: " + ex.ToString());

                        }
                    }
                    objEqp.vehicle_upload = objEqp.vehicle_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objEqp.FunAddVehicleDetails(objEqp))
                {
                    TempData["Successdata"] = "Vehicle details added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVehicleDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EquipmentDetailsList");
        }

         
        [AllowAnonymous]
        public ActionResult EquipmentDetailsList(int? page, string SearchText)
        {
            EquipmentDetailModelsList objEqpList = new EquipmentDetailModelsList();
            objEqpList.lstCat = new List<EquipmentDetailModels>();
            EquipmentCategoryModels objEqp=new EquipmentCategoryModels();

            try
            {
                string sSqlstmt = "select id_equipment,Cat_name,vehicle_no,vehicle_reg,vehicle_renewal_date,vehicle_renewal_due_date,"
                + "vehicle_upload,"
                + "vehicle_model,vehicle_make,chasis_no,engine_no,location,gpsno,status,vehicle_color,plate_type,driver,card,card_no,"
                +"card_expdate,ADPC_stickers,ADPC_RFID,star_energy,GCC_drivers,civildefence_card,civildefence_expiry from t_truck_detail where Active=1";

                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + "  and (vehicle_no ='" + SearchText + "' or vehicle_no like '" + SearchText + "%' or vehicle_no like '%" + SearchText + "%' or vehicle_no like '%" + SearchText + "'"
                    +" or vehicle_model ='" + SearchText + "' or vehicle_model like '" + SearchText + "%' or vehicle_model like '%" + SearchText + "%' or vehicle_model like '%" + SearchText + "'"
                    + " or vehicle_make ='" + SearchText + "' or vehicle_make like '" + SearchText + "%' or vehicle_make like '%" + SearchText + "%' or vehicle_make like '%" + SearchText + "')"; 
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_equipment desc";

                DataSet dsEqpList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEqpList.Tables.Count > 0 && dsEqpList.Tables[0].Rows.Count > 0)
                {
                   
                        if (objGlobaldata.GetCurrentUserSession().DeleteFlag.ToLower() == "y")
                        {
                            ViewBag.DeleteFlg = true;
                        }
                    
                    for (int i = 0; i < dsEqpList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EquipmentDetailModels objEqpModel = new EquipmentDetailModels
                            {
                                id_equipment = Convert.ToInt16(dsEqpList.Tables[0].Rows[i]["id_equipment"].ToString()),
                                Cat_name =objEqp.getCategoryNamById(dsEqpList.Tables[0].Rows[i]["Cat_name"].ToString()),
                                vehicle_no =dsEqpList.Tables[0].Rows[i]["vehicle_no"].ToString(),
                                vehicle_reg =dsEqpList.Tables[0].Rows[i]["vehicle_reg"].ToString(),
                                vehicle_model = dsEqpList.Tables[0].Rows[i]["vehicle_model"].ToString(),
                                vehicle_make = dsEqpList.Tables[0].Rows[i]["vehicle_make"].ToString(),
                                chasis_no = dsEqpList.Tables[0].Rows[i]["chasis_no"].ToString(),
                                engine_no = dsEqpList.Tables[0].Rows[i]["engine_no"].ToString(),
                                location = dsEqpList.Tables[0].Rows[i]["location"].ToString(),
                                gpsno = dsEqpList.Tables[0].Rows[i]["gpsno"].ToString(),
                                status =objGlobaldata.getVehicleStatusById(dsEqpList.Tables[0].Rows[i]["status"].ToString()),
                                vehicle_color = dsEqpList.Tables[0].Rows[i]["vehicle_color"].ToString(),
                                plate_type = dsEqpList.Tables[0].Rows[i]["plate_type"].ToString(),
                                driver =objGlobaldata.GetEmpHrNameById(dsEqpList.Tables[0].Rows[i]["driver"].ToString()),
                                card = dsEqpList.Tables[0].Rows[i]["card"].ToString(),
                                card_no = dsEqpList.Tables[0].Rows[i]["card_no"].ToString(),
                                ADPC_stickers = dsEqpList.Tables[0].Rows[i]["ADPC_stickers"].ToString(),
                                ADPC_RFID = dsEqpList.Tables[0].Rows[i]["ADPC_RFID"].ToString(),
                                star_energy = dsEqpList.Tables[0].Rows[i]["star_energy"].ToString(),
                                GCC_drivers = dsEqpList.Tables[0].Rows[i]["GCC_drivers"].ToString(),
                                civildefence_card = dsEqpList.Tables[0].Rows[i]["civildefence_card"].ToString(),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsEqpList.Tables[0].Rows[i]["vehicle_renewal_date"].ToString(), out dtValue))
                            {
                                objEqpModel.vehicle_renewal_date = dtValue;
                            }
                            if (DateTime.TryParse(dsEqpList.Tables[0].Rows[i]["vehicle_renewal_due_date"].ToString(), out dtValue))
                            {
                                objEqpModel.vehicle_renewal_due_date = dtValue;
                            }
                            if (DateTime.TryParse(dsEqpList.Tables[0].Rows[i]["card_expdate"].ToString(), out dtValue))
                            {
                                objEqpModel.card_expdate = dtValue;
                            }
                             if (DateTime.TryParse(dsEqpList.Tables[0].Rows[i]["civildefence_expiry"].ToString(), out dtValue))
                            {
                                objEqpModel.civildefence_expiry = dtValue;
                            }
                            objEqpList.lstCat.Add(objEqpModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in EquipmentDetailsList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentDetailsList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objEqpList.lstCat.ToList().ToPagedList(page ?? 1, 40));
        }

         
        [AllowAnonymous]
        public ActionResult TrailerList(int? page, string SearchText)
        {
            EquipmentDetailModelsList objEqpList = new EquipmentDetailModelsList();
            objEqpList.lstCat = new List<EquipmentDetailModels>();
            EquipmentCategoryModels objEqp = new EquipmentCategoryModels();

            try
            {
                string sSqlstmt = "select id_trailer,Cat_name,trailerno,trailerreg,reg_date,exp_date,passing_date,"
                +"cur_location,yom,chasisno,trailerlen,trailertype,height,company,GVW,suspensiontype,remarks"
                + " from t_trailer_detail where Active=1 order by id_trailer desc";

                DataSet dsEqpList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEqpList.Tables.Count > 0 && dsEqpList.Tables[0].Rows.Count > 0)
                {
                   
                        if (objGlobaldata.GetCurrentUserSession().DeleteFlag.ToLower() == "y")
                        {
                            ViewBag.DeleteFlg = true;
                        }
                    
                    for (int i = 0; i < dsEqpList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EquipmentDetailModels objEqpModel = new EquipmentDetailModels
                            {
                                id_trailer = Convert.ToInt16(dsEqpList.Tables[0].Rows[i]["id_trailer"].ToString()),
                                Cat_name = objEqp.getCategoryNamById(dsEqpList.Tables[0].Rows[i]["Cat_name"].ToString()),
                                trailerno = dsEqpList.Tables[0].Rows[i]["trailerno"].ToString(),
                                trailerreg = dsEqpList.Tables[0].Rows[i]["trailerreg"].ToString(),
                                cur_location = dsEqpList.Tables[0].Rows[i]["cur_location"].ToString(),
                                yom = dsEqpList.Tables[0].Rows[i]["yom"].ToString(),
                                chasisno = dsEqpList.Tables[0].Rows[i]["chasisno"].ToString(),
                                trailerlen = dsEqpList.Tables[0].Rows[i]["trailerlen"].ToString(),
                                trailertype =objGlobaldata.getTrailerTypeById(dsEqpList.Tables[0].Rows[i]["trailertype"].ToString()),
                                height = dsEqpList.Tables[0].Rows[i]["height"].ToString(),
                                company =objGlobaldata.getTrailerCompanyById(dsEqpList.Tables[0].Rows[i]["company"].ToString()),
                                GVW = dsEqpList.Tables[0].Rows[i]["GVW"].ToString(),
                                suspensiontype =objGlobaldata.getSuspensionTypeById(dsEqpList.Tables[0].Rows[i]["suspensiontype"].ToString()),
                                remarks = dsEqpList.Tables[0].Rows[i]["remarks"].ToString(),
                                
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsEqpList.Tables[0].Rows[i]["reg_date"].ToString(), out dtValue))
                            {
                                objEqpModel.reg_date = dtValue;
                            }
                            if (DateTime.TryParse(dsEqpList.Tables[0].Rows[i]["exp_date"].ToString(), out dtValue))
                            {
                                objEqpModel.exp_date = dtValue;
                            }
                            if (DateTime.TryParse(dsEqpList.Tables[0].Rows[i]["passing_date"].ToString(), out dtValue))
                            {
                                objEqpModel.passing_date = dtValue;
                            }
                            objEqpList.lstCat.Add(objEqpModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrailerList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrailerList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objEqpList.lstCat.ToList().ToPagedList(page ?? 1, 40));
        }

         
        [AllowAnonymous]
        public ActionResult VehicleDetailsEdit()
        {
            EquipmentDetailModels objEqpModel = new EquipmentDetailModels();
            EquipmentCategoryModels objEqp = new EquipmentCategoryModels();
            try
            {
                ViewBag.Category = objGlobaldata.getCategories();
                ViewBag.Vehicle = objEqpModel.getVehicleList();
                ViewBag.Equipment = objEqpModel.getEquipmentList();
                ViewBag.VehicleStatus = objGlobaldata.getVehicleStatusList();
                ViewBag.Emirates = objGlobaldata.GetConstantValue("Emirates");
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.TrailerType = objGlobaldata.getTrailerTypeList();
                ViewBag.Company = objGlobaldata.getTrailerCompanyList();
                ViewBag.Suspension = objGlobaldata.getSuspensionTypeList();
                if (Request.QueryString["id_equipment"] != null && Request.QueryString["id_equipment"] != "")
                {
                    string sid_equipment = Request.QueryString["id_equipment"];

                    string sSqlstmt = "select id_equipment,Cat_name,vehicle_no,vehicle_reg,vehicle_renewal_date,"
                    + "vehicle_renewal_due_date,vehicle_upload,"
                    + "vehicle_model,vehicle_make,chasis_no,"
                    + "engine_no,location,gpsno,status,vehicle_color,plate_type,driver,card,card_no,"
                    + "card_expdate,ADPC_stickers,ADPC_RFID,star_energy,GCC_drivers,civildefence_card,civildefence_upload,civildefence_expiry from t_truck_detail"
                    + " where id_equipment='" + sid_equipment + "'";

                    DataSet dsEqpList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEqpList.Tables.Count > 0 && dsEqpList.Tables[0].Rows.Count > 0)
                    {

                        objEqpModel = new EquipmentDetailModels
                        {
                            id_equipment = Convert.ToInt16(dsEqpList.Tables[0].Rows[0]["id_equipment"].ToString()),
                            Cat_name = objEqp.getCategoryNamById(dsEqpList.Tables[0].Rows[0]["Cat_name"].ToString()),
                            vehicle_no = dsEqpList.Tables[0].Rows[0]["vehicle_no"].ToString(),
                            vehicle_reg = dsEqpList.Tables[0].Rows[0]["vehicle_reg"].ToString(),

                            vehicle_upload = dsEqpList.Tables[0].Rows[0]["vehicle_upload"].ToString(),
                            vehicle_model = dsEqpList.Tables[0].Rows[0]["vehicle_model"].ToString(),
                            vehicle_make = dsEqpList.Tables[0].Rows[0]["vehicle_make"].ToString(),
                            chasis_no = dsEqpList.Tables[0].Rows[0]["chasis_no"].ToString(),
                            engine_no = dsEqpList.Tables[0].Rows[0]["engine_no"].ToString(),
                            location = dsEqpList.Tables[0].Rows[0]["location"].ToString(),
                            gpsno = dsEqpList.Tables[0].Rows[0]["gpsno"].ToString(),
                            status = objGlobaldata.getVehicleStatusById(dsEqpList.Tables[0].Rows[0]["status"].ToString()),
                            vehicle_color = dsEqpList.Tables[0].Rows[0]["vehicle_color"].ToString(),
                            plate_type = dsEqpList.Tables[0].Rows[0]["plate_type"].ToString(),
                            driver =objGlobaldata.GetEmpHrNameById(dsEqpList.Tables[0].Rows[0]["driver"].ToString()),
                            card = dsEqpList.Tables[0].Rows[0]["card"].ToString(),
                            card_no = dsEqpList.Tables[0].Rows[0]["card_no"].ToString(),
                            ADPC_stickers = dsEqpList.Tables[0].Rows[0]["ADPC_stickers"].ToString(),
                            ADPC_RFID = dsEqpList.Tables[0].Rows[0]["ADPC_RFID"].ToString(),
                            star_energy = dsEqpList.Tables[0].Rows[0]["star_energy"].ToString(),
                            GCC_drivers = dsEqpList.Tables[0].Rows[0]["GCC_drivers"].ToString(),
                            civildefence_card = dsEqpList.Tables[0].Rows[0]["civildefence_card"].ToString(),
                            civildefence_upload = dsEqpList.Tables[0].Rows[0]["civildefence_upload"].ToString(),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["vehicle_renewal_date"].ToString(), out dtValue))
                        {
                            objEqpModel.vehicle_renewal_date = dtValue;
                        }
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["vehicle_renewal_due_date"].ToString(), out dtValue))
                        {
                            objEqpModel.vehicle_renewal_due_date = dtValue;
                        }

                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["card_expdate"].ToString(), out dtValue))
                        {
                            objEqpModel.card_expdate = dtValue;
                        }
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["civildefence_expiry"].ToString(), out dtValue))
                        {
                            objEqpModel.civildefence_expiry = dtValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Equipment ID cannot be Null or empty";
                        return RedirectToAction("EquipmentDetailsList");
                    }
                }     
                else
                {

                    TempData["alertdata"] = "Equipment ID cannot be Null or empty";
                    return RedirectToAction("EquipmentDetailsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentDetailsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EquipmentDetailsList");
            }
            return View(objEqpModel);
        }

         
        [AllowAnonymous]
        public ActionResult EquipmentDetailsEdit()
        {
            EquipmentDetailModels objEqpModel = new EquipmentDetailModels();
            EquipmentCategoryModels objEqp = new EquipmentCategoryModels();
            try
            {
                ViewBag.Category = objGlobaldata.getCategories();
                ViewBag.Vehicle = objEqpModel.getVehicleList();
                ViewBag.Equipment = objEqpModel.getEquipmentList();
                ViewBag.VehicleStatus = objGlobaldata.getVehicleStatusList();
                ViewBag.Emirates = objGlobaldata.GetConstantValue("Emirates");
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.TrailerType = objGlobaldata.getTrailerTypeList();
                ViewBag.Company = objGlobaldata.getTrailerCompanyList();
                ViewBag.Suspension = objGlobaldata.getSuspensionTypeList();
               
                if (Request.QueryString["id_trailer"] != null && Request.QueryString["id_trailer"] != "")
                {
                    string sid_trailer = Request.QueryString["id_trailer"];

                    string sSqlstmt = "select id_trailer,Cat_name,trailerno,trailerreg,reg_date,exp_date,passing_date,"
                + "cur_location,yom,chasisno,trailerlen,trailertype,height,company,GVW,suspensiontype,remarks from t_trailer_detail"
                    + " where id_trailer='" + sid_trailer + "'";

                    DataSet dsEqpList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEqpList.Tables.Count > 0 && dsEqpList.Tables[0].Rows.Count > 0)
                    {

                        objEqpModel = new EquipmentDetailModels
                        {
                            id_trailer = Convert.ToInt16(dsEqpList.Tables[0].Rows[0]["id_trailer"].ToString()),
                            Cat_name = objEqp.getCategoryNamById(dsEqpList.Tables[0].Rows[0]["Cat_name"].ToString()),
                            trailerno = dsEqpList.Tables[0].Rows[0]["trailerno"].ToString(),
                            trailerreg = dsEqpList.Tables[0].Rows[0]["trailerreg"].ToString(),
                            cur_location = dsEqpList.Tables[0].Rows[0]["cur_location"].ToString(),
                            yom = dsEqpList.Tables[0].Rows[0]["yom"].ToString(),
                            chasisno = dsEqpList.Tables[0].Rows[0]["chasisno"].ToString(),
                            trailerlen = dsEqpList.Tables[0].Rows[0]["trailerlen"].ToString(),
                            trailertype = objGlobaldata.getTrailerTypeById(dsEqpList.Tables[0].Rows[0]["trailertype"].ToString()),
                            height = dsEqpList.Tables[0].Rows[0]["height"].ToString(),
                            company = objGlobaldata.getTrailerCompanyById(dsEqpList.Tables[0].Rows[0]["company"].ToString()),
                            GVW = dsEqpList.Tables[0].Rows[0]["GVW"].ToString(),
                            suspensiontype = objGlobaldata.getSuspensionTypeById(dsEqpList.Tables[0].Rows[0]["suspensiontype"].ToString()),
                            remarks = dsEqpList.Tables[0].Rows[0]["remarks"].ToString(),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["reg_date"].ToString(), out dtValue))
                        {
                            objEqpModel.reg_date = dtValue;
                        }
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["exp_date"].ToString(), out dtValue))
                        {
                            objEqpModel.exp_date = dtValue;
                        }
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["passing_date"].ToString(), out dtValue))
                        {
                            objEqpModel.passing_date = dtValue;
                        }
                    }
                    else
                    {

                        TempData["alertdata"] = "Equipment ID cannot be Null or empty";
                        return RedirectToAction("EquipmentDetailsList");
                    }
                }
                else
                {

                    TempData["alertdata"] = "Equipment ID cannot be Null or empty";
                    return RedirectToAction("EquipmentDetailsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentDetailsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EquipmentDetailsList");
            }
            return View(objEqpModel);
        }


         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VehicleDetailsEdit(EquipmentDetailModels objEqp, FormCollection form, IEnumerable<HttpPostedFileBase> vehicle_upload
            , HttpPostedFileBase civildefence_upload)
        {
            try
            {
                string QCDelete = Request.Form["QCDocsValselectall"];
                HttpPostedFileBase files = Request.Files[0];
               
                objEqp.status = form["status"];
                objEqp.Cat_name = form["Cat_name"];

                string sCurrentDriver = objGlobaldata.GetEmpHrIDByName(form["driver"]);
                objEqp.driver = sCurrentDriver;
              
                DateTime dateValue;
                if (DateTime.TryParse(form["vehicle_renewal_date"], out dateValue) == true)
                {
                    objEqp.vehicle_renewal_date = dateValue;
                }
                if (DateTime.TryParse(form["vehicle_renewal_due_date"], out dateValue) == true)
                {
                    objEqp.vehicle_renewal_due_date = dateValue;
                }
                        
                if (DateTime.TryParse(form["card_expdate"], out dateValue) == true)
                {
                    objEqp.card_expdate = dateValue;
                }
                if (DateTime.TryParse(form["civildefence_expiry"], out dateValue) == true)
                {
                    objEqp.civildefence_expiry = dateValue;
                }
                if (civildefence_upload != null && civildefence_upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(civildefence_upload.FileName));
                        string sFilename = "Civildefence" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        civildefence_upload.SaveAs(sFilepath + "/" + sFilename);
                        objEqp.civildefence_upload = "~/DataUpload/MgmtDocs/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in civildefence-upload: " + ex.ToString());
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (vehicle_upload != null && files.ContentLength > 0)
                {
                    objEqp.vehicle_upload = "";
                    foreach (var file in vehicle_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                            string sFilename = "VehicleCard" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objEqp.vehicle_upload = objEqp.vehicle_upload + "," + "~/DataUpload/MgmtDocs/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in vehicle-upload: " + ex.ToString());

                        }
                    }
                    objEqp.vehicle_upload = objEqp.vehicle_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objEqp.vehicle_upload = objEqp.vehicle_upload + "," + form["QCDocsVal"];
                    objEqp.vehicle_upload = objEqp.vehicle_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objEqp.vehicle_upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objEqp.vehicle_upload = null;
                }
                if (objEqp.FunUpdateVehicleDetails(objEqp))
                {
                    TempData["Successdata"] = "Vehicle details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in VehicleDetailsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EquipmentDetailsList");
            }

            return RedirectToAction("EquipmentDetailsList");
        }

         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EquipmentDetailsEdit(EquipmentDetailModels objEqp, FormCollection form)
        {
            try
            {
                string QCDelete = Request.Form["QCDocsValselectall"];
                objEqp.trailertype = form["trailertype"];
                objEqp.company = form["company"];
                objEqp.suspensiontype = form["suspensiontype"];
                objEqp.Cat_name = form["Cat_name"];

                DateTime dateValue;
                
                if (DateTime.TryParse(form["passing_date"], out dateValue) == true)
                {
                    objEqp.passing_date = dateValue;
                }
                if (DateTime.TryParse(form["reg_date"], out dateValue) == true)
                {
                    objEqp.reg_date = dateValue;
                }
                if (DateTime.TryParse(form["exp_date"], out dateValue) == true)
                {
                    objEqp.exp_date = dateValue;
                }

               
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objEqp.vehicle_upload = objEqp.vehicle_upload + "," + form["QCDocsVal"];
                    objEqp.vehicle_upload = objEqp.vehicle_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null)
                {
                    objEqp.vehicle_upload = null;
                }

                if (objEqp.FunUpdateEquipmentDetails(objEqp))
                {
                    TempData["Successdata"] = "Equipment details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentDetailsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EquipmentDetailsList");
            }
            return RedirectToAction("TrailerList");
        }

         
        [AllowAnonymous]
        public ActionResult EquipmentDetails()
        {
            EquipmentDetailModels objEqpModel = new EquipmentDetailModels();
            EquipmentCategoryModels objEqp = new EquipmentCategoryModels();
            try
            {
                if (Request.QueryString["id_trailer"] != null && Request.QueryString["id_trailer"] != "")
                {
                    string sid_trailer = Request.QueryString["id_trailer"];

                    string sSqlstmt = "select id_trailer,Cat_name,trailerno,trailerreg,reg_date,exp_date,passing_date,"
                + "cur_location,yom,chasisno,trailerlen,trailertype,height,company,GVW,suspensiontype,remarks from t_trailer_detail"
                    + " where id_trailer='" + sid_trailer + "'";

                    DataSet dsEqpList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEqpList.Tables.Count > 0 && dsEqpList.Tables[0].Rows.Count > 0)
                    {

                        objEqpModel = new EquipmentDetailModels
                        {
                            id_trailer = Convert.ToInt16(dsEqpList.Tables[0].Rows[0]["id_trailer"].ToString()),
                            Cat_name = objEqp.getCategoryNamById(dsEqpList.Tables[0].Rows[0]["Cat_name"].ToString()),
                            trailerno = dsEqpList.Tables[0].Rows[0]["trailerno"].ToString(),
                            trailerreg = dsEqpList.Tables[0].Rows[0]["trailerreg"].ToString(),
                            cur_location = dsEqpList.Tables[0].Rows[0]["cur_location"].ToString(),
                            yom = dsEqpList.Tables[0].Rows[0]["yom"].ToString(),
                            chasisno = dsEqpList.Tables[0].Rows[0]["chasisno"].ToString(),
                            trailerlen = dsEqpList.Tables[0].Rows[0]["trailerlen"].ToString(),
                            trailertype = objGlobaldata.getTrailerTypeById(dsEqpList.Tables[0].Rows[0]["trailertype"].ToString()),
                            height = dsEqpList.Tables[0].Rows[0]["height"].ToString(),
                            company = objGlobaldata.getTrailerCompanyById(dsEqpList.Tables[0].Rows[0]["company"].ToString()),
                            GVW = dsEqpList.Tables[0].Rows[0]["GVW"].ToString(),
                            suspensiontype = objGlobaldata.getSuspensionTypeById(dsEqpList.Tables[0].Rows[0]["suspensiontype"].ToString()),
                            remarks = dsEqpList.Tables[0].Rows[0]["remarks"].ToString(),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["reg_date"].ToString(), out dtValue))
                        {
                            objEqpModel.reg_date = dtValue;
                        }
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["exp_date"].ToString(), out dtValue))
                        {
                            objEqpModel.exp_date = dtValue;
                        }
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["passing_date"].ToString(), out dtValue))
                        {
                            objEqpModel.passing_date = dtValue;
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EquipmentDetailsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objEqpModel);

        }

         
        [AllowAnonymous]
        public ActionResult VehicleDetails()
        {
            EquipmentDetailModels objEqpModel = new EquipmentDetailModels();
            EquipmentCategoryModels objEqp = new EquipmentCategoryModels();
            try
            {
                if (Request.QueryString["id_equipment"] != null && Request.QueryString["id_equipment"] != "")
                {
                    string sid_equipment = Request.QueryString["id_equipment"];

                    string sSqlstmt = "select id_equipment,Cat_name,vehicle_no,vehicle_reg,vehicle_renewal_date,"
                    + "vehicle_renewal_due_date,vehicle_upload,"
                    + "vehicle_model,vehicle_make,chasis_no,"
                    + "engine_no,location,gpsno,status,vehicle_color,plate_type,driver,card,card_no,"
                    + "card_expdate,ADPC_stickers,ADPC_RFID,star_energy,GCC_drivers,civildefence_card,civildefence_upload,civildefence_expiry from t_truck_detail"
                    + " where id_equipment='" + sid_equipment + "'";

                    DataSet dsEqpList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEqpList.Tables.Count > 0 && dsEqpList.Tables[0].Rows.Count > 0)
                    {
                        objEqpModel = new EquipmentDetailModels
                        {
                            id_equipment = Convert.ToInt16(dsEqpList.Tables[0].Rows[0]["id_equipment"].ToString()),
                            Cat_name = objEqp.getCategoryNamById(dsEqpList.Tables[0].Rows[0]["Cat_name"].ToString()),
                            vehicle_no = dsEqpList.Tables[0].Rows[0]["vehicle_no"].ToString(),
                            vehicle_reg = dsEqpList.Tables[0].Rows[0]["vehicle_reg"].ToString(),
                            vehicle_upload = dsEqpList.Tables[0].Rows[0]["vehicle_upload"].ToString(),
                            vehicle_model = dsEqpList.Tables[0].Rows[0]["vehicle_model"].ToString(),
                            vehicle_make = dsEqpList.Tables[0].Rows[0]["vehicle_make"].ToString(),
                            chasis_no = dsEqpList.Tables[0].Rows[0]["chasis_no"].ToString(),
                            engine_no = dsEqpList.Tables[0].Rows[0]["engine_no"].ToString(),
                            location = dsEqpList.Tables[0].Rows[0]["location"].ToString(),
                            gpsno = dsEqpList.Tables[0].Rows[0]["gpsno"].ToString(),
                            status = objGlobaldata.getVehicleStatusById(dsEqpList.Tables[0].Rows[0]["status"].ToString()),
                            vehicle_color = dsEqpList.Tables[0].Rows[0]["vehicle_color"].ToString(),
                            plate_type = dsEqpList.Tables[0].Rows[0]["plate_type"].ToString(),
                            driver =objGlobaldata.GetEmpHrNameById(dsEqpList.Tables[0].Rows[0]["driver"].ToString()),
                            card = dsEqpList.Tables[0].Rows[0]["card"].ToString(),
                            card_no = dsEqpList.Tables[0].Rows[0]["card_no"].ToString(),
                            ADPC_stickers = dsEqpList.Tables[0].Rows[0]["ADPC_stickers"].ToString(),
                            ADPC_RFID = dsEqpList.Tables[0].Rows[0]["ADPC_RFID"].ToString(),
                            star_energy = dsEqpList.Tables[0].Rows[0]["star_energy"].ToString(),
                            GCC_drivers = dsEqpList.Tables[0].Rows[0]["GCC_drivers"].ToString(),
                            civildefence_card = dsEqpList.Tables[0].Rows[0]["civildefence_card"].ToString(),
                            civildefence_upload = dsEqpList.Tables[0].Rows[0]["civildefence_upload"].ToString(),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["vehicle_renewal_date"].ToString(), out dtValue))
                        {
                            objEqpModel.vehicle_renewal_date = dtValue;
                        }
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["vehicle_renewal_due_date"].ToString(), out dtValue))
                        {
                            objEqpModel.vehicle_renewal_due_date = dtValue;
                        }

                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["card_expdate"].ToString(), out dtValue))
                        {
                            objEqpModel.card_expdate = dtValue;
                        }
                        if (DateTime.TryParse(dsEqpList.Tables[0].Rows[0]["civildefence_expiry"].ToString(), out dtValue))
                        {
                            objEqpModel.civildefence_expiry = dtValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("EquipmentDetailsList");
                    }
                }
                
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EquipmentDetailsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objEqpModel);

        }

        [AllowAnonymous]
        public JsonResult EquipmentDetailsDelete(FormCollection form)
        {
            try
            {
              
                    if (objGlobaldata.GetCurrentUserSession().DeleteFlag.ToLower() == "y")
                    {
                        if (form["id_equipment"] != null && form["id_equipment"] != "")
                        {

                            EquipmentDetailModels Doc = new EquipmentDetailModels();
                            string sid_equipment = form["id_equipment"];


                            if (Doc.FunDeleteEquipmentDetails(sid_equipment))
                            {
                                TempData["Successdata"] = "Vehicle details deleted successfully";
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
                            TempData["alertdata"] = "Equipment Id cannot be Null or empty";
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
                objGlobaldata.AddFunctionalLog("Exception in EquipmentDetailsDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

         
        [AllowAnonymous]
        public JsonResult TrailerDelete(FormCollection form)
        {
            try
            {
              
                    if (objGlobaldata.GetCurrentUserSession().DeleteFlag.ToLower() == "y")
                    {
                        if (form["id_trailer"] != null && form["id_trailer"] != "")
                        {

                            EquipmentDetailModels Doc = new EquipmentDetailModels();
                            string sid_trailer = form["id_trailer"];


                            if (Doc.FunDeleteTrailer(sid_trailer))
                            {
                                TempData["Successdata"] = "Equipment details deleted successfully";
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
                            TempData["alertdata"] = "Equipment Id cannot be Null or empty";
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
                objGlobaldata.AddFunctionalLog("Exception in TrailerDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

         
        [AllowAnonymous]
        public JsonResult EquipmentDelete(FormCollection form)
        {
            try
            {
              
                    if (objGlobaldata.GetCurrentUserSession().DeleteFlag.ToLower() == "y")
                    {
                        if (form["id_cat"] != null && form["id_cat"] != "")
                        {

                            EquipmentCategoryModels Doc = new EquipmentCategoryModels();
                            string sid_cat = form["id_cat"];

                            if (Doc.FunDeleteEquipment(sid_cat))
                            {
                                TempData["Successdata"] = "Equipment deleted successfully";
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
                            TempData["alertdata"] = "Equipment Id cannot be Null or empty";
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
                objGlobaldata.AddFunctionalLog("Exception in EquipmentDetailsDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

         
        public ActionResult AddVehicleInsuranceDetails()
        {
            VehiclePassModels objInsurance = new VehiclePassModels();
            try
            {
                ViewBag.InsuranceType = objGlobaldata.getVehicleInsuranceTypeList();
                if (Request.QueryString["id_equipment"] != null && Request.QueryString["id_equipment"] != "")
                {
                    string sid_equipment = Request.QueryString["id_equipment"];
                    string sSqlStmt = "select id_equipment from t_truck_detail where id_equipment='" + sid_equipment + "'";
                    DataSet dsInsList = objGlobaldata.Getdetails(sSqlStmt);
                    if (dsInsList.Tables.Count > 0 && dsInsList.Tables[0].Rows.Count > 0)
                    {
                        objInsurance = new VehiclePassModels
                        {
                            id_equipment = Convert.ToInt32(dsInsList.Tables[0].Rows[0]["id_equipment"].ToString()),

                        };
                        return View(objInsurance);
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EquipmentDetailsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVehicleInsuranceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("EquipmentDetailsList");
        }

         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddVehicleInsuranceDetails(VehiclePassModels objVehicle, FormCollection form)
        {
            try
            {

                VehiclePassModelsList lstIns = new VehiclePassModelsList();
                lstIns.lstPass = new List<VehiclePassModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    VehiclePassModels objInsurance = new VehiclePassModels();
                    DateTime dateValues;
                    objInsurance.Insurance_type = form["Insurance_type" + i];
                    objInsurance.Upload = form["Upload" + i];
                    objInsurance.Mulki_upload = form["Mulki_upload" + i];
                    objInsurance.Insurance_cover = form["Insurance_cover" + i];
                    if (DateTime.TryParse(form["Insurance_date" + i], out dateValues) == true)
                    {
                        objInsurance.Insurance_date = dateValues;
                    }
                    if (DateTime.TryParse(form["Insurance_expdate" + i], out dateValues) == true)
                    {
                        objInsurance.Insurance_expdate = dateValues;
                    }

                    lstIns.lstPass.Add(objInsurance);
                }

                if (objVehicle.FunAddInsuranceDetails(objVehicle, lstIns))
                {
                    TempData["Successdata"] = "Insurance details added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVehicleInsuranceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EquipmentDetailsList");
        }

         
        [AllowAnonymous]
        public ActionResult InsuranceList(string SearchText, int? page)
        {

            VehiclePassModelsList Inslst = new VehiclePassModelsList();
            Inslst.lstPass = new List<VehiclePassModels>();

            try
            {
                if (Request.QueryString["id_equipment"] != null && Request.QueryString["id_equipment"] != "")
                {
                    string sid_equipment = Request.QueryString["id_equipment"];
                    string sSqlstmt = "select id_vehicle_pass,id_equipment,Insurance_type,Upload,Mulki_upload,Insurance_date,Insurance_expdate,Insurance_cover,Company"
                    + " from t_vehicle_insurance where id_equipment='" + sid_equipment + "' and Active=1 order by Insurance_type";

                    DataSet dsInsuranceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsInsuranceList.Tables.Count > 0 && dsInsuranceList.Tables[0].Rows.Count > 0)
                    {

                            if (objGlobaldata.GetCurrentUserSession().DeleteFlag.ToLower() == "y")
                            {
                                ViewBag.DeleteFlg = true;
                            }
                        
                        for (int i = 0; i < dsInsuranceList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                VehiclePassModels objInsurance = new VehiclePassModels
                                {
                                    id_vehicle_pass = Convert.ToInt32(dsInsuranceList.Tables[0].Rows[i]["id_vehicle_pass"].ToString()),
                                    id_equipment = Convert.ToInt32(dsInsuranceList.Tables[0].Rows[i]["id_equipment"].ToString()),
                                    Insurance_type = objGlobaldata.getVehicleInsuranceTypeById(dsInsuranceList.Tables[0].Rows[i]["Insurance_type"].ToString()),
                                    Upload = dsInsuranceList.Tables[0].Rows[i]["Upload"].ToString(),
                                    Mulki_upload = dsInsuranceList.Tables[0].Rows[i]["Mulki_upload"].ToString(),
                                    Insurance_cover = dsInsuranceList.Tables[0].Rows[i]["Insurance_cover"].ToString(),
                                    Company = dsInsuranceList.Tables[0].Rows[i]["Company"].ToString()
                                };

                                DateTime dtDate;
                                if (DateTime.TryParse(dsInsuranceList.Tables[0].Rows[i]["Insurance_date"].ToString(), out dtDate))
                                {
                                    objInsurance.Insurance_date = dtDate;
                                }
                                if (DateTime.TryParse(dsInsuranceList.Tables[0].Rows[i]["Insurance_expdate"].ToString(), out dtDate))
                                {
                                    objInsurance.Insurance_expdate = dtDate;
                                }
                                Inslst.lstPass.Add(objInsurance);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in InsuranceList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EquipmentDetailsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InsuranceList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(Inslst.lstPass.ToList().ToPagedList(page ?? 1, 10000));
        }

         
        [AllowAnonymous]
        public ActionResult InsuranceEdit()
        {

            VehiclePassModels objIns = new VehiclePassModels();

            try
            {
                ViewBag.InsuranceType = objGlobaldata.getVehicleInsuranceTypeList();
                if (Request.QueryString["id_vehicle_pass"] != null && Request.QueryString["id_vehicle_pass"] != "")
                {
                    string sid_vehicle_pass = Request.QueryString["id_vehicle_pass"];
                    string sSqlstmt = "select id_vehicle_pass,id_equipment,Insurance_type,Upload,Mulki_upload,Insurance_date,"
                    + "Insurance_expdate,Insurance_cover,Company from t_vehicle_insurance where id_vehicle_pass='" + sid_vehicle_pass + "'";

                    DataSet dsInsuranceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsInsuranceList.Tables.Count > 0 && dsInsuranceList.Tables[0].Rows.Count > 0)
                    {

                        objIns = new VehiclePassModels
                        {
                            id_vehicle_pass = Convert.ToInt32(dsInsuranceList.Tables[0].Rows[0]["id_vehicle_pass"].ToString()),
                            id_equipment = Convert.ToInt32(dsInsuranceList.Tables[0].Rows[0]["id_equipment"].ToString()),
                            Insurance_type = objGlobaldata.getVehicleInsuranceTypeById(dsInsuranceList.Tables[0].Rows[0]["Insurance_type"].ToString()),
                            Upload = dsInsuranceList.Tables[0].Rows[0]["Upload"].ToString(),
                            Mulki_upload = dsInsuranceList.Tables[0].Rows[0]["Mulki_upload"].ToString(),
                            Insurance_cover = dsInsuranceList.Tables[0].Rows[0]["Insurance_cover"].ToString(),
                            Company = dsInsuranceList.Tables[0].Rows[0]["Company"].ToString()

                        };
                        DateTime dtDate;
                        if (DateTime.TryParse(dsInsuranceList.Tables[0].Rows[0]["Insurance_date"].ToString(), out dtDate))
                        {
                            objIns.Insurance_date = dtDate;
                        }
                        if (DateTime.TryParse(dsInsuranceList.Tables[0].Rows[0]["Insurance_expdate"].ToString(), out dtDate))
                        {
                            objIns.Insurance_expdate = dtDate;
                        }
                    }
                    else
                    {

                        TempData["alertdata"] = "InsuranceID cannot be Null or empty";
                        return RedirectToAction("EquipmentDetailsList");
                    }
                }
                else
                {

                    TempData["alertdata"] = "InsuranceID cannot be Null or empty";
                    return RedirectToAction("EquipmentDetailsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InsuranceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EquipmentDetailsList");
            }
            return View(objIns);
        }

         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsuranceEdit(VehiclePassModels objVehicle, FormCollection form, HttpPostedFileBase Upload, HttpPostedFileBase Mulki_upload)
        {
            try
            {
                objVehicle.Insurance_type = form["Insurance_type"];

                DateTime dateValue;

                if (DateTime.TryParse(form["Insurance_date"], out dateValue) == true)
                {
                    objVehicle.Insurance_date = dateValue;
                }
                if (DateTime.TryParse(form["Insurance_expdate"], out dateValue) == true)
                {
                    objVehicle.Insurance_expdate = dateValue;
                }
                if (Upload != null && Upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(Upload.FileName));
                        string sFilename = "VehicleInsurance" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Upload.SaveAs(sFilepath + "/" + sFilename);
                        objVehicle.Upload = "~/DataUpload/MgmtDocs/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in Upload: " + ex.ToString());
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (Mulki_upload != null && Mulki_upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(Mulki_upload.FileName));
                        string sFilename = "Mulki" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Mulki_upload.SaveAs(sFilepath + "/" + sFilename);
                        objVehicle.Mulki_upload = "~/DataUpload/MgmtDocs/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in Mulki_upload: " + ex.ToString());
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objVehicle.FunUpdateInsurance(objVehicle))
                {
                    TempData["Successdata"] = "Insurance details Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InsuranceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EquipmentDetailsList");
            }
            return RedirectToAction("EquipmentDetailsList");
        }

         
        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase Upload = Request.Files;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "VehicleInsurance" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/" + "VehicleInsurance" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
            }

            return Json("Failed");
        }

         
        [HttpPost]
        public JsonResult UploadPassDocument()
        {
            HttpFileCollectionBase Upload = Request.Files;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "VehiclePass" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/" + "VehiclePass" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
            }

            return Json("Failed");
        }

         
        [HttpPost]
        public JsonResult UploadMulkiDocument()
        {
            HttpFileCollectionBase Upload = Request.Files;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "Mulki" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/" + "Mulki" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
            }

            return Json("Failed");
        }

         
        public JsonResult InsuranceDelete(FormCollection form)
        {
          
            try
            {
               
                    if (objGlobaldata.GetCurrentUserSession().DeleteFlag.ToLower() == "y")
                    {
                        if (form["id_vehicle_pass"] != null && form["id_vehicle_pass"] != "")
                        {
                            VehiclePassModels obj = new VehiclePassModels();
                            string sid_vehicle_pass = form["id_vehicle_pass"];

                            if (obj.FunDeleteInsurance(sid_vehicle_pass))
                            {
                                TempData["Successdata"] = "Insurance details deleted successfully";

                            }
                            else
                            {
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return Json("Success");
                            }
                        }
                        else
                        {
                            TempData["alertdata"] = "Insurance Id cannot be Null or empty";
                            return Json("Failed");
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Access Denied";
                        return Json("Failed");
                    }
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InsuranceDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

         
        [HttpPost]
        public JsonResult GetEmployeeList(string driver)
        {
            objEmployeeList = objGlobaldata.GetEmployeeNameListboxForAutoCompMaster();

            List<string> lstFilteredList = objEmployeeList.FindAll(d => d.StartsWith(driver.ToUpper()));

            return Json(lstFilteredList);
        }

         
        public ActionResult AddVehiclePassDetails()
        {
            EquipmentDetailModels objPass = new EquipmentDetailModels();
            try
            {
                ViewBag.PassType = objGlobaldata.getVehiclePassTypeList();
                if (Request.QueryString["id_equipment"] != null && Request.QueryString["id_equipment"] != "")
                {
                    string sid_equipment = Request.QueryString["id_equipment"];
                    string sSqlStmt = "select id_equipment from t_truck_detail where id_equipment='" + sid_equipment + "'";
                    DataSet dsVehiclList = objGlobaldata.Getdetails(sSqlStmt);
                    if (dsVehiclList.Tables.Count > 0 && dsVehiclList.Tables[0].Rows.Count > 0)
                    {
                        objPass = new EquipmentDetailModels
                        {
                            id_equipment = Convert.ToInt32(dsVehiclList.Tables[0].Rows[0]["id_equipment"].ToString()),

                        };
                        return View(objPass);
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EquipmentDetailsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVehiclePassDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EquipmentDetailsList");
        }

         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddVehiclePassDetails(EquipmentDetailModels objPass, FormCollection form)
        {
            try
            {
                EquipmentDetailModelsList lstpass = new EquipmentDetailModelsList();
                lstpass.lstCat = new List<EquipmentDetailModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    EquipmentDetailModels objVehclPass = new EquipmentDetailModels();
                    DateTime dateValues;
                    objVehclPass.PassType = form["PassType" + i];
                    objVehclPass.Upload = form["Upload" + i];

                    if (DateTime.TryParse(form["ExpDate" + i], out dateValues) == true)
                    {
                        objVehclPass.ExpDate = dateValues;
                    }

                    lstpass.lstCat.Add(objVehclPass);
                }

                if (objPass.FunAddPassDetails(objPass, lstpass))
                {
                    TempData["Successdata"] = "Vehicle Pass details added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVehiclePassDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EquipmentDetailsList");
        }

         
        [AllowAnonymous]
        public ActionResult VehiclePassList(string SearchText, int? page)
        {

            EquipmentDetailModelsList lstpass = new EquipmentDetailModelsList();
            lstpass.lstCat = new List<EquipmentDetailModels>();

            try
            {
                if (Request.QueryString["id_equipment"] != null && Request.QueryString["id_equipment"] != "")
                {
                    string sid_equipment = Request.QueryString["id_equipment"];
                    string sSqlstmt = "select id_pass,id_equipment,PassType,Upload,ExpDate from t_truck_pass where id_equipment='" + sid_equipment + "' and Active=1 order by PassType";

                    DataSet dsPassList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPassList.Tables.Count > 0 && dsPassList.Tables[0].Rows.Count > 0)
                    {

                            if (objGlobaldata.GetCurrentUserSession().DeleteFlag.ToLower() == "y")
                            {
                                ViewBag.DeleteFlg = true;
                            }
                        
                        for (int i = 0; i < dsPassList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EquipmentDetailModels objPass = new EquipmentDetailModels
                                {
                                    id_pass = Convert.ToInt32(dsPassList.Tables[0].Rows[i]["id_pass"].ToString()),
                                    id_equipment = Convert.ToInt32(dsPassList.Tables[0].Rows[i]["id_equipment"].ToString()),
                                    PassType = objGlobaldata.getPassTypeById(dsPassList.Tables[0].Rows[i]["PassType"].ToString()),
                                    Upload = dsPassList.Tables[0].Rows[i]["Upload"].ToString(),

                                };

                                DateTime dtDate;
                                if (DateTime.TryParse(dsPassList.Tables[0].Rows[i]["ExpDate"].ToString(), out dtDate))
                                {
                                    objPass.ExpDate = dtDate;
                                }
                                lstpass.lstCat.Add(objPass);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in VehiclePassList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EquipmentDetailsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in VehiclePassList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(lstpass.lstCat.ToList().ToPagedList(page ?? 1, 10000));
        }

         
        public JsonResult VehiclePassDelete(FormCollection form)
        {

            try
            {
                
                    if (objGlobaldata.GetCurrentUserSession().DeleteFlag.ToLower() == "y")
                    {
                        if (form["id_pass"] != null && form["id_pass"] != "")
                        {
                            EquipmentDetailModels obj = new EquipmentDetailModels();
                            string sid_pass = form["id_pass"];

                            if (obj.FunDeleteVehiclePass(sid_pass))
                            {
                                TempData["Successdata"] = "Vehicle Pass deleted successfully";

                            }
                            else
                            {
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return Json("Success");
                            }
                        }
                        else
                        {
                            TempData["alertdata"] = "Pass Id cannot be Null or empty";
                            return Json("Failed");
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Access Denied";
                        return Json("Failed");
                    }
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in VehiclePassDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

         
        [AllowAnonymous]
        public ActionResult VehiclePassEdit()
        {

            EquipmentDetailModels objPass = new EquipmentDetailModels();

            try
            {
                ViewBag.PassType = objGlobaldata.getVehiclePassTypeList();
                if (Request.QueryString["id_pass"] != null && Request.QueryString["id_pass"] != "")
                {
                    string sid_pass = Request.QueryString["id_pass"];
                    string sSqlstmt = "select id_pass,id_equipment,PassType,Upload,ExpDate from t_truck_pass where id_pass='" + sid_pass + "'";

                    DataSet dsPassList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPassList.Tables.Count > 0 && dsPassList.Tables[0].Rows.Count > 0)
                    {

                        objPass = new EquipmentDetailModels
                        {
                            id_pass = Convert.ToInt32(dsPassList.Tables[0].Rows[0]["id_pass"].ToString()),
                            id_equipment = Convert.ToInt32(dsPassList.Tables[0].Rows[0]["id_equipment"].ToString()),
                            PassType = objGlobaldata.getPassTypeById(dsPassList.Tables[0].Rows[0]["PassType"].ToString()),
                            Upload = dsPassList.Tables[0].Rows[0]["Upload"].ToString(),
                        };
                        DateTime dtDate;
                        if (DateTime.TryParse(dsPassList.Tables[0].Rows[0]["ExpDate"].ToString(), out dtDate))
                        {
                            objPass.ExpDate = dtDate;
                        }
                    }
                    else
                    {

                        TempData["alertdata"] = "PassID cannot be Null or empty";
                        return RedirectToAction("EquipmentDetailsList");
                    }
                }
                else
                {

                    TempData["alertdata"] = "PassID cannot be Null or empty";
                    return RedirectToAction("EquipmentDetailsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in VehiclePassEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EquipmentDetailsList");
            }
            return View(objPass);
        }

         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VehiclePassEdit(EquipmentDetailModels objPass, FormCollection form, HttpPostedFileBase Upload)
        {
            try
            {
                objPass.PassType = form["PassType"];
                DateTime dateValue;

                if (DateTime.TryParse(form["ExpDate"], out dateValue) == true)
                {
                    objPass.ExpDate = dateValue;
                }
                if (Upload != null && Upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(Upload.FileName));
                        string sFilename = "VehiclePass" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Upload.SaveAs(sFilepath + "/" + sFilename);
                        objPass.Upload = "~/DataUpload/MgmtDocs/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in VehiclePassEditUpload: " + ex.ToString());
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objPass.FunUpdatePass(objPass))
                {
                    TempData["Successdata"] = "Vehicle Pass details Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in VehiclePassEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EquipmentDetailsList");
            }
            return RedirectToAction("EquipmentDetailsList");
        }

    }
}