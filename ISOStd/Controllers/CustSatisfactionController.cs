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
    public class CustSatisfactionController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddCustSatisfaction()
        {
            CustSatisfactionModels objModel = new CustSatisfactionModels();
            try
            {
                string branch = objGlobaldata.GetCurrentUserSession().division;

                objModel.branch = objGlobaldata.GetCurrentUserSession().division;
                objModel.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objModel.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(branch);         
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(branch);
                ViewBag.Customer = objGlobaldata.GetCustomerListbox();
                ViewBag.performance = objGlobaldata.GetDropdownList("Customer Performance/reliability of the product");
                ViewBag.delivery = objGlobaldata.GetDropdownList("Customer on time delivery");
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.complaint_type = objGlobaldata.GetDropdownList("Customer Complaint Type");
                ViewBag.survey_form = objGlobaldata.GetDropdownList("Feedback through satisfaction survey form");
                ViewBag.Reviewer = objGlobaldata.GetSceenNotificationEmpList("Customer Satisfaction", "To be reviewed by");
                ViewBag.cust_satisfied = objGlobaldata.GetDropdownList("Customer Satisfied");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustSatisfaction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddCustSatisfaction(CustSatisfactionModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> enquiries_upload, IEnumerable<HttpPostedFileBase> orders_upload)
        {
            try
            {
                IList<HttpPostedFileBase> enquiries_uploadList = (IList<HttpPostedFileBase>)enquiries_upload;
                IList<HttpPostedFileBase> orders_uploadList = (IList<HttpPostedFileBase>)orders_upload;

             
                DateTime dateValue;

                if (form["frm_date"] != null && DateTime.TryParse(form["frm_date"], out dateValue) == true)
                {
                    objModel.frm_date = dateValue;
                }
                if (form["to_date"] != null && DateTime.TryParse(form["to_date"], out dateValue) == true)
                {
                    objModel.to_date = dateValue;
                }
                if (form["eval_date"] != null && DateTime.TryParse(form["eval_date"], out dateValue) == true)
                {
                    objModel.eval_date = dateValue;
                }
                if(enquiries_upload != null)
                {
                    if (enquiries_uploadList[0] != null)
                    {
                        objModel.enquiries_upload = "";
                        foreach (var file in enquiries_upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustSatisfaction"), Path.GetFileName(file.FileName));
                                string sFilename = "CS" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objModel.enquiries_upload = objModel.enquiries_upload + "," + "~/DataUpload/MgmtDocs/CustSatisfaction/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddCustSatisfaction: " + ex.ToString());
                            }
                        }
                        objModel.enquiries_upload = objModel.enquiries_upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                }
                if (orders_upload != null)
                {
                    if (orders_uploadList[0] != null)
                    {
                        objModel.orders_upload = "";
                        foreach (var file in orders_upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustSatisfaction"), Path.GetFileName(file.FileName));
                                string sFilename = "CS" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objModel.orders_upload = objModel.orders_upload + "," + "~/DataUpload/MgmtDocs/CustSatisfaction/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddCustSatisfaction: " + ex.ToString());
                            }
                        }
                        objModel.orders_upload = objModel.orders_upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                }

                // QULAITY
                CustSatisfactionModelsList objQList = new CustSatisfactionModelsList();
                objQList.CSList = new List<CustSatisfactionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    CustSatisfactionModels objModels = new CustSatisfactionModels();
                    if (form["performance " + i] != null && form["performance " + i] != "")
                    {
                        objModels.performance = form["performance " + i];
                        objModels.perf_remarks = form["perf_remarks " + i];
                        objModels.perf_upload = form["perf_upload1 " + i];

                        objQList.CSList.Add(objModels);
                    }
                }
                // DELIVERY
                CustSatisfactionModelsList objDList = new CustSatisfactionModelsList();
                objDList.CSList = new List<CustSatisfactionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt2"]); i++)
                {
                    CustSatisfactionModels objModels = new CustSatisfactionModels();
                    if (form["delivery " + i] != null && form["delivery " + i] != "")
                    {
                        objModels.delivery = form["delivery " + i];
                        objModels.delivery_remarks = form["delivery_remarks " + i];
                        objModels.delivery_upload = form["delivery_upload1 " + i];

                        objDList.CSList.Add(objModels);
                    }
                }
                // COMPLAINT
                CustSatisfactionModelsList objCList = new CustSatisfactionModelsList();
                objCList.CSList = new List<CustSatisfactionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt3"]); i++)
                {
                    CustSatisfactionModels objModels = new CustSatisfactionModels();
                    if (form["complaint_type " + i] != null && form["complaint_type " + i] != "")
                    {
                       
                        objModels.complaint_type = form["complaint_type " + i];
                        objModels.complaint_no = form["complaint_no " + i];
                        objModels.complaint_upload = form["complaint_upload1 " + i];
                        objCList.CSList.Add(objModels);
                    }
                }

                if (objModel.FunAddCustomerSatisfaction(objModel, objQList, objDList, objCList))
                {
                    TempData["Successdata"] = "Added customer satisfaction successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustSatisfaction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustSatisfactionList");
        }

        [AllowAnonymous]
        public ActionResult EditCustSatisfaction()
        {
            CustSatisfactionModels objModels = new CustSatisfactionModels();
            try
            {

                if (Request.QueryString["id_cust_satisfaction"] != null && Request.QueryString["id_cust_satisfaction"] != "")
                {
                    string id_cust_satisfaction = Request.QueryString["id_cust_satisfaction"];

                    string sSqlstmt = "select id_cust_satisfaction,branch,Department,Location,cust_name,prod_delivered,contact_details,repeat_enquiries,no_enquiries,repeat_orders,"
                    + "no_orders,enquiries_upload,orders_upload,survey_form,csi,reviewed_by,eval_date,frm_date,to_date,complaint,cust_satisfied from t_cust_satisfaction where id_cust_satisfaction=" + id_cust_satisfaction;

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {
                       
                        objModels = new CustSatisfactionModels
                        {
                            id_cust_satisfaction = (dsModelsList.Tables[0].Rows[0]["id_cust_satisfaction"].ToString()),
                            branch = (dsModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsModelsList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = (dsModelsList.Tables[0].Rows[0]["Location"].ToString()),
                            cust_name = (dsModelsList.Tables[0].Rows[0]["cust_name"].ToString()),
                            prod_delivered = (dsModelsList.Tables[0].Rows[0]["prod_delivered"].ToString()),
                            contact_details = (dsModelsList.Tables[0].Rows[0]["contact_details"].ToString()),
                            repeat_enquiries = (dsModelsList.Tables[0].Rows[0]["repeat_enquiries"].ToString()),
                            no_enquiries = (dsModelsList.Tables[0].Rows[0]["no_enquiries"].ToString()),
                            repeat_orders = (dsModelsList.Tables[0].Rows[0]["repeat_orders"].ToString()),
                            no_orders = (dsModelsList.Tables[0].Rows[0]["no_orders"].ToString()),
                            enquiries_upload = (dsModelsList.Tables[0].Rows[0]["enquiries_upload"].ToString()),
                            orders_upload = (dsModelsList.Tables[0].Rows[0]["orders_upload"].ToString()),
                            survey_form = (dsModelsList.Tables[0].Rows[0]["survey_form"].ToString()),
                            csi = (dsModelsList.Tables[0].Rows[0]["csi"].ToString()),
                            reviewed_by = (dsModelsList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            complaint = (dsModelsList.Tables[0].Rows[0]["complaint"].ToString()),
                            cust_satisfied = (dsModelsList.Tables[0].Rows[0]["cust_satisfied"].ToString()),

                        };
                        DateTime dtDocDate = new DateTime();
                        if (dsModelsList.Tables[0].Rows[0]["frm_date"].ToString() != ""
                            && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["frm_date"].ToString(), out dtDocDate))
                        {
                            objModels.frm_date = dtDocDate;
                        }
                        if (dsModelsList.Tables[0].Rows[0]["to_date"].ToString() != ""
                           && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["to_date"].ToString(), out dtDocDate))
                        {
                            objModels.to_date = dtDocDate;
                        }
                        if (dsModelsList.Tables[0].Rows[0]["eval_date"].ToString() != ""
                           && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["eval_date"].ToString(), out dtDocDate))
                        {
                            objModels.eval_date = dtDocDate;
                        }
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Department = objGlobaldata.GetDepartmentListbox(objModels.branch);
                        ViewBag.Location = objGlobaldata.GetDivisionLocationList(objModels.branch);
                        ViewBag.Customer = objGlobaldata.GetCustomerListbox();
                        ViewBag.performance = objGlobaldata.GetDropdownList("Customer Performance/reliability of the product");
                        ViewBag.delivery = objGlobaldata.GetDropdownList("Customer on time delivery");
                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.complaint_type = objGlobaldata.GetDropdownList("Customer Complaint Type");
                        ViewBag.survey_form = objGlobaldata.GetDropdownList("Feedback through satisfaction survey form");
                        ViewBag.Reviewer = objGlobaldata.GetSceenNotificationEmpList("Customer Satisfaction", "To be reviewed by");
                        ViewBag.cust_satisfied = objGlobaldata.GetDropdownList("Customer Satisfied");


                        // QULAITY
                        CustSatisfactionModelsList objQList = new CustSatisfactionModelsList();
                        objQList.CSList = new List<CustSatisfactionModels>();

                        sSqlstmt = "select id_perf,id_cust_satisfaction,performance,perf_remarks,perf_upload from t_cust_satisfaction_perf where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    CustSatisfactionModels OBJModel = new CustSatisfactionModels
                                    {
                                        id_perf = dsList.Tables[0].Rows[i]["id_perf"].ToString(),
                                        id_cust_satisfaction = dsList.Tables[0].Rows[i]["id_cust_satisfaction"].ToString(),
                                        performance = dsList.Tables[0].Rows[i]["performance"].ToString(),
                                        perf_remarks = dsList.Tables[0].Rows[i]["perf_remarks"].ToString(),
                                        perf_upload = dsList.Tables[0].Rows[i]["perf_upload"].ToString(),

                                    };
                                    objQList.CSList.Add(OBJModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in EditCustSatisfaction: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("CustSatisfactionList");
                                }
                            }
                            ViewBag.objQList = objQList;
                        }

                        // DELIVERY
                        CustSatisfactionModelsList objDList = new CustSatisfactionModelsList();
                        objDList.CSList = new List<CustSatisfactionModels>();

                        sSqlstmt = "select id_delivery,id_cust_satisfaction,delivery,delivery_remarks,delivery_upload from t_cust_satisfaction_delivery where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    CustSatisfactionModels OBJModel = new CustSatisfactionModels
                                    {
                                        id_delivery = dsList.Tables[0].Rows[i]["id_delivery"].ToString(),
                                        id_cust_satisfaction = dsList.Tables[0].Rows[i]["id_cust_satisfaction"].ToString(),
                                        delivery = dsList.Tables[0].Rows[i]["delivery"].ToString(),
                                        delivery_remarks = dsList.Tables[0].Rows[i]["delivery_remarks"].ToString(),
                                        delivery_upload = dsList.Tables[0].Rows[i]["delivery_upload"].ToString(),

                                    };
                                    objDList.CSList.Add(OBJModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in EditCustSatisfaction: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("CustSatisfactionList");
                                }
                            }
                            ViewBag.objDList = objDList;
                        }
                        // COMPLAINT
                        CustSatisfactionModelsList objCList = new CustSatisfactionModelsList();
                        objCList.CSList = new List<CustSatisfactionModels>();

                        sSqlstmt = "select id_complaint,id_cust_satisfaction,complaint_type,complaint_no,complaint_upload from t_cust_satisfaction_complaints where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    CustSatisfactionModels OBJModel = new CustSatisfactionModels
                                    {
                                        id_complaint = dsList.Tables[0].Rows[i]["id_complaint"].ToString(),
                                        id_cust_satisfaction = dsList.Tables[0].Rows[i]["id_cust_satisfaction"].ToString(),
                                       
                                        complaint_type = dsList.Tables[0].Rows[i]["complaint_type"].ToString(),
                                        complaint_no = dsList.Tables[0].Rows[i]["complaint_no"].ToString(),
                                        complaint_upload = dsList.Tables[0].Rows[i]["complaint_upload"].ToString(),
                                    };
                                    objCList.CSList.Add(OBJModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in EditCustSatisfaction: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("CustSatisfactionList");
                                }
                            }
                            ViewBag.objCList = objCList;
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EditCustSatisfaction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditCustSatisfaction(CustSatisfactionModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> enquiries_upload, IEnumerable<HttpPostedFileBase> orders_upload)
        {
            try
            {
                IList<HttpPostedFileBase> enquiries_uploadList = (IList<HttpPostedFileBase>)enquiries_upload;
                IList<HttpPostedFileBase> orders_uploadList = (IList<HttpPostedFileBase>)orders_upload;
                string QCDelete = Request.Form["QCDocsValselectall"];
                string QCDelete1 = Request.Form["QCDocsValselectall1"];

                DateTime dateValue;

                if (form["frm_date"] != null && DateTime.TryParse(form["frm_date"], out dateValue) == true)
                {
                    objModel.frm_date = dateValue;
                }
                if (form["to_date"] != null && DateTime.TryParse(form["to_date"], out dateValue) == true)
                {
                    objModel.to_date = dateValue;
                }
                if (form["eval_date"] != null && DateTime.TryParse(form["eval_date"], out dateValue) == true)
                {
                    objModel.eval_date = dateValue;
                }
                if (enquiries_upload != null)
                {
                    if (enquiries_uploadList[0] != null)
                    {
                        objModel.enquiries_upload = "";
                        foreach (var file in enquiries_upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustSatisfaction"), Path.GetFileName(file.FileName));
                                string sFilename = "CS" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objModel.enquiries_upload = objModel.enquiries_upload + "," + "~/DataUpload/MgmtDocs/CustSatisfaction/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddCustSatisfaction: " + ex.ToString());
                            }
                        }
                        objModel.enquiries_upload = objModel.enquiries_upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                    if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                    {
                        objModel.enquiries_upload = objModel.enquiries_upload + "," + form["QCDocsVal"];
                        objModel.enquiries_upload = objModel.enquiries_upload.Trim(',');
                    }
                    else if (form["QCDocsVal"] == null && QCDelete != null && enquiries_uploadList[0] == null)
                    {
                        objModel.enquiries_upload = null;
                    }
                    else if (form["QCDocsVal"] == null && enquiries_uploadList[0] == null)
                    {
                        objModel.enquiries_upload = null;
                    }
                }
                   
                if (orders_upload != null)
                {
                    if (orders_uploadList[0] != null)
                    {
                        objModel.orders_upload = "";
                        foreach (var file in orders_upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustSatisfaction"), Path.GetFileName(file.FileName));
                                string sFilename = "CS" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objModel.orders_upload = objModel.orders_upload + "," + "~/DataUpload/MgmtDocs/CustSatisfaction/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddCustSatisfaction: " + ex.ToString());
                            }
                        }
                        objModel.orders_upload = objModel.orders_upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                    if (form["QCDocsVal1"] != null && form["QCDocsVal1"] != "")
                    {
                        objModel.orders_upload = objModel.orders_upload + "," + form["QCDocsVal1"];
                        objModel.orders_upload = objModel.orders_upload.Trim(',');
                    }
                    else if (form["QCDocsVal1"] == null && QCDelete1 != null && orders_uploadList[0] == null)
                    {
                        objModel.orders_upload = null;
                    }
                    else if (form["QCDocsVal1"] == null && orders_uploadList[0] == null)
                    {
                        objModel.orders_upload = null;
                    }
                }
                    
                // QULAITY
                CustSatisfactionModelsList objQList = new CustSatisfactionModelsList();
                objQList.CSList = new List<CustSatisfactionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    CustSatisfactionModels objModels = new CustSatisfactionModels();
                    if (form["performance " + i] != null && form["performance " + i] != "")
                    {
                        objModels.id_perf = form["id_perf " + i];
                        objModels.performance = form["performance " + i];
                        objModels.perf_remarks = form["perf_remarks " + i];
                        objModels.perf_upload = form["perf_upload1 " + i];

                        objQList.CSList.Add(objModels);
                    }
                }
                // DELIVERY
                CustSatisfactionModelsList objDList = new CustSatisfactionModelsList();
                objDList.CSList = new List<CustSatisfactionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt2"]); i++)
                {
                    CustSatisfactionModels objModels = new CustSatisfactionModels();
                    if (form["delivery " + i] != null && form["delivery " + i] != "")
                    {
                        objModels.id_delivery = form["id_delivery " + i];
                        objModels.delivery = form["delivery " + i];
                        objModels.delivery_remarks = form["delivery_remarks " + i];
                        objModels.delivery_upload = form["delivery_upload1 " + i];

                        objDList.CSList.Add(objModels);
                    }
                }
                // COMPLAINT
                CustSatisfactionModelsList objCList = new CustSatisfactionModelsList();
                objCList.CSList = new List<CustSatisfactionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt3"]); i++)
                {
                    CustSatisfactionModels objModels = new CustSatisfactionModels();
                    if (form["complaint " + i] != null && form["complaint " + i] != "")
                    {
                        objModels.id_complaint = form["id_complaint " + i];
                        objModels.complaint = form["complaint " + i];
                        objModels.complaint_type = form["complaint_type " + i];
                        objModels.complaint_no = form["complaint_no " + i];
                        objModels.complaint_upload = form["complaint_upload1 " + i];
                        objCList.CSList.Add(objModels);
                    }
                }

                if (objModel.FunEditCustomerSatisfaction(objModel, objQList, objDList, objCList))
                {
                    TempData["Successdata"] = "Updated customer satisfaction successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EditCustSatisfaction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustSatisfactionList");
        }


        [AllowAnonymous]
        public ActionResult CustSatisfactionList(string branch_name)
        {
            ViewBag.SubMenutype = "CustSatisfactionList";
            CustSatisfactionModelsList objQList = new CustSatisfactionModelsList();
            objQList.CSList = new List<CustSatisfactionModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_cust_satisfaction,ref_no,branch,Department,Location,cust_name,prod_delivered,contact_details,eval_date,frm_date,to_date," +
                    "(case when review_status=0 then 'Pending for Review'  when review_status=1 then 'Rejected'  when review_status=2 then 'Approved' end) as review_status,review_status as review_status_id"
                    + " from t_cust_satisfaction where active = 1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + " order by id_cust_satisfaction desc";

                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            CustSatisfactionModels objModel = new CustSatisfactionModels
                            {
                                id_cust_satisfaction = dsList.Tables[0].Rows[i]["id_cust_satisfaction"].ToString(),
                                branch =objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetLocationNameById(dsList.Tables[0].Rows[i]["Location"].ToString()),
                                cust_name =objGlobaldata.GetCustomerNameById(dsList.Tables[0].Rows[i]["cust_name"].ToString()),
                                prod_delivered = (dsList.Tables[0].Rows[i]["prod_delivered"].ToString()),
                                contact_details = dsList.Tables[0].Rows[i]["contact_details"].ToString(),
                                review_status = dsList.Tables[0].Rows[i]["review_status"].ToString(),
                                review_status_id = dsList.Tables[0].Rows[i]["review_status_id"].ToString(),
                                ref_no = dsList.Tables[0].Rows[i]["ref_no"].ToString(),
                            };

                            DateTime dtDocDate;
                            if (dsList.Tables[0].Rows[i]["frm_date"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["frm_date"].ToString(), out dtDocDate))
                            {
                                objModel.frm_date = dtDocDate;
                            }
                            if (dsList.Tables[0].Rows[i]["to_date"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["to_date"].ToString(), out dtDocDate))
                            {
                                objModel.to_date = dtDocDate;
                            }
                            if (dsList.Tables[0].Rows[i]["eval_date"].ToString() != ""
                            && DateTime.TryParse(dsList.Tables[0].Rows[i]["eval_date"].ToString(), out dtDocDate))
                            {
                                objModel.eval_date = dtDocDate;
                            }
                            objQList.CSList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingPlanList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustSatisfactionList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objQList.CSList.ToList());
        }

        [AllowAnonymous]
        public JsonResult CustSatisfactionDelete(FormCollection form)
        {
            try
            {
                if (form["Id"] != null && form["Id"] != "")
                {
                    CustSatisfactionModels objModel = new CustSatisfactionModels();
                    string sId = form["Id"];

                    if (objModel.FunDeleteCustomerSatisfaction(sId))
                    {
                        TempData["Successdata"] = "Deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in CustSatisfactionDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult CustSatisfactionReview()
        {
            CustSatisfactionModels objModels = new CustSatisfactionModels();
            try
            {
                if (Request.QueryString["id_cust_satisfaction"] != null && Request.QueryString["id_cust_satisfaction"] != "")
                {
                    string id_cust_satisfaction = Request.QueryString["id_cust_satisfaction"];

                    string sSqlstmt = "select id_cust_satisfaction,branch,Department,Location,cust_name,prod_delivered,contact_details,repeat_enquiries,no_enquiries,repeat_orders,"
                    + "no_orders,enquiries_upload,orders_upload,survey_form,csi,reviewed_by,eval_date,frm_date,to_date,complaint,cust_satisfied from t_cust_satisfaction where id_cust_satisfaction=" + id_cust_satisfaction;

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objModels = new CustSatisfactionModels
                        {
                            id_cust_satisfaction = (dsModelsList.Tables[0].Rows[0]["id_cust_satisfaction"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetDeptNameById(dsModelsList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetLocationNameById(dsModelsList.Tables[0].Rows[0]["Location"].ToString()),
                            cust_name = objGlobaldata.GetCustomerNameById(dsModelsList.Tables[0].Rows[0]["cust_name"].ToString()),
                            prod_delivered = (dsModelsList.Tables[0].Rows[0]["prod_delivered"].ToString()),
                            contact_details = (dsModelsList.Tables[0].Rows[0]["contact_details"].ToString()),
                            repeat_enquiries = (dsModelsList.Tables[0].Rows[0]["repeat_enquiries"].ToString()),
                            no_enquiries = (dsModelsList.Tables[0].Rows[0]["no_enquiries"].ToString()),
                            repeat_orders = (dsModelsList.Tables[0].Rows[0]["repeat_orders"].ToString()),
                            no_orders = (dsModelsList.Tables[0].Rows[0]["no_orders"].ToString()),
                            enquiries_upload = (dsModelsList.Tables[0].Rows[0]["enquiries_upload"].ToString()),
                            orders_upload = (dsModelsList.Tables[0].Rows[0]["orders_upload"].ToString()),
                            survey_form =objGlobaldata.GetDropdownitemById(dsModelsList.Tables[0].Rows[0]["survey_form"].ToString()),
                            csi = (dsModelsList.Tables[0].Rows[0]["csi"].ToString()),
                            reviewed_by = (dsModelsList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            complaint = (dsModelsList.Tables[0].Rows[0]["complaint"].ToString()),
                            cust_satisfied = objGlobaldata.GetDropdownitemById(dsModelsList.Tables[0].Rows[0]["cust_satisfied"].ToString()),

                        };
                        DateTime dtDocDate = new DateTime();
                        if (dsModelsList.Tables[0].Rows[0]["frm_date"].ToString() != ""
                            && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["frm_date"].ToString(), out dtDocDate))
                        {
                            objModels.frm_date = dtDocDate;
                        }
                        if (dsModelsList.Tables[0].Rows[0]["to_date"].ToString() != ""
                           && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["to_date"].ToString(), out dtDocDate))
                        {
                            objModels.to_date = dtDocDate;
                        }
                        if (dsModelsList.Tables[0].Rows[0]["eval_date"].ToString() != ""
                           && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["eval_date"].ToString(), out dtDocDate))
                        {
                            objModels.eval_date = dtDocDate;
                        }
                       


                        // QUALITY
                    
                        sSqlstmt = "select id_perf,id_cust_satisfaction,performance,perf_remarks,perf_upload from t_cust_satisfaction_perf where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        ViewBag.objQList = objGlobaldata.Getdetails(sSqlstmt);
                       

                        // DELIVERY
                       
                        sSqlstmt = "select id_delivery,id_cust_satisfaction,delivery,delivery_remarks,delivery_upload from t_cust_satisfaction_delivery where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        ViewBag.objDList = objGlobaldata.Getdetails(sSqlstmt);
                       
                        // COMPLAINT
                       
                        sSqlstmt = "select id_complaint,id_cust_satisfaction,complaint_type,complaint_no,complaint_upload from t_cust_satisfaction_complaints where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        ViewBag.objCList = objGlobaldata.Getdetails(sSqlstmt);
                      
                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustSatisfactionReview: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objModels);
        }

        public ActionResult ReviewCustSatisfaction(CustSatisfactionModels objModel, FormCollection form)
        {
            try
            {

                if (objModel.FunCustomerSatisfactionReview(objModel))
                {
                    TempData["Successdata"] = "Review status updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustSatisfactionReview: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ImprovementAction()
        {
            CustSatisfactionModels objModels = new CustSatisfactionModels();
            try
            {
                if (Request.QueryString["id_cust_satisfaction"] != null && Request.QueryString["id_cust_satisfaction"] != "")
                {
                    string id_cust_satisfaction = Request.QueryString["id_cust_satisfaction"];

                    string sSqlstmt = "select id_cust_satisfaction,branch,Department,Location,cust_name,prod_delivered,contact_details,repeat_enquiries,no_enquiries,repeat_orders,"
                    + "no_orders,enquiries_upload,orders_upload,survey_form,csi,reviewed_by,eval_date,frm_date,to_date,complaint,cust_satisfied,action_initiated_by from t_cust_satisfaction where id_cust_satisfaction=" + id_cust_satisfaction;

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.curr_user = objGlobaldata.GetCurrentUserSession().empid;
                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objModels = new CustSatisfactionModels
                        {
                            id_cust_satisfaction = (dsModelsList.Tables[0].Rows[0]["id_cust_satisfaction"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetDeptNameById(dsModelsList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetLocationNameById(dsModelsList.Tables[0].Rows[0]["Location"].ToString()),
                            cust_name = objGlobaldata.GetCustomerNameById(dsModelsList.Tables[0].Rows[0]["cust_name"].ToString()),
                            prod_delivered = (dsModelsList.Tables[0].Rows[0]["prod_delivered"].ToString()),
                            contact_details = (dsModelsList.Tables[0].Rows[0]["contact_details"].ToString()),
                            repeat_enquiries = (dsModelsList.Tables[0].Rows[0]["repeat_enquiries"].ToString()),
                            no_enquiries = (dsModelsList.Tables[0].Rows[0]["no_enquiries"].ToString()),
                            repeat_orders = (dsModelsList.Tables[0].Rows[0]["repeat_orders"].ToString()),
                            no_orders = (dsModelsList.Tables[0].Rows[0]["no_orders"].ToString()),
                            enquiries_upload = (dsModelsList.Tables[0].Rows[0]["enquiries_upload"].ToString()),
                            orders_upload = (dsModelsList.Tables[0].Rows[0]["orders_upload"].ToString()),
                            survey_form = objGlobaldata.GetDropdownitemById(dsModelsList.Tables[0].Rows[0]["survey_form"].ToString()),
                            csi = (dsModelsList.Tables[0].Rows[0]["csi"].ToString()),
                            reviewed_by = (dsModelsList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            complaint = (dsModelsList.Tables[0].Rows[0]["complaint"].ToString()),
                            cust_satisfied = objGlobaldata.GetDropdownitemById(dsModelsList.Tables[0].Rows[0]["cust_satisfied"].ToString()),
                            action_initiated_by= (dsModelsList.Tables[0].Rows[0]["action_initiated_by"].ToString()),
                        };
                        DateTime dtDocDate = new DateTime();
                        if (dsModelsList.Tables[0].Rows[0]["frm_date"].ToString() != ""
                            && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["frm_date"].ToString(), out dtDocDate))
                        {
                            objModels.frm_date = dtDocDate;
                        }
                        if (dsModelsList.Tables[0].Rows[0]["to_date"].ToString() != ""
                           && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["to_date"].ToString(), out dtDocDate))
                        {
                            objModels.to_date = dtDocDate;
                        }
                        if (dsModelsList.Tables[0].Rows[0]["eval_date"].ToString() != ""
                           && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["eval_date"].ToString(), out dtDocDate))
                        {
                            objModels.eval_date = dtDocDate;
                        }

                        ViewBag.pers_resp = objGlobaldata.GetHrEmployeeList();

                        CustSatisfactionModelsList objQList = new CustSatisfactionModelsList();
                        objQList.CSList = new List<CustSatisfactionModels>();

                        sSqlstmt = "select id_act,id_cust_satisfaction,action_taken,pers_resp,target_date,resource_req from t_cust_satisfaction_act where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    CustSatisfactionModels OBJModel = new CustSatisfactionModels
                                    {
                                        id_act = dsList.Tables[0].Rows[i]["id_act"].ToString(),
                                        id_cust_satisfaction = dsList.Tables[0].Rows[i]["id_cust_satisfaction"].ToString(),
                                        action_taken = dsList.Tables[0].Rows[i]["action_taken"].ToString(),
                                        pers_resp = dsList.Tables[0].Rows[i]["pers_resp"].ToString(),
                                        resource_req = dsList.Tables[0].Rows[i]["resource_req"].ToString(),

                                    };
                                   
                                    if (dsList.Tables[0].Rows[0]["target_date"].ToString() != ""
                                     && DateTime.TryParse(dsList.Tables[0].Rows[0]["target_date"].ToString(), out dtDocDate))
                                    {
                                        OBJModel.target_date = dtDocDate;
                                    }


                                    objQList.CSList.Add(OBJModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in ImprovementAction: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("CustSatisfactionList");
                                }
                            }
                            ViewBag.objQList = objQList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImprovementAction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ImprovementAction(CustSatisfactionModels objModel, FormCollection form)
        {
            try
            {
                
               
                CustSatisfactionModelsList objQList = new CustSatisfactionModelsList();
                objQList.CSList = new List<CustSatisfactionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    CustSatisfactionModels objModels = new CustSatisfactionModels();
                    if (form["action_taken " + i] != null && form["action_taken " + i] != "")
                    {
                        objModels.id_act = form["id_act " + i];
                        objModels.id_cust_satisfaction = objModel.id_cust_satisfaction;
                        objModels.action_taken = form["action_taken " + i];
                        objModels.pers_resp = form["pers_resp " + i];
                        objModels.resource_req = form["resource_req " + i];
                        DateTime dateValue;
                        if (DateTime.TryParse(form["target_date " + i], out dateValue) == true)
                        {
                            objModels.target_date = dateValue;
                        }
                        objQList.CSList.Add(objModels);
                    }
                }
               
         

                if (objModel.FunUpdateActionInitiated(objModel,objQList))
                {
                    TempData["Successdata"] = "Improvement actions updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImprovementAction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustSatisfactionList");
        }

        [AllowAnonymous]
        public ActionResult ImprovementActionStatusUpdate()
        {
            CustSatisfactionModels objModels = new CustSatisfactionModels();
            try
            {
                if (Request.QueryString["id_cust_satisfaction"] != null && Request.QueryString["id_cust_satisfaction"] != "")
                {
                    string id_cust_satisfaction = Request.QueryString["id_cust_satisfaction"];

                    string sSqlstmt = "select id_cust_satisfaction,branch,Department,Location,cust_name,prod_delivered,contact_details,repeat_enquiries,no_enquiries,repeat_orders,"
                    + "no_orders,enquiries_upload,orders_upload,survey_form,csi,reviewed_by,eval_date,frm_date,to_date,complaint,cust_satisfied from t_cust_satisfaction where id_cust_satisfaction=" + id_cust_satisfaction;

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objModels = new CustSatisfactionModels
                        {
                            id_cust_satisfaction = (dsModelsList.Tables[0].Rows[0]["id_cust_satisfaction"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetDeptNameById(dsModelsList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetLocationNameById(dsModelsList.Tables[0].Rows[0]["Location"].ToString()),
                            cust_name = objGlobaldata.GetCustomerNameById(dsModelsList.Tables[0].Rows[0]["cust_name"].ToString()),
                            prod_delivered = (dsModelsList.Tables[0].Rows[0]["prod_delivered"].ToString()),
                            contact_details = (dsModelsList.Tables[0].Rows[0]["contact_details"].ToString()),
                            repeat_enquiries = (dsModelsList.Tables[0].Rows[0]["repeat_enquiries"].ToString()),
                            no_enquiries = (dsModelsList.Tables[0].Rows[0]["no_enquiries"].ToString()),
                            repeat_orders = (dsModelsList.Tables[0].Rows[0]["repeat_orders"].ToString()),
                            no_orders = (dsModelsList.Tables[0].Rows[0]["no_orders"].ToString()),
                            enquiries_upload = (dsModelsList.Tables[0].Rows[0]["enquiries_upload"].ToString()),
                            orders_upload = (dsModelsList.Tables[0].Rows[0]["orders_upload"].ToString()),
                            survey_form = objGlobaldata.GetDropdownitemById(dsModelsList.Tables[0].Rows[0]["survey_form"].ToString()),
                            csi = (dsModelsList.Tables[0].Rows[0]["csi"].ToString()),
                            reviewed_by = (dsModelsList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            complaint = (dsModelsList.Tables[0].Rows[0]["complaint"].ToString()),
                            cust_satisfied = objGlobaldata.GetDropdownitemById(dsModelsList.Tables[0].Rows[0]["cust_satisfied"].ToString()),

                        };
                        DateTime dtDocDate = new DateTime();
                        if (dsModelsList.Tables[0].Rows[0]["frm_date"].ToString() != ""
                            && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["frm_date"].ToString(), out dtDocDate))
                        {
                            objModels.frm_date = dtDocDate;
                        }
                        if (dsModelsList.Tables[0].Rows[0]["to_date"].ToString() != ""
                           && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["to_date"].ToString(), out dtDocDate))
                        {
                            objModels.to_date = dtDocDate;
                        }
                        if (dsModelsList.Tables[0].Rows[0]["eval_date"].ToString() != ""
                           && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["eval_date"].ToString(), out dtDocDate))
                        {
                            objModels.eval_date = dtDocDate;
                        }

                        ViewBag.Status = objGlobaldata.GetDropdownList("Customer Satisfaction Assessment Status");

                        CustSatisfactionModelsList objQList = new CustSatisfactionModelsList();
                        objQList.CSList = new List<CustSatisfactionModels>();

                        sSqlstmt = "select id_act,id_cust_satisfaction,action_taken,pers_resp,target_date,resource_req,act_status,remarks,update_date from t_cust_satisfaction_act where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    CustSatisfactionModels OBJModel = new CustSatisfactionModels
                                    {
                                        id_act = dsList.Tables[0].Rows[i]["id_act"].ToString(),
                                        id_cust_satisfaction = dsList.Tables[0].Rows[i]["id_cust_satisfaction"].ToString(),
                                        action_taken = dsList.Tables[0].Rows[i]["action_taken"].ToString(),
                                        pers_resp = dsList.Tables[0].Rows[i]["pers_resp"].ToString(),
                                        resource_req = dsList.Tables[0].Rows[i]["resource_req"].ToString(),
                                        act_status = dsList.Tables[0].Rows[i]["act_status"].ToString(),
                                        remarks = dsList.Tables[0].Rows[i]["remarks"].ToString(),

                                    };

                                    if (dsList.Tables[0].Rows[0]["target_date"].ToString() != ""
                                     && DateTime.TryParse(dsList.Tables[0].Rows[0]["target_date"].ToString(), out dtDocDate))
                                    {
                                        OBJModel.target_date = dtDocDate;
                                    }
                                    if (dsList.Tables[0].Rows[0]["update_date"].ToString() != ""
                                    && DateTime.TryParse(dsList.Tables[0].Rows[0]["update_date"].ToString(), out dtDocDate))
                                    {
                                        OBJModel.update_date = dtDocDate;
                                    }
                                    objQList.CSList.Add(OBJModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in ImprovementAction: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("CustSatisfactionList");
                                }
                            }
                            ViewBag.objQList = objQList;
                        }
                        else
                        {
                            TempData["Successdata"] = "No data exists";
                            return RedirectToAction("CustSatisfactionList");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImprovementActionStatusUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModels);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ImprovementActionStatusUpdate(CustSatisfactionModels objModel, FormCollection form)
        {
            try
            {


                CustSatisfactionModelsList objQList = new CustSatisfactionModelsList();
                objQList.CSList = new List<CustSatisfactionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    CustSatisfactionModels objModels = new CustSatisfactionModels();
                    if (form["act_status " + i] != null && form["act_status " + i] != "")
                    {
                        objModels.id_act = form["id_act " + i];
                        objModels.id_cust_satisfaction = objModel.id_cust_satisfaction;
                        objModels.act_status = form["act_status " + i];
                        objModels.remarks = form["remarks " + i];
                 
                        DateTime dateValue;
                        if (DateTime.TryParse(form["update_date " + i], out dateValue) == true)
                        {
                            objModels.update_date = dateValue;
                        }
                        objQList.CSList.Add(objModels);
                    }
                }



                if (objModel.FunUpdateActionList(objQList))
                {
                    TempData["Successdata"] = "Improvement actions status updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImprovementAction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustSatisfactionList");
        }

        [AllowAnonymous]
        public ActionResult CustSatisfactionDetail()
        {
            CustSatisfactionModels objModels = new CustSatisfactionModels();
            try
            {
                if (Request.QueryString["id_cust_satisfaction"] != null && Request.QueryString["id_cust_satisfaction"] != "")
                {
                    string id_cust_satisfaction = Request.QueryString["id_cust_satisfaction"];

                    string sSqlstmt = "select id_cust_satisfaction,branch,Department,Location,cust_name,prod_delivered,contact_details,repeat_enquiries,no_enquiries,repeat_orders,"
                     +"(case when review_status=0 then 'Pending for Review'  when review_status=1 then 'Rejected'  when review_status=2 then 'Approved' end) as review_status,"
                        + "no_orders,enquiries_upload,orders_upload,survey_form,csi,reviewed_by,eval_date,frm_date,to_date,complaint,cust_satisfied,review_status,review_comment,review_date,action_initiated_by,ref_no from t_cust_satisfaction where id_cust_satisfaction=" + id_cust_satisfaction;

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objModels = new CustSatisfactionModels
                        {
                            id_cust_satisfaction = (dsModelsList.Tables[0].Rows[0]["id_cust_satisfaction"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetDeptNameById(dsModelsList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetLocationNameById(dsModelsList.Tables[0].Rows[0]["Location"].ToString()),
                            cust_name = objGlobaldata.GetCustomerNameById(dsModelsList.Tables[0].Rows[0]["cust_name"].ToString()),
                            prod_delivered = (dsModelsList.Tables[0].Rows[0]["prod_delivered"].ToString()),
                            contact_details = (dsModelsList.Tables[0].Rows[0]["contact_details"].ToString()),
                            repeat_enquiries = (dsModelsList.Tables[0].Rows[0]["repeat_enquiries"].ToString()),
                            no_enquiries = (dsModelsList.Tables[0].Rows[0]["no_enquiries"].ToString()),
                            repeat_orders = (dsModelsList.Tables[0].Rows[0]["repeat_orders"].ToString()),
                            no_orders = (dsModelsList.Tables[0].Rows[0]["no_orders"].ToString()),
                            enquiries_upload = (dsModelsList.Tables[0].Rows[0]["enquiries_upload"].ToString()),
                            orders_upload = (dsModelsList.Tables[0].Rows[0]["orders_upload"].ToString()),
                            survey_form = objGlobaldata.GetDropdownitemById(dsModelsList.Tables[0].Rows[0]["survey_form"].ToString()),
                            csi = (dsModelsList.Tables[0].Rows[0]["csi"].ToString()),
                            reviewed_by =objGlobaldata.GetMultiHrEmpNameById(dsModelsList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            complaint = (dsModelsList.Tables[0].Rows[0]["complaint"].ToString()),
                            cust_satisfied = objGlobaldata.GetDropdownitemById(dsModelsList.Tables[0].Rows[0]["cust_satisfied"].ToString()),
                            review_status = (dsModelsList.Tables[0].Rows[0]["review_status"].ToString()),
                            review_comment = (dsModelsList.Tables[0].Rows[0]["review_comment"].ToString()),
                            ref_no = (dsModelsList.Tables[0].Rows[0]["ref_no"].ToString()),
                            action_initiated_by =objGlobaldata.GetEmpHrNameById(dsModelsList.Tables[0].Rows[0]["action_initiated_by"].ToString()),
                        };
                        DateTime dtDocDate = new DateTime();
                        if (dsModelsList.Tables[0].Rows[0]["frm_date"].ToString() != ""
                            && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["frm_date"].ToString(), out dtDocDate))
                        {
                            objModels.frm_date = dtDocDate;
                        }
                        if (dsModelsList.Tables[0].Rows[0]["to_date"].ToString() != ""
                           && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["to_date"].ToString(), out dtDocDate))
                        {
                            objModels.to_date = dtDocDate;
                        }
                        if (dsModelsList.Tables[0].Rows[0]["eval_date"].ToString() != ""
                           && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["eval_date"].ToString(), out dtDocDate))
                        {
                            objModels.eval_date = dtDocDate;
                        }

                        if (dsModelsList.Tables[0].Rows[0]["review_date"].ToString() != ""
                           && DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["review_date"].ToString(), out dtDocDate))
                        {
                            objModels.review_date = dtDocDate;
                        }


                        // QUALITY

                        sSqlstmt = "select id_perf,id_cust_satisfaction,performance,perf_remarks,perf_upload from t_cust_satisfaction_perf where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        ViewBag.objQList = objGlobaldata.Getdetails(sSqlstmt);


                        // DELIVERY

                        sSqlstmt = "select id_delivery,id_cust_satisfaction,delivery,delivery_remarks,delivery_upload from t_cust_satisfaction_delivery where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        ViewBag.objDList = objGlobaldata.Getdetails(sSqlstmt);

                        // COMPLAINT

                        sSqlstmt = "select id_complaint,id_cust_satisfaction,complaint_type,complaint_no,complaint_upload from t_cust_satisfaction_complaints where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        ViewBag.objCList = objGlobaldata.Getdetails(sSqlstmt);

                        // ACTION

                        sSqlstmt = "select id_act,action_taken,pers_resp,target_date,resource_req,act_status,remarks,update_date from t_cust_satisfaction_act where id_cust_satisfaction='" + id_cust_satisfaction + "'";
                        ViewBag.objAList = objGlobaldata.Getdetails(sSqlstmt);

                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustSatisfactionDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objModels);
        }

        [HttpPost]
        public JsonResult UploadMultipleDocument()
        {
            DocumentTrackingModels obj = new DocumentTrackingModels();
            try
            {
                HttpFileCollectionBase upload = Request.Files;
               
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustSatisfaction"), Path.GetFileName(file.FileName));
                    string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                    file.SaveAs(sFilepath + "/" + sFilename);
                    //return Json("~/DataUpload/MgmtDocs/Surveillance/" + "Surveillance" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                    obj.upload = obj.upload + "," + "~/DataUpload/MgmtDocs/CustSatisfaction/" + sFilename;

                }
                obj.upload = obj.upload.Trim(',');
                return Json(obj.upload);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UploadMultipleDocument: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(obj);
        }
    }
}