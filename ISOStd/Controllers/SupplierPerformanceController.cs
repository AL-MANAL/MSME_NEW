using ISOStd.Filters;
using ISOStd.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class SupplierPerformanceController : Controller
    {
        private clsGlobal objGlobalData = new clsGlobal();

        public SupplierPerformanceController()
        {
            ViewBag.Menutype = "Suppliers";
        }

        //
        // GET: /SupplierPerformance/

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public JsonResult SupplierPerformanceDelete(FormCollection form)
        {
            try
            {
                if (form["Sup_Perf_id"] != null && form["Sup_Perf_id"] != "")
                {
                    SupplierPerformanceModels Doc = new SupplierPerformanceModels();
                    string sSup_Perf_id = form["Sup_Perf_id"];

                    if (Doc.FunDeleteSupplierPerformanceDoc(sSup_Perf_id))
                    {
                        TempData["Successdata"] = "Document deleted successfully";
                        return Json("Success");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        return Json("Failed");
                    }
                }
                else
                {
                    TempData["alertdata"] = "SupplierPerformanceDoc Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        // GET: /SupplierPerformance/AddSupplierPerformance

        [AllowAnonymous]
        public ActionResult AddSupplierPerformance()
        {
            ViewBag.SubMenutype = "SupplierPerformance";
            ViewBag.SuppList = objGlobalData.GetSupplierList();
            ViewBag.EvalToYear = objGlobalData.GetDropdownList("Years");
            ViewBag.HSE = objGlobalData.GetConstantValue("YesNo");
            ViewBag.ISO = objGlobalData.GetConstantValue("YesNo");
            ViewBag.Env = objGlobalData.GetConstantValue("YesNo");
            ViewBag.EmpList = objGlobalData.GetHrEmployeeListbox();
            ViewBag.Rating = objGlobalData.GetConstantValue("SupplierRating");
            return View();
        }

        // POST: /SupplierPerformance/AddSupplierPerformance

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSupplierPerformance(SupplierPerformanceModels objSupplierPerformance, FormCollection form)
        {
            try
            {
                if (objSupplierPerformance != null)
                {
                    DateTime dateValue;

                    if (DateTime.TryParse(form["Eval_FromDate"], out dateValue) == true)
                    {
                        objSupplierPerformance.Eval_FromDate = dateValue;
                    }
                    //if (DateTime.TryParse(form["Eval_ToDate"], out dateValue) == true)
                    //{
                    //    objSupplierPerformance.Eval_ToDate = dateValue;
                    //}

                    objSupplierPerformance.Eval_ToDate = form["Eval_ToDate"];

                    if (objSupplierPerformance.FunAddSupplierPerf(objSupplierPerformance))
                    {
                        TempData["Successdata"] = "Added Supplier performance details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in AddSupplierPerformance: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SupplierPerformanceList");
        }

        public ActionResult DisplaySupplierPerformance()
        {
            try
            {
                ViewBag.SubMenutype = "SupplierPerformance";

                SupplierPerformanceModels objSupplierPerformance = new SupplierPerformanceModels();

                string sqlstmt = "select item_id,item_desc,item_fulldesc from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                                    + "and header_desc='Supplier Performance Criteria' order by item_desc desc";
                DataSet dsItems = objGlobalData.Getdetails(sqlstmt);
                ViewBag.dsItems = dsItems;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DisplaySupplierPerformance: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        // GET: /SupplierPerformance/SupplierPerformanceList

        [AllowAnonymous]
        public ActionResult SupplierPerformanceList(string chkAll, string SupplierId, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "SupplierPerformance";

            SupplierPerformanceModelsList objSupplierPerformanceModelsList = new SupplierPerformanceModelsList();
            objSupplierPerformanceModelsList.SupplierPerformanceMList = new List<SupplierPerformanceModels>();

            try
            {
                ViewBag.SupplierList = objGlobalData.GetSupplierList();
                ViewBag.EvalToYear = objGlobalData.GetDropdownList("Years");
                string sBranch_name = objGlobalData.GetCurrentUserSession().division;
                string sBranchtree = objGlobalData.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobalData.GetMultiBranchListByID(sBranchtree);

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                string sSqlstmt = "select Sup_Perf_id,Eval_FromDate,Eval_ToDate,Supplier_Name,Scope_OfSupplies,total_delv,accept_delv"
                + ",ontime_delv,lowest_price,supp_price,SHE_total from t_supplier_performance where Active=1";
                string sSearchtext = "";

                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;
                    if (SupplierId != null && SupplierId != "Select")
                    {
                        ViewBag.SupplierId = SupplierId;
                        sSearchtext = sSearchtext + " and (Supplier_Name ='" + SupplierId + "')";
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Supplier_Name";
                DataSet dsSupplierPerformanceModels = objGlobalData.Getdetails(sSqlstmt);

                string sSqlstmts = "select item_id as Id, item_desc as Name,item_fulldesc as perc from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id"
                + " and header_desc='Supplier Performance Criteria' order by item_desc asc";
                DataSet dsCriteria = objGlobalData.Getdetails(sSqlstmts);
                SupplierPerformanceModelsList Suplist = new SupplierPerformanceModelsList();
                Suplist.SupplierPerformanceMList = new List<SupplierPerformanceModels>();
                if (dsCriteria.Tables.Count > 0 && dsCriteria.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCriteria.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            SupplierPerformanceModels objsup = new SupplierPerformanceModels
                            {
                                Criteria = (dsCriteria.Tables[0].Rows[i]["Name"].ToString()),
                                perc = (dsCriteria.Tables[0].Rows[i]["perc"].ToString()),
                            };
                            Suplist.SupplierPerformanceMList.Add(objsup);
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceList: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
                int perPrice = 1, perQuality = 1, perService = 1, perSHE = 1;
                if (Suplist.SupplierPerformanceMList[0].Criteria == "PRICE" && Suplist.SupplierPerformanceMList[0].perc != "")
                {
                    perPrice = Convert.ToInt32(Suplist.SupplierPerformanceMList[0].perc);
                }
                if (Suplist.SupplierPerformanceMList[1].Criteria == "QUALITY" && Suplist.SupplierPerformanceMList[1].perc != "")
                {
                    perQuality = Convert.ToInt32(Suplist.SupplierPerformanceMList[1].perc);
                }
                if (Suplist.SupplierPerformanceMList[2].Criteria == "SERVICE" && Suplist.SupplierPerformanceMList[2].perc != "")
                {
                    perService = Convert.ToInt32(Suplist.SupplierPerformanceMList[2].perc);
                }
                if (Suplist.SupplierPerformanceMList[3].Criteria == "SHE Compliance" && Suplist.SupplierPerformanceMList[3].perc != "")
                {
                    perSHE = Convert.ToInt32(Suplist.SupplierPerformanceMList[3].perc);
                }
                if (dsSupplierPerformanceModels.Tables.Count > 0 && dsSupplierPerformanceModels.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsSupplierPerformanceModels.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            decimal stotal_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["total_delv"].ToString());
                            decimal saccept_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["accept_delv"].ToString());
                            decimal sontime_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["ontime_delv"].ToString());
                            decimal slowest_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["lowest_price"].ToString());
                            decimal ssupp_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["supp_price"].ToString());
                            decimal sSHE_total = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["SHE_total"].ToString());

                            decimal Price = 0, Quality = 0, Service = 0, SHE = 0;
                            if (slowest_price != 0 && perPrice != 1)
                            {
                                Price = (slowest_price * perPrice) / ssupp_price;
                            }
                            if (saccept_delv != 0 && perQuality != 1)
                            {
                                Quality = (saccept_delv * perQuality) / stotal_delv;
                            }
                            if (sontime_delv != 0 && perService != 1)
                            {
                                Service = (sontime_delv * perService) / stotal_delv;
                            }
                            if (sSHE_total != 0 && perSHE != 1)
                            {
                                SHE = (sSHE_total * perSHE) / stotal_delv;
                            }

                            decimal srating = Price + Quality + Service + SHE;
                            SupplierPerformanceModels objSupplierPerformanceModels = new SupplierPerformanceModels
                            {
                                Sup_Perf_id = Convert.ToInt16(dsSupplierPerformanceModels.Tables[0].Rows[i]["Sup_Perf_id"].ToString()),
                                Supplier_Name = objGlobalData.GetSupplierNameById(dsSupplierPerformanceModels.Tables[0].Rows[i]["Supplier_Name"].ToString()),
                                Scope_OfSupplies = dsSupplierPerformanceModels.Tables[0].Rows[i]["Scope_OfSupplies"].ToString(),
                                total_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["total_delv"].ToString()),
                                accept_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["accept_delv"].ToString()),
                                ontime_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["ontime_delv"].ToString()),
                                lowest_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["lowest_price"].ToString()),
                                supp_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["supp_price"].ToString()),
                                SHE_total = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["SHE_total"].ToString()),
                                Eval_ToDate = objGlobalData.GetDropdownitemById(dsSupplierPerformanceModels.Tables[0].Rows[i]["Eval_ToDate"].ToString()),
                                PRICE = System.Math.Round(Price, 2),
                                QUALITY = System.Math.Round(Quality, 2),
                                SERVICE = System.Math.Round(Service, 2),
                                SHE = System.Math.Round(SHE, 2),
                                rating = System.Math.Round(srating, 2),
                            };
                            DateTime dateValue;
                            if (DateTime.TryParse(dsSupplierPerformanceModels.Tables[0].Rows[i]["Eval_FromDate"].ToString(), out dateValue))
                            {
                                objSupplierPerformanceModels.Eval_FromDate = dateValue;
                            }
                            //if (DateTime.TryParse(dsSupplierPerformanceModels.Tables[0].Rows[i]["Eval_ToDate"].ToString(), out dateValue))
                            //{
                            //    objSupplierPerformanceModels.Eval_ToDate = dateValue;
                            //}
                            objSupplierPerformanceModels.Action_sup = objGlobalData.GetSupplierPerfAction(objSupplierPerformanceModels.rating);
                            objSupplierPerformanceModelsList.SupplierPerformanceMList.Add(objSupplierPerformanceModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceList: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceList: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return View(objSupplierPerformanceModelsList.SupplierPerformanceMList.ToList());
        }

        [AllowAnonymous]
        public JsonResult SupplierPerformanceListSearch(string chkAll, string SupplierId, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "SupplierPerformance";

            SupplierPerformanceModelsList objSupplierPerformanceModelsList = new SupplierPerformanceModelsList();
            objSupplierPerformanceModelsList.SupplierPerformanceMList = new List<SupplierPerformanceModels>();

            try
            {
                ViewBag.SupplierList = objGlobalData.GetSupplierList();
                ViewBag.EvalToYear = objGlobalData.GetDropdownList("Years");
                string sBranch_name = objGlobalData.GetCurrentUserSession().division;
                string sBranchtree = objGlobalData.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobalData.GetMultiBranchListByID(sBranchtree);

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                string sSqlstmt = "select Sup_Perf_id,Eval_FromDate,Eval_ToDate,Supplier_Name,Scope_OfSupplies,total_delv,accept_delv"
                + ",ontime_delv,lowest_price,supp_price,SHE_total from t_supplier_performance where Active=1";
                string sSearchtext = "";

                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;
                    if (SupplierId != null && SupplierId != "Select")
                    {
                        ViewBag.SupplierId = SupplierId;
                        sSearchtext = sSearchtext + " and (Supplier_Name ='" + SupplierId + "')";
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Supplier_Name";
                DataSet dsSupplierPerformanceModels = objGlobalData.Getdetails(sSqlstmt);

                string sSqlstmts = "select item_id as Id, item_desc as Name,item_fulldesc as perc from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id"
                + " and header_desc='Supplier Performance Criteria' order by item_desc asc";
                DataSet dsCriteria = objGlobalData.Getdetails(sSqlstmts);
                SupplierPerformanceModelsList Suplist = new SupplierPerformanceModelsList();
                Suplist.SupplierPerformanceMList = new List<SupplierPerformanceModels>();
                if (dsCriteria.Tables.Count > 0 && dsCriteria.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCriteria.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            SupplierPerformanceModels objsup = new SupplierPerformanceModels
                            {
                                Criteria = (dsCriteria.Tables[0].Rows[i]["Name"].ToString()),
                                perc = (dsCriteria.Tables[0].Rows[i]["perc"].ToString()),
                            };
                            Suplist.SupplierPerformanceMList.Add(objsup);
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
                int perPrice = 1, perQuality = 1, perService = 1, perSHE = 1;
                if (Suplist.SupplierPerformanceMList[0].Criteria == "PRICE" && Suplist.SupplierPerformanceMList[0].perc != "")
                {
                    perPrice = Convert.ToInt32(Suplist.SupplierPerformanceMList[0].perc);
                }
                if (Suplist.SupplierPerformanceMList[1].Criteria == "QUALITY" && Suplist.SupplierPerformanceMList[1].perc != "")
                {
                    perQuality = Convert.ToInt32(Suplist.SupplierPerformanceMList[1].perc);
                }
                if (Suplist.SupplierPerformanceMList[2].Criteria == "SERVICE" && Suplist.SupplierPerformanceMList[2].perc != "")
                {
                    perService = Convert.ToInt32(Suplist.SupplierPerformanceMList[2].perc);
                }
                if (Suplist.SupplierPerformanceMList[3].Criteria == "SHE Compliance" && Suplist.SupplierPerformanceMList[3].perc != "")
                {
                    perSHE = Convert.ToInt32(Suplist.SupplierPerformanceMList[3].perc);
                }
                if (dsSupplierPerformanceModels.Tables.Count > 0 && dsSupplierPerformanceModels.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsSupplierPerformanceModels.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            decimal stotal_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["total_delv"].ToString());
                            decimal saccept_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["accept_delv"].ToString());
                            decimal sontime_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["ontime_delv"].ToString());
                            decimal slowest_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["lowest_price"].ToString());
                            decimal ssupp_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["supp_price"].ToString());
                            decimal sSHE_total = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["SHE_total"].ToString());

                            decimal Price = 0, Quality = 0, Service = 0, SHE = 0;
                            if (slowest_price != 0 && perPrice != 1)
                            {
                                Price = (slowest_price * perPrice) / ssupp_price;
                            }
                            if (saccept_delv != 0 && perQuality != 1)
                            {
                                Quality = (saccept_delv * perQuality) / stotal_delv;
                            }
                            if (sontime_delv != 0 && perService != 1)
                            {
                                Service = (sontime_delv * perService) / stotal_delv;
                            }
                            if (sSHE_total != 0 && perSHE != 1)
                            {
                                SHE = (sSHE_total * perSHE) / stotal_delv;
                            }

                            decimal srating = Price + Quality + Service + SHE;
                            SupplierPerformanceModels objSupplierPerformanceModels = new SupplierPerformanceModels
                            {
                                Sup_Perf_id = Convert.ToInt16(dsSupplierPerformanceModels.Tables[0].Rows[i]["Sup_Perf_id"].ToString()),
                                Supplier_Name = objGlobalData.GetSupplierNameById(dsSupplierPerformanceModels.Tables[0].Rows[i]["Supplier_Name"].ToString()),
                                Scope_OfSupplies = dsSupplierPerformanceModels.Tables[0].Rows[i]["Scope_OfSupplies"].ToString(),
                                total_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["total_delv"].ToString()),
                                accept_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["accept_delv"].ToString()),
                                ontime_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["ontime_delv"].ToString()),
                                lowest_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["lowest_price"].ToString()),
                                supp_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["supp_price"].ToString()),
                                SHE_total = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[i]["SHE_total"].ToString()),
                                Eval_ToDate = objGlobalData.GetDropdownitemById(dsSupplierPerformanceModels.Tables[0].Rows[i]["Eval_ToDate"].ToString()),
                                PRICE = System.Math.Round(Price, 2),
                                QUALITY = System.Math.Round(Quality, 2),
                                SERVICE = System.Math.Round(Service, 2),
                                SHE = System.Math.Round(SHE, 2),
                                rating = System.Math.Round(srating, 2),
                            };
                            DateTime dateValue;
                            if (DateTime.TryParse(dsSupplierPerformanceModels.Tables[0].Rows[i]["Eval_FromDate"].ToString(), out dateValue))
                            {
                                objSupplierPerformanceModels.Eval_FromDate = dateValue;
                            }
                            //if (DateTime.TryParse(dsSupplierPerformanceModels.Tables[0].Rows[i]["Eval_ToDate"].ToString(), out dateValue))
                            //{
                            //    objSupplierPerformanceModels.Eval_ToDate = dateValue;
                            //}
                            objSupplierPerformanceModels.Action_sup = objGlobalData.GetSupplierPerfAction(objSupplierPerformanceModels.rating);
                            objSupplierPerformanceModelsList.SupplierPerformanceMList.Add(objSupplierPerformanceModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        // GET: /SupplierPerformance/SupplierPerformanceDetails

        [AllowAnonymous]
        public ActionResult SupplierPerformanceDetails()
        {
            ViewBag.SubMenutype = "SupplierPerformance";

            SupplierPerformanceModels objSupplierPerformanceModels = new SupplierPerformanceModels();
            try
            {
                if (Request.QueryString["Sup_Perf_id"] != null && Request.QueryString["Sup_Perf_id"] != "")
                {
                    string sSup_Perf_id = Request.QueryString["Sup_Perf_id"];
                    string sSqlstmt = "select Sup_Perf_id,Eval_FromDate,Eval_ToDate,Supplier_Name,Scope_OfSupplies,total_delv,accept_delv"
                     + ",ontime_delv,lowest_price,supp_price,SHE_total,hse_compliance,iso9001_compliance,recomend_by,payment_terms,sale_perf,EnvMgmt from t_supplier_performance where Sup_Perf_id='" + sSup_Perf_id + "' ";
                    sSqlstmt = sSqlstmt + " order by Sup_Perf_id desc";
                    DataSet dsSupplierPerformanceModels = objGlobalData.Getdetails(sSqlstmt);

                    string sSqlstmts = "select item_id as Id, item_desc as Name,item_fulldesc as perc from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id"
                    + " and header_desc='Supplier Performance Criteria' order by item_desc asc";
                    DataSet dsCriteria = objGlobalData.Getdetails(sSqlstmts);
                    SupplierPerformanceModelsList Suplist = new SupplierPerformanceModelsList();
                    Suplist.SupplierPerformanceMList = new List<SupplierPerformanceModels>();
                    if (dsCriteria.Tables.Count > 0 && dsCriteria.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsCriteria.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                SupplierPerformanceModels objsup = new SupplierPerformanceModels
                                {
                                    Criteria = (dsCriteria.Tables[0].Rows[i]["Name"].ToString()),
                                    perc = (dsCriteria.Tables[0].Rows[i]["perc"].ToString()),
                                };
                                Suplist.SupplierPerformanceMList.Add(objsup);
                            }
                            catch (Exception ex)
                            {
                                objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceList: " + ex.ToString());
                                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    int perPrice = 1, perQuality = 1, perService = 1, perSHE = 1;
                    if (Suplist.SupplierPerformanceMList[0].Criteria == "PRICE" && Suplist.SupplierPerformanceMList[0].perc != "")
                    {
                        perPrice = Convert.ToInt32(Suplist.SupplierPerformanceMList[0].perc);
                    }
                    if (Suplist.SupplierPerformanceMList[1].Criteria == "QUALITY" && Suplist.SupplierPerformanceMList[1].perc != "")
                    {
                        perQuality = Convert.ToInt32(Suplist.SupplierPerformanceMList[1].perc);
                    }
                    if (Suplist.SupplierPerformanceMList[2].Criteria == "SERVICE" && Suplist.SupplierPerformanceMList[2].perc != "")
                    {
                        perService = Convert.ToInt32(Suplist.SupplierPerformanceMList[2].perc);
                    }
                    if (Suplist.SupplierPerformanceMList[3].Criteria == "SHE Compliance" && Suplist.SupplierPerformanceMList[3].perc != "")
                    {
                        perSHE = Convert.ToInt32(Suplist.SupplierPerformanceMList[3].perc);
                    }
                    if (dsSupplierPerformanceModels.Tables.Count > 0 && dsSupplierPerformanceModels.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            decimal stotal_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["total_delv"].ToString());
                            decimal saccept_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["accept_delv"].ToString());
                            decimal sontime_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["ontime_delv"].ToString());
                            decimal slowest_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["lowest_price"].ToString());
                            decimal ssupp_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["supp_price"].ToString());
                            decimal sSHE_total = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["SHE_total"].ToString());

                            decimal Price = 0, Quality = 0, Service = 0, SHE = 0;
                            if (slowest_price != 0 && perPrice != 1)
                            {
                                Price = (slowest_price * perPrice) / ssupp_price;
                            }
                            if (saccept_delv != 0 && perQuality != 1)
                            {
                                Quality = (saccept_delv * perQuality) / stotal_delv;
                            }
                            if (sontime_delv != 0 && perService != 1)
                            {
                                Service = (sontime_delv * perService) / stotal_delv;
                            }
                            if (sSHE_total != 0 && perSHE != 1)
                            {
                                SHE = (sSHE_total * perSHE) / stotal_delv;
                            }
                            decimal srating = Price + Quality + Service + SHE;
                            objSupplierPerformanceModels = new SupplierPerformanceModels
                            {
                                Sup_Perf_id = Convert.ToInt16(dsSupplierPerformanceModels.Tables[0].Rows[0]["Sup_Perf_id"].ToString()),
                                Supplier_Name = objGlobalData.GetSupplierNameById(dsSupplierPerformanceModels.Tables[0].Rows[0]["Supplier_Name"].ToString()),
                                Scope_OfSupplies = dsSupplierPerformanceModels.Tables[0].Rows[0]["Scope_OfSupplies"].ToString(),
                                total_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["total_delv"].ToString()),
                                accept_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["accept_delv"].ToString()),
                                ontime_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["ontime_delv"].ToString()),
                                lowest_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["lowest_price"].ToString()),
                                supp_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["supp_price"].ToString()),
                                SHE_total = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["SHE_total"].ToString()),
                                Eval_ToDate = objGlobalData.GetDropdownitemById(dsSupplierPerformanceModels.Tables[0].Rows[0]["Eval_ToDate"].ToString()),
                                PRICE = System.Math.Round(Price, 2),
                                QUALITY = System.Math.Round(Quality, 2),
                                SERVICE = System.Math.Round(Service, 2),
                                SHE = System.Math.Round(SHE, 2),
                                rating = System.Math.Round(srating, 2),
                                hse_compliance = dsSupplierPerformanceModels.Tables[0].Rows[0]["hse_compliance"].ToString(),
                                iso9001_compliance = dsSupplierPerformanceModels.Tables[0].Rows[0]["iso9001_compliance"].ToString(),
                                recomend_by = objGlobalData.GetEmpHrNameById(dsSupplierPerformanceModels.Tables[0].Rows[0]["recomend_by"].ToString()),
                                payment_terms = dsSupplierPerformanceModels.Tables[0].Rows[0]["payment_terms"].ToString(),
                                sale_perf = dsSupplierPerformanceModels.Tables[0].Rows[0]["sale_perf"].ToString(),
                                EnvMgmt = dsSupplierPerformanceModels.Tables[0].Rows[0]["EnvMgmt"].ToString(),
                            };
                            DateTime dateValue;
                            if (DateTime.TryParse(dsSupplierPerformanceModels.Tables[0].Rows[0]["Eval_FromDate"].ToString(), out dateValue))
                            {
                                objSupplierPerformanceModels.Eval_FromDate = dateValue;
                            }
                            //if (DateTime.TryParse(dsSupplierPerformanceModels.Tables[0].Rows[0]["Eval_ToDate"].ToString(), out dateValue))
                            //{
                            //    objSupplierPerformanceModels.Eval_ToDate = dateValue;
                            //}
                            objSupplierPerformanceModels.Action_sup = objGlobalData.GetSupplierPerfAction(objSupplierPerformanceModels.rating);
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceList: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("SupplierPerformanceList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "SupID cannot be Null or empty";
                    return RedirectToAction("SupplierPerformanceList");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceList: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return View(objSupplierPerformanceModels);
        }

        [AllowAnonymous]
        public ActionResult SupplierPerformanceInfo(int id)
        {
            ViewBag.SubMenutype = "SupplierPerformance";

            SupplierPerformanceModels objSupplierPerformanceModels = new SupplierPerformanceModels();
            try
            {
                if (Request.QueryString["id"] != null && Request.QueryString["id"] != "")
                {
                    string sSup_Perf_id = Request.QueryString["Sup_Perf_id"];
                    string sSqlstmt = "select Sup_Perf_id,Eval_FromDate,Eval_ToDate,Supplier_Name,Scope_OfSupplies,total_delv,accept_delv"
                     + ",ontime_delv,lowest_price,supp_price,SHE_total from t_supplier_performance where Sup_Perf_id='" + id + "' ";
                    sSqlstmt = sSqlstmt + " order by Sup_Perf_id desc";
                    DataSet dsSupplierPerformanceModels = objGlobalData.Getdetails(sSqlstmt);

                    string sSqlstmts = "select item_id as Id, item_desc as Name,item_fulldesc as perc from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id"
                    + " and header_desc='Supplier Performance Criteria' order by item_desc asc";
                    DataSet dsCriteria = objGlobalData.Getdetails(sSqlstmts);
                    SupplierPerformanceModelsList Suplist = new SupplierPerformanceModelsList();
                    Suplist.SupplierPerformanceMList = new List<SupplierPerformanceModels>();
                    if (dsCriteria.Tables.Count > 0 && dsCriteria.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsCriteria.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                SupplierPerformanceModels objsup = new SupplierPerformanceModels
                                {
                                    Criteria = (dsCriteria.Tables[0].Rows[i]["Name"].ToString()),
                                    perc = (dsCriteria.Tables[0].Rows[i]["perc"].ToString()),
                                };
                                Suplist.SupplierPerformanceMList.Add(objsup);
                            }
                            catch (Exception ex)
                            {
                                objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceList: " + ex.ToString());
                                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    int perPrice = 1, perQuality = 1, perService = 1, perSHE = 1;
                    if (Suplist.SupplierPerformanceMList[0].Criteria == "PRICE" && Suplist.SupplierPerformanceMList[0].perc != "")
                    {
                        perPrice = Convert.ToInt32(Suplist.SupplierPerformanceMList[0].perc);
                    }
                    if (Suplist.SupplierPerformanceMList[1].Criteria == "QUALITY" && Suplist.SupplierPerformanceMList[1].perc != "")
                    {
                        perQuality = Convert.ToInt32(Suplist.SupplierPerformanceMList[1].perc);
                    }
                    if (Suplist.SupplierPerformanceMList[2].Criteria == "SERVICE" && Suplist.SupplierPerformanceMList[2].perc != "")
                    {
                        perService = Convert.ToInt32(Suplist.SupplierPerformanceMList[2].perc);
                    }
                    if (Suplist.SupplierPerformanceMList[3].Criteria == "SHE Compliance" && Suplist.SupplierPerformanceMList[3].perc != "")
                    {
                        perSHE = Convert.ToInt32(Suplist.SupplierPerformanceMList[3].perc);
                    }
                    if (dsSupplierPerformanceModels.Tables.Count > 0 && dsSupplierPerformanceModels.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            decimal stotal_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["total_delv"].ToString());
                            decimal saccept_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["accept_delv"].ToString());
                            decimal sontime_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["ontime_delv"].ToString());
                            decimal slowest_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["lowest_price"].ToString());
                            decimal ssupp_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["supp_price"].ToString());
                            decimal sSHE_total = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["SHE_total"].ToString());

                            decimal Price = 0, Quality = 0, Service = 0, SHE = 0;
                            if (slowest_price != 0 && perPrice != 1)
                            {
                                Price = (slowest_price * perPrice) / ssupp_price;
                            }
                            if (saccept_delv != 0 && perQuality != 1)
                            {
                                Quality = (saccept_delv * perQuality) / stotal_delv;
                            }
                            if (sontime_delv != 0 && perService != 1)
                            {
                                Service = (sontime_delv * perService) / stotal_delv;
                            }
                            if (sSHE_total != 0 && perSHE != 1)
                            {
                                SHE = (sSHE_total * perSHE) / stotal_delv;
                            }
                            decimal srating = Price + Quality + Service + SHE;
                            objSupplierPerformanceModels = new SupplierPerformanceModels
                            {
                                Sup_Perf_id = Convert.ToInt16(dsSupplierPerformanceModels.Tables[0].Rows[0]["Sup_Perf_id"].ToString()),
                                Supplier_Name = objGlobalData.GetSupplierNameById(dsSupplierPerformanceModels.Tables[0].Rows[0]["Supplier_Name"].ToString()),
                                Scope_OfSupplies = dsSupplierPerformanceModels.Tables[0].Rows[0]["Scope_OfSupplies"].ToString(),
                                total_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["total_delv"].ToString()),
                                accept_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["accept_delv"].ToString()),
                                ontime_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["ontime_delv"].ToString()),
                                lowest_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["lowest_price"].ToString()),
                                supp_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["supp_price"].ToString()),
                                SHE_total = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["SHE_total"].ToString()),
                                Eval_ToDate = objGlobalData.GetDropdownitemById(dsSupplierPerformanceModels.Tables[0].Rows[0]["Eval_ToDate"].ToString()),
                                PRICE = System.Math.Round(Price, 2),
                                QUALITY = System.Math.Round(Quality, 2),
                                SERVICE = System.Math.Round(Service, 2),
                                SHE = System.Math.Round(SHE, 2),
                                rating = System.Math.Round(srating, 2),
                            };
                            DateTime dateValue;
                            if (DateTime.TryParse(dsSupplierPerformanceModels.Tables[0].Rows[0]["Eval_FromDate"].ToString(), out dateValue))
                            {
                                objSupplierPerformanceModels.Eval_FromDate = dateValue;
                            }
                            //if (DateTime.TryParse(dsSupplierPerformanceModels.Tables[0].Rows[0]["Eval_ToDate"].ToString(), out dateValue))
                            //{
                            //    objSupplierPerformanceModels.Eval_ToDate = dateValue;
                            //}
                            objSupplierPerformanceModels.Action_sup = objGlobalData.GetSupplierPerfAction(objSupplierPerformanceModels.rating);
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceInfo: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("SupplierPerformanceList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "SupID cannot be Null or empty";
                    return RedirectToAction("SupplierPerformanceList");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceList: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return View(objSupplierPerformanceModels);
        }

        // GET: /SupplierPerformance/SupplierPerformanceEdit

        [AllowAnonymous]
        public ActionResult SupplierPerformanceEdit()
        {
            ViewBag.SubMenutype = "SupplierPerformance";

            string sSup_Perf_id = Request.QueryString["Sup_Perf_id"];
            SupplierPerformanceModels objSupplierPerformanceModels = new SupplierPerformanceModels();

            try
            {
                ViewBag.HSE = objGlobalData.GetConstantValue("YesNo");
                ViewBag.ISO = objGlobalData.GetConstantValue("YesNo");
                ViewBag.EmpList = objGlobalData.GetHrEmployeeListbox();
                ViewBag.Env = objGlobalData.GetConstantValue("YesNo");
                ViewBag.Rating = objGlobalData.GetConstantValue("SupplierRating");
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                string sSqlstmt = "select Sup_Perf_id,Eval_FromDate,Eval_ToDate,Supplier_Name,Scope_OfSupplies,total_delv,accept_delv"
                + ",ontime_delv,lowest_price,supp_price,SHE_total,hse_compliance,iso9001_compliance,recomend_by,payment_terms,sale_perf,EnvMgmt from t_supplier_performance  where Sup_Perf_id='" + sSup_Perf_id + "' order by Sup_Perf_id desc";

                DataSet dsSupplierPerformanceModels = objGlobalData.Getdetails(sSqlstmt);

                if (dsSupplierPerformanceModels.Tables.Count > 0 && dsSupplierPerformanceModels.Tables[0].Rows.Count > 0)
                {
                    objSupplierPerformanceModels = new SupplierPerformanceModels
                    {
                        Sup_Perf_id = Convert.ToInt16(dsSupplierPerformanceModels.Tables[0].Rows[0]["Sup_Perf_id"].ToString()),
                        Supplier_Name = objGlobalData.GetSupplierNameById(dsSupplierPerformanceModels.Tables[0].Rows[0]["Supplier_Name"].ToString()),
                        Scope_OfSupplies = dsSupplierPerformanceModels.Tables[0].Rows[0]["Scope_OfSupplies"].ToString(),
                        total_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["total_delv"].ToString()),
                        accept_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["accept_delv"].ToString()),
                        ontime_delv = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["ontime_delv"].ToString()),
                        lowest_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["lowest_price"].ToString()),
                        supp_price = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["supp_price"].ToString()),
                        SHE_total = Convert.ToDecimal(dsSupplierPerformanceModels.Tables[0].Rows[0]["SHE_total"].ToString()),
                        Eval_ToDate = objGlobalData.GetDropdownitemById(dsSupplierPerformanceModels.Tables[0].Rows[0]["Eval_ToDate"].ToString()),
                        hse_compliance = dsSupplierPerformanceModels.Tables[0].Rows[0]["hse_compliance"].ToString(),
                        iso9001_compliance = dsSupplierPerformanceModels.Tables[0].Rows[0]["iso9001_compliance"].ToString(),
                        recomend_by = dsSupplierPerformanceModels.Tables[0].Rows[0]["recomend_by"].ToString(),
                        payment_terms = dsSupplierPerformanceModels.Tables[0].Rows[0]["payment_terms"].ToString(),
                        sale_perf = dsSupplierPerformanceModels.Tables[0].Rows[0]["sale_perf"].ToString(),
                        EnvMgmt = dsSupplierPerformanceModels.Tables[0].Rows[0]["EnvMgmt"].ToString(),
                    };
                    DateTime dateValue;
                    if (DateTime.TryParse(dsSupplierPerformanceModels.Tables[0].Rows[0]["Eval_FromDate"].ToString(), out dateValue))
                    {
                        objSupplierPerformanceModels.Eval_FromDate = dateValue;
                    }
                    //if (DateTime.TryParse(dsSupplierPerformanceModels.Tables[0].Rows[0]["Eval_ToDate"].ToString(), out dateValue))
                    //{
                    //    objSupplierPerformanceModels.Eval_ToDate = dateValue;
                    //}
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            ViewBag.Sup_Perf_id = sSup_Perf_id;
            ViewBag.SuppList = objGlobalData.GetSupplierList();
            ViewBag.EvalToYear = objGlobalData.GetDropdownList("Years");
            return View(objSupplierPerformanceModels);
        }

        // POST: /SupplierPerformance/SupplierPerformanceEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierPerformanceEdit(SupplierPerformanceModels objSupplierPerformance, FormCollection form)
        {
            try
            {
                if (objSupplierPerformance != null)
                {
                    objSupplierPerformance.Sup_Perf_id = Convert.ToInt16(form["Sup_Perf_id"]);
                    objSupplierPerformance.Supplier_Name = form["Supplier_Name"];
                    objSupplierPerformance.Eval_ToDate = form["Eval_ToDate"];
                    DateTime dateValue;

                    if (DateTime.TryParse(form["Eval_FromDate"], out dateValue) == true)
                    {
                        objSupplierPerformance.Eval_FromDate = dateValue;
                    }
                    //if (DateTime.TryParse(form["Eval_ToDate"], out dateValue) == true)
                    //{
                    //    objSupplierPerformance.Eval_ToDate = dateValue;
                    //}

                    if (objSupplierPerformance.FunUpdateSupplierPerf(objSupplierPerformance))
                    {
                        TempData["Successdata"] = "Supplier performance details updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SupplierPerformanceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SupplierPerformanceList");
        }
    }
}