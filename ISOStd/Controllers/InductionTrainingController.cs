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
    public class InductionTrainingController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public InductionTrainingController()
        {
            ViewBag.Menutype = "Employee";
            ViewBag.SubMenutype = "Induction";
        }

        [AllowAnonymous]
        public ActionResult AddInduction()
        {
            InductionTrainingModels objModel = new InductionTrainingModels();
            try
            {
                objModel.planned_by = objGlobaldata.GetCurrentUserSession().empid;
                objModel.division = objGlobaldata.GetCurrentUserSession().division;
                objModel.department = objGlobaldata.GetCurrentUserSession().DeptID;
                objModel.location = objGlobaldata.GetCurrentUserSession().Work_Location;
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objModel.division);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objModel.division);
                                
                ViewBag.EmpList = objGlobaldata.GetHrEmpListByDivision(objModel.division);
                ViewBag.AllEmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.TrainingTopic = objGlobaldata.GetTrainingTopicList();
                ViewBag.Priority = objModel.GetInductionPriorityList();
                ViewBag.Status = objModel.GetInductionStatusList();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInduction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddInduction(InductionTrainingModels objModel, FormCollection form)
        {
            try
            {
                if (objModel != null)
                {
                    objModel.division = form["division"];
                    objModel.location = form["location"];
                    objModel.department = form["department"];
                    objModel.planned_by = form["planned_by"];
                    objModel.training_topic = form["training_topic"];

                    objModel.final_status = objModel.GetInductionStatusIdByName("In Progress");

                    DateTime dateValue;
                    if (form["start_date"] != null && DateTime.TryParse(form["start_date"], out dateValue) == true)
                    {
                        objModel.start_date = dateValue;
                        int iHr, iMin;
                        if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                            form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                        {
                            objModel.start_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                        }
                    }

                    if (form["end_date"] != null && DateTime.TryParse(form["end_date"], out dateValue) == true)
                    {
                        objModel.end_date = dateValue;
                        int iiHr, iiMin;
                        if (form["EndTimeInHour"] != null && int.TryParse(form["EndTimeInHour"], out iiHr) &&
                            form["EndTimeInMin"] != null && int.TryParse(form["EndTimeInMin"], out iiMin))
                        {
                            objModel.end_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iiHr + ":" + iiMin + ":00");
                        }
                    }

                    //Training Employees
                    objModel.employee_id = "";
                    for (int i = 0; i < Convert.ToInt16(form["Empcount"]); i++)
                    {
                        if (form["iempno " + i] != "" && form["iempno " + i] != null)
                        {
                            objModel.employee_id = objModel.employee_id + "," + form["iempno " + i];
                        }
                    }
                    if (objModel.employee_id != null)
                    {
                        objModel.employee_id = objModel.employee_id.Trim(',');
                    }

                    //Notified To
                    for (int i = 0; i < Convert.ToInt16(form["NotifyCount"]); i++)
                    {
                        if (form["nempno " + i] != "" && form["nempno " + i] != null)
                        {
                            objModel.plan_notifiedto = form["nempno " + i] + "," + objModel.plan_notifiedto;
                        }
                    }
                    if (objModel.plan_notifiedto != null)
                    {
                        objModel.plan_notifiedto = objModel.plan_notifiedto.Trim(',');
                    }

                    //planned_by
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["empno " + i] != "" && form["empno " + i] != null)
                        {
                            objModel.planned_by = form["empno " + i] + "," + objModel.planned_by;
                        }
                    }
                    if (objModel.planned_by != null)
                    {
                        objModel.planned_by = objModel.planned_by.Trim(',');
                    }

                    //t_induction_training_material
                    InductionTrainingModelsList objIndList = new InductionTrainingModelsList();
                    objIndList.lstInd = new List<InductionTrainingModels>();

                    for (int i = 0; i < Convert.ToInt16(form["matcount"]); i++)
                    {
                        InductionTrainingModels objITModel = new InductionTrainingModels();
                        if (form["mat_description " + i] != "" && form["mat_description " + i] != null)
                        {
                            objITModel.mat_description = form["mat_description " + i];
                            objITModel.upload = form["matupload " + i];                            
                        }
                        objIndList.lstInd.Add(objITModel);
                    }

                    if (objModel.FunAddInduction(objModel, objIndList))
                    {
                        TempData["Successdata"] = "Added Induction Training details successfully with RefNo '" + objModel.ref_no + "'";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInduction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("InductionList");
        }
        
        [AllowAnonymous]
        public ActionResult InductionList(string branch_name)
        {
            InductionTrainingModelsList objIndList = new InductionTrainingModelsList();
            objIndList.lstInd = new List<InductionTrainingModels>();

            InductionTrainingModels objModels = new InductionTrainingModels();
            try
            {
                string sSearchtext = "";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_induction, ref_no, division, department, location,training_topic, planned_by, start_date,  end_date, employee_id,"
                   + "plan_notifiedto, priority, logged_by from t_induction_training where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', division)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_induction desc";

                DataSet dsIndList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsIndList.Tables.Count > 0 && dsIndList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsIndList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                             objModels = new InductionTrainingModels
                            {
                                id_induction = dsIndList.Tables[0].Rows[i]["id_induction"].ToString(),
                                ref_no = dsIndList.Tables[0].Rows[i]["ref_no"].ToString(),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsIndList.Tables[0].Rows[i]["division"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsIndList.Tables[0].Rows[i]["department"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsIndList.Tables[0].Rows[i]["location"].ToString()),
                                planned_by = dsIndList.Tables[0].Rows[i]["planned_by"].ToString(),
                                training_topic = objGlobaldata.GetTrainingTopicById(dsIndList.Tables[0].Rows[i]["training_topic"].ToString()),
                                employee_id = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[i]["employee_id"].ToString()),
                                priority = objModels.GetInductionPriorityById(dsIndList.Tables[0].Rows[i]["priority"].ToString()),
                               };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[i]["start_date"].ToString(), out dtValue))
                            {
                                objModels.start_date = dtValue;
                            }
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[i]["end_date"].ToString(), out dtValue))
                            {
                                objModels.end_date = dtValue;
                            }

                            string sSqlstmt1 = "select final_status from t_induction_training_trans where id_induction ='"+ dsIndList.Tables[0].Rows[i]["id_induction"].ToString() + "' and active=1";
                            DataSet dsTransList = objGlobaldata.Getdetails(sSqlstmt1);
                            if (dsTransList.Tables.Count > 0 && dsTransList.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dsTransList.Tables[0].Rows.Count; j++)
                                {
                                    try
                                    {
                                       if(objModels.GetInductionStatusById(dsTransList.Tables[0].Rows[j]["final_status"].ToString()).ToLower() == "completed")
                                        {
                                            objModels.final_status = dsTransList.Tables[0].Rows[j]["final_status"].ToString();
                                        }
                                        else
                                        {
                                            objModels.final_status = "In Progress";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in InductionList: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }
                                                       
                            string sSqlstmt2 = "select comments from t_induction_training_material where id_induction ='" + dsIndList.Tables[0].Rows[i]["id_induction"].ToString() + "' and active=1";
                            DataSet dscommList = objGlobaldata.Getdetails(sSqlstmt2);
                            if (dscommList.Tables.Count > 0 && dscommList.Tables[0].Rows.Count > 0)
                            {
                                for (int p = 0; p < dscommList.Tables[0].Rows.Count; p++)
                                {
                                    try
                                    {
                                        if (dscommList.Tables[0].Rows[p]["comments"].ToString() == "" || dscommList.Tables[0].Rows[p]["comments"].ToString() == null)
                                        {
                                            objModels.comments = "comment";
                                            break;
                                        }                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in InductionList: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }

                            objIndList.lstInd.Add(objModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in InductionList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }               

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InductionList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objIndList.lstInd.ToList());
        }

        [AllowAnonymous]
        public ActionResult InductionEdit()
        {
            InductionTrainingModels objModels = new InductionTrainingModels();
            string sid_induction = Request.QueryString["id_induction"];
            if (sid_induction != "" && sid_induction != null)
            {
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
               
                //ViewBag.EmpList = objGlobaldata.GetHrEmpListByDivision(objModel.division);
                ViewBag.AllEmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.TrainingTopic = objGlobaldata.GetTrainingTopicList();
                ViewBag.Priority = objModels.GetInductionPriorityList();
                ViewBag.Status = objModels.GetInductionStatusList();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                try
               {
                string sSqlstmt = "select id_induction, ref_no, division, department,location,training_topic, planned_by, start_date,  end_date, employee_id,"
                   + "plan_notifiedto, priority, logged_by from t_induction_training where id_induction='"+ sid_induction + "'";

                DataSet dsIndList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsIndList.Tables.Count > 0 && dsIndList.Tables[0].Rows.Count > 0)
                {
                        try
                        {
                            objModels = new InductionTrainingModels
                            {
                                id_induction = dsIndList.Tables[0].Rows[0]["id_induction"].ToString(),
                                ref_no = dsIndList.Tables[0].Rows[0]["ref_no"].ToString(),
                                division =/* objGlobaldata.GetMultiCompanyBranchNameById*/(dsIndList.Tables[0].Rows[0]["division"].ToString()),
                                department =/* objGlobaldata.GetMultiDeptNameById*/(dsIndList.Tables[0].Rows[0]["department"].ToString()),
                                location =/* objGlobaldata.GetDivisionLocationById*/(dsIndList.Tables[0].Rows[0]["location"].ToString()),
                                planned_by = dsIndList.Tables[0].Rows[0]["planned_by"].ToString(),
                                training_topic = /*objGlobaldata.GetDropdownitemById*/(dsIndList.Tables[0].Rows[0]["training_topic"].ToString()),
                                employee_id =/* objGlobaldata.GetMultiHrEmpNameById*/(dsIndList.Tables[0].Rows[0]["employee_id"].ToString()),
                                priority = /*objModels.GetInductionPriorityById*/(dsIndList.Tables[0].Rows[0]["priority"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["start_date"].ToString(), out dtValue))
                            {
                                objModels.start_date = dtValue;
                            }
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["end_date"].ToString(), out dtValue))
                            {
                                objModels.end_date = dtValue;
                            }
                            ViewBag.Department = objGlobaldata.GetDepartmentList1(dsIndList.Tables[0].Rows[0]["division"].ToString());
                            ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsIndList.Tables[0].Rows[0]["division"].ToString());

                            if (dsIndList.Tables[0].Rows[0]["planned_by"].ToString() != "")
                            {
                                ViewBag.PlanbyArray = (dsIndList.Tables[0].Rows[0]["planned_by"].ToString()).Split(',');
                            }
                            if (dsIndList.Tables[0].Rows[0]["plan_notifiedto"].ToString() != "")
                            {
                                ViewBag.PlanToArray = (dsIndList.Tables[0].Rows[0]["plan_notifiedto"].ToString()).Split(',');
                            }

                            InductionTrainingModelsList objIndList = new InductionTrainingModelsList();
                            objIndList.lstInd = new List<InductionTrainingModels>();
                           if(dsIndList.Tables[0].Rows[0]["employee_id"].ToString() != null && dsIndList.Tables[0].Rows[0]["employee_id"].ToString() != "")
                            {
                                string empId = dsIndList.Tables[0].Rows[0]["employee_id"].ToString();
                                string sempId = "";
                                if (empId.Contains(','))
                                {
                                    sempId = empId.Substring(0, empId.IndexOf(","));
                                }
                                else
                                {
                                    sempId = empId;
                                }
                             
                             string sSqlstmt1 = "select id_material, mat_description, upload,traing_start_date from t_induction_training_material where id_induction='" + sid_induction + "' and employee_id='"+ sempId +"'";
                            DataSet dsIndList1 = objGlobaldata.Getdetails(sSqlstmt1);

                            if (dsIndList1.Tables.Count > 0 && dsIndList1.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsIndList1.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        InductionTrainingModels objIndModels = new InductionTrainingModels
                                        {
                                            id_material = dsIndList1.Tables[0].Rows[i]["id_material"].ToString(),
                                            mat_description = dsIndList1.Tables[0].Rows[i]["mat_description"].ToString(),
                                            upload = dsIndList1.Tables[0].Rows[i]["upload"].ToString(),
                                        };
                                        DateTime dtValue1;
                                        if (DateTime.TryParse(dsIndList1.Tables[0].Rows[i]["traing_start_date"].ToString(), out dtValue1))
                                        {
                                            objIndModels.traing_start_date = dtValue1;
                                        }
                                        objIndList.lstInd.Add(objIndModels);
                                        ViewBag.objMatList = objIndList;
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in AddPerformInduction: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }
                        } 
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in InductionEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }                       
                    }              
               }
              catch (Exception ex)
              {
                objGlobaldata.AddFunctionalLog("Exception in InductionEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
               }
            }
            return View(objModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult InductionEdit(InductionTrainingModels objModel, FormCollection form)
        {
            try
            {
                if (objModel != null)
                {
                    objModel.division = form["division"];
                    objModel.location = form["location"];
                    objModel.department = form["department"];
                   // objModel.planned_by = form["planned_by"];
                    objModel.training_topic = form["training_topic"];
                    objModel.employee_id = form["employee_id"];
                    
                    DateTime dateValue;
                    if (form["start_date"] != null && DateTime.TryParse(form["start_date"], out dateValue) == true)
                    {
                        objModel.start_date = dateValue;
                        int iHr, iMin;
                        if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                            form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                        {
                            objModel.start_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                        }
                    }

                    if (form["end_date"] != null && DateTime.TryParse(form["end_date"], out dateValue) == true)
                    {
                        objModel.end_date = dateValue;
                        int iiHr, iiMin;
                        if (form["EndTimeInHour"] != null && int.TryParse(form["EndTimeInHour"], out iiHr) &&
                            form["EndTimeInMin"] != null && int.TryParse(form["EndTimeInMin"], out iiMin))
                        {
                            objModel.end_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iiHr + ":" + iiMin + ":00");
                        }
                    }

                    //Training Employees
                    //objModel.employee_id = "";
                    //for (int i = 0; i < Convert.ToInt16(form["Empcount"]); i++)
                    //{
                    //    if (form["iempno " + i] != "" && form["iempno " + i] != null)
                    //    {
                    //        objModel.employee_id = objModel.employee_id + "," + form["iempno " + i];
                    //    }
                    //}
                    //if (objModel.employee_id != null)
                    //{
                    //    objModel.employee_id = objModel.employee_id.Trim(',');
                    //}

                    //plan_notifiedto
                    for (int i = 0; i < Convert.ToInt16(form["NotifyCount"]); i++)
                    {
                        if (form["nempno " + i] != "" && form["nempno " + i] != null)
                        {
                            objModel.plan_notifiedto = form["nempno " + i] + "," + objModel.plan_notifiedto;
                        }
                    }
                    if (objModel.plan_notifiedto != null)
                    {
                        objModel.plan_notifiedto = objModel.plan_notifiedto.Trim(',');
                    }


                    //planned_by
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["empno " + i] != "" && form["empno " + i] != null)
                        {
                            objModel.planned_by = form["empno " + i] + "," + objModel.planned_by;
                        }
                    }
                    if (objModel.planned_by != null)
                    {
                        objModel.planned_by = objModel.planned_by.Trim(',');
                    }


                    //t_induction_training_material
                    InductionTrainingModelsList objIndList = new InductionTrainingModelsList();
                    objIndList.lstInd = new List<InductionTrainingModels>();

                    for (int i = 0; i < Convert.ToInt16(form["matcount"]); i++)
                    {
                        InductionTrainingModels objITModel = new InductionTrainingModels();
                        if (form["mat_description " + i] != "" && form["mat_description " + i] != null)
                        {
                            objITModel.mat_description = form["mat_description " + i];
                            objITModel.upload = form["matupload " + i];

                            objIndList.lstInd.Add(objITModel);
                        }
                       
                    }

                    if (objModel.FunUpdateInduction(objModel, objIndList))
                    {
                        TempData["Successdata"] = "Updated Induction Training details successfully with RefNo '" + objModel.ref_no + "'";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InductionEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("InductionList");
        }

        [AllowAnonymous]
        public ActionResult InductionDetails()
        {
            InductionTrainingModels objModels = new InductionTrainingModels();
            string sid_induction = Request.QueryString["id_induction"];
            if (sid_induction != "" && sid_induction != null)
            {           
                try
                {
                    string sSqlstmt = "select id_induction, ref_no, division, department,location,training_topic, planned_by, start_date,  end_date, employee_id,"
                       + "plan_notifiedto, priority, logged_by from t_induction_training where id_induction='" + sid_induction + "'";

                    DataSet dsIndList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsIndList.Tables.Count > 0 && dsIndList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objModels = new InductionTrainingModels
                            {
                                id_induction = dsIndList.Tables[0].Rows[0]["id_induction"].ToString(),
                                ref_no = dsIndList.Tables[0].Rows[0]["ref_no"].ToString(),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsIndList.Tables[0].Rows[0]["division"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsIndList.Tables[0].Rows[0]["department"].ToString()),
                                location =objGlobaldata.GetDivisionLocationById(dsIndList.Tables[0].Rows[0]["location"].ToString()),
                                planned_id = (dsIndList.Tables[0].Rows[0]["planned_by"].ToString()),
                                planned_by = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["planned_by"].ToString()),
                                training_topic = objGlobaldata.GetTrainingTopicById(dsIndList.Tables[0].Rows[0]["training_topic"].ToString()),
                                employee_id = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["employee_id"].ToString()),
                                priority = objModels.GetInductionPriorityById(dsIndList.Tables[0].Rows[0]["priority"].ToString()),
                                plan_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["plan_notifiedto"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["start_date"].ToString(), out dtValue))
                            {
                                objModels.start_date = dtValue;
                            }
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["end_date"].ToString(), out dtValue))
                            {
                                objModels.end_date = dtValue;
                            }
                            
                            InductionTrainingModelsList objIndList = new InductionTrainingModelsList();
                            objIndList.lstInd = new List<InductionTrainingModels>();
                            if (dsIndList.Tables[0].Rows[0]["employee_id"].ToString() != null && dsIndList.Tables[0].Rows[0]["employee_id"].ToString() != "")
                            {
                                string empId = dsIndList.Tables[0].Rows[0]["employee_id"].ToString();
                                string sempId = "";
                                if (empId.Contains(','))
                                {
                                    sempId = empId.Substring(0, empId.IndexOf(","));
                                }
                                else
                                {
                                    sempId = empId;
                                }

                                string sSqlstmt1 = "select id_material, mat_description, upload,traing_start_date from t_induction_training_material where id_induction='" + sid_induction + "' and employee_id='" + sempId + "'";
                                DataSet dsIndList1 = objGlobaldata.Getdetails(sSqlstmt1);

                                if (dsIndList1.Tables.Count > 0 && dsIndList1.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < dsIndList1.Tables[0].Rows.Count; i++)
                                    {
                                        try
                                        {
                                            InductionTrainingModels objIndModels = new InductionTrainingModels
                                            {
                                                id_material = dsIndList1.Tables[0].Rows[i]["id_material"].ToString(),
                                                mat_description = dsIndList1.Tables[0].Rows[i]["mat_description"].ToString(),
                                                upload = dsIndList1.Tables[0].Rows[i]["upload"].ToString(),
                                            };
                                            DateTime dtValue1;
                                            if (DateTime.TryParse(dsIndList1.Tables[0].Rows[i]["traing_start_date"].ToString(), out dtValue1))
                                            {
                                                objIndModels.traing_start_date = dtValue1;
                                            }
                                            objIndList.lstInd.Add(objIndModels);
                                            ViewBag.objMatList = objIndList;
                                        }
                                        catch (Exception ex)
                                        {
                                            objGlobaldata.AddFunctionalLog("Exception in AddPerformInduction: " + ex.ToString());
                                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                        }
                                    }
                                }


                                string sSqlstmt2 = "select comments from t_induction_training_material where id_induction ='" + sid_induction + "' and active=1";
                                DataSet dscommList = objGlobaldata.Getdetails(sSqlstmt2);
                                if (dscommList.Tables.Count > 0 && dscommList.Tables[0].Rows.Count > 0)
                                {
                                    for (int p = 0; p < dscommList.Tables[0].Rows.Count; p++)
                                    {
                                        try
                                        {
                                            if (dscommList.Tables[0].Rows[p]["comments"].ToString() == "" || dscommList.Tables[0].Rows[p]["comments"].ToString() == null)
                                            {
                                                objModels.comments = "comment";
                                                break;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            objGlobaldata.AddFunctionalLog("Exception in InductionList: " + ex.ToString());
                                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                        }
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in InductionDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in InductionDetails: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            return View(objModels);
        }


        [AllowAnonymous]
        public JsonResult InductionDelete(FormCollection form)
        {
            try
            {

                if (form["id_induction"] != null && form["id_induction"] != "")
                {

                    InductionTrainingModels Doc = new InductionTrainingModels();
                    string sid_induction = form["id_induction"];

                    if (Doc.FunInductionDelete(sid_induction))
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
                objGlobaldata.AddFunctionalLog("Exception in InductionDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult AddPerformInduction()
        {
            InductionTrainingModels objModels = new InductionTrainingModels();
            try
            {
                //ViewBag.CurrentEmp_id = objGlobaldata.GetCurrentUserSession().empid;
                //objModels.employee_Name = objGlobaldata.GetEmpHrNameById(ViewBag.CurrentEmp_id);
                ViewBag.Status = objModels.GetInductionStatusList();
                ViewBag.Effectness = objModels.GetInductionEffectivenessList();
                ViewBag.DeptHead = objGlobaldata.GetDeptHeadList();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                               
                string sid_induction = Request.QueryString["id_induction"];
                string semployee_id = Request.QueryString["employee_id"];
                ViewBag.semployee_id = semployee_id;
                if (sid_induction != null && sid_induction != ""&& semployee_id != null && semployee_id != "")
                {

                    string sSqlstmt = "select b.id_induction, ref_no, division, department, location,training_topic, planned_by, start_date,  end_date, "
                      + "plan_notifiedto, priority, logged_by,final_status,completion_date," +
                      "effectness,suggestion,further_training,final_notifyto,b.employee_id from t_induction_training a, t_induction_training_trans b " +
                      "where b.id_induction='" + sid_induction + "' and b.employee_id='" + semployee_id + "' and a.id_induction=b.id_induction and a.active=1 and b.active=1";

                    DataSet dsIndList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsIndList.Tables.Count > 0 && dsIndList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objModels = new InductionTrainingModels
                            {
                                id_induction = dsIndList.Tables[0].Rows[0]["id_induction"].ToString(),
                                ref_no = dsIndList.Tables[0].Rows[0]["ref_no"].ToString(),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsIndList.Tables[0].Rows[0]["division"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsIndList.Tables[0].Rows[0]["department"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsIndList.Tables[0].Rows[0]["location"].ToString()),
                                training_topic = objGlobaldata.GetTrainingTopicById(dsIndList.Tables[0].Rows[0]["training_topic"].ToString()),
                                employee_Name = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["employee_id"].ToString()),
                                priority = objModels.GetInductionPriorityById(dsIndList.Tables[0].Rows[0]["priority"].ToString()),
                              
                                final_status = dsIndList.Tables[0].Rows[0]["final_status"].ToString(),
                                effectness = dsIndList.Tables[0].Rows[0]["effectness"].ToString(),
                                suggestion = dsIndList.Tables[0].Rows[0]["suggestion"].ToString(),
                                further_training = dsIndList.Tables[0].Rows[0]["further_training"].ToString(),
                                final_notifyto = dsIndList.Tables[0].Rows[0]["final_notifyto"].ToString()                               
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["start_date"].ToString(), out dtValue))
                            {
                                objModels.start_date = dtValue;
                            }
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["end_date"].ToString(), out dtValue))
                            {
                                objModels.end_date = dtValue;
                            }
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["completion_date"].ToString(), out dtValue))
                            {
                                objModels.completion_date = dtValue;
                            }

                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddPerformInduction: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }

                    }


                    InductionTrainingModelsList objIndList = new InductionTrainingModelsList();
                    objIndList.lstInd = new List<InductionTrainingModels>();

                    string sSqlstmt1 = "select id_material,id_induction, employee_id, mat_description, upload,traing_start_date,comments, " +
                        "`explain`,compled_date,notify_to from t_induction_training_material where id_induction='" + sid_induction + "' and employee_id ='" + semployee_id + "' and active=1";

                    DataSet dsIndList1 = objGlobaldata.Getdetails(sSqlstmt1);

                    if (dsIndList1.Tables.Count > 0 && dsIndList1.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsIndList1.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                InductionTrainingModels objIndModels = new InductionTrainingModels
                                {
                                    id_material = dsIndList1.Tables[0].Rows[i]["id_material"].ToString(),
                                    mat_description = dsIndList1.Tables[0].Rows[i]["mat_description"].ToString(),
                                    upload = dsIndList1.Tables[0].Rows[i]["upload"].ToString(),
                                    comments = dsIndList1.Tables[0].Rows[i]["comments"].ToString(),
                                    explain = dsIndList1.Tables[0].Rows[i]["explain"].ToString(),
                                    notify_to = dsIndList1.Tables[0].Rows[i]["notify_to"].ToString(),
                                };
                                DateTime dtValue;
                                if (DateTime.TryParse(dsIndList1.Tables[0].Rows[i]["traing_start_date"].ToString(), out dtValue))
                                {
                                    objIndModels.traing_start_date = dtValue;
                                }
                                if (DateTime.TryParse(dsIndList1.Tables[0].Rows[i]["compled_date"].ToString(), out dtValue))
                                {
                                    objIndModels.compled_date = dtValue;
                                }
                                objIndList.lstInd.Add(objIndModels);
                                ViewBag.objTransList = objIndList;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddPerformInduction: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }

                    return View(objModels);
                }               

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPerformInduction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("PerformInductionList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddPerformInduction(InductionTrainingModels objModel, FormCollection form)
        {
            try
            {
                if (objModel != null)
                {
                    objModel.employee_id = form["employee_id"];
                    DateTime dateValue;
                    if (form["traing_start_date"] != null && DateTime.TryParse(form["traing_start_date"], out dateValue) == true)
                    {
                        objModel.traing_start_date = dateValue;
                    }

                    if (form["completion_date"] != null && DateTime.TryParse(form["completion_date"], out dateValue) == true)
                    {
                        objModel.completion_date = dateValue;                        
                    }  

                    //t_induction_training_material
                    InductionTrainingModelsList objIndList = new InductionTrainingModelsList();
                    objIndList.lstInd = new List<InductionTrainingModels>();

                    for (int i = 0; i < Convert.ToInt16(form["matcount"]); i++)
                    {
                        InductionTrainingModels objITModel = new InductionTrainingModels();
                        if (form["mat_description " + i] != "" && form["mat_description " + i] != null)
                        {
                            objITModel.id_material = form["id_material " + i];
                            //objITModel.mat_description = form["mat_description " + i];
                            //objITModel.upload = form["upload " + i];
                            objITModel.comments = form["comments " + i];
                            objITModel.explain = form["explain " + i];
                            objITModel.notify_to = form["notify_to " + i];                           
                        }
                        DateTime dtValue;
                        if (DateTime.TryParse(form["traing_start_date " + i], out dtValue))
                        {
                            objITModel.traing_start_date = dtValue;
                        }
                        if (DateTime.TryParse(form["compled_date " + i], out dtValue))
                        {
                            objITModel.compled_date = dtValue;
                        }

                        objIndList.lstInd.Add(objITModel);
                    }

                    if (objModel.FunAddPerformInduction(objModel, objIndList))
                    {
                        TempData["Successdata"] = "Added Perform Induction Training details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPerformInduction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("PerformInductionList",new { id_induction=objModel.id_induction});
        }

        [AllowAnonymous]
        public ActionResult PerformInductionList()
        {
            string sid_induction = Request.QueryString["id_induction"];
            if (sid_induction != null && sid_induction != "")
            {
            InductionTrainingModelsList objIndList = new InductionTrainingModelsList();
            objIndList.lstInd = new List<InductionTrainingModels>();

            InductionTrainingModels objModels = new InductionTrainingModels();
            try
            {
                string sSqlstmt = "select a.id_induction, ref_no, division, department, location,training_topic, planned_by, start_date,  end_date,"
                    + "plan_notifiedto, priority, a.employee_id as allemployee_id, b.employee_id,b.final_status,completion_date,effectness,suggestion," +
                    "further_training,final_notifyto from t_induction_training a, t_induction_training_trans b where a.id_induction=b.id_induction and a.id_induction='"+ sid_induction +"' and a.active=1 and b.active=1";

                DataSet dsIndList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsIndList.Tables.Count > 0 && dsIndList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsIndList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objModels = new InductionTrainingModels
                            {
                                id_induction = dsIndList.Tables[0].Rows[i]["id_induction"].ToString(),
                                ref_no = dsIndList.Tables[0].Rows[i]["ref_no"].ToString(),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsIndList.Tables[0].Rows[i]["division"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsIndList.Tables[0].Rows[i]["department"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsIndList.Tables[0].Rows[i]["location"].ToString()),
                                training_topic = objGlobaldata.GetTrainingTopicById(dsIndList.Tables[0].Rows[i]["training_topic"].ToString()),
                                employee_id = (dsIndList.Tables[0].Rows[i]["employee_id"].ToString()),
                                employee_Name = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[i]["employee_id"].ToString()),
                                priority = objModels.GetInductionPriorityById(dsIndList.Tables[0].Rows[i]["priority"].ToString()),
                                planned_by = (dsIndList.Tables[0].Rows[i]["planned_by"].ToString()),

                                final_status = objModels.GetInductionStatusById(dsIndList.Tables[0].Rows[i]["final_status"].ToString()),
                                effectness = dsIndList.Tables[0].Rows[i]["effectness"].ToString(),
                                suggestion = dsIndList.Tables[0].Rows[i]["suggestion"].ToString(),
                                further_training = dsIndList.Tables[0].Rows[i]["further_training"].ToString(),
                                final_notifyto = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[i]["final_notifyto"].ToString())
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[i]["start_date"].ToString(), out dtValue))
                            {
                                objModels.start_date = dtValue;
                            }
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[i]["end_date"].ToString(), out dtValue))
                            {
                                objModels.end_date = dtValue;
                            }
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[i]["completion_date"].ToString(), out dtValue))
                            {
                                objModels.completion_date = dtValue;
                            }

                            objIndList.lstInd.Add(objModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PerformInductionList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformInductionList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objIndList.lstInd.ToList());
          }
            return RedirectToAction("InductionList");
        }
                
        [AllowAnonymous]
        public ActionResult PerformInductionDetails()
        {
            string sid_induction = Request.QueryString["id_induction"];
            string semployee_id = Request.QueryString["employee_id"];
            ViewBag.semployee_Name = objGlobaldata.GetMultiHrEmpNameById(semployee_id);
            ViewBag.semployee_id = semployee_id;
            if (sid_induction != null && sid_induction != "" && semployee_id != "" && semployee_id != null)
            {
              InductionTrainingModels objModels = new InductionTrainingModels();
                try
                {
                    string sSqlstmt = "select a.id_induction, ref_no, division, department, location,training_topic, planned_by, start_date,  end_date,"
                        + "plan_notifiedto, priority, a.employee_id as allemployee_id, b.employee_id,b.final_status,completion_date,effectness,suggestion," +
                        "further_training,final_notifyto from t_induction_training a, t_induction_training_trans b where a.id_induction=b.id_induction and a.id_induction='" + sid_induction + "' and  b.employee_id= '" + semployee_id + "' and a.active=1 and b.active=1";

                    DataSet dsIndList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsIndList.Tables.Count > 0 && dsIndList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objModels = new InductionTrainingModels
                            {
                                id_induction = dsIndList.Tables[0].Rows[0]["id_induction"].ToString(),
                                ref_no = dsIndList.Tables[0].Rows[0]["ref_no"].ToString(),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsIndList.Tables[0].Rows[0]["division"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsIndList.Tables[0].Rows[0]["department"].ToString()),
                                location =objGlobaldata.GetDivisionLocationById(dsIndList.Tables[0].Rows[0]["location"].ToString()),
                                training_topic = objGlobaldata.GetTrainingTopicById(dsIndList.Tables[0].Rows[0]["training_topic"].ToString()),
                                employee_id = (dsIndList.Tables[0].Rows[0]["allemployee_id"].ToString()),
                                employee_Name = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["allemployee_id"].ToString()),
                                priority = objModels.GetInductionPriorityById(dsIndList.Tables[0].Rows[0]["priority"].ToString()),
                                plan_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["plan_notifiedto"].ToString()),
                                planned_by = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["planned_by"].ToString()),

                                final_status = objModels.GetInductionStatusById(dsIndList.Tables[0].Rows[0]["final_status"].ToString()),
                                effectness = objModels.GetInductionEffectivenessById(dsIndList.Tables[0].Rows[0]["effectness"].ToString()),
                                suggestion = dsIndList.Tables[0].Rows[0]["suggestion"].ToString(),
                                further_training = dsIndList.Tables[0].Rows[0]["further_training"].ToString(),
                                final_notifyto = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["final_notifyto"].ToString())
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["start_date"].ToString(), out dtValue))
                            {
                                objModels.start_date = dtValue;
                            }
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["end_date"].ToString(), out dtValue))
                            {
                                objModels.end_date = dtValue;
                            }
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["completion_date"].ToString(), out dtValue))
                            {
                                objModels.completion_date = dtValue;
                            }

                            InductionTrainingModelsList objIndList = new InductionTrainingModelsList();
                            objIndList.lstInd = new List<InductionTrainingModels>();

                            string sSqlstmt1 = "select id_material,id_induction, employee_id, mat_description, upload,traing_start_date,comments, " +
                                "`explain`,compled_date,notify_to from t_induction_training_material where id_induction='" + sid_induction + "' and employee_id ='" + semployee_id + "' and active=1";

                            DataSet dsIndList1 = objGlobaldata.Getdetails(sSqlstmt1);

                            if (dsIndList1.Tables.Count > 0 && dsIndList1.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsIndList1.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        InductionTrainingModels objIndModels = new InductionTrainingModels
                                        {
                                            id_material = dsIndList1.Tables[0].Rows[i]["id_material"].ToString(),
                                            mat_description = dsIndList1.Tables[0].Rows[i]["mat_description"].ToString(),
                                            upload = dsIndList1.Tables[0].Rows[i]["upload"].ToString(),
                                            comments = dsIndList1.Tables[0].Rows[i]["comments"].ToString(),
                                            explain = dsIndList1.Tables[0].Rows[i]["explain"].ToString(),
                                            notify_to =objGlobaldata.GetMultiHrEmpNameById(dsIndList1.Tables[0].Rows[i]["notify_to"].ToString()),
                                        };

                                        if (DateTime.TryParse(dsIndList1.Tables[0].Rows[i]["traing_start_date"].ToString(), out dtValue))
                                        {
                                            objIndModels.traing_start_date = dtValue;
                                        }
                                        if (DateTime.TryParse(dsIndList1.Tables[0].Rows[i]["compled_date"].ToString(), out dtValue))
                                        {
                                            objIndModels.compled_date = dtValue;
                                        }
                                        objIndList.lstInd.Add(objIndModels);
                                        ViewBag.objMatList = objIndList;
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in AddPerformInduction: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PerformInductionDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in PerformInductionDetails: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                return View(objModels);
            }
            return RedirectToAction("InductionList");
        }

        [AllowAnonymous]
        public ActionResult PerformInductionPdf(FormCollection form)
        {
            string sid_induction = form["id_induction"];
            string semployee_id = form["employee_id"];
            ViewBag.semployee_Name = objGlobaldata.GetMultiHrEmpNameById(semployee_id);
            if (sid_induction != null && sid_induction != "" && semployee_id != "" && semployee_id != null)
            {
                InductionTrainingModels objModels = new InductionTrainingModels();
                try
                {
                    string sSqlstmt = "select a.id_induction, ref_no, division, department, location,training_topic, planned_by, start_date,  end_date,"
                        + "plan_notifiedto, priority, a.employee_id as allemployee_id, b.employee_id,b.final_status,completion_date,effectness,suggestion," +
                        "further_training,final_notifyto from t_induction_training a, t_induction_training_trans b where a.id_induction=b.id_induction and a.id_induction='" + sid_induction + "' and  b.employee_id= '" + semployee_id + "' and a.active=1 and b.active=1";

                    DataSet dsIndList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsIndList.Tables.Count > 0 && dsIndList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objModels = new InductionTrainingModels
                            {
                                id_induction = dsIndList.Tables[0].Rows[0]["id_induction"].ToString(),
                                ref_no = dsIndList.Tables[0].Rows[0]["ref_no"].ToString(),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsIndList.Tables[0].Rows[0]["division"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsIndList.Tables[0].Rows[0]["department"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsIndList.Tables[0].Rows[0]["location"].ToString()),
                                training_topic = objGlobaldata.GetTrainingTopicById(dsIndList.Tables[0].Rows[0]["training_topic"].ToString()),
                                // employee_id = (dsIndList.Tables[0].Rows[0]["employee_id"].ToString()),
                                employee_Name = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["allemployee_id"].ToString()),
                                priority = objModels.GetInductionPriorityById(dsIndList.Tables[0].Rows[0]["priority"].ToString()),
                                plan_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["plan_notifiedto"].ToString()),
                                planned_by = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["planned_by"].ToString()),

                                final_status = objModels.GetInductionStatusById(dsIndList.Tables[0].Rows[0]["final_status"].ToString()),
                                effectness = objModels.GetInductionEffectivenessById(dsIndList.Tables[0].Rows[0]["effectness"].ToString()),
                                suggestion = dsIndList.Tables[0].Rows[0]["suggestion"].ToString(),
                                further_training = dsIndList.Tables[0].Rows[0]["further_training"].ToString(),
                                final_notifyto = objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["final_notifyto"].ToString())
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["start_date"].ToString(), out dtValue))
                            {
                                objModels.start_date = dtValue;
                            }
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["end_date"].ToString(), out dtValue))
                            {
                                objModels.end_date = dtValue;
                            }
                            if (DateTime.TryParse(dsIndList.Tables[0].Rows[0]["completion_date"].ToString(), out dtValue))
                            {
                                objModels.completion_date = dtValue;
                            }

                            InductionTrainingModelsList objIndList = new InductionTrainingModelsList();
                            objIndList.lstInd = new List<InductionTrainingModels>();

                            string sSqlstmt1 = "select id_material,id_induction, employee_id, mat_description, upload,traing_start_date,comments, " +
                                "`explain`,compled_date,notify_to from t_induction_training_material where id_induction='" + sid_induction + "' and employee_id ='" + semployee_id + "' and active=1";

                            DataSet dsIndList1 = objGlobaldata.Getdetails(sSqlstmt1);

                            if (dsIndList1.Tables.Count > 0 && dsIndList1.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsIndList1.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        InductionTrainingModels objIndModels = new InductionTrainingModels
                                        {
                                            id_material = dsIndList1.Tables[0].Rows[i]["id_material"].ToString(),
                                            mat_description = dsIndList1.Tables[0].Rows[i]["mat_description"].ToString(),
                                            upload = dsIndList1.Tables[0].Rows[i]["upload"].ToString(),
                                            comments = dsIndList1.Tables[0].Rows[i]["comments"].ToString(),
                                            explain = dsIndList1.Tables[0].Rows[i]["explain"].ToString(),
                                            notify_to = objGlobaldata.GetMultiHrEmpNameById(dsIndList1.Tables[0].Rows[i]["notify_to"].ToString()),
                                        };

                                        if (DateTime.TryParse(dsIndList1.Tables[0].Rows[i]["traing_start_date"].ToString(), out dtValue))
                                        {
                                            objIndModels.traing_start_date = dtValue;
                                        }
                                        if (DateTime.TryParse(dsIndList1.Tables[0].Rows[i]["compled_date"].ToString(), out dtValue))
                                        {
                                            objIndModels.compled_date = dtValue;
                                        }
                                        objIndList.lstInd.Add(objIndModels);
                                        ViewBag.objMatList = objIndList;

                                        CompanyModels objCompany = new CompanyModels();
                                        dsIndList = objCompany.GetCompanyDetailsForReport(dsIndList);
                                        dsIndList = objGlobaldata.GetReportDetails(dsIndList, objModels.ref_no, objGlobaldata.GetCurrentUserSession().empid, "TRAINING REPORT");
                                        ViewBag.CompanyInfo = dsIndList;

                                        ViewBag.TrainList=objModels;
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in PerformInductionPdf: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PerformInductionPdf: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in PerformInductionPdf: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }               
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("PerformInductionPdf")
            {
                //FileName = "InductionReport.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [AllowAnonymous]
        public JsonResult PerformInductionDelete(FormCollection form)
        {
            try
            {

                if (form["id_induction"] != null && form["id_induction"] != ""&& form["employee_id"] != null && form["employee_id"] != "")
                {

                    InductionTrainingModels Doc = new InductionTrainingModels();
                    string sid_induction = form["id_induction"];
                    string semployee_id = form["employee_id"];

                    if (Doc.FunInductionTransDelete(sid_induction, semployee_id))
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
                objGlobaldata.AddFunctionalLog("Exception in PerformInductionDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase upload = Request.Files;
            InductionTrainingModels obj = new InductionTrainingModels();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + sFilename);
                obj.upload = obj.upload + "," + "~/DataUpload/MgmtDocs/Training/" + sFilename;

            }
            obj.upload = obj.upload.Trim(',');
            return Json(obj.upload);
        }
        
    }
}