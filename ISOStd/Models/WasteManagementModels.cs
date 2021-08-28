using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;


namespace ISOStd.Models
{
    public class WasteManagementModels
    {
        clsGlobal objGlobaldata = new clsGlobal();

        [Display(Name = "Id")]
        public string Id_waste { get; set; }
        
        [Required]
        [Display(Name = "Waste Type")]
        public string Wate_type { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
               
        [Required]
        [Display(Name = "Units")]
        public string Units { get; set; }
               
        [Required]
        [Display(Name = "Collection Method")]
        public string Collection_Method { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Destination")]
        public string Destination { get; set; }

        [Required]
        [Display(Name = "Recv facility")]
        public string Recv_facility { get; set; }

        [Required]
        [Display(Name = "Waste Collected From")]
        public string Collected_by { get; set; }

        [Required]
        [Display(Name = "Upload doc")]
        public string Upload_doc { get; set; }
       
        [Required]
        [Display(Name = "Remarks")]
        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Required]
        [Display(Name = "Disposal Date")]
        public DateTime Disposal_date { get; set; }

        [Display(Name = "Disposal By")]
        public string DisposalBy { get; set; }

        internal bool FunDeleteWaste(string Id_waste)
        {
            try
            {
                string sSqlstmt = "update t_waste_management set Active=0 where Id_waste='" + Id_waste + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteWaste: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddWaste(WasteManagementModels objComp)
        {
            try
            {
                string sSqlstmt = "insert into t_waste_management (Wate_type,Quantity,Units,Collection_Method,Location,Destination,Recv_facility,Collected_by,Upload_doc,Remarks,branch,DisposalBy";
                string sBranch = objGlobaldata.GetCurrentUserSession().division;
                string sFields = "", sFieldValue = "";

                if (objComp.Disposal_date != null && objComp.Disposal_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Disposal_date";
                    sFieldValue = sFieldValue + ", '" + objComp.Disposal_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ")values('" + objComp.Wate_type + "','" + objComp.Quantity + "','" + objComp.Units + "'"
                    + ",'" + objComp.Collection_Method + "','" + objComp.Location + "','" + objComp.Destination + "','" + objComp.Recv_facility 
                    + "','" + objComp.Collected_by + "','" + objComp.Upload_doc + "','" + objComp.Remarks + "','" + sBranch + "','" + objComp.DisposalBy + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";

                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddWaste: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateWaste(WasteManagementModels objComp)
        {
            try
            {
                string sSqlstmt = "update t_Waste_Management set Wate_type ='" + objComp.Wate_type + "', Quantity='" + objComp.Quantity + "', "
                    + "Units='" + objComp.Units + "',Collection_Method='" + objComp.Collection_Method + "',Location='" + objComp.Location
                    + "',Destination='" + objComp.Destination + "',Recv_facility='" + objComp.Recv_facility + "',Collected_by='" + objComp.Collected_by 
                    + "',Upload_doc='" + objComp.Upload_doc + "',Remarks='" + objComp.Remarks + "',DisposalBy='" + objComp.DisposalBy + "'";

                if (objComp.Disposal_date != null && objComp.Disposal_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Disposal_date='" + objComp.Disposal_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where Id_waste='" + objComp.Id_waste + "'";

                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateWaste: " + ex.ToString());
            }

            return false;
        }


    }
    public class WasteManagementModelsList
    {
        public List<WasteManagementModels> WasteManagementList { get; set; }
    }
}