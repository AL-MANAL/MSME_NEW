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
    public class ExtProviderPerformanceController : Controller
    {
        private clsGlobal objGlobalData = new clsGlobal();

        public ExtProviderPerformanceController()
        {
            ViewBag.Menutype = "Suppliers";
            ViewBag.SubMenutype = "ExtProviderPerformance";
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET: /ExtProviderPerformance/AddExtProviderPerformance

        [AllowAnonymous]
        public ActionResult AddExtProviderPerformance()
        {
            ExtProviderPerformanceModels objExtProviderList = new ExtProviderPerformanceModels();

            ViewBag.SubMenutype = "ExtProviderPerformance";

            objExtProviderList.branch = objGlobalData.GetCurrentUserSession().division;
            objExtProviderList.Department = objGlobalData.GetCurrentUserSession().DeptID;
            objExtProviderList.Location = objGlobalData.GetCurrentUserSession().Work_Location;

            // ViewBag.EmpList = objGlobalData.GetGEmpListBymulitBDL(objExtProviderList.branch, objExtProviderList.Department, objExtProviderList.Location);
            // ViewBag.Approver = objGlobalData.GetGRoleList(objExtProviderList.branch, objExtProviderList.Department, objExtProviderList.Location, "Approver");

            ViewBag.Branch = objGlobalData.GetCompanyBranchListbox();
            ViewBag.Department = objGlobalData.GetDepartmentListbox(objExtProviderList.branch);
            ViewBag.Location = objGlobalData.GetDivisionLocationList(objExtProviderList.branch);

            ViewBag.SuppList = objGlobalData.GetSupplierList();

            ViewBag.EmpList = objGlobalData.GetHrEmployeeListbox();
            ViewBag.Approver = objGlobalData.GetApprover();
            ViewBag.Priority = objGlobalData.GetDropdownList("ExtProvider Performance Priority");
            //ViewBag.Rating = objGlobalData.GetConstantValue("ExtProviderRating");
            return View(objExtProviderList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExtProviderPerformance(ExtProviderPerformanceModels objPerfModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                ViewBag.SubMenutype = "ExtProviderPerformance";
                objPerfModel.branch = form["branch"];
                objPerfModel.Department = form["Department"];
                objPerfModel.Location = form["Location"];

                if (objPerfModel != null)
                {
                    DateTime dateValue;

                    if (DateTime.TryParse(form["Eval_Date"], out dateValue) == true)
                    {
                        objPerfModel.Eval_Date = dateValue;
                    }
                    if (DateTime.TryParse(form["Eval_FromDate"], out dateValue) == true)
                    {
                        objPerfModel.Eval_FromDate = dateValue;
                    }
                    if (DateTime.TryParse(form["Eval_ToDate"], out dateValue) == true)
                    {
                        objPerfModel.Eval_ToDate = dateValue;
                    }

                    //notified_to
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                    {
                        if (form["nempno " + i] != "" && form["nempno " + i] != null)
                        {
                            objPerfModel.notified_to = form["nempno " + i] + "," + objPerfModel.notified_to;
                        }
                    }
                    if (objPerfModel.notified_to != null)
                    {
                        objPerfModel.notified_to = objPerfModel.notified_to.Trim(',');
                    }

                    //upload
                    HttpPostedFileBase files = Request.Files[0];
                    if (upload != null && files.ContentLength > 0)
                    {
                        objPerfModel.upload = "";
                        foreach (var file in upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(file.FileName));
                                string sFilename = "Perf" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objPerfModel.upload = objPerfModel.upload + "," + "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobalData.AddFunctionalLog("Exception in AddExtProviderPerformance-upload: " + ex.ToString());
                            }
                        }
                        objPerfModel.upload = objPerfModel.upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    ExtProviderPerformanceModelsList objExtList = new ExtProviderPerformanceModelsList();
                    objExtList.ExtPerfpList = new List<ExtProviderPerformanceModels>();
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        ExtProviderPerformanceModels AudModel = new ExtProviderPerformanceModels();
                        if (form["Actions" + i] != "" && form["Actions" + i] != null)
                        {
                            AudModel.Actions = form["Actions" + i];
                            AudModel.Personnel_Resp = form["Personnel_Resp" + i];
                            AudModel.Priority = form["Priority" + i];

                            if (DateTime.TryParse(form["Due_Date" + i], out dateValue) == true)
                            {
                                AudModel.Due_Date = dateValue;
                            }
                        }
                        objExtList.ExtPerfpList.Add(AudModel);
                    }

                    if (objPerfModel.FunAddExtProviderPerf(objPerfModel, objExtList))
                    {
                        TempData["Successdata"] = "Added ExtProvider performance details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in AddExtProviderPerformance: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ExtProviderPerformanceList");
        }

        //[AllowAnonymous]
        //public ActionResult ExtProviderPerformanceList(string ExtProviderId, int? page, string branch_name)
        //{
        //    ViewBag.SubMenutype = "ExtProviderPerformance";

        //    ExtProviderPerformanceModelsList objExtProviderList = new ExtProviderPerformanceModelsList();
        //    objExtProviderList.ExtPerfpList = new List<ExtProviderPerformanceModels>();

        //    try
        //    {
        //        string sBranch_name = objGlobalData.GetCurrentUserSession().division;
        //        string sBranchtree = objGlobalData.GetCurrentUserSession().BranchTree;
        //        ViewBag.Branch = objGlobalData.GetMultiBranchListByID(sBranchtree);

        //        string sSqlstmt = "select Id_Performace,ReportNo,Ext_Provider_Name,Eval_Date,Eval_FromDate,Eval_ToDate,Scope_ofSupplies,PO_Issued,PO_Completed,"
        //        + "Scheduled_by,Approved_by,branch,Department,Location from t_extprovider_performance where Active=1";
        //        string sSearchtext = "";
        //        string SupplierId = "";
        //        DateTime EveFromDate = new DateTime(0001, 01, 01);
        //        DateTime EveToDate = new DateTime(0001, 01, 01);
        //        decimal sQuality_Issue = 0, sDelivery_Issue = 0;
        //        decimal varQuality_Issue = 0, varDelivery_Issue = 0;

        //        if (branch_name != null && branch_name != "")
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
        //            ViewBag.Branch_name = branch_name;
        //        }
        //        else
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
        //        }
        //        sSqlstmt = sSqlstmt + sSearchtext + " order by Ext_Provider_Name";
        //        DataSet dsExtProviderModels = objGlobalData.Getdetails(sSqlstmt);
        //        if (dsExtProviderModels.Tables.Count > 0 && dsExtProviderModels.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsExtProviderModels.Tables[0].Rows.Count; i++)
        //            {
        //                try
        //                {
        //                    string Sqlstmt = "select Id_Performace,Ext_Provider_Name,Eval_FromDate,Eval_ToDate from t_extprovider_performance where Id_Performace='" + dsExtProviderModels.Tables[0].Rows[i]["Id_Performace"].ToString() + "'";

        //                    DataSet dsExtModels = objGlobalData.Getdetails(Sqlstmt);
        //                    if (dsExtModels.Tables.Count > 0 && dsExtModels.Tables[0].Rows.Count > 0)
        //                    {
        //                        try
        //                        {
        //                            SupplierId = (dsExtModels.Tables[0].Rows[0]["Ext_Provider_Name"].ToString());

        //                            DateTime dateValue1;

        //                            if (DateTime.TryParse(dsExtModels.Tables[0].Rows[0]["Eval_FromDate"].ToString(), out dateValue1))
        //                            {
        //                                EveFromDate = dateValue1;
        //                            }
        //                            if (DateTime.TryParse(dsExtModels.Tables[0].Rows[0]["Eval_ToDate"].ToString(), out dateValue1))
        //                            {
        //                                EveToDate = dateValue1;
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
        //                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
        //                        }
        //                    }
        //                    if (SupplierId != "" && EveFromDate > Convert.ToDateTime("0001/01/01") && EveToDate > Convert.ToDateTime("0001/01/01"))
        //                    {
        //                        string stmt = "select count(*) as counts from t_external_provider_discrepancylog ," +
        //                            "(Select item_id FROM dropdownitems m, dropdownheader n where m.header_id = n.header_id and header_desc = 'ExtProvider Discrepancy Type'" +
        //                            " and item_desc = 'Delivery') as tt where find_in_set(item_id, discre_relatedto ) > 0 " +
        //                            "and ext_provider_name = '" + SupplierId + "' and discre_registerd_date >= '" + EveFromDate + "' and discre_registerd_date <= '" + EveToDate + "'";
        //                        DataSet dsModels = objGlobalData.Getdetails(stmt);

        //                        if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
        //                        {
        //                            try
        //                            {
        //                                sDelivery_Issue = Convert.ToInt32(dsModels.Tables[0].Rows[0]["counts"].ToString());
        //                                varDelivery_Issue = sDelivery_Issue;
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
        //                                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
        //                            }
        //                        }
        //                        if(sDelivery_Issue == 0 )
        //                        {
        //                            sDelivery_Issue = 1;
        //                        }

        //                        string stmt1 = "select count(*) as counts from t_external_provider_discrepancylog ," +
        //                           "(Select item_id FROM dropdownitems m, dropdownheader n where m.header_id = n.header_id and header_desc = 'ExtProvider Discrepancy Type'" +
        //                           " and item_desc = 'Quality') as tt where find_in_set(item_id, discre_relatedto ) > 0 " +
        //                           "and ext_provider_name = '" + SupplierId + "' and discre_registerd_date >= '" + EveFromDate + "' and discre_registerd_date <= '" + EveToDate + "'";
        //                        DataSet dsModel = objGlobalData.Getdetails(stmt1);

        //                        if (dsModel.Tables.Count > 0 && dsModel.Tables[0].Rows.Count > 0)
        //                        {
        //                            try
        //                            {
        //                                sQuality_Issue = Convert.ToInt32(dsModel.Tables[0].Rows[0]["counts"].ToString());
        //                                varQuality_Issue = sQuality_Issue;                                   }
        //                            catch (Exception ex)
        //                            {
        //                                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
        //                                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
        //                            }
        //                        }

        //                    }
        //                    if (sQuality_Issue == 0)
        //                    {
        //                        sQuality_Issue = 1;
        //                    }

        //                    decimal sPO_Completed = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["PO_Completed"].ToString());

        //                            decimal QualityRate = 0, DeliveryRate = 0, Perfp_rate = 0;
        //                            string Perfp_rate_action = "";
        //                            if (sQuality_Issue != 0 && sPO_Completed != 0)
        //                            {
        //                                QualityRate = (sQuality_Issue * sPO_Completed) / 100;
        //                            }
        //                            if (sDelivery_Issue != 0 && sPO_Completed != 0)
        //                            {
        //                                DeliveryRate = (sDelivery_Issue * sPO_Completed) / 100;
        //                            }
        //                            if (QualityRate != 0 && DeliveryRate != 0)
        //                            {
        //                                Perfp_rate = ((70 * QualityRate) / 100) + ((30 * DeliveryRate) / 100);
        //                            }
        //                            if (Perfp_rate != 0)
        //                            {
        //                                Perfp_rate = System.Math.Round(Perfp_rate, 2)*100;

        //                                if (Perfp_rate < 95)
        //                                {
        //                                    Perfp_rate_action = "Continue to maintain as approved external provider";
        //                                }
        //                                if (Perfp_rate < 85)
        //                                {
        //                                    Perfp_rate_action = "Monitor closely the performance of External Provider";
        //                                }
        //                                if (Perfp_rate < 75)
        //                                {
        //                                    Perfp_rate_action = "Hold on until External Provider submits improvent action plan";
        //                                }
        //                                if (Perfp_rate < 65)
        //                                {
        //                                    Perfp_rate_action = "Remove from approved external providers list";
        //                                }
        //                            }

        //                   ExtProviderPerformanceModels objExtProviderPerformanceModels = new ExtProviderPerformanceModels
        //                    {
        //                        Id_Performace = dsExtProviderModels.Tables[0].Rows[i]["Id_Performace"].ToString(),
        //                        ReportNo = dsExtProviderModels.Tables[0].Rows[i]["ReportNo"].ToString(),
        //                        Ext_Provider_Name = objGlobalData.GetSupplierNameById(dsExtProviderModels.Tables[0].Rows[i]["Ext_Provider_Name"].ToString()),
        //                        Scope_ofSupplies = dsExtProviderModels.Tables[0].Rows[i]["Scope_ofSupplies"].ToString(),
        //                        PO_Issued = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["PO_Issued"].ToString()),
        //                        PO_Completed = System.Math.Round(sPO_Completed, 2),
        //                        Quality_Issue = System.Math.Round(varQuality_Issue, 2),
        //                        Delivery_Issue = System.Math.Round(varDelivery_Issue, 2),
        //                       //Quality_Issue = System.Math.Round(Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["Quality_Issue"].ToString())),
        //                       // Delivery_Issue = System.Math.Round(Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["Delivery_Issue"].ToString())),
        //                        QualityRate = System.Math.Round(QualityRate, 2),
        //                        DeliveryRate = System.Math.Round(DeliveryRate, 2),
        //                        Perfp_rate = Perfp_rate,
        //                        Perfp_rate_action = Perfp_rate_action,
        //                        Scheduled_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[i]["Scheduled_by"].ToString()),
        //                        Approved_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[i]["Approved_by"].ToString()),
        //                        branch = objGlobalData.GetMultiCompanyBranchNameById(dsExtProviderModels.Tables[0].Rows[i]["branch"].ToString()),
        //                        Department = objGlobalData.GetMultiDeptNameById(dsExtProviderModels.Tables[0].Rows[i]["Department"].ToString()),
        //                        Location = objGlobalData.GetDivisionLocationById(dsExtProviderModels.Tables[0].Rows[i]["Location"].ToString()),
        //                   };
        //                    DateTime dateValue;
        //                    if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[i]["Eval_Date"].ToString(), out dateValue))
        //                    {
        //                        objExtProviderPerformanceModels.Eval_Date = dateValue;
        //                    }
        //                    if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[i]["Eval_FromDate"].ToString(), out dateValue))
        //                    {
        //                        objExtProviderPerformanceModels.Eval_FromDate = dateValue;
        //                    }
        //                    if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[i]["Eval_ToDate"].ToString(), out dateValue))
        //                    {
        //                        objExtProviderPerformanceModels.Eval_ToDate = dateValue;
        //                    }
        //                    objExtProviderList.ExtPerfpList.Add(objExtProviderPerformanceModels);
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
        //        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View(objExtProviderList.ExtPerfpList.ToList());
        //}

        [AllowAnonymous]
        public ActionResult ExtProviderPerformanceList(string ExtProviderId, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "ExtProviderPerformance";

            ExtProviderPerformanceModelsList objExtProviderList = new ExtProviderPerformanceModelsList();
            objExtProviderList.ExtPerfpList = new List<ExtProviderPerformanceModels>();

            try
            {
                string sBranch_name = objGlobalData.GetCurrentUserSession().division;
                string sBranchtree = objGlobalData.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobalData.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select Id_Performace,ReportNo,Ext_Provider_Name,Eval_Date,Eval_FromDate,Eval_ToDate,Scope_ofSupplies,PO_Issued,PO_Completed,"
                + "Scheduled_by,Approved_by,branch,Department,Location,Quality_Issue,Delivery_Issue,quantity_issue,lots_received,quality_rating,quantity_rating,delivery_rating,total_rating,apprv_status from t_extprovider_performance where Active=1";
                string sSearchtext = "";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Id_Performace desc";
                DataSet dsExtProviderModels = objGlobalData.Getdetails(sSqlstmt);
                if (dsExtProviderModels.Tables.Count > 0 && dsExtProviderModels.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsExtProviderModels.Tables[0].Rows.Count; i++)
                    {
                        ExtProviderPerformanceModels objExtProviderPerformanceModels = new ExtProviderPerformanceModels
                        {
                            Id_Performace = dsExtProviderModels.Tables[0].Rows[i]["Id_Performace"].ToString(),
                            ReportNo = dsExtProviderModels.Tables[0].Rows[i]["ReportNo"].ToString(),
                            Ext_Provider_Name = objGlobalData.GetSupplierNameById(dsExtProviderModels.Tables[0].Rows[i]["Ext_Provider_Name"].ToString()),
                            Scope_ofSupplies = dsExtProviderModels.Tables[0].Rows[i]["Scope_ofSupplies"].ToString(),

                            Scheduled_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[i]["Scheduled_by"].ToString()),
                            Approved_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[i]["Approved_by"].ToString()),
                            branch = objGlobalData.GetMultiCompanyBranchNameById(dsExtProviderModels.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobalData.GetMultiDeptNameById(dsExtProviderModels.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobalData.GetDivisionLocationById(dsExtProviderModels.Tables[0].Rows[i]["Location"].ToString()),
                            apprv_status = dsExtProviderModels.Tables[0].Rows[i]["apprv_status"].ToString(),
                        };
                        if (dsExtProviderModels.Tables[0].Rows[i]["Quality_Issue"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.Quality_Issue = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["Quality_Issue"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[i]["Delivery_Issue"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.Delivery_Issue = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["Delivery_Issue"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[i]["quantity_issue"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.quantity_issue = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["quantity_issue"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[i]["lots_received"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.lots_received = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["lots_received"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[i]["quality_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.quality_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["quality_rating"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[i]["quantity_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.quantity_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["quantity_rating"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[i]["delivery_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.delivery_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["delivery_rating"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[i]["total_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.total_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["total_rating"].ToString());
                        }

                        DateTime dateValue;
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[i]["Eval_Date"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_Date = dateValue;
                        }
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[i]["Eval_FromDate"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_FromDate = dateValue;
                        }
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[i]["Eval_ToDate"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_ToDate = dateValue;
                        }
                        objExtProviderList.ExtPerfpList.Add(objExtProviderPerformanceModels);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return View(objExtProviderList.ExtPerfpList.ToList());
        }

        [AllowAnonymous]
        public JsonResult ExtProviderPerformanceListSearch(string ExtProviderId, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "ExtProviderPerformance";

            ExtProviderPerformanceModelsList objExtProviderList = new ExtProviderPerformanceModelsList();
            objExtProviderList.ExtPerfpList = new List<ExtProviderPerformanceModels>();

            try
            {
                string sBranch_name = objGlobalData.GetCurrentUserSession().division;
                string sBranchtree = objGlobalData.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobalData.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select Id_Performace,ReportNo,Ext_Provider_Name,Eval_Date,Eval_FromDate,Eval_ToDate,Scope_ofSupplies,PO_Issued,PO_Completed,"
                + "Scheduled_by,Approved_by,branch,Department,Location  from t_extprovider_performance where Active=1";
                string sSearchtext = "";
                string SupplierId = "";
                DateTime EveFromDate = new DateTime(0001, 01, 01);
                DateTime EveToDate = new DateTime(0001, 01, 01);
                decimal sQuality_Issue = 0, sDelivery_Issue = 0;
                decimal varQuality_Issue = 0, varDelivery_Issue = 0;

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Ext_Provider_Name";
                DataSet dsExtProviderModels = objGlobalData.Getdetails(sSqlstmt);
                if (dsExtProviderModels.Tables.Count > 0 && dsExtProviderModels.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsExtProviderModels.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            string Sqlstmt = "select Id_Performace,Ext_Provider_Name,Eval_FromDate,Eval_ToDate from t_extprovider_performance where Id_Performace='" + dsExtProviderModels.Tables[0].Rows[i]["Id_Performace"].ToString() + "'";

                            DataSet dsExtModels = objGlobalData.Getdetails(Sqlstmt);
                            if (dsExtModels.Tables.Count > 0 && dsExtModels.Tables[0].Rows.Count > 0)
                            {
                                try
                                {
                                    SupplierId = (dsExtModels.Tables[0].Rows[0]["Ext_Provider_Name"].ToString());

                                    DateTime dateValue1;

                                    if (DateTime.TryParse(dsExtModels.Tables[0].Rows[0]["Eval_FromDate"].ToString(), out dateValue1))
                                    {
                                        EveFromDate = dateValue1;
                                    }
                                    if (DateTime.TryParse(dsExtModels.Tables[0].Rows[0]["Eval_ToDate"].ToString(), out dateValue1))
                                    {
                                        EveToDate = dateValue1;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
                                    TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            if (SupplierId != "" && EveFromDate > Convert.ToDateTime("0001/01/01") && EveToDate > Convert.ToDateTime("0001/01/01"))
                            {
                                string stmt = "select count(*) as counts from t_external_provider_discrepancylog ," +
                                    "(Select item_id FROM dropdownitems m, dropdownheader n where m.header_id = n.header_id and header_desc = 'ExtProvider Discrepancy Type'" +
                                    " and item_desc = 'Delivery') as tt where find_in_set(item_id, discre_relatedto ) > 0 " +
                                    "and ext_provider_name = '" + SupplierId + "' and discre_registerd_date >= '" + EveFromDate + "' and discre_registerd_date <= '" + EveToDate + "'";
                                DataSet dsModels = objGlobalData.Getdetails(stmt);

                                if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
                                {
                                    try
                                    {
                                        sDelivery_Issue = Convert.ToInt32(dsModels.Tables[0].Rows[0]["counts"].ToString());
                                        varDelivery_Issue = sDelivery_Issue;
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
                                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                                if (sDelivery_Issue == 0)
                                {
                                    sDelivery_Issue = 1;
                                }

                                string stmt1 = "select count(*) as counts from t_external_provider_discrepancylog ," +
                                   "(Select item_id FROM dropdownitems m, dropdownheader n where m.header_id = n.header_id and header_desc = 'ExtProvider Discrepancy Type'" +
                                   " and item_desc = 'Quality') as tt where find_in_set(item_id, discre_relatedto ) > 0 " +
                                   "and ext_provider_name = '" + SupplierId + "' and discre_registerd_date >= '" + EveFromDate + "' and discre_registerd_date <= '" + EveToDate + "'";
                                DataSet dsModel = objGlobalData.Getdetails(stmt1);

                                if (dsModel.Tables.Count > 0 && dsModel.Tables[0].Rows.Count > 0)
                                {
                                    try
                                    {
                                        sQuality_Issue = Convert.ToInt32(dsModel.Tables[0].Rows[0]["counts"].ToString());
                                        varQuality_Issue = sQuality_Issue;
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
                                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }
                            if (sQuality_Issue == 0)
                            {
                                sQuality_Issue = 1;
                            }

                            decimal sPO_Completed = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["PO_Completed"].ToString());

                            decimal QualityRate = 0, DeliveryRate = 0, Perfp_rate = 0;
                            string Perfp_rate_action = "";
                            if (sQuality_Issue != 0 && sPO_Completed != 0)
                            {
                                QualityRate = (sQuality_Issue * sPO_Completed) / 100;
                            }
                            if (sDelivery_Issue != 0 && sPO_Completed != 0)
                            {
                                DeliveryRate = (sDelivery_Issue * sPO_Completed) / 100;
                            }
                            if (QualityRate != 0 && DeliveryRate != 0)
                            {
                                Perfp_rate = ((70 * QualityRate) / 100) + ((30 * DeliveryRate) / 100);
                            }
                            if (Perfp_rate != 0)
                            {
                                Perfp_rate = System.Math.Round(Perfp_rate, 2) * 100;

                                if (Perfp_rate < 95)
                                {
                                    Perfp_rate_action = "Continue to maintain as approved external provider";
                                }
                                if (Perfp_rate < 85)
                                {
                                    Perfp_rate_action = "Monitor closely the performance of External Provider";
                                }
                                if (Perfp_rate < 75)
                                {
                                    Perfp_rate_action = "Hold on until External Provider submits improvent action plan";
                                }
                                if (Perfp_rate < 65)
                                {
                                    Perfp_rate_action = "Remove from approved external providers list";
                                }
                            }

                            ExtProviderPerformanceModels objExtProviderPerformanceModels = new ExtProviderPerformanceModels
                            {
                                Id_Performace = dsExtProviderModels.Tables[0].Rows[i]["Id_Performace"].ToString(),
                                ReportNo = dsExtProviderModels.Tables[0].Rows[i]["ReportNo"].ToString(),
                                Ext_Provider_Name = objGlobalData.GetSupplierNameById(dsExtProviderModels.Tables[0].Rows[i]["Ext_Provider_Name"].ToString()),
                                Scope_ofSupplies = dsExtProviderModels.Tables[0].Rows[i]["Scope_ofSupplies"].ToString(),
                                PO_Issued = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["PO_Issued"].ToString()),
                                PO_Completed = System.Math.Round(sPO_Completed, 2),
                                Quality_Issue = System.Math.Round(varQuality_Issue, 2),
                                Delivery_Issue = System.Math.Round(varDelivery_Issue, 2),
                                //Quality_Issue = System.Math.Round(Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["Quality_Issue"].ToString())),
                                //Delivery_Issue = System.Math.Round(Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[i]["Delivery_Issue"].ToString())),
                                QualityRate = System.Math.Round(QualityRate, 2),
                                DeliveryRate = System.Math.Round(DeliveryRate, 2),
                                Perfp_rate = Perfp_rate,
                                Perfp_rate_action = Perfp_rate_action,
                                Scheduled_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[i]["Scheduled_by"].ToString()),
                                Approved_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[i]["Approved_by"].ToString()),
                                branch = objGlobalData.GetMultiCompanyBranchNameById(dsExtProviderModels.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobalData.GetMultiDeptNameById(dsExtProviderModels.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobalData.GetDivisionLocationById(dsExtProviderModels.Tables[0].Rows[i]["Location"].ToString()),
                            };
                            DateTime dateValue;
                            if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[i]["Eval_FromDate"].ToString(), out dateValue))
                            {
                                objExtProviderPerformanceModels.Eval_FromDate = dateValue;
                            }
                            if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[i]["Eval_FromDate"].ToString(), out dateValue))
                            {
                                objExtProviderPerformanceModels.Eval_FromDate = dateValue;
                            }
                            if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[i]["Eval_ToDate"].ToString(), out dateValue))
                            {
                                objExtProviderPerformanceModels.Eval_ToDate = dateValue;
                            }
                            objExtProviderList.ExtPerfpList.Add(objExtProviderPerformanceModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult ExtProviderPerformanceEdit()
        {
            ViewBag.SubMenutype = "ExtProviderPerformance";

            string sId_Performace = Request.QueryString["Id_Performace"];
            ExtProviderPerformanceModels objExtProviderPerformanceModels = new ExtProviderPerformanceModels();

            try
            {
                ViewBag.SubMenutype = "ExtProviderPerformance";
                ViewBag.SuppList = objGlobalData.GetSupplierList();
                ViewBag.EmpList = objGlobalData.GetHrEmployeeListbox();
                ViewBag.Approver = objGlobalData.GetApprover();
                ViewBag.Priority = objGlobalData.GetDropdownList("ExtProvider Performance Priority");

                string sSqlstmt = "select Id_Performace,ReportNo,Ext_Provider_Name,Eval_Date,Eval_FromDate,Eval_ToDate,Scope_ofSupplies,PO_Issued,PO_Completed,"
               + "Quality_Issue,Delivery_Issue,Scheduled_by,Approved_by,branch,Department,Location,quantity_issue,lots_received,quality_rating,quantity_rating,delivery_rating,total_rating,upload,notified_to,po_detail"
               + " from t_extprovider_performance where Id_Performace='" + sId_Performace + "' order by Id_Performace desc";

                DataSet dsExtProviderModels = objGlobalData.Getdetails(sSqlstmt);
                if (dsExtProviderModels.Tables.Count > 0 && dsExtProviderModels.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        objExtProviderPerformanceModels = new ExtProviderPerformanceModels()
                        {
                            Id_Performace = dsExtProviderModels.Tables[0].Rows[0]["Id_Performace"].ToString(),
                            ReportNo = dsExtProviderModels.Tables[0].Rows[0]["ReportNo"].ToString(),
                            Ext_Provider_Name = /*objGlobalData.GetSupplierNameById*/(dsExtProviderModels.Tables[0].Rows[0]["Ext_Provider_Name"].ToString()),
                            Scope_ofSupplies = dsExtProviderModels.Tables[0].Rows[0]["Scope_ofSupplies"].ToString(),
                            PO_Issued = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["PO_Issued"].ToString()),
                            PO_Completed = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["PO_Completed"].ToString()),
                            //Quality_Issue = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["Quality_Issue"].ToString()),
                            //Delivery_Issue = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["Delivery_Issue"].ToString()),
                            Scheduled_by = /*objGlobalData.GetMultiHrEmpNameById*/(dsExtProviderModels.Tables[0].Rows[0]["Scheduled_by"].ToString()),
                            Approved_by = /*objGlobalData.GetMultiHrEmpNameById*/(dsExtProviderModels.Tables[0].Rows[0]["Approved_by"].ToString()),
                            branch = (dsExtProviderModels.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsExtProviderModels.Tables[0].Rows[0]["Department"].ToString()),
                            Location = (dsExtProviderModels.Tables[0].Rows[0]["Location"].ToString()),
                            upload = (dsExtProviderModels.Tables[0].Rows[0]["upload"].ToString()),
                            po_detail = (dsExtProviderModels.Tables[0].Rows[0]["po_detail"].ToString()),
                        };
                        if (dsExtProviderModels.Tables[0].Rows[0]["notified_to"].ToString() != "")
                        {
                            ViewBag.notified_Array = (dsExtProviderModels.Tables[0].Rows[0]["notified_to"].ToString()).Split(',');
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["Quality_Issue"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.Quality_Issue = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["Quality_Issue"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["Delivery_Issue"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.Delivery_Issue = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["Delivery_Issue"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["quantity_issue"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.quantity_issue = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["quantity_issue"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["lots_received"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.lots_received = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["lots_received"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["quality_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.quality_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["quality_rating"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["quantity_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.quantity_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["quantity_rating"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["delivery_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.delivery_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["delivery_rating"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["total_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.total_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["total_rating"].ToString());
                        }

                        ViewBag.Branch = objGlobalData.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobalData.GetLocationbyMultiDivision(dsExtProviderModels.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobalData.GetDepartmentList1(dsExtProviderModels.Tables[0].Rows[0]["branch"].ToString());
                        // ViewBag.EmpList = objGlobalData.GetGEmpListBymulitBDL(dsExtProviderModels.Tables[0].Rows[0]["branch"].ToString(), dsExtProviderModels.Tables[0].Rows[0]["Department"].ToString(), dsExtProviderModels.Tables[0].Rows[0]["Location"].ToString());
                        //   ViewBag.Approver = objGlobalData.GetGRoleList(dsExtProviderModels.Tables[0].Rows[0]["branch"].ToString(), dsExtProviderModels.Tables[0].Rows[0]["Department"].ToString(), dsExtProviderModels.Tables[0].Rows[0]["Location"].ToString(), "Approver");

                        DateTime dateValue;
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_Date"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_Date = dateValue;
                        }
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_FromDate"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_FromDate = dateValue;
                        }
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_ToDate"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_ToDate = dateValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("ExtProviderPerformanceList");
                }
                ExtProviderPerformanceModelsList objList = new ExtProviderPerformanceModelsList();
                objList.ExtPerfpList = new List<ExtProviderPerformanceModels>();

                sSqlstmt = "select id_Performance_trans,Id_Performace,Actions,Personnel_Resp,Due_Date,Priority " +
                    "from t_extprovider_performance_trans where Id_Performace='" + sId_Performace + "'";

                DataSet dsActionList = objGlobalData.Getdetails(sSqlstmt);
                if (dsActionList.Tables.Count > 0 && dsActionList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsActionList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            ExtProviderPerformanceModels objActionMdl = new ExtProviderPerformanceModels
                            {
                                id_Performance_trans = dsActionList.Tables[0].Rows[i]["id_Performance_trans"].ToString(),
                                Id_Performace = dsActionList.Tables[0].Rows[i]["Id_Performace"].ToString(),
                                Personnel_Resp = /*objGlobalData.GetMultiHrEmpNameById*/(dsActionList.Tables[0].Rows[i]["Personnel_Resp"].ToString()),
                                Actions = dsActionList.Tables[0].Rows[i]["Actions"].ToString(),
                                Priority = dsActionList.Tables[0].Rows[i]["Priority"].ToString(),
                            };

                            DateTime dtDocDate1;
                            if (dsActionList.Tables[0].Rows[i]["Due_Date"].ToString() != ""
                     && DateTime.TryParse(dsActionList.Tables[0].Rows[i]["Due_Date"].ToString(), out dtDocDate1))
                            {
                                objActionMdl.Due_Date = dtDocDate1;
                            }

                            objList.ExtPerfpList.Add(objActionMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                            return RedirectToAction("ExtProviderPerformanceList");
                        }
                    }
                    ViewBag.ExtPerfpList = objList;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }

            return View(objExtProviderPerformanceModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExtProviderPerformanceEdit(ExtProviderPerformanceModels objExtProviderPerformance, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            ViewBag.SubMenutype = "ExtProviderPerformance";
            try
            {
                if (objExtProviderPerformance != null)
                {
                    //objExtProviderPerformance.Id_Performace = Convert.ToInt16(form["Id_Performace"]);
                    //objExtProviderPerformance.ExtProvider_Name = form["ExtProvider_Name"];
                    //objExtProviderPerformance.Eval_ToDate = form["Eval_ToDate"];
                    objExtProviderPerformance.branch = form["branch"];
                    objExtProviderPerformance.Department = form["Department"];
                    objExtProviderPerformance.Location = form["Location"];
                    DateTime dateValue;

                    if (DateTime.TryParse(form["Eval_Date"], out dateValue) == true)
                    {
                        objExtProviderPerformance.Eval_Date = dateValue;
                    }
                    if (DateTime.TryParse(form["Eval_FromDate"], out dateValue) == true)
                    {
                        objExtProviderPerformance.Eval_FromDate = dateValue;
                    }
                    if (DateTime.TryParse(form["Eval_ToDate"], out dateValue) == true)
                    {
                        objExtProviderPerformance.Eval_ToDate = dateValue;
                    }
                    //notified_to
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                    {
                        if (form["nempno " + i] != "" && form["nempno " + i] != null)
                        {
                            objExtProviderPerformance.notified_to = form["nempno " + i] + "," + objExtProviderPerformance.notified_to;
                        }
                    }
                    if (objExtProviderPerformance.notified_to != null)
                    {
                        objExtProviderPerformance.notified_to = objExtProviderPerformance.notified_to.Trim(',');
                    }
                    //upload
                    HttpPostedFileBase files = Request.Files[0];
                    string QCDelete = Request.Form["QCDocsValselectall"];

                    if (upload != null && files.ContentLength > 0)
                    {
                        objExtProviderPerformance.upload = "";
                        foreach (var file in upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(file.FileName));
                                string sFilename = "Perf" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objExtProviderPerformance.upload = objExtProviderPerformance.upload + "," + "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceEdit-upload: " + ex.ToString());
                            }
                        }
                        objExtProviderPerformance.upload = objExtProviderPerformance.upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                    {
                        objExtProviderPerformance.upload = objExtProviderPerformance.upload + "," + form["QCDocsVal"];
                        objExtProviderPerformance.upload = objExtProviderPerformance.upload.Trim(',');
                    }
                    else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                    {
                        objExtProviderPerformance.upload = null;
                    }
                    else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                    {
                        objExtProviderPerformance.upload = null;
                    }
                    ExtProviderPerformanceModelsList objExtProviderList = new ExtProviderPerformanceModelsList();
                    objExtProviderList.ExtPerfpList = new List<ExtProviderPerformanceModels>();

                    int iCnt = 0;
                    if (form["itemcnt"] != null && form["itemcnt"] != "" && int.TryParse(form["itemcnt"], out iCnt))
                    {
                        for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                        {
                            if (form["Actions " + i] != null || form["Personnel_Resp " + i] != null)
                            {
                                ExtProviderPerformanceModels objPerfMdl = new ExtProviderPerformanceModels
                                {
                                    Id_Performace = form["Id_Performace " + i],
                                    Actions = form["Actions " + i],
                                    Personnel_Resp = form["Personnel_Resp " + i],
                                    Priority = form["Priority " + i],
                                };
                                if (DateTime.TryParse(form["Due_Date " + i], out dateValue) == true)
                                {
                                    objPerfMdl.Due_Date = dateValue;
                                }
                                objExtProviderList.ExtPerfpList.Add(objPerfMdl);
                            }
                        }
                    }

                    if (objExtProviderPerformance.FunUpdateExtProviderPerf(objExtProviderPerformance, objExtProviderList))
                    {
                        TempData["Successdata"] = "ExtProvider performance details updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ExtProviderPerformanceList");
        }

        //action initiated
        //Audit Status
        [AllowAnonymous]
        public ActionResult ActionInitiated()
        {
            ExtProviderPerformanceModels objModel = new ExtProviderPerformanceModels();
            try
            {
                if (Request.QueryString["Id_Performace"] != null && Request.QueryString["Id_Performace"] != "")
                {
                    string Id_Performace = Request.QueryString["Id_Performace"];

                    string sSqlstmt = "select Id_Performace,ReportNo,Ext_Provider_Name,initiated_date,action_taken,Due_Date,action_by,action_notified_to from t_extprovider_performance where Id_Performace='" + Id_Performace + "'";

                    DataSet dsModelsList = objGlobalData.Getdetails(sSqlstmt);
                    ViewBag.Action = objGlobalData.GetDropdownList("Supplier action to be taken");
                    ViewBag.EmpList = objGlobalData.GetHrEmployeeListbox();
                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new ExtProviderPerformanceModels
                        {
                            Id_Performace = dsModelsList.Tables[0].Rows[0]["Id_Performace"].ToString(),
                            ReportNo = dsModelsList.Tables[0].Rows[0]["ReportNo"].ToString(),
                            Ext_Provider_Name = objGlobalData.GetSupplierNameById(dsModelsList.Tables[0].Rows[0]["Ext_Provider_Name"].ToString()),
                            action_taken = dsModelsList.Tables[0].Rows[0]["action_taken"].ToString(),
                            action_by = dsModelsList.Tables[0].Rows[0]["action_by"].ToString(),
                            action_notified_to = dsModelsList.Tables[0].Rows[0]["action_notified_to"].ToString(),
                        };
                        if (dsModelsList.Tables[0].Rows[0]["action_by"].ToString() != "")
                        {
                            ViewBag.actbyArray = (dsModelsList.Tables[0].Rows[0]["action_by"].ToString()).Split(',');
                        }
                        //else
                        //{
                        //    if (objGlobalData.GetEmpIDByEmpName("Malhari") != "")
                        //    {
                        //        ViewBag.actbyArray = objGlobalData.GetEmpIDByEmpName("Malhari").Split(',');
                        //    }
                        //}
                        if (dsModelsList.Tables[0].Rows[0]["action_notified_to"].ToString() != "")
                        {
                            ViewBag.NotifiedToArray = (dsModelsList.Tables[0].Rows[0]["action_notified_to"].ToString()).Split(',');
                        }
                        //else
                        //{
                        //    if (objGlobalData.GetQAQCDeptEmployees() != "")
                        //    {
                        //        ViewBag.NotifiedToArray = objGlobalData.GetQAQCDeptEmployees().Split(',');
                        //    }
                        //}
                        DateTime dtValue;
                        if (DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["initiated_date"].ToString(), out dtValue))
                        {
                            objModel.initiated_date = dtValue;
                        }
                        if (DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["Due_Date"].ToString(), out dtValue))
                        {
                            objModel.Due_Date = dtValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in AuditStatusUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ActionInitiated(FormCollection form, ExtProviderPerformanceModels objModel)
        {
            try
            {
                DateTime dateValue;
                //action by
                for (int i = 0; i < Convert.ToInt16(form["actby_cnt"]); i++)
                {
                    if (form["act_empno " + i] != "" && form["act_empno " + i] != null)
                    {
                        objModel.action_by = form["act_empno " + i] + "," + objModel.action_by;
                    }
                }
                if (objModel.action_by != null)
                {
                    objModel.action_by = objModel.action_by.Trim(',');
                }
                //notified_to
                for (int i = 0; i < Convert.ToInt16(form["notified_cnt"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objModel.action_notified_to = form["empno " + i] + "," + objModel.action_notified_to;
                    }
                }
                if (objModel.action_notified_to != null)
                {
                    objModel.action_notified_to = objModel.action_notified_to.Trim(',');
                }
                if (DateTime.TryParse(form["initiated_date"], out dateValue) == true)
                {
                    objModel.initiated_date = dateValue;
                }
                if (DateTime.TryParse(form["Due_Date"], out dateValue) == true)
                {
                    objModel.Due_Date = dateValue;
                }
                if (objModel.FunUpdateActionInitiated(objModel))
                {
                    TempData["Successdata"] = "Action initiated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in ActionInitiated: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ExtProviderPerformanceList");
        }

        [AllowAnonymous]
        public ActionResult ExtProviderPerformanceInfo(int id)
        {
            ViewBag.SubMenutype = "ExtProviderPerformance";

            ExtProviderPerformanceModels objExtProviderPerformanceModels = new ExtProviderPerformanceModels();

            try
            {
                string SupplierId = "";
                DateTime EveFromDate = new DateTime(0001, 01, 01);
                DateTime EveToDate = new DateTime(0001, 01, 01);
                decimal sQuality_Issue = 0, sDelivery_Issue = 0;
                decimal varQuality_Issue = 0, varDelivery_Issue = 0;

                string Sqlstmt = "select Id_Performace,Ext_Provider_Name,Eval_FromDate,Eval_ToDate " +
                    " from t_extprovider_performance where Id_Performace='" + id + "'";

                DataSet dsExtModels = objGlobalData.Getdetails(Sqlstmt);
                if (dsExtModels.Tables.Count > 0 && dsExtModels.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        SupplierId = (dsExtModels.Tables[0].Rows[0]["Ext_Provider_Name"].ToString());

                        DateTime dateValue;

                        if (DateTime.TryParse(dsExtModels.Tables[0].Rows[0]["Eval_FromDate"].ToString(), out dateValue))
                        {
                            EveFromDate = dateValue;
                        }
                        if (DateTime.TryParse(dsExtModels.Tables[0].Rows[0]["Eval_ToDate"].ToString(), out dateValue))
                        {
                            EveToDate = dateValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (SupplierId != "" && EveFromDate > Convert.ToDateTime("0001/01/01") && EveToDate > Convert.ToDateTime("0001/01/01"))
                {
                    string stmt = "select count(*) as counts from t_external_provider_discrepancylog ," +
                        "(Select item_id FROM dropdownitems m, dropdownheader n where m.header_id = n.header_id and header_desc = 'ExtProvider Discrepancy Type'" +
                        " and item_desc = 'Delivery') as tt where find_in_set(item_id, discre_relatedto ) > 0 " +
                        "and ext_provider_name = '" + SupplierId + "' and discre_registerd_date >= '" + EveFromDate + "' and discre_registerd_date <= '" + EveToDate + "'";
                    DataSet dsModels = objGlobalData.Getdetails(stmt);

                    if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            sDelivery_Issue = Convert.ToInt32(dsModels.Tables[0].Rows[0]["counts"].ToString());
                            varDelivery_Issue = sDelivery_Issue;
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    if (sDelivery_Issue == 0)
                    {
                        sDelivery_Issue = 1;
                    }

                    string stmt1 = "select count(*) as counts from t_external_provider_discrepancylog ," +
                       "(Select item_id FROM dropdownitems m, dropdownheader n where m.header_id = n.header_id and header_desc = 'ExtProvider Discrepancy Type'" +
                       " and item_desc = 'Quality') as tt where find_in_set(item_id, discre_relatedto ) > 0 " +
                       "and ext_provider_name = '" + SupplierId + "' and discre_registerd_date >= '" + EveFromDate + "' and discre_registerd_date <= '" + EveToDate + "'";
                    DataSet dsModel = objGlobalData.Getdetails(stmt1);

                    if (dsModel.Tables.Count > 0 && dsModel.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            sQuality_Issue = Convert.ToInt32(dsModel.Tables[0].Rows[0]["counts"].ToString());
                            varQuality_Issue = sQuality_Issue;
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    if (sQuality_Issue == 0)
                    {
                        sQuality_Issue = 1;
                    }
                }

                string sSqlstmt = "select Id_Performace,ReportNo,Ext_Provider_Name,Eval_Date,Eval_FromDate,Eval_ToDate,Scope_ofSupplies,PO_Issued,PO_Completed,"
               + "Scheduled_by,Approved_by,branch,Department,Location from t_extprovider_performance where Id_Performace='" + id + "' order by Id_Performace desc";

                DataSet dsExtProviderModels = objGlobalData.Getdetails(sSqlstmt);
                if (dsExtProviderModels.Tables.Count > 0 && dsExtProviderModels.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        decimal sPO_Completed = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["PO_Completed"].ToString());

                        decimal QualityRate = 0, DeliveryRate = 0, Perfp_rate = 0;
                        string Perfp_rate_action = "";
                        if (sQuality_Issue != 0 && sPO_Completed != 0)
                        {
                            QualityRate = (sQuality_Issue * sPO_Completed) / 100;
                        }
                        if (sDelivery_Issue != 0 && sPO_Completed != 0)
                        {
                            DeliveryRate = (sDelivery_Issue * sPO_Completed) / 100;
                        }
                        if (QualityRate != 0 && DeliveryRate != 0)
                        {
                            Perfp_rate = ((70 * QualityRate) / 100) + ((30 * DeliveryRate) / 100);
                        }
                        if (Perfp_rate != 0)
                        {
                            Perfp_rate = System.Math.Round(Perfp_rate, 2) * 100;

                            if (Perfp_rate < 95)
                            {
                                Perfp_rate_action = "Continue to maintain as approved external provider";
                            }
                            if (Perfp_rate < 85)
                            {
                                Perfp_rate_action = "Monitor closely the performance of External Provider";
                            }
                            if (Perfp_rate < 75)
                            {
                                Perfp_rate_action = "Hold on until External Provider submits improvent action plan";
                            }
                            if (Perfp_rate < 65)
                            {
                                Perfp_rate_action = "Remove from approved external providers list";
                            }
                        }

                        objExtProviderPerformanceModels = new ExtProviderPerformanceModels()
                        {
                            Id_Performace = dsExtProviderModels.Tables[0].Rows[0]["Id_Performace"].ToString(),
                            ReportNo = dsExtProviderModels.Tables[0].Rows[0]["ReportNo"].ToString(),
                            Ext_Provider_Name = objGlobalData.GetSupplierNameById(dsExtProviderModels.Tables[0].Rows[0]["Ext_Provider_Name"].ToString()),
                            Scope_ofSupplies = dsExtProviderModels.Tables[0].Rows[0]["Scope_ofSupplies"].ToString(),
                            PO_Issued = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["PO_Issued"].ToString()),
                            PO_Completed = System.Math.Round(sPO_Completed, 2),
                            Quality_Issue = System.Math.Round(varQuality_Issue, 2),
                            //Quality_Issue = System.Math.Round(Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["Quality_Issue"].ToString())),
                            //Delivery_Issue = System.Math.Round(Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["Delivery_Issue"].ToString())),
                            Delivery_Issue = System.Math.Round(varDelivery_Issue, 2),
                            QualityRate = System.Math.Round(QualityRate, 2),
                            DeliveryRate = System.Math.Round(DeliveryRate, 2),
                            Perfp_rate = Perfp_rate,
                            Perfp_rate_action = Perfp_rate_action,
                            Scheduled_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[0]["Scheduled_by"].ToString()),
                            Approved_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[0]["Approved_by"].ToString()),
                            branch = objGlobalData.GetMultiCompanyBranchNameById(dsExtProviderModels.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobalData.GetMultiDeptNameById(dsExtProviderModels.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobalData.GetDivisionLocationById(dsExtProviderModels.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        DateTime dateValue;
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_Date"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_Date = dateValue;
                        }
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_FromDate"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_FromDate = dateValue;
                        }
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_ToDate"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_ToDate = dateValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceInfo: " + ex.ToString());
                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("ExtProviderPerformanceList");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }

            return View(objExtProviderPerformanceModels);
        }

        [AllowAnonymous]
        public ActionResult ExtProviderPerformanceDetails()
        {
            string sId_Performace = Request.QueryString["Id_Performace"];
            ExtProviderPerformanceModels objExtProviderPerformanceModels = new ExtProviderPerformanceModels();

            try
            {
                ViewBag.ApproveStatus = objGlobalData.GetConstantValueKeyValuePair("ExtProviderPerf");
                string sSqlstmt = "select Id_Performace,ReportNo,Ext_Provider_Name,Eval_Date,Eval_FromDate,Eval_ToDate,Scope_ofSupplies,PO_Issued,PO_Completed,"
               + "Quality_Issue,Delivery_Issue,Scheduled_by,Approved_by,branch,Department,Location,quantity_issue,lots_received,quality_rating,quantity_rating,delivery_rating,total_rating,upload,notified_to,po_detail,initiated_date,action_taken,Due_Date,action_by,action_notified_to,apprv_status as apprv_status_id,approved_date,apprv_comments,approver_upload,"
                 + "(CASE WHEN apprv_status = '0' THEN 'Pending for review' WHEN apprv_status = '1' THEN 'Rejected' WHEN apprv_status = '2' THEN 'Reviewed' END) as apprv_status"
                + " from t_extprovider_performance where Id_Performace='" + sId_Performace + "' order by Id_Performace desc";

                DataSet dsExtProviderModels = objGlobalData.Getdetails(sSqlstmt);
                if (dsExtProviderModels.Tables.Count > 0 && dsExtProviderModels.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        objExtProviderPerformanceModels = new ExtProviderPerformanceModels()
                        {
                            Id_Performace = dsExtProviderModels.Tables[0].Rows[0]["Id_Performace"].ToString(),
                            ReportNo = dsExtProviderModels.Tables[0].Rows[0]["ReportNo"].ToString(),
                            Ext_Provider_Name = objGlobalData.GetSupplierNameById(dsExtProviderModels.Tables[0].Rows[0]["Ext_Provider_Name"].ToString()),
                            Scope_ofSupplies = dsExtProviderModels.Tables[0].Rows[0]["Scope_ofSupplies"].ToString(),

                            Scheduled_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[0]["Scheduled_by"].ToString()),
                            Approved_by = (dsExtProviderModels.Tables[0].Rows[0]["Approved_by"].ToString()),
                            branch = objGlobalData.GetMultiCompanyBranchNameById(dsExtProviderModels.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobalData.GetMultiDeptNameById(dsExtProviderModels.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobalData.GetDivisionLocationById(dsExtProviderModels.Tables[0].Rows[0]["Location"].ToString()),
                            upload = (dsExtProviderModels.Tables[0].Rows[0]["upload"].ToString()),
                            po_detail = (dsExtProviderModels.Tables[0].Rows[0]["po_detail"].ToString()),
                            notified_to = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[0]["notified_to"].ToString()),

                            action_taken = objGlobalData.GetDropdownitemById(dsExtProviderModels.Tables[0].Rows[0]["action_taken"].ToString()),
                            action_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[0]["action_by"].ToString()),
                            action_notified_to = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[0]["action_notified_to"].ToString()),

                            apprv_status = dsExtProviderModels.Tables[0].Rows[0]["apprv_status"].ToString(),
                            apprv_comments = dsExtProviderModels.Tables[0].Rows[0]["apprv_comments"].ToString(),
                            apprv_status_id = dsExtProviderModels.Tables[0].Rows[0]["apprv_status_id"].ToString(),
                            approver_upload = dsExtProviderModels.Tables[0].Rows[0]["approver_upload"].ToString(),
                        };

                        if (dsExtProviderModels.Tables[0].Rows[0]["Quality_Issue"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.Quality_Issue = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["Quality_Issue"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["Delivery_Issue"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.Delivery_Issue = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["Delivery_Issue"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["quantity_issue"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.quantity_issue = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["quantity_issue"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["lots_received"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.lots_received = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["lots_received"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["quality_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.quality_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["quality_rating"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["quantity_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.quantity_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["quantity_rating"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["delivery_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.delivery_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["delivery_rating"].ToString());
                        }
                        if (dsExtProviderModels.Tables[0].Rows[0]["total_rating"].ToString() != "")
                        {
                            objExtProviderPerformanceModels.total_rating = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["total_rating"].ToString());
                        }

                        DateTime dateValue;
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_Date"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_Date = dateValue;
                        }
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_FromDate"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_FromDate = dateValue;
                        }
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_ToDate"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Eval_ToDate = dateValue;
                        }

                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["initiated_date"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.initiated_date = dateValue;
                        }
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Due_Date"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.Due_Date = dateValue;
                        }
                        if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["approved_date"].ToString(), out dateValue))
                        {
                            objExtProviderPerformanceModels.approved_date = dateValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("ExtProviderPerformanceList");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }

            return View(objExtProviderPerformanceModels);
        }

        //Review
        public ActionResult ExtProviderPerformanceReview(ExtProviderPerformanceModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> approver_upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                if (approver_upload != null && files.ContentLength > 0)
                {
                    objModel.approver_upload = "";
                    foreach (var file in approver_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(file.FileName));
                            string sFilename = "Perf" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.approver_upload = objModel.approver_upload + "," + "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceReview-upload: " + ex.ToString());
                        }
                    }
                    objModel.approver_upload = objModel.approver_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objModel.FunReviewPerfEvaluation(objModel))
                {
                    TempData["Successdata"] = "Updated Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in AuditChecklistApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        //[AllowAnonymous]
        //public ActionResult ExtProviderPerformanceDetails()
        //{
        //    ViewBag.SubMenutype = "ExtProviderPerformance";

        //    string sId_Performace = Request.QueryString["Id_Performace"];
        //    ExtProviderPerformanceModels objExtProviderPerformanceModels = new ExtProviderPerformanceModels();

        //    try
        //    {
        //        string SupplierId = "";
        //        DateTime EveFromDate = new DateTime(0001,01,01);
        //        DateTime EveToDate = new DateTime(0001,01,01);
        //        decimal sQuality_Issue=0, sDelivery_Issue=0;
        //        decimal varQuality_Issue = 0, varDelivery_Issue = 0;

        //        string Sqlstmt = "select Id_Performace,Ext_Provider_Name,Eval_FromDate,Eval_ToDate" +
        //            "  from t_extprovider_performance where Id_Performace='" + sId_Performace + "'";

        //        DataSet dsExtModels = objGlobalData.Getdetails(Sqlstmt);
        //        if (dsExtModels.Tables.Count > 0 && dsExtModels.Tables[0].Rows.Count > 0)
        //        {
        //            try
        //            {
        //                SupplierId = (dsExtModels.Tables[0].Rows[0]["Ext_Provider_Name"].ToString());

        //                DateTime dateValue;

        //                if (DateTime.TryParse(dsExtModels.Tables[0].Rows[0]["Eval_FromDate"].ToString(), out dateValue))
        //                {
        //                    EveFromDate = dateValue;
        //                }
        //                if (DateTime.TryParse(dsExtModels.Tables[0].Rows[0]["Eval_ToDate"].ToString(), out dateValue))
        //                {
        //                    EveToDate = dateValue;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
        //                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
        //            }
        //        }
        //        if (SupplierId != "" && EveFromDate > Convert.ToDateTime("0001/01/01") && EveToDate > Convert.ToDateTime("0001/01/01"))
        //       {
        //        string stmt = "select count(*) as counts from t_external_provider_discrepancylog ," +
        //            "(Select item_id FROM dropdownitems m, dropdownheader n where m.header_id = n.header_id and header_desc = 'ExtProvider Discrepancy Type'" +
        //            " and item_desc = 'Delivery') as tt where find_in_set(item_id, discre_relatedto ) > 0 " +
        //            "and ext_provider_name = '" + SupplierId + "' and discre_registerd_date >= '" + EveFromDate + "' and discre_registerd_date <= '" + EveToDate + "'";
        //        DataSet dsModels = objGlobalData.Getdetails(stmt);

        //        if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
        //        {
        //            try
        //            {
        //                    sDelivery_Issue = Convert.ToInt32(dsModels.Tables[0].Rows[0]["counts"].ToString());
        //                    varDelivery_Issue = sDelivery_Issue;
        //            }
        //            catch (Exception ex)
        //            {
        //                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
        //                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
        //            }
        //        }
        //            if (sDelivery_Issue == 0)
        //            {
        //                sDelivery_Issue = 1;
        //            }
        //            string stmt1 = "select count(*) as counts from t_external_provider_discrepancylog ," +
        //               "(Select item_id FROM dropdownitems m, dropdownheader n where m.header_id = n.header_id and header_desc = 'ExtProvider Discrepancy Type'" +
        //               " and item_desc = 'Quality') as tt where find_in_set(item_id, discre_relatedto ) > 0 " +
        //               "and ext_provider_name = '" + SupplierId + "' and discre_registerd_date >= '" + EveFromDate + "' and discre_registerd_date <= '" + EveToDate + "'";
        //            DataSet dsModel = objGlobalData.Getdetails(stmt1);

        //            if (dsModel.Tables.Count > 0 && dsModel.Tables[0].Rows.Count > 0)
        //            {
        //                try
        //                {
        //                    sQuality_Issue = Convert.ToInt32(dsModel.Tables[0].Rows[0]["counts"].ToString());
        //                    varQuality_Issue = sQuality_Issue;
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceList: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
        //                }
        //            }
        //            if (sQuality_Issue == 0)
        //            {
        //                sQuality_Issue = 1;
        //            }
        //        }
        //        string sSqlstmt = "select Id_Performace,ReportNo,Ext_Provider_Name,Eval_Date,Eval_FromDate,Eval_ToDate,Scope_ofSupplies,PO_Issued,PO_Completed,"
        //       + "Scheduled_by,Approved_by,branch,Department,Location from t_extprovider_performance where Id_Performace='" + sId_Performace + "' order by Id_Performace desc";

        //        DataSet dsExtProviderModels = objGlobalData.Getdetails(sSqlstmt);
        //        if (dsExtProviderModels.Tables.Count > 0 && dsExtProviderModels.Tables[0].Rows.Count > 0)
        //        {
        //            try
        //            {
        //                decimal sPO_Completed = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["PO_Completed"].ToString());

        //                decimal QualityRate = 0, DeliveryRate = 0, Perfp_rate = 0;
        //                string Perfp_rate_action = "";
        //                if (sQuality_Issue != 0 && sPO_Completed != 0)
        //                {
        //                    QualityRate = (sQuality_Issue * sPO_Completed) / 100;
        //                }
        //                if (sDelivery_Issue != 0 && sPO_Completed != 0)
        //                {
        //                    DeliveryRate = (sDelivery_Issue * sPO_Completed) / 100;
        //                }
        //                if (QualityRate != 0 && DeliveryRate != 0)
        //                {
        //                    Perfp_rate = ((70 * QualityRate) / 100) + ((30 * DeliveryRate) / 100);
        //                }
        //                if (Perfp_rate != 0)
        //                {
        //                    Perfp_rate = System.Math.Round(Perfp_rate, 2)*100;

        //                    if (Perfp_rate < 95)
        //                    {
        //                        Perfp_rate_action = "Continue to maintain as approved external provider";
        //                    }
        //                    if (Perfp_rate < 85)
        //                    {
        //                        Perfp_rate_action = "Monitor closely the performance of External Provider";
        //                    }
        //                    if (Perfp_rate < 75)
        //                    {
        //                        Perfp_rate_action = "Hold on until External Provider submits improvent action plan";
        //                    }
        //                    if (Perfp_rate < 65)
        //                    {
        //                        Perfp_rate_action = "Remove from approved external providers list";
        //                    }
        //                }

        //                objExtProviderPerformanceModels = new ExtProviderPerformanceModels()
        //                {
        //                    Id_Performace = dsExtProviderModels.Tables[0].Rows[0]["Id_Performace"].ToString(),
        //                    ReportNo = dsExtProviderModels.Tables[0].Rows[0]["ReportNo"].ToString(),
        //                    Ext_Provider_Name = objGlobalData.GetSupplierNameById(dsExtProviderModels.Tables[0].Rows[0]["Ext_Provider_Name"].ToString()),
        //                    Scope_ofSupplies = dsExtProviderModels.Tables[0].Rows[0]["Scope_ofSupplies"].ToString(),
        //                    PO_Issued = Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["PO_Issued"].ToString()),
        //                    PO_Completed = System.Math.Round(sPO_Completed, 2),
        //                    Quality_Issue = System.Math.Round(varQuality_Issue, 2),
        //                    Delivery_Issue = System.Math.Round(varDelivery_Issue, 2),
        //                    //Quality_Issue = System.Math.Round(Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["Quality_Issue"].ToString())),
        //                    //Delivery_Issue = System.Math.Round(Convert.ToDecimal(dsExtProviderModels.Tables[0].Rows[0]["Delivery_Issue"].ToString())),
        //                    QualityRate = System.Math.Round(QualityRate, 2),
        //                    DeliveryRate = System.Math.Round(DeliveryRate, 2),
        //                    Perfp_rate = Perfp_rate,
        //                    Perfp_rate_action = Perfp_rate_action,
        //                    Scheduled_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[0]["Scheduled_by"].ToString()),
        //                    Approved_by = objGlobalData.GetMultiHrEmpNameById(dsExtProviderModels.Tables[0].Rows[0]["Approved_by"].ToString()),
        //                    branch = objGlobalData.GetMultiCompanyBranchNameById(dsExtProviderModels.Tables[0].Rows[0]["branch"].ToString()),
        //                    Department = objGlobalData.GetMultiDeptNameById(dsExtProviderModels.Tables[0].Rows[0]["Department"].ToString()),
        //                    Location = objGlobalData.GetDivisionLocationById(dsExtProviderModels.Tables[0].Rows[0]["Location"].ToString()),

        //                };
        //                DateTime dateValue;
        //                if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_Date"].ToString(), out dateValue))
        //                {
        //                    objExtProviderPerformanceModels.Eval_Date = dateValue;
        //                }
        //                if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_FromDate"].ToString(), out dateValue))
        //                {
        //                    objExtProviderPerformanceModels.Eval_FromDate = dateValue;
        //                }
        //                if (DateTime.TryParse(dsExtProviderModels.Tables[0].Rows[0]["Eval_ToDate"].ToString(), out dateValue))
        //                {
        //                    objExtProviderPerformanceModels.Eval_ToDate = dateValue;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceDetails: " + ex.ToString());
        //                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
        //            }
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = "Id cannot be Null or empty";
        //            return RedirectToAction("ExtProviderPerformanceList");
        //        }
        //        ExtProviderPerformanceModelsList objList = new ExtProviderPerformanceModelsList();
        //        objList.ExtPerfpList = new List<ExtProviderPerformanceModels>();

        //        sSqlstmt = "select id_Performance_trans,Id_Performace,Actions,Personnel_Resp,Due_Date,Priority " +
        //            "from t_extprovider_performance_trans where Id_Performace='" + sId_Performace + "'";

        //        DataSet dsActionList = objGlobalData.Getdetails(sSqlstmt);
        //        if (dsActionList.Tables.Count > 0 && dsActionList.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsActionList.Tables[0].Rows.Count; i++)
        //            {
        //                try
        //                {
        //                    ExtProviderPerformanceModels objActionMdl = new ExtProviderPerformanceModels
        //                    {
        //                        id_Performance_trans = dsActionList.Tables[0].Rows[i]["id_Performance_trans"].ToString(),
        //                        Id_Performace = dsActionList.Tables[0].Rows[i]["Id_Performace"].ToString(),
        //                        Personnel_Resp = objGlobalData.GetMultiHrEmpNameById(dsActionList.Tables[0].Rows[i]["Personnel_Resp"].ToString()),
        //                        Actions = dsActionList.Tables[0].Rows[i]["Actions"].ToString(),
        //                        Priority = objGlobalData.GetDropdownitemById(dsActionList.Tables[0].Rows[i]["Priority"].ToString()),
        //                    };

        //                    DateTime dtDocDate1;
        //                    if (dsActionList.Tables[0].Rows[i]["Due_Date"].ToString() != ""
        //             && DateTime.TryParse(dsActionList.Tables[0].Rows[i]["Due_Date"].ToString(), out dtDocDate1))
        //                    {
        //                        objActionMdl.Due_Date = dtDocDate1;
        //                    }

        //                    objList.ExtPerfpList.Add(objActionMdl);
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceEdit: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
        //                    return RedirectToAction("ExtProviderPerformanceList");
        //                }
        //            }
        //            ViewBag.objPerfList = objList;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceDetails: " + ex.ToString());
        //        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
        //    }

        //    return View(objExtProviderPerformanceModels);
        //}

        [AllowAnonymous]
        public JsonResult ExtProviderPerformanceDelete(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "ExtProviderPerformance";
                if (form["Id_Performace"] != null && form["Id_Performace"] != "")
                {
                    ExtProviderPerformanceModels Doc = new ExtProviderPerformanceModels();
                    string sId_Performace = form["Id_Performace"];

                    if (Doc.FunDeleteExtProviderPerf(sId_Performace))
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
                    TempData["alertdata"] = "ExtProviderPerformanceDelete Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in ExtProviderPerformanceDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult AddDiscrepancyLog()
        {
            ExtProviderDiscrepencyLogModels objDiscrepency = new ExtProviderDiscrepencyLogModels();
            try
            {
                ViewBag.SubMenutype = "DiscrepancyLog";
                objDiscrepency.branch = objGlobalData.GetCurrentUserSession().division;
                objDiscrepency.Department = objGlobalData.GetCurrentUserSession().DeptID;
                objDiscrepency.Location = objGlobalData.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobalData.GetCompanyBranchListbox();
                ViewBag.Department = objGlobalData.GetDepartmentListbox(objDiscrepency.branch);
                ViewBag.Location = objGlobalData.GetDivisionLocationList(objDiscrepency.branch);
                ViewBag.SupplierList = objGlobalData.GetSupplierList();
                ViewBag.DiscrepancyType = objGlobalData.GetDropdownList("ExtProvider Discrepancy Type");
                ViewBag.YesNo = objGlobalData.GetConstantValue("YesNo");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in AddDiscrepancyLog: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }

            return View(objDiscrepency);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDiscrepancyLog(ExtProviderDiscrepencyLogModels objDiscrepencyLogModels, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objDiscrepencyLogModels.branch = form["branch"];
                objDiscrepencyLogModels.Department = form["Department"];
                objDiscrepencyLogModels.Location = form["Location"];

                HttpPostedFileBase files = Request.Files[0];

                objDiscrepencyLogModels.discre_relatedto = form["discre_relatedto"];
                DateTime dateValue;

                if (DateTime.TryParse(form["discre_registerd_date"], out dateValue) == true)
                {
                    objDiscrepencyLogModels.discre_registerd_date = dateValue;
                }

                if (DateTime.TryParse(form["discre_reported_date"], out dateValue) == true)
                {
                    objDiscrepencyLogModels.discre_reported_date = dateValue;
                }

                if (upload != null && files.ContentLength > 0)
                {
                    objDiscrepencyLogModels.upload = "";

                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(files.FileName));
                            string sFilename = "Discrepancy" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            files.SaveAs(sFilepath + "/" + sFilename);
                            objDiscrepencyLogModels.upload = objDiscrepencyLogModels.upload + "," + "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in AddDiscrepancyLog: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objDiscrepencyLogModels.upload = objDiscrepencyLogModels.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objDiscrepencyLogModels.FunAddExtProviderDiscrepencyLog(objDiscrepencyLogModels))
                {
                    TempData["Successdata"] = "Added Supplier Discrepency Log details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in AddDiscrepancyLog: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DiscrepancyLogList");
        }

        [AllowAnonymous]
        public ActionResult DiscrepancyLogList(int? page, string branch_name)
        {
            ViewBag.SubMenutype = "DiscrepancyLog";

            ExtProviderDiscrepencyLogModelsList objList = new ExtProviderDiscrepencyLogModelsList();
            objList.DescripList = new List<ExtProviderDiscrepencyLogModels>();

            try
            {
                string sBranch_name = objGlobalData.GetCurrentUserSession().division;
                string sBranchtree = objGlobalData.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobalData.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_discrepancylog, discrepancy_no, discre_registerd_date,discre_reported_date," +
                    "ext_provider_name, delivery_note_no, po_no, discre_detail,discre_relatedto,upload,actions,impact," +
                    "ncr_required,branch,Department,Location,id_nc from t_external_provider_discrepancylog where active=1 and ext_valid='Valid'";

                SupplierModels objSupplier = new SupplierModels();

                ViewBag.SupplierList = objGlobalData.GetSupplierList();
                string sSearchtext = "";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by id_discrepancylog desc";

                DataSet dsSupplier = objGlobalData.Getdetails(sSqlstmt);

                if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsSupplier.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            ExtProviderDiscrepencyLogModels objSupplierModels = new ExtProviderDiscrepencyLogModels
                            {
                                id_discrepancylog = dsSupplier.Tables[0].Rows[i]["id_discrepancylog"].ToString(),
                                discrepancy_no = dsSupplier.Tables[0].Rows[i]["discrepancy_no"].ToString(),
                                ext_provider_name = objGlobalData.GetSupplierNameById(dsSupplier.Tables[0].Rows[i]["ext_provider_name"].ToString()),
                                delivery_note_no = dsSupplier.Tables[0].Rows[i]["delivery_note_no"].ToString(),
                                po_no = dsSupplier.Tables[0].Rows[i]["po_no"].ToString(),
                                discre_detail = dsSupplier.Tables[0].Rows[i]["discre_detail"].ToString(),
                                discre_relatedto = objGlobalData.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["discre_relatedto"].ToString()),
                                upload = dsSupplier.Tables[0].Rows[i]["upload"].ToString(),
                                actions = dsSupplier.Tables[0].Rows[i]["actions"].ToString(),
                                impact = dsSupplier.Tables[0].Rows[i]["impact"].ToString(),
                                ncr_required = (dsSupplier.Tables[0].Rows[i]["ncr_required"].ToString()),
                                branch = objGlobalData.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobalData.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobalData.GetDivisionLocationById(dsSupplier.Tables[0].Rows[i]["Location"].ToString()),
                                id_ncs = objGlobalData.GetNCNOById(dsSupplier.Tables[0].Rows[i]["id_nc"].ToString()),
                                id_nc = (dsSupplier.Tables[0].Rows[i]["id_nc"].ToString()),
                            };
                            DateTime dtDocDate;

                            if (dsSupplier.Tables[0].Rows[i]["discre_registerd_date"].ToString() != ""
                               && DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["discre_registerd_date"].ToString(), out dtDocDate))
                            {
                                objSupplierModels.discre_registerd_date = dtDocDate;
                            }
                            if (dsSupplier.Tables[0].Rows[i]["discre_reported_date"].ToString() != ""
                              && DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["discre_reported_date"].ToString(), out dtDocDate))
                            {
                                objSupplierModels.discre_reported_date = dtDocDate;
                            }
                            objList.DescripList.Add(objSupplierModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogList: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogList: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }

            return View(objList.DescripList.ToList());
        }

        [AllowAnonymous]
        public JsonResult DiscrepancyLogListSearch(int? page, string branch_name)
        {
            ViewBag.SubMenutype = "DiscrepancyLog";

            ExtProviderDiscrepencyLogModelsList objList = new ExtProviderDiscrepencyLogModelsList();
            objList.DescripList = new List<ExtProviderDiscrepencyLogModels>();

            try
            {
                string sBranch_name = objGlobalData.GetCurrentUserSession().division;
                string sBranchtree = objGlobalData.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobalData.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_discrepancylog, discrepancy_no, discre_registerd_date,discre_reported_date," +
                    "ext_provider_name, delivery_note_no, po_no, discre_detail,discre_relatedto,upload,actions,impact," +
                    "ncr_required,branch,Department,Location from t_external_provider_discrepancylog where active=1";

                SupplierModels objSupplier = new SupplierModels();

                ViewBag.SupplierList = objGlobalData.GetSupplierList();
                string sSearchtext = "";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by id_discrepancylog desc";

                DataSet dsSupplier = objGlobalData.Getdetails(sSqlstmt);

                if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsSupplier.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            ExtProviderDiscrepencyLogModels objSupplierModels = new ExtProviderDiscrepencyLogModels
                            {
                                id_discrepancylog = dsSupplier.Tables[0].Rows[i]["id_discrepancylog"].ToString(),
                                discrepancy_no = dsSupplier.Tables[0].Rows[i]["discrepancy_no"].ToString(),
                                ext_provider_name = objGlobalData.GetSupplierNameById(dsSupplier.Tables[0].Rows[i]["ext_provider_name"].ToString()),
                                delivery_note_no = dsSupplier.Tables[0].Rows[i]["delivery_note_no"].ToString(),
                                po_no = dsSupplier.Tables[0].Rows[i]["po_no"].ToString(),
                                discre_detail = dsSupplier.Tables[0].Rows[i]["discre_detail"].ToString(),
                                discre_relatedto = objGlobalData.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["discre_relatedto"].ToString()),
                                upload = dsSupplier.Tables[0].Rows[i]["upload"].ToString(),
                                actions = dsSupplier.Tables[0].Rows[i]["actions"].ToString(),
                                impact = dsSupplier.Tables[0].Rows[i]["impact"].ToString(),
                                ncr_required = (dsSupplier.Tables[0].Rows[i]["ncr_required"].ToString()),
                                branch = objGlobalData.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobalData.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobalData.GetDivisionLocationById(dsSupplier.Tables[0].Rows[i]["Location"].ToString()),
                            };
                            DateTime dtDocDate;

                            if (dsSupplier.Tables[0].Rows[i]["discre_registerd_date"].ToString() != ""
                               && DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["discre_registerd_date"].ToString(), out dtDocDate))
                            {
                                objSupplierModels.discre_registerd_date = dtDocDate;
                            }
                            if (dsSupplier.Tables[0].Rows[i]["discre_reported_date"].ToString() != ""
                              && DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["discre_reported_date"].ToString(), out dtDocDate))
                            {
                                objSupplierModels.discre_reported_date = dtDocDate;
                            }
                            objList.DescripList.Add(objSupplierModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult DiscrepancyLogDetails()
        {
            ViewBag.SubMenutype = "DiscrepancyLog";
            ExtProviderDiscrepencyLogModels objSupplierModels = new ExtProviderDiscrepencyLogModels();
            try
            {
                if (Request.QueryString["id_discrepancylog"] != null && Request.QueryString["id_discrepancylog"] != "")
                {
                    string sid_discrepancylog = Request.QueryString["id_discrepancylog"];

                    string sSqlstmt = "select id_discrepancylog, discrepancy_no, discre_registerd_date,discre_reported_date," +
                   "ext_provider_name, delivery_note_no, po_no, discre_detail,discre_relatedto,upload,actions,impact,ncr_required,branch,Department,Location from t_external_provider_discrepancylog"
                   + " where id_discrepancylog = '" + sid_discrepancylog + "'";

                    DataSet dsSupplier = objGlobalData.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objSupplierModels = new ExtProviderDiscrepencyLogModels
                            {
                                id_discrepancylog = dsSupplier.Tables[0].Rows[0]["id_discrepancylog"].ToString(),
                                discrepancy_no = dsSupplier.Tables[0].Rows[0]["discrepancy_no"].ToString(),
                                ext_provider_name = objGlobalData.GetSupplierNameById(dsSupplier.Tables[0].Rows[0]["ext_provider_name"].ToString()),
                                delivery_note_no = dsSupplier.Tables[0].Rows[0]["delivery_note_no"].ToString(),
                                po_no = dsSupplier.Tables[0].Rows[0]["po_no"].ToString(),
                                discre_detail = dsSupplier.Tables[0].Rows[0]["discre_detail"].ToString(),
                                discre_relatedto = objGlobalData.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["discre_relatedto"].ToString()),
                                upload = dsSupplier.Tables[0].Rows[0]["upload"].ToString(),
                                actions = dsSupplier.Tables[0].Rows[0]["actions"].ToString(),
                                impact = dsSupplier.Tables[0].Rows[0]["impact"].ToString(),
                                ncr_required = (dsSupplier.Tables[0].Rows[0]["ncr_required"].ToString()),
                                branch = objGlobalData.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                                Department = objGlobalData.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                                Location = objGlobalData.GetDivisionLocationById(dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                            };
                            DateTime dtDocDate;

                            if (dsSupplier.Tables[0].Rows[0]["discre_registerd_date"].ToString() != ""
                                   && DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["discre_registerd_date"].ToString(), out dtDocDate))
                            {
                                objSupplierModels.discre_registerd_date = dtDocDate;
                            }
                            if (dsSupplier.Tables[0].Rows[0]["discre_reported_date"].ToString() != ""
                              && DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["discre_reported_date"].ToString(), out dtDocDate))
                            {
                                objSupplierModels.discre_reported_date = dtDocDate;
                            }
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return View(objSupplierModels);
        }

        [AllowAnonymous]
        public ActionResult DiscrepancyLogReport(FormCollection form)
        {
            ViewBag.SubMenutype = "DiscrepancyLog";
            ExtProviderDiscrepencyLogModels objSupplierModels = new ExtProviderDiscrepencyLogModels();
            try
            {
                string sid_discrepancylog = form["id_discrepancylog"];
                if (sid_discrepancylog != null && sid_discrepancylog != "")
                {
                    string sSqlstmt = "select id_discrepancylog, discrepancy_no, discre_registerd_date,discre_reported_date," +
                   "ext_provider_name, delivery_note_no, po_no, discre_detail,discre_relatedto,upload,actions,impact,ncr_required,branch,Department,Location from t_external_provider_discrepancylog"
                   + " where id_discrepancylog = '" + sid_discrepancylog + "'";

                    DataSet dsSupplier = objGlobalData.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objSupplierModels = new ExtProviderDiscrepencyLogModels
                            {
                                id_discrepancylog = dsSupplier.Tables[0].Rows[0]["id_discrepancylog"].ToString(),
                                discrepancy_no = dsSupplier.Tables[0].Rows[0]["discrepancy_no"].ToString(),
                                ext_provider_name = objGlobalData.GetSupplierNameById(dsSupplier.Tables[0].Rows[0]["ext_provider_name"].ToString()),
                                delivery_note_no = dsSupplier.Tables[0].Rows[0]["delivery_note_no"].ToString(),
                                po_no = dsSupplier.Tables[0].Rows[0]["po_no"].ToString(),
                                discre_detail = dsSupplier.Tables[0].Rows[0]["discre_detail"].ToString(),
                                discre_relatedto = objGlobalData.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["discre_relatedto"].ToString()),
                                upload = dsSupplier.Tables[0].Rows[0]["upload"].ToString(),
                                actions = dsSupplier.Tables[0].Rows[0]["actions"].ToString(),
                                impact = dsSupplier.Tables[0].Rows[0]["impact"].ToString(),
                                ncr_required = (dsSupplier.Tables[0].Rows[0]["ncr_required"].ToString()),
                                branch = objGlobalData.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                                Department = objGlobalData.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                                Location = objGlobalData.GetDivisionLocationById(dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                            };
                            DateTime dtDocDate;

                            if (dsSupplier.Tables[0].Rows[0]["discre_registerd_date"].ToString() != ""
                                   && DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["discre_registerd_date"].ToString(), out dtDocDate))
                            {
                                objSupplierModels.discre_registerd_date = dtDocDate;
                            }
                            if (dsSupplier.Tables[0].Rows[0]["discre_reported_date"].ToString() != ""
                              && DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["discre_reported_date"].ToString(), out dtDocDate))
                            {
                                objSupplierModels.discre_reported_date = dtDocDate;
                            }
                            ViewBag.Supplier = objSupplierModels;
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogReport: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogReport: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string header = Server.MapPath("~/views/ExtProviderPerformance/PrintHeader.html");//Path of PrintHeader.html File

            string customSwitches = string.Format("--header-html  \"{0}\" " +
                                 "--header-spacing \"0\" ", header);

            return new ViewAsPdf("DiscrepancyLogReport", "ExtProviderPerformance")
            {
                //FileName = "DiscrepancyLogReport.pdf",
                Cookies = cookieCollection,
                PageSize = Rotativa.Options.Size.A3,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                CustomSwitches =
                   customSwitches,
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 35 },
                // PageMargins = new Rotativa.Options.Margins(0, 3, 32, 3),
            };
        }

        [AllowAnonymous]
        public ActionResult DiscrepancyLogInfo(int id)
        {
            ViewBag.SubMenutype = "DiscrepancyLog";
            ExtProviderDiscrepencyLogModels objSupplierModels = new ExtProviderDiscrepencyLogModels();
            try
            {
                string sSqlstmt = "select id_discrepancylog, discrepancy_no, discre_registerd_date,discre_reported_date," +
                   "ext_provider_name, delivery_note_no, po_no, discre_detail,discre_relatedto,upload,actions,impact,ncr_required,branch,Department,Location from t_external_provider_discrepancylog"
                   + " where id_discrepancylog = '" + id + "'";

                DataSet dsSupplier = objGlobalData.Getdetails(sSqlstmt);

                if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        objSupplierModels = new ExtProviderDiscrepencyLogModels
                        {
                            id_discrepancylog = dsSupplier.Tables[0].Rows[0]["id_discrepancylog"].ToString(),
                            discrepancy_no = dsSupplier.Tables[0].Rows[0]["discrepancy_no"].ToString(),
                            ext_provider_name = objGlobalData.GetSupplierNameById(dsSupplier.Tables[0].Rows[0]["ext_provider_name"].ToString()),
                            delivery_note_no = dsSupplier.Tables[0].Rows[0]["delivery_note_no"].ToString(),
                            po_no = dsSupplier.Tables[0].Rows[0]["po_no"].ToString(),
                            discre_detail = dsSupplier.Tables[0].Rows[0]["discre_detail"].ToString(),
                            discre_relatedto = objGlobalData.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["discre_relatedto"].ToString()),
                            upload = dsSupplier.Tables[0].Rows[0]["upload"].ToString(),
                            actions = dsSupplier.Tables[0].Rows[0]["actions"].ToString(),
                            impact = dsSupplier.Tables[0].Rows[0]["impact"].ToString(),
                            ncr_required = (dsSupplier.Tables[0].Rows[0]["ncr_required"].ToString()),
                            branch = objGlobalData.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobalData.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobalData.GetDivisionLocationById(dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        DateTime dtDocDate;

                        if (dsSupplier.Tables[0].Rows[0]["discre_registerd_date"].ToString() != ""
                               && DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["discre_registerd_date"].ToString(), out dtDocDate))
                        {
                            objSupplierModels.discre_registerd_date = dtDocDate;
                        }
                        if (dsSupplier.Tables[0].Rows[0]["discre_reported_date"].ToString() != ""
                          && DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["discre_reported_date"].ToString(), out dtDocDate))
                        {
                            objSupplierModels.discre_reported_date = dtDocDate;
                        }
                    }
                    catch (Exception ex)
                    {
                        objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogInfo: " + ex.ToString());
                        TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return View(objSupplierModels);
        }

        [AllowAnonymous]
        public ActionResult DiscrepancyLogEdit()
        {
            ViewBag.SubMenutype = "DiscrepancyLog";
            ExtProviderDiscrepencyLogModels objSupplierModels = new ExtProviderDiscrepencyLogModels();
            SupplierModels objSupplier = new SupplierModels();
            try
            {
                ViewBag.SupplierList = objGlobalData.GetSupplierList();
                ViewBag.DiscrepancyType = objGlobalData.GetDropdownList("ExtProvider Discrepancy Type");
                ViewBag.YesNo = objGlobalData.GetConstantValue("YesNo");

                if (Request.QueryString["id_discrepancylog"] != null && Request.QueryString["id_discrepancylog"] != "")
                {
                    string sid_discrepancylog = Request.QueryString["id_discrepancylog"];
                    string sSqlstmt = "select id_discrepancylog, discrepancy_no, discre_registerd_date,discre_reported_date," +
                   "ext_provider_name, delivery_note_no, po_no, discre_detail,discre_relatedto,upload,actions,impact,ncr_required,branch,Department,Location from t_external_provider_discrepancylog"
                   + " where id_discrepancylog = '" + sid_discrepancylog + "'";

                    DataSet dsSupplier = objGlobalData.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        objSupplierModels = new ExtProviderDiscrepencyLogModels
                        {
                            id_discrepancylog = dsSupplier.Tables[0].Rows[0]["id_discrepancylog"].ToString(),
                            discrepancy_no = dsSupplier.Tables[0].Rows[0]["discrepancy_no"].ToString(),
                            ext_provider_name = /*objGlobalData.GetSupplierNameById*/(dsSupplier.Tables[0].Rows[0]["ext_provider_name"].ToString()),
                            delivery_note_no = dsSupplier.Tables[0].Rows[0]["delivery_note_no"].ToString(),
                            po_no = dsSupplier.Tables[0].Rows[0]["po_no"].ToString(),
                            discre_detail = dsSupplier.Tables[0].Rows[0]["discre_detail"].ToString(),
                            discre_relatedto = (dsSupplier.Tables[0].Rows[0]["discre_relatedto"].ToString()),
                            upload = dsSupplier.Tables[0].Rows[0]["upload"].ToString(),
                            actions = dsSupplier.Tables[0].Rows[0]["actions"].ToString(),
                            impact = dsSupplier.Tables[0].Rows[0]["impact"].ToString(),
                            ncr_required = (dsSupplier.Tables[0].Rows[0]["ncr_required"].ToString()),
                            branch = (dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                            Location = (dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        DateTime dtDocDate;

                        if (dsSupplier.Tables[0].Rows[0]["discre_registerd_date"].ToString() != ""
                               && DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["discre_registerd_date"].ToString(), out dtDocDate))
                        {
                            objSupplierModels.discre_registerd_date = dtDocDate;
                        }
                        if (dsSupplier.Tables[0].Rows[0]["discre_reported_date"].ToString() != ""
                          && DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["discre_reported_date"].ToString(), out dtDocDate))
                        {
                            objSupplierModels.discre_reported_date = dtDocDate;
                        }

                        ViewBag.Branch = objGlobalData.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobalData.GetLocationbyMultiDivision(dsSupplier.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobalData.GetDepartmentList1(dsSupplier.Tables[0].Rows[0]["branch"].ToString());

                        return View(objSupplierModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("DiscrepancyLogList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Discrepancy Id cannot be null";
                    return RedirectToAction("DiscrepancyLogList");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DiscrepancyLogList"); ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DiscrepancyLogEdit(ExtProviderDiscrepencyLogModels objDiscrepencyLogModels, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objDiscrepencyLogModels.branch = form["branch"];
                objDiscrepencyLogModels.Department = form["Department"];
                objDiscrepencyLogModels.Location = form["Location"];

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                objDiscrepencyLogModels.discre_relatedto = form["discre_relatedto"];
                DateTime dateValue;

                if (DateTime.TryParse(form["discre_registerd_date"], out dateValue) == true)
                {
                    objDiscrepencyLogModels.discre_registerd_date = dateValue;
                }
                if (DateTime.TryParse(form["discre_reported_date"], out dateValue) == true)
                {
                    objDiscrepencyLogModels.discre_reported_date = dateValue;
                }

                if (upload != null && files.ContentLength > 0)
                {
                    objDiscrepencyLogModels.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(file.FileName));
                            string sFilename = "Log" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objDiscrepencyLogModels.upload = objDiscrepencyLogModels.upload + "," + "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objDiscrepencyLogModels.upload = objDiscrepencyLogModels.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objDiscrepencyLogModels.upload = objDiscrepencyLogModels.upload + "," + form["QCDocsVal"];
                    objDiscrepencyLogModels.upload = objDiscrepencyLogModels.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objDiscrepencyLogModels.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objDiscrepencyLogModels.upload = null;
                }

                if (objDiscrepencyLogModels.FunUpdateExtProviderDiscrepencyLog(objDiscrepencyLogModels))
                {
                    TempData["Successdata"] = "External Provider Discrepency Log details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DiscrepancyLogList");
        }

        [AllowAnonymous]
        public JsonResult DiscrepancyLogDelete(FormCollection form)
        {
            try
            {
                if (form["id_discrepancylog"] != null && form["id_discrepancylog"] != "")
                {
                    ExtProviderDiscrepencyLogModels Doc = new ExtProviderDiscrepencyLogModels();
                    string sSupplierDiscreLogId = form["id_discrepancylog"];

                    if (Doc.FunDeleteDiscrepencyLogDoc(sSupplierDiscreLogId))
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
                    TempData["alertdata"] = "Discrepency Log Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DiscrepancyLogDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public JsonResult FunGetSupplierScope(string SupplierName)
        {
            try
            {
                if (SupplierName != "")
                {
                    string scope = "";
                    string sSsqlstmt = "Select SupplyScope from t_supplier where SupplierID = '" + SupplierName + "'";
                    DataSet dsList = objGlobalData.Getdetails(sSsqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        scope = dsList.Tables[0].Rows[0]["SupplyScope"].ToString();
                    }
                    return Json(scope);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGetSupplierScope: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }

        [AllowAnonymous]
        public JsonResult DiscrepancyInvalid(string id_discrepancylog, string invalid_reason)
        {
            try
            {
                if (id_discrepancylog != "" && id_discrepancylog != "")
                {
                    ExtProviderDiscrepencyLogModels Doc = new ExtProviderDiscrepencyLogModels();
                    string sid_discrepancylog = id_discrepancylog;
                    if (Doc.FunInvalidSupplierDescp(sid_discrepancylog, invalid_reason))
                    {
                        TempData["Successdata"] = "Updated successfully";
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DiscrepancyInvalid: " + ex.ToString());
                TempData["alertdata"] = objGlobalData.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }
}