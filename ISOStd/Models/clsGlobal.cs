using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Net.Mime;
using System.Text.RegularExpressions;
using ISOStd.Controllers;
using System.Web.Routing;


namespace ISOStd.Models
{
    public class clsGlobal
    {
        private object fileUploader;
        private object mail;

        //--------------------------------10/14/2021-----------------------------------------------
        public string GetQAQCDeptEmployees()
        {
            try
            {
                string sSqlstmt = "select group_concat(emp_no) emp_no"
               + " from t_hr_employee t,t_departments tt where emp_status = 1 and t.Dept_Id = tt.DeptId and DeptName like '%QA/QC%'";

                DataSet dsData = Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0 && dsData.Tables[0].Rows[0]["emp_no"].ToString() != "")
                {
                    return (dsData.Tables[0].Rows[0]["emp_no"].ToString());
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetQAQCDeptEmployees: " + ex.ToString());
            }
            return "";
        }
        //returning emp_no by name
        public string GetEmpIDByEmpName(string emp_firstname)
        {
            try
            {
                if (emp_firstname != "")
                {
                    DataSet dsComp = Getdetails("select emp_no from t_hr_employee where emp_firstname='" + emp_firstname + "' and emp_status=1");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["emp_no"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetEmpIDByEmpName: " + ex.ToString());
            }
            return "";
        }
        public DataSet getListPendingForSupplierPerfReview(string sempid)
        {
            try
            {
                string sSqlstmt = "select Id_Performace,ReportNo,Ext_Provider_Name,Scheduled_by from t_extprovider_performance where active=1 and " +
                    "(apprv_status = '0' and find_in_set('" + sempid + "',Approved_by)) ";

                DataSet dsApprovalList = Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    return dsApprovalList;
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getListPendingForSupplierPerfReview: " + ex.ToString());
            }
            return null;
        }

        //--------------------------------10/7/2021------------------------------------------------
        public string GetNCNOById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSqlstmt = "select group_concat(nc_no) nc_no from t_nc where id_nc in (" + item_id + ")";

                    DataSet dsData = Getdetails(sSqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["nc_no"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetNCNOById: " + ex.ToString());
            }
            return "";
        }

        //--------------------------------9/30/2021-------------------------------------------------
        public DataSet getListPendingForReviewEmpCompetence(string sempid) // emp competence eval review
        {
            try
            {
                string sSqlstmt = "select CompetenceId,Evalaution_Done_By,Name,LoggedBy from t_emp_competence_eval where Active = 1 and eval_status = 0 and find_in_set('" + sempid + "', Eval_ReviewedBy)";

                DataSet dsApprovalList = Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    return dsApprovalList;
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getListPendingForReviewEmpCompetence: " + ex.ToString());
            }
            return null;
        }
        public DataSet getListPendingForApprovalEmpCompetence(string sempid) // emp competence eval approve
        {
            try
            {
                string sSqlstmt = "select CompetenceId,Evalaution_Done_By,Name,LoggedBy from t_emp_competence_eval where Active = 1 and eval_status = 2 and find_in_set('" + sempid + "', Eval_ApprovedBy)";

                DataSet dsApprovalList = Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    return dsApprovalList;
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getListPendingForApprovalEmpCompetence: " + ex.ToString());
            }
            return null;
        }
        public MultiSelectList GetTrainingTopicList()
        {
            DropdownList objProductList = new DropdownList();
            objProductList.lstDropdown = new List<DropdownItems>();

            try
            {
                string sSsqlstmt = "select TrainingOrientation_Id as Id, Topic as Name from t_trainingorientation where Active=1";


                DataSet dsBranch = Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objProduct = new DropdownItems()
                        {
                            Id = dsBranch.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsBranch.Tables[0].Rows[i]["Name"].ToString()
                        };

                        objProductList.lstDropdown.Add(objProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetTrainingTopicList: " + ex.ToString());
            }

            return new MultiSelectList(objProductList.lstDropdown, "Id", "Name");
        }
        public string GetTrainingTopicById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSqlstmt = "select group_concat(Topic) Topic from t_trainingorientation where TrainingOrientation_Id in (" + item_id + ")";

                    DataSet dsData = Getdetails(sSqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Topic"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetTrainingTopicById: " + ex.ToString());
            }
            return "";
        }

        public string GetHRDeptEmployees() 
        {
            try
            {
                string sSqlstmt = "select group_concat(emp_no) emp_no"
               + " from t_hr_employee t,t_departments tt where emp_status = 1 and t.Dept_Id = tt.DeptId and DeptName like '%HR%'";

                DataSet dsData = Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["emp_no"].ToString());
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHRDeptEmployees: " + ex.ToString());
            }
            return "";
        }

       

