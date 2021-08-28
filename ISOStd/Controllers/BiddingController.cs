using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.IO;
using System.Data;
using PagedList;
using PagedList.Mvc;
using System.Globalization;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class BiddingController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        BiddingModels objBidding = new BiddingModels();


        public BiddingController()
        {
            ViewBag.Menutype = "Customer";
            ViewBag.SubMenutype = "Bidding";
        }
        [AllowAnonymous]
        public ActionResult AddBiddingDocument()
        {
            try
            {
               
                ViewBag.Employee = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Reviewer = objGlobaldata.GetReviewer();
                ViewBag.GetPreparer = objGlobaldata.GetPreparer();
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddBiddingDocument: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBiddingDocument(BiddingModels objBidModels, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                int Notificationval = 1;
                objBidModels.checkedby = form["checkedby"];
                objBidModels.NotificationPeriod = form["NotificationPeriod"];
                objBidModels.NotificationValue = form["NotificationValue"];
                if (objBidModels.NotificationPeriod == "Days")
                {

                    Notificationval = Convert.ToInt32(form["NotificationValue"]);
                }
                if (objBidModels.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objBidModels.NotificationValue = "0";
                }
                else if (objBidModels.NotificationPeriod == "Weeks" && int.TryParse(objBidModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objBidModels.NotificationPeriod == "Months" && int.TryParse(objBidModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objBidModels.NotificationDays = Notificationval;

                if (upload != null && files.ContentLength > 0)
                {
                    objBidModels.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                            string sFilename = "Bidding" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objBidModels.upload = objBidModels.upload + "," + "~/DataUpload/MgmtDocs/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddBiddingDocument-uploaddocument: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objBidModels.upload = objBidModels.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                DateTime dateValue;

                if (DateTime.TryParse(form["submission_date"], out dateValue) == true)
                {
                    objBidModels.submission_date = dateValue;
                }
               
                if (DateTime.TryParse(form["proposal_date"], out dateValue) == true)
                {
                    objBidModels.proposal_date = dateValue;
                }


               
                if (objBidModels.FunAddBiddingDetails(objBidModels))
                {
                    TempData["Successdata"] = "Added Bidding details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddBiddingDocument: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("BiddingList");
        }

         
        public ActionResult BiddingList(int? page)
        {

            BiddingModelsList objBiddingList = new BiddingModelsList();
            objBiddingList.BiddingList = new List<BiddingModels>();
            try
            {
                string sSqlstmt = "select id_bidding,client,folderref,projectname,submission_date,preparedby,checkedby,proposalref,"
                + "proposal_date,(case when proposal_status='1' then 'Approved' else 'Not Approved' end) as proposal_status from t_bidding where active=1 order by id_bidding asc";

                DataSet dsBiddingList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsBiddingList.Tables.Count > 0)
                {
                  
                       
                    
                    for (int i = 0; i < dsBiddingList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            BiddingModels objBidding = new BiddingModels
                            {
                                id_bidding = Convert.ToInt32(dsBiddingList.Tables[0].Rows[i]["id_bidding"].ToString()),
                                client = dsBiddingList.Tables[0].Rows[i]["client"].ToString(),
                                folderref = dsBiddingList.Tables[0].Rows[i]["folderref"].ToString(),
                                projectname = dsBiddingList.Tables[0].Rows[i]["projectname"].ToString(),
                                preparedby =objGlobaldata.GetEmpHrNameById(dsBiddingList.Tables[0].Rows[i]["preparedby"].ToString()),
                                checkedby = objGlobaldata.GetMultiHrEmpNameById(dsBiddingList.Tables[0].Rows[i]["checkedby"].ToString()),
                                proposalref = dsBiddingList.Tables[0].Rows[i]["proposalref"].ToString(),
                                proposal_status = dsBiddingList.Tables[0].Rows[i]["proposal_status"].ToString(),
                              

                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsBiddingList.Tables[0].Rows[i]["submission_date"].ToString(), out dtValue))
                            {
                                objBidding.submission_date = dtValue;
                            }
                            if (DateTime.TryParse(dsBiddingList.Tables[0].Rows[i]["proposal_date"].ToString(), out dtValue))
                            {
                                objBidding.proposal_date = dtValue;
                            }
                            objBiddingList.BiddingList.Add(objBidding);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in BiddingList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in BiddingList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objBiddingList.BiddingList.ToList().ToPagedList(page ?? 1, 40));
        }

        
        [AllowAnonymous]
        public ActionResult BiddingEdit()
        {
            ViewBag.Employee = objGlobaldata.GetHrEmployeeListbox();
            ViewBag.Reviewer = objGlobaldata.GetReviewer();
            ViewBag.GetPreparer = objGlobaldata.GetPreparer();
            ViewBag.SubMenutype = "Bidding";
            BiddingModels objBiddingModel = new BiddingModels();
            try
            {

                if (Request.QueryString["id_bidding"] != null && Request.QueryString["id_bidding"] != "")
                {
                    string sid_bidding = Request.QueryString["id_bidding"];

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select id_bidding,client,folderref,projectname,submission_date,preparedby,"
                    + "checkedby,proposalref,proposal_date,upload from t_bidding where id_bidding='" + sid_bidding + "'";

                    DataSet dsBiddingList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsBiddingList.Tables.Count > 0 && dsBiddingList.Tables[0].Rows.Count > 0)
                    {
                        objBiddingModel = new BiddingModels
                        {
                            id_bidding = Convert.ToInt32(dsBiddingList.Tables[0].Rows[0]["id_bidding"].ToString()),
                            client = dsBiddingList.Tables[0].Rows[0]["client"].ToString(),
                            folderref = dsBiddingList.Tables[0].Rows[0]["folderref"].ToString(),
                            projectname = dsBiddingList.Tables[0].Rows[0]["projectname"].ToString(),
                            preparedby = objGlobaldata.GetEmpHrNameById(dsBiddingList.Tables[0].Rows[0]["preparedby"].ToString()),
                            checkedby = objGlobaldata.GetMultiHrEmpNameById(dsBiddingList.Tables[0].Rows[0]["checkedby"].ToString()),
                            proposalref = dsBiddingList.Tables[0].Rows[0]["proposalref"].ToString(),
                            upload = dsBiddingList.Tables[0].Rows[0]["upload"].ToString(),
                        };

                       
                        DateTime dtValue;
                        if (DateTime.TryParse(dsBiddingList.Tables[0].Rows[0]["submission_date"].ToString(), out dtValue))
                        {
                            objBiddingModel.submission_date = dtValue;
                            objBiddingModel.sub_date = objBiddingModel.submission_date.ToString("yyyy-MM-dd");
                        }
                       
                        if (DateTime.TryParse(dsBiddingList.Tables[0].Rows[0]["proposal_date"].ToString(), out dtValue))
                        {
                            objBiddingModel.proposal_date = dtValue;
                            objBiddingModel.prop_date = objBiddingModel.proposal_date.ToString("yyyy-MM-dd");
                        }

                       
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("BiddingList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("BiddingList");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in BiddingEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("BiddingList");
            }
            return View(objBiddingModel);

        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BiddingEdit(BiddingModels objBidding, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            ViewBag.SubMenutype = "Binding";

            try
            {
                HttpPostedFileBase files = Request.Files[0];
                objBidding.checkedby = form["checkedby"];
                string QCDelete = Request.Form["QCDocsValselectall"];
                if (upload != null && files.ContentLength > 0)
                {
                    objBidding.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                            string sFilename = "Bidding" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objBidding.upload = objBidding.upload + "," + "~/DataUpload/MgmtDocs/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in BiddingEdit-upload: " + ex.ToString());
                           
                        }
                    }
                    objBidding.upload = objBidding.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objBidding.upload = objBidding.upload + "," + form["QCDocsVal"];
                    objBidding.upload = objBidding.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objBidding.upload = null;
                }
                else if (form["QCDocsVal"] == null  && files.ContentLength == 0)
                {
                    objBidding.upload = null;
                }
                DateTime dateValue;

                if (DateTime.TryParse(form["submission_date"], out dateValue) == true)
                {
                    objBidding.submission_date = dateValue;
                }

                if (DateTime.TryParse(form["proposal_date"], out dateValue) == true)
                {
                    objBidding.proposal_date = dateValue;
                }

                if (objBidding.FunUpdateBiddingDetails(objBidding))
                {
                    TempData["Successdata"] = "Details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in BiddingEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("BiddingList");
        }

        
        [AllowAnonymous]
        public ActionResult BiddingApprove(string id_bidding, int iStatus, string biddingcomments)
        {
            try
            {
                BiddingModels objBiddingModels = new BiddingModels();

                string sStatus = "";
                if (iStatus == 1)
                {
                    sStatus = "Approved";

                }

                if (objBiddingModels.FunBiddingDocApprove(id_bidding, iStatus, biddingcomments))
                {
                    TempData["Successdata"] = "Document" + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in BiddingApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ListPendingForApproval", "Dashboard");
        }

        
        [HttpPost]
        public JsonResult BiddingDelete(FormCollection form)
        {
            string sid_bidding = form["id_bidding"];
            if (sid_bidding != "")
            {
                try
                {
                    BiddingModels objBiddingModels = new BiddingModels();

                    if (objBiddingModels.FunDeleteDoc(sid_bidding))
                    {
                        TempData["Successdata"] = "Deleted successfully";
                        return Json("Deleted successfully");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }

                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in BiddingDelete: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            return Json("Delete Failed, Customer Id null");
        }

         
        [AllowAnonymous]
        public ActionResult BiddingDetails()
        {
            BiddingModels objBiddingModel = new BiddingModels();

            try
            {
                if (Request.QueryString["id_bidding"] != null && Request.QueryString["id_bidding"] != "")
                {
                    string sid_bidding = Request.QueryString["id_bidding"];

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select id_bidding,client,folderref,projectname,submission_date,preparedby,"
                    + "checkedby,proposalref,proposal_date,(case when proposal_status=1 then 'Approved' else 'Not approved' end) as proposal_status,"
                    + "approved_date,upload,Approvers from t_bidding where id_bidding='" + sid_bidding + "'";

                    DataSet dsBiddingList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsBiddingList.Tables.Count > 0 && dsBiddingList.Tables[0].Rows.Count > 0)
                    {
                        objBiddingModel = new BiddingModels
                        {
                            id_bidding = Convert.ToInt32(dsBiddingList.Tables[0].Rows[0]["id_bidding"].ToString()),
                            client = dsBiddingList.Tables[0].Rows[0]["client"].ToString(),
                            folderref = dsBiddingList.Tables[0].Rows[0]["folderref"].ToString(),
                            projectname = dsBiddingList.Tables[0].Rows[0]["projectname"].ToString(),
                            preparedby = objGlobaldata.GetEmpHrNameById(dsBiddingList.Tables[0].Rows[0]["preparedby"].ToString()),
                            checkedby = objGlobaldata.GetMultiHrEmpNameById(dsBiddingList.Tables[0].Rows[0]["checkedby"].ToString()),
                            proposalref = dsBiddingList.Tables[0].Rows[0]["proposalref"].ToString(),
                            upload = dsBiddingList.Tables[0].Rows[0]["upload"].ToString(),
                            proposal_status = dsBiddingList.Tables[0].Rows[0]["proposal_status"].ToString(),
                            Approvers = objGlobaldata.GetMultiHrEmpNameById(dsBiddingList.Tables[0].Rows[0]["Approvers"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsBiddingList.Tables[0].Rows[0]["submission_date"].ToString(), out dtValue))
                        {
                            objBiddingModel.submission_date = dtValue;
                        }
                        if (DateTime.TryParse(dsBiddingList.Tables[0].Rows[0]["proposal_date"].ToString(), out dtValue))
                        {
                            objBiddingModel.proposal_date = dtValue;
                        }
                        if (DateTime.TryParse(dsBiddingList.Tables[0].Rows[0]["approved_date"].ToString(), out dtValue))
                        {
                            objBiddingModel.approved_date = dtValue;
                        }

                        DataSet dsBidding = objGlobaldata.GetBiddingCommentdetails(sid_bidding);
                        ViewBag.CommentDetails = dsBidding;
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("BiddingList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("BiddingList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in BiddingDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objBiddingModel);

        }
        
    }
}