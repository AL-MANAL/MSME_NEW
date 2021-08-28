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
using Rotativa;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class HseIndController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        HseIndModels objHse = new HseIndModels();


        public HseIndController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "HseInd";
        }

        public ActionResult AddHseInd()
        {
            return InitilizeHseInd();
        }

        private ActionResult InitilizeHseInd()
        {
            try
            {
                ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Employee = objGlobaldata.GetHrEmployeeListbox(); 
                ViewBag.Dept = objGlobaldata.GetDepartmentListbox();
                ViewBag.HseInd = objHse.GetMultiHseInd();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
                ViewBag.EmpType = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.Visitors = objGlobaldata.GetVisitorList();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeHseInd: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHseInd(HseIndModels objHseInd, FormCollection form)
        {
            try
            {
                objHseInd.Visitors = form["Visitors"];
                objHseInd.Others = form["Others"];
                objHseInd.Employee = form["Employee"];

                DateTime dateValue;

                if (form["Hse_DateTime"] != null && DateTime.TryParse(form["Hse_DateTime"], out dateValue) == true)
                {
                    objHseInd.Hse_DateTime = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objHseInd.Hse_DateTime = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }

                if (objHseInd.FunAddHseInd(objHseInd))
                {
                    TempData["Successdata"] = "Added Hse Ind details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHseInd: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HseIndList");
        }

        public ActionResult HseIndList(string branch_name)
        {
            ViewBag.SubMenutype = "HseInd";
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

            HseIndModelsList objHseIndModelsList = new HseIndModelsList();
            objHseIndModelsList.HseIndList = new List<HseIndModels>();
           
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];


                string sSqlstmt = "SELECT id_Hse_insp,Hse_DateTime, Location, Employee, Dept," +
                    " General_Behavior, Site_Details_Responsibilities, Personal_Protective_Equipments_Compliance," +
                    "First_Aid,Emergency_Assistance,IMS,Equipments_Procedures,Remarks,Induction_given_by,Project,ReportNo,EmpType,Visitors,Others"
                    + "  from t_hse_ind where Active='1'";

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
                DataSet dsHseIndList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsHseIndList.Tables.Count > 0)
                {                  

                    for (int i = 0; i < dsHseIndList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtHseDate = Convert.ToDateTime(dsHseIndList.Tables[0].Rows[i]["Hse_DateTime"].ToString());

                        try
                        {
                            HseIndModels objHseIndModels = new HseIndModels
                            {
                                id_Hse_insp = dsHseIndList.Tables[0].Rows[i]["id_Hse_insp"].ToString(),
                                Location = objGlobaldata.GetCompanyBranchNameById(dsHseIndList.Tables[0].Rows[i]["Location"].ToString()),
                                Hse_DateTime = dtHseDate,
                                Employee = objGlobaldata.GetMultiHrEmpNameById(dsHseIndList.Tables[0].Rows[i]["Employee"].ToString()),
                                Dept = objGlobaldata.GetDeptNameById(dsHseIndList.Tables[0].Rows[i]["Dept"].ToString()),
                                General_Behavior = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["General_Behavior"].ToString()),
                                Site_Details_Responsibilities = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["Site_Details_Responsibilities"].ToString()),
                                Personal_Protective_Equipments_Compliance = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["Personal_Protective_Equipments_Compliance"].ToString()),
                                First_Aid = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["First_Aid"].ToString()),
                                Emergency_Assistance = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["Emergency_Assistance"].ToString()),
                                IMS = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["IMS"].ToString()),
                                Equipments_Procedures = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["Equipments_Procedures"].ToString()),
                                Remarks = dsHseIndList.Tables[0].Rows[i]["Remarks"].ToString(),
                                Induction_given_by = dsHseIndList.Tables[0].Rows[i]["Induction_given_by"].ToString(),
                                Project = objGlobaldata.GetDropdownitemById(dsHseIndList.Tables[0].Rows[i]["Project"].ToString()),
                                ReportNo = dsHseIndList.Tables[0].Rows[i]["ReportNo"].ToString(),
                                EmpType = dsHseIndList.Tables[0].Rows[i]["EmpType"].ToString(),
                                Visitors = objGlobaldata.GetMultiVisitorsByID(dsHseIndList.Tables[0].Rows[i]["Visitors"].ToString()),
                                Others = (dsHseIndList.Tables[0].Rows[i]["Others"].ToString()),
                            };
                            objHseIndModelsList.HseIndList.Add(objHseIndModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in HseIndList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HseIndList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objHseIndModelsList.HseIndList.ToList());
        }

        public JsonResult HseIndListSearch(string branch_name)
        {
            ViewBag.SubMenutype = "HseInd";
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

            HseIndModelsList objHseIndModelsList = new HseIndModelsList();
            objHseIndModelsList.HseIndList = new List<HseIndModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];


                string sSqlstmt = "SELECT id_Hse_insp,Hse_DateTime, Location, Employee, Dept, General_Behavior, Site_Details_Responsibilities, Personal_Protective_Equipments_Compliance," +
                    "First_Aid,Emergency_Assistance,IMS,Equipments_Procedures,Remarks,Induction_given_by,Project,ReportNo,EmpType,Visitors,Others"
                    + "  from t_hse_ind where Active='1'";

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
                DataSet dsHseIndList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsHseIndList.Tables.Count > 0)
                {

                    for (int i = 0; i < dsHseIndList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtHseDate = Convert.ToDateTime(dsHseIndList.Tables[0].Rows[i]["Hse_DateTime"].ToString());

                        try
                        {
                            HseIndModels objHseIndModels = new HseIndModels
                            {
                                id_Hse_insp = dsHseIndList.Tables[0].Rows[i]["id_Hse_insp"].ToString(),
                                Location = objGlobaldata.GetCompanyBranchNameById(dsHseIndList.Tables[0].Rows[i]["Location"].ToString()),
                                Hse_DateTime = dtHseDate,
                                Employee = objGlobaldata.GetMultiHrEmpNameById(dsHseIndList.Tables[0].Rows[i]["Employee"].ToString()),
                                Dept = objGlobaldata.GetDeptNameById(dsHseIndList.Tables[0].Rows[i]["Dept"].ToString()),
                                General_Behavior = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["General_Behavior"].ToString()),
                                Site_Details_Responsibilities = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["Site_Details_Responsibilities"].ToString()),
                                Personal_Protective_Equipments_Compliance = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["Personal_Protective_Equipments_Compliance"].ToString()),
                                First_Aid = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["First_Aid"].ToString()),
                                Emergency_Assistance = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["Emergency_Assistance"].ToString()),
                                IMS = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["IMS"].ToString()),
                                Equipments_Procedures = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[i]["Equipments_Procedures"].ToString()),
                                Remarks = dsHseIndList.Tables[0].Rows[i]["Remarks"].ToString(),
                                Induction_given_by = dsHseIndList.Tables[0].Rows[i]["Induction_given_by"].ToString(),
                                Project = objGlobaldata.GetDropdownitemById(dsHseIndList.Tables[0].Rows[i]["Project"].ToString()),
                                ReportNo = dsHseIndList.Tables[0].Rows[i]["ReportNo"].ToString(),
                                EmpType = dsHseIndList.Tables[0].Rows[i]["EmpType"].ToString(),
                                Visitors = objGlobaldata.GetMultiVisitorsByID(dsHseIndList.Tables[0].Rows[i]["Visitors"].ToString()),
                                Others = (dsHseIndList.Tables[0].Rows[i]["Others"].ToString()),
                            };
                            objHseIndModelsList.HseIndList.Add(objHseIndModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in HseIndListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HseIndListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        public ActionResult HseIndDetails()
        {
            ViewBag.SubMenutype = "HseInd";
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
            HseIndModels objComp = new HseIndModels();

            try
            {
                if (Request.QueryString["id_Hse_insp"] != null && Request.QueryString["id_Hse_insp"] != "")
                {
                    string sid_Hse_insp = Request.QueryString["id_Hse_insp"];
                    string sSqlstmt = "SELECT id_Hse_insp,Hse_DateTime, Location, Employee, Dept, General_Behavior, Site_Details_Responsibilities, Personal_Protective_Equipments_Compliance,First_Aid," +
                        "Emergency_Assistance,IMS,Equipments_Procedures,Remarks,Induction_given_by,Project,ReportNo,EmpType,Visitors,Others from t_hse_ind"
                    + " where id_Hse_insp='" + sid_Hse_insp + "'";

                    DataSet dsHseIndList = objGlobaldata.Getdetails(sSqlstmt);


                    if (dsHseIndList.Tables.Count > 0 && dsHseIndList.Tables[0].Rows.Count > 0)
                    {

                        DateTime dtHseDate = Convert.ToDateTime(dsHseIndList.Tables[0].Rows[0]["Hse_DateTime"].ToString());
                        objComp = new HseIndModels
                        {
                            id_Hse_insp = dsHseIndList.Tables[0].Rows[0]["id_Hse_insp"].ToString(),
                            Location = objGlobaldata.GetCompanyBranchNameById(dsHseIndList.Tables[0].Rows[0]["Location"].ToString()),
                            Hse_DateTime = dtHseDate,
                            Employee = objGlobaldata.GetMultiHrEmpNameById(dsHseIndList.Tables[0].Rows[0]["Employee"].ToString()),
                            Dept = objGlobaldata.GetDeptNameById(dsHseIndList.Tables[0].Rows[0]["Dept"].ToString()),
                            General_Behavior = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["General_Behavior"].ToString()),
                            Site_Details_Responsibilities = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Site_Details_Responsibilities"].ToString()),
                            Personal_Protective_Equipments_Compliance = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Personal_Protective_Equipments_Compliance"].ToString()),
                            First_Aid = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["First_Aid"].ToString()),
                            Emergency_Assistance = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Emergency_Assistance"].ToString()),
                            IMS = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["IMS"].ToString()),
                            Equipments_Procedures = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Equipments_Procedures"].ToString()),
                            Remarks = dsHseIndList.Tables[0].Rows[0]["Remarks"].ToString(),
                            Induction_given_by = dsHseIndList.Tables[0].Rows[0]["Induction_given_by"].ToString(),
                            Project = objGlobaldata.GetDropdownitemById(dsHseIndList.Tables[0].Rows[0]["Project"].ToString()),
                            ReportNo = dsHseIndList.Tables[0].Rows[0]["ReportNo"].ToString(),
                            EmpType = dsHseIndList.Tables[0].Rows[0]["EmpType"].ToString(),
                            Visitors = objGlobaldata.GetMultiVisitorsByID(dsHseIndList.Tables[0].Rows[0]["Visitors"].ToString()),
                            Others = (dsHseIndList.Tables[0].Rows[0]["Others"].ToString()),
                        };
                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("HseIndList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("HseIndList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HseIndDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("HseIndList");
            }
            return View(objComp);
        }

        public ActionResult HseIndInfo(int id)
        {
            HseIndModels objComp = new HseIndModels();

            try
            {
                ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

                string sSqlstmt = "SELECT id_Hse_insp,Hse_DateTime, Location, Employee, Dept, General_Behavior, Site_Details_Responsibilities, Personal_Protective_Equipments_Compliance," +
                    "First_Aid,Emergency_Assistance,IMS,Equipments_Procedures,Remarks,Induction_given_by,Project,ReportNo,EmpType,Visitors,Others from t_hse_ind"
            + " where id_Hse_insp='" + id + "'";

                DataSet dsHseIndList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsHseIndList.Tables.Count > 0 && dsHseIndList.Tables[0].Rows.Count > 0)
                {
                    DateTime dtHseDate = Convert.ToDateTime(dsHseIndList.Tables[0].Rows[0]["Hse_DateTime"].ToString());
                    objComp = new HseIndModels
                    {
                        id_Hse_insp = dsHseIndList.Tables[0].Rows[0]["id_Hse_insp"].ToString(),
                        Location = objGlobaldata.GetCompanyBranchNameById(dsHseIndList.Tables[0].Rows[0]["Location"].ToString()),
                        Hse_DateTime = dtHseDate,
                        Employee = objGlobaldata.GetMultiHrEmpNameById(dsHseIndList.Tables[0].Rows[0]["Employee"].ToString()),
                        Dept = objGlobaldata.GetDeptNameById(dsHseIndList.Tables[0].Rows[0]["Dept"].ToString()),
                        General_Behavior = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["General_Behavior"].ToString()),
                        Site_Details_Responsibilities = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Site_Details_Responsibilities"].ToString()),
                        Personal_Protective_Equipments_Compliance = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Personal_Protective_Equipments_Compliance"].ToString()),
                        First_Aid = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["First_Aid"].ToString()),
                        Emergency_Assistance = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Emergency_Assistance"].ToString()),
                        IMS = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["IMS"].ToString()),
                        Equipments_Procedures = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Equipments_Procedures"].ToString()),
                        Remarks = dsHseIndList.Tables[0].Rows[0]["Remarks"].ToString(),
                        Induction_given_by = dsHseIndList.Tables[0].Rows[0]["Induction_given_by"].ToString(),
                        Project = objGlobaldata.GetDropdownitemById(dsHseIndList.Tables[0].Rows[0]["Project"].ToString()),
                        ReportNo = dsHseIndList.Tables[0].Rows[0]["ReportNo"].ToString(),
                        EmpType = dsHseIndList.Tables[0].Rows[0]["EmpType"].ToString(),
                        Visitors = objGlobaldata.GetMultiVisitorsByID(dsHseIndList.Tables[0].Rows[0]["Visitors"].ToString()),
                        Others = (dsHseIndList.Tables[0].Rows[0]["Others"].ToString()),
                    };
                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("HseIndList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HseIndInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objComp);
        }

        [AllowAnonymous]
        public JsonResult HseIndDelete(FormCollection form)
        {
            try
            {
               
                    if (form["id_Hse_insp"] != null && form["id_Hse_insp"] != "")
                    {
                        HseIndModels Doc = new HseIndModels();
                        string sid_Hse_insp = form["id_Hse_insp"];

                        if (Doc.FunDeleteHseInd(sid_Hse_insp))
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
                        TempData["alertdata"] = "Hse Ind Id cannot be Null or empty";
                        return Json("Failed");
                    }                

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HseIndDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public ActionResult HseIndEdit()
        {
            ViewBag.SubMenutype = "HseInd";
           
            HseIndModels objComp = new HseIndModels();
            try
            {
                if (Request.QueryString["id_Hse_insp"] != null && Request.QueryString["id_Hse_insp"] != "")
                {

                    string sid_Hse_insp = Request.QueryString["id_Hse_insp"];
                    string sSqlstmt = "SELECT id_Hse_insp,Hse_DateTime, Location, Employee, Dept, General_Behavior, Site_Details_Responsibilities, Personal_Protective_Equipments_Compliance,First_Aid," +
                        "Emergency_Assistance,IMS,Equipments_Procedures,Remarks,Induction_given_by,Project,ReportNo,EmpType,Visitors,Others from t_hse_ind"
                        + " where id_Hse_insp=" + sid_Hse_insp;

                    DataSet dsHseIndList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsHseIndList.Tables.Count > 0 && dsHseIndList.Tables[0].Rows.Count > 0)
                    {

                        DateTime dtHseDate = Convert.ToDateTime(dsHseIndList.Tables[0].Rows[0]["Hse_DateTime"].ToString());
                        objComp = new HseIndModels
                        {
                            id_Hse_insp = dsHseIndList.Tables[0].Rows[0]["id_Hse_insp"].ToString(),
                            Location = objGlobaldata.GetCompanyBranchNameById(dsHseIndList.Tables[0].Rows[0]["Location"].ToString()),
                            Hse_DateTime = dtHseDate,
                            Employee = /*objGlobaldata.GetMultiHrEmpNameById*/(dsHseIndList.Tables[0].Rows[0]["Employee"].ToString()),
                            Dept = objGlobaldata.GetDeptNameById(dsHseIndList.Tables[0].Rows[0]["Dept"].ToString()),
                            General_Behavior = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["General_Behavior"].ToString()),
                            Site_Details_Responsibilities = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Site_Details_Responsibilities"].ToString()),
                            Personal_Protective_Equipments_Compliance = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Personal_Protective_Equipments_Compliance"].ToString()),
                            First_Aid = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["First_Aid"].ToString()),
                            Emergency_Assistance = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Emergency_Assistance"].ToString()),
                            IMS = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["IMS"].ToString()),
                            Equipments_Procedures = objHse.GetHseIndById(dsHseIndList.Tables[0].Rows[0]["Equipments_Procedures"].ToString()),
                            Remarks = dsHseIndList.Tables[0].Rows[0]["Remarks"].ToString(),
                            Induction_given_by = dsHseIndList.Tables[0].Rows[0]["Induction_given_by"].ToString(),
                            Project = objGlobaldata.GetDropdownitemById(dsHseIndList.Tables[0].Rows[0]["Project"].ToString()),
                            ReportNo = dsHseIndList.Tables[0].Rows[0]["ReportNo"].ToString(),
                            EmpType = dsHseIndList.Tables[0].Rows[0]["EmpType"].ToString(),
                            Visitors = /*objGlobaldata.GetMultiVisitorsByID*/(dsHseIndList.Tables[0].Rows[0]["Visitors"].ToString()),
                            Others = (dsHseIndList.Tables[0].Rows[0]["Others"].ToString()),
                        };

                        InitilizeHseInd();

                        return View(objComp);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("HseIndList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Hse Ind Id cannot be null";
                    return RedirectToAction("HseIndList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HseIndEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("HseIndList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HseIndEdit(HseIndModels objHseInd, FormCollection form)
        {
            try
            {
                objHseInd.Visitors = form["Visitors"];
                objHseInd.Employee = form["Employee"];
                DateTime dateValue;

                if (form["Hse_DateTime"] != null && DateTime.TryParse(form["Hse_DateTime"], out dateValue) == true)
                {
                    objHseInd.Hse_DateTime = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objHseInd.Hse_DateTime = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (objHseInd.FunUpdateHseInd(objHseInd))
                {
                    TempData["Successdata"] = "Updated Hse Ind details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HseIndEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("HseIndList");
        }

        [HttpPost]
        public JsonResult FunGetReportNo(string Location)
        {
            DataSet dsData; string RepNo = "";

            dsData = objGlobaldata.GetReportNo("HID","", Location);
            if (dsData != null && dsData.Tables.Count > 0)
            {
                RepNo = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
            }
            return Json(RepNo);
        }
    }
}