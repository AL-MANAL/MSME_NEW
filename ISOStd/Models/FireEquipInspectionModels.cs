using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace ISOStd.Models
{
    public class FireEquipInspectionModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        public string id_FireEquip { get; set; }

        [Display(Name = "Date and Timing of Inspection")]
        public DateTime Fire_DateTime { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Date  of Next Inspection")]
        public DateTime FireNext_DateTime { get; set; }

        [Display(Name = "Smoke Detector")]
        public string Smoke_Detector { get; set; }

        [Display(Name = "Smoke Detector Remarks")]
        public string Smoke_Detector_Remarks { get; set; }

        [Display(Name = "Fire Alarm")]
        public string Fire_Alarm { get; set; }

        [Display(Name = "Fire Alarm Remarks")]
        public string Fire_Alarm_Remarks { get; set; }

        [Display(Name = " Fire Water Pumps")]
        public string Fire_Water_Pumps { get; set; }

        [Display(Name = " Fire Water Pumps Remarks")]
        public string Fire_Water_Pumps_Remarks { get; set; }
        
        [Display(Name = "  Fire.Extinguishing Box #")]
        public string Fire_Box_No { get; set; }
        
        [Display(Name = "  Fire.Extinguishing Box Location")]
        public string Fire_Box_No_Location { get; set; }

        [Display(Name = "  Fire.Extinguishing Type")]
        public string Fire_Box_No_Type { get; set; }

        [Display(Name = "  Fire.Extinguishing Box # Type Remarks")]
        public string Fire_Box_No_Type_Remarks { get; set; }

        [Display(Name = "  Fire.Extinguishing Box # Remarks")]
        public string Fire_Box_No_Remarks { get; set; }

        [Display(Name = "  Fire Hydrants")]
        public string Fire_Hydrants { get; set; }

        [Display(Name = "  Fire Hydrants Remarks")]
        public string Fire_Hydrants_Remarks { get; set; }

        [Display(Name = "Project")]
        public string Project { get; set; }

        [Display(Name = "ReportNo")]
        public string ReportNo { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        internal bool FunDeleteFireEquipInspection(string sid_FireEquip)
        {
            try
            {
                string sSqlstmt = "update t_fireequip_inspection set Active=0 where id_FireEquip='" + sid_FireEquip + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteFireequipInspectionDoc: " + ex.ToString());
            }
            return false;
        }

        internal string GetFireEquipTypeById(string sSourceID)
        {
            try
            {
                //DataSet dsEmp = objGlobaldata.Getdetails("select impact_id, impact_name from impact where impact_id='" + sImpact_id + "'");
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                     + "and header_desc='FireEquip-Type' and (item_id='" + sSourceID + "' or item_desc='" + sSourceID + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetFireEquipNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetFireEquipType()
        {
            DropdownFireEquipList lst = new DropdownFireEquipList();
            lst.lst = new List<DropdownFireEquipItems>();
            try
            {

                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='FireEquip-Type' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownFireEquipItems reg = new DropdownFireEquipItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetMultiFireEquiqList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        internal bool FunAddFireEquip(FireEquipInspectionModels objFireEquip)
        {
            try
            {
                string sFire_DateTime = objFireEquip.Fire_DateTime.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sFireNext_DateTime = objFireEquip.FireNext_DateTime.ToString("yyyy-MM-dd");
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "";
                
                {
                    sSqlstmt = sSqlstmt + "insert into t_fireequip_inspection(Fire_DateTime, Location, FireNext_DateTime, Smoke_Detector, Smoke_Detector_Remarks, Fire_Alarm, Fire_Alarm_Remarks, Fire_Water_Pumps,Fire_Water_Pumps_Remarks,Fire_Box_No," +
                        "Fire_Box_No_Location,Fire_Box_No_Type_Remarks,Fire_Box_No_Type,Fire_Box_No_Remarks,Fire_Hydrants,Fire_Hydrants_Remarks,Project,ReportNo,upload,branch )"
                                + " values('" + sFire_DateTime + "', '" + objFireEquip.Location + "','" + sFireNext_DateTime + "','" + objFireEquip.Smoke_Detector
                                 + "','" + objFireEquip.Smoke_Detector_Remarks + "','" + objFireEquip.Fire_Alarm + "','" + objFireEquip.Fire_Alarm_Remarks + "','" + objFireEquip.Fire_Water_Pumps
                                + "','" + objFireEquip.Fire_Water_Pumps_Remarks + "','" + objFireEquip.Fire_Box_No + "','" + objFireEquip.Fire_Box_No_Location + "','" + objFireEquip.Fire_Box_No_Type_Remarks
                                + "','" + objFireEquip.Fire_Box_No_Type + "','" + objFireEquip.Fire_Box_No_Remarks + "','" + objFireEquip.Fire_Hydrants + "','" + objFireEquip.Fire_Hydrants_Remarks + "','" + objFireEquip.Project + "','" + objFireEquip.ReportNo + "','" + objFireEquip.upload + "','" + sBranch + "')";
                }

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddFireEquipInspection: " + ex.ToString());
            }
            return false;
        }
    
        internal bool FunUpdateFireEquip(FireEquipInspectionModels objFireInspection)
        {
            try
            {
                string sSqlstmt = " update t_fireequip_inspection set upload='" + objFireInspection.upload + "',Smoke_Detector='" + objFireInspection.Smoke_Detector + "',Smoke_Detector_Remarks='" + objFireInspection.Smoke_Detector_Remarks + "',Fire_Alarm='" + objFireInspection.Fire_Alarm
                    + "', Fire_Alarm_Remarks='" + objFireInspection.Fire_Alarm_Remarks + "', Fire_Water_Pumps='" + objFireInspection.Fire_Water_Pumps + "',Fire_Water_Pumps_Remarks='" + objFireInspection.Fire_Water_Pumps_Remarks + "',Fire_Box_No='" + objFireInspection.Fire_Box_No + "',Fire_Box_No_Location='" + objFireInspection.Fire_Box_No_Location
                    + "',Fire_Box_No_Type_Remarks='" + objFireInspection.Fire_Box_No_Type_Remarks + "',Fire_Box_No_Type_Remarks='" + objFireInspection.Fire_Box_No_Type_Remarks + "',Fire_Box_No_Type='" + objFireInspection.Fire_Box_No_Type
                    + "',Fire_Box_No_Remarks='" + objFireInspection.Fire_Box_No_Remarks + "',Fire_Hydrants='" + objFireInspection.Fire_Hydrants + "',Fire_Hydrants_Remarks='" + objFireInspection.Fire_Hydrants_Remarks /*+ "',Project='" + objFireInspection.Project + "',ReportNo='" + objFireInspection.ReportNo*/;


                if (objFireInspection.Fire_DateTime > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + "', Fire_DateTime='" + objFireInspection.Fire_DateTime.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objFireInspection.FireNext_DateTime > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", FireNext_DateTime='" + objFireInspection.FireNext_DateTime.ToString("yyyy-MM-dd") + "' ";
                }

                sSqlstmt = sSqlstmt + " where id_FireEquip=" + objFireInspection.id_FireEquip;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDailyInspection: " + ex.ToString());
            }
            return false;
        }
    }

    public class FireEquipInspectionModelsList
    {
        public List<FireEquipInspectionModels> FireEquipList { get; set; }
    }

    public class DropdownFireEquipItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownFireEquipList
    {
        public List<DropdownFireEquipItems> lst { get; set; }
    }
}