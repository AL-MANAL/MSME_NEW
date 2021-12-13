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
    public class OFIController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public OFIController()
        {
            ViewBag.Menutype = "Risk";
            ViewBag.SubMenutype = "OFI";
        }

        //checked
        [AllowAnonymous]
        public ActionResult AddReportOFI(string risk_id = "")
        {
            OFIModels objModel = new OFIModels();
            try
            {
                objModel.division = objGlobaldata.GetCurrentUserSession().division;
                objModel.department = objGlobaldata.GetCurrentUserSession().DeptID;
                objModel.location = objGlobaldata.GetCurrentUserSession().Work_Location;
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objModel.division);
                ViewBag.Location = objGlobaldata.GetLocationListBox();

                objModel.reportedby = objGlobaldata.GetCurrentUserSession().empid;
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                //ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(objModel.division, objModel.department, objModel.location);
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.RiskNo = objGlobaldata.GetRiskNoList();
                ViewBag.Identified = objModel.GetOFIIdentifiedList();
                if (risk_id != "")
                {
                    objModel.risk_no = risk_id;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddReportOFI: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //checked
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddReportOFI(OFIModels objModel, FormCollection form/*, IEnumerable<HttpPostedFileBase> nc_upload*/)
        {
            try
            {
                if (objModel != null)
                {
                    objModel.division = form["division"];
                    objModel.location = form["location"];
                    objModel.department = form["department"];
                    objModel.identified_in = form["identified_in"];
                    objModel.risk_no = form["risk_no"];
                    objModel.pers_resp = form["pers_resp"];
                    objModel.ofi_status = objModel.GetOFIStatusIdByName("Pending");
                    if (objGlobaldata.GetRiskRefByRiskId(form["risk_no"]) != null && objGlobaldata.GetRiskRefByRiskId(form["risk_no"]) != "")
                    {
                        objModel.ofi_no = "IO-" + objGlobaldata.GetRiskRefByRiskId(form["risk_no"]);
                    }
                    else
                    {
                        DataSet dsData = objGlobaldata.GetReportNo("OFI", "", objGlobaldata.GetBranchShortNameByID(objGlobaldata.GetCurrentUserSession().division));
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            objModel.ofi_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }
                    }

                    ////if (Request.Files.Count > 0)
                    //if (nc_upload != null)
                    //{
                    //    HttpPostedFileBase files = Request.Files[0];
                    //    if (nc_upload != null && files.ContentLength > 0)
                    //    {
                    //        objModel.nc_upload = "";
                    //        foreach (var file in nc_upload)
                    //        {
                    //            try
                    //            {
                    //                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(file.FileName));
                    //                string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                    //                file.SaveAs(sFilepath + "/" + sFilename);
                    //                objModel.nc_upload = objModel.nc_upload + "," + "~/DataUpload/MgmtDocs/NC/" + sFilename;
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                objGlobaldata.AddFunctionalLog("Exception in AddReportOFI-upload: " + ex.ToString());

                    //            }
                    //        }
                    //        objModel.nc_upload = objModel.nc_upload.Trim(',');
                    //    }
                    //    else
                    //    {
                    //        ViewBag.Message = "You have not specified a file.";
                    //    }
                    //}

                    //Reported By
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["empno " + i] != "" && form["empno " + i] != null)
                        {
                            objModel.reportedby = form["empno " + i] + "," + objModel.reportedby;
                        }
                    }
                    if (objModel.reportedby != null)
                    {
                        objModel.reportedby = objModel.reportedby.Trim(',');
                    }

                    //Approved by
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                    {
                        if (form["nempno " + i] != "" && form["nempno " + i] != null)
                        {
                            objModel.approvedby = form["nempno " + i] + "," + objModel.approvedby;
                        }
                    }
                    if (objModel.approvedby != null)
                    {
                        objModel.approvedby = objModel.approvedby.Trim(',');
                    }

                    if (objModel.FunAddReportOFI(objModel))
                    {
                        TempData["Successdata"] = "Added Improvement Opportunity(IO) details successfully with ofi_no '" + objModel.ofi_no + "'";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddReportOFI: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult FunGetRiskNoDesc()
        {
            string[] len = new string[100];
            DataSet dsData = new DataSet();
            try
            {
                string sSqlstmt = "select  risk_desc as Name, 'RR' as Name1 from risk_register where active = 1 and Risk_Type = 'Positive' " +
                    "union " +
                    "select  hazards as Name, 'HR' as Name1 from t_hazard where active = 1 " +
                    "union " +
                    "select activity as Name, 'ER' as Name1 from t_environment_risk where active = 1";
                dsData = objGlobaldata.Getdetails(sSqlstmt);

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        len[i] = dsData.Tables[0].Rows[i]["Name"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetRiskNoDesc: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(len);
        }

        //checked
        [AllowAnonymous]
        public ActionResult OFIList(string branch_name)
        {
            OFIModelsList objList = new OFIModelsList();
            objList.OFIList = new List<OFIModels>();

            OFIModels objModel = new OFIModels();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);

                //string sSqlstmt = "select id_ofi, ofi_no, risk_no, reportedby, reported_date, division, department, location, identified_in,opportunity, improvement,"
                //    + "target_date, approvedby,loggedby,ofi_status,(case when approved_status = '0' then 'Not approved'  when approved_status = '1' then 'Approved' when approved_status = '2' then 'Rejected' end) as approved_status" +
                //    ",(case when realization_approved_status = '0' then 'Not approved'  when realization_approved_status = '1' then 'Approved' when realization_approved_status = '2' then 'Rejected' end) as realization_approved_status,checkedby,checkedbystatus from t_ofi where active =1";
                string sSqlstmt = "select id_ofi, ofi_no, risk_no, reportedby, reported_date, division, department, location, identified_in,opportunity, improvement,"
                    + "target_date, approvedby,loggedby,ofi_status,approved_status,realization_approved_status,checkedby,checkedbystatus from t_ofi where active =1";

                string sSearchtext = "";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', division)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_ofi desc";

                DataSet dsOFIModels = objGlobaldata.Getdetails(sSqlstmt);

                if (dsOFIModels.Tables.Count > 0 && dsOFIModels.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsOFIModels.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            OFIModels objNCModels = new OFIModels
                            {
                                id_ofi = (dsOFIModels.Tables[0].Rows[i]["id_ofi"].ToString()),
                                ofi_no = (dsOFIModels.Tables[0].Rows[i]["ofi_no"].ToString()),
                                risk_no = objGlobaldata.GetRiskRefByRiskId(dsOFIModels.Tables[0].Rows[i]["risk_no"].ToString()),
                                risk_nodesc = objGlobaldata.GetRiskDescriptionByRiskId(dsOFIModels.Tables[0].Rows[i]["risk_no"].ToString()),
                                approved_status = (dsOFIModels.Tables[0].Rows[i]["approved_status"].ToString()),
                                realization_approved_status = (dsOFIModels.Tables[0].Rows[i]["realization_approved_status"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsOFIModels.Tables[0].Rows[i]["location"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsOFIModels.Tables[0].Rows[i]["department"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsOFIModels.Tables[0].Rows[i]["division"].ToString()),
                                checkedby = dsOFIModels.Tables[0].Rows[i]["checkedby"].ToString(),
                                ofi_status = objModel.GetOFIStatusById(dsOFIModels.Tables[0].Rows[i]["ofi_status"].ToString()),
                                checkedbystatus = (dsOFIModels.Tables[0].Rows[i]["checkedbystatus"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[i]["reported_date"].ToString(), out dtValue))
                            {
                                objNCModels.reported_date = dtValue;
                            }
                            objList.OFIList.Add(objNCModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in OFIList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in OFIList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objList.OFIList.ToList());
        }

        //checked
        [AllowAnonymous]
        public ActionResult UpdateOFI()
        {
            OFIModels objModel = new OFIModels();
            try
            {
                if (Request.QueryString["id_ofi"] != null && Request.QueryString["id_ofi"] != "")
                {
                    string sid_ofi = Request.QueryString["id_ofi"];
                    //string sSqlstmt = "select id_ofi, ofi_no, risk_no, reportedby, reported_date, division, department, location, identified_in,opportunity, improvement,"
                    //    + "target_date, approvedby,loggedby,ofi_status,(case when approved_status = '0' then 'Not approved'  when approved_status = '1' then 'Approved' when approved_status = '2' then 'Rejected' end) as approved_status" +
                    //    ",(case when realization_approved_status = '0' then 'Not approved'  when realization_approved_status = '1' then 'Approved' when realization_approved_status = '2' then 'Rejected' end) as realization_approved_status,checkedby from t_ofi where id_ofi = '"+ sid_ofi + "'";
                    string sSqlstmt = "select id_ofi, ofi_no, risk_no, reportedby, reported_date, division, department, location, identified_in,opportunity, improvement,"
                        + "target_date, approvedby,loggedby,checkedby,pers_resp from t_ofi where id_ofi = '" + sid_ofi + "'";

                    DataSet dsOFIModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsOFIModels.Tables.Count > 0 && dsOFIModels.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objModel = new OFIModels
                            {
                                id_ofi = (dsOFIModels.Tables[0].Rows[0]["id_ofi"].ToString()),
                                ofi_no = (dsOFIModels.Tables[0].Rows[0]["ofi_no"].ToString()),
                                risk_no = objGlobaldata.GetRiskRefByRiskId(dsOFIModels.Tables[0].Rows[0]["risk_no"].ToString()),
                                risk_nodesc = objGlobaldata.GetRiskDescriptionByRiskId(dsOFIModels.Tables[0].Rows[0]["risk_no"].ToString()),
                                //approved_status = (dsOFIModels.Tables[0].Rows[0]["approved_status"].ToString()),
                                //realization_approved_status = (dsOFIModels.Tables[0].Rows[0]["realization_approved_status"].ToString()),
                                location = /*objGlobaldata.GetDivisionLocationById*/(dsOFIModels.Tables[0].Rows[0]["location"].ToString()),
                                department = /*objGlobaldata.GetMultiDeptNameById*/(dsOFIModels.Tables[0].Rows[0]["department"].ToString()),
                                division = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsOFIModels.Tables[0].Rows[0]["division"].ToString()),
                                checkedby = dsOFIModels.Tables[0].Rows[0]["checkedby"].ToString(),
                                //ofi_status = (dsOFIModels.Tables[0].Rows[0]["ofi_status"].ToString()),
                                identified_in = dsOFIModels.Tables[0].Rows[0]["identified_in"].ToString(),
                                opportunity = dsOFIModels.Tables[0].Rows[0]["opportunity"].ToString(),
                                improvement = dsOFIModels.Tables[0].Rows[0]["improvement"].ToString(),
                                approvedby = dsOFIModels.Tables[0].Rows[0]["approvedby"].ToString(),
                                loggedby = dsOFIModels.Tables[0].Rows[0]["loggedby"].ToString(),
                            };
                            ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                            ViewBag.Department = objGlobaldata.GetDepartmentList1(dsOFIModels.Tables[0].Rows[0]["division"].ToString());
                            ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsOFIModels.Tables[0].Rows[0]["division"].ToString());
                            // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsOFIModels.Tables[0].Rows[0]["division"].ToString(), dsOFIModels.Tables[0].Rows[0]["department"].ToString(), dsOFIModels.Tables[0].Rows[0]["location"].ToString());

                            DateTime dtValue;
                            if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                            {
                                objModel.reported_date = dtValue;
                            }
                            if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                            {
                                objModel.target_date = dtValue;
                            }

                            if (dsOFIModels.Tables[0].Rows[0]["reportedby"].ToString() != "")
                            {
                                ViewBag.ReportedByArray = (dsOFIModels.Tables[0].Rows[0]["reportedby"].ToString()).Split(',');
                            }
                            if (dsOFIModels.Tables[0].Rows[0]["approvedby"].ToString() != "")
                            {
                                ViewBag.ApprovedtoArray = (dsOFIModels.Tables[0].Rows[0]["approvedby"].ToString()).Split(',');
                            }

                            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                            ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                            ViewBag.RiskNo = objGlobaldata.GetRiskNoList();
                            ViewBag.Identified = objModel.GetOFIIdentifiedList();
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in OFIList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UpdateOFI: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //checked
        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateOFI(OFIModels objModel, FormCollection form)
        {
            try
            {
                if (objModel != null)
                {
                    objModel.division = form["division"];
                    objModel.location = form["location"];
                    objModel.department = form["department"];
                    objModel.identified_in = form["identified_in"];

                    //Reported By
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["empno " + i] != "" && form["empno " + i] != null)
                        {
                            objModel.reportedby = form["empno " + i] + "," + objModel.reportedby;
                        }
                    }
                    if (objModel.reportedby != null)
                    {
                        objModel.reportedby = objModel.reportedby.Trim(',');
                    }

                    //Approved by
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                    {
                        if (form["nempno " + i] != "" && form["nempno " + i] != null)
                        {
                            objModel.approvedby = form["nempno " + i] + "," + objModel.approvedby;
                        }
                    }
                    if (objModel.approvedby != null)
                    {
                        objModel.approvedby = objModel.approvedby.Trim(',');
                    }

                    if (objModel.FunUpdateReportOFI(objModel))
                    {
                        TempData["Successdata"] = "Updated Improvement Opportunity(IO) details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UpdateOFI: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        //Realization OFI
        public ActionResult AddRealizationOFI()
        {
            OFIModels objModel = new OFIModels();
            try
            {
                if (Request.QueryString["id_ofi"] != null && Request.QueryString["id_ofi"] != "")
                {
                    string sid_ofi = Request.QueryString["id_ofi"];
                    ViewBag.id_ofi = sid_ofi;

                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.Approver = objGlobaldata.GetApprover();
                    ViewBag.AreaImproved = objModel.GetOFIAreaImprovedList();

                    string sSqlstmt = "select id_ofi, ofi_no, risk_no, reportedby, reported_date, division, department, location, identified_in,opportunity, improvement,"
                        + "target_date, approvedby,loggedby,checkedby,action_proposedby,realization_approved_by,realization_approved_status from t_ofi where id_ofi='" + sid_ofi + "'";
                    DataSet dsOFIModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsOFIModels.Tables.Count > 0 && dsOFIModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new OFIModels
                        {
                            id_ofi = (dsOFIModels.Tables[0].Rows[0]["id_ofi"].ToString()),
                            ofi_no = (dsOFIModels.Tables[0].Rows[0]["ofi_no"].ToString()),
                            risk_no = objGlobaldata.GetRiskRefByRiskId(dsOFIModels.Tables[0].Rows[0]["risk_no"].ToString()),
                            risk_nodesc = objGlobaldata.GetRiskDescriptionByRiskId(dsOFIModels.Tables[0].Rows[0]["risk_no"].ToString()),
                            division = objGlobaldata.GetCompanyBranchNameById(dsOFIModels.Tables[0].Rows[0]["division"].ToString()),
                            department = objGlobaldata.GetMultiDeptNameById(dsOFIModels.Tables[0].Rows[0]["department"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsOFIModels.Tables[0].Rows[0]["location"].ToString()),
                            checkedby = dsOFIModels.Tables[0].Rows[0]["checkedby"].ToString(),
                            //ofi_status = (dsOFIModels.Tables[0].Rows[0]["ofi_status"].ToString()),
                            identified_in = objModel.GetOFIIdentifiedById(dsOFIModels.Tables[0].Rows[0]["identified_in"].ToString()),
                            opportunity = dsOFIModels.Tables[0].Rows[0]["opportunity"].ToString(),
                            improvement = dsOFIModels.Tables[0].Rows[0]["improvement"].ToString(),
                            approvedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["approvedby"].ToString()),
                            reportedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["reportedby"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                        {
                            objModel.reported_date = dtValue;
                        }
                        if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                        {
                            objModel.target_date = dtValue;
                        }

                        if (dsOFIModels.Tables[0].Rows[0]["action_proposedby"].ToString() != "")
                        {
                            ViewBag.ProposedByArray = (dsOFIModels.Tables[0].Rows[0]["action_proposedby"].ToString()).Split(',');
                        }
                        if (dsOFIModels.Tables[0].Rows[0]["realization_approved_by"].ToString() != "")
                        {
                            ViewBag.ApprovedtoArray = (dsOFIModels.Tables[0].Rows[0]["realization_approved_by"].ToString()).Split(',');
                        }

                        // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsOFIModels.Tables[0].Rows[0]["division"].ToString(), dsOFIModels.Tables[0].Rows[0]["department"].ToString(), dsOFIModels.Tables[0].Rows[0]["location"].ToString());
                        // ViewBag.Approver = objGlobaldata.GetGRoleList(dsOFIModels.Tables[0].Rows[0]["division"].ToString(), dsOFIModels.Tables[0].Rows[0]["department"].ToString(), dsOFIModels.Tables[0].Rows[0]["location"].ToString(), "Approver");

                        OFIModelsList ObjOFIList = new OFIModelsList();
                        ObjOFIList.OFIList = new List<OFIModels>();

                        string Ssql = "select id_ofi_action,id_ofi,action_details,area_improved,resp,resource,action_target_date from t_ofi_action where id_ofi = '" + sid_ofi + "'";

                        DataSet dsCALsit = objGlobaldata.Getdetails(Ssql);

                        if (dsCALsit.Tables.Count > 0 && dsCALsit.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsCALsit.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    OFIModels objNCModels = new OFIModels
                                    {
                                        id_ofi_action = (dsCALsit.Tables[0].Rows[i]["id_ofi_action"].ToString()),
                                        id_ofi = (dsCALsit.Tables[0].Rows[i]["id_ofi"].ToString()),
                                        action_details = (dsCALsit.Tables[0].Rows[i]["action_details"].ToString()),
                                        area_improved = (dsCALsit.Tables[0].Rows[i]["area_improved"].ToString()),
                                        resp = (dsCALsit.Tables[0].Rows[i]["resp"].ToString()),
                                        resource = (dsCALsit.Tables[0].Rows[i]["resource"].ToString()),
                                    };
                                    if (DateTime.TryParse(dsCALsit.Tables[0].Rows[i]["action_target_date"].ToString(), out dtValue))
                                    {
                                        objNCModels.action_target_date = dtValue;
                                    }
                                    ObjOFIList.OFIList.Add(objNCModels);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddRealizationOFI: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }
                        ViewBag.ObjOFIList = ObjOFIList;

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("OFIList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRealizationOFI: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("OFIList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddRealizationOFI(OFIModels objModel, FormCollection form)
        {
            try
            {
                OFIModelsList objModelList = new OFIModelsList();
                objModelList.OFIList = new List<OFIModels>();

                //action_proposedby
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objModel.action_proposedby = objModel.action_proposedby + "," + form["empno " + i];
                    }
                }
                if (objModel.action_proposedby != null)
                {
                    objModel.action_proposedby = objModel.action_proposedby.Trim(',');
                }

                //Notifed To
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.realization_approved_by = objModel.realization_approved_by + "," + form["nempno " + i];
                    }
                }
                if (objModel.realization_approved_by != null)
                {
                    objModel.realization_approved_by = objModel.realization_approved_by.Trim(',');
                }

                DateTime dateValue;
                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    OFIModels objNCModel = new OFIModels();
                    if (form["action_details " + i] != "" && form["action_details " + i] != null)
                    {
                        objNCModel.action_details = form["action_details " + i];
                        objNCModel.area_improved = form["area_improved " + i];
                        objNCModel.resp = form["resp " + i];
                        objNCModel.resource = form["resource " + i];
                        if (DateTime.TryParse(form["action_target_date " + i], out dateValue) == true)
                        {
                            objNCModel.action_target_date = dateValue;
                        }
                        objModelList.OFIList.Add(objNCModel);
                    }
                }

                if (objModel.FunAddRealizationOFI(objModel, objModelList))
                {
                    TempData["Successdata"] = "Added Corrective Action details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRealizationOFI: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        //Improvement Achieved
        public ActionResult AddImprovementOFI()
        {
            OFIModels objModel = new OFIModels();
            try
            {
                if (Request.QueryString["id_ofi"] != null && Request.QueryString["id_ofi"] != "")
                {
                    string sid_ofi = Request.QueryString["id_ofi"];
                    ViewBag.id_ofi = sid_ofi;

                    ViewBag.ImprovementAchieved = objModel.GetOFIExpectedImporvementList();
                    ViewBag.ImproveAchievedIn = objModel.GetOFIAreaImprovedList();
                    ViewBag.DocTypeList = objGlobaldata.GetDropdownList("DocType");
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                    //ViewBag.AreaImproved = objModel.GetOFIAreaImprovedList();

                    string sSqlstmt = "select id_ofi, ofi_no, risk_no, reportedby, reported_date, division, department, location, identified_in,opportunity, improvement,"
                        + "target_date, approvedby,loggedby,checkedby,action_proposedby,realization_approved_by,realization_approved_status" +
                        ",updated,updatedby,improvement_achieve,checkedby from t_ofi where id_ofi='" + sid_ofi + "'";
                    DataSet dsOFIModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsOFIModels.Tables.Count > 0 && dsOFIModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new OFIModels
                        {
                            id_ofi = (dsOFIModels.Tables[0].Rows[0]["id_ofi"].ToString()),
                            ofi_no = (dsOFIModels.Tables[0].Rows[0]["ofi_no"].ToString()),
                            risk_no = objGlobaldata.GetRiskRefByRiskId(dsOFIModels.Tables[0].Rows[0]["risk_no"].ToString()),
                            risk_nodesc = objGlobaldata.GetRiskDescriptionByRiskId(dsOFIModels.Tables[0].Rows[0]["risk_no"].ToString()),
                            division = objGlobaldata.GetCompanyBranchNameById(dsOFIModels.Tables[0].Rows[0]["division"].ToString()),
                            department = objGlobaldata.GetMultiDeptNameById(dsOFIModels.Tables[0].Rows[0]["department"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsOFIModels.Tables[0].Rows[0]["location"].ToString()),
                            identified_in = objModel.GetOFIIdentifiedById(dsOFIModels.Tables[0].Rows[0]["identified_in"].ToString()),
                            opportunity = dsOFIModels.Tables[0].Rows[0]["opportunity"].ToString(),
                            improvement = dsOFIModels.Tables[0].Rows[0]["improvement"].ToString(),
                            approvedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["approvedby"].ToString()),
                            reportedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["reportedby"].ToString()),

                            updated = dsOFIModels.Tables[0].Rows[0]["updated"].ToString(),
                            updatedby = (dsOFIModels.Tables[0].Rows[0]["updatedby"].ToString()),
                            improvement_achieve = dsOFIModels.Tables[0].Rows[0]["improvement_achieve"].ToString(),
                            checkedby = (dsOFIModels.Tables[0].Rows[0]["checkedby"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                        {
                            objModel.reported_date = dtValue;
                        }
                        if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                        {
                            objModel.target_date = dtValue;
                        }

                        if (dsOFIModels.Tables[0].Rows[0]["updatedby"].ToString() != "")
                        {
                            ViewBag.ProposedByArray = (dsOFIModels.Tables[0].Rows[0]["updatedby"].ToString()).Split(',');
                        }
                        if (dsOFIModels.Tables[0].Rows[0]["checkedby"].ToString() != "")
                        {
                            ViewBag.ApprovedtoArray = (dsOFIModels.Tables[0].Rows[0]["checkedby"].ToString()).Split(',');
                        }
                        // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsOFIModels.Tables[0].Rows[0]["division"].ToString(), dsOFIModels.Tables[0].Rows[0]["department"].ToString(), dsOFIModels.Tables[0].Rows[0]["location"].ToString());

                        OFIModelsList ObjOFIList = new OFIModelsList();
                        ObjOFIList.OFIList = new List<OFIModels>();

                        string Ssql = "select id_ofi_improvement,id_ofi,improve_details,improve_achievedin,improve_measurable,imporve_upload from t_ofi_improvement where id_ofi = '" + sid_ofi + "'";
                        DataSet dsCALsit = objGlobaldata.Getdetails(Ssql);

                        if (dsCALsit.Tables.Count > 0 && dsCALsit.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsCALsit.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    OFIModels objNCModels = new OFIModels
                                    {
                                        id_ofi_improvement = (dsCALsit.Tables[0].Rows[i]["id_ofi_improvement"].ToString()),
                                        id_ofi = (dsCALsit.Tables[0].Rows[i]["id_ofi"].ToString()),
                                        improve_details = (dsCALsit.Tables[0].Rows[i]["improve_details"].ToString()),
                                        improve_achievedin = (dsCALsit.Tables[0].Rows[i]["improve_achievedin"].ToString()),
                                        improve_measurable = (dsCALsit.Tables[0].Rows[i]["improve_measurable"].ToString()),
                                        imporve_upload = (dsCALsit.Tables[0].Rows[i]["imporve_upload"].ToString()),
                                    };
                                    ObjOFIList.OFIList.Add(objNCModels);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddImprovementOFI: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }
                        ViewBag.ObjOFIList = ObjOFIList;

                        OFIModelsList ObjOFIDocList = new OFIModelsList();
                        ObjOFIDocList.OFIList = new List<OFIModels>();

                        string Ssql1 = "select id_ofi_doc,id_ofi,doc_type,doc_ref,doc_name from t_ofi_doc where id_ofi = '" + sid_ofi + "'";
                        DataSet dsOFIList = objGlobaldata.Getdetails(Ssql1);

                        if (dsOFIList.Tables.Count > 0 && dsOFIList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsOFIList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    OFIModels objdoccModel = new OFIModels
                                    {
                                        id_ofi_doc = (dsOFIList.Tables[0].Rows[i]["id_ofi_doc"].ToString()),
                                        id_ofi = (dsOFIList.Tables[0].Rows[i]["id_ofi"].ToString()),
                                        doc_type = (dsOFIList.Tables[0].Rows[i]["doc_type"].ToString()),
                                        doc_ref = (dsOFIList.Tables[0].Rows[i]["doc_ref"].ToString()),
                                        doc_name = (dsOFIList.Tables[0].Rows[i]["doc_name"].ToString()),
                                    };
                                    ObjOFIDocList.OFIList.Add(objdoccModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddImprovementOFI: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }
                        ViewBag.ObjOFIDocList = ObjOFIDocList;

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("OFIList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddImprovementOFI: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("OFIList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddImprovementOFI(OFIModels objModel, FormCollection form)
        {
            try
            {
                //updatedby
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objModel.updatedby = objModel.updatedby + "," + form["empno " + i];
                    }
                }
                if (objModel.updatedby != null)
                {
                    objModel.updatedby = objModel.updatedby.Trim(',');
                }

                //checkedby
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.checkedby = objModel.checkedby + "," + form["nempno " + i];
                    }
                }
                if (objModel.checkedby != null)
                {
                    objModel.checkedby = objModel.checkedby.Trim(',');
                }

                OFIModelsList objModelList = new OFIModelsList();
                objModelList.OFIList = new List<OFIModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    OFIModels objNCModel = new OFIModels();
                    if (form["improve_details " + i] != "" && form["improve_details " + i] != null)
                    {
                        objNCModel.improve_details = form["improve_details " + i];
                        objNCModel.improve_achievedin = form["improve_achievedin " + i];
                        objNCModel.improve_measurable = form["improve_measurable " + i];
                        objNCModel.imporve_upload = form["imporve_upload1 " + i];

                        objModelList.OFIList.Add(objNCModel);
                    }
                }

                OFIModelsList objOFIList = new OFIModelsList();
                objOFIList.OFIList = new List<OFIModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcount1"]); i++)
                {
                    OFIModels objOFIModel = new OFIModels();
                    if (form["doc_type " + i] != "" && form["doc_type " + i] != null)
                    {
                        objOFIModel.doc_type = form["doc_type " + i];
                        objOFIModel.doc_ref = form["doc_ref " + i];
                        objOFIModel.doc_name = form["doc_name " + i];

                        objOFIList.OFIList.Add(objOFIModel);
                    }
                }
                if (objModel.FunAddImprovementOFI(objModel, objModelList, objOFIList))
                {
                    TempData["Successdata"] = "Added Improvement Achieved details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddImprovementOFI: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [AllowAnonymous]
        public JsonResult OFIDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_ofi"] != null && form["id_ofi"] != "")
                {
                    OFIModels Doc = new OFIModels();
                    string sid_nc = form["id_ofi"];

                    if (Doc.FunDeleteOFIDoc(sid_nc))
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
                objGlobaldata.AddFunctionalLog("Exception in OFIDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public JsonResult FunGetRiskDetails(string srisk_id)
        {
            try
            {
                string srisk_desc = "";
                string sSsqlstmt = "";
                if (srisk_id != "")
                {
                    string[] id = srisk_id.Split(':');
                    if (id[1] == "RR")
                    {
                        sSsqlstmt = "select risk_desc as Name from risk_register where risk_id='" + id[0] + "'";
                    }
                    if (id[1] == "HR")
                    {
                        sSsqlstmt = "select hazards as Name from t_hazard where id_hazard='" + id[0] + "'";
                    }
                    if (id[1] == "ER")
                    {
                        sSsqlstmt = "select aspects as Name from t_environment_risk where id_env_risk='" + id[0] + "'";
                    }
                    DataSet dsList = objGlobaldata.Getdetails(sSsqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        srisk_desc = dsList.Tables[0].Rows[0]["Name"].ToString();
                    }
                    return Json(srisk_desc);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetRiskDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }

        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase upload = Request.Files;
            DocumentTrackingModels obj = new DocumentTrackingModels();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/RiskMgmt"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + sFilename);
                //return Json("~/DataUpload/MgmtDocs/Surveillance/" + "Surveillance" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                obj.upload = obj.upload + "," + "~/DataUpload/MgmtDocs/RiskMgmt/" + sFilename;
            }
            obj.upload = obj.upload.Trim(',');
            return Json(obj.upload);
        }

        [AllowAnonymous]
        public ActionResult OFIDetails()
        {
            OFIModels objModel = new OFIModels();
            try
            {
                ViewBag.OFIStatus = objModel.GetOFIStatusList();
                ViewBag.ApproveStatus = objGlobaldata.GetConstantValueKeyValuePair("OFI");
                if (Request.QueryString["id_ofi"] != null && Request.QueryString["id_ofi"] != "")
                {
                    string sid_ofi = Request.QueryString["id_ofi"];
                    string sSqlstmt = "select id_ofi, ofi_no, risk_no, reportedby, reported_date, division, department, location, identified_in,opportunity, improvement,"
                        + "target_date, approvedby,approved_date,loggedby,ofi_status,approved_status,realization_approved_status,checkedby,checkedbystatus,realization_approved_by,updatedby,updated,improvement_achieve,action_proposedby,remark,pers_resp,approver_comments,approver_upload,realization_approved_status,realization_apporved_date from t_ofi where id_ofi = '" + sid_ofi + "'";

                    DataSet dsOFIModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsOFIModels.Tables.Count > 0 && dsOFIModels.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objModel = new OFIModels
                            {
                                id_ofi = (dsOFIModels.Tables[0].Rows[0]["id_ofi"].ToString()),
                                ofi_no = (dsOFIModels.Tables[0].Rows[0]["ofi_no"].ToString()),
                                risk_no = objGlobaldata.GetRiskRefByRiskId(dsOFIModels.Tables[0].Rows[0]["risk_no"].ToString()),
                                risk_nodesc = objGlobaldata.GetRiskDescriptionByRiskId(dsOFIModels.Tables[0].Rows[0]["risk_no"].ToString()),
                                //approved_status = (dsOFIModels.Tables[0].Rows[0]["approved_status"].ToString()),
                                //realization_approved_status = (dsOFIModels.Tables[0].Rows[0]["realization_approved_status"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsOFIModels.Tables[0].Rows[0]["location"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsOFIModels.Tables[0].Rows[0]["department"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsOFIModels.Tables[0].Rows[0]["division"].ToString()),
                                ofi_status = objModel.GetOFIStatusById(dsOFIModels.Tables[0].Rows[0]["ofi_status"].ToString()),
                                ofi_statusId = (dsOFIModels.Tables[0].Rows[0]["ofi_status"].ToString()),
                                identified_in = objModel.GetOFIIdentifiedById(dsOFIModels.Tables[0].Rows[0]["identified_in"].ToString()),
                                opportunity = dsOFIModels.Tables[0].Rows[0]["opportunity"].ToString(),
                                improvement = dsOFIModels.Tables[0].Rows[0]["improvement"].ToString(),
                                reportedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["reportedby"].ToString()),
                                approvedby = (dsOFIModels.Tables[0].Rows[0]["approvedby"].ToString()),
                                checkedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["checkedby"].ToString()),
                                checkedbyId = (dsOFIModels.Tables[0].Rows[0]["checkedby"].ToString()),
                                realization_approved_by = (dsOFIModels.Tables[0].Rows[0]["realization_approved_by"].ToString()),
                                updatedby = (dsOFIModels.Tables[0].Rows[0]["updatedby"].ToString()),
                                action_proposedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["action_proposedby"].ToString()),
                                updated = dsOFIModels.Tables[0].Rows[0]["updated"].ToString(),
                                improvement_achieve = objModel.GetOFIExpectedImporvementById(dsOFIModels.Tables[0].Rows[0]["improvement_achieve"].ToString()),
                                checkedbystatus = dsOFIModels.Tables[0].Rows[0]["checkedbystatus"].ToString(),
                                remark = dsOFIModels.Tables[0].Rows[0]["remark"].ToString(),
                                approved_status = dsOFIModels.Tables[0].Rows[0]["approved_status"].ToString(),
                                pers_resp = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["pers_resp"].ToString()),
                                approver_comments = dsOFIModels.Tables[0].Rows[0]["approver_comments"].ToString(),
                                approver_upload = dsOFIModels.Tables[0].Rows[0]["approver_upload"].ToString(),
                                realization_approved_status = dsOFIModels.Tables[0].Rows[0]["realization_approved_status"].ToString(),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                            {
                                objModel.reported_date = dtValue;
                            }
                            if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                            {
                                objModel.target_date = dtValue;
                            }
                            if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["approved_date"].ToString(), out dtValue))
                            {
                                objModel.approved_date = dtValue;
                            }
                            if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["realization_apporved_date"].ToString(), out dtValue))
                            {
                                objModel.realization_apporved_date = dtValue;
                            }
                            //----------------- OFI Action Starts----------------
                            OFIModelsList ObjOFIList = new OFIModelsList();
                            ObjOFIList.OFIList = new List<OFIModels>();

                            string Ssql = "select id_ofi_action,id_ofi,action_details,area_improved,resp,resource,action_target_date from t_ofi_action where id_ofi = '" + sid_ofi + "'";
                            DataSet dsCALsit = objGlobaldata.Getdetails(Ssql);

                            if (dsCALsit.Tables.Count > 0 && dsCALsit.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsCALsit.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        OFIModels objNCModels = new OFIModels
                                        {
                                            id_ofi_action = (dsCALsit.Tables[0].Rows[i]["id_ofi_action"].ToString()),
                                            id_ofi = (dsCALsit.Tables[0].Rows[i]["id_ofi"].ToString()),
                                            action_details = (dsCALsit.Tables[0].Rows[i]["action_details"].ToString()),
                                            area_improved = (dsCALsit.Tables[0].Rows[i]["area_improved"].ToString()),
                                            resp = (dsCALsit.Tables[0].Rows[i]["resp"].ToString()),
                                            resource = (dsCALsit.Tables[0].Rows[i]["resource"].ToString()),
                                        };
                                        if (DateTime.TryParse(dsCALsit.Tables[0].Rows[i]["action_target_date"].ToString(), out dtValue))
                                        {
                                            objNCModels.action_target_date = dtValue;
                                        }
                                        ObjOFIList.OFIList.Add(objNCModels);
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in OFIDetails: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }
                            ViewBag.ObjOFIActionList = ObjOFIList;
                            //------------------OFI Action Ends---------------------

                            //-----------------OFI Improvement starts-----------------
                            OFIModelsList ObjofiList = new OFIModelsList();
                            ObjofiList.OFIList = new List<OFIModels>();

                            string Ssql1 = "select id_ofi_improvement,id_ofi,improve_details,improve_achievedin,improve_measurable,imporve_upload from t_ofi_improvement where id_ofi = '" + sid_ofi + "'";
                            DataSet dsImpLsit = objGlobaldata.Getdetails(Ssql1);

                            if (dsImpLsit.Tables.Count > 0 && dsImpLsit.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsImpLsit.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        OFIModels objImpModels = new OFIModels
                                        {
                                            id_ofi_improvement = (dsImpLsit.Tables[0].Rows[i]["id_ofi_improvement"].ToString()),
                                            id_ofi = (dsImpLsit.Tables[0].Rows[i]["id_ofi"].ToString()),
                                            improve_details = (dsImpLsit.Tables[0].Rows[i]["improve_details"].ToString()),
                                            improve_achievedin = (dsImpLsit.Tables[0].Rows[i]["improve_achievedin"].ToString()),
                                            improve_measurable = (dsImpLsit.Tables[0].Rows[i]["improve_measurable"].ToString()),
                                            imporve_upload = (dsImpLsit.Tables[0].Rows[i]["imporve_upload"].ToString()),
                                        };
                                        ObjofiList.OFIList.Add(objImpModels);
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in OFIDetails: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }
                            ViewBag.ObjOFIImpList = ObjofiList;
                            //-------------------OFI Improvement Ends---------------

                            //-------------------OFI Document Starts---------------
                            OFIModelsList ObjOFIDocList = new OFIModelsList();
                            ObjOFIDocList.OFIList = new List<OFIModels>();

                            string Ssql11 = "select id_ofi_doc,id_ofi,doc_type,doc_ref,doc_name from t_ofi_doc where id_ofi = '" + sid_ofi + "'";
                            DataSet dsOFIList = objGlobaldata.Getdetails(Ssql11);

                            if (dsOFIList.Tables.Count > 0 && dsOFIList.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsOFIList.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        OFIModels objDocModel = new OFIModels
                                        {
                                            id_ofi_doc = (dsOFIList.Tables[0].Rows[i]["id_ofi_doc"].ToString()),
                                            id_ofi = (dsOFIList.Tables[0].Rows[i]["id_ofi"].ToString()),
                                            doc_type = (dsOFIList.Tables[0].Rows[i]["doc_type"].ToString()),
                                            doc_ref = (dsOFIList.Tables[0].Rows[i]["doc_ref"].ToString()),
                                            doc_name = (dsOFIList.Tables[0].Rows[i]["doc_name"].ToString()),
                                        };
                                        ObjOFIDocList.OFIList.Add(objDocModel);
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in OFIDetails: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }
                            ViewBag.ObjOFIDocList = ObjOFIDocList;
                            //-------------------OFI Document Ends---------------
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in OFIList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in OFIDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult OFICheckedDetail(OFIModels objModel, FormCollection form)
        {
            try
            {
                if (objModel != null)
                {
                    objModel.remark = form["remarks"];

                    if (objModel.FunAddOFICheckedbystatus(objModel))
                    {
                        TempData["Successdata"] = "Added Final Status successfully with ofi_no '" + objModel.ofi_no + "'";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in OFICheckedDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("OFILIST");
        }

        //public JsonResult OFIInitialApproveNoty(string id_ofi, string sStatus, string PendingFlg)
        //{
        //    try
        //    {
        //        OFIModels objModel = new OFIModels();

        //        if (objModel.FunOFIInitialApproval(id_ofi, sStatus))
        //        {
        //            return Json("Success" + sStatus);
        //        }
        //        else
        //        {
        //            return Json("Failed");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in OFIInitialApproveNoty: " + ex.ToString());
        //    }

        //    if (PendingFlg != null && PendingFlg == "true")
        //    {
        //        return Json("Success");
        //    }
        //    else
        //    {
        //        return Json("Failed");
        //    }
        //}

        public ActionResult OFIInitialApprove(OFIModels objModel, FormCollection form, string id_ofi, string sStatus, string PendingFlg, IEnumerable<HttpPostedFileBase> approver_upload, IEnumerable<HttpPostedFileBase> realization_upload)
        {
            try
            {
                sStatus = form["approved_status"];
                HttpPostedFileBase files = Request.Files[0];
                if (approver_upload != null && files.ContentLength > 0)
                {
                    objModel.approver_upload = "";
                    foreach (var file in approver_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/RiskMgmt"), Path.GetFileName(file.FileName));
                            string sFilename = "OFI" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.approver_upload = objModel.approver_upload + "," + "~/DataUpload/MgmtDocs/RiskMgmt/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in OFIInitialApprove-upload: " + ex.ToString());
                        }
                    }
                    objModel.approver_upload = objModel.approver_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (realization_upload != null && files.ContentLength > 0)
                {
                    objModel.realization_upload = "";
                    foreach (var file in realization_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/RiskMgmt"), Path.GetFileName(file.FileName));
                            string sFilename = "OFI" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.realization_upload = objModel.realization_upload + "," + "~/DataUpload/MgmtDocs/RiskMgmt/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in OFIInitialApprove-upload: " + ex.ToString());
                        }
                    }
                    objModel.realization_upload = objModel.realization_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objModel.FunOFIInitialApproval(id_ofi, sStatus, objModel))
                {
                    TempData["Successdata"] = "Improvement Opportunity(IO) is " + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in OFIInitialApproveNoty: " + ex.ToString());
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult OFIPDF(FormCollection form)
        {
            OFIModels objModel = new OFIModels();
            try
            {
                ViewBag.OFIStatus = objModel.GetOFIStatusList();
                ViewBag.ApproveStatus = objGlobaldata.GetConstantValueKeyValuePair("OFI");
                if (form["id_ofi"] != null && form["id_ofi"] != "")
                {
                    string sid_ofi = form["id_ofi"];
                    string sSqlstmt = "select id_ofi, ofi_no, risk_no, reportedby, reported_date, division, department, location, identified_in,opportunity, improvement,"
                        + "target_date, approvedby,approved_date,loggedby,ofi_status,approved_status,realization_approved_status,checkedby,checkedbystatus,realization_approved_by," +
                        "updatedby,updated,improvement_achieve,action_proposedby,remark,pers_resp,approver_comments,approver_upload,realization_approved_status,realization_apporved_date,loggedby from t_ofi where id_ofi = '" + sid_ofi + "'";

                    DataSet dsOFIModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsOFIModels.Tables.Count > 0 && dsOFIModels.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objModel = new OFIModels
                            {
                                id_ofi = (dsOFIModels.Tables[0].Rows[0]["id_ofi"].ToString()),
                                ofi_no = (dsOFIModels.Tables[0].Rows[0]["ofi_no"].ToString()),
                                risk_no = objGlobaldata.GetRiskRefByRiskId(dsOFIModels.Tables[0].Rows[0]["risk_no"].ToString()),
                                risk_nodesc = objGlobaldata.GetRiskDescriptionByRiskId(dsOFIModels.Tables[0].Rows[0]["risk_no"].ToString()),
                                //approved_status = (dsOFIModels.Tables[0].Rows[0]["approved_status"].ToString()),
                                //realization_approved_status = (dsOFIModels.Tables[0].Rows[0]["realization_approved_status"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsOFIModels.Tables[0].Rows[0]["location"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsOFIModels.Tables[0].Rows[0]["department"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsOFIModels.Tables[0].Rows[0]["division"].ToString()),
                                ofi_status = objModel.GetOFIStatusById(dsOFIModels.Tables[0].Rows[0]["ofi_status"].ToString()),
                                ofi_statusId = (dsOFIModels.Tables[0].Rows[0]["ofi_status"].ToString()),
                                identified_in = objModel.GetOFIIdentifiedById(dsOFIModels.Tables[0].Rows[0]["identified_in"].ToString()),
                                opportunity = dsOFIModels.Tables[0].Rows[0]["opportunity"].ToString(),
                                improvement = dsOFIModels.Tables[0].Rows[0]["improvement"].ToString(),
                                reportedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["reportedby"].ToString()),
                                approvedby = (dsOFIModels.Tables[0].Rows[0]["approvedby"].ToString()),
                                checkedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["checkedby"].ToString()),
                                checkedbyId = (dsOFIModels.Tables[0].Rows[0]["checkedby"].ToString()),
                                realization_approved_by = (dsOFIModels.Tables[0].Rows[0]["realization_approved_by"].ToString()),
                                updatedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["updatedby"].ToString()),
                                action_proposedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["action_proposedby"].ToString()),
                                updated = dsOFIModels.Tables[0].Rows[0]["updated"].ToString(),
                                improvement_achieve = objModel.GetOFIExpectedImporvementById(dsOFIModels.Tables[0].Rows[0]["improvement_achieve"].ToString()),
                                checkedbystatus = dsOFIModels.Tables[0].Rows[0]["checkedbystatus"].ToString(),
                                remark = dsOFIModels.Tables[0].Rows[0]["remark"].ToString(),
                                approved_status = dsOFIModels.Tables[0].Rows[0]["approved_status"].ToString(),
                                pers_resp = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["pers_resp"].ToString()),
                                approver_comments = dsOFIModels.Tables[0].Rows[0]["approver_comments"].ToString(),
                                approver_upload = dsOFIModels.Tables[0].Rows[0]["approver_upload"].ToString(),
                                realization_approved_status = dsOFIModels.Tables[0].Rows[0]["realization_approved_status"].ToString(),
                                loggedby = objGlobaldata.GetMultiHrEmpNameById(dsOFIModels.Tables[0].Rows[0]["loggedby"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["reported_date"].ToString(), out dtValue))
                            {
                                objModel.reported_date = dtValue;
                            }
                            if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["target_date"].ToString(), out dtValue))
                            {
                                objModel.target_date = dtValue;
                            }
                            if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["approved_date"].ToString(), out dtValue))
                            {
                                objModel.approved_date = dtValue;
                            }
                            if (DateTime.TryParse(dsOFIModels.Tables[0].Rows[0]["realization_apporved_date"].ToString(), out dtValue))
                            {
                                objModel.realization_apporved_date = dtValue;
                            }
                            CompanyModels objCompany = new CompanyModels();

                            dsOFIModels = objCompany.GetCompanyDetailsForReport(dsOFIModels);
                            dsOFIModels = objGlobaldata.GetReportDetails(dsOFIModels, objModel.ofi_no, objGlobaldata.GetCurrentUserSession().empid, "IO REPORT");

                            ViewBag.CompanyInfo = dsOFIModels;
                            //----------------- OFI Action Starts----------------
                            OFIModelsList ObjOFIList = new OFIModelsList();
                            ObjOFIList.OFIList = new List<OFIModels>();

                            string Ssql = "select id_ofi_action,id_ofi,action_details,area_improved,resp,resource,action_target_date from t_ofi_action where id_ofi = '" + sid_ofi + "'";
                            DataSet dsCALsit = objGlobaldata.Getdetails(Ssql);

                            if (dsCALsit.Tables.Count > 0 && dsCALsit.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsCALsit.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        OFIModels objNCModels = new OFIModels
                                        {
                                            id_ofi_action = (dsCALsit.Tables[0].Rows[i]["id_ofi_action"].ToString()),
                                            id_ofi = (dsCALsit.Tables[0].Rows[i]["id_ofi"].ToString()),
                                            action_details = (dsCALsit.Tables[0].Rows[i]["action_details"].ToString()),
                                            area_improved = (dsCALsit.Tables[0].Rows[i]["area_improved"].ToString()),
                                            resp = (dsCALsit.Tables[0].Rows[i]["resp"].ToString()),
                                            resource = (dsCALsit.Tables[0].Rows[i]["resource"].ToString()),
                                        };
                                        if (DateTime.TryParse(dsCALsit.Tables[0].Rows[i]["action_target_date"].ToString(), out dtValue))
                                        {
                                            objNCModels.action_target_date = dtValue;
                                        }
                                        ObjOFIList.OFIList.Add(objNCModels);
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in OFIDetails: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }
                            ViewBag.ObjOFIActionList = ObjOFIList;
                            //------------------OFI Action Ends---------------------

                            //-----------------OFI Improvement starts-----------------
                            OFIModelsList ObjofiList = new OFIModelsList();
                            ObjofiList.OFIList = new List<OFIModels>();

                            string Ssql1 = "select id_ofi_improvement,id_ofi,improve_details,improve_achievedin,improve_measurable,imporve_upload from t_ofi_improvement where id_ofi = '" + sid_ofi + "'";
                            DataSet dsImpLsit = objGlobaldata.Getdetails(Ssql1);

                            if (dsImpLsit.Tables.Count > 0 && dsImpLsit.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsImpLsit.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        OFIModels objImpModels = new OFIModels
                                        {
                                            id_ofi_improvement = (dsImpLsit.Tables[0].Rows[i]["id_ofi_improvement"].ToString()),
                                            id_ofi = (dsImpLsit.Tables[0].Rows[i]["id_ofi"].ToString()),
                                            improve_details = (dsImpLsit.Tables[0].Rows[i]["improve_details"].ToString()),
                                            improve_achievedin = (dsImpLsit.Tables[0].Rows[i]["improve_achievedin"].ToString()),
                                            improve_measurable = (dsImpLsit.Tables[0].Rows[i]["improve_measurable"].ToString()),
                                            imporve_upload = (dsImpLsit.Tables[0].Rows[i]["imporve_upload"].ToString()),
                                        };
                                        ObjofiList.OFIList.Add(objImpModels);
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in OFIDetails: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }
                            ViewBag.ObjOFIImpList = ObjofiList;
                            //-------------------OFI Improvement Ends---------------

                            //-------------------OFI Document Starts---------------
                            OFIModelsList ObjOFIDocList = new OFIModelsList();
                            ObjOFIDocList.OFIList = new List<OFIModels>();

                            string Ssql11 = "select id_ofi_doc,id_ofi,doc_type,doc_ref,doc_name from t_ofi_doc where id_ofi = '" + sid_ofi + "'";
                            DataSet dsOFIList = objGlobaldata.Getdetails(Ssql11);

                            if (dsOFIList.Tables.Count > 0 && dsOFIList.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsOFIList.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        OFIModels objDocModel = new OFIModels
                                        {
                                            id_ofi_doc = (dsOFIList.Tables[0].Rows[i]["id_ofi_doc"].ToString()),
                                            id_ofi = (dsOFIList.Tables[0].Rows[i]["id_ofi"].ToString()),
                                            doc_type = (dsOFIList.Tables[0].Rows[i]["doc_type"].ToString()),
                                            doc_ref = (dsOFIList.Tables[0].Rows[i]["doc_ref"].ToString()),
                                            doc_name = (dsOFIList.Tables[0].Rows[i]["doc_name"].ToString()),
                                        };
                                        ObjOFIDocList.OFIList.Add(objDocModel);
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in OFIDetails: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }
                            ViewBag.ObjOFIDocList = ObjOFIDocList;
                            //-------------------OFI Document Ends---------------
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in OFIList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in OFIPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            //return View(objModel);
            ViewBag.ObjOFIList = objModel;
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("OFIPDF")
            {
                //FileName = "OFIPDF.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }
    }
}