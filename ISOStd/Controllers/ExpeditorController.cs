using AutoMapper;
using DataAccess.Contract;
using DataAccess.Models;
using ISOStd.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    public class ExpeditorController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        private readonly IExpeditorDAL _expeditorDAL;
        private readonly IExpeditorOrderDAL _expeditorOrderDAL;
        private readonly IDropDownDAL _dropDownDAL;

        private MapperConfiguration configuration;

        private Mapper _mapper;

        public ExpeditorController(IExpeditorDAL expeditorDAL, IExpeditorOrderDAL expeditorOrderDAL,IDropDownDAL dropDownDAL)
        {
            ViewBag.Menutype = "Expeditor";
            _expeditorDAL = expeditorDAL;
            _expeditorOrderDAL = expeditorOrderDAL;
            _dropDownDAL = dropDownDAL;

            configuration = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ExpeditorTemplateModel, ExpeditorListItem>()
                );

            _mapper = new Mapper(configuration);
        }
        // GET: Expeditor
        public ActionResult Index()
        {
            ViewBag.SubMenutype = "Index";
            return View();
        }

        public ActionResult ExpeditorImport()
        {
            ViewBag.SubMenutype = "ExpeditorImport";
            return View();
        }

        [HttpPost]
        public ActionResult UploadBatchData(HttpPostedFileBase fileUploader)
        {
            ViewBag.SubMenutype = "Dashboard";
            try
            {
                DataSet ds = new DataSet();
                if (Request.Files["fileUploader"].ContentLength > 0)
                {
                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    string fileExtension = Path.GetExtension(Request.Files["fileUploader"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        //new changes
                        string fileLocation = Path.Combine(Server.MapPath("~/DataUpload/ExpeditorDocuments"), Path.GetFileName(Guid.NewGuid().ToString("N")) + fileExtension);
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
                        fileUploader.SaveAs(sFilepath + "/" + sFilename);//new changes
                        //Request.Files["fileUploader"].SaveAs(sFilepath + "/1" + sFilename);
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

                    string SqlStmt = "select IfNull(max(BatchId), 0) BatchId FROM t_expeditor_order_uploads;";
                    DataSet dsEmp = objGlobaldata.Getdetails(SqlStmt);
                    int batchId = 0;
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        batchId = int.Parse(dsEmp.Tables[0].Rows[0]["BatchId"].ToString());
                    }
                    batchId++;

                    ExpeditorTemplateModel model;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        model = new ExpeditorTemplateModel()
                        {
                            BatchId = batchId,
                            Company = ds.Tables[0].Rows[i]["Company"].ToString(),
                            Division = ds.Tables[0].Rows[i]["Division"].ToString(),
                            Area= ds.Tables[0].Rows[i]["Area"].ToString(),
                            Department = ds.Tables[0].Rows[i]["Department"].ToString(),
                            SalesmanCode = ds.Tables[0].Rows[i]["SalesmanCode"].ToString(),
                            SalesmanName = ds.Tables[0].Rows[i]["SalesmanName"].ToString(),
                            QuotationNo = ds.Tables[0].Rows[i]["QuotationNo"].ToString(),
                            QuotationDate = ds.Tables[0].Rows[i]["QuotationDate"].ToString(),
                            CustomerReferenceDate = ds.Tables[0].Rows[i]["CustomerReferenceDate"].ToString(),
                            CustomerPurchaseorder = ds.Tables[0].Rows[i]["CustomerPurchaseorder"].ToString(),
                            KanooCustomerPOInternalId = ds.Tables[0].Rows[i]["KanooCustomerPOInternalId"].ToString(),
                            CustomerMaterialCode = ds.Tables[0].Rows[i]["CustomerMaterialCode"].ToString(),
                            LineItemDescription = ds.Tables[0].Rows[i]["LineItemDescription"].ToString(),
                            Quantity = ds.Tables[0].Rows[i]["Quantity"].ToString(),
                            Unit = ds.Tables[0].Rows[i]["Unit"].ToString(),
                            ExpectedDeliveryDate = ds.Tables[0].Rows[i]["ExpectedDeliveryDate"].ToString(),
                            KanooAgreedDeliveryDate = ds.Tables[0].Rows[i]["KanooAgreedDeliveryDate"].ToString(),
                            SupplierPONo = ds.Tables[0].Rows[i]["SupplierPONo"].ToString(),
                            SupplierPODate = ds.Tables[0].Rows[i]["SupplierPODate"].ToString(),
                            MaterialCode = ds.Tables[0].Rows[i]["MaterialCode"].ToString(),
                            MaterialDetail = ds.Tables[0].Rows[i]["MaterialDetail"].ToString(),
                            SupplierQty = ds.Tables[0].Rows[i]["SupplierQty"].ToString(),
                            SupplierUnit = ds.Tables[0].Rows[i]["SupplierUnit"].ToString(),
                            ShipmentMode = ds.Tables[0].Rows[i]["ShipmentMode"].ToString(),
                            ExpectedReceivingDate = ds.Tables[0].Rows[i]["ExpectedReceivingDate"].ToString(),
                            AdvancePaymentStatus = ds.Tables[0].Rows[i]["AdvancePaymentStatus"].ToString(),
                            AdvancePaymentDoneOn = ds.Tables[0].Rows[i]["AdvancePaymentDoneOn"].ToString(),
                            CreatedBy = int.Parse(objUserInfo.empid),
                            IsProcessed = false,
                        };
                        _expeditorDAL.InsertOrderLineItem(model);

                    }
                    TempData["Successdata"] = "successfully uploaded";
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImportToExcel: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View("ExpeditorImport");
        }

        [HttpPost]
        public ActionResult UpdateMetaDataForOrder(ExpeditorOrderItemModel model)
        {
            try
            {
                var ss = model.Peta;
                int? i = null;
                var result = _expeditorDAL.InsertOrderItem(model);
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UpdateMetaDataForOrder: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("True", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExpeditorMaterialReport(DateTime? fromDate, DateTime? toDate,string dep,string status,string customerPo,string supplierPo,string salesman)
        {
            List<ExpeditorTemplateModel> details = new List<ExpeditorTemplateModel>();
            try
            {
                int? i = null;
                details = _expeditorDAL.GetOrderLineItem(fromDate, toDate,dep, status, customerPo, supplierPo,salesman);
                if (details.Any())
                {
                    foreach (var d in details)
                        d.FollowUp = d.FollowUp != null? d.FollowUp.Replace("</p>,<p>", "</p><p>"):string.Empty;
                }

                return Json(_mapper.Map<List<ExpeditorTemplateModel>, List<ExpeditorListItem>>(details), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetSummaryReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMaterialById(int id)
        {
            List<ExpeditorOrderItemModel> details = new List<ExpeditorOrderItemModel>();
            try
            {
                int? i = null;
                details = _expeditorOrderDAL.GetOrderLineItemsById(id);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetSummaryReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFilters()
        {
            Dictionary<string, List<DropDownModel>> filters = new Dictionary<string, List<DropDownModel>>();
            List<DropDownModel> details = new List<DropDownModel>();
            try
            {
                details = _dropDownDAL.GetDepartment("department");
                filters.Add("department", details);
                details = _dropDownDAL.GetDepartment("salesmanName");
                filters.Add("salesmanName", details);
                details = _dropDownDAL.GetDepartment("customerpurchaseorder");
                filters.Add("customerpurchaseorder", details);
                details = _dropDownDAL.GetDepartment("supplierpono");
                filters.Add("supplierpono", details);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetSummaryReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(filters, JsonRequestBehavior.AllowGet);
        }
    }
}