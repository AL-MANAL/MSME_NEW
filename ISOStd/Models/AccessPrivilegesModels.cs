using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISOStd.Models
{
    public class AccessPrivilegesModels
    {
        private clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "AuditNum")]
        public string id_AccessPrivileges { get; set; }

        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }

        [Display(Name = "Name")]
        public string Emp_Name { get; set; }

        [Display(Name = "IMS Documents")]
        public string DocMgmt { get; set; }

        [Display(Name = "Objectives and KPI")]
        public string ObjKPI { get; set; }

        [Display(Name = "Risk Mgmt")]
        public string Risk { get; set; }

        [Display(Name = "Human Resource")]
        public string HR { get; set; }

        [Display(Name = "Meeting Mgmt")]
        public string Meeting { get; set; }

        [Display(Name = "Supplier")]
        public string Supplier { get; set; }

        [Display(Name = "Audit Mgmt")]
        public string Audit { get; set; }

        [Display(Name = "Customer")]
        public string CustomerMgmt { get; set; }

        [Display(Name = "Equipment Mgmt")]
        public string Equipment { get; set; }

        [Display(Name = "Certificates")]
        public string Certificates { get; set; }

        [Display(Name = "HSE")]
        public string HSE { get; set; }

        [Display(Name = "Mgmt Reports")]
        public string MgmtReports { get; set; }

        [Display(Name = "Legal Register")]
        public string LegalRegister { get; set; }

        [Display(Name = "Leave Mgmt")]
        public string LeaveMgmt { get; set; }

        [Display(Name = "Induction Training")]
        public string TrainingOrientation { get; set; }

        [Display(Name = "KnowledgeBase")]
        public string KnowledgBase { get; set; }

        [Display(Name = "Leave")]
        public string LeaveApply { get; set; }

        internal bool FunUpdateAccessPrivileges(string selected, int status)
        {
            try
            {
                string[] Sselected = null;
                Sselected = selected.Split(',');
                if (status == 1)
                {
                    for (int j = 0; j <= Sselected.Length - 1; j = j + 2)
                    {
                        string sqlstmt = "update t_accessprivileges set " + Sselected[j + 1] + "=1 where id_AccessPrivileges='" + Sselected[j] + "'";
                        objGlobalData.ExecuteQuery(sqlstmt);
                    }
                    return true;
                }
                if (status == 0)
                {
                    for (int i = 0; i <= Sselected.Length - 1; i = i + 2)
                    {
                        string sqlstmt = "update t_accessprivileges set " + Sselected[i + 1] + "=0 where id_AccessPrivileges='" + Sselected[i] + "'";
                        objGlobalData.ExecuteQuery(sqlstmt);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAccessPrivileges: " + ex.ToString());
            }
            return false;
        }
    }

    public class AccessPrivilegesModelsList
    {
        public List<AccessPrivilegesModels> AccessPrivilegesList { get; set; }
    }
}