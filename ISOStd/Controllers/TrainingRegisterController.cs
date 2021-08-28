using ISOStd.Filters;
using ISOStd.Models;
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
    public class TrainingRegisterController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public TrainingRegisterController()
        {
            ViewBag.Menutype = "Employee";
            ViewBag.SubMenutype = "TrainingRegister";
        }
        public ActionResult AddTrainingRegister()
        {
            TrainingRegisterModels objTR = new TrainingRegisterModels();
            try
            {
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DeptHead = objGlobaldata.GetDeptHeadList();
                ViewBag.TrainingTopic = objGlobaldata.GetDropdownList("Training Topic");
                ViewBag.TrainingType = objGlobaldata.GetDropdownList("Training Type");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainingRegister: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTR);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTrainingRegister(TrainingRegisterModels objTR, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objTR.tr_topic = form["tr_topic"];
                objTR.tr_type = form["tr_type"];

                DateTime dateValue;
                if (DateTime.TryParse(form["date_tr"], out dateValue) == true)
                {
                    objTR.date_tr = dateValue;
                }

                if (DateTime.TryParse(form["tr_startdate"], out dateValue) == true)
                {
                    objTR.tr_startdate = dateValue;
                }

                if (DateTime.TryParse(form["tr_enddate"], out dateValue) == true)
                {
                    objTR.tr_enddate = dateValue;
                }

                if (DateTime.TryParse(form["tr_expirydate"], out dateValue) == true)
                {
                    objTR.tr_expirydate = dateValue;
                }
                
                HttpPostedFileBase files = Request.Files[0];
                if (upload != null && files.ContentLength > 0)
                {
                    objTR.upload = "";
                    foreach (var file in upload)
                    {

                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                            string sFilename = "TR" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            files.SaveAs(sFilepath + "/" + sFilename);
                            objTR.upload = objTR.upload + "," + "~/DataUpload/MgmtDocs/Training/" + sFilename;

                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddTrainingRegister file-upload: " + ex.ToString());
                        }
                    }
                    objTR.upload = objTR.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objTR.FunAddTR(objTR))
                {
                    TempData["Successdata"] = "Added Training details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainingRegister: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("TrainingRegisterList");
        }
  
        public ActionResult TrainingRegisterList(FormCollection form, int? page, string branch_name)
        {

            TrainingRegisterModelsList objTRList = new TrainingRegisterModelsList();
            objTRList.TRList = new List<TrainingRegisterModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
             
                string sSqlstmt = "select id_tr,emp_id,division,dept_head,date_tr,tr_type,tr_topic,tr_centername,tr_location," +
                    "tr_startdate,tr_enddate,tr_expirydate,upload,loggedby from t_training_register where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', division)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_tr desc";
                DataSet dsTRList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsTRList.Tables.Count > 0 && dsTRList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsTRList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            TrainingRegisterModels objTrainingModels = new TrainingRegisterModels
                            {
                                id_tr = dsTRList.Tables[0].Rows[i]["id_tr"].ToString(),
                                emp_id = objGlobaldata.GetMultiHrEmpNameById(dsTRList.Tables[0].Rows[i]["emp_id"].ToString()),
                                dept_head = objGlobaldata.GetMultiHrEmpNameById(dsTRList.Tables[0].Rows[i]["dept_head"].ToString()),
                                tr_type = objGlobaldata.GetDropdownitemById(dsTRList.Tables[0].Rows[i]["tr_type"].ToString()),
                                tr_topic = objGlobaldata.GetDropdownitemById(dsTRList.Tables[0].Rows[i]["tr_topic"].ToString()),
                                tr_centername = dsTRList.Tables[0].Rows[i]["tr_centername"].ToString(),
                                tr_location = dsTRList.Tables[0].Rows[i]["tr_location"].ToString(),
                                upload = dsTRList.Tables[0].Rows[i]["upload"].ToString(),
                                //loggedby = dsTRList.Tables[0].Rows[i]["loggedby"].ToString(),
                                department = objGlobaldata.GetDeptNameById(objGlobaldata.GetDeptIdByHrEmpId(dsTRList.Tables[0].Rows[i]["emp_id"].ToString())),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsTRList.Tables[0].Rows[i]["emp_id"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(objGlobaldata.GetWorkLocationIdByHrEmpId(dsTRList.Tables[0].Rows[i]["emp_id"].ToString())),
                                designation = objGlobaldata.GetEmpDesignationByHrEmpId(dsTRList.Tables[0].Rows[i]["emp_id"].ToString()),
                           };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsTRList.Tables[0].Rows[i]["date_tr"].ToString(), out dtValue))
                            {
                                objTrainingModels.date_tr = dtValue;
                            }
                            if (DateTime.TryParse(dsTRList.Tables[0].Rows[i]["tr_startdate"].ToString(), out dtValue))
                            {
                                objTrainingModels.tr_startdate = dtValue;
                            }
                            if (DateTime.TryParse(dsTRList.Tables[0].Rows[i]["tr_enddate"].ToString(), out dtValue))
                            {
                                objTrainingModels.tr_enddate = dtValue;
                            }
                            if (DateTime.TryParse(dsTRList.Tables[0].Rows[i]["tr_expirydate"].ToString(), out dtValue))
                            {
                                objTrainingModels.tr_expirydate = dtValue;
                            }
                            objTRList.TRList.Add(objTrainingModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingRegisterList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingRegisterList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTRList.TRList.ToList());
        }

        public ActionResult TrainingRegisterEdit()
        {
            TrainingRegisterModels objModels = new TrainingRegisterModels();

            try
            {
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DeptHead = objGlobaldata.GetDeptHeadList();
                ViewBag.TrainingTopic = objGlobaldata.GetDropdownList("Training Topic");
                ViewBag.TrainingType = objGlobaldata.GetDropdownList("Training Type");

                if (Request.QueryString["id_tr"] != null && Request.QueryString["id_tr"] != "")
                {
                    string sid_tr = Request.QueryString["id_tr"];

                    string sSqlstmt = "select id_tr,emp_id,division,dept_head,date_tr,tr_type,tr_topic,tr_centername,tr_location," +
                    "tr_startdate,tr_enddate,tr_expirydate,upload from t_training_register where id_tr='" + sid_tr + "'";

                    DataSet dsTRList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTRList.Tables.Count > 0 && dsTRList.Tables[0].Rows.Count > 0)
                    {
                        objModels = new TrainingRegisterModels
                        {
                            id_tr = dsTRList.Tables[0].Rows[0]["id_tr"].ToString(),
                            emp_id = /*objGlobaldata.GetMultiHrEmpNameById*/(dsTRList.Tables[0].Rows[0]["emp_id"].ToString()),
                            dept_head =/* objGlobaldata.GetMultiHrEmpNameById*/(dsTRList.Tables[0].Rows[0]["dept_head"].ToString()),
                            tr_type =/* objGlobaldata.GetDropdownitemById*/(dsTRList.Tables[0].Rows[0]["tr_type"].ToString()),
                            tr_topic = /*objGlobaldata.GetDropdownitemById*/(dsTRList.Tables[0].Rows[0]["tr_topic"].ToString()),
                            tr_centername = dsTRList.Tables[0].Rows[0]["tr_centername"].ToString(),
                            tr_location = dsTRList.Tables[0].Rows[0]["tr_location"].ToString(),
                            upload = dsTRList.Tables[0].Rows[0]["upload"].ToString(),
                        //    department = objGlobaldata.GetDeptNameById(objGlobaldata.GetDeptIdByHrEmpId(dsTRList.Tables[0].Rows[0]["emp_id"].ToString())),
                        //    division = objGlobaldata.GetMultiCompanyBranchNameById(dsTRList.Tables[0].Rows[0]["emp_id"].ToString()),
                        //    location = objGlobaldata.GetDivisionLocationById(objGlobaldata.GetWorkLocationIdByHrEmpId(dsTRList.Tables[0].Rows[0]["emp_id"].ToString())),
                        //    designation = objGlobaldata.GetEmpDesignationByHrEmpId(dsTRList.Tables[0].Rows[0]["emp_id"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsTRList.Tables[0].Rows[0]["date_tr"].ToString(), out dtValue))
                        {
                            objModels.date_tr = dtValue;
                        }
                        if (DateTime.TryParse(dsTRList.Tables[0].Rows[0]["tr_startdate"].ToString(), out dtValue))
                        {
                            objModels.tr_startdate = dtValue;
                        }
                        if (DateTime.TryParse(dsTRList.Tables[0].Rows[0]["tr_enddate"].ToString(), out dtValue))
                        {
                            objModels.tr_enddate = dtValue;
                        }
                        if (DateTime.TryParse(dsTRList.Tables[0].Rows[0]["tr_expirydate"].ToString(), out dtValue))
                        {
                            objModels.tr_expirydate = dtValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("TrainingRegisterList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("TrainingRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingRegisterEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("TrainingRegisterList");
            }
            return View(objModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrainingRegisterEdit(TrainingRegisterModels objTR, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objTR.tr_topic = form["tr_topic"];
                objTR.tr_type = form["tr_type"];

                DateTime dateValue;
                if (DateTime.TryParse(form["date_tr"], out dateValue) == true)
                {
                    objTR.date_tr = dateValue;
                }

                if (DateTime.TryParse(form["tr_startdate"], out dateValue) == true)
                {
                    objTR.tr_startdate = dateValue;
                }

                if (DateTime.TryParse(form["tr_enddate"], out dateValue) == true)
                {
                    objTR.tr_enddate = dateValue;
                }

                if (DateTime.TryParse(form["tr_expirydate"], out dateValue) == true)
                {
                    objTR.tr_expirydate = dateValue;
                }
                
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];
                if (upload != null && files.ContentLength > 0)
                {
                    objTR.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                            string sFilename = "TR" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objTR.upload = objTR.upload + "," + "~/DataUpload/MgmtDocs/Training/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingRegisterEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objTR.upload = objTR.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objTR.upload = objTR.upload + "," + form["QCDocsVal"];
                    objTR.upload = objTR.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objTR.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objTR.upload = null;
                }

                if (objTR.FunUpdateTR(objTR))
                {
                    TempData["Successdata"] = "Training details Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingRegisterEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("TrainingRegisterList");
            }
            return RedirectToAction("TrainingRegisterList");
        }

        [AllowAnonymous]
        public JsonResult TrainingRegisterDelete(FormCollection form)
        {
            try
            {

                if (form["id_tr"] != null && form["id_tr"] != "")
                {

                    TrainingRegisterModels Doc = new TrainingRegisterModels();
                    string sid_tr = form["id_tr"];

                    if (Doc.FunDeleteTR(sid_tr))
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
                    TempData["alertdata"] = "Issue Id cannot be Null or empty";
                    return Json("Failed");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingRegisterDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public ActionResult TrainingRegisterDetail()
        {
            TrainingRegisterModels objModels = new TrainingRegisterModels();

            try
            {
                if (Request.QueryString["id_tr"] != null && Request.QueryString["id_tr"] != "")
                {
                    string sid_tr = Request.QueryString["id_tr"];

                    string sSqlstmt = "select id_tr,emp_id,division,dept_head,date_tr,tr_type,tr_topic,tr_centername,tr_location," +
                    "tr_startdate,tr_enddate,tr_expirydate,upload from t_training_register where id_tr='" + sid_tr + "'";

                    DataSet dsTRList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTRList.Tables.Count > 0 && dsTRList.Tables[0].Rows.Count > 0)
                    {
                        objModels = new TrainingRegisterModels
                        {
                            id_tr = dsTRList.Tables[0].Rows[0]["id_tr"].ToString(),
                            emp_id = objGlobaldata.GetMultiHrEmpNameById(dsTRList.Tables[0].Rows[0]["emp_id"].ToString()),
                            dept_head = objGlobaldata.GetMultiHrEmpNameById(dsTRList.Tables[0].Rows[0]["dept_head"].ToString()),
                            tr_type = objGlobaldata.GetDropdownitemById(dsTRList.Tables[0].Rows[0]["tr_type"].ToString()),
                            tr_topic = objGlobaldata.GetDropdownitemById(dsTRList.Tables[0].Rows[0]["tr_topic"].ToString()),
                            tr_centername = dsTRList.Tables[0].Rows[0]["tr_centername"].ToString(),
                            tr_location = dsTRList.Tables[0].Rows[0]["tr_location"].ToString(),
                            upload = dsTRList.Tables[0].Rows[0]["upload"].ToString(),
                            department = objGlobaldata.GetDeptNameById(objGlobaldata.GetDeptIdByHrEmpId(dsTRList.Tables[0].Rows[0]["emp_id"].ToString())),
                            division = objGlobaldata.GetMultiCompanyBranchNameById(dsTRList.Tables[0].Rows[0]["emp_id"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(objGlobaldata.GetWorkLocationIdByHrEmpId(dsTRList.Tables[0].Rows[0]["emp_id"].ToString())),
                            designation = objGlobaldata.GetEmpDesignationByHrEmpId(dsTRList.Tables[0].Rows[0]["emp_id"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsTRList.Tables[0].Rows[0]["date_tr"].ToString(), out dtValue))
                        {
                            objModels.date_tr = dtValue;
                        }
                        if (DateTime.TryParse(dsTRList.Tables[0].Rows[0]["tr_startdate"].ToString(), out dtValue))
                        {
                            objModels.tr_startdate = dtValue;
                        }
                        if (DateTime.TryParse(dsTRList.Tables[0].Rows[0]["tr_enddate"].ToString(), out dtValue))
                        {
                            objModels.tr_enddate = dtValue;
                        }
                        if (DateTime.TryParse(dsTRList.Tables[0].Rows[0]["tr_expirydate"].ToString(), out dtValue))
                        {
                            objModels.tr_expirydate = dtValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("TrainingRegisterList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("TrainingRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingRegisterDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("TrainingRegisterList");
            }
            return View(objModels);
        }
    }
}