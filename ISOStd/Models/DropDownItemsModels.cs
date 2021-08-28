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
    public class DropDownItemsModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string header_id { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string header_desc { get; set; }

        [Required]
        [Display(Name = "ID")]
        public string item_id { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string item_desc { get; set; }

        [Required]
        [Display(Name = "ID")]
        public string item_fulldesc { get; set; }


        internal bool FunAddItems(DropDownItemsModels objDropDownItemsModels)
        {
            try
            {
                string sSqlstmt = "insert into dropdownitems ( header_id, item_desc, item_fulldesc) values('" 
                    + objDropDownItemsModels.header_id + "','" + objDropDownItemsModels.item_desc + "','" + objDropDownItemsModels.item_fulldesc+ "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddItems: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateItems(DropDownItemsModels objDropDownItemsModels)
        {
            try
            {
                string sSqlstmt = "update dropdownitems set item_desc='" + objDropDownItemsModels.item_desc + "', item_fulldesc='" + objDropDownItemsModels.item_fulldesc
                    + "' where item_id='" + objDropDownItemsModels.item_id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateItems: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteItems(string item_id)
        {
            string sSqlstmt = "delete from dropdownitems where item_id='" + item_id + "'";

            return objGlobalData.ExecuteQuery(sSqlstmt);
        }

        public MultiSelectList GetDropDownItemsListbox(string sheader_id)
        {
            DropDownItemsList lstDropDownItemsList = new DropDownItemsList();
            lstDropDownItemsList.lstDropDownItems = new List<DropDownItems>();
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id, item_desc from dropdownitems where header_id='" + sheader_id + "' order by item_desc asc");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropDownItems items = new DropDownItems()
                        {
                            item_id = dsEmp.Tables[0].Rows[i]["item_id"].ToString(),
                            item_desc = dsEmp.Tables[0].Rows[i]["item_desc"].ToString()
                        };

                        lstDropDownItemsList.lstDropDownItems.Add(items);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDropDownItemsListbox: " + ex.ToString());
            }
            return new MultiSelectList(lstDropDownItemsList.lstDropDownItems, "item_id", "item_desc");
        }

        public MultiSelectList GetDropDownItemsListbox1(string sheader_id)
        {
            DropDownItemsList lstDropDownItemsList = new DropDownItemsList();
            lstDropDownItemsList.lstDropDownItems = new List<DropDownItems>();
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id, item_desc, item_fulldesc from dropdownitems where header_id='" + sheader_id + "' order by item_desc asc");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropDownItems items = new DropDownItems()
                        {
                            item_id = dsEmp.Tables[0].Rows[i]["item_id"].ToString(),
                            item_desc = dsEmp.Tables[0].Rows[i]["item_desc"].ToString()+"__"+dsEmp.Tables[0].Rows[i]["item_fulldesc"].ToString()
                        };

                        lstDropDownItemsList.lstDropDownItems.Add(items);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDropDownItemsListbox: " + ex.ToString());
            }
            return new MultiSelectList(lstDropDownItemsList.lstDropDownItems, "item_id", "item_desc");
        }

        public MultiSelectList GetDropDownHeaderListbox()
        {
            DropDownItemsList lstDropDownItemsList = new DropDownItemsList();
            lstDropDownItemsList.lstDropDownItems = new List<DropDownItems>();
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select header_id, header_desc from dropdownheader order by header_desc asc");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropDownItems items = new DropDownItems()
                        {
                            item_id = dsEmp.Tables[0].Rows[i]["header_id"].ToString(),
                            item_desc = dsEmp.Tables[0].Rows[i]["header_desc"].ToString()
                        };

                        lstDropDownItemsList.lstDropDownItems.Add(items);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDropDownHeaderListbox: " + ex.ToString());
            }
            return new MultiSelectList(lstDropDownItemsList.lstDropDownItems, "item_id", "item_desc");
        }

        public string GetdropdownitemsNameById(string item_id)
        {
            if (item_id != "")
            {
                DataSet dsItem = objGlobalData.Getdetails("select item_desc from dropdownitems where item_id='" + item_id + "' order by item_desc asc");
                if (dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
                {
                    return (dsItem.Tables[0].Rows[0]["item_desc"].ToString());
                }
            }
            return "";
        }

        public string GetdropdownHeaderNameById(string header_id)
        {
            if (header_id != "")
            {
                DataSet dsItem = objGlobalData.Getdetails("select header_desc from dropdownheader where header_id='" + header_id + "' order by header_desc asc");
                if (dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
                {
                    return (dsItem.Tables[0].Rows[0]["header_desc"].ToString());
                }
            }
            return "";
        }

        internal bool FunDeleteDropdownAllItem(string sheader_id)
        {
            try
            {
                string sSqlstmt = "delete from dropdownitems where header_id='" + sheader_id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDropdownAllItem: " + ex.ToString());
            }
            return false;
        }


    }

    public class DropDownItems
    {
        public string item_id { get; set; }
        public string item_desc { get; set; }
    }

    public class DropDownItemsList
    {
        public List<DropDownItems> lstDropDownItems { get; set; }
    }

    public class DropDownItems1
    {
        public string item_id { get; set; }
        public string item_desc { get; set; }
        public string item_fulldesc { get; set; }
    }

    public class DropDownItemsList1
    {
        public List<DropDownItems1> lstDropDownItems1 { get; set; }
    }
}