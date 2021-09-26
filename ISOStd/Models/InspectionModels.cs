using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{

    public class InspectionCategoryModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        public string id_inspection_question { get; set; }
        public string Category { get; set; }

        public string GetInspectionCategoryByID(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Inspection Category' and (item_id='" + item_id + "' or item_desc='" + item_id + "')";
                    DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionCategoryByID: " + ex.ToString());
            }
            return "";
        }

        public string getInspectionCategoryIDByName(string item_id)
        {
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                   + "and header_desc='Inspection Category' and item_desc='" + item_id + "'";
                DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Id"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in getInspectionCategoryIDByName: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetInspectionCategoryList()
        {
            DropdownInspectionList objReportList = new DropdownInspectionList();
            objReportList.lstDropdown = new List<DropdownInspectionItems>();
            try
            {
                string sSsqlstmt = "select distinct category as Id,item_desc as Name from  t_inspection_section t,dropdownitems tt where tt.item_id=t.category";

                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownInspectionItems objReport = new DropdownInspectionItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetInspectionCategoryList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public MultiSelectList GetInspectionCategoryListbox()
        {
            DropdownInspectionList objReportList = new DropdownInspectionList();
            objReportList.lstDropdown = new List<DropdownInspectionItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Inspection Category' order by item_desc asc";

                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownInspectionItems objReport = new DropdownInspectionItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetInspectionCategoryListbox: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public MultiSelectList GetInspectionSectionListbox(string sCategory, string sBranch)
        {
            DropdownInspectionList objReportList = new DropdownInspectionList();
            objReportList.lstDropdown = new List<DropdownInspectionItems>();
            try
            {
                string sSsqlstmt = "select id_inspection as Id, Section as Name from t_inspection_section where Category='" + sCategory + "' and  branch='" + sBranch + "' and active=1";

                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownInspectionItems objReport = new DropdownInspectionItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetInspectionSectionListbox: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public MultiSelectList GetInspSectionRespPersonList(string dept)
        {
            DropdownInspectionList objReportList = new DropdownInspectionList();
            objReportList.lstDropdown = new List<DropdownInspectionItems>();
            try
            {
                string sSsqlstmt = "select id_inspection as Id, Section as Name,dept from t_inspection_section where dept='" + dept + "'  and active=1";

                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownInspectionItems objReport = new DropdownInspectionItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString() + "_" + dsReportType.Tables[0].Rows[i]["dept"].ToString() + "_" + objGlobalData.GetDeptNameById(dsReportType.Tables[0].Rows[i]["dept"].ToString())
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspSectionRespPersonList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetSectionNamebyId(string Section_id)
        {
            try
            {
                string sSsqlstmt = "select Section as Name from t_inspection_section where id_inspection='" + Section_id + "'";
                DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSectionNamebyId: " + ex.ToString());
            }
            return "";
        }

        //public MultiSelectList GetInspRespPersonList(string sCategory, string sBranch)
        //{
        //    DropdownInspectionList objReportList = new DropdownInspectionList();
        //    objReportList.lstDropdown = new List<DropdownInspectionItems>();
        //    try
        //    {
        //        string sSsqlstmt = "select id_inspection as Id, Resp_person as Name from t_inspection_section where Category='" + sCategory + "' and  branch='" + sBranch + "'";

        //        DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

        //        if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
        //            {
        //                DropdownInspectionItems objReport = new DropdownInspectionItems()
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
        //        objGlobalData.AddFunctionalLog("Exception in GetInspRespPersonList: " + ex.ToString());
        //    }

        //    return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        //}

        //public string newfucntion(string sCategory, string sBranch)
        //{
        //    string name = null;
        //    try
        //    {
        //        string sSsqlstmt = "select Resp_person as Resp_person from t_inspection_section where Category='" + sCategory + "' and  branch='" + sBranch + "'";

        //        DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

        //        if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
        //            {
        //                name = objGlobalData.GetMultiHrEmpNameById(dsReportType.Tables[0].Rows[i]["Resp_person"].ToString());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in GetInspectionSectionListbox: " + ex.ToString());
        //    }

        //    return name;
        //}

    }

    public class InspctionQuestionModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        public string id_inspection_question { get; set; }

        public string id_inspection_rating { get; set; }

        public string id_inspection { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Section")]
        public string Section { get; set; }

        [Display(Name = "Questions")]
        public string InspectionQuestions { get; set; }

        [Display(Name = "Ratings")]
        public string inspection_rating { get; set; }

        [Display(Name = "Resposible Person")]
        public string Resp_person { get; set; }

        [Display(Name = "Department")]
        public string dept { get; set; }

        //t_inspection_question_master

        [Display(Name = "Id")]
        public string id_question_master { get; set; }

        [Display(Name = "Checklist Ref")]
        public string checklist_ref { get; set; }

        [Display(Name = "Type of Inspection")]
        public string insp_type { get; set; }

        [Display(Name = "Details")]
        public string insp_detail { get; set; }

        [Display(Name = "Area")]
        public string insp_area { get; set; }

        [Display(Name = "Criteria")]
        public string insp_criteria { get; set; }

        [Display(Name = "Status of checklist")]
        public string checklist_status { get; set; }

        [Display(Name = "Dated")]
        public DateTime logged_date { get; set; }

        [Display(Name = "To be Reviewed By")]
        public string reviewed_by { get; set; }

        public string reviewed_status { get; set; }

        public DateTime reviewed_date { get; set; }

        [Display(Name = "Comments")]
        public string reviewer_comments { get; set; }

        [Display(Name = "Document(s)")]
        public string reviewer_upload { get; set; }

        [Display(Name = "To be Approved By")]
        public string approved_by { get; set; }

        [Display(Name = "Approval Status")]
        public string approved_status { get; set; }

        public DateTime approved_date { get; set; }

        [Display(Name = "Comments")]
        public string approver_comments { get; set; }

        [Display(Name = "Document(s)")]
        public string approver_upload { get; set; }

        [Display(Name = "Criticality")]
        public string criticality { get; set; }

        [Display(Name = "Criteria")]
        public string criteria { get; set; }

        [Display(Name = "Logged By")]
        public string logged_by { get; set; }

        [Display(Name = "Rev No.")]
        public string RevNo { get; set; }

        //t_inspection_plan

        [Display(Name = "Id")]
        public string id_inspection_plan { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "Frequency of Inspection")]
        public string insp_freq { get; set; }

        [Display(Name = "Project / Job ")]
        public string project { get; set; }

        [Display(Name = "Date of inspection")]
        public DateTime insp_date { get; set; }

        [Display(Name = "Person Responsible")]
        public string pers_resp { get; set; }

        [Display(Name = "Inspection Status")]
        public string insp_status { get; set; }

        public string id_inspection_date { get; set; }

        public string branch { get; set; }
        public string dept_name { get; set; }

        public string id_question_master_history { get; set; }

        [Display(Name = "Date of Inspection")]
        public DateTime insp_perf_date { get; set; }

        [Display(Name = "Notification to")]
        public string notification_to { get; set; }

        public string id_inspection_perform_checklist { get; set; }

        [Display(Name = "Inspection result")]
        public string insp_result { get; set; }

        [Display(Name = "Details of observation")]
        public string details { get; set; }

        [Display(Name = "Action required")]
        public string action_required { get; set; }

        [Display(Name = "Suggestion")]
        public string suggestion { get; set; }

        [Display(Name = "Document(s)")]
        public string upload { get; set; }

        internal bool FunAddInspectionQuestion(InspctionQuestionModels objInsp, InspctionQuestionList objInspList)
        {
            try
            {

                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string branch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_inspection_question_master (branch,insp_type,insp_detail,dept,Section,insp_area,insp_criteria,logged_by,reviewed_by,approved_by";

                string sFields = "", sFieldValue = "";

                if (objInsp.logged_date != null && objInsp.logged_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", logged_date";
                    sFieldValue = sFieldValue + ", '" + objInsp.logged_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('"+ branch + "','"+insp_type+"','"+insp_detail + "','" + dept + "','" + Section + "','" + insp_area + "','" + insp_criteria + "','" + user + "','" + reviewed_by + "','" + approved_by + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_question_master = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_question_master))
                {
                    string sName = objGlobalData.GetDropdownitemById(insp_type) + "-" + objGlobalData.GetDropdownitemById(insp_criteria);
                    DataSet dsData = objGlobalData.GetReportNo("INSPCHECKLIST", "", sName);

                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        checklist_ref = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_inspection_question_master set checklist_ref='" + checklist_ref + "' where id_question_master='" + id_question_master + "'";
                    objGlobalData.ExecuteQuery(sql1);

                    if (id_question_master > 0)
                    {
                        objInspList.InspectionQstList[0].id_question_master = id_question_master.ToString();
                        FunAddInspQuestList(objInspList);
                        return SendInspChecklistmail(id_question_master,"Inspection Checklist");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspectionQuestion: " + ex.ToString());
            }
            return false;
        }
        //checklist edit
        internal bool FunEditInspectionQuestion(InspctionQuestionModels objInsp, InspctionQuestionList objInspList)
        {
            try
            {
                

                string sSqlstmt = "update t_inspection_question_master set insp_detail='" + insp_detail + "',dept='" + dept + "',Section='" + Section + "',insp_area='" + insp_area + "',reviewed_by='" + reviewed_by + "',approved_by='" + approved_by + "'";

                if (objInsp.logged_date != null && objInsp.logged_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",logged_date='" + objInsp.logged_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_question_master='" + objInsp.id_question_master + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    
                    if (objInspList.InspectionQstList.Count > 0)
                    {
                        objInspList.InspectionQstList[0].id_question_master = id_question_master.ToString();
                        return FunAddInspQuestList(objInspList);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunEditInspectionQuestion: " + ex.ToString());
            }
            return false;
        }
        //checklist rev
        internal bool FunRevInspectionQuestion(InspctionQuestionModels objInsp, InspctionQuestionList objInspList)
        {
            try
            {



                //----------------------------------------

                InspctionQuestionList sobjInsp = new InspctionQuestionList();
                sobjInsp.InspectionQstList = new List<InspctionQuestionModels>();

                string sSqlstmt1 = "select id_inspection_question,id_question_master,InspectionQuestions,criticality,criteria from t_inspection_questions where id_question_master = '" + id_question_master + "'";
                DataSet dsChklist = objGlobalData.Getdetails(sSqlstmt1);
                if (dsChklist.Tables.Count > 0 && dsChklist.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsChklist.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            InspctionQuestionModels objAuditModel = new InspctionQuestionModels
                            {
                                InspectionQuestions = dsChklist.Tables[0].Rows[i]["InspectionQuestions"].ToString(),
                                criticality = dsChklist.Tables[0].Rows[i]["criticality"].ToString(),
                                criteria = dsChklist.Tables[0].Rows[i]["criteria"].ToString(),
                            };
                            sobjInsp.InspectionQstList.Add(objAuditModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in EditInspectionQuestions: " + ex.ToString());
                         
                        }
                    }                   
                }

                string sql = "select id_question_master,branch,checklist_ref,insp_type,insp_detail,dept,Section,insp_area,insp_criteria,checklist_status,logged_date,logged_by,reviewed_by,reviewed_status,reviewed_date,reviewer_comments,reviewer_upload,approved_by,approved_status,approved_date,approver_comments,approver_upload,active,RevNo,chk_valid,invalid_reason,invalid_date,invalid_logged_by"

                        + " from t_inspection_question_master where id_question_master = '" + id_question_master + "'";
                DataSet dsList = objGlobalData.Getdetails(sql);

                
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    DateTime dtDocDate;
                    string invalid_date = "", approved_date = "", reviewed_date = "", logged_date = "";
                    if (dsList.Tables[0].Rows[0]["invalid_date"].ToString() != ""
                       && DateTime.TryParse(dsList.Tables[0].Rows[0]["invalid_date"].ToString(), out dtDocDate))
                    {
                       invalid_date = Convert.ToDateTime(dsList.Tables[0].Rows[0]["invalid_date"].ToString()).ToString("yyyy/MM/dd hh:mm:ss");
                    }
                    else
                    {
                        invalid_date = "NULL";
                    }
                    if (dsList.Tables[0].Rows[0]["approved_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["approved_date"].ToString(), out dtDocDate))
                    {
                        approved_date = Convert.ToDateTime(dsList.Tables[0].Rows[0]["approved_date"].ToString()).ToString("yyyy/MM/dd hh:mm:ss");
                    }
                    else
                    {
                        approved_date = "NULL";
                    }
                    if (dsList.Tables[0].Rows[0]["reviewed_date"].ToString() != ""
                     && DateTime.TryParse(dsList.Tables[0].Rows[0]["reviewed_date"].ToString(), out dtDocDate))
                    {
                        reviewed_date = Convert.ToDateTime(dsList.Tables[0].Rows[0]["reviewed_date"].ToString()).ToString("yyyy/MM/dd hh:mm:ss");
                    }
                    else
                    {
                        reviewed_date = "NULL";
                    }
                    if (dsList.Tables[0].Rows[0]["logged_date"].ToString() != ""
                    && DateTime.TryParse(dsList.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                    {
                        logged_date = Convert.ToDateTime(dsList.Tables[0].Rows[0]["logged_date"].ToString()).ToString("yyyy/MM/dd hh:mm:ss");
                    }
                    else
                    {
                        logged_date = "NULL";
                    }
                    string sql1 = "insert into t_inspection_question_master_history (id_question_master,branch,checklist_ref,insp_type,insp_detail,dept,Section,insp_area,insp_criteria,checklist_status,logged_date,logged_by,reviewed_by,reviewed_status,reviewed_date,reviewer_comments,reviewer_upload,approved_by,approved_status,approved_date,approver_comments,approver_upload,active,RevNo,chk_valid,invalid_reason,invalid_date,invalid_logged_by)"
                        + " values ('"+ dsList.Tables[0].Rows[0]["id_question_master"].ToString() + "'"
                        + ",'" + dsList.Tables[0].Rows[0]["branch"].ToString() + "'"
                         + ",'" + dsList.Tables[0].Rows[0]["checklist_ref"].ToString() + "'"
                          + ",'" + dsList.Tables[0].Rows[0]["insp_type"].ToString() + "'"
                           + ",'" + dsList.Tables[0].Rows[0]["insp_detail"].ToString() + "'"
                            + ",'" + dsList.Tables[0].Rows[0]["dept"].ToString() + "'"
                             + ",'" + dsList.Tables[0].Rows[0]["Section"].ToString() + "'"
                              + ",'" + dsList.Tables[0].Rows[0]["insp_area"].ToString() + "'"
                               + ",'" + dsList.Tables[0].Rows[0]["insp_criteria"].ToString() + "'"
                                + ",'" + dsList.Tables[0].Rows[0]["checklist_status"].ToString() + "'"
                                 + ",'" + logged_date + "'"
                                  + ",'" + dsList.Tables[0].Rows[0]["logged_by"].ToString() + "'"
                                   + ",'" + dsList.Tables[0].Rows[0]["reviewed_by"].ToString() + "'"
                                    + ",'" + dsList.Tables[0].Rows[0]["reviewed_status"].ToString() + "'"
                                     + ",'" + reviewed_date + "'"
                                 + ",'" + dsList.Tables[0].Rows[0]["reviewer_comments"].ToString() + "'"
                                 + ",'" + dsList.Tables[0].Rows[0]["reviewer_upload"].ToString() + "'"
                                 + ",'" + dsList.Tables[0].Rows[0]["approved_by"].ToString() + "'"
                                 + ",'" + dsList.Tables[0].Rows[0]["approved_status"].ToString() + "'"
                                 + ",'" + approved_date + "'"
                                 + ",'" + dsList.Tables[0].Rows[0]["approver_comments"].ToString() + "'"
                                 + ",'" + dsList.Tables[0].Rows[0]["approver_upload"].ToString() + "'"
                                 + ",'" + dsList.Tables[0].Rows[0]["active"].ToString() + "'"
                                  + ",'" + dsList.Tables[0].Rows[0]["RevNo"].ToString() + "'"
                                   + ",'" + dsList.Tables[0].Rows[0]["chk_valid"].ToString() + "'"
                                    + ",'" + dsList.Tables[0].Rows[0]["invalid_reason"].ToString() + "'"
                                     + ","+ invalid_date + ""
                                   + ",'" + dsList.Tables[0].Rows[0]["invalid_logged_by"].ToString() + "'"
                        + ")";
                    int id_question_master_history = 0;
                    if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sql1).ToString(), out id_question_master_history))
                    {

                        if (id_question_master_history > 0)
                        {
                            sobjInsp.InspectionQstList[0].id_question_master_history = id_question_master_history.ToString();
                            FunAddInspQuestHistoryList(sobjInsp);          
                        }
                    }
                    }
                // end


                string sSqlstmt = "update t_inspection_question_master set RevNo='"+ RevNo + "',insp_type='" + insp_type + "',insp_detail='" + insp_detail + "',dept='" + dept + "',Section='" + Section + "',insp_area='" + insp_area + "',insp_criteria='" + insp_criteria + "',reviewed_by='" + reviewed_by + "',approved_by='" + approved_by + "',checklist_status='0',reviewed_status=NULL,reviewed_date=NULL,reviewer_comments=NULL,reviewer_upload=NULL,approved_status=NULL,approved_date=NULL,approver_comments=NULL,approver_upload=NULL";

                if (objInsp.logged_date != null && objInsp.logged_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",logged_date='" + objInsp.logged_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_question_master='" + objInsp.id_question_master + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    string sName = objGlobalData.GetDropdownitemById(insp_type) + "-" + objGlobalData.GetDropdownitemById(insp_criteria);
                    DataSet dsData = objGlobalData.GetReportNo("INSPCHECKLIST", "", sName);

                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        checklist_ref = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_inspection_question_master set checklist_ref='" + checklist_ref + "' where id_question_master='" + id_question_master + "'";
                    objGlobalData.ExecuteQuery(sql1);
                    if (objInspList.InspectionQstList.Count > 0)
                    {
                        objInspList.InspectionQstList[0].id_question_master = id_question_master.ToString();
                        return FunAddInspQuestList(objInspList);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunRevInspectionQuestion: " + ex.ToString());
            }
            return false;
        }
        //checklist history questions
        internal bool FunAddInspQuestHistoryList(InspctionQuestionList objInspList)
        {
            try
            {

                string sSqlstmt = "";


                for (int i = 0; i < objInspList.InspectionQstList.Count; i++)
                {
                  
                    sSqlstmt = sSqlstmt + "insert into t_inspection_questions_history(id_question_master_history,InspectionQuestions,criticality,criteria";

                  
                  
                    sSqlstmt = sSqlstmt + ") values('" + objInspList.InspectionQstList[0].id_question_master_history + "','" + objInspList.InspectionQstList[i].InspectionQuestions + "','" + objInspList.InspectionQstList[i].criticality + "','" + objInspList.InspectionQstList[i].criteria + "'";
                    sSqlstmt = sSqlstmt +  ");";
                
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspQuestHistoryList: " + ex.ToString());
            }
            return false;
        }
        //checklist Mail
        internal bool SendInspChecklistmail(int id_question_master,  string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select id_question_master,checklist_ref,insp_type,reviewed_by,reviewer_comments,approved_by,approver_comments,logged_by"
               + " from t_inspection_question_master  where id_question_master = '" + id_question_master + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["reviewed_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["reviewed_by"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["logged_by"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["approved_by"].ToString());


                    sHeader = "<tr><td colspan=3><b>Checklist Ref:<b></td> <td colspan=3>"
                    + (dsList.Tables[0].Rows[0]["checklist_ref"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Inspection Type:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()) + "</td></tr>";
                       
                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Inspection checklist");
                    sContent = sContent.Replace("{content}", sHeader);
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
                objGlobalData.AddFunctionalLog("Exception in SendInspChecklistmail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddInspQuestList(InspctionQuestionList objInspList)
        {
            try
            {

                string sSqlstmt = "delete from t_inspection_questions where id_question_master='" + objInspList.InspectionQstList[0].id_question_master + "'; ";

             
                for (int i = 0; i < objInspList.InspectionQstList.Count; i++)
                {
                    string sid_inspection_question = "null";
                    sSqlstmt = sSqlstmt + "insert into t_inspection_questions(id_inspection_question,id_question_master,InspectionQuestions,criticality,criteria";

                    string sFieldValue = "",  sValue = "", sStatement = ""; ;
                    if (objInspList.InspectionQstList[i].id_inspection_question != null)
                    {
                        sid_inspection_question = objInspList.InspectionQstList[i].id_inspection_question;
                    }
                      
                    sSqlstmt = sSqlstmt + ") values(" + sid_inspection_question + ",'" + objInspList.InspectionQstList[0].id_question_master + "','" + objInspList.InspectionQstList[i].InspectionQuestions + "','" + objInspList.InspectionQstList[i].criticality + "','" + objInspList.InspectionQstList[i].criteria + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                    + " id_inspection_question= values(id_inspection_question), id_question_master= values(id_question_master), InspectionQuestions = values(InspectionQuestions), criticality= values(criticality), criteria= values(criteria)";                
                    sSqlstmt = sSqlstmt + sValue + ";";
                  
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspQuestList: " + ex.ToString());
            }
            return false;
        }
        //checklist review
        internal bool FunInspChecklistReview(InspctionQuestionModels objModel)
        {
            try
            {
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                
                string sSqlstmt = "update t_inspection_question_master set checklist_status='" + reviewed_status + "',reviewed_status='" + reviewed_status + "',reviewed_date='" + TodayDate + "',reviewer_comments='" + reviewer_comments + "',reviewer_upload='" + reviewer_upload + "'";
               
                sSqlstmt = sSqlstmt + " where id_question_master='" + objModel.id_question_master + "'";
                objGlobalData.ExecuteQuery(sSqlstmt);
                return SendInspChecklistReviewmail(id_question_master, reviewed_status, "Inspection checklist review Status");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunInspChecklistReview: " + ex.ToString());
            }
            return false;
        }
        //checklist review Mail
        internal bool SendInspChecklistReviewmail(string id_question_master, string iStatus, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select id_question_master,checklist_ref,insp_type,reviewed_by,reviewer_comments,approved_by,approver_comments,logged_by"
               + " from t_inspection_question_master  where id_question_master = '" + id_question_master + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["logged_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["logged_by"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["reviewed_by"].ToString())+"," + objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["approved_by"].ToString());
                   
                    if (iStatus == "2") // Approve
                    {
                        sHeader = "<tr><td colspan=3><b>Checklist Ref:<b></td> <td colspan=3>"
                       + (dsList.Tables[0].Rows[0]["checklist_ref"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Inspection Type:<b></td> <td colspan=3>" +objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()) + "</td></tr>"                
                       + "<tr><td colspan=3><b>Review Status:<b></td> <td colspan=3>Reviewed</td></tr>"
                       + "<tr><td colspan=3><b>Reviewed By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["reviewed_by"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Comment:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["reviewer_comments"].ToString()) + "</td></tr>";
                    }
                    else
                    {
                        sHeader = "<tr><td colspan=3><b>Checklist Ref:<b></td> <td colspan=3>"
                        + (dsList.Tables[0].Rows[0]["checklist_ref"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Inspection Type:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Review Status:<b></td> <td colspan=3>Rejected</td></tr>"
                        + "<tr><td colspan=3><b>Reviewed By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["reviewed_by"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Comment:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["reviewer_comments"].ToString()) + "</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Inspection checklist review Status");
                    sContent = sContent.Replace("{content}", sHeader);
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
                objGlobalData.AddFunctionalLog("Exception in SendInspChecklistReviewmail: " + ex.ToString());
            }
            return false;
        }
        //checklist approve
        internal bool FunInspChecklistApprove(InspctionQuestionModels objModel)
        {
            try
            {
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sSqlstmt = "update t_inspection_question_master set checklist_status='" + approved_status + "',approved_status='" + approved_status + "',approved_date='" + TodayDate + "',approver_comments='" + approver_comments + "',approver_upload='" + approver_upload + "'";

                sSqlstmt = sSqlstmt + " where id_question_master='" + objModel.id_question_master + "'";
                if(objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (approved_status == "4")
                    {
                        string sql = "select RevNo from t_inspection_question_master where id_question_master='" + objModel.id_question_master + "'";
                        DataSet dsData = objGlobalData.Getdetails(sql);
                        if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                        {
                           
                            int len = dsData.Tables[0].Rows[0]["RevNo"].ToString().Length; ;
                            if (len > 3)
                            {
                                string stmt = "update t_inspection_question_master set RevNo ='00' where id_question_master='" + objModel.id_question_master + "'";
                                objGlobalData.ExecuteQuery(stmt);

                            }
                         
                        }
                    }
                }
                return SendInspChecklistApprovemail(id_question_master, reviewed_status, "Inspection checklist approve Status");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunInspChecklistApprove: " + ex.ToString());
            }
            return false;
        }
        //checklist review Mail
        internal bool SendInspChecklistApprovemail(string id_question_master, string iStatus, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select id_question_master,checklist_ref,insp_type,reviewed_by,reviewer_comments,approved_by,approver_comments,logged_by"
                + " from t_inspection_question_master  where id_question_master = '" + id_question_master + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["logged_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["logged_by"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["reviewed_by"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["approved_by"].ToString());

                    if (iStatus == "4") // Approve
                    {
                        sHeader = "<tr><td colspan=3><b>Checklist Ref:<b></td> <td colspan=3>"
                       + (dsList.Tables[0].Rows[0]["checklist_ref"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Inspection Type:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Approve Status:<b></td> <td colspan=3>Approved</td></tr>"
                       + "<tr><td colspan=3><b>Approved By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Comment:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["approver_comments"].ToString()) + "</td></tr>";
                    }
                    else
                    {
                        sHeader = "<tr><td colspan=3><b>Checklist Ref:<b></td> <td colspan=3>"
                        + (dsList.Tables[0].Rows[0]["checklist_ref"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Inspection Type:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Approve Status:<b></td> <td colspan=3>Rejected</td></tr>"
                        + "<tr><td colspan=3><b>Approved By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Comment:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["approver_comments"].ToString()) + "</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Inspection checklist approve Status");
                    sContent = sContent.Replace("{content}", sHeader);
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
                objGlobalData.AddFunctionalLog("Exception in SendInspChecklistApprovemail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteInspChecklist(string sid_question_master)
        {
            try
            {
                string sSqlstmt = "update t_inspection_question_master set active=0 where id_question_master='" + sid_question_master + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteInspChecklist: " + ex.ToString());
            }
            return false;
        }
        internal bool FunInvalidInspChecklist(string sid_question_master,string invalid_reason)
        {
            try
            {
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sSqlstmt = "update t_inspection_question_master set chk_valid='Invalid',invalid_reason='"+ invalid_reason + "',invalid_date='" + TodayDate + "',invalid_logged_by='" + user + "' where id_question_master='" + sid_question_master + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunInvalidInspChecklist: " + ex.ToString());
            }
            return false;
        }
        //inspection plan
        internal bool FunAddInspectionPlan(InspctionQuestionModels objInsp, InspctionQuestionList objInspList)
        {
            try
            {

                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string branch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_inspection_plan (branch,checklist_ref,RevNo,insp_type,insp_detail,dept,Section,insp_area,location,insp_freq,project,logged_by,approved_by";

                string sFields = "", sFieldValue = "";

                if (objInsp.logged_date != null && objInsp.logged_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", logged_date";
                    sFieldValue = sFieldValue + ", '" + objInsp.logged_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + branch + "','" + checklist_ref + "','" + RevNo + "','" + insp_type + "','" + insp_detail + "','" + dept + "','" + Section + "','" + insp_area + "','" + location + "','" + insp_freq + "','" + project + "','" + user + "','" + approved_by + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_inspection_plan = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_inspection_plan))
                {                 
                    if (id_inspection_plan > 0)
                    {
                        objInspList.InspectionQstList[0].id_inspection_plan = id_inspection_plan.ToString();
                        FunAddInspectionPlanDateList(objInspList);
                        return SendInspplanmail(id_inspection_plan, "Inspections Plan");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspectionPlan: " + ex.ToString());
            }
            return false;
        }
        internal bool FunEditInspectionPlan(InspctionQuestionModels objInsp, InspctionQuestionList objInspList)
        {
            try
            {
              
                string sSqlstmt = "update t_inspection_plan set insp_detail='" + insp_detail + "',Section='" + Section + "',insp_area='" + insp_area + "',location='" + location + "',insp_freq='" + insp_freq + "',project='" + project + "',approved_by='" + approved_by + "'";

                sSqlstmt = sSqlstmt + " where id_inspection_plan='" + objInsp.id_inspection_plan + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    if (objInspList.InspectionQstList.Count > 0)
                    {
                        objInspList.InspectionQstList[0].id_inspection_plan = id_inspection_plan.ToString();
                        return FunAddInspectionPlanDateList(objInspList);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunEditInspectionPlan: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddInspectionPlanDateList(InspctionQuestionList objInspList)
        {
            try
            {

                string sSqlstmt = "delete from t_inspection_plan_dates where id_inspection_plan='" + objInspList.InspectionQstList[0].id_inspection_plan + "'; ";

                string inspstatus = objGlobalData.GetInspectionStatusIdByName("Planned");
                for (int i = 0; i < objInspList.InspectionQstList.Count; i++)
                {
                    string sid_inspection_date = "null";
                    sSqlstmt = sSqlstmt + "insert into t_inspection_plan_dates(id_inspection_date,id_inspection_plan,pers_resp,insp_status";

                    string sFieldValue = "", sFields = "", sValue = "", sStatement = "";
                    if (objInspList.InspectionQstList[i].id_inspection_date != null)
                    {
                        sid_inspection_date = objInspList.InspectionQstList[i].id_inspection_date;
                    }
                    if (objInspList.InspectionQstList[i].insp_date != null && objInspList.InspectionQstList[i].insp_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", insp_date= values(insp_date)";
                        sFields = sFields + ", insp_date";
                        sFieldValue = sFieldValue + ", '" + objInspList.InspectionQstList[i].insp_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sid_inspection_date + ",'" + objInspList.InspectionQstList[0].id_inspection_plan + "','" + objInspList.InspectionQstList[i].pers_resp + "','" + inspstatus + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                    + " id_inspection_date= values(id_inspection_date), id_inspection_plan= values(id_inspection_plan), pers_resp = values(pers_resp), insp_status= values(insp_status)";
                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";

                }
                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspectionPlanDateList: " + ex.ToString());
            }
            return false;
        }
        //inspection plan Mail
        internal bool SendInspplanmail(int id_inspection_plan, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select id_inspection_plan,insp_type,dept,Section,insp_area,location,project,checklist_ref,approved_by,logged_by"
               + " from t_inspection_plan  where id_inspection_plan = '" + id_inspection_plan + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["approved_by"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["logged_by"].ToString());
                    InspectionCategoryModels objInspModel = new InspectionCategoryModels();

                    sHeader = "<tr><td colspan=3><b>Type of inspection:<b></td> <td colspan=3>"
                  + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()) + "</td></tr>"
                  + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Section:<b></td> <td colspan=3>" + objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[0]["Section"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Area:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_area"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetDivisionLocationById(dsList.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>Project/Job:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["project"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Checklist reference:<b></td> <td colspan=3>" + objGlobalData.GetInspectionChecklistRefNamebyId(dsList.Tables[0].Rows[0]["checklist_ref"].ToString()) + "</td></tr>";

                    //inspection dates
                    string sql = "select insp_date,pers_resp from t_inspection_plan_dates where id_inspection_plan='"+ id_inspection_plan + "'";
                    DataSet dsItems = objGlobalData.Getdetails(sql);
                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<tr> "
                            + "<td colspan=3><label><b>Date(s) of inspection</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Date</th>"
                            + "<th style='width:300px'>Personnel Responsible</th>"

                            + "</tr>";


                        for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                        {
                            string sDate = "";
                            if (dsItems.Tables[0].Rows[i]["insp_date"].ToString() != null && dsItems.Tables[0].Rows[i]["insp_date"].ToString() != "")
                            {
                                sDate = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["insp_date"].ToString()).ToString("dd/MM/yyyy");

                            }

                            sInformation = sInformation + "<tr>"
                                + " <td>" + (i + 1) + "</td>"
                                + " <td style='width:300px'>" + sDate + "</td>"
                                + " <td style='width:300px'>" + objGlobalData.GetMultiHrEmpNameById(dsItems.Tables[0].Rows[i]["pers_resp"].ToString()) + "</td>"
                              
                                                       + " </tr>";
                             sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["pers_resp"].ToString());


                        }
                    }

                  
                    sHeader = sHeader + sInformation;

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Inspection checklist");
                    sContent = sContent.Replace("{content}", sHeader);
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
                objGlobalData.AddFunctionalLog("Exception in SendInspChecklistmail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteInspPlan(string sid_inspection_plan)
        {
            try
            {
                string sSqlstmt = "update t_inspection_plan set active=0 where id_inspection_plan='" + sid_inspection_plan + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteInspPlan: " + ex.ToString());
            }
            return false;
        }
        //inspection plan approve
        internal bool FunInspPlanApprove(InspctionQuestionModels objModel)
        {
            try
            {
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sSqlstmt = "update t_inspection_plan set approved_status='" + approved_status + "',approved_date='" + TodayDate + "',approver_comments='" + approver_comments + "'";

                sSqlstmt = sSqlstmt + " where id_inspection_plan='" + objModel.id_inspection_plan + "'";
                objGlobalData.ExecuteQuery(sSqlstmt);            
                return SendInspplanApprovemail(id_inspection_plan, approved_status, "Inspection plan approve Status");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunInspPlanApprove: " + ex.ToString());
            }
            return false;
        }
        //inspection plan approve Mail
        internal bool SendInspplanApprovemail(string id_inspection_plan,string iStatus, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select id_inspection_plan,insp_type,dept,Section,insp_area,location,project,checklist_ref,approved_by,logged_by,approver_comments"
               + " from t_inspection_plan  where id_inspection_plan = '" + id_inspection_plan + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                    string sHeader="", sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["approved_by"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["logged_by"].ToString());
                    InspectionCategoryModels objInspModel = new InspectionCategoryModels();

                    if(iStatus == "1") // rejected
                    {
                        sHeader = "<tr><td colspan=3><b>Type of inspection:<b></td> <td colspan=3>"
            + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()) + "</td></tr>"
            + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()) + "</td></tr>"
              + "<tr><td colspan=3><b>Section:<b></td> <td colspan=3>" + objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[0]["Section"].ToString()) + "</td></tr>"
                + "<tr><td colspan=3><b>Area:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_area"].ToString()) + "</td></tr>"
                  + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetDivisionLocationById(dsList.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Project/Job:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["project"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Checklist reference:<b></td> <td colspan=3>" + objGlobalData.GetInspectionChecklistRefNamebyId(dsList.Tables[0].Rows[0]["checklist_ref"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Approval Status:<b></td> <td colspan=3> Rejected </td></tr>"
                        +"<tr><td colspan=3><b>Comments:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["approver_comments"].ToString()) + "</td></tr>";
                    }
                    if (iStatus == "2") // approved
                    {
                        sHeader = "<tr><td colspan=3><b>Type of inspection:<b></td> <td colspan=3>"
            + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()) + "</td></tr>"
            + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()) + "</td></tr>"
              + "<tr><td colspan=3><b>Section:<b></td> <td colspan=3>" + objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[0]["Section"].ToString()) + "</td></tr>"
                + "<tr><td colspan=3><b>Area:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_area"].ToString()) + "</td></tr>"
                  + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetDivisionLocationById(dsList.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Project/Job:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["project"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Checklist reference:<b></td> <td colspan=3>" + objGlobalData.GetInspectionChecklistRefNamebyId(dsList.Tables[0].Rows[0]["checklist_ref"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Approval Status:<b></td> <td colspan=3> Approved </td></tr>"
                        + "<tr><td colspan=3><b>Comments:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["approver_comments"].ToString()) + "</td></tr>";
                    }
                    //inspection dates
                    string sql = "select insp_date,pers_resp from t_inspection_plan_dates where id_inspection_plan='" + id_inspection_plan + "'";
                    DataSet dsItems = objGlobalData.Getdetails(sql);
                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<tr> "
                            + "<td colspan=3><label><b>Date(s) of inspection</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Date</th>"
                            + "<th style='width:300px'>Personnel Responsible</th>"

                            + "</tr>";


                        for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                        {
                            string sDate = "";
                            if (dsItems.Tables[0].Rows[i]["insp_date"].ToString() != null && dsItems.Tables[0].Rows[i]["insp_date"].ToString() != "")
                            {
                                sDate = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["insp_date"].ToString()).ToString("dd/MM/yyyy");

                            }

                            sInformation = sInformation + "<tr>"
                                + " <td>" + (i + 1) + "</td>"
                                + " <td style='width:300px'>" + sDate + "</td>"
                                + " <td style='width:300px'>" + objGlobalData.GetMultiHrEmpNameById(dsItems.Tables[0].Rows[i]["pers_resp"].ToString()) + "</td>"

                                                       + " </tr>";
                            sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["pers_resp"].ToString());


                        }
                    }


                    sHeader = sHeader +"<br>"+ sInformation;

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Inspection Plan");
                    sContent = sContent.Replace("{content}", sHeader);
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
                objGlobalData.AddFunctionalLog("Exception in SendInspplanApprovemail: " + ex.ToString());
            }
            return false;
        }
        //perform inspection
        internal bool FunPerformInspection(InspctionQuestionModels objInsp, InspctionQuestionList objInspList)
        {

            try
            {
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;

                string sSqlstmt = "update t_inspection_plan_dates set insp_status='" + insp_status + "',notification_to='" + notification_to + "',logged_by='" + user + "',logged_date='" + TodayDate + "'";

                if (objInsp.insp_perf_date != null && objInsp.insp_perf_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",insp_perf_date='" + objInsp.insp_perf_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_inspection_date='" + objInsp.id_inspection_date + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    if (objInspList.InspectionQstList.Count > 0)
                    {
                        objInspList.InspectionQstList[0].id_inspection_plan = id_inspection_plan.ToString();
                        objInspList.InspectionQstList[0].id_inspection_date = id_inspection_date.ToString();
                         FunAddInspPerformList(objInspList);
                        return SendPerformInspectionmail(id_inspection_date.ToString(), "Inspection Performed");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunPerformInspection: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddInspPerformList(InspctionQuestionList objInspList)
        {
            try
            {

                string sSqlstmt = "delete from t_inspection_perform_checklist where id_inspection_date='" + objInspList.InspectionQstList[0].id_inspection_date + "'; ";


                for (int i = 0; i < objInspList.InspectionQstList.Count; i++)
                {
                    string sid_inspection_perform_checklist = "null";
                    sSqlstmt = sSqlstmt + "insert into t_inspection_perform_checklist(id_inspection_perform_checklist,id_inspection_date,id_inspection_plan,InspectionQuestions,criticality,criteria,insp_result,details,upload,action_required,suggestion";

                    string sFieldValue = "", sValue = "", sStatement = ""; ;
                    if (objInspList.InspectionQstList[i].id_inspection_perform_checklist != null && objInspList.InspectionQstList[i].id_inspection_perform_checklist != "")
                    {
                        sid_inspection_perform_checklist = objInspList.InspectionQstList[i].id_inspection_perform_checklist;
                    }

                    sSqlstmt = sSqlstmt + ") values(" + sid_inspection_perform_checklist + ",'" + objInspList.InspectionQstList[0].id_inspection_date + "','" + objInspList.InspectionQstList[0].id_inspection_plan + "','" + objInspList.InspectionQstList[i].InspectionQuestions + "','" + objInspList.InspectionQstList[i].criticality + "','" + objInspList.InspectionQstList[i].criteria + "'"
                        + ",'" + objInspList.InspectionQstList[i].insp_result + "','" + objInspList.InspectionQstList[i].details + "','" + objInspList.InspectionQstList[i].upload + "','" + objInspList.InspectionQstList[i].action_required + "','" + objInspList.InspectionQstList[i].suggestion + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                    + " id_inspection_perform_checklist= values(id_inspection_perform_checklist), id_inspection_date= values(id_inspection_date),id_inspection_plan= values(id_inspection_plan), InspectionQuestions = values(InspectionQuestions), criticality= values(criticality), criteria= values(criteria), insp_result= values(insp_result), details= values(details), upload= values(upload), action_required= values(action_required), suggestion= values(suggestion)";
                    sSqlstmt = sSqlstmt + sValue + ";";

                }
                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspPerformList: " + ex.ToString());
            }
            return false;
        }
        //inspection plan Mail
        internal bool SendPerformInspectionmail(string  id_inspection_date, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select id_inspection_date,T1.id_inspection_plan,RevNo,branch,checklist_ref,insp_type,insp_detail,dept,Section,insp_area,location,insp_freq,project,approved_by,insp_status,insp_perf_date,notification_to,T2.logged_by,pers_resp"
                    + " from t_inspection_plan T1,t_inspection_plan_dates T2 where T1.id_inspection_plan = T2.id_inspection_plan and id_inspection_date = '" + id_inspection_date + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["notification_to"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["notification_to"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["logged_by"].ToString())+","+ objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["pers_resp"].ToString());
                    InspectionCategoryModels objInspModel = new InspectionCategoryModels();

                    sHeader = "<tr><td colspan=3><b>Type of inspection:<b></td> <td colspan=3>"
                  + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()) + "</td></tr>"
                  + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Section:<b></td> <td colspan=3>" + objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[0]["Section"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Area:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_area"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetDivisionLocationById(dsList.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>Project/Job:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["project"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Checklist reference:<b></td> <td colspan=3>" + objGlobalData.GetInspectionChecklistRefNamebyRevNo(dsList.Tables[0].Rows[0]["checklist_ref"].ToString(), dsList.Tables[0].Rows[0]["RevNo"].ToString()) + "</td></tr>";



                    sHeader = sHeader + sInformation;

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Inspection Performed Details");
                    sContent = sContent.Replace("{content}", sHeader);
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
                objGlobalData.AddFunctionalLog("Exception in SendPerformInspectionmail: " + ex.ToString());
            }
            return false;
        }
        //-------------------------------------------------------------
        public MultiSelectList GetInspectionQuestionsListbox(string sCategory)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where Category='" + sCategory + "' and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            InspectionQuestions = dsQuest.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditElementsListbox: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }

        public MultiSelectList GetInspectionQuestionsList(string sCategory, string sSection)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where Category='" + sCategory + "' and Section='" + sSection + "'and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            InspectionQuestions = dsQuest.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionQuestionsList: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }

        public MultiSelectList GetInspectionQuestionsListboxWithBranch(string sbranch)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where branch='" + sbranch + "' and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            InspectionQuestions = dsQuest.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditElementsListbox: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }

        public MultiSelectList GetInspectionQuestionsListbox(string sCategory, string branchId)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where Category='" + sCategory + "' and branch='" + branchId + "' and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            InspectionQuestions = dsQuest.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionQuestionsListbox: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }

        public MultiSelectList GetInspectionQuestList(string sCategory, string sSection, string branchId)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where Category='" + sCategory + "' and Section='" + sSection + "' and branch='" + branchId + "' and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            InspectionQuestions = dsQuest.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionQuestList: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }

        public MultiSelectList GetInspectionQuestions(string sCategory, string branchId)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                string sUser = objGlobalData.GetCurrentUserSession().empid;
                string id_inspection = "";
                string Section_Id = "";
                DataSet dsQuest;
                dsQuest = objGlobalData.Getdetails("select id_inspection from t_inspection_section where Category='" + sCategory +
                    "' and branch='" + branchId + "' and (find_in_set('" + sUser + "', Resp_person) || Resp_person is NULL || Resp_person ='' ) and Active=1");

                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        id_inspection = dsQuest.Tables[0].Rows[i]["id_inspection"].ToString();
                        Section_Id = string.Concat(Section_Id, id_inspection, ",");
                    }
                    Section_Id = Section_Id.Remove(Section_Id.Length - 1, 1);
                }

                if (Section_Id != "")
                {
                    DataSet dsQuest1 = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where Section in (" + Section_Id + ") and Active=1");
                    if (dsQuest1.Tables.Count > 0 && dsQuest1.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsQuest1.Tables[0].Rows.Count; i++)
                        {
                            InspctionQuestionModels Qst = new InspctionQuestionModels()
                            {
                                id_inspection_question = dsQuest1.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                                InspectionQuestions = dsQuest1.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                            };

                            InspQst.InspectionQstList.Add(Qst);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionQuestions" + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }


        public MultiSelectList GetSectionQuestions(string sCategory, string branchId)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            InspectionCategoryModels obj = new InspectionCategoryModels();

            try
            {
                string sUser = objGlobalData.GetCurrentUserSession().empid;
                string id_inspection = "";
                string Section_Id = "";
                DataSet dsQuest;

                dsQuest = objGlobalData.Getdetails("select id_inspection from t_inspection_section where Category='" + sCategory +
                    "' and branch='" + branchId + "' and (find_in_set('" + sUser + "', Resp_person) ||  Resp_person is NULL || Resp_person ='') and Active=1");

                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        id_inspection = dsQuest.Tables[0].Rows[i]["id_inspection"].ToString();
                        Section_Id = string.Concat(Section_Id, id_inspection, ",");
                    }
                    Section_Id = Section_Id.Remove(Section_Id.Length - 1, 1);
                }
                if(Section_Id != "" )
                {                
                DataSet dsQuest1 = objGlobalData.Getdetails("select id_inspection_question,Section from t_inspection_questions where Section in (" + Section_Id + ") and Active=1 order by Section asc");
                if (dsQuest1.Tables.Count > 0 && dsQuest1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest1.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest1.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            Section = obj.GetSectionNamebyId(dsQuest1.Tables[0].Rows[i]["Section"].ToString())
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSectionQuestions: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "Section");
        }


        internal bool FunDeleteQuestions(string sid_inspection_question)
        {
            try
            {
                string sSqlstmt = "update t_inspection_questions set Active=0 where id_inspection_question='" + sid_inspection_question + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteQuestions: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInspectionQuestions(int id_inspection_question, string InspectionQuestions)
        {
            try
            {
                string sSqlstmt = "update t_inspection_questions set InspectionQuestions='" + InspectionQuestions + "' where id_inspection_question='" + id_inspection_question + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInspectionQuestions: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddInspectionQuestions(InspctionQuestionModels objInspModels)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_inspection_questions (InspectionQuestions,Category,Section,branch) values('" + objInspModels.InspectionQuestions + "','" + objInspModels.Category + "','" + objInspModels.Section + "','" + sBranch + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspectionQuestions: " + ex.ToString());
            }
            return false;
        }

        public DataSet GetInspectionRating()
        {

            DataSet dsRating = new DataSet();
            try
            {
                dsRating = objGlobalData.Getdetails("select id_inspection_rating, inspection_rating from t_inspection_rating order by id_inspection_rating");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionRating: " + ex.ToString());
            }
            return dsRating;//new MultiSelectList(lstSurvey.lstSurveyRating, "SQ_OptionsId", "RatingOptions");
        }

        public DataSet GetInspectionRating(string Category)
        {

            DataSet dsRating = new DataSet();
            try
            {
                dsRating = objGlobalData.Getdetails("select id_inspection_rating, inspection_rating from t_inspection_rating where Category='" + Category + "' and Active=1 order by id_inspection_rating");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionRating: " + ex.ToString());
            }
            return dsRating;//new MultiSelectList(lstSurvey.lstSurveyRating, "SQ_OptionsId", "RatingOptions");
        }
        //------------------------Rating-------------------------------------

        public MultiSelectList GetInspectionRatingListbox(string sCategory)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_rating,inspection_rating from t_inspection_rating where Category='" + sCategory + "' and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_rating = dsQuest.Tables[0].Rows[i]["id_inspection_rating"].ToString(),
                            inspection_rating = dsQuest.Tables[0].Rows[i]["inspection_rating"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionRatingListbox: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_rating", "inspection_rating");
        }

        internal bool FunAddInspectionRatings(InspctionQuestionModels objInspModels)
        {
            try
            {
                string sSqlstmt = "insert into t_inspection_rating (inspection_rating,Category) values('" + objInspModels.inspection_rating + "','" + objInspModels.Category + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspectionRatings: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteRatings(string id_inspection_rating)
        {
            try
            {
                string sSqlstmt = "update t_inspection_rating set Active=0 where id_inspection_rating='" + id_inspection_rating + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteRatings: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInspectionRatings(int id_inspection_rating, string inspection_rating)
        {
            try
            {
                string sSqlstmt = "update t_inspection_rating set inspection_rating='" + inspection_rating + "' where id_inspection_rating='" + id_inspection_rating + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInspectionRatings: " + ex.ToString());
            }
            return false;
        }

        //-----------------------Section-----------------------------

        public MultiSelectList GetInspectionSectionListbox(string sCategory, string sbranch)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection,Section from t_inspection_section where Category='" + sCategory + "' and branch = '" + sbranch + "' and Active = 1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection = dsQuest.Tables[0].Rows[i]["id_inspection"].ToString(),
                            Section = dsQuest.Tables[0].Rows[i]["Section"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionSectionListbox: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection", "Section");
        }

        internal bool FunAddInspectionSection(InspctionQuestionModels objInspModels)
        {
            try
            {
                string sBranch_name = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_inspection_section (dept,Section) values('" + objInspModels.dept + "','" + objInspModels.Section + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspectionSection: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteSection(string sid_inspection)
        {
            try
            {
                string sSqlstmt = "update t_inspection_section set active=0 where id_inspection='" + sid_inspection + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSection: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInspectionSection(int sid_inspection, string sSection, string sdept)
        {
            try
            {
                string sSqlstmt = "update t_inspection_section set Section='" + sSection + "' , dept = '" + sdept + "' where id_inspection='" + sid_inspection + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInspectionSection: " + ex.ToString());
            }
            return false;
        }


    }

    public class GenerateInspectionChecklistModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "id")]
        public string id_inspection_checklist { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Inspection Date")]
        public DateTime Inspection_date { get; set; }

        [Display(Name = "Inspection By")]
        public string Inspection_by { get; set; }

        [Display(Name = "Location/Site")]
        public string Location { get; set; }

        [Display(Name = "Project")]
        public string Project { get; set; }

        [Display(Name = "Tag No")]
        public string TagNo { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Group")]
        public string Department { get; set; }

        [Display(Name = "Directorate")]
        public string branch { get; set; }

        [Display(Name = "Team")]
        public string team { get; set; }

        public string id_inspection { get; set; }
        public string cat_id { get; set; }

        public string id_inspection_question { get; set; }
        public string Section { get; set; }
        public string InspectionQuestions { get; set; }
        public List<GenerateInspectionChecklistModels> InspChk { get; set; }

        internal bool FunAddInspectionChecklist(GenerateInspectionChecklistModels objInspChecklist, InspectionChecklistList objInsp)
        {
            InspectionChecklistModels obj = new InspectionChecklistModels();
            string Inspection_date = objInspChecklist.Inspection_date.ToString("yyyy-MM-dd HH':'mm':'ss");
           // string sBranch = objGlobalData.GetCurrentUserSession().division;

            string sSqlstmt = "insert into t_inspection_checklist (Category,Inspection_date,Inspection_by,Location,Project,TagNo,Comment,branch,Department,team) values('" + objInspChecklist.Category + "',"
          + "'" + Inspection_date + "','" + objInspChecklist.Inspection_by + "','" + objInspChecklist.Location + "','" + objInspChecklist.Project + "','" + objInspChecklist.TagNo + "','" + objInspChecklist.Comment + "','" + objInspChecklist.branch + "','" + objInspChecklist.Department + "','" + objInspChecklist.team + "')";

            return obj.FunAddInspectionChecklistDetails(objInsp, objGlobalData.ExecuteQueryReturnId(sSqlstmt));

        }

        internal bool FunUpdateInspectionChecklist(GenerateInspectionChecklistModels objInspChecklist, InspectionChecklistList objInsp)
        {
            try
            {
                string Inspection_date = objInspChecklist.Inspection_date.ToString("yyyy-MM-dd HH':'mm':'ss");


                string sSqlstmt = "update t_inspection_checklist set Category='" + objInspChecklist.Category + "', Inspection_by='" + objInspChecklist.Inspection_by
                   + "', Location='" + objInspChecklist.Location + "', Project='" + objInspChecklist.Project + "', TagNo='" + objInspChecklist.TagNo + "', Comment='" + objInspChecklist.Comment
                   + "', Department='" + objInspChecklist.Department + "', branch='" + objInspChecklist.branch + "', team='" + objInspChecklist.team + "'";

                if (objInspChecklist.Inspection_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Inspection_date='" + Inspection_date + "' ";
                }
                sSqlstmt = sSqlstmt + " where id_inspection_checklist='" + objInspChecklist.id_inspection_checklist + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    InspectionChecklistModels objElement = new InspectionChecklistModels();
                    objInsp.lst[0].id_inspection_checklist = objInspChecklist.id_inspection_checklist;
                    objElement.FunUpdateInspectionChecklistDetails(objInsp);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInspectionChecklist: " + ex.ToString());
            }
            return false;
        }

    }

    public class InspectionChecklistModels
    {
        clsGlobal objGlobalData = new clsGlobal();
        [Display(Name = "id")]
        public string id_inspection_checklist_trans { get; set; }

        [Display(Name = "ID")]
        public string id_inspection_checklist { get; set; }

        [Display(Name = "id")]
        public string id_inspection_question { get; set; }

        [Display(Name = "id")]
        public string id_inspection_rating { get; set; }

        [Display(Name = "Action")]
        public string Action { get; set; }

        [Display(Name = "Action By")]
        public string ActionBy { get; set; }

        [Display(Name = "Upload")]
        public string Upload { get; set; }

        public string id_ques { get; set; }

        internal bool FunAddInspectionChecklistDetails(InspectionChecklistList objInsp, int id_inspection_checklist)
        {
            try
            {
                if (objInsp.lst.Count > 0)
                {

                    string sSqlstmt = "delete from t_inspection_checklist_trans where id_inspection_checklist='"
                        + id_inspection_checklist + "'; ";
                    for (int i = 0; i < objInsp.lst.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_inspection_checklist_trans (id_inspection_checklist, id_inspection_question, id_inspection_rating,Action,ActionBy,Upload"
                        + ") values('" + id_inspection_checklist + "','" + objInsp.lst[i].id_inspection_question
                        + "','" + objInsp.lst[i].id_inspection_rating + "','" + objInsp.lst[i].Action + "','" + objInsp.lst[i].ActionBy + "','" + objInsp.lst[i].Upload + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspectionChecklistDetails: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInspectionChecklistDetails(InspectionChecklistList objInsp)
        {
            try
            {
                if (objInsp.lst.Count > 0)
                {
                    string sSqlstmt = "";
                    for (int i = 0; i < objInsp.lst.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "update t_inspection_checklist_trans set id_inspection_question='" + objInsp.lst[i].id_inspection_question + "',"
                            + "id_inspection_rating='" + objInsp.lst[i].id_inspection_rating + "',Action='" + objInsp.lst[i].Action + "',ActionBy='" + objInsp.lst[i].ActionBy + "'";
                        if (objInsp.lst[i].Upload != null)
                        {
                            sSqlstmt = sSqlstmt + ", Upload='" + objInsp.lst[i].Upload + "' ";
                        }
                        sSqlstmt = sSqlstmt + " where id_inspection_checklist_trans='" + objInsp.lst[i].id_inspection_checklist_trans + "';";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInspectionChecklistDetails: " + ex.ToString());
            }
            return false;
        }

        public string GetInspectionQuestionNameById(string sid_inspection_question)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("SELECT InspectionQuestions FROM t_inspection_questions where id_inspection_question='" + sid_inspection_question + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["InspectionQuestions"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionQuestionNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetInspectionRatingNameById(string sid_inspection_rating)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("SELECT inspection_rating FROM t_inspection_rating where id_inspection_rating='" + sid_inspection_rating + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["inspection_rating"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionRatingNameById: " + ex.ToString());
            }
            return "";
        }

        internal bool FunDeleteInspectionChecklist(string sid_inspection_checklist)
        {
            try
            {
                string sSqlstmt = "update t_inspection_checklist set Active=0 where id_inspection_checklist='" + sid_inspection_checklist + "' and Active=1";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteInspectionChecklist: " + ex.ToString());
            }
            return false;
        }
    }

    public class InspectionCategoryList
    {
        public List<InspectionCategoryModels> InspectionLst { get; set; }
    }

    public class InspctionQuestionList
    {
        public List<InspctionQuestionModels> InspectionQstList { get; set; }
    }

    public class GenerateInspectionChecklistList
    {
        public List<GenerateInspectionChecklistModels> lstChk { get; set; }
    }
    public class InspectionChecklistList
    {
        public List<InspectionChecklistModels> lst { get; set; }
    }

    public class DropdownInspectionItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownInspectionList
    {
        public List<DropdownInspectionItems> lstDropdown { get; set; }
    }
}