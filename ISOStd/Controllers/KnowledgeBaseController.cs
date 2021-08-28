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
using Microsoft.Reporting.WebForms;
using System.Web.Mvc.Html;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class KnowledgeBaseController : Controller
    {

        clsGlobal objGlobaldata = new clsGlobal();

        static List<string> objLeaveList = new List<string>();

        public KnowledgeBaseController()
        {
            ViewBag.Menutype = "KnowledgeBase";
        }

        //public ActionResult Index()
        //{
        //    return View();
        //}
         
        [AllowAnonymous]
        public ActionResult AddKnowledgeBase()
        {
            try
            {
              
                    ViewBag.currentuser = objGlobaldata.GetCurrentUserSession().firstname;
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddKnowledgeBase: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return InitilizeAddKnowledgeBase();
        }

         
        private ActionResult InitilizeAddKnowledgeBase()
        {
            KnowledgeBaseModels obj = new KnowledgeBaseModels();
            try
            {
               
                    ViewBag.applied_date = objGlobaldata.GetCurrentUserSession().ToString();
                
                //  obj.deptid = objGlobaldata.GetCurrentUserSession().DeptID;

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddKnowledgeBase: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(obj);
        }



         
        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult AddKnowledgeBase(KnowledgeBaseModels objKnowledgeModel, FormCollection form, HttpPostedFileBase Evidence)
        {


            try
            {
                if (objKnowledgeModel != null)
                {
                    
                        objKnowledgeModel.uploaded_by = objGlobaldata.GetCurrentUserSession().empid;
                    
                    objKnowledgeModel.topic = form["topic"];
                    objKnowledgeModel.details = form["details"];



                    if (Evidence != null && Evidence.ContentLength > 0)
                    {

                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/KnowledgeBase"), Path.GetFileName(Evidence.FileName));
                            string sFilename = "Evidence" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);
                            Evidence.SaveAs(sFilepath + "/" + sFilename);
                            objKnowledgeModel.Evidence = "~/DataUpload/MgmtDocs/KnowledgeBase/" + sFilename;
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



                }
                if (objKnowledgeModel.FunAddKnowledge(objKnowledgeModel))
                {
                    TempData["Successdata"] = "Added Knowledge Base  successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddKnowledgeBase: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("DisplayKnowledgeBase");
        }



        //=================================

      
        [AllowAnonymous]
        public ActionResult DisplayKnowledgeBase(KnowledgeBaseModels objKnowledgeBase , FormCollection form,  int? page ,string SearchText)

        {
            KnowledgeModelsList objKnowledgeModelsList = new KnowledgeModelsList();
            objKnowledgeModelsList.KnowledgeMList = new List<KnowledgeBaseModels>();
         
            try
            {
                //if (Request.QueryString["id_knowledge_base"] != null)
                //{
                    string sid_knowledge_base = Request.QueryString["id_knowledge_base"];
                    string sSqlstmt = "select  id_knowledge_base ,topic, details, Evidence ,uploaded_by from t_knowledge_base ";
                     string sSearchtext = "";


                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = " (topic ='" + SearchText + "' or topic like '%" + SearchText + "%' or id_knowledge_base ='" + SearchText + "' or id_knowledge_base like '" + SearchText + "%' ) ";



                    sSqlstmt = sSqlstmt + " where " + sSearchtext + " order by topic asc";

                }

                DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                    for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                    {

                        try
                        {
                            KnowledgeBaseModels objKnowledgeBaseModels = new KnowledgeBaseModels
                            {
                                id_knowledge_base = Convert.ToInt16( dsObjectivesModelsList.Tables[0].Rows[i]["id_knowledge_base"].ToString()),
                                topic = dsObjectivesModelsList.Tables[0].Rows[i]["topic"].ToString(),
                                details = dsObjectivesModelsList.Tables[0].Rows[i]["details"].ToString(),
                                Evidence = dsObjectivesModelsList.Tables[0].Rows[i]["Evidence"].ToString(),
                                uploaded_by = dsObjectivesModelsList.Tables[0].Rows[i]["uploaded_by"].ToString()

                            };
                            objKnowledgeModelsList.KnowledgeMList.Add(objKnowledgeBaseModels);
                        }
                        catch (Exception ex)
                        { }
                    }
                    }
                //}
                //else
                //{
                //    TempData["alertdata"] = "KnowledgeBaseList Trans Id cannot be null";
                //    return RedirectToAction("KnowledgeBaseList");
                //}
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DisplayKnowledgeBase: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objKnowledgeModelsList.KnowledgeMList.ToList().ToPagedList(page ?? 1, 40));
        }



        //================================  

         
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult KnowledgeBaseEdit(int? page)
        {

            KnowledgeModelsList objKnowledgeModelsList = new KnowledgeModelsList();
            objKnowledgeModelsList.KnowledgeMList = new List<KnowledgeBaseModels>();


            //KnowledgeModelsList objKnowledgeModelsList = new KnowledgeModelsList();
            //objKnowledgeModelsList.KnowledgeMList = new List<KnowledgeBaseModels>();

            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();

                if (Request.QueryString["id_knowledge_base"] != null)
                {
                    string sid_knowledge_base = Request.QueryString["id_knowledge_base"];

                    string sSqlstmt = "select id_knowledge_base,   topic, details,Evidence "
                    + "  from t_knowledge_base   where id_knowledge_base='" + sid_knowledge_base + "'";

                    DataSet dsKnowledgeModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsKnowledgeModelsList.Tables.Count > 0 && dsKnowledgeModelsList.Tables[0].Rows.Count > 0)
                    {


                        KnowledgeBaseModels objKnowledgeBaseModels = new KnowledgeBaseModels
                        {
                                id_knowledge_base = Convert.ToInt16(dsKnowledgeModelsList.Tables[0].Rows[0]["id_knowledge_base"].ToString()),
                                topic = dsKnowledgeModelsList.Tables[0].Rows[0]["topic"].ToString(),
                                details = dsKnowledgeModelsList.Tables[0].Rows[0]["details"].ToString(),
                                Evidence = dsKnowledgeModelsList.Tables[0].Rows[0]["Evidence"].ToString()
                            };
                        //objKnowledgeBaseModels.KnowledgeMList.Add(objKnowledgeBaseModels);
                        return View(objKnowledgeBaseModels);
                    }
                    else
                    {
                        ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("DisplayKnowledgeBase");

                    }
                }
                else
                {

        
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KnowledgeBaseEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objKnowledgeModelsList.KnowledgeMList.ToList().ToPagedList(page ?? 1, 40));

            //return RedirectToAction("DisplayKnowledgeBase");
        }


         
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult KnowledgeBaseEdit(KnowledgeBaseModels objKnowledgeBaseModels, FormCollection form, HttpPostedFileBase Evidence ,int? page)
        {
            try
            {

                objKnowledgeBaseModels.id_knowledge_base = Convert.ToInt16(form["id_knowledge_base"]);
                objKnowledgeBaseModels.topic = form["topic"];
                objKnowledgeBaseModels.details = form["details"];




                if (Evidence != null && Evidence.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/KnowledgeBase"), Path.GetFileName(Evidence.FileName));
                        string sFilename = "Evidence" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Evidence.SaveAs(sFilepath + "/" + sFilename);
                        objKnowledgeBaseModels.Evidence = "~/DataUpload/MgmtDocs/KnowledgeBase/" + sFilename;
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

                if (objKnowledgeBaseModels.FunUpdateKnowledgeBase(objKnowledgeBaseModels))
                {
                    TempData["Successdata"] = "Knowledge Base  updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
           }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in KnowledgeBaseEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
     

            //return View(objKnowledgeModelsList.KnowledgeMList.ToList().ToPagedList(page ?? 1, 40));
            return RedirectToAction("DisplayKnowledgeAdmin", new { id_knowledge_base = objKnowledgeBaseModels.id_knowledge_base });
        }
        //=======================================================================

         
        [AllowAnonymous]
        public ActionResult DisplayKnowledgeAdmin(KnowledgeBaseModels objKnowledgeBase, FormCollection form, string SearchText, int? page)

        {
            KnowledgeModelsList objKnowledgeModelsList = new KnowledgeModelsList();
            objKnowledgeModelsList.KnowledgeMList = new List<KnowledgeBaseModels>();

            try
            {
                //if (Request.QueryString["id_knowledge_base"] != null)
                //{
                string sid_knowledge_base = Request.QueryString["id_knowledge_base"];
                string sSqlstmt = "select  id_knowledge_base ,topic, details, Evidence ,uploaded_by from t_knowledge_base ";
                string sSearchtext = "";


                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = " (topic ='" + SearchText + "' or topic like '%" + SearchText + "%' or id_knowledge_base ='" + SearchText + "' or id_knowledge_base like '" + SearchText + "%' ) ";



                    sSqlstmt = sSqlstmt + " where " + sSearchtext + " order by topic asc";

                }

                DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                    {

                        try
                        {
                            KnowledgeBaseModels objKnowledgeBaseModels = new KnowledgeBaseModels
                            {
                                id_knowledge_base = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["id_knowledge_base"].ToString()),
                                topic = dsObjectivesModelsList.Tables[0].Rows[i]["topic"].ToString(),
                                details = dsObjectivesModelsList.Tables[0].Rows[i]["details"].ToString(),
                                Evidence = dsObjectivesModelsList.Tables[0].Rows[i]["Evidence"].ToString(),
                                uploaded_by = dsObjectivesModelsList.Tables[0].Rows[i]["uploaded_by"].ToString()
                            };
                            objKnowledgeModelsList.KnowledgeMList.Add(objKnowledgeBaseModels);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                //}
                //else
                //{
                //    TempData["alertdata"] = "KnowledgeBaseList Trans Id cannot be null";
                //    return RedirectToAction("KnowledgeBaseList");
                //}
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DisplayKnowledgeAdmin: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objKnowledgeModelsList.KnowledgeMList.ToList().ToPagedList(page ?? 1, 40));
        }


    }
}