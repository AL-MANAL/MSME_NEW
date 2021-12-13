using ISOStd.Models;
using System;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    public class DocumentLevelsController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public ActionResult AddDocumentLevel(string sDocLevelId)
        {
            try
            {
                DocumentLevelsModels objDocType = new DocumentLevelsModels();
                ViewBag.DocumentLevels = objDocType.GetDocLevelListbox();

                if (sDocLevelId != null && sDocLevelId != "")
                {
                    string sql = "Select id_levels_details,id_doc_level,description from t_document_levels_details where id_doc_level='" + sDocLevelId + "' and active=1";
                    ViewBag.dsDoclevelDetails = objGlobaldata.Getdetails(sql);
                    ViewBag.DocLevelId = sDocLevelId;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocumentLevels: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDocumentLevel(DocumentLevelsModels objModels, FormCollection form)
        {
            try
            {
                objModels.Document_Level = form["Document_Level"];

                if (objModels.FunAddDocLevelType(objModels))
                {
                    TempData["Successdata"] = "Added Document level details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                return RedirectToAction("AddDocumentLevel");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDocumentLevel: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AddDocumentLevel");
        }

        //public ActionResult InitializeDocLevelType(string sDoclevelId = "")
        //{
        //    DocumentLevelsModels objDocType = new DocumentLevelsModels();
        //    ViewBag.DocumentLevels = objDocType.GetDocLevelListbox();
        //    ViewBag.Roles = objGlobaldata.GetRoles();

        //    if (sDoclevelId != null && sDoclevelId != "")
        //    {
        //        MeetingAgendaModels objAgenda = new MeetingAgendaModels();
        //        string sql = "Select * from t_document_levels_details where DocLevelId='" + sDoclevelId + "'";
        //        ViewBag.dsDoclevel = objGlobaldata.Getdetails(sql);
        //        ViewBag.DoclevelId = sDoclevelId;
        //    }

        //    return View();
        //}

        ////--------------- Start Directorate-----------------

        //public ActionResult AddDirectoratecode()
        //{
        //    try
        //    {
        //            string sql = "Select * from t_company_branch where active=1;";

        //            ViewBag.dsDirectorate = objGlobaldata.Getdetails(sql);
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in Directorate: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddDirectoratecode(DirectorateModels obj, FormCollection form)
        //{
        //    try
        //    {
        //            if (obj.FunAddDirectorate(obj))
        //            {
        //                TempData["Successdata"] = "Added Directorate details successfully";
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //            }

        //        return InitializeDirectoratecode();
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddMeetingType: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return InitializeDirectoratecode();
        //}

        //public ActionResult InitializeDirectoratecode()
        //{
        //    try
        //    {
        //        string sql = "Select * from t_company_branch;";
        //        ViewBag.dsDirectorate = objGlobaldata.Getdetails(sql);
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in Directorate: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View();
        //}

        //[AllowAnonymous]
        //public JsonResult DocDirectorateUpdate(string id_element, string Type_Desc, string Type_Details)
        //{
        //    DirectorateModels obj = new DirectorateModels();
        //    obj.Directorate_id = id_element;
        //    obj.Type_Desc = Type_Desc;
        //    obj.Type_Details = Type_Details;
        //    try
        //    {
        //        if (obj.FunUpdateDirectorate(obj))
        //        {
        //            TempData["Successdata"] = "Updated Directorate details successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in DirectorateUpdate: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return Json("Failed");
        //}

        //[AllowAnonymous]
        //public JsonResult DirectorateDelete(string id_element)
        //{
        //    DirectorateModels obj = new DirectorateModels();
        //    obj.Directorate_id = id_element;

        //    try
        //    {
        //        if (obj.FunDeleteDirectorate(obj))
        //        {
        //            TempData["Successdata"] = " Document Directorate deleted successfully";

        //            //AddMeetingType(objMeetingAgenda.MeetingType);
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in DirectorateDelete: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json("Failed");
        //}

        ////--------------- End Directorate-----------------

        ////--------------- Start Group-----------------

        //public ActionResult AddGroupcode(string Directorate_id)
        //{
        //    try
        //    {
        //        DirectorateModels objDocType = new DirectorateModels();

        //        if (Directorate_id != null && Directorate_id != "")
        //        {
        //            string sql = "Select * from t_departments where find_in_set('" + Directorate_id + "',branch)";
        //            ViewBag.dsDirectorateGroup = objGlobaldata.Getdetails(sql);
        //            ViewBag.Directorate_id = Directorate_id;
        //        }
        //        ViewBag.Directorate = objGlobaldata.GetCompanyBranchListbox();
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in Group: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddGroupcode(DirectorateGroupModels obj, FormCollection form)
        //{
        //    try
        //    {
        //        if (obj.FunAddGroup(obj))
        //        {
        //            TempData["Successdata"] = "Added Group details successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }

        //        return InitializeGroupCode(obj.Directorate_id);

        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in GroupAdd: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return InitializeGroupCode();
        //}

        //public ActionResult InitializeGroupCode(string Directorate_id = "")
        //{
        //    DirectorateModels objDocType = new DirectorateModels();

        //    if (Directorate_id != null && Directorate_id != "")
        //    {
        //        string sql = "Select * from t_departments where branch='" + Directorate_id + "'";

        //        ViewBag.dsDirectorateGroup = objGlobaldata.Getdetails(sql);
        //        ViewBag.Directorate_id = Directorate_id;
        //    }
        //    //ViewBag.Directorate = objDocType.GetDocDirectorateListbox();
        //    ViewBag.Directorate = objGlobaldata.GetCompanyBranchListbox();

        //    return View();
        //}

        //[AllowAnonymous]
        //public JsonResult DocGroupUpdate(string id_element, string Type_Desc, string Type_Details)
        //{
        //    DirectorateGroupModels obj = new DirectorateGroupModels();
        //    obj.Group_id = id_element;
        //    obj.Type_Desc = Type_Desc;
        //    obj.Type_Details = Type_Details;
        //    try
        //    {
        //        if (obj.FunUpdateGroup(obj))
        //        {
        //            TempData["Successdata"] = "Updated Group details successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in GroupUpdate: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return Json("Failed");
        //}

        //[AllowAnonymous]
        //public JsonResult GroupDelete(string id_element)
        //{
        //    DirectorateGroupModels obj = new DirectorateGroupModels();
        //    obj.Group_id = id_element;
        //    try
        //    {
        //        if (obj.FunDeleteGroup(obj))
        //        {
        //            TempData["Successdata"] = " Document Group deleted successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in GroupDelete: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return Json("Failed");
        //}

        ////--------------- End Group-----------------

        //public bool AddDocLevelDetails(DocLevelDetailsModels obj)
        //{
        //    return obj.FunAdd(obj);
        //}

        [AllowAnonymous]
        public ActionResult DocLevelUpdate(DocumentLevelsModels objLevel)
        {
            try
            {
                if (objLevel.FunUpdateDocLevelType(objLevel))
                {
                    TempData["Successdata"] = "Updated Document Level details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocLevelUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        public ActionResult DocLevelDelete(string Id)
        {
            DocumentLevelsModels objModels = new DocumentLevelsModels();
            try
            {
                if (objModels.FunDeleteDocLevelType(Id))
                {
                    TempData["Successdata"] = "Deleted Document Level details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocLevelDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public JsonResult GetDocLevelDetailsList(string sDocId)
        {
            DocumentLevelsModels obj = new DocumentLevelsModels();
            return Json(obj.GetDocLeveldetailsListbox(sDocId));
        }

        //[AllowAnonymous]
        //public JsonResult GetGroupList(string sDocId)
        //{
        //    DirectorateGroupModels obj = new DirectorateGroupModels();
        //   // return Json(obj.GetDocGroupListbox(sDocId));
        //    return Json(objGlobaldata.GetFullDepartmentListbyBranch(sDocId));
        //}

        //[AllowAnonymous]
        //public JsonResult GetTeamList(string sDocId)
        //{
        //    GroupTeamModels obj = new GroupTeamModels();
        //    return Json(obj.GetDocTeamListbox(sDocId));
        //}

        //[AllowAnonymous]
        //public JsonResult DocTeamUpdate(string id_element, string Type_Desc, string Type_Details)
        //{
        //    GroupTeamModels obj = new GroupTeamModels();
        //    obj.Team_id = id_element;
        //    obj.Type_Desc = Type_Desc;
        //    obj.Type_Details = Type_Details;
        //    try
        //    {
        //        if (obj.FunUpdate(obj))
        //        {
        //            TempData["Successdata"] = "Updated Team details successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in TeamUpdate: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return Json("Failed");
        //}

        //[AllowAnonymous]
        //public JsonResult TeamDelete(string id_element)
        //{
        //    GroupTeamModels obj = new GroupTeamModels();
        //    obj.Team_id = id_element;

        //    try
        //    {
        //        if (obj.FunDelete(obj))
        //        {
        //            TempData["Successdata"] = " Document Team deleted successfully";

        //            //AddMeetingType(objMeetingAgenda.MeetingType);
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in TeamDelete: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return Json("Failed");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLevelDetails(DocumentLevelsModels objModels, FormCollection form)
        {
            try

            {
                objModels.id_doc_level = form["id_doc_level"];
                objModels.description = form["description"];

                if (objModels.FunAddDocLevelDetails(objModels))
                {
                    TempData["Successdata"] = "Added Document details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                return RedirectToAction("AddDocumentLevel", "DocumentLevels", new { sDocLevelId = objModels.id_doc_level });
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLevelDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddDocumentLevel", "DocumentLevels");
        }

        [AllowAnonymous]
        public JsonResult DocDetailsUpdate(string id_element, string description)
        {
            DocumentLevelsModels obj = new DocumentLevelsModels();
            obj.id_levels_details = id_element;
            obj.description = description;

            try
            {
                if (obj.FunUpdateDocLevelDetails(obj))
                {
                    TempData["Successdata"] = "Updated Document details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocDetailsUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public JsonResult DocDetailsDelete(string id_element)
        {
            DocumentLevelsModels obj = new DocumentLevelsModels();
            obj.id_levels_details = id_element;
            try
            {
                if (obj.FunDeleteDocLevelDetails(obj))
                {
                    TempData["Successdata"] = " Document Details deleted successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DocDetailsDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult doesDocNameExist(string Document_Level)
        {
            DocumentLevelsModels obj = new DocumentLevelsModels();
            var user = obj.CheckForDocNameExists(Document_Level);

            return Json(user);
        }
    }
}