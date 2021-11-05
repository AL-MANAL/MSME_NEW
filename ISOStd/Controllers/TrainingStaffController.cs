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
    public class TrainingStaffController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public TrainingStaffController()
        {
            ViewBag.Menutype = "Employee";
            ViewBag.SubMenutype = "Staff_Training";
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddTrainingStaff()
        {
            try
            {
                Initilization();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingStaff: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        public void Initilization()
        {
            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            ViewBag.DeptHead = objGlobaldata.GetDeptHeadList();
            ViewBag.Acceptance = objGlobaldata.GetConstantValue("TrainingStaff");
            ViewBag.TrainingType = objGlobaldata.GetDropdownList("Training Topic");
            ViewBag.Citicality = objGlobaldata.GetDropdownList("Training Criticality");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddTrainingStaff(FormCollection form, TrainingStaffModels objMdl)
        {
            try
            {
                DateTime dateValue;
                               
                objMdl.dept_head = form["dept_head"];

                if (form["date_taining"] != null && DateTime.TryParse(form["date_taining"], out dateValue) == true)
                {
                    objMdl.date_taining = dateValue;
                }

                TrainingStaffModelsList objList = new TrainingStaffModelsList();
                objList.TrainList = new List<TrainingStaffModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    TrainingStaffModels objTrainingMdl = new TrainingStaffModels();
                    if (form["training_type" + i] != null)
                    {
                        objTrainingMdl.training_type = form["training_type" + i];
                        objTrainingMdl.justification = form["justification" + i];
                        objTrainingMdl.criticality = form["criticality" + i];
                        //if (DateTime.TryParse(form["scheduled_date" + i], out dateValue) == true)
                        //{
                        //    objTrainingMdl.scheduled_date = dateValue;
                        //}
                        objList.TrainList.Add(objTrainingMdl);
                    }
                }

                if (objMdl.FunAddTrainingStaff(objMdl, objList))
                {
                    TempData["Successdata"] = "Training Added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainingStaff: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingStaffList");
        }

        [AllowAnonymous]
        public ActionResult TrainingStaffList(FormCollection form, int? page, string branch_name)
        {
            TrainingStaffModelsList objList = new TrainingStaffModelsList();
            objList.TrainList = new List<TrainingStaffModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_training,employee,date_taining,dept_head,comments,comment_head from t_training_staff where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + " order by id_training desc";

                DataSet dsTrainingList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsTrainingList.Tables.Count > 0 && dsTrainingList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsTrainingList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            TrainingStaffModels objTrainingMdl = new TrainingStaffModels
                            {
                                id_training = dsTrainingList.Tables[0].Rows[i]["id_training"].ToString(),
                                employee = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[i]["employee"].ToString()),
                                dept_head = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[i]["dept_head"].ToString()),
                                comments = (dsTrainingList.Tables[0].Rows[i]["comments"].ToString()),
                                comment_head = (dsTrainingList.Tables[0].Rows[i]["comment_head"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsTrainingList.Tables[0].Rows[i]["date_taining"].ToString() != ""
                               && DateTime.TryParse(dsTrainingList.Tables[0].Rows[i]["date_taining"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.date_taining = dtDocDate;
                            }

                            objList.TrainList.Add(objTrainingMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingStaffList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingStaffList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objList.TrainList.ToList());
        }

        [AllowAnonymous]
        public ActionResult TrainingStaffEdit()
        {
            TrainingStaffModels objTrainingMdl = new TrainingStaffModels();
            try
            {
               
                if (Request.QueryString["id_training"] != "" && Request.QueryString["id_training"] != null)
                {
                    string sid_training = Request.QueryString["id_training"];
                    Initilization();
                    string sSqlstmt = "select id_training,employee,date_taining,dept_head,comments from t_training_staff where id_training = '" + sid_training + "'";
                    DataSet dsTrainingList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTrainingList.Tables.Count > 0 && dsTrainingList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                             objTrainingMdl = new TrainingStaffModels
                            {
                                id_training = dsTrainingList.Tables[0].Rows[0]["id_training"].ToString(),
                                employee = /*objGlobaldata.GetMultiHrEmpNameById*/(dsTrainingList.Tables[0].Rows[0]["employee"].ToString()),
                                dept_head = /*objGlobaldata.GetMultiHrEmpNameById*/(dsTrainingList.Tables[0].Rows[0]["dept_head"].ToString()),
                                comments = (dsTrainingList.Tables[0].Rows[0]["comments"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString() != ""
                               && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.date_taining = dtDocDate;
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingStaffEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                    TrainingStaffModelsList objList = new TrainingStaffModelsList();
                    objList.TrainList = new List<TrainingStaffModels>();

                    string Ssqlstmt1 = "select id_training_trans,id_training,training_type,justification,scheduled_date,criticality " +
                        "from t_training_staff_trans where id_training = '" + sid_training + "'";
                    DataSet dsTrainList = objGlobaldata.Getdetails(Ssqlstmt1);
                    if(dsTrainList.Tables.Count>0 && dsTrainList.Tables[0].Rows.Count>0)
                    {
                        try
                        {
                            for(int i = 0; i < dsTrainList.Tables[0].Rows.Count; i++)
                            {
                                TrainingStaffModels objtrain = new TrainingStaffModels
                                {
                                    id_training_trans=dsTrainList.Tables[0].Rows[i]["id_training_trans"].ToString(),
                                    training_type = dsTrainList.Tables[0].Rows[i]["training_type"].ToString(),
                                    criticality = dsTrainList.Tables[0].Rows[i]["criticality"].ToString(),
                                    justification = dsTrainList.Tables[0].Rows[i]["justification"].ToString(),
                                };
                                //DateTime dtDocDate;
                                //if (dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString() != ""
                                //   && DateTime.TryParse(dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString(), out dtDocDate))
                                //{
                                //    objtrain.scheduled_date = dtDocDate;
                                //}
                                objList.TrainList.Add(objtrain);
                            }
                            ViewBag.TrainDetails = objList;
                        }
                        catch(Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingStaffEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
           
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingStaffEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTrainingMdl);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult TrainingStaffEdit(FormCollection form, TrainingStaffModels objMdl)
        {
            try
            {
                DateTime dateValue;

                objMdl.dept_head = form["dept_head"];

                if (form["date_taining"] != null && DateTime.TryParse(form["date_taining"], out dateValue) == true)
                {
                    objMdl.date_taining = dateValue;
                }

                TrainingStaffModelsList objList = new TrainingStaffModelsList();
                objList.TrainList = new List<TrainingStaffModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    TrainingStaffModels objTrainingMdl = new TrainingStaffModels();
                    if (form["training_type " + i] != null)
                    {
                        objTrainingMdl.training_type = form["training_type " + i];
                        objTrainingMdl.criticality = form["criticality " + i];
                        objTrainingMdl.justification = form["justification " + i];

                        //if (DateTime.TryParse(form["scheduled_date " + i], out dateValue) == true)
                        //{
                        //    objTrainingMdl.scheduled_date = dateValue;
                        //}
                        objList.TrainList.Add(objTrainingMdl);
                    }
                }

                if (objMdl.FunUpdateTrainingStaff(objMdl, objList))
                {
                    TempData["Successdata"] = "Training Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingStaffEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingStaffList");
        }

        [AllowAnonymous]
        public ActionResult TrainingDeptHeadEdit()
        {
            TrainingStaffModels objTrainingMdl = new TrainingStaffModels();
            try
            {

                if (Request.QueryString["id_training"] != "" && Request.QueryString["id_training"] != null)
                {
                    string sid_training = Request.QueryString["id_training"];
                    Initilization();
                    string sSqlstmt = "select id_training,employee,date_taining,dept_head,comments,comment_head from t_training_staff where id_training = '" + sid_training + "'";
                    DataSet dsTrainingList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTrainingList.Tables.Count > 0 && dsTrainingList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objTrainingMdl = new TrainingStaffModels
                            {
                                id_training = dsTrainingList.Tables[0].Rows[0]["id_training"].ToString(),
                                employee = /*objGlobaldata.GetMultiHrEmpNameById*/(dsTrainingList.Tables[0].Rows[0]["employee"].ToString()),
                                dept_head = /*objGlobaldata.GetMultiHrEmpNameById*/(dsTrainingList.Tables[0].Rows[0]["dept_head"].ToString()),
                                comments = (dsTrainingList.Tables[0].Rows[0]["comments"].ToString()),
                                comment_head = (dsTrainingList.Tables[0].Rows[0]["comment_head"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString() != ""
                               && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.date_taining = dtDocDate;
                            }

                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingStaffEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                    TrainingStaffModelsList objList = new TrainingStaffModelsList();
                    objList.TrainList = new List<TrainingStaffModels>();

                    string Ssqlstmt1 = "select id_training_trans,id_training,training_type,justification,scheduled_date,acceptance,suggestion,criticality,scheduled_date " +
                        "from t_training_staff_trans where id_training = '" + sid_training + "'";
                    DataSet dsTrainList = objGlobaldata.Getdetails(Ssqlstmt1);
                    if (dsTrainList.Tables.Count > 0 && dsTrainList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            for (int i = 0; i < dsTrainList.Tables[0].Rows.Count; i++)
                            {
                                TrainingStaffModels objtrain = new TrainingStaffModels
                                {
                                    id_training_trans = dsTrainList.Tables[0].Rows[i]["id_training_trans"].ToString(),
                                    training_type = dsTrainList.Tables[0].Rows[i]["training_type"].ToString(),
                                    criticality = dsTrainList.Tables[0].Rows[i]["criticality"].ToString(),
                                    justification = dsTrainList.Tables[0].Rows[i]["justification"].ToString(),
                                    acceptance = dsTrainList.Tables[0].Rows[i]["acceptance"].ToString(),
                                    suggestion = dsTrainList.Tables[0].Rows[i]["suggestion"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString() != ""
                                   && DateTime.TryParse(dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString(), out dtDocDate))
                                {
                                    objtrain.scheduled_date = dtDocDate;
                                }
                                objList.TrainList.Add(objtrain);
                            }
                            ViewBag.TrainDetails = objList;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingDeptHeadEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingDeptHeadEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTrainingMdl);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult TrainingDeptHeadEdit(FormCollection form, TrainingStaffModels objMdl)
        {
            try
            {
                DateTime dateValue;

                objMdl.dept_head = form["dept_head"];

                if (form["date_taining"] != null && DateTime.TryParse(form["date_taining"], out dateValue) == true)
                {
                    objMdl.date_taining = dateValue;
                }

                TrainingStaffModelsList objList = new TrainingStaffModelsList();
                objList.TrainList = new List<TrainingStaffModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    TrainingStaffModels objTrainingMdl = new TrainingStaffModels();
                    if (form["training_type " + i] != null)
                    {
                        objTrainingMdl.training_type = form["training_type " + i];
                        objTrainingMdl.criticality = form["criticality " + i];
                        objTrainingMdl.justification = form["justification " + i];
                        objTrainingMdl.acceptance = form["acceptance " + i];
                        objTrainingMdl.suggestion = form["suggestion " + i];
                        objTrainingMdl.scheduled_time = form["scheduled_time " + i];
                        if (DateTime.TryParse(form["scheduled_date " + i], out dateValue) == true)
                        {
                            objTrainingMdl.scheduled_date = dateValue;

                        }
                        objList.TrainList.Add(objTrainingMdl);
                    }
                }

                if (objMdl.FunUpdateTrainingDept_head(objMdl, objList))
                {
                    TempData["Successdata"] = "Training Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingDeptHeadEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingStaffList");
        }
        
        [AllowAnonymous]
        public ActionResult TrainingStaffDetails()
        {
            TrainingStaffModels objTrainingMdl = new TrainingStaffModels();
            try
            {

                if (Request.QueryString["id_training"] != "" && Request.QueryString["id_training"] != null)
                {
                    string sid_training = Request.QueryString["id_training"];
                    string sSqlstmt = "select id_training,employee,date_taining,dept_head,comments,comment_head from t_training_staff where id_training = '" + sid_training + "'";
                    DataSet dsTrainingList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTrainingList.Tables.Count > 0 && dsTrainingList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objTrainingMdl = new TrainingStaffModels
                            {
                                id_training = dsTrainingList.Tables[0].Rows[0]["id_training"].ToString(),
                                employee = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["employee"].ToString()),
                                dept_head = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["dept_head"].ToString()),
                                comments = (dsTrainingList.Tables[0].Rows[0]["comments"].ToString()),
                                comment_head = (dsTrainingList.Tables[0].Rows[0]["comment_head"].ToString()),
                             };

                            DateTime dtDocDate;
                            if (dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString() != ""
                               && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.date_taining = dtDocDate;
                            }

                            string stmt = "Select division,Dept_Id,Designation" +
                            " from t_hr_employee where emp_no ='" + dsTrainingList.Tables[0].Rows[0]["employee"].ToString() + "'";
                            DataSet dsEmpList = objGlobaldata.Getdetails(stmt);
                            if (dsEmpList.Tables.Count > 0 && dsEmpList.Tables[0].Rows.Count > 0)
                            {
                                    objTrainingMdl.division = objGlobaldata.GetCompanyBranchNameById(dsEmpList.Tables[0].Rows[0]["division"].ToString());
                                    objTrainingMdl.dept = objGlobaldata.GetDeptNameById(dsEmpList.Tables[0].Rows[0]["Dept_Id"].ToString());
                                    objTrainingMdl.designation = dsEmpList.Tables[0].Rows[0]["Designation"].ToString();
                            }

                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingStaffEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                    TrainingStaffModelsList objList = new TrainingStaffModelsList();
                    objList.TrainList = new List<TrainingStaffModels>();

                    string Ssqlstmt1 = "select id_training_trans,id_training,training_type,justification,scheduled_date,acceptance,suggestion,criticality " +
                        "from t_training_staff_trans where id_training = '" + sid_training + "'";
                    DataSet dsTrainList = objGlobaldata.Getdetails(Ssqlstmt1);
                    if (dsTrainList.Tables.Count > 0 && dsTrainList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            for (int i = 0; i < dsTrainList.Tables[0].Rows.Count; i++)
                            {
                                TrainingStaffModels objtrain = new TrainingStaffModels
                                {
                                    id_training_trans = dsTrainList.Tables[0].Rows[i]["id_training_trans"].ToString(),
                                    training_type = dsTrainList.Tables[0].Rows[i]["training_type"].ToString(),
                                    justification = dsTrainList.Tables[0].Rows[i]["justification"].ToString(),
                                    acceptance = dsTrainList.Tables[0].Rows[i]["acceptance"].ToString(),
                                    suggestion = dsTrainList.Tables[0].Rows[i]["suggestion"].ToString(),
                                    criticality = dsTrainList.Tables[0].Rows[i]["criticality"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString() != ""
                                   && DateTime.TryParse(dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString(), out dtDocDate))
                                {
                                    objtrain.scheduled_date = dtDocDate;
                                }
                                objList.TrainList.Add(objtrain);
                            }
                            ViewBag.TrainDetails = objList;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingDeptHeadEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingStaffDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTrainingMdl);
        }

        [AllowAnonymous]
        public ActionResult TrainingStaffInfo(int id)
        {
            TrainingStaffModels objTrainingMdl = new TrainingStaffModels();
            try
            {
                if (id > 0)
                {
                    string sSqlstmt = "select id_training,employee,date_taining,dept_head,comments,comment_head from t_training_staff where id_training = '" + id + "'";
                    DataSet dsTrainingList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTrainingList.Tables.Count > 0 && dsTrainingList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objTrainingMdl = new TrainingStaffModels
                            {
                                id_training = dsTrainingList.Tables[0].Rows[0]["id_training"].ToString(),
                                employee = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["employee"].ToString()),
                                dept_head = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["dept_head"].ToString()),
                                comments = (dsTrainingList.Tables[0].Rows[0]["comments"].ToString()),
                                comment_head = (dsTrainingList.Tables[0].Rows[0]["comment_head"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString() != ""
                               && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.date_taining = dtDocDate;
                            }

                            string stmt = "Select division,Dept_Id,Designation" +
                            " from t_hr_employee where emp_no ='" + dsTrainingList.Tables[0].Rows[0]["employee"].ToString() + "'";
                            DataSet dsEmpList = objGlobaldata.Getdetails(stmt);
                            if (dsEmpList.Tables.Count > 0 && dsEmpList.Tables[0].Rows.Count > 0)
                            {
                                objTrainingMdl.division = objGlobaldata.GetCompanyBranchNameById(dsEmpList.Tables[0].Rows[0]["division"].ToString());
                                objTrainingMdl.dept = objGlobaldata.GetDeptNameById(dsEmpList.Tables[0].Rows[0]["Dept_Id"].ToString());
                                objTrainingMdl.designation = dsEmpList.Tables[0].Rows[0]["Designation"].ToString();
                            }

                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingStaffEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                    //TrainingStaffModelsList objList = new TrainingStaffModelsList();
                    //objList.TrainList = new List<TrainingStaffModels>();

                    //string Ssqlstmt1 = "select id_training_trans,id_training,training_type,justification,scheduled_date,acceptance,suggestion " +
                    //    "from t_training_staff_trans where id_training = '" + id + "'";
                    //DataSet dsTrainList = objGlobaldata.Getdetails(Ssqlstmt1);
                    //if (dsTrainList.Tables.Count > 0 && dsTrainList.Tables[0].Rows.Count > 0)
                    //{
                    //    try
                    //    {
                    //        for (int i = 0; i < dsTrainList.Tables[0].Rows.Count; i++)
                    //        {
                    //            TrainingStaffModels objtrain = new TrainingStaffModels
                    //            {
                    //                id_training_trans = dsTrainList.Tables[0].Rows[i]["id_training_trans"].ToString(),
                    //                training_type = dsTrainList.Tables[0].Rows[i]["training_type"].ToString(),
                    //                justification = dsTrainList.Tables[0].Rows[i]["justification"].ToString(),
                    //                acceptance = dsTrainList.Tables[0].Rows[i]["acceptance"].ToString(),
                    //                suggestion = dsTrainList.Tables[0].Rows[i]["suggestion"].ToString(),
                    //            };
                    //            DateTime dtDocDate;
                    //            if (dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString() != ""
                    //               && DateTime.TryParse(dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString(), out dtDocDate))
                    //            {
                    //                objtrain.scheduled_date = dtDocDate;
                    //            }
                    //            objList.TrainList.Add(objtrain);
                    //        }
                    //        ViewBag.TrainDetails = objList;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        objGlobaldata.AddFunctionalLog("Exception in TrainingDeptHeadEdit: " + ex.ToString());
                    //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    //    }
                    //}
                }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingStaffInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTrainingMdl);
        }

        [AllowAnonymous]
        public ActionResult TrainingStaffPDF(FormCollection form)
        {
            TrainingStaffModels objTrainingMdl = new TrainingStaffModels();
            try
            {

                if (form["id_training"] != "" && form["id_training"] != null)
                {
                    string sid_training = form["id_training"];
                    string sSqlstmt = "select id_training,employee,date_taining,dept_head,comments,comment_head from t_training_staff where id_training = '" + sid_training + "'";
                    DataSet dsTrainingList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTrainingList.Tables.Count > 0 && dsTrainingList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objTrainingMdl = new TrainingStaffModels
                            {
                                id_training = dsTrainingList.Tables[0].Rows[0]["id_training"].ToString(),
                                employee = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["employee"].ToString()),
                                dept_head = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["dept_head"].ToString()),
                                comments = (dsTrainingList.Tables[0].Rows[0]["comments"].ToString()),
                                comment_head = (dsTrainingList.Tables[0].Rows[0]["comment_head"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString() != ""
                               && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.date_taining = dtDocDate;
                            }

                            CompanyModels objCompany = new CompanyModels();
                            dsTrainingList = objCompany.GetCompanyDetailsForReport(dsTrainingList);

                            string loggedby = objGlobaldata.GetCurrentUserSession().empid;
                            dsTrainingList = objGlobaldata.GetReportDetails(dsTrainingList, objTrainingMdl.id_training, loggedby, "STAFF TRAINING REPORT");
                            ViewBag.CompanyInfo = dsTrainingList;

                            string stmt = "Select division,Dept_Id,Designation" +
                            " from t_hr_employee where emp_no ='" + dsTrainingList.Tables[0].Rows[0]["employee"].ToString() + "'";
                            DataSet dsEmpList = objGlobaldata.Getdetails(stmt);
                            if (dsEmpList.Tables.Count > 0 && dsEmpList.Tables[0].Rows.Count > 0)
                            {
                                objTrainingMdl.division = objGlobaldata.GetCompanyBranchNameById(dsEmpList.Tables[0].Rows[0]["division"].ToString());
                                objTrainingMdl.dept = objGlobaldata.GetDeptNameById(dsEmpList.Tables[0].Rows[0]["Dept_Id"].ToString());
                                objTrainingMdl.designation = dsEmpList.Tables[0].Rows[0]["Designation"].ToString();
                            }
                            ViewBag.Training = objTrainingMdl;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingStaffPDF: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                    TrainingStaffModelsList objList = new TrainingStaffModelsList();
                    objList.TrainList = new List<TrainingStaffModels>();

                    string Ssqlstmt1 = "select id_training_trans,id_training,training_type,justification,scheduled_date,acceptance,suggestion " +
                        "from t_training_staff_trans where id_training = '" + sid_training + "'";
                    DataSet dsTrainList = objGlobaldata.Getdetails(Ssqlstmt1);
                    if (dsTrainList.Tables.Count > 0 && dsTrainList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            for (int i = 0; i < dsTrainList.Tables[0].Rows.Count; i++)
                            {
                                TrainingStaffModels objtrain = new TrainingStaffModels
                                {
                                    id_training_trans = dsTrainList.Tables[0].Rows[i]["id_training_trans"].ToString(),
                                    training_type = dsTrainList.Tables[0].Rows[i]["training_type"].ToString(),
                                    justification = dsTrainList.Tables[0].Rows[i]["justification"].ToString(),
                                    acceptance = dsTrainList.Tables[0].Rows[i]["acceptance"].ToString(),
                                    suggestion = dsTrainList.Tables[0].Rows[i]["suggestion"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString() != ""
                                   && DateTime.TryParse(dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString(), out dtDocDate))
                                {
                                    objtrain.scheduled_date = dtDocDate;
                                }
                                objList.TrainList.Add(objtrain);
                            }
                            ViewBag.TrainDetails = objList;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingStaffPDF: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingStaffPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("TrainingStaffPDF")
            {
                //FileName = "TrainingRpt.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [AllowAnonymous]
        public ActionResult GenerateTrainingReport()
        {
            TrainingStaffModels objTrainingMdl = new TrainingStaffModels();
            try
            {

                if (Request.QueryString["id_training"] != "" && Request.QueryString["id_training"] != null)
                {
                    string sid_training = Request.QueryString["id_training"];
                    string sSqlstmt = "select id_training,employee,date_taining,dept_head,comments,comment_head,start_date,end_date,conducted_by,details," +
                        "upload,effective,reason,recommendation,traing_required,notify_to,training_status,due_date from t_training_staff where id_training = '" + sid_training + "'";
                    DataSet dsTrainingList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTrainingList.Tables.Count > 0 && dsTrainingList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objTrainingMdl = new TrainingStaffModels
                            {
                                id_training = dsTrainingList.Tables[0].Rows[0]["id_training"].ToString(),
                                employee = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["employee"].ToString()),
                                dept_head = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["dept_head"].ToString()),
                                comments = dsTrainingList.Tables[0].Rows[0]["comments"].ToString(),
                                comment_head = dsTrainingList.Tables[0].Rows[0]["comment_head"].ToString(),

                                conducted_by = dsTrainingList.Tables[0].Rows[0]["conducted_by"].ToString(),
                                details = dsTrainingList.Tables[0].Rows[0]["details"].ToString(),
                                upload = dsTrainingList.Tables[0].Rows[0]["upload"].ToString(),
                                effective = dsTrainingList.Tables[0].Rows[0]["effective"].ToString(),
                                reason = dsTrainingList.Tables[0].Rows[0]["reason"].ToString(),
                                recommendation = dsTrainingList.Tables[0].Rows[0]["recommendation"].ToString(),
                                traing_required = dsTrainingList.Tables[0].Rows[0]["traing_required"].ToString(),
                                notify_to = dsTrainingList.Tables[0].Rows[0]["notify_to"].ToString(),
                                training_status = dsTrainingList.Tables[0].Rows[0]["training_status"].ToString(),
                            };

                            DateTime dtDocDate;
                            if (dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString() != ""
                               && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.date_taining = dtDocDate;
                            }

                            if (dsTrainingList.Tables[0].Rows[0]["start_date"].ToString() != ""
                              && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["start_date"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.start_date = dtDocDate;
                            }
                            if (dsTrainingList.Tables[0].Rows[0]["end_date"].ToString() != ""
                              && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["end_date"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.end_date = dtDocDate;
                            }
                            if (dsTrainingList.Tables[0].Rows[0]["due_date"].ToString() != ""
                              && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["due_date"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.due_date = dtDocDate;
                            }
                            string stmt = "Select division,Dept_Id,Designation" +
                            " from t_hr_employee where emp_no ='" + dsTrainingList.Tables[0].Rows[0]["employee"].ToString() + "'";
                            DataSet dsEmpList = objGlobaldata.Getdetails(stmt);
                            if (dsEmpList.Tables.Count > 0 && dsEmpList.Tables[0].Rows.Count > 0)
                            {
                                objTrainingMdl.division = objGlobaldata.GetCompanyBranchNameById(dsEmpList.Tables[0].Rows[0]["division"].ToString());
                                objTrainingMdl.dept = objGlobaldata.GetDeptNameById(dsEmpList.Tables[0].Rows[0]["Dept_Id"].ToString());
                                objTrainingMdl.designation = dsEmpList.Tables[0].Rows[0]["Designation"].ToString();
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingStaffEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                    TrainingStaffModelsList objList = new TrainingStaffModelsList();
                    objList.TrainList = new List<TrainingStaffModels>();

                    string Ssqlstmt1 = "select id_training_trans,id_training,training_type,justification,scheduled_date,acceptance,suggestion " +
                        "from t_training_staff_trans where id_training = '" + sid_training + "'";
                    DataSet dsTrainList = objGlobaldata.Getdetails(Ssqlstmt1);
                    if (dsTrainList.Tables.Count > 0 && dsTrainList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            for (int i = 0; i < dsTrainList.Tables[0].Rows.Count; i++)
                            {
                                TrainingStaffModels objtrain = new TrainingStaffModels
                                {
                                    id_training_trans = dsTrainList.Tables[0].Rows[i]["id_training_trans"].ToString(),
                                    training_type = dsTrainList.Tables[0].Rows[i]["training_type"].ToString(),
                                    justification = dsTrainList.Tables[0].Rows[i]["justification"].ToString(),
                                    acceptance = dsTrainList.Tables[0].Rows[i]["acceptance"].ToString(),
                                    suggestion = dsTrainList.Tables[0].Rows[i]["suggestion"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString() != ""
                                   && DateTime.TryParse(dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString(), out dtDocDate))
                                {
                                    objtrain.scheduled_date = dtDocDate;
                                }
                                objList.TrainList.Add(objtrain);
                            }
                            ViewBag.TrainDetails = objList;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingDeptHeadEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                    ViewBag.Status = objGlobaldata.GetDropdownList("Staff Training Status");
                    ViewBag.Effectness = objGlobaldata.GetDropdownList("Staff Training Effectiveness");
                    ViewBag.DeptHead = objGlobaldata.GetDeptHeadList();
                    ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateTrainingReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTrainingMdl);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GenerateTrainingReport(FormCollection form, TrainingStaffModels objTraining, IEnumerable<HttpPostedFileBase> upload)
        {
            
            try
            {
                DateTime dateValue;

                string QCDelete = Request.Form["QCDocsValselectall"];

                objTraining.conducted_by = form["conducted_by"];
                objTraining.notify_to = form["notify_to"];            

                if (DateTime.TryParse(form["start_date"], out dateValue) == true)
                {
                    objTraining.start_date = dateValue;
                }

                if (DateTime.TryParse(form["end_date"], out dateValue) == true)
                {
                    objTraining.end_date = dateValue;
                }
                if (DateTime.TryParse(form["due_date"], out dateValue) == true)
                {
                    objTraining.due_date = dateValue;
                }
                HttpPostedFileBase files = Request.Files[0];

                if (upload != null && files.ContentLength > 0)
                {
                    objTraining.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                            string sFilename = "Training" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objTraining.upload = objTraining.upload + "," + "~/DataUpload/MgmtDocs/Training/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in GenerateTrainingReport-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objTraining.upload = objTraining.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objTraining.upload = objTraining.upload + "," + form["QCDocsVal"];
                    objTraining.upload = objTraining.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objTraining.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objTraining.upload = null;
                }
                
                if (objTraining.FunUpdateStaffTrainingReport(objTraining))
                {
                    TempData["Successdata"] = "Staff training Report updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateTrainingReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingStaffList");
        }

        [AllowAnonymous]
        public ActionResult GenerateTrainingReportPDF(FormCollection form)
        {
            TrainingStaffModels objTrainingMdl = new TrainingStaffModels();
            try
            {

                if (form["id_training"] != "" && form["id_training"] != null)
                {
                    string sid_training = form["id_training"];
                    string sSqlstmt = "select id_training,employee,date_taining,dept_head,comments,comment_head,start_date,end_date,conducted_by,details," +
                        "upload,effective,reason,recommendation,traing_required,notify_to,training_status,due_date from t_training_staff where id_training = '" + sid_training + "'";
                    DataSet dsTrainingList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTrainingList.Tables.Count > 0 && dsTrainingList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objTrainingMdl = new TrainingStaffModels
                            {
                                id_training = dsTrainingList.Tables[0].Rows[0]["id_training"].ToString(),
                                employee = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["employee"].ToString()),
                                dept_head = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["dept_head"].ToString()),
                                comments = dsTrainingList.Tables[0].Rows[0]["comments"].ToString(),
                                comment_head = dsTrainingList.Tables[0].Rows[0]["comment_head"].ToString(),

                                conducted_by = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["conducted_by"].ToString()),
                                details = dsTrainingList.Tables[0].Rows[0]["details"].ToString(),
                                upload = dsTrainingList.Tables[0].Rows[0]["upload"].ToString(),
                                effective = objGlobaldata.GetDropdownitemById(dsTrainingList.Tables[0].Rows[0]["effective"].ToString()),
                                reason = dsTrainingList.Tables[0].Rows[0]["reason"].ToString(),
                                recommendation = dsTrainingList.Tables[0].Rows[0]["recommendation"].ToString(),
                                traing_required = dsTrainingList.Tables[0].Rows[0]["traing_required"].ToString(),
                                notify_to = objGlobaldata.GetMultiHrEmpNameById(dsTrainingList.Tables[0].Rows[0]["notify_to"].ToString()),
                                training_status = objGlobaldata.GetDropdownitemById(dsTrainingList.Tables[0].Rows[0]["training_status"].ToString()),                                
                            };

                            DateTime dtDocDate;
                            if (dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString() != ""
                               && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["date_taining"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.date_taining = dtDocDate;
                            }

                            if (dsTrainingList.Tables[0].Rows[0]["start_date"].ToString() != ""
                              && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["start_date"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.start_date = dtDocDate;
                            }
                            if (dsTrainingList.Tables[0].Rows[0]["end_date"].ToString() != ""
                              && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["end_date"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.end_date = dtDocDate;
                            }
                            if (dsTrainingList.Tables[0].Rows[0]["due_date"].ToString() != ""
                              && DateTime.TryParse(dsTrainingList.Tables[0].Rows[0]["due_date"].ToString(), out dtDocDate))
                            {
                                objTrainingMdl.due_date = dtDocDate;
                            }
                            CompanyModels objCompany = new CompanyModels();
                            dsTrainingList = objCompany.GetCompanyDetailsForReport(dsTrainingList);

                            string loggedby = objGlobaldata.GetCurrentUserSession().empid;
                            dsTrainingList = objGlobaldata.GetReportDetails(dsTrainingList, objTrainingMdl.id_training, loggedby, "STAFF TRAINING REPORT");
                            ViewBag.CompanyInfo = dsTrainingList;

                            string stmt = "Select division,Dept_Id,Designation" +
                            " from t_hr_employee where emp_no ='" + dsTrainingList.Tables[0].Rows[0]["employee"].ToString() + "'";
                            DataSet dsEmpList = objGlobaldata.Getdetails(stmt);
                            if (dsEmpList.Tables.Count > 0 && dsEmpList.Tables[0].Rows.Count > 0)
                            {
                                objTrainingMdl.division = objGlobaldata.GetCompanyBranchNameById(dsEmpList.Tables[0].Rows[0]["division"].ToString());
                                objTrainingMdl.dept = objGlobaldata.GetDeptNameById(dsEmpList.Tables[0].Rows[0]["Dept_Id"].ToString());
                                objTrainingMdl.designation = dsEmpList.Tables[0].Rows[0]["Designation"].ToString();
                            }

                            ViewBag.Training = objTrainingMdl;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in GenerateTrainingReportPDF: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                    //TrainingStaffModelsList objList = new TrainingStaffModelsList();
                    //objList.TrainList = new List<TrainingStaffModels>();

                    //string Ssqlstmt1 = "select id_training_trans,id_training,training_type,justification,scheduled_date,acceptance,suggestion " +
                    //    "from t_training_staff_trans where id_training = '" + sid_training + "'";
                    //DataSet dsTrainList = objGlobaldata.Getdetails(Ssqlstmt1);
                    //if (dsTrainList.Tables.Count > 0 && dsTrainList.Tables[0].Rows.Count > 0)
                    //{
                    //    try
                    //    {
                    //        for (int i = 0; i < dsTrainList.Tables[0].Rows.Count; i++)
                    //        {
                    //            TrainingStaffModels objtrain = new TrainingStaffModels
                    //            {
                    //                id_training_trans = dsTrainList.Tables[0].Rows[i]["id_training_trans"].ToString(),
                    //                training_type = dsTrainList.Tables[0].Rows[i]["training_type"].ToString(),
                    //                justification = dsTrainList.Tables[0].Rows[i]["justification"].ToString(),
                    //                acceptance = dsTrainList.Tables[0].Rows[i]["acceptance"].ToString(),
                    //                suggestion = dsTrainList.Tables[0].Rows[i]["suggestion"].ToString(),
                    //            };
                    //            DateTime dtDocDate;
                    //            if (dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString() != ""
                    //               && DateTime.TryParse(dsTrainList.Tables[0].Rows[i]["scheduled_date"].ToString(), out dtDocDate))
                    //            {
                    //                objtrain.scheduled_date = dtDocDate;
                    //            }
                    //            objList.TrainList.Add(objtrain);
                    //        }
                    //        ViewBag.TrainDetails = objList;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        objGlobaldata.AddFunctionalLog("Exception in TrainingDeptHeadEdit: " + ex.ToString());
                    //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    //    }
                    //}
                 }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateTrainingReportPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("GenerateTrainingReportPDF")
            {
                //FileName = "TrainingRpt.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [AllowAnonymous]
        public JsonResult TrainingStaffDelete(FormCollection form)
        {
            try
            {
                if (form["id_training"] != null && form["id_training"] != "")
                {
                    TrainingStaffModels Doc = new TrainingStaffModels();
                    string sid_training = form["id_training"];
                    if (Doc.FunDeleteTrainingStaff(sid_training))
                    {
                        TempData["Successdata"] = "Training deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in TrainingStaffDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public JsonResult FunGetEmpDetails(string EmpId)
        {
            EmployeeModels objMdl = new EmployeeModels();
            string stmt = "Select division as division,Dept_Id,Designation" +
                " from t_hr_employee where emp_no ='" + EmpId + "'";
            DataSet dsTrainingList = objGlobaldata.Getdetails(stmt);
            if(dsTrainingList.Tables.Count>0 && dsTrainingList.Tables[0].Rows.Count>0)
            {
                objMdl = new EmployeeModels
                {
                    Work_Location = objGlobaldata.GetCompanyBranchNameById(dsTrainingList.Tables[0].Rows[0]["division"].ToString()),
                    DeptID = objGlobaldata.GetDeptNameById(dsTrainingList.Tables[0].Rows[0]["Dept_Id"].ToString()),
                    Designation = dsTrainingList.Tables[0].Rows[0]["Designation"].ToString(),
                };
            }

            return Json(objMdl);
        }
    }
}