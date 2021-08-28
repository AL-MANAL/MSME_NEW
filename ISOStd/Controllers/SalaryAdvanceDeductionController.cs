using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using PagedList;
using PagedList.Mvc;

namespace ISOStd.Controllers
{
    public class SalaryAdvanceDeductionController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        EmployeeMasterModels objEmpMaster = new EmployeeMasterModels();

        //
        // GET: /SalaryAdvanceDeduction/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /SalaryAdvanceDeduction/AddSalaryAdvance
        [AllowAnonymous]
        public ActionResult AddSalaryAdvance()
        {
            try
            {
                if (Request.QueryString["EmpId"] != null && Request.QueryString["EmpId"] != "")
                {
                    SalaryAdvanceDeductionModels objAdv = new SalaryAdvanceDeductionModels();

                    string sEmp_no = Request.QueryString["EmpId"];
                    ViewBag.EmpId = sEmp_no;
                    ViewBag.EmpName = objEmpMaster.GetEmpNameById(sEmp_no);

                    ViewBag.Installments = objAdv.GetInstallmentDuration(sEmp_no);

                    return View();
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSalaryAdvance: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList");
            
        }

           //
        // POST: /SalaryAdvanceDeduction/AddSalaryAdvance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSalaryAdvance(SalaryAdvanceDeductionModels objSalaryAdvance, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                if (DateTime.TryParse(form["TransDate"], out dateValue) == true)
                {
                    objSalaryAdvance.TransDate = dateValue;
                }

                objSalaryAdvance.EmpId = form["EmpId"];
                if (objSalaryAdvance.FunAddAdvanceAmt(objSalaryAdvance))
                {
                    TempData["Successdata"] = "Added Advance details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                return RedirectToAction("SalaryAdvanceList", new { EmpId = objSalaryAdvance.EmpId });
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSalaryAdvance: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList", "EmployeeDetails");

        }

        //
        // GET: /SalaryAdvanceDeduction/SalaryAdvanceList
        [AllowAnonymous]
        public ActionResult SalaryAdvanceList(string SearchText, int? page)
        {
            SalaryAdvanceDeductionModelsList objSalaryAdvanceList = new SalaryAdvanceDeductionModelsList();
            objSalaryAdvanceList.lstSalaryAdvanceDeduction = new List<SalaryAdvanceDeductionModels>();

            try
            {
                if (Request.QueryString["EmpId"] != null && Request.QueryString["EmpId"] != "")
                {
                    string sEmp_no = Request.QueryString["EmpId"];

                    ViewBag.EmpName = objGlobaldata.GetEmpNameById(sEmp_no);
                    ViewBag.EmpId = (sEmp_no);
                    string sSqlstmt = "SELECT Advance_AmtId, Adv_Amt, EmpId, Num_Installment, Deduction_Amt, Details, Balance_Amt, TransDate, ProcessedBy FROM t_advance_amt "
                        + " where EmpId='" + sEmp_no + "'";

                    //if (SearchText != null && SearchText != "")
                    //{
                    //    ViewBag.SearchText = SearchText;
                    //    sSqlstmt = sSqlstmt + " and (emp_firstname ='" + SearchText + "' or emp_firstname like '" + SearchText + "%')";
                    //}

                    sSqlstmt = sSqlstmt + " order by TransDate desc";
                    DataSet dsAdvanceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAdvanceList.Tables.Count > 0 && dsAdvanceList.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.EmpId = sEmp_no;
                        for (int i = 0; i < dsAdvanceList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                DateTime dtTransDate = Convert.ToDateTime(dsAdvanceList.Tables[0].Rows[i]["TransDate"].ToString());
                                SalaryAdvanceDeductionModels objAdvance = new SalaryAdvanceDeductionModels
                                {
                                    Advance_AmtId = dsAdvanceList.Tables[0].Rows[i]["Advance_AmtId"].ToString(),
                                    Adv_Amt = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Adv_Amt"].ToString()),
                                    EmpId = objEmpMaster.GetEmpNameById(dsAdvanceList.Tables[0].Rows[i]["EmpId"].ToString()),
                                    Num_Installment = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Num_Installment"].ToString()),
                                    Deduction_Amt = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Deduction_Amt"].ToString()),
                                    Details = dsAdvanceList.Tables[0].Rows[i]["Details"].ToString(),
                                    Balance_Amt = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Balance_Amt"].ToString()),
                                    TransDate = dtTransDate,
                                    ProcessedBy = objGlobaldata.GetEmpNameById(dsAdvanceList.Tables[0].Rows[i]["ProcessedBy"].ToString())
                                };

                                objSalaryAdvanceList.lstSalaryAdvanceDeduction.Add(objAdvance);
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                    //else
                    //{
                    //    TempData["alertdata"] = "No Data exists";
                    //}
                    //return RedirectToAction("EmployeeList", "EmployeeDetails");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objSalaryAdvanceList.lstSalaryAdvanceDeduction.ToList().ToPagedList(page ?? 1, 10));
        }

        //
        // GET: /SalaryAdvanceDeduction/SalaryAdvanceDetails
        [AllowAnonymous]
        public ActionResult SalaryAdvanceDetails()
        {
            try
            {
                if (Request.QueryString["Advance_AmtId"] != null && Request.QueryString["Advance_AmtId"] != ""
                    && Request.QueryString["EmpId"] != null && Request.QueryString["EmpId"] != "")
                {
                    string sEmpId = Request.QueryString["EmpId"];
                    string sSqlstmt = "SELECT Advance_AmtId, Adv_Amt, EmpId, Num_Installment, Deduction_Amt, Details, Balance_Amt, TransDate, ProcessedBy FROM t_advance_amt "
                        + " where Advance_AmtId='" + Request.QueryString["Advance_AmtId"] + "'";

                    DataSet dsAdvanceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAdvanceList.Tables.Count > 0 && dsAdvanceList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtTransDate = Convert.ToDateTime(dsAdvanceList.Tables[0].Rows[0]["TransDate"].ToString());
                        SalaryAdvanceDeductionModels objAdvance = new SalaryAdvanceDeductionModels
                        {
                            Advance_AmtId = dsAdvanceList.Tables[0].Rows[0]["Advance_AmtId"].ToString(),
                            Adv_Amt = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Adv_Amt"].ToString()),
                            EmpId = objEmpMaster.GetEmpNameById(dsAdvanceList.Tables[0].Rows[0]["EmpId"].ToString()),
                            Num_Installment = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Num_Installment"].ToString()),
                            Deduction_Amt = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Deduction_Amt"].ToString()),
                            Details = dsAdvanceList.Tables[0].Rows[0]["Details"].ToString(),
                            Balance_Amt = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Balance_Amt"].ToString()),
                            TransDate = dtTransDate,
                            ProcessedBy = objGlobaldata.GetEmpNameById(dsAdvanceList.Tables[0].Rows[0]["ProcessedBy"].ToString())
                        };

                        ViewBag.EmpId = sEmpId;
                        return View(objAdvance);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("SalaryAdvanceList", new { EmpId = sEmpId });
                    }                    
                }
                else
                {
                    TempData["alertdata"] = "Advance Id cannot be Null";
                    return RedirectToAction("EmployeeList", "EmployeeDetails");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SalaryAdvanceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList", "EmployeeDetails");
        }


        //
        // GET: /SalaryAdvanceDeduction/SalaryAdvanceEdit
        [AllowAnonymous]
        public ActionResult SalaryAdvanceEdit()
        {
            try
            {
                if (Request.QueryString["Advance_AmtId"] != null && Request.QueryString["Advance_AmtId"] != ""
                    && Request.QueryString["EmpId"] != null && Request.QueryString["EmpId"] != "")
                {
                    string sEmpId = Request.QueryString["EmpId"];
                    string sSqlstmt = "SELECT Advance_AmtId, Adv_Amt, EmpId, Num_Installment, Deduction_Amt, Details, Balance_Amt, TransDate, ProcessedBy FROM t_advance_amt "
                        + " where Advance_AmtId='" + Request.QueryString["Advance_AmtId"] + "'";

                    DataSet dsAdvanceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAdvanceList.Tables.Count > 0 && dsAdvanceList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtTransDate = Convert.ToDateTime(dsAdvanceList.Tables[0].Rows[0]["TransDate"].ToString());
                        SalaryAdvanceDeductionModels objAdvance = new SalaryAdvanceDeductionModels
                        {
                            Advance_AmtId = dsAdvanceList.Tables[0].Rows[0]["Advance_AmtId"].ToString(),
                            Adv_Amt = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Adv_Amt"].ToString()),
                            EmpId = objEmpMaster.GetEmpNameById(dsAdvanceList.Tables[0].Rows[0]["EmpId"].ToString()),
                            Num_Installment = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Num_Installment"].ToString()),
                            Deduction_Amt = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Deduction_Amt"].ToString()),
                            Details = dsAdvanceList.Tables[0].Rows[0]["Details"].ToString(),
                            Balance_Amt = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Balance_Amt"].ToString()),
                            TransDate = dtTransDate,
                            ProcessedBy = dsAdvanceList.Tables[0].Rows[0]["ProcessedBy"].ToString()
                        };

                        ViewBag.EmpId = sEmpId;

                        ViewBag.Installments = objAdvance.GetInstallmentDuration(sEmpId);
                        return View(objAdvance);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("SalaryAdvanceList", new { EmpId = sEmpId });
                    }
                }
                else
                {
                    TempData["alertdata"] = "Advance Id cannot be Null";
                    return RedirectToAction("EmployeeList", "EmployeeDetails");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SalaryAdvanceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList", "EmployeeDetails");
        }


        //
        // POST: /SalaryAdvanceDeduction/SalaryAdvanceEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalaryAdvanceEdit(SalaryAdvanceDeductionModels objSalaryAdvance, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                if (DateTime.TryParse(form["TransDate"], out dateValue) == true)
                {
                    objSalaryAdvance.TransDate = dateValue;
                }

                objSalaryAdvance.EmpId = form["EmpId"];
                objSalaryAdvance.ProcessedBy = objGlobaldata.GetCurrentUserSession().empid;
                if (objSalaryAdvance.FunUpdateAdvanceAmt(objSalaryAdvance))
                {
                    TempData["Successdata"] = "Advance details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                return RedirectToAction("SalaryAdvanceList", new { EmpId = objSalaryAdvance.EmpId });
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSalaryAdvance: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeList", "EmployeeDetails");

        }
    }
}
