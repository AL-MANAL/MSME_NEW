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
    public class PartiesController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public PartiesController()
        {
            ViewBag.Menutype = "PartiesIssues";
            ViewBag.SubMenutype = "Parties";
        }

        [AllowAnonymous]
        public ActionResult AddParties()
        {
            PartiesModel objParties = new PartiesModel();
            try
            {
                ViewBag.SubMenutype = "Parties";

                objParties.branch = objGlobaldata.GetCurrentUserSession().division;
                objParties.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objParties.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.PartyName = objParties.getParties();
                ViewBag.PartyType = objGlobaldata.GetConstantValue("PartyType");
                ViewBag.ISOStds = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.Priority = objGlobaldata.GetDropdownList("Priority");
                ViewBag.Interest = objGlobaldata.GetDropdownList("Priority");

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objParties.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objParties.branch);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddParties: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objParties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddParties(PartiesModel objParties, FormCollection form)
        {
            try
            {
                objParties.PartyName = form["PartyName"];
                objParties.PartyType = form["PartyType"];
                objParties.Expectations = form["Expectations"];
                objParties.Impact = form["Impact"];
                objParties.Isostd = form["Isostd"];
                objParties.Issues = form["Issues"];
                objParties.priority = form["priority"];
                objParties.monit_mech = form["monit_mech"];
                objParties.pi_rank = form["pi_rank"];
                objParties.branch = form["branch"];
                objParties.Location = form["Location"];
                objParties.Department = form["Department"];

                if (objParties.FunAddParties(objParties))
                {
                    TempData["Successdata"] = "Added Parties details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddParties: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("PartiesList");
        }

        [AllowAnonymous]
        public ActionResult PartiesList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Parties";

            PartiesModelsList objPartiesList = new PartiesModelsList();
            objPartiesList.PartiesList = new List<PartiesModel>();
            PartiesModel objParties = new PartiesModel();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sLocation_name = objGlobaldata.GetCurrentUserSession().Work_Location;
                string sLocationtree = objGlobaldata.GetCurrentUserSession().LocationTree;
                ViewBag.Location = objGlobaldata.GetMultiLocationListByID(sLocationtree);

                string sSearchtext = "";
                string sSqlstmt = "select Ref_no,id_parties,PartyName,PartyType,Expectations,Impact," +
                    "Isostd,Issues,priority,monit_mech,pi_rank,interest_level,branch,Location,Department from t_parties where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                //if (location_name != null && location_name != "")
                //{
                //    sSearchtext = sSearchtext + " and find_in_set('" + location_name + "', Location)";
                //    ViewBag.Location_name = location_name;
                //}
                //else
                //{
                //    sSearchtext = sSearchtext + " and find_in_set('" + sLocation_name + "', Location)";
                //}
                sSqlstmt = sSqlstmt + sSearchtext + "order by id_parties desc";
                DataSet dsPartiesList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsPartiesList.Tables.Count > 0 && dsPartiesList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsPartiesList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            PartiesModel objPartiesModels = new PartiesModel
                            {
                                id_parties = Convert.ToInt16(dsPartiesList.Tables[0].Rows[i]["id_parties"].ToString()),
                                Ref_no = dsPartiesList.Tables[0].Rows[i]["Ref_no"].ToString(),
                                PartyName = objParties.GetPartyNameById(dsPartiesList.Tables[0].Rows[i]["PartyName"].ToString()),
                                PartyType = dsPartiesList.Tables[0].Rows[i]["PartyType"].ToString(),
                                Expectations = dsPartiesList.Tables[0].Rows[i]["Expectations"].ToString(),
                                Impact = dsPartiesList.Tables[0].Rows[i]["Impact"].ToString(),
                                Isostd = objGlobaldata.GetIsoStdDescriptionById(dsPartiesList.Tables[0].Rows[i]["Isostd"].ToString()),
                                Issues = dsPartiesList.Tables[0].Rows[i]["Issues"].ToString(),
                                priority = objGlobaldata.GetDropdownitemById(dsPartiesList.Tables[0].Rows[i]["priority"].ToString()),
                                monit_mech = dsPartiesList.Tables[0].Rows[i]["monit_mech"].ToString(),
                                pi_rank = (dsPartiesList.Tables[0].Rows[i]["pi_rank"].ToString()),
                                interest_level = objGlobaldata.GetDropdownitemById(dsPartiesList.Tables[0].Rows[i]["interest_level"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPartiesList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsPartiesList.Tables[0].Rows[i]["Location"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsPartiesList.Tables[0].Rows[i]["Department"].ToString()),
                            };
                            objPartiesList.PartiesList.Add(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PartiesList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PartiesList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPartiesList.PartiesList.ToList());
        }

        //[AllowAnonymous]
        //public JsonResult PartiesListSearch(FormCollection form, int? page, string branch_name)
        //{
        //    ViewBag.SubMenutype = "Parties";

        //    PartiesModelsList objPartiesList = new PartiesModelsList();
        //    objPartiesList.PartiesList = new List<PartiesModel>();
        //    PartiesModel objParties = new PartiesModel();
        //    try
        //    {
        //        string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
        //        string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
        //        ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
        //        string sSearchtext = "";

        //        string sSqlstmt = "select Ref_no,id_parties,PartyName,PartyType,Expectations,Impact," +
        //            "Isostd,Issues,priority,monit_mech,pi_rank,interest_level,branch,Location,Department from t_parties where Active=1";

        //        if (branch_name != null && branch_name != "")
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
        //            ViewBag.Branch_name = branch_name;
        //        }
        //        else
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
        //        }
        //        sSqlstmt = sSqlstmt + sSearchtext + "order by id_parties desc";
        //        DataSet dsPartiesList = objGlobaldata.Getdetails(sSqlstmt);

        //        if (dsPartiesList.Tables.Count > 0 && dsPartiesList.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsPartiesList.Tables[0].Rows.Count; i++)
        //            {
        //                try
        //                {
        //                    PartiesModel objPartiesModels = new PartiesModel
        //                    {
        //                        id_parties = Convert.ToInt16(dsPartiesList.Tables[0].Rows[i]["id_parties"].ToString()),
        //                        Ref_no = dsPartiesList.Tables[0].Rows[i]["Ref_no"].ToString(),
        //                        PartyName = objParties.GetPartyNameById(dsPartiesList.Tables[0].Rows[i]["PartyName"].ToString()),
        //                        PartyType = dsPartiesList.Tables[0].Rows[i]["PartyType"].ToString(),
        //                        Expectations = dsPartiesList.Tables[0].Rows[i]["Expectations"].ToString(),
        //                        Impact = dsPartiesList.Tables[0].Rows[i]["Impact"].ToString(),
        //                        Isostd = objGlobaldata.GetIsoStdDescriptionById(dsPartiesList.Tables[0].Rows[i]["Isostd"].ToString()),
        //                        Issues = dsPartiesList.Tables[0].Rows[i]["Issues"].ToString(),
        //                        priority = objGlobaldata.GetDropdownitemById(dsPartiesList.Tables[0].Rows[i]["priority"].ToString()),
        //                        monit_mech = dsPartiesList.Tables[0].Rows[i]["monit_mech"].ToString(),
        //                        pi_rank = (dsPartiesList.Tables[0].Rows[i]["pi_rank"].ToString()),
        //                        interest_level = objGlobaldata.GetDropdownitemById(dsPartiesList.Tables[0].Rows[i]["interest_level"].ToString()),
        //                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPartiesList.Tables[0].Rows[i]["branch"].ToString()),
        //                        Location = objGlobaldata.GetDivisionLocationById(dsPartiesList.Tables[0].Rows[i]["Location"].ToString()),
        //                        Department = objGlobaldata.GetMultiDeptNameById(dsPartiesList.Tables[0].Rows[i]["Department"].ToString()),
        //                    };

        //                    objPartiesList.PartiesList.Add(objPartiesModels);
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobaldata.AddFunctionalLog("Exception in PartiesListSearch: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in PartiesListSearch: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json("Success");
        //}

        [AllowAnonymous]
        public ActionResult PartiesInfo(int id)
        {
            ViewBag.SubMenutype = "Parties";

            PartiesModelsList objPartiesList = new PartiesModelsList();
            objPartiesList.PartiesList = new List<PartiesModel>();
            PartiesModel objParties = new PartiesModel();
            try
            {
                string sSqlstmt = "select Ref_no,id_parties,PartyName,PartyType,Expectations,Impact," +
                    "Isostd,Issues,priority,monit_mech,pi_rank,interest_level,branch,Location,Department from t_parties where Active=1"
                + " and id_parties='" + id + "' order by id_parties desc";
                DataSet dsPartiesList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsPartiesList.Tables.Count > 0 && dsPartiesList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsPartiesList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            PartiesModel objPartiesModels = new PartiesModel
                            {
                                id_parties = Convert.ToInt16(dsPartiesList.Tables[0].Rows[i]["id_parties"].ToString()),
                                Ref_no = dsPartiesList.Tables[0].Rows[i]["Ref_no"].ToString(),
                                PartyName = objParties.GetPartyNameById(dsPartiesList.Tables[0].Rows[i]["PartyName"].ToString()),
                                PartyType = dsPartiesList.Tables[0].Rows[i]["PartyType"].ToString(),
                                Expectations = dsPartiesList.Tables[0].Rows[i]["Expectations"].ToString(),
                                Impact = dsPartiesList.Tables[0].Rows[i]["Impact"].ToString(),
                                Isostd = objGlobaldata.GetIsoStdDescriptionById(dsPartiesList.Tables[0].Rows[i]["Isostd"].ToString()),
                                Issues = dsPartiesList.Tables[0].Rows[i]["Issues"].ToString(),
                                priority = objGlobaldata.GetDropdownitemById(dsPartiesList.Tables[0].Rows[i]["priority"].ToString()),
                                monit_mech = dsPartiesList.Tables[0].Rows[i]["monit_mech"].ToString(),
                                pi_rank = (dsPartiesList.Tables[0].Rows[i]["pi_rank"].ToString()),
                                interest_level = objGlobaldata.GetDropdownitemById(dsPartiesList.Tables[0].Rows[i]["interest_level"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPartiesList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsPartiesList.Tables[0].Rows[i]["Location"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsPartiesList.Tables[0].Rows[i]["Department"].ToString()),
                            };

                            objPartiesList.PartiesList.Add(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PartiesList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PartiesList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPartiesList.PartiesList.ToList());
        }

        [AllowAnonymous]
        public JsonResult PartiesDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_parties"] != null && form["id_parties"] != "")
                {
                    PartiesModel Doc = new PartiesModel();
                    string sid_parties = form["id_parties"];

                    if (Doc.FunDeletePartiesDoc(sid_parties))
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
                    TempData["alertdata"] = "Parties Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PartiesDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult PartiesEdit()
        {
            ViewBag.SubMenutype = "Parties";
            PartiesModel objPartiesModels = new PartiesModel();

            try
            {
                ViewBag.PartyName = objPartiesModels.getParties();
                ViewBag.PartyType = objGlobaldata.GetConstantValue("PartyType");
                ViewBag.ISOStds = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.Priority = objGlobaldata.GetDropdownList("Priority");
                ViewBag.PI_rank = objGlobaldata.GetDropdownList("PI Rank Value");

                if (Request.QueryString["id_parties"] != null && Request.QueryString["id_parties"] != "")
                {
                    string id_parties = Request.QueryString["id_parties"];

                    ViewBag.id_parties = id_parties;

                    string sSqlstmt = "select Ref_no,id_parties,PartyName,PartyType,Expectations,Impact,Isostd," +
                        "Issues,priority,monit_mech,pi_rank,interest_level,branch,Location,Department from t_parties where id_parties='" + id_parties + "'";

                    DataSet dsPartiesList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsPartiesList.Tables.Count > 0 && dsPartiesList.Tables[0].Rows.Count > 0)
                    {
                        objPartiesModels = new PartiesModel
                        {
                            id_parties = Convert.ToInt16(dsPartiesList.Tables[0].Rows[0]["id_parties"].ToString()),
                            PartyName = objPartiesModels.GetPartyNameById(dsPartiesList.Tables[0].Rows[0]["PartyName"].ToString()),
                            PartyType = dsPartiesList.Tables[0].Rows[0]["PartyType"].ToString(),
                            Expectations = dsPartiesList.Tables[0].Rows[0]["Expectations"].ToString(),
                            Impact = dsPartiesList.Tables[0].Rows[0]["Impact"].ToString(),
                            Isostd = objGlobaldata.GetIsoStdDescriptionById(dsPartiesList.Tables[0].Rows[0]["Isostd"].ToString()),
                            Issues = dsPartiesList.Tables[0].Rows[0]["Issues"].ToString(),
                            Ref_no = dsPartiesList.Tables[0].Rows[0]["Ref_no"].ToString(),
                            priority = objGlobaldata.GetDropdownitemById(dsPartiesList.Tables[0].Rows[0]["priority"].ToString()),
                            interest_level = objGlobaldata.GetDropdownitemById(dsPartiesList.Tables[0].Rows[0]["interest_level"].ToString()),
                            monit_mech = dsPartiesList.Tables[0].Rows[0]["monit_mech"].ToString(),
                            pi_rank = (dsPartiesList.Tables[0].Rows[0]["pi_rank"].ToString()),
                            branch = (dsPartiesList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = (dsPartiesList.Tables[0].Rows[0]["Location"].ToString()),
                            Department = (dsPartiesList.Tables[0].Rows[0]["Department"].ToString()),
                        };
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsPartiesList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsPartiesList.Tables[0].Rows[0]["branch"].ToString());
                    }
                    else
                    {
                        TempData["alertdata"] = "PartiesID cannot be Null or empty";
                        return RedirectToAction("PartiesList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "PartiesID cannot be Null or empty";
                    return RedirectToAction("PartiesList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PartiesEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("PartiesList");
            }
            return View(objPartiesModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PartiesEdit(PartiesModel objParties, FormCollection form)
        {
            try
            {
                objParties.PartyName = form["PartyName"];
                objParties.PartyType = form["PartyType"];
                objParties.Expectations = form["Expectations"];
                objParties.Impact = form["Impact"];
                objParties.Isostd = form["Isostd"];
                objParties.Issues = form["Issues"];
                objParties.priority = form["priority"];
                objParties.monit_mech = form["monit_mech"];
                objParties.pi_rank = form["pi_rank"];
                objParties.branch = form["branch"];
                objParties.Location = form["Location"];
                objParties.Department = form["Department"];

                if (objParties.FunUpdateParties(objParties))
                {
                    TempData["Successdata"] = "Parties details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PartiesEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("PartiesList");
            }

            return RedirectToAction("PartiesList");
        }

        [HttpPost]
        public JsonResult doesPartiesRefNoExist(string Ref_no)
        {
            IssuesModels objIssueDocuments = new IssuesModels();
            var user = objIssueDocuments.CheckForPartiesRefNoExists(Ref_no);

            return Json(user);
        }

        public JsonResult funGetPIRank(string priority, string interest)
        {
            string PI = "";
            if (priority != "" && interest != "")
            {
                string sqlstmt = "select pi_value from t_pi_rank where priority='" + priority + "' and interest='" + interest + "' ";
                DataSet dsData = objGlobaldata.Getdetails(sqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    PI = dsData.Tables[0].Rows[0]["pi_value"].ToString();
                }
            }
            return Json(PI);
        }
    }
}