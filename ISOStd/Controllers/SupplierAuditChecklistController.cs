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
    public class SupplierAuditChecklistController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public ActionResult GenerateAuditChecklist()
        {

            SupplierAuditElementsModels obj = new SupplierAuditElementsModels();

            try
            {
                ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                ViewBag.Business = objGlobaldata.GetDropdownList("Business Nature");
                ViewBag.Supplier = objGlobaldata.GetSupplierList();
                ViewBag.AuditCriteria = objGlobaldata.GetAuditCriteria();
                string sSqlstmt = "Select Itemno from t_supplier_auditchecklist";
                DataSet dsChecklist = objGlobaldata.Getdetails(sSqlstmt);
                if (dsChecklist.Tables.Count > 0 && dsChecklist.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        string sSSqlstmt = "select Itemno from t_supplier_auditchecklist order by id_AuditChecklist Desc limit 1";
                        DataSet dschecklist = objGlobaldata.Getdetails(sSSqlstmt);
                        if (dschecklist.Tables.Count > 0 && dschecklist.Tables[0].Rows.Count > 0)
                        {
                            int ItemNo = Convert.ToInt32(dschecklist.Tables[0].Rows[0]["Itemno"].ToString());
                            ItemNo = ItemNo + 1;
                            ViewBag.ItemNo = ItemNo;
                            Session["ItemNo"] = ViewBag.ItemNo;
                        }

                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in GenerateAuditChecklist: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }

                }
                else
                {
                    ViewBag.ItemNo = "1";
                    Session["ItemNo"] = ViewBag.ItemNo;
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateAuditChecklist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

         
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateAuditChecklist(FormCollection form, SupplierAuditChecklistModels objChecklist)
        {
            try
            {
                objChecklist.Itemno = Convert.ToInt32(Session["ItemNo"]);
                objChecklist.Business = form["Business"];
                objChecklist.AuditCriteria = form["AuditCriteria"];
                string quest = "";
                int cnt = Convert.ToInt16(form["itemcnt"]);
                for (int i = 0; i < cnt; i++)
                {
                    quest = quest + form["Audit_Elements " + i] + ",";


                }
                objChecklist.Questions = quest.TrimEnd(',');
                if (objChecklist.FunAddAuditChecklist(objChecklist))
                {
                    TempData["Successdata"] = "Audit checklist Generated successfully";

                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateAuditChecklist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditChecklistList");
        }

         
        [AllowAnonymous]
        public ActionResult AuditChecklistList(string SearchText, string ChangeIn, string Approvestatus, int? page)
        {
            SupplierAuditChecklistModelsList objAuditChecklist = new SupplierAuditChecklistModelsList();
            objAuditChecklist.AuditCheckList = new List<SupplierAuditChecklistModels>();

            try
            {
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select id_AuditChecklist,Itemno,Business,Department,Supplier,AuditCriteria from t_supplier_auditchecklist where Active=1";


                DataSet dsChecklistModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsChecklistModelsList.Tables.Count > 0 && dsChecklistModelsList.Tables[0].Rows.Count > 0)
                {
                   
                       
                    for (int i = 0; i < dsChecklistModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            SupplierAuditChecklistModels objChecklist = new SupplierAuditChecklistModels
                            {
                                id_AuditChecklist = Convert.ToInt16(dsChecklistModelsList.Tables[0].Rows[i]["id_AuditChecklist"].ToString()),
                                Itemno = Convert.ToInt16(dsChecklistModelsList.Tables[0].Rows[i]["Itemno"].ToString()),
                                Business = objGlobaldata.GetDropdownitemById(dsChecklistModelsList.Tables[0].Rows[i]["Business"].ToString()),
                                AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsChecklistModelsList.Tables[0].Rows[i]["AuditCriteria"].ToString()),
                                Department = objGlobaldata.GetDeptNameById(dsChecklistModelsList.Tables[0].Rows[i]["Department"].ToString()),
                                Supplier = objGlobaldata.GetSupplierNameById(dsChecklistModelsList.Tables[0].Rows[i]["Supplier"].ToString()),
                            };
                            objAuditChecklist.AuditCheckList.Add(objChecklist);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditChecklistList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditChecklistList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objAuditChecklist.AuditCheckList.ToList().ToPagedList(page ?? 1, 40));
        }

         
        public ActionResult AddAuditElements(string sBusiness, string sDepartment)
        {
            try
            {
                SupplierAuditElementsModels obj = new SupplierAuditElementsModels();
                ViewBag.Business = objGlobaldata.GetDropdownList("Business Nature");
                ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                if (sBusiness != null && sBusiness != "")
                {
                    ViewBag.Business_Id = sBusiness;
                    ViewBag.AuditElements = obj.GetAuditElementsListbox(sBusiness, sDepartment);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditElements: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAuditElements(SupplierAuditElementsModels objElementModels, FormCollection form)
        {
            SupplierAuditElementsModels obj = new SupplierAuditElementsModels();
            try
            {
                ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                ViewBag.Department_Id = objElementModels.Department;
                ViewBag.Business_Id = objElementModels.Business;
                ViewBag.Business = objGlobaldata.GetDropdownList("Business Nature");
                if (objElementModels.FunAddAuditElements(objElementModels))
                {
                    TempData["Successdata"] = "Added Questions added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                ViewBag.AuditElements = obj.GetAuditElementsListbox(objElementModels.Business, objElementModels.Department);

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditElements: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objElementModels);
        }

         
        [AllowAnonymous]
        public JsonResult AuditElementUpdate(int id_element, string Audit_Elements)
        {
            SupplierAuditElementsModels obj = new SupplierAuditElementsModels();
            try
            {
                if (obj.FunUpdateAuditElements(id_element, Audit_Elements))
                {
                    TempData["Successdata"] = "Audit Questions updated successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditElementUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }

         
        [HttpPost]
        public JsonResult doesElementExist(string Audit_Elements)
        {

            SupplierAuditElementsModels obj = new SupplierAuditElementsModels();
            var exists = obj.checkAuditElementExists(Audit_Elements);
            return Json(exists);
        }

         
        [AllowAnonymous]
        public JsonResult AuditChecklistDocDelete(FormCollection form)
        {
            try
            {

                        if (form["id_AuditChecklist"] != null && form["id_AuditChecklist"] != "")
                        {
                            SupplierAuditChecklistModels Doc = new SupplierAuditChecklistModels();
                            string sid_AuditChecklist = form["id_AuditChecklist"];


                            if (Doc.FunDeleteAuditChecklist(sid_AuditChecklist))
                            {
                                TempData["Successdata"] = "Checklist deleted successfully";
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
                            TempData["alertdata"] = "Audit checklist Id cannot be Null or empty";
                            return Json("Failed");
                        }
                    
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditChecklistDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

         
        public ActionResult AuditQuestionDelete(string id_element, string Business, string Department)
        {
            SupplierAuditElementsModels obj = new SupplierAuditElementsModels();
            try
            {
                if (obj.FunDeleteQuestions(id_element))
                {
                    TempData["Successdata"] = "Question deleted successfully";
                    return RedirectToAction("AddAuditElements", new { sBusiness = Business });
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditQuestionDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AddAuditElements", new { sBusiness = Business, sDepartment = Department });
        }

         
        public JsonResult AuditQuestionDelete1(string id_element, string Business, string Department)
        {

            SupplierAuditElementsModels obj = new SupplierAuditElementsModels();
            try
            {

                if (obj.FunDeleteQuestions(id_element))
                {
                    TempData["Successdata"] = "Question deleted successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditQuestionDelete1: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }

         
        public ActionResult PerformAudit()
        {
            SupplierAuditChecklistModels objAuditChecklistModels = new SupplierAuditChecklistModels();

            try
            {
                ViewBag.Finding = objGlobaldata.GetDropdownList("Audit Finding Category");
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                SupplierAuditElementsModels obj = new SupplierAuditElementsModels();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                ViewBag.AuditRating = obj.GetAuditRating();
                ViewBag.Business = objGlobaldata.GetDropdownList("Business Nature");
                if (Request.QueryString["id_AuditChecklist"] != null && Request.QueryString["id_AuditChecklist"] != "")
                {
                    string sid_AuditChecklist = Request.QueryString["id_AuditChecklist"];


                    string sSqlstmt = "select id_AuditChecklist,Itemno,Business,Department,Supplier,AuditCriteria,Questions from t_supplier_auditchecklist where id_AuditChecklist='" + sid_AuditChecklist + "'";

                    DataSet dsChecklistModelsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsChecklistModelsList.Tables.Count > 0 && dsChecklistModelsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsChecklistModelsList.Tables[0].Rows.Count; i++)
                        {

                            objAuditChecklistModels = new SupplierAuditChecklistModels
                            {
                                id_AuditChecklist = Convert.ToInt16(dsChecklistModelsList.Tables[0].Rows[i]["id_AuditChecklist"].ToString()),
                                Itemno = Convert.ToInt16(dsChecklistModelsList.Tables[0].Rows[i]["Itemno"].ToString()),
                                Business = objGlobaldata.GetDropdownitemById(dsChecklistModelsList.Tables[0].Rows[i]["Business"].ToString()),
                                AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsChecklistModelsList.Tables[0].Rows[i]["AuditCriteria"].ToString()),
                                Questions = dsChecklistModelsList.Tables[0].Rows[i]["Questions"].ToString(),
                                Department = objGlobaldata.GetDeptNameById(dsChecklistModelsList.Tables[0].Rows[i]["Department"].ToString()),
                                Supplier = objGlobaldata.GetSupplierNameById(dsChecklistModelsList.Tables[0].Rows[i]["Supplier"].ToString()),

                            };
                            ViewBag.AuditElements = obj.GetAuditElementsListbox(dsChecklistModelsList.Tables[0].Rows[i]["Business"].ToString(), dsChecklistModelsList.Tables[0].Rows[i]["Department"].ToString());
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("AuditChecklistList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("AuditChecklistList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAuditChecklistModels);
        }

         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerformAudit(SupplierAuditChecklistModels objAuditChecklist, FormCollection form)
        {
            try
            {
                objAuditChecklist.id_AuditChecklist = Convert.ToInt32(form["id_AuditChecklist"]);
                objAuditChecklist.AuditNo = form["AuditNo"];
                objAuditChecklist.Auditors = form["Auditors"];
                objAuditChecklist.Auditee = form["Auditee"];
                objAuditChecklist.Notes = form["Notes"];
                objAuditChecklist.Remarks = form["Remarks"];
                objAuditChecklist.Business = form["Business"];

                //string dept = "";
                //string sSqlstmt = "select Business from t_supplier_generateauditchecklist where idt_checklist='" + objAuditChecklist.idt_checklist + "'";
                //DataSet dsChecklistModelsList = objGlobaldata.Getdetails(sSqlstmt);
                //if (dsChecklistModelsList.Tables.Count > 0 && dsChecklistModelsList.Tables[0].Rows.Count > 0)
                //{
                //    for (int j = 0; j < dsChecklistModelsList.Tables[0].Rows.Count; j++)
                //    {

                //        //SupplierAuditChecklistModels objAuditChecklistModels = new SupplierAuditChecklistModels
                //        //{
                //        //   Business = dsChecklistModelsList.Tables[0].Rows[i]["Business"].ToString(),
                //        //};        
                //        dept = dsChecklistModelsList.Tables[0].Rows[j]["Business"].ToString();
                //    }
                //}


                DateTime dateValue;
                if (DateTime.TryParse(form["AuditDate"], out dateValue) == true)
                {
                    objAuditChecklist.AuditDate = dateValue;
                }

                SupplierAuditPerformanceModelsList objAudit = new SupplierAuditPerformanceModelsList();
                objAudit.lstAudit = new List<SupplierAuditPerformanceModels>();

                SupplierAuditElementsModels obj = new SupplierAuditElementsModels();
                MultiSelectList AuditQuestions = obj.GetAuditElementsListbox(objAuditChecklist.Business, objAuditChecklist.Department);
                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in AuditQuestions)
                {
                    if (i <= Convert.ToInt16(form["itmctn"]))
                    {
                        SupplierAuditPerformanceModels objElements = new SupplierAuditPerformanceModels();
                        objElements.id_element = form["Audit_Elements " + item.Value];
                        objElements.id_auditratings = form["id_auditratings " + item.Value];
                        objElements.comment = form["comment " + i];
                        objElements.evidence_upload = form["evidence_upload" + i];
                        objElements.finding_cat = form["finding_cat " + i];
                        objAudit.lstAudit.Add(objElements);
                    }
                    i++;
                }


                if (objAuditChecklist.FunAddAuditPerformance(objAuditChecklist, objAudit))
                {
                    TempData["Successdata"] = "Audit Performance details added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditChecklistList");
        }

         
        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase Evidence_upload = Request.Files;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "Evidence" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/" + "Evidence" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);


            }

            return Json("Failed");
        }

         
        [HttpPost]
        public ActionResult AuditChecklistReport(FormCollection form)
        {
            string sidt_checklist = form["idt_checklist"];
            try
            {
                if (sidt_checklist != "")
                {

                    string sSqlstmt = "select t.id_AuditChecklist,idt_checklist,Itemno,Business,Department,Supplier,AuditCriteria,AuditNo,AuditDate,"
                    + "Auditors,Auditee,Notes,Remarks from t_supplier_auditchecklist t,t_supplier_generateauditchecklist tt where t.id_AuditChecklist=tt.id_AuditChecklist and idt_checklist='" + sidt_checklist + "'";


                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        SupplierAuditChecklistModels objAudit = new SupplierAuditChecklistModels
                        {
                            idt_checklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["idt_checklist"].ToString()),
                            Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
                            Business = objGlobaldata.GetDropdownitemById(dsAudit.Tables[0].Rows[0]["Business"].ToString()),
                            AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                            AuditNo = (dsAudit.Tables[0].Rows[0]["AuditNo"].ToString()),
                            Auditors = dsAudit.Tables[0].Rows[0]["Auditors"].ToString(),
                            Auditee = (dsAudit.Tables[0].Rows[0]["Auditee"].ToString()),
                            Notes = dsAudit.Tables[0].Rows[0]["Notes"].ToString(),
                            Remarks = dsAudit.Tables[0].Rows[0]["Remarks"].ToString(),
                            Department = objGlobaldata.GetDeptNameById(dsAudit.Tables[0].Rows[0]["Department"].ToString()),
                            Supplier = objGlobaldata.GetSupplierNameById(dsAudit.Tables[0].Rows[0]["Supplier"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAudit.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                        {
                            objAudit.AuditDate = dtValue;
                        }
                        ViewBag.ChecklistDetails = objAudit;

                        sSqlstmt = "SELECT id_AuditPerformanceChecklist,idt_checklist,id_element,id_auditratings,finding_cat,"
                        + "comment FROM t_supplier_auditperformancechecklist where idt_checklist='"
                            + objAudit.idt_checklist + "'";
                        DataSet dsAuditElement = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.AuditElement = dsAuditElement;
                        SupplierAuditElementsModels obj = new SupplierAuditElementsModels();



                        ViewBag.AuditRating = obj.GetAuditRating();
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("AuditChecklistList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("AuditChecklistList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditChecklistReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("AuditChecklistReportToPDF")
            {
                FileName = "AuditChecklistReport.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };


        }

         
        [AllowAnonymous]
        public ActionResult AuditChecklistDetails()
        {

            try
            {
                if (Request.QueryString["idt_checklist"] != null && Request.QueryString["idt_checklist"] != "")
                {
                    string sidt_checklist = Request.QueryString["idt_checklist"];

                    string sSqlstmt = "select idt_checklist,t.id_AuditChecklist,Itemno,Business,Department,Supplier,AuditCriteria,AuditNo,AuditDate,"
                    + "Auditors,Auditee,Notes,Remarks,Questions from t_supplier_generateauditchecklist t,t_supplier_auditchecklist tt where t.id_AuditChecklist=tt.id_AuditChecklist and idt_checklist='" + sidt_checklist + "'";


                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        SupplierAuditChecklistModels objAudit = new SupplierAuditChecklistModels
                        {
                            idt_checklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["idt_checklist"].ToString()),
                            id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["id_AuditChecklist"].ToString()),
                            Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
                            Business = objGlobaldata.GetDropdownitemById(dsAudit.Tables[0].Rows[0]["Business"].ToString()),
                            AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                            AuditNo = (dsAudit.Tables[0].Rows[0]["AuditNo"].ToString()),
                            Auditors = dsAudit.Tables[0].Rows[0]["Auditors"].ToString(),
                            Auditee = (dsAudit.Tables[0].Rows[0]["Auditee"].ToString()),
                            Notes = dsAudit.Tables[0].Rows[0]["Notes"].ToString(),
                            Remarks = dsAudit.Tables[0].Rows[0]["Remarks"].ToString(),
                            Department = objGlobaldata.GetDeptNameById(dsAudit.Tables[0].Rows[0]["Department"].ToString()),
                            Supplier = objGlobaldata.GetSupplierNameById(dsAudit.Tables[0].Rows[0]["Supplier"].ToString()),

                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAudit.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                        {
                            objAudit.AuditDate = dtValue;
                        }

                        sSqlstmt = "SELECT id_AuditPerformanceChecklist,idt_checklist,id_element,id_auditratings,"
                       + "comment,evidence_upload,finding_cat FROM t_supplier_auditperformancechecklist where idt_checklist='"
                           + objAudit.idt_checklist + "'";

                        DataSet dsAuditElement = objGlobaldata.Getdetails(sSqlstmt);


                        SupplierAuditPerformanceModelsList objAuditPerformanceList = new SupplierAuditPerformanceModelsList();
                        objAuditPerformanceList.lstAudit = new List<SupplierAuditPerformanceModels>();

                        SupplierAuditElementsModels obj = new SupplierAuditElementsModels();

                        for (int i = 0; dsAuditElement.Tables.Count > 0 && i < dsAuditElement.Tables[0].Rows.Count; i++)
                        {
                            SupplierAuditPerformanceModels objElements = new SupplierAuditPerformanceModels
                            {

                                id_element = obj.GetAuditQuestionNameById(dsAuditElement.Tables[0].Rows[i]["id_element"].ToString()),
                                id_auditratings = obj.GetAuditRatingNameById(dsAuditElement.Tables[0].Rows[i]["id_auditratings"].ToString()),
                                comment = dsAuditElement.Tables[0].Rows[i]["comment"].ToString(),
                                evidence_upload = dsAuditElement.Tables[0].Rows[i]["evidence_upload"].ToString(),
                                finding_cat = objGlobaldata.GetDropdownitemById(dsAuditElement.Tables[0].Rows[i]["finding_cat"].ToString()),
                            };
                            objAuditPerformanceList.lstAudit.Add(objElements);
                        }

                        ViewBag.AuditElement = objAuditPerformanceList;
                        ViewBag.AuditRating = obj.GetAuditRating();
                        return View(objAudit);

                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("AuditChecklistList");
                    }
                }
                else
                {
                    TempData["alertdata"] = " Id cannot be null";
                    return RedirectToAction("AuditChecklistList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditChecklistDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditChecklistList");
        }


         
        [AllowAnonymous]
        public ActionResult AuditChecklistEdit()
        {

            SupplierAuditElementsModels obj = new SupplierAuditElementsModels();
            SupplierAuditChecklistModels objAudit = new SupplierAuditChecklistModels();
            try
            {
                ViewBag.Business = objGlobaldata.GetDropdownList("Business Nature");
                ViewBag.AuditCriteria = objGlobaldata.GetAuditCriteria();
                if (Request.QueryString["id_AuditChecklist"] != null && Request.QueryString["id_AuditChecklist"] != "")
                {

                    string sid_AuditChecklist = Request.QueryString["id_AuditChecklist"];

                    string sSqlstmt = "select id_AuditChecklist,Itemno,Business,AuditCriteria,Questions"
                    + " from t_supplier_auditchecklist where id_AuditChecklist='" + sid_AuditChecklist + "'";

                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        objAudit = new SupplierAuditChecklistModels
                        {
                            id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["id_AuditChecklist"].ToString()),
                            Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
                            Business = objGlobaldata.GetDeptNameById(dsAudit.Tables[0].Rows[0]["Business"].ToString()),
                            AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                            Questions = dsAudit.Tables[0].Rows[0]["Questions"].ToString(),

                        };
                        ViewBag.AuditElements = obj.GetAuditElementsListbox(dsAudit.Tables[0].Rows[0]["Business"].ToString(), dsAudit.Tables[0].Rows[0]["Department"].ToString());
                    }
                    else
                    {

                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("");
                    }
                }
                else
                {

                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditChecklistEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("");
            }

            return View(objAudit);

        }

          
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public ActionResult AuditChecklistEdit(SupplierAuditChecklistModels objAudit, FormCollection form)
        {
            try
            {
                objAudit.Business = form["Business"];
                objAudit.AuditCriteria = form["AuditCriteria"];
                int cnt = Convert.ToInt16(form["itemcnt"]);
                string quest = "";
                for (int i = 0; i < cnt; i++)
                {
                    quest = quest + form["Audit_Elements " + i] + ",";

                    objAudit.Questions = quest;
                }


                if (objAudit.FunUpdateAuditChecklist(objAudit, cnt))
                {
                    TempData["Successdata"] = "Audit checklist details updated successfully";

                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditChecklistEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditChecklistList");
        }
          
        [HttpPost]
        public JsonResult GetQuestions(string Business,string Department)
        {
            SupplierAuditElementsModels obj = new SupplierAuditElementsModels();

            var data = obj.GetAuditElementsListbox(Business, Department);

            return Json(data);
        }
          
        [AllowAnonymous]
        public ActionResult AuditList(int? page)
        {
            SupplierAuditChecklistModelsList objAuditChecklist = new SupplierAuditChecklistModelsList();
            objAuditChecklist.AuditCheckList = new List<SupplierAuditChecklistModels>();

            try
            {

                if (Request.QueryString["id_AuditChecklist"] != null && Request.QueryString["id_AuditChecklist"] != "")
                {

                    string sid_AuditChecklist = Request.QueryString["id_AuditChecklist"];
                    string sSqlstmt = "select idt_checklist,id_AuditChecklist,AuditNo,AuditDate,Auditors,Auditee from t_supplier_generateauditchecklist where id_AuditChecklist='" + sid_AuditChecklist + "' and Active=1";

                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                       
                        
                        
                        for (int i = 0; i < dsAudit.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                SupplierAuditChecklistModels objAudit = new SupplierAuditChecklistModels
                                {
                                    idt_checklist = Convert.ToInt16(dsAudit.Tables[0].Rows[i]["idt_checklist"].ToString()),
                                    id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[i]["id_AuditChecklist"].ToString()),
                                    AuditNo = dsAudit.Tables[0].Rows[i]["AuditNo"].ToString(),
                                    Auditors = dsAudit.Tables[0].Rows[i]["Auditors"].ToString(),
                                    Auditee = (dsAudit.Tables[0].Rows[i]["Auditee"].ToString()),


                                };
                                DateTime dtValue;
                                if (DateTime.TryParse(dsAudit.Tables[0].Rows[i]["AuditDate"].ToString(), out dtValue))
                                {
                                    objAudit.AuditDate = dtValue;
                                }

                                objAuditChecklist.AuditCheckList.Add(objAudit);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AuditList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("AuditChecklistList");
                    }

                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("AuditChecklistList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objAuditChecklist.AuditCheckList.ToList().ToPagedList(page ?? 1, 40));
        }

          
        [AllowAnonymous]
        public ActionResult AuditPerformanceEdit()
        {

            try
            {
                if (Request.QueryString["idt_checklist"] != null && Request.QueryString["idt_checklist"] != "")
                {
                    string sidt_checklist = Request.QueryString["idt_checklist"];

                    string sSqlstmt = "select t.id_AuditChecklist,Itemno,Business,AuditCriteria,Questions,idt_checklist,AuditNo,"
                    + "AuditDate,Auditors,Auditee,Notes,Remarks from t_supplier_auditchecklist t,t_supplier_generateauditchecklist tt where t.id_AuditChecklist=tt.id_AuditChecklist"
                    + " and idt_checklist='" + sidt_checklist + "'";


                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        SupplierAuditChecklistModels objPerformance = new SupplierAuditChecklistModels
                        {
                            idt_checklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["idt_checklist"].ToString()),
                            id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["id_AuditChecklist"].ToString()),
                            Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
                            Business = objGlobaldata.GetDeptNameById(dsAudit.Tables[0].Rows[0]["Business"].ToString()),
                            AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                            AuditNo = objGlobaldata.GetAuditNoById(dsAudit.Tables[0].Rows[0]["AuditNo"].ToString()),
                            Auditors = dsAudit.Tables[0].Rows[0]["Auditors"].ToString(),
                            Auditee = objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["Auditee"].ToString()),
                            Notes = dsAudit.Tables[0].Rows[0]["Notes"].ToString(),
                            Remarks = dsAudit.Tables[0].Rows[0]["Remarks"].ToString(),
                        };
                        string dept = dsAudit.Tables[0].Rows[0]["Business"].ToString();
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAudit.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                        {
                            objPerformance.AuditDate = dtValue;
                        }

                        sSqlstmt = "SELECT id_AuditPerformanceChecklist,idt_checklist,id_element,id_auditratings,"
                       + "comment,evidence_upload FROM t_supplier_auditperformancechecklist where idt_checklist='"
                           + objPerformance.idt_checklist + "'";

                        DataSet dsAuditElement = objGlobaldata.Getdetails(sSqlstmt);

                        SupplierAuditPerformanceModelsList objAuditPerformanceList = new SupplierAuditPerformanceModelsList();
                        objAuditPerformanceList.lstAudit = new List<SupplierAuditPerformanceModels>();

                        SupplierAuditElementsModels obj = new SupplierAuditElementsModels();

                        Dictionary<string, string> dicPerformanceElements = new Dictionary<string, string>();

                        for (int i = 0; dsAuditElement.Tables.Count > 0 && i < dsAuditElement.Tables[0].Rows.Count; i++)
                        {
                            ViewBag.dsAudit = dsAuditElement;
                        }

                        for (int i = 0; dsAuditElement.Tables.Count > 0 && i < dsAuditElement.Tables[0].Rows.Count; i++)
                        {
                            SupplierAuditPerformanceModels objElements = new SupplierAuditPerformanceModels
                            {
                                id_element = obj.GetAuditQuestionNameById(dsAuditElement.Tables[0].Rows[i]["id_element"].ToString()),
                                id_auditratings = obj.GetAuditRatingNameById(dsAuditElement.Tables[0].Rows[i]["id_auditratings"].ToString()),
                                comment = dsAuditElement.Tables[0].Rows[i]["comment"].ToString(),
                                evidence_upload = dsAuditElement.Tables[0].Rows[i]["evidence_upload"].ToString(),
                            };

                            dicPerformanceElements.Add(dsAuditElement.Tables[0].Rows[i]["id_element"].ToString(), dsAuditElement.Tables[0].Rows[i]["id_auditratings"].ToString());
                            objAuditPerformanceList.lstAudit.Add(objElements);
                        }

                        ViewBag.PerformanceElement = objAuditPerformanceList;
                        ViewBag.dicPerformanceElement = dicPerformanceElements;
                        //make changes
                        ViewBag.AuditQuestions = obj.GetAuditElementsListbox(dept, dept);
                        ViewBag.AuditRating = obj.GetAuditRating();
                        ViewBag.Business = objGlobaldata.GetDropdownList("Business Nature");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        return View(objPerformance);

                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("AuditChecklistList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("AuditChecklistList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditPerformanceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditChecklistList");
        }

          
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuditPerformanceEdit(SupplierAuditChecklistModels objAuditChecklist, FormCollection form)
        {
            try
            {
                objAuditChecklist.id_AuditChecklist = Convert.ToInt32(form["id_AuditChecklist"]);
                objAuditChecklist.AuditNo = form["AuditNo"];
                objAuditChecklist.Auditors = form["Auditors"];
                objAuditChecklist.Auditee = form["Auditee"];
                objAuditChecklist.Notes = form["Notes"];
                objAuditChecklist.Remarks = form["Remarks"];
                objAuditChecklist.Business = form["Business"];



                DateTime dateValue;
                if (DateTime.TryParse(form["AuditDate"], out dateValue) == true)
                {
                    objAuditChecklist.AuditDate = dateValue;
                }

                SupplierAuditPerformanceModelsList objAudit = new SupplierAuditPerformanceModelsList();
                objAudit.lstAudit = new List<SupplierAuditPerformanceModels>();

                SupplierAuditElementsModels obj = new SupplierAuditElementsModels();
                MultiSelectList AuditQuestions = obj.GetAuditElementsListbox(objAuditChecklist.Business, objAuditChecklist.Department);
                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in AuditQuestions)
                {
                    if (i <= Convert.ToInt16(form["itmctn"]))
                    {
                        SupplierAuditPerformanceModels objElements = new SupplierAuditPerformanceModels();
                        string id = form["id_AuditPerformanceChecklist " + i];
                        objElements.id_AuditPerformanceChecklist = Convert.ToInt32(id);
                        objElements.id_element = form["Audit_Elements " + item.Value];
                        objElements.id_auditratings = form["id_auditratings " + item.Value];
                        objElements.comment = form["comment " + i];
                        string upload = form["evidence_upload" + i];
                        if (upload != null)
                        {
                            objElements.evidence_upload = form["evidence_upload" + i];

                        }

                        objAudit.lstAudit.Add(objElements);
                    }
                    i++;
                }


                if (objAuditChecklist.FunUpdateAuditPerformance(objAuditChecklist, objAudit))
                {
                    TempData["Successdata"] = "Audit Performance details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditPerformanceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditList", new { id_AuditChecklist = objAuditChecklist.id_AuditChecklist });
        }

          
        [AllowAnonymous]
        public JsonResult ChecklistDocDelete(FormCollection form)
        {
            try
            {
               
                      if (form["idt_checklist"] != null && form["idt_checklist"] != "")
                        {
                            SupplierAuditChecklistModels Doc = new SupplierAuditChecklistModels();
                            string sidt_checklist = form["idt_checklist"];


                            if (Doc.FunDeleteChecklist(sidt_checklist))
                            {
                                TempData["Successdata"] = "Audit Checklist deleted successfully";
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
                            TempData["alertdata"] = "Audit checklist Id cannot be Null or empty";
                            return Json("Failed");
                        }
                    
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChecklistDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }
}