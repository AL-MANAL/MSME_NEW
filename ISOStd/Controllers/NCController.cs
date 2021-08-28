using ISOStd.Filters;
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
    [PreventFromUrl]
    public class NCController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public NCController()
        {
            ViewBag.Menutype = "NC";
            ViewBag.SubMenutype = "NC";
        }

        [AllowAnonymous]
        public ActionResult AddNC()
        {
            NCModels objModel = new NCModels();
            try
            {

                //objModel.division = objGlobaldata.GetCurrentUserSession().division;
                // objModel.department = objGlobaldata.GetCurrentUserSession().DeptID;
                //objModel.location = objGlobaldata.GetCurrentUserSession().Work_Location;
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                // ViewBag.Department = objGlobaldata.GetDepartmentListbox(objModel.division);
                //ViewBag.Location = objGlobaldata.GetDivisionLocationList(objModel.division);
                objModel.nc_reportedby = objGlobaldata.GetCurrentUserSession().empid;

                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.RiskLevel = objGlobaldata.GetDropdownList("NC Risklevel");
                ViewBag.Category = objGlobaldata.GetDropdownList("NC Category");
                ViewBag.RelatedAspect = objGlobaldata.GetDropdownList("NC Related Aspect");
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.AuditNo = objGlobaldata.GetAuditNoFromAuditProcessList();
                ViewBag.RaisedDueto = objGlobaldata.GetDropdownList("NC Raised Due to");
                if (Request.QueryString["objective_Id"] != null && Request.QueryString["objective_Id"] != "")
                { 
                   ViewBag.objective_Id = Request.QueryString["objective_Id"];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddNC: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }
       
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddNC(NCModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> nc_upload)
        {
            try
            {
                if (objModel != null)
                {
                    objModel.nc_division = form["nc_division"];
                    objModel.location = form["location"];
                    objModel.department = form["department"];
                    //objModel.nc_reportedby = form["nc_reportedby"];
                    //objModel.nc_notifiedto = form["nc_notifiedto"];

                    //if (Request.Files.Count > 0)
                    if(nc_upload != null)
                    {
                    HttpPostedFileBase files = Request.Files[0];
                    if (nc_upload != null && files.ContentLength > 0)
                    {
                        objModel.nc_upload = "";
                        foreach (var file in nc_upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(file.FileName));
                                string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objModel.nc_upload = objModel.nc_upload + "," + "~/DataUpload/MgmtDocs/NC/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddNC-upload: " + ex.ToString());

                            }
                        }
                        objModel.nc_upload = objModel.nc_upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                  }
                    //Issued To
                    for (int i = 0; i < Convert.ToInt16(form["count"]); i++)
                    {
                        if (form["iempno " + i] != "" && form["iempno " + i] != null)
                        {
                            objModel.nc_issueto = form["iempno " + i] + "," + objModel.nc_issueto;
                        }
                    }
                    if (objModel.nc_issueto != null)
                    {
                        objModel.nc_issueto = objModel.nc_issueto.Trim(',');
                    }

                    //Reported By
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["empno " + i] != "" && form["empno " + i] != null)
                        {
                            objModel.nc_reportedby = objModel.nc_reportedby + "," + form["empno " + i];
                        }
                    }
                    if (objModel.nc_reportedby != null)
                    {
                        objModel.nc_reportedby = objModel.nc_reportedby.Trim(',');
                    }

                    //Notified To
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                    {
                        if (form["nempno " + i] != "" && form["nempno " + i] != null)
                        {
                            objModel.nc_notifiedto =   form["nempno " + i] + "," + objModel.nc_notifiedto;
                        }
                    }
                    if (objModel.nc_notifiedto != null)
                    {
                        objModel.nc_notifiedto = objModel.nc_notifiedto.Trim(',');
                    }

                    //NC Issue
                    //NCModelsList objNcList = new NCModelsList();
                    //objNcList.lstNC = new List<NCModels>();                   

                    //for (int i = 0; i < Convert.ToInt16(form["itemcounts"]); i++)
                    //{
                    //    NCModels objNCModel = new NCModels();
                    //    if (form["nc_issue_name " + i] != "" && form["nc_issue_name " + i] != null)
                    //    {
                    //        objNCModel.nc_issue_name = form["nc_issue_name " + i];
                    //        objNCModel.nc_issue_entity = form["nc_issue_entity " + i];
                    //        objNCModel.nc_issue_div = form["nc_issue_div " + i];
                    //        objNCModel.nc_issue_loc = form["nc_issue_loc " + i];
                    //        objNCModel.nc_issue_dept = form["nc_issue_dept " + i];

                    //        objNcList.lstNC.Add(objNCModel);
                    //    }
                    //}

                    //NC Related
                    NCModelsList objNCRelatedList = new NCModelsList();
                    objNCRelatedList.lstNC = new List<NCModels>();

                    for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                    {
                        NCModels objNCRelatedModel = new NCModels();
                        if (form["nc_related_aspect " + i] != "" && form["nc_related_aspect " + i] != null)
                        {
                            objNCRelatedModel.nc_related_aspect = form["nc_related_aspect " + i];
                            objNCRelatedModel.nc_related_explain = form["nc_related_explain " + i];
                            objNCRelatedModel.nc_related_doc = form["nc_related_doc " + i];
                           
                            objNCRelatedList.lstNC.Add(objNCRelatedModel);
                        }
                    }

                    if (objModel.FunAddNC(objModel, objNCRelatedList))
                    {
                        TempData["Successdata"] = "Added NC details successfully with NC Number '" + objModel.nc_no + "'";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddNC: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult NCList(string SearchText, string risk_status_id, int? page, string branch_name)
        {
            NCModelsList NcList = new NCModelsList();
            NcList.lstNC = new List<NCModels>();
          
            NCModels objRisk = new NCModels();

            try
            {
                //ViewBag.risk_status_id = objRisk.GetMultiRiskStatusList("Risk-Status");
                //UserCredentials objUser = new UserCredentials();
                //objUser = objGlobaldata.GetCurrentUserSession();
                //ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_nc, nc_no, nc_reported_date, nc_detected_date, nc_category, nc_description, nc_activity, nc_performed, nc_pnc, nc_upload,"
                    + "nc_impact, nc_risk, risklevel, nc_reportedby,  nc_notifiedto, nc_division, division, department, location,nc_audit,audit_no,nc_raise_dueto," +
                    "(case when nc_issuedto_status=1 then 'Accepted' end) as nc_issuedto_status,nc_issuedto_status as nc_issuedto_statusId,nc_initial_status as nc_initial_statusId,ca_verfiry_duedate from t_nc where Active=1";
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

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_nc desc";
                DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNCModels.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            NCModels objNCModels = new NCModels
                            {
                                id_nc = (dsNCModels.Tables[0].Rows[i]["id_nc"].ToString()),
                                nc_no = (dsNCModels.Tables[0].Rows[i]["nc_no"].ToString()),
                                nc_category = (dsNCModels.Tables[0].Rows[i]["nc_category"].ToString()),
                                nc_description = (dsNCModels.Tables[0].Rows[i]["nc_description"].ToString()),
                                nc_activity = (dsNCModels.Tables[0].Rows[i]["nc_activity"].ToString()),
                                nc_performed = (dsNCModels.Tables[0].Rows[i]["nc_performed"].ToString()),
                                nc_pnc = (dsNCModels.Tables[0].Rows[i]["nc_pnc"].ToString()),
                                nc_upload = (dsNCModels.Tables[0].Rows[i]["nc_upload"].ToString()),
                                nc_impact = (dsNCModels.Tables[0].Rows[i]["nc_impact"].ToString()),
                                nc_risk = (dsNCModels.Tables[0].Rows[i]["nc_risk"].ToString()),
                                risklevel = (dsNCModels.Tables[0].Rows[i]["risklevel"].ToString()),
                                nc_reportedby = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[i]["nc_reportedby"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[i]["location"].ToString()),
                                nc_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[i]["nc_notifiedto"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[i]["department"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsNCModels.Tables[0].Rows[i]["division"].ToString()),
                                nc_division = objGlobaldata.GetMultiCompanyBranchNameById(dsNCModels.Tables[0].Rows[i]["nc_division"].ToString()),
                                nc_audit = (dsNCModels.Tables[0].Rows[i]["nc_audit"].ToString()),
                                audit_no = (dsNCModels.Tables[0].Rows[i]["audit_no"].ToString()),
                                nc_raise_dueto = objGlobaldata.GetDropdownitemById(dsNCModels.Tables[0].Rows[i]["nc_raise_dueto"].ToString()),
                                nc_issuedto_status = dsNCModels.Tables[0].Rows[i]["nc_issuedto_status"].ToString(),
                                nc_issuedto_statusId = dsNCModels.Tables[0].Rows[i]["nc_issuedto_statusId"].ToString(),
                                nc_initial_statusId = dsNCModels.Tables[0].Rows[i]["nc_initial_statusId"].ToString()
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsNCModels.Tables[0].Rows[i]["nc_reported_date"].ToString(), out dtValue))
                            {
                                objNCModels.nc_reported_date = dtValue;
                            }
                            if (DateTime.TryParse(dsNCModels.Tables[0].Rows[i]["nc_detected_date"].ToString(), out dtValue))
                            {
                                objNCModels.nc_detected_date = dtValue;
                            }
                            if (DateTime.TryParse(dsNCModels.Tables[0].Rows[i]["ca_verfiry_duedate"].ToString(), out dtValue))
                            {
                                objNCModels.ca_verfiry_duedate = dtValue;
                            }

                            string sSqlstmt1 = "select nc_issuedto,nc_stauts,nc_approve_reject_date from t_nc_status where id_nc = '" + dsNCModels.Tables[0].Rows[i]["id_nc"].ToString() + "' order by id_nc_status desc";
                            DataSet dsNCStatusModels = objGlobaldata.Getdetails(sSqlstmt1);
                            if (dsNCStatusModels.Tables.Count > 0 && dsNCStatusModels.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dsNCStatusModels.Tables[0].Rows.Count; j++)
                                {
                                    if(dsNCStatusModels.Tables[0].Rows[j]["nc_stauts"].ToString() == "1")
                                        {
                                            objNCModels.nc_initial_status = objNCModels.nc_initial_status+","+ "Approved - " + objGlobaldata.GetMultiHrEmpNameById(dsNCStatusModels.Tables[0].Rows[j]["nc_issuedto"].ToString());
                                        }
                                    if (dsNCStatusModels.Tables[0].Rows[j]["nc_stauts"].ToString() == "0")
                                    {
                                        objNCModels.nc_initial_status = objNCModels.nc_initial_status + "," + "Pending - " + objGlobaldata.GetMultiHrEmpNameById(dsNCStatusModels.Tables[0].Rows[j]["nc_issuedto"].ToString());
                                    }
                                    if (dsNCStatusModels.Tables[0].Rows[j]["nc_stauts"].ToString() == "2")
                                    {
                                        objNCModels.nc_initial_status = objNCModels.nc_initial_status + "," + "Rejected - " + objGlobaldata.GetMultiHrEmpNameById(dsNCStatusModels.Tables[0].Rows[j]["nc_issuedto"].ToString());
                                    }
  
                                }
                                if (objNCModels.nc_initial_status != null)
                                {
                                    objNCModels.nc_initial_status = objNCModels.nc_initial_status.Trim(',');
                                }
                            }
                            NcList.lstNC.Add(objNCModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in NCList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(NcList.lstNC.ToList());
        }
              

        [AllowAnonymous]
        public ActionResult NCEdit()
        {
            NCModels objModel = new NCModels();

            NCModelsList NcList = new NCModelsList();
            NcList.lstNC = new List<NCModels>();
            try
            {
               
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.RiskLevel = objGlobaldata.GetDropdownList("NC Risklevel");
                ViewBag.Category = objGlobaldata.GetDropdownList("NC Category");
                ViewBag.RelatedAspect = objGlobaldata.GetDropdownList("NC Related Aspect");
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.AuditNo = objGlobaldata.GetAuditNoFromAuditProcessList();
                ViewBag.RaisedDueto = objGlobaldata.GetDropdownList("NC Raised Due to");

                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];
                    string sSqlstmt = "select id_nc, nc_no, nc_reported_date, nc_detected_date, nc_category, nc_description, nc_activity, nc_performed,  nc_pnc, nc_upload,"
                    + "nc_impact, nc_risk, risklevel, nc_reportedby,  nc_notifiedto, nc_division,division, department, location,nc_issueto,nc_audit,audit_no,nc_raise_dueto from t_nc where id_nc ='" + sid_nc + "'";

                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        //for (int i = 0; i < dsNCModels.Tables[0].Rows.Count; i++)
                        //{
                            try
                            {
                                objModel = new NCModels
                                {
                                    id_nc = (dsNCModels.Tables[0].Rows[0]["id_nc"].ToString()),
                                    nc_no = (dsNCModels.Tables[0].Rows[0]["nc_no"].ToString()),
                                    nc_category = (dsNCModels.Tables[0].Rows[0]["nc_category"].ToString()),
                                    nc_description = (dsNCModels.Tables[0].Rows[0]["nc_description"].ToString()),
                                    nc_activity = (dsNCModels.Tables[0].Rows[0]["nc_activity"].ToString()),
                                    nc_performed = (dsNCModels.Tables[0].Rows[0]["nc_performed"].ToString()),
                                    nc_pnc = (dsNCModels.Tables[0].Rows[0]["nc_pnc"].ToString()),
                                    nc_upload = (dsNCModels.Tables[0].Rows[0]["nc_upload"].ToString()),
                                    nc_impact = (dsNCModels.Tables[0].Rows[0]["nc_impact"].ToString()),
                                    nc_risk = (dsNCModels.Tables[0].Rows[0]["nc_risk"].ToString()),
                                    risklevel = (dsNCModels.Tables[0].Rows[0]["risklevel"].ToString()),
                                    nc_reportedby = /*objGlobaldata.GetMultiHrEmpNameById*/(dsNCModels.Tables[0].Rows[0]["nc_reportedby"].ToString()),
                                    location = /*objGlobaldata.GetDivisionLocationById*/(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                                    nc_notifiedto = /*objGlobaldata.GetMultiHrEmpNameById*/(dsNCModels.Tables[0].Rows[0]["nc_notifiedto"].ToString()),
                                    department = /*objGlobaldata.GetMultiDeptNameById*/(dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                                    division = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                                    nc_division = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsNCModels.Tables[0].Rows[0]["nc_division"].ToString()),
                                    nc_audit = (dsNCModels.Tables[0].Rows[0]["nc_audit"].ToString()),
                                    audit_no = (dsNCModels.Tables[0].Rows[0]["audit_no"].ToString()),
                                    nc_raise_dueto = dsNCModels.Tables[0].Rows[0]["nc_raise_dueto"].ToString(),
                                };
                                if (dsNCModels.Tables[0].Rows[0]["nc_issueto"].ToString() != "")
                                {
                                    ViewBag.IssuedToArray = (dsNCModels.Tables[0].Rows[0]["nc_issueto"].ToString()).Split(',');
                                }
                                if (dsNCModels.Tables[0].Rows[0]["nc_reportedby"].ToString() != "")
                                {
                                    ViewBag.ReportedByArray = (dsNCModels.Tables[0].Rows[0]["nc_reportedby"].ToString()).Split(',');
                                }
                                if (dsNCModels.Tables[0].Rows[0]["nc_notifiedto"].ToString() != "")
                                {
                                    ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["nc_notifiedto"].ToString()).Split(',');
                                }

                                DateTime dtValue;
                                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["nc_reported_date"].ToString(), out dtValue))
                                {
                                    objModel.nc_reported_date = dtValue;
                                }
                                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["nc_detected_date"].ToString(), out dtValue))
                                {
                                    objModel.nc_detected_date = dtValue;
                                }

                                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                                ViewBag.Department = objGlobaldata.GetDepartmentList1(dsNCModels.Tables[0].Rows[0]["nc_division"].ToString());
                                ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsNCModels.Tables[0].Rows[0]["nc_division"].ToString());

                                NcList.lstNC.Add(objModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in NCEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        //}
                    }

                    NCModelsList NcIssueList = new NCModelsList();
                    NcIssueList.lstNC = new List<NCModels>();

                    //string SSqlstmt1 = "Select id_nc_issue,nc_issue_name,nc_issue_entity,nc_issue_div,nc_issue_loc,nc_issue_dept from t_nc_issue where id_nc = '"+ sid_nc + "'";
                    //DataSet dsNcIssueLsit = objGlobaldata.Getdetails(SSqlstmt1);

                    //if(dsNcIssueLsit.Tables.Count > 0 && dsNcIssueLsit.Tables[0].Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < dsNcIssueLsit.Tables[0].Rows.Count; i++)
                    //    {
                    //        try
                    //        {
                    //            NCModels objNCModels = new NCModels
                    //            {
                    //                id_nc_issue = (dsNcIssueLsit.Tables[0].Rows[i]["id_nc_issue"].ToString()),
                    //                nc_issue_name = /*objGlobaldata.GetMultiHrEmpNameById*/(dsNcIssueLsit.Tables[0].Rows[i]["nc_issue_name"].ToString()),
                    //                nc_issue_entity = (dsNcIssueLsit.Tables[0].Rows[i]["nc_issue_entity"].ToString()),
                    //                nc_issue_loc = /*objGlobaldata.GetDivisionLocationById*/(dsNcIssueLsit.Tables[0].Rows[i]["nc_issue_loc"].ToString()),
                    //                nc_issue_dept = /*objGlobaldata.GetMultiDeptNameById*/(dsNcIssueLsit.Tables[0].Rows[i]["nc_issue_dept"].ToString()),
                    //                nc_issue_div = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsNcIssueLsit.Tables[0].Rows[i]["nc_issue_div"].ToString()),
                    //            };
                    //            NcIssueList.lstNC.Add(objNCModels);
                    //            ViewBag.NCIssueList= NcIssueList;
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            objGlobaldata.AddFunctionalLog("Exception in NCList: " + ex.ToString());
                    //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    //        }
                    //    }
                    //}

                    NCModelsList NcRelatedList = new NCModelsList();
                    NcRelatedList.lstNC = new List<NCModels>();

                    string SSqlstmt2 = "Select id_nc_related,nc_related_aspect,nc_related_explain,nc_related_doc from t_nc_related where id_nc = '" + sid_nc + "'";
                    DataSet dsNcRelatedLsit = objGlobaldata.Getdetails(SSqlstmt2);

                    if (dsNcRelatedLsit.Tables.Count > 0 && dsNcRelatedLsit.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsNcRelatedLsit.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                NCModels objNCModels = new NCModels
                                {
                                    id_nc_related = (dsNcRelatedLsit.Tables[0].Rows[i]["id_nc_related"].ToString()),
                                    nc_related_aspect = (dsNcRelatedLsit.Tables[0].Rows[i]["nc_related_aspect"].ToString()),
                                    nc_related_explain = (dsNcRelatedLsit.Tables[0].Rows[i]["nc_related_explain"].ToString()),
                                    nc_related_doc = (dsNcRelatedLsit.Tables[0].Rows[i]["nc_related_doc"].ToString()),
                                };
                                NcRelatedList.lstNC.Add(objNCModels);

                                ViewBag.NcRelatedList = NcRelatedList;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in NCEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "NC Id cannot be null";
                    return RedirectToAction("NCList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }
    
        [HttpPost]
        [AllowAnonymous]
        public ActionResult NCEdit(NCModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> nc_upload)
        {
            try
            {
                if (objModel != null)
                {
                    objModel.nc_division = form["nc_division"];
                    objModel.location = form["location"];
                    objModel.department = form["department"];

                    IList<HttpPostedFileBase> nc_uploadList = (IList<HttpPostedFileBase>)nc_upload;
                    string QCDelete = Request.Form["QCDocsValselectall"];

                    if ( nc_uploadList[0] != null)
                        {
                            objModel.nc_upload = "";
                            foreach (var file in nc_upload)
                            {
                                try
                                {
                                    string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(file.FileName));
                                    string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                    file.SaveAs(sFilepath + "/" + sFilename);
                                    objModel.nc_upload = objModel.nc_upload + "," + "~/DataUpload/MgmtDocs/NC/" + sFilename;
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddNC-upload: " + ex.ToString());

                                }
                            }
                            objModel.nc_upload = objModel.nc_upload.Trim(',');
                        }
                        else
                        {
                            ViewBag.Message = "You have not specified a file.";
                        }
                        if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                        {
                            objModel.nc_upload = objModel.nc_upload + "," + form["QCDocsVal"];
                            objModel.nc_upload = objModel.nc_upload.Trim(',');
                        }
                        else if (form["QCDocsVal"] == null && QCDelete != null && nc_uploadList[0] == null)
                        {
                            objModel.nc_upload = null;
                        }
                        else if (form["QCDocsVal"] == null && nc_uploadList[0] == null)
                        {
                            objModel.nc_upload = null;
                        }

                    //Issued To
                    for (int i = 0; i < Convert.ToInt16(form["count"]); i++)
                    {
                        if (form["iempno " + i] != "" && form["iempno " + i] != null)
                        {
                            objModel.nc_issueto = form["iempno " + i] + "," + objModel.nc_issueto;
                        }
                    }
                    if (objModel.nc_issueto != null)
                    {
                        objModel.nc_issueto = objModel.nc_issueto.Trim(',');
                    }

                    //Reported By
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["empno " + i] != "" && form["empno " + i] != null)
                        {
                            objModel.nc_reportedby = objModel.nc_reportedby + "," + form["empno " + i];
                        }
                    }
                    if (objModel.nc_reportedby != null)
                    {
                        objModel.nc_reportedby = objModel.nc_reportedby.Trim(',');
                    }

                    //Notified To
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                    {
                        if (form["nempno " + i] != "" && form["nempno " + i] != null)
                        {
                            objModel.nc_notifiedto = form["nempno " + i] + "," + objModel.nc_notifiedto;
                        }
                    }
                    if (objModel.nc_notifiedto != null)
                    {
                        objModel.nc_notifiedto = objModel.nc_notifiedto.Trim(',');
                    }

                    ////NC Issue
                    //NCModelsList objNcList = new NCModelsList();
                    //objNcList.lstNC = new List<NCModels>();

                    //for (int i = 0; i < Convert.ToInt16(form["itemcounts"]); i++)
                    //{
                    //    NCModels objNCModel = new NCModels();
                    //    if (form["nc_issue_name " + i] != "" && form["nc_issue_name " + i] != null)
                    //    {
                    //        objNCModel.nc_issue_name = form["nc_issue_name " + i];
                    //        objNCModel.nc_issue_entity = form["nc_issue_entity " + i];
                    //        objNCModel.nc_issue_div = form["nc_issue_div " + i];
                    //        objNCModel.nc_issue_loc = form["nc_issue_loc " + i];
                    //        objNCModel.nc_issue_dept = form["nc_issue_dept " + i];

                    //        objNcList.lstNC.Add(objNCModel);
                    //    }
                    //}

                    //NC Related
                    NCModelsList objNCRelatedList = new NCModelsList();
                    objNCRelatedList.lstNC = new List<NCModels>();

                    for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                    {
                        NCModels objNCRelatedModel = new NCModels();
                        if (form["nc_related_aspect " + i] != "" && form["nc_related_aspect " + i] != null)
                        {
                            objNCRelatedModel.nc_related_aspect = form["nc_related_aspect " + i];
                            objNCRelatedModel.nc_related_explain = form["nc_related_explain " + i];
                            objNCRelatedModel.nc_related_doc = form["nc_related_doc " + i];

                            objNCRelatedList.lstNC.Add(objNCRelatedModel);
                        }
                    }

                    if (objModel.FunUpdateNC(objModel, objNCRelatedList))
                    {
                        TempData["Successdata"] = "Updated NC details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }
        
        //Disposition
        [AllowAnonymous]
        public ActionResult AddDisposition()
        {
            NCModels objModel = new NCModels();
            try
            {
                 if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];
                    ViewBag.id_nc = sid_nc;

                    ViewBag.DispositonAction = objGlobaldata.GetDropdownList("NC Disposition Action");
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                    string sSqlstmt = "select id_nc,nc_no,nc_reported_date,nc_description,division,department,location,nc_issueto,nc_activity,nc_performed,nc_upload,disp_upload," +
                        "disp_action_taken,disp_explain,disp_notifiedto,disp_notifeddate from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new NCModels
                        {
                            nc_no = (dsNCModels.Tables[0].Rows[0]["nc_no"].ToString()),
                            nc_description = (dsNCModels.Tables[0].Rows[0]["nc_description"].ToString()),
                            division = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                            divisionName = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                            department = (dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                            departmentName = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                            location = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            locationName = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            disp_action_taken = (dsNCModels.Tables[0].Rows[0]["disp_action_taken"].ToString()),
                            disp_explain = (dsNCModels.Tables[0].Rows[0]["disp_explain"].ToString()),
                            disp_notifiedto = (dsNCModels.Tables[0].Rows[0]["disp_notifiedto"].ToString()),
                            nc_issueto = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[0]["nc_issueto"].ToString()),
                            nc_activity = (dsNCModels.Tables[0].Rows[0]["nc_activity"].ToString()),
                            nc_performed = (dsNCModels.Tables[0].Rows[0]["nc_performed"].ToString()),
                            nc_upload = (dsNCModels.Tables[0].Rows[0]["nc_upload"].ToString()),
                            disp_upload = (dsNCModels.Tables[0].Rows[0]["disp_upload"].ToString()),
                        };
                        if (dsNCModels.Tables[0].Rows[0]["disp_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["disp_notifiedto"].ToString()).Split(',');
                        }
                        DateTime dtValue;
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["nc_reported_date"].ToString(), out dtValue))
                        {
                            objModel.nc_reported_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["disp_notifeddate"].ToString(), out dtValue))
                        {
                            objModel.disp_notifeddate = dtValue;
                        }


                        NCModelsList NcDispList = new NCModelsList();
                        NcDispList.lstNC = new List<NCModels>();

                        string sSqlstmt1 = "select id_nc_disp_action,disp_action,disp_resp_person,disp_complete_date from t_nc_disp_action where id_nc='" + sid_nc + "'";
                        DataSet dsDispModels = objGlobaldata.Getdetails(sSqlstmt1);

                        if (dsDispModels.Tables.Count > 0 && dsDispModels.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsDispModels.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    NCModels objDispModel = new NCModels
                                    {
                                        id_nc_disp_action = (dsDispModels.Tables[0].Rows[i]["id_nc_disp_action"].ToString()),
                                        disp_action = (dsDispModels.Tables[0].Rows[i]["disp_action"].ToString()),
                                        disp_resp_person = (dsDispModels.Tables[0].Rows[i]["disp_resp_person"].ToString()),
                                    };

                                    if (DateTime.TryParse(dsDispModels.Tables[0].Rows[i]["disp_complete_date"].ToString(), out dtValue))
                                    {
                                        objDispModel.disp_complete_date = dtValue;
                                    }
                                    NcDispList.lstNC.Add(objDispModel);
                                 }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                                ViewBag.NcDispList = NcDispList;
                            }
                        }
                        
                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("NCList");
                    }                   
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("NCList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddDisposition(NCModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> disp_upload)
        {
            try
            {
                NCModelsList objModelList = new NCModelsList();
                objModelList.lstNC = new List<NCModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["disp_notifeddate"], out dateValue) == true)
                {
                    objModel.disp_notifeddate = dateValue;
                }


                //Notified To
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.disp_notifiedto = objModel.disp_notifiedto + "," + form["nempno " + i];
                    }
                }
                if (objModel.disp_notifiedto != null)
                {
                    objModel.disp_notifiedto = objModel.disp_notifiedto.Trim(',');
                }

                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    NCModels objNCModel = new NCModels();
                    if (form["disp_action " + i] != "" && form["disp_action " + i] != null)
                    {
                        objNCModel.disp_action = form["disp_action " + i];
                        objNCModel.disp_resp_person = form["disp_resp_person " + i];
                        if (DateTime.TryParse(form["disp_complete_date " + i], out dateValue) == true)
                        {
                            objNCModel.disp_complete_date = dateValue;
                        }
                        objModelList.lstNC.Add(objNCModel);
                    }
                }

                IList<HttpPostedFileBase> disp_uploadList = (IList<HttpPostedFileBase>)disp_upload;
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (disp_uploadList[0] != null)
                {
                    objModel.disp_upload = "";
                    foreach (var file in disp_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.disp_upload = objModel.disp_upload + "," + "~/DataUpload/MgmtDocs/NC/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddDisposition-upload: " + ex.ToString());
                        }
                    }
                    objModel.disp_upload = objModel.disp_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.disp_upload = objModel.disp_upload + "," + form["QCDocsVal"];
                    objModel.disp_upload = objModel.disp_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && disp_uploadList[0] == null)
                {
                    objModel.disp_upload = null;
                }
                else if (form["QCDocsVal"] == null && disp_uploadList[0] == null)
                {
                    objModel.disp_upload = null;
                }

                if (objModel.FunUpdateDisposition(objModel, objModelList))
                {
                    TempData["Successdata"] = "Added Disposition details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }


        //Team
        [AllowAnonymous]
        public ActionResult AddTeam()
        {
            NCModels objModel = new NCModels();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];
                    ViewBag.id_nc = sid_nc;

                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                    string sSqlstmt = "select id_nc,nc_no,nc_reported_date,nc_description,division,department,location,nc_issueto,nc_activity,nc_performed,nc_upload," +
                        "nc_team,team_approvedby,team_notifiedto,team_targetdate from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new NCModels
                        {
                            nc_no = (dsNCModels.Tables[0].Rows[0]["nc_no"].ToString()),
                            nc_description = (dsNCModels.Tables[0].Rows[0]["nc_description"].ToString()),
                            division = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                            divisionName = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                            department = (dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                            departmentName = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                            location = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            locationName = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            nc_team = (dsNCModels.Tables[0].Rows[0]["nc_team"].ToString()),
                            team_approvedby = (dsNCModels.Tables[0].Rows[0]["team_approvedby"].ToString()),
                            team_notifiedto = (dsNCModels.Tables[0].Rows[0]["team_notifiedto"].ToString()),
                            nc_issueto = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[0]["nc_issueto"].ToString()),
                            nc_activity = (dsNCModels.Tables[0].Rows[0]["nc_activity"].ToString()),
                            nc_performed = (dsNCModels.Tables[0].Rows[0]["nc_performed"].ToString()),
                            nc_upload = (dsNCModels.Tables[0].Rows[0]["nc_upload"].ToString()),

                        };
                        if(dsNCModels.Tables[0].Rows[0]["nc_team"].ToString() != "")
                        {
                            ViewBag.TeamArray = (dsNCModels.Tables[0].Rows[0]["nc_team"].ToString()).Split(',');
                        }

                        if (dsNCModels.Tables[0].Rows[0]["team_approvedby"].ToString() != "")
                        {
                            ViewBag.ApprovedByArray = (dsNCModels.Tables[0].Rows[0]["team_approvedby"].ToString()).Split(',');
                        }

                        if (dsNCModels.Tables[0].Rows[0]["team_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["team_notifiedto"].ToString()).Split(',');
                        }

                        DateTime dtValue;
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["nc_reported_date"].ToString(), out dtValue))
                        {
                            objModel.nc_reported_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["team_targetdate"].ToString(), out dtValue))
                        {
                            objModel.team_targetdate = dtValue;
                        }


                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("NCList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTeam: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("NCList");          
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddTeam(NCModels objModel, FormCollection form)
        {
            try
            {
                //NCModelsList objModelList = new NCModelsList();
                //objModelList.lstNC = new List<NCModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["team_targetdate"], out dateValue) == true)
                {
                    objModel.team_targetdate = dateValue;
                }

                //Team
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["tempno " + i] != "" && form["tempno " + i] != null)
                    {
                        objModel.nc_team = objModel.nc_team + "," + form["tempno " + i];
                    }
                }
                if (objModel.nc_team != null)
                {
                    objModel.nc_team = objModel.nc_team.Trim(',');
                }

                //ApprovedBy
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["aempno " + i] != "" && form["aempno " + i] != null)
                    {
                        objModel.team_approvedby = objModel.team_approvedby + "," + form["aempno " + i];
                    }
                }
                if (objModel.team_approvedby != null)
                {
                    objModel.team_approvedby = objModel.team_approvedby.Trim(',');
                }


                //Notified To
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.team_notifiedto = objModel.team_notifiedto + "," + form["nempno " + i];
                    }
                }
                if (objModel.team_notifiedto != null)
                {
                    objModel.team_notifiedto = objModel.team_notifiedto.Trim(',');
                }
                //for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                //{
                //    NCModels objNCModel = new NCModels();
                //    if (form["measure " + i] != "" && form["measure " + i] != null)
                //    {
                //        //objNCModel.mit_id = form["mit_id " + i];
                //        //objNCModel.measure = form["measure " + i];
                //        //objNCModel.pers_resp = form["pers_resp " + i];
                //        //if (DateTime.TryParse(form["target_date " + i], out dateValue) == true)
                //        //{
                //        //    objNCModel.target_date = dateValue;
                //        //}
                //        objModelList.lstNC.Add(objNCModel);
                //    }
                //}

                if (objModel.FunUpdateTeam(objModel))
                {
                    TempData["Successdata"] = "Added Team details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTeam: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        //RCA
        public ActionResult AddRCA()
        {
            NCModels objModel = new NCModels();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];
                    ViewBag.id_nc = sid_nc;

                    ViewBag.RCATechniqueList = objGlobaldata.GetDropdownList("RCA Technique");
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                    
                    string sSqlstmt = "select id_nc,nc_no,nc_reported_date,nc_description,division,department,location,nc_issueto,nc_activity,nc_performed,nc_upload," +
                        "rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate,rca_startedon from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new NCModels
                        {
                            nc_no = (dsNCModels.Tables[0].Rows[0]["nc_no"].ToString()),
                            nc_description = (dsNCModels.Tables[0].Rows[0]["nc_description"].ToString()),
                            division = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                            divisionName = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                            department = (dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                            departmentName = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                            location = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            locationName = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            nc_issueto = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[0]["nc_issueto"].ToString()),
                            nc_activity = (dsNCModels.Tables[0].Rows[0]["nc_activity"].ToString()),
                            nc_performed = (dsNCModels.Tables[0].Rows[0]["nc_performed"].ToString()),
                            nc_upload = (dsNCModels.Tables[0].Rows[0]["nc_upload"].ToString()),

                            rca_technique = (dsNCModels.Tables[0].Rows[0]["rca_technique"].ToString()),
                            rca_details = (dsNCModels.Tables[0].Rows[0]["rca_details"].ToString()),
                            rca_upload = (dsNCModels.Tables[0].Rows[0]["rca_upload"].ToString()),
                            rca_action = (dsNCModels.Tables[0].Rows[0]["rca_action"].ToString()),
                            rca_justify = (dsNCModels.Tables[0].Rows[0]["rca_justify"].ToString()),
                            rca_reportedby = (dsNCModels.Tables[0].Rows[0]["rca_reportedby"].ToString()),
                            rca_notifiedto = (dsNCModels.Tables[0].Rows[0]["rca_notifiedto"].ToString()),
                        };
                       
                        if (dsNCModels.Tables[0].Rows[0]["rca_reportedby"].ToString() != "")
                        {
                            ViewBag.ReportedByArray = (dsNCModels.Tables[0].Rows[0]["rca_reportedby"].ToString()).Split(',');
                        }
                        else
                        {
                            ViewBag.ReportedByArray = (objGlobaldata.GetCurrentUserSession().empid).Split(',');
                        }

                        if (dsNCModels.Tables[0].Rows[0]["rca_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["rca_notifiedto"].ToString()).Split(',');
                        }

                        DateTime dtValue;
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["nc_reported_date"].ToString(), out dtValue))
                        {
                            objModel.nc_reported_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["rca_startedon"].ToString(), out dtValue))
                        {
                            objModel.rca_startedon = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["rca_reporteddate"].ToString(), out dtValue))
                        {
                            objModel.rca_reporteddate = dtValue;
                        }

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("NCList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRCA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }        
            return RedirectToAction("NCList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddRCA(NCModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> rca_upload)
        {
            try
            {
                NCModelsList objModelList = new NCModelsList();
                objModelList.lstNC = new List<NCModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["rca_startedon"], out dateValue) == true)
                {
                    objModel.rca_startedon = dateValue;
                }

                if (DateTime.TryParse(form["rca_reporteddate"], out dateValue) == true)
                {
                    objModel.rca_reporteddate = dateValue;
                }

                IList<HttpPostedFileBase> rca_uploadList = (IList<HttpPostedFileBase>)rca_upload;
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (rca_uploadList[0] != null)
                {
                    objModel.rca_upload = "";
                    foreach (var file in rca_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.rca_upload = objModel.rca_upload + "," + "~/DataUpload/MgmtDocs/NC/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddRCA-upload: " + ex.ToString());

                        }
                    }
                    objModel.rca_upload = objModel.rca_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.rca_upload = objModel.rca_upload + "," + form["QCDocsVal"];
                    objModel.rca_upload = objModel.rca_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && rca_uploadList[0] == null)
                {
                    objModel.rca_upload = null;
                }
                else if (form["QCDocsVal"] == null && rca_uploadList[0] == null)
                {
                    objModel.rca_upload = null;
                }

                //Reported By
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["aempno " + i] != "" && form["aempno " + i] != null)
                    {
                        objModel.rca_reportedby = objModel.rca_reportedby + "," + form["aempno " + i];
                    }
                }
                if (objModel.rca_reportedby != null)
                {
                    objModel.rca_reportedby = objModel.rca_reportedby.Trim(',');
                }


                //Notifed To
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.rca_notifiedto = objModel.rca_notifiedto + "," + form["nempno " + i];
                    }
                }
                if (objModel.rca_notifiedto != null)
                {  
                    objModel.rca_notifiedto = objModel.rca_notifiedto.Trim(',');
                }

                //for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                //{
                //    NCModels objNCModel = new NCModels();
                //    if (form["measure " + i] != "" && form["measure " + i] != null)
                //    {
                //        objNCModel.mit_id = form["mit_id " + i];
                //        objNCModel.measure = form["measure " + i];
                //        objNCModel.pers_resp = form["pers_resp " + i];
                //        if (DateTime.TryParse(form["target_date " + i], out dateValue) == true)
                //        {
                //            objNCModel.target_date = dateValue;
                //        }
                //        objModelList.lstNC.Add(objNCModel);
                //    }
                //}

                if (objModel.FunUpdateRCA(objModel, objModelList))
                {
                    TempData["Successdata"] = "Added Root Cause Analysis details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRCA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        //Corrective Action
        public ActionResult AddCA()
        {
            NCModels objModel = new NCModels();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];
                    ViewBag.id_nc = sid_nc;

                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                   
                    string sSqlstmt = "select id_nc,nc_no,nc_reported_date,nc_description,division,department,location,nc_issueto,nc_activity,nc_performed,nc_upload,rca_details," +
                        "ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new NCModels
                        {
                            nc_no = (dsNCModels.Tables[0].Rows[0]["nc_no"].ToString()),
                            nc_description = (dsNCModels.Tables[0].Rows[0]["nc_description"].ToString()),
                            division = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                            divisionName = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                            department = (dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                            departmentName = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                            location = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            locationName = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            nc_issueto = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[0]["nc_issueto"].ToString()),
                            nc_activity = (dsNCModels.Tables[0].Rows[0]["nc_activity"].ToString()),
                            nc_performed = (dsNCModels.Tables[0].Rows[0]["nc_performed"].ToString()),
                            nc_upload = (dsNCModels.Tables[0].Rows[0]["nc_upload"].ToString()),
                            rca_details = (dsNCModels.Tables[0].Rows[0]["rca_details"].ToString()),

                            ca_proposed_by = (dsNCModels.Tables[0].Rows[0]["ca_proposed_by"].ToString()),
                            ca_notifiedto = (dsNCModels.Tables[0].Rows[0]["ca_notifiedto"].ToString()),
                        };
                        if(dsNCModels.Tables[0].Rows[0]["ca_proposed_by"].ToString() != "")
                        {
                            ViewBag.ReportedByArray = (dsNCModels.Tables[0].Rows[0]["ca_proposed_by"].ToString()).Split(',');
                        }
                        if (dsNCModels.Tables[0].Rows[0]["ca_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["ca_notifiedto"].ToString()).Split(',');
                        }

                        DateTime dtValue;
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["nc_reported_date"].ToString(), out dtValue))
                        {
                            objModel.nc_reported_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["ca_verfiry_duedate"].ToString(), out dtValue))
                        {
                            objModel.ca_verfiry_duedate = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["ca_notifed_date"].ToString(), out dtValue))
                        {
                            objModel.ca_notifed_date = dtValue;
                        }


                        NCModelsList CAList = new NCModelsList();
                        CAList.lstNC = new List<NCModels>();

                        string Ssql = "select id_nc_corrective_action,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
                        "ca_target_date,ca_resp_person from t_nc_corrective_action where id_nc = '" + sid_nc + "' and ca_active=1";

                        DataSet dsCALsit = objGlobaldata.Getdetails(Ssql);

                        if (dsCALsit.Tables.Count > 0 && dsCALsit.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsCALsit.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    NCModels objNCModels = new NCModels
                                    {
                                        id_nc_corrective_action = (dsCALsit.Tables[0].Rows[i]["id_nc_corrective_action"].ToString()),
                                        //ca_div = (dsCALsit.Tables[0].Rows[i]["ca_div"].ToString()),
                                        //ca_loc = (dsCALsit.Tables[0].Rows[i]["ca_loc"].ToString()),
                                        //ca_dept = (dsCALsit.Tables[0].Rows[i]["ca_dept"].ToString()),
                                        //ca_rootcause = (dsCALsit.Tables[0].Rows[i]["ca_rootcause"].ToString()),
                                        ca_ca = (dsCALsit.Tables[0].Rows[i]["ca_ca"].ToString()),
                                        ca_resource = (dsCALsit.Tables[0].Rows[i]["ca_resource"].ToString()),
                                        ca_resp_person = (dsCALsit.Tables[0].Rows[i]["ca_resp_person"].ToString()),
                                    };
                                    if (DateTime.TryParse(dsCALsit.Tables[0].Rows[i]["ca_target_date"].ToString(), out dtValue))
                                    {
                                        objNCModels.ca_target_date = dtValue;
                                    }
                                    CAList.lstNC.Add(objNCModels);                                   
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in NCEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }
                        ViewBag.CorrectiveAction = CAList;

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("NCList");
                    }                    
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("NCList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddCA(NCModels objModel, FormCollection form)
        {
            try
            {
                NCModelsList objModelList = new NCModelsList();
                objModelList.lstNC = new List<NCModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["ca_verfiry_duedate"], out dateValue) == true)
                {
                    objModel.ca_verfiry_duedate = dateValue;
                }

                if (DateTime.TryParse(form["ca_notifed_date"], out dateValue) == true)
                {
                    objModel.ca_notifed_date = dateValue;
                }


                //Reported By
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["aempno " + i] != "" && form["aempno " + i] != null)
                    {
                        objModel.ca_proposed_by = objModel.ca_proposed_by + "," + form["aempno " + i];
                    }
                }
                if (objModel.ca_proposed_by != null)
                {
                    objModel.ca_proposed_by = objModel.ca_proposed_by.Trim(',');
                }


                //Notifed To
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.ca_notifiedto = objModel.ca_notifiedto + "," + form["nempno " + i];
                    }
                }
                if (objModel.ca_notifiedto != null)
                {
                    objModel.ca_notifiedto = objModel.ca_notifiedto.Trim(',');
                }

                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    NCModels objNCModel = new NCModels();
                    if (form["ca_ca " + i] != "" && form["ca_resource " + i] != null)
                    {
                        objNCModel.id_nc_corrective_action = form["id_nc_corrective_action " + i];
                        //objNCModel.ca_div = form["ca_div " + i];
                        //objNCModel.ca_loc = form["ca_loc " + i];
                        //objNCModel.ca_dept = form["ca_dept " + i];
                        //objNCModel.ca_rootcause = form["ca_rootcause " + i];
                        objNCModel.ca_ca = form["ca_ca " + i];
                        objNCModel.ca_resource = form["ca_resource " + i];
                        objNCModel.ca_resp_person = form["ca_resp_person " + i];
                        if (DateTime.TryParse(form["ca_target_date " + i], out dateValue) == true)
                        {
                            objNCModel.ca_target_date = dateValue;
                        }
                        objModelList.lstNC.Add(objNCModel);
                    }
                }

                if (objModel.FunUpdateCA(objModel, objModelList))
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
                objGlobaldata.AddFunctionalLog("Exception in AddCA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        //Verification
        public ActionResult AddVerification()
        {
            NCModels objModel = new NCModels();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];
                    ViewBag.id_nc = sid_nc;

                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                    ViewBag.NCRStatus = objGlobaldata.GetDropdownList("NCR Status");
                    ViewBag.Action = objGlobaldata.GetDropdownList("NC Action Implementation");
                    ViewBag.OpenClose = objGlobaldata.GetConstantValue("OpenClose");

                    string sSqlstmt = "select id_nc,nc_no,nc_reported_date,nc_description,division,department,location,nc_issueto,nc_activity,nc_performed,nc_upload,rca_details," +
                        "v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_upload," +
                        "v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new NCModels
                        {
                            nc_no = (dsNCModels.Tables[0].Rows[0]["nc_no"].ToString()),
                            nc_description = (dsNCModels.Tables[0].Rows[0]["nc_description"].ToString()),
                            division = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                            divisionName = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                            department = (dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                            departmentName = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                            location = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            locationName = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            nc_issueto = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[0]["nc_issueto"].ToString()),
                            nc_activity = (dsNCModels.Tables[0].Rows[0]["nc_activity"].ToString()),
                            nc_performed = (dsNCModels.Tables[0].Rows[0]["nc_performed"].ToString()),
                            nc_upload = (dsNCModels.Tables[0].Rows[0]["nc_upload"].ToString()),
                            rca_details= dsNCModels.Tables[0].Rows[0]["rca_details"].ToString(),

                            v_implement = (dsNCModels.Tables[0].Rows[0]["v_implement"].ToString()),
                            v_implement_explain = (dsNCModels.Tables[0].Rows[0]["v_implement_explain"].ToString()),
                            v_rca = (dsNCModels.Tables[0].Rows[0]["v_rca"].ToString()),
                            v_rca_explain = (dsNCModels.Tables[0].Rows[0]["v_rca_explain"].ToString()),
                            v_discrepancies = (dsNCModels.Tables[0].Rows[0]["v_discrepancies"].ToString()),
                            v_discrep_explain = (dsNCModels.Tables[0].Rows[0]["v_discrep_explain"].ToString()),
                            v_upload = (dsNCModels.Tables[0].Rows[0]["v_upload"].ToString()),
                            v_status = (dsNCModels.Tables[0].Rows[0]["v_status"].ToString()),
                            v_verifiedto = (dsNCModels.Tables[0].Rows[0]["v_verifiedto"].ToString()),
                            v_notifiedto = (dsNCModels.Tables[0].Rows[0]["v_notifiedto"].ToString()),
                        };

                        if (dsNCModels.Tables[0].Rows[0]["v_verifiedto"].ToString() != "")
                        {
                            ViewBag.ReportedByArray = (dsNCModels.Tables[0].Rows[0]["v_verifiedto"].ToString()).Split(',');
                        }
                        else
                        {
                            ViewBag.ReportedByArray = (objGlobaldata.GetCurrentUserSession().empid).Split(',');
                        }

                        if (dsNCModels.Tables[0].Rows[0]["v_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["v_notifiedto"].ToString()).Split(',');
                        }

                        DateTime dtValue;
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["nc_reported_date"].ToString(), out dtValue))
                        {
                            objModel.nc_reported_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["v_verified_date"].ToString(), out dtValue))
                        {
                            objModel.v_verified_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["v_closed_date"].ToString(), out dtValue))
                        {
                            objModel.v_closed_date = dtValue;
                        }

                        NCModelsList objVeriList = new NCModelsList();
                        objVeriList.lstNC = new List<NCModels>();

                        string Sql = "Select id_nc_corrective_action,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
                       "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_nc_corrective_action where id_nc = '" + sid_nc + "' and ca_active=1";
                        
                        DataSet dsCAModels = objGlobaldata.Getdetails(Sql);

                        if (dsCAModels.Tables.Count > 0 && dsCAModels.Tables[0].Rows.Count > 0)
                        {
                            for (int i=0;i< dsCAModels.Tables[0].Rows.Count;i++)
                            {
                                try
                                {
                                    NCModels objCAModel = new NCModels
                                    {
                                        id_nc_corrective_action = (dsCAModels.Tables[0].Rows[i]["id_nc_corrective_action"].ToString()),
                                        ca_div = (dsCAModels.Tables[0].Rows[i]["ca_div"].ToString()),
                                        ca_loc = (dsCAModels.Tables[0].Rows[i]["ca_loc"].ToString()),
                                        ca_dept = (dsCAModels.Tables[0].Rows[i]["ca_dept"].ToString()),
                                        ca_rootcause = (dsCAModels.Tables[0].Rows[i]["ca_rootcause"].ToString()),
                                        ca_ca = (dsCAModels.Tables[0].Rows[i]["ca_ca"].ToString()),
                                        ca_resource = (dsCAModels.Tables[0].Rows[i]["ca_resource"].ToString()),
                                        ca_resp_person = (dsCAModels.Tables[0].Rows[i]["ca_resp_person"].ToString()),
                                        implement_status = (dsCAModels.Tables[0].Rows[i]["implement_status"].ToString()),
                                        ca_effective = (dsCAModels.Tables[0].Rows[i]["ca_effective"].ToString()),
                                        reason = (dsCAModels.Tables[0].Rows[i]["reason"].ToString()),
                                    };

                                    if (DateTime.TryParse(dsCAModels.Tables[0].Rows[i]["ca_target_date"].ToString(), out dtValue))
                                    {
                                        objCAModel.ca_target_date = dtValue;
                                    }
                                    objVeriList.lstNC.Add(objCAModel);
                                }
                                catch(Exception Ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddVerification: " + Ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                           }
                            ViewBag.CorrectiveAction = objVeriList;
                        }

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("NCList");
                    }

                 }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVerification: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }           
            return RedirectToAction("NCList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddVerification(NCModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> v_upload)
        {
            try
            {
                NCModelsList objModelList = new NCModelsList();
                objModelList.lstNC = new List<NCModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["team_targetdate"], out dateValue) == true)
                {
                    objModel.team_targetdate = dateValue;
                }

                IList<HttpPostedFileBase> v_uploadList = (IList<HttpPostedFileBase>)v_upload;
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (v_uploadList[0] != null)
                {
                    objModel.v_upload = "";
                    foreach (var file in v_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.v_upload = objModel.v_upload + "," + "~/DataUpload/MgmtDocs/NC/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddVerification-upload: " + ex.ToString());
                        }
                    }
                    objModel.v_upload = objModel.v_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.v_upload = objModel.v_upload + "," + form["QCDocsVal"];
                    objModel.v_upload = objModel.v_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && v_uploadList[0] == null)
                {
                    objModel.v_upload = null;
                }
                else if (form["QCDocsVal"] == null && v_uploadList[0] == null)
                {
                    objModel.v_upload = null;
                }
                //Notified By
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["aempno " + i] != "" && form["aempno " + i] != null)
                    {
                        objModel.v_verifiedto = objModel.v_verifiedto + "," + form["aempno " + i];
                    }
                }
                if (objModel.v_verifiedto != null)
                {
                    objModel.v_verifiedto = objModel.v_verifiedto.Trim(',');
                }


                //Notifed To
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.v_notifiedto = objModel.v_notifiedto + "," + form["nempno " + i];
                    }
                }
                if (objModel.v_notifiedto != null)
                {
                    objModel.v_notifiedto = objModel.v_notifiedto.Trim(',');
                }


                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    NCModels objNCModel = new NCModels();
                    if (form["id_nc_corrective_action " + i] != "" && form["id_nc_corrective_action " + i] != null)
                    {
                        // objNCModel.ca_div = form["ca_div " + i];
                        // objNCModel.ca_loc = form["ca_loc " + i];
                        //   objNCModel.ca_dept = form["ca_dept " + i];
                        //  //objNCModel.ca_rootcause = form["ca_rootcause " + i];
                        //  objNCModel.ca_ca = form["ca_ca " + i];
                        //objNCModel.ca_resource = form["ca_resource " + i];
                        //objNCModel.ca_resp_person = form["ca_resp_person " + i];
                        //if (DateTime.TryParse(form["ca_target_date " + i], out dateValue) == true)
                        //{
                        //    objNCModel.ca_target_date = dateValue;
                        //}
                        objNCModel.id_nc_corrective_action = form["id_nc_corrective_action " + i];
                        objNCModel.implement_status = form["implement_status " + i];
                        objNCModel.ca_effective = form["ca_effective " + i];
                        objNCModel.reason = form["reason " + i];
                    }
                    objModelList.lstNC.Add(objNCModel);
                }

                if (objModel.FunUpdateVerification(objModel, objModelList))
                {
                    TempData["Successdata"] = "Added Verification details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVerification: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        //NC Details
        [AllowAnonymous]
        public ActionResult NCDetails()
        {
            NCModels objModel = new NCModels();

            NCModelsList NcList = new NCModelsList();
            NcList.lstNC = new List<NCModels>();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];
                  
                    string sSqlstmt = "select id_nc, nc_no, nc_reported_date, nc_detected_date, nc_category, nc_description, nc_activity, nc_performed, nc_pnc, nc_upload,"
                   + "nc_impact, nc_risk, risklevel, nc_reportedby,  nc_notifiedto, nc_division, division, department, location,nc_audit,audit_no,nc_raise_dueto,nc_issueto," +
                   "(case when nc_issuedto_status=1 then 'Accepted' end) as nc_issuedto_status,nc_issuedto_status as nc_issuedto_statusId,nc_initial_status as nc_initial_statusId,nc_issuer_rejector,nc_issuers from t_nc where id_nc ='" + sid_nc + "'";
                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                            try
                            {
                                objModel = new NCModels
                                {
                                    id_nc = (dsNCModels.Tables[0].Rows[0]["id_nc"].ToString()),
                                    nc_no = (dsNCModels.Tables[0].Rows[0]["nc_no"].ToString()),
                                    nc_category = objGlobaldata.GetDropdownitemById(dsNCModels.Tables[0].Rows[0]["nc_category"].ToString()),
                                    nc_description = (dsNCModels.Tables[0].Rows[0]["nc_description"].ToString()),
                                    nc_activity = (dsNCModels.Tables[0].Rows[0]["nc_activity"].ToString()),
                                    nc_performed = (dsNCModels.Tables[0].Rows[0]["nc_performed"].ToString()),
                                    nc_pnc = (dsNCModels.Tables[0].Rows[0]["nc_pnc"].ToString()),
                                    nc_upload = (dsNCModels.Tables[0].Rows[0]["nc_upload"].ToString()),
                                    nc_impact = (dsNCModels.Tables[0].Rows[0]["nc_impact"].ToString()),
                                    nc_risk = (dsNCModels.Tables[0].Rows[0]["nc_risk"].ToString()),
                                    risklevel =objGlobaldata.GetDropdownitemById(dsNCModels.Tables[0].Rows[0]["risklevel"].ToString()),
                                    nc_reportedby = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[0]["nc_reportedby"].ToString()),
                                    emp_id = (dsNCModels.Tables[0].Rows[0]["nc_reportedby"].ToString()),
                                    location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                                    nc_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[0]["nc_notifiedto"].ToString()),
                                    nc_notifiedtoId =(dsNCModels.Tables[0].Rows[0]["nc_notifiedto"].ToString()),
                                    department = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                                    division = objGlobaldata.GetMultiCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                                    nc_issueto = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[0]["nc_issueto"].ToString()),
                                    nc_issuetoId = (dsNCModels.Tables[0].Rows[0]["nc_issueto"].ToString()),
                                    nc_division = objGlobaldata.GetMultiCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["nc_division"].ToString()),
                                    nc_audit = (dsNCModels.Tables[0].Rows[0]["nc_audit"].ToString()),
                                    audit_no = objGlobaldata.GetAuditNoFromAuditProcessById(dsNCModels.Tables[0].Rows[0]["audit_no"].ToString()),
                                    nc_raise_dueto = objGlobaldata.GetDropdownitemById(dsNCModels.Tables[0].Rows[0]["nc_raise_dueto"].ToString()),
                                    nc_issuedto_status = dsNCModels.Tables[0].Rows[0]["nc_issuedto_status"].ToString(),
                                    nc_issuedto_statusId = dsNCModels.Tables[0].Rows[0]["nc_issuedto_statusId"].ToString(),
                                    nc_issuer_rejector = dsNCModels.Tables[0].Rows[0]["nc_issuer_rejector"].ToString(),
                                    nc_issuers = dsNCModels.Tables[0].Rows[0]["nc_issuers"].ToString(),
                                    nc_initial_statusId = dsNCModels.Tables[0].Rows[0]["nc_initial_statusId"].ToString(),
                                };

                               DateTime dtValue;
                                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["nc_reported_date"].ToString(), out dtValue))
                                {
                                    objModel.nc_reported_date = dtValue;
                                }
                                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["nc_detected_date"].ToString(), out dtValue))
                                {
                                    objModel.nc_detected_date = dtValue;
                                }
                            string sSqlstmt1 = "select nc_issuedto,nc_stauts,nc_approve_reject_date from t_nc_status where id_nc = '" + dsNCModels.Tables[0].Rows[0]["id_nc"].ToString() + "' order by id_nc_status desc";
                            DataSet dsNCStatusModels = objGlobaldata.Getdetails(sSqlstmt1);
                            if (dsNCStatusModels.Tables.Count > 0 && dsNCStatusModels.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dsNCStatusModels.Tables[0].Rows.Count; j++)
                                {
                                    if (DateTime.TryParse(dsNCStatusModels.Tables[0].Rows[j]["nc_approve_reject_date"].ToString(), out dtValue))
                                    {
                                        objModel.nc_approve_reject_date = dtValue;
                                    }
                                    if (dsNCStatusModels.Tables[0].Rows[j]["nc_stauts"].ToString() == "1")
                                    {
                                        objModel.nc_initial_status = objModel.nc_initial_status + "," + "Approved - " + objGlobaldata.GetMultiHrEmpNameById(dsNCStatusModels.Tables[0].Rows[j]["nc_issuedto"].ToString())+" - " + objModel.nc_approve_reject_date;
                                    }
                                    if (dsNCStatusModels.Tables[0].Rows[j]["nc_stauts"].ToString() == "0")
                                    {
                                        objModel.nc_initial_status = objModel.nc_initial_status + "," + "Pending - " + objGlobaldata.GetMultiHrEmpNameById(dsNCStatusModels.Tables[0].Rows[j]["nc_issuedto"].ToString()) + " - " + objModel.nc_approve_reject_date;
                                    }
                                    if (dsNCStatusModels.Tables[0].Rows[j]["nc_stauts"].ToString() == "2")
                                    {
                                        objModel.nc_initial_status = objModel.nc_initial_status + "," + "Rejected - " + objGlobaldata.GetMultiHrEmpNameById(dsNCStatusModels.Tables[0].Rows[j]["nc_issuedto"].ToString()) + " - " + objModel.nc_approve_reject_date;
                                    }                                   
                                }
                               
                                if (objModel.nc_initial_status != null)
                                {
                                    objModel.nc_initial_status = objModel.nc_initial_status.Trim(',');
                                }
                            }
                            NcList.lstNC.Add(objModel);
                          }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in NCDetails: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                    }

                    string SSqlstmt11 = "Select nc_issuedto,(case when nc_stauts= 0 then 'Pending' when nc_stauts = 1 then 'Approved' when nc_stauts = 2 then 'Rejected' end) as nc_stauts,nc_approve_reject_date,nc_reject_comment,nc_reject_upload from t_nc_status where id_nc = '" + sid_nc + "'";
                    DataSet dsNcStatusList = objGlobaldata.Getdetails(SSqlstmt11);
                    ViewBag.NcStatus = dsNcStatusList;

                    string SSqlstmt2 = "Select id_nc_related,nc_related_aspect,nc_related_explain,nc_related_doc from t_nc_related where id_nc = '" + sid_nc + "'";
                    DataSet dsNcRelatedLsit = objGlobaldata.Getdetails(SSqlstmt2);
                    ViewBag.NcRelated = dsNcRelatedLsit;

                    //Disposition
                    string sSqlstmt11 = "select id_nc,nc_no,nc_reported_date,nc_description,division,department,location," +
                       "disp_action_taken,disp_explain,disp_notifiedto,disp_notifeddate,disp_upload from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsDispModel = objGlobaldata.Getdetails(sSqlstmt11);
                    ViewBag.Disposition = dsDispModel;

                    string sSqlstmt12 = "select id_nc_disp_action,disp_action,disp_resp_person,disp_complete_date from t_nc_disp_action where id_nc='" + sid_nc + "'";
                    DataSet dsDispModels = objGlobaldata.Getdetails(sSqlstmt12);
                    ViewBag.DispAction = dsDispModels;

                    //Team
                    string sSqlstmt13 = "select nc_team,team_approvedby,team_notifiedto,team_targetdate from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsTeamModel = objGlobaldata.Getdetails(sSqlstmt13);
                    ViewBag.Team = dsTeamModel;

                    //RCA
                    string sSqlstmt14 = "select rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate,rca_startedon from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsRCAModels = objGlobaldata.Getdetails(sSqlstmt14);
                    ViewBag.RCA = dsRCAModels;

                    //CA
                    string sSqlstmt15 = "select ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsCAModels = objGlobaldata.Getdetails(sSqlstmt15);
                    ViewBag.CA = dsCAModels;

                    string sSqlstmt16 = "select id_nc_corrective_action,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
                        "ca_target_date,ca_resp_person from t_nc_corrective_action where id_nc = '" + sid_nc + "' and ca_active=1";
                    DataSet dsCAList = objGlobaldata.Getdetails(sSqlstmt16);
                    ViewBag.CAList = dsCAList;

                    //Verification
                    string sSqlstmt17 = "select v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_upload," +
                        "v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsVerifyModels = objGlobaldata.Getdetails(sSqlstmt17);
                    ViewBag.Verification = dsVerifyModels;

                    string sSqlstmt18 = "Select id_nc_corrective_action,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
                    "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_nc_corrective_action where id_nc = '" + sid_nc + "' and ca_active=1";
                    DataSet dsVerifyModel = objGlobaldata.Getdetails(sSqlstmt18);
                    ViewBag.VerificationList = dsVerifyModel;

                }
                else
                {
                    TempData["alertdata"] = "NC Id cannot be null";
                    return RedirectToAction("NCList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //NC Details       
        public ActionResult NCPDF()
        {
            NCModels objModel = new NCModels();
            CompanyModels objCompany = new CompanyModels();

            NCModelsList NcList = new NCModelsList();
            NcList.lstNC = new List<NCModels>();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];
                    string sSqlstmt = "select id_nc, nc_no, nc_reported_date, nc_detected_date, nc_category, nc_description, nc_activity, nc_performed,  nc_pnc, nc_upload,"
                    + "nc_impact, nc_risk, risklevel, nc_reportedby,  nc_notifiedto, nc_division,division, department, location,nc_issueto,logged_by,nc_audit,audit_no,nc_raise_dueto from t_nc where id_nc ='" + sid_nc + "'";

                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                       try
                        {
                            objModel = new NCModels
                            {
                                id_nc = (dsNCModels.Tables[0].Rows[0]["id_nc"].ToString()),
                                nc_no = (dsNCModels.Tables[0].Rows[0]["nc_no"].ToString()),
                                nc_category = objGlobaldata.GetDropdownitemById(dsNCModels.Tables[0].Rows[0]["nc_category"].ToString()),
                                nc_description = (dsNCModels.Tables[0].Rows[0]["nc_description"].ToString()),
                                nc_activity = (dsNCModels.Tables[0].Rows[0]["nc_activity"].ToString()),
                                nc_performed = (dsNCModels.Tables[0].Rows[0]["nc_performed"].ToString()),
                                nc_pnc = (dsNCModels.Tables[0].Rows[0]["nc_pnc"].ToString()),
                                nc_upload = (dsNCModels.Tables[0].Rows[0]["nc_upload"].ToString()),
                                nc_impact = (dsNCModels.Tables[0].Rows[0]["nc_impact"].ToString()),
                                nc_risk = (dsNCModels.Tables[0].Rows[0]["nc_risk"].ToString()),
                                risklevel = objGlobaldata.GetDropdownitemById(dsNCModels.Tables[0].Rows[0]["risklevel"].ToString()),
                                nc_reportedby = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[0]["nc_reportedby"].ToString()),
                                emp_id = (dsNCModels.Tables[0].Rows[0]["nc_reportedby"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                                nc_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[0]["nc_notifiedto"].ToString()),
                                nc_notifiedtoId = (dsNCModels.Tables[0].Rows[0]["nc_notifiedto"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["department"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                                nc_issueto = objGlobaldata.GetMultiHrEmpNameById(dsNCModels.Tables[0].Rows[0]["nc_issueto"].ToString()),
                                nc_issuetoId = (dsNCModels.Tables[0].Rows[0]["nc_issueto"].ToString()),
                                logged_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsNCModels.Tables[0].Rows[0]["logged_by"].ToString()),
                                nc_division = objGlobaldata.GetMultiCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["nc_division"].ToString()),
                                nc_audit = (dsNCModels.Tables[0].Rows[0]["nc_audit"].ToString()),
                                audit_no = objGlobaldata.GetAuditNoFromAuditProcessById(dsNCModels.Tables[0].Rows[0]["audit_no"].ToString()),
                                nc_raise_dueto = objGlobaldata.GetDropdownitemById(dsNCModels.Tables[0].Rows[0]["nc_raise_dueto"].ToString()),
                            };                          

                            DateTime dtValue;
                            if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["nc_reported_date"].ToString(), out dtValue))
                            {
                                objModel.nc_reported_date = dtValue;
                            }
                            if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["nc_detected_date"].ToString(), out dtValue))
                            {
                                objModel.nc_detected_date = dtValue;
                            }
                            ViewBag.NonConfirmance = objModel;


                            //string sql = "select reported_by from t_accident_report where id_accident_rept='" + objIncidentReport.accident_reportno + "'";
                            //DataSet dsData = objGlobaldata.Getdetails(sql);

                            dsNCModels = objCompany.GetCompanyDetailsForReport(dsNCModels);
                            dsNCModels = objGlobaldata.GetReportDetails(dsNCModels, objModel.nc_no, dsNCModels.Tables[0].Rows[0]["logged_by"].ToString(), "NON CONFORMANCE REPORT");

                            ViewBag.CompanyInfo = dsNCModels;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in NCDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }                        
                    }                   

                    NCModelsList NcRelatedList = new NCModelsList();
                    NcRelatedList.lstNC = new List<NCModels>();

                    string SSqlstmt2 = "Select id_nc_related,nc_related_aspect,nc_related_explain,nc_related_doc from t_nc_related where id_nc = '" + sid_nc + "'";
                    DataSet dsNcRelatedLsit = objGlobaldata.Getdetails(SSqlstmt2);
                   
                    if (dsNcRelatedLsit.Tables.Count > 0 && dsNcRelatedLsit.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsNcRelatedLsit.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                NCModels objNCModels = new NCModels
                                {
                                    id_nc_related = (dsNcRelatedLsit.Tables[0].Rows[i]["id_nc_related"].ToString()),
                                    nc_related_aspect = objGlobaldata.GetDropdownitemById(dsNcRelatedLsit.Tables[0].Rows[i]["nc_related_aspect"].ToString()),
                                    nc_related_explain = (dsNcRelatedLsit.Tables[0].Rows[i]["nc_related_explain"].ToString()),
                                    nc_related_doc = (dsNcRelatedLsit.Tables[0].Rows[i]["nc_related_doc"].ToString()),
                                };
                                NcRelatedList.lstNC.Add(objNCModels);                                
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in NCEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    ViewBag.NcRelatedList = NcRelatedList;

                    //Disposition
                    string sSqlstmt11 = "select id_nc,nc_no,nc_reported_date,nc_description,division,department,location," +
                       "disp_action_taken,disp_explain,disp_notifiedto,disp_notifeddate,disp_upload from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsDispModel = objGlobaldata.Getdetails(sSqlstmt11);
                    ViewBag.Disposition = dsDispModel;

                    string sSqlstmt12 = "select id_nc_disp_action,disp_action,disp_resp_person,disp_complete_date from t_nc_disp_action where id_nc='" + sid_nc + "'";
                    DataSet dsDispModels = objGlobaldata.Getdetails(sSqlstmt12);
                    ViewBag.DispAction = dsDispModels;

                    //Team
                    string sSqlstmt13 = "select nc_team,team_approvedby,team_notifiedto,team_targetdate from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsTeamModel = objGlobaldata.Getdetails(sSqlstmt13);
                    ViewBag.Team = dsTeamModel;

                    //RCA
                    string sSqlstmt14 = "select rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate,rca_startedon from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsRCAModels = objGlobaldata.Getdetails(sSqlstmt14);
                    ViewBag.RCA = dsRCAModels;

                    //CA
                    string sSqlstmt15 = "select ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsCAModels = objGlobaldata.Getdetails(sSqlstmt15);
                    ViewBag.CA = dsCAModels;

                    string sSqlstmt16 = "select id_nc_corrective_action,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
                        "ca_target_date,ca_resp_person from t_nc_corrective_action where id_nc = '" + sid_nc + "' and ca_active=1";
                    DataSet dsCAList = objGlobaldata.Getdetails(sSqlstmt16);
                    ViewBag.CAList = dsCAList;

                    //Verification
                    string sSqlstmt17 = "select v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_upload," +
                        "v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto from t_nc where id_nc='" + sid_nc + "'";
                    DataSet dsVerifyModels = objGlobaldata.Getdetails(sSqlstmt17);
                    ViewBag.Verification = dsVerifyModels;

                    string sSqlstmt18 = "Select id_nc_corrective_action,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
                    "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_nc_corrective_action where id_nc = '" + sid_nc + "' and ca_active=1";
                    DataSet dsVerifyModel = objGlobaldata.Getdetails(sSqlstmt18);
                    ViewBag.VerificationList = dsVerifyModel;

                    //For PDF Report
                    string sSqlstmtRpt = "select immediate_action,team,ca,rca,verification from t_pfd_report where report_name= 'Main Non Conformance' and active=1";
                    DataSet RptPDF = objGlobaldata.Getdetails(sSqlstmtRpt);

                    if (RptPDF.Tables.Count > 0 && RptPDF.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.ncimmediate_action = RptPDF.Tables[0].Rows[0]["immediate_action"].ToString();
                        ViewBag.ncteam = RptPDF.Tables[0].Rows[0]["team"].ToString();
                        ViewBag.ncca = RptPDF.Tables[0].Rows[0]["ca"].ToString();
                        ViewBag.ncrca = RptPDF.Tables[0].Rows[0]["rca"].ToString();
                        ViewBag.ncverification = RptPDF.Tables[0].Rows[0]["verification"].ToString();
                    }
                }
                else
                {
                    TempData["alertdata"] = "NC Id cannot be null";
                    return RedirectToAction("NCList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("NCPDF")
            {
                FileName = "NCPDF.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }


        //Delete

        [AllowAnonymous]
        public JsonResult NCDocDelete(FormCollection form)
        {
            try
            {

                if (form["id_nc"] != null && form["id_nc"] != "")
                {

                    NCModels Doc = new NCModels();
                    string sid_nc = form["id_nc"];

                    if (Doc.FunDeleteNCDoc(sid_nc))
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
                objGlobaldata.AddFunctionalLog("Exception in NCDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult CADocDelete(FormCollection form)
        {
            try
            {

                if (form["id_nc_corrective_action"] != null && form["id_nc_corrective_action"] != "")
                {

                    NCModels Doc = new NCModels();
                    string sid_nc_corrective_action = form["id_nc_corrective_action"];

                    if (Doc.FunDeleteCADoc(sid_nc_corrective_action))
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
                objGlobaldata.AddFunctionalLog("Exception in CADocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public JsonResult FunGetEmpDetails(string semp_no)
        {

            NCModels objModels = new NCModels();
            try
            {
                string sSqlstmt = "select emp_no,emp_id,division,Emp_work_location,Dept_Id,EmailId from t_hr_employee where emp_no = '" + semp_no + "'";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    objModels = new NCModels
                    {
                        //emp_no = Convert.ToInt32(dsList.Tables[0].Rows[0]["emp_no"].ToString()),
                        emp_id = (dsList.Tables[0].Rows[0]["emp_id"].ToString()),
                        empname = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["emp_no"].ToString()),
                        division = objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[0]["division"].ToString()),
                        location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[0]["Emp_work_location"].ToString()),
                        department = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["Dept_Id"].ToString()),
                        EmailId = (dsList.Tables[0].Rows[0]["EmailId"].ToString())
                    };
                }
                return Json(objModels);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetEmpDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }


        [AllowAnonymous]
        public ActionResult NCApproveReject(FormCollection form, HttpPostedFileBase nc_reject_upload)
        {
            try
            {
                NCModels objNC = new NCModels();

                objNC.id_nc = form["id_nc"];
                objNC.nc_reject_comment = form["nc_reject_comment"];
                //objNC.nc_issuedto = objGlobaldata.GetCurrentUserSession().empid;
                objNC.nc_stauts = form["nc_stauts"];
                string sStatus = "";

                if (nc_reject_upload != null && nc_reject_upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(nc_reject_upload.FileName));
                        string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        nc_reject_upload.SaveAs(sFilepath + "/" + sFilename);
                        objNC.nc_reject_upload = "~/DataUpload/MgmtDocs/NC/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in NCApproveReject: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objNC.nc_stauts == "0")
                {
                    sStatus = "Pending";
                }
                else if (objNC.nc_stauts == "1")
                {
                    sStatus = "Accepted";
                }
                else if (objNC.nc_stauts == "2")
                {
                    sStatus = "Rejected";
                }
                if (objNC.FunNCApproveOrReject(objNC))
                {
                    TempData["Successdata"] = "NC is " + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
               //string path = Url.Action();
                string path = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
                string[] url = path.Split('/');

                //var controller = url[1];
                if (url[3] == "NC" || url[4] == "NC")
                {
                    //string del = Request.Url.ToString();
                    return RedirectToAction("NCList", "NC");
                }
                else
                {
                    return RedirectToAction("ListPendingForApproval", "Dashboard");
                }            
        }


        //Pdf report format        
        public ActionResult NCPDFReportFormat()
        {
            ReportPDFModels objModel = new ReportPDFModels();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    ViewBag.id_nc = Request.QueryString["id_nc"];

                    //ViewBag.YesNo = objGlobaldata.GetConstantValueKeyValuePair("YesNo");

                    string sSqlstmt = "select immediate_action,team,ca,rca,verification from t_pfd_report where report_name= 'Main Non Conformance' and active=1";
                    DataSet dsNCModel = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModel.Tables.Count > 0 && dsNCModel.Tables[0].Rows.Count > 0)
                    {
                        objModel = new ReportPDFModels
                        {
                            immediate_action = (dsNCModel.Tables[0].Rows[0]["immediate_action"].ToString()),
                            team = (dsNCModel.Tables[0].Rows[0]["team"].ToString()),
                            ca = (dsNCModel.Tables[0].Rows[0]["ca"].ToString()),
                            rca = (dsNCModel.Tables[0].Rows[0]["rca"].ToString()),
                            verification = (dsNCModel.Tables[0].Rows[0]["verification"].ToString()),
                        };

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerComplaintDetails", new { id_complaint = objModel.id_complaint });
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCPDFReportFormat: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("NCDetails", new { id_nc = objModel.id_nc });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult NCPDFReportFormat(ReportPDFModels objModel)
        {
            ReportPDFModels objRptModel = new ReportPDFModels();
            try
            {
                if (objModel.FunUpdatePDFReportFormat(objModel, "Main Non Conformance"))
                {
                    TempData["Successdata"] = "Updated PDF Report setting successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCPDFReportFormat: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("NCPDF", new { id_nc = objModel.id_nc });
        }

    }
}