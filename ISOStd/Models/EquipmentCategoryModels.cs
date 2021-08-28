using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data;
namespace ISOStd.Models
{
    public class EquipmentCategoryModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int id_cat { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required]
        [Display(Name = "Name")]
        [System.Web.Mvc.Remote("doesCategoryNameExist", "EquipmentCategory", HttpMethod = "POST", ErrorMessage = "Already Exists")]
        public string Cat_name { get; set; }

        internal bool FunDeleteEquipment(string sid_cat)
        {
            try
            {
                string sSqlstmt = "update t_equipment_category set Active=0 where id_cat='" + sid_cat + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteEquipment: " + ex.ToString());
            }
            return false;
        }
        internal bool CheckForCategoryNameExists(string sCat_name)
        {
            try
            {
                DataSet dsDoc = objGlobalData.Getdetails("select Cat_name from t_equipment_category where Active=1 and (Cat_name='" + sCat_name + "' or"
                +"Cat_name like '%"+Cat_name+"%' or Cat_name like '"+sCat_name+"%' or Cat_name like '%"+sCat_name+"' )");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in CheckForCategoryNameExists: " + ex.ToString());
            }
            return true;
        }


        internal bool FunDeleteCategory(string sid_cat)
        {
            try
            {
                string sSqlstmt = "update t_equipment_category set Active=0 where id_cat='" + sid_cat + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCategory: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddCategory(EquipmentCategoryModels objCat)
        {
            try
            {
                string sSqlstmt = "";
                sSqlstmt = "insert into t_equipment_category(Category,Cat_name)"
                + "values('" + objCat.Category + "','" + objCat.Cat_name + "')";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCategory: " + ex.ToString());
            }
            return false;
        }

        internal string getCategoryNamById(string cat_name)
        {
            try
            {
                if (cat_name != "")
                {
                    string sSsqlstmt = "select Cat_name from t_equipment_category where id_cat='" + cat_name + "'";
                    DataSet dsData =objGlobalData.Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Cat_name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in getCategoryNamById: " + ex.ToString());
            }
            return "";
        }
    }

    public class EquipmentDetailModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        // truck 
        [Display(Name = "Id")]
        public int id_equipment { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Name")]
        public string Cat_name { get; set; }

        [Display(Name = "Vehicle No")]
        public string vehicle_no { get; set; }

        [Display(Name = "Vehicle Registered In")]
        public string vehicle_reg { get; set; }

        [Display(Name = "Vehicle Renewal Date")]
        public DateTime vehicle_renewal_date { get; set; }

        [Display(Name = "Vehicle Renewal Due Date")]
        public DateTime vehicle_renewal_due_date { get; set; }

        [Display(Name = "Vehicle card")]
        public string vehicle_upload { get; set; }

        [Display(Name = "Model")]
        public string vehicle_model { get; set; }

        [Display(Name = "Make")]
        public string vehicle_make { get; set; }

        [Display(Name = "Chasis No")]
        public string chasis_no { get; set; }

        [Display(Name = "Engine No")]
        public string engine_no { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "GPS No")]
        public string gpsno { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        [Display(Name = "Vehicle Color")]
        public string vehicle_color { get; set; }

        [Display(Name = "Plate Type")]
        public string plate_type { get; set; }

        [Display(Name = "Current Driver")]
        public string driver { get; set; }

        [Display(Name = "ADNOC Card")]
        public string card { get; set; }

        [Display(Name = "ADNOC Card No")]
        public string card_no { get; set; }

        [Display(Name = "ADNOC Card Expiry Date")]
        public DateTime card_expdate { get; set; }

        [Display(Name = "ADPC Stickers")]
        public string ADPC_stickers { get; set; }

        [Display(Name = "ADPC RFID")]
        public string ADPC_RFID { get; set; }

        [Display(Name = "Star Energy")]
        public string star_energy { get; set; }

        [Display(Name = "GCC Drivers")]
        public string GCC_drivers { get; set; }

        [Display(Name = "Civil defence card")]
        public string civildefence_card { get; set; }

        [Display(Name = "Civil defence card")]
        public string civildefence_upload { get; set; }

        [Display(Name = "Civil defence card expiry")]
        public DateTime civildefence_expiry { get; set; }

        //Pass

        [Display(Name = "Id")]
        public int id_pass { get; set; }

        [Display(Name = "Pass Type")]
        public string PassType { get; set; }

        [Display(Name = "Upload")]
        public string Upload { get; set; }

        [Display(Name = "Pass Expiry Date")]
        public DateTime ExpDate { get; set; }

        //trailer

        [Display(Name = "Id")]
        public int id_trailer { get; set; }

        [Display(Name = "Trailer No")]
        public string trailerno { get; set; }

        [Display(Name = "Trailer Reg")]
        public string trailerreg { get; set; }

        [Display(Name = "Reg Date")]
        public DateTime reg_date { get; set; }

        [Display(Name = "Exp Date")]
        public DateTime exp_date { get; set; }

        [Display(Name = "Passing Date")]
        public DateTime passing_date { get; set; }

        [Display(Name = "Current Location")]
        public string cur_location { get; set; }

        [Display(Name = "YOM")]
        public string yom { get; set; }

        [Display(Name = "Chasis No")]
        public string chasisno { get; set; }

        [Display(Name = "Trailer Length")]
        public string trailerlen { get; set; }

        [Display(Name = "Trailer Type")]
        public string trailertype { get; set; }

        [Display(Name = "Height")]
        public string height { get; set; }

        [Display(Name = "Company")]
        public string company { get; set; }

        [Display(Name = "GVW")]
        public string GVW { get; set; }

        [Display(Name = "Suspension Type")]
        public string suspensiontype { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        public string truck { get; set; }
        public string trailer { get; set; }

        internal bool FunDeleteVehiclePass(string sid_pass)
        {
            try
            {
                string sSqlstmt = "update t_truck_pass set Active=0 where id_pass='" + sid_pass + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteVehiclePass: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteTrailer(string sid_trailer)
        {
            try
            {
                string sSqlstmt = "update t_trailer_detail set Active=0 where id_trailer='" + sid_trailer + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteTrailer: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteEquipmentDetails(string sid_equipment)
        {
            try
            {
                string sSqlstmt = "update t_truck_detail set Active=0 where id_equipment='" + sid_equipment + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteEquipmentDetails: " + ex.ToString());
            }
            return false;
        }
   
     
        public MultiSelectList getVehicleList()
        {
            EquipmentCategoryModelsList objEqpList = new EquipmentCategoryModelsList();
            objEqpList.lstCat = new List<EquipmentCategoryModels>();
            try
            {
                string sSsqlstmt = "select id_cat,Cat_name from t_equipment_category c,dropdownheader d,dropdownitems dd"
                + " where Active=1 and d.header_id=dd.header_id and c.Category=dd.item_id and dd.item_desc='Vehicle'";
                DataSet dsReportType =objGlobalData.Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        EquipmentCategoryModels objEqp = new EquipmentCategoryModels()
                        {
                            id_cat =Convert.ToInt32(dsReportType.Tables[0].Rows[i]["id_cat"].ToString()),
                            Cat_name = dsReportType.Tables[0].Rows[i]["Cat_name"].ToString()
                        };
                        objEqpList.lstCat.Add(objEqp);
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in getVehicleList: " + ex.ToString());
            }
            return new MultiSelectList(objEqpList.lstCat, "id_cat", "Cat_name");
        }

        public MultiSelectList getEquipmentList()
        {
            EquipmentCategoryModelsList objEqpList = new EquipmentCategoryModelsList();
            objEqpList.lstCat = new List<EquipmentCategoryModels>();
            try
            {
                string sSsqlstmt = "select id_cat,Cat_name from t_equipment_category c,dropdownheader d,dropdownitems dd"
                + " where Active=1 and d.header_id=dd.header_id and c.Category=dd.item_id and dd.item_desc='Equipment'";
                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        EquipmentCategoryModels objEqp = new EquipmentCategoryModels()
                        {
                            id_cat = Convert.ToInt32(dsReportType.Tables[0].Rows[i]["id_cat"].ToString()),
                            Cat_name = dsReportType.Tables[0].Rows[i]["Cat_name"].ToString()
                        };
                        objEqpList.lstCat.Add(objEqp);
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in getEquipmentList: " + ex.ToString());
            }
            return new MultiSelectList(objEqpList.lstCat, "id_cat", "Cat_name");
        }

        internal bool FunAddVehicleDetails(EquipmentDetailModels objEqp)
        {
            try
            {
               
                    string sSqlstmt = "";
                    sSqlstmt = "insert into t_truck_detail (Cat_name,vehicle_no,vehicle_reg,"
                    + "vehicle_model,vehicle_make,chasis_no,engine_no,location,gpsno,status,"
                    + "vehicle_color,plate_type,driver,card,card_no,ADPC_stickers,ADPC_RFID,star_energy,GCC_drivers,civildefence_card";
                    string sFields = "", sFieldValue = "";

                    if (objEqp.vehicle_renewal_date != null && objEqp.vehicle_renewal_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", vehicle_renewal_date";
                        sFieldValue = sFieldValue + ", '" + objEqp.vehicle_renewal_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objEqp.vehicle_renewal_due_date != null && objEqp.vehicle_renewal_due_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", vehicle_renewal_due_date";
                        sFieldValue = sFieldValue + ", '" + objEqp.vehicle_renewal_due_date.ToString("yyyy/MM/dd") + "'";
                    }

                    if (objEqp.card_expdate != null && objEqp.card_expdate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", card_expdate";
                        sFieldValue = sFieldValue + ", '" + objEqp.card_expdate.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objEqp.civildefence_expiry != null && objEqp.civildefence_expiry > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", civildefence_expiry";
                        sFieldValue = sFieldValue + ", '" + objEqp.civildefence_expiry.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objEqp.vehicle_upload != null && objEqp.vehicle_upload != "")
                    {
                        sFields = sFields + ", vehicle_upload";
                        sFieldValue = sFieldValue + ", '" + objEqp.vehicle_upload + "'";
                    }
                    if (objEqp.civildefence_upload != null && objEqp.civildefence_upload != "")
                    {
                        sFields = sFields + ", civildefence_upload";
                        sFieldValue = sFieldValue + ", '" + objEqp.civildefence_upload + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;

                    sSqlstmt = sSqlstmt + ") values('" + objEqp.Cat_name + "','" + objEqp.vehicle_no + "'"
                    + ",'" + objEqp.vehicle_reg + "'"
                    + ",'" + objEqp.vehicle_model + "','" + objEqp.vehicle_make + "','" + objEqp.chasis_no + "','" + objEqp.engine_no + "'"
                    + ",'" + objEqp.location + "','" + objEqp.gpsno + "','" + objEqp.status + "','" + objEqp.vehicle_color + "'"
                    + ",'" + objEqp.plate_type + "','" + objEqp.driver + "','" + objEqp.card + "','" + objEqp.card_no + "','" + objEqp.ADPC_stickers + "'"
                    + ",'" + objEqp.ADPC_RFID + "','" + objEqp.star_energy + "','" + objEqp.GCC_drivers + "','" + objEqp.civildefence_card + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    return objGlobalData.ExecuteQuery(sSqlstmt); 
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddVehicleDetails: " + ex.ToString());

            }
            return false;
        }
        internal bool FunAddEquipmentDetails(EquipmentDetailModels objEqp)
        {
            try
            {
                   string sSqlstmt = "";
                    sSqlstmt = "insert into t_trailer_detail (Cat_name,trailerno,trailerreg,cur_location,yom,chasisno,"
                    + "trailerlen,trailertype,height,company,GVW,suspensiontype,remarks";
                    string sFields = "", sFieldValue = "";

                    if (objEqp.reg_date != null && objEqp.reg_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", reg_date";
                        sFieldValue = sFieldValue + ", '" + objEqp.reg_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objEqp.exp_date != null && objEqp.exp_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", exp_date";
                        sFieldValue = sFieldValue + ", '" + objEqp.exp_date.ToString("yyyy/MM/dd") + "'";
                    }

                    if (objEqp.passing_date != null && objEqp.passing_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", passing_date";
                        sFieldValue = sFieldValue + ", '" + objEqp.passing_date.ToString("yyyy/MM/dd") + "'";
                    }

                    sSqlstmt = sSqlstmt + sFields;

                    sSqlstmt = sSqlstmt + ") values('" + objEqp.Cat_name + "','" + objEqp.trailerno + "'"
                    + ",'" + objEqp.trailerreg + "'"
                    + ",'" + objEqp.cur_location + "','" + objEqp.yom + "','" + objEqp.chasisno + "','" + objEqp.trailerlen + "'"
                    + ",'" + objEqp.trailertype + "','" + objEqp.height + "','" + objEqp.company + "','" + objEqp.GVW + "'"
                    + ",'" + objEqp.suspensiontype + "','" + objEqp.remarks + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEquipmentDetails: " + ex.ToString());

            }
            return false;
        }
        internal bool FunUpdateVehicleDetails(EquipmentDetailModels objEqp)
        {
            try
            {
                   string sSqlstmt = "update t_truck_detail set  Cat_name='" + objEqp.Cat_name + "', "
                        + "vehicle_no='" + objEqp.vehicle_no + "',vehicle_reg='" + objEqp.vehicle_reg + "'"
                        + ",vehicle_model='" + objEqp.vehicle_model + "',vehicle_make='" + objEqp.vehicle_make + "',chasis_no='" + objEqp.chasis_no + "'"
                        + ",engine_no='" + objEqp.engine_no + "',location='" + objEqp.location + "'"
                        + ",gpsno='" + objEqp.gpsno + "',status='" + objEqp.status + "',vehicle_color='" + objEqp.vehicle_color + "',plate_type='" + objEqp.plate_type + "'"
                        + ",driver='" + objEqp.driver + "',card='" + objEqp.card + "',card_no='" + objEqp.card_no + "',ADPC_stickers='" + objEqp.ADPC_stickers + "'"
                        + ",ADPC_RFID='" + objEqp.ADPC_RFID + "',star_energy='" + objEqp.star_energy + "',GCC_drivers='" + objEqp.GCC_drivers + "',civildefence_card='" + objEqp.civildefence_card + "'"
                        + ",vehicle_upload='" + objEqp.vehicle_upload + "'";

                    if (objEqp.vehicle_renewal_date != null && objEqp.vehicle_renewal_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sSqlstmt = sSqlstmt + ", vehicle_renewal_date='" + objEqp.vehicle_renewal_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objEqp.vehicle_renewal_due_date != null && objEqp.vehicle_renewal_due_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sSqlstmt = sSqlstmt + ", vehicle_renewal_due_date='" + objEqp.vehicle_renewal_due_date.ToString("yyyy/MM/dd") + "'";
                    }

                    if (objEqp.card_expdate != null && objEqp.card_expdate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sSqlstmt = sSqlstmt + ", card_expdate='" + objEqp.card_expdate.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objEqp.civildefence_expiry != null && objEqp.civildefence_expiry > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sSqlstmt = sSqlstmt + ", civildefence_expiry='" + objEqp.civildefence_expiry.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objEqp.civildefence_upload != null && objEqp.civildefence_upload != "")
                    {
                        sSqlstmt = sSqlstmt + ", civildefence_upload='" + objEqp.civildefence_upload + "'";
                    }

                    sSqlstmt = sSqlstmt + " where id_equipment='" + objEqp.id_equipment + "'";
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                
                
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateVehicleDetails: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateEquipmentDetails(EquipmentDetailModels objEqp)
        {
            try
            {
                
              string sSqlstmt = "update t_trailer_detail set Cat_name='" + objEqp.Cat_name + "', "
                        + "trailerno='" + objEqp.trailerno + "',trailerreg='" + objEqp.trailerreg + "'"
                        + ",cur_location='" + objEqp.cur_location + "',yom='" + objEqp.yom + "',chasisno='" + objEqp.chasisno + "'"
                        + ",trailerlen='" + objEqp.trailerlen + "',trailertype='" + objEqp.trailertype + "'"
                        + ",height='" + objEqp.height + "',company='" + objEqp.company + "',GVW='" + objEqp.GVW + "',suspensiontype='" + objEqp.suspensiontype + "'"
                        + ",remarks='" + objEqp.remarks + "'";

                    if (objEqp.reg_date != null && objEqp.reg_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sSqlstmt = sSqlstmt + ", reg_date='" + objEqp.reg_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objEqp.exp_date != null && objEqp.exp_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sSqlstmt = sSqlstmt + ", exp_date='" + objEqp.exp_date.ToString("yyyy/MM/dd") + "'";
                    }

                    if (objEqp.passing_date != null && objEqp.passing_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sSqlstmt = sSqlstmt + ", passing_date='" + objEqp.passing_date.ToString("yyyy/MM/dd") + "'";
                    }


                    sSqlstmt = sSqlstmt + " where id_trailer='" + objEqp.id_trailer + "'";
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEquipmentDetails: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddPassDetails(EquipmentDetailModels objPass, EquipmentDetailModelsList lstpass)
        {

            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < lstpass.lstCat.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_truck_pass (id_equipment,PassType";

                    string sFields = "", sFieldValue = "";

                    if (lstpass.lstCat[i].ExpDate != null && lstpass.lstCat[i].ExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", ExpDate";
                        sFieldValue = sFieldValue + ", '" + lstpass.lstCat[i].ExpDate.ToString("yyyy/MM/dd") + "'";
                    }

                    if (lstpass.lstCat[i].Upload != null && lstpass.lstCat[i].Upload != "")
                    {
                        sFields = sFields + ", Upload";
                        sFieldValue = sFieldValue + ", '" + lstpass.lstCat[i].Upload + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objPass.id_equipment + "','" + lstpass.lstCat[i].PassType + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPassDetails: " + ex.ToString());

            }
            return false;
        }
        internal bool FunUpdatePass(EquipmentDetailModels objPass)
        {
            try
            {
                string sSqlstmt = "update t_truck_pass set PassType ='" + objPass.PassType + "'";

                if (objPass.Upload != null)
                {
                    sSqlstmt = sSqlstmt + ", Upload='" + objPass.Upload + "'";
                }

                if (objPass.ExpDate != null && objPass.ExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {

                    sSqlstmt = sSqlstmt + ", ExpDate='" + objPass.ExpDate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_pass='" + objPass.id_pass + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePass: " + ex.ToString());
            }

            return false;
        }
    }

    public class VehiclePassModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int id_vehicle_pass { get; set; }

        [Display(Name = "Id")]
        public int id_equipment { get; set; }

        [Display(Name = "Insurance Type")]
        public string Insurance_type { get; set; }

        [Display(Name = "Upload")]
        public string Upload { get; set; }

        [Display(Name = "Mulkia Upload")]
        public string Mulki_upload { get; set; }

        [Display(Name = "Insurance Date")]
        public DateTime Insurance_date { get; set; }

        [Display(Name = "Insurance Expiry Date")]
        public DateTime Insurance_expdate { get; set; }

        [Display(Name = "Insurance cover")]
        public string  Insurance_cover { get; set; }

        [Display(Name = "Insurance company")]
        public string Company { get; set; }

       
        internal bool FunDeleteInsurance(string sid_vehicle_pass)
        {
            try
            {
                string sSqlstmt = "update t_vehicle_insurance set Active=0 where id_vehicle_pass='" + sid_vehicle_pass + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeletePass: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInsurance(VehiclePassModels objVehicle)
        {
            try
            {
                string sSqlstmt = "update t_vehicle_insurance set Insurance_type ='" + objVehicle.Insurance_type + "',Insurance_cover ='" + objVehicle.Insurance_cover + "'"
                    + ",Company ='" + objVehicle.Company + "'";

                if (objVehicle.Upload != null)
                {
                    sSqlstmt = sSqlstmt + ", Upload='" + objVehicle.Upload + "'";
                }
                if (objVehicle.Mulki_upload != null)
                {
                    sSqlstmt = sSqlstmt + ", Mulki_upload='" + objVehicle.Mulki_upload + "'";
                }
                if (objVehicle.Insurance_date != null && objVehicle.Insurance_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Insurance_date='" + objVehicle.Insurance_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objVehicle.Insurance_expdate != null && objVehicle.Insurance_expdate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Insurance_expdate='" + objVehicle.Insurance_expdate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_vehicle_pass='" + objVehicle.id_vehicle_pass + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInsurance: " + ex.ToString());
            }

            return false;
        }
        internal bool FunAddInsuranceDetails(VehiclePassModels objVehicle, VehiclePassModelsList lstIns)
        {

            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < lstIns.lstPass.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_vehicle_insurance (id_equipment,Insurance_type,Insurance_cover,Company";

                    string sFields = "", sFieldValue = "";

                    if (lstIns.lstPass[i].Insurance_date != null && lstIns.lstPass[i].Insurance_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", Insurance_date";
                        sFieldValue = sFieldValue + ", '" + lstIns.lstPass[i].Insurance_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (lstIns.lstPass[i].Insurance_expdate != null && lstIns.lstPass[i].Insurance_expdate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", Insurance_expdate";
                        sFieldValue = sFieldValue + ", '" + lstIns.lstPass[i].Insurance_expdate.ToString("yyyy/MM/dd") + "'";
                    }
                    if (lstIns.lstPass[i].Upload != null && lstIns.lstPass[i].Upload != "")
                    {
                        sFields = sFields + ", Upload";
                        sFieldValue = sFieldValue + ", '" + lstIns.lstPass[i].Upload + "'";
                    }
                    if (lstIns.lstPass[i].Mulki_upload != null && lstIns.lstPass[i].Mulki_upload != "")
                    {
                        sFields = sFields + ", Mulki_upload";
                        sFieldValue = sFieldValue + ", '" + lstIns.lstPass[i].Mulki_upload + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objVehicle.id_equipment + "','" + lstIns.lstPass[i].Insurance_type + "','" + lstIns.lstPass[i].Insurance_cover + "',"
                        + "'" + objVehicle.Company + "' ";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInsuranceDetails: " + ex.ToString());

            }
            return false;
        }
    }
    public class EquipmentCategoryModelsList
    {
        public List<EquipmentCategoryModels> lstCat { get; set; }
    }

    public class EquipmentDetailModelsList
    {
        public List<EquipmentDetailModels> lstCat { get; set; }
    }
    public class VehiclePassModelsList
    {
        public List<VehiclePassModels> lstPass { get; set; }
    }

}