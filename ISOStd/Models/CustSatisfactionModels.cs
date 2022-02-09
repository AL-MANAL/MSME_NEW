using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{
    public class CustSatisfactionModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //t_cust_satisfaction
        [Display(Name = "ID")]
        public string id_cust_satisfaction { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Customer Name")]
        public string cust_name { get; set; }

        [Display(Name = "From Date")]
        public DateTime frm_date { get; set; }

        [Display(Name = "To Date")]
        public DateTime to_date { get; set; }

        [Display(Name = "Products or services delivered")]
        public string prod_delivered { get; set; }

        [Display(Name = "Customer PO / Contract details")]
        public string contact_details { get; set; }

        [Display(Name = "Customer Satisfaction Evaluation Date")]
        public DateTime eval_date { get; set; }

        [Display(Name = "Repeat enquiries")]
        public string repeat_enquiries { get; set; }

        [Display(Name = "No. of enquiries")]
        public string no_enquiries { get; set; }

        [Display(Name = "Repeat orders")]
        public string repeat_orders { get; set; }

        [Display(Name = "No. of orders")]
        public string no_orders { get; set; }

        [Display(Name = "Upload records")]
        public string enquiries_upload { get; set; }

        [Display(Name = "Upload records")]
        public string orders_upload { get; set; }

        [Display(Name = "Feedback through satisfaction survey form")]
        public string survey_form { get; set; }

        [Display(Name = "Overall Customer Satisfaction Index")]
        public string csi { get; set; }

        [Display(Name = "To be reviewed by")]
        public string reviewed_by { get; set; }

        [Display(Name = "Is customer satisfied?")]
        public string cust_satisfied { get; set; }

        //t_cust_satisfaction_perf
        [Display(Name = "ID")]
        public string id_perf { get; set; }

        [Display(Name = "Remarks")]
        public string perf_remarks { get; set; }

        [Display(Name = "Upload Records")]
        public string perf_upload { get; set; }

        [Display(Name = "Performance/reliability of the product")]
        public string performance { get; set; }

        //t_cust_satisfaction_delivery

        [Display(Name = "ID")]
        public string id_delivery { get; set; }

        [Display(Name = "Remarks")]
        public string delivery_remarks { get; set; }

        [Display(Name = "Upload Records")]
        public string delivery_upload { get; set; }

        [Display(Name = "On time delivery")]
        public string delivery { get; set; }

        //t_cust_satisfaction_complaints

        [Display(Name = "ID")]
        public string id_complaint { get; set; }

        [Display(Name = "Customer complaints if any")]
        public string complaint { get; set; }

        [Display(Name = "Type")]
        public string complaint_type { get; set; }

        [Display(Name = "No. of complaints")]
        public string complaint_no { get; set; }

        [Display(Name = "Upload Records")]
        public string complaint_upload { get; set; }

        [Display(Name = "Review Status")]
        public string review_status { get; set; }

        [Display(Name = "Comment")]
        public string review_comment { get; set; }

        [Display(Name = "Date")]
        public DateTime review_date { get; set; }

        //t_cust_satisfaction_act

        [Display(Name = "Id")]
        public string id_act { get; set; }

        [Display(Name = "Action to be taken")]
        public string action_taken { get; set; }

        [Display(Name = "Personnel Responsible")]
        public string pers_resp { get; set; }

        [Display(Name = "Target Date")]
        public DateTime target_date { get; set; }

        [Display(Name = "Resources required ")]
        public string resource_req { get; set; }

        [Display(Name = "Status")]
        public string act_status { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        [Display(Name = "Update Date")]
        public DateTime update_date { get; set; }

        public string review_status_id { get; set; }

        [Display(Name = "Reference No")]
        public string ref_no { get; set; }

        [Display(Name = "Action Initiated By")]
        public string action_initiated_by { get; set; }


        internal bool FunAddCustomerSatisfaction(CustSatisfactionModels objModel, CustSatisfactionModelsList objQList, CustSatisfactionModelsList objDList, CustSatisfactionModelsList objCList)
        {
            try
            {

                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;

             
                string sSqlstmt = "insert into t_cust_satisfaction (branch,Department,Location,cust_name,prod_delivered,contact_details,repeat_enquiries,no_enquiries,repeat_orders,"
                    + "no_orders,enquiries_upload,orders_upload,survey_form,csi,reviewed_by,logged_by,complaint,cust_satisfied";

                string sFields = "", sFieldValue = "";

                if (objModel.frm_date != null && objModel.frm_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", frm_date";
                    sFieldValue = sFieldValue + ", '" + objModel.frm_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.to_date != null && objModel.to_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", to_date";
                    sFieldValue = sFieldValue + ", '" + objModel.to_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.eval_date != null && objModel.eval_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", eval_date";
                    sFieldValue = sFieldValue + ", '" + objModel.eval_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('"+branch+ "','" + Department + "','" + Location + "','" + cust_name + "','" + prod_delivered + "','" + contact_details + "','" + repeat_enquiries + "','" + no_enquiries + "','" + repeat_orders + "',"
                    + "'" + no_orders + "','" + enquiries_upload + "','" + orders_upload + "','" + survey_form + "','" + csi + "','" + reviewed_by + "','" + user + "','" + complaint + "','" + cust_satisfied + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_cust_satisfaction = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_cust_satisfaction))
                {
                    if (id_cust_satisfaction > 0)
                    {

                        if (id_cust_satisfaction > 0 && Convert.ToInt32(objQList.CSList.Count) > 0)
                        {
                            objQList.CSList[0].id_cust_satisfaction = id_cust_satisfaction.ToString();

                            FunAddPerfList(objQList);
                        }
                        if (id_cust_satisfaction > 0 && Convert.ToInt32(objDList.CSList.Count) > 0)
                        {
                            objDList.CSList[0].id_cust_satisfaction = id_cust_satisfaction.ToString();

                            FunAddDeliveryList(objDList);
                        }
                        if (id_cust_satisfaction > 0 && Convert.ToInt32(objCList.CSList.Count) > 0)
                        {
                            objCList.CSList[0].id_cust_satisfaction = id_cust_satisfaction.ToString();

                            FunAddComplaintList(objCList);
                        }
                        string sName = objGlobalData.GetBranchShortNameByID(branch);
                        DataSet dsData = objGlobalData.GetReportNo("CustSatisfaction", "", sName);

                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            ref_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }
                        objGlobalData.ExecuteQuery("Update t_cust_satisfaction set ref_no = '" + ref_no + "' where id_cust_satisfaction= '" + id_cust_satisfaction + "'");

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCustomerSatisfaction: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddPerfList(CustSatisfactionModelsList objQList)
        {
            try
            {
                string sSqlstmt = "delete from t_cust_satisfaction_perf where id_cust_satisfaction='" + objQList.CSList[0].id_cust_satisfaction + "'; ";

                for (int i = 0; i < objQList.CSList.Count; i++)
                {
                    string sid_perf = "null";
                    if (objQList.CSList[i].id_perf != null)
                    {
                        sid_perf = objQList.CSList[i].id_perf;
                    }
                    sSqlstmt = sSqlstmt + " insert into t_cust_satisfaction_perf (id_perf, id_cust_satisfaction, performance, perf_remarks, perf_upload";
                    string sFieldValue = "", sFields = "", sValue = "", sStatement = ""; ;
                   
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sid_perf + ", " + objQList.CSList[0].id_cust_satisfaction
                    + ",'" + objQList.CSList[i].performance + "','" + objQList.CSList[i].perf_remarks
                    + "','" + objQList.CSList[i].perf_upload + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                     + " id_perf= values(id_perf), id_cust_satisfaction= values(id_cust_satisfaction), performance = values(performance), perf_remarks= values(perf_remarks), perf_upload= values(perf_upload)";
                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";


                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPerfList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddDeliveryList(CustSatisfactionModelsList objDList)
        {
            try
            {
                string sSqlstmt = "delete from t_cust_satisfaction_delivery where id_cust_satisfaction='" + objDList.CSList[0].id_cust_satisfaction + "'; ";

                for (int i = 0; i < objDList.CSList.Count; i++)
                {
                    string sid_delivery = "null";
                    if (objDList.CSList[i].id_delivery != null)
                    {
                        sid_delivery = objDList.CSList[i].id_delivery;
                    }
                    sSqlstmt = sSqlstmt + " insert into t_cust_satisfaction_delivery (id_delivery, id_cust_satisfaction, delivery, delivery_remarks, delivery_upload";
                    string sFieldValue = "", sFields = "", sValue = "", sStatement = ""; ;

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sid_delivery + ", " + objDList.CSList[0].id_cust_satisfaction
                    + ",'" + objDList.CSList[i].delivery + "','" + objDList.CSList[i].delivery_remarks
                    + "','" + objDList.CSList[i].delivery_upload + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                     + " id_delivery= values(id_delivery), id_cust_satisfaction= values(id_cust_satisfaction), delivery = values(delivery), delivery_remarks= values(delivery_remarks), delivery_upload= values(delivery_upload)";
                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";


                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDeliveryList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddComplaintList(CustSatisfactionModelsList objCList)
        {
            try
            {
                string sSqlstmt = "delete from t_cust_satisfaction_complaints where id_cust_satisfaction='" + objCList.CSList[0].id_cust_satisfaction + "'; ";

                for (int i = 0; i < objCList.CSList.Count; i++)
                {
                    string sid_complaint = "null";
                    if (objCList.CSList[i].id_complaint != null)
                    {
                        sid_complaint = objCList.CSList[i].id_complaint;
                    }
                    sSqlstmt = sSqlstmt + " insert into t_cust_satisfaction_complaints (id_complaint, id_cust_satisfaction, complaint_type, complaint_no,complaint_upload";
                    string sFieldValue = "", sFields = "", sValue = "", sStatement = ""; ;

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sid_complaint + ", " + objCList.CSList[0].id_cust_satisfaction
                    + ",'" + objCList.CSList[i].complaint_type
                    + "','" + objCList.CSList[i].complaint_no + "','" + objCList.CSList[i].complaint_upload + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                     + " id_complaint= values(id_complaint), id_cust_satisfaction= values(id_cust_satisfaction),complaint_type= values(complaint_type), complaint_no= values(complaint_no), complaint_upload= values(complaint_upload)";
                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";


                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddComplaintList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateActionInitiated(CustSatisfactionModels objModel, CustSatisfactionModelsList objQList)
        {
            try
            {
                string sSqlstmt = "update t_cust_satisfaction set action_initiated_by='" + action_initiated_by + "'";
                sSqlstmt = sSqlstmt + " where id_cust_satisfaction='" + objModel.id_cust_satisfaction + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objQList.CSList.Count) > 0)
                    {

                        FunAddActionList(objQList);
                    }
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunEditCustomerSatisfaction: " + ex.ToString());
            }
            return false;
        }


        internal bool FunAddActionList(CustSatisfactionModelsList objQList)
        {
            try
            {
                string sSqlstmt = "delete from t_cust_satisfaction_act where id_cust_satisfaction='" + objQList.CSList[0].id_cust_satisfaction + "'; ";

                for (int i = 0; i < objQList.CSList.Count; i++)
                {
                    string sid_act = "null";
                    if (objQList.CSList[i].id_act != null)
                    {
                        sid_act = objQList.CSList[i].id_act;
                    }
                    sSqlstmt = sSqlstmt + " insert into t_cust_satisfaction_act (id_act, id_cust_satisfaction, action_taken, pers_resp, resource_req";
                    string sFieldValue = "", sFields = "", sValue = "", sStatement = ""; ;
                    if (objQList.CSList[i].target_date != null && objQList.CSList[i].target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", target_date";
                        sFieldValue = sFieldValue + ", '" + objQList.CSList[i].target_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sid_act + ", " + objQList.CSList[0].id_cust_satisfaction
                    + ",'" + objQList.CSList[i].action_taken + "','" + objQList.CSList[i].pers_resp
                    + "','" + objQList.CSList[i].resource_req + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                     + " id_act= values(id_act), id_cust_satisfaction= values(id_cust_satisfaction), action_taken = values(action_taken), pers_resp= values(pers_resp), resource_req= values(resource_req)";
                    if (objQList.CSList[i].target_date != null && objQList.CSList[i].target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sValue = sValue + ", target_date= values(target_date)";
                       
                    }

                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";


                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddActionList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunEditCustomerSatisfaction(CustSatisfactionModels objModel, CustSatisfactionModelsList objQList, CustSatisfactionModelsList objDList, CustSatisfactionModelsList objCList)
        {
            try
            {
                string sSqlstmt = "update t_cust_satisfaction set branch='" + branch + "',Department='" + Department + "',Location='" + Location + "',cust_name='" + cust_name + "',prod_delivered='" + prod_delivered + "',contact_details='" + contact_details + "',repeat_enquiries='" + repeat_enquiries + "',no_enquiries='" + no_enquiries + "',repeat_orders='" + repeat_orders + "',"
                    + "no_orders='" + no_orders + "',enquiries_upload='" + enquiries_upload + "',orders_upload='" + orders_upload + "',survey_form='" + survey_form + "',csi='" + csi + "',reviewed_by='" + reviewed_by + "',complaint='" + complaint + "',cust_satisfied='" + cust_satisfied + "'";

                if (objModel.frm_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", frm_date='" + objModel.frm_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                if (objModel.to_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", to_date='" + objModel.to_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                if (objModel.eval_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", eval_date='" + objModel.eval_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where id_cust_satisfaction='" + objModel.id_cust_satisfaction + "'";

                if(objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objQList.CSList.Count) > 0)
                    {
                        objQList.CSList[0].id_cust_satisfaction = id_cust_satisfaction.ToString();

                        FunAddPerfList(objQList);
                    }
                    if (Convert.ToInt32(objDList.CSList.Count) > 0)
                    {
                        objDList.CSList[0].id_cust_satisfaction = id_cust_satisfaction.ToString();

                        FunAddDeliveryList(objDList);
                    }
                    if (Convert.ToInt32(objCList.CSList.Count) > 0)
                    {
                        objCList.CSList[0].id_cust_satisfaction = id_cust_satisfaction.ToString();

                        FunAddComplaintList(objCList);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunEditCustomerSatisfaction: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteCustomerSatisfaction(string sId)
        {
            try
            {
                string sSqlstmt = "update t_cust_satisfaction set Active=0 where id_cust_satisfaction=" + sId;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCustomerSatisfaction: " + ex.ToString());
            }
            return false;
        }

        internal bool FunCustomerSatisfactionReview(CustSatisfactionModels objModel)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sSqlstmt = "update t_cust_satisfaction set review_status='" + review_status + "',review_comment='" + review_comment + "',review_date='" + sApprovedDate + "' where id_cust_satisfaction='" + id_cust_satisfaction + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
               
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunTrainingPlanReview: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateActionList(CustSatisfactionModelsList objActionList)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objActionList.CSList.Count; i++)
                {

                    sSqlstmt = sSqlstmt + "update t_cust_satisfaction_act set  act_status='" + objActionList.CSList[i].act_status
                    + "', remarks='" + objActionList.CSList[i].remarks + "'";

                    if (objActionList.CSList[i].update_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sSqlstmt = sSqlstmt + ", update_date='" + objActionList.CSList[i].update_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                    }

                    sSqlstmt = sSqlstmt + " where id_act='" + objActionList.CSList[i].id_act + "';";

                }

                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateActionList: " + ex.ToString());
            }
            return false;
        }
    }

    public class CustSatisfactionModelsList
    {
        public List<CustSatisfactionModels> CSList { get; set; }
    }

}