using ISOStd.Models;
using System;
using System.Data;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    public class DocumentsController : Controller
    {
        public DocumentsController()
        {
            ViewBag.Menutype = "DocumentsStats";
        }

        private clsGlobal objGlobaldata = new clsGlobal();

        // GET: Documents
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Documents()
        {
            try
            {
                //internal documents start
                string sSqlstmt = "Select  * from t_mgmt_documents where `status` = 1;";
                DataSet dsInternalTotal = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsInternalTotal = dsInternalTotal;

                sSqlstmt = "select * from t_mgmt_documents where Reviewed_Status=0 and Approved_Status = 0;";
                DataSet dsInternalReview = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsInternalReview = dsInternalReview;

                sSqlstmt = "select * from t_mgmt_documents where Approved_Status=1;";
                DataSet dsInternalApproved = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsInternalApproved = dsInternalApproved;
                //internal documents ends

                //External Documents starts
                sSqlstmt = "select * from t_mgmt_doc_external where active=1;";
                DataSet dsExternalTotal = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsExternalTotal = dsExternalTotal;

                sSqlstmt = "select * from t_mgmt_doc_external where datediff(now(),docdate)> 300 and active = 1";
                DataSet dsExternalApproved = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsExternalApproved = dsExternalApproved;

                sSqlstmt = "select * from t_mgmt_doc_external where datediff(now(),docdate)> 300 and active = 0;";
                DataSet dsExternalDeclined = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsExternalDeclined = dsExternalDeclined;
                //External Documents ends

                //Doc Change Requests starts
                sSqlstmt = "select * from t_documentchangerequest where active=1";
                DataSet dsDocChangeTotal = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsDocChangeTotal = dsDocChangeTotal;

                sSqlstmt = "select * from t_documentchangerequest where active=1 and  ApproveStatus=1";
                DataSet dsDocChangeApproved = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsDocChangeApproved = dsDocChangeApproved;

                sSqlstmt = "select * from t_documentchangerequest where Rejector is not null or active=0;";
                DataSet dsDocChangeDeclined = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsDocChangeDeclined = dsDocChangeDeclined;
                //Doc Change Ends

                //Documents Tracked starts
                sSqlstmt = "select * from t_document_tracking where active=1;";
                DataSet dsDocTrackTotal = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsDocTrackTotal = dsDocTrackTotal;

                sSqlstmt = "select * from t_document_tracking where datediff(now(),exp_date) > 30 and active = 1; ";
                DataSet dsDocTrackApproved = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsDocTrackApproved = dsDocTrackApproved;

                sSqlstmt = "select * from t_document_tracking where datediff(now(),exp_date) > 30 and active = 0;";
                DataSet dsDocTrackDeclined = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsDocTrackDeclined = dsDocTrackDeclined;
                //Documents Tracked ends

                //Passports Starts
                sSqlstmt = "select * from t_hr_employee where emp_status=1;";
                DataSet dsPassportTotal = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsPassportTotal = dsPassportTotal;

                sSqlstmt = "select * from t_hr_employee where datediff(now(),Passport_expiry) >90 and emp_status = 1; ";
                DataSet dsPassportApproved = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsPassportApproved = dsPassportApproved;

                sSqlstmt = "Select * from t_hr_employee where datediff(now(),Passport_expiry) >90 and emp_status = 0; ";
                DataSet dsPassportDeclined = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsPassportDeclined = dsPassportDeclined;
                //Passport ends

                //Visa Documents
                sSqlstmt = "select * from t_hr_employee where emp_status=1;";
                DataSet dsVisaTotal = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsVisaTotal = dsVisaTotal;

                sSqlstmt = "select * from t_hr_employee where emp_status=1 and datediff(now(), visa_Exp_date) > 60";
                DataSet dsVisaApproved = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsVisaApproved = dsVisaApproved;

                sSqlstmt = "select * from t_hr_employee where emp_status=0 and datediff(now(), visa_Exp_date) > 60";
                DataSet dsVisaDeclined = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsVisaDeclined = dsVisaDeclined;
                //Visa Documents Ends

                //Employee Insurance
                sSqlstmt = "select * from t_hr_employee where emp_status=1;";
                DataSet dsInsuranceTotal = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsInsuranceTotal = dsInsuranceTotal;

                sSqlstmt = "select * from t_hr_employee where datediff(now(),Health_Insurance_Expiry) >60 and emp_status = 1; ";
                DataSet dsInsuranceApproved = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsInsuranceApproved = dsInsuranceApproved;

                sSqlstmt = "select * from t_hr_employee where datediff(now(),Health_Insurance_Expiry) >60 and emp_status = 0; ";
                DataSet dsInsurcaneDeclined = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsInsurcaneDeclined = dsInsurcaneDeclined;
                //Employee Insurance ends

                //Peformance Evaluation
                sSqlstmt = "select * from t_emp_performance_eval where active=1;";
                DataSet dsPerformanceTotal = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsPerformanceTotal = dsPerformanceTotal;

                sSqlstmt = "select * from t_emp_performance_eval where datediff(now(), Evaluation_DoneOn) > 30 and active = 1; ";
                DataSet dsPerformanceApproved = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsPerformanceApproved = dsPerformanceApproved;

                sSqlstmt = "select * from t_emp_performance_eval where datediff(now(), Evaluation_DoneOn) > 30 and active = 0; ";
                DataSet dsPerformanceDeclined = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsPerformanceDeclined = dsPerformanceDeclined;
                //Peformance Evaluation Ends

                //MOC
                sSqlstmt = "select * from t_changemanagement where  active=1;";
                DataSet dsMOCTotal = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsMOCTotal = dsMOCTotal;

                sSqlstmt = "select * from t_changemanagement where approvestatus=0 and active=1; ";
                DataSet dsMOCApproved = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsMOCApproved = dsMOCApproved;

                sSqlstmt = "select * from t_changemanagement where approvestatus=2 and active=1; ";
                DataSet dsMOCDeclined = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsMOCDeclined = dsMOCDeclined;
                //MOC Ends

                //Access Request - Goverment
                sSqlstmt = "select * from t_portal_authorization where  active=1;";
                DataSet dsAccessGovtTotal = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsAccessGovtTotal = dsAccessGovtTotal;

                sSqlstmt = "select * from t_portal_authorization where apporved_status=0 and active=1; ";
                DataSet dsAccessGovtCreate = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsAccessGovtCreate = dsAccessGovtCreate;

                sSqlstmt = "select * from t_portal_authorization where apporved_status=1 and active=1; ";
                DataSet dsAccessGovtRecomd = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsAccessGovtRecomd = dsAccessGovtRecomd;

                sSqlstmt = "select * from t_portal_authorization where apporved_status=2 and active=1; ";
                DataSet dsAccessGovtCEO = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsAccessGovtCEO = dsAccessGovtCEO;

                sSqlstmt = "select * from t_portal_authorization where apporved_status=3 and active=1; ";
                DataSet dsAccessGovtVP = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsAccessGovtVP = dsAccessGovtVP;

                sSqlstmt = "select * from t_portal_authorization where apporved_status=4 and active=1; ";
                DataSet dsAccessGovtDecline = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsAccessGovtDecline = dsAccessGovtDecline;

                sSqlstmt = "select * from t_portal_authorization where apporved_status=5 and active=1; ";
                DataSet dsAccessGovtApproved = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.dsAccessGovtApproved = dsAccessGovtApproved;
                //Access Request - Goverment Ends
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Docstats: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
    }
}