using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ExpeditorOrderItemModel
    {
        public string LineItemAction { get { return "Edit"; } }

        #region Template table Column

        public int Id { get; set; }
        public int BatchId { get; set; }
        public string Company { get; set; }
        public string Division { get; set; }
        public string Area { get; set; }
        public string Department { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanName { get; set; }
        public string QuotationNo { get; set; }
        public string QuotationDate { get; set; }
        public string CustomerReferenceDate { get; set; }
        public string CustomerPurchaseorder { get; set; }
        public string KanooCustomerPOInternalId { get; set; }
        public string CustomerMaterialCode { get; set; }
        public string LineItemDescription { get; set; }
        public string Quantity { get; set; }
        public string Unit { get; set; }
        public string ExpectedDeliveryDate { get; set; }
        public string KanooAgreedDeliveryDate { get; set; }
        public string SupplierPONo { get; set; }
        public string SupplierPODate { get; set; }

        public string MaterialCode { get; set; }
        public string MaterialDetail { get; set; }
        public string SupplierQty { get; set; }
        public string SupplierUnit { get; set; }
        public string ShipmentMode { get; set; }
        public string ExpectedReceivingDate { get; set; }
        public string AdvancePaymentStatus { get; set; }
        public DateTime AdvancePaymentDoneOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsProcessed { get; set; }

        #endregion

        public int POrder_UploadedId { get; set; }
        public int PCustomer_sale_order_id { get; set; }
        public string PCompany { get; set; }
        public string PDivision { get; set; }
        public string PDepartment { get; set; }
        public string PSalesman { get; set; }
        public string PSales_expeditor_name { get; set; }
        public int PCustomer_id { get; set; }
        public string PSale_order_no { get; set; }
        public int PSale_order_line_item_no { get; set; }
        public DateTime Sale_order_date { get; set; }
        public string PSale_order_date
        {
            get { return this.Sale_order_date != DateTime.MinValue ? Sale_order_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { Sale_order_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }

        public string Pt_Customer_sale_orderscol { get; set; }
        public string Pmaterial_code { get; set; }
        public string Pmaterial_detail { get; set; }
        public string Punit { get; set; }
        public int Pquantity { get; set; }
        public string PSpecial_permit_status { get; set; }
        public DateTime PCustomer_delivery_date { get; set; }
        public string PCustomer_payment_terms { get; set; }
        public string PCustomer_delivery_terms { get; set; }
        public string PCustomer_nmr_requirements { get; set; }
        public string PSupplier_order { get; set; }
        public DateTime Supplier_order_date { get; set; }
        public string PSupplier_order_date
        {
            get { return this.Supplier_order_date != DateTime.MinValue ? Supplier_order_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { Supplier_order_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public DateTime Supplier_delivery_date { get; set; }
        public string PSupplier_delivery_date
        {
            get { return this.Supplier_delivery_date != DateTime.MinValue ? Supplier_delivery_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { Supplier_delivery_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public string PSupplier_payment_terms { get; set; }
        public string PSupplier_delivery_terms { get; set; }
        public string PSupplier_nmr_requirements { get; set; }
        public string PSaber_requirements { get; set; }
        public string Pbill_no { get; set; }
        public DateTime bill_date { get; set; }
        public string Pbill_date
        {
            get { return this.bill_date != DateTime.MinValue ? bill_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { bill_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public int Pbill_quantity { get; set; }
        public DateTime eta { get; set; }
        public string Peta
        {
            get { return this.eta != DateTime.MinValue ? eta.ToString("yyyy-MM-dd") : string.Empty; }
            set { eta = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public string Pmode_of_shipping { get; set; }
        public string PShipping_doc_rect { get; set; }
        public string PShipping_doc_to_ccl_dept { get; set; }
        public DateTime Shipping_Arrival_Date { get; set; }
        public string PShipping_Arrival_Date
        {
            get { return this.Shipping_Arrival_Date != DateTime.MinValue ? Shipping_Arrival_Date.ToString("yyyy-MM-dd") : string.Empty; }
            set { Shipping_Arrival_Date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public string Pmaterial_recieved_status { get; set; }
        public string Pprr_no { get; set; }
        public DateTime prr_date { get; set; }
        public string Pprr_date
        {
            get { return this.prr_date != DateTime.MinValue ? prr_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { prr_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }

        public string PSrv_no { get; set; }
        public DateTime Srv_date { get; set; }
        public string PSrv_date
        {
            get { return this.Srv_date != DateTime.MinValue ? Srv_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { Srv_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public string Pasn_no { get; set; }
        public int Pasn_quantity { get; set; }
        public string Pmaterial_delivery_to_Customer_status { get; set; }
        public string PDelivery_no { get; set; }
        public DateTime Delivery_date { get; set; }
        public string PDelivery_date
        {
            get { return this.Delivery_date != DateTime.MinValue ? Delivery_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { Delivery_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }

        public string Pkanoo_invoice_no { get; set; }
        public DateTime kanoo_invoice_date { get; set; }
        public string Pkanoo_invoice_date
        {
            get { return this.kanoo_invoice_date != DateTime.MinValue ? kanoo_invoice_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { kanoo_invoice_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public string Pfollow_upStatus { get; set; }
        public string PDelay_reasons { get; set; }
        public string Proot_cause_of_details { get; set; }

        public string PFollowUpStatus { get; set; }

        public string PFollowUpDateTime { get; set; }

        public string PNextFollowUpDateTime { get; set; }

        public DateTime CustomerReceiptofMaterialGRDate { get; set; }
        public string PCustomerReceiptofMaterialGRDate
        {
            get { return this.CustomerReceiptofMaterialGRDate != DateTime.MinValue ? CustomerReceiptofMaterialGRDate.ToString("yyyy-MM-dd") : string.Empty; }
            set { CustomerReceiptofMaterialGRDate = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }

        public string PDeliveryNoteNo { get; set; }

        public string PAdvancePaymentDoneOn
        {
            get { return this.AdvancePaymentDoneOn != DateTime.MinValue ? AdvancePaymentDoneOn.ToString("yyyy-MM-dd") : string.Empty; }
            set { AdvancePaymentDoneOn = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }

        // need to add
        public string PDeliveryStatus { get; set; }

        public string PDelayType { get; set; }

        public DateTime ActivityTargetCompletionDate { get; set; }
        public string PActivityTargetCompletionDate
        {
            get { return this.ActivityTargetCompletionDate != DateTime.MinValue ? ActivityTargetCompletionDate.ToString("yyyy-MM-dd") : string.Empty; }
            set { ActivityTargetCompletionDate = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }

        public string PInspectionRequirments { get; set; }

        public string PItemStatus { get; set; }

        public DateTime DeliveryNoteDate { get; set; }

        public string PDeliveryNoteDate
        {
            get { return this.DeliveryNoteDate != DateTime.MinValue ? DeliveryNoteDate.ToString("yyyy-MM-dd") : string.Empty; }
            set { DeliveryNoteDate = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
    }
}
