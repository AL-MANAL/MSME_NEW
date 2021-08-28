using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class CustEnquiryModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Inq. No")]
        public int id_enquiry { get; set; }        

        [Display(Name = "Mode of Inquiry")]
        public string mode_enquiry { get; set; }

        [Display(Name = "Date")]
        public DateTime date_enquiry { get; set; }

        [Display(Name = "Company Name")]
        public string companyname { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "Project Name")]
        public string projectname { get; set; }

        [Display(Name = "Contact Person")]
        public string contactperson { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "Sales Incharge")]
        public string sales_incharge { get; set; }

        [Display(Name = "Incharge Person")]
        public string incharge { get; set; }

        [Display(Name = "Date of quoted fax")]
        public DateTime date_fax { get; set; }

        [Display(Name = "Amount")]
        public decimal amt { get; set; }

        [Display(Name = "Q Pfq Rtqc No")]
        public string ref_no { get; set; }

        [Display(Name = "Date of LPO Received")]
        public DateTime date_lpo { get; set; }

        [Display(Name = "LPO No")]
        public string lpo_no { get; set; }

        [Display(Name = "LPO Amount")]
        public decimal lpo_amt { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        [Display(Name = "Document")]
        public string upload { get; set; }

        [Display(Name = "Telephone No")]
        public string telno { get; set; }

        [Display(Name = "Fax No")]
        public string faxno { get; set; }

        internal bool FunAddCustEnquiry(CustEnquiryModels objModels)
        {
            try
            {               
                string user = objGlobalData.GetCurrentUserSession().empid;

                string sdate_enquiry = objModels.date_enquiry.ToString("yyyy-MM-dd");
                string sdate_fax = objModels.date_fax.ToString("yyyy-MM-dd HH");
                string sdate_lpo = objModels.date_lpo.ToString("yyyy-MM-dd HH");

                string sSqlstmt = "insert into t_custenquiry(mode_enquiry,date_enquiry,companyname,description,projectname,contactperson,location,sales_incharge,"
                + "incharge,date_fax,amt,ref_no,date_lpo,lpo_no,lpo_amt,status,upload,telno)"
                + " values('" + objModels.mode_enquiry + "','" + sdate_enquiry + "','" + objModels.companyname + "','" + objModels.description + "','"
                + objModels.projectname + "','" + objModels.contactperson + "','" + objModels.location + "','" + objModels.sales_incharge + "','"
                + objModels.incharge + "','" + sdate_fax + "','" + objModels.amt + "','" + objModels.ref_no + "','" + sdate_lpo + "','" + objModels.lpo_no
                + "','" + objModels.lpo_amt + "','" + objModels.status + "','" + objModels.upload + "','" + objModels.telno + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCustEnquiry: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateCustEnquiry(CustEnquiryModels objModels)
        {
            try
            {               

                string sEnquiryDate = objModels.date_enquiry.ToString("yyyy-MM-dd");
                string sFaxDate = objModels.date_fax.ToString("yyyy-MM-dd");
                string sLpoDate = objModels.date_lpo.ToString("yyyy-MM-dd");
                
                string sSqlstmt = "update t_custenquiry set mode_enquiry='" + objModels.mode_enquiry + "',date_enquiry='" + sEnquiryDate + "',companyname='" + objModels.companyname
               + "',description='" + objModels.description + "',projectname='" + objModels.projectname + "',contactperson='" + objModels.contactperson + "',location='" + objModels.location
               + "',sales_incharge='" + objModels.sales_incharge + "',incharge='" + objModels.incharge + "',date_fax='" +sFaxDate + "',amt='" + objModels.amt + "',amt='" + objModels.amt
               + "',ref_no='" + objModels.ref_no + "',date_lpo='" + sLpoDate + "',lpo_no='" + objModels.lpo_no + "',lpo_amt='" + objModels.lpo_amt + "',status='" + objModels.status+"'";


               if (objModels.upload != null)
                {
                    sSqlstmt = sSqlstmt + ",upload='" + objModels.upload + "'";
                }

                sSqlstmt = sSqlstmt + " where id_enquiry='" + objModels.id_enquiry + "';";


                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCustEnquiry: " + ex.ToString());
            }

            return false;
        }

        internal bool FunDeleteCustEnquiry(string sid_enquiry)
        {
            try
            {
                string sSqlstmt = "update t_custenquiry set Active=0 where id_enquiry='" + sid_enquiry + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCustEnquiry: " + ex.ToString());
            }

            return false;
        }

        internal bool SendCustEquiryEmail(int id_enquiry, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string user = objGlobalData.GetCurrentUserSession().empid;
               
                string sSqlstmt = "select id_enquiry,mode_enquiry,date_enquiry,companyname,description,projectname,contactperson,location,sales_incharge," +
                    "incharge,date_fax,amt,ref_no,date_lpo,lpo_no,lpo_amt,status,upload,telno,faxno from t_custenquiry where id_enquiry='" + id_enquiry + "'";


                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);
                CustEnquiryModels objType = new CustEnquiryModels();


                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {                    
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";
                                      
                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    string sName = "All";
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["incharge"].ToString());
                    string sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(user);

                    aAttachment = HttpContext.Current.Server.MapPath(dsList.Tables[0].Rows[0]["upload"].ToString());

                    sHeader = "<tr><td colspan=3><b>Inq. No:<b></td> <td colspan=3>"
                        + dsList.Tables[0].Rows[0]["id_enquiry"].ToString() + "</td></tr>"
                         + "<tr><td colspan=3><b>Enquiry Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsList.Tables[0].Rows[0]["date_enquiry"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>"
                          + "<tr><td colspan=3><b>Incharge:<b></td> <td colspan=3>" + objGlobalData.GetEmpHrNameById(dsList.Tables[0].Rows[0]["incharge"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Company Name:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["companyname"].ToString()) + "</td></tr>"
                         + "<tr><td colspan=3><b>Project Name:<b></td> <td colspan=3>" + dsList.Tables[0].Rows[0]["projectname"].ToString() + "</td></tr>"
                          + "<tr><td colspan=3><b>Contact Person:<b></td> <td colspan=3>" + dsList.Tables[0].Rows[0]["contactperson"].ToString() + "</td></tr>"
                          + "<tr><td colspan=3><b>ModeOfEnquiry:<b></td> <td colspan=3>" + objType.GetModeofEnquiryById(dsList.Tables[0].Rows[0]["mode_enquiry"].ToString()) + "</td></tr>"
                             + "<tr><td colspan=3><b>Status:<b></td> <td colspan=3>" + objType.GetEnquiryStatusById(dsList.Tables[0].Rows[0]["status"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Description:<b></td> <td colspan=3>" + dsList.Tables[0].Rows[0]["description"].ToString() + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }



                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Customer Enquiry Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendCustEquiryEmail: " + ex.ToString());
            }
            return false;
        }

        public MultiSelectList GetModeofEnquiryList()
        {
            DropdownComplaintList lst = new DropdownComplaintList();
            lst.lst = new List<DropdownComplaintItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Enquiry Mode' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownComplaintItems reg = new DropdownComplaintItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lst.lst.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetModeofEnquiryList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        internal string GetModeofEnquiryById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Enquiry Mode' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetModeofEnquiryById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetEnquiryStatusList()
        {
            DropdownComplaintList lst = new DropdownComplaintList();
            lst.lst = new List<DropdownComplaintItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Enquiry Status' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownComplaintItems reg = new DropdownComplaintItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lst.lst.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEnquiryStatusList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        internal string GetEnquiryStatusById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Enquiry Status' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEnquiryStatusById: " + ex.ToString());
            }
            return "";
        }

    }

    public class QuotationModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_quotation { get; set; }

        [Display(Name = "Reference No")]
        public string ref_no { get; set; }

        [Display(Name = "Date")]
        public DateTime date_quotation { get; set; }

        [Display(Name = "To")]
        public string to_quotation { get; set; }

        [Display(Name = "Tel No")]
        public string telephone { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Project Number")]
        public string pro_ref { get; set; }

        [Display(Name = "Approved")]
        public string approved_by { get; set; }

        [Display(Name = "Id")]
        public string id_quo_trans { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "Quantity")]
        public decimal qty { get; set; }

        [Display(Name = "Price")]
        public decimal price { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        [Display(Name = "Total")]
        public decimal total { get; set; }

        [Display(Name = "Total")]
        public decimal sum { get; set; }

        [Display(Name = "Vat @5%")]
        public decimal vat { get; set; }

        public string logged_by { get; set; }

        internal bool FunAddQuotation(QuotationModels objModel, QuotationModelsList objList)
        {
            try
            {
                string sSqlstmt = "insert into t_quotation(ref_no,to_quotation,telephone,email,pro_ref,approved_by,upload,logged_by,sum,vat";
                string sFields = "", sFieldValue = "";

                if (objModel.date_quotation != null && objModel.date_quotation > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", date_quotation";
                    sFieldValue = sFieldValue + ", '" + objModel.date_quotation.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.ref_no + "','" + objModel.to_quotation + "','" + objModel.telephone
                  + "','" + objModel.email + "','" + objModel.pro_ref + "','" + objModel.approved_by + "','" + objModel.upload + "','" 
                  + objGlobalData.GetCurrentUserSession().empid + "','" + objModel.sum + "','" + objModel.vat + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int Id_quotation = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Id_quotation))
                {
                    if (Id_quotation > 0 && Convert.ToInt32(objList.QuotList.Count) > 0)
                    {
                        objList.QuotList[0].id_quotation = Id_quotation.ToString();
                        FunAddQuotationList(objList);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddQuotation: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddQuotationList(QuotationModelsList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_quotation_trans where id_quotation='" + objList.QuotList[0].id_quotation + "'; ";
                
                for (int i = 0; i < objList.QuotList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_quotation_trans (id_quotation,description,qty,price,total";

                    String sFieldValue = "";
                    sSqlstmt = sSqlstmt + ") values('" + objList.QuotList[0].id_quotation + "', '" + objList.QuotList[i].description +
                        "', '" + objList.QuotList[i].qty + "', '" + objList.QuotList[i].price + "', '" + objList.QuotList[i].total + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddQuotationList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateQuotation(QuotationModels objQuot, QuotationModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_quotation set ref_no='" + objQuot.ref_no + "', to_quotation='" + objQuot.to_quotation + "', telephone='" + objQuot.telephone
                    + "', email='" + objQuot.email + "', pro_ref='" + objQuot.pro_ref + "', approved_by='" + objQuot.approved_by
                    + "', logged_by='" + objGlobalData.GetCurrentUserSession().empid + "', upload='" + objQuot.upload + "', sum='" + objQuot.sum + "', vat='" + objQuot.vat + "'";

                if (objQuot.date_quotation != null && objQuot.date_quotation > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", date_quotation='" + objQuot.date_quotation.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + "where id_quotation = '" + objQuot.id_quotation + "'";

                int Id = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Id))
                {
                    if (Convert.ToInt32(objList.QuotList.Count) > 0)
                    {
                        objList.QuotList[0].id_quotation = objQuot.id_quotation;
                        FunAddQuotationList(objList);
                    }
                    else
                    {
                        FunUpdateQuotationList(objQuot.id_quotation);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateQuotation: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateQuotationList(string sid_quotation)
        {
            try
            {
                string sSqlstmt = "delete from t_quotation_trans where id_quotation='" + sid_quotation + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateQuotationList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteQuotation(string sid_quotation)
        {
            try
            {
                string sSqlstmt = "update t_quotation set active=0 where id_quotation='" + sid_quotation + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteQuotation: " + ex.ToString());
            }
            return false;
        }

        public decimal FunGrandTot(decimal tot,decimal vat)
        {
            try
            {
                decimal Grand = 0;
                Grand = tot + vat;
                return (Grand);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGrandTot: " + ex.ToString());
            }
            return 0;
        }

    }

    public class CustEnquiryModelsList
    {
        public List<CustEnquiryModels> EnquiryList { get; set; }
    }

    public class QuotationModelsList
    {
        public List <QuotationModels> QuotList { get; set; }
    }
}