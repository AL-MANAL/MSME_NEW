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
using Microsoft.Reporting.WebForms;
using ISOStd.Filters;
using Rotativa;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class ObjectivesController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public ObjectivesController()
        {
            ViewBag.Menutype = "ObjKPI";
            ViewBag.SubMenutype = "Objectives";
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AddObjectives()
        {
            return InitilizeAddObjectives();
        }

        private ActionResult InitilizeAddObjectives()
        {
            ObjectivesModels objObjectives = new ObjectivesModels();
            try
            {               
                ViewBag.View = Request.QueryString["View"];
                if (ViewBag.View == "1")
                {
                    ViewBag.IsostdName = "ISO 9001:2015";
                }
                if (ViewBag.View == "2")
                {
                    ViewBag.IsostdName = "ISO 45001:2018";
                }
                if (ViewBag.View == "3")
                {
                    ViewBag.IsostdName = "ISO 14001:2015";
                }
                ViewBag.LoginEmp = objGlobaldata.GetCurrentUserSession().empid;
                ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                //ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.Freq_of_Eval = objGlobaldata.GetDropdownList("Objective Frequency Evaluation");
                ViewBag.Status_Obj_Eval = objObjectives.GetMultiObjectiveStatusList();
                ViewBag.RespPerson = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.EmpLists = objGlobaldata.GetDeptHeadList();

                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                ViewBag.Objlevel = objGlobaldata.GetDropdownList("Objective Level");
                ViewBag.Approver = objGlobaldata.GetApprover();
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.ObjectInlineWith = objObjectives.GetObjectiveInlineList();
                ViewBag.Unit = objObjectives.GetObjectiveUnitList();
                ViewBag.userbranch = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(ViewBag.userbranch);
                objObjectives.Location = objGlobaldata.GetCurrentUserSession().Work_Location;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddObjectives: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objObjectives);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddObjectives(ObjectivesModels objObjectives, FormCollection form, HttpPostedFileBase fileUploader)
        {
            string sView = form["view"];
            try
            {
                if (objObjectives != null)
                {
                    objObjectives.Dept = form["Dept"];
                    objObjectives.Audit_Criteria = objGlobaldata.GetIsoStdIdByName(form["Audit_Criteria"]);
                    //objObjectives.Estld_by = form["Estld_by"];
                    objObjectives.CreatedBy = objGlobaldata.GetCurrentUserSession().empid;
                    objObjectives.branch = form["branch"];
                    objObjectives.Location = form["Location"];
                    if (objObjectives.branch == null || objObjectives.branch == "")
                    {
                        objObjectives.branch_view = objGlobaldata.GetCurrentUserSession().division;
                    }
                    else
                    {
                        objObjectives.branch_view = objGlobaldata.GetCurrentUserSession().division + "," + objObjectives.branch;
                    }

                    //Estld_by
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                    {
                        if (form["nempno " + i] != "" && form["nempno " + i] != null)
                        {
                            objObjectives.Estld_by = form["nempno " + i] + "," + objObjectives.Estld_by;
                        }
                    }
                    if (objObjectives.Estld_by != null)
                    {
                        objObjectives.Estld_by = objObjectives.Estld_by.Trim(',');
                    }

                    ObjectivesModelsList objObjectivesModelsList = new ObjectivesModelsList();
                    objObjectivesModelsList.ObjectivesMList = new List<ObjectivesModels>();


                    if (fileUploader != null && fileUploader.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Objectives"), Path.GetFileName(fileUploader.FileName));
                            string sFilename = objObjectives.Obj_Ref + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            fileUploader.SaveAs(sFilepath + "/" + sFilename);
                            objObjectives.DocUploadPath = "~/DataUpload/MgmtDocs/Objectives/" + sFilename;
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

                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        ObjectivesModels objObjectivesModels = new ObjectivesModels();

                        DateTime dateValue;

                        if (DateTime.TryParse(form["Obj_Estld_On" + i], out dateValue) == true)
                        {
                            objObjectivesModels.Obj_Estld_On = dateValue;
                        }
                        if (DateTime.TryParse(form["Target_Date" + i], out dateValue) == true)
                        {
                            objObjectivesModels.Target_Date = dateValue;
                        }

                        objObjectivesModels.Objectives_val = form["Objectives_val" + i];
                        objObjectivesModels.Obj_Target = form["Obj_Target" + i];
                        objObjectivesModels.Base_Line_Value = form["Base_Line_Value" + i];
                        objObjectivesModels.Accepted_Value = form["Accepted_Value" + i];
                        //objObjectivesModels.Personal_Responsible = form["Personal_Responsible" + i];
                        objObjectivesModels.Approved_By = form["Approved_By" + i];

                        objObjectivesModels.Monitoring_Mechanism = form["Monitoring_Mechanism" + i];
                        if (form["Action_Plan" + i] != null && form["Action_Plan" + i] != "")
                        {
                            objObjectivesModels.Action_Plan = form["Action_Plan" + i];
                        }

                        objObjectivesModels.Freq_of_Eval = form["Freq_of_Eval" + i];
                        objObjectivesModels.Risk_ifObjFails = form["Risk_ifObjFails" + i];

                        objObjectivesModels.baseline_data = form["baseline_data" + i];
                        objObjectivesModels.unit = form["unit" + i];
                        objObjectivesModels.obj_inline = form["obj_inline" + i];

                        objObjectivesModelsList.ObjectivesMList.Add(objObjectivesModels);

                    }

                    if (objObjectives.FunAddObjectives(objObjectives, objObjectivesModelsList, fileUploader))
                    {

                        TempData["Successdata"] = "Added Objective details successfully  with Reference Number '" + objObjectives.Obj_Ref + "'";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddObjectives: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ObjectivesList", new { View = sView });
        }
               
        [AllowAnonymous]
        public ActionResult ObjectivesList(string branch_name)
        {
            // string sObjectivesTrans_Id = Request.QueryString["ObjectivesTrans_Id"];
            ObjectivesModelsList objObjectivesModelsList = new ObjectivesModelsList();
            objObjectivesModelsList.ObjectivesMList = new List<ObjectivesModels>();

            ObjectivesModels objObjective = new ObjectivesModels();
            
            try
            {
                ViewBag.View = Request.QueryString["View"];
                string sQuality = objGlobaldata.GetIsoStdIdByName("ISO 9001:2015");
                string sHealth = objGlobaldata.GetIsoStdIdByName("ISO 45001:2018");
                string sEnvironment = objGlobaldata.GetIsoStdIdByName("ISO 14001:2015");

                string sSqlstmt = "select a.Objectives_Id,ObjectivesTrans_Id, Obj_Ref, Dept, Audit_Criteria, Estld_by,"
                + "CreatedBy,DocUploadPath,objective_level,branch,Location,Obj_Estld_On,Objectives_val,Obj_Target,Base_Line_Value," +
                "Monitoring_Mechanism,Target_Date,Action_Plan,Freq_of_Eval,Accepted_Value,Risk_ifObjFails,baseline_data,unit,obj_inline," +
                "Approved_By,ApprovedDate,Approved_Status as Approved_StatusId, (case when  Approved_Status=0 then 'Pending for Approval'  when Approved_Status=1 " +
                "then 'Approved' when Approved_Status=2 then 'Rejected' end) as Approved_Status from t_objectives a, t_objectives_trans b"
                + " where Active=1 and a.Objectives_Id=b.Objectives_Id and trans_active=1";

                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch_view)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and  find_in_set('" + sBranch_name + "', branch_view) ";
                }

                if (ViewBag.View == "1")
                {
                    ViewBag.SubMenutype = "Objectives1";
                    sSqlstmt = sSqlstmt + " and Audit_Criteria= '" + sQuality + "' order by b.ObjectivesTrans_Id desc";
                    Session["state"] = ViewBag.View;
                }

                if (ViewBag.View == "2")
                {
                    ViewBag.SubMenutype = "Objectives2";
                    sSqlstmt = sSqlstmt + " and Audit_Criteria= '" + sHealth + "' order by b.ObjectivesTrans_Id desc";
                    Session["state"] = ViewBag.View;
                }
                if (ViewBag.View == "3")
                {
                    ViewBag.SubMenutype = "Objectives3";
                    sSqlstmt = sSqlstmt + " and Audit_Criteria= '" + sEnvironment + "' order by b.ObjectivesTrans_Id desc";
                    Session["state"] = ViewBag.View;
                }

                DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objObjective = new ObjectivesModels
                            {
                                Objectives_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["Objectives_Id"].ToString()),
                                ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["ObjectivesTrans_Id"].ToString()),
                                Obj_Ref = dsObjectivesModelsList.Tables[0].Rows[i]["Obj_Ref"].ToString(),
                                Dept = objGlobaldata.GetMultiDeptNameById(dsObjectivesModelsList.Tables[0].Rows[i]["Dept"].ToString()),
                                Dept_Head = objGlobaldata.GetDeptHeadNameByDepartment(dsObjectivesModelsList.Tables[0].Rows[i]["Dept"].ToString()),
                                //Freq_of_Eval = (dsObjectivesModelsList.Tables[0].Rows[i]["Freq_of_Eval"].ToString()),
                                // Personal_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[i]["Personal_Responsible"].ToString()),
                                //Personal_Responsible_EditCheck = objGlobaldata.GetMultiEmpNameByIdEdit(dsObjectivesModelsList.Tables[0].Rows[i]["Personal_Responsible"].ToString()),
                                //Personal_Responsible_EditCheck2=objGlobaldata.GetCurrentUserSession().empid,
                                Audit_Criteria = objGlobaldata.GetIsoStdDescriptionById(dsObjectivesModelsList.Tables[0].Rows[i]["Audit_Criteria"].ToString()),
                                Estld_by = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[i]["Estld_by"].ToString()),
                                CreatedBy = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[i]["CreatedBy"].ToString()),
                                DocUploadPath = dsObjectivesModelsList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                                // EstYear = dsObjectivesModelsList.Tables[0].Rows[i]["EstYear"].ToString(),
                                objective_level = objGlobaldata.GetDropdownitemById(dsObjectivesModelsList.Tables[0].Rows[i]["objective_level"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsObjectivesModelsList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsObjectivesModelsList.Tables[0].Rows[i]["Location"].ToString()),

                                Objectives_val = dsObjectivesModelsList.Tables[0].Rows[i]["Objectives_val"].ToString(),
                                Obj_Target = dsObjectivesModelsList.Tables[0].Rows[i]["Obj_Target"].ToString(),
                                Base_Line_Value = dsObjectivesModelsList.Tables[0].Rows[i]["Base_Line_Value"].ToString(),
                                Monitoring_Mechanism = dsObjectivesModelsList.Tables[0].Rows[i]["Monitoring_Mechanism"].ToString(),
                                Action_Plan = dsObjectivesModelsList.Tables[0].Rows[i]["Action_Plan"].ToString(),
                                Freq_of_Eval = objGlobaldata.GetDropdownitemById(dsObjectivesModelsList.Tables[0].Rows[i]["Freq_of_Eval"].ToString()),
                                Accepted_Value = dsObjectivesModelsList.Tables[0].Rows[i]["Accepted_Value"].ToString(),
                                Risk_ifObjFails = dsObjectivesModelsList.Tables[0].Rows[i]["Risk_ifObjFails"].ToString(),
                                baseline_data = dsObjectivesModelsList.Tables[0].Rows[i]["baseline_data"].ToString(),
                                unit = objObjective.GetObjectiveUnitById(dsObjectivesModelsList.Tables[0].Rows[i]["unit"].ToString()),
                                obj_inline = objObjective.GetObjectiveInlineById(dsObjectivesModelsList.Tables[0].Rows[i]["obj_inline"].ToString()),
                                Approved_By = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[i]["Approved_By"].ToString()),
                                Approved_Status = dsObjectivesModelsList.Tables[0].Rows[i]["Approved_Status"].ToString(),
                                Approved_Status_id = dsObjectivesModelsList.Tables[0].Rows[i]["Approved_StatusId"].ToString(),
                            };

                            DateTime dtDocDate = new DateTime();

                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[i]["Obj_Estld_On"].ToString(), out dtDocDate))
                            {
                                objObjective.Obj_Estld_On = dtDocDate;
                            }
                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[i]["Target_Date"].ToString(), out dtDocDate))
                            {
                                objObjective.Target_Date = dtDocDate;
                            }
                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[i]["ApprovedDate"].ToString(), out dtDocDate))
                            {
                                objObjective.ApprovedDate = dtDocDate;
                            }


                            //ObjectivesModelsList evallist = new ObjectivesModelsList();
                            //evallist.ObjectivesMList = new List<ObjectivesModels>();

                            //string sql = "select ObjectivesTrans_Id, Objectives_Id, Obj_Estld_On, Objectives_val, Obj_Target,"
                            //+ "Base_Line_Value, Monitoring_Mechanism, Action_Plan,Target_Date, Freq_of_Eval,Personal_Responsible,Approved_By,"
                            //+ "(case when Approved_Status = '1' then 'Approved' when Approved_Status = '2' then 'Rejected' else 'Not Approved' end)"
                            //+ " as Approved_Status,Approved_Status as Approved_Status_id, ApprovedDate,Accepted_Value,Risk_ifObjFails from t_objectives_trans"
                            //+ " where  Objectives_Id='" + objObjectivesModels.Objectives_Id + "' order by ObjectivesTrans_Id desc";
                            //DataSet dsData = objGlobaldata.Getdetails(sql);
                            //if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                            //{
                            //    for (int j = 0; j < dsData.Tables[0].Rows.Count; j++)
                            //    {
                            //        evalModel = new ObjectivesModels
                            //        {
                            //            ObjectivesTrans_Id = Convert.ToInt32(dsData.Tables[0].Rows[j]["ObjectivesTrans_Id"].ToString()),
                            //            Objectives_Id = Convert.ToInt32(dsData.Tables[0].Rows[j]["Objectives_Id"].ToString()),
                            //            Objectives_val = dsData.Tables[0].Rows[j]["Objectives_val"].ToString(),
                            //            Obj_Target = dsData.Tables[0].Rows[j]["Obj_Target"].ToString(),
                            //            Base_Line_Value = dsData.Tables[0].Rows[j]["Base_Line_Value"].ToString(),
                            //            Monitoring_Mechanism = dsData.Tables[0].Rows[j]["Monitoring_Mechanism"].ToString(),
                            //            Action_Plan = dsData.Tables[0].Rows[j]["Action_Plan"].ToString(),
                            //            Freq_of_Eval = (dsData.Tables[0].Rows[j]["Freq_of_Eval"].ToString()),
                            //            Personal_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[j]["Personal_Responsible"].ToString()),
                            //            Approved_By = objGlobaldata.GetEmpHrNameById(dsData.Tables[0].Rows[j]["Approved_By"].ToString()),
                            //            Approved_Status = dsData.Tables[0].Rows[j]["Approved_Status"].ToString(),
                            //            Approved_Status_id = dsData.Tables[0].Rows[j]["Approved_Status_id"].ToString(),
                            //            Accepted_Value = dsData.Tables[0].Rows[j]["Accepted_Value"].ToString(),
                            //            Risk_ifObjFails = dsData.Tables[0].Rows[j]["Risk_ifObjFails"].ToString(),
                            //        };
                            //        if (dsData.Tables[0].Rows[j]["Obj_Estld_On"].ToString() != ""
                            //    && DateTime.TryParse(dsData.Tables[0].Rows[j]["Obj_Estld_On"].ToString(), out dtDocDate))
                            //        {
                            //            evalModel.Obj_Estld_On = dtDocDate;
                            //        }
                            //        if (dsData.Tables[0].Rows[j]["Target_Date"].ToString() != ""
                            //          && DateTime.TryParse(dsData.Tables[0].Rows[j]["Target_Date"].ToString(), out dtDocDate))
                            //        {
                            //            evalModel.Target_Date = dtDocDate;
                            //        }
                            //        if (dsData.Tables[0].Rows[j]["ApprovedDate"].ToString() != ""
                            //            && DateTime.TryParse(dsData.Tables[0].Rows[j]["ApprovedDate"].ToString(), out dtDocDate))
                            //        {
                            //            evalModel.ApprovedDate = dtDocDate;
                            //        }
                            ////        ObjectivesModelsList evallist1 = new ObjectivesModelsList();
                            ////        evallist1.ObjectivesMList = new List<ObjectivesModels>();

                            ////        string sql1 = "select ObjectivesEval_Id, ObjectivesTrans_Id, Obj_Eval_On, FromPeriod, ToPeriod, Obj_Achieved_Val, Trend, NCR_Ref, Evidence,Status_Obj_Eval  from t_objectives_evaluation where ObjectivesTrans_Id='" + evalModel.ObjectivesTrans_Id + "'";
                            ////        DataSet sdsData = objGlobaldata.Getdetails(sql1);
                            ////        if (sdsData.Tables.Count > 0 && sdsData.Tables[0].Rows.Count > 0)
                            ////        {
                            ////            for (int k = 0; k < sdsData.Tables[0].Rows.Count; k++)
                            ////            {
                            ////                evalModels = new ObjectivesModels
                            ////                {
                            ////                    ObjectivesEval_Id = Convert.ToInt32(sdsData.Tables[0].Rows[k]["ObjectivesEval_Id"].ToString()),
                            ////                    ObjectivesTrans_Id = Convert.ToInt32(sdsData.Tables[0].Rows[k]["ObjectivesTrans_Id"].ToString()),
                            ////                    Obj_Achieved_Val = sdsData.Tables[0].Rows[k]["Obj_Achieved_Val"].ToString(),
                            ////                    Trend = objObjectivesModels.GetMultiObjectiveEvaluationById(sdsData.Tables[0].Rows[k]["Trend"].ToString()),
                            ////                    NCR_Ref = objGlobaldata.GetNCRNumberById(sdsData.Tables[0].Rows[k]["NCR_Ref"].ToString()),
                            ////                    Evidence = sdsData.Tables[0].Rows[k]["Evidence"].ToString(),
                            ////                    Status_Obj_Eval = objObjectivesModels.GetObjectiveStatusNameById(sdsData.Tables[0].Rows[k]["Status_Obj_Eval"].ToString()),
                            ////                };
                            ////                if (sdsData.Tables[0].Rows[k]["Obj_Eval_On"].ToString() != ""
                            ////  && DateTime.TryParse(sdsData.Tables[0].Rows[k]["Obj_Eval_On"].ToString(), out dtDocDate))
                            ////                {
                            ////                    evalModels.Obj_Eval_On = dtDocDate;
                            ////                }
                            ////                if (sdsData.Tables[0].Rows[k]["FromPeriod"].ToString() != ""
                            ////&& DateTime.TryParse(sdsData.Tables[0].Rows[k]["FromPeriod"].ToString(), out dtDocDate))
                            ////                {
                            ////                    evalModels.FromPeriod = dtDocDate;
                            ////                }
                            ////                if (sdsData.Tables[0].Rows[k]["ToPeriod"].ToString() != ""
                            ////&& DateTime.TryParse(sdsData.Tables[0].Rows[k]["ToPeriod"].ToString(), out dtDocDate))
                            ////                {
                            ////                    evalModels.ToPeriod = dtDocDate;
                            ////                }
                            ////                evallist1.ObjectivesMList.Add(evalModels);
                            ////            }
                            ////        }
                            ////        evalModel.ObjectivesEvalList = evallist1.ObjectivesMList;

                            ////        evallist.ObjectivesMList.Add(evalModel);
                            //    }
                            //}

                            //   objObjectivesModels.ObjectivesMList = evallist.ObjectivesMList;

                            objObjectivesModelsList.ObjectivesMList.Add(objObjective);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ObjectivesList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectivesList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            //ViewBag.ObjectivesTrans_Id = sObjectivesTrans_Id;
            ViewBag.Freq_of_Eval = objGlobaldata.GetConstantValue("Freq_of_Eval");

            return View(objObjectivesModelsList.ObjectivesMList.ToList());
        }
        
        [AllowAnonymous]
        public ActionResult ObjectivesDetails()
        {
            ObjectivesModels objObjectivesModels = new ObjectivesModels();

            ViewBag.View = Request.QueryString["View"];
            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();

                string sObjectives_Id = Request.QueryString["Objectives_Id"];
                string sObjectivesTrans_Id = Request.QueryString["ObjectivesTrans_Id"];

                if (sObjectives_Id != null)
                {
                    if (ViewBag.View == "1")
                    {
                        ViewBag.IsostdName = "ISO 9001:2015";
                        ViewBag.SubMenutype = "Objectives1";
                    }
                    if (ViewBag.View == "2")
                    {
                        ViewBag.IsostdName = "ISO 45001:2018";
                        ViewBag.SubMenutype = "Objectives2";
                    }
                    if (ViewBag.View == "3")
                    {
                        ViewBag.IsostdName = "ISO 14001:2015";
                        ViewBag.SubMenutype = "Objectives3";
                    }

                    string sSqlstmt = "select a.Objectives_Id,ObjectivesTrans_Id, Obj_Ref, Dept, Audit_Criteria, Estld_by,"
                    + "CreatedBy,DocUploadPath,objective_level,branch,Location,Obj_Estld_On,Objectives_val,Obj_Target,Base_Line_Value," +
                    "Monitoring_Mechanism,Target_Date,Action_Plan,Freq_of_Eval,Accepted_Value,Risk_ifObjFails,baseline_data,unit,obj_inline,Approved_By," +
                    "ApprovedDate,Approved_Status as Approved_StatusId, (case when  Approved_Status=0 then 'Pending for Approval' when Approved_Status=1 " +
                    "then 'Approved' when Approved_Status=2 then 'Rejected' end) as Approved_Status,obj_reject_comment,obj_reject_upload from t_objectives a, t_objectives_trans b"
                    + " where Active=1 and trans_active=1 and a.Objectives_Id=b.Objectives_Id and b.ObjectivesTrans_Id='" + sObjectivesTrans_Id + "'";

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                         {
                            objObjectivesModels = new ObjectivesModels
                            {
                                Objectives_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["Objectives_Id"].ToString()),
                                ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["ObjectivesTrans_Id"].ToString()),
                                Obj_Ref = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Ref"].ToString(),
                                Dept = objGlobaldata.GetMultiDeptNameById(dsObjectivesModelsList.Tables[0].Rows[0]["Dept"].ToString()),
                                Audit_Criteria = objGlobaldata.GetIsoStdDescriptionById(dsObjectivesModelsList.Tables[0].Rows[0]["Audit_Criteria"].ToString()),
                                Estld_by = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[0]["Estld_by"].ToString()),
                                CreatedBy = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[0]["CreatedBy"].ToString()),
                                DocUploadPath = dsObjectivesModelsList.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                                objective_level = objGlobaldata.GetDropdownitemById(dsObjectivesModelsList.Tables[0].Rows[0]["objective_level"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsObjectivesModelsList.Tables[0].Rows[0]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsObjectivesModelsList.Tables[0].Rows[0]["Location"].ToString()),

                                Objectives_val = dsObjectivesModelsList.Tables[0].Rows[0]["Objectives_val"].ToString(),
                                Obj_Target = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Target"].ToString(),
                                Base_Line_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Base_Line_Value"].ToString(),
                                Monitoring_Mechanism = dsObjectivesModelsList.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString(),
                                Action_Plan = dsObjectivesModelsList.Tables[0].Rows[0]["Action_Plan"].ToString(),
                                Freq_of_Eval = objGlobaldata.GetDropdownitemById(dsObjectivesModelsList.Tables[0].Rows[0]["Freq_of_Eval"].ToString()),
                                Accepted_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Accepted_Value"].ToString(),
                                Risk_ifObjFails = dsObjectivesModelsList.Tables[0].Rows[0]["Risk_ifObjFails"].ToString(),
                                baseline_data = dsObjectivesModelsList.Tables[0].Rows[0]["baseline_data"].ToString(),
                                unit = objObjectivesModels.GetObjectiveUnitById(dsObjectivesModelsList.Tables[0].Rows[0]["unit"].ToString()),
                                obj_inline = objObjectivesModels.GetObjectiveInlineById(dsObjectivesModelsList.Tables[0].Rows[0]["obj_inline"].ToString()),
                                Approved_By = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[0]["Approved_By"].ToString()),
                                Approved_ById = (dsObjectivesModelsList.Tables[0].Rows[0]["Approved_By"].ToString()),
                                Approved_Status = dsObjectivesModelsList.Tables[0].Rows[0]["Approved_Status"].ToString(),
                                Approved_Status_id = dsObjectivesModelsList.Tables[0].Rows[0]["Approved_StatusId"].ToString(),
                                obj_reject_comment = dsObjectivesModelsList.Tables[0].Rows[0]["obj_reject_comment"].ToString(),
                                obj_reject_upload = dsObjectivesModelsList.Tables[0].Rows[0]["obj_reject_upload"].ToString(),

                            };

                            DateTime dtDocDate = new DateTime();

                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Estld_On"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.Obj_Estld_On = dtDocDate;
                            }
                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["Target_Date"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.Target_Date = dtDocDate;
                            }
                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["ApprovedDate"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.ApprovedDate = dtDocDate;
                            }
                        }

                        ViewBag.ObjectivesTrans_Id = sObjectivesTrans_Id;

                        //PCFF
                        ObjectivesModelsList objPCFFList = new ObjectivesModelsList();
                        objPCFFList.ObjectivesMList = new List<ObjectivesModels>();

                        string sSqlstmtPCFF = "select id_potential, ObjectivesTrans_Id,potential_causes,impact,mitigation_measure,targeted_on,updated_on,potential_status,logged_by,Pcff_Notify " +
                            "from t_objectives_potential where ObjectivesTrans_Id  = '" + sObjectivesTrans_Id + "' and pot_active=1";
                       
                        DataSet dsPCFFList = objGlobaldata.Getdetails(sSqlstmtPCFF);
                        if (dsPCFFList.Tables.Count > 0 && dsPCFFList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsPCFFList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    ObjectivesModels objplanModels = new ObjectivesModels
                                    {
                                        id_potential = (dsPCFFList.Tables[0].Rows[i]["id_potential"].ToString()),
                                        ObjectivesTrans_Id = Convert.ToInt16(dsPCFFList.Tables[0].Rows[i]["ObjectivesTrans_Id"].ToString()),
                                        potential_causes = dsPCFFList.Tables[0].Rows[i]["potential_causes"].ToString(),
                                        impact = objObjectivesModels.GetObjectivePotentialImpactById(dsPCFFList.Tables[0].Rows[i]["impact"].ToString()),
                                        mitigation_measure = dsPCFFList.Tables[0].Rows[i]["mitigation_measure"].ToString(),
                                        potential_status = objObjectivesModels.GetObjectivePotentialStatusById(dsPCFFList.Tables[0].Rows[i]["potential_status"].ToString()),
                                        Pcff_Notify = objGlobaldata.GetMultiHrEmpNameById(dsPCFFList.Tables[0].Rows[i]["Pcff_Notify"].ToString())
                                    };
                                    DateTime dtDocDate;
                                    if (dsPCFFList.Tables[0].Rows[i]["targeted_on"].ToString() != ""
                                       && DateTime.TryParse(dsPCFFList.Tables[0].Rows[i]["targeted_on"].ToString(), out dtDocDate))
                                    {
                                        objplanModels.targeted_on = dtDocDate;
                                    }
                                    if (dsPCFFList.Tables[0].Rows[i]["updated_on"].ToString() != ""
                                      && DateTime.TryParse(dsPCFFList.Tables[0].Rows[i]["updated_on"].ToString(), out dtDocDate))
                                    {
                                        objplanModels.updated_on = dtDocDate;
                                    }
                                    objPCFFList.ObjectivesMList.Add(objplanModels);
                                    ViewBag.ObjectPCFF = objPCFFList;
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in ObjectivesDetails: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }


                        //Action Plan
                        ObjectivesModelsList objList = new ObjectivesModelsList();
                        objList.ObjectivesMList = new List<ObjectivesModels>();

                        string sSqlstmtplan = "select id_objective_action, ObjectivesTrans_Id,actionplan,begin_date,end_date,upload,resource,resp_person,action_ref_no,action_status,reason_notcomplete,status_updated_on " +
                            "from t_objectives_actionplan where ObjectivesTrans_Id  = '" + sObjectivesTrans_Id + "' and active=1";
                        
                        DataSet dsplanList = objGlobaldata.Getdetails(sSqlstmtplan);
                        if (dsplanList.Tables.Count > 0 && dsplanList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsplanList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    ObjectivesModels objplanModels = new ObjectivesModels
                                    {
                                        id_objective_action = (dsplanList.Tables[0].Rows[i]["id_objective_action"].ToString()),
                                        ObjectivesTrans_Id = Convert.ToInt16(dsplanList.Tables[0].Rows[i]["ObjectivesTrans_Id"].ToString()),
                                        actionplan = dsplanList.Tables[0].Rows[i]["actionplan"].ToString(),
                                        upload = dsplanList.Tables[0].Rows[i]["upload"].ToString(),
                                        resource = dsplanList.Tables[0].Rows[i]["resource"].ToString(),
                                        resp_person = objGlobaldata.GetMultiHrEmpNameById(dsplanList.Tables[0].Rows[i]["resp_person"].ToString()),
                                        action_ref_no = dsplanList.Tables[0].Rows[i]["action_ref_no"].ToString(),

                                        action_status = objGlobaldata.GetDropdownitemById(dsplanList.Tables[0].Rows[i]["action_status"].ToString()),
                                        reason_notcomplete = (dsplanList.Tables[0].Rows[i]["reason_notcomplete"].ToString()),

                                    };
                                    DateTime dtDocDate;
                                    if (dsplanList.Tables[0].Rows[i]["begin_date"].ToString() != ""
                                       && DateTime.TryParse(dsplanList.Tables[0].Rows[i]["begin_date"].ToString(), out dtDocDate))
                                    {
                                        objplanModels.begin_date = dtDocDate;
                                    }
                                    if (dsplanList.Tables[0].Rows[i]["end_date"].ToString() != ""
                                      && DateTime.TryParse(dsplanList.Tables[0].Rows[i]["end_date"].ToString(), out dtDocDate))
                                    {
                                        objplanModels.end_date = dtDocDate;
                                    }
                                    if (dsplanList.Tables[0].Rows[i]["status_updated_on"].ToString() != ""
                                      && DateTime.TryParse(dsplanList.Tables[0].Rows[i]["status_updated_on"].ToString(), out dtDocDate))
                                    {
                                        objplanModels.status_updated_on = dtDocDate;
                                    }
                                    
                                    objList.ObjectivesMList.Add(objplanModels);
                                    ViewBag.ObjectPlan = objList;
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in ObjectivesDetails: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }

                        //Evaluation

                        ObjectivesModelsList objEvalList = new ObjectivesModelsList();
                        objEvalList.ObjectivesMList = new List<ObjectivesModels>();

                        string sSqlstmtEval = "select ObjectivesEval_Id, ObjectivesTrans_Id, Obj_Eval_On, FromPeriod, ToPeriod, Obj_Achieved_Val, Trend, NCR_Ref, Evidence,Status_Obj_Eval," +
                              " Source_data,Method_eval,Remark,Notified_to from t_objectives_evaluation where ObjectivesTrans_Id = '" + sObjectivesTrans_Id + "'";
                       
                        DataSet dsEvalList = objGlobaldata.Getdetails(sSqlstmtEval);

                        if (dsEvalList.Tables.Count > 0 && dsEvalList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsEvalList.Tables[0].Rows.Count; i++)
                            {

                                DateTime dtFromPeriod = Convert.ToDateTime(dsEvalList.Tables[0].Rows[i]["FromPeriod"].ToString());
                                DateTime dtToPeriod = Convert.ToDateTime(dsEvalList.Tables[0].Rows[i]["ToPeriod"].ToString());

                                try
                                {
                                    ObjectivesModels objEvalModels = new ObjectivesModels
                                    {
                                        ObjectivesEval_Id = Convert.ToInt16(dsEvalList.Tables[0].Rows[i]["ObjectivesEval_Id"].ToString()),
                                        ObjectivesTrans_Id = Convert.ToInt16(dsEvalList.Tables[0].Rows[i]["ObjectivesTrans_Id"].ToString()),
                                        FromPeriod = dtFromPeriod,
                                        ToPeriod = dtToPeriod,
                                        Obj_Achieved_Val = dsEvalList.Tables[0].Rows[i]["Obj_Achieved_Val"].ToString(),
                                        Trend = objObjectivesModels.GetMultiObjectiveEvaluationById(dsEvalList.Tables[0].Rows[i]["Trend"].ToString()),
                                        NCR_Ref = objGlobaldata.GetNCRNumberById(dsEvalList.Tables[0].Rows[i]["NCR_Ref"].ToString()),
                                        Evidence = dsEvalList.Tables[0].Rows[i]["Evidence"].ToString(),
                                        Status_Obj_Eval = objObjectivesModels.GetObjectiveStatusNameById(dsEvalList.Tables[0].Rows[i]["Status_Obj_Eval"].ToString()),
                                        Source_data = dsEvalList.Tables[0].Rows[i]["Source_data"].ToString(),
                                        Method_eval = dsEvalList.Tables[0].Rows[i]["Method_eval"].ToString(),
                                        Remark = dsEvalList.Tables[0].Rows[i]["Remark"].ToString(),
                                        Notified_to = dsEvalList.Tables[0].Rows[i]["Notified_to"].ToString(),
                                    };

                                    if (dsEvalList.Tables[0].Rows[i]["Obj_Eval_On"].ToString() != "")
                                    {
                                        objEvalModels.Obj_Eval_On = Convert.ToDateTime(dsEvalList.Tables[0].Rows[i]["Obj_Eval_On"].ToString());
                                    }
                                    objEvalList.ObjectivesMList.Add(objEvalModels);
                                    ViewBag.EvalList = objEvalList;
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationList: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            if (sObjectivesTrans_Id != "")
                            {
                                DataSet dsObjectives_Id = objGlobaldata.Getdetails("select Objectives_Id from t_objectives_trans where ObjectivesTrans_Id='" + sObjectivesTrans_Id + "'");
                                if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                                {
                                    ViewBag.Objectives_Id = dsObjectives_Id.Tables[0].Rows[0]["Objectives_Id"].ToString();
                                }
                            }
                            ViewBag.Visible = "Not visible";
                        }

                        return View(objObjectivesModels);
                    } 
                }
                else
                {
                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectivesDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
        }

        [AllowAnonymous]
        public ActionResult ObjectivesInfo(int id)
        {
            ObjectivesModels objObjectivesModels = new ObjectivesModels();

            ViewBag.View = Request.QueryString["View"];
            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                
                if (id > 0)
                {
                    if (ViewBag.View == "1")
                    {
                        ViewBag.IsostdName = "ISO 9001:2015";
                    }
                    if (ViewBag.View == "2")
                    {
                        ViewBag.IsostdName = "ISO 45001:2018";
                    }
                    if (ViewBag.View == "3")
                    {
                        ViewBag.IsostdName = "ISO 14001:2015";
                    }

                    string sSqlstmt = "select a.Objectives_Id,ObjectivesTrans_Id, Obj_Ref, Dept, Audit_Criteria, Estld_by,"
                    + "CreatedBy,DocUploadPath,objective_level,branch,Location,Obj_Estld_On,Objectives_val,Obj_Target,Base_Line_Value," +
                    "Monitoring_Mechanism,Target_Date,Action_Plan,Freq_of_Eval,Accepted_Value,Risk_ifObjFails,baseline_data,unit,obj_inline,Approved_By,Approved_Status from t_objectives a, t_objectives_trans b"
                    + " where Active=1 and trans_active=1 and a.Objectives_Id=b.Objectives_Id and b.ObjectivesTrans_Id='" + id + "'";

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        {
                            objObjectivesModels = new ObjectivesModels
                            {
                                Objectives_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["Objectives_Id"].ToString()),
                                ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["ObjectivesTrans_Id"].ToString()),
                                Obj_Ref = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Ref"].ToString(),
                                Dept = objGlobaldata.GetMultiDeptNameById(dsObjectivesModelsList.Tables[0].Rows[0]["Dept"].ToString()),
                                Audit_Criteria = objGlobaldata.GetIsoStdDescriptionById(dsObjectivesModelsList.Tables[0].Rows[0]["Audit_Criteria"].ToString()),
                                Estld_by = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[0]["Estld_by"].ToString()),
                                CreatedBy = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[0]["CreatedBy"].ToString()),
                                DocUploadPath = dsObjectivesModelsList.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                                objective_level = objGlobaldata.GetDropdownitemById(dsObjectivesModelsList.Tables[0].Rows[0]["objective_level"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsObjectivesModelsList.Tables[0].Rows[0]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsObjectivesModelsList.Tables[0].Rows[0]["Location"].ToString()),

                                Objectives_val = dsObjectivesModelsList.Tables[0].Rows[0]["Objectives_val"].ToString(),
                                Obj_Target = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Target"].ToString(),
                                Base_Line_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Base_Line_Value"].ToString(),
                                Monitoring_Mechanism = dsObjectivesModelsList.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString(),
                                Action_Plan = dsObjectivesModelsList.Tables[0].Rows[0]["Action_Plan"].ToString(),
                                Freq_of_Eval = objGlobaldata.GetDropdownitemById(dsObjectivesModelsList.Tables[0].Rows[0]["Freq_of_Eval"].ToString()),
                                Accepted_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Accepted_Value"].ToString(),
                                Risk_ifObjFails = dsObjectivesModelsList.Tables[0].Rows[0]["Risk_ifObjFails"].ToString(),
                                baseline_data = dsObjectivesModelsList.Tables[0].Rows[0]["baseline_data"].ToString(),
                                unit = objObjectivesModels.GetObjectiveUnitById(dsObjectivesModelsList.Tables[0].Rows[0]["unit"].ToString()),
                                obj_inline = objObjectivesModels.GetObjectiveInlineById(dsObjectivesModelsList.Tables[0].Rows[0]["obj_inline"].ToString()),
                                Approved_By = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[0]["Approved_By"].ToString()),
                                //Approved_Status = dsObjectivesModelsList.Tables[0].Rows[i]["Approved_Status"].ToString(),
                                Approved_Status_id = dsObjectivesModelsList.Tables[0].Rows[0]["Approved_Status"].ToString(),
                            };

                            DateTime dtDocDate = new DateTime();

                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Estld_On"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.Obj_Estld_On = dtDocDate;
                            }
                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["Target_Date"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.Target_Date = dtDocDate;
                            }
                        }                 
                    }
                    return View(objObjectivesModels);
                }
                else
                {
                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectivesInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
        }

        [AllowAnonymous]
        public ActionResult ObjectivesPDF(FormCollection form)
        {
            ObjectivesModels objObjectivesModels = new ObjectivesModels();

            ViewBag.View = Request.QueryString["View"];
            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();

                string sObjectives_Id = form["Objectives_Id"];
                string sObjectivesTrans_Id = form["ObjectivesTrans_Id"];

                if (sObjectives_Id != null)
                {
                    if (ViewBag.View == "1")
                    {
                        ViewBag.IsostdName = "ISO 9001:2015";
                    }
                    if (ViewBag.View == "2")
                    {
                        ViewBag.IsostdName = "ISO 45001:2018";
                    }
                    if (ViewBag.View == "3")
                    {
                        ViewBag.IsostdName = "ISO 14001:2015";
                    }

                    string sSqlstmt = "select a.Objectives_Id,ObjectivesTrans_Id, Obj_Ref, Dept, Audit_Criteria, Estld_by,"
                    + "CreatedBy,DocUploadPath,objective_level,branch,Location,Obj_Estld_On,Objectives_val,Obj_Target,Base_Line_Value," +
                    "Monitoring_Mechanism,Target_Date,Action_Plan,Freq_of_Eval,Accepted_Value,Risk_ifObjFails,baseline_data,unit,obj_inline,Approved_By,Approved_Status,ApprovedDate from t_objectives a, t_objectives_trans b"
                    + " where Active=1 and trans_active=1 and a.Objectives_Id=b.Objectives_Id and b.ObjectivesTrans_Id='" + sObjectivesTrans_Id + "'";

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        {
                            objObjectivesModels = new ObjectivesModels
                            {
                                Objectives_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["Objectives_Id"].ToString()),
                                ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["ObjectivesTrans_Id"].ToString()),
                                Obj_Ref = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Ref"].ToString(),
                                Dept = objGlobaldata.GetMultiDeptNameById(dsObjectivesModelsList.Tables[0].Rows[0]["Dept"].ToString()),
                                Audit_Criteria = objGlobaldata.GetIsoStdDescriptionById(dsObjectivesModelsList.Tables[0].Rows[0]["Audit_Criteria"].ToString()),
                                Estld_by = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[0]["Estld_by"].ToString()),
                                CreatedBy = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[0]["CreatedBy"].ToString()),
                                DocUploadPath = dsObjectivesModelsList.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                                objective_level = objGlobaldata.GetDropdownitemById(dsObjectivesModelsList.Tables[0].Rows[0]["objective_level"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsObjectivesModelsList.Tables[0].Rows[0]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsObjectivesModelsList.Tables[0].Rows[0]["Location"].ToString()),

                                Objectives_val = dsObjectivesModelsList.Tables[0].Rows[0]["Objectives_val"].ToString(),
                                Obj_Target = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Target"].ToString(),
                                Base_Line_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Base_Line_Value"].ToString(),
                                Monitoring_Mechanism = dsObjectivesModelsList.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString(),
                                Action_Plan = dsObjectivesModelsList.Tables[0].Rows[0]["Action_Plan"].ToString(),
                                Freq_of_Eval = objGlobaldata.GetDropdownitemById(dsObjectivesModelsList.Tables[0].Rows[0]["Freq_of_Eval"].ToString()),
                                Accepted_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Accepted_Value"].ToString(),
                                Risk_ifObjFails = dsObjectivesModelsList.Tables[0].Rows[0]["Risk_ifObjFails"].ToString(),
                                baseline_data = dsObjectivesModelsList.Tables[0].Rows[0]["baseline_data"].ToString(),
                                unit = objObjectivesModels.GetObjectiveUnitById(dsObjectivesModelsList.Tables[0].Rows[0]["unit"].ToString()),
                                obj_inline = objObjectivesModels.GetObjectiveInlineById(dsObjectivesModelsList.Tables[0].Rows[0]["obj_inline"].ToString()),
                                Approved_By = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[0]["Approved_By"].ToString()),
                                //Approved_Status = dsObjectivesModelsList.Tables[0].Rows[i]["Approved_Status"].ToString(),
                                Approved_Status_id = dsObjectivesModelsList.Tables[0].Rows[0]["Approved_Status"].ToString(),
                            };

                            DateTime dtDocDate = new DateTime();

                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Estld_On"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.Obj_Estld_On = dtDocDate;
                            }
                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["Target_Date"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.Target_Date = dtDocDate;
                            }
                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["ApprovedDate"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.ApprovedDate = dtDocDate;
                            }
                        }

                        ViewBag.ObjectivesTrans_Id = sObjectivesTrans_Id;

                        //PCFF
                        ObjectivesModelsList objPCFFList = new ObjectivesModelsList();
                        objPCFFList.ObjectivesMList = new List<ObjectivesModels>();

                        string sSqlstmtPCFF = "select id_potential, ObjectivesTrans_Id,potential_causes,impact,mitigation_measure,targeted_on,updated_on,potential_status,logged_by,Pcff_Notify " +
                            "from t_objectives_potential where ObjectivesTrans_Id  = '" + sObjectivesTrans_Id + "' and pot_active=1";
                        DataSet dsPCFFList = objGlobaldata.Getdetails(sSqlstmtPCFF);
                        if (dsPCFFList.Tables.Count > 0 && dsPCFFList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsPCFFList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    ObjectivesModels objplanModels = new ObjectivesModels
                                    {
                                        id_potential = (dsPCFFList.Tables[0].Rows[i]["id_potential"].ToString()),
                                        ObjectivesTrans_Id = Convert.ToInt16(dsPCFFList.Tables[0].Rows[i]["ObjectivesTrans_Id"].ToString()),
                                        potential_causes = dsPCFFList.Tables[0].Rows[i]["potential_causes"].ToString(),
                                        impact = objObjectivesModels.GetObjectivePotentialImpactById(dsPCFFList.Tables[0].Rows[i]["impact"].ToString()),
                                        mitigation_measure = dsPCFFList.Tables[0].Rows[i]["mitigation_measure"].ToString(),
                                        potential_status = objObjectivesModels.GetObjectivePotentialStatusById(dsPCFFList.Tables[0].Rows[i]["potential_status"].ToString()),
                                        Pcff_Notify = objGlobaldata.GetMultiHrEmpNameById(dsPCFFList.Tables[0].Rows[i]["Pcff_Notify"].ToString())
                                    };
                                    DateTime dtDocDate;
                                    if (dsPCFFList.Tables[0].Rows[i]["targeted_on"].ToString() != ""
                                       && DateTime.TryParse(dsPCFFList.Tables[0].Rows[i]["targeted_on"].ToString(), out dtDocDate))
                                    {
                                        objplanModels.targeted_on = dtDocDate;
                                    }
                                    if (dsPCFFList.Tables[0].Rows[i]["updated_on"].ToString() != ""
                                      && DateTime.TryParse(dsPCFFList.Tables[0].Rows[i]["updated_on"].ToString(), out dtDocDate))
                                    {
                                        objplanModels.updated_on = dtDocDate;
                                    }
                                    objPCFFList.ObjectivesMList.Add(objplanModels);
                                    ViewBag.ObjectPCFF = objPCFFList;
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in ObjectivesDetails: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }


                        //Action Plan

                        ObjectivesModelsList objList = new ObjectivesModelsList();
                        objList.ObjectivesMList = new List<ObjectivesModels>();

                        string sSqlstmtplan = "select id_objective_action, ObjectivesTrans_Id,actionplan,begin_date,end_date,upload,resource,resp_person,action_ref_no,action_status,reason_notcomplete,status_updated_on " +
                            "from t_objectives_actionplan where ObjectivesTrans_Id  = '" + sObjectivesTrans_Id + "' and active=1";
                        DataSet dsplanList = objGlobaldata.Getdetails(sSqlstmtplan);
                        if (dsplanList.Tables.Count > 0 && dsplanList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsplanList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    ObjectivesModels objplanModels = new ObjectivesModels
                                    {
                                        id_objective_action = (dsplanList.Tables[0].Rows[i]["id_objective_action"].ToString()),
                                        ObjectivesTrans_Id = Convert.ToInt16(dsplanList.Tables[0].Rows[i]["ObjectivesTrans_Id"].ToString()),
                                        actionplan = dsplanList.Tables[0].Rows[i]["actionplan"].ToString(),
                                        upload = dsplanList.Tables[0].Rows[i]["upload"].ToString(),
                                        resource = dsplanList.Tables[0].Rows[i]["resource"].ToString(),
                                        resp_person = objGlobaldata.GetMultiHrEmpNameById(dsplanList.Tables[0].Rows[i]["resp_person"].ToString()),
                                        action_ref_no = dsplanList.Tables[0].Rows[i]["action_ref_no"].ToString(),

                                        action_status = objGlobaldata.GetDropdownitemById(dsplanList.Tables[0].Rows[i]["action_status"].ToString()),
                                        reason_notcomplete = dsplanList.Tables[0].Rows[i]["reason_notcomplete"].ToString(),

                                    };
                                    DateTime dtDocDate;
                                    if (dsplanList.Tables[0].Rows[i]["begin_date"].ToString() != ""
                                       && DateTime.TryParse(dsplanList.Tables[0].Rows[i]["begin_date"].ToString(), out dtDocDate))
                                    {
                                        objplanModels.begin_date = dtDocDate;
                                    }
                                    if (dsplanList.Tables[0].Rows[i]["end_date"].ToString() != ""
                                      && DateTime.TryParse(dsplanList.Tables[0].Rows[i]["end_date"].ToString(), out dtDocDate))
                                    {
                                        objplanModels.end_date = dtDocDate;
                                    }

                                    if (dsplanList.Tables[0].Rows[i]["status_updated_on"].ToString() != ""
                                     && DateTime.TryParse(dsplanList.Tables[0].Rows[i]["status_updated_on"].ToString(), out dtDocDate))
                                    {
                                        objplanModels.status_updated_on = dtDocDate;
                                    }
                                    objList.ObjectivesMList.Add(objplanModels);
                                    ViewBag.ObjectPlan = objList;
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in ObjectivesPDF: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }

                        //Evaluation

                        ObjectivesModelsList objEvalList = new ObjectivesModelsList();
                        objEvalList.ObjectivesMList = new List<ObjectivesModels>();

                        string sSqlstmtEval = "select ObjectivesEval_Id, ObjectivesTrans_Id, Obj_Eval_On, FromPeriod, ToPeriod, Obj_Achieved_Val, Trend, NCR_Ref, Evidence,Status_Obj_Eval," +
                              " Source_data,Method_eval,Remark,Notified_to from t_objectives_evaluation where ObjectivesTrans_Id = '" + sObjectivesTrans_Id + "'";
                        DataSet dsEvalList = objGlobaldata.Getdetails(sSqlstmtEval);

                        if (dsEvalList.Tables.Count > 0 && dsEvalList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsEvalList.Tables[0].Rows.Count; i++)
                            {

                                DateTime dtFromPeriod = Convert.ToDateTime(dsEvalList.Tables[0].Rows[i]["FromPeriod"].ToString());
                                DateTime dtToPeriod = Convert.ToDateTime(dsEvalList.Tables[0].Rows[i]["ToPeriod"].ToString());

                                try
                                {
                                    ObjectivesModels objEvalModels = new ObjectivesModels
                                    {
                                        ObjectivesEval_Id = Convert.ToInt16(dsEvalList.Tables[0].Rows[i]["ObjectivesEval_Id"].ToString()),
                                        ObjectivesTrans_Id = Convert.ToInt16(dsEvalList.Tables[0].Rows[i]["ObjectivesTrans_Id"].ToString()),
                                        FromPeriod = dtFromPeriod,
                                        ToPeriod = dtToPeriod,
                                        Obj_Achieved_Val = dsEvalList.Tables[0].Rows[i]["Obj_Achieved_Val"].ToString(),
                                        Trend = objObjectivesModels.GetMultiObjectiveEvaluationById(dsEvalList.Tables[0].Rows[i]["Trend"].ToString()),
                                        NCR_Ref = objGlobaldata.GetNCRNumberById(dsEvalList.Tables[0].Rows[i]["NCR_Ref"].ToString()),
                                        Evidence = dsEvalList.Tables[0].Rows[i]["Evidence"].ToString(),
                                        Status_Obj_Eval = objObjectivesModels.GetObjectiveStatusNameById(dsEvalList.Tables[0].Rows[i]["Status_Obj_Eval"].ToString()),
                                        Source_data = dsEvalList.Tables[0].Rows[i]["Source_data"].ToString(),
                                        Method_eval = dsEvalList.Tables[0].Rows[i]["Method_eval"].ToString(),
                                        Remark = dsEvalList.Tables[0].Rows[i]["Remark"].ToString(),
                                        Notified_to = dsEvalList.Tables[0].Rows[i]["Notified_to"].ToString(),
                                    };

                                    if (dsEvalList.Tables[0].Rows[i]["Obj_Eval_On"].ToString() != "")
                                    {
                                        objEvalModels.Obj_Eval_On = Convert.ToDateTime(dsEvalList.Tables[0].Rows[i]["Obj_Eval_On"].ToString());
                                    }
                                    objEvalList.ObjectivesMList.Add(objEvalModels);
                                    ViewBag.EvalList = objEvalList;

                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in ObjectivesPDF: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            if (sObjectivesTrans_Id != "")
                            {
                                DataSet dsObjectives_Id = objGlobaldata.Getdetails("select Objectives_Id from t_objectives_trans where ObjectivesTrans_Id='" + sObjectivesTrans_Id + "'");
                                if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                                {
                                    ViewBag.Objectives_Id = dsObjectives_Id.Tables[0].Rows[0]["Objectives_Id"].ToString();
                                }
                            }                           
                        }                        
                    }


                    ViewBag.ObjectivesDetails = objObjectivesModels;

                    CompanyModels objCompany = new CompanyModels();
                    dsObjectivesModelsList = objCompany.GetCompanyDetailsForReport(dsObjectivesModelsList);
                    dsObjectivesModelsList = objGlobaldata.GetReportDetails(dsObjectivesModelsList, objObjectivesModels.Obj_Ref, objGlobaldata.GetCurrentUserSession().empid, "OBJECTIVE REPORT");

                    ViewBag.CompanyInfo = dsObjectivesModelsList;

                }
                else
                {
                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectivesPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("ObjectivesPDF")
            {
                //FileName = "Objectives.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [AllowAnonymous]
        public ActionResult ObjectivesEdit()
        {
            ObjectivesModels objObjectivesModels = new ObjectivesModels();

            ViewBag.View = Request.QueryString["View"];
            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();

                string sObjectives_Id = Request.QueryString["Objectives_Id"];
                string sObjectivesTrans_Id = Request.QueryString["ObjectivesTrans_Id"];

                if (sObjectives_Id != null)
                {
                    if (ViewBag.View == "1")
                    {
                        ViewBag.IsostdName = "ISO 9001:2015";
                        ViewBag.SubMenutype = "Objectives1";
                    }
                    if (ViewBag.View == "2")
                    {
                        ViewBag.IsostdName = "ISO 45001:2018";
                        ViewBag.SubMenutype = "Objectives2";
                    }
                    if (ViewBag.View == "3")
                    {
                        ViewBag.IsostdName = "ISO 14001:2015";
                        ViewBag.SubMenutype = "Objectives3";
                    }

                                       
                    string sSqlstmt = "select a.Objectives_Id,ObjectivesTrans_Id, Obj_Ref, Dept, Audit_Criteria, Estld_by,"
              + "CreatedBy,DocUploadPath,objective_level,branch,Location,Obj_Estld_On,Objectives_val,Obj_Target,Base_Line_Value," +
              "Monitoring_Mechanism,Target_Date,Action_Plan,Freq_of_Eval,Accepted_Value,Risk_ifObjFails,baseline_data,unit,obj_inline,Approved_By,Approved_Status,branch_view from t_objectives a, t_objectives_trans b"
              + " where Active=1 and a.Objectives_Id=b.Objectives_Id and b.ObjectivesTrans_Id='" + sObjectivesTrans_Id + "'";

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        //if (objUser.empid == dsObjectivesModelsList.Tables[0].Rows[0]["CreatedBy"].ToString()
                        //    && dsObjectivesModelsList.Tables[0].Rows[0]["Approved_Status"].ToString() == "0")
                        {
                            objObjectivesModels = new ObjectivesModels
                            {
                                Objectives_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["Objectives_Id"].ToString()),
                                ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["ObjectivesTrans_Id"].ToString()),
                                Obj_Ref = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Ref"].ToString(),
                                Dept = /*objGlobaldata.GetMultiDeptNameById*/(dsObjectivesModelsList.Tables[0].Rows[0]["Dept"].ToString()),
                                //Dept_Head = objGlobaldata.GetDeptHeadNameByDepartment(dsObjectivesModelsList.Tables[0].Rows[0]["Dept"].ToString()),
                                //Freq_of_Eval = (dsObjectivesModelsList.Tables[0].Rows[i]["Freq_of_Eval"].ToString()),
                                // Personal_Responsible = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[i]["Personal_Responsible"].ToString()),
                                //Personal_Responsible_EditCheck = objGlobaldata.GetMultiEmpNameByIdEdit(dsObjectivesModelsList.Tables[0].Rows[i]["Personal_Responsible"].ToString()),
                                //Personal_Responsible_EditCheck2=objGlobaldata.GetCurrentUserSession().empid,
                                Audit_Criteria = objGlobaldata.GetIsoStdDescriptionById(dsObjectivesModelsList.Tables[0].Rows[0]["Audit_Criteria"].ToString()),
                                Estld_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsObjectivesModelsList.Tables[0].Rows[0]["Estld_by"].ToString()),
                                CreatedBy = /*objGlobaldata.GetEmpHrNameById*/(dsObjectivesModelsList.Tables[0].Rows[0]["CreatedBy"].ToString()),
                                DocUploadPath = dsObjectivesModelsList.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                                // EstYear = dsObjectivesModelsList.Tables[0].Rows[i]["EstYear"].ToString(),
                                objective_level = /*objGlobaldata.GetDropdownitemById*/(dsObjectivesModelsList.Tables[0].Rows[0]["objective_level"].ToString()),
                                branch = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsObjectivesModelsList.Tables[0].Rows[0]["branch"].ToString()),
                                Location = /*objGlobaldata.GetDivisionLocationById*/(dsObjectivesModelsList.Tables[0].Rows[0]["Location"].ToString()),

                                Objectives_val = dsObjectivesModelsList.Tables[0].Rows[0]["Objectives_val"].ToString(),
                                Obj_Target = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Target"].ToString(),
                                Base_Line_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Base_Line_Value"].ToString(),
                                Monitoring_Mechanism = dsObjectivesModelsList.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString(),
                                Action_Plan = dsObjectivesModelsList.Tables[0].Rows[0]["Action_Plan"].ToString(),
                                Freq_of_Eval = objGlobaldata.GetDropdownitemById(dsObjectivesModelsList.Tables[0].Rows[0]["Freq_of_Eval"].ToString()),
                                Accepted_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Accepted_Value"].ToString(),
                                Risk_ifObjFails = dsObjectivesModelsList.Tables[0].Rows[0]["Risk_ifObjFails"].ToString(),
                                baseline_data = dsObjectivesModelsList.Tables[0].Rows[0]["baseline_data"].ToString(),
                                unit = /*objObjectivesModels.GetObjectiveUnitById*/(dsObjectivesModelsList.Tables[0].Rows[0]["unit"].ToString()),
                                obj_inline = /*objObjectivesModels.GetObjectiveInlineById*/(dsObjectivesModelsList.Tables[0].Rows[0]["obj_inline"].ToString()),
                                Approved_By = /*objGlobaldata.GetEmpHrNameById*/(dsObjectivesModelsList.Tables[0].Rows[0]["Approved_By"].ToString()),
                                //Approved_Status = dsObjectivesModelsList.Tables[0].Rows[i]["Approved_Status"].ToString(),
                                Approved_Status_id = dsObjectivesModelsList.Tables[0].Rows[0]["Approved_Status"].ToString(),
                                branch_view = dsObjectivesModelsList.Tables[0].Rows[0]["branch_view"].ToString(),
                            };

                            DateTime dtDocDate = new DateTime();

                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Estld_On"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.Obj_Estld_On = dtDocDate;
                            }
                            if (DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["Target_Date"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.Target_Date = dtDocDate;
                            }

                            if (dsObjectivesModelsList.Tables[0].Rows[0]["Estld_by"].ToString() != "")
                            {
                                ViewBag.Estld_byArray = (dsObjectivesModelsList.Tables[0].Rows[0]["Estld_by"].ToString()).Split(',');
                            }
                        }
                             
                            ViewBag.Approver = objGlobaldata.GetApprover();
                            //ViewBag.Approver = objGlobaldata.GetGRoleList(dsObjectivesModelsList.Tables[0].Rows[0]["branch"].ToString(), dsObjectivesModelsList.Tables[0].Rows[0]["Dept"].ToString(), dsObjectivesModelsList.Tables[0].Rows[0]["Location"].ToString(), "Approver");
                            ViewBag.userbranch = objGlobaldata.GetCurrentUserSession().division;
                            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                            ViewBag.Freq_of_Eval = objGlobaldata.GetDropdownList("Objective Frequency Evaluation");
                            ViewBag.Status_Obj_Eval = objObjectivesModels.GetMultiObjectiveStatusList();
                            ViewBag.RespPerson = objGlobaldata.GetHrEmployeeListbox();
                            ViewBag.EmpLists = objGlobaldata.GetDeptHeadList();
                            ViewBag.Objlevel = objGlobaldata.GetDropdownList("Objective Level");
                            ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                            ViewBag.ObjectInlineWith = objObjectivesModels.GetObjectiveInlineList();
                            ViewBag.Unit = objObjectivesModels.GetObjectiveUnitList();
                            ViewBag.Location = objGlobaldata.GetLocationListBox();

                            return View(objObjectivesModels);
                        }

                    }
                    else
                    {
                        ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
                    }  
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectivesEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ObjectivesEdit(ObjectivesModels objModel, FormCollection form, HttpPostedFileBase Action_Plan)
        {
            string sView = form["view"];
            try
            {
                objModel.Dept = form["Dept"];
                //objModel.Estld_by = form["Estld_by"];
                objModel.branch = form["branch"];
                objModel.Location = form["Location"];
                objModel.Objectives_val = form["Objectives_val"];
                objModel.Obj_Target = form["Obj_Target"];
                objModel.Freq_of_Eval = form["Freq_of_Eval"];
                objModel.Base_Line_Value = form["Base_Line_Value"];
                objModel.Monitoring_Mechanism = form["Monitoring_Mechanism"];
                objModel.baseline_data = form["baseline_data"];
                objModel.unit = form["unit"];
                objModel.obj_inline = form["obj_inline"];
                objModel.branch_view = form["branch_view"];

                if (objModel.branch != null) 
                {
                    if (objModel.branch_view.Contains(","))
                    { 
                        if( ! objModel.branch_view.ToString().Split(',').Contains(objModel.branch))
                        {
                            objModel.branch_view = objModel.branch_view + "," + objModel.branch;
                        }                        
                    }
                    else
                    {
                        if ( ! objModel.branch_view.ToString().Contains(objModel.branch))
                        {
                            objModel.branch_view = objModel.branch_view + "," + objModel.branch;
                        }
                    }
                    
                }

                //Estld_by
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.Estld_by = form["nempno " + i] + "," + objModel.Estld_by;
                    }
                }
                if (objModel.Estld_by != null)
                {
                    objModel.Estld_by = objModel.Estld_by.Trim(',');
                }

                DateTime dateValue;

                if (DateTime.TryParse(form["Obj_Estld_On"], out dateValue) == true)
                {
                    objModel.Obj_Estld_On = dateValue;
                }
                if (DateTime.TryParse(form["Target_Date"], out dateValue) == true)
                {
                    objModel.Target_Date = dateValue;
                }

                if (Action_Plan != null && Action_Plan.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Objectives"), Path.GetFileName(Action_Plan.FileName));
                        string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                        Action_Plan.SaveAs(sFilepath + "/" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                        objModel.Action_Plan = "~/DataUpload/MgmtDocs/Objectives/" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename;
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

                if (objModel.FunUpdateObjectives(objModel))
                {
                    TempData["Successdata"] = "Objectives updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectivePlanUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ObjectivesList", new { View = sView });
        }

        [AllowAnonymous]
        public JsonResult ObjectiveDocDelete(string ObjectivesTrans_Id)
        {
            try
            {
                if (ObjectivesTrans_Id != null && ObjectivesTrans_Id != "")
                {
                    ObjectivesModels Doc = new ObjectivesModels();

                    if (Doc.FunDeleteObjectiveDoc(ObjectivesTrans_Id))
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
                    TempData["alertdata"] = "Objective Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectiveDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }


        // ----------------Objective Evaluation------
        [AllowAnonymous]
        public ActionResult AddObjectiveEvaluation()
{
    ObjectivesModelsList objObjectivesModelsList = new ObjectivesModelsList();
    objObjectivesModelsList.ObjectivesMList = new List<ObjectivesModels>();
    ObjectivesModels objObjectivesModels = new ObjectivesModels();
    UserCredentials objUser = new UserCredentials();
    objUser = objGlobaldata.GetCurrentUserSession();
    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
    ViewBag.EvalStatus = objObjectivesModels.GetMultiObjectiveStatusList();
    ViewBag.Evaluation = objObjectivesModels.GetMultiObjectiveEvaluation();
    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

    try
    {
        ViewBag.View = Request.QueryString["View"];
        string sObjectivesTrans_Id = Request.QueryString["ObjectivesTrans_Id"];
        if (sObjectivesTrans_Id != null)
        {

            //string sSqlstmt = "select b.Obj_Ref,c.Objectives_Id,Obj_Estld_On, ObjectivesEval_Id, a.ObjectivesTrans_Id, Obj_Eval_On, FromPeriod, ToPeriod, Obj_Achieved_Val, Trend, NCR_Ref, Evidence,Status_Obj_Eval," +
            //    " Obj_Target,Base_Line_Value,Monitoring_Mechanism,Target_Date,Objectives_val,Source_data,Method_eval,Remark,Notified_to from t_objectives_evaluation a, t_objectives_trans b,t_objectives c where a.ObjectivesTrans_Id = b.ObjectivesTrans_Id " +
            //    "and b.ObjectivesTrans_Id  = '" + sObjectivesTrans_Id + "' and c.Objectives_Id=b.Objectives_Id ";
             string sSqlstmt = "select ObjectivesEval_Id, ObjectivesTrans_Id, Obj_Eval_On, FromPeriod, ToPeriod, Obj_Achieved_Val, Trend, NCR_Ref, Evidence,Status_Obj_Eval," +
                                " Source_data,Method_eval,Remark,Notified_to from t_objectives_evaluation where ObjectivesTrans_Id = '" + sObjectivesTrans_Id + "'" +
                                " order by FromPeriod asc";

            ViewBag.ObjectivesTrans_Id = sObjectivesTrans_Id;

            DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

            if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
            {               
                for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                {
                            
                    DateTime dtFromPeriod = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[i]["FromPeriod"].ToString());
                    DateTime dtToPeriod = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[i]["ToPeriod"].ToString());

                    try
                    {
                        objObjectivesModels = new ObjectivesModels
                        {
                            ObjectivesEval_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["ObjectivesEval_Id"].ToString()),
                            ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["ObjectivesTrans_Id"].ToString()),                            
                            FromPeriod = dtFromPeriod,
                            ToPeriod = dtToPeriod,
                            Obj_Achieved_Val = dsObjectivesModelsList.Tables[0].Rows[i]["Obj_Achieved_Val"].ToString(),
                            Trend = objObjectivesModels.GetMultiObjectiveEvaluationById(dsObjectivesModelsList.Tables[0].Rows[i]["Trend"].ToString()),
                            NCR_Ref = objGlobaldata.GetNCRNumberById(dsObjectivesModelsList.Tables[0].Rows[i]["NCR_Ref"].ToString()),
                            Evidence = dsObjectivesModelsList.Tables[0].Rows[i]["Evidence"].ToString(),
                            Status_Obj_Eval = objObjectivesModels.GetObjectiveStatusNameById(dsObjectivesModelsList.Tables[0].Rows[i]["Status_Obj_Eval"].ToString()),
                            Source_data = dsObjectivesModelsList.Tables[0].Rows[i]["Source_data"].ToString(),
                            Method_eval = dsObjectivesModelsList.Tables[0].Rows[i]["Method_eval"].ToString(),
                            Remark = dsObjectivesModelsList.Tables[0].Rows[i]["Remark"].ToString(),
                            Notified_to = dsObjectivesModelsList.Tables[0].Rows[i]["Notified_to"].ToString(),
                        };
                                
                                if (dsObjectivesModelsList.Tables[0].Rows[i]["Obj_Eval_On"].ToString() != "")
                                {
                                    objObjectivesModels.Obj_Eval_On = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[i]["Obj_Eval_On"].ToString());
                                }
                                objObjectivesModelsList.ObjectivesMList.Add(objObjectivesModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (sObjectivesTrans_Id != "")
                {
                    DataSet dsObjectives_Id = objGlobaldata.Getdetails("select Objectives_Id from t_objectives_trans where ObjectivesTrans_Id='" + sObjectivesTrans_Id + "'");
                    if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.Objectives_Id = dsObjectives_Id.Tables[0].Rows[0]["Objectives_Id"].ToString();
                    }
                }
                        ViewBag.Visible = "Not visible";
            }
            else
            {
                TempData["alertdata"] = "No data exists";
                return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
            }

                    string sObjectivesEval_Id = Request.QueryString["ObjectivesEval_Id"];
                    if (sObjectivesEval_Id != null)
                    {
                        string sSqlstmtEval = "select ObjectivesEval_Id, ObjectivesTrans_Id, Obj_Eval_On, FromPeriod, ToPeriod, Obj_Achieved_Val, Trend, NCR_Ref, Evidence,Status_Obj_Eval," +
                                        " Source_data,Method_eval,Remark,Notified_to from t_objectives_evaluation where ObjectivesEval_Id = '" + sObjectivesEval_Id + "'";

                        DataSet dsEvalList = objGlobaldata.Getdetails(sSqlstmtEval);

                        if (dsEvalList.Tables.Count > 0 && dsEvalList.Tables[0].Rows.Count > 0)
                        {
                            DateTime dtFromPeriod = Convert.ToDateTime(dsEvalList.Tables[0].Rows[0]["FromPeriod"].ToString());
                            DateTime dtToPeriod = Convert.ToDateTime(dsEvalList.Tables[0].Rows[0]["ToPeriod"].ToString());

                            ViewBag.ObjectivesEval_Id = Convert.ToInt16(dsEvalList.Tables[0].Rows[0]["ObjectivesEval_Id"].ToString());
                            ViewBag.ObjectivesTrans_Id = Convert.ToInt16(dsEvalList.Tables[0].Rows[0]["ObjectivesTrans_Id"].ToString());
                            ViewBag.FromPeriod = dtFromPeriod;
                            ViewBag.ToPeriod = dtToPeriod;
                            ViewBag.Obj_Achieved_Val = dsEvalList.Tables[0].Rows[0]["Obj_Achieved_Val"].ToString();
                            ViewBag.Trend = dsEvalList.Tables[0].Rows[0]["Trend"].ToString();
                            ViewBag.NCR_Ref = /*objGlobaldata.GetNCRNumberById*/(dsEvalList.Tables[0].Rows[0]["NCR_Ref"].ToString());
                            ViewBag.Evidence = dsEvalList.Tables[0].Rows[0]["Evidence"].ToString();
                            ViewBag.Status_Obj_Eval = dsEvalList.Tables[0].Rows[0]["Status_Obj_Eval"].ToString();
                            ViewBag.Source_data = dsEvalList.Tables[0].Rows[0]["Source_data"].ToString();
                            ViewBag.Method_eval = dsEvalList.Tables[0].Rows[0]["Method_eval"].ToString();
                            ViewBag.Remark = dsEvalList.Tables[0].Rows[0]["Remark"].ToString();
                            ViewBag.Notified_to = dsEvalList.Tables[0].Rows[0]["Notified_to"].ToString();
                            
                            if (dsEvalList.Tables[0].Rows[0]["Obj_Eval_On"].ToString() != "")
                            {
                                ViewBag.Obj_Eval_On = Convert.ToDateTime(dsEvalList.Tables[0].Rows[0]["Obj_Eval_On"].ToString());
                            }
                            if (ViewBag.Status_Obj_Eval == "")
                            {
                                ViewBag.Status_Obj_Eval = objObjectivesModels.GetObjectiveStatusIdByName("In Progress");
                            }
                            if (dsEvalList.Tables[0].Rows[0]["Notified_to"].ToString() != "")
                            {
                                ViewBag.NotifiedtoArray = (dsEvalList.Tables[0].Rows[0]["Notified_to"].ToString()).Split(',');
                            }

                            ViewBag.Visible = "Visible";
                        }
                    }
                }
        else
        {
            TempData["alertdata"] = "Objectives Trans Id cannot be null";
            return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
        }
    }
    catch (Exception ex)
    {
        objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationList: " + ex.ToString());
        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
    }
    return View(objObjectivesModelsList.ObjectivesMList.ToList());
}


[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult AddObjectiveEvaluation(ObjectivesModels objObjectivesModels, FormCollection form, HttpPostedFileBase Evidence)
{
    ViewBag.View = form["view"];
    objObjectivesModels.Notified_to = form["Notified_to"];

    try
    {
        DateTime dateValue;

        if (DateTime.TryParse(form["Obj_Eval_On"], out dateValue) == true)
        {
            objObjectivesModels.Obj_Eval_On = dateValue;
        }
        //if (DateTime.TryParse(form["FromPeriod"], out dateValue) == true)
        //{
        //    objObjectivesModels.FromPeriod = dateValue;
        //}
        //if (DateTime.TryParse(form["ToPeriod"], out dateValue) == true)
        //{
        //    objObjectivesModels.ToPeriod = dateValue;
        //}
        objObjectivesModels.ObjectivesTrans_Id = Convert.ToInt16(form["ObjectivesTrans_Id"]);


                //Notified To
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objObjectivesModels.Notified_to = form["nempno " + i] + "," + objObjectivesModels.Notified_to;
                    }
                }
                if (objObjectivesModels.Notified_to != null)
                {
                    objObjectivesModels.Notified_to = objObjectivesModels.Notified_to.Trim(',');
                }
                if (Evidence != null && Evidence.ContentLength > 0)
        {
            try
            {
                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Objectives"), Path.GetFileName(Evidence.FileName));
                string sFilename = "Evidence" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                string sFilepath = Path.GetDirectoryName(spath);

                Evidence.SaveAs(sFilepath + "/" + sFilename);
                objObjectivesModels.Evidence = "~/DataUpload/MgmtDocs/Objectives/" + sFilename;
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

        if (objObjectivesModels.FunAddObjectivesEvaluation(objObjectivesModels))
        {
            TempData["Successdata"] = "Added Objective Evaluation details successfully";
        }
        else
        {
            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        }
    }
    catch (Exception ex)
    {
        objGlobaldata.AddFunctionalLog("Exception in AddObjectiveEvaluation: " + ex.ToString());
        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
    }
    return RedirectToAction("AddObjectiveEvaluation", new { ObjectivesTrans_Id = objObjectivesModels.ObjectivesTrans_Id, View = ViewBag.View });
}

[AllowAnonymous]
public ActionResult ObjectiveEvaluationDetails()
{
    ObjectivesModels objObjectivesModels = new ObjectivesModels();
    ObjectivesModels objObjectivesModels2 = new ObjectivesModels();
    UserCredentials objUser = new UserCredentials();
    objUser = objGlobaldata.GetCurrentUserSession();

    try
    {
        string sObjectivesTrans_Id = Request.QueryString["ObjectivesTrans_Id"];

        if (sObjectivesTrans_Id != null && sObjectivesTrans_Id != "")
        {

            string sSqlstmt = "select ObjectivesEval_Id, ObjectivesTrans_Id, Obj_Eval_On, FromPeriod, ToPeriod, Obj_Achieved_Val, Trend, NCR_Ref, Evidence,Status_Obj_Eval "
                                + "from t_objectives_evaluation where ObjectivesTrans_Id=" + sObjectivesTrans_Id;

            DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

            if (dsObjectivesModelsList.Tables[0].Rows.Count > 0)
            {
                DateTime dtObj_Eval_On = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Eval_On"].ToString());
                DateTime dtFromPeriod = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["FromPeriod"].ToString());
                DateTime dtToPeriod = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["ToPeriod"].ToString());

                objObjectivesModels = new ObjectivesModels
                {
                    ObjectivesEval_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["ObjectivesEval_Id"].ToString()),
                    ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["ObjectivesTrans_Id"].ToString()),
                    Obj_Eval_On = dtObj_Eval_On,
                    FromPeriod = dtFromPeriod,
                    ToPeriod = dtToPeriod,
                    Obj_Achieved_Val = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Achieved_Val"].ToString(),
                    Trend = objObjectivesModels2.GetMultiObjectiveEvaluationById(dsObjectivesModelsList.Tables[0].Rows[0]["Trend"].ToString()),
                    NCR_Ref = objGlobaldata.GetNCRNumberById(dsObjectivesModelsList.Tables[0].Rows[0]["NCR_Ref"].ToString()),
                    Evidence = dsObjectivesModelsList.Tables[0].Rows[0]["Evidence"].ToString(),
                    Status_Obj_Eval = objObjectivesModels.GetObjectiveStatusNameById(dsObjectivesModelsList.Tables[0].Rows[0]["Status_Obj_Eval"].ToString())
                };

                DataSet dsObjectives_Id = objGlobaldata.Getdetails("select Objectives_Id from t_objectives_trans where ObjectivesTrans_Id='"
                    + objObjectivesModels.ObjectivesTrans_Id + "'");
                ViewBag.Objectives_Id = dsObjectives_Id.Tables[0].Rows[0]["Objectives_Id"].ToString();
            }
            else if (Request.QueryString["Objectives_Id"] != null)
            {
                TempData["alertdata"] = "No data exists";
                return RedirectToAction("ObjectivesDetails", new { Objectives_Id = Request.QueryString["Objectives_Id"], View = ViewBag.View });
            }
            else
            {
                return RedirectToAction("AddObjectiveEvaluation", new { ObjectivesTrans_Id = sObjectivesTrans_Id, View = ViewBag.View });
            }
        }
        else if (Request.QueryString["Objectives_Id"] != null)
        {
            TempData["alertdata"] = "No data exists";
            return RedirectToAction("ObjectivesDetails", new { Objectives_Id = Request.QueryString["Objectives_Id"], View = ViewBag.View });
        }
        else
        {
            ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
            TempData["alertdata"] = "No data exists";
            return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
        }
    }
    catch (Exception ex)
    {
        objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationDetails: " + ex.ToString());
        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
    }
    return View(objObjectivesModels);

}

