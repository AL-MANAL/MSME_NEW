using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
namespace ISOStd.Models
{
    public class HseIndModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        public string id_Hse_insp { get; set; }

        [Display(Name = "Date and Timing")]
        public DateTime Hse_DateTime { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Employee")]
        public string Employee { get; set; }

        [Display(Name = "Department")]
        public string Dept { get; set; }
        
        [Display(Name = "General Behavior")]
        public string General_Behavior { get; set; }

        [Display(Name = "Site Details & Responsibilities")]
        public string Site_Details_Responsibilities { get; set; }

        [Display(Name = "Personal Protective Equipments & Compliance")]
        public string Personal_Protective_Equipments_Compliance { get; set; }

        [Display(Name = "First Aid")]
        public string First_Aid { get; set; }

        [Display(Name = "Emergency Assistance")]
        public string Emergency_Assistance { get; set; }

        [Display(Name = "IMS - requirements for compliance")]
        public string IMS { get; set; }

        [Display(Name = "Equipments & Procedures")]
        public string Equipments_Procedures { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Induction given by")]
        public string Induction_given_by { get; set; }

        [Display(Name = "Project")]
        public string Project { get; set; }

        [Display(Name = "Report No")]
        public string ReportNo { get; set; }

        [Display(Name = "Visitors")]
        public string Visitors { get; set; }

        [Required]
        [Display(Name = "Induction for Employees")]
        public string EmpType { get; set; }

        [Display(Name = "Others")]
        public string Others { get; set; }

        internal bool FunDeleteHseInd(string sid_Hse_insp)
        {
            try
            {
                string sSqlstmt = "update t_hse_ind set Active=0 where id_Hse_insp='" + sid_Hse_insp + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteHseIndDoc: " + ex.ToString());
            }
            return false;
        }

        //internal string GetDeptById(string sSourceID)
        //{
        //    try
        //    {
        //        //DataSet dsEmp = objGlobaldata.Getdetails("select impact_id, impact_name from impact where impact_id='" + sImpact_id + "'");
        //        DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
        //             + "and header_desc='HseInd-Dept' and (item_id='" + sSourceID + "' or item_desc='" + sSourceID + "')");
        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in GetDailyHseInd-DeptById: " + ex.ToString());
        //    }
        //    return "";
        //}

        //public MultiSelectList GetMultiDept()
        //{
        //    DropdownDailyInspectionList lst = new DropdownDailyInspectionList();
        //    lst.lst = new List<DropdownTDailyinspectionItems>();
        //    try
        //    {
        //        //string sSqlstmt = "select impact_id, impact_name from impact";
        //        string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
        //            + "and header_desc='HseInd-Dept' order by item_desc asc";
        //        DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
        //            {
        //                DropdownTDailyinspectionItems reg = new DropdownTDailyinspectionItems()
        //                {
        //                    Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
        //                    Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
        //                };

        //                lst.lst.Add(reg);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in GetMultiHseInd-DeptList: " + ex.ToString());
        //    }

        //    return new MultiSelectList(lst.lst, "Id", "Name");
        //}

        internal string GetHseIndById(string sSourceID)
        {
            try
            {
                //DataSet dsEmp = objGlobaldata.Getdetails("select impact_id, impact_name from impact where impact_id='" + sImpact_id + "'");
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                     + "and header_desc='Hse Induction' and (item_id='" + sSourceID + "' or item_desc='" + sSourceID + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetHseIndById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiHseInd()
        {
            DropdownList lst = new DropdownList();
            lst.lstDropdown = new List<DropdownItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Hse Induction' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems reg = new DropdownItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lst.lstDropdown.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiHseInd: " + ex.ToString());
            }

            return new MultiSelectList(lst.lstDropdown, "Id", "Name");
        }

        internal bool FunAddHseInd(HseIndModels objHseInd)
        {
            try
            {
                string sHse_DateTime = objHseInd.Hse_DateTime.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sSqlstmt = "";

                //foreach (string AgendaId in lstAgendaId)
                {
                    sSqlstmt = sSqlstmt + "insert into t_hse_ind(Hse_DateTime, Location, Employee, Dept, General_Behavior, Site_Details_Responsibilities, " +
                        "Personal_Protective_Equipments_Compliance,First_Aid,Emergency_Assistance,IMS,Equipments_Procedures,Remarks,Induction_given_by,Project,ReportNo,Visitors,EmpType,branch,Others"
                        + ")"
                                + " values('" + sHse_DateTime + "', '" + objHseInd.Location
                                + "','" + objHseInd.Employee + "','" + objHseInd.Dept + "','"+ objHseInd.General_Behavior
                                + "','" + objHseInd.Site_Details_Responsibilities + "','" + objHseInd.Personal_Protective_Equipments_Compliance + "','" + objHseInd.First_Aid + "','"
                                + objHseInd.Emergency_Assistance + "','" + objHseInd.IMS + "','" + objHseInd.Equipments_Procedures + "','" + objHseInd.Remarks + "','" + objHseInd.Induction_given_by + 
                                "','" + objHseInd.Project + "','" + objHseInd.ReportNo + "','" + objHseInd.Visitors + "','" + objHseInd.EmpType + "','" + sBranch + "','" + objHseInd.Others + "')";
                }

                int id_Hse_insp = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_Hse_insp))
                {
                    if (id_Hse_insp > 0)
                    {
                        string LocationName = objGlobalData.GetCompanyBranchNameById(sBranch);
                        DataSet dsData = objGlobalData.GetReportNo("HID", "", LocationName);
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            ReportNo = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        string sql = "update t_hse_ind set ReportNo='" + ReportNo + "' where id_Hse_insp = '" + id_Hse_insp + "'";

                        return objGlobalData.ExecuteQuery(sql);
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddHseInd: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateHseInd(HseIndModels objHseInd)
        {
            try
            {
                string sSqlstmt = " update t_hse_ind set Employee='" + objHseInd.Employee + "',Dept='" + objHseInd.Dept 
                    + "', General_Behavior='" + objHseInd.General_Behavior + "', Site_Details_Responsibilities='" + objHseInd.Site_Details_Responsibilities + "',Personal_Protective_Equipments_Compliance='" + objHseInd.Personal_Protective_Equipments_Compliance + "',First_Aid='" + objHseInd.First_Aid + "',Emergency_Assistance='" + objHseInd.Emergency_Assistance
                    + "',IMS='" + objHseInd.IMS + "',Remarks='" + objHseInd.Remarks + "',Induction_given_by='" + objHseInd.Induction_given_by + "',Project='" + objHseInd.Project + "',Location='" + objHseInd.Location /*+ "',ReportNo='" + objHseInd.ReportNo*/ + "',EmpType='" + objHseInd.EmpType + "',Visitors='" + objHseInd.Visitors + "',Others='" + objHseInd.Others;

                if (objHseInd.Hse_DateTime > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + "', Hse_DateTime='" + objHseInd.Hse_DateTime.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where id_Hse_insp=" + objHseInd.id_Hse_insp;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateHseInd: " + ex.ToString());
            }
            return false;
        }
    }

    public class HseIndModelsList
    {
        public List<HseIndModels> HseIndList { get; set; }
    }

    public class DropdownHseIndItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownHseIndList
    {
        public List<DropdownHseIndItems> lst { get; set; }
    }
}