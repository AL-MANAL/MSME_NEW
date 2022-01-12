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
    public class JDController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        [AllowAnonymous]
        public ActionResult JDList(int? page)
        {
            JDModelsList obj = new JDModelsList();
            obj.JDList = new List<JDModels>();
            JDModels objstd = new JDModels();
            try
            {
                string sSqlstmt = "select id_jd,item_id as designation_id,item_desc as designation,jd_report,approve_status from dropdownitems t1 join dropdownheader t2 on t1.header_id = t2.header_id"
                + " and header_desc = 'Employee Designation' left outer join t_jd t3 on t1.item_id = t3.designation_id order by item_desc asc";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objstd = new JDModels
                            {
                                designation_id = (dsList.Tables[0].Rows[i]["designation_id"].ToString()),
                                id_jd = (dsList.Tables[0].Rows[i]["id_jd"].ToString()),
                                designation = (dsList.Tables[0].Rows[i]["designation"].ToString()),
                                jd_report = (dsList.Tables[0].Rows[i]["jd_report"].ToString()),
                                approve_status = (dsList.Tables[0].Rows[i]["approve_status"].ToString()),
                            };

                            obj.JDList.Add(objstd);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in JDList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in JDList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(obj.JDList.ToList());
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult JD()
        {
            JDModels objRoleModel = new JDModels();
            try
            {
                if (Request.QueryString["designation_id"] != null && Request.QueryString["designation_id"] != "")
                {
                    string designation_id = Request.QueryString["designation_id"];
                    ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                    ViewBag.DesignationList = objGlobaldata.GetDesignationforJD(designation_id);
                    ViewBag.DesignationName = objGlobaldata.GetDropdownitemById(designation_id);
                    ViewBag.ApprovedBy = objGlobaldata.GetDeptHeadList();
                    ViewBag.DesignationId = designation_id;

                    string sSqlstmt = "select id_jd,designation_id,deptid,report_to,supervises,responsibility,authorities,interfaces_internal,interfaces_external,"
                        + "accountable,academic_mandatory,academic_optional,trade_mandatory,trade_optional,experience_mandatory,experience_optional,trainings_mandatory,"
                        + "trainings_optional,skills_mandatory,skills_optional,jd_date,revised_date,approved_by,rev_no,reviewed_by from t_jd where designation_id='" + designation_id + "'";

                    DataSet dsRole = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRole.Tables.Count > 0 && dsRole.Tables[0].Rows.Count > 0)
                    {
                        objRoleModel = new JDModels
                        {
                            id_jd = dsRole.Tables[0].Rows[0]["id_jd"].ToString(),
                            designation_id = dsRole.Tables[0].Rows[0]["designation_id"].ToString(),
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
                            rev_no = dsRole.Tables[0].Rows[0]["rev_no"].ToString(),
                            reviewed_by = dsRole.Tables[0].Rows[0]["reviewed_by"].ToString(),
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
                    return RedirectToAction("JDList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in JD: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRoleModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult JD(JDModels objModel, FormCollection form)
        {
            try
            {
                objModel.reviewed_by = form["reviewed_by"];
                objModel.approved_by = form["approved_by"];
                if (objModel.id_jd != null && objModel.id_jd != "")
                {
                    if (objModel.FunUpdateJD(objModel))
                    {
                        return RedirectToAction("JDPdf", new { designation_id = objModel.designation_id, flag = 0 });
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
                        return RedirectToAction("JDPdf", new { designation_id = objModel.designation_id, flag = 1 });
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in JD: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("JDList");
        }

        public ActionResult JDPdf(string designation_id, string flag)
        {
            JDModels objRoleModel = new JDModels();
            CompanyModels objCompany = new CompanyModels();
            try
            {
                if (designation_id != null && designation_id != "")
                {
                    string sSqlstmt = "select id_jd,designation_id,deptid,report_to,supervises,responsibility,authorities,interfaces_internal,interfaces_external,"
                        + "accountable,academic_mandatory,academic_optional,trade_mandatory,trade_optional,experience_mandatory,experience_optional,trainings_mandatory,"
                        + "trainings_optional,skills_mandatory,skills_optional,jd_date,revised_date,approved_by from t_jd where designation_id='" + designation_id + "'";

                    DataSet dsRole = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRole.Tables.Count > 0 && dsRole.Tables[0].Rows.Count > 0)
                    {
                        objRoleModel = new JDModels
                        {
                            id_jd = Convert.ToString(dsRole.Tables[0].Rows[0]["id_jd"].ToString()),
                            designation_id = objGlobaldata.GetDropdownitemById(dsRole.Tables[0].Rows[0]["designation_id"].ToString()),
                            deptid = objGlobaldata.GetDeptNameById(dsRole.Tables[0].Rows[0]["deptid"].ToString()),
                            report_to = objGlobaldata.GetDropdownitemById(dsRole.Tables[0].Rows[0]["report_to"].ToString()),
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
                            approved_by = objGlobaldata.GetMultiHrEmpNameById(dsRole.Tables[0].Rows[0]["approved_by"].ToString()),
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

                        ViewAsPdf pdf = new ViewAsPdf("JDPdf")
                        {
                            FileName = "JD" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "JD.pdf",
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
                    return RedirectToAction("JDList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in JD: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("JDList");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult JDDetail()
        {
            JDModels objRoleModel = new JDModels();
            try
            {
                if (Request.QueryString["designation_id"] != null && Request.QueryString["designation_id"] != "")
                {
                    string id_jd = Request.QueryString["id_jd"];
                    string designation_id = Request.QueryString["designation_id"];
                    ViewBag.designation_id = designation_id;
                    ViewBag.Designation = objGlobaldata.GetDropdownitemById(designation_id);
                    string sSqlstmt = "select id_jd,designation_id,deptid,report_to,supervises,responsibility,authorities,interfaces_internal,interfaces_external,"
                        + "accountable,academic_mandatory,academic_optional,trade_mandatory,trade_optional,experience_mandatory,experience_optional,trainings_mandatory,"
                        + "trainings_optional,skills_mandatory,skills_optional,jd_date,revised_date,approved_by,reviewed_by,rev_no from t_jd where designation_id='" + designation_id + "'";

                    DataSet dsRole = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsRole.Tables.Count > 0 && dsRole.Tables[0].Rows.Count > 0)
                    {
                        objRoleModel = new JDModels
                        {
                            id_jd = dsRole.Tables[0].Rows[0]["id_jd"].ToString(),
                            designation_id = dsRole.Tables[0].Rows[0]["designation_id"].ToString(),
                            deptid = objGlobaldata.GetDeptNameById(dsRole.Tables[0].Rows[0]["deptid"].ToString()),
                            report_to = objGlobaldata.GetDropdownitemById(dsRole.Tables[0].Rows[0]["report_to"].ToString()),
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
                            reviewed_by = dsRole.Tables[0].Rows[0]["reviewed_by"].ToString(),
                            rev_no = dsRole.Tables[0].Rows[0]["rev_no"].ToString(),
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

                        string user = objGlobaldata.GetCurrentUserSession().empid;

                        string sSqlstmt1 = "select id_jd,designation_id,deptid,report_to,supervises,jd_date,logged_by,jd_report,approved_by"
                         + " from t_jd where approve_status = 0"
                         + " and (find_in_set('" + user + "', reviewed_by) and not find_in_set('" + user + "', Reviewers))"
                         + " and (find_in_set('" + user + "', reviewed_by) and not find_in_set('" + user + "', ReviewRejector))";
                        DataSet dsApprovalList = objGlobaldata.Getdetails(sSqlstmt1);
                        if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.ReviewStatus = true;
                        }

                        string sSqlstmt2 = "select id_jd,designation_id,deptid,report_to,supervises,jd_date,logged_by,jd_report,approved_by"
                        + " from t_jd where approve_status = 2"
                        + " and (find_in_set('" + user + "', approved_by) and not find_in_set('" + user + "', Approvers))"
                        + " and (find_in_set('" + user + "', approved_by) and not find_in_set('" + user + "', ApprovalRejector))";

                        dsApprovalList = objGlobaldata.Getdetails(sSqlstmt2);
                        if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.ApproveStatus = true;
                        }

                        string sql = "select id_review,id_jd,emp_id,(case when apprv_status=0 then 'Pending for Review' when apprv_status=1 then 'Rejected' when apprv_status=2 then 'Reviewed' end) as apprv_status,apprv_date,comments from t_jd_review where id_jd='" + id_jd + "'";
                        ViewBag.dsReview = objGlobaldata.Getdetails(sql);

                        sql = "select id_approve,id_jd,emp_id,(case when apprv_status=2 then 'Pending for Approval' when apprv_status=3 then 'Rejected' when apprv_status=4 then 'Approved'  end) as apprv_status,apprv_date,comments from t_jd_approve where id_jd='" + id_jd + "'";
                        ViewBag.dsApprove = objGlobaldata.Getdetails(sql);

                        return View(objRoleModel);
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("JDList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in JDDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objRoleModel);
        }

        [AllowAnonymous]
        public ActionResult JDReviewOrReject(FormCollection form, JDModels objModel)
        {
            try
            {
                if (objModel.FunJDReviewOrReject(objModel))
                {
                    TempData["Successdata"] = "Job description status updated";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in JDApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult JDApproveReject(FormCollection form, JDModels objModel)
        {
            try
            {
                if (objModel.FunJDApproveOrReject(objModel))
                {
                    TempData["Successdata"] = "Job description status updated";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in JDApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("Index", "Home");
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