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

namespace ISOStd.Controllers
{
    public class InspectionController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        public InspectionController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "Inspection";
        }

        public ActionResult AddInspectionQuestions(string sCategory, string sSection, string branch_name)
        {
            try
            {
                InspectionCategoryModels obj = new InspectionCategoryModels();
                InspctionQuestionModels objQst = new InspctionQuestionModels();

              //  ViewBag.Category = obj.GetInspectionCategoryListbox();
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
                objGlobaldata.AddFunctionalLog("Exception in AddInspectionQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInspectionQuestions(InspctionQuestionModels objInspModels, FormCollection form)
        {
            InspectionCategoryModels obj = new InspectionCategoryModels();
            InspctionQuestionModels objQst = new InspctionQuestionModels();
            try
            {
                ViewBag.Category_Id = objInspModels.Category;
                ViewBag.Section_Id = objInspModels.Section;
                // ViewBag.Emp_Id = objInspModels.resp_person;
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                //ViewBag.Category = obj.GetInspectionCategoryListbox();
                ViewBag.Category = obj.GetInspectionCategoryList();
                ViewBag.Section = obj.GetInspectionSectionListbox(objInspModels.Category, sBranch_name);

                if (objQst.FunAddInspectionQuestions(objInspModels))
                {
                    TempData["Successdata"] = "Added Inspection Questions successfully";
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

            ViewBag.CategoryQuestions = objQst.GetInspectionQuestionsListbox(objInspModels.Category);

            return View(objInspModels);
        }
        
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

        public ActionResult AddInspectionSection(string sCategory)
        {
            try
            {
                InspectionCategoryModels obj = new InspectionCategoryModels();
                InspctionQuestionModels objQst = new InspctionQuestionModels();
                ViewBag.Category = obj.GetInspectionCategoryListbox();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Branch_name = objGlobaldata.GetCurrentUserSession().division;
                if (sCategory != null && sCategory != "")
                {
                    ViewBag.Category_Id = sCategory;
                    //ViewBag.CategorySection = objQst.GetInspectionSectionListbox(sCategory, ViewBag.Branch_name);  
                    //ViewBag.CategoryRespPerson= obj.GetInspRespPersonList(sCategory, ViewBag.Branch_name);
                    ViewBag.dsItems = objGlobaldata.Getdetails("select id_inspection, Section, Resp_person from t_inspection_section where Category='" + sCategory + "' and branch='" + ViewBag.Branch_name + "' ");
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
                ViewBag.Category_Id = objInspModels.Category;
                ViewBag.Category = obj.GetInspectionCategoryListbox();
                ViewBag.Branch_name = objGlobaldata.GetCurrentUserSession().division;

                objInspModels.Resp_person = form["Resp_person"];

                if (objQst.FunAddInspectionSection(objInspModels))
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
                objGlobaldata.AddFunctionalLog("Exception in AddInspectionSection: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            ViewBag.CategorySection = objQst.GetInspectionSectionListbox(objInspModels.Category, ViewBag.Branch_name);
            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

            return View(objInspModels);
        }

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

        public JsonResult InspectionSectionDelete1(string id_inspection, string Category)
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

        [AllowAnonymous]
        public JsonResult InspectionSectionUpdate(int id_inspection, string Section, string[] Resp_person)
        {
            InspctionQuestionModels obj = new InspctionQuestionModels();
            try
            {
                var Resp_person1 = string.Join(",", Resp_person);
                //int id_inspections = Convert.ToInt32(id_inspection);
                if (obj.FunUpdateInspectionSection(id_inspection, Section, Resp_person1))
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
        public JsonResult FunGetSectionRespPerson(string Category, string branch_name)
        {
            InspectionCategoryModels obj = new InspectionCategoryModels();
            InspctionQuestionModels objMdl = new InspctionQuestionModels();
            var data = new object();
            string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
            if (branch_name != "" && branch_name != null)
            {
                data = obj.GetInspSectionRespPersonList(Category, branch_name);
                //ViewBag.Resp_person = obj.newfucntion(Category, branch_name);
            }
            else
            {
                data = obj.GetInspSectionRespPersonList(Category, sBranch_name);
                //ViewBag.Resp_person = obj.newfucntion(Category, sBranch_name);
            }
            return Json(data);
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

                   // ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                    ViewBag.Department = objGlobaldata.GetDepartmentListbox(sbranch);
                    ViewBag.Location = objGlobaldata.GetDivisionLocationList(sbranch);

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
                FileName = "GenerateInspectionChecklistReport.pdf",
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
                        + "Project,TagNo,Comment,branch,Department from t_inspection_checklist where id_inspection_checklist='" + sid_inspection_checklist + "' and Active=1";

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
                    + "Project,TagNo,Comment,branch,Department from t_inspection_checklist where id_inspection_checklist='" + id + "' and Active=1";

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
                        + "TagNo,branch,Department,Comment from t_inspection_checklist where id_inspection_checklist='" + sid_inspection_checklist + "' and Active=1";

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
                            Comment = dsInspect.Tables[0].Rows[0]["Comment"].ToString(),
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
                FileName = "InspectionChecklistReport.pdf",
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
                        + "Project,TagNo,Comment,branch,Department from t_inspection_checklist where Category='" + sCategory + "' and Active=1";

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
                       + "Project,TagNo,Comment,branch,Department from t_inspection_checklist where Category='" + sCategory + "' and Active=1";
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
                                    branch = objGlobaldata.GetMultiCompanyBranchNameById(dsInspect.Tables[0].Rows[0]["branch"].ToString()),
                                    Department = objGlobaldata.GetMultiDeptNameById(dsInspect.Tables[0].Rows[0]["Department"].ToString()),
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
                    + ",Location,Project,TagNo,Comment,branch,Department from t_inspection_checklist where id_inspection_checklist='" + sid_inspection_checklist + "'";

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

                        //ViewBag.BranchList = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.LocationList = objGlobaldata.GetLocationbyMultiDivision(dsChecklist.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.DepartmentList = objGlobaldata.GetDepartmentList1(dsChecklist.Tables[0].Rows[0]["branch"].ToString());

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