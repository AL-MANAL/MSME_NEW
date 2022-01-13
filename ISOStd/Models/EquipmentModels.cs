using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{
    public class EquipmentModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string Equipment_Id { get; set; }

        [Required]
        [Display(Name = "Equipment Name")]
        public string Equipment_Name { get; set; }

        [Required]
        [Display(Name = "Serial Number/Equipment No")]
        [System.Web.Mvc.Remote("doesEquipmentExist", "Equipment", HttpMethod = "POST", ErrorMessage = "Equipment with same Serial No. already exists.")]
        public string Equipment_serial_no { get; set; }
               
        [Display(Name = "Model Number")]
        public string Model_No { get; set; }
                
        [Display(Name = "Purpose of Machine/Instrument")]
        public string Equipment_Application { get; set; }

        [Required]
        [Display(Name = "Source of calibration")]
        public string Source_of_calibration { get; set; }

        [Required]
        [Display(Name = "Calibration Frequency")]
        public string Freq_of_calibration { get; set; }

        [Required]
        [Display(Name = "Commissioning Date")]
        public DateTime Commissioning_Date { get; set; }

        [Required]
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Equipment_status { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Updated On")]
        public DateTime Logged_date { get; set; }

        [Display(Name = "Document")]
        public string DocUploadPath { get; set; }

        [Display(Name = "Notified To")]
        public string RespPerson { get; set; }

        [Display(Name = "Equipment Type")]
        public string equp_type { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "Equipment Location")]
        public string Equipment_location { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Range")]
        public string eqp_range { get; set; }


        public DateTime Next_Maint_Date { get; set; }

        internal bool FunDeleteEquipmentDoc(string sEquipment_Id)
        {
            try
            {
                string sSqlstmt = "update t_equipment set Active=0 where Equipment_Id='" + sEquipment_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteEquipmentDoc: " + ex.ToString());

            }
            return false;
        }
        internal bool FunDeleteCalibrationDoc(string scalibration_id)
        {
            try
            {
                string sSqlstmt = "update t_calibration set Active=0 where calibration_id='" + scalibration_id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCalibrationDoc: " + ex.ToString());

            }
            return false;
        }

        internal bool FunAddEquipment(EquipmentModels objEquipmentModels)
        {
            try
            {
                //string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sCommissioning_Date = objEquipmentModels.Commissioning_Date.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into t_equipment (Equipment_serial_no, Equipment_Name, Equipment_Application, Source_of_calibration, Freq_of_calibration, "
                    + "Commissioning_Date, Manufacturer, Equipment_status, Model_No,Department,DocUploadPath,RespPerson,equp_type,branch,location,Equipment_location,eqp_range)"
                + " values('" + objEquipmentModels.Equipment_serial_no + "','" + objEquipmentModels.Equipment_Name + "','" + objEquipmentModels.Equipment_Application
                + "','" + objEquipmentModels.Source_of_calibration + "','" + objEquipmentModels.Freq_of_calibration + "','" + sCommissioning_Date + "','"
                + objEquipmentModels.Manufacturer + "','" + objEquipmentModels.Equipment_status + "','" + objEquipmentModels.Model_No + "','" + objEquipmentModels.Department + "'"
                + ",'" + objEquipmentModels.DocUploadPath + "','" + objEquipmentModels.RespPerson + "','" + objEquipmentModels.equp_type + "','" + objEquipmentModels.branch + "','" + objEquipmentModels.location + "','" + objEquipmentModels.Equipment_location + "','" + eqp_range + "')";


                int Equipment_Id = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Equipment_Id))
                {
                    if (Equipment_Id > 0)
                    {
                        return sendEquipmentMail(Equipment_Id, "Equipment Details");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEquipment: " + ex.ToString());

            }
            return false;
        }

        internal bool sendEquipmentMail(int Equipment_Id, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select Equipment_Id,equp_type,Equipment_serial_no,Equipment_Name,Model_No,Freq_of_calibration,Commissioning_Date,Source_of_calibration,Equipment_status,RespPerson from  t_equipment where Equipment_Id='" + Equipment_Id + "'";

                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "", sCCEmailIds="";

                    //using streamreader for reading my htmltemplate 
                    //Form the Email Subject and Body content
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
                    //string sName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["RespPerson"].ToString());
                   // string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["pers_resp"].ToString());

                    string Commissioning_Date = "";
                    if (dsData.Tables[0].Rows[0]["Commissioning_Date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["Commissioning_Date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        Commissioning_Date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["Commissioning_Date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Equipment Type:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["equp_type"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Serial Number/Equipment No:<b></td> <td colspan=3>" + dsData.Tables[0].Rows[0]["Equipment_serial_no"].ToString() + "</td></tr>"
                    + "<tr><td colspan=3><b>Equipment Name:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["Equipment_Name"].ToString()) +"</td></tr>"

                     + "<tr><td colspan=3><b>Model Number:< b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["Model_No"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Calibration Frequency:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["Freq_of_calibration"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Commissioning Date:<b></td> <td colspan=3>" + Commissioning_Date + "</td></tr>"

                    + "<tr><td colspan=3><b>Source of calibration:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["Source_of_calibration"].ToString()) + "</td></tr>"
                    +"<tr><td colspan=3><b>Status:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["Equipment_status"].ToString()) + "</td></tr>";



                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Equipment Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in sendEquipmentMail: " + ex.ToString());
            }
            return false;
        }



        internal bool FunUpdateEquipment(EquipmentModels objEquipmentModels)
        {
            try
            {
                string sCommissioning_Date = objEquipmentModels.Commissioning_Date.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_equipment set  Equipment_Name='"
                    + objEquipmentModels.Equipment_Name + "', Equipment_Application='" + objEquipmentModels.Equipment_Application
                    + "', Source_of_calibration='" + objEquipmentModels.Source_of_calibration + "', Freq_of_calibration='" + objEquipmentModels.Freq_of_calibration
                    + "', Commissioning_Date='" + sCommissioning_Date + "', Manufacturer='" + objEquipmentModels.Manufacturer
                    + "', Equipment_status='" + objEquipmentModels.Equipment_status + "', Model_No='" + objEquipmentModels.Model_No
                    + "', Department='" + objEquipmentModels.Department + "', RespPerson='" + objEquipmentModels.RespPerson + "', equp_type='" + objEquipmentModels.equp_type
                    + "', location='" + objEquipmentModels.location + "', branch='" + objEquipmentModels.branch + "', Equipment_location='" + objEquipmentModels.Equipment_location + "', eqp_range='" + eqp_range + "'";

                if (objEquipmentModels.DocUploadPath != null && objEquipmentModels.DocUploadPath != "")
                {
                    sSqlstmt = sSqlstmt + ", DocUploadPath='" + objEquipmentModels.DocUploadPath + "'";
                }
                sSqlstmt = sSqlstmt + " where Equipment_Id='" + objEquipmentModels.Equipment_Id + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEquipment: " + ex.ToString());

            }
            return false;
        }

        internal string GetEquipmentNameById(string sEquipment_Id)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select Equipment_Name from t_equipment where Equipment_Id='" + sEquipment_Id + "'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Equipment_Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEquipmentNameById: " + ex.ToString());

            }
            return "";
        }

        public MultiSelectList GetEquipmentListbox()
        {
            EquipmentDetailsList Equipmentlist = new EquipmentDetailsList();
            Equipmentlist.EquipmentDetailsMList = new List<EquipmentDetails>();
            try
            {
                string statusId = objGlobalData.getActiveEquipmentID();
                DataSet dsEquip = objGlobalData.Getdetails("select Equipment_Id, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name"
                    + " from t_equipment where Equipment_status='" + statusId + "' and Active=1 order by Equipment_Name asc");
                if (dsEquip.Tables.Count > 0 && dsEquip.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEquip.Tables[0].Rows.Count; i++)
                    {
                        EquipmentDetails Equipments = new EquipmentDetails()
                        {
                            Equipment_Id = dsEquip.Tables[0].Rows[i]["Equipment_Id"].ToString(),
                            Equipment_Name = dsEquip.Tables[0].Rows[i]["Equipment_Name"].ToString()
                        };

                        Equipmentlist.EquipmentDetailsMList.Add(Equipments);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEquipmentListbox: " + ex.ToString());
            }
            return new MultiSelectList(Equipmentlist.EquipmentDetailsMList, "Equipment_Id", "Equipment_Name");
        }


        public MultiSelectList GetCalibatedEquipmentListbox()
        {
            EquipmentDetailsList Equipmentlist = new EquipmentDetailsList();
            Equipmentlist.EquipmentDetailsMList = new List<EquipmentDetails>();
            try
            {
                //EquipmentModels eqipmdl = new EquipmentModels();
                string statusId = objGlobalData.getActiveEquipmentID();
                string equp_type = GetCalibratedEquipmentID();
                DataSet dsEquip = objGlobalData.Getdetails("select Equipment_Id, concat(Equipment_name,' - ', Equipment_serial_no) as Equipment_Name"
                    + " from t_equipment where Equipment_status='" + statusId + "' and Active=1 and equp_type = '"+ equp_type +"' order by Equipment_Name asc");
                if (dsEquip.Tables.Count > 0 && dsEquip.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEquip.Tables[0].Rows.Count; i++)
                    {
                        EquipmentDetails Equipments = new EquipmentDetails()
                        {
                            Equipment_Id = dsEquip.Tables[0].Rows[i]["Equipment_Id"].ToString(),
                            Equipment_Name = dsEquip.Tables[0].Rows[i]["Equipment_Name"].ToString()
                        };

                        Equipmentlist.EquipmentDetailsMList.Add(Equipments);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetCalibatedEquipmentListbox: " + ex.ToString());
            }
            return new MultiSelectList(Equipmentlist.EquipmentDetailsMList, "Equipment_Id", "Equipment_Name");
        }


        public string GetCalibratedEquipmentID()
        {
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                   + "and header_desc='Equipment Type' and item_desc='Calibrated Equipment'";
                DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Id"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetCalibratedEquipmentID: " + ex.ToString());
            }

            return "";
        }

        public bool checkSerialNoExists(string serial_no)
        {
            try
            {
                string sSqlstmt = "select Equipment_Id from t_equipment where Equipment_serial_no='" + serial_no + "' and active=1";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkSerialNoExists: " + ex.ToString());
            }
            return true;
        }

        internal string GetCalibrationSourceNameById(string sSource)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Calibration Source' and (item_id='" + sSource + "' or item_desc='" + sSource + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetCalibrationSourceNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiCalibrationSourceList()
        {
            DropdownEquipmentList lst = new DropdownEquipmentList();
            lst.lst = new List<DropdownEquipmentItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Calibration Source' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownEquipmentItems reg = new DropdownEquipmentItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetMultiCalibrationSourceList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        internal string GetCalibrationFrequencyNameById(string sFreq)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Calibration Frequency' and (item_id='" + sFreq + "' or item_desc='" + sFreq + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetCalibrationFrequencyNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiCalibrationFrequencyList()
        {
            DropdownEquipmentList lst = new DropdownEquipmentList();
            lst.lst = new List<DropdownEquipmentItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Calibration Frequency' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownEquipmentItems reg = new DropdownEquipmentItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetMultiCalibrationFrequencyList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        internal string GetCalibrationStatusNameById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Calibration Status' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetCalibrationStatusNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiCalibrationStatusList()
        {
            DropdownEquipmentList lst = new DropdownEquipmentList();
            lst.lst = new List<DropdownEquipmentItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Calibration Status' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownEquipmentItems reg = new DropdownEquipmentItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetMultiCalibrationStatusList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        
    }

    public class EquipmentModelsList
    {
        public List<EquipmentModels> EquipmentMList { get; set; }
    }


    public class EquipmentMaintanance
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Maintenance Id")]
        public string Maintenance_Id { get; set; }

        [Required]
        [Display(Name = "Equipment Id")]
        public string Equipment_Id { get; set; }

        public string Equipment_Id1 { get; set; }

        [Required]
        [Display(Name = "Maintenance Date")]
        public DateTime Maintenance_Date { get; set; }

        [Required]
        [Display(Name = "Maintenance Details")]
        public string Maintenance_Details { get; set; }

        [Required]
        [Display(Name = "Details of Rectification Work")]
        [DataType(DataType.MultilineText)]
        public string Maintenance_RectificationWork { get; set; }

        [Required]
        [Display(Name = "Down Time")]
        public DateTime Down_Time_From { get; set; }

        [Required]
        [Display(Name = "Down Time")]
        public DateTime Down_Time_To { get; set; }

        //[Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Spare used")]
        public string Spare_Used { get; set; }

        //[Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        //t_equpiment_preventive_maint
        [Required]
        [Display(Name = "Peventive Maintenance Id")]
        public string Preventive_Id { get; set; }

        [Required]
        [Display(Name = "Next Maintenance Date")]
        public DateTime Next_Maint_Date { get; set; }

        [Required]
        [Display(Name = "Maintenance Done By")]
        public string done_by { get; set; }

        [Display(Name = "Amount")]
        public Decimal Amt_Spent { get; set; }

        [Display(Name = "Email Notification Period")]
        public string NotificationPeriod { get; set; }

        [Display(Name = "Notification Value")]
        public string NotificationValue { get; set; }

        [Display(Name = "Notification Days")]
        public int NotificationDays { get; set; }

        [Display(Name = "Voucher")]
        public string voucher { get; set; }

        public string id_spare { get; set; }

        internal bool FunAddMaintanance(EquipmentMaintanance objEquipmentMaintanance, EquipmentMaintananceList objList)
        {
            try
            {
                string sMaintanance_Date = objEquipmentMaintanance.Maintenance_Date.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sDown_Time_From = objEquipmentMaintanance.Down_Time_From.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sDown_Time_To = objEquipmentMaintanance.Down_Time_To.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into t_equipment_maintenance (Equipment_Id, Maintenance_Date, Maintenance_Details, Maintenance_RectificationWork,"
                    + " Down_Time_From, Down_Time_To, Spare_Used, Remarks, Amt_Spent)"
                + " values('" + objEquipmentMaintanance.Equipment_Id + "','" + sMaintanance_Date + "','" + objEquipmentMaintanance.Maintenance_Details
                + "','" + objEquipmentMaintanance.Maintenance_RectificationWork + "','" + sDown_Time_From + "','" + sDown_Time_To
                + "','" + objEquipmentMaintanance.Spare_Used + "','" + objEquipmentMaintanance.Remarks + "','" + objEquipmentMaintanance.Amt_Spent + "')";
                //Down_Time_To, Down_Time_From
               
                int Maintenance_Id = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Maintenance_Id))
                {
                    if (Maintenance_Id > 0)
                    {

                        if (Convert.ToInt32(objList.lstEquipmentMaintanance.Count) > 0)
                        {
                            objList.lstEquipmentMaintanance[0].Maintenance_Id = Maintenance_Id.ToString();
                            return FunAddSpareList(objList);
                        }
                    }
                }
                       

                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddMaintanance: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddSpareList(EquipmentMaintananceList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_equipment_spare where Maintenance_Id='" + objList.lstEquipmentMaintanance[0].Maintenance_Id + "'; ";

                for (int i = 0; i < objList.lstEquipmentMaintanance.Count; i++)
                {

                    sSqlstmt = sSqlstmt + "insert into t_equipment_spare(Maintenance_Id,Spare_Used,Amt_Spent,voucher";

                    string sFieldValue = "", sFields = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objList.lstEquipmentMaintanance[0].Maintenance_Id + "','" + objList.lstEquipmentMaintanance[i].Spare_Used + "','" + objList.lstEquipmentMaintanance[i].Amt_Spent + "','" + objList.lstEquipmentMaintanance[i].voucher + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSpareList: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateMaintanance(EquipmentMaintanance objEquipmentMaintanance, EquipmentMaintananceList objList)
        {
            try
            {
                string sMaintanance_Date = objEquipmentMaintanance.Maintenance_Date.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sDown_Time_From = objEquipmentMaintanance.Down_Time_From.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sDown_Time_To = objEquipmentMaintanance.Down_Time_To.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_equipment_maintenance set Maintenance_Date='" + sMaintanance_Date + "', Maintenance_RectificationWork='"
                    + objEquipmentMaintanance.Maintenance_RectificationWork + "', Down_Time_From='" + sDown_Time_From
                    + "', Down_Time_To='" + sDown_Time_To + "', Spare_Used='" + objEquipmentMaintanance.Spare_Used + "', Amt_Spent='" + objEquipmentMaintanance.Amt_Spent + "', Remarks='"
                    + objEquipmentMaintanance.Remarks + "'";

                if (objEquipmentMaintanance.Maintenance_Details != null)
                {
                    sSqlstmt = sSqlstmt + ", Maintenance_Details='" + objEquipmentMaintanance.Maintenance_Details + "'";
                }
                sSqlstmt = sSqlstmt + " where Maintenance_Id='" + objEquipmentMaintanance.Maintenance_Id + "'";

                objGlobalData.ExecuteQuery(sSqlstmt);

                if (Convert.ToInt32(objList.lstEquipmentMaintanance.Count) > 0)
                {
                    objList.lstEquipmentMaintanance[0].Maintenance_Id = Maintenance_Id.ToString();
                    return FunAddSpareList(objList);
                }
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateMaintanance: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddPreventiveMaint(EquipmentMaintanance objEquipmentMaintanance)
        {
            try
            {
                string sMaintanance_Date = objEquipmentMaintanance.Maintenance_Date.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sNext_Maint_Date = objEquipmentMaintanance.Next_Maint_Date.ToString("yyyy-MM-dd HH':'mm':'ss");
              
                string sSqlstmt = "insert into t_equpiment_preventive_maint (Equipment_Id, Maintenance_Date, Maintenance_Details, Next_Maint_Date, Remarks, done_by,Amt_Spent,NotificationDays,NotificationPeriod,NotificationValue)"
                 + " values('" + objEquipmentMaintanance.Equipment_Id + "','" + sMaintanance_Date + "','" + objEquipmentMaintanance.Maintenance_Details
               + "','" + sNext_Maint_Date + "','" + objEquipmentMaintanance.Remarks + "','" + objEquipmentMaintanance.done_by + "','" + objEquipmentMaintanance.Amt_Spent
               + "','" + objEquipmentMaintanance.NotificationDays + "','" + objEquipmentMaintanance.NotificationPeriod + "','" + objEquipmentMaintanance.NotificationValue + "')";
               
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPreventiveMaint: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdatePreventiveMaint(EquipmentMaintanance objEquipmentMaintanance)
        {
            try
            {
                string sMaintanance_Date = objEquipmentMaintanance.Maintenance_Date.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sNext_Maint_Date = objEquipmentMaintanance.Next_Maint_Date.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_equpiment_preventive_maint set Maintenance_Date='" + sMaintanance_Date + "', Next_Maint_Date='" + sNext_Maint_Date + "', done_by='"
                    + objEquipmentMaintanance.done_by + "', Remarks='"+ objEquipmentMaintanance.Remarks + "', Amt_Spent= '"+objEquipmentMaintanance.Amt_Spent
                    + "', NotificationPeriod= '" + objEquipmentMaintanance.NotificationPeriod + "', NotificationDays= '" + objEquipmentMaintanance.NotificationDays + "', NotificationValue= '" + objEquipmentMaintanance.NotificationValue + "'";

                if (objEquipmentMaintanance.Maintenance_Details != null)
                {
                    sSqlstmt = sSqlstmt + ", Maintenance_Details='" + objEquipmentMaintanance.Maintenance_Details + "'";
                }
                sSqlstmt = sSqlstmt + " where Preventive_Id='" + objEquipmentMaintanance.Preventive_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePreventiveMaint: " + ex.ToString());
            }
            return false;
        }
    }

    public class EquipmentDetails
    {
        public string Equipment_Id { get; set; }
        public string Equipment_Name { get; set; }
    }
    public class EquipmentDetailsList
    {
        public List<EquipmentDetails> EquipmentDetailsMList { get; set; }
    }

    public class EquipmentMaintananceList
    {
        public List<EquipmentMaintanance> lstEquipmentMaintanance { get; set; }
    }

    //Class model for Calibration
    public class CalibrationModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Equipment Name")]
        public string Equipment_Id { get; set; }

         [Display(Name = "Calibration Id")]
        public string calibration_id { get; set; }

        [Required]
        [Display(Name = "Calibrated by")]
         public string calibration_by { get; set; }

        [Required]
        [Display(Name = "Method of Calibration")]
        public string method_of_calibration { get; set; }

        [Required]
        [Display(Name = "Accuracy")]
        public string accuracy { get; set; }

        [Required]
        [Display(Name = "Calibration status")]
        public string calibration_status { get; set; }

        [Required]
        [Display(Name = "Report Ref.")]
        [System.Web.Mvc.Remote("doesFileTypeValidation", "Equipment", HttpMethod = "POST", ErrorMessage = "File format Not supported. Please enter a different ID.")]
        public string calibration_report_ref { get; set; }


        [Required]
        [Display(Name = "Calibration Certificate")]
        public string calibration_certificate { get; set; }

        [Required]
        [Display(Name = "Due Date")]
        public DateTime due_date { get; set; }

        //[Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Required]
        [Display(Name = "Calibration Date")]
        public DateTime CalibrationDate { get; set; }

        [Required]
        [Display(Name = "Email Notification Period")]
        public string NotificationPeriod { get; set; }

        [Required]
        [Display(Name = "Notification Value")]
        public string NotificationValue { get; set; }

        [Required]
        [Display(Name = "Notification Days")]
        public int NotificationDays { get; set; }

        [Required]
        [Display(Name = "Notified To")]
        public string Person_Responsible { get; set; }

        [Display(Name = "Reference No")]
        public string Ref_no { get; set; }

        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        public string Location { get; set; }

        [Display(Name = "Certificate No")]
        public string certificate_no { get; set; }

        

        [Display(Name = "Calibration Frequency")]
        public string Freq_of_calibration { get; set; }


        internal bool FunAddCalibration(CalibrationModels objCalibrationModels)
        {
            try
            {
                string sdue_date = objCalibrationModels.due_date.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sCalibrationDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_calibration (Equipment_id, calibration_by, method_of_calibration, accuracy, calibration_status, "
                    + "calibration_report_ref, calibration_certificate, due_date, Remarks, CalibrationDate, NotificationPeriod, NotificationValue, Person_Responsible,"
                    + " NotificationDays,Ref_no,branch, certificate_no";

                sSqlstmt = sSqlstmt + ") values('" + objCalibrationModels.Equipment_Id + "','" + objCalibrationModels.calibration_by + "','" + objCalibrationModels.method_of_calibration
                + "','" + objCalibrationModels.accuracy + "','" + objCalibrationModels.calibration_status + "','" + objCalibrationModels.calibration_report_ref + "','"
                + objCalibrationModels.calibration_certificate + "','" + sdue_date + "','" + objCalibrationModels.Remarks + "','" + sCalibrationDate
                + "','" + objCalibrationModels.NotificationPeriod + "','" + objCalibrationModels.NotificationValue + "','" + objCalibrationModels.Person_Responsible
                + "', '" + objCalibrationModels.NotificationDays + "','" + objCalibrationModels.Ref_no + "','" + sBranch + "','" + certificate_no + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCalibration: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateCalibration(CalibrationModels objCalibrationModels)
        {
            try
            {
                string sdue_date = objCalibrationModels.due_date.ToString("yyyy-MM-dd HH':'mm':'ss");
                //string sCalibrationDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_calibration set Equipment_id ='" + objCalibrationModels.Equipment_Id + "', calibration_by='"
                    + objCalibrationModels.calibration_by + "', method_of_calibration='" + objCalibrationModels.method_of_calibration
                    + "', accuracy='" + objCalibrationModels.accuracy + "', calibration_status='" + objCalibrationModels.calibration_status
                    + "', due_date='" + sdue_date + "', Remarks='" + objCalibrationModels.Remarks
                    + "', NotificationPeriod='" + objCalibrationModels.NotificationPeriod + "', NotificationValue='" + objCalibrationModels.NotificationValue
                    + "', Person_Responsible='" + objCalibrationModels.Person_Responsible + "', NotificationDays='" + objCalibrationModels.NotificationDays
                    + "',Ref_no='" + objCalibrationModels.Ref_no + "',certificate_no='" + objCalibrationModels.certificate_no + "',calibration_report_ref='" + objCalibrationModels.calibration_report_ref + "',calibration_certificate='" + objCalibrationModels.calibration_certificate + "'";

                //if (objCalibrationModels.calibration_report_ref != null && objCalibrationModels.calibration_report_ref != "")
                //{
                //    sSqlstmt = sSqlstmt + ", calibration_report_ref='" + objCalibrationModels.calibration_report_ref + "'";
                //}
                //if (objCalibrationModels.calibration_certificate != null && objCalibrationModels.calibration_certificate != "")
                //{
                //    sSqlstmt = sSqlstmt + ", calibration_certificate='" + objCalibrationModels.calibration_certificate + "'";
                //}
                if (objCalibrationModels.CalibrationDate != null && objCalibrationModels.CalibrationDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", CalibrationDate='" + objCalibrationModels.CalibrationDate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where calibration_id='" + objCalibrationModels.calibration_id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCalibration: " + ex.ToString());
            }
            return false;
        }
    }

    public class CalibrationModelsList
    {
        public List<CalibrationModels> CalibrationMList { get; set; }
    }

    public class DropdownEquipmentItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownEquipmentList
    {
        public List<DropdownEquipmentItems> lst { get; set; }
    }
}