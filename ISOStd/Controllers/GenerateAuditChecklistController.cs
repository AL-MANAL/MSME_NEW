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
    public class GenerateAuditChecklistController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        //Generate Checklist 
        public GenerateAuditChecklistController()
        {
            ViewBag.Menutype = "Audit";
            ViewBag.SubMenutype = "GenerateAuditChecklist";
        }

        public ActionResult GenerateAuditChecklist()
        {

            //  AuditElementsModels obj = new AuditElementsModels();
            GenerateAuditChecklistModels objChkList = new GenerateAuditChecklistModels();
            try
            {
                //ViewBag.Department_Id = objGlobaldata.GetCurrentUserSession().DeptID;
                ////ViewBag.AuditElements = obj.GetAuditElementsListbox(sDepartment);

                //ViewBag.division_Id = objGlobaldata.GetCurrentUserSession().division;
              //  ViewBag.location_Id = objGlobaldata.GetCurrentUserSession().Work_Location;
               // ViewBag.Department = objGlobaldata.GetDepartmentListbox(objGlobaldata.GetCurrentUserSession().division);
              //  ViewBag.location = objGlobaldata.GetDivisionLocationList(objGlobaldata.GetCurrentUserSession().division);
               // ViewBag.division = objGlobaldata.GetCompanyBranchListbox();
               // ViewBag.Department = objGlobaldata.GetDepartmentListbox();               
               //// ViewBag.AuditCriteria = objGlobaldata.GetAuditCriteria();
                objChkList.prepared_by = objGlobaldata.GetCurrentUserSession().empid;
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.IsoStd = objGlobaldata.GetIsoStdListbox();               
                ViewBag.Directorate = objGlobaldata.GetCompanyBranchListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateAuditChecklist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objChkList);
        }
                   
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateAuditChecklist(FormCollection form, GenerateAuditChecklistModels objChecklist)
        {
            try
            {
                objChecklist.grp = form["grp"];
                if(objChecklist.grp == "" || objChecklist.grp == null) {
                    objChecklist.grp_common = "common";
                }
                else
                {
                    objChecklist.grp_common = "";
                }

               
                // objChecklist.location = form["location"];                                
                                
               // objChecklist.ChecklistRef = objGlobaldata.GetMultiCompanyBranchNameById(form["division"]) + "-"+ objGlobaldata.GetDivisionLocationById(form["location"]) + "-"+ objGlobaldata.GetDeptNameById(form["Department"]) + "-00" + objChecklist.Itemno;

               
                //Reported By
                for (int i = 0; i < Convert.ToInt16(form["count"]); i++)
                {
                    if (form["iempno " + i] != "" && form["iempno " + i] != null)
                    {
                        objChecklist.prepared_by = objChecklist.prepared_by + "," + form["iempno " + i];
                    }
                }
                if (objChecklist.prepared_by != null)
                {
                    objChecklist.prepared_by = objChecklist.prepared_by.Trim(',');
                }

                //Notified To
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objChecklist.notified_to = form["nempno " + i] + "," + objChecklist.notified_to;
                    }
                }
                if (objChecklist.notified_to != null)
                {
                    objChecklist.notified_to = objChecklist.notified_to.Trim(',');
                }

                GenerateAuditChecklistModelsList objChkList = new GenerateAuditChecklistModelsList();
                objChkList.AuditCheckList = new List<GenerateAuditChecklistModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    GenerateAuditChecklistModels objAudit = new GenerateAuditChecklistModels();
                    if (form["IsoStd " + i] != null && form["IsoStd " + i] != "")
                    {
                        objAudit.IsoStd = form["IsoStd " + i];
                        objAudit.Clause = form["Clause " + i];
                        objAudit.Questions = form["Questions " + i];
                    }
                    objChkList.AuditCheckList.Add(objAudit);
                }

                if (objChecklist.FunAddAuditChecklist(objChecklist, objChkList))
                {
                    TempData["Successdata"] = "Audit checklist "+ objChecklist.ChecklistRef + " Generated successfully";
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
        public ActionResult AuditChecklistList(string branch_name)
        {
            GenerateAuditChecklistModelsList objAuditChecklist = new GenerateAuditChecklistModelsList();
            objAuditChecklist.AuditCheckList = new List<GenerateAuditChecklistModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select id_AuditChecklist,location,ChecklistRef," +
                    "prepared_by,created_on,ammended_on,notified_to,directorate,grp,grp_common from t_auditchecklist where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and directorate='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and directorate='" + sBranch_name + "' ";
                }

                sSqlstmt=sSqlstmt + sSearchtext;
                DataSet dsChecklistModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsChecklistModelsList.Tables.Count > 0 && dsChecklistModelsList.Tables[0].Rows.Count > 0)
                {   
                    for (int i = 0; i < dsChecklistModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            GenerateAuditChecklistModels objChecklist = new GenerateAuditChecklistModels
                            {
                                id_AuditChecklist = Convert.ToInt16(dsChecklistModelsList.Tables[0].Rows[i]["id_AuditChecklist"].ToString()),
                              //  Itemno = Convert.ToInt16(dsChecklistModelsList.Tables[0].Rows[i]["Itemno"].ToString()),
                                //Department = objGlobaldata.GetDeptNameById(dsChecklistModelsList.Tables[0].Rows[i]["Department"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsChecklistModelsList.Tables[0].Rows[i]["location"].ToString()),
                               // audit_division = objGlobaldata.GetMultiCompanyBranchNameById(dsChecklistModelsList.Tables[0].Rows[i]["audit_division"].ToString()),
                                ChecklistRef = dsChecklistModelsList.Tables[0].Rows[i]["ChecklistRef"].ToString(),
                              //  AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsChecklistModelsList.Tables[0].Rows[i]["AuditCriteria"].ToString()),
                                prepared_by = objGlobaldata.GetMultiHrEmpNameById(dsChecklistModelsList.Tables[0].Rows[i]["prepared_by"].ToString()),
                                notified_to = objGlobaldata.GetMultiHrEmpNameById(dsChecklistModelsList.Tables[0].Rows[i]["notified_to"].ToString()),
                                directorate = objGlobaldata.GetMultiCompanyBranchNameById(dsChecklistModelsList.Tables[0].Rows[i]["directorate"].ToString()),
                                grp = objGlobaldata.GetMultiDeptNameById(dsChecklistModelsList.Tables[0].Rows[i]["grp"].ToString()),
                                grp_common = dsChecklistModelsList.Tables[0].Rows[i]["grp_common"].ToString(),
                              };
                            DateTime dtDocDate;
                            if (dsChecklistModelsList.Tables[0].Rows[i]["created_on"].ToString() != ""
                               && DateTime.TryParse(dsChecklistModelsList.Tables[0].Rows[i]["created_on"].ToString(), out dtDocDate))
                            {
                                objChecklist.created_on = dtDocDate;
                            }

                            if (dsChecklistModelsList.Tables[0].Rows[i]["ammended_on"].ToString() != ""
                              && DateTime.TryParse(dsChecklistModelsList.Tables[0].Rows[i]["ammended_on"].ToString(), out dtDocDate))
                            {
                                objChecklist.ammended_on = dtDocDate;
                            }

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

            return View(objAuditChecklist.AuditCheckList.ToList());
        }

        [AllowAnonymous]
        public JsonResult AuditChecklistListSearch(string branch_name)
        {
            GenerateAuditChecklistModelsList objAuditChecklist = new GenerateAuditChecklistModelsList();
            objAuditChecklist.AuditCheckList = new List<GenerateAuditChecklistModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select id_AuditChecklist,location,ChecklistRef," +
                    "prepared_by,created_on,ammended_on,notified_to,directorate,grp,grp_common from t_auditchecklist where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and directorate='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and directorate='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsChecklistModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsChecklistModelsList.Tables.Count > 0 && dsChecklistModelsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsChecklistModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            GenerateAuditChecklistModels objChecklist = new GenerateAuditChecklistModels
                            {
                                id_AuditChecklist = Convert.ToInt16(dsChecklistModelsList.Tables[0].Rows[i]["id_AuditChecklist"].ToString()),
                               // Itemno = Convert.ToInt16(dsChecklistModelsList.Tables[0].Rows[i]["Itemno"].ToString()),
                                //Department = objGlobaldata.GetDeptNameById(dsChecklistModelsList.Tables[0].Rows[i]["Department"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsChecklistModelsList.Tables[0].Rows[i]["location"].ToString()),
                              //  audit_division = objGlobaldata.GetMultiCompanyBranchNameById(dsChecklistModelsList.Tables[0].Rows[i]["audit_division"].ToString()),
                                ChecklistRef = dsChecklistModelsList.Tables[0].Rows[i]["ChecklistRef"].ToString(),
                               // AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsChecklistModelsList.Tables[0].Rows[i]["AuditCriteria"].ToString()),
                                //dept_common = dsChecklistModelsList.Tables[0].Rows[i]["dept_common"].ToString(),
                                prepared_by = objGlobaldata.GetMultiHrEmpNameById(dsChecklistModelsList.Tables[0].Rows[i]["prepared_by"].ToString()),
                                notified_to = objGlobaldata.GetMultiHrEmpNameById(dsChecklistModelsList.Tables[0].Rows[i]["notified_to"].ToString()),
                                directorate = objGlobaldata.GetMultiCompanyBranchNameById(dsChecklistModelsList.Tables[0].Rows[i]["directorate"].ToString()),
                                grp = objGlobaldata.GetMultiDeptNameById(dsChecklistModelsList.Tables[0].Rows[i]["grp"].ToString()),
                                grp_common = dsChecklistModelsList.Tables[0].Rows[i]["grp_common"].ToString(),
                             
                            };
                            DateTime dtDocDate;
                            if (dsChecklistModelsList.Tables[0].Rows[i]["created_on"].ToString() != ""
                               && DateTime.TryParse(dsChecklistModelsList.Tables[0].Rows[i]["created_on"].ToString(), out dtDocDate))
                            {
                                objChecklist.created_on = dtDocDate;
                            }

                            if (dsChecklistModelsList.Tables[0].Rows[i]["ammended_on"].ToString() != ""
                              && DateTime.TryParse(dsChecklistModelsList.Tables[0].Rows[i]["ammended_on"].ToString(), out dtDocDate))
                            {
                                objChecklist.ammended_on = dtDocDate;
                            }

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
                objGlobaldata.AddFunctionalLog("Exception in AuditChecklistListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult AuditChecklistEdit()
        {
            AuditElementsModels obj = new AuditElementsModels();
            GenerateAuditChecklistModels objAudit = new GenerateAuditChecklistModels();
            try
            {
                //ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                //ViewBag.AuditCriteria = objGlobaldata.GetAuditCriteria();
                if (Request.QueryString["id_AuditChecklist"] != null && Request.QueryString["id_AuditChecklist"] != "")
                {

                    string sid_AuditChecklist = Request.QueryString["id_AuditChecklist"];

                    string sSqlstmt = "select id_AuditChecklist,location,ChecklistRef," +
                   "prepared_by,created_on,ammended_on,notified_to,directorate,grp,grp_common from t_auditchecklist where id_AuditChecklist='" + sid_AuditChecklist + "'";

                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        objAudit = new GenerateAuditChecklistModels
                        {
                            id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["id_AuditChecklist"].ToString()),
                            // Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
                           // Department = /*objGlobaldata.GetDeptNameById*/(dsAudit.Tables[0].Rows[0]["Department"].ToString()),
                            //AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                            //Questions = dsAudit.Tables[0].Rows[0]["Questions"].ToString(),
                           // audit_division = dsAudit.Tables[0].Rows[0]["audit_division"].ToString(),
                            location = dsAudit.Tables[0].Rows[0]["location"].ToString(),
                           // dept_common = dsAudit.Tables[0].Rows[0]["dept_common"].ToString(),
                            prepared_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsAudit.Tables[0].Rows[0]["prepared_by"].ToString()),
                            notified_to = /*objGlobaldata.GetMultiHrEmpNameById*/(dsAudit.Tables[0].Rows[0]["notified_to"].ToString()),
                            directorate = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsAudit.Tables[0].Rows[0]["directorate"].ToString()),
                            grp = /*objGlobaldata.GetMultiDeptNameById*/(dsAudit.Tables[0].Rows[0]["grp"].ToString()),
                            grp_common = dsAudit.Tables[0].Rows[0]["grp_common"].ToString(),
                        };

                        if (dsAudit.Tables[0].Rows[0]["prepared_by"].ToString() != "")
                        {
                            ViewBag.PreparedToArray = (dsAudit.Tables[0].Rows[0]["prepared_by"].ToString()).Split(',');
                        }

                        if (dsAudit.Tables[0].Rows[0]["notified_to"].ToString() != "")
                        {
                            ViewBag.NotifiedByArray = (dsAudit.Tables[0].Rows[0]["notified_to"].ToString()).Split(',');
                        }

                        DateTime dtDocDate;
                        if (dsAudit.Tables[0].Rows[0]["created_on"].ToString() != ""
                           && DateTime.TryParse(dsAudit.Tables[0].Rows[0]["created_on"].ToString(), out dtDocDate))
                        {
                            objAudit.created_on = dtDocDate;
                        }

                        if (dsAudit.Tables[0].Rows[0]["ammended_on"].ToString() != ""
                          && DateTime.TryParse(dsAudit.Tables[0].Rows[0]["ammended_on"].ToString(), out dtDocDate))
                        {
                            objAudit.ammended_on = dtDocDate;
                        }
                        // ViewBag.AuditElements = obj.GetAuditElementsListboxByIsoStd(dsAudit.Tables[0].Rows[0]["Department"].ToString(), dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString());
                        //ViewBag.Department = objGlobaldata.GetDepartmentListbox(dsAudit.Tables[0].Rows[0]["audit_division"].ToString());
                        // ViewBag.location = objGlobaldata.GetDivisionLocationList(dsAudit.Tables[0].Rows[0]["branch"].ToString());
                       // ViewBag.division = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.IsoStd = objGlobaldata.GetIsoStdListbox();
                        //DirectorateModels objDocType = new DirectorateModels();
                       // DirectorateGroupModels objGrp = new DirectorateGroupModels();
                        ViewBag.Directorate = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Group = objGlobaldata.GetDepartmentListbox(dsAudit.Tables[0].Rows[0]["directorate"].ToString());
                        
                        GenerateAuditChecklistModelsList ObjList = new GenerateAuditChecklistModelsList();
                        ObjList.AuditCheckList = new List<GenerateAuditChecklistModels>();
                        string sSqlstmt1 = "select id_auditchecklist_trans,id_AuditChecklist,IsoStd,Clause,Questions"
                                + " from t_auditchecklist_trans where id_AuditChecklist='" + sid_AuditChecklist + "'";

                        DataSet dsAuditTrans = objGlobaldata.Getdetails(sSqlstmt1);
                        if (dsAuditTrans.Tables.Count > 0 && dsAuditTrans.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsAuditTrans.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    GenerateAuditChecklistModels objAudit1 = new GenerateAuditChecklistModels
                                    {
                                        id_auditchecklist_trans = (dsAuditTrans.Tables[0].Rows[i]["id_auditchecklist_trans"].ToString()),
                                        id_AuditChecklist = Convert.ToInt16(dsAuditTrans.Tables[0].Rows[i]["id_AuditChecklist"].ToString()),
                                        IsoStd = (dsAuditTrans.Tables[0].Rows[i]["IsoStd"].ToString()),
                                        Clause = (dsAuditTrans.Tables[0].Rows[i]["Clause"].ToString()),
                                        Questions = dsAuditTrans.Tables[0].Rows[i]["Questions"].ToString(),
                                    };

                                    ObjList.AuditCheckList.Add(objAudit1);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                                ViewBag.AuditTransList = ObjList;
                            }
                        }
                        
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("AuditChecklistList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditChecklistEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AuditChecklistList");
            }
            return View(objAudit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult AuditChecklistEdit(GenerateAuditChecklistModels objAudit, FormCollection form)
        {
            try
            {
                objAudit.grp = form["grp"];
                
                if (objAudit.grp == "" || objAudit.grp == null)
                {
                    objAudit.grp_common = "common";
                }
                else
                {
                    objAudit.grp_common = "";
                }

               
                // int cnt = Convert.ToInt16(form["itemcnt"]);


                //objAudit.location = form["location"];
                //objAudit.division = form["division"];
                //objAudit.ChecklistRef = objGlobaldata.GetMultiCompanyBranchNameById(form["division"])+"-" + objGlobaldata.GetDivisionLocationById(form["location"])+"-" + objGlobaldata.GetDeptNameById(form["Department"]) + "-00" + objAudit.Itemno;
                //string quest = "";
                //for (int i = 0; i < cnt; i++)
                //{
                //    quest = quest + form["Audit_Elements " + i] + ",";

                //    objAudit.Questions = quest;
                //}


                //Reported By
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["iempno " + i] != "" && form["iempno " + i] != null)
                    {
                        objAudit.prepared_by = objAudit.prepared_by + "," + form["iempno " + i];
                    }
                }
                if (objAudit.prepared_by != null)
                {
                    objAudit.prepared_by = objAudit.prepared_by.Trim(',');
                }

                //Notified To
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objAudit.notified_to = form["nempno " + i] + "," + objAudit.notified_to;
                    }
                }
                if (objAudit.notified_to != null)
                {
                    objAudit.notified_to = objAudit.notified_to.Trim(',');
                }

                GenerateAuditChecklistModelsList objChkList = new GenerateAuditChecklistModelsList();
                objChkList.AuditCheckList = new List<GenerateAuditChecklistModels>();
                for (int i = 0; i < Convert.ToInt16(form["count"]); i++)
                {
                    if (form["IsoStd " + i] != null && form["IsoStd " + i] != "")
                    {
                        GenerateAuditChecklistModels objAuditModel = new GenerateAuditChecklistModels();
                        {
                            objAuditModel.IsoStd = form["IsoStd " + i];
                            objAuditModel.Clause = form["Clause " + i];
                            objAuditModel.Questions = form["Questions " + i];
                        }
                        objChkList.AuditCheckList.Add(objAuditModel);
                    }
                }

                if (objAudit.FunUpdateAuditChecklist(objAudit, objChkList))
                {
                    TempData["Successdata"] = "Audit checklist details " + objAudit.ChecklistRef + " updated successfully";
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

        [AllowAnonymous]
        public ActionResult AuditChecklistDetails()
        {
            AuditElementsModels obj = new AuditElementsModels();
            GenerateAuditChecklistModels objAudit = new GenerateAuditChecklistModels();
            try
            {
                //ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                //ViewBag.AuditCriteria = objGlobaldata.GetAuditCriteria();
                if (Request.QueryString["id_AuditChecklist"] != null && Request.QueryString["id_AuditChecklist"] != "")
                {

                    string sid_AuditChecklist = Request.QueryString["id_AuditChecklist"];

                    string sSqlstmt = "select id_AuditChecklist,location,ChecklistRef," +
                   "prepared_by,created_on,ammended_on,notified_to,directorate,grp,grp_common from t_auditchecklist where id_AuditChecklist='" + sid_AuditChecklist + "'";

                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        objAudit = new GenerateAuditChecklistModels
                        {
                            id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["id_AuditChecklist"].ToString()),
                            // Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
                           // Department = objGlobaldata.GetDeptNameById(dsAudit.Tables[0].Rows[0]["Department"].ToString()),
                            //AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                            ChecklistRef = dsAudit.Tables[0].Rows[0]["ChecklistRef"].ToString(),
                           // audit_division = objGlobaldata.GetCompanyBranchNameById(dsAudit.Tables[0].Rows[0]["audit_division"].ToString()),
                            //location = dsAudit.Tables[0].Rows[0]["location"].ToString(),
                           // dept_common = dsAudit.Tables[0].Rows[0]["dept_common"].ToString(),
                            prepared_by = objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["prepared_by"].ToString()),
                            notified_to = objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["notified_to"].ToString()),
                            directorate = objGlobaldata.GetMultiCompanyBranchNameById(dsAudit.Tables[0].Rows[0]["directorate"].ToString()),
                            grp = objGlobaldata.GetMultiDeptNameById(dsAudit.Tables[0].Rows[0]["grp"].ToString()),
                            grp_common = dsAudit.Tables[0].Rows[0]["grp_common"].ToString(),
                         };

                        //if (dsAudit.Tables[0].Rows[0]["prepared_by"].ToString() != "")
                        //{
                        //    ViewBag.PreparedToArray = (dsAudit.Tables[0].Rows[0]["prepared_by"].ToString()).Split(',');
                        //}

                        //if (dsAudit.Tables[0].Rows[0]["notified_to"].ToString() != "")
                        //{
                        //    ViewBag.NotifiedByArray = (dsAudit.Tables[0].Rows[0]["notified_to"].ToString()).Split(',');
                        //}

                        DateTime dtDocDate;
                        if (dsAudit.Tables[0].Rows[0]["created_on"].ToString() != ""
                           && DateTime.TryParse(dsAudit.Tables[0].Rows[0]["created_on"].ToString(), out dtDocDate))
                        {
                            objAudit.created_on = dtDocDate;
                        }

                        if (dsAudit.Tables[0].Rows[0]["ammended_on"].ToString() != ""
                          && DateTime.TryParse(dsAudit.Tables[0].Rows[0]["ammended_on"].ToString(), out dtDocDate))
                        {
                            objAudit.ammended_on = dtDocDate;
                        }
                       
                        GenerateAuditChecklistModelsList ObjList = new GenerateAuditChecklistModelsList();
                        ObjList.AuditCheckList = new List<GenerateAuditChecklistModels>();
                        string sSqlstmt1 = "select id_auditchecklist_trans,id_AuditChecklist,IsoStd,Clause,Questions"
                                + " from t_auditchecklist_trans where id_AuditChecklist='" + sid_AuditChecklist + "'";

                        DataSet dsAuditTrans = objGlobaldata.Getdetails(sSqlstmt1);
                        if (dsAuditTrans.Tables.Count > 0 && dsAuditTrans.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsAuditTrans.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    GenerateAuditChecklistModels objAudit1 = new GenerateAuditChecklistModels
                                    {
                                        id_auditchecklist_trans = (dsAuditTrans.Tables[0].Rows[i]["id_auditchecklist_trans"].ToString()),
                                        id_AuditChecklist = Convert.ToInt16(dsAuditTrans.Tables[0].Rows[i]["id_AuditChecklist"].ToString()),
                                        IsoStd =objGlobaldata.GetIsoStdNameById(dsAuditTrans.Tables[0].Rows[i]["IsoStd"].ToString()),
                                        Clause = (dsAuditTrans.Tables[0].Rows[i]["Clause"].ToString()),
                                        Questions = dsAuditTrans.Tables[0].Rows[i]["Questions"].ToString(),
                                    };

                                    ObjList.AuditCheckList.Add(objAudit1);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AuditChecklistDetails: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                                ViewBag.AuditTransList = ObjList;
                            }
                        }                        
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("AuditChecklistList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditChecklistDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AuditChecklistList");
            }
            return View(objAudit);
        }
        
        [AllowAnonymous]
        public ActionResult AuditChecklistInfo(int id)
        {
            AuditElementsModels obj = new AuditElementsModels();
            GenerateAuditChecklistModels objAudit = new GenerateAuditChecklistModels();
            try
            {
                //ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                //ViewBag.AuditCriteria = objGlobaldata.GetAuditCriteria();
                if (id > 0)
                {

                    string sid_AuditChecklist = Request.QueryString["id_AuditChecklist"];

                    string sSqlstmt = "select id_AuditChecklist,location,ChecklistRef," +
                   "prepared_by,created_on,ammended_on,notified_to,directorate,grp,grp_common from t_auditchecklist where id_AuditChecklist='" + id + "'";

                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        objAudit = new GenerateAuditChecklistModels
                        {
                            id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["id_AuditChecklist"].ToString()),
                            // Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
                            //Department = objGlobaldata.GetDeptNameById(dsAudit.Tables[0].Rows[0]["Department"].ToString()),
                            //AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                            ChecklistRef = dsAudit.Tables[0].Rows[0]["ChecklistRef"].ToString(),
                            //audit_division = objGlobaldata.GetCompanyBranchNameById(dsAudit.Tables[0].Rows[0]["audit_division"].ToString()),
                            //location = dsAudit.Tables[0].Rows[0]["location"].ToString(),
                           // dept_common = dsAudit.Tables[0].Rows[0]["dept_common"].ToString(),
                            prepared_by = objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["prepared_by"].ToString()),
                            notified_to = objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["notified_to"].ToString()),
                            directorate = objGlobaldata.GetMultiCompanyBranchNameById(dsAudit.Tables[0].Rows[0]["directorate"].ToString()),
                            grp = objGlobaldata.GetMultiDeptNameById(dsAudit.Tables[0].Rows[0]["grp"].ToString()),
                            grp_common = dsAudit.Tables[0].Rows[0]["grp_common"].ToString(),
                        
                        };

                        //if (dsAudit.Tables[0].Rows[0]["prepared_by"].ToString() != "")
                        //{
                        //    ViewBag.PreparedToArray = (dsAudit.Tables[0].Rows[0]["prepared_by"].ToString()).Split(',');
                        //}

                        //if (dsAudit.Tables[0].Rows[0]["notified_to"].ToString() != "")
                        //{
                        //    ViewBag.NotifiedByArray = (dsAudit.Tables[0].Rows[0]["notified_to"].ToString()).Split(',');
                        //}

                        DateTime dtDocDate;
                        if (dsAudit.Tables[0].Rows[0]["created_on"].ToString() != ""
                           && DateTime.TryParse(dsAudit.Tables[0].Rows[0]["created_on"].ToString(), out dtDocDate))
                        {
                            objAudit.created_on = dtDocDate;
                        }

                        if (dsAudit.Tables[0].Rows[0]["ammended_on"].ToString() != ""
                          && DateTime.TryParse(dsAudit.Tables[0].Rows[0]["ammended_on"].ToString(), out dtDocDate))
                        {
                            objAudit.ammended_on = dtDocDate;
                        }

                        GenerateAuditChecklistModelsList ObjList = new GenerateAuditChecklistModelsList();
                        ObjList.AuditCheckList = new List<GenerateAuditChecklistModels>();
                        string sSqlstmt1 = "select id_auditchecklist_trans,id_AuditChecklist,IsoStd,Clause,Questions"
                                + " from t_auditchecklist_trans where id_AuditChecklist='" + id + "'";

                        DataSet dsAuditTrans = objGlobaldata.Getdetails(sSqlstmt1);
                        if (dsAuditTrans.Tables.Count > 0 && dsAuditTrans.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsAuditTrans.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    GenerateAuditChecklistModels objAudit1 = new GenerateAuditChecklistModels
                                    {
                                        id_auditchecklist_trans = (dsAuditTrans.Tables[0].Rows[i]["id_auditchecklist_trans"].ToString()),
                                        id_AuditChecklist = Convert.ToInt16(dsAuditTrans.Tables[0].Rows[i]["id_AuditChecklist"].ToString()),
                                        IsoStd = objGlobaldata.GetIsoStdNameById(dsAuditTrans.Tables[0].Rows[i]["IsoStd"].ToString()),
                                        Clause = (dsAuditTrans.Tables[0].Rows[i]["Clause"].ToString()),
                                        Questions = dsAuditTrans.Tables[0].Rows[i]["Questions"].ToString(),
                                    };

                                    ObjList.AuditCheckList.Add(objAudit1);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AuditChecklistInfo: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                                ViewBag.AuditTransList = ObjList;
                            }
                        }                        
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("AuditChecklistList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditChecklistInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AuditChecklistList");
            }
            return View(objAudit);
        }

        [AllowAnonymous]
        public ActionResult AuditChecklistReport(FormCollection form)
        {
            AuditElementsModels obj = new AuditElementsModels();
            GenerateAuditChecklistModels objAudit = new GenerateAuditChecklistModels();
            try
            {
                //ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                //ViewBag.AuditCriteria = objGlobaldata.GetAuditCriteria();
                if (form["id_AuditChecklist"] != null && form["id_AuditChecklist"] != "")
                {

                    string sid_AuditChecklist = form["id_AuditChecklist"];

                    string sSqlstmt = "select id_AuditChecklist,location,ChecklistRef," +
                   "prepared_by,created_on,ammended_on,notified_to,directorate,grp,grp_common from t_auditchecklist where id_AuditChecklist='" + sid_AuditChecklist + "'";

                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        objAudit = new GenerateAuditChecklistModels
                        {
                            id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["id_AuditChecklist"].ToString()),
                            // Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
                           // Department = objGlobaldata.GetDeptNameById(dsAudit.Tables[0].Rows[0]["Department"].ToString()),
                            //AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                            ChecklistRef = dsAudit.Tables[0].Rows[0]["ChecklistRef"].ToString(),
                           // audit_division = objGlobaldata.GetCompanyBranchNameById(dsAudit.Tables[0].Rows[0]["audit_division"].ToString()),
                            //location = dsAudit.Tables[0].Rows[0]["location"].ToString(),
                           // dept_common = dsAudit.Tables[0].Rows[0]["dept_common"].ToString(),
                            prepared_by = objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["prepared_by"].ToString()),
                            notified_to = objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["notified_to"].ToString()),
                            directorate = objGlobaldata.GetMultiCompanyBranchNameById(dsAudit.Tables[0].Rows[0]["directorate"].ToString()),
                            grp = objGlobaldata.GetMultiDeptNameById(dsAudit.Tables[0].Rows[0]["grp"].ToString()),
                            grp_common = dsAudit.Tables[0].Rows[0]["grp_common"].ToString(),
                         
                        };

                        //if (dsAudit.Tables[0].Rows[0]["prepared_by"].ToString() != "")
                        //{
                        //    ViewBag.PreparedToArray = (dsAudit.Tables[0].Rows[0]["prepared_by"].ToString()).Split(',');
                        //}

                        //if (dsAudit.Tables[0].Rows[0]["notified_to"].ToString() != "")
                        //{
                        //    ViewBag.NotifiedByArray = (dsAudit.Tables[0].Rows[0]["notified_to"].ToString()).Split(',');
                        //}

                        DateTime dtDocDate;
                        if (dsAudit.Tables[0].Rows[0]["created_on"].ToString() != ""
                           && DateTime.TryParse(dsAudit.Tables[0].Rows[0]["created_on"].ToString(), out dtDocDate))
                        {
                            objAudit.created_on = dtDocDate;
                        }

                        if (dsAudit.Tables[0].Rows[0]["ammended_on"].ToString() != ""
                          && DateTime.TryParse(dsAudit.Tables[0].Rows[0]["ammended_on"].ToString(), out dtDocDate))
                        {
                            objAudit.ammended_on = dtDocDate;
                        }
                        ViewBag.ChecklistDetails = objAudit;

                        CompanyModels objCompany = new CompanyModels();
                        dsAudit = objCompany.GetCompanyDetailsForReport(dsAudit);
                        string logged_by = objGlobaldata.GetCurrentUserSession().empid;

                        dsAudit = objGlobaldata.GetReportDetails(dsAudit, objAudit.ChecklistRef, logged_by, "AUDIT CHECKLIST REPORT");
                        ViewBag.CompanyInfo = dsAudit;

                        GenerateAuditChecklistModelsList ObjList = new GenerateAuditChecklistModelsList();
                        ObjList.AuditCheckList = new List<GenerateAuditChecklistModels>();
                        string sSqlstmt1 = "select id_auditchecklist_trans,id_AuditChecklist,IsoStd,Clause,Questions"
                                + " from t_auditchecklist_trans where id_AuditChecklist='" + sid_AuditChecklist + "'";

                        DataSet dsAuditTrans = objGlobaldata.Getdetails(sSqlstmt1);
                        if (dsAuditTrans.Tables.Count > 0 && dsAuditTrans.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsAuditTrans.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    GenerateAuditChecklistModels objAudit1 = new GenerateAuditChecklistModels
                                    {
                                        id_auditchecklist_trans = (dsAuditTrans.Tables[0].Rows[i]["id_auditchecklist_trans"].ToString()),
                                        id_AuditChecklist = Convert.ToInt16(dsAuditTrans.Tables[0].Rows[i]["id_AuditChecklist"].ToString()),
                                        IsoStd = objGlobaldata.GetIsoStdNameById(dsAuditTrans.Tables[0].Rows[i]["IsoStd"].ToString()),
                                        Clause = (dsAuditTrans.Tables[0].Rows[i]["Clause"].ToString()),
                                        Questions = dsAuditTrans.Tables[0].Rows[i]["Questions"].ToString(),
                                    };

                                    ObjList.AuditCheckList.Add(objAudit1);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AuditChecklistReport: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                                ViewBag.AuditTransList = ObjList;
                            }
                        }
                        
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("AuditChecklistList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditChecklistReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AuditChecklistList");
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

        //[AllowAnonymous]
        //public ActionResult AuditChecklistDetails()
        //{
        //    try
        //    {
        //        if (Request.QueryString["idt_checklist"] != null && Request.QueryString["idt_checklist"] != "")
        //        {
        //            string sidt_checklist = Request.QueryString["idt_checklist"];

        //            string sSqlstmt = "select idt_checklist,t.id_AuditChecklist,Itemno,Department,AuditCriteria,AuditNo,AuditDate,"
        //            + "Auditors,Auditee,Notes,Remarks,Questions from t_generateauditchecklist t,t_auditchecklist tt where t.id_AuditChecklist=tt.id_AuditChecklist and idt_checklist='" + sidt_checklist + "'";


        //            DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
        //            {
        //                GenerateAuditChecklistModels objAudit = new GenerateAuditChecklistModels
        //                {
        //                    idt_checklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["idt_checklist"].ToString()),
        //                    id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["id_AuditChecklist"].ToString()),
        //                    Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
        //                    Department = objGlobaldata.GetDeptNameById(dsAudit.Tables[0].Rows[0]["Department"].ToString()),
        //                    AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
        //                    AuditNo = objGlobaldata.GetAuditNoById(dsAudit.Tables[0].Rows[0]["AuditNo"].ToString()),
        //                    Auditors = dsAudit.Tables[0].Rows[0]["Auditors"].ToString(),
        //                    Auditee = (dsAudit.Tables[0].Rows[0]["Auditee"].ToString()),
        //                    Notes = dsAudit.Tables[0].Rows[0]["Notes"].ToString(),
        //                    Remarks = dsAudit.Tables[0].Rows[0]["Remarks"].ToString(),
        //                };

        //                DateTime dtValue;
        //                if (DateTime.TryParse(dsAudit.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
        //                {
        //                    objAudit.AuditDate = dtValue;
        //                }

        //                sSqlstmt = "SELECT id_AuditPerformanceChecklist,idt_checklist,id_element,id_auditratings,"
        //               + "comment,evidence_upload FROM t_auditperformancechecklist where idt_checklist='"
        //                   + objAudit.idt_checklist + "'";

        //                DataSet dsAuditElement = objGlobaldata.Getdetails(sSqlstmt);


        //                AuditPerformanceModelsList objAuditPerformanceList = new AuditPerformanceModelsList();
        //                objAuditPerformanceList.lstAudit = new List<AuditPerformanceModels>();

        //                AuditElementsModels obj = new AuditElementsModels();

        //                for (int i = 0; dsAuditElement.Tables.Count > 0 && i < dsAuditElement.Tables[0].Rows.Count; i++)
        //                {
        //                    AuditPerformanceModels objElements = new AuditPerformanceModels
        //                    {

        //                        id_element = obj.GetAuditQuestionNameById(dsAuditElement.Tables[0].Rows[i]["id_element"].ToString()),
        //                        id_auditratings = obj.GetAuditRatingNameById(dsAuditElement.Tables[0].Rows[i]["id_auditratings"].ToString()),
        //                        comment = dsAuditElement.Tables[0].Rows[i]["comment"].ToString(),
        //                        evidence_upload = dsAuditElement.Tables[0].Rows[i]["evidence_upload"].ToString(),
        //                    };
        //                    objAuditPerformanceList.lstAudit.Add(objElements);
        //                }

        //                ViewBag.AuditElement = objAuditPerformanceList;
        //                ViewBag.AuditRating = obj.GetAuditRating();
        //                return View(objAudit);

        //            }
        //            else
        //            {
        //                TempData["alertdata"] = "No data exists";
        //                return RedirectToAction("AuditChecklistList");
        //            }
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = " Id cannot be null";
        //            return RedirectToAction("AuditChecklistList");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AuditChecklistDetails: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return RedirectToAction("AuditChecklistList");
        //}

        //[AllowAnonymous]
        //public ActionResult AuditChecklistInfo(int id)
        //{
        //    try
        //    {
        //        string sSqlstmt = "select idt_checklist,t.id_AuditChecklist,Itemno,Department,AuditCriteria,AuditNo,AuditDate,"
        //        + "Auditors,Auditee,Notes,Remarks,Questions from t_generateauditchecklist t,t_auditchecklist tt where t.id_AuditChecklist=tt.id_AuditChecklist and idt_checklist='" + id + "'";

        //        DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

        //        if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
        //        {
        //            GenerateAuditChecklistModels objAudit = new GenerateAuditChecklistModels
        //            {
        //                idt_checklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["idt_checklist"].ToString()),
        //                id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["id_AuditChecklist"].ToString()),
        //                Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
        //                Department = objGlobaldata.GetDeptNameById(dsAudit.Tables[0].Rows[0]["Department"].ToString()),
        //                AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
        //                AuditNo = objGlobaldata.GetAuditNoById(dsAudit.Tables[0].Rows[0]["AuditNo"].ToString()),
        //                Auditors = dsAudit.Tables[0].Rows[0]["Auditors"].ToString(),
        //                Auditee = (dsAudit.Tables[0].Rows[0]["Auditee"].ToString()),
        //                Notes = dsAudit.Tables[0].Rows[0]["Notes"].ToString(),
        //                Remarks = dsAudit.Tables[0].Rows[0]["Remarks"].ToString(),
        //            };

        //            DateTime dtValue;
        //            if (DateTime.TryParse(dsAudit.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
        //            {
        //                objAudit.AuditDate = dtValue;
        //            }

        //            sSqlstmt = "SELECT id_AuditPerformanceChecklist,idt_checklist,id_element,id_auditratings,"
        //           + "comment,evidence_upload FROM t_auditperformancechecklist where idt_checklist='"
        //               + objAudit.idt_checklist + "'";

        //            DataSet dsAuditElement = objGlobaldata.Getdetails(sSqlstmt);


        //            AuditPerformanceModelsList objAuditPerformanceList = new AuditPerformanceModelsList();
        //            objAuditPerformanceList.lstAudit = new List<AuditPerformanceModels>();

        //            AuditElementsModels obj = new AuditElementsModels();

        //            for (int i = 0; dsAuditElement.Tables.Count > 0 && i < dsAuditElement.Tables[0].Rows.Count; i++)
        //            {
        //                AuditPerformanceModels objElements = new AuditPerformanceModels
        //                {

        //                    id_element = obj.GetAuditQuestionNameById(dsAuditElement.Tables[0].Rows[i]["id_element"].ToString()),
        //                    id_auditratings = obj.GetAuditRatingNameById(dsAuditElement.Tables[0].Rows[i]["id_auditratings"].ToString()),
        //                    comment = dsAuditElement.Tables[0].Rows[i]["comment"].ToString(),
        //                    evidence_upload = dsAuditElement.Tables[0].Rows[i]["evidence_upload"].ToString(),
        //                };
        //                objAuditPerformanceList.lstAudit.Add(objElements);
        //            }

        //            ViewBag.AuditElement = objAuditPerformanceList;
        //            ViewBag.AuditRating = obj.GetAuditRating();
        //            return View(objAudit);

        //        }
        //        else
        //        {
        //            TempData["alertdata"] = "No data exists";
        //            return RedirectToAction("AuditChecklistList");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AuditChecklistDetails: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return RedirectToAction("AuditChecklistList");
        //}

        //[HttpPost]
        //public ActionResult AuditChecklistReport(FormCollection form)
        //{
        //    string sidt_checklist = form["idt_checklist"];
        //    try
        //    {
        //        if (sidt_checklist != "")
        //        {
        //            //string sSqlstmt = "select t.id_AuditChecklist,idt_checklist,Itemno,Department,AuditCriteria,AuditNo,AuditDate,"
        //            //+ "Auditors,Auditee,Notes,Remarks from t_auditchecklist t,t_generateauditchecklist tt where t.id_AuditChecklist=tt.id_AuditChecklist and idt_checklist='" + sidt_checklist + "'";

        //            string sSqlstmt = "select t.id_AuditChecklist,idt_checklist,Itemno,Department,t.AuditCriteria,AuditNo," +
        //                "tt.AuditDate,tt.Auditors,tt.Auditee,Notes,Remarks,AuditNum from t_auditchecklist t," +
        //                "t_generateauditchecklist tt , t_internal_audit ttt where t.id_AuditChecklist=tt.id_AuditChecklist " +
        //                "and tt.AuditNo=ttt.AuditID and idt_checklist ='" + sidt_checklist + "'";


        //            DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
        //            {
        //                GenerateAuditChecklistModels objAudit = new GenerateAuditChecklistModels
        //                {
        //                    idt_checklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["idt_checklist"].ToString()),
        //                    Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
        //                    Department = objGlobaldata.GetDeptNameById(dsAudit.Tables[0].Rows[0]["Department"].ToString()),
        //                    AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
        //                    AuditNo = objGlobaldata.GetAuditNoById(dsAudit.Tables[0].Rows[0]["AuditNo"].ToString()),
        //                    Auditors = dsAudit.Tables[0].Rows[0]["Auditors"].ToString(),
        //                    Auditee = (dsAudit.Tables[0].Rows[0]["Auditee"].ToString()),
        //                    Notes = dsAudit.Tables[0].Rows[0]["Notes"].ToString(),
        //                    Remarks = dsAudit.Tables[0].Rows[0]["Remarks"].ToString(),
        //                    AuditNum = dsAudit.Tables[0].Rows[0]["AuditNum"].ToString(),

        //                };
        //                DateTime dtValue;
        //                if (DateTime.TryParse(dsAudit.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
        //                {
        //                    objAudit.AuditDate = dtValue;
        //                }

        //                CompanyModels objCompany = new CompanyModels();
        //                dsAudit = objCompany.GetCompanyDetailsForReport(dsAudit);
        //                string logged_by = objGlobaldata.GetCurrentUserSession().empid;

        //                dsAudit = objGlobaldata.GetReportDetails(dsAudit, objAudit.AuditNum, logged_by, "AUDIT CHECKLIST REPORT");
        //                ViewBag.CompanyInfo = dsAudit;


        //                ViewBag.ChecklistDetails = objAudit;

        //                sSqlstmt = "SELECT id_AuditPerformanceChecklist,idt_checklist,id_element,id_auditratings,"
        //                + "comment FROM t_auditperformancechecklist where idt_checklist='"
        //                    + objAudit.idt_checklist + "'";

        //                DataSet dsAuditElement = objGlobaldata.Getdetails(sSqlstmt);
        //                ViewBag.AuditElement = dsAuditElement;
        //                AuditElementsModels obj = new AuditElementsModels();



        //                ViewBag.AuditRating = obj.GetAuditRating();
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = "No data exists";
        //                return RedirectToAction("AuditChecklistList");
        //            }
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = "Id cannot be null";
        //            return RedirectToAction("AuditChecklistList");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AuditChecklistReport: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

        //    foreach (var key in Request.Cookies.AllKeys)
        //    {
        //        cookieCollection.Add(key, Request.Cookies.Get(key).Value);
        //    }
        //    string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

        //    return new ViewAsPdf("AuditChecklistReportToPDF")
        //    {
        //        FileName = "AuditChecklistReport.pdf",
        //        Cookies = cookieCollection,
        //        CustomSwitches = footer
        //    };
        //}

        //End Generate Checklist

        public ActionResult AddAuditElements(string sDepartment,string sDivision,string sLocation,string sIsoStd)
        {
            try
            {
                AuditElementsModels obj = new AuditElementsModels();
                ////ViewBag.Department = objGlobaldata.GetDepartmentListbox();
               
                if (sDepartment != null && sDepartment != "" && sDivision != null && sDivision != "" && sLocation != null && sLocation != "" && sIsoStd != null && sIsoStd != "")
                {
                    ViewBag.Department_Id = sDepartment;
                    ViewBag.AuditElements = obj.GetAuditElementsListboxByIsoStd(sDepartment, sIsoStd);
                    ViewBag.division_Id = sDivision;
                    ViewBag.location_Id = sLocation;
                    ViewBag.sIsoStd_Id = sIsoStd;

                    ViewBag.Department = objGlobaldata.GetDepartmentListbox(sDivision);
                    ViewBag.location = objGlobaldata.GetDivisionLocationList(sDivision);
                    ViewBag.division = objGlobaldata.GetCompanyBranchListbox();
                    ViewBag.IsoStd = objGlobaldata.GetIsoStdListbox();

                }
                else
                {
                    ViewBag.Department_Id = objGlobaldata.GetCurrentUserSession().DeptID;
                    ////ViewBag.AuditElements = obj.GetAuditElementsListbox(sDepartment);

                    ViewBag.division_Id = objGlobaldata.GetCurrentUserSession().division;
                    ViewBag.location_Id = objGlobaldata.GetCurrentUserSession().Work_Location;
                    ViewBag.Department = objGlobaldata.GetDepartmentListbox(objGlobaldata.GetCurrentUserSession().division);
                    ViewBag.location = objGlobaldata.GetDivisionLocationList(objGlobaldata.GetCurrentUserSession().division);
                    ViewBag.division = objGlobaldata.GetCompanyBranchListbox();
                    ViewBag.IsoStd = objGlobaldata.GetIsoStdListbox();
                }
            }
            catch(Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditElements: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
           
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAuditElements(AuditElementsModels objElementModels, FormCollection form)
        {
            AuditElementsModels obj = new AuditElementsModels();
            try
            {
                ViewBag.Department_Id = objElementModels.Department;
                ViewBag.division_Id = objElementModels.division;
                ViewBag.location_Id = objElementModels.location;
                ViewBag.sIsoStd_Id = objElementModels.IsoStd;

                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objElementModels.division);
                ViewBag.location = objGlobaldata.GetDivisionLocationList(objElementModels.division);
                ViewBag.division = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.IsoStd = objGlobaldata.GetIsoStdListbox();


                if (objElementModels.FunAddAuditElements(objElementModels))
                {
                    TempData["Successdata"] = "Added Questions added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                ViewBag.AuditElements = obj.GetAuditElementsListboxByIsoStd(objElementModels.Department, objElementModels.IsoStd);

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
            AuditElementsModels obj = new AuditElementsModels();
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
            AuditElementsModels obj = new AuditElementsModels();
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
                            GenerateAuditChecklistModels Doc = new GenerateAuditChecklistModels();
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
                   
        public ActionResult AuditQuestionDelete(string id_element, string Department)
        {

            AuditElementsModels obj = new AuditElementsModels();
            try
            {

                if (obj.FunDeleteQuestions(id_element))
                {
                    TempData["Successdata"] = "Question deleted successfully";
                    return RedirectToAction("AddAuditElements", new { sDepartment = Department });
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

            return RedirectToAction("AddAuditElements");
        }
                   
        public JsonResult AuditQuestionDelete1(string id_element, string Department)
        {

            AuditElementsModels obj = new AuditElementsModels();
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
            GenerateAuditChecklistModels objAuditChecklistModels = new GenerateAuditChecklistModels();
           
            try
            {
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                AuditElementsModels obj = new AuditElementsModels();
                ViewBag.AuditNo = obj.GetAuditNoListbox();
                ViewBag.AuditRating = obj.GetAuditRating();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                if (Request.QueryString["id_AuditChecklist"] != null && Request.QueryString["id_AuditChecklist"] != "")
                {
                    string sid_AuditChecklist = Request.QueryString["id_AuditChecklist"];


                    string sSqlstmt = "select id_AuditChecklist,Itemno,Department,AuditCriteria,Questions from t_auditchecklist where id_AuditChecklist='" + sid_AuditChecklist + "'";

                    DataSet dsChecklistModelsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsChecklistModelsList.Tables.Count > 0 && dsChecklistModelsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsChecklistModelsList.Tables[0].Rows.Count; i++)
                        {

                            objAuditChecklistModels = new GenerateAuditChecklistModels
                            {
                                id_AuditChecklist = Convert.ToInt16(dsChecklistModelsList.Tables[0].Rows[i]["id_AuditChecklist"].ToString()),
                                Itemno = Convert.ToInt16(dsChecklistModelsList.Tables[0].Rows[i]["Itemno"].ToString()),
                                Department = dsChecklistModelsList.Tables[0].Rows[i]["Department"].ToString(),
                                AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsChecklistModelsList.Tables[0].Rows[i]["AuditCriteria"].ToString()),
                                Questions = dsChecklistModelsList.Tables[0].Rows[i]["Questions"].ToString(),

                            };
                            ViewBag.AuditElements = obj.GetAuditElementsListbox(dsChecklistModelsList.Tables[0].Rows[i]["Department"].ToString());
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
        public ActionResult PerformAudit(GenerateAuditChecklistModels objAuditChecklist, FormCollection form)
        {
            try
            {
                objAuditChecklist.id_AuditChecklist = Convert.ToInt32(form["id_AuditChecklist"]);
                objAuditChecklist.AuditNo = form["AuditNo"];
                objAuditChecklist.Auditors = form["Auditors"];
                objAuditChecklist.Auditee = form["Auditee"];
                objAuditChecklist.Notes = form["Notes"];
                objAuditChecklist.Remarks = form["Remarks"];
                objAuditChecklist.Department = form["Department"];

                //string dept = "";
                //string sSqlstmt = "select Department from t_generateauditchecklist where idt_checklist='" + objAuditChecklist.idt_checklist + "'";
                //DataSet dsChecklistModelsList = objGlobaldata.Getdetails(sSqlstmt);
                //if (dsChecklistModelsList.Tables.Count > 0 && dsChecklistModelsList.Tables[0].Rows.Count > 0)
                //{
                //    for (int j = 0; j < dsChecklistModelsList.Tables[0].Rows.Count; j++)
                //    {

                //        //GenerateAuditChecklistModels objAuditChecklistModels = new GenerateAuditChecklistModels
                //        //{
                //        //   Department = dsChecklistModelsList.Tables[0].Rows[i]["Department"].ToString(),
                //        //};        
                //        dept = dsChecklistModelsList.Tables[0].Rows[j]["Department"].ToString();
                //    }
                //}


                DateTime dateValue;
                if (DateTime.TryParse(form["AuditDate"], out dateValue) == true)
                {
                    objAuditChecklist.AuditDate = dateValue;
                }

                AuditPerformanceModelsList objAudit = new AuditPerformanceModelsList();
                objAudit.lstAudit = new List<AuditPerformanceModels>();

                AuditElementsModels obj = new AuditElementsModels();
                MultiSelectList AuditQuestions = obj.GetAuditElementsListbox(objAuditChecklist.Department);
                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in AuditQuestions)
                {
                    if (i <= cnt)
                    {
                        AuditPerformanceModels objElements = new AuditPerformanceModels();
                        objElements.id_element = form["Audit_Elements " + item.Value];
                        objElements.id_auditratings = form["id_auditratings " + item.Value];
                        objElements.comment = form["comment " + i];
                        objElements.evidence_upload = form["evidence_upload" + i];
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
        public ActionResult PerformAuditReport(FormCollection form)
        {
           
            string id_AuditChecklist = form["id_AuditChecklist"];
            string sDepartment = form["Department"];
            try
            {
                
                if (id_AuditChecklist != null )
                {
                    GenerateAuditChecklistModels objAuditChecklistModels = new GenerateAuditChecklistModels();
                    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                    AuditElementsModels obj = new AuditElementsModels();
                    ViewBag.AuditNo = obj.GetAuditNoListbox();
                    ViewBag.AuditRating = obj.GetAuditRating();
                    ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                    ViewBag.AuditElements = obj.GetAuditElementsListbox(sDepartment);

                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("PerformAudit");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("PerformAuditReport")
            {
                FileName = "PerformAuditReport.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase Evidence_upload = Request.Files;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "Evidence" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/Audit/" + "Evidence" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
            }
            return Json("Failed");
        }
              
        [HttpPost]
        public JsonResult GetQuestions(string Department,string IsoStd)
        {
            AuditElementsModels obj = new AuditElementsModels();

            //var data = obj.GetAuditElementsListbox(Department);
            var data = obj.GetAuditElementsListboxByIsoStd(Department,IsoStd);
            return Json(data);
        }
                
        [AllowAnonymous]
        public ActionResult AuditList()
        {
            GenerateAuditChecklistModelsList objAuditChecklist = new GenerateAuditChecklistModelsList();
            objAuditChecklist.AuditCheckList = new List<GenerateAuditChecklistModels>();

            try
            {

                if (Request.QueryString["id_AuditChecklist"] != null && Request.QueryString["id_AuditChecklist"] != "")
                {

                    string sid_AuditChecklist = Request.QueryString["id_AuditChecklist"];
                    string sSqlstmt = "select idt_checklist,id_AuditChecklist,AuditNo,AuditDate,Auditors,Auditee from t_generateauditchecklist where id_AuditChecklist='" + sid_AuditChecklist + "' and Active=1";

                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        
                           
                        
                        for (int i = 0; i < dsAudit.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                GenerateAuditChecklistModels objAudit = new GenerateAuditChecklistModels
                                {
                                    idt_checklist = Convert.ToInt16(dsAudit.Tables[0].Rows[i]["idt_checklist"].ToString()),
                                    id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[i]["id_AuditChecklist"].ToString()),
                                    AuditNo = objGlobaldata.GetAuditNoById(dsAudit.Tables[0].Rows[i]["AuditNo"].ToString()),
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

            return View(objAuditChecklist.AuditCheckList.ToList());
        }
                
        [AllowAnonymous]
        public ActionResult AuditPerformanceEdit()
        {

            try
            {
                if (Request.QueryString["idt_checklist"] != null && Request.QueryString["idt_checklist"] != "")
                {
                    string sidt_checklist = Request.QueryString["idt_checklist"];

                    string sSqlstmt = "select t.id_AuditChecklist,Itemno,Department,AuditCriteria,Questions,idt_checklist,AuditNo,"
                    + "AuditDate,Auditors,Auditee,Notes,Remarks from t_auditchecklist t,t_generateauditchecklist tt where t.id_AuditChecklist=tt.id_AuditChecklist"
                    + " and idt_checklist='" + sidt_checklist + "'";


                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        GenerateAuditChecklistModels objPerformance = new GenerateAuditChecklistModels
                        {
                            idt_checklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["idt_checklist"].ToString()),
                            id_AuditChecklist = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["id_AuditChecklist"].ToString()),
                            Itemno = Convert.ToInt16(dsAudit.Tables[0].Rows[0]["Itemno"].ToString()),
                            Department = objGlobaldata.GetDeptNameById(dsAudit.Tables[0].Rows[0]["Department"].ToString()),
                            AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsAudit.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                            AuditNo = objGlobaldata.GetAuditNoById(dsAudit.Tables[0].Rows[0]["AuditNo"].ToString()),
                            Auditors = dsAudit.Tables[0].Rows[0]["Auditors"].ToString(),
                            Auditee = (dsAudit.Tables[0].Rows[0]["Auditee"].ToString()),
                            Notes = dsAudit.Tables[0].Rows[0]["Notes"].ToString(),
                            Remarks = dsAudit.Tables[0].Rows[0]["Remarks"].ToString(),
                        };
                        string dept = dsAudit.Tables[0].Rows[0]["Department"].ToString();
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAudit.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                        {
                            objPerformance.AuditDate = dtValue;
                        }

                        sSqlstmt = "SELECT id_AuditPerformanceChecklist,idt_checklist,id_element,id_auditratings,"
                       + "comment,evidence_upload FROM t_auditperformancechecklist where idt_checklist='"
                           + objPerformance.idt_checklist + "'";

                        DataSet dsAuditElement = objGlobaldata.Getdetails(sSqlstmt);

                        AuditPerformanceModelsList objAuditPerformanceList = new AuditPerformanceModelsList();
                        objAuditPerformanceList.lstAudit = new List<AuditPerformanceModels>();

                        AuditElementsModels obj = new AuditElementsModels();

                        Dictionary<string, string> dicPerformanceElements = new Dictionary<string, string>();

                        for (int i = 0; dsAuditElement.Tables.Count > 0 && i < dsAuditElement.Tables[0].Rows.Count; i++)
                        {
                            ViewBag.dsAudit = dsAuditElement;
                        }

                        for (int i = 0; dsAuditElement.Tables.Count > 0 && i < dsAuditElement.Tables[0].Rows.Count; i++)
                        {
                            AuditPerformanceModels objElements = new AuditPerformanceModels
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
                        ViewBag.AuditNo = obj.GetAuditNoListbox();
                        ViewBag.AuditQuestions = obj.GetAuditElementsListbox(dept);
                        ViewBag.AuditRating = obj.GetAuditRating();
                        ViewBag.Department = objGlobaldata.GetDepartmentListbox();
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
        public ActionResult AuditPerformanceEdit(GenerateAuditChecklistModels objAuditChecklist, FormCollection form)
        {
            try
            {
                objAuditChecklist.id_AuditChecklist = Convert.ToInt32(form["id_AuditChecklist"]);
                objAuditChecklist.AuditNo = form["AuditNo"];
                objAuditChecklist.Auditors = form["Auditors"];
                objAuditChecklist.Auditee = form["Auditee"];
                objAuditChecklist.Notes = form["Notes"];
                objAuditChecklist.Remarks = form["Remarks"];
                objAuditChecklist.Department = form["Department"];



                DateTime dateValue;
                if (DateTime.TryParse(form["AuditDate"], out dateValue) == true)
                {
                    objAuditChecklist.AuditDate = dateValue;
                }

                AuditPerformanceModelsList objAudit = new AuditPerformanceModelsList();
                objAudit.lstAudit = new List<AuditPerformanceModels>();

                AuditElementsModels obj = new AuditElementsModels();
                MultiSelectList AuditQuestions = obj.GetAuditElementsListbox(objAuditChecklist.Department);
                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in AuditQuestions)
                {
                    if (i <= Convert.ToInt16(form["itmctn"]))
                    {
                        AuditPerformanceModels objElements = new AuditPerformanceModels();
                        string id = form["id_AuditPerformanceChecklist " + i];
                        objElements.id_AuditPerformanceChecklist =Convert.ToInt32(id);
                        objElements.id_element = form["Audit_Elements " + item.Value];
                        objElements.id_auditratings = form["id_auditratings " + item.Value];
                        objElements.comment = form["comment " + i];        
                       string upload = form["evidence_upload" + i];
                       if (upload!=null)
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
                            GenerateAuditChecklistModels Doc = new GenerateAuditChecklistModels();
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

        public JsonResult FungetAuditDetail(string AuditNo,string dept)
        {
            InternalAuditModels objPerf = new InternalAuditModels();
            if (AuditNo != "")
            {
                string sql = "select Auditee,Auditor from  t_internal_audit t,t_internal_audit_trans tt"
                + " where t.AuditID=tt.AuditID and t.AuditID='" + AuditNo + "' and DeptID='"+dept+"'";
                DataSet dsData = objGlobaldata.Getdetails(sql);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    objPerf = new InternalAuditModels()
                    {
                        Auditee = objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["Auditee"].ToString()),
                        Auditor = objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["Auditor"].ToString()),
                    };


                }
            }
            return Json(objPerf);
        }

        //[AllowAnonymous]
        //public JsonResult GetDocLevelGroup(string DocLevelId)
        //{
        //    return Json(objGlobaldata.GetDocLevelGroupType(DocLevelId));
        //}

        //[AllowAnonymous]
        //public JsonResult GetDocLevelTeam(string DocLevelId)
        //{
        //    return Json(objGlobaldata.GetDocLevelTeamType(DocLevelId));
        //}
    }
}