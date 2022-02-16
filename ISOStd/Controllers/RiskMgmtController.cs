using ISOStd.Filters;
using ISOStd.Models;
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
    public class RiskMgmtController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public RiskMgmtController()
        {
            ViewBag.Menutype = "Risk";
            ViewBag.SubMenutype = "RiskMgmt";
        }

        //
        // GET: /RiskMgmt/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddRisk(string id_issue)
        {
            return InitilizeAddRisk(id_issue);
        }

        private ActionResult InitilizeAddRisk(string id_issue)
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            try
            {
                if (id_issue != "" && id_issue != null)
                {
                    ViewBag.Issue = objRisk.GetIsssuesNo(id_issue);
                    objRisk.Issue = id_issue;
                }
                else
                {
                    ViewBag.Issue = objRisk.GetIsssuesNo();
                }

                objRisk.branch_id = objGlobaldata.GetCurrentUserSession().division;
                objRisk.dept = objGlobaldata.GetCurrentUserSession().DeptID;
                objRisk.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(objRisk.branch_id, objRisk.dept, objRisk.Location);
                ViewBag.risk_status_id = objGlobaldata.GetDropdownList("Risk-Status");
                ViewBag.reg_id = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.source_id = objGlobaldata.GetDropdownList("Risk-source");
                //ViewBag.cat_id = objRisk.GetMultiRiskCategoryList();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                // ViewBag.Approver = objGlobaldata.GetApprover();
                //ViewBag.tech_id = objRisk.GetMultiRiskTechnologyList();
                ViewBag.impact_id = objGlobaldata.GetDropdownList("Risk-Severity");
                ViewBag.like_id = objGlobaldata.GetDropdownList("Risk-likelihood");
                ViewBag.Risk_Type = objGlobaldata.GetConstantValue("Impact");
                //ViewBag.Issue = objRisk.GetIsssuesNo();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objRisk.branch_id);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objRisk.branch_id);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddRisk: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRisk);
        }

        public ActionResult RiskMatrix()
        {
            try
            {
                RiskMgmtModels objRisk = new RiskMgmtModels();
                //matrix array
                string[,] SevArray = new string[10, 10];
                ViewBag.Matrixlike_id = objGlobaldata.GetDropdownList("Risk-likelihood");
                ViewBag.MatrixSev_id = objGlobaldata.GetDropdownList("Risk-Severity");

                List<int> array1 = new List<int>();
                List<int> array2 = new List<int>();
                int[,] result = new int[10, 10];
                string sqlstmt = "select item_id as Id, item_fulldesc as severity from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Risk-Severity' order by severity desc";
                DataSet dsSeverity = objGlobaldata.Getdetails(sqlstmt);
                if (dsSeverity.Tables.Count > 0 && dsSeverity.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsSeverity.Tables[0].Rows.Count; i++)
                    {
                        array1.Add(Convert.ToInt32(dsSeverity.Tables[0].Rows[i]["severity"].ToString()));
                    }
                }

                string sqlstmts = "select item_id as Id, item_fulldesc as likely from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                  + "and header_desc='Risk-likelihood' order by likely asc";
                DataSet dsLikely = objGlobaldata.Getdetails(sqlstmts);
                if (dsLikely.Tables.Count > 0 && dsLikely.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsLikely.Tables[0].Rows.Count; i++)
                    {
                        array2.Add(Convert.ToInt32(dsLikely.Tables[0].Rows[i]["likely"].ToString()));
                    }
                }
                //multiplication of matrix
                for (int i = 0; i < array1.Count; i++)
                {
                    for (int j = 0; j < array2.Count; j++)
                    {
                        result[i, j] = array1[i] * array2[j];
                    }
                }
                ViewBag.Matrix = result;

                string sqlstmtss1;
                string sqlstmtss;
                int count = 0;
                DataSet dsMatrixcolor;

                sqlstmtss1 = "delete from rmatrix ";
                objGlobaldata.Getdetails(sqlstmtss1);

                foreach (var item in ViewBag.Matrix)
                {
                    count++;
                    sqlstmtss = "insert into rmatrix (id,matvalue) values ( '" + count + "','" + item + "')";
                    dsMatrixcolor = objGlobaldata.Getdetails(sqlstmtss);
                    objRisk.GetMatrixColordetails();
                }

                Dictionary<string, string> dsMatcolor = new Dictionary<string, string>();

                DataSet dsMatclr = objGlobaldata.Getdetails("Select id,matvalue,color from rmatrix");

                if (dsMatclr.Tables.Count > 0 && dsMatclr.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsMatclr.Tables[0].Rows.Count; i++)
                    {
                        if (dsMatclr.Tables[0].Rows[i]["matvalue"].ToString() != "0")
                        {
                            dsMatcolor.Add(dsMatclr.Tables[0].Rows[i]["id"].ToString(), dsMatclr.Tables[0].Rows[i]["color"].ToString());
                        }
                    }
                }
                ViewBag.dsMatcolors = dsMatcolor;

                string sql = "select from_value,to_value,rate_desc from risk_ratings";
                DataSet dsRating = objGlobaldata.Getdetails(sql);
                ViewBag.dsRating = dsRating;

                ViewBag.dsColor = objGlobaldata.GetRiskMatrixRatewithColor();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskMatrix: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        // POST: /RiskMgmt/AddRisk

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddRisk(RiskMgmtModels objRiskMgmt, FormCollection form)
        {
            try
            {
                if (objRiskMgmt != null)
                {
                    objRiskMgmt.risk_status_id = form["risk_status_id"];
                    objRiskMgmt.reg_id = form["reg_id"];
                    objRiskMgmt.branch_id = form["branch_id"];
                    objRiskMgmt.source_id = form["source_id"];
                    objRiskMgmt.risk_owner = form["risk_owner"];
                    objRiskMgmt.risk_manager = form["risk_manager"];
                    objRiskMgmt.impact_id = form["impact_id"];
                    objRiskMgmt.like_id = form["like_id"];
                    objRiskMgmt.Issue = form["Issue"];
                    objRiskMgmt.dept = form["dept"];
                    objRiskMgmt.Location = form["Location"];
                    // objRiskMgmt.notified_to = form["notified_to"];
                    objRiskMgmt.submitted_by = objGlobaldata.GetCurrentUserSession().empid;

                    //notified_to
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                    {
                        if (form["nempno " + i] != "" && form["nempno " + i] != null)
                        {
                            objRiskMgmt.notified_to = form["nempno " + i] + "," + objRiskMgmt.notified_to;
                        }
                    }
                    if (objRiskMgmt.notified_to != null)
                    {
                        objRiskMgmt.notified_to = objRiskMgmt.notified_to.Trim(',');
                    }

                    if (objRiskMgmt.FunAddRisk(objRiskMgmt))
                    {
                        //if (objRiskMgmt.Issue != null && objRiskMgmt.Issue != "")
                        //{
                        TempData["Successdata"] = "Added Risk details successfully with Reference Number '" + objRiskMgmt.risk_refno + "'";
                        //}
                        //else
                        //{
                        //    TempData["Successdata"] = "Risk entered not related to issues";

                        //}
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRisk: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskList");
        }

        // GET: /RiskMgmt/RiskList

        [AllowAnonymous]
        public JsonResult RiskMgmtDocDelete(FormCollection form)
        {
            try
            {
                if (form["risk_id"] != null && form["risk_id"] != "")
                {
                    RiskMgmtModels Doc = new RiskMgmtModels();
                    string srisk_id = form["risk_id"];

                    if (Doc.FunDeleteRiskMgmtDoc(srisk_id))
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
                objGlobaldata.AddFunctionalLog("Exception in RiskMgmtDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult RiskList(string SearchText, string risk_status_id, int? page, string branch_name)
        {
            RiskMgmtModelsList objRiskModelsList = new RiskMgmtModelsList();
            objRiskModelsList.lstRiskMgmtModels = new List<RiskMgmtModels>();

            RiskMgmtModels objRisk = new RiskMgmtModels();

            try
            {
                ViewBag.risk_status_id = objGlobaldata.GetDropdownList("Risk-Status");
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                //string sSqlstmt1 = "select risk_id_trans from risk_register_trans where risk_id='" + srisk_id + "'";
                //       DataSet dsRiskModels1 = objGlobaldata.Getdetails(sSqlstmt1);
                //       int count = dsRiskModels1.Tables[0].Rows.Count;
                //       if (count > 1)
                //       {
                //           sSqlstmt = "select tt.risk_id_trans,t.risk_id,t.risk_desc,t.dept,t.branch_id,t.source_id,t.risk_owner,t.risk_manager,t.submission_date,t.submitted_by,t.consequences,t.Location,tt.evaluation_date,tt.approved_by,tt.approved_date,tt.reeval_due_date,tt.impact_id,tt.like_id,t.Issue from risk_register t"
                //           + " left join  risk_register_trans tt on t.risk_id = tt.risk_id  where t.risk_id = '" + srisk_id + "' order by risk_id_trans desc limit 1";
                //       }
                //       else
                //       {
                //           sSqlstmt = "select risk_id,risk_desc,dept,branch_id,source_id,risk_owner,risk_manager,submission_date,submitted_by,consequences,Location,evaluation_date,approved_by,approved_date,reeval_due_date,impact_id,like_id,Issue from risk_register"
                //+ " where risk_id='" + srisk_id + "' ";

                //       }

                //string sSqlstmt = "select risk_id, risk_status_id,risk_refno, risk_desc, dept, reg_id, branch_id, source_id, cat_id, tech_id, risk_owner, risk_manager,"
                //    + "assessment, notes, submission_date, close_date,  close_by, submitted_by, impact_id, like_id,opp_desc,Location,reeval_due_date,Risk_Type,apprv_status from risk_register where Active=1";

                string sSqlstmt = "select T1.risk_id, T1.risk_status_id,T1.risk_refno, T1.risk_desc, T1.dept, T1.reg_id, T1.branch_id,T1.source_id, T1.cat_id, T1.tech_id, T1.risk_owner, T1.risk_manager,T1.assessment, T1.notes, T1.submission_date,"
              + "T1.close_date,  T1.close_by, T1.submitted_by, T1.impact_id, T1.like_id,T1.opp_desc,T1.Location,T1.Risk_Type,"
              + "(select impact_id from risk_register_trans T2 where  T1.risk_id = T2.risk_id order by risk_id_trans desc limit 1) as curr_impact_id,T1.apprv_status  as init_apprv_status,"
              + "(select like_id from risk_register_trans T2 where T1.risk_id = T2.risk_id order by risk_id_trans desc limit 1) as curr_like_id,(select apprv_status from risk_register_trans T2 where T1.risk_id = T2.risk_id order by risk_id_trans desc limit 1) as apprv_status,"
              + "T1.reeval_due_date as init_reeval_due_date,(select reeval_due_date from risk_register_trans T2 where T1.risk_id = T2.risk_id order by risk_id_trans desc limit 1) as reeval_due_date,T1.approved_by as init_approved_by,(select approved_by from risk_register_trans T2 where T1.risk_id = T2.risk_id order by risk_id_trans desc limit 1) as approved_by "
              + " from risk_register T1 where T1.Active = 1";

                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = " (risk_desc ='" + SearchText + "' or risk_desc like '" + SearchText + "%')";
                }

                if (sSearchtext != "")
                {
                    sSqlstmt = sSqlstmt + " and";
                }

                if (risk_status_id != null && risk_status_id != "Select")
                {
                    ViewBag.risk_status_idVal = risk_status_id;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (risk_status_id ='" + risk_status_id + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and (risk_status_id ='" + risk_status_id + "')";
                    }
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch_id)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch_id)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by risk_id desc";

                DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsRiskModels.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                        Dictionary<string, string> dicRatings_curr = new Dictionary<string, string>();

                        if (dsRiskModels.Tables[0].Rows[i]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[i]["like_id"].ToString() != "")
                        {
                            dicRatings = objRisk.GetRiskRatings(dsRiskModels.Tables[0].Rows[i]["impact_id"].ToString(),
                            dsRiskModels.Tables[0].Rows[i]["like_id"].ToString());
                        }
                        if (dsRiskModels.Tables[0].Rows[i]["curr_impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[i]["curr_like_id"].ToString() != "")
                        {
                            dicRatings_curr = objRisk.GetRiskRatings(dsRiskModels.Tables[0].Rows[i]["curr_impact_id"].ToString(),
                            dsRiskModels.Tables[0].Rows[i]["curr_like_id"].ToString());
                        }
                        try
                        {
                            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels
                            {
                                risk_id = (dsRiskModels.Tables[0].Rows[i]["risk_id"].ToString()),
                                risk_status_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[i]["risk_status_id"].ToString()),
                                risk_desc = (dsRiskModels.Tables[0].Rows[i]["risk_desc"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[i]["dept"].ToString()),
                                reg_id = objGlobaldata.GetISONameById(dsRiskModels.Tables[0].Rows[i]["reg_id"].ToString()),
                                branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[i]["branch_id"].ToString()),
                                source_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[i]["source_id"].ToString()),
                                //cat_id = objRisk.GetRiskCategoryNameById(dsRiskModels.Tables[0].Rows[i]["cat_id"].ToString()),
                                //tech_id = objRisk.GetRiskTechnologyNameById(dsRiskModels.Tables[0].Rows[i]["tech_id"].ToString()),
                                risk_refno = (dsRiskModels.Tables[0].Rows[i]["risk_refno"].ToString()),
                                risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[i]["risk_owner"].ToString()),
                                risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[i]["risk_manager"].ToString()),
                                assessment = (dsRiskModels.Tables[0].Rows[i]["assessment"].ToString()),
                                notes = (dsRiskModels.Tables[0].Rows[i]["notes"].ToString()),
                                submission_date = Convert.ToDateTime(dsRiskModels.Tables[0].Rows[i]["submission_date"].ToString()),
                                submitted_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[i]["submitted_by"].ToString()),
                                opp_desc = (dsRiskModels.Tables[0].Rows[i]["opp_desc"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[i]["Location"].ToString()),
                                Risk_Type = dsRiskModels.Tables[0].Rows[i]["Risk_Type"].ToString(),
                                apprv_status = dsRiskModels.Tables[0].Rows[i]["apprv_status"].ToString(),
                                init_apprv_status = dsRiskModels.Tables[0].Rows[i]["init_apprv_status"].ToString(),
                                init_approved_by = dsRiskModels.Tables[0].Rows[i]["init_approved_by"].ToString()
                            };
                            DateTime dtValue;

                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[i]["reeval_due_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.reeval_due_date = dtValue;
                            }
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[i]["init_reeval_due_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.init_reeval_due_date = dtValue;
                            }
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[i]["reeval_due_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.reeval_due_date = dtValue;
                            }
                            if (dicRatings != null && dicRatings.Count > 0)
                            {
                                objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
                                objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
                            }
                            if (dicRatings_curr != null && dicRatings_curr.Count > 0)
                            {
                                objRiskMgmtModels.RiskRating_curr = dicRatings_curr.FirstOrDefault().Key;
                                objRiskMgmtModels.color_code_curr = dicRatings_curr.FirstOrDefault().Value;
                            }

                            string sql = "select t.mit_id from risk_mitigations t,risk_register tt where t.risk_id = tt.risk_id and"
                            + " t.risk_id = '" + dsRiskModels.Tables[0].Rows[i]["risk_id"].ToString() + "'";
                            DataSet dsRisk = objGlobaldata.Getdetails(sSqlstmt);

                            if (dsRisk.Tables.Count > 0 && dsRisk.Tables[0].Rows.Count > 0)
                            {
                                objRiskMgmtModels.mitmeasure = "Yes";
                            }
                            else
                            {
                                objRiskMgmtModels.mitmeasure = "No";
                            }
                            objRiskModelsList.lstRiskMgmtModels.Add(objRiskMgmtModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RiskList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRiskModelsList.lstRiskMgmtModels.ToList());
        }

        //[AllowAnonymous]
        //public JsonResult RiskListSearch(string SearchText, string risk_status_id, int? page, string branch_name)
        //{
        //    RiskMgmtModelsList objRiskModelsList = new RiskMgmtModelsList();
        //    objRiskModelsList.lstRiskMgmtModels = new List<RiskMgmtModels>();

        //    RiskMgmtModels objRisk = new RiskMgmtModels();

        //    try
        //    {
        //        ViewBag.risk_status_id = objGlobaldata.GetDropdownList("Risk-Status");
        //        UserCredentials objUser = new UserCredentials();
        //        objUser = objGlobaldata.GetCurrentUserSession();
        //        ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);
        //        string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
        //        string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
        //        ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

        //        //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
        //        string sSqlstmt = "select risk_id, risk_status_id, risk_desc, dept, reg_id, branch_id, source_id, cat_id, tech_id, risk_owner, risk_manager,"
        //            + "assessment, notes, submission_date, close_date,  close_by, submitted_by, impact_id, like_id,opp_desc,Location,Risk_Type from risk_register where Active=1";
        //        string sSearchtext = "";

        //        if (SearchText != null && SearchText != "")
        //        {
        //            ViewBag.SearchText = SearchText;
        //            sSearchtext = " (risk_desc ='" + SearchText + "' or risk_desc like '" + SearchText + "%')";
        //        }

        //        if (sSearchtext != "")
        //        {
        //            sSqlstmt = sSqlstmt + " and";
        //        }

        //        if (risk_status_id != null && risk_status_id != "Select")
        //        {
        //            ViewBag.risk_status_idVal = risk_status_id;
        //            if (sSearchtext != "")
        //            {
        //                sSearchtext = sSearchtext + " and (risk_status_id ='" + risk_status_id + "')";
        //            }
        //            else
        //            {
        //                sSearchtext = sSearchtext + " and (risk_status_id ='" + risk_status_id + "')";
        //            }
        //        }

        //        if (branch_name != null && branch_name != "")
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch_id)";
        //            ViewBag.Branch_name = branch_name;
        //        }
        //        else
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch_id)";
        //        }

        //        sSqlstmt = sSqlstmt + sSearchtext + " order by submission_date desc";

        //        DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

        //        if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsRiskModels.Tables[0].Rows.Count; i++)
        //            {
        //                Dictionary<string, string> dicRatings = new Dictionary<string, string>();

        //                if (dsRiskModels.Tables[0].Rows[i]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[i]["like_id"].ToString() != "")
        //                {
        //                    dicRatings = objRisk.GetRiskRatings(dsRiskModels.Tables[0].Rows[i]["impact_id"].ToString(),
        //                     dsRiskModels.Tables[0].Rows[i]["like_id"].ToString());
        //                }

        //                try
        //                {
        //                    RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels
        //                    {
        //                        risk_id = (dsRiskModels.Tables[0].Rows[i]["risk_id"].ToString()),
        //                        risk_status_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[i]["risk_status_id"].ToString()),
        //                        risk_desc = (dsRiskModels.Tables[0].Rows[i]["risk_desc"].ToString()),
        //                        dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[i]["dept"].ToString()),
        //                        reg_id = objGlobaldata.GetISONameById(dsRiskModels.Tables[0].Rows[i]["reg_id"].ToString()),
        //                        branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[i]["branch_id"].ToString()),
        //                        source_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[i]["source_id"].ToString()),
        //                        //cat_id = objRisk.GetRiskCategoryNameById(dsRiskModels.Tables[0].Rows[i]["cat_id"].ToString()),
        //                        //tech_id = objRisk.GetRiskTechnologyNameById(dsRiskModels.Tables[0].Rows[i]["tech_id"].ToString()),

        //                        risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[i]["risk_owner"].ToString()),
        //                        risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[i]["risk_manager"].ToString()),
        //                        assessment = (dsRiskModels.Tables[0].Rows[i]["assessment"].ToString()),
        //                        notes = (dsRiskModels.Tables[0].Rows[i]["notes"].ToString()),
        //                        submission_date = Convert.ToDateTime(dsRiskModels.Tables[0].Rows[i]["submission_date"].ToString()),
        //                        submitted_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[i]["submitted_by"].ToString()),
        //                        opp_desc = (dsRiskModels.Tables[0].Rows[i]["opp_desc"].ToString()),
        //                        Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[i]["Location"].ToString()),
        //                        Risk_Type= dsRiskModels.Tables[0].Rows[i]["Risk_Type"].ToString()
        //                    };

        //                    if (dicRatings != null && dicRatings.Count > 0)
        //                    {
        //                        objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
        //                        objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
        //                    }

        //                    objRiskModelsList.lstRiskMgmtModels.Add(objRiskMgmtModels);
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobaldata.AddFunctionalLog("Exception in RiskListSearch: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in RiskListSearch: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json("Success");
        //}

        //[AllowAnonymous]
        //public ActionResult RiskHistoryList(string SearchText, string risk_status_id, int? page, string risk_id)
        //{
        //    RiskMgmtModelsList objRiskModelsList = new RiskMgmtModelsList();
        //    objRiskModelsList.lstRiskMgmtModels = new List<RiskMgmtModels>();

        //    RiskMgmtModels objRisk = new RiskMgmtModels();

        //    try
        //    {
        //        ViewBag.risk_status_id = objRisk.GetMultiRiskStatusList("Risk-Status");
        //        ViewBag.risk_id = risk_id;
        //        //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
        //        string sSqlstmt = "select risk_id_trans, risk_id, risk_status_id, risk_desc, dept, reg_id, branch_id, source_id, cat_id, tech_id, risk_owner, risk_manager,"
        //            + "assessment, notes, submission_date, close_date,  close_by, submitted_by, impact_id, like_id from risk_register_trans where risk_id='" + risk_id + "' ";
        //        string sSearchtext = "";

        //        if (SearchText != null && SearchText != "")
        //        {
        //            ViewBag.SearchText = SearchText;
        //            sSearchtext = " and (risk_desc ='" + SearchText + "' or risk_desc like '" + SearchText + "%')";
        //        }

        //        //if (sSearchtext != "")
        //        //{
        //        //    sSqlstmt = sSqlstmt + " where";
        //        //}

        //        if (risk_status_id != null && risk_status_id != "Select")
        //        {
        //            ViewBag.risk_status_idVal = risk_status_id;
        //            if (sSearchtext != "")
        //            {
        //                sSearchtext = sSearchtext + " and (risk_status_id ='" + risk_status_id + "')";
        //            }

        //        }

        //        sSqlstmt = sSqlstmt + sSearchtext + " order by risk_id desc";

        //        DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

        //        if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsRiskModels.Tables[0].Rows.Count; i++)
        //            {
        //                Dictionary<string, string> dicRatings = new Dictionary<string, string>();
        //                if (dsRiskModels.Tables[0].Rows[i]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[i]["like_id"].ToString() != "")
        //                {
        //                    dicRatings = objRisk.GetRiskRatings(dsRiskModels.Tables[0].Rows[i]["impact_id"].ToString(),
        //                        dsRiskModels.Tables[0].Rows[i]["like_id"].ToString());
        //                }
        //                try
        //                {
        //                    RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels
        //                    {
        //                        risk_id_trans = (dsRiskModels.Tables[0].Rows[i]["risk_id_trans"].ToString()),
        //                        risk_id = (dsRiskModels.Tables[0].Rows[i]["risk_id"].ToString()),
        //                        risk_status_id = objRisk.GetRiskStatusNameById(dsRiskModels.Tables[0].Rows[i]["risk_status_id"].ToString()),
        //                        risk_desc = (dsRiskModels.Tables[0].Rows[i]["risk_desc"].ToString()),
        //                        dept = objGlobaldata.GetDeptNameById(dsRiskModels.Tables[0].Rows[i]["dept"].ToString()),
        //                        reg_id = objGlobaldata.GetISONameById(dsRiskModels.Tables[0].Rows[i]["reg_id"].ToString()),
        //                        branch_id = objGlobaldata.GetCompanyBranchNameById(dsRiskModels.Tables[0].Rows[i]["branch_id"].ToString()),
        //                        source_id = objRisk.GetRiskSourceNameById(dsRiskModels.Tables[0].Rows[i]["source_id"].ToString()),
        //                        //cat_id = objRisk.GetRiskCategoryNameById(dsRiskModels.Tables[0].Rows[i]["cat_id"].ToString()),
        //                        //tech_id = objRisk.GetRiskTechnologyNameById(dsRiskModels.Tables[0].Rows[i]["tech_id"].ToString()),

        //                        risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[i]["risk_owner"].ToString()),
        //                        risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[i]["risk_manager"].ToString()),
        //                        assessment = (dsRiskModels.Tables[0].Rows[i]["assessment"].ToString()),
        //                        notes = (dsRiskModels.Tables[0].Rows[i]["notes"].ToString()),
        //                        submission_date = Convert.ToDateTime(dsRiskModels.Tables[0].Rows[i]["submission_date"].ToString()),
        //                        submitted_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[i]["submitted_by"].ToString())
        //                    };

        //                    if (dicRatings != null && dicRatings.Count > 0)
        //                    {
        //                        objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
        //                        objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
        //                    }

        //                    objRiskModelsList.lstRiskMgmtModels.Add(objRiskMgmtModels);
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobaldata.AddFunctionalLog("Exception in RiskHistoryList: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in RiskHistoryList: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return View(objRiskModelsList.lstRiskMgmtModels.ToList());
        //}

        // GET: /RiskMgmt/RiskDetails

        //[AllowAnonymous]
        //public ActionResult RiskDetails()
        //{
        //    RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();

        //    //RiskMgmtModels objRisk = new RiskMgmtModels();

        //    try
        //    {
        //        if (Request.QueryString["risk_id"] != null && Request.QueryString["risk_id"] != "")
        //        {
        //            string risk_id = Request.QueryString["risk_id"];
        //            //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
        //            string sSqlstmt="";
        //            string sSqlstmt1 = "select risk_id_trans from risk_register_trans where risk_id='" + risk_id + "'";
        //            DataSet dsRiskModels1 = objGlobaldata.Getdetails(sSqlstmt1);
        //            int count = dsRiskModels1.Tables[0].Rows.Count;

        //            if (count > 1)
        //            {
        //                sSqlstmt = "select b.risk_id_trans,b.assessment as Assesment_trans,a.risk_id, a.risk_status_id,a.Risk_Type, a.risk_desc, a.dept, a.reg_id, a.branch_id, a.source_id, a.cat_id, a.tech_id, a.risk_owner, a.risk_manager,"
        //                 + " a.assessment, a.notes, a.submission_date,a.Issue, a.close_date, a.close_by, a.submitted_by, a.impact_id, a.like_id, a.consequences,opp_desc,a.Location"
        //                 + "from risk_register a left join risk_register_trans b on a.risk_id=b.risk_id where a.risk_id='" + risk_id + "' order by b.risk_id_trans desc limit 1";

        //            }
        //            else
        //            {
        //                sSqlstmt = "select null as Assesment_trans,risk_id, risk_status_id, risk_desc, dept, reg_id, branch_id, source_id, cat_id, tech_id, risk_owner, risk_manager,"
        //                    + "assessment, notes, submission_date, close_date,Issue, close_by, submitted_by, impact_id, like_id, consequences,Risk_Type,opp_desc,Location from risk_register where risk_id='" + risk_id + "'";
        //            }

        //            DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
        //            {
        //                Dictionary<string, string> dicRatings = new Dictionary<string, string>();
        //                if (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[0]["like_id"].ToString() != "")
        //                {
        //                    dicRatings = objRiskMgmtModels.GetRiskRatings(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString(),
        //                        dsRiskModels.Tables[0].Rows[0]["like_id"].ToString());
        //                }
        //                objRiskMgmtModels = new RiskMgmtModels
        //                {
        //                    risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
        //                    risk_status_id = objRiskMgmtModels.GetRiskStatusNameById(dsRiskModels.Tables[0].Rows[0]["risk_status_id"].ToString()),
        //                    risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
        //                    dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    reg_id = objGlobaldata.GetISONameById(dsRiskModels.Tables[0].Rows[0]["reg_id"].ToString()),
        //                    branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
        //                    source_id = objRiskMgmtModels.GetRiskSourceNameById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
        //                    //cat_id = objRiskMgmtModels.GetRiskCategoryNameById(dsRiskModels.Tables[0].Rows[0]["cat_id"].ToString()),
        //                    //tech_id = objRiskMgmtModels.GetRiskTechnologyNameById(dsRiskModels.Tables[0].Rows[0]["tech_id"].ToString()),

        //                    risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_owner"].ToString()),
        //                    risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
        //                    assessment = (dsRiskModels.Tables[0].Rows[0]["assessment"].ToString()),
        //                    notes = (dsRiskModels.Tables[0].Rows[0]["notes"].ToString()),
        //                    submission_date = Convert.ToDateTime(dsRiskModels.Tables[0].Rows[0]["submission_date"].ToString()),
        //                    submitted_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["submitted_by"].ToString()),
        //                    impact_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
        //                    like_id = objRiskMgmtModels.GetLikelihoodNameById(dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
        //                    Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),

        //                    Assesment_trans = (dsRiskModels.Tables[0].Rows[0]["Assesment_trans"].ToString()),
        //                    consequences = dsRiskModels.Tables[0].Rows[0]["consequences"].ToString(),
        //                    Risk_Type = objRiskMgmtModels.GetRiskTypeNameById(dsRiskModels.Tables[0].Rows[0]["Risk_Type"].ToString()),
        //                    Issue = objRiskMgmtModels.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
        //                    opp_desc = dsRiskModels.Tables[0].Rows[0]["opp_desc"].ToString()
        //                };

        //                if (dicRatings != null && dicRatings.Count > 0)
        //                {
        //                    objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
        //                    objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
        //                }
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = "No Data exists";
        //                return RedirectToAction("RiskList");
        //            }

        //            //Items to populate Mitigation data
        //            RiskMitigationModels objMitigation = new RiskMitigationModels();
        //            DataSet dsMitigation = objGlobaldata.Getdetails("SELECT mit_id, risk_id, submission_date, last_update, eval_id, effort_id, mitigation_owner,"
        //                + " current_solution, submitted_by, MitigationStatus,DocUpload,TargetDate FROM mitigations where risk_id='" + risk_id + "'");

        //            if (dsMitigation.Tables.Count > 0 && dsMitigation.Tables[0].Rows.Count > 0)
        //            {
        //                objMitigation.mit_id = dsMitigation.Tables[0].Rows[0]["mit_id"].ToString();
        //                objMitigation.risk_id = objRiskMgmtModels.GetRiskNameById(dsMitigation.Tables[0].Rows[0]["risk_id"].ToString());
        //                objMitigation.eval_id = objMitigation.GetEvaluationStatusNameById(dsMitigation.Tables[0].Rows[0]["eval_id"].ToString());
        //                objMitigation.effort_id = objMitigation.GetEffortNameById(dsMitigation.Tables[0].Rows[0]["effort_id"].ToString());
        //                objMitigation.mitigation_owner = objGlobaldata.GetEmpHrNameById(dsMitigation.Tables[0].Rows[0]["mitigation_owner"].ToString());
        //                objMitigation.current_solution = dsMitigation.Tables[0].Rows[0]["current_solution"].ToString();
        //                objMitigation.submission_date = Convert.ToDateTime(dsMitigation.Tables[0].Rows[0]["submission_date"].ToString());
        //                objMitigation.submitted_by = objGlobaldata.GetEmpHrNameById(dsMitigation.Tables[0].Rows[0]["submitted_by"].ToString());
        //                objMitigation.MitigationStatus = dsMitigation.Tables[0].Rows[0]["MitigationStatus"].ToString();
        //                objMitigation.DocUpload = dsMitigation.Tables[0].Rows[0]["DocUpload"].ToString();
        //                DateTime dtDocDate = new DateTime();
        //                if (dsMitigation.Tables[0].Rows[0]["TargetDate"].ToString() != ""
        //                         && DateTime.TryParse(dsMitigation.Tables[0].Rows[0]["TargetDate"].ToString(), out dtDocDate))
        //                {
        //                    objMitigation.TargetDate = dtDocDate;
        //                }
        //            }

        //            ViewBag.objMitigation = objMitigation;

        //            //Items to populate Risk Review data
        //            RiskReviewModelsList objRiskReviewModelslst = new RiskReviewModelsList();
        //            objRiskReviewModelslst.lstRiskReviewModels = new List<RiskReviewModels>();
        //            RiskReviewModels objRiskReviewModelsMember = new RiskReviewModels();

        //            DataSet dsRiskReview = objGlobaldata.Getdetails("SELECT review_id,risk_id,submission_date,reviewer,what_monit,where_monit,when_monit,who_monit,how_monit"
        //                    + " FROM mgmt_reviews where risk_id='" + risk_id + "'");

        //            if (dsRiskReview.Tables.Count > 0 && dsRiskReview.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < dsRiskReview.Tables[0].Rows.Count; i++)
        //                {
        //                    RiskReviewModels objRiskReviewModels = new RiskReviewModels
        //                    {
        //                        review_id = (dsRiskReview.Tables[0].Rows[i]["review_id"].ToString()),
        //                        risk_id = objRiskMgmtModels.GetRiskNameById(dsRiskReview.Tables[0].Rows[i]["risk_id"].ToString()),
        //                        reviewer = objGlobaldata.GetEmpHrNameById(dsRiskReview.Tables[0].Rows[i]["reviewer"].ToString()),
        //                        submission_date = Convert.ToDateTime(dsRiskReview.Tables[0].Rows[i]["submission_date"].ToString()),
        //                        what_monit = (dsRiskReview.Tables[0].Rows[i]["what_monit"].ToString()),
        //                        where_monit = (dsRiskReview.Tables[0].Rows[i]["where_monit"].ToString()),
        //                        when_monit = (dsRiskReview.Tables[0].Rows[i]["when_monit"].ToString()),
        //                        who_monit = (dsRiskReview.Tables[0].Rows[i]["who_monit"].ToString()),
        //                        how_monit = (dsRiskReview.Tables[0].Rows[i]["how_monit"].ToString()),
        //                    };
        //                    objRiskReviewModelslst.lstRiskReviewModels.Add(objRiskReviewModels);
        //                }
        //                ViewBag.objRiskReview = objRiskReviewModelslst;
        //            }
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = "Risk Id cannot be Null or empty";
        //            return RedirectToAction("RiskList");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in RiskDetails: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View(objRiskMgmtModels);
        //}

        [AllowAnonymous]
        public ActionResult RiskMgmtInfo(int id)
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();

            //RiskMgmtModels objRisk = new RiskMgmtModels();

            try
            {
                //if (Request.QueryString["risk_id"] != null && Request.QueryString["risk_id"] != "")
                //{
                string sSqlstmt = "";
                string sSqlstmt1 = "select risk_id_trans from risk_register_trans where risk_id='" + id + "'";
                DataSet dsRiskModels1 = objGlobaldata.Getdetails(sSqlstmt1);
                int count = dsRiskModels1.Tables[0].Rows.Count;

                if (count > 1)
                {
                    sSqlstmt = "select b.risk_id_trans,b.assessment as Assesment_trans,a.risk_id, a.risk_status_id,a.Risk_Type, a.risk_desc, a.dept, a.reg_id, a.branch_id, a.source_id, a.cat_id, a.tech_id, a.risk_owner, a.risk_manager,"
                     + " a.assessment, a.notes, a.submission_date,a.Issue, a.close_date, a.close_by, a.submitted_by, a.impact_id, a.like_id, a.consequences,opp_desc,a.Location "
                     + "from risk_register a left join risk_register_trans b on a.risk_id=b.risk_id where a.risk_id='" + id + "' order by b.risk_id_trans desc limit 1";
                }
                else
                {
                    sSqlstmt = "select null as Assesment_trans,risk_id, risk_status_id, risk_desc, dept, reg_id, branch_id, source_id, cat_id, tech_id, risk_owner, risk_manager,"
                        + "assessment, notes, submission_date, close_date,Issue, close_by, submitted_by, impact_id, like_id, consequences,Risk_Type,opp_desc,Location from risk_register where risk_id='" + id + "'";
                }

                DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                {
                    Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                    if (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                    {
                        dicRatings = objRiskMgmtModels.GetRiskRatings(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString(),
                            dsRiskModels.Tables[0].Rows[0]["like_id"].ToString());
                    }
                    objRiskMgmtModels = new RiskMgmtModels
                    {
                        risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                        risk_status_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["risk_status_id"].ToString()),
                        risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
                        dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                        reg_id = objGlobaldata.GetISONameById(dsRiskModels.Tables[0].Rows[0]["reg_id"].ToString()),
                        branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                        source_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                        //cat_id = objRiskMgmtModels.GetRiskCategoryNameById(dsRiskModels.Tables[0].Rows[0]["cat_id"].ToString()),
                        //tech_id = objRiskMgmtModels.GetRiskTechnologyNameById(dsRiskModels.Tables[0].Rows[0]["tech_id"].ToString()),

                        risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_owner"].ToString()),
                        risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
                        assessment = (dsRiskModels.Tables[0].Rows[0]["assessment"].ToString()),
                        notes = (dsRiskModels.Tables[0].Rows[0]["notes"].ToString()),
                        submission_date = Convert.ToDateTime(dsRiskModels.Tables[0].Rows[0]["submission_date"].ToString()),
                        submitted_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["submitted_by"].ToString()),
                        impact_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                        like_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                        Assesment_trans = (dsRiskModels.Tables[0].Rows[0]["Assesment_trans"].ToString()),
                        consequences = dsRiskModels.Tables[0].Rows[0]["consequences"].ToString(),
                        Risk_Type = (dsRiskModels.Tables[0].Rows[0]["Risk_Type"].ToString()),
                        Issue = objRiskMgmtModels.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                        opp_desc = dsRiskModels.Tables[0].Rows[0]["opp_desc"].ToString(),
                        Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                    };

                    if (dicRatings != null && dicRatings.Count > 0)
                    {
                        objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
                        objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
                    }
                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("RiskList");
                }

                //Items to populate Mitigation data
                RiskMitigationModels objMitigation = new RiskMitigationModels();
                DataSet dsMitigation = objGlobaldata.Getdetails("SELECT mit_id, risk_id, submission_date, last_update, eval_id, effort_id, mitigation_owner,"
                    + " current_solution, submitted_by, MitigationStatus,DocUpload,TargetDate FROM mitigations where risk_id='" + id + "'");

                if (dsMitigation.Tables.Count > 0 && dsMitigation.Tables[0].Rows.Count > 0)
                {
                    objMitigation.mit_id = dsMitigation.Tables[0].Rows[0]["mit_id"].ToString();
                    objMitigation.risk_id = objRiskMgmtModels.GetRiskNameById(dsMitigation.Tables[0].Rows[0]["risk_id"].ToString());
                    objMitigation.eval_id = objGlobaldata.GetDropdownitemById(dsMitigation.Tables[0].Rows[0]["eval_id"].ToString());
                    objMitigation.effort_id = objGlobaldata.GetDropdownitemById(dsMitigation.Tables[0].Rows[0]["effort_id"].ToString());
                    objMitigation.mitigation_owner = objGlobaldata.GetEmpHrNameById(dsMitigation.Tables[0].Rows[0]["mitigation_owner"].ToString());
                    objMitigation.current_solution = dsMitigation.Tables[0].Rows[0]["current_solution"].ToString();
                    objMitigation.submission_date = Convert.ToDateTime(dsMitigation.Tables[0].Rows[0]["submission_date"].ToString());
                    objMitigation.submitted_by = objGlobaldata.GetEmpHrNameById(dsMitigation.Tables[0].Rows[0]["submitted_by"].ToString());
                    objMitigation.MitigationStatus = dsMitigation.Tables[0].Rows[0]["MitigationStatus"].ToString();
                    objMitigation.DocUpload = dsMitigation.Tables[0].Rows[0]["DocUpload"].ToString();
                    DateTime dtDocDate = new DateTime();
                    if (dsMitigation.Tables[0].Rows[0]["TargetDate"].ToString() != ""
                             && DateTime.TryParse(dsMitigation.Tables[0].Rows[0]["TargetDate"].ToString(), out dtDocDate))
                    {
                        objMitigation.TargetDate = dtDocDate;
                    }
                    ViewBag.objMitigation = objMitigation;
                }

                //Items to populate Risk Review data
                RiskReviewModelsList objRiskReviewModelslst = new RiskReviewModelsList();
                objRiskReviewModelslst.lstRiskReviewModels = new List<RiskReviewModels>();
                RiskReviewModels objRiskReviewModelsMember = new RiskReviewModels();

                DataSet dsRiskReview = objGlobaldata.Getdetails("SELECT review_id, risk_id, submission_date,reviewer,what_monit,where_monit,when_monit,who_monit,how_monit"
                        + " FROM mgmt_reviews where risk_id='" + id + "'");

                if (dsRiskReview.Tables.Count > 0 && dsRiskReview.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsRiskReview.Tables[0].Rows.Count; i++)
                    {
                        RiskReviewModels objRiskReviewModels = new RiskReviewModels
                        {
                            review_id = (dsRiskReview.Tables[0].Rows[i]["review_id"].ToString()),
                            risk_id = objRiskMgmtModels.GetRiskNameById(dsRiskReview.Tables[0].Rows[i]["risk_id"].ToString()),
                            reviewer = objGlobaldata.GetEmpHrNameById(dsRiskReview.Tables[0].Rows[i]["reviewer"].ToString()),
                            submission_date = Convert.ToDateTime(dsRiskReview.Tables[0].Rows[i]["submission_date"].ToString()),
                            what_monit = (dsRiskReview.Tables[0].Rows[i]["what_monit"].ToString()),
                            where_monit = (dsRiskReview.Tables[0].Rows[i]["where_monit"].ToString()),
                            when_monit = (dsRiskReview.Tables[0].Rows[i]["when_monit"].ToString()),
                            who_monit = (dsRiskReview.Tables[0].Rows[i]["who_monit"].ToString()),
                            how_monit = (dsRiskReview.Tables[0].Rows[i]["how_monit"].ToString()),
                        };

                        objRiskReviewModelslst.lstRiskReviewModels.Add(objRiskReviewModels);
                    }

                    ViewBag.objRiskReview = objRiskReviewModelslst;
                }
                //}
                //else
                //{
                //    TempData["alertdata"] = "Risk Id cannot be Null or empty";
                //    return RedirectToAction("RiskList");
                //}
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objRiskMgmtModels);
        }

        // GET: /RiskMgmt/RiskDetails

        //[AllowAnonymous]
        //public ActionResult RiskHistoryDetails()
        //{
        //    RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();

        //    //RiskMgmtModels objRisk = new RiskMgmtModels();

        //    try
        //    {
        //        if (Request.QueryString["risk_id_trans"] != null && Request.QueryString["risk_id_trans"] != "")
        //        {
        //            string risk_id_trans = Request.QueryString["risk_id_trans"];
        //            //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
        //            string sSqlstmt = "select risk_id_trans, risk_id, risk_status_id, risk_desc, dept, reg_id, branch_id, source_id, cat_id, tech_id, risk_owner, risk_manager,"
        //                + "assessment, notes, submission_date, close_date, close_by, submitted_by, impact_id, like_id, consequences from risk_register_trans where risk_id_trans='"
        //                + risk_id_trans + "'";

        //            DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
        //            {
        //                Dictionary<string, string> dicRatings = new Dictionary<string, string>();
        //                if (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[0]["like_id"].ToString() != "")
        //                {
        //                    dicRatings = objRiskMgmtModels.GetRiskRatings(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString(),
        //                        dsRiskModels.Tables[0].Rows[0]["like_id"].ToString());
        //                }
        //                objRiskMgmtModels = new RiskMgmtModels
        //                {
        //                    risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
        //                    risk_id_trans = dsRiskModels.Tables[0].Rows[0]["risk_id_trans"].ToString(),
        //                    risk_status_id = objRiskMgmtModels.GetRiskStatusNameById(dsRiskModels.Tables[0].Rows[0]["risk_status_id"].ToString()),
        //                    risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
        //                    dept = objGlobaldata.GetDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    reg_id = objGlobaldata.GetISONameById(dsRiskModels.Tables[0].Rows[0]["reg_id"].ToString()),
        //                    branch_id = objGlobaldata.GetCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
        //                    source_id = objRiskMgmtModels.GetRiskSourceNameById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
        //                    //cat_id = objRiskMgmtModels.GetRiskCategoryNameById(dsRiskModels.Tables[0].Rows[0]["cat_id"].ToString()),
        //                    //tech_id = objRiskMgmtModels.GetRiskTechnologyNameById(dsRiskModels.Tables[0].Rows[0]["tech_id"].ToString()),

        //                    risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_owner"].ToString()),
        //                    risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
        //                    assessment = (dsRiskModels.Tables[0].Rows[0]["assessment"].ToString()),
        //                    notes = (dsRiskModels.Tables[0].Rows[0]["notes"].ToString()),
        //                    submission_date = Convert.ToDateTime(dsRiskModels.Tables[0].Rows[0]["submission_date"].ToString()),
        //                    submitted_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["submitted_by"].ToString()),
        //                    impact_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),

        //                    like_id = objRiskMgmtModels.GetLikelihoodNameById(dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
        //                    consequences = dsRiskModels.Tables[0].Rows[0]["consequences"].ToString()
        //                };

        //                if (dicRatings != null && dicRatings.Count > 0)
        //                {
        //                    objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
        //                    objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
        //                }
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = "No Data exists";
        //                return RedirectToAction("RiskList");
        //            }

        //            //Items to populate Mitigation data
        //            RiskMitigationModels objMitigation = new RiskMitigationModels();
        //            DataSet dsMitigation = objGlobaldata.Getdetails("SELECT mit_id, risk_id, submission_date, last_update, eval_id, effort_id, mitigation_owner,"
        //                + " current_solution, submitted_by, MitigationStatus FROM mitigations where risk_id='" + risk_id_trans + "'");

        //            if (dsMitigation.Tables.Count > 0 && dsMitigation.Tables[0].Rows.Count > 0)
        //            {
        //                objMitigation.mit_id = dsMitigation.Tables[0].Rows[0]["mit_id"].ToString();
        //                objMitigation.risk_id = objRiskMgmtModels.GetRiskNameById(dsMitigation.Tables[0].Rows[0]["risk_id"].ToString());
        //                objMitigation.eval_id = objMitigation.GetEvaluationStatusNameById(dsMitigation.Tables[0].Rows[0]["eval_id"].ToString());
        //                objMitigation.effort_id = objMitigation.GetEffortNameById(dsMitigation.Tables[0].Rows[0]["effort_id"].ToString());
        //                objMitigation.mitigation_owner = objGlobaldata.GetEmpHrNameById(dsMitigation.Tables[0].Rows[0]["mitigation_owner"].ToString());
        //                objMitigation.current_solution = dsMitigation.Tables[0].Rows[0]["current_solution"].ToString();
        //                objMitigation.submission_date = Convert.ToDateTime(dsMitigation.Tables[0].Rows[0]["submission_date"].ToString());
        //                objMitigation.submitted_by = objGlobaldata.GetEmpHrNameById(dsMitigation.Tables[0].Rows[0]["submitted_by"].ToString());
        //                objMitigation.MitigationStatus = dsMitigation.Tables[0].Rows[0]["MitigationStatus"].ToString();
        //                ViewBag.objMitigation = objMitigation;
        //            }

        //            //Items to populate Risk Review data
        //            RiskReviewModelsList objRiskReviewModelslst = new RiskReviewModelsList();
        //            objRiskReviewModelslst.lstRiskReviewModels = new List<RiskReviewModels>();
        //            RiskReviewModels objRiskReviewModelsMember = new RiskReviewModels();

        //            DataSet dsRiskReview = objGlobaldata.Getdetails("SELECT review_id, risk_id, submission_date,reviewer,what_monit,where_monit,when_monit,who_monit,how_monit"
        //                    + " FROM mgmt_reviews where risk_id='" + risk_id_trans + "'");

        //            if (dsRiskReview.Tables.Count > 0 && dsRiskReview.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < dsRiskReview.Tables[0].Rows.Count; i++)
        //                {
        //                    RiskReviewModels objRiskReviewModels = new RiskReviewModels
        //                    {
        //                        review_id = (dsRiskReview.Tables[0].Rows[i]["review_id"].ToString()),
        //                        risk_id = objRiskMgmtModels.GetRiskNameById(dsRiskReview.Tables[0].Rows[i]["risk_id"].ToString()),
        //                        reviewer = objGlobaldata.GetEmpHrNameById(dsRiskReview.Tables[0].Rows[i]["reviewer"].ToString()),
        //                        submission_date = Convert.ToDateTime(dsRiskReview.Tables[0].Rows[i]["submission_date"].ToString()),
        //                        what_monit = (dsRiskReview.Tables[0].Rows[i]["what_monit"].ToString()),
        //                        where_monit = (dsRiskReview.Tables[0].Rows[i]["where_monit"].ToString()),
        //                        when_monit = (dsRiskReview.Tables[0].Rows[i]["when_monit"].ToString()),
        //                        who_monit = (dsRiskReview.Tables[0].Rows[i]["who_monit"].ToString()),
        //                        how_monit = (dsRiskReview.Tables[0].Rows[i]["how_monit"].ToString()),
        //                    };

        //                    objRiskReviewModelslst.lstRiskReviewModels.Add(objRiskReviewModels);
        //                }

        //                ViewBag.objRiskReview = objRiskReviewModelslst;
        //            }
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = "Risk Id cannot be Null or empty";
        //            return RedirectToAction("RiskList");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in RiskHistoryDetails: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return View(objRiskMgmtModels);
        //}

        // GET: /RiskMgmt/RiskEdit

        [HttpGet]
        [AllowAnonymous]
        public ActionResult RiskEdit()
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();

            RiskMgmtModels objRisk = new RiskMgmtModels();

            try
            {
                if (Request.QueryString["risk_id"] != null && Request.QueryString["risk_id"] != "")
                {
                    string risk_id = Request.QueryString["risk_id"];
                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                    string sSqlstmt = "select risk_id, risk_refno,risk_status_id, risk_desc, dept, reg_id, branch_id, source_id, cat_id, tech_id, risk_owner, risk_manager,"
                        + "assessment, notes,notified_to, submission_date, close_date, close_by, submitted_by, impact_id, like_id,opp_desc, consequences,Risk_Type,Issue,Location,origin_risk from risk_register where risk_id='"
                        + risk_id + "'";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {
                        //if (objRisk.GetRiskStatusNameById(dsRiskModels.Tables[0].Rows[0]["risk_status_id"].ToString()).ToLower() != "closed")
                        //{
                        objRiskMgmtModels = new RiskMgmtModels
                        {
                            risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                            risk_status_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["risk_status_id"].ToString()),
                            risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
                            dept = dsRiskModels.Tables[0].Rows[0]["dept"].ToString(),
                            reg_id = objGlobaldata.GetISONameById(dsRiskModels.Tables[0].Rows[0]["reg_id"].ToString()),
                            branch_id = (dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                            //cat_id = objRisk.GetRiskCategoryNameById(dsRiskModels.Tables[0].Rows[0]["cat_id"].ToString()),
                            //tech_id = objRisk.GetRiskTechnologyNameById(dsRiskModels.Tables[0].Rows[0]["tech_id"].ToString()),
                            notified_to = (dsRiskModels.Tables[0].Rows[0]["notified_to"].ToString()),
                            risk_owner = (dsRiskModels.Tables[0].Rows[0]["risk_owner"].ToString()),
                            risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
                            assessment = (dsRiskModels.Tables[0].Rows[0]["assessment"].ToString()),
                            notes = (dsRiskModels.Tables[0].Rows[0]["notes"].ToString()),
                            submission_date = Convert.ToDateTime(dsRiskModels.Tables[0].Rows[0]["submission_date"].ToString()),
                            submitted_by = (dsRiskModels.Tables[0].Rows[0]["submitted_by"].ToString()),
                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            consequences = dsRiskModels.Tables[0].Rows[0]["consequences"].ToString(),
                            Risk_Type = dsRiskModels.Tables[0].Rows[0]["Risk_Type"].ToString(),
                            Issue =/*objRisk.GetIssueNameById*/(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                            opp_desc = dsRiskModels.Tables[0].Rows[0]["opp_desc"].ToString(),
                            Location = /*objGlobaldata.GetDivisionLocationById*/(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                            risk_refno = (dsRiskModels.Tables[0].Rows[0]["risk_refno"].ToString()),
                            origin_risk = (dsRiskModels.Tables[0].Rows[0]["origin_risk"].ToString()),
                        };
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString());
                        // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                        if (dsRiskModels.Tables[0].Rows[0]["notified_to"].ToString() != "")
                        {
                            ViewBag.notified_Array = (dsRiskModels.Tables[0].Rows[0]["notified_to"].ToString()).Split(',');
                        }
                        //}
                        //else
                        //{
                        //    TempData["alertdata"] = "Access Denied";
                        //    return RedirectToAction("RiskList");
                        //}
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("RiskList");
                    }
                    //Items to populate Mitigation data
                    RiskMitigationModels objMitigation = new RiskMitigationModels();
                    ViewBag.risk_id = objRisk.GetMultiRiskList();
                    ViewBag.eval_id = objGlobaldata.GetDropdownList("Risk Evaluation Status");
                    ViewBag.effort_id = objGlobaldata.GetDropdownList("Mitigation-effort");
                    // ViewBag.Approver = objGlobaldata.GetApprover();
                    // ViewBag.DeptHead = objGlobaldata.GetDeptHeadList();
                    ViewBag.MitigationStatus = objGlobaldata.GetDropdownList("Mitigation Status");
                    ViewBag.Risk_Type = objGlobaldata.GetConstantValue("Impact");
                    ViewBag.Issue = objRisk.GetIsssuesNo();

                    ViewBag.risk_status_id = objGlobaldata.GetDropdownList("Risk-Status");
                    ViewBag.reg_id = objGlobaldata.GetAllIsoStdListbox();
                    ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                    ViewBag.source_id = objGlobaldata.GetDropdownList("Risk-source");
                    ViewBag.impact_id = objGlobaldata.GetDropdownList("Risk-Severity");
                    ViewBag.like_id = objGlobaldata.GetDropdownList("Risk-likelihood");

                    DataSet dsMitigation = objGlobaldata.Getdetails("SELECT mit_id, risk_id, submission_date, last_update, eval_id, effort_id, mitigation_owner,"
                        + " current_solution, submitted_by, MitigationStatus,TargetDate,DocUpload FROM mitigations where risk_id='" + risk_id + "'");

                    if (dsMitigation.Tables.Count > 0 && dsMitigation.Tables[0].Rows.Count > 0)
                    {
                        objMitigation.mit_id = dsMitigation.Tables[0].Rows[0]["mit_id"].ToString();
                        objMitigation.risk_id = dsMitigation.Tables[0].Rows[0]["risk_id"].ToString();
                        objMitigation.eval_id = dsMitigation.Tables[0].Rows[0]["eval_id"].ToString();
                        objMitigation.effort_id = dsMitigation.Tables[0].Rows[0]["effort_id"].ToString();
                        objMitigation.mitigation_owner = dsMitigation.Tables[0].Rows[0]["mitigation_owner"].ToString();
                        objMitigation.current_solution = dsMitigation.Tables[0].Rows[0]["current_solution"].ToString();
                        objMitigation.DocUpload = dsMitigation.Tables[0].Rows[0]["DocUpload"].ToString();
                        objMitigation.MitigationStatus = objGlobaldata.GetDropdownitemById(dsMitigation.Tables[0].Rows[0]["MitigationStatus"].ToString());

                        DateTime dtDocDate = new DateTime();
                        if (dsMitigation.Tables[0].Rows[0]["TargetDate"].ToString() != ""
                                 && DateTime.TryParse(dsMitigation.Tables[0].Rows[0]["TargetDate"].ToString(), out dtDocDate))
                        {
                            objMitigation.TargetDate = dtDocDate;
                        }

                        ViewBag.objMitigation = objMitigation;
                    }

                    //Items to populate Risk Review data
                    RiskReviewModelsList objRiskReviewModelslst = new RiskReviewModelsList();
                    objRiskReviewModelslst.lstRiskReviewModels = new List<RiskReviewModels>();
                    RiskReviewModels objRiskReviewModelsMember = new RiskReviewModels();

                    ViewBag.level_id = objGlobaldata.GetDropdownList("Risk-ReviewLevel");
                    DataSet dsRiskReview = objGlobaldata.Getdetails("SELECT review_id, risk_id, submission_date, reviewer,what_monit,where_monit,when_monit,who_monit,how_monit"
                            + " FROM mgmt_reviews where risk_id='" + risk_id + "'");

                    if (dsRiskReview.Tables.Count > 0 && dsRiskReview.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsRiskReview.Tables[0].Rows.Count; i++)
                        {
                            RiskReviewModels objRiskReviewModels = new RiskReviewModels
                            {
                                review_id = (dsRiskReview.Tables[0].Rows[i]["review_id"].ToString()),
                                risk_id = (dsRiskReview.Tables[0].Rows[i]["risk_id"].ToString()),
                                reviewer = objGlobaldata.GetEmpHrNameById(dsRiskReview.Tables[0].Rows[i]["reviewer"].ToString()),
                                submission_date = Convert.ToDateTime(dsRiskReview.Tables[0].Rows[i]["submission_date"].ToString()),
                                what_monit = (dsRiskReview.Tables[0].Rows[i]["what_monit"].ToString()),
                                where_monit = (dsRiskReview.Tables[0].Rows[i]["where_monit"].ToString()),
                                when_monit = (dsRiskReview.Tables[0].Rows[i]["when_monit"].ToString()),
                                who_monit = (dsRiskReview.Tables[0].Rows[i]["who_monit"].ToString()),
                                how_monit = (dsRiskReview.Tables[0].Rows[i]["how_monit"].ToString()),
                            };

                            objRiskReviewModelslst.lstRiskReviewModels.Add(objRiskReviewModels);
                        }

                        ViewBag.objRiskReview = objRiskReviewModelslst;
                    }
                }
                else
                {
                    TempData["alertdata"] = "Risk Id cannot be Null or empty";
                    return RedirectToAction("RiskList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objRiskMgmtModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RiskEdit(RiskMgmtModels objRiskMgmt, FormCollection form)
        {
            try
            {
                objRiskMgmt.risk_status_id = form["risk_status_id"];
                objRiskMgmt.reg_id = form["reg_id"];
                objRiskMgmt.branch_id = form["branch_id"];
                objRiskMgmt.source_id = form["source_id"];
                objRiskMgmt.risk_owner = form["risk_owner"];
                objRiskMgmt.risk_manager = form["risk_manager"];
                objRiskMgmt.risk_id = form["risk_id"];
                objRiskMgmt.Issue = form["Issue"];
                objRiskMgmt.dept = form["dept"];
                objRiskMgmt.Location = form["Location"];
                objRiskMgmt.notified_to = form["notified_to"];
                objRiskMgmt.Risk_Type = form["Risk_Type"];
                //objRiskMgmt.submitted_by = objGlobaldata.GetCurrentUserSession().empid;

                //notified_to
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objRiskMgmt.notified_to = form["nempno " + i] + "," + objRiskMgmt.notified_to;
                    }
                }
                if (objRiskMgmt.notified_to != null)
                {
                    objRiskMgmt.notified_to = objRiskMgmt.notified_to.Trim(',');
                }

                if (objRiskMgmt.FunUpdateRisk(objRiskMgmt))
                {
                    TempData["Successdata"] = "Risk details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskList");
        }

        // POST: /RiskMgmt/AddRiskMitigation

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRiskMitigation(FormCollection form, HttpPostedFileBase fileUploader)
        {
            try
            {
                RiskMitigationModels objMitigation = new RiskMitigationModels();

               
                objMitigation.risk_id = form["risk_id"];
                objMitigation.eval_id = form["eval_id"];
                objMitigation.effort_id = form["effort_id"];
                objMitigation.mitigation_owner = form["mitigation_owner"];
                objMitigation.current_solution = form["current_solution"];
                objMitigation.MitigationStatus = form["MitigationStatus"];

                objMitigation.submitted_by = objGlobaldata.GetCurrentUserSession().empid;

                DateTime dateValue;
                if (DateTime.TryParse(form["TargetDate"], out dateValue) == true)
                {
                    objMitigation.TargetDate = dateValue;
                }
                if (fileUploader != null && fileUploader.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/RiskMgmt"), Path.GetFileName(fileUploader.FileName));
                        string sFilename = "Risk" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        fileUploader.SaveAs(sFilepath + "/" + sFilename);
                        objMitigation.DocUpload = "~/DataUpload/MgmtDocs/RiskMgmt/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objMitigation.FunAddRiskMitigations(objMitigation))
                {
                    TempData["Successdata"] = "Added Risk Mitigation details successfully";
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

            return RedirectToAction("RiskList");
        }

        // POST: /RiskMgmt/AddRiskMgmtReview

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRiskMgmtReview(FormCollection form)
        {
            try
            {
                if (form["chkNew"] != null && form["chkNew"].ToLower() == "true")
                {
                    RiskReviewModels objMitigation = new RiskReviewModels();
                    objMitigation.what_monit = form["what_monit"];
                    objMitigation.where_monit = form["where_monit"];
                    objMitigation.when_monit = form["when_monit"];
                    objMitigation.who_monit = form["who_monit"];
                    objMitigation.how_monit = form["how_monit"];
                    objMitigation.risk_id = form["risk_id"];

                    objMitigation.reviewer = objGlobaldata.GetCurrentUserSession().empid;

                    if (objMitigation.FunAddRiskReview(objMitigation))
                    {
                        TempData["Successdata"] = "Added Risk Review details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    if (form["review_id"] != null && form["review_id"] != "")
                    {
                        RiskMgmtReviewUpdate(form);
                    }
                    else
                    {
                        TempData["alertdata"] = "Please select Review item to edit or Select New for adding new review";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskMgmtReview: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskList");
        }

        // POST: /RiskMgmt/RiskMgmtReviewUpdate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RiskMgmtReviewUpdate(FormCollection form)
        {
            try
            {
                RiskReviewModels objReview = new RiskReviewModels();
                objReview.what_monit = form["what_monit"];
                objReview.where_monit = form["where_monit"];
                objReview.when_monit = form["when_monit"];
                objReview.who_monit = form["who_monit"];
                objReview.how_monit = form["how_monit"];
                objReview.review_id = form["review_id"];

                objReview.reviewer = objGlobaldata.GetCurrentUserSession().empid;

                if (objReview.FunUpdateRiskReview(objReview))
                {
                    TempData["Successdata"] = "Risk Review details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskMgmtReviewUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskList");
        }

        [AllowAnonymous]
        public ActionResult MitigationHistoryList(string risk_id, int? page)
        {
            RiskMitigationModelsList objRiskModelsList = new RiskMitigationModelsList();
            objRiskModelsList.MitigationList = new List<RiskMitigationModels>();
            //MitigationimpactList objRiskModelsList = new MitigationimpactList();
            //objRiskModelsList.lstMitigationimpact = new List<RiskMitigationModels>();

            RiskMitigationModels objMitigation = new RiskMitigationModels();
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                string sSqlstmt = "select mit_trans_id, mit_id, risk_id, eval_id, effort_id, current_solution, DocUpload, TargetDate from mitigations_trans where risk_id='" + risk_id + "' order by risk_id desc ";
                DataSet dsMitigation = objGlobaldata.Getdetails(sSqlstmt);

                if (dsMitigation.Tables.Count > 0 && dsMitigation.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsMitigation.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objMitigation = new RiskMitigationModels();
                            {
                                objMitigation.mit_trans_id = dsMitigation.Tables[0].Rows[i]["mit_trans_id"].ToString();
                                objMitigation.mit_id = dsMitigation.Tables[0].Rows[i]["mit_id"].ToString();
                                objMitigation.risk_id = objRiskMgmtModels.GetRiskNameById(dsMitigation.Tables[0].Rows[i]["risk_id"].ToString());
                                objMitigation.eval_id = objGlobaldata.GetDropdownitemById(dsMitigation.Tables[0].Rows[i]["eval_id"].ToString());
                                objMitigation.effort_id = objGlobaldata.GetDropdownitemById(dsMitigation.Tables[0].Rows[i]["effort_id"].ToString());
                                objMitigation.current_solution = dsMitigation.Tables[0].Rows[i]["current_solution"].ToString();
                                objMitigation.DocUpload = dsMitigation.Tables[0].Rows[i]["DocUpload"].ToString();
                            };
                            DateTime dtDocDate = new DateTime();
                            if (dsMitigation.Tables[0].Rows[i]["TargetDate"].ToString() != ""
                                     && DateTime.TryParse(dsMitigation.Tables[0].Rows[i]["TargetDate"].ToString(), out dtDocDate))
                            {
                                objMitigation.TargetDate = dtDocDate;
                            }
                            objRiskModelsList.MitigationList.Add(objMitigation);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in MgmtDocumentsHistoryList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MitigationHistoryList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objRiskModelsList.MitigationList.ToList());
        }

        [HttpPost]
        public JsonResult FunGetRiskRating(string impact, string like)
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            //string RiskRating = "";
            if (impact != null && like != null)
            {
                Dictionary<string, string> dicRatings = new Dictionary<string, string>();

                dicRatings = objRisk.GetRiskRatings(impact, like);
                if (dicRatings != null && dicRatings.Count > 0)
                {
                    //RiskRating = dicRatings.FirstOrDefault().Key;
                    objRisk.RiskRating = dicRatings.FirstOrDefault().Key;
                    objRisk.color_code = dicRatings.FirstOrDefault().Value;
                }
            }

            return Json(objRisk);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddRiskEvaluation()
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                if (Request.QueryString["risk_id"] != null && Request.QueryString["risk_id"] != "")
                {
                    string risk_id = Request.QueryString["risk_id"];
                    ViewBag.impact_id = objGlobaldata.GetDropdownList("Risk-Severity");
                    ViewBag.like_id = objGlobaldata.GetDropdownList("Risk-likelihood");

                    ViewBag.EvaluatedBy = objGlobaldata.GetDeptHeadbyDivisionList();
                    // ViewBag.Issue = objRisk.GetIsssuesNo();                   
                    ViewBag.risk_id = risk_id;
                    string sSqlstmt = "select dept,branch_id,Location,risk_id,impact_id,like_id,risk_manager,Issue,evaluation_date,eval_notified_to  from risk_register where risk_id='"
                        + risk_id + "'";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {
                        objRiskMgmtModels = new RiskMgmtModels
                        {
                            risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            risk_manager =/* objGlobaldata.GetMultiHrEmpNameById*/(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
                            // Issue = objRisk.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                            eval_notified_to = /*objGlobaldata.GetMultiHrEmpNameById*/(dsRiskModels.Tables[0].Rows[0]["eval_notified_to"].ToString()),
                            branch_id = (dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            dept = (dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            Location = (dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.evaluation_date = dtValue;
                        }

                    
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        //ViewBag.Approver = objGlobaldata.GetGRoleList(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString(), "Approver");
                        //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("RiskList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRiskMgmtModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddRiskEvaluation(FormCollection form, RiskMgmtModels objRiskMgmt)
        {
            try
            {

                objRiskMgmt.eval_notified_to = form["eval_notified_to"];

                if (objRiskMgmt.FunUpdateRiskEvaluation(objRiskMgmt))
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
            return RedirectToAction("RiskList");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddRiskMitigations()
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                if (Request.QueryString["risk_id"] != null && Request.QueryString["risk_id"] != "")
                {
                    string risk_id = Request.QueryString["risk_id"];
                    ViewBag.risk_id = risk_id;

                    string sSqlstmt = "select dept,branch_id,Location,risk_id,risk_refno,impact_id,like_id,risk_desc,approved_by,reeval_due_date,mit_notified_to,branch_id,dept,Location,source_id,Issue,Risk_Type,consequences,risk_owner from risk_register where risk_id='"
                        + risk_id + "'";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {
                        objRiskMgmtModels = new RiskMgmtModels
                        {
                            risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
                            approved_by = (dsRiskModels.Tables[0].Rows[0]["approved_by"].ToString()),
                            risk_refno = (dsRiskModels.Tables[0].Rows[0]["risk_refno"].ToString()),
                            mit_notified_to = (dsRiskModels.Tables[0].Rows[0]["mit_notified_to"].ToString()),

                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                            source_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Issue = objRiskMgmtModels.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                            Risk_Type = dsRiskModels.Tables[0].Rows[0]["Risk_Type"].ToString(),
                            consequences = dsRiskModels.Tables[0].Rows[0]["consequences"].ToString(),
                            risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_owner"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.reeval_due_date = dtValue;
                        }

                        if (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() == null || dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() == "")
                        {
                            TempData["Successdata"] = "Kindly do the Risk Evaluation";
                            return RedirectToAction("RiskList");
                        }
                        ViewBag.Approver = objGlobaldata.GetApprover();
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        //ViewBag.Approver = objGlobaldata.GetGRoleList(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString(), "Approver");
                        //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());

                        RiskMgmtModelsList objRiskList = new RiskMgmtModelsList();
                        objRiskList.lstRiskMgmtModels = new List<RiskMgmtModels>();

                        sSqlstmt = "select mit_id,risk_id,measure,pers_resp,target_date from risk_mitigations where risk_id='" + risk_id + "'";
                        DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    RiskMgmtModels objMit = new RiskMgmtModels
                                    {
                                        mit_id = dsMitList.Tables[0].Rows[i]["mit_id"].ToString(),
                                        risk_id = dsMitList.Tables[0].Rows[i]["risk_id"].ToString(),
                                        measure = dsMitList.Tables[0].Rows[i]["measure"].ToString(),
                                        pers_resp = dsMitList.Tables[0].Rows[i]["pers_resp"].ToString(),
                                    };
                                    if (DateTime.TryParse(dsMitList.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                                    {
                                        objMit.target_date = dtValue;
                                    }
                                    objRiskList.lstRiskMgmtModels.Add(objMit);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddRiskMitigations: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("RiskList");
                                }
                            }
                            ViewBag.objMitList = objRiskList;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("RiskList");
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
        public ActionResult AddRiskMitigations(FormCollection form, RiskMgmtModels objRiskMgmt)
        {
            try
            {
                objRiskMgmt.mit_notified_to = form["mit_notified_to"];
                RiskMgmtModelsList objRiskList = new RiskMgmtModelsList();
                objRiskList.lstRiskMgmtModels = new List<RiskMgmtModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["reeval_due_date"], out dateValue) == true)
                {
                    objRiskMgmt.reeval_due_date = dateValue;
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    RiskMgmtModels objMitModel = new RiskMgmtModels();
                    if (form["measure " + i] != "" && form["measure " + i] != null)
                    {
                        objMitModel.mit_id = form["mit_id " + i];
                        objMitModel.measure = form["measure " + i];
                        objMitModel.pers_resp = form["pers_resp " + i];
                        if (DateTime.TryParse(form["target_date " + i], out dateValue) == true)
                        {
                            objMitModel.target_date = dateValue;
                        }
                        objRiskList.lstRiskMgmtModels.Add(objMitModel);
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
            return RedirectToAction("RiskList");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddRiskReEvaluation()
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                if (Request.QueryString["risk_id"] != null && Request.QueryString["risk_id"] != "")
                {
                    string risk_id = Request.QueryString["risk_id"];
                    ViewBag.impact_id = objGlobaldata.GetDropdownList("Risk-Severity");
                    ViewBag.like_id = objGlobaldata.GetDropdownList("Risk-likelihood");
                    ViewBag.EvaluatedBy = objGlobaldata.GetDeptHeadbyDivisionList();
                    ViewBag.Issue = objRisk.GetIsssuesNo();
                    ViewBag.risk_id = risk_id;
                    DateTime today_date = DateTime.Now;
                    //string sSqlstmt = "select risk_id_trans,risk_refno,risk_desc,risk_id,impact_id,like_id,risk_manager,Issue,evaluation_date,reeval_due_date from risk_register_trans where risk_id='"
                    //    + risk_id + "' order by risk_id_trans desc limit 1";
                    string sSqlstmt = "select t.dept,t.branch_id,t.Location,tt.risk_id_trans,tt.risk_refno,tt.risk_desc,tt.risk_id,tt.impact_id,tt.like_id,t.impact_id as initimpact_id,t.like_id as initlike_id,tt.risk_manager,"
                    + " tt.Issue,tt.evaluation_date,tt.reeval_due_date,tt.eval_notified_to,t.evaluation_date as initevaluation_date from risk_register_trans tt,risk_register t"
                    + " where t.risk_id = tt.risk_id and tt.risk_id = '" + risk_id + "' order by risk_id_trans desc limit 1";
                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {
                        objRiskMgmtModels = new RiskMgmtModels
                        {
                            risk_id_trans = (dsRiskModels.Tables[0].Rows[0]["risk_id_trans"].ToString()),
                            risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
                            Issue = objRisk.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                            risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
                            risk_refno = (dsRiskModels.Tables[0].Rows[0]["risk_refno"].ToString()),
                            initimpact_id = (dsRiskModels.Tables[0].Rows[0]["initimpact_id"].ToString()),
                            initlike_id = (dsRiskModels.Tables[0].Rows[0]["initlike_id"].ToString()),
                            eval_notified_to = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["eval_notified_to"].ToString()),
                            branch_id = (dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            dept = (dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            Location = (dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        //ViewBag.Approver = objGlobaldata.GetGRoleList(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString(), "Approver");
                        //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                       
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        DateTime dtValue;
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["initevaluation_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.initevaluation_date = dtValue;
                        }
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                        {
                            if (today_date < dtValue)
                            {
                                if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                                {
                                    objRiskMgmtModels.evaluation_date = dtValue;
                                }
                            }
                        }
                        else
                        {
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.evaluation_date = dtValue;
                            }
                        }
                    }
                    else if (dsRiskModels.Tables[0].Rows.Count == 0)
                    {
                        sSqlstmt = "select dept,branch_id,Location,risk_id,eval_notified_to,impact_id,like_id,risk_manager,risk_desc,risk_refno,Issue,evaluation_date  from risk_register where risk_id='"
                      + risk_id + "'";

                        dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);
                        objRiskMgmtModels = new RiskMgmtModels
                        {
                            risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                            initimpact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            initlike_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
                            Issue = objRisk.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                            risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
                            risk_refno = (dsRiskModels.Tables[0].Rows[0]["risk_refno"].ToString()),
                            eval_notified_to = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["eval_notified_to"].ToString()),
                            branch_id = (dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            dept = (dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            Location = (dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.initevaluation_date = dtValue;
                        }
                        //ViewBag.Approver = objGlobaldata.GetGRoleList(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString(), "Approver");
                        //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());
                        ViewBag.Approver = objGlobaldata.GetApprover();
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("RiskList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskReEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRiskMgmtModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddRiskReEvaluation(FormCollection form, RiskMgmtModels objRiskMgmt)
        {
            try
            {
               
                objRiskMgmt.eval_notified_to = form["eval_notified_to"];

                if (objRiskMgmt.FunUpdateRiskReEvaluation(objRiskMgmt))
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
            return RedirectToAction("RiskList");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult FurtherRiskMitigations()
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                if (Request.QueryString["risk_id"] != null && Request.QueryString["risk_id"] != "")
                {
                    string risk_id = Request.QueryString["risk_id"];
                    ViewBag.risk_id = risk_id;
                    ViewBag.Approver = objGlobaldata.GetApprover();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    DateTime today_date = DateTime.Now;
                    string sSqlstmt = "select t.dept,t.branch_id,t.Location,risk_id_trans,tt.risk_id,tt.risk_refno,tt.impact_id," +
                        "tt.like_id,tt.risk_desc,tt.approved_by,tt.reeval_due_date,tt.evaluation_date,tt.mit_notified_to,t.branch_id,t.dept,t.Location,t.source_id,t.Issue,t.Risk_Type,t.consequences,t.risk_owner from risk_register_trans tt,risk_register t where tt.risk_id='"
                        + risk_id + "' and t.risk_id = tt.risk_id order by risk_id_trans desc limit 1";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {
                        objRiskMgmtModels = new RiskMgmtModels
                        {
                            risk_id_trans = (dsRiskModels.Tables[0].Rows[0]["risk_id_trans"].ToString()),
                            risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                            impact_id = (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = (dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
                            approved_by = (dsRiskModels.Tables[0].Rows[0]["approved_by"].ToString()),
                            risk_refno = (dsRiskModels.Tables[0].Rows[0]["risk_refno"].ToString()),
                            mit_notified_to = (dsRiskModels.Tables[0].Rows[0]["mit_notified_to"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                            source_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                            Issue = objRiskMgmtModels.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                            Risk_Type = dsRiskModels.Tables[0].Rows[0]["Risk_Type"].ToString(),
                            consequences = dsRiskModels.Tables[0].Rows[0]["consequences"].ToString(),
                            risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_owner"].ToString()),
                        };

                        DateTime dtValue, dtEval;
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
                                    return RedirectToAction("RiskList");
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
                        ViewBag.Approver = objGlobaldata.GetApprover();
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        //ViewBag.Approver = objGlobaldata.GetGRoleList(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString(), "Approver");
                        //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString(), dsRiskModels.Tables[0].Rows[0]["dept"].ToString(), dsRiskModels.Tables[0].Rows[0]["Location"].ToString());

                        RiskMgmtModelsList objRiskList = new RiskMgmtModelsList();
                        objRiskList.lstRiskMgmtModels = new List<RiskMgmtModels>();

                        sSqlstmt = "select mit_id_trans,risk_id_trans,mit_id,risk_id,measure,pers_resp,target_date from risk_mitigations_trans where risk_id_trans='" + objRiskMgmtModels.risk_id_trans + "'";
                        DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    RiskMgmtModels objMit = new RiskMgmtModels
                                    {
                                        mit_id_trans = dsMitList.Tables[0].Rows[i]["mit_id_trans"].ToString(),
                                        risk_id_trans = dsMitList.Tables[0].Rows[i]["risk_id_trans"].ToString(),
                                        mit_id = dsMitList.Tables[0].Rows[i]["mit_id"].ToString(),
                                        risk_id = dsMitList.Tables[0].Rows[i]["risk_id"].ToString(),
                                        measure = dsMitList.Tables[0].Rows[i]["measure"].ToString(),
                                        pers_resp = dsMitList.Tables[0].Rows[i]["pers_resp"].ToString(),
                                    };
                                    if (DateTime.TryParse(dsMitList.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                                    {
                                        objMit.target_date = dtValue;
                                    }
                                    objRiskList.lstRiskMgmtModels.Add(objMit);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddRiskMitigations: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("RiskList");
                                }
                            }
                            ViewBag.objMitList = objRiskList;
                        }
                    }
                    else
                    {
                        TempData["Successdata"] = "Id is null";
                        return RedirectToAction("RiskList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FurtherRiskMitigations: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRiskMgmtModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult FurtherRiskMitigations(FormCollection form, RiskMgmtModels objRiskMgmt)
        {
            try
            {
                objRiskMgmt.mit_notified_to = form["mit_notified_to"];

                RiskMgmtModelsList objRiskList = new RiskMgmtModelsList();
                objRiskList.lstRiskMgmtModels = new List<RiskMgmtModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["reeval_due_date"], out dateValue) == true)
                {
                    objRiskMgmt.reeval_due_date = dateValue;
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    RiskMgmtModels objMitModel = new RiskMgmtModels();
                    if (form["measure " + i] != "" && form["measure " + i] != null)
                    {
                        objMitModel.mit_id_trans = form["mit_id_trans " + i];
                        objMitModel.mit_id = form["mit_id " + i];
                        objMitModel.measure = form["measure " + i];
                        objMitModel.pers_resp = form["pers_resp " + i];
                        if (DateTime.TryParse(form["target_date " + i], out dateValue) == true)
                        {
                            objMitModel.target_date = dateValue;
                        }
                        objRiskList.lstRiskMgmtModels.Add(objMitModel);
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
                objGlobaldata.AddFunctionalLog("Exception in AddRiskMitigation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("RiskList");
        }

        [AllowAnonymous]
        public ActionResult RiskDetails()
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                if (Request.QueryString["risk_id"] != null)
                {
                    string srisk_id = Request.QueryString["risk_id"];
                    string sSqlstmt = "";
                    string sSqlstmt1 = "select risk_id_trans from risk_register_trans where risk_id='" + srisk_id + "'";
                    DataSet dsRiskModels1 = objGlobaldata.Getdetails(sSqlstmt1);
                    int count = dsRiskModels1.Tables[0].Rows.Count;
                    if (count >= 1)
                    {
                        sSqlstmt = "select risk_id_trans,tt.risk_id_trans,t.risk_id,t.risk_refno,t.risk_desc,t.dept,t.branch_id,t.source_id,t.risk_owner,t.risk_manager,t.submission_date,t.submitted_by,t.consequences,t.Location,tt.evaluation_date,tt.approved_by,tt.approved_date,tt.reeval_due_date,tt.impact_id,tt.like_id,t.Issue,t.Risk_Type,t.origin_risk,"
                           + "(CASE WHEN tt.apprv_status='0' THEN 'Pending for Approval' WHEN tt.apprv_status='1' THEN 'Rejected' WHEN tt.apprv_status='2' THEN 'Approved' END) as apprv_status,tt.apprv_comment"
                        + " from risk_register t left join  risk_register_trans tt on t.risk_id = tt.risk_id  where t.risk_id = '" + srisk_id + "' order by risk_id_trans desc limit 1";

                        DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                        {
                            Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                            if (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                            {
                                dicRatings = objRiskMgmtModels.GetRiskRatings(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString(),
                                    dsRiskModels.Tables[0].Rows[0]["like_id"].ToString());
                            }
                            objRiskMgmtModels = new RiskMgmtModels
                            {
                                risk_id_trans = (dsRiskModels.Tables[0].Rows[0]["risk_id_trans"].ToString()),
                                risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                                risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                                branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                                source_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                                risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_owner"].ToString()),
                                risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
                                submitted_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["submitted_by"].ToString()),
                                impact_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                                like_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                                consequences = dsRiskModels.Tables[0].Rows[0]["consequences"].ToString(),
                                Issue = objRiskMgmtModels.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                                approved_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["approved_by"].ToString()),
                                risk_refno = (dsRiskModels.Tables[0].Rows[0]["risk_refno"].ToString()),
                                Risk_Type = dsRiskModels.Tables[0].Rows[0]["Risk_Type"].ToString(),

                                apprv_status = dsRiskModels.Tables[0].Rows[0]["apprv_status"].ToString(),
                                apprv_comment = dsRiskModels.Tables[0].Rows[0]["apprv_comment"].ToString(),
                                approved_by_Id = (dsRiskModels.Tables[0].Rows[0]["approved_by"].ToString()),
                                origin_risk = (dsRiskModels.Tables[0].Rows[0]["origin_risk"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.evaluation_date = dtValue;
                            }
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.reeval_due_date = dtValue;
                            }
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["approved_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.approved_date = dtValue;
                            }
                            if (dicRatings != null && dicRatings.Count > 0)
                            {
                                objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
                                objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
                            }

                            sSqlstmt = "select mit_id,risk_id,measure,pers_resp,target_date from risk_mitigations where risk_id='" + srisk_id + "'";
                            ViewBag.Mit = objGlobaldata.Getdetails(sSqlstmt);

                            return View(objRiskMgmtModels);
                        }
                        else
                        {
                            TempData["alertdata"] = "No Data exists";
                            return RedirectToAction("RiskList");
                        }
                    }
                    else
                    {
                        sSqlstmt = "select risk_id,risk_refno,risk_desc,dept,branch_id,source_id,risk_owner,risk_manager,submission_date,submitted_by,"
                        + "(CASE WHEN apprv_status='0' THEN 'Pending for Approval' WHEN apprv_status='1' THEN 'Rejected' WHEN apprv_status='2' THEN 'Approved' END) as apprv_status,apprv_comment,"
                        + "consequences,Location,evaluation_date,approved_by,approved_date,reeval_due_date,impact_id,like_id,Issue,Risk_Type,origin_risk from risk_register"
                        + " where risk_id='" + srisk_id + "' ";

                        DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                        {
                            Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                            if (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                            {
                                dicRatings = objRiskMgmtModels.GetRiskRatings(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString(),
                                    dsRiskModels.Tables[0].Rows[0]["like_id"].ToString());
                            }
                            objRiskMgmtModels = new RiskMgmtModels
                            {
                                risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                                risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                                branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                                source_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                                risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_owner"].ToString()),
                                risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
                                submitted_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["submitted_by"].ToString()),
                                impact_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                                like_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                                consequences = dsRiskModels.Tables[0].Rows[0]["consequences"].ToString(),
                                Issue = objRiskMgmtModels.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                                approved_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["approved_by"].ToString()),
                                risk_refno = (dsRiskModels.Tables[0].Rows[0]["risk_refno"].ToString()),
                                Risk_Type = dsRiskModels.Tables[0].Rows[0]["Risk_Type"].ToString(),

                                apprv_status = dsRiskModels.Tables[0].Rows[0]["apprv_status"].ToString(),
                                apprv_comment = dsRiskModels.Tables[0].Rows[0]["apprv_comment"].ToString(),
                                approved_by_Id = (dsRiskModels.Tables[0].Rows[0]["approved_by"].ToString()),
                                origin_risk = (dsRiskModels.Tables[0].Rows[0]["origin_risk"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.evaluation_date = dtValue;
                            }
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.reeval_due_date = dtValue;
                            }
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["approved_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.approved_date = dtValue;
                            }
                            if (dicRatings != null && dicRatings.Count > 0)
                            {
                                objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
                                objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
                            }

                            sSqlstmt = "select mit_id,risk_id,measure,pers_resp,target_date from risk_mitigations where risk_id='" + srisk_id + "'";
                            ViewBag.Mit = objGlobaldata.Getdetails(sSqlstmt);

                            return View(objRiskMgmtModels);
                        }
                        else
                        {
                            TempData["alertdata"] = "No Data exists";
                            return RedirectToAction("RiskList");
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("RiskList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("RiskList");
        }

        [AllowAnonymous]
        public ActionResult RiskApprovalDetails()
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                if (Request.QueryString["risk_id"] != null)
                {
                    string srisk_id = Request.QueryString["risk_id"];
                    string sSqlstmt = "";
                    string sSqlstmt1 = "select risk_id_trans from risk_register_trans where risk_id='" + srisk_id + "'";
                    DataSet dsRiskModels1 = objGlobaldata.Getdetails(sSqlstmt1);
                    int count = dsRiskModels1.Tables[0].Rows.Count;
                    if (count >= 1)
                    {
                        sSqlstmt = "select risk_id_trans,tt.risk_id_trans,t.risk_id,t.risk_refno,t.risk_desc,t.dept,t.branch_id,t.source_id,t.risk_owner,t.risk_manager,t.submission_date,t.submitted_by,t.consequences,t.Location,tt.evaluation_date,tt.approved_by,tt.approved_date,tt.reeval_due_date,tt.impact_id,tt.like_id,t.Issue,t.Risk_Type,"
                           + "(CASE WHEN tt.apprv_status='0' THEN 'Pending for Approval' WHEN tt.apprv_status='1' THEN 'Rejected' WHEN tt.apprv_status='2' THEN 'Approved' END) as apprv_status,tt.apprv_comment"
                        + " from risk_register t left join  risk_register_trans tt on t.risk_id = tt.risk_id  where t.risk_id = '" + srisk_id + "' order by risk_id_trans desc limit 1";

                        DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                        {
                            Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                            if (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                            {
                                dicRatings = objRiskMgmtModels.GetRiskRatings(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString(),
                                    dsRiskModels.Tables[0].Rows[0]["like_id"].ToString());
                            }
                            objRiskMgmtModels = new RiskMgmtModels
                            {
                                risk_id_trans = (dsRiskModels.Tables[0].Rows[0]["risk_id_trans"].ToString()),
                                risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                                risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                                branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                                source_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                                risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_owner"].ToString()),
                                risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
                                submitted_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["submitted_by"].ToString()),
                                impact_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                                like_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                                consequences = dsRiskModels.Tables[0].Rows[0]["consequences"].ToString(),
                                Issue = objRiskMgmtModels.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                                approved_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["approved_by"].ToString()),
                                risk_refno = (dsRiskModels.Tables[0].Rows[0]["risk_refno"].ToString()),
                                Risk_Type = dsRiskModels.Tables[0].Rows[0]["Risk_Type"].ToString(),

                                apprv_status = dsRiskModels.Tables[0].Rows[0]["apprv_status"].ToString(),
                                apprv_comment = dsRiskModels.Tables[0].Rows[0]["apprv_comment"].ToString(),
                                approved_by_Id = (dsRiskModels.Tables[0].Rows[0]["approved_by"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.evaluation_date = dtValue;
                            }
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.reeval_due_date = dtValue;
                            }
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["approved_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.approved_date = dtValue;
                            }
                            if (dicRatings != null && dicRatings.Count > 0)
                            {
                                objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
                                objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
                            }

                            sSqlstmt = "select mit_id,risk_id,measure,pers_resp,target_date from risk_mitigations_trans where risk_id_trans='" + objRiskMgmtModels.risk_id_trans + "'";
                            ViewBag.Mit = objGlobaldata.Getdetails(sSqlstmt);

                            return View(objRiskMgmtModels);
                        }
                        else
                        {
                            TempData["alertdata"] = "No Data exists";
                            return RedirectToAction("RiskList");
                        }
                    }
                    else
                    {
                        sSqlstmt = "select risk_id,risk_refno,risk_desc,dept,branch_id,source_id,risk_owner,risk_manager,submission_date,submitted_by,"
                        + "(CASE WHEN apprv_status='0' THEN 'Pending for Approval' WHEN apprv_status='1' THEN 'Rejected' WHEN apprv_status='2' THEN 'Approved' END) as apprv_status,apprv_comment,"
                        + "consequences,Location,evaluation_date,approved_by,approved_date,reeval_due_date,impact_id,like_id,Issue,Risk_Type from risk_register"
                        + " where risk_id='" + srisk_id + "' ";

                        DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                        {
                            Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                            if (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                            {
                                dicRatings = objRiskMgmtModels.GetRiskRatings(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString(),
                                    dsRiskModels.Tables[0].Rows[0]["like_id"].ToString());
                            }
                            objRiskMgmtModels = new RiskMgmtModels
                            {
                                risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                                risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                                branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                                source_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                                risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_owner"].ToString()),
                                risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
                                submitted_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["submitted_by"].ToString()),
                                impact_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                                like_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                                consequences = dsRiskModels.Tables[0].Rows[0]["consequences"].ToString(),
                                Issue = objRiskMgmtModels.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                                approved_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["approved_by"].ToString()),
                                risk_refno = (dsRiskModels.Tables[0].Rows[0]["risk_refno"].ToString()),
                                Risk_Type = dsRiskModels.Tables[0].Rows[0]["Risk_Type"].ToString(),

                                apprv_status = dsRiskModels.Tables[0].Rows[0]["apprv_status"].ToString(),
                                apprv_comment = dsRiskModels.Tables[0].Rows[0]["apprv_comment"].ToString(),
                                approved_by_Id = (dsRiskModels.Tables[0].Rows[0]["approved_by"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.evaluation_date = dtValue;
                            }
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.reeval_due_date = dtValue;
                            }
                            if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["approved_date"].ToString(), out dtValue))
                            {
                                objRiskMgmtModels.approved_date = dtValue;
                            }
                            if (dicRatings != null && dicRatings.Count > 0)
                            {
                                objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
                                objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
                            }

                            sSqlstmt = "select mit_id,risk_id,measure,pers_resp,target_date from risk_mitigations where risk_id='" + srisk_id + "'";
                            ViewBag.Mit = objGlobaldata.Getdetails(sSqlstmt);

                            return View(objRiskMgmtModels);
                        }
                        else
                        {
                            TempData["alertdata"] = "No Data exists";
                            return RedirectToAction("RiskList");
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("RiskList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskApprovalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("RiskList");
        }

        public ActionResult RiskApprove(RiskMgmtModels objRisk, FormCollection form)
        {
            try
            {
                if (objRisk.FunUpdateApprove(objRisk))
                {
                    TempData["Successdata"] = "Status updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult RiskHistoryList()
        {
            RiskMgmtModelsList objRiskModelsList = new RiskMgmtModelsList();
            objRiskModelsList.lstRiskMgmtModels = new List<RiskMgmtModels>();
            RiskMgmtModels objRisk = new RiskMgmtModels();

            try
            {
                if (Request.QueryString["risk_id"] != null)
                {
                    string srisk_id = Request.QueryString["risk_id"];
                    string sSqlstmt = "select risk_id_trans,risk_id,risk_desc,like_id,impact_id,approved_by,evaluation_date,reeval_due_date from risk_register_trans"
             + " where risk_id='" + srisk_id + "' ";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; dsRiskModels.Tables.Count > 0 && i < dsRiskModels.Tables[0].Rows.Count; i++)
                        {
                            Dictionary<string, string> dicRatings = new Dictionary<string, string>();

                            if (dsRiskModels.Tables[0].Rows[i]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[i]["like_id"].ToString() != "")
                            {
                                dicRatings = objRisk.GetRiskRatings(dsRiskModels.Tables[0].Rows[i]["impact_id"].ToString(),
                                dsRiskModels.Tables[0].Rows[i]["like_id"].ToString());
                            }

                            try
                            {
                                RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels
                                {
                                    risk_id = (dsRiskModels.Tables[0].Rows[i]["risk_id"].ToString()),
                                    risk_id_trans = (dsRiskModels.Tables[0].Rows[i]["risk_id_trans"].ToString()),
                                    risk_desc = (dsRiskModels.Tables[0].Rows[i]["risk_desc"].ToString()),
                                    impact_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[i]["impact_id"].ToString()),
                                    like_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[i]["like_id"].ToString()),
                                    approved_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[i]["approved_by"].ToString()),
                                };
                                DateTime dtValue;
                                if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[i]["evaluation_date"].ToString(), out dtValue))
                                {
                                    objRiskMgmtModels.evaluation_date = dtValue;
                                }
                                if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[i]["reeval_due_date"].ToString(), out dtValue))
                                {
                                    objRiskMgmtModels.reeval_due_date = dtValue;
                                }
                                if (dicRatings != null && dicRatings.Count > 0)
                                {
                                    objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
                                    objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
                                }

                                objRiskModelsList.lstRiskMgmtModels.Add(objRiskMgmtModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in RiskHistoryList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["Successdata"] = "No Data Exists";
                        return RedirectToAction("RiskList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("RiskList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskHistoryList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objRiskModelsList.lstRiskMgmtModels.ToList());
        }

        [AllowAnonymous]
        public ActionResult RiskHistoryDetails()
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                if (Request.QueryString["risk_id_trans"] != null)
                {
                    string srisk_id_trans = Request.QueryString["risk_id_trans"];
                    string sSqlstmt = "";

                    sSqlstmt = "select t.risk_id,tt.risk_desc,t.dept,t.branch_id,t.source_id,t.risk_owner,t.risk_manager,t.submission_date,"
                    + "t.submitted_by,t.consequences,t.Location,tt.evaluation_date,tt.approved_by,tt.approved_date,tt.reeval_due_date,"
                    + "(CASE WHEN tt.apprv_status='0' THEN 'Pending for Approval' WHEN tt.apprv_status='1' THEN 'Rejected' WHEN tt.apprv_status='2' THEN 'Approved' END) as apprv_status,tt.apprv_comment,"
                    + " tt.impact_id,tt.like_id,t.Issue from risk_register t, risk_register_trans tt where t.risk_id = tt.risk_id"
                    + " and risk_id_trans = '" + srisk_id_trans + "'";

                    DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                    {
                        Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                        if (dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsRiskModels.Tables[0].Rows[0]["like_id"].ToString() != "")
                        {
                            dicRatings = objRiskMgmtModels.GetRiskRatings(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString(),
                                dsRiskModels.Tables[0].Rows[0]["like_id"].ToString());
                        }
                        objRiskMgmtModels = new RiskMgmtModels
                        {
                            risk_id = (dsRiskModels.Tables[0].Rows[0]["risk_id"].ToString()),
                            risk_desc = (dsRiskModels.Tables[0].Rows[0]["risk_desc"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsRiskModels.Tables[0].Rows[0]["dept"].ToString()),
                            branch_id = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskModels.Tables[0].Rows[0]["branch_id"].ToString()),
                            source_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["source_id"].ToString()),
                            risk_owner = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_owner"].ToString()),
                            risk_manager = objGlobaldata.GetMultiHrEmpNameById(dsRiskModels.Tables[0].Rows[0]["risk_manager"].ToString()),
                            submitted_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["submitted_by"].ToString()),
                            impact_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["impact_id"].ToString()),
                            like_id = objGlobaldata.GetDropdownitemById(dsRiskModels.Tables[0].Rows[0]["like_id"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsRiskModels.Tables[0].Rows[0]["Location"].ToString()),
                            consequences = dsRiskModels.Tables[0].Rows[0]["consequences"].ToString(),
                            Issue = objRiskMgmtModels.GetIssueNameById(dsRiskModels.Tables[0].Rows[0]["Issue"].ToString()),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsRiskModels.Tables[0].Rows[0]["approved_by"].ToString()),

                            apprv_status = dsRiskModels.Tables[0].Rows[0]["apprv_status"].ToString(),
                            apprv_comment = dsRiskModels.Tables[0].Rows[0]["apprv_comment"].ToString(),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["evaluation_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.evaluation_date = dtValue;
                        }
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.reeval_due_date = dtValue;
                        }
                        if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["approved_date"].ToString(), out dtValue))
                        {
                            objRiskMgmtModels.approved_date = dtValue;
                        }
                        if (dicRatings != null && dicRatings.Count > 0)
                        {
                            objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
                            objRiskMgmtModels.color_code = dicRatings.FirstOrDefault().Value;
                        }

                        sSqlstmt = "select mit_id,risk_id,measure,pers_resp,target_date from risk_mitigations_trans where risk_id_trans='" + srisk_id_trans + "'";
                        ViewBag.Mit = objGlobaldata.Getdetails(sSqlstmt);

                        return View(objRiskMgmtModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("RiskList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("RiskList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskHistoryDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("RiskList");
        }

        public JsonResult FunGetIssueDescbyIssueId(string IssueId)
        {
            try
            {
                if (IssueId != "")
                {
                    string Issue = "";
                    string sSsqlstmt = "Select Issue from t_issues where id_issue = '" + IssueId + "'";
                    DataSet dsList = objGlobaldata.Getdetails(sSsqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        Issue = dsList.Tables[0].Rows[0]["Issue"].ToString();
                    }
                    return Json(Issue);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskHistoryList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }
    }
}