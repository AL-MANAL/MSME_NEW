using ISOStd.Filters;
using ISOStd.Models;
using PagedList;
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
    public class TrainingOrientationController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public TrainingOrientationController()
        {
            //ViewBag.Menutype = "Training Orientation";
            ViewBag.Menutype = "Employee";
            ViewBag.SubMenutype = "OrientationDetails";
        }

        [AllowAnonymous]
        public JsonResult TrainingOrientationDelete(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "TrainingOrientation";

                if (form["emp_orientation_id"] != null && form["emp_orientation_id"] != "")
                {
                    TrainingOrientationModels Orientation = new TrainingOrientationModels();
                    string semp_orientation_id = form["emp_orientation_id"];

                    if (Orientation.FunDeleteTrainingOrientation(semp_orientation_id))
                    {
                        TempData["Successdata"] = "Orientation details deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in TrainingOrientationDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult OrientationTopicDelete(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "TrainingOrientation";

                if (form["TrainingOrientation_Id"] != null && form["TrainingOrientation_Id"] != "")
                {
                    TrainingOrientationModels Orientation = new TrainingOrientationModels();
                    string sTrainingOrientation_Id = form["TrainingOrientation_Id"];

                    if (Orientation.FunDeleteTrainingOrientationTopic(sTrainingOrientation_Id))
                    {
                        TempData["Successdata"] = "Orientation Topic deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in OrientationTopicDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddTrainingOrientation(TrainingOrientationModels objOrientation, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                ViewBag.SubMenutype = "TrainingOrientation";
                objOrientation.branch = form["branch"];
                objOrientation.Department = form["Department"];
                objOrientation.Location = form["Location"];

                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/TrainingOrientation"), Path.GetFileName(file.FileName));
                        string sFilename = objOrientation.Topic + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objOrientation.DocUploadPath = "~/DataUpload/MgmtDocs/TrainingOrientation/" + sFilename;
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
                if (objOrientation.FunAddOrientation(objOrientation))
                {
                    TempData["Successdata"] = "Added Training Orientation Topic details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainingOrientation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddOrientationDetails");
        }

        [AllowAnonymous]
        public ActionResult AddOrientationDetails(int? page)
        {
            ViewBag.SubMenutype = "TrainingOrientation";
            TrainingOrientationModelList objOrientation = new TrainingOrientationModelList();
            objOrientation.Orientation = new List<TrainingOrientationModels>();

            try
            {
                ViewBag.branch = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                ViewBag.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.BranchList = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.DepartmentList = objGlobaldata.GetDepartmentListbox(ViewBag.branch);
                ViewBag.LocationList = objGlobaldata.GetDivisionLocationList(ViewBag.branch);

                string sSqlstmt = "select TrainingOrientation_Id,Topic,DocUploadPath,branch,Department,Location from t_trainingorientation where Active=1";
                DataSet dsOrientation = objGlobaldata.Getdetails(sSqlstmt);

                if (dsOrientation.Tables.Count > 0 && dsOrientation.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsOrientation.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            TrainingOrientationModels objOrinetation = new TrainingOrientationModels
                            {
                                TrainingOrientation_Id = Convert.ToInt32(dsOrientation.Tables[0].Rows[i]["TrainingOrientation_Id"].ToString()),
                                Topic = dsOrientation.Tables[0].Rows[i]["Topic"].ToString(),
                                DocUploadPath = dsOrientation.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsOrientation.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsOrientation.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsOrientation.Tables[0].Rows[i]["Location"].ToString()),
                            };
                            objOrientation.Orientation.Add(objOrinetation);
                        }
                        catch (Exception ex)
                        {
                            string error = ex.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddOrientationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objOrientation.Orientation.ToList());
        }

        [AllowAnonymous]
        public ActionResult TrainingOrientationEdit()
        {
            ViewBag.SubMenutype = "TrainingOrientation";
            TrainingOrientationModels objOrientaionModels = new TrainingOrientationModels();

            try
            {
                if (Request.QueryString["TrainingOrientation_Id"] != null && Request.QueryString["TrainingOrientation_Id"] != "")
                {
                    string sTrainingOrientation_Id = Request.QueryString["TrainingOrientation_Id"];

                    string sSqlstmt = "select TrainingOrientation_Id,Topic,DocUploadPath,branch,Department,Location from t_trainingorientation where TrainingOrientation_Id='" + sTrainingOrientation_Id + "'";

                    DataSet dsOrientationList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsOrientationList.Tables.Count > 0 && dsOrientationList.Tables[0].Rows.Count > 0)
                    {
                        objOrientaionModels = new TrainingOrientationModels
                        {
                            TrainingOrientation_Id = Convert.ToInt16(dsOrientationList.Tables[0].Rows[0]["TrainingOrientation_Id"].ToString()),
                            Topic = dsOrientationList.Tables[0].Rows[0]["Topic"].ToString(),
                            DocUploadPath = dsOrientationList.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            branch = (dsOrientationList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = dsOrientationList.Tables[0].Rows[0]["Department"].ToString(),
                            Location = dsOrientationList.Tables[0].Rows[0]["Location"].ToString(),
                        };

                        ViewBag.BranchList = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.LocationList = objGlobaldata.GetLocationbyMultiDivision(dsOrientationList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.DepartmentList = objGlobaldata.GetDepartmentList1(dsOrientationList.Tables[0].Rows[0]["branch"].ToString());
                    }
                    else
                    {
                        TempData["alertdata"] = "TrainingOrientationID cannot be Null or empty";
                        return RedirectToAction("AddOrientationDetails");
                    }
                }
                else
                {
                    TempData["alertdata"] = "TrainingOrientationID cannot be Null or empty";
                    return RedirectToAction("AddOrientationDetails");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingOrientationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AddOrientationDetails");
            }
            return View(objOrientaionModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrainingOrientationEdit(TrainingOrientationModels objOrientaionModels, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                ViewBag.SubMenutype = "TrainingOrientation";

                objOrientaionModels.Topic = form["Topic"];
                objOrientaionModels.branch = form["branch"];
                objOrientaionModels.Department = form["Department"];
                objOrientaionModels.Location = form["Location"];

                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/TrainingOrientation"), Path.GetFileName(file.FileName));
                        string sFilename = objOrientaionModels.Topic + "_" + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objOrientaionModels.DocUploadPath = "~/DataUpload/MgmtDocs/TrainingOrientation/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in TrainingOrientationEdit-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objOrientaionModels.FunUpdateOrientation(objOrientaionModels))
                {
                    TempData["Successdata"] = "Updated Orientation successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingOrientationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddOrientationDetails");
        }

        [AllowAnonymous]
        public ActionResult AddEmployeeTrainingOrientation()
        {
            try
            {
                ViewBag.SubMenutype = "TrainingOrientation";
                TrainingOrientationModels orientation = new TrainingOrientationModels();

                ViewBag.TrainingOrientation = orientation.GetTrainingTopicListbox();
                ViewBag.SurveyRating = orientation.GetSurveyRating();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeeTrainingOrientation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployeeTrainingOrientation(TrainingOrientationModels objOrientation, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "TrainingOrientation";
                EmpOrientationModelsList obj = new EmpOrientationModelsList();
                obj.lstOrientation = new List<EmpOrientationModels>();
                TrainingOrientationModels objOrient = new TrainingOrientationModels();
                DataSet TrainingTopic = new DataSet();

                //SurveyModels objSurvey = new SurveyModels();
                //MultiSelectList SurveyQuestions = objSurvey.GetSurveyTypeListbox("Employee Performance");

                TrainingTopic = objOrient.GetTrainingTopicListbox();

                for (int i = 0; TrainingTopic.Tables.Count > 0 && i < TrainingTopic.Tables[0].Rows.Count; i++)
                {
                    EmpOrientationModels objOrient1 = new EmpOrientationModels();
                    objOrient1.TrainingOrientation_Id = form["Topic " + TrainingTopic.Tables[0].Rows[i]["TrainingOrientation_Id"].ToString()];
                    objOrient1.SQ_OptionsId = form["SQ_OptionsId " + TrainingTopic.Tables[0].Rows[i]["TrainingOrientation_Id"].ToString()];
                    obj.lstOrientation.Add(objOrient1);
                }

                //foreach (var item in TrainingTopic)
                //{
                //    EmpPerformanceElementsModels objElements = new EmpPerformanceElementsModels();

                //    objElements.SQId = form["Question " + item.Value];
                //    objElements.SQ_OptionsId = form["SQ_OptionsId " + item.Value];

                //    objEmpPerformanceElements.lstEmpPerformanceElements.Add(objElements);
                //}

                if (objOrientation.FunAddEmpOrientation(objOrientation, obj))
                {
                    TempData["Successdata"] = "Added Performance details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeeTrainingOrientation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeTrainingOrientationList");
        }

        [AllowAnonymous]
        public ActionResult EmployeeTrainingOrientationList(string SearchText, int? page, string branch_name)
        {
            TrainingOrientationModelList objEmpOrient = new TrainingOrientationModelList();
            objEmpOrient.Orientation = new List<TrainingOrientationModels>();

            try
            {
                ViewBag.SubMenutype = "TrainingOrientation";
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.DeptHead = objUser.DeptInCharge;

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select emp_orientation_id,Emp_id,First_name,Designation,Department,VerifiedBy FROM t_emp_orientation where Active=1";

                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + " and (Emp_id ='" + SearchText + "' or Emp_id like '" + SearchText + "%')";
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by First_name";
                DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsCustomer.Tables.Count > 0 && i < dsCustomer.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        TrainingOrientationModels objCustomer = new TrainingOrientationModels
                        {
                            emp_orientation_id = dsCustomer.Tables[0].Rows[i]["emp_orientation_id"].ToString(),
                            Emp_id = dsCustomer.Tables[0].Rows[i]["Emp_id"].ToString(),
                            First_name = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["Emp_id"].ToString()),
                            Designation = objGlobaldata.GetEmpDesignationByHrEmpId(dsCustomer.Tables[0].Rows[i]["Emp_id"].ToString()),
                            Department = objGlobaldata.GetDeptIdByHrEmpId(dsCustomer.Tables[0].Rows[i]["Emp_id"].ToString()),
                            VerifiedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["VerifiedBy"].ToString())
                        };

                        objEmpOrient.Orientation.Add(objCustomer);
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeTrainingOrientationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEmpOrient.Orientation.ToList().ToPagedList(page ?? 1, 40));
        }

        [AllowAnonymous]
        public JsonResult EmployeeTrainingOrientationListSearch(string SearchText, int? page, string branch_name)
        {
            TrainingOrientationModelList objEmpOrient = new TrainingOrientationModelList();
            objEmpOrient.Orientation = new List<TrainingOrientationModels>();

            try
            {
                ViewBag.SubMenutype = "TrainingOrientation";
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.DeptHead = objUser.DeptInCharge;

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select emp_orientation_id,Emp_id,First_name,Designation,Department,VerifiedBy FROM t_emp_orientation where Active=1";

                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + " and (Emp_id ='" + SearchText + "' or Emp_id like '" + SearchText + "%')";
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by First_name";
                DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsCustomer.Tables.Count > 0 && i < dsCustomer.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        TrainingOrientationModels objCustomer = new TrainingOrientationModels
                        {
                            emp_orientation_id = dsCustomer.Tables[0].Rows[i]["emp_orientation_id"].ToString(),
                            Emp_id = dsCustomer.Tables[0].Rows[i]["Emp_id"].ToString(),
                            First_name = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["Emp_id"].ToString()),
                            Designation = objGlobaldata.GetEmpDesignationByHrEmpId(dsCustomer.Tables[0].Rows[i]["Emp_id"].ToString()),
                            Department = objGlobaldata.GetDeptIdByHrEmpId(dsCustomer.Tables[0].Rows[i]["Emp_id"].ToString()),
                            VerifiedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["VerifiedBy"].ToString())
                        };

                        objEmpOrient.Orientation.Add(objCustomer);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in EmployeeTrainingOrientationListSearch: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeTrainingOrientationListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult EmployeeTrainingOrientationDetails()
        {
            try
            {
                ViewBag.SubMenutype = "TrainingOrientation";
                if (Request.QueryString["emp_orientation_id"] != null && Request.QueryString["emp_orientation_id"] != "")
                {
                    string semp_orientation_id = Request.QueryString["emp_orientation_id"];

                    string sSqlstmt = "select emp_orientation_id,Emp_id,First_name,Designation,Department,"
                    + "Orientation_date,VerifiedBy,TrainingStatus,Action,Evidence,OrientationStatus,VerificationDate FROM t_emp_orientation  where emp_orientation_id='" + semp_orientation_id + "'";

                    DataSet dsOrientation = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsOrientation.Tables.Count > 0 && dsOrientation.Tables[0].Rows.Count > 0)
                    {
                        TrainingOrientationModels objOrientation = new TrainingOrientationModels
                        {
                            emp_orientation_id = dsOrientation.Tables[0].Rows[0]["emp_orientation_id"].ToString(),
                            Emp_id = dsOrientation.Tables[0].Rows[0]["Emp_id"].ToString(),
                            First_name = objGlobaldata.GetEmpHrNameById(dsOrientation.Tables[0].Rows[0]["Emp_id"].ToString()),
                            Designation = objGlobaldata.GetEmpDesignationByHrEmpId(dsOrientation.Tables[0].Rows[0]["Emp_id"].ToString()),
                            Department = objGlobaldata.GetDeptIdByHrEmpId(dsOrientation.Tables[0].Rows[0]["Emp_id"].ToString()),
                            VerifiedBy = objGlobaldata.GetEmpHrNameById(dsOrientation.Tables[0].Rows[0]["VerifiedBy"].ToString()),
                            TrainingStatus = dsOrientation.Tables[0].Rows[0]["TrainingStatus"].ToString(),
                            Action = dsOrientation.Tables[0].Rows[0]["Action"].ToString(),
                            Evidence = dsOrientation.Tables[0].Rows[0]["Evidence"].ToString(),
                            OrientationStatus = dsOrientation.Tables[0].Rows[0]["OrientationStatus"].ToString(),
                        };

                        DateTime dtDocDate = new DateTime();
                        if (dsOrientation.Tables[0].Rows[0]["Orientation_date"].ToString() != ""
                            && DateTime.TryParse(dsOrientation.Tables[0].Rows[0]["Orientation_date"].ToString(), out dtDocDate))
                        {
                            objOrientation.Orientation_date = dtDocDate;
                        }

                        if (dsOrientation.Tables[0].Rows[0]["VerificationDate"].ToString() != ""
                           && DateTime.TryParse(dsOrientation.Tables[0].Rows[0]["VerificationDate"].ToString(), out dtDocDate))
                        {
                            objOrientation.VerificationDate = dtDocDate;
                        }

                        sSqlstmt = "SELECT Id,emp_orientation_id,TrainingOrientation_Id,SQ_OptionsId from t_emp_orientation_elements where emp_orientation_id='"
                            + objOrientation.emp_orientation_id + "'";

                        DataSet dsOrientationElement = objGlobaldata.Getdetails(sSqlstmt);

                        EmpOrientationModelsList obTrainingOrient = new EmpOrientationModelsList();
                        obTrainingOrient.lstOrientation = new List<EmpOrientationModels>();
                        SurveyModels objSurvey = new SurveyModels();
                        TrainingOrientationModels objOrient = new TrainingOrientationModels();
                        for (int i = 0; dsOrientationElement.Tables.Count > 0 && i < dsOrientationElement.Tables[0].Rows.Count; i++)
                        {
                            EmpOrientationModels objElements = new EmpOrientationModels
                            {
                                TrainingOrientation_Id = objOrient.GetTopicsNameById(dsOrientationElement.Tables[0].Rows[i]["TrainingOrientation_Id"].ToString()),
                                SQ_OptionsId = objOrient.GetRatingNameById(dsOrientationElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString())
                            };
                            obTrainingOrient.lstOrientation.Add(objElements);
                        }

                        ViewBag.PerformanceElement = obTrainingOrient;

                        return View(objOrientation);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmployeeTrainingOrientationList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EmployeeTrainingOrientationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeTrainingOrientationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeePerformanceEvalList");
        }

        [HttpPost]
        public JsonResult doesTopicNameExist(string Topic)
        {
            TrainingOrientationModels obj = new TrainingOrientationModels();
            var user = obj.CheckForTopicNameExists(Topic);

            return Json(user);
        }

        [AllowAnonymous]
        public ActionResult TrainingOrientationVerification()
        {
            UserCredentials objUser = new UserCredentials();

            try
            {
                ViewBag.SubMenutype = "TrainingOrientation";
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.Name = objUser.firstname;
                ViewBag.DeptName = objGlobaldata.GetDeptNameById(objUser.DeptID);
                ViewBag.Designation = objUser.Designation;
                ViewBag.OrientationStatus = objGlobaldata.GetConstantValue("OrientationStatus");
                ViewBag.TrainingStatus = objGlobaldata.GetConstantValue("TrainingStatus");
                if (Request.QueryString["emp_orientation_id"] != null && Request.QueryString["emp_orientation_id"] != "")
                {
                    string semp_orientation_id = Request.QueryString["emp_orientation_id"];

                    string sSqlstmt = "select emp_orientation_id,Emp_id,First_name,Designation,Department,Orientation_date FROM t_emp_orientation  where emp_orientation_id='" + semp_orientation_id + "'";

                    DataSet dsOrientation = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsOrientation.Tables.Count > 0 && dsOrientation.Tables[0].Rows.Count > 0)
                    {
                        TrainingOrientationModels objOrientation = new TrainingOrientationModels
                        {
                            emp_orientation_id = dsOrientation.Tables[0].Rows[0]["emp_orientation_id"].ToString(),
                            Emp_id = dsOrientation.Tables[0].Rows[0]["Emp_id"].ToString(),
                            First_name = objGlobaldata.GetEmpHrNameById(dsOrientation.Tables[0].Rows[0]["Emp_id"].ToString()),
                            Designation = objGlobaldata.GetEmpDesignationByHrEmpId(dsOrientation.Tables[0].Rows[0]["Emp_id"].ToString()),
                            Department = dsOrientation.Tables[0].Rows[0]["Department"].ToString(),
                        };

                        DateTime dtDocDate = new DateTime();
                        if (dsOrientation.Tables[0].Rows[0]["Orientation_date"].ToString() != ""
                            && DateTime.TryParse(dsOrientation.Tables[0].Rows[0]["Orientation_date"].ToString(), out dtDocDate))
                        {
                            objOrientation.Orientation_date = dtDocDate;
                        }

                        sSqlstmt = "SELECT Id,emp_orientation_id,TrainingOrientation_Id,SQ_OptionsId from t_emp_orientation_elements where emp_orientation_id='"
                            + objOrientation.emp_orientation_id + "'";

                        DataSet dsOrientationElement = objGlobaldata.Getdetails(sSqlstmt);

                        EmpOrientationModelsList obTrainingOrient = new EmpOrientationModelsList();
                        obTrainingOrient.lstOrientation = new List<EmpOrientationModels>();
                        SurveyModels objSurvey = new SurveyModels();
                        TrainingOrientationModels objOrient = new TrainingOrientationModels();
                        for (int i = 0; dsOrientationElement.Tables.Count > 0 && i < dsOrientationElement.Tables[0].Rows.Count; i++)
                        {
                            EmpOrientationModels objElements = new EmpOrientationModels
                            {
                                TrainingOrientation_Id = objOrient.GetTopicsNameById(dsOrientationElement.Tables[0].Rows[i]["TrainingOrientation_Id"].ToString()),
                                SQ_OptionsId = objOrient.GetRatingNameById(dsOrientationElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString())
                            };
                            obTrainingOrient.lstOrientation.Add(objElements);
                        }

                        ViewBag.PerformanceElement = obTrainingOrient;

                        return View(objOrientation);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmployeeTrainingOrientationList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EmployeeTrainingOrientationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingOrientationVerification: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeePerformanceEvalList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrainingOrientationVerification(TrainingOrientationModels objOrientation, FormCollection form, HttpPostedFileBase Evidence)
        {
            try
            {
                ViewBag.SubMenutype = "TrainingOrientation";
                objOrientation.TrainingStatus = form["TrainingStatus"];
                objOrientation.Action = form["Action"];
                objOrientation.OrientationStatus = form["OrientationStatus"];
                objOrientation.emp_orientation_id = form["emp_orientation_id"];

                if (Evidence != null && Evidence.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/TrainingOrientation"), Path.GetFileName(Evidence.FileName));
                        string sFilename = "TrainiingOrientation" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Evidence.SaveAs(sFilepath + "/" + sFilename);
                        objOrientation.Evidence = "~/DataUpload/MgmtDocs/TrainingOrientation/" + sFilename;
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

                if (objOrientation.FunAddOrientationVerification(objOrientation))
                {
                    TempData["Successdata"] = "Training orientation verification done successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingOrientationVerification: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeTrainingOrientationList");
        }
    }
}