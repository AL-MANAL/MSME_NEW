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
using ISOStd.Filters;
using Rotativa;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class KPIController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public KPIController()
        {
            ViewBag.Menutype = "ObjKPI";
            ViewBag.SubMenutype = "KPI";
        }

        //
        // GET: /KPI/
       
        public ActionResult Index()
        {
            return View();
        }

        // GET: /KPI/AddKPIEvaluation
        
        [AllowAnonymous]
        public ActionResult AddKPIEvaluation()
        {
            return InitilizeAddKPIEvaluation();
        }

        // GET: /KPI/InitilizeAddKPIEvaluation
        
        private ActionResult InitilizeAddKPIEvaluation()
        {
            KPIModels objKpi = new KPIModels();
            try
            {
                objKpi.branch = objGlobaldata.GetCurrentUserSession().division;
                objKpi.group_name = objGlobaldata.GetCurrentUserSession().DeptID;              
              
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objKpi.branch);       
               
                ViewBag.Perf = objGlobaldata.GetKPIPerformanceIndicatorList();
                ViewBag.Level = objGlobaldata.GetKPILevelList();
                ViewBag.Type = objGlobaldata.GetKPITypeList();
              //  ViewBag.Monitoring = objGlobaldata.GetKPIMonitoringMechanismList();
                ViewBag.Frequency = objGlobaldata.GetKPIFrequencyEvaluationList();
                ViewBag.Status = objGlobaldata.GetKPIStatusList();
                ViewBag.Unit = objGlobaldata.GetConstantValue("KPIUnit");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddKPIEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objKpi);
        }

        // POST: /KPI/AddKPIEvaluation
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddKPIEvaluation(KPIModels objKPI, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                if (objKPI != null)
                {
                    objKPI.pers_resp = form["pers_resp"];
                    IList<HttpPostedFileBase> upload_file = (IList<HttpPostedFileBase>)upload;

                    DateTime dateValue;
                    if (DateTime.TryParse(form["established_date"], out dateValue) == true)
                    {
                        objKPI.established_date = dateValue;
                    }
                   

                    KPIModelsList objModelsList = new KPIModelsList();
                    objModelsList.KPIMList = new List<KPIModels>();

                    if (form["causes_failure"] != "")
                    {
                        objKPI.causes_failure = form["causes_failure"];
                        objModelsList.KPIMList.Add(objKPI);
                    }

                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        KPIModels objModels = new KPIModels();

                        if (form["causes_failure" + i] != null && form["causes_failure" + i] != "")
                        {

                            objModels.causes_failure = form["causes_failure" + i];
                            objModelsList.KPIMList.Add(objModels);
                        }
                    }

                    KPIModelsList objMeasureList = new KPIModelsList();
                    objMeasureList.KPIMList = new List<KPIModels>();

                    for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                    {
                        KPIModels objMeasureModels = new KPIModels();
                      
                        if (form["measurable_indicator " + i] != null && form["measurable_indicator " + i] != "")
                        {
                            objMeasureModels.measurable_indicator = form["measurable_indicator " + i];
                            objMeasureModels.expected_value = form["expected_value " + i];
                            objMeasureModels.expected_value_unit = form["expected_value_unit " + i];
                            objMeasureModels.kpi_type = form["kpi_type " + i];
                            if (DateTime.TryParse(form["monitoring_frm_date " + i], out dateValue) == true)
                            {
                                objMeasureModels.monitoring_frm_date = dateValue;
                            }
                            if (DateTime.TryParse(form["monitoring_to_date " + i], out dateValue) == true)
                            {
                                objMeasureModels.monitoring_to_date = dateValue;
                            }
                            objMeasureModels.monitoring_mechanism = form["monitoring_mechanism " + i];
                            objMeasureModels.frequency_eval = form["frequency_eval " + i];
                            objMeasureModels.risk = form["risk " + i];

                            objMeasureList.KPIMList.Add(objMeasureModels);
                        }
                    }

                    if (upload_file[0] != null)
                    {
                        objKPI.upload = "";
                        foreach (var file in upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/KPI"), Path.GetFileName(file.FileName));
                                string sFilename = "KPI" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objKPI.upload = objKPI.upload + "," + "~/DataUpload/MgmtDocs/KPI/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddKPIEvaluation-upload: " + ex.ToString());
                            }
                        }
                        objKPI.upload = objKPI.upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    objKPI.branch = objGlobaldata.GetCurrentUserSession().division;

                    if (objKPI.FunAddKPI(objKPI, objModelsList, objMeasureList))
                    {
                        TempData["Successdata"] = "Added KPI successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddKPIEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json(true);
        }


        // GET: /KPI/KPIEvaluationList

        [AllowAnonymous]
        public ActionResult KPIEvaluationList(int? page, string branch_name)
        {
            KPIModelsList objKPIModelsList = new KPIModelsList();
            objKPIModelsList.KPIMList = new List<KPIModels>();

            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);

                string sSqlstmt = "select KPI_Id,kpi_ref_no,established_date,branch,group_name,process_indicator,"
                       + "kpi_level,process_monitor,pers_resp,upload,"
                       + "approved_by,kpi_status,status_reason,kpiapprv_status  from t_kpi where Active=1";
                string sSearchtext = "";
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                    ViewBag.Branch_name = sBranch_name;
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by KPI_Id desc";
                DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsKPIModelsList.Tables[0].Rows.Count; i++)
                    {
                       
                        try
                        {
                            KPIModels objKPIModels = new KPIModels
                            {
                                KPI_Id = (dsKPIModelsList.Tables[0].Rows[i]["KPI_Id"].ToString()),
                                kpi_ref_no = (dsKPIModelsList.Tables[0].Rows[i]["kpi_ref_no"].ToString()),
                                branch = objGlobaldata.GetCompanyBranchNameById(dsKPIModelsList.Tables[0].Rows[i]["branch"].ToString()),
                                group_name = objGlobaldata.GetDeptNameById(dsKPIModelsList.Tables[0].Rows[i]["group_name"].ToString()),
                                process_indicator =objGlobaldata.GetKPIPerformanceIndicatorById(dsKPIModelsList.Tables[0].Rows[i]["process_indicator"].ToString()),
                                kpi_level =objGlobaldata.GetKPILevelById(dsKPIModelsList.Tables[0].Rows[i]["kpi_level"].ToString()),
                                process_monitor = (dsKPIModelsList.Tables[0].Rows[i]["process_monitor"].ToString()),
                                kpiapprv_status = (dsKPIModelsList.Tables[0].Rows[i]["kpiapprv_status"].ToString()),

                            };
                            objKPIModelsList.KPIMList.Add(objKPIModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objKPIModelsList.KPIMList.ToList());
        }

        [AllowAnonymous]
        public JsonResult KPIEvaluationListSearch(int? page, string branch_name)
        {
            KPIModelsList objKPIModelsList = new KPIModelsList();
            objKPIModelsList.KPIMList = new List<KPIModels>();

            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);

                string sSqlstmt = "select KPI_Id,kpi_ref_no,established_date,branch,group_name,process_indicator,"
                       + "kpi_level,process_monitor,measurable_indicator"
                       + ",pers_resp,upload,"
                       + "approved_by,kpi_status,status_reason  from t_kpi where Active=1";
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
                sSqlstmt = sSqlstmt + sSearchtext + " order by KPI_Id desc";
                DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsKPIModelsList.Tables[0].Rows.Count; i++)
                    {

                        try
                        {
                            KPIModels objKPIModels = new KPIModels
                            {
                                KPI_Id = (dsKPIModelsList.Tables[0].Rows[i]["KPI_Id"].ToString()),
                                kpi_ref_no = (dsKPIModelsList.Tables[0].Rows[i]["kpi_ref_no"].ToString()),
                                branch = objGlobaldata.GetCompanyBranchNameById(dsKPIModelsList.Tables[0].Rows[i]["branch"].ToString()),
                                group_name = objGlobaldata.GetDeptNameById(dsKPIModelsList.Tables[0].Rows[i]["group_name"].ToString()),
                                process_indicator = objGlobaldata.GetKPIPerformanceIndicatorById(dsKPIModelsList.Tables[0].Rows[i]["process_indicator"].ToString()),
                                kpi_level = objGlobaldata.GetKPILevelById(dsKPIModelsList.Tables[0].Rows[i]["kpi_level"].ToString()),
                                process_monitor = (dsKPIModelsList.Tables[0].Rows[i]["process_monitor"].ToString()),
                                //measurable_indicator = (dsKPIModelsList.Tables[0].Rows[i]["measurable_indicator"].ToString()),
                                //kpi_type = objGlobaldata.GetKPITypeById(dsKPIModelsList.Tables[0].Rows[i]["kpi_type"].ToString()),
                                //monitoring_mechanism = objGlobaldata.GetKPIMonitoringMechanismById(dsKPIModelsList.Tables[0].Rows[i]["monitoring_mechanism"].ToString()),
                                //frequency_eval = objGlobaldata.GetKPIFrequencyEvaluationById(dsKPIModelsList.Tables[0].Rows[i]["frequency_eval"].ToString()),
                            };
                            objKPIModelsList.KPIMList.Add(objKPIModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        //// GET: /KPI/KPIEvaluationDetails

        //[AllowAnonymous]
        //public ActionResult KPIEvaluationDetails()
        //{
        //    KPIModels objKPIModels = new KPIModels();
        //    try
        //    {

        //        if (Request.QueryString["EvalId"] != null && Request.QueryString["EvalId"] != "")
        //        {
        //            string sEvalId = Request.QueryString["EvalId"];

        //            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

        //            string sSqlstmt = "select EvalId, Dept, Dept_Head, KPI_Ref, Freq_of_Eval, SubProcess_InCharge, KPI_Estd_On, KPI_No, Key_Perf_Indicator, Measurable_Value,"
        //                                + "KPI_Monitoring_Mechanism, KPI_Status_eval_on, KPI_Eval_Record, NCR_Ref  from t_kpievaluation where EvalId=" + sEvalId;

        //            DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsKPIModelsList.Tables.Count >0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
        //            {
        //                DateTime dtKPI_Status_eval_onDate = Convert.ToDateTime(dsKPIModelsList.Tables[0].Rows[0]["KPI_Status_eval_on"].ToString());
        //                DateTime dtKPI_Estd_OnDate = Convert.ToDateTime(dsKPIModelsList.Tables[0].Rows[0]["KPI_Estd_On"].ToString());

        //                objKPIModels = new KPIModels
        //                {
        //                    EvalId = Convert.ToInt16(dsKPIModelsList.Tables[0].Rows[0]["EvalId"].ToString()),
        //                    Dept = dsKPIModelsList.Tables[0].Rows[0]["Dept"].ToString(),
        //                    Dept_Head = dsKPIModelsList.Tables[0].Rows[0]["Dept_Head"].ToString(),
        //                    KPI_Ref = dsKPIModelsList.Tables[0].Rows[0]["KPI_Ref"].ToString(),
        //                    Freq_of_Eval = dsKPIModelsList.Tables[0].Rows[0]["Freq_of_Eval"].ToString(),
        //                    SubProcess_InCharge = dsKPIModelsList.Tables[0].Rows[0]["SubProcess_InCharge"].ToString(),
        //                    KPI_Estd_On = dtKPI_Estd_OnDate,
        //                    KPI_No = dsKPIModelsList.Tables[0].Rows[0]["KPI_No"].ToString(),
        //                    Key_Perf_Indicator = dsKPIModelsList.Tables[0].Rows[0]["Key_Perf_Indicator"].ToString(),
        //                    Measurable_Value = dsKPIModelsList.Tables[0].Rows[0]["Measurable_Value"].ToString(),
        //                    KPI_Monitoring_Mechanism = dsKPIModelsList.Tables[0].Rows[0]["KPI_Monitoring_Mechanism"].ToString(),
        //                    KPI_Eval_Record = dsKPIModelsList.Tables[0].Rows[0]["KPI_Eval_Record"].ToString(),
        //                    KPI_Status_eval_on = dtKPI_Status_eval_onDate,
        //                    NCR_Ref = dsKPIModelsList.Tables[0].Rows[0]["NCR_Ref"].ToString(),
        //                };

        //            }
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = "Evaluation Id cannot be Null or empty";
        //            return RedirectToAction("KPIEvaluationList");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationDetails: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View(objKPIModels);
        //}

        // GET: /KPI/KPIEvaluationEdit

        [AllowAnonymous]
        public ActionResult KPIEvaluationEdit()
        {
            KPIModels objKPIModels = new KPIModels();
            try
            {

                if (Request.QueryString["KPI_Id"] != null && Request.QueryString["KPI_Id"] != "")
                {
                    string KPI_Id = Request.QueryString["KPI_Id"];

                    string sSqlstmt = "select KPI_Id,kpi_ref_no,established_date,branch,group_name,process_indicator,"
                        +"kpi_level,process_monitor,pers_resp,upload,"
                        + "approved_by,kpi_status,status_reason  from t_kpi where KPI_Id=" + KPI_Id;

                    DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
                    {
                       
                        objKPIModels = new KPIModels
                        {
                            KPI_Id = (dsKPIModelsList.Tables[0].Rows[0]["KPI_Id"].ToString()),
                            kpi_ref_no = (dsKPIModelsList.Tables[0].Rows[0]["kpi_ref_no"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsKPIModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            group_name = objGlobaldata.GetDeptNameById(dsKPIModelsList.Tables[0].Rows[0]["group_name"].ToString()),
                            process_indicator = (dsKPIModelsList.Tables[0].Rows[0]["process_indicator"].ToString()),
                            kpi_level = (dsKPIModelsList.Tables[0].Rows[0]["kpi_level"].ToString()),
                            process_monitor = (dsKPIModelsList.Tables[0].Rows[0]["process_monitor"].ToString()),
                            //measurable_indicator = (dsKPIModelsList.Tables[0].Rows[0]["measurable_indicator"].ToString()),
                            //expected_value = (dsKPIModelsList.Tables[0].Rows[0]["expected_value"].ToString()),
                            //expected_value_unit = (dsKPIModelsList.Tables[0].Rows[0]["expected_value_unit"].ToString()),
                            //kpi_type = (dsKPIModelsList.Tables[0].Rows[0]["kpi_type"].ToString()),
                            //monitoring_mechanism = (dsKPIModelsList.Tables[0].Rows[0]["monitoring_mechanism"].ToString()),
                            //frequency_eval = (dsKPIModelsList.Tables[0].Rows[0]["frequency_eval"].ToString()),
                            //risk = (dsKPIModelsList.Tables[0].Rows[0]["risk"].ToString()),
                            pers_resp = (dsKPIModelsList.Tables[0].Rows[0]["pers_resp"].ToString()),
                            upload = (dsKPIModelsList.Tables[0].Rows[0]["upload"].ToString()),
                            approved_by = (dsKPIModelsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            kpi_status = (dsKPIModelsList.Tables[0].Rows[0]["kpi_status"].ToString()),
                            status_reason = (dsKPIModelsList.Tables[0].Rows[0]["status_reason"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["established_date"].ToString(), out dtValue))
                        {
                            objKPIModels.established_date = dtValue;
                        }
                       
                        //if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["monitoring_frm_date"].ToString(), out dtValue))
                        //{
                        //    objKPIModels.monitoring_frm_date = dtValue;
                        //}
                      
                        //if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["monitoring_to_date"].ToString(), out dtValue))
                        //{
                        //    objKPIModels.monitoring_to_date = dtValue;
                        //}

                        ViewBag.Perf = objGlobaldata.GetKPIPerformanceIndicatorList();
                        ViewBag.Level = objGlobaldata.GetKPILevelList();
                        ViewBag.Type = objGlobaldata.GetKPITypeList();
                        ViewBag.Monitoring = objGlobaldata.GetKPIMonitoringMechanismList();
                        ViewBag.Frequency = objGlobaldata.GetKPIFrequencyEvaluationList();
                        ViewBag.Status = objGlobaldata.GetKPIStatusList();
                        ViewBag.Unit = objGlobaldata.GetConstantValue("KPIUnit");
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                        KPIModelsList objModelsList = new KPIModelsList();
                        objModelsList.KPIMList = new List<KPIModels>();

                        sSqlstmt = "select id_kpi_failure,KPI_Id,causes_failure from t_kpi_failure where KPI_Id='" + objKPIModels.KPI_Id + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    KPIModels objRisk = new KPIModels
                                    {
                                        id_kpi_failure = dsList.Tables[0].Rows[i]["id_kpi_failure"].ToString(),
                                        KPI_Id = dsList.Tables[0].Rows[i]["KPI_Id"].ToString(),
                                        causes_failure = dsList.Tables[0].Rows[i]["causes_failure"].ToString(),
                                    };
                                    objModelsList.KPIMList.Add(objRisk);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("KPIEvaluationList");
                                }
                            }
                            ViewBag.objList = objModelsList;
                        }

                        KPIModelsList objMIList = new KPIModelsList();
                        objMIList.KPIMList = new List<KPIModels>();

                        string sql2 = "select id_measurable,KPI_Id,measurable_indicator,expected_value,expected_value_unit,kpi_type,"
                       + "monitoring_frm_date,monitoring_to_date,monitoring_mechanism,frequency_eval,risk from t_kpi_measurable_indicator where KPI_Id=" + KPI_Id;
                        DataSet dsMIList = objGlobaldata.Getdetails(sql2);
                        if (dsMIList.Tables.Count > 0 && dsMIList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsMIList.Tables[0].Rows.Count; i++)
                            {
                                KPIModels objMIModels = new KPIModels
                                {
                                    id_measurable = (dsMIList.Tables[0].Rows[i]["id_measurable"].ToString()),
                                    measurable_indicator = (dsMIList.Tables[0].Rows[i]["measurable_indicator"].ToString()),
                                    expected_value = (dsMIList.Tables[0].Rows[i]["expected_value"].ToString()),
                                    expected_value_unit = (dsMIList.Tables[0].Rows[i]["expected_value_unit"].ToString()),
                                    kpi_type = (dsMIList.Tables[0].Rows[i]["kpi_type"].ToString()),
                                    monitoring_mechanism = (dsMIList.Tables[0].Rows[i]["monitoring_mechanism"].ToString()),
                                    frequency_eval = (dsMIList.Tables[0].Rows[i]["frequency_eval"].ToString()),
                                    risk = (dsMIList.Tables[0].Rows[i]["risk"].ToString()),
                                   
                                };
                                DateTime dtDocDate;
                                if (dsMIList.Tables[0].Rows[i]["monitoring_frm_date"].ToString() != ""
                              && DateTime.TryParse(dsMIList.Tables[0].Rows[i]["monitoring_frm_date"].ToString(), out dtDocDate))
                                {
                                    objMIModels.monitoring_frm_date = dtDocDate;
                                }
                                if (dsMIList.Tables[0].Rows[i]["monitoring_to_date"].ToString() != ""
                             && DateTime.TryParse(dsMIList.Tables[0].Rows[i]["monitoring_to_date"].ToString(), out dtDocDate))
                                {
                                    objMIModels.monitoring_to_date = dtDocDate;
                                }
                                objMIList.KPIMList.Add(objMIModels);
                            }
                            ViewBag.MIList = objMIList;
                        }
                    }
                   
                }
               

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
          
            return View(objKPIModels);
        }

        // POST: /KPI/KPIEvaluationEdit

        [HttpPost]
        [AllowAnonymous]
        public ActionResult KPIEvaluationEdit(KPIModels objKPI, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                if (objKPI != null)
                {
                    objKPI.pers_resp = form["pers_resp"];
                    HttpPostedFileBase files = Request.Files[0];
                    string QCDelete = Request.Form["QCDocsValselectall"];

                    DateTime dateValue;
                    if (DateTime.TryParse(form["established_date"], out dateValue) == true)
                    {
                        objKPI.established_date = dateValue;
                    }
                  

                    KPIModelsList objModelsList = new KPIModelsList();
                    objModelsList.KPIMList = new List<KPIModels>();


                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        KPIModels objModels = new KPIModels();
                        if (form["causes_failure" + i] != null && form["causes_failure" + i] != "")
                        {
                            objModels.causes_failure = form["causes_failure" + i];
                            objModelsList.KPIMList.Add(objModels);
                        }
                    }
                    KPIModelsList objMeasureList = new KPIModelsList();
                    objMeasureList.KPIMList = new List<KPIModels>();

                    for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                    {
                        KPIModels objMeasureModels = new KPIModels();

                        if (form["measurable_indicator " + i] != null && form["measurable_indicator " + i] != "")
                        {
                            objMeasureModels.id_measurable = form["id_measurable " + i];
                            objMeasureModels.measurable_indicator = form["measurable_indicator " + i];
                            objMeasureModels.expected_value = form["expected_value " + i];
                            objMeasureModels.expected_value_unit = form["expected_value_unit " + i];
                            objMeasureModels.kpi_type = form["kpi_type " + i];
                            if (DateTime.TryParse(form["monitoring_frm_date " + i], out dateValue) == true)
                            {
                                objMeasureModels.monitoring_frm_date = dateValue;
                            }
                            if (DateTime.TryParse(form["monitoring_to_date " + i], out dateValue) == true)
                            {
                                objMeasureModels.monitoring_to_date = dateValue;
                            }
                            objMeasureModels.monitoring_mechanism = form["monitoring_mechanism " + i];
                            objMeasureModels.frequency_eval = form["frequency_eval " + i];
                            objMeasureModels.risk = form["risk " + i];

                            objMeasureList.KPIMList.Add(objMeasureModels);
                        }
                    }

                    if (upload != null && files.ContentLength > 0)
                    {
                        objKPI.upload = "";
                        foreach (var file in upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/KPI"), Path.GetFileName(file.FileName));
                                string sFilename = "KPI" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objKPI.upload = objKPI.upload + "," + "~/DataUpload/MgmtDocs/KPI/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationEdit-upload: " + ex.ToString());
                            }
                        }
                        objKPI.upload = objKPI.upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                    if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                    {
                        objKPI.upload = objKPI.upload + "," + form["QCDocsVal"];
                        objKPI.upload = objKPI.upload.Trim(',');
                    }
                    else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                    {
                        objKPI.upload = null;
                    }
                    else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                    {
                        objKPI.upload = null;
                    }

                    if (objKPI.FunUpdateKPI(objKPI, objModelsList, objMeasureList))
                    {
                        TempData["Successdata"] = "Updated KPI successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [AllowAnonymous]
        public JsonResult KPIDelete(FormCollection form)
        {
            try
            {
                if (form["Id"] != null && form["Id"] != "")
                {
                    KPIModels objModel = new KPIModels();
                    string sId = form["Id"];

                    if (objModel.FunDeleteKPI(sId))
                    {
                        TempData["Successdata"] = "Deleted successfully";
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult AddActionPlan()
        {

            KPIModels objKPI = new KPIModels();

            try
            {

                if (Request.QueryString["KPI_Id"] != null)
                {
                    string KPI_Id = Request.QueryString["KPI_Id"];
                    string sSqlstmt = "select KPI_Id,kpi_ref_no,process_monitor,action_upload  from t_kpi where KPI_Id='" + KPI_Id + "'";

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objKPI = new KPIModels
                        {

                            KPI_Id = dsModelsList.Tables[0].Rows[0]["KPI_Id"].ToString(),
                            kpi_ref_no = dsModelsList.Tables[0].Rows[0]["kpi_ref_no"].ToString(),
                            process_monitor = dsModelsList.Tables[0].Rows[0]["process_monitor"].ToString(),
                          
                            action_upload = dsModelsList.Tables[0].Rows[0]["action_upload"].ToString(),
                        };
                      

                        KPIModelsList objList = new KPIModelsList();
                        objList.KPIMList = new List<KPIModels>();

                        sSqlstmt = "select id_kpi_actions,KPI_Id,Action,TargetDate,PersonResponsible,remarks"
                       + " from t_kpi_actions where KPI_Id='" + KPI_Id + "' order by id_kpi_actions";
                        DataSet dsActnList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsActnList.Tables.Count > 0 && dsActnList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsActnList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    KPIModels objActn = new KPIModels
                                    {
                                        id_kpi_actions = (dsActnList.Tables[0].Rows[i]["id_kpi_actions"].ToString()),
                                        KPI_Id = (dsActnList.Tables[0].Rows[i]["KPI_Id"].ToString()),
                                        Action = dsActnList.Tables[0].Rows[i]["Action"].ToString(),
                                        PersonResponsible = dsActnList.Tables[0].Rows[i]["PersonResponsible"].ToString(),
                                        remarks = dsActnList.Tables[0].Rows[i]["remarks"].ToString(),
                                    };
                                    DateTime dtDocDate;
                                    if (dsActnList.Tables[0].Rows[i]["TargetDate"].ToString() != ""
                                       && DateTime.TryParse(dsActnList.Tables[0].Rows[i]["TargetDate"].ToString(), out dtDocDate))
                                    {
                                        objActn.TargetDate = dtDocDate;
                                    }
                                    objList.KPIMList.Add(objActn);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddActionPlan: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("KPIEvaluationList");
                                }
                            }
                            ViewBag.objAttnList = objList;
                        }
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();                  
                        return View(objKPI);
                    }
                    else
                    {

                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("KPIEvaluationList");

                    }
                }
                else
                {

                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("KPIEvaluationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddActionPlan: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("KPIEvaluationList");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddActionPlan(KPIModels objKPI, FormCollection form, IEnumerable<HttpPostedFileBase> action_upload)
        {
            try
            {

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (action_upload != null && files.ContentLength > 0)
                {
                    objKPI.action_upload = "";
                    foreach (var file in action_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/KPI"), Path.GetFileName(file.FileName));
                            string sFilename = "KPI" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objKPI.action_upload = objKPI.action_upload + "," + "~/DataUpload/MgmtDocs/KPI/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddActionPlan-upload: " + ex.ToString());
                        }
                    }
                    objKPI.action_upload = objKPI.action_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objKPI.action_upload = objKPI.action_upload + "," + form["QCDocsVal"];
                    objKPI.action_upload = objKPI.action_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objKPI.action_upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objKPI.action_upload = null;
                }

                KPIModelsList objList = new KPIModelsList();
                objList.KPIMList = new List<KPIModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    KPIModels objkpiModels = new KPIModels();
                    DateTime dateValue;
                    if (form["Action" + i] != null && form["Action" + i] != "")
                    {
                        if (DateTime.TryParse(form["TargetDate" + i], out dateValue) == true)
                        {
                            objkpiModels.TargetDate = dateValue;
                        }

                        objkpiModels.Action = form["Action" + i];
                        objkpiModels.PersonResponsible = form["PersonResponsible" + i];
                        objkpiModels.remarks = form["remarks" + i];
                        objList.KPIMList.Add(objkpiModels);
                    }
                }

              

                if (objKPI.FunAddActionPlan(objKPI, objList))
                {
                    TempData["Successdata"] = "Action Plan Added Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddActionPlan: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("KPIEvaluationList");
        }


        [AllowAnonymous]
        public ActionResult AddEvaluation(int? page)
        {
            KPIModelsList objKPIModelsList = new KPIModelsList();
            objKPIModelsList.KPIMList = new List<KPIModels>();

            try
            {
                if(Request.QueryString["KPI_Id"] !=null && Request.QueryString["KPI_Id"] != "")
                {
                    string KPI_Id = Request.QueryString["KPI_Id"];
                    string id_measurable = Request.QueryString["id_measurable"];
                    string sSqlstmt = "select id_measurable,KPI_Id,kpi_ref_no,measurable_indicator,expected_value,expected_value_unit,kpi_type,monitoring_frm_date,monitoring_to_date,monitoring_mechanism,frequency_eval,risk"
                    + " from t_kpi_measurable_indicator where KPI_Id='"+ KPI_Id + "'";

                    ViewBag.Unit= objGlobaldata.GetConstantValue("KPIUnit");
                    ViewBag.Status = objGlobaldata.GetKPIEvaluationStatusList();
                    ViewBag.YesNo= objGlobaldata.GetConstantValue("YesNo");

                    DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsKPIModelsList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                KPIModels objKPIModels = new KPIModels
                                {
                                    id_measurable = (dsKPIModelsList.Tables[0].Rows[i]["id_measurable"].ToString()),
                                    KPI_Id = (dsKPIModelsList.Tables[0].Rows[i]["KPI_Id"].ToString()),
                                    kpi_ref_no = (dsKPIModelsList.Tables[0].Rows[i]["kpi_ref_no"].ToString()),
                                    measurable_indicator = (dsKPIModelsList.Tables[0].Rows[i]["measurable_indicator"].ToString()),
                                    expected_value = (dsKPIModelsList.Tables[0].Rows[i]["expected_value"].ToString()),
                                    expected_value_unit = (dsKPIModelsList.Tables[0].Rows[i]["expected_value_unit"].ToString()),
                                    kpi_type =objGlobaldata.GetKPITypeById(dsKPIModelsList.Tables[0].Rows[i]["kpi_type"].ToString()),
                                    monitoring_mechanism =/*objGlobaldata.GetKPIMonitoringMechanismById*/(dsKPIModelsList.Tables[0].Rows[i]["monitoring_mechanism"].ToString()),
                                    frequency_eval =objGlobaldata.GetKPIFrequencyEvaluationById(dsKPIModelsList.Tables[0].Rows[i]["frequency_eval"].ToString()),
                                    risk = (dsKPIModelsList.Tables[0].Rows[i]["risk"].ToString()),
                                };
                                DateTime dtDocDate;
                                if (dsKPIModelsList.Tables[0].Rows[i]["monitoring_frm_date"].ToString() != ""
                                   && DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[i]["monitoring_frm_date"].ToString(), out dtDocDate))
                                {
                                    objKPIModels.monitoring_frm_date = dtDocDate;
                                }
                                if (dsKPIModelsList.Tables[0].Rows[i]["monitoring_to_date"].ToString() != ""
                                  && DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[i]["monitoring_to_date"].ToString(), out dtDocDate))
                                {
                                    objKPIModels.monitoring_to_date = dtDocDate;
                                }
                                objKPIModelsList.KPIMList.Add(objKPIModels);

                              
                               
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddEvaluation: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }

                        string sql1 = "select kpi_ref_no,process_monitor from t_kpi where KPI_Id='" + KPI_Id + "'";
                        DataSet dsKPIList = objGlobaldata.Getdetails(sql1);
                        if (dsKPIList.Tables.Count > 0 && dsKPIList.Tables[0].Rows.Count > 0)
                        {
                            KPIModels objModels = new KPIModels
                            {
                                kpi_ref_no = (dsKPIList.Tables[0].Rows[0]["kpi_ref_no"].ToString()),
                                process_monitor = (dsKPIList.Tables[0].Rows[0]["process_monitor"].ToString()),

                            };
                            ViewBag.KPI = objModels;
                        }


                        string sql2 = "select id_measurable,KPI_Id,expected_value,monitoring_frm_date,monitoring_to_date from t_kpi_measurable_indicator where id_measurable='" + id_measurable + "'";
                        DataSet dsMIList = objGlobaldata.Getdetails(sql2);
                        if (dsMIList.Tables.Count > 0 && dsMIList.Tables[0].Rows.Count > 0)
                        {
                            KPIModels objMIModels = new KPIModels
                            {
                                id_measurable = (dsMIList.Tables[0].Rows[0]["id_measurable"].ToString()),

                                KPI_Id = (dsMIList.Tables[0].Rows[0]["KPI_Id"].ToString()),

                                expected_value = (dsMIList.Tables[0].Rows[0]["expected_value"].ToString()),

                            };
                            DateTime dtDocDate;
                            if (dsMIList.Tables[0].Rows[0]["monitoring_frm_date"].ToString() != ""
                          && DateTime.TryParse(dsMIList.Tables[0].Rows[0]["monitoring_frm_date"].ToString(), out dtDocDate))
                            {
                                objMIModels.monitoring_frm_date = dtDocDate;
                            }
                            if (dsMIList.Tables[0].Rows[0]["monitoring_to_date"].ToString() != ""
                         && DateTime.TryParse(dsMIList.Tables[0].Rows[0]["monitoring_to_date"].ToString(), out dtDocDate))
                            {
                                objMIModels.monitoring_to_date = dtDocDate;
                            }
                            ViewBag.MI = objMIModels;
                        }


                        string sql = "select id_evaluation,KPI_Id,id_measurable,evaluation_date,source_evaluate,upload,"
                                + "method_evaluation,value_achieved,unit,kpi_status,remarks,raise_need from t_kpi_evaluation"
                                + " where  id_measurable='" + id_measurable + "'";
                        DataSet dsEvalList = objGlobaldata.Getdetails(sql);
                        if (dsEvalList.Tables.Count > 0 && dsEvalList.Tables[0].Rows.Count > 0)
                        {
                            KPIModels objEvalModels = new KPIModels
                            {
                                id_evaluation = (dsEvalList.Tables[0].Rows[0]["id_evaluation"].ToString()),
                                KPI_Id = (dsEvalList.Tables[0].Rows[0]["KPI_Id"].ToString()),
                                id_measurable = (dsEvalList.Tables[0].Rows[0]["id_measurable"].ToString()),
                                source_evaluate = (dsEvalList.Tables[0].Rows[0]["source_evaluate"].ToString()),
                                upload = (dsEvalList.Tables[0].Rows[0]["upload"].ToString()),
                                method_evaluation = (dsEvalList.Tables[0].Rows[0]["method_evaluation"].ToString()),
                                value_achieved = (dsEvalList.Tables[0].Rows[0]["value_achieved"].ToString()),
                                unit = (dsEvalList.Tables[0].Rows[0]["unit"].ToString()),
                                kpi_status = (dsEvalList.Tables[0].Rows[0]["kpi_status"].ToString()),
                                remarks = (dsEvalList.Tables[0].Rows[0]["remarks"].ToString()),
                                raise_need = (dsEvalList.Tables[0].Rows[0]["raise_need"].ToString()),
                            };
                            DateTime dtDocDate;
                            if (dsEvalList.Tables[0].Rows[0]["evaluation_date"].ToString() != ""
                          && DateTime.TryParse(dsEvalList.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtDocDate))
                            {
                                objEvalModels.evaluation_date = dtDocDate;
                            }
                            ViewBag.Eval = objEvalModels;
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id Cannot be null";
                }            
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objKPIModelsList.KPIMList.ToList());
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddEvaluation(KPIModels objKPI, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (upload != null && files.ContentLength > 0)
                {
                    objKPI.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/KPI"), Path.GetFileName(file.FileName));
                            string sFilename = "KPI" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objKPI.upload = objKPI.upload + "," + "~/DataUpload/MgmtDocs/KPI/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddEvaluation-upload: " + ex.ToString());
                        }
                    }
                    objKPI.upload = objKPI.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objKPI.upload = objKPI.upload + "," + form["QCDocsVal"];
                    objKPI.upload = objKPI.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objKPI.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objKPI.upload = null;
                }            
                if(objKPI.id_evaluation != null)
                {
                    if (objKPI.FunUpdateKPIEvaluation(objKPI))
                    {
                        TempData["Successdata"] = "Evaluation Added Successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    if (objKPI.FunAddKPIEvaluation(objKPI))
                    {
                        TempData["Successdata"] = "Evaluation Added Successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AddEvaluation", new { KPI_Id= objKPI.KPI_Id });
        }


        [AllowAnonymous]
        public ActionResult KPIEvaluationDetails()
        {
            KPIModels objKPIModels = new KPIModels();
            try
            {

                if (Request.QueryString["KPI_Id"] != null && Request.QueryString["KPI_Id"] != "")
                {
                    string KPI_Id = Request.QueryString["KPI_Id"];

                    string sSqlstmt = "select KPI_Id,kpi_ref_no,established_date,branch,group_name,process_indicator,"
                        + "kpi_level,process_monitor,pers_resp,upload,"
                        + "(CASE WHEN kpiapprv_status='0' THEN 'Pending for Approval' WHEN kpiapprv_status='1' THEN 'Rejected' ELSE 'Approved' END) as kpiapprv_status,"
                        + "approved_by,kpi_status,status_reason,approvedby_comments,ApproveOrRejectOn  from t_kpi where KPI_Id=" + KPI_Id;

                    DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
                    {

                        objKPIModels = new KPIModels
                        {
                            KPI_Id = (dsKPIModelsList.Tables[0].Rows[0]["KPI_Id"].ToString()),
                            kpi_ref_no = (dsKPIModelsList.Tables[0].Rows[0]["kpi_ref_no"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsKPIModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            group_name = objGlobaldata.GetDeptNameById(dsKPIModelsList.Tables[0].Rows[0]["group_name"].ToString()),
                            process_indicator =objGlobaldata.GetKPIPerformanceIndicatorById(dsKPIModelsList.Tables[0].Rows[0]["process_indicator"].ToString()),
                            kpi_level =objGlobaldata.GetKPILevelById(dsKPIModelsList.Tables[0].Rows[0]["kpi_level"].ToString()),
                            process_monitor = (dsKPIModelsList.Tables[0].Rows[0]["process_monitor"].ToString()),
                            //measurable_indicator = (dsKPIModelsList.Tables[0].Rows[0]["measurable_indicator"].ToString()),
                            //expected_value = (dsKPIModelsList.Tables[0].Rows[0]["expected_value"].ToString()),
                            //expected_value_unit = (dsKPIModelsList.Tables[0].Rows[0]["expected_value_unit"].ToString()),
                            //kpi_type = (dsKPIModelsList.Tables[0].Rows[0]["kpi_type"].ToString()),
                            //monitoring_mechanism = (dsKPIModelsList.Tables[0].Rows[0]["monitoring_mechanism"].ToString()),
                            //frequency_eval = (dsKPIModelsList.Tables[0].Rows[0]["frequency_eval"].ToString()),
                            //risk = (dsKPIModelsList.Tables[0].Rows[0]["risk"].ToString()),
                            pers_resp =objGlobaldata.GetMultiHrEmpNameById(dsKPIModelsList.Tables[0].Rows[0]["pers_resp"].ToString()),
                            upload = (dsKPIModelsList.Tables[0].Rows[0]["upload"].ToString()),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsKPIModelsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            kpi_status =objGlobaldata.GetKPIStatusById(dsKPIModelsList.Tables[0].Rows[0]["kpi_status"].ToString()),
                            status_reason = (dsKPIModelsList.Tables[0].Rows[0]["status_reason"].ToString()),
                            kpiapprv_status = (dsKPIModelsList.Tables[0].Rows[0]["kpiapprv_status"].ToString()),
                            approvedby_comments = (dsKPIModelsList.Tables[0].Rows[0]["approvedby_comments"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["established_date"].ToString(), out dtValue))
                        {
                            objKPIModels.established_date = dtValue;
                        }
                        if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["ApproveOrRejectOn"].ToString(), out dtValue))
                        {
                            objKPIModels.ApproveOrRejectOn = dtValue;
                        }
                        //if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["monitoring_frm_date"].ToString(), out dtValue))
                        //{
                        //    objKPIModels.monitoring_frm_date = dtValue;
                        //}

                        //if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["monitoring_to_date"].ToString(), out dtValue))
                        //{
                        //    objKPIModels.monitoring_to_date = dtValue;
                        //}

                        sSqlstmt = "select id_kpi_failure,KPI_Id,causes_failure from t_kpi_failure where KPI_Id='" + objKPIModels.KPI_Id + "'";
                        ViewBag.objList = objGlobaldata.Getdetails(sSqlstmt);

                        string sql2 = "select kpi_ref_no,measurable_indicator,expected_value,expected_value_unit,kpi_type,monitoring_frm_date,monitoring_to_date,monitoring_mechanism," +
                        "frequency_eval,risk,evaluation_date,source_evaluate,upload,method_evaluation,value_achieved,unit,kpi_status,remarks,raise_need "+
                        "from t_kpi_measurable_indicator t left join t_kpi_evaluation tt on t.id_measurable = tt.id_measurable where t.KPI_Id ='"+ objKPIModels.KPI_Id + "'";
                        ViewBag.MIList = objGlobaldata.Getdetails(sql2);

                        string sql3 = "select Action,TargetDate,PersonResponsible,remarks from t_kpi_actions where KPI_Id='" + objKPIModels.KPI_Id + "'";
                        ViewBag.ActionList = objGlobaldata.Getdetails(sql3);

                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objKPIModels);
        }

        [AllowAnonymous]
        public ActionResult KPIEvaluationApprvDetails()
        {
            KPIModels objKPIModels = new KPIModels();
            try
            {
                if (Request.QueryString["KPI_Id"] != null && Request.QueryString["KPI_Id"] != "")
                {
                    string KPI_Id = Request.QueryString["KPI_Id"];

                    string sSqlstmt = "select KPI_Id,kpi_ref_no,established_date,branch,group_name,process_indicator,"
                        + "kpi_level,process_monitor,pers_resp,upload,"
                        + "approved_by,kpi_status,status_reason from t_kpi where KPI_Id=" + KPI_Id;

                    DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
                    {

                        objKPIModels = new KPIModels
                        {
                            KPI_Id = (dsKPIModelsList.Tables[0].Rows[0]["KPI_Id"].ToString()),
                            kpi_ref_no = (dsKPIModelsList.Tables[0].Rows[0]["kpi_ref_no"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsKPIModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            group_name = objGlobaldata.GetDeptNameById(dsKPIModelsList.Tables[0].Rows[0]["group_name"].ToString()),
                            process_indicator = objGlobaldata.GetKPIPerformanceIndicatorById(dsKPIModelsList.Tables[0].Rows[0]["process_indicator"].ToString()),
                            kpi_level = objGlobaldata.GetKPILevelById(dsKPIModelsList.Tables[0].Rows[0]["kpi_level"].ToString()),
                            process_monitor = (dsKPIModelsList.Tables[0].Rows[0]["process_monitor"].ToString()),
                           
                            pers_resp = objGlobaldata.GetEmpHrNameById(dsKPIModelsList.Tables[0].Rows[0]["pers_resp"].ToString()),
                            upload = (dsKPIModelsList.Tables[0].Rows[0]["upload"].ToString()),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsKPIModelsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            kpi_status = (dsKPIModelsList.Tables[0].Rows[0]["kpi_status"].ToString()),
                            status_reason = (dsKPIModelsList.Tables[0].Rows[0]["status_reason"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["established_date"].ToString(), out dtValue))
                        {
                            objKPIModels.established_date = dtValue;
                        }

                       

                        sSqlstmt = "select id_kpi_failure,KPI_Id,causes_failure from t_kpi_failure where KPI_Id='" + objKPIModels.KPI_Id + "'";
                        ViewBag.objList = objGlobaldata.Getdetails(sSqlstmt);

                        string sql2 = "select kpi_ref_no,measurable_indicator,expected_value,expected_value_unit,kpi_type,monitoring_frm_date,monitoring_to_date,monitoring_mechanism," +
                        "frequency_eval,risk,evaluation_date,source_evaluate,upload,method_evaluation,value_achieved,unit,kpi_status,remarks,raise_need " +
                        "from t_kpi_measurable_indicator t left join t_kpi_evaluation tt on t.id_measurable = tt.id_measurable where t.KPI_Id ='" + objKPIModels.KPI_Id + "'";
                        ViewBag.MIList = objGlobaldata.Getdetails(sql2);

                        string sql3 = "select Action,TargetDate,PersonResponsible,remarks from t_kpi_actions where KPI_Id='" + objKPIModels.KPI_Id + "'";
                        ViewBag.ActionList = objGlobaldata.Getdetails(sql3);

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationApprvDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objKPIModels);
        }

        public ActionResult KPIApprove(KPIModels objKPI, FormCollection form)
        {
            try
            {

                if (objKPI.FunUpdateApprove(objKPI))
                {
                    TempData["Successdata"] = "Approved Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }


        [AllowAnonymous]
        public ActionResult KPIReportPDF(FormCollection form)
        {
            KPIModels objKPIModels = new KPIModels();
            try
            {

                string KPI_Id = form["KPI_Id"];
                if (KPI_Id != null && KPI_Id != "")
                {


                    string sSqlstmt = "select KPI_Id,kpi_ref_no,established_date,branch,group_name,process_indicator,"
                        + "kpi_level,process_monitor,pers_resp,upload,"
                        + "(CASE WHEN kpiapprv_status='0' THEN 'Pending for Approval' WHEN kpiapprv_status='1' THEN 'Rejected' ELSE 'Approved' END) as kpiapprv_status,"
                        + "approved_by,kpi_status,status_reason,approvedby_comments,ApproveOrRejectOn  from t_kpi where KPI_Id=" + KPI_Id;

                    DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
                    {

                        objKPIModels = new KPIModels
                        {
                            KPI_Id = (dsKPIModelsList.Tables[0].Rows[0]["KPI_Id"].ToString()),
                            kpi_ref_no = (dsKPIModelsList.Tables[0].Rows[0]["kpi_ref_no"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsKPIModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            group_name = objGlobaldata.GetDeptNameById(dsKPIModelsList.Tables[0].Rows[0]["group_name"].ToString()),
                            process_indicator = objGlobaldata.GetKPIPerformanceIndicatorById(dsKPIModelsList.Tables[0].Rows[0]["process_indicator"].ToString()),
                            kpi_level = objGlobaldata.GetKPILevelById(dsKPIModelsList.Tables[0].Rows[0]["kpi_level"].ToString()),
                            process_monitor = (dsKPIModelsList.Tables[0].Rows[0]["process_monitor"].ToString()),
                            //measurable_indicator = (dsKPIModelsList.Tables[0].Rows[0]["measurable_indicator"].ToString()),
                            //expected_value = (dsKPIModelsList.Tables[0].Rows[0]["expected_value"].ToString()),
                            //expected_value_unit = (dsKPIModelsList.Tables[0].Rows[0]["expected_value_unit"].ToString()),
                            //kpi_type = (dsKPIModelsList.Tables[0].Rows[0]["kpi_type"].ToString()),
                            //monitoring_mechanism = (dsKPIModelsList.Tables[0].Rows[0]["monitoring_mechanism"].ToString()),
                            //frequency_eval = (dsKPIModelsList.Tables[0].Rows[0]["frequency_eval"].ToString()),
                            //risk = (dsKPIModelsList.Tables[0].Rows[0]["risk"].ToString()),
                            pers_resp = objGlobaldata.GetEmpHrNameById(dsKPIModelsList.Tables[0].Rows[0]["pers_resp"].ToString()),
                            upload = (dsKPIModelsList.Tables[0].Rows[0]["upload"].ToString()),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsKPIModelsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            kpi_status = objGlobaldata.GetKPIStatusById(dsKPIModelsList.Tables[0].Rows[0]["kpi_status"].ToString()),
                            status_reason = (dsKPIModelsList.Tables[0].Rows[0]["status_reason"].ToString()),
                            kpiapprv_status = (dsKPIModelsList.Tables[0].Rows[0]["kpiapprv_status"].ToString()),
                            approvedby_comments = (dsKPIModelsList.Tables[0].Rows[0]["approvedby_comments"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["established_date"].ToString(), out dtValue))
                        {
                            objKPIModels.established_date = dtValue;
                        }
                        if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["ApproveOrRejectOn"].ToString(), out dtValue))
                        {
                            objKPIModels.ApproveOrRejectOn = dtValue;
                        }

                        ViewBag.KPI = objKPIModels;

                        sSqlstmt = "select id_kpi_failure,KPI_Id,causes_failure from t_kpi_failure where KPI_Id='" + objKPIModels.KPI_Id + "'";
                        ViewBag.objList = objGlobaldata.Getdetails(sSqlstmt);

                        string sql2 = "select kpi_ref_no,measurable_indicator,expected_value,expected_value_unit,kpi_type,monitoring_frm_date,monitoring_to_date,monitoring_mechanism," +
                        "frequency_eval,risk,evaluation_date,source_evaluate,upload,method_evaluation,value_achieved,unit,kpi_status,remarks,raise_need " +
                        "from t_kpi_measurable_indicator t left join t_kpi_evaluation tt on t.id_measurable = tt.id_measurable where t.KPI_Id ='" + objKPIModels.KPI_Id + "'";
                        ViewBag.MIList = objGlobaldata.Getdetails(sql2);

                        string sql3 = "select Action,TargetDate,PersonResponsible,remarks from t_kpi_actions where KPI_Id='" + objKPIModels.KPI_Id + "'";
                        ViewBag.ActionList = objGlobaldata.Getdetails(sql3);

                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIReportPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string header = Server.MapPath("~/views/KPI/PrintHeader.html");//Path of PrintHeader.html File

            string customSwitches = string.Format("--header-html  \"{0}\" " +
                                "--header-spacing \"0\" " + 
            " --header-font-size \"10\" ", header);

            return new ViewAsPdf("KPIReportPDF", "KPI")
            {
                //FileName = "KPIReport.pdf",
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
        public ActionResult KPIEvaluationPDF(FormCollection form)
        {
            KPIModels objKPIModels = new KPIModels();
            try
            {

                string KPI_Id = form["KPI_Id"];
                if (KPI_Id != null && KPI_Id != "")
                {


                    string sSqlstmt = "select KPI_Id,kpi_ref_no,established_date,branch,group_name,process_indicator,"
                        + "kpi_level,process_monitor,pers_resp,upload,"
                        + "(CASE WHEN kpiapprv_status='0' THEN 'Pending for Approval' WHEN kpiapprv_status='1' THEN 'Rejected' ELSE 'Approved' END) as kpiapprv_status,"
                        + "approved_by,kpi_status,status_reason,approvedby_comments,ApproveOrRejectOn  from t_kpi where KPI_Id=" + KPI_Id;

                    DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
                    {

                        objKPIModels = new KPIModels
                        {
                            KPI_Id = (dsKPIModelsList.Tables[0].Rows[0]["KPI_Id"].ToString()),
                            kpi_ref_no = (dsKPIModelsList.Tables[0].Rows[0]["kpi_ref_no"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsKPIModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            group_name = objGlobaldata.GetDeptNameById(dsKPIModelsList.Tables[0].Rows[0]["group_name"].ToString()),
                            process_indicator = objGlobaldata.GetKPIPerformanceIndicatorById(dsKPIModelsList.Tables[0].Rows[0]["process_indicator"].ToString()),
                            kpi_level = objGlobaldata.GetKPILevelById(dsKPIModelsList.Tables[0].Rows[0]["kpi_level"].ToString()),
                            process_monitor = (dsKPIModelsList.Tables[0].Rows[0]["process_monitor"].ToString()),
                            //measurable_indicator = (dsKPIModelsList.Tables[0].Rows[0]["measurable_indicator"].ToString()),
                            //expected_value = (dsKPIModelsList.Tables[0].Rows[0]["expected_value"].ToString()),
                            //expected_value_unit = (dsKPIModelsList.Tables[0].Rows[0]["expected_value_unit"].ToString()),
                            //kpi_type = (dsKPIModelsList.Tables[0].Rows[0]["kpi_type"].ToString()),
                            //monitoring_mechanism = (dsKPIModelsList.Tables[0].Rows[0]["monitoring_mechanism"].ToString()),
                            //frequency_eval = (dsKPIModelsList.Tables[0].Rows[0]["frequency_eval"].ToString()),
                            //risk = (dsKPIModelsList.Tables[0].Rows[0]["risk"].ToString()),
                            pers_resp = objGlobaldata.GetEmpHrNameById(dsKPIModelsList.Tables[0].Rows[0]["pers_resp"].ToString()),
                            upload = (dsKPIModelsList.Tables[0].Rows[0]["upload"].ToString()),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsKPIModelsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            kpi_status = objGlobaldata.GetKPIStatusById(dsKPIModelsList.Tables[0].Rows[0]["kpi_status"].ToString()),
                            status_reason = (dsKPIModelsList.Tables[0].Rows[0]["status_reason"].ToString()),
                            kpiapprv_status = (dsKPIModelsList.Tables[0].Rows[0]["kpiapprv_status"].ToString()),
                            approvedby_comments = (dsKPIModelsList.Tables[0].Rows[0]["approvedby_comments"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["established_date"].ToString(), out dtValue))
                        {
                            objKPIModels.established_date = dtValue;
                        }
                        if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["ApproveOrRejectOn"].ToString(), out dtValue))
                        {
                            objKPIModels.ApproveOrRejectOn = dtValue;
                        }

                        ViewBag.KPI = objKPIModels;

                        sSqlstmt = "select id_kpi_failure,KPI_Id,causes_failure from t_kpi_failure where KPI_Id='" + objKPIModels.KPI_Id + "'";
                        ViewBag.objList = objGlobaldata.Getdetails(sSqlstmt);

                        string sql2 = "select kpi_ref_no,measurable_indicator,expected_value,expected_value_unit,kpi_type,monitoring_frm_date,monitoring_to_date,monitoring_mechanism," +
                        "frequency_eval,risk,evaluation_date,source_evaluate,upload,method_evaluation,value_achieved,unit,kpi_status,remarks,raise_need " +
                        "from t_kpi_measurable_indicator t left join t_kpi_evaluation tt on t.id_measurable = tt.id_measurable where t.KPI_Id ='" + objKPIModels.KPI_Id + "'";
                        ViewBag.MIList = objGlobaldata.Getdetails(sql2);

                        string sql3 = "select Action,TargetDate,PersonResponsible,remarks from t_kpi_actions where KPI_Id='" + objKPIModels.KPI_Id + "'";
                        ViewBag.ActionList = objGlobaldata.Getdetails(sql3);

                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string header = Server.MapPath("~/views/KPI/PrintHeaderEvaluation.html");//Path of PrintHeader.html File

            string customSwitches = string.Format("--header-html  \"{0}\" " +
                                "--header-spacing \"0\" " +
            " --header-font-size \"10\" ", header);

            return new ViewAsPdf("KPIEvaluationPDF", "KPI")
            {
                //FileName = "KPIEvaluationReport.pdf",
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
        public ActionResult AddPotentialCauses()
        {
            KPIModels objKPIModels = new KPIModels();
            try
            {

                if (Request.QueryString["KPI_Id"] != null && Request.QueryString["KPI_Id"] != "")
                {
                    string KPI_Id = Request.QueryString["KPI_Id"];

                    string sSqlstmt = "select KPI_Id,kpi_ref_no,established_date,branch,group_name,process_indicator,"
                        + "kpi_level,process_monitor,pers_resp,upload,"
                        + "approved_by,kpi_status,status_reason  from t_kpi where KPI_Id=" + KPI_Id;

                    DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
                    {

                        objKPIModels = new KPIModels
                        {
                            KPI_Id = (dsKPIModelsList.Tables[0].Rows[0]["KPI_Id"].ToString()),
                            kpi_ref_no = (dsKPIModelsList.Tables[0].Rows[0]["kpi_ref_no"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsKPIModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            group_name = objGlobaldata.GetDeptNameById(dsKPIModelsList.Tables[0].Rows[0]["group_name"].ToString()),
                            process_indicator = (dsKPIModelsList.Tables[0].Rows[0]["process_indicator"].ToString()),
                            kpi_level =objGlobaldata.GetDropdownitemById(dsKPIModelsList.Tables[0].Rows[0]["kpi_level"].ToString()),
                            process_monitor = (dsKPIModelsList.Tables[0].Rows[0]["process_monitor"].ToString()),
                            //measurable_indicator = (dsKPIModelsList.Tables[0].Rows[0]["measurable_indicator"].ToString()),
                            //expected_value = (dsKPIModelsList.Tables[0].Rows[0]["expected_value"].ToString()),
                            //expected_value_unit = (dsKPIModelsList.Tables[0].Rows[0]["expected_value_unit"].ToString()),
                            //kpi_type = (dsKPIModelsList.Tables[0].Rows[0]["kpi_type"].ToString()),
                            //monitoring_mechanism = (dsKPIModelsList.Tables[0].Rows[0]["monitoring_mechanism"].ToString()),
                            //frequency_eval = (dsKPIModelsList.Tables[0].Rows[0]["frequency_eval"].ToString()),
                            //risk = (dsKPIModelsList.Tables[0].Rows[0]["risk"].ToString()),
                            pers_resp = (dsKPIModelsList.Tables[0].Rows[0]["pers_resp"].ToString()),
                            upload = (dsKPIModelsList.Tables[0].Rows[0]["upload"].ToString()),
                            approved_by = (dsKPIModelsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            kpi_status = (dsKPIModelsList.Tables[0].Rows[0]["kpi_status"].ToString()),
                            status_reason = (dsKPIModelsList.Tables[0].Rows[0]["status_reason"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["established_date"].ToString(), out dtValue))
                        {
                            objKPIModels.established_date = dtValue;
                        }

                        //if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["monitoring_frm_date"].ToString(), out dtValue))
                        //{
                        //    objKPIModels.monitoring_frm_date = dtValue;
                        //}

                        //if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["monitoring_to_date"].ToString(), out dtValue))
                        //{
                        //    objKPIModels.monitoring_to_date = dtValue;
                        //}

                        ViewBag.Status = objGlobaldata.GetKPIPotentialStatusList();
                        ViewBag.impact = objGlobaldata.GetKPIPotentialImpactList();

                        KPIModelsList objModelsList = new KPIModelsList();
                        objModelsList.KPIMList = new List<KPIModels>();

                        sSqlstmt = "select id_kpi_failure,KPI_Id,causes_failure,impact,mitigation_measures,target_date,failure_status,status_updated_date from t_kpi_failure where KPI_Id='" + objKPIModels.KPI_Id + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    KPIModels objRisk = new KPIModels
                                    {
                                        id_kpi_failure = dsList.Tables[0].Rows[i]["id_kpi_failure"].ToString(),
                                        KPI_Id = dsList.Tables[0].Rows[i]["KPI_Id"].ToString(),
                                        causes_failure = dsList.Tables[0].Rows[i]["causes_failure"].ToString(),
                                        impact = dsList.Tables[0].Rows[i]["impact"].ToString(),
                                        mitigation_measures = dsList.Tables[0].Rows[i]["mitigation_measures"].ToString(),
                                        failure_status = dsList.Tables[0].Rows[i]["failure_status"].ToString(),
                                      
                                    };                    
                                    if (DateTime.TryParse(dsList.Tables[0].Rows[i]["target_date"].ToString(), out dtValue))
                                    {
                                        objRisk.target_date = dtValue;
                                    }
                                    if (DateTime.TryParse(dsList.Tables[0].Rows[i]["status_updated_date"].ToString(), out dtValue))
                                    {
                                        objRisk.status_updated_date = dtValue;
                                    }
                                    objModelsList.KPIMList.Add(objRisk);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddPotentialCauses: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("KPIEvaluationList");
                                }
                            }
                            ViewBag.objList = objModelsList;
                        }

                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPotentialCauses: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objKPIModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddPotentialCauses(KPIModels objKPI, FormCollection form)
        {
            try
            {
               

                    KPIModelsList objModelsList = new KPIModelsList();
                    objModelsList.KPIMList = new List<KPIModels>();

                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        KPIModels objModels = new KPIModels();
                        if (form["causes_failure" + i] != null && form["causes_failure" + i] != "")
                        {
                        objModels.causes_failure = form["causes_failure" + i];
                        objModels.impact = form["impact" + i];
                        objModels.mitigation_measures = form["mitigation_measures" + i];
                        objModels.failure_status = form["failure_status" + i];
                        DateTime dateValue;
                        if (DateTime.TryParse(form["target_date" + i], out dateValue) == true)
                        {
                            objModels.target_date = dateValue;
                        }
                        if (DateTime.TryParse(form["status_updated_date" + i], out dateValue) == true)
                        {
                            objModels.status_updated_date = dateValue;
                        }
                        objModelsList.KPIMList.Add(objModels);
                        }
                    }

                if (objKPI.FunAddPotentialCauses(objModelsList))
                {
                    TempData["Successdata"] = "Added Potential Causes successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPotentialCauses: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("KPIEvaluationList");
        }


    }
}