[AllowAnonymous]
public ActionResult ObjectiveEvaluationEdit()
{
    ObjectivesModels objObjectivesModels = new ObjectivesModels();

    try
    {
        UserCredentials objUser = new UserCredentials();
        objUser = objGlobaldata.GetCurrentUserSession();
        ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
        ViewBag.Evaluation = objObjectivesModels.GetMultiObjectiveEvaluation();
        ViewBag.Status_Obj_Eval = objObjectivesModels.GetMultiObjectiveStatusList();
        ViewBag.NcrRef = objGlobaldata.GetNCRNumberList();
        string sObjectivesEval_Id = Request.QueryString["ObjectivesEval_Id"];
        ViewBag.View = Request.QueryString["View"];
        if (sObjectivesEval_Id != null && sObjectivesEval_Id != "")
        {

            string sSqlstmt = "select ObjectivesEval_Id, a.ObjectivesTrans_Id, Obj_Eval_On, FromPeriod, ToPeriod, Obj_Achieved_Val, Trend, NCR_Ref, Evidence,Status_Obj_Eval," +
                             " Obj_Target,Base_Line_Value,Monitoring_Mechanism,Target_Date from t_objectives_evaluation a, t_objectives_trans b where" +
                             " a.ObjectivesTrans_Id = b.ObjectivesTrans_Id and ObjectivesEval_Id = '" + sObjectivesEval_Id + "'";

            DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

            if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
            {
                DateTime dtObj_Eval_On = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Eval_On"].ToString());
                DateTime dtFromPeriod = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["FromPeriod"].ToString());
                DateTime dtToPeriod = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["ToPeriod"].ToString());
                DateTime dtTarget_Date = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["Target_Date"].ToString());

                objObjectivesModels = new ObjectivesModels
                {
                    ObjectivesEval_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["ObjectivesEval_Id"].ToString()),
                    ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["ObjectivesTrans_Id"].ToString()),
                    Obj_Eval_On = dtObj_Eval_On,
                    FromPeriod = dtFromPeriod,
                    ToPeriod = dtToPeriod,
                    Obj_Achieved_Val = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Achieved_Val"].ToString(),
                    Trend = dsObjectivesModelsList.Tables[0].Rows[0]["Trend"].ToString(),
                    NCR_Ref = /*objGlobaldata.GetNCRNumberById*/(dsObjectivesModelsList.Tables[0].Rows[0]["NCR_Ref"].ToString()),
                    Evidence = dsObjectivesModelsList.Tables[0].Rows[0]["Evidence"].ToString(),
                    Status_Obj_Eval = dsObjectivesModelsList.Tables[0].Rows[0]["Status_Obj_Eval"].ToString(),
                    Obj_Target = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Target"].ToString(),
                    Base_Line_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Base_Line_Value"].ToString(),
                    Monitoring_Mechanism = dsObjectivesModelsList.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString(),
                    Target_Date = dtTarget_Date,
                };
                ViewBag.ObjectivesTrans_Id = objObjectivesModels.ObjectivesTrans_Id;
                ViewBag.ObjectivesEval_Id = objObjectivesModels.ObjectivesEval_Id;
                DataSet dsObjectives_Id = objGlobaldata.Getdetails("select Objectives_Id,Target_Date,Obj_Estld_On from t_objectives_trans where ObjectivesTrans_Id='"
                    + objObjectivesModels.ObjectivesTrans_Id + "'");

                if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                {
                    objObjectivesModels.Target_Date = Convert.ToDateTime(dsObjectives_Id.Tables[0].Rows[0]["Target_Date"].ToString());
                    objObjectivesModels.Obj_Estld_On = Convert.ToDateTime(dsObjectives_Id.Tables[0].Rows[0]["Obj_Estld_On"].ToString());
                }
            }
            else if (Request.QueryString["Objectives_Id"] != null)
            {
                TempData["alertdata"] = "No data exists";
                return RedirectToAction("ObjectivesDetails", new { Objectives_Id = Request.QueryString["Objectives_Id"], View = ViewBag.View });
            }
            else
            {
                TempData["alertdata"] = "No data exists";
                return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
            }
        }
        else if (Request.QueryString["Objectives_Id"] != null)
        {
            TempData["alertdata"] = "No data exists";
            return RedirectToAction("ObjectivesDetails", new { Objectives_Id = Request.QueryString["Objectives_Id"], View = ViewBag.View });
        }
        else
        {
            TempData["alertdata"] = "No data exists";
            return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
        }
    }
    catch (Exception ex)
    {
        objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationEdit: " + ex.ToString());
        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
    }

    return View(objObjectivesModels);
}

