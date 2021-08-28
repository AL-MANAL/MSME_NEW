using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using DataAccess.Contract;
using DataAccess.Models;
using HA.HALoG5BWService.DatabaseAccess.Impl;
using MySql.Data.MySqlClient;

namespace DataAccess.Impl
{
    public class ExpeditorDAL : GenericRepository<ExpeditorTemplateModel>, IExpeditorDAL
    {
        private readonly string _connectionString;

        #region Constructor

        public ExpeditorDAL() : base(Constants.DbConnectionString)
        {
            this._connectionString = Constants.DbConnectionString;
        }
        #endregion

        //#region DB methods

        public List<ExpeditorTemplateModel> GetOrderLineItem(DateTime? fromDate, DateTime? toDate, string dep, string status, string customerPo, string supplierPo, string salesman)
        {
            object parameters = new
            {
                PDep = dep,
                PStatus = status,
                PFromDate = fromDate.HasValue ? fromDate.Value.ToString("yyyy-MM-dd"):DateTime.Now.AddYears(-5).ToString("yyyy-MM-dd"),
                PToDate = toDate.HasValue ? toDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"),
                PCustomerPo = customerPo,
                PSupplierPo = supplierPo,
                PSalesman = salesman
            };
            var transaction = base.GetAll(parameters, "get_expeditor_order_line_items");

            return transaction;
        }

        public int InsertOrderLineItem(ExpeditorTemplateModel detail)
        {
            object parameters = new
            {
                pid = detail.Id,
                pBatchId = detail.BatchId,
                pCompany = detail.Company,
                pDivision = detail.Division,
                pArea = detail.Area,
                pDepartment = detail.Department,
                pSalesmanCode = detail.SalesmanCode,
                pSalesmanName = detail.SalesmanName,
                pQuotationNo = detail.QuotationNo,
                pQuotationDate = detail.QuotationDate,
                pCustomerReferenceDate = detail.CustomerReferenceDate,
                pCustomerPurchaseorder = detail.CustomerPurchaseorder,
                pKanooCustomerPOInternalId = detail.KanooCustomerPOInternalId,
                pCustomerMaterialCode = detail.CustomerMaterialCode,
                pLineItemDescription = detail.LineItemDescription,
                pQuantity = detail.Quantity,
                pUnit = detail.Unit,
                pExpectedDeliveryDate = detail.ExpectedDeliveryDate,
                pKanooAgreedDeliveryDate = detail.KanooAgreedDeliveryDate,
                pSupplierPONo = detail.SupplierPONo,
                pSupplierPODate = detail.SupplierPODate,
                pMaterialCode = detail.MaterialCode,
                pMaterialDetail = detail.MaterialDetail,
                pSupplierQty = detail.SupplierQty,
                pSupplierUnit = detail.SupplierUnit,
                pShipmentMode = detail.ShipmentMode,
                pExpectedReceivingDate = detail.ExpectedReceivingDate,
                pAdvancePaymentStatus = detail.AdvancePaymentStatus,
                pAdvancePaymentDoneOn = detail.AdvancePaymentDoneOn,
                pCreatedBy = detail.CreatedBy,
                pCreatedDate = detail.CreatedDate,
                pIsProcessed = detail.IsProcessed
            };

            return (int)base.Insert(parameters, "insert_expeditor_order_line_item");
        }

