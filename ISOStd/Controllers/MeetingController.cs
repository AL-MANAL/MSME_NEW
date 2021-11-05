using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.Net;
using System.IO;
using PagedList;
using PagedList.Mvc;
using Rotativa;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class MeetingController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public MeetingController()
        {
            ViewBag.Menutype = "Meeting";
        }

        //
        // GET: /Meeting/
        
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Meeting/AddMeetingType
        
        [AllowAnonymous]
        public ActionResult AddMeetingType(string sMeetingId)
        {
            ViewBag.SubMenutype = "MeetingType";
            MeetingTypeModels objMeetingType = new MeetingTypeModels();
            ViewBag.dsMeeting = objMeetingType.GetmeetingtypeListbox();

            if (sMeetingId != null && sMeetingId != "")
            {
                MeetingAgendaModels objAgenda = new MeetingAgendaModels();
                string sql = "Select Agenda_Id,Agenda_Desc,Agenda_Details from t_meeting_agenda where MeetingType_Id='" + sMeetingId + "'";
                //ViewBag.dsAgenda = objAgenda.GetMeetingAgendaListbox(sMeetingId);
                ViewBag.dsAgenda = objGlobaldata.Getdetails(sql);
                ViewBag.MeetungTypeId = sMeetingId;
            }
            return View();
        }

         
        public ActionResult InitializeAddMeetingType(string sMeetingId="")
        {
            MeetingTypeModels objMeetingType = new MeetingTypeModels();
            ViewBag.dsMeeting = objMeetingType.GetmeetingtypeListbox();

            if (sMeetingId != null && sMeetingId != "")
            {
                MeetingAgendaModels objAgenda = new MeetingAgendaModels();
                string sql = "Select Agenda_Id,Agenda_Desc,Agenda_Details from t_meeting_agenda where MeetingType_Id='" + sMeetingId + "'";
                //ViewBag.dsAgenda = objAgenda.GetMeetingAgendaListbox(sMeetingId);
                ViewBag.dsAgenda = objGlobaldata.Getdetails(sql);
                ViewBag.MeetungTypeId = sMeetingId;
            }

            return View();
        }

         
        [HttpPost]
        public JsonResult doesMeetingTypeExist(string MeetingName)
        {
            MeetingTypeModels objMeetingType = new MeetingTypeModels();

            var exists = objMeetingType.checkMeetingTypeExists(MeetingName);

            return Json(exists);
        }

        //
        // POST: /Meeting/AddMeetingType
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMeetingType(MeetingTypeModels objMeetingTypeModels, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "MeetingType";

                if (Request["button"].Equals("Save"))
                {
                    objMeetingTypeModels.MeetingName = form["MeetingName"];
                        if (objMeetingTypeModels.FunAddMeetingType(objMeetingTypeModels))
                        {
                            TempData["Successdata"] = "Added Meeting type details successfully";
                        }
                        else
                        {
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }

                    return InitializeAddMeetingType();

                }
                else if (Request["button"].Equals("Save Agenda"))
                {
                    MeetingAgendaModels objMeetingAgenda = new MeetingAgendaModels();
                    objMeetingAgenda.MeetingType = form["MeetingTypeId"];
                    objMeetingAgenda.Agenda_Desc = form["Agenda_Desc"];
                    objMeetingAgenda.Agenda_Details = form["Agenda_Details"];

                    if (AddAgendaDetails(objMeetingAgenda))
                    {
                        TempData["Successdata"] = "Added Agenda details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                    return InitializeAddMeetingType(objMeetingAgenda.MeetingType);
                  
                }
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddMeetingType: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return InitializeAddMeetingType();
        }


        //
        // GET: /Meeting/MeetingTypeDelete
        [AllowAnonymous]
        public ActionResult MeetingTypeDelete(string Id)
        {
            
            ViewBag.SubMenutype = "MeetingType";
            MeetingTypeModels objMeetingModels = new MeetingTypeModels();
            try
            {
                if (objMeetingModels.FunDeleteMeetingType(Id))
                {
                    TempData["Successdata"] = "Deleted Meeting type details successfully";
                   
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                  
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingTypeDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        public bool AddAgendaDetails(MeetingAgendaModels objMeetingAgenda)
        {
            ViewBag.SubMenutype = "Meeting";

            return objMeetingAgenda.FunAddMeetingAgenda(objMeetingAgenda);

            //try
            //{
            //    if (objMeetingAgenda.FunAddMeetingAgenda(objMeetingAgenda))
            //    {
            //        TempData["Successdata"] = "Added Meeting Agenda details successfully";
            //        //return Json("Success");
            //    }
            //    else
            //    {
            //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            //        //return Json("Failed");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    objGlobaldata.AddFunctionalLog("Exception in AddMeetingAgenda: " + ex.ToString());
            //    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            //}

           // return Json("Success");
        }
               
        //
        // GET: /Meeting/AddMeetingAgenda
         
        [AllowAnonymous]
        public ActionResult AddMeetingAgenda()
        {
            ViewBag.SubMenutype = "Meeting";
            return InitializeAddMeetingAgenda();
        }

         
        public ActionResult InitializeAddMeetingAgenda()
        {
            //MeetingAgendaModels objAgenda = new MeetingAgendaModels();
            //ViewBag.dsAgenda = objAgenda.GetMeetingAgendaListbox("");

            MeetingTypeModels objMeetingType = new MeetingTypeModels();
            ViewBag.MeetingType = objMeetingType.GetmeetingtypeListbox();
            return View();
        }

        //
        // POST: /Company/AddMeetingAgenda
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMeetingAgenda(MeetingAgendaModels objMeetingAgenda,  FormCollection form)
        {
            ViewBag.SubMenutype = "Meeting";

            objMeetingAgenda.MeetingType = form["MeetingType"];            

            try
            {
                if (objMeetingAgenda.FunAddMeetingAgenda(objMeetingAgenda))
                {
                    TempData["Successdata"] = "Added Meeting Agenda details successfully";
                    //return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    //return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddMeetingAgenda: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return InitializeAddMeetingAgenda();
        }     
                     
        //
        // GET: /Meeting/AddMeeting
         
        [AllowAnonymous]
        public ActionResult AddMeeting()
        {
            return InitilizeAddMeeting();
        }

        // GET: /Meeting/InitilizeAddNCRLog
         
        private ActionResult InitilizeAddMeeting()
        {
            try
            {
                MeetingTypeModels objMeetingType = new MeetingTypeModels();
                ViewBag.MeetingType = objMeetingType.GetmeetingtypeListbox();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.item_status = objGlobaldata.GetDropdownList("Meeting Item Status");
                //ViewBag.item_status = objGlobaldata.GetConstantValue("AuditStatus");
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriodMeeting");
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddMeeting: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMeeting(MeetingModels objMeeting, FormCollection form)
        {
            ViewBag.SubMenutype = "MOM";

            try
            {
                objMeeting.meeting_type_desc = form["meeting_type_desc"];
                objMeeting.last_meeting_id = form["last_meeting_id"];
                objMeeting.NotificationPeriod = form["NotificationPeriod"];
                objMeeting.NotificationValue = form["NotificationValue"];
                objMeeting.ext_attendees = form["ext_attendees"];
                objMeeting.ext_email = form["ext_email"];

                decimal Notificationval = 1;

                if (objMeeting.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objMeeting.NotificationValue = "0";
                }
                else if (objMeeting.NotificationPeriod == "Weeks" && decimal.TryParse(objMeeting.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objMeeting.NotificationPeriod == "Months" && decimal.TryParse(objMeeting.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                else if (objMeeting.NotificationPeriod == "Minutes" && decimal.TryParse(objMeeting.NotificationValue, out Notificationval))
                {
                    Notificationval = Notificationval/1440 ;
                }
                objMeeting.NotificationDays = Convert.ToDecimal(Notificationval);

                //objMeeting.meeting_ref = form["meeting_ref"];
               
                    objMeeting.MeetingCreatedBy = objGlobaldata.GetCurrentUserSession().empid;
                
                objMeeting.AttendeesUser = form["AttendeesUser"];
                DateTime dateValue;

                if (form["meeting_date"] != null && DateTime.TryParse(form["meeting_date"], out dateValue) == true)
                {
                    objMeeting.meeting_date = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objMeeting.meeting_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }

                
                List<string> lstAgendaId = new List<string>();
                if (form["Agendas"] != null && form["Agendas"] != "")
                {
                    //lstAgendaId = new List<string>(form["Agendas"].ToString().Split(','));
                    objMeeting.agenda_id = form["Agendas"].ToString();
                }
                MeetingModelsList objModelsList = new MeetingModelsList();
                objModelsList.MeetingMList = new List<MeetingModels>();
                //external attendees
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    MeetingModels objModels = new MeetingModels();

                    if (form["company_name" + i] != null && form["company_name" + i] != "")
                    {

                        objModels.company_name = form["company_name" + i];
                        objModels.attendee_name = form["attendee_name" + i];
                        objModels.designation = form["designation" + i];
                        objModels.email_id = form["email_id" + i];
                        objModelsList.MeetingMList.Add(objModels);
                    }
                }

                if (objMeeting.FunAddMeetingSchedule(objMeeting, objModelsList))
                {
                    TempData["Successdata"] = "Added Meeting details successfully  with Reference Number '" + objMeeting.meeting_ref + "'"; 
                    //Send MOM email
                    MeetingItemModels MeetingItem = new MeetingItemModels();
                    MeetingItem.SendMOMEmail(objMeeting.meeting_ref, "Meeting Request");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddMeeting: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MeetingList");
        }


        //
        // GET: /Meeting/AddMeeting
         
        [AllowAnonymous]
        public ActionResult AddMeetingNow()
        {
            return InitilizeAddMeetingNow();
        }

        // GET: /Meeting/InitilizeAddNCRLog
         
        private ActionResult InitilizeAddMeetingNow()
        {
            ViewBag.SubMenutype = "AddMeetingNow";
            try
            {
                MeetingTypeModels objMeetingType = new MeetingTypeModels();
                ViewBag.MeetingType = objMeetingType.GetmeetingtypeListbox();
                ViewBag.item_status = objGlobaldata.GetDropdownList("Meeting Item Status");
               // ViewBag.item_status = objGlobaldata.GetConstantValue("AuditStatus");
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriodMeeting");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddMeetingNow: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMeetingNow(MeetingModels objMeeting, FormCollection form, MeetingItemModels MeetingItem, HttpPostedFileBase Attendees)
        {
            ViewBag.SubMenutype = "AddMeetingNow";

            try
            {

                //Code to get Meeting details
                objMeeting.meeting_type_desc = form["meeting_type_desc"];
                objMeeting.last_meeting_id = form["last_meeting_id"];
                objMeeting.meeting_ref = form["meeting_ref"];
              
                objMeeting.MeetingCreatedBy = objGlobaldata.GetCurrentUserSession().empid;
                
                objMeeting.NotificationPeriod = form["NotificationPeriod"];
                objMeeting.NotificationValue = form["NotificationValue"];

                decimal Notificationval = 1;

                if (objMeeting.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objMeeting.NotificationValue = "0";
                }
                else if (objMeeting.NotificationPeriod == "Weeks" && decimal.TryParse(objMeeting.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objMeeting.NotificationPeriod == "Months" && decimal.TryParse(objMeeting.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                else if (objMeeting.NotificationPeriod == "Minutes" && decimal.TryParse(objMeeting.NotificationValue, out Notificationval))
                {
                    Notificationval = Notificationval / 1440;
                }
                objMeeting.NotificationDays = Convert.ToDecimal(Notificationval);
                
                //objMeeting.meeting_ref = form["meeting_ref"];
                //objMeeting.MeetingCreatedBy = objGlobaldata.GetCurrentUserSession().empid;

                objMeeting.AttendeesUser = form["AttendeesUser"];
                objMeeting.ext_attendees = form["ext_attendees"];
                objMeeting.ext_email = form["ext_email"];

                DateTime dateValue;

                if (form["meeting_date"] != null && DateTime.TryParse(form["meeting_date"], out dateValue) == true)
                {
                    objMeeting.meeting_date = dateValue;
                }

                if (form["Agendas"] != null && form["Agendas"] != "")
                {
                    //lstAgendaId = new List<string>(form["Agendas"].ToString().Split(','));
                    objMeeting.agenda_id = form["Agendas"].ToString();
                }

                //End

                //Code for Meeting items

                if (Attendees != null && Attendees.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Meeting"), Path.GetFileName(Attendees.FileName));
                        string sFilename = objMeeting.meeting_type_desc + "Attendees_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath),
                            sFilepath = Path.GetDirectoryName(spath);
                        Attendees.SaveAs(sFilepath + "/" + sFilename);
                        objMeeting.Attendees = "~/DataUpload/MgmtDocs/Meeting/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                MeetingItemModelsList objMeetingItemList = new MeetingItemModelsList();
                objMeetingItemList.MeetingItemMList = new List<MeetingItemModels>();

                List<string> lstAgendaId = new List<string>();

                if (form["itemcnt"] != null && Convert.ToInt16(form["itemcnt"]) > 0)
                {
                    string sitem_discussed = "", saction_agreed = "", saction_owner = "", sitem_status = "", sAgendaId = "", sremarks="";

                    for (int i = 1; i <= Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["item_discussed" + i] != null)
                        {
                            sitem_discussed = form["item_discussed" + i].Replace("'/", "");
                        }

                        if (form["action_agreed" + i] != null)
                        {
                            saction_agreed = form["action_agreed" + i].Replace("'/", "");
                        }

                        if (form["action_owner" + i] != null)
                        {
                            saction_owner = form["action_owner" + i].Replace("'/", "");
                        }

                        if (form["remarks" + i] != null)
                        {
                            sremarks = form["remarks" + i].Replace("'/", "");
                        }
                        if (form["item_status" + i] != null)
                        {
                            sitem_status = form["item_status" + i].Replace("'/", "");
                        }

                        if (form["CurrentAgendaId" + i] != null && !lstAgendaId.Contains(form["CurrentAgendaId" + i].Replace("'/", "")))
                        {
                            lstAgendaId.Add(form["CurrentAgendaId" + i].Replace("'/", ""));
                            sAgendaId = form["CurrentAgendaId" + i].Replace("'/", "");
                        }

                        MeetingItemModels objItem = new MeetingItemModels
                        {
                            item_discussed = sitem_discussed,
                            action_agreed = saction_agreed,
                            action_owner = saction_owner,
                            remarks = sremarks,
                            item_status = sitem_status,
                            agenda_id = sAgendaId,
                            Meeting_Ref = objMeeting.meeting_ref
                        };

                        if (form["due_date" + i] != null && DateTime.TryParse(form["due_date" + i].Replace("'/", ""), out dateValue) == true)
                        {
                            objItem.due_date = dateValue;
                        }

                        objMeetingItemList.MeetingItemMList.Add(objItem);
                    }
                }
                //end


                if (objMeeting.FunAddUnplanMeetingSchedule(objMeeting) && objMeeting.FunAddMeeting(objMeeting, objMeetingItemList, lstAgendaId))
                {
                    TempData["Successdata"] = "Added Meeting details successfully";

                    //Send MOM email
                    MeetingItem.SendMOMEmail(objMeeting.meeting_ref, "Minutes of Meeting");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }   

                //if ()
                //{
                //    TempData["Successdata"] = "Added Meeting details successfully";

                //    //Send MOM email
                //    //MeetingItemModels MeetingItem = new MeetingItemModels();
                //    MeetingItem.SendMOMEmail(objMeeting.meeting_ref, "Meeting Request");
                //}
                //else
                //{
                //    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                //}
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddMeetingNow: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            //return View(objMeeting);
            return RedirectToAction("MeetingList");
        }

        // GET: /Meeting/MeetingList
         
        [AllowAnonymous]
        public JsonResult MeetingDelete(FormCollection form)
        {
            try
            {
              
                   
                        if (form["meeting_id"] != null && form["meeting_id"] != "")
                        {

                            MeetingModels Doc = new MeetingModels();
                            string smeeting_id = form["meeting_id"];


                            if (Doc.FunDeleteMeetingDoc(smeeting_id))
                            {
                                TempData["Successdata"] = "Meeting deleted successfully";
                                return Json("Success");
                            }
                            else
                            {
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return Json("Failed");
                            }
                        }
                        else
                        {
                            TempData["alertdata"] = "Meeting Id cannot be Null or empty";
                            return Json("Failed");
                        }
                    
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

         
        [AllowAnonymous]
        public ActionResult MeetingList(string branch_name)
        {
            ViewBag.SubMenutype = "MOM";

            MeetingModelsList objMeetingModelsList = new MeetingModelsList();
            objMeetingModelsList.MeetingMList = new List<MeetingModels>();

            MeetingTypeModels objType = new MeetingTypeModels();           
            
            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                ViewBag.MeetingType = objType.GetmeetingtypeListbox();
                string sSqlstmt = "SELECT distinct (MeetingName) as meeting_type,meeting_id, last_meeting_id, meeting_date,  meeting_ref,  Attendees, AttendeesUser, Venue"
                    + ", NotificationPeriod, NotificationValue,ext_attendees,ext_email  from t_meeting, t_meetingtype  where meeting_type_desc= t_meetingtype.Id and Active='1'";

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " group by meeting_ref order by meeting_date desc";

                DataSet dsMeetingList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsMeetingList.Tables.Count > 0)
                {            
                        
                    
                    for (int i = 0; i < dsMeetingList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtMeetingDate = Convert.ToDateTime(dsMeetingList.Tables[0].Rows[i]["meeting_date"].ToString());

                        try
                        {
                            MeetingModels objMeetingModels = new MeetingModels
                            {
                                last_meeting_id = dsMeetingList.Tables[0].Rows[i]["last_meeting_id"].ToString(),
                                meeting_type_desc = (dsMeetingList.Tables[0].Rows[i]["meeting_type"].ToString()),
                                meeting_date = dtMeetingDate,
                                meeting_ref = dsMeetingList.Tables[0].Rows[i]["meeting_ref"].ToString(),
                                Attendees = dsMeetingList.Tables[0].Rows[i]["Attendees"].ToString(),
                                meeting_id = dsMeetingList.Tables[0].Rows[i]["meeting_id"].ToString(),
                                Venue = dsMeetingList.Tables[0].Rows[i]["Venue"].ToString(),
                                AttendeesUser = objGlobaldata.GetMultiHrEmpNameById(dsMeetingList.Tables[0].Rows[i]["AttendeesUser"].ToString()),
                                NotificationPeriod = dsMeetingList.Tables[0].Rows[i]["NotificationPeriod"].ToString(),
                                NotificationValue = dsMeetingList.Tables[0].Rows[i]["NotificationValue"].ToString(),
                                ext_attendees = dsMeetingList.Tables[0].Rows[i]["ext_attendees"].ToString(),
                                ext_email = dsMeetingList.Tables[0].Rows[i]["ext_email"].ToString()
                            };
                            objMeetingModelsList.MeetingMList.Add(objMeetingModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in MeetingList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objMeetingModelsList.MeetingMList.ToList());
        }

        [AllowAnonymous]
        public JsonResult MeetingListSearch(string branch_name)
        {
            ViewBag.SubMenutype = "MOM";

            MeetingModelsList objMeetingModelsList = new MeetingModelsList();
            objMeetingModelsList.MeetingMList = new List<MeetingModels>();

            MeetingTypeModels objType = new MeetingTypeModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                ViewBag.MeetingType = objType.GetmeetingtypeListbox();
                string sSqlstmt = "SELECT distinct (MeetingName) as meeting_type,meeting_id, last_meeting_id, meeting_date,  meeting_ref,  Attendees, AttendeesUser, Venue"
                    + ", NotificationPeriod, NotificationValue,ext_attendees,ext_email  from t_meeting, t_meetingtype  where meeting_type_desc= t_meetingtype.Id and Active='1'";

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " group by meeting_ref order by meeting_date desc";

                DataSet dsMeetingList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsMeetingList.Tables.Count > 0)
                {                                       

                    for (int i = 0; i < dsMeetingList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtMeetingDate = Convert.ToDateTime(dsMeetingList.Tables[0].Rows[i]["meeting_date"].ToString());

                        try
                        {
                            MeetingModels objMeetingModels = new MeetingModels
                            {
                                last_meeting_id = dsMeetingList.Tables[0].Rows[i]["last_meeting_id"].ToString(),
                                meeting_type_desc = (dsMeetingList.Tables[0].Rows[i]["meeting_type"].ToString()),
                                meeting_date = dtMeetingDate,
                                meeting_ref = dsMeetingList.Tables[0].Rows[i]["meeting_ref"].ToString(),
                                Attendees = dsMeetingList.Tables[0].Rows[i]["Attendees"].ToString(),
                                meeting_id = dsMeetingList.Tables[0].Rows[i]["meeting_id"].ToString(),
                                Venue = dsMeetingList.Tables[0].Rows[i]["Venue"].ToString(),
                                AttendeesUser = objGlobaldata.GetMultiHrEmpNameById(dsMeetingList.Tables[0].Rows[i]["AttendeesUser"].ToString()),
                                NotificationPeriod = dsMeetingList.Tables[0].Rows[i]["NotificationPeriod"].ToString(),
                                NotificationValue = dsMeetingList.Tables[0].Rows[i]["NotificationValue"].ToString(),
                                ext_attendees = dsMeetingList.Tables[0].Rows[i]["ext_attendees"].ToString(),
                                ext_email = dsMeetingList.Tables[0].Rows[i]["ext_email"].ToString()
                            };
                            objMeetingModelsList.MeetingMList.Add(objMeetingModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in MeetingListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
        // GET: /Meeting/MeetingScheduleEdit

        [AllowAnonymous]
        public ActionResult MeetingScheduleEdit()
        {
            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                if (Request.QueryString["meeting_id"] != null && Request.QueryString["meeting_id"] != "")
                {

                    string smeeting_id = Request.QueryString["meeting_id"];

                    MeetingTypeModels objType = new MeetingTypeModels();
                    MeetingAgendaModels objMeetingAgendaModels = new MeetingAgendaModels();

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "SELECT agenda_id, meeting_id, last_meeting_id, meeting_date, meeting_type_desc, meeting_ref,  Attendees, AttendeesUser, Venue"
                        + ", NotificationPeriod, NotificationValue,ext_attendees,ext_email,remarks from t_meeting where meeting_id='" + smeeting_id + "'";

                    DataSet dsMeetingList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMeetingList.Tables.Count > 0 && dsMeetingList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtMeetingDate = Convert.ToDateTime(dsMeetingList.Tables[0].Rows[0]["meeting_date"].ToString());

                        MeetingModels objMeetingModels = new MeetingModels
                        {
                            meeting_id = dsMeetingList.Tables[0].Rows[0]["meeting_id"].ToString(),
                            last_meeting_id = dsMeetingList.Tables[0].Rows[0]["last_meeting_id"].ToString(),
                            meeting_type_desc = dsMeetingList.Tables[0].Rows[0]["meeting_type_desc"].ToString(),
                            meeting_date = dtMeetingDate,
                            meeting_ref = dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString(),
                            // Attendees = (dsMeetingList.Tables[0].Rows[0]["Attendees"].ToString()),
                            agenda_id = dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString(),
                            Venue = dsMeetingList.Tables[0].Rows[0]["Venue"].ToString(),
                            AttendeesUser =(dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString()),
                            NotificationPeriod = dsMeetingList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsMeetingList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            ext_attendees = dsMeetingList.Tables[0].Rows[0]["ext_attendees"].ToString(),
                            ext_email = dsMeetingList.Tables[0].Rows[0]["ext_email"].ToString(),
                            remarks = dsMeetingList.Tables[0].Rows[0]["remarks"].ToString(),
                        };
                        // ViewBag.AgendaList = objMeetingAgendaModels.GetMeetingAgendaListbox(dsMeetingList.Tables[0].Rows[0]["meeting_type_desc"].ToString());

                        ViewBag.AgendaList = objGlobaldata.Getdetails("select Agenda_Id, Agenda_Desc,Agenda_Details from t_meeting_agenda where MeetingType_Id='" + (dsMeetingList.Tables[0].Rows[0]["meeting_type_desc"].ToString()) + "'");
                        ViewBag.MeetingAgendas = objMeetingAgendaModels.GetOnlyMeetingAgendaById(dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString());
                        ViewBag.Agendadetails = objMeetingAgendaModels.GetMeetingAgendadetailsById(dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString());
                        // ViewBag.MeetingAgendas = objMeetingAgendaModels.GetOnlyMeetingAgendaById(dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString());
                        ViewBag.MeetingType = objType.GetmeetingtypeListbox();

                        ViewBag.item_status = objGlobaldata.GetDropdownList("Meeting Item Status");
                        // ViewBag.item_status = objGlobaldata.GetConstantValue("AuditStatus");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriodMeeting");
                        ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                        ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        MeetingModelsList objModelsList = new MeetingModelsList();
                        objModelsList.MeetingMList = new List<MeetingModels>();

                        sSqlstmt = "select id_attendees,meeting_id,company_name,attendee_name,designation,email_id from t_meeting_external_attendees where meeting_id='" + smeeting_id + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    MeetingModels objMeeting = new MeetingModels
                                    {
                                        id_attendees = dsList.Tables[0].Rows[i]["id_attendees"].ToString(),
                                        meeting_id = dsList.Tables[0].Rows[i]["meeting_id"].ToString(),
                                        company_name = dsList.Tables[0].Rows[i]["company_name"].ToString(),
                                        attendee_name = dsList.Tables[0].Rows[i]["attendee_name"].ToString(),
                                        designation = dsList.Tables[0].Rows[i]["designation"].ToString(),
                                        email_id = dsList.Tables[0].Rows[i]["email_id"].ToString(),
                                    };
                                    objModelsList.MeetingMList.Add(objMeeting);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in MeetingScheduleEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("MeetingList");
                                }
                            }
                            ViewBag.objList = objModelsList;
                        }

                        return View(objMeetingModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("MeetingList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Meeting Ref cannot be Null or empty";
                    return RedirectToAction("MeetingList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingScheduleEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return RedirectToAction("MeetingList");
        }

        //POST: /Meeting/MeetingEdit
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MeetingScheduleEdit(MeetingModels objMeetingModels, FormCollection form, HttpPostedFileBase fileAttendees)
        {
            ViewBag.SubMenutype = "MOM";
            string smeeting_ref = form["meeting_ref"];

            try
            {
                DateTime dateValue;

                if (DateTime.TryParse(form["meeting_date"], out dateValue) == true)
                {
                    objMeetingModels.meeting_date = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objMeetingModels.meeting_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
               
                objMeetingModels.meeting_ref = smeeting_ref;
             
                    objMeetingModels.MeetingCreatedBy = objGlobaldata.GetCurrentUserSession().empid;
                
                objMeetingModels.agenda_id = form["Agendas"];
                objMeetingModels.meeting_type_desc = form["meeting_type_desc"];
                objMeetingModels.meeting_id = form["meeting_id"];

                objMeetingModels.NotificationPeriod = form["NotificationPeriod"];
                objMeetingModels.NotificationValue = form["NotificationValue"];

                objMeetingModels.ext_attendees = form["ext_attendees"];
                objMeetingModels.ext_email = form["ext_email"];
                decimal Notificationval = 1;

                if (objMeetingModels.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objMeetingModels.NotificationValue = "0";
                }
                else if (objMeetingModels.NotificationPeriod == "Weeks" && decimal.TryParse(objMeetingModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objMeetingModels.NotificationPeriod == "Months" && decimal.TryParse(objMeetingModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                else if (objMeetingModels.NotificationPeriod == "Minutes" && decimal.TryParse(objMeetingModels.NotificationValue, out Notificationval))
                {
                    Notificationval = Notificationval / 1440;
                }
                objMeetingModels.NotificationDays = Convert.ToDecimal(Notificationval);
               
                objMeetingModels.AttendeesUser = form["AttendeesUser"];
                MeetingModelsList objModelsList = new MeetingModelsList();
                objModelsList.MeetingMList = new List<MeetingModels>();
                //external attendees
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    MeetingModels objModels = new MeetingModels();

                    if (form["company_name" + i] != null && form["company_name" + i] != "")
                    {
                        objModels.company_name = form["company_name" + i];
                        objModels.attendee_name = form["attendee_name" + i];
                        objModels.designation = form["designation" + i];
                        objModels.email_id = form["email_id" + i];
                        objModelsList.MeetingMList.Add(objModels);
                    }
                }

                if (objMeetingModels.FunUpdateMeeting(objMeetingModels, objModelsList))
                {
                    TempData["Successdata"] = "Meeting details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

                //Send MOM email
                MeetingItemModels objItem = new MeetingItemModels();
                objItem.SendMOMEmail(smeeting_ref, "Meeting Scheduled Update");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingScheduleEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MeetingList");//View();
        }

        // GET: /Meeting/MeetingDetails
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MeetingDetails(FormCollection form)
        {
            string smeeting_ref = form["meeting_ref"];

            MeetingModels objMeetingModels = new MeetingModels();

            MeetingAgendaModels objMeetingAgendaModels = new MeetingAgendaModels();
            MeetingTypeModels objType = new MeetingTypeModels();
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            try
            {
                if (smeeting_ref != "")
                {

                    //string smeeting_ref = Request.QueryString["meeting_ref"];

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "SELECT agenda_id, meeting_id, last_meeting_id, meeting_date, meeting_type_desc, meeting_ref,  Attendees, AttendeesUser, Venue ,ext_attendees,ext_email "
                        + "from t_meeting where meeting_ref='" + smeeting_ref + "' order by agenda_id desc limit 1";

                    DataSet dsMeetingList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMeetingList.Tables.Count > 0 && dsMeetingList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtMeetingDate = Convert.ToDateTime(dsMeetingList.Tables[0].Rows[0]["meeting_date"].ToString());

                        objMeetingModels = new MeetingModels
                        {
                            meeting_id = dsMeetingList.Tables[0].Rows[0]["meeting_id"].ToString(),
                            last_meeting_id = dsMeetingList.Tables[0].Rows[0]["last_meeting_id"].ToString(),
                            meeting_type_desc = objType.GetMeetingTypeNameById(dsMeetingList.Tables[0].Rows[0]["meeting_type_desc"].ToString()),
                            meeting_date = dtMeetingDate,
                            meeting_ref = dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString(),
                            Attendees = (dsMeetingList.Tables[0].Rows[0]["Attendees"].ToString()),
                            agenda_id = objMeetingAgendaModels.GetMeetingAgendaNameById(dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString()),
                            Venue = dsMeetingList.Tables[0].Rows[0]["Venue"].ToString(),
                            AttendeesUser = objGlobaldata.GetMultiHrEmpNameById(dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString()),
                            ext_attendees = dsMeetingList.Tables[0].Rows[0]["ext_attendees"].ToString(),
                            ext_email = dsMeetingList.Tables[0].Rows[0]["ext_email"].ToString(),
                        };
                        ViewBag.Meeting = objMeetingModels;
                        ViewBag.meeting_id = dsMeetingList.Tables[0].Rows[0]["meeting_id"].ToString();
                        ViewBag.Agenda_Id = dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString();

                        CompanyModels objCompany = new CompanyModels();
                        dsMeetingList = objCompany.GetCompanyDetailsForReport(dsMeetingList);

                        string loggedby = objGlobaldata.GetCurrentUserSession().empid;
                        dsMeetingList = objGlobaldata.GetReportDetails(dsMeetingList, objMeetingModels.meeting_ref, loggedby, "MEETING REPORT");
                        ViewBag.CompanyInfo = dsMeetingList;

                        string sql = "select * from t_meeting_external_attendees where meeting_id='" + objMeetingModels.meeting_id + "'";
                        ViewBag.items = objGlobaldata.Getdetails(sql);

                        sSqlstmt = "select item_no, t_meeting_items.Agenda_Id, item_discussed, action_agreed, due_date, action_owner,"
                       + "item_status, completiondate,t_meeting_items.remarks,upload from t_meeting_items, t_meeting where t_meeting_items.meeting_ref=t_meeting.meeting_ref and"
                       + " t_meeting.meeting_ref='" + dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString() + "' order by item_no desc";
                        DataSet dsItems = new DataSet();
                        dsItems = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.Itemlist = dsItems;
                 
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                   // TempData["alertdata"] = "Meeting Ref cannot be Null or empty";
                    return View();
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string header = Server.MapPath("~/views/Meeting/PrintHeader.html");//Path of PrintHeader.html File

            string customSwitches = string.Format("--header-html  \"{0}\" " +
                                 "--header-spacing \"0\" ", header);

            return new ViewAsPdf("MeetingDetailsToPDF","Meeting")
            {
                //FileName = "MeetingDetails.pdf",
                Cookies = cookieCollection,
                CustomSwitches = customSwitches
            };
           
        }

         
        //[AllowAnonymous]
        //public ActionResult MeetingDetailsToPDF(string smeeting_ref)
        //{
        //    ViewBag.SubMenutype = "MOM";

        //    MeetingModels objMeetingModels = new MeetingModels();

        //    MeetingAgendaModels objMeetingAgendaModels = new MeetingAgendaModels();
        //    MeetingTypeModels objType = new MeetingTypeModels();

        //    try
        //    {
        //        UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

        //        if (smeeting_ref!="")
        //        {

        //            //string smeeting_ref = Request.QueryString["meeting_ref"];

        //            //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
        //            string sSqlstmt = "SELECT agenda_id, meeting_id, last_meeting_id, meeting_date, meeting_type_desc, meeting_ref,  Attendees, AttendeesUser, Venue "
        //                + "from t_meeting where meeting_ref='" + smeeting_ref + "' order by agenda_id desc limit 1";

        //            DataSet dsMeetingList = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsMeetingList.Tables.Count > 0 && dsMeetingList.Tables[0].Rows.Count > 0)
        //            {
        //                DateTime dtMeetingDate = Convert.ToDateTime(dsMeetingList.Tables[0].Rows[0]["meeting_date"].ToString());

        //                objMeetingModels = new MeetingModels
        //                {
        //                    meeting_id = dsMeetingList.Tables[0].Rows[0]["meeting_id"].ToString(),
        //                    last_meeting_id = dsMeetingList.Tables[0].Rows[0]["last_meeting_id"].ToString(),
        //                    meeting_type_desc = objType.GetMeetingTypeNameById(dsMeetingList.Tables[0].Rows[0]["meeting_type_desc"].ToString()),
        //                    meeting_date = dtMeetingDate,
        //                    meeting_ref = dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString(),
        //                    Attendees = (dsMeetingList.Tables[0].Rows[0]["Attendees"].ToString()),
        //                    agenda_id = objMeetingAgendaModels.GetMeetingAgendaNameById(dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString()),
        //                    Venue = dsMeetingList.Tables[0].Rows[0]["Venue"].ToString(),
        //                    AttendeesUser = objGlobaldata.GetMultiHrEmpNameById(dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString())
        //                };
        //                ViewBag.meeting_id = dsMeetingList.Tables[0].Rows[0]["meeting_id"].ToString();
        //                ViewBag.Agenda_Id = dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString();

        //                sSqlstmt = "select item_no, t_meeting_items.Agenda_Id, item_discussed, action_agreed, due_date, action_owner,"
        //                + "item_status, completiondate,remarks from t_meeting_items, t_meeting where t_meeting_items.meeting_ref=t_meeting.meeting_ref and"
        //                + " t_meeting.meeting_ref='" + dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString() + "' order by item_no desc";
        //                DataSet dsItems = new DataSet();
        //                dsItems = objGlobaldata.Getdetails(sSqlstmt);
        //                ViewBag.Itemlist = dsItems;
        //                return View();
        //            }
        //            else
        //            {
        //                return View();
        //            }
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = "Meeting Ref cannot be Null or empty";
        //            return View();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in MeetingDetailsToPDF: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View();

        //}

         
        [AllowAnonymous]
        public ActionResult MeetingDetails()
        {
            ViewBag.SubMenutype = "MOM";

            MeetingModels objMeetingModels = new MeetingModels();

            MeetingAgendaModels objMeetingAgendaModels = new MeetingAgendaModels();
            MeetingTypeModels objType = new MeetingTypeModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                if (Request.QueryString["meeting_ref"] != null && Request.QueryString["meeting_ref"] != "")
                {

                    string smeeting_ref = Request.QueryString["meeting_ref"];                    

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "SELECT agenda_id, meeting_id, last_meeting_id, meeting_date, meeting_type_desc, meeting_ref,  Attendees, AttendeesUser, Venue,ext_attendees,ext_email,remarks,action_status,status_update_on "
                        + "from t_meeting where meeting_ref='" + smeeting_ref + "' order by agenda_id desc limit 1";

                    DataSet dsMeetingList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMeetingList.Tables.Count > 0 && dsMeetingList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtMeetingDate = Convert.ToDateTime(dsMeetingList.Tables[0].Rows[0]["meeting_date"].ToString());

                        objMeetingModels = new MeetingModels
                        {
                            meeting_id = dsMeetingList.Tables[0].Rows[0]["meeting_id"].ToString(),
                            last_meeting_id = dsMeetingList.Tables[0].Rows[0]["last_meeting_id"].ToString(),
                            meeting_type_desc = objType.GetMeetingTypeNameById(dsMeetingList.Tables[0].Rows[0]["meeting_type_desc"].ToString()),
                            meeting_date = dtMeetingDate,
                            meeting_ref = dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString(),
                            Attendees = (dsMeetingList.Tables[0].Rows[0]["Attendees"].ToString()),
                            agenda_id = objMeetingAgendaModels.GetMeetingAgendaNameById(dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString()),
                            Venue = dsMeetingList.Tables[0].Rows[0]["Venue"].ToString(),
                            AttendeesUser = objGlobaldata.GetMultiHrEmpNameById(dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString()),
                            ext_attendees = dsMeetingList.Tables[0].Rows[0]["ext_attendees"].ToString(),
                            ext_email = dsMeetingList.Tables[0].Rows[0]["ext_email"].ToString(),
                            remarks = dsMeetingList.Tables[0].Rows[0]["remarks"].ToString(),
                            action_status =objGlobaldata.GetDropdownitemById(dsMeetingList.Tables[0].Rows[0]["action_status"].ToString()),
                        };
                        DateTime dtValue;
                        if (dsMeetingList.Tables[0].Rows[0]["status_update_on"].ToString() != ""
                         && DateTime.TryParse(dsMeetingList.Tables[0].Rows[0]["status_update_on"].ToString(), out dtValue))
                        {
                            objMeetingModels.status_update_on = dtValue;
                        }
                        //if(dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString() != "")
                        //{
                        //    ViewBag.attendee = dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString().Split(',');
                        //}
                        
                        ViewBag.meeting_id = dsMeetingList.Tables[0].Rows[0]["meeting_id"].ToString();
                        ViewBag.Agenda_Id = dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString();

                        string sql = "select * from t_meeting_external_attendees where meeting_id='" + objMeetingModels.meeting_id + "'";
                        ViewBag.items = objGlobaldata.Getdetails(sql);

                        sSqlstmt = "select item_no, t_meeting_items.Agenda_Id, item_discussed, action_agreed, due_date, action_owner,"
                        + "item_status, completiondate,t_meeting_items.remarks,upload from t_meeting_items, t_meeting where t_meeting_items.meeting_ref=t_meeting.meeting_ref and"
                        + " t_meeting.meeting_ref='" + dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString() + "' order by item_no desc";
                        DataSet dsItems = new DataSet();
                        dsItems = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.Itemlist = dsItems;
                        return View(objMeetingModels);
                    }
                    else
                    {
                        return RedirectToAction("AddMOM", new { meeting_ref = smeeting_ref });
                    }
                }
                else
                {
                    TempData["alertdata"] = "Meeting Ref cannot be Null or empty";
                    return RedirectToAction("MeetingList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("MeetingList");
            
        }

         
        [AllowAnonymous]
        public ActionResult MeetingInfo(int id)
        {
            ViewBag.SubMenutype = "MOM";

            MeetingModels objMeetingModels = new MeetingModels();

            MeetingAgendaModels objMeetingAgendaModels = new MeetingAgendaModels();
            MeetingTypeModels objType = new MeetingTypeModels();

            try
            {
                    string sSqlstmt = "SELECT agenda_id, meeting_id, last_meeting_id, meeting_date, meeting_type_desc, meeting_ref,  Attendees, AttendeesUser, Venue,ext_attendees,ext_email "
                        + "from t_meeting where meeting_id='" + id + "' order by agenda_id desc limit 1";

                    DataSet dsMeetingList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMeetingList.Tables.Count > 0 && dsMeetingList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtMeetingDate = Convert.ToDateTime(dsMeetingList.Tables[0].Rows[0]["meeting_date"].ToString());

                        objMeetingModels = new MeetingModels
                        {
                            meeting_id = dsMeetingList.Tables[0].Rows[0]["meeting_id"].ToString(),
                            last_meeting_id = dsMeetingList.Tables[0].Rows[0]["last_meeting_id"].ToString(),
                            meeting_type_desc = objType.GetMeetingTypeNameById(dsMeetingList.Tables[0].Rows[0]["meeting_type_desc"].ToString()),
                            meeting_date = dtMeetingDate,
                            meeting_ref = dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString(),
                            Attendees = (dsMeetingList.Tables[0].Rows[0]["Attendees"].ToString()),
                            agenda_id = objMeetingAgendaModels.GetMeetingAgendaNameById(dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString()),
                            Venue = dsMeetingList.Tables[0].Rows[0]["Venue"].ToString(),
                            AttendeesUser = objGlobaldata.GetMultiHrEmpNameById(dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString()),
                            ext_attendees = dsMeetingList.Tables[0].Rows[0]["ext_attendees"].ToString(),
                            ext_email = dsMeetingList.Tables[0].Rows[0]["ext_email"].ToString(),
                        };
                   
                        sSqlstmt = "select item_no, t_meeting_items.Agenda_Id, item_discussed, action_agreed, due_date, action_owner,"
                        + "item_status, completiondate,t_meeting_items.remarks,upload from t_meeting_items, t_meeting where t_meeting_items.meeting_ref=t_meeting.meeting_ref and"
                        + " t_meeting.meeting_ref='" + dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString() + "' order by item_no desc";
                        DataSet dsItems = new DataSet();
                        dsItems = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.Itemlist = dsItems;
                        return View(objMeetingModels);
                    }
                    
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("MeetingList");

        }
        // GET: /Meeting/MeetingEdit

         
        [AllowAnonymous]
        public ActionResult MeetingEdit()
        {
            ViewBag.SubMenutype = "MOM";           

            MeetingTypeModels objType = new MeetingTypeModels();
            MeetingAgendaModels objMeetingAgendaModels = new MeetingAgendaModels();
            MeetingModels objMeetingModels = new MeetingModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                if (Request.QueryString["meeting_ref"] != null && Request.QueryString["meeting_ref"] != "")
                {

                    string sMeeting_ref = Request.QueryString["meeting_ref"];
                    ViewBag.meeting_ref = sMeeting_ref;
                                                          
                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                     string sSqlstmt = "SELECT agenda_id, meeting_id, last_meeting_id, meeting_date, meeting_type_desc, meeting_ref,  Attendees, AttendeesUser, Venue"
                        +" from t_meeting where meeting_ref='" + sMeeting_ref + "' order by meeting_id desc";

                    DataSet dsMeetingList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMeetingList.Tables.Count > 0 && dsMeetingList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtMeetingDate = Convert.ToDateTime(dsMeetingList.Tables[0].Rows[0]["meeting_date"].ToString());

                        objMeetingModels = new MeetingModels
                        {
                            meeting_id = dsMeetingList.Tables[0].Rows[0]["meeting_id"].ToString(),
                            last_meeting_id = dsMeetingList.Tables[0].Rows[0]["last_meeting_id"].ToString(),
                            meeting_type_desc = objType.GetMeetingTypeNameById(dsMeetingList.Tables[0].Rows[0]["meeting_type_desc"].ToString()),
                            meeting_date = dtMeetingDate,
                            meeting_ref = dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString(),
                            Attendees = (dsMeetingList.Tables[0].Rows[0]["Attendees"].ToString()),
                            agenda_id = objMeetingAgendaModels.GetMeetingAgendaNameById(dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString()),
                            Venue = dsMeetingList.Tables[0].Rows[0]["Venue"].ToString(),
                            AttendeesUser = objGlobaldata.GetMultiHrEmpNameById(dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString())
                        };


                        ViewBag.agenda_id = dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString();

                        //ViewBag.MeetingType = objGlobaldata.GetMeetingTypeListbox();                      

                        ViewBag.AgendaList = objMeetingAgendaModels.GetOnlyMeetingAgendasListbox(sMeeting_ref, dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString());
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();

                        ViewBag.item_status = objGlobaldata.GetDropdownList("Meeting Item Status");
                        //ViewBag.item_status = objGlobaldata.GetConstantValue("AuditStatus");

                        sSqlstmt = "select item_no, t_meeting_items.agenda_id, item_discussed, action_agreed,"
                        + "due_date, action_owner, item_status, completiondate,t_meeting_items.remarks,upload from t_meeting_items, t_meeting"
                        + " where t_meeting_items.meeting_ref=t_meeting.meeting_ref and t_meeting.meeting_ref='" + sMeeting_ref + "' order by item_no desc";

                        DataSet dsItems = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.Itemlist = dsItems;

                        if (dsItems.Tables.Count == 0 || dsItems.Tables[0].Rows.Count == 0)
                        {
                            return RedirectToAction("AddMOM", new { meeting_ref = sMeeting_ref });
                        }

                        return View(objMeetingModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("MeetingList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Meeting Ref cannot be Null or empty";
                    return RedirectToAction("MeetingList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }           
            return RedirectToAction("MeetingList");
        }

        //POST: /Meeting/MeetingEdit
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MeetingEdit(MeetingModels objMeetingModels, FormCollection form, HttpPostedFileBase fileAttendees, HttpPostedFileBase upload)
        {
            ViewBag.SubMenutype = "MOM";
            string smeeting_ref = form["meeting_ref"];
            string sendmail = Request.Form["selectall"];
            try
            {
                if (Request["button"].Equals("Meeting Update"))
                {
                    objMeetingModels.meeting_ref = smeeting_ref;
                    
                        objMeetingModels.MeetingCreatedBy = objGlobaldata.GetCurrentUserSession().empid;
                    

                    if (fileAttendees != null && fileAttendees.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Meeting"), Path.GetFileName(fileAttendees.FileName));
                            string sFilename = objMeetingModels.meeting_type_desc + "Attendees_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), 
                                sFilepath = Path.GetDirectoryName(spath);
                            fileAttendees.SaveAs(sFilepath + "/" +  sFilename);
                            objMeetingModels.Attendees = "~/DataUpload/MgmtDocs/Meeting/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (objMeetingModels.FunUpdateAttendeesSheet(objMeetingModels))
                    {
                        TempData["Successdata"] = "Meeting details updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else if (Request["button"].Equals("Save"))
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Meeting"), Path.GetFileName(upload.FileName));
                            string sFilename = DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath),
                            sFilepath = Path.GetDirectoryName(spath);
                            upload.SaveAs(sFilepath + "/" + sFilename);
                            objMeetingModels.upload = "~/DataUpload/MgmtDocs/Meeting/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (MeetingItemAdd(objMeetingModels, form))
                    {
                        TempData["Successdata"] = "Added Meeting Item details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Meeting"), Path.GetFileName(upload.FileName));
                            string sFilename = DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath),
                            sFilepath = Path.GetDirectoryName(spath);
                            upload.SaveAs(sFilepath + "/" + sFilename);
                            objMeetingModels.upload = "~/DataUpload/MgmtDocs/Meeting/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (MeetingItemUpdate(objMeetingModels, form))
                    {
                        TempData["Successdata"] = "Meeting Item details updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }

                //Send MOM email
                MeetingItemModels objItem = new MeetingItemModels();
                //if (sendmail != null && sendmail != "")
                //{
                    objItem.SendMOMEmail(smeeting_ref, "Update on Minutes of Meeting");
                //}
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MeetingEdit", new { meeting_ref = smeeting_ref });//View();
        }


        //POST: /Meeting/MeetingItemUpdate
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool MeetingItemUpdate(MeetingModels objMeetingModels, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "MOM";

                objMeetingModels.item_discussed = form["item_discussed"];
                objMeetingModels.action_agreed = form["action_agreed"];
                objMeetingModels.remarks = form["remarks"];
                objMeetingModels.action_owner = form["action_owner"] ;
                objMeetingModels.item_status = form["item_status"];
                objMeetingModels.item_no = form["item_no"];

                DateTime dateValue;

                if (DateTime.TryParse(form["due_date"], out dateValue) == true)
                {
                    objMeetingModels.due_date = dateValue;
                }               

                //ExternalAuditModels objExternalAuditModels = new ExternalAuditModels();
                return objMeetingModels.FunUpdateMeetingItem(objMeetingModels);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingItemUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return false;
            }
        }


        //POST: /Meeting/MeetingItemAdd
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool MeetingItemAdd(MeetingModels objMeetingModels, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "MOM";

                MeetingModelsList objMeetingModelsList = new MeetingModelsList();
                objMeetingModelsList.MeetingMList = new List<MeetingModels>();

                objMeetingModels.item_discussed = form["item_discussed"];
                objMeetingModels.action_agreed = form["action_agreed"];
                objMeetingModels.remarks = form["remarks"];
                objMeetingModels.action_owner = form["action_owner"];
                objMeetingModels.item_status = form["item_status"];
                objMeetingModels.agenda_id = form["agenda_id"];

                DateTime dateValue;

                if (DateTime.TryParse(form["due_date"], out dateValue) == true)
                {
                    objMeetingModels.due_date = dateValue;
                }
                

                objMeetingModelsList.MeetingMList.Add(objMeetingModels);
                //ExternalAuditModels objExternalAuditModels = new ExternalAuditModels();
                return objMeetingModels.FunAddMeetingItems(objMeetingModelsList, (objMeetingModels.agenda_id));
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingItemAdd: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return false;
            }
        }


        //POST: /Meeting/MeetingItemEdit
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string MeetingItemEdit(string sitem_discussed, string saction_agreed, string sdue_date, string sitem_status, string saction_owner, string sItemNo,string sremarks)
        {
            try
            {
                ViewBag.SubMenutype = "MOM";

                MeetingItemModels objItem = new MeetingItemModels()
                {
                    item_discussed = sitem_discussed,
                    action_agreed = saction_agreed,
                    item_status = sitem_status,
                    remarks = sremarks,
                    action_owner = saction_owner.TrimEnd(','),
                    item_no = sItemNo
                };
               
                DateTime dateValue;

                if (DateTime.TryParse(sdue_date, out dateValue) == true)
                {
                    objItem.due_date = dateValue;
                }


                if (objItem.FunUpdateMeetingItem(objItem))
                {
                    TempData["Successdata"] = "Meeting Item details updated successfully";
                    return "Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingItemEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return "Failed";
        }
        

        //
        // GET: /Meeting/AddMeetingItemLog
        
        [AllowAnonymous]
        public  ActionResult AddMeetingItemLog()
        {
            return InitilizeAddMeetingItemLog();
        }

        // GET: /Meeting/InitilizeAddNCRLog
        
        private ActionResult InitilizeAddMeetingItemLog()
        {
            try
            {
                ViewBag.SubMenutype = "MOM";

                ViewBag.Item_no = Request.QueryString["Item_no"];
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                //ViewBag.LogStatus = objGlobaldata.GetConstantValue("AuditStatus");
                ViewBag.LogStatus = objGlobaldata.GetDropdownList("Meeting Item Status");


                DataSet dsMeeting = objGlobaldata.Getdetails("select item_no, meeting_ref, item_discussed from t_meeting_items where item_no='"
                    + Request.QueryString["Item_no"] + "' order by item_no desc");

                if (dsMeeting.Tables[0].Rows.Count > 0)
                {
                    MeetingModels objMeeting = new MeetingModels();
                    //ViewBag.meeting_ref = objMeeting.GetMeetingRefbyId(dsMeeting.Tables[0].Rows[0]["agenda_Id"].ToString());
                    ViewBag.meeting_ref = dsMeeting.Tables[0].Rows[0]["meeting_ref"].ToString();
                    ViewBag.item_discussed = dsMeeting.Tables[0].Rows[0]["item_discussed"].ToString();
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddMeetingItemLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View("");
        }

         //
        // POST: /Meeting/AddMeetingItemLog
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMeetingItemLog(MeetingItemLogModels objLog, FormCollection form)
        {
            objLog.action_completed_by = form["action_completed_by"];
            objLog.item_no = form["Item_no"];
            objLog.LogStatus = form["LogStatus"];
           
            if (objLog.FunAddItemLog(objLog))
            {
                TempData["Successdata"] = "Added Meeting Item log details successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MeetingItemLogList", "Meeting", new { Item_no = objLog.item_no });
        }

        //
        // GET: /Meeting/MeetingItemLogList
        
        [AllowAnonymous]
        public ActionResult MeetingItemLogList(string item_no, string meeting_ref)
        {
            ViewBag.SubMenutype = "MOM";
           
            //objGlobaldata.CreateUserSession();
            MeetingItemLogModelsList objMeetingItemLogList = new MeetingItemLogModelsList();
            objMeetingItemLogList.MeetingItemLogMList = new List<MeetingItemLogModels>();

            try
            {
                //ViewBag.meeting_ref = meeting_ref;
                ViewBag.meeting_ref = objGlobaldata.GetMettingRefList();
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                //ViewBag.LogStatus = objGlobaldata.GetConstantValue("AuditStatus");
                ViewBag.LogStatus = objGlobaldata.GetDropdownList("Meeting Item Status");
                ViewBag.Item_no = item_no;
                string sSqlstmt = "select meeting_log_id, agenda_id, t_meeting_items_log.Item_no, action_taken_on, action_completed_by, item_discussed, LogStatus,remarks from t_meeting_items_log , "
                    + "t_meeting_items where t_meeting_items.Item_no=t_meeting_items_log.Item_no and t_meeting_items_log.Item_no='" + item_no + "' order by meeting_log_id desc";
                DataSet dsItemlog = objGlobaldata.Getdetails(sSqlstmt);
                if (dsItemlog.Tables.Count > 0)
                {
                    for (int i = 0; i < dsItemlog.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtaction_taken_on = Convert.ToDateTime(dsItemlog.Tables[0].Rows[i]["action_taken_on"].ToString());

                        MeetingItemLogModels objMeetingItemLog = new MeetingItemLogModels
                        {
                            meeting_log_id = dsItemlog.Tables[0].Rows[i]["meeting_log_id"].ToString(),
                            action_completed_by = objGlobaldata.GetEmpHrNameById(dsItemlog.Tables[0].Rows[i]["action_completed_by"].ToString()),
                            action_taken_on = dtaction_taken_on,
                            item_no = dsItemlog.Tables[0].Rows[i]["item_discussed"].ToString(),
                            LogStatus = objGlobaldata.GetDropdownitemById(dsItemlog.Tables[0].Rows[i]["LogStatus"].ToString())
                        };
                        objMeetingItemLogList.MeetingItemLogMList.Add(objMeetingItemLog);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingItemLogList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }     
            return View(objMeetingItemLogList.MeetingItemLogMList.ToList());
        }

        //POST: /Meeting/MeetingItemLogEdit
        
        [HttpPost]
        // [AllowAnonymous]
        public JsonResult MeetingItemLogEdit(FormCollection form)
        {
            try
            {
                string item_no = form["item_no"], meeting_ref = form["item_no"], saction_taken_on = form["saction_taken_on"], saction_completed_by = form["saction_completed_by"];
                string sLogStatus = form["sLogStatus"], smeeting_log_id = form["smeeting_log_id"];

                ViewBag.SubMenutype = "MOM";

                
                MeetingItemLogModels objLog = new MeetingItemLogModels();
                DateTime dateValue;

                if (DateTime.TryParse(saction_taken_on, out dateValue) == true)
                {
                    objLog.action_taken_on = dateValue;
                }
                else if (saction_taken_on != null && saction_taken_on != "")
                {
                    objLog.action_taken_on = Convert.ToDateTime(saction_taken_on);
                }
                objLog.action_completed_by = saction_completed_by;
                objLog.meeting_log_id = smeeting_log_id;
                objLog.LogStatus = sLogStatus;

                if (objLog.FunUpdateItemLog(objLog))
                {
                    TempData["Successdata"] = "Meeting Item log details updated successfully";
                    return Json("Meeting Item log details updated successfully");
                }
                else
                {
                 
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    return Json("Update Failed");
                }

                //DataSet dsItem = objGlobaldata.Getdetails("select item_no from t_meeting_items_log where meeting_log_id='" + smeeting_log_id + "'");

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KPIEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return Json("Update Failed");
            }            
        }
        //
        // GET: /Meeting/AddMOM
        
        [AllowAnonymous]
         public ActionResult AddMOM(string sMeeting_ref)
        {
            ViewBag.SubMenutype = "MOM";
            return InitializeAddMOM();
        }
                       
        public ActionResult InitializeAddMOM()
        {
            MeetingTypeModels objType = new MeetingTypeModels();
            MeetingAgendaModels objMeetingAgendaModels = new MeetingAgendaModels();
            MeetingModels objMeetingModels = new MeetingModels();

            
            try
            {
                ViewBag.MeetingType = objType.GetmeetingtypeListbox();
                ViewBag.item_status = objGlobaldata.GetDropdownList("Meeting Item Status");

              //  ViewBag.item_status = objGlobaldata.GetConstantValue("AuditStatus");
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();

                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                if (Request.QueryString["meeting_ref"] != null && Request.QueryString["meeting_ref"] != "")
                {

                    string sMeeting_ref = Request.QueryString["meeting_ref"];
                    ViewBag.meeting_ref = sMeeting_ref;

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "SELECT agenda_id, meeting_id, last_meeting_id, meeting_date, meeting_type_desc, meeting_ref,  Attendees, AttendeesUser, Venue,ext_attendees,ext_email"
                        + " from t_meeting where meeting_ref='" + sMeeting_ref + "' order by meeting_id desc";

                    DataSet dsMeetingList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsMeetingList.Tables.Count > 0 && dsMeetingList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtMeetingDate = Convert.ToDateTime(dsMeetingList.Tables[0].Rows[0]["meeting_date"].ToString());

                        objMeetingModels = new MeetingModels
                        {
                            meeting_id = dsMeetingList.Tables[0].Rows[0]["meeting_id"].ToString(),
                            last_meeting_id = dsMeetingList.Tables[0].Rows[0]["last_meeting_id"].ToString(),
                            meeting_type_desc = objType.GetMeetingTypeNameById(dsMeetingList.Tables[0].Rows[0]["meeting_type_desc"].ToString()),
                            meeting_date = dtMeetingDate,
                            meeting_ref = dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString(),
                            Attendees = (dsMeetingList.Tables[0].Rows[0]["Attendees"].ToString()),
                            agenda_id = objMeetingAgendaModels.GetMeetingAgendaNameById(dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString()),
                            Venue = dsMeetingList.Tables[0].Rows[0]["Venue"].ToString(),
                            AttendeesUser = objGlobaldata.GetMultiHrEmpNameById(dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString()),
                            ext_attendees = dsMeetingList.Tables[0].Rows[0]["ext_attendees"].ToString(),
                            ext_email = dsMeetingList.Tables[0].Rows[0]["ext_email"].ToString(),
                        };
                        ViewBag.MeetingType = dsMeetingList.Tables[0].Rows[0]["meeting_type_desc"].ToString();
                        ViewBag.AgendaList = objMeetingAgendaModels.GetOnlyMeetingAgendasListbox(sMeeting_ref, dsMeetingList.Tables[0].Rows[0]["agenda_id"].ToString());
                        return View(objMeetingModels);
                    }
                }
                else if (Request.QueryString["immediate"] != null && Request.QueryString["immediate"] == "1")
                {
                    return View();
                }
                else
                {
                    TempData["alertdata"] = "Meeting Ref cannot be Null or empty";
                    return RedirectToAction("MeetingList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitializeAddMOM: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return RedirectToAction("MeetingList");
        }

        //
        // GET: /Meeting/AddMOM
        
        [AllowAnonymous]
        public JsonResult GetAgendaList(string sMeetingTypeId)
        {
            MeetingAgendaModels objAgenda = new MeetingAgendaModels();
            return Json(objAgenda.GetMeetingAgendaListbox(sMeetingTypeId));
        }

        //
        // GET: /Meeting/AddMOM
        
        [AllowAnonymous]
        public JsonResult GetPreviousMeetingRef(string sMeetingTypeId)
        {
            string sqlstmt = "select meeting_id, meeting_ref from t_meeting where meeting_type_desc='" + sMeetingTypeId + "' order by meeting_id desc limit 1";

            DataSet dsAgenda = objGlobaldata.Getdetails(sqlstmt);
            if (dsAgenda.Tables[0].Rows.Count > 0)
            {
                return Json( dsAgenda.Tables[0].Rows[0]["meeting_ref"].ToString());
            }

            return Json("");
        }

        
        [HttpPost]
        public JsonResult doesMeetingRefExist(string meeting_ref)
        {
            MeetingModels objMeeting = new MeetingModels();

            var user = objMeeting.checkMeetingRefExists(meeting_ref);

            return Json(user);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMOM(MeetingModels objMeeting, FormCollection form, MeetingItemModels MeetingItem, HttpPostedFileBase Attendees)
        {
            ViewBag.SubMenutype = "MOM";
            string sendmail = Request.Form["selectall"];
            try
            {
                objMeeting.meeting_type_desc = form["meeting_type_desc"];
                objMeeting.last_meeting_id = form["last_meeting_id"];
                objMeeting.meeting_ref = form["meeting_ref"];
              
                objMeeting.MeetingCreatedBy = objGlobaldata.GetCurrentUserSession().empid;
                
                //objMeeting.agenda_id = form["agenda_id"];

                if (Attendees != null && Attendees.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Meeting"), Path.GetFileName(Attendees.FileName));
                        string sFilename = objMeeting.meeting_type_desc + "Attendees_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath),
                        sFilepath = Path.GetDirectoryName(spath);
                        Attendees.SaveAs(sFilepath + "/" + sFilename);
                        objMeeting.Attendees = "~/DataUpload/MgmtDocs/Meeting/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                DateTime dateValue;

                if (form["meeting_date"] != null && DateTime.TryParse(form["meeting_date"], out dateValue) == true)
                {
                    objMeeting.meeting_date = dateValue;
                }

                MeetingItemModelsList objMeetingItemList = new MeetingItemModelsList();
                objMeetingItemList.MeetingItemMList = new List<MeetingItemModels>();

                List<string> lstAgendaId = new List<string>();

                if (form["itemcnt"] != null && Convert.ToInt16(form["itemcnt"]) > 0)
                {
                    string sitem_discussed = "", saction_agreed = "", saction_owner = "", sitem_status = "", sAgendaId = "",sremarks ="",supload="";

                    for (int i = 1; i <= Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["item_discussed" + i] != null)
                        {
                            sitem_discussed = form["item_discussed" + i].Replace("'/", "");
                        }

                        if (form["action_agreed" + i] != null)
                        {
                            saction_agreed = form["action_agreed" + i].Replace("'/", "");
                        }

                        if (form["action_owner" + i] != null)
                        {
                            saction_owner = form["action_owner" + i].Replace("'/", "");
                        }
                        if (form["remarks" + i] != null)
                        {
                            sremarks = form["remarks" + i].Replace("'/", "");
                        }
                        if (form["item_status" + i] != null)
                        {
                            sitem_status = form["item_status" + i].Replace("'/", "");
                        }
                        if (form["upload" + i] != null && form["upload" + i] != "")
                        {
                            supload = form["upload" + i];
                        }

                        if (form["CurrentAgendaId" + i] != null && !lstAgendaId.Contains(form["CurrentAgendaId" + i].Replace("'/", "")))
                        {
                            lstAgendaId.Add(form["CurrentAgendaId" + i].Replace("'/", ""));
                            sAgendaId = form["CurrentAgendaId" + i].Replace("'/", "");
                        }

                        MeetingItemModels objItem = new MeetingItemModels
                        {
                            item_discussed = sitem_discussed,
                            action_agreed = saction_agreed,
                            action_owner = saction_owner,
                            remarks = sremarks,
                            item_status = sitem_status,
                            agenda_id = sAgendaId,
                            Meeting_Ref = objMeeting.meeting_ref,
                            upload=supload
                        };

                        if (form["due_date" + i] != null && DateTime.TryParse(form["due_date" + i].Replace("'/", ""), out dateValue) == true)
                        {
                            objItem.due_date = dateValue;
                        }

                        objMeetingItemList.MeetingItemMList.Add(objItem);                       
                    }
                }

                //if (objMeeting.FunAddMeeting(objMeeting, objMeetingItemList, lstAgendaId))
                if (objMeeting.FunAddMeeting(objMeeting, objMeetingItemList, lstAgendaId))
                
                {
                    TempData["Successdata"] = "Added Meeting details successfully";
                    //if (sendmail != null && sendmail != "")
                    //{
                        //Send MOM email
                        MeetingItem.SendMOMEmail(objMeeting.meeting_ref, "Minutes of Meeting");
                    //}
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddMOM: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MeetingList");
        }
                              
        [AllowAnonymous]
        public JsonResult MeetingElementUpdate(string id_element, string Agenda_Desc, string Agenda_Details)
        {

            MeetingAgendaModels objMeetingAgenda = new MeetingAgendaModels();
            objMeetingAgenda.Agenda_Id = id_element;
            objMeetingAgenda.Agenda_Desc = Agenda_Desc;
            objMeetingAgenda.Agenda_Details = Agenda_Details;
            try
            {

           
            if (objMeetingAgenda.FunUpdateMeetingAgenda(objMeetingAgenda))
            {
                TempData["Successdata"] = "Updated Agenda details successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingElementUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }
                          
        [AllowAnonymous]
        public JsonResult MeetingElementDelete(string id_element, string MeetingType)
        {

            MeetingAgendaModels objMeetingAgenda = new MeetingAgendaModels();
            objMeetingAgenda.Agenda_Id = id_element;
            objMeetingAgenda.MeetingType = MeetingType;
            try
            {


                if (objMeetingAgenda.FunDeleteMeetingAgenda(objMeetingAgenda))
                {
                    TempData["Successdata"] = "Updated Agenda deleted successfully";

                   

                    //AddMeetingType(objMeetingAgenda.MeetingType);
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MeetingElementDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }

        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase Action_Plan1 = Request.Files;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Meeting"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/Meeting/" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);

            }
            return Json("");//Failed return null value
        }

        // Update Action Status
        [AllowAnonymous]
        public ActionResult StatusUpdate()
        {
            MeetingModels objModel = new MeetingModels();
            MeetingAgendaModels objMeetingAgendaModels = new MeetingAgendaModels();
            try
            {

                if (Request.QueryString["meeting_id"] != null && Request.QueryString["meeting_id"] != "")
                {
                    string meeting_id = Request.QueryString["meeting_id"];

                    string sSqlstmt = "select meeting_id,meeting_ref,Agenda_Id,action_status,status_update_on from t_meeting where meeting_id='" + meeting_id + "'";

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objModel = new MeetingModels
                        {
                            meeting_id = (dsModelsList.Tables[0].Rows[0]["meeting_id"].ToString()),
                            meeting_ref = (dsModelsList.Tables[0].Rows[0]["meeting_ref"].ToString()),
                            agenda_id = objMeetingAgendaModels.GetOnlyMeetingAgendaById(dsModelsList.Tables[0].Rows[0]["Agenda_Id"].ToString()),
                            action_status = dsModelsList.Tables[0].Rows[0]["action_status"].ToString(),
                        };
                      
                        DateTime dtValue;
                        if (DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["status_update_on"].ToString(), out dtValue))
                        {
                            objModel.status_update_on = dtValue;
                        }

                        string sql = "select * from t_meeting_items where meeting_ref='" + objModel.meeting_ref + "'";
                        ViewBag.items = objGlobaldata.Getdetails(sql);
                    }
                    ViewBag.Status = objGlobaldata.GetDropdownList("Meeting Item Status");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in StatusUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult StatusUpdate(FormCollection form, MeetingModels obj)
        {
            try
            {
                DateTime dateValue;
            

                if (DateTime.TryParse(form["status_update_on"], out dateValue) == true)
                {
                    obj.status_update_on = dateValue;
                }
                if (obj.FunUpdateStatus(obj))
                {
                    TempData["Successdata"] = "Status updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in StatusUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("MeetingList");
        }
    }
}