        //HSE Inspection
        public string GetInspectionChecklistRefNamebyRevNo(string item_id, string RevNo)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("select checklist_ref from t_inspection_question_master where id_question_master='" + item_id + "' and RevNo='" + RevNo + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["checklist_ref"].ToString());
                    }
                    else
                    {
                        DataSet dsEmp1 = Getdetails("select checklist_ref from t_inspection_question_master_history where id_question_master='" + item_id + "' and RevNo='" + RevNo + "'");
                        if (dsEmp1.Tables.Count > 0 && dsEmp1.Tables[0].Rows.Count > 0)
                        {
                            return (dsEmp1.Tables[0].Rows[0]["checklist_ref"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetInspectionChecklistRefNamebyRevNo: " + ex.ToString());
            }
            return "";
        }
        public DataSet getListPendingForApproveInspPlan(string sempid)
        {
            try
            {
                string sSqlstmt = "select id_inspection_plan,checklist_ref,insp_type,logged_by from t_inspection_plan"
                + " where active = 1 and approved_status = 0 and approved_by = '" + sempid + "' order by id_inspection_plan desc";

                DataSet dsApprovalList = Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    return dsApprovalList;
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getListPendingForApproveInspPlan: " + ex.ToString());
            }
            return null;
        }
        public string GetInspectionChecklistRefNamebyId(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("select checklist_ref from t_inspection_question_master where id_question_master='" + item_id + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["checklist_ref"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetInspectionChecklistRefNamebyId: " + ex.ToString());
            }
            return "";
        }
        public string GetInspectionStatusIdByName(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  item_id FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='Inspection Status' and item_desc = '" + item_id + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["item_id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetInspectionStatusIdByName: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList FunGetChecklistRefList(string insp_type)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string branch = GetCurrentUserSession().division;
                if (insp_type != "")
                {
                    string sSsqlstmt = "select id_question_master as Id,checklist_ref as Name from t_inspection_question_master"
                    + " where active = 1 and chk_valid = 'Valid' and branch = '" + branch + "' and insp_type='" + insp_type + "' and checklist_status=4";

                    DataSet dsDropdown = Getdetails(sSsqlstmt);
                    if (dsDropdown.Tables.Count > 0 && dsDropdown.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsDropdown.Tables[0].Rows.Count; i++)
                        {
                            DropdownItems objReport = new DropdownItems()
                            {
                                Id = dsDropdown.Tables[0].Rows[i]["Id"].ToString(),
                                Name = dsDropdown.Tables[0].Rows[i]["Name"].ToString()
                            };
                            objReportList.lstDropdown.Add(objReport);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in FunGetChecklistRefList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public DataSet getListPendingForReviewInspChecklist(string sempid)
        {
            try
            {
                string sSqlstmt = "select id_question_master,checklist_ref,insp_type,logged_by from t_inspection_question_master"
                + " where active = 1 and checklist_status = 0 and reviewed_by = '" + sempid + "' order by id_question_master desc";

                DataSet dsApprovalList = Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    return dsApprovalList;
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getListPendingForReviewInspChecklist: " + ex.ToString());
            }
            return null;
        }
        public DataSet getListPendingForApproveInspChecklist(string sempid)
        {
            try
            {
                string sSqlstmt = "select id_question_master,checklist_ref,insp_type,logged_by from t_inspection_question_master"
                + " where active = 1 and checklist_status = 2 and approved_by = '" + sempid + "' order by id_question_master desc";

                DataSet dsApprovalList = Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    return dsApprovalList;
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getListPendingForApproveInspChecklist: " + ex.ToString());
            }
            return null;
        }
        public MultiSelectList GetTopMgmtEmpListbox()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                string branch = GetCurrentUserSession().division;
                DataSet dsEmp = Getdetails("select emp_no as Empid,concat(concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')),' - ',EmailId) as Empname from t_hr_employee e,roles r where e.emp_status=1"
               + " and FIND_IN_SET(id,Role) and RoleName='Top Management' and division='" + branch + "'");


                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {

                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetTopMgmtEmpListbox: " + ex.ToString());
            }
            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList FunGetDeptSectionList(string dept)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                if (dept != "")
                {
                    string sSsqlstmt = "select id_inspection as Id, Section as Name from t_inspection_section where dept='" + dept + "' and active=1";

                    DataSet dsDropdown = Getdetails(sSsqlstmt);
                    if (dsDropdown.Tables.Count > 0 && dsDropdown.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsDropdown.Tables[0].Rows.Count; i++)
                        {
                            DropdownItems objReport = new DropdownItems()
                            {
                                Id = dsDropdown.Tables[0].Rows[i]["Id"].ToString(),
                                Name = dsDropdown.Tables[0].Rows[i]["Name"].ToString()
                            };
                            objReportList.lstDropdown.Add(objReport);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in FunGetDeptSectionList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetComplianceStausIdByName(string item_desc)
        {
            try
            {
                if (item_desc != "")
                {
                    string sSsqlstmt = "select item_id as Id from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Legal Register Compliance Status' and (item_desc='" + item_desc + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetComplianceStausIdByName: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetAllIssueList(string deptId="")
        {
            DropdownList objProductList = new DropdownList();
            objProductList.lstDropdown = new List<DropdownItems>();

            try
            {
                string sSsqlstmt = "";

                if (deptId != "")
                {
                    sSsqlstmt = "select T1.id_issue as Id,CONCAT(Issue_refno,'-',(select count(*) from t_issues AS T2 where T2.Repet_Issue=T1.id_issue)) as Name"
                    + " from t_issues as T1 where T1.Active = 1 and find_in_set('"+ deptId + "', Deptid) order by id_issue asc";
                }
                else
                {
                    sSsqlstmt = "select T1.id_issue as Id,CONCAT(Issue_refno,'-',(select count(*) from t_issues AS T2 where T2.Repet_Issue=T1.id_issue)) as Name"
                   + " from t_issues as T1 where T1.Active = 1  order by id_issue asc";
                }

                DataSet dsBranch = Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objProduct = new DropdownItems()
                        {
                            Id = dsBranch.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsBranch.Tables[0].Rows[i]["Name"].ToString()
                        };

                        objProductList.lstDropdown.Add(objProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAllIssueList: " + ex.ToString());
            }

            return new MultiSelectList(objProductList.lstDropdown, "Id", "Name");
        }

        public string GetIssueRefnobyId(string Id)
        {
            try
            {
                string sql = "Select Issue_refno from t_issues where id_issue = '" + Id + "'";
                DataSet dsEmp = Getdetails(sql);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Issue_refno"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetIssueRefnobyId: " + ex.ToString());
            }
            return "";
        }

        public string GetIssueDetailbyId(string Id)
        {
            try
            {
                string sql = "Select Issue from t_issues where id_issue = '" + Id + "'";
                DataSet dsEmp = Getdetails(sql);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Issue"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetIssueDetailbyId: " + ex.ToString());
            }
            return "";
        }



        public MultiSelectList GetHrEmployeeListbyDesignation(string Designation)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid from t_hr_employee" +
                    " where emp_status=1 and Designation like '" + Designation + "' order by emp_firstname asc";
                DataSet dsEmp = Getdetails(sSqlstmt);

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {

                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")

                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHrEmployeeListbyDesignation: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetComplaintRealtedToList()
        {
            DropdownList objProductList = new DropdownList();
            objProductList.lstDropdown = new List<DropdownItems>();

            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Complaint Related To' order by item_desc asc";

                DataSet dsBranch = Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objProduct = new DropdownItems()
                        {
                            Id = dsBranch.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsBranch.Tables[0].Rows[i]["Name"].ToString()
                        };

                        objProductList.lstDropdown.Add(objProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetComplaintRealtedToList: " + ex.ToString());
            }

            return new MultiSelectList(objProductList.lstDropdown, "Id", "Name");
        }

        public string GetCustomerContactPersonByCustId(string CustId)
        {
            try
            {
                string sql = "Select ContactPerson from t_customer_info_contacts where ContactsId = '"+ CustId +"'";
                DataSet dsEmp = Getdetails(sql);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["ContactPerson"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustomerContactPersonByCustId: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetAllCustomerContactPersonList( string CustID)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSqlstmt = "select ContactsId,a.ContactPerson from t_customer_info_contacts a," +
                    "t_customer_info b where a.CustID = b.CustID and a.active = 1 and a.CustID ='"+ CustID + "'";
                DataSet dsEmp = Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["ContactsId"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["ContactPerson"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAllCustomerContactPersonList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetKPIPotentialStatusById(string id)
        {
            try
            {
                DataSet dsEmp = Getdetails("select item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='KPI Potential Status' and (item_id='" + id + "' or item_desc='" + id + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIPotentialStatusById: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetKPIPotentialStatusList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='KPI Potential Status' order by item_desc asc";
                DataSet dsEmp = Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIPotentialStatusList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetKPIPotentialImpactById(string id)
        {
            try
            {
                DataSet dsEmp = Getdetails("select item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='KPI Potential Impact' and (item_id='" + id + "' or item_desc='" + id + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIPotentialImpactById: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetKPIPotentialImpactList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='KPI Potential Impact' order by item_desc asc";
                DataSet dsEmp = Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIPotentialImpactList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetKPIStatusIdByName(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  item_id FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='KPI Status' and item_desc = '" + item_id + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["item_id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIStatusNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiCompanyBranchNameByID(string sBranchtree)
        {
            CompanyBranchList Branchlist = new CompanyBranchList();
            Branchlist.CompBranchList = new List<CompanyBranch>();
            try
            {
                DataSet dsBranch = Getdetails("select id, branchname from t_company_branch where Active=1 and id in (" + sBranchtree + ")");// and CompanyId='" + sCompanyId+"'");

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        CompanyBranch Branch = new CompanyBranch()
                        {
                            BranchId = dsBranch.Tables[0].Rows[i]["Id"].ToString(),
                            BranchName = dsBranch.Tables[0].Rows[i]["BranchName"].ToString().ToUpper()
                        };

                        Branchlist.CompBranchList.Add(Branch);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiCompanyBranchNameByID: " + ex.ToString());
            }
            return new MultiSelectList(Branchlist.CompBranchList, "BranchId", "BranchName");
        }

        public MultiSelectList GetKPIFrequencyEvaluationList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='KPI Frequency Evaluation' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIFrequencyEvaluationList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }
        public string GetKPIFrequencyEvaluationById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='KPI Frequency Evaluation' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIFrequencyEvaluationById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetKPIPerformanceIndicatorList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='KPI Performance Indicator' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIPerformanceIndicatorList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }
        public string GetKPIPerformanceIndicatorById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='KPI Performance Indicator' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIPerformanceIndicatorById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetKPILevelList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='KPI Level' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPILevelList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }
        public string GetKPILevelById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='KPI Level' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPILevelById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetKPITypeList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='KPI Type' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPITypeList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }
        public string GetKPITypeById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='KPI Type' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPITypeById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetKPIMonitoringMechanismList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='KPI Monitoring Mechanism' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIMonitoringMechanismList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }
        public string GetKPIMonitoringMechanismById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='KPI Monitoring Mechanism' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIMonitoringMechanismById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetKPIStatusList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='KPI Status' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIStatusList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }
        public string GetKPIStatusById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='KPI Status' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIStatusById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetKPIEvaluationStatusList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='KPI Evaluation Status' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIEvaluationStatusList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }
        public string GetKPIEvaluationStatusById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='KPI Evaluation Status' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIEvaluationStatusById: " + ex.ToString());
            }
            return "";
        }

        //------------------------------------6/21/2021--------------------------------------------

        //---------------------Start GG Functions-------------

        public MultiSelectList GetGEmpListBymulitBDL(string sDivision = "", string sDept = "", string sLoc = "")
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                    string Sqlstmt = "select emp_no as Empid,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Empname"
                                 + " from t_hr_employee where emp_status = 1 ";

                    if (sDivision == "" && sDept == "" && sLoc == "")
                    {
                        Sqlstmt = Sqlstmt + " order by Empname";
                    }
                    else if (sDivision != "" && sDept != "" && sLoc != "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') and FIND_IN_SET(Dept_Id, '" + sDept + "') and FIND_IN_SET(Emp_work_location, '" + sLoc + "') order by Empname";
                    }
                    else if (sDept == "" && sLoc == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') order by Empname";
                    }
                    else if (sLoc == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') and FIND_IN_SET(Dept_Id, '" + sDept + "') order by Empname ";
                    }
                    else if (sDivision == "" && sDept == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(Emp_work_location, '" + sLoc + "') order by Empname";
                    }
                    else if (sDept == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') and FIND_IN_SET(Emp_work_location, '" + sLoc + "') order by Empname ";
                    }

                    DataSet dsEmp = Getdetails(Sqlstmt);

                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                        {
                            Employee emp = new Employee()
                            {
                                Empid = dsEmp.Tables[0].Rows[i]["empid"].ToString(),
                                Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                            };
                            emp.Empname = emp.Empname.Trim();
                            emplist.EmpList.Add(emp);
                        }
                    }
                }           
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetGEmpListBymulitBDL: " + ex.ToString());
            }
            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetGEmpListByMultiDept(string dept)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {                 
                    string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid from t_hr_employee where emp_status=1" +
                        " and Dept_Id in (" + dept + ") order by emp_firstname asc";
                    DataSet dsEmp = Getdetails(sSqlstmt);

                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                        {
                            Employee emp = new Employee()
                            {
                                Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                                Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")

                            };
                            emp.Empname = emp.Empname.Trim();
                            emplist.EmpList.Add(emp);
                        }
                    }                      
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetGEmpListByMultiDept: " + ex.ToString());
            }
            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetGEmpListbymultiBranch(string branch)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                if (branch != "")
                {                   
                    string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) " +
                        "as Empname, emp_no as Empid from t_hr_employee where emp_status=1 and division in (" + branch + ") order by emp_firstname asc";
                    DataSet dsEmp = Getdetails(sSqlstmt);

                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                        {

                            Employee emp = new Employee()
                            {
                                Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                                Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                            };
                            emp.Empname = emp.Empname.Trim();
                            emplist.EmpList.Add(emp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetGEmpListbymultiBranch: " + ex.ToString());
            }
            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetGLocListbymultiBranch(string branch)
        {
            CompanyBranchList LocList = new CompanyBranchList();
            LocList.CompBranchList = new List<CompanyBranch>();

            try
            {
                if (branch != "" && branch != null)
                {                   
                    string sql = "SELECT distinct id_location as Id ,location_name as BranchName FROM t_location a,t_division_mapping b " +
                     "where a.id_country = b.id_country and b.id_branch in (" + branch + ") and active=1";

                    DataSet dsLoc = Getdetails(sql);

                    if (dsLoc.Tables.Count > 0 && dsLoc.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsLoc.Tables[0].Rows.Count; i++)
                        {
                            CompanyBranch div = new CompanyBranch()
                            {
                                BranchId = dsLoc.Tables[0].Rows[i]["Id"].ToString(),
                                BranchName = dsLoc.Tables[0].Rows[i]["BranchName"].ToString()
                            };
                            LocList.CompBranchList.Add(div);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetGLocListbymultiBranch: " + ex.ToString());
            }
            return new MultiSelectList(LocList.CompBranchList, "BranchId", "BranchName");
        }

        public MultiSelectList GetGDeptListbymultiBranch(string branch)
        {
            DepartmentList Deptlist = new DepartmentList();
            Deptlist.DeptList = new List<Department>();

            try
            {
                if (branch != "" && branch != null)
                {                   
                    string sql = "select distinct DeptId,DeptName from t_dept_division where find_in_set(divid,'" + branch + "');";

                    DataSet dsEmp = Getdetails(sql);

                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                        {
                            Department emp = new Department()
                            {
                                Deptid = dsEmp.Tables[0].Rows[i]["DeptId"].ToString(),
                                Deptname = dsEmp.Tables[0].Rows[i]["DeptName"].ToString()
                            };

                            Deptlist.DeptList.Add(emp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetGDeptListbymultiBranch: " + ex.ToString());
            }
            return new MultiSelectList(Deptlist.DeptList, "Deptid", "Deptname");
        }

        public MultiSelectList GetGRoleList(string sDivision = "", string sDept = "", string sLoc = "", string sRole = "")
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                if (sRole != "")
                {
                    string Sqlstmt = "select emp_no as Empid,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Empname"
                                 + " from t_hr_employee h, roles r where  h.emp_status = 1 and FIND_IN_SET(id, Role) and RoleName = '" + sRole + "'";

                    if (sDivision == "" && sDept == "" && sLoc == "")
                    {
                        Sqlstmt = Sqlstmt + " order by Empname";
                    }
                    else if (sDivision != "" && sDept != "" && sLoc != "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') and FIND_IN_SET(Dept_Id, '" + sDept + "') and FIND_IN_SET(Emp_work_location, '" + sLoc + "') order by Empname";
                    }
                    else if (sDept == "" && sLoc == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') order by Empname";
                    }
                    else if (sLoc == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') and FIND_IN_SET(Dept_Id, '" + sDept + "') order by Empname ";
                    }
                    else if(sDivision == "" && sDept == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(Emp_work_location, '" + sLoc + "') order by Empname";
                    }
                    else if (sDept == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') and FIND_IN_SET(Emp_work_location, '" + sLoc + "') order by Empname ";
                    }                  

                    DataSet dsEmp = Getdetails(Sqlstmt);

                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                        {
                            Employee emp = new Employee()
                            {
                                Empid = dsEmp.Tables[0].Rows[i]["empid"].ToString(),
                                Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                            };
                            emp.Empname = emp.Empname.Trim();
                            emplist.EmpList.Add(emp);
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetGRoleList: " + ex.ToString());
            }
            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetGRolelikeList(string sDivision = "", string sDept = "", string sLoc = "", string sRole = "")
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                if (sRole != "")
                {
                    string Sqlstmt = "select emp_no as Empid,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Empname"
                                 + " from t_hr_employee h, roles r where  h.emp_status = 1 and FIND_IN_SET(id, Role) and RoleName Like '"+sRole+"'";

                    if (sDivision == "" && sDept == "" && sLoc == "")
                    {
                        Sqlstmt = Sqlstmt + " order by Empname";
                    }
                    else if (sDivision != "" && sDept != "" && sLoc != "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') and FIND_IN_SET(Dept_Id, '" + sDept + "') and FIND_IN_SET(Emp_work_location, '" + sLoc + "') order by Empname";
                    }
                    else if (sDept == "" && sLoc == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') order by Empname";
                    }
                    else if (sLoc == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') and FIND_IN_SET(Dept_Id, '" + sDept + "') order by Empname ";
                    }
                    else if (sDivision == "" && sDept == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(Emp_work_location, '" + sLoc + "') order by Empname";
                    }
                    else if (sDept == "")
                    {
                        Sqlstmt = Sqlstmt + " and FIND_IN_SET(division, '" + sDivision + "') and FIND_IN_SET(Emp_work_location, '" + sLoc + "') order by Empname ";
                    }

                    DataSet dsEmp = Getdetails(Sqlstmt);

                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                        {
                            Employee emp = new Employee()
                            {
                                Empid = dsEmp.Tables[0].Rows[i]["empid"].ToString(),
                                Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                            };
                            emp.Empname = emp.Empname.Trim();
                            emplist.EmpList.Add(emp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetGRolelikeList: " + ex.ToString());
            }
            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }


        //--------------------End GG Functions-------------


        public string GetSectionNameById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='DCR Question Sections' and (item_id='" + item_id + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetSectionNameById: " + ex.ToString());
            }
            return "";
        }
        public string GetDCRNOById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct dcr_no FROM t_document_create_request where id_doc_request = '" + item_id + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["dcr_no"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDCRNOById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetDCRDocumentList(string branch,string location)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_doc_request as Id, dcr_no as Name from t_document_create_request where doc_status = 2  and active = 1";
                
                if(branch == "" && location == "")
                {
                    sSsqlstmt = sSsqlstmt+ "order by Name asc"; 
                }
                else if (branch != "" && location != "")
                {
                    sSsqlstmt = sSsqlstmt + "  and find_in_set(division,'" + branch + "') and  find_in_set(location,'" + location + "')  order by Name asc";
                }
                else if (location == "")
                {
                    sSsqlstmt = sSsqlstmt + "  and find_in_set(division,'" + branch + "') order by Name asc";
                }
                else if (branch != "")
                {
                    sSsqlstmt = sSsqlstmt + "  and find_in_set(location,'" + location + "') order by Name asc";
                }

                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDCRDocumentList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        //----------------audit---------------

        public string GetNCStatusIdByName(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  item_id FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='NCR Status' and item_desc = '" + item_id + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["item_id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetNCStatusIdByName: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetAuditeesList(string branch = "", string group = "")
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            DataSet dsData;
            try
            {
                if (branch != null && group != null && branch != "" && group != "")
                {
                    dsData = Getdetails("select emp_no as Empid,concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname"
                   + " from t_hr_employee where emp_status = 1 and (division = '" + branch + "' and Dept_Id = '" + group + "')");
                }
                else
                {
                    dsData = Getdetails("select emp_no as Empid,concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname"
                    + " from t_hr_employee where emp_status = 1 and (division = '" + branch + "' or  Dept_Id = '" + group + "')");
                }

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsData.Tables[0].Rows[i]["empid"].ToString(),
                            Empname = Regex.Replace(dsData.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditeesList: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }
               
        public MultiSelectList GetAuditorsList(string branch, string shedule_date)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            DataSet dsData;
            try
            {
                if (branch != "" && shedule_date != "" && branch != null && shedule_date != null)
                {
                    //dsData = Getdetails("select distinct emp_no as Empid,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Empname from t_auditor_detail T1, t_hr_employee T2"
                    //+ " where id_auditor not in (select id_auditor FROM kipic.t_auditor_availability where '"+ shedule_date + "' between from_date and to_date) and T1.auditor_name = T2.emp_no and T2.division != '" + branch + "'");

                    dsData = Getdetails("select distinct emp_no as Empid,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Empname from t_auditor_detail T1, t_hr_employee T2"
                    + " where auditor_name not in (select auditor_name FROM t_auditor_availability T1, t_auditor_detail T2 where (('" + shedule_date + "' between from_date and to_date) or from_date = '" + shedule_date + "')"
                    + " and t1.id_auditor = t2.id_auditor) and auditor_name not in (select auditors from t_audit_process_plan where AuditDate = '" + shedule_date + "') and T1.auditor_name = T2.emp_no and T2.division != '" + branch + "'");


                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                        {
                            Employee emp = new Employee()
                            {
                                Empid = dsData.Tables[0].Rows[i]["empid"].ToString(),
                                Empname = Regex.Replace(dsData.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                            };
                            emp.Empname = emp.Empname.Trim();
                            emplist.EmpList.Add(emp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditeesList: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }


        public MultiSelectList GetScheduleAuditScopeList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Schedule Audit Scope' order by item_desc asc";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetScheduleAuditScopeList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetMultiScheduleAuditScopeById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("Select GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='Schedule Audit Scope' and item_id in ('" + item_id + "')");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiScheduleAuditScopeById: " + ex.ToString());
            }
            return "";
        }


        public MultiSelectList GetNCRiskLevelList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='NC Risklevel' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetNCRiskLevelList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public string GetNCRiskLevelById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='NC Risklevel' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetNCRiskLevelById: " + ex.ToString());
            }
            return "";
        }

        public string GetAuditorTypeOfCourseById(string id)
        {
            try
            {
                DataSet dsEmp = Getdetails("select item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Auditors Type of Course' and (item_id='" + id + "' or item_desc='" + id + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditorTypeOfCourseById: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetAuditorTypeOfCourseList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Auditors Type of Course' order by item_desc asc";
                DataSet dsEmp = Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditorTypeOfCourseList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }


        public MultiSelectList GetHrEmployeeListbox(string EmailID = "")
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                string sSqlstmt = "";
                if (EmailID != "")
                {
                    sSqlstmt = "select concat(concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')),' - ',EmailId) as Empname, emp_no as Empid"
                    + " from t_hr_employee where emp_status = 1 order by emp_firstname asc";
                }
                else
                {
                    sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid from t_hr_employee where emp_status=1 order by emp_firstname asc";
                }

                DataSet dsEmp = Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {

                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")

                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHrEmployeeListbox: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetAuditNCList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='NC Category' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditNCList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public string GetAuditNCById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='NC Category' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditNCById: " + ex.ToString());
            }
            return "";
        }

        public string GetAuditStatusIdByName(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  item_id FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='Audit Status' and item_desc = '" + item_id + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["item_id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditStatusNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetAuditStatusList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Audit Status' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditStatusList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public string GetAuditStatusById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='Audit Status' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditStatusById: " + ex.ToString());
            }
            return "";
        }


        public MultiSelectList GetChecklistTypeByChecklistRef(string branch = "")
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "";
                if (branch != "" && branch != null)
                {
                    sSsqlstmt = "select id_AuditChecklist as Id, ChecklistRef as Name from t_auditchecklist where Active=1 and  directorate='" + branch + "' order by id_AuditChecklist asc";
                }
                else
                {
                    sSsqlstmt = "select id_AuditChecklist as Id, ChecklistRef as Name from t_auditchecklist where Active=1 order by id_AuditChecklist asc";
                }

                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetChecklistTypeByChecklistRef: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }


        //public MultiSelectList GetAuditStatusList()
        //{
        //    DropdownList objReportList = new DropdownList();
        //    objReportList.lstDropdown = new List<DropdownItems>();
        //    try
        //    {
        //        string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
        //            + "and header_desc='Audit Status' order by item_desc asc";

        //        DataSet dsReportType = Getdetails(sSsqlstmt);
        //        if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
        //            {
        //                DropdownItems objReport = new DropdownItems()
        //                {
        //                    Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
        //                    Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
        //                };
        //                objReportList.lstDropdown.Add(objReport);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AddFunctionalLog("Exception in GetAuditStatusList: " + ex.ToString());
        //    }
        //    return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        //}
        //public string GetAuditStatusById(string item_id)
        //{
        //    try
        //    {
        //        if (item_id != "" && item_id != null)
        //        {
        //            DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
        //            + " and header_desc='Audit Status' and item_id in (" + item_id + ")");
        //            if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //            {
        //                string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
        //                if (sDesc != "" && sDesc.Contains(','))
        //                {
        //                    return sDesc.Replace(",", ", ");
        //                }
        //                return sDesc;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AddFunctionalLog("Exception in GetAuditStatusById: " + ex.ToString());
        //    }
        //    return "";
        //}

        //public string GetTeamNameByID(string item_id)
        //{
        //    try
        //    {
        //        if (item_id != "" && item_id != null)
        //        {
        //            DataSet dsData = Getdetails("Select  GROUP_CONCAT(Type_Details) Name FROM t_doc_team  where Team_id in (" + item_id + ")");
        //            if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
        //            {
        //                string sDesc = dsData.Tables[0].Rows[0]["Name"].ToString();
        //                if (sDesc != "" && sDesc.Contains(','))
        //                {
        //                    return sDesc.Replace(",", ", ");
        //                }
        //                return sDesc;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AddFunctionalLog("Exception in GetTeamNameByID: " + ex.ToString());
        //    }
        //    return "";
        //}

        //public MultiSelectList GetMultiTeambyMultiGroup(string sGroupId = "")
        //{
        //    EmployeeList emplist = new EmployeeList();
        //    emplist.EmpList = new List<Employee>();

        //    try
        //    {
        //        string sSqlstmt = "";
        //        if (sGroupId != "")
        //        {
        //            sSqlstmt = "select Team_id,Type_Details from t_doc_team where Group_id in (" + sGroupId + ") order by Type_Details asc";
        //        }
        //        else
        //        {
        //            sSqlstmt = "select Team_id,Type_Details from t_doc_team  order by Type_Details asc";
        //        }
        //        DataSet dsEmp = Getdetails(sSqlstmt);

        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
        //            {

        //                Employee emp = new Employee()
        //                {
        //                    Empid = dsEmp.Tables[0].Rows[i]["Team_id"].ToString(),
        //                    Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Type_Details"].ToString(), " +", " ")

        //                };
        //                emp.Empname = emp.Empname.Trim();
        //                emplist.EmpList.Add(emp);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        AddFunctionalLog("Exception in GetMultiTeambyMultiGroup: " + ex.ToString());
        //    }
        //    return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        //}

        //public MultiSelectList GetDocTeamListbyGroup(string sId)
        //{
        //    DropdownList objReportList = new DropdownList();
        //    objReportList.lstDropdown = new List<DropdownItems>();
        //    try
        //    {
        //        DataSet dsEmp = Getdetails("select Team_id,Type_Details from t_doc_team where Group_id='" + sId + "'");

        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
        //            {
        //                DropdownItems objReport = new DropdownItems()
        //                {
        //                    Id = dsEmp.Tables[0].Rows[i]["Team_id"].ToString(),
        //                    Name = dsEmp.Tables[0].Rows[i]["Type_Details"].ToString()
        //                };
        //                objReportList.lstDropdown.Add(objReport);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AddFunctionalLog("Exception in GetDocTeamListbyGroup: " + ex.ToString());
        //    }
        //    return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        //}

        //public MultiSelectList GetDocTeamList()
        //{
        //    DropdownList objReportList = new DropdownList();
        //    objReportList.lstDropdown = new List<DropdownItems>();
        //    try
        //    {
        //        DataSet dsEmp = Getdetails("select Team_id,Type_Details from t_doc_team ");

        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
        //            {
        //                DropdownItems emp = new DropdownItems()
        //                {
        //                    Id = dsEmp.Tables[0].Rows[i]["Team_id"].ToString(),
        //                    Name = dsEmp.Tables[0].Rows[i]["Type_Details"].ToString()
        //                };

        //                objReportList.lstDropdown.Add(emp);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AddFunctionalLog("Exception in GetDocTeamList: " + ex.ToString());
        //    }
        //    return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        //}


        public MultiSelectList GetAuidtorLevelList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Auditor Level' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuidtorLevelList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public string GetAuditorLevelById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='Auditor Level' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditorLevelById: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList FunGetAuditProductList(string audit_type)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();

            try
            {
                if (audit_type != "" && audit_type != "")
                {

                    string sSsqlstmt = "select id_audit_type,audit_code from t_audit_type where audit_type = '" + audit_type + "'";

                    DataSet dsReportType = Getdetails(sSsqlstmt);
                    if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                        {
                            DropdownItems objReport = new DropdownItems()
                            {
                                Id = dsReportType.Tables[0].Rows[i]["id_audit_type"].ToString(),
                                Name = dsReportType.Tables[0].Rows[i]["audit_code"].ToString()
                            };
                            objReportList.lstDropdown.Add(objReport);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditeesList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public string GetAuditProductById(string item_id)
        {

            try
            {
                if (item_id != "")
                {
                    DataSet dsDept = Getdetails("select audit_code from t_audit_type where id_audit_type = '" + item_id + "'");
                    if (dsDept.Tables.Count > 0 && dsDept.Tables[0].Rows.Count > 0)
                    {
                        return (dsDept.Tables[0].Rows[0]["audit_code"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditProductById: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetAuidtTypeList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Audit Type' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuidtTypeList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public string GetAuidtTypeById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='Audit Type' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuidtTypeById: " + ex.ToString());
            }
            return "";
        }


        public MultiSelectList GetAuidtCycleList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Audit Cycle' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuidtCycleList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public string GettAuidtCycleById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.item_desc) item_desc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='Audit Cycle' and item_id in (" + item_id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GettAuidtCycleById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetAuditNoFromAuditProcessList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select Audit_Id as Id, Audit_no as Name from t_audit_process where active = 1 order by Audit_Id desc";
                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditNoFromAuditProcessList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }

        public string GetAuditNoFromAuditProcessById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select Audit_no as Name from t_audit_process where Audit_Id='" + item_id + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditNoFromAuditProcessById: " + ex.ToString());
            }
            return "";
        }

       

        //-----------------

        public MultiSelectList GetDropdownList(string header_desc)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                if (header_desc != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                  + "and header_desc='" + header_desc + "' order by item_desc asc";

                DataSet dsDropdown = Getdetails(sSsqlstmt);
                if (dsDropdown.Tables.Count > 0 && dsDropdown.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsDropdown.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsDropdown.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsDropdown.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDropdownList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetDropdownitemById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select group_concat(item_desc) as Name from dropdownitems where item_id in (" + item_id + ")";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsData.Tables[0].Rows[0]["Name"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDropdownitemById: " + ex.ToString());
            }
            return "";
        }

        public string GetComplaintStausIdByName(string item_desc)
        {
            try
            {
                if (item_desc != "")
                {
                    string sSsqlstmt = "select item_id as Id from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Complaint Status' and (item_desc='" + item_desc + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetComplaintStausIdByName: " + ex.ToString());
            }
            return "";
        }

        public string GetDocumentLevelbyId(string Id)
        {
            try
            {
                DataSet dsDoc = Getdetails("select Document_Level from t_document_levels where id_doc_level='" + Id + "' and active=1");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return dsDoc.Tables[0].Rows[0]["Document_Level"].ToString();
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDocumentLevelbyId: " + ex.ToString());
            }
            return "";
        }

        public string GetDocumentTypebyId(string Id)
        {
            try
            {
                DataSet dsDoc = Getdetails("select description from t_document_levels_details where id_levels_details='" + Id + "' and active=1");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return dsDoc.Tables[0].Rows[0]["description"].ToString();
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDocumentTypebyId: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetDocLevelsList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_doc_level as Id,Document_Level as Name from t_document_levels where active=1";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDocLevelsList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }

        public MultiSelectList GetDocTypeListbyDocLevels(string LevelId)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_levels_details as Id,description as Name from t_document_levels_details where id_doc_level = '" + LevelId + "' and active=1";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDocTypeListbyDocLevels: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }

        public MultiSelectList GetRiskNoList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select risk_id as Id, risk_refno as Name, 'RR' as Name1 from risk_register where active = 1 and Risk_Type = 'Positive' " +
                    "union " +
                    "select id_hazard as Id, hazard_refno as Name, 'HR' as Name1 from t_hazard where active = 1 " +
                    "union " +
                    "select id_env_risk as Id, env_refno as Name, 'ER' as Name1 from t_environment_risk where active = 1";
                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString()+":"+ dsReportType.Tables[0].Rows[i]["Name1"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString(),                          
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRiskNoList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }

        public string GetRiskRefByRiskId(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "";
                    string[] id=item_id.Split(':');
                    if(id[1] == "RR")
                    {
                        sSsqlstmt = "select risk_refno as Name from risk_register where risk_id='" + id[0] + "'";
                    }
                    if (id[1] == "HR")
                    {
                        sSsqlstmt = "select hazard_refno as Name from t_hazard where id_hazard='" + id[0] + "'";
                    }
                    if (id[1] == "ER")
                    {
                        sSsqlstmt = "select env_refno as Name from t_environment_risk where id_env_risk='" + id[0] + "'";
                    }

                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRiskRefByRiskId: " + ex.ToString());
            }
            return "";
        }

        public string GetRiskDescriptionByRiskId(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "";
                    string[] id = item_id.Split(':');
                    if (id[1] == "RR")
                    {
                        sSsqlstmt = "select risk_desc as Name from risk_register where risk_id='" + id[0] + "'";
                    }
                    if (id[1] == "HR")
                    {
                        sSsqlstmt = "select activity as Name from t_hazard where id_hazard='" + id[0] + "'";
                    }
                    if (id[1] == "ER")
                    {
                        sSsqlstmt = "select activity as Name from t_environment_risk where id_env_risk='" + id[0] + "'";
                    }

                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRiskDescriptionByRiskId: " + ex.ToString());
            }
            return "";
        }
         
        public MultiSelectList GetAuditListforAuditLog()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select a.Audit_Id as Id,Audit_no as Name from t_audit_process a,t_audit_process_perform b" +
                    " where a.Audit_Id=b.Audit_Id and followup_date != '' and nc_status='Close' and a.active = 1 order by a.Audit_Id desc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditListforAuditLog: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetDeptHeadbyDept(string dept_id)
        {
            try
            {
                if (dept_id != "")
                {
                    DataSet dsEmp = Getdetails("select group_concat(emp_no) as depthead FROM t_hr_employee where find_in_set(Dept_Id,dept_id)>0 and DeptInCharge='Yes' and emp_status=1");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["depthead"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDeptHeadbyDept: " + ex.ToString());
            }
            return "";
        }

        public string GetDivisionIdByHrEmpId(string Emp_no)
        {
            try
            {
                if (Emp_no != "")
                {
                    DataSet dsDept = Getdetails("select division from t_hr_employee where emp_no='" + Emp_no + "'");
                    if (dsDept.Tables.Count > 0 && dsDept.Tables[0].Rows.Count > 0)
                    {
                        return (dsDept.Tables[0].Rows[0]["division"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDivisionIdByHrEmpId: " + ex.ToString());
            }
            return "";
        }
             
        public string GetBranchShortNameByID(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select BranchCode as Name from t_company_branch where id = '" + item_id + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetBranchShortNameByID: " + ex.ToString());
            }
            return "";
        }

        public bool checkDCRChkListRefExists(string checklistRef)
        {
            try
            {

                string sSqlstmt = "select checklistRef from t_document_create_request_chklist where checklistRef='" + checklistRef + "' and active=1";
                DataSet dsEmp = Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in checkDCRChkListRefExists: " + ex.ToString());
            }
            return true;
        }

        public string GetDCRChecklistById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select ChecklistRef from t_document_create_request_chklist where id_chklist = '" + item_id + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["ChecklistRef"].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDCRChecklistById: " + ex.ToString());
            }
            return "";
        }
        
        public string GetDCRNoByDocId(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select dcr_no from t_document_create_request where id_doc_request='" + item_id + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["dcr_no"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDCRNoByDocId: " + ex.ToString());
            }
            return "";
        }

        public string GetDCRChecklistByRef(string checklistRef)
        {
            try
            {
                if (checklistRef != "")
                {
                    string sSsqlstmt = "select checklistRef as Name from t_document_create_request_chklist where checklistRef ='ChkList-" + checklistRef + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDCRChecklistByRef: " + ex.ToString());
            }
            return "";
        }

        public string GetChklistIdByRef(string checklistRef)
        {
            try
            {
                if (checklistRef != "")
                {
                    string sSsqlstmt = "select id_chklist as Id from t_document_create_request_chklist where checklistRef ='" + checklistRef + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetChklistIdByRef: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetIMSGroupRole()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                DataSet dsEmp = Getdetails("select emp_no as Empid,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Empname"
               + " from t_hr_employee h, roles r where  h.emp_status = 1 and FIND_IN_SET(id, Role) and RoleName Like '%IMS %Rep%'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetIMSGroupRole: " + ex.ToString());
            }
            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetPortalNameListBox()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_portal_ministry_name as Id,portal_name  as Name from t_portal_ministry_name where active=1 order by portal_name asc";
                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetPortalNameListBox: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetPortalNamebyPortalId(string id_portal_ministry_name)
        {           
            try
            {
                if (id_portal_ministry_name != "")
                {
                    string sSsqlstmt = "select portal_name  as Name from t_portal_ministry_name where id_portal_ministry_name='" + id_portal_ministry_name + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }                
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetPortalNamebyPortalId: " + ex.ToString());
            }
            return "";
        }

        public string GetMinistryNamebyPortalId(string id_portal_ministry_name)
        {
            try
            {
                if (id_portal_ministry_name != "")
                {
                    string sSsqlstmt = "select ministry_name  as Name from t_portal_ministry_name where id_portal_ministry_name='" + id_portal_ministry_name + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMinistryNamebyPortalId: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetPortalAccessUserNameList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select distinct nominated_emp as Id,concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' '))  as Name from t_portal_authorization a, t_hr_employee b where b.emp_no=a.nominated_emp and a.active =1 and apporved_status = 5 order by id_authorization desc";
                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetPortalAccessUserNameList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }

        //Noly for management here empstatus=0
        public MultiSelectList GetVP()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                string sSqlstmt = "select emp_no as Id,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Name " +
                    "from t_hr_employee h, roles r where  h.mgmtflag = 0 and FIND_IN_SET(id, Role) and RoleName like '%VP%' || RoleName like '%president%'";

                DataSet dsEmp = Getdetails(sSqlstmt);

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Name"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetVP: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }
        
        //Noly for management here empstatus=0
        public MultiSelectList GetChairman()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                string sSqlstmt = "select emp_no as Id,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Name " +
                    "from t_hr_employee h, roles r where  h.mgmtflag = 1 and FIND_IN_SET(id, Role) and RoleName like '%CHAIRMAN%'";

                DataSet dsEmp = Getdetails(sSqlstmt);

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Name"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetChairman: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }
        
        public MultiSelectList GetCEO()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                DataSet dsEmp = Getdetails("select emp_no as Empid,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Empname"
               + " from t_hr_employee h, roles r where  h.emp_status = 1 and FIND_IN_SET(id, Role) and (RoleName = 'CEO' or RoleName like '%CEO%')");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCEO: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }
                
        public string GetPortalCategoryIdByName(string item_Name)
        {
            try
            {
                if (item_Name != "")
                {
                    string sSsqlstmt = "select item_id as Id from dropdownitems a,dropdownheader b where b.header_desc ='Portal Category'" +
                        " and b.header_id=a.header_id and item_desc='" + item_Name + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetPortalCategoryIdByName: " + ex.ToString());
            }
            return "";
        }
         
        //public MultiSelectList GetAuditNoFromAuditProcessList()
        //{
        //    DropdownList objReportList = new DropdownList();
        //    objReportList.lstDropdown = new List<DropdownItems>();
        //    try
        //    {
        //        string sSsqlstmt = "select Audit_Id as Id, Audit_no as Name from t_audit_process where active = 1 order by Audit_Id desc";
        //        DataSet dsReportType = Getdetails(sSsqlstmt);
        //        if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
        //            {
        //                DropdownItems objReport = new DropdownItems()
        //                {
        //                    Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
        //                    Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
        //                };
        //                objReportList.lstDropdown.Add(objReport);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AddFunctionalLog("Exception in GetAuditNoFromAuditProcessList: " + ex.ToString());
        //    }
        //    return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        //}

        //public string GetAuditNoFromAuditProcessById(string item_id)
        //{
        //    try
        //    {
        //        if (item_id != "")
        //        {
        //            string sSsqlstmt = "select Audit_no as Name from t_audit_process where Audit_Id='" + item_id + "'";
        //            DataSet dsData = Getdetails(sSsqlstmt);
        //            if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
        //            {
        //                return (dsData.Tables[0].Rows[0]["Name"].ToString());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AddFunctionalLog("Exception in GetAuditNoFromAuditProcessById: " + ex.ToString());
        //    }
        //    return "";
        //}
        
        public string GetWorkLocationIdByHrEmpId(string Emp_no)
        {
            try
            {
                if (Emp_no != "")
                {
                    DataSet dsLoc = Getdetails("select Emp_work_location from t_hr_employee where emp_no='" + Emp_no + "'");
                    if (dsLoc.Tables.Count > 0 && dsLoc.Tables[0].Rows.Count > 0)
                    {
                        return (dsLoc.Tables[0].Rows[0]["Emp_work_location"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetWorkLocationIdByHrEmpId: " + ex.ToString());
            }
            return "";
        }
             
        public string GetIsoStdIdByName(string sName)
        {
            try
            {
                if (sName != "")
                {
                    string sSsqlstmt = "select StdId as Name from t_isostandards where IsoName='" + sName + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetIsoStdIdByName: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetNCRNumberList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
             string sSsqlstmt = "select CAR_ID as Id, NC_Num as Name from t_ncr_raise where Active=1 order by CAR_ID desc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetNCRNumberList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetNCRNumberById(string sCAR_ID)
        {
            try
            {
                if (sCAR_ID != "")
                {
                    string sSsqlstmt = "select NC_Num as Name from t_ncr_raise where CAR_ID='" + sCAR_ID + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetNCRNumberById: " + ex.ToString());
            }
            return "";
        }         
      
        public MultiSelectList GetDepartmentList1(string branch)
        {
            DepartmentList Deptlist = new DepartmentList();
            Deptlist.DeptList = new List<Department>();
            try
            {
                string sql = "";
                if (branch != "" && branch != null)
                {
                    sql = "select distinct DeptId,DeptName from t_dept_division where find_in_set(divid,'" + branch + "');";
                }
                DataSet dsEmp = Getdetails(sql);

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Department emp = new Department()
                        {
                            Deptid = dsEmp.Tables[0].Rows[i]["DeptId"].ToString(),
                            Deptname = dsEmp.Tables[0].Rows[i]["DeptName"].ToString()
                        };

                        Deptlist.DeptList.Add(emp);

                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDepartmentList1: " + ex.ToString());
            }
            return new MultiSelectList(Deptlist.DeptList, "DeptId", "DeptName");
        }

        public MultiSelectList GetLocationbyMultiDivision(string branch)
        {
            CompanyBranchList Branchlist = new CompanyBranchList();
            Branchlist.CompBranchList = new List<CompanyBranch>();
            try
            {
               if (branch != "" && branch != null)
                {
                   string sql = "SELECT distinct id_location as Id ,location_name as BranchName FROM t_location a,t_division_mapping b " +
                    "where a.id_country = b.id_country and b.id_branch in (" + branch + ") and active=1";

                    DataSet dsLoc = Getdetails(sql);

                    if (dsLoc.Tables.Count > 0 && dsLoc.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsLoc.Tables[0].Rows.Count; i++)
                        {
                            CompanyBranch div = new CompanyBranch()
                            {
                                BranchId = dsLoc.Tables[0].Rows[i]["Id"].ToString(),
                                BranchName = dsLoc.Tables[0].Rows[i]["BranchName"].ToString()
                            };

                            Branchlist.CompBranchList.Add(div);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLocationbyMultiDivision: " + ex.ToString());
            }
            return new MultiSelectList(Branchlist.CompBranchList, "BranchId", "BranchName");
        }

        public string GetDivisionLocationById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("Select GROUP_CONCAT(location_name) item_desc FROM t_location " +
                        "where id_location in (" + item_id + ") and active =1");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_desc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDivisionLocationById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetDivisionLocationList(string Division)
        {
            CompanyBranchList Branchlist = new CompanyBranchList();
            Branchlist.CompBranchList = new List<CompanyBranch>();
            try
            {
                string sqlstmt = "SELECT id_location as Id ,location_name as BranchName FROM t_location a,t_division_mapping b " +
                    "where a.id_country = b.id_country and b.id_branch = '" + Division + "' and active=1";
                DataSet dsBranch = Getdetails(sqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        CompanyBranch Branch = new CompanyBranch()
                        {
                            BranchId = dsBranch.Tables[0].Rows[i]["Id"].ToString(),
                            BranchName = dsBranch.Tables[0].Rows[i]["BranchName"].ToString().ToUpper()
                        };

                        Branchlist.CompBranchList.Add(Branch);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDivisionLocationList: " + ex.ToString());
            }
            return new MultiSelectList(Branchlist.CompBranchList, "BranchId", "BranchName");
        }

      
        public MultiSelectList GetAreaTypeNameList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_fulldesc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                  + "and header_desc='Area Type' order by item_fulldesc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAreaTypeNameList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetAreaTypeNameById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_fulldesc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Area Type' and (item_id='" + item_id + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAreaTypeNameById: " + ex.ToString());
            }
            return "";
        }
             
        public MultiSelectList GetLocationListBox()
        {
            LocationModelsList location = new LocationModelsList();
            location.LocList = new List<LocationModels>();
            try
            {
                DataSet dsLoc = Getdetails("select id_location, location_name from t_location where active=1 order by location_name asc");

                if (dsLoc.Tables.Count > 0 && dsLoc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsLoc.Tables[0].Rows.Count; i++)
                    {
                        LocationModels Locations = new LocationModels()
                        {
                            id_location = dsLoc.Tables[0].Rows[i]["id_location"].ToString(),
                            location_name = dsLoc.Tables[0].Rows[i]["location_name"].ToString()
                        };

                        location.LocList.Add(Locations);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLocationListBox: " + ex.ToString());
            }
            return new MultiSelectList(location.LocList, "id_location", "location_name");
        }

        public string GetLocationNameById(string Location_id)
        {
            try
            {
                if (Location_id != "")
                {
                    string sSsqlstmt = "select location_name as Name from t_location where id_location = '" + Location_id + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLocationNameById: " + ex.ToString());
            }
            return "";
        }


        public string GetLocationIdbyName(string location_name)
        {
            try
            {
                if (location_name != "")
                {
                    string sSsqlstmt = "select id_location as Id from t_location where location_name = '" + location_name + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLocationIdbyName: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetLocationListWithCountrynAreaId(string Country_Id, string Area_Id)
        {
            LocationModelsList location = new LocationModelsList();
            location.LocList = new List<LocationModels>();
            try
            {
                DataSet dsLoc = Getdetails("select id_location, location_name from t_location where id_country = '" + Country_Id + "' and area_type = '" + Area_Id + "' and active=1 order by id_country asc");

                if (dsLoc.Tables.Count > 0 && dsLoc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsLoc.Tables[0].Rows.Count; i++)
                    {
                        LocationModels Locations = new LocationModels()
                        {
                            id_location = dsLoc.Tables[0].Rows[i]["id_location"].ToString(),
                            location_name = dsLoc.Tables[0].Rows[i]["location_name"].ToString()
                        };

                        location.LocList.Add(Locations);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLocationListWithCountrynAreaId: " + ex.ToString());
            }
            return new MultiSelectList(location.LocList, "id_location", "location_name");
        }

        public MultiSelectList GetLocationListWithCountryId(string Country_Id)
        {
            LocationModelsList location = new LocationModelsList();
            location.LocList = new List<LocationModels>();
            try
            {
                DataSet dsLoc = Getdetails("select id_location, location_name from t_location where id_country = '" + Country_Id + "' and active=1 order by id_country asc");

                if (dsLoc.Tables.Count > 0 && dsLoc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsLoc.Tables[0].Rows.Count; i++)
                    {
                        LocationModels Locations = new LocationModels()
                        {
                            id_location = dsLoc.Tables[0].Rows[i]["id_location"].ToString(),
                            location_name = dsLoc.Tables[0].Rows[i]["location_name"].ToString()
                        };

                        location.LocList.Add(Locations);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLocationListWithCountryId: " + ex.ToString());
            }
            return new MultiSelectList(location.LocList, "id_location", "location_name");
        }

        public MultiSelectList GetCountryListbox()
        {
            LocationModelsList location = new LocationModelsList();
            location.LocList = new List<LocationModels>();
            try
            {
                DataSet dsLoc = Getdetails("select id_country, country_name from t_country order by country_name asc");

                if (dsLoc.Tables.Count > 0 && dsLoc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsLoc.Tables[0].Rows.Count; i++)
                    {
                        LocationModels Locations = new LocationModels()
                        {
                            id_country = dsLoc.Tables[0].Rows[i]["id_country"].ToString(),
                            country_name = dsLoc.Tables[0].Rows[i]["country_name"].ToString()
                        };

                        location.LocList.Add(Locations);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCountryListbox: " + ex.ToString());
            }
            return new MultiSelectList(location.LocList, "id_country", "country_name");
        }

        internal DataSet GetReportDetails(DataSet dsData, string report_no, string empid, string report)
        {
            DataSet dsDetails = new DataSet();
            try
            {
                //dsDetails = Getdetails("select  case  when  item_desc is null then  deptname else concat(item_desc, '/', ifnull(deptname, '')) end as division, BranchName"
                //+ " from t_company_branch a left outer join t_hr_employee b on a.id = b.emp_work_location left outer join t_departments c"
                //+ " on c.deptid = b.dept_id  left outer join dropdownitems d on item_id = division where b.emp_no = '" + empid + "'");

                dsDetails = Getdetails("select  BranchName as division,deptName,location_name,emp_firstname from t_company_branch a" +
                    " left outer join t_hr_employee b on a.id = b.division left outer join t_departments c on c.deptid = b.dept_id " +
                    "  left outer join t_location  d on b.emp_work_location = d.id_location  where b.emp_no  = '" + empid + "'");

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    dsData.Tables[0].Columns.Add("LoggedName", System.Type.GetType("System.String"));
                    dsData.Tables[0].Columns.Add("Loggeddivision", System.Type.GetType("System.String"));
                    dsData.Tables[0].Columns.Add("Loggeddept", System.Type.GetType("System.String"));
                    dsData.Tables[0].Columns.Add("LoggedLocation", System.Type.GetType("System.String"));
                    dsData.Tables[0].Columns.Add("report", System.Type.GetType("System.String"));
                    dsData.Tables[0].Columns.Add("sreportno", System.Type.GetType("System.String"));

                    dsData.Tables[0].Rows[0]["LoggedName"] = dsDetails.Tables[0].Rows[0]["emp_firstname"].ToString();
                    dsData.Tables[0].Rows[0]["Loggeddivision"] = dsDetails.Tables[0].Rows[0]["division"].ToString();
                    dsData.Tables[0].Rows[0]["Loggeddept"] = dsDetails.Tables[0].Rows[0]["deptName"].ToString();
                    dsData.Tables[0].Rows[0]["LoggedLocation"] = dsDetails.Tables[0].Rows[0]["location_name"].ToString();
                    dsData.Tables[0].Rows[0]["report"] = report;
                    dsData.Tables[0].Rows[0]["sreportno"] = report_no;
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetReportDetails: " + ex.ToString());
            }
            return dsData;
        }

        public bool SendEmployeemail(string sToEmailid, System.IO.FileStream[] fsSource, string sSubject, string sBody, string sCCList = "", string sBccList = "")
        {
            try
            {
                string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"].ToString().Trim();//"mail.almanalmgt.com";
                int portNumber = 0;

                if (int.TryParse(ConfigurationManager.AppSettings["portNumber"].ToString().Trim(), out portNumber))
                {

                }//587;
                //bool enableSSL = false;
                bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"].ToString().Trim());

                string sLoggedEmail = "";

                sLoggedEmail = GetHrEmpEmailIdById(GetCurrentUserSession().empid);

                string emailFrom = ConfigurationManager.AppSettings["emailFrom"].ToString().Trim(); //"msmesupport@almanalmgt.com";//
                string password = ConfigurationManager.AppSettings["Pwd"].ToString().Trim(); //"msme@123";//


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    if (sToEmailid != "")
                    {
                        mail.To.Add(sToEmailid);
                    }
                    else
                    {
                        mail.To.Add(sLoggedEmail);
                    }

                    mail.Subject = sSubject;
                    mail.Body = sBody;
                    mail.IsBodyHtml = true;
                    if (sCCList != "" && sCCList != null)
                    {
                        mail.CC.Add(sCCList);
                    }

                    if (sBccList != "" && sBccList != null)
                    {
                        mail.Bcc.Add(sBccList);
                    }

                    foreach (var sFile in fsSource)
                    {
                        if (sFile != null)
                        {
                            mail.Attachments.Add(new Attachment(sFile, sFile.Name, MediaTypeNames.Application.Octet));
                        }

                    }

                    SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);

                    smtp.Credentials = new System.Net.NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    //smtp.Send(mail);

                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable)
                            {
                                AddFunctionalLog("Delivery failed - retrying in 5 seconds.");
                                System.Threading.Thread.Sleep(5000);
                                smtp.Send(mail);
                            }
                            else
                            {
                                AddFunctionalLog("Failed to deliver message to {0}",
                                    ex.InnerExceptions[i].FailedRecipient);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddFunctionalLog("Exception caught in RetryIfBusy(): {0}",
                                ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in SendJDmail: " + ex.ToString());
            }
            return true;
        }

        public string GetEmpIDByUsername(string sUsername, string sPwd)
        {
            try
            {
                if (sUsername != "")
                {
                    DataSet dsEmp = Getdetails("select EmpID from t_employee where emailAddress='" + sUsername + "' and Pwd='" + sPwd + "' and active=1");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["EmpID"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetEmpIDByUsername: " + ex.ToString());
            }
            return "";
        }

        public DataSet LoginPaswdExpAuth(string sUsername, string sPwd)
        {

            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("login_pswdexpiryauth", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@vEmailid", sUsername);
                cmd.Parameters.AddWithValue("@pass", sPwd);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in LoginPaswdExpAuth: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public DataSet GetHSEMonthlyData(string vyear, string vmonth)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_HSE_monthly_Perf_Report", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@years", vyear);
                cmd.Parameters.AddWithValue("@months", vmonth);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHSEMonthlyData: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
              
        public string GetHRManagerEmployee() // ROLE BASED
        {
            try
            {

                DataSet dsData = Getdetails("select group_concat(emp_no) emp_no from t_hr_employee e,roles r where e.emp_status=1"
                + " and FIND_IN_SET(id,Role) and RoleName='HR Manager'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["emp_no"].ToString());
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHRManagerEmployee: " + ex.ToString());
            }
            return "";
        }

        public bool SendJDmail(string sToEmailid, string sSubject, string sBody, HttpPostedFileBase sFilename, string sCCList = "", string sBccList = "")
        {
            try
            {
                string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"].ToString().Trim();//"mail.almanalmgt.com";
                int portNumber = 0;

                if (int.TryParse(ConfigurationManager.AppSettings["portNumber"].ToString().Trim(), out portNumber))
                {

                }//587;
                //bool enableSSL = false;
                bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"].ToString().Trim());

                string sLoggedEmail = "";

                sLoggedEmail = GetHrEmpEmailIdById(GetCurrentUserSession().empid);

                string emailFrom = ConfigurationManager.AppSettings["emailFrom"].ToString().Trim(); //"msmesupport@almanalmgt.com";//
                string password = ConfigurationManager.AppSettings["Pwd"].ToString().Trim(); //"msme@123";//

                //string smtpAddress = "mailv.emirates.net.ae";
                //int portNumber = 25;
                //bool enableSSL = false;

                //string emailFrom = Properties.Settings.Default.EmailId.Trim();
                //string password = "flexiflo2017";//Properties.Settings.Default.Pwd.Trim();


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    if (sToEmailid != "")
                    {
                        mail.To.Add(sToEmailid);
                    }
                    else
                    {
                        mail.To.Add(sLoggedEmail);
                    }

                    mail.Subject = sSubject;
                    mail.Body = sBody;
                    mail.IsBodyHtml = true;
                    if (sCCList != "")
                    {
                        mail.CC.Add(sCCList);
                    }

                    if (sBccList != "")
                    {
                        mail.Bcc.Add(sBccList);
                    }

                    // Can set to false, if you are sending pure text.

                    //if (sFilename != null)
                    //{
                    //    string fileName = Path.GetFileName(sFilename.FileName);
                    //    mail.Attachments.Add(new Attachment(sFilename.InputStream, fileName));
                    //}

                    if (sFilename != null)
                    {

                        string fileName = Path.GetFileName(sFilename.FileName);
                        mail.Attachments.Add(new Attachment(sFilename.InputStream, fileName));

                    }
                    SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);

                    smtp.Credentials = new System.Net.NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    //smtp.Send(mail);

                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable)
                            {
                                AddFunctionalLog("Delivery failed - retrying in 5 seconds.");
                                System.Threading.Thread.Sleep(5000);
                                smtp.Send(mail);
                            }
                            else
                            {
                                AddFunctionalLog("Failed to deliver message to {0}",
                                    ex.InnerExceptions[i].FailedRecipient);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddFunctionalLog("Exception caught in RetryIfBusy(): {0}",
                                ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in SendJDmail: " + ex.ToString());
            }
            return true;
        }

        public MultiSelectList GetRolesForJD(string role_id)
        {
            EmployeeRolesList Roleslist = new EmployeeRolesList();
            Roleslist.EmpRolesList = new List<EmployeeRoles>();

            try
            {
                if (role_id != null && role_id != "")
                {
                    DataSet dsRoles = Getdetails("select Id, RoleName from Roles where active=1 and"
                    + "'" + role_id + "' not in (Id) order by RoleName asc");
                    if (dsRoles.Tables.Count > 0 && dsRoles.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsRoles.Tables[0].Rows.Count; i++)
                        {
                            EmployeeRoles emp = new EmployeeRoles()
                            {
                                id = dsRoles.Tables[0].Rows[i]["Id"].ToString(),
                                Rolename = dsRoles.Tables[0].Rows[i]["RoleName"].ToString()
                            };

                            Roleslist.EmpRolesList.Add(emp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRolesForJD: " + ex.ToString());
            }
            return new MultiSelectList(Roleslist.EmpRolesList, "Id", "RoleName");

        }

        public DataSet GetMRMData(string vyear)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_MRM_new", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@vyear", vyear);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMRMData: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public MultiSelectList GetDMR()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {

                DataSet dsEmp = Getdetails("select emp_no as Empid,concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname"
                + " from t_hr_employee h, roles r where h.emp_status = 1 and FIND_IN_SET(id, Role) and RoleName = 'DMR'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDMR: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetCMR()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                DataSet dsEmp = Getdetails("select emp_no as Empid,concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname"
              + " from t_hr_employee h, roles r where h.emp_status = 1 and FIND_IN_SET(id, Role) and RoleName = 'CMR'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCMR: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }
          
        public string GetCalendarEventPriorityColorById(string item_id)
        {
            try
            {
                if (item_id != "" && item_id != null)
                {
                    DataSet dsEmp = Getdetails("Select GROUP_CONCAT(m.item_fulldesc) item_fulldesc FROM dropdownitems m,dropdownheader n where m.header_id=n.header_id"
                    + " and header_desc='Calendar Event Priority' and item_id in ('" + item_id + "')");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["item_fulldesc"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCalendarEventPriorityColorById: " + ex.ToString());
            }
            return "";
        }
         
        public MultiSelectList GetHrEmpEvaluatedByList()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                string Emp_Id = GetCurrentUserSession().empid;
                string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid from t_hr_employee where emp_status=1 and EvaluatedBy='" + Emp_Id + "' order by emp_firstname asc";

                DataSet dsEmp = Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHrEmpEvaluatedByList: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");

        }

        public string GetHrEmpEvaluatedById(string userid)
        {
            try
            {
                if (userid != "")
                {
                    string sSsqlstmt = "select EvaluatedBy as Id from t_hr_employee where emp_no='" + userid + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHrEmpEvaluatedById: " + ex.ToString());
            }
            return "";
        }

        //-----------------------------------------------14/7/2020---------------------------------------
        public string GetIncidentInvestigators(string Incident_Id)
        {
            try
            {
                if (Incident_Id != null && Incident_Id != "")
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.invest_team) invest_team FROM t_incident_report m  where Incident_Id in (" + Incident_Id + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["invest_team"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAttendees: " + ex.ToString());
            }
            return "";
        }
        
        public MultiSelectList GetHrEmpHseList()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                string Emp_Id = GetCurrentUserSession().empid;
                string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid"
                + " from t_hr_employee t,t_departments tt where emp_status = 1 and t.Dept_Id = tt.DeptId and DeptName like '%hse' or '%hse%' or 'hse%' or '%qhse' or '%qhse%' or 'qhse%'";

                DataSet dsEmp = Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHrEmpHseList: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");

        }

        public MultiSelectList GetAccidentReportsWithInvestList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_accident_rept,invest_reportno from t_accident_report where invest_need='yes'";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["id_accident_rept"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["invest_reportno"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAccidentReportsWithInvestList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
      
        public DataSet BranchLevel(string sId)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("branch_level", con);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@vId", sId);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in BranchLevel: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
                 
        public MultiSelectList GetAuditor()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                DataSet dsEmp = Getdetails("select emp_no as Empid,concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname"
             + " from t_hr_employee h, roles r where h.emp_status = 1 and FIND_IN_SET(id, Role) and RoleName like '%Auditor%'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditor: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetChecklistType()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_AuditChecklist as Id, Department as Name from t_auditchecklist where Active=1 order by id_AuditChecklist asc";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["Id"].ToString()+"-"+GetDeptNameById(dsData.Tables[0].Rows[i]["Name"].ToString())
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetChecklistType: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
                     
        public MultiSelectList GetChecklistTypeByChecklistRef()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_AuditChecklist as Id, ChecklistRef as Name from t_auditchecklist where Active=1 order by id_AuditChecklist asc";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetChecklistTypeByChecklistRef: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        
        public string GetChecklistBychecklistId(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select ChecklistRef from t_auditchecklist where id_AuditChecklist = '" + item_id + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["ChecklistRef"].ToString());
                    }
                }


            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetChecklistBychecklistId: " + ex.ToString());
            }
            return "";
        }
         
        public bool Sendmail2(string sToEmailid, string sSubject, string sBody, string sCCList = "", string sBccList = "")
        {
            try
            {
                string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"].ToString().Trim();//"mail.almanalmgt.com";
                int portNumber = 0;

                if (int.TryParse(ConfigurationManager.AppSettings["portNumber"].ToString().Trim(), out portNumber))
                {

                }//587;
                bool enableSSL = false;
                string sLoggedEmail = "";

                //sLoggedEmail = GetHrEmpEmailIdById(GetCurrentUserSession().empid);

                string emailFrom = ConfigurationManager.AppSettings["emailFrom"].ToString().Trim(); //"msmesupport@almanalmgt.com";//
                string password = ConfigurationManager.AppSettings["Pwd"].ToString().Trim(); //"msme@123";//

                //string smtpAddress = "mailv.emirates.net.ae";
                //int portNumber = 25;
                //bool enableSSL = false;

                //string emailFrom = Properties.Settings.Default.EmailId.Trim();
                //string password = "flexiflo2017";//Properties.Settings.Default.Pwd.Trim();


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    if (sToEmailid != "")
                    {
                        mail.To.Add(sToEmailid);
                    }
                    else
                    {
                        mail.To.Add(sLoggedEmail);
                    }

                    mail.Subject = sSubject;
                    mail.Body = sBody;
                    mail.IsBodyHtml = true;
                    if (sCCList != "")
                    {
                        mail.CC.Add(sCCList);
                    }

                    if (sBccList != "")
                    {
                        mail.Bcc.Add(sBccList);
                    }

                    // Can set to false, if you are sending pure text.

                    //if (sFilename != "")
                    //{
                    //    if (File.Exists(sFilename))
                    //    {
                    //        mail.Attachments.Add(new Attachment(sFilename));
                    //    }
                    //}
                    SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);

                    smtp.Credentials = new System.Net.NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in Sendmail2: " + ex.ToString());
            }
            return true;
        }

        public string GetGlobalCompanyName()
        {

            var name = "";
            try
            {
                //string sSsqlstmt = "select ProductId, ProductName from t_product where Active=1";
                string sSsqlstmt = "SELECT * FROM t_companyinfo where CompanyID=1";

                DataSet CName = Getdetails(sSsqlstmt);
                name = CName.Tables[0].Rows[0]["CompanyName"].ToString();

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetGlobalGetCompanyName: " + ex.ToString());
            }

            return name;
        }
         
        public MultiSelectList GetHRRMatrixRatewithColor()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select rate_desc,color_code from risk_ratings_hrr";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["rate_desc"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["color_code"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHRRMatrixRatewithColor: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }

        public MultiSelectList GetEnvMatrixRatewithColor()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select rate_desc,color_code from risk_ratings_env";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["rate_desc"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["color_code"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetEnvMatrixRatewithColor: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }

        public MultiSelectList GetRiskMatrixRatewithColor()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select rate_desc,color_code from risk_ratings";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["rate_desc"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["color_code"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRiskMatrixRatewithColor: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }

        public string GetDropdownHeaderIdByDesc(string header_desc)
        {
            try
            {
                if (header_desc != "" && header_desc != null)
                {
                    DataSet dsData = Getdetails("select header_id from dropdownheader where header_desc='" + header_desc + "'");
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        string sheader_id = dsData.Tables[0].Rows[0]["header_id"].ToString();

                        return sheader_id;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHeaderIdByDesc: " + ex.ToString());
            }
            return "";
        }
        
        public string GetViewDeptNameById(string Deptid)
        {
            try
            {
                if (Deptid != "" && Deptid != null)
                {
                    DataSet dsData = Getdetails("SELEct  GROUP_CONCAT(m.DeptName) DeptName FROM t_views m  where DeptId in (" + Deptid + ")");
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsData.Tables[0].Rows[0]["DeptName"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetViewDeptNameById: " + ex.ToString());
            }
            return "";
        }
        
        public bool SendCustRetunmail(string sToEmailid, string sSubject, string sBody, IEnumerable<HttpPostedFileBase> sFilename, string sCCList = "", string sBccList = "")
        {
            try
            {
                string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"].ToString().Trim();//"mail.almanalmgt.com";
                int portNumber = 0;

                if (int.TryParse(ConfigurationManager.AppSettings["portNumber"].ToString().Trim(), out portNumber))
                {

                }//587;
                //bool enableSSL = false;
                bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"].ToString().Trim());

                string sLoggedEmail = "";

                sLoggedEmail = GetHrEmpEmailIdById(GetCurrentUserSession().empid);

                string emailFrom = ConfigurationManager.AppSettings["emailFrom"].ToString().Trim(); //"msmesupport@almanalmgt.com";//
                string password = ConfigurationManager.AppSettings["Pwd"].ToString().Trim(); //"msme@123";//

                //string smtpAddress = "mailv.emirates.net.ae";
                //int portNumber = 25;
                //bool enableSSL = false;

                //string emailFrom = Properties.Settings.Default.EmailId.Trim();
                //string password = "flexiflo2017";//Properties.Settings.Default.Pwd.Trim();


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    if (sToEmailid != "")
                    {
                        mail.To.Add(sToEmailid);
                    }
                    else
                    {
                        mail.To.Add(sLoggedEmail);
                    }

                    mail.Subject = sSubject;
                    mail.Body = sBody;
                    mail.IsBodyHtml = true;
                    if (sCCList != "")
                    {
                        mail.CC.Add(sCCList);
                    }

                    if (sBccList != "")
                    {
                        mail.Bcc.Add(sBccList);
                    }

                    // Can set to false, if you are sending pure text.

                    //if (sFilename != null)
                    //{
                    //    string fileName = Path.GetFileName(sFilename.FileName);
                    //    mail.Attachments.Add(new Attachment(sFilename.InputStream, fileName));
                    //}

                    if (sFilename != null)
                    {
                        foreach (var file in sFilename)
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            mail.Attachments.Add(new Attachment(file.InputStream, fileName));
                        }
                    }
                    SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);

                    smtp.Credentials = new System.Net.NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    //smtp.Send(mail);

                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable)
                            {
                                AddFunctionalLog("Delivery failed - retrying in 5 seconds.");
                                System.Threading.Thread.Sleep(5000);
                                smtp.Send(mail);
                            }
                            else
                            {
                                AddFunctionalLog("Failed to deliver message to {0}",
                                    ex.InnerExceptions[i].FailedRecipient);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddFunctionalLog("Exception caught in RetryIfBusy(): {0}",
                                ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in Sendmail: " + ex.ToString());
            }
            return true;
        }
        
        public MultiSelectList GetImprovementSeverityList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select rate_id,rate_desc from risk_ratings order by rate_id asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["rate_id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["rate_desc"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetImprovementSeverityList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }

        public string GetImprovementSeverityById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select rate_desc from risk_ratings where rate_id='" + item_id + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["rate_desc"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiImprovementSeverityById: " + ex.ToString());
            }
            return "";
        }
                       
        public DataSet GetDecendants(string EmpNo, string BranchId)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("get_descendents", con);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@vempno", EmpNo);
                cmd.Parameters.AddWithValue("@vbranch", BranchId);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDecendants: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        
        public bool UpdateAccesdetails(string role_id)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("update_access", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@vrole_id", role_id);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in UpdateAccesdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return true;
        }

        public bool UpdateBranchdetails(string role_id)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("update_accessbranchtree", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@vrole_id", role_id);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in UpdateBranchdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return true;
        }

        public DataSet GetKPIReportdetails(string dtFromDate, string dtToDate)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_kpi_rpt", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@vFromperiod", dtFromDate);
                cmd.Parameters.AddWithValue("@vToperiod", dtToDate);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetKPIReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public bool SendmailonlyBCC(string sBccList, string sSubject, string sBody, string sFilename, string sCCList = "", string sToEmailid = "")
        {
            try
            {
                string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"].ToString().Trim();//"mail.almanalmgt.com";
                int portNumber = 0;

                if (int.TryParse(ConfigurationManager.AppSettings["portNumber"].ToString().Trim(), out portNumber))
                {

                }//587;

                bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"].ToString().Trim());
                string sLoggedEmail = "";

                sLoggedEmail = GetHrEmpEmailIdById(GetCurrentUserSession().empid);

                string emailFrom = ConfigurationManager.AppSettings["emailFrom"].ToString().Trim(); //"msmesupport@almanalmgt.com";//
                string password = ConfigurationManager.AppSettings["Pwd"].ToString().Trim(); //"msme@123";//

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    if (sBccList != "")
                    {
                        mail.Bcc.Add(sBccList);
                    }
                    else
                    {
                        mail.Bcc.Add(sLoggedEmail);
                    }

                    mail.Subject = sSubject;
                    mail.Body = sBody;
                    mail.IsBodyHtml = true;
                    if (sCCList != "")
                    {
                        mail.CC.Add(sCCList);
                    }

                    if (sToEmailid != "")
                    {
                        mail.To.Add(sToEmailid);
                    }

                    // Can set to false, if you are sending pure text.

                    if (sFilename != "")
                    {
                        if (File.Exists(sFilename))
                        {
                            mail.Attachments.Add(new Attachment(sFilename));
                        }
                    }
                    SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);

                    smtp.Credentials = new System.Net.NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in SendmailonlyBCC: " + ex.ToString());
            }
            return true;

        }

        public string GetCompanyBranchNameById(string Branchid)
        {
            try
            {
                if (Branchid != "")
                {
                    DataSet dsComp = Getdetails("select branchname from t_company_branch where id='" + Branchid + "' or branchname='" + Branchid + "' order by branchname asc");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["branchname"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCompanyBranchNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetCompanyBranchIDByName(string BranchName)
        {
            try
            {
                if (BranchName != "")
                {
                    DataSet dsComp = Getdetails("select id from t_company_branch where BranchName='" + BranchName + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCompanyBranchIDByName: " + ex.ToString());
            }
            return "";
        }

        public string GetMultiCompanyBranchNameById(string Branchid)
        {
            try
            {
                if (Branchid != "" && Branchid != null)
                {
                    DataSet dsComp = Getdetails("select group_concat(branchname) as branchname from t_company_branch where id in (" + Branchid + ") order by branchname asc");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        string Desc = (dsComp.Tables[0].Rows[0]["branchname"].ToString());
                        if (Desc != "" && Desc.Contains(','))
                        {
                            Desc.Replace(",", ", ");
                        }
                        return Desc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiCompanyBranchNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiBranchListByID(string sBranchtree)
        {
            CompanyBranchList Branchlist = new CompanyBranchList();
            Branchlist.CompBranchList = new List<CompanyBranch>();
            try
            {
                sBranchtree = sBranchtree.Trim().TrimStart(',').TrimEnd(',');
                sBranchtree = Regex.Replace(sBranchtree, @",+", ",");
                //DataSet dsBranch = Getdetails("select id, branchname from t_company_branch where Active=1 and id in (" + sBranchtree + ")");// and CompanyId='" + sCompanyId+"'");
                string sql= "select id, branchname from t_company_branch where Active=1 and find_in_set(id,'" + sBranchtree + "')";
                DataSet dsBranch = Getdetails(sql);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        CompanyBranch Branch = new CompanyBranch()
                        {
                            BranchId = dsBranch.Tables[0].Rows[i]["Id"].ToString(),
                            BranchName = dsBranch.Tables[0].Rows[i]["BranchName"].ToString().ToUpper()
                        };

                        Branchlist.CompBranchList.Add(Branch);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiBranchListByID: " + ex.ToString());
            }
            return new MultiSelectList(Branchlist.CompBranchList, "BranchId", "BranchName");
        }

        public MultiSelectList GetMultiLocationListByID(string sLocationtree)
        {
            CompanyBranchList Branchlist = new CompanyBranchList();
            Branchlist.CompBranchList = new List<CompanyBranch>();
            try
            {
                sLocationtree = sLocationtree.Trim().TrimStart(',').TrimEnd(',');
                sLocationtree = Regex.Replace(sLocationtree, @",+", ",");
                //DataSet dsBranch = Getdetails("select id, branchname from t_company_branch where Active=1 and id in (" + sBranchtree + ")");// and CompanyId='" + sCompanyId+"'");
                string sql = "select id_location, location_name from t_location where Active=1 and find_in_set(id_location,'" + sLocationtree + "')";
                DataSet dsBranch = Getdetails(sql);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        CompanyBranch Branch = new CompanyBranch()
                        {
                            BranchId = dsBranch.Tables[0].Rows[i]["id_location"].ToString(),
                            BranchName = dsBranch.Tables[0].Rows[i]["location_name"].ToString().ToUpper()
                        };

                        Branchlist.CompBranchList.Add(Branch);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiLocationListByID: " + ex.ToString());
            }
            return new MultiSelectList(Branchlist.CompBranchList, "BranchId", "BranchName");
        }

        //-------------------------------Start of Pending For Review---------------------------------------------------


        //==========================SYSTEM DOCUMENTS======================================
        public DataSet getListPendingForReviewSystemDocuments(string sempid)
        {

            //string sSqlstmt = "select * from t_mgmt_documents where Status= 1 and Approved_Status=0 and Reviewed_Status=0 and  find_in_set('" + sempid + "',ReviewedBy)"
            //       + " and find_in_set('" + sempid + "',ReviewedBy) ";
            //sSqlstmt = sSqlstmt + " order by DocLevels, idmgmt desc";
            string sSqlstmt = "select * from t_mgmt_documents where Status= 1 and Approved_Status=0 and Reviewed_Status=0 and" +
                                 " ( find_in_set('" + sempid + "',ReviewedBy) and not find_in_set('" + sempid + "',Reviewers)) "
                                + "and ( find_in_set('" + sempid + "',ReviewedBy) and not find_in_set('" + sempid + "',ReviewRejector))";
            sSqlstmt = sSqlstmt + " order by DocLevels, idmgmt desc";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }


            return null;
        }

        //==========================LEGAL REGISTER======================================
                          
        public DataSet getListPendingForReviewLegalRegister(string sempid)
        {

            string sSqlstmt = "select LegalRequirement_Id, lawNo, lawTitle, initialdevelopmentdate, origin_of_requirement , document_storage_location,frequency_of_evaluation, activeStatus, updatedOn,"
                 + "  reviewedBy,approvedBy,updatedByName from t_legalregister where approvestatus=0 and reviewstatus=0 and reviewedBy='" + sempid + "'";
            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }


            return null;
        }

         //==========================RISK REGISTER ACTIVITY(ENVIRONMENT)======================================

        public DataSet getListPendingForReviewRiskEnv(string sempid)
        {

            string sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence, Cur_Opt_Ctrl, Severity, "
                          + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                          + " DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, Comments,Exposure_id,Appl_law from t_risk_register_activity_eval as trEval, "
                          + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id"
                          + " and Review_status=0 and tract.Active=1 and trEval.Active=1 and Reviewer_QHSE ='" + sempid + "'";
            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================RISK REGISTER ACTIVITY(HRR)======================================
        
        public DataSet getListPendingForReviewRiskHRR(string sempid)
        {

            string sSqlstmt = "select risk_hrr_id,Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id,Evaluation_status,"
               + "Cur_Opt_Ctrl,Person_resp,Eval_Date,ReEval_Date,Action_TakenBy,Reviewer_QHSE,ApprovedBy from t_risk_register_hrrevaluation"
               + " where Active=1 and Review_status=0 and Reviewer_QHSE ='" + sempid + "'";
            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }


            return null;
        }

            //==========================RISK REGISTER ACTIVITY(HRR)======================================

         public DataSet getListPendingForReviewSuppReevaluation(string sempid)
            {
                string sSqlstmt = "select id_reevaluation, branch, supplier, logged_by, perf_review_year, recommanded,isrecommand, recommand_date, approved_to,isapproved,approved_date from t_supplier_reevaluation"
                    + " where isrecommand =0 and Active=1 and recommanded_to ='" + sempid + "'";

              DataSet dsApprovalList = Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    return dsApprovalList;
                }
                return null;
            }

            //-------------------------------End of Pending For Review---------------------------------------------------

            //-------------------------------Start of Pending For Approval---------------------------------------------------
            //==========================SYSTEM DOCUMENTS======================================
            public DataSet getListPendingForApprovalSystemDocuments(string sempid)
        {

            //string sSqlstmt = "select * from t_mgmt_documents where Status= 1 and Approved_Status=0 and Reviewed_Status=1 and" +
            //               " find_in_set('" + sempid + "',ApprovedBy) "
            //              + "and find_in_set('" + sempid + "',ApprovedBy) ";
            string sSqlstmt = "select * from t_mgmt_documents where Status= 1 and Approved_Status=0 and Reviewed_Status=1 and" +
                             " ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Approvers)) "
                            + "and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',ApprovalRejector))";
            sSqlstmt = sSqlstmt + " order by DocLevels, idmgmt desc";

            DataSet dsApprovalList = Getdetails(sSqlstmt);

            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
                     
        //==========================ANNEXURE======================================
        public DataSet getListPendingForApprovalANNEXURE(string sempid)
        {
            string sSqlstmt = "select idAnnexure,MgmtId,DocRef,DocName,IssueNo,RevNo,PreparedBy,ApprovedBy,DocDate,DocUploadPath"
                    + " from t_mgmt_annexure where Status=1 and ApprovedStatus=0 and ( find_in_set('" + sempid + "',ApprovedBy))";

            sSqlstmt = sSqlstmt + " order by idAnnexure desc";
            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
              
        
        //==========================TRAINING======================================

        public DataSet getListPendingForApprovalTRAINING(string sempid)
        {
            //    string sSqlstmt = "SELECT TrainingID, Attendees, DeptId, Training_Topic, Traning_BeforeDate, Training_Requested_By, Reasonfor_Training, Training_Status, RequestStatus, "
            //                + " ApprovedBy from t_trainings where Active=1 and RequestStatus='0' and ApprovedBy='" + sempid + "' order by TrainingID desc";

            string sSqlstmt = "SELECT TrainingID, Attendees, DeptId, Training_Topic, Training_Start_Date, Expected_Date_Completion,Training_Requested_By, Reasonfor_Training, RequestStatus, "
                          + " ApprovedBy from t_trainings where Active=1 and RequestStatus='0' and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Approvers))"
                          + "and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',ApprovalRejector)) order by TrainingID desc";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================OBJECTIVES======================================
        public DataSet getListPendingForApprovalOBJECTIVES(string sempid)
        {
            //string sSqlstmt = "select Objectives_Id, Obj_Ref, Dept, Freq_of_Eval, Personal_Responsible, Audit_Criteria, Estld_by, Approved_By,ApprovedDate, CreatedBy,DocUploadPath from t_objectives"
            //          + " where approved_status=0 and Active=1 and Approved_By ='" + sempid + "'";

            string sSqlstmt = "select ObjectivesTrans_Id,a.Objectives_Id,Obj_Ref, Dept, b.Freq_of_Eval, b.Personal_Responsible, Audit_Criteria, Estld_by, " +
                  "b.Approved_By,b.ApprovedDate, CreatedBy,DocUploadPath from t_objectives a, t_objectives_trans b" +
                  " where b.approved_status = 0 and Active = 1 and trans_active=1 and a.Objectives_Id = b.Objectives_Id  and b.Approved_By = '" + sempid + "'";
            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================LEGAL  REGISTER======================================
        public DataSet getListPendingForApprovalLegal(string sempid)
        {
            string sSqlstmt = "select LegalRequirement_Id, lawNo, lawTitle, initialdevelopmentdate, origin_of_requirement , document_storage_location,frequency_of_evaluation, activeStatus, updatedOn,"
                    + "  reviewedBy,approvedBy,updatedByName from t_legalregister"
                    + " where approvestatus=0 and approvedBy ='" + sempid + "'";


            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
        
        //==========================Document Revise Request======================================
        public DataSet getListPendingForApprovalDocChangeRequest(string sempid)
        {
            string sSqlstmt = "select * from t_documentchangerequest where ApproveStatus=0 and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Approvers))"
                 + " and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Rejector))";


            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================CHANGE MANAGEMENT REQUEST======================================
        public DataSet getListPendingForApprovalChangeManagementRequest(string sempid)
        {
            string sSqlstmt = "select * from t_changemanagement where ApproveStatus=0 and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Approvers))";


            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================SUPPLIER======================================
        public DataSet getListPendingForApprovalSUPPLIER(string sempid)
        {
            string sSqlstmt = "select SupplierID,SupplierCode,SupplierName,City,ContactPerson,ContactNo,Address,SupplyScope,ApprovalCriteria from t_supplier where"
                             + " Active=1 and ApprovedStatus=0 and ApprovedBy ='" + sempid + "'";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================RISK REGISTER ACTIVITY(ENVIRONMENT)======================================
        public DataSet getListPendingForApprovalRiskEnv(string sempid)
        {
            string sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence, Cur_Opt_Ctrl, Severity, "
                        + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                        + " DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, Comments,Exposure_id,Appl_law from t_risk_register_activity_eval as trEval, "
                        + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id"
                        + " and Approve_status=0 and Review_status=1 and tract.Active=1 and trEval.Active=1 and ApprovedBy ='" + sempid + "'";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
        
        //==========================RISK REGISTER ACTIVITY(HRR)======================================
        public DataSet getListPendingForApprovalRiskHRR(string sempid)
        {
            string sSqlstmt = "select risk_hrr_id,Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id,Evaluation_status,"
               + "Cur_Opt_Ctrl,Person_resp,Eval_Date,ReEval_Date,Action_TakenBy,Reviewer_QHSE,ApprovedBy from t_risk_register_hrrevaluation"
               + " where Active=1 and Approve_status=0 and Review_status=1 and ApprovedBy ='" + sempid + "'";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        ////==========================AUDIT======================================
        //public DataSet getListPendingForApprovalAudit(string sempid)
        //{
        //    string sSqlstmt = "select AuditID,AuditNum,AuditDate,AuditCriteria,AuditLocation,upload,Audit_Prepared_by,ApprovedBy from t_internal_audit where Active=1 and ApprvStatus=0 and ApprovedBy='" + sempid + "'";


        //    DataSet dsApprovalList = Getdetails(sSqlstmt);
        //    if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
        //    {
        //        return dsApprovalList;
        //    }
        //    return null;
        //}
        ////==========================AUDIT PROCESS(Internal Audit Plan)======================================
        //public DataSet GetListPendingForApprovalAuditProcessDeptHead(string sempid)
        //{
        //    string sSqlstmt = "select Audit_Id,Audit_no,AuditDate,Audit_criteria from t_audit_process where active=1 and" +
        //         " ((Audit_Status='Pending' or Audit_Status='Rescheduled') and ( find_in_set('" + sempid + "',dept_head)) and dept_head_status=0)";


        //    DataSet dsApprovalList = Getdetails(sSqlstmt);
        //    if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
        //    {
        //        return dsApprovalList;
        //    }
        //    return null;
        //}

        ////==========================AUDIT PROCESS(Internal Audit Plan)======================================
        //public DataSet getListPendingForApprovalAuditProcess(string sempid)
        //{
        //    string sSqlstmt = "select Audit_Id,Audit_no,AuditDate,Audit_criteria from t_audit_process where active=1 and" +                 
        //          " (((Audit_Status='Pending' or Audit_Status='Rescheduled') and ( find_in_set('" + sempid + "',internal_audit_team)) and audit_team_status=0 ) or" +
        //    " (dept_head_status=1 and audit_team_status=1 and (Audit_Status='Pending' or Audit_Status='Rescheduled') and ( find_in_set('" + sempid + "',Notified_To))))";


        //    DataSet dsApprovalList = Getdetails(sSqlstmt);
        //    if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
        //    {
        //        return dsApprovalList;
        //    }
        //    return null;
        //}
        
        ////==========================AUDIT NC1======================================
        //public DataSet getListPendingForApprovalAuditNCAuditee(string sempid)
        //{
        //    string sSqlstmt = "select a.Audit_Id,Audit_no,AuditDate,Audit_criteria,Category,Non_comformance,id_audit_process_perform from t_audit_process a,t_audit_process_perform b where active=1 and" +
        //          " Audit_Status='Completed' and ( find_in_set('" + sempid + "',auditee_team)) and nc_step_status=1 and a.Audit_Id=b.Audit_Id";


        //    DataSet dsApprovalList = Getdetails(sSqlstmt);
        //    if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
        //    {
        //        return dsApprovalList;
        //    }
        //    return null;
        //}

        ////==========================AUDIT NC2======================================
        //public DataSet getListPendingForApprovalAuditNCAuditor(string sempid)
        //{
        //    string sSqlstmt = "select a.Audit_Id,Audit_no,AuditDate,Audit_criteria,Category,Non_comformance,id_audit_process_perform from t_audit_process a,t_audit_process_perform b where active=1 and" +
        //          " Audit_Status='Completed' and ( find_in_set('" + sempid + "',internal_audit_team)) and nc_step_status=2 and a.Audit_Id=b.Audit_Id";


        //    DataSet dsApprovalList = Getdetails(sSqlstmt);
        //    if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
        //    {
        //        return dsApprovalList;
        //    }
        //    return null;
        //}

        //==========================WORK PERMIT======================================
        public DataSet getListPendingForApprovalAudiWorkPermit(string sempid)
        {
            string sSqlstmt = "select id_access_permit,permitno,requestor,company,contactno,area,location,description,date_issued,date_expired,approved_by"
                       + " from t_access_permit where Active=1 and approve_status=0 and approved_by ='" + sempid + "'";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================SafetyViolationLog======================================
        public DataSet getListPendingForApprovalSafetyViolation(string sempid)
        {
            try
            {
                //string sSqlstmt = "select ViolationLog_Id,Report_No,HSE_observation,UnsafeAct_ReportedBy,IssuedBy,ApprovedBy,Supervisor"
                //                       + " from t_safety_violationlog where Active=1 and Approved_Status=0 and ApprovedBy ='" + sempid + "'";
                string sSqlstmt = "select * from t_safety_violationlog where Active = 1 and Approved_Status = 0 and " +
                    "( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Approvers))" +
                    " and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',ApprovalRejector)) " +
                    "order by ViolationLog_Id, Reported_On desc";

                DataSet dsApprovalList = Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    return dsApprovalList;
                }
            }
            catch (Exception ex) {
                AddFunctionalLog("Exception in getListPendingForApprovalSafetyViolation: " + ex.ToString());
            }
            return null;
        }

        //==========================JD======================================
        public DataSet getListPendingForApprovalJD(string sempid)
        {
            string sSqlstmt = "select id_role_jd,role_id,deptid,report_to,supervises,jd_date,logged_by,jd_report,approved_by"
                       + " from t_role_jd where approve_status=0 and approved_by ='" + sempid + "'";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================Supplier Reevalution======================================

        public DataSet getListPendingForApprovalSuppReevalution(string sempid)
            {
             string sSqlstmt = "select id_reevaluation, branch, supplier, logged_by, perf_review_year, recommanded,isrecommand, recommand_date, approved_to,isapproved,approved_date from t_supplier_reevaluation"
                    + " where isapproved=0 and isrecommand =1 and Active=1 and approved_to ='" + sempid + "'";

                DataSet dsApprovalList = Getdetails(sSqlstmt);
                               
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    return dsApprovalList;
                }
                return null;
            }
        //=========================Document Create Request======================================

        public DataSet getListPendingForApprovalDocCreateRequest(string sempid)
        {
            string sSqlstmt = "select id_doc_request,dcr_no,date_request,division,`department`,reason,upload,checkedby,doc_status as doc_statusId,logged_by," +
                      "case when doc_status = '0' then 'Pending for Department Head.' when doc_status = '1' then 'Pending for Assistant Manager QHSE' when doc_status = '2' then 'Approved' when doc_status = '3' then 'Rejected' end  as doc_status " +
                       " from t_document_create_request where Active=1 and (doc_status = 0 and find_in_set('" + sempid + "',checkedby)) or (doc_status = 1 and find_in_set('" + sempid + "',doc_control))";

            DataSet dsApprovalList = Getdetails(sSqlstmt);

            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================Portal Authorization======================================

        public DataSet getListPendingForApprovalPortalAuthorization(string sempid)
        {
            string sSqlstmt = "select id_authorization,access_no,request_date,access_date,valid_date,portal_category," +
                 "id_portal_master,upload,nominated_emp,justification,recommended_by,requested_by,approve_chairman,approve_vp,approve_ceo,logged_by,division,apporved_status" +
                 " from t_portal_authorization where Active=1" 
                 + " and((apporved_status = '0' and find_in_set('" + sempid + "', recommended_by))"
                 + " or(apporved_status = '1' and find_in_set('" + sempid + "', approve_ceo))"
                 + " or(apporved_status = '2' and find_in_set('" + sempid + "', approve_vp))"
                 + " or(apporved_status = '3' and find_in_set('" + sempid + "', approve_chairman)))";


            DataSet dsApprovalList = Getdetails(sSqlstmt);

            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================OFI Approval======================================
        public DataSet getListPendingForApprovalOFIInitial(string sempid)
        {
            string sSqlstmt = "select id_ofi,ofi_no,risk_no,reported_date,approved_status,realization_approved_status,ofi_status,reportedby from t_ofi where active=1 and " +
                "((approved_status = '0' and find_in_set('" + sempid + "',approvedby)) " +
                "or approved_status = 'Approved' and realization_approved_status = '0' and find_in_set('" + sempid + "',realization_approved_by))";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
       
        //==========================OFI Realization CheckedBy======================================
        public DataSet getListPendingForApprovalOFIChecked(string sempid)
        {
            string sSqlstmt = "select id_ofi,ofi_no,risk_no,reported_date,approved_status,realization_approved_status,ofi_status,reportedby from t_ofi where active=1 and " +
                "approved_status = 'Approved' and realization_approved_status = 'Approved'  and checkedbystatus = '0' and find_in_set('" + sempid + "',checkedby)";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================NC======================================
        public DataSet getListPendingForApprovalNC(string sempid)
        {
            string sSqlstmt = "select * from t_nc where active= 1 and nc_issuedto_status=0 and" +
                             " ( find_in_set('" + sempid + "',nc_issueto) and not find_in_set('" + sempid + "',nc_issuers)) "
                            + "and ( find_in_set('" + sempid + "',nc_issueto) and not find_in_set('" + sempid + "',nc_issuer_rejector)) order by id_nc desc";
            
            DataSet dsApprovalList = Getdetails(sSqlstmt);

            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }


        //==========================AUDIT======================================
        public DataSet getListPendingForApprovalAudit(string sempid)
        {
            string sSqlstmt = "select AuditID,AuditNum,AuditDate,AuditCriteria,AuditLocation,upload,Audit_Prepared_by,ApprovedBy from t_internal_audit where Active=1 and ApprvStatus=0 and ApprovedBy='" + sempid + "' order by AuditID desc";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
        //==========================AUDIT PLAN FOR AUDITOR======================================
        public DataSet GetListPendingForApprovalAuditPlanForAuditor(string sempid)
        {
            string sSqlstmt = "select distinct Audit_no,Audit_criteria,logged_by,t.Audit_Id"
            + " from t_audit_process t, t_audit_process_plan tt, t_auditor_approval t1 where active = 1 and t.Audit_Id = tt.Audit_Id and tt.Audit_Id = t1.Audit_Id"
            + " and tt.Plan_Id = t1.Plan_Id and Approved_Status = 2 and(t1.auditor = '" + sempid + "' and t1.apprv_status = 0) order by Audit_Id desc";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
        //==========================AUDIT PLAN FOR AUDITEE======================================
        public DataSet GetListPendingForApprovalAuditPlanForAuditee(string sempid)
        {
            string sSqlstmt = "select distinct Audit_no,Audit_criteria,logged_by,t.Audit_Id"
            + " from t_audit_process t, t_audit_process_plan tt, t_auditee_approval t1 where active = 1 and t.Audit_Id = tt.Audit_Id and tt.Audit_Id = t1.Audit_Id"
            + " and tt.Plan_Id = t1.Plan_Id and Approved_Status = 2 and(t1.auditee = '" + sempid + "' and t1.apprv_status = 0) order by Audit_Id desc";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
        //==========================AUDIT PROCESS FOR APPROVEL======================================
        public DataSet getListPendingForApprovalAuditProcess(string sempid)
        {
            string sSqlstmt = "select t.Audit_Id,Audit_criteria,Audit_no from t_audit_process t,t_audit_process_approval tt"
            + " where active = 1 and t.Audit_Id = tt.Audit_Id and Approved_Status = 0  and(find_in_set('" + sempid + "', ApprovedBy)) and approver = '" + sempid + "' and apprv_status = 0 order by t.Audit_Id desc";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
        //==========================NC-AUDITEE======================================
        public DataSet getListPendingForNCAuditeeApprval(string sempid)
        {
            string sSqlstmt = "select id_nc,T3.Plan_Id,T1.Audit_Id,nc_no,nc_date,Audit_no,T1.logged_by"
            + " from t_audit_process_nc T1,t_audit_process T2, t_audit_process_plan T3 where active = 1 and T1.Audit_Id = T2.Audit_Id and T1.Plan_Id = T3.Plan_Id"
            + " and apprv_status = 0  and (find_in_set('" + sempid + "', auditee_team)) order by id_nc desc";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
        //==========================NC-AUDITOR======================================
        public DataSet getListPendingForNCAuditorApprval(string sempid)
        {
            string sSqlstmt = "select id_nc,T3.Plan_Id,T1.Audit_Id,nc_no,nc_date,Audit_no,T1.logged_by"
            + " from t_audit_process_nc T1,t_audit_process T2, t_audit_process_plan T3 where active = 1 and T1.Audit_Id = T2.Audit_Id and T1.Plan_Id = T3.Plan_Id"
            + " and apprv_status = 2  and T1.logged_by='" + sempid + "' order by id_nc desc";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
        //==========================AUDIT NC1======================================
        public DataSet getListPendingForApprovalAuditNCAuditee(string sempid)
        {
            //string sSqlstmt = "select a.Audit_Id,Audit_no,AuditDate,Audit_criteria,Category,Non_comformance,id_audit_process_perform from t_audit_process a,t_audit_process_perform b where active=1 and" +
            //      " Audit_Status='Completed' and ( find_in_set('" + sempid + "',auditee_team)) and nc_step_status=1 and a.Audit_Id=b.Audit_Id";


            //DataSet dsApprovalList = Getdetails(sSqlstmt);
            //if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            //{
            //    return dsApprovalList;
            //}
            return null;
        }
        //==========================AUDIT NC2======================================
        public DataSet getListPendingForApprovalAuditNCAuditor(string sempid)
        {
            //string sSqlstmt = "select a.Audit_Id,Audit_no,AuditDate,Audit_criteria,Category,Non_comformance,id_audit_process_perform from t_audit_process a,t_audit_process_perform b where active=1 and" +
            //      " Audit_Status='Completed' and ( find_in_set('" + sempid + "',internal_audit_team)) and nc_step_status=2 and a.Audit_Id=b.Audit_Id";


            //DataSet dsApprovalList = Getdetails(sSqlstmt);
            //if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            //{
            //    return dsApprovalList;
            //}
            return null;
        }
        //-----------------------------------KPI----------------------------------------------------
        public DataSet getListPendingForApprovalKPI(string sempid)
        {
            string sSqlstmt = "select KPI_Id,logged_by,kpi_ref_no,established_date,branch,group_name,team,process_indicator,"
                       + "kpi_level,process_monitor,pers_resp,upload,"
                       + "approved_by,kpi_status,status_reason  from t_kpi where Active=1" +
            " and ( kpiapprv_status= 0 and find_in_set('" + sempid + "',approved_by)) order by KPI_Id desc";

            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }

        //==========================Document Review Frequency======================================
        public DataSet getListPendingForDocReviewFrequency(string sempid)
        {
            string sSqlstmt = "select id_doc_review,review_date,doc_level,doc_type,frequency,criteria,approvedby,division,loggedby"
                    + " from t_document_review where active=1 and approve_status=0 and ( find_in_set('" + sempid + "',approvedby))";

            sSqlstmt = sSqlstmt + " order by id_doc_review desc";
            DataSet dsApprovalList = Getdetails(sSqlstmt);
            if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
            {
                return dsApprovalList;
            }
            return null;
        }
        //-------------------------------End of Pending For Approval---------------------------------------------------

        public DataSet GetPermitNo(string smod, string sPermitType, string sLocation)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Get_next_no", con);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@vmodname", smod);
                cmd.Parameters.AddWithValue("@vpermittype", sPermitType);
                cmd.Parameters.AddWithValue("@vlocation", sLocation);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetPermitNo: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public DataSet GetPermitRating()
        {

            DataSet dsRating = new DataSet();
            try
            {
                dsRating = Getdetails("select id_ratings, Options from t_permittratings order by id_ratings");
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetPermitRating: " + ex.ToString());
            }
            return dsRating;//new MultiSelectList(lstSurvey.lstSurveyRating, "SQ_OptionsId", "RatingOptions");
        }

        public MultiSelectList getWorkPermitTypeList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_permit_type as Id,permit_type as Name from t_permit_type";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getWorkPermitTypeList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }

        public MultiSelectList getPermitQuestionList(string permit_type)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_questions as Id,question as Name from t_work_permit_questions where permit_id='" + permit_type + "' ";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getPermitQuestionList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public MultiSelectList getPermitSectionQuestionList(string permit_type, string section)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_questions as Id,question as Name from t_work_permit_questions where permit_id='" + permit_type + "' and section='" + section + "' ";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getPermitSectionQuestionList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        //--------------------------------------------------EMMET--------------------------------------------------------
        public DataSet GetYearlyStockReportdetails(string dtFromDate, string dtToDate)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("generate_stock_report", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@frdate", dtFromDate);
                cmd.Parameters.AddWithValue("@todate", dtToDate);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetYearlyStockReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        public DataSet PurchaseStockReportdetails(string dtFromDate, string dtToDate)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("generate_stock_issue_report", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@frdate", dtFromDate);
                cmd.Parameters.AddWithValue("@todate", dtToDate);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in PurchaseStockReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        public DataSet SupplierAnalysisReportdetails(string dtFromDate, string dtToDate,string supplier)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("generate_supplier_analysis", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@frdate", dtFromDate);
                cmd.Parameters.AddWithValue("@todate", dtToDate);
                cmd.Parameters.AddWithValue("@suppnames", supplier);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in SupplierAnalysisReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        public DataSet UsageReportdetails(string dtFromDate, string dtToDate)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("generate_stock_usagereport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@frdate", dtFromDate);
                cmd.Parameters.AddWithValue("@todate", dtToDate);
             

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in UsageReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        public DataSet OutOfStockReportdetails(string dtFromDate, string dtToDate)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("generate_minstock_report", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@frdate", dtFromDate);
                cmd.Parameters.AddWithValue("@todate", dtToDate);


                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in OutOfStockReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        public DataSet PriceComparisonReportdetails(string dtFromDate, string dtToDate, string supplier,string material)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("generate_pricecomp_report", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@frdate", dtFromDate);
                cmd.Parameters.AddWithValue("@todate", dtToDate);
                cmd.Parameters.AddWithValue("@suppid", supplier);
                cmd.Parameters.AddWithValue("@mat_id", material);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in PriceComparisonReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        //---------------------------------------------------HOME PAGE---------------------------------------------------
        public DataSet GetDashboard()
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_QHSE_dashboard", con);
                cmd.CommandType = CommandType.StoredProcedure;
               
                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDashboard: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        public MultiSelectList GetNearMissDashboardData()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                DataSet dsData = GetDashboard();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems chart = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["cnt"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["mname"].ToString()
                        };

                        objReportList.lstDropdown.Add(chart);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDashboardData: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public MultiSelectList GetMockDrillDashboardData()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                DataSet dsData = GetDashboard();

                if (dsData.Tables.Count > 0 && dsData.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[1].Rows.Count; i++)
                    {
                        DropdownItems chart = new DropdownItems()
                        {
                            Id = dsData.Tables[1].Rows[i]["cnt"].ToString(),
                            Name = dsData.Tables[1].Rows[i]["mname"].ToString()
                        };

                        objReportList.lstDropdown.Add(chart);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMockDrillDashboardData: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public MultiSelectList GetToolboxDashboardData()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                DataSet dsData = GetDashboard();

                if (dsData.Tables.Count > 0 && dsData.Tables[2].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[2].Rows.Count; i++)
                    {
                        DropdownItems chart = new DropdownItems()
                        {
                            Id = dsData.Tables[2].Rows[i]["cnt"].ToString(),
                            Name = dsData.Tables[2].Rows[i]["mname"].ToString()
                        };

                        objReportList.lstDropdown.Add(chart);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetToolboxDashboardData: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        //-------------------------LEAVE MGMT----------------------------------------------
        internal string FunGetWeekend()
        {
            try
            {
                DataSet dsData = Getdetails("select  group_concat(item_desc) as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id and header_desc='Weekend' order by item_desc asc");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in FunGetWeekend: " + ex.ToString());
            }
            return "";
        }

        public DataSet generateAnnualLeave(string syear)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("generate_annual_leave", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Vyear", syear);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in generateAnnualLeave: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }


        public MultiSelectList getLeaveTypeList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_leave_type as Id,type as Name from t_leave_type";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getLeaveTypeList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public MultiSelectList getLeaveOperationList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_leave_op as Id,type as Name from t_leave_op";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getLeaveOperationList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        //---------------------------------------------------------------------------------

        //------------------------EMMET------------------------------------------------
           
        public MultiSelectList GetAuditNumList()
        {
            AuditNumList list = new AuditNumList();
            list.AuditList = new List<AuditNumber>();
            try
            {
                DataSet dsAudit = Getdetails("select distinct a.AuditNum,t.AuditID from t_auditfindings a,t_internal_audit t,t_internal_audit_trans tt"
                +" where t.AuditID=tt.AuditID and tt.AuditTransID=a.AuditTransID and active=1");

                if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsAudit.Tables[0].Rows.Count; i++)
                    {
                        AuditNumber emp = new AuditNumber()
                        {
                            AuditID = dsAudit.Tables[0].Rows[i]["AuditID"].ToString(),
                            AuditNum = dsAudit.Tables[0].Rows[i]["AuditNum"].ToString()
                        };

                        list.AuditList.Add(emp);

                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditNumList: " + ex.ToString());
            }
            return new MultiSelectList(list.AuditList, "AuditID", "AuditNum");
        }
     
        public MultiSelectList getRoundbarLengthList(string material)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_stock as Id,length as Name from t_stock_master where material='" + material + "'";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getRoundbarLengthList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public MultiSelectList getHeatNoList(string material)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select distinct heatno as Name,material as Id from t_roundbar_trans where material='" + material + "'";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getHeatNoList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public MultiSelectList getChemicalExpiryList(string material)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {

               
                DateTime dateValue = new DateTime();

                
                string sSsqlstmt = "select id_stock as Id,expiry_date as Name from t_stock_master where material='" + material + "'";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            //Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                           
                        };
                        if (dsData.Tables[0].Rows[i]["Name"].ToString() != null && DateTime.TryParse(dsData.Tables[0].Rows[i]["Name"].ToString(), out dateValue) == true)
                        {
                            objReport.Name = dateValue.ToString("dd/MM/yyyy");
                        }
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getChemicalExpiryList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetExpDateById(string expiry_date)
        {
            try
            {
                DateTime dateValue = new DateTime();
                if (expiry_date != "")
                {
                    DataSet dsData = Getdetails("select expiry_date from t_stock_master where id_stock='" + expiry_date + "'");
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        if (dsData.Tables[0].Rows[0]["expiry_date"].ToString() != null && DateTime.TryParse(dsData.Tables[0].Rows[0]["expiry_date"].ToString(), out dateValue) == true)
                        {
                            return dateValue.ToString("yyyy-MM-dd");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetExpDateById: " + ex.ToString());
            }
            return "";
        }

        public string GetStockQtyByLength(string id_stock)
        {
            try
            {
                if (id_stock != "")
                {
                    DataSet dsEmp = Getdetails("select (qty-dummy_bal) as qty from t_stock_master where id_stock='" + id_stock + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["qty"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetStockQtyByLength: " + ex.ToString());
            }
            return "";
        }

        public string GetStockQtyByExpiryDate(string id_stock)
        {
            try
            {
                if (id_stock != "")
                {
                    DataSet dsEmp = Getdetails("select (bal_qty-dummy_bal) as bal_qty from t_stock_master where id_stock='" + id_stock + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["bal_qty"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetStockQtyByExpiryDate: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList getMaterialCategoryStockList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Material Category' order by item_desc asc";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        if (dsData.Tables[0].Rows[i]["Name"].ToString() != "Roundbar" && dsData.Tables[0].Rows[i]["Name"].ToString() != "Chemicals" && dsData.Tables[0].Rows[i]["Name"].ToString() != "Paints" && dsData.Tables[0].Rows[i]["Name"].ToString() != "Structured Fabrication")
                        {
                            DropdownItems objReport = new DropdownItems()
                            {

                                Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                                Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                            };
                            objReportList.lstDropdown.Add(objReport);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getMaterialCategorystockList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        
        public MultiSelectList getMaterialCategoryRoundbarList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Material Category' order by item_desc asc";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        if (dsData.Tables[0].Rows[i]["Name"].ToString() == "Roundbar" || dsData.Tables[0].Rows[i]["Name"].ToString() == "Structured Fabrication")
                        {
                            DropdownItems objReport = new DropdownItems()
                            {

                                Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                                Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                            };
                            objReportList.lstDropdown.Add(objReport);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getMaterialCategoryRoundbarList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        public MultiSelectList getMaterialCategoryChemicalList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Material Category' order by item_desc asc";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        if (dsData.Tables[0].Rows[i]["Name"].ToString() == "Chemicals" || dsData.Tables[0].Rows[i]["Name"].ToString() == "Paints")
                        {
                            DropdownItems objReport = new DropdownItems()
                            {

                                Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                                Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                            };
                            objReportList.lstDropdown.Add(objReport);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getMaterialCategoryChemicalList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }
        
        public DataSet GenerateMaterialNo(string subcategory, string spec, string material_id, string diameter, string length, string material_type)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Gen_Material_no", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@vsubcategory", subcategory);
                cmd.Parameters.AddWithValue("@vspec", spec);
                cmd.Parameters.AddWithValue("@vmaterial_id", material_id);
                cmd.Parameters.AddWithValue("@vdiameter", diameter);
                cmd.Parameters.AddWithValue("@vlength", length);
                cmd.Parameters.AddWithValue("@vmaterial_type", material_type);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GenerateMaterialNo: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public MultiSelectList getMaterialList(string category)
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "";
                if (category != "")
                {
                    sSsqlstmt = "select id_material_master as Id,material as Name from t_material_master where active='1' and category='" + category + "'";
                }
                else
                {
                    sSsqlstmt = "select id_material_master as Id,material as Name from t_material_master where active='1'";
                }
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getMaterialList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public MultiSelectList getRoundbarMaterialList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "";
                sSsqlstmt = "select id_material_master as Id,material as Name from t_material_master m where active=1 and"
                + " category=(select item_id from dropdownheader d,dropdownitems dd where d.header_id=dd.header_id and header_desc='Material Category' and item_desc='Roundbar')";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsData.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsData.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getRoundbarMaterialList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }


        public string getMaterialById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select material from t_material_master where id_material_master='" + item_id + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["material"].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getMaterialById: " + ex.ToString());
            }
            return "";
        }

        public string GetStockQty(string category, string material)
        {
            try
            {
                if (category != "" && material != "")
                {
                    DataSet dsEmp = Getdetails("select (bal_qty-dummy_bal) as bal_qty from t_stock_master where category='" + category + "' and material='" + material + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["bal_qty"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetStockQty: " + ex.ToString());
            }
            return "";
        }

        public bool SetStockQty(string category, string material, int issue_qty)
        {
            try
            {
                if (category != "" && material != "")
                {
                    string sSqlstmt = "update t_stock_master set dummy_bal=dummy_bal + '" + issue_qty + "' where category='" + category + "' and material='" + material + "'";
                    if (ExecuteQuery(sSqlstmt))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in SetetStockQty: " + ex.ToString());
            }
            return false;
        }
        public bool SetRoundbarQty(string material,string length, int issue_qty)
        {
            try
            {
                if (material != "")
                {
                    string sSqlstmt = "update t_stock_master set dummy_bal=dummy_bal + '" + issue_qty + "' where  material='" + material + "' and  length='" + length + "'";
                    if (ExecuteQuery(sSqlstmt))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in SetRoundbarQty: " + ex.ToString());
            }
            return false;
        }
        public bool SetChemicalQty(string material, string expiry_date, int issue_qty)
        {
            try
            {
                if (material != "")
                {
                    string sSqlstmt = "update t_stock_master set dummy_bal=dummy_bal + '" + issue_qty + "' where  material='" + material + "' and  expiry_date='" + expiry_date + "'";
                    if (ExecuteQuery(sSqlstmt))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in SetRoundbarQty: " + ex.ToString());
            }
            return false;
        }
        public bool UpdateStockQty(string category, string material, int issue_qty)
        {
            try
            {
                if (category != "" && material != "")
                {
                    string sSqlstmt = "update t_stock_master set dummy_bal=dummy_bal - '" + issue_qty + "' where category='" + category + "' and material='" + material + "'";
                    if (ExecuteQuery(sSqlstmt))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in UpdateStockQty: " + ex.ToString());
            }
            return false;
        }
        public bool UpdateRoundbarQty(string material, string length,int issue_qty)
        {
            try
            {
                if (material != "")
                {
                    string sSqlstmt = "update t_stock_master set dummy_bal=dummy_bal - '" + issue_qty + "' where  material='" + material + "' and length='" + length + "' ";
                    if (ExecuteQuery(sSqlstmt))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in UpdateStockQty: " + ex.ToString());
            }
            return false;
        }
        public bool UpdateChemicalQty(string material, string sexpdate, int issue_qty)
        {
            try
            {
                if (material != "")
                {
                    string sSqlstmt = "update t_stock_master set dummy_bal=dummy_bal - '" + issue_qty + "' where  material='" + material + "' and expiry_date='" + sexpdate + "' ";
                    if (ExecuteQuery(sSqlstmt))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in UpdateStockQty: " + ex.ToString());
            }
            return false;
        }
        public bool CheckMaterialExists(string material)
        {
            try
            {
                string sSqlstmt = "select material from t_material_master where material='" + material + "' and active=1";
                DataSet dsData = Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in CheckMaterialExists: " + ex.ToString());
            }
            return true;
        }
        //-----------------------------------------------------------------------------
        //------------------------------t_hr_employee------------------------
        public string GetMultiHrEmpEmailIdById(string emp_no)
        {
            try
            {
                if (emp_no != "" && emp_no != null)
                {
                    DataSet dsEmp = Getdetails("Select  GROUP_CONCAT(m.EmailId) EmailId FROM t_hr_employee m  where emp_no in (" + emp_no + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["EmailId"].ToString();
                        //if (sDesc != "" && sDesc.Contains(','))
                        //{
                        //    return sDesc.Replace(",", ",");
                        //}
                        return (sDesc);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiEmpEmailIdById: " + ex.ToString());
            }
            return "";
        }

        public string GetMultiHrEmpNameById(string Emp_no)
        {
            try
            {
                if (Emp_no != "" && Emp_no != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as firstname FROM t_hr_employee m  where emp_no in (" + Emp_no + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = Regex.Replace(dsEmp.Tables[0].Rows[0]["firstname"].ToString(), " +", " ");
                        sDesc = sDesc.Trim();
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiHrEmpNameById: " + ex.ToString());
            }
            return "";
        }
               
        public string GetEmpHrNameById(string Emp_no)
        {
            try
            {
                if (Emp_no != "")
                {
                    DataSet dsEmp = Getdetails("select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as firstname from t_hr_employee  where emp_no='" + Emp_no + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string  firstname=Regex.Replace(dsEmp.Tables[0].Rows[0]["firstname"].ToString(), " +", " ");
                        return firstname.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetEmpHrNameById: " + ex.ToString());
            }
            return "";
        }
               
        public string GetHrEmpEmailIdById(string emp_no)
        {
            try
            {
                if (emp_no != "")
                {
                    DataSet dsEmp = Getdetails("select EmailId from t_hr_employee where emp_no='" + emp_no + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["EmailId"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHrEmpEmailIdById: " + ex.ToString());
            }
            return "";
        }
        
        public bool checkEmpInchargeById(string Empid)
        {
            try
            {
                DataSet dsEmp = Getdetails("select emp_firstname from t_hr_employee where emp_no='" + Empid + "' and DeptInCharge='Yes'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in checkEmpInchargeById: " + ex.ToString());
            }
            return false;
        }
        
        public MultiSelectList GetHrMultiEmployeeListByID(string Empids)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid"
                    + " from t_hr_employee where emp_status=1";
                if (Empids != "")
                {
                    sSqlstmt = sSqlstmt + " and emp_no in (" + Empids + ")";
                }
                sSqlstmt = sSqlstmt + " order by emp_firstname asc";
                DataSet dsEmp = Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = dsEmp.Tables[0].Rows[i]["Empname"].ToString()
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHrMultiEmployeeListByDept: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");

        }


        public string GetDeptIdByHrEmpId(string Emp_no)
        {
            try
            {
                if (Emp_no != "")
                {
                    DataSet dsDept = Getdetails("select group_concat(dept_id) dept_id from t_hr_employee where emp_no in (" + Emp_no + ")");
                    if (dsDept.Tables.Count > 0 && dsDept.Tables[0].Rows.Count > 0)
                    {
                        return (dsDept.Tables[0].Rows[0]["dept_id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDeptIdByHrEmpId: " + ex.ToString());
            }
            return "";
        }


        public string checkEmailAddressExists(string emailAddress)
        {
            try
            {
             
                string sSqlstmt = "select EmailId from t_hr_employee where EmailId='" + emailAddress + "' and emp_status=1";
                DataSet dsEmp = Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return ("false");
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in checkEmailAddressExists: " + ex.ToString());
            }
            //return true;
            return (emailAddress);
        }
        public string checkUserAcctnEmailExists(string emailAddress, string emp_no)
        {
            try
            {
                if (emailAddress != null && emailAddress != "")
                {
                    string sSqlstmt = "select EmailId from t_hr_employee where EmailId='" + emailAddress + "' and emp_status=1";
                    DataSet dsEmp = Getdetails(sSqlstmt);
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return ("false");
                    }
                }
                else
                {
                    string sSqlstmt = "select EmpID from t_employee where CompEmpId='" + emp_no + "' and active=1";
                    DataSet dsEmp = Getdetails(sSqlstmt);
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return ("false");
                    }    
                }
              
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in checkEmailAddressExists: " + ex.ToString());
            }
            //return true;
            return (emailAddress);
        }

        public string checkEmpIdExists(string EmpId)
        {
            try
            {
              
                string sSqlstmt = "select emp_id from t_hr_employee where emp_id='" + EmpId + "' and emp_status=1";
                DataSet dsEmp = Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return "false";
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in checkEmpIdExists: " + ex.ToString());
            }
            return EmpId;
        }
        public string GetHSEEmployee()
        {
            try
            {

                DataSet dsData = Getdetails("select group_concat(emp_no) EmpID from t_hr_employee e,t_departments d where emp_status=1 and e.Dept_Id=d.DeptId"
                    + " and (DeptName ='hse' or DeptName like 'hse%' or DeptName like '%hse' or  DeptName like '%hse%')");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["EmpID"].ToString());
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHSEEmployee: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetHrEmployeeListbox()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid from t_hr_employee where emp_status=1 order by emp_firstname asc";
                DataSet dsEmp = Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {

                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")

                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHrEmployeeListbox: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetHrEmpListByDept(string Dept_Id="")
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                if (Dept_Id != "")
                {
                    string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid from t_hr_employee where emp_status=1" +
                        " and Dept_Id in (" + Dept_Id + ") order by emp_firstname asc";
                    DataSet dsEmp = Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {

                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")

                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
                }
               
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHrEmpListByDept: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }
               
        public MultiSelectList GetHrEmpListByDivision(string Division_Id = "")
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                if (Division_Id != "")
                {
                    string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) " +
                        "as Empname, emp_no as Empid from t_hr_employee where emp_status=1 and division in (" + Division_Id + ") order by emp_firstname asc";
                    DataSet dsEmp = Getdetails(sSqlstmt);

                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                        {

                            Employee emp = new Employee()
                            {
                                Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                                Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")

                            };
                            emp.Empname = emp.Empname.Trim();
                            emplist.EmpList.Add(emp);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHrEmpListByDivision: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }


        public MultiSelectList GetHrQHSEEmployeeListbox()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid from t_hr_employee as emp, t_departments as dept where emp_status=1 and emp.Dept_Id= dept.DeptId and"
                    + " (DeptName like '%QHSE%' or DeptName like '%QHSE' or DeptName like 'QHSE%' or DeptName ='QHSE' or DeptName like '%HSE' or  DeptName like 'HSE%' or DeptName ='HSE') order by emp_firstname asc";

                DataSet dsEmp = Getdetails(sSqlstmt);

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHrQHSEEmployeeListbox: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }


        public string GetEmpIDByEmpNo(string emp_no)
        {
            try
            {
                if (emp_no != "")
                {
                    DataSet dsComp = Getdetails("select emp_id from t_hr_employee where emp_no='" + emp_no + "' and emp_status=1");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["emp_id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetEmpIDByEmpNo: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetDeptHeadbyDivisionList(string DivisionId = "")
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid from t_hr_employee where emp_status=1 and DeptInCharge='Yes' order by emp_firstname asc";
                if (DivisionId != "")
                {

                    sSqlstmt = "SELECT  emp_no as Empid, concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname FROM t_hr_employee as emp, t_company_branch as division where emp.division=division.id  and DeptInCharge='Yes'"
                        //+ " and emp.Dept_Id=" + Deptid + " and emp_status=1 ";
                        + " and find_in_set( emp.division, '" + DivisionId + "') and emp_status=1 ";
                }
                DataSet dsEmp = Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDeptHeadbyDivisionList: " + ex.ToString());
            }
            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetDeptHeadList(string Deptid = "")
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid from t_hr_employee where emp_status=1 and DeptInCharge='Yes' order by emp_firstname asc";
                if (Deptid != "")
                {

                    sSqlstmt = "SELECT  emp_no as Empid, concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname FROM t_hr_employee as emp, t_departments as dept where emp.Dept_Id=dept.DeptId  and DeptInCharge='Yes'"
                        //+ " and emp.Dept_Id=" + Deptid + " and emp_status=1 ";
                        + " and find_in_set( emp.Dept_Id, '" + Deptid + "') and emp_status=1 ";
                }
                DataSet dsEmp = Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDeptHeadList: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");

        }
        public string GetDeptHeadNameByDepartment(string Deptid)
        {
            try
            {
                if (Deptid != "" && Deptid != null)
                {
                    string sSqlstmt = "SELECT  group_concat(concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')))  as Empname FROM t_hr_employee as emp, t_departments as dept where emp.Dept_Id=dept.DeptId and DeptInCharge='Yes'"
                      + " and find_in_set( emp.Dept_Id, '" + Deptid + "') and emp_status=1 ";

                    DataSet dsDept = Getdetails(sSqlstmt);
                    if (dsDept.Tables.Count > 0 && dsDept.Tables[0].Rows.Count > 0)
                    {
                        return (dsDept.Tables[0].Rows[0]["Empname"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDeptHeadNameByDepartment: " + ex.ToString());
            }
            return "";
        }
        public string GetEmpDesignationByHrEmpId(string Emp_no)
        {
            try
            {
                if (Emp_no != "")
                {
                    DataSet dsDept = Getdetails("select Designation from t_hr_employee where emp_no='" + Emp_no + "'");
                    if (dsDept.Tables.Count > 0 && dsDept.Tables[0].Rows.Count > 0)
                    {
                        return (dsDept.Tables[0].Rows[0]["Designation"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetEmpDesignationByHrEmpId: " + ex.ToString());
            }
            return "";
        }

        public List<string> GetEmployeeNameListboxForAutoCompMaster()
        {
            List<string> dicLeave = new List<string>();
            try
            {
                string sSsqlstmt = "select emp_firstname from t_hr_employee where emp_status = 1 order by emp_firstname asc";

                DataSet dsLeave = Getdetails(sSsqlstmt);

                if (dsLeave.Tables.Count > 0 && dsLeave.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsLeave.Tables[0].Rows.Count; i++)
                    {

                        dicLeave.Add(dsLeave.Tables[0].Rows[i]["emp_firstname"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetEmployeeListboxForAutoCompMaster: " + ex.ToString());
            }

            return (dicLeave);
        }



        //-----------------------------------------------------------------------------
      
        public string GetTopMgmtEmployee() // ROLE BASED
        {
            try
            {

                DataSet dsData = Getdetails("select group_concat(emp_no) emp_no from t_hr_employee e,roles r where e.emp_status=1"
                + " and FIND_IN_SET(id,Role) and RoleName='Top Management'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["emp_no"].ToString());
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetTopMgmtEmployee: " + ex.ToString());
            }
            return "";
        }
        public DataSet GetLeaveBalCalReport()
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_leave_master", con);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.Parameters.AddWithValue("@Vyear", year);
                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLeaveBalCalReport: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        //---------------------------------------------------------------

        public MultiSelectList GetMettingRefList()
        {
            MeetingModelsList objMeetingModelsList = new MeetingModelsList();
            objMeetingModelsList.MeetingMList = new List<MeetingModels>();
            try
            {
                string sSsqlstmt = "select meeting_ref from t_meeting_items a, t_meeting_items_log b where a.Item_no=b.item_no";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        MeetingModels objReport = new MeetingModels()
                        {
                            meeting_ref = dsReportType.Tables[0].Rows[i]["meeting_ref"].ToString(),
                        };
                        objMeetingModelsList.MeetingMList.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMettingRefList: " + ex.ToString());
            }
            return new MultiSelectList(objMeetingModelsList.MeetingMList, "meeting_ref");
        }
              
        public string GetATSSourceFullFormById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    DataSet dsEmp = Getdetails("select item_id as Id, item_fulldesc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='ATS Source' and (item_id='" + item_id + "' or item_desc='" + item_id + "')");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetATSSourceFullFormById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetATSSourceFullFormList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_fulldesc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='ATS Source' order by item_fulldesc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetATSSourceFullFormList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }
         
        internal string GetSupplierPerfAction(decimal sValue)
        {
            try
            {
                DataSet dsData = Getdetails("SELECT action_sup,rating FROM t_supplier_perf_action where fromValue <= '" + sValue + "' and toValue >= '" + sValue + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["action_sup"].ToString().ToUpper() + "&" + dsData.Tables[0].Rows[0]["rating"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetSupplierPerfAction: " + ex.ToString());
            }
            return "";
        }

        internal string GetExposureValueById(string sSeverity_Id)
        {
            try
            {
                DataSet dsEmp =Getdetails("select item_id as Id, item_fulldesc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Risk-Exposure' and (item_id='" + sSeverity_Id + "' or item_desc='" + sSeverity_Id + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetSeverityLevelValueById: " + ex.ToString());
            }
            return "";
        }
             
        public DataSet Call_Generate_leave_master_proc()
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_leave_master", con);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in Call_Generate_leave_master_proc: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
           
       public DataSet GetBiddingCommentdetails(string Id)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("generate_bidding_comments", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocID", Id);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetBiddingCommentdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        
        public string GetLeaveMasterEmpIDByName(string emp_firstname)
        {
            try
            {
                if (emp_firstname != "")
                {
                    DataSet dsEmp = Getdetails("select emp_no from t_leave_master  where emp_firstname='" + emp_firstname + "'"
                    + " and leaveActive=1 and Active=1");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return (dsEmp.Tables[0].Rows[0]["emp_no"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLeaveMasterEmpIDByName: " + ex.ToString());
            }
            return "";
        }
                
        public int GetLeaveMasterID(string emp_no)
        {
            try
            {
                if (emp_no != "")
                {
                    DataSet dsBal = Getdetails("select emp_no,leave_id from t_leave_master "
                   + " where emp_no='" + emp_no + "' and Active = 1 and leaveActive=1 order by emp_no asc ");
                    if (dsBal.Tables.Count > 0 && dsBal.Tables[0].Rows.Count > 0)
                    {
                        return (Convert.ToInt32(dsBal.Tables[0].Rows[0]["leave_id"].ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLeaveMasterID: " + ex.ToString());
            }
            return 0;
        }
        public decimal GetLeaveBalance(string emp_no)
        {
            try
            {
                if (emp_no != "")
                {
                    DataSet dsBal = Getdetails("select balance from t_leave_master "
                   + " where emp_no='" + emp_no + "'");
                    if (dsBal.Tables.Count > 0 && dsBal.Tables[0].Rows.Count > 0)
                    {
                        return (Convert.ToDecimal(dsBal.Tables[0].Rows[0]["balance"].ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLeaveBalance: " + ex.ToString());
            }
            return 0;
        }
        public string GetOnSiteStatus(string emp_no)
        {
            try
            {
                if (emp_no != "")
                {
                    DataSet dsBal = Getdetails("select emp_no,onSite from t_leave_master "
                   + " where emp_no='" + emp_no + "' and Active = 1 and leaveActive=1 order by emp_no asc ");
                    if (dsBal.Tables.Count > 0 && dsBal.Tables[0].Rows.Count > 0)
                    {
                        return (dsBal.Tables[0].Rows[0]["onSite"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetOnSiteStatus: " + ex.ToString());
            }
            return "";
        }
        public List<string> GetLeaveMasterEmployeeNameListbox()
        {
            List<string> dicLeave = new List<string>();
            try
            {
                string sSsqlstmt = "select emp_firstname from t_leave_master  where Active = 1 and leaveActive=1 order by emp_firstname asc";

                DataSet dsLeave = Getdetails(sSsqlstmt);

                if (dsLeave.Tables.Count > 0 && dsLeave.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsLeave.Tables[0].Rows.Count; i++)
                    {

                        dicLeave.Add(dsLeave.Tables[0].Rows[i]["emp_firstname"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLeaveMasterEmployeeListbox: " + ex.ToString());
            }

            return (dicLeave);
        }
        public string getEmployeeLeaveStatus(string empno, string year)
        {
            try
            {
                if (empno != "")
                {
                    DataSet dsComp = Getdetails("select emp_no from t_leave_master where emp_no='" + empno + "' and cal_year='" + year + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["emp_no"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getEmployeeLeaveStatus: " + ex.ToString());
            }
            return "";
        }
      
     

        //TRYCHEM
        public MultiSelectList GetTrychemSalesmanList()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                string sSqlstmt = "select emp_firstname , emp_no from t_hr_employee where emp_status=1 and Designation='Sales' or Designation='sales' order by emp_firstname asc";
                DataSet dsEmp = Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["emp_no"].ToString(),
                            Empname = dsEmp.Tables[0].Rows[i]["emp_firstname"].ToString()
                        };

                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetTrychemSalesmanList: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "emp_no", "emp_firstname");
        }
        public string GetTrychemCustContactIDByName(string CustName,string custID)
        {
            try
            {
                if (CustName != "")
                {
                    DataSet dsRoles = Getdetails("select ContactsId from t_customer_info_contacts_trychem  where ContactPerson='" + CustName + "' and CustID='" + custID + "'");
                    if (dsRoles.Tables.Count > 0 && dsRoles.Tables[0].Rows.Count > 0)
                    {
                        return (dsRoles.Tables[0].Rows[0]["ContactsId"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetTrychemCustContactIDByName: " + ex.ToString());
            }
            return "";
        }
        public string GetTrychemCustContactNameByID(string Id)
        {
            try
            {
                if (Id != "")
                {
                    DataSet dsRoles = Getdetails("select ContactPerson from t_customer_info_contacts_trychem  where ContactsId='" + Id + "'");
                    if (dsRoles.Tables.Count > 0 && dsRoles.Tables[0].Rows.Count > 0)
                    {
                        return (dsRoles.Tables[0].Rows[0]["ContactPerson"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetTrychemCustContactNameByID: " + ex.ToString());
            }
            return "";
        }
        public string GetTrychemCustomerIDbyName(string cust)
        {
            try
            {
                if (cust != "")
                {
                    DataSet dsRoles = Getdetails("select CustID from t_customer_info_trychem  where CustomerName='" + cust + "' and Active=1");
                    if (dsRoles.Tables.Count > 0 && dsRoles.Tables[0].Rows.Count > 0)
                    {
                        return (dsRoles.Tables[0].Rows[0]["CustID"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetTrychemCustomerIDbyName: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetTrychemCustomerContactListbox(string sCompanyName)
        {
            CompanySupplierList objCompanySupplierList = new CompanySupplierList();
            objCompanySupplierList.CompanySuppList = new List<CompanySupplier>();

            try
            {
                if (sCompanyName != "")
                {
                    string sSsqlstmt = "select tcic.ContactsId, tcic.ContactPerson from t_customer_info_contacts_trychem as tcic,"
                    + "t_customer_info_trychem as tcinf where tcic.CustID= tcinf.CustID and (CustomerName='" + sCompanyName + "' or tcinf.CustID='" + sCompanyName + "')"
                    +"and tcic.Active=1 order by tcic.ContactPerson asc";

                    DataSet dsComp = Getdetails(sSsqlstmt);

                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsComp.Tables[0].Rows.Count; i++)
                        {
                            CompanySupplier Comp = new CompanySupplier()
                            {
                                Id = dsComp.Tables[0].Rows[i]["ContactsId"].ToString(),
                                Name = (dsComp.Tables[0].Rows[i]["ContactPerson"].ToString())
                            };

                            objCompanySupplierList.CompanySuppList.Add(Comp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetTrychemCustomerContactListbox: " + ex.ToString());
            }

            return new MultiSelectList(objCompanySupplierList.CompanySuppList, "Id", "Name");
        }
        public string GetCEOEmailID()//trychem
        {
            try
            {
                DataSet dsEmp = Getdetails("select group_concat(EmailId) EmailId from t_hr_employee e,roles r where e.emp_status=1"
               + " and (FIND_IN_SET(id,Role) and (RoleName='CEO' or RoleName like '%CEO%')or FIND_IN_SET(id,Role) and RoleName='COO')");
              
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    string sDesc = dsEmp.Tables[0].Rows[0]["EmailId"].ToString();
                    if (sDesc != "" && sDesc.Contains(','))
                    {
                        return sDesc.Replace(",", ", ");
                    }
                    return (sDesc);
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCEOEmailID: " + ex.ToString());
            }
            return "";
        }
        public string GetMultiEmpEmailIdByDivision(string Division)//trychem
        {
            try
            {
                if (Division != "")
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(m.EmailId) EmailId FROM t_hr_employee m  where find_in_set('" + Division + "',Role) and emp_status=1");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["EmailId"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return (sDesc);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiEmpEmailIdByDivision: " + ex.ToString());
            }
            return "";
        }
        public string GetRolesIDByName(string Role)
        {
            try
            {
                if (Role != "")
                {
                    DataSet dsRoles = Getdetails("select Id from Roles  where RoleName='" + Role + "'");
                    if (dsRoles.Tables.Count > 0 && dsRoles.Tables[0].Rows.Count > 0)
                    {
                        return (dsRoles.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRolesIDByName: " + ex.ToString());
            }
            return "";
        }

        public string GetINCOIdByName(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='INCO' and (item_id='" + item_id + "' or item_desc='" + item_id + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetINCOIdByName: " + ex.ToString());
            }
            return "";
        }
        public string GetINCOById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='INCO' and (item_id='" + item_id + "' or item_desc='" + item_id + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetINCOById: " + ex.ToString());
            }
            return "";
        }
        public List<string> GetCountryListboxList()
        {
            List<string> dicCustomer = new List<string>();
            try
            {
                string sSsqlstmt = "select country_name as Name from t_country order by Name asc";

                DataSet dsCountry = Getdetails(sSsqlstmt);

                if (dsCountry.Tables.Count > 0 && dsCountry.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCountry.Tables[0].Rows.Count; i++)
                    {
                        dicCustomer.Add(dsCountry.Tables[0].Rows[i]["Name"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCountryListboxList: " + ex.ToString());
            }

            return (dicCustomer);
        }
        public string GetProposalStatusById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Proposal Status' and (item_id='" + item_id + "' or item_desc='" + item_id + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetProposalStatusById: " + ex.ToString());
            }
            return "";
        }
        public string GetDivisionById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Division' and (item_id='" + item_id + "' or item_desc='" + item_id + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDivisionById: " + ex.ToString());
            }
            return "";
        }
        public Decimal GetProductPrice(string id)
        {
            try
            {
                if (id != "")
                {
                    DataSet dsComp = Getdetails("select ZR10 from t_product_trychem where name='" + id + "' or id_product='" + id + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return Convert.ToDecimal(dsComp.Tables[0].Rows[0]["ZR10"]);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetProductPrice: " + ex.ToString());
            }
            return 0;
        }
        public List<string> GetTryChemProductList()
        {
            List<string> dicCustomer = new List<string>();
            try
            {
                string sSsqlstmt = "select name,proddesc from t_product_trychem where Active=1 order by name asc";

                DataSet dsProduct = Getdetails(sSsqlstmt);

                if (dsProduct.Tables.Count > 0 && dsProduct.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsProduct.Tables[0].Rows.Count; i++)
                    {
                        dicCustomer.Add(dsProduct.Tables[0].Rows[i]["name"].ToString().ToUpper() + " - " + dsProduct.Tables[0].Rows[i]["proddesc"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetTryChemProductList: " + ex.ToString());
            }

            return (dicCustomer);
        }
        public MultiSelectList GetINCO()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='INCO' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetINCO: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }
        public List<string> GetTryChemCustomerListboxList()
        {
            List<string> dicCustomer = new List<string>();
            try
            {
                string sSsqlstmt = "select CustomerName from t_customer_info_trychem where Active=1 order by CustomerName asc";

                DataSet dsCust = Getdetails(sSsqlstmt);

                if (dsCust.Tables.Count > 0 && dsCust.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCust.Tables[0].Rows.Count; i++)
                    {
                        dicCustomer.Add(dsCust.Tables[0].Rows[i]["CustomerName"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetTryChemCustomerListboxList: " + ex.ToString());
            }

            return (dicCustomer);
        }
        public MultiSelectList GetDivision()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Division' order by item_desc asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDivision: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }
        public string GetCountryNameById(string Country)
        {
            try
            {
                if (Country != "" && Country != null)
                {
                    DataSet dsComp = Getdetails("select GROUP_CONCAT(country_name) as Name from t_country where id_country in (" + Country + ") order by Name asc");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsComp.Tables[0].Rows[0]["Name"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }                
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCountryNameById: " + ex.ToString());
            }

            return "";
        }

       public string GetEmployeeRealtionshipsIdByName(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Employee Relationship' and (item_id='" + item_id + "' or item_desc='" + item_id + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetEmployeeRealtionshipsIdByName: " + ex.ToString());
            }
            return "";
        }
       

        public string GetProjectStatusIdByName(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Project Status' and (item_id='" + item_id + "' or item_desc='" + item_id + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetProjectStatusIdByName: " + ex.ToString());
            }
            return "";
        }

        public string GetDiciplineIdByName(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Project Dicipline' and (item_id='" + item_id + "' or item_desc='" + item_id + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDiciplineIdByName: " + ex.ToString());
            }
            return "";
        }

         public string getActiveEquipmentID()
        {
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                   + "and header_desc='Calibration Status' and item_desc='Active'";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Id"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in getActiveEquipmentID: " + ex.ToString());
            }
            
            return "";
        }

      
       
        public string GetDocumentRefById(string id)
        {
            try
            {
                if (id != "")
                {
                    DataSet DsDoc = Getdetails("select DocRef from t_mgmt_documents where idMgmt='" + id + "'");
                    if (DsDoc.Tables.Count > 0 && DsDoc.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < DsDoc.Tables[0].Rows.Count; i++)
                        {
                            return (DsDoc.Tables[0].Rows[i]["DocRef"].ToString());
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDocumentRefById: " + ex.ToString());
            }
            return "";
        }
        public List<string> GetDocumentNameListboxList(string DeptID, string DeptName)
        {
            List<string> Document = new List<string>();
            try
            {
                if (DeptName.ToLower() != "all")
                {

                    string sSsqlstmt = "select DocName from t_mgmt_documents where Department='" + DeptID + "' and Status=1";

                    DataSet dsDocument = Getdetails(sSsqlstmt);

                    if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsDocument.Tables[0].Rows.Count; i++)
                        {
                            Document.Add(dsDocument.Tables[0].Rows[i]["DocName"].ToString().ToUpper());
                        }
                    }
                }
                else
                {
                    string sSsqlstmt = "select DocName from t_mgmt_documents where Status=1";

                    DataSet dsDocument = Getdetails(sSsqlstmt);

                    if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsDocument.Tables[0].Rows.Count; i++)
                        {
                            Document.Add(dsDocument.Tables[0].Rows[i]["DocName"].ToString().ToUpper());
                        }
                    }
                
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDocumentNameListboxList: " + ex.ToString());
            }

            return (Document);
        }

        public MultiSelectList GetDocumentRefList()
        {
            DocumentRefList docref = new DocumentRefList();
            docref.DoctRefList = new List<DocumentRef>();
            try
            {
                DataSet dsDocRef = Getdetails("Select idMgmt,DocRef from t_mgmt_documents where Status=1 and Approved_Status='1' and Reviewed_Status='1'");
                if (dsDocRef.Tables.Count > 0 && dsDocRef.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsDocRef.Tables[0].Rows.Count; i++)
                    {
                        DocumentRef Ref = new DocumentRef()
                        {
                            idMgmt = dsDocRef.Tables[0].Rows[i]["idMgmt"].ToString(),
                            DocRef = dsDocRef.Tables[0].Rows[i]["DocRef"].ToString().ToUpper()
                        };

                        docref.DoctRefList.Add(Ref);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDocumentRefList: " + ex.ToString());
            }

            return new MultiSelectList(docref.DoctRefList, "idMgmt", "DocRef");
        }
        public MultiSelectList GetDocumentRefListForEdit()
        {
            DocumentRefList docref = new DocumentRefList();
            docref.DoctRefList = new List<DocumentRef>();
            try
            {
                DataSet dsDocRef = Getdetails("Select idMgmt,DocRef from t_mgmt_documents where Status=1");
                if (dsDocRef.Tables.Count > 0 && dsDocRef.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsDocRef.Tables[0].Rows.Count; i++)
                    {
                        DocumentRef Ref = new DocumentRef()
                        {
                            idMgmt = dsDocRef.Tables[0].Rows[i]["idMgmt"].ToString(),
                            DocRef = dsDocRef.Tables[0].Rows[i]["DocRef"].ToString().ToUpper()
                        };

                        docref.DoctRefList.Add(Ref);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDocumentRefList: " + ex.ToString());
            }

            return new MultiSelectList(docref.DoctRefList, "idMgmt", "DocRef");
        }
        public MultiSelectList GetDocumentNameList()
        {
            DocumentRefList docref = new DocumentRefList();
            docref.DoctRefList = new List<DocumentRef>();
            try
            {
                DataSet dsDocRef = Getdetails("Select idMgmt,DocName from t_mgmt_documents where Status=1 and Approved_Status='1' and Reviewed_Status='1'");
                if (dsDocRef.Tables.Count > 0 && dsDocRef.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsDocRef.Tables[0].Rows.Count; i++)
                    {
                        DocumentRef Ref = new DocumentRef()
                        {
                            idMgmt = dsDocRef.Tables[0].Rows[i]["idMgmt"].ToString(),
                            DocName = dsDocRef.Tables[0].Rows[i]["DocName"].ToString().ToUpper()
                        };

                        docref.DoctRefList.Add(Ref);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDocumentNameList: " + ex.ToString());
            }

            return new MultiSelectList(docref.DoctRefList, "idMgmt", "DocName");
        }
        

        public string GetDashboardReportTypeByID(string dashboard_id)
        {
            try
            {
                if (dashboard_id != "")
                {
                    string sSsqlstmt = "select dashboard_name from dashboard_reports where dashboard_id='" + dashboard_id + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["dashboard_name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDashboardReportTypeByID: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetDashboardReportType()
        {
            DashboardReportList objReportList = new DashboardReportList();
            objReportList.lstDropdown = new List<DashboardReport>();
            try
            {
                string sSsqlstmt = "select dashboard_id,dashboard_name from dashboard_reports order by dashboard_name asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DashboardReport objReport = new DashboardReport()
                        {
                            dashboard_id = dsReportType.Tables[0].Rows[i]["dashboard_id"].ToString(),
                            dashboard_name = dsReportType.Tables[0].Rows[i]["dashboard_name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDashboardReportType: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "dashboard_id", "dashboard_name");

        }
        
        public string GetDocRefByID(string Ref)
        {
            try
            {
                if (Ref != "")
                {
                    DataSet dsComp = Getdetails("select DocRef from t_mgmt_documents where idMgmt='" + Ref + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["DocRef"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDocRefByID: " + ex.ToString());
            }
            return "";
        }
        public string GetDocNameByID(string DocName)
        {
            try
            {
                if (DocName != "")
                {
                    DataSet dsComp = Getdetails("select DocName from t_mgmt_documents where idMgmt='" + DocName + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["DocName"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDocNameByID: " + ex.ToString());
            }
            return "";
        }

        
    

        public string GetMultiVisitorsByID(string id_visitors)
        {
            try
            {
                if (id_visitors != "")
                {
                    string sSsqlstmt = "select  GROUP_CONCAT(concat(firstname,' ',ifnull(lastname,' '))) as Name from t_visitors where id_visitors in (" + id_visitors + ") ";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsData.Tables[0].Rows[0]["Name"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiVisitorsByID: " + ex.ToString());
            }
            return "";
        }
        public string GetVisitorsByID(string id_visitors)
        {
            try
            {
                if (id_visitors != "")
                {
                    string sSsqlstmt = "select  concat(firstname,' ',ifnull(lastname,' ')) as Name from t_visitors where id_visitors='" + id_visitors + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetVisitorsByID: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetVisitorList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
           
            try
            {
                string sSsqlstmt = "select id_visitors as Id, concat(firstname,' ',ifnull(lastname,' ')) as Name from t_visitors where hse_ind='Yes' order by Name asc";

                DataSet dsReportType = Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetVisitorList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }
         
     
        public string GetEmployeeIdBytranid(string stran_id)
        {
            try
            {
                if (stran_id != "")
                {
                    DataSet dsComp = Getdetails("select emp_no from t_leave_tran where tran_id='" + stran_id + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["emp_no"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetEmployeeIdBytranid: " + ex.ToString());
            }
            return "";
        }
               
     

        public void AddFunctionalLog(string msg, string Filename = "Generallog.txt")
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\";
                StreamWriter log;

                if (!File.Exists(path + Filename))
                {
                    log = new StreamWriter(path + Filename);
                }
                else
                {
                    log = File.AppendText(path + Filename);
                }

                log.WriteLine(DateTime.Now);
                log.WriteLine(msg);
                log.WriteLine();

                // Close the stream:
                log.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog(ex.ToString());
            }
        }

        public bool ExecuteQuery(string sSqlStmt)
        {
            //string pattern = @"([a-z0-9]+)\'+([a-z0-9]+)";
            //string replacement = "$1''$2";
            //sSqlStmt = Regex.Replace(sSqlStmt, pattern, replacement);

            int response = 0;
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand(sSqlStmt, con);

                response = cmd.ExecuteNonQuery();
               
                return true;
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in ExecuteQuery: " + sSqlStmt);
                AddFunctionalLog("Exception in ExecuteQuery: " + ex.ToString());
            }
            finally {
                con.Close();
            }

            return false;
        }

        public int ExecuteQueryReturnId(string sSqlStmt)
        {
            //string pattern = @"([a-z0-9]+)\'+([a-z0-9]+)";
            //string replacement = "$1''$2";
            //sSqlStmt = Regex.Replace(sSqlStmt, pattern, replacement);

            int id = 0;
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand(sSqlStmt, con);

                cmd.ExecuteNonQuery();
                id = Convert.ToInt16(cmd.LastInsertedId);
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in ExecuteQueryReturnId: " + sSqlStmt);
                AddFunctionalLog("Exception in ExecuteQueryReturnId: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return id;
        }


        public DataSet Getdetails(string sSqlStmt)
        {
            DataSet dsData = new DataSet();
           
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlDataAdapter objAdap = new MySqlDataAdapter(sSqlStmt, con);

                objAdap.Fill(dsData);
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in Getdetails: " + ex.ToString());
                AddFunctionalLog("Exception in Getdetails: " + sSqlStmt);
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public DataSet GetAuditNCReportdetails(string sAuditNum, string sDept)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_internal_audit_rpt", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AuditNO", sAuditNum);
                cmd.Parameters.AddWithValue("@VDeptID", sDept);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditNCReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public DataSet LoginAuth(string sUsername, string sPwd)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("login_auth", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@vEmailid", sUsername);
                cmd.Parameters.AddWithValue("@pass", sPwd);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in LoginAuth: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public DataSet GetAuditLogReportdetails(string sAuditNum)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_internal_audit_log_all", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AuditNO", sAuditNum);
                //cmd.Parameters.AddWithValue("@VDeptID", sDept);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditLogReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public DataSet GetHSEStatisticsReport(string months, string years)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_HSE_stats", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@months", months);
                cmd.Parameters.AddWithValue("@years", years);
                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHSEStatisticsReport: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public DataSet GetAuditSummaryReportdetails(string sAuditNum)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_audit_int_summary_rpt", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AuditNO", sAuditNum);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditSummaryReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        public DataSet GetSupplierReport()
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_supplierlist", con);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetSupplierReport: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        public DataSet GetHSEReport(string month,string year)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_HSE_Report", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Vmonth", month);
                cmd.Parameters.AddWithValue("@Vyear", year);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetHSEReport: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        public DataSet GetMgmtMasterDocListReportdetails()
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_masterlist", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@Vempid", empid);
                //cmd.Parameters.AddWithValue("@Vdeptid", Department);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMgmtDocumentReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }
        public DataSet GetDocumentChangeRequestCommentdetails(string Id)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("generate_documentchangerequest_comments", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocID", Id);
              
                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMgmtDocumentReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public DataSet GetObjectivesReportdetails(string dtFromDate, string dtToDate)
        {
            DataSet dsData = new DataSet();

            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_objectives_rpt", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@vFromperiod", dtFromDate);
                cmd.Parameters.AddWithValue("@vToperiod", dtToDate);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMgmtDocumentReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public DataSet GetShortFallSourcedetails(string HSEfromDate, string HSEtoDate)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Generate_shortfall", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@frdate", HSEfromDate);
                cmd.Parameters.AddWithValue("@todate", HSEtoDate);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditLogReportdetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public DataSet GetReportNo(string smod, string sProject, string slocation)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                string empid = GetCurrentUserSession().empid;

                con.Open();
                MySqlCommand cmd = new MySqlCommand("Get_next_no", con);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@vmodname", smod);
                cmd.Parameters.AddWithValue("@vproject", sProject);
                cmd.Parameters.AddWithValue("@brname", slocation);
                cmd.Parameters.AddWithValue("@vempid", empid);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetReportNo: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public List<string> GetDepartmentList()
        {
            List<string> lstDept = new List<string>();
            try
            {
                DataSet dsData = Getdetails("select DeptName from t_departments");

               
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    lstDept = dsData.Tables[0].AsEnumerable().Select(r => r[0].ToString()).ToList();
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDepartmentList: " + ex.ToString());
            }
            return lstDept;
        }       

        public List<string> GetISOStdList()
        {
            List<string> lstIsoStd = new List<string>();
            try
            {
                DataSet dsData = Getdetails("select IsoName from t_isostandards where Active=1");

                
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    lstIsoStd = dsData.Tables[0].AsEnumerable().Select(r => r[0].ToString()).ToList();
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetISOStdList: " + ex.ToString());
            }
            return lstIsoStd;
        }
              

        public MultiSelectList GetPreparer()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                DataSet dsEmp = Getdetails("select emp_no as Empid,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Empname"
                +" from t_hr_employee h, roles r where  h.emp_status = 1 and FIND_IN_SET(id, Role) and RoleName = 'Preparer'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetPreparer: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetReviewer()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();           
            try
            {
                DataSet dsEmp = Getdetails("select emp_no as Empid,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Empname"
               + " from t_hr_employee h, roles r where  h.emp_status = 1 and FIND_IN_SET(id, Role) and RoleName = 'Reviewer'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                       
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetReviewer: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        public MultiSelectList GetApprover()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();            
            try
            {
                DataSet dsEmp = Getdetails("select emp_no as Empid,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) as Empname"
               + " from t_hr_employee h, roles r where  h.emp_status = 1 and FIND_IN_SET(id, Role) and RoleName = 'Approver'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetApprover: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

       


        public MultiSelectList GetCompanyBranchListbox()
        {
            CompanyBranchList Branchlist = new CompanyBranchList();
            Branchlist.CompBranchList = new List<CompanyBranch>();
            try
            {
                DataSet dsBranch = Getdetails("select id, branchname from t_company_branch, t_CompanyInfo where active=1 and compid=CompanyID order by branchname asc");// and CompanyId='" + sCompanyId+"'");

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        CompanyBranch Branch = new CompanyBranch()
                        {
                            BranchId = dsBranch.Tables[0].Rows[i]["Id"].ToString(),
                            BranchName = dsBranch.Tables[0].Rows[i]["BranchName"].ToString().ToUpper()
                        };

                        Branchlist.CompBranchList.Add(Branch);
                    }
                }
                else
                {
                    CompanyBranch Branch = new CompanyBranch()
                    {
                        BranchId = "1",
                        BranchName = "Main Office"
                    };

                    Branchlist.CompBranchList.Add(Branch);
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCompanyBranchListbox: " + ex.ToString());
            }
            return new MultiSelectList(Branchlist.CompBranchList, "BranchId", "BranchName");
        }
                       
        public MultiSelectList GetSupplierList()
        {
            SupplierList lstSupplier = new SupplierList();
            lstSupplier.lstSupplier = new List<Suppliers>();
            try
            {
                string sSqlstmt = "select SupplierId, SupplierName from t_supplier where ApprovedStatus=1 and Active=1 order by SupplierName asc";

                DataSet dsSupp = Getdetails(sSqlstmt);
                if (dsSupp.Tables.Count > 0 && dsSupp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsSupp.Tables[0].Rows.Count; i++)
                    {
                        Suppliers reg = new Suppliers()
                        {
                            SupplierId = dsSupp.Tables[0].Rows[i]["SupplierId"].ToString(),
                            SupplierName = dsSupp.Tables[0].Rows[i]["SupplierName"].ToString()
                        };

                        lstSupplier.lstSupplier.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetSupplierList: " + ex.ToString());
            }

            return new MultiSelectList(lstSupplier.lstSupplier, "SupplierId", "SupplierName");
        }

        public MultiSelectList GetCustomerListbox()
        {
            CompanySupplierList objCompanySupplierList = new CompanySupplierList();
            objCompanySupplierList.CompanySuppList = new List<CompanySupplier>();

            try
            {
                string sSsqlstmt = "select CustID, CompanyName from t_customer_info where compstatus=1 order by CompanyName asc";
                
                DataSet dsBranch = Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        CompanySupplier Branch = new CompanySupplier()
                        {
                            Id = dsBranch.Tables[0].Rows[i]["CustID"].ToString(),
                            Name = dsBranch.Tables[0].Rows[i]["CompanyName"].ToString().ToUpper()
                        };

                        objCompanySupplierList.CompanySuppList.Add(Branch);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustomerListbox: " + ex.ToString());
            }

            return new MultiSelectList(objCompanySupplierList.CompanySuppList, "Id", "Name");
        }


        public MultiSelectList GetCustomerListboxwithBranch(string Branch_Id)
        {
            CompanySupplierList objCompanySupplierList = new CompanySupplierList();
            objCompanySupplierList.CompanySuppList = new List<CompanySupplier>();

            try
            {
                //string sSsqlstmt = "select CustID, CompanyName from t_customer_info where compstatus=1 and branch = '"+ Branch_Id + "' order by CompanyName asc";
                string sSsqlstmt = "select CustID, CompanyName from t_customer_info where compstatus=1 and find_in_set('" + Branch_Id + "', branch) order by CompanyName asc";

                DataSet dsBranch = Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        CompanySupplier Branch = new CompanySupplier()
                        {
                            Id = dsBranch.Tables[0].Rows[i]["CustID"].ToString(),
                            Name = dsBranch.Tables[0].Rows[i]["CompanyName"].ToString().ToUpper()
                        };

                        objCompanySupplierList.CompanySuppList.Add(Branch);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustomerListboxwithBranch: " + ex.ToString());
            }

            return new MultiSelectList(objCompanySupplierList.CompanySuppList, "Id", "Name");
        }

        public MultiSelectList GetCustomerContactListbox(string sCompanyName)
        {
            CompanySupplierList objCompanySupplierList = new CompanySupplierList();
            objCompanySupplierList.CompanySuppList = new List<CompanySupplier>();

            try
            {
                if (sCompanyName != "")
                {
                    string sSsqlstmt = "select tcic.ContactsId, tcic.ContactPerson, tcic.PhoneNumber, tcic.EmailId, tcic.MobileNumber from t_customer_info_contacts as tcic, "
                        +"t_customer_info as tcinf where tcic.CustID= tcinf.CustID and (CompanyName='" + sCompanyName + "' or tcinf.CustID='" + sCompanyName
                        + "') and Active=1 order by tcic.ContactPerson asc";

                    DataSet dsBranch = Getdetails(sSsqlstmt);

                    if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                        {
                            CompanySupplier Branch = new CompanySupplier()
                            {
                                Id = dsBranch.Tables[0].Rows[i]["ContactsId"].ToString() + "&" + dsBranch.Tables[0].Rows[i]["PhoneNumber"].ToString()
                                + "&" + dsBranch.Tables[0].Rows[i]["MobileNumber"].ToString() + "&" + dsBranch.Tables[0].Rows[i]["EmailId"].ToString(),
                                Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dsBranch.Tables[0].Rows[i]["ContactPerson"].ToString())
                            };

                            objCompanySupplierList.CompanySuppList.Add(Branch);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustomerContactListbox: " + ex.ToString());
            }

            return new MultiSelectList(objCompanySupplierList.CompanySuppList, "Id", "Name");
        }



        public List<string> GetCustomerListboxList()
        {
            List<string> dicCustomer = new List<string>();
            try
            {
                string sSsqlstmt = "select CompanyName from t_customer_info where compstatus=1 order by CompanyName asc";

                DataSet dsBranch = Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        dicCustomer.Add(dsBranch.Tables[0].Rows[i]["CompanyName"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustomerContactListbox: " + ex.ToString());
            }

            return (dicCustomer);
        }

        public List<string> GetSupplierNameList()
        {
            List<string> lstSupplier = new List<string>();
            try
            {
                string sSsqlstmt = "select SupplierName from t_supplier where Active=1 order by SupplierName asc";

                DataSet dsBranch = Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        lstSupplier.Add(dsBranch.Tables[0].Rows[i]["SupplierName"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetSupplierNameList: " + ex.ToString());
            }

            return (lstSupplier);
        }
               
      
        public string GetCustomerNameById(string id)
        {
            try
            {
                if (id != "")
                {
                    DataSet dsComp = Getdetails("select CompanyName from t_customer_info where CustID='" + id + "' or CompanyName='" + id + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["CompanyName"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustomerNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetContactPersonNameById(string id)
        {
            try
            {
                if (id != "")
                {
                    DataSet dsComp = Getdetails("select ContactPerson from t_customer_info_contacts where ContactsId='" + id + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["CompanyName"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetContactPersonNameById: " + ex.ToString());
            }
            return "";
        }
        public string GetCustContactPersonNameById(string ContactsId, string CustID)
        {
            try
            {
                if (ContactsId != "")
                {
                    if (ContactsId.Contains('&'))
                    {
                        string[] ContactDetails = ContactsId.Split('&');
                        ContactsId = ContactDetails[0];
                    }

                    DataSet dsComp = Getdetails("select ContactPerson from t_customer_info_contacts where ContactsId='" + ContactsId
                         + "' or ContactPerson='" + ContactsId + "' and CustID='" + CustID + "'");

                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dsComp.Tables[0].Rows[0]["ContactPerson"].ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustContactPersonNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetCustContactPersonEmailIdById(string ContactsId, string CustID)
        {
            try
            {
                if (ContactsId != "")
                {
                    DataSet dsComp = Getdetails("select EmailId from t_customer_info_contacts where ContactsId='" + ContactsId
                         + "' or ContactPerson='" + ContactsId + "' and CustID='" + CustID + "'");

                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["EmailId"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustContactPersonEmailIdById: " + ex.ToString());
            }
            return "";
        }

        public string GetCustContactPersonPhoneById(string ContactsId, string CustID)
        {
            try
            {
                if (ContactsId != "")
                {
                    DataSet dsComp = Getdetails("select PhoneNumber from t_customer_info_contacts where ContactsId='" + ContactsId
                        + "' or ContactPerson='" + ContactsId + "' and CustID='" + CustID + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["PhoneNumber"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustContactPersonPhoneById: " + ex.ToString());
            }
            return "";
        }

        public string GetCustContactPersonMobileById(string ContactsId, string CustID)
        {
            try
            {
                if (ContactsId != "")
                {
                    DataSet dsComp = Getdetails("select MobileNumber from t_customer_info_contacts where ContactsId='" + ContactsId
                        + "' or ContactPerson='" + ContactsId + "' and CustID='" + CustID + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["MobileNumber"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustContactPersonMobileById: " + ex.ToString());
            }
            return "";
        }

        public string GetCustomerIdByName(string sName)
        {
            try
            {
                if (sName != "")
                {
                    DataSet dsComp = Getdetails("select CustID from t_customer_info where CompanyName='" + sName + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["CustID"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustomerIdByName: " + ex.ToString());
            }
            return "";
        }

        internal string GetSupplierIdByName(string SupplierName)
        {
            try
            {
                DataSet dsData = Getdetails("select SupplierId from t_supplier where SupplierName='" + SupplierName + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["SupplierId"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetSupplierIdByName: " + ex.ToString());
            }
            return "";
        }

        public string GetSupplierNameById(string SupplierId)
        {
            try
            {
                DataSet dsData = Getdetails("select SupplierName from t_supplier where SupplierId='" + SupplierId + "' order by SupplierName asc");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["SupplierName"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetSupplierNameById: " + ex.ToString());
            }
            return "";
        }
        
        public string GetSupplierCodeBySupplierId(string SupplierId)
        {
            try
            {
                DataSet dsData = Getdetails("select SupplierCode from t_supplier where SupplierId='" + SupplierId + "' and active = 1 order by SupplierName asc");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["SupplierCode"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetSupplierCodeBySupplierId: " + ex.ToString());
            }
            return "";
        }

        public string GetSupplierScopeBySupplierId(string SupplierId)
        {
            try
            {
                DataSet dsData = Getdetails("select SupplyScope from t_supplier where SupplierId='" + SupplierId + "' and active = 1 order by SupplierName asc");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["SupplyScope"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetSupplierScopeBySupplierId: " + ex.ToString());
            }
            return "";
        }

        public string GetCustomerTypeById(string id)
        {
            try
            {
                if (id != "")
                {
                    DataSet dsComp = Getdetails("select CustType from t_customer_info where CustID='" + id + "' or CompanyName='" + id + "'");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["CustType"].ToString().ToLower());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustomerTypeById: " + ex.ToString());
            }
            return "";
        }

      

        public string GetSO_NOByProductionId(string prod_log_id)
        {
            try
            {
                if (prod_log_id != "")
                {
                    DataSet dsComp = Getdetails("SELECT SO_No from t_prod_log as pro, t_customer_enquiry as tce where pro.enquiry_id=tce.enquiry_id and prod_log_id='" + prod_log_id
                        + "' and Enq_Status=1");
                    if (dsComp.Tables.Count > 0 && dsComp.Tables[0].Rows.Count > 0)
                    {
                        return (dsComp.Tables[0].Rows[0]["SO_No"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetSO_NOByProductionId: " + ex.ToString());
            }
            return "";
        }
        
        public MultiSelectList GetCountryCodeByName(string id_country = "")
        {
            CountryCodeList objCountryList = new CountryCodeList();
            objCountryList.CountryCodeLists = new List<CountryCode>();
            try
            {
                string sSqlstmt = "select id_country,country_code as Code from t_country where id_country='" + id_country + "'";
                if (id_country != "")
                {
                    sSqlstmt = "select id_country,country_code as Code from t_country where id_country='" + id_country + "'";
                }
                DataSet dsEmp = Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        CountryCode objCountry = new CountryCode()
                        {
                            id_country = dsEmp.Tables[0].Rows[i]["id_country"].ToString(),
                            
                            Code = dsEmp.Tables[0].Rows[i]["Code"].ToString(),
                        };

                        objCountryList.CountryCodeLists.Add(objCountry);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCountryCodeByName: " + ex.ToString());
            }

            return new MultiSelectList(objCountryList.CountryCodeLists, "id_country", "Code");

        }
        public MultiSelectList GetCountryCode()
        {
            CountryCodeList objCountryList = new CountryCodeList();
            objCountryList.CountryCodeLists = new List<CountryCode>();
            try
            {
                string sSsqlstmt = "select id_country,country_name as Name,country_code as Code from t_country order by Name asc";
                DataSet dsCountry = Getdetails(sSsqlstmt);

                if (dsCountry.Tables.Count>0 && dsCountry.Tables[0].Rows.Count >0)
                {
                    for (int i=0;i<dsCountry.Tables[0].Rows.Count;i++)
                    {
                        CountryCode objCountry = new CountryCode()
                        {
                            id_country = dsCountry.Tables[0].Rows[i]["id_country"].ToString(),
                            Name = dsCountry.Tables[0].Rows[i]["Name"].ToString(),
                            Code = dsCountry.Tables[0].Rows[i]["Code"].ToString(),

                        };
                        objCountryList.CountryCodeLists.Add(objCountry);
                    }

                }
            }
            catch(Exception ex)
            {
                AddFunctionalLog("Exception in GetCountryCode: " + ex.ToString());
            }
            return new MultiSelectList(objCountryList.CountryCodeLists, "id_country", "Name", "Code");
        }
        public MultiSelectList GetModeOfComplaint()
        {
            DropdownList objProductList = new DropdownList();
            objProductList.lstDropdown = new List<DropdownItems>();

            try
            {
                //string sSsqlstmt = "select ProductId, ProductName from t_product where Active=1";
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Mode Of Complaint' order by item_desc asc";

                DataSet dsBranch = Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objProduct = new DropdownItems()
                        {
                            Id = dsBranch.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsBranch.Tables[0].Rows[i]["Name"].ToString()
                        };

                        objProductList.lstDropdown.Add(objProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetModeOfComplaint: " + ex.ToString());
            }

            return new MultiSelectList(objProductList.lstDropdown, "Id", "Name");
        }
        public string GetModeOfComplaintById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Mode Of Complaint' and (item_id='" + item_id + "' or item_desc='" + item_id + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetModeOfComplaintById: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetCurrencyCode()
        {
            DropdownList objProductList = new DropdownList();
            objProductList.lstDropdown = new List<DropdownItems>();

            try
            {
                //string sSsqlstmt = "select ProductId, ProductName from t_product where Active=1";
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Currency Code' order by item_desc asc";

                DataSet dsBranch = Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objProduct = new DropdownItems()
                        {
                            Id = dsBranch.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsBranch.Tables[0].Rows[i]["Name"].ToString()
                        };

                        objProductList.lstDropdown.Add(objProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCurrencyCode: " + ex.ToString());
            }

            return new MultiSelectList(objProductList.lstDropdown, "Id", "Name");          
        }

        public string GetCurrencyNameById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Currency Code' and (item_id='" + item_id + "' or item_desc='" + item_id + "')";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCurrencyNameById: " + ex.ToString());
            }
            return "";
        }


        public string GetCurrencyCodeByBranchId(string BranchId)
        {
            try
            {
                if (BranchId != "")
                {
                    string sSsqlstmt = "SELECT curr_code FROM t_company_branch where id='" + BranchId + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["curr_code"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCurrencyCodeByBranchId: " + ex.ToString());
            }
            return "";
        }
               
        public string GetVersionNumber()
        {
            try
            {
               
                    string sSsqlstmt = "Select Version_no from t_version order by Version_no desc limit 1;";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Version_no"].ToString());
                    }
                
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetVersionNumber: " + ex.ToString());
            }
            return "";
        }
        
        public string GetVersionChanges()
        {
            try
            {

                string sSsqlstmt = "Select Changes from t_version order by Version_no desc limit 1;";
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Changes"].ToString());
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetVersionNumber: " + ex.ToString());
            }
            return "";
        }
        
        public string GetPODocsByOrderNo(string Order_No)
        {
            try
            {
                if (Order_No != "")
                {
                    string sSsqlstmt = "select PO_Docs from t_customer_enquiry where Order_No='" + Order_No + "'";
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["PO_Docs"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetPODocsByOrderNo: " + ex.ToString());
            }
            return "";
        }
        
        public MultiSelectList GetMeetingTypeListbox()
        {
            MeetingTypeList MeetingTypeList = new MeetingTypeList();
            MeetingTypeList.TypeMList = new List<MeetingTypeDetails>();

            try
            {
                DataSet dsMeeting = Getdetails("select id, MeetingName from t_MeetingType");// and CompanyId='" + sCompanyId+"'");
                if (dsMeeting.Tables.Count > 0 && dsMeeting.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsMeeting.Tables[0].Rows.Count; i++)
                    {
                        MeetingTypeDetails Branch = new MeetingTypeDetails()
                        {
                            id = dsMeeting.Tables[0].Rows[i]["Id"].ToString(),
                            MeetingName = dsMeeting.Tables[0].Rows[i]["MeetingName"].ToString()
                        };

                        MeetingTypeList.TypeMList.Add(Branch);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMeetingTypeListbox: " + ex.ToString());
            }

            return new MultiSelectList(MeetingTypeList.TypeMList, "TypeId", "MeetingName");
        }    
        
        public MultiSelectList GetRoles()
        {
            EmployeeRolesList Roleslist = new EmployeeRolesList();
            Roleslist.EmpRolesList = new List<EmployeeRoles>();

            try
            {
                DataSet dsRoles = Getdetails("select Id, RoleName from Roles where active=1 order by RoleName asc");// and CompanyId='" + sCompanyId+"'");
                if (dsRoles.Tables.Count > 0 && dsRoles.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsRoles.Tables[0].Rows.Count; i++)
                    {
                        EmployeeRoles emp = new EmployeeRoles()
                        {
                            id = dsRoles.Tables[0].Rows[i]["Id"].ToString(),
                            Rolename = dsRoles.Tables[0].Rows[i]["RoleName"].ToString()
                        };

                        Roleslist.EmpRolesList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRoles: " + ex.ToString());
            }
            return new MultiSelectList(Roleslist.EmpRolesList, "Id", "RoleName");
        }

        public string GetRolesNameById(string sid)
        {
            try
            {
                if (sid != "")
                {
                    DataSet dsRoles = Getdetails("select RoleName from Roles  where Id='" + sid + "'");
                    if (dsRoles.Tables.Count > 0 && dsRoles.Tables[0].Rows.Count > 0)
                    {
                        return (dsRoles.Tables[0].Rows[0]["RoleName"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRolesNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetMultiRolesNameById(string sid)
        {
            try
            {
                if (sid != "" && sid != null)
                {
                    DataSet dsEmp = Getdetails("SELEct GROUP_CONCAT(m.RoleName) RoleName FROM Roles m  where Id in (" + sid + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["RoleName"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiRolesNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetRolesIdByName(string sRoleName)
        {
            try
            {
                if (sRoleName != "")
                {
                    string sql = "select group_concat(Id) as Id from Roles  where find_in_set(RoleName,'" + sRoleName + "')";
                    DataSet dsRoles = Getdetails(sql);
                    if (dsRoles.Tables.Count > 0 && dsRoles.Tables[0].Rows.Count > 0)
                    {
                        return (dsRoles.Tables[0].Rows[0]["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRolesIdByName: " + ex.ToString());
            }
            return "";
        }

        public string GetAuditNumById(string sAuditnum)
        {
            try
            {
                if (sAuditnum != "")
                {
                    DataSet dsAudit = Getdetails("select AuditNum from t_internal_audit where AuditID='" + sAuditnum + "'");
                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        return (dsAudit.Tables[0].Rows[0]["AuditNum"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditNumById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetAuditNum()
        {
            AuditNumList list = new AuditNumList();
            list.AuditList = new List<AuditNumber>();
            try
            {
                DataSet dsAudit = Getdetails("select distinct AuditNum,AuditID from t_internal_audit where Active=1");

                if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsAudit.Tables[0].Rows.Count; i++)
                    {
                        AuditNumber emp = new AuditNumber()
                        {
                            AuditID = dsAudit.Tables[0].Rows[i]["AuditID"].ToString(),
                            AuditNum = dsAudit.Tables[0].Rows[i]["AuditNum"].ToString()
                        };

                        list.AuditList.Add(emp);

                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditNum: " + ex.ToString());
            }
            return new MultiSelectList(list.AuditList, "AuditID", "AuditNum");
        }

        public string GetLawNoById(string sLegalRequirement_Id)
        {
            try
            {
                if (sLegalRequirement_Id != "")
                {
                    DataSet dsLaw = Getdetails("Select lawNo from t_legalregister where LegalRequirement_Id='" + sLegalRequirement_Id + "'");
                    if (dsLaw.Tables.Count > 0 && dsLaw.Tables[0].Rows.Count > 0)
                    {
                        return (dsLaw.Tables[0].Rows[0]["lawNo"].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLawNoById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetLawNo()
        {
            LeagalRegisterList list = new LeagalRegisterList();
            list.LegalRegisterList = new List<LeagalRegister>();
            try
            {
                DataSet dsLaw = Getdetails("select LegalRequirement_Id,lawNo from t_legalregister where Active=1");

                if (dsLaw.Tables.Count > 0 && dsLaw.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsLaw.Tables[0].Rows.Count; i++)
                    {
                        LeagalRegister Legal = new LeagalRegister()
                        {
                            LegalRequirement_Id = dsLaw.Tables[0].Rows[i]["LegalRequirement_Id"].ToString(),
                            lawNo = dsLaw.Tables[0].Rows[i]["lawNo"].ToString()
                        };
                        list.LegalRegisterList.Add(Legal);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetLawNo: " + ex.ToString());
            }
            return new MultiSelectList(list.LegalRegisterList, "LegalRequirement_Id", "lawNo");
        }

        public MultiSelectList GetDepartmentListbox(string branch="")
        {
            DepartmentList Deptlist = new DepartmentList();
            Deptlist.DeptList = new List<Department>();
            try
            {
                string sql = "";
                if(branch != "" && branch != null)
                {                  
                    sql = "select DeptId, DeptName from t_departments where find_in_set('"+branch+"',branch)";
                }
                else
                {
                    //string curBranch = GetCurrentUserSession().division;
                    //sql = "select DeptId, DeptName from t_departments where find_in_set('" + curBranch + "',branch)";
                    sql = "select DeptId, DeptName from t_departments";
                }
                DataSet dsEmp = Getdetails(sql);

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Department emp = new Department()
                        {
                            Deptid = dsEmp.Tables[0].Rows[i]["DeptId"].ToString(),
                            Deptname = dsEmp.Tables[0].Rows[i]["DeptName"].ToString()
                        };

                        Deptlist.DeptList.Add(emp);

                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDepartmentListbox: " + ex.ToString());
            }
            return new MultiSelectList(Deptlist.DeptList, "DeptId", "DeptName");
        }

        public MultiSelectList GetAuditTypeList()
        {
            AuditTypeList AuditList = new AuditTypeList();
            AuditList.AuditList = new List<AuditType>();
            try
            {
                string[,] sAuditList = new string[3, 2] { { "1", "Internal Audit NC" }, { "2", "Audit Log" }, { "3", "Audit Summary" } };
              
                for (int i = 0; i < 3; i++)
                {
                    AuditType Audit = new AuditType()
                    {
                        AuditTypeid = sAuditList[i, 0].ToString(),
                        AuditTypename = sAuditList[i, 1].ToString()
                    };

                    AuditList.AuditList.Add(Audit);
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditTypeList: " + ex.ToString());
            }
            return new MultiSelectList(AuditList.AuditList, "AuditTypeid", "AuditTypename");
        }

        public MultiSelectList GetDepartmentWithIdListbox()
        {
            DepartmentList Deptlist = new DepartmentList();
            Deptlist.DeptList = new List<Department>();
            try
            {
                string curBranch = GetCurrentUserSession().division;
                DataSet dsEmp = Getdetails("select DeptId, DeptName from t_departments where find_in_set('" + curBranch + "',branch)");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Department emp = new Department()
                        {
                            Deptid = dsEmp.Tables[0].Rows[i]["DeptId"].ToString(),
                            Deptname = dsEmp.Tables[0].Rows[i]["DeptName"].ToString()
                        };

                        Deptlist.DeptList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDepartmentWithIdListbox: " + ex.ToString());
            }
            return new MultiSelectList(Deptlist.DeptList, "DeptId", "DeptName");
        }
       
        public string GetDeptNameById(string Deptid)
        {
            try
            {
                if (Deptid != "")
                {
                    DataSet dsDept = Getdetails("select DeptName from t_departments where deptid='" + Deptid + "'");
                    if (dsDept.Tables.Count > 0 && dsDept.Tables[0].Rows.Count > 0)
                    {
                        return (dsDept.Tables[0].Rows[0]["DeptName"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDeptNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetMultiDeptNameById(string Deptid)
        {
            try
            {
                if (Deptid != "" && Deptid != null)
                {
                    DataSet dsData = Getdetails("SELEct  GROUP_CONCAT(m.DeptName) DeptName FROM t_departments m  where DeptId in (" + Deptid + ")");
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsData.Tables[0].Rows[0]["DeptName"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiDeptNameById: " + ex.ToString());
            }
            return "";
        }
      
        public string GetDeptIDByName(string DeptName)
        {
            try
            {
                if (DeptName != "")
                {
                    DataSet dsDept = Getdetails("select deptid from t_departments where DeptName='" + DeptName + "'");
                    if (dsDept.Tables.Count > 0 && dsDept.Tables[0].Rows.Count > 0)
                    {
                        return (dsDept.Tables[0].Rows[0]["deptid"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetDeptIDByName: " + ex.ToString());
            }
            return "";
        }
        
        public MultiSelectList GetViewsListbox()
        {
            DepartmentList Deptlist = new DepartmentList();
            Deptlist.DeptList = new List<Department>();
            try
            {
                DataSet dsEmp = Getdetails("select DeptId, DeptName from t_views");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Department emp = new Department()
                        {
                            Deptid = dsEmp.Tables[0].Rows[i]["DeptId"].ToString(),
                            Deptname = dsEmp.Tables[0].Rows[i]["DeptName"].ToString()
                        };

                        Deptlist.DeptList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetViewsListbox: " + ex.ToString());
            }
            return new MultiSelectList(Deptlist.DeptList, "DeptId", "DeptName");
        }
        
        public string GetISONameById(string StdId)
        {
            try
            {
                if (StdId != "")
                {
                    DataSet dsISO = Getdetails("select IsoName from t_isostandards where StdId='" + StdId + "' and Active=1");
                    if (dsISO.Tables.Count > 0 && dsISO.Tables[0].Rows.Count > 0)
                    {
                        return (dsISO.Tables[0].Rows[0]["IsoName"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetISONameById: " + ex.ToString());
            }
            return "";
        }

       
                
        public string GetEmployeeHrEmpNoById(string Empid)
        {            
            try
            {
                string sSsqlstmt = ("select emp_no from t_hr_employee where emp_id = '" + Empid + "'");
                DataSet dsData = Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["emp_no"].ToString());
                }                
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetEmployeeEmpNoById: " + ex.ToString());
            }
            return "";
        }
                
        public string GetEmployeeCompEmpIdByEmpId(string Empid) // For access priviledges
        {
            try
            {
                if (Empid != "")
                {
                    string sSsqlstmt = ("select CompEmpId from t_employee where EmpID = '" + Empid + "'");
                    DataSet dsData = Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["CompEmpId"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetEmployeeCompEmpIdByEmpId: " + ex.ToString());
            }
            return "";
        }
        
        public DataSet GetCustomerPersonDetailsById(string ContactsId)
        {
            DataSet dsData = new DataSet();
            try
            {
              dsData = Getdetails("select PhoneNumber, EmailId from t_customer_info_contacts where ContactsId='" + ContactsId + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData);
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetCustomerPersonDetailsById: " + ex.ToString());
            }
            return dsData;
        }
           
        public string GetMultiWorkLocationById(string Work_Location)
        {
            try
            {
                if (Work_Location != "" && Work_Location != null)
                {
                    DataSet dsEmp = Getdetails("SELEct  GROUP_CONCAT(b.BranchName) BranchName FROM t_company_branch b  where id in (" + Work_Location + ")");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsEmp.Tables[0].Rows[0]["BranchName"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiWorkLocationById: " + ex.ToString());
            }
            return "";
        }

        public string GetMultiRoleById(string Role)
        {
            try
            {
                if (Role != "" && Role != null)
                {
                    DataSet dsRole = Getdetails("SELEct  GROUP_CONCAT(r.RoleName) RoleName FROM roles r where Id in (" + Role + ")");
                    if (dsRole.Tables.Count > 0 && dsRole.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsRole.Tables[0].Rows[0]["RoleName"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return sDesc;
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetMultiRoleById: " + ex.ToString());
            }
            return "";
        }
      
        public string GetIsoStdDescriptionById(string Isoid)
        {
            try
            {
                if (Isoid != "" && Isoid != null)
                {
                    DataSet dsIso = Getdetails("SELEct  GROUP_CONCAT(m.Descriptions) Descriptions from t_isostandards m  where StdId in (" + Isoid + ") and Active=1");
                    if (dsIso.Tables.Count > 0 && dsIso.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsIso.Tables[0].Rows[0]["Descriptions"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return (sDesc);
                    }
                }
               
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetIsoStdDescriptionById: " + ex.ToString());
            }
            return "";
        }

        public string GetIsoStdNamebyId(string Isoid)
        {
            try
            {
                if (Isoid != "" && Isoid != null)
                {
                    DataSet dsIso = Getdetails("SELEct  GROUP_CONCAT(m.IsoName) IsoName from t_isostandards m  where StdId in (" + Isoid + ") and Active=1");
                    if (dsIso.Tables.Count > 0 && dsIso.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsIso.Tables[0].Rows[0]["IsoName"].ToString();
                        if (sDesc != "" && sDesc.Contains(','))
                        {
                            return sDesc.Replace(",", ", ");
                        }
                        return (sDesc);
                    }
                }

            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetIsoStdNamebyId: " + ex.ToString());
            }
            return "";
        }

        public string GetAuditNoById(string sAuditID)
        {
            try
            {
                DataSet dsData = Getdetails("SELECT AuditNum FROM t_internal_audit where AuditID='" + sAuditID + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["AuditNum"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditNoById: " + ex.ToString());
            }
            return "";
        }

        public string GetAuditDivisionById(string sAuditID)
        {
            try
            {
                DataSet dsData = Getdetails("SELECT AuditLocation FROM t_internal_audit where AuditID='" + sAuditID + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return GetMultiCompanyBranchNameById(dsData.Tables[0].Rows[0]["AuditLocation"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditDivisionById: " + ex.ToString());
            }
            return "";
        }

        public string GetIsoStdNameById(string Isoid)
        {
            try
            {
                if (Isoid != "" && Isoid != null)
                {
                    DataSet dsIso = Getdetails("SELEct  GROUP_CONCAT(m.IsoName) IsoName from t_isostandards m  where StdId in (" + Isoid + ") and Active=1");
                    if (dsIso.Tables.Count > 0 && dsIso.Tables[0].Rows.Count > 0)
                    {
                        string sDesc = dsIso.Tables[0].Rows[0]["IsoName"].ToString();
                        //if (sDesc != "" && sDesc.Contains(','))
                        //{
                        //    return sDesc.Replace(",", ", ");
                        //}
                        return (sDesc);
                    }
                }
                
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetIsoStdNameById: " + ex.ToString());
            }
            return "";
        }
             

        public MultiSelectList GetIsoStdListbox()
        {
            ISOStdList IsoStdlist = new ISOStdList();
            IsoStdlist.IsoStdList = new List<ISOStds>();

            try
            {
                DataSet dsEmp = Getdetails("select StdId, IsoName from t_isostandards where FIND_IN_SET(stdid, (select Audit_Criteria from t_CompanyInfo)) and Active=1");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        ISOStds Isostd = new ISOStds()
                        {
                            StdId = dsEmp.Tables[0].Rows[i]["StdId"].ToString(),
                            IsoName = dsEmp.Tables[0].Rows[i]["IsoName"].ToString()
                        };

                        IsoStdlist.IsoStdList.Add(Isostd);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetIsoStdListbox: " + ex.ToString());
            }
            return new MultiSelectList(IsoStdlist.IsoStdList, "StdId", "IsoName");
        }
        public MultiSelectList GetAuditCriteria()
        {
            AuditCriteriaList IsoStdlist = new AuditCriteriaList();
            IsoStdlist.IsoStdList = new List<AuditCriteria>();

            try
            {
                DataSet dsEmp = Getdetails("select StdId, Descriptions from t_isostandards where Active=1");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        AuditCriteria Isostd = new AuditCriteria()
                        {
                            StdId = dsEmp.Tables[0].Rows[i]["StdId"].ToString(),
                            Descriptions = dsEmp.Tables[0].Rows[i]["Descriptions"].ToString()
                        };

                        IsoStdlist.IsoStdList.Add(Isostd);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditCriteria: " + ex.ToString());
            }
            return new MultiSelectList(IsoStdlist.IsoStdList, "StdId", "Descriptions");
        }
        public MultiSelectList GetIsoStdListboxIn(string sIsoIds)
        {
            ISOStdList IsoStdlist = new ISOStdList();
            IsoStdlist.IsoStdList = new List<ISOStds>();
            try
            {
                if (sIsoIds != "")
                {
                    DataSet dsEmp = Getdetails("select StdId, IsoName from t_isostandards where stdid in (" + sIsoIds + ") and Active=1");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                        {
                            ISOStds Isostd = new ISOStds()
                            {
                                StdId = dsEmp.Tables[0].Rows[i]["StdId"].ToString(),
                                IsoName = dsEmp.Tables[0].Rows[i]["IsoName"].ToString()
                            };

                            IsoStdlist.IsoStdList.Add(Isostd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetIsoStdListboxIn: " + ex.ToString());
            }
            return new MultiSelectList(IsoStdlist.IsoStdList, "StdId", "IsoName");
        }

        public MultiSelectList GetAllIsoStdListbox()
        {
            ISOStdList IsoStdlist = new ISOStdList();
            IsoStdlist.IsoStdList = new List<ISOStds>();
            try
            {
                DataSet dsEmp = Getdetails("select StdId, IsoName from t_isostandards where Active=1");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        ISOStds Isostd = new ISOStds()
                        {
                            StdId = dsEmp.Tables[0].Rows[i]["StdId"].ToString(),
                            IsoName = dsEmp.Tables[0].Rows[i]["IsoName"].ToString()
                        };

                        IsoStdlist.IsoStdList.Add(Isostd);
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAllIsoStdListbox: " + ex.ToString());
            }
            return new MultiSelectList(IsoStdlist.IsoStdList, "StdId", "IsoName");
        }
              

        public List<string> GetAuditTimeInHour(bool bIncludeZero=false)
        {
            List<string> lstIsoStd = new List<string>();
            try
            {
                int StartIndex = 0;
                if (bIncludeZero)
                {
                    StartIndex = -1;
                }
                for (int i = StartIndex; i < 25; i++)
                {
                    lstIsoStd.Add(i.ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetAuditTimeInHour: " + ex.ToString());
            }
            return lstIsoStd;
        }

        public List<string> GetAuditTimeInMin()
        {
            List<string> lstIsoStd = new List<string>();
            lstIsoStd.Add("00");
            lstIsoStd.Add("05");
            lstIsoStd.Add("10");
            lstIsoStd.Add("15");
            lstIsoStd.Add("20");
            lstIsoStd.Add("25");
            lstIsoStd.Add("30");
            lstIsoStd.Add("35");
            lstIsoStd.Add("40");
            lstIsoStd.Add("45");
            lstIsoStd.Add("50");
            lstIsoStd.Add("55");
           
            return lstIsoStd;
        }

        public void CreateUserSession(string sempid, string sCompEmpId, string sfirstname, string srole, string sProfilePic, string sWork_Location,string sdivision,
           string sDeptID, string sDesignation, string sDeptInCharge, string sAudit_Criteria, string sEvents, string sDocuments, string sInternalDoc,
           string sExternalDoc, string sRecord, string sDocChangeReq, string sDocTrack, string sObjKPI, string sObjectives, string sKpi,
           string sRiskMgmt, string sChangeMgmt, string sContextOrganise,string sParties, string sIssues, string sRisk, string sHazardRiskReg, string sHR,
           string sEmp, string sEmpPerfEval, string sCompetancy, string sOrgChart, string sManHour, string sExitEmp, string sVisitor,
           string sLeaveMgmt, string sLeaveCredit, string sAdjustLeave, string sApplyLeave, string sLeaveMaster, string sHoliday, string sATSS,
           string sATS, string sHseATS, string sMeeting, string sMAgenda, string sMSchedule, string sMUnplaned, string sSuppliers,
           string sSupplier, string sDLog, string sSupplierPer, string sProviders, string sSupplierRate, string sCustMgmt, string sVisits, string sAddCust,
           string sComplaints, string sSurveyQues, string sUploadSurvey, string sCustReturnProcuct, string sBidding, string sTrainingOri, string sAddTopic, string sPerftraining,
           string sEmpTrainingOri, string sAudit, string sInterAudit, string sExterAudit, string sExtAuditRpt, string sCustAudit,
           string sRaiseNc, string sInterChecklist, string sAuditChecklist, string sHSE, string sIncidentRpt, string sNearMissRept,
           string sEmergPlan, string sToolTalk, string sSafetyLog, string sPpeLog, string sHseInsp, string sAirNoise,
           string sWaste, string sObservCard, string sHseIndu, string sFirstBox, string sFireInspection, string sProject, string sEquip,
           string sMaintenance, string sCalibration, string sLegalReg, string sLegal, string sCertificates, string sTraining,
           string sTrainingList, string sTrainingCal, string sReport, string sRept, string sDashRept, string sMISRept,
           string sPermits, string sAccessPermit, string sWorkPermit, string sSettings, string sCompany, string sDept,
           string sUser, string sDropDown, string sEmpRole, string sISOStd, string sBranchTree, string sTRA, string sResConsump,
           string sNC, string sPortal, string ssub_cr, string saccess_portal, string sportal_userlog,string sLocationTree)
        {
            var session = HttpContext.Current.Session;
            DateTime sstarttime = DateTime.Now;
            session["UserCredentials"] = new UserCredentials()
            {
                firstname = sfirstname,
                empid = sempid,
                CompEmpId = sCompEmpId,
                role = srole,
                ProfilePic = sProfilePic,
                Work_Location = sWork_Location,
                division = sdivision,
                DeptID = sDeptID,
                Designation = sDesignation,
                DeptInCharge = sDeptInCharge,
                Audit_Criteria = sAudit_Criteria,
                Events = sEvents,
                Documents = sDocuments,
                InternalDoc = sInternalDoc,
                ExternalDoc = sExternalDoc,
                Record = sRecord,
                DocChangeReq = sDocChangeReq,
                DocTrack = sDocTrack,
                ObjKPI = sObjKPI,
                Objectives = sObjectives,
                Kpi = sKpi,
                RiskMgmt = sRiskMgmt,
                ChangeMgmt = sChangeMgmt,
                ContextOrganise = sContextOrganise,
                Parties = sParties,
                Issues = sIssues,
                Risk = sRisk,
                HazardRiskReg = sHazardRiskReg,
                HR = sHR,
                Emp = sEmp,
                EmpPerfEval = sEmpPerfEval,
                Competancy = sCompetancy,
                OrgChart = sOrgChart,
                ManHour = sManHour,
                ExitEmp = sExitEmp,
                Visitor = sVisitor,
                LeaveMgmt = sLeaveMgmt,
                LeaveCredit = sLeaveCredit,
                AdjustLeave = sAdjustLeave,
                ApplyLeave = sApplyLeave,
                LeaveMaster = sLeaveMaster,
                Holiday = sHoliday,
                ATSS = sATSS,
                ATS = sATS,
                HseATS = sHseATS,
                Meeting = sMeeting,
                MAgenda = sMAgenda,
                MSchedule = sMSchedule,
                MUnplaned = sMUnplaned,
                Suppliers = sSuppliers,
                Supplier = sSupplier,
                DLog = sDLog,
                SupplierPer = sSupplierPer,
                Providers = sProviders,
                SupplierRate = sSupplierRate,
                CustMgmt = sCustMgmt,
                Visits = sVisits,
                AddCust = sAddCust,
                Complaints = sComplaints,
                SurveyQues = sSurveyQues,
                UploadSurvey = sUploadSurvey,
                CustReturnProcuct = sCustReturnProcuct,
                Bidding = sBidding,
                TrainingOri = sTrainingOri,
                AddTopic = sAddTopic,
                Perftraining = sPerftraining,
                EmpTrainingOri = sEmpTrainingOri,
                Audit = sAudit,
                InterAudit = sInterAudit,
                ExterAudit = sExterAudit,
                ExtAuditRpt = sExtAuditRpt,
                CustAudit = sCustAudit,
                RaiseNc = sRaiseNc,
                InterChecklist = sInterChecklist,
                AuditChecklist = sAuditChecklist,
                HSE = sHSE,
                IncidentRpt = sIncidentRpt,
                NearMissRept = sNearMissRept,
                EmergPlan = sEmergPlan,
                ToolTalk = sToolTalk,
                SafetyLog = sSafetyLog,
                PpeLog = sPpeLog,
                HseInsp = sHseInsp,
                AirNoise = sAirNoise,
                Waste = sWaste,
                ObservCard = sObservCard,
                HseIndu = sHseIndu,
                FirstBox = sFirstBox,
                FireInspection = sFireInspection,
                Project = sProject,
                Equip = sEquip,
                Maintenance = sMaintenance,
                Calibration = sCalibration,
                LegalReg = sLegalReg,
                Legal = sLegal,
                Certificates = sCertificates,
                Training = sTraining,
                TrainingList = sTrainingList,
                TrainingCal = sTrainingCal,
                Report = sReport,
                Rept = sRept,
                DashRept = sDashRept,
                MISRept = sMISRept,
                Permits = sPermits,
                AccessPermit = sAccessPermit,
                WorkPermit = sWorkPermit,
                Settings = sSettings,
                Company = sCompany,
                Dept = sDept,
                User = sUser,
                DropDown = sDropDown,
                EmpRole = sEmpRole,
                ISOStd = sISOStd,
                BranchTree = sBranchTree,
                TRA = sTRA,
                ResConsump = sResConsump,
                NC = sNC,
                Portal = sPortal,
                sub_cr = ssub_cr,
                access_portal = saccess_portal,
                portal_userlog = sportal_userlog,
                LocationTree= sLocationTree
            };
        }
        
        
        public UserCredentials GetCurrentUserSession()
        {
            UserCredentials objUser = new UserCredentials();
            if (HttpContext.Current.Session["UserCredentials"] != null)
            {   
                objUser = (UserCredentials)HttpContext.Current.Session["UserCredentials"];
             }
            else
            {
                try
                {
                    //HttpContext.Current.Response.Redirect("~/Account/Login", true);
                    //HttpContext.Current.Response.End();
                    //HttpContext.Current.Session.Abandon();
              
                }
                catch (Exception ex)
                {
                    AddFunctionalLog("Exception in GetCurrentUserSession: " + ex.ToString());
                }

            }    
            return objUser;
        }

        public string GetRoleName(string sCurrentUserRole)
        {
            try
            {
                DataSet dsRoles = Getdetails("select Rolename from roles where active=1 and id='" + sCurrentUserRole + "'");

                if (dsRoles.Tables.Count > 0 && dsRoles.Tables[0].Rows.Count > 0)
                {
                    return dsRoles.Tables[0].Rows[0]["Rolename"].ToString();
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRoleName: " + ex.ToString());
            }
            return "";
        }
       
        public void ClearUserSession()
        {
            HttpContext.Current.Session.Abandon();

            //session["UserCredentials"].;
        }
        public bool SendmailNew(string sToEmailid, string sSubject, string sBody, HttpPostedFileBase sFilename, string sCCList = "", string sBccList = "")
        {
            try
            {
                string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"].ToString().Trim();//"mail.almanalmgt.com";
                int portNumber = 0;

                if (int.TryParse(ConfigurationManager.AppSettings["portNumber"].ToString().Trim(), out portNumber))
                {

                }//587;
                 //bool enableSSL = false;
                bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"].ToString().Trim());

                string sLoggedEmail = "";
              
                     sLoggedEmail = GetHrEmpEmailIdById(GetCurrentUserSession().empid);
                
                string emailFrom = ConfigurationManager.AppSettings["emailFrom"].ToString().Trim(); //"msmesupport@almanalmgt.com";//
                string password = ConfigurationManager.AppSettings["Pwd"].ToString().Trim(); //"msme@123";//

                //string smtpAddress = "mailv.emirates.net.ae";
                //int portNumber = 25;
                //bool enableSSL = false;

                //string emailFrom = Properties.Settings.Default.EmailId.Trim();
                //string password = "flexiflo2017";//Properties.Settings.Default.Pwd.Trim();


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    if (sToEmailid != "")
                    {
                        mail.To.Add(sToEmailid);
                    }
                    else
                    {
                        mail.To.Add(sLoggedEmail);
                    }
                   
                    mail.Subject = sSubject;
                    mail.Body = sBody;
                    mail.IsBodyHtml = true;
                    if (sCCList != "")
                    {
                        mail.CC.Add(sCCList);
                    }

                    if (sBccList != "")
                    {
                        mail.Bcc.Add(sBccList);
                    }
                    // Can set to false, if you are sending pure text.


                    if (sFilename != null)
                    {
                        string fileName = Path.GetFileName(sFilename.FileName);
                        mail.Attachments.Add(new Attachment(sFilename.InputStream, fileName));
                    }
                    // if (sFilename != "")
                    // {
                    //    if (File.Exists(sFilename))
                    //    {
                    //      mail.Attachments.Add(new Attachment(sFilename));
                    //     }
                    //  }
                    SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);

                    smtp.Credentials = new System.Net.NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in Sendmail: " + ex.ToString());
            }
            return true;

        }
        public bool SendmailReview(string sToEmailid, System.IO.FileStream fsSource,string path,string sSubject, string sBody, string sFilename, string sCCList = "", string sBccList = "")
        {
            try
            {
                string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"].ToString().Trim();//"mail.almanalmgt.com";
                int portNumber = 0;
                string sLoggedEmail = "";
             
                     sLoggedEmail = GetHrEmpEmailIdById(GetCurrentUserSession().empid);
                
                if (int.TryParse(ConfigurationManager.AppSettings["portNumber"].ToString().Trim(), out portNumber))
                {

                }//587;
                 //bool enableSSL = false;
                bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"].ToString().Trim());

                string emailFrom = ConfigurationManager.AppSettings["emailFrom"].ToString().Trim(); //"msmesupport@almanalmgt.com";//
                string password = ConfigurationManager.AppSettings["Pwd"].ToString().Trim(); //"msme@123";//

                //string smtpAddress = "mailv.emirates.net.ae";
                //int portNumber = 25;
                //bool enableSSL = false;

                //string emailFrom = Properties.Settings.Default.EmailId.Trim();
                //string password = "flexiflo2017";//Properties.Settings.Default.Pwd.Trim();


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    if (sToEmailid != "")
                    {
                        mail.To.Add(sToEmailid);
                    }
                    else
                    {
                        mail.To.Add(sLoggedEmail);
                    }
                   
                    mail.Subject = sSubject;
                    mail.Body = sBody;
                    mail.IsBodyHtml = true;
                    if (sCCList != "")
                    {
                        mail.CC.Add(sCCList);
                    }

                    if (sBccList != "")
                    {
                        mail.Bcc.Add(sBccList);
                    }
                    // Can set to false, if you are sending pure text.



                    //  if (fsSource != "")
                    //  {
                    //    if (File.Exists(fsSource))
                    //    {
                    mail.Attachments.Add(new Attachment(fsSource,path,MediaTypeNames.Application.Octet));
                    //   }
                    // }
                    SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);

                    smtp.Credentials = new System.Net.NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in Sendmail: " + ex.ToString());
            }
            return true;

        }
        public bool SendmailApprove(string sToEmailid, System.IO.Stream fsSource, string path, string sSubject, string sBody, string sFilename, string sCCList = "", string sBccList = "")
        {
            try
            {
                string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"].ToString().Trim();//"mail.almanalmgt.com";
                int portNumber = 0;

                if (int.TryParse(ConfigurationManager.AppSettings["portNumber"].ToString().Trim(), out portNumber))
                {

                }//587;
                 //bool enableSSL = false;
                bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"].ToString().Trim());

                string sLoggedEmail = "";
               
                     sLoggedEmail = GetHrEmpEmailIdById(GetCurrentUserSession().empid);
                
                string emailFrom = ConfigurationManager.AppSettings["emailFrom"].ToString().Trim(); //"msmesupport@almanalmgt.com";//
                string password = ConfigurationManager.AppSettings["Pwd"].ToString().Trim(); //"msme@123";//

                //string smtpAddress = "mailv.emirates.net.ae";
                //int portNumber = 25;
                //bool enableSSL = false;

                //string emailFrom = Properties.Settings.Default.EmailId.Trim();
                //string password = "flexiflo2017";//Properties.Settings.Default.Pwd.Trim();

                
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    if (sToEmailid != "")
                    {
                        mail.To.Add(sToEmailid);
                    }
                    else
                    {
                        mail.To.Add(sLoggedEmail);
                    }
                   
                    mail.Subject = sSubject;
                    mail.Body = sBody;
                    mail.IsBodyHtml = true;
                    if (sCCList != "")
                    {
                        mail.CC.Add(sCCList);
                    }

                    if (sBccList != "")
                    {
                        mail.Bcc.Add(sBccList);
                    }
                    // Can set to false, if you are sending pure text.



                    //  if (fsSource != "")
                    //  {
                    //    if (File.Exists(fsSource))
                    //    {
                    mail.Attachments.Add(new Attachment(fsSource, path, MediaTypeNames.Application.Octet));
                    //   }
                    // }
                    SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);

                    smtp.Credentials = new System.Net.NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in Sendmail: " + ex.ToString());
            }
            return true;

        }
       
        public bool Sendmail(string sToEmailid,string sSubject, string sBody, string sFilename, string sCCList = "", string sBccList = "")
        {
            try
            {
                string smtpAddress = ConfigurationManager.AppSettings["smtpAddress"].ToString().Trim();//"mail.almanalmgt.com";
                int portNumber = 0;

                if (int.TryParse(ConfigurationManager.AppSettings["portNumber"].ToString().Trim(), out portNumber))
                {
 
                }//587;
                 //bool enableSSL = false;
                bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"].ToString().Trim());

                string sLoggedEmail = "";
                
                  sLoggedEmail = GetHrEmpEmailIdById(GetCurrentUserSession().empid);
                
                string emailFrom = ConfigurationManager.AppSettings["emailFrom"].ToString().Trim(); //"msmesupport@almanalmgt.com";//
                string password = ConfigurationManager.AppSettings["Pwd"].ToString().Trim(); //"msme@123";//

                //string smtpAddress = "mailv.emirates.net.ae";
                //int portNumber = 25;
                //bool enableSSL = false;

                //string emailFrom = Properties.Settings.Default.EmailId.Trim();
                //string password = "flexiflo2017";//Properties.Settings.Default.Pwd.Trim();


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    if (sToEmailid != "")
                    {
                        mail.To.Add(sToEmailid);
                    }
                    else
                    {
                        mail.To.Add(sLoggedEmail);
                    }
                   
                    mail.Subject = sSubject;
                    mail.Body = sBody;
                    mail.IsBodyHtml = true;
                    if (sCCList != "")
                    {
                        mail.CC.Add(sCCList);
                    }

                    if (sBccList != "")
                    {
                        mail.Bcc.Add(sBccList);
                    }
                  
                    // Can set to false, if you are sending pure text.

                    if (sFilename != "")
                   {                        
                        if (sFilename.Contains(','))
                        {
                            string[] varfile = sFilename.Split(',');
                            
                            for (int i = 0; i < varfile.Length; i++)
                            {
                                if (File.Exists(varfile[i]))
                                {
                                    mail.Attachments.Add(new Attachment(varfile[i]));
                                }
                             }
                        }
                     else{
                            if (File.Exists(sFilename))
                            {
                                mail.Attachments.Add(new Attachment(sFilename));
                            }
                        }
                      
                    }

                    SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);

                    smtp.Credentials = new System.Net.NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    //smtp.Send(mail);

                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable)
                            {
                                AddFunctionalLog("Delivery failed - retrying in 5 seconds.");
                                System.Threading.Thread.Sleep(5000);
                                smtp.Send(mail);
                            }
                            else
                            {
                                AddFunctionalLog("Failed to deliver message to {0}",
                                    ex.InnerExceptions[i].FailedRecipient);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddFunctionalLog("Exception caught in RetryIfBusy(): {0}",
                                ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in Sendmail: " + ex.ToString());
            }
            return true;
        }

        public Dictionary<string, string> FormEmailBody(string sUsername, string sType, string sHeader, string sExtraMessage = "", string sTitle = "")
        {
            string body = string.Empty;
            Dictionary<string, string> dicEmailContent = new Dictionary<string, string>();
            dicEmailContent.Add("subject", "");
            dicEmailContent.Add("body", "");

            sUsername = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sUsername.ToLower());
            string sFilePath = "~/Views/EmailTemplate/EmailTemplate.html";
          
            try
            {
                //using streamreader for reading my htmltemplate   
                DataSet dsEmailXML = new DataSet();
                dsEmailXML.ReadXml(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                {
                    using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(sFilePath)))
                    {
                        body = reader.ReadToEnd();
                    }                    

                    body = body.Replace("{FromMsg}", "");
                    body = body.Replace("{UserName}", sUsername); //replacing the required things  

                    //body = body.Replace("{Title}", dsEmailXML.Tables[sType].Rows[0]["title"].ToString());
                    body = body.Replace("{Title}", sTitle);

                    body = body.Replace("{message}", dsEmailXML.Tables[sType].Rows[0]["message"].ToString());
                    body = body.Replace("{extramessage}", sExtraMessage);
                    body = body.Replace("{content}",sHeader);

                    dicEmailContent["subject"] = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    dicEmailContent["body"] = body;
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in FormEmailBody: " + ex.ToString());
            }
            return dicEmailContent; 
        }

        public DataSet GetCompanyBranch(string sCompId)
        {

            string sSqlstmt = "select tcb.*, item_id, item_desc from t_Company_Branch as tcb, dropdownitems, dropdownheader where CompId="
                + sCompId + " and active=1 and dropdownheader.header_id=dropdownitems.header_id and curr_code=item_id and header_desc='Currency Code'";
            
            return Getdetails(sSqlstmt);
            
        }


        public string GenerateTempPassword()
        {
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";

            string characters = numbers;
            //if (rbType.SelectedItem.Value == "1")
            {
                characters += alphabets + small_alphabets + numbers;
            }
            int length = 8; //Length of password
            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return otp;
        }

        public string checkCompEmpIdExists(string CompEmpId)
        {
            try
            {
                string sSqlstmt = "select Compempid from t_employee where Compempid='" + CompEmpId + "' and active=1";
                DataSet dsEmp = Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return "false";
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in checkCompEmpIdExists: " + ex.ToString());
            }
            return CompEmpId;
        }


     

        public static string Encrypt(string clearText)
        {

            if (clearText != null)
            {
                string EncryptionKey = "manaliso";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }
            else
            {
                return "";
            }
        }


        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "manaliso";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public string[] GetConstantValue(string sType)
        {
            
            switch(sType)
            {
                case "Issue Frequency Evaluation": return new string[] { "Quarterly", "Semi Annually", "Yearly" };
                case "IssueImpact": return new string[] { "Low", "Moderate", "High","Extreme" };
                case "KPIUnit": return new string[] { "Number", "Each", "%" };
                case "NC-RootCause": return new string[] { "Completely", "Partially", "No" };
                case "HazardOP": return new string[] { "Elimination","Substitution","Engineering", "Administrative", "PPE", "General" };
                case "LeaveType": return new string[] { "Full Day", "Half Day" };
                case "OperationType": return new string[] { "Receive", "Issue", "Return" };
                case "SupplierRating": return new string[] { "Excellent", "Good","Average","Poor"};
                case "EvaluationStatus": return new string[] { "Accepted", "UnAccepted"};
                case "AuditCycle": return new string[] { "First Cycle", "Second Cycle","Third Cycle" };
                case "CAPAStatus": return new string[] { "Open", "Closed"};
                case "Emirates": return new string[] { "Abu Dhabi", "Ajman", "Fujairah", "Sharjah", "Dubai", "Ras al-Khaimah", "Umm al-Qaiwain" };
                case "VisitActionStatus": return new string[] { "Open", "Closed" ,"In Progress"};
                case "EnqMode": return new string[] { "Phone Call", "Visit" };
                case "EntityType": return new string[] { "Supplier", "Customer" };  
                case "LeaveOnSite": return new string[] { "No", "Yes" };   
                case "QualityLimit": return new string[] { "No", "Yes" };   
                case "PartyType": return new string[] { "Internal", "External" }; 
                case "Impact": return new string[] { "Positive", "Negative" }; 
                case "IssueType": return new string[] { "Internal", "External"};   
                case "EmergencyType": return new string[] { "Fire", "Medical", "Earth Quick", "Sand Strom" };
                case "TrainingStatus": return new string[] { "No", "Yes" };
                case "OrientationStatus": return new string[] { "Pending", "Completed" };
                case "change_type": return new string[] { "Planned", "Un Planned"};
                case "Action_status": return new string[] { "Pending", "Completed","Cancelled"};
                case "Risk_Type_RiskMgmt": return new string[] { "Risk/Threat", "Opportunity"};
                case "approved_status": return new string[] { "Cancelled", "Rescheduled", "Approved", "Rejected", "Not Yet Reveiwed" };
                case "leave_type": return new string[] { "Medical Leave", "Maternity Leave", "Vacation" };
                case "comply": return new string[] { "No", "Yes" };
                case "applicable": return new string[] { "No", "Yes" };
                case "activeStatus": return new string[] { "No", "Yes" };
                case "frequency_of_evaluation": return new string[] { "Monthly", "Quarterly", "Semi Annually", "Annually" };
                case "RiskLevel": return new string[] { "High", "Medium","Low"};
                case "Risk": return new string[] { "Potential Risk", "Current Risk"};
                case "VisaType": return new string[] {"Mission", "Work", "Tourist"};
                //case "EmpType": return new string[] { "Internal", "External" };
                case "SOType": return new string[] { "Internal", "External", "Mobile van" };
                case "DeptInCharge": return new string[] { "No", "Yes" };
                case "YesNo": return new string[] { "No", "Yes" };
                case "Visa_Type": return new string[] { "Mission", "Work", "Tourist" };
                case "Marital_status": return new string[] { "Married", "UnMarried"};
                case "Gender": return new string[] { "Male", "Female" };
                case "Doctype": return new string[] { "Manual", "Procedure", "Form", "Work Instructions", "HSE Registers" };
                case "ExceptionError": return new string[] { "Sorry something went wrong please try again later or contact the Administrator if issue still perists." };
                case "Freq_of_Eval": return new string[] { "Monthly", "Quarterly", "Semi Annually", "Annually","Ongoing"};
                case "Status_Obj_Eval": return new string[] { "Not Achieved", "In Progress", "Achieved" };
                case "Status": return new string[] { "Active", "In Active" };
                case "calibration_status" : return new string[] { "Accepted", "Rejected" };
                case "AuditStatus": return new string[] { "In Progress","Closed" };
                case "Findingcategory": return new string[] { "Major", "Minor", "Observation", "Area of Concern", "Opportunity for Improvment"/*,"No findings"*/};
                case "CustType": return new string[] { "Customer", "Supplier" };
                case "ApprovalCriteria": return new string[] { "Previous Experience", "Branded", "Sample Approval", "Customer Recommended","Already approved vendor" };
                case "RequestStatus": return new string[] { "Pending", "Approved", "Rejected" };
                case "Training_Status": return new string[] { "Not Completed", "Completed", "Re-Scheduled", "Cancelled" };
                case "Eval_Method": return new string[] { "On Job Evaluation", "Performance Evaluation", "Test" };
                case "Cert_Status": return new string[] { "No Certificate", "Training certificate issued" };
                case "MitigationStatus": return new string[] { "No Plan", "Planned" };
                case "ProposalStatus": return new string[] { "Got Order", "Pending", "Lost", "Cancelled", "Forwarded to HO", "Forwarded to Branch", "Regret" };
                case "ReasonForOrderLost": return new string[] { "High Pricing", "Long Delivery", "Non Availability", "Other" };
                case "RequirementRcvdThru": return new string[] { "Visit to Customer", "Email", "Fax", "Phone", "Walk in Customer", "Repeat Order", "Price List",
                    "Mobile van sales" };
                case "ReasonsforDelay": return new string[] { "Quality Issue", "Purchase Delay", "Payment Delay", "Logistics Delay", "Customer Delay", "Production Delay",
                    "Sales Delay", "Other" };
                case "Months": return new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dece" };
                case "PO Status": return new string[] { "Completed", "Pending", "Foreclose", "Cancelled" };
                case "PRType": return new string[] { "Stock", "Customer" };
                case "Mode": return new string[] { "Air", "Sea", "Road", "Courier" };
                case "Month_Not_Closed": return new string[] { "Month not Closed", "LPO not Recieved" };
                case "LCLFCL": return new string[] { "LCL", "FCL-20", "FCL-40" };
                case "Delv_Doc_Type": return new string[] { "MDN", "Invoice" };
                case "EvaluationOutput": return new string[] { "More than expectation", "Meeting the minimum requirements", "Not adequate" };
                case "Evaluated_Freq": return new string[] { "At the time of joining", "Periodical" };
                case "HSEPerformance": return new string[] { "Conducted", "Postponed" };
                case "DrillPerformance": return new string[] { "Satisfactory", "Need Improvements", "Not Satisfactory" };
                case "InjuryNature": return new string[] { "Minor", "Major", "Fatalities" };
                case "IncidentType": return new string[] { "Near Miss", "Accident", "First Aid Case(FAC)", "Environmental Incidents", "Property Damages", "Road Traffic Accidents(RTA)", "Behavioral Incidents", "Unsafe Acts/Conditions"};
                case "AccidentType": return new string[] { "N0. of personnel with minor injuries (Only First Aid, but no LTI)", "No. of personnel with minor injuries (Treatment in the clinic and there is LTI case)",
                "No. of personnel with major injuries (ong term LTI and / or permanent disability)"};
                case "Activity_Category": return new string[] { "Routine", "Non Routine" };
                case "Risk_Type": return new string[] { "Environmental", "OH & S" };
                case "Activity_Status": return new string[] { "Active", "In-Active" };
                case "OperationalType": return new string[] { "Elimination", "Substitution", "Engineering Control", "Administrative Control", "PPE" };
                case "OrderStatus": return new string[] { "Completed", "Pending" };
                case "RecordsType": return new string[] { "Quality", "Environment", "Health & Safety", "Health & Safety and Environment", 
                    "Quality, Health & Safety and Enviornment", "OSHAD","FSC","PEFC","FSC-PEFC","IMS"};

                case "HSEStatus": return new string[] { "Not started", "In Progress", "Completed" };
                case "ShortFallSource": return new string[] { "HSEMS Internal Audit", "Near Miss (High-High Medium)", "Accident", "Property Damage", "SPC Recommendations",
                "External Audit", "HSE Patrol", "TCM (Technical Commitee Meeting)", "HSE Commitee", "HSE Monthly Meeting", "HSEIA (Health Safety Environment Impact Assessment)",
                "Emergency Drill", "Risk Assessments (HAZID, HAZOP, TRA, SIIMOPS, ...etc)", "Others"};
                case "HSECategory": return new string[] { "Not started", "In Progress", "Completed" };
                case "RiskRank": return new string[] { "Medium", "High Medium", "High" };
                case "SONO": return new string[] {"SO NO Already exists"};
                case "PermitStatus": return new string[] { "Open", "Closed" };
                case "Permitworktype": return new string[] { "Hot Work", "Cold Work" };
                case "Availability": return new string[] { "Available", "Not Available" };
                case "Criticality": return new string[] { "Critical", "Non Critical" };
                case "MaterialOperationType": return new string[] { "Issued", "Received" };
                case "TrainingStaff": return new string[] { "Accept", "Not Accept" };
                case "OpenClose": return new string[] { "Open", "Close" };
                //case "DocLevel": return new string[] { "Level I", "Level II", "Level III", "Level IV" };

                default: return new string[]{};
            }
        }

        public Dictionary<string,string> GetConstantValueKeyValuePair(string sType)
        {
            Dictionary<string, string> dicKeyValue = new Dictionary<string, string>();

            switch (sType)
            {
                case "ExtProviderPerf":

                    dicKeyValue.Add("2", "Reviewed");
                    dicKeyValue.Add("1", "Rejected");
                    return dicKeyValue;
                case "CompetenceEvalApprove":

                    dicKeyValue.Add("4", "Approved");
                    dicKeyValue.Add("3", "Rejected");
                    return dicKeyValue;
                case "CompetenceEvalReview":

                    dicKeyValue.Add("2", "Reviewed");
                    dicKeyValue.Add("1", "Rejected");
                    return dicKeyValue;
                case "InspPlanApprove":

                    dicKeyValue.Add("2", "Approved");
                    dicKeyValue.Add("1", "Rejected");
                    return dicKeyValue;
                case "InspChkApprove":

                    dicKeyValue.Add("4", "Approved");
                    dicKeyValue.Add("3", "Rejected");
                    return dicKeyValue;
                case "InspChkReview":

                    dicKeyValue.Add("2", "Reviewed");
                    dicKeyValue.Add("1", "Rejected");
                    return dicKeyValue;
                case "OFI":

                    dicKeyValue.Add("Approved", "Approved");
                    dicKeyValue.Add("Rejected", "Rejected");
                    return dicKeyValue;
                case "AuditNC":

                    dicKeyValue.Add("2", "Accept");
                    dicKeyValue.Add("1", "Reject");
                    return dicKeyValue;
                case "AuditorStatus":

                    dicKeyValue.Add("2", "Confirm");
                    dicKeyValue.Add("1", "Not Confirm");
                    return dicKeyValue;
                case "AuditStatus":

                    dicKeyValue.Add("2", "Accept");
                    dicKeyValue.Add("1", "Reject");
                    return dicKeyValue;
                case "DocStatus":
                    //dicKeyValue.Add("All", "All");
                    dicKeyValue.Add("1", "Approved");
                    dicKeyValue.Add("0", "Not Approved");
                    return dicKeyValue;

                case "DocStatus2":
                    //dicKeyValue.Add("All", "All");
                    dicKeyValue.Add("1", "Approved");
                    dicKeyValue.Add("0", "Not Approved");
                    dicKeyValue.Add("2", "Rejected");
                    return dicKeyValue;

                case "NotificationPeriod":
                    dicKeyValue.Add("None", "None");
                    dicKeyValue.Add("Days", "Days");
                    dicKeyValue.Add("Weeks", "Weeks");
                    dicKeyValue.Add("Months", "Months");                   
                    return dicKeyValue;

                case "NotificationPeriodMeeting":
                    dicKeyValue.Add("None", "None");
                    dicKeyValue.Add("Days", "Days");
                    dicKeyValue.Add("Weeks", "Weeks");
                    dicKeyValue.Add("Months", "Months");
                    dicKeyValue.Add("Minutes", "Minutes");
                    return dicKeyValue;

                case "RequestStatus":
                    dicKeyValue.Add("0", "Pending");
                    dicKeyValue.Add("1", "Approved");
                    dicKeyValue.Add("2", "Rejected");
                    return dicKeyValue;

                case "Approvestatus":
                    dicKeyValue.Add("All", "All");
                    dicKeyValue.Add("1", "Approved");
                    dicKeyValue.Add("0", "Not Approved");
                    return dicKeyValue;

                case "LeaveApprovestatus":
                    dicKeyValue.Add("All", "All");
                    //  dicKeyValue.Add("0", "Not Yet Reveiwed");
                    dicKeyValue.Add("1", "Approved");
                    dicKeyValue.Add("2", "Rejected");
                    dicKeyValue.Add("3", "Rescheduled");
                    return dicKeyValue;

                default: return dicKeyValue;
            }
        }


        public DataSet GetQualityInspectionEmailList()
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("ItemHandedOverToQuality", con);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetQualityInspectionEmailList: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }


        internal string GetRiskRating(int sValue)
        {
            try
            {
                DataSet dsRisk = Getdetails("SELECT rate_desc, color_code FROM risk_ratings where from_value <= " + sValue + " and to_value >= " + sValue + "");
                if (dsRisk.Tables.Count > 0 && dsRisk.Tables[0].Rows.Count > 0)
                {
                    return (dsRisk.Tables[0].Rows[0]["rate_desc"].ToString().ToUpper() + "&" + dsRisk.Tables[0].Rows[0]["color_code"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRiskRating: " + ex.ToString());
            }
            return "";
        }
        internal string GetRiskRatingForEnv(int sValue)
        {
            try
            {
                DataSet dsRisk = Getdetails("SELECT rate_desc, color_code FROM risk_ratings_env where from_value <= " + sValue + " and to_value >= " + sValue + "");
                if (dsRisk.Tables.Count > 0 && dsRisk.Tables[0].Rows.Count > 0)
                {
                    return (dsRisk.Tables[0].Rows[0]["rate_desc"].ToString().ToUpper() + "&" + dsRisk.Tables[0].Rows[0]["color_code"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRiskRating: " + ex.ToString());
            }
            return "";
        }
        internal string GetRiskRatingForExposure(int sValue)
        {
            try
            {
                DataSet dsRisk = Getdetails("SELECT rate_desc, color_code FROM risk_ratings_hrr where from_value <= " + sValue + " and to_value >= " + sValue + "");
                if (dsRisk.Tables.Count > 0 && dsRisk.Tables[0].Rows.Count > 0)
                {
                    return (dsRisk.Tables[0].Rows[0]["rate_desc"].ToString().ToUpper() + "&" + dsRisk.Tables[0].Rows[0]["color_code"].ToString());
                }
            }
            catch (Exception ex)
            {
                AddFunctionalLog("Exception in GetRiskRating: " + ex.ToString());
            }
            return "";
        }
       
    }

    public class Department
    {
        public string Deptid { get; set; }
        public string Deptname { get; set; }
    }

    public class DepartmentList
    {
        public List<Department> DeptList { get; set; }
    }


    public class DocLevels
    {
        public string Levelid { get; set; }
        public string Levelname { get; set; }
    }

    public class DocLevelsList
    {
        public List<DocLevels> DocLevelList { get; set; }
    }

    public class ISOStds
    {
        public string StdId { get; set; }
        public string IsoName { get; set; }
    }
    public class AuditCriteria
    {
        public string StdId { get; set; }
        public string Descriptions { get; set; }
    }

    public class ISOStdList
    {
        public List<ISOStds> IsoStdList { get; set; }
    }
    public class AuditCriteriaList
    {
        public List<AuditCriteria> IsoStdList { get; set; }
    }
    public class AuditType
    {
        public string AuditTypeid { get; set; }
        public string AuditTypename { get; set; }
    }

    public class AuditTypeList
    {
        public List<AuditType> AuditList { get; set; }
    }

    public class Employee
    {
        public string Empid { get; set; }
        public string Empname { get; set; }
      
    }
    
    public class EmployeeRoles
    {
        public string id { get; set; }
        public string Rolename { get; set; }
    }

    public class EmployeeRolesList
    {
        public List<EmployeeRoles> EmpRolesList { get; set; }
    }

    public class EmployeeList
    {
        public List<Employee> EmpList { get; set; }
    }

    public class UserCredentials
    {
        public string empid { get; set; }
        public string CompEmpId { get; set; }
        public string firstname { get; set; }
        public string role { get; set; }
        public string ProfilePic { get; set; }
        public string CompanyID { get; set; }
        public string Work_Location { get; set; }
        public string division { get; set; }
        public string DeptInCharge { get; set; }
        public string DeptID { get; set; }
        public string Designation { get; set; }
        public string Audit_Criteria { get; set; }       
        public string Events { get; set; }
        public string Documents { get; set; }
        public string InternalDoc { get; set; }
        public string ExternalDoc { get; set; }
        public string Record { get; set; }
        public string DocChangeReq { get; set; }
        public string DocTrack { get; set; }
        public string ObjKPI { get; set; }
        public string Objectives { get; set; }
        public string Kpi { get; set; }
        public string RiskMgmt { get; set; }
        public string ChangeMgmt { get; set; }
        public string ContextOrganise { get; set; }
        public string Parties { get; set; }
        public string Issues { get; set; }
        public string Risk { get; set; }
        public string HazardRiskReg { get; set; }
        public string HR { get; set; }
        public string Emp { get; set; }
        public string EmpPerfEval { get; set; }
        public string Competancy { get; set; }
        public string OrgChart { get; set; }
        public string ManHour { get; set; }
        public string ExitEmp { get; set; }
        public string Visitor { get; set; }
        public string LeaveMgmt { get; set; }
        public string LeaveCredit { get; set; }
        public string AdjustLeave { get; set; }
        public string ApplyLeave { get; set; }
        public string LeaveMaster { get; set; }
        public string Holiday { get; set; }
        public string ATSS { get; set; }
        public string ATS { get; set; }
        public string HseATS { get; set; }
        public string Meeting { get; set; }
        public string MAgenda { get; set; }
        public string MSchedule { get; set; }
        public string MUnplaned { get; set; }
        public string Suppliers { get; set; }
        public string Supplier { get; set; }
        public string DLog { get; set; }
        public string SupplierPer { get; set; }
        public string Providers { get; set; }
        public string SupplierRate { get; set; }
        public string CustMgmt { get; set; }
        public string Visits { get; set; }
        public string AddCust { get; set; }
        public string Complaints { get; set; }
        public string SurveyQues { get; set; }
        public string UploadSurvey { get; set; }
        public string CustReturnProcuct { get; set; }
        public string Bidding { get; set; }
        public string TrainingOri { get; set; }
        public string AddTopic { get; set; }
        public string Perftraining { get; set; }
        public string EmpTrainingOri { get; set; }
        public string Audit { get; set; }
        public string InterAudit { get; set; }
        public string ExterAudit { get; set; }
        public string ExtAuditRpt { get; set; }
        public string CustAudit { get; set; }
        public string RaiseNc { get; set; }
        public string InterChecklist { get; set; }
        public string AuditChecklist { get; set; }
        public string HSE { get; set; }
        public string IncidentRpt { get; set; }
        public string NearMissRept { get; set; }
        public string EmergPlan { get; set; }
        public string ToolTalk { get; set; }
        public string SafetyLog { get; set; }
        public string PpeLog { get; set; }
        public string HseInsp { get; set; }
        public string AirNoise { get; set; }
        public string Waste { get; set; }
        public string ObservCard { get; set; }
        public string HseIndu { get; set; }
        public string FirstBox { get; set; }
        public string FireInspection { get; set; }
        public string Project { get; set; }
        public string Equip { get; set; }
        public string Maintenance { get; set; }
        public string Calibration { get; set; }
        public string LegalReg { get; set; }
        public string Legal { get; set; }
        public string Certificates { get; set; }
        public string Training { get; set; }
        public string TrainingList { get; set; }
        public string TrainingCal { get; set; }
        public string Report { get; set; }
        public string Rept { get; set; }
        public string DashRept { get; set; }
        public string MISRept { get; set; }
        public string Permits { get; set; }
        public string AccessPermit { get; set; }
        public string WorkPermit { get; set; }
        public string Settings { get; set; }
        public string Company { get; set; }
        public string Dept { get; set; }
        public string User { get; set; }
        public string DropDown { get; set; }
        public string EmpRole { get; set; }
        public string ISOStd { get; set; }
        public string BranchTree { get; set; }
        public string TRA { get; set; }
        public string ResConsump { get; set; }
        public string NC { get; set; }
        public string Portal { get; set; }
        public string sub_cr { get; set; }
        public string access_portal { get; set; }
        public string portal_userlog { get; set; }
        public string LocationTree { get; set; }
    }
   
    public class CompanyBranch
    {
        public string BranchId { get; set; }
        public string BranchName { get; set; }
    }

    public class CompanyBranchList
    {
        public List<CompanyBranch> CompBranchList { get; set; }
    }

    public class DocumentRef
    {
        public string idMgmt { get; set; }
        public string DocRef { get; set; }
        public string DocName { get; set; }
    }

    public class DocumentRefList
    {
        public List<DocumentRef> DoctRefList { get; set; }
    }
    public class CompanySupplier
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class CompanySupplierList
    {
        public List<CompanySupplier> CompanySuppList { get; set; }
    }
    public class CountryCode
    {
        public string id_country { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class AuditNumber
    {
        public string AuditID { get; set; }
        public string AuditNum { get; set; }
    }

    public class AuditNumList
    {
        public List<AuditNumber> AuditList { get; set; }
    }
    public class CountryCodeList
    {
        public List<CountryCode> CountryCodeLists { get; set; }

    }
    public class DropdownItems
    {
        public string Id { get; set; }
        public string Name { get; set; }      
    }

    public class DropdownList
    {
        public List<DropdownItems> lstDropdown { get; set; }
    }

    public class SelectItems
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class SelectItemsList
    {
        public List<SelectItems> lstSelectItems { get; set; }
    }

    public class DashboardReport
    {
        public string dashboard_id { get; set; }
        public string dashboard_name { get; set; }
    }

    public class DashboardReportList
    {
        public List<DashboardReport> lstDropdown { get; set; }
    }

    public class LeagalRegister
    {
        public string LegalRequirement_Id { get; set; }
        public string lawNo { get; set; }
    }
    public class LeagalRegisterList
    {
        public List<LeagalRegister> LegalRegisterList { get; set; }
    }
}

public class TransactionCount
{
    public string MonthName { get; set; }
    public int Count { get; set; }

}


