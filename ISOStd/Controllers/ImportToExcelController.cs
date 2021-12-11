using ISOStd.Filters;
using ISOStd.Models;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class ImportToExcelController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public ActionResult ImportToExcel()
        {
            try
            {
                ViewBag.LeaveType = objGlobaldata.GetDropdownList("File Type");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImportToExcel: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        public ActionResult ImportToExcel(string leavetype, HttpPostedFileBase file)
        {
            try
            {
                string sleavetype = objGlobaldata.GetDropdownitemById(leavetype);
                DataSet ds = new DataSet();
                if (Request.Files["file"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        //new changes
                        string fileLocation = Path.Combine(Server.MapPath("~/DataUpload/ImportExcelOLEDB"), Path.GetFileName(file.FileName));
                        string sFilepath = Path.GetDirectoryName(fileLocation);
                        string sFilename = Path.GetFileName(fileLocation);

                        //string fileLocation = Server.MapPath("~/DataUpload/ImportExcelOLEDB/") + Request.Files["file"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        //if (System.IO.File.Exists(fileLocation))
                        //{
                        //    System.IO.File.Delete(fileLocation);
                        //}
                        file.SaveAs(sFilepath + "/" + sFilename);//new changes
                                                                 //Request.Files["file"].SaveAs(fileLocation);
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
                    }
                    if (fileExtension.ToString().ToLower().Equals(".xml"))
                    {
                        string fileLocation = Server.MapPath("~/Content/") + Request.Files["FileUpload"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }

                        Request.Files["FileUpload"].SaveAs(fileLocation);
                        XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                        // DataSet ds = new DataSet();
                        ds.ReadXml(xmlreader);
                        xmlreader.Close();
                    }

                    if (sleavetype == "Leave Calender")
                    {
                        string SqlStmt = "truncate table t_holiday";
                        if (objGlobaldata.ExecuteQuery(SqlStmt))
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                string SqStmt = "Insert into t_holiday(Name,Frdate,Todate,NoOfDays) Values('" + ds.Tables[0].Rows[i][0].ToString() + "','" + Convert.ToDateTime(ds.Tables[0].Rows[i][1].ToString()).ToString("yyyy-MM-dd HH':'mm':'ss") + "','" + Convert.ToDateTime(ds.Tables[0].Rows[i][2].ToString()).ToString("yyyy-MM-dd HH':'mm':'ss") + "','" + ds.Tables[0].Rows[i][3].ToString() + "')";
                                if (objGlobaldata.ExecuteQuery(SqStmt))
                                {
                                    TempData["Successdata"] = "successfully uploaded";
                                }
                                else
                                {
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }
                    }

                    if (sleavetype == "Leave Master")
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string loggedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                            string SqStmt = "Insert into t_leave_master(emp_no,open_bal,close_bal,balance,logged_date,onSite)"
                            + "Values('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "'"
                            + ",'" + ds.Tables[0].Rows[i][2].ToString() + "','" + ds.Tables[0].Rows[i][3].ToString() + "','" + loggedDate + "','" + ds.Tables[0].Rows[i][4].ToString() + "')";
                            if (objGlobaldata.ExecuteQuery(SqStmt))
                            {
                                TempData["Successdata"] = "successfully uploaded";
                            }
                            else
                            {
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }

                    if (sleavetype == "Supplier")
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string loggedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                            string SqStmt = "Insert into t_supplier(SupplierCode,SupplierName,City,ContactPerson,ContactNo,Address,FaxNo,"
                            + "PO_No,SupplyScope,ApprovedOn,ApprovedStatus,Email,VatNo,Country)"
                            + "Values('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "',"
                            + "'" + ds.Tables[0].Rows[i][2].ToString() + "','" + ds.Tables[0].Rows[i][3].ToString() + "',"
                            + "'" + ds.Tables[0].Rows[i][4].ToString() + "','" + ds.Tables[0].Rows[i][5].ToString() + "',"
                            + "'" + ds.Tables[0].Rows[i][6].ToString() + "','" + ds.Tables[0].Rows[i][7].ToString() + "',"
                            + "'" + ds.Tables[0].Rows[i][8].ToString() + "',"
                            + "'" + Convert.ToDateTime(ds.Tables[0].Rows[i][9].ToString()).ToString("yyyy-MM-dd HH':'mm':'ss") + "','1',"
                            + "'" + ds.Tables[0].Rows[i][10].ToString() + "','" + ds.Tables[0].Rows[i][11].ToString() + "',"
                            + "'" + ds.Tables[0].Rows[i][12].ToString() + "')";
                            if (objGlobaldata.ExecuteQuery(SqStmt))
                            {
                                TempData["Successdata"] = "successfully uploaded";
                            }
                            else
                            {
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    if (sleavetype == "Customer")
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string loggedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                            string SqStmt = "Insert into t_customer_info(CompanyName,Address,City,PostalCode,Country,FaxNumber,"
                            + "CompStatus,Email_Id,Cust_Code)"
                            + "Values('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "',"
                            + "'" + ds.Tables[0].Rows[i][2].ToString() + "','" + ds.Tables[0].Rows[i][3].ToString() + "',"
                            + "'" + ds.Tables[0].Rows[i][4].ToString() + "','" + ds.Tables[0].Rows[i][5].ToString() + "',"
                            + "'1','" + ds.Tables[0].Rows[i][6].ToString() + "',"
                            + "'" + ds.Tables[0].Rows[i][7].ToString() + "')";
                            if (objGlobaldata.ExecuteQuery(SqStmt))
                            {
                                TempData["Successdata"] = "successfully uploaded";
                            }
                            else
                            {
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }

                    if (sleavetype == "Equipment")
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string SqStmt = "Insert into t_equipment(Equipment_serial_no,Equipment_Name,Equipment_Application,Commissioning_Date,"
                            + "Manufacturer,Model_No)"
                            + "Values('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "',"
                            + "'" + ds.Tables[0].Rows[i][2].ToString() + "','" + Convert.ToDateTime(ds.Tables[0].Rows[i][3].ToString()).ToString("yyyy-MM-dd HH':'mm':'ss") + "',"
                            + "'" + ds.Tables[0].Rows[i][4].ToString() + "','" + ds.Tables[0].Rows[i][5].ToString() + "')";
                            if (objGlobaldata.ExecuteQuery(SqStmt))
                            {
                                TempData["Successdata"] = "successfully uploaded";
                            }
                            else
                            {
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImportToExcel: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View();
        }
    }
}