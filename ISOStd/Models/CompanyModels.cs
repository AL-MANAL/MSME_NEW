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
    public class CompanyModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string CompanyID { get; set; }

        [Required]
        [Editable(false)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Emp. ID")]
        public string EmpID { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        //[Required]
        [Display(Name = "State/Province")]
        public string State { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "P.O. Box No.")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Phone No.")]
        public string PhoneNumber { get; set; }

        //[Required]
        [Display(Name = "Fax No.")]
        public string FaxNumber { get; set; }

        //[Required]
        [Display(Name = "Branch office name")]
        public string Audit_location { get; set; }

        [Display(Name = "Branch office address")]
        public string BranchAddress { get; set; }  

        [Required]
        [Display(Name = "ISO Standards")]
        public string Audit_Criteria { get; set; }

        [Display(Name = "Scope")]
        public string Scope { get; set; }

        [Display(Name = "Exclusion")]
        public string Exclusion { get; set; }

        [Required]
        [Display(Name = "Logo")] 
        public string logo { get; set; }

      
        [Display(Name = "Company License")]
        public string License { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime expiry_date { get; set; }

        [Display(Name = "Email ID")]
        public string Email_Id { get; set; }

        // ISOSTD

        [Display(Name = "Audit Criteria Id")]
        public string StdId { get; set; }

        [Display(Name = "Audit Criteria Name")]
        public string IsoName { get; set; }

        [Display(Name = "Descriptions")]
        public string Descriptions { get; set; }   

        internal bool FunAddISOStd(CompanyModels objModel)
        {
            try
            {
                string sSqlstmt = "insert into t_isostandards (IsoName,Descriptions) values('" + objModel.IsoName + "', '" + objModel.Descriptions + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDept: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateISOStd(CompanyModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_isostandards set IsoName='" + objModel.IsoName + "', Descriptions = '"+ objModel.Descriptions + "' where StdId=" + StdId;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateISOStd: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteISOStd(string sStdId)
        {
            try
            {
                string sSqlstmt = "update t_isostandards set Active=0 where StdId=" + sStdId;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteISOStd: " + ex.ToString());
            }
            return false;
        }


      // Company Information

        internal bool FunRegisterCompany(CompanyModels objCompanyModels, Dictionary<string, string> dicBranch)
        {
            try
            {
                string sSqlstmt = "insert into t_CompanyInfo (CompanyName, Address, City, State, PostalCode, Country, PhoneNumber, FaxNumber, Audit_location,"
                            + " Audit_Criteria, BranchAddress,Email_Id";

                if (objCompanyModels.logo != null && objCompanyModels.logo != "")
                {
                    sSqlstmt = sSqlstmt + ", logo";
                }
                sSqlstmt = sSqlstmt + ") values('" + objCompanyModels.CompanyName + "','" + objCompanyModels.Address + "','" + objCompanyModels.City
                        + "','" + objCompanyModels.State + "','" + objCompanyModels.PostalCode + "','" + objCompanyModels.Country + "','" + objCompanyModels.PhoneNumber
                        + "','" + objCompanyModels.FaxNumber + "','" + objCompanyModels.Audit_location + "','" + objCompanyModels.Audit_Criteria
                        + "','" + objCompanyModels.BranchAddress + "','" + objCompanyModels.Email_Id + "'";


                if (objCompanyModels.logo != null && objCompanyModels.logo != "")
                {
                    sSqlstmt = sSqlstmt + ", '" + objCompanyModels.logo + "'";
                }
                sSqlstmt = sSqlstmt + ")";
                int iCompid = objGlobalData.ExecuteQueryReturnId(sSqlstmt);
                if (dicBranch.Count > 0)
                {
                   return FunAddBranch(dicBranch, iCompid);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunRegisterCompany: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateCompany(CompanyModels objCompanyModels)
        {
            try
            {
                string sSqlstmt = "update t_CompanyInfo set CompanyName='" + objCompanyModels.CompanyName + "', Address='" + objCompanyModels.Address + "', "
                    + "City='" + objCompanyModels.City + "', State='" + objCompanyModels.State + "', PostalCode='" + objCompanyModels.PostalCode
                    + "', Country='" + objCompanyModels.Country + "', PhoneNumber='" + objCompanyModels.PhoneNumber + "', FaxNumber='"
                    + objCompanyModels.FaxNumber + "', Audit_location='" + objCompanyModels.Audit_location + "', Audit_Criteria='" + objCompanyModels.Audit_Criteria
                    + "', BranchAddress='" + objCompanyModels.BranchAddress + "', Scope='" + objCompanyModels.Scope+ "', Exclusion='" + objCompanyModels.Exclusion + "',License='" + objCompanyModels .License + "',Email_Id='" + objCompanyModels.Email_Id + "'";

                if (objCompanyModels.logo != null && objCompanyModels.logo != "")
                {
                    sSqlstmt = sSqlstmt + ", logo='" + objCompanyModels.logo + "' ";
                }
                if (objCompanyModels.expiry_date != null && objCompanyModels.expiry_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",expiry_date='" + objCompanyModels.expiry_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where CompanyID=" + objCompanyModels.CompanyID;
               return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCompany: " + ex.ToString());
            }

            return false;
        }

        internal bool FunAddBranch(Dictionary<string, string> dicBranch, int sCompId)
        {
            try
            {
                string sSqlstmt = "";
                if (dicBranch.Count > 0)
                {
                    foreach (KeyValuePair<string, string> entry in dicBranch)
                    {
                        string[] sAddressNCurrency = entry.Value.Split('$');

                        if (sAddressNCurrency.Length > 1)
                        {
                            sSqlstmt = sSqlstmt + "insert into t_Company_Branch (CompId, BranchName, Address, scope,parent_level,BranchCode) values('"
                                + sCompId + "','" + entry.Key + "','" + sAddressNCurrency[0] + "','" + sAddressNCurrency[1] + "','" + sAddressNCurrency[2] + "','" + sAddressNCurrency[3] + "'); ";
                        }
                        else
                        {
                            sSqlstmt = sSqlstmt + "insert into t_Company_Branch (CompId, BranchName, Address) values('"
                                + sCompId + "','" + entry.Key + "','" + entry.Value + "'); ";
                        }
                    }
                    if (sSqlstmt != "")
                    {
                        int id_branch = 0;

                        if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_branch))
                        {
                            
                                foreach (KeyValuePair<string, string> entry in dicBranch)
                                {
                                    string[] sAddressNCurrency = entry.Value.Split('$');
                                    for (int i = 0; i < sAddressNCurrency.Length; i++)
                                    {
                                        string sSqlstmt1 = "insert into t_division_mapping (id_branch,id_country) values('" + id_branch + "','" + sAddressNCurrency[i] + "')";
                                        objGlobalData.ExecuteQuery(sSqlstmt1);
                                    }
                                }                            

                            DataSet ds = objGlobalData.BranchLevel(id_branch.ToString());
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddBranch: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateBranch(string sId, string sParent, string sName, string sCode, string sAddress,  string scope)
        {
            try
            {
                string sSqlstmt = "update t_Company_Branch set parent_level='" + sParent + "',BranchName='" + sName + "',BranchCode='" + sCode + "', Address='" + sAddress + "', scope='" + scope + "' where id=" + sId;

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    string sSql = "delete from t_division_mapping where id_branch ='" + sId + "'";
                    objGlobalData.ExecuteQuery(sSql);

                    string[] sCountry= sAddress.Split(',');
                    for(int i=0;i< sCountry.Length; i++)
                    {
                        string sSqlstmt1 = "insert into t_division_mapping (id_branch,id_country) values('" + sId + "','" + sCountry[i] + "')";
                        objGlobalData.ExecuteQuery(sSqlstmt1);
                    }                    

                    DataSet ds = objGlobalData.BranchLevel(sId);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateBranch: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteBranch(string sId)
        {
            try
            {
                string sSqlstmt = "update t_Company_Branch set active=0 where id=" + sId;

                if( objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    string sSql = "delete from t_division_mapping where id_branch ='" + sId + "'";
                    return objGlobalData.ExecuteQuery(sSql);
                }                
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteBranch: " + ex.ToString());
            }
            return false;
        }

        internal DataSet GetCompanyDetailsForReport(DataSet dsData)
        {
            DataSet dsCompany = new DataSet();
            try
            {
               dsCompany = objGlobalData.Getdetails("select * from t_CompanyInfo");

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    dsData.Tables[0].Columns.Add("Logo", System.Type.GetType("System.String"));
                    dsData.Tables[0].Columns.Add("CompanyName", System.Type.GetType("System.String"));
                    dsData.Tables[0].Columns.Add("City", System.Type.GetType("System.String"));
                    if (dsCompany.Tables[0].Rows[0]["Logo"].ToString() != "")
                    {
                        dsData.Tables[0].Rows[0]["Logo"] = dsCompany.Tables[0].Rows[0]["Logo"].ToString();
                    }
                    else
                    {
                        dsData.Tables[0].Rows[0]["Logo"] = Path.Combine(HttpContext.Current.Server.MapPath("~/Images"), "ManalLogo.png");
                    }

                    dsData.Tables[0].Rows[0]["CompanyName"] = dsCompany.Tables[0].Rows[0]["CompanyName"].ToString();
                    dsData.Tables[0].Rows[0]["City"] = dsCompany.Tables[0].Rows[0]["City"].ToString();
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteBranch: " + ex.ToString());
            }
            return dsData;
        }
        //internal DataSet GetCompanyDetailsReport(DataSet dsCompany)
        //{
        //    DataSet dsComp = objGlobalData.Getdetails("select * from t_CompanyInfo");

        //    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
        //    {
        //        dsComp.Tables[0].Columns.Add("logo", System.Type.GetType("System.String"));
        //        dsComp.Tables[0].Columns.Add("CompanyName", System.Type.GetType("System.String"));

        //        if (dsComp.Tables[0].Rows[0]["logo"].ToString() != "")
        //        {
        //            dsComp.Tables[0].Rows[0]["logo"] = dsComp.Tables[0].Rows[0]["logo"].ToString();
        //        }
        //        else
        //        {
        //            dsComp.Tables[0].Rows[0]["logo"] = Path.Combine(HttpContext.Current.Server.MapPath("~/Images"), "ManalLogo.png");
        //        }

        //        dsComp.Tables[0].Rows[0]["CompanyName"] = dsComp.Tables[0].Rows[0]["CompanyName"].ToString();
        //    }

        //    return dsComp;
        //}
    }

    public class CompanyModelsList
    {
        public List<CompanyModels> CompanyMList { get; set; }
    }

    public class BranchModels
    {
        public string id { get; set; }
        public string text { get; set; }
        public string level_step { get; set; }
    }
    public class BranchModelsList
    {
        public List<BranchModels> BranchList { get; set; }
    }
    public class DepartmentModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string DeptID { get; set; }

        [Required]
        [Display(Name = "Department Name")]
        public string DeptName { get; set; }

        [Required]
        [Display(Name = "Division Name")]
        public string branch { get; set; }


        internal bool FunAddDept(DepartmentModels objDepartment)
        {
            try
            {
                string sSqlstmt = "insert into t_departments (DeptName,branch) values('" + objDepartment.DeptName + "','" + objDepartment.branch + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDept: " + ex.ToString());
            }
            return false;
        }


        internal bool FunDeleteDept(string sDeptID)
        {
            try
            {
                string sSqlstmt = "delete from t_departments where DeptId=" + sDeptID;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDept: " + ex.ToString());
            }
            return false;
        }


        internal bool FunEditDept(string sDeptID,string Dept,string branch)
        {
            try
            {
                string sSqlstmt = "update t_departments set DeptName='" + Dept + "',branch='" + branch + "' where DeptId=" + sDeptID;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDept: " + ex.ToString());
            }
            return false;
        }
    }   

}