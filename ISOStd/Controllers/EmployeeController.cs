using ISOStd.Filters;
using ISOStd.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class EmployeeController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public EmployeeController()
        {
            ViewBag.Menutype = "Settings";
        }

        //
        // GET: /Employee/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Employee/AddEmployee
        [HttpGet]
        public ActionResult PasswordExpiry()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPasswordExp(EmployeeModels objEmployeeModel, FormCollection form)
        {
            ViewBag.SubMenutype = "Employee";
            LoginModel model = new LoginModel();
            string sUsername = form["emailAddress"];
            string sPwd = form["OldPwd"];

            string stmt = "Update t_employee set logged_date='" + DateTime.Today.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where emailAddress='" + sUsername + "'";
            if (objGlobaldata.ExecuteQuery(stmt))
            {
                if (model.LoginAuthenticate(sUsername, sPwd))
                {
                    string sEmpID = objGlobaldata.GetEmpIDByUsername(sUsername, clsGlobal.Encrypt(sPwd));
                    if (objEmployeeModel.FunPwdReset(sEmpID, clsGlobal.Encrypt(objEmployeeModel.Pwd)))
                    {
                        TempData["Successdata"] = "Password reset successful";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            else
            {
                TempData["alertdata"] = "The UserID or password provided is incorrect.";
                return RedirectToAction("PasswordExpiry", "Employee");
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult AddEmployee()
        {
            //if (objGlobaldata.GetCompanyBranchNameById(objGlobaldata.GetCurrentUserSession().division).ToLower() == "all")
            //{
            try
            {
                ViewBag.SubMenutype = "Employee";
                //objGlobaldata.CreateUserSession();
                //UserCedentials objUserInfo = (UserCedentials)Session["UserCredentials"];
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                //  ViewBag.DeptList = objGlobaldata.GetDepartmentWithIdListbox();//GetDepartmentList();
                //ViewBag.Roles = objGlobaldata.GetRoles();
                // ViewBag.EmpType = objGlobaldata.GetConstantValue("EmpType");//new string[] { "Internal", "External" };
                // ViewBag.DeptInCharge = objGlobaldata.GetConstantValue("DeptInCharge"); //new string[] { "No", "Yes" };
                //  ViewBag.Work_Location = objGlobaldata.GetCompanyBranchListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployee: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
            //}
            //else
            //{
            //    TempData["alertdata"] = "Access Denied";
            //    return RedirectToAction("Index", "Home");
            //}
        }

        public JsonResult FunGetDetails(string emp_no)
        {
            EmployeeModels objEmployeeModel = new EmployeeModels();
            if (emp_no != "")
            {
                string sql = "select emp_id,emp_firstname,emp_middlename,emp_lastname,EmailId,Dept_Id,Designation,Emp_work_location,DeptInCharge,division from t_hr_employee where emp_no='" + emp_no + "'";
                DataSet dsData = objGlobaldata.Getdetails(sql);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    objEmployeeModel = new EmployeeModels()
                    {
                        EmpID = dsData.Tables[0].Rows[0]["emp_id"].ToString(),
                        FirstName = dsData.Tables[0].Rows[0]["emp_firstname"].ToString(),
                        MiddleName = dsData.Tables[0].Rows[0]["emp_middlename"].ToString(),
                        LastName = dsData.Tables[0].Rows[0]["emp_lastname"].ToString(),
                        emailAddress = dsData.Tables[0].Rows[0]["EmailId"].ToString(),
                        DeptID = objGlobaldata.GetDeptNameById(dsData.Tables[0].Rows[0]["Dept_Id"].ToString()),
                        Designation = dsData.Tables[0].Rows[0]["Designation"].ToString(),
                        DeptInCharge = dsData.Tables[0].Rows[0]["DeptInCharge"].ToString(),
                        // Work_Location = objGlobaldata.GetMultiWorkLocationById(dsData.Tables[0].Rows[0]["Emp_work_location"].ToString()),
                        Work_Location = objGlobaldata.GetDivisionLocationById(dsData.Tables[0].Rows[0]["Emp_work_location"].ToString()),
                        division = objGlobaldata.GetCompanyBranchNameById(dsData.Tables[0].Rows[0]["division"].ToString()),
                    };
                }
            }
            return Json(objEmployeeModel);
        }

        //[HttpPost]
        public JsonResult doesCompEmpIDExist(string CompEmpId)
        {
            var user = "";
            //string id = objGlobaldata.GetEmpIDByEmpNo(CompEmpId);
            if (CompEmpId != null)
            {
                user = objGlobaldata.checkCompEmpIdExists(CompEmpId);
            }

            return Json(user);
        }

        //[HttpPost]
        public JsonResult doesEmailIDExist(string EmailId)
        {
            var user = "";
            if (EmailId != null)
            {
                user = objGlobaldata.checkEmailAddressExists(EmailId);
            }

            return Json(user);
        }

        //
        // POST: /Employee/AddEmployee

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployee(EmployeeModels objEmployeeModel, FormCollection form, HttpPostedFileBase ProfilePic)
        {
            try
            {
                //if (ModelState.IsValid)
                {
                    //objEmployeeModel.CompEmpId= objGlobaldata.GetEmpIDByEmpNo(form["CompEmpId"]);
                    objEmployeeModel.CompEmpId = (form["CompEmpId"]);
                    //objEmployeeModel.DeptID = objGlobaldata.GetDeptIDByName(form["DeptID"]);
                    // objEmployeeModel.EmpType = form["EmpType"];
                    //objEmployeeModel.DeptInCharge = form["DeptInCharge"];
                    objEmployeeModel.Role = form["Role"];
                    //objEmployeeModel.Work_Location = objGlobaldata.GetCompanyBranchIDByName(form["Work_Location"]);

                    //if (ProfilePic != null && ProfilePic.ContentLength > 0)
                    //{
                    //    try
                    //    {
                    //        //file.FileName=DateTime.Now.ToString().Replace('/',' ');
                    //        string spath = Path.Combine(Server.MapPath("~/DataUpload/ProfilePic"), Path.GetFileName(ProfilePic.FileName));
                    //        string sFilename = Path.GetExtension(ProfilePic.FileName), sFilepath = Path.GetDirectoryName(spath);
                    //        ProfilePic.SaveAs(sFilepath + "/" + objEmployeeModel.CompEmpId + "_" + objEmployeeModel.FirstName  + sFilename);
                    //        objEmployeeModel.ProfilePic = "~/DataUpload/ProfilePic/" + objEmployeeModel.CompEmpId + "_" + objEmployeeModel.FirstName + sFilename;
                    //        ViewBag.Message = "File uploaded successfully";
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    //    }
                    //}
                    //else
                    //{
                    //    ViewBag.Message = "You have not specified a file.";
                    //}

                    if (objEmployeeModel.FunRegisterUser(objEmployeeModel))
                    {
                        //objEmployeeModel.MailTempPassword(objEmployeeModel.emailAddress);
                        TempData["Successdata"] = "User account created successfully and temporary password shared over email to user";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployee: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList");
        }

        //
        // GET: /Employee/EmployeeList
        //[HttpPost]

        [AllowAnonymous]
        public ActionResult EmployeeList(string SearchText, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Employee";

            //objGlobaldata.CreateUserSession();
            EmployeeModelList objEmployeeModelList = new EmployeeModelList();
            objEmployeeModelList.EmployeeList = new List<EmployeeModels>();

            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
            try
            {
                //if (objGlobaldata.GetCompanyBranchNameById(objGlobaldata.GetCurrentUserSession().Work_Location).ToLower() == "all")
                //{
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select EmpID, CompEmpId, emp_firstname, emp_middlename,emp_lastname, emailAddress, Dept_Id, Designation,"
                + "DeptInCharge, ProfilePic, Pwd, Emp_work_location,division,Role from t_employee t,t_hr_employee tt where Active=1 "
                + "and t.CompEmpId=tt.emp_no";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSqlstmt = sSqlstmt + " and (emp_firstname ='" + SearchText + "' or emp_firstname like '%" + SearchText + "%' or emp_lastname ='" + SearchText + "' or emp_lastname like '%" + SearchText + "%' or emailaddress='" + SearchText
                        + "' or emailaddress like '%" + SearchText + "%')";
                }
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and tt.division='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and tt.division='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by emp_firstname asc";
                DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsEmployeeList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmployeeModels objEmployee = new EmployeeModels
                            {
                                EmpID = dsEmployeeList.Tables[0].Rows[i]["EmpID"].ToString(),
                                CompEmpId = objGlobaldata.GetEmpIDByEmpNo(dsEmployeeList.Tables[0].Rows[i]["CompEmpId"].ToString()),
                                FirstName = dsEmployeeList.Tables[0].Rows[i]["emp_firstname"].ToString(),
                                MiddleName = dsEmployeeList.Tables[0].Rows[i]["emp_middlename"].ToString(),
                                LastName = dsEmployeeList.Tables[0].Rows[i]["emp_lastname"].ToString(),
                                emailAddress = dsEmployeeList.Tables[0].Rows[i]["emailAddress"].ToString(),
                                DeptID = objGlobaldata.GetDeptNameById(dsEmployeeList.Tables[0].Rows[i]["Dept_Id"].ToString()),
                                Designation = dsEmployeeList.Tables[0].Rows[i]["Designation"].ToString(),
                                DeptInCharge = dsEmployeeList.Tables[0].Rows[i]["DeptInCharge"].ToString(),
                                Role = objGlobaldata.GetMultiRoleById(dsEmployeeList.Tables[0].Rows[i]["Role"].ToString()),
                                Work_Location = objGlobaldata.GetDivisionLocationById(dsEmployeeList.Tables[0].Rows[i]["Emp_work_location"].ToString()),
                                division = objGlobaldata.GetCompanyBranchNameById(dsEmployeeList.Tables[0].Rows[i]["division"].ToString()),
                            };

                            objEmployeeModelList.EmployeeList.Add(objEmployee);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in EmployeeList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
                //}
                else
                {
                    TempData["alertdata"] = "Access Denied";
                    return RedirectToAction("Index", "Home");
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
        public JsonResult EmployeeListSearch(string SearchText, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Employee";

            //objGlobaldata.CreateUserSession();
            EmployeeModelList objEmployeeModelList = new EmployeeModelList();
            objEmployeeModelList.EmployeeList = new List<EmployeeModels>();

            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
            try
            {
                //if (objGlobaldata.GetCompanyBranchNameById(objGlobaldata.GetCurrentUserSession().Work_Location).ToLower() == "all")
                //{
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select EmpID, CompEmpId, emp_firstname, emp_middlename,emp_lastname, emailAddress, Dept_Id, Designation,"
                + "DeptInCharge, ProfilePic, Pwd, Emp_work_location,division from t_employee t,t_hr_employee tt where Active=1 "
                + "and t.CompEmpId=tt.emp_no";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSqlstmt = sSqlstmt + " and (emp_firstname ='" + SearchText + "' or emp_firstname like '%" + SearchText + "%' or emp_lastname ='" + SearchText + "' or emp_lastname like '%" + SearchText + "%' or emailaddress='" + SearchText
                        + "' or emailaddress like '%" + SearchText + "%')";
                }
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and tt.division='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and tt.division='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + " order by emp_firstname asc";
                DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsEmployeeList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmployeeModels objEmployee = new EmployeeModels
                            {
                                EmpID = dsEmployeeList.Tables[0].Rows[i]["EmpID"].ToString(),
                                CompEmpId = objGlobaldata.GetEmpIDByEmpNo(dsEmployeeList.Tables[0].Rows[i]["CompEmpId"].ToString()),
                                FirstName = dsEmployeeList.Tables[0].Rows[i]["emp_firstname"].ToString(),
                                MiddleName = dsEmployeeList.Tables[0].Rows[i]["emp_middlename"].ToString(),
                                LastName = dsEmployeeList.Tables[0].Rows[i]["emp_lastname"].ToString(),
                                emailAddress = dsEmployeeList.Tables[0].Rows[i]["emailAddress"].ToString(),
                                DeptID = objGlobaldata.GetDeptNameById(dsEmployeeList.Tables[0].Rows[i]["Dept_Id"].ToString()),
                                Designation = dsEmployeeList.Tables[0].Rows[i]["Designation"].ToString(),
                                DeptInCharge = dsEmployeeList.Tables[0].Rows[i]["DeptInCharge"].ToString(),
                                //Role = objGlobaldata.GetMultiRoleById(dsEmployeeList.Tables[0].Rows[i]["Role"].ToString()),
                                Work_Location = objGlobaldata.GetDivisionLocationById(dsEmployeeList.Tables[0].Rows[i]["Emp_work_location"].ToString()),
                                division = objGlobaldata.GetCompanyBranchNameById(dsEmployeeList.Tables[0].Rows[i]["division"].ToString()),
                            };

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
        public ActionResult EmployeeDetails()
        {
            ViewBag.SubMenutype = "Employee";

            //objGlobaldata.CreateUserSession();
            EmployeeModels objEmployee = new EmployeeModels();

            try
            {
                string sSqlstmt = "";
                if (Request.QueryString["EmpID"] != null && Request.QueryString["EmpID"] != "")
                {
                    if (Request.QueryString["View"] == "Profile")
                    {
                        string sEmpID = Request.QueryString["EmpID"];
                        ViewBag.EditFlg = true;
                        sSqlstmt = "select EmpID, CompEmpId, emp_firstname, emp_middlename,emp_lastname, emailAddress, Dept_Id, Designation,"
                        + "DeptInCharge, ProfilePic, Pwd, Emp_work_location,division from t_employee t,t_hr_employee tt where Active=1"
                        + " and t.CompEmpId=tt.emp_no and CompEmpId=" + sEmpID + "";
                    }
                    else
                    {
                        string sEmpID = Request.QueryString["EmpID"];
                        //if (objGlobaldata.GetCompanyBranchNameById(objGlobaldata.GetCurrentUserSession().Work_Location).ToLower() == "all")
                        //{
                        ViewBag.EditFlg = true;
                        sSqlstmt = "select EmpID, CompEmpId, emp_firstname, emp_middlename,emp_lastname, emailAddress, Dept_Id, Designation,"
                        + "DeptInCharge, ProfilePic, Pwd, Emp_work_location,division from t_employee t,t_hr_employee tt where Active=1"
                        + " and t.CompEmpId=tt.emp_no and EmpID=" + sEmpID + "";
                    }

                    DataSet dsEmployee = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmployee.Tables.Count > 0 && dsEmployee.Tables[0].Rows.Count > 0)
                    {
                        //string sIncharge = "Yes", sEmpType = "Internal";
                        //if (dsEmployee.Tables[0].Rows[0]["DeptInCharge"].ToString() == "N")
                        //{
                        //    sIncharge = "No";
                        //}

                        //if (dsEmployee.Tables[0].Rows[0]["EmpType"].ToString() == "E")
                        //{
                        //    sEmpType = "External";
                        //}
                        objEmployee = new EmployeeModels
                        {
                            EmpID = dsEmployee.Tables[0].Rows[0]["EmpID"].ToString(),
                            CompEmpId = objGlobaldata.GetEmpIDByEmpNo(dsEmployee.Tables[0].Rows[0]["CompEmpId"].ToString()),
                            FirstName = dsEmployee.Tables[0].Rows[0]["emp_firstname"].ToString(),
                            MiddleName = dsEmployee.Tables[0].Rows[0]["emp_middlename"].ToString(),
                            LastName = dsEmployee.Tables[0].Rows[0]["emp_lastname"].ToString(),
                            emailAddress = dsEmployee.Tables[0].Rows[0]["emailAddress"].ToString(),
                            DeptID = objGlobaldata.GetDeptNameById(dsEmployee.Tables[0].Rows[0]["Dept_Id"].ToString()),
                            Designation = dsEmployee.Tables[0].Rows[0]["Designation"].ToString(),
                            DeptInCharge = dsEmployee.Tables[0].Rows[0]["DeptInCharge"].ToString(),
                            //   EmpType = sEmpType,
                            //   ProfilePic = dsEmployee.Tables[0].Rows[0]["ProfilePic"].ToString(),
                            //Role = objGlobaldata.GetMultiRoleById(dsEmployee.Tables[0].Rows[0]["Role"].ToString()),
                            Work_Location = objGlobaldata.GetDivisionLocationById(dsEmployee.Tables[0].Rows[0]["Emp_work_location"].ToString()),
                            division = objGlobaldata.GetMultiWorkLocationById(dsEmployee.Tables[0].Rows[0]["division"].ToString())
                        };
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("EmployeeList");
                    }
                    //}
                    //else
                    //{
                    //    TempData["alertdata"] = "Access Denied";
                    //    return RedirectToAction("Index", "Home");
                    //}
                }
                else
                {
                    TempData["alertdata"] = "Employee Id cannot be Null or empty";
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objEmployee);
        }

        [AllowAnonymous]
        public ActionResult EmployeeEdit()
        {
            EmployeeModels objEmployee = new EmployeeModels();
            try
            {
                //   ViewBag.DeptList = objGlobaldata.GetDepartmentWithIdListbox();//GetDepartmentList();
                // ViewBag.Roles = objGlobaldata.GetRoles();
                // ViewBag.EmpType = new string[] { "Internal", "External" };
                // ViewBag.DeptInCharge = new string[] { "No", "Yes" };
                //  ViewBag.Work_Location = objGlobaldata.GetCompanyBranchListbox();

                ViewBag.SubMenutype = "Employee";

                //objGlobaldata.CreateUserSession();

                if (Request.QueryString["EmpID"] != null && Request.QueryString["EmpID"] != "")
                {
                    string sEmpID = Request.QueryString["EmpID"];
                    //if (objGlobaldata.GetCompanyBranchNameById(objGlobaldata.GetCurrentUserSession().Work_Location).ToLower() == "all")
                    //{
                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    string sSqlstmt = "select EmpID, CompEmpId, emp_firstname, emp_middlename,emp_lastname, emailAddress, Dept_Id, Designation,"
                   + "DeptInCharge, ProfilePic, Pwd, Emp_work_location,division from t_employee t,t_hr_employee tt where Active=1"
                   + " and t.CompEmpId=tt.emp_no and EmpID=" + sEmpID + "";
                    DataSet dsEmployee = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsEmployee.Tables.Count > 0 && dsEmployee.Tables[0].Rows.Count > 0)
                    {
                        objEmployee = new EmployeeModels
                        {
                            EmpID = dsEmployee.Tables[0].Rows[0]["EmpID"].ToString(),
                            CompEmpId = objGlobaldata.GetEmpIDByEmpNo(dsEmployee.Tables[0].Rows[0]["CompEmpId"].ToString()),
                            FirstName = dsEmployee.Tables[0].Rows[0]["emp_firstname"].ToString(),
                            MiddleName = dsEmployee.Tables[0].Rows[0]["emp_middlename"].ToString(),
                            LastName = dsEmployee.Tables[0].Rows[0]["emp_lastname"].ToString(),
                            emailAddress = dsEmployee.Tables[0].Rows[0]["emailAddress"].ToString(),
                            DeptID = objGlobaldata.GetDeptNameById(dsEmployee.Tables[0].Rows[0]["Dept_Id"].ToString()),
                            Designation = dsEmployee.Tables[0].Rows[0]["Designation"].ToString(),
                            DeptInCharge = dsEmployee.Tables[0].Rows[0]["DeptInCharge"].ToString(),
                            //   EmpType = sEmpType,
                            //   ProfilePic = dsEmployee.Tables[0].Rows[0]["ProfilePic"].ToString(),
                            //Role = objGlobaldata.GetMultiRoleById(dsEmployee.Tables[0].Rows[0]["Role"].ToString()),
                            Work_Location = objGlobaldata.GetDivisionLocationById(dsEmployee.Tables[0].Rows[0]["Emp_work_location"].ToString()),
                            division = objGlobaldata.GetMultiWorkLocationById(dsEmployee.Tables[0].Rows[0]["division"].ToString())
                        };
                    }
                }
                else
                {
                    TempData["alertdata"] = "Employee Id cannot be Null or empty";
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEmployee);
        }

        //
        // POST: /Employee/EmployeeEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeEdit(EmployeeModels objEmployeeModel, FormCollection form, HttpPostedFileBase ProfilePic)
        {
            try
            {
                //objEmployeeModel.DeptID = form["DeptID"];
                //objEmployeeModel.EmpType = form["EmpType"];
                //objEmployeeModel.DeptInCharge = form["DeptInCharge"];
                objEmployeeModel.Role = form["Role"];
                //objEmployeeModel.EmpID = form["EmpId"];
                //objEmployeeModel.Work_Location = form["Work_Location"];

                //if (ProfilePic != null && ProfilePic.ContentLength > 0)
                //{
                //    try
                //    {
                //        //file.FileName=DateTime.Now.ToString().Replace('/',' ');
                //        string spath = Path.Combine(Server.MapPath("~/DataUpload/ProfilePic"), Path.GetFileName(ProfilePic.FileName));
                //        string sFilename = Path.GetExtension(ProfilePic.FileName), sFilepath = Path.GetDirectoryName(spath);
                //        ProfilePic.SaveAs(sFilepath + "/" + objEmployeeModel.CompEmpId + "_" + objEmployeeModel.FirstName + sFilename);
                //        objEmployeeModel.ProfilePic = "~/DataUpload/ProfilePic/" + objEmployeeModel.CompEmpId + "_" + objEmployeeModel.FirstName + sFilename;
                //        ViewBag.Message = "File uploaded successfully";
                //    }
                //    catch (Exception ex)
                //    {
                //        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                //    }
                //}
                //else
                //{
                //    ViewBag.Message = "You have not specified a file.";
                //}

                if (objEmployeeModel.FunUpdateUser(objEmployeeModel))
                {
                    TempData["Successdata"] = "Updated Employee details successfully";
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

        //
        // GET: /Employee/EmployeeDelete

        [HttpPost]
        public ActionResult EmployeeDelete(FormCollection form)
        {
            string sEmpID = form["EmpID"];
            if (sEmpID != "")
            {
                try
                {
                    EmployeeModels objEmployeeModel = new EmployeeModels();

                    if (objEmployeeModel.FunDeleteEmployee(sEmpID))
                    {
                        TempData["Successdata"] = "User Account deleted successfully";
                        return Json("Deleted successfully");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        return Json("Deleted Failed");
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in EmployeeDelete: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            return Json("Delete Failed, Emp Id null");
        }

        //
        // GET: /Employee/EmployeeActivate
        //[AllowAnonymous]

        [HttpPost]
        public ActionResult EmployeeActivate(FormCollection form)
        {
            string sEmpID = form["EmpID"];
            if (sEmpID != "")
            {
                try
                {
                    EmployeeModels objEmployeeModel = new EmployeeModels();

                    if (objEmployeeModel.FunActivateEmployee(sEmpID))
                    {
                        TempData["Successdata"] = "Employee details deleted successfully";
                        return Json("Activated successfully");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in EmployeeActivate: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            return Json("Activation Failed, Emp Id null");
        }

        //
        // GET: /Employee/ResetPassword

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            if (Request.QueryString["EmpID"] != null && Request.QueryString["EmpID"] != "")
            {
                string sEmpID = Request.QueryString["EmpID"];
                ViewBag.EmpID = sEmpID;
            }
            else
            {
                TempData["alertdata"] = "Employee Id cannot be Null or empty";
                return RedirectToAction("EmployeeList");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(EmployeeModels objEmployeeModel, FormCollection form)
        {
            ViewBag.SubMenutype = "Employee";

            if (Request.QueryString["EmpID"] != null && Request.QueryString["EmpID"] != "")
            {
                string sEmpID = form["EmpID"];

                if (objEmployeeModel.Pwd != null)
                {
                    if (objEmployeeModel.FunPwdReset(sEmpID, clsGlobal.Encrypt(objEmployeeModel.Pwd)))
                    {
                        TempData["Successdata"] = "Password reset successful";
                        //return Json("Password reset successful");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    TempData["alertdata"] = "Password can not Null or empty";
                }
            }
            else
            {
                TempData["alertdata"] = "Employee Id cannot be Null or empty";
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult SendResetPassword(FormCollection form)
        {
            string sEmailid = form["Emailid"];
            string sUsername = form["Username"];
            if (sEmailid != "")
            {
                EmployeeModels objEmp = new EmployeeModels();

                if (objEmp.MailTempPassword(sEmailid, sUsername))
                {
                    TempData["Successdata"] = "System generated password sent to user email id, please contact Admin in case email not recevied";
                    return Json("System generated password sent to user email id, please contact Admin in case email not recevied");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    return Json("Password Reset Failed");
                }
            }

            return Json("Password Reset Failed, Email id is null");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.SubMenutype = "Employee";
            //string sTempPwd= objGlobaldata.GenerateTempPassword();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(EmployeeModels model, FormCollection form)
        {
            ViewBag.SubMenutype = "Employee";

            string sEmailId = form["emailAddress"];
            if (objGlobaldata.checkEmailAddressExists(sEmailId) == "false")
            {
                if (model.MailTempPassword(sEmailId, sEmailId.Split('@')[0]))
                {
                    TempData["Successdata"] = "System generated password sent to user email id, please contact Admin in case email not recevied";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];// objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

                //return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["alertdata"] = "The email id does not exists.";
            }

            return View();
        }

        //[HttpPost]
        //public JsonResult FunCheckPaswd(string Password)
        //{
        //    bool result = false;
        //    try
        //    {
        //        string sSqlstmt = "select count(*)  as empcount from t_hr_employee where emp_status=1";
        //        DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
        //        if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
        //        {
        //            count = dsData.Tables[0].Rows[0]["empcount"].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in FunCheckPaswd: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json(result);
        //}
    }
}