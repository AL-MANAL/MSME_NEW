using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ExpeditorTemplateModel : ExpeditorTemplate
    {
        

        [JsonIgnore]
        public string ExpectedReceivingDate { get; set; }

        [JsonIgnore]
        public string AdvancePaymentStatus { get; set; }

        [JsonIgnore]
        public string AdvancePaymentDoneOn { get; set; }

        [JsonIgnore]
        public int CreatedBy { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public bool IsProcessed { get; set; }

        [JsonIgnore]
        public int Customer_sale_order_id { get; set; }

        [JsonIgnore]
        public string Supplier_Order { get; set; }

        

        [JsonIgnore]
        private DateTime Customer_delivery_date { get; set; }

        [JsonIgnore]
        public string PCustomer_delivery_date
        {
            get { return this.Customer_delivery_date != DateTime.MinValue ? Customer_delivery_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { Customer_delivery_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
    }

    public class ExpeditorTemplate
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string LineItemAction { get { return "Edit"; } }
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
        public string SupplierPODate
        {
            get { return this.Supplier_Order_Date != DateTime.MinValue ? Supplier_Order_Date.ToString("yyyy-MM-dd") : string.Empty; }
            set { Supplier_Order_Date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }

        [JsonIgnore]
        public string MaterialCode { get; set; }

        [JsonIgnore]
        public string MaterialDetail { get; set; }

        [JsonIgnore]
        public string SupplierQty { get; set; }

        [JsonIgnore]
        public string SupplierUnit { get; set; }
        
        public string Supplier_payment_terms { get; set; }
        public string Supplier_delivery_terms { get; set; }
        public string Supplier_nmr_requirements { get; set; }
        public string Saber_requirements { get; set; }
        public string Bill_No { get; set; }

       
        public DateTime bill_date { get; set; }
        public string BillDate
        {
            get { return this.bill_date != DateTime.MinValue ? bill_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { bill_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public int Bill_Quantity { get; set; }


        public DateTime eta { get; set; }
        public string DisplayETA
        {
            get { return this.eta != DateTime.MinValue ? eta.ToString("yyyy-MM-dd") : string.Empty; }
            set { eta = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public string ShipmentMode { get; set; }
        public string Shipping_Doc_Rect { get; set; }
        public string Shipping_Doc_to_CCL_Dept { get; set; }
        public string Material_recieved_status { get; set; }
        //public string PRRDate
        //{
        //    get { return this.prr_date != DateTime.MinValue ? prr_date.ToString("yyyy-MM-dd") : string.Empty; }
        //    set { prr_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        //}

        
        //private DateTime prr_date { get; set; }

        public DateTime prr_date { get; set; }
        public string PRRDate
        {
            get { return this.prr_date != DateTime.MinValue ? prr_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { prr_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public string PRR_No { get; set; }

        public DateTime Srv_date { get; set; }
        public string SRVDate
        {
            get { return this.Srv_date != DateTime.MinValue ? Srv_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { Srv_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }

        public string SRV_No { get; set; }
        public string asn_no { get; set; }
        public string Asn_no { get; set; }
        public string Asn_quantity { get; set; }

        public string DeliveryStatus { get; set; }
        public string Delivery_Status
        {
            get
            {
                var tt = string.Empty;
                if (string.IsNullOrEmpty(DeliveryStatus))
                {
                    tt = string.Empty;
                }
                else if (DeliveryStatus == "1")
                    tt = "Inprogress";
                else if (DeliveryStatus == "2")
                    tt = "Pending";
                else if (DeliveryStatus == "3")
                    tt = "OnHold";
                else if (DeliveryStatus == "4")
                    tt = "Completed";
                else if (DeliveryStatus == "5")
                    tt = "Cancelled";

                return tt;
            }
            set
            {
                DeliveryStatus = value;
            }
        }
        public string DeliveryNoteNo { get; set; }

        public DateTime CustomerReceiptofMaterialGRDate { get; set; }
        public string CustomerReceiptofMaterial_GRDate
        {
            get { return this.CustomerReceiptofMaterialGRDate != DateTime.MinValue ? CustomerReceiptofMaterialGRDate.ToString("yyyy-MM-dd") : string.Empty; }
            set { CustomerReceiptofMaterialGRDate = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public string kanoo_invoice_no { get; set; }

        public DateTime kanoo_invoice_date { get; set; }
        public string Pkanoo_invoice_date
        {
            get { return this.kanoo_invoice_date != DateTime.MinValue ? kanoo_invoice_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { kanoo_invoice_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }
        public string FollowUp { get; set; }
        public string Root_cause_of_details { get; set; }

        public string DelayType { get; set; }

        [JsonIgnore]
        private DateTime Supplier_Order_Date { get; set; }

        public DateTime Supplier_delivery_date { get; set; }
        public string PSupplier_delivery_date
        {
            get { return this.Supplier_delivery_date != DateTime.MinValue ? Supplier_delivery_date.ToString("yyyy-MM-dd") : string.Empty; }
            set { Supplier_delivery_date = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }

        public DateTime DeliveryNoteDate { get; set; }

        public string PDeliveryNoteDate
        {
            get { return this.DeliveryNoteDate != DateTime.MinValue ? DeliveryNoteDate.ToString("yyyy-MM-dd") : string.Empty; }
            set { DeliveryNoteDate = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value); }
        }

        public string NextFollowUpDateTime { get; set; }

        public string Delay_reasons { get; set; }
    }
}