[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult ObjectiveEvaluationEdit(ObjectivesModels objObjectivesModels, FormCollection form, HttpPostedFileBase Evidence)
{
    try
    {
        DateTime dateValue;

        ViewBag.View = form["view"];

        if (DateTime.TryParse(form["Obj_Eval_On"], out dateValue) == true)
        {
            objObjectivesModels.Obj_Eval_On = dateValue;
        }
        if (DateTime.TryParse(form["FromPeriod"], out dateValue) == true)
        {
            objObjectivesModels.FromPeriod = dateValue;
        }
        if (DateTime.TryParse(form["ToPeriod"], out dateValue) == true)
        {
            objObjectivesModels.ToPeriod = dateValue;
        }
        objObjectivesModels.ObjectivesEval_Id = Convert.ToInt16(form["ObjectivesEval_Id"]);
        objObjectivesModels.ObjectivesTrans_Id = Convert.ToInt16(form["ObjectivesTrans_Id"]);

        objObjectivesModels.CreatedBy = objGlobaldata.GetCurrentUserSession().empid;


        if (Evidence != null && Evidence.ContentLength > 0)
        {
            try
            {
                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Objectives"), Path.GetFileName(Evidence.FileName));
                string sFilename = "Evidence" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                string sFilepath = Path.GetDirectoryName(spath);

                Evidence.SaveAs(sFilepath + "/" + sFilename);
                objObjectivesModels.Evidence = "~/DataUpload/MgmtDocs/Objectives/" + sFilename;
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

        if (objObjectivesModels.FunUpdateObjectivesEvaluation(objObjectivesModels))
        {
            TempData["Successdata"] = "Objective Evaluation details updated successfully";
        }
        else
        {
            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        }
    }
    catch (Exception ex)
    {
        objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationEdit: " + ex.ToString());
        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
    }
    return RedirectToAction("ObjectiveEvaluationList", new { ObjectivesTrans_Id = objObjectivesModels.ObjectivesTrans_Id, View = ViewBag.View });
}

        // ----------------End Objective Evaluation------

        // ----------------Start Objective ActionPlan------

        [AllowAnonymous]
public ActionResult AddObjectiveActionPlan()
{

    ObjectivesModelsList objObjectivesModelsList = new ObjectivesModelsList();
    objObjectivesModelsList.ObjectivesMList = new List<ObjectivesModels>();
    ObjectivesModels objObjectivesModels = new ObjectivesModels();
    UserCredentials objUser = new UserCredentials();
    objUser = objGlobaldata.GetCurrentUserSession();
    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
    ViewBag.Evaluation = objObjectivesModels.GetMultiObjectiveEvaluation();
    ViewBag.Status = objGlobaldata.GetDropdownList("Equipment Activity Status");
    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
    try
    {
        ViewBag.View = Request.QueryString["View"];
        if (Request.QueryString["ObjectivesTrans_Id"] != null)
        {
            string sObjectivesTrans_Id = Request.QueryString["ObjectivesTrans_Id"];
                    int count = 0;
                    string sSql = "select id_objective_action from t_objectives_actionplan where ObjectivesTrans_Id  = '" + sObjectivesTrans_Id + "'";
                    DataSet dsList = objGlobaldata.Getdetails(sSql);
                    count = dsList.Tables[0].Rows.Count + 1;

                    string sSqlstmt1 = "select Obj_Ref, c.Objectives_Id,Obj_Estld_On, b.ObjectivesTrans_Id, " +
               " Obj_Target,Base_Line_Value,Monitoring_Mechanism,Target_Date,Objectives_val from t_objectives_trans b,t_objectives c where " +
               "  c.Objectives_Id = b.Objectives_Id  and b.ObjectivesTrans_Id  = '" + sObjectivesTrans_Id + "' and active=1 and trans_active=1";

                    ViewBag.ObjectivesTrans_Id = sObjectivesTrans_Id;

                    DataSet dsObjectivesList = objGlobaldata.Getdetails(sSqlstmt1);

                    if (dsObjectivesList.Tables.Count > 0 && dsObjectivesList.Tables[0].Rows.Count > 0)
                    {

                        ViewBag.Obj_ref = dsObjectivesList.Tables[0].Rows[0]["Obj_Ref"].ToString() + "-AP"+ count;
                        ViewBag.Objectives_Id = dsObjectivesList.Tables[0].Rows[0]["Objectives_Id"].ToString();
                        ViewBag.Obj_Estld_On = Convert.ToDateTime(dsObjectivesList.Tables[0].Rows[0]["Obj_Estld_On"].ToString()).ToString("dd/MM/yyyy");
                        ViewBag.Base_Line_Value = dsObjectivesList.Tables[0].Rows[0]["Base_Line_Value"].ToString();
                        ViewBag.Target_Date = Convert.ToDateTime(dsObjectivesList.Tables[0].Rows[0]["Target_Date"].ToString()).ToString("dd/MM/yyyy");
                        ViewBag.Obj_Target = dsObjectivesList.Tables[0].Rows[0]["Obj_Target"].ToString();
                        ViewBag.Monitoring_Mechanism = dsObjectivesList.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString();
                        ViewBag.Objectives_val = dsObjectivesList.Tables[0].Rows[0]["Objectives_val"].ToString();                       
                    }

                    //    string sSqlstmt = "select Obj_Ref, c.Objectives_Id,Obj_Estld_On,id_objective_action, a.ObjectivesTrans_Id,id_objective_action,actionplan,begin_date,end_date,upload,resource,resp_person, " +
                    //" Obj_Target,Base_Line_Value,Monitoring_Mechanism,Target_Date,Objectives_val from t_objectives_actionplan a, t_objectives_trans b,t_objectives c where a.ObjectivesTrans_Id = b.ObjectivesTrans_Id " +
                    //" and  c.Objectives_Id = b.Objectives_Id  and b.ObjectivesTrans_Id  = '" + sObjectivesTrans_Id + "'";
                    string sSqlstmt = "select id_objective_action, ObjectivesTrans_Id,actionplan,begin_date,end_date,upload,resource,resp_person,action_ref_no " +
                    "from t_objectives_actionplan where ObjectivesTrans_Id  = '" + sObjectivesTrans_Id + "' and active=1";

                    ViewBag.ObjectivesTrans_Id = sObjectivesTrans_Id;

            DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

            if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        objObjectivesModels = new ObjectivesModels
                        {
                            id_objective_action = (dsObjectivesModelsList.Tables[0].Rows[i]["id_objective_action"].ToString()),
                            ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["ObjectivesTrans_Id"].ToString()),
                            actionplan = dsObjectivesModelsList.Tables[0].Rows[i]["actionplan"].ToString(),
                            upload = dsObjectivesModelsList.Tables[0].Rows[i]["upload"].ToString(),
                            resource = dsObjectivesModelsList.Tables[0].Rows[i]["resource"].ToString(),
                            //action_status = objGlobaldata.getEquipmentActionTypeById(dsObjectivesModelsList.Tables[0].Rows[i]["action_status"].ToString()),
                            resp_person = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[i]["resp_person"].ToString()),
                            action_ref_no = dsObjectivesModelsList.Tables[0].Rows[i]["action_ref_no"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsObjectivesModelsList.Tables[0].Rows[i]["begin_date"].ToString() != ""
                           && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[i]["begin_date"].ToString(), out dtDocDate))
                        {
                            objObjectivesModels.begin_date = dtDocDate;
                        }
                        if (dsObjectivesModelsList.Tables[0].Rows[i]["end_date"].ToString() != ""
                          && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[i]["end_date"].ToString(), out dtDocDate))
                        {
                            objObjectivesModels.end_date = dtDocDate;
                        }
                        objObjectivesModelsList.ObjectivesMList.Add(objObjectivesModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddObjectiveActionPlan: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (sObjectivesTrans_Id != "")
                {
                    DataSet dsObjectives_Id = objGlobaldata.Getdetails("select Objectives_Id from t_objectives_trans where ObjectivesTrans_Id='" + sObjectivesTrans_Id + "'");
                    if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.Objectives_Id = dsObjectives_Id.Tables[0].Rows[0]["Objectives_Id"].ToString();
                    }
                }
            }
            //else if (Request.QueryString["Objectives_Id"] != null)
            //{
            //    TempData["alertdata"] = "No data exists";
            //    return RedirectToAction("ObjectivesDetails", new { Objectives_Id = Request.QueryString["Objectives_Id"], View = ViewBag.View });
            //}
            //else
            //{
            //    TempData["alertdata"] = "No data exists";
            //    return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
            //}
        }
        else
        {
            TempData["alertdata"] = "Objectives Trans Id cannot be null";
            return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
        }
    }
    catch (Exception ex)
    {
        objGlobaldata.AddFunctionalLog("Exception in AddObjectiveActionPlan: " + ex.ToString());
        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
    }
    return View(objObjectivesModelsList.ObjectivesMList.ToList());
}

