using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.IO;
using System.Data;
using PagedList;
using PagedList.Mvc;
using Rotativa;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class TRAController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public TRAController()
        {
            ViewBag.Menutype = "Risk";
            ViewBag.SubMenutype = "TRA";
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddTRA()
        {
            TRAModels objTRAMdl = new TRAModels();
            try
            {
                RiskMgmtModels objRisk = new RiskMgmtModels();
                objTRAMdl.branch = objGlobaldata.GetCurrentUserSession().division;
                objTRAMdl.department = objGlobaldata.GetCurrentUserSession().DeptID;
                objTRAMdl.location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objTRAMdl.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objTRAMdl.branch);
                ViewBag.Task = objGlobaldata.GetConstantValue("TRATaskPerformer");
                ViewBag.severity = objGlobaldata.GetDropdownList("Risk-Severity");
                ViewBag.probability = objGlobaldata.GetDropdownList("Risk-likelihood");
            }
            catch(Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTRA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTRAMdl);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult AddTRA(FormCollection form, TRAModels objTRA)
        {
           
            try
            {
                objTRA.branch = form["branch"];
                objTRA.location = form["location"];
                objTRA.department = form["department"];

                TRAModelsList objTRAPersList = new TRAModelsList();
                objTRAPersList.TRAList = new List<TRAModels>();

                TRAModelsList objTRATaskList = new TRAModelsList();
                objTRATaskList.TRAList = new List<TRAModels>();
                
                DateTime dateValue;

                if(DateTime.TryParse(form["tra_date"], out dateValue) == true)
                {
                    objTRA.tra_date = dateValue;
                }
                if (objTRA.pers_name != "" && objTRA.pers_name != null)
                {
                    TRAModels objTRAModel = new TRAModels();
                    objTRAModel.pers_name = form["pers_name"];
                    objTRAModel.designation = form["designation"];
                    objTRAPersList.TRAList.Add(objTRAModel);
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    TRAModels objTRAModel = new TRAModels();
                    if (form["pers_name " + i] != "" && form["pers_name " + i] != null)
                    {
                    objTRAModel.pers_name = form["pers_name " + i];
                    objTRAModel.designation = form["designation " + i];
                    objTRAPersList.TRAList.Add(objTRAModel);
                    }
                }
                if (objTRA.activity != "" && objTRA.activity != null)
                {
                    TRAModels objTRATaskModel = new TRAModels();
                    objTRATaskModel.activity = form["activity"];
                    objTRATaskModel.hazards = form["hazards"];
                    objTRATaskModel.causes = form["causes"];
                    objTRATaskModel.consequences = form["consequences"];
                    objTRATaskModel.existing_measure = form["existing_measure"];
                    objTRATaskModel.severity = form["severity"];
                    objTRATaskModel.probability = form["probability"];
                    objTRATaskModel.risk_rating = form["risk_rating"];
                    objTRATaskList.TRAList.Add(objTRATaskModel);
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    TRAModels objTRATaskModel = new TRAModels();
                    if (form["activity " + i] != "" && form["activity " + i] != null)
                    {
                        objTRATaskModel.activity = form["activity " + i];
                        objTRATaskModel.hazards = form["hazards " + i];
                        objTRATaskModel.causes = form["causes " + i];
                        objTRATaskModel.consequences = form["consequences " + i];
                        objTRATaskModel.existing_measure = form["existing_measure " + i];
                        objTRATaskModel.severity = form["severity " + i];
                        objTRATaskModel.probability = form["probability " + i];
                        objTRATaskModel.risk_rating = form["risk_rating " + i];
                        objTRATaskList.TRAList.Add(objTRATaskModel);
                    }
                }

                if(objTRA.FunAddTRA(objTRA, objTRAPersList, objTRATaskList))
                {
                    TempData["Successdata"] = "Added TRA details successfully  with Reference Number '" + objTRA.tra_no + "'"; 
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTRA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult TRAList(string branch_name)
        {
            TRAModelsList objTRAList = new TRAModelsList();
            objTRAList.TRAList = new List<TRAModels>();
            try
            {
                string sSqlstmt = "select id_tra,tra_no,tra_date,revno,tra_ref,location,desc_work,document_title," +
                    "duration,task_performer,branch,department from t_tra where active=1";

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

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

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_tra desc";

                DataSet dsTRA = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsTRA.Tables.Count > 0 && i < dsTRA.Tables[0].Rows.Count; i++)
                {                  
                    try
                    {
                        TRAModels objTRAModels = new TRAModels
                        {
                            id_tra = dsTRA.Tables[0].Rows[i]["id_tra"].ToString(),
                            tra_no = dsTRA.Tables[0].Rows[i]["tra_no"].ToString(),
                            revno = dsTRA.Tables[0].Rows[i]["revno"].ToString(),
                            tra_ref = dsTRA.Tables[0].Rows[i]["tra_ref"].ToString(),
                            location = objGlobaldata.GetDivisionLocationById(dsTRA.Tables[0].Rows[i]["location"].ToString()),
                            desc_work = dsTRA.Tables[0].Rows[i]["desc_work"].ToString(),
                            document_title = dsTRA.Tables[0].Rows[i]["document_title"].ToString(),
                            duration = dsTRA.Tables[0].Rows[i]["duration"].ToString(),
                            task_performer = dsTRA.Tables[0].Rows[i]["task_performer"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsTRA.Tables[0].Rows[i]["branch"].ToString()),
                            department = objGlobaldata.GetMultiDeptNameById(dsTRA.Tables[0].Rows[i]["department"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsTRA.Tables[0].Rows[i]["tra_date"].ToString(), out dateValue))
                        {
                            objTRAModels.tra_date = dateValue;
                        }

                        objTRAList.TRAList.Add(objTRAModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SupplierList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objTRAList.TRAList.ToList());
        }

        [AllowAnonymous]
        public JsonResult TRASearchList(string branch_name)
        {
            TRAModelsList objTRAList = new TRAModelsList();
            objTRAList.TRAList = new List<TRAModels>();
            try
            {
                string sSqlstmt = "select id_tra,tra_no,tra_date,revno,tra_ref,location,desc_work,document_title," +
                   "duration,task_performer,branch,department from t_tra where active=1";

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

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

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_tra desc";

                DataSet dsTRA = objGlobaldata.Getdetails(sSqlstmt);


                for (int i = 0; dsTRA.Tables.Count > 0 && i < dsTRA.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        TRAModels objTRAModels = new TRAModels
                        {
                            id_tra = dsTRA.Tables[0].Rows[i]["id_tra"].ToString(),
                            tra_no = dsTRA.Tables[0].Rows[i]["tra_no"].ToString(),
                            revno = dsTRA.Tables[0].Rows[i]["revno"].ToString(),
                            tra_ref = dsTRA.Tables[0].Rows[i]["tra_ref"].ToString(),
                            location = objGlobaldata.GetDivisionLocationById(dsTRA.Tables[0].Rows[i]["location"].ToString()),
                            desc_work = dsTRA.Tables[0].Rows[i]["desc_work"].ToString(),
                            document_title = dsTRA.Tables[0].Rows[i]["document_title"].ToString(),
                            duration = dsTRA.Tables[0].Rows[i]["duration"].ToString(),
                            task_performer = dsTRA.Tables[0].Rows[i]["task_performer"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsTRA.Tables[0].Rows[i]["branch"].ToString()),
                            department = objGlobaldata.GetMultiDeptNameById(dsTRA.Tables[0].Rows[i]["department"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsTRA.Tables[0].Rows[i]["tra_date"].ToString(), out dateValue))
                        {
                            objTRAModels.tra_date = dateValue;
                        }

                        objTRAList.TRAList.Add(objTRAModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in TRASearchList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TRASearchList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }



        [AllowAnonymous]
        public ActionResult TRAEdit()
        {
            RiskMgmtModels objRisk = new RiskMgmtModels(); 
            try
            {
                if (Request.QueryString["id_tra"] != null)
                {
                    ViewBag.Task = objGlobaldata.GetConstantValue("TRATaskPerformer");
                    ViewBag.severity = objGlobaldata.GetDropdownList("Risk-Severity");
                    ViewBag.probability = objGlobaldata.GetDropdownList("Risk-likelihood");
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.Status = objGlobaldata.GetDropdownList("TRA Status");

                    string sid_tra = Request.QueryString["id_tra"];

                    string sSqlstmt = "select id_tra,tra_no,tra_date,revno,tra_ref,location,desc_work,document_title,duration,task_performer,branch,department from t_tra"
                + " where id_tra='" + sid_tra + "' ";

                    DataSet dsTRAList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTRAList.Tables.Count > 0 && dsTRAList.Tables[0].Rows.Count > 0)
                    {
                        TRAModels objTRAModel = new TRAModels
                        {
                            id_tra = dsTRAList.Tables[0].Rows[0]["id_tra"].ToString(),
                            tra_no = dsTRAList.Tables[0].Rows[0]["tra_no"].ToString(),
                            revno = dsTRAList.Tables[0].Rows[0]["revno"].ToString(),
                            tra_ref = dsTRAList.Tables[0].Rows[0]["tra_ref"].ToString(),
                            location = dsTRAList.Tables[0].Rows[0]["location"].ToString(),
                            desc_work = dsTRAList.Tables[0].Rows[0]["desc_work"].ToString(),
                            document_title = dsTRAList.Tables[0].Rows[0]["document_title"].ToString(),
                            duration = dsTRAList.Tables[0].Rows[0]["duration"].ToString(),
                            task_performer = dsTRAList.Tables[0].Rows[0]["task_performer"].ToString(),
                            branch = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsTRAList.Tables[0].Rows[0]["branch"].ToString()),
                            department = /*objGlobaldata.GetMultiDeptNameById*/(dsTRAList.Tables[0].Rows[0]["department"].ToString()),

                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsTRAList.Tables[0].Rows[0]["tra_date"].ToString(), out dtValue))
                        {
                            objTRAModel.tra_date = dtValue;
                        }
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsTRAList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsTRAList.Tables[0].Rows[0]["branch"].ToString());


                        TRAModelsList objTRAPersList = new TRAModelsList();
                        objTRAPersList.TRAList = new List<TRAModels>();

                        sSqlstmt = "select id_jobperformer,id_tra,pers_name,designation from t_tra_jobperformer where id_tra='" + sid_tra + "'";
                        DataSet dsPersList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsPersList.Tables.Count > 0 && dsPersList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsPersList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    TRAModels objPers = new TRAModels
                                    {
                                        id_jobperformer = dsPersList.Tables[0].Rows[i]["id_jobperformer"].ToString(),
                                        id_tra = dsPersList.Tables[0].Rows[i]["id_tra"].ToString(),
                                        pers_name = dsPersList.Tables[0].Rows[i]["pers_name"].ToString(),
                                        designation = dsPersList.Tables[0].Rows[i]["designation"].ToString(),
                                    };
                                    objTRAPersList.TRAList.Add(objPers);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in TRAEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("HSEPatrolList");
                                }
                            }
                            ViewBag.objPersList = objTRAPersList;
                        }

                        TRAModelsList objTRATaskList = new TRAModelsList();
                        objTRATaskList.TRAList = new List<TRAModels>();

                        sSqlstmt = "select id_task,id_tra,activity,hazards,causes,consequences,existing_measure,severity,probability,risk_rating,additional_measures,add_severity,add_probability,add_risk_rating from t_tra_task where id_tra='" + sid_tra + "'";
                        DataSet dsTaskList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsTaskList.Tables.Count > 0 && dsTaskList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsTaskList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    TRAModels objTask = new TRAModels
                                    {
                                        id_task = dsTaskList.Tables[0].Rows[i]["id_task"].ToString(),
                                        id_tra = dsTaskList.Tables[0].Rows[i]["id_tra"].ToString(),
                                        activity = dsTaskList.Tables[0].Rows[i]["activity"].ToString(),
                                        hazards = dsTaskList.Tables[0].Rows[i]["hazards"].ToString(),
                                        causes = dsTaskList.Tables[0].Rows[i]["causes"].ToString(),
                                        consequences = dsTaskList.Tables[0].Rows[i]["consequences"].ToString(),
                                        existing_measure = dsTaskList.Tables[0].Rows[i]["existing_measure"].ToString(),
                                        severity = dsTaskList.Tables[0].Rows[i]["severity"].ToString(),
                                        probability = dsTaskList.Tables[0].Rows[i]["probability"].ToString(),
                                        risk_rating = dsTaskList.Tables[0].Rows[i]["risk_rating"].ToString(),

                                        additional_measures = dsTaskList.Tables[0].Rows[i]["additional_measures"].ToString(),
                                        add_severity = dsTaskList.Tables[0].Rows[i]["add_severity"].ToString(),
                                        add_probability = dsTaskList.Tables[0].Rows[i]["add_probability"].ToString(),
                                        add_risk_rating = dsTaskList.Tables[0].Rows[i]["add_risk_rating"].ToString(),
                                    };
                                    objTRATaskList.TRAList.Add(objTask);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in TRAEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("HSEPatrolList");
                                }
                            }
                            ViewBag.objTaskList = objTRATaskList;
                        }


                        TRAModelsList objTRAEvalList = new TRAModelsList();
                        objTRAEvalList.TRAList = new List<TRAModels>();

                        sSqlstmt = "select id_task,id_tra,activity,hazards,causes,consequences,existing_measure,severity,probability,risk_rating,additional_measures,add_severity,add_probability,add_risk_rating,tra_status,close_date,close_by,out_come from t_tra_task where id_tra='" + sid_tra + "'";
                        DataSet dsEvalList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsEvalList.Tables.Count > 0 && dsTaskList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsEvalList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    TRAModels objEval = new TRAModels
                                    {
                                        id_task = dsEvalList.Tables[0].Rows[i]["id_task"].ToString(),
                                        id_tra = dsEvalList.Tables[0].Rows[i]["id_tra"].ToString(),
                                        activity = dsEvalList.Tables[0].Rows[i]["activity"].ToString(),
                                        hazards = dsEvalList.Tables[0].Rows[i]["hazards"].ToString(),
                                        causes = dsEvalList.Tables[0].Rows[i]["causes"].ToString(),
                                        consequences = dsEvalList.Tables[0].Rows[i]["consequences"].ToString(),
                                        existing_measure = dsEvalList.Tables[0].Rows[i]["existing_measure"].ToString(),
                                        severity = dsEvalList.Tables[0].Rows[i]["severity"].ToString(),
                                        probability = dsEvalList.Tables[0].Rows[i]["probability"].ToString(),
                                        risk_rating = dsEvalList.Tables[0].Rows[i]["risk_rating"].ToString(),

                                        additional_measures = dsEvalList.Tables[0].Rows[i]["additional_measures"].ToString(),
                                        add_severity = dsEvalList.Tables[0].Rows[i]["add_severity"].ToString(),
                                        add_probability = dsEvalList.Tables[0].Rows[i]["add_probability"].ToString(),
                                        add_risk_rating = dsEvalList.Tables[0].Rows[i]["add_risk_rating"].ToString(),

                                        tra_status = dsEvalList.Tables[0].Rows[i]["tra_status"].ToString(),
                                        close_by = dsEvalList.Tables[0].Rows[i]["close_by"].ToString(),
                                        out_come = dsEvalList.Tables[0].Rows[i]["out_come"].ToString(),
                                    };
                                   
                                    if (DateTime.TryParse(dsEvalList.Tables[0].Rows[i]["close_date"].ToString(), out dtValue))
                                    {
                                        objEval.close_date = dtValue;
                                    }
                                    objTRAEvalList.TRAList.Add(objEval);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in TRAEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("HSEPatrolList");
                                }
                            }
                            ViewBag.objEvalList = objTRAEvalList;
                        }

                        return View(objTRAModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("TRAList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("TRAList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TRAEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TRAList");
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult TRAEdit(FormCollection form, TRAModels objTRA)
        {
            try
            {
                objTRA.branch = form["branch"];
                objTRA.location = form["location"];
                objTRA.department = form["department"];

                TRAModelsList objTRAPersList = new TRAModelsList();
                objTRAPersList.TRAList = new List<TRAModels>();

                TRAModelsList objTRATaskList = new TRAModelsList();
                objTRATaskList.TRAList = new List<TRAModels>();
                
                DateTime dateValue;

                if (DateTime.TryParse(form["tra_date"], out dateValue) == true)
                {
                    objTRA.tra_date = dateValue;
                }
                if (objTRA.pers_name != "" && objTRA.pers_name != null)
                {
                    TRAModels objTRAModel = new TRAModels();
                    objTRAModel.pers_name = form["pers_name"];
                    objTRAModel.designation = form["designation"];
                    objTRAPersList.TRAList.Add(objTRAModel);
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    TRAModels objTRAModel = new TRAModels();
                    if (form["pers_name " + i] != "" && form["pers_name " + i] != null)
                    {
                        objTRAModel.pers_name = form["pers_name " + i];
                        objTRAModel.designation = form["designation " + i];
                        objTRAPersList.TRAList.Add(objTRAModel);
                    }
                }
                if (objTRA.activity != "" && objTRA.activity != null)
                {
                    TRAModels objTRATaskModel = new TRAModels();
                    objTRATaskModel.activity = form["activity"];
                    objTRATaskModel.hazards = form["hazards"];
                    objTRATaskModel.causes = form["causes"];
                    objTRATaskModel.consequences = form["consequences"];
                    objTRATaskModel.existing_measure = form["existing_measure"];
                    objTRATaskModel.severity = form["severity"];
                    objTRATaskModel.probability = form["probability"];
                    objTRATaskModel.risk_rating = form["risk_rating"];
                    objTRATaskList.TRAList.Add(objTRATaskModel);
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    TRAModels objTRATaskModel = new TRAModels();
                    if (form["activity " + i] != "" && form["activity " + i] != null)
                    {
                        objTRATaskModel.activity = form["activity " + i];
                        objTRATaskModel.hazards = form["hazards " + i];
                        objTRATaskModel.causes = form["causes " + i];
                        objTRATaskModel.consequences = form["consequences " + i];
                        objTRATaskModel.existing_measure = form["existing_measure " + i];
                        objTRATaskModel.severity = form["severity " + i];
                        objTRATaskModel.probability = form["probability " + i];
                        objTRATaskModel.risk_rating = form["risk_rating " + i];

                        objTRATaskModel.additional_measures = form["additional_measures " + i];
                        objTRATaskModel.add_severity = form["add_severity" + i];
                        objTRATaskModel.add_probability = form["add_probability" + i];
                        objTRATaskModel.add_risk_rating = form["add_risk_rating " + i];

                        objTRATaskModel.tra_status = form["tra_status" + i];
                        objTRATaskModel.close_by = form["close_by" + i];
                        objTRATaskModel.out_come = form["out_come" + i];
                        if (DateTime.TryParse(form["close_date" +i], out dateValue) == true)
                        {
                            objTRATaskModel.close_date = dateValue;
                        }

                        objTRATaskList.TRAList.Add(objTRATaskModel);
                    }
                }

                if (objTRA.FunUpdateTRA(objTRA, objTRAPersList, objTRATaskList))
                {
                    TempData["Successdata"] = "Updated TRA successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TRAEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult TRADetail()
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            try
            {
                if (Request.QueryString["id_tra"] != null)
                {
                   
                    string sid_tra = Request.QueryString["id_tra"];

                    string sSqlstmt = "select id_tra,tra_no,tra_date,revno,tra_ref,location,desc_work,document_title,duration,task_performer,branch,department from t_tra"
                + " where id_tra='" + sid_tra + "' ";

                    DataSet dsTRAList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTRAList.Tables.Count > 0 && dsTRAList.Tables[0].Rows.Count > 0)
                    {
                        TRAModels objTRAModel = new TRAModels
                        {
                            id_tra = dsTRAList.Tables[0].Rows[0]["id_tra"].ToString(),
                            tra_no = dsTRAList.Tables[0].Rows[0]["tra_no"].ToString(),
                            revno = dsTRAList.Tables[0].Rows[0]["revno"].ToString(),
                            tra_ref = dsTRAList.Tables[0].Rows[0]["tra_ref"].ToString(),
                            location = objGlobaldata.GetDivisionLocationById(dsTRAList.Tables[0].Rows[0]["location"].ToString()),
                            desc_work = dsTRAList.Tables[0].Rows[0]["desc_work"].ToString(),
                            document_title = dsTRAList.Tables[0].Rows[0]["document_title"].ToString(),
                            duration = dsTRAList.Tables[0].Rows[0]["duration"].ToString(),
                            task_performer = dsTRAList.Tables[0].Rows[0]["task_performer"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsTRAList.Tables[0].Rows[0]["branch"].ToString()),
                            department = objGlobaldata.GetMultiDeptNameById(dsTRAList.Tables[0].Rows[0]["department"].ToString()),

                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsTRAList.Tables[0].Rows[0]["tra_date"].ToString(), out dtValue))
                        {
                            objTRAModel.tra_date = dtValue;
                        }

                        

                        sSqlstmt = "select id_jobperformer,id_tra,pers_name,designation from t_tra_jobperformer where id_tra='" + sid_tra + "'";
                        ViewBag.Pers = objGlobaldata.Getdetails(sSqlstmt);
                
                        sSqlstmt = "select id_task,id_tra,activity,hazards,causes,consequences,existing_measure," +
                            "severity,probability,risk_rating,additional_measures,add_severity,add_probability,add_risk_rating" +
                            ",tra_status,close_date,close_by,out_come from t_tra_task where id_tra='" + sid_tra + "'";
                        ViewBag.Task = objGlobaldata.Getdetails(sSqlstmt);
                       
                        return View(objTRAModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("TRAList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("TRAList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TRADetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TRAList");
        }
        
        [AllowAnonymous]
        public JsonResult TRADocDelete(FormCollection form)
        {
            try
            {

                   if (form["id_tra"] != null && form["id_tra"] != "")
                    {

                        TRAModels Doc = new TRAModels();
                        string sid_tra = form["id_tra"];


                        if (Doc.FunDeleteTRADoc(sid_tra))
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
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return Json("Failed");
                    }
               

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TRADocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        
        [HttpPost]
        public JsonResult FunGetRiskRating(string severity, string probability)
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            string RiskRating = "";
            if (severity != null && probability != null)
            {
                Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                dicRatings = objRisk.GetRiskRatings(severity, probability);
                if (dicRatings != null && dicRatings.Count > 0)
                {
                    RiskRating = dicRatings.FirstOrDefault().Key;
                }
            }

            return Json(RiskRating);
        }

    }   
}