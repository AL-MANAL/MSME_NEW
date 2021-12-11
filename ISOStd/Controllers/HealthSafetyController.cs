using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using PagedList;
using PagedList.Mvc;
using System.IO;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    public class HealthSafetyController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public HealthSafetyController()
        {
            ViewBag.Menutype = "Risk";
            ViewBag.SubMenutype = "Hazard";
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddHazard(string id_issue)
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            HealthSafetyModels objHazard = new HealthSafetyModels();
            try
            {
                if (id_issue != "" && id_issue != null)
                {
                    ViewBag.Issue = objRisk.GetIsssuesNo(id_issue);
                    objHazard.Issue = id_issue;
                }
                else
                {
                    ViewBag.Issue = objRisk.GetIsssuesNo();
                }
                objHazard.branch_id = objGlobaldata.GetCurrentUserSession().division;
                objHazard.dept = objGlobaldata.GetCurrentUserSession().DeptID;
                objHazard.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objHazard.branch_id);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objHazard.branch_id);

                ViewBag.source_id = objHazard.GetMultiRiskSourceList();
                ViewBag.Activity = objGlobaldata.GetDropdownList("HS_activity_type");
                ViewBag.Consequences = objGlobaldata.GetDropdownList("HS_consequences");
                ViewBag.Injury = objGlobaldata.GetDropdownList("HS_injury");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
               //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(objHazard.branch_id, objHazard.dept, objHazard.Location);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHazard: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objHazard);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddHazard(HealthSafetyModels objHazard, FormCollection form)
        {
            try
            {
                objHazard.notified_to = form["notified_to"];
                objHazard.consequences = form["consequences"];
                if (objHazard.FunAddHazard(objHazard))
                {
                    TempData["Successdata"] = "Added Hazard details successfully with Reference Number '" + objHazard.hazard_refno + "'";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHazard: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult HazardList(string SearchText, string risk_status_id, int? page, string branch_name)
        {
            HealthSafetyModelsList objHazardModelsList = new HealthSafetyModelsList();
            objHazardModelsList.lstHazard = new List<HealthSafetyModels>();
            HealthSafetyModels objSafety = new HealthSafetyModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
                ViewBag.IssueCategory = objGlobaldata.GetDropdownList("Issue Category Type");
                string sSqlstmt = "select T1.id_hazard,T1.hazard_refno,T1.impact_id,T1.like_id,T1.dept,T1.branch_id,T1.Location,T1.source_id,T1.activity_type,T1.consequences,T1.injury,T1.activity,T1.hazards,T1.notified_to,T1.reported_by,T1.reported_date,"
                +" (select impact_id from t_hazard_trans T2 where T1.id_hazard = T2.id_hazard order by id_hazard_trans desc limit 1) as curr_impact_id,"
                +" (select like_id from t_hazard_trans T2 where T1.id_hazard = T2.id_hazard order by id_hazard_trans desc limit 1) as curr_like_id"
                +" from t_hazard T1 where T1.Active = 1";
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch_id)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch_id)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_hazard desc";

                DataSet dsHazardModels = objGlobaldata.Getdetails(sSqlstmt);

                if (dsHazardModels.Tables.Count > 0 && dsHazardModels.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsHazardModels.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                        Dictionary<string, string> dicRatings_curr = new Dictionary<string, string>();

                        if (dsHazardModels.Tables[0].Rows[i]["impact_id"].ToString() != "" && dsHazardModels.Tables[0].Rows[i]["like_id"].ToString() != "")
                        {
                            dicRatings = objSafety.GetRiskRatings(dsHazardModels.Tables[0].Rows[i]["impact_id"].ToString(),
                            dsHazardModels.Tables[0].Rows[i]["like_id"].ToString());
                        }
                        if (dsHazardModels.Tables[0].Rows[i]["curr_impact_id"].ToString() != "" && dsHazardModels.Tables[0].Rows[i]["curr_like_id"].ToString() != "")
                        {
                            dicRatings_curr = objSafety.GetRiskRatings(dsHazardModels.Tables[0].Rows[i]["curr_impact_id"].ToString(),
                            dsHazardModels.Tables[0].Rows[i]["curr_like_id"].ToString());
                        }

                        try
                        {
                            HealthSafetyModels objHazard = new HealthSafetyModels
                            {
                                id_hazard = (dsHazardModels.Tables[0].Rows[i]["id_hazard"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsHazardModels.Tables[0].Rows[i]["dept"].ToString()),
                                branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardModels.Tables[0].Rows[i]["branch_id"].ToString()),
                                source_id = objSafety.GetRiskSourceNameById(dsHazardModels.Tables[0].Rows[i]["source_id"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsHazardModels.Tables[0].Rows[i]["Location"].ToString()),
                                hazard_refno = (dsHazardModels.Tables[0].Rows[i]["hazard_refno"].ToString()),
                                activity_type = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["activity_type"].ToString()),
                                consequences = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["consequences"].ToString()),
                                injury = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["injury"].ToString()),
                                activity = (dsHazardModels.Tables[0].Rows[i]["activity"].ToString()),
                                hazards = (dsHazardModels.Tables[0].Rows[i]["hazards"].ToString()),
                                notified_to = (dsHazardModels.Tables[0].Rows[i]["notified_to"].ToString()),
                                reported_by = (dsHazardModels.Tables[0].Rows[i]["reported_by"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsHazardModels.Tables[0].Rows[i]["reported_date"].ToString(), out dtValue))
                            {
                                objHazard.reported_date = dtValue;
                            }
                            if (dicRatings != null && dicRatings.Count > 0)
                            {
                                objHazard.RiskRating = dicRatings.FirstOrDefault().Key;
                                objHazard.color_code = dicRatings.FirstOrDefault().Value;
                            }
                            if (dicRatings_curr != null && dicRatings_curr.Count > 0)
                            {
                                objHazard.RiskRating_curr = dicRatings_curr.FirstOrDefault().Key;
                                objHazard.color_code_curr = dicRatings_curr.FirstOrDefault().Value;
                            }
                            string sql = "select t.mit_id from t_hazard_mitigations t,t_hazard tt where t.id_hazard = tt.id_hazard and"
                            + " t.id_hazard = '" + dsHazardModels.Tables[0].Rows[i]["id_hazard"].ToString() + "'";
                            DataSet dsRisk = objGlobaldata.Getdetails(sSqlstmt);

                            if (dsRisk.Tables.Count > 0 && dsRisk.Tables[0].Rows.Count > 0)
                            {
                                objHazard.mitmeasure = "Yes";
                            }
                            else
                            {
                                objHazard.mitmeasure = "No";
                            }
                            objHazardModelsList.lstHazard.Add(objHazard);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in HazardList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objHazardModelsList.lstHazard.ToList());
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddRiskEvaluation()
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            HealthSafetyModels objHazardModels = new HealthSafetyModels();
            try
            {
                if (Request.QueryString["id_hazard"] != null && Request.QueryString["id_hazard"] != "")
                {
                    string id_hazard = Request.QueryString["id_hazard"];
                    ViewBag.impact_id = objGlobaldata.GetDropdownList("Risk-Severity");
                    ViewBag.like_id = objGlobaldata.GetDropdownList("Risk-likelihood");
                    ViewBag.Legal = objGlobaldata.GetLawNo();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.EvaluatedBy = objGlobaldata.GetDeptHeadbyDivisionList();
                    ViewBag.id_hazard = id_hazard;
                    ViewBag.OP = objGlobaldata.GetConstantValue("HazardOP");
                    string sSqlstmt = "select id_hazard,hazard_refno,dept,branch_id,Location,activity,hazards,consequences,"
                    + "further_consequences,op_control,cnt_engineering,cnt_administrative,cnt_ppe,cnt_general,impact_id," +
                    "like_id,legal,legal_voilation,evaluated_by,eval_notified_to,evaluation_date,cnt_elimination,cnt_substitution  from t_hazard where id_hazard='"
                        + id_hazard + "'";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {

                        objHazardModels = new HealthSafetyModels
                        {
                            id_hazard = (dsRiskModels.Tables[0].Rows[0]["id_hazard"].ToString()),
                            hazard_refno = (dsRiskModels.Tables[0].Rows[0]["hazard_refno"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            Location = (dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                            activity = (dsRiskModels.Tables[0].Rows[0]["activity"].ToString()),
                            hazards = (dsRiskModels.Tables[0].Rows[0]["hazards"].ToString()),
                            consequences = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["consequences"].ToString()),
                            further_consequences = (dsRiskModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                            op_control = (dsRiskModels.Tables[0].Rows[0]["op_control"].ToString()),
                            cnt_engineering = (dsRiskModels.Tables[0].Rows[0]["cnt_engineering"].ToString()),
                            cnt_administrative = (dsRiskModels.Tables[0].Rows[0]["cnt_administrative"].ToString()),
                            cnt_ppe = (dsRiskModels.Tables[0].Rows[0]["cnt_ppe"].ToString()),
                            cnt_general = (dsRiskModels.Tables[0].Rows[0]["cnt_general"].ToString()),
                            cnt_elimination = (dsRiskModels.Tables[0].Rows[0]["cnt_elimination"].ToString()),
                            cnt_substitution = (dsRiskModels.Tables[0].Rows[0]["cnt_substitution"].ToString()),

                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            legal = (dsRiskModels.Tables[0].Rows[0]["legal"].ToString()),
                            legal_voilation = (dsRiskModels.Tables[0].Rows[0]["legal_voilation"].ToString()),
                            evaluated_by = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["evaluated_by"].ToString()),
                            eval_notified_to = (dsRiskModels.Tables[0].Rows[0]["eval_notified_to"].ToString()),
                          
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                        {
                            objHazardModels.evaluation_date = dtValue;
                        }
                       // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                    }
                    else
                    {
                        TempData["alertdata"] = "Kindly do Evaluation";
                        return RedirectToAction("HazardList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objHazardModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult AddRiskEvaluation(FormCollection form, HealthSafetyModels objHazard)
        {
            try
            {
                objHazard.eval_notified_to = form["eval_notified_to"];
                if (objHazard.op_control1 != "" && objHazard.op_control1 != null)
                {
                    objHazard.op_control = String.Concat("Engineering");
                }
                if (objHazard.op_control2 != "" && objHazard.op_control2 != null)
                {
                    if(objHazard.op_control != "" && objHazard.op_control != null)
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "Administrative");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("Administrative");
                    }    
                }
                if (objHazard.op_control3 != "" && objHazard.op_control3 != null)
                {
                    if (objHazard.op_control != "" && objHazard.op_control != null)
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "PPE");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("PPE");
                    }
                }
                if (objHazard.op_control4 != "" && objHazard.op_control4 != null)
                {
                    if (objHazard.op_control != "" && objHazard.op_control != null)
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "General");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("General");
                    }
                }
                if (objHazard.op_control5 != "" && objHazard.op_control5 != null)
                {
                    if (objHazard.op_control != "" && objHazard.op_control != null)
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "Elimination");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("Elimination");
                    }
                }
                if (objHazard.op_control6 != "" && objHazard.op_control6 != null)
                {
                    if (objHazard.op_control != "" && objHazard.op_control != null)
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "Substitution");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("Substitution");
                    }
                }

                if (objHazard.FunUpdateRiskEvaluation(objHazard))
                {
                    TempData["Successdata"] = "Added Initial Risk Evaluation Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddRiskMitigations()
        {

            HealthSafetyModels objRiskMgmtModels = new HealthSafetyModels();
            try
            {
                if (Request.QueryString["id_hazard"] != null && Request.QueryString["id_hazard"] != "")
                {
                    string id_hazard = Request.QueryString["id_hazard"];
                    ViewBag.id_hazard = id_hazard;
                    ViewBag.Approver = objGlobaldata.GetApprover();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.Control = objGlobaldata.GetConstantValue("HazardOP");
                    string sSqlstmt = "select id_hazard,dept,branch_id,Location,hazard_refno,impact_id,like_id,mit_evaluated_by,mit_notified_to,proposed_date,reeval_due_date,source_id,Issue,activity_type,consequences,injury,activity,hazards,reported_date,reported_by from t_hazard where id_hazard='"
                        + id_hazard + "'";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {

                        objRiskMgmtModels = new HealthSafetyModels
                        {
                            id_hazard = (dsRiskModels.Tables[0].Rows[0]["id_hazard"].ToString()),
                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            hazard_refno = (dsRiskModels.Tables[0].Rows[0]["hazard_refno"].ToString()),
                            mit_evaluated_by = (dsRiskModels.Tables[0].Rows[0]["mit_evaluated_by"].ToString()),
                            mit_notified_to = (dsRiskModels.Tables[0].Rows[0]["mit_notified_to"].ToString()),

                            dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objRiskMgmtModels.GetRiskSourceNameById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                            
                            activity_type = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            consequences = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["consequences"].ToString()),
                            injury = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["injury"].ToString()),
                            activity = (dsRiskModels.Tables[0].Rows[0]["activity"].ToString()),
                            hazards = (dsRiskModels.Tables[0].Rows[0]["hazards"].ToString()),
                         
                            reported_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["reported_by"].ToString()),
                            
                           
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["proposed_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.proposed_date = dtValue;
                        }
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.reeval_due_date = dtValue;
                        }
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.reported_date = dtValue;
                        }
                        if (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() == null || dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() == "")
                        {
                            TempData["Successdata"] = "Kindly do the Risk Evaluation";
                            return RedirectToAction("HazardList");
                        }

                        //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                        //ViewBag.Approver = objGlobaldata.GetGRoleList(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString(), "Approver");

                        HealthSafetyModelsList objRiskList = new HealthSafetyModelsList();
                        objRiskList.lstHazard = new List<HealthSafetyModels>();

                        sSqlstmt = "select mit_id,id_hazard,cnt_type,cnt_desc,pers_resp,target_date from t_hazard_mitigations where id_hazard='" + id_hazard + "'";
                        DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    HealthSafetyModels objMit = new HealthSafetyModels
                                    {
                                        mit_id = dsMitList.Tables[0].Rows[i]["mit_id"].ToString(),
                                        id_hazard = dsMitList.Tables[0].Rows[i]["id_hazard"].ToString(),
                                        cnt_type = dsMitList.Tables[0].Rows[i]["cnt_type"].ToString(),
                                        cnt_desc = dsMitList.Tables[0].Rows[i]["cnt_desc"].ToString(),
                                        pers_resp = dsMitList.Tables[0].Rows[i]["pers_resp"].ToString(),
                                    };
                                    if (DateTime.TryParse(dsMitList.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                                    {
                                        objMit.target_date = dtValue;
                                    }
                                    objRiskList.lstHazard.Add(objMit);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddRiskMitigations: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("HazardList");
                                }
                            }
                            ViewBag.objMitList = objRiskList;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Kindly do Mitigation";
                        return RedirectToAction("HazardList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskMitigation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRiskMgmtModels);
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult AddRiskMitigations(FormCollection form, HealthSafetyModels objRiskMgmt)
        {
            try
            {
                objRiskMgmt.mit_notified_to = form["mit_notified_to"];
                HealthSafetyModelsList objRiskList = new HealthSafetyModelsList();
                objRiskList.lstHazard = new List<HealthSafetyModels>();


                DateTime dateValue;

                if (DateTime.TryParse(form["reeval_due_date"], out dateValue) == true)
                {
                    objRiskMgmt.reeval_due_date = dateValue;
                }
                if (DateTime.TryParse(form["proposed_date"], out dateValue) == true)
                {
                    objRiskMgmt.proposed_date = dateValue;
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    HealthSafetyModels objMitModel = new HealthSafetyModels();
                    if (form["cnt_desc " + i] != "" && form["cnt_desc " + i] != null)
                    {
                        objMitModel.mit_id = form["mit_id " + i];
                        objMitModel.cnt_type = form["cnt_type " + i];
                        objMitModel.cnt_desc = form["cnt_desc " + i];
                        objMitModel.pers_resp = form["pers_resp " + i];
                        if (DateTime.TryParse(form["target_date " + i], out dateValue) == true)
                        {
                            objMitModel.target_date = dateValue;
                        }
                        objRiskList.lstHazard.Add(objMitModel);
                    }
                }
                if (objRiskMgmt.FunUpdateRiskMitigation(objRiskMgmt, objRiskList))
                {
                    TempData["Successdata"] = "Added Mitigation Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskMitigation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddRiskReEvaluation()
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            HealthSafetyModels objHazardModels = new HealthSafetyModels();
            try
            {
                if (Request.QueryString["id_hazard"] != null && Request.QueryString["id_hazard"] != "")
                {
                    string id_hazard = Request.QueryString["id_hazard"];
                    ViewBag.impact_id = objGlobaldata.GetDropdownList("Risk-Severity");
                    ViewBag.like_id = objGlobaldata.GetDropdownList("Risk-likelihood");
                    ViewBag.Legal = objGlobaldata.GetLawNo();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.EvaluatedBy = objGlobaldata.GetDeptHeadbyDivisionList();
                    ViewBag.id_hazard = id_hazard;
                    ViewBag.OP = objGlobaldata.GetConstantValue("HazardOP");
                    DateTime today_date = DateTime.Now;
                    string sSqlstmt = "select tt.id_hazard,t.hazard_refno,t.dept,t.branch_id,tt.Location,t.activity,t.hazards,t.consequences,"
                    + "tt.further_consequences,tt.op_control,tt.cnt_engineering,tt.cnt_administrative,tt.cnt_ppe,tt.cnt_general,tt.cnt_elimination,tt.cnt_substitution,"
                    + "tt.impact_id,tt.like_id,tt.legal,tt.legal_voilation,tt.reeval_due_date,tt.evaluated_by,tt.eval_notified_to,tt.evaluation_date,t.impact_id as initimpact_id,t.like_id as initlike_id,t.evaluation_date as initevaluation_date"
                    + " from t_hazard t, t_hazard_trans tt"
                    +" where t.id_hazard = tt.id_hazard and tt.id_hazard = '"+ id_hazard + "' order by id_hazard_trans desc limit 1";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {

                        objHazardModels = new HealthSafetyModels
                        {
                            id_hazard = (dsRiskModels.Tables[0].Rows[0]["id_hazard"].ToString()),
                            hazard_refno = (dsRiskModels.Tables[0].Rows[0]["hazard_refno"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            activity = (dsRiskModels.Tables[0].Rows[0]["activity"].ToString()),
                            hazards = (dsRiskModels.Tables[0].Rows[0]["hazards"].ToString()),
                            consequences = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["consequences"].ToString()),
                            further_consequences = (dsRiskModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                            op_control = (dsRiskModels.Tables[0].Rows[0]["op_control"].ToString()),
                            cnt_engineering = (dsRiskModels.Tables[0].Rows[0]["cnt_engineering"].ToString()),
                            cnt_administrative = (dsRiskModels.Tables[0].Rows[0]["cnt_administrative"].ToString()),
                            cnt_ppe = (dsRiskModels.Tables[0].Rows[0]["cnt_ppe"].ToString()),
                            cnt_general = (dsRiskModels.Tables[0].Rows[0]["cnt_general"].ToString()),
                            cnt_elimination = (dsRiskModels.Tables[0].Rows[0]["cnt_elimination"].ToString()),
                            cnt_substitution = (dsRiskModels.Tables[0].Rows[0]["cnt_substitution"].ToString()),

                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            legal = (dsRiskModels.Tables[0].Rows[0]["legal"].ToString()),
                            legal_voilation = (dsRiskModels.Tables[0].Rows[0]["legal_voilation"].ToString()),
                            evaluated_by = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["evaluated_by"].ToString()),
                            eval_notified_to = (dsRiskModels.Tables[0].Rows[0]["eval_notified_to"].ToString()),
                            initimpact_id = (dsRiskModels.Tables[0].Rows[0]["initimpact_id"].ToString()),
                            initlike_id = (dsRiskModels.Tables[0].Rows[0]["initlike_id"].ToString()),
                        };
                      
                        DateTime dtValue;
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["initevaluation_date"].ToString(), out dtValue))
                        {
                            objHazardModels.initevaluation_date = dtValue;
                        }
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                        {
                            if (today_date < dtValue)
                            {
                                if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                                {
                                    objHazardModels.evaluation_date = dtValue;
                                }
                            }

                        }
                        else
                        {
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                            {
                                objHazardModels.evaluation_date = dtValue;
                            }
                        }
                       // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());

                    }
                    else if (dsRiskModels.Tables[0].Rows.Count == 0)
                    {
                         sSqlstmt = "select id_hazard,hazard_refno,dept,branch_id,Location,activity,hazards,consequences,"
                   + "further_consequences,op_control,cnt_engineering,cnt_administrative,cnt_ppe,cnt_general,impact_id," +
                   "like_id,legal,legal_voilation,evaluated_by,eval_notified_to,evaluation_date,cnt_elimination,cnt_substitution  from t_hazard where id_hazard='"
                       + id_hazard + "'";

                         dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                        if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                        {

                            objHazardModels = new HealthSafetyModels
                            {
                                id_hazard = (dsRiskModels.Tables[0].Rows[0]["id_hazard"].ToString()),
                                hazard_refno = (dsRiskModels.Tables[0].Rows[0]["hazard_refno"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                                branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                                activity = (dsRiskModels.Tables[0].Rows[0]["activity"].ToString()),
                                hazards = (dsRiskModels.Tables[0].Rows[0]["hazards"].ToString()),
                                consequences = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["consequences"].ToString()),
                                further_consequences = (dsRiskModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                                op_control = (dsRiskModels.Tables[0].Rows[0]["op_control"].ToString()),
                                cnt_engineering = (dsRiskModels.Tables[0].Rows[0]["cnt_engineering"].ToString()),
                                cnt_administrative = (dsRiskModels.Tables[0].Rows[0]["cnt_administrative"].ToString()),
                                cnt_ppe = (dsRiskModels.Tables[0].Rows[0]["cnt_ppe"].ToString()),
                                cnt_general = (dsRiskModels.Tables[0].Rows[0]["cnt_general"].ToString()),
                                cnt_elimination = (dsRiskModels.Tables[0].Rows[0]["cnt_elimination"].ToString()),
                                cnt_substitution = (dsRiskModels.Tables[0].Rows[0]["cnt_substitution"].ToString()),

                                initimpact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                                initlike_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                                legal = (dsRiskModels.Tables[0].Rows[0]["legal"].ToString()),
                                legal_voilation = (dsRiskModels.Tables[0].Rows[0]["legal_voilation"].ToString()),
                                evaluated_by = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["evaluated_by"].ToString()),
                                eval_notified_to = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["eval_notified_to"].ToString()),

                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                            {
                                objHazardModels.initevaluation_date = dtValue;
                            }
                            // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                           
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("HazardList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskReEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objHazardModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult AddRiskReEvaluation(FormCollection form, HealthSafetyModels objHazard)
        {
            try
            {
                objHazard.eval_notified_to = form["eval_notified_to"];
                if (objHazard.op_control1 != "" && objHazard.op_control1 != null)
                {
                    objHazard.op_control = String.Concat("Engineering");
                }

                if (objHazard.op_control2 != "" && objHazard.op_control2 != null)
                {
                    if (objHazard.op_control != "" && objHazard.op_control != null)
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "Administrative");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("Administrative");
                    }
                }

                if (objHazard.op_control3 != "" && objHazard.op_control3 != null)
                {
                    if (objHazard.op_control != "" && objHazard.op_control != null)
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "PPE");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("PPE");
                    }
                }

                if (objHazard.op_control4 != "" && objHazard.op_control4 != null)
                {
                    if (objHazard.op_control != "" && objHazard.op_control != null)
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "General");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("General");
                    }
                }

                if (objHazard.op_control5 != "" && objHazard.op_control5 != null)
                {
                    if (objHazard.op_control != "" && objHazard.op_control != null)
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "Elimination");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("Elimination");
                    }
                }

                if (objHazard.op_control6 != "" && objHazard.op_control6 != null)
                {
                    if (objHazard.op_control != "" && objHazard.op_control != null)
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "Substitution");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("Substitution");
                    }
                }

                if (objHazard.FunUpdateRiskReEvaluation(objHazard))
                {
                    TempData["Successdata"] = "Added Risk ReEvaluation Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskReEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult FurtherRiskMitigations()
        {
            HealthSafetyModels objRiskMgmtModels = new HealthSafetyModels();
            try
            {
                if (Request.QueryString["id_hazard"] != null && Request.QueryString["id_hazard"] != "")
                {
                    string id_hazard = Request.QueryString["id_hazard"];
                    ViewBag.id_hazard = id_hazard;
                    ViewBag.Approver = objGlobaldata.GetApprover();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.Control = objGlobaldata.GetConstantValue("HazardOP");
                    DateTime today_date = DateTime.Now;
                    string sSqlstmt = "select id_hazard_trans,tt.id_hazard,t.branch_id,t.dept,t.Location,tt.hazard_refno,tt.impact_id,tt.like_id,tt.mit_evaluated_by,tt.mit_notified_to,tt.proposed_date,tt.reeval_due_date,tt.evaluation_date,t.source_id,t.Issue,t.activity_type,t.consequences,t.injury,t.activity,t.hazards,t.reported_date,t.reported_by from t_hazard t,t_hazard_trans tt where tt.id_hazard='"
                        + id_hazard + "' and t.id_hazard = tt.id_hazard order by id_hazard_trans desc limit 1";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {

                        objRiskMgmtModels = new HealthSafetyModels
                        {
                            id_hazard_trans = (dsRiskModels.Tables[0].Rows[0]["id_hazard_trans"].ToString()),
                            id_hazard = (dsRiskModels.Tables[0].Rows[0]["id_hazard"].ToString()),
                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            hazard_refno = (dsRiskModels.Tables[0].Rows[0]["hazard_refno"].ToString()),
                            mit_evaluated_by = (dsRiskModels.Tables[0].Rows[0]["mit_evaluated_by"].ToString()),
                            mit_notified_to = (dsRiskModels.Tables[0].Rows[0]["mit_notified_to"].ToString()),

                            dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objRiskMgmtModels.GetRiskSourceNameById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),

                            activity_type = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            consequences = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["consequences"].ToString()),
                            injury = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["injury"].ToString()),
                            activity = (dsRiskModels.Tables[0].Rows[0]["activity"].ToString()),
                            hazards = (dsRiskModels.Tables[0].Rows[0]["hazards"].ToString()),

                            reported_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["reported_by"].ToString()),

                        };
                     
                        DateTime dtValue, dtEval;
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.reported_date = dtValue;
                        }
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["proposed_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.proposed_date = dtValue;
                        }
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtEval))
                        {
                            objRiskMgmtModels.evaluation_date = dtEval;
                        }
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                        {
                            if (today_date < dtValue)
                            {
                                if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                                {
                                    objRiskMgmtModels.reeval_due_date = dtValue;
                                }
                            }
                            else
                            {
                                if (dtValue > dtEval)
                                {
                                    TempData["Successdata"] = "Kindly do Risk ReEvaluation";
                                    return RedirectToAction("HazardList");
                                }
                                else
                                {
                                    if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                                    {
                                        objRiskMgmtModels.reeval_due_date = dtValue;
                                    }
                                }
                            }
                        }
                        //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                        //ViewBag.Approver = objGlobaldata.GetGRoleList(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString(), "Approver");

                        HealthSafetyModelsList objRiskList = new HealthSafetyModelsList();
                        objRiskList.lstHazard = new List<HealthSafetyModels>();

                        sSqlstmt = "select mit_id_trans,id_hazard_trans,mit_id,id_hazard,cnt_type,cnt_desc,pers_resp,target_date from t_hazard_mitigations_trans where id_hazard_trans='" + objRiskMgmtModels.id_hazard_trans + "'";
                        DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    HealthSafetyModels objMit = new HealthSafetyModels
                                    {
                                        mit_id_trans = dsMitList.Tables[0].Rows[i]["mit_id_trans"].ToString(),
                                        id_hazard_trans = dsMitList.Tables[0].Rows[i]["id_hazard_trans"].ToString(),
                                        mit_id = dsMitList.Tables[0].Rows[i]["mit_id"].ToString(),
                                        id_hazard = dsMitList.Tables[0].Rows[i]["id_hazard"].ToString(),
                                        cnt_type = dsMitList.Tables[0].Rows[i]["cnt_type"].ToString(),
                                        cnt_desc = dsMitList.Tables[0].Rows[i]["cnt_desc"].ToString(),
                                        pers_resp = dsMitList.Tables[0].Rows[i]["pers_resp"].ToString(),
                                    };
                                    if (DateTime.TryParse(dsMitList.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                                    {
                                        objMit.target_date = dtValue;
                                    }
                                    objRiskList.lstHazard.Add(objMit);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in FurtherRiskMitigations: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("HazardList");
                                }
                            }
                            ViewBag.objMitList = objRiskList;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Kindly do Mitigation";
                        return RedirectToAction("HazardList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskMitigation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRiskMgmtModels);
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult FurtherRiskMitigations(FormCollection form, HealthSafetyModels objRiskMgmt)
        {
            try
            {
                objRiskMgmt.mit_notified_to = form["mit_notified_to"];
                HealthSafetyModelsList objRiskList = new HealthSafetyModelsList();
                objRiskList.lstHazard = new List<HealthSafetyModels>();


                DateTime dateValue;

                if (DateTime.TryParse(form["reeval_due_date"], out dateValue) == true)
                {
                    objRiskMgmt.reeval_due_date = dateValue;
                }
                if (DateTime.TryParse(form["proposed_date"], out dateValue) == true)
                {
                    objRiskMgmt.proposed_date = dateValue;
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    HealthSafetyModels objMitModel = new HealthSafetyModels();
                    if (form["cnt_desc " + i] != "" && form["cnt_desc " + i] != null)
                    {
                        objMitModel.mit_id_trans = form["mit_id_trans " + i];
                        objMitModel.mit_id = form["mit_id " + i];
                        objMitModel.cnt_type = form["cnt_type " + i];
                        objMitModel.cnt_desc = form["cnt_desc " + i];
                        objMitModel.pers_resp = form["pers_resp " + i];
                        if (DateTime.TryParse(form["target_date " + i], out dateValue) == true)
                        {
                            objMitModel.target_date = dateValue;
                        }
                        objRiskList.lstHazard.Add(objMitModel);
                    }
                }
                if (objRiskMgmt.FunUpdateFurtherRiskMitigation(objRiskMgmt, objRiskList))
                {
                    TempData["Successdata"] = "Added Mitigation Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FurtherRiskMitigations: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult HazardDetails()
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            HealthSafetyModels HazardModels = new HealthSafetyModels();
            HealthSafetyModels objSafety = new HealthSafetyModels();
            try
            {
                if (Request.QueryString["id_hazard"] != null)
                {
                    string sid_hazard = Request.QueryString["id_hazard"];
                    string sSqlstmt = "select id_hazard, hazard_refno, dept, branch_id, Location, source_id, activity_type, consequences, injury, activity, hazards, notified_to, reported_by, reported_date,further_consequences,impact_id,like_id,legal,legal_voilation,Issue from t_hazard where id_hazard = '" + sid_hazard + "'";
                    DataSet dsHazardModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsHazardModels.Tables.Count > 0 && dsHazardModels.Tables[0].Rows.Count > 0)
                    {
                        Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                        if (dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsHazardModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                        {
                            dicRatings = HazardModels.GetRiskRatings(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString(),
                                dsHazardModels.Tables[0].Rows[0]["like_id"].ToString());
                        }
                        HazardModels = new HealthSafetyModels
                        {
                            id_hazard = (dsHazardModels.Tables[0].Rows[0]["id_hazard"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsHazardModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objSafety.GetRiskSourceNameById(dsHazardModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsHazardModels.Tables[0].Rows[0]["Location"].ToString()),
                            hazard_refno = (dsHazardModels.Tables[0].Rows[0]["hazard_refno"].ToString()),
                            activity_type = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            consequences = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["consequences"].ToString()),
                            injury = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["injury"].ToString()),
                            activity = (dsHazardModels.Tables[0].Rows[0]["activity"].ToString()),
                            hazards = (dsHazardModels.Tables[0].Rows[0]["hazards"].ToString()),
                            notified_to = (dsHazardModels.Tables[0].Rows[0]["notified_to"].ToString()),
                            reported_by = objGlobaldata.GetEmpHrNameById(dsHazardModels.Tables[0].Rows[0]["reported_by"].ToString()),
                            further_consequences = (dsHazardModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                            legal =objGlobaldata.GetLawNoById(dsHazardModels.Tables[0].Rows[0]["legal"].ToString()),
                            legal_voilation = (dsHazardModels.Tables[0].Rows[0]["legal_voilation"].ToString()),
                            impact_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = objSafety.GetLikelihoodNameById(dsHazardModels.Tables[0].Rows[0]["like_id"].ToString()),
                            Issue = objRisk.GetIssueNameById(dsHazardModels.Tables[0].Rows[0]["Issue"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsHazardModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                        {
                            HazardModels.reported_date = dtValue;
                        }
                        if (dicRatings != null && dicRatings.Count > 0)
                        {
                            HazardModels.RiskRating = dicRatings.FirstOrDefault().Key;
                            HazardModels.color_code = dicRatings.FirstOrDefault().Value;
                        }
                        sSqlstmt = "select mit_id,id_hazard,cnt_type,cnt_desc,pers_resp,target_date from t_hazard_mitigations where id_hazard='" + sid_hazard + "'";
                        ViewBag.Mit = objGlobaldata.Getdetails(sSqlstmt);

                        return View(HazardModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("HazardList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("HazardList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HazardList");
        }

        [AllowAnonymous]
        public ActionResult EditHazard()
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            HealthSafetyModels HazardModels = new HealthSafetyModels();
            HealthSafetyModels objSafety = new HealthSafetyModels();
            try
            {
                if (Request.QueryString["id_hazard"] != null)
                {
                    //objSafety.branch_id = objGlobaldata.GetCurrentUserSession().division;
                    //objSafety.dept = objGlobaldata.GetCurrentUserSession().DeptID;
                    //objSafety.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
               
                ViewBag.source_id = objSafety.GetMultiRiskSourceList();
                ViewBag.Activity = objGlobaldata.GetDropdownList("HS_activity_type");
                ViewBag.Consequences = objGlobaldata.GetDropdownList("HS_consequences");
                ViewBag.Injury = objGlobaldata.GetDropdownList("HS_injury");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Issue = objRisk.GetIsssuesNo();
                    string sid_hazard = Request.QueryString["id_hazard"];
                    string sSqlstmt = "select id_hazard, hazard_refno, dept, branch_id, Location, source_id, activity_type, consequences, injury, activity, hazards, notified_to, reported_by, reported_date,further_consequences,impact_id,like_id,legal,legal_voilation,Issue from t_hazard where id_hazard = '" + sid_hazard + "'";
                    DataSet dsHazardModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsHazardModels.Tables.Count > 0 && dsHazardModels.Tables[0].Rows.Count > 0)
                    {
                        //Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                        //if (dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsHazardModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                        //{
                        //    dicRatings = HazardModels.GetRiskRatings(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString(),
                        //        dsHazardModels.Tables[0].Rows[0]["like_id"].ToString());
                        //}
                        HazardModels = new HealthSafetyModels
                        {
                            id_hazard = (dsHazardModels.Tables[0].Rows[0]["id_hazard"].ToString()),
                            dept = (dsHazardModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = (dsHazardModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objSafety.GetRiskSourceNameById(dsHazardModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Location = (dsHazardModels.Tables[0].Rows[0]["Location"].ToString()),
                            hazard_refno = (dsHazardModels.Tables[0].Rows[0]["hazard_refno"].ToString()),
                            activity_type = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            consequences =/* objGlobaldata.GetDropdownitemById*/(dsHazardModels.Tables[0].Rows[0]["consequences"].ToString()),
                            injury = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["injury"].ToString()),
                            activity = (dsHazardModels.Tables[0].Rows[0]["activity"].ToString()),
                            hazards = (dsHazardModels.Tables[0].Rows[0]["hazards"].ToString()),
                            notified_to = (dsHazardModels.Tables[0].Rows[0]["notified_to"].ToString()),
                            reported_by = (dsHazardModels.Tables[0].Rows[0]["reported_by"].ToString()),
                            further_consequences = (dsHazardModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                            legal = objGlobaldata.GetLawNoById(dsHazardModels.Tables[0].Rows[0]["legal"].ToString()),
                            legal_voilation = (dsHazardModels.Tables[0].Rows[0]["legal_voilation"].ToString()),
                            impact_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = objSafety.GetLikelihoodNameById(dsHazardModels.Tables[0].Rows[0]["like_id"].ToString()),
                            Issue = (dsHazardModels.Tables[0].Rows[0]["Issue"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsHazardModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                        {
                            HazardModels.reported_date = dtValue;
                        }
                       // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsHazardModels.Tables[0].Rows[0]["branch_id"].ToString(), dsHazardModels.Tables[0].Rows[0]["dept"].ToString(), dsHazardModels.Tables[0].Rows[0]["Location"].ToString());
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsHazardModels.Tables[0].Rows[0]["branch_id"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsHazardModels.Tables[0].Rows[0]["branch_id"].ToString());
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        //if (dicRatings != null && dicRatings.Count > 0)
                        //{
                        //    HazardModels.RiskRating = dicRatings.FirstOrDefault().Key;
                        //    HazardModels.color_code = dicRatings.FirstOrDefault().Value;
                        //}


                        return View(HazardModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("HazardList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("HazardList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HazardList");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditHazard(HealthSafetyModels objHazard, FormCollection form)
        {
            try
            {
                objHazard.notified_to = form["notified_to"];
                objHazard.consequences = form["consequences"];
                if (objHazard.FunUpdateHazard(objHazard))
                {
                    TempData["Successdata"] = "Updated Hazard details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EditHazard: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json(true);
        }


        [AllowAnonymous]
        public JsonResult HazardDelete(FormCollection form)
        {
            try
            {

                if (form["id_hazard"] != null && form["id_hazard"] != "")
                {

                    HealthSafetyModels Doc = new HealthSafetyModels();
                    string sid_hazard = form["id_hazard"];


                    if (Doc.FunDeleteHazard(sid_hazard))
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
                objGlobaldata.AddFunctionalLog("Exception in HazardDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult HazardListSearch(FormCollection form, int? page, string branch_name)
        {
            HealthSafetyModelsList objHazardModelsList = new HealthSafetyModelsList();
            objHazardModelsList.lstHazard = new List<HealthSafetyModels>();
            HealthSafetyModels objSafety = new HealthSafetyModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_hazard,hazard_refno,dept,branch_id,Location,source_id,activity_type,consequences,injury,activity,hazards,notified_to,reported_by,reported_date from t_hazard where Active=1";
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch_id)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch_id)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "order by id_hazard desc";
                DataSet dsHazardModels = objGlobaldata.Getdetails(sSqlstmt);

                if (dsHazardModels.Tables.Count > 0 && dsHazardModels.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsHazardModels.Tables[0].Rows.Count; i++)
                    {
                        //Dictionary<string, string> dicRatings = new Dictionary<string, string>();

                        //if (dsRiskModels.Tables[0].Rows[i]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[i]["like_id"].ToString() != "")
                        //{
                        //    dicRatings = objRisk.GetRiskRatings(dsRiskModels.Tables[0].Rows[i]["impact_id"].ToString(),
                        //    dsRiskModels.Tables[0].Rows[i]["like_id"].ToString());
                        //}

                        try
                        {
                            HealthSafetyModels objHazard = new HealthSafetyModels
                            {
                                id_hazard = (dsHazardModels.Tables[0].Rows[i]["id_hazard"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsHazardModels.Tables[0].Rows[i]["dept"].ToString()),
                                branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardModels.Tables[0].Rows[i]["branch_id"].ToString()),
                                source_id = objSafety.GetRiskSourceNameById(dsHazardModels.Tables[0].Rows[i]["source_id"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsHazardModels.Tables[0].Rows[i]["Location"].ToString()),
                                hazard_refno = (dsHazardModels.Tables[0].Rows[i]["hazard_refno"].ToString()),
                                activity_type = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["activity_type"].ToString()),
                                consequences = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["consequences"].ToString()),
                                injury = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["injury"].ToString()),
                                activity = (dsHazardModels.Tables[0].Rows[i]["activity"].ToString()),
                                hazards = (dsHazardModels.Tables[0].Rows[i]["hazards"].ToString()),
                                notified_to = (dsHazardModels.Tables[0].Rows[i]["notified_to"].ToString()),
                                reported_by = (dsHazardModels.Tables[0].Rows[i]["reported_by"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsHazardModels.Tables[0].Rows[i]["reported_date"].ToString(), out dtValue))
                            {
                                objHazard.reported_date = dtValue;
                            }
                            //if (dicRatings != null && dicRatings.Count > 0)
                            //{
                            //    objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
                            //    objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
                            //}

                            //string sql = "select t.mit_id from risk_mitigations t,risk_register tt where t.risk_id = tt.risk_id and"
                            //+ " t.risk_id = '" + dsRiskModels.Tables[0].Rows[i]["risk_id"].ToString() + "'";
                            //DataSet dsRisk = objGlobaldata.Getdetails(sSqlstmt);

                            //if (dsRisk.Tables.Count > 0 && dsRisk.Tables[0].Rows.Count > 0)
                            //{
                            //    objRiskMgmtModels.mitmeasure = "Yes";
                            //}
                            //else
                            //{
                            //    objRiskMgmtModels.mitmeasure = "No";
                            //}
                            objHazardModelsList.lstHazard.Add(objHazard);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in HazardListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult HazardHistoryList()
        {
            HealthSafetyModelsList objHazardModelsList = new HealthSafetyModelsList();
            objHazardModelsList.lstHazard = new List<HealthSafetyModels>();
            HealthSafetyModels objRisk = new HealthSafetyModels();

            try
            {
                if (Request.QueryString["id_hazard"] != null)
                {
                    string sid_hazard = Request.QueryString["id_hazard"];
                    string sSqlstmt = "select tt.id_hazard_trans,tt.id_hazard,tt.hazard_refno,tt.dept,tt.branch_id,tt.Location,"
                    +" tt.source_id,t.activity_type,t.consequences,tt.injury,tt.activity,tt.hazards,tt.notified_to,"
                    +" tt.reported_by,tt.reported_date,t.impact_id,t.like_id,tt.evaluated_by,tt.evaluation_date,tt.reeval_due_date"
                    +" from t_hazard_trans tt, t_hazard t"
                    +" where t.id_hazard = tt.id_hazard"
                    +" and tt.id_hazard = '" + sid_hazard + "'";

                    DataSet dsHazardModels = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsHazardModels.Tables.Count > 0 && dsHazardModels.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; dsHazardModels.Tables.Count > 0 && i < dsHazardModels.Tables[0].Rows.Count; i++)
                        {
                            Dictionary<string, string> dicRatings = new Dictionary<string, string>();

                            if (dsHazardModels.Tables[0].Rows[i]["impact_id"].ToString() != "" && dsHazardModels.Tables[0].Rows[i]["like_id"].ToString() != "")
                            {
                                dicRatings = objRisk.GetRiskRatings(dsHazardModels.Tables[0].Rows[i]["impact_id"].ToString(),
                                dsHazardModels.Tables[0].Rows[i]["like_id"].ToString());
                            }

                            try
                            {
                                HealthSafetyModels objRiskMgmtModels = new HealthSafetyModels
                                {
                                    id_hazard_trans = (dsHazardModels.Tables[0].Rows[i]["id_hazard_trans"].ToString()),
                                    id_hazard = (dsHazardModels.Tables[0].Rows[i]["id_hazard"].ToString()),
                                    dept = objGlobaldata.GetMultiDeptNameById(dsHazardModels.Tables[0].Rows[i]["dept"].ToString()),
                                    branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardModels.Tables[0].Rows[i]["branch_id"].ToString()),
                                    source_id = objRisk.GetRiskSourceNameById(dsHazardModels.Tables[0].Rows[i]["source_id"].ToString()),
                                    Location = objGlobaldata.GetDivisionLocationById(dsHazardModels.Tables[0].Rows[i]["Location"].ToString()),
                                    hazard_refno = (dsHazardModels.Tables[0].Rows[i]["hazard_refno"].ToString()),
                                    activity_type = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["activity_type"].ToString()),
                                    consequences = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["consequences"].ToString()),
                                    injury = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["injury"].ToString()),
                                    activity = (dsHazardModels.Tables[0].Rows[i]["activity"].ToString()),
                                    hazards = (dsHazardModels.Tables[0].Rows[i]["hazards"].ToString()),
                                    notified_to = (dsHazardModels.Tables[0].Rows[i]["notified_to"].ToString()),
                                    evaluated_by = objGlobaldata.GetEmpHrNameById(dsHazardModels.Tables[0].Rows[i]["evaluated_by"].ToString()),
                                    impact_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["impact_id"].ToString()),
                                    like_id = objRisk.GetLikelihoodNameById(dsHazardModels.Tables[0].Rows[i]["like_id"].ToString()),
                                };
                                DateTime dtValue;
                                if (DateTime.TryParse(dsHazardModels.Tables[0].Rows[i]["evaluation_date"].ToString(), out dtValue))
                                {
                                    objRiskMgmtModels.evaluation_date = dtValue;
                                }
                                if (DateTime.TryParse(dsHazardModels.Tables[0].Rows[i]["reeval_due_date"].ToString(), out dtValue))
                                {
                                    objRiskMgmtModels.reeval_due_date = dtValue;
                                }
                                if (dicRatings != null && dicRatings.Count > 0)
                                {
                                    objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
                                    objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
                                }

                                objHazardModelsList.lstHazard.Add(objRiskMgmtModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in HazardHistoryList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["Successdata"] = "No Data Exists";
                        return RedirectToAction("HazardList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("HazardList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardHistoryList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objHazardModelsList.lstHazard.ToList());
        }

        [AllowAnonymous]
        public ActionResult HazardHistoryDetails()
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            HealthSafetyModels HazardModels = new HealthSafetyModels();
            HealthSafetyModels objSafety = new HealthSafetyModels();
            try
            {
                if (Request.QueryString["id_hazard_trans"] != null)
                {
                    string id_hazard_trans = Request.QueryString["id_hazard_trans"];
                    string sSqlstmt = "select tt.id_hazard, t.hazard_refno, t.dept, t.branch_id, t.Location, t.source_id, t.activity_type,"
                    +"t.consequences, t.injury, t.activity, t.hazards, t.notified_to, t.reported_by,"
                    + " t.reported_date,t.further_consequences,tt.impact_id,tt.like_id,t.legal,t.legal_voilation,t.Issue"
                    + " from t_hazard_trans tt, t_hazard t"
                    +" where t.id_hazard = tt.id_hazard"
                    +" and id_hazard_trans = '"+ id_hazard_trans + "'";
                    DataSet dsHazardModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsHazardModels.Tables.Count > 0 && dsHazardModels.Tables[0].Rows.Count > 0)
                    {
                        Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                        if (dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsHazardModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                        {
                            dicRatings = HazardModels.GetRiskRatings(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString(),
                                dsHazardModels.Tables[0].Rows[0]["like_id"].ToString());
                        }
                        HazardModels = new HealthSafetyModels
                        {
                            id_hazard = (dsHazardModels.Tables[0].Rows[0]["id_hazard"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsHazardModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objSafety.GetRiskSourceNameById(dsHazardModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsHazardModels.Tables[0].Rows[0]["Location"].ToString()),
                            hazard_refno = (dsHazardModels.Tables[0].Rows[0]["hazard_refno"].ToString()),
                            activity_type = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            consequences = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["consequences"].ToString()),
                            injury = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["injury"].ToString()),
                            activity = (dsHazardModels.Tables[0].Rows[0]["activity"].ToString()),
                            hazards = (dsHazardModels.Tables[0].Rows[0]["hazards"].ToString()),
                            notified_to = (dsHazardModels.Tables[0].Rows[0]["notified_to"].ToString()),
                            reported_by = (dsHazardModels.Tables[0].Rows[0]["reported_by"].ToString()),
                            further_consequences = (dsHazardModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                            legal = objGlobaldata.GetLawNoById(dsHazardModels.Tables[0].Rows[0]["legal"].ToString()),
                            legal_voilation = (dsHazardModels.Tables[0].Rows[0]["legal_voilation"].ToString()),
                            impact_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = objSafety.GetLikelihoodNameById(dsHazardModels.Tables[0].Rows[0]["like_id"].ToString()),
                            Issue = objRiskMgmtModels.GetIssueNameById(dsHazardModels.Tables[0].Rows[0]["Issue"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsHazardModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                        {
                            HazardModels.reported_date = dtValue;
                        }
                        if (dicRatings != null && dicRatings.Count > 0)
                        {
                            HazardModels.RiskRating = dicRatings.FirstOrDefault().Key;
                            HazardModels.color_code = dicRatings.FirstOrDefault().Value;
                        }
                        sSqlstmt = "select mit_id,id_hazard,cnt_type,cnt_desc,pers_resp,target_date from t_hazard_mitigations_trans where id_hazard_trans='" + id_hazard_trans + "'";
                        ViewBag.Mit = objGlobaldata.Getdetails(sSqlstmt);

                        return View(HazardModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("HazardList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("HazardList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardHistoryDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HazardList");
        }
    }
}