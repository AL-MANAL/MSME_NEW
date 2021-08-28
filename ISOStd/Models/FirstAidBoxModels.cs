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
    public class FirstAidBoxModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        public string id_FirstAidBox { get; set; }

        [Display(Name = "Date")]
        public DateTime FirstAid_DateTime { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "First Aid Box")]
        public string FirstAidBox_1 { get; set; }
        
        internal string GetFirstAidById(string sSourceID)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
             + " and header_desc='First-Aid-Box' and item_id in (" + sSourceID + ")");
                string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                if (sDesc != "" && sDesc.Contains(','))
                {
                    return sDesc.Replace(",", ", ");
                }
                return sDesc;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetFirstAidNameById: " + ex.ToString());
            }
            return "";
        }

        internal bool FunDeleteFirstAidBox(string sid_FirstAidBox)
        {
            try
            {
                string sSqlstmt = "update t_first_aid set Active=0 where id_FirstAidBox='" + sid_FirstAidBox + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteFirstAidBoxDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddFirstAid(FirstAidBoxModels objFirstAidModels)
        {
            try
            {
                string sFirstAid_DateTime = objFirstAidModels.FirstAid_DateTime.ToString("yyyy-MM-dd");

                string sSqlstmt = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                {
                    sSqlstmt = sSqlstmt + "insert into t_first_aid(FirstAid_DateTime, Location, FirstAidBox_1,branch"
                        + ")"
                                + " values('" + sFirstAid_DateTime + "', '" + objFirstAidModels.Location
                                + "','" + objFirstAidModels.FirstAidBox_1 + "','" + sBranch + "')";
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
                objGlobalData.AddFunctionalLog("Exception in FunAddFirstAidBox: " + ex.ToString());
            }
            return false;
        }

        public MultiSelectList GetFirstAid()
        {
            DropdownFirstAidBoxList lst = new DropdownFirstAidBoxList();
            lst.lst = new List<DropdownFirstAidBoxItems>();
            try
            {
                
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='First-Aid-Box' order by item_desc asc";

                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownFirstAidBoxItems reg = new DropdownFirstAidBoxItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetFirstAidBoxList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        internal bool FunUpdateFirstAid(FirstAidBoxModels objFirstAidBox)
        {
            try
            {
                string sSqlstmt = " update t_first_aid set Location='" + objFirstAidBox.Location + "', FirstAidBox_1='" + objFirstAidBox.FirstAidBox_1 ;

                               
                if (objFirstAidBox.FirstAid_DateTime > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + "', FirstAid_DateTime='" + objFirstAidBox.FirstAid_DateTime.ToString("yyyy-MM-dd") + "' ";
                }


                sSqlstmt = sSqlstmt + " where id_FirstAidBox=" + objFirstAidBox.id_FirstAidBox;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateFirstAid: " + ex.ToString());
            }
            return false;
        }

    }


    public class FirstAidBoxModelsList
    {
        public List<FirstAidBoxModels> FirstAidBoxList { get; set; }
    }

    public class DropdownFirstAidBoxItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownFirstAidBoxList
    {
        public List<DropdownFirstAidBoxItems> lst { get; set; }
    }

}
