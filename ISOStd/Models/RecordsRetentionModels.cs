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
    public class RecordsRetentionModels
    {
        clsGlobal objGlobalData = new clsGlobal();
             
        [Display(Name = "Id")]
        public string Records_Id { get; set; }
         
        [Display(Name = "Record Type")]
        public string Records_Type { get; set; }
               
        [Display(Name = "Location")]
        public string Work_Location { get; set; }
        
        [Display(Name = "Department")]
        public string Dept_id { get; set; }
               
        [Display(Name = "Document Name")]
        public string Doc_name { get; set; }
        
        [Required]
        [Display(Name = "Record Name")]
        public string Record_Title { get; set; }
           
        [Display(Name = "Generated On")]
        public DateTime Generated_On { get; set; }
          
        [Display(Name = "Upload")]
        public string Upload_Path { get; set; }
               
        [Display(Name = "Retention Period")]
        public string Retention_Period { get; set; }
        
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }
             
        [Display(Name = "Logged Date")]
        public DateTime LoggedDate { get; set; }
              
        [Display(Name = "Record Date")]
        public DateTime RecordDate { get; set; }

        [Display(Name = "Record Related To")]
        public string Jobno { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Criteria for Retention")]
        public string retention_criteria { get; set; }

        internal bool FunDeleteRecordDoc(string sRecords_Id)
        {
            try
            {
                string sSqlstmt = "update t_records_retention_period set Active=0 where Records_Id='" + sRecords_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteRecordDoc: " + ex.ToString());
            }

            return false;
        }
        internal bool FunAddRecords(RecordsRetentionModels objRecordsRetentionModels)
        {
            try
            {
                //string RecRetnt = objGlobalData.GetRecordRetentionByID(objRecordsRetentionModels.Retention_Period);
                string sColumn = "", sValues = "";
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
               // string sBranch = objGlobalData.GetCurrentUserSession().Work_Location;

                string sSqlstmt = "insert into t_records_retention_period (Records_Type, Work_Location, Dept_id, Doc_name, Record_Title, Retention_Period, LoggedBy, LoggedDate,Jobno,branch,retention_criteria";

                if (objRecordsRetentionModels.Generated_On > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Generated_On";
                    sValues = sValues + ", '" + objRecordsRetentionModels.Generated_On.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objRecordsRetentionModels.Upload_Path != null && objRecordsRetentionModels.Upload_Path != "")
                {
                    sColumn = sColumn + ", Upload_Path";
                    sValues = sValues + ", '" + objRecordsRetentionModels.Upload_Path + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objGlobalData.GetDropdownitemById(objRecordsRetentionModels.Records_Type) + "','" + objRecordsRetentionModels.Work_Location
                    + "','" + objRecordsRetentionModels.Dept_id + "','" + objRecordsRetentionModels.Doc_name
                        + "','" + objRecordsRetentionModels.Record_Title + "','" + objRecordsRetentionModels.Retention_Period
                        + "','" + user + "','" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "','" + objRecordsRetentionModels.Jobno + "','" + objRecordsRetentionModels.branch + "','" + objRecordsRetentionModels.retention_criteria + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddRecords: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateRecords(RecordsRetentionModels objRecordsRetentionModels)
        {
            try
            {

                //string RecRetnt = objGlobalData.GetRecordRetentionByID(objRecordsRetentionModels.Retention_Period);
                string sSqlstmt = "update t_records_retention_period set Records_Type='" + objGlobalData.GetDropdownitemById(objRecordsRetentionModels.Records_Type)
                    + "', Work_Location='" + objRecordsRetentionModels.Work_Location + "', Dept_id='" + objRecordsRetentionModels.Dept_id
                    + "', Doc_name='" + objRecordsRetentionModels.Doc_name + "', Retention_Period='" + objRecordsRetentionModels.Retention_Period
                    + "', Record_Title='" + objRecordsRetentionModels.Record_Title + "', Jobno='" + objRecordsRetentionModels.Jobno + "', branch='" + objRecordsRetentionModels.branch + "', retention_criteria='" + objRecordsRetentionModels.retention_criteria + "'";

                if (objRecordsRetentionModels.Generated_On > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Generated_On='" + objRecordsRetentionModels.Generated_On.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objRecordsRetentionModels.Upload_Path != null && objRecordsRetentionModels.Upload_Path != "")
                {
                    sSqlstmt = sSqlstmt + ", Upload_Path='" + objRecordsRetentionModels.Upload_Path + "' ";
                }

                sSqlstmt = sSqlstmt + " where Records_Id='" + objRecordsRetentionModels.Records_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateRecords: " + ex.ToString());
            }
            return false;
        }

        //internal string GetRecordTypeNameById(string sRecordType)
        //{
        //    try
        //    {
        //        DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
        //              + "and header_desc='Record Type' and (item_id='" + sRecordType + "' or item_desc='" + sRecordType + "')");
        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in GetRecordTypeNameById: " + ex.ToString());
        //    }
        //    return "";
        //}

        //public MultiSelectList GetMultiRecordTypeList()
        //{
        //    DropdownRecordList lst = new DropdownRecordList();
        //    lst.lst = new List<DropdownRecordItems>();
        //    try
        //    {
        //        string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
        //            + "and header_desc='Record Type' order by item_desc asc";
        //        DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
        //            {
        //                DropdownRecordItems reg = new DropdownRecordItems()
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
        //        objGlobalData.AddFunctionalLog("Exception in GetMultiRecordTypeList: " + ex.ToString());
        //    }

        //    return new MultiSelectList(lst.lst, "Id", "Name");
        //}
        public string GetMultiRetention_PeriodList(string idMgmt)
        {
            Retention_Period lstClause = new Retention_Period();
            //lstClause.lstRetention_PeriodList = new List<Retention_Period>();
            try
            {
                //string sSqlstmt = "select clause_id, clause_no, clause_desc from t_isoclause where stdid in(" + ISOStdId + ")";
                string sSqlstmt = "select item_id,item_fulldesc from dropdownitems where item_id= " + idMgmt;
                DataSet dsClause = objGlobalData.Getdetails(sSqlstmt);
                if (dsClause.Tables.Count > 0 && dsClause.Tables[0].Rows.Count > 0)
                {
                    //for (int i = 0; i < dsClause.Tables[0].Rows.Count; i++)
                    //{
                    //    ISOStdClause Clause = new ISOStdClause()
                    //    {
                    lstClause.clause_id = dsClause.Tables[0].Rows[0]["item_id"].ToString();
                    lstClause.clause = dsClause.Tables[0].Rows[0]["item_fulldesc"].ToString();
                        //};

                        //lstClause.lstISOStdClause.Add(Clause);
                    
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiISOClauseList: " + ex.ToString());
            }

            return lstClause.clause;
        }

    }

    public class RecordsRetentionModelsList
    {
        public List<RecordsRetentionModels> lstRecordsRetention { get; set; }
    }

    public class DropdownRecordItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownRecordList
    {
        public List<DropdownRecordItems> lst { get; set; }
    }

    public class Retention_Period
    {
        public string clause_id { get; set; }
        public string clause { get; set; }
    }


    
}