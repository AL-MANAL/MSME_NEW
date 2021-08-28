using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.IO;
using PagedList;
using PagedList.Mvc;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class RiskRegisterActivityController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public RiskRegisterActivityController()
        {
            ViewBag.Menutype = "Risk";
            ViewBag.SubMenutype = "RiskRegisterActivity";
        }
       
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AddRiskRegisterActivity()
        {
            RiskRegisterActivityModels obj = new RiskRegisterActivityModels();
            try
            {
                obj.branch = objGlobaldata.GetCurrentUserSession().division;
                obj.DeptId = objGlobaldata.GetCurrentUserSession().DeptID;
                obj.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(obj.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(obj.branch);
                ViewBag.Activity_Category = objGlobaldata.GetDropdownList("RiskRegister-Activity Category");
                ViewBag.Risk_Type = objGlobaldata.GetDropdownList("RiskRegister-RiskType");
                ViewBag.Activity_Status = objGlobaldata.GetDropdownList("RiskRegister-Activity Status");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskRegisterActivity: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(obj);
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRiskRegisterActivity(RiskRegisterActivityModels objRiskRegisterActivity, FormCollection form)
        {
            try
            {
                objRiskRegisterActivity.branch = form["branch"];
                objRiskRegisterActivity.DeptId = form["DeptId"];
                objRiskRegisterActivity.Location = form["Location"];

                if (objRiskRegisterActivity.FunAddRiskRegisterActivity(objRiskRegisterActivity))
                {
                    TempData["Successdata"] = "Added Risk Register Activity details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskRegisterActivity: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterActivityList");
        }
        
        [AllowAnonymous]
        public JsonResult RiskRegisterActivityDocDelete(FormCollection form)
        {
            try
            {
                  if (form["Risk_Reg_Activity_Id"] != null && form["Risk_Reg_Activity_Id"] != "")
                    {
                        RiskRegisterActivityModels Doc = new RiskRegisterActivityModels();
                        string sRisk_Reg_Activity_Id = form["Risk_Reg_Activity_Id"];
                    
                        if (Doc.FunDeleteRiskActivityDoc(sRisk_Reg_Activity_Id))
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
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        
        [AllowAnonymous]
        public JsonResult RiskRegisterHRRDelete(FormCollection form)
        {
            try
            {               
                    if (form["risk_hrr_id"] != null && form["risk_hrr_id"] != "")
                    {

                        RiskRegisterActivityModels Doc = new RiskRegisterActivityModels();
                        string srisk_hrr_id = form["risk_hrr_id"];


                        if (Doc.FunDeleteRiskActivityHRRDoc(srisk_hrr_id))
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
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterEvalDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult RiskRegisterEvlDelete(FormCollection form)
        {
            try
            {
                
                    if (form["Reg_Activity_eval_Id"] != null && form["Reg_Activity_eval_Id"] != "")
                    {

                        RiskRegisterActivityModels Doc = new RiskRegisterActivityModels();
                        string sReg_Activity_eval_Id = form["Reg_Activity_eval_Id"];


                        if (Doc.FunDeleteRiskActivityEvlDoc(sReg_Activity_eval_Id))
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
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterEvlDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        //
        // GET: /RiskRegisterActivity/RiskRegisterActivityList

        public ActionResult RiskRegisterActivityHistoryList(string SearchText, int? page, string Risk_Reg_Activity_Id)
        {

            RiskRegisterActivityModelsList objRiskRegisterActivityList = new RiskRegisterActivityModelsList();
            objRiskRegisterActivityList.lstRiskRegisterActivity = new List<RiskRegisterActivityModels>();
            RiskRegisterActivityModels objreg = new RiskRegisterActivityModels();
            ViewBag.Risk_Reg_Activity_Id = Risk_Reg_Activity_Id;

            try
            {
                string sSqlstmt = "select Risk_Reg_Activity_Id_trans,Risk_Reg_Activity_Id, DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, LoggedBy from t_risk_register_activity_trans  where Active=1";

                if (SearchText != null && SearchText != "")
                {
                    sSqlstmt = sSqlstmt + " and (Activity like '" + SearchText + "%' or Activity like '%" + SearchText + "%' or Activity like '%" + SearchText + "')";
                }

                sSqlstmt = sSqlstmt + " and (Risk_Reg_Activity_Id ='" + Risk_Reg_Activity_Id + "')";
                sSqlstmt = sSqlstmt + " order by DeptId";

                DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRiskRegisterActivityList.Tables.Count > 0)
                {

                    
                    for (int i = 0; i < dsRiskRegisterActivityList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            RiskRegisterActivityModels objRiskRegisterActivity = new RiskRegisterActivityModels
                            {
                                Risk_Reg_Activity_Id_trans = dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id_trans"].ToString(),
                                Risk_Reg_Activity_Id = dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id"].ToString(),
                                DeptId = objGlobaldata.GetDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["DeptId"].ToString()),
                                Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_No"].ToString(),
                                Activity = dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity"].ToString(),
                                Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_Category"].ToString()),
                                Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Type"].ToString()),
                                Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_Status"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["LoggedBy"].ToString())
                            };

                            objRiskRegisterActivityList.lstRiskRegisterActivity.Add(objRiskRegisterActivity);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityHistoryList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objRiskRegisterActivityList.lstRiskRegisterActivity.ToList());
        }

        [AllowAnonymous]
        public ActionResult RiskRegisterActivityList(string SearchText, int? page, string branch_name)
        {
            RiskRegisterActivityModelsList objRiskRegisterActivityList = new RiskRegisterActivityModelsList();
            objRiskRegisterActivityList.lstRiskRegisterActivity = new List<RiskRegisterActivityModels>();
            RiskRegisterActivityModels objreg = new RiskRegisterActivityModels();
           
            try
            {
                ViewBag.View = Request.QueryString["View"];

                String RiskType = objreg.GetRiskTypeIdbyName("Environmental");
                String RiskType1 = objreg.GetRiskTypeIdbyName("OH & S");

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select Risk_Reg_Activity_Id, DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, LoggedBy,branch,Location" +
                    " from t_risk_register_activity  where Active=1";

                if (SearchText != null && SearchText != "")
                {
                    sSqlstmt = sSqlstmt + " and (Activity like '" + SearchText + "%' or Activity like '%" + SearchText + "%' or Activity like '%" + SearchText + "')";
                }

                if (Request.QueryString["View"] == "2")
                {
                    Session["state"] = Request.QueryString["View"];

                    sSqlstmt = sSqlstmt + " and Risk_Type =  '" + RiskType + "'";
                }
                else if (Request.QueryString["View"] == "3")
                {
                    Session["state"] = Request.QueryString["View"];

                    sSqlstmt = sSqlstmt + " and Risk_Type = '" + RiskType1 + "'";
                }
                else if (Request.QueryString["View"] == "1")
                {
                    Session["state"] = Request.QueryString["View"];

                    sSqlstmt = sSqlstmt + "";
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

                sSqlstmt = sSqlstmt + sSearchtext + " order by DeptId";

                DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRiskRegisterActivityList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsRiskRegisterActivityList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            RiskRegisterActivityModels objRiskRegisterActivity = new RiskRegisterActivityModels
                            {
                                Risk_Reg_Activity_Id = dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id"].ToString(),
                                DeptId = objGlobaldata.GetMultiDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["DeptId"].ToString()),
                                Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_No"].ToString(),
                                Activity = dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity"].ToString(),
                                Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_Category"].ToString()),
                                Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Type"].ToString()),
                                Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_Status"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            objRiskRegisterActivityList.lstRiskRegisterActivity.Add(objRiskRegisterActivity);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objRiskRegisterActivityList.lstRiskRegisterActivity.ToList());
        }

        [AllowAnonymous]
        public JsonResult RiskRegisterActivityListSearch(string SearchText, int? page, string branch_name)
        {
            RiskRegisterActivityModelsList objRiskRegisterActivityList = new RiskRegisterActivityModelsList();
            objRiskRegisterActivityList.lstRiskRegisterActivity = new List<RiskRegisterActivityModels>();
            RiskRegisterActivityModels objreg = new RiskRegisterActivityModels();
            
            try
            {
                ViewBag.View = Request.QueryString["View"];

                String RiskType = objreg.GetRiskTypeIdbyName("Environmental");
                String RiskType1 = objreg.GetRiskTypeIdbyName("OH & S");

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select Risk_Reg_Activity_Id, DeptId, Activity_No, Activity, Activity_Category, Risk_Type, " +
                    "Activity_Status, LoggedBy,branch,Location from t_risk_register_activity  where Active=1";

                if (SearchText != null && SearchText != "")
                {
                    sSqlstmt = sSqlstmt + " and (Activity like '" + SearchText + "%' or Activity like '%" + SearchText + "%' or Activity like '%" + SearchText + "')";
                }

                if (Request.QueryString["View"] == "2")
                {
                    Session["state"] = Request.QueryString["View"];

                    sSqlstmt = sSqlstmt + " and Risk_Type =  '" + RiskType + "'";
                }
                else if (Request.QueryString["View"] == "3")
                {
                    Session["state"] = Request.QueryString["View"];

                    sSqlstmt = sSqlstmt + " and Risk_Type = '" + RiskType1 + "'";
                }
                else if (Request.QueryString["View"] == "1")
                {
                    Session["state"] = Request.QueryString["View"];

                    sSqlstmt = sSqlstmt + "";
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

                sSqlstmt = sSqlstmt + sSearchtext + " order by DeptId";

                DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRiskRegisterActivityList.Tables.Count > 0)
                { 
                    for (int i = 0; i < dsRiskRegisterActivityList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            RiskRegisterActivityModels objRiskRegisterActivity = new RiskRegisterActivityModels
                            {
                                Risk_Reg_Activity_Id = dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id"].ToString(),
                                DeptId = objGlobaldata.GetMultiDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["DeptId"].ToString()),
                                Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_No"].ToString(),
                                Activity = dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity"].ToString(),
                                Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_Category"].ToString()),
                                Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Type"].ToString()),
                                Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_Status"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            objRiskRegisterActivityList.lstRiskRegisterActivity.Add(objRiskRegisterActivity);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        //[AllowAnonymous]
        //public ActionResult RiskRegisterActivityListSearch(string SearchText, int? page)
        //{
        //    RiskRegisterActivityModelsList objRiskRegisterActivityList = new RiskRegisterActivityModelsList();
        //    objRiskRegisterActivityList.lstRiskRegisterActivity = new List<RiskRegisterActivityModels>();
        //    RiskRegisterActivityModels objreg = new RiskRegisterActivityModels();

        //    ViewBag.View = Request.QueryString["View"];

        //    String RiskType = objreg.GetRiskTypeIdbyName("Environmental");
        //    String RiskType1 = objreg.GetRiskTypeIdbyName("OH & S");

        //    try
        //    {
        //        string sSqlstmt = "select Risk_Reg_Activity_Id, DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, LoggedBy from t_risk_register_activity  where Active=1";

        //        if (SearchText != null && SearchText != "")
        //        {
        //            sSqlstmt = sSqlstmt + " and (Activity like '" + SearchText + "%' or Activity like '%" + SearchText + "%' or Activity like '%" + SearchText + "')";
        //        }


        //        if (Session["state"].ToString() == "2")
        //        {
        //            sSqlstmt = sSqlstmt + " and Risk_Type =  '" + RiskType + "'";
        //        }

        //        else if (Session["state"].ToString() == "3")
        //        {
        //            sSqlstmt = sSqlstmt + " and Risk_Type = '" + RiskType1 + "'";
        //        }

        //        else if (Session["state"].ToString() == "1")
        //        {
        //            sSqlstmt = sSqlstmt + "";
        //        }
        //        sSqlstmt = sSqlstmt + " order by DeptId";

        //        DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);
        //        if (dsRiskRegisterActivityList.Tables.Count > 0)
        //        {

        //            

        //            for (int i = 0; i < dsRiskRegisterActivityList.Tables[0].Rows.Count; i++)
        //            {
        //                try
        //                {
        //                    RiskRegisterActivityModels objRiskRegisterActivity = new RiskRegisterActivityModels
        //                    {
        //                        Risk_Reg_Activity_Id = dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id"].ToString(),
        //                        DeptId = objGlobaldata.GetDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["DeptId"].ToString()),
        //                        Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_No"].ToString(),
        //                        Activity = dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity"].ToString(),
        //                        Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_Category"].ToString()),
        //                        Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Type"].ToString()),
        //                        Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_Status"].ToString()),
        //                        LoggedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["LoggedBy"].ToString())
        //                    };

        //                    objRiskRegisterActivityList.lstRiskRegisterActivity.Add(objRiskRegisterActivity);
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityListSearch: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityListSearch: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return View(objRiskRegisterActivityList.lstRiskRegisterActivity.ToList().ToPagedList(page ?? 1, 40));
        //}



        [AllowAnonymous]
        public ActionResult RiskRegisterActivityInfo(int id)
        {
            RiskRegisterActivityModelsList objRiskRegisterActivityList = new RiskRegisterActivityModelsList();
            objRiskRegisterActivityList.lstRiskRegisterActivity = new List<RiskRegisterActivityModels>();
            RiskRegisterActivityModels objreg = new RiskRegisterActivityModels();
            try
            {
                string sSqlstmt = "select Risk_Reg_Activity_Id, DeptId, Activity_No, Activity, Activity_Category, Risk_Type,"
                + " Activity_Status, LoggedBy,branch,Location from t_risk_register_activity where Active=1 and Risk_Reg_Activity_Id='" + id + "'";

                DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRiskRegisterActivityList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsRiskRegisterActivityList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            RiskRegisterActivityModels objRiskRegisterActivity = new RiskRegisterActivityModels
                            {
                                Risk_Reg_Activity_Id = dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id"].ToString(),
                                DeptId = objGlobaldata.GetMultiDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["DeptId"].ToString()),
                                Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_No"].ToString(),
                                Activity = dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity"].ToString(),
                                Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_Category"].ToString()),
                                Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Type"].ToString()),
                                Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Activity_Status"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            objRiskRegisterActivityList.lstRiskRegisterActivity.Add(objRiskRegisterActivity);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objRiskRegisterActivityList.lstRiskRegisterActivity.ToList());
        }
        
        public ActionResult EnvironmentalMatrix()
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRiskRegister = new RiskRegisterActivityEvaluationModels();
                //matrix array 
                string[,] SevArray = new string[10, 10];
                ViewBag.MatrixProbability = objGlobaldata.GetDropdownList("ENV-Probability");
                ViewBag.MatrixSeverity = objGlobaldata.GetDropdownList("ENV-Severity");

                List<int> array1 = new List<int>();
                List<int> array2 = new List<int>();
                int[,] result = new int[10, 10];
                string sqlstmt = "select item_id as Id, item_fulldesc as severity from dropdownitems, dropdownheader"
                + " where dropdownheader.header_id=dropdownitems.header_id and header_desc='ENV-Severity' order by severity desc";
                DataSet dsSeverity = objGlobaldata.Getdetails(sqlstmt);
                if (dsSeverity.Tables.Count > 0 && dsSeverity.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsSeverity.Tables[0].Rows.Count; i++)
                    {
                        array1.Add(Convert.ToInt32(dsSeverity.Tables[0].Rows[i]["severity"].ToString()));
                    }
                }

                string sqlstmts = "select item_id as Id, item_fulldesc as probability from dropdownitems, dropdownheader"
                + " where dropdownheader.header_id=dropdownitems.header_id and header_desc='ENV-Probability' order by probability asc";
                DataSet dsProbability = objGlobaldata.Getdetails(sqlstmts);
                if (dsProbability.Tables.Count > 0 && dsProbability.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsProbability.Tables[0].Rows.Count; i++)
                    {
                        array2.Add(Convert.ToInt32(dsProbability.Tables[0].Rows[i]["probability"].ToString()));
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
                    objRiskRegister.GetEVNMatrixColordetails();
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

                string sql = "select from_value,to_value,rate_desc from risk_ratings_env";
                DataSet dsRating = objGlobaldata.Getdetails(sql);
                ViewBag.dsRating = dsRating;

                ViewBag.dsColor = objGlobaldata.GetEnvMatrixRatewithColor();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetEnvironmentalMatrix: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
        
        public ActionResult HRRMatrix()
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRiskRegister = new RiskRegisterActivityEvaluationModels();
                //matrix array 
                string[,] SevArray = new string[10, 10];
                ViewBag.MatrixProbability = objGlobaldata.GetDropdownList("HRR-Probability");
                ViewBag.MatrixSeverity = objRiskRegister.GetMultiHRRRiskSeveirtyForMatrix();
                ViewBag.MatrixExposure = objRiskRegister.GetMultiHRRRiskExposureForMatrix();
                ViewBag.MatrixSev_id = objGlobaldata.GetDropdownList("HRR-Severity");

                List<int> array1 = new List<int>();
                List<int> array2 = new List<int>();
                List<int> array3 = new List<int>();
                int[,] result = new int[10, 10];

                string qlstmt = "select item_id as Id, item_fulldesc as Exposure from dropdownitems, dropdownheader"
            + " where dropdownheader.header_id=dropdownitems.header_id and header_desc='Risk-Exposure' order by Exposure desc";
                DataSet dsExp = objGlobaldata.Getdetails(qlstmt);
                if (dsExp.Tables.Count > 0 && dsExp.Tables[0].Rows.Count > 0)
                {
                    string sqlstmt = "select item_id as Id, item_fulldesc as severity from dropdownitems, dropdownheader"
                    + " where dropdownheader.header_id=dropdownitems.header_id and header_desc='HRR-Severity' order by severity desc";
                    DataSet dsSeverity = objGlobaldata.Getdetails(sqlstmt);
                    if (dsSeverity.Tables.Count > 0 && dsSeverity.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsSeverity.Tables[0].Rows.Count; i++)
                        {
                            array1.Add(Convert.ToInt32(dsSeverity.Tables[0].Rows[i]["severity"].ToString()));
                        }
                    }

                    string sqlstmts = "select item_id as Id, item_fulldesc as probability from dropdownitems, dropdownheader"
                    + " where dropdownheader.header_id=dropdownitems.header_id and header_desc='HRR-Probability' order by probability asc";
                    DataSet dsProbability = objGlobaldata.Getdetails(sqlstmts);
                    if (dsProbability.Tables.Count > 0 && dsProbability.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsProbability.Tables[0].Rows.Count; i++)
                        {
                            array2.Add(Convert.ToInt32(dsProbability.Tables[0].Rows[i]["probability"].ToString()));
                        }
                    }

                    string ssqlstmt = "select item_id as Id, item_fulldesc as Exposure from dropdownitems, dropdownheader"
                 + " where dropdownheader.header_id=dropdownitems.header_id and header_desc='Risk-Exposure' order by Exposure desc";
                    DataSet dsExposure = objGlobaldata.Getdetails(ssqlstmt);
                    if (dsExposure.Tables.Count > 0 && dsExposure.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsExposure.Tables[0].Rows.Count; i++)
                        {
                            array3.Add(Convert.ToInt32(dsExposure.Tables[0].Rows[i]["Exposure"].ToString()));
                        }
                    }
                    //multiplication of matrix
                    for (int i = 0; i < array1.Count; i++)
                    {
                        for (int j = 0; j < array2.Count; j++)
                        {
                            result[i, j] = array1[i] * array2[j] * array3[i];
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
                        objRiskRegister.GetHRRMatrixColordetails();
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
                }
                else
                {

                    string sqlstmt = "select item_id as Id, item_fulldesc as severity from dropdownitems, dropdownheader"
                       + " where dropdownheader.header_id=dropdownitems.header_id and header_desc='HRR-Severity' order by severity desc";
                    DataSet dsSeverity = objGlobaldata.Getdetails(sqlstmt);
                    if (dsSeverity.Tables.Count > 0 && dsSeverity.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsSeverity.Tables[0].Rows.Count; i++)
                        {
                            array1.Add(Convert.ToInt32(dsSeverity.Tables[0].Rows[i]["severity"].ToString()));
                        }
                    }

                    string sqlstmts = "select item_id as Id, item_fulldesc as probability from dropdownitems, dropdownheader"
                    + " where dropdownheader.header_id=dropdownitems.header_id and header_desc='HRR-Probability' order by probability asc";
                    DataSet dsProbability = objGlobaldata.Getdetails(sqlstmts);
                    if (dsProbability.Tables.Count > 0 && dsProbability.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsProbability.Tables[0].Rows.Count; i++)
                        {
                            array2.Add(Convert.ToInt32(dsProbability.Tables[0].Rows[i]["probability"].ToString()));
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
                    ViewBag.Matrix1 = result;


                    string sqlstmtss1;
                    string sqlstmtss;
                    int count = 0;
                    DataSet dsMatrixcolor;

                    sqlstmtss1 = "delete from rmatrix ";
                    objGlobaldata.Getdetails(sqlstmtss1);

                    foreach (var item in ViewBag.Matrix1)
                    {
                        count++;
                        sqlstmtss = "insert into rmatrix (id,matvalue) values ( '" + count + "','" + item + "')";
                        dsMatrixcolor = objGlobaldata.Getdetails(sqlstmtss);
                        objRiskRegister.GetHRRMatrixColordetails();
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

                }
                string sql = "select from_value,to_value,rate_desc from risk_ratings_hrr";
                DataSet dsRating = objGlobaldata.Getdetails(sql);
                ViewBag.dsRating = dsRating;

                ViewBag.dsColor = objGlobaldata.GetHRRMatrixRatewithColor();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HRRMatrix: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
        //
        // GET: /RiskRegisterActivity/RiskRegisterActivityDetails

        [AllowAnonymous]
        public ActionResult RiskRegisterActivityDetails()
        {
            try
            {
                RiskRegisterActivityModels objrisk = new RiskRegisterActivityModels();
                if (Request.QueryString["Risk_Reg_Activity_Id"] != null && Request.QueryString["Risk_Reg_Activity_Id"] != "")
                {
                    string sRisk_Reg_Eval_Id = Request.QueryString["Risk_Reg_Activity_Id"];
                    string sSqlstmt = "select Risk_Reg_Activity_Id, DeptId, Activity_No, Activity, Activity_Category, Risk_Type, " +
                        "Activity_Status, LoggedBy,Location,branch from "
                       + "t_risk_register_activity where Risk_Reg_Activity_Id='" + sRisk_Reg_Eval_Id + "'";

                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsRiskRegisterActivityList.Tables.Count > 0)
                    {
                        RiskRegisterActivityModels objRiskRegisterActivity = new RiskRegisterActivityModels
                        {
                            Risk_Reg_Activity_Id = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString(),
                            DeptId = objGlobaldata.GetMultiDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_No"].ToString(),
                            Activity = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity"].ToString(),
                            Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Category"].ToString()),
                            Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Type"].ToString()),
                            Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Status"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        return View(objRiskRegisterActivity);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Risk Activity data exists";
                        return RedirectToAction("RiskRegisterActivityList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Risk Activity Id cannot be null";
                    return RedirectToAction("RiskRegisterActivityList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("RiskRegisterActivityList");
        }

        public ActionResult RiskRegisterActivityHistoryDetails()
        {
            try
            {
                RiskRegisterActivityModels objrisk = new RiskRegisterActivityModels();
                if (Request.QueryString["Risk_Reg_Activity_Id_trans"] != null && Request.QueryString["Risk_Reg_Activity_Id_trans"] != "")
                {
                    string sRisk_Reg_Activity_Id_trans = Request.QueryString["Risk_Reg_Activity_Id_trans"];
                    string sSqlstmt = "select Risk_Reg_Activity_Id,Risk_Reg_Activity_Id_trans, DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, LoggedBy from "
                       + "t_risk_register_activity_trans where Risk_Reg_Activity_Id_trans ='" + sRisk_Reg_Activity_Id_trans + "'";

                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsRiskRegisterActivityList.Tables.Count > 0)
                    {

                        RiskRegisterActivityModels objRiskRegisterActivity = new RiskRegisterActivityModels
                        {
                            Risk_Reg_Activity_Id_trans = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id_trans"].ToString(),
                            Risk_Reg_Activity_Id = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString(),
                            DeptId = objGlobaldata.GetDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_No"].ToString(),
                            Activity = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity"].ToString(),
                            Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Category"].ToString()),
                            Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Type"].ToString()),
                            Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Status"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["LoggedBy"].ToString())
                        };

                        return View(objRiskRegisterActivity);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Risk Activity data exists";
                        return RedirectToAction("RiskRegisterActivityList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Risk Activity Id cannot be null";
                    return RedirectToAction("RiskRegisterActivityList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterActivityList");
        }
        
        [AllowAnonymous]
        public ActionResult RiskRegisterActivityEdit()
        {
            try
            {
                RiskRegisterActivityModels objrisk = new RiskRegisterActivityModels();
                if (Request.QueryString["Risk_Reg_Activity_Id"] != null && Request.QueryString["Risk_Reg_Activity_Id"] != "")
                {
                    string sRisk_Reg_Eval_Id = Request.QueryString["Risk_Reg_Activity_Id"];
                    string sSqlstmt = "select Risk_Reg_Activity_Id, DeptId, Activity_No, Activity, Activity_Category, Risk_Type," +
                        " Activity_Status, LoggedBy,branch,Location from "
                        + "t_risk_register_activity where Risk_Reg_Activity_Id='" + sRisk_Reg_Eval_Id + "'";

                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsRiskRegisterActivityList.Tables.Count > 0)
                    {
                        RiskRegisterActivityModels objRiskRegisterActivity = new RiskRegisterActivityModels
                        {
                            Risk_Reg_Activity_Id = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString(),
                            DeptId = (dsRiskRegisterActivityList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_No"].ToString(),
                            Activity = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Category"].ToString()),
                            Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Type"].ToString()),
                            Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Status"].ToString()),
                            branch = (dsRiskRegisterActivityList.Tables[0].Rows[0]["branch"].ToString()),
                            Location = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsRiskRegisterActivityList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsRiskRegisterActivityList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Activity_Category = objGlobaldata.GetDropdownList("RiskRegister-Activity Category");
                        ViewBag.Risk_Type = objGlobaldata.GetDropdownList("RiskRegister-RiskType");
                        ViewBag.Activity_Status = objGlobaldata.GetDropdownList("RiskRegister-Activity Status");

                        return View(objRiskRegisterActivity);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Risk Activity data exists";
                        return RedirectToAction("RiskRegisterActivityList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Risk Activity Id cannot be null";
                    return RedirectToAction("RiskRegisterActivityList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterActivityList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RiskRegisterActivityEdit(RiskRegisterActivityModels objRiskRegisterActivity, FormCollection form)
        {
            try
            {
                objRiskRegisterActivity.branch = form["branch"];
                objRiskRegisterActivity.DeptId = form["DeptId"];
                objRiskRegisterActivity.Location = form["Location"];

                if (objRiskRegisterActivity.FunUpdateRiskRegisterActivity(objRiskRegisterActivity))
                {
                    TempData["Successdata"] = "Updated Risk Register Activity details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterActivityList");
        }

        [AllowAnonymous]
        public ActionResult AddRiskRegisterActivityEvaluation()
        {
            try
            {
                if (Request.QueryString["Risk_Reg_Activity_Id"] != null && Request.QueryString["Risk_Reg_Activity_Id"] != "")
                {
                    string sRisk_Reg_Eval_Id = Request.QueryString["Risk_Reg_Activity_Id"];
                    string sSqlstmt = "select Risk_Reg_Activity_Id, DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, LoggedBy from "
                        + "t_risk_register_activity where Risk_Reg_Activity_Id='" + sRisk_Reg_Eval_Id + "'";

                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                    {
                        RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

                        RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels();

                        objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                        {
                            Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(objRiskRegisterActivityEval.Risk_Reg_Activity_Id),
                            DeptId = objGlobaldata.GetDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_No"].ToString(),
                            Activity = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity"].ToString(),
                            Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Category"].ToString()),
                            Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Type"].ToString()),
                            Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Status"].ToString()),
                        };
                        ViewBag.ActivityId = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString();
                        ViewBag.Exposure = objGlobaldata.GetDropdownList("Risk-Exposure");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.Employee = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.QHSEEmp = objGlobaldata.GetHrQHSEEmployeeListbox();
                        ViewBag.RiskProbability = objGlobaldata.GetDropdownList("ENV-Probability");
                        ViewBag.RiskSeverity = objGlobaldata.GetDropdownList("ENV-Severity");
                        ViewBag.OperationalType = objGlobaldata.GetDropdownList("RiskRegister-Operational Control");
                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.objRiskRegisterActivity = objRiskRegisterActivityEval;
                        ViewBag.ApplLaw = objGlobaldata.GetLawNo();

                        return View(objRiskRegisterActivityEval);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Risk Activity data exists";
                        return RedirectToAction("RiskRegisterActivityList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Risk Activity Id cannot be null";
                    return RedirectToAction("RiskRegisterActivityList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskRegisterActivityEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterActivityEvaluationList");
        }

        //
        // POST: /RiskRegisterActivity/AddRiskRegisterActivityEvaluation

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRiskRegisterActivityEvaluation(RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval, FormCollection form)
        {
            try
            {
                objRiskRegisterActivityEval.EvalBy = form["EvalBy"];
                objRiskRegisterActivityEval.Reviewer_QHSE = form["Reviewer_QHSE"];

                DateTime dateValue;

                if (form["Eval_Date"] != null && DateTime.TryParse(form["Eval_Date"], out dateValue) == true)
                {
                    objRiskRegisterActivityEval.Eval_Date = dateValue;
                }

                if (form["Due_Date"] != null && DateTime.TryParse(form["Due_Date"], out dateValue) == true)
                {
                    objRiskRegisterActivityEval.Due_Date = dateValue;
                }

                if (form["ReEval_Date"] != null && DateTime.TryParse(form["ReEval_Date"], out dateValue) == true)
                {
                    objRiskRegisterActivityEval.ReEval_Date = dateValue;
                }

                if (objRiskRegisterActivityEval.FunAddRiskRegisterActivityEval(objRiskRegisterActivityEval))
                {
                    TempData["Successdata"] = "Added Risk Register Activity Evaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskRegisterActivityEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterActivityEvaluationList", new { Risk_Reg_Activity_Id = objRiskRegisterActivityEval.Risk_Reg_Activity_Id });
        }


        [AllowAnonymous]
        public ActionResult AddHRRActivityEvaluation()
        {
            try
            {
                if (Request.QueryString["Risk_Reg_Activity_Id"] != null && Request.QueryString["Risk_Reg_Activity_Id"] != "")
                {
                    string sRisk_Reg_Eval_Id = Request.QueryString["Risk_Reg_Activity_Id"];
                    string sSqlstmt = "select Risk_Reg_Activity_Id, DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, LoggedBy from "
                        + "t_risk_register_activity where Risk_Reg_Activity_Id='" + sRisk_Reg_Eval_Id + "'";

                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                    {
                        RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

                        RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels();

                        objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                        {
                            Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(objRiskRegisterActivityEval.Risk_Reg_Activity_Id),
                            DeptId = objGlobaldata.GetDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_No"].ToString(),
                            Activity = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity"].ToString(),
                            Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Category"].ToString()),
                            Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Type"].ToString()),
                            Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Status"].ToString()),
                        };
                        ViewBag.Employee = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.ActivityId = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString();
                        ViewBag.EvalStatus = objGlobaldata.GetConstantValue("EvaluationStatus");
                        ViewBag.Exposure = objGlobaldata.GetDropdownList("Risk-Exposure");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.RiskProbability = objGlobaldata.GetDropdownitemById("HRR-Probability");
                        ViewBag.RiskSeverity = objGlobaldata.GetDropdownitemById("HRR-Severity");
                        ViewBag.objRiskRegisterActivity = objRiskRegisterActivityEval;
                        return View(objRiskRegisterActivityEval);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Risk Activity data exists";
                        return RedirectToAction("RiskRegisterActivityList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Risk Activity Id cannot be null";
                    return RedirectToAction("RiskRegisterActivityList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHRRActivityEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterActivityList");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHRRActivityEvaluation(RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval, FormCollection form)
        {
            try
            {

                DateTime dateValue;

                if (form["Eval_Date"] != null && DateTime.TryParse(form["Eval_Date"], out dateValue) == true)
                {
                    objRiskRegisterActivityEval.Eval_Date = dateValue;
                }

                if (form["ReEval_Date"] != null && DateTime.TryParse(form["ReEval_Date"], out dateValue) == true)
                {
                    objRiskRegisterActivityEval.ReEval_Date = dateValue;
                }

                if (objRiskRegisterActivityEval.FunAddRiskRegisterHRREval(objRiskRegisterActivityEval))
                {
                    TempData["Successdata"] = "Added Risk Register Activity Evaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHRRActivityEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterHRREvaluationList", new { Risk_Reg_Activity_Id = objRiskRegisterActivityEval.Risk_Reg_Activity_Id });
        }


        [AllowAnonymous]
        public ActionResult RiskRegisterHRREvaluationList(string SearchText, int? page)
        {
            RiskRegisterActivityEvaluationModelsList objRiskRegisterActivityEvalList = new RiskRegisterActivityEvaluationModelsList();
            objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval = new List<RiskRegisterActivityEvaluationModels>();

            RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

            try
            {
                if (Request.QueryString["Risk_Reg_Activity_Id"] != null && Request.QueryString["Risk_Reg_Activity_Id"] != "")
                {
                    string sRisk_Reg_Activity_Id = Request.QueryString["Risk_Reg_Activity_Id"];

                    ViewBag.Risk_Reg_Activity_Id = sRisk_Reg_Activity_Id;

                    string sSqlstmt = "select risk_hrr_id,Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id,Evaluation_status,"
                    + "Cur_Opt_Ctrl,Person_resp,Eval_Date,ReEval_Date,Action_TakenBy,Reviewer_QHSE,ApprovedBy from t_risk_register_hrrevaluation tt"
                    + " where Risk_Reg_Activity_Id='" + sRisk_Reg_Activity_Id + "' and tt.Active=1 ";


                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);

                    RiskRegisterActivityEvaluationModels objRiskActivityEval = new RiskRegisterActivityEvaluationModels();

                    if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                    {
                       
                        for (int i = 0; i < dsRiskRegisterActivityList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                                {
                                    risk_hrr_id = (dsRiskRegisterActivityList.Tables[0].Rows[i]["risk_hrr_id"].ToString()),
                                    Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id"].ToString()),
                                    Risk_Id = dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id"].ToString(),
                                    hazard = (dsRiskRegisterActivityList.Tables[0].Rows[i]["hazard"].ToString()),
                                    Severity = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Severity"].ToString()),
                                    Probability = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Probability"].ToString()),
                                    Exposure_id = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Exposure_id"].ToString()),
                                    Evaluation_status = dsRiskRegisterActivityList.Tables[0].Rows[i]["Evaluation_status"].ToString(),
                                    Cur_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[i]["Cur_Opt_Ctrl"].ToString(),
                                    Person_resp = dsRiskRegisterActivityList.Tables[0].Rows[i]["Person_resp"].ToString(),
                                    Action_TakenBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Action_TakenBy"].ToString()),
                                    Reviewer_QHSE = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Reviewer_QHSE"].ToString()),
                                    ApprovedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                };


                                DateTime dateValue;
                                if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[i]["Eval_Date"].ToString(), out dateValue))
                                {
                                    objRiskRegisterActivityEval.Eval_Date = dateValue;
                                }
                                if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[i]["ReEval_Date"].ToString(), out dateValue))
                                {
                                    objRiskRegisterActivityEval.ReEval_Date = dateValue;
                                }

                                int iProbability = 0, iSeverity = 0, iExposure_id = 0;
                                if (dsRiskRegisterActivityList.Tables[0].Rows[i]["Exposure_id"].ToString() != "" && dsRiskRegisterActivityList.Tables[0].Rows[i]["Exposure_id"].ToString() != null)
                                {
                                    if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Probability"].ToString()),
                                       out iProbability) &&
                                       Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Severity"].ToString()),
                                       out iSeverity) &&
                                       Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Exposure_id"].ToString()),
                                       out iExposure_id))
                                    {
                                        objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForExposure(iProbability * iSeverity * iExposure_id);
                                    }
                                }
                                else
                                {
                                    if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Probability"].ToString()),
                                         out iProbability) &&
                                         Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Severity"].ToString()),
                                         out iSeverity))
                                    {
                                        objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForExposure(iProbability * iSeverity * 1);
                                    }

                                }

                                objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval.Add(objRiskRegisterActivityEval);


                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Activity Id cannot be null";
                    return RedirectToAction("RiskRegisterActivityList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterHRREvaluationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval.ToList());
        }


        [AllowAnonymous]
        public ActionResult RiskActivityApproved(string Regeval_Id, string PendingFlg)
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRisk = new RiskRegisterActivityEvaluationModels();

                if (objRisk.FunUpdateActivityEvalApproveUpdate(Regeval_Id))
                {
                    TempData["Successdata"] = "Risk Activity Reviewed successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskActivityApproved: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("RiskRegisterActivityEvaluationHistoryList");
            }
        }

       


        [AllowAnonymous]
        public ActionResult RiskActivityReviewed(string Regeval_Id, string PendingFlg)
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRisk = new RiskRegisterActivityEvaluationModels();

                if (objRisk.FunUpdateActivityEvalReviewUpdate(Regeval_Id))
                {
                    TempData["Successdata"] = "Risk Activity Reviewed successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskActivityReviewed: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForReview", "Dashboard");
            }
            else
            {
                return RedirectToAction("RiskRegisterActivityEvaluationHistoryList");
            }
        }


        [AllowAnonymous]
        public ActionResult RiskActivityHRRReviewed(string risk_hrr_id, string PendingFlg)
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRisk = new RiskRegisterActivityEvaluationModels();

                if (objRisk.FunUpdateActivityHRRReviewUpdate(risk_hrr_id))
                {
                    TempData["Successdata"] = "Risk Activity Reviewed successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskActivityReviewed: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForReview", "Dashboard");
            }
            else
            {
                return RedirectToAction("RiskRegisterActivityEvaluationHistoryList");
            }
        }


        [AllowAnonymous]
        public ActionResult RiskActivityHRRApproved(string risk_hrr_id, string PendingFlg)
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRisk = new RiskRegisterActivityEvaluationModels();

                if (objRisk.FunUpdateActivityHRRApproveUpdate(risk_hrr_id))
                {
                    TempData["Successdata"] = "Risk Activity Reviewed successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskActivityReviewed: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("RiskRegisterActivityEvaluationHistoryList");
            }
        }


       

        [AllowAnonymous]
        public ActionResult RiskRegisterHRRActivityEvaluationEdit()
        {
            RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

            try
            {
                if (Request.QueryString["risk_hrr_id"] != null && Request.QueryString["risk_hrr_id"] != "")
                {
                    string srisk_hrr_id = Request.QueryString["risk_hrr_id"];
                    string Risk_Reg_Activity_Id = Request.QueryString["Risk_Reg_Activity_Id"];

                    string sSqlstmt = "select DeptId,Activity,Activity_Category,Risk_Type,Activity_Status,risk_hrr_id,"
                    + "t.Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id,Evaluation_status,Cur_Opt_Ctrl,Person_resp,"
                    + "Eval_Date,ReEval_Date,Action_TakenBy,Reviewer_QHSE,ApprovedBy from t_risk_register_activity t,t_risk_register_hrrevaluation tt where"
                    + " t.Risk_Reg_Activity_Id=tt.Risk_Reg_Activity_Id and risk_hrr_id='" + srisk_hrr_id + "'";

                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);

                    RiskRegisterActivityEvaluationModels objRiskActivityEval = new RiskRegisterActivityEvaluationModels();

                    if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                    {
                        RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                        {
                            Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(objRiskActivityEval.Risk_Reg_Activity_Id),
                            DeptId = objGlobaldata.GetDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Activity = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity"].ToString(),
                            Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Category"].ToString()),
                            Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Type"].ToString()),
                            Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Status"].ToString()),
                            risk_hrr_id = (dsRiskRegisterActivityList.Tables[0].Rows[0]["risk_hrr_id"].ToString()),
                            hazard = (dsRiskRegisterActivityList.Tables[0].Rows[0]["hazard"].ToString()),
                            Severity = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                            Probability = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                            Exposure_id = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString()),
                            Evaluation_status = dsRiskRegisterActivityList.Tables[0].Rows[0]["Evaluation_status"].ToString(),
                            Cur_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Cur_Opt_Ctrl"].ToString(),
                            Person_resp = dsRiskRegisterActivityList.Tables[0].Rows[0]["Person_resp"].ToString(),
                            Action_TakenBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Action_TakenBy"].ToString()),
                            Reviewer_QHSE = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Reviewer_QHSE"].ToString()),
                            ApprovedBy = (dsRiskRegisterActivityList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                        };

                        ViewBag.ActivityId = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString();

                        DateTime dateValue;
                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["Eval_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.Eval_Date = dateValue;
                        }
                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["ReEval_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.ReEval_Date = dateValue;
                        }
                        ViewBag.Employee = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.ActivityId = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString();
                        ViewBag.EvalStatus = objGlobaldata.GetConstantValue("EvaluationStatus");
                        ViewBag.Exposure = objGlobaldata.GetDropdownList("Risk-Exposure");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.RiskProbability = objGlobaldata.GetDropdownitemById("HRR-Probability");
                        ViewBag.RiskSeverity = objGlobaldata.GetDropdownitemById("HRR-Severity");
                        ViewBag.objRiskRegisterActivity = objRiskRegisterActivityEval;

                        return View(objRiskRegisterActivityEval);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("RiskRegisterHRREvaluationList", new { Risk_Reg_Activity_Id = Risk_Reg_Activity_Id });
                    }
                }
                else
                {
                    TempData["alertdata"] = "Evaluation Id cannot be null";
                    return RedirectToAction("RiskRegisterHRREvaluationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterHRRActivityEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterHRREvaluationList");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RiskRegisterHRRActivityEvaluationEdit(RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval, FormCollection form)
        {
            try
            {

                DateTime dateValue;

                if (form["Eval_Date"] != null && DateTime.TryParse(form["Eval_Date"], out dateValue) == true)
                {
                    objRiskRegisterActivityEval.Eval_Date = dateValue;
                }

                if (form["ReEval_Date"] != null && DateTime.TryParse(form["ReEval_Date"], out dateValue) == true)
                {
                    objRiskRegisterActivityEval.ReEval_Date = dateValue;
                }

                if (objRiskRegisterActivityEval.FunUpdateRiskRegisterHRRActivityEval(objRiskRegisterActivityEval))
                {
                    TempData["Successdata"] = "Updated Risk Register Activity Evaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHRRActivityEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterHRREvaluationList", new { Risk_Reg_Activity_Id = objRiskRegisterActivityEval.Risk_Reg_Activity_Id });
        }


        [AllowAnonymous]
        public ActionResult RiskRegisterHRRActivityEvalDetails()
        {

            RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

            try
            {
                if (Request.QueryString["risk_hrr_id"] != null && Request.QueryString["risk_hrr_id"] != "")
                {
                    string srisk_hrr_id = Request.QueryString["risk_hrr_id"];
                    string Risk_Reg_Activity_Id = Request.QueryString["Risk_Reg_Activity_Id"];

                    string sSqlstmt = "select DeptId,Activity,Activity_Category,Risk_Type,Activity_Status,risk_hrr_id,"
                    + "t.Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id,Evaluation_status,Cur_Opt_Ctrl,Person_resp,"
                    + " (case when Approve_status='1' then 'Approved' else 'Not Approved' end) as Approve_status,"
                    + "Eval_Date,ReEval_Date,Action_TakenBy,Reviewer_QHSE,ApprovedBy from t_risk_register_activity t,t_risk_register_hrrevaluation tt where"
                    + " t.Risk_Reg_Activity_Id=tt.Risk_Reg_Activity_Id and risk_hrr_id='" + srisk_hrr_id + "'";

                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);

                    RiskRegisterActivityEvaluationModels objRiskActivityEval = new RiskRegisterActivityEvaluationModels();

                    if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                    {
                        RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                        {
                            Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(objRiskActivityEval.Risk_Reg_Activity_Id),
                            DeptId = objGlobaldata.GetDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Activity = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity"].ToString(),
                            Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Category"].ToString()),
                            Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Type"].ToString()),
                            Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Status"].ToString()),
                            risk_hrr_id = (dsRiskRegisterActivityList.Tables[0].Rows[0]["risk_hrr_id"].ToString()),
                            hazard = (dsRiskRegisterActivityList.Tables[0].Rows[0]["hazard"].ToString()),
                            Severity = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                            Probability = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                            Exposure_id = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString()),
                            Evaluation_status = dsRiskRegisterActivityList.Tables[0].Rows[0]["Evaluation_status"].ToString(),
                            Cur_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Cur_Opt_Ctrl"].ToString(),
                            Person_resp = dsRiskRegisterActivityList.Tables[0].Rows[0]["Person_resp"].ToString(),
                            Approve_status = dsRiskRegisterActivityList.Tables[0].Rows[0]["Approve_status"].ToString(),
                            Action_TakenBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Action_TakenBy"].ToString()),
                            Reviewer_QHSE = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Reviewer_QHSE"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                        };
                        int iProbability = 0, iSeverity = 0, iExposure_id = 0;
                        if (dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString() != "" && dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString() != null)
                        {
                            if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                               out iProbability) &&
                               Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                               out iSeverity) &&
                               Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString()),
                               out iExposure_id))
                            {
                                objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForExposure(iProbability * iSeverity * iExposure_id);
                            }
                        }
                        else
                        {
                            if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                                 out iProbability) &&
                                 Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                                 out iSeverity))
                            {
                                objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForExposure(iProbability * iSeverity * 1);
                            }

                        }
                        ViewBag.ActivityId = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString();

                        DateTime dateValue;
                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["Eval_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.Eval_Date = dateValue;
                        }
                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["ReEval_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.ReEval_Date = dateValue;
                        }
                        ViewBag.ActivityId = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString();
                        ViewBag.EvalStatus = objGlobaldata.GetConstantValue("EvaluationStatus");
                        ViewBag.Exposure = objGlobaldata.GetDropdownList("Risk-Exposure");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.RiskProbability = objGlobaldata.GetDropdownitemById("HRR-Probability");
                        ViewBag.RiskSeverity = objGlobaldata.GetDropdownitemById("HRR-Severity");
                        ViewBag.objRiskRegisterActivity = objRiskRegisterActivityEval;

                        return View(objRiskRegisterActivityEval);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("RiskRegisterHRREvaluationList", new { Risk_Reg_Activity_Id = Risk_Reg_Activity_Id });
                    }
                }
                else
                {
                    TempData["alertdata"] = "Evaluation Id cannot be null";
                    return RedirectToAction("RiskRegisterHRREvaluationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterHRRActivityEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterHRREvaluationList");
        }


        [AllowAnonymous]
        public ActionResult RiskRegisterHRRActivityEvalInfo(int id)
        {

            RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

            try
            {


                string sSqlstmt = "select DeptId,Activity,Activity_Category,Risk_Type,Activity_Status,risk_hrr_id,"
                + "t.Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id,Evaluation_status,Cur_Opt_Ctrl,Person_resp,"
                + " (case when Approve_status='1' then 'Approved' else 'Not Approved' end) as Approve_status,"
                + "Eval_Date,ReEval_Date,Action_TakenBy,Reviewer_QHSE,ApprovedBy from t_risk_register_activity t,t_risk_register_hrrevaluation tt where"
                + " t.Risk_Reg_Activity_Id=tt.Risk_Reg_Activity_Id and risk_hrr_id='" + id + "'";

                DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);

                RiskRegisterActivityEvaluationModels objRiskActivityEval = new RiskRegisterActivityEvaluationModels();

                if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                {
                    RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                    {
                        Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(objRiskActivityEval.Risk_Reg_Activity_Id),
                        DeptId = objGlobaldata.GetDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["DeptId"].ToString()),
                        Activity = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity"].ToString(),
                        Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Category"].ToString()),
                        Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Type"].ToString()),
                        Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Status"].ToString()),
                        risk_hrr_id = (dsRiskRegisterActivityList.Tables[0].Rows[0]["risk_hrr_id"].ToString()),
                        hazard = (dsRiskRegisterActivityList.Tables[0].Rows[0]["hazard"].ToString()),
                        Severity = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                        Probability = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                        Exposure_id = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString()),
                        Evaluation_status = dsRiskRegisterActivityList.Tables[0].Rows[0]["Evaluation_status"].ToString(),
                        Cur_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Cur_Opt_Ctrl"].ToString(),
                        Person_resp = dsRiskRegisterActivityList.Tables[0].Rows[0]["Person_resp"].ToString(),
                        Approve_status = dsRiskRegisterActivityList.Tables[0].Rows[0]["Approve_status"].ToString(),
                        Action_TakenBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Action_TakenBy"].ToString()),
                        Reviewer_QHSE = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Reviewer_QHSE"].ToString()),
                        ApprovedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                    };
                    int iProbability = 0, iSeverity = 0, iExposure_id = 0;
                    if (dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString() != "" && dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString() != null)
                    {
                        if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                           out iProbability) &&
                           Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                           out iSeverity) &&
                           Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString()),
                           out iExposure_id))
                        {
                            objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForExposure(iProbability * iSeverity * iExposure_id);
                        }
                    }
                    else
                    {
                        if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                             out iProbability) &&
                             Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                             out iSeverity))
                        {
                            objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForExposure(iProbability * iSeverity * 1);
                        }

                    }
                    ViewBag.ActivityId = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString();

                    DateTime dateValue;
                    if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["Eval_Date"].ToString(), out dateValue))
                    {
                        objRiskRegisterActivityEval.Eval_Date = dateValue;
                    }
                    if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["ReEval_Date"].ToString(), out dateValue))
                    {
                        objRiskRegisterActivityEval.ReEval_Date = dateValue;
                    }
                    ViewBag.ActivityId = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString();
                    ViewBag.EvalStatus = objGlobaldata.GetConstantValue("EvaluationStatus");
                    ViewBag.Exposure = objGlobaldata.GetDropdownList("Risk-Exposure");
                    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.RiskProbability = objGlobaldata.GetDropdownitemById("HRR-Probability");
                    ViewBag.RiskSeverity = objGlobaldata.GetDropdownitemById("HRR-Severity");
                    ViewBag.objRiskRegisterActivity = objRiskRegisterActivityEval;

                    return View(objRiskRegisterActivityEval);

                }
                else
                {
                    TempData["alertdata"] = "Evaluation Id cannot be null";
                    return RedirectToAction("RiskRegisterHRREvaluationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterHRRActivityEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterHRREvaluationList");
        }

        //
        // GET: /RiskRegisterActivity/RiskRegisterActivityEvaluationList

        [AllowAnonymous]
        public ActionResult RiskRegisterActivityEvaluationList(string SearchText, int? page)
        {
            RiskRegisterActivityEvaluationModelsList objRiskRegisterActivityEvalList = new RiskRegisterActivityEvaluationModelsList();
            objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval = new List<RiskRegisterActivityEvaluationModels>();

            RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

            try
            {
                if (Request.QueryString["Risk_Reg_Activity_Id"] != null && Request.QueryString["Risk_Reg_Activity_Id"] != "")
                {
                    string sRisk_Reg_Activity_Id = Request.QueryString["Risk_Reg_Activity_Id"];

                    ViewBag.Risk_Reg_Activity_Id = sRisk_Reg_Activity_Id;

                    string sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence, Cur_Opt_Ctrl, Severity, "
                            + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                              + " (case when Approve_status='1' then 'Approved' else 'Not Approved' end) as Approve_status,"
                            + " DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, Comments,Exposure_id,Appl_law from t_risk_register_activity_eval as trEval, "
                            + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id and tract.Risk_Reg_Activity_Id='"
                            + sRisk_Reg_Activity_Id + "' and trEval.Active=1";


                    if (SearchText != null && SearchText != "")
                    {
                        sSqlstmt = sSqlstmt + " and Activity like '" + SearchText + "%' or Activity like '%" + SearchText + "%' or Activity like '%" + SearchText + "'";
                    }

                    sSqlstmt = sSqlstmt + " order by Reg_Activity_eval_Id desc";

                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);

                    RiskRegisterActivityEvaluationModels objRiskActivityEval = new RiskRegisterActivityEvaluationModels();

                    if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                    {
                        
                        for (int i = 0; i < dsRiskRegisterActivityList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                                {
                                    Reg_Activity_eval_Id = (dsRiskRegisterActivityList.Tables[0].Rows[i]["Reg_Activity_eval_Id"].ToString()),
                                    Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id"].ToString()),
                                    Risk_Id = dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id"].ToString(),
                                    EvalBy = objGlobaldata.GetMultiHrEmpNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["EvalBy"].ToString()),
                                    Reviewer_QHSE = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Reviewer_QHSE"].ToString()),
                                    ApprovedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                    Consequence = (dsRiskRegisterActivityList.Tables[0].Rows[i]["Consequence"].ToString()),
                                    Cur_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[i]["Cur_Opt_Ctrl"].ToString(),
                                    Severity = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Severity"].ToString()),
                                    Probability = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Probability"].ToString()),
                                    Exposure_id = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Exposure_id"].ToString()),
                                    Add_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[i]["Add_Opt_Ctrl"].ToString(),
                                    Opt_Ctrl_Implt = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Opt_Ctrl_Implt"].ToString()),
                                    Desc_Opt_ctrl = dsRiskRegisterActivityList.Tables[0].Rows[i]["Desc_Opt_ctrl"].ToString(),
                                    Action_TakenBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Action_TakenBy"].ToString()),
                                    Comments = dsRiskRegisterActivityList.Tables[0].Rows[i]["Comments"].ToString(),
                                    Appl_law = objGlobaldata.GetLawNoById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Appl_law"].ToString())
                                };


                                DateTime dateValue;
                                if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[i]["Eval_Date"].ToString(), out dateValue))
                                {
                                    objRiskRegisterActivityEval.Eval_Date = dateValue;
                                }

                                if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[i]["Due_Date"].ToString(), out dateValue))
                                {
                                    objRiskRegisterActivityEval.Due_Date = dateValue;
                                }

                                if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[i]["ReEval_Date"].ToString(), out dateValue))
                                {
                                    objRiskRegisterActivityEval.ReEval_Date = dateValue;
                                }

                                int iProbability = 0, iSeverity = 0;
                                if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Probability"].ToString()),
                                    out iProbability) &&
                                    Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Severity"].ToString()),
                                    out iSeverity))
                                {
                                    objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForEnv(iProbability * iSeverity);
                                }
                                objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval.Add(objRiskRegisterActivityEval);


                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Activity Id cannot be null";
                    return RedirectToAction("RiskRegisterActivityList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityEvaluationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval.ToList());
        }


        [AllowAnonymous]
        public ActionResult RiskRegisterActivityEvaluationInfo(int id)
        {
            RiskRegisterActivityEvaluationModelsList objRiskRegisterActivityEvalList = new RiskRegisterActivityEvaluationModelsList();
            objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval = new List<RiskRegisterActivityEvaluationModels>();

            RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

            try
            {
                string sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence, Cur_Opt_Ctrl, Severity, "
                        + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                        + " (case when Approve_status='1' then 'Approved' else 'Not Approved' end) as Approve_status,"
                        + " DeptId, Activity_No, Activity, Activity_Category, Risk_Type,Appl_law, Activity_Status,hazard, Comments,Exposure_id from t_risk_register_activity_eval as trEval, "
                        + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id and trEval.Reg_Activity_eval_Id='"
                        + id + "'";


                DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);

                RiskRegisterActivityEvaluationModels objRiskActivityEval = new RiskRegisterActivityEvaluationModels();

                if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsRiskRegisterActivityList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                            {
                                Reg_Activity_eval_Id = (dsRiskRegisterActivityList.Tables[0].Rows[i]["Reg_Activity_eval_Id"].ToString()),
                                Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id"].ToString()),
                                EvalBy = objGlobaldata.GetMultiHrEmpNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["EvalBy"].ToString()),
                                Reviewer_QHSE = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Reviewer_QHSE"].ToString()),
                                ApprovedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                Consequence = (dsRiskRegisterActivityList.Tables[0].Rows[i]["Consequence"].ToString()),
                                Cur_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[i]["Cur_Opt_Ctrl"].ToString(),
                                Severity = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Severity"].ToString()),
                                Probability = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Probability"].ToString()),
                                Exposure_id = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString()),
                                Add_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[i]["Add_Opt_Ctrl"].ToString(),
                                Opt_Ctrl_Implt = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Opt_Ctrl_Implt"].ToString()),
                                Desc_Opt_ctrl = dsRiskRegisterActivityList.Tables[0].Rows[i]["Desc_Opt_ctrl"].ToString(),
                                Action_TakenBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Action_TakenBy"].ToString()),
                                Comments = dsRiskRegisterActivityList.Tables[0].Rows[i]["Comments"].ToString(),
                                Approve_status = dsRiskRegisterActivityList.Tables[0].Rows[0]["Approve_status"].ToString(),
                                hazard = dsRiskRegisterActivityList.Tables[0].Rows[i]["hazard"].ToString(),
                                Appl_law = objGlobaldata.GetLawNoById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Appl_law"].ToString())
                            };


                            DateTime dateValue;
                            if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[i]["Eval_Date"].ToString(), out dateValue))
                            {
                                objRiskRegisterActivityEval.Eval_Date = dateValue;
                            }

                            if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[i]["Due_Date"].ToString(), out dateValue))
                            {
                                objRiskRegisterActivityEval.Due_Date = dateValue;
                            }

                            if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[i]["ReEval_Date"].ToString(), out dateValue))
                            {
                                objRiskRegisterActivityEval.ReEval_Date = dateValue;
                            }

                            int iProbability = 0, iSeverity = 0, iExposure = 0;
                            if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Probability"].ToString()),
                                out iProbability) &&
                                Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Severity"].ToString()),
                                out iSeverity))
                            {
                                objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForEnv(iProbability * iSeverity);
                            }
                            if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                              out iProbability) &&
                              Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                              out iSeverity) &&
                              Int32.TryParse(objGlobaldata.GetExposureValueById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString()),
                              out iExposure))
                            {
                                objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForExposure(iProbability * iSeverity * iExposure);
                            }
                            objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval.Add(objRiskRegisterActivityEval);


                        }
                        catch (Exception ex)
                        { }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityEvaluationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval.ToList());
        }

        //
        // GET: /RiskRegisterActivity/RiskRegisterActivityEvaluationHistoryList

        [AllowAnonymous]
        public ActionResult RiskRegisterActivityEvaluationHistoryList(string SearchText, int? page)
        {
            RiskRegisterActivityEvaluationModelsList objRiskRegisterActivityEvalList = new RiskRegisterActivityEvaluationModelsList();
            objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval = new List<RiskRegisterActivityEvaluationModels>();

            RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

            try
            {
                if (Request.QueryString["Risk_Reg_Activity_Id"] != null && Request.QueryString["Risk_Reg_Activity_Id"] != "")
                {
                    string sRisk_Reg_Activity_Id = Request.QueryString["Risk_Reg_Activity_Id"];

                    ViewBag.Risk_Reg_Activity_Id = sRisk_Reg_Activity_Id;

                    string sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence, Cur_Opt_Ctrl, Severity, "
                            + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                            + " DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, trEval.Comments from t_risk_register_activity_eval as trEval, "
                            + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id and tract.Risk_Reg_Activity_Id='"
                            + sRisk_Reg_Activity_Id + "'";


                    if (SearchText != null && SearchText != "")
                    {
                        sSqlstmt = sSqlstmt + " and Activity like '" + SearchText + "%' or Activity like '%" + SearchText + "%' or Activity like '%" + SearchText + "'";
                    }

                    sSqlstmt = sSqlstmt + " order by Reg_Activity_eval_Id desc";

                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);

                    RiskRegisterActivityEvaluationModels objRiskActivityEval = new RiskRegisterActivityEvaluationModels();

                    if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsRiskRegisterActivityList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                                {
                                    Reg_Activity_eval_Id = (dsRiskRegisterActivityList.Tables[0].Rows[i]["Reg_Activity_eval_Id"].ToString()),
                                    Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Risk_Reg_Activity_Id"].ToString()),
                                    EvalBy = objGlobaldata.GetMultiHrEmpNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["EvalBy"].ToString()),
                                    Reviewer_QHSE = objGlobaldata.GetMultiHrEmpNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Reviewer_QHSE"].ToString()),
                                    ApprovedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                    Consequence = (dsRiskRegisterActivityList.Tables[0].Rows[i]["Consequence"].ToString()),
                                    Cur_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[i]["Cur_Opt_Ctrl"].ToString(),
                                    Severity = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Severity"].ToString()),
                                    Probability = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Probability"].ToString()),

                                    Add_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[i]["Add_Opt_Ctrl"].ToString(),
                                    Opt_Ctrl_Implt = (dsRiskRegisterActivityList.Tables[0].Rows[i]["Opt_Ctrl_Implt"].ToString()),
                                    Desc_Opt_ctrl = dsRiskRegisterActivityList.Tables[0].Rows[i]["Desc_Opt_ctrl"].ToString(),
                                    Action_TakenBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Action_TakenBy"].ToString()),
                                    Comments = dsRiskRegisterActivityList.Tables[0].Rows[i]["Comments"].ToString()
                                };


                                DateTime dateValue;
                                if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[i]["Eval_Date"].ToString(), out dateValue))
                                {
                                    objRiskRegisterActivityEval.Eval_Date = dateValue;
                                }

                                if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[i]["Due_Date"].ToString(), out dateValue))
                                {
                                    objRiskRegisterActivityEval.Due_Date = dateValue;
                                }

                                if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[i]["ReEval_Date"].ToString(), out dateValue))
                                {
                                    objRiskRegisterActivityEval.ReEval_Date = dateValue;
                                }

                                int iProbability = 0, iSeverity = 0;
                                if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Probability"].ToString()),
                                    out iProbability) &&
                                    Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[i]["Severity"].ToString()),
                                    out iSeverity))
                                {
                                    objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForEnv(iProbability * iSeverity);
                                }

                                objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval.Add(objRiskRegisterActivityEval);


                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Activity Id cannot be null";
                    return RedirectToAction("RiskRegisterActivityList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityEvaluationHistoryList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objRiskRegisterActivityEvalList.lstRiskRegisterActivityEval.ToList().ToPagedList(page ?? 1, 40));
        }


        //
        // GET: /RiskRegisterActivity/RiskRegisterActivityEvaluationDetails

        [AllowAnonymous]
        public ActionResult RiskRegisterActivityEvaluationDetails()
        {
            RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

            try
            {
                if (Request.QueryString["Reg_Activity_eval_Id"] != null && Request.QueryString["Reg_Activity_eval_Id"] != "")
                {
                    string sReg_Activity_eval_Id = Request.QueryString["Reg_Activity_eval_Id"];
                    string Risk_Reg_Activity_Id = Request.QueryString["Risk_Reg_Activity_Id"];

                    string sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence,hazard, Cur_Opt_Ctrl, Severity, "
                            + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                           + " (case when Approve_status='1' then 'Approved' else 'Not Approved' end) as Approve_status,"
                            + " DeptId, Activity_No, Activity, Activity_Category, Risk_Type,Appl_law, Activity_Status, trEval.Comments,Exposure_id from t_risk_register_activity_eval as trEval, "
                            + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id and Reg_Activity_eval_Id='" + sReg_Activity_eval_Id + "'";

                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);

                    RiskRegisterActivityEvaluationModels objRiskActivityEval = new RiskRegisterActivityEvaluationModels();

                    if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                    {
                        RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                        {
                            Reg_Activity_eval_Id = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Reg_Activity_eval_Id"].ToString()),
                            Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString()),
                            EvalBy = objGlobaldata.GetMultiHrEmpNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["EvalBy"].ToString()),
                            Reviewer_QHSE = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Reviewer_QHSE"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            Consequence = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Consequence"].ToString()),
                            Cur_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Cur_Opt_Ctrl"].ToString(),
                            Severity = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                            Probability = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                            Add_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Add_Opt_Ctrl"].ToString(),
                            Opt_Ctrl_Implt = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Opt_Ctrl_Implt"].ToString()),
                            Desc_Opt_ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Desc_Opt_ctrl"].ToString(),
                            Action_TakenBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Action_TakenBy"].ToString()),
                            Activity = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity"].ToString(),
                            DeptId = objGlobaldata.GetDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_No"].ToString(),
                            hazard = dsRiskRegisterActivityList.Tables[0].Rows[0]["hazard"].ToString(),
                            Approve_status = dsRiskRegisterActivityList.Tables[0].Rows[0]["Approve_status"].ToString(),
                            Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Category"].ToString()),
                            Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Type"].ToString()),
                            Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Status"].ToString()),
                            Comments = dsRiskRegisterActivityList.Tables[0].Rows[0]["Comments"].ToString(),
                            Appl_law = objGlobaldata.GetLawNoById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Appl_law"].ToString()),
                            Exposure_id = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString()),
                        };

                        ViewBag.ActivityId = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString();

                        DateTime dateValue;
                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["Eval_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.Eval_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["Due_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.Due_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["ReEval_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.ReEval_Date = dateValue;
                        }

                        int iProbability = 0, iSeverity = 0, iExposure = 0;
                        if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                            out iProbability) &&
                            Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                            out iSeverity))
                        {
                            objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForEnv(iProbability * iSeverity);
                        }
                        if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                                  out iProbability) &&
                                  Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                                  out iSeverity) &&
                                  Int32.TryParse(objGlobaldata.GetExposureValueById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString()),
                                  out iExposure))
                        {
                            objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForExposure(iProbability * iSeverity * iExposure);
                        }
                        return View(objRiskRegisterActivityEval);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("RiskRegisterActivityEvaluationList", new { Risk_Reg_Activity_Id = Risk_Reg_Activity_Id });
                    }
                }
                else
                {
                    TempData["alertdata"] = "Evaluation Id cannot be null";
                    return RedirectToAction("RiskRegisterActivityEvaluationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityEvaluationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterActivityEvaluationList");
        }


        //
        // GET: /RiskRegisterActivity/RiskRegisterActivityEvaluationDetails

        [AllowAnonymous]
        public ActionResult RiskRegisterActivityEvaluationHistoryDetails()
        {
            RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

            try
            {
                if (Request.QueryString["Reg_Activity_eval_Id"] != null && Request.QueryString["Reg_Activity_eval_Id"] != "")
                {
                    string sReg_Activity_eval_Id = Request.QueryString["Reg_Activity_eval_Id"];
                    string Risk_Reg_Activity_Id = Request.QueryString["Risk_Reg_Activity_Id"];

                    string sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence, Cur_Opt_Ctrl, Severity, "
                            + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                            + " DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, trEval.Comments from t_risk_register_activity_eval as trEval, "
                            + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id and Reg_Activity_eval_Id='" + sReg_Activity_eval_Id + "'";

                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);

                    RiskRegisterActivityEvaluationModels objRiskActivityEval = new RiskRegisterActivityEvaluationModels();

                    if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                    {
                        RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                        {
                            Reg_Activity_eval_Id = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Reg_Activity_eval_Id"].ToString()),
                            Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString()),
                            EvalBy = objGlobaldata.GetMultiHrEmpNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["EvalBy"].ToString()),
                            Reviewer_QHSE = objGlobaldata.GetMultiHrEmpNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Reviewer_QHSE"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            Consequence = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Consequence"].ToString()),
                            Cur_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Cur_Opt_Ctrl"].ToString(),
                            Severity = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                            Probability = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                            Add_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Add_Opt_Ctrl"].ToString(),
                            Opt_Ctrl_Implt = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Opt_Ctrl_Implt"].ToString()),
                            Desc_Opt_ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Desc_Opt_ctrl"].ToString(),
                            Action_TakenBy = objGlobaldata.GetEmpHrNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Action_TakenBy"].ToString()),

                            DeptId = objGlobaldata.GetDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_No"].ToString(),
                            Activity = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity"].ToString(),
                            Activity_Category = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Category"].ToString(),
                            Risk_Type = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Type"].ToString()),
                            Activity_Status = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Status"].ToString(),
                            Comments = dsRiskRegisterActivityList.Tables[0].Rows[0]["Comments"].ToString()
                        };

                        ViewBag.ActivityId = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString();

                        DateTime dateValue;
                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["Eval_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.Eval_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["Due_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.Due_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["ReEval_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.ReEval_Date = dateValue;
                        }

                        int iProbability = 0, iSeverity = 0;
                        if (Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                            out iProbability) &&
                            Int32.TryParse(objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                            out iSeverity))
                        {
                            objRiskRegisterActivityEval.Risk_Rating = objGlobaldata.GetRiskRatingForEnv(iProbability * iSeverity);
                        }

                        return View(objRiskRegisterActivityEval);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("RiskRegisterActivityEvaluationList", new { Risk_Reg_Activity_Id = Risk_Reg_Activity_Id });
                    }
                }
                else
                {
                    TempData["alertdata"] = "Evaluation Id cannot be null";
                    return RedirectToAction("RiskRegisterActivityEvaluationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityEvaluationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterActivityEvaluationList");
        }


        //
        // GET: /RiskRegisterActivity/RiskRegisterActivityEvaluationEdit

        [AllowAnonymous]
        public ActionResult RiskRegisterActivityEvaluationEdit()
        {
            RiskRegisterActivityModels objRiskActivity = new RiskRegisterActivityModels();

            try
            {
                if (Request.QueryString["Reg_Activity_eval_Id"] != null && Request.QueryString["Reg_Activity_eval_Id"] != "")
                {
                    string sReg_Activity_eval_Id = Request.QueryString["Reg_Activity_eval_Id"];
                    string Risk_Reg_Activity_Id = Request.QueryString["Risk_Reg_Activity_Id"];

                    string sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence,hazard, Cur_Opt_Ctrl, Severity, "
                            + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                            + " DeptId, Activity_No, Activity, Activity_Category,Appl_law, Risk_Type, Activity_Status, trEval.Comments,Exposure_id from t_risk_register_activity_eval as trEval, "
                            + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id and Reg_Activity_eval_Id='" + sReg_Activity_eval_Id + "'";


                    DataSet dsRiskRegisterActivityList = objGlobaldata.Getdetails(sSqlstmt);

                    RiskRegisterActivityEvaluationModels objRiskActivityEval = new RiskRegisterActivityEvaluationModels();

                    if (dsRiskRegisterActivityList.Tables.Count > 0 && dsRiskRegisterActivityList.Tables[0].Rows.Count > 0)
                    {
                        RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval = new RiskRegisterActivityEvaluationModels
                        {
                            Reg_Activity_eval_Id = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Reg_Activity_eval_Id"].ToString()),
                            Risk_Reg_Activity_Id = objRiskActivity.GetRiskAcivityNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString()),
                            EvalBy = (dsRiskRegisterActivityList.Tables[0].Rows[0]["EvalBy"].ToString()),
                            Reviewer_QHSE = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Reviewer_QHSE"].ToString()),
                            ApprovedBy = (dsRiskRegisterActivityList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            Consequence = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Consequence"].ToString()),
                            Cur_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Cur_Opt_Ctrl"].ToString(),
                            Severity = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Severity"].ToString()),
                            Probability = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Probability"].ToString()),
                            Risk_Rating = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Rating"].ToString()),
                            Add_Opt_Ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Add_Opt_Ctrl"].ToString(),
                            Opt_Ctrl_Implt = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Opt_Ctrl_Implt"].ToString()),
                            Desc_Opt_ctrl = dsRiskRegisterActivityList.Tables[0].Rows[0]["Desc_Opt_ctrl"].ToString(),
                            Action_TakenBy = (dsRiskRegisterActivityList.Tables[0].Rows[0]["Action_TakenBy"].ToString()),
                            Activity_No = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_No"].ToString(),
                            DeptId = objGlobaldata.GetDeptNameById(dsRiskRegisterActivityList.Tables[0].Rows[0]["DeptId"].ToString()),
                            hazard = dsRiskRegisterActivityList.Tables[0].Rows[0]["hazard"].ToString(),
                            Activity = dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity"].ToString(),
                            Comments = dsRiskRegisterActivityList.Tables[0].Rows[0]["Comments"].ToString(),
                            Appl_law = dsRiskRegisterActivityList.Tables[0].Rows[0]["Appl_law"].ToString(),
                            Exposure_id = dsRiskRegisterActivityList.Tables[0].Rows[0]["Exposure_id"].ToString(),
                            Activity_Category = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Category"].ToString()),
                            Risk_Type = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Type"].ToString()),
                            Activity_Status = objGlobaldata.GetDropdownitemById(dsRiskRegisterActivityList.Tables[0].Rows[0]["Activity_Status"].ToString()),
                        };

                        ViewBag.ActivityId = dsRiskRegisterActivityList.Tables[0].Rows[0]["Risk_Reg_Activity_Id"].ToString();

                        DateTime dateValue;
                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["Eval_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.Eval_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["Due_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.Due_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsRiskRegisterActivityList.Tables[0].Rows[0]["ReEval_Date"].ToString(), out dateValue))
                        {
                            objRiskRegisterActivityEval.ReEval_Date = dateValue;
                        }
                        ViewBag.Employee = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.Exposure = objGlobaldata.GetDropdownList("Risk-Exposure");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.deptid = objGlobaldata.GetDepartmentWithIdListbox();
                        ViewBag.DeptHead = objGlobaldata.GetDeptHeadList();
                        ViewBag.QHSEEmp = objGlobaldata.GetHrQHSEEmployeeListbox();
                        ViewBag.RiskProbability = objGlobaldata.GetDropdownList("ENV-Probability");
                        ViewBag.RiskSeverity = objGlobaldata.GetDropdownList("ENV-Severity");
                        ViewBag.OperationalType = objGlobaldata.GetDropdownList("RiskRegister-Operational Control");
                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.ApplLaw = objGlobaldata.GetLawNo();
                        return View(objRiskRegisterActivityEval);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("RiskRegisterActivityEvaluationList", new { Risk_Reg_Activity_Id = Risk_Reg_Activity_Id });
                    }
                }
                else
                {
                    TempData["alertdata"] = "Evaluation Id cannot be null";
                    return RedirectToAction("RiskRegisterActivityEvaluationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskRegisterActivityEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterActivityEvaluationList");
        }


        //
        // POST: /RiskRegisterActivity/RiskRegisterActivityEvaluationEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RiskRegisterActivityEvaluationEdit(RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval, FormCollection form)
        {
            try
            {
                objRiskRegisterActivityEval.EvalBy = form["EvalBy"];
                objRiskRegisterActivityEval.Reviewer_QHSE = form["Reviewer_QHSE"];

                DateTime dateValue;

                if (form["Eval_Date"] != null && DateTime.TryParse(form["Eval_Date"], out dateValue) == true)
                {
                    objRiskRegisterActivityEval.Eval_Date = dateValue;
                }

                if (form["Due_Date"] != null && DateTime.TryParse(form["Due_Date"], out dateValue) == true)
                {
                    objRiskRegisterActivityEval.Due_Date = dateValue;
                }

                if (form["ReEval_Date"] != null && DateTime.TryParse(form["ReEval_Date"], out dateValue) == true)
                {
                    objRiskRegisterActivityEval.ReEval_Date = dateValue;
                }

                if (objRiskRegisterActivityEval.FunUpdateRiskRegisterActivityEval(objRiskRegisterActivityEval))
                {
                    TempData["Successdata"] = "Updated Risk Register Activity Evaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRiskRegisterActivityEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RiskRegisterActivityEvaluationList", new { Risk_Reg_Activity_Id = objRiskRegisterActivityEval.Risk_Reg_Activity_Id });
        }
        public JsonResult RiskActivityHRRApprovedNoty(string risk_hrr_id, string PendingFlg)
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRisk = new RiskRegisterActivityEvaluationModels();

                if (objRisk.FunUpdateActivityHRRApproveUpdate(risk_hrr_id))
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskActivityReviewed: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return Json("Success");
            }
            else
            {
                return Json("Failed");
            }
        }


        public JsonResult RiskActivityApprovedNoty(string Regeval_Id, string PendingFlg)
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRisk = new RiskRegisterActivityEvaluationModels();

                if (objRisk.FunUpdateActivityEvalApproveUpdate(Regeval_Id))
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskActivityApproved: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            if (PendingFlg != null && PendingFlg == "true")
            {
                return Json("Success");
            }
            else
            {
                return Json("Failed");
            }
        }

        public JsonResult RiskActivityReviewedNoty(string Regeval_Id, string PendingFlg)
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRisk = new RiskRegisterActivityEvaluationModels();

                if (objRisk.FunUpdateActivityEvalReviewUpdate(Regeval_Id))
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskActivityReviewed: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return Json("Success");
            }
            else
            {
                return Json("Failed");
            }
        }


        public JsonResult RiskActivityHRRReviewedNoty(string risk_hrr_id, string PendingFlg)
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRisk = new RiskRegisterActivityEvaluationModels();

                if (objRisk.FunUpdateActivityHRRReviewUpdate(risk_hrr_id))
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RiskActivityReviewed: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return Json("Success");
            }
            else
            {
                return Json("Failed");
            }
        }

    }
}
