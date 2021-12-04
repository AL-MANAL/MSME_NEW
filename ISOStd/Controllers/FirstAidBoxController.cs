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
    public class FirstAidBoxController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();
        private FirstAidBoxModels objFirst = new FirstAidBoxModels();

        public FirstAidBoxController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "FirstAidBox";
        }

        public ActionResult AddFirstAidBox()
        {
            return InitilizeFirstAidBox();
        }

        private ActionResult InitilizeFirstAidBox()
        {
            try
            {
                ViewBag.Location = objGlobaldata.GetDropdownList("FireEquip-Location");
                ViewBag.FirstAid = objFirst.GetFirstAid();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeFirstAid: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFirstAidBox(FirstAidBoxModels objFirstAid, FormCollection form)
        {
            try
            {
                objFirstAid.FirstAidBox_1 = form["FirstAidBox_1"];

                DateTime dateValue;

                if (DateTime.TryParse(form["FirstAid_DateTime"], out dateValue) == true)
                {
                    objFirstAid.FirstAid_DateTime = dateValue;
                }

                if (objFirstAid.FunAddFirstAid(objFirstAid))
                {
                    TempData["Successdata"] = "Added First Aid details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddFirstAid: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("FirstAidBoxList");
        }

        public ActionResult FirstAidBoxList(string branch_name)
        {
            ViewBag.SubMenutype = "FirstAidBox";

            FirstAidBoxModelsList objFirstAidList = new FirstAidBoxModelsList();
            objFirstAidList.FirstAidBoxList = new List<FirstAidBoxModels>();

            FirstAidBoxModels objType = new FirstAidBoxModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT id_FirstAidBox,FirstAid_DateTime,Location, FirstAidBox_1 from t_first_aid where Active='1'";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext;

                DataSet dsFirstAidBoxList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsFirstAidBoxList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsFirstAidBoxList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtValue;

                        try
                        {
                            FirstAidBoxModels objFirstAidBoxModels = new FirstAidBoxModels
                            {
                                id_FirstAidBox = dsFirstAidBoxList.Tables[0].Rows[i]["id_FirstAidBox"].ToString(),
                                Location = objGlobaldata.GetDropdownitemById(dsFirstAidBoxList.Tables[0].Rows[i]["Location"].ToString()),
                                FirstAidBox_1 = objType.GetFirstAidById(dsFirstAidBoxList.Tables[0].Rows[i]["FirstAidBox_1"].ToString()),
                            };
                            if (DateTime.TryParse(dsFirstAidBoxList.Tables[0].Rows[i]["FirstAid_DateTime"].ToString(), out dtValue))
                            {
                                objFirstAidBoxModels.FirstAid_DateTime = dtValue;
                            }
                            objFirstAidList.FirstAidBoxList.Add(objFirstAidBoxModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in FirstAidBoxList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FirstAidBoxList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objFirstAidList.FirstAidBoxList.ToList());
        }

        public JsonResult FirstAidBoxListSearch(string branch_name)
        {
            ViewBag.SubMenutype = "FirstAidBox";

            FirstAidBoxModelsList objFirstAidList = new FirstAidBoxModelsList();
            objFirstAidList.FirstAidBoxList = new List<FirstAidBoxModels>();

            FirstAidBoxModels objType = new FirstAidBoxModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT id_FirstAidBox,FirstAid_DateTime,Location, FirstAidBox_1 from t_first_aid where Active='1'";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext;

                DataSet dsFirstAidBoxList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsFirstAidBoxList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsFirstAidBoxList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtValue;

                        try
                        {
                            FirstAidBoxModels objFirstAidBoxModels = new FirstAidBoxModels
                            {
                                id_FirstAidBox = dsFirstAidBoxList.Tables[0].Rows[i]["id_FirstAidBox"].ToString(),
                                Location = objGlobaldata.GetDropdownitemById(dsFirstAidBoxList.Tables[0].Rows[i]["Location"].ToString()),
                                FirstAidBox_1 = objType.GetFirstAidById(dsFirstAidBoxList.Tables[0].Rows[i]["FirstAidBox_1"].ToString()),
                            };
                            if (DateTime.TryParse(dsFirstAidBoxList.Tables[0].Rows[i]["FirstAid_DateTime"].ToString(), out dtValue))
                            {
                                objFirstAidBoxModels.FirstAid_DateTime = dtValue;
                            }
                            objFirstAidList.FirstAidBoxList.Add(objFirstAidBoxModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in FirstAidBoxListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FirstAidBoxListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public JsonResult FirstAidBoxDelete(FormCollection form)
        {
            try
            {
                if (form["id_FirstAidBox"] != null && form["id_FirstAidBox"] != "")
                {
                    FirstAidBoxModels Doc = new FirstAidBoxModels();
                    string sid_FirstAidBox = form["id_FirstAidBox"];

                    if (Doc.FunDeleteFirstAidBox(sid_FirstAidBox))
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
                    TempData["alertdata"] = "First Aid Box Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FirstAidBoxDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public ActionResult FirstAidBoxEdit()
        {
            ViewBag.SubMenutype = "FirstAidBox";
            FirstAidBoxModels objComp = new FirstAidBoxModels();
            try
            {
                if (Request.QueryString["id_FirstAidBox"] != null && Request.QueryString["id_FirstAidBox"] != "")
                {
                    string sid_FirstAidBox = Request.QueryString["id_FirstAidBox"];
                    string sSqlstmt = "select id_FirstAidBox,FirstAid_DateTime,Location,FirstAidBox_1 from t_first_aid where id_FirstAidBox=" + sid_FirstAidBox;

                    DataSet dsFirstAidBoxList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsFirstAidBoxList.Tables.Count > 0 && dsFirstAidBoxList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtValue;

                        objComp = new FirstAidBoxModels
                        {
                            id_FirstAidBox = dsFirstAidBoxList.Tables[0].Rows[0]["id_FirstAidBox"].ToString(),
                            Location = objGlobaldata.GetDropdownitemById(dsFirstAidBoxList.Tables[0].Rows[0]["Location"].ToString()),
                            FirstAidBox_1 = objFirst.GetFirstAidById(dsFirstAidBoxList.Tables[0].Rows[0]["FirstAidBox_1"].ToString()),
                        };
                        if (DateTime.TryParse(dsFirstAidBoxList.Tables[0].Rows[0]["FirstAid_DateTime"].ToString(), out dtValue))
                        {
                            objComp.FirstAid_DateTime = dtValue;
                        }

                        InitilizeFirstAidBox();

                        return View(objComp);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("FirstAidBoxList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "First Aid Box Id cannot be null";
                    return RedirectToAction("FirstAidBoxList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FirstAidBoxEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("FirstAidBoxList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FirstAidBoxEdit(FirstAidBoxModels objFirstAid, FormCollection form)
        {
            try
            {
                objFirstAid.FirstAidBox_1 = form["FirstAidBox_1"];

                DateTime dateValue;

                if (form["FirstAid_DateTime"] != null && DateTime.TryParse(form["FirstAid_DateTime"], out dateValue) == true)
                {
                    objFirstAid.FirstAid_DateTime = dateValue;
                }
                if (objFirstAid.FunUpdateFirstAid(objFirstAid))
                {
                    TempData["Successdata"] = "Updated First Aid details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FirstAidEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("FirstAidBoxList");
        }

        [HttpPost]
        public JsonResult FunGetReportNo(string Location)
        {
            DataSet dsData; string RepNo = "";

            dsData = objGlobaldata.GetReportNo("FAB", "", Location);
            if (dsData != null && dsData.Tables.Count > 0)
            {
                RepNo = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
            }
            return Json(RepNo);
        }
    }
}