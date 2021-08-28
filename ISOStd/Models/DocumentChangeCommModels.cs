using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISOStd.Models
{
    public class DocumentChangeCommModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display (Name = "Id")]
        public string id_dcc { get; set; }

        [Display(Name = "Doc No. ")]
        public string doc_no { get; set; }

        [Display(Name = "Title")]
        public string title { get; set; }

        [Display(Name = "Current Date")]
        public DateTime current_date { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        [Display(Name = "Next Revision Due")]
        public DateTime next_revision_due { get; set; }

        [Display(Name = "Date of enforcement")]
        public DateTime enforcement_date { get; set; }

        [Display(Name = "Responsible Group")]
        public string resp_group { get; set; }        

        [Display(Name = "Major change")]
        public string major_changes { get; set; }

        [Display(Name = "Position to contact with Questions/ suggestions")]
        public string contact_position { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        [Display(Name = "Inform in Operations Meeting ")]
        public string inform_om_by { get; set; }

        [Display(Name = "")]
        public DateTime inform_om_date { get; set; }

        [Display(Name = "Inclusion in all Meetings agenda " +
            "(to inform all about the new document - to be minuted.)")]
        public string meeting_by { get; set; }

        [Display(Name = "")]
        public DateTime meeting_date { get; set; }

        [Display(Name = "Sign off Sheet for the intended recipients" +
            " (A separate Sheet mentioning that the document has been" +
            " released and the signatory is aware of its existence.Sheet attached." +
            "The  Sheet will be returned to IMS Rep at the respective sites).")]
        public string signoffsheet_by { get; set; }

        [Display(Name = "")]
        public DateTime signoffsheet_date { get; set; }

        [Display(Name = "Presentation / Seminar a.Special presentation" +
            " b.In Operation meetings c.Upon request from other groups")]
        public string presentation_by { get; set; }

        [Display(Name = "")]
        public DateTime presentation_date { get; set; }

        [Display(Name = "Toolbox/ Safety talk: Safety Engineers shall " +
            "carry out safety talk at the respective sites for Supervisors " +
            "and Operators working in Ops area.")]
        public string toolbox_by { get; set; }

        [Display(Name = "")]
        public DateTime toolbox_date { get; set; }

        [Display(Name = "Inform Contractor’s Key Personnel")]
        public string inform_contractor_by { get; set; }

        [Display(Name = "")]
        public DateTime inform_contractor_date { get; set; }

        [Display(Name = "Flyers")]
        public string flyers_by { get; set; }

        [Display(Name = "")]
        public DateTime flyers_date { get; set; }

        [Display(Name = "E-mail/Webmaster message/Popup")]
        public string email_by { get; set; }

        [Display(Name = "")]
        public DateTime email_date { get; set; }

        [Display(Name = "Campaign")]
        public string campaign_by { get; set; }

        [Display(Name = "")]
        public DateTime campaign_date { get; set; }

        [Display(Name = "Intranet: Intranet, Homepage announcement   ")]
        public string intranet_by { get; set; }

        [Display(Name = "")]
        public DateTime intranet_date { get; set; }

        [Display(Name = "Bulletin")]
        public string bulletin_by { get; set; }

        [Display(Name = "")]
        public DateTime bulletin_date { get; set; }

        [Display(Name = "Classroom training (to be incorporated in existing training courses)")]
        public string training_classroom_by { get; set; }

        [Display(Name = "Date")]
        public DateTime training_classroom_date { get; set; }

        [Display(Name = "Toolbox/ safety talks")]
        public string training_toolbox_by { get; set; }

        [Display(Name = "Date")]
        public DateTime training_toolbox_date { get; set; }

        [Display(Name = "On job training")]
        public string training_job_by { get; set; }

        [Display(Name = "")]
        public DateTime training_job_date { get; set; }

        [Display(Name = "Certificate")]
        public string certificate_by { get; set; }

        [Display(Name = "")]
        public DateTime certificate_date { get; set; }

        [Display(Name = "Hardware")]
        public string hardware_by { get; set; }

        [Display(Name = "")]
        public DateTime hardware_date { get; set; }

        [Display(Name = "Register/log")]
        public string register_by { get; set; }

        [Display(Name = "")]
        public DateTime register_date { get; set; }

        [Display(Name = "Records")]
        public string record_by { get; set; }

        [Display(Name = "")]
        public DateTime record_date { get; set; }

        [Display(Name = "Forms")]
        public string form_by { get; set; }

        [Display(Name = "")]
        public DateTime form_date { get; set; }

        [Display(Name = "Documentation")]
        public string document_by { get; set; }

        [Display(Name = "")]
        public DateTime document_date { get; set; }

        [Display(Name = "Monitoring ")]
        public string monitoring_by { get; set; }

        [Display(Name = "")]
        public DateTime monitoring_date { get; set; }

        [Display(Name = "Others [Please specify] Remove obsolete copies ")]
        public string others { get; set; }

        [Display(Name = "")]
        public string others_by { get; set; }

        [Display(Name = "")]
        public DateTime others_date { get; set; }

        internal bool FunAddDocChangeComm(DocumentChangeCommModels objModel)
        {
            try
            {
                string sSqlstmt = "insert into monitoring_by(doc_no,title,status,resp_group,contact_position," +
                    "major_changes,remarks,inform_om_by,meeting_by,signoffsheet_by,presentation_by,toolbox_by,inform_contractor_by," +
                    "flyers_by,email_by,campaign_by,intranet_by,bulletin_by,training_classroom_by,training_toolbox_by,training_job_by," +
                    "certificate_by,hardware_by,register_by,record_by,form_by,document_by,monitoring_by,others,others_by,loggedby";

                string sFields = "", sFieldValue = "";

                if (objModel.current_date != null && objModel.current_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", current_date";
                    sFieldValue = sFieldValue + ", '" + objModel.current_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.next_revision_due != null && objModel.next_revision_due > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", next_revision_due";
                    sFieldValue = sFieldValue + ", '" + objModel.next_revision_due.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.enforcement_date != null && objModel.enforcement_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", enforcement_date";
                    sFieldValue = sFieldValue + ", '" + objModel.enforcement_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.inform_om_date != null && objModel.inform_om_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", inform_om_date";
                    sFieldValue = sFieldValue + ", '" + objModel.inform_om_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.meeting_date != null && objModel.meeting_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", meeting_date";
                    sFieldValue = sFieldValue + ", '" + objModel.meeting_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.signoffsheet_date != null && objModel.signoffsheet_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", signoffsheet_date";
                    sFieldValue = sFieldValue + ", '" + objModel.signoffsheet_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.presentation_date != null && objModel.presentation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", presentation_date";
                    sFieldValue = sFieldValue + ", '" + objModel.presentation_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.toolbox_date != null && objModel.toolbox_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", toolbox_date";
                    sFieldValue = sFieldValue + ", '" + objModel.toolbox_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.inform_contractor_date != null && objModel.inform_contractor_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", inform_contractor_date";
                    sFieldValue = sFieldValue + ", '" + objModel.inform_contractor_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.flyers_date != null && objModel.flyers_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", flyers_date";
                    sFieldValue = sFieldValue + ", '" + objModel.flyers_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.email_date != null && objModel.email_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", email_date";
                    sFieldValue = sFieldValue + ", '" + objModel.email_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.campaign_date != null && objModel.campaign_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", campaign_date";
                    sFieldValue = sFieldValue + ", '" + objModel.campaign_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.intranet_date != null && objModel.intranet_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", intranet_date";
                    sFieldValue = sFieldValue + ", '" + objModel.intranet_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.bulletin_date != null && objModel.bulletin_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", bulletin_date";
                    sFieldValue = sFieldValue + ", '" + objModel.bulletin_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.training_classroom_date != null && objModel.training_classroom_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", training_classroom_date";
                    sFieldValue = sFieldValue + ", '" + objModel.training_classroom_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.training_toolbox_date != null && objModel.training_toolbox_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", training_toolbox_date";
                    sFieldValue = sFieldValue + ", '" + objModel.training_toolbox_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.training_job_date != null && objModel.training_job_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", training_job_date";
                    sFieldValue = sFieldValue + ", '" + objModel.training_job_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.certificate_date != null && objModel.certificate_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", certificate_date";
                    sFieldValue = sFieldValue + ", '" + objModel.certificate_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.hardware_date != null && objModel.hardware_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", hardware_date";
                    sFieldValue = sFieldValue + ", '" + objModel.hardware_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.register_date != null && objModel.register_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", register_date";
                    sFieldValue = sFieldValue + ", '" + objModel.register_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.record_date != null && objModel.record_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", record_date";
                    sFieldValue = sFieldValue + ", '" + objModel.record_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.form_date != null && objModel.form_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", form_date";
                    sFieldValue = sFieldValue + ", '" + objModel.form_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.document_date != null && objModel.document_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", document_date";
                    sFieldValue = sFieldValue + ", '" + objModel.document_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.monitoring_date != null && objModel.monitoring_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", monitoring_date";
                    sFieldValue = sFieldValue + ", '" + objModel.monitoring_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.others_date != null && objModel.others_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", others_date";
                    sFieldValue = sFieldValue + ", '" + objModel.others_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objModel.doc_no + "','" + objModel.title + "','" + objModel.status + "','" + objModel.resp_group + "','" + objModel.contact_position
                    + "','" + objModel.major_changes + "','" + objModel.remarks + "','" + objModel.inform_om_by + "','" + objModel.meeting_by + "','" + objModel.signoffsheet_by + "','" + objModel.presentation_by
                    + "','" + objModel.toolbox_by + "','" + objModel.inform_contractor_by + "','" + objModel.flyers_by + "','" + objModel.email_by + "','" + objModel.campaign_by + "','" + objModel.intranet_by
                    + "','" + objModel.bulletin_by + "','" + objModel.training_classroom_by + "','" + objModel.training_toolbox_by + "','" + objModel.training_job_by + "','" + objModel.certificate_by 
                    + "','" + objModel.hardware_by + "','" + objModel.register_by + "','" + objModel.record_by + "','" + objModel.form_by 
                    + "','" + objModel.document_by + "','" + objModel.monitoring_by + "','" + objModel.others + "','" + objModel.others_by +"','"+ objGlobalData.GetCurrentUserSession().empid+ "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDocChangeComm: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateDocChangeComm(DocumentChangeCommModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_document_change_communication set doc_no ='" + objModel.doc_no + "',title ='" + objModel.title + "',status ='" + objModel.status + "'"
                    + ",resp_group ='" + objModel.resp_group + "',contact_position ='" + objModel.contact_position + "',major_changes ='" + objModel.major_changes + "',remarks ='" + objModel.remarks
                    + "',inform_om_by ='" + objModel.inform_om_by + "',meeting_by ='" + objModel.meeting_by + "',signoffsheet_by ='" + objModel.signoffsheet_by + "',presentation_by ='" + objModel.presentation_by
                    + "',toolbox_by ='" + objModel.toolbox_by + "',inform_contractor_by ='" + objModel.inform_contractor_by + "',flyers_by ='" + objModel.flyers_by + "',email_by ='" + objModel.email_by
                    + "',campaign_by ='" + objModel.campaign_by + "',intranet_by ='" + objModel.intranet_by + "',bulletin_by ='" + objModel.bulletin_by + "',training_classroom_by ='" + objModel.training_classroom_by
                    + "',training_toolbox_by ='" + objModel.training_toolbox_by + "',training_job_by ='" + objModel.training_job_by + "',certificate_by ='" + objModel.certificate_by + "',hardware_by ='" + objModel.hardware_by
                    + "',register_by ='" + objModel.register_by + "',record_by ='" + objModel.record_by + "',form_by ='" + objModel.form_by + "',document_by ='" + objModel.document_by + "',monitoring_by ='" + objModel.monitoring_by
                    + "',others ='" + objModel.others + "',others_by ='" + objModel.others_by + "'";

                if (objModel.current_date != null && objModel.current_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", current_date='" + objModel.current_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.next_revision_due != null && objModel.next_revision_due > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", next_revision_due='" + objModel.next_revision_due.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.enforcement_date != null && objModel.enforcement_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", enforcement_date='" + objModel.enforcement_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.inform_om_date != null && objModel.inform_om_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", inform_om_date='" + objModel.inform_om_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.meeting_date != null && objModel.meeting_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", meeting_date='" + objModel.meeting_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.signoffsheet_date != null && objModel.signoffsheet_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", signoffsheet_date='" + objModel.signoffsheet_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.presentation_date != null && objModel.presentation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", presentation_date='" + objModel.presentation_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.toolbox_date != null && objModel.toolbox_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", toolbox_date='" + objModel.toolbox_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.inform_contractor_date != null && objModel.inform_contractor_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", inform_contractor_date='" + objModel.inform_contractor_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.flyers_date != null && objModel.flyers_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", flyers_date='" + objModel.flyers_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.email_date != null && objModel.email_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", email_date='" + objModel.email_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.campaign_date != null && objModel.campaign_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", campaign_date='" + objModel.campaign_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.intranet_date != null && objModel.intranet_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", intranet_date='" + objModel.intranet_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.bulletin_date != null && objModel.bulletin_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", bulletin_date='" + objModel.bulletin_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.training_classroom_date != null && objModel.training_classroom_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", training_classroom_date='" + objModel.training_classroom_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.training_toolbox_date != null && objModel.training_toolbox_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", training_toolbox_date='" + objModel.training_toolbox_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.training_job_date != null && objModel.training_job_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", training_job_date='" + objModel.training_job_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.certificate_date != null && objModel.certificate_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", certificate_date='" + objModel.certificate_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.hardware_date != null && objModel.hardware_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", hardware_date='" + objModel.hardware_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.register_date != null && objModel.register_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", register_date='" + objModel.register_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.record_date != null && objModel.record_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", record_date='" + objModel.record_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.form_date != null && objModel.form_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", form_date='" + objModel.form_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.document_date != null && objModel.document_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", document_date='" + objModel.document_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.monitoring_date != null && objModel.monitoring_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", monitoring_date='" + objModel.monitoring_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.others_date != null && objModel.others_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", others_date='" + objModel.others_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_dcc='" + objModel.id_dcc + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDocChangeComm: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteDocChangeComm(string sid_dcc)
        {
            try
            {
                string sSqlstmt = "update t_document_change_communication set Active=0 where id_dcc='" + sid_dcc + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDocChangeComm: " + ex.ToString());
            }
            return false;
        }
    }
}