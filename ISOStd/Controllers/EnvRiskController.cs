using ISOStd.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    public class EnvRiskController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public EnvRiskController()
        {
            ViewBag.Menutype = "Risk";
            ViewBag.SubMenutype = "Env";
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddEnvRisk(string id_issue)
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            EnvRiskModels objHazard = new EnvRiskModels();
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
                //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(objHazard.branch_id, objHazard.dept, objHazard.Location);

                ViewBag.source_id = objHazard.GetMultiRiskSourceList();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Impact = objGlobaldata.GetDropdownList("Environmental Impact");
                ViewBag.Activity = objGlobaldata.GetDropdownList("HS_activity_type");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEnvRisk: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objHazard);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddEnvRisk(EnvRiskModels objHazard, FormCollection form)
        {
            try
            {
                if (objHazard.FunAddHazard(objHazard))
                {
                    TempData["Successdata"] = "Added Hazard details successfully with Reference Number '" + objHazard.env_refno + "'";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEnvRisk: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult EnvList(string SearchText, string risk_status_id, int? page, string branch_name)
        {
            EnvRiskModelsList objHazardModelsList = new EnvRiskModelsList();
            objHazardModelsList.lstHazard = new List<EnvRiskModels>();
            EnvRiskModels objSafety = new EnvRiskModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
                ViewBag.IssueCategory = objGlobaldata.GetDropdownList("Issue Category Type");
                string sSqlstmt = "select id_env_risk,env_refno,impact_id,like_id,dept,branch_id,Location," +
                    "source_id,activity,product,aspects,impact,area_affected,notified_to,reported_by,reported_date from t_environment_risk where Active=1";
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch_id)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch_id)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_env_risk desc";

                DataSet dsHazardModels = objGlobaldata.Getdetails(sSqlstmt);

                if (dsHazardModels.Tables.Count > 0 && dsHazardModels.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsHazardModels.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> dicRatings = new Dictionary<string, string>();

                        if (dsHazardModels.Tables[0].Rows[i]["impact_id"].ToString() != "" && dsHazardModels.Tables[0].Rows[i]["like_id"].ToString() != "")
                        {
                            dicRatings = objSafety.GetRiskRatings(dsHazardModels.Tables[0].Rows[i]["impact_id"].ToString(),
                            dsHazardModels.Tables[0].Rows[i]["like_id"].ToString());
                        }

                        try
                        {
                            EnvRiskModels objHazard = new EnvRiskModels
                            {
                                id_env_risk = (dsHazardModels.Tables[0].Rows[i]["id_env_risk"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsHazardModels.Tables[0].Rows[i]["dept"].ToString()),
                                branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardModels.Tables[0].Rows[i]["branch_id"].ToString()),
                                source_id = objSafety.GetRiskSourceNameById(dsHazardModels.Tables[0].Rows[i]["source_id"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsHazardModels.Tables[0].Rows[i]["Location"].ToString()),
                                product = (dsHazardModels.Tables[0].Rows[i]["product"].ToString()),
                                activity = (dsHazardModels.Tables[0].Rows[i]["activity"].ToString()),
                                aspects = (dsHazardModels.Tables[0].Rows[i]["aspects"].ToString()),
                                impact = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["impact"].ToString()),
                                area_affected = (dsHazardModels.Tables[0].Rows[i]["area_affected"].ToString()),
                                notified_to = (dsHazardModels.Tables[0].Rows[i]["notified_to"].ToString()),
                                reported_by = (dsHazardModels.Tables[0].Rows[i]["reported_by"].ToString()),
                                env_refno = (dsHazardModels.Tables[0].Rows[i]["env_refno"].ToString()),
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

                            string sql = "select t.mit_id from t_environment_risk_mitigations t,t_environment_risk tt where t.id_env_risk = tt.id_env_risk and"
                            + " t.id_env_risk = '" + dsHazardModels.Tables[0].Rows[i]["id_env_risk"].ToString() + "'";
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
                            objGlobaldata.AddFunctionalLog("Exception in EnvList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EnvList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objHazardModelsList.lstHazard.ToList());
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddRiskEvaluation()
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            EnvRiskModels objHazardModels = new EnvRiskModels();
            try
            {
                if (Request.QueryString["id_env_risk"] != null && Request.QueryString["id_env_risk"] != "")
                {
                    string id_env_risk = Request.QueryString["id_env_risk"];
                    ViewBag.impact_id = objGlobaldata.GetDropdownList("Risk-Severity");
                    ViewBag.like_id = objGlobaldata.GetDropdownList("Risk-likelihood");
                    ViewBag.Legal = objGlobaldata.GetLawNo();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.EvaluatedBy = objGlobaldata.GetDeptHeadbyDivisionList();
                    ViewBag.id_env_risk = id_env_risk;
                    ViewBag.OP = objGlobaldata.GetConstantValue("HazardOP");
                    string sSqlstmt = "select id_env_risk,env_refno,dept,branch_id,Location,activity,activity_type,aspects,impact,"
                    + "further_consequences,op_control,cnt_engineering,cnt_administrative,cnt_ppe,cnt_general,impact_id,like_id,legal,legal_voilation,evaluated_by,eval_notified_to,evaluation_date  from t_environment_risk where id_env_risk='"
                        + id_env_risk + "'";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {
                        objHazardModels = new EnvRiskModels
                        {
                            id_env_risk = (dsRiskModels.Tables[0].Rows[0]["id_env_risk"].ToString()),
                            env_refno = (dsRiskModels.Tables[0].Rows[0]["env_refno"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            activity = (dsRiskModels.Tables[0].Rows[0]["activity"].ToString()),
                            activity_type = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            aspects = (dsRiskModels.Tables[0].Rows[0]["aspects"].ToString()),
                            impact = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact"].ToString()),
                            further_consequences = (dsRiskModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                            op_control = (dsRiskModels.Tables[0].Rows[0]["op_control"].ToString()),
                            cnt_engineering = (dsRiskModels.Tables[0].Rows[0]["cnt_engineering"].ToString()),
                            cnt_administrative = (dsRiskModels.Tables[0].Rows[0]["cnt_administrative"].ToString()),
                            cnt_ppe = (dsRiskModels.Tables[0].Rows[0]["cnt_ppe"].ToString()),
                            cnt_general = (dsRiskModels.Tables[0].Rows[0]["cnt_general"].ToString()),

                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            legal = (dsRiskModels.Tables[0].Rows[0]["legal"].ToString()),
                            legal_voilation = (dsRiskModels.Tables[0].Rows[0]["legal_voilation"].ToString()),
                            evaluated_by = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["evaluated_by"].ToString()),
                            eval_notified_to = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["eval_notified_to"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                        {
                            objHazardModels.evaluation_date = dtValue;
                        }
                        //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                    }
                    else
                    {
                        TempData["alertdata"] = "Kindly do Evaluation";
                        return RedirectToAction("EnvList");
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
        public JsonResult AddRiskEvaluation(FormCollection form, EnvRiskModels objHazard)
        {
            try
            {
                if (objHazard.op_control1 != "")
                {
                    objHazard.op_control = String.Concat("Engineering");
                }
                if (objHazard.op_control2 != "")
                {
                    if (objHazard.op_control != "")
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "Administrative");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("Administrative");
                    }
                }
                if (objHazard.op_control3 != "")
                {
                    if (objHazard.op_control != "")
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "PPE");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("PPE");
                    }
                }
                if (objHazard.op_control4 != "")
                {
                    if (objHazard.op_control != "")
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "General");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("General");
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
            EnvRiskModels objRiskMgmtModels = new EnvRiskModels();
            try
            {
                if (Request.QueryString["id_env_risk"] != null && Request.QueryString["id_env_risk"] != "")
                {
                    string id_env_risk = Request.QueryString["id_env_risk"];
                    ViewBag.id_env_risk = id_env_risk;
                    ViewBag.Approver = objGlobaldata.GetApprover();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.Control = objGlobaldata.GetConstantValue("HazardOP");
                    string sSqlstmt = "select id_env_risk,branch_id,dept,Location,env_refno,impact_id,like_id,mit_evaluated_by,mit_notified_to," +
                        "proposed_date,reeval_due_date,source_id,Issue,impact,area_affected,activity,product,aspects,reported_date,reported_by,notified_to,activity_type from t_environment_risk where id_env_risk='"
                        + id_env_risk + "'";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {
                        objRiskMgmtModels = new EnvRiskModels
                        {
                            id_env_risk = (dsRiskModels.Tables[0].Rows[0]["id_env_risk"].ToString()),
                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            env_refno = (dsRiskModels.Tables[0].Rows[0]["env_refno"].ToString()),
                            mit_evaluated_by = (dsRiskModels.Tables[0].Rows[0]["mit_evaluated_by"].ToString()),
                            mit_notified_to = (dsRiskModels.Tables[0].Rows[0]["mit_notified_to"].ToString()),

                            dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objRiskMgmtModels.GetRiskSourceNameById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),

                            activity_type = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            activity = (dsRiskModels.Tables[0].Rows[0]["activity"].ToString()),
                            product = (dsRiskModels.Tables[0].Rows[0]["product"].ToString()),
                            aspects = (dsRiskModels.Tables[0].Rows[0]["aspects"].ToString()),
                            impact = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact"].ToString()),
                            area_affected = (dsRiskModels.Tables[0].Rows[0]["area_affected"].ToString()),
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
                            return RedirectToAction("EnvList");
                        }
                        // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                        // ViewBag.Approver = objGlobaldata.GetGRoleList(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString(), "Approver");

                        EnvRiskModelsList objRiskList = new EnvRiskModelsList();
                        objRiskList.lstHazard = new List<EnvRiskModels>();

                        sSqlstmt = "select mit_id,id_env_risk,cnt_type,cnt_desc,pers_resp,target_date from t_environment_risk_mitigations where id_env_risk='" + id_env_risk + "'";
                        DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EnvRiskModels objMit = new EnvRiskModels
                                    {
                                        mit_id = dsMitList.Tables[0].Rows[i]["mit_id"].ToString(),
                                        id_env_risk = dsMitList.Tables[0].Rows[i]["id_env_risk"].ToString(),
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
                                    return RedirectToAction("EnvList");
                                }
                            }
                            ViewBag.objMitList = objRiskList;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Kindly do Mitigation";
                        return RedirectToAction("EnvList");
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

        [AllowAnonymous]
        public JsonResult AddRiskMitigations(FormCollection form, EnvRiskModels objRiskMgmt)
        {
            try
            {
                EnvRiskModelsList objRiskList = new EnvRiskModelsList();
                objRiskList.lstHazard = new List<EnvRiskModels>();

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
                    EnvRiskModels objMitModel = new EnvRiskModels();
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
            EnvRiskModels objHazardModels = new EnvRiskModels();
            try
            {
                if (Request.QueryString["id_env_risk"] != null && Request.QueryString["id_env_risk"] != "")
                {
                    string id_env_risk = Request.QueryString["id_env_risk"];
                    ViewBag.impact_id = objGlobaldata.GetDropdownList("Risk-Severity");
                    ViewBag.like_id = objGlobaldata.GetDropdownList("Risk-likelihood");
                    ViewBag.Legal = objGlobaldata.GetLawNo();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.EvaluatedBy = objGlobaldata.GetDeptHeadbyDivisionList();
                    ViewBag.id_env_risk = id_env_risk;
                    ViewBag.OP = objGlobaldata.GetConstantValue("HazardOP");
                    DateTime today_date = DateTime.Now;
                    string sSqlstmt = "select tt.id_env_risk,t.env_refno,t.dept,t.branch_id,t.Location,t.activity,t.activity_type,t.aspects,t.impact,"
    + " tt.further_consequences,tt.op_control,tt.cnt_engineering,tt.cnt_administrative,tt.cnt_ppe,tt.cnt_general,"
    + " tt.impact_id,tt.like_id,tt.legal,tt.legal_voilation,tt.evaluated_by,tt.eval_notified_to,tt.evaluation_date,tt.reeval_due_date,t.impact_id as initimpact_id,t.like_id as initlike_id,t.evaluation_date as initevaluation_date"
    + " from t_environment_risk t, t_environment_risk_trans tt"
    + " where t.id_env_risk = tt.id_env_risk and tt.id_env_risk = '" + id_env_risk + "' order by id_env_risk_trans desc limit 1";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {
                        objHazardModels = new EnvRiskModels
                        {
                            id_env_risk = (dsRiskModels.Tables[0].Rows[0]["id_env_risk"].ToString()),
                            env_refno = (dsRiskModels.Tables[0].Rows[0]["env_refno"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            activity = (dsRiskModels.Tables[0].Rows[0]["activity"].ToString()),
                            activity_type = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            aspects = (dsRiskModels.Tables[0].Rows[0]["aspects"].ToString()),
                            impact = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact"].ToString()),
                            further_consequences = (dsRiskModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                            op_control = (dsRiskModels.Tables[0].Rows[0]["op_control"].ToString()),
                            cnt_engineering = (dsRiskModels.Tables[0].Rows[0]["cnt_engineering"].ToString()),
                            cnt_administrative = (dsRiskModels.Tables[0].Rows[0]["cnt_administrative"].ToString()),
                            cnt_ppe = (dsRiskModels.Tables[0].Rows[0]["cnt_ppe"].ToString()),
                            cnt_general = (dsRiskModels.Tables[0].Rows[0]["cnt_general"].ToString()),

                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            legal = (dsRiskModels.Tables[0].Rows[0]["legal"].ToString()),
                            legal_voilation = (dsRiskModels.Tables[0].Rows[0]["legal_voilation"].ToString()),
                            evaluated_by = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["evaluated_by"].ToString()),
                            eval_notified_to = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["eval_notified_to"].ToString()),
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
                        //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                    }
                    else if (dsRiskModels.Tables[0].Rows.Count == 0)
                    {
                        sSqlstmt = "select id_env_risk,env_refno,dept,branch_id,Location,activity,activity_type,aspects,impact,"
                        + " further_consequences,op_control,cnt_engineering,cnt_administrative,cnt_ppe,cnt_general,"
                        + " impact_id,like_id,legal,legal_voilation,evaluated_by,eval_notified_to,evaluation_date"
                        + " from t_environment_risk where id_env_risk='" + id_env_risk + "'";

                        dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                        if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                        {
                            objHazardModels = new EnvRiskModels
                            {
                                id_env_risk = (dsRiskModels.Tables[0].Rows[0]["id_env_risk"].ToString()),
                                env_refno = (dsRiskModels.Tables[0].Rows[0]["env_refno"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                                branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                                activity = (dsRiskModels.Tables[0].Rows[0]["activity"].ToString()),
                                activity_type = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["activity_type"].ToString()),
                                aspects = (dsRiskModels.Tables[0].Rows[0]["aspects"].ToString()),
                                impact = (dsRiskModels.Tables[0].Rows[0]["impact"].ToString()),
                                further_consequences = (dsRiskModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                                op_control = (dsRiskModels.Tables[0].Rows[0]["op_control"].ToString()),
                                cnt_engineering = (dsRiskModels.Tables[0].Rows[0]["cnt_engineering"].ToString()),
                                cnt_administrative = (dsRiskModels.Tables[0].Rows[0]["cnt_administrative"].ToString()),
                                cnt_ppe = (dsRiskModels.Tables[0].Rows[0]["cnt_ppe"].ToString()),
                                cnt_general = (dsRiskModels.Tables[0].Rows[0]["cnt_general"].ToString()),

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
                        return RedirectToAction("EnvList");
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
        public JsonResult AddRiskReEvaluation(FormCollection form, EnvRiskModels objHazard)
        {
            try
            {
                if (objHazard.op_control1 != "")
                {
                    objHazard.op_control = String.Concat("Engineering");
                }
                if (objHazard.op_control2 != "")
                {
                    if (objHazard.op_control != "")
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "Administrative");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("Administrative");
                    }
                }
                if (objHazard.op_control3 != "")
                {
                    if (objHazard.op_control != "")
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "PPE");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("PPE");
                    }
                }
                if (objHazard.op_control4 != "")
                {
                    if (objHazard.op_control != "")
                    {
                        objHazard.op_control = String.Concat(objHazard.op_control, ',', "General");
                    }
                    else
                    {
                        objHazard.op_control = String.Concat("General");
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
            EnvRiskModels objRiskMgmtModels = new EnvRiskModels();
            try
            {
                if (Request.QueryString["id_env_risk"] != null && Request.QueryString["id_env_risk"] != "")
                {
                    string id_env_risk = Request.QueryString["id_env_risk"];
                    ViewBag.id_env_risk = id_env_risk;
                    ViewBag.Approver = objGlobaldata.GetApprover();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.Control = objGlobaldata.GetConstantValue("HazardOP");
                    DateTime today_date = DateTime.Now;
                    string sSqlstmt = "select id_env_risk_trans,t.branch_id,t.dept,t.Location,tt.id_env_risk,tt.env_refno,tt.impact_id,tt.like_id,tt.mit_evaluated_by," +
                        "tt.mit_notified_to,tt.proposed_date,tt.reeval_due_date,tt.evaluation_date,t.source_id,t.Issue,t.impact,t.area_affected,t.activity,t.product,t.aspects,t.reported_date,t.reported_by,t.notified_to,t.activity_type from t_environment_risk t,t_environment_risk_trans tt where tt.id_env_risk='"
                        + id_env_risk + "' and t.id_env_risk=tt.id_env_risk order by id_env_risk_trans desc limit 1";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {
                        objRiskMgmtModels = new EnvRiskModels
                        {
                            id_env_risk_trans = (dsRiskModels.Tables[0].Rows[0]["id_env_risk_trans"].ToString()),
                            id_env_risk = (dsRiskModels.Tables[0].Rows[0]["id_env_risk"].ToString()),
                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            env_refno = (dsRiskModels.Tables[0].Rows[0]["env_refno"].ToString()),
                            mit_evaluated_by = (dsRiskModels.Tables[0].Rows[0]["mit_evaluated_by"].ToString()),
                            mit_notified_to = (dsRiskModels.Tables[0].Rows[0]["mit_notified_to"].ToString()),

                            dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objRiskMgmtModels.GetRiskSourceNameById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),

                            activity_type = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            activity = (dsRiskModels.Tables[0].Rows[0]["activity"].ToString()),
                            product = (dsRiskModels.Tables[0].Rows[0]["product"].ToString()),
                            aspects = (dsRiskModels.Tables[0].Rows[0]["aspects"].ToString()),
                            impact = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact"].ToString()),
                            area_affected = (dsRiskModels.Tables[0].Rows[0]["area_affected"].ToString()),
                            reported_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["reported_by"].ToString()),
                        };

                        DateTime dtValue, dtEval;
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["proposed_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.proposed_date = dtValue;
                        }
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtEval))
                        {
                            objRiskMgmtModels.evaluation_date = dtEval;
                        }
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtEval))
                        {
                            objRiskMgmtModels.reported_date = dtEval;
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
                                    return RedirectToAction("EnvList");
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
                        // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                        // ViewBag.Approver = objGlobaldata.GetGRoleList(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString(), "Approver");

                        EnvRiskModelsList objRiskList = new EnvRiskModelsList();
                        objRiskList.lstHazard = new List<EnvRiskModels>();

                        sSqlstmt = "select mit_id_trans,id_env_risk_trans,mit_id,id_env_risk,cnt_type,cnt_desc,pers_resp,target_date from t_environment_risk_mitigations_trans where id_env_risk_trans='" + objRiskMgmtModels.id_env_risk_trans + "'";
                        DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EnvRiskModels objMit = new EnvRiskModels
                                    {
                                        mit_id_trans = dsMitList.Tables[0].Rows[i]["mit_id_trans"].ToString(),
                                        id_env_risk_trans = dsMitList.Tables[0].Rows[i]["id_env_risk_trans"].ToString(),
                                        mit_id = dsMitList.Tables[0].Rows[i]["mit_id"].ToString(),
                                        id_env_risk = dsMitList.Tables[0].Rows[i]["id_env_risk"].ToString(),
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
                                    return RedirectToAction("EnvList");
                                }
                            }
                            ViewBag.objMitList = objRiskList;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Kindly do Mitigation";
                        return RedirectToAction("EnvList");
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
        public JsonResult FurtherRiskMitigations(FormCollection form, EnvRiskModels objRiskMgmt)
        {
            try
            {
                EnvRiskModelsList objRiskList = new EnvRiskModelsList();
                objRiskList.lstHazard = new List<EnvRiskModels>();

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
                    EnvRiskModels objMitModel = new EnvRiskModels();
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
            EnvRiskModels HazardModels = new EnvRiskModels();
            RiskMgmtModels objSafety = new RiskMgmtModels();
            try
            {
                if (Request.QueryString["id_env_risk"] != null)
                {
                    string sid_env_risk = Request.QueryString["id_env_risk"];
                    string sSqlstmt = "select id_env_risk, env_refno, dept, branch_id, Location, source_id, activity_type, product, aspects, activity, impact,area_affected, notified_to, reported_by, reported_date,further_consequences,impact_id,like_id,legal,legal_voilation,Issue from t_environment_risk where id_env_risk = '" + sid_env_risk + "'";
                    DataSet dsHazardModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsHazardModels.Tables.Count > 0 && dsHazardModels.Tables[0].Rows.Count > 0)
                    {
                        Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                        if (dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsHazardModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                        {
                            dicRatings = HazardModels.GetRiskRatings(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString(),
                                dsHazardModels.Tables[0].Rows[0]["like_id"].ToString());
                        }
                        HazardModels = new EnvRiskModels
                        {
                            id_env_risk = (dsHazardModels.Tables[0].Rows[0]["id_env_risk"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsHazardModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsHazardModels.Tables[0].Rows[0]["Location"].ToString()),
                            env_refno = (dsHazardModels.Tables[0].Rows[0]["env_refno"].ToString()),
                            activity_type = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            activity = (dsHazardModels.Tables[0].Rows[0]["activity"].ToString()),
                            product = (dsHazardModels.Tables[0].Rows[0]["product"].ToString()),
                            aspects = (dsHazardModels.Tables[0].Rows[0]["aspects"].ToString()),
                            impact = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["impact"].ToString()),
                            area_affected = (dsHazardModels.Tables[0].Rows[0]["area_affected"].ToString()),
                            notified_to = (dsHazardModels.Tables[0].Rows[0]["notified_to"].ToString()),
                            reported_by = objGlobaldata.GetEmpHrNameById(dsHazardModels.Tables[0].Rows[0]["reported_by"].ToString()),
                            further_consequences = (dsHazardModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                            legal = objGlobaldata.GetLawNoById(dsHazardModels.Tables[0].Rows[0]["legal"].ToString()),
                            legal_voilation = (dsHazardModels.Tables[0].Rows[0]["legal_voilation"].ToString()),
                            impact_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["like_id"].ToString()),
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
                        sSqlstmt = "select mit_id,id_env_risk,cnt_type,cnt_desc,pers_resp,target_date from t_environment_risk_mitigations where id_env_risk='" + sid_env_risk + "'";
                        ViewBag.Mit = objGlobaldata.Getdetails(sSqlstmt);

                        return View(HazardModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("EnvList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("EnvList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("EnvList");
        }

        [AllowAnonymous]
        public ActionResult EditHazard()
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            EnvRiskModels HazardModels = new EnvRiskModels();
            RiskMgmtModels objSafety = new RiskMgmtModels();
            try
            {
                if (Request.QueryString["id_env_risk"] != null)
                {
                    //objSafety.branch_id = objGlobaldata.GetCurrentUserSession().division;
                    //objSafety.dept = objGlobaldata.GetCurrentUserSession().DeptID;
                    //objSafety.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                    ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();

                    ViewBag.source_id = HazardModels.GetMultiRiskSourceList();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(objSafety.branch_id, objSafety.dept, objSafety.Location);

                    ViewBag.Impact = objGlobaldata.GetDropdownList("Environmental Impact");
                    ViewBag.Activity = objGlobaldata.GetDropdownList("HS_activity_type");
                    ViewBag.Issue = objRisk.GetIsssuesNo();
                    string sid_env_risk = Request.QueryString["id_env_risk"];

                    string sSqlstmt = "select id_env_risk, env_refno, dept, branch_id, Location, source_id, activity_type, product, aspects, activity, impact,area_affected, notified_to, reported_by, reported_date,further_consequences,impact_id,like_id,legal,legal_voilation,Issue from t_environment_risk where id_env_risk = '" + sid_env_risk + "'";
                    DataSet dsHazardModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsHazardModels.Tables.Count > 0 && dsHazardModels.Tables[0].Rows.Count > 0)
                    {
                        //Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                        //if (dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsHazardModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                        //{
                        //    dicRatings = HazardModels.GetRiskRatings(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString(),
                        //        dsHazardModels.Tables[0].Rows[0]["like_id"].ToString());
                        //}
                        HazardModels = new EnvRiskModels
                        {
                            id_env_risk = (dsHazardModels.Tables[0].Rows[0]["id_env_risk"].ToString()),
                            dept = (dsHazardModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = (dsHazardModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Location = (dsHazardModels.Tables[0].Rows[0]["Location"].ToString()),
                            env_refno = (dsHazardModels.Tables[0].Rows[0]["env_refno"].ToString()),
                            activity_type = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            product = (dsHazardModels.Tables[0].Rows[0]["product"].ToString()),
                            aspects = (dsHazardModels.Tables[0].Rows[0]["aspects"].ToString()),
                            activity = (dsHazardModels.Tables[0].Rows[0]["activity"].ToString()),
                            impact = (dsHazardModels.Tables[0].Rows[0]["impact"].ToString()),
                            area_affected = (dsHazardModels.Tables[0].Rows[0]["area_affected"].ToString()),
                            notified_to = (dsHazardModels.Tables[0].Rows[0]["notified_to"].ToString()),
                            reported_by = (dsHazardModels.Tables[0].Rows[0]["reported_by"].ToString()),
                            further_consequences = (dsHazardModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                            legal = objGlobaldata.GetLawNoById(dsHazardModels.Tables[0].Rows[0]["legal"].ToString()),
                            legal_voilation = (dsHazardModels.Tables[0].Rows[0]["legal_voilation"].ToString()),
                            impact_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["like_id"].ToString()),
                            Issue = (dsHazardModels.Tables[0].Rows[0]["Issue"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsHazardModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                        {
                            HazardModels.reported_date = dtValue;
                        }
                        ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsHazardModels.Tables[0].Rows[0]["branch_id"].ToString(), dsHazardModels.Tables[0].Rows[0]["dept"].ToString(), dsHazardModels.Tables[0].Rows[0]["Location"].ToString());
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
                        return RedirectToAction("EnvList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("EnvList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("EnvList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditHazard(EnvRiskModels objHazard, FormCollection form)
        {
            try
            {
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
                if (form["id_env_risk"] != null && form["id_env_risk"] != "")
                {
                    EnvRiskModels Doc = new EnvRiskModels();
                    string sid_env_risk = form["id_env_risk"];

                    if (Doc.FunDeleteHazard(sid_env_risk))
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
        public JsonResult EnvListSearch(FormCollection form, int? page, string branch_name)
        {
            EnvRiskModelsList objHazardModelsList = new EnvRiskModelsList();
            objHazardModelsList.lstHazard = new List<EnvRiskModels>();
            EnvRiskModels objSafety = new EnvRiskModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_env_risk,env_refno,impact_id,like_id,dept,branch_id,Location,source_id,activity,product,aspects,impact,area_affected,notified_to,reported_by,reported_date from t_environment_risk where Active=1";
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "order by id_env_risk desc";
                DataSet dsHazardModels = objGlobaldata.Getdetails(sSqlstmt);

                if (dsHazardModels.Tables.Count > 0 && dsHazardModels.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsHazardModels.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> dicRatings = new Dictionary<string, string>();

                        if (dsHazardModels.Tables[0].Rows[i]["impact_id"].ToString() != "" && dsHazardModels.Tables[0].Rows[i]["like_id"].ToString() != "")
                        {
                            dicRatings = objSafety.GetRiskRatings(dsHazardModels.Tables[0].Rows[i]["impact_id"].ToString(),
                            dsHazardModels.Tables[0].Rows[i]["like_id"].ToString());
                        }

                        try
                        {
                            EnvRiskModels objHazard = new EnvRiskModels
                            {
                                id_env_risk = (dsHazardModels.Tables[0].Rows[i]["id_env_risk"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsHazardModels.Tables[0].Rows[i]["dept"].ToString()),
                                branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardModels.Tables[0].Rows[i]["branch_id"].ToString()),
                                source_id = objSafety.GetRiskSourceNameById(dsHazardModels.Tables[0].Rows[i]["source_id"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsHazardModels.Tables[0].Rows[i]["Location"].ToString()),
                                product = (dsHazardModels.Tables[0].Rows[i]["product"].ToString()),
                                activity = (dsHazardModels.Tables[0].Rows[i]["activity"].ToString()),
                                aspects = (dsHazardModels.Tables[0].Rows[i]["aspects"].ToString()),
                                impact = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["impact"].ToString()),
                                area_affected = (dsHazardModels.Tables[0].Rows[i]["area_affected"].ToString()),
                                notified_to = (dsHazardModels.Tables[0].Rows[i]["notified_to"].ToString()),
                                reported_by = (dsHazardModels.Tables[0].Rows[i]["reported_by"].ToString()),
                                env_refno = (dsHazardModels.Tables[0].Rows[i]["env_refno"].ToString()),
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

                            string sql = "select t.mit_id from t_environment_risk_mitigations t,t_environment_risk tt where t.id_env_risk = tt.id_env_risk and"
                            + " t.id_env_risk = '" + dsHazardModels.Tables[0].Rows[i]["id_env_risk"].ToString() + "'";
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
                            objGlobaldata.AddFunctionalLog("Exception in EnvList: " + ex.ToString());
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
            EnvRiskModelsList objHazardModelsList = new EnvRiskModelsList();
            objHazardModelsList.lstHazard = new List<EnvRiskModels>();
            EnvRiskModels objRisk = new EnvRiskModels();
            RiskMgmtModels obj = new RiskMgmtModels();

            try
            {
                if (Request.QueryString["id_env_risk"] != null)
                {
                    string sid_env_risk = Request.QueryString["id_env_risk"];
                    string sSqlstmt = "select tt.id_env_risk_trans,tt.id_env_risk,tt.env_refno,tt.dept,tt.branch_id,tt.Location,"
                      + " tt.source_id,t.activity_type,t.product,t.aspects,t.activity,t.impact,t.area_affected,tt.notified_to,"
                      + " tt.reported_by,tt.reported_date,t.impact_id,t.like_id,tt.evaluated_by,tt.evaluation_date,tt.reeval_due_date"
                      + " from t_environment_risk_trans tt, t_environment_risk t"
                      + " where t.id_env_risk = tt.id_env_risk"
                      + " and tt.id_env_risk = '" + sid_env_risk + "'";

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
                                EnvRiskModels objRiskMgmtModels = new EnvRiskModels
                                {
                                    id_env_risk_trans = (dsHazardModels.Tables[0].Rows[i]["id_env_risk_trans"].ToString()),
                                    id_env_risk = (dsHazardModels.Tables[0].Rows[i]["id_env_risk"].ToString()),
                                    dept = objGlobaldata.GetMultiDeptNameById(dsHazardModels.Tables[0].Rows[i]["dept"].ToString()),
                                    branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardModels.Tables[0].Rows[i]["branch_id"].ToString()),
                                    source_id = objRisk.GetRiskSourceNameById(dsHazardModels.Tables[0].Rows[i]["source_id"].ToString()),
                                    Location = objGlobaldata.GetDivisionLocationById(dsHazardModels.Tables[0].Rows[i]["Location"].ToString()),
                                    product = (dsHazardModels.Tables[0].Rows[i]["product"].ToString()),
                                    activity = (dsHazardModels.Tables[0].Rows[i]["activity"].ToString()),
                                    aspects = (dsHazardModels.Tables[0].Rows[i]["aspects"].ToString()),
                                    impact = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["impact"].ToString()),
                                    area_affected = (dsHazardModels.Tables[0].Rows[i]["area_affected"].ToString()),
                                    notified_to = (dsHazardModels.Tables[0].Rows[i]["notified_to"].ToString()),
                                    reported_by = (dsHazardModels.Tables[0].Rows[i]["reported_by"].ToString()),
                                    env_refno = (dsHazardModels.Tables[0].Rows[i]["env_refno"].ToString()),
                                    evaluated_by = objGlobaldata.GetEmpHrNameById(dsHazardModels.Tables[0].Rows[i]["evaluated_by"].ToString()),
                                    impact_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["impact_id"].ToString()),
                                    like_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["like_id"].ToString()),
                                    activity_type = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[i]["activity_type"].ToString()),
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
                        return RedirectToAction("EnvList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("EnvList");
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
            EnvRiskModels HazardModels = new EnvRiskModels();
            RiskMgmtModels objSafety = new RiskMgmtModels();
            try
            {
                if (Request.QueryString["id_env_risk_trans"] != null)
                {
                    string id_env_risk_trans = Request.QueryString["id_env_risk_trans"];
                    string sSqlstmt = "select tt.id_env_risk, t.env_refno, t.dept, t.branch_id, t.Location, t.source_id, t.activity_type,"
                      + " t.product, t.aspects, t.activity, t.impact, t.area_affected, t.notified_to, t.reported_by,"
                      + " t.reported_date,t.further_consequences,tt.impact_id,tt.like_id,t.legal,t.legal_voilation,t.Issue"
                      + " from t_environment_risk_trans tt, t_environment_risk t"
                      + " where t.id_env_risk = tt.id_env_risk"
                      + " and id_env_risk_trans = '" + id_env_risk_trans + "'";
                    DataSet dsHazardModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsHazardModels.Tables.Count > 0 && dsHazardModels.Tables[0].Rows.Count > 0)
                    {
                        Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                        if (dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsHazardModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                        {
                            dicRatings = HazardModels.GetRiskRatings(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString(),
                                dsHazardModels.Tables[0].Rows[0]["like_id"].ToString());
                        }
                        HazardModels = new EnvRiskModels
                        {
                            id_env_risk = (dsHazardModels.Tables[0].Rows[0]["id_env_risk"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsHazardModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsHazardModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsHazardModels.Tables[0].Rows[0]["Location"].ToString()),
                            env_refno = (dsHazardModels.Tables[0].Rows[0]["env_refno"].ToString()),
                            activity_type = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["activity_type"].ToString()),
                            activity = (dsHazardModels.Tables[0].Rows[0]["activity"].ToString()),
                            product = (dsHazardModels.Tables[0].Rows[0]["product"].ToString()),
                            aspects = (dsHazardModels.Tables[0].Rows[0]["aspects"].ToString()),
                            impact = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["impact"].ToString()),
                            area_affected = (dsHazardModels.Tables[0].Rows[0]["area_affected"].ToString()),
                            notified_to = (dsHazardModels.Tables[0].Rows[0]["notified_to"].ToString()),
                            reported_by = objGlobaldata.GetEmpHrNameById(dsHazardModels.Tables[0].Rows[0]["reported_by"].ToString()),
                            further_consequences = (dsHazardModels.Tables[0].Rows[0]["further_consequences"].ToString()),
                            legal = objGlobaldata.GetLawNoById(dsHazardModels.Tables[0].Rows[0]["legal"].ToString()),
                            legal_voilation = (dsHazardModels.Tables[0].Rows[0]["legal_voilation"].ToString()),
                            impact_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = objGlobaldata.GetDropdownitemById(dsHazardModels.Tables[0].Rows[0]["like_id"].ToString()),
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
                        sSqlstmt = "select mit_id,id_env_risk,cnt_type,cnt_desc,pers_resp,target_date from t_environment_risk_mitigations_trans where id_env_risk_trans='" + id_env_risk_trans + "'";
                        ViewBag.Mit = objGlobaldata.Getdetails(sSqlstmt);

                        return View(HazardModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("EnvList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("EnvList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HazardHistoryDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("EnvList");
        }
    }
}