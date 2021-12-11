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
    public class DieselEquipmentController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public DieselEquipmentController()
        {
            ViewBag.Menutype = "VehicleMgmt";
        }

        //
        // GET: /DieselEquipment/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /DieselEquipment/AddDieselEuipment

        [AllowAnonymous]
        public ActionResult AddDieselEuipment()
        {
            return View();
        }

        //
        // POST: /DieselEquipment/AddDieselEuipment

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDieselEuipment(DieselEquipmentModels objDieselEquipment, FormCollection form)
        {
            try
            {
                DateTime dateValue;

                if (form["Date_Of_Purchase"] != null && DateTime.TryParse(form["Date_Of_Purchase"], out dateValue) == true)
                {
                    objDieselEquipment.Date_Of_Purchase = dateValue;
                }

                if (form["Reg_Issued_Date"] != null && DateTime.TryParse(form["Reg_Issued_Date"], out dateValue) == true)
                {
                    objDieselEquipment.Reg_Issued_Date = dateValue;
                }

                if (form["Reg_Expr_Date"] != null && DateTime.TryParse(form["Reg_Expr_Date"], out dateValue) == true)
                {
                    objDieselEquipment.Reg_Expr_Date = dateValue;
                }
                if (form["Insurance_ExpDate"] != null && DateTime.TryParse(form["Insurance_ExpDate"], out dateValue) == true)
                {
                    objDieselEquipment.Insurance_ExpDate = dateValue;
                }

                if (objDieselEquipment.FunAddDieselEquipment(objDieselEquipment))
                {
                    TempData["Successdata"] = "Added Euipment details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDieselEuipment: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("DieselEuipmentList");
        }

        //
        // GET: /DieselEquipment/DieselEuipmentList

        [AllowAnonymous]
        public ActionResult DieselEuipmentList(string Equip_No, int? page)
        {
            DieselEquipmentModelsList objDieselEquipmentList = new DieselEquipmentModelsList();
            objDieselEquipmentList.lstDieselEquipment = new List<DieselEquipmentModels>();

            try
            {
                string sSqlstmt = "select Diesel_Equip_Id, Equip_No, Description, Make, Capacity, Model, Chasis_No, Engine_No, Date_Of_Purchase, Reg_Issued_Date, "
                    + " Reg_Expr_Date, LoggedBy, Equp_Status,Index_no,Plate_no,Insurance_ExpDate from t_diesel_equipment where Equp_Status=1 ";

                if (Equip_No != null && Equip_No != "")
                {
                    ViewBag.Equip_No = Equip_No;

                    sSqlstmt = sSqlstmt + " and (Equip_No ='" + Equip_No + "'  or Equip_No like '%" + Equip_No + "%' or Equip_No like '" + Equip_No + "%')";
                }

                sSqlstmt = sSqlstmt + " order by Description, Equip_No asc";

                DataSet dsRecordsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRecordsList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsRecordsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            DieselEquipmentModels objDieselEquipment = new DieselEquipmentModels
                            {
                                Diesel_Equip_Id = dsRecordsList.Tables[0].Rows[i]["Diesel_Equip_Id"].ToString(),
                                Equip_No = dsRecordsList.Tables[0].Rows[i]["Equip_No"].ToString(),
                                Description = dsRecordsList.Tables[0].Rows[i]["Description"].ToString(),
                                Make = (dsRecordsList.Tables[0].Rows[i]["Make"].ToString()),
                                Capacity = dsRecordsList.Tables[0].Rows[i]["Capacity"].ToString(),
                                Model = (dsRecordsList.Tables[0].Rows[i]["Model"].ToString()),
                                Chasis_No = dsRecordsList.Tables[0].Rows[i]["Chasis_No"].ToString(),
                                Engine_No = dsRecordsList.Tables[0].Rows[i]["Engine_No"].ToString(),
                                Equp_Status = dsRecordsList.Tables[0].Rows[i]["Equp_Status"].ToString(),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsRecordsList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                Index_no = dsRecordsList.Tables[0].Rows[i]["Index_no"].ToString(),
                                Plate_no = dsRecordsList.Tables[0].Rows[i]["Plate_no"].ToString(),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[i]["Date_Of_Purchase"].ToString(), out dateValue))
                            {
                                objDieselEquipment.Date_Of_Purchase = dateValue;
                            }

                            if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[i]["Reg_Issued_Date"].ToString(), out dateValue))
                            {
                                objDieselEquipment.Reg_Issued_Date = dateValue;
                            }

                            if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[i]["Reg_Expr_Date"].ToString(), out dateValue))
                            {
                                objDieselEquipment.Reg_Expr_Date = dateValue;
                            }
                            if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[i]["Insurance_ExpDate"].ToString(), out dateValue))
                            {
                                objDieselEquipment.Insurance_ExpDate = dateValue;
                            }
                            objDieselEquipment.color_code_Reg = "#fff";
                            objDieselEquipment.color_code_Issue = "#fff";

                            DateTime Today, TodayGap;
                            Today = DateTime.Today;
                            TodayGap = Today.AddDays(-30);

                            if (objDieselEquipment.Insurance_ExpDate <= TodayGap || objDieselEquipment.Insurance_ExpDate <= Today)
                            {
                                objDieselEquipment.color_code_Issue = "#ff0000";
                            }

                            if (objDieselEquipment.Reg_Expr_Date <= TodayGap || objDieselEquipment.Reg_Expr_Date <= Today)
                            {
                                objDieselEquipment.color_code_Reg = "#ff0000";
                            }

                            objDieselEquipmentList.lstDieselEquipment.Add(objDieselEquipment);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DieselEuipmentList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DieselEuipmentList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objDieselEquipmentList.lstDieselEquipment.ToList().ToPagedList(page ?? 1, 40));
        }

        //
        // GET:/DieselEquipment/DieselEuipmentDetails

        [AllowAnonymous]
        public ActionResult DieselEuipmentDetails()
        {
            try
            {
                if (Request.QueryString["Diesel_Equip_Id"] != null && Request.QueryString["Diesel_Equip_Id"] != "")
                {
                    string sDiesel_Equip_Id = Request.QueryString["Diesel_Equip_Id"];

                    string sSqlstmt = "select Diesel_Equip_Id, Equip_No, Description, Make, Capacity, Model, Chasis_No, Engine_No, Date_Of_Purchase, Reg_Issued_Date, "
                   + " Reg_Expr_Date, LoggedBy, Equp_Status,Index_no,Plate_no,Insurance_ExpDate from t_diesel_equipment where Diesel_Equip_Id='" + sDiesel_Equip_Id + "'";

                    DataSet dsRecordsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsRecordsList.Tables.Count > 0)
                    {
                        DieselEquipmentModels objDieselEquipment = new DieselEquipmentModels
                        {
                            Diesel_Equip_Id = dsRecordsList.Tables[0].Rows[0]["Diesel_Equip_Id"].ToString(),
                            Equip_No = dsRecordsList.Tables[0].Rows[0]["Equip_No"].ToString(),
                            Description = dsRecordsList.Tables[0].Rows[0]["Description"].ToString(),
                            Make = (dsRecordsList.Tables[0].Rows[0]["Make"].ToString()),
                            Capacity = dsRecordsList.Tables[0].Rows[0]["Capacity"].ToString(),
                            Model = (dsRecordsList.Tables[0].Rows[0]["Model"].ToString()),
                            Chasis_No = dsRecordsList.Tables[0].Rows[0]["Chasis_No"].ToString(),
                            Engine_No = dsRecordsList.Tables[0].Rows[0]["Engine_No"].ToString(),
                            Equp_Status = dsRecordsList.Tables[0].Rows[0]["Equp_Status"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsRecordsList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Index_no = dsRecordsList.Tables[0].Rows[0]["Index_no"].ToString(),
                            Plate_no = dsRecordsList.Tables[0].Rows[0]["Plate_no"].ToString()
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["Date_Of_Purchase"].ToString(), out dateValue))
                        {
                            objDieselEquipment.Date_Of_Purchase = dateValue;
                        }

                        if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["Reg_Issued_Date"].ToString(), out dateValue))
                        {
                            objDieselEquipment.Reg_Issued_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["Reg_Expr_Date"].ToString(), out dateValue))
                        {
                            objDieselEquipment.Reg_Expr_Date = dateValue;
                        }
                        if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["Insurance_ExpDate"].ToString(), out dateValue))
                        {
                            objDieselEquipment.Insurance_ExpDate = dateValue;
                        }

                        return View(objDieselEquipment);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("DieselEuipmentList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Equipment Id cannot be null";
                    return RedirectToAction("DieselEuipmentList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DieselEuipmentDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("DieselEuipmentList");
        }

        //
        // GET: /DieselEquipment/DieselEuipmentEdit

        [AllowAnonymous]
        public ActionResult DieselEuipmentEdit()
        {
            try
            {
                if (Request.QueryString["Diesel_Equip_Id"] != null && Request.QueryString["Diesel_Equip_Id"] != "")
                {
                    string sDiesel_Equip_Id = Request.QueryString["Diesel_Equip_Id"];

                    string sSqlstmt = "select Diesel_Equip_Id, Equip_No, Description, Make, Capacity, Model, Chasis_No, Engine_No, Date_Of_Purchase, Reg_Issued_Date, "
                   + " Reg_Expr_Date, LoggedBy, Equp_Status,Index_no,Plate_no,Insurance_ExpDate from t_diesel_equipment where Diesel_Equip_Id='" + sDiesel_Equip_Id + "'";

                    DataSet dsRecordsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsRecordsList.Tables.Count > 0)
                    {
                        DieselEquipmentModels objDieselEquipment = new DieselEquipmentModels
                        {
                            Diesel_Equip_Id = dsRecordsList.Tables[0].Rows[0]["Diesel_Equip_Id"].ToString(),
                            Equip_No = dsRecordsList.Tables[0].Rows[0]["Equip_No"].ToString(),
                            Description = dsRecordsList.Tables[0].Rows[0]["Description"].ToString(),
                            Make = (dsRecordsList.Tables[0].Rows[0]["Make"].ToString()),
                            Capacity = dsRecordsList.Tables[0].Rows[0]["Capacity"].ToString(),
                            Model = (dsRecordsList.Tables[0].Rows[0]["Model"].ToString()),
                            Chasis_No = dsRecordsList.Tables[0].Rows[0]["Chasis_No"].ToString(),
                            Engine_No = dsRecordsList.Tables[0].Rows[0]["Engine_No"].ToString(),
                            Equp_Status = dsRecordsList.Tables[0].Rows[0]["Equp_Status"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsRecordsList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Index_no = dsRecordsList.Tables[0].Rows[0]["Index_no"].ToString(),
                            Plate_no = dsRecordsList.Tables[0].Rows[0]["Plate_no"].ToString()
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["Date_Of_Purchase"].ToString(), out dateValue))
                        {
                            objDieselEquipment.Date_Of_Purchase = dateValue;
                        }

                        if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["Reg_Issued_Date"].ToString(), out dateValue))
                        {
                            objDieselEquipment.Reg_Issued_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["Insurance_ExpDate"].ToString(), out dateValue))
                        {
                            objDieselEquipment.Insurance_ExpDate = dateValue;
                        }

                        return View(objDieselEquipment);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("DieselEuipmentList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Equipment Id cannot be null";
                    return RedirectToAction("DieselEuipmentList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DieselEuipmentEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("DieselEuipmentList");
        }

        //
        // POST: /DieselEquipment/DieselEuipmentEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DieselEuipmentEdit(DieselEquipmentModels objDieselEquipment, FormCollection form)
        {
            try
            {
                DateTime dateValue;

                if (form["Date_Of_Purchase"] != null && DateTime.TryParse(form["Date_Of_Purchase"], out dateValue) == true)
                {
                    objDieselEquipment.Date_Of_Purchase = dateValue;
                }

                if (form["Reg_Issued_Date"] != null && DateTime.TryParse(form["Reg_Issued_Date"], out dateValue) == true)
                {
                    objDieselEquipment.Reg_Issued_Date = dateValue;
                }

                if (form["Reg_Expr_Date"] != null && DateTime.TryParse(form["Reg_Expr_Date"], out dateValue) == true)
                {
                    objDieselEquipment.Reg_Expr_Date = dateValue;
                }
                if (form["Insurance_ExpDate"] != null && DateTime.TryParse(form["Insurance_ExpDate"], out dateValue) == true)
                {
                    objDieselEquipment.Insurance_ExpDate = dateValue;
                }

                if (objDieselEquipment.FunUpdateDieselEquipment(objDieselEquipment))
                {
                    TempData["Successdata"] = "Updated Euipment details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DieselEuipmentEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("DieselEuipmentList");
        }

        //
        // GET: /DieselEquipment/DieselEquipmentDelete

        [HttpPost]
        public JsonResult DieselEquipmentDelete(FormCollection form)
        {
            try
            {
                if (form["Diesel_Equip_Id"] != null && form["Diesel_Equip_Id"] != "")
                {
                    DieselEquipmentModels objEquipment = new DieselEquipmentModels();
                    string sDiesel_Equip_Id = form["Diesel_Equip_Id"];

                    if (objEquipment.FunDeleteEquipment(sDiesel_Equip_Id))
                    {
                        TempData["Successdata"] = "Vehicle / Equipment details deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in DieselEquipmentDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //
        // GET: /DieselEquipment/DieselConsumption

        [AllowAnonymous]
        public ActionResult DieselConsumption(string Diesel_Equip_IdSearch, int? page)
        {
            DieselEquipmentModels objDiesel = new DieselEquipmentModels();

            DieselConsumptionModelsList objDieselConsumptionList = new DieselConsumptionModelsList();
            objDieselConsumptionList.lstDieselConsumption = new List<DieselConsumptionModels>();

            try
            {
                ViewBag.DieselEquipId = objDiesel.GetDieselEquipmentList();
                ViewBag.CurrentQty = objDiesel.GetAvailableDieselStock();
                string sSqlstmt = "select Consumption_Id, Diesel_Equip_Id, Qty, Issued_Date, Issued_By, LoggedBy, LoggedDate from t_diesel_consumption ";

                if (Diesel_Equip_IdSearch != null && Diesel_Equip_IdSearch != "" && Diesel_Equip_IdSearch != "Select")
                {
                    ViewBag.Diesel_Equip_IdVal = Diesel_Equip_IdSearch;
                    sSqlstmt = sSqlstmt + " where Diesel_Equip_Id ='" + Diesel_Equip_IdSearch + "'";
                }

                sSqlstmt = sSqlstmt + " order by Consumption_Id desc";

                DataSet dsRecordsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRecordsList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsRecordsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            DieselConsumptionModels objDieselConsumption = new DieselConsumptionModels
                            {
                                Consumption_Id = dsRecordsList.Tables[0].Rows[i]["Consumption_Id"].ToString(),
                                Diesel_Equip_Id = objDiesel.GetDieselEquipNoById(dsRecordsList.Tables[0].Rows[i]["Diesel_Equip_Id"].ToString()),
                                Issued_By = dsRecordsList.Tables[0].Rows[i]["Issued_By"].ToString(),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsRecordsList.Tables[0].Rows[i]["LoggedBy"].ToString())
                            };

                            DateTime dateValue;
                            decimal dValue;
                            if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[i]["Issued_Date"].ToString(), out dateValue))
                            {
                                objDieselConsumption.Issued_Date = dateValue;
                            }

                            if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[i]["LoggedDate"].ToString(), out dateValue))
                            {
                                objDieselConsumption.LoggedDate = dateValue;
                            }

                            if (decimal.TryParse(dsRecordsList.Tables[0].Rows[i]["Qty"].ToString(), out dValue))
                            {
                                objDieselConsumption.Qty = dValue;
                            }

                            objDieselConsumptionList.lstDieselConsumption.Add(objDieselConsumption);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DieselConsumption: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objDieselConsumptionList.lstDieselConsumption.ToList().ToPagedList(page ?? 1, 40));
        }

        //
        // POST: /DieselEquipment/DieselConsumption

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DieselConsumption(DieselConsumptionModels objDieselConsumption, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                decimal dValue;
                if (decimal.TryParse(form["Qty"], out dValue))
                {
                    objDieselConsumption.Qty = dValue;
                }

                if (form["Issued_Date"] != null && DateTime.TryParse(form["Issued_Date"], out dateValue) == true)
                {
                    objDieselConsumption.Issued_Date = dateValue;
                }

                if (form["OperationFlg"] != null && form["OperationFlg"] == "add" && objDieselConsumption.FunAddDieselConsumption(objDieselConsumption))
                {
                    TempData["Successdata"] = "Added Diesel Consumption details successfully";
                }
                else if (form["OperationFlg"] != null && form["OperationFlg"] == "edit" && objDieselConsumption.FunUpdateDieselConsumption(objDieselConsumption))
                {
                    TempData["Successdata"] = "Updated Diesel Consumption details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DieselConsumption: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("DieselConsumption");
        }

        //
        // GET: /DieselEquipment/DieselStock

        [AllowAnonymous]
        public ActionResult DieselStock(string frmDate, string toDate, int? page)
        {
            DieselEquipmentModels objDiesel = new DieselEquipmentModels();
            DieselStockModelsList objDieselStockList = new DieselStockModelsList();
            objDieselStockList.lstDieselStock = new List<DieselStockModels>();

            try
            {
                ViewBag.Branch_id = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.CurrentQty = objDiesel.GetAvailableDieselStock();
                string sSqlstmt = "select Diesel_Rcvd_TransId, Qty, Rcvd_Date, Rcvd_By, Branch_id, LoggedBy, LoggedDate from t_diesel_rcvd_trans ";

                DateTime frmdateValue, todatevalue;

                if (frmDate != null && DateTime.TryParse(frmDate, out frmdateValue) == true &&
                    toDate != null && DateTime.TryParse(toDate, out todatevalue) == true)
                {
                    ViewBag.Diesel_Equip_IdVal = frmDate;
                    sSqlstmt = sSqlstmt + " where ( Rcvd_Date between '" + frmdateValue.ToString("yyyy/MM/dd HH:mm") + "' and '"
                        + todatevalue.ToString("yyyy/MM/dd HH:mm") + "')";
                }

                sSqlstmt = sSqlstmt + " order by Diesel_Rcvd_TransId desc";

                DataSet dsStockList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsStockList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsStockList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            DieselStockModels objDieselConsumption = new DieselStockModels
                            {
                                Diesel_Rcvd_TransId = dsStockList.Tables[0].Rows[i]["Diesel_Rcvd_TransId"].ToString(),
                                Branch_id = objGlobaldata.GetCompanyBranchNameById(dsStockList.Tables[0].Rows[i]["Branch_id"].ToString()),
                                Rcvd_By = dsStockList.Tables[0].Rows[i]["Rcvd_By"].ToString(),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsStockList.Tables[0].Rows[i]["LoggedBy"].ToString())
                            };

                            DateTime dateValue;
                            decimal dValue;
                            if (DateTime.TryParse(dsStockList.Tables[0].Rows[i]["Rcvd_Date"].ToString(), out dateValue))
                            {
                                objDieselConsumption.Rcvd_Date = dateValue;
                            }

                            if (DateTime.TryParse(dsStockList.Tables[0].Rows[i]["LoggedDate"].ToString(), out dateValue))
                            {
                                objDieselConsumption.LoggedDate = dateValue;
                            }

                            if (decimal.TryParse(dsStockList.Tables[0].Rows[i]["Qty"].ToString(), out dValue))
                            {
                                objDieselConsumption.Qty = dValue;
                            }

                            objDieselStockList.lstDieselStock.Add(objDieselConsumption);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DieselStock: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objDieselStockList.lstDieselStock.ToList().ToPagedList(page ?? 1, 40));
        }

        //
        // POST: /DieselEquipment/DieselStock

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DieselStock(DieselStockModels objDieselStock, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                decimal dValue;
                if (decimal.TryParse(form["Qty"], out dValue))
                {
                    objDieselStock.Qty = dValue;
                }

                if (form["Rcvd_Date"] != null && DateTime.TryParse(form["Rcvd_Date"], out dateValue) == true)
                {
                    objDieselStock.Rcvd_Date = dateValue;
                }

                if (form["OperationFlg"] != null && form["OperationFlg"] == "add" && objDieselStock.FunAddDieselStock(objDieselStock))
                {
                    TempData["Successdata"] = "Added Diesel Stock successfully";
                }
                else if (form["OperationFlg"] != null && form["OperationFlg"] == "edit" && objDieselStock.FunUpdateDieselStock(objDieselStock))
                {
                    TempData["Successdata"] = "Updated Diesel Stock successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DieselStock: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("DieselStock");
        }
    }
}