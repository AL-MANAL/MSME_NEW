using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.IO;
using System.Data;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class DropDownItemsController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public DropDownItemsController()
        {
            ViewBag.Menutype = "Settings";
        }

        //
        // GET: /DropDownItems/
         
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /DropDownItems/AddItems
         
        [AllowAnonymous]
        public ActionResult AddItems(string Header_id = "")
        {
            DropDownItemsModels objDropDown = new DropDownItemsModels();
            try
            {
                ViewBag.SubMenutype = "Items";
                MultiSelectList lstDropDownHeader = objDropDown.GetDropDownHeaderListbox();
                ViewBag.DropDownHeader = lstDropDownHeader;//GetDepartmentList();

                if (Header_id != null && Header_id != "")
                {
                    ViewBag.dsItems = objGlobaldata.Getdetails("select item_id, header_id, item_desc, item_fulldesc,isDefault from dropdownitems where header_id='" + Header_id + "' order by item_desc asc");
                    ViewBag.Header_id = Header_id;
                }
                //else
                //{
                //    foreach (var item in lstDropDownHeader)
                //    {
                //        ViewBag.DropDownItems = objDropDown.GetDropDownItemsListbox(item.Value);
                //        ViewBag.Header_id = item.Value;
                //        break;
                //    }
                //}
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddItems: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objDropDown);
        }

         
        [HttpPost]
        public JsonResult GetDropdownItems(string header_id)
        {
            DropDownItemsModels objDropDown = new DropDownItemsModels();

            return Json(objDropDown.GetDropDownItemsListbox(header_id));
        }

         //
        // POST: /DropDownItems/AddItems
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItems(DropDownItemsModels objDropDownItemsModels, FormCollection form)
        {
            try
            {
                
                if(form["isDefault"] != "" && form["isDefault"] != null)
                {
                    objDropDownItemsModels.isDefault = "yes";
                }
              
                if (objDropDownItemsModels.FunAddItems(objDropDownItemsModels))
                    {
                        //objEmployeeModel.MailTempPassword(objEmployeeModel.emailAddress);
                        TempData["Successdata"] = "Added Item details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddItems: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddItems", new { Header_id = objDropDownItemsModels.header_id });
        }

        //
        // GET: /DropDownItems/UpdateItems
         
        [AllowAnonymous]
        public JsonResult UpdateItems(string item_id, string header_id, string item_desc, string item_fulldesc, string isDefault)
        {
            DropDownItemsModels objDropDownItemsModels = new DropDownItemsModels();

            try
            {
                objDropDownItemsModels.item_id = item_id;
                objDropDownItemsModels.item_desc = item_desc;
                objDropDownItemsModels.item_fulldesc = item_fulldesc;
                objDropDownItemsModels.header_id = header_id;
                if (isDefault == "true")
                {
                    objDropDownItemsModels.isDefault = "yes";
                }
                else
                {
                    objDropDownItemsModels.isDefault = null;
                }
                
                if (objDropDownItemsModels.FunUpdateItems(objDropDownItemsModels))
                {
                    //objEmployeeModel.MailTempPassword(objEmployeeModel.emailAddress);
                    TempData["Successdata"] = "Updated Item details successfully";
                    return Json("Update Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UpdateItems: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Update Failed");

           // return RedirectToAction("AddItems", new { Header_id = header_id });
        }

        //
        // GET: /DropDownItems/DeleteItems
         
         [HttpPost]
        //[AllowAnonymous]
        public JsonResult DeleteItems(FormCollection form)
        {
            string item_id = form["item_id"];
            string header_id = form["header_id"];

            if (item_id != "")
            {
                DropDownItemsModels objDropDownItemsModels = new DropDownItemsModels();
                try
                {
                    if (objDropDownItemsModels.FunDeleteItems(item_id))
                    {
                        TempData["Successdata"] = "Deleted Item details successfully";
                        return Json("Deleted Successfully");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }

                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in DeleteItems: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }

            //return RedirectToAction("AddItems", new { Header_id = header_id });
            return Json("Delete Failed, Null Id");
        }

         
        [HttpPost]
        public JsonResult GetHeaderItems(string Header_id)
        {
            DropDownItemsModels objDropDownItemsModels = new DropDownItemsModels();
            MultiSelectList lstItems = objDropDownItemsModels.GetDropDownItemsListbox1(Header_id);

            return Json(lstItems);
        }

        [AllowAnonymous]
        public JsonResult DropdownAllItemDelete(FormCollection form)
        {
            try
            {

                if (form["header_id"] != null && form["header_id"] != "")
                {

                    DropDownItemsModels Doc = new DropDownItemsModels();
                    string sheader_id = form["header_id"];

                    if (Doc.FunDeleteDropdownAllItem(sheader_id))
                    {
                        TempData["Successdata"] = "All Dropdown items deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in DropdownItemDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }
}
