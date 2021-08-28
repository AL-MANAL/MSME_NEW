using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;

namespace ISOStd.Controllers
{    

    public class EmployeeImportController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        
        public ActionResult AddEmployeeImport()
        {
            ViewBag.SubMenutype = "EmployeeImport";
            return View();
        }

        [HttpPost]
        public JsonResult AddEmployeeImport(HttpPostedFileBase upload)
        {
            ViewBag.SubMenutype = "AddEmployeeImport";
            int inc = 0;
            int err = 0;
            try
            {
                DataSet ds = new DataSet();
                if (Request.Files["upload"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["upload"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = Path.Combine(Server.MapPath("~/DataUpload/ImportExcelOLEDB"), Path.GetFileName(upload.FileName));
                        string sFilepath = Path.GetDirectoryName(fileLocation);
                        string sFilename = Path.GetFileName(fileLocation);

                        if (System.IO.File.Exists(fileLocation))
                        {
                           System.IO.File.Delete(fileLocation);
                        }
                        
                        upload.SaveAs(sFilepath + "/" + sFilename);
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        //Create Connection to Excel work book and add oledb namespace
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }
                        excelConnection.Close();

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }
                        excelConnection1.Close();
                    }
                    string stsql = "delete from t_hr_employee_upload; delete from t_hr_employee_upload_error;";
                    objGlobaldata.ExecuteQuery(stsql);

                    EmployeeDetailsController objemp = new EmployeeDetailsController();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string empid = objGlobaldata.checkEmpIdExists(ds.Tables[0].Rows[i][0].ToString());
                        // string email = objGlobaldata.checkEmailAddressExists(ds.Tables[0].Rows[i][4].ToString());
                        string division = objGlobaldata.GetCompanyBranchIDByName(ds.Tables[0].Rows[i][5].ToString());

                        if (empid != "false" && division != "")
                        {
                            string SqStmt = "Insert into t_hr_employee(emp_id,emp_firstname,emp_middlename,emp_lastname, " +
                                "EmailId,division,Emp_work_location,Dept_Id,Designation,Nationaliity," +
                                "Eid_no,MobileNo,EvaluatedBy,Role)" +
                                " Values('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "','" + ds.Tables[0].Rows[i][2].ToString() + "','" + ds.Tables[0].Rows[i][3].ToString()
                                + "','" + ds.Tables[0].Rows[i][4].ToString() + "','" + division + "','" + objGlobaldata.GetLocationIdbyName(ds.Tables[0].Rows[i][6].ToString()) + "','" + objGlobaldata.GetDeptIDByName(ds.Tables[0].Rows[i][7].ToString()) + "','" + ds.Tables[0].Rows[i][8].ToString() + "','" + ds.Tables[0].Rows[i][9].ToString()
                                + "','" + ds.Tables[0].Rows[i][10].ToString() + "','" + ds.Tables[0].Rows[i][11].ToString() + "','" + objGlobaldata.GetEmployeeHrEmpNoById(ds.Tables[0].Rows[i][12].ToString()) + "','" + objGlobaldata.GetRolesIdByName(ds.Tables[0].Rows[i][13].ToString()) + "');";

                            if (objGlobaldata.ExecuteQuery(SqStmt))
                            {
                                inc++;

                                string SqlStmt2 = "Insert into t_hr_employee_upload(emp_id,emp_firstname,emp_middlename,emp_lastname, " +
                                "EmailId,division,Emp_work_location,Dept_Id,Designation,Nationaliity," +
                                "Eid_no,MobileNo,EvaluatedBy,Role)" +
                                " Values('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "','" + ds.Tables[0].Rows[i][2].ToString() + "','" + ds.Tables[0].Rows[i][3].ToString()
                                + "','" + ds.Tables[0].Rows[i][4].ToString() + "','" + division + "','" + objGlobaldata.GetLocationIdbyName(ds.Tables[0].Rows[i][6].ToString()) + "','" + objGlobaldata.GetDeptIDByName(ds.Tables[0].Rows[i][7].ToString()) + "','" + ds.Tables[0].Rows[i][8].ToString() + "','" + ds.Tables[0].Rows[i][9].ToString()
                                + "','" + ds.Tables[0].Rows[i][10].ToString() + "','" + ds.Tables[0].Rows[i][11].ToString() + "','" + objGlobaldata.GetEmployeeHrEmpNoById(ds.Tables[0].Rows[i][12].ToString()) + "','" + objGlobaldata.GetRolesIdByName(ds.Tables[0].Rows[i][13].ToString()) + "');";

                                objGlobaldata.ExecuteQuery(SqlStmt2);
                            }
                        }
                        else
                        {
                            string SqStmt = "";
                            if (empid == "false")
                            {
                                 SqStmt = "Insert into t_hr_employee_upload_error(emp_id,emp_name,error_found) Values('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "', 'Employee Id Exist');";
                            }
                            else if (division == "")
                            {
                                 SqStmt = "Insert into t_hr_employee_upload_error(emp_id,emp_name,error_found) Values('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "', 'Division is not correct');";
                            }

                            if (objGlobaldata.ExecuteQuery(SqStmt))
                            {
                                err++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeeImport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            //return new JsonResult { Data = new { status = inc } };
            var result = new { data1 = inc, data2 = err };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChkUploadEmployee()
        {
            EmployeeMasterModelList objList = new EmployeeMasterModelList();
            objList.EmployeeList = new List<EmployeeMasterModels>();
            EmployeeMasterModels objEmp = new EmployeeMasterModels();
            try
            {              

                string sSqlstmt = "select emp_id,emp_firstname,emp_middlename,emp_lastname, " +
                                "EmailId,division,Emp_work_location,Dept_Id,Designation,Nationaliity," +
                                "Eid_no,MobileNo,EvaluatedBy,Role,upload_date from t_hr_employee_upload order by emp_no asc";
                 DataSet dsEmployeeList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmployeeList.Tables.Count > 0 && dsEmployeeList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmployeeList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objEmp = new EmployeeMasterModels
                            {
                               // emp_no = dsEmployeeList.Tables[0].Rows[i]["emp_no"].ToString(),
                                MobileNo = (dsEmployeeList.Tables[0].Rows[i]["MobileNo"].ToString()),
                                emp_id = (dsEmployeeList.Tables[0].Rows[i]["emp_id"].ToString()),
                                emp_firstname = dsEmployeeList.Tables[0].Rows[i]["emp_firstname"].ToString(),
                                emp_middlename = dsEmployeeList.Tables[0].Rows[i]["emp_middlename"].ToString(),
                                emp_lastname = dsEmployeeList.Tables[0].Rows[i]["emp_lastname"].ToString(),
                                EmailId = dsEmployeeList.Tables[0].Rows[i]["EmailId"].ToString(),
                                DeptId = objGlobaldata.GetDeptNameById(dsEmployeeList.Tables[0].Rows[i]["dept_id"].ToString()),
                                Designation = dsEmployeeList.Tables[0].Rows[i]["Designation"].ToString(),
                                Emp_work_location = objGlobaldata.GetDivisionLocationById(dsEmployeeList.Tables[0].Rows[i]["Emp_work_location"].ToString()),
                                Eid_no = dsEmployeeList.Tables[0].Rows[i]["Eid_no"].ToString(),
                                Nationaliity = dsEmployeeList.Tables[0].Rows[i]["Nationaliity"].ToString(),
                                Role = objGlobaldata.GetMultiRolesNameById(dsEmployeeList.Tables[0].Rows[i]["Role"].ToString()),
                                EvaluatedBy = objGlobaldata.GetEmpHrNameById(dsEmployeeList.Tables[0].Rows[i]["EvaluatedBy"].ToString()),
                                division = objGlobaldata.GetCompanyBranchNameById(dsEmployeeList.Tables[0].Rows[i]["division"].ToString()),
                            };
                            DateTime dtDate;
                            if (DateTime.TryParse(dsEmployeeList.Tables[0].Rows[i]["upload_date"].ToString(), out dtDate))
                            {
                                objEmp.upload_date = dtDate;
                            }

                            objList.EmployeeList.Add(objEmp);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PartiesList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }

                string sSqlstmt1 = "select emp_id,emp_name,upload_error_date,error_found from t_hr_employee_upload_error order by emp_no asc";
                DataSet dsEmployeeList1 = objGlobaldata.Getdetails(sSqlstmt1);

                if (dsEmployeeList1.Tables.Count > 0 && dsEmployeeList1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmployeeList1.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objEmp = new EmployeeMasterModels
                            {
                                emp_no = dsEmployeeList1.Tables[0].Rows[i]["emp_id"].ToString(),
                                JobDesc = (dsEmployeeList1.Tables[0].Rows[i]["emp_name"].ToString()),
                                error_found = (dsEmployeeList1.Tables[0].Rows[i]["error_found"].ToString()),
                            };

                            DateTime dtDate;
                            if (DateTime.TryParse(dsEmployeeList1.Tables[0].Rows[i]["upload_error_date"].ToString(), out dtDate))
                            {
                                objEmp.upload_error_date = dtDate;
                            }

                            objList.EmployeeList.Add(objEmp);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ChkUploadEmployee: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChkUploadEmployee: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objList.EmployeeList.ToList());

        }
    }
}