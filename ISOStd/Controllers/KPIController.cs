using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.Net;
using PagedList;
using PagedList.Mvc;
using ISOStd.Filters;

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
            try
            {
                ViewBag.Dept = objGlobaldata.GetDepartmentListbox();
                ViewBag.Freq_of_Eval = new string[] { "Monthly", "Quarterly", "Semi Annually", "Annually" };
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddKPIEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View();
        }

        // POST: /KPI/AddKPIEvaluation
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddKPIEvaluation(KPIModels objKPI, FormCollection form)
        {
            try
            {
                if (objKPI != null)
                {
                    DateTime dateValue;

                    if (DateTime.TryParse(form["KPI_Estd_On"], out dateValue) == true)
                    {
                        objKPI.KPI_Estd_On = dateValue;
                    }
                    if (DateTime.TryParse(form["KPI_Status_eval_on"], out dateValue) == true)
                    {
                        objKPI.KPI_Status_eval_on = dateValue;
                    }

                    if (objKPI.FunAddKPI(objKPI))
                    {
                        TempData["Successdata"] = "Added KPI details successfully";
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

            return RedirectToAction("KPIEvaluationList");
        }


        // GET: /KPI/KPIEvaluationList
        
        [AllowAnonymous]
        public ActionResult KPIEvaluationList(int? page)
        {
            KPIModelsList objKPIModelsList = new KPIModelsList();
            objKPIModelsList.KPIMList = new List<KPIModels>();

            try
            {
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select EvalId, Dept, Dept_Head, KPI_Ref, Freq_of_Eval, SubProcess_InCharge, KPI_Estd_On, KPI_No, Key_Perf_Indicator, Measurable_Value,"
                                    + "KPI_Monitoring_Mechanism, KPI_Status_eval_on, KPI_Eval_Record, NCR_Ref  from t_kpievaluation order by EvalId desc";

                DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);
                if(dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0 )
                {
                    for (int i = 0; i < dsKPIModelsList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtKPI_Status_eval_onDate = Convert.ToDateTime(dsKPIModelsList.Tables[0].Rows[i]["KPI_Status_eval_on"].ToString());
                        DateTime dtKPI_Estd_OnDate = Convert.ToDateTime(dsKPIModelsList.Tables[0].Rows[i]["KPI_Estd_On"].ToString());

                        try
                        {
                            KPIModels objKPIModels = new KPIModels
                            {
                                EvalId = Convert.ToInt16(dsKPIModelsList.Tables[0].Rows[i]["EvalId"].ToString()),
                                Dept =objGlobaldata.GetDeptNameById(dsKPIModelsList.Tables[0].Rows[i]["Dept"].ToString()),
                                Dept_Head = dsKPIModelsList.Tables[0].Rows[i]["Dept_Head"].ToString(),
                                KPI_Ref = dsKPIModelsList.Tables[0].Rows[i]["KPI_Ref"].ToString(),
                                Freq_of_Eval = dsKPIModelsList.Tables[0].Rows[i]["Freq_of_Eval"].ToString(),
                                KPI_Estd_On = dtKPI_Estd_OnDate,
                                KPI_No = dsKPIModelsList.Tables[0].Rows[i]["KPI_No"].ToString(),
                                Key_Perf_Indicator = dsKPIModelsList.Tables[0].Rows[i]["Key_Perf_Indicator"].ToString(),
                                Measurable_Value = dsKPIModelsList.Tables[0].Rows[i]["Measurable_Value"].ToString(),
                                KPI_Monitoring_Mechanism = dsKPIModelsList.Tables[0].Rows[i]["KPI_Monitoring_Mechanism"].ToString(),
                                KPI_Eval_Record = dsKPIModelsList.Tables[0].Rows[i]["KPI_Eval_Record"].ToString(),
                                KPI_Status_eval_on = dtKPI_Status_eval_onDate,
                                NCR_Ref = dsKPIModelsList.Tables[0].Rows[i]["NCR_Ref"].ToString(),
                                SubProcess_InCharge = dsKPIModelsList.Tables[0].Rows[i]["SubProcess_InCharge"].ToString()
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
            return View(objKPIModelsList.KPIMList.ToList().ToPagedList(page ?? 1, 40));
        }

        // GET: /KPI/KPIEvaluationDetails
        
        [AllowAnonymous]
        public ActionResult KPIEvaluationDetails()
        {
            KPIModels objKPIModels = new KPIModels();
            try
            {

                if (Request.QueryString["EvalId"] != null && Request.QueryString["EvalId"] != "")
                {
                    string sEvalId = Request.QueryString["EvalId"];

                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    string sSqlstmt = "select EvalId, Dept, Dept_Head, KPI_Ref, Freq_of_Eval, SubProcess_InCharge, KPI_Estd_On, KPI_No, Key_Perf_Indicator, Measurable_Value,"
                                        + "KPI_Monitoring_Mechanism, KPI_Status_eval_on, KPI_Eval_Record, NCR_Ref  from t_kpievaluation where EvalId=" + sEvalId;

                    DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKPIModelsList.Tables.Count >0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtKPI_Status_eval_onDate = Convert.ToDateTime(dsKPIModelsList.Tables[0].Rows[0]["KPI_Status_eval_on"].ToString());
                        DateTime dtKPI_Estd_OnDate = Convert.ToDateTime(dsKPIModelsList.Tables[0].Rows[0]["KPI_Estd_On"].ToString());

                        objKPIModels = new KPIModels
                        {
                            EvalId = Convert.ToInt16(dsKPIModelsList.Tables[0].Rows[0]["EvalId"].ToString()),
                            Dept = dsKPIModelsList.Tables[0].Rows[0]["Dept"].ToString(),
                            Dept_Head = dsKPIModelsList.Tables[0].Rows[0]["Dept_Head"].ToString(),
                            KPI_Ref = dsKPIModelsList.Tables[0].Rows[0]["KPI_Ref"].ToString(),
                            Freq_of_Eval = dsKPIModelsList.Tables[0].Rows[0]["Freq_of_Eval"].ToString(),
                            SubProcess_InCharge = dsKPIModelsList.Tables[0].Rows[0]["SubProcess_InCharge"].ToString(),
                            KPI_Estd_On = dtKPI_Estd_OnDate,
                            KPI_No = dsKPIModelsList.Tables[0].Rows[0]["KPI_No"].ToString(),
                            Key_Perf_Indicator = dsKPIModelsList.Tables[0].Rows[0]["Key_Perf_Indicator"].ToString(),
                            Measurable_Value = dsKPIModelsList.Tables[0].Rows[0]["Measurable_Value"].ToString(),
                            KPI_Monitoring_Mechanism = dsKPIModelsList.Tables[0].Rows[0]["KPI_Monitoring_Mechanism"].ToString(),
                            KPI_Eval_Record = dsKPIModelsList.Tables[0].Rows[0]["KPI_Eval_Record"].ToString(),
                            KPI_Status_eval_on = dtKPI_Status_eval_onDate,
                            NCR_Ref = dsKPIModelsList.Tables[0].Rows[0]["NCR_Ref"].ToString(),
                        };

                    }
                }
                else
                {
                    TempData["alertdata"] = "Evaluation Id cannot be Null or empty";
                    return RedirectToAction("KPIEvaluationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objKPIModels);
        }

        // GET: /KPI/KPIEvaluationEdit
        
        [AllowAnonymous]
        public ActionResult KPIEvaluationEdit()
        {
            KPIModels objKPIModels = new KPIModels();
            try
            {

                if (Request.QueryString["EvalId"] != null && Request.QueryString["EvalId"] != "")
                {
                    string sEvalId = Request.QueryString["EvalId"];
                    ViewBag.EvalId = sEvalId;

                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    string sSqlstmt = "select EvalId, Dept, Dept_Head, KPI_Ref, Freq_of_Eval, SubProcess_InCharge, KPI_Estd_On, KPI_No, Key_Perf_Indicator, Measurable_Value,"
                                        + "KPI_Monitoring_Mechanism, KPI_Status_eval_on, KPI_Eval_Record, NCR_Ref  from t_kpievaluation where EvalId=" + sEvalId;

                    DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtKPI_Status_eval_onDate = Convert.ToDateTime(dsKPIModelsList.Tables[0].Rows[0]["KPI_Status_eval_on"].ToString());
                        DateTime dtKPI_Estd_OnDate = Convert.ToDateTime(dsKPIModelsList.Tables[0].Rows[0]["KPI_Estd_On"].ToString());

                        objKPIModels = new KPIModels
                        {
                            EvalId = Convert.ToInt16(dsKPIModelsList.Tables[0].Rows[0]["EvalId"].ToString()),
                            Dept = dsKPIModelsList.Tables[0].Rows[0]["Dept"].ToString(),
                            Dept_Head = dsKPIModelsList.Tables[0].Rows[0]["Dept_Head"].ToString(),
                            KPI_Ref = dsKPIModelsList.Tables[0].Rows[0]["KPI_Ref"].ToString(),
                            Freq_of_Eval = dsKPIModelsList.Tables[0].Rows[0]["Freq_of_Eval"].ToString(),
                            SubProcess_InCharge = dsKPIModelsList.Tables[0].Rows[0]["SubProcess_InCharge"].ToString(),
                            KPI_Estd_On = dtKPI_Estd_OnDate,
                            KPI_No = dsKPIModelsList.Tables[0].Rows[0]["KPI_No"].ToString(),
                            Key_Perf_Indicator = dsKPIModelsList.Tables[0].Rows[0]["Key_Perf_Indicator"].ToString(),
                            Measurable_Value = dsKPIModelsList.Tables[0].Rows[0]["Measurable_Value"].ToString(),
                            KPI_Monitoring_Mechanism = dsKPIModelsList.Tables[0].Rows[0]["KPI_Monitoring_Mechanism"].ToString(),
                            KPI_Eval_Record = dsKPIModelsList.Tables[0].Rows[0]["KPI_Eval_Record"].ToString(),
                            KPI_Status_eval_on = dtKPI_Status_eval_onDate,
                            NCR_Ref = dsKPIModelsList.Tables[0].Rows[0]["NCR_Ref"].ToString(),
                        };
                    }
                }
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            ViewBag.Dept = objGlobaldata.GetDepartmentListbox();
            ViewBag.Freq_of_Eval = new string[] { "Monthly", "Quarterly", "Semi Annually", "Annually" };

            return View(objKPIModels);
        }

        // POST: /KPI/KPIEvaluationEdit
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KPIEvaluationEdit(KPIModels objKPI, FormCollection form)
        {
            try
            {
                objKPI.EvalId = Convert.ToInt16(form["EvalId"]);

                DateTime dateValue;

                if (DateTime.TryParse(form["KPI_Estd_On"], out dateValue) == true)
                {
                    objKPI.KPI_Estd_On = dateValue;
                }
                if (DateTime.TryParse(form["KPI_Status_eval_on"], out dateValue) == true)
                {
                    objKPI.KPI_Status_eval_on = dateValue;
                }


                if (objKPI.FunUpdateKPI(objKPI))
                {
                    TempData["Successdata"] = "KPI Evaluation details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("KPIEvaluationList");
        }
    }
}
