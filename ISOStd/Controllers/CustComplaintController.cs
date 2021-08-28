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
using Rotativa;
using ISOStd.Filters;

using System.ComponentModel.DataAnnotations;


namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class CustComplaintController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();


        public CustComplaintController()
        {
            ViewBag.Menutype = "CustomerMgmt";
            ViewBag.SubMenutype = "CustComplaint";
        }

        public ActionResult AddCustomerComplaint()
        {
            return InitilizeACustomerCompliant();
        }

        public ActionResult FunGetDeptEmpList(string DeptId)
        {
            MultiSelectList lstEmp = objGlobaldata.GetHrEmpListByDept(DeptId);
            return Json(lstEmp);
        }

        private ActionResult InitilizeACustomerCompliant()
        {
            CustComplaintModels objcomp = new CustComplaintModels();
            try
            {
                
                ViewBag.Complaint_Status =objGlobaldata.GetDropdownList("Complaint Status");
                //objcomp.ComplaintStatus = objGlobaldata.GetComplaintStausById("Open");

                ViewBag.Mode = objGlobaldata.GetModeOfComplaint();
                ViewBag.DeptList = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Customer = objGlobaldata.GetCustomerListbox();

                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.Name = objUser.firstname;
                ViewBag.DeptName = objGlobaldata.GetDeptNameById(objUser.DeptID);
                ViewBag.Designation = objUser.Designation;                

                //try
                //{

                //    string sSqlstmt = "Select ComplaintNo from t_custcomplaint";
                //    DataSet dsComplaint = objGlobaldata.Getdetails(sSqlstmt);
                //    if (dsComplaint.Tables.Count > 0 && dsComplaint.Tables[0].Rows.Count > 0)
                //    {
                //        try
                //        {
                //            string sSSqlstmt = "select SUBSTRING(ComplaintNo, 1, length(ComplaintNo)-4) as ComplaintNo from t_custcomplaint order by id_complaint Desc limit 1";
                //            DataSet dsCustComplaint = objGlobaldata.Getdetails(sSSqlstmt);
                //            if (dsCustComplaint.Tables.Count > 0 && dsCustComplaint.Tables[0].Rows.Count > 0)
                //            {
                //                string SlNo = dsCustComplaint.Tables[0].Rows[0]["ComplaintNo"].ToString();
                //                int CompNo = Convert.ToInt32(SlNo);
                //                CompNo = CompNo + 1;
                //                ViewBag.ComplaintNo = CompNo + Convert.ToString(DateTime.Today.Year);
                //                Session["ComplaintNo"] = ViewBag.ComplaintNo;
                //            }

                //        }
                //        catch (Exception ex)
                //        {
                //            objGlobaldata.AddFunctionalLog("Exception in ChangeMgmt: " + ex.ToString());
                //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                //        }

                //    }
                //    else
                //    {
                //        ViewBag.ComplaintNo = "1" + DateTime.Today.Year;
                //        Session["ComplaintNo"] = ViewBag.ComplaintNo;
                //    }

                //}
                //catch (Exception ex)
                //{
                //    objGlobaldata.AddFunctionalLog("Exception in ChangeMgmt: " + ex.ToString());
                //    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                //}
            }
            catch (Exception ex)
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                objGlobaldata.AddFunctionalLog("Exception in AddCustomerComplaint: " + ex.ToString());

            }
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddCustomerComplaint(CustComplaintModels objCustomerCompliant, FormCollection form, IEnumerable<HttpPostedFileBase> file)
        {
            try
            {

                objCustomerCompliant.CustomerName = form["CustomerName"];
                objCustomerCompliant.ProjectName = form["ProjectName"];
                objCustomerCompliant.ReportedBy = form["ReportedBy"];
                objCustomerCompliant.ModeOfComplaint = form["ModeOfComplaint"];
                //objCustomerCompliant.ComplaintStatus = form["ComplaintStatus"];
                objCustomerCompliant.Details = form["Details"];
                objCustomerCompliant.divisionId = form["DeptId"];
                objCustomerCompliant.ForwardTo = form["ForwardTo"];
                objCustomerCompliant.ComplaintStatus = objGlobaldata.GetComplaintStausIdByName("Open");

                DateTime dateValue;

                if (DateTime.TryParse(form["ReceivedDate"], out dateValue) == true)
                {
                    objCustomerCompliant.ReceivedDate = dateValue;
                }

                if (DateTime.TryParse(form["registered_on"], out dateValue) == true)
                {
                    objCustomerCompliant.registered_on = dateValue;
                }

                HttpPostedFileBase files = Request.Files[0];

                if (file != null && files.ContentLength > 0)
                {
                    foreach (var doc in file)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Customer"), Path.GetFileName(doc.FileName));
                            string sFilename = "Complaint" + "_" + objCustomerCompliant.ComplaintNo + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            doc.SaveAs(sFilepath + "/" + sFilename);
                            objCustomerCompliant.Document = objCustomerCompliant.Document + "," + "~/DataUpload/MgmtDocs/Customer/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objCustomerCompliant.Document = objCustomerCompliant.Document.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objCustomerCompliant.FunAddCustomerCompliant(objCustomerCompliant))
                {
                    TempData["Successdata"] = "Customer complaint registered successfully with Complaint Number : '" + objCustomerCompliant.ComplaintNo + "'";
                    objCustomerCompliant.SendCustComplaintRegistmail(objCustomerCompliant.ComplaintNo, "Customer Complaint Registered");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustomerComplaint: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerComplaintList");
        }


        [AllowAnonymous]
        public ActionResult CustomerComplaintList(string SearchText, string ChangeIn, string Approvestatus, int? page, string year, string branch_name)
        {
            CustComplaintModels objcomp = new CustComplaintModels();
            CustComplaintModelsList objComplaintModelsList = new CustComplaintModelsList();
            objComplaintModelsList.CustComplaintList = new List<CustComplaintModels>();
            ViewBag.year = objGlobaldata.GetDropdownList("Years");

            try
            {

                //UserCredentials objUser = new UserCredentials();
                //objUser = objGlobaldata.GetCurrentUserSession();
                //ViewBag.User = objUser.firstname;
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select id_complaint,ComplaintNo,LoggedDate,LoggedBy,CustomerName,ProjectName,ReceivedDate,ReportedBy,ModeOfComplaint,"
                    + "Details,ForwardTo,ComplaintStatus,Document,ForwarderAssign,CustomerRef,b.branch,reportedby_email,reportedby_desig,reportedby_no,registered_on from t_custcomplaint a,t_customer_info b where" +
                    " a.Active=1 and a.CustomerName=b.CustID";

                string sSearchtext = "";

                //if (SearchText != null && SearchText != "")
                //{
                //    ViewBag.SearchText = SearchText;
                //    sSearchtext = sSearchtext + "  and (ComplaintNo ='" + SearchText + "' or ComplaintNo like '" + SearchText + "%' or ComplaintNo like '%" + SearchText + "%')";
                //}

                //if (year != "" && year != null && year != "Select")
                //{
                //    ViewBag.yearval = objGlobaldata.GetDropdownitemById(year);
                //    sSearchtext = sSearchtext + " and year(ReceivedDate) = '" + objGlobaldata.GetDropdownitemById(year) + "'";
                //}
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                //if (ChangeIn != null && ChangeIn != "Select")
                //{
                //    ViewBag.Change_In = ChangeIn;

                //    if (sSearchtext != "")
                //    {
                //        sSearchtext = sSearchtext + " and (ChangeIn ='" + ChangeIn + "')";
                //    }
                //    else
                //    {
                //        sSearchtext = sSearchtext + " and (ChangeIn ='" + ChangeIn + "')";
                //    }
                //}
                //if (Approvestatus != null && Approvestatus != "All" && Approvestatus != "")
                //{
                //    ViewBag.ApprovestatusVal = Approvestatus;
                //    if (sSearchtext != "" || ChangeIn != "")
                //    {
                //        sSearchtext = sSearchtext + " and (ApproveStatus ='" + Approvestatus + "')";
                //    }
                //    else
                //    {
                //        sSearchtext = sSearchtext + " and (ApproveStatus ='" + Approvestatus + "')";
                //    }
                //}

                sSqlstmt = sSqlstmt + sSearchtext + " order by ReceivedDate desc";
                DataSet dsComplaintModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsComplaintModelsList.Tables.Count > 0 && dsComplaintModelsList.Tables[0].Rows.Count > 0)
                {  
                    for (int i = 0; i < dsComplaintModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            CustComplaintModels objComplaintModels = new CustComplaintModels
                            {
                                id_complaint = Convert.ToInt32(dsComplaintModelsList.Tables[0].Rows[i]["id_complaint"].ToString()),
                                ComplaintNo = dsComplaintModelsList.Tables[0].Rows[i]["ComplaintNo"].ToString(),
                                LoggedBy = /*objGlobaldata.GetEmpHrNameById*/(dsComplaintModelsList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                CustomerName = objGlobaldata.GetCustomerNameById(dsComplaintModelsList.Tables[0].Rows[i]["CustomerName"].ToString()),
                                ProjectName = dsComplaintModelsList.Tables[0].Rows[i]["ProjectName"].ToString(),
                                ReportedBy =(dsComplaintModelsList.Tables[0].Rows[i]["ReportedBy"].ToString()),
                                ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsComplaintModelsList.Tables[0].Rows[i]["ModeOfComplaint"].ToString()),
                                Details = dsComplaintModelsList.Tables[0].Rows[i]["Details"].ToString(),
                                ForwardTo = objGlobaldata.GetMultiHrEmpNameById(dsComplaintModelsList.Tables[0].Rows[i]["ForwardTo"].ToString()),
                                ForwardToId = (dsComplaintModelsList.Tables[0].Rows[i]["ForwardTo"].ToString()),
                                //ComplaintStatus = objcomp.GetComplaintStatusNameById(dsComplaintModelsList.Tables[0].Rows[i]["ComplaintStatus"].ToString()),
                                Document = dsComplaintModelsList.Tables[0].Rows[i]["Document"].ToString(),
                                ForwarderAssign = dsComplaintModelsList.Tables[0].Rows[i]["ForwarderAssign"].ToString(),
                                CustomerRef = dsComplaintModelsList.Tables[0].Rows[i]["CustomerRef"].ToString(),
                                reportedby_email = dsComplaintModelsList.Tables[0].Rows[i]["reportedby_email"].ToString(),
                                reportedby_desig = dsComplaintModelsList.Tables[0].Rows[i]["reportedby_desig"].ToString(),
                                reportedby_no = dsComplaintModelsList.Tables[0].Rows[i]["reportedby_no"].ToString(),
                            };                          

                            DateTime dtDocDate = new DateTime();
                            if (dsComplaintModelsList.Tables[0].Rows[i]["LoggedDate"].ToString() != ""
                                && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[i]["LoggedDate"].ToString(), out dtDocDate))
                            {
                                objComplaintModels.LoggedDate = dtDocDate;
                            }

                            if (dsComplaintModelsList.Tables[0].Rows[0]["registered_on"].ToString() != ""
                    && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["registered_on"].ToString(), out dtDocDate))
                            {
                                objComplaintModels.registered_on = dtDocDate;
                            }

                            if (dsComplaintModelsList.Tables[0].Rows[i]["ReceivedDate"].ToString() != ""
                          && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[i]["ReceivedDate"].ToString(), out dtDocDate))
                            {
                                objComplaintModels.ReceivedDate = dtDocDate;
                            }

                            objComplaintModels.ComplaintStatus = "Open";
                            //----------start ---

                            CustComplaintModelsList compList = new CustComplaintModelsList();
                            compList.CustComplaintList = new List<CustComplaintModels>();

                            string sSqlstmt1 = "select id_custcomplaint_nc,a.id_complaint,b.ForwarderAssign,nc_no,a.TargetDate,ca_verfiry_duedate,v_status from t_custcomplaint a,t_custcomplaint_nc b where Active=1" +
                          " and a.id_complaint=b.id_complaint and a.id_complaint = '"+ objComplaintModels.id_complaint+ "'";
                            DataSet dsData = objGlobaldata.Getdetails(sSqlstmt1);
                            if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dsData.Tables[0].Rows.Count; j++)
                                {
                                    objcomp = new CustComplaintModels
                                    {
                                        id_complaint = Convert.ToInt32(dsData.Tables[0].Rows[j]["id_complaint"].ToString()),
                                        id_custcomplaint_nc = (dsData.Tables[0].Rows[j]["id_custcomplaint_nc"].ToString()),
                                        ForwardToId = (dsData.Tables[0].Rows[j]["ForwarderAssign"].ToString()),
                                        ForwarderAssign = objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[j]["ForwarderAssign"].ToString()),
                                        nc_no = dsData.Tables[0].Rows[j]["nc_no"].ToString(),                                       
                                    };

                                    DateTime dtDate = new DateTime();
                                    if (dsData.Tables[0].Rows[j]["TargetDate"].ToString() != ""
                                        && DateTime.TryParse(dsData.Tables[0].Rows[j]["TargetDate"].ToString(), out dtDate))
                                    {
                                        objcomp.TargetDate = dtDate;
                                    }

                                    if (dsData.Tables[0].Rows[j]["ca_verfiry_duedate"].ToString() != ""
                                       && DateTime.TryParse(dsData.Tables[0].Rows[j]["ca_verfiry_duedate"].ToString(), out dtDate))
                                    {
                                        objcomp.ca_verfiry_duedate = dtDate;
                                    }
                                    
                                    compList.CustComplaintList.Add(objcomp);
                                }
                            }

                            objComplaintModels.CustList = compList.CustComplaintList;
                                    //--------end ------

                           // --------------------objComplaintModels Status-------
                            string sSqlstmt11 = "select v_status from t_custcomplaint a,t_custcomplaint_nc b where nc_active=1" +
                          " and a.id_complaint=b.id_complaint and a.id_complaint = '" + objComplaintModels.id_complaint + "'";
                            DataSet dsData1 = objGlobaldata.Getdetails(sSqlstmt11);
                            if (dsData1.Tables.Count > 0 && dsData1.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dsData1.Tables[0].Rows.Count; j++)
                                {
                                    if (dsData1.Tables[0].Rows[j]["v_status"].ToString() != "")
                                    {
                                        if (objGlobaldata.GetDropdownitemById(dsData1.Tables[0].Rows[j]["v_status"].ToString()) == "Open")
                                        {
                                            objComplaintModels.ComplaintStatus = "Open";
                                            break;
                                        }
                                        else
                                        {
                                            objComplaintModels.ComplaintStatus = "Closed";
                                        }
                                    }
                                }
                            }                            
                        
                        //--------------------End Complaint Status----------------

                           objComplaintModelsList.CustComplaintList.Add(objComplaintModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }  
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objComplaintModelsList.CustComplaintList.ToList());
        }
        
        [AllowAnonymous]
        public ActionResult CustomerComplaintEdit()
        {
            ViewBag.SubMenutype = "CustomerComplaint";
            CustComplaintModels objComplaintModels = new CustComplaintModels();

            try
            {
                ViewBag.Complaint_Status =objGlobaldata.GetDropdownList("Complaint Status");
                ViewBag.Mode = objGlobaldata.GetModeOfComplaint();
                ViewBag.DeptList = objGlobaldata.GetCompanyBranchListbox();
                //ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Customer = objGlobaldata.GetCustomerListbox();
                UserCredentials objUser = new UserCredentials();

                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.Name = objUser.firstname;
                ViewBag.DeptName = objGlobaldata.GetDeptNameById(objUser.DeptID);
                ViewBag.Designation = objUser.Designation;


                if (Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    string sid_complaint = Request.QueryString["id_complaint"];

                    string sSqlstmt = "select id_complaint,ComplaintNo,LoggedDate,LoggedBy,CustomerName,ProjectName,ReceivedDate,ReportedBy,ModeOfComplaint,"
                    + "Details,divisionId,ForwardTo,ComplaintStatus,Document,CustomerRef,reportedby_email,reportedby_desig,reportedby_no,registered_on from t_custcomplaint where id_complaint='" + sid_complaint + "'";

                    DataSet dsComplaintList = objGlobaldata.Getdetails(sSqlstmt);


                    if (dsComplaintList.Tables.Count > 0 && dsComplaintList.Tables[0].Rows.Count > 0)
                    {

                        if (objUser.empid == dsComplaintList.Tables[0].Rows[0]["LoggedBy"].ToString())
                        {
                            objComplaintModels = new CustComplaintModels
                            {
                                id_complaint = Convert.ToInt16(dsComplaintList.Tables[0].Rows[0]["id_complaint"].ToString()),
                                ComplaintNo = dsComplaintList.Tables[0].Rows[0]["ComplaintNo"].ToString(),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsComplaintList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                                CustomerName = dsComplaintList.Tables[0].Rows[0]["CustomerName"].ToString(),
                                ProjectName = dsComplaintList.Tables[0].Rows[0]["ProjectName"].ToString(),
                                ReportedBy = dsComplaintList.Tables[0].Rows[0]["ReportedBy"].ToString(),
                                ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsComplaintList.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                                Details = dsComplaintList.Tables[0].Rows[0]["Details"].ToString(),
                                divisionId = (dsComplaintList.Tables[0].Rows[0]["divisionId"].ToString()),
                                ForwardTo = (dsComplaintList.Tables[0].Rows[0]["ForwardTo"].ToString()),
                                ComplaintStatus = objGlobaldata.GetDropdownitemById(dsComplaintList.Tables[0].Rows[0]["ComplaintStatus"].ToString()),
                                Document = dsComplaintList.Tables[0].Rows[0]["Document"].ToString(),
                                CustomerRef = dsComplaintList.Tables[0].Rows[0]["CustomerRef"].ToString(),
                                reportedby_email = dsComplaintList.Tables[0].Rows[0]["reportedby_email"].ToString(),
                                reportedby_desig = dsComplaintList.Tables[0].Rows[0]["reportedby_desig"].ToString(),
                                reportedby_no = dsComplaintList.Tables[0].Rows[0]["reportedby_no"].ToString(),

                            };
                            ViewBag.EmpLists = objGlobaldata.GetHrEmpListByDivision(dsComplaintList.Tables[0].Rows[0]["divisionId"].ToString());
                            DateTime dtDocDate = new DateTime();
                            if (dsComplaintList.Tables[0].Rows[0]["LoggedDate"].ToString() != ""
                                && DateTime.TryParse(dsComplaintList.Tables[0].Rows[0]["LoggedDate"].ToString(), out dtDocDate))
                            {
                                objComplaintModels.LoggedDate = dtDocDate;
                            }

                            if (dsComplaintList.Tables[0].Rows[0]["registered_on"].ToString() != ""
                               && DateTime.TryParse(dsComplaintList.Tables[0].Rows[0]["registered_on"].ToString(), out dtDocDate))
                            {
                                objComplaintModels.registered_on = dtDocDate;
                            }

                            if (dsComplaintList.Tables[0].Rows[0]["ReceivedDate"].ToString() != ""
                            && DateTime.TryParse(dsComplaintList.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtDocDate))
                            {
                                objComplaintModels.ReceivedDate = dtDocDate;
                            }

                            if (dsComplaintList.Tables[0].Rows[0]["ForwardTo"].ToString() != "")
                            {
                                ViewBag.NotifiedtoArray = (dsComplaintList.Tables[0].Rows[0]["ForwardTo"].ToString()).Split(',');
                            }
                        }
                        else
                        {

                            ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);

                            TempData["alertdata"] = "Access Denied";
                            return RedirectToAction("CustomerComplaintList");
                        }

                    }
                    else
                    {

                        ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);

                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("CustomerComplaintList");
                    }
                }
                else
                {

                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);

                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("CustomerComplaintList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("CustomerComplaintList");
            }

            return View(objComplaintModels);

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult CustomerComplaintEdit(CustComplaintModels objCustomerCompliant, FormCollection form, IEnumerable<HttpPostedFileBase> file)
        {
            try
            {
                objCustomerCompliant.CustomerName = form["CustomerName"];
                objCustomerCompliant.ProjectName = form["ProjectName"];
                objCustomerCompliant.ReportedBy = form["ReportedBy"];
                objCustomerCompliant.ModeOfComplaint = form["ModeOfComplaint"];
                objCustomerCompliant.ComplaintStatus = form["ComplaintStatus"];
                objCustomerCompliant.Details = form["Details"];
                objCustomerCompliant.divisionId = form["DeptId"];
                objCustomerCompliant.ForwardTo = form["ForwardTo"];

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                DateTime dateValue;

                if (DateTime.TryParse(form["ReceivedDate"], out dateValue) == true)
                {
                    objCustomerCompliant.ReceivedDate = dateValue;
                }

                if (DateTime.TryParse(form["registered_on"], out dateValue) == true)
                {
                    objCustomerCompliant.registered_on = dateValue;
                }

                if (file != null && files.ContentLength > 0)
                {
                    objCustomerCompliant.Document = "";

                    foreach (var document in file)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Customer"), Path.GetFileName(document.FileName));
                            string sFilename = "Complaint" + "_" + objCustomerCompliant.ComplaintNo + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            document.SaveAs(sFilepath + "/" + sFilename);
                            objCustomerCompliant.Document = objCustomerCompliant.Document + "," + "~/DataUpload/MgmtDocs/Customer/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objCustomerCompliant.Document = objCustomerCompliant.Document.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objCustomerCompliant.Document = objCustomerCompliant.Document + "," + form["QCDocsVal"];
                    objCustomerCompliant.Document = objCustomerCompliant.Document.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objCustomerCompliant.Document = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objCustomerCompliant.Document = null;
                }

                if (objCustomerCompliant.FunUpdateCustomerComplaint(objCustomerCompliant))
                {
                    TempData["Successdata"] = "Customer complaint Updated successfully";
                    objCustomerCompliant.SendCustComplaintRegistmail(objCustomerCompliant.ComplaintNo, "Customer Complaint Registered");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerComplaintList");
        }

        [AllowAnonymous]
        public ActionResult CustomerComplaintDetails(FormCollection form)
        {
            CustComplaintModels objComplaintModels = new CustComplaintModels();
            //CustComplaintModels objnewModel= new CustComplaintModels();
            try
            {
                UserCredentials objUser = new UserCredentials();

                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);


                ViewBag.Complaint_Status = objGlobaldata.GetDropdownList("Complaint Status");

                CustComplaintModels objModel = new CustComplaintModels();
               

                if (Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    string sid_complaint = Request.QueryString["id_complaint"];


                    string sSqlstmt = "select *  from t_custcomplaint where id_complaint='" + sid_complaint + "'";

                    DataSet dsComplaintModelsList = objGlobaldata.Getdetails(sSqlstmt);
                                                           
                    if (dsComplaintModelsList.Tables.Count > 0 && dsComplaintModelsList.Tables[0].Rows.Count > 0)
                    {
                        objComplaintModels = new CustComplaintModels
                        {
                            id_complaint = Convert.ToInt16(dsComplaintModelsList.Tables[0].Rows[0]["id_complaint"].ToString()),
                            ComplaintNo = dsComplaintModelsList.Tables[0].Rows[0]["ComplaintNo"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsComplaintModelsList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            CustomerName = objGlobaldata.GetCustomerNameById(dsComplaintModelsList.Tables[0].Rows[0]["CustomerName"].ToString()),
                            ProjectName = dsComplaintModelsList.Tables[0].Rows[0]["ProjectName"].ToString(),
                            ReportedBy = dsComplaintModelsList.Tables[0].Rows[0]["ReportedBy"].ToString(),
                            ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsComplaintModelsList.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                            Details = dsComplaintModelsList.Tables[0].Rows[0]["Details"].ToString(),
                            ForwardTo = objGlobaldata.GetMultiHrEmpNameById(dsComplaintModelsList.Tables[0].Rows[0]["ForwardTo"].ToString()),
                            //ComplaintStatus = objComplaintModels.GetComplaintStatusNameById(dsComplaintModelsList.Tables[0].Rows[0]["ComplaintStatus"].ToString()),
                            Document = dsComplaintModelsList.Tables[0].Rows[0]["Document"].ToString(),
                            ForwarderAssign = dsComplaintModelsList.Tables[0].Rows[0]["ForwarderAssign"].ToString(),
                            CustomerRef = dsComplaintModelsList.Tables[0].Rows[0]["CustomerRef"].ToString(),
                            reportedby_email = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_email"].ToString(),
                            reportedby_desig = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_desig"].ToString(),
                            reportedby_no = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_no"].ToString(),

                            c_response = objGlobaldata.GetDropdownitemById(dsComplaintModelsList.Tables[0].Rows[0]["c_response"].ToString()),
                            c_response_details = dsComplaintModelsList.Tables[0].Rows[0]["c_response_details"].ToString(),
                            c_reponse_upload = dsComplaintModelsList.Tables[0].Rows[0]["c_reponse_upload"].ToString(),
                            
                            complaint_valid = dsComplaintModelsList.Tables[0].Rows[0]["complaint_valid"].ToString(),
                            complaint_deviation = dsComplaintModelsList.Tables[0].Rows[0]["complaint_deviation"].ToString(),
                            complaint_remark = dsComplaintModelsList.Tables[0].Rows[0]["complaint_remark"].ToString(),
                            rej_reason = dsComplaintModelsList.Tables[0].Rows[0]["rej_reason"].ToString(),
                            rej_upload = dsComplaintModelsList.Tables[0].Rows[0]["rej_upload"].ToString(),
                        };

                        DateTime dtDocDate = new DateTime();
                        if (dsComplaintModelsList.Tables[0].Rows[0]["LoggedDate"].ToString() != ""
                            && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["LoggedDate"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.LoggedDate = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["registered_on"].ToString() != ""
                      && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["registered_on"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.registered_on = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["ReceivedDate"].ToString() != ""
                    && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.ReceivedDate = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["c_response_date"].ToString() != ""
                   && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["c_response_date"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.c_response_date = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["complain_review_date"].ToString() != ""
                    && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["complain_review_date"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.complain_review_date = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["TargetDate"].ToString() != ""
                   && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["TargetDate"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.TargetDate = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["AssignDate"].ToString() != ""
                   && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["AssignDate"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.AssignDate = dtDocDate;
                        }

                        string sql = "Select branch,Department,Location from t_customer_info where CustID = '" + dsComplaintModelsList.Tables[0].Rows[0]["CustomerName"].ToString() + "'";
                        DataSet CustList = objGlobaldata.Getdetails(sql);
                        if (CustList.Tables.Count > 0 && CustList.Tables[0].Rows.Count > 0)
                        {
                            objComplaintModels.branch = objGlobaldata.GetMultiCompanyBranchNameById(CustList.Tables[0].Rows[0]["branch"].ToString());
                            objComplaintModels.Department = objGlobaldata.GetMultiDeptNameById(CustList.Tables[0].Rows[0]["Department"].ToString());
                            objComplaintModels.Location = objGlobaldata.GetDivisionLocationById(CustList.Tables[0].Rows[0]["Location"].ToString());
                        }

                        // --------------------objComplaintModels Status-------
                        objComplaintModels.ComplaintStatus = "Open";
                        string sSqlstmt101 = "select v_status from t_custcomplaint a,t_custcomplaint_nc b where nc_active=1" +
                      " and a.id_complaint=b.id_complaint and a.id_complaint = '" + objComplaintModels.id_complaint + "'";
                        DataSet dsData1 = objGlobaldata.Getdetails(sSqlstmt101);
                        if (dsData1.Tables.Count > 0 && dsData1.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j < dsData1.Tables[0].Rows.Count; j++)
                            {
                                if (dsData1.Tables[0].Rows[j]["v_status"].ToString() != "")
                                {
                                    if (objGlobaldata.GetDropdownitemById(dsData1.Tables[0].Rows[j]["v_status"].ToString()) == "Open")
                                    {
                                        objComplaintModels.ComplaintStatus = "Open";
                                        break;
                                    }
                                    else
                                    {
                                        objComplaintModels.ComplaintStatus = "Closed";
                                    }
                                }
                            }
                        }

                        //--------------------End Complaint Status----------------


                        CustComplaintModelsList NcOverallList = new CustComplaintModelsList();
                        NcOverallList.CustComplaintList = new List<CustComplaintModels>();


                        CustComplaintModelsList NCTeamList = new CustComplaintModelsList();
                        NCTeamList.CustComplaintList = new List<CustComplaintModels>();

                        string stmt = " select id_custcomplaint_nc,ForwarderAssign from t_custcomplaint_nc where id_complaint = '" + sid_complaint + "'";
                        DataSet CList = objGlobaldata.Getdetails(stmt);
                        if (CList.Tables.Count > 0 && CList.Tables[0].Rows.Count > 0)
                        {
                            for (int t = 0; t < CList.Tables[0].Rows.Count; t++)
                            { 
                            objComplaintModels.id_custcomplaint_nc = (CList.Tables[0].Rows[t]["id_custcomplaint_nc"].ToString());
                            
                                //All 5 steps included here
                                    string sSqlstmt11 = "select * from t_custcomplaint_nc where id_custcomplaint_nc='" + objComplaintModels.id_custcomplaint_nc + "'";
                                    DataSet dsDispModel = objGlobaldata.Getdetails(sSqlstmt11);
                                    //ViewBag.Disposition = dsDispModel;
                                    if (dsDispModel.Tables.Count > 0 && dsDispModel.Tables[0].Rows.Count > 0)
                                    {
                                        
                                        objModel = new CustComplaintModels
                                        {
                                            ForwarderAssign = (dsDispModel.Tables[0].Rows[0]["ForwarderAssign"].ToString()),
                                            disp_action_taken = objGlobaldata.GetDropdownitemById(dsDispModel.Tables[0].Rows[0]["disp_action_taken"].ToString()),
                                            disp_explain = (dsDispModel.Tables[0].Rows[0]["disp_explain"].ToString()),
                                            disp_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["disp_notifiedto"].ToString()),
                                            disp_upload = (dsDispModel.Tables[0].Rows[0]["disp_upload"].ToString()),

                                            nc_team = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["nc_team"].ToString()),

                                            rca_technique = objGlobaldata.GetDropdownitemById(dsDispModel.Tables[0].Rows[0]["rca_technique"].ToString()),
                                            rca_details = (dsDispModel.Tables[0].Rows[0]["rca_details"].ToString()),
                                            rca_upload = (dsDispModel.Tables[0].Rows[0]["rca_upload"].ToString()),
                                            rca_action = (dsDispModel.Tables[0].Rows[0]["rca_action"].ToString()),
                                            rca_justify = (dsDispModel.Tables[0].Rows[0]["rca_justify"].ToString()),
                                            rca_reportedby = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["rca_reportedby"].ToString()),
                                            rca_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["rca_notifiedto"].ToString()),

                                            ca_proposed_by = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["ca_proposed_by"].ToString()),
                                            ca_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["ca_notifiedto"].ToString()),

                                            v_implement = objGlobaldata.GetDropdownitemById(dsDispModel.Tables[0].Rows[0]["v_implement"].ToString()),
                                            v_implement_explain = (dsDispModel.Tables[0].Rows[0]["v_implement_explain"].ToString()),
                                            v_rca = (dsDispModel.Tables[0].Rows[0]["v_rca"].ToString()),
                                            v_rca_explain = (dsDispModel.Tables[0].Rows[0]["v_rca_explain"].ToString()),
                                            v_discrepancies = (dsDispModel.Tables[0].Rows[0]["v_discrepancies"].ToString()),
                                            v_discrep_explain = (dsDispModel.Tables[0].Rows[0]["v_discrep_explain"].ToString()),
                                            v_upload = (dsDispModel.Tables[0].Rows[0]["v_upload"].ToString()),
                                            v_status = objGlobaldata.GetDropdownitemById(dsDispModel.Tables[0].Rows[0]["v_status"].ToString()),
                                            v_verifiedto = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["v_verifiedto"].ToString()),
                                            v_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["v_notifiedto"].ToString()),
                                        };

                                        DateTime dtValue;
                                        if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["disp_notifeddate"].ToString(), out dtValue))
                                        {
                                            objModel.disp_notifeddate = dtValue;
                                        }

                                        if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["team_targetdate"].ToString(), out dtValue))
                                        {
                                           objModel.team_targetdate = dtValue;
                                        }

                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["rca_startdate"].ToString(), out dtValue))
                                    {
                                        objModel.rca_startdate = dtValue;
                                    }
                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["rca_reporteddate"].ToString(), out dtValue))
                                    {
                                        objModel.rca_reporteddate = dtValue;
                                    }

                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["ca_verfiry_duedate"].ToString(), out dtValue))
                                    {
                                        objModel.ca_verfiry_duedate = dtValue;
                                    }
                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["ca_notifed_date"].ToString(), out dtValue))
                                    {
                                        objModel.ca_notifed_date = dtValue;
                                    }

                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["v_verified_date"].ToString(), out dtValue))
                                    {
                                        objModel.v_verified_date = dtValue;
                                    }
                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["v_closed_date"].ToString(), out dtValue))
                                    {
                                        objModel.v_closed_date = dtValue;
                                    }

                                    //------------------start DispTrans-----------
                                    CustComplaintModelsList objdispList = new CustComplaintModelsList();
                                    objdispList.CustComplaintList = new List<CustComplaintModels>();

                                    string sSqlstmt1 = "select id_cust_nc_disp_action,disp_action,disp_resp_person,disp_complete_date from t_custcomplaint_nc_disp_action where id_custcomplaint_nc='" + objComplaintModels.id_custcomplaint_nc + "'";
                                    DataSet dsDisptransModels = objGlobaldata.Getdetails(sSqlstmt1);

                                    if (dsDisptransModels.Tables.Count > 0 && dsDisptransModels.Tables[0].Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dsDisptransModels.Tables[0].Rows.Count; i++)
                                        {
                                            try
                                            {
                                                CustComplaintModels objDispModel = new CustComplaintModels
                                                {
                                                    id_cust_nc_disp_action = (dsDisptransModels.Tables[0].Rows[i]["id_cust_nc_disp_action"].ToString()),
                                                    disp_action = (dsDisptransModels.Tables[0].Rows[i]["disp_action"].ToString()),
                                                    disp_resp_person = objGlobaldata.GetMultiHrEmpNameById(dsDisptransModels.Tables[0].Rows[i]["disp_resp_person"].ToString()),
                                                };

                                                if (DateTime.TryParse(dsDisptransModels.Tables[0].Rows[i]["disp_complete_date"].ToString(), out dtValue))
                                                {
                                                    objDispModel.disp_complete_date = dtValue;
                                                }
                                                objdispList.CustComplaintList.Add(objDispModel);
                                            }
                                            catch (Exception ex)
                                            {
                                                objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
                                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                            }
                                            objModel.CustList = objdispList.CustComplaintList;
                                        }
                                    }

                                    //------------------End DispTrans-----------


                                    //------------------start CA-----------                                   

                                    CustComplaintModelsList CAList = new CustComplaintModelsList();
                                    CAList.CustComplaintList = new List<CustComplaintModels>();

                                    string Ssql = "select id_cust_nc_ca,id_custcomplaint_nc,ca_ca,ca_resource," +
                                    "ca_target_date,ca_resp_person from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc = '" + objComplaintModels.id_custcomplaint_nc + "' and ca_active=1";

                                    DataSet dsCALsit = objGlobaldata.Getdetails(Ssql);

                                    if (dsCALsit.Tables.Count > 0 && dsCALsit.Tables[0].Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dsCALsit.Tables[0].Rows.Count; i++)
                                        {
                                            try
                                            {
                                                CustComplaintModels objCustComplaintModels = new CustComplaintModels
                                                {
                                                    id_cust_nc_ca = (dsCALsit.Tables[0].Rows[i]["id_cust_nc_ca"].ToString()),
                                                    ca_ca = (dsCALsit.Tables[0].Rows[i]["ca_ca"].ToString()),
                                                    ca_resource = (dsCALsit.Tables[0].Rows[i]["ca_resource"].ToString()),
                                                    ca_resp_person = objGlobaldata.GetMultiHrEmpNameById(dsCALsit.Tables[0].Rows[i]["ca_resp_person"].ToString()),
                                                };
                                                if (DateTime.TryParse(dsCALsit.Tables[0].Rows[i]["ca_target_date"].ToString(), out dtValue))
                                                {
                                                    objCustComplaintModels.ca_target_date = dtValue;
                                                }
                                                CAList.CustComplaintList.Add(objCustComplaintModels);
                                            }
                                            catch (Exception ex)
                                            {
                                                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintDetails: " + ex.ToString());
                                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                            }
                                        }
                                    }
                                    objModel.CustList = CAList.CustComplaintList;

                                    //------------------End CA-----------


                                    //---------------Start Verification----------------
                                    CustComplaintModelsList objVeriList = new CustComplaintModelsList();
                                    objVeriList.CustComplaintList = new List<CustComplaintModels>();

                                    string Sql = "Select id_cust_nc_ca,ca_ca,ca_resource," +
                                   "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc = '" + objComplaintModels.id_custcomplaint_nc + "' and ca_active=1";

                                    DataSet dsCAModels = objGlobaldata.Getdetails(Sql);

                                    if (dsCAModels.Tables.Count > 0 && dsCAModels.Tables[0].Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dsCAModels.Tables[0].Rows.Count; i++)
                                        {
                                            try
                                            {
                                                CustComplaintModels objCAModel = new CustComplaintModels
                                                {
                                                    id_cust_nc_ca = (dsCAModels.Tables[0].Rows[i]["id_cust_nc_ca"].ToString()),
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
                                                objVeriList.CustComplaintList.Add(objCAModel);
                                            }
                                            catch (Exception Ex)
                                            {
                                                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintDetails: " + Ex.ToString());
                                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                            }
                                        }
                                        objModel.CustList = objVeriList.CustComplaintList;
                                    }
                                    //---------------End Verification------------------


                                    NcOverallList.CustComplaintList.Add(objModel);
                               }
                            }
                        ViewBag.NcOverallList = NcOverallList;
                    }
                       
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerComplaintList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("CustomerComplaintList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objComplaintModels);
        }


        [AllowAnonymous]
        public ActionResult CustomerComplaintInfo(int id)
        {
            CustComplaintModels objComplaintModels = new CustComplaintModels();
            try
            {
                string sSqlstmt = "select *  from t_custcomplaint where id_complaint='" + id + "'";
                DataSet dsComplaintModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsComplaintModelsList.Tables.Count > 0 && dsComplaintModelsList.Tables[0].Rows.Count > 0)
                {
                    objComplaintModels = new CustComplaintModels
                    {
                        id_complaint = Convert.ToInt16(dsComplaintModelsList.Tables[0].Rows[0]["id_complaint"].ToString()),
                        ComplaintNo = dsComplaintModelsList.Tables[0].Rows[0]["ComplaintNo"].ToString(),
                        LoggedBy = objGlobaldata.GetEmpHrNameById(dsComplaintModelsList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                        CustomerName = objGlobaldata.GetCustomerNameById(dsComplaintModelsList.Tables[0].Rows[0]["CustomerName"].ToString()),
                        ProjectName = dsComplaintModelsList.Tables[0].Rows[0]["ProjectName"].ToString(),
                        ReportedBy = dsComplaintModelsList.Tables[0].Rows[0]["ReportedBy"].ToString(),
                        ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsComplaintModelsList.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                        Details = dsComplaintModelsList.Tables[0].Rows[0]["Details"].ToString(),
                        ForwardTo = objGlobaldata.GetMultiHrEmpNameById(dsComplaintModelsList.Tables[0].Rows[0]["ForwardTo"].ToString()),
                        ComplaintStatus = objGlobaldata.GetDropdownitemById(dsComplaintModelsList.Tables[0].Rows[0]["ComplaintStatus"].ToString()),
                        Document = dsComplaintModelsList.Tables[0].Rows[0]["Document"].ToString(),
                        ForwarderAssign = dsComplaintModelsList.Tables[0].Rows[0]["ForwarderAssign"].ToString(),
                        CustomerRef = dsComplaintModelsList.Tables[0].Rows[0]["CustomerRef"].ToString(),
                        reportedby_email = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_email"].ToString(),
                        reportedby_desig = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_desig"].ToString(),
                        reportedby_no = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_no"].ToString(),
                    };

                    DateTime dtDocDate = new DateTime();
                    if (dsComplaintModelsList.Tables[0].Rows[0]["LoggedDate"].ToString() != ""
                        && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["LoggedDate"].ToString(), out dtDocDate))
                    {
                        objComplaintModels.LoggedDate = dtDocDate;
                    }

                    if (dsComplaintModelsList.Tables[0].Rows[0]["registered_on"].ToString() != ""
                    && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["registered_on"].ToString(), out dtDocDate))
                    {
                        objComplaintModels.registered_on = dtDocDate;
                    }

                    if (dsComplaintModelsList.Tables[0].Rows[0]["ReceivedDate"].ToString() != ""
                  && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtDocDate))
                    {
                        objComplaintModels.ReceivedDate = dtDocDate;
                    }             
                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("CustomerComplaintList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objComplaintModels);

        }
             

       
        //[ValidateAntiForgeryToken]
        public ActionResult CustomerComplaintReport()
        {
            CustComplaintModels objComplaintModels = new CustComplaintModels();
            //CustComplaintModels objnewModel= new CustComplaintModels();
            try
            {
                UserCredentials objUser = new UserCredentials();

                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);


                ViewBag.Complaint_Status = objGlobaldata.GetDropdownList("Complaint Status");

                CustComplaintModels objModel = new CustComplaintModels();
                
                if (Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    string sid_complaint = Request.QueryString["id_complaint"];


                    string sSqlstmt = "select *  from t_custcomplaint where id_complaint='" + sid_complaint + "'";

                    DataSet dsComplaintModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsComplaintModelsList.Tables.Count > 0 && dsComplaintModelsList.Tables[0].Rows.Count > 0)
                    {
                        objComplaintModels = new CustComplaintModels
                        {
                            id_complaint = Convert.ToInt16(dsComplaintModelsList.Tables[0].Rows[0]["id_complaint"].ToString()),
                            ComplaintNo = dsComplaintModelsList.Tables[0].Rows[0]["ComplaintNo"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsComplaintModelsList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            CustomerName = objGlobaldata.GetCustomerNameById(dsComplaintModelsList.Tables[0].Rows[0]["CustomerName"].ToString()),
                            ProjectName = dsComplaintModelsList.Tables[0].Rows[0]["ProjectName"].ToString(),
                            ReportedBy = dsComplaintModelsList.Tables[0].Rows[0]["ReportedBy"].ToString(),
                            ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsComplaintModelsList.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                            Details = dsComplaintModelsList.Tables[0].Rows[0]["Details"].ToString(),
                            ForwardTo = objGlobaldata.GetMultiHrEmpNameById(dsComplaintModelsList.Tables[0].Rows[0]["ForwardTo"].ToString()),
                            ComplaintStatus = objGlobaldata.GetDropdownitemById(dsComplaintModelsList.Tables[0].Rows[0]["ComplaintStatus"].ToString()),
                            Document = dsComplaintModelsList.Tables[0].Rows[0]["Document"].ToString(),
                            ForwarderAssign = dsComplaintModelsList.Tables[0].Rows[0]["ForwarderAssign"].ToString(),
                            CustomerRef = dsComplaintModelsList.Tables[0].Rows[0]["CustomerRef"].ToString(),
                            reportedby_email = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_email"].ToString(),
                            reportedby_desig = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_desig"].ToString(),
                            reportedby_no = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_no"].ToString(),

                            c_response = objGlobaldata.GetDropdownitemById(dsComplaintModelsList.Tables[0].Rows[0]["c_response"].ToString()),
                            c_response_details = dsComplaintModelsList.Tables[0].Rows[0]["c_response_details"].ToString(),
                            c_reponse_upload = dsComplaintModelsList.Tables[0].Rows[0]["c_reponse_upload"].ToString(),

                            complaint_valid = dsComplaintModelsList.Tables[0].Rows[0]["complaint_valid"].ToString(),
                            complaint_deviation = dsComplaintModelsList.Tables[0].Rows[0]["complaint_deviation"].ToString(),
                            complaint_remark = dsComplaintModelsList.Tables[0].Rows[0]["complaint_remark"].ToString(),
                            rej_reason = dsComplaintModelsList.Tables[0].Rows[0]["rej_reason"].ToString(),
                            rej_upload = dsComplaintModelsList.Tables[0].Rows[0]["rej_upload"].ToString(),
                        };

                        DateTime dtDocDate = new DateTime();
                        if (dsComplaintModelsList.Tables[0].Rows[0]["LoggedDate"].ToString() != ""
                            && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["LoggedDate"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.LoggedDate = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["registered_on"].ToString() != ""
                      && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["registered_on"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.registered_on = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["ReceivedDate"].ToString() != ""
                    && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.ReceivedDate = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["c_response_date"].ToString() != ""
                   && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["c_response_date"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.c_response_date = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["complain_review_date"].ToString() != ""
                    && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["complain_review_date"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.complain_review_date = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["TargetDate"].ToString() != ""
                   && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["TargetDate"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.TargetDate = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["AssignDate"].ToString() != ""
                   && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["AssignDate"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.AssignDate = dtDocDate;
                        }

                        string sql = "Select branch,Department,Location from t_customer_info where CustID = '" + dsComplaintModelsList.Tables[0].Rows[0]["CustomerName"].ToString() + "'";
                        DataSet CustList = objGlobaldata.Getdetails(sql);
                        if (CustList.Tables.Count > 0 && CustList.Tables[0].Rows.Count > 0)
                        {
                            objComplaintModels.branch = objGlobaldata.GetMultiCompanyBranchNameById(CustList.Tables[0].Rows[0]["branch"].ToString());
                            objComplaintModels.Department = objGlobaldata.GetMultiDeptNameById(CustList.Tables[0].Rows[0]["Department"].ToString());
                            objComplaintModels.Location = objGlobaldata.GetDivisionLocationById(CustList.Tables[0].Rows[0]["Location"].ToString());
                        }

                        CompanyModels objCompany = new CompanyModels();
                        dsComplaintModelsList = objCompany.GetCompanyDetailsForReport(dsComplaintModelsList);

                        dsComplaintModelsList = objGlobaldata.GetReportDetails(dsComplaintModelsList, objComplaintModels.ComplaintNo, dsComplaintModelsList.Tables[0].Rows[0]["LoggedBy"].ToString(), "CUSTOMER COMPLAINT REPORT");
                        ViewBag.CompanyInfo = dsComplaintModelsList;

                        // --------------------objComplaintModels Status-------
                        objComplaintModels.ComplaintStatus = "Open";
                        string sSqlstmt101 = "select v_status from t_custcomplaint a,t_custcomplaint_nc b where nc_active=1" +
                      " and a.id_complaint=b.id_complaint and a.id_complaint = '" + objComplaintModels.id_complaint + "'";
                        DataSet dsData1 = objGlobaldata.Getdetails(sSqlstmt101);
                        if (dsData1.Tables.Count > 0 && dsData1.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j < dsData1.Tables[0].Rows.Count; j++)
                            {
                                if (dsData1.Tables[0].Rows[j]["v_status"].ToString() != "")
                                {
                                    if (objGlobaldata.GetDropdownitemById(dsData1.Tables[0].Rows[j]["v_status"].ToString()) == "Open")
                                    {
                                        objComplaintModels.ComplaintStatus = "Open";
                                        break;
                                    }
                                    else
                                    {
                                        objComplaintModels.ComplaintStatus = "Closed";
                                    }
                                }
                            }
                        }

                        //--------------------End Complaint Status----------------


                        CustComplaintModelsList NcOverallList = new CustComplaintModelsList();
                        NcOverallList.CustComplaintList = new List<CustComplaintModels>();


                        CustComplaintModelsList NCTeamList = new CustComplaintModelsList();
                        NCTeamList.CustComplaintList = new List<CustComplaintModels>();

                        string stmt = " select id_custcomplaint_nc,ForwarderAssign from t_custcomplaint_nc where id_complaint = '" + sid_complaint + "'";
                        DataSet CList = objGlobaldata.Getdetails(stmt);
                        if (CList.Tables.Count > 0 && CList.Tables[0].Rows.Count > 0)
                        {
                            for (int t = 0; t < CList.Tables[0].Rows.Count; t++)
                            {
                                objComplaintModels.id_custcomplaint_nc = (CList.Tables[0].Rows[t]["id_custcomplaint_nc"].ToString());

                                //All 5 steps included here
                                string sSqlstmt11 = "select * from t_custcomplaint_nc where id_custcomplaint_nc='" + objComplaintModels.id_custcomplaint_nc + "'";
                                DataSet dsDispModel = objGlobaldata.Getdetails(sSqlstmt11);
                                //ViewBag.Disposition = dsDispModel;
                                if (dsDispModel.Tables.Count > 0 && dsDispModel.Tables[0].Rows.Count > 0)
                                {

                                    objModel = new CustComplaintModels
                                    {
                                        ForwarderAssign = (dsDispModel.Tables[0].Rows[0]["ForwarderAssign"].ToString()),
                                        disp_action_taken = objGlobaldata.GetDropdownitemById(dsDispModel.Tables[0].Rows[0]["disp_action_taken"].ToString()),
                                        disp_explain = (dsDispModel.Tables[0].Rows[0]["disp_explain"].ToString()),
                                        disp_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["disp_notifiedto"].ToString()),
                                        disp_upload = (dsDispModel.Tables[0].Rows[0]["disp_upload"].ToString()),

                                        nc_team = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["nc_team"].ToString()),

                                        rca_technique = objGlobaldata.GetDropdownitemById(dsDispModel.Tables[0].Rows[0]["rca_technique"].ToString()),
                                        rca_details = (dsDispModel.Tables[0].Rows[0]["rca_details"].ToString()),
                                        rca_upload = (dsDispModel.Tables[0].Rows[0]["rca_upload"].ToString()),
                                        rca_action = (dsDispModel.Tables[0].Rows[0]["rca_action"].ToString()),
                                        rca_justify = (dsDispModel.Tables[0].Rows[0]["rca_justify"].ToString()),
                                        rca_reportedby = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["rca_reportedby"].ToString()),
                                        rca_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["rca_notifiedto"].ToString()),

                                        ca_proposed_by = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["ca_proposed_by"].ToString()),
                                        ca_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["ca_notifiedto"].ToString()),

                                        v_implement = objGlobaldata.GetDropdownitemById(dsDispModel.Tables[0].Rows[0]["v_implement"].ToString()),
                                        v_implement_explain = (dsDispModel.Tables[0].Rows[0]["v_implement_explain"].ToString()),
                                        v_rca = (dsDispModel.Tables[0].Rows[0]["v_rca"].ToString()),
                                        v_rca_explain = (dsDispModel.Tables[0].Rows[0]["v_rca_explain"].ToString()),
                                        v_discrepancies = (dsDispModel.Tables[0].Rows[0]["v_discrepancies"].ToString()),
                                        v_discrep_explain = (dsDispModel.Tables[0].Rows[0]["v_discrep_explain"].ToString()),
                                        v_upload = (dsDispModel.Tables[0].Rows[0]["v_upload"].ToString()),
                                        v_status = objGlobaldata.GetDropdownitemById(dsDispModel.Tables[0].Rows[0]["v_status"].ToString()),
                                        v_verifiedto = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["v_verifiedto"].ToString()),
                                        v_notifiedto = objGlobaldata.GetMultiHrEmpNameById(dsDispModel.Tables[0].Rows[0]["v_notifiedto"].ToString()),
                                    };

                                    DateTime dtValue;
                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["disp_notifeddate"].ToString(), out dtValue))
                                    {
                                        objModel.disp_notifeddate = dtValue;
                                    }

                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["team_targetdate"].ToString(), out dtValue))
                                    {
                                        objModel.team_targetdate = dtValue;
                                    }

                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["rca_startdate"].ToString(), out dtValue))
                                    {
                                        objModel.rca_startdate = dtValue;
                                    }
                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["rca_reporteddate"].ToString(), out dtValue))
                                    {
                                        objModel.rca_reporteddate = dtValue;
                                    }

                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["ca_verfiry_duedate"].ToString(), out dtValue))
                                    {
                                        objModel.ca_verfiry_duedate = dtValue;
                                    }
                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["ca_notifed_date"].ToString(), out dtValue))
                                    {
                                        objModel.ca_notifed_date = dtValue;
                                    }

                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["v_verified_date"].ToString(), out dtValue))
                                    {
                                        objModel.v_verified_date = dtValue;
                                    }
                                    if (DateTime.TryParse(dsDispModel.Tables[0].Rows[0]["v_closed_date"].ToString(), out dtValue))
                                    {
                                        objModel.v_closed_date = dtValue;
                                    }

                                    //------------------start DispTrans-----------
                                    CustComplaintModelsList objdispList = new CustComplaintModelsList();
                                    objdispList.CustComplaintList = new List<CustComplaintModels>();

                                    string sSqlstmt1 = "select id_cust_nc_disp_action,disp_action,disp_resp_person,disp_complete_date from t_custcomplaint_nc_disp_action where id_custcomplaint_nc='" + objComplaintModels.id_custcomplaint_nc + "'";
                                    DataSet dsDisptransModels = objGlobaldata.Getdetails(sSqlstmt1);

                                    if (dsDisptransModels.Tables.Count > 0 && dsDisptransModels.Tables[0].Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dsDisptransModels.Tables[0].Rows.Count; i++)
                                        {
                                            try
                                            {
                                                CustComplaintModels objDispModel = new CustComplaintModels
                                                {
                                                    id_cust_nc_disp_action = (dsDisptransModels.Tables[0].Rows[i]["id_cust_nc_disp_action"].ToString()),
                                                    disp_action = (dsDisptransModels.Tables[0].Rows[i]["disp_action"].ToString()),
                                                    disp_resp_person = objGlobaldata.GetMultiHrEmpNameById(dsDisptransModels.Tables[0].Rows[i]["disp_resp_person"].ToString()),
                                                };

                                                if (DateTime.TryParse(dsDisptransModels.Tables[0].Rows[i]["disp_complete_date"].ToString(), out dtValue))
                                                {
                                                    objDispModel.disp_complete_date = dtValue;
                                                }
                                                objdispList.CustComplaintList.Add(objDispModel);
                                            }
                                            catch (Exception ex)
                                            {
                                                objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
                                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                            }
                                            objModel.CustList = objdispList.CustComplaintList;
                                        }
                                    }

                                    //------------------End DispTrans-----------


                                    //------------------start CA-----------                                   

                                    CustComplaintModelsList CAList = new CustComplaintModelsList();
                                    CAList.CustComplaintList = new List<CustComplaintModels>();

                                    string Ssql = "select id_cust_nc_ca,id_custcomplaint_nc,ca_ca,ca_resource," +
                                    "ca_target_date,ca_resp_person from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc = '" + objComplaintModels.id_custcomplaint_nc + "' and ca_active=1";

                                    DataSet dsCALsit = objGlobaldata.Getdetails(Ssql);

                                    if (dsCALsit.Tables.Count > 0 && dsCALsit.Tables[0].Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dsCALsit.Tables[0].Rows.Count; i++)
                                        {
                                            try
                                            {
                                                CustComplaintModels objCustComplaintModels = new CustComplaintModels
                                                {
                                                    id_cust_nc_ca = (dsCALsit.Tables[0].Rows[i]["id_cust_nc_ca"].ToString()),
                                                    ca_ca = (dsCALsit.Tables[0].Rows[i]["ca_ca"].ToString()),
                                                    ca_resource = (dsCALsit.Tables[0].Rows[i]["ca_resource"].ToString()),
                                                    ca_resp_person = objGlobaldata.GetMultiHrEmpNameById(dsCALsit.Tables[0].Rows[i]["ca_resp_person"].ToString()),
                                                };
                                                if (DateTime.TryParse(dsCALsit.Tables[0].Rows[i]["ca_target_date"].ToString(), out dtValue))
                                                {
                                                    objCustComplaintModels.ca_target_date = dtValue;
                                                }
                                                CAList.CustComplaintList.Add(objCustComplaintModels);
                                            }
                                            catch (Exception ex)
                                            {
                                                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintDetails: " + ex.ToString());
                                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                            }
                                        }
                                    }
                                    objModel.CustList = CAList.CustComplaintList;

                                    //------------------End CA-----------


                                    //---------------Start Verification----------------
                                    CustComplaintModelsList objVeriList = new CustComplaintModelsList();
                                    objVeriList.CustComplaintList = new List<CustComplaintModels>();

                                    string Sql = "Select id_cust_nc_ca,ca_ca,ca_resource," +
                                   "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc = '" + objComplaintModels.id_custcomplaint_nc + "' and ca_active=1";

                                    DataSet dsCAModels = objGlobaldata.Getdetails(Sql);

                                    if (dsCAModels.Tables.Count > 0 && dsCAModels.Tables[0].Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dsCAModels.Tables[0].Rows.Count; i++)
                                        {
                                            try
                                            {
                                                CustComplaintModels objCAModel = new CustComplaintModels
                                                {
                                                    id_cust_nc_ca = (dsCAModels.Tables[0].Rows[i]["id_cust_nc_ca"].ToString()),
                                                    ca_ca = (dsCAModels.Tables[0].Rows[i]["ca_ca"].ToString()),
                                                    ca_resource = (dsCAModels.Tables[0].Rows[i]["ca_resource"].ToString()),
                                                    ca_resp_person = objGlobaldata.GetMultiHrEmpNameById(dsCAModels.Tables[0].Rows[i]["ca_resp_person"].ToString()),
                                                    implement_status = (dsCAModels.Tables[0].Rows[i]["implement_status"].ToString()),
                                                    ca_effective = (dsCAModels.Tables[0].Rows[i]["ca_effective"].ToString()),
                                                    reason = (dsCAModels.Tables[0].Rows[i]["reason"].ToString()),
                                                };

                                                if (DateTime.TryParse(dsCAModels.Tables[0].Rows[i]["ca_target_date"].ToString(), out dtValue))
                                                {
                                                    objCAModel.ca_target_date = dtValue;
                                                }
                                                objVeriList.CustComplaintList.Add(objCAModel);
                                            }
                                            catch (Exception Ex)
                                            {
                                                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintDetails: " + Ex.ToString());
                                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                            }
                                        }
                                        objModel.CustList = objVeriList.CustComplaintList;
                                    }
                                    //---------------End Verification------------------


                                    NcOverallList.CustComplaintList.Add(objModel);
                                }
                            }
                            ViewBag.NcOverallList = NcOverallList;
                        }


                        //For PDF Report
                        string sSqlstmtRpt = "select report_name,immediate_action,team,ca,rca,verification,review_assign,cust_response from t_pfd_report where report_name= 'Customer Complaint' and active=1";
                        DataSet RptPDF = objGlobaldata.Getdetails(sSqlstmtRpt);

                        if (RptPDF.Tables.Count > 0 && RptPDF.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.immediate_action = RptPDF.Tables[0].Rows[0]["immediate_action"].ToString();
                            ViewBag.team = RptPDF.Tables[0].Rows[0]["team"].ToString();
                            ViewBag.ca = RptPDF.Tables[0].Rows[0]["ca"].ToString();
                            ViewBag.rca = RptPDF.Tables[0].Rows[0]["rca"].ToString();
                            ViewBag.verification = RptPDF.Tables[0].Rows[0]["verification"].ToString();
                            ViewBag.review_assign = RptPDF.Tables[0].Rows[0]["review_assign"].ToString();
                            ViewBag.cust_response = RptPDF.Tables[0].Rows[0]["cust_response"].ToString();
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerComplaintList");
                    }

                    ViewBag.ComplaintDetail = objComplaintModels;
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("CustomerComplaintList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("CustomerComplaintToPDF")
            {
                FileName = "CustomerComplaint.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CustomerComplaintReport(FormCollection form)
        //{
        //    string sid_complaint = form["id_complaint"];
        //    CustComplaintModels objComplaintModels = new CustComplaintModels();
        //    try
        //    {
        //        if (sid_complaint != "")
        //        {

        //            string sSqlstmt = "select * from t_custcomplaint where id_complaint='" + sid_complaint + "'";

        //            DataSet dsComplaintList = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsComplaintList.Tables.Count > 0 && dsComplaintList.Tables[0].Rows.Count > 0)
        //            {
        //                objComplaintModels = new CustComplaintModels
        //                {
        //                    id_complaint = Convert.ToInt16(dsComplaintList.Tables[0].Rows[0]["id_complaint"].ToString()),
        //                    ComplaintNo = dsComplaintList.Tables[0].Rows[0]["ComplaintNo"].ToString(),
        //                    LoggedBy = objGlobaldata.GetEmpHrNameById(dsComplaintList.Tables[0].Rows[0]["LoggedBy"].ToString()),
        //                    CustomerName = objGlobaldata.GetCustomerNameById(dsComplaintList.Tables[0].Rows[0]["CustomerName"].ToString()),
        //                    ProjectName = dsComplaintList.Tables[0].Rows[0]["ProjectName"].ToString(),
        //                    ReportedBy = dsComplaintList.Tables[0].Rows[0]["ReportedBy"].ToString(),
        //                    ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsComplaintList.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
        //                    Details = dsComplaintList.Tables[0].Rows[0]["Details"].ToString(),
        //                    ForwardTo = objGlobaldata.GetMultiHrEmpNameById(dsComplaintList.Tables[0].Rows[0]["ForwardTo"].ToString()),
        //                    ComplaintStatus = objComplaintModels.GetComplaintStatusNameById(dsComplaintList.Tables[0].Rows[0]["ComplaintStatus"].ToString()),
        //                    Document = dsComplaintList.Tables[0].Rows[0]["Document"].ToString(),
        //                    ForwarderAssign = dsComplaintList.Tables[0].Rows[0]["ForwarderAssign"].ToString(),
        //                    CustomerRef = dsComplaintList.Tables[0].Rows[0]["CustomerRef"].ToString(),
        //                    reportedby_email = dsComplaintList.Tables[0].Rows[0]["reportedby_email"].ToString(),
        //                    reportedby_desig = dsComplaintList.Tables[0].Rows[0]["reportedby_desig"].ToString(),
        //                    reportedby_no = dsComplaintList.Tables[0].Rows[0]["reportedby_no"].ToString(),
        //                };

        //                DateTime dtDocDate = new DateTime();
        //                if (dsComplaintList.Tables[0].Rows[0]["LoggedDate"].ToString() != ""
        //                    && DateTime.TryParse(dsComplaintList.Tables[0].Rows[0]["LoggedDate"].ToString(), out dtDocDate))
        //                {
        //                    objComplaintModels.LoggedDate = dtDocDate;
        //                }

        //                if (dsComplaintList.Tables[0].Rows[0]["registered_on"].ToString() != ""
        //            && DateTime.TryParse(dsComplaintList.Tables[0].Rows[0]["registered_on"].ToString(), out dtDocDate))
        //                {
        //                    objComplaintModels.registered_on = dtDocDate;
        //                }

        //                if (dsComplaintList.Tables[0].Rows[0]["ReceivedDate"].ToString() != ""
        //              && DateTime.TryParse(dsComplaintList.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtDocDate))
        //                {
        //                    objComplaintModels.ReceivedDate = dtDocDate;
        //                }

        //                CompanyModels objCompany = new CompanyModels();
        //                dsComplaintList = objCompany.GetCompanyDetailsForReport(dsComplaintList);

        //                dsComplaintList = objGlobaldata.GetReportDetails(dsComplaintList, objComplaintModels.ComplaintNo, dsComplaintList.Tables[0].Rows[0]["LoggedBy"].ToString(), "CUSTOMER COMPLAINT REPORT");
        //                ViewBag.CompanyInfo = dsComplaintList;

        //                string sql = "Select branch,Department,Location from t_customer_info where CustID = '" + dsComplaintList.Tables[0].Rows[0]["CustomerName"].ToString() + "'";
        //                DataSet CustList = objGlobaldata.Getdetails(sql);
        //                if (CustList.Tables.Count > 0 && CustList.Tables[0].Rows.Count > 0)
        //                {
        //                    objComplaintModels.branch = objGlobaldata.GetMultiCompanyBranchNameById(CustList.Tables[0].Rows[0]["branch"].ToString());
        //                    objComplaintModels.Department = objGlobaldata.GetMultiDeptNameById(CustList.Tables[0].Rows[0]["Department"].ToString());
        //                    objComplaintModels.Location = objGlobaldata.GetDivisionLocationById(CustList.Tables[0].Rows[0]["Location"].ToString());
        //                }

        //                ViewBag.ComplaintDetail = objComplaintModels;

        //                //sSqlstmt = "select AssignedBy,ActionedBy,AssignDate,TargetDate,ReasonForProblem,ImmediateAction,"
        //                //            + "ImmediateAction_TargetDate,CorrectiveAction,CorrectiveAction_TargetDate from t_custcomplaint_assign,t_custcomplaint_action"
        //                //            + " where t_custcomplaint_assign.id_complaint_assign=t_custcomplaint_action.id_complaint_assign"
        //                //            + " and t_custcomplaint_assign.id_complaint='" + sid_complaint + "'";
        //                //DataSet dsItems = new DataSet();
        //                //dsItems = objGlobaldata.Getdetails(sSqlstmt);
        //                //ViewBag.ComplaintActionDetails = dsItems;

        //            }
        //            else
        //            {
        //                TempData["alertdata"] = "No data exists";

        //            }
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = "Id cannot be null";

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintReport: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

        //    foreach (var key in Request.Cookies.AllKeys)
        //    {
        //        cookieCollection.Add(key, Request.Cookies.Get(key).Value);
        //    }
        //    string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

        //    return new ViewAsPdf("CustomerComplaintToPDF")
        //    {
        //        FileName = "CustomerComplaint.pdf",
        //        Cookies = cookieCollection,
        //        CustomSwitches = footer
        //    };

        //}

        [AllowAnonymous]
        public JsonResult CustComplaintDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_complaint"] != null && form["id_complaint"] != "")
                {
                    CustComplaintModels Doc = new CustComplaintModels();
                    string sid_complaint = form["id_complaint"];


                    if (Doc.FunDeleteCustComplaintDoc(sid_complaint))
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
                    TempData["alertdata"] = "Customer complaint Id cannot be Null or empty";
                    return Json("Failed");
                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustComplaintDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }


        [AllowAnonymous]
        public ActionResult AssignCustomerComplaint()
        {
            ViewBag.SubMenutype = "CustomerComplaint";
            CustComplaintModels objComplaintModels = new CustComplaintModels();

            try
            {

                //ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                ViewBag.DeptList = objGlobaldata.GetCompanyBranchListbox();               
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.Deviation = objGlobaldata.GetDropdownList("Customer Complaint Deviation");

                UserCredentials objUser = new UserCredentials();

                objUser = objGlobaldata.GetCurrentUserSession();



                if (Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    string sid_complaint = Request.QueryString["id_complaint"];

                    string sSqlstmt = "select id_complaint,ComplaintNo,LoggedDate,LoggedBy,CustomerName,ProjectName,ReceivedDate,ReportedBy,ModeOfComplaint,"
                    + "Details,ForwardTo,ComplaintStatus,Document,ForwarderAssign,divisionId," +
                    "TargetDate,complain_review_date,complaint_valid,complaint_deviation,complaint_remark,rej_reason,rej_upload from t_custcomplaint where id_complaint='" + sid_complaint + "'";

                    DataSet dsComplaintList = objGlobaldata.Getdetails(sSqlstmt);


                    if (dsComplaintList.Tables.Count > 0 && dsComplaintList.Tables[0].Rows.Count > 0)
                    {

                        if (dsComplaintList.Tables[0].Rows[0]["ForwardTo"].ToString().Contains(objUser.empid) && !(dsComplaintList.Tables[0].Rows[0]["ForwarderAssign"].ToString().Contains(objUser.empid)))
                        {
                            objComplaintModels = new CustComplaintModels
                            {
                                id_complaint = Convert.ToInt16(dsComplaintList.Tables[0].Rows[0]["id_complaint"].ToString()),
                                ComplaintNo = dsComplaintList.Tables[0].Rows[0]["ComplaintNo"].ToString(),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsComplaintList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                                CustomerName = objGlobaldata.GetCustomerNameById(dsComplaintList.Tables[0].Rows[0]["CustomerName"].ToString()),
                                ProjectName = dsComplaintList.Tables[0].Rows[0]["ProjectName"].ToString(),
                                ReportedBy = dsComplaintList.Tables[0].Rows[0]["ReportedBy"].ToString(),
                                ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsComplaintList.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                                Details = dsComplaintList.Tables[0].Rows[0]["Details"].ToString(),
                                ForwardTo = objGlobaldata.GetMultiHrEmpNameById(dsComplaintList.Tables[0].Rows[0]["ForwardTo"].ToString()),
                                ComplaintStatus = dsComplaintList.Tables[0].Rows[0]["ComplaintStatus"].ToString(),
                                Document = dsComplaintList.Tables[0].Rows[0]["Document"].ToString(),

                                complaint_valid = dsComplaintList.Tables[0].Rows[0]["complaint_valid"].ToString(),
                                complaint_deviation =(dsComplaintList.Tables[0].Rows[0]["complaint_deviation"].ToString()),
                                complaint_remark = dsComplaintList.Tables[0].Rows[0]["complaint_remark"].ToString(),
                                rej_reason = dsComplaintList.Tables[0].Rows[0]["rej_reason"].ToString(),
                                rej_upload = dsComplaintList.Tables[0].Rows[0]["rej_upload"].ToString(),
                            };
                            DateTime dtDocDate = new DateTime();
                            if (dsComplaintList.Tables[0].Rows[0]["LoggedDate"].ToString() != ""
                                && DateTime.TryParse(dsComplaintList.Tables[0].Rows[0]["LoggedDate"].ToString(), out dtDocDate))
                            {
                                objComplaintModels.LoggedDate = dtDocDate;
                            }

                            if (dsComplaintList.Tables[0].Rows[0]["ReceivedDate"].ToString() != ""
                            && DateTime.TryParse(dsComplaintList.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtDocDate))
                            {
                                objComplaintModels.ReceivedDate = dtDocDate;
                            }

                            if (dsComplaintList.Tables[0].Rows[0]["TargetDate"].ToString() != ""
                           && DateTime.TryParse(dsComplaintList.Tables[0].Rows[0]["TargetDate"].ToString(), out dtDocDate))
                            {
                                objComplaintModels.TargetDate = dtDocDate;
                            }

                            if (dsComplaintList.Tables[0].Rows[0]["complain_review_date"].ToString() != ""
                           && DateTime.TryParse(dsComplaintList.Tables[0].Rows[0]["complain_review_date"].ToString(), out dtDocDate))
                            {
                                objComplaintModels.complain_review_date = dtDocDate;
                            }

                            ViewBag.EmpList = objGlobaldata.GetHrEmpListByDivision(dsComplaintList.Tables[0].Rows[0]["divisionId"].ToString());
                        }
                        else
                        {
                            ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);

                            TempData["alertdata"] = "Access Denied";
                            return RedirectToAction("CustomerComplaintList");
                        }

                    }
                    else
                    {
                        ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);

                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("CustomerComplaintList");
                    }
                }
                else
                {

                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);

                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("CustomerComplaintList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AssignCustomerComplaint: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("CustomerComplaintList");
            }

            return View(objComplaintModels);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult AssignCustomerComplaint(CustComplaintModels objCustomerCompliant, FormCollection form, IEnumerable<HttpPostedFileBase> rej_upload)
        {
            try
            {
                objCustomerCompliant.AssignedTo = form["AssignTo"];
                objCustomerCompliant.divisionId = form["DeptId"];               

                DateTime dateValue;

                if (DateTime.TryParse(form["TargetDate"], out dateValue) == true)
                {
                    objCustomerCompliant.TargetDate = dateValue;
                }

                if (DateTime.TryParse(form["complain_review_date"], out dateValue) == true)
                {
                    objCustomerCompliant.complain_review_date = dateValue;
                }

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];
                
                if (rej_upload != null && files.ContentLength > 0)
                {
                    objCustomerCompliant.rej_upload = "";
                    foreach (var document in rej_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustComplaint"), Path.GetFileName(document.FileName));
                            string sFilename = "CC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            document.SaveAs(sFilepath + "/" + sFilename);
                            objCustomerCompliant.rej_upload = objCustomerCompliant.rej_upload + "," + "~/DataUpload/MgmtDocs/CustComplaint/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objCustomerCompliant.rej_upload = objCustomerCompliant.rej_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objCustomerCompliant.rej_upload = objCustomerCompliant.rej_upload + "," + form["QCDocsVal"];
                    objCustomerCompliant.rej_upload = objCustomerCompliant.rej_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objCustomerCompliant.rej_upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objCustomerCompliant.rej_upload = null;
                }


                if (objCustomerCompliant.FunAssignCustomerComplaint(objCustomerCompliant))
                {
                    TempData["Successdata"] = "Customer complaint Assigned successfully";                    
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AssignCustomerComplaint: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerComplaintList");
        }


        //Customer Complaint Response
        [AllowAnonymous]
        public ActionResult CustomerComplaintResponse()
        {
            CustComplaintModels objModel = new CustComplaintModels();
            try
            {
                if (Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    objModel.id_complaint = Convert.ToInt32(Request.QueryString["id_complaint"]);
                    
                    ViewBag.CustomerResponse = objGlobaldata.GetDropdownList("Customer Complaint Response");
                    
                    string sSqlstmt = "select id_complaint,ForwarderAssign,TargetDate,ComplaintNo,CustomerName,CustomerRef,ProjectName,ReceivedDate,ModeOfComplaint,ReportedBy,reportedby_email,reportedby_no,reportedby_desig,registered_on," +
                        "c_response,c_response_details,c_reponse_upload,c_response_date from t_custcomplaint where id_complaint='" + objModel.id_complaint + "'";
                    DataSet dsCustComplaintModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustComplaintModels.Tables.Count > 0 && dsCustComplaintModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new CustComplaintModels
                        {
                            id_complaint = Convert.ToInt32(dsCustComplaintModels.Tables[0].Rows[0]["id_complaint"].ToString()),
                            ComplaintNo = (dsCustComplaintModels.Tables[0].Rows[0]["ComplaintNo"].ToString()),
                            CustomerRef = (dsCustComplaintModels.Tables[0].Rows[0]["CustomerRef"].ToString()),
                            CustomerName = objGlobaldata.GetCustomerNameById(dsCustComplaintModels.Tables[0].Rows[0]["CustomerName"].ToString()),
                            ProjectName = (dsCustComplaintModels.Tables[0].Rows[0]["ProjectName"].ToString()),
                            ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsCustComplaintModels.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                            ReportedBy = (dsCustComplaintModels.Tables[0].Rows[0]["ReportedBy"].ToString()),
                            reportedby_email = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_email"].ToString()),
                            reportedby_no = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_no"].ToString()),
                            reportedby_desig = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_desig"].ToString()),

                            c_response = (dsCustComplaintModels.Tables[0].Rows[0]["c_response"].ToString()),
                            c_response_details = (dsCustComplaintModels.Tables[0].Rows[0]["c_response_details"].ToString()),
                            c_reponse_upload = (dsCustComplaintModels.Tables[0].Rows[0]["c_reponse_upload"].ToString()),
                        };
                       
                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtValue))
                        {
                            objModel.ReceivedDate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["registered_on"].ToString(), out dtValue))
                        {
                            objModel.registered_on = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["c_response_date"].ToString(), out dtValue))
                        {
                            objModel.c_response_date = dtValue;
                        }

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerComplaintList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintResponse: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerComplaintList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CustomerComplaintResponse(CustComplaintModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> c_reponse_upload)
        {
            try
            {
                CustComplaintModelsList objModelList = new CustComplaintModelsList();
                objModelList.CustComplaintList = new List<CustComplaintModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["c_response_date"], out dateValue) == true)
                {
                    objModel.c_response_date = dateValue;
                }
                                             
                IList<HttpPostedFileBase> c_reponse_uploadList = (IList<HttpPostedFileBase>)c_reponse_upload;
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (c_reponse_uploadList[0] != null)
                {
                    objModel.c_reponse_upload = "";
                    foreach (var file in c_reponse_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustComplaint"), Path.GetFileName(file.FileName));
                            string sFilename = "CC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.c_reponse_upload = objModel.c_reponse_upload + "," + "~/DataUpload/MgmtDocs/CustComplaint/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintResponse-upload: " + ex.ToString());
                        }
                    }
                    objModel.c_reponse_upload = objModel.c_reponse_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.c_reponse_upload = objModel.c_reponse_upload + "," + form["QCDocsVal"];
                    objModel.c_reponse_upload = objModel.c_reponse_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && c_reponse_uploadList[0] == null)
                {
                    objModel.c_reponse_upload = null;
                }
                else if (form["QCDocsVal"] == null && c_reponse_uploadList[0] == null)
                {
                    objModel.c_reponse_upload = null;
                }

                if (objModel.FunUpdateCustomerResponse(objModel))
                {
                    TempData["Successdata"] = "Updated customer response successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintResponse: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerComplaintList");
        }


        
        //Pdf report format
        [AllowAnonymous]
        public ActionResult CustComplaintPDFReportFormat()
        {
            ReportPDFModels objModel = new ReportPDFModels();
            try
            {
                if (Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    ViewBag.id_complaint = Request.QueryString["id_complaint"];

                    //ViewBag.YesNo = objGlobaldata.GetConstantValueKeyValuePair("YesNo");

                    string sSqlstmt = "select report_name,immediate_action,team,ca,rca,verification,review_assign,cust_response from t_pfd_report where report_name= 'Customer Complaint' and active=1";
                    DataSet dsCustComplaintModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustComplaintModels.Tables.Count > 0 && dsCustComplaintModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new ReportPDFModels
                        {
                            report_name = (dsCustComplaintModels.Tables[0].Rows[0]["report_name"].ToString()),
                            immediate_action = (dsCustComplaintModels.Tables[0].Rows[0]["immediate_action"].ToString()),
                            team = (dsCustComplaintModels.Tables[0].Rows[0]["team"].ToString()),
                            ca = (dsCustComplaintModels.Tables[0].Rows[0]["ca"].ToString()),
                            rca = (dsCustComplaintModels.Tables[0].Rows[0]["rca"].ToString()),
                            verification = (dsCustComplaintModels.Tables[0].Rows[0]["verification"].ToString()),
                            review_assign = (dsCustComplaintModels.Tables[0].Rows[0]["review_assign"].ToString()),
                            cust_response = (dsCustComplaintModels.Tables[0].Rows[0]["cust_response"].ToString()),
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
                objGlobaldata.AddFunctionalLog("Exception in CustComplaintPDFReportFormat: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerComplaintDetails",new { id_complaint= objModel.id_complaint });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CustComplaintPDFReportFormat(ReportPDFModels objModel)
        {
            ReportPDFModels objRptModel = new ReportPDFModels();
            try
            {                
                if (objModel.FunUpdatePDFReportFormat(objModel, "Customer Complaint"))
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
                objGlobaldata.AddFunctionalLog("Exception in CustComplaintPDFReportFormat: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            //return RedirectToAction("CustomerComplaintDetails", new { id_complaint = objModel.id_complaint });
            return RedirectToAction("CustomerComplaintReport", new { id_complaint = objModel.id_complaint });
        }


        public JsonResult GetCustomerDetails(string CustId)
        {
            CustComplaintModels objCust = new CustComplaintModels();
            string sql = "Select branch,Department,Location from t_customer_info where CustID = '" + CustId + "'";
            DataSet CustList = objGlobaldata.Getdetails(sql);
            if (CustList.Tables.Count > 0 && CustList.Tables[0].Rows.Count > 0)
            {
                objCust = new CustComplaintModels
                {
                    branch = objGlobaldata.GetMultiCompanyBranchNameById( CustList.Tables[0].Rows[0]["branch"].ToString()),
                    Department = objGlobaldata.GetMultiDeptNameById(CustList.Tables[0].Rows[0]["Department"].ToString()),
                    Location = objGlobaldata.GetDivisionLocationById(CustList.Tables[0].Rows[0]["Location"].ToString()),
                };
            }
            return Json(objCust);
        }


        //CustComplaint NC

        [AllowAnonymous]
        public ActionResult CustomerComplaintView(FormCollection form)
        {
            CustComplaintModels objComplaintModels = new CustComplaintModels();

            try
            {
                UserCredentials objUser = new UserCredentials();

                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.user = objGlobaldata.GetEmpHrNameById(objUser.empid);


                ViewBag.Complaint_Status = objGlobaldata.GetDropdownList("Complaint Status");

                if (Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    string sid_complaint = Request.QueryString["id_complaint"];

                    string sSqlstmt = "select *  from t_custcomplaint where id_complaint='" + sid_complaint + "'";

                    DataSet dsComplaintModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsComplaintModelsList.Tables.Count > 0 && dsComplaintModelsList.Tables[0].Rows.Count > 0)
                    {
                        objComplaintModels = new CustComplaintModels
                        {
                            id_complaint = Convert.ToInt16(dsComplaintModelsList.Tables[0].Rows[0]["id_complaint"].ToString()),
                            ComplaintNo = dsComplaintModelsList.Tables[0].Rows[0]["ComplaintNo"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsComplaintModelsList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            CustomerName = objGlobaldata.GetCustomerNameById(dsComplaintModelsList.Tables[0].Rows[0]["CustomerName"].ToString()),
                            ProjectName = dsComplaintModelsList.Tables[0].Rows[0]["ProjectName"].ToString(),
                            ReportedBy = dsComplaintModelsList.Tables[0].Rows[0]["ReportedBy"].ToString(),
                            ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsComplaintModelsList.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                            Details = dsComplaintModelsList.Tables[0].Rows[0]["Details"].ToString(),
                            ForwardTo = objGlobaldata.GetMultiHrEmpNameById(dsComplaintModelsList.Tables[0].Rows[0]["ForwardTo"].ToString()),
                            ComplaintStatus = objGlobaldata.GetDropdownitemById(dsComplaintModelsList.Tables[0].Rows[0]["ComplaintStatus"].ToString()),
                            Document = dsComplaintModelsList.Tables[0].Rows[0]["Document"].ToString(),
                            ForwarderAssign = dsComplaintModelsList.Tables[0].Rows[0]["ForwarderAssign"].ToString(),
                            CustomerRef = dsComplaintModelsList.Tables[0].Rows[0]["CustomerRef"].ToString(),
                            reportedby_email = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_email"].ToString(),
                            reportedby_desig = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_desig"].ToString(),
                            reportedby_no = dsComplaintModelsList.Tables[0].Rows[0]["reportedby_no"].ToString(),
                        };

                        DateTime dtDocDate = new DateTime();
                        if (dsComplaintModelsList.Tables[0].Rows[0]["LoggedDate"].ToString() != ""
                            && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["LoggedDate"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.LoggedDate = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["registered_on"].ToString() != ""
                      && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["registered_on"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.registered_on = dtDocDate;
                        }

                        if (dsComplaintModelsList.Tables[0].Rows[0]["ReceivedDate"].ToString() != ""
                    && DateTime.TryParse(dsComplaintModelsList.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtDocDate))
                        {
                            objComplaintModels.ReceivedDate = dtDocDate;
                        }

                        string sql = "Select branch,Department,Location from t_customer_info where CustID = '" + dsComplaintModelsList.Tables[0].Rows[0]["CustomerName"].ToString() + "'";
                        DataSet CustList = objGlobaldata.Getdetails(sql);
                        if (CustList.Tables.Count > 0 && CustList.Tables[0].Rows.Count > 0)
                        {
                            objComplaintModels.branch = objGlobaldata.GetMultiCompanyBranchNameById(CustList.Tables[0].Rows[0]["branch"].ToString());
                            objComplaintModels.Department = objGlobaldata.GetMultiDeptNameById(CustList.Tables[0].Rows[0]["Department"].ToString());
                            objComplaintModels.Location = objGlobaldata.GetDivisionLocationById(CustList.Tables[0].Rows[0]["Location"].ToString());
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerComplaintList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("CustomerComplaintList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerComplaintDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objComplaintModels);
        }

        //Disposition
        [AllowAnonymous]
        public ActionResult AddDisposition()
        {
            CustComplaintModels objModel = new CustComplaintModels();
            try
            {
                if (Request.QueryString["id_custcomplaint_nc"] != null && Request.QueryString["id_custcomplaint_nc"] != "" && Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    string sid_custcomplaint_nc = Request.QueryString["id_custcomplaint_nc"];
                    ViewBag.id_custcomplaint_nc = sid_custcomplaint_nc;
                    string sid_complaint = Request.QueryString["id_complaint"];

                    ViewBag.DispositonAction = objGlobaldata.GetDropdownList("NC Disposition Action");
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                    string sSqlstmt = "select id_custcomplaint_nc,a.id_complaint,b.ForwarderAssign,a.TargetDate,ComplaintNo,CustomerName,CustomerRef,ProjectName,ReceivedDate,ModeOfComplaint,ReportedBy,reportedby_email,reportedby_no,reportedby_desig,registered_on," +
                        "nc_no,disp_action_taken,disp_explain,disp_notifiedto,disp_notifeddate,disp_upload from t_custcomplaint a, t_custcomplaint_nc b where id_custcomplaint_nc='" + sid_custcomplaint_nc + "' and a.id_complaint= b.id_complaint and a.id_complaint='"+ sid_complaint + "'";
                    DataSet dsCustComplaintModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustComplaintModels.Tables.Count > 0 && dsCustComplaintModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new CustComplaintModels
                        {
                            id_custcomplaint_nc = (dsCustComplaintModels.Tables[0].Rows[0]["id_custcomplaint_nc"].ToString()),
                            id_complaint = Convert.ToInt32(dsCustComplaintModels.Tables[0].Rows[0]["id_complaint"].ToString()),
                            ComplaintNo =(dsCustComplaintModels.Tables[0].Rows[0]["ComplaintNo"].ToString()),
                            CustomerRef = (dsCustComplaintModels.Tables[0].Rows[0]["CustomerRef"].ToString()),
                            CustomerName = objGlobaldata.GetCustomerNameById(dsCustComplaintModels.Tables[0].Rows[0]["CustomerName"].ToString()),
                            ProjectName = (dsCustComplaintModels.Tables[0].Rows[0]["ProjectName"].ToString()),
                            ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsCustComplaintModels.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                            ReportedBy= (dsCustComplaintModels.Tables[0].Rows[0]["ReportedBy"].ToString()),
                            reportedby_email = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_email"].ToString()),
                            reportedby_no = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_no"].ToString()),
                            reportedby_desig = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_desig"].ToString()),
                            nc_no = dsCustComplaintModels.Tables[0].Rows[0]["nc_no"].ToString(),

                            disp_action_taken = (dsCustComplaintModels.Tables[0].Rows[0]["disp_action_taken"].ToString()),
                            disp_explain = (dsCustComplaintModels.Tables[0].Rows[0]["disp_explain"].ToString()),
                            disp_notifiedto = (dsCustComplaintModels.Tables[0].Rows[0]["disp_notifiedto"].ToString()),
                            disp_upload = (dsCustComplaintModels.Tables[0].Rows[0]["disp_upload"].ToString()),
                        };
                        if (dsCustComplaintModels.Tables[0].Rows[0]["disp_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsCustComplaintModels.Tables[0].Rows[0]["disp_notifiedto"].ToString()).Split(',');
                        }
                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtValue))
                        {
                            objModel.ReceivedDate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["registered_on"].ToString(), out dtValue))
                        {
                            objModel.registered_on = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["disp_notifeddate"].ToString(), out dtValue))
                        {
                            objModel.disp_notifeddate = dtValue;
                        }


                        CustComplaintModelsList NcDispList = new CustComplaintModelsList();
                        NcDispList.CustComplaintList = new List<CustComplaintModels>();

                        string sSqlstmt1 = "select id_cust_nc_disp_action,disp_action,disp_resp_person,disp_complete_date from t_custcomplaint_nc_disp_action where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                        DataSet dsDispModels = objGlobaldata.Getdetails(sSqlstmt1);

                        if (dsDispModels.Tables.Count > 0 && dsDispModels.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsDispModels.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    CustComplaintModels objDispModel = new CustComplaintModels
                                    {
                                        id_cust_nc_disp_action = (dsDispModels.Tables[0].Rows[i]["id_cust_nc_disp_action"].ToString()),
                                        disp_action = (dsDispModels.Tables[0].Rows[i]["disp_action"].ToString()),
                                        disp_resp_person = (dsDispModels.Tables[0].Rows[i]["disp_resp_person"].ToString()),
                                    };

                                    if (DateTime.TryParse(dsDispModels.Tables[0].Rows[i]["disp_complete_date"].ToString(), out dtValue))
                                    {
                                        objDispModel.disp_complete_date = dtValue;
                                    }
                                    NcDispList.CustComplaintList.Add(objDispModel);
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
                        return RedirectToAction("CustomerComplaintList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerComplaintList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddDisposition(CustComplaintModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> disp_upload)
        {
            try
            {
                CustComplaintModelsList objModelList = new CustComplaintModelsList();
                objModelList.CustComplaintList = new List<CustComplaintModels>();

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
                    CustComplaintModels objNCModel = new CustComplaintModels();
                    if (form["disp_action " + i] != "" && form["disp_action " + i] != null)
                    {
                        objNCModel.disp_action = form["disp_action " + i];
                        objNCModel.disp_resp_person = form["disp_resp_person " + i];
                        if (DateTime.TryParse(form["disp_complete_date " + i], out dateValue) == true)
                        {
                            objNCModel.disp_complete_date = dateValue;
                        }
                        objModelList.CustComplaintList.Add(objNCModel);
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
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustComplaint"), Path.GetFileName(file.FileName));
                            string sFilename = "CC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.disp_upload = objModel.disp_upload + "," + "~/DataUpload/MgmtDocs/CustComplaint/" + sFilename;
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
            CustComplaintModels objModel = new CustComplaintModels();
            try
            {
                if (Request.QueryString["id_custcomplaint_nc"] != null && Request.QueryString["id_custcomplaint_nc"] != "" && Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    string sid_custcomplaint_nc = Request.QueryString["id_custcomplaint_nc"];
                    ViewBag.id_custcomplaint_nc = sid_custcomplaint_nc;
                    string sid_complaint = Request.QueryString["id_complaint"];

                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                    string sSqlstmt = "select id_custcomplaint_nc,a.id_complaint,b.ForwarderAssign,a.TargetDate,ComplaintNo,CustomerName,CustomerRef,ProjectName,ReceivedDate,ModeOfComplaint,ReportedBy,reportedby_email,reportedby_no,reportedby_desig,registered_on," +
                     "nc_no,nc_team,team_targetdate from t_custcomplaint a, t_custcomplaint_nc b where id_custcomplaint_nc='" + sid_custcomplaint_nc + "' and a.id_complaint= b.id_complaint and a.id_complaint='" + sid_complaint + "'";

                   DataSet dsCustComplaintModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustComplaintModels.Tables.Count > 0 && dsCustComplaintModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new CustComplaintModels
                        {
                            id_custcomplaint_nc = (dsCustComplaintModels.Tables[0].Rows[0]["id_custcomplaint_nc"].ToString()),
                            id_complaint = Convert.ToInt32(dsCustComplaintModels.Tables[0].Rows[0]["id_complaint"].ToString()),
                            ComplaintNo = (dsCustComplaintModels.Tables[0].Rows[0]["ComplaintNo"].ToString()),
                            CustomerName = objGlobaldata.GetCustomerNameById(dsCustComplaintModels.Tables[0].Rows[0]["CustomerName"].ToString()),
                            CustomerRef = (dsCustComplaintModels.Tables[0].Rows[0]["CustomerRef"].ToString()),
                            ProjectName = (dsCustComplaintModels.Tables[0].Rows[0]["ProjectName"].ToString()),
                            ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsCustComplaintModels.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                            ReportedBy = (dsCustComplaintModels.Tables[0].Rows[0]["ReportedBy"].ToString()),
                            reportedby_email = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_email"].ToString()),
                            reportedby_desig = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_desig"].ToString()),
                            reportedby_no = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_no"].ToString()),
                            nc_no = dsCustComplaintModels.Tables[0].Rows[0]["nc_no"].ToString(),
                        };
                        if (dsCustComplaintModels.Tables[0].Rows[0]["nc_team"].ToString() != "")
                        {
                            ViewBag.TeamArray = (dsCustComplaintModels.Tables[0].Rows[0]["nc_team"].ToString()).Split(',');
                        }                        

                        DateTime dtValue;

                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtValue))
                        {
                            objModel.ReceivedDate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["registered_on"].ToString(), out dtValue))
                        {
                            objModel.registered_on = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["team_targetdate"].ToString(), out dtValue))
                        {
                            objModel.team_targetdate = dtValue;
                        }
                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerComplaintList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTeam: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerComplaintList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddTeam(CustComplaintModels objModel, FormCollection form)
        {
            try
            {
              
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
            CustComplaintModels objModel = new CustComplaintModels();
            try
            {
                if (Request.QueryString["id_custcomplaint_nc"] != null && Request.QueryString["id_custcomplaint_nc"] != "" && Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    string sid_custcomplaint_nc = Request.QueryString["id_custcomplaint_nc"];
                    ViewBag.id_custcomplaint_nc = sid_custcomplaint_nc;
                    string sid_complaint = Request.QueryString["id_complaint"];

                    ViewBag.RCATechniqueList = objGlobaldata.GetDropdownList("RCA Technique");
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");

                    string sSqlstmt = "select id_custcomplaint_nc,a.id_complaint,b.ForwarderAssign,a.TargetDate,ComplaintNo,CustomerName,CustomerRef,ProjectName,ReceivedDate,ModeOfComplaint,ReportedBy,reportedby_email,reportedby_no,reportedby_desig,registered_on," +
                                   "nc_no,rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate,rca_startdate from t_custcomplaint a, t_custcomplaint_nc b where id_custcomplaint_nc='" + sid_custcomplaint_nc + "' and a.id_complaint= b.id_complaint and a.id_complaint='" + sid_complaint + "'";
                    DataSet dsCustComplaintModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustComplaintModels.Tables.Count > 0 && dsCustComplaintModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new CustComplaintModels
                        {
                            id_custcomplaint_nc = (dsCustComplaintModels.Tables[0].Rows[0]["id_custcomplaint_nc"].ToString()),
                            id_complaint = Convert.ToInt32(dsCustComplaintModels.Tables[0].Rows[0]["id_complaint"].ToString()),
                            ComplaintNo = (dsCustComplaintModels.Tables[0].Rows[0]["ComplaintNo"].ToString()),
                            CustomerName = objGlobaldata.GetCustomerNameById(dsCustComplaintModels.Tables[0].Rows[0]["CustomerName"].ToString()),
                            CustomerRef = (dsCustComplaintModels.Tables[0].Rows[0]["CustomerRef"].ToString()),
                            ProjectName = (dsCustComplaintModels.Tables[0].Rows[0]["ProjectName"].ToString()),
                            ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsCustComplaintModels.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                            ReportedBy = (dsCustComplaintModels.Tables[0].Rows[0]["ReportedBy"].ToString()),
                            reportedby_email = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_email"].ToString()),
                            reportedby_no = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_no"].ToString()),
                            reportedby_desig = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_desig"].ToString()),
                            nc_no = dsCustComplaintModels.Tables[0].Rows[0]["nc_no"].ToString(),

                            rca_technique = (dsCustComplaintModels.Tables[0].Rows[0]["rca_technique"].ToString()),
                            rca_details = (dsCustComplaintModels.Tables[0].Rows[0]["rca_details"].ToString()),
                            rca_upload = (dsCustComplaintModels.Tables[0].Rows[0]["rca_upload"].ToString()),
                            rca_action = (dsCustComplaintModels.Tables[0].Rows[0]["rca_action"].ToString()),
                            rca_justify = (dsCustComplaintModels.Tables[0].Rows[0]["rca_justify"].ToString()),
                            rca_reportedby = (dsCustComplaintModels.Tables[0].Rows[0]["rca_reportedby"].ToString()),
                            rca_notifiedto = (dsCustComplaintModels.Tables[0].Rows[0]["rca_notifiedto"].ToString()),
                        };

                        if (dsCustComplaintModels.Tables[0].Rows[0]["rca_reportedby"].ToString() != "")
                        {
                            ViewBag.ReportedByArray = (dsCustComplaintModels.Tables[0].Rows[0]["rca_reportedby"].ToString()).Split(',');
                        }
                        else
                        {
                            ViewBag.ReportedByArray = (objGlobaldata.GetCurrentUserSession().empid).Split(',');
                        }

                        if (dsCustComplaintModels.Tables[0].Rows[0]["rca_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsCustComplaintModels.Tables[0].Rows[0]["rca_notifiedto"].ToString()).Split(',');
                        }

                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtValue))
                        {
                            objModel.ReceivedDate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["registered_on"].ToString(), out dtValue))
                        {
                            objModel.registered_on = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["rca_startdate"].ToString(), out dtValue))
                        {
                            objModel.rca_startdate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["rca_reporteddate"].ToString(), out dtValue))
                        {
                            objModel.rca_reporteddate = dtValue;
                        }

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerComplaintList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRCA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerComplaintList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddRCA(CustComplaintModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> rca_upload)
        {
            try
            {
                CustComplaintModelsList objModelList = new CustComplaintModelsList();
                objModelList.CustComplaintList = new List<CustComplaintModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["team_targetdate"], out dateValue) == true)
                {
                    objModel.team_targetdate = dateValue;
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
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustComplaint"), Path.GetFileName(file.FileName));
                            string sFilename = "CC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.rca_upload = objModel.rca_upload + "," + "~/DataUpload/MgmtDocs/CustComplaint/" + sFilename;
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
            CustComplaintModels objModel = new CustComplaintModels();
            try
            {
                if (Request.QueryString["id_custcomplaint_nc"] != null && Request.QueryString["id_custcomplaint_nc"] != "" && Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    string sid_custcomplaint_nc = Request.QueryString["id_custcomplaint_nc"];
                    string sid_complaint = Request.QueryString["id_complaint"];
                   
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                  
                    string sSqlstmt = "select id_custcomplaint_nc,a.id_complaint,b.ForwarderAssign,a.TargetDate,ComplaintNo,CustomerName,CustomerRef,ProjectName,ReceivedDate,ModeOfComplaint,ReportedBy,reportedby_email,reportedby_no,reportedby_desig,registered_on,rca_details," +
                                   "nc_no,ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date from t_custcomplaint a, t_custcomplaint_nc b where id_custcomplaint_nc='" + sid_custcomplaint_nc + "' and a.id_complaint= b.id_complaint and a.id_complaint='" + sid_complaint + "'";
                    DataSet dsCustComplaintModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustComplaintModels.Tables.Count > 0 && dsCustComplaintModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new CustComplaintModels
                        {
                            id_custcomplaint_nc = (dsCustComplaintModels.Tables[0].Rows[0]["id_custcomplaint_nc"].ToString()),
                            id_complaint = Convert.ToInt32(dsCustComplaintModels.Tables[0].Rows[0]["id_complaint"].ToString()),
                            ComplaintNo = (dsCustComplaintModels.Tables[0].Rows[0]["ComplaintNo"].ToString()),
                            CustomerName = objGlobaldata.GetCustomerNameById(dsCustComplaintModels.Tables[0].Rows[0]["CustomerName"].ToString()),
                            CustomerRef = (dsCustComplaintModels.Tables[0].Rows[0]["CustomerRef"].ToString()),
                            ProjectName = (dsCustComplaintModels.Tables[0].Rows[0]["ProjectName"].ToString()),
                            ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsCustComplaintModels.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                            ReportedBy = (dsCustComplaintModels.Tables[0].Rows[0]["ReportedBy"].ToString()),
                            reportedby_email = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_email"].ToString()),
                            reportedby_no = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_no"].ToString()),
                            reportedby_desig = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_desig"].ToString()),
                            nc_no = dsCustComplaintModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            rca_details = (dsCustComplaintModels.Tables[0].Rows[0]["rca_details"].ToString()),

                            ca_proposed_by = (dsCustComplaintModels.Tables[0].Rows[0]["ca_proposed_by"].ToString()),
                            ca_notifiedto = (dsCustComplaintModels.Tables[0].Rows[0]["ca_notifiedto"].ToString()),
                        };
                        if (dsCustComplaintModels.Tables[0].Rows[0]["ca_proposed_by"].ToString() != "")
                        {
                            ViewBag.ReportedByArray = (dsCustComplaintModels.Tables[0].Rows[0]["ca_proposed_by"].ToString()).Split(',');
                        }
                        if (dsCustComplaintModels.Tables[0].Rows[0]["ca_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsCustComplaintModels.Tables[0].Rows[0]["ca_notifiedto"].ToString()).Split(',');
                        }

                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtValue))
                        {
                            objModel.ReceivedDate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["registered_on"].ToString(), out dtValue))
                        {
                            objModel.registered_on = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["ca_verfiry_duedate"].ToString(), out dtValue))
                        {
                            objModel.ca_verfiry_duedate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["ca_notifed_date"].ToString(), out dtValue))
                        {
                            objModel.ca_notifed_date = dtValue;
                        }


                        CustComplaintModelsList CAList = new CustComplaintModelsList();
                        CAList.CustComplaintList = new List<CustComplaintModels>();

                        string Ssql = "select id_cust_nc_ca,id_custcomplaint_nc,ca_ca,ca_resource," +
                        "ca_target_date,ca_resp_person from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc = '" + sid_custcomplaint_nc + "' and ca_active=1";

                        DataSet dsCALsit = objGlobaldata.Getdetails(Ssql);

                        if (dsCALsit.Tables.Count > 0 && dsCALsit.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsCALsit.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    CustComplaintModels objCustComplaintModels = new CustComplaintModels
                                    {
                                        id_cust_nc_ca = (dsCALsit.Tables[0].Rows[i]["id_cust_nc_ca"].ToString()),
                                        ca_ca = (dsCALsit.Tables[0].Rows[i]["ca_ca"].ToString()),
                                        ca_resource = (dsCALsit.Tables[0].Rows[i]["ca_resource"].ToString()),
                                        ca_resp_person = (dsCALsit.Tables[0].Rows[i]["ca_resp_person"].ToString()),
                                    };
                                    if (DateTime.TryParse(dsCALsit.Tables[0].Rows[i]["ca_target_date"].ToString(), out dtValue))
                                    {
                                        objCustComplaintModels.ca_target_date = dtValue;
                                    }
                                    CAList.CustComplaintList.Add(objCustComplaintModels);
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
                        return RedirectToAction("CustomerComplaintList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerComplaintList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddCA(CustComplaintModels objModel, FormCollection form)
        {
            try
            {
                CustComplaintModelsList objModelList = new CustComplaintModelsList();
                objModelList.CustComplaintList = new List<CustComplaintModels>();

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
                    CustComplaintModels objNCModel = new CustComplaintModels();
                    if (form["ca_ca " + i] != "" && form["ca_resource " + i] != null)
                    {
                        objNCModel.id_cust_nc_ca = form["id_cust_nc_ca " + i];
                        objNCModel.ca_ca = form["ca_ca " + i];
                        objNCModel.ca_resource = form["ca_resource " + i];
                        objNCModel.ca_resp_person = form["ca_resp_person " + i];
                        if (DateTime.TryParse(form["ca_target_date " + i], out dateValue) == true)
                        {
                            objNCModel.ca_target_date = dateValue;
                        }
                        objModelList.CustComplaintList.Add(objNCModel);
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
            CustComplaintModels objModel = new CustComplaintModels();
            try
            {
                if (Request.QueryString["id_custcomplaint_nc"] != null && Request.QueryString["id_custcomplaint_nc"] != "" && Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    string sid_custcomplaint_nc = Request.QueryString["id_custcomplaint_nc"];
                    string sid_complaint = Request.QueryString["id_complaint"];

                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                    ViewBag.NCRStatus = objGlobaldata.GetDropdownList("NCR Status");
                    ViewBag.Action = objGlobaldata.GetDropdownList("NC Action Implementation");
                    ViewBag.OpenClose = objGlobaldata.GetConstantValue("OpenClose");

                    string sSqlstmt = "select id_custcomplaint_nc,a.id_complaint,b.ForwarderAssign,a.TargetDate,ComplaintNo,CustomerName,CustomerRef,ProjectName,ReceivedDate,ModeOfComplaint,ReportedBy,reportedby_email,reportedby_no,reportedby_desig,registered_on,rca_details," +
                                        "nc_no,v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_upload," +
                                 "v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto from t_custcomplaint a, t_custcomplaint_nc b where id_custcomplaint_nc='" + sid_custcomplaint_nc + "' and a.id_complaint= b.id_complaint and a.id_complaint='" + sid_complaint + "'";
                    DataSet dsCustComplaintModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustComplaintModels.Tables.Count > 0 && dsCustComplaintModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new CustComplaintModels
                        {
                            id_custcomplaint_nc = (dsCustComplaintModels.Tables[0].Rows[0]["id_custcomplaint_nc"].ToString()),
                            id_complaint = Convert.ToInt32(dsCustComplaintModels.Tables[0].Rows[0]["id_complaint"].ToString()),
                            ComplaintNo = (dsCustComplaintModels.Tables[0].Rows[0]["ComplaintNo"].ToString()),
                            CustomerName = objGlobaldata.GetCustomerNameById(dsCustComplaintModels.Tables[0].Rows[0]["CustomerName"].ToString()),
                            CustomerRef = (dsCustComplaintModels.Tables[0].Rows[0]["CustomerRef"].ToString()),
                            ProjectName = (dsCustComplaintModels.Tables[0].Rows[0]["ProjectName"].ToString()),
                            ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsCustComplaintModels.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                            ReportedBy = (dsCustComplaintModels.Tables[0].Rows[0]["ReportedBy"].ToString()),
                            reportedby_email = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_email"].ToString()),
                            reportedby_no = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_no"].ToString()),
                            reportedby_desig = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_desig"].ToString()),
                            nc_no = dsCustComplaintModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            rca_details = dsCustComplaintModels.Tables[0].Rows[0]["rca_details"].ToString(),

                            v_implement = (dsCustComplaintModels.Tables[0].Rows[0]["v_implement"].ToString()),
                            v_implement_explain = (dsCustComplaintModels.Tables[0].Rows[0]["v_implement_explain"].ToString()),
                            v_rca = (dsCustComplaintModels.Tables[0].Rows[0]["v_rca"].ToString()),
                            v_rca_explain = (dsCustComplaintModels.Tables[0].Rows[0]["v_rca_explain"].ToString()),
                            v_discrepancies = (dsCustComplaintModels.Tables[0].Rows[0]["v_discrepancies"].ToString()),
                            v_discrep_explain = (dsCustComplaintModels.Tables[0].Rows[0]["v_discrep_explain"].ToString()),
                            v_upload = (dsCustComplaintModels.Tables[0].Rows[0]["v_upload"].ToString()),
                            v_status = (dsCustComplaintModels.Tables[0].Rows[0]["v_status"].ToString()),
                            v_verifiedto = (dsCustComplaintModels.Tables[0].Rows[0]["v_verifiedto"].ToString()),
                            v_notifiedto = (dsCustComplaintModels.Tables[0].Rows[0]["v_notifiedto"].ToString()),
                        };

                        if (dsCustComplaintModels.Tables[0].Rows[0]["v_verifiedto"].ToString() != "")
                        {
                            ViewBag.ReportedByArray = (dsCustComplaintModels.Tables[0].Rows[0]["v_verifiedto"].ToString()).Split(',');
                        }
                        else
                        {
                            ViewBag.ReportedByArray = (objGlobaldata.GetCurrentUserSession().empid).Split(',');
                        }

                        if (dsCustComplaintModels.Tables[0].Rows[0]["v_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsCustComplaintModels.Tables[0].Rows[0]["v_notifiedto"].ToString()).Split(',');
                        }

                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtValue))
                        {
                            objModel.ReceivedDate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["registered_on"].ToString(), out dtValue))
                        {
                            objModel.registered_on = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["v_verified_date"].ToString(), out dtValue))
                        {
                            objModel.v_verified_date = dtValue;
                        }
                        if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["v_closed_date"].ToString(), out dtValue))
                        {
                            objModel.v_closed_date = dtValue;
                        }

                        CustComplaintModelsList objVeriList = new CustComplaintModelsList();
                        objVeriList.CustComplaintList = new List<CustComplaintModels>();

                        string Sql = "Select id_cust_nc_ca,ca_ca,ca_resource," +
                       "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc = '" + sid_custcomplaint_nc + "' and ca_active=1";

                        DataSet dsCAModels = objGlobaldata.Getdetails(Sql);

                        if (dsCAModels.Tables.Count > 0 && dsCAModels.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsCAModels.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    CustComplaintModels objCAModel = new CustComplaintModels
                                    {
                                        id_cust_nc_ca = (dsCAModels.Tables[0].Rows[i]["id_cust_nc_ca"].ToString()),
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
                                    objVeriList.CustComplaintList.Add(objCAModel);
                                }
                                catch (Exception Ex)
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
                        return RedirectToAction("CustomerComplaintList");
                    }

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVerification: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerComplaintList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddVerification(CustComplaintModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> v_upload)
        {
            try
            {
                CustComplaintModelsList objModelList = new CustComplaintModelsList();
                objModelList.CustComplaintList = new List<CustComplaintModels>();

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
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustComplaint"), Path.GetFileName(file.FileName));
                            string sFilename = "CC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.v_upload = objModel.v_upload + "," + "~/DataUpload/MgmtDocs/CustComplaint/" + sFilename;
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
                    CustComplaintModels objNCModel = new CustComplaintModels();
                    if (form["id_cust_nc_ca " + i] != "" && form["id_cust_nc_ca " + i] != null)
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
                        objNCModel.id_cust_nc_ca = form["id_cust_nc_ca " + i];
                        objNCModel.implement_status = form["implement_status " + i];
                        objNCModel.ca_effective = form["ca_effective " + i];
                        objNCModel.reason = form["reason " + i];
                    }
                    objModelList.CustComplaintList.Add(objNCModel);
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

        //CustComplaint NC Details
        [AllowAnonymous]
        public ActionResult NCDetails()
        {
            CustComplaintModels objModel = new CustComplaintModels();

            CustComplaintModelsList NcList = new CustComplaintModelsList();
            NcList.CustComplaintList = new List<CustComplaintModels>();
            try
            {
                if (Request.QueryString["id_custcomplaint_nc"] != null && Request.QueryString["id_custcomplaint_nc"] != "" && Request.QueryString["id_complaint"] != null && Request.QueryString["id_complaint"] != "")
                {
                    string sid_custcomplaint_nc = Request.QueryString["id_custcomplaint_nc"];
                    string sid_complaint = Request.QueryString["id_complaint"];

                    string sSqlstmt = "select id_complaint,TargetDate,ComplaintNo,CustomerName,ProjectName,ReceivedDate,ModeOfComplaint,ReportedBy,reportedby_email,reportedby_no,reportedby_desig,registered_on,Details," +
                        "ForwardTo,ComplaintStatus,Document,ForwarderAssign,CustomerRef,LoggedBy" +
                                      " from t_custcomplaint where id_complaint = '" + sid_complaint + "'";
                    DataSet dsCustComplaintModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustComplaintModels.Tables.Count > 0 && dsCustComplaintModels.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objModel = new CustComplaintModels
                            {
                                id_complaint = Convert.ToInt32(dsCustComplaintModels.Tables[0].Rows[0]["id_complaint"].ToString()),
                                ComplaintNo = (dsCustComplaintModels.Tables[0].Rows[0]["ComplaintNo"].ToString()),
                                CustomerName = objGlobaldata.GetCustomerNameById(dsCustComplaintModels.Tables[0].Rows[0]["CustomerName"].ToString()),
                                ProjectName = (dsCustComplaintModels.Tables[0].Rows[0]["ProjectName"].ToString()),
                                ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsCustComplaintModels.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                                ReportedBy = (dsCustComplaintModels.Tables[0].Rows[0]["ReportedBy"].ToString()),
                                reportedby_email = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_email"].ToString()),
                                reportedby_desig = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_desig"].ToString()),
                                reportedby_no = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_no"].ToString()),
                                Details = dsCustComplaintModels.Tables[0].Rows[0]["Details"].ToString(),

                                ForwardTo = objGlobaldata.GetMultiHrEmpNameById(dsCustComplaintModels.Tables[0].Rows[0]["ForwardTo"].ToString()),
                                //ComplaintStatus = dsCustComplaintModels.Tables[0].Rows[0]["ComplaintStatus"].ToString(),
                                Document = dsCustComplaintModels.Tables[0].Rows[0]["Document"].ToString(),
                                ForwarderAssign = objGlobaldata.GetMultiHrEmpNameById(dsCustComplaintModels.Tables[0].Rows[0]["ForwarderAssign"].ToString()),
                                CustomerRef = dsCustComplaintModels.Tables[0].Rows[0]["CustomerRef"].ToString(),
                                LoggedBy= objGlobaldata.GetMultiHrEmpNameById(dsCustComplaintModels.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtValue))
                            {
                                objModel.ReceivedDate = dtValue;
                            }
                            if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["registered_on"].ToString(), out dtValue))
                            {
                                objModel.registered_on = dtValue;
                            }

                            // --------------------objComplaintModels Status-------
                            string sSqlstmt1 = "select v_status from t_custcomplaint a,t_custcomplaint_nc b where nc_active=1" +
                          " and a.id_complaint=b.id_complaint and a.id_complaint = '" + sid_complaint + "'";
                            DataSet dsData1 = objGlobaldata.Getdetails(sSqlstmt1);
                            if (dsData1.Tables.Count > 0 && dsData1.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dsData1.Tables[0].Rows.Count; j++)
                                {
                                    if (dsData1.Tables[0].Rows[j]["v_status"].ToString() != "")
                                    {
                                        if (objGlobaldata.GetDropdownitemById(dsData1.Tables[0].Rows[j]["v_status"].ToString()) == "Open")
                                        {
                                            objModel.ComplaintStatus = "Open";
                                            break;
                                        }
                                        else
                                        {
                                            objModel.ComplaintStatus = "Closed";
                                        }
                                    }
                                }
                            }

                            //--------------------End Complaint Status----------------

                            string sql = "Select branch,Department,Location from t_customer_info where CustID = '" + dsCustComplaintModels.Tables[0].Rows[0]["CustomerName"].ToString() + "'";
                            DataSet CustList = objGlobaldata.Getdetails(sql);
                            if (CustList.Tables.Count > 0 && CustList.Tables[0].Rows.Count > 0)
                            {
                                objModel.branch = objGlobaldata.GetMultiCompanyBranchNameById(CustList.Tables[0].Rows[0]["branch"].ToString());
                                objModel.Department = objGlobaldata.GetMultiDeptNameById(CustList.Tables[0].Rows[0]["Department"].ToString());
                                objModel.Location = objGlobaldata.GetDivisionLocationById(CustList.Tables[0].Rows[0]["Location"].ToString());
                            }

                            NcList.CustComplaintList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in NCDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                       
                    //Disposition
                    string sSqlstmt11 = "select id_custcomplaint_nc,nc_no," +
                       "disp_action_taken,disp_explain,disp_notifiedto,disp_notifeddate,disp_upload from t_custcomplaint_nc where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsDispModel = objGlobaldata.Getdetails(sSqlstmt11);
                    ViewBag.Disposition = dsDispModel;

                    string sSqlstmt12 = "select id_cust_nc_disp_action,disp_action,disp_resp_person,disp_complete_date from t_custcomplaint_nc_disp_action where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsDispModels = objGlobaldata.Getdetails(sSqlstmt12);
                    ViewBag.DispAction = dsDispModels;

                    //Team
                    string sSqlstmt13 = "select nc_team,team_targetdate from t_custcomplaint_nc where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsTeamModel = objGlobaldata.Getdetails(sSqlstmt13);
                    ViewBag.Team = dsTeamModel;

                    //RCA
                    string sSqlstmt14 = "select rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate,rca_startdate from t_custcomplaint_nc where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsRCAModels = objGlobaldata.Getdetails(sSqlstmt14);
                    ViewBag.RCA = dsRCAModels;

                    //CA
                    string sSqlstmt15 = "select ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date from t_custcomplaint_nc where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsCAModels = objGlobaldata.Getdetails(sSqlstmt15);
                    ViewBag.CA = dsCAModels;

                    string sSqlstmt16 = "select id_cust_nc_ca,ca_ca,ca_resource," +
                        "ca_target_date,ca_resp_person from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc = '" + sid_custcomplaint_nc + "' and ca_active=1";
                    DataSet dsCAList = objGlobaldata.Getdetails(sSqlstmt16);
                    ViewBag.CAList = dsCAList;

                    //Verification
                    string sSqlstmt17 = "select v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_upload," +
                        "v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto from t_custcomplaint_nc where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsVerifyModels = objGlobaldata.Getdetails(sSqlstmt17);
                    ViewBag.Verification = dsVerifyModels;

                    string sSqlstmt18 = "Select id_cust_nc_ca,ca_ca,ca_resource," +
                    "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc = '" + sid_custcomplaint_nc + "' and ca_active=1";
                    DataSet dsVerifyModel = objGlobaldata.Getdetails(sSqlstmt18);
                    ViewBag.VerificationList = dsVerifyModel;

                }
                else
                {
                    TempData["alertdata"] = "NC Id cannot be null";
                    return RedirectToAction("CustomerComplaintList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }


        [AllowAnonymous]
        public ActionResult NCPDF(FormCollection form)
        {
            CustComplaintModels objModel = new CustComplaintModels();

            CustComplaintModelsList NcList = new CustComplaintModelsList();
            NcList.CustComplaintList = new List<CustComplaintModels>();
            try
            {
                if (form["id_custcomplaint_nc"] != null && form["id_custcomplaint_nc"] != "" && form["id_complaint"] != null && form["id_complaint"] != "")
                {
                    string sid_custcomplaint_nc = form["id_custcomplaint_nc"];
                    string sid_complaint = form["id_complaint"];

                    string sSqlstmt = "select id_complaint,TargetDate,ComplaintNo,CustomerName,ProjectName,ReceivedDate,ModeOfComplaint,ReportedBy,reportedby_email,reportedby_no,reportedby_desig,registered_on,Details," +
                        "ForwardTo,ComplaintStatus,Document,ForwarderAssign,CustomerRef,LoggedBy" +
                                      " from t_custcomplaint where id_complaint = '" + sid_complaint + "'";
                    DataSet dsCustComplaintModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustComplaintModels.Tables.Count > 0 && dsCustComplaintModels.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objModel = new CustComplaintModels
                            {
                                id_complaint = Convert.ToInt32(dsCustComplaintModels.Tables[0].Rows[0]["id_complaint"].ToString()),
                                ComplaintNo = (dsCustComplaintModels.Tables[0].Rows[0]["ComplaintNo"].ToString()),
                                CustomerName = objGlobaldata.GetCustomerNameById(dsCustComplaintModels.Tables[0].Rows[0]["CustomerName"].ToString()),
                                ProjectName = (dsCustComplaintModels.Tables[0].Rows[0]["ProjectName"].ToString()),
                                ModeOfComplaint = objGlobaldata.GetModeOfComplaintById(dsCustComplaintModels.Tables[0].Rows[0]["ModeOfComplaint"].ToString()),
                                ReportedBy = (dsCustComplaintModels.Tables[0].Rows[0]["ReportedBy"].ToString()),
                                reportedby_email = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_email"].ToString()),
                                reportedby_desig = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_desig"].ToString()),
                                reportedby_no = (dsCustComplaintModels.Tables[0].Rows[0]["reportedby_no"].ToString()),
                                Details = dsCustComplaintModels.Tables[0].Rows[0]["Details"].ToString(),

                                ForwardTo = objGlobaldata.GetMultiHrEmpNameById(dsCustComplaintModels.Tables[0].Rows[0]["ForwardTo"].ToString()),
                                ComplaintStatus = dsCustComplaintModels.Tables[0].Rows[0]["ComplaintStatus"].ToString(),
                                Document = dsCustComplaintModels.Tables[0].Rows[0]["Document"].ToString(),
                                ForwarderAssign = objGlobaldata.GetMultiHrEmpNameById(dsCustComplaintModels.Tables[0].Rows[0]["ForwarderAssign"].ToString()),
                                CustomerRef = dsCustComplaintModels.Tables[0].Rows[0]["CustomerRef"].ToString(),
                                LoggedBy = objGlobaldata.GetMultiHrEmpNameById(dsCustComplaintModels.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["ReceivedDate"].ToString(), out dtValue))
                            {
                                objModel.ReceivedDate = dtValue;
                            }
                            if (DateTime.TryParse(dsCustComplaintModels.Tables[0].Rows[0]["registered_on"].ToString(), out dtValue))
                            {
                                objModel.registered_on = dtValue;
                            }

                            // --------------------objComplaintModels Status-------
                            string sSqlstmt1 = "select v_status from t_custcomplaint a,t_custcomplaint_nc b where nc_active=1" +
                          " and a.id_complaint=b.id_complaint and a.id_complaint = '" + sid_complaint + "'";
                            DataSet dsData1 = objGlobaldata.Getdetails(sSqlstmt1);
                            if (dsData1.Tables.Count > 0 && dsData1.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dsData1.Tables[0].Rows.Count; j++)
                                {
                                    if (dsData1.Tables[0].Rows[j]["v_status"].ToString() != "")
                                    {
                                        if (objGlobaldata.GetDropdownitemById(dsData1.Tables[0].Rows[j]["v_status"].ToString()) == "Open")
                                        {
                                            objModel.ComplaintStatus = "Open";
                                            break;
                                        }
                                        else
                                        {
                                            objModel.ComplaintStatus = "Closed";
                                        }
                                    }
                                }
                            }


                            string sql = "Select branch,Department,Location from t_customer_info where CustID = '" + dsCustComplaintModels.Tables[0].Rows[0]["CustomerName"].ToString() + "'";
                            DataSet CustList = objGlobaldata.Getdetails(sql);
                            if (CustList.Tables.Count > 0 && CustList.Tables[0].Rows.Count > 0)
                            {
                                objModel.branch = objGlobaldata.GetMultiCompanyBranchNameById(CustList.Tables[0].Rows[0]["branch"].ToString());
                                objModel.Department = objGlobaldata.GetMultiDeptNameById(CustList.Tables[0].Rows[0]["Department"].ToString());
                                objModel.Location = objGlobaldata.GetDivisionLocationById(CustList.Tables[0].Rows[0]["Location"].ToString());
                            }

                            NcList.CustComplaintList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in NCDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                    CompanyModels objCompany = new CompanyModels();
                    dsCustComplaintModels = objCompany.GetCompanyDetailsForReport(dsCustComplaintModels);
                    dsCustComplaintModels = objGlobaldata.GetReportDetails(dsCustComplaintModels, objModel.ComplaintNo, dsCustComplaintModels.Tables[0].Rows[0]["LoggedBy"].ToString(), "NON CONFORMANCE REPORT");

                    ViewBag.CompanyInfo = dsCustComplaintModels;


                    //Disposition
                    string sSqlstmt11 = "select id_custcomplaint_nc,nc_no," +
                       "disp_action_taken,disp_explain,disp_notifiedto,disp_notifeddate,disp_upload from t_custcomplaint_nc where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsDispModel = objGlobaldata.Getdetails(sSqlstmt11);
                    ViewBag.Disposition = dsDispModel;

                    string sSqlstmt12 = "select id_cust_nc_disp_action,disp_action,disp_resp_person,disp_complete_date from t_custcomplaint_nc_disp_action where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsDispModels = objGlobaldata.Getdetails(sSqlstmt12);
                    ViewBag.DispAction = dsDispModels;

                    //Team
                    string sSqlstmt13 = "select nc_team,team_targetdate from t_custcomplaint_nc where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsTeamModel = objGlobaldata.Getdetails(sSqlstmt13);
                    ViewBag.Team = dsTeamModel;

                    //RCA
                    string sSqlstmt14 = "select rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate,rca_startdate from t_custcomplaint_nc where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsRCAModels = objGlobaldata.Getdetails(sSqlstmt14);
                    ViewBag.RCA = dsRCAModels;

                    //CA
                    string sSqlstmt15 = "select ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date from t_custcomplaint_nc where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsCAModels = objGlobaldata.Getdetails(sSqlstmt15);
                    ViewBag.CA = dsCAModels;

                    string sSqlstmt16 = "select id_cust_nc_ca,ca_ca,ca_resource," +
                        "ca_target_date,ca_resp_person from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc = '" + sid_custcomplaint_nc + "' and ca_active=1";
                    DataSet dsCAList = objGlobaldata.Getdetails(sSqlstmt16);
                    ViewBag.CAList = dsCAList;

                    //Verification
                    string sSqlstmt17 = "select v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_upload," +
                        "v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto from t_custcomplaint_nc where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";
                    DataSet dsVerifyModels = objGlobaldata.Getdetails(sSqlstmt17);
                    ViewBag.Verification = dsVerifyModels;

                    string sSqlstmt18 = "Select id_cust_nc_ca,ca_ca,ca_resource," +
                    "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc = '" + sid_custcomplaint_nc + "' and ca_active=1";
                    DataSet dsVerifyModel = objGlobaldata.Getdetails(sSqlstmt18);
                    ViewBag.VerificationList = dsVerifyModel;

                }
                else
                {
                    TempData["alertdata"] = "NC Id cannot be null";
                    return RedirectToAction("CustomerComplaintList");
                }
                ViewBag.NonConfirmance = objModel;
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

                if (form["id_custcomplaint_nc"] != null && form["id_custcomplaint_nc"] != "")
                {

                    CustComplaintModels Doc = new CustComplaintModels();
                    string sid_custcomplaint_nc = form["id_custcomplaint_nc"];

                    if (Doc.FunDeleteNCDoc(sid_custcomplaint_nc))
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

                if (form["id_cust_nc_ca"] != null && form["id_cust_nc_ca"] != "")
                {

                    CustComplaintModels Doc = new CustComplaintModels();
                    string sid_cust_nc_ca = form["id_cust_nc_ca"];

                    if (Doc.FunDeleteCADoc(sid_cust_nc_ca))
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

    }
}