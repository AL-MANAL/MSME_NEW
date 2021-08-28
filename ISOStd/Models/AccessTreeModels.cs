using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISOStd.Models
{
    public class AccessTreeModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id { get; set; }

        [Display(Name = "Id")]
        public string Id_access { get; set; }

        [Display(Name = "Parent")]
        public string parent_level { get; set; }

        [Display(Name = "Child")]
        public string child_level { get; set; }

        [Display(Name = "Branch")]
        public string BranchName { get; set; }        

        [Display(Name = "Branchtree")]
        public string BranchTree { get; set; }

        public string Levels { get; set; }

        internal bool FunUpdateAccessTree(string selected, string Id, string role_id)
        {
            try
            {
                if (selected != "")
                {
                    selected = selected.Substring(0, (selected.Length - 1));
                } 
                    string sqlstmt = "update t_access set BranchTree ='" + selected + "' where Id_access='" + Id + "'";
                    objGlobalData.ExecuteQuery(sqlstmt);
                    //}

                    return objGlobalData.UpdateBranchdetails(role_id);
                   
                //}
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAccessPrivileges: " + ex.ToString());
            }
            return false;
        }
    }

    public class AccessTreeModelsList
    {
        public List<AccessTreeModels> TreeList { get; set; }
    }

}