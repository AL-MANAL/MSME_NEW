using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ISOStd.Models
{
    public class CustomerReturnProductModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_return_product { get; set; }

        [Display(Name = "Date")]
        public DateTime return_date { get; set; }

        [Display(Name = "Customer")]
        public string CustID { get; set; }

        [Display(Name = "Reference No")]
        public string refno { get; set; }

        [Display(Name = "Delivery Note/ Invoice No")]
        public string retun_deliverynote { get; set; }

        [Display(Name = "Delivery Date")]
        public DateTime retun_deliverydate { get; set; }

        [Display(Name = "Product Details")]
        public string product_details { get; set; }

        [Display(Name = "Reason")]
        public string reason { get; set; }

        [Display(Name = "Action Taken")]
        public string action_taken { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        [Display(Name = "NCN Ref")]
        public string Ncn_ref { get; set; }

        [Display(Name = "Status")]
        public string product_status { get; set; }

        [Display(Name = "Logged By")]
        public string loggedby { get; set; }
        
        [Display(Name = "Responsible Person")]
        public string resp_person { get; set; }

        [Display(Name = "Document")]
        public string upload { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        internal bool FunAddCustReturnProduct(CustomerReturnProductModels objProp,IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                string sSqlstmt = "insert into t_cust_return_product (CustID,refno,retun_deliverynote,product_details,reason,action_taken,remarks,Ncn_ref,product_status,loggedby,resp_person,upload";
                string sFields = "", sFieldValue = "";
                if (objProp.return_date != null && objProp.return_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", return_date";
                    sFieldValue = sFieldValue + ", '" + objProp.return_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objProp.retun_deliverydate != null && objProp.retun_deliverydate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", retun_deliverydate";
                    sFieldValue = sFieldValue + ", '" + objProp.retun_deliverydate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objProp.CustID + "','" + objProp.refno + "','" + objProp.retun_deliverynote + "'"
                    + ",'" + objProp.product_details + "','" + objProp.reason + "','" + objProp.action_taken + "','" + objProp.remarks + "','" + objProp.Ncn_ref + "','" + objProp.product_status
                    + "','" + objGlobalData.GetCurrentUserSession().empid + "','" + objProp.resp_person + "','" + objProp.upload + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
 
               int Id = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Id))
                {
                    if (Id > 0)
                    {
                        DataSet dsData = objGlobalData.GetReportNo("CRP", "", "");
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            refno = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        string sql = "update t_cust_return_product set refno='" + refno + "'where id_return_product = '" + Id + "'";

                        if(objGlobalData.ExecuteQuery(sql))
                        {
                            SendRespPersonEmail(objProp,upload);
                        }
                        return true; 
                    }
                }                

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCustReturnProduct: " + ex.ToString());
            }
            return false;
        }

        internal bool SendRespPersonEmail(CustomerReturnProductModels objProp, IEnumerable<HttpPostedFileBase> upload)
        {

            try
            {

                string sInformation = "", sHeader = "", sToEmailId = "", sCCList = "", sUserName = "", sUser = "";

                // string sInformation = "", sHeader = "";
                sCCList = objGlobalData.GetMultiHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().empid);
                sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(objProp.resp_person);
                //sUserName = objGlobalData.GetMultiHrEmpNameById(objGlobalData.GetHSEEmployee());

                sInformation =
                "Customer:'" + objGlobalData.GetCustomerNameById(objProp.CustID)+ "'"
                + " <br />"
                + "Date:'" + objProp.return_date.ToString("dd/MM/yyyy") + "'"
                + " <br />"
                + "Delivery Note/ Invoice No:'" + objProp.retun_deliverynote + "'"
                + " <br />"
                + "Delivery Date:'" + objProp.retun_deliverydate.ToString("dd/MM/yyyy") + "'"
                + " <br />"
                + "Product Details:'" + objProp.product_details + "'"
                + " <br />"
                + "NCN Ref:'" + objProp.Ncn_ref + "'"
                + " <br />"
                + "Status:'" + objGlobalData.GetDropdownitemById(objProp.product_status) + "'";

                sHeader = sHeader + sInformation;

                sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                sToEmailId = sToEmailId.Trim();
                sToEmailId = sToEmailId.TrimEnd(',');
                sToEmailId = sToEmailId.TrimStart(',');

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "CustomerReturnProduct", sHeader, "", "Customer Return Product");
                objGlobalData.SendCustRetunmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], upload, sCCList, "");
                return true;

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendRespPersonEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateCustReturnProduct(CustomerReturnProductModels objProp)
        {
            try
            {
                string sSqlstmt = "update t_cust_return_product set CustID ='" + objProp.CustID /*+ "', refno='" + objProp.refno */+ "', "
                    + "retun_deliverynote='" + objProp.retun_deliverynote + "',reason='" + objProp.reason + "',product_details='" + objProp.product_details + "',Ncn_ref='" + objProp.Ncn_ref + "'"
                    + ",product_status='" + objProp.product_status + "',action_taken='" + objProp.action_taken + "',remarks='" + objProp.remarks + "',resp_person='" + objProp.resp_person + "',upload='" + objProp.upload + "'";

                if (objProp.return_date != null && objProp.return_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", return_date='" + objProp.return_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objProp.retun_deliverydate != null && objProp.retun_deliverydate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", retun_deliverydate='" + objProp.retun_deliverydate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_return_product='" + objProp.id_return_product + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCustReturnProduct: " + ex.ToString());
            }
            return false;
        }


        internal bool FunDelCustReturnProduct(string sid_return_product)
        {
            try
            {
                string sSqlstmt = "update t_cust_return_product set Active=0 where id_return_product='" + sid_return_product + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDelCustReturnProduct: " + ex.ToString());
            }
            return false;
        }

    }
    public class CustomerReturnProductModelsList
    {
        public List<CustomerReturnProductModels> PropList { get; set; }
    }
}