using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace ISOStd.Controllers
{
    public class EquipmentController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
         
        public EquipmentController()
        {
            ViewBag.Menutype = "Equipments";
            ViewBag.SubMenutype = "Equipment";
        }

        //
        // GET: /Equipment/
          
        public ActionResult Index()
        {
            return View();
        }


        // GET: /Equipment/AddEquipment
          
        [AllowAnonymous]
        public ActionResult AddEquipment()
        {
            return InitilizeAddEquipment();
        }

        // GET: /Equipment/AddEquipment
          
        private ActionResult InitilizeAddEquipment()
        {
            EquipmentModels objEquip = new EquipmentModels();
            try
            {
                ViewBag.SubMenutype = "Equipment";
                objEquip.branch = objGlobaldata.GetCurrentUserSession().division;
                objEquip.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objEquip.location = objGlobaldata.GetCurrentUserSession().Work_Location;
                
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objEquip.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objEquip.branch);
                ViewBag.Source_of_calibration = objEquip.GetMultiCalibrationSourceList();
                ViewBag.Equipment_status = objEquip.GetMultiCalibrationStatusList();
                ViewBag.Freq_of_calibration = objEquip.GetMultiCalibrationFrequencyList();
                ViewBag.RespPerson = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Type = objGlobaldata.GetDropdownList("Equipment Type");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEquipment: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objEquip);
        }

         
        [AllowAnonymous]
        public JsonResult EquimentDocDelete(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "Equipment";

                if (form["Equipment_Id"] != null && form["Equipment_Id"] != "")
                        {

                            EquipmentModels Doc = new EquipmentModels();
                            string sEquipment_Id = form["Equipment_Id"];


                            if (Doc.FunDeleteEquipmentDoc(sEquipment_Id))
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
                            TempData["alertdata"] = "Equipment Id cannot be Null or empty";
                            return Json("Failed");
                        }
                   
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquimentDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

          
        [AllowAnonymous]
        public JsonResult CalibrationDocDelete(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "Calibration";
                if (form["calibration_id"] != null && form["calibration_id"] != "")
                        {

                            EquipmentModels Doc = new EquipmentModels();
                            string scalibration_id = form["calibration_id"];


                            if (Doc.FunDeleteCalibrationDoc(scalibration_id))
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
                            TempData["alertdata"] = "Calibration Id cannot be Null or empty";
                            return Json("Failed");
                        }
                  
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CalibrationDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

          
        [HttpPost]
        public JsonResult doesEquipmentExist(string Equipment_serial_no)
        {
            EquipmentModels objEquipment = new EquipmentModels();
            var Equip = objEquipment.checkSerialNoExists(Equipment_serial_no);

            return Json(Equip);
        }
 
        // POST: /Equipment/AddEquipment
          
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEquipment(EquipmentModels objEquipment, FormCollection form, IEnumerable <HttpPostedFileBase> file)
        {
            ViewBag.SubMenutype = "Equipment";
            try
            {
                objEquipment.branch = form["branch"];
                objEquipment.location = form["location"];
                objEquipment.Department = form["Department"];
                objEquipment.RespPerson = form["RespPerson"];


                HttpPostedFileBase files = Request.Files[0];
                if (objEquipment != null)
                {
                    objEquipment.Source_of_calibration = form["Source_of_calibration"];
                    objEquipment.Equipment_status = form["Equipment_status"];

                    DateTime dateValue;

                    if (DateTime.TryParse(form["Commissioning_Date"], out dateValue) == true)
                    {
                        objEquipment.Commissioning_Date = dateValue;
                    }
                    if (file != null && files.ContentLength > 0)
                    {
                        foreach (var document in file)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Equipment"), Path.GetFileName(document.FileName));
                                string sFilename = objEquipment.Equipment_Name + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                                string sFilepath = Path.GetDirectoryName(spath);

                                document.SaveAs(sFilepath + "/" + sFilename);
                                objEquipment.DocUploadPath = objEquipment.DocUploadPath + "," + "~/DataUpload/MgmtDocs/Equipment/" + sFilename;
                                ViewBag.Message = "File uploaded successfully";
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddEquipment-upload: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                        objEquipment.DocUploadPath = objEquipment.DocUploadPath.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (objEquipment.FunAddEquipment(objEquipment))
                    {
                        TempData["Successdata"] = "Added Equipment details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEquipment: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EquipmentList");
        }
           
        [AllowAnonymous]
        public ActionResult EquipmentList(string SearchText, string Equipment_status , int? page,string CalibrationSource,string chkAll, string branch_name)
        {
            ViewBag.SubMenutype = "Equipment";
            EquipmentModelsList objEquipmentModelsList = new EquipmentModelsList();
            objEquipmentModelsList.EquipmentMList = new List<EquipmentModels>();

            EquipmentModels objEquip = new EquipmentModels();

            ViewBag.CalibrationSource= objEquip.GetMultiCalibrationSourceList();
          
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                //string sSqlstmt = "select Equipment_Id, Equipment_serial_no, Equipment_Name, Equipment_Application, Source_of_calibration, Freq_of_calibration, "
                //    + "Commissioning_Date, Manufacturer, Equipment_status, model_no,Department,RespPerson,equp_type,location from t_equipment where Active=1";

                string sSqlstmt = "select a.Equipment_Id, Equipment_serial_no, ifnull(max(Next_Maint_Date), '0001-01-01') as Next_Maint_Date,Equipment_Name," +
                    "Equipment_Application,Source_of_calibration,Freq_of_calibration,Commissioning_Date, Manufacturer,Equipment_status, model_no,Department," +
                    "RespPerson,equp_type,location,branch,Equipment_location from t_equipment a left outer join t_equpiment_preventive_maint b on a.Equipment_Id = b.Equipment_Id where a.Active = 1" +
                    " group by a.Equipment_Id, Equipment_serial_no,Equipment_Name,Equipment_Application, Equipment_status, model_no, Department,RespPerson,equp_type,location";

                string sSearchtext = "";

                if (chkAll != "All" && chkAll == null)
                {                    
                    if (SearchText != null && SearchText != "")
                    {
                        ViewBag.SearchText = SearchText;
                        sSearchtext = " (Equipment_Name ='" + SearchText + "' or Equipment_Name like '" + SearchText + "%')";
                    }

                    if (sSearchtext != "")
                    {
                        sSearchtext = " and " + sSearchtext;
                    }
                    if (Equipment_status != null && Equipment_status != "Select")
                    {
                        ViewBag.Equipment_statusText = Equipment_status;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and (Equipment_status ='" + Equipment_status + "')";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and (Equipment_status ='" + Equipment_status + "')";
                        }
                    }

                    if (CalibrationSource != null && CalibrationSource != "Select")
                    {
                        ViewBag.CalibrationSourceVal = CalibrationSource;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and (Source_of_calibration ='" + CalibrationSource + "')";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and (Source_of_calibration ='" + CalibrationSource + "')";
                        }
                    }

                }
                else
                {
                    ViewBag.chkAll = true;
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Equipment_Name asc";

                //ViewBag.Equipment_status = new string[] { "Active", "In Active" };
                ViewBag.Equipment_status = objEquip.GetMultiCalibrationStatusList();
              

                DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; i < dsEquipmentList.Tables[0].Rows.Count; i++)
                {                      
                    try
                    {
                        EquipmentModels objEquipmentModels = new EquipmentModels
                        {
                            Equipment_Id = (dsEquipmentList.Tables[0].Rows[i]["Equipment_Id"].ToString()),
                            Equipment_serial_no = dsEquipmentList.Tables[0].Rows[i]["Equipment_serial_no"].ToString(),
                            Equipment_Name = (dsEquipmentList.Tables[0].Rows[i]["Equipment_Name"].ToString()),
                            Equipment_Application = dsEquipmentList.Tables[0].Rows[i]["Equipment_Application"].ToString(),
                            Source_of_calibration =objEquip.GetCalibrationSourceNameById(dsEquipmentList.Tables[0].Rows[i]["Source_of_calibration"].ToString()),
                            Freq_of_calibration =objEquip.GetCalibrationFrequencyNameById(dsEquipmentList.Tables[0].Rows[i]["Freq_of_calibration"].ToString()),
                            Commissioning_Date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[i]["Commissioning_Date"].ToString()),
                            Manufacturer = (dsEquipmentList.Tables[0].Rows[i]["Manufacturer"].ToString()),
                            Equipment_status =objEquip.GetCalibrationStatusNameById(dsEquipmentList.Tables[0].Rows[i]["Equipment_status"].ToString()),
                            Model_No = (dsEquipmentList.Tables[0].Rows[0]["model_no"].ToString()),
                            RespPerson = objGlobaldata.GetEmpHrNameById(dsEquipmentList.Tables[0].Rows[i]["RespPerson"].ToString()),
                            equp_type = objGlobaldata.GetDropdownitemById(dsEquipmentList.Tables[0].Rows[i]["equp_type"].ToString()),
                            Equipment_location = (dsEquipmentList.Tables[0].Rows[i]["Equipment_location"].ToString()),
                            Next_Maint_Date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[i]["Next_Maint_Date"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsEquipmentList.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsEquipmentList.Tables[0].Rows[i]["Department"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsEquipmentList.Tables[0].Rows[i]["location"].ToString()),
                        };                       
                        objEquipmentModelsList.EquipmentMList.Add(objEquipmentModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in EquipmentList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objEquipmentModelsList.EquipmentMList.ToList());
        }

        [AllowAnonymous]
        public JsonResult EquipmentListSearch(string SearchText, string Equipment_status, int? page, string CalibrationSource, string chkAll, string branch_name)
        {
            ViewBag.SubMenutype = "Equipment";
            EquipmentModelsList objEquipmentModelsList = new EquipmentModelsList();
            objEquipmentModelsList.EquipmentMList = new List<EquipmentModels>();

            EquipmentModels objEquip = new EquipmentModels();

            ViewBag.CalibrationSource = objEquip.GetMultiCalibrationSourceList();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                //string sSqlstmt = "select Equipment_Id, Equipment_serial_no, Equipment_Name, Equipment_Application, Source_of_calibration, Freq_of_calibration, "
                //    + "Commissioning_Date, Manufacturer, Equipment_status, model_no,Department,RespPerson,equp_type,location from t_equipment where Active=1";
                string sSqlstmt = "select a.Equipment_Id, Equipment_serial_no, ifnull(max(Next_Maint_Date), '0001-01-01') as Next_Maint_Date,Equipment_Name," +
                    "Equipment_Application,Source_of_calibration,Freq_of_calibration,Commissioning_Date, Manufacturer,Equipment_status, model_no,Department," +
                    "RespPerson,equp_type,location,branch,Equipment_location from t_equipment a left outer join t_equpiment_preventive_maint b on a.Equipment_Id = b.Equipment_Id where a.Active = 1" +
                    " group by a.Equipment_Id, Equipment_serial_no,Equipment_Name,Equipment_Application, Equipment_status, model_no, Department,RespPerson,equp_type,location";

                string sSearchtext = "";

                if (chkAll != "All" && chkAll == null)
                {
                    if (SearchText != null && SearchText != "")
                    {
                        ViewBag.SearchText = SearchText;
                        sSearchtext = " (Equipment_Name ='" + SearchText + "' or Equipment_Name like '" + SearchText + "%')";
                    }

                    if (sSearchtext != "")
                    {
                        sSearchtext = " and " + sSearchtext;
                    }
                    if (Equipment_status != null && Equipment_status != "Select")
                    {
                        ViewBag.Equipment_statusText = Equipment_status;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and (Equipment_status ='" + Equipment_status + "')";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and (Equipment_status ='" + Equipment_status + "')";
                        }
                    }

                    if (CalibrationSource != null && CalibrationSource != "Select")
                    {
                        ViewBag.CalibrationSourceVal = CalibrationSource;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and (Source_of_calibration ='" + CalibrationSource + "')";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and (Source_of_calibration ='" + CalibrationSource + "')";
                        }
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Equipment_Name asc";

                //ViewBag.Equipment_status = new string[] { "Active", "In Active" };
                ViewBag.Equipment_status = objEquip.GetMultiCalibrationStatusList();


                DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; i < dsEquipmentList.Tables[0].Rows.Count; i++)
                {   
                    try
                    {
                        EquipmentModels objEquipmentModels = new EquipmentModels
                        {
                            Equipment_Id = (dsEquipmentList.Tables[0].Rows[i]["Equipment_Id"].ToString()),
                            Equipment_serial_no = dsEquipmentList.Tables[0].Rows[i]["Equipment_serial_no"].ToString(),
                            Equipment_Name = (dsEquipmentList.Tables[0].Rows[i]["Equipment_Name"].ToString()),
                            Equipment_Application = dsEquipmentList.Tables[0].Rows[i]["Equipment_Application"].ToString(),
                            Source_of_calibration = objEquip.GetCalibrationSourceNameById(dsEquipmentList.Tables[0].Rows[i]["Source_of_calibration"].ToString()),
                            Freq_of_calibration = objEquip.GetCalibrationFrequencyNameById(dsEquipmentList.Tables[0].Rows[i]["Freq_of_calibration"].ToString()),
                            Commissioning_Date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[i]["Commissioning_Date"].ToString()),
                            Manufacturer = (dsEquipmentList.Tables[0].Rows[i]["Manufacturer"].ToString()),
                            Equipment_status = objEquip.GetCalibrationStatusNameById(dsEquipmentList.Tables[0].Rows[i]["Equipment_status"].ToString()),
                            Model_No = (dsEquipmentList.Tables[0].Rows[0]["model_no"].ToString()),
                            RespPerson = objGlobaldata.GetEmpHrNameById(dsEquipmentList.Tables[0].Rows[i]["RespPerson"].ToString()),
                            equp_type = objGlobaldata.GetDropdownitemById(dsEquipmentList.Tables[0].Rows[i]["equp_type"].ToString()),
                            Equipment_location = (dsEquipmentList.Tables[0].Rows[i]["Equipment_location"].ToString()),
                            Next_Maint_Date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[i]["Next_Maint_Date"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsEquipmentList.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsEquipmentList.Tables[0].Rows[i]["Department"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsEquipmentList.Tables[0].Rows[i]["location"].ToString()),

                        };
                        objEquipmentModelsList.EquipmentMList.Add(objEquipmentModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in EquipmentListSearch: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult EquipmentDetails()
        {
            ViewBag.SubMenutype = "Equipment";

            EquipmentModels objEquipmentModels = new EquipmentModels();
            try
            {
                if (Request.QueryString["Equipment_Id"] != null && Request.QueryString["Equipment_Id"] != "")
                {
                    string sEquipment_Id = Request.QueryString["Equipment_Id"];

                   
                    string sSqlstmt = "select Equipment_Id, Equipment_serial_no, Equipment_Name, Equipment_Application, Source_of_calibration, Freq_of_calibration,equp_type, "
                        + "Commissioning_Date, Manufacturer, Equipment_status, model_no,Department,Logged_date,DocUploadPath,RespPerson,location,branch,Equipment_location from t_equipment where Equipment_Id='" + sEquipment_Id + "'";
                   // string sSqlstmt = "select a.Equipment_Id, Equipment_serial_no, ifnull(max(Next_Maint_Date), '0001-01-01') as Next_Maint_Date,Equipment_Name," +
                    //"Equipment_Application,Source_of_calibration,Freq_of_calibration,Commissioning_Date, Manufacturer,Equipment_status, model_no,Department," +
                    //"RespPerson,equp_type,location,Logged_date,DocUploadPath from t_equipment a left outer join t_equpiment_preventive_maint b on a.Equipment_Id = b.Equipment_Id where a.Active = 1" +
                    //" group by a.Equipment_Id, Equipment_serial_no,Equipment_Name,Equipment_Application, Equipment_status, model_no, Department,RespPerson,equp_type,location";

                    DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsEquipmentList.Tables.Count > 0 && dsEquipmentList.Tables[0].Rows.Count > 0)
                    {
                        objEquipmentModels = new EquipmentModels
                        {
                            Equipment_Id = (dsEquipmentList.Tables[0].Rows[0]["Equipment_Id"].ToString()),
                            Equipment_serial_no = dsEquipmentList.Tables[0].Rows[0]["Equipment_serial_no"].ToString(),
                            Equipment_Name = (dsEquipmentList.Tables[0].Rows[0]["Equipment_Name"].ToString()),
                            Equipment_Application = dsEquipmentList.Tables[0].Rows[0]["Equipment_Application"].ToString(),
                            Source_of_calibration =objEquipmentModels.GetCalibrationSourceNameById(dsEquipmentList.Tables[0].Rows[0]["Source_of_calibration"].ToString()),
                            Freq_of_calibration = objEquipmentModels.GetCalibrationFrequencyNameById(dsEquipmentList.Tables[0].Rows[0]["Freq_of_calibration"].ToString()),
                            Commissioning_Date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[0]["Commissioning_Date"].ToString()),
                            Manufacturer = (dsEquipmentList.Tables[0].Rows[0]["Manufacturer"].ToString()),
                            Equipment_status = objEquipmentModels.GetCalibrationStatusNameById(dsEquipmentList.Tables[0].Rows[0]["Equipment_status"].ToString()),
                            Model_No = (dsEquipmentList.Tables[0].Rows[0]["model_no"].ToString()),
                            DocUploadPath = (dsEquipmentList.Tables[0].Rows[0]["DocUploadPath"].ToString()),
                            RespPerson=objGlobaldata.GetEmpHrNameById(dsEquipmentList.Tables[0].Rows[0]["RespPerson"].ToString()),
                            equp_type =objGlobaldata.GetDropdownitemById(dsEquipmentList.Tables[0].Rows[0]["equp_type"].ToString()),
                            Equipment_location = (dsEquipmentList.Tables[0].Rows[0]["Equipment_location"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsEquipmentList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsEquipmentList.Tables[0].Rows[0]["Department"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsEquipmentList.Tables[0].Rows[0]["location"].ToString()),
                            //Next_Maint_Date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[0]["Next_Maint_Date"].ToString()),
                        };
                        DateTime dateValue;
                        if (DateTime.TryParse(dsEquipmentList.Tables[0].Rows[0]["Logged_date"].ToString(), out dateValue))
                        {
                            objEquipmentModels.Logged_date = dateValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EquipmentList");
                    }

                    string sSqlstmt1 = "select Maintenance_Id, tmain.Equipment_Id, Maintenance_Date, Maintenance_Details, Maintenance_RectificationWork,"
                       + " Down_Time_From, Down_Time_To, Spare_Used, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name,Amt_Spent from "
                       + " t_equipment_maintenance as tmain, t_equipment as tequp where tmain.equipment_id=tequp.equipment_id and tmain.equipment_id='" + sEquipment_Id + "'";
                    ViewBag.Maintenance = objGlobaldata.Getdetails(sSqlstmt1);


                    string sSqlstmt2 = "select Preventive_Id, tmain.Equipment_Id, Maintenance_Date, Maintenance_Details, Next_Maint_Date,Amt_Spent,"
                     + " done_by, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name from "
                     + " t_equpiment_preventive_maint as tmain, t_equipment as tequp where tmain.equipment_id=tequp.equipment_id and tmain.equipment_id='" + sEquipment_Id + "'";
                    ViewBag.PMaintenance = objGlobaldata.Getdetails(sSqlstmt2);
                }
                else
                {
                    TempData["alertdata"] = "Equipment Id cannot be Null or empty";
                    return RedirectToAction("EquipmentList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEquipmentModels);
        }
                  
        [AllowAnonymous]
        public ActionResult EquipmentInfo(int id)
        {            
            EquipmentModels objEquipmentModels = new EquipmentModels();
            try
            {

                string sSqlstmt = "select Equipment_Id, Equipment_serial_no, Equipment_Name, Equipment_Application, Source_of_calibration, Freq_of_calibration,equp_type, "
                    + "Commissioning_Date, Manufacturer, Equipment_status, model_no,Department,Logged_date,RespPerson,location,branch,Equipment_location from t_equipment where Equipment_Id='" + id + "'";
                //string sSqlstmt = "select a.Equipment_Id, Equipment_serial_no, ifnull(max(Next_Maint_Date), '0001-01-01') as Next_Maint_Date,Equipment_Name," +
                //"Equipment_Application,Source_of_calibration,Freq_of_calibration,Commissioning_Date, Manufacturer,Equipment_status, model_no,Department," +
                //"RespPerson,equp_type,location from t_equipment a left outer join t_equpiment_preventive_maint b on a.Equipment_Id = b.Equipment_Id where a.Active = 1" +
                //" group by a.Equipment_Id, Equipment_serial_no,Equipment_Name,Equipment_Application, Equipment_status, model_no, Department,RespPerson,equp_type,location";

                DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsEquipmentList.Tables.Count > 0 && dsEquipmentList.Tables[0].Rows.Count > 0)
                    {
                        objEquipmentModels = new EquipmentModels
                        {
                            Equipment_Id = (dsEquipmentList.Tables[0].Rows[0]["Equipment_Id"].ToString()),
                            Equipment_serial_no = dsEquipmentList.Tables[0].Rows[0]["Equipment_serial_no"].ToString(),
                            Equipment_Name = (dsEquipmentList.Tables[0].Rows[0]["Equipment_Name"].ToString()),
                            Equipment_Application = dsEquipmentList.Tables[0].Rows[0]["Equipment_Application"].ToString(),
                            Source_of_calibration = objEquipmentModels.GetCalibrationSourceNameById(dsEquipmentList.Tables[0].Rows[0]["Source_of_calibration"].ToString()),
                            Freq_of_calibration = objEquipmentModels.GetCalibrationFrequencyNameById(dsEquipmentList.Tables[0].Rows[0]["Freq_of_calibration"].ToString()),
                            Commissioning_Date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[0]["Commissioning_Date"].ToString()),
                            Manufacturer = (dsEquipmentList.Tables[0].Rows[0]["Manufacturer"].ToString()),
                            Equipment_status = objEquipmentModels.GetCalibrationStatusNameById(dsEquipmentList.Tables[0].Rows[0]["Equipment_status"].ToString()),
                            Model_No = (dsEquipmentList.Tables[0].Rows[0]["model_no"].ToString()),
                            RespPerson=objGlobaldata.GetEmpHrNameById(dsEquipmentList.Tables[0].Rows[0]["RespPerson"].ToString()),
                            Equipment_location = (dsEquipmentList.Tables[0].Rows[0]["Equipment_location"].ToString()),
                            equp_type = objGlobaldata.GetDropdownitemById(dsEquipmentList.Tables[0].Rows[0]["equp_type"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsEquipmentList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsEquipmentList.Tables[0].Rows[0]["Department"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsEquipmentList.Tables[0].Rows[0]["location"].ToString()),
                            // Next_Maint_Date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[0]["Next_Maint_Date"].ToString()),
                        };
                        DateTime dateValue;
                        if (DateTime.TryParse(dsEquipmentList.Tables[0].Rows[0]["Logged_date"].ToString(), out dateValue))
                        {
                            objEquipmentModels.Logged_date = dateValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EquipmentList");
                    }

                    string sSqlstmt1 = "select Maintenance_Id, tmain.Equipment_Id, Maintenance_Date, Maintenance_Details, Maintenance_RectificationWork,"
                       + " Down_Time_From, Down_Time_To, Spare_Used, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name,Amt_Spent from "
                       + " t_equipment_maintenance as tmain, t_equipment as tequp where tmain.equipment_id=tequp.equipment_id and tmain.equipment_id='" + id + "'";
                    ViewBag.Maintenance = objGlobaldata.Getdetails(sSqlstmt1);

                   string sSqlstmt2 = "select Preventive_Id, tmain.Equipment_Id, Maintenance_Date, Maintenance_Details, Next_Maint_Date,Amt_Spent,"
                    + " done_by, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name from "
                    + " t_equpiment_preventive_maint as tmain, t_equipment as tequp where tmain.equipment_id=tequp.equipment_id and tmain.equipment_id='" + id + "'";
                    ViewBag.PMaintenance = objGlobaldata.Getdetails(sSqlstmt2);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEquipmentModels);
        }
          
        [AllowAnonymous]
        public ActionResult EquipmentEdit()
        {
            ViewBag.SubMenutype = "Equipment";
            EquipmentModels objEquipmentModels = new EquipmentModels();
            ViewBag.RespPerson = objGlobaldata.GetHrEmployeeListbox();

            try
            {

                if (Request.QueryString["Equipment_Id"] != null && Request.QueryString["Equipment_Id"] != "")
                {
                    string sEquipment_Id = Request.QueryString["Equipment_Id"];

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select Equipment_Id, Equipment_serial_no, Equipment_Name, Equipment_Application, Source_of_calibration, Freq_of_calibration, "
                        + "Commissioning_Date, Manufacturer, Equipment_status, model_no,Department,DocUploadPath,RespPerson,equp_type,location,branch,Equipment_location from t_equipment where Equipment_Id='" + sEquipment_Id + "'";

                    DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEquipmentList.Tables.Count > 0 && dsEquipmentList.Tables[0].Rows.Count > 0)
                    {
                        objEquipmentModels = new EquipmentModels
                        {
                            Equipment_Id = (dsEquipmentList.Tables[0].Rows[0]["Equipment_Id"].ToString()),
                            Equipment_serial_no = dsEquipmentList.Tables[0].Rows[0]["Equipment_serial_no"].ToString(),
                            Equipment_Name = (dsEquipmentList.Tables[0].Rows[0]["Equipment_Name"].ToString()),
                            Equipment_Application = dsEquipmentList.Tables[0].Rows[0]["Equipment_Application"].ToString(),
                            Source_of_calibration =objEquipmentModels.GetCalibrationSourceNameById(dsEquipmentList.Tables[0].Rows[0]["Source_of_calibration"].ToString()),
                            Freq_of_calibration = objEquipmentModels.GetCalibrationFrequencyNameById(dsEquipmentList.Tables[0].Rows[0]["Freq_of_calibration"].ToString()),
                            Commissioning_Date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[0]["Commissioning_Date"].ToString()),
                            Manufacturer = (dsEquipmentList.Tables[0].Rows[0]["Manufacturer"].ToString()),
                            Equipment_status =objEquipmentModels.GetCalibrationStatusNameById(dsEquipmentList.Tables[0].Rows[0]["Equipment_status"].ToString()),
                            Model_No = (dsEquipmentList.Tables[0].Rows[0]["model_no"].ToString()),
                            DocUploadPath = (dsEquipmentList.Tables[0].Rows[0]["DocUploadPath"].ToString()),
                            RespPerson=(dsEquipmentList.Tables[0].Rows[0]["RespPerson"].ToString()),
                            equp_type = (dsEquipmentList.Tables[0].Rows[0]["equp_type"].ToString()),
                            Equipment_location = (dsEquipmentList.Tables[0].Rows[0]["Equipment_location"].ToString()),
                            branch = (dsEquipmentList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsEquipmentList.Tables[0].Rows[0]["Department"].ToString()),
                            location = (dsEquipmentList.Tables[0].Rows[0]["location"].ToString()),
                        };

                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsEquipmentList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsEquipmentList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Source_of_calibration = objEquipmentModels.GetMultiCalibrationSourceList();
                        ViewBag.Equipment_status = objEquipmentModels.GetMultiCalibrationStatusList();
                        ViewBag.Freq_of_calibration = objEquipmentModels.GetMultiCalibrationFrequencyList();
                        ViewBag.Type = objGlobaldata.GetDropdownList("Equipment Type");
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EquipmentList");
                    }
                }               
                else
                {
                    TempData["alertdata"] = "Equipment Id cannot be Null or empty";
                    return RedirectToAction("EquipmentList");
                }
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EquipmentList");
            }            
            return View(objEquipmentModels);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EquipmentEdit(EquipmentModels objEquipment, FormCollection form, IEnumerable<HttpPostedFileBase> file)
        {
            ViewBag.SubMenutype = "Equipment";

            try
            {
                objEquipment.branch = form["branch"];
                objEquipment.location = form["location"];
                objEquipment.Department = form["Department"];
                objEquipment.RespPerson = form["RespPerson"];

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (objEquipment != null)
                {
                    objEquipment.Source_of_calibration = form["Source_of_calibration"];
                    objEquipment.Equipment_status = form["Equipment_status"];

                    DateTime dateValue;

                    if (DateTime.TryParse(form["Commissioning_Date"], out dateValue) == true)
                    {
                        objEquipment.Commissioning_Date = dateValue;
                    }

                    objEquipment.Equipment_Id = form["Equipment_Id"];
                    if (file != null && files.ContentLength > 0)
                    {
                        foreach (var document in file)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Equipment"), Path.GetFileName(document.FileName));
                                string sFilename = objEquipment.Equipment_Name + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                                string sFilepath = Path.GetDirectoryName(spath);

                                document.SaveAs(sFilepath + "/" + sFilename);
                                objEquipment.DocUploadPath = objEquipment.DocUploadPath + "," + "~/DataUpload/MgmtDocs/Equipment/" + sFilename;
                                ViewBag.Message = "File uploaded successfully";
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddEquipment-upload: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                        objEquipment.DocUploadPath = objEquipment.DocUploadPath.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                    {
                        objEquipment.DocUploadPath = objEquipment.DocUploadPath + "," + form["QCDocsVal"];
                        objEquipment.DocUploadPath = objEquipment.DocUploadPath.Trim(',');
                    }
                    else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                    {
                        objEquipment.DocUploadPath = null;
                    }
                    else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                    {
                        objEquipment.DocUploadPath = null;
                    }
                   
                    if (objEquipment.FunUpdateEquipment(objEquipment))
                    {
                        TempData["Successdata"] = "Equiment details updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EquipmentEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EquipmentList");
        }
         
        [AllowAnonymous]
        public ActionResult AddCalibration()
        {            
            return InitilizeAddCalibration();
        }

        // GET: /Equipment/AddCalibration
         
        private ActionResult InitilizeAddCalibration()
        {
            try
            {
                ViewBag.SubMenutype = "Calibration";
                EquipmentModels objEquipment = new EquipmentModels();
                ViewBag.Equipmentnames = objEquipment.GetCalibatedEquipmentListbox();
                //ViewBag.Equipmentnames = objEquipment.GetEquipmentListbox();
                ViewBag.calibration_status = objGlobaldata.GetConstantValue("calibration_status");
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCalibration: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
         
        [HttpPost]
        public JsonResult doesFileTypeValidation(string calibration_report_ref)
        {
            var user = true;
            if (calibration_report_ref != null)
            {
                EmployeeMasterModels objEmployeeModel = new EmployeeMasterModels();
                user = objEmployeeModel.checkCompEmpIdExists(calibration_report_ref);
            }

            return Json(user);
        }

        // POST: /Equipment/AddCalibration
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCalibration(CalibrationModels objCalibrationModels, FormCollection form, HttpPostedFileBase fileCert, HttpPostedFileBase calibration_report_ref)
        {
            ViewBag.SubMenutype = "Calibration";
            try
            {
                //if (objCalibrationModels != null)
                {
                    objCalibrationModels.calibration_status = form["calibration_status"];
                    objCalibrationModels.Equipment_Id = form["Equipment_Id"];
                    objCalibrationModels.NotificationPeriod = form["NotificationPeriod"];
                    objCalibrationModels.NotificationValue = form["NotificationValue"];
                    objCalibrationModels.Person_Responsible = form["Person_Responsible"];

                    int Notificationval = 1;

                    if (objCalibrationModels.NotificationPeriod == "None")
                    {
                        Notificationval = 0;
                        objCalibrationModels.NotificationValue = "0";
                    }
                    else if (objCalibrationModels.NotificationPeriod == "Weeks" && int.TryParse(objCalibrationModels.NotificationValue, out Notificationval))
                    {
                        Notificationval = 7 * Notificationval;
                    }
                    else if (objCalibrationModels.NotificationPeriod == "Months" && int.TryParse(objCalibrationModels.NotificationValue, out Notificationval))
                    {
                        Notificationval = 30 * Notificationval;
                    }
                    objCalibrationModels.NotificationDays = Notificationval;

                    DateTime dateValue;

                    if (DateTime.TryParse(form["due_date"], out dateValue) == true)
                    {
                        objCalibrationModels.due_date = dateValue;
                    }

                    if (fileCert != null && fileCert.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Equipment"), Path.GetFileName(fileCert.FileName));
                            string sFilename = objCalibrationModels.Equipment_Id + "Cert_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            fileCert.SaveAs(sFilepath + "/" + sFilename);
                            objCalibrationModels.calibration_certificate = "~/DataUpload/MgmtDocs/Equipment/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddCalibration: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (calibration_report_ref != null && calibration_report_ref.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Equipment"), Path.GetFileName(calibration_report_ref.FileName));
                            string sFilename = objCalibrationModels.Equipment_Id + "Report_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            calibration_report_ref.SaveAs(sFilepath + "/" + sFilename);
                            objCalibrationModels.calibration_report_ref = "~/DataUpload/MgmtDocs/Equipment/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddCalibration: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (objCalibrationModels.FunAddCalibration(objCalibrationModels))
                    {
                        TempData["Successdata"] = "Added Calibration details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCalibration: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CalibrationList");
        }


        // GET: /Equipment/CalibrationList
         
        [AllowAnonymous]
        public ActionResult CalibrationList(string SearchText, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Calibration";
            CalibrationModelsList objCalibrationModelsList = new CalibrationModelsList();
            objCalibrationModelsList.CalibrationMList = new List<CalibrationModels>();
            EquipmentModels objModel = new EquipmentModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select calibration_id, tcal.Equipment_id, calibration_by, method_of_calibration, accuracy, calibration_status,"
                    + " calibration_report_ref, calibration_certificate, due_date, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name, "
                    + "CalibrationDate,Ref_no,certificate_no from  t_calibration as tcal, t_equipment as tequp"
                    + " where tcal.equipment_id=tequp.equipment_id and calibration_id in (select max(calibration_id) from t_calibration where tcal.Active=1 group by Equipment_id)";
                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = " and (tequp.Equipment_Name ='" + SearchText + "' or tequp.Equipment_Name like '" + SearchText + "%')";
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and tequp.branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and tequp.branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Equipment_Name asc";
                
                DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEquipmentList.Tables.Count > 0)
                {

                    for (int i = 0; i < dsEquipmentList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            CalibrationModels objCalibrationModels = new CalibrationModels
                            {
                                calibration_id = (dsEquipmentList.Tables[0].Rows[i]["calibration_id"].ToString()),
                                Equipment_Id = (dsEquipmentList.Tables[0].Rows[i]["Equipment_Name"].ToString()),
                                calibration_by = dsEquipmentList.Tables[0].Rows[i]["calibration_by"].ToString(),
                                method_of_calibration = (dsEquipmentList.Tables[0].Rows[i]["method_of_calibration"].ToString()),
                                accuracy = dsEquipmentList.Tables[0].Rows[i]["accuracy"].ToString(),
                                calibration_status = dsEquipmentList.Tables[0].Rows[i]["calibration_status"].ToString(),
                                calibration_report_ref = (dsEquipmentList.Tables[0].Rows[i]["calibration_report_ref"].ToString()),
                                due_date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[i]["due_date"].ToString()),
                                calibration_certificate = (dsEquipmentList.Tables[0].Rows[i]["calibration_certificate"].ToString()),
                                Remarks = (dsEquipmentList.Tables[0].Rows[i]["Remarks"].ToString()),
                                Ref_no = (dsEquipmentList.Tables[0].Rows[i]["Ref_no"].ToString()),
                                certificate_no = (dsEquipmentList.Tables[0].Rows[i]["certificate_no"].ToString())
                            };
                            DateTime dtDocDate;
                            if (dsEquipmentList.Tables[0].Rows[i]["CalibrationDate"].ToString() != ""
                             && DateTime.TryParse(dsEquipmentList.Tables[0].Rows[i]["CalibrationDate"].ToString(), out dtDocDate))
                            {
                                objCalibrationModels.CalibrationDate = dtDocDate;
                            }
                            string stmt = "Select Department,Freq_of_calibration from t_equipment where active=1 and " +
                   "Equipment_Id = '" + dsEquipmentList.Tables[0].Rows[i]["Equipment_id"].ToString() + "'";
                            DataSet EquipList = objGlobaldata.Getdetails(stmt);
                            if (EquipList.Tables.Count > 0 && EquipList.Tables[0].Rows.Count > 0)
                            {

                                objCalibrationModels.Department = objGlobaldata.GetMultiDeptNameById(EquipList.Tables[0].Rows[0]["Department"].ToString());

                                objCalibrationModels.Freq_of_calibration = objModel.GetCalibrationFrequencyNameById(EquipList.Tables[0].Rows[0]["Freq_of_calibration"].ToString());
                            }

                            objCalibrationModelsList.CalibrationMList.Add(objCalibrationModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CalibrationList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CalibrationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            ViewBag.Equipment_status = objGlobaldata.GetConstantValue("Status");

            return View(objCalibrationModelsList.CalibrationMList.ToList());
        }

        [AllowAnonymous]
        public JsonResult CalibrationListSearch(string SearchText, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Calibration";
            CalibrationModelsList objCalibrationModelsList = new CalibrationModelsList();
            objCalibrationModelsList.CalibrationMList = new List<CalibrationModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select calibration_id, tcal.Equipment_id, calibration_by, method_of_calibration, accuracy, calibration_status,"
                    + " calibration_report_ref, calibration_certificate, due_date, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name, "
                    + "CalibrationDate,Ref_no from  t_calibration as tcal, t_equipment as tequp"
                    + " where tcal.equipment_id=tequp.equipment_id and calibration_id in (select max(calibration_id) from t_calibration where tcal.Active=1 group by Equipment_id)";
                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = " and (tequp.Equipment_Name ='" + SearchText + "' or tequp.Equipment_Name like '" + SearchText + "%')";
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and tequp.branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and tequp.branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Equipment_Name asc";

                DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEquipmentList.Tables.Count > 0)
                {

                  

                    for (int i = 0; i < dsEquipmentList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            CalibrationModels objCalibrationModels = new CalibrationModels
                            {
                                calibration_id = (dsEquipmentList.Tables[0].Rows[i]["calibration_id"].ToString()),
                                Equipment_Id = (dsEquipmentList.Tables[0].Rows[i]["Equipment_Name"].ToString()),
                                calibration_by = dsEquipmentList.Tables[0].Rows[i]["calibration_by"].ToString(),
                                method_of_calibration = (dsEquipmentList.Tables[0].Rows[i]["method_of_calibration"].ToString()),
                                accuracy = dsEquipmentList.Tables[0].Rows[i]["accuracy"].ToString(),
                                calibration_status = dsEquipmentList.Tables[0].Rows[i]["calibration_status"].ToString(),
                                calibration_report_ref = (dsEquipmentList.Tables[0].Rows[i]["calibration_report_ref"].ToString()),
                                due_date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[i]["due_date"].ToString()),
                                calibration_certificate = (dsEquipmentList.Tables[0].Rows[i]["calibration_certificate"].ToString()),
                                Remarks = (dsEquipmentList.Tables[0].Rows[i]["Remarks"].ToString()),
                                Ref_no = (dsEquipmentList.Tables[0].Rows[i]["Ref_no"].ToString())
                            };
                            objCalibrationModelsList.CalibrationMList.Add(objCalibrationModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CalibrationListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CalibrationListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            ViewBag.Equipment_status = objGlobaldata.GetConstantValue("Status");

            return Json("Success");
        }

        // GET: /Equipment/EquipmentDetails

        [AllowAnonymous]
        public ActionResult CalibrationDetails()
        {
            ViewBag.SubMenutype = "Calibration";
            CalibrationModels objCalibrationModels = new CalibrationModels();
            try
            {

                if (Request.QueryString["calibration_id"] != null && Request.QueryString["calibration_id"] != "")
                {
                    string scalibration_id = Request.QueryString["calibration_id"];

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select calibration_id, tcal.Equipment_id, calibration_by, method_of_calibration, accuracy, calibration_status,"
                        + " calibration_report_ref, calibration_certificate, due_date, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name,"
                        + " NotificationPeriod, NotificationValue, Person_Responsible,Ref_no,certificate_no,CalibrationDate from  t_calibration as tcal, t_equipment as tequp where tcal.equipment_id=tequp.equipment_id "
                        + "and calibration_id='" + scalibration_id + "'";

                    DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsEquipmentList.Tables.Count > 0 && dsEquipmentList.Tables[0].Rows.Count > 0)
                    {
                        objCalibrationModels = new CalibrationModels
                        {
                            calibration_id = (dsEquipmentList.Tables[0].Rows[0]["calibration_id"].ToString()),
                            Equipment_Id = (dsEquipmentList.Tables[0].Rows[0]["Equipment_Name"].ToString()),
                            calibration_by = dsEquipmentList.Tables[0].Rows[0]["calibration_by"].ToString(),
                            method_of_calibration = (dsEquipmentList.Tables[0].Rows[0]["method_of_calibration"].ToString()),
                            accuracy = dsEquipmentList.Tables[0].Rows[0]["accuracy"].ToString(),
                            calibration_status = dsEquipmentList.Tables[0].Rows[0]["calibration_status"].ToString(),
                            calibration_report_ref = (dsEquipmentList.Tables[0].Rows[0]["calibration_report_ref"].ToString()),
                            due_date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[0]["due_date"].ToString()),
                            calibration_certificate = (dsEquipmentList.Tables[0].Rows[0]["calibration_certificate"].ToString()),
                            Remarks = (dsEquipmentList.Tables[0].Rows[0]["Remarks"].ToString()),
                            NotificationPeriod = dsEquipmentList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsEquipmentList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            Person_Responsible =objGlobaldata.GetEmpHrNameById(dsEquipmentList.Tables[0].Rows[0]["Person_Responsible"].ToString()),
                            Ref_no = dsEquipmentList.Tables[0].Rows[0]["Ref_no"].ToString(),
                            certificate_no = dsEquipmentList.Tables[0].Rows[0]["certificate_no"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsEquipmentList.Tables[0].Rows[0]["CalibrationDate"].ToString() != ""
                         && DateTime.TryParse(dsEquipmentList.Tables[0].Rows[0]["CalibrationDate"].ToString(), out dtDocDate))
                        {
                            objCalibrationModels.CalibrationDate = dtDocDate;
                        }
                        if (dsEquipmentList.Tables[0].Rows[0]["Equipment_Id"].ToString() != "" && dsEquipmentList.Tables[0].Rows[0]["Equipment_Id"].ToString() != null)
                        {

                            string stmt = "Select Department,branch,location from t_equipment where active=1 and " +
                                "Equipment_Id = '" + dsEquipmentList.Tables[0].Rows[0]["Equipment_Id"].ToString() + "'";
                            DataSet EquipList = objGlobaldata.Getdetails(stmt);
                            if (EquipList.Tables.Count > 0 && EquipList.Tables[0].Rows.Count > 0)
                            {
                                objCalibrationModels.branch = objGlobaldata.GetMultiCompanyBranchNameById(EquipList.Tables[0].Rows[0]["branch"].ToString());
                                objCalibrationModels.Department = objGlobaldata.GetMultiDeptNameById(EquipList.Tables[0].Rows[0]["Department"].ToString());
                                objCalibrationModels.Location = objGlobaldata.GetDivisionLocationById(EquipList.Tables[0].Rows[0]["location"].ToString());
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("CalibrationList");
                    }

                   
                }
                else
                {
                    TempData["alertdata"] = "Calibration Id cannot be Null or empty";
                    return RedirectToAction("CalibrationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CalibrationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objCalibrationModels);
        }

        [AllowAnonymous]
        public ActionResult CalibrationDetail()
        {
            ViewBag.SubMenutype = "Calibration";
            CalibrationModels objCalibrationModels = new CalibrationModels();
            EquipmentModels objModel = new EquipmentModels();
            try
            {

                if (Request.QueryString["calibration_id"] != null && Request.QueryString["calibration_id"] != "")
                {
                    string scalibration_id = Request.QueryString["calibration_id"];

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select calibration_id, tcal.Equipment_id, calibration_by, method_of_calibration, accuracy, calibration_status,"
                        + " calibration_report_ref, calibration_certificate, due_date, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name,"
                        + " NotificationPeriod, NotificationValue, Person_Responsible,Ref_no,certificate_no,CalibrationDate from  t_calibration as tcal, t_equipment as tequp where tcal.equipment_id=tequp.equipment_id "
                        + "and calibration_id='" + scalibration_id + "'";

                    DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsEquipmentList.Tables.Count > 0 && dsEquipmentList.Tables[0].Rows.Count > 0)
                    {
                        objCalibrationModels = new CalibrationModels
                        {
                            calibration_id = (dsEquipmentList.Tables[0].Rows[0]["calibration_id"].ToString()),
                            Equipment_Id = (dsEquipmentList.Tables[0].Rows[0]["Equipment_Name"].ToString()),
                            calibration_by = dsEquipmentList.Tables[0].Rows[0]["calibration_by"].ToString(),
                            method_of_calibration = (dsEquipmentList.Tables[0].Rows[0]["method_of_calibration"].ToString()),
                            accuracy = dsEquipmentList.Tables[0].Rows[0]["accuracy"].ToString(),
                            calibration_status = dsEquipmentList.Tables[0].Rows[0]["calibration_status"].ToString(),
                            calibration_report_ref = (dsEquipmentList.Tables[0].Rows[0]["calibration_report_ref"].ToString()),
                            due_date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[0]["due_date"].ToString()),
                            calibration_certificate = (dsEquipmentList.Tables[0].Rows[0]["calibration_certificate"].ToString()),
                            Remarks = (dsEquipmentList.Tables[0].Rows[0]["Remarks"].ToString()),
                            NotificationPeriod = dsEquipmentList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsEquipmentList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            Person_Responsible = objGlobaldata.GetEmpHrNameById(dsEquipmentList.Tables[0].Rows[0]["Person_Responsible"].ToString()),
                            Ref_no = dsEquipmentList.Tables[0].Rows[0]["Ref_no"].ToString(),
                            certificate_no = dsEquipmentList.Tables[0].Rows[0]["certificate_no"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsEquipmentList.Tables[0].Rows[0]["CalibrationDate"].ToString() != ""
                         && DateTime.TryParse(dsEquipmentList.Tables[0].Rows[0]["CalibrationDate"].ToString(), out dtDocDate))
                        {
                            objCalibrationModels.CalibrationDate = dtDocDate;
                        }
                        if (dsEquipmentList.Tables[0].Rows[0]["Equipment_Id"].ToString() != "" && dsEquipmentList.Tables[0].Rows[0]["Equipment_Id"].ToString() != null)
                        {

                            string stmt = "Select Department,branch,location,Freq_of_calibration from t_equipment where active=1 and " +
                                "Equipment_Id = '" + dsEquipmentList.Tables[0].Rows[0]["Equipment_Id"].ToString() + "'";
                            DataSet EquipList = objGlobaldata.Getdetails(stmt);
                            if (EquipList.Tables.Count > 0 && EquipList.Tables[0].Rows.Count > 0)
                            {
                                objCalibrationModels.branch = objGlobaldata.GetMultiCompanyBranchNameById(EquipList.Tables[0].Rows[0]["branch"].ToString());
                                objCalibrationModels.Department = objGlobaldata.GetMultiDeptNameById(EquipList.Tables[0].Rows[0]["Department"].ToString());
                                objCalibrationModels.Location = objGlobaldata.GetDivisionLocationById(EquipList.Tables[0].Rows[0]["location"].ToString());
                                objCalibrationModels.Freq_of_calibration = objModel.GetCalibrationFrequencyNameById(EquipList.Tables[0].Rows[0]["Freq_of_calibration"].ToString());
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("CalibrationList");
                    }


                }
                else
                {
                    TempData["alertdata"] = "Calibration Id cannot be Null or empty";
                    return RedirectToAction("CalibrationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CalibrationDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objCalibrationModels);
        }

        [AllowAnonymous]
        public ActionResult CalibrationInfo(int id)
        {
            CalibrationModels objCalibrationModels = new CalibrationModels();
            try
            {
                    string sSqlstmt = "select calibration_id, tcal.Equipment_id, calibration_by, method_of_calibration, accuracy, calibration_status,"
                        + " calibration_report_ref, calibration_certificate, due_date, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name,"
                        + " NotificationPeriod, NotificationValue, Person_Responsible,Ref_no from  t_calibration as tcal, t_equipment as tequp where tcal.equipment_id=tequp.equipment_id "
                        + "and calibration_id='" + id + "'";

                    DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsEquipmentList.Tables.Count > 0 && dsEquipmentList.Tables[0].Rows.Count > 0)
                    {
                        objCalibrationModels = new CalibrationModels
                        {
                            calibration_id = (dsEquipmentList.Tables[0].Rows[0]["calibration_id"].ToString()),
                            Equipment_Id = (dsEquipmentList.Tables[0].Rows[0]["Equipment_Name"].ToString()),
                            calibration_by = dsEquipmentList.Tables[0].Rows[0]["calibration_by"].ToString(),
                            method_of_calibration = (dsEquipmentList.Tables[0].Rows[0]["method_of_calibration"].ToString()),
                            accuracy = dsEquipmentList.Tables[0].Rows[0]["accuracy"].ToString(),
                            calibration_status = dsEquipmentList.Tables[0].Rows[0]["calibration_status"].ToString(),
                            calibration_report_ref = (dsEquipmentList.Tables[0].Rows[0]["calibration_report_ref"].ToString()),
                            due_date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[0]["due_date"].ToString()),
                            calibration_certificate = (dsEquipmentList.Tables[0].Rows[0]["calibration_certificate"].ToString()),
                            Remarks = (dsEquipmentList.Tables[0].Rows[0]["Remarks"].ToString()),
                            NotificationPeriod = dsEquipmentList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsEquipmentList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            Person_Responsible = objGlobaldata.GetEmpHrNameById(dsEquipmentList.Tables[0].Rows[0]["Person_Responsible"].ToString()),
                            Ref_no = dsEquipmentList.Tables[0].Rows[0]["Ref_no"].ToString(),
                        };
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("CalibrationList");
                    }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CalibrationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objCalibrationModels);
        }

        // GET: /Equipment/EquipmentEdit
         
        [AllowAnonymous]
        public ActionResult CalibrationEdit()
        {
            ViewBag.SubMenutype = "Calibration";
            CalibrationModels objCalibrationModels = new CalibrationModels();

            try
            {
                if (Request.QueryString["calibration_id"] != null && Request.QueryString["calibration_id"] != "")
                {
                    string scalibration_id = Request.QueryString["calibration_id"];

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select calibration_id,  tcal.Equipment_id, calibration_by, method_of_calibration, accuracy, calibration_status,"
                        + " calibration_report_ref, calibration_certificate, due_date, Remarks, tequp.Equipment_Name, NotificationPeriod, NotificationValue,  "
                        + " Person_Responsible,Ref_no,certificate_no,CalibrationDate from t_calibration as tcal, t_equipment as tequp where tcal.equipment_id=tequp.equipment_id and calibration_id='"
                        + scalibration_id + "'";

                    DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEquipmentList.Tables.Count > 0 && dsEquipmentList.Tables[0].Rows.Count > 0)
                    {
                        objCalibrationModels = new CalibrationModels
                        {
                            calibration_id = (dsEquipmentList.Tables[0].Rows[0]["calibration_id"].ToString()),
                            Equipment_Id = (dsEquipmentList.Tables[0].Rows[0]["Equipment_Id"].ToString()),
                            calibration_by = dsEquipmentList.Tables[0].Rows[0]["calibration_by"].ToString(),
                            method_of_calibration = (dsEquipmentList.Tables[0].Rows[0]["method_of_calibration"].ToString()),
                            accuracy = dsEquipmentList.Tables[0].Rows[0]["accuracy"].ToString(),
                            calibration_status = dsEquipmentList.Tables[0].Rows[0]["calibration_status"].ToString(),
                            calibration_report_ref = (dsEquipmentList.Tables[0].Rows[0]["calibration_report_ref"].ToString()),
                            due_date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[0]["due_date"].ToString()),
                            calibration_certificate = (dsEquipmentList.Tables[0].Rows[0]["calibration_certificate"].ToString()),
                            Remarks = (dsEquipmentList.Tables[0].Rows[0]["Remarks"].ToString()),
                            NotificationPeriod = dsEquipmentList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsEquipmentList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            Person_Responsible = (dsEquipmentList.Tables[0].Rows[0]["Person_Responsible"].ToString()),
                            Ref_no = dsEquipmentList.Tables[0].Rows[0]["Ref_no"].ToString(),
                            certificate_no = dsEquipmentList.Tables[0].Rows[0]["certificate_no"].ToString()
                        };
                        DateTime dtDocDate;
                        if (dsEquipmentList.Tables[0].Rows[0]["CalibrationDate"].ToString() != ""
                         && DateTime.TryParse(dsEquipmentList.Tables[0].Rows[0]["CalibrationDate"].ToString(), out dtDocDate))
                        {
                            objCalibrationModels.CalibrationDate = dtDocDate;
                        }
                        ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                        EquipmentModels objEquipment = new EquipmentModels();
                        ViewBag.Equipment_Id = objEquipment.GetEquipmentListbox();
                        ViewBag.calibration_status = objGlobaldata.GetConstantValue("calibration_status");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        return View(objCalibrationModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("CalibrationList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Calibration Id cannot be Null or empty";
                    return RedirectToAction("CalibrationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CalibrationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0][0];
            }
            return RedirectToAction("CalibrationList");
        }

        // POST: /Equipment/CalibrationEdit
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CalibrationEdit(CalibrationModels objCalibrationModels, FormCollection form, HttpPostedFileBase fileCert, HttpPostedFileBase fileReport_ref)
        {
            ViewBag.SubMenutype = "Calibration";
            try
            {

                if (objCalibrationModels != null)
                {
                    objCalibrationModels.calibration_status = form["calibration_status"];
                    objCalibrationModels.Equipment_Id = form["Equipment_Id"];

                    objCalibrationModels.calibration_id = form["calibration_id"];
                    objCalibrationModels.NotificationPeriod = form["NotificationPeriod"];
                    objCalibrationModels.NotificationValue = form["NotificationValue"];
                    objCalibrationModels.Person_Responsible = form["Person_Responsible"];

                    int Notificationval = 1;

                    if (objCalibrationModels.NotificationPeriod == "None")
                    {
                        Notificationval = 0;
                        objCalibrationModels.NotificationValue = "0";
                    }
                    else if (objCalibrationModels.NotificationPeriod == "Weeks" && int.TryParse(objCalibrationModels.NotificationValue, out Notificationval))
                    {
                        Notificationval = 7 * Notificationval;
                    }
                    else if (objCalibrationModels.NotificationPeriod == "Months" && int.TryParse(objCalibrationModels.NotificationValue, out Notificationval))
                    {
                        Notificationval = 30 * Notificationval;
                    }
                    objCalibrationModels.NotificationDays = Notificationval;

                    DateTime dateValue;

                    if (DateTime.TryParse(form["due_date"], out dateValue) == true)
                    {
                        objCalibrationModels.due_date = dateValue;
                    }

                    if (fileCert != null && fileCert.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Equipment"), Path.GetFileName(fileCert.FileName));
                            string sFilename = objCalibrationModels.Equipment_Id + "Cert_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            fileCert.SaveAs(sFilepath + "/" + sFilename);
                            objCalibrationModels.calibration_certificate = "~/DataUpload/MgmtDocs/Equipment/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CalibrationEdit-cert upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (fileReport_ref != null && fileReport_ref.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Equipment"), Path.GetFileName(fileReport_ref.FileName));
                            string sFilename = objCalibrationModels.Equipment_Id + "Report_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            fileReport_ref.SaveAs(sFilepath + "/" + sFilename);
                            objCalibrationModels.calibration_report_ref = "~/DataUpload/MgmtDocs/Equipment/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CalibrationEdit-report upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (objCalibrationModels.FunUpdateCalibration(objCalibrationModels))
                    {
                        TempData["Successdata"] = "Updated Calibartion details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CalibrationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CalibrationList");
        }

       public JsonResult FunGetEquipmentDetails(string EquipmentId)
        {
            EquipmentModels objModels = new EquipmentModels();
            if(EquipmentId != "" && EquipmentId != null)
            {
                
                string stmt= "Select Department,branch,location,Freq_of_calibration from t_equipment where active=1 and " +
                    "Equipment_Id = '" + EquipmentId + "'";
                DataSet EquipList = objGlobaldata.Getdetails(stmt);
                if(EquipList.Tables.Count>0 && EquipList.Tables[0].Rows.Count>0)
                {
                    objModels.branch = objGlobaldata.GetMultiCompanyBranchNameById(EquipList.Tables[0].Rows[0]["branch"].ToString());
                    objModels.Department = objGlobaldata.GetMultiDeptNameById(EquipList.Tables[0].Rows[0]["Department"].ToString());
                    objModels.location = objGlobaldata.GetDivisionLocationById(EquipList.Tables[0].Rows[0]["location"].ToString());
                    objModels.Freq_of_calibration = objModels.GetCalibrationFrequencyNameById(EquipList.Tables[0].Rows[0]["Freq_of_calibration"].ToString());
                }
               
            }
            return Json(objModels);
        }

        [AllowAnonymous]
        public ActionResult AddMaintanance()
        {

            ViewBag.SubMenutype = "Equipment";
            if (Request.QueryString["Equipment_Id"] != null && Request.QueryString["Equipment_Id"] != "")
            {
                return InitilizeAddMaintanance(Request.QueryString["Equipment_Id"]);
            }
            else
            {
                TempData["alertdata"] = "Equipment Id cannot be Null or empty";
            }

            return RedirectToAction("MaintananceList");
        }

        // GET: /Equipment/AddMaintanance
         
        [AllowAnonymous]
        public ActionResult InitilizeAddMaintanance(string sEquipment_Id)
        {
            try
            {
                EquipmentModels objEquipment = new EquipmentModels();
                //ViewBag.Equipmentnames = objEquipment.GetEquipmentListbox();

                ViewBag.Equipment_Id = sEquipment_Id;
                ViewBag.Equipment_Name = objEquipment.GetEquipmentNameById(sEquipment_Id);

                ViewBag.TimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.TimeInMin = objGlobaldata.GetAuditTimeInMin();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddMaintanance: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        // POST: /Equipment/AddMaintanance
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMaintanance(EquipmentMaintanance objEquipmentMaintanance, FormCollection form, HttpPostedFileBase Maintenance_Details)
        {
            ViewBag.SubMenutype = "Equipment";
            string sEquipment_Id = form["Equipment_Id"];
            try
            {
                objEquipmentMaintanance.Equipment_Id = sEquipment_Id;
                DateTime dateValue;

                if (DateTime.TryParse(form["Maintanance_Date"], out dateValue) == true)
                {
                    objEquipmentMaintanance.Maintenance_Date = dateValue;
                }

                if (DateTime.TryParse(form["FromTimeDate"], out dateValue) == true)
                {
                    objEquipmentMaintanance.Down_Time_From = Convert.ToDateTime(dateValue.ToString("yyyy/MM/dd") + " " + form["FromTimeInHour"] + ":" + form["FromTimeInMin"]);
                }

                if (DateTime.TryParse(form["ToTimeDate"], out dateValue) == true)
                {
                    objEquipmentMaintanance.Down_Time_To =  Convert.ToDateTime(dateValue.ToString("yyyy/MM/dd") + " " + form["ToTimeInHour"] + ":" + form["ToTimeInMin"]);
                }               
                

                if (Maintenance_Details != null && Maintenance_Details.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Equipment"), Path.GetFileName(Maintenance_Details.FileName));
                        string sFileFullPath = objEquipmentMaintanance.Equipment_Id + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Maintenance_Details.SaveAs(sFilepath + "/" +  sFileFullPath);
                        objEquipmentMaintanance.Maintenance_Details = "~/DataUpload/MgmtDocs/Equipment/" + sFileFullPath;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddMaintanance-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }

                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objEquipmentMaintanance.FunAddMaintanance(objEquipmentMaintanance))
                {
                    TempData["Successdata"] = "Added Maintenance details successfully";
                    return RedirectToAction("MaintenanceList", new { Equipment_Id = sEquipment_Id });
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddMaintanance: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return InitilizeAddMaintanance(sEquipment_Id);
        }


        // GET: /Equipment/MaintenanceList
         
        [AllowAnonymous]
        public ActionResult MaintenanceList(string SearchText, int? page)
        {
            ViewBag.SubMenutype = "Equipment";
            EquipmentMaintananceList objEquipmentMaintananceList = new EquipmentMaintananceList();
            objEquipmentMaintananceList.lstEquipmentMaintanance = new List<EquipmentMaintanance>();

            try
            {
                if (Request.QueryString["Equipment_Id"] != null && Request.QueryString["Equipment_Id"] != "")
                {
                    string sEquipment_Id = Request.QueryString["Equipment_Id"];
                    ViewBag.Equipment_Id = sEquipment_Id;
                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select Maintenance_Id, tmain.Equipment_Id, Maintenance_Date, Maintenance_Details, Maintenance_RectificationWork,"
                        + " Down_Time_From, Down_Time_To, Spare_Used, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name,Amt_Spent from "
                        + " t_equipment_maintenance as tmain, t_equipment as tequp where tmain.equipment_id=tequp.equipment_id and tmain.equipment_id='" + sEquipment_Id + "'";
                    string sSearchtext = "";

                    //if (SearchText != null && SearchText != "")
                    //{
                    //    ViewBag.SearchText = SearchText;
                    //    sSearchtext = " and (tequp.Equipment_Name ='" + SearchText + "' or tequp.Equipment_Name '" + SearchText + "%')";
                    //}


                    sSqlstmt = sSqlstmt + sSearchtext + " order by Equipment_name ";


                    DataSet dsMaintenanceList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMaintenanceList.Tables.Count > 0)
                    {
                        for (int i = 0; i < dsMaintenanceList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EquipmentMaintanance objEquipmentMaintanance = new EquipmentMaintanance
                                {
                                    Maintenance_Id = (dsMaintenanceList.Tables[0].Rows[i]["Maintenance_Id"].ToString()),
                                    Equipment_Id = (dsMaintenanceList.Tables[0].Rows[i]["Equipment_Name"].ToString()),
                                    Maintenance_Details = dsMaintenanceList.Tables[0].Rows[i]["Maintenance_Details"].ToString(),
                                    Maintenance_RectificationWork = (dsMaintenanceList.Tables[0].Rows[i]["Maintenance_RectificationWork"].ToString()),
                                    Down_Time_From =Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[i]["Down_Time_From"].ToString()),
                                    Down_Time_To = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[i]["Down_Time_To"].ToString()),
                                    Spare_Used = dsMaintenanceList.Tables[0].Rows[i]["Spare_Used"].ToString(),
                                    Maintenance_Date = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[i]["Maintenance_Date"].ToString()),
                                    Remarks = (dsMaintenanceList.Tables[0].Rows[i]["Remarks"].ToString()),
                                    Amt_Spent = Convert.ToDecimal(dsMaintenanceList.Tables[0].Rows[i]["Amt_Spent"].ToString())
                                };
                                objEquipmentMaintananceList.lstEquipmentMaintanance.Add(objEquipmentMaintanance);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in MaintananceList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EquipmentList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Equipment Id cannot be Null or empty";
                    return RedirectToAction("EquipmentList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MaintananceList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEquipmentMaintananceList.lstEquipmentMaintanance.ToList());
        }

        // GET: /Equipment/MaintenanceDetails
         
        [AllowAnonymous]
        public ActionResult MaintenanceDetails()
        {
            ViewBag.SubMenutype = "Equipment";
            EquipmentMaintanance objEquipmentMaintanance = new EquipmentMaintanance();

            try
            {
                if (Request.QueryString["Maintenance_Id"] != null && Request.QueryString["Maintenance_Id"] != "")
                {
                    string sMaintenance_Id = Request.QueryString["Maintenance_Id"];
                    string sSqlstmt = "select Maintenance_Id, tmain.Equipment_Id, Equipment_Name, Maintenance_Date, Maintenance_Details, Maintenance_RectificationWork,"
                        + " Down_Time_From, Down_Time_To, Spare_Used, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name,Amt_Spent from  t_equipment_maintenance as tmain, "
                                + "t_equipment as tequp where tmain.equipment_id=tequp.equipment_id and tmain.Maintenance_Id='" + sMaintenance_Id + "'";

                    DataSet dsMaintenanceList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMaintenanceList.Tables.Count > 0 && dsMaintenanceList.Tables[0].Rows.Count > 0)
                    {
                        objEquipmentMaintanance = new EquipmentMaintanance
                        {
                            Maintenance_Id = (dsMaintenanceList.Tables[0].Rows[0]["Maintenance_Id"].ToString()),
                            Equipment_Id = (dsMaintenanceList.Tables[0].Rows[0]["Equipment_Name"].ToString()),
                            Equipment_Id1 = (dsMaintenanceList.Tables[0].Rows[0]["Equipment_Id"].ToString()),
                            Maintenance_Details = dsMaintenanceList.Tables[0].Rows[0]["Maintenance_Details"].ToString(),
                            Maintenance_RectificationWork = (dsMaintenanceList.Tables[0].Rows[0]["Maintenance_RectificationWork"].ToString()),
                            Down_Time_From =Convert.ToDateTime( dsMaintenanceList.Tables[0].Rows[0]["Down_Time_From"].ToString()),
                            Down_Time_To = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[0]["Down_Time_To"].ToString()),
                            Spare_Used = dsMaintenanceList.Tables[0].Rows[0]["Spare_Used"].ToString(),
                            Maintenance_Date = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[0]["Maintenance_Date"].ToString()),
                            Remarks = (dsMaintenanceList.Tables[0].Rows[0]["Remarks"].ToString()),
                            Amt_Spent = Convert.ToDecimal(dsMaintenanceList.Tables[0].Rows[0]["Amt_Spent"].ToString())
                        };
                        ViewBag.Equipment_Id = sMaintenance_Id;
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("EquipmentList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Equipment Id cannot be Null or empty";
                    return RedirectToAction("EquipmentList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MaintenanceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEquipmentMaintanance);
        }


        // GET: /Equipment/MaintenanceEdit
         
        [AllowAnonymous]
        public ActionResult MaintenanceEdit()
        {
            ViewBag.SubMenutype = "Equipment";
            EquipmentMaintanance objEquipmentMaintanance = new EquipmentMaintanance();

            try
            {
                if (Request.QueryString["Maintenance_Id"] != null && Request.QueryString["Maintenance_Id"] != "")
                {
                    string sMaintenance_Id = Request.QueryString["Maintenance_Id"];
                    string sSqlstmt = "select Maintenance_Id, tmain.Equipment_Id, Equipment_Name, Maintenance_Date, Maintenance_Details, Maintenance_RectificationWork,"
                        + " Down_Time_From, Down_Time_To, Spare_Used, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name,Amt_Spent from "
                        + " t_equipment_maintenance as tmain, t_equipment as tequp where tmain.equipment_id=tequp.equipment_id and tmain.Maintenance_Id='" 
                        + sMaintenance_Id + "'";

                    DataSet dsMaintenanceList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMaintenanceList.Tables.Count > 0 && dsMaintenanceList.Tables[0].Rows.Count > 0)
                    {
                        objEquipmentMaintanance = new EquipmentMaintanance
                        {
                            Maintenance_Id = (dsMaintenanceList.Tables[0].Rows[0]["Maintenance_Id"].ToString()),
                            Equipment_Id = (dsMaintenanceList.Tables[0].Rows[0]["Equipment_Name"].ToString()),
                            Maintenance_Details = dsMaintenanceList.Tables[0].Rows[0]["Maintenance_Details"].ToString(),
                            Maintenance_RectificationWork = (dsMaintenanceList.Tables[0].Rows[0]["Maintenance_RectificationWork"].ToString()),
                            Down_Time_From = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[0]["Down_Time_From"].ToString()),
                            Down_Time_To = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[0]["Down_Time_To"].ToString()),
                            Spare_Used = dsMaintenanceList.Tables[0].Rows[0]["Spare_Used"].ToString(),
                            Maintenance_Date = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[0]["Maintenance_Date"].ToString()),
                            Remarks = (dsMaintenanceList.Tables[0].Rows[0]["Remarks"].ToString()),
                            Amt_Spent = Convert.ToDecimal(dsMaintenanceList.Tables[0].Rows[0]["Amt_Spent"].ToString())
                        };
                        ViewBag.Equipment_Id = dsMaintenanceList.Tables[0].Rows[0]["Equipment_Id"].ToString();

                        ViewBag.TimeInHour = objGlobaldata.GetAuditTimeInHour();
                        ViewBag.TimeInMin = objGlobaldata.GetAuditTimeInMin();
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("EquipmentList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Equipment Id cannot be Null or empty";
                    return RedirectToAction("EquipmentList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MaintenanceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            
            return View(objEquipmentMaintanance);
        }


        // POST: /Equipment/AddMaintanance
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MaintenanceEdit(EquipmentMaintanance objEquipmentMaintanance, FormCollection form, HttpPostedFileBase Maintenance_Details)
        {
            ViewBag.SubMenutype = "Equipment";
            string sEquipment_Id = form["Equipment_Id"];
            try
            {

                DateTime dateValue;

                if (DateTime.TryParse(form["Maintanance_Date"], out dateValue) == true)
                {
                    objEquipmentMaintanance.Maintenance_Date = dateValue;
                }

                if (DateTime.TryParse(form["FromTimeDate"], out dateValue) == true)
                {
                    objEquipmentMaintanance.Down_Time_From = Convert.ToDateTime(dateValue.ToString("yyyy/MM/dd") + " " + form["FromTimeInHour"] + ":" + form["FromTimeInMin"]);
                }

                if (DateTime.TryParse(form["ToTimeDate"], out dateValue) == true)
                {
                    objEquipmentMaintanance.Down_Time_To =Convert.ToDateTime(dateValue.ToString("yyyy/MM/dd") + " " + form["ToTimeInHour"] + ":" + form["ToTimeInMin"]);
                }  

                if (Maintenance_Details != null && Maintenance_Details.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Equipment"), Path.GetFileName(Maintenance_Details.FileName));
                        string sFileFullPath = objEquipmentMaintanance.Equipment_Id + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Maintenance_Details.SaveAs(sFilepath + "/" + sFileFullPath);
                        objEquipmentMaintanance.Maintenance_Details = "~/DataUpload/MgmtDocs/Equipment/" + sFileFullPath;
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in MaintenanceEdit-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objEquipmentMaintanance.FunUpdateMaintanance(objEquipmentMaintanance))
                {
                    TempData["Successdata"] = "Maintenance details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MaintenanceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MaintenanceList", new { Equipment_Id = sEquipment_Id });
        }


        //Preventive Maintainance

        [AllowAnonymous]
        public ActionResult AddPreventiveMaint()
        {

            ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
            ViewBag.SubMenutype = "Equipment";
            if (Request.QueryString["Equipment_Id"] != null && Request.QueryString["Equipment_Id"] != "")
            {
                return InitilizeAddMaintanance(Request.QueryString["Equipment_Id"]);
            }
            else
            {
                TempData["alertdata"] = "Equipment Id cannot be Null or empty";
            }

            return RedirectToAction("PMaintenanceList");
        }
                     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPreventiveMaint(EquipmentMaintanance objEquipmentMaintanance, FormCollection form, HttpPostedFileBase Maintenance_Details)
        {
            ViewBag.SubMenutype = "Equipment";
            string sEquipment_Id = form["Equipment_Id"];
            try
            {
                objEquipmentMaintanance.Equipment_Id = sEquipment_Id;
                DateTime dateValue;

                if (DateTime.TryParse(form["Maintanance_Date"], out dateValue) == true)
                {
                    objEquipmentMaintanance.Maintenance_Date = dateValue;
                }

                if (DateTime.TryParse(form["Next_Maint_Date"], out dateValue) == true)
                {
                    objEquipmentMaintanance.Next_Maint_Date = dateValue;
                }

                int Notificationval = 1;

                if (objEquipmentMaintanance.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objEquipmentMaintanance.NotificationValue = "0";
                }
                else if (objEquipmentMaintanance.NotificationPeriod == "Weeks" && int.TryParse(objEquipmentMaintanance.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objEquipmentMaintanance.NotificationPeriod == "Months" && int.TryParse(objEquipmentMaintanance.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objEquipmentMaintanance.NotificationDays = Notificationval;

                if (Maintenance_Details != null && Maintenance_Details.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Equipment"), Path.GetFileName(Maintenance_Details.FileName));
                        string sFileFullPath = objEquipmentMaintanance.Equipment_Id + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Maintenance_Details.SaveAs(sFilepath + "/" + sFileFullPath);
                        objEquipmentMaintanance.Maintenance_Details = "~/DataUpload/MgmtDocs/Equipment/" + sFileFullPath;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddPreventiveMaint-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }

                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objEquipmentMaintanance.FunAddPreventiveMaint(objEquipmentMaintanance))
                {
                    TempData["Successdata"] = "Added Preventive Maintenance details successfully";
                    return RedirectToAction("PMaintenanceList", new { Equipment_Id = sEquipment_Id });
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPreventiveMaint: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return InitilizeAddMaintanance(sEquipment_Id);
        }

        [AllowAnonymous]
        public ActionResult PMaintenanceList(string SearchText, int? page)
        {
            ViewBag.SubMenutype = "Equipment";
            EquipmentMaintananceList objEquipmentMaintananceList = new EquipmentMaintananceList();
            objEquipmentMaintananceList.lstEquipmentMaintanance = new List<EquipmentMaintanance>();

            try
            {
                if (Request.QueryString["Equipment_Id"] != null && Request.QueryString["Equipment_Id"] != "")
                {
                    string sEquipment_Id = Request.QueryString["Equipment_Id"];
                    ViewBag.Equipment_Id = sEquipment_Id;
                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select Preventive_Id, tmain.Equipment_Id, Maintenance_Date, Maintenance_Details, Next_Maint_Date,Amt_Spent,"
                        + " done_by, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name from "
                        + " t_equpiment_preventive_maint as tmain, t_equipment as tequp where tmain.equipment_id=tequp.equipment_id and tmain.equipment_id='" + sEquipment_Id + "'";
                    string sSearchtext = "";

                    sSqlstmt = sSqlstmt + sSearchtext + " order by Equipment_name ";
                    
                    DataSet dsMaintenanceList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMaintenanceList.Tables.Count > 0)
                    {
                        for (int i = 0; i < dsMaintenanceList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EquipmentMaintanance objEquipmentMaintanance = new EquipmentMaintanance
                                {
                                    Preventive_Id = (dsMaintenanceList.Tables[0].Rows[i]["Preventive_Id"].ToString()),
                                    Equipment_Id = (dsMaintenanceList.Tables[0].Rows[i]["Equipment_Name"].ToString()),
                                    Maintenance_Details = dsMaintenanceList.Tables[0].Rows[i]["Maintenance_Details"].ToString(),
                                    done_by = (dsMaintenanceList.Tables[0].Rows[i]["done_by"].ToString()),
                                    Next_Maint_Date = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[i]["Next_Maint_Date"].ToString()),
                                    Maintenance_Date = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[i]["Maintenance_Date"].ToString()),
                                    Remarks = (dsMaintenanceList.Tables[0].Rows[i]["Remarks"].ToString()),
                                    Amt_Spent=Convert.ToDecimal(dsMaintenanceList.Tables[0].Rows[i]["Amt_Spent"].ToString())
                                };
                                objEquipmentMaintananceList.lstEquipmentMaintanance.Add(objEquipmentMaintanance);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in PMaintananceList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EquipmentList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Equipment Id cannot be Null or empty";
                    return RedirectToAction("EquipmentList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PMaintenanceList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEquipmentMaintananceList.lstEquipmentMaintanance.ToList());
        }

        [AllowAnonymous]
        public ActionResult PMaintenanceEdit()
        {
            ViewBag.SubMenutype = "Equipment";
            EquipmentMaintanance objEquipmentMaintanance = new EquipmentMaintanance();

            ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");

            try
            {
                if (Request.QueryString["Preventive_Id"] != null && Request.QueryString["Preventive_Id"] != "")
                {
                    string sPreventive_Id = Request.QueryString["Preventive_Id"];
                    string sSqlstmt = "select Preventive_Id, tmain.Equipment_Id, Equipment_Name, Maintenance_Date, Maintenance_Details, Next_Maint_Date,Amt_Spent,"
                        + " done_by, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name,NotificationDays,NotificationPeriod,NotificationValue from "
                        + " t_equpiment_preventive_maint as tmain, t_equipment as tequp where tmain.equipment_id=tequp.equipment_id and tmain.Preventive_Id='"
                        + sPreventive_Id + "'";

                    DataSet dsMaintenanceList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMaintenanceList.Tables.Count > 0 && dsMaintenanceList.Tables[0].Rows.Count > 0)
                    {
                        objEquipmentMaintanance = new EquipmentMaintanance
                        {
                            Preventive_Id = dsMaintenanceList.Tables[0].Rows[0]["Preventive_Id"].ToString(),
                            Equipment_Id = dsMaintenanceList.Tables[0].Rows[0]["Equipment_Name"].ToString(),
                            Maintenance_Details = dsMaintenanceList.Tables[0].Rows[0]["Maintenance_Details"].ToString(),
                            Next_Maint_Date = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[0]["Next_Maint_Date"].ToString()),
                            done_by = dsMaintenanceList.Tables[0].Rows[0]["done_by"].ToString(),
                            Maintenance_Date = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[0]["Maintenance_Date"].ToString()),
                            Remarks = (dsMaintenanceList.Tables[0].Rows[0]["Remarks"].ToString()),
                            Amt_Spent=Convert.ToDecimal(dsMaintenanceList.Tables[0].Rows[0]["Amt_Spent"].ToString()),
                            NotificationPeriod = dsMaintenanceList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsMaintenanceList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                        };
                        ViewBag.Equipment_Id = dsMaintenanceList.Tables[0].Rows[0]["Equipment_Id"].ToString();

                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("EquipmentList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Equipment Id cannot be Null or empty";
                    return RedirectToAction("EquipmentList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PMaintenanceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEquipmentMaintanance);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PMaintenanceEdit(EquipmentMaintanance objEquipmentMaintanance, FormCollection form, HttpPostedFileBase Maintenance_Details)
        {
            ViewBag.SubMenutype = "Equipment";
            string sEquipment_Id = form["Equipment_Id"];
            try
            {
                DateTime dateValue;

                if (DateTime.TryParse(form["Maintanance_Date"], out dateValue) == true)
                {
                    objEquipmentMaintanance.Maintenance_Date = dateValue;
                }

                if (DateTime.TryParse(form["Next_Maint_Date"], out dateValue) == true)
                {
                    objEquipmentMaintanance.Next_Maint_Date = dateValue;
                }

                int Notificationval = 1;

                if (objEquipmentMaintanance.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objEquipmentMaintanance.NotificationValue = "0";
                }
                else if (objEquipmentMaintanance.NotificationPeriod == "Weeks" && int.TryParse(objEquipmentMaintanance.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objEquipmentMaintanance.NotificationPeriod == "Months" && int.TryParse(objEquipmentMaintanance.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objEquipmentMaintanance.NotificationDays = Notificationval;

                if (Maintenance_Details != null && Maintenance_Details.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Equipment"), Path.GetFileName(Maintenance_Details.FileName));
                        string sFileFullPath = objEquipmentMaintanance.Equipment_Id + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Maintenance_Details.SaveAs(sFilepath + "/" + sFileFullPath);
                        objEquipmentMaintanance.Maintenance_Details = "~/DataUpload/MgmtDocs/Equipment/" + sFileFullPath;
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in PMaintenanceEdit-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objEquipmentMaintanance.FunUpdatePreventiveMaint(objEquipmentMaintanance))
                {
                    TempData["Successdata"] = "Maintenance details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PMaintenanceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("PMaintenanceList", new { Equipment_Id = sEquipment_Id });
        }

        [AllowAnonymous]
        public ActionResult PMaintenanceDetails()
        {
            ViewBag.SubMenutype = "Equipment";
            EquipmentMaintanance objEquipmentMaintanance = new EquipmentMaintanance();

            try
            {
                if (Request.QueryString["Preventive_Id"] != null && Request.QueryString["Preventive_Id"] != "")
                {
                    string sPreventive_Id = Request.QueryString["Preventive_Id"];
                    string sSqlstmt = "select Preventive_Id, tmain.Equipment_Id, Equipment_Name, Maintenance_Date, Maintenance_Details, Next_Maint_Date,Amt_Spent,"
                        + " done_by, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name,NotificationPeriod,NotificationDays,NotificationValue from "
                        + " t_equpiment_preventive_maint as tmain, t_equipment as tequp where tmain.equipment_id=tequp.equipment_id and tmain.Preventive_Id='"+ sPreventive_Id + "'";
                    
                    DataSet dsMaintenanceList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMaintenanceList.Tables.Count > 0 && dsMaintenanceList.Tables[0].Rows.Count > 0)
                    {
                        objEquipmentMaintanance = new EquipmentMaintanance
                        {
                            Preventive_Id = dsMaintenanceList.Tables[0].Rows[0]["Preventive_Id"].ToString(),
                            Equipment_Id = dsMaintenanceList.Tables[0].Rows[0]["Equipment_Name"].ToString(),
                            Equipment_Id1 = dsMaintenanceList.Tables[0].Rows[0]["Equipment_Id"].ToString(),
                            Maintenance_Details = dsMaintenanceList.Tables[0].Rows[0]["Maintenance_Details"].ToString(),
                            Next_Maint_Date = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[0]["Next_Maint_Date"].ToString()),
                            done_by = dsMaintenanceList.Tables[0].Rows[0]["done_by"].ToString(),
                            Maintenance_Date = Convert.ToDateTime(dsMaintenanceList.Tables[0].Rows[0]["Maintenance_Date"].ToString()),
                            Remarks = (dsMaintenanceList.Tables[0].Rows[0]["Remarks"].ToString()),
                            Amt_Spent = Convert.ToDecimal(dsMaintenanceList.Tables[0].Rows[0]["Amt_Spent"].ToString()),
                            NotificationPeriod = dsMaintenanceList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsMaintenanceList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                        };
                        ViewBag.Equipment_Id = sPreventive_Id;
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("EquipmentList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Equipment Id cannot be Null or empty";
                    return RedirectToAction("EquipmentList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PMaintenanceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEquipmentMaintanance);
        }

        [AllowAnonymous]
        public ActionResult CalibrationHistoryList(string SearchText, int? page)
        {
            ViewBag.SubMenutype = "Calibration";
            CalibrationModelsList objCalibrationModelsList = new CalibrationModelsList();
            objCalibrationModelsList.CalibrationMList = new List<CalibrationModels>();

            try
            {
                if (Request.QueryString["calibration_id"] != null && Request.QueryString["calibration_id"] != "")
                {
                    string calibration_id = Request.QueryString["calibration_id"].ToString();

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select calibration_id, tcal.Equipment_id, calibration_by, method_of_calibration, accuracy, calibration_status,"
                        + " calibration_report_ref, calibration_certificate, due_date, Remarks, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name, "
                        + "CalibrationDate,Ref_no from  t_calibration_history as tcal, t_equipment as tequp"
                        + " where tcal.equipment_id=tequp.equipment_id and calibration_id in (select max(calibration_id) from t_calibration_history where tcal.Active=1 and calibration_id='" + calibration_id + "' group by Equipment_id)";
                    string sSearchtext = "";


                    sSqlstmt = sSqlstmt + sSearchtext + " order by Equipment_Name asc";

                    DataSet dsEquipmentList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsEquipmentList.Tables.Count > 0)
                    {
                        for (int i = 0; i < dsEquipmentList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                CalibrationModels objCalibrationModels = new CalibrationModels
                                {
                                    calibration_id = (dsEquipmentList.Tables[0].Rows[i]["calibration_id"].ToString()),
                                    Equipment_Id = (dsEquipmentList.Tables[0].Rows[i]["Equipment_Name"].ToString()),
                                    calibration_by = dsEquipmentList.Tables[0].Rows[i]["calibration_by"].ToString(),
                                    method_of_calibration = (dsEquipmentList.Tables[0].Rows[i]["method_of_calibration"].ToString()),
                                    accuracy = dsEquipmentList.Tables[0].Rows[i]["accuracy"].ToString(),
                                    calibration_status = dsEquipmentList.Tables[0].Rows[i]["calibration_status"].ToString(),
                                    calibration_report_ref = (dsEquipmentList.Tables[0].Rows[i]["calibration_report_ref"].ToString()),
                                    due_date = Convert.ToDateTime(dsEquipmentList.Tables[0].Rows[i]["due_date"].ToString()),
                                    calibration_certificate = (dsEquipmentList.Tables[0].Rows[i]["calibration_certificate"].ToString()),
                                    Remarks = (dsEquipmentList.Tables[0].Rows[i]["Remarks"].ToString()),
                                    Ref_no = (dsEquipmentList.Tables[0].Rows[i]["Ref_no"].ToString())
                                };
                                objCalibrationModelsList.CalibrationMList.Add(objCalibrationModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in CalibrationHistoryList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CalibrationHistoryList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            ViewBag.Equipment_status = objGlobaldata.GetConstantValue("Status");

            return View(objCalibrationModelsList.CalibrationMList.ToList());
        }

    }
}
