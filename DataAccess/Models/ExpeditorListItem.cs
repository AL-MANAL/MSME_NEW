using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ExpeditorListItem
    {
        public int Id { get; set; }

        public int BatchId { get; set; }
        public string LineItemAction { get; set; }
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
        public string SpecialPermitStatus { get; set; }
        public string Customer_payment_terms { get; set; }
        public string Customer_delivery_terms { get; set; }
        public string Customer_nmr_requirements { get; set; }
        public string SupplierPONo { get; set; }
        public string SupplierPODate { get; set; }
        public string PSupplier_delivery_date { get; set; }
        public string Supplier_payment_terms { get; set; }
        public string Supplier_delivery_terms { get; set; }
        public string Supplier_nmr_requirements { get; set; }
        public string Saber_requirements { get; set; }
        public string Bill_No { get; set; }
        public string BillDate { get; set; }
        public string Bill_Quantity { get; set; }
        public string DisplayETA { get; set; }
        public string ShipmentMode { get; set; }
        public string Shipping_Doc_Rect { get; set; }
        public string Shipping_Doc_to_CCL_Dept { get; set; }
        public string Material_recieved_status { get; set; }
        public string PRRDate { get; set; }
        public string PRR_No { get; set; }
        public string SRVDate { get; set; }
        public string SRV_No { get; set; }
        public string asn_no { get; set; }
        public string Asn_quantity { get; set; }
        public string Delivery_Status { get; set; }
        public string DeliveryNoteNo { get; set; }
        public string CustomerReceiptofMaterial_GRDate { get; set; }
        public string kanoo_invoice_no { get; set; }
        public string Pkanoo_invoice_date { get; set; }
        public string FollowUp { get; set; }
        public string Root_cause_of_details { get; set; }
        public string NextFollowUpDateTime { get; set; }
        public string DeliveryNoteDate { get; set; }
        public string DelayType { get; set; }
        public string Delay_reasons { get; set; }
    }
}
