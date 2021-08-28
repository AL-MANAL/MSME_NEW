using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using PagedList;
using PagedList.Mvc;
using System.Globalization;

namespace ISOStd.Controllers
{
    public class SalaryStatementController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        EmployeeMasterModels objEmpMaster = new EmployeeMasterModels();
        //
        // GET: /SalaryStatement/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /SalaryStatement/GenerateSalaryStatement
        [AllowAnonymous]
        public ActionResult GenerateSalaryStatement()
        {
            return View();
        }

        // GET: /SalaryStatement/GenerateSalaryStatement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateSalaryStatement(SalaryStatementModels objSalaryStatement, FormCollection form)
        {
            try
            {
                 DateTime dateValue;
                if (DateTime.TryParse(form["SalaryFor_Month"], out dateValue) == true)
                {
                    objSalaryStatement.SalaryFor_Month = dateValue;
                } 
                objSalaryStatement.ProcessedBy=objGlobaldata.GetCurrentUserSession().empid;

                if (objSalaryStatement.FunGenerateSalaryStmt(objSalaryStatement))
                {
                    TempData["Successdata"] = "Salary statement generated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                return RedirectToAction("SalaryStatementList", new { SalaryFor_Month = objSalaryStatement.SalaryFor_Month.ToString("MMMM yyyy") });
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateSalaryStatement: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View();
        }

        //
        // GET: /SalaryStatement/SalaryStatementList
        [AllowAnonymous]
        public ActionResult SalaryStatementList(string SearchText, string SalaryFor_Month, FormCollection form, string command, int? page)
        {           
           
            SalaryStatementModelsList objSalaryStatementList = new SalaryStatementModelsList();
            objSalaryStatementList.lstSalaryStatement = new List<SalaryStatementModels>();

            try
            {
                string sSqlstmt = "";

                if (command != null && command.Equals("Print"))
                {
                    Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

                    foreach (var key in Request.Cookies.AllKeys)
                    {
                        cookieCollection.Add(key, Request.Cookies.Get(key).Value);
                    }
                    return PrintSalaryStatement(SearchText, SalaryFor_Month );


                    //return new Rotativa.ActionAsPdf("PrintSalaryStatement", new { SearchText = SearchText, SalaryFor_Month = SalaryFor_Month })
                    //{
                    //    FileName = SalaryFor_Month + "SalaryStatement.pdf",
                    //    Cookies = cookieCollection,
                    //    PageOrientation = Rotativa.Options.Orientation.Landscape,
                    //    PageSize = Rotativa.Options.Size.A3
                    //};
                }
                else if (Request.QueryString["SalaryFor_Month"] != null && Request.QueryString["SalaryFor_Month"] != "")                    
                {
                    //if (DateTime.TryParse(form["SalaryFor_Month"], out dateValue) == true)
                    //{
 
                    //}
                    string[] SalaryDate=SalaryFor_Month.Split(' ');

                    string month="", year="";
                    if (SalaryDate.Length > 0)
                    {
                        ViewBag.SalaryFor_Month = SalaryFor_Month;
                        month = DateTime.ParseExact(SalaryDate[0], "MMMM", CultureInfo.CurrentCulture).Month.ToString();
                        year = SalaryDate[1];

                        //dateTime = DateTime.ParseExact(SalaryFor_Month, "MMM yyyy", new CultureInfo("en-US"), DateTimeStyles.None);

                       // if (DateTime.TryParse(Request.QueryString["SalaryFor_Month"], out dateValue) == true)
                        {
                            sSqlstmt = "SELECT EmpId, emp_id as Emp_Code, emp_firstname, Emp_info_no, Designation, Basic_Salary, Acc_allow, Other_allow, Food_allow, Transport_allow, SalaryStmtId,"
                                     + "SalaryFor_Month, Other_Earnings, Bonus, Salary_Days, Leave_Days, Normal_OT_Hrs, Friday_OT_Hrs, Public_Holiday_OT_Hrs, Other_Income,"
                                     + " Salary_Adv, ProcessedBy, Misslliance_Deduction FROM t_emp_salarystmt as tempSal, t_hr_employee as temp where tempSal.EmpId=temp.emp_no and temp.emp_status=1"
                                     + " and (month(SalaryFor_Month) = '" + month + "' and year(SalaryFor_Month)='" + year + "')";
                        }
                    }

                }
                 
                else
                {
                    sSqlstmt = "SELECT (SalaryFor_Month) as SalaryFor_Month, EmpId, emp_id as Emp_Code, emp_firstname, Emp_info_no, Designation, Basic_Salary, "
                            + " Acc_allow, Other_allow, Food_allow, Transport_allow, SalaryStmtId, Other_Earnings, Bonus, Salary_Days, Leave_Days, Normal_OT_Hrs, "
                            + " Friday_OT_Hrs, Public_Holiday_OT_Hrs, Other_Income, Salary_Adv, ProcessedBy, Misslliance_Deduction FROM t_emp_salarystmt as tempSal, t_hr_employee as temp "
                            + " where tempSal.EmpId=temp.emp_no and temp.emp_status=1 and year(salaryfor_month)=year(current_date - interval 1 month) and"
                            + " month(salaryfor_month)= month(current_date - interval 1 month)  ";
                           
                }

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSqlstmt = sSqlstmt + " and (emp_firstname ='" + SearchText + "' or emp_firstname like '" + SearchText + "%')";
                }

                sSqlstmt = sSqlstmt + " order by Emp_Code desc";

                DataSet dsAdvanceList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsAdvanceList.Tables.Count > 0 && dsAdvanceList.Tables[0].Rows.Count > 0)
                {
                    //ViewBag.EmpId = siYear;
                    for (int i = 0; i < dsAdvanceList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            DateTime dtSalaryFor_Month = Convert.ToDateTime(dsAdvanceList.Tables[0].Rows[i]["SalaryFor_Month"].ToString());
                            SalaryStatementModels objSalaryStmt = new SalaryStatementModels
                            {
                                EmpId = dsAdvanceList.Tables[0].Rows[i]["EmpId"].ToString(),
                                Emp_Code = (dsAdvanceList.Tables[0].Rows[i]["Emp_Code"].ToString()),
                                emp_firstname = (dsAdvanceList.Tables[0].Rows[i]["emp_firstname"].ToString()),
                                Emp_info_no = (dsAdvanceList.Tables[0].Rows[i]["Emp_info_no"].ToString()),
                                Designation = dsAdvanceList.Tables[0].Rows[i]["Designation"].ToString(),
                                Basic_Salary = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Basic_Salary"].ToString()),
                                Acc_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Acc_allow"].ToString()),
                                Other_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Other_allow"].ToString()),
                                Food_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Food_allow"].ToString()),
                                Transport_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Transport_allow"].ToString()),
                                SalaryStmtId = (dsAdvanceList.Tables[0].Rows[i]["SalaryStmtId"].ToString()),
                                SalaryFor_Month = dtSalaryFor_Month,
                                Other_Earnings = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Other_Earnings"].ToString()),
                                Bonus = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Bonus"].ToString()),
                                Salary_Days = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Salary_Days"].ToString()),
                                Leave_Days = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Leave_Days"].ToString()),
                                Normal_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Normal_OT_Hrs"].ToString()),
                                Friday_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Friday_OT_Hrs"].ToString()),
                                Public_Holiday_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Friday_OT_Hrs"].ToString()),
                                Other_Income = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Other_Income"].ToString()),
                                Salary_Adv = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Salary_Adv"].ToString()),
                                ProcessedBy = objGlobaldata.GetEmpNameById(dsAdvanceList.Tables[0].Rows[i]["ProcessedBy"].ToString()),
                                Misslliance_Deduction = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Misslliance_Deduction"].ToString())
                            };

                            objSalaryStatementList.lstSalaryStatement.Add(objSalaryStmt);
                        }
                        catch (Exception ex)
                        { }
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
                objGlobaldata.AddFunctionalLog("Exception in SalaryStatementList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objSalaryStatementList.lstSalaryStatement.ToList().ToPagedList(page ?? 1, 10));
        }

        //
        // GET: /SalaryStatement/PrintPreviewSalaryStatement
        [AllowAnonymous]
        public ActionResult PrintPreviewSalaryStatement(string SearchText, string SalaryFor_Month)
        {
            SalaryStatementModelsList objSalaryStatementList = new SalaryStatementModelsList();
            objSalaryStatementList.lstSalaryStatement = new List<SalaryStatementModels>();

            try
            {
                string sSqlstmt = "";

                if (Request.QueryString["SalaryFor_Month"] != null && Request.QueryString["SalaryFor_Month"] != "")
                {
                    string[] SalaryDate = SalaryFor_Month.Split(' ');

                    string month = "", year = "";
                    if (SalaryDate.Length > 0)
                    {
                        ViewBag.SalaryFor_Month = SalaryFor_Month;
                        month = DateTime.ParseExact(SalaryDate[0], "MMMM", CultureInfo.CurrentCulture).Month.ToString();
                        year = SalaryDate[1];

                        sSqlstmt = "SELECT EmpId, emp_id as Emp_Code, emp_firstname, Emp_info_no, Designation, Basic_Salary, Acc_allow, Other_allow, Food_allow, Transport_allow, SalaryStmtId,"
                                 + "SalaryFor_Month, Other_Earnings, Bonus, Salary_Days, Leave_Days, Normal_OT_Hrs, Friday_OT_Hrs, Public_Holiday_OT_Hrs, Other_Income,"
                                 + " Salary_Adv, ProcessedBy, Misslliance_Deduction FROM t_emp_salarystmt as tempSal, t_hr_employee as temp where tempSal.EmpId=temp.emp_no and temp.emp_status=1"
                                 + " and (month(SalaryFor_Month) = '" + month + "' and year(SalaryFor_Month)='" + year + "')";
                    }
                }
                else
                {
                    sSqlstmt = "SELECT (SalaryFor_Month) as SalaryFor_Month, EmpId, emp_id as Emp_Code, emp_firstname, Emp_info_no, Designation, Basic_Salary, "
                            + " Acc_allow, Other_allow, Food_allow, Transport_allow, SalaryStmtId, Other_Earnings, Bonus, Salary_Days, Leave_Days, Normal_OT_Hrs, "
                            + " Friday_OT_Hrs, Public_Holiday_OT_Hrs, Other_Income, Salary_Adv, ProcessedBy, Misslliance_Deduction FROM t_emp_salarystmt as tempSal, t_hr_employee as temp "
                            + " where tempSal.EmpId=temp.emp_no and temp.emp_status=1 and year(salaryfor_month)=year(current_date - interval 1 month) and "
                            + "month(salaryfor_month)= month(current_date - interval 1 month)";
                }

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSqlstmt = sSqlstmt + " and (emp_firstname ='" + SearchText + "' or emp_firstname like '" + SearchText + "%')";
                }

                sSqlstmt = sSqlstmt + " order by Emp_Code desc";

                DataSet dsAdvanceList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsAdvanceList.Tables.Count > 0 && dsAdvanceList.Tables[0].Rows.Count > 0)
                {
                    //ViewBag.EmpId = siYear;
                    for (int i = 0; i < dsAdvanceList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            DateTime dtSalaryFor_Month = Convert.ToDateTime(dsAdvanceList.Tables[0].Rows[i]["SalaryFor_Month"].ToString());
                            SalaryStatementModels objSalaryStmt = new SalaryStatementModels
                            {
                                EmpId = dsAdvanceList.Tables[0].Rows[i]["EmpId"].ToString(),
                                Emp_Code = (dsAdvanceList.Tables[0].Rows[i]["Emp_Code"].ToString()),
                                emp_firstname = (dsAdvanceList.Tables[0].Rows[i]["emp_firstname"].ToString()),
                                Emp_info_no = (dsAdvanceList.Tables[0].Rows[i]["Emp_info_no"].ToString()),
                                Designation = dsAdvanceList.Tables[0].Rows[i]["Designation"].ToString(),
                                Basic_Salary = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Basic_Salary"].ToString()),
                                Acc_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Acc_allow"].ToString()),
                                Other_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Other_allow"].ToString()),
                                Food_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Food_allow"].ToString()),
                                Transport_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Transport_allow"].ToString()),
                                SalaryStmtId = (dsAdvanceList.Tables[0].Rows[i]["SalaryStmtId"].ToString()),
                                SalaryFor_Month = dtSalaryFor_Month,
                                Other_Earnings = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Other_Earnings"].ToString()),
                                Bonus = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Bonus"].ToString()),
                                Salary_Days = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Salary_Days"].ToString()),
                                Leave_Days = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Leave_Days"].ToString()),
                                Normal_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Normal_OT_Hrs"].ToString()),
                                Friday_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Friday_OT_Hrs"].ToString()),
                                Public_Holiday_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Friday_OT_Hrs"].ToString()),
                                Other_Income = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Other_Income"].ToString()),
                                Salary_Adv = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Salary_Adv"].ToString()),
                                ProcessedBy = objGlobaldata.GetEmpNameById(dsAdvanceList.Tables[0].Rows[i]["ProcessedBy"].ToString()),
                                Misslliance_Deduction = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Misslliance_Deduction"].ToString())
                            };

                            objSalaryStatementList.lstSalaryStatement.Add(objSalaryStmt);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PrintPreviewSalaryStatement: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
           
            return View(objSalaryStatementList.lstSalaryStatement.ToList());
        }
        //
        // GET: /SalaryStatement/PrintSalaryStatement
        [AllowAnonymous]
        public ActionResult PrintSalaryStatement(string SearchText, string SalaryFor_Month)
        {
            SalaryStatementModelsList objSalaryStatementList = new SalaryStatementModelsList();
            objSalaryStatementList.lstSalaryStatement = new List<SalaryStatementModels>();

            try
            {
                string sSqlstmt = "";

                if (Request.QueryString["SalaryFor_Month"] != null && Request.QueryString["SalaryFor_Month"] != "")
                {                  
                    string[] SalaryDate = SalaryFor_Month.Split(' ');

                    string month = "", year = "";
                    if (SalaryDate.Length > 0)
                    {
                        month = DateTime.ParseExact(SalaryDate[0], "MMMM", CultureInfo.CurrentCulture).Month.ToString();
                        year = SalaryDate[1];

                        sSqlstmt = "SELECT EmpId, emp_id as Emp_Code, emp_firstname, Emp_info_no, Designation, Basic_Salary, Acc_allow, Other_allow, Food_allow, Transport_allow, SalaryStmtId,"
                                 + "SalaryFor_Month, Other_Earnings, Bonus, Salary_Days, Leave_Days, Normal_OT_Hrs, Friday_OT_Hrs, Public_Holiday_OT_Hrs, Other_Income,"
                                 + " Salary_Adv, ProcessedBy, Misslliance_Deduction FROM t_emp_salarystmt as tempSal, t_hr_employee as temp where tempSal.EmpId=temp.emp_no and temp.emp_status=1"
                                 + " and (month(SalaryFor_Month) = '" + month + "' and year(SalaryFor_Month)='" + year + "')";
                    }
                }
                else
                {
                    sSqlstmt = "SELECT (SalaryFor_Month) as SalaryFor_Month, EmpId, emp_id as Emp_Code, emp_firstname, Emp_info_no, Designation, Basic_Salary, "
                            + " Acc_allow, Other_allow, Food_allow, Transport_allow, SalaryStmtId, Other_Earnings, Bonus, Salary_Days, Leave_Days, Normal_OT_Hrs, "
                            + " Friday_OT_Hrs, Public_Holiday_OT_Hrs, Other_Income, Salary_Adv, ProcessedBy, Misslliance_Deduction FROM t_emp_salarystmt as tempSal, t_hr_employee as temp "
                            + " where tempSal.EmpId=temp.emp_no and temp.emp_status=1 and year(salaryfor_month)=year(current_date - interval 1 month) and "
                            + "month(salaryfor_month)= month(current_date - interval 1 month)";                            
                }

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSqlstmt = sSqlstmt + " and (emp_firstname ='" + SearchText + "' or emp_firstname like '" + SearchText + "%')";
                }

                sSqlstmt = sSqlstmt + " order by Emp_Code desc";

                DataSet dsAdvanceList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsAdvanceList.Tables.Count > 0 && dsAdvanceList.Tables[0].Rows.Count > 0)
                {
                    //ViewBag.EmpId = siYear;
                    for (int i = 0; i < dsAdvanceList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            DateTime dtSalaryFor_Month = Convert.ToDateTime(dsAdvanceList.Tables[0].Rows[i]["SalaryFor_Month"].ToString());
                            SalaryStatementModels objSalaryStmt = new SalaryStatementModels
                            {
                                EmpId = dsAdvanceList.Tables[0].Rows[i]["EmpId"].ToString(),
                                Emp_Code = (dsAdvanceList.Tables[0].Rows[i]["Emp_Code"].ToString()),
                                emp_firstname = (dsAdvanceList.Tables[0].Rows[i]["emp_firstname"].ToString()),
                                Emp_info_no = (dsAdvanceList.Tables[0].Rows[i]["Emp_info_no"].ToString()),
                                Designation = dsAdvanceList.Tables[0].Rows[i]["Designation"].ToString(),
                                Basic_Salary = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Basic_Salary"].ToString()),
                                Acc_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Acc_allow"].ToString()),
                                Other_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Other_allow"].ToString()),
                                Food_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Food_allow"].ToString()),
                                Transport_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Transport_allow"].ToString()),
                                SalaryStmtId = (dsAdvanceList.Tables[0].Rows[i]["SalaryStmtId"].ToString()),
                                SalaryFor_Month = dtSalaryFor_Month,
                                Other_Earnings = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Other_Earnings"].ToString()),
                                Bonus = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Bonus"].ToString()),
                                Salary_Days = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Salary_Days"].ToString()),
                                Leave_Days = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Leave_Days"].ToString()),
                                Normal_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Normal_OT_Hrs"].ToString()),
                                Friday_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Friday_OT_Hrs"].ToString()),
                                Public_Holiday_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[i]["Friday_OT_Hrs"].ToString()),
                                Other_Income = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Other_Income"].ToString()),
                                Salary_Adv = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Salary_Adv"].ToString()),
                                ProcessedBy = objGlobaldata.GetEmpNameById(dsAdvanceList.Tables[0].Rows[i]["ProcessedBy"].ToString()),
                                Misslliance_Deduction = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[i]["Misslliance_Deduction"].ToString())
                            };

                            objSalaryStatementList.lstSalaryStatement.Add(objSalaryStmt);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SalaryStatementList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return new Rotativa.ViewAsPdf(objSalaryStatementList.lstSalaryStatement.ToList())
            {
                FileName = SalaryFor_Month + "SalaryStatement.pdf",
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.A3
            };
            //return View(objSalaryStatementList.lstSalaryStatement.ToList());
        }


        //
        // GET: /SalaryStatement/SalaryStatementList
        [AllowAnonymous]
        public ActionResult SalaryStatementDetails()
        {
            try
            {

                if (Request.QueryString["SalaryStmtId"] != null && Request.QueryString["SalaryStmtId"] != "")
                {
                    string SalaryStmtId = Request.QueryString["SalaryStmtId"];
                    string sSqlstmt = "SELECT EmpId, emp_id as Emp_Code, emp_firstname, Emp_info_no, Designation, Basic_Salary, Acc_allow, Other_allow, Food_allow, Transport_allow, SalaryStmtId,"
                                 + "SalaryFor_Month, Other_Earnings, Bonus, Salary_Days, Leave_Days, Normal_OT_Hrs, Friday_OT_Hrs, Public_Holiday_OT_Hrs, Other_Income,"
                                 + " Salary_Adv, ProcessedBy, Misslliance_Deduction FROM t_emp_salarystmt as tempSal, t_hr_employee as temp where tempSal.EmpId=temp.emp_id and SalaryStmtId='" + SalaryStmtId + "'";


                    DataSet dsAdvanceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAdvanceList.Tables.Count > 0 && dsAdvanceList.Tables[0].Rows.Count > 0)
                    {

                        DateTime dtSalaryFor_Month = Convert.ToDateTime(dsAdvanceList.Tables[0].Rows[0]["SalaryFor_Month"].ToString());
                        SalaryStatementModels objSalaryStmt = new SalaryStatementModels
                        {
                            EmpId = dsAdvanceList.Tables[0].Rows[0]["EmpId"].ToString(),
                            Emp_Code = (dsAdvanceList.Tables[0].Rows[0]["Emp_Code"].ToString()),
                            emp_firstname = (dsAdvanceList.Tables[0].Rows[0]["emp_firstname"].ToString()),
                            Emp_info_no = (dsAdvanceList.Tables[0].Rows[0]["Emp_info_no"].ToString()),
                            Designation = dsAdvanceList.Tables[0].Rows[0]["Designation"].ToString(),
                            Basic_Salary = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Basic_Salary"].ToString()),
                            Acc_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Acc_allow"].ToString()),
                            Other_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Other_allow"].ToString()),
                            Food_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Food_allow"].ToString()),
                            Transport_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Transport_allow"].ToString()),
                            SalaryStmtId = (dsAdvanceList.Tables[0].Rows[0]["SalaryStmtId"].ToString()),
                            SalaryFor_Month = dtSalaryFor_Month,
                            Other_Earnings = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Other_Earnings"].ToString()),
                            Bonus = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Bonus"].ToString()),
                            Salary_Days = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Salary_Days"].ToString()),
                            Leave_Days = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Leave_Days"].ToString()),
                            Normal_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Normal_OT_Hrs"].ToString()),
                            Friday_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Friday_OT_Hrs"].ToString()),
                            Public_Holiday_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Friday_OT_Hrs"].ToString()),
                            Other_Income = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Other_Income"].ToString()),
                            Salary_Adv = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Salary_Adv"].ToString()),
                            ProcessedBy = objGlobaldata.GetEmpNameById(dsAdvanceList.Tables[0].Rows[0]["ProcessedBy"].ToString()),
                            Misslliance_Deduction = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Misslliance_Deduction"].ToString())
                        };

                        return View(objSalaryStmt);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("SalaryStatementList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "SalaryStmtId Id cannot be Null";
                    return RedirectToAction("SalaryStatementList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SalaryAdvanceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SalaryStatementList");
        }

        //
        // GET: /SalaryStatement/SalaryStatementEdit
        [AllowAnonymous]
        public ActionResult SalaryStatementEdit()
        {
            try
            {

                if (Request.QueryString["SalaryStmtId"] != null && Request.QueryString["SalaryStmtId"] != "")
                {
                    string SalaryStmtId = Request.QueryString["SalaryStmtId"];
                    string sSqlstmt = "SELECT EmpId, emp_id as Emp_Code, emp_firstname, Emp_info_no, Designation, Basic_Salary, Acc_allow, Other_allow, Food_allow, Transport_allow, SalaryStmtId,"
                                 + "SalaryFor_Month, Other_Earnings, Bonus, Salary_Days, Leave_Days, Normal_OT_Hrs, Friday_OT_Hrs, Public_Holiday_OT_Hrs, Other_Income,"
                                 + " Salary_Adv, ProcessedBy, Misslliance_Deduction FROM t_emp_salarystmt as tempSal, t_hr_employee as temp where tempSal.EmpId=temp.emp_id and SalaryStmtId='" + SalaryStmtId + "'";


                    DataSet dsAdvanceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAdvanceList.Tables.Count > 0 && dsAdvanceList.Tables[0].Rows.Count > 0)
                    {

                        DateTime dtSalaryFor_Month = Convert.ToDateTime(dsAdvanceList.Tables[0].Rows[0]["SalaryFor_Month"].ToString());
                        SalaryStatementModels objSalaryStmt = new SalaryStatementModels
                        {
                            EmpId = dsAdvanceList.Tables[0].Rows[0]["EmpId"].ToString(),
                            Emp_Code = (dsAdvanceList.Tables[0].Rows[0]["Emp_Code"].ToString()),
                            emp_firstname = (dsAdvanceList.Tables[0].Rows[0]["emp_firstname"].ToString()),
                            Emp_info_no = (dsAdvanceList.Tables[0].Rows[0]["Emp_info_no"].ToString()),
                            Designation = dsAdvanceList.Tables[0].Rows[0]["Designation"].ToString(),
                            Basic_Salary = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Basic_Salary"].ToString()),
                            Acc_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Acc_allow"].ToString()),
                            Other_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Other_allow"].ToString()),
                            Food_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Food_allow"].ToString()),
                            Transport_allow = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Transport_allow"].ToString()),
                            SalaryStmtId = (dsAdvanceList.Tables[0].Rows[0]["SalaryStmtId"].ToString()),
                            SalaryFor_Month = dtSalaryFor_Month,
                            Other_Earnings = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Other_Earnings"].ToString()),
                            Bonus = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Bonus"].ToString()),
                            Salary_Days = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Salary_Days"].ToString()),
                            Leave_Days = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Leave_Days"].ToString()),
                            Normal_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Normal_OT_Hrs"].ToString()),
                            Friday_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Friday_OT_Hrs"].ToString()),
                            Public_Holiday_OT_Hrs = Convert.ToInt32(dsAdvanceList.Tables[0].Rows[0]["Friday_OT_Hrs"].ToString()),
                            Other_Income = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Other_Income"].ToString()),
                            Salary_Adv = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Salary_Adv"].ToString()),
                            ProcessedBy = objGlobaldata.GetEmpNameById(dsAdvanceList.Tables[0].Rows[0]["ProcessedBy"].ToString()),
                            Misslliance_Deduction = Convert.ToDouble(dsAdvanceList.Tables[0].Rows[0]["Misslliance_Deduction"].ToString())
                        };

                        return View(objSalaryStmt);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("SalaryStatementList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "SalaryStmtId Id cannot be Null";
                    return RedirectToAction("SalaryStatementList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SalaryStatementEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SalaryStatementList");
        }


        //
        // POST: /SalaryStatement/SalaryStatementEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalaryStatementEdit(SalaryStatementModels objSalaryStatementModels, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                if (DateTime.TryParse(form["SalaryFor_Month"], out dateValue) == true)
                {
                    objSalaryStatementModels.SalaryFor_Month = dateValue;
                }

                objSalaryStatementModels.SalaryStmtId = form["SalaryStmtId"];
                objSalaryStatementModels.ProcessedBy = objGlobaldata.GetCurrentUserSession().empid;
                if (objSalaryStatementModels.FunUpdateSalaryStmt(objSalaryStatementModels))
                {
                    TempData["Successdata"] = "Salary details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                return RedirectToAction("SalaryStatementList");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SalaryStatementList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SalaryStatementList", new { SalaryFor_Month = objSalaryStatementModels.SalaryFor_Month.ToString("MMMM yyyy") });

        }
    }
}
