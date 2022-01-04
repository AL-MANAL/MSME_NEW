using ISOStd.Models;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    public class InspectionController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public InspectionController()
        {
            ViewBag.Menutype = "HSE";
            //ViewBag.SubMenutype = "Inspection";
        }

        // Department section

        public ActionResult AddInspectionSection(string sDepartment)
        {
            ViewBag.SubMenutype = "AddInspectionSection";
            try
            {
                InspectionCategoryModels obj = new InspectionCategoryModels();
                InspctionQuestionModels objQst = new InspctionQuestionModels();
                string branch = objGlobaldata.GetCurrentUserSession().division;
                //ViewBag.Category = obj.GetInspectionCategoryListbox();
                //ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                //ViewBag.Branch_name = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(branch);

                if (sDepartment != null && sDepartment != "")
                {
                    ViewBag.Dept_Id = sDepartment;
                    //ViewBag.CategorySection = objQst.GetInspectionSectionListbox(sCategory, ViewBag.Branch_name);
                    //ViewBag.CategoryRespPerson= obj.GetInspRespPersonList(sCategory, ViewBag.Branch_name);
                    ViewBag.dsItems = objGlobaldata.Getdetails("select id_inspection, Section, dept from t_inspection_section where dept='" + sDepartment + "' and active=1");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInspectionSection: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInspectionSection(InspctionQuestionModels objInspModels, FormCollection form)
        {
            InspectionCategoryModels obj = new InspectionCategoryModels();
            InspctionQuestionModels objQst = new InspctionQuestionModels();
            try
            {
                string branch = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(branch);
                ViewBag.Dept_Id = objInspModels.dept;
                // ViewBag.Category_Id = objInspModels.Category;
                // ViewBag.Category = obj.GetInspectionCategoryListbox();
                // ViewBag.Branch_name = objGlobaldata.GetCurrentUserSession().division;

                // objInspModels.Resp_person = form["Resp_person"];

                if (objQst.FunAddInspectionSection(objInspModels))
                {
                    TempData["Successdata"] = "Added Ratings successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                ViewBag.dsItems = objGlobaldata.Getdetails("select id_inspection, Section, dept from t_inspection_section where dept='" + objInspModels.dept + "' and active=1");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInspectionSection: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            // ViewBag.CategorySection = objQst.GetInspectionSectionListbox(objInspModels.Category, ViewBag.Branch_name);
            //ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

            return View(objInspModels);
        }

        public JsonResult InspectionSectionDelete1(string id_inspection)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();

            try
            {
                if (obj.FunDeleteSection(id_inspection))
                {
                    TempData["Successdata"] = "Insepction Section deleted successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionSectionDelete2: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }

        public JsonResult InspectionSectionUpdate(int id_inspection, string Section, string[] dept)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();
            try
            {
                var sdept = "";
                if (dept != null)
                {
                    sdept = string.Join(",", dept);
                }

                //int id_inspections = Convert.ToInt32(id_inspection);
                if (obj.FunUpdateInspectionSection(id_inspection, Section, sdept))
                {
                    TempData["Successdata"] = "Inspection Section updated successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionSectionUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }

        [HttpPost]
        public JsonResult FunGetSectionRespPerson(string dept)
        {
            InspectionCategoryModels obj = new InspectionCategoryModels();
            InspctionQuestionModels objMdl = new InspctionQuestionModels();
            var data = new object();
            data = obj.GetInspSectionRespPersonList(dept);
            //string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
            //if (branch_name != "" && branch_name != null)
            //{
            //    data = obj.GetInspSectionRespPersonList(Category, branch_name);
            //    //ViewBag.Resp_person = obj.newfucntion(Category, branch_name);
            //}
            //else
            //{
            //    data = obj.GetInspSectionRespPersonList(Category, sBranch_name);
            //    //ViewBag.Resp_person = obj.newfucntion(Category, sBranch_name);
            //}
            return Json(data);
        }

        public ActionResult FunGetDeptSectionList(string dept)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            MultiSelectList lstEmp = new MultiSelectList(emplist.EmpList, "Value", "Text");
            try
            {
                lstEmp = objGlobaldata.FunGetDeptSectionList(dept);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetDeptSectionList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(lstEmp);
        }

        // Inspection question
        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddInspectionQuestions()
        {
            try
            {
                string branch = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.InspType = objGlobaldata.GetDropdownList("Inspection Type");
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(branch);
                ViewBag.InspArea = objGlobaldata.GetDropdownList("Inspection Area");
                ViewBag.InspCriteria = objGlobaldata.GetDropdownList("Inspection Criteria");
                ViewBag.Criticality = objGlobaldata.GetDropdownList("Inspection Criticality");
                ViewBag.AuditCriteria = objGlobaldata.GetIsoStdListbox();
                ViewBag.Reviewer = objGlobaldata.GetReviewer();
                ViewBag.Approver = objGlobaldata.GetApprover();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInspectionQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddInspectionQuestions(InspctionQuestionModels objModel, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                objModel.insp_criteria = form["insp_criteria"];
                objModel.Section = form["Section"];
                if (DateTime.TryParse(form["logged_date"], out dateValue) == true)
                {
                    objModel.logged_date = dateValue;
                }

                InspctionQuestionList objInsp = new InspctionQuestionList();
                objInsp.InspectionQstList = new List<InspctionQuestionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    InspctionQuestionModels obj = new InspctionQuestionModels();

                    if (form["InspectionQuestions" + i] != "" && form["InspectionQuestions" + i] != null)
                    {
                        obj.InspectionQuestions = form["InspectionQuestions" + i];
                        obj.criticality = form["criticality" + i];
                        obj.criteria = form["criteria" + i];

                        objInsp.InspectionQstList.Add(obj);
                    }
                }
                if (objModel.FunAddInspectionQuestion(objModel, objInsp))
                {
                    TempData["Successdata"] = "Inspection Checklist Generated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInspectionQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("GenerateInspChecklist");
        }

        [AllowAnonymous]
        public ActionResult EditInspectionQuestions()
        {
            InspctionQuestionModels objModel = new InspctionQuestionModels();

            try
            {
                if (Request.QueryString["id_question_master"] != null && Request.QueryString["id_question_master"] != "")
                {
                    string id_question_master = Request.QueryString["id_question_master"];
                    string sSqlstmt = "select id_question_master,branch,checklist_ref,insp_type,insp_detail,dept,Section,insp_area,insp_criteria,logged_date,reviewed_by,approved_by"

                        + " from t_inspection_question_master where id_question_master = '" + id_question_master + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new InspctionQuestionModels
                        {
                            branch = dsList.Tables[0].Rows[0]["branch"].ToString(),
                            id_question_master = dsList.Tables[0].Rows[0]["id_question_master"].ToString(),
                            checklist_ref = dsList.Tables[0].Rows[0]["checklist_ref"].ToString(),
                            insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()),
                            insp_detail = dsList.Tables[0].Rows[0]["insp_detail"].ToString(),
                            dept = (dsList.Tables[0].Rows[0]["dept"].ToString()),
                            Section = (dsList.Tables[0].Rows[0]["Section"].ToString()),
                            insp_area = (dsList.Tables[0].Rows[0]["insp_area"].ToString()),
                            insp_criteria = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_criteria"].ToString()),

                            reviewed_by = (dsList.Tables[0].Rows[0]["reviewed_by"].ToString()),

                            approved_by = (dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["logged_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                    }

                    ViewBag.InspType = objGlobaldata.GetDropdownList("Inspection Type");
                    ViewBag.Department = objGlobaldata.GetDepartmentListbox(objModel.branch);
                    ViewBag.InspArea = objGlobaldata.GetDropdownList("Inspection Area");
                    ViewBag.InspCriteria = objGlobaldata.GetDropdownList("Inspection Criteria");
                    ViewBag.Criticality = objGlobaldata.GetDropdownList("Inspection Criticality");
                    ViewBag.AuditCriteria = objGlobaldata.GetIsoStdListbox();
                    ViewBag.Reviewer = objGlobaldata.GetReviewer();
                    ViewBag.Approver = objGlobaldata.GetApprover();
                    ViewBag.Section = objGlobaldata.FunGetDeptSectionList(objModel.dept);
                    // Checklist questions

                    InspctionQuestionList objInsp = new InspctionQuestionList();
                    objInsp.InspectionQstList = new List<InspctionQuestionModels>();

                    string sSqlstmt1 = "select id_inspection_question,id_question_master,InspectionQuestions,criticality,criteria from t_inspection_questions where id_question_master = '" + id_question_master + "'";
                    DataSet dsChklist = objGlobaldata.Getdetails(sSqlstmt1);
                    if (dsChklist.Tables.Count > 0 && dsChklist.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsChklist.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                InspctionQuestionModels objAuditModel = new InspctionQuestionModels
                                {
                                    id_inspection_question = dsChklist.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                                    id_question_master = dsChklist.Tables[0].Rows[i]["id_question_master"].ToString(),
                                    InspectionQuestions = dsChklist.Tables[0].Rows[i]["InspectionQuestions"].ToString(),
                                    criticality = dsChklist.Tables[0].Rows[i]["criticality"].ToString(),
                                    criteria = dsChklist.Tables[0].Rows[i]["criteria"].ToString(),
                                };

                                objInsp.InspectionQstList.Add(objAuditModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in EditInspectionQuestions: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("GenerateInspChecklist");
                            }
                        }
                        ViewBag.objList = objInsp;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EditInspectionQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditInspectionQuestions(InspctionQuestionModels objModel, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                objModel.insp_criteria = form["insp_criteria"];
                objModel.Section = form["Section"];
                if (DateTime.TryParse(form["logged_date"], out dateValue) == true)
                {
                    objModel.logged_date = dateValue;
                }

                InspctionQuestionList objInsp = new InspctionQuestionList();
                objInsp.InspectionQstList = new List<InspctionQuestionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    InspctionQuestionModels obj = new InspctionQuestionModels();

                    if (form["InspectionQuestions" + i] != "" && form["InspectionQuestions" + i] != null)
                    {
                        obj.id_inspection_question = form["id_inspection_question" + i];
                        obj.InspectionQuestions = form["InspectionQuestions" + i];
                        obj.criticality = form["criticality" + i];
                        obj.criteria = form["criteria" + i];

                        objInsp.InspectionQstList.Add(obj);
                    }
                }
                if (objModel.FunEditInspectionQuestion(objModel, objInsp))
                {
                    TempData["Successdata"] = "Inspection Checklist Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EditInspectionQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("GenerateInspChecklist");
        }

        [AllowAnonymous]
        public ActionResult GenerateInspChecklist(string branch_name)
        {
            ViewBag.SubMenutype = "GenerateInspChecklist";
            InspctionQuestionList objInsp = new InspctionQuestionList();
            objInsp.InspectionQstList = new List<InspctionQuestionModels>();

            InspectionCategoryModels objInspModel = new InspectionCategoryModels();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_question_master,checklist_ref,insp_type,dept,Section,insp_area,insp_criteria," +
                 "(CASE WHEN checklist_status = '0' THEN 'Draft' WHEN checklist_status = '1' THEN 'Reviewer Rejected' WHEN checklist_status = '2' THEN 'Reviewed' WHEN checklist_status = '3' THEN 'Approver Rejected' ELSE 'Approved' END) as checklist_status" +
                    " from t_inspection_question_master where active = 1 and chk_valid='Valid'";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                    ViewBag.Branch_name = sBranch_name;
                }

                sSqlstmt = sSqlstmt + " order by id_question_master desc";

                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            InspctionQuestionModels objModel = new InspctionQuestionModels
                            {
                                id_question_master = dsList.Tables[0].Rows[i]["id_question_master"].ToString(),
                                checklist_ref = dsList.Tables[0].Rows[i]["checklist_ref"].ToString(),
                                insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_type"].ToString()),
                                dept = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[i]["dept"].ToString()),
                                Section = objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[i]["Section"].ToString()),
                                insp_area = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_area"].ToString()),
                                insp_criteria = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_criteria"].ToString()),
                                checklist_status = dsList.Tables[0].Rows[i]["checklist_status"].ToString(),
                            };
                            objInsp.InspectionQstList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in GenerateInspChecklist: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateInspChecklist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objInsp.InspectionQstList.ToList());
        }

        [AllowAnonymous]
        public ActionResult GenerateInspChecklistHistory()
        {
            InspctionQuestionList objInsp = new InspctionQuestionList();
            objInsp.InspectionQstList = new List<InspctionQuestionModels>();

            if (Request.QueryString["id_question_master"] != null && Request.QueryString["id_question_master"] != "")
            {
                string id_question_master = Request.QueryString["id_question_master"];

                InspectionCategoryModels objInspModel = new InspectionCategoryModels();

                try
                {
                    string sSqlstmt = "select id_question_master_history,id_question_master,checklist_ref,insp_type,dept,Section,insp_area,insp_criteria," +
                     "(CASE WHEN checklist_status = '0' THEN 'Draft' WHEN checklist_status = '1' THEN 'Reviewer Rejected' WHEN checklist_status = '2' THEN 'Reviewed' WHEN checklist_status = '3' THEN 'Approver Rejected' ELSE 'Approved' END) as checklist_status" +
                        " from t_inspection_question_master_history where id_question_master='" + id_question_master + "'";

                    sSqlstmt = sSqlstmt + " order by id_question_master_history desc";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                InspctionQuestionModels objModel = new InspctionQuestionModels
                                {
                                    id_question_master_history = dsList.Tables[0].Rows[i]["id_question_master_history"].ToString(),
                                    id_question_master = dsList.Tables[0].Rows[i]["id_question_master"].ToString(),
                                    checklist_ref = dsList.Tables[0].Rows[i]["checklist_ref"].ToString(),
                                    insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_type"].ToString()),
                                    dept = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[i]["dept"].ToString()),
                                    Section = objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[i]["Section"].ToString()),
                                    insp_area = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_area"].ToString()),
                                    insp_criteria = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_criteria"].ToString()),
                                    checklist_status = dsList.Tables[0].Rows[i]["checklist_status"].ToString(),
                                };
                                objInsp.InspectionQstList.Add(objModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in GenerateInspChecklistHistory: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("GenerateInspChecklist");
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in GenerateInspChecklistHistory: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            return View(objInsp.InspectionQstList.ToList());
        }

        [AllowAnonymous]
        public ActionResult InspectionQuestionsRev()
        {
            InspctionQuestionModels objModel = new InspctionQuestionModels();
            try
            {
                if (Request.QueryString["id_question_master"] != null && Request.QueryString["id_question_master"] != "")
                {
                    string id_question_master = Request.QueryString["id_question_master"];
                    string sSqlstmt = "select id_question_master,branch,checklist_ref,insp_type,insp_detail,dept,Section,insp_area,insp_criteria,logged_date,reviewed_by,approved_by,RevNo"

                        + " from t_inspection_question_master where id_question_master = '" + id_question_master + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new InspctionQuestionModels
                        {
                            branch = dsList.Tables[0].Rows[0]["branch"].ToString(),
                            id_question_master = dsList.Tables[0].Rows[0]["id_question_master"].ToString(),
                            checklist_ref = dsList.Tables[0].Rows[0]["checklist_ref"].ToString(),
                            insp_type = (dsList.Tables[0].Rows[0]["insp_type"].ToString()),
                            insp_detail = dsList.Tables[0].Rows[0]["insp_detail"].ToString(),
                            dept = (dsList.Tables[0].Rows[0]["dept"].ToString()),
                            Section = (dsList.Tables[0].Rows[0]["Section"].ToString()),
                            insp_area = (dsList.Tables[0].Rows[0]["insp_area"].ToString()),
                            insp_criteria = (dsList.Tables[0].Rows[0]["insp_criteria"].ToString()),

                            reviewed_by = (dsList.Tables[0].Rows[0]["reviewed_by"].ToString()),

                            approved_by = (dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                        };

                        ViewBag.RevNo = Convert.ToInt32(dsList.Tables[0].Rows[0]["RevNo"].ToString()) + 1;

                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["logged_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                    }

                    ViewBag.InspType = objGlobaldata.GetDropdownList("Inspection Type");
                    ViewBag.Department = objGlobaldata.GetDepartmentListbox(objModel.branch);
                    ViewBag.InspArea = objGlobaldata.GetDropdownList("Inspection Area");
                    ViewBag.InspCriteria = objGlobaldata.GetDropdownList("Inspection Criteria");
                    ViewBag.Criticality = objGlobaldata.GetDropdownList("Inspection Criticality");
                    ViewBag.AuditCriteria = objGlobaldata.GetIsoStdListbox();
                    ViewBag.Reviewer = objGlobaldata.GetReviewer();
                    ViewBag.Approver = objGlobaldata.GetApprover();
                    ViewBag.Section = objGlobaldata.FunGetDeptSectionList(objModel.dept);

                    // Checklist questions

                    InspctionQuestionList objInsp = new InspctionQuestionList();
                    objInsp.InspectionQstList = new List<InspctionQuestionModels>();

                    string sSqlstmt1 = "select id_inspection_question,id_question_master,InspectionQuestions,criticality,criteria from t_inspection_questions where id_question_master = '" + id_question_master + "'";
                    DataSet dsChklist = objGlobaldata.Getdetails(sSqlstmt1);
                    if (dsChklist.Tables.Count > 0 && dsChklist.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsChklist.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                InspctionQuestionModels objAuditModel = new InspctionQuestionModels
                                {
                                    id_inspection_question = dsChklist.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                                    id_question_master = dsChklist.Tables[0].Rows[i]["id_question_master"].ToString(),
                                    InspectionQuestions = dsChklist.Tables[0].Rows[i]["InspectionQuestions"].ToString(),
                                    criticality = dsChklist.Tables[0].Rows[i]["criticality"].ToString(),
                                    criteria = dsChklist.Tables[0].Rows[i]["criteria"].ToString(),
                                };

                                objInsp.InspectionQstList.Add(objAuditModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in InspectionQuestionsRev: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("GenerateInspChecklist");
                            }
                        }
                        ViewBag.objList = objInsp;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionQuestionsRev: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult InspectionQuestionsRev(InspctionQuestionModels objModel, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                objModel.insp_criteria = form["insp_criteria"];
                objModel.Section = form["Section"];
                if (DateTime.TryParse(form["logged_date"], out dateValue) == true)
                {
                    objModel.logged_date = dateValue;
                }

                InspctionQuestionList objInsp = new InspctionQuestionList();
                objInsp.InspectionQstList = new List<InspctionQuestionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    InspctionQuestionModels obj = new InspctionQuestionModels();

                    if (form["InspectionQuestions" + i] != "" && form["InspectionQuestions" + i] != null)
                    {
                        obj.id_inspection_question = form["id_inspection_question" + i];
                        obj.InspectionQuestions = form["InspectionQuestions" + i];
                        obj.criticality = form["criticality" + i];
                        obj.criteria = form["criteria" + i];

                        objInsp.InspectionQstList.Add(obj);
                    }
                }
                if (objModel.FunRevInspectionQuestion(objModel, objInsp))
                {
                    TempData["Successdata"] = "Inspection checklist revise request generated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionQuestionsRev: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("GenerateInspChecklist");
        }

        [AllowAnonymous]
        public ActionResult InspChecklistApprovalDetails()
        {
            InspctionQuestionModels objModel = new InspctionQuestionModels();

            InspectionCategoryModels objInspModel = new InspectionCategoryModels();
            try
            {
                if (Request.QueryString["id_question_master"] != null && Request.QueryString["id_question_master"] != "")
                {
                    ViewBag.status = Request.QueryString["status"];
                    ViewBag.ReviewStatus = objGlobaldata.GetConstantValueKeyValuePair("InspChkReview");
                    ViewBag.ApproveStatus = objGlobaldata.GetConstantValueKeyValuePair("InspChkApprove");
                    string id_question_master = Request.QueryString["id_question_master"];
                    string sSqlstmt = "select id_question_master,checklist_ref,insp_type,insp_detail,dept,Section,insp_area,insp_criteria,logged_date,logged_by,reviewed_by,reviewed_date,reviewer_comments,reviewer_upload,approved_by,approved_date,approver_comments,approver_upload,RevNo,"
                       + "(CASE WHEN checklist_status = '0' THEN 'Draft' WHEN checklist_status = '1' THEN 'Reviewer Rejected' WHEN checklist_status = '2' THEN 'Reviewed' WHEN checklist_status = '3' THEN 'Approver Rejected' ELSE 'Approved' END) as checklist_status,"
                        + "(CASE WHEN reviewed_status = '2' THEN 'Reviewed' WHEN reviewed_status = '1' THEN 'Rejected' ELSE 'Pending for Review' END) as  reviewed_status,"
                     + "(CASE WHEN approved_status = '2' THEN 'Pending for Approve' WHEN approved_status = '3' THEN 'Rejected'  WHEN approved_status = '4' THEN 'Approved' END) as  approved_status"
                        + " from t_inspection_question_master where id_question_master = '" + id_question_master + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new InspctionQuestionModels
                        {
                            id_question_master = dsList.Tables[0].Rows[0]["id_question_master"].ToString(),
                            checklist_ref = dsList.Tables[0].Rows[0]["checklist_ref"].ToString(),
                            insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()),
                            insp_detail = dsList.Tables[0].Rows[0]["insp_detail"].ToString(),
                            dept = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()),
                            Section = objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[0]["Section"].ToString()),
                            insp_area = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_area"].ToString()),
                            insp_criteria = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_criteria"].ToString()),
                            checklist_status = dsList.Tables[0].Rows[0]["checklist_status"].ToString(),
                            logged_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["logged_by"].ToString()),
                            reviewed_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            reviewer_comments = dsList.Tables[0].Rows[0]["reviewer_comments"].ToString(),
                            reviewer_upload = dsList.Tables[0].Rows[0]["reviewer_upload"].ToString(),
                            approved_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            approver_comments = dsList.Tables[0].Rows[0]["approver_comments"].ToString(),
                            approver_upload = dsList.Tables[0].Rows[0]["approver_upload"].ToString(),
                            RevNo = dsList.Tables[0].Rows[0]["RevNo"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["logged_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["reviewed_date"].ToString() != ""
                                              && DateTime.TryParse(dsList.Tables[0].Rows[0]["reviewed_date"].ToString(), out dtDocDate))
                        {
                            objModel.reviewed_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["approved_date"].ToString() != ""
                                             && DateTime.TryParse(dsList.Tables[0].Rows[0]["approved_date"].ToString(), out dtDocDate))
                        {
                            objModel.approved_date = dtDocDate;
                        }
                    }

                    // Checklist questions
                    string sSqlstmt1 = "select id_inspection_question,id_question_master,InspectionQuestions,criticality,criteria from t_inspection_questions where id_question_master = '" + id_question_master + "'";
                    DataSet dsChklist = objGlobaldata.Getdetails(sSqlstmt1);
                    ViewBag.objChklist = dsChklist;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspChecklistApprovalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //checklis review
        public ActionResult InspChecklistReview(InspctionQuestionModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> reviewer_upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                if (reviewer_upload != null && files.ContentLength > 0)
                {
                    objModel.reviewer_upload = "";
                    foreach (var file in reviewer_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE_INSP"), Path.GetFileName(file.FileName));
                            string sFilename = "HSE_INSP" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.reviewer_upload = objModel.reviewer_upload + "," + "~/DataUpload/MgmtDocs/HSE_INSP/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in InspChecklistReview-upload: " + ex.ToString());
                        }
                    }
                    objModel.reviewer_upload = objModel.reviewer_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objModel.FunInspChecklistReview(objModel))
                {
                    TempData["Successdata"] = "Updated Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspChecklistReview: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        //checklis approve
        public ActionResult InspChecklistApprove(InspctionQuestionModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> approver_upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                if (approver_upload != null && files.ContentLength > 0)
                {
                    objModel.approver_upload = "";
                    foreach (var file in approver_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE_INSP"), Path.GetFileName(file.FileName));
                            string sFilename = "HSE_INSP" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.approver_upload = objModel.approver_upload + "," + "~/DataUpload/MgmtDocs/HSE_INSP/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in InspChecklistReview-upload: " + ex.ToString());
                        }
                    }
                    objModel.approver_upload = objModel.approver_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objModel.FunInspChecklistApprove(objModel))
                {
                    TempData["Successdata"] = "Updated Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspChecklistApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public JsonResult InspChecklistDelete(string id_question_master)
        {
            try
            {
                if (id_question_master != "")
                {
                    InspctionQuestionModels Doc = new InspctionQuestionModels();
                    string sid_question_master = id_question_master;
                    if (Doc.FunDeleteInspChecklist(sid_question_master))
                    {
                        TempData["Successdata"] = "Deleted successfully";
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspChecklistDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult InspChecklistInvalid(string id_question_master, string invalid_reason)
        {
            try
            {
                if (id_question_master != "" && invalid_reason != "")
                {
                    InspctionQuestionModels Doc = new InspctionQuestionModels();
                    string sid_question_master = id_question_master;
                    if (Doc.FunInvalidInspChecklist(sid_question_master, invalid_reason))
                    {
                        TempData["Successdata"] = "Updated successfully";
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspChecklistInvalid: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult InspChecklistDetails()
        {
            InspctionQuestionModels objModel = new InspctionQuestionModels();

            InspectionCategoryModels objInspModel = new InspectionCategoryModels();
            try
            {
                if (Request.QueryString["id_question_master"] != null && Request.QueryString["id_question_master"] != "")
                {
                    string id_question_master = Request.QueryString["id_question_master"];
                    string sSqlstmt = "select id_question_master,checklist_ref,insp_type,insp_detail,dept,Section,insp_area,insp_criteria,logged_date,logged_by,reviewed_by,reviewed_date,reviewer_comments,reviewer_upload,approved_by,approved_date,approver_comments,approver_upload,RevNo,"
                       + "(CASE WHEN checklist_status = '0' THEN 'Draft' WHEN checklist_status = '1' THEN 'Reviewer Rejected' WHEN checklist_status = '2' THEN 'Reviewed' WHEN checklist_status = '3' THEN 'Approver Rejected' ELSE 'Approved' END) as checklist_status,"
                        + "(CASE WHEN reviewed_status = '2' THEN 'Reviewed' WHEN reviewed_status = '1' THEN 'Rejected' ELSE 'Pending for Review' END) as  reviewed_status,"
                     + "(CASE WHEN approved_status = '2' THEN 'Pending for Approve' WHEN approved_status = '3' THEN 'Rejected'  WHEN approved_status = '4' THEN 'Approved' END) as  approved_status"
                        + " from t_inspection_question_master where id_question_master = '" + id_question_master + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new InspctionQuestionModels
                        {
                            id_question_master = dsList.Tables[0].Rows[0]["id_question_master"].ToString(),
                            checklist_ref = dsList.Tables[0].Rows[0]["checklist_ref"].ToString(),
                            insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()),
                            insp_detail = dsList.Tables[0].Rows[0]["insp_detail"].ToString(),
                            dept = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()),
                            Section = objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[0]["Section"].ToString()),
                            insp_area = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_area"].ToString()),
                            insp_criteria = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_criteria"].ToString()),
                            checklist_status = dsList.Tables[0].Rows[0]["checklist_status"].ToString(),
                            reviewed_status = dsList.Tables[0].Rows[0]["reviewed_status"].ToString(),
                            approved_status = dsList.Tables[0].Rows[0]["approved_status"].ToString(),
                            logged_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["logged_by"].ToString()),
                            reviewed_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            reviewer_comments = dsList.Tables[0].Rows[0]["reviewer_comments"].ToString(),
                            reviewer_upload = dsList.Tables[0].Rows[0]["reviewer_upload"].ToString(),
                            approved_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            approver_comments = dsList.Tables[0].Rows[0]["approver_comments"].ToString(),
                            approver_upload = dsList.Tables[0].Rows[0]["approver_upload"].ToString(),
                            RevNo = (dsList.Tables[0].Rows[0]["RevNo"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["logged_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["reviewed_date"].ToString() != ""
                                              && DateTime.TryParse(dsList.Tables[0].Rows[0]["reviewed_date"].ToString(), out dtDocDate))
                        {
                            objModel.reviewed_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["approved_date"].ToString() != ""
                                             && DateTime.TryParse(dsList.Tables[0].Rows[0]["approved_date"].ToString(), out dtDocDate))
                        {
                            objModel.approved_date = dtDocDate;
                        }
                    }

                    // Checklist questions
                    string sSqlstmt1 = "select id_inspection_question,id_question_master,InspectionQuestions,criticality,criteria from t_inspection_questions where id_question_master = '" + id_question_master + "'";
                    DataSet dsChklist = objGlobaldata.Getdetails(sSqlstmt1);
                    ViewBag.objChklist = dsChklist;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspChecklistDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [AllowAnonymous]
        public ActionResult InspChecklistDetailsHistory()
        {
            InspctionQuestionModels objModel = new InspctionQuestionModels();

            InspectionCategoryModels objInspModel = new InspectionCategoryModels();
            try
            {
                if (Request.QueryString["id_question_master_history"] != null && Request.QueryString["id_question_master_history"] != "")
                {
                    string id_question_master_history = Request.QueryString["id_question_master_history"];
                    string sSqlstmt = "select id_question_master,checklist_ref,insp_type,insp_detail,dept,Section,insp_area,insp_criteria,logged_date,logged_by,reviewed_by,reviewed_date,reviewer_comments,reviewer_upload,approved_by,approved_date,approver_comments,approver_upload,RevNo,"
                       + "(CASE WHEN checklist_status = '0' THEN 'Draft' WHEN checklist_status = '1' THEN 'Reviewer Rejected' WHEN checklist_status = '2' THEN 'Reviewed' WHEN checklist_status = '3' THEN 'Approver Rejected' ELSE 'Approved' END) as checklist_status,"
                        + "(CASE WHEN reviewed_status = '2' THEN 'Reviewed' WHEN reviewed_status = '1' THEN 'Rejected' ELSE 'Pending for Review' END) as  reviewed_status,"
                     + "(CASE WHEN approved_status = '2' THEN 'Pending for Approve' WHEN approved_status = '3' THEN 'Rejected'  WHEN approved_status = '4' THEN 'Approved' END) as  approved_status"
                        + " from t_inspection_question_master_history where id_question_master_history = '" + id_question_master_history + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new InspctionQuestionModels
                        {
                            id_question_master = dsList.Tables[0].Rows[0]["id_question_master"].ToString(),
                            checklist_ref = dsList.Tables[0].Rows[0]["checklist_ref"].ToString(),
                            insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()),
                            insp_detail = dsList.Tables[0].Rows[0]["insp_detail"].ToString(),
                            dept = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()),
                            Section = objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[0]["Section"].ToString()),
                            insp_area = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_area"].ToString()),
                            insp_criteria = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_criteria"].ToString()),
                            checklist_status = dsList.Tables[0].Rows[0]["checklist_status"].ToString(),
                            reviewed_status = dsList.Tables[0].Rows[0]["reviewed_status"].ToString(),
                            approved_status = dsList.Tables[0].Rows[0]["approved_status"].ToString(),
                            logged_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["logged_by"].ToString()),
                            reviewed_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            reviewer_comments = dsList.Tables[0].Rows[0]["reviewer_comments"].ToString(),
                            reviewer_upload = dsList.Tables[0].Rows[0]["reviewer_upload"].ToString(),
                            approved_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            approver_comments = dsList.Tables[0].Rows[0]["approver_comments"].ToString(),
                            approver_upload = dsList.Tables[0].Rows[0]["approver_upload"].ToString(),
                            RevNo = (dsList.Tables[0].Rows[0]["RevNo"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["logged_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["reviewed_date"].ToString() != ""
                                              && DateTime.TryParse(dsList.Tables[0].Rows[0]["reviewed_date"].ToString(), out dtDocDate))
                        {
                            objModel.reviewed_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["approved_date"].ToString() != ""
                                             && DateTime.TryParse(dsList.Tables[0].Rows[0]["approved_date"].ToString(), out dtDocDate))
                        {
                            objModel.approved_date = dtDocDate;
                        }
                    }

                    // Checklist questions
                    string sSqlstmt1 = "select InspectionQuestions,criticality,criteria from t_inspection_questions_history where id_question_master_history = '" + id_question_master_history + "'";
                    DataSet dsChklist = objGlobaldata.Getdetails(sSqlstmt1);
                    ViewBag.objChklist = dsChklist;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspChecklistDetailsHistory: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //Plan Inspection
        public ActionResult FunGetChecklistRefList(string insp_type)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            MultiSelectList lstEmp = new MultiSelectList(emplist.EmpList, "Value", "Text");
            try
            {
                lstEmp = objGlobaldata.FunGetChecklistRefList(insp_type);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetChecklistRefList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(lstEmp);
        }

        public ActionResult FunGetApproverList(string dept)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            MultiSelectList lstEmp = new MultiSelectList(emplist.EmpList, "Value", "Text");
            MultiSelectList lstEmp1 = new MultiSelectList(emplist.EmpList, "Value", "Text");
            MultiSelectList lstEmp2 = new MultiSelectList(emplist.EmpList, "Value", "Text");
            IEnumerable<SelectListItem> item = lstEmp.Select(C => new SelectListItem { Value = C.Value, Text = C.Text });

            try
            {
                lstEmp = objGlobaldata.GetHrQHSEEmployeeListbox(); // QHSE employees
                lstEmp1 = objGlobaldata.GetHrEmpListByDept(dept); // Department employees
                lstEmp2 = objGlobaldata.GetTopMgmtEmpListbox(); // top mgmt

                item = (lstEmp.Concat(lstEmp1)).Concat(lstEmp2);

                var distinct = item.GroupBy(l => new { l.Value, l.Text }).Select(d => new
                {
                    Value = d.Key.Value,
                    Text = d.Key.Text
                });
                return Json(distinct);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetApproverList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(item);
        }

        public MultiSelectList FunGetApproverMultiList(string dept)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            MultiSelectList lstEmp = new MultiSelectList(emplist.EmpList, "Value", "Text");
            MultiSelectList lstEmp1 = new MultiSelectList(emplist.EmpList, "Value", "Text");
            MultiSelectList lstEmp2 = new MultiSelectList(emplist.EmpList, "Value", "Text");
            IEnumerable<SelectListItem> item = lstEmp.Select(C => new SelectListItem { Value = C.Value, Text = C.Text });
            try
            {
                lstEmp = objGlobaldata.GetHrQHSEEmployeeListbox(); // QHSE employees
                lstEmp1 = objGlobaldata.GetHrEmpListByDept(dept); // Department employees
                lstEmp2 = objGlobaldata.GetTopMgmtEmpListbox(); // top mgmt

                item = (lstEmp.Concat(lstEmp1)).Concat(lstEmp2);
                var distinct = item.GroupBy(l => new { l.Value, l.Text }).Select(d => new
                {
                    Value = d.Key.Value,
                    Text = d.Key.Text
                });
                return new MultiSelectList(distinct, "Value", "Text");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetApproverMultiList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return new MultiSelectList(item, "Value", "Text");
        }

        public ActionResult FunGetPersRespList(string dept)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            MultiSelectList lstEmp = new MultiSelectList(emplist.EmpList, "Value", "Text");
            MultiSelectList lstEmp1 = new MultiSelectList(emplist.EmpList, "Value", "Text");
            IEnumerable<SelectListItem> item = lstEmp.Select(C => new SelectListItem { Value = C.Value, Text = C.Text });
            try
            {
                lstEmp = objGlobaldata.GetHrQHSEEmployeeListbox(); // QHSE employees
                lstEmp1 = objGlobaldata.GetHrEmpListByDept(dept); // Department employees

                item = (lstEmp.Concat(lstEmp1)).Concat(lstEmp1);
                var distinct = item.GroupBy(l => new { l.Value, l.Text }).Select(d => new
                {
                    Value = d.Key.Value,
                    Text = d.Key.Text
                });
                return Json(distinct);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetPersRespList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(item);
        }

        public MultiSelectList FunGetPersRespMultiList(string dept)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            MultiSelectList lstEmp = new MultiSelectList(emplist.EmpList, "Value", "Text");
            MultiSelectList lstEmp1 = new MultiSelectList(emplist.EmpList, "Value", "Text");
            IEnumerable<SelectListItem> item = lstEmp.Select(C => new SelectListItem { Value = C.Value, Text = C.Text });
            try
            {
                lstEmp = objGlobaldata.GetHrQHSEEmployeeListbox(); // QHSE employees
                lstEmp1 = objGlobaldata.GetHrEmpListByDept(dept); // Department employees

                item = (lstEmp.Concat(lstEmp1)).Concat(lstEmp1);
                var distinct = item.GroupBy(l => new { l.Value, l.Text }).Select(d => new
                {
                    Value = d.Key.Value,
                    Text = d.Key.Text
                });
                return new MultiSelectList(distinct, "Value", "Text");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetPersRespMultiList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return new MultiSelectList(item, "Value", "Text");
        }

        public JsonResult FunGetChecklistRefRevNo(string checklist_ref)
        {
            string result = "";
            try
            {
                string sql = "select RevNo from t_inspection_question_master where id_question_master='" + checklist_ref + "'";
                DataSet dsList = objGlobaldata.Getdetails(sql);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    result = dsList.Tables[0].Rows[0]["RevNo"].ToString();
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetChecklistRefRevNo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddPlanInspection()
        {
            try
            {
                string branch = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.InspType = objGlobaldata.GetDropdownList("Inspection Type");
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(branch);
                ViewBag.InspArea = objGlobaldata.GetDropdownList("Inspection Area");
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(branch);
                ViewBag.Frequency = objGlobaldata.GetDropdownList("Inspection Frequency");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPlanInspection: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddPlanInspection(InspctionQuestionModels objModel, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                objModel.insp_criteria = form["insp_criteria"];
                objModel.notified_to = form["notified_to"];
                objModel.Section = form["Section"];
                InspctionQuestionList objInsp = new InspctionQuestionList();
                objInsp.InspectionQstList = new List<InspctionQuestionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    InspctionQuestionModels obj = new InspctionQuestionModels();

                    if (form["insp_date" + i] != "" && form["insp_date" + i] != null)
                    {
                        if (DateTime.TryParse(form["insp_date" + i], out dateValue) == true)
                        {
                            obj.insp_date = dateValue;
                        }
                        obj.pers_resp = form["pers_resp" + i];

                        objInsp.InspectionQstList.Add(obj);
                    }
                }
                if (objModel.FunAddInspectionPlan(objModel, objInsp))
                {
                    TempData["Successdata"] = "Inspection plan added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPlanInspection: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("PlanInspectionlist");
        }

        [AllowAnonymous]
        public ActionResult EditPlanInspection()
        {
            InspctionQuestionModels objModel = new InspctionQuestionModels();
            try
            {
                if (Request.QueryString["id_inspection_plan"] != null && Request.QueryString["id_inspection_plan"] != "")
                {
                    string id_inspection_plan = Request.QueryString["id_inspection_plan"];

                    string sSqlstmt = "select id_inspection_plan,branch,checklist_ref,RevNo,insp_type,insp_detail,dept,Section,insp_area,location,insp_freq,project,approved_by,notified_to"

                        + " from t_inspection_plan where id_inspection_plan = '" + id_inspection_plan + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new InspctionQuestionModels
                        {
                            id_inspection_plan = dsList.Tables[0].Rows[0]["id_inspection_plan"].ToString(),
                            checklist_ref = objGlobaldata.GetInspectionChecklistRefNamebyRevNo(dsList.Tables[0].Rows[0]["checklist_ref"].ToString(), dsList.Tables[0].Rows[0]["RevNo"].ToString()),
                            insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()),
                            insp_detail = (dsList.Tables[0].Rows[0]["insp_detail"].ToString()),
                            dept = (dsList.Tables[0].Rows[0]["dept"].ToString()),
                            Section = (dsList.Tables[0].Rows[0]["Section"].ToString()),
                            insp_area = (dsList.Tables[0].Rows[0]["insp_area"].ToString()),
                            location = (dsList.Tables[0].Rows[0]["location"].ToString()),
                            insp_freq = (dsList.Tables[0].Rows[0]["insp_freq"].ToString()),
                            project = (dsList.Tables[0].Rows[0]["project"].ToString()),
                            approved_by = (dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            branch = (dsList.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()),
                            notified_to = (dsList.Tables[0].Rows[0]["notified_to"].ToString()),
                        };
                    }

                    ViewBag.InspType = objGlobaldata.GetDropdownList("Inspection Type");

                    ViewBag.InspArea = objGlobaldata.GetDropdownList("Inspection Area");
                    ViewBag.Location = objGlobaldata.GetDivisionLocationList(objModel.branch);
                    ViewBag.Frequency = objGlobaldata.GetDropdownList("Inspection Frequency");
                    ViewBag.Section = objGlobaldata.FunGetDeptSectionList(objModel.dept);
                    ViewBag.checklist_ref = objGlobaldata.FunGetChecklistRefList(objModel.insp_type);
                    ViewBag.ApprovedBy = FunGetApproverMultiList(objModel.dept);
                   // ViewBag.pers_resp = FunGetPersRespMultiList(objModel.dept);
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    // inspection dates

                    InspctionQuestionList objInsp = new InspctionQuestionList();
                    objInsp.InspectionQstList = new List<InspctionQuestionModels>();

                    string sSqlstmt1 = "select id_inspection_date,id_inspection_plan,insp_date,pers_resp from t_inspection_plan_dates where id_inspection_plan = '" + id_inspection_plan + "'";
                    DataSet dsChklist = objGlobaldata.Getdetails(sSqlstmt1);
                    if (dsChklist.Tables.Count > 0 && dsChklist.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsChklist.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                InspctionQuestionModels objAuditModel = new InspctionQuestionModels
                                {
                                    id_inspection_date = dsChklist.Tables[0].Rows[i]["id_inspection_date"].ToString(),
                                    id_inspection_plan = dsChklist.Tables[0].Rows[i]["id_inspection_plan"].ToString(),
                                    pers_resp = dsChklist.Tables[0].Rows[i]["pers_resp"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsChklist.Tables[0].Rows[i]["insp_date"].ToString() != ""
                                 && DateTime.TryParse(dsChklist.Tables[0].Rows[i]["insp_date"].ToString(), out dtDocDate))
                                {
                                    objAuditModel.insp_date = dtDocDate;
                                }
                                objInsp.InspectionQstList.Add(objAuditModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in EditPlanInspection: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("PlanInspectionlist");
                            }
                        }
                        ViewBag.objList = objInsp;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EditPlanInspection: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditPlanInspection(InspctionQuestionModels objModel, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                objModel.insp_criteria = form["insp_criteria"];
                objModel.notified_to = form["notified_to"];
                objModel.Section = form["Section"];
                InspctionQuestionList objInsp = new InspctionQuestionList();
                objInsp.InspectionQstList = new List<InspctionQuestionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    InspctionQuestionModels obj = new InspctionQuestionModels();

                    if (form["insp_date" + i] != "" && form["insp_date" + i] != null)
                    {
                        if (DateTime.TryParse(form["insp_date" + i], out dateValue) == true)
                        {
                            obj.insp_date = dateValue;
                        }
                        obj.pers_resp = form["pers_resp" + i];

                        objInsp.InspectionQstList.Add(obj);
                    }
                }
                if (objModel.FunEditInspectionPlan(objModel, objInsp))
                {
                    TempData["Successdata"] = "Inspection plan edited successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EditPlanInspection: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("PlanInspectionlist");
        }

        [AllowAnonymous]
        public ActionResult PlanInspectionlist(string branch_name)
        {
            ViewBag.SubMenutype = "PlanInspectionlist";
            InspctionQuestionList objInsp = new InspctionQuestionList();
            objInsp.InspectionQstList = new List<InspctionQuestionModels>();

            InspectionCategoryModels objInspModel = new InspectionCategoryModels();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_inspection_date,T1.id_inspection_plan,insp_date,checklist_ref,RevNo,insp_type,dept,Section,insp_area,location,pers_resp,insp_status,approved_status,approved_by"
                + " from t_inspection_plan T1,t_inspection_plan_dates T2 where T1.id_inspection_plan = T2.id_inspection_plan and active = 1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                    ViewBag.Branch_name = sBranch_name;
                }

                sSqlstmt = sSqlstmt + " order by id_inspection_plan desc";

                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            InspctionQuestionModels objModel = new InspctionQuestionModels
                            {
                                id_inspection_date = dsList.Tables[0].Rows[i]["id_inspection_date"].ToString(),
                                id_inspection_plan = dsList.Tables[0].Rows[i]["id_inspection_plan"].ToString(),
                                checklist_ref = objGlobaldata.GetInspectionChecklistRefNamebyRevNo(dsList.Tables[0].Rows[i]["checklist_ref"].ToString(), dsList.Tables[0].Rows[i]["RevNo"].ToString()),
                                insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_type"].ToString()),
                                dept = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[i]["dept"].ToString()),
                                Section = objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[i]["Section"].ToString()),
                                insp_area = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_area"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[i]["location"].ToString()),
                                pers_resp = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[i]["pers_resp"].ToString()),
                                insp_status = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_status"].ToString()),
                                approved_status = dsList.Tables[0].Rows[i]["approved_status"].ToString(),
                                approved_by = dsList.Tables[0].Rows[i]["approved_by"].ToString(),
                            };
                            DateTime dtDocDate;
                            if (dsList.Tables[0].Rows[i]["insp_date"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["insp_date"].ToString(), out dtDocDate))
                            {
                                objModel.insp_date = dtDocDate;
                            }
                            objInsp.InspectionQstList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PlanInspectionlist: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PlanInspectionlist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objInsp.InspectionQstList.ToList());
        }

        [AllowAnonymous]
        public JsonResult InspPlanDelete(string id_inspection_plan)
        {
            try
            {
                if (id_inspection_plan != "")
                {
                    InspctionQuestionModels Doc = new InspctionQuestionModels();
                    string sid_inspection_plan = id_inspection_plan;
                    if (Doc.FunDeleteInspPlan(sid_inspection_plan))
                    {
                        TempData["Successdata"] = "Deleted successfully";
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspPlanDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult PlanInspectionApprovalDetails()
        {
            InspctionQuestionModels objModel = new InspctionQuestionModels();

            InspectionCategoryModels objInspModel = new InspectionCategoryModels();
            try
            {
                if (Request.QueryString["id_inspection_plan"] != null && Request.QueryString["id_inspection_plan"] != "")
                {
                    ViewBag.ApproveStatus = objGlobaldata.GetConstantValueKeyValuePair("InspPlanApprove");
                    string id_inspection_plan = Request.QueryString["id_inspection_plan"];
                    string sSqlstmt = "select id_inspection_plan,checklist_ref,RevNo,insp_type,insp_detail,dept,Section,insp_area,location,insp_freq,project"
                        + " from t_inspection_plan where id_inspection_plan = '" + id_inspection_plan + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new InspctionQuestionModels
                        {
                            id_inspection_plan = dsList.Tables[0].Rows[0]["id_inspection_plan"].ToString(),
                            checklist_ref = objGlobaldata.GetInspectionChecklistRefNamebyRevNo(dsList.Tables[0].Rows[0]["checklist_ref"].ToString(), dsList.Tables[0].Rows[0]["RevNo"].ToString()),
                            insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()),
                            insp_detail = dsList.Tables[0].Rows[0]["insp_detail"].ToString(),
                            dept = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()),
                            Section = objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[0]["Section"].ToString()),
                            insp_area = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_area"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[0]["location"].ToString()),
                            insp_freq = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_freq"].ToString()),
                            project = dsList.Tables[0].Rows[0]["project"].ToString(),
                        };
                    }

                    // inspection dates
                    string sSqlstmt1 = "select insp_date,pers_resp,insp_status from t_inspection_plan_dates where id_inspection_plan = '" + id_inspection_plan + "'";
                    DataSet dsChklist = objGlobaldata.Getdetails(sSqlstmt1);
                    ViewBag.objChklist = dsChklist;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PlanInspectionApprovalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //insp approve
        public ActionResult InspPlanApprove(InspctionQuestionModels objModel, FormCollection form)
        {
            try
            {
                if (objModel.FunInspPlanApprove(objModel))
                {
                    TempData["Successdata"] = "Updated Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspPlanApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult PlanInspectionDetails()
        {
            InspctionQuestionModels objModel = new InspctionQuestionModels();

            InspectionCategoryModels objInspModel = new InspectionCategoryModels();
            try
            {
                if (Request.QueryString["id_inspection_plan"] != null && Request.QueryString["id_inspection_plan"] != "")
                {
                    ViewBag.ApproveStatus = objGlobaldata.GetConstantValueKeyValuePair("InspPlanApprove");
                    string id_inspection_plan = Request.QueryString["id_inspection_plan"];
                    string sSqlstmt = "select id_inspection_plan,checklist_ref,RevNo,insp_type,insp_detail,dept,Section,insp_area,location,insp_freq,project,logged_by,logged_date,approved_by,approved_date,approver_comments,"
                          + "(CASE WHEN approved_status = '0' THEN 'Pending for Approval' WHEN approved_status = '1' THEN 'Rejected' WHEN approved_status = '2' THEN 'Approved' END) as approved_status,notified_to"
                        + " from t_inspection_plan where id_inspection_plan = '" + id_inspection_plan + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new InspctionQuestionModels
                        {
                            id_inspection_plan = dsList.Tables[0].Rows[0]["id_inspection_plan"].ToString(),
                            checklist_ref = objGlobaldata.GetInspectionChecklistRefNamebyRevNo(dsList.Tables[0].Rows[0]["checklist_ref"].ToString(), dsList.Tables[0].Rows[0]["RevNo"].ToString()),
                            insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()),
                            insp_detail = dsList.Tables[0].Rows[0]["insp_detail"].ToString(),
                            dept = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()),
                            Section = objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[0]["Section"].ToString()),
                            insp_area = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_area"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[0]["location"].ToString()),
                            insp_freq = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_freq"].ToString()),
                            project = dsList.Tables[0].Rows[0]["project"].ToString(),
                            logged_by = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["logged_by"].ToString()),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            approver_comments = dsList.Tables[0].Rows[0]["approver_comments"].ToString(),
                            approved_status = dsList.Tables[0].Rows[0]["approved_status"].ToString(),
                            notified_to = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["notified_to"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["logged_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["approved_date"].ToString() != ""
                        && DateTime.TryParse(dsList.Tables[0].Rows[0]["approved_date"].ToString(), out dtDocDate))
                        {
                            objModel.approved_date = dtDocDate;
                        }
                    }

                    // inspection dates
                    string sSqlstmt1 = "select insp_date,pers_resp,insp_status from t_inspection_plan_dates where id_inspection_plan = '" + id_inspection_plan + "'";
                    DataSet dsChklist = objGlobaldata.Getdetails(sSqlstmt1);
                    ViewBag.objChklist = dsChklist;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PlanInspectionDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //insp perform

        [AllowAnonymous]
        public ActionResult PerformInspectionlist(string branch_name)
        {
            ViewBag.SubMenutype = "PerformInspectionlist";
            InspctionQuestionList objInsp = new InspctionQuestionList();
            objInsp.InspectionQstList = new List<InspctionQuestionModels>();

            InspectionCategoryModels objInspModel = new InspectionCategoryModels();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_inspection_date,T1.id_inspection_plan,insp_date,checklist_ref,RevNo,insp_type,dept,Section,insp_area,location,pers_resp,insp_status,approved_status"
                + " from t_inspection_plan T1,t_inspection_plan_dates T2 where T1.id_inspection_plan = T2.id_inspection_plan and active = 1 and approved_status=2 ";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                    ViewBag.Branch_name = sBranch_name;
                }

                sSqlstmt = sSqlstmt + " order by id_inspection_plan desc";

                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            InspctionQuestionModels objModel = new InspctionQuestionModels
                            {
                                id_inspection_date = dsList.Tables[0].Rows[i]["id_inspection_date"].ToString(),
                                id_inspection_plan = dsList.Tables[0].Rows[i]["id_inspection_plan"].ToString(),
                                checklist_ref = objGlobaldata.GetInspectionChecklistRefNamebyRevNo(dsList.Tables[0].Rows[i]["checklist_ref"].ToString(), dsList.Tables[0].Rows[i]["RevNo"].ToString()),
                                insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_type"].ToString()),
                                dept = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[i]["dept"].ToString()),
                                Section = objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[i]["Section"].ToString()),
                                insp_area = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_area"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[i]["location"].ToString()),
                                pers_resp = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[i]["pers_resp"].ToString()),
                                insp_status = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["insp_status"].ToString()),
                                approved_status = dsList.Tables[0].Rows[i]["approved_status"].ToString(),
                            };
                            DateTime dtDocDate;
                            if (dsList.Tables[0].Rows[i]["insp_date"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["insp_date"].ToString(), out dtDocDate))
                            {
                                objModel.insp_date = dtDocDate;
                            }
                            objInsp.InspectionQstList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PerformInspectionlist: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformInspectionlist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objInsp.InspectionQstList.ToList());
        }

        [AllowAnonymous]
        public ActionResult PerformInspection()
        {
            InspctionQuestionModels objModel = new InspctionQuestionModels();

            InspectionCategoryModels objInspModel = new InspectionCategoryModels();
            try
            {
                if (Request.QueryString["id_inspection_plan"] != null && Request.QueryString["id_inspection_plan"] != "" && Request.QueryString["id_inspection_date"] != null && Request.QueryString["id_inspection_date"] != "")
                {
                    string id_inspection_plan = Request.QueryString["id_inspection_plan"];
                    string id_inspection_date = Request.QueryString["id_inspection_date"];

                    string sSqlstmt = "select id_inspection_date,T1.id_inspection_plan,RevNo,branch,checklist_ref,insp_type,insp_detail,dept,Section,insp_area,location,insp_freq,project,approved_by,insp_status,insp_perf_date,notification_to"
                    + " from t_inspection_plan T1,t_inspection_plan_dates T2 where T1.id_inspection_plan = T2.id_inspection_plan and id_inspection_date = '" + id_inspection_date + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new InspctionQuestionModels
                        {
                            id_inspection_date = dsList.Tables[0].Rows[0]["id_inspection_date"].ToString(),
                            id_inspection_plan = dsList.Tables[0].Rows[0]["id_inspection_plan"].ToString(),

                            checklist_ref = objGlobaldata.GetInspectionChecklistRefNamebyRevNo(dsList.Tables[0].Rows[0]["checklist_ref"].ToString(), dsList.Tables[0].Rows[0]["RevNo"].ToString()),
                            insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()),
                            insp_detail = dsList.Tables[0].Rows[0]["insp_detail"].ToString(),
                            dept = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()),
                            Section = objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[0]["Section"].ToString()),
                            insp_area = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_area"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[0]["location"].ToString()),
                            insp_freq = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_freq"].ToString()),
                            project = dsList.Tables[0].Rows[0]["project"].ToString(),
                            insp_status = dsList.Tables[0].Rows[0]["insp_status"].ToString(),
                            notification_to = dsList.Tables[0].Rows[0]["notification_to"].ToString(),
                        };

                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["insp_perf_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["insp_perf_date"].ToString(), out dtDocDate))
                        {
                            objModel.insp_perf_date = dtDocDate;
                        }
                    }

                    // inspection checklist

                    InspctionQuestionList objInsp = new InspctionQuestionList();
                    objInsp.InspectionQstList = new List<InspctionQuestionModels>();

                    if (Convert.ToString(objModel.insp_perf_date) == "01/01/0001 00:00:00")
                    {
                        string sql = "select id_question_master from t_inspection_question_master where id_question_master='" + dsList.Tables[0].Rows[0]["checklist_ref"].ToString() + "' and  RevNo = '" + dsList.Tables[0].Rows[0]["RevNo"].ToString() + "'";
                        DataSet dslist = objGlobaldata.Getdetails(sql);
                        if (dslist.Tables.Count > 0 && dslist.Tables[0].Rows.Count > 0)
                        {
                            string sSqlstmt1 = "select InspectionQuestions,criticality,criteria from t_inspection_questions where id_question_master = '" + dsList.Tables[0].Rows[0]["checklist_ref"].ToString() + "'";
                            DataSet dsChklist = objGlobaldata.Getdetails(sSqlstmt1);
                            if (dsChklist.Tables.Count > 0 && dsChklist.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsChklist.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        InspctionQuestionModels objAuditModel = new InspctionQuestionModels
                                        {
                                            InspectionQuestions = dsChklist.Tables[0].Rows[i]["InspectionQuestions"].ToString(),
                                            criticality = dsChklist.Tables[0].Rows[i]["criticality"].ToString(),
                                            criteria = dsChklist.Tables[0].Rows[i]["criteria"].ToString(),
                                        };
                                        objInsp.InspectionQstList.Add(objAuditModel);
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in PerformInspection: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                        return RedirectToAction("PerformInspectionlist");
                                    }
                                }
                                ViewBag.objList = objInsp;
                            }
                        }
                        else
                        {
                            string sql1 = "select id_question_master_history from t_inspection_question_master_history where id_question_master='" + dsList.Tables[0].Rows[0]["checklist_ref"].ToString() + "' and  RevNo = '" + dsList.Tables[0].Rows[0]["RevNo"].ToString() + "'";
                            DataSet dslist1 = objGlobaldata.Getdetails(sql1);
                            if (dslist1.Tables.Count > 0 && dslist1.Tables[0].Rows.Count > 0)
                            {
                                string sSqlstmt1 = "select InspectionQuestions,criticality,criteria from t_inspection_questions_history where id_question_master_history = '" + dslist1.Tables[0].Rows[0]["id_question_master_history"].ToString() + "'";
                                DataSet dsChklist = objGlobaldata.Getdetails(sSqlstmt1);
                                if (dsChklist.Tables.Count > 0 && dsChklist.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < dsChklist.Tables[0].Rows.Count; i++)
                                    {
                                        try
                                        {
                                            InspctionQuestionModels objAuditModel = new InspctionQuestionModels
                                            {
                                                InspectionQuestions = dsChklist.Tables[0].Rows[i]["InspectionQuestions"].ToString(),
                                                criticality = dsChklist.Tables[0].Rows[i]["criticality"].ToString(),
                                                criteria = dsChklist.Tables[0].Rows[i]["criteria"].ToString(),
                                            };
                                            objInsp.InspectionQstList.Add(objAuditModel);
                                        }
                                        catch (Exception ex)
                                        {
                                            objGlobaldata.AddFunctionalLog("Exception in PerformInspection: " + ex.ToString());
                                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                            return RedirectToAction("PerformInspectionlist");
                                        }
                                    }
                                    ViewBag.objList = objInsp;
                                }
                            }
                        }
                    }
                    else
                    {
                        string sSqlstmt1 = "select id_inspection_perform_checklist,id_inspection_date,id_inspection_plan,InspectionQuestions,criticality,criteria,insp_result,details,upload,action_required,suggestion from t_inspection_perform_checklist where id_inspection_date = '" + id_inspection_date + "'";
                        DataSet dsChklist = objGlobaldata.Getdetails(sSqlstmt1);
                        if (dsChklist.Tables.Count > 0 && dsChklist.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsChklist.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    InspctionQuestionModels objAuditModel = new InspctionQuestionModels
                                    {
                                        id_inspection_perform_checklist = dsChklist.Tables[0].Rows[i]["id_inspection_perform_checklist"].ToString(),
                                        id_inspection_date = dsChklist.Tables[0].Rows[i]["id_inspection_date"].ToString(),
                                        id_inspection_plan = dsChklist.Tables[0].Rows[i]["id_inspection_plan"].ToString(),
                                        InspectionQuestions = dsChklist.Tables[0].Rows[i]["InspectionQuestions"].ToString(),
                                        criticality = dsChklist.Tables[0].Rows[i]["criticality"].ToString(),
                                        criteria = dsChklist.Tables[0].Rows[i]["criteria"].ToString(),

                                        insp_result = dsChklist.Tables[0].Rows[i]["insp_result"].ToString(),
                                        details = dsChklist.Tables[0].Rows[i]["details"].ToString(),
                                        upload = dsChklist.Tables[0].Rows[i]["upload"].ToString(),
                                        action_required = dsChklist.Tables[0].Rows[i]["action_required"].ToString(),
                                        suggestion = dsChklist.Tables[0].Rows[i]["suggestion"].ToString(),
                                    };
                                    objInsp.InspectionQstList.Add(objAuditModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in PerformInspection: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("PerformInspectionlist");
                                }
                            }
                            ViewBag.objList = objInsp;
                        }
                    }

                    ViewBag.insp_result = objGlobaldata.GetDropdownList("Inspection Result");
                    ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                    ViewBag.Status = objGlobaldata.GetDropdownList("Inspection Status");
                    ViewBag.Notification = FunGetApproverMultiList(dsList.Tables[0].Rows[0]["dept"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformInspection: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult PerformInspection(InspctionQuestionModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> supload0, IEnumerable<HttpPostedFileBase> supload1, IEnumerable<HttpPostedFileBase> supload2, IEnumerable<HttpPostedFileBase> supload3, IEnumerable<HttpPostedFileBase> supload4, IEnumerable<HttpPostedFileBase> supload5, IEnumerable<HttpPostedFileBase> supload6, IEnumerable<HttpPostedFileBase> supload7, IEnumerable<HttpPostedFileBase> supload8, IEnumerable<HttpPostedFileBase> supload9, IEnumerable<HttpPostedFileBase> supload10, IEnumerable<HttpPostedFileBase> supload_test)
        {
            try
            {
                string QCDeletes = "";
                DateTime dateValue;
                objModel.notification_to = form["notification_to"];

                if (DateTime.TryParse(form["insp_perf_date"], out dateValue) == true)
                {
                    objModel.insp_perf_date = dateValue;
                }

                InspctionQuestionList objInsp = new InspctionQuestionList();
                objInsp.InspectionQstList = new List<InspctionQuestionModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    InspctionQuestionModels obj = new InspctionQuestionModels();

                    obj.id_inspection_perform_checklist = form["id_inspection_perform_checklist" + i];

                    obj.InspectionQuestions = form["InspectionQuestions" + i];
                    obj.criticality = form["criticality" + i];
                    obj.criteria = form["criteria" + i];
                    obj.insp_result = form["insp_result" + i];
                    obj.details = form["details" + i];

                    obj.action_required = form["action_required" + i];
                    obj.suggestion = form["suggestion" + i];

                    //certificate upload
                    IList<HttpPostedFileBase> upload = (IList<HttpPostedFileBase>)supload_test;
                    if (i == 0)
                    {
                        upload = (IList<HttpPostedFileBase>)supload0;
                        QCDeletes = Request.Form["QCDocsValselectall" + i];
                    }
                    if (i == 1)
                    {
                        upload = (IList<HttpPostedFileBase>)supload1;
                        QCDeletes = Request.Form["QCDocsValselectall" + i];
                    }
                    if (i == 2)
                    {
                        upload = (IList<HttpPostedFileBase>)supload2;
                        QCDeletes = Request.Form["QCDocsValselectall" + i];
                    }
                    if (i == 3)
                    {
                        upload = (IList<HttpPostedFileBase>)supload3;
                        QCDeletes = Request.Form["QCDocsValselectall" + i];
                    }
                    if (i == 4)
                    {
                        upload = (IList<HttpPostedFileBase>)supload4;
                        QCDeletes = Request.Form["QCDocsValselectall" + i];
                    }
                    if (i == 5)
                    {
                        upload = (IList<HttpPostedFileBase>)supload5;
                        QCDeletes = Request.Form["QCDocsValselectall" + i];
                    }
                    if (i == 6)
                    {
                        upload = (IList<HttpPostedFileBase>)supload6;
                        QCDeletes = Request.Form["QCDocsValselectall" + i];
                    }
                    if (i == 7)
                    {
                        upload = (IList<HttpPostedFileBase>)supload7;
                        QCDeletes = Request.Form["QCDocsValselectall" + i];
                    }
                    if (i == 8)
                    {
                        upload = (IList<HttpPostedFileBase>)supload8;
                        QCDeletes = Request.Form["QCDocsValselectall" + i];
                    }
                    if (i == 9)
                    {
                        upload = (IList<HttpPostedFileBase>)supload9;
                        QCDeletes = Request.Form["QCDocsValselectall" + i];
                    }
                    if (i == 10)
                    {
                        upload = (IList<HttpPostedFileBase>)supload10;
                        QCDeletes = Request.Form["QCDocsValselectall" + i];
                    }
                    if (upload[0] != null)
                    {
                        obj.upload = "";
                        foreach (var file in upload)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE_INSP"), Path.GetFileName(file.FileName));
                                string sFilename = "HSE_INSP" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                obj.upload = obj.upload + "," + "~/DataUpload/MgmtDocs/HSE_INSP/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in PerformInspection: " + ex.ToString());
                            }
                        }
                        obj.upload = obj.upload.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                    if (form["QCDocsVal" + i] != null && form["QCDocsVal" + i] != "")
                    {
                        obj.upload = obj.upload + "," + form["QCDocsVal" + i];
                        obj.upload = obj.upload.Trim(',');
                    }
                    else if (form["QCDocsVal" + i] == null && QCDeletes != null && upload[0] == null)
                    {
                        obj.upload = null;
                    }
                    else if (form["QCDocsVal" + i] == null && upload[0] == null)
                    {
                        obj.upload = null;
                    }

                    objInsp.InspectionQstList.Add(obj);
                }
                if (objModel.FunPerformInspection(objModel, objInsp))
                {
                    TempData["Successdata"] = "Inspection performed successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformInspection: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("PerformInspectionlist");
        }

        [AllowAnonymous]
        public ActionResult PerformInspectionDetail()
        {
            InspctionQuestionModels objModel = new InspctionQuestionModels();

            InspectionCategoryModels objInspModel = new InspectionCategoryModels();
            try
            {
                if (Request.QueryString["id_inspection_plan"] != null && Request.QueryString["id_inspection_plan"] != "" && Request.QueryString["id_inspection_date"] != null && Request.QueryString["id_inspection_date"] != "")
                {
                    string id_inspection_plan = Request.QueryString["id_inspection_plan"];
                    string id_inspection_date = Request.QueryString["id_inspection_date"];

                    string sSqlstmt = "select id_inspection_date,T1.id_inspection_plan,branch,checklist_ref,insp_type,insp_detail,dept,Section,insp_area,location,insp_freq,project,approved_by,insp_status,insp_perf_date,notification_to,T2.logged_by,T2.logged_date,pers_resp"
                    + " from t_inspection_plan T1,t_inspection_plan_dates T2 where T1.id_inspection_plan = T2.id_inspection_plan and id_inspection_date = '" + id_inspection_date + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new InspctionQuestionModels
                        {
                            id_inspection_date = dsList.Tables[0].Rows[0]["id_inspection_date"].ToString(),
                            id_inspection_plan = dsList.Tables[0].Rows[0]["id_inspection_plan"].ToString(),
                            checklist_ref = objGlobaldata.GetInspectionChecklistRefNamebyId(dsList.Tables[0].Rows[0]["checklist_ref"].ToString()),
                            insp_type = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_type"].ToString()),
                            insp_detail = dsList.Tables[0].Rows[0]["insp_detail"].ToString(),
                            dept = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["dept"].ToString()),
                            Section = objInspModel.GetSectionNamebyId(dsList.Tables[0].Rows[0]["Section"].ToString()),
                            insp_area = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_area"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[0]["location"].ToString()),
                            insp_freq = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_freq"].ToString()),
                            project = dsList.Tables[0].Rows[0]["project"].ToString(),
                            insp_status = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["insp_status"].ToString()),
                            notification_to = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["notification_to"].ToString()),
                            logged_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["logged_by"].ToString()),
                            pers_resp = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["pers_resp"].ToString()),
                        };

                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["insp_perf_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["insp_perf_date"].ToString(), out dtDocDate))
                        {
                            objModel.insp_perf_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["logged_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                    }

                    // inspection checklist

                    string sSqlstmt1 = "select id_inspection_perform_checklist,id_inspection_date,id_inspection_plan,InspectionQuestions,criticality,criteria,insp_result,details,upload,action_required,suggestion from t_inspection_perform_checklist where id_inspection_date = '" + id_inspection_date + "'";
                    ViewBag.objList = objGlobaldata.Getdetails(sSqlstmt1);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformInspectionDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //--------------------------------------

        public JsonResult AddInspectionQuestionsSearch(string sCategory, string sSection, string branch_name)
        {
            try
            {
                InspectionCategoryModels obj = new InspectionCategoryModels();
                InspctionQuestionModels objQst = new InspctionQuestionModels();

                ViewBag.Category = obj.GetInspectionCategoryList();
                //ViewBag.Section = obj.GetInspectionSectionListbox(sCategory);
                //ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name != "" && sCategory != null && sCategory != "" && sSection != null && sSection != "")
                {
                    ViewBag.Branch_name = branch_name;
                    ViewBag.Category_Id = sCategory;
                    ViewBag.Section_Id = sSection;
                    ViewBag.CategoryQuestions = objQst.GetInspectionQuestList(sCategory, sSection, branch_name);
                }
                else if (branch_name != null && branch_name != "")
                {
                    ViewBag.Branch_name = branch_name;
                    ViewBag.CategoryQuestions = objQst.GetInspectionQuestionsListboxWithBranch(branch_name);
                }
                else if (sCategory != null && sCategory != "" && sSection != null && sSection != "")
                {
                    ViewBag.Category_Id = sCategory;
                    ViewBag.Section_Id = sSection;
                    ViewBag.CategoryQuestions = objQst.GetInspectionQuestList(sCategory, sSection, sBranch_name);
                }
                else
                {
                    ViewBag.CategoryQuestions = objQst.GetInspectionQuestionsListboxWithBranch(sBranch_name);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInspectionQuestionsSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddInspectionQuestions(InspctionQuestionModels objInspModels, FormCollection form)
        //{
        //    InspectionCategoryModels obj = new InspectionCategoryModels();
        //    InspctionQuestionModels objQst = new InspctionQuestionModels();
        //    try
        //    {
        //        ViewBag.Category_Id = objInspModels.Category;
        //        ViewBag.Section_Id = objInspModels.Section;
        //        // ViewBag.Emp_Id = objInspModels.resp_person;
        //        string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
        //        //ViewBag.Category = obj.GetInspectionCategoryListbox();
        //        ViewBag.Category = obj.GetInspectionCategoryList();
        //        ViewBag.Section = obj.GetInspectionSectionListbox(objInspModels.Category, sBranch_name);

        //        if (objQst.FunAddInspectionQuestions(objInspModels))
        //        {
        //            TempData["Successdata"] = "Added Inspection Questions successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddInspectionQuestions: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    ViewBag.CategoryQuestions = objQst.GetInspectionQuestionsListbox(objInspModels.Category);

        //    return View(objInspModels);
        //}

        public ActionResult InspectionQuestionDelete(string id_inspection_question, string Category)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();
            try
            {
                if (obj.FunDeleteQuestions(id_inspection_question))
                {
                    TempData["Successdata"] = "Question deleted successfully";
                    return RedirectToAction("AddInspectionQuestions", new { sCategory = Category });
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionQuestionDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddInspectionQuestions");
        }

        public JsonResult InspectionQuestionDelete1(string id_inspection_question, string Category)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();

            try
            {
                if (obj.FunDeleteQuestions(id_inspection_question))
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
                objGlobaldata.AddFunctionalLog("Exception in InspectionQuestionDelete1: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult InspectionQuestionsUpdate(int id_inspection_question, string InspectionQuestions)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();
            try
            {
                if (obj.FunUpdateInspectionQuestions(id_inspection_question, InspectionQuestions))
                {
                    TempData["Successdata"] = "Inspection Questions updated successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionQuestionsUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }

        [HttpPost]
        public JsonResult GetQuestions(string Category, string Section, string branch_name)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();
            var data = new object();
            string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
            if (branch_name != "" && branch_name != null)
            {
                data = obj.GetInspectionQuestList(Category, Section, branch_name);
                //data = obj.GetInspectionQuestionsListbox(Category,branch_name);
            }
            else
            {
                data = obj.GetInspectionQuestList(Category, Section, sBranch_name);
            }

            return Json(data);
        }

        [HttpPost]
        public JsonResult GetQuestions1(string Category, string branch_name)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();
            var data = new object();
            string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
            if (branch_name != "" && branch_name != null)
            {
                data = obj.GetInspectionQuestionsListbox(Category, branch_name);
            }
            else
            {
                data = obj.GetInspectionQuestionsListbox(Category, sBranch_name);
            }

            return Json(data);
        }

        [HttpPost]
        public JsonResult FunGetSection(string Category, string branch_name)
        {
            InspectionCategoryModels obj = new InspectionCategoryModels();
            var data = new object();
            string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
            if (branch_name != "" && branch_name != null)
            {
                data = obj.GetInspectionSectionListbox(Category, branch_name);
                //data = obj.GetInspectionQuestionsListbox(Category);
            }
            else
            {
                data = obj.GetInspectionSectionListbox(Category, sBranch_name);
            }
            return Json(data);
        }

        //------------------------------------------------------------

        public ActionResult AddInspectionRatings(string sCategory)
        {
            try
            {
                InspectionCategoryModels obj = new InspectionCategoryModels();
                InspctionQuestionModels objQst = new InspctionQuestionModels();
                ViewBag.Category = obj.GetInspectionCategoryListbox();
                if (sCategory != null && sCategory != "")
                {
                    ViewBag.Category_Id = sCategory;
                    ViewBag.CategoryRatings = objQst.GetInspectionRatingListbox(sCategory);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInspectionRatings: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInspectionRatings(InspctionQuestionModels objInspModels, FormCollection form)
        {
            InspectionCategoryModels obj = new InspectionCategoryModels();
            InspctionQuestionModels objQst = new InspctionQuestionModels();
            try
            {
                ViewBag.Category_Id = objInspModels.Category;
                ViewBag.Category = obj.GetInspectionCategoryListbox();

                if (objQst.FunAddInspectionRatings(objInspModels))
                {
                    TempData["Successdata"] = "Added Ratings successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInspectionRatings: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            ViewBag.CategoryRatings = objQst.GetInspectionRatingListbox(objInspModels.Category);

            return View(objInspModels);
        }

        public ActionResult InspectionRatingDelete(string id_inspection_rating, string Category)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();
            try
            {
                if (obj.FunDeleteRatings(id_inspection_rating))
                {
                    TempData["Successdata"] = "Rating deleted successfully";
                    return RedirectToAction("AddInspectionRatings", new { sCategory = Category });
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionRatingDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddInspectionRatings");
        }

        public JsonResult InspectionRatingDelete1(string id_inspection_rating, string Category)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();

            try
            {
                if (obj.FunDeleteRatings(id_inspection_rating))
                {
                    TempData["Successdata"] = "Rating deleted successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionRatingDelete1: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult InspectionRatingUpdate(int id_inspection_rating, string inspection_rating)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();
            try
            {
                if (obj.FunUpdateInspectionRatings(id_inspection_rating, inspection_rating))
                {
                    TempData["Successdata"] = "Inspection Ratings updated successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionRatingUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }

        [HttpPost]
        public JsonResult GetRatings(string Category)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();

            var data = obj.GetInspectionRatingListbox(Category);

            return Json(data);
        }

        //---------------------Section-------------------------

        public ActionResult InspectionSectionDelete(string id_inspection, string Category)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();
            try
            {
                if (obj.FunDeleteSection(id_inspection))
                {
                    TempData["Successdata"] = "Section deleted successfully";
                    return RedirectToAction("AddInspectionSection", new { sCategory = Category });
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionSectionDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddInspectionRatings");
        }

        //------------------------------------------------------------

        [AllowAnonymous]
        public ActionResult InspectionList(string branch_name)
        {
            InspectionCategoryList objInsplist = new InspectionCategoryList();
            objInsplist.InspectionLst = new List<InspectionCategoryModels>();

            InspectionCategoryModels obj = new InspectionCategoryModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                // string sSqlstmt = "select  DISTINCT Category  from t_inspection_questions where Active=1";
                string sSqlstmt = "SELECT DISTINCT Category FROM t_inspection_questions WHERE Category IN" +
                    "(SELECT DISTINCT Category FROM t_inspection_rating where Active = 1) and Active = 1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set(branch,'" + branch_name + "')";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set(branch,'" + sBranch_name + "')";
                    ViewBag.Branch_name = sBranch_name;
                }
                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsInspectionModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsInspectionModelsList.Tables.Count > 0 && dsInspectionModelsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsInspectionModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            InspectionCategoryModels objInspclist = new InspectionCategoryModels
                            {
                                Category = obj.GetInspectionCategoryByID(dsInspectionModelsList.Tables[0].Rows[i]["Category"].ToString()),
                            };
                            objInsplist.InspectionLst.Add(objInspclist);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in InspectionList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objInsplist.InspectionLst.ToList());
        }

        [AllowAnonymous]
        public JsonResult InspectionListSearch(string branch_name)
        {
            InspectionCategoryList objInsplist = new InspectionCategoryList();
            objInsplist.InspectionLst = new List<InspectionCategoryModels>();

            InspectionCategoryModels obj = new InspectionCategoryModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                // string sSqlstmt = "select  DISTINCT Category  from t_inspection_questions where Active=1";
                string sSqlstmt = "SELECT DISTINCT Category FROM t_inspection_questions WHERE Category IN" +
                    "(SELECT DISTINCT Category FROM t_inspection_rating where Active = 1) and Active = 1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set(branch,'" + branch_name + "')";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set(branch,'" + sBranch_name + "')";
                }
                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsInspectionModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsInspectionModelsList.Tables.Count > 0 && dsInspectionModelsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsInspectionModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            InspectionCategoryModels objInspclist = new InspectionCategoryModels
                            {
                                Category = obj.GetInspectionCategoryByID(dsInspectionModelsList.Tables[0].Rows[i]["Category"].ToString()),
                            };
                            objInsplist.InspectionLst.Add(objInspclist);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in InspectionListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        public ActionResult GenerateInspectionChecklist()
        {
            GenerateInspectionChecklistModels objInspModels = new GenerateInspectionChecklistModels();
            ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
            InspctionQuestionModels obj = new InspctionQuestionModels();
            InspectionCategoryModels objCat = new InspectionCategoryModels();

            try
            {
                if (Request.QueryString["branch"] != "" && Request.QueryString["Category"] != "")
                {
                    string Category = Request.QueryString["Category"];
                    string sbranch = Request.QueryString["branch"];

                    ViewBag.InspectionQuestions = obj.GetInspectionQuestions(objCat.getInspectionCategoryIDByName(Category), sbranch);
                    ViewBag.SectionQuestions = obj.GetSectionQuestions(objCat.getInspectionCategoryIDByName(Category), sbranch);
                    ViewBag.InspectionRating = obj.GetInspectionRating(objCat.getInspectionCategoryIDByName(Category));
                    objInspModels.Category = Category;
                    objInspModels.branch = sbranch;
                    ViewBag.sBranch = objGlobaldata.GetMultiCompanyBranchNameById(sbranch);

                    //objInspModels.branch = objGlobaldata.GetCurrentUserSession().division;
                    objInspModels.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                    objInspModels.Location = objGlobaldata.GetCurrentUserSession().Work_Location;
                    //  objInspModels.team = objGlobaldata.GetCurrentUserSession().team;
                    // ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                    ViewBag.Department = objGlobaldata.GetDepartmentListbox(sbranch);
                    ViewBag.Location = objGlobaldata.GetDivisionLocationList(sbranch);
                    //  ViewBag.Team = objGlobaldata.GetMultiTeambyMultiGroup(objInspModels.Department);
                }
                else
                {
                    TempData["alertdata"] = "Category cannot be null";
                    return RedirectToAction("AuditChecklistList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateInspectionChecklist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objInspModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateInspectionChecklist(GenerateInspectionChecklistModels objInspChecklist, FormCollection form)
        {
            try
            {
                InspectionCategoryModels obj = new InspectionCategoryModels();
                objInspChecklist.Category = obj.getInspectionCategoryIDByName(form["Category"]);
                objInspChecklist.Inspection_by = form["Inspection_by"];
                objInspChecklist.Location = form["Location"];
                objInspChecklist.Project = form["Project"];
                objInspChecklist.TagNo = form["TagNo"];
                objInspChecklist.Comment = form["Comment"];
                objInspChecklist.branch = form["branchs"];
                objInspChecklist.Department = form["Department"];
                //  objInspChecklist.team = form["team"];

                DateTime dateValue;
                if (DateTime.TryParse(form["Inspection_date"], out dateValue) == true)
                {
                    objInspChecklist.Inspection_date = dateValue;
                }

                InspctionQuestionModels objInsChk = new InspctionQuestionModels();

                InspectionChecklistList objInsp = new InspectionChecklistList();
                objInsp.lst = new List<InspectionChecklistModels>();

                MultiSelectList InspectionQuestions = objInsChk.GetInspectionQuestions(objInspChecklist.Category, objInspChecklist.branch);

                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in InspectionQuestions)
                {
                    if (i <= Convert.ToInt16(form["itmctn"]))
                    {
                        InspectionChecklistModels objElements = new InspectionChecklistModels();
                        objElements.id_inspection_question = form["InspectionQuestions " + item.Value];
                        objElements.id_inspection_rating = form["id_inspection_rating " + item.Value];
                        objElements.Action = form["Action " + i];
                        objElements.ActionBy = form["ActionBy " + i];
                        objElements.Upload = form["Upload" + i];
                        objInsp.lst.Add(objElements);
                    }
                    i++;
                }

                if (objInspChecklist.FunAddInspectionChecklist(objInspChecklist, objInsp))
                {
                    TempData["Successdata"] = "Inspection performed Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateInspectionChecklist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("InspectionList", new { branch_name = objInspChecklist.branch });
        }

        [HttpPost]
        public ActionResult GenerateInspectionChecklistReport(FormCollection form)
        {
            string sCategory = form["Category"];
            string sbranch = form["branchs"];
            try
            {
                if (sCategory != "" && sbranch != "")
                {
                    GenerateInspectionChecklistModels objInspModels = new GenerateInspectionChecklistModels();

                    InspctionQuestionModels obj = new InspctionQuestionModels();
                    InspectionCategoryModels objCat = new InspectionCategoryModels();

                    ViewBag.InspectionQuestions = obj.GetInspectionQuestions(objCat.getInspectionCategoryIDByName(sCategory), sbranch);
                    ViewBag.SectionQuestions = obj.GetSectionQuestions(objCat.getInspectionCategoryIDByName(sCategory), sbranch);
                    ViewBag.InspectionRating = obj.GetInspectionRating(objCat.getInspectionCategoryIDByName(sCategory));
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("GenerateInspectionChecklist");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateInspectionChecklistReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("GenerateInspectionChecklistReportToPDF")
            {
                //FileName = "GenerateInspectionChecklistReport.pdf",
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

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "InspectionChecklist" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/HSE/" + "InspectionChecklist" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
            }

            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult InspectionChecklistDetails()
        {
            try
            {
                if (Request.QueryString["id_inspection_checklist"] != null && Request.QueryString["id_inspection_checklist"] != "")
                {
                    InspectionCategoryModels obj = new InspectionCategoryModels();
                    InspctionQuestionModels objRating = new InspctionQuestionModels();

                    string sid_inspection_checklist = Request.QueryString["id_inspection_checklist"];
                    string sSqlstmt = "select id_inspection_checklist,Category,Inspection_date,Inspection_by,Location,"
                        + "Project,TagNo,Comment,branch,Department,team from t_inspection_checklist where id_inspection_checklist='" + sid_inspection_checklist + "' and Active=1";

                    DataSet dsInspect = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsInspect.Tables.Count > 0 && dsInspect.Tables[0].Rows.Count > 0)
                    {
                        GenerateInspectionChecklistModels objInsp = new GenerateInspectionChecklistModels
                        {
                            id_inspection_checklist = dsInspect.Tables[0].Rows[0]["id_inspection_checklist"].ToString(),
                            cat_id = (dsInspect.Tables[0].Rows[0]["Category"].ToString()),
                            Category = obj.GetInspectionCategoryByID(dsInspect.Tables[0].Rows[0]["Category"].ToString()),
                            Inspection_by = objGlobaldata.GetEmpHrNameById(dsInspect.Tables[0].Rows[0]["Inspection_by"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsInspect.Tables[0].Rows[0]["Location"].ToString()),
                            Project = dsInspect.Tables[0].Rows[0]["Project"].ToString(),
                            TagNo = dsInspect.Tables[0].Rows[0]["TagNo"].ToString(),
                            Comment = dsInspect.Tables[0].Rows[0]["Comment"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsInspect.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsInspect.Tables[0].Rows[0]["Department"].ToString()),
                            //  team = objGlobaldata.GetTeamNameByID(dsInspect.Tables[0].Rows[0]["team"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsInspect.Tables[0].Rows[0]["Inspection_date"].ToString(), out dtValue))
                        {
                            objInsp.Inspection_date = dtValue;
                        }

                        sSqlstmt = "SELECT id_inspection_checklist_trans,id_inspection_checklist,id_inspection_question,"
                            + "id_inspection_rating,Action,ActionBy,Upload from t_inspection_checklist_trans where id_inspection_checklist='" + sid_inspection_checklist + "'";

                        DataSet dsInspElement = objGlobaldata.Getdetails(sSqlstmt);

                        InspectionChecklistList objInspList = new InspectionChecklistList();
                        objInspList.lst = new List<InspectionChecklistModels>();

                        InspectionChecklistModels objIns = new InspectionChecklistModels();

                        for (int i = 0; dsInspElement.Tables.Count > 0 && i < dsInspElement.Tables[0].Rows.Count; i++)
                        {
                            InspectionChecklistModels objElements = new InspectionChecklistModels
                            {
                                id_ques = dsInspElement.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                                id_inspection_question = objIns.GetInspectionQuestionNameById(dsInspElement.Tables[0].Rows[i]["id_inspection_question"].ToString()),
                                id_inspection_rating = objIns.GetInspectionRatingNameById(dsInspElement.Tables[0].Rows[i]["id_inspection_rating"].ToString()),
                                Action = dsInspElement.Tables[0].Rows[i]["Action"].ToString(),
                                ActionBy = objGlobaldata.GetEmpHrNameById(dsInspElement.Tables[0].Rows[i]["ActionBy"].ToString()),
                                Upload = dsInspElement.Tables[0].Rows[i]["Upload"].ToString(),
                            };
                            objInspList.lst.Add(objElements);
                        }

                        ViewBag.InspectionElement = objInspList;
                        ViewBag.SectionQuestions = objRating.GetSectionQuestions(objInsp.cat_id, objInsp.branch);
                        ViewBag.InspectionRating = objRating.GetInspectionRating();
                        return View(objInsp);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("InspectionList");
                    }
                }
                else
                {
                    TempData["alertdata"] = " Id cannot be null";
                    return RedirectToAction("InspectionList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionChecklistDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("InspectionList");
        }

        [AllowAnonymous]
        public ActionResult InspectionChecklistInfo(int id)
        {
            try
            {
                InspectionCategoryModels obj = new InspectionCategoryModels();
                InspctionQuestionModels objRating = new InspctionQuestionModels();
                string sSqlstmt = "select id_inspection_checklist,Category,Inspection_date,Inspection_by,Location,"
                    + "Project,TagNo,Comment,branch,Department,team from t_inspection_checklist where id_inspection_checklist='" + id + "' and Active=1";

                DataSet dsInspect = objGlobaldata.Getdetails(sSqlstmt);

                if (dsInspect.Tables.Count > 0 && dsInspect.Tables[0].Rows.Count > 0)
                {
                    GenerateInspectionChecklistModels objInsp = new GenerateInspectionChecklistModels
                    {
                        id_inspection_checklist = dsInspect.Tables[0].Rows[0]["id_inspection_checklist"].ToString(),
                        cat_id = (dsInspect.Tables[0].Rows[0]["Category"].ToString()),
                        Category = obj.GetInspectionCategoryByID(dsInspect.Tables[0].Rows[0]["Category"].ToString()),
                        Inspection_by = objGlobaldata.GetEmpHrNameById(dsInspect.Tables[0].Rows[0]["Inspection_by"].ToString()),
                        Location = objGlobaldata.GetDivisionLocationById(dsInspect.Tables[0].Rows[0]["Location"].ToString()),
                        Project = dsInspect.Tables[0].Rows[0]["Project"].ToString(),
                        TagNo = dsInspect.Tables[0].Rows[0]["TagNo"].ToString(),
                        Comment = dsInspect.Tables[0].Rows[0]["Comment"].ToString(),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsInspect.Tables[0].Rows[0]["branch"].ToString()),
                        Department = objGlobaldata.GetMultiDeptNameById(dsInspect.Tables[0].Rows[0]["Department"].ToString()),
                        //  team = objGlobaldata.GetTeamNameByID(dsInspect.Tables[0].Rows[0]["team"].ToString()),
                    };
                    DateTime dtValue;
                    if (DateTime.TryParse(dsInspect.Tables[0].Rows[0]["Inspection_date"].ToString(), out dtValue))
                    {
                        objInsp.Inspection_date = dtValue;
                    }

                    sSqlstmt = "SELECT id_inspection_checklist_trans,id_inspection_checklist,id_inspection_question,"
                        + "id_inspection_rating,Action,ActionBy,Upload from t_inspection_checklist_trans where id_inspection_checklist='" + id + "'";

                    DataSet dsInspElement = objGlobaldata.Getdetails(sSqlstmt);
                    InspectionChecklistList objInspList = new InspectionChecklistList();
                    objInspList.lst = new List<InspectionChecklistModels>();

                    InspectionChecklistModels objIns = new InspectionChecklistModels();

                    for (int i = 0; dsInspElement.Tables.Count > 0 && i < dsInspElement.Tables[0].Rows.Count; i++)
                    {
                        InspectionChecklistModels objElements = new InspectionChecklistModels
                        {
                            id_ques = dsInspElement.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            id_inspection_question = objIns.GetInspectionQuestionNameById(dsInspElement.Tables[0].Rows[i]["id_inspection_question"].ToString()),
                            id_inspection_rating = objIns.GetInspectionRatingNameById(dsInspElement.Tables[0].Rows[i]["id_inspection_rating"].ToString()),
                            Action = dsInspElement.Tables[0].Rows[i]["Action"].ToString(),
                            ActionBy = objGlobaldata.GetEmpHrNameById(dsInspElement.Tables[0].Rows[i]["ActionBy"].ToString()),
                            Upload = dsInspElement.Tables[0].Rows[i]["Upload"].ToString(),
                        };
                        objInspList.lst.Add(objElements);
                    }

                    ViewBag.InspectionElement = objInspList;
                    ViewBag.SectionQuestions = objRating.GetSectionQuestions(objInsp.cat_id, objInsp.branch);
                    ViewBag.InspectionRating = objRating.GetInspectionRating();
                    return View(objInsp);
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("InspectionList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionChecklistDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("InspectionList");
        }

        [HttpPost]
        public ActionResult InspectionChecklistReport(FormCollection form)
        {
            string sid_inspection_checklist = form["id_inspection_checklist"];
            try
            {
                if (sid_inspection_checklist != "")
                {
                    InspectionCategoryModels obj = new InspectionCategoryModels();
                    InspctionQuestionModels objRating = new InspctionQuestionModels();
                    string sSqlstmt = "select id_inspection_checklist,Category,Inspection_date,Inspection_by,Location,Project,"
                        + "TagNo,branch,Department,team from t_inspection_checklist where id_inspection_checklist='" + sid_inspection_checklist + "' and Active=1";

                    DataSet dsInspect = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsInspect.Tables.Count > 0 && dsInspect.Tables[0].Rows.Count > 0)
                    {
                        GenerateInspectionChecklistModels objinsp = new GenerateInspectionChecklistModels
                        {
                            id_inspection_checklist = dsInspect.Tables[0].Rows[0]["id_inspection_checklist"].ToString(),
                            cat_id = (dsInspect.Tables[0].Rows[0]["Category"].ToString()),
                            Category = obj.GetInspectionCategoryByID(dsInspect.Tables[0].Rows[0]["Category"].ToString()),
                            Inspection_by = objGlobaldata.GetEmpHrNameById(dsInspect.Tables[0].Rows[0]["Inspection_by"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsInspect.Tables[0].Rows[0]["Location"].ToString()),
                            Project = dsInspect.Tables[0].Rows[0]["Project"].ToString(),
                            TagNo = dsInspect.Tables[0].Rows[0]["TagNo"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsInspect.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsInspect.Tables[0].Rows[0]["Department"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsInspect.Tables[0].Rows[0]["team"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsInspect.Tables[0].Rows[0]["Inspection_date"].ToString(), out dtValue))
                        {
                            objinsp.Inspection_date = dtValue;
                        }

                        CompanyModels objCompany = new CompanyModels();
                        dsInspect = objCompany.GetCompanyDetailsForReport(dsInspect);

                        dsInspect = objGlobaldata.GetReportDetails(dsInspect, objinsp.TagNo, objGlobaldata.GetCurrentUserSession().empid, "INSPECTION REPORT");
                        ViewBag.CompanyInfo = dsInspect;

                        ViewBag.ChecklistDetails = objinsp;

                        sSqlstmt = "SELECT id_inspection_checklist_trans,id_inspection_checklist,id_inspection_question,"
                            + "id_inspection_rating,Action,ActionBy,Upload from t_inspection_checklist_trans where id_inspection_checklist='" + objinsp.id_inspection_checklist + "'";
                        DataSet dsInspecElement = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.InspectionElement = dsInspecElement;
                        ViewBag.SectionQuestions = objRating.GetSectionQuestions(objinsp.cat_id, objinsp.branch);
                        ViewBag.InspectionRating = objRating.GetInspectionRating(obj.getInspectionCategoryIDByName(objinsp.Category));
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("InspectionList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("InspectionList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionChecklistReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("InspectionChecklistReportToPDF")
            {
                //FileName = "InspectionChecklistReport.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [AllowAnonymous]
        public JsonResult InspectionChecklistDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_inspection_checklist"] != null && form["id_inspection_checklist"] != "")
                {
                    InspectionChecklistModels Doc = new InspectionChecklistModels();

                    string sid_inspection_checklist = form["id_inspection_checklist"];

                    if (Doc.FunDeleteInspectionChecklist(sid_inspection_checklist))
                    {
                        TempData["Successdata"] = "Document deleted successfully";
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
                    TempData["alertdata"] = "Inspection checklist Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionChecklistDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult InspectionChecklistList(FormCollection form)
        {
            GenerateInspectionChecklistList objInspChecklist = new GenerateInspectionChecklistList();
            objInspChecklist.lstChk = new List<GenerateInspectionChecklistModels>();
            InspectionCategoryModels obj = new InspectionCategoryModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                obj.Category = form["Cate"];
                ViewBag.Cate = form["Cate"];
                ViewBag.Cates = form["Cat"];

                if (Request.QueryString["Category"] != null && Request.QueryString["Category"] != "" && Request.QueryString["branch"] != "" && Request.QueryString["branch"] != null)
                {
                    string sCat = Request.QueryString["Category"];
                    string sCategory = obj.getInspectionCategoryIDByName(sCat);
                    ViewBag.Cate = sCategory;
                    string sid_inspection_checklist = Request.QueryString["Category"];
                    string sSearchtext = "";
                    string branch_name = Request.QueryString["branch"];

                    string sSqlstmt = "select id_inspection_checklist,Category,Inspection_date,Inspection_by,Location,"
                        + "Project,TagNo,Comment,branch,Department,team from t_inspection_checklist where Category='" + sCategory + "' and Active=1";

                    if (branch_name != null && branch_name != "")
                    {
                        sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                        ViewBag.Branch_name = branch_name;
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                    }
                    sSqlstmt = sSqlstmt + sSearchtext + "order by Inspection_date desc";
                    DataSet dsInspect = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsInspect.Tables.Count > 0 && dsInspect.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsInspect.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                GenerateInspectionChecklistModels objInsp = new GenerateInspectionChecklistModels
                                {
                                    id_inspection_checklist = dsInspect.Tables[0].Rows[i]["id_inspection_checklist"].ToString(),
                                    Category = obj.GetInspectionCategoryByID(dsInspect.Tables[0].Rows[i]["Category"].ToString()),
                                    Inspection_by = objGlobaldata.GetEmpHrNameById(dsInspect.Tables[0].Rows[i]["Inspection_by"].ToString()),
                                    Location = objGlobaldata.GetDivisionLocationById(dsInspect.Tables[0].Rows[i]["Location"].ToString()),
                                    Project = dsInspect.Tables[0].Rows[i]["Project"].ToString(),
                                    TagNo = dsInspect.Tables[0].Rows[i]["TagNo"].ToString(),
                                    Comment = dsInspect.Tables[0].Rows[i]["Comment"].ToString(),
                                    branch = objGlobaldata.GetMultiCompanyBranchNameById(dsInspect.Tables[0].Rows[i]["branch"].ToString()),
                                    Department = objGlobaldata.GetMultiDeptNameById(dsInspect.Tables[0].Rows[i]["Department"].ToString()),
                                    // team = objGlobaldata.GetTeamNameByID(dsInspect.Tables[0].Rows[i]["team"].ToString()),
                                };
                                ViewBag.Category = obj.GetInspectionCategoryByID(dsInspect.Tables[0].Rows[i]["Category"].ToString());
                                DateTime dtValue;
                                if (DateTime.TryParse(dsInspect.Tables[0].Rows[i]["Inspection_date"].ToString(), out dtValue))
                                {
                                    objInsp.Inspection_date = dtValue;
                                }

                                objInspChecklist.lstChk.Add(objInsp);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in InspectionChecklistList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("InspectionList");
                    }
                }
                else
                {
                    //TempData["alertdata"] = "No data exists";
                    return RedirectToAction("InspectionList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objInspChecklist.lstChk.ToList());
        }

        //[AllowAnonymous]
        //public JsonResult InspectionlistListSearch(string branch_name, string CategoryName)
        //{
        //    GenerateInspectionChecklistList objInspChecklist = new GenerateInspectionChecklistList();
        //    objInspChecklist.lstChk = new List<GenerateInspectionChecklistModels>();
        //    InspectionCategoryModels obj = new InspectionCategoryModels();
        //    try
        //    {
        //        string sBranch_name = objGlobaldata.GetCurrentUserSession().Work_Location;
        //        string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
        //        ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
        //        string Category = obj.getInspectionCategoryIDByName(CategoryName);
        //        ViewBag.Cate = Category;
        //        if (Category != null && Category != "")
        //        {
        //            string sCat = Request.QueryString["Category"];
        //            string sCategory = obj.getInspectionCategoryIDByName(sCat);
        //            string sid_inspection_checklist = Request.QueryString["Category"];
        //            string sSearchtext = "";

        //            string sSqlstmt = "select id_inspection_checklist,Category,Inspection_date,Inspection_by,Location,"
        //               + "Project,TagNo,Comment from t_inspection_checklist where Category='" + Category + "' and Active=1";

        //            if (branch_name != null && branch_name != "")
        //            {
        //                sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
        //                ViewBag.Branch_name = branch_name;
        //            }
        //            else
        //            {
        //                sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
        //            }
        //            sSqlstmt = sSqlstmt + sSearchtext + "order by Inspection_date desc";
        //            DataSet dsInspect = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsInspect.Tables.Count > 0 && dsInspect.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < dsInspect.Tables[0].Rows.Count; i++)
        //                {
        //                    try
        //                    {
        //                        GenerateInspectionChecklistModels objInsp = new GenerateInspectionChecklistModels
        //                        {
        //                            id_inspection_checklist = dsInspect.Tables[0].Rows[i]["id_inspection_checklist"].ToString(),
        //                            Category = obj.GetInspectionCategoryByID(dsInspect.Tables[0].Rows[i]["Category"].ToString()),
        //                            Inspection_by = objGlobaldata.GetEmpHrNameById(dsInspect.Tables[0].Rows[i]["Inspection_by"].ToString()),
        //                            Location = objGlobaldata.GetCompanyBranchNameById(dsInspect.Tables[0].Rows[i]["Location"].ToString()),
        //                            Project = dsInspect.Tables[0].Rows[i]["Project"].ToString(),
        //                            TagNo = dsInspect.Tables[0].Rows[i]["TagNo"].ToString(),
        //                            Comment = dsInspect.Tables[0].Rows[i]["Comment"].ToString(),
        //                        };
        //                        ViewBag.Category = obj.GetInspectionCategoryByID(dsInspect.Tables[0].Rows[i]["Category"].ToString());
        //                        DateTime dtValue;
        //                        if (DateTime.TryParse(dsInspect.Tables[0].Rows[i]["Inspection_date"].ToString(), out dtValue))
        //                        {
        //                            objInsp.Inspection_date = dtValue;
        //                        }

        //                        objInspChecklist.lstChk.Add(objInsp);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        objGlobaldata.AddFunctionalLog("Exception in InspectionlistListSearch: " + ex.ToString());
        //                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in InspectionlistListSearch: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return Json("Success");
        //}

        [AllowAnonymous]
        public ActionResult InspectionChecklistListSearch(int? page, string InspfromDate, string InsptoDate, string chkAll, string Category)
        {
            GenerateInspectionChecklistList objInspChecklist = new GenerateInspectionChecklistList();
            objInspChecklist.lstChk = new List<GenerateInspectionChecklistModels>();
            InspectionCategoryModels obj = new InspectionCategoryModels();
            try
            {
                if (Category != null && Category != "")
                {
                    string sCat = Category;
                    string sCategory = obj.getInspectionCategoryIDByName(sCat);
                    string sid_inspection_checklist = Request.QueryString["Category"];
                    string sSqlstmt = "select id_inspection_checklist,Category,Inspection_date,Inspection_by,Location,"
                       + "Project,TagNo,Comment,branch,Department,team from t_inspection_checklist where Category='" + sCategory + "' and Active=1";
                    string sSearchtext = "";
                    DateTime dtFromdate, dtToDate;
                    if (chkAll == null && chkAll != "All")
                    {
                        if (InspfromDate != null && DateTime.TryParse(InsptoDate, out dtToDate) && DateTime.TryParse(InspfromDate, out dtFromdate))
                        {
                            ViewBag.InspfromDate = InspfromDate;
                            ViewBag.InsptoDate = InsptoDate;
                            if (sSearchtext != "")
                            {
                                sSearchtext = sSearchtext + " and Inspection_date between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                            }
                            else
                            {
                                sSearchtext = sSearchtext + " and Inspection_date between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                            }
                        }
                    }
                    else
                    {
                        ViewBag.chkAll = true;
                    }
                    sSqlstmt = sSqlstmt + sSearchtext + " order by Inspection_date desc";
                    DataSet dsInspect = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsInspect.Tables.Count > 0 && dsInspect.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsInspect.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                GenerateInspectionChecklistModels objInsp = new GenerateInspectionChecklistModels
                                {
                                    id_inspection_checklist = dsInspect.Tables[0].Rows[i]["id_inspection_checklist"].ToString(),
                                    Category = obj.GetInspectionCategoryByID(dsInspect.Tables[0].Rows[i]["Category"].ToString()),
                                    Inspection_by = objGlobaldata.GetEmpHrNameById(dsInspect.Tables[0].Rows[i]["Inspection_by"].ToString()),
                                    Location = objGlobaldata.GetDivisionLocationById(dsInspect.Tables[0].Rows[i]["Location"].ToString()),
                                    Project = dsInspect.Tables[0].Rows[i]["Project"].ToString(),
                                    TagNo = dsInspect.Tables[0].Rows[i]["TagNo"].ToString(),
                                    Comment = dsInspect.Tables[0].Rows[i]["Comment"].ToString(),
                                    branch = objGlobaldata.GetMultiCompanyBranchNameById(dsInspect.Tables[0].Rows[i]["branch"].ToString()),
                                    Department = objGlobaldata.GetMultiDeptNameById(dsInspect.Tables[0].Rows[i]["Department"].ToString()),
                                    //  team = objGlobaldata.GetTeamNameByID(dsInspect.Tables[0].Rows[i]["team"].ToString()),
                                };
                                ViewBag.Category = obj.GetInspectionCategoryByID(dsInspect.Tables[0].Rows[i]["Category"].ToString());
                                DateTime dtValue;
                                if (DateTime.TryParse(dsInspect.Tables[0].Rows[i]["Inspection_date"].ToString(), out dtValue))
                                {
                                    objInsp.Inspection_date = dtValue;
                                }

                                objInspChecklist.lstChk.Add(objInsp);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in InspectionChecklistListSearch: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("InspectionList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("InspectionList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objInspChecklist.lstChk.ToList().ToPagedList(page ?? 1, 40));
        }

        [AllowAnonymous]
        public ActionResult InspectionChecklistEdit(int? page)
        {
            try
            {
                if (Request.QueryString["id_inspection_checklist"] != null && Request.QueryString["id_inspection_checklist"] != "")
                {
                    InspectionCategoryModels obj = new InspectionCategoryModels();
                    string sid_inspection_checklist = Request.QueryString["id_inspection_checklist"];

                    string sSqlstmt = "select id_inspection_checklist,Category,Inspection_date,Inspection_by"
                    + ",Location,Project,TagNo,Comment,branch,Department,team from t_inspection_checklist where id_inspection_checklist='" + sid_inspection_checklist + "'";

                    DataSet dsChecklist = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsChecklist.Tables.Count > 0 && dsChecklist.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.id_inspection_checklist = dsChecklist.Tables[0].Rows[0]["id_inspection_checklist"].ToString();
                        ViewBag.Cate_id = (dsChecklist.Tables[0].Rows[0]["Category"].ToString());
                        ViewBag.Category = obj.GetInspectionCategoryByID(dsChecklist.Tables[0].Rows[0]["Category"].ToString());
                        ViewBag.Inspection_by = objGlobaldata.GetEmpHrNameById(dsChecklist.Tables[0].Rows[0]["Inspection_by"].ToString());
                        ViewBag.Location = dsChecklist.Tables[0].Rows[0]["Location"].ToString();
                        ViewBag.Project = dsChecklist.Tables[0].Rows[0]["Project"].ToString();
                        ViewBag.TagNo = dsChecklist.Tables[0].Rows[0]["TagNo"].ToString();
                        ViewBag.Comment = dsChecklist.Tables[0].Rows[0]["Comment"].ToString();
                        ViewBag.branch = dsChecklist.Tables[0].Rows[0]["branch"].ToString();
                        ViewBag.branchName = objGlobaldata.GetMultiCompanyBranchNameById(dsChecklist.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = dsChecklist.Tables[0].Rows[0]["Department"].ToString();
                        ViewBag.Team = dsChecklist.Tables[0].Rows[0]["team"].ToString();

                        //ViewBag.BranchList = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.LocationList = objGlobaldata.GetLocationbyMultiDivision(dsChecklist.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.DepartmentList = objGlobaldata.GetDepartmentListbox(dsChecklist.Tables[0].Rows[0]["branch"].ToString());
                        //     ViewBag.TeamList = objGlobaldata.GetMultiTeambyMultiGroup(dsChecklist.Tables[0].Rows[0]["Department"].ToString());

                        DateTime dtValue;
                        if (DateTime.TryParse(dsChecklist.Tables[0].Rows[0]["Inspection_date"].ToString(), out dtValue))
                        {
                            ViewBag.Inspection_date = dtValue;
                        }

                        sSqlstmt = "SELECT id_inspection_checklist_trans,id_inspection_checklist,id_inspection_question,id_inspection_rating"
                        + ",Action,ActionBy,Upload FROM t_inspection_checklist_trans where id_inspection_checklist='"
                             + ViewBag.id_inspection_checklist + "'";
                        DataSet dsInspectionElement = objGlobaldata.Getdetails(sSqlstmt);
                        InspectionChecklistList objInspectionList = new InspectionChecklistList();
                        objInspectionList.lst = new List<InspectionChecklistModels>();
                        InspectionCategoryModels cat = new InspectionCategoryModels();
                        InspectionChecklistModels objIns = new InspectionChecklistModels();
                        InspctionQuestionModels objRating = new InspctionQuestionModels();
                        Dictionary<string, string> dicInspectionElements = new Dictionary<string, string>();
                        InspectionChecklistModels objElements = new InspectionChecklistModels();

                        if (dsInspectionElement.Tables.Count > 0 && dsInspectionElement.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; dsInspectionElement.Tables.Count > 0 && i < dsInspectionElement.Tables[0].Rows.Count; i++)
                            {
                                ViewBag.dsInspection = dsInspectionElement;
                            }
                            for (int i = 0; dsInspectionElement.Tables.Count > 0 && i < dsInspectionElement.Tables[0].Rows.Count; i++)
                            {
                                objElements = new InspectionChecklistModels
                                {
                                    id_inspection_checklist = dsInspectionElement.Tables[0].Rows[i]["id_inspection_checklist"].ToString(),
                                    id_inspection_question = dsInspectionElement.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                                    id_inspection_rating = dsInspectionElement.Tables[0].Rows[i]["id_inspection_rating"].ToString(),
                                    Action = dsInspectionElement.Tables[0].Rows[i]["Action"].ToString(),
                                    ActionBy = objGlobaldata.GetEmpHrNameById(dsInspectionElement.Tables[0].Rows[i]["ActionBy"].ToString()),
                                    Upload = dsInspectionElement.Tables[0].Rows[i]["Upload"].ToString(),
                                };
                                dicInspectionElements.Add(dsInspectionElement.Tables[0].Rows[i]["id_inspection_question"].ToString(), dsInspectionElement.Tables[0].Rows[i]["id_inspection_rating"].ToString());
                                objInspectionList.lst.Add(objElements);
                            }
                        }
                        else
                        {
                            TempData["alertdata"] = "No data exists";
                            return RedirectToAction("InspectionList");
                        }
                        ViewBag.InspectionElement = objInspectionList;
                        ViewBag.dicInspectionElement = dicInspectionElements;
                        ViewBag.InspectionQuestions = objRating.GetInspectionQuestions(ViewBag.Cate_id, ViewBag.branch);
                        ViewBag.InspectionRating = objRating.GetInspectionRating(ViewBag.Cate_id);
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.SectionQuestions = objRating.GetSectionQuestions(ViewBag.Cate_id, ViewBag.branch);

                        return View();
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("InspectionChecklistList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("InspectionChecklistList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionChecklistEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("InspectionChecklistList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InspectionChecklistEdit(GenerateInspectionChecklistModels objInspChecklist, FormCollection form)
        {
            InspectionCategoryModels objCat = new InspectionCategoryModels();
            string CatId = objCat.GetInspectionCategoryByID(objInspChecklist.Category);
            ViewBag.branch = form["branchs"];
            try
            {
                InspectionCategoryModels obj = new InspectionCategoryModels();
                objInspChecklist.id_inspection_checklist = form["id_inspection_checklist"];
                objInspChecklist.Category = obj.getInspectionCategoryIDByName(form["Category"]);
                objInspChecklist.Inspection_by = form["Inspection_by"];
                objInspChecklist.Location = form["Location"];
                objInspChecklist.Project = form["Project"];
                objInspChecklist.TagNo = form["TagNo"];
                objInspChecklist.Comment = form["Comment"];
                objInspChecklist.branch = form["branchs"];
                objInspChecklist.Department = form["Department"];
                objInspChecklist.team = form["team"];

                DateTime dateValue;
                if (DateTime.TryParse(form["Inspection_date"], out dateValue) == true)
                {
                    objInspChecklist.Inspection_date = dateValue;
                }

                InspctionQuestionModels objInsChk = new InspctionQuestionModels();

                InspectionChecklistList objInsp = new InspectionChecklistList();
                objInsp.lst = new List<InspectionChecklistModels>();

                // MultiSelectList InspectionQuestions = objInsChk.GetInspectionQuestionsListbox(objInspChecklist.Category);
                MultiSelectList InspectionQuestions = objInsChk.GetInspectionQuestions(objInspChecklist.Category, ViewBag.branch);
                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in InspectionQuestions)
                {
                    if (i <= Convert.ToInt16(form["itmctn"]))
                    {
                        InspectionChecklistModels objElements = new InspectionChecklistModels();

                        objElements.id_inspection_question = form["InspectionQuestions " + item.Value];
                        objElements.id_inspection_rating = form["id_inspection_rating " + item.Value];
                        objElements.Action = form["Action " + i];
                        objElements.ActionBy = form["ActionBy " + i];
                        objElements.id_inspection_checklist_trans = form["id_inspection_checklist_trans " + i];

                        string upload = form["Upload" + i];
                        if (upload != null)
                        {
                            objElements.Upload = form["Upload" + i];
                        }
                        objInsp.lst.Add(objElements);
                    }
                    i++;
                }

                if (objInspChecklist.FunUpdateInspectionChecklist(objInspChecklist, objInsp))
                {
                    TempData["Successdata"] = "Inspection Updated Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InspectionChecklistEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("InspectionChecklistList", new { Category = CatId, branch = ViewBag.branch });
        }
    }
}