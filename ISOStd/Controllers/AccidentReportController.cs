using ISOStd.Models;
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
    public class AccidentReportController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public AccidentReportController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "AccidentReport";
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddAccidentReport()
        {
            AccidentReportModels objAccident = new AccidentReportModels();

            try
            {
                IncidentReportModels objInc = new IncidentReportModels();

                ViewBag.SubMenutype = "AccidentReport";
                objAccident.branch = objGlobaldata.GetCurrentUserSession().division;
                objAccident.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objAccident.location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objAccident.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objAccident.branch);
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.AccidentType = objGlobaldata.GetDropdownList("Injuries Type");
                ViewBag.Incident_Type = objGlobaldata.GetDropdownList("Incident Type");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAccidentReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAccident);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddAccidentReport(FormCollection form, AccidentReportModels objAccident, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                ViewBag.SubMenutype = "AccidentReport";
                DateTime dateValue;

                objAccident.reported_by = form["reported_by"];
                objAccident.branch = form["branch"];
                objAccident.location = form["location"];
                objAccident.Department = form["Department"];

                if (form["acc_date"] != null && DateTime.TryParse(form["acc_date"], out dateValue) == true)
                {
                    objAccident.acc_date = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objAccident.acc_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (DateTime.TryParse(form["reported_date"], out dateValue) == true)
                {
                    objAccident.reported_date = dateValue;
                }

                HttpPostedFileBase files = Request.Files[0];
                if (upload != null && files.ContentLength > 0)
                {
                    objAccident.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/AccidentRpt"), Path.GetFileName(file.FileName));
                            string sFilename = "AccidentRpt" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objAccident.upload = objAccident.upload + "," + "~/DataUpload/MgmtDocs/AccidentRpt/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddAccidentReport-upload: " + ex.ToString());
                        }
                    }
                    objAccident.upload = objAccident.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                AccidentReportModelsList objAccTypeList = new AccidentReportModelsList();
                objAccTypeList.AccidentList = new List<AccidentReportModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    AccidentReportModels objAccRpt = new AccidentReportModels();

                    objAccRpt.injury_type = form["injury_type " + i];
                    objAccRpt.no_person = form["no_person " + i];
                    objAccRpt.pers_name = form["pers_name " + i];
                    objAccTypeList.AccidentList.Add(objAccRpt);
                }

                AccidentReportModelsList objAccInfoList = new AccidentReportModelsList();
                objAccInfoList.AccidentList = new List<AccidentReportModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    AccidentReportModels AudModel = new AccidentReportModels();

                    AudModel.reported_to = form["reported_to " + i];
                    // AudModel.reported_date = form["reported_date" + i];
                    AudModel.comments = form["comments " + i];

                    if (DateTime.TryParse(form["reportedon_date " + i], out dateValue) == true)
                    {
                        AudModel.reportedon_date = dateValue;
                    }

                    objAccInfoList.AccidentList.Add(AudModel);
                }

                if (objAccident.FunAddAccidentReport(objAccident, objAccTypeList, objAccInfoList))
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
                objGlobaldata.AddFunctionalLog("Exception in AddAccidentReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AccidentList");
        }

        [AllowAnonymous]
        public ActionResult AccidentList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "AccidentReport";
            AccidentReportModelsList objAttList = new AccidentReportModelsList();
            objAttList.AccidentList = new List<AccidentReportModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_accident_rept,report_no,acc_date,reported_date,reported_by,location,details,upload," +
                    "damage,invest_need,justify,invest_reportno,Actions_Taken,branch,Department from t_accident_report where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + " order by id_accident_rept desc";

                DataSet dsAccidentList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsAccidentList.Tables.Count > 0 && dsAccidentList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsAccidentList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            AccidentReportModels objAccidentReportModels = new AccidentReportModels
                            {
                                id_accident_rept = dsAccidentList.Tables[0].Rows[i]["id_accident_rept"].ToString(),
                                report_no = dsAccidentList.Tables[0].Rows[i]["report_no"].ToString(),
                                reported_by = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[i]["reported_by"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsAccidentList.Tables[0].Rows[i]["location"].ToString()),
                                details = dsAccidentList.Tables[0].Rows[i]["details"].ToString(),
                                upload = dsAccidentList.Tables[0].Rows[i]["upload"].ToString(),
                                damage = dsAccidentList.Tables[0].Rows[i]["damage"].ToString(),
                                invest_need = dsAccidentList.Tables[0].Rows[i]["invest_need"].ToString(),
                                justify = dsAccidentList.Tables[0].Rows[i]["justify"].ToString(),
                                invest_reportno = dsAccidentList.Tables[0].Rows[i]["invest_reportno"].ToString(),
                                Actions_Taken = dsAccidentList.Tables[0].Rows[i]["Actions_Taken"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAccidentList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsAccidentList.Tables[0].Rows[i]["Department"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsAccidentList.Tables[0].Rows[i]["acc_date"].ToString() != ""
                               && DateTime.TryParse(dsAccidentList.Tables[0].Rows[i]["acc_date"].ToString(), out dtDocDate))
                            {
                                objAccidentReportModels.acc_date = dtDocDate;
                            }
                            if (dsAccidentList.Tables[0].Rows[i]["reported_date"].ToString() != ""
                              && DateTime.TryParse(dsAccidentList.Tables[0].Rows[i]["reported_date"].ToString(), out dtDocDate))
                            {
                                objAccidentReportModels.reported_date = dtDocDate;
                            }
                            objAttList.AccidentList.Add(objAccidentReportModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AccidentList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccidentList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objAttList.AccidentList.ToList());
        }

        [AllowAnonymous]
        public JsonResult AccidentListSearch(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "AccidentReport";
            AccidentReportModelsList objAttList = new AccidentReportModelsList();
            objAttList.AccidentList = new List<AccidentReportModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_accident_rept,report_no,acc_date,reported_date,reported_by,location,details,upload," +
                    "damage,invest_need,justify,invest_reportno,Actions_Taken,branch,Department from t_accident_report where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + " order by id_accident_rept desc";

                DataSet dsAccidentList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsAccidentList.Tables.Count > 0 && dsAccidentList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsAccidentList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            AccidentReportModels objAccidentReportModels = new AccidentReportModels
                            {
                                id_accident_rept = dsAccidentList.Tables[0].Rows[i]["id_accident_rept"].ToString(),
                                report_no = dsAccidentList.Tables[0].Rows[i]["report_no"].ToString(),
                                reported_by = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[i]["reported_by"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsAccidentList.Tables[0].Rows[i]["location"].ToString()),
                                details = dsAccidentList.Tables[0].Rows[i]["details"].ToString(),
                                upload = dsAccidentList.Tables[0].Rows[i]["upload"].ToString(),
                                damage = dsAccidentList.Tables[0].Rows[i]["damage"].ToString(),
                                invest_need = dsAccidentList.Tables[0].Rows[i]["invest_need"].ToString(),
                                justify = dsAccidentList.Tables[0].Rows[i]["justify"].ToString(),
                                invest_reportno = dsAccidentList.Tables[0].Rows[i]["invest_reportno"].ToString(),
                                Actions_Taken = dsAccidentList.Tables[0].Rows[i]["Actions_Taken"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAccidentList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsAccidentList.Tables[0].Rows[i]["Department"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsAccidentList.Tables[0].Rows[i]["acc_date"].ToString() != ""
                               && DateTime.TryParse(dsAccidentList.Tables[0].Rows[i]["acc_date"].ToString(), out dtDocDate))
                            {
                                objAccidentReportModels.acc_date = dtDocDate;
                            }
                            if (dsAccidentList.Tables[0].Rows[i]["reported_date"].ToString() != ""
                              && DateTime.TryParse(dsAccidentList.Tables[0].Rows[i]["reported_date"].ToString(), out dtDocDate))
                            {
                                objAccidentReportModels.reported_date = dtDocDate;
                            }
                            objAttList.AccidentList.Add(objAccidentReportModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AccidentList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccidentList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult AccidentReportEdit()
        {
            ViewBag.SubMenutype = "AccidentReport";
            AccidentReportModels objAccidentReportModels = new AccidentReportModels();
            try
            {
                IncidentReportModels objInc = new IncidentReportModels();

                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.AccidentType = objGlobaldata.GetDropdownList("Injuries Type");
                ViewBag.Incident_Type = objGlobaldata.GetDropdownList("Incident Type");
                if (Request.QueryString["id_accident_rept"] != null && Request.QueryString["id_accident_rept"] != "")
                {
                    string id_accident_rept = Request.QueryString["id_accident_rept"];
                    string sSqlstmt = "select id_accident_rept,report_no,acc_date,reported_date,reported_by,location,details,upload," +
                    "damage,invest_need,justify,invest_reportno,accident_place,Incident_Type,Actions_Taken,branch,Department,eqp_involved,job_type,ppe from t_accident_report where id_accident_rept ='" + id_accident_rept + "'";

                    DataSet dsAccidentList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAccidentList.Tables.Count > 0 && dsAccidentList.Tables[0].Rows.Count > 0)
                    {
                        objAccidentReportModels = new AccidentReportModels
                        {
                            id_accident_rept = dsAccidentList.Tables[0].Rows[0]["id_accident_rept"].ToString(),
                            report_no = dsAccidentList.Tables[0].Rows[0]["report_no"].ToString(),
                            reported_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsAccidentList.Tables[0].Rows[0]["reported_by"].ToString()),
                            location =/* objGlobaldata.GetDivisionLocationById*/(dsAccidentList.Tables[0].Rows[0]["location"].ToString()),
                            details = dsAccidentList.Tables[0].Rows[0]["details"].ToString(),
                            upload = dsAccidentList.Tables[0].Rows[0]["upload"].ToString(),
                            damage = dsAccidentList.Tables[0].Rows[0]["damage"].ToString(),
                            invest_need = dsAccidentList.Tables[0].Rows[0]["invest_need"].ToString(),
                            justify = dsAccidentList.Tables[0].Rows[0]["justify"].ToString(),
                            invest_reportno = dsAccidentList.Tables[0].Rows[0]["invest_reportno"].ToString(),
                            accident_place = dsAccidentList.Tables[0].Rows[0]["accident_place"].ToString(),
                            Incident_Type = dsAccidentList.Tables[0].Rows[0]["Incident_Type"].ToString(),
                            Actions_Taken = dsAccidentList.Tables[0].Rows[0]["Actions_Taken"].ToString(),
                            branch = (dsAccidentList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsAccidentList.Tables[0].Rows[0]["Department"].ToString()),
                            eqp_involved = (dsAccidentList.Tables[0].Rows[0]["eqp_involved"].ToString()),
                            job_type = (dsAccidentList.Tables[0].Rows[0]["job_type"].ToString()),
                            ppe = (dsAccidentList.Tables[0].Rows[0]["ppe"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString() != ""
                              && DateTime.TryParse(dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString(), out dtDocDate))
                        {
                            objAccidentReportModels.acc_date = dtDocDate;
                        }
                        if (dsAccidentList.Tables[0].Rows[0]["reported_date"].ToString() != ""
                          && DateTime.TryParse(dsAccidentList.Tables[0].Rows[0]["reported_date"].ToString(), out dtDocDate))
                        {
                            objAccidentReportModels.reported_date = dtDocDate;
                        }
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsAccidentList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsAccidentList.Tables[0].Rows[0]["branch"].ToString());
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("AccidentList");
                    }

                    AccidentReportModelsList objInfoList = new AccidentReportModelsList();
                    objInfoList.AccidentList = new List<AccidentReportModels>();

                    sSqlstmt = "select id_accident_info,id_accident_rept,reported_to,reportedon_date,comments from t_accident_info where id_accident_rept='" + id_accident_rept + "'";

                    DataSet dsAttnList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAttnList.Tables.Count > 0 && dsAttnList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsAttnList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                AccidentReportModels objAccTypeList = new AccidentReportModels
                                {
                                    id_accident_info = dsAttnList.Tables[0].Rows[i]["id_accident_info"].ToString(),
                                    id_accident_rept = dsAttnList.Tables[0].Rows[i]["id_accident_rept"].ToString(),
                                    reported_to = /*objGlobaldata.GetMultiHrEmpNameById*/(dsAttnList.Tables[0].Rows[i]["reported_to"].ToString()),
                                    comments = dsAttnList.Tables[0].Rows[i]["comments"].ToString(),
                                };

                                DateTime dtDocDate1;
                                if (dsAttnList.Tables[0].Rows[0]["reportedon_date"].ToString() != ""
                         && DateTime.TryParse(dsAttnList.Tables[0].Rows[0]["reportedon_date"].ToString(), out dtDocDate1))
                                {
                                    objAccTypeList.reportedon_date = dtDocDate1;
                                }

                                objInfoList.AccidentList.Add(objAccTypeList);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AccidentReportEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AccidentList");
                            }
                        }
                        ViewBag.objAccTypeList = objInfoList;
                    }

                    AccidentReportModelsList objReporterList = new AccidentReportModelsList();
                    objReporterList.AccidentList = new List<AccidentReportModels>();

                    sSqlstmt = "select id_accident_type,id_accident_rept,injury_type,no_person,pers_name from t_accident_type where id_accident_rept='" + id_accident_rept + "'";

                    DataSet dsReportList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsReportList.Tables.Count > 0 && dsReportList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsReportList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                AccidentReportModels objAccInfoList = new AccidentReportModels
                                {
                                    id_accident_type = dsReportList.Tables[0].Rows[i]["id_accident_type"].ToString(),
                                    id_accident_rept = dsReportList.Tables[0].Rows[i]["id_accident_rept"].ToString(),
                                    injury_type = /*objGlobaldata.GetAccidentTypeById*/(dsReportList.Tables[0].Rows[i]["injury_type"].ToString()),
                                    no_person = dsReportList.Tables[0].Rows[i]["no_person"].ToString(),
                                    pers_name = dsReportList.Tables[0].Rows[i]["pers_name"].ToString(),
                                };
                                objReporterList.AccidentList.Add(objAccInfoList);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AccidentReportEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AccidentList");
                            }
                        }
                        ViewBag.objReporterList = objReporterList;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccidentReportEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AccidentList");
            }
            return View(objAccidentReportModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AccidentReportEdit(FormCollection form, AccidentReportModels objAccident, IEnumerable<HttpPostedFileBase> upload)
        {
            ViewBag.SubMenutype = "AccidentReport";
            try
            {
                DateTime dateValue;

                string QCDelete = Request.Form["QCDocsValselectall"];

                objAccident.reported_by = form["reported_by"];
                objAccident.branch = form["branch"];
                objAccident.location = form["location"];
                objAccident.Department = form["Department"];

                if (DateTime.TryParse(form["acc_date"], out dateValue) == true)
                {
                    objAccident.acc_date = dateValue;
                }

                if (DateTime.TryParse(form["reported_date"], out dateValue) == true)
                {
                    objAccident.reported_date = dateValue;
                }

                HttpPostedFileBase files = Request.Files[0];

                if (upload != null && files.ContentLength > 0)
                {
                    objAccident.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/AccidentRpt"), Path.GetFileName(file.FileName));
                            string sFilename = "Accident" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objAccident.upload = objAccident.upload + "," + "~/DataUpload/MgmtDocs/AccidentRpt/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AccidentReportEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objAccident.upload = objAccident.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objAccident.upload = objAccident.upload + "," + form["QCDocsVal"];
                    objAccident.upload = objAccident.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objAccident.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objAccident.upload = null;
                }
                AccidentReportModelsList objAccTypeList = new AccidentReportModelsList();
                objAccTypeList.AccidentList = new List<AccidentReportModels>();

                //for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                //{
                //    if (form["injury_type " + i] != null && form["no_person " + i] != null)
                //    {
                //        AccidentReportModels objAccRpt = new AccidentReportModels();
                //        objAccRpt.injury_type = form["injury_type " + i];
                //        objAccRpt.no_person = form["no_person " + i];

                //        objAccTypeList.AccidentList.Add(objAccRpt);
                //    }
                //}

                //if (objAccident.injury_type != null)
                //{
                //    AccidentReportModels objAccRpt = new AccidentReportModels();
                //    objAccRpt.injury_type = form["injury_type "];
                //    objAccRpt.no_person = form["no_person "];

                //    objAccTypeList.AccidentList.Add(objAccRpt);
                //}

                int iCnt = 0;
                if (form["itemcnt"] != null && form["itemcnt"] != "" && int.TryParse(form["itemcnt"], out iCnt))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["injury_type " + i] != null || form["no_person " + i] != null)
                        {
                            AccidentReportModels objAccRpt = new AccidentReportModels
                            {
                                id_accident_type = form["id_accident_type " + i],
                                injury_type = form["injury_type " + i],
                                no_person = form["no_person " + i],
                                pers_name = form["pers_name " + i],
                            };
                            objAccTypeList.AccidentList.Add(objAccRpt);
                        }
                    }
                }

                AccidentReportModelsList objAccInfoList = new AccidentReportModelsList();
                objAccInfoList.AccidentList = new List<AccidentReportModels>();
                //for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                //{
                //    if (form["reported_to " + i] != null && form["comments " + i] != null)
                //    {
                //        AccidentReportModels AuditorModel = new AccidentReportModels();

                //        AuditorModel.reported_to = form["reported_to " + i];
                //        AuditorModel.comments = form["comments " + i];

                //        if (DateTime.TryParse(form["reportedon_date " + i], out dateValue) == true)
                //        {
                //            AuditorModel.reportedon_date = dateValue;
                //        }

                //        objAccInfoList.AccidentList.Add(AuditorModel);
                //    }
                //}

                //if (objAccident.reported_to != null)
                //{
                //    AccidentReportModels AuditorModel = new AccidentReportModels();

                //    AuditorModel.reported_to = form["reported_to "];
                //    AuditorModel.comments = form["comments "];

                //    if (DateTime.TryParse(form["reportedon_date "], out dateValue) == true)
                //    {
                //        AuditorModel.reportedon_date = dateValue;
                //    }
                //    objAccInfoList.AccidentList.Add(AuditorModel);
                //}
                int iCnts = 0;
                DateTime dateValue1;
                if (form["itemcnt1"] != null && form["itemcnt1"] != "" && int.TryParse(form["itemcnt1"], out iCnts))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                    {
                        if (form["reported_to " + i] != null || form["comments " + i] != null)
                        {
                            AccidentReportModels objAccRpt = new AccidentReportModels
                            {
                                id_accident_info = form["id_accident_info" + i],
                                reported_to = form["reported_to " + i],
                                comments = form["comments " + i],
                            };
                            if (DateTime.TryParse(form["reportedon_date " + i], out dateValue1) == true)
                            {
                                objAccRpt.reportedon_date = dateValue1;
                            }
                            objAccInfoList.AccidentList.Add(objAccRpt);
                        }
                    }
                }

                if (objAccident.FunUpdateAccidentReport(objAccident, objAccTypeList, objAccInfoList))
                {
                    TempData["Successdata"] = "Report updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccidentReportEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AccidentList");
        }

        [AllowAnonymous]
        public ActionResult AccidentReportInfo(int id)
        {
            ViewBag.SubMenutype = "AccidentReport";
            AccidentReportModels objAccidentReportModels = new AccidentReportModels();
            try
            {
                string sSqlstmt = "select id_accident_rept,report_no,acc_date,reported_date,reported_by,location,details,upload," +
                "damage,invest_need,justify,invest_reportno,Actions_Taken,branch,Department from t_accident_report where id_accident_rept ='" + id + "'";

                DataSet dsAccidentList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsAccidentList.Tables.Count > 0 && dsAccidentList.Tables[0].Rows.Count > 0)
                {
                    objAccidentReportModels = new AccidentReportModels
                    {
                        id_accident_rept = dsAccidentList.Tables[0].Rows[0]["id_accident_rept"].ToString(),
                        report_no = dsAccidentList.Tables[0].Rows[0]["report_no"].ToString(),
                        reported_by = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[0]["reported_by"].ToString()),
                        location = objGlobaldata.GetDivisionLocationById(dsAccidentList.Tables[0].Rows[0]["location"].ToString()),
                        details = dsAccidentList.Tables[0].Rows[0]["details"].ToString(),
                        upload = dsAccidentList.Tables[0].Rows[0]["upload"].ToString(),
                        damage = dsAccidentList.Tables[0].Rows[0]["damage"].ToString(),
                        invest_need = dsAccidentList.Tables[0].Rows[0]["invest_need"].ToString(),
                        justify = dsAccidentList.Tables[0].Rows[0]["justify"].ToString(),
                        invest_reportno = dsAccidentList.Tables[0].Rows[0]["invest_reportno"].ToString(),
                        Actions_Taken = dsAccidentList.Tables[0].Rows[0]["Actions_Taken"].ToString(),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAccidentList.Tables[0].Rows[0]["branch"].ToString()),
                        Department = objGlobaldata.GetMultiDeptNameById(dsAccidentList.Tables[0].Rows[0]["Department"].ToString()),
                    };
                    DateTime dtDocDate;
                    if (dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString() != ""
                          && DateTime.TryParse(dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString(), out dtDocDate))
                    {
                        objAccidentReportModels.acc_date = dtDocDate;
                    }
                    if (dsAccidentList.Tables[0].Rows[0]["reported_date"].ToString() != ""
                      && DateTime.TryParse(dsAccidentList.Tables[0].Rows[0]["reported_date"].ToString(), out dtDocDate))
                    {
                        objAccidentReportModels.reported_date = dtDocDate;
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("AccidentList");
                }

                //AccidentReportModelsList objInfoList = new AccidentReportModelsList();
                //objInfoList.AccidentList = new List<AccidentReportModels>();

                //sSqlstmt = "select id_accident_info,id_accident_rept,reported_to,reportedon_date,comments from t_accident_info where id_accident_rept='" + id_accident_rept + "'";

                //DataSet dsAttnList = objGlobaldata.Getdetails(sSqlstmt);
                //if (dsAttnList.Tables.Count > 0 && dsAttnList.Tables[0].Rows.Count > 0)
                //{
                //    for (int i = 0; i < dsAttnList.Tables[0].Rows.Count; i++)
                //    {
                //        try
                //        {
                //            AccidentReportModels objAccTypeList = new AccidentReportModels
                //            {
                //                id_accident_info = dsAttnList.Tables[0].Rows[i]["id_accident_info"].ToString(),
                //                id_accident_rept = dsAttnList.Tables[0].Rows[i]["id_accident_rept"].ToString(),
                //                reported_to = /*objGlobaldata.GetMultiHrEmpNameById*/(dsAttnList.Tables[0].Rows[i]["reported_to"].ToString()),
                //                comments = dsAttnList.Tables[0].Rows[i]["comments"].ToString(),
                //            };

                //            DateTime dtDocDate1;
                //            if (dsAttnList.Tables[0].Rows[0]["reportedon_date"].ToString() != ""
                //     && DateTime.TryParse(dsAttnList.Tables[0].Rows[0]["reportedon_date"].ToString(), out dtDocDate1))
                //            {
                //                objAccTypeList.reportedon_date = dtDocDate1;
                //            }

                //            objInfoList.AccidentList.Add(objAccTypeList);
                //        }
                //        catch (Exception ex)
                //        {
                //            objGlobaldata.AddFunctionalLog("Exception in AccidentReportInfo: " + ex.ToString());
                //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                //            return RedirectToAction("AccidentList");
                //        }
                //    }
                //    ViewBag.objAccTypeList = objInfoList;
                //}

                //AccidentReportModelsList objReporterList = new AccidentReportModelsList();
                //objReporterList.AccidentList = new List<AccidentReportModels>();

                //sSqlstmt = "select id_accident_type,id_accident_rept,injury_type,no_person from t_accident_type where id_accident_rept='" + id_accident_rept + "'";

                //DataSet dsReportList = objGlobaldata.Getdetails(sSqlstmt);
                //if (dsReportList.Tables.Count > 0 && dsReportList.Tables[0].Rows.Count > 0)
                //{
                //    for (int i = 0; i < dsReportList.Tables[0].Rows.Count; i++)
                //    {
                //        try
                //        {
                //            AccidentReportModels objAccInfoList = new AccidentReportModels
                //            {
                //                id_accident_type = dsReportList.Tables[0].Rows[i]["id_accident_type"].ToString(),
                //                id_accident_rept = dsReportList.Tables[0].Rows[i]["id_accident_rept"].ToString(),
                //                injury_type = /*objGlobaldata.GetAccidentTypeById*/(dsReportList.Tables[0].Rows[i]["injury_type"].ToString()),
                //                no_person = dsReportList.Tables[0].Rows[i]["no_person"].ToString(),
                //            };
                //            objReporterList.AccidentList.Add(objAccInfoList);
                //        }
                //        catch (Exception ex)
                //        {
                //            objGlobaldata.AddFunctionalLog("Exception in AccidentReportEdit: " + ex.ToString());
                //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                //            return RedirectToAction("AccidentList");
                //        }
                //    }
                //    ViewBag.objReporterList = objReporterList;
                //}
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccidentReportInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AccidentList");
            }
            return View(objAccidentReportModels);
        }

        [AllowAnonymous]
        public ActionResult AccidentReportDetails()
        {
            ViewBag.SubMenutype = "AccidentReport";
            AccidentReportModels objAccidentReportModels = new AccidentReportModels();
            IncidentReportModels objinc = new IncidentReportModels();
            try
            {
                if (Request.QueryString["id_accident_rept"] != null && Request.QueryString["id_accident_rept"] != "")
                {
                    string id_accident_rept = Request.QueryString["id_accident_rept"];
                    string sSqlstmt = "select id_accident_rept,report_no,acc_date,reported_date,reported_by,location,details,upload," +
                    "damage,invest_need,justify,invest_reportno,accident_place,Incident_Type,Actions_Taken,branch,Department,eqp_involved,job_type,ppe from t_accident_report where id_accident_rept ='" + id_accident_rept + "'";

                    DataSet dsAccidentList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAccidentList.Tables.Count > 0 && dsAccidentList.Tables[0].Rows.Count > 0)
                    {
                        objAccidentReportModels = new AccidentReportModels
                        {
                            id_accident_rept = dsAccidentList.Tables[0].Rows[0]["id_accident_rept"].ToString(),
                            report_no = dsAccidentList.Tables[0].Rows[0]["report_no"].ToString(),
                            reported_by = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[0]["reported_by"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsAccidentList.Tables[0].Rows[0]["location"].ToString()),
                            details = dsAccidentList.Tables[0].Rows[0]["details"].ToString(),
                            upload = dsAccidentList.Tables[0].Rows[0]["upload"].ToString(),
                            damage = dsAccidentList.Tables[0].Rows[0]["damage"].ToString(),
                            invest_need = dsAccidentList.Tables[0].Rows[0]["invest_need"].ToString(),
                            justify = dsAccidentList.Tables[0].Rows[0]["justify"].ToString(),
                            invest_reportno = dsAccidentList.Tables[0].Rows[0]["invest_reportno"].ToString(),
                            accident_place = dsAccidentList.Tables[0].Rows[0]["accident_place"].ToString(),
                            Incident_Type = objinc.GetIncidentTypeNameById(dsAccidentList.Tables[0].Rows[0]["Incident_Type"].ToString()),
                            Actions_Taken = dsAccidentList.Tables[0].Rows[0]["Actions_Taken"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAccidentList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsAccidentList.Tables[0].Rows[0]["Department"].ToString()),
                            eqp_involved = dsAccidentList.Tables[0].Rows[0]["eqp_involved"].ToString(),
                            job_type = dsAccidentList.Tables[0].Rows[0]["job_type"].ToString(),
                            ppe = dsAccidentList.Tables[0].Rows[0]["ppe"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString() != ""
                              && DateTime.TryParse(dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString(), out dtDocDate))
                        {
                            objAccidentReportModels.acc_date = dtDocDate;
                        }
                        if (dsAccidentList.Tables[0].Rows[0]["reported_date"].ToString() != ""
                          && DateTime.TryParse(dsAccidentList.Tables[0].Rows[0]["reported_date"].ToString(), out dtDocDate))
                        {
                            objAccidentReportModels.reported_date = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("AccidentList");
                    }

                    AccidentReportModelsList objAccTypeList = new AccidentReportModelsList();
                    objAccTypeList.AccidentList = new List<AccidentReportModels>();

                    sSqlstmt = "select id_accident_type,id_accident_rept,injury_type,no_person,pers_name from t_accident_type where id_accident_rept='" + id_accident_rept + "'";

                    DataSet dsAccTypeList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAccTypeList.Tables.Count > 0 && dsAccTypeList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsAccTypeList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                AccidentReportModels objAccInfoMdl = new AccidentReportModels
                                {
                                    id_accident_type = dsAccTypeList.Tables[0].Rows[i]["id_accident_type"].ToString(),
                                    id_accident_rept = dsAccTypeList.Tables[0].Rows[i]["id_accident_rept"].ToString(),
                                    injury_type = objGlobaldata.GetDropdownitemById(dsAccTypeList.Tables[0].Rows[i]["injury_type"].ToString()),
                                    no_person = dsAccTypeList.Tables[0].Rows[i]["no_person"].ToString(),
                                    pers_name = dsAccTypeList.Tables[0].Rows[i]["pers_name"].ToString(),
                                };
                                objAccTypeList.AccidentList.Add(objAccInfoMdl);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AccidentReportEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AccidentList");
                            }
                        }
                        ViewBag.objAccTypeList = objAccTypeList;
                    }

                    AccidentReportModelsList objReporterList = new AccidentReportModelsList();
                    objReporterList.AccidentList = new List<AccidentReportModels>();

                    sSqlstmt = "select id_accident_info,id_accident_rept,reported_to,reportedon_date,comments from t_accident_info where id_accident_rept='" + id_accident_rept + "'";

                    DataSet dsAttnList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAttnList.Tables.Count > 0 && dsAttnList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsAttnList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                AccidentReportModels objReptModel = new AccidentReportModels
                                {
                                    id_accident_info = dsAttnList.Tables[0].Rows[i]["id_accident_info"].ToString(),
                                    id_accident_rept = dsAttnList.Tables[0].Rows[i]["id_accident_rept"].ToString(),
                                    reported_to = objGlobaldata.GetMultiHrEmpNameById(dsAttnList.Tables[0].Rows[i]["reported_to"].ToString()),
                                    comments = dsAttnList.Tables[0].Rows[i]["comments"].ToString(),
                                };

                                DateTime dtDocDate1;
                                if (dsAttnList.Tables[0].Rows[i]["reportedon_date"].ToString() != ""
                         && DateTime.TryParse(dsAttnList.Tables[0].Rows[i]["reportedon_date"].ToString(), out dtDocDate1))
                                {
                                    objReptModel.reportedon_date = dtDocDate1;
                                }

                                objReporterList.AccidentList.Add(objReptModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AccidentReportEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AccidentList");
                            }
                        }
                        ViewBag.objReporterList = objReporterList;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccidentReportDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AccidentList");
            }
            return View(objAccidentReportModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AccidentReportDetails(FormCollection form)
        {
            ViewBag.SubMenutype = "AccidentReport";
            CompanyModels objCompany = new CompanyModels();
            AccidentReportModels objAccidentReportModels = new AccidentReportModels();
            IncidentReportModels objinc = new IncidentReportModels();
            try
            {
                if (form["id_accident_rept"] != null && form["id_accident_rept"] != "")
                {
                    string id_accident_rept = form["id_accident_rept"];
                    string sSqlstmt = "select id_accident_rept,report_no,acc_date,reported_date,reported_by,location,details,upload," +
                    "damage,invest_need,justify,invest_reportno,logged_by,Incident_Type,Actions_Taken,branch,Department,eqp_involved,job_type,ppe from t_accident_report where id_accident_rept ='" + id_accident_rept + "'";

                    DataSet dsAccidentList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAccidentList.Tables.Count > 0 && dsAccidentList.Tables[0].Rows.Count > 0)
                    {
                        objAccidentReportModels = new AccidentReportModels
                        {
                            id_accident_rept = dsAccidentList.Tables[0].Rows[0]["id_accident_rept"].ToString(),
                            report_no = dsAccidentList.Tables[0].Rows[0]["report_no"].ToString(),
                            reported_by = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[0]["reported_by"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsAccidentList.Tables[0].Rows[0]["location"].ToString()),
                            details = dsAccidentList.Tables[0].Rows[0]["details"].ToString(),
                            upload = dsAccidentList.Tables[0].Rows[0]["upload"].ToString(),
                            damage = dsAccidentList.Tables[0].Rows[0]["damage"].ToString(),
                            invest_need = dsAccidentList.Tables[0].Rows[0]["invest_need"].ToString(),
                            justify = dsAccidentList.Tables[0].Rows[0]["justify"].ToString(),
                            invest_reportno = dsAccidentList.Tables[0].Rows[0]["invest_reportno"].ToString(),
                            logged_by = dsAccidentList.Tables[0].Rows[0]["logged_by"].ToString(),
                            Incident_Type = objinc.GetIncidentTypeNameById(dsAccidentList.Tables[0].Rows[0]["Incident_Type"].ToString()),
                            Actions_Taken = dsAccidentList.Tables[0].Rows[0]["Actions_Taken"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAccidentList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsAccidentList.Tables[0].Rows[0]["Department"].ToString()),
                            eqp_involved = dsAccidentList.Tables[0].Rows[0]["eqp_involved"].ToString(),
                            job_type = dsAccidentList.Tables[0].Rows[0]["job_type"].ToString(),
                            ppe = dsAccidentList.Tables[0].Rows[0]["ppe"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString() != ""
                              && DateTime.TryParse(dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString(), out dtDocDate))
                        {
                            objAccidentReportModels.acc_date = dtDocDate;
                        }
                        if (dsAccidentList.Tables[0].Rows[0]["reported_date"].ToString() != ""
                          && DateTime.TryParse(dsAccidentList.Tables[0].Rows[0]["reported_date"].ToString(), out dtDocDate))
                        {
                            objAccidentReportModels.reported_date = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("AccidentList");
                    }
                    dsAccidentList = objCompany.GetCompanyDetailsForReport(dsAccidentList);
                    if (objinc.GetIncidentTypeNameById(dsAccidentList.Tables[0].Rows[0]["Incident_Type"].ToString()) == "Accident")
                    {
                        dsAccidentList = objGlobaldata.GetReportDetails(dsAccidentList, objAccidentReportModels.report_no, objAccidentReportModels.logged_by, "ACCIDENT REPORT");
                    }
                    else
                    {
                        dsAccidentList = objGlobaldata.GetReportDetails(dsAccidentList, objAccidentReportModels.report_no, objAccidentReportModels.logged_by, "INCIDENT REPORT");
                    }

                    ViewBag.CompanyInfo = dsAccidentList;

                    ViewBag.objAccidentList = objAccidentReportModels;

                    string sSqlstmt1 = "select id_accident_type,id_accident_rept,injury_type,no_person,pers_name from t_accident_type where id_accident_rept='" + id_accident_rept + "'";
                    ViewBag.objAccTypeList = objGlobaldata.Getdetails(sSqlstmt1);

                    string sSqlstmt2 = "select id_accident_info,id_accident_rept,reported_to,reportedon_date,comments from t_accident_info where id_accident_rept='" + id_accident_rept + "'";
                    ViewBag.objReporterList = objGlobaldata.Getdetails(sSqlstmt2);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccidentReportDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AccidentList");
            }

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("AccidentReportToPDF")
            {
                //FileName = "IncidentReport.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [AllowAnonymous]
        public JsonResult AccidentReportDelete(FormCollection form)
        {
            try
            {
                if (form["id_accident_rept"] != null && form["id_accident_rept"] != "")
                {
                    AccidentReportModels Doc = new AccidentReportModels();
                    string sid_accident_rept = form["id_accident_rept"];
                    if (Doc.FunDeleteAccidentDoc(sid_accident_rept))
                    {
                        TempData["Successdata"] = "Report deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in AccidentReportDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }
}