[HttpPost]
[AllowAnonymous]
public ActionResult AddObjectiveActionPlan(FormCollection form, ObjectivesModels objObjectivesModels, HttpPostedFileBase upload)
{
    try
    {
        DateTime dateValue;

        ViewBag.View = form["view"];
        if (DateTime.TryParse(form["begin_date"], out dateValue) == true)
        {
            objObjectivesModels.begin_date = dateValue;
        }
        if (DateTime.TryParse(form["end_date"], out dateValue) == true)
        {
            objObjectivesModels.end_date = dateValue;
        }

        objObjectivesModels.action_ref_no = form["action_ref_no"];
        objObjectivesModels.actionplan = form["actionplan"];
        objObjectivesModels.resource = form["resource"];
        objObjectivesModels.resp_person = form["resp_person"];
        objObjectivesModels.Objectives_Id = Convert.ToInt16(form["Objectives_Id"]);
        objObjectivesModels.ObjectivesTrans_Id = Convert.ToInt16(form["ObjectivesTrans_Id"]);

        if (upload != null && upload.ContentLength > 0)
        {
            try
            {
                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Objectives"), Path.GetFileName(upload.FileName));
                string sFilename = "Upload" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                string sFilepath = Path.GetDirectoryName(spath);

                upload.SaveAs(sFilepath + "/" + sFilename);
                objObjectivesModels.upload = "~/DataUpload/MgmtDocs/Objectives/" + sFilename;
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

        if (objObjectivesModels.FunAddActionPlanTrans(objObjectivesModels))
        {
            TempData["Successdata"] = "Added Objective Plan details successfully";
        }
        else
        {
            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        }
    }
    catch (Exception ex)
    {
        objGlobaldata.AddFunctionalLog("Exception in AddObjectiveActionPlan: " + ex.ToString());
        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
    }
    return RedirectToAction("AddObjectiveActionPlan", new { ObjectivesTrans_Id = objObjectivesModels.ObjectivesTrans_Id, /*Objectives_Id = objObjectivesModels.Objectives_Id,*/ View = ViewBag.View });
}


        [AllowAnonymous]
        public ActionResult AddObjectiveActionPlanStatus()
        {

            ObjectivesModelsList ObjectivesList = new ObjectivesModelsList();
            ObjectivesList.ObjectivesMList = new List<ObjectivesModels>();
            
            ObjectivesModels objObjectivesModels = new ObjectivesModels();
            
            try
            {
                ViewBag.View = Request.QueryString["View"];
                if (Request.QueryString["ObjectivesTrans_Id"] != null)
                {
                    string sObjectivesTrans_Id = Request.QueryString["ObjectivesTrans_Id"];

                    //     string sSqlstmt1 = "select Obj_Ref, c.Objectives_Id,Obj_Estld_On, b.ObjectivesTrans_Id, " +
                    //" Obj_Target,Base_Line_Value,Monitoring_Mechanism,Target_Date,Objectives_val from t_objectives_trans b,t_objectives c where " +
                    //"  c.Objectives_Id = b.Objectives_Id  and b.ObjectivesTrans_Id  = '" + sObjectivesTrans_Id + "' and active=1 and trans_active=1";

                    //     ViewBag.ObjectivesTrans_Id = sObjectivesTrans_Id;

                    //     DataSet dsObjectivesList = objGlobaldata.Getdetails(sSqlstmt1);

                    //     if (dsObjectivesList.Tables.Count > 0 && dsObjectivesList.Tables[0].Rows.Count > 0)
                    //     {

                    //         ViewBag.Obj_ref = dsObjectivesList.Tables[0].Rows[0]["Obj_Ref"].ToString() + "-AP" + count;
                    //         ViewBag.Objectives_Id = dsObjectivesList.Tables[0].Rows[0]["Objectives_Id"].ToString();
                    //         ViewBag.Obj_Estld_On = Convert.ToDateTime(dsObjectivesList.Tables[0].Rows[0]["Obj_Estld_On"].ToString()).ToString("dd/MM/yyyy");
                    //         ViewBag.Base_Line_Value = dsObjectivesList.Tables[0].Rows[0]["Base_Line_Value"].ToString();
                    //         ViewBag.Target_Date = Convert.ToDateTime(dsObjectivesList.Tables[0].Rows[0]["Target_Date"].ToString()).ToString("dd/MM/yyyy");
                    //         ViewBag.Obj_Target = dsObjectivesList.Tables[0].Rows[0]["Obj_Target"].ToString();
                    //         ViewBag.Monitoring_Mechanism = dsObjectivesList.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString();
                    //         ViewBag.Objectives_val = dsObjectivesList.Tables[0].Rows[0]["Objectives_val"].ToString();
                    //     }

                    ViewBag.ActionStatus = objGlobaldata.GetDropdownList("Objective Action Plan Staus");
                    string sSqlstmt = "select id_objective_action, ObjectivesTrans_Id,actionplan,begin_date,end_date,upload,resource,resp_person,action_ref_no," +
                        "action_status,reason_notcomplete,status_updated_on " +
                    "from t_objectives_actionplan where ObjectivesTrans_Id  = '" + sObjectivesTrans_Id + "' and active=1";

                    ViewBag.ObjectivesTrans_Id = sObjectivesTrans_Id;

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                objObjectivesModels = new ObjectivesModels
                                {
                                    id_objective_action = (dsObjectivesModelsList.Tables[0].Rows[i]["id_objective_action"].ToString()),
                                    ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["ObjectivesTrans_Id"].ToString()),
                                    actionplan = dsObjectivesModelsList.Tables[0].Rows[i]["actionplan"].ToString(),
                                    upload = dsObjectivesModelsList.Tables[0].Rows[i]["upload"].ToString(),
                                    resource = dsObjectivesModelsList.Tables[0].Rows[i]["resource"].ToString(),
                                    resp_person = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[i]["resp_person"].ToString()),
                                    action_ref_no = dsObjectivesModelsList.Tables[0].Rows[i]["action_ref_no"].ToString(),

                                    action_status = /*objGlobaldata.GetDropdownitemById*/(dsObjectivesModelsList.Tables[0].Rows[i]["action_status"].ToString()),
                                    reason_notcomplete = (dsObjectivesModelsList.Tables[0].Rows[i]["reason_notcomplete"].ToString()),
                                };
                                DateTime dtDocDate;
                                if (dsObjectivesModelsList.Tables[0].Rows[i]["begin_date"].ToString() != ""
                                   && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[i]["begin_date"].ToString(), out dtDocDate))
                                {
                                    objObjectivesModels.begin_date = dtDocDate;
                                }
                                if (dsObjectivesModelsList.Tables[0].Rows[i]["end_date"].ToString() != ""
                                  && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[i]["end_date"].ToString(), out dtDocDate))
                                {
                                    objObjectivesModels.end_date = dtDocDate;
                                }
                                if (dsObjectivesModelsList.Tables[0].Rows[i]["status_updated_on"].ToString() != ""
                                 && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[i]["status_updated_on"].ToString(), out dtDocDate))
                                {
                                    objObjectivesModels.status_updated_on = dtDocDate;
                                }
                                ObjectivesList.ObjectivesMList.Add(objObjectivesModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddObjectiveActionPlanStatus: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                        ViewBag.ObjectivesList = ObjectivesList;

                        if (sObjectivesTrans_Id != "")
                        {
                            DataSet dsObjectives_Id = objGlobaldata.Getdetails("select Objectives_Id from t_objectives_trans where ObjectivesTrans_Id='" + sObjectivesTrans_Id + "'");
                            if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                            {
                                ViewBag.Objectives_Id = dsObjectives_Id.Tables[0].Rows[0]["Objectives_Id"].ToString();
                            }
                        }
                    }                    
                }
                else
                {
                    TempData["alertdata"] = "Objectives Trans Id cannot be null";
                    return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddObjectiveActionPlanStatus: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objObjectivesModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddObjectiveActionPlanStatus(FormCollection form)
        {
            ObjectivesModels objModel = new ObjectivesModels();

            ViewBag.View = form["view"];
            try
            {     
                ObjectivesModelsList objList = new ObjectivesModelsList();
                objList.ObjectivesMList = new List<ObjectivesModels>();

                int iCnts = 0;
                DateTime dateValue1;
                if (form["itemcount"] != null && form["itemcount"] != "" && int.TryParse(form["itemcount"], out iCnts))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                    {
                        if (form["action_status " + i] != null || form["action_status " + i] != "")
                        {
                            objModel = new ObjectivesModels()
                            {
                                id_objective_action = form["id_objective_action " + i],
                           
                                action_status = form["action_status " + i],
                                reason_notcomplete = form["reason_notcomplete " + i]
                            };
                            
                            if (DateTime.TryParse(form["status_updated_on " + i], out dateValue1) == true)
                            {
                                objModel.status_updated_on = dateValue1;
                            }

                            objList.ObjectivesMList.Add(objModel);
                        }
                    }
                }

                if (objModel.FunUpdateActionPlanStatus(objList))
                {
                    TempData["Successdata"] = "Updated Objective ActionPlan Status successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddObjectiveActionPlanStatus: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            //return RedirectToAction("AddObjectiveActionPlanStatus", new { ObjectivesTrans_Id = objModel.ObjectivesTrans_Id, /*Objectives_Id = objObjectivesModels.Objectives_Id,*/ View = ViewBag.View });
            return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
        }



        [AllowAnonymous]
public ActionResult ObjectiveActionPlanEdit()
{
    ObjectivesModelsList objObjectivesModelsList = new ObjectivesModelsList();
    objObjectivesModelsList.ObjectivesMList = new List<ObjectivesModels>();
    ObjectivesModels objObjectivesModels = new ObjectivesModels();
    UserCredentials objUser = new UserCredentials();
    objUser = objGlobaldata.GetCurrentUserSession();
    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
    string sObjectivesTrans_Id = "";
    try
    {
        ViewBag.View = Request.QueryString["View"];
        if (Request.QueryString["id_objective_action"] != null)
        {
            ViewBag.Status = objGlobaldata.GetDropdownList("Equipment Activity Status");
            string id_objective_action = Request.QueryString["id_objective_action"];

            //string sSqlstmt = "select b.Obj_Ref, id_objective_action, a.ObjectivesTrans_Id,id_objective_action,actionplan,begin_date,end_date,upload,resource,resp_person, " +
            //    " Obj_Target,Base_Line_Value,Monitoring_Mechanism,Target_Date,Objectives_val from t_objectives_actionplan a, t_objectives_trans b,t_objectives c where a.ObjectivesTrans_Id = b.ObjectivesTrans_Id " +
            //    " and  c.Objectives_Id = b.Objectives_Id  and id_objective_action  = '" + id_objective_action + "'";
            string sSqlstmt = "select id_objective_action, ObjectivesTrans_Id,actionplan,begin_date,end_date,upload,resource,resp_person,action_ref_no " +
                    "from t_objectives_actionplan where id_objective_action  = '" + id_objective_action + "'";

            DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

            if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
            {
                //ViewBag.Base_Line_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Base_Line_Value"].ToString();
                //ViewBag.Target_Date = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["Target_Date"].ToString()).ToString("dd/MM/yyyy");
                //ViewBag.Obj_Target = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Target"].ToString();
                //ViewBag.Monitoring_Mechanism = dsObjectivesModelsList.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString();
                //ViewBag.Objectives_val = dsObjectivesModelsList.Tables[0].Rows[0]["Objectives_val"].ToString();
                //ViewBag.Obj_Ref = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Ref"].ToString();

                try
                {
                    objObjectivesModels = new ObjectivesModels
                    {
                        id_objective_action = (dsObjectivesModelsList.Tables[0].Rows[0]["id_objective_action"].ToString()),
                        ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["ObjectivesTrans_Id"].ToString()),
                        actionplan = dsObjectivesModelsList.Tables[0].Rows[0]["actionplan"].ToString(),
                        upload = dsObjectivesModelsList.Tables[0].Rows[0]["upload"].ToString(),
                        resource = dsObjectivesModelsList.Tables[0].Rows[0]["resource"].ToString(),
                        //action_status = (dsObjectivesModelsList.Tables[0].Rows[0]["action_status"].ToString()),
                        resp_person = (dsObjectivesModelsList.Tables[0].Rows[0]["resp_person"].ToString()),
                        action_ref_no= dsObjectivesModelsList.Tables[0].Rows[0]["action_ref_no"].ToString()
                    };
                    DateTime dtDocDate;
                    if (dsObjectivesModelsList.Tables[0].Rows[0]["begin_date"].ToString() != ""
                       && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["begin_date"].ToString(), out dtDocDate))
                    {
                        objObjectivesModels.begin_date = dtDocDate;
                    }
                    if (dsObjectivesModelsList.Tables[0].Rows[0]["end_date"].ToString() != ""
                      && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["end_date"].ToString(), out dtDocDate))
                    {
                        objObjectivesModels.end_date = dtDocDate;
                    }

                    ViewBag.ObjectivesTrans_Id = objObjectivesModels.ObjectivesTrans_Id;
                    sObjectivesTrans_Id = Convert.ToString(objObjectivesModels.ObjectivesTrans_Id);
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in ObjectiveActionPlanEdit" + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

                if (sObjectivesTrans_Id != "")
                {
                    DataSet dsObjectives_Id = objGlobaldata.Getdetails("select Objectives_Id from t_objectives_trans where ObjectivesTrans_Id='" + sObjectivesTrans_Id + "'");
                    if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.Objectives_Id = dsObjectives_Id.Tables[0].Rows[0]["Objectives_Id"].ToString();
                    }
                }
            }
            else
            {
                TempData["alertdata"] = "No data exists";
                return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
            }
        }
        else
        {
            TempData["alertdata"] = "Objectives Trans Id cannot be null";
            return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
        }
    }
    catch (Exception ex)
    {
        objGlobaldata.AddFunctionalLog("Exception in ObjectiveActionPlanEdit: " + ex.ToString());
        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
    }

    return View(objObjectivesModels);
}

