using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class DieselEquipmentModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "ID")]
        public string Diesel_Equip_Id { get; set; }

        [Required]
        [System.Web.Mvc.Remote("doesEquipNoExist", "DieselEquipment", HttpMethod = "POST", ErrorMessage = "Vehicle/Equipment  Number. already exists. Please enter a different No.")]
        [Display(Name = "Vehicle/Equipment No.")]
        public string Equip_No { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Make")]
        public string Make { get; set; }

        [Required]
        [Display(Name = "Capacity")]
        public string Capacity { get; set; }

        [Required]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required]
        [Display(Name = "Chasis No.")]
        public string Chasis_No { get; set; }

        [Required]
        [Display(Name = "Engine No.")]
        public string Engine_No { get; set; }

        [Required]
        [Display(Name = "Date Of Purchase")]
        public DateTime Date_Of_Purchase { get; set; }

        [Required]
        [Display(Name = "Registration Issued Date")]
        public DateTime Reg_Issued_Date { get; set; }

        [Required]
        [Display(Name = "Registration Expiry Date")]
        public DateTime Reg_Expr_Date { get; set; }

        [Required]
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [Required]
        [Display(Name = "Equipment Status")]
        public string Equp_Status { get; set; }

        [Required]
        [Display(Name = "Index No")]
        public string Index_no { get; set; }


        [Required]
        [Display(Name = "Plate No")]
        public string Plate_no { get; set; }

        [Required]
        [Display(Name = "Insurance Expiry Date")]
        public DateTime Insurance_ExpDate { get; set; }

        [Required]
        [Display(Name = "Color Code")]
        public string color_code_Reg { get; set; }

        [Required]
        [Display(Name = "Color Code")]
        public string color_code_Issue { get; set; }

        internal bool FunAddDieselEquipment(DieselEquipmentModels objDieselEquipment)
        {
            try
            {
                string sColumn = "", sValues = "";
                string user = "";
              
                    user = objGlobalData.GetCurrentUserSession().empid;
                
                string sSqlstmt = "insert into t_diesel_equipment (Equip_No, Description, Make, Capacity, Model, Chasis_No, Engine_No,"
                    + " LoggedBy,Index_no,Plate_no";

                if (objDieselEquipment.Date_Of_Purchase > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Date_Of_Purchase";
                    sValues = sValues + ", '" + objDieselEquipment.Date_Of_Purchase.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objDieselEquipment.Reg_Issued_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Reg_Issued_Date";
                    sValues = sValues + ", '" + objDieselEquipment.Reg_Issued_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objDieselEquipment.Reg_Expr_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Reg_Expr_Date";
                    sValues = sValues + ", '" + objDieselEquipment.Reg_Expr_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objDieselEquipment.Insurance_ExpDate > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Insurance_ExpDate";
                    sValues = sValues + ", '" + objDieselEquipment.Insurance_ExpDate.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objDieselEquipment.Equip_No + "','" + objDieselEquipment.Description
                    + "','" + objDieselEquipment.Make + "','" + objDieselEquipment.Capacity + "','" + objDieselEquipment.Model
                 + "','" + objDieselEquipment.Chasis_No + "','" + objDieselEquipment.Engine_No + "','" + user
                 + "','" + objDieselEquipment.Index_no + "','" + objDieselEquipment.Plate_no + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDieselEquipment: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateDieselEquipment(DieselEquipmentModels objDieselEquipment)
        {
            try
            {
                string sSqlstmt = "update t_diesel_equipment set Equip_No='" + objDieselEquipment.Equip_No + "', Description='" + objDieselEquipment.Description
                    + "', Make='" + objDieselEquipment.Make + "', Capacity='" + objDieselEquipment.Capacity + "', Model='" + objDieselEquipment.Model
                 + "', Chasis_No='" + objDieselEquipment.Chasis_No + "', Engine_No='" + objDieselEquipment.Engine_No
                 + "', Index_no='" + objDieselEquipment.Index_no + "', Plate_no='" + objDieselEquipment.Plate_no + "'";

                if (objDieselEquipment.Date_Of_Purchase > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Date_Of_Purchase='" + objDieselEquipment.Date_Of_Purchase.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objDieselEquipment.Reg_Issued_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Reg_Issued_Date='" + objDieselEquipment.Reg_Issued_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objDieselEquipment.Reg_Expr_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Reg_Expr_Date='" + objDieselEquipment.Reg_Expr_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objDieselEquipment.Insurance_ExpDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Insurance_ExpDate='" + objDieselEquipment.Insurance_ExpDate.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where Diesel_Equip_Id='" + objDieselEquipment.Diesel_Equip_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDieselEquipment: " + ex.ToString());
            }
            return false;

        }

        internal bool FunDeleteEquipment(string sDiesel_Equip_Id)
        {
            try
            {
                if (sDiesel_Equip_Id != "")
                {
                    string sSqlstmt = "update t_diesel_equipment set Equp_Status=0 where Diesel_Equip_Id='" + sDiesel_Equip_Id + "'";

                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteEquipment: " + ex.ToString());
            }
            return false;
        }

        public bool checkEquipNoExists(string Equip_No)
        {
            try
            {
                if (Equip_No != "")
                {
                    string sSqlstmt = "select Diesel_Equip_Id from t_diesel_equipment where Equp_Status='1' and Equip_No='" + Equip_No + "' ";
                    DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteEquipment: " + ex.ToString());
            }
            return true;
        }


        public MultiSelectList GetDieselEquipmentList()
        {
            DropdownList objProductList = new DropdownList();
            objProductList.lstDropdown = new List<DropdownItems>();

            try
            {
                //string sSsqlstmt = "select ProductId, ProductName from t_product where Active=1";
                string sSsqlstmt = "select Diesel_Equip_Id as Id, concat(Equip_No,'-',Description) as Name from t_diesel_equipment where Equp_Status=1 order by Equip_No asc";

                DataSet dsBranch =objGlobalData.Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objProduct = new DropdownItems()
                        {
                            Id = dsBranch.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsBranch.Tables[0].Rows[i]["Name"].ToString()
                        };

                        objProductList.lstDropdown.Add(objProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDieselEquipmentList: " + ex.ToString());
            }

            return new MultiSelectList(objProductList.lstDropdown, "Id", "Name");
        }

        public string GetDieselEquipNoById(string Diesel_Equip_Id)
        {
            try
            {
                if (Diesel_Equip_Id != "")
                {
                    DataSet dsEmp = objGlobalData.Getdetails("select Equip_No from t_diesel_equipment where Diesel_Equip_Id='" + Diesel_Equip_Id + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["Equip_No"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDieselEquipNoById: " + ex.ToString());
            }
            return "";
        }

        public decimal GetAvailableDieselStock()
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select Qty from t_diesel_stock");
                decimal dValue;
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0 && decimal.TryParse(dsEmp.Tables[0].Rows[0]["Qty"].ToString(), out dValue))
                {
                    return dValue;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAvailableDieselStock: " + ex.ToString());
            }
            return 0;
        }
    }


    public class DieselConsumptionModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "ID")]
        public string Consumption_Id { get; set; }
       
        [Required]
        [Display(Name = "Vehicle/Equipment No.")]
        public string Diesel_Equip_Id { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public decimal Qty { get; set; }

        [Required]
        [Display(Name = "Issued Date")]
        public DateTime Issued_Date { get; set; }

        [Required]
        [Display(Name = "Issued By")]
        public string Issued_By { get; set; }

        [Required]
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [Required]
        [Display(Name = "Logged Date")]
        public DateTime LoggedDate { get; set; }

        internal bool FunAddDieselConsumption(DieselConsumptionModels objDieselConsumption)
        {
            try
            {
                string sColumn = "", sValues = "";
                string user = "";
              
                    user = objGlobalData.GetCurrentUserSession().empid;
                
                string sSqlstmt = "insert into t_diesel_consumption (Diesel_Equip_Id, Qty, Issued_By, LoggedBy, LoggedDate";

                if (objDieselConsumption.Issued_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Issued_Date";
                    sValues = sValues + ", '" + objDieselConsumption.Issued_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objDieselConsumption.Diesel_Equip_Id + "','" + objDieselConsumption.Qty
                    + "','" + objDieselConsumption.Issued_By + "','" + user + "','" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDieselConsumption: " + ex.ToString());
            }
            return false;

        }


        internal bool FunUpdateDieselConsumption(DieselConsumptionModels objDieselConsumption)
        {
            try
            {
                string sSqlstmt = "update t_diesel_consumption set Diesel_Equip_Id='" + objDieselConsumption.Diesel_Equip_Id + "', Qty='" + objDieselConsumption.Qty
                    + "', Issued_By='" + objDieselConsumption.Issued_By + "'";

                if (objDieselConsumption.Issued_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Issued_Date='" + objDieselConsumption.Issued_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where Consumption_Id='" + objDieselConsumption.Consumption_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDieselConsumption: " + ex.ToString());
            }
            return false;
        }

    }


    public class DieselStockModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "ID")]
        public string Diesel_Rcvd_TransId { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public decimal Qty { get; set; }

        [Required]
        [Display(Name = "Recieved Date")]
        public DateTime Rcvd_Date { get; set; }

        [Required]
        [Display(Name = "Recieved By")]
        public string Rcvd_By { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string Branch_id { get; set; }

        [Required]
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [Required]
        [Display(Name = "Logged Date")]
        public DateTime LoggedDate { get; set; }

        internal bool FunAddDieselStock(DieselStockModels objDieselStock)
        {
            try
            {
                string sColumn = "", sValues = "";
                string sSqlstmt = "insert into t_diesel_rcvd_trans ( Qty, Rcvd_By, Branch_id, LoggedBy, LoggedDate";
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                
                if (objDieselStock.Rcvd_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Rcvd_Date";
                    sValues = sValues + ", '" + objDieselStock.Rcvd_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objDieselStock.Qty + "','" + objDieselStock.Rcvd_By
                    + "','" + objDieselStock.Branch_id + "','" + user + "','" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDieselStock: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateDieselStock(DieselStockModels objDieselStock)
        {
            try
            {
                string sSqlstmt = "update t_diesel_rcvd_trans set Qty='" + objDieselStock.Qty + "', Rcvd_By='" + objDieselStock.Rcvd_By
                    + "', Branch_id='" + objDieselStock.Branch_id + "'";

                if (objDieselStock.Rcvd_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Rcvd_Date='" + objDieselStock.Rcvd_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where Diesel_Rcvd_TransId='" + objDieselStock.Diesel_Rcvd_TransId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDieselStock: " + ex.ToString());
            }
            return false;
        }

    }

    public class DieselEquipmentModelsList
    {
        public List<DieselEquipmentModels> lstDieselEquipment { get; set; }
    }

    public class DieselConsumptionModelsList
    {
        public List<DieselConsumptionModels> lstDieselConsumption { get; set; }
    }

    public class DieselStockModelsList
    {
        public List<DieselStockModels> lstDieselStock { get; set; }
    }
}