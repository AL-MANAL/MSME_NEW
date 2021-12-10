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
    public class ProjectMgmtModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int id_projectmgmt { get; set; }

        [Display(Name = "Project No")]
        public string ProjectNo { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Company")]
        public string Location { get; set; }

        [Display(Name = "Customer")]
        public string Customer { get; set; }

        [Display(Name = "Project Manager")]
        public string ProjectManager { get; set; }


        [Display(Name = "Team Members")]
        public string Team { get; set; }

        [Display(Name = "Project Documents")]
        public string ProjectDocs { get; set; }

        [Display(Name = "Project Status")]
        public string ProjectStatus { get; set; }

        [Display(Name = "Project Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Planned End Date")]
        public DateTime PlannedEndDate { get; set; }

        [Display(Name = "Actual End Date")]
        public DateTime ActualEndDate { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }


        [Display(Name = "Id")]
        public int id_projectdesign { get; set; }

        [Display(Name = "Dicipline")]
        public string Dicipline { get; set; }

        [Display(Name = "Internal RevNo")]
        public int IntRevno { get; set; }

        [Display(Name = "Drawing No")]
        public string DrawingNumber { get; set; }

        [Display(Name = "Drawing Upload")]
        public string Upload { get; set; }

        [Display(Name = "Reviewed By")]
        public string ReviewedBy { get; set; }

        [Display(Name = "Reviewed Date")]
        public DateTime ReviewedDate { get; set; }

        [Display(Name = "Reviewer comment")]
        public string ReviewerComment { get; set; }

        [Display(Name = "Review Status")]
        public string ReviewStatus { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime ApprovedDate { get; set; }

        [Display(Name = "Approver comment")]
        public string ApproverComment { get; set; }

        [Display(Name = "Approve Status")]
        public string ApproveStatus { get; set; }

        public string LoggedBy { get; set; }

        [Display(Name = "Logged Date")]
        public DateTime LoggedDate { get; set; }

        [Display(Name = "Customer Approved Date")]
        public DateTime CustApprDate { get; set; }

        [Display(Name = "Design Sent to Customer")]
        public DateTime DgnSntDate { get; set; }

        [Display(Name = "Customer Feedback")]
        public string CustFeedback { get; set; }

        [Display(Name = "Rev No")]
        public string RevNo { get; set; }

        [Display(Name = "Customer Approval Status")]
        public string CustApprStatus { get; set; }

        internal bool FunDeleteProjectDoc(string sid_projectmgmt)
        {
            try
            {
                string sSqlstmt = "update t_projectmgmt set Active=0 where id_projectmgmt='" + sid_projectmgmt + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteProjectDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddProjectDetails(ProjectMgmtList lstDesign,ProjectMgmtModels objPro)
        {

            try
            {
                string sStartDate = objPro.StartDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sPlannedEndDate = objPro.PlannedEndDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sActualEndDate = objPro.ActualEndDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sLoggedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";
              
                    user = objGlobalData.GetCurrentUserSession().empid;
                
                string sSqlstmt = "insert into t_projectmgmt (ProjectNo,ProjectName,Location,Customer,ProjectManager,Team,ProjectDocs,ProjectStatus"
                + ",StartDate,PlannedEndDate,ActualEndDate,Remarks,LoggedBy,LoggedDate";
                           
                sSqlstmt = sSqlstmt + ") values('" + objPro.ProjectNo + "','" + objPro.ProjectName + "','" + objPro.Location + "',"
                + "'" + objPro.Customer + "','" + objPro.ProjectManager + "','" + objPro.Team + "','" + objPro.ProjectDocs + "','" + objPro.ProjectStatus + "','" + sStartDate + "',"
                + "'" + sPlannedEndDate + "','" + sActualEndDate + "','" + objPro.Remarks + "','" + user + "','" + sLoggedDate + "')";

                return FunAddDesignReviewProcess(lstDesign, objGlobalData.ExecuteQueryReturnId(sSqlstmt), objPro);
            
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddProjectDetails: " + ex.ToString());
            }
             return false;
        }
        internal bool FunUpdateDesignReviewProcess(ProjectMgmtList lstDesign, int id_projectmgmt, ProjectMgmtModels objPro)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < lstDesign.lstPrj.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_projectdesign (id_projectmgmt,Dicipline,IntRevno,DrawingNumber,Upload,ReviewedBy,ApprovedBy,"
                        + "CustFeedback,RevNo,CustApprStatus";
                    string sFields = "", sFieldValue = "";

                    if (lstDesign.lstPrj[i].CustApprDate != null && lstDesign.lstPrj[i].CustApprDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", CustApprDate";
                        sFieldValue = sFieldValue + ", '" + lstDesign.lstPrj[i].CustApprDate.ToString("yyyy/MM/dd") + "'";
                    }

                    if (lstDesign.lstPrj[i].DgnSntDate != null && lstDesign.lstPrj[i].DgnSntDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", DgnSntDate";
                        sFieldValue = sFieldValue + ", '" + lstDesign.lstPrj[i].DgnSntDate.ToString("yyyy/MM/dd") + "'";
                    }

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + id_projectmgmt + "','" + lstDesign.lstPrj[i].Dicipline
                   + "','" + lstDesign.lstPrj[i].IntRevno + "','" + lstDesign.lstPrj[i].DrawingNumber + "','" + lstDesign.lstPrj[i].Upload + "','" + lstDesign.lstPrj[i].ReviewedBy + "','" + lstDesign.lstPrj[i].ApprovedBy + "'"
                    + ",'" + lstDesign.lstPrj[i].CustFeedback + "','" + lstDesign.lstPrj[i].RevNo + "','" + lstDesign.lstPrj[i].CustApprStatus + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                }
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDesignReviewProcess: " + ex.ToString());

            }
            return false;
        }
        internal bool FunAddDesignReviewProcess(ProjectMgmtList lstDesign, int id_projectmgmt,ProjectMgmtModels objPro)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < lstDesign.lstPrj.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_projectdesign (id_projectmgmt,Dicipline,IntRevno,DrawingNumber,Upload,ReviewedBy,ApprovedBy,"
                        + "CustFeedback,RevNo,CustApprStatus";
                    string sFields = "", sFieldValue = "";

                    if (lstDesign.lstPrj[i].CustApprDate != null && lstDesign.lstPrj[i].CustApprDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", CustApprDate";
                        sFieldValue = sFieldValue + ", '" + lstDesign.lstPrj[i].CustApprDate.ToString("yyyy/MM/dd") + "'";
                    }

                    if (lstDesign.lstPrj[i].DgnSntDate != null && lstDesign.lstPrj[i].DgnSntDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", DgnSntDate";
                        sFieldValue = sFieldValue + ", '" + lstDesign.lstPrj[i].DgnSntDate.ToString("yyyy/MM/dd") + "'";
                    }

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + id_projectmgmt + "','" + lstDesign.lstPrj[i].Dicipline
                   + "','" + lstDesign.lstPrj[i].IntRevno + "','" + lstDesign.lstPrj[i].DrawingNumber + "','" + lstDesign.lstPrj[i].Upload + "','" + lstDesign.lstPrj[i].ReviewedBy + "','" + lstDesign.lstPrj[i].ApprovedBy + "'"
                    + ",'" + lstDesign.lstPrj[i].CustFeedback + "','" + lstDesign.lstPrj[i].RevNo + "','" + lstDesign.lstPrj[i].CustApprStatus + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                   SendDesignProcessReviewEmail(lstDesign,objPro);
                   return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDesignReviewProcess: " + ex.ToString());

            }
            return false;
        }
        internal bool FunUpdateProjectDetails(ProjectMgmtModels objPro)
        {

            try
            {
                string sStartDate = objPro.StartDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sPlannedEndDate = objPro.PlannedEndDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sActualEndDate = objPro.ActualEndDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlStmt = "update t_projectmgmt set ProjectNo='" + objPro.ProjectNo + "',ProjectName='" + objPro.ProjectName + "',Location='" + objPro.Location + "'"
                + ",Customer='" + objPro.Customer + "',ProjectManager='" + objPro.ProjectManager + "',Team='" + objPro.Team + "',ProjectStatus='" + objPro.ProjectStatus + "',Remarks='" + objPro.Remarks + "',"
                + "StartDate='" + sStartDate + "',PlannedEndDate='" + sPlannedEndDate + "',ActualEndDate='" + sActualEndDate + "'";
                if (objPro.ProjectDocs != null && objPro.ProjectDocs != "")
                {
                    sSqlStmt = sSqlStmt + ", ProjectDocs='" + objPro.ProjectDocs + "'";
                }

               
               
                sSqlStmt = sSqlStmt + " where id_projectmgmt=" + objPro.id_projectmgmt;
                return objGlobalData.ExecuteQuery(sSqlStmt);
               
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateProjectDetails: " + ex.ToString());
            }
            return false;
        }

       

        [AllowAnonymous]
        public bool DesignReviewPlanAdd(ProjectMgmtModels objPrjModels, FormCollection form)
        {
            try
            {
                ProjectMgmtList objProjectList = new ProjectMgmtList();
                objProjectList.lstPrj = new List<ProjectMgmtModels>();
                DateTime dateValues;
                objPrjModels.Dicipline = form["Dicipline"];
                objPrjModels.DrawingNumber = form["DrawingNumber"].ToUpper();
                objPrjModels.IntRevno = Convert.ToInt32(form["IntRevno"]);
                objPrjModels.ReviewedBy = form["ReviewedBy"];
                objPrjModels.ApprovedBy = form["ApprovedBy"];
              
                if (DateTime.TryParse(form["CustApprDate"], out dateValues) == true)
                {
                    objPrjModels.CustApprDate = dateValues;
                }
                if (DateTime.TryParse(form["DgnSntDate"], out dateValues) == true)
                {
                    objPrjModels.DgnSntDate = dateValues;
                }
                objPrjModels.CustFeedback = form["CustFeedback"];
                objPrjModels.RevNo = form["RevNo"];
                objPrjModels.CustApprStatus = form["CustApprStatus"];
                objProjectList.lstPrj.Add(objPrjModels);
                return objPrjModels.FunAddDesignReviewProcess(objProjectList, Convert.ToInt16(objPrjModels.id_projectmgmt), objPrjModels);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DesignReviewPlanAdd: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateDesignReviewProcess(ProjectMgmtModels objPrjModels)
        {
            try
            {
                if (objPrjModels.Upload != null && objPrjModels.Upload != "")
                {
                    objPrjModels.Upload = "," + objPrjModels.Upload;
                }

                string sSqlstmt = "update t_projectdesign set Dicipline ='" + objPrjModels.Dicipline + "', IntRevno='" + objPrjModels.IntRevno + "', "
                    + "DrawingNumber='" + objPrjModels.DrawingNumber + "', ReviewedBy='" + objPrjModels.ReviewedBy
                    + "', ApprovedBy='" + objPrjModels.ApprovedBy + "', CustFeedback='" + objPrjModels.CustFeedback + "', RevNo='" + objPrjModels.RevNo + "', CustApprStatus='" + objPrjModels.CustApprStatus + "'";

                if (objPrjModels.Upload != null && objPrjModels.Upload != "")
                {
                    sSqlstmt = sSqlstmt + ", Upload=CONCAT(Upload,'" + objPrjModels.Upload + "')";
                }
                if (objPrjModels.CustApprDate != null && objPrjModels.CustApprDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {

                    sSqlstmt = sSqlstmt + ", CustApprDate='" + objPrjModels.CustApprDate.ToString("yyyy/MM/dd") + "'";
                }

                if (objPrjModels.DgnSntDate != null && objPrjModels.DgnSntDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {

                    sSqlstmt = sSqlstmt + ", DgnSntDate='" + objPrjModels.DgnSntDate.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_projectdesign='" + objPrjModels.id_projectdesign + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDesignReviewProcess: " + ex.ToString());
            }
            return false;
        }

        internal bool FunProjectDesignReview(string sid_projectdesign,string sid_projectmgmt, int sStatus, string comments)
        {
            try
            {
             string sReviewedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

            string sSqlstmt = "update t_projectdesign set ReviewStatus='" + sStatus + "',ReviewedDate='" + sReviewedDate + "'"
            + ",ReviewerComment='" + comments + "' where id_projectdesign='" + sid_projectdesign + "'";

            if (objGlobalData.ExecuteQuery(sSqlstmt))
            {
                SendDesignProcessApproveEmail(sid_projectdesign, sid_projectmgmt);
                return true;
            }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunProjectDesignReview: " + ex.ToString());
            }
           
            return false;
        }
        internal bool FunProjectDesignApprove(string sid_projectdesign,string sid_projectmgmt, int sStatus, string comments)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_projectdesign set ApproveStatus='" + sStatus + "',ApprovedDate='" + sApprovedDate + "'"
                + ",ApproverComment='" + comments + "' where id_projectdesign='" + sid_projectdesign + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendDesignProcessEmail(sid_projectdesign, sid_projectmgmt);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunProjectDesignApprove: " + ex.ToString());
            }

            return false;
        }

        internal bool SendDesignProcessReviewEmail(ProjectMgmtList lstDesign, ProjectMgmtModels objPro)
        {
            try {

                string sHeader = "", sCCList = "";
             
                     sCCList = objGlobalData.GetHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().empid);
                
                if(lstDesign.lstPrj.Count!=0 && lstDesign.lstPrj.Count>0)
                {
                   for(int i=0;i<lstDesign.lstPrj.Count;i++)
                   {
                       string sUserName =objGlobalData.GetEmpHrNameById(lstDesign.lstPrj[i].ReviewedBy);
                       string sToEmailId = objGlobalData.GetHrEmpEmailIdById(lstDesign.lstPrj[i].ReviewedBy);
                       string sFileName = lstDesign.lstPrj[i].Upload;
                       sHeader = "<tr><td ><b>Project Name:<b></td> <td >"
                              + objPro.ProjectName + "</td></tr>"
                              + "<tr><td ><b>Project No:<b></td> <td >" + objPro.ProjectNo + "</td></tr>"
                              + "<tr><td ><b>Dicipline:<b></td> <td >" +objGlobalData.GetDropdownitemById(lstDesign.lstPrj[i].Dicipline) +"</td></tr>"
                              + "<tr><td ><b>Internal Revno:<b></td> <td >" + lstDesign.lstPrj[i].IntRevno + "</td></tr>"
                              + "<tr><td ><b>Drawing Number:<b></td> <td >" + lstDesign.lstPrj[i].DrawingNumber + "</td></tr>";

                       Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "DesignReview", sHeader, "", "Design Details for Review");
                       objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], sFileName, sCCList, "");
                   }
                }
                else
                {
                   return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendDesignProcessReviewEmail: " + ex.ToString());

            }
            return false;
        }

        internal bool SendDesignProcessApproveEmail(string sid_projectdesign,string sid_projectmgmt)
        {
            try
            {

                string sHeader = "";
              
                string sSqlStmt = "select t.id_projectmgmt,ProjectNo,ProjectName,Dicipline,Revno,DrawingNumber,Upload,ReviewedBy,"
                + "ApprovedBy,LoggedBy from t_projectmgmt t,t_projectdesign tt where t.id_projectmgmt=tt.id_projectmgmt and "
                + "t.id_projectmgmt='" + sid_projectmgmt + "' and id_projectdesign='" + sid_projectdesign + "'";

                DataSet dsProject = objGlobalData.Getdetails(sSqlStmt);

                if (dsProject.Tables.Count > 0 && dsProject.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsProject.Tables[0].Rows.Count; )
                    {
                        string sUserName = objGlobalData.GetEmpHrNameById(dsProject.Tables[0].Rows[i]["ApprovedBy"].ToString());
                        string sToEmailId = objGlobalData.GetHrEmpEmailIdById(dsProject.Tables[0].Rows[i]["ApprovedBy"].ToString());
                        string sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsProject.Tables[0].Rows[i]["ReviewedBy"].ToString());

                        sHeader = "<tr><td ><b>Project Name:<b></td> <td >"
                               + dsProject.Tables[0].Rows[i]["ProjectName"].ToString() + "</td></tr>"
                               + "<tr><td ><b>Project No:<b></td> <td >" + dsProject.Tables[0].Rows[i]["ProjectNo"].ToString() + "</td></tr>"
                               + "<tr><td ><b>Dicipline:<b></td> <td >" +objGlobalData.GetDropdownitemById(dsProject.Tables[0].Rows[i]["Dicipline"].ToString()) + "</td></tr>"
                               + "<tr><td ><b>Revno:<b></td> <td >" + dsProject.Tables[0].Rows[i]["Revno"].ToString() + "</td></tr>"
                               + "<tr><td ><b>Drawing Number:<b></td> <td >" + dsProject.Tables[0].Rows[i]["DrawingNumber"].ToString() + "</td></tr>"
                                 + "<tr><td ><b>Reviewed By:<b></td> <td >" +objGlobalData.GetEmpHrNameById(dsProject.Tables[0].Rows[i]["ReviewedBy"].ToString()) + "</td></tr>";

                        Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "DesignApprove", sHeader, "", "Design Details for approval");
                        objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                        return true;
                    }
                }
                            
                
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendDesignProcessReviewEmail: " + ex.ToString());

            }
            return false;
        }

        internal bool SendDesignProcessEmail(string sid_projectdesign, string sid_projectmgmt)
        {
            try
            {

                string sHeader = "";

                string sSqlStmt = "select t.id_projectmgmt,ProjectNo,ProjectName,Dicipline,Revno,DrawingNumber,Upload,ReviewedBy,"
                + "ApprovedBy,LoggedBy from t_projectmgmt t,t_projectdesign tt where t.id_projectmgmt=tt.id_projectmgmt and "
                + "t.id_projectmgmt='" + sid_projectmgmt + "' and id_projectdesign='" + sid_projectdesign + "'";

                DataSet dsProject = objGlobalData.Getdetails(sSqlStmt);

                if (dsProject.Tables.Count > 0 && dsProject.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsProject.Tables[0].Rows.Count;)
                    {
                        string sUserName = "All,";
                        string sAEmailID = objGlobalData.GetHrEmpEmailIdById(dsProject.Tables[0].Rows[i]["ApprovedBy"].ToString());
                        string sREmailID = objGlobalData.GetHrEmpEmailIdById(dsProject.Tables[0].Rows[i]["ReviewedBy"].ToString());
                        string sCCList = objGlobalData.GetHrEmpEmailIdById(dsProject.Tables[0].Rows[i]["LoggedBy"].ToString());
                        string sToEmailId = sAEmailID +","+ sREmailID;
                        sHeader = "<tr><td ><b>Project Name:<b></td> <td >"
                               + dsProject.Tables[0].Rows[i]["ProjectName"].ToString() + "</td></tr>"
                               + "<tr><td ><b>Project No:<b></td> <td >" + dsProject.Tables[0].Rows[i]["ProjectNo"].ToString() + "</td></tr>"
                               + "<tr><td ><b>Dicipline:<b></td> <td >" + objGlobalData.GetDropdownitemById(dsProject.Tables[0].Rows[i]["Dicipline"].ToString()) + "</td></tr>"
                               + "<tr><td ><b>Revno:<b></td> <td >" + dsProject.Tables[0].Rows[i]["Revno"].ToString() + "</td></tr>"
                               + "<tr><td ><b>Drawing Number:<b></td> <td >" + dsProject.Tables[0].Rows[i]["DrawingNumber"].ToString() + "</td></tr>"
                               + "<tr><td ><b>Reviewed By:<b></td> <td >" + objGlobalData.GetEmpHrNameById(dsProject.Tables[0].Rows[i]["ReviewedBy"].ToString()) + "</td></tr>"
                               + "<tr><td ><b>Approved By:<b></td> <td >" + objGlobalData.GetEmpHrNameById(dsProject.Tables[0].Rows[i]["ApprovedBy"].ToString()) + "</td></tr>";

                        Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "Design", sHeader, "", "Approved Design Details");
                        objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                        return true;
                    }
                }


            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendDesignProcessReviewEmail: " + ex.ToString());

            }
            return false;
        }


    }

    
    public class ProjectMgmtList {

        public List<ProjectMgmtModels> lstPrj { get; set; }
    }

    
}