[HttpPost]
[AllowAnonymous]
public ActionResult ObjectiveActionPlanEdit(FormCollection form, ObjectivesModels objObjectivesModels, IEnumerable<HttpPostedFileBase> upload)
{
    try
    {
        HttpPostedFileBase files = Request.Files[0];
        string QCDelete = Request.Form["QCDocsValselectall"];
        ViewBag.View = form["view"];
        if (upload != null && files.ContentLength > 0)
        {
            objObjectivesModels.upload = "";
            foreach (var file in upload)
            {
                try
                {
                    string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Objectives"), Path.GetFileName(file.FileName));
                    string sFilename = "ActionPlan" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                    file.SaveAs(sFilepath + "/" + sFilename);
                    objObjectivesModels.upload = objObjectivesModels.upload + "," + "~/DataUpload/MgmtDocs/Objectives/" + sFilename;
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in ObjectiveActionPlanEdit-upload: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            objObjectivesModels.upload = objObjectivesModels.upload.Trim(',');
        }
        else
        {
            ViewBag.Message = "You have not specified a file.";
        }
        if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
        {
            objObjectivesModels.upload = objObjectivesModels.upload + "," + form["QCDocsVal"];
            objObjectivesModels.upload = objObjectivesModels.upload.Trim(',');
        }
        else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
        {
            objObjectivesModels.upload = null;
        }
        else if (form["QCDocsVal"] == null && files.ContentLength == 0)
        {
            objObjectivesModels.upload = null;
        }

        if (objObjectivesModels.FunUpdateActionPlanTrans(objObjectivesModels))
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
        objGlobaldata.AddFunctionalLog("Exception in ObjectiveActionPlanEdit: " + ex.ToString());
        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
    }
    return RedirectToAction("AddObjectiveActionPlan", new { ObjectivesTrans_Id = objObjectivesModels.ObjectivesTrans_Id, View = ViewBag.View });
}


        [AllowAnonymous]
        public JsonResult ObjectiveActionPlanDelete(string id_objective_action)
        {
            try
            {

                if (id_objective_action != null && id_objective_action != "")
                {
                    ObjectivesModels Doc = new ObjectivesModels();                


                    if (Doc.FunDeleteObjectivePlan(id_objective_action))
                    {
                        TempData["Successdata"] = "Objective Action Plan deleted successfully";
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
                    TempData["alertdata"] = "Objective Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectiveActionPlanDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        // ----------------End Objective ActionPlan------


        //-------start potential------------------

        [AllowAnonymous]
        public ActionResult AddObjectivePotential()
        {
            ObjectivesModels objModels = new ObjectivesModels();

            try
            {

                ObjectivesModelsList objObjectivesModelsList = new ObjectivesModelsList();
                objObjectivesModelsList.ObjectivesMList = new List<ObjectivesModels>();

                ViewBag.PotentialStatus = objModels.GetObjectivePotentialStatusList();
                ViewBag.PotentialImpact = objModels.GetObjectivePotentialImpactList();
                ViewBag.DeptHead = objGlobaldata.GetDeptHeadList();
                ViewBag.View = Request.QueryString["View"];


                if (Request.QueryString["ObjectivesTrans_Id"] != null)
                {
                    string sObjectivesTrans_Id = Request.QueryString["ObjectivesTrans_Id"];
                    ViewBag.ObjectivesTrans_Id = sObjectivesTrans_Id;
                    DataSet dsObjectives_Id = objGlobaldata.Getdetails("select Obj_Ref,b.Objectives_Id,b.Objectives_val,Obj_Estld_On,Obj_Target,Base_Line_Value,Monitoring_Mechanism,Target_Date from t_objectives a,t_objectives_trans b where a.Objectives_Id=b.Objectives_Id and ObjectivesTrans_Id='"
                        + sObjectivesTrans_Id + "'");
                    if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.Obj_ref = dsObjectives_Id.Tables[0].Rows[0]["Obj_Ref"].ToString();
                        ViewBag.Objectives_Id = dsObjectives_Id.Tables[0].Rows[0]["Objectives_Id"].ToString();
                        ViewBag.Obj_Target = dsObjectives_Id.Tables[0].Rows[0]["Obj_Target"].ToString();
                        ViewBag.Base_Line_Value = dsObjectives_Id.Tables[0].Rows[0]["Base_Line_Value"].ToString();
                        ViewBag.Monitoring_Mechanism = dsObjectives_Id.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString();
                        ViewBag.Target_Date = Convert.ToDateTime(dsObjectives_Id.Tables[0].Rows[0]["Target_Date"].ToString()).ToString("dd/MM/yyyy");
                        ViewBag.Obj_Estld_On = Convert.ToDateTime(dsObjectives_Id.Tables[0].Rows[0]["Obj_Estld_On"].ToString()).ToString("dd/MM/yyyy");
                        ViewBag.Objectives_val = dsObjectives_Id.Tables[0].Rows[0]["Objectives_val"].ToString();
                    }

                    string sSqlstmt = "select id_potential, ObjectivesTrans_Id,potential_causes,impact,mitigation_measure,targeted_on,updated_on,potential_status,Pcff_Notify from t_objectives_potential " +
                        "where pot_active = 1 and ObjectivesTrans_Id = '" + sObjectivesTrans_Id + "'";

                    ViewBag.ObjectivesTrans_Id = sObjectivesTrans_Id;
                    ViewBag.updated_on = DateTime.Now;
                    ViewBag.potential_status = objModels.GetObjectivePotentialStatusByName("Risk Not Reduced");

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                ObjectivesModels objObjectivesModels = new ObjectivesModels
                                {
                                    ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["ObjectivesTrans_Id"].ToString()),
                                    id_potential = dsObjectivesModelsList.Tables[0].Rows[i]["id_potential"].ToString(),
                                    potential_causes = dsObjectivesModelsList.Tables[0].Rows[i]["potential_causes"].ToString(),
                                    impact = objModels.GetObjectivePotentialImpactById(dsObjectivesModelsList.Tables[0].Rows[i]["impact"].ToString()),
                                    mitigation_measure = dsObjectivesModelsList.Tables[0].Rows[i]["mitigation_measure"].ToString(),
                                    potential_status = objModels.GetObjectivePotentialStatusById(dsObjectivesModelsList.Tables[0].Rows[i]["potential_status"].ToString()),
                                    Pcff_Notify = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[i]["Pcff_Notify"].ToString()),
                                };
                                DateTime dtDocDate;
                                if (dsObjectivesModelsList.Tables[0].Rows[i]["targeted_on"].ToString() != ""
                                   && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[i]["targeted_on"].ToString(), out dtDocDate))
                                {
                                    objObjectivesModels.targeted_on = dtDocDate;
                                }
                                if (dsObjectivesModelsList.Tables[0].Rows[i]["updated_on"].ToString() != ""
                                  && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[i]["updated_on"].ToString(), out dtDocDate))
                                {
                                    objObjectivesModels.updated_on = dtDocDate;
                                }

                                if (dsObjectivesModelsList.Tables[0].Rows[0]["Pcff_Notify"].ToString() != "")
                                {
                                    ViewBag.NotifyToArray = (dsObjectivesModelsList.Tables[0].Rows[0]["Pcff_Notify"].ToString()).Split(',');
                                }
                                objObjectivesModelsList.ObjectivesMList.Add(objObjectivesModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddObjectivePotential: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }

                    return View(objObjectivesModelsList.ObjectivesMList.ToList());
                }
                else
                {
                    TempData["alertdata"] = "Objective Id cannot be Null or empty";
                    return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddObjectivePotential: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AddObjectivePotential", new { ObjectivesTrans_Id = objModels.ObjectivesTrans_Id, /*Objectives_Id = objObjectivesModels.Objectives_Id,*/ View = ViewBag.View });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddObjectivePotential(FormCollection form, ObjectivesModels objObjectivesModels)
        {
            try
            {
                DateTime dateValue;

                ViewBag.View = form["view"];
                if (DateTime.TryParse(form["targeted_on"], out dateValue) == true)
                {
                    objObjectivesModels.targeted_on = dateValue;
                }
                if (DateTime.TryParse(form["updated_on"], out dateValue) == true)
                {
                    objObjectivesModels.updated_on = dateValue;
                }

                objObjectivesModels.impact = form["impact"];
                objObjectivesModels.Objectives_Id = Convert.ToInt16(form["Objectives_Id"]);
                objObjectivesModels.ObjectivesTrans_Id = Convert.ToInt16(form["ObjectivesTrans_Id"]);

                // Nofify
                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objObjectivesModels.Pcff_Notify = form["nempno " + i] + "," + objObjectivesModels.Pcff_Notify;
                    }
                }
                if (objObjectivesModels.Pcff_Notify != null)
                {
                    objObjectivesModels.Pcff_Notify = objObjectivesModels.Pcff_Notify.Trim(',');
                }

                if (objObjectivesModels.FunAddObjectivePotential(objObjectivesModels))
                {
                    TempData["Successdata"] = "Added Potential Causes successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddObjectivePotential: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AddObjectivePotential", new { ObjectivesTrans_Id = objObjectivesModels.ObjectivesTrans_Id, Objectives_Id = objObjectivesModels.Objectives_Id, View = ViewBag.View });
        }      

        [AllowAnonymous]
        public ActionResult ObjectivePotentialEdit()
        {
            ObjectivesModelsList objObjectivesModelsList = new ObjectivesModelsList();
            objObjectivesModelsList.ObjectivesMList = new List<ObjectivesModels>();
            ObjectivesModels objObjectivesModels = new ObjectivesModels();

            ViewBag.PotentialStatus = objObjectivesModels.GetObjectivePotentialStatusList();
            ViewBag.PotentialImpact = objObjectivesModels.GetObjectivePotentialImpactList();
            ViewBag.DeptHead = objGlobaldata.GetDeptHeadList();

            string sObjectivesTrans_Id = "";
            try
            {
                ViewBag.View = Request.QueryString["View"];
                if (Request.QueryString["id_potential"] != null)
                {
                    string id_potential = Request.QueryString["id_potential"];

                    string sSqlstmt = "select id_potential, ObjectivesTrans_Id,potential_causes,impact,mitigation_measure,targeted_on,updated_on,potential_status,logged_by,Pcff_Notify " +
                             "from t_objectives_potential where id_potential  = '" + id_potential + "'";

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        //ViewBag.Base_Line_Value = dsObjectivesModelsList.Tables[0].Rows[0]["Base_Line_Value"].ToString();
                        //ViewBag.Target_Date = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["Target_Date"].ToString()).ToString("dd/MM/yyyy");
                        //ViewBag.Obj_Target = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Target"].ToString();
                        //ViewBag.Monitoring_Mechanism = dsObjectivesModelsList.Tables[0].Rows[0]["Monitoring_Mechanism"].ToString();
                        //ViewBag.Objectives_val = dsObjectivesModelsList.Tables[0].Rows[0]["Objectives_val"].ToString();
                        //ViewBag.Obj_Ref = dsObjectivesModelsList.Tables[0].Rows[0]["Obj_Ref"].ToString();

                        try
                        {
                            objObjectivesModels = new ObjectivesModels
                            {
                                id_potential = (dsObjectivesModelsList.Tables[0].Rows[0]["id_potential"].ToString()),
                                ObjectivesTrans_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["ObjectivesTrans_Id"].ToString()),
                                potential_causes = dsObjectivesModelsList.Tables[0].Rows[0]["potential_causes"].ToString(),
                                impact = dsObjectivesModelsList.Tables[0].Rows[0]["impact"].ToString(),
                                mitigation_measure = dsObjectivesModelsList.Tables[0].Rows[0]["mitigation_measure"].ToString(),
                                potential_status = (dsObjectivesModelsList.Tables[0].Rows[0]["potential_status"].ToString()),
                                Pcff_Notify= (dsObjectivesModelsList.Tables[0].Rows[0]["Pcff_Notify"].ToString())
                            };
                            DateTime dtDocDate;
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["targeted_on"].ToString() != ""
                               && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["targeted_on"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.targeted_on = dtDocDate;
                            }
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["updated_on"].ToString() != ""
                              && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["updated_on"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.updated_on = dtDocDate;
                            }

                            if (dsObjectivesModelsList.Tables[0].Rows[0]["Pcff_Notify"].ToString() != "")
                            {
                                ViewBag.NotifiedtoArray = (dsObjectivesModelsList.Tables[0].Rows[0]["Pcff_Notify"].ToString()).Split(',');
                            }

                            ViewBag.ObjectivesTrans_Id = objObjectivesModels.ObjectivesTrans_Id;
                            sObjectivesTrans_Id = Convert.ToString(objObjectivesModels.ObjectivesTrans_Id);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ObjectiveActionPlanEdit" + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }

                        if (sObjectivesTrans_Id != "")
                        {
                            DataSet dsObjectives_Id = objGlobaldata.Getdetails("select Objectives_Id from t_objectives_trans where ObjectivesTrans_Id='" + sObjectivesTrans_Id + "'");
                            if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                            {
                                ViewBag.Objectives_Id = dsObjectives_Id.Tables[0].Rows[0]["Objectives_Id"].ToString();
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
                    }
                }
                else
                {
                    TempData["alertdata"] = "Objectives Trans Id cannot be null";
                    return RedirectToAction("ObjectivesList", new { View = ViewBag.View });
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectivePotentialEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objObjectivesModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ObjectivePotentialEdit(FormCollection form, ObjectivesModels objModels)
        {
            try
            {
                ViewBag.View = form["view"];
                objModels.impact = form["impact"];

                DateTime dateValue;

                if (DateTime.TryParse(form["targeted_on"], out dateValue) == true)
                {
                    objModels.targeted_on = dateValue;
                }
                if (DateTime.TryParse(form["updated_on"], out dateValue) == true)
                {
                    objModels.updated_on = dateValue;
                }

                // Nofify
                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModels.Pcff_Notify = form["nempno " + i] + "," + objModels.Pcff_Notify;
                    }
                }
                if (objModels.Pcff_Notify != null)
                {
                    objModels.Pcff_Notify = objModels.Pcff_Notify.Trim(',');
                }

                if (objModels.FunUpdateObjectivePotential(objModels))
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
                objGlobaldata.AddFunctionalLog("Exception in ObjectivePotentialEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AddObjectivePotential", new { ObjectivesTrans_Id = objModels.ObjectivesTrans_Id, View = ViewBag.View });
        }

        [AllowAnonymous]
        public JsonResult ObjectivePotentialDelete(string id_potential)
        {
            try
            {
                if (id_potential != null && id_potential != "")
                {
                    ObjectivesModels Doc = new ObjectivesModels();

                    if (Doc.FunDeleteObjectivePotential(id_potential))
                    {
                        TempData["Successdata"] = "Objective Potential deleted successfully";
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
                    TempData["alertdata"] = "Objective Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectivePotentialDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        
        //-----------End Potential

        [AllowAnonymous]
        public ActionResult ObjectivesApprove(string ObjectivesTrans_Id, int iStatus, string PendingFlg)
        {
            try
            {
                ObjectivesModels objObjectives = new ObjectivesModels();
                //if (objGlobaldata.GetRoleName(objGlobaldata.GetCurrentUserSession().role) == "Approver")
                //{
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().empid;

                string sStatus = "";
                if (iStatus == 0)
                {
                    sStatus = "Pending";
                }
                else if (iStatus == 1)
                {
                    sStatus = "Approved";

                }
                else if (iStatus == 2)
                {
                    sStatus = "Rejected";
                }

                if (objObjectives.FunUpdateObjectivesApproval(user, ObjectivesTrans_Id, iStatus))
                {
                    TempData["Successdata"] = "Objectives is " + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                //}
                //else
                //{
                //    TempData["alertdata"] = "Access Denied";
                //}

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectivesApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
        }

        [AllowAnonymous]
        public ActionResult ObjectivesApprovebyDetails(FormCollection form, HttpPostedFileBase obj_reject_upload)
        {
            string sView = form["view"];
            try
            {
                ObjectivesModels objObjectives = new ObjectivesModels();
                objObjectives.ObjectivesTrans_Id = Convert.ToInt32(form["ObjectivesTrans_Id"]);
                objObjectives.Approved_Status = form["obj_status"];
                objObjectives.obj_reject_comment = form["obj_reject_comment"];  

                if (obj_reject_upload != null && obj_reject_upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Objectives"), Path.GetFileName(obj_reject_upload.FileName));
                        string sFilename = "OBJ" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        obj_reject_upload.SaveAs(sFilepath + "/" + sFilename);
                        objObjectives.obj_reject_upload = "~/DataUpload/MgmtDocs/Objectives/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in ObjectivesApprovebyDetails: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                string sStatus = "";
                if (objObjectives.Approved_Status == "0")
                {
                    sStatus = "Pending";
                }
                else if (objObjectives.Approved_Status == "1")
                {
                    sStatus = "Approved";

                }
                else if (objObjectives.Approved_Status == "2")
                {
                    sStatus = "Rejected";
                }

                if (objObjectives.FunObjectivesApprovalbyDetail(objObjectives))
                {
                    TempData["Successdata"] = "Objectives is " + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }              

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectivesApprovebyDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ObjectivesList", new { View = sView });
        }

        public JsonResult ObjectivesApproveNoty(string Objectives_Id, int iStatus, string PendingFlg)
        {
            try
            {
                ObjectivesModels objObjectives = new ObjectivesModels();
                //if (objGlobaldata.GetRoleName(objGlobaldata.GetCurrentUserSession().role) == "Approver")
                //{
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().empid;

                string sStatus = "";
                if (iStatus == 0)
                {
                    sStatus = "Pending";
                }
                else if (iStatus == 1)
                {
                    sStatus = "Approved";

                }
                else if (iStatus == 2)
                {
                    sStatus = "Rejected";
                }

                if (objObjectives.FunUpdateObjectivesApproval(user, Objectives_Id, iStatus))
                {
                    return Json("Success" + iStatus);
                }
                else
                {
                    return Json("Failed");
                }
                //}
                //else
                //{
                //    TempData["alertdata"] = "Access Denied";
                //}

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectivesApprove: " + ex.ToString());

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

        public ActionResult FunGetDeptEmpList(string DeptId)
        {
            MultiSelectList lstEmp = objGlobaldata.GetDeptHeadList(DeptId);
            return Json(lstEmp);
        }

        public ActionResult FunGetDeptList(string[] Branch)
        {
            string sBranch = string.Join(",", Branch);
            MultiSelectList lstEmp = objGlobaldata.GetDepartmentList1(sBranch);
            return Json(lstEmp);
        }

        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase Action_Plan1 = Request.Files;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Objectives"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "ActionPlan" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/Objectives/" + "ActionPlan" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);

            }
            return Json("");//Failed return null value
        }

        [HttpPost]
        public JsonResult doesObjRefExist(string Obj_Ref)
        {
            ObjectivesModels objObjectives = new ObjectivesModels();
            var Objective = objObjectives.checkObjRefExists(Obj_Ref);

            return Json(Objective);
        }
        [HttpPost]
        public JsonResult UploadDocument1()
        {
            HttpFileCollectionBase upload = Request.Files;
            ObjectivesModels obj = new ObjectivesModels();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Objectives"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "ActionPlan" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                //return Json("~/DataUpload/MgmtDocs/Surveillance/" + "Surveillance" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                obj.upload = obj.upload + "," + "~/DataUpload/MgmtDocs/Objectives/" + "ActionPlan" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename;

            }
            obj.upload = obj.upload.Trim(',');
            return Json(obj.upload);
        }

        public JsonResult FunGetNCRNumberList()
        {
            MultiSelectList NCRNo = objGlobaldata.GetNCRNumberList();
            return Json(NCRNo);
        }

    }
}
