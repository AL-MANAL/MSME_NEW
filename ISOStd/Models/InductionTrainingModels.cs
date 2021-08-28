using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class InductionTrainingModels
    {
        clsGlobal objGlobaldata = new clsGlobal();

        //t_induction_training
        [Display(Name = "Id")]
        public string id_induction { get; set; }

        [Display(Name = "Training Reference No.")]
        public string ref_no { get; set; }

        [Display(Name = "Division")]
        public string division { get; set; }

        [Display(Name = "Department")]
        public string department { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "Training Topic")]
        public string training_topic { get; set; }

        [Display(Name = "Planned by")]
        public string planned_by { get; set; }
        public string planned_id { get; set; }

        [Display(Name = "Plan Date & Time")]
        public DateTime start_date { get; set; }

        [Display(Name = "End Date & Time")]
        public DateTime end_date { get; set; }

        [Display(Name = "Attendees")]
        public string employee_id { get; set; }
        public string employee_Name { get; set; }

        [Display(Name = "Notified To")]
        public string plan_notifiedto { get; set; }

        [Display(Name = "Priority")]
        public string priority { get; set; }

        //t_induction_training_trans
        [Display(Name = "Training Status")]
        public string final_status { get; set; }

        [Display(Name = "Training Effectiveness")]
        public string effectness { get; set; }

        [Display(Name = "Overall Training Completed On")]
        public DateTime completion_date { get; set; }

        [Display(Name = "Suggestion")]
        public string suggestion { get; set; }

        [Display(Name = "Further Training")]
        public string further_training { get; set; }

        [Display(Name = "Completion Notified To")]
        public string final_notifyto { get; set; }

        //t_induction_training_material

        [Display(Name = "Id")]
        public string id_material { get; set; }

        [Display(Name = "Material Description")]
        public string mat_description { get; set; }

        [Display(Name = "Preview")]
        public string upload { get; set; }

        [Display(Name = "Start Date")]
        public DateTime traing_start_date { get; set; }

        [Display(Name = "Comments")]
        public string comments { get; set; }

        [Display(Name = "Explain")]
        public string explain { get; set; }

        [Display(Name = "Compled Date")]
        public DateTime compled_date { get; set; }

        [Display(Name = "Notify To")]
        public string notify_to { get; set; }

        public MultiSelectList GetInductionEffectivenessList()
        {
            DropdownNCList NcdropList = new DropdownNCList();
            NcdropList.NcDropdownList = new List<DropdownNCItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Induction Training Effectiveness' order by item_id asc";
                DataSet dsNc = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNc.Tables.Count > 0 && dsNc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNc.Tables[0].Rows.Count; i++)
                    {
                        DropdownNCItems ncmodel = new DropdownNCItems()
                        {
                            Id = dsNc.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsNc.Tables[0].Rows[i]["Name"].ToString()
                        };
                        NcdropList.NcDropdownList.Add(ncmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetInductionEffectivenessList: " + ex.ToString());
            }
            return new MultiSelectList(NcdropList.NcDropdownList, "Id", "Name");
        }

        internal string GetInductionEffectivenessById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Induction Training Effectiveness' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetInductionEffectivenessById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetInductionPriorityList()
        {
            DropdownNCList NcdropList = new DropdownNCList();
            NcdropList.NcDropdownList = new List<DropdownNCItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Induction Training Priority' order by item_id asc";
                DataSet dsNc = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNc.Tables.Count > 0 && dsNc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNc.Tables[0].Rows.Count; i++)
                    {
                        DropdownNCItems ncmodel = new DropdownNCItems()
                        {
                            Id = dsNc.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsNc.Tables[0].Rows[i]["Name"].ToString()
                        };
                        NcdropList.NcDropdownList.Add(ncmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetInductionPriorityList: " + ex.ToString());
            }
            return new MultiSelectList(NcdropList.NcDropdownList, "Id", "Name");
        }

        internal string GetInductionPriorityById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Induction Training Priority' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetInductionPriorityById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetInductionStatusList()
        {
            DropdownNCList NcdropList = new DropdownNCList();
            NcdropList.NcDropdownList = new List<DropdownNCItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Induction Training Status' order by item_desc asc";
                DataSet dsNc = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNc.Tables.Count > 0 && dsNc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNc.Tables[0].Rows.Count; i++)
                    {
                        DropdownNCItems ncmodel = new DropdownNCItems()
                        {
                            Id = dsNc.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsNc.Tables[0].Rows[i]["Name"].ToString()
                        };
                        NcdropList.NcDropdownList.Add(ncmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetInductionStatusList: " + ex.ToString());
            }
            return new MultiSelectList(NcdropList.NcDropdownList, "Id", "Name");
        }

        internal string GetInductionStatusById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Induction Training Status' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetInductionStatusById: " + ex.ToString());
            }
            return "";
        }

        internal string GetInductionStatusIdByName(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Induction Training Status' and item_desc='" + sStatus + "' ");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Id"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetInductionStatusIdByName: " + ex.ToString());
            }
            return "";
        }

        internal bool FunAddInduction(InductionTrainingModels objModels, InductionTrainingModelsList objIndList)
        {
            try
            {
                string sFields = "", sFieldValue = "";

                string sSqlstmt = "insert into t_induction_training(division,department,location,training_topic,planned_by,employee_id,plan_notifiedto,priority,logged_by";

                if (objModels.start_date != null && objModels.start_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", start_date";
                    sFieldValue = sFieldValue + ", '" + objModels.start_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                }
                if (objModels.end_date != null && objModels.end_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", end_date";
                    sFieldValue = sFieldValue + ", '" + objModels.end_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                }
                sSqlstmt = sSqlstmt + sFields + ") values('" + objModels.division + "','" + objModels.department
                + "','" + objModels.location + "','" + objModels.training_topic + "','" + objModels.planned_by + "','" + objModels.employee_id
                + "','" + objModels.plan_notifiedto + "','" + objModels.priority + "','" + objGlobaldata.GetCurrentUserSession().empid  + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int iid_induction = 0;

                if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out iid_induction))
                {
                    string semployee_id = objModels.employee_id;
                    if (iid_induction > 0 && Convert.ToInt32(objIndList.lstInd.Count) > 0)
                    {
                        objIndList.lstInd[0].id_induction = iid_induction.ToString();
                        FunAddInductionMat(objIndList, semployee_id, objModels.start_date);
                    }

                    if (iid_induction > 0)
                    {
                        string SSql = "";
                        if (semployee_id != "")
                        {
                            string[] empList = semployee_id.Split(',');
                            foreach (string employee in empList)
                            {
                                SSql = SSql + " insert into t_induction_training_trans (id_induction,employee_id,final_status)"
                                                   + " values(" + iid_induction + ",'" + employee + "','" + objModels.final_status + "')"
                                                   + " ON DUPLICATE KEY UPDATE "
                                                   + "id_induction= values(id_induction), employee_id= values(employee_id), final_status= values(final_status); ";
                            }
                            objGlobaldata.ExecuteQuery(SSql);
                        }

                        string sDivision = objGlobaldata.GetBranchShortNameByID(objModels.division);
                        string sDept = objGlobaldata.GetDeptNameById(objModels.department);
                        DataSet dsData = objGlobaldata.GetReportNo("Induction Training", sDept, sDivision);
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            ref_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }
                        string sql1 = "update t_induction_training set ref_no='" + ref_no + "' where id_induction='" + iid_induction + "'";
                        if (objGlobaldata.ExecuteQuery(sql1))
                        {
                            SendIndEmail(iid_induction, "Induction Training");
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddInduction: " + ex.ToString());
            }
            return false;
        }

        internal bool SendIndEmail(int id_induction, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_induction, ref_no, division, department, location,training_topic, planned_by, start_date,  end_date, employee_id,"
                    + "plan_notifiedto, priority, logged_by from t_induction_training where id_induction ='" + id_induction + "'";

                DataSet dsIndList = objGlobaldata.Getdetails(sSqlstmt);
                InductionTrainingModels objType = new InductionTrainingModels();

                if (dsIndList.Tables.Count > 0 && dsIndList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

                    //Form the Email Subject and Body content
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
                    string sName = "All";
                    string sToEmailIds = "";
                    if (objGlobaldata.GetMultiHrEmpEmailIdById(dsIndList.Tables[0].Rows[0]["employee_id"].ToString()) != "")
                    {
                        sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsIndList.Tables[0].Rows[0]["employee_id"].ToString()) + ",";
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    if (objGlobaldata.GetMultiHrEmpEmailIdById(dsIndList.Tables[0].Rows[0]["plan_notifiedto"].ToString()) != "")
                    {
                        sToEmailIds = sToEmailIds + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsIndList.Tables[0].Rows[0]["plan_notifiedto"].ToString()) + ",";
                    }
                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');
                    string sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsIndList.Tables[0].Rows[0]["planned_by"].ToString()) + "," + objGlobaldata.GetHrEmpEmailIdById(dsIndList.Tables[0].Rows[0]["logged_by"].ToString());
                    //aAttachment = HttpContext.Current.Server.MapPath(dsIndList.Tables[0].Rows[0]["nc_upload"].ToString());


                    sHeader = "<tr><td colspan=3><b>Ref Number:<b></td> <td colspan=3>" + dsIndList.Tables[0].Rows[0]["ref_no"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobaldata.GetMultiCompanyBranchNameById(dsIndList.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobaldata.GetMultiDeptNameById(dsIndList.Tables[0].Rows[0]["department"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobaldata.GetDivisionLocationById(dsIndList.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Plan Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsIndList.Tables[0].Rows[0]["start_date"].ToString()).ToString("dd/MM/yyyy")+ "</td></tr>"
                        + "<tr><td colspan=3><b>Training Topic:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsIndList.Tables[0].Rows[0]["training_topic"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Attendees:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["employee_id"].ToString()) + "</td></tr>";

                    //if (File.Exists(aAttachment))
                    //{
                    //    sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    //}

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Induction Training Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendIndEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInduction(InductionTrainingModels objModels, InductionTrainingModelsList objIndList)
        {
            try
            {

                string sSqlstmt = "update t_induction_training set  division='" + objModels.division + "', " + "department='" + objModels.department
                     + "', location='" + objModels.location + "', training_topic='" + objModels.training_topic + "', planned_by='" + objModels.planned_by + "', plan_notifiedto='" + objModels.plan_notifiedto
                     + "', priority='" + objModels.priority + "'";

                if (objModels.start_date != null && objModels.start_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", start_date ='" + objModels.start_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                }

                if (objModels.end_date != null && objModels.end_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", end_date ='" + objModels.end_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_induction='" + objModels.id_induction + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objIndList.lstInd.Count) > 0)
                    {
                        objIndList.lstInd[0].id_induction = id_induction.ToString();
                        FunAddInductionMat(objIndList, objModels.employee_id,objModels.start_date);
                    }
                    else
                    {
                        FunUpdateInductionMat(id_induction);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateInduction: " + ex.ToString());
            }
            return false;
        }


        //Induction Trans Table

        internal bool FunAddInductionMat(InductionTrainingModelsList objInduList,string semployee_id,DateTime sStart_date)
        {
            try
            {              
                string sSqlstmt = "delete from t_induction_training_material where id_induction='" + objInduList.lstInd[0].id_induction + "'; ";
                //string sSqlstmt = "";
                if (sStart_date != null && sStart_date > Convert.ToDateTime("01/01/0001"))
                {
                    traing_start_date = sStart_date;
                }

                for (int i = 0; i < objInduList.lstInd.Count; i++)
                {                    
                    string[] empList = semployee_id.Split(',');
                    foreach (string employee in empList)
                    {
                        sSqlstmt = sSqlstmt + " insert into t_induction_training_material (id_induction,employee_id,mat_description,upload,traing_start_date)"
                                           + " values(" + objInduList.lstInd[0].id_induction + ",'" + employee + "','" + objInduList.lstInd[i].mat_description + "','" + objInduList.lstInd[i].upload + "','" + traing_start_date.ToString("yyyy/MM/dd") + "')"
                                           + " ON DUPLICATE KEY UPDATE "
                                           + "id_induction= values(id_induction), employee_id= values(employee_id), mat_description = values(mat_description), upload = values(upload), traing_start_date = values(traing_start_date); ";
                    }
                   
                }
               
                return objGlobaldata.ExecuteQuery(sSqlstmt);               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddInductionMat: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInductionMat(string sid_induction)
        {
            try
            {
                string sSqlstmt = "delete from t_induction_training_material where id_induction='" + sid_induction + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateInductionMat: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddPerformInduction(InductionTrainingModels objModels, InductionTrainingModelsList objIndList)
        {
            try
            {
                string sSqlstmt = "update t_induction_training_trans set  final_status='" + objModels.final_status + "', " + "effectness='" + objModels.effectness + "', suggestion='" + objModels.suggestion
                     + "', further_training='" + objModels.further_training + "', final_notifyto='" + objModels.final_notifyto + "'";

                if (objModels.completion_date != null && objModels.completion_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", completion_date ='" + objModels.completion_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_induction='" + objModels.id_induction + "' and employee_id = '"+objModels.employee_id + "'";

                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                { 
                   if(Convert.ToInt32(objIndList.lstInd.Count) > 0)
                    {
                       // objIndList.lstInd[0].id_induction = objModels.id_induction;
                        FunUpdateInductionTrans(objIndList);
                    }

                    SendPerformInductionEmail(objModels.id_induction,objModels.employee_id, "Perform Induction Training");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddPerformInduction: " + ex.ToString());
            }
            return false;
        }

        internal bool SendPerformInductionEmail(string sid_induction, string semployee_id,string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select ref_no, division, department, location,final_status,completion_date,effectness,suggestion,further_training,final_notifyto from t_induction_training a,t_induction_training_trans b where a.id_induction=b.id_induction and b.id_induction='" + sid_induction + "' and b.employee_id ='" + semployee_id + "' and b.active=1";

                DataSet dsIndList = objGlobaldata.Getdetails(sSqlstmt);
                InductionTrainingModels objType = new InductionTrainingModels();

                if (dsIndList.Tables.Count > 0 && dsIndList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

                    //Form the Email Subject and Body content
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
                    string sName = "All";
                    string sToEmailIds = "";
                    if (objGlobaldata.GetMultiHrEmpEmailIdById(dsIndList.Tables[0].Rows[0]["final_notifyto"].ToString()) != "")
                    {
                        sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsIndList.Tables[0].Rows[0]["final_notifyto"].ToString()) + ",";
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');
                   
                    string sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(semployee_id) ;
                    //aAttachment = HttpContext.Current.Server.MapPath(dsIndList.Tables[0].Rows[0]["nc_upload"].ToString());


                    sHeader = "<tr><td colspan=3><b>Ref Number:<b></td> <td colspan=3>" + dsIndList.Tables[0].Rows[0]["ref_no"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobaldata.GetMultiCompanyBranchNameById(dsIndList.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobaldata.GetMultiDeptNameById(dsIndList.Tables[0].Rows[0]["department"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobaldata.GetDivisionLocationById(dsIndList.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Completion Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsIndList.Tables[0].Rows[0]["completion_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                        + "<tr><td colspan=3><b>Suggestion:<b></td> <td colspan=3>" + (dsIndList.Tables[0].Rows[0]["suggestion"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Effectness:<b></td> <td colspan=3>" + objType.GetInductionEffectivenessById(dsIndList.Tables[0].Rows[0]["effectness"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Further Training Required:<b></td> <td colspan=3>" + (dsIndList.Tables[0].Rows[0]["further_training"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Training Notified To:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsIndList.Tables[0].Rows[0]["final_notifyto"].ToString()) + "</td></tr>";


                    //if (File.Exists(aAttachment))
                    //{
                    //    sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    //}

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Perfmorm Induction Training Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendPerformInductionEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInductionTrans(InductionTrainingModelsList objInduList)
        {
            try
            {
               string sSqlstmt = "";
                for (int i = 0; i < objInduList.lstInd.Count; i++)
                {
                    string sid_material = "";
                    if (objInduList.lstInd[i].id_material != null && objInduList.lstInd[i].id_material != "")
                    {
                        sid_material = objInduList.lstInd[i].id_material;
                    }
                    sSqlstmt = sSqlstmt + " update t_induction_training_material set comments = '" + objInduList.lstInd[i].comments
                        + "', `explain` = '" + objInduList.lstInd[i].explain + "', notify_to ='" + objInduList.lstInd[i].notify_to + "'";

                    if (objInduList.lstInd[i].compled_date != null && objInduList.lstInd[i].compled_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sSqlstmt = sSqlstmt + ", compled_date ='" + objInduList.lstInd[i].compled_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objInduList.lstInd[i].traing_start_date != null && objInduList.lstInd[i].traing_start_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sSqlstmt = sSqlstmt + ", traing_start_date ='" + objInduList.lstInd[i].traing_start_date.ToString("yyyy/MM/dd") + "'";
                    }

                    sSqlstmt = sSqlstmt + " where id_material = '" + sid_material + "';";
                }               
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddInductionTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunInductionDelete(string sid_induction)
        {
            try
            {
                string sSqlstmt = "update t_induction_training set active=0 where id_induction='" + sid_induction + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunInductionDelete: " + ex.ToString());
            }
            return false;
        }

        internal bool FunInductionTransDelete(string sid_induction, string semployee_id)
        {
            try
            {
                string sSqlstmt = "update t_induction_training_material set active=0 where id_induction='" + sid_induction + "' and employee_id= '"+ semployee_id + "'";
                objGlobaldata.ExecuteQuery(sSqlstmt);

                string sSqlstmt1 = "update t_induction_training_trans set active=0 where id_induction='" + sid_induction + "' and employee_id= '" + semployee_id + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt1);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunInductionDelete: " + ex.ToString());
            }
            return false;
        }

    }

    public class InductionTrainingModelsList
    {
        public List<InductionTrainingModels> lstInd { get; set; }
    }
}