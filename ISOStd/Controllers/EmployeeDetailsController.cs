using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.IO;
using PagedList;
using PagedList.Mvc;
using ISOStd.Filters;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class EmployeeDetailsController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public EmployeeDetailsController()
        {
            ViewBag.Menutype = "Employee";
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult OrgChartEdit()
        {
            try
            {
                if (Request.QueryString["chartId"] != null && Request.QueryString["chartId"] != "")
                {
                    string schartId = Request.QueryString["chartId"];
                    string sSqlstmt = "SELECT chartId,ChartName,DocUploadPath from orgcharts where chartId='" + schartId + "'";
                    DataSet dsChartList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsChartList.Tables.Count > 0 && dsChartList.Tables[0].Rows.Count > 0)
                    {
                        EmployeeMasterModels objEmployeeMasterModels = new EmployeeMasterModels
                        {
                            chartId = (dsChartList.Tables[0].Rows[0]["chartId"].ToString()),
                            ChartName = (dsChartList.Tables[0].Rows[0]["ChartName"].ToString()),
                            DocUploadPath = (dsChartList.Tables[0].Rows[0]["DocUploadPath"].ToString()),
                        };
                        return View(objEmployeeMasterModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("OrgChart");
                    }
                }
                else
                {
                    TempData["alertdata"] = "OrgChartEdit Id cannot be null";
                    return RedirectToAction("OrgChart");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in OrgChartEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("OrgChart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrgChartEdit(EmployeeMasterModels objEmployeeMasterModels, FormCollection form, HttpPostedFileBase file)
        {
            try
            {

                objEmployeeMasterModels.ChartName = form["ChartName"];

                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/OrgChart"), Path.GetFileName(file.FileName));
                        string sFilename = objEmployeeMasterModels.ChartName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeMasterModels.DocUploadPath = "~/DataUpload/MgmtDocs/OrgChart/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in UpdateOrgChart-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objEmployeeMasterModels.FunUpdateOrgChart(objEmployeeMasterModels))
                {
                    TempData["Successdata"] = "Updated Chart successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in OrgChartEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("OrgChart");
        }

        [AllowAnonymous]
        public ActionResult AddOrgChart(EmployeeMasterModels objEmployeeModel, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                objEmployeeModel.ChartName = form["ChartName"];

                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/OrgChart"), Path.GetFileName(file.FileName));
                        string sFilename = objEmployeeModel.ChartName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeModel.DocUploadPath = "~/DataUpload/MgmtDocs/OrgChart/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddOrgChart-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objEmployeeModel.FunAddOrgChart(objEmployeeModel))
                {
                    TempData["Successdata"] = "Added Chart successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddOrgChart: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("OrgChart");
        }

        [AllowAnonymous]
        public ActionResult OrgChart(int? page)
        {
            EmployeeMasterModelList objEmployeeModelList = new EmployeeMasterModelList();
            objEmployeeModelList.EmployeeList = new List<EmployeeMasterModels>();

            try
            {
                string sSqlstmt = "select chartId,ChartName,DocUploadPath from orgcharts where Status=1";
                DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                {   
                    for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmployeeMasterModels objEmployee = new EmployeeMasterModels
                            {
                                chartId = (dsEmployeeList.Tables[0].Rows[i]["chartId"].ToString()),
                                ChartName = dsEmployeeList.Tables[0].Rows[i]["ChartName"].ToString(),
                                DocUploadPath = dsEmployeeList.Tables[0].Rows[i]["DocUploadPath"].ToString(),

                            };
                            objEmployeeModelList.EmployeeList.Add(objEmployee);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddOrgChart: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in OrgChart: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEmployeeModelList.EmployeeList.ToList().ToPagedList(page ?? 1, 10000));

        }

        public ActionResult OrgChartHistory(int? page)
        {

            EmployeeMasterModelList objEmployeeModelList = new EmployeeMasterModelList();
            objEmployeeModelList.EmployeeList = new List<EmployeeMasterModels>();
            try
            {
                if (Request.QueryString["chartId"] != null && Request.QueryString["chartId"] != "")
                {
                    string schartId = Request.QueryString["chartId"];
                    string SqlStmt = "select chart_historyId,chartId,ChartName,DocUploadPath from orgcharts_history where chartId='" + schartId + "'";
                    DataSet dsEmployeeList = objGlobaldata.Getdetails(SqlStmt);
                    if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EmployeeMasterModels objEmployee = new EmployeeMasterModels
                                {
                                    chartId = (dsEmployeeList.Tables[0].Rows[i]["chartId"].ToString()),
                                    ChartName = dsEmployeeList.Tables[0].Rows[i]["ChartName"].ToString(),
                                    DocUploadPath = dsEmployeeList.Tables[0].Rows[i]["DocUploadPath"].ToString(),

                                };
                                objEmployeeModelList.EmployeeList.Add(objEmployee);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddOrgChart: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in OrgChartHistory: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objEmployeeModelList.EmployeeList.ToList().ToPagedList(page ?? 1, 40));
        }

        public JsonResult ChartDelete(FormCollection form)
        {
            try
            {

               
                    if (form["chartId"] != null && form["chartId"] != "")
                    {

                        EmployeeMasterModels chart = new EmployeeMasterModels();
                        string schartId = form["chartId"];


                        if (chart.FunDeleteChart(schartId))
                        {
                            TempData["Successdata"] = "Chart details deleted successfully";
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
                        TempData["alertdata"] = "Chart Id cannot be Null or empty";
                        return Json("Failed");
                    }
               

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChartDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult AddEmployee()
        {
            try
            {
                ViewBag.SubMenutype = "EmployeeDetails";

                ViewBag.DeptList = objGlobaldata.GetDepartmentWithIdListbox();
                ViewBag.Visa_Type = objGlobaldata.GetConstantValue("Visa_Type");
                ViewBag.Marital_status = objGlobaldata.GetConstantValue("Marital_status");
                ViewBag.Gender = objGlobaldata.GetConstantValue("Gender");
                ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList("");
                //ViewBag.CustomerList = objGlobaldata.GetCustomerListbox();
                ViewBag.Division = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");                
                ViewBag.Roles = objGlobaldata.GetRoles();
                ViewBag.EmploymentType = objGlobaldata.GetDropdownList("Employment Type");
                ViewBag.Qualification = objGlobaldata.GetDropdownList("Employee Qualification");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployee: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }



        // [HttpPost]
        public JsonResult doesEmpIDExist(string emp_id)
        {
            var user = "";

            if (emp_id != null)
            {
                user = objGlobaldata.checkEmpIdExists(emp_id);
            }

            return Json(user);
        }

        //Used in Edit Page
        public JsonResult doesEmailIDExist(string EmailId, string emp_no)
        {
            var user = "";
            if (emp_no != null)
            {
                user = objGlobaldata.checkUserAcctnEmailExists(EmailId, emp_no);
            }

            return Json(user);
        }


        //Used In Add Page
        public JsonResult doesEmailExist(string EmailId)
        {
            var user = "";
            if (EmailId != null)
            {
                user = objGlobaldata.checkEmailAddressExists(EmailId);
            }

            return Json(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployee(EmployeeMasterModels objEmployeeModel, FormCollection form, HttpPostedFileBase ProfilePic,
            HttpPostedFileBase JobDesc, HttpPostedFileBase CompetancyDoc, HttpPostedFileBase PassportUpload, HttpPostedFileBase EIDUpload,
            HttpPostedFileBase HealthCardUpload, HttpPostedFileBase Visa_upload)
        {
            try
            {
                objEmployeeModel.qualification = form["qualification"];
                DateTime dateValue;

                if (DateTime.TryParse(form["Passport_expiry"], out dateValue) == true)
                {
                    objEmployeeModel.Passport_expiry = dateValue;
                }
                if (DateTime.TryParse(form["Labour_cardexpiry"], out dateValue) == true)
                {
                    objEmployeeModel.Labour_cardexpiry = dateValue;
                }
                if (DateTime.TryParse(form["Eid_visa_date"], out dateValue) == true)
                {
                    objEmployeeModel.Eid_visa_date = dateValue;
                }
                if (DateTime.TryParse(form["visa_Exp_date"], out dateValue) == true)
                {
                    objEmployeeModel.visa_Exp_date = dateValue;
                }
                if (DateTime.TryParse(form["Visa_stamped_on"], out dateValue) == true)
                {
                    objEmployeeModel.Visa_stamped_on = dateValue;
                }

                if (DateTime.TryParse(form["Health_Insurance_Expiry"], out dateValue) == true)
                {
                    objEmployeeModel.Health_Insurance_Expiry = dateValue;
                }
                if (DateTime.TryParse(form["Date_of_join"], out dateValue) == true)
                {
                    objEmployeeModel.Date_of_join = dateValue;
                }
                if (DateTime.TryParse(form["Date_of_exit"], out dateValue) == true)
                {
                    objEmployeeModel.Date_of_exit = dateValue;
                }

                if (DateTime.TryParse(form["Date_of_Birth"], out dateValue) == true)
                {
                    objEmployeeModel.Date_of_Birth = dateValue;
                }

                if (DateTime.TryParse(form["CompetancyFromDate"], out dateValue) == true)
                {
                    objEmployeeModel.CompetancyFromDate = dateValue;
                }

                if (DateTime.TryParse(form["CompetancyToDate"], out dateValue) == true)
                {
                    objEmployeeModel.CompetancyToDate = dateValue;
                }
                if (DateTime.TryParse(form["HealthCardIssueDate"], out dateValue) == true)
                {
                    objEmployeeModel.HealthCardIssueDate = dateValue;
                }
                if (DateTime.TryParse(form["HealthCardExpDate"], out dateValue) == true)
                {
                    objEmployeeModel.HealthCardExpDate = dateValue;
                }

                //JobDesc, EvaluatedBy, CompetancyFromDate, CompetancyToDate, CompetancyDoc,
                objEmployeeModel.EvaluatedBy = form["EvaluatedBy"];
                objEmployeeModel.Role = form["Role"];
                //if (form["Gender"] != null && form["Gender"] != "")
                //{
                //    objEmployeeModel.Gender = form["Gender"].Substring(0, 1);
                //}
                //if (form["Marital_status"] != null && form["Marital_status"] != "")
                //{
                //    objEmployeeModel.Marital_status = form["Marital_status"].Substring(0, 1);
                //}
                //if (form["Visa_Type"] != null && form["Visa_Type"] != "")
                //{
                //    objEmployeeModel.Visa_Type = form["Visa_Type"].Substring(0, 1);
                //}
                objEmployeeModel.dept_id = form["DeptID"];
                objEmployeeModel.Emp_work_location = form["Emp_work_location"];
                objEmployeeModel.DeptInCharge = form["DeptInCharge"];

                if (ProfilePic != null && ProfilePic.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/ProfilePic"), Path.GetFileName(ProfilePic.FileName));
                        string sFileExt = Path.GetExtension(ProfilePic.FileName), sFilepath = Path.GetDirectoryName(spath);
                        ProfilePic.SaveAs(sFilepath + "/" + objEmployeeModel.emp_firstname + sFileExt);
                        objEmployeeModel.ProfilePic = "~/DataUpload/ProfilePic/" + objEmployeeModel.emp_firstname + sFileExt;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-profile pic upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }

                if (JobDesc != null && JobDesc.ContentLength > 0)
                {
                    try
                    {

                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(JobDesc.FileName));
                        string sFilename = objEmployeeModel.emp_firstname + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        JobDesc.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeModel.JobDesc = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-job desc upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }

                if (CompetancyDoc != null && CompetancyDoc.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(CompetancyDoc.FileName));
                        string sFileExt = Path.GetExtension(CompetancyDoc.FileName), sFilepath = Path.GetDirectoryName(spath);
                        CompetancyDoc.SaveAs(sFilepath + "/" + objEmployeeModel.emp_firstname + "_CompDoc_" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFileExt);
                        objEmployeeModel.CompetancyDoc = "~/DataUpload/MgmtDocs/Employee/" + objEmployeeModel.emp_firstname + "_CompDoc_" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFileExt;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-competancydoc upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (PassportUpload != null && PassportUpload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(PassportUpload.FileName));
                        string sFilename = "Passport" + objEmployeeModel.emp_firstname + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        PassportUpload.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeModel.PassportUpload = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-Passport upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (EIDUpload != null && EIDUpload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(EIDUpload.FileName));
                        string sFilename = "EmeratesID" + objEmployeeModel.emp_firstname + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        EIDUpload.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeModel.EIDUpload = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-EID upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (HealthCardUpload != null && HealthCardUpload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(HealthCardUpload.FileName));
                        string sFilename = "HealthCard" + objEmployeeModel.emp_firstname + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        HealthCardUpload.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeModel.HealthCardUpload = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-HealthCard upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (Visa_upload != null && Visa_upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(Visa_upload.FileName));
                        string sFilename = "Visa" + objEmployeeModel.emp_firstname + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        Visa_upload.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeModel.Visa_upload = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-Visa upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                string jd_report = "";
                if (objEmployeeModel.Role != "" && objEmployeeModel.Role != null)
                {
                    DataSet dsData = objGlobaldata.Getdetails("Select  GROUP_CONCAT(jd_report) jd_report from t_role_jd where role_id in (" + objEmployeeModel.Role + ")");
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        jd_report = dsData.Tables[0].Rows[0]["jd_report"].ToString();
                    }
                }
                FileStream fsSource;
                FileStream[] fsSource1 = new FileStream[15];
                string[] sFileName;
                if(jd_report != "" && jd_report.Contains(","))
                { 
                sFileName = jd_report.Split(',');
                int i = 0;
                foreach (var file in sFileName)
                {

                    string filename = Path.GetFileName(file);
                    fsSource = new FileStream(Server.MapPath(file), FileMode.Open, FileAccess.Read);
                    fsSource1[i] = fsSource;
                    i++;
                }
            }                 

                if (objEmployeeModel.FunAddEmployeeMaster(objEmployeeModel, fsSource1))
                {
                    TempData["Successdata"] = "Added Employee details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployee: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList");
        }

        [AllowAnonymous]
        public ActionResult EmployeeList(string SearchText, int? page, string Department, string branch_name)
        {
            ViewBag.SubMenutype = "EmployeeDetails";
            EmployeeMasterModelList objEmployeeModelList = new EmployeeMasterModelList();
            objEmployeeModelList.EmployeeList = new List<EmployeeMasterModels>();
            ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select emp_no, emp_id, emp_lastname, emp_firstname, emp_middlename, Nationaliity, Designation,"
                    + " Gender,  Marital_status, "
                        + "EmailId, MobileNo, UID_no, Visa_Type, Visa_no, Visa_stamped_on, Eid_no, Emp_info_no, Passport_no, Passport_expiry, Labour_cardno, "
                        + "Labour_cardexpiry, Eid_visa_date, Health_insurance_provider, Health_Insurance_Expiry, Emp_local_contact, Emp_native_phoneno, Emp_native_country,"
                        + " Emp_work_location, Emp_accomodation, Date_of_join, Date_of_exit, Basic_Salary, Acc_allow, Other_allow, Gratuity, Remarks, Custody_Documents,"
                        + " Date_of_Birth, dept_id, Food_allow, Transport_allow, JobDesc, EvaluatedBy, CompetancyFromDate, CompetancyToDate, CompetancyDoc,visa_Exp_date,DeptInCharge,division"
                        + " from t_hr_employee where emp_status=1";
                //Date_of_join,visa_Exp_date,Passport_expiry,Health_Insurance_Expiry
                //string sSqlstmt = "select emp_no, emp_id, emp_lastname, emp_firstname, emp_middlename, Nationaliity, Designation,"
                //   + " Gender,  Marital_status, "
                //       + "EmailId, MobileNo, UID_no, Visa_Type, Visa_no, Visa_stamped_on, Eid_no, Emp_info_no, Passport_no,  Labour_cardno, "
                //       + "Labour_cardexpiry, Eid_visa_date, Health_insurance_provider, Emp_local_contact, Emp_native_phoneno, Emp_native_country,"
                //       + " Emp_work_location, Emp_accomodation, Basic_Salary, Acc_allow, Other_allow, Gratuity, Remarks, Custody_Documents,"
                //       + " dept_id, Food_allow, Transport_allow, JobDesc, EvaluatedBy, CompetancyDoc,DeptInCharge"
                //       + " from t_hr_employee where emp_status=1";

                string sSearchtext = "";
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and division='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and division='" + sBranch_name + "' ";
                }

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + " and (emp_firstname ='" + SearchText + "' or emp_firstname like '" + SearchText + "%' or emp_firstname ='%" + SearchText + "' or emp_firstname like '%" + SearchText + "%'"
                    + " or emp_lastname ='" + SearchText + "' or emp_lastname like '" + SearchText + "%' or emp_lastname ='%" + SearchText + "' or emp_lastname like '%" + SearchText + "%')";
                }
                if (Department != null && Department != "")
                {
                    sSearchtext = sSearchtext + " and Dept_Id='" + Department + "'";
                    ViewBag.DeptSearch = objGlobaldata.GetDeptNameById(Department);
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by emp_firstname";
                DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);


                if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                {

                   

                    for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmployeeMasterModels objEmployee = new EmployeeMasterModels
                            {
                                emp_no = dsEmployeeList.Tables[0].Rows[i]["emp_no"].ToString(),
                                MobileNo = (dsEmployeeList.Tables[0].Rows[i]["MobileNo"].ToString()),
                                emp_id = (dsEmployeeList.Tables[0].Rows[i]["emp_id"].ToString()),
                                emp_firstname = dsEmployeeList.Tables[0].Rows[i]["emp_firstname"].ToString(),
                                emp_middlename = dsEmployeeList.Tables[0].Rows[i]["emp_middlename"].ToString(),
                                emp_lastname = dsEmployeeList.Tables[0].Rows[i]["emp_lastname"].ToString(),
                                EmailId = dsEmployeeList.Tables[0].Rows[i]["EmailId"].ToString(),
                                dept_id = objGlobaldata.GetDeptNameById(dsEmployeeList.Tables[0].Rows[i]["dept_id"].ToString()),
                                Designation = dsEmployeeList.Tables[0].Rows[i]["Designation"].ToString(),
                                Emp_work_location = objGlobaldata.GetDivisionLocationById(dsEmployeeList.Tables[0].Rows[i]["Emp_work_location"].ToString()),
                                Eid_no = dsEmployeeList.Tables[0].Rows[i]["Eid_no"].ToString(),
                                Gender = dsEmployeeList.Tables[0].Rows[i]["Gender"].ToString(),
                                JobDesc = dsEmployeeList.Tables[0].Rows[i]["JobDesc"].ToString(),
                                EvaluatedBy = objGlobaldata.GetEmpHrNameById(dsEmployeeList.Tables[0].Rows[i]["EvaluatedBy"].ToString()),
                                CompetancyDoc = dsEmployeeList.Tables[0].Rows[i]["CompetancyDoc"].ToString(),
                                DeptInCharge = dsEmployeeList.Tables[0].Rows[i]["DeptInCharge"].ToString(),
                                division = objGlobaldata.GetCompanyBranchNameById(dsEmployeeList.Tables[0].Rows[i]["division"].ToString()),
                            };

                            DateTime dtDate;
                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_Birth"].ToString(), out dtDate))
                            {
                                objEmployee.Date_of_Birth = dtDate;
                            }

                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["CompetancyFromDate"].ToString(), out dtDate))
                            {
                                objEmployee.CompetancyFromDate = dtDate;
                            }

                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["CompetancyToDate"].ToString(), out dtDate))
                            {
                                objEmployee.CompetancyToDate = dtDate;
                            }

                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_exit"].ToString(), out dtDate))
                            {
                                objEmployee.Date_of_exit = dtDate;
                            }

                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_join"].ToString(), out dtDate))
                            {
                                objEmployee.Date_of_join = dtDate;
                            }

                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["visa_Exp_date"].ToString(), out dtDate))
                            {
                                objEmployee.visa_Exp_date = dtDate;
                            }
                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Passport_expiry"].ToString(), out dtDate))
                            {
                                objEmployee.Passport_expiry = dtDate;
                            }

                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Health_Insurance_Expiry"].ToString(), out dtDate))
                            {
                                objEmployee.Health_Insurance_Expiry = dtDate;
                            }

                            objEmployeeModelList.EmployeeList.Add(objEmployee);
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
                objGlobaldata.AddFunctionalLog("Exception in EmployeeList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEmployeeModelList.EmployeeList.ToList());
        }

        [AllowAnonymous]
        public JsonResult EmployeeListSearch(string SearchText, int? page, string Department, string branch_name)
        {
            ViewBag.SubMenutype = "EmployeeDetails";
            EmployeeMasterModelList objEmployeeModelList = new EmployeeMasterModelList();
            objEmployeeModelList.EmployeeList = new List<EmployeeMasterModels>();
            ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select emp_no, emp_id, emp_lastname, emp_firstname, emp_middlename, Nationaliity, Designation,"
                    + " Gender,  Marital_status, "
                        + "EmailId, MobileNo, UID_no, Visa_Type, Visa_no, Visa_stamped_on, Eid_no, Emp_info_no, Passport_no, Passport_expiry, Labour_cardno, "
                        + "Labour_cardexpiry, Eid_visa_date, Health_insurance_provider, Health_Insurance_Expiry, Emp_local_contact, Emp_native_phoneno, Emp_native_country,"
                        + " Emp_work_location, Emp_accomodation, Date_of_join, Date_of_exit, Basic_Salary, Acc_allow, Other_allow, Gratuity, Remarks, Custody_Documents,"
                        + " Date_of_Birth, dept_id, Food_allow, Transport_allow, JobDesc, EvaluatedBy, CompetancyFromDate, CompetancyToDate, CompetancyDoc,visa_Exp_date,DeptInCharge,division"
                        + " from t_hr_employee where emp_status=1";

                string sSearchtext = "";
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and division='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and division='" + sBranch_name + "' ";
                }

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + " and (emp_firstname ='" + SearchText + "' or emp_firstname like '" + SearchText + "%' or emp_firstname ='%" + SearchText + "' or emp_firstname like '%" + SearchText + "%'"
                    + " or emp_lastname ='" + SearchText + "' or emp_lastname like '" + SearchText + "%' or emp_lastname ='%" + SearchText + "' or emp_lastname like '%" + SearchText + "%')";
                }
                if (Department != null && Department != "")
                {
                    sSearchtext = sSearchtext + " and Dept_Id='" + Department + "'";
                    ViewBag.DeptSearch = objGlobaldata.GetDeptNameById(Department);
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by emp_firstname";
                DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                
                if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                { 
                    for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmployeeMasterModels objEmployee = new EmployeeMasterModels
                            {
                                emp_no = dsEmployeeList.Tables[0].Rows[i]["emp_no"].ToString(),
                                MobileNo = (dsEmployeeList.Tables[0].Rows[i]["MobileNo"].ToString()),
                                emp_id = (dsEmployeeList.Tables[0].Rows[i]["emp_id"].ToString()),
                                emp_firstname = dsEmployeeList.Tables[0].Rows[i]["emp_firstname"].ToString(),
                                emp_middlename = dsEmployeeList.Tables[0].Rows[i]["emp_middlename"].ToString(),
                                emp_lastname = dsEmployeeList.Tables[0].Rows[i]["emp_lastname"].ToString(),
                                EmailId = dsEmployeeList.Tables[0].Rows[i]["EmailId"].ToString(),
                                dept_id = objGlobaldata.GetDeptNameById(dsEmployeeList.Tables[0].Rows[i]["dept_id"].ToString()),
                                Designation = dsEmployeeList.Tables[0].Rows[i]["Designation"].ToString(),
                                Emp_work_location = objGlobaldata.GetDivisionLocationById(dsEmployeeList.Tables[0].Rows[i]["Emp_work_location"].ToString()),
                                Eid_no = dsEmployeeList.Tables[0].Rows[i]["Eid_no"].ToString(),
                                Gender = dsEmployeeList.Tables[0].Rows[i]["Gender"].ToString(),
                                JobDesc = dsEmployeeList.Tables[0].Rows[i]["JobDesc"].ToString(),
                                EvaluatedBy = objGlobaldata.GetEmpHrNameById(dsEmployeeList.Tables[0].Rows[i]["EvaluatedBy"].ToString()),
                                CompetancyDoc = dsEmployeeList.Tables[0].Rows[i]["CompetancyDoc"].ToString(),
                                DeptInCharge = dsEmployeeList.Tables[0].Rows[i]["DeptInCharge"].ToString(),
                                division = objGlobaldata.GetCompanyBranchNameById(dsEmployeeList.Tables[0].Rows[i]["division"].ToString()),
                            };

                            DateTime dtDate;
                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_Birth"].ToString(), out dtDate))
                            {
                                objEmployee.Date_of_Birth = dtDate;
                            }

                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["CompetancyFromDate"].ToString(), out dtDate))
                            {
                                objEmployee.CompetancyFromDate = dtDate;
                            }

                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["CompetancyToDate"].ToString(), out dtDate))
                            {
                                objEmployee.CompetancyToDate = dtDate;
                            }

                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_exit"].ToString(), out dtDate))
                            {
                                objEmployee.Date_of_exit = dtDate;
                            }
                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_join"].ToString(), out dtDate))
                            {
                                objEmployee.Date_of_join = dtDate;
                            }

                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["visa_Exp_date"].ToString(), out dtDate))
                            {
                                objEmployee.visa_Exp_date = dtDate;
                            }
                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Passport_expiry"].ToString(), out dtDate))
                            {
                                objEmployee.Passport_expiry = dtDate;
                            }

                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Health_Insurance_Expiry"].ToString(), out dtDate))
                            {
                                objEmployee.Health_Insurance_Expiry = dtDate;
                            }

                            objEmployeeModelList.EmployeeList.Add(objEmployee);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in EmployeeListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
        

        [AllowAnonymous]
        public ActionResult AllEmployeesList(string SearchText, int? page)
        {
            EmployeeMasterModelList objEmployeeModelList = new EmployeeMasterModelList();
            objEmployeeModelList.EmployeeList = new List<EmployeeMasterModels>();
            try
            {              
                string sSqlstmt = "select emp_no, emp_id, emp_lastname, emp_firstname, emp_middlename, Nationaliity, Designation,"
                    + " Gender,  Marital_status, "
                        + "EmailId, MobileNo, UID_no, Visa_Type, Visa_no, Visa_stamped_on, Eid_no, Emp_info_no, Passport_no, Passport_expiry, Labour_cardno, "
                        + "Labour_cardexpiry, Eid_visa_date, Health_insurance_provider, Health_Insurance_Expiry, Emp_local_contact, Emp_native_phoneno, Emp_native_country,"
                        + " Emp_work_location, Emp_accomodation, Date_of_join, Date_of_exit, Basic_Salary, Acc_allow, Other_allow, Gratuity, Remarks, Custody_Documents,"
                        + " Date_of_Birth, dept_id, Food_allow, Transport_allow, JobDesc, EvaluatedBy, CompetancyFromDate, CompetancyToDate, CompetancyDoc,visa_Exp_date,DeptInCharge,division,Role"
                        + " from t_hr_employee where emp_status=1 order by emp_firstname";
               
                DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                
                if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmployeeMasterModels objEmployee = new EmployeeMasterModels
                            {
                                emp_no = dsEmployeeList.Tables[0].Rows[i]["emp_no"].ToString(),
                                MobileNo = (dsEmployeeList.Tables[0].Rows[i]["MobileNo"].ToString()),
                                emp_id = (dsEmployeeList.Tables[0].Rows[i]["emp_id"].ToString()),
                                emp_firstname = dsEmployeeList.Tables[0].Rows[i]["emp_firstname"].ToString(),
                                emp_middlename = dsEmployeeList.Tables[0].Rows[i]["emp_middlename"].ToString(),
                                emp_lastname = dsEmployeeList.Tables[0].Rows[i]["emp_lastname"].ToString(),
                                EmailId = dsEmployeeList.Tables[0].Rows[i]["EmailId"].ToString(),
                                dept_id = objGlobaldata.GetDeptNameById(dsEmployeeList.Tables[0].Rows[i]["dept_id"].ToString()),
                                Designation = dsEmployeeList.Tables[0].Rows[i]["Designation"].ToString(),
                                Emp_work_location = objGlobaldata.GetDivisionLocationById(dsEmployeeList.Tables[0].Rows[i]["Emp_work_location"].ToString()),
                                Eid_no = dsEmployeeList.Tables[0].Rows[i]["Eid_no"].ToString(),
                                Gender = dsEmployeeList.Tables[0].Rows[i]["Gender"].ToString(),
                                JobDesc = dsEmployeeList.Tables[0].Rows[i]["JobDesc"].ToString(),
                                EvaluatedBy = objGlobaldata.GetEmpHrNameById(dsEmployeeList.Tables[0].Rows[i]["EvaluatedBy"].ToString()),
                                CompetancyDoc = dsEmployeeList.Tables[0].Rows[i]["CompetancyDoc"].ToString(),
                                DeptInCharge = dsEmployeeList.Tables[0].Rows[i]["DeptInCharge"].ToString(),
                                division = objGlobaldata.GetCompanyBranchNameById(dsEmployeeList.Tables[0].Rows[i]["division"].ToString()),
                                Role = objGlobaldata.GetMultiRoleById(dsEmployeeList.Tables[0].Rows[i]["Role"].ToString()),
                            };

                            //DateTime dtDate;
                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_Birth"].ToString(), out dtDate))
                            //{
                            //    objEmployee.Date_of_Birth = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["CompetancyFromDate"].ToString(), out dtDate))
                            //{
                            //    objEmployee.CompetancyFromDate = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["CompetancyToDate"].ToString(), out dtDate))
                            //{
                            //    objEmployee.CompetancyToDate = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_exit"].ToString(), out dtDate))
                            //{
                            //    objEmployee.Date_of_exit = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_join"].ToString(), out dtDate))
                            //{
                            //    objEmployee.Date_of_join = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["visa_Exp_date"].ToString(), out dtDate))
                            //{
                            //    objEmployee.visa_Exp_date = dtDate;
                            //}
                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Passport_expiry"].ToString(), out dtDate))
                            //{
                            //    objEmployee.Passport_expiry = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Health_Insurance_Expiry"].ToString(), out dtDate))
                            //{
                            //    objEmployee.Health_Insurance_Expiry = dtDate;
                            //}

                            objEmployeeModelList.EmployeeList.Add(objEmployee);
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
                objGlobaldata.AddFunctionalLog("Exception in AllEmployeesList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEmployeeModelList.EmployeeList.ToList());
        }

        [AllowAnonymous]
        public ActionResult AllMgmtList(int? page)
        {
            EmployeeMasterModelList objEmployeeModelList = new EmployeeMasterModelList();
            objEmployeeModelList.EmployeeList = new List<EmployeeMasterModels>();
            try
            {
                string sSqlstmt = "select emp_no, emp_id, emp_lastname, emp_firstname, emp_middlename, Nationaliity, Designation,"
                    + " Gender,  Marital_status, "
                        + "EmailId, MobileNo, UID_no, Visa_Type, Visa_no, Visa_stamped_on, Eid_no, Emp_info_no, Passport_no, Passport_expiry, Labour_cardno, "
                        + "Labour_cardexpiry, Eid_visa_date, Health_insurance_provider, Health_Insurance_Expiry, Emp_local_contact, Emp_native_phoneno, Emp_native_country,"
                        + " Emp_work_location, Emp_accomodation, Date_of_join, Date_of_exit, Basic_Salary, Acc_allow, Other_allow, Gratuity, Remarks, Custody_Documents,"
                        + " Date_of_Birth, dept_id, Food_allow, Transport_allow, JobDesc, EvaluatedBy, CompetancyFromDate, CompetancyToDate, CompetancyDoc,visa_Exp_date,DeptInCharge,division,Role"
                        + " from t_hr_employee where emp_status=0 and mgmtflag=1 order by emp_firstname";

                DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmployeeMasterModels objEmployee = new EmployeeMasterModels
                            {
                                emp_no = dsEmployeeList.Tables[0].Rows[i]["emp_no"].ToString(),
                                MobileNo = (dsEmployeeList.Tables[0].Rows[i]["MobileNo"].ToString()),
                                emp_id = (dsEmployeeList.Tables[0].Rows[i]["emp_id"].ToString()),
                                emp_firstname = dsEmployeeList.Tables[0].Rows[i]["emp_firstname"].ToString(),
                                emp_middlename = dsEmployeeList.Tables[0].Rows[i]["emp_middlename"].ToString(),
                                emp_lastname = dsEmployeeList.Tables[0].Rows[i]["emp_lastname"].ToString(),
                                EmailId = dsEmployeeList.Tables[0].Rows[i]["EmailId"].ToString(),
                                dept_id = objGlobaldata.GetDeptNameById(dsEmployeeList.Tables[0].Rows[i]["dept_id"].ToString()),
                                Designation = dsEmployeeList.Tables[0].Rows[i]["Designation"].ToString(),
                                Emp_work_location = objGlobaldata.GetDivisionLocationById(dsEmployeeList.Tables[0].Rows[i]["Emp_work_location"].ToString()),
                                Eid_no = dsEmployeeList.Tables[0].Rows[i]["Eid_no"].ToString(),
                                Gender = dsEmployeeList.Tables[0].Rows[i]["Gender"].ToString(),
                                JobDesc = dsEmployeeList.Tables[0].Rows[i]["JobDesc"].ToString(),
                                EvaluatedBy = objGlobaldata.GetEmpHrNameById(dsEmployeeList.Tables[0].Rows[i]["EvaluatedBy"].ToString()),
                                CompetancyDoc = dsEmployeeList.Tables[0].Rows[i]["CompetancyDoc"].ToString(),
                                DeptInCharge = dsEmployeeList.Tables[0].Rows[i]["DeptInCharge"].ToString(),
                                division = objGlobaldata.GetCompanyBranchNameById(dsEmployeeList.Tables[0].Rows[i]["division"].ToString()),
                                Role = objGlobaldata.GetMultiRoleById(dsEmployeeList.Tables[0].Rows[i]["Role"].ToString()),
                            };

                            //DateTime dtDate;
                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_Birth"].ToString(), out dtDate))
                            //{
                            //    objEmployee.Date_of_Birth = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["CompetancyFromDate"].ToString(), out dtDate))
                            //{
                            //    objEmployee.CompetancyFromDate = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["CompetancyToDate"].ToString(), out dtDate))
                            //{
                            //    objEmployee.CompetancyToDate = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_exit"].ToString(), out dtDate))
                            //{
                            //    objEmployee.Date_of_exit = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_join"].ToString(), out dtDate))
                            //{
                            //    objEmployee.Date_of_join = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["visa_Exp_date"].ToString(), out dtDate))
                            //{
                            //    objEmployee.visa_Exp_date = dtDate;
                            //}
                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Passport_expiry"].ToString(), out dtDate))
                            //{
                            //    objEmployee.Passport_expiry = dtDate;
                            //}

                            //if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Health_Insurance_Expiry"].ToString(), out dtDate))
                            //{
                            //    objEmployee.Health_Insurance_Expiry = dtDate;
                            //}

                            objEmployeeModelList.EmployeeList.Add(objEmployee);
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
                objGlobaldata.AddFunctionalLog("Exception in AllMgmtList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEmployeeModelList.EmployeeList.ToList());
        }

        [AllowAnonymous]
        public ActionResult EmployeeDetails(string SearchText)
        {
            ViewBag.SubMenutype = "EmployeeDetails";
            EmployeeMasterModels objEmployee = new EmployeeMasterModels();

            try
            {
                if (Request.QueryString["emp_no"] != null && Request.QueryString["emp_no"] != "")
                {
                    string sEmp_no = Request.QueryString["emp_no"];
                    string sSqlstmt = "select emp_no, emp_id, emp_lastname, emp_firstname, emp_middlename, Nationaliity, Designation,"
                        + " Gender, Marital_status,"
                            + "EmailId, MobileNo, UID_no, Visa_Type, Visa_no, Visa_stamped_on, Eid_no, Emp_info_no, Passport_no, Passport_expiry, Labour_cardno, "
                            + "Labour_cardexpiry, Eid_visa_date, Health_insurance_provider, Health_Insurance_Expiry, Emp_local_contact, Emp_native_phoneno, Emp_native_country,"
                            + " Emp_work_location, Emp_accomodation, Date_of_join, Date_of_exit, Basic_Salary, Acc_allow, Other_allow, Gratuity, Remarks, Custody_Documents,"
                            + " Date_of_Birth, dept_id, ProfilePic, JobDesc, EvaluatedBy, CompetancyFromDate, CompetancyToDate, CompetancyDoc, Basic_Salary, Acc_allow,"
                            + " Other_allow Food_allow, Transport_allow,visa_Exp_date,PassportUpload,EIDUpload,HealthCardUpload,HealthCardIssueDate,HealthCardExpDate,"
                            + " Logged_date,Visa_upload,DeptInCharge,division,Role,employment_type,qualification,years_exp from t_hr_employee where emp_status=1 and emp_no='" + sEmp_no + "'";

                    DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                    {
                        objEmployee = new EmployeeMasterModels
                        {
                            emp_no = dsEmployeeList.Tables[0].Rows[0]["emp_no"].ToString(),
                            emp_id = (dsEmployeeList.Tables[0].Rows[0]["emp_id"].ToString()),
                            emp_firstname = dsEmployeeList.Tables[0].Rows[0]["emp_firstname"].ToString(),
                            emp_middlename = dsEmployeeList.Tables[0].Rows[0]["emp_middlename"].ToString(),
                            emp_lastname = dsEmployeeList.Tables[0].Rows[0]["emp_lastname"].ToString(),
                            EmailId = dsEmployeeList.Tables[0].Rows[0]["EmailId"].ToString(),
                            Nationaliity = dsEmployeeList.Tables[0].Rows[0]["Nationaliity"].ToString(),
                            dept_id = objGlobaldata.GetDeptNameById(dsEmployeeList.Tables[0].Rows[0]["dept_id"].ToString()),
                            Designation = dsEmployeeList.Tables[0].Rows[0]["Designation"].ToString(),
                            MobileNo = (dsEmployeeList.Tables[0].Rows[0]["MobileNo"].ToString()),
                            Eid_no = dsEmployeeList.Tables[0].Rows[0]["Eid_no"].ToString(),
                            Gender = dsEmployeeList.Tables[0].Rows[0]["Gender"].ToString(),
                            Marital_status = dsEmployeeList.Tables[0].Rows[0]["Marital_status"].ToString(),
                            UID_no = dsEmployeeList.Tables[0].Rows[0]["UID_no"].ToString(),
                            Visa_Type = dsEmployeeList.Tables[0].Rows[0]["Visa_Type"].ToString(),
                            Visa_no = dsEmployeeList.Tables[0].Rows[0]["Visa_no"].ToString(),
                            Emp_info_no = dsEmployeeList.Tables[0].Rows[0]["Emp_info_no"].ToString(),
                            Passport_no = dsEmployeeList.Tables[0].Rows[0]["Passport_no"].ToString(),
                            Labour_cardno = dsEmployeeList.Tables[0].Rows[0]["Labour_cardno"].ToString(),
                            Health_insurance_provider = dsEmployeeList.Tables[0].Rows[0]["Health_insurance_provider"].ToString(),
                            Emp_local_contact = (dsEmployeeList.Tables[0].Rows[0]["Emp_local_contact"].ToString()),
                            Emp_native_phoneno = (dsEmployeeList.Tables[0].Rows[0]["Emp_native_phoneno"].ToString()),
                            Emp_native_country = dsEmployeeList.Tables[0].Rows[0]["Emp_native_country"].ToString(),
                            Emp_work_location = objGlobaldata.GetDivisionLocationById(dsEmployeeList.Tables[0].Rows[0]["Emp_work_location"].ToString()),
                            Emp_accomodation = dsEmployeeList.Tables[0].Rows[0]["Emp_accomodation"].ToString(),
                            Remarks = dsEmployeeList.Tables[0].Rows[0]["Remarks"].ToString(),
                            Custody_Documents = dsEmployeeList.Tables[0].Rows[0]["Custody_Documents"].ToString(),
                            ProfilePic = dsEmployeeList.Tables[0].Rows[0]["ProfilePic"].ToString(),
                            JobDesc = dsEmployeeList.Tables[0].Rows[0]["JobDesc"].ToString(),
                            EvaluatedBy = objGlobaldata.GetEmpHrNameById(dsEmployeeList.Tables[0].Rows[0]["EvaluatedBy"].ToString()),
                            CompetancyDoc = dsEmployeeList.Tables[0].Rows[0]["CompetancyDoc"].ToString(),
                            PassportUpload = dsEmployeeList.Tables[0].Rows[0]["PassportUpload"].ToString(),
                            EIDUpload = dsEmployeeList.Tables[0].Rows[0]["EIDUpload"].ToString(),
                            HealthCardUpload = dsEmployeeList.Tables[0].Rows[0]["HealthCardUpload"].ToString(),
                            Visa_upload = dsEmployeeList.Tables[0].Rows[0]["Visa_upload"].ToString(),
                            DeptInCharge = dsEmployeeList.Tables[0].Rows[0]["DeptInCharge"].ToString(),
                            division =objGlobaldata.GetCompanyBranchNameById(dsEmployeeList.Tables[0].Rows[0]["division"].ToString()),
                            Role = objGlobaldata.GetMultiRoleById(dsEmployeeList.Tables[0].Rows[0]["Role"].ToString()),
                            employment_type =objGlobaldata.GetDropdownitemById(dsEmployeeList.Tables[0].Rows[0]["employment_type"].ToString()),
                            qualification = objGlobaldata.GetDropdownitemById(dsEmployeeList.Tables[0].Rows[0]["qualification"].ToString()),
                            years_exp = dsEmployeeList.Tables[0].Rows[0]["years_exp"].ToString(),
                        };

                        decimal dValue;

                        if (decimal.TryParse(dsEmployeeList.Tables[0].Rows[0]["Transport_allow"].ToString(), out dValue))
                        {
                            objEmployee.Transport_allow = dValue;
                        }

                        if (decimal.TryParse(dsEmployeeList.Tables[0].Rows[0]["Basic_Salary"].ToString(), out dValue))
                        {
                            objEmployee.Basic_Salary = dValue;
                        }

                        if (decimal.TryParse(dsEmployeeList.Tables[0].Rows[0]["Acc_allow"].ToString(), out dValue))
                        {
                            objEmployee.Acc_allow = dValue;
                        }

                        if (decimal.TryParse(dsEmployeeList.Tables[0].Rows[0]["Food_allow"].ToString(), out dValue))
                        {
                            objEmployee.Food_allow = dValue;
                        }

                        if (decimal.TryParse(dsEmployeeList.Tables[0].Rows[0]["Other_allow"].ToString(), out dValue))
                        {
                            objEmployee.Other_allow = dValue;
                        }

                        DateTime dateValue;
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Passport_expiry"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Passport_expiry = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Labour_cardexpiry"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Labour_cardexpiry = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Eid_visa_date"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Eid_visa_date = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["visa_Exp_date"].ToString(), out dateValue) == true)
                        {
                            objEmployee.visa_Exp_date = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Visa_stamped_on"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Visa_stamped_on = dateValue;
                        }

                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Health_Insurance_Expiry"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Health_Insurance_Expiry = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Date_of_join"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Date_of_join = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Date_of_exit"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Date_of_exit = dateValue;
                        }

                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Date_of_Birth"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Date_of_Birth = dateValue;
                        }


                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["CompetancyFromDate"].ToString(), out dateValue))
                        {
                            objEmployee.CompetancyFromDate = dateValue;
                        }

                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["CompetancyToDate"].ToString(), out dateValue))
                        {
                            objEmployee.CompetancyToDate = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["HealthCardIssueDate"].ToString(), out dateValue))
                        {
                            objEmployee.HealthCardIssueDate = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["HealthCardExpDate"].ToString(), out dateValue))
                        {
                            objEmployee.HealthCardExpDate = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Logged_date"].ToString(), out dateValue))
                        {
                            objEmployee.Logged_date = dateValue;
                        }
                        return View(objEmployee);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("EmployeeList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList");
        }

        [AllowAnonymous]
        public ActionResult EmployeeEdit()
        {
            ViewBag.SubMenutype = "EmployeeDetails";
            EmployeeMasterModels objEmployee = new EmployeeMasterModels();

            try
            {
                if (Request.QueryString["emp_no"] != null && Request.QueryString["emp_no"] != "")
                {
                    string sEmp_no = Request.QueryString["emp_no"];
                    string sSqlstmt = "select emp_no, emp_id, emp_lastname, emp_firstname, emp_middlename, Nationaliity, Designation,"
                        + " Gender, Marital_status,"
                            + "EmailId, MobileNo, UID_no, Visa_Type, Visa_no, Visa_stamped_on, Eid_no, Emp_info_no, Passport_no, Passport_expiry, Labour_cardno, "
                            + "Labour_cardexpiry, Eid_visa_date, Health_insurance_provider, Health_Insurance_Expiry, Emp_local_contact, Emp_native_phoneno, Emp_native_country,"
                            + " Emp_work_location, Emp_accomodation, Date_of_join, Date_of_exit, Basic_Salary, Acc_allow, Other_allow, Gratuity, Remarks, Custody_Documents,"
                            + " Date_of_Birth, dept_id, ProfilePic, JobDesc, EvaluatedBy, CompetancyFromDate, CompetancyToDate, CompetancyDoc, Basic_Salary, Acc_allow,"
                            + " Other_allow Food_allow, Transport_allow,visa_Exp_date,PassportUpload,EIDUpload,HealthCardUpload,HealthCardIssueDate,HealthCardExpDate,"
                            + " Visa_upload,DeptInCharge,division,Role,employment_type,qualification,years_exp from t_hr_employee where emp_status=1 and emp_no='" + sEmp_no + "'";

                    DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                    {
                        objEmployee = new EmployeeMasterModels
                        {
                            emp_no = dsEmployeeList.Tables[0].Rows[0]["emp_no"].ToString(),
                            emp_id = (dsEmployeeList.Tables[0].Rows[0]["emp_id"].ToString()),
                            emp_firstname = dsEmployeeList.Tables[0].Rows[0]["emp_firstname"].ToString(),
                            emp_middlename = dsEmployeeList.Tables[0].Rows[0]["emp_middlename"].ToString(),
                            emp_lastname = dsEmployeeList.Tables[0].Rows[0]["emp_lastname"].ToString(),
                            EmailId = dsEmployeeList.Tables[0].Rows[0]["EmailId"].ToString(),
                            Nationaliity = dsEmployeeList.Tables[0].Rows[0]["Nationaliity"].ToString(),
                            dept_id = objGlobaldata.GetDeptNameById(dsEmployeeList.Tables[0].Rows[0]["dept_id"].ToString()),
                            // ReportingTo = dsEmployeeList.Tables[0].Rows[0]["ReportingTo"].ToString(),
                            Designation = dsEmployeeList.Tables[0].Rows[0]["Designation"].ToString(),
                            MobileNo = (dsEmployeeList.Tables[0].Rows[0]["MobileNo"].ToString()),
                            Eid_no = dsEmployeeList.Tables[0].Rows[0]["Eid_no"].ToString(),
                            Gender = dsEmployeeList.Tables[0].Rows[0]["Gender"].ToString(),
                            Marital_status = dsEmployeeList.Tables[0].Rows[0]["Marital_status"].ToString(),
                            UID_no = dsEmployeeList.Tables[0].Rows[0]["UID_no"].ToString(),
                            Visa_Type = dsEmployeeList.Tables[0].Rows[0]["Visa_Type"].ToString(),
                            Visa_no = dsEmployeeList.Tables[0].Rows[0]["Visa_no"].ToString(),
                            Emp_info_no = dsEmployeeList.Tables[0].Rows[0]["Emp_info_no"].ToString(),
                            Passport_no = dsEmployeeList.Tables[0].Rows[0]["Passport_no"].ToString(),
                            Labour_cardno = dsEmployeeList.Tables[0].Rows[0]["Labour_cardno"].ToString(),
                            Health_insurance_provider = dsEmployeeList.Tables[0].Rows[0]["Health_insurance_provider"].ToString(),
                            Emp_local_contact = (dsEmployeeList.Tables[0].Rows[0]["Emp_local_contact"].ToString()),
                            Emp_native_phoneno = (dsEmployeeList.Tables[0].Rows[0]["Emp_native_phoneno"].ToString()),
                            Emp_native_country = dsEmployeeList.Tables[0].Rows[0]["Emp_native_country"].ToString(),
                            Emp_work_location = (dsEmployeeList.Tables[0].Rows[0]["Emp_work_location"].ToString()),
                            Emp_accomodation = dsEmployeeList.Tables[0].Rows[0]["Emp_accomodation"].ToString(),
                            Remarks = dsEmployeeList.Tables[0].Rows[0]["Remarks"].ToString(),
                            Custody_Documents = dsEmployeeList.Tables[0].Rows[0]["Custody_Documents"].ToString(),
                            ProfilePic = dsEmployeeList.Tables[0].Rows[0]["ProfilePic"].ToString(),
                            JobDesc = dsEmployeeList.Tables[0].Rows[0]["JobDesc"].ToString(),
                            EvaluatedBy = objGlobaldata.GetEmpHrNameById(dsEmployeeList.Tables[0].Rows[0]["EvaluatedBy"].ToString()),
                            CompetancyDoc = dsEmployeeList.Tables[0].Rows[0]["CompetancyDoc"].ToString(),
                            //DeptInCharge = dsEmployeeList.Tables[0].Rows[0]["DeptInCharge"].ToString()
                            PassportUpload = dsEmployeeList.Tables[0].Rows[0]["PassportUpload"].ToString(),
                            EIDUpload = dsEmployeeList.Tables[0].Rows[0]["EIDUpload"].ToString(),
                            HealthCardUpload = dsEmployeeList.Tables[0].Rows[0]["HealthCardUpload"].ToString(),
                            Visa_upload = dsEmployeeList.Tables[0].Rows[0]["Visa_upload"].ToString(),
                            DeptInCharge = dsEmployeeList.Tables[0].Rows[0]["DeptInCharge"].ToString(),
                            division = dsEmployeeList.Tables[0].Rows[0]["division"].ToString(),
                            Role = objGlobaldata.GetMultiRoleById(dsEmployeeList.Tables[0].Rows[0]["Role"].ToString()),
                            DeptId = dsEmployeeList.Tables[0].Rows[0]["dept_id"].ToString(),
                            employment_type = dsEmployeeList.Tables[0].Rows[0]["employment_type"].ToString(),
                            qualification = dsEmployeeList.Tables[0].Rows[0]["qualification"].ToString(),
                            years_exp = dsEmployeeList.Tables[0].Rows[0]["years_exp"].ToString()
                        };

                        decimal dValue;

                        if (decimal.TryParse(dsEmployeeList.Tables[0].Rows[0]["Transport_allow"].ToString(), out dValue))
                        {
                            objEmployee.Transport_allow = dValue;
                        }

                        if (decimal.TryParse(dsEmployeeList.Tables[0].Rows[0]["Basic_Salary"].ToString(), out dValue))
                        {
                            objEmployee.Basic_Salary = dValue;
                        }

                        if (decimal.TryParse(dsEmployeeList.Tables[0].Rows[0]["Acc_allow"].ToString(), out dValue))
                        {
                            objEmployee.Acc_allow = dValue;
                        }

                        if (decimal.TryParse(dsEmployeeList.Tables[0].Rows[0]["Food_allow"].ToString(), out dValue))
                        {
                            objEmployee.Food_allow = dValue;
                        }

                        if (decimal.TryParse(dsEmployeeList.Tables[0].Rows[0]["Other_allow"].ToString(), out dValue))
                        {
                            objEmployee.Other_allow = dValue;
                        }

                        DateTime dateValue;
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Passport_expiry"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Passport_expiry = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Labour_cardexpiry"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Labour_cardexpiry = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Eid_visa_date"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Eid_visa_date = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["visa_Exp_date"].ToString(), out dateValue) == true)
                        {
                            objEmployee.visa_Exp_date = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Visa_stamped_on"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Visa_stamped_on = dateValue;
                        }

                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Health_Insurance_Expiry"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Health_Insurance_Expiry = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Date_of_join"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Date_of_join = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Date_of_exit"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Date_of_exit = dateValue;
                        }

                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["Date_of_Birth"].ToString(), out dateValue) == true)
                        {
                            objEmployee.Date_of_Birth = dateValue;
                        }

                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["CompetancyFromDate"].ToString(), out dateValue))
                        {
                            objEmployee.CompetancyFromDate = dateValue;
                        }

                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["CompetancyToDate"].ToString(), out dateValue))
                        {
                            objEmployee.CompetancyToDate = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["HealthCardIssueDate"].ToString(), out dateValue))
                        {
                            objEmployee.HealthCardIssueDate = dateValue;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["HealthCardExpDate"].ToString(), out dateValue))
                        {
                            objEmployee.HealthCardExpDate = dateValue;
                        }
                        
                        ViewBag.Location = objGlobaldata.GetDivisionLocationList(dsEmployeeList.Tables[0].Rows[0]["division"].ToString());
                        ViewBag.DeptList = objGlobaldata.GetDepartmentListbox(dsEmployeeList.Tables[0].Rows[0]["division"].ToString());//GetDepartmentList();
                        ViewBag.Visa_Type = objGlobaldata.GetConstantValue("Visa_Type");
                        ViewBag.Marital_status = objGlobaldata.GetConstantValue("Marital_status");
                        ViewBag.Gender = objGlobaldata.GetConstantValue("Gender");
                        ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList("");
                        //ViewBag.CustomerList = objGlobaldata.GetCustomerListbox();
                         ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.Division = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Roles = objGlobaldata.GetRoles();
                        ViewBag.EmploymentType = objGlobaldata.GetDropdownList("Employment Type");
                        ViewBag.Qualification = objGlobaldata.GetDropdownList("Employee Qualification");
                        return View(objEmployee); 
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("EmployeeList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeEdit(EmployeeMasterModels objEmployeeModel, FormCollection form, HttpPostedFileBase ProfilePic,
            HttpPostedFileBase JobDesc, HttpPostedFileBase CompetancyDoc, HttpPostedFileBase PassportUpload, HttpPostedFileBase EIDUpload,
            HttpPostedFileBase HealthCardUpload, HttpPostedFileBase Visa_upload)
        {
            try
            {

                objEmployeeModel.emp_no = form["emp_no"];
                objEmployeeModel.dept_id = form["DeptID"];
                objEmployeeModel.Emp_work_location = form["Emp_work_location"];
                objEmployeeModel.DeptInCharge = form["DeptInCharge"];
                objEmployeeModel.emp_id = form["emp_id"];
                objEmployeeModel.Role = form["Role"];
                objEmployeeModel.qualification = form["qualification"];
                DateTime dateValue;

                if (DateTime.TryParse(form["Passport_expiry"], out dateValue) == true)
                {
                    objEmployeeModel.Passport_expiry = dateValue;
                }
                if (DateTime.TryParse(form["Labour_cardexpiry"], out dateValue) == true)
                {
                    objEmployeeModel.Labour_cardexpiry = dateValue;
                }
                if (DateTime.TryParse(form["Eid_visa_date"], out dateValue) == true)
                {
                    objEmployeeModel.Eid_visa_date = dateValue;
                }
                if (DateTime.TryParse(form["visa_Exp_date"], out dateValue) == true)
                {
                    objEmployeeModel.visa_Exp_date = dateValue;
                }
                if (DateTime.TryParse(form["Visa_stamped_on"], out dateValue) == true)
                {
                    objEmployeeModel.Visa_stamped_on = dateValue;
                }

                if (DateTime.TryParse(form["Health_Insurance_Expiry"], out dateValue) == true)
                {
                    objEmployeeModel.Health_Insurance_Expiry = dateValue;
                }
                if (DateTime.TryParse(form["Date_of_join"], out dateValue) == true)
                {
                    objEmployeeModel.Date_of_join = dateValue;
                }
                if (DateTime.TryParse(form["Date_of_exit"], out dateValue) == true)
                {
                    objEmployeeModel.Date_of_exit = dateValue;
                }

                if (objEmployeeModel.Date_of_exit != null && objEmployeeModel.Date_of_exit > Convert.ToDateTime("01/01/0001"))
                {
                    if (objEmployeeModel.FunDeleteEmployee(objEmployeeModel.emp_no))
                    {
                        TempData["Successdata"] = "Employee Terminated Successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (DateTime.TryParse(form["Date_of_Birth"], out dateValue) == true)
                {
                    objEmployeeModel.Date_of_Birth = dateValue;
                }

                if (DateTime.TryParse(form["CompetancyFromDate"], out dateValue) == true)
                {
                    objEmployeeModel.CompetancyFromDate = dateValue;
                }

                if (DateTime.TryParse(form["CompetancyToDate"], out dateValue) == true)
                {
                    objEmployeeModel.CompetancyToDate = dateValue;
                }


                objEmployeeModel.EvaluatedBy = form["EvaluatedBy"];

                if (ProfilePic != null && ProfilePic.ContentLength > 0)
                {
                    try
                    {
                        //file.FileName=DateTime.Now.ToString().Replace('/',' ');
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/ProfilePic"), Path.GetFileName(ProfilePic.FileName));
                        string sFileExt = Path.GetExtension(ProfilePic.FileName), sFilepath = Path.GetDirectoryName(spath);
                        ProfilePic.SaveAs(sFilepath + "/" + objEmployeeModel.emp_firstname + sFileExt);
                        objEmployeeModel.ProfilePic = "~/DataUpload/ProfilePic/" + objEmployeeModel.emp_firstname + sFileExt;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in EmployeeEdit-profile pic upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }

                if (JobDesc != null && JobDesc.ContentLength > 0)
                {
                    try
                    {

                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(JobDesc.FileName));
                        string sFilename = objEmployeeModel.emp_firstname + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        JobDesc.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeModel.JobDesc = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-job desc upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }

                if (CompetancyDoc != null && CompetancyDoc.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(CompetancyDoc.FileName));
                        string sFileExt = Path.GetExtension(CompetancyDoc.FileName), sFilepath = Path.GetDirectoryName(spath);
                        CompetancyDoc.SaveAs(sFilepath + "/" + objEmployeeModel.emp_firstname + "_CompDoc_" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFileExt);
                        objEmployeeModel.CompetancyDoc = "~/DataUpload/MgmtDocs/Employee/" + objEmployeeModel.emp_firstname + "_CompDoc_" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFileExt;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-competancydoc upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (PassportUpload != null && PassportUpload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(PassportUpload.FileName));
                        string sFilename = "Passport" + objEmployeeModel.emp_firstname + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        PassportUpload.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeModel.PassportUpload = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-Passport upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (EIDUpload != null && EIDUpload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(EIDUpload.FileName));
                        string sFilename = "EmeratesID" + objEmployeeModel.emp_firstname + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        EIDUpload.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeModel.EIDUpload = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-EID upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (HealthCardUpload != null && HealthCardUpload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(HealthCardUpload.FileName));
                        string sFilename = "HealthCard" + objEmployeeModel.emp_firstname + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        HealthCardUpload.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeModel.HealthCardUpload = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-HealthCard upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                if (Visa_upload != null && Visa_upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(Visa_upload.FileName));
                        string sFilename = "Visa" + objEmployeeModel.emp_firstname + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        Visa_upload.SaveAs(sFilepath + "/" + sFilename);
                        objEmployeeModel.Visa_upload = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployee-Visa upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                //if (form["Gender"] != null && form["Gender"] != "")
                //{
                //    objEmployeeModel.Gender = form["Gender"];
                //}
                //if (form["Marital_status"] != null && form["Marital_status"] != "")
                //{
                //    objEmployeeModel.Marital_status = form["Marital_status"];
                //}
                //if (form["Visa_Type"] != null && form["Visa_Type"] != "")
                //{
                //    objEmployeeModel.Visa_Type = form["Visa_Type"];
                //}


                if (objEmployeeModel.FunUpdateEmployeeMaster(objEmployeeModel))
                {
                    TempData["Successdata"] = "Employee details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList");
        }

        [HttpPost]
        public ActionResult EmployeeDelete(FormCollection form)
        {
            string sEmp_no = form["emp_no"];


            if (sEmp_no != "")
            {
                try
                {
                    EmployeeMasterModels objEmployeeModel = new EmployeeMasterModels();

                    if (objEmployeeModel.FunDeleteEmployee(sEmp_no))
                    {
                        TempData["Successdata"] = "Employee details deleted successfully";
                        return Json("Delete successfully");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        return Json("Delete Failed");
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in EmployeeDelete: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            return Json("Delete Failed, Customer Id null");
        }

        public ActionResult AddEmployeeDependents()
        {
            EmployeeDependentModels objEmployee = new EmployeeDependentModels();

            try
            {
                ViewBag.EmpRel = objGlobaldata.GetDropdownList("Employee Relationship");
                ViewBag.Gender = objGlobaldata.GetConstantValue("Gender");
                if (Request.QueryString["emp_no"] != null && Request.QueryString["emp_no"] != "")
                {
                    string semp_no = Request.QueryString["emp_no"];
                    string sSqlStmt = "select emp_no from t_hr_employee where emp_no='" + semp_no + "'";
                    DataSet dsEmpDepList = objGlobaldata.Getdetails(sSqlStmt);
                    if (dsEmpDepList.Tables.Count > 0 && dsEmpDepList.Tables[0].Rows.Count > 0)
                    {
                        objEmployee = new EmployeeDependentModels
                        {
                            emp_no = Convert.ToInt32(dsEmpDepList.Tables[0].Rows[0]["emp_no"].ToString()),

                        };
                        return View(objEmployee);
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeeDependents: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployeeDependents(EmployeeDependentModels objEmp, FormCollection form)
        {
            try
            {

                EmployeeDependentModelsList lstemp = new EmployeeDependentModelsList();
                lstemp.EmpDepList = new List<EmployeeDependentModels>();


                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    EmployeeDependentModels objEmpDep = new EmployeeDependentModels();
                    DateTime dateValues;
                    objEmpDep.FirstName = form["FirstName" + i];
                    objEmpDep.LastName = form["LastName" + i];
                    objEmpDep.Gender = form["Gender" + i];
                    objEmpDep.Relationship = form["Relationship" + i];
                    objEmpDep.PassportNo = form["PassportNo" + i];
                    objEmpDep.VisaNo = form["VisaNo" + i];
                    objEmpDep.EIDNo = form["EIDNo" + i];
                    objEmpDep.HealthInsProvider = form["HealthInsProvider" + i];
                    if (DateTime.TryParse(form["DOB" + i], out dateValues) == true)
                    {
                        objEmpDep.DOB = dateValues;
                    }
                    if (DateTime.TryParse(form["PassportExpDate" + i], out dateValues) == true)
                    {
                        objEmpDep.PassportExpDate = dateValues;
                    }
                    if (DateTime.TryParse(form["VisaExpDate" + i], out dateValues) == true)
                    {
                        objEmpDep.VisaExpDate = dateValues;
                    }
                    if (DateTime.TryParse(form["EIDExpDate" + i], out dateValues) == true)
                    {
                        objEmpDep.EIDExpDate = dateValues;
                    }
                    if (DateTime.TryParse(form["HealthInsExp" + i], out dateValues) == true)
                    {
                        objEmpDep.HealthInsExp = dateValues;
                    }

                    lstemp.EmpDepList.Add(objEmpDep);
                }

                if (objEmp.FunAddEmpDependentDetails(objEmp, lstemp))
                {
                    TempData["Successdata"] = "Employee Dependent details added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeeDependents: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList");
        }

        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase Upload = Request.Files;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "PassDoc" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/Employee/" + "PassDoc" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
            }

            return Json("Failed");
        }
        [HttpPost]
        public JsonResult UploadMultipleDocument()
        {
            HttpFileCollectionBase upload = Request.Files;
            DocumentTrackingModels obj = new DocumentTrackingModels();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + sFilename);
                //return Json("~/DataUpload/MgmtDocs/Surveillance/" + "Surveillance" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                obj.upload = obj.upload + "," + "~/DataUpload/MgmtDocs/Employee/" + sFilename;

            }
            obj.upload = obj.upload.Trim(',');
            return Json(obj.upload);
        }
        public ActionResult AddEmployeePassDetails()
        {
            EmployeePassModels objEmployee = new EmployeePassModels();
            try
            {
                ViewBag.PassType = objGlobaldata.GetDropdownList("Employee Pass Type");
                if (Request.QueryString["emp_no"] != null && Request.QueryString["emp_no"] != "")
                {
                    string semp_no = Request.QueryString["emp_no"];
                    string sSqlStmt = "select emp_no from t_hr_employee where emp_no='" + semp_no + "'";
                    DataSet dsEmpDepList = objGlobaldata.Getdetails(sSqlStmt);
                    if (dsEmpDepList.Tables.Count > 0 && dsEmpDepList.Tables[0].Rows.Count > 0)
                    {
                        objEmployee = new EmployeePassModels
                        {
                            emp_no = Convert.ToInt32(dsEmpDepList.Tables[0].Rows[0]["emp_no"].ToString()),

                        };
                        return View(objEmployee);
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeePassDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployeePassDetails(EmployeePassModels objEmp, FormCollection form)
        {
            try
            {

                EmployeePassModelsList lstemp = new EmployeePassModelsList();
                lstemp.EmpPassList = new List<EmployeePassModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    EmployeePassModels objEmpPass = new EmployeePassModels();
                    DateTime dateValues;
                    if (form["PassType" + i] != null && form["Upload" + i] != null)
                    { 
                    objEmpPass.PassType = form["PassType" + i];
                    objEmpPass.Upload = form["Upload" + i];

                    if (DateTime.TryParse(form["ExpDate" + i], out dateValues) == true)
                    {
                        objEmpPass.ExpDate = dateValues;
                    }

                    lstemp.EmpPassList.Add(objEmpPass);
                   }
                }

                if (objEmp.FunAddEmpPassDetails(objEmp, lstemp))
                {
                    TempData["Successdata"] = "Employee Pass details added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeeDependents: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList");
        }

        [AllowAnonymous]
        public ActionResult EmployeePassList(string SearchText, int? page)
        {

            EmployeePassModelsList Emplst = new EmployeePassModelsList();
            Emplst.EmpPassList = new List<EmployeePassModels>();

            try
            {
                if (Request.QueryString["emp_no"] != null && Request.QueryString["emp_no"] != "")
                {
                    string semp_no = Request.QueryString["emp_no"];
                    string sSqlstmt = "select id_pass,emp_no,PassType,Upload,ExpDate from t_hr_employee_pass where emp_no='" + semp_no + "' and Active=1 order by PassType";

                    DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                    {


                        for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EmployeePassModels objEmployee = new EmployeePassModels
                                {
                                    id_pass = Convert.ToInt32(dsEmployeeList.Tables[0].Rows[i]["id_pass"].ToString()),
                                    emp_no = Convert.ToInt32(dsEmployeeList.Tables[0].Rows[i]["emp_no"].ToString()),
                                    PassType = objGlobaldata.GetDropdownitemById(dsEmployeeList.Tables[0].Rows[i]["PassType"].ToString()),
                                    Upload = dsEmployeeList.Tables[0].Rows[i]["Upload"].ToString(),

                                };

                                DateTime dtDate;
                                if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["ExpDate"].ToString(), out dtDate))
                                {
                                    objEmployee.ExpDate = dtDate;
                                }
                                Emplst.EmpPassList.Add(objEmployee);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in EmployeeList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeePassList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(Emplst.EmpPassList.ToList());
        }

        [AllowAnonymous]
        public ActionResult EmployeePassEdit()
        {

            EmployeePassModels objEmp = new EmployeePassModels();

            try
            {
                ViewBag.PassType = objGlobaldata.GetDropdownList("Employee Pass Type");
                if (Request.QueryString["id_pass"] != null && Request.QueryString["id_pass"] != "")
                {
                    string sid_pass = Request.QueryString["id_pass"];
                    string sSqlstmt = "select id_pass,emp_no,PassType,Upload,ExpDate from t_hr_employee_pass where id_pass='" + sid_pass + "'";

                    DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                    {

                        objEmp = new EmployeePassModels
                        {
                            id_pass = Convert.ToInt32(dsEmployeeList.Tables[0].Rows[0]["id_pass"].ToString()),
                            emp_no = Convert.ToInt32(dsEmployeeList.Tables[0].Rows[0]["emp_no"].ToString()),
                            PassType = objGlobaldata.GetDropdownitemById(dsEmployeeList.Tables[0].Rows[0]["PassType"].ToString()),
                            Upload = dsEmployeeList.Tables[0].Rows[0]["Upload"].ToString(),

                        };
                        DateTime dtDate;
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["ExpDate"].ToString(), out dtDate))
                        {
                            objEmp.ExpDate = dtDate;
                        }
                    }
                    else
                    {

                        TempData["alertdata"] = "PassID cannot be Null or empty";
                        return RedirectToAction("EmployeeList");
                    }
                }
                else
                {

                    TempData["alertdata"] = "PassID cannot be Null or empty";
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeePassEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EmployeeList");
            }
            return View(objEmp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeePassEdit(EmployeePassModels objEmp, FormCollection form, HttpPostedFileBase Upload)
        {
            try
            {
                objEmp.PassType = form["PassType"];
                DateTime dateValue;

                if (DateTime.TryParse(form["ExpDate"], out dateValue) == true)
                {
                    objEmp.ExpDate = dateValue;
                }
                if (Upload != null && Upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(Upload.FileName));
                        string sFilename = "PassDoc" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Upload.SaveAs(sFilepath + "/" + sFilename);
                        objEmp.Upload = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in EmployeePassEditUpload: " + ex.ToString());
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objEmp.FunUpdatePass(objEmp))
                {
                    TempData["Successdata"] = "Pass details Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeePassEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EmployeeList");
            }
            return RedirectToAction("EmployeeList");
        }

        public JsonResult PassDelete(FormCollection form)
        {
            string semp_no = form["emp_no"];
            try
            {

                   if (form["id_pass"] != null && form["id_pass"] != "")
                    {

                        EmployeePassModels obj = new EmployeePassModels();
                        string sid_pass = form["id_pass"];

                        if (obj.FunDeletePass(sid_pass))
                        {
                            TempData["Successdata"] = "Pass details deleted successfully";

                        }
                        else
                        {
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            return Json("Success");
                        }
                   
                }
                else
                {
                    TempData["alertdata"] = "Access Denied";
                    return Json("Failed");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PassDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public JsonResult DependentsDelete(FormCollection form)
        {
            string semp_no = form["emp_no"];
            try
            {

                 if (form["id_hr_emp_dependents"] != null && form["id_hr_emp_dependents"] != "")
                    {

                        EmployeePassModels obj = new EmployeePassModels();
                        string id_hr_emp_dependents = form["id_hr_emp_dependents"];

                        if (obj.FunDeleteDependents(id_hr_emp_dependents))
                        {
                            TempData["Successdata"] = "Details deleted successfully";

                        }
                        else
                        {
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            return Json("Success");
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
                objGlobaldata.AddFunctionalLog("Exception in DependentsDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }


        [AllowAnonymous]
        public ActionResult EmployeeDependentList(string SearchText, int? page)
        {

            EmployeeDependentModelsList Emplst = new EmployeeDependentModelsList();
            Emplst.EmpDepList = new List<EmployeeDependentModels>();

            try
            {
                if (Request.QueryString["emp_no"] != null && Request.QueryString["emp_no"] != "")
                {
                    string semp_no = Request.QueryString["emp_no"];
                    string sSqlstmt = "select id_hr_emp_dependents,emp_no,FirstName,LastName,DOB,Gender,Relationship,PassportNo,PassportExpDate,EIDNo,EIDExpDate,HealthInsProvider,HealthInsExp,VisaNo,VisaExpDate from t_hr_employee_dependents where emp_no='" + semp_no + "' and Active=1 order by id_hr_emp_dependents";

                    DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                    {                     

                        for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EmployeeDependentModels objEmployee = new EmployeeDependentModels
                                {
                                    id_hr_emp_dependents = Convert.ToInt32(dsEmployeeList.Tables[0].Rows[i]["id_hr_emp_dependents"].ToString()),
                                    emp_no = Convert.ToInt32(dsEmployeeList.Tables[0].Rows[i]["emp_no"].ToString()),
                                    FirstName = (dsEmployeeList.Tables[0].Rows[i]["FirstName"].ToString()),
                                    LastName = dsEmployeeList.Tables[0].Rows[i]["LastName"].ToString(),
                                    Gender = dsEmployeeList.Tables[0].Rows[i]["Gender"].ToString(),
                                    Relationship =objGlobaldata.GetDropdownitemById(dsEmployeeList.Tables[0].Rows[i]["Relationship"].ToString()),
                                    PassportNo = dsEmployeeList.Tables[0].Rows[i]["PassportNo"].ToString(),
                                    EIDNo = dsEmployeeList.Tables[0].Rows[i]["EIDNo"].ToString(),
                                    HealthInsProvider = dsEmployeeList.Tables[0].Rows[i]["HealthInsProvider"].ToString(),
                                    VisaNo = dsEmployeeList.Tables[0].Rows[i]["VisaNo"].ToString(),
                                };

                                DateTime dtDate;
                                if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["DOB"].ToString(), out dtDate))
                                {
                                    objEmployee.DOB = dtDate;
                                }
                                if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["PassportExpDate"].ToString(), out dtDate))
                                {
                                    objEmployee.PassportExpDate = dtDate;
                                }
                                if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["EIDExpDate"].ToString(), out dtDate))
                                {
                                    objEmployee.EIDExpDate = dtDate;
                                }
                                if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["HealthInsExp"].ToString(), out dtDate))
                                {
                                    objEmployee.HealthInsExp = dtDate;
                                }
                                if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["VisaExpDate"].ToString(), out dtDate))
                                {
                                    objEmployee.VisaExpDate = dtDate;
                                }
                                Emplst.EmpDepList.Add(objEmployee);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in EmployeeDependentList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeDependentList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(Emplst.EmpDepList.ToList());
        }

        public ActionResult EmployeeDependentsEdit()
        {

            EmployeeDependentModels objEmp = new EmployeeDependentModels();

            try
            {
                ViewBag.EmpRel = objGlobaldata.GetDropdownList("Employee Relationship");
                ViewBag.Gender = objGlobaldata.GetConstantValue("Gender");
                if (Request.QueryString["id_hr_emp_dependents"] != null && Request.QueryString["id_hr_emp_dependents"] != "")
                {
                    string id_hr_emp_dependents = Request.QueryString["id_hr_emp_dependents"];
                    string sSqlstmt = "select id_hr_emp_dependents,emp_no,FirstName,LastName,DOB,Gender,Relationship,PassportNo,PassportExpDate,EIDNo,EIDExpDate,HealthInsProvider,HealthInsExp,VisaNo,VisaExpDate from t_hr_employee_dependents where id_hr_emp_dependents='" + id_hr_emp_dependents + "'";

                    DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                    {

                        objEmp = new EmployeeDependentModels
                        {
                            id_hr_emp_dependents = Convert.ToInt32(dsEmployeeList.Tables[0].Rows[0]["id_hr_emp_dependents"].ToString()),
                            emp_no = Convert.ToInt32(dsEmployeeList.Tables[0].Rows[0]["emp_no"].ToString()),
                            FirstName = (dsEmployeeList.Tables[0].Rows[0]["FirstName"].ToString()),
                            LastName = dsEmployeeList.Tables[0].Rows[0]["LastName"].ToString(),
                            Gender = dsEmployeeList.Tables[0].Rows[0]["Gender"].ToString(),
                            Relationship = objGlobaldata.GetDropdownitemById(dsEmployeeList.Tables[0].Rows[0]["Relationship"].ToString()),
                            PassportNo = dsEmployeeList.Tables[0].Rows[0]["PassportNo"].ToString(),
                            EIDNo = dsEmployeeList.Tables[0].Rows[0]["EIDNo"].ToString(),
                            HealthInsProvider = dsEmployeeList.Tables[0].Rows[0]["HealthInsProvider"].ToString(),
                            VisaNo = dsEmployeeList.Tables[0].Rows[0]["VisaNo"].ToString(),
                        };
                        DateTime dtDate;
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["DOB"].ToString(), out dtDate))
                        {
                            objEmp.DOB = dtDate;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["PassportExpDate"].ToString(), out dtDate))
                        {
                            objEmp.PassportExpDate = dtDate;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["EIDExpDate"].ToString(), out dtDate))
                        {
                            objEmp.EIDExpDate = dtDate;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["HealthInsExp"].ToString(), out dtDate))
                        {
                            objEmp.HealthInsExp = dtDate;
                        }
                        if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[0]["VisaExpDate"].ToString(), out dtDate))
                        {
                            objEmp.VisaExpDate = dtDate;
                        }
                    }
                    else
                    {

                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("EmployeeList");
                    }
                }
                else
                {

                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeDependentsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("EmployeeList");
            }
            return View(objEmp);
        }

        [HttpPost]
        public JsonResult FunGetRelationshipID(string Relationship)
        {
            string sRelationship = ""; ;
            if (Relationship != null)
            {
                sRelationship = objGlobaldata.GetEmployeeRealtionshipsIdByName(Relationship);
            }
            return Json(sRelationship);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeDependentsEdit(EmployeeDependentModels objEmp, FormCollection form)
        {
            try
            {
               
                    DateTime dateValues;
                    objEmp.FirstName = form["FirstName"];
                    objEmp.LastName = form["LastName"];
                    objEmp.Gender = form["Gender"];
                    objEmp.Relationship = form["Relationship"];
                    objEmp.PassportNo = form["PassportNo"];
                    objEmp.VisaNo = form["VisaNo"];
                    objEmp.EIDNo = form["EIDNo"];
                    objEmp.HealthInsProvider = form["HealthInsProvider"];
                    if (DateTime.TryParse(form["DOB"], out dateValues) == true)
                    {
                        objEmp.DOB = dateValues;
                    }
                    if (DateTime.TryParse(form["PassportExpDate"], out dateValues) == true)
                    {
                        objEmp.PassportExpDate = dateValues;
                    }
                    if (DateTime.TryParse(form["EIDExpDate"], out dateValues) == true)
                    {
                        objEmp.EIDExpDate = dateValues;
                    }
                    if (DateTime.TryParse(form["HealthInsExp"], out dateValues) == true)
                    {
                        objEmp.HealthInsExp = dateValues;
                    }
                   if (DateTime.TryParse(form["VisaExpDate"], out dateValues) == true)
                   {
                      objEmp.VisaExpDate = dateValues;
                   }
                if (objEmp.FunUpdateEmpDependant(objEmp))
                    {
                        TempData["Successdata"] = "Employee Dependent details updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeDependentsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("EmployeeList");
        }

        [AllowAnonymous]
        public ActionResult ExitEmployeeList(string SearchText, int? page, string Department, string branch_name)
        {
            ViewBag.SubMenutype = "EmployeeDetails";
            EmployeeMasterModelList objEmployeeModelList = new EmployeeMasterModelList();
            objEmployeeModelList.EmployeeList = new List<EmployeeMasterModels>();
            ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select emp_no, emp_id, emp_lastname, emp_firstname, emp_middlename, Nationaliity, Designation,"
                    + " Gender,Marital_status,EmailId, MobileNo,Emp_work_location,Date_of_exit,Date_of_Birth,dept_id from t_hr_employee where emp_status=0 and Date_of_exit > (0001-01-01) ";
                string sSearchtext = "";
                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + " and (emp_firstname ='" + SearchText + "' or emp_firstname like '" + SearchText + "%' or emp_firstname ='%" + SearchText + "' or emp_firstname like '%" + SearchText + "%'"
                    + " or emp_lastname ='" + SearchText + "' or emp_lastname like '" + SearchText + "%' or emp_lastname ='%" + SearchText + "' or emp_lastname like '%" + SearchText + "%')";
                }
                if (Department != null && Department != "")
                {
                    sSearchtext = sSearchtext + " and Dept_Id='" + Department + "'";
                    ViewBag.DeptSearch = objGlobaldata.GetDeptNameById(Department);
                }
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and division='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and division='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by emp_firstname";
                DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                {                

                    for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmployeeMasterModels objEmployee = new EmployeeMasterModels
                            {
                                emp_no = dsEmployeeList.Tables[0].Rows[i]["emp_no"].ToString(),
                                MobileNo = (dsEmployeeList.Tables[0].Rows[i]["MobileNo"].ToString()),
                                emp_id = (dsEmployeeList.Tables[0].Rows[i]["emp_id"].ToString()),
                                emp_firstname = dsEmployeeList.Tables[0].Rows[i]["emp_firstname"].ToString(),
                                emp_middlename = dsEmployeeList.Tables[0].Rows[i]["emp_middlename"].ToString(),
                                emp_lastname = dsEmployeeList.Tables[0].Rows[i]["emp_lastname"].ToString(),
                                EmailId = dsEmployeeList.Tables[0].Rows[i]["EmailId"].ToString(),
                                dept_id = objGlobaldata.GetDeptNameById(dsEmployeeList.Tables[0].Rows[i]["dept_id"].ToString()),
                                Designation = dsEmployeeList.Tables[0].Rows[i]["Designation"].ToString(),
                                Emp_work_location = objGlobaldata.GetDivisionLocationById(dsEmployeeList.Tables[0].Rows[i]["Emp_work_location"].ToString()),
                                Gender = dsEmployeeList.Tables[0].Rows[i]["Gender"].ToString(),
                            };

                            DateTime dtDate;
                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_exit"].ToString(), out dtDate))
                            {
                                objEmployee.Date_of_exit = dtDate;
                            }

                            objEmployeeModelList.EmployeeList.Add(objEmployee);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ExitEmployeeList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExitEmployeeList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEmployeeModelList.EmployeeList.ToList());
        }


        [AllowAnonymous]
        public JsonResult ExitEmployeeListSearch(string SearchText, int? page, string Department, string branch_name)
        {
            ViewBag.SubMenutype = "EmployeeDetails";
            EmployeeMasterModelList objEmployeeModelList = new EmployeeMasterModelList();
            objEmployeeModelList.EmployeeList = new List<EmployeeMasterModels>();
            ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select emp_no, emp_id, emp_lastname, emp_firstname, emp_middlename, Nationaliity, Designation,"
                    + " Gender,Marital_status,EmailId, MobileNo,Emp_work_location,Date_of_exit,Date_of_Birth,dept_id from t_hr_employee where emp_status=0 and Date_of_exit > (0001-01-01) ";
                string sSearchtext = "";
                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + " and (emp_firstname ='" + SearchText + "' or emp_firstname like '" + SearchText + "%' or emp_firstname ='%" + SearchText + "' or emp_firstname like '%" + SearchText + "%'"
                    + " or emp_lastname ='" + SearchText + "' or emp_lastname like '" + SearchText + "%' or emp_lastname ='%" + SearchText + "' or emp_lastname like '%" + SearchText + "%')";
                }
                if (Department != null && Department != "")
                {
                    sSearchtext = sSearchtext + " and Dept_Id='" + Department + "'";
                    ViewBag.DeptSearch = objGlobaldata.GetDeptNameById(Department);
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

                sSqlstmt = sSqlstmt + sSearchtext + " order by emp_firstname";
                DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                {

                  

                    for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmployeeMasterModels objEmployee = new EmployeeMasterModels
                            {
                                emp_no = dsEmployeeList.Tables[0].Rows[i]["emp_no"].ToString(),
                                MobileNo = (dsEmployeeList.Tables[0].Rows[i]["MobileNo"].ToString()),
                                emp_id = (dsEmployeeList.Tables[0].Rows[i]["emp_id"].ToString()),
                                emp_firstname = dsEmployeeList.Tables[0].Rows[i]["emp_firstname"].ToString(),
                                emp_middlename = dsEmployeeList.Tables[0].Rows[i]["emp_middlename"].ToString(),
                                emp_lastname = dsEmployeeList.Tables[0].Rows[i]["emp_lastname"].ToString(),
                                EmailId = dsEmployeeList.Tables[0].Rows[i]["EmailId"].ToString(),
                                dept_id = objGlobaldata.GetDeptNameById(dsEmployeeList.Tables[0].Rows[i]["dept_id"].ToString()),
                                Designation = dsEmployeeList.Tables[0].Rows[i]["Designation"].ToString(),
                                Emp_work_location = objGlobaldata.GetDivisionLocationById(dsEmployeeList.Tables[0].Rows[i]["Emp_work_location"].ToString()),
                                Gender = dsEmployeeList.Tables[0].Rows[i]["Gender"].ToString(),
                            };


                            DateTime dtDate;
                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["Date_of_exit"].ToString(), out dtDate))
                            {
                                objEmployee.Date_of_exit = dtDate;
                            }

                            objEmployeeModelList.EmployeeList.Add(objEmployee);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ExitEmployeeListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExitEmployeeListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }


        //Customer Visit

        [AllowAnonymous]
        public ActionResult AddVisitors()
        {
            ViewBag.SubMenutype = "Visitor";
            try
            {
                ViewBag.HSEIndReq = objGlobaldata.GetConstantValue("YesNo");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVisitors: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddVisitors(VisitorsModels objVisitor, FormCollection form, HttpPostedFileBase file)
        {
            ViewBag.SubMenutype = "Visitor";
            try
            {

                DateTime dateValue;

                if (DateTime.TryParse(form["visit_date"], out dateValue) == true)
                {
                    objVisitor.visit_date = dateValue;
                }

                //objCustomerVisitModels.Branch = form["Branch"];


                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(file.FileName));
                        string sFilename = Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objVisitor.upload = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddVisitors-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }


                if (objVisitor.FunAddVisitors(objVisitor))
                {
                    TempData["Successdata"] = "Added Visitor details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVisitors: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("VisitorList");
        }

        [AllowAnonymous]
        public ActionResult VisitorList(string SearchText, string hse_ind, string chkAll, int? Page, string branch_name)
        {
            ViewBag.SubMenutype = "Visitor";
            VisitorsModelList objVisitorList = new VisitorsModelList();
            objVisitorList.VisiorList = new List<VisitorsModels>();

            ViewBag.HSEIndReq = objGlobaldata.GetConstantValue("YesNo");
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "SELECT id_visitors,firstname, lastname,designation, contact_no, email, v_company, purpose_visit,hse_ind, visit_date,upload FROM t_visitors where Active=1";

                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;

                    if (SearchText != null && SearchText != "")
                    {
                        ViewBag.SearchText = SearchText;
                        sSearchtext = sSearchtext + " and (firstname ='" + SearchText + "' or firstname like '%" + SearchText + "%' or lastname ='" + SearchText + "' or lastname like '%" + SearchText + "%')";
                    }


                    if (hse_ind != null && hse_ind != "" && hse_ind != "Select")
                    {
                        ViewBag.HSEIND = hse_ind;

                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and (hse_ind ='" + hse_ind + "')";
                        }
                        else
                        {
                            sSearchtext = " and (hse_ind ='" + hse_ind + "')";

                        }
                    }

                }
                else
                {
                    ViewBag.chkAll = true;
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

                sSqlstmt = sSqlstmt + sSearchtext + " order by visit_date asc";
                DataSet dsVisitors = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsVisitors.Tables.Count > 0 && i < dsVisitors.Tables[0].Rows.Count; i++)
                {
                    try
                    {

                     

                        VisitorsModels objVisitor = new VisitorsModels
                        {
                            id_visitors = dsVisitors.Tables[0].Rows[i]["id_visitors"].ToString(),
                            firstname = dsVisitors.Tables[0].Rows[i]["firstname"].ToString(),
                            lastname = dsVisitors.Tables[0].Rows[i]["lastname"].ToString(),
                            designation = dsVisitors.Tables[0].Rows[i]["designation"].ToString().ToUpper(),
                            contact_no = dsVisitors.Tables[0].Rows[i]["contact_no"].ToString(),
                            email = dsVisitors.Tables[0].Rows[i]["email"].ToString(),
                            v_company = dsVisitors.Tables[0].Rows[i]["v_company"].ToString(),
                            purpose_visit = dsVisitors.Tables[0].Rows[i]["purpose_visit"].ToString(),
                            hse_ind = dsVisitors.Tables[0].Rows[i]["hse_ind"].ToString(),
                            upload = dsVisitors.Tables[0].Rows[i]["upload"].ToString(),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsVisitors.Tables[0].Rows[i]["visit_date"].ToString(), out dtValue))
                        {
                            objVisitor.visit_date = dtValue;
                        }

                        objVisitorList.VisiorList.Add(objVisitor);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in VisitorList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in VisitorList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objVisitorList.VisiorList.ToList());

        }

        [AllowAnonymous]
        public ActionResult VisitorListSearch(string SearchText, string hse_ind, string chkAll, int? Page, string branch_name)
        {
            ViewBag.SubMenutype = "Visitor";
            VisitorsModelList objVisitorList = new VisitorsModelList();
            objVisitorList.VisiorList = new List<VisitorsModels>();

            ViewBag.HSEIndReq = objGlobaldata.GetConstantValue("YesNo");
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "SELECT id_visitors,firstname, lastname,designation, contact_no, email, v_company, purpose_visit,hse_ind, visit_date,upload FROM t_visitors where Active=1";

                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;

                    if (SearchText != null && SearchText != "")
                    {
                        ViewBag.SearchText = SearchText;
                        sSearchtext = sSearchtext + " and (firstname ='" + SearchText + "' or firstname like '%" + SearchText + "%' or lastname ='" + SearchText + "' or lastname like '%" + SearchText + "%')";
                    }


                    if (hse_ind != null && hse_ind != "" && hse_ind != "Select")
                    {
                        ViewBag.HSEIND = hse_ind;

                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and (hse_ind ='" + hse_ind + "')";
                        }
                        else
                        {
                            sSearchtext = " and (hse_ind ='" + hse_ind + "')";

                        }
                    }

                }
                else
                {
                    ViewBag.chkAll = true;
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

                sSqlstmt = sSqlstmt + sSearchtext + " order by visit_date asc";
                DataSet dsVisitors = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsVisitors.Tables.Count > 0 && i < dsVisitors.Tables[0].Rows.Count; i++)
                {
                    try
                    {



                        VisitorsModels objVisitor = new VisitorsModels
                        {
                            id_visitors = dsVisitors.Tables[0].Rows[i]["id_visitors"].ToString(),
                            firstname = dsVisitors.Tables[0].Rows[i]["firstname"].ToString(),
                            lastname = dsVisitors.Tables[0].Rows[i]["lastname"].ToString(),
                            designation = dsVisitors.Tables[0].Rows[i]["designation"].ToString().ToUpper(),
                            contact_no = dsVisitors.Tables[0].Rows[i]["contact_no"].ToString(),
                            email = dsVisitors.Tables[0].Rows[i]["email"].ToString(),
                            v_company = dsVisitors.Tables[0].Rows[i]["v_company"].ToString(),
                            purpose_visit = dsVisitors.Tables[0].Rows[i]["purpose_visit"].ToString(),
                            hse_ind = dsVisitors.Tables[0].Rows[i]["hse_ind"].ToString(),
                            upload = dsVisitors.Tables[0].Rows[i]["upload"].ToString(),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsVisitors.Tables[0].Rows[i]["visit_date"].ToString(), out dtValue))
                        {
                            objVisitor.visit_date = dtValue;
                        }

                        objVisitorList.VisiorList.Add(objVisitor);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in VisitorListSearch: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in VisitorListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult VisitorEdit()
        {
            ViewBag.SubMenutype = "Visitor";
            ViewBag.HSEIndReq = objGlobaldata.GetConstantValue("YesNo");
            try
            {
                if (Request.QueryString["id_visitors"] != null && Request.QueryString["id_visitors"] != "")
                {
                    string sid_visitors = Request.QueryString["id_visitors"];

                    string sSqlstmt = "SELECT id_visitors, firstname, lastname,designation, contact_no, email, v_company, purpose_visit,hse_ind, visit_date,upload FROM t_visitors where id_visitors='" + sid_visitors + "'";

                    DataSet dsVisitors = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsVisitors.Tables.Count > 0 && dsVisitors.Tables[0].Rows.Count > 0)
                    {
                        VisitorsModels objVisitor = new VisitorsModels
                        {
                            id_visitors = dsVisitors.Tables[0].Rows[0]["id_visitors"].ToString(),
                            firstname = dsVisitors.Tables[0].Rows[0]["firstname"].ToString(),
                            lastname = dsVisitors.Tables[0].Rows[0]["lastname"].ToString(),
                            designation = dsVisitors.Tables[0].Rows[0]["designation"].ToString().ToUpper(),
                            contact_no = dsVisitors.Tables[0].Rows[0]["contact_no"].ToString(),
                            email = dsVisitors.Tables[0].Rows[0]["email"].ToString(),
                            v_company = dsVisitors.Tables[0].Rows[0]["v_company"].ToString(),
                            purpose_visit = dsVisitors.Tables[0].Rows[0]["purpose_visit"].ToString(),
                            hse_ind = dsVisitors.Tables[0].Rows[0]["hse_ind"].ToString(),
                            upload = dsVisitors.Tables[0].Rows[0]["upload"].ToString(),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsVisitors.Tables[0].Rows[0]["visit_date"].ToString(), out dtValue))
                        {
                            objVisitor.visit_date = dtValue;
                        }

                        return View(objVisitor);

                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("VisitorList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("VisitorList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in VisitorEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("VisitorList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VisitorEdit(VisitorsModels objVisitor, FormCollection form, HttpPostedFileBase file)
        {
            ViewBag.SubMenutype = "Visitor";
            try
            {

                DateTime dateValue;

                if (DateTime.TryParse(form["visit_date"], out dateValue) == true)
                {
                    objVisitor.visit_date = dateValue;
                }
                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(file.FileName));
                        string sFilename = Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objVisitor.upload = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in VisitorEdit-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objVisitor.FunUpdateVisitors(objVisitor))
                {
                    TempData["Successdata"] = "Updated Visitor details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in VisitorEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("VisitorList");
        }

        [AllowAnonymous]
        public JsonResult VisitorDelete(FormCollection form)
        {
            try
            {            
                    if (form["id_visitors"] != null && form["id_visitors"] != "")
                    {

                        VisitorsModels Doc = new VisitorsModels();
                        string sid_visitors = form["id_visitors"];


                        if (Doc.FunDeleteVisitors(sid_visitors))
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
                        TempData["alertdata"] = "Customer Visit Id cannot be Null or empty";
                        return Json("Failed");
                    }
               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in VisitorDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        
        public ActionResult FunGetDeptList(string branch)
        {     
               MultiSelectList  lstDept = objGlobaldata.GetDepartmentListbox(branch);
               return Json(lstDept);
        }

        public ActionResult FunGetLocationList(string division)
        {
            MultiSelectList LocList = objGlobaldata.GetDivisionLocationList(division);
            return Json(LocList);
        }

        //Universal fucntion
        public ActionResult FunGetAllDeptList(string[] branch)
        {
            string Division = "";
            if (branch != null)
            { 
              Division = string.Join(",", branch);
            }
            MultiSelectList lstDept = objGlobaldata.GetDepartmentList1(Division);
            return Json(lstDept);
        }

        //Universal fucntion
        public ActionResult FunGetAllLocList(string[] branch)
        {
            string Division = "";
            if (branch != null && branch[0] != "")
            {
                Division = string.Join(",", branch);
            }
            MultiSelectList LocList = objGlobaldata.GetLocationbyMultiDivision(Division);
            return Json(LocList);
        }       

        public ActionResult FunGetEmpBydept(string dept)
        { 
            MultiSelectList LocList = objGlobaldata.GetHrEmpListByDept(dept);
            return Json(LocList);
        }

        public ActionResult FunGetEmpByMultiDept(string[] dept)
        {
            string Department = "";
            if (dept != null)
            {
                Department = string.Join(",", dept);
            }
            MultiSelectList EmpList = objGlobaldata.GetHrEmpListByDept(Department);
            return Json(EmpList);
        }

        //Universal fucntion
        public ActionResult FunGetAllEmpList(string[] branch)
        {
            string Division = "";
            if (branch != null)
            {
                Division = string.Join(",", branch);
            }
            MultiSelectList EmpList = objGlobaldata.GetHrEmpListByDivision(Division);
            return Json(EmpList);
        }

        //public ActionResult FunGetAllRoleList(string branch,string role)
        //{            
        //    MultiSelectList lstDept = objGlobaldata.GetDepartmentList1(branch,,role);
        //    return Json(lstDept);
        //}

        BranchModelsList printSeries(int m, int level, DataSet dsList, BranchModelsList objBranchModelList, BranchModels objModels)
        {
            int  count = 0;
            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
            {
                if (m ==Convert.ToInt32(dsList.Tables[0].Rows[i]["parent_level"].ToString()))
                {
                    bool value = objBranchModelList.BranchList.Exists(item => item.id == dsList.Tables[0].Rows[i]["id"].ToString());
                    if (!value)
                    {
                        objModels = new BranchModels
                        {
                            id = (dsList.Tables[0].Rows[i]["id"].ToString()),
                            text = (dsList.Tables[0].Rows[i]["text"].ToString()),
                            level_step = (dsList.Tables[0].Rows[i]["level_step"].ToString())
                        };
                        objBranchModelList.BranchList.Add(objModels);

                        level = Convert.ToInt32(dsList.Tables[0].Rows[i]["parent_level"].ToString());

                      return  objBranchModelList=printSeries(Convert.ToInt32(dsList.Tables[0].Rows[i]["id"].ToString()), level, dsList, objBranchModelList, objModels);
                    }                    
                }              
                    count++;               
            }

            if (level - 1 >= 0)
            {
                objBranchModelList=printSeries(Convert.ToInt32(dsList.Tables[0].Rows[level - 1]["id"].ToString()), Convert.ToInt32(dsList.Tables[0].Rows[level - 1]["parent_level"].ToString()), dsList, objBranchModelList, objModels);
            }            
                return objBranchModelList;            
        }

        public ActionResult FunGetBranchList()
        {
           
            DataSet dsList = new DataSet();
            BranchModelsList objBranchModelList = new BranchModelsList();
            objBranchModelList.BranchList = new List<BranchModels>();
            BranchModels objModels = new BranchModels();
            try
            {

                string sSqlstmt = "select id,BranchName as text,parent_level,level_step from t_company_branch where Active=1 order by parent_level asc";

                dsList = objGlobaldata.Getdetails(sSqlstmt);
                for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                {

                    if (dsList.Tables[0].Rows[i]["id"].ToString() == "1")
                    {
                    objModels = new BranchModels
                           {
                               id = (dsList.Tables[0].Rows[i]["id"].ToString()),
                               text = (dsList.Tables[0].Rows[i]["text"].ToString()),
                               level_step = (dsList.Tables[0].Rows[i]["level_step"].ToString())
                           };
                    objBranchModelList.BranchList.Add(objModels);
                    }
                }
                objBranchModelList=printSeries(Convert.ToInt32(dsList.Tables[0].Rows[0]["id"].ToString()), 0, dsList, objBranchModelList, objModels);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetDeptList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            //var json = JsonConvert.SerializeObject(objBranchModelList, Formatting.Indented);
            var json = new JavaScriptSerializer().Serialize(objBranchModelList.BranchList);

            return Json(json);

        }

        public JsonResult FunGetEmpDetails(string semp_no)
        {

            NCModels objModels = new NCModels();
            try
            {
                string sSqlstmt = "select emp_no,emp_id,division,Emp_work_location,Dept_Id,EmailId,Designation from t_hr_employee where emp_no = '" + semp_no + "'";
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
                        EmailId = (dsList.Tables[0].Rows[0]["EmailId"].ToString()),
                        Designation = (dsList.Tables[0].Rows[0]["Designation"].ToString())
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
        public ActionResult AddCompetenceDetails()
        {
           EmployeeMasterModels  objModels = new EmployeeMasterModels();
            try
            {

                if (Request.QueryString["emp_no"] != null && Request.QueryString["emp_no"] != "")
                {
                    string emp_no = Request.QueryString["emp_no"];

                    string sSqlstmt = "select emp_no,emp_id,emp_firstname,years_exp from t_hr_employee where emp_no=" + emp_no;

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objModels = new EmployeeMasterModels
                        {
                            emp_no = (dsModelsList.Tables[0].Rows[0]["emp_no"].ToString()),
                            emp_id = (dsModelsList.Tables[0].Rows[0]["emp_id"].ToString()),
                            emp_firstname = (dsModelsList.Tables[0].Rows[0]["emp_firstname"].ToString()),
                            years_exp = (dsModelsList.Tables[0].Rows[0]["years_exp"].ToString()),

                        };

                        ViewBag.qualification = objGlobaldata.GetDropdownList("Employee Qualification");
                        ViewBag.year = objGlobaldata.GetDropdownList("Years");

                        //Qualification

                        EmployeeMasterModelList objQModelsList = new EmployeeMasterModelList();
                        objQModelsList.EmployeeList = new List<EmployeeMasterModels>();

                        sSqlstmt = "select id_emp_qualification,emp_no,qualification,q_year,upload from t_hr_employee_qualification where emp_no='" + emp_no + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EmployeeMasterModels objQualific = new EmployeeMasterModels
                                    {
                                        id_emp_qualification = dsList.Tables[0].Rows[i]["id_emp_qualification"].ToString(),
                                        emp_no = dsList.Tables[0].Rows[i]["emp_no"].ToString(),
                                        qualification = dsList.Tables[0].Rows[i]["qualification"].ToString(),
                                        q_year = dsList.Tables[0].Rows[i]["q_year"].ToString(),
                                        upload = dsList.Tables[0].Rows[i]["upload"].ToString(),
                                    };
                                    objQModelsList.EmployeeList.Add(objQualific);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddCompetenceDetails: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("EmployeeList");
                                }
                            }
                            ViewBag.objQList = objQModelsList;
                        }


                        //Skills

                        EmployeeMasterModelList objSkillModelsList = new EmployeeMasterModelList();
                        objSkillModelsList.EmployeeList = new List<EmployeeMasterModels>();

                        sSqlstmt = "select id_emp_skill,emp_no,skill from t_hr_employee_skills where emp_no='" + emp_no + "'";
                        dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EmployeeMasterModels objQualific = new EmployeeMasterModels
                                    {
                                        id_emp_skill = dsList.Tables[0].Rows[i]["id_emp_skill"].ToString(),
                                        emp_no = dsList.Tables[0].Rows[i]["emp_no"].ToString(),
                                        skill = dsList.Tables[0].Rows[i]["skill"].ToString(),
                                       
                                    };
                                    objSkillModelsList.EmployeeList.Add(objQualific);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddCompetenceDetails: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("EmployeeList");
                                }
                            }
                            ViewBag.objSkillList = objSkillModelsList;
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCompetenceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddCompetenceDetails(EmployeeMasterModels objKPI, FormCollection form)
        {
            try
            {

                EmployeeMasterModelList objQModelsList = new EmployeeMasterModelList();
                objQModelsList.EmployeeList = new List<EmployeeMasterModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    EmployeeMasterModels objModels = new EmployeeMasterModels();
                    if (form["qualification " + i] != null && form["qualification " + i] != "")
                    {
                        objModels.qualification = form["qualification " + i];
                        objModels.q_year = form["q_year " + i];
                        objModels.upload = form["upload1 " + i];

                        objQModelsList.EmployeeList.Add(objModels);
                    }
                }

                EmployeeMasterModelList objSkillModelsList = new EmployeeMasterModelList();
                objSkillModelsList.EmployeeList = new List<EmployeeMasterModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    EmployeeMasterModels objModels = new EmployeeMasterModels();
                    if (form["skill " + i] != null && form["skill " + i] != "")
                    {
                        objModels.skill = form["skill " + i];

                        objSkillModelsList.EmployeeList.Add(objModels);
                    }
                }


                if (objKPI.FunAddCompetenceDetails(objQModelsList, objSkillModelsList))
                {
                    TempData["Successdata"] = "Added Competence Details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCompetenceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("EmployeeList");
        }


        [AllowAnonymous]
        public ActionResult CompetenceDetails()
        {
            EmployeeMasterModels objModels = new EmployeeMasterModels();
            try
            {

                if (Request.QueryString["emp_no"] != null && Request.QueryString["emp_no"] != "")
                {
                    string emp_no = Request.QueryString["emp_no"];

                    string sSqlstmt = "select emp_no,emp_id,emp_firstname,years_exp from t_hr_employee where emp_no=" + emp_no;

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objModels = new EmployeeMasterModels
                        {
                            emp_no = (dsModelsList.Tables[0].Rows[0]["emp_no"].ToString()),
                            emp_id = (dsModelsList.Tables[0].Rows[0]["emp_id"].ToString()),
                            emp_firstname = (dsModelsList.Tables[0].Rows[0]["emp_firstname"].ToString()),
                            years_exp = (dsModelsList.Tables[0].Rows[0]["years_exp"].ToString()),

                        };

                        ViewBag.qualification = objGlobaldata.GetDropdownList("Employee Qualification");
                        ViewBag.year = objGlobaldata.GetDropdownList("Years");

                        //Qualification

                        EmployeeMasterModelList objQModelsList = new EmployeeMasterModelList();
                        objQModelsList.EmployeeList = new List<EmployeeMasterModels>();

                        sSqlstmt = "select id_emp_qualification,emp_no,qualification,q_year,upload from t_hr_employee_qualification where emp_no='" + emp_no + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EmployeeMasterModels objQualific = new EmployeeMasterModels
                                    {
                                        id_emp_qualification = dsList.Tables[0].Rows[i]["id_emp_qualification"].ToString(),
                                        emp_no = dsList.Tables[0].Rows[i]["emp_no"].ToString(),
                                        qualification = dsList.Tables[0].Rows[i]["qualification"].ToString(),
                                        q_year = dsList.Tables[0].Rows[i]["q_year"].ToString(),
                                        upload = dsList.Tables[0].Rows[i]["upload"].ToString(),
                                    };
                                    objQModelsList.EmployeeList.Add(objQualific);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddCompetenceDetails: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("EmployeeList");
                                }
                            }
                            ViewBag.objQList = objQModelsList;
                        }


                        //Skills

                        EmployeeMasterModelList objSkillModelsList = new EmployeeMasterModelList();
                        objSkillModelsList.EmployeeList = new List<EmployeeMasterModels>();

                        sSqlstmt = "select id_emp_skill,emp_no,skill from t_hr_employee_skills where emp_no='" + emp_no + "'";
                        dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EmployeeMasterModels objQualific = new EmployeeMasterModels
                                    {
                                        id_emp_skill = dsList.Tables[0].Rows[i]["id_emp_skill"].ToString(),
                                        emp_no = dsList.Tables[0].Rows[i]["emp_no"].ToString(),
                                        skill = dsList.Tables[0].Rows[i]["skill"].ToString(),

                                    };
                                    objSkillModelsList.EmployeeList.Add(objQualific);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddCompetenceDetails: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("EmployeeList");
                                }
                            }
                            ViewBag.objSkillList = objSkillModelsList;
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CompetenceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objModels);
        }

    }
}
