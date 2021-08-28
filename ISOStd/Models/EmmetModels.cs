using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;

namespace ISOStd.Models
{
    public class EmmetModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_roundbar { get; set; }

        [Display(Name = "Material")]
        public string material { get; set; }

        [Display(Name = "Diameter")]
        public string diameter { get; set; }

        [Display(Name = "PO")]
        public string po { get; set; }

        [Display(Name = "Supplier")]
        public string supplier { get; set; }

        [Display(Name = "PO Document")]
        public string po_upload { get; set; }

        [Display(Name = "MTC Document")]
        public string mtc_upload { get; set; }

        [Display(Name = "Date")]
        public DateTime added_date { get; set; }

        [Display(Name = "Id")]
        public string id_roundbar_trans { get; set; }

        [Display(Name = "Heat No")]
        public string heatno { get; set; }

        [Display(Name = "Total Length(Inches)")]
        public decimal length { get; set; }

        [Display(Name = "Qty")]
        public int qty { get; set; }

        [Display(Name = "Unit Price")]
        public decimal price { get; set; }

        [Display(Name = "Total Price")]
        public decimal tprice { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime expiry_date { get; set; }

        [Display(Name = "Id")]
        public string id_stock { get; set; }

        [Display(Name = "Id")]
        public string id_roundbar_issue { get; set; }

        public decimal bal_qty { get; set; }

        [Display(Name = "Date")]
        public DateTime issue_date { get; set; }

        [Display(Name = "Job no")]
        public string jobno { get; set; }

        [Display(Name = "Issue Length(Inches)")]
        public decimal issue_length { get; set; }

        [Display(Name = "Issue Qty")]
        public int issue_qty { get; set; }

        [Display(Name = "Category")]
        public string category { get; set; }

        [Display(Name = "Sub Category")]
        public string subcategory { get; set; }

        [Display(Name = "Other Specification")]
        public string spec { get; set; }

        [Display(Name = "Material ID")]
        public string material_id { get; set; }

        [Display(Name = "Material Type")]
        public string material_type { get; set; }

        [Display(Name = "id")]
        public string id_material_master { get; set; }

     
        [Display(Name = "Length")]
        public string mlength { get; set; }

        [Display(Name = "id")]
        public string id_stock_receive { get; set; }

        [Display(Name = "id")]
        public string id_stock_receive_trans { get; set; }

        [Display(Name = "id")]
        public string id_stock_issue { get; set; }

        [Display(Name = "id")]
        public string id_stock_issue_trans { get; set; }

        [Display(Name = "Employee")]
        public string empid { get; set; }

        [Display(Name = "id")]
        public string id_stock_return { get; set; }

        [Display(Name = "Date")]
        public DateTime return_date { get; set; }

        [Display(Name = "Return Qty")]
        public int return_qty { get; set; }

        [Display(Name = "Status")]
        public string return_status { get; set; }

      
        [Display(Name = "Balance Qty")]
        public int balance_qty { get; set; }

        [Display(Name = "Minimum Qty")]
        public int min_qty { get; set; }

        [Display(Name = "Id")]
        public string id_stock_log { get; set; }

        [Display(Name = "Operation Type")]
        public string op_type { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTime tran_date { get; set; }

        [Display(Name = "Document")]
        public string upload { get; set; }

        [Display(Name = "Id")]
        public string id_chemical_receive { get; set; }

        [Display(Name = "Id")]
        public string id_chemical_receive_trans { get; set; }

        [Display(Name = "Id")]
        public string id_chemical_issue { get; set; }

        [Display(Name = "Id")]
        public string id_chemical_issue_trans { get; set; }

        [Display(Name = "Id")]
        public string id_chemical_return { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }

        internal bool FunDeleteRoundbarDoc(string id_roundbar)
        {
            try
            {
                string sSqlstmt = "update t_roundbar set Active=0 where id_roundbar='" + id_roundbar + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return FunUpdateRoundbarDoc(id_roundbar);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteRoundbarDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteRoundbarIssueDoc(string id_roundbar_issue)
        {
            try
            {
                string sSqlstmt = "update t_roundbar_issue set Active=0 where id_roundbar_issue='" + id_roundbar_issue + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return FunUpdateRoundbarIssueDoc(id_roundbar_issue);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteRoundbarIssueDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddRoundBar(EmmetModels objModel, EmmetModelsList objList)
        {
            try
            {
                string sql = "update t_stock_master set dummy_bal='0' where dummy_bal != '0'";
                objGlobalData.ExecuteQuery(sql);

                string sSqlstmt = "insert into t_roundbar(po,supplier,po_upload,mtc_upload";
                string sFields = "", sFieldValue = "";

                if (objModel.added_date != null && objModel.added_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", added_date";
                    sFieldValue = sFieldValue + ", '" + objModel.added_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.po +
                    "','" + objModel.supplier + "','" + objModel.po_upload + "','" + objModel.mtc_upload + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int id_roundbar = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_roundbar))
                {
                    if (id_roundbar > 0 && Convert.ToInt32(objList.EmmetList.Count) > 0)
                    {
                        objList.EmmetList[0].id_roundbar = id_roundbar.ToString();
                        FunAddRoundBarList(objList);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddRoundBar: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddRoundBarList(EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_roundbar_trans where id_roundbar='" + objList.EmmetList[0].id_roundbar + "'; ";


                for (int i = 0; i < objList.EmmetList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_roundbar_trans(id_roundbar,heatno,length,qty,price,category,material,tprice";

                    String sFieldValue = "", sFields = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objList.EmmetList[0].id_roundbar + "', '" + objList.EmmetList[i].heatno + "', '" + objList.EmmetList[i].length + "', '" + objList.EmmetList[i].qty + "', '" + objList.EmmetList[i].price + "','" + objList.EmmetList[i].category + "', '" + objList.EmmetList[i].material + "','" + objList.EmmetList[i].tprice + "'";
              
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddRoundBarList: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateRoundBar(EmmetModels objModel, EmmetModelsList objList)
        {
            try
            {
               
                string sSqlstmt = "update t_roundbar set po ='" + objModel.po + "',supplier ='" + objModel.supplier + "',po_upload ='" + objModel.po_upload + "',mtc_upload ='" + objModel.mtc_upload + "'";
                if (objModel.added_date != null && objModel.added_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", added_date='" + objModel.added_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_roundbar='" + objModel.id_roundbar + "'";
                int id_roundbar = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_roundbar))
                {
                    if (Convert.ToInt32(objList.EmmetList.Count) > 0)
                    {
                        objList.EmmetList[0].id_roundbar = objModel.id_roundbar;
                        FunAddRoundBarList(objList);
                    }
                    else
                    {
                        FunUpdateRoundbarDoc(objModel.id_roundbar);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateRoundBar: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateRoundbarDoc(string id_roundbar)
        {
            try
            {
                string sSqlstmt = "delete from t_roundbar_trans where id_roundbar='" + id_roundbar + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateRoundbarDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddStockIssue(EmmetModels objModel,EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "insert into t_roundbar_issue(jobno,po";
                string sFields = "", sFieldValue = "";

                if (objModel.issue_date != null && objModel.issue_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", issue_date";
                    sFieldValue = sFieldValue + ", '" + objModel.issue_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.jobno + "','" + objModel.po + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int id_roundbar_issue = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_roundbar_issue))
                {
                    if (id_roundbar_issue > 0 && Convert.ToInt32(objList.EmmetList.Count) > 0)
                    {
                        objList.EmmetList[0].id_roundbar_issue = id_roundbar_issue.ToString();
                        FunAddStockIssueList(objList);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddStockIssue: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddStockIssueList(EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_roundbar_issue_trans where id_roundbar_issue='" + objList.EmmetList[0].id_roundbar_issue + "'; ";


                for (int i = 0; i < objList.EmmetList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_roundbar_issue_trans(id_roundbar_issue,id_stock,qty,length,issue_qty,issue_length,category,material,heatno";

                    String sFieldValue = "", sFields = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objList.EmmetList[0].id_roundbar_issue + "', '" + objList.EmmetList[i].id_stock + "', '" + objList.EmmetList[i].qty + "', '" + objList.EmmetList[i].length + "', '" + objList.EmmetList[i].issue_qty + "', '" + objList.EmmetList[i].issue_length + "', '" + objList.EmmetList[i].category + "', '" + objList.EmmetList[i].material + "', '" + objList.EmmetList[i].heatno + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddStockIssueList: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateRoundbarIssueDoc(string id_roundbar_issue)
        {
            try
            {
                string sSqlstmt = "delete from t_roundbar_issue_trans where id_roundbar_issue='" + id_roundbar_issue + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateRoundbarIssueDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddMaterial(EmmetModels objModel)
        {
            try
            {
                string sSqlstmt = "insert into t_material_master(category,subcategory,spec,material_id,diameter,mlength,material_type,material,min_qty";
                string sFields = "", sFieldValue = "";
              
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objModel.category + "','" + objModel.subcategory + "','" + objModel.spec + "','" + objModel.material_id + "','" + objModel.diameter + "','" + objModel.mlength + "','" + objModel.material_type + "','" + objModel.material + "','" + objModel.min_qty + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddMaterial: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateMaterial(EmmetModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_material_master set category ='" + objModel.category + "',subcategory ='" + objModel.subcategory + "',spec ='" + objModel.spec + "'"
                    + ",material_id ='" + objModel.material_id + "',diameter ='" + objModel.diameter + "',mlength ='" + objModel.mlength + "',material_type ='" + objModel.material_type + "',material ='" + objModel.material + "',min_qty ='" + objModel.min_qty + "'";


                sSqlstmt = sSqlstmt + " where id_material_master='" + objModel.id_material_master + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateMaterial: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteMaterial(string id_material_master)
        {
            try
            {
                string sSqlstmt = "update t_material_master set Active=0 where id_material_master='" + id_material_master + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return FunUpdateRoundbarDoc(id_roundbar);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteMaterial: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddStock(EmmetModels objModel, EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "insert into t_stock_receive(po,supplier,po_upload,upload";
                string sFields = "", sFieldValue = "";

                if (objModel.added_date != null && objModel.added_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", added_date";
                    sFieldValue = sFieldValue + ", '" + objModel.added_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.po +
                    "','" + objModel.supplier + "','" + objModel.po_upload + "','" + objModel.upload + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int id_stock_receive = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_stock_receive))
                {
                    if (id_stock_receive > 0 && Convert.ToInt32(objList.EmmetList.Count) > 0)
                    {
                        objList.EmmetList[0].id_stock_receive = id_stock_receive.ToString();
                        FunAddStockList(objList);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddStock: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddStockList(EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_stock_receive_trans where id_stock_receive='" + objList.EmmetList[0].id_stock_receive + "'; ";


                for (int i = 0; i < objList.EmmetList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_stock_receive_trans(id_stock_receive,category,material,qty,price,tprice";

                    String sFieldValue = "", sFields = "";
                  
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objList.EmmetList[0].id_stock_receive + "', '" + objList.EmmetList[i].category + "', '" + objList.EmmetList[i].material + "','" + objList.EmmetList[i].qty + "', '" + objList.EmmetList[i].price + "', '" + objList.EmmetList[i].tprice + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddStockList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddChemical(EmmetModels objModel, EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "insert into t_chemical_receive(po,supplier,po_upload,upload";
                string sFields = "", sFieldValue = "";

                if (objModel.added_date != null && objModel.added_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", added_date";
                    sFieldValue = sFieldValue + ", '" + objModel.added_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.po +
                    "','" + objModel.supplier + "','" + objModel.po_upload + "','" + objModel.upload + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int id_chemical_receive = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_chemical_receive))
                {
                    if (id_chemical_receive > 0 && Convert.ToInt32(objList.EmmetList.Count) > 0)
                    {
                        objList.EmmetList[0].id_chemical_receive = id_chemical_receive.ToString();
                        FunAddChemicalList(objList);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddChemical: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddChemicalList(EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_chemical_receive_trans where id_chemical_receive='" + objList.EmmetList[0].id_chemical_receive + "'; ";


                for (int i = 0; i < objList.EmmetList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_chemical_receive_trans(id_chemical_receive,category,material,qty,price,tprice";

                    String sFieldValue = "", sFields = "";
                    if (objList.EmmetList[i].expiry_date != null && objList.EmmetList[i].expiry_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", expiry_date";
                        sFieldValue = sFieldValue + ", '" + objList.EmmetList[i].expiry_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objList.EmmetList[0].id_chemical_receive + "', '" + objList.EmmetList[i].category + "', '" + objList.EmmetList[i].material + "','" + objList.EmmetList[i].qty + "', '" + objList.EmmetList[i].price + "','" + objList.EmmetList[i].tprice + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddChemicalList: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddChemicalIssue(EmmetModels objModel, EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "insert into t_chemical_issue(jobno,po";
                string sFields = "", sFieldValue = "";

                if (objModel.issue_date != null && objModel.issue_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", issue_date";
                    sFieldValue = sFieldValue + ", '" + objModel.issue_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.jobno + "','" + objModel.po + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int id_chemical_issue = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_chemical_issue))
                {
                    if (id_chemical_issue > 0 && Convert.ToInt32(objList.EmmetList.Count) > 0)
                    {
                        objList.EmmetList[0].id_chemical_issue = id_chemical_issue.ToString();
                        FunAddChemicalIssueList(objList);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddChemicalIssue: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddChemicalIssueList(EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_chemical_issue_trans where id_chemical_issue='" + objList.EmmetList[0].id_chemical_issue + "'; ";


                for (int i = 0; i < objList.EmmetList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_chemical_issue_trans(id_chemical_issue,id_stock,issue_qty,category,material,balance_qty,empid";

                    String sFieldValue = "", sFields = "";
                    if (objList.EmmetList[i].expiry_date != null && objList.EmmetList[i].expiry_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", expiry_date";
                        sFieldValue = sFieldValue + ", '" + objList.EmmetList[i].expiry_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objList.EmmetList[0].id_chemical_issue + "', '" + objList.EmmetList[i].id_stock + "', '" + objList.EmmetList[i].issue_qty + "', '" + objList.EmmetList[i].category + "', '" + objList.EmmetList[i].material + "','" + objList.EmmetList[i].issue_qty + "','" + objList.EmmetList[i].empid + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddChemicalIssueList: " + ex.ToString());
            }
            return false;
        }



        internal bool FunEditAddStock(EmmetModels objModel, EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_stock_receive set po ='" + objModel.po + "',supplier ='" + objModel.supplier + "',po_upload ='" + objModel.po_upload + "'";
                if (objModel.added_date != null && objModel.added_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", added_date='" + objModel.added_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_stock_receive='" + objModel.id_stock_receive + "'";

                int id_stock_receive = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_stock_receive))
                {
                    if (Convert.ToInt32(objList.EmmetList.Count) > 0)
                    {
                        objList.EmmetList[0].id_stock_receive = objModel.id_stock_receive;
                        FunAddStockList(objList);
                    }
                    else
                    {
                        FunUpdateAddStockDoc(objModel.id_stock_receive);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunEditAddStock: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateAddStockDoc(string id_stock_receive)
        {
            try
            {
                string sSqlstmt = "delete from t_stock_receive_trans where id_stock_receive='" + id_stock_receive + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAddStockDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteStockReceive(string id_stock_receive)
        {
            try
            {
                string sSqlstmt = "update t_stock_receive set Active=0 where id_stock_receive='" + id_stock_receive + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return FunUpdateRoundbarDoc(id_roundbar);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteStockReceive: " + ex.ToString());
            }
            return false;
        }
        internal bool FunIssueStock(EmmetModels objModel, EmmetModelsList objList)
        {
            try
            {
                string sql = "update t_stock_master set dummy_bal='0' where dummy_bal != '0'";
                objGlobalData.ExecuteQuery(sql);

                string sSqlstmt = "insert into t_stock_issue(jobno,po";
                string sFields = "", sFieldValue = "";

                if (objModel.issue_date != null && objModel.issue_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", issue_date";
                    sFieldValue = sFieldValue + ", '" + objModel.issue_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.jobno + "','" + objModel.po + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int id_stock_issue = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_stock_issue))
                {
                    if (id_stock_issue > 0 && Convert.ToInt32(objList.EmmetList.Count) > 0)
                    {
                        objList.EmmetList[0].id_stock_issue = id_stock_issue.ToString();
                        return FunIssueStockList(objList);
                    }
                }

               
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunIssueStock: " + ex.ToString());
            }
            return false;
        }
        internal bool FunIssueStockList(EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_stock_issue_trans where id_stock_issue='" + objList.EmmetList[0].id_stock_issue + "'; ";


                for (int i = 0; i < objList.EmmetList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_stock_issue_trans(id_stock_issue,empid,category,material,issue_qty,balance_qty";

                    String sFieldValue = "", sFields = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objList.EmmetList[0].id_stock_issue + "', '" + objList.EmmetList[i].empid + "', '" + objList.EmmetList[i].category + "', '" + objList.EmmetList[i].material + "', '" + objList.EmmetList[i].issue_qty + "', '" + objList.EmmetList[i].issue_qty + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunIssueStockList: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateIssueStock(EmmetModels objModel, EmmetModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_stock_issue set jobno ='" + objModel.jobno + "'";
                if (objModel.issue_date != null && objModel.issue_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", issue_date='" + objModel.issue_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_stock_issue='" + objModel.id_stock_issue + "'";
                int id_stock_issue = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_stock_issue))
                {
                    if (Convert.ToInt32(objList.EmmetList.Count) > 0)
                    {
                        objList.EmmetList[0].id_stock_issue = objModel.id_stock_issue;
                        FunIssueStockList(objList);
                    }
                    else
                    {
                        FunUpdateIssueStockDoc(objModel.id_stock_issue);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateRoundBar: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateIssueStockDoc(string id_stock_issue)
        {
            try
            {
                string sSqlstmt = "delete from t_stock_issue_trans where id_stock_issue='" + id_stock_issue + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateIssueStockDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunReturnStock(EmmetModels objModel)
        {
            try
            {
                string sSqlstmt = "insert into t_stock_return(id_stock_issue_trans,return_qty,return_status";
                string sFields = "", sFieldValue = "";

                if (objModel.return_date != null && objModel.return_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", return_date";
                    sFieldValue = sFieldValue + ", '" + objModel.return_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.id_stock_issue_trans + "','" + objModel.return_qty + "','" + objModel.return_status + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunReturnStock: " + ex.ToString());
            }
            return false;
        }
        internal bool FunReturnChemical(EmmetModels objModel)
        {
            try
            {
                string sSqlstmt = "insert into t_chemical_return(id_chemical_issue_trans,return_qty,return_status";
                string sFields = "", sFieldValue = "";

                if (objModel.return_date != null && objModel.return_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", return_date";
                    sFieldValue = sFieldValue + ", '" + objModel.return_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.id_chemical_issue_trans + "','" + objModel.return_qty + "','" + objModel.return_status + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunReturnChemical: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateReturnStock(EmmetModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_stock_return set return_qty ='" + objModel.return_qty + "',return_status ='" + objModel.return_status + "'";
                if (objModel.return_date != null && objModel.return_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", return_date='" + objModel.return_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_stock_return='" + objModel.id_stock_return + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateReturnStock: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteReturnStockDoc(string id_stock_return)
        {
            try
            {
                string sSqlstmt = "update t_stock_return set Active=0 where id_stock_return='" + id_stock_return + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return FunUpdateRoundbarDoc(id_roundbar);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteReturnStockDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteIssueStockDoc(string id_stock_issue)
        {
            try
            {
                string sSqlstmt = "update t_stock_issue set Active=0 where id_stock_issue='" + id_stock_issue + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return FunUpdateRoundbarDoc(id_roundbar);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteIssueStockDoc: " + ex.ToString());
            }
            return false;
        }
    }
    public class EmmetModelsList
    {
        public List<EmmetModels> EmmetList { get; set; }
    }

}