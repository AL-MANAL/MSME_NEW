using ISOStd.Filters;
using ISOStd.Models;
using PagedList;
using Rotativa;
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
    public class WorkPermitController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public WorkPermitController()
        {
            ViewBag.Menutype = "HSE";
        }

        public ActionResult AddAccessPermit()
        {
            ViewBag.SubMenutype = "AccessPermit";
            WorkPermitModels objPermit = new WorkPermitModels();
            try
            {
                objPermit.branch = objGlobaldata.GetCurrentUserSession().division;
                objPermit.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objPermit.location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objPermit.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objPermit.branch);
                ViewBag.Approvers = objGlobaldata.GetApprover();
                ViewBag.TimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.TimeInMin = objGlobaldata.GetAuditTimeInMin();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAccessPermit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPermit);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult AddAccessPermit(FormCollection form, WorkPermitModels objPermit)
        {
            try
            {
                string s = form["FromTimeInHour"];
                objPermit.branch = form["branch"];
                objPermit.location = form["location"];
                objPermit.Department = form["Department"];

                DateTime dateValue;
                if (DateTime.TryParse(form["date_issued"], out dateValue) == true)
                {
                    objPermit.date_issued = Convert.ToDateTime(dateValue.ToString("yyyy/MM/dd") + " " + form["FromTimeInHour"] + ":" + form["FromTimeInMin"]);
                }
                if (DateTime.TryParse(form["date_expired"], out dateValue) == true)
                {
                    objPermit.date_expired = Convert.ToDateTime(dateValue.ToString("yyyy/MM/dd") + " " + form["ToTimeInHour"] + ":" + form["ToTimeInMin"]);
                }

                WorkPermitModelsList objInList = new WorkPermitModelsList();
                objInList.PermitList = new List<WorkPermitModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["pers_name" + i] != null)
                    {
                        WorkPermitModels InModel = new WorkPermitModels();

                        InModel.pers_name = form["pers_name" + i];
                        InModel.idno = form["idno" + i];
                        if (DateTime.TryParse(form["date_in" + i], out dateValue) == true)
                        {
                            InModel.date_in = dateValue;
                        }
                        objInList.PermitList.Add(InModel);
                    }
                }

                WorkPermitModelsList objOutList = new WorkPermitModelsList();
                objOutList.PermitList = new List<WorkPermitModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["pers_name_out" + i] != null)
                    {
                        WorkPermitModels OutModel = new WorkPermitModels();
                        OutModel.pers_name_out = form["pers_name_out" + i];
                        OutModel.idno_out = form["idno_out" + i];
                        if (DateTime.TryParse(form["date_out" + i], out dateValue) == true)
                        {
                            OutModel.date_out = dateValue;
                        }
                        objOutList.PermitList.Add(OutModel);
                    }
                }

                if (objPermit.FunAddAccessPermit(objPermit, objInList, objOutList))
                {
                    TempData["Successdata"] = "Added Access Permit successfully with Access Permit No'" + objPermit.permitno + "'";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAccessPermit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AccessPermitList");
        }

        [AllowAnonymous]
        public ActionResult AccessPermitEdit()
        {
            ViewBag.SubMenutype = "AccessPermit";
            WorkPermitModels objPermit = new WorkPermitModels();
            try
            {
                ViewBag.Approvers = objGlobaldata.GetApprover();
                ViewBag.TimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.TimeInMin = objGlobaldata.GetAuditTimeInMin();

                if (Request.QueryString["id_access_permit"] != null && Request.QueryString["id_access_permit"] != "")
                {
                    string id_access_permit = Request.QueryString["id_access_permit"];
                    string sSqlstmt = "select id_access_permit,permitno,requestor,company,contactno,area,location,description,approved_by,"
                   + "(case when approve_status = '0' then 'Pending'  when approve_status = '1' then 'Approved' else 'Rejected' end) as approve_status,"
                        + "date_issued,date_expired,branch,Department from t_access_permit where id_access_permit='" + id_access_permit + "'";

                    DataSet dsPermitList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPermitList.Tables.Count > 0 && dsPermitList.Tables[0].Rows.Count > 0)
                    {
                        objPermit = new WorkPermitModels
                        {
                            id_access_permit = dsPermitList.Tables[0].Rows[0]["id_access_permit"].ToString(),
                            permitno = dsPermitList.Tables[0].Rows[0]["permitno"].ToString(),
                            requestor = dsPermitList.Tables[0].Rows[0]["requestor"].ToString(),
                            company = dsPermitList.Tables[0].Rows[0]["company"].ToString(),
                            contactno = dsPermitList.Tables[0].Rows[0]["contactno"].ToString(),
                            area = dsPermitList.Tables[0].Rows[0]["area"].ToString(),
                            location = dsPermitList.Tables[0].Rows[0]["location"].ToString(),
                            description = dsPermitList.Tables[0].Rows[0]["description"].ToString(),
                            approved_by = dsPermitList.Tables[0].Rows[0]["approved_by"].ToString(),
                            approve_status = dsPermitList.Tables[0].Rows[0]["approve_status"].ToString(),
                            branch = dsPermitList.Tables[0].Rows[0]["branch"].ToString(),
                            Department = dsPermitList.Tables[0].Rows[0]["Department"].ToString(),
                        };

                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsPermitList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsPermitList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();

                        DateTime dtDocDate;
                        if (dsPermitList.Tables[0].Rows[0]["date_issued"].ToString() != ""
                           && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["date_issued"].ToString(), out dtDocDate))
                        {
                            objPermit.date_issued = dtDocDate;
                        }
                        if (dsPermitList.Tables[0].Rows[0]["date_expired"].ToString() != ""
                        && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["date_expired"].ToString(), out dtDocDate))
                        {
                            objPermit.date_expired = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("AccessPermitList");
                    }

                    WorkPermitModelsList objInList = new WorkPermitModelsList();
                    objInList.PermitList = new List<WorkPermitModels>();

                    sSqlstmt = "select id_access_permit_in,id_access_permit,pers_name,idno,date_in from t_access_permit_in where id_access_permit='" + id_access_permit + "'";

                    DataSet dsInList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsInList.Tables.Count > 0 && dsInList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsInList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                WorkPermitModels objInPermit = new WorkPermitModels
                                {
                                    id_access_permit_in = dsInList.Tables[0].Rows[i]["id_access_permit_in"].ToString(),
                                    id_access_permit = dsInList.Tables[0].Rows[i]["id_access_permit"].ToString(),
                                    pers_name = dsInList.Tables[0].Rows[i]["pers_name"].ToString(),
                                    idno = dsInList.Tables[0].Rows[i]["idno"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsInList.Tables[0].Rows[i]["date_in"].ToString() != ""
                                && DateTime.TryParse(dsInList.Tables[0].Rows[i]["date_in"].ToString(), out dtDocDate))
                                {
                                    objInPermit.date_in = dtDocDate;
                                }
                                objInList.PermitList.Add(objInPermit);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AccessPermitEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AccessPermitList");
                            }
                        }
                        ViewBag.objInList = objInList;
                    }

                    WorkPermitModelsList objOutList = new WorkPermitModelsList();
                    objOutList.PermitList = new List<WorkPermitModels>();

                    sSqlstmt = "select id_access_permit_out,id_access_permit,pers_name_out,idno_out,date_out from t_access_permit_out where id_access_permit='" + id_access_permit + "'";

                    DataSet dsOutList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsOutList.Tables.Count > 0 && dsOutList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsOutList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                WorkPermitModels objOutPermit = new WorkPermitModels
                                {
                                    id_access_permit_out = dsOutList.Tables[0].Rows[i]["id_access_permit_out"].ToString(),
                                    id_access_permit = dsOutList.Tables[0].Rows[i]["id_access_permit"].ToString(),
                                    pers_name_out = dsOutList.Tables[0].Rows[i]["pers_name_out"].ToString(),
                                    idno_out = dsOutList.Tables[0].Rows[i]["idno_out"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsOutList.Tables[0].Rows[0]["date_out"].ToString() != ""
                                && DateTime.TryParse(dsOutList.Tables[0].Rows[0]["date_out"].ToString(), out dtDocDate))
                                {
                                    objOutPermit.date_out = dtDocDate;
                                }
                                objOutList.PermitList.Add(objOutPermit);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AccessPermitEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AccessPermitList");
                            }
                        }
                        ViewBag.objOutList = objOutList;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AccessPermitList");
            }
            return View(objPermit);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult AccessPermitEdit(FormCollection form, WorkPermitModels objPermit)
        {
            try
            {
                objPermit.branch = form["branch"];
                objPermit.location = form["location"];
                objPermit.Department = form["Department"];

                DateTime dateValue;
                if (DateTime.TryParse(form["date_issued"], out dateValue) == true)
                {
                    objPermit.date_issued = Convert.ToDateTime(dateValue.ToString("yyyy/MM/dd") + " " + form["FromTimeInHour"] + ":" + form["FromTimeInMin"]);
                }
                if (DateTime.TryParse(form["date_expired"], out dateValue) == true)
                {
                    objPermit.date_expired = Convert.ToDateTime(dateValue.ToString("yyyy/MM/dd") + " " + form["ToTimeInHour"] + ":" + form["ToTimeInMin"]);
                }

                WorkPermitModelsList objInList = new WorkPermitModelsList();
                objInList.PermitList = new List<WorkPermitModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["pers_name" + i] != null)
                    {
                        WorkPermitModels InModel = new WorkPermitModels();

                        InModel.pers_name = form["pers_name" + i];
                        InModel.idno = form["idno" + i];
                        if (DateTime.TryParse(form["date_in" + i], out dateValue) == true)
                        {
                            InModel.date_in = dateValue;
                        }
                        objInList.PermitList.Add(InModel);
                    }
                }

                WorkPermitModelsList objOutList = new WorkPermitModelsList();
                objOutList.PermitList = new List<WorkPermitModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["pers_name_out" + i] != null)
                    {
                        WorkPermitModels OutModel = new WorkPermitModels();

                        OutModel.pers_name_out = form["pers_name_out" + i];
                        OutModel.idno_out = form["idno_out" + i];
                        if (DateTime.TryParse(form["date_out" + i], out dateValue) == true)
                        {
                            OutModel.date_out = dateValue;
                        }
                        objOutList.PermitList.Add(OutModel);
                    }
                }

                if (objPermit.FunUpdateAccessPermit(objPermit, objInList, objOutList))
                {
                    TempData["Successdata"] = "Added  successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AccessPermitList");
        }

        [AllowAnonymous]
        public ActionResult AccessPermitOutEdit()
        {
            WorkPermitModels objPermit = new WorkPermitModels();
            try
            {
                ViewBag.Approvers = objGlobaldata.GetApprover();
                ViewBag.TimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.TimeInMin = objGlobaldata.GetAuditTimeInMin();

                if (Request.QueryString["id_access_permit"] != null && Request.QueryString["id_access_permit"] != "")
                {
                    string id_access_permit = Request.QueryString["id_access_permit"];
                    string sSqlstmt = "select id_access_permit,permitno,requestor,company,contactno,area,location,description,approved_by,"
                   + "(case when approve_status = '0' then 'Pending'  when approve_status = '1' then 'Approved' else 'Rejected' end) as approve_status,"
                        + "date_issued,date_expired from t_access_permit where id_access_permit='" + id_access_permit + "'";

                    DataSet dsPermitList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPermitList.Tables.Count > 0 && dsPermitList.Tables[0].Rows.Count > 0)
                    {
                        objPermit = new WorkPermitModels
                        {
                            id_access_permit = dsPermitList.Tables[0].Rows[0]["id_access_permit"].ToString(),
                            permitno = dsPermitList.Tables[0].Rows[0]["permitno"].ToString(),
                            requestor = dsPermitList.Tables[0].Rows[0]["requestor"].ToString(),
                            company = dsPermitList.Tables[0].Rows[0]["company"].ToString(),
                            contactno = dsPermitList.Tables[0].Rows[0]["contactno"].ToString(),
                            area = dsPermitList.Tables[0].Rows[0]["area"].ToString(),
                            location = dsPermitList.Tables[0].Rows[0]["location"].ToString(),
                            description = dsPermitList.Tables[0].Rows[0]["description"].ToString(),
                            approved_by = dsPermitList.Tables[0].Rows[0]["approved_by"].ToString(),
                            approve_status = dsPermitList.Tables[0].Rows[0]["approve_status"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsPermitList.Tables[0].Rows[0]["date_issued"].ToString() != ""
                           && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["date_issued"].ToString(), out dtDocDate))
                        {
                            objPermit.date_issued = dtDocDate;
                        }
                        if (dsPermitList.Tables[0].Rows[0]["date_expired"].ToString() != ""
                        && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["date_expired"].ToString(), out dtDocDate))
                        {
                            objPermit.date_expired = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("AccessPermitList");
                    }

                    WorkPermitModelsList objInList = new WorkPermitModelsList();
                    objInList.PermitList = new List<WorkPermitModels>();

                    sSqlstmt = "select id_access_permit_in,id_access_permit,pers_name,idno,date_in,date_out from t_access_permit_in where id_access_permit='" + id_access_permit + "'";

                    DataSet dsInList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsInList.Tables.Count > 0 && dsInList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsInList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                WorkPermitModels objInPermit = new WorkPermitModels
                                {
                                    id_access_permit_in = dsInList.Tables[0].Rows[i]["id_access_permit_in"].ToString(),
                                    id_access_permit = dsInList.Tables[0].Rows[i]["id_access_permit"].ToString(),
                                    pers_name = dsInList.Tables[0].Rows[i]["pers_name"].ToString(),
                                    idno = dsInList.Tables[0].Rows[i]["idno"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsInList.Tables[0].Rows[i]["date_in"].ToString() != ""
                                && DateTime.TryParse(dsInList.Tables[0].Rows[i]["date_in"].ToString(), out dtDocDate))
                                {
                                    objInPermit.date_in = dtDocDate;
                                }

                                if (dsInList.Tables[0].Rows[i]["date_out"].ToString() != ""
                               && DateTime.TryParse(dsInList.Tables[0].Rows[i]["date_out"].ToString(), out dtDocDate))
                                {
                                    objInPermit.date_out = dtDocDate;
                                }
                                objInList.PermitList.Add(objInPermit);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AccessPermitEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AccessPermitList");
                            }
                        }
                        ViewBag.objInList = objInList;
                    }

                    //WorkPermitModelsList objOutList = new WorkPermitModelsList();
                    //objOutList.PermitList = new List<WorkPermitModels>();

                    //sSqlstmt = "select id_access_permit_out,id_access_permit,pers_name_out,idno_out,date_out from t_access_permit_out where id_access_permit='" + id_access_permit + "'";

                    //DataSet dsOutList = objGlobaldata.Getdetails(sSqlstmt);
                    //if (dsOutList.Tables.Count > 0 && dsOutList.Tables[0].Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < dsOutList.Tables[0].Rows.Count; i++)
                    //    {
                    //        try
                    //        {
                    //            WorkPermitModels objOutPermit = new WorkPermitModels
                    //            {
                    //                id_access_permit_out = dsOutList.Tables[0].Rows[i]["id_access_permit_out"].ToString(),
                    //                id_access_permit = dsOutList.Tables[0].Rows[i]["id_access_permit"].ToString(),
                    //                pers_name_out = dsOutList.Tables[0].Rows[i]["pers_name_out"].ToString(),
                    //                idno_out = dsOutList.Tables[0].Rows[i]["idno_out"].ToString(),
                    //            };
                    //            DateTime dtDocDate;
                    //            if (dsOutList.Tables[0].Rows[0]["date_out"].ToString() != ""
                    //            && DateTime.TryParse(dsOutList.Tables[0].Rows[0]["date_out"].ToString(), out dtDocDate))
                    //            {
                    //                objOutPermit.date_out = dtDocDate;
                    //            }
                    //            objOutList.PermitList.Add(objOutPermit);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            objGlobaldata.AddFunctionalLog("Exception in AccessPermitEdit: " + ex.ToString());
                    //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    //            return RedirectToAction("AccessPermitList");
                    //        }
                    //    }
                    //    ViewBag.objOutList = objOutList;
                    //}
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AccessPermitList");
            }
            return View(objPermit);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult AccessPermitOutEdit(FormCollection form, WorkPermitModels objPermit)
        {
            try
            {
                DateTime dateValue;

                WorkPermitModelsList objOutList = new WorkPermitModelsList();
                objOutList.PermitList = new List<WorkPermitModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["pers_name" + i] != null)
                    {
                        WorkPermitModels OutModel = new WorkPermitModels();

                        OutModel.pers_name = form["pers_name" + i];
                        OutModel.idno = form["idno" + i];
                        if (DateTime.TryParse(form["date_in" + i], out dateValue) == true)
                        {
                            OutModel.date_in = dateValue;
                        }
                        if (DateTime.TryParse(form["date_out" + i], out dateValue) == true)
                        {
                            OutModel.date_out = dateValue;
                        }
                        objOutList.PermitList.Add(OutModel);
                    }
                }

                if (objPermit.FunAddOutPersAccessPermit(objPermit, objOutList))
                //if (objPermit.FunUpdateOutPersAccessPermit(objPermit, objOutList))
                {
                    TempData["Successdata"] = "Updated  successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitOutEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AccessPermitList");
        }

        [AllowAnonymous]
        public ActionResult AccessPermitList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "AccessPermit";
            WorkPermitModelsList objAccessList = new WorkPermitModelsList();
            objAccessList.PermitList = new List<WorkPermitModels>();

            try
            {
                string sSqlstmt = "select id_access_permit,permitno,requestor,company,contactno,area,location,date_issued,date_expired,"
                + " (case when approve_status = '0' then 'Pending'  when approve_status = '1' then 'Approved' else 'Rejected' end) as approve_status,branch,Department"
                + " from t_access_permit where active=1";
                string sSearchtext = "";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_access_permit desc";

                DataSet dsPermitList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsPermitList.Tables.Count > 0 && dsPermitList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsPermitList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            WorkPermitModels objPermitModels = new WorkPermitModels
                            {
                                id_access_permit = dsPermitList.Tables[0].Rows[i]["id_access_permit"].ToString(),
                                permitno = dsPermitList.Tables[0].Rows[i]["permitno"].ToString(),
                                requestor = dsPermitList.Tables[0].Rows[i]["requestor"].ToString(),
                                company = dsPermitList.Tables[0].Rows[i]["company"].ToString(),
                                contactno = dsPermitList.Tables[0].Rows[i]["contactno"].ToString(),
                                area = dsPermitList.Tables[0].Rows[i]["area"].ToString(),
                                approve_status = dsPermitList.Tables[0].Rows[i]["approve_status"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPermitList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsPermitList.Tables[0].Rows[i]["Department"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsPermitList.Tables[0].Rows[i]["location"].ToString()),
                            };
                            DateTime dtDocDate;
                            if (dsPermitList.Tables[0].Rows[i]["date_issued"].ToString() != ""
                               && DateTime.TryParse(dsPermitList.Tables[0].Rows[i]["date_issued"].ToString(), out dtDocDate))
                            {
                                objPermitModels.date_issued = dtDocDate;
                            }
                            if (dsPermitList.Tables[0].Rows[i]["date_expired"].ToString() != ""
                                && DateTime.TryParse(dsPermitList.Tables[0].Rows[i]["date_expired"].ToString(), out dtDocDate))
                            {
                                objPermitModels.date_expired = dtDocDate;
                            }
                            objAccessList.PermitList.Add(objPermitModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AccessPermitList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objAccessList.PermitList.ToList().ToPagedList(page ?? 1, 80000));
        }

        [AllowAnonymous]
        public JsonResult AccessPermitListSearch(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "AccessPermit";
            WorkPermitModelsList objAccessList = new WorkPermitModelsList();
            objAccessList.PermitList = new List<WorkPermitModels>();

            try
            {
                string sSqlstmt = "select id_access_permit,permitno,requestor,company,contactno,area,location,date_issued,date_expired,"
                + " (case when approve_status = '0' then 'Pending'  when approve_status = '1' then 'Approved' else 'Rejected' end) as approve_status,branch,Department"
                + " from t_access_permit where active=1";
                string sSearchtext = "";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_access_permit desc";

                DataSet dsPermitList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsPermitList.Tables.Count > 0 && dsPermitList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsPermitList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            WorkPermitModels objPermitModels = new WorkPermitModels
                            {
                                id_access_permit = dsPermitList.Tables[0].Rows[i]["id_access_permit"].ToString(),
                                permitno = dsPermitList.Tables[0].Rows[i]["permitno"].ToString(),
                                requestor = dsPermitList.Tables[0].Rows[i]["requestor"].ToString(),
                                company = dsPermitList.Tables[0].Rows[i]["company"].ToString(),
                                contactno = dsPermitList.Tables[0].Rows[i]["contactno"].ToString(),
                                area = dsPermitList.Tables[0].Rows[i]["area"].ToString(),
                                approve_status = dsPermitList.Tables[0].Rows[i]["approve_status"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPermitList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsPermitList.Tables[0].Rows[i]["Department"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsPermitList.Tables[0].Rows[i]["location"].ToString()),
                            };
                            DateTime dtDocDate;
                            if (dsPermitList.Tables[0].Rows[i]["date_issued"].ToString() != ""
                               && DateTime.TryParse(dsPermitList.Tables[0].Rows[i]["date_issued"].ToString(), out dtDocDate))
                            {
                                objPermitModels.date_issued = dtDocDate;
                            }
                            if (dsPermitList.Tables[0].Rows[i]["date_expired"].ToString() != ""
                                && DateTime.TryParse(dsPermitList.Tables[0].Rows[i]["date_expired"].ToString(), out dtDocDate))
                            {
                                objPermitModels.date_expired = dtDocDate;
                            }
                            objAccessList.PermitList.Add(objPermitModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AccessPermitListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        public ActionResult AccessPermitApproveReject(string id_access_permit, string iStatus, string PendingFlg, string approver_comment)
        {
            try
            {
                WorkPermitModels objModel = new WorkPermitModels();

                string sStatus = "";
                if (iStatus == "0")
                {
                    sStatus = "Pending";
                }
                else if (iStatus == "1")
                {
                    sStatus = "Approved";
                }
                else if (iStatus == "2")
                {
                    sStatus = "Rejected";
                }
                if (objModel.FunAccessPermitApproveOrReject(id_access_permit, iStatus, approver_comment))
                {
                    TempData["Successdata"] = "Access work permit" + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("WorkPermitList");
            }
        }

        [AllowAnonymous]
        public ActionResult AccessPermitDetail()
        {
            ViewBag.SubMenutype = "AccessPermit";
            WorkPermitModels objPermit = new WorkPermitModels();
            try
            {
                ViewBag.Approvers = objGlobaldata.GetApprover();
                if (Request.QueryString["id_access_permit"] != null && Request.QueryString["id_access_permit"] != "")
                {
                    string id_access_permit = Request.QueryString["id_access_permit"];
                    string sSqlstmt = "select id_access_permit,permitno,requestor,company,contactno,area,location,description,approved_by,"
                        + " (case when approve_status = '0' then 'Pending'  when approve_status = '1' then 'Approved' else 'Rejected' end) as approve_status,"
                    + "date_issued,date_expired,branch,Department from t_access_permit where id_access_permit='" + id_access_permit + "'";

                    DataSet dsPermitList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPermitList.Tables.Count > 0 && dsPermitList.Tables[0].Rows.Count > 0)
                    {
                        objPermit = new WorkPermitModels
                        {
                            id_access_permit = dsPermitList.Tables[0].Rows[0]["id_access_permit"].ToString(),
                            permitno = dsPermitList.Tables[0].Rows[0]["permitno"].ToString(),
                            requestor = dsPermitList.Tables[0].Rows[0]["requestor"].ToString(),
                            company = dsPermitList.Tables[0].Rows[0]["company"].ToString(),
                            contactno = dsPermitList.Tables[0].Rows[0]["contactno"].ToString(),
                            area = dsPermitList.Tables[0].Rows[0]["area"].ToString(),
                            description = dsPermitList.Tables[0].Rows[0]["description"].ToString(),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsPermitList.Tables[0].Rows[0]["approved_by"].ToString()),
                            approve_status = dsPermitList.Tables[0].Rows[0]["approve_status"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPermitList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsPermitList.Tables[0].Rows[0]["Department"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsPermitList.Tables[0].Rows[0]["location"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsPermitList.Tables[0].Rows[0]["date_issued"].ToString() != ""
                           && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["date_issued"].ToString(), out dtDocDate))
                        {
                            objPermit.date_issued = dtDocDate;
                        }
                        if (dsPermitList.Tables[0].Rows[0]["date_expired"].ToString() != ""
                        && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["date_expired"].ToString(), out dtDocDate))
                        {
                            objPermit.date_expired = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("AccessPermitList");
                    }

                    WorkPermitModelsList objInList = new WorkPermitModelsList();
                    objInList.PermitList = new List<WorkPermitModels>();

                    sSqlstmt = "select id_access_permit_in,id_access_permit,pers_name,idno,date_in,date_out from t_access_permit_in where id_access_permit='" + id_access_permit + "'";

                    DataSet dsInList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsInList.Tables.Count > 0 && dsInList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsInList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                WorkPermitModels objInPermit = new WorkPermitModels
                                {
                                    id_access_permit_in = dsInList.Tables[0].Rows[i]["id_access_permit_in"].ToString(),
                                    id_access_permit = dsInList.Tables[0].Rows[i]["id_access_permit"].ToString(),
                                    pers_name = dsInList.Tables[0].Rows[i]["pers_name"].ToString(),
                                    idno = dsInList.Tables[0].Rows[i]["idno"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsInList.Tables[0].Rows[i]["date_in"].ToString() != ""
                                && DateTime.TryParse(dsInList.Tables[0].Rows[i]["date_in"].ToString(), out dtDocDate))
                                {
                                    objInPermit.date_in = dtDocDate;
                                }
                                if (dsInList.Tables[0].Rows[i]["date_out"].ToString() != ""
                               && DateTime.TryParse(dsInList.Tables[0].Rows[i]["date_out"].ToString(), out dtDocDate))
                                {
                                    objInPermit.date_out = dtDocDate;
                                }

                                objInList.PermitList.Add(objInPermit);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AccessPermitEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AccessPermitList");
                            }
                        }
                        ViewBag.objInList = objInList;
                    }

                    //WorkPermitModelsList objOutList = new WorkPermitModelsList();
                    //objOutList.PermitList = new List<WorkPermitModels>();

                    //sSqlstmt = "select id_access_permit_out,id_access_permit,pers_name_out,idno_out,date_out from t_access_permit_out where id_access_permit='" + id_access_permit + "'";

                    //DataSet dsOutList = objGlobaldata.Getdetails(sSqlstmt);
                    //if (dsOutList.Tables.Count > 0 && dsOutList.Tables[0].Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < dsOutList.Tables[0].Rows.Count; i++)
                    //    {
                    //        try
                    //        {
                    //            WorkPermitModels objOutPermit = new WorkPermitModels
                    //            {
                    //                id_access_permit_out = dsOutList.Tables[0].Rows[i]["id_access_permit_out"].ToString(),
                    //                id_access_permit = dsOutList.Tables[0].Rows[i]["id_access_permit"].ToString(),
                    //                pers_name_out = dsOutList.Tables[0].Rows[i]["pers_name_out"].ToString(),
                    //                idno_out = dsOutList.Tables[0].Rows[i]["idno_out"].ToString(),
                    //            };
                    //            DateTime dtDocDate;
                    //            if (dsOutList.Tables[0].Rows[0]["date_out"].ToString() != ""
                    //            && DateTime.TryParse(dsOutList.Tables[0].Rows[0]["date_out"].ToString(), out dtDocDate))
                    //            {
                    //                objOutPermit.date_out = dtDocDate;
                    //            }
                    //            objOutList.PermitList.Add(objOutPermit);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            objGlobaldata.AddFunctionalLog("Exception in AccessPermitDetail: " + ex.ToString());
                    //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    //            return RedirectToAction("AccessPermitList");
                    //        }
                    //    }
                    //    ViewBag.objOutList = objOutList;
                    //}
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AccessPermitList");
            }
            return View(objPermit);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AccessPermitDetail(FormCollection form)
        {
            WorkPermitModels objPermit = new WorkPermitModels();
            try
            {
                string id_access_permit = form["id_access_permit"];
                if (id_access_permit != null && id_access_permit != "")
                {
                    string sSqlstmt = "select id_access_permit,permitno,requestor,company,contactno,area,location,description,approved_by,"
                        + " (case when approve_status = '0' then 'Pending'  when approve_status = '1' then 'Approved' else 'Rejected' end) as approve_status,"
                    + "date_issued,date_expired,loggedby,date_submitted,approved_date,branch,Department from t_access_permit where id_access_permit='" + id_access_permit + "'";

                    DataSet dsPermitList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPermitList.Tables.Count > 0 && dsPermitList.Tables[0].Rows.Count > 0)
                    {
                        objPermit = new WorkPermitModels
                        {
                            id_access_permit = dsPermitList.Tables[0].Rows[0]["id_access_permit"].ToString(),
                            permitno = dsPermitList.Tables[0].Rows[0]["permitno"].ToString(),
                            requestor = dsPermitList.Tables[0].Rows[0]["requestor"].ToString(),
                            company = dsPermitList.Tables[0].Rows[0]["company"].ToString(),
                            contactno = dsPermitList.Tables[0].Rows[0]["contactno"].ToString(),
                            area = dsPermitList.Tables[0].Rows[0]["area"].ToString(),
                            description = dsPermitList.Tables[0].Rows[0]["description"].ToString(),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsPermitList.Tables[0].Rows[0]["approved_by"].ToString()),
                            approve_status = dsPermitList.Tables[0].Rows[0]["approve_status"].ToString(),
                            loggedby = dsPermitList.Tables[0].Rows[0]["loggedby"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPermitList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsPermitList.Tables[0].Rows[0]["Department"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsPermitList.Tables[0].Rows[0]["location"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsPermitList.Tables[0].Rows[0]["date_issued"].ToString() != ""
                           && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["date_issued"].ToString(), out dtDocDate))
                        {
                            objPermit.date_issued = dtDocDate;
                        }
                        if (dsPermitList.Tables[0].Rows[0]["date_expired"].ToString() != ""
                        && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["date_expired"].ToString(), out dtDocDate))
                        {
                            objPermit.date_expired = dtDocDate;
                        }
                        if (dsPermitList.Tables[0].Rows[0]["date_submitted"].ToString() != ""
                     && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["date_submitted"].ToString(), out dtDocDate))
                        {
                            objPermit.date_submitted = dtDocDate;
                        }
                        if (dsPermitList.Tables[0].Rows[0]["approved_date"].ToString() != ""
                && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["approved_date"].ToString(), out dtDocDate))
                        {
                            objPermit.approved_date = dtDocDate;
                        }

                        CompanyModels objCompany = new CompanyModels();
                        dsPermitList = objCompany.GetCompanyDetailsForReport(dsPermitList);

                        dsPermitList = objGlobaldata.GetReportDetails(dsPermitList, objPermit.permitno, objPermit.loggedby, "ACCESS PERMIT REPORT");
                        ViewBag.CompanyInfo = dsPermitList;

                        ViewBag.Permit = objPermit;
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("AccessPermitList");
                    }

                    WorkPermitModelsList objInList = new WorkPermitModelsList();
                    objInList.PermitList = new List<WorkPermitModels>();

                    sSqlstmt = "select id_access_permit_in,id_access_permit,pers_name,idno,date_in,date_out from t_access_permit_in where id_access_permit='" + id_access_permit + "'";

                    DataSet dsInList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsInList.Tables.Count > 0 && dsInList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsInList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                WorkPermitModels objInPermit = new WorkPermitModels
                                {
                                    id_access_permit_in = dsInList.Tables[0].Rows[i]["id_access_permit_in"].ToString(),
                                    id_access_permit = dsInList.Tables[0].Rows[i]["id_access_permit"].ToString(),
                                    pers_name = dsInList.Tables[0].Rows[i]["pers_name"].ToString(),
                                    idno = dsInList.Tables[0].Rows[i]["idno"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsInList.Tables[0].Rows[i]["date_in"].ToString() != ""
                                && DateTime.TryParse(dsInList.Tables[0].Rows[i]["date_in"].ToString(), out dtDocDate))
                                {
                                    objInPermit.date_in = dtDocDate;
                                }
                                if (dsInList.Tables[0].Rows[i]["date_out"].ToString() != ""
                               && DateTime.TryParse(dsInList.Tables[0].Rows[i]["date_out"].ToString(), out dtDocDate))
                                {
                                    objInPermit.date_out = dtDocDate;
                                }
                                objInList.PermitList.Add(objInPermit);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AccessPermitEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AccessPermitList");
                            }
                        }
                        ViewBag.objInList = objInList;
                    }

                    //WorkPermitModelsList objOutList = new WorkPermitModelsList();
                    //objOutList.PermitList = new List<WorkPermitModels>();

                    //sSqlstmt = "select id_access_permit_out,id_access_permit,pers_name_out,idno_out,date_out from t_access_permit_out where id_access_permit='" + id_access_permit + "'";

                    //DataSet dsOutList = objGlobaldata.Getdetails(sSqlstmt);
                    //if (dsOutList.Tables.Count > 0 && dsOutList.Tables[0].Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < dsOutList.Tables[0].Rows.Count; i++)
                    //    {
                    //        try
                    //        {
                    //            WorkPermitModels objOutPermit = new WorkPermitModels
                    //            {
                    //                id_access_permit_out = dsOutList.Tables[0].Rows[i]["id_access_permit_out"].ToString(),
                    //                id_access_permit = dsOutList.Tables[0].Rows[i]["id_access_permit"].ToString(),
                    //                pers_name_out = dsOutList.Tables[0].Rows[i]["pers_name_out"].ToString(),
                    //                idno_out = dsOutList.Tables[0].Rows[i]["idno_out"].ToString(),
                    //            };
                    //            DateTime dtDocDate;
                    //            if (dsOutList.Tables[0].Rows[0]["date_out"].ToString() != ""
                    //            && DateTime.TryParse(dsOutList.Tables[0].Rows[0]["date_out"].ToString(), out dtDocDate))
                    //            {
                    //                objOutPermit.date_out = dtDocDate;
                    //            }
                    //            objOutList.PermitList.Add(objOutPermit);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            objGlobaldata.AddFunctionalLog("Exception in AccessPermitDetail: " + ex.ToString());
                    //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    //            return RedirectToAction("AccessPermitList");
                    //        }
                    //    }
                    //    ViewBag.objOutList = objOutList;
                    //}
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AccessPermitList");
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("AccessPermitPdf")
            {
                //FileName = "InspectionChecklistReport.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [HttpGet]
        public ActionResult AddWorkPermit()
        {
            ViewBag.SubMenutype = "WorkPermit";
            WorkPermitModels objPermit = new WorkPermitModels();
            try
            {
                objPermit.branch = objGlobaldata.GetCurrentUserSession().division;
                objPermit.dept = objGlobaldata.GetCurrentUserSession().DeptID;
                objPermit.location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objPermit.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objPermit.branch);

                ViewBag.Type = objGlobaldata.getWorkPermitTypeList();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Rating = objGlobaldata.GetPermitRating();
                ViewBag.work = objGlobaldata.GetConstantValue("Permitworktype");
                ViewBag.HydroTest = objGlobaldata.getPermitQuestionList("Hydro Test");
                // ViewBag.Dept = objGlobaldata.GetDepartmentListbox();
                ViewBag.PermitLocation = objGlobaldata.GetDropdownList("Work Permit Location");

                ViewBag.Pneumatic = objGlobaldata.getPermitQuestionList("Pneumatic");

                ViewBag.Electrical = objGlobaldata.getPermitQuestionList("Electrical");

                ViewBag.Hazard = objGlobaldata.getPermitSectionQuestionList("Excavation", "Hazards Identified");
                ViewBag.WorkSite = objGlobaldata.getPermitSectionQuestionList("Excavation", "Work Site Precautions");
                ViewBag.Safety = objGlobaldata.getPermitSectionQuestionList("Excavation", "Personal Protection and Safety Equipment ");

                ViewBag.HotType = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Type of work");
                ViewBag.HotHazard = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Hazards Identified");
                ViewBag.Welfare = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Welfare");
                ViewBag.HotWorkSite = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Work Site Precautions");

                ViewBag.FalseHazard = objGlobaldata.getPermitSectionQuestionList("False Work", "Hazards Identified");
                ViewBag.FalseWorkSite = objGlobaldata.getPermitSectionQuestionList("False Work", "Work Site Precautions");
                ViewBag.FalseSafety = objGlobaldata.getPermitSectionQuestionList("False Work", "Personal Protection and Safety Equipment");

                ViewBag.ShaftHazard = objGlobaldata.getPermitSectionQuestionList("Shaft Work", "Hazards Identified");
                ViewBag.ShaftWorkSite = objGlobaldata.getPermitSectionQuestionList("Shaft Work", "Work Site Precautions");

                ViewBag.Confined = objGlobaldata.getPermitQuestionList("Confined");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddWorkPermit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPermit);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult AddWorkPermit(FormCollection form, WorkPermitModels objPermit, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objPermit.branch = form["branch"];
                objPermit.dept = form["dept"];
                objPermit.location = form["location"];
                objPermit.permit_location = form["permit_location"];

                DateTime dateValue;

                if (DateTime.TryParse(form["start_date"], out dateValue) == true)
                {
                    objPermit.start_date = dateValue;
                }
                if (DateTime.TryParse(form["finish_date"], out dateValue) == true)
                {
                    objPermit.finish_date = dateValue;
                }
                if (form["start_date"] != null && DateTime.TryParse(form["start_date"], out dateValue) == true)
                {
                    objPermit.start_date = dateValue;
                    int iHr, iMin;
                    if (form["StartTimeInHour"] != null && int.TryParse(form["StartTimeInHour"], out iHr) &&
                        form["StartTimeInMin"] != null && int.TryParse(form["StartTimeInMin"], out iMin))
                    {
                        objPermit.start_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (form["finish_date"] != null && DateTime.TryParse(form["finish_date"], out dateValue) == true)
                {
                    objPermit.finish_date = dateValue;
                    int iHr, iMin;
                    if (form["FinishTimeInHour"] != null && int.TryParse(form["FinishTimeInHour"], out iHr) &&
                        form["FinishTimeInMin"] != null && int.TryParse(form["FinishTimeInMin"], out iMin))
                    {
                        objPermit.finish_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase files = Request.Files[0];
                    if (upload != null && files.ContentLength > 0)
                    {
                        objPermit.upload = "";
                        foreach (var file in upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Permit"), Path.GetFileName(file.FileName));
                                string sFilename = "Permit" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objPermit.upload = objPermit.upload + "," + "~/DataUpload/MgmtDocs/Permit/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddWorkPermit-upload: " + ex.ToString());
                            }
                        }
                        objPermit.upload = objPermit.upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                }
                WorkPermitModelsList objInList = new WorkPermitModelsList();
                objInList.PermitList = new List<WorkPermitModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["pers_name" + i] != null)
                    {
                        WorkPermitModels InModel = new WorkPermitModels();

                        InModel.pers_name = form["pers_name" + i];
                        InModel.idno = form["idno" + i];
                        if (DateTime.TryParse(form["date_in" + i], out dateValue) == true)
                        {
                            InModel.date_in = dateValue;
                        }
                        objInList.PermitList.Add(InModel);
                    }
                }

                //Hydro
                MultiSelectList HydroTest = objGlobaldata.getPermitQuestionList("Hydro Test");

                MultiSelectList Pneumatic = objGlobaldata.getPermitQuestionList("Pneumatic");

                MultiSelectList Electrical = objGlobaldata.getPermitQuestionList("Electrical");

                MultiSelectList Confined = objGlobaldata.getPermitQuestionList("Confined");

                //Excavation
                MultiSelectList Hazard = objGlobaldata.getPermitSectionQuestionList("Excavation", "Hazards Identified");
                MultiSelectList WorkSite = objGlobaldata.getPermitSectionQuestionList("Excavation", "Work Site Precautions");
                MultiSelectList Safety = objGlobaldata.getPermitSectionQuestionList("Excavation", "Personal Protection and Safety Equipment ");

                //hot work
                MultiSelectList HotType = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Type of work");
                MultiSelectList HotHazard = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Hazards Identified");
                MultiSelectList Welfare = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Welfare");
                MultiSelectList HotWorkSite = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Work Site Precautions");

                //False work
                MultiSelectList FalseHazard = objGlobaldata.getPermitSectionQuestionList("False Work", "Hazards Identified");
                MultiSelectList FalseWorkSite = objGlobaldata.getPermitSectionQuestionList("False Work", "Work Site Precautions");
                MultiSelectList FalseSafety = objGlobaldata.getPermitSectionQuestionList("False Work", "Personal Protection and Safety Equipment");

                MultiSelectList ShaftHazard = objGlobaldata.getPermitSectionQuestionList("Shaft Work", "Hazards Identified");
                MultiSelectList ShaftWorkSite = objGlobaldata.getPermitSectionQuestionList("Shaft Work", "Work Site Precautions");

                //Confined
                WorkPermitModelsList objConfinedList = new WorkPermitModelsList();
                objConfinedList.PermitList = new List<WorkPermitModels>();

                int c = 1;
                foreach (var item in Confined)
                {
                    if (c <= Convert.ToInt16(form["cnt18"]))
                    {
                        if (form["id_ratings18 " + item.Value] != null)
                        {
                            WorkPermitModels objConfined = new WorkPermitModels();
                            objConfined.id_questions = form["question18 " + item.Value];
                            objConfined.id_ratings = form["id_ratings18 " + item.Value];
                            objConfinedList.PermitList.Add(objConfined);
                        }
                    }
                    c++;
                }

                //Hydro
                WorkPermitModelsList objHydroList = new WorkPermitModelsList();
                objHydroList.PermitList = new List<WorkPermitModels>();

                int j = 1;
                foreach (var item in HydroTest)
                {
                    if (j <= Convert.ToInt16(form["cnt1"]))
                    {
                        if (form["id_ratings1 " + item.Value] != null)
                        {
                            WorkPermitModels objHydro = new WorkPermitModels();
                            objHydro.id_questions = form["question1 " + item.Value];
                            objHydro.id_ratings = form["id_ratings1 " + item.Value];
                            objHydroList.PermitList.Add(objHydro);
                        }
                    }
                    j++;
                }

                //Excavation
                WorkPermitModelsList objExcvHazardList = new WorkPermitModelsList();
                objExcvHazardList.PermitList = new List<WorkPermitModels>();

                int k = 1;
                foreach (var item in Hazard)
                {
                    if (k <= Convert.ToInt16(form["  "]))
                    {
                        if (form["id_ratings2 " + item.Value] != null)
                        {
                            WorkPermitModels objExcvHazard = new WorkPermitModels();
                            objExcvHazard.id_questions = form["question2 " + item.Value];
                            objExcvHazard.id_ratings = form["id_ratings2 " + item.Value];
                            objExcvHazardList.PermitList.Add(objExcvHazard);
                        }
                    }
                    k++;
                }

                WorkPermitModelsList objExcvSiteList = new WorkPermitModelsList();
                objExcvSiteList.PermitList = new List<WorkPermitModels>();

                int l = 1;
                foreach (var item in WorkSite)
                {
                    if (l <= Convert.ToInt16(form["cnt3"]))
                    {
                        if (form["id_ratings3 " + item.Value] != null)
                        {
                            WorkPermitModels objExcvSite = new WorkPermitModels();
                            objExcvSite.id_questions = form["question3 " + item.Value];
                            objExcvSite.id_ratings = form["id_ratings3 " + item.Value];
                            objExcvSiteList.PermitList.Add(objExcvSite);
                        }
                    }
                    l++;
                }

                WorkPermitModelsList objExcvSafetyList = new WorkPermitModelsList();
                objExcvSafetyList.PermitList = new List<WorkPermitModels>();

                int m = 1;
                foreach (var item in Safety)
                {
                    if (m <= Convert.ToInt16(form["cnt3"]))
                    {
                        if (form["id_ratings4 " + item.Value] != null)
                        {
                            WorkPermitModels objExcvSafety = new WorkPermitModels();
                            objExcvSafety.id_questions = form["question4 " + item.Value];
                            objExcvSafety.id_ratings = form["id_ratings4 " + item.Value];
                            objExcvSafetyList.PermitList.Add(objExcvSafety);
                        }
                    }
                    m++;
                }

                //Hot work
                WorkPermitModelsList objHotTypeList = new WorkPermitModelsList();
                objHotTypeList.PermitList = new List<WorkPermitModels>();

                int n = 1;
                foreach (var item in HotType)
                {
                    if (n <= Convert.ToInt16(form["cnt5"]))
                    {
                        if (form["id_ratings5 " + item.Value] != null)
                        {
                            WorkPermitModels objHotType = new WorkPermitModels();
                            objHotType.id_questions = form["question5 " + item.Value];
                            objHotType.id_ratings = form["id_ratings5 " + item.Value];
                            objHotTypeList.PermitList.Add(objHotType);
                        }
                    }
                    n++;
                }

                WorkPermitModelsList objHotHazardList = new WorkPermitModelsList();
                objHotHazardList.PermitList = new List<WorkPermitModels>();

                int o = 1;
                foreach (var item in HotHazard)
                {
                    if (o <= Convert.ToInt16(form["cnt6"]))
                    {
                        if (form["id_ratings6 " + item.Value] != null)
                        {
                            WorkPermitModels objHotHazard = new WorkPermitModels();
                            objHotHazard.id_questions = form["question6 " + item.Value];
                            objHotHazard.id_ratings = form["id_ratings6 " + item.Value];
                            objHotHazardList.PermitList.Add(objHotHazard);
                        }
                    }
                    o++;
                }

                WorkPermitModelsList objHotWelfareList = new WorkPermitModelsList();
                objHotWelfareList.PermitList = new List<WorkPermitModels>();

                int p = 1;
                foreach (var item in Welfare)
                {
                    if (p <= Convert.ToInt16(form["cnt7"]))
                    {
                        if (form["id_ratings7 " + item.Value] != null)
                        {
                            WorkPermitModels HotWelfare = new WorkPermitModels();
                            HotWelfare.id_questions = form["question7 " + item.Value];
                            HotWelfare.id_ratings = form["id_ratings7 " + item.Value];
                            objHotWelfareList.PermitList.Add(HotWelfare);
                        }
                    }
                    p++;
                }

                WorkPermitModelsList objHotSiteList = new WorkPermitModelsList();
                objHotSiteList.PermitList = new List<WorkPermitModels>();

                int q = 1;
                foreach (var item in HotWorkSite)
                {
                    if (q <= Convert.ToInt16(form["cnt8"]))
                    {
                        if (form["id_ratings8 " + item.Value] != null)
                        {
                            WorkPermitModels objHotSite = new WorkPermitModels();
                            objHotSite.id_questions = form["question8 " + item.Value];
                            objHotSite.id_ratings = form["id_ratings8 " + item.Value];
                            objHotSiteList.PermitList.Add(objHotSite);
                        }
                    }
                    q++;
                }

                WorkPermitModelsList objHotSafetyList = new WorkPermitModelsList();
                objHotSafetyList.PermitList = new List<WorkPermitModels>();

                int r = 1;
                foreach (var item in Safety)
                {
                    if (r <= Convert.ToInt16(form["cnt9"]))
                    {
                        if (form["id_ratings9 " + item.Value] != null)
                        {
                            WorkPermitModels objHotSafety = new WorkPermitModels();
                            objHotSafety.id_questions = form["question9 " + item.Value];
                            objHotSafety.id_ratings = form["id_ratings9 " + item.Value];
                            objHotSafetyList.PermitList.Add(objHotSafety);
                        }
                    }
                    r++;
                }

                //False work
                WorkPermitModelsList objFalseHazardList = new WorkPermitModelsList();
                objFalseHazardList.PermitList = new List<WorkPermitModels>();

                int s = 1;
                foreach (var item in FalseHazard)
                {
                    if (s <= Convert.ToInt16(form["cnt10"]))
                    {
                        if (form["id_ratings10 " + item.Value] != null)
                        {
                            WorkPermitModels objFalseHazard = new WorkPermitModels();
                            objFalseHazard.id_questions = form["question10 " + item.Value];
                            objFalseHazard.id_ratings = form["id_ratings10 " + item.Value];
                            objFalseHazardList.PermitList.Add(objFalseHazard);
                        }
                    }
                    s++;
                }

                WorkPermitModelsList objFalseSiteList = new WorkPermitModelsList();
                objFalseSiteList.PermitList = new List<WorkPermitModels>();

                int t = 1;
                foreach (var item in FalseWorkSite)
                {
                    if (t <= Convert.ToInt16(form["cnt11"]))
                    {
                        if (form["id_ratings11 " + item.Value] != null)
                        {
                            WorkPermitModels objFalseSite = new WorkPermitModels();
                            objFalseSite.id_questions = form["question11 " + item.Value];
                            objFalseSite.id_ratings = form["id_ratings11 " + item.Value];
                            objFalseSiteList.PermitList.Add(objFalseSite);
                        }
                    }
                    t++;
                }

                WorkPermitModelsList objFalseSafetyList = new WorkPermitModelsList();
                objFalseSafetyList.PermitList = new List<WorkPermitModels>();

                int u = 1;
                foreach (var item in FalseSafety)
                {
                    if (u <= Convert.ToInt16(form["cnt12"]))
                    {
                        if (form["id_ratings12 " + item.Value] != null)
                        {
                            WorkPermitModels objFalseSafety = new WorkPermitModels();
                            objFalseSafety.id_questions = form["question12 " + item.Value];
                            objFalseSafety.id_ratings = form["id_ratings12 " + item.Value];
                            objFalseSafetyList.PermitList.Add(objFalseSafety);
                        }
                    }
                    u++;
                }
                //shaft work

                WorkPermitModelsList objShaftHazardList = new WorkPermitModelsList();
                objShaftHazardList.PermitList = new List<WorkPermitModels>();

                int v = 1;
                foreach (var item in ShaftHazard)
                {
                    if (v <= Convert.ToInt16(form["cnt13"]))
                    {
                        if (form["id_ratings13 " + item.Value] != null)
                        {
                            WorkPermitModels objShaftHazard = new WorkPermitModels();
                            objShaftHazard.id_questions = form["question13 " + item.Value];
                            objShaftHazard.id_ratings = form["id_ratings13 " + item.Value];
                            objShaftHazardList.PermitList.Add(objShaftHazard);
                        }
                    }
                    v++;
                }

                WorkPermitModelsList objShaftSiteList = new WorkPermitModelsList();
                objShaftSiteList.PermitList = new List<WorkPermitModels>();

                int w = 1;
                foreach (var item in ShaftWorkSite)
                {
                    if (w <= Convert.ToInt16(form["cnt14"]))
                    {
                        if (form["id_ratings14 " + item.Value] != null)
                        {
                            WorkPermitModels objShaftSite = new WorkPermitModels();
                            objShaftSite.id_questions = form["question14 " + item.Value];
                            objShaftSite.id_ratings = form["id_ratings14 " + item.Value];
                            objShaftSiteList.PermitList.Add(objShaftSite);
                        }
                    }
                    w++;
                }

                WorkPermitModelsList objShaftSafetyList = new WorkPermitModelsList();
                objShaftSafetyList.PermitList = new List<WorkPermitModels>();

                int x = 1;
                foreach (var item in Safety)
                {
                    if (x <= Convert.ToInt16(form["cnt15"]))
                    {
                        if (form["id_ratings15 " + item.Value] != null)
                        {
                            WorkPermitModels objShaftSafety = new WorkPermitModels();
                            objShaftSafety.id_questions = form["question15 " + item.Value];
                            objShaftSafety.id_ratings = form["id_ratings15 " + item.Value];
                            objShaftSafetyList.PermitList.Add(objShaftSafety);
                        }
                    }
                    x++;
                }

                //Pneumatic

                WorkPermitModelsList objPneumaticList = new WorkPermitModelsList();
                objPneumaticList.PermitList = new List<WorkPermitModels>();

                int y = 1;
                foreach (var item in Pneumatic)
                {
                    if (y <= Convert.ToInt16(form["cnt16"]))
                    {
                        if (form["id_ratings16 " + item.Value] != null)
                        {
                            WorkPermitModels objPneumatic = new WorkPermitModels();
                            objPneumatic.id_questions = form["question16 " + item.Value];
                            objPneumatic.id_ratings = form["id_ratings16 " + item.Value];
                            objPneumaticList.PermitList.Add(objPneumatic);
                        }
                    }
                    y++;
                }
                //Electrical

                WorkPermitModelsList objElectricalList = new WorkPermitModelsList();
                objElectricalList.PermitList = new List<WorkPermitModels>();

                int z = 1;
                foreach (var item in Electrical)
                {
                    if (z <= Convert.ToInt16(form["cnt17"]))
                    {
                        if (form["id_ratings17 " + item.Value] != null)
                        {
                            WorkPermitModels objElectrical = new WorkPermitModels();
                            objElectrical.id_questions = form["question17 " + item.Value];
                            objElectrical.id_ratings = form["id_ratings17 " + item.Value];
                            objElectricalList.PermitList.Add(objElectrical);
                        }
                    }
                    z++;
                }

                if (objPermit.FunAddWorkPermit(objPermit, objInList, objHydroList, objExcvHazardList, objExcvSiteList, objExcvSafetyList, objHotTypeList, objHotHazardList, objHotWelfareList, objHotSiteList, objHotSafetyList, objFalseHazardList, objFalseSiteList, objFalseSafetyList, objShaftHazardList, objShaftSiteList, objShaftSafetyList, objPneumaticList, objElectricalList, objConfinedList))
                {
                    TempData["Successdata"] = "Added Work Permit successfully with Permit No '" + objPermit.permitno + "'";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddWorkPermit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("WorkPermitList");
        }

        [AllowAnonymous]
        public ActionResult WorkPermitList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "WorkPermit";
            WorkPermitModelsList objAccessList = new WorkPermitModelsList();
            objAccessList.PermitList = new List<WorkPermitModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_work_permit,permit_type,permitno,job_performer,date_submitted,area,location," +
                    "contractor,start_date,finish_date,branch,dept,permit_location from t_work_permit where active=1";
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

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_work_permit desc";

                DataSet dsPermitList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsPermitList.Tables.Count > 0 && dsPermitList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsPermitList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            WorkPermitModels objPermitModels = new WorkPermitModels
                            {
                                id_work_permit = dsPermitList.Tables[0].Rows[i]["id_work_permit"].ToString(),
                                permit_type = (dsPermitList.Tables[0].Rows[i]["permit_type"].ToString()),
                                permitno = dsPermitList.Tables[0].Rows[i]["permitno"].ToString(),
                                contractor = dsPermitList.Tables[0].Rows[i]["contractor"].ToString(),
                                job_performer = dsPermitList.Tables[0].Rows[i]["job_performer"].ToString(),
                                area = dsPermitList.Tables[0].Rows[i]["area"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPermitList.Tables[0].Rows[i]["branch"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsPermitList.Tables[0].Rows[i]["dept"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsPermitList.Tables[0].Rows[i]["location"].ToString()),
                            };
                            DateTime dtDocDate;
                            if (dsPermitList.Tables[0].Rows[i]["start_date"].ToString() != ""
                               && DateTime.TryParse(dsPermitList.Tables[0].Rows[i]["start_date"].ToString(), out dtDocDate))
                            {
                                objPermitModels.start_date = dtDocDate;
                            }
                            if (dsPermitList.Tables[0].Rows[i]["finish_date"].ToString() != ""
                                && DateTime.TryParse(dsPermitList.Tables[0].Rows[i]["finish_date"].ToString(), out dtDocDate))
                            {
                                objPermitModels.finish_date = dtDocDate;
                            }
                            if (dsPermitList.Tables[0].Rows[i]["date_submitted"].ToString() != ""
                              && DateTime.TryParse(dsPermitList.Tables[0].Rows[i]["date_submitted"].ToString(), out dtDocDate))
                            {
                                objPermitModels.date_submitted = dtDocDate;
                            }
                            objAccessList.PermitList.Add(objPermitModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in WorkPermitList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in WorkPermitList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objAccessList.PermitList.ToList().ToPagedList(page ?? 1, 80000));
        }

        [AllowAnonymous]
        public JsonResult WorkPermitListSearch(FormCollection form, int? page, string branch_name)
        {
            WorkPermitModelsList objAccessList = new WorkPermitModelsList();
            objAccessList.PermitList = new List<WorkPermitModels>();
            ViewBag.SubMenutype = "WorkPermit";
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_work_permit,permit_type,permitno,job_performer,date_submitted,area," +
                    "location,contractor,start_date,finish_date,branch,dept,permit_location from t_work_permit where active=1";
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

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_work_permit desc";

                DataSet dsPermitList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsPermitList.Tables.Count > 0 && dsPermitList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsPermitList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            WorkPermitModels objPermitModels = new WorkPermitModels
                            {
                                id_work_permit = dsPermitList.Tables[0].Rows[i]["id_work_permit"].ToString(),
                                permit_type = (dsPermitList.Tables[0].Rows[i]["permit_type"].ToString()),
                                permitno = dsPermitList.Tables[0].Rows[i]["permitno"].ToString(),
                                contractor = dsPermitList.Tables[0].Rows[i]["contractor"].ToString(),
                                job_performer = dsPermitList.Tables[0].Rows[i]["job_performer"].ToString(),
                                area = dsPermitList.Tables[0].Rows[i]["area"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPermitList.Tables[0].Rows[i]["branch"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsPermitList.Tables[0].Rows[i]["dept"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsPermitList.Tables[0].Rows[i]["location"].ToString()),
                            };
                            DateTime dtDocDate;
                            if (dsPermitList.Tables[0].Rows[i]["start_date"].ToString() != ""
                               && DateTime.TryParse(dsPermitList.Tables[0].Rows[i]["start_date"].ToString(), out dtDocDate))
                            {
                                objPermitModels.start_date = dtDocDate;
                            }
                            if (dsPermitList.Tables[0].Rows[i]["finish_date"].ToString() != ""
                                && DateTime.TryParse(dsPermitList.Tables[0].Rows[i]["finish_date"].ToString(), out dtDocDate))
                            {
                                objPermitModels.finish_date = dtDocDate;
                            }
                            if (dsPermitList.Tables[0].Rows[i]["date_submitted"].ToString() != ""
                              && DateTime.TryParse(dsPermitList.Tables[0].Rows[i]["date_submitted"].ToString(), out dtDocDate))
                            {
                                objPermitModels.date_submitted = dtDocDate;
                            }
                            objAccessList.PermitList.Add(objPermitModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in WorkPermitListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in WorkPermitListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult WorkPermitEdit()
        {
            ViewBag.SubMenutype = "WorkPermit";
            WorkPermitModels objPermit = new WorkPermitModels();
            try
            {
                ViewBag.Type = objGlobaldata.getWorkPermitTypeList();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Rating = objGlobaldata.GetPermitRating();
                ViewBag.Status = objGlobaldata.GetConstantValue("PermitStatus");
                ViewBag.work = objGlobaldata.GetConstantValue("Permitworktype");
                ViewBag.HydroTest = objGlobaldata.getPermitQuestionList("Hydro Test");
                //ViewBag.Dept = objGlobaldata.GetDepartmentListbox();

                ViewBag.Pneumatic = objGlobaldata.getPermitQuestionList("Pneumatic");

                ViewBag.Electrical = objGlobaldata.getPermitQuestionList("Electrical");

                ViewBag.Confined = objGlobaldata.getPermitQuestionList("Confined");

                ViewBag.Hazard = objGlobaldata.getPermitSectionQuestionList("Excavation", "Hazards Identified");
                ViewBag.WorkSite = objGlobaldata.getPermitSectionQuestionList("Excavation", "Work Site Precautions");
                ViewBag.Safety = objGlobaldata.getPermitSectionQuestionList("Excavation", "Personal Protection and Safety Equipment ");

                ViewBag.HotType = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Type of work");
                ViewBag.HotHazard = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Hazards Identified");
                ViewBag.Welfare = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Welfare");
                ViewBag.HotWorkSite = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Work Site Precautions");

                ViewBag.FalseHazard = objGlobaldata.getPermitSectionQuestionList("False Work", "Hazards Identified");
                ViewBag.FalseWorkSite = objGlobaldata.getPermitSectionQuestionList("False Work", "Work Site Precautions");
                ViewBag.FalseSafety = objGlobaldata.getPermitSectionQuestionList("False Work", "Personal Protection and Safety Equipment");

                ViewBag.ShaftHazard = objGlobaldata.getPermitSectionQuestionList("Shaft Work", "Hazards Identified");
                ViewBag.ShaftWorkSite = objGlobaldata.getPermitSectionQuestionList("Shaft Work", "Work Site Precautions");
                if (Request.QueryString["id_work_permit"] != null && Request.QueryString["id_work_permit"] != "")
                {
                    string id_work_permit = Request.QueryString["id_work_permit"];
                    string sSqlstmt = "select id_work_permit,permit_type,permitno,area,location,job_performer,contractor,contactno,section,equipment,"
                    + "jobno,workorder,access_level,no_person,no_days,resp_pers,dept,start_date,finish_date,resp_section,description,upload,pressure" +
                    ",IA,drawings,method_state,rescue_plan,work_level,comments,loggedby,work_status,work_safe,add_comments,completion_date,work_type,branch,permit_location from t_work_permit where id_work_permit='" + id_work_permit + "'";

                    DataSet dsPermitList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPermitList.Tables.Count > 0 && dsPermitList.Tables[0].Rows.Count > 0)
                    {
                        objPermit = new WorkPermitModels
                        {
                            id_work_permit = dsPermitList.Tables[0].Rows[0]["id_work_permit"].ToString(),
                            permit_type = (dsPermitList.Tables[0].Rows[0]["permit_type"].ToString()),
                            permitno = dsPermitList.Tables[0].Rows[0]["permitno"].ToString(),
                            area = dsPermitList.Tables[0].Rows[0]["area"].ToString(),
                            location = (dsPermitList.Tables[0].Rows[0]["location"].ToString()),
                            job_performer = dsPermitList.Tables[0].Rows[0]["job_performer"].ToString(),
                            contractor = dsPermitList.Tables[0].Rows[0]["contractor"].ToString(),
                            contactno = dsPermitList.Tables[0].Rows[0]["contactno"].ToString(),
                            section = dsPermitList.Tables[0].Rows[0]["section"].ToString(),
                            equipment = dsPermitList.Tables[0].Rows[0]["equipment"].ToString(),
                            jobno = dsPermitList.Tables[0].Rows[0]["jobno"].ToString(),
                            workorder = dsPermitList.Tables[0].Rows[0]["workorder"].ToString(),
                            access_level = dsPermitList.Tables[0].Rows[0]["access_level"].ToString(),
                            no_person = dsPermitList.Tables[0].Rows[0]["no_person"].ToString(),
                            no_days = dsPermitList.Tables[0].Rows[0]["no_days"].ToString(),
                            resp_pers = dsPermitList.Tables[0].Rows[0]["resp_pers"].ToString(),
                            dept = (dsPermitList.Tables[0].Rows[0]["dept"].ToString()),
                            resp_section = dsPermitList.Tables[0].Rows[0]["resp_section"].ToString(),
                            description = dsPermitList.Tables[0].Rows[0]["description"].ToString(),
                            upload = dsPermitList.Tables[0].Rows[0]["upload"].ToString(),
                            pressure = dsPermitList.Tables[0].Rows[0]["pressure"].ToString(),
                            IA = dsPermitList.Tables[0].Rows[0]["IA"].ToString(),
                            drawings = dsPermitList.Tables[0].Rows[0]["drawings"].ToString(),
                            method_state = dsPermitList.Tables[0].Rows[0]["method_state"].ToString(),
                            rescue_plan = dsPermitList.Tables[0].Rows[0]["rescue_plan"].ToString(),
                            work_level = dsPermitList.Tables[0].Rows[0]["work_level"].ToString(),
                            comments = dsPermitList.Tables[0].Rows[0]["comments"].ToString(),
                            work_status = dsPermitList.Tables[0].Rows[0]["work_status"].ToString(),
                            work_safe = dsPermitList.Tables[0].Rows[0]["work_safe"].ToString(),
                            add_comments = dsPermitList.Tables[0].Rows[0]["add_comments"].ToString(),
                            work_type = dsPermitList.Tables[0].Rows[0]["work_type"].ToString(),
                            branch = (dsPermitList.Tables[0].Rows[0]["branch"].ToString()),
                            permit_location = objGlobaldata.GetDropdownitemById(dsPermitList.Tables[0].Rows[0]["permit_location"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsPermitList.Tables[0].Rows[0]["start_date"].ToString() != ""
                           && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["start_date"].ToString(), out dtDocDate))
                        {
                            objPermit.start_date = dtDocDate;
                        }
                        if (dsPermitList.Tables[0].Rows[0]["finish_date"].ToString() != ""
                        && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["finish_date"].ToString(), out dtDocDate))
                        {
                            objPermit.finish_date = dtDocDate;
                        }
                        if (dsPermitList.Tables[0].Rows[0]["completion_date"].ToString() != ""
                       && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["completion_date"].ToString(), out dtDocDate))
                        {
                            objPermit.completion_date = dtDocDate;
                        }
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsPermitList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsPermitList.Tables[0].Rows[0]["branch"].ToString());
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("WorkPermitList");
                    }

                    WorkPermitModelsList objInList = new WorkPermitModelsList();
                    objInList.PermitList = new List<WorkPermitModels>();

                    sSqlstmt = "select id_work_in,id_work_permit,pers_name,idno,date_in from t_work_in where id_work_permit='" + id_work_permit + "'";

                    DataSet dsInList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsInList.Tables.Count > 0 && dsInList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsInList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                WorkPermitModels objInPermit = new WorkPermitModels
                                {
                                    id_work_in = dsInList.Tables[0].Rows[i]["id_work_in"].ToString(),
                                    id_work_permit = dsInList.Tables[0].Rows[i]["id_work_permit"].ToString(),
                                    pers_name = dsInList.Tables[0].Rows[i]["pers_name"].ToString(),
                                    idno = dsInList.Tables[0].Rows[i]["idno"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsInList.Tables[0].Rows[0]["date_in"].ToString() != ""
                                && DateTime.TryParse(dsInList.Tables[0].Rows[0]["date_in"].ToString(), out dtDocDate))
                                {
                                    objInPermit.date_in = dtDocDate;
                                }
                                objInList.PermitList.Add(objInPermit);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in WorkPermitEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("finish_date");
                            }
                        }
                        ViewBag.objInList = objInList;
                    }

                    //Confined
                    string sql18 = "select id_confined,id_work_permit,id_questions,id_ratings from t_permit_confined where id_work_permit='" + id_work_permit + "'";
                    DataSet dsConfined = objGlobaldata.Getdetails(sql18);

                    WorkPermitModelsList objConfinedList = new WorkPermitModelsList();
                    objConfinedList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicObjConfinedList = new Dictionary<string, string>();

                    for (int i = 0; dsConfined.Tables.Count > 0 && i < dsConfined.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objConfined = new WorkPermitModels
                        {
                            id_confined = dsConfined.Tables[0].Rows[i]["id_confined"].ToString(),
                            id_questions = (dsConfined.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsConfined.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicObjConfinedList.Add(dsConfined.Tables[0].Rows[i]["id_questions"].ToString(), dsConfined.Tables[0].Rows[i]["id_ratings"].ToString());
                        objConfinedList.PermitList.Add(objConfined);
                    }
                    ViewBag.objConfinedList = objConfinedList;
                    ViewBag.dicObjConfinedList = dicObjConfinedList;

                    //Hydro
                    string sql1 = "select id_hydro_safety,id_work_permit,id_questions,id_ratings from t_permit_hydro_safety where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHydroSafety = objGlobaldata.Getdetails(sql1);

                    WorkPermitModelsList objHydroList = new WorkPermitModelsList();
                    objHydroList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicObjHydroList = new Dictionary<string, string>();

                    for (int i = 0; dsHydroSafety.Tables.Count > 0 && i < dsHydroSafety.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHydroSafety = new WorkPermitModels
                        {
                            id_hydro_safety = dsHydroSafety.Tables[0].Rows[i]["id_hydro_safety"].ToString(),
                            id_questions = (dsHydroSafety.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHydroSafety.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicObjHydroList.Add(dsHydroSafety.Tables[0].Rows[i]["id_questions"].ToString(), dsHydroSafety.Tables[0].Rows[i]["id_ratings"].ToString());
                        objHydroList.PermitList.Add(objHydroSafety);
                    }
                    ViewBag.objHydroList = objHydroList;
                    ViewBag.dicObjHydroList = dicObjHydroList;

                    //Excavation
                    string sql2 = "select id_excv_hazard,id_work_permit,id_questions,id_ratings from t_permit_excv_hazard where id_work_permit='" + id_work_permit + "'";
                    DataSet dsExcvHazard = objGlobaldata.Getdetails(sql2);

                    WorkPermitModelsList ExcvHazardList = new WorkPermitModelsList();
                    ExcvHazardList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicExcvHazardList = new Dictionary<string, string>();

                    for (int i = 0; dsExcvHazard.Tables.Count > 0 && i < dsExcvHazard.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objExcvHazard = new WorkPermitModels
                        {
                            id_excv_hazard = dsExcvHazard.Tables[0].Rows[i]["id_excv_hazard"].ToString(),
                            id_questions = (dsExcvHazard.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsExcvHazard.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicExcvHazardList.Add(dsExcvHazard.Tables[0].Rows[i]["id_questions"].ToString(), dsExcvHazard.Tables[0].Rows[i]["id_ratings"].ToString());
                        ExcvHazardList.PermitList.Add(objExcvHazard);
                    }
                    ViewBag.ExcvHazardList = ExcvHazardList;
                    ViewBag.dicExcvHazardList = dicExcvHazardList;

                    string sql3 = "select id_excv_site,id_work_permit,id_questions,id_ratings from t_permit_excv_site where id_work_permit='" + id_work_permit + "'";
                    DataSet dsExcvSite = objGlobaldata.Getdetails(sql3);

                    WorkPermitModelsList ExcvSiteList = new WorkPermitModelsList();
                    ExcvSiteList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicExcvSiteList = new Dictionary<string, string>();

                    for (int i = 0; dsExcvSite.Tables.Count > 0 && i < dsExcvSite.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objExcvSite = new WorkPermitModels
                        {
                            id_excv_site = dsExcvSite.Tables[0].Rows[i]["id_excv_site"].ToString(),
                            id_questions = (dsExcvSite.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsExcvSite.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicExcvSiteList.Add(dsExcvSite.Tables[0].Rows[i]["id_questions"].ToString(), dsExcvSite.Tables[0].Rows[i]["id_ratings"].ToString());
                        ExcvSiteList.PermitList.Add(objExcvSite);
                    }
                    ViewBag.ExcvSiteList = ExcvSiteList;
                    ViewBag.dicExcvSiteList = dicExcvSiteList;

                    string sql4 = "select id_excv_safety,id_work_permit,id_questions,id_ratings from t_permit_excv_safety where id_work_permit='" + id_work_permit + "'";
                    DataSet dsExcvSafety = objGlobaldata.Getdetails(sql4);

                    WorkPermitModelsList ExcvSafetyList = new WorkPermitModelsList();
                    ExcvSafetyList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicExcvSafetyList = new Dictionary<string, string>();

                    for (int i = 0; dsExcvSafety.Tables.Count > 0 && i < dsExcvSafety.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objExcvSafety = new WorkPermitModels
                        {
                            id_excv_safety = dsExcvSafety.Tables[0].Rows[i]["id_excv_safety"].ToString(),
                            id_questions = (dsExcvSafety.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsExcvSafety.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicExcvSafetyList.Add(dsExcvSafety.Tables[0].Rows[i]["id_questions"].ToString(), dsExcvSafety.Tables[0].Rows[i]["id_ratings"].ToString());
                        ExcvSafetyList.PermitList.Add(objExcvSafety);
                    }
                    ViewBag.ExcvSafetyList = ExcvSafetyList;
                    ViewBag.dicExcvSafetyList = dicExcvSafetyList;

                    //Hot work
                    string sql5 = "select id_hot_type,id_work_permit,id_questions,id_ratings from t_permit_hot_type where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHotType = objGlobaldata.Getdetails(sql5);

                    WorkPermitModelsList HotTypeList = new WorkPermitModelsList();
                    HotTypeList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicHotTypeList = new Dictionary<string, string>();

                    for (int i = 0; dsHotType.Tables.Count > 0 && i < dsHotType.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHotTypey = new WorkPermitModels
                        {
                            id_hot_type = dsHotType.Tables[0].Rows[i]["id_hot_type"].ToString(),
                            id_questions = (dsHotType.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHotType.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicHotTypeList.Add(dsHotType.Tables[0].Rows[i]["id_questions"].ToString(), dsHotType.Tables[0].Rows[i]["id_ratings"].ToString());
                        HotTypeList.PermitList.Add(objHotTypey);
                    }
                    ViewBag.HotTypeList = HotTypeList;
                    ViewBag.dicHotTypeList = dicHotTypeList;

                    string sql6 = "select id_hot_hazard,id_work_permit,id_questions,id_ratings from t_permit_hot_hazard where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHotHazard = objGlobaldata.Getdetails(sql6);

                    WorkPermitModelsList HotHazardList = new WorkPermitModelsList();
                    HotHazardList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicHotHazardList = new Dictionary<string, string>();

                    for (int i = 0; dsHotHazard.Tables.Count > 0 && i < dsHotHazard.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHotHazard = new WorkPermitModels
                        {
                            id_hot_hazard = dsHotHazard.Tables[0].Rows[i]["id_hot_hazard"].ToString(),
                            id_questions = (dsHotHazard.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHotHazard.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicHotHazardList.Add(dsHotHazard.Tables[0].Rows[i]["id_questions"].ToString(), dsHotHazard.Tables[0].Rows[i]["id_ratings"].ToString());
                        HotHazardList.PermitList.Add(objHotHazard);
                    }
                    ViewBag.HotHazardList = HotHazardList;
                    ViewBag.dicHotHazardList = dicHotHazardList;

                    string sql7 = "select id_hot_welfare,id_work_permit,id_questions,id_ratings from t_permit_hot_welfare where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHotWelfare = objGlobaldata.Getdetails(sql7);

                    WorkPermitModelsList HotWelfareList = new WorkPermitModelsList();
                    HotWelfareList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicHotWelfareList = new Dictionary<string, string>();

                    for (int i = 0; dsHotWelfare.Tables.Count > 0 && i < dsHotWelfare.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHotWelfare = new WorkPermitModels
                        {
                            id_hot_welfare = dsHotWelfare.Tables[0].Rows[i]["id_hot_welfare"].ToString(),
                            id_questions = (dsHotWelfare.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHotWelfare.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicHotWelfareList.Add(dsHotWelfare.Tables[0].Rows[i]["id_questions"].ToString(), dsHotWelfare.Tables[0].Rows[i]["id_ratings"].ToString());
                        HotWelfareList.PermitList.Add(objHotWelfare);
                    }
                    ViewBag.HotWelfareList = HotWelfareList;
                    ViewBag.dicHotWelfareList = dicHotWelfareList;

                    string sql8 = "select id_hot_site,id_work_permit,id_questions,id_ratings from t_permit_hot_site where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHotSite = objGlobaldata.Getdetails(sql8);

                    WorkPermitModelsList HotSiteList = new WorkPermitModelsList();
                    HotSiteList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicHotSiteList = new Dictionary<string, string>();

                    for (int i = 0; dsHotSite.Tables.Count > 0 && i < dsHotSite.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHotSite = new WorkPermitModels
                        {
                            id_hot_site = dsHotSite.Tables[0].Rows[i]["id_hot_site"].ToString(),
                            id_questions = (dsHotSite.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHotSite.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicHotSiteList.Add(dsHotSite.Tables[0].Rows[i]["id_questions"].ToString(), dsHotSite.Tables[0].Rows[i]["id_ratings"].ToString());
                        HotSiteList.PermitList.Add(objHotSite);
                    }
                    ViewBag.HotSiteList = HotSiteList;
                    ViewBag.dicHotSiteList = dicHotSiteList;

                    string sql9 = "select id_hot_safety,id_work_permit,id_questions,id_ratings from t_permit_hot_safety where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHotSafety = objGlobaldata.Getdetails(sql9);

                    WorkPermitModelsList HotSafetyList = new WorkPermitModelsList();
                    HotSafetyList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicHotSafetyList = new Dictionary<string, string>();

                    for (int i = 0; dsHotSafety.Tables.Count > 0 && i < dsHotSafety.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHotSafety = new WorkPermitModels
                        {
                            id_hot_safety = dsHotSafety.Tables[0].Rows[i]["id_hot_safety"].ToString(),
                            id_questions = (dsHotSafety.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHotSafety.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicHotSafetyList.Add(dsHotSafety.Tables[0].Rows[i]["id_questions"].ToString(), dsHotSafety.Tables[0].Rows[i]["id_ratings"].ToString());
                        HotSafetyList.PermitList.Add(objHotSafety);
                    }
                    ViewBag.HotSafetyList = HotSafetyList;
                    ViewBag.dicHotSafetyList = dicHotSafetyList;

                    //false work

                    string sql10 = "select id_false_hazard,id_work_permit,id_questions,id_ratings from t_permit_false_hazard where id_work_permit='" + id_work_permit + "'";
                    DataSet dsFalseHazard = objGlobaldata.Getdetails(sql10);

                    WorkPermitModelsList FalseHazardList = new WorkPermitModelsList();
                    FalseHazardList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicFalseHazardList = new Dictionary<string, string>();

                    for (int i = 0; dsFalseHazard.Tables.Count > 0 && i < dsFalseHazard.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objFalseHazard = new WorkPermitModels
                        {
                            id_false_hazard = dsFalseHazard.Tables[0].Rows[i]["id_false_hazard"].ToString(),
                            id_questions = (dsFalseHazard.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsFalseHazard.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicFalseHazardList.Add(dsFalseHazard.Tables[0].Rows[i]["id_questions"].ToString(), dsFalseHazard.Tables[0].Rows[i]["id_ratings"].ToString());
                        FalseHazardList.PermitList.Add(objFalseHazard);
                    }
                    ViewBag.FalseHazardList = FalseHazardList;
                    ViewBag.dicFalseHazardList = dicFalseHazardList;

                    string sql11 = "select id_false_site,id_work_permit,id_questions,id_ratings from t_permit_false_site where id_work_permit='" + id_work_permit + "'";
                    DataSet dsFalseSite = objGlobaldata.Getdetails(sql11);

                    WorkPermitModelsList FalseSiteList = new WorkPermitModelsList();
                    FalseSiteList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicFalseSiteList = new Dictionary<string, string>();

                    for (int i = 0; dsFalseSite.Tables.Count > 0 && i < dsFalseSite.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objFalseSite = new WorkPermitModels
                        {
                            id_false_site = dsFalseSite.Tables[0].Rows[i]["id_false_site"].ToString(),
                            id_questions = (dsFalseSite.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsFalseSite.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicFalseSiteList.Add(dsFalseSite.Tables[0].Rows[i]["id_questions"].ToString(), dsFalseSite.Tables[0].Rows[i]["id_ratings"].ToString());
                        FalseSiteList.PermitList.Add(objFalseSite);
                    }
                    ViewBag.FalseSiteList = FalseSiteList;
                    ViewBag.dicFalseSiteList = dicFalseSiteList;

                    string sql12 = "select id_false_safety,id_work_permit,id_questions,id_ratings from t_permit_false_safety where id_work_permit='" + id_work_permit + "'";
                    DataSet dsFalseSafety = objGlobaldata.Getdetails(sql12);

                    WorkPermitModelsList FalseSafetyList = new WorkPermitModelsList();
                    FalseSafetyList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicFalseSafetyList = new Dictionary<string, string>();

                    for (int i = 0; dsFalseSafety.Tables.Count > 0 && i < dsFalseSafety.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objFalseSafety = new WorkPermitModels
                        {
                            id_false_safety = dsFalseSafety.Tables[0].Rows[i]["id_false_safety"].ToString(),
                            id_questions = (dsFalseSafety.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsFalseSafety.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicFalseSafetyList.Add(dsFalseSafety.Tables[0].Rows[i]["id_questions"].ToString(), dsFalseSafety.Tables[0].Rows[i]["id_ratings"].ToString());
                        FalseSafetyList.PermitList.Add(objFalseSafety);
                    }
                    ViewBag.FalseSafetyList = FalseSafetyList;
                    ViewBag.dicFalseSafetyList = dicFalseSafetyList;

                    //Shaft work

                    string sql13 = "select id_shaft_hazard,id_work_permit,id_questions,id_ratings from t_permit_shaft_hazard where id_work_permit='" + id_work_permit + "'";
                    DataSet dsShaftHazard = objGlobaldata.Getdetails(sql13);

                    WorkPermitModelsList ShaftHazardList = new WorkPermitModelsList();
                    ShaftHazardList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicShaftHazardList = new Dictionary<string, string>();

                    for (int i = 0; dsShaftHazard.Tables.Count > 0 && i < dsShaftHazard.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objShaftHazard = new WorkPermitModels
                        {
                            id_shaft_hazard = dsShaftHazard.Tables[0].Rows[i]["id_shaft_hazard"].ToString(),
                            id_questions = (dsShaftHazard.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsShaftHazard.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicShaftHazardList.Add(dsShaftHazard.Tables[0].Rows[i]["id_questions"].ToString(), dsShaftHazard.Tables[0].Rows[i]["id_ratings"].ToString());
                        ShaftHazardList.PermitList.Add(objShaftHazard);
                    }
                    ViewBag.ShaftHazardList = ShaftHazardList;
                    ViewBag.dicShaftHazardList = dicShaftHazardList;

                    string sql14 = "select id_shaft_site,id_work_permit,id_questions,id_ratings from t_permit_shaft_site where id_work_permit='" + id_work_permit + "'";
                    DataSet dsShaftSite = objGlobaldata.Getdetails(sql14);

                    WorkPermitModelsList ShaftSiteList = new WorkPermitModelsList();
                    ShaftSiteList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicShaftSiteList = new Dictionary<string, string>();

                    for (int i = 0; dsShaftSite.Tables.Count > 0 && i < dsShaftSite.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objShaftSite = new WorkPermitModels
                        {
                            id_shaft_site = dsShaftSite.Tables[0].Rows[i]["id_shaft_site"].ToString(),
                            id_questions = (dsShaftSite.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsShaftSite.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicShaftSiteList.Add(dsShaftSite.Tables[0].Rows[i]["id_questions"].ToString(), dsShaftSite.Tables[0].Rows[i]["id_ratings"].ToString());
                        ShaftSiteList.PermitList.Add(objShaftSite);
                    }
                    ViewBag.ShaftSiteList = ShaftSiteList;
                    ViewBag.dicShaftSiteList = dicShaftSiteList;

                    string sql15 = "select id_shaft_safety,id_work_permit,id_questions,id_ratings from t_permit_shaft_safety where id_work_permit='" + id_work_permit + "'";
                    DataSet dsShaftSafety = objGlobaldata.Getdetails(sql15);

                    WorkPermitModelsList ShaftSafetyList = new WorkPermitModelsList();
                    ShaftSafetyList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicShaftSafetyList = new Dictionary<string, string>();

                    for (int i = 0; dsShaftSafety.Tables.Count > 0 && i < dsShaftSafety.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objShaftSafety = new WorkPermitModels
                        {
                            id_shaft_safety = dsShaftSafety.Tables[0].Rows[i]["id_shaft_safety"].ToString(),
                            id_questions = (dsShaftSafety.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsShaftSafety.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicShaftSafetyList.Add(dsShaftSafety.Tables[0].Rows[i]["id_questions"].ToString(), dsShaftSafety.Tables[0].Rows[i]["id_ratings"].ToString());
                        ShaftSafetyList.PermitList.Add(objShaftSafety);
                    }
                    ViewBag.ShaftSafetyList = ShaftSafetyList;
                    ViewBag.dicShaftSafetyList = dicShaftSafetyList;

                    //Pneumatic

                    string sql16 = "select id_pneumatic,id_work_permit,id_questions,id_ratings from t_permit_pneumatic where id_work_permit='" + id_work_permit + "'";
                    DataSet dsPneumatic = objGlobaldata.Getdetails(sql16);

                    WorkPermitModelsList PneumaticList = new WorkPermitModelsList();
                    PneumaticList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicPneumaticList = new Dictionary<string, string>();

                    for (int i = 0; dsPneumatic.Tables.Count > 0 && i < dsPneumatic.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objPneumatic = new WorkPermitModels
                        {
                            id_pneumatic = dsPneumatic.Tables[0].Rows[i]["id_pneumatic"].ToString(),
                            id_questions = (dsPneumatic.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsPneumatic.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicPneumaticList.Add(dsPneumatic.Tables[0].Rows[i]["id_questions"].ToString(), dsPneumatic.Tables[0].Rows[i]["id_ratings"].ToString());
                        PneumaticList.PermitList.Add(objPneumatic);
                    }
                    ViewBag.PneumaticList = PneumaticList;
                    ViewBag.dicPneumaticList = dicPneumaticList;

                    //Electrical

                    string sql17 = "select id_electrical,id_work_permit,id_questions,id_ratings from t_permit_electrical where id_work_permit='" + id_work_permit + "'";
                    DataSet dsElectrical = objGlobaldata.Getdetails(sql17);

                    WorkPermitModelsList ElectricalList = new WorkPermitModelsList();
                    ElectricalList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicElectricalList = new Dictionary<string, string>();

                    for (int i = 0; dsElectrical.Tables.Count > 0 && i < dsElectrical.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objElectrical = new WorkPermitModels
                        {
                            id_electrical = dsElectrical.Tables[0].Rows[i]["id_electrical"].ToString(),
                            id_questions = (dsElectrical.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsElectrical.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicElectricalList.Add(dsElectrical.Tables[0].Rows[i]["id_questions"].ToString(), dsElectrical.Tables[0].Rows[i]["id_ratings"].ToString());
                        ElectricalList.PermitList.Add(objElectrical);
                    }
                    ViewBag.ElectricalList = ElectricalList;
                    ViewBag.dicElectricalList = dicElectricalList;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in WorkPermitEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("finish_date");
            }
            return View(objPermit);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult WorkPermitEdit(FormCollection form, WorkPermitModels objPermit, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objPermit.branch = form["branch"];
                objPermit.dept = form["dept"];
                objPermit.location = form["location"];

                DateTime dateValue;

                string QCDelete = Request.Form["QCDocsValselectall"];
                if (DateTime.TryParse(form["start_date"], out dateValue) == true)
                {
                    objPermit.start_date = dateValue;
                }
                if (DateTime.TryParse(form["finish_date"], out dateValue) == true)
                {
                    objPermit.finish_date = dateValue;
                }
                if (DateTime.TryParse(form["completion_date"], out dateValue) == true)
                {
                    objPermit.completion_date = dateValue;
                }
                if (form["start_date"] != null && DateTime.TryParse(form["start_date"], out dateValue) == true)
                {
                    objPermit.start_date = dateValue;
                    int iHr, iMin;
                    if (form["StartTimeInHour"] != null && int.TryParse(form["StartTimeInHour"], out iHr) &&
                        form["StartTimeInMin"] != null && int.TryParse(form["StartTimeInMin"], out iMin))
                    {
                        objPermit.start_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (form["finish_date"] != null && DateTime.TryParse(form["finish_date"], out dateValue) == true)
                {
                    objPermit.finish_date = dateValue;
                    int iHr, iMin;
                    if (form["FinishTimeInHour"] != null && int.TryParse(form["FinishTimeInHour"], out iHr) &&
                        form["FinishTimeInMin"] != null && int.TryParse(form["FinishTimeInMin"], out iMin))
                    {
                        objPermit.finish_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (form["completion_date"] != null && DateTime.TryParse(form["completion_date"], out dateValue) == true)
                {
                    objPermit.completion_date = dateValue;
                    int iHr, iMin;
                    if (form["CompTimeInHour"] != null && int.TryParse(form["CompTimeInHour"], out iHr) &&
                        form["CompTimeInMin"] != null && int.TryParse(form["CompTimeInMin"], out iMin))
                    {
                        objPermit.completion_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase files = Request.Files[0];
                    if (upload != null && files.ContentLength > 0)
                    {
                        objPermit.upload = "";
                        foreach (var file in upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Permit"), Path.GetFileName(file.FileName));
                                string sFilename = "Permit" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objPermit.upload = objPermit.upload + "," + "~/DataUpload/MgmtDocs/Permit/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in WorkPermitEdit-upload: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                        objPermit.upload = objPermit.upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                    {
                        objPermit.upload = objPermit.upload + "," + form["QCDocsVal"];
                        objPermit.upload = objPermit.upload.Trim(',');
                    }
                    else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                    {
                        objPermit.upload = null;
                    }
                    else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                    {
                        objPermit.upload = null;
                    }
                }
                WorkPermitModelsList objInList = new WorkPermitModelsList();
                objInList.PermitList = new List<WorkPermitModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["pers_name" + i] != null)
                    {
                        WorkPermitModels InModel = new WorkPermitModels();

                        InModel.pers_name = form["pers_name" + i];
                        InModel.idno = form["idno" + i];
                        if (DateTime.TryParse(form["date_in" + i], out dateValue) == true)
                        {
                            InModel.date_in = dateValue;
                        }
                        objInList.PermitList.Add(InModel);
                    }
                }

                //Hydro
                MultiSelectList HydroTest = objGlobaldata.getPermitQuestionList("Hydro Test");

                MultiSelectList Pneumatic = objGlobaldata.getPermitQuestionList("Pneumatic");

                MultiSelectList Electrical = objGlobaldata.getPermitQuestionList("Electrical");

                MultiSelectList Confined = objGlobaldata.getPermitQuestionList("Confined");
                //Excavation
                MultiSelectList Hazard = objGlobaldata.getPermitSectionQuestionList("Excavation", "Hazards Identified");
                MultiSelectList WorkSite = objGlobaldata.getPermitSectionQuestionList("Excavation", "Work Site Precautions");
                MultiSelectList Safety = objGlobaldata.getPermitSectionQuestionList("Excavation", "Personal Protection and Safety Equipment ");

                //hot work
                MultiSelectList HotType = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Type of work");
                MultiSelectList HotHazard = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Hazards Identified");
                MultiSelectList Welfare = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Welfare");
                MultiSelectList HotWorkSite = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Work Site Precautions");

                //False work
                MultiSelectList FalseHazard = objGlobaldata.getPermitSectionQuestionList("False Work", "Hazards Identified");
                MultiSelectList FalseWorkSite = objGlobaldata.getPermitSectionQuestionList("False Work", "Work Site Precautions");
                MultiSelectList FalseSafety = objGlobaldata.getPermitSectionQuestionList("False Work", "Personal Protection and Safety Equipment");

                MultiSelectList ShaftHazard = objGlobaldata.getPermitSectionQuestionList("Shaft Work", "Hazards Identified");
                MultiSelectList ShaftWorkSite = objGlobaldata.getPermitSectionQuestionList("Shaft Work", "Work Site Precautions");

                //Confined
                WorkPermitModelsList objConfinedList = new WorkPermitModelsList();
                objConfinedList.PermitList = new List<WorkPermitModels>();

                int c = 1;
                foreach (var item in Confined)
                {
                    if (c <= Convert.ToInt16(form["cnt18"]))
                    {
                        if (form["id_ratings18 " + item.Value] != null)
                        {
                            WorkPermitModels objConfined = new WorkPermitModels();
                            objConfined.id_questions = form["question18 " + item.Value];
                            objConfined.id_ratings = form["id_ratings18 " + item.Value];
                            objConfinedList.PermitList.Add(objConfined);
                        }
                    }
                    c++;
                }
                //Hydro
                WorkPermitModelsList objHydroList = new WorkPermitModelsList();
                objHydroList.PermitList = new List<WorkPermitModels>();

                int j = 1;
                foreach (var item in HydroTest)
                {
                    if (j <= Convert.ToInt16(form["cnt1"]))
                    {
                        if (form["id_ratings1 " + item.Value] != null)
                        {
                            WorkPermitModels objHydro = new WorkPermitModels();
                            objHydro.id_questions = form["question1 " + item.Value];
                            objHydro.id_ratings = form["id_ratings1 " + item.Value];
                            objHydroList.PermitList.Add(objHydro);
                        }
                    }
                    j++;
                }

                //Excavation
                WorkPermitModelsList objExcvHazardList = new WorkPermitModelsList();
                objExcvHazardList.PermitList = new List<WorkPermitModels>();

                int k = 1;
                foreach (var item in Hazard)
                {
                    if (k <= Convert.ToInt16(form["cnt2"]))
                    {
                        if (form["id_ratings2 " + item.Value] != null)
                        {
                            WorkPermitModels objExcvHazard = new WorkPermitModels();
                            objExcvHazard.id_questions = form["question2 " + item.Value];
                            objExcvHazard.id_ratings = form["id_ratings2 " + item.Value];
                            objExcvHazardList.PermitList.Add(objExcvHazard);
                        }
                    }
                    k++;
                }

                WorkPermitModelsList objExcvSiteList = new WorkPermitModelsList();
                objExcvSiteList.PermitList = new List<WorkPermitModels>();

                int l = 1;
                foreach (var item in WorkSite)
                {
                    if (l <= Convert.ToInt16(form["cnt3"]))
                    {
                        if (form["id_ratings3 " + item.Value] != null)
                        {
                            WorkPermitModels objExcvSite = new WorkPermitModels();
                            objExcvSite.id_questions = form["question3 " + item.Value];
                            objExcvSite.id_ratings = form["id_ratings3 " + item.Value];
                            objExcvSiteList.PermitList.Add(objExcvSite);
                        }
                    }
                    l++;
                }

                WorkPermitModelsList objExcvSafetyList = new WorkPermitModelsList();
                objExcvSafetyList.PermitList = new List<WorkPermitModels>();

                int m = 1;
                foreach (var item in Safety)
                {
                    if (m <= Convert.ToInt16(form["cnt3"]))
                    {
                        if (form["id_ratings4 " + item.Value] != null)
                        {
                            WorkPermitModels objExcvSafety = new WorkPermitModels();
                            objExcvSafety.id_questions = form["question4 " + item.Value];
                            objExcvSafety.id_ratings = form["id_ratings4 " + item.Value];
                            objExcvSafetyList.PermitList.Add(objExcvSafety);
                        }
                    }
                    m++;
                }

                //Hot work
                WorkPermitModelsList objHotTypeList = new WorkPermitModelsList();
                objHotTypeList.PermitList = new List<WorkPermitModels>();

                int n = 1;
                foreach (var item in HotType)
                {
                    if (n <= Convert.ToInt16(form["cnt5"]))
                    {
                        if (form["id_ratings5 " + item.Value] != null)
                        {
                            WorkPermitModels objHotType = new WorkPermitModels();
                            objHotType.id_questions = form["question5 " + item.Value];
                            objHotType.id_ratings = form["id_ratings5 " + item.Value];
                            objHotTypeList.PermitList.Add(objHotType);
                        }
                    }
                    n++;
                }

                WorkPermitModelsList objHotHazardList = new WorkPermitModelsList();
                objHotHazardList.PermitList = new List<WorkPermitModels>();

                int o = 1;
                foreach (var item in HotHazard)
                {
                    if (o <= Convert.ToInt16(form["cnt6"]))
                    {
                        if (form["id_ratings6 " + item.Value] != null)
                        {
                            WorkPermitModels objHotHazard = new WorkPermitModels();
                            objHotHazard.id_questions = form["question6 " + item.Value];
                            objHotHazard.id_ratings = form["id_ratings6 " + item.Value];
                            objHotHazardList.PermitList.Add(objHotHazard);
                        }
                    }
                    o++;
                }

                WorkPermitModelsList objHotWelfareList = new WorkPermitModelsList();
                objHotWelfareList.PermitList = new List<WorkPermitModels>();

                int p = 1;
                foreach (var item in Welfare)
                {
                    if (p <= Convert.ToInt16(form["cnt7"]))
                    {
                        if (form["id_ratings7 " + item.Value] != null)
                        {
                            WorkPermitModels HotWelfare = new WorkPermitModels();
                            HotWelfare.id_questions = form["question7 " + item.Value];
                            HotWelfare.id_ratings = form["id_ratings7 " + item.Value];
                            objHotWelfareList.PermitList.Add(HotWelfare);
                        }
                    }
                    p++;
                }

                WorkPermitModelsList objHotSiteList = new WorkPermitModelsList();
                objHotSiteList.PermitList = new List<WorkPermitModels>();

                int q = 1;
                foreach (var item in HotWorkSite)
                {
                    if (q <= Convert.ToInt16(form["cnt8"]))
                    {
                        if (form["id_ratings8 " + item.Value] != null)
                        {
                            WorkPermitModels objHotSite = new WorkPermitModels();
                            objHotSite.id_questions = form["question8 " + item.Value];
                            objHotSite.id_ratings = form["id_ratings8 " + item.Value];
                            objHotSiteList.PermitList.Add(objHotSite);
                        }
                    }
                    q++;
                }

                WorkPermitModelsList objHotSafetyList = new WorkPermitModelsList();
                objHotSafetyList.PermitList = new List<WorkPermitModels>();

                int r = 1;
                foreach (var item in Safety)
                {
                    if (r <= Convert.ToInt16(form["cnt9"]))
                    {
                        if (form["id_ratings9 " + item.Value] != null)
                        {
                            WorkPermitModels objHotSafety = new WorkPermitModels();
                            objHotSafety.id_questions = form["question9 " + item.Value];
                            objHotSafety.id_ratings = form["id_ratings9 " + item.Value];
                            objHotSafetyList.PermitList.Add(objHotSafety);
                        }
                    }
                    r++;
                }

                //False work
                WorkPermitModelsList objFalseHazardList = new WorkPermitModelsList();
                objFalseHazardList.PermitList = new List<WorkPermitModels>();

                int s = 1;
                foreach (var item in FalseHazard)
                {
                    if (s <= Convert.ToInt16(form["cnt10"]))
                    {
                        if (form["id_ratings10 " + item.Value] != null)
                        {
                            WorkPermitModels objFalseHazard = new WorkPermitModels();
                            objFalseHazard.id_questions = form["question10 " + item.Value];
                            objFalseHazard.id_ratings = form["id_ratings10 " + item.Value];
                            objFalseHazardList.PermitList.Add(objFalseHazard);
                        }
                    }
                    s++;
                }

                WorkPermitModelsList objFalseSiteList = new WorkPermitModelsList();
                objFalseSiteList.PermitList = new List<WorkPermitModels>();

                int t = 1;
                foreach (var item in FalseWorkSite)
                {
                    if (t <= Convert.ToInt16(form["cnt11"]))
                    {
                        if (form["id_ratings11 " + item.Value] != null)
                        {
                            WorkPermitModels objFalseSite = new WorkPermitModels();
                            objFalseSite.id_questions = form["question11 " + item.Value];
                            objFalseSite.id_ratings = form["id_ratings11 " + item.Value];
                            objFalseSiteList.PermitList.Add(objFalseSite);
                        }
                    }
                    t++;
                }

                WorkPermitModelsList objFalseSafetyList = new WorkPermitModelsList();
                objFalseSafetyList.PermitList = new List<WorkPermitModels>();

                int u = 1;
                foreach (var item in FalseSafety)
                {
                    if (u <= Convert.ToInt16(form["cnt12"]))
                    {
                        if (form["id_ratings12 " + item.Value] != null)
                        {
                            WorkPermitModels objFalseSafety = new WorkPermitModels();
                            objFalseSafety.id_questions = form["question12 " + item.Value];
                            objFalseSafety.id_ratings = form["id_ratings12 " + item.Value];
                            objFalseSafetyList.PermitList.Add(objFalseSafety);
                        }
                    }
                    u++;
                }
                //shaft work

                WorkPermitModelsList objShaftHazardList = new WorkPermitModelsList();
                objShaftHazardList.PermitList = new List<WorkPermitModels>();

                int v = 1;
                foreach (var item in ShaftHazard)
                {
                    if (v <= Convert.ToInt16(form["cnt13"]))
                    {
                        if (form["id_ratings13 " + item.Value] != null)
                        {
                            WorkPermitModels objShaftHazard = new WorkPermitModels();
                            objShaftHazard.id_questions = form["question13 " + item.Value];
                            objShaftHazard.id_ratings = form["id_ratings13 " + item.Value];
                            objShaftHazardList.PermitList.Add(objShaftHazard);
                        }
                    }
                    v++;
                }

                WorkPermitModelsList objShaftSiteList = new WorkPermitModelsList();
                objShaftSiteList.PermitList = new List<WorkPermitModels>();

                int w = 1;
                foreach (var item in ShaftWorkSite)
                {
                    if (w <= Convert.ToInt16(form["cnt14"]))
                    {
                        if (form["id_ratings14 " + item.Value] != null)
                        {
                            WorkPermitModels objShaftSite = new WorkPermitModels();
                            objShaftSite.id_questions = form["question14 " + item.Value];
                            objShaftSite.id_ratings = form["id_ratings14 " + item.Value];
                            objShaftSiteList.PermitList.Add(objShaftSite);
                        }
                    }
                    w++;
                }

                WorkPermitModelsList objShaftSafetyList = new WorkPermitModelsList();
                objShaftSafetyList.PermitList = new List<WorkPermitModels>();

                int x = 1;
                foreach (var item in Safety)
                {
                    if (x <= Convert.ToInt16(form["cnt15"]))
                    {
                        if (form["id_ratings15 " + item.Value] != null)
                        {
                            WorkPermitModels objShaftSafety = new WorkPermitModels();
                            objShaftSafety.id_questions = form["question15 " + item.Value];
                            objShaftSafety.id_ratings = form["id_ratings15 " + item.Value];
                            objShaftSafetyList.PermitList.Add(objShaftSafety);
                        }
                    }
                    x++;
                }

                //Pneumatic

                WorkPermitModelsList objPneumaticList = new WorkPermitModelsList();
                objPneumaticList.PermitList = new List<WorkPermitModels>();

                int y = 1;
                foreach (var item in Pneumatic)
                {
                    if (y <= Convert.ToInt16(form["cnt16"]))
                    {
                        if (form["id_ratings16 " + item.Value] != null)
                        {
                            WorkPermitModels objPneumatic = new WorkPermitModels();
                            objPneumatic.id_questions = form["question16 " + item.Value];
                            objPneumatic.id_ratings = form["id_ratings16 " + item.Value];
                            objPneumaticList.PermitList.Add(objPneumatic);
                        }
                    }
                    y++;
                }
                //Electrical

                WorkPermitModelsList objElectricalList = new WorkPermitModelsList();
                objElectricalList.PermitList = new List<WorkPermitModels>();

                int z = 1;
                foreach (var item in Electrical)
                {
                    if (z <= Convert.ToInt16(form["cnt17"]))
                    {
                        if (form["id_ratings17 " + item.Value] != null)
                        {
                            WorkPermitModels objElectrical = new WorkPermitModels();
                            objElectrical.id_questions = form["question17 " + item.Value];
                            objElectrical.id_ratings = form["id_ratings17 " + item.Value];
                            objElectricalList.PermitList.Add(objElectrical);
                        }
                    }
                    z++;
                }

                if (objPermit.FunUpdateWorkPermit(objPermit, objInList, objHydroList, objExcvHazardList, objExcvSiteList, objExcvSafetyList, objHotTypeList, objHotHazardList, objHotWelfareList, objHotSiteList, objHotSafetyList, objFalseHazardList, objFalseSiteList, objFalseSafetyList, objShaftHazardList, objShaftSiteList, objShaftSafetyList, objPneumaticList, objElectricalList, objConfinedList))
                {
                    TempData["Successdata"] = "Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAccessPermit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("WorkPermitList");
        }

        [AllowAnonymous]
        public ActionResult WorkPermitDetail(FormCollection form)
        {
            ViewBag.SubMenutype = "WorkPermit";
            WorkPermitModels objPermit = new WorkPermitModels();
            try
            {
                ViewBag.Type = objGlobaldata.getWorkPermitTypeList();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Rating = objGlobaldata.GetPermitRating();
                ViewBag.Status = objGlobaldata.GetConstantValue("PermitStatus");
                ViewBag.HydroTest = objGlobaldata.getPermitQuestionList("Hydro Test");
                ViewBag.work = objGlobaldata.GetConstantValue("Permitworktype");
                ViewBag.Pneumatic = objGlobaldata.getPermitQuestionList("Pneumatic");
                ViewBag.Confined = objGlobaldata.getPermitQuestionList("Confined");
                ViewBag.Electrical = objGlobaldata.getPermitQuestionList("Electrical");

                ViewBag.Hazard = objGlobaldata.getPermitSectionQuestionList("Excavation", "Hazards Identified");
                ViewBag.WorkSite = objGlobaldata.getPermitSectionQuestionList("Excavation", "Work Site Precautions");
                ViewBag.Safety = objGlobaldata.getPermitSectionQuestionList("Excavation", "Personal Protection and Safety Equipment ");

                ViewBag.HotType = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Type of work");
                ViewBag.HotHazard = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Hazards Identified");
                ViewBag.Welfare = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Welfare");
                ViewBag.HotWorkSite = objGlobaldata.getPermitSectionQuestionList("Hot Work", "Work Site Precautions");

                ViewBag.FalseHazard = objGlobaldata.getPermitSectionQuestionList("False Work", "Hazards Identified");
                ViewBag.FalseWorkSite = objGlobaldata.getPermitSectionQuestionList("False Work", "Work Site Precautions");
                ViewBag.FalseSafety = objGlobaldata.getPermitSectionQuestionList("False Work", "Personal Protection and Safety Equipment");

                ViewBag.ShaftHazard = objGlobaldata.getPermitSectionQuestionList("Shaft Work", "Hazards Identified");
                ViewBag.ShaftWorkSite = objGlobaldata.getPermitSectionQuestionList("Shaft Work", "Work Site Precautions");
                string id_work_permit = form["id_work_permit"];
                if (id_work_permit != null && id_work_permit != "")
                {
                    string sSqlstmt = "select id_work_permit,permit_type,permitno,area,location,job_performer,contractor,contactno,section,equipment,"
                    + "jobno,workorder,access_level,no_person,no_days,resp_pers,date_submitted,dept,start_date,finish_date,resp_section,description" +
                    ",upload,pressure,IA,drawings,method_state,rescue_plan,work_level,comments,loggedby,work_status,work_safe,add_comments,completion_date,work_type,branch,permit_location from t_work_permit where id_work_permit='" + id_work_permit + "'";

                    DataSet dsPermitList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPermitList.Tables.Count > 0 && dsPermitList.Tables[0].Rows.Count > 0)
                    {
                        objPermit = new WorkPermitModels
                        {
                            id_work_permit = dsPermitList.Tables[0].Rows[0]["id_work_permit"].ToString(),
                            permit_type = (dsPermitList.Tables[0].Rows[0]["permit_type"].ToString()),
                            permitno = dsPermitList.Tables[0].Rows[0]["permitno"].ToString(),
                            area = dsPermitList.Tables[0].Rows[0]["area"].ToString(),
                            permit_location = objGlobaldata.GetDivisionLocationById(dsPermitList.Tables[0].Rows[0]["permit_location"].ToString()),
                            job_performer = dsPermitList.Tables[0].Rows[0]["job_performer"].ToString(),
                            contractor = dsPermitList.Tables[0].Rows[0]["contractor"].ToString(),
                            contactno = dsPermitList.Tables[0].Rows[0]["contactno"].ToString(),
                            section = dsPermitList.Tables[0].Rows[0]["section"].ToString(),
                            equipment = dsPermitList.Tables[0].Rows[0]["equipment"].ToString(),
                            jobno = dsPermitList.Tables[0].Rows[0]["jobno"].ToString(),
                            workorder = dsPermitList.Tables[0].Rows[0]["workorder"].ToString(),
                            access_level = dsPermitList.Tables[0].Rows[0]["access_level"].ToString(),
                            no_person = dsPermitList.Tables[0].Rows[0]["no_person"].ToString(),
                            no_days = dsPermitList.Tables[0].Rows[0]["no_days"].ToString(),
                            resp_pers = objGlobaldata.GetEmpHrNameById(dsPermitList.Tables[0].Rows[0]["resp_pers"].ToString()),
                            resp_section = dsPermitList.Tables[0].Rows[0]["resp_section"].ToString(),
                            description = dsPermitList.Tables[0].Rows[0]["description"].ToString(),
                            upload = dsPermitList.Tables[0].Rows[0]["upload"].ToString(),
                            pressure = dsPermitList.Tables[0].Rows[0]["pressure"].ToString(),
                            IA = objGlobaldata.GetEmpHrNameById(dsPermitList.Tables[0].Rows[0]["IA"].ToString()),
                            drawings = dsPermitList.Tables[0].Rows[0]["drawings"].ToString(),
                            method_state = dsPermitList.Tables[0].Rows[0]["method_state"].ToString(),
                            rescue_plan = dsPermitList.Tables[0].Rows[0]["rescue_plan"].ToString(),
                            work_level = dsPermitList.Tables[0].Rows[0]["work_level"].ToString(),
                            comments = dsPermitList.Tables[0].Rows[0]["comments"].ToString(),
                            work_status = dsPermitList.Tables[0].Rows[0]["work_status"].ToString(),
                            work_safe = dsPermitList.Tables[0].Rows[0]["work_safe"].ToString(),
                            add_comments = dsPermitList.Tables[0].Rows[0]["add_comments"].ToString(),
                            work_type = dsPermitList.Tables[0].Rows[0]["work_type"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPermitList.Tables[0].Rows[0]["branch"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsPermitList.Tables[0].Rows[0]["dept"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsPermitList.Tables[0].Rows[0]["location"].ToString()),
                            loggedby = dsPermitList.Tables[0].Rows[0]["loggedby"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsPermitList.Tables[0].Rows[0]["start_date"].ToString() != ""
                           && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["start_date"].ToString(), out dtDocDate))
                        {
                            objPermit.start_date = dtDocDate;
                        }
                        if (dsPermitList.Tables[0].Rows[0]["finish_date"].ToString() != ""
                        && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["finish_date"].ToString(), out dtDocDate))
                        {
                            objPermit.finish_date = dtDocDate;
                        }
                        if (dsPermitList.Tables[0].Rows[0]["date_submitted"].ToString() != ""
                      && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["date_submitted"].ToString(), out dtDocDate))
                        {
                            objPermit.date_submitted = dtDocDate;
                        }
                        if (dsPermitList.Tables[0].Rows[0]["completion_date"].ToString() != ""
                   && DateTime.TryParse(dsPermitList.Tables[0].Rows[0]["completion_date"].ToString(), out dtDocDate))
                        {
                            objPermit.completion_date = dtDocDate;
                        }

                        CompanyModels objCompany = new CompanyModels();
                        dsPermitList = objCompany.GetCompanyDetailsForReport(dsPermitList);

                        dsPermitList = objGlobaldata.GetReportDetails(dsPermitList, objPermit.permitno, objPermit.loggedby, "WORK PERMIT REPORT");
                        ViewBag.CompanyInfo = dsPermitList;

                        ViewBag.Permit = objPermit;
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("WorkPermitList");
                    }

                    WorkPermitModelsList objInList = new WorkPermitModelsList();
                    objInList.PermitList = new List<WorkPermitModels>();

                    sSqlstmt = "select id_work_in,id_work_permit,pers_name,idno,date_in from t_work_in where id_work_permit='" + id_work_permit + "'";

                    DataSet dsInList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsInList.Tables.Count > 0 && dsInList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsInList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                WorkPermitModels objInPermit = new WorkPermitModels
                                {
                                    id_work_in = dsInList.Tables[0].Rows[i]["id_work_in"].ToString(),
                                    id_work_permit = dsInList.Tables[0].Rows[i]["id_work_permit"].ToString(),
                                    pers_name = dsInList.Tables[0].Rows[i]["pers_name"].ToString(),
                                    idno = dsInList.Tables[0].Rows[i]["idno"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsInList.Tables[0].Rows[0]["date_in"].ToString() != ""
                                && DateTime.TryParse(dsInList.Tables[0].Rows[0]["date_in"].ToString(), out dtDocDate))
                                {
                                    objInPermit.date_in = dtDocDate;
                                }
                                objInList.PermitList.Add(objInPermit);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in WorkPermitDetail: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("finish_date");
                            }
                        }
                        ViewBag.objInList = objInList;
                    }
                    //Confined
                    string sql18 = "select id_confined,id_work_permit,id_questions,id_ratings from t_permit_confined where id_work_permit='" + id_work_permit + "'";
                    DataSet dsConfined = objGlobaldata.Getdetails(sql18);

                    WorkPermitModelsList objConfinedList = new WorkPermitModelsList();
                    objConfinedList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicObjConfinedList = new Dictionary<string, string>();

                    for (int i = 0; dsConfined.Tables.Count > 0 && i < dsConfined.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objConfined = new WorkPermitModels
                        {
                            id_confined = dsConfined.Tables[0].Rows[i]["id_confined"].ToString(),
                            id_questions = (dsConfined.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsConfined.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicObjConfinedList.Add(dsConfined.Tables[0].Rows[i]["id_questions"].ToString(), dsConfined.Tables[0].Rows[i]["id_ratings"].ToString());
                        objConfinedList.PermitList.Add(objConfined);
                    }
                    ViewBag.objConfinedList = objConfinedList;
                    ViewBag.dicObjConfinedList = dicObjConfinedList;
                    //Hydro
                    string sql1 = "select id_hydro_safety,id_work_permit,id_questions,id_ratings from t_permit_hydro_safety where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHydroSafety = objGlobaldata.Getdetails(sql1);

                    WorkPermitModelsList objHydroList = new WorkPermitModelsList();
                    objHydroList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicObjHydroList = new Dictionary<string, string>();

                    for (int i = 0; dsHydroSafety.Tables.Count > 0 && i < dsHydroSafety.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHydroSafety = new WorkPermitModels
                        {
                            id_hydro_safety = dsHydroSafety.Tables[0].Rows[i]["id_hydro_safety"].ToString(),
                            id_questions = (dsHydroSafety.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHydroSafety.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicObjHydroList.Add(dsHydroSafety.Tables[0].Rows[i]["id_questions"].ToString(), dsHydroSafety.Tables[0].Rows[i]["id_ratings"].ToString());
                        objHydroList.PermitList.Add(objHydroSafety);
                    }
                    ViewBag.objHydroList = objHydroList;
                    ViewBag.dicObjHydroList = dicObjHydroList;

                    //Excavation
                    string sql2 = "select id_excv_hazard,id_work_permit,id_questions,id_ratings from t_permit_excv_hazard where id_work_permit='" + id_work_permit + "'";
                    DataSet dsExcvHazard = objGlobaldata.Getdetails(sql2);

                    WorkPermitModelsList ExcvHazardList = new WorkPermitModelsList();
                    ExcvHazardList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicExcvHazardList = new Dictionary<string, string>();

                    for (int i = 0; dsExcvHazard.Tables.Count > 0 && i < dsExcvHazard.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objExcvHazard = new WorkPermitModels
                        {
                            id_excv_hazard = dsExcvHazard.Tables[0].Rows[i]["id_excv_hazard"].ToString(),
                            id_questions = (dsExcvHazard.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsExcvHazard.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicExcvHazardList.Add(dsExcvHazard.Tables[0].Rows[i]["id_questions"].ToString(), dsExcvHazard.Tables[0].Rows[i]["id_ratings"].ToString());
                        ExcvHazardList.PermitList.Add(objExcvHazard);
                    }
                    ViewBag.ExcvHazardList = ExcvHazardList;
                    ViewBag.dicExcvHazardList = dicExcvHazardList;

                    string sql3 = "select id_excv_site,id_work_permit,id_questions,id_ratings from t_permit_excv_site where id_work_permit='" + id_work_permit + "'";
                    DataSet dsExcvSite = objGlobaldata.Getdetails(sql3);

                    WorkPermitModelsList ExcvSiteList = new WorkPermitModelsList();
                    ExcvSiteList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicExcvSiteList = new Dictionary<string, string>();

                    for (int i = 0; dsExcvSite.Tables.Count > 0 && i < dsExcvSite.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objExcvSite = new WorkPermitModels
                        {
                            id_excv_site = dsExcvSite.Tables[0].Rows[i]["id_excv_site"].ToString(),
                            id_questions = (dsExcvSite.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsExcvSite.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicExcvSiteList.Add(dsExcvSite.Tables[0].Rows[i]["id_questions"].ToString(), dsExcvSite.Tables[0].Rows[i]["id_ratings"].ToString());
                        ExcvSiteList.PermitList.Add(objExcvSite);
                    }
                    ViewBag.ExcvSiteList = ExcvSiteList;
                    ViewBag.dicExcvSiteList = dicExcvSiteList;

                    string sql4 = "select id_excv_safety,id_work_permit,id_questions,id_ratings from t_permit_excv_safety where id_work_permit='" + id_work_permit + "'";
                    DataSet dsExcvSafety = objGlobaldata.Getdetails(sql4);

                    WorkPermitModelsList ExcvSafetyList = new WorkPermitModelsList();
                    ExcvSafetyList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicExcvSafetyList = new Dictionary<string, string>();

                    for (int i = 0; dsExcvSafety.Tables.Count > 0 && i < dsExcvSafety.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objExcvSafety = new WorkPermitModels
                        {
                            id_excv_safety = dsExcvSafety.Tables[0].Rows[i]["id_excv_safety"].ToString(),
                            id_questions = (dsExcvSafety.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsExcvSafety.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicExcvSafetyList.Add(dsExcvSafety.Tables[0].Rows[i]["id_questions"].ToString(), dsExcvSafety.Tables[0].Rows[i]["id_ratings"].ToString());
                        ExcvSafetyList.PermitList.Add(objExcvSafety);
                    }
                    ViewBag.ExcvSafetyList = ExcvSafetyList;
                    ViewBag.dicExcvSafetyList = dicExcvSafetyList;

                    //Hot work
                    string sql5 = "select id_hot_type,id_work_permit,id_questions,id_ratings from t_permit_hot_type where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHotType = objGlobaldata.Getdetails(sql5);

                    WorkPermitModelsList HotTypeList = new WorkPermitModelsList();
                    HotTypeList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicHotTypeList = new Dictionary<string, string>();

                    for (int i = 0; dsHotType.Tables.Count > 0 && i < dsHotType.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHotTypey = new WorkPermitModels
                        {
                            id_hot_type = dsHotType.Tables[0].Rows[i]["id_hot_type"].ToString(),
                            id_questions = (dsHotType.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHotType.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicHotTypeList.Add(dsHotType.Tables[0].Rows[i]["id_questions"].ToString(), dsHotType.Tables[0].Rows[i]["id_ratings"].ToString());
                        HotTypeList.PermitList.Add(objHotTypey);
                    }
                    ViewBag.HotTypeList = HotTypeList;
                    ViewBag.dicHotTypeList = dicHotTypeList;

                    string sql6 = "select id_hot_hazard,id_work_permit,id_questions,id_ratings from t_permit_hot_hazard where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHotHazard = objGlobaldata.Getdetails(sql6);

                    WorkPermitModelsList HotHazardList = new WorkPermitModelsList();
                    HotHazardList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicHotHazardList = new Dictionary<string, string>();

                    for (int i = 0; dsHotHazard.Tables.Count > 0 && i < dsHotHazard.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHotHazard = new WorkPermitModels
                        {
                            id_hot_hazard = dsHotHazard.Tables[0].Rows[i]["id_hot_hazard"].ToString(),
                            id_questions = (dsHotHazard.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHotHazard.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicHotHazardList.Add(dsHotHazard.Tables[0].Rows[i]["id_questions"].ToString(), dsHotHazard.Tables[0].Rows[i]["id_ratings"].ToString());
                        HotHazardList.PermitList.Add(objHotHazard);
                    }
                    ViewBag.HotHazardList = HotHazardList;
                    ViewBag.dicHotHazardList = dicHotHazardList;

                    string sql7 = "select id_hot_welfare,id_work_permit,id_questions,id_ratings from t_permit_hot_welfare where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHotWelfare = objGlobaldata.Getdetails(sql7);

                    WorkPermitModelsList HotWelfareList = new WorkPermitModelsList();
                    HotWelfareList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicHotWelfareList = new Dictionary<string, string>();

                    for (int i = 0; dsHotWelfare.Tables.Count > 0 && i < dsHotWelfare.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHotWelfare = new WorkPermitModels
                        {
                            id_hot_welfare = dsHotWelfare.Tables[0].Rows[i]["id_hot_welfare"].ToString(),
                            id_questions = (dsHotWelfare.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHotWelfare.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicHotWelfareList.Add(dsHotWelfare.Tables[0].Rows[i]["id_questions"].ToString(), dsHotWelfare.Tables[0].Rows[i]["id_ratings"].ToString());
                        HotWelfareList.PermitList.Add(objHotWelfare);
                    }
                    ViewBag.HotWelfareList = HotWelfareList;
                    ViewBag.dicHotWelfareList = dicHotWelfareList;

                    string sql8 = "select id_hot_site,id_work_permit,id_questions,id_ratings from t_permit_hot_site where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHotSite = objGlobaldata.Getdetails(sql8);

                    WorkPermitModelsList HotSiteList = new WorkPermitModelsList();
                    HotSiteList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicHotSiteList = new Dictionary<string, string>();

                    for (int i = 0; dsHotSite.Tables.Count > 0 && i < dsHotSite.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHotSite = new WorkPermitModels
                        {
                            id_hot_site = dsHotSite.Tables[0].Rows[i]["id_hot_site"].ToString(),
                            id_questions = (dsHotSite.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHotSite.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicHotSiteList.Add(dsHotSite.Tables[0].Rows[i]["id_questions"].ToString(), dsHotSite.Tables[0].Rows[i]["id_ratings"].ToString());
                        HotSiteList.PermitList.Add(objHotSite);
                    }
                    ViewBag.HotSiteList = HotSiteList;
                    ViewBag.dicHotSiteList = dicHotSiteList;

                    string sql9 = "select id_hot_safety,id_work_permit,id_questions,id_ratings from t_permit_hot_safety where id_work_permit='" + id_work_permit + "'";
                    DataSet dsHotSafety = objGlobaldata.Getdetails(sql9);

                    WorkPermitModelsList HotSafetyList = new WorkPermitModelsList();
                    HotSafetyList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicHotSafetyList = new Dictionary<string, string>();

                    for (int i = 0; dsHotSafety.Tables.Count > 0 && i < dsHotSafety.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objHotSafety = new WorkPermitModels
                        {
                            id_hot_safety = dsHotSafety.Tables[0].Rows[i]["id_hot_safety"].ToString(),
                            id_questions = (dsHotSafety.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsHotSafety.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicHotSafetyList.Add(dsHotSafety.Tables[0].Rows[i]["id_questions"].ToString(), dsHotSafety.Tables[0].Rows[i]["id_ratings"].ToString());
                        HotSafetyList.PermitList.Add(objHotSafety);
                    }
                    ViewBag.HotSafetyList = HotSafetyList;
                    ViewBag.dicHotSafetyList = dicHotSafetyList;

                    //false work

                    string sql10 = "select id_false_hazard,id_work_permit,id_questions,id_ratings from t_permit_false_hazard where id_work_permit='" + id_work_permit + "'";
                    DataSet dsFalseHazard = objGlobaldata.Getdetails(sql10);

                    WorkPermitModelsList FalseHazardList = new WorkPermitModelsList();
                    FalseHazardList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicFalseHazardList = new Dictionary<string, string>();

                    for (int i = 0; dsFalseHazard.Tables.Count > 0 && i < dsFalseHazard.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objFalseHazard = new WorkPermitModels
                        {
                            id_false_hazard = dsFalseHazard.Tables[0].Rows[i]["id_false_hazard"].ToString(),
                            id_questions = (dsFalseHazard.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsFalseHazard.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicFalseHazardList.Add(dsFalseHazard.Tables[0].Rows[i]["id_questions"].ToString(), dsFalseHazard.Tables[0].Rows[i]["id_ratings"].ToString());
                        FalseHazardList.PermitList.Add(objFalseHazard);
                    }
                    ViewBag.FalseHazardList = FalseHazardList;
                    ViewBag.dicFalseHazardList = dicFalseHazardList;

                    string sql11 = "select id_false_site,id_work_permit,id_questions,id_ratings from t_permit_false_site where id_work_permit='" + id_work_permit + "'";
                    DataSet dsFalseSite = objGlobaldata.Getdetails(sql11);

                    WorkPermitModelsList FalseSiteList = new WorkPermitModelsList();
                    FalseSiteList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicFalseSiteList = new Dictionary<string, string>();

                    for (int i = 0; dsFalseSite.Tables.Count > 0 && i < dsFalseSite.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objFalseSite = new WorkPermitModels
                        {
                            id_false_site = dsFalseSite.Tables[0].Rows[i]["id_false_site"].ToString(),
                            id_questions = (dsFalseSite.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsFalseSite.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicFalseSiteList.Add(dsFalseSite.Tables[0].Rows[i]["id_questions"].ToString(), dsFalseSite.Tables[0].Rows[i]["id_ratings"].ToString());
                        FalseSiteList.PermitList.Add(objFalseSite);
                    }
                    ViewBag.FalseSiteList = FalseSiteList;
                    ViewBag.dicFalseSiteList = dicFalseSiteList;

                    string sql12 = "select id_false_safety,id_work_permit,id_questions,id_ratings from t_permit_false_safety where id_work_permit='" + id_work_permit + "'";
                    DataSet dsFalseSafety = objGlobaldata.Getdetails(sql12);

                    WorkPermitModelsList FalseSafetyList = new WorkPermitModelsList();
                    FalseSafetyList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicFalseSafetyList = new Dictionary<string, string>();

                    for (int i = 0; dsFalseSafety.Tables.Count > 0 && i < dsFalseSafety.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objFalseSafety = new WorkPermitModels
                        {
                            id_false_safety = dsFalseSafety.Tables[0].Rows[i]["id_false_safety"].ToString(),
                            id_questions = (dsFalseSafety.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsFalseSafety.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicFalseSafetyList.Add(dsFalseSafety.Tables[0].Rows[i]["id_questions"].ToString(), dsFalseSafety.Tables[0].Rows[i]["id_ratings"].ToString());
                        FalseSafetyList.PermitList.Add(objFalseSafety);
                    }
                    ViewBag.FalseSafetyList = FalseSafetyList;
                    ViewBag.dicFalseSafetyList = dicFalseSafetyList;

                    //Shaft work

                    string sql13 = "select id_shaft_hazard,id_work_permit,id_questions,id_ratings from t_permit_shaft_hazard where id_work_permit='" + id_work_permit + "'";
                    DataSet dsShaftHazard = objGlobaldata.Getdetails(sql13);

                    WorkPermitModelsList ShaftHazardList = new WorkPermitModelsList();
                    ShaftHazardList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicShaftHazardList = new Dictionary<string, string>();

                    for (int i = 0; dsShaftHazard.Tables.Count > 0 && i < dsShaftHazard.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objShaftHazard = new WorkPermitModels
                        {
                            id_shaft_hazard = dsShaftHazard.Tables[0].Rows[i]["id_shaft_hazard"].ToString(),
                            id_questions = (dsShaftHazard.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsShaftHazard.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicShaftHazardList.Add(dsShaftHazard.Tables[0].Rows[i]["id_questions"].ToString(), dsShaftHazard.Tables[0].Rows[i]["id_ratings"].ToString());
                        ShaftHazardList.PermitList.Add(objShaftHazard);
                    }
                    ViewBag.ShaftHazardList = ShaftHazardList;
                    ViewBag.dicShaftHazardList = dicShaftHazardList;

                    string sql14 = "select id_shaft_site,id_work_permit,id_questions,id_ratings from t_permit_shaft_site where id_work_permit='" + id_work_permit + "'";
                    DataSet dsShaftSite = objGlobaldata.Getdetails(sql14);

                    WorkPermitModelsList ShaftSiteList = new WorkPermitModelsList();
                    ShaftSiteList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicShaftSiteList = new Dictionary<string, string>();

                    for (int i = 0; dsShaftSite.Tables.Count > 0 && i < dsShaftSite.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objShaftSite = new WorkPermitModels
                        {
                            id_shaft_site = dsShaftSite.Tables[0].Rows[i]["id_shaft_site"].ToString(),
                            id_questions = (dsShaftSite.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsShaftSite.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicShaftSiteList.Add(dsShaftSite.Tables[0].Rows[i]["id_questions"].ToString(), dsShaftSite.Tables[0].Rows[i]["id_ratings"].ToString());
                        ShaftSiteList.PermitList.Add(objShaftSite);
                    }
                    ViewBag.ShaftSiteList = ShaftSiteList;
                    ViewBag.dicShaftSiteList = dicShaftSiteList;

                    string sql15 = "select id_shaft_safety,id_work_permit,id_questions,id_ratings from t_permit_shaft_safety where id_work_permit='" + id_work_permit + "'";
                    DataSet dsShaftSafety = objGlobaldata.Getdetails(sql15);

                    WorkPermitModelsList ShaftSafetyList = new WorkPermitModelsList();
                    ShaftSafetyList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicShaftSafetyList = new Dictionary<string, string>();

                    for (int i = 0; dsShaftSafety.Tables.Count > 0 && i < dsShaftSafety.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objShaftSafety = new WorkPermitModels
                        {
                            id_shaft_safety = dsShaftSafety.Tables[0].Rows[i]["id_shaft_safety"].ToString(),
                            id_questions = (dsShaftSafety.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsShaftSafety.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicShaftSafetyList.Add(dsShaftSafety.Tables[0].Rows[i]["id_questions"].ToString(), dsShaftSafety.Tables[0].Rows[i]["id_ratings"].ToString());
                        ShaftSafetyList.PermitList.Add(objShaftSafety);
                    }
                    ViewBag.ShaftSafetyList = ShaftSafetyList;
                    ViewBag.dicShaftSafetyList = dicShaftSafetyList;

                    //Pneumatic

                    string sql16 = "select id_pneumatic,id_work_permit,id_questions,id_ratings from t_permit_pneumatic where id_work_permit='" + id_work_permit + "'";
                    DataSet dsPneumatic = objGlobaldata.Getdetails(sql16);

                    WorkPermitModelsList PneumaticList = new WorkPermitModelsList();
                    PneumaticList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicPneumaticList = new Dictionary<string, string>();

                    for (int i = 0; dsPneumatic.Tables.Count > 0 && i < dsPneumatic.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objPneumatic = new WorkPermitModels
                        {
                            id_pneumatic = dsPneumatic.Tables[0].Rows[i]["id_pneumatic"].ToString(),
                            id_questions = (dsPneumatic.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsPneumatic.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicPneumaticList.Add(dsPneumatic.Tables[0].Rows[i]["id_questions"].ToString(), dsPneumatic.Tables[0].Rows[i]["id_ratings"].ToString());
                        PneumaticList.PermitList.Add(objPneumatic);
                    }
                    ViewBag.PneumaticList = PneumaticList;
                    ViewBag.dicPneumaticList = dicPneumaticList;

                    //Electrical

                    string sql17 = "select id_electrical,id_work_permit,id_questions,id_ratings from t_permit_electrical where id_work_permit='" + id_work_permit + "'";
                    DataSet dsElectrical = objGlobaldata.Getdetails(sql17);

                    WorkPermitModelsList ElectricalList = new WorkPermitModelsList();
                    ElectricalList.PermitList = new List<WorkPermitModels>();

                    Dictionary<string, string> dicElectricalList = new Dictionary<string, string>();

                    for (int i = 0; dsElectrical.Tables.Count > 0 && i < dsElectrical.Tables[0].Rows.Count; i++)
                    {
                        WorkPermitModels objElectrical = new WorkPermitModels
                        {
                            id_electrical = dsElectrical.Tables[0].Rows[i]["id_electrical"].ToString(),
                            id_questions = (dsElectrical.Tables[0].Rows[i]["id_questions"].ToString()),
                            id_ratings = (dsElectrical.Tables[0].Rows[i]["id_ratings"].ToString())
                        };

                        dicElectricalList.Add(dsElectrical.Tables[0].Rows[i]["id_questions"].ToString(), dsElectrical.Tables[0].Rows[i]["id_ratings"].ToString());
                        ElectricalList.PermitList.Add(objElectrical);
                    }
                    ViewBag.ElectricalList = ElectricalList;
                    ViewBag.dicElectricalList = dicElectricalList;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in WorkPermitDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("finish_date");
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("WorkPermitPdf")
            {
                FileName = "WorkPermitPdf.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [AllowAnonymous]
        public JsonResult WorkPermitDelete(FormCollection form)
        {
            try
            {
                if (form["id_work_permit"] != null && form["id_work_permit"] != "")
                {
                    WorkPermitModels Doc = new WorkPermitModels();
                    string id_work_permit = form["id_work_permit"];
                    if (Doc.FunDeleteWorkPermit(id_work_permit))
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
                objGlobaldata.AddFunctionalLog("Exception in WorkPermitDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult AccessPermitDelete(FormCollection form)
        {
            try
            {
                if (form["id_access_permit"] != null && form["id_access_permit"] != "")
                {
                    WorkPermitModels Doc = new WorkPermitModels();
                    string id_access_permit = form["id_access_permit"];
                    if (Doc.FunDeleteAccessPermit(id_access_permit))
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
                objGlobaldata.AddFunctionalLog("Exception in WorkPermitDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public JsonResult AccessPermitApproveRejectNoty(string id_access_permit, string iStatus, string PendingFlg, string approver_comment)
        {
            try
            {
                WorkPermitModels objModel = new WorkPermitModels();

                if (objModel.FunAccessPermitApproveOrReject(id_access_permit, iStatus, approver_comment))
                {
                    return Json("Success" + iStatus);
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitApproveReject: " + ex.ToString());
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