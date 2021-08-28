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

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class KPIEvaluationController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public KPIEvaluationController()
        {
            ViewBag.Menutype = "ObjKPI";
            ViewBag.SubMenutype = "KPI";
        }
        //
        // GET: /KPIEvaluation/
        
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AddKPIEvaluation()
        {
            ViewBag.SubMenutype = "KPI";
            return InitilizeAddKPIEvaluation();
        }
 
        private ActionResult InitilizeAddKPIEvaluation()
        {
            KPIEvaluationModels objkpi = new KPIEvaluationModels();

            try
            {
                ViewBag.SubMenutype = "KPI";

                objkpi.branch = objGlobaldata.GetCurrentUserSession().division;
                objkpi.DeptId = objGlobaldata.GetCurrentUserSession().DeptID;
                objkpi.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(objkpi.branch, objkpi.DeptId, objkpi.Location);

                //ViewBag.DeptId = objGlobaldata.GetDepartmentListbox();
                ViewBag.Freq_of_Eval = objkpi.GetFrequencyList();
               // ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objkpi.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objkpi.branch);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddKPIEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objkpi);
        }
                 
        [HttpPost]
        public JsonResult doesObjRefExist(string KPI_Ref)
        {
            KPIEvaluationModels objKPI = new KPIEvaluationModels();
            var KPI = objKPI.checkObjRefExists(KPI_Ref);

            return Json(KPI);
        }
                 
        [AllowAnonymous]
        public JsonResult KPIEvaluationDocDelete(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "KPI";

                if (form["KPI_Id"] != null && form["KPI_Id"] != "")
                        {

                            KPIEvaluationModels Doc = new KPIEvaluationModels();
                            string sKPI_Id = form["KPI_Id"];


                            if (Doc.FunDeleteKPIEvalDoc(sKPI_Id))
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
                        TempData["alertdata"] = "Access Denied";
                        return Json("Access Denied");
                    }
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
         
        public ActionResult FunGetDeptHeadList(string DeptId)
        {
            MultiSelectList lstEmp = objGlobaldata.GetDeptHeadList(DeptId);
            return Json(lstEmp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddKPIEvaluation(KPIEvaluationModels objKPIEvaluationModels, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "KPI";
                //if (objKPIEvaluationModels != null)
                {
                    objKPIEvaluationModels.DeptId = form["DeptId"];
                    objKPIEvaluationModels.Freq_of_Eval = form["Freq_of_Eval"];
                    objKPIEvaluationModels.Person_Responsible = form["Person_Responsible"];
                    objKPIEvaluationModels.branch = form["branch"];
                    objKPIEvaluationModels.Location = form["Location"];

                    KPIEvaluationModelsList objKPIModelsList = new KPIEvaluationModelsList();
                    objKPIModelsList.lstKPIEvaluation = new List<KPIEvaluationModels>();

                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        KPIEvaluationModels objKPIModels = new KPIEvaluationModels();

                        DateTime dateValue;

                        if (DateTime.TryParse(form["EstablishedOn" + i], out dateValue) == true)
                        {
                            objKPIModels.EstablishedOn = dateValue;
                        }

                        objKPIModels.Process_Indicator = form["Process_Indicator" + i];
                        objKPIModels.Measurable_Value = form["Measurable_Value" + i];
                        objKPIModels.Monitoring_Mechanism = form["Monitoring_Mechanism" + i];
                        objKPIModels.Freq_of_Eval = form["Freq_of_Eval" + i];
                        objKPIModelsList.lstKPIEvaluation.Add(objKPIModels);
                    }

                    if (objKPIEvaluationModels.FunAddKPI(objKPIEvaluationModels, objKPIModelsList))
                    {

                        TempData["Successdata"] = "Added KPI details successfully  with Reference Number '" + objKPIEvaluationModels.KPI_Ref + "'"; 
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
         
        [AllowAnonymous]
        public ActionResult KPIEvaluationList(string SearchText, string DeptId, int? page, string branch_name)
        {
            KPIEvaluationModelsList objKPIEvaluationModels = new KPIEvaluationModelsList();
            objKPIEvaluationModels.lstKPIEvaluation = new List<KPIEvaluationModels>();
            KPIEvaluationModels objkpi = new KPIEvaluationModels();
            try
            {
                ViewBag.SubMenutype = "KPI";

                ViewBag.DeptId = objGlobaldata.GetDepartmentListbox();
                ViewBag.Freq_of_Eval = objGlobaldata.GetConstantValue("Freq_of_Eval");
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select distinct year(EstablishedOn) as EstYear,t.KPI_Id, DeptId, Person_Responsible, KPI_Ref,branch,Location from t_kpi t,t_kpi_trans tt"
                +" where t.KPI_Id=tt.KPI_Id and  Active=1";
                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = " (KPI_Ref ='" + SearchText + "' or KPI_Ref like '" + SearchText + "%')";
                }

                if (sSearchtext != "")
                {
                    sSqlstmt = sSqlstmt + " and";
                }

                if (DeptId != null && DeptId != "Select")
                {
                    ViewBag.DeptIdVal = DeptId;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (DeptId ='" + DeptId + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and (DeptId ='" + DeptId + "')";
                    }
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

                sSqlstmt = sSqlstmt + sSearchtext + " order by EstYear desc, KPI_Ref asc";

                DataSet dsKPIEvaluationModels = objGlobaldata.Getdetails(sSqlstmt);

                if (dsKPIEvaluationModels.Tables.Count > 0 && dsKPIEvaluationModels.Tables[0].Rows.Count > 0)
                {                   
                    for (int i = 0; i < dsKPIEvaluationModels.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            KPIEvaluationModels objObjectivesModels = new KPIEvaluationModels
                            {
                                KPI_Id = Convert.ToInt16(dsKPIEvaluationModels.Tables[0].Rows[i]["KPI_Id"].ToString()),
                                DeptId = objGlobaldata.GetMultiDeptNameById(dsKPIEvaluationModels.Tables[0].Rows[i]["DeptId"].ToString()),
                                Person_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsKPIEvaluationModels.Tables[0].Rows[i]["Person_Responsible"].ToString()),
                                KPI_Ref = dsKPIEvaluationModels.Tables[0].Rows[i]["KPI_Ref"].ToString(),
                                EstYear= dsKPIEvaluationModels.Tables[0].Rows[i]["EstYear"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsKPIEvaluationModels.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsKPIEvaluationModels.Tables[0].Rows[i]["Location"].ToString()),
                            };
                            
                            objKPIEvaluationModels.lstKPIEvaluation.Add(objObjectivesModels);
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

            
            return View(objKPIEvaluationModels.lstKPIEvaluation.ToList());
        }

        [AllowAnonymous]
        public JsonResult KPIEvaluationListSearch(string SearchText, string DeptId, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "KPI";

            KPIEvaluationModelsList objKPIEvaluationModels = new KPIEvaluationModelsList();
            objKPIEvaluationModels.lstKPIEvaluation = new List<KPIEvaluationModels>();
            KPIEvaluationModels objkpi = new KPIEvaluationModels();
            try
            {
                ViewBag.DeptId = objGlobaldata.GetDepartmentListbox();
                ViewBag.Freq_of_Eval = objGlobaldata.GetConstantValue("Freq_of_Eval");
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select distinct year(EstablishedOn) as EstYear,t.KPI_Id, DeptId, Person_Responsible, KPI_Ref from t_kpi t,t_kpi_trans tt"
                + " where t.KPI_Id=tt.KPI_Id and  Active=1";
                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = " (KPI_Ref ='" + SearchText + "' or KPI_Ref like '" + SearchText + "%')";
                }

                if (sSearchtext != "")
                {
                    sSqlstmt = sSqlstmt + " and";
                }

                if (DeptId != null && DeptId != "Select")
                {
                    ViewBag.DeptIdVal = DeptId;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (DeptId ='" + DeptId + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and (DeptId ='" + DeptId + "')";
                    }
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by EstYear desc, KPI_Ref asc";

                DataSet dsKPIEvaluationModels = objGlobaldata.Getdetails(sSqlstmt);

                if (dsKPIEvaluationModels.Tables.Count > 0 && dsKPIEvaluationModels.Tables[0].Rows.Count > 0)
                {                

                    for (int i = 0; i < dsKPIEvaluationModels.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            KPIEvaluationModels objObjectivesModels = new KPIEvaluationModels
                            {
                                KPI_Id = Convert.ToInt16(dsKPIEvaluationModels.Tables[0].Rows[i]["KPI_Id"].ToString()),
                                DeptId = objGlobaldata.GetMultiDeptNameById(dsKPIEvaluationModels.Tables[0].Rows[i]["DeptId"].ToString()),
                                Person_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsKPIEvaluationModels.Tables[0].Rows[i]["Person_Responsible"].ToString()),
                                KPI_Ref = dsKPIEvaluationModels.Tables[0].Rows[i]["KPI_Ref"].ToString(),
                                EstYear = dsKPIEvaluationModels.Tables[0].Rows[i]["EstYear"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsKPIEvaluationModels.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsKPIEvaluationModels.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            objKPIEvaluationModels.lstKPIEvaluation.Add(objObjectivesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
        
        [AllowAnonymous]
        public ActionResult KPIEvaluationDetails()
        {
            KPIEvaluationModels objKPIEvaluationModels = new KPIEvaluationModels();
            ViewBag.SubMenutype = "KPI";
            try
            {
                if (Request.QueryString["KPI_Id"] != null && Request.QueryString["KPI_Id"] != "")
                {
                    string sKPI_Id = Request.QueryString["KPI_Id"];
                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select KPI_Id, DeptId, Person_Responsible, KPI_Ref, Freq_of_Eval,branch,Location from t_kpi where KPI_Id='" + sKPI_Id + "'";

                    DataSet dsKPIEvaluationModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKPIEvaluationModels.Tables.Count > 0 && dsKPIEvaluationModels.Tables[0].Rows.Count > 0)
                    {
                        objKPIEvaluationModels = new KPIEvaluationModels
                        {
                            KPI_Id = Convert.ToInt16(dsKPIEvaluationModels.Tables[0].Rows[0]["KPI_Id"].ToString()),
                            DeptId = objGlobaldata.GetMultiDeptNameById(dsKPIEvaluationModels.Tables[0].Rows[0]["DeptId"].ToString()),
                            Freq_of_Eval =objKPIEvaluationModels.GetFrequencyNameById(dsKPIEvaluationModels.Tables[0].Rows[0]["Freq_of_Eval"].ToString()),
                            Person_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsKPIEvaluationModels.Tables[0].Rows[0]["Person_Responsible"].ToString()),
                            KPI_Ref = dsKPIEvaluationModels.Tables[0].Rows[0]["KPI_Ref"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsKPIEvaluationModels.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsKPIEvaluationModels.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        sSqlstmt = "select KPI_Trans_Id, KPI_Id, EstablishedOn, Process_Indicator, Measurable_Value, Monitoring_Mechanism,Freq_of_Eval,target_date from t_kpi_trans where KPI_Id='"
                            + sKPI_Id + "'";
                        ViewBag.dsKpiTrans = objGlobaldata.Getdetails(sSqlstmt);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("KPIEvaluationList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "KPI Id cannot be Null or empty";
                    return RedirectToAction("KPIEvaluationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objKPIEvaluationModels);
        }
                 
        [AllowAnonymous]
        public ActionResult KPIEvaluationInfo(int id)
        {
            KPIEvaluationModels objKPIEvaluationModels = new KPIEvaluationModels();
            ViewBag.SubMenutype = "KPI";
            try
            {                
                    string sSqlstmt = "select KPI_Id, DeptId, Person_Responsible, KPI_Ref, Freq_of_Eval,branch,Location from t_kpi where KPI_Id='" + id + "'";

                    DataSet dsKPIEvaluationModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKPIEvaluationModels.Tables.Count > 0 && dsKPIEvaluationModels.Tables[0].Rows.Count > 0)
                    {
                        objKPIEvaluationModels = new KPIEvaluationModels
                        {
                            KPI_Id = Convert.ToInt16(dsKPIEvaluationModels.Tables[0].Rows[0]["KPI_Id"].ToString()),
                            DeptId = objGlobaldata.GetMultiDeptNameById(dsKPIEvaluationModels.Tables[0].Rows[0]["DeptId"].ToString()),
                            Freq_of_Eval = objKPIEvaluationModels.GetFrequencyNameById(dsKPIEvaluationModels.Tables[0].Rows[0]["Freq_of_Eval"].ToString()),
                            Person_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsKPIEvaluationModels.Tables[0].Rows[0]["Person_Responsible"].ToString()),
                            KPI_Ref = dsKPIEvaluationModels.Tables[0].Rows[0]["KPI_Ref"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsKPIEvaluationModels.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsKPIEvaluationModels.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        sSqlstmt = "select KPI_Trans_Id, KPI_Id, EstablishedOn, Process_Indicator, Measurable_Value, Monitoring_Mechanism,Freq_of_Eval,target_date from t_kpi_trans where KPI_Id='"
                            + id + "'";
                        ViewBag.dsKpiTrans = objGlobaldata.Getdetails(sSqlstmt);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("KPIEvaluationList");
                    }
               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objKPIEvaluationModels);
        }
                 
        [AllowAnonymous]
        public ActionResult KPIEvaluationEdit()
        {
            KPIEvaluationModels objKPIEvaluationModels = new KPIEvaluationModels();
            ViewBag.SubMenutype = "KPI";
            try
            {
                if (Request.QueryString["KPI_Id"] != null && Request.QueryString["KPI_Id"] != "")
                {
                    string sKPI_Id = Request.QueryString["KPI_Id"];
                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select KPI_Id, DeptId, Person_Responsible, KPI_Ref, Freq_of_Eval,branch,Location from t_kpi where KPI_Id='" + sKPI_Id + "'";

                    DataSet dsKPIEvaluationModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKPIEvaluationModels.Tables.Count > 0 && dsKPIEvaluationModels.Tables[0].Rows.Count > 0)
                    {
                        objKPIEvaluationModels = new KPIEvaluationModels
                        {
                            KPI_Id = Convert.ToInt16(dsKPIEvaluationModels.Tables[0].Rows[0]["KPI_Id"].ToString()),
                            DeptId = /*objGlobaldata.GetMultiDeptNameById*/(dsKPIEvaluationModels.Tables[0].Rows[0]["DeptId"].ToString()),
                            Freq_of_Eval =objKPIEvaluationModels.GetFrequencyNameById(dsKPIEvaluationModels.Tables[0].Rows[0]["Freq_of_Eval"].ToString()),
                            Person_Responsible = (dsKPIEvaluationModels.Tables[0].Rows[0]["Person_Responsible"].ToString()),
                            KPI_Ref = dsKPIEvaluationModels.Tables[0].Rows[0]["KPI_Ref"].ToString(),
                            branch =/* objGlobaldata.GetMultiCompanyBranchNameById*/(dsKPIEvaluationModels.Tables[0].Rows[0]["branch"].ToString()),
                            Location =/* objGlobaldata.GetDivisionLocationById*/(dsKPIEvaluationModels.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsKPIEvaluationModels.Tables[0].Rows[0]["branch"].ToString(), dsKPIEvaluationModels.Tables[0].Rows[0]["DeptId"].ToString(), dsKPIEvaluationModels.Tables[0].Rows[0]["Location"].ToString());
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsKPIEvaluationModels.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsKPIEvaluationModels.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Person_ResponsibleVal = dsKPIEvaluationModels.Tables[0].Rows[0]["Person_Responsible"].ToString();
                        ViewBag.Freq_of_Eval = objKPIEvaluationModels.GetFrequencyList();
                        //ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                        sSqlstmt = "select KPI_Trans_Id, KPI_Id, EstablishedOn, Process_Indicator, Measurable_Value, Monitoring_Mechanism,Freq_of_Eval,target_date from t_kpi_trans where KPI_Id='"
                            + sKPI_Id + "'";
                        ViewBag.dsKpiTrans = objGlobaldata.Getdetails(sSqlstmt);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("KPIEvaluationList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "KPI Id cannot be Null or empty";
                    return RedirectToAction("KPIEvaluationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objKPIEvaluationModels);
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KPIEvaluationEdit(KPIEvaluationModels objKPIEvaluation, FormCollection form)
        {
            ViewBag.SubMenutype = "KPI";
            if (Request["button"].Equals("Update"))
            {
                objKPIEvaluation.DeptId = form["DeptId"];
                objKPIEvaluation.Freq_of_Eval = form["Freq_of_Eval"];
                objKPIEvaluation.Person_Responsible = form["Person_Responsible"];
                objKPIEvaluation.branch = form["branch"];
                objKPIEvaluation.Location = form["Location"];

                if (objKPIEvaluation.FunUpdateKPI(objKPIEvaluation))
                {
                    TempData["Successdata"] = "KPI details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            else if (Request["button"].Equals("Save"))
            {              

                if (KPIPlanAdd(objKPIEvaluation, form))
                {
                    TempData["Successdata"] = "Added KPI plan successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            else
            {

                if (KPIPlanUpdate(objKPIEvaluation, form))
                {
                    TempData["Successdata"] = "KPI plan updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            return RedirectToAction("KPIEvaluationEdit", new { KPI_Id = objKPIEvaluation.KPI_Id });
        }


        //POST: /KPIEvaluation/KPIPlanUpdate
         
        [AllowAnonymous]
        public bool KPIPlanUpdate(KPIEvaluationModels objKPIEvaluation, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "KPI";

                DateTime dateValue;

                if (DateTime.TryParse(form["EstablishedOn"], out dateValue) == true)
                {
                    objKPIEvaluation.EstablishedOn = dateValue;
                }

                objKPIEvaluation.Process_Indicator = form["Process_Indicator"];
                objKPIEvaluation.Measurable_Value = form["Measurable_Value"];
                objKPIEvaluation.Monitoring_Mechanism = form["Monitoring_Mechanism"];

                return objKPIEvaluation.FunUpdateKPIPlan(objKPIEvaluation);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //POST: /KPIEvaluation/KPIPlanAdd
         
        [AllowAnonymous]
        public bool KPIPlanAdd(KPIEvaluationModels objKPIEvaluationModels, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "KPI";
                KPIEvaluationModelsList objKPIEvaluationModelsList = new KPIEvaluationModelsList();
                objKPIEvaluationModelsList.lstKPIEvaluation = new List<KPIEvaluationModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["EstablishedOn"], out dateValue) == true)
                {
                    objKPIEvaluationModels.EstablishedOn = dateValue;
                }

                objKPIEvaluationModels.Process_Indicator = form["Process_Indicator"];
                objKPIEvaluationModels.Measurable_Value = form["Measurable_Value"];
                objKPIEvaluationModels.Monitoring_Mechanism = form["Monitoring_Mechanism"];
                objKPIEvaluationModels.Freq_of_Eval = form["Freq_of_Eval"];
                objKPIEvaluationModelsList.lstKPIEvaluation.Add(objKPIEvaluationModels);

                return objKPIEvaluationModels.FunAddKPIPlanTrans(objKPIEvaluationModelsList, Convert.ToInt16(objKPIEvaluationModels.KPI_Id));
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //
        // GET: /KPIEvaluation/AddKPIPlanEvaluation
         
        [AllowAnonymous]
        public ActionResult AddKPIPlanEvaluation()
        {
            ViewBag.SubMenutype = "KPI";
            if (Request.QueryString["KPI_Trans_Id"] != null)
            {
                return InitilizeAddKPIPlanEvaluation(Request.QueryString["KPI_Trans_Id"]);
            }
            else
            {
                TempData["alertdata"] = "KPI Trans Id cannot be Null or empty";
                return RedirectToAction("KPIEvaluationList");
            }
        }

        //
        // GET: /KPIEvaluation/InitilizeAddKPIPlanEvaluation
         
        [AllowAnonymous]
        private ActionResult InitilizeAddKPIPlanEvaluation(string sKPI_Trans_Id)
        {
            ViewBag.SubMenutype = "KPI";
            try
            {
                KPIEvaluationModels objkpi = new KPIEvaluationModels();
                ViewBag.KPI_Trans_Id = sKPI_Trans_Id;
                ViewBag.Eval_Status = objkpi.GetEvaluationList();

                DataSet dskpi = objGlobaldata.Getdetails("select KPI_Trans_Id,b.KPI_Id,EstablishedOn,Process_Indicator,Measurable_Value,Monitoring_Mechanism from t_kpi a,t_kpi_trans b " +
                        "where a.KPI_Id=b.KPI_Id and KPI_Trans_Id='" + sKPI_Trans_Id + "'");
                if (dskpi.Tables.Count > 0 && dskpi.Tables[0].Rows.Count > 0)
                {
                    ViewBag.KPI_Id = dskpi.Tables[0].Rows[0]["KPI_Id"].ToString();
                    ViewBag.Process_Indicator = dskpi.Tables[0].Rows[0]["Process_Indicator"].ToString();
                    ViewBag.Measurable_Value = dskpi.Tables[0].Rows[0]["Measurable_Value"].ToString();
                    ViewBag.Monitoring_Mechanism = dskpi.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString();
                    ViewBag.EstablishedOn = Convert.ToDateTime(dskpi.Tables[0].Rows[0]["EstablishedOn"].ToString()).ToString("dd/MM/yyyy");

                }
               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddKPIPlanEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        //
        // POST: /KPIEvaluation/AddKPIPlanEvaluation
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddKPIPlanEvaluation(KPIEvaluationModels objKPIEvaluationModels, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            ViewBag.SubMenutype = "KPI";
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;

                if (DateTime.TryParse(form["Eval_On"], out dateValue) == true)
                {
                    objKPIEvaluationModels.Eval_On = dateValue;
                }

                objKPIEvaluationModels.Eval_Status = form["Eval_Status"];

                objKPIEvaluationModels.Measured_Value = form["Measured_Value"];
                
                objKPIEvaluationModels.KPI_Trans_Id = Convert.ToInt16(form["KPI_Trans_Id"]);
                objKPIEvaluationModels.NCRRef = form["NCRRef"];

                if (upload != null && files.ContentLength > 0)
                {
                    objKPIEvaluationModels.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/KPI"), Path.GetFileName(file.FileName));
                            string sFilename = "kpi" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objKPIEvaluationModels.upload = objKPIEvaluationModels.upload + "," + "~/DataUpload/MgmtDocs/KPI/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddKPIPlanEvaluation-upload: " + ex.ToString());
                        }
                    }
                    objKPIEvaluationModels.upload = objKPIEvaluationModels.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }




                if (objKPIEvaluationModels.FunAddKPIPlanEvaluation(objKPIEvaluationModels))
                {
                    TempData["Successdata"] = "Added KPI Evaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddKPIPlanEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return RedirectToAction("KPIPlanEvaluationList", new { KPI_Trans_Id = objKPIEvaluationModels.KPI_Trans_Id });
        }


        // GET: /KPIEvaluation/KPIPlanEvaluationList
         
        [AllowAnonymous]
        public ActionResult KPIPlanEvaluationList()
        {
            ViewBag.SubMenutype = "KPI";

            KPIEvaluationModelsList objKPIEvaluationModelsList = new KPIEvaluationModelsList();
            objKPIEvaluationModelsList.lstKPIEvaluation = new List<KPIEvaluationModels>();
            KPIEvaluationModels objkpi = new KPIEvaluationModels();
            if (Request.QueryString["KPI_Trans_Id"] != null && Request.QueryString["KPI_Trans_Id"] != "")
            {
                string sKPI_Trans_Id = Request.QueryString["KPI_Trans_Id"];
                ViewBag.KPI_Trans_Id = sKPI_Trans_Id;

                DataSet dsKPI_Id = objGlobaldata.Getdetails("select KPI_Id,EstablishedOn,Process_Indicator,Measurable_Value,Monitoring_Mechanism from t_kpi_trans where KPI_Trans_Id='" + sKPI_Trans_Id + "'");
                if (dsKPI_Id.Tables.Count > 0 && dsKPI_Id.Tables[0].Rows.Count > 0)
                {
                    ViewBag.KPI_Id = dsKPI_Id.Tables[0].Rows[0]["KPI_Id"].ToString();
                    ViewBag.Process_Indicator = dsKPI_Id.Tables[0].Rows[0]["Process_Indicator"].ToString();
                    ViewBag.Measurable_Value = dsKPI_Id.Tables[0].Rows[0]["Measurable_Value"].ToString();
                    ViewBag.Monitoring_Mechanism = dsKPI_Id.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString();
                    ViewBag.EstablishedOn = Convert.ToDateTime(dsKPI_Id.Tables[0].Rows[0]["EstablishedOn"].ToString()).ToString("dd/MM/yyyy");
                }

                try
                {
                    string sSqlstmt = "select KPI_Trans_Eval_Id, KPI_Trans_Id, Measured_Value, Eval_Status, Eval_On, NCRRef,upload from t_kpi_trans_evaluation where KPI_Trans_Id='"
                        + sKPI_Trans_Id + "'";

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                        {
                            DateTime dtEval_On = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[i]["Eval_On"].ToString());

                            try
                            {
                                KPIEvaluationModels objKPIEvaluationModels = new KPIEvaluationModels
                                {
                                    KPI_Trans_Eval_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["KPI_Trans_Eval_Id"].ToString()),
                                    KPI_Trans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["KPI_Trans_Id"].ToString()),
                                    Eval_On = dtEval_On,
                                    Measured_Value = dsObjectivesModelsList.Tables[0].Rows[i]["Measured_Value"].ToString(),
                                    Eval_Status =objkpi.GetEvaluationNameById(dsObjectivesModelsList.Tables[0].Rows[i]["Eval_Status"].ToString()),
                                    NCRRef = dsObjectivesModelsList.Tables[0].Rows[i]["NCRRef"].ToString(),
                                    upload = dsObjectivesModelsList.Tables[0].Rows[i]["upload"].ToString()
                                };
                                objKPIEvaluationModelsList.lstKPIEvaluation.Add(objKPIEvaluationModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in KPIPlanEvaluationList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in KPIPlanEvaluationList: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            else
            {
                TempData["alertdata"] = "KPI Trans Id cannot be Null or empty";
                return RedirectToAction("KPIEvaluationList");
            }
            
            return View(objKPIEvaluationModelsList.lstKPIEvaluation.ToList());
        }


        // GET: /KPIEvaluation/KPIPlanEvaluationEdit
         
        [AllowAnonymous]
        public ActionResult KPIPlanEvaluationEdit()
        {
            ViewBag.SubMenutype = "KPI";
            KPIEvaluationModels objKPIEvaluationModels = new KPIEvaluationModels();

            if (Request.QueryString["KPI_Trans_Eval_Id"] != null && Request.QueryString["KPI_Trans_Eval_Id"] != "")
            {
                string sKPI_Trans_Eval_Id = Request.QueryString["KPI_Trans_Eval_Id"];
                ViewBag.KPI_Trans_Id = sKPI_Trans_Eval_Id;
                ViewBag.Eval_Status = objKPIEvaluationModels.GetEvaluationList();

                try
                {
                    string sSqlstmt = "select KPI_Trans_Eval_Id, a.KPI_Trans_Id, Measured_Value, Eval_Status, Eval_On, NCRRef,upload,EstablishedOn,Process_Indicator,Measurable_Value," +
                        "Monitoring_Mechanism from t_kpi_trans_evaluation a,t_kpi_trans b where a.KPI_Trans_Id=b.KPI_Trans_Id and  KPI_Trans_Eval_Id='" + sKPI_Trans_Eval_Id + "'";

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtEval_On = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["Eval_On"].ToString());
                        DateTime dtEstablishedOn = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["EstablishedOn"].ToString());

                        try
                        {
                            objKPIEvaluationModels = new KPIEvaluationModels
                            {
                                KPI_Trans_Eval_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["KPI_Trans_Eval_Id"].ToString()),
                                KPI_Trans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["KPI_Trans_Id"].ToString()),
                                Eval_On = dtEval_On,
                                Measured_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Measured_Value"].ToString(),
                                Eval_Status =objKPIEvaluationModels.GetEvaluationNameById(dsObjectivesModelsList.Tables[0].Rows[0]["Eval_Status"].ToString()),
                                NCRRef = dsObjectivesModelsList.Tables[0].Rows[0]["NCRRef"].ToString(),
                                upload = dsObjectivesModelsList.Tables[0].Rows[0]["upload"].ToString(),
                                Process_Indicator = dsObjectivesModelsList.Tables[0].Rows[0]["Process_Indicator"].ToString(),
                                Measurable_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Measurable_Value"].ToString(),
                                Monitoring_Mechanism = dsObjectivesModelsList.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString(),
                                EstablishedOn = dtEstablishedOn,
                            };
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in KPIPlanEvaluationEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
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
                    objGlobaldata.AddFunctionalLog("Exception in KPIPlanEvaluationEdit: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }                              
            }
            else
            {
                TempData["alertdata"] = "KPI Trans Id cannot be Null or empty";
                return RedirectToAction("KPIEvaluationList");
            }
            return View(objKPIEvaluationModels);
        }

        //
        // POST: /KPIEvaluation/AddKPIPlanEvaluation
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KPIPlanEvaluationEdit(KPIEvaluationModels objKPIEvaluationModels, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            ViewBag.SubMenutype = "KPI";
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];
                DateTime dateValue;

                if (DateTime.TryParse(form["Eval_On"], out dateValue) == true)
                {
                    objKPIEvaluationModels.Eval_On = dateValue;
                }

                objKPIEvaluationModels.Eval_Status = form["Eval_Status"];

                //objKPIEvaluationModels.Measured_Value = form["Measured_Value"];

                objKPIEvaluationModels.KPI_Trans_Eval_Id = Convert.ToInt16(form["KPI_Trans_Eval_Id"]);
                objKPIEvaluationModels.KPI_Trans_Id = Convert.ToInt16(form["KPI_Trans_Id"]);
                //objKPIEvaluationModels.NCRRef = form["NCRRef"];

                if (upload != null && files.ContentLength > 0)
                {
                    objKPIEvaluationModels.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/KPI"), Path.GetFileName(file.FileName));
                            string sFilename = "kpi" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objKPIEvaluationModels.upload = objKPIEvaluationModels.upload + "," + "~/DataUpload/MgmtDocs/KPI/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in KPIPlanEvaluationEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objKPIEvaluationModels.upload = objKPIEvaluationModels.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objKPIEvaluationModels.upload = objKPIEvaluationModels.upload + "," + form["QCDocsVal"];
                    objKPIEvaluationModels.upload = objKPIEvaluationModels.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objKPIEvaluationModels.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objKPIEvaluationModels.upload = null;
                }

                if (objKPIEvaluationModels.FunUpdateKPIPlanEvaluation(objKPIEvaluationModels))
                {
                    TempData["Successdata"] = "Added KPI Evaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIPlanEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            
            return RedirectToAction("KPIPlanEvaluationList", new { KPI_Trans_Id = objKPIEvaluationModels.KPI_Trans_Id });
        }
    }

   
}
