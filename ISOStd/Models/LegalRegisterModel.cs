using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace ISOStd.Models
{

    public class LegalRegisterModel
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public int LegalRequirement_Id { get; set; }

        //[Required]
        //[Display(Name = "Id")]
        //    public int id_requirements { get; set; }

        //    public int legalregister_evaluation_Id { get; set; }
               
        [Required]
        [Display(Name = "Initial Development Date")]
        public DateTime initialdevelopmentdate { get; set; }

        [Required]
        [Display(Name = "Law Number")]
        [System.Web.Mvc.Remote("doesLawNumberExist", "LegalRegister", HttpMethod = "POST", ErrorMessage = " This Law  Number already exists. Please enter a different Law Nuber.")]
        public string lawNo { get; set; }

        [Required]
        [Display(Name = "Law Title")]
        [DataType(DataType.MultilineText)]
        public string lawTitle { get; set; }

        [Required]
        [Display(Name = "Origin of Requirements")]
        [DataType(DataType.MultilineText)]
        public string origin_of_requirement { get; set; }

        [Required]
        [Display(Name = "Document Storage Location")]
        public string document_storage_location { get; set; }

        [Required]
        [Display(Name = "Freq of Evaluation")]
        public string frequency_of_evaluation { get; set; }

        //[Required]
        [Display(Name = "Updated  By")]
        public string updatedByName { get; set; }

        // [Required]
        [Display(Name = "Active Status")]
        public string activeStatus { get; set; }

        //[Required]
        [Display(Name = "Updated on")]
        public DateTime updatedOn { get; set; }

        //[Required]
        [Display(Name = "Reviewed By")]
        public string reviewedBy { get; set; }

        [Required]
        [Display(Name = "Approved by")]
        public string approvedBy { get; set; }

        //[Required]
        [Display(Name = "Review Status")]
        public string reviewstatus { get; set; }

        // [Required]
        [Display(Name = "Approved Status")]
        public string approvestatus { get; set; }

        [Display(Name = "ID Requirement")]
        public int id_requirements { get; set; }

        // [Required]
        [Display(Name = "Article")]
        [DataType(DataType.MultilineText)]
        public string article { get; set; }

        [Required]
        [Display(Name = "Article Details")]
        [DataType(DataType.MultilineText)]
        public string requirements { get; set; }

        [Required]
        [Display(Name = "Procedure reference ")]
        [DataType(DataType.MultilineText)]
        public string reference { get; set; }

        [Required]
        [Display(Name = "monitoring requirements/parameters ")]
        [DataType(DataType.MultilineText)]
        public string monitoring { get; set; }


        //[Required(ErrorMessage = "{0} is required")]
        //[StringLength(100, MinimumLength = 3,
        // ErrorMessage = "Name Should be minimum 3 characters and a maximum of 100 characters")]
        //[DataType(DataType.Text)]
        //[AllowHtml]
        //public string requirements { get; set; }

        // [Required]
        [Display(Name = "If Non Applicable ,Justify")]
        [DataType(DataType.MultilineText)]
        public string nonapplicablejustify { get; set; }

        // [Required]
        [Display(Name = "Updated by")]
        public string updatedBynametrans { get; set; }

        [Required]
        [Display(Name = "Updated By Designation")]
        public string updatedByposition { get; set; }

        //[Required]
        [Display(Name = "Updated By Department")]
        public string updatedByDept { get; set; }

        //[Required]
        [Display(Name = "Updated Date")]
        public DateTime updatedDate { get; set; }

        // [Required]
        [Display(Name = "Is It Applicable ?")]
        public string applicable { get; set; }

        //[Required]
        [Display(Name = "Evaluation Date")]
        public DateTime LegalRegister_Eval_On { get; set; }

        //[Required]
        [Display(Name = "Due Date  for implementing the actions")]
        public DateTime Duedate { get; set; }

        [Required]
        [Display(Name = "Does it Comply ?")]
        public string comply { get; set; }

        // [Required]
        [Display(Name = "If Yes ,enter compliance Details")]
        [DataType(DataType.MultilineText)]
        public string yes_comply_reason { get; set; }

        //  [Required]
        [Display(Name = "If No ,enter compliance Details")]
        [DataType(DataType.MultilineText)]
        public string no_comply_reason { get; set; }

        [Required]
        [Display(Name = "Upload Records")]
        public string Evidence { get; set; }

        [Required]
        [Display(Name = "Personnel responsible")]
        public string personel_responsible { get; set; }

        [Required]
        [Display(Name = "Actions to be taken to comply with legal requirement")]
        public string Comply_action { get; set; }

        [Required]
        [Display(Name = "evaluation ID")]
        public int legalregister_evaluation_Id { get; set; }


        //[Required]
        //[Display(Name = " Personal_Responsible")]
        //public string Personal_Responsible { get; set; }

        [Display(Name = "Document")]
        public string upload { get; set; }

        [Display(Name = "ID")]
        public string id_law { get; set; }

        [Display(Name = "Standard")]
        public string Isostd { get; set; }

        [Display(Name = "Department")]
        public string deptid { get; set; }

        [Display(Name = "Compliance")]
        public string compliance { get; set; }

        [Display(Name = "URL")]
        public string url { get; set; }

        [Display(Name = "Evaluation Date")]
        public DateTime Eve_Date { get; set; }

        [Display(Name = "Revision Date")]
        public DateTime Revision_Date { get; set; }

        [Display(Name = "Issue No/Revision No ")]
        public string Revision_No { get; set; }

        [Display(Name = "Requirement")]
        public string requirement { get; set; }

        [Display(Name = "Compliance Description")]
        public string description { get; set; }

        [Display(Name = "Next Evaluation Date")]
        public DateTime nexteval_date { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        internal bool FunDeleteComplianceDoc(string sid_law)
        {
            try
            {
                string sSqlstmt = "update t_compliance_obligation set Active=0 where id_law='" + sid_law + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteComplianceDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddCompliance(LegalRegisterModel objComp)
        {
            try
            {
                string sSqlstmt = "insert into t_compliance_obligation (lawNo,Isostd,lawTitle,deptid,compliance,upload,url,Revision_No,requirement,description,branch,Location";
               // string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sFields = "", sFieldValue = "";
                if (objComp.Eve_Date != null && objComp.Eve_Date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Eve_Date";
                    sFieldValue = sFieldValue + ", '" + objComp.Eve_Date.ToString("yyyy/MM/dd") + "'";
                }
                if (objComp.nexteval_date != null && objComp.nexteval_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", nexteval_date";
                    sFieldValue = sFieldValue + ", '" + objComp.nexteval_date.ToString("yyyy/MM/dd") + "'";
                }
                
                if (objComp.Revision_Date != null && objComp.Revision_Date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Revision_Date";
                    sFieldValue = sFieldValue + ", '" + objComp.Revision_Date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ")values('" + objComp.lawNo + "','" + objComp.Isostd + "','" + objComp.lawTitle + "'"
                    + ",'" + objComp.deptid + "','" + objComp.compliance + "','" + objComp.upload + "','" + objComp.url + "','"
                    + objComp.Revision_No + "','" + objComp.requirement + "','" + objComp.description + "','" + objComp.branch + "','" + objComp.Location + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCompliance: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateCompliance(LegalRegisterModel objComp)
        {
            try
            {
                string sSqlstmt = "update t_compliance_obligation set lawNo ='" + objComp.lawNo + "', Isostd='" + objComp.Isostd + "', "
                    + "lawTitle='" + objComp.lawTitle + "',deptid='" + objComp.deptid + "',compliance='" + objComp.compliance + "',upload='" + objComp.upload + "',url='" + objComp.url + "',Revision_No='" + objComp.Revision_No + "'"
                    + ",requirement='" + objComp.requirement + "',description='" + objComp.description + "',branch='" + objComp.branch + "',Location='" + objComp.Location + "'";

                if (objComp.Eve_Date != null && objComp.Eve_Date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Eve_Date ='" + objComp.Eve_Date.ToString("yyyy/MM/dd") + "'";
                }
                if (objComp.nexteval_date != null && objComp.nexteval_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", nexteval_date ='" + objComp.nexteval_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objComp.Revision_Date != null && objComp.Revision_Date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Revision_Date ='" + objComp.Revision_Date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_law='" + objComp.id_law + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCompliance: " + ex.ToString());
            }

            return false;
        }

        internal bool FunDeleteLegalRequirementsDoc(string sLegalRequirement_Id)
        {
            try
            {
                string sSqlstmt = "update t_legalregister set Active=0 where LegalRequirement_Id='" + sLegalRequirement_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteLegalRequirementsDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddLegalRegister(LegalRegisterModel objLegalRegisterModel, LegalRegisterModelsList objLegalRegisterModelList)
        {
            try
            {
                string supdatedOn = objLegalRegisterModel.updatedOn.ToString("yyyy-MM-dd HH':'mm':'ss");
                //DateTime sTodayDate = DateTime.Now;
                //string Today = sTodayDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sinitialdevelopmentdate = objLegalRegisterModel.initialdevelopmentdate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sSqlstmt = "insert into t_legalregister (lawNo, lawTitle, initialdevelopmentdate, origin_of_requirement , document_storage_location,frequency_of_evaluation, activeStatus, updatedOn,"
                + "  reviewedBy,approvedBy,updatedByName,upload)"
            + " values('" + objLegalRegisterModel.lawNo + "','" + objLegalRegisterModel.lawTitle + "','" + sinitialdevelopmentdate + "','" + objLegalRegisterModel.origin_of_requirement
                + "','" + objLegalRegisterModel.document_storage_location + "','" + objLegalRegisterModel.frequency_of_evaluation + "','" + objLegalRegisterModel.activeStatus + "','"
                + supdatedOn + "','" + objLegalRegisterModel.reviewedBy + "','"
                + objLegalRegisterModel.approvedBy + "','" +
                objLegalRegisterModel.updatedByName + "','" + objLegalRegisterModel.upload + "')";

                return FunAddLegalRegisterTrans(objLegalRegisterModelList, objGlobalData.ExecuteQueryReturnId(sSqlstmt));
            }

            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddLegalRegister: " + ex.ToString());
            }
            return false;


        }

        internal bool FunAddLegalRegisterTrans(LegalRegisterModelsList objLegalRegisterModelList, int LegalRequirement_Id)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objLegalRegisterModelList.LegalRegisterMList.Count; i++)
                {
                    if (objLegalRegisterModelList.LegalRegisterMList[i].article != null)
                    {
                        string supdatedDate = objLegalRegisterModelList.LegalRegisterMList[i].updatedDate.ToString("yyyy-MM-dd HH':'mm':'ss");


                        sSqlstmt = sSqlstmt + "insert into t_legalrequirementstrans (LegalRequirement_Id, article,  requirements, applicable, nonapplicablejustify, updatedBynametrans, updatedByposition, "
                        + " updatedByDept, updatedDate,reference,monitoring)"
                            + " values('" + LegalRequirement_Id
                     + "','" + objLegalRegisterModelList.LegalRegisterMList[i].article + "','" + objLegalRegisterModelList.LegalRegisterMList[i].requirements
                     + "','" + objLegalRegisterModelList.LegalRegisterMList[i].applicable + "','" + objLegalRegisterModelList.LegalRegisterMList[i].nonapplicablejustify
                     + "','" + objLegalRegisterModelList.LegalRegisterMList[i].updatedBynametrans + "', '" + objLegalRegisterModelList.LegalRegisterMList[i].updatedByposition + "','" + objLegalRegisterModelList.LegalRegisterMList[i].updatedByDept
                     + "','" + supdatedDate + "','" + objLegalRegisterModelList.LegalRegisterMList[i].reference + "','" + objLegalRegisterModelList.LegalRegisterMList[i].monitoring + "'); ";

                    }

                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddLegalRegisterTrans: " + ex.ToString());
            }
            return false;

        }

        public bool checkLawNumberExists(string lawNo)
        {
            try
            {
                string sSqlstmt = "select LegalRequirement_Id from t_legalregister where lawNo='" + lawNo + "'";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkLawNumberExists: " + ex.ToString());
            }
            return true;
        }


        internal bool FunAddLegalRegisterEvaluation(LegalRegisterModel objLegalRegisterModel)
        {

            try
            {
                string sDuedate = objLegalRegisterModel.Duedate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sLegalRegister_Eval_On = objLegalRegisterModel.LegalRegister_Eval_On.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into t_legalregister_evaluation (id_requirements, LegalRegister_Eval_On, Duedate, comply, yes_comply_reason, no_comply_reason,Comply_action,personel_responsible";

                if (objLegalRegisterModel.Evidence != null && objLegalRegisterModel.Evidence != "")
                {
                    sSqlstmt = sSqlstmt + ", Evidence";
                }
                sSqlstmt = sSqlstmt + ") values('" + objLegalRegisterModel.id_requirements + "','" + sLegalRegister_Eval_On + "','" + sDuedate
                + "','" + objLegalRegisterModel.comply + "','" + objLegalRegisterModel.yes_comply_reason + "','" + objLegalRegisterModel.no_comply_reason + "','" + objLegalRegisterModel.Comply_action + "','" + objLegalRegisterModel.personel_responsible + "'";

                if (objLegalRegisterModel.Evidence != null && objLegalRegisterModel.Evidence != "")
                {
                    sSqlstmt = sSqlstmt + ", '" + objLegalRegisterModel.Evidence + "'";
                }
                sSqlstmt = sSqlstmt + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddLegalRegisterEvaluation: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateLegalRegister(LegalRegisterModel objLegalRegisterModel)
        {
            try
            {
                string sinitialdevelopmentdate = objLegalRegisterModel.initialdevelopmentdate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string supdatedOn = objLegalRegisterModel.updatedOn.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_legalregister set lawTitle='" + objLegalRegisterModel.lawTitle + "', updatedOn = '" + supdatedOn
                    + "', frequency_of_evaluation= '" + objLegalRegisterModel.frequency_of_evaluation + "', origin_of_requirement='" + objLegalRegisterModel.origin_of_requirement
                    + "', document_storage_location='" + objLegalRegisterModel.document_storage_location + "', activeStatus='" + objLegalRegisterModel.activeStatus + "', updatedOn='" + supdatedOn
                  + "', updatedByName='" + objLegalRegisterModel.updatedByName + "', reviewedBy='" + objLegalRegisterModel.reviewedBy + "', approvedBy='"
                    + objLegalRegisterModel.approvedBy + "'";
                if (objLegalRegisterModel.upload != null && objLegalRegisterModel.upload != "")
                {
                    sSqlstmt = sSqlstmt + ", upload='" + objLegalRegisterModel.upload + "'";
                }
                sSqlstmt = sSqlstmt + " where LegalRequirement_Id='" + objLegalRegisterModel.LegalRequirement_Id + "'";
                //sSqlstmt = sSqlstmt + "update t_legalrequirementstrans set article='" + objLegalRegisterModel.article + "' where LegalRequirement_Id='"
                //        + objLegalRegisterModel.LegalRequirement_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateLegalRegister: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateLegisterRegisterPlan(LegalRegisterModel objLegalRegisterModel)
        {
            try
            {
                string supdatedDate = objLegalRegisterModel.updatedDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_legalrequirementstrans set updatedDate ='" + supdatedDate + "', article='" + objLegalRegisterModel.article + "', "
                    + "requirements='" + objLegalRegisterModel.requirements + "', applicable='" + objLegalRegisterModel.applicable
                    + "', nonapplicablejustify='" + objLegalRegisterModel.nonapplicablejustify + "', updatedByposition='" + objLegalRegisterModel.updatedByposition + "', updatedByDept='" + objLegalRegisterModel.updatedByDept
                    + "', updatedBynametrans='" + objLegalRegisterModel.updatedBynametrans + "', reference='" + objLegalRegisterModel.reference + "', monitoring='" + objLegalRegisterModel.monitoring + "'";

                sSqlstmt = sSqlstmt + " where id_requirements='" + objLegalRegisterModel.id_requirements + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateLegisterRegisterPlan: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateLegalRegisterEvaluation(LegalRegisterModel objLegalRegisterModel)
        {
            try
            {
                string sLegalRegister_Eval_On = objLegalRegisterModel.LegalRegister_Eval_On.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sDuedate = objLegalRegisterModel.Duedate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_legalregister_evaluation set LegalRegister_Eval_On ='" + sLegalRegister_Eval_On
                    + "', Duedate='" + sDuedate + "', comply='" + objLegalRegisterModel.comply + "', yes_comply_reason='" + objLegalRegisterModel.yes_comply_reason + "', no_comply_reason='" + objLegalRegisterModel.no_comply_reason
                    + "', Comply_action='" + objLegalRegisterModel.Comply_action + "', personel_responsible='" + objLegalRegisterModel.personel_responsible + "'";


                if (objLegalRegisterModel.Evidence != null && objLegalRegisterModel.Evidence != "")
                {
                    sSqlstmt = sSqlstmt + ", Evidence='" + objLegalRegisterModel.Evidence + "'";
                }
                sSqlstmt = sSqlstmt + " where legalregister_evaluation_Id='" + objLegalRegisterModel.legalregister_evaluation_Id + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateLegalRegisterEvaluation: " + ex.ToString());
            }
            return false;
        }

        public List<string> GetLegalRegisterRef(string sDeptId)
        {
            List<string> lstObjectives = new List<string>();
            try
            {
                DataSet dsObjectives = objGlobalData.Getdetails("select lawNo, frequency_of_evaluation,origin_of_requirement, document_storage_location, Approved_By, activeStatus "
                    + "from t_legalregister where updatedByName='" + sDeptId + "'");
                if (dsObjectives.Tables.Count > 0 && dsObjectives.Tables[0].Rows.Count > 0)
                {
                    lstObjectives = dsObjectives.Tables[0].AsEnumerable().Select(r => r[0].ToString()).ToList();
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetLegalRegisterRef: " + ex.ToString());
            }
            return lstObjectives;
        }

        public DataSet GetLegalRequirementsDetails(string lawo)
        {
            DataSet dsObjectives = new DataSet();
            try
            {
                dsObjectives = objGlobalData.Getdetails("select * from t_legalregister where lawo='" + lawo + "'");
                // DataSet dsObjectives = objGlobalData.Getdetails("select * from t_legalregister where LegalRequirement_Id=1");
                if (dsObjectives.Tables.Count > 0 && dsObjectives.Tables[0].Rows.Count > 0)
                {
                    return (dsObjectives);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetLegalRequirementsDetails: " + ex.ToString());
            }
            return dsObjectives;
        }

        public bool checkObjRefExists(string lawNo)
        {
            try
            {
                string sSqlstmt = "select LegalRequirement_Id from t_legalregister where lawNo='" + lawNo + "'";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkObjRefExists: " + ex.ToString());
            }
            return true;
        }

        internal bool FunUpdateLegalRegisterApproval(string approvedBy, string LegalRequirement_Id)
        {

            try
            {
                string sSqlstmt = "update t_legalregister set approvestatus ='1' where approvedBy='" + approvedBy + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateLegalRegisterApproval: " + ex.ToString());
            }
            return false;
        }

        //internal bool FunAddComments(string sEmpId, string sComments, string LegalRequirement_Id)
        //{
        //    string sCommentDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

        //    string sSqlstmt = "insert into  t_objectives_comments (LegalRequirement_Id, empId, Comments, CommentDate) values('" + LegalRequirement_Id + "','" + sEmpId + "','"
        //        + sComments + "','" + sCommentDate + "')";

        //    return objGlobalData.ExecuteQuery(sSqlstmt);
        //}



        internal bool FunUpdateLegalRegisterReview(string sLegalRequirement_Id, string lawno)
        {
            //    string sReviewedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
            try
            {
                string sSqlstmt = "update t_legalregister set reviewstatus='1' where LegalRequirement_Id='" + sLegalRequirement_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateLegalRegisterReview: " + ex.ToString());
            }
            return false;
        }



    }


    public class LegalRegisterModelsList
    {
        public List<LegalRegisterModel> LegalRegisterMList { get; set; }
    }
}

