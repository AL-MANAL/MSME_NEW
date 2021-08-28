using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace ISOStd.Models
{
    public class SuppliedMaterialModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //t_supplied_materials
        [Display(Name = "Id")]
        public string id_materials { get; set; }

        [Display(Name = "Order Date")]
        public DateTime order_date { get; set; }

        [Display(Name = "Order Number")]
        public string orderno { get; set; }

        [Display(Name = "Provider Type")]
        public string provider_type { get; set; }

        [Display(Name = "Supplier Name")]
        public string supplier_name { get; set; }

        [Display(Name = "Customer Name")]
        public string customer_name { get; set; }

        [Display(Name = "Remark")]
        public string remark { get; set; }

        //t_supplied_materials_trans
        [Display(Name = "Id")]
        public string id_materials_trans { get; set; }

        [Display(Name = "Date")]
        public DateTime qty_date { get; set; }

        [Display(Name = "Type of Operation")]
        public string operation_type { get; set; }       
            
        [Display(Name = "Quantity")]
        public int quantity { get; set; }

        [Display(Name = "Done by")]
        public string done_by { get; set; }

        [Display(Name = "Details of the material received")]
        public string details { get; set; }

        public int balance { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        internal bool FunAddSuppliedMaterial(SuppliedMaterialModels objMdl, SuppliedMaterialModelsList objList)
        {
            try
            {
                //string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_supplied_materials (orderno,provider_type,supplier_name,customer_name,remark," +
                    "loggedby,branch,details,Department,Location";
                string sFields = "", sFieldValue = "";
                if (objMdl.order_date != null && objMdl.order_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", order_date";
                    sFieldValue = sFieldValue + ", '" + objMdl.order_date.ToString("yyyy/MM/dd") + "'";
                }                
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objMdl.orderno + "','" + objMdl.provider_type + "','" + objMdl.supplier_name 
                    + "','" + objMdl.customer_name + "','" + objMdl.remark + "','" + objGlobalData.GetCurrentUserSession().empid
                    + "','" + objMdl.branch + "','" + objMdl.details + "','" + objMdl.Department + "','" + objMdl.Location + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_materials = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_materials))
                {
                    if (id_materials > 0 && Convert.ToInt32(objList.MaterialList.Count) > 0)
                    {
                        objList.MaterialList[0].id_materials = id_materials.ToString();
                        FunAddSuppliedMaterialTans(objList);
                    }

                    //if (id_materials > 0)
                    //{
                    //    string LocationName = objGlobalData.GetCompanyBranchNameById(sBranch);
                    //    DataSet dsData = objGlobalData.GetReportNo("FIRE", "", LocationName);
                    //    if (dsData != null && dsData.Tables.Count > 0)
                    //    {
                    //        report_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    //    }
                    //    string sql = "update t_supplied_materials set report_no='" + report_no + "' where id_materials = '" + id_materials + "'";

                    //    return objGlobalData.ExecuteQuery(sql);
                    //}
                }
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSuppliedMaterial: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddSuppliedMaterialTans(SuppliedMaterialModelsList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_supplied_materials_trans where id_materials='" + objList.MaterialList[0].id_materials + "'; ";

                for (int i = 0; i < objList.MaterialList.Count; i++)
                {
                    string sid_fireequip_trans = "null";
                    string sqty_date = "";                    

                    //if (objList.MaterialList[i].id_fireequip_trans != null)
                    //{
                    //    sid_fireequip_trans = objList.MaterialList[i].id_fireequip_trans;
                    //}
                    if (objList.MaterialList[i].qty_date != null && objList.MaterialList[i].qty_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sqty_date = objList.MaterialList[i].qty_date.ToString("yyyy-MM-dd");
                    }

                    sSqlstmt = sSqlstmt + " insert into t_supplied_materials_trans (id_materials_trans,id_materials,operation_type," +
                        "quantity,done_by,qty_date)"
                    + " values(" + sid_fireequip_trans + "," + objList.MaterialList[0].id_materials + ",'"
                    + objList.MaterialList[i].operation_type + "','" + objList.MaterialList[i].quantity + "','"
                    + objList.MaterialList[i].done_by + "','" + sqty_date + "')"
                    + " ON DUPLICATE KEY UPDATE" + " id_materials_trans= values(id_materials_trans), id_materials= values(id_materials), operation_type = values(operation_type)," +
                    " quantity= values(quantity), done_by= values(done_by), qty_date= values(qty_date) ; ";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSuppliedMaterialTans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateSuppliedMaterial(SuppliedMaterialModels objMdl, SuppliedMaterialModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_supplied_materials set orderno ='" + objMdl.orderno + "', provider_type = '" + objMdl.provider_type 
                    + "', supplier_name = '" + objMdl.supplier_name + "', customer_name= '" + objMdl.customer_name + "', remark= '" + objMdl.remark 
                    + "', details= '" + objMdl.details + "', branch= '" + objMdl.branch + "', Department= '" + objMdl.Department + "', Location= '" + objMdl.Location + "'";

                if (objMdl.order_date != null && objMdl.order_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", order_date='" + objMdl.order_date.ToString("yyyy/MM/dd") + "'";
                }
               
                sSqlstmt = sSqlstmt + " where id_materials='" + objMdl.id_materials + "'";

                int id_materials = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_materials))
                {
                    if (Convert.ToInt32(objList.MaterialList.Count) > 0)
                    {
                        objList.MaterialList[0].id_materials = objMdl.id_materials;
                        FunAddSuppliedMaterialTans(objList);
                    }
                    else
                    {
                        FunUpdateSuppliedMaterialTrans(objMdl.id_materials);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSuppliedMaterial: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateSuppliedMaterialTrans(string sid_materials)
        {
            try
            {
                string sSqlstmt = "delete from t_supplied_materials_trans where id_materials='" + sid_materials + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSuppliedMaterialTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteSuppliedMaterial(string sid_materials)
        {
            try
            {
                string sSqlstmt = "update t_supplied_materials set Active=0 where id_materials='" + sid_materials + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSuppliedMaterial: " + ex.ToString());
            }
            return false;
        }
    }
    public class SuppliedMaterialModelsList
    {
        public List<SuppliedMaterialModels> MaterialList { get; set; }
    }
}