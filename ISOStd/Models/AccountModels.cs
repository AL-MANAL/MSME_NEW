using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string UserName { get; set; }
    }

    public class LoginModel
    {
        private clsGlobal objGlobal = new clsGlobal();

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public bool PasswordExpiryAuthenticate(string sUsername, string sPwd)
        {
            try
            {
                DataSet dsEmp = objGlobal.LoginPaswdExpAuth(sUsername, clsGlobal.Encrypt(sPwd));
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobal.AddFunctionalLog("Exception in PasswordExpiryAuthenticate: " + ex.ToString());
            }
            return false;
        }

        public bool LoginAuthenticate(string sUsername, string sPwd)
        {
            try
            {
                //string enc = clsGlobal.Encrypt("Test@123");
                //string org = clsGlobal.Encrypt(enc);
                DataSet dsEmp = objGlobal.LoginAuth(sUsername, clsGlobal.Encrypt(sPwd));
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    objGlobal.CreateUserSession(dsEmp.Tables[0].Rows[0]["empid"].ToString(), dsEmp.Tables[0].Rows[0]["CompEmpId"].ToString(), Regex.Replace(dsEmp.Tables[0].Rows[0]["firstname"].ToString(), " +", " ").Trim(),
                        dsEmp.Tables[0].Rows[0]["role"].ToString(), dsEmp.Tables[0].Rows[0]["ProfilePic"].ToString(), dsEmp.Tables[0].Rows[0]["Work_Location"].ToString(), dsEmp.Tables[0].Rows[0]["division"].ToString(),
                        dsEmp.Tables[0].Rows[0]["DeptID"].ToString(), dsEmp.Tables[0].Rows[0]["Designation"].ToString(), dsEmp.Tables[0].Rows[0]["DeptInCharge"].ToString(), dsEmp.Tables[0].Rows[0]["Audit_Criteria"].ToString(), dsEmp.Tables[0].Rows[0]["Events"].ToString(),
                        dsEmp.Tables[0].Rows[0]["Documents"].ToString(), dsEmp.Tables[0].Rows[0]["InternalDoc"].ToString(), dsEmp.Tables[0].Rows[0]["ExternalDoc"].ToString(), dsEmp.Tables[0].Rows[0]["Record"].ToString(), dsEmp.Tables[0].Rows[0]["DocChangeReq"].ToString(), dsEmp.Tables[0].Rows[0]["DocTrack"].ToString(),
                        dsEmp.Tables[0].Rows[0]["ObjKPI"].ToString(), dsEmp.Tables[0].Rows[0]["Objectives"].ToString(), dsEmp.Tables[0].Rows[0]["Kpi"].ToString(), dsEmp.Tables[0].Rows[0]["RiskMgmt"].ToString(), dsEmp.Tables[0].Rows[0]["ChangeMgmt"].ToString(),
                        dsEmp.Tables[0].Rows[0]["ContextOrganise"].ToString(), dsEmp.Tables[0].Rows[0]["Parties"].ToString(), dsEmp.Tables[0].Rows[0]["Issues"].ToString(), dsEmp.Tables[0].Rows[0]["Risk"].ToString(), dsEmp.Tables[0].Rows[0]["HazardRiskReg"].ToString(), dsEmp.Tables[0].Rows[0]["HR"].ToString(),
                        dsEmp.Tables[0].Rows[0]["Emp"].ToString(), dsEmp.Tables[0].Rows[0]["EmpPerfEval"].ToString(), dsEmp.Tables[0].Rows[0]["Competancy"].ToString(), dsEmp.Tables[0].Rows[0]["OrgChart"].ToString(), dsEmp.Tables[0].Rows[0]["ManHour"].ToString(),
                        dsEmp.Tables[0].Rows[0]["ExitEmp"].ToString(), dsEmp.Tables[0].Rows[0]["Visitor"].ToString(), dsEmp.Tables[0].Rows[0]["LeaveMgmt"].ToString(), dsEmp.Tables[0].Rows[0]["LeaveCredit"].ToString(), dsEmp.Tables[0].Rows[0]["AdjustLeave"].ToString(),
                        dsEmp.Tables[0].Rows[0]["ApplyLeave"].ToString(), dsEmp.Tables[0].Rows[0]["LeaveMaster"].ToString(), dsEmp.Tables[0].Rows[0]["Holiday"].ToString(), dsEmp.Tables[0].Rows[0]["ATSS"].ToString(), dsEmp.Tables[0].Rows[0]["ATS"].ToString(),
                        dsEmp.Tables[0].Rows[0]["HseATS"].ToString(), dsEmp.Tables[0].Rows[0]["Meeting"].ToString(), dsEmp.Tables[0].Rows[0]["MAgenda"].ToString(), dsEmp.Tables[0].Rows[0]["MSchedule"].ToString(), dsEmp.Tables[0].Rows[0]["MUnplaned"].ToString(),
                        dsEmp.Tables[0].Rows[0]["Suppliers"].ToString(), dsEmp.Tables[0].Rows[0]["Supplier"].ToString(), dsEmp.Tables[0].Rows[0]["DLog"].ToString(), dsEmp.Tables[0].Rows[0]["SupplierPer"].ToString(), dsEmp.Tables[0].Rows[0]["Providers"].ToString(), dsEmp.Tables[0].Rows[0]["SupplierRate"].ToString(),
                        dsEmp.Tables[0].Rows[0]["CustMgmt"].ToString(), dsEmp.Tables[0].Rows[0]["Visits"].ToString(), dsEmp.Tables[0].Rows[0]["AddCust"].ToString(), dsEmp.Tables[0].Rows[0]["Complaints"].ToString(), dsEmp.Tables[0].Rows[0]["SurveyQues"].ToString(), dsEmp.Tables[0].Rows[0]["UploadSurvey"].ToString(), dsEmp.Tables[0].Rows[0]["CustReturnProcuct"].ToString(),
                        dsEmp.Tables[0].Rows[0]["Bidding"].ToString(), dsEmp.Tables[0].Rows[0]["TrainingOri"].ToString(), dsEmp.Tables[0].Rows[0]["AddTopic"].ToString(), dsEmp.Tables[0].Rows[0]["Perftraining"].ToString(), dsEmp.Tables[0].Rows[0]["EmpTrainingOri"].ToString(), dsEmp.Tables[0].Rows[0]["Audit"].ToString(),
                        dsEmp.Tables[0].Rows[0]["InterAudit"].ToString(), dsEmp.Tables[0].Rows[0]["ExterAudit"].ToString(), dsEmp.Tables[0].Rows[0]["ExtAuditRpt"].ToString(), dsEmp.Tables[0].Rows[0]["CustAudit"].ToString(), dsEmp.Tables[0].Rows[0]["RaiseNc"].ToString(), dsEmp.Tables[0].Rows[0]["InterChecklist"].ToString(),
                        dsEmp.Tables[0].Rows[0]["AuditChecklist"].ToString(), dsEmp.Tables[0].Rows[0]["HSE"].ToString(), dsEmp.Tables[0].Rows[0]["IncidentRpt"].ToString(), dsEmp.Tables[0].Rows[0]["NearMissRept"].ToString(), dsEmp.Tables[0].Rows[0]["EmergPlan"].ToString(), dsEmp.Tables[0].Rows[0]["ToolTalk"].ToString(),
                        dsEmp.Tables[0].Rows[0]["SafetyLog"].ToString(), dsEmp.Tables[0].Rows[0]["PpeLog"].ToString(), dsEmp.Tables[0].Rows[0]["HseInsp"].ToString(), dsEmp.Tables[0].Rows[0]["AirNoise"].ToString(), dsEmp.Tables[0].Rows[0]["Waste"].ToString(), dsEmp.Tables[0].Rows[0]["ObservCard"].ToString(),
                        dsEmp.Tables[0].Rows[0]["HseIndu"].ToString(), dsEmp.Tables[0].Rows[0]["FirstBox"].ToString(), dsEmp.Tables[0].Rows[0]["FireInspection"].ToString(), dsEmp.Tables[0].Rows[0]["Project"].ToString(), dsEmp.Tables[0].Rows[0]["Equip"].ToString(), dsEmp.Tables[0].Rows[0]["Maintenance"].ToString(),
                        dsEmp.Tables[0].Rows[0]["Calibration"].ToString(), dsEmp.Tables[0].Rows[0]["LegalReg"].ToString(), dsEmp.Tables[0].Rows[0]["Legal"].ToString(), dsEmp.Tables[0].Rows[0]["Certificates"].ToString(), dsEmp.Tables[0].Rows[0]["Training"].ToString(), dsEmp.Tables[0].Rows[0]["TrainingList"].ToString(),
                        dsEmp.Tables[0].Rows[0]["TrainingCal"].ToString(), dsEmp.Tables[0].Rows[0]["Report"].ToString(), dsEmp.Tables[0].Rows[0]["Rept"].ToString(), dsEmp.Tables[0].Rows[0]["DashRept"].ToString(), dsEmp.Tables[0].Rows[0]["MISRept"].ToString(), dsEmp.Tables[0].Rows[0]["Permits"].ToString(),
                        dsEmp.Tables[0].Rows[0]["AccessPermit"].ToString(), dsEmp.Tables[0].Rows[0]["WorkPermit"].ToString(), dsEmp.Tables[0].Rows[0]["Settings"].ToString(), dsEmp.Tables[0].Rows[0]["Company"].ToString(), dsEmp.Tables[0].Rows[0]["Dept"].ToString(), dsEmp.Tables[0].Rows[0]["User"].ToString(),
                        dsEmp.Tables[0].Rows[0]["DropDown"].ToString(), dsEmp.Tables[0].Rows[0]["EmpRole"].ToString(), dsEmp.Tables[0].Rows[0]["ISOStd"].ToString(), dsEmp.Tables[0].Rows[0]["BranchTree"].ToString(), dsEmp.Tables[0].Rows[0]["TRA"].ToString(), dsEmp.Tables[0].Rows[0]["ResConsump"].ToString(),
                        dsEmp.Tables[0].Rows[0]["NC"].ToString(), dsEmp.Tables[0].Rows[0]["Portal"].ToString(), dsEmp.Tables[0].Rows[0]["sub_cr"].ToString(), dsEmp.Tables[0].Rows[0]["access_portal"].ToString(), dsEmp.Tables[0].Rows[0]["portal_userlog"].ToString(), dsEmp.Tables[0].Rows[0]["LocationTree"].ToString()

                       );
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobal.AddFunctionalLog("Exception in LoginAuthenticate: " + ex.ToString());
            }
            return false;
        }

        //public bool MailTempPassword(string sEmailid)
        //{
        //    string sTempPassword = objGlobal.GenerateTempPassword();
        //    sTempPassword = clsGlobal.Encrypt(sTempPassword);

        //    EmployeeModels objEmp = new EmployeeModels();
        //    objEmp.MailTempPassword(sEmailid, sTempPassword);
        //    objGlobal.Sendmail(sEmailid, "Password Reset", "Please use Temproray Password: " + sTempPassword, "");

        //    return true;
        //}
    }
}