using ISOStd.Filters;
using ISOStd.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class CustomerAuditController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();
        private CustomerAuditModels objEAudit = new CustomerAuditModels();

        public CustomerAuditController()
        {
            ViewBag.Menutype = "Audit";
            ViewBag.SubMenutype = "CustomerAuditList";
        }

        // GET: /CustomerAudit/
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AddCustomerAudit()
        {
            return InitilizeAddCustomerAudit();
        }

        private ActionResult InitilizeAddCustomerAudit()
        {
            CustomerAuditModels objCust = new CustomerAuditModels();
            try
            {
                objCust.branch = objGlobaldata.GetCurrentUserSession().division;
                objCust.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objCust.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objCust.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objCust.branch);

                ViewBag.NC_status = objGlobaldata.GetDropdownList("Audit Status");
                ViewBag.CustList = objGlobaldata.GetCustomerListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustomerAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCust);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomerAudit(CustomerAuditModels objCustomerAudit, FormCollection form)
        {
            //if (ModelState.IsValid)
            try
            {
                //if (objCustomerAudit != null)
                {
                    objCustomerAudit.NC_status = form["NC_status"];
                    objCustomerAudit.CustomerName = form["CustomerName"];
                    objCustomerAudit.Department = form["Department"];
                    objCustomerAudit.branch = form["branch"];
                    objCustomerAudit.Location = form["Location"];

                    DateTime dateValue;

                    if (DateTime.TryParse(form["AuditDate"], out dateValue) == true)
                    {
                        objCustomerAudit.AuditDate = dateValue;
                    }

                    CustomerAuditModelsList objCustomerAuditModelsList = new CustomerAuditModelsList();
                    objCustomerAuditModelsList.CustomerAuditMList = new List<CustomerAuditModels>();
                    int iitemcnt = 0;
                    if (form["itemcnt"] != null && int.TryParse(form["itemcnt"].ToString(), out iitemcnt))
                    {
                        for (int i = 0; i < iitemcnt; i++)
                        {
                            CustomerAuditModels objCustomerAuditModels = new CustomerAuditModels();

                            objCustomerAuditModels.NCNo = form["NCNo" + i];
                            objCustomerAuditModels.AuditFindingDesc = form["AuditFindingDesc" + i];
                            objCustomerAuditModels.CorrectionTaken = form["CorrectionTaken" + i];
                            if (DateTime.TryParse(form["CorrectionDate" + i], out dateValue) == true)
                            {
                                objCustomerAuditModels.CorrectionDate = dateValue;
                            }
                            objCustomerAuditModels.ProposedcorrectiveAction = form["ProposedcorrectiveAction" + i];
                            if (DateTime.TryParse(form["CorrectiveActionDate" + i], out dateValue) == true)
                            {
                                objCustomerAuditModels.CorrectiveActionDate = dateValue;
                            }
                            objCustomerAuditModels.Action_taken_by = form["Action_taken_by" + i];
                            objCustomerAuditModels.NC_status = form["NC_status" + i];
                            objCustomerAuditModels.reviewed_by = form["reviewed_by" + i];

                            objCustomerAuditModelsList.CustomerAuditMList.Add(objCustomerAuditModels);
                        }
                    }
                    objCustomerAudit.FunAddCustomerAudit(objCustomerAudit, objCustomerAuditModelsList);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustomerAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerAuditList");
        }

        [AllowAnonymous]
        public JsonResult CustomerAuditDocDelete(FormCollection form)
        {
            try
            {
                if (form["CustAuditID"] != null && form["CustAuditID"] != "")
                {
                    CustomerAuditModels Doc = new CustomerAuditModels();
                    string sCustAuditID = form["CustAuditID"];

                    if (Doc.FunDeleteCustomerAuditDoc(sCustAuditID))
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
                    TempData["alertdata"] = "Access Denied";
                    return Json("Access Denied");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerAuditDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult CustomerAuditList(string branch_name)
        {
            CustomerAuditModelsList CustomerAuditModelsList = new CustomerAuditModelsList();
            CustomerAuditModelsList.CustomerAuditMList = new List<CustomerAuditModels>();

            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                string sSqlstmt = "SELECT CustAuditID, CustomerName, Customer_Audit_Team, AuditNum, AuditDate,branch,Department,Location" +
                    " FROM t_customer_audit where Active=1";
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by CustomerName";

                DataSet dsCertificationBodyList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsCertificationBodyList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsCertificationBodyList.Tables[0].Rows.Count; i++)
                    {
                        DateTime AuditDateVal = Convert.ToDateTime(dsCertificationBodyList.Tables[0].Rows[i]["AuditDate"].ToString());

                        try
                        {
                            CustomerAuditModels objCustomerAuditModels = new CustomerAuditModels
                            {
                                CustAuditID = dsCertificationBodyList.Tables[0].Rows[i]["CustAuditID"].ToString(),
                                AuditNum = dsCertificationBodyList.Tables[0].Rows[i]["AuditNum"].ToString(),
                                Customer_Audit_Team = dsCertificationBodyList.Tables[0].Rows[i]["Customer_Audit_Team"].ToString(),
                                CustomerName = objGlobaldata.GetCustomerNameById(dsCertificationBodyList.Tables[0].Rows[i]["CustomerName"].ToString()),
                                AuditDate = AuditDateVal,
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCertificationBodyList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsCertificationBodyList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsCertificationBodyList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            CustomerAuditModelsList.CustomerAuditMList.Add(objCustomerAuditModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustomerAuditList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerAuditList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(CustomerAuditModelsList.CustomerAuditMList.ToList());
        }

        [AllowAnonymous]
        public JsonResult CustomerAuditListSearch(string branch_name)
        {
            CustomerAuditModelsList CustomerAuditModelsList = new CustomerAuditModelsList();
            CustomerAuditModelsList.CustomerAuditMList = new List<CustomerAuditModels>();

            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                string sSqlstmt = "SELECT CustAuditID, CustomerName, Customer_Audit_Team, AuditNum, AuditDate,branch,Department,Location" +
                    " FROM t_customer_audit where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by CustomerName";

                DataSet dsCertificationBodyList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsCertificationBodyList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsCertificationBodyList.Tables[0].Rows.Count; i++)
                    {
                        DateTime AuditDateVal = Convert.ToDateTime(dsCertificationBodyList.Tables[0].Rows[i]["AuditDate"].ToString());

                        try
                        {
                            CustomerAuditModels objCustomerAuditModels = new CustomerAuditModels
                            {
                                CustAuditID = dsCertificationBodyList.Tables[0].Rows[i]["CustAuditID"].ToString(),
                                AuditNum = dsCertificationBodyList.Tables[0].Rows[i]["AuditNum"].ToString(),
                                Customer_Audit_Team = dsCertificationBodyList.Tables[0].Rows[i]["Customer_Audit_Team"].ToString(),
                                CustomerName = objGlobaldata.GetCustomerNameById(dsCertificationBodyList.Tables[0].Rows[i]["CustomerName"].ToString()),
                                AuditDate = AuditDateVal,
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCertificationBodyList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsCertificationBodyList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsCertificationBodyList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            CustomerAuditModelsList.CustomerAuditMList.Add(objCustomerAuditModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustomerAuditListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerAuditListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult CustomerAuditDetails()
        {
            CustomerAuditModels objCustomerAuditModels = new CustomerAuditModels();
            if (Request.QueryString["CustAuditID"] != null)
            {
                string sCustAuditID = Request.QueryString["CustAuditID"];
                try
                {
                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                    string sSqlstmt = "SELECT CustAuditID, AuditNum,CustomerName,Customer_Audit_Team, AuditDate,branch,Department,Location" +
                        " FROM t_customer_audit where CustAuditID='" + sCustAuditID + "'";

                    DataSet dsCustomerAuditModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustomerAuditModelsList.Tables.Count > 0 && dsCustomerAuditModelsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime AuditDateVal = Convert.ToDateTime(dsCustomerAuditModelsList.Tables[0].Rows[0]["AuditDate"].ToString());

                        objCustomerAuditModels = new CustomerAuditModels
                        {
                            CustAuditID = dsCustomerAuditModelsList.Tables[0].Rows[0]["CustAuditID"].ToString(),
                            AuditNum = dsCustomerAuditModelsList.Tables[0].Rows[0]["AuditNum"].ToString(),
                            Customer_Audit_Team = dsCustomerAuditModelsList.Tables[0].Rows[0]["Customer_Audit_Team"].ToString(),
                            CustomerName = objGlobaldata.GetCustomerNameById(dsCustomerAuditModelsList.Tables[0].Rows[0]["CustomerName"].ToString()),
                            AuditDate = AuditDateVal,
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCustomerAuditModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsCustomerAuditModelsList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsCustomerAuditModelsList.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        sSqlstmt = "select CustAudtTransId, CustAuditID, NCNo, AuditFindingDesc, CorrectionTaken, CorrectionDate, ProposedcorrectiveAction,"
                           + " CorrectiveActionDate, Action_taken_by, NC_status, reviewed_by from t_customer_audit_trans where CustAuditID=" + objCustomerAuditModels.CustAuditID
                           + " order by CustAudtTransId desc";
                        ViewBag.FindingsDetails = objGlobaldata.Getdetails(sSqlstmt);
                        //objCertificationBodyList.ExternalAuditList.Add(objExternalAuditorModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerAuditList");
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in CustomerAuditDetails: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            else
            {
                TempData["alertdata"] = "Customer Audit id cannot be empty";
                return RedirectToAction("CustomerAuditList");
            }
            return View(objCustomerAuditModels);
        }

        [AllowAnonymous]
        public ActionResult CustomerAuditInfo(int id)
        {
            CustomerAuditModels objCustomerAuditModels = new CustomerAuditModels();

            try
            {
                string sSqlstmt = "SELECT CustAuditID, AuditNum,CustomerName,Customer_Audit_Team, AuditDate,branch,Department,Location" +
                " FROM t_customer_audit where CustAuditID='" + id + "'";

                DataSet dsCustomerAuditModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCustomerAuditModelsList.Tables.Count > 0 && dsCustomerAuditModelsList.Tables[0].Rows.Count > 0)
                {
                    DateTime AuditDateVal = Convert.ToDateTime(dsCustomerAuditModelsList.Tables[0].Rows[0]["AuditDate"].ToString());

                    objCustomerAuditModels = new CustomerAuditModels
                    {
                        CustAuditID = dsCustomerAuditModelsList.Tables[0].Rows[0]["CustAuditID"].ToString(),
                        AuditNum = dsCustomerAuditModelsList.Tables[0].Rows[0]["AuditNum"].ToString(),
                        Customer_Audit_Team = dsCustomerAuditModelsList.Tables[0].Rows[0]["Customer_Audit_Team"].ToString(),
                        CustomerName = objGlobaldata.GetCustomerNameById(dsCustomerAuditModelsList.Tables[0].Rows[0]["CustomerName"].ToString()),
                        AuditDate = AuditDateVal,
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCustomerAuditModelsList.Tables[0].Rows[0]["branch"].ToString()),
                        Department = objGlobaldata.GetMultiDeptNameById(dsCustomerAuditModelsList.Tables[0].Rows[0]["Department"].ToString()),
                        Location = objGlobaldata.GetDivisionLocationById(dsCustomerAuditModelsList.Tables[0].Rows[0]["Location"].ToString()),
                    };

                    sSqlstmt = "select CustAudtTransId, CustAuditID, NCNo, AuditFindingDesc, CorrectionTaken, CorrectionDate, ProposedcorrectiveAction,"
                       + " CorrectiveActionDate, Action_taken_by, NC_status, reviewed_by from t_customer_audit_trans where CustAuditID=" + objCustomerAuditModels.CustAuditID
                       + " order by CustAudtTransId desc";
                    ViewBag.FindingsDetails = objGlobaldata.Getdetails(sSqlstmt);
                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("CustomerAuditList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerAuditDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCustomerAuditModels);
        }

        [AllowAnonymous]
        public ActionResult CustomerAuditEdit()
        {
            CustomerAuditModels objCustomerAuditModels = new CustomerAuditModels();

            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
            if (Request.QueryString["CustAuditID"] != null)
            {
                string sCustAuditID = Request.QueryString["CustAuditID"];
                try
                {
                    ViewBag.CustList = objGlobaldata.GetCustomerListbox();
                    ViewBag.NC_status = objGlobaldata.GetDropdownList("Audit Status");
                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                    string sSqlstmt = "SELECT  CustAuditID, AuditNum,CustomerName,Customer_Audit_Team,AuditDate,branch,Department,Location" +
                        " FROM t_customer_audit where CustAuditID=" + sCustAuditID;

                    DataSet dsCustomerAuditModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustomerAuditModelsList.Tables.Count > 0 && dsCustomerAuditModelsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime AuditDateVal = Convert.ToDateTime(dsCustomerAuditModelsList.Tables[0].Rows[0]["AuditDate"].ToString());

                        objCustomerAuditModels = new CustomerAuditModels
                        {
                            CustAuditID = dsCustomerAuditModelsList.Tables[0].Rows[0]["CustAuditID"].ToString(),
                            AuditNum = dsCustomerAuditModelsList.Tables[0].Rows[0]["AuditNum"].ToString(),
                            Customer_Audit_Team = dsCustomerAuditModelsList.Tables[0].Rows[0]["Customer_Audit_Team"].ToString(),
                            CustomerName = objGlobaldata.GetCustomerNameById(dsCustomerAuditModelsList.Tables[0].Rows[0]["CustomerName"].ToString()),
                            AuditDate = AuditDateVal,
                            branch = (dsCustomerAuditModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsCustomerAuditModelsList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = (dsCustomerAuditModelsList.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsCustomerAuditModelsList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsCustomerAuditModelsList.Tables[0].Rows[0]["branch"].ToString());

                        sSqlstmt = "select CustAudtTransId, CustAuditID, NCNo, AuditFindingDesc, CorrectionTaken, CorrectionDate, ProposedcorrectiveAction,"
                               + " CorrectiveActionDate, Action_taken_by, NC_status, reviewed_by from t_customer_audit_trans where CustAuditID=" + objCustomerAuditModels.CustAuditID
                               + " order by CustAudtTransId desc";
                        ViewBag.FindingsDetails = objGlobaldata.Getdetails(sSqlstmt);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerAuditList");
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in CustomerAuditEdit: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            else
            {
                TempData["alertdata"] = "Customer Audit id cannot be empty";
                return RedirectToAction("CustomerAuditList");
            }

            return View(objCustomerAuditModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerAuditEdit(CustomerAuditModels objCustomerAuditModels, FormCollection form)
        {
            try
            {
                if (Request["button"].Equals("Audit Update"))
                {
                    objCustomerAuditModels.CustAuditID = form["CustAuditID"];
                    objCustomerAuditModels.CustomerName = form["CustomerName"];
                    objCustomerAuditModels.Department = form["Department"];
                    objCustomerAuditModels.branch = form["branch"];
                    objCustomerAuditModels.Location = form["Location"];

                    DateTime dateValue;

                    if (DateTime.TryParse(form["AuditDate"], out dateValue) == true)
                    {
                        objCustomerAuditModels.AuditDate = dateValue;
                    }

                    if (objEAudit.FunUpdateCustomerAudit(objCustomerAuditModels))
                    {
                        TempData["Successdata"] = "Customer Audit details updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }

                    ;
                }
                else if (Request["button"].Equals("Save"))
                {
                    if (CustomerAuditFindingsAdd(objCustomerAuditModels, form))
                    {
                        TempData["Successdata"] = "Added Customer Audit findings successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    if (CustomerAuditFindingsUpdate(objCustomerAuditModels, form))
                    {
                        TempData["Successdata"] = "Customer Compliant updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerAuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerAuditList");//View();
        }

        public bool CustomerAuditFindingsUpdate(CustomerAuditModels objCustomerAuditModels, FormCollection form)
        {
            try
            {
                objCustomerAuditModels.CustAuditID = form["CustAuditID"];
                objCustomerAuditModels.CustAudtTransId = form["CustAudtTransId"];
                //ExternalAuditModels objExternalAuditModels = new ExternalAuditModels();

                return objCustomerAuditModels.FunUpdateCustomerAuditFindings(objCustomerAuditModels);
            }
            catch (Exception ex)
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                objGlobaldata.AddFunctionalLog("Exception in CustomerAuditFindingsUpdate: " + ex.ToString());
            }

            return false;
        }

        public bool CustomerAuditFindingsAdd(CustomerAuditModels objCustomerAuditModels, FormCollection form)
        {
            try
            {
                CustomerAuditModelsList objCustomerAuditModelsList = new CustomerAuditModelsList();
                objCustomerAuditModelsList.CustomerAuditMList = new List<CustomerAuditModels>();

                objCustomerAuditModels.CustAuditID = form["CustAuditID"];
                // objExternalAuditModels.ExternalAuditID = form["ExtAuditTransID"];

                objCustomerAuditModelsList.CustomerAuditMList.Add(objCustomerAuditModels);
                //ExternalAuditModels objExternalAuditModels = new ExternalAuditModels();
                return objCustomerAuditModels.FunAddCustomerAuditFindings(objCustomerAuditModelsList, Convert.ToInt16(objCustomerAuditModels.CustAuditID));
            }
            catch (Exception ex)
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                objGlobaldata.AddFunctionalLog("Exception in CustomerAuditFindingsAdd: " + ex.ToString());
            }
            return false;
        }
    }
}