using ISOStd.Filters;
using ISOStd.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Filters;
using Rotativa;
using System.IO;
using Microsoft.Web.Helpers;

namespace Role.Controllers
{
    [PreventFromUrl]
    public class RoleController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public RoleController()
        {
            ViewBag.Menutype = "Settings";
        }

        [AllowAnonymous]
        public ActionResult AddRole(RoleModels objModel, FormCollection form)
        {
            ViewBag.SubMenutype = "Roles";
            ViewBag.Role = objGlobaldata.GetRoles();
            try
            {
                if (objModel.FunAddRole(objModel))
                {
                    TempData["Successdata"] = "Role added Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRole: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("RoleList");
        }

        [AllowAnonymous]
        public ActionResult RoleList(int? page)
        {
            RoleModelsList obj = new RoleModelsList();
            obj.RoleList = new List<RoleModels>();
            RoleModels objstd = new RoleModels();
            try
            {
                string sSqlstmt = "select Id,RoleName,jd_report from roles t left outer join t_role_jd tt on"
                +" t.id = tt.role_id where Active = 1 order by RoleName asc";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                   
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objstd = new RoleModels
                            {
                                Id = (dsList.Tables[0].Rows[i]["Id"].ToString()),
                                RoleName = (dsList.Tables[0].Rows[i]["RoleName"].ToString()),
                                jd_report = (dsList.Tables[0].Rows[i]["jd_report"].ToString()),
                            };

                            obj.RoleList.Add(objstd);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RoleList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoleList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(obj.RoleList.ToList());

        }

        public ActionResult RoleEdit()
        {
            RoleModels obj = new RoleModels();
            try
            {

                if (Request.QueryString["RoleId"] != null && Request.QueryString["RoleId"] != "")
                {
                    string sId = Request.QueryString["RoleId"];
                    ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                    string sSqlstmt = "select Id,RoleName,appl_branch from roles where Id='" + sId + "'";
                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        obj = new RoleModels
                        {
                            Id = (dsList.Tables[0].Rows[0]["Id"].ToString()),
                            RoleName = (dsList.Tables[0].Rows[0]["RoleName"].ToString()),
                            appl_branch = (dsList.Tables[0].Rows[0]["appl_branch"].ToString()),
                        };
                    }
                    else
                    {

                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("RoleList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("RoleList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoleEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("RoleList");
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleEdit(RoleModels obj, FormCollection form)
        {
            try
            {

                if (obj.FunUpdateRole(obj))
                {
                    TempData["Successdata"] = "Role details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoleEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RoleList");
        }

        [AllowAnonymous]
        public JsonResult RoleDelete(FormCollection form)
        {
            try
            {

                if (form["Id"] != null && form["Id"] != "")
                    {

                        RoleModels objmdl = new RoleModels();
                        string sId = form["Id"];

                        if (objmdl.FunDeleteRole(sId))
                        {
                            TempData["Successdata"] = "Role deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in RoleDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult RoleGroupList(int? page)
        {
            RoleModelsList obj = new RoleModelsList();
            obj.RoleList = new List<RoleModels>();
            RoleModels objstd = new RoleModels();
            try
            {
                //string sSqlstmt = "select role_id,branch_id,concat(RoleName,' - ',BranchCode) as Role,RoleName,appl_branch from t_access t,t_company_branch b,roles r"
                //+ " where t.role_id=r.Id and t.branch_id=b.id order by role_id , branch_id asc";
                string sSqlstmt = " select role_id,branch_id,RoleName,appl_branch from t_access t,roles r  where t.role_id = r.Id and r.active=1 order by role_id asc";

                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objstd = new RoleModels();
                            { 
                                objstd.role_id = (dsList.Tables[0].Rows[i]["role_id"].ToString());
                                objstd.branch_id = (dsList.Tables[0].Rows[i]["branch_id"].ToString());
                                objstd.RoleName = (dsList.Tables[0].Rows[i]["RoleName"].ToString());

                                //objstd.Role = (dsList.Tables[0].Rows[i]["Role"].ToString());
                                objstd.appl_branch = (dsList.Tables[0].Rows[i]["appl_branch"].ToString());
                                obj.RoleList.Add(objstd);
                            }                   
                                                      
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RoleGroupList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoleGroupList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(obj.RoleList.ToList());

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult RoleJD()
        {
            RoleModels objRoleModel = new RoleModels();
            try
            {
               
                if (Request.QueryString["RoleId"] != null && Request.QueryString["RoleId"] != "")
                {
                    string RoleId = Request.QueryString["RoleId"];
                    ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                    ViewBag.RolesList = objGlobaldata.GetRolesForJD(RoleId);
                    ViewBag.RoleName = objGlobaldata.GetRolesNameById(RoleId);
                    ViewBag.ApprovedBy = objGlobaldata.GetDeptHeadList();
                    ViewBag.RoleId = RoleId;

                    string sSqlstmt = "select id_role_jd,role_id,deptid,report_to,supervises,responsibility,authorities,interfaces_internal,interfaces_external,"
                        +"accountable,academic_mandatory,academic_optional,trade_mandatory,trade_optional,experience_mandatory,experience_optional,trainings_mandatory,"
                        + "trainings_optional,skills_mandatory,skills_optional,jd_date,revised_date,approved_by from t_role_jd where role_id='" + RoleId + "'";

                    DataSet dsRole = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRole.Tables.Count > 0 && dsRole.Tables[0].Rows.Count > 0)
                    {
                        objRoleModel = new RoleModels
                        {
                            id_role_jd = dsRole.Tables[0].Rows[0]["id_role_jd"].ToString(),
                            role_id = dsRole.Tables[0].Rows[0]["role_id"].ToString(),
                            deptid = dsRole.Tables[0].Rows[0]["deptid"].ToString(),
                            report_to = dsRole.Tables[0].Rows[0]["report_to"].ToString(),
                            supervises = dsRole.Tables[0].Rows[0]["supervises"].ToString(),
                            responsibility = dsRole.Tables[0].Rows[0]["responsibility"].ToString(),
                            authorities = dsRole.Tables[0].Rows[0]["authorities"].ToString(),
                            interfaces_internal = dsRole.Tables[0].Rows[0]["interfaces_internal"].ToString(),
                            interfaces_external = dsRole.Tables[0].Rows[0]["interfaces_external"].ToString(),
                            accountable = dsRole.Tables[0].Rows[0]["accountable"].ToString(),
                            academic_mandatory = dsRole.Tables[0].Rows[0]["academic_mandatory"].ToString(),
                            academic_optional = dsRole.Tables[0].Rows[0]["academic_optional"].ToString(),
                            trade_mandatory = dsRole.Tables[0].Rows[0]["trade_mandatory"].ToString(),
                            trade_optional = dsRole.Tables[0].Rows[0]["trade_optional"].ToString(),
                            experience_mandatory = dsRole.Tables[0].Rows[0]["experience_mandatory"].ToString(),
                            experience_optional = dsRole.Tables[0].Rows[0]["experience_optional"].ToString(),
                            trainings_mandatory = dsRole.Tables[0].Rows[0]["trainings_mandatory"].ToString(),
                            trainings_optional = dsRole.Tables[0].Rows[0]["trainings_optional"].ToString(),
                            skills_mandatory = dsRole.Tables[0].Rows[0]["skills_mandatory"].ToString(),
                            skills_optional = dsRole.Tables[0].Rows[0]["skills_optional"].ToString(),
                            approved_by = dsRole.Tables[0].Rows[0]["approved_by"].ToString(),
                      
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsRole.Tables[0].Rows[0]["jd_date"].ToString(), out dtValue))
                        {
                            objRoleModel.jd_date = dtValue;
                        }
                        if (DateTime.TryParse(dsRole.Tables[0].Rows[0]["revised_date"].ToString(), out dtValue))
                        {
                            objRoleModel.revised_date = dtValue;
                        }

                        return View(objRoleModel);

                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("RoleList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoleJD: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRoleModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult RoleJD(RoleModels objModel, FormCollection form)
        {

            try
            {
               if(objModel.id_role_jd != null && objModel.id_role_jd != "")
                {
                    if (objModel.FunUpdateJD(objModel))
                    {
                        return RedirectToAction("RoleJDPdf", new { role_id = objModel.role_id,flag=0 });
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
              
                else
                {
                    if (objModel.FunAddJD(objModel))
                    {
                        return RedirectToAction("RoleJDPdf", new { role_id = objModel.RoleId, flag = 1 });
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }

            
               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoleJD: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RoleList");
        }

        public ActionResult RoleJDPdf(string role_id,string flag)
        {
            RoleModels objRoleModel = new RoleModels();
            CompanyModels objCompany = new CompanyModels();
            try
            {
                if (role_id != null && role_id != "")
                {
                    
                    string sSqlstmt = "select id_role_jd,role_id,deptid,report_to,supervises,responsibility,authorities,interfaces_internal,interfaces_external,"
                        + "accountable,academic_mandatory,academic_optional,trade_mandatory,trade_optional,experience_mandatory,experience_optional,trainings_mandatory,"
                        + "trainings_optional,skills_mandatory,skills_optional,jd_date,revised_date,approved_by from t_role_jd where role_id='" + role_id + "'";

                    DataSet dsRole = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRole.Tables.Count > 0 && dsRole.Tables[0].Rows.Count > 0)
                    {
                        objRoleModel = new RoleModels
                        {
                            id_role_jd =Convert.ToString(dsRole.Tables[0].Rows[0]["id_role_jd"].ToString()),
                            role_id =objGlobaldata.GetRolesNameById(dsRole.Tables[0].Rows[0]["role_id"].ToString()),
                            deptid =objGlobaldata.GetDeptNameById(dsRole.Tables[0].Rows[0]["deptid"].ToString()),
                            report_to =objGlobaldata.GetEmpHrNameById(dsRole.Tables[0].Rows[0]["report_to"].ToString()),
                            supervises = Convert.ToString(dsRole.Tables[0].Rows[0]["supervises"].ToString()),
                            responsibility = Convert.ToString(dsRole.Tables[0].Rows[0]["responsibility"]),
                            authorities = Convert.ToString(dsRole.Tables[0].Rows[0]["authorities"].ToString()),
                            interfaces_internal = Convert.ToString(dsRole.Tables[0].Rows[0]["interfaces_internal"].ToString()),
                            interfaces_external = Convert.ToString(dsRole.Tables[0].Rows[0]["interfaces_external"].ToString()),
                            accountable = Convert.ToString(dsRole.Tables[0].Rows[0]["accountable"].ToString()),
                            academic_mandatory = Convert.ToString(dsRole.Tables[0].Rows[0]["academic_mandatory"].ToString()),
                            academic_optional = Convert.ToString(dsRole.Tables[0].Rows[0]["academic_optional"].ToString()),
                            trade_mandatory = Convert.ToString(dsRole.Tables[0].Rows[0]["trade_mandatory"].ToString()),
                            trade_optional = Convert.ToString(dsRole.Tables[0].Rows[0]["trade_optional"].ToString()),
                            experience_mandatory = Convert.ToString(dsRole.Tables[0].Rows[0]["experience_mandatory"].ToString()),
                            experience_optional = Convert.ToString(dsRole.Tables[0].Rows[0]["experience_optional"].ToString()),
                            trainings_mandatory = Convert.ToString(dsRole.Tables[0].Rows[0]["trainings_mandatory"].ToString()),
                            trainings_optional = Convert.ToString(dsRole.Tables[0].Rows[0]["trainings_optional"].ToString()),
                            skills_mandatory = Convert.ToString(dsRole.Tables[0].Rows[0]["skills_mandatory"].ToString()),
                            skills_optional = Convert.ToString(dsRole.Tables[0].Rows[0]["skills_optional"].ToString()),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsRole.Tables[0].Rows[0]["approved_by"].ToString()),
                            approvedby = (dsRole.Tables[0].Rows[0]["approved_by"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsRole.Tables[0].Rows[0]["jd_date"].ToString(), out dtValue))
                        {
                            objRoleModel.jd_date = dtValue;
                        }
                        if (DateTime.TryParse(dsRole.Tables[0].Rows[0]["revised_date"].ToString(), out dtValue))
                        {
                            objRoleModel.revised_date = dtValue;
                        }

                        ViewBag.RoleModel = objRoleModel;
                      
                        dsRole = objCompany.GetCompanyDetailsForReport(dsRole);
                        ViewBag.dsRole = dsRole;

                        Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
                      
                        foreach (var key in Request.Cookies.AllKeys)
                        {
                            cookieCollection.Add(key, Request.Cookies.Get(key).Value);
                        }
                        string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

                        ViewAsPdf pdf = new ViewAsPdf("RoleJDPdf")
                        {
                            FileName = "JD" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm")+"JD.pdf",
                            Cookies = cookieCollection,
                            CustomSwitches = footer
                        };
                        byte[] pdfData = pdf.BuildPdf(ControllerContext);
                        string fullPath = Server.MapPath("~/DataUpload/MgmtDocs/JD/") + pdf.FileName;
                        using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                        {
                            fileStream.Write(pdfData, 0, pdfData.Length);
                        }
                        objRoleModel.jd_report = "~/DataUpload/MgmtDocs/JD/" + pdf.FileName;
                        // byte[] bytes = jdpdf.BuildPdf(ControllerContext);
                         HttpPostedFileBase jd_report = (HttpPostedFileBase)new MemoryPostedFile(pdfData, pdf.FileName);
                        pdfData = null;

                        if (objRoleModel.FunUpdateJDReport(objRoleModel, jd_report, flag))
                        {
                            TempData["Successdata"] = "Updated successfully";
                        }
                        else
                        {
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("RoleList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoleJD: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RoleList");
        }
        [AllowAnonymous]
        public ActionResult JDApproveReject(string id_role_jd, string iStatus, string PendingFlg, string Document, string JD_comment)
        {
            try
            {
                RoleModels objModel = new RoleModels();

                string sStatus = "";
                if (iStatus == "0")
                {
                    sStatus = "Pending";
                }
                else if (iStatus == "1")
                {
                    sStatus = "Approved";
                }
                else if (iStatus == "2")
                {
                    sStatus = "Rejected";
                }
                if (objModel.FunJDApproveOrReject(id_role_jd, iStatus, JD_comment))
                {
                    TempData["Successdata"] = "Job Description " + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("RoleList");
            }
        }

        public JsonResult JDApproveRejectNoty(string id_role_jd, string iStatus, string PendingFlg, string Document, string JD_comment)
        {
            try
            {
                RoleModels objModel = new RoleModels();
                string filename = Path.GetFileName(Document);
                FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);

                if (objModel.FunJDApproveOrReject(id_role_jd, iStatus, JD_comment))
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyViolationApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
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

    }
    public class MemoryPostedFile : HttpPostedFileBase
    {
        private readonly byte[] fileBytes;

        public MemoryPostedFile(byte[] fileBytes, string fileName = null)
        {
            this.fileBytes = fileBytes;
            this.FileName = fileName;
            this.InputStream = new MemoryStream(fileBytes);
            this.ContentType = "application/pdf";

        }

        public override int ContentLength => fileBytes.Length;

        public override string FileName { get; }

        public override Stream InputStream { get; }

        public override string ContentType { get; }

        public override void SaveAs(string filename)
        {

        }
    }
}