        public int InsertOrderItem(ExpeditorOrderItemModel detail)
        {
            object parameters = new
            {
                POrder_UploadedId = detail.POrder_UploadedId,
                Pcustomer_sale_order_id = detail.PCustomer_sale_order_id,
                //PCompany = detail.PCompany,
                //Pdivision = detail.PDivision,
                //Pdepartment = detail.PDepartment,
                //PSalesman = detail.PSalesman,
                //PSales_expeditor_name = detail.PSales_expeditor_name,
                PCustomer_id = detail.PCustomer_id,
                //PSale_order_no = detail.PSale_order_no,
                //PSale_order_line_item_no = detail.PSale_order_line_item_no,
                //PSale_order_date = detail.Sale_order_date,
                Pt_Customer_sale_orderscol = detail.Pt_Customer_sale_orderscol,
                //Pmaterial_code = detail.Pmaterial_code,
                //Pmaterial_detail = detail.Pmaterial_detail,
                //Punit = detail.Punit,
                //Pquantity = detail.Pquantity,
                PSpecial_permit_status = detail.PSpecial_permit_status,
                PCustomer_delivery_date = detail.PCustomer_delivery_date,
                PCustomer_payment_terms = detail.PCustomer_payment_terms,
                PCustomer_delivery_terms = detail.PCustomer_delivery_terms,
                PCustomer_nmr_requirements = detail.PCustomer_nmr_requirements,
                //PSupplier_order = detail.PSupplier_order,
                //PSupplier_order_date = detail.Supplier_order_date,
                PSupplier_delivery_date = detail.Supplier_delivery_date,
                PSupplier_payment_terms = detail.PSupplier_payment_terms,
                PSupplier_delivery_terms = detail.PSupplier_delivery_terms,
                PSupplier_nmr_requirements = detail.PSupplier_nmr_requirements,
                PSaber_requirements = detail.PSaber_requirements,
                Pbill_no = detail.Pbill_no,
                Pbill_date = detail.bill_date,
                Pbill_quantity = detail.Pbill_quantity,
                Peta = detail.eta,
                Pmode_of_shipping = detail.Pmode_of_shipping,
                PShipping_doc_rect = detail.PShipping_doc_rect,
                PShipping_doc_to_ccl_dept = detail.PShipping_doc_to_ccl_dept,
                Pmaterial_recieved_status = detail.Pmaterial_recieved_status,
                Pprr_no = detail.Pprr_no,
                Pprr_date = detail.prr_date,
                PSrv_no = detail.PSrv_no,
                PSrv_date = detail.Srv_date,
                Pasn_no = detail.Pasn_no,
                Pasn_quantity = detail.Pasn_quantity,
                Pmaterial_delivery_to_Customer_status = detail.Pmaterial_delivery_to_Customer_status,
                Pdelivery_no = detail.PDelivery_no,
                Pdelivery_date = detail.Delivery_date,
                Pkanoo_invoice_no = detail.Pkanoo_invoice_no,
                Pkanoo_invoice_date = detail.kanoo_invoice_date,
                Pfollow_uPStatus = detail.Pfollow_upStatus,
                Pdelay_reasons = detail.PDelay_reasons,
                Proot_cause_of_details = detail.Proot_cause_of_details,
                PFollowUpStatus = detail.PFollowUpStatus,
                PFollowUpDateTime = detail.PFollowUpDateTime,
                PShipping_Arrival_Date = detail.Shipping_Arrival_Date,
                PCustomerReceiptofMaterialGRDate = detail.CustomerReceiptofMaterialGRDate,
                PDeliveryNoteNo = detail.PDeliveryNoteNo,
                //PAdvancePaymentDoneOn = detail.AdvancePaymentDoneOn,
                PDeliveryStatus = detail.PDeliveryStatus,
                PDelayType = detail.PDelayType,
                PActivityTargetCompletionDate = detail.ActivityTargetCompletionDate,
                PInspectionRequirments = detail.PInspectionRequirments,
                PItemStatus = detail.PItemStatus,
                PDeliveryNoteDate = detail.DeliveryNoteDate,
                PNextFollowUpDateTime = detail.PNextFollowUpDateTime
            };

            return (int)base.Insert(parameters, "insert_expeditor_order_item");
        }
        //#endregion
    }

    public class ExpeditorOrderDAL : GenericRepository<ExpeditorOrderItemModel>, IExpeditorOrderDAL
    {
        private readonly string _connectionString;

        #region Constructor

        public ExpeditorOrderDAL() : base(Constants.DbConnectionString)
        {
            this._connectionString = Constants.DbConnectionString;
        }
        #endregion

        public List<ExpeditorOrderItemModel> GetOrderLineItemsById(int id)
        {
            object parameters = new
            {
                PId = id
            };
            var transaction = base.GetAll(parameters, "get_expeditor_items");

            return transaction;
        }
    }
}
