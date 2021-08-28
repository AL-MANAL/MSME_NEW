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
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class EmmetController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddRoundbar()
        {
            try
            {
                ViewBag.CatList = objGlobaldata.getMaterialCategoryRoundbarList();
                ViewBag.Suppplier = objGlobaldata.GetSupplierList();
               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRoundbar: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddRoundbar(EmmetModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> po_upload, IEnumerable<HttpPostedFileBase> mtc_upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                EmmetModelsList objList = new EmmetModelsList();
                objList.EmmetList = new List<EmmetModels>();

              
                DateTime dateValue;
                if (DateTime.TryParse(form["added_date"], out dateValue) == true)
                {
                    objModel.added_date = dateValue;
                }
                if (po_upload != null && files.ContentLength > 0)
                {
                    objModel.po_upload = "";
                    foreach (var file in po_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Emmet"), Path.GetFileName(file.FileName));
                            string sFilename = "PO" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.po_upload = objModel.po_upload + "," + "~/DataUpload/MgmtDocs/Emmet/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddRoundbar-upload: " + ex.ToString());
                        }
                    }
                    objModel.po_upload = objModel.po_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (mtc_upload != null && files.ContentLength > 0)
                {
                    objModel.mtc_upload = "";
                    foreach (var file in mtc_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Emmet"), Path.GetFileName(file.FileName));
                            string sFilename = "MTC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.mtc_upload = objModel.mtc_upload + "," + "~/DataUpload/MgmtDocs/Emmet/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddRoundbar-upload: " + ex.ToString());
                        }
                    }
                    objModel.mtc_upload = objModel.mtc_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["heatno" + i] != null && form["length" + i] != null  && form["price" + i] != null)
                    {
                        EmmetModels objEmmet = new EmmetModels();

                        objEmmet.heatno = form["heatno" + i];
                        objEmmet.length =Convert.ToDecimal(form["length" + i]);
                        objEmmet.material = (form["material" + i]);
                        objEmmet.category = (form["category" + i]);
                        objEmmet.qty = Convert.ToInt32(form["qty" + i]);
                        objEmmet.price = Convert.ToDecimal(form["price" + i]);
                        objEmmet.tprice = Convert.ToDecimal(form["tprice" + i]);
                        objList.EmmetList.Add(objEmmet);
                    }
                }


                if (objModel.FunAddRoundBar(objModel, objList))
                {
                    TempData["Successdata"] = "Added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRoundbar: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("RoundbarList");
        }

        [AllowAnonymous]
        public ActionResult RoundbarList(FormCollection form, int? page)
        {

            EmmetModelsList objList = new EmmetModelsList();
            objList.EmmetList = new List<EmmetModels>();

            try
            {
                string sSearchtext = "";
                string sSqlstmt = "select id_roundbar,added_date,po,supplier from t_roundbar where active=1";

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_roundbar asc";

                DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                {
                   
                    for (int i = 0; i < dsEmmetList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmmetModels objModel = new EmmetModels
                            {

                                id_roundbar = dsEmmetList.Tables[0].Rows[i]["id_roundbar"].ToString(),
                                //material =objGlobaldata.getRoundbarMaterialById(dsEmmetList.Tables[0].Rows[i]["material"].ToString()),
                                //diameter =objGlobaldata.getRoundbarDiamterById(dsEmmetList.Tables[0].Rows[i]["diameter"].ToString()),
                                po = dsEmmetList.Tables[0].Rows[i]["po"].ToString(),
                                supplier =objGlobaldata.GetSupplierNameById(dsEmmetList.Tables[0].Rows[i]["supplier"].ToString()), 
                            };

                            DateTime dtDocDate;

                            if (dsEmmetList.Tables[0].Rows[i]["added_date"].ToString() != ""
                               && DateTime.TryParse(dsEmmetList.Tables[0].Rows[i]["added_date"].ToString(), out dtDocDate))
                            {
                                objModel.added_date = dtDocDate;
                            }

                            objList.EmmetList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RoundbarList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoundbarList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objList.EmmetList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult RoundbarEdit()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {

                ViewBag.CatList = objGlobaldata.getMaterialCategoryRoundbarList();
                ViewBag.Suppplier = objGlobaldata.GetSupplierList();
                if (Request.QueryString["id_roundbar"] != null && Request.QueryString["id_roundbar"] != "")
                {
                    string id_roundbar = Request.QueryString["id_roundbar"];

                    string sSqlstmt = "select id_roundbar,added_date,po,supplier,po_upload,mtc_upload from t_roundbar where id_roundbar = '" + id_roundbar + "'";

                    DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new EmmetModels
                        {
                            id_roundbar = dsEmmetList.Tables[0].Rows[0]["id_roundbar"].ToString(),
                            //material =objGlobaldata.getRoundbarMaterialById(dsEmmetList.Tables[0].Rows[0]["material"].ToString()),
                            //diameter = (dsEmmetList.Tables[0].Rows[0]["diameter"].ToString()),
                            po = dsEmmetList.Tables[0].Rows[0]["po"].ToString(),
                            supplier = (dsEmmetList.Tables[0].Rows[0]["supplier"].ToString()),
                            po_upload = (dsEmmetList.Tables[0].Rows[0]["po_upload"].ToString()),
                            mtc_upload = (dsEmmetList.Tables[0].Rows[0]["mtc_upload"].ToString()), 
                          
                        };

                        DateTime dtDocDate;

                        if (dsEmmetList.Tables[0].Rows[0]["added_date"].ToString() != ""
                           && DateTime.TryParse(dsEmmetList.Tables[0].Rows[0]["added_date"].ToString(), out dtDocDate))
                        {
                            objModel.added_date = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("RoundbarList");/*Change*/
                    }

                    EmmetModelsList objList = new EmmetModelsList();
                    objList.EmmetList = new List<EmmetModels>();

                    sSqlstmt = "select id_roundbar_trans,id_roundbar,heatno,length,qty,price,category,material from t_roundbar_trans"
                        + " where id_roundbar='" + id_roundbar + "'";

                    DataSet dsTranList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTranList.Tables.Count > 0 && dsTranList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsTranList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EmmetModels objTranModels = new EmmetModels
                                {
                                    id_roundbar_trans = dsTranList.Tables[0].Rows[i]["id_roundbar_trans"].ToString(),
                                    id_roundbar = dsTranList.Tables[0].Rows[i]["id_roundbar"].ToString(),
                                    heatno = dsTranList.Tables[0].Rows[i]["heatno"].ToString(),
                                    length =Convert.ToDecimal(dsTranList.Tables[0].Rows[i]["length"].ToString()),
                                    //diameter = dsTranList.Tables[0].Rows[i]["diameter"].ToString(),
                                    qty =Convert.ToInt32(dsTranList.Tables[0].Rows[i]["qty"].ToString()),
                                    price =Convert.ToDecimal(dsTranList.Tables[0].Rows[i]["price"].ToString()),
                                    category = dsTranList.Tables[0].Rows[i]["category"].ToString(),
                                    material = dsTranList.Tables[0].Rows[i]["material"].ToString(),
                                };

                                objList.EmmetList.Add(objTranModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in RoundbarEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("RoundbarList");
                            }
                        }
                        ViewBag.objList = objList;
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("RoundbarList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoundbarEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("RoundbarList");
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RoundbarEdit(EmmetModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> po_upload, IEnumerable<HttpPostedFileBase> mtc_upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                EmmetModelsList objList = new EmmetModelsList();
                objList.EmmetList = new List<EmmetModels>();

                string QCDelete = Request.Form["QCDocsValselectall"];
                string mtcDelete = Request.Form["mtcDocsValselectall"];

                DateTime dateValue;
                if (DateTime.TryParse(form["added_date"], out dateValue) == true)
                {
                    objModel.added_date = dateValue;
                }
                if (po_upload != null && files.ContentLength > 0)
                {
                    objModel.po_upload = "";
                    foreach (var file in po_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Emmet"), Path.GetFileName(file.FileName));
                            string sFilename = "PO" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.po_upload = objModel.po_upload + "," + "~/DataUpload/MgmtDocs/Emmet/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddRoundbar-upload: " + ex.ToString());
                        }
                    }
                    objModel.po_upload = objModel.po_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.po_upload = objModel.po_upload + "," + form["QCDocsVal"];
                    objModel.po_upload = objModel.po_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objModel.po_upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objModel.po_upload = null;
                }
                if (mtc_upload != null && files.ContentLength > 0)
                {
                    objModel.mtc_upload = "";
                    foreach (var file in mtc_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Emmet"), Path.GetFileName(file.FileName));
                            string sFilename = "MTC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.mtc_upload = objModel.mtc_upload + "," + "~/DataUpload/MgmtDocs/Emmet/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddRoundbar-upload: " + ex.ToString());
                        }
                    }
                    objModel.mtc_upload = objModel.mtc_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["mtcDocsVal"] != null && form["mtcDocsVal"] != "")
                {
                    objModel.mtc_upload = objModel.mtc_upload + "," + form["mtcDocsVal"];
                    objModel.mtc_upload = objModel.mtc_upload.Trim(',');
                }
                else if (form["mtcDocsVal"] == null && mtcDelete != null && files.ContentLength == 0)
                {
                    objModel.mtc_upload = null;
                }
                else if (form["mtcDocsVal"] == null && files.ContentLength == 0)
                {
                    objModel.mtc_upload = null;
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["heatno" + i] != null && form["length" + i] != null && form["price" + i] != null)
                    {
                        EmmetModels objEmmet = new EmmetModels();

                        objEmmet.heatno = form["heatno" + i];
                        objEmmet.length = Convert.ToDecimal(form["length" + i]);                  
                        objEmmet.qty = Convert.ToInt32(form["qty" + i]);
                        objEmmet.price = Convert.ToDecimal(form["price" + i]);
                        objEmmet.category = form["category" + i];
                        objEmmet.material = form["material" + i];
                        objList.EmmetList.Add(objEmmet);
                    }
                }

                if (objModel.heatno != null  && objModel.category != null && objModel.material != null)
                {
                    EmmetModels objEmmet = new EmmetModels();

                    objEmmet.heatno = form["heatno"];
                    objEmmet.length = Convert.ToDecimal(form["length"]);            
                    objEmmet.qty = Convert.ToInt32(form["qty"]);
                    objEmmet.price = Convert.ToDecimal(form["price"]);
                    objEmmet.category = form["category"];
                    objEmmet.material = form["material"];
                    objList.EmmetList.Add(objEmmet);
                }

                if (objModel.FunUpdateRoundBar(objModel, objList))
                {
                    TempData["Successdata"] = "Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoundbarEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("RoundbarEdit", new { id_roundbar = objModel.id_roundbar });
        }

        [AllowAnonymous]
        public ActionResult RoundbarDetail()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {

               
                if (Request.QueryString["id_roundbar"] != null && Request.QueryString["id_roundbar"] != "")
                {
                    string id_roundbar = Request.QueryString["id_roundbar"];

                    string sSqlstmt = "select id_roundbar,added_date,po,supplier,po_upload,mtc_upload from t_roundbar where id_roundbar = '" + id_roundbar + "'";

                    DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new EmmetModels
                        {
                            id_roundbar = dsEmmetList.Tables[0].Rows[0]["id_roundbar"].ToString(),
                            //material =objGlobaldata.getRoundbarMaterialById(dsEmmetList.Tables[0].Rows[0]["material"].ToString()),
                         
                            po = dsEmmetList.Tables[0].Rows[0]["po"].ToString(),
                            supplier =objGlobaldata.GetSupplierNameById(dsEmmetList.Tables[0].Rows[0]["supplier"].ToString()),
                            po_upload = (dsEmmetList.Tables[0].Rows[0]["po_upload"].ToString()),
                            mtc_upload = (dsEmmetList.Tables[0].Rows[0]["mtc_upload"].ToString()),

                        };

                        DateTime dtDocDate;

                        if (dsEmmetList.Tables[0].Rows[0]["added_date"].ToString() != ""
                           && DateTime.TryParse(dsEmmetList.Tables[0].Rows[0]["added_date"].ToString(), out dtDocDate))
                        {
                            objModel.added_date = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("RoundbarList");/*Change*/
                    }

                    EmmetModelsList objList = new EmmetModelsList();
                    objList.EmmetList = new List<EmmetModels>();

                    sSqlstmt = "select id_roundbar_trans,id_roundbar,heatno,length,qty,price,category,material,tprice from t_roundbar_trans"
                        + " where id_roundbar='" + id_roundbar + "'";

                    DataSet dsTranList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTranList.Tables.Count > 0 && dsTranList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsTranList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EmmetModels objTranModels = new EmmetModels
                                {
                                    id_roundbar_trans = dsTranList.Tables[0].Rows[i]["id_roundbar_trans"].ToString(),
                                    id_roundbar = dsTranList.Tables[0].Rows[i]["id_roundbar"].ToString(),
                                    heatno = dsTranList.Tables[0].Rows[i]["heatno"].ToString(),
                                    length = Convert.ToDecimal(dsTranList.Tables[0].Rows[i]["length"].ToString()),
                                    qty = Convert.ToInt32(dsTranList.Tables[0].Rows[i]["qty"].ToString()),
                                    price = Convert.ToDecimal(dsTranList.Tables[0].Rows[i]["price"].ToString()),
                                    category = dsTranList.Tables[0].Rows[i]["category"].ToString(),
                                    material = dsTranList.Tables[0].Rows[i]["material"].ToString(),
                                   
                                  
                                };
                                if (dsTranList.Tables[0].Rows[i]["tprice"].ToString() != "")
                                {
                                    objTranModels.tprice = Convert.ToDecimal(dsTranList.Tables[0].Rows[i]["tprice"].ToString());
                                }
                                objList.EmmetList.Add(objTranModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in RoundbarEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("RoundbarList");
                            }
                        }
                        ViewBag.objList = objList;
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("RoundbarList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoundbarDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("RoundbarList");
            }
            return View(objModel);
        }

        [AllowAnonymous]
        public JsonResult RoundbarDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_roundbar"] != null && form["id_roundbar"] != "")
                {
                    EmmetModels Doc = new EmmetModels();
                    string id_roundbar = form["id_roundbar"];
                    if (Doc.FunDeleteRoundbarDoc(id_roundbar))
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoundbarDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult RoundbarIssueDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_roundbar_issue"] != null && form["id_roundbar_issue"] != "")
                {
                    EmmetModels Doc = new EmmetModels();
                    string id_roundbar_issue = form["id_roundbar_issue"];
                    if (Doc.FunDeleteRoundbarIssueDoc(id_roundbar_issue))
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoundbarDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult RoundbarStockUpdate()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {

                ViewBag.category = objGlobaldata.getMaterialCategoryRoundbarList();
                string sql = "update t_stock_master set dummy_bal='0' where dummy_bal != '0'";
                objGlobaldata.ExecuteQuery(sql);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoundbarStockUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RoundbarStockUpdate(FormCollection form, string material)
        {
            EmmetModels objModel = new EmmetModels();
            try
            {
               
                ViewBag.MaterialList = objGlobaldata.getRoundbarMaterialList();
                ViewBag.LengthList = objGlobaldata.getRoundbarLengthList(material);
                if (material != null && material != "")
                {
                    ViewBag.MaterialListval = material;
                }
             
                objModel.material = material;
            
                string sqlStmt = "select id_stock,material,length,qty,bal_qty from t_stock_master where material='" + material + "'";
                ViewBag.dsStock = objGlobaldata.Getdetails(sqlStmt);
                string sql = "update t_stock_master set dummy_bal='0' where dummy_bal != '0'";
                objGlobaldata.ExecuteQuery(sql);

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoundbarStockUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRoundbarIssue(EmmetModels objModel, FormCollection form)
        {
            try
            {
                EmmetModelsList objList = new EmmetModelsList();
                objList.EmmetList = new List<EmmetModels>();

                DateTime dateValue;
                if (DateTime.TryParse(form["issue_date"], out dateValue) == true)
                {
                    objModel.issue_date = dateValue;
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["qty" + i] != null && form["length" + i] != null && form["issue_qty" + i] != null && form["issue_length" + i] != null)
                    {
                        EmmetModels objEmmet = new EmmetModels();
                        objEmmet.category = (form["category" + i]);
                        objEmmet.material = (form["material" + i]);
                        objEmmet.heatno = (form["heatno" + i]);
                        objEmmet.id_stock = (form["id_stock" + i]);
                        objEmmet.qty = Convert.ToInt32(form["qty" + i]);
                        objEmmet.length = Convert.ToDecimal(form["length" + i]);
                        objEmmet.issue_qty = Convert.ToInt32(form["issue_qty" + i]);
                        objEmmet.issue_length = Convert.ToDecimal(form["issue_length" + i]);
                        objList.EmmetList.Add(objEmmet);
                    }
                }
                if (objModel.FunAddStockIssue(objModel, objList))
                {
                    TempData["Successdata"] = "Stock updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRoundbarIssue: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RoundbarIssueList");
        }

        [AllowAnonymous]
        public ActionResult RoundbarIssueList(FormCollection form, int? page)
        {

            EmmetModelsList objList = new EmmetModelsList();
            objList.EmmetList = new List<EmmetModels>();

            try
            {
                string sSearchtext = "";
                string sSqlstmt = "select id_roundbar_issue,issue_date,jobno,po from t_roundbar_issue where active=1";

                sSqlstmt = sSqlstmt + sSearchtext + " order by issue_date asc";

                DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                {
                   
                    for (int i = 0; i < dsEmmetList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmmetModels objModel = new EmmetModels
                            {

                                id_roundbar_issue = dsEmmetList.Tables[0].Rows[i]["id_roundbar_issue"].ToString(),
                            
                                //material = objGlobaldata.getMaterialById(dsEmmetList.Tables[0].Rows[i]["material"].ToString()),
                                //diameter = objGlobaldata.getRoundbarDiamterById(dsEmmetList.Tables[0].Rows[i]["diameter"].ToString()),
                           
                                jobno = (dsEmmetList.Tables[0].Rows[i]["jobno"].ToString()),
                                po = (dsEmmetList.Tables[0].Rows[i]["po"].ToString()),
                            };

                            DateTime dtDocDate;

                            if (dsEmmetList.Tables[0].Rows[i]["issue_date"].ToString() != ""
                               && DateTime.TryParse(dsEmmetList.Tables[0].Rows[i]["issue_date"].ToString(), out dtDocDate))
                            {
                                objModel.issue_date = dtDocDate;
                            }

                            objList.EmmetList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RoundbarIssueList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoundbarIssueList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objList.EmmetList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult RoundbarMasterList(FormCollection form, int? page, string material, string category, string chkAll)
        {

            EmmetModelsList objList = new EmmetModelsList();
            objList.EmmetList = new List<EmmetModels>();

            try
            {
              
                ViewBag.CategoryList = objGlobaldata.GetDropdownList("Material Category");
                string sSearchtext = "";
                string sSqlstmt = "select id_stock,category,material,bal_qty,length,qty,expiry_date from t_stock_master";

              
                if (chkAll == null && chkAll != "All")
                {

                    if (material != "" && material != null)
                    {
                        ViewBag.MaterialList = objGlobaldata.getMaterialList(category);
                        ViewBag.MaterialListval = material;
                        sSearchtext = sSearchtext + " where material ='" + material + "'";
                    }
                    if (category != "" && category != null)
                    {
                        ViewBag.MaterialList = objGlobaldata.getMaterialList(category);
                        ViewBag.CategoryListval = category;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and category ='" + category + "'";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " where category ='" + category + "'";
                        }
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_stock asc";

                DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                {
                  
                    for (int i = 0; i < dsEmmetList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmmetModels objModel = new EmmetModels
                            {

                              
                                id_stock = dsEmmetList.Tables[0].Rows[i]["id_stock"].ToString(),
                                material = objGlobaldata.getMaterialById(dsEmmetList.Tables[0].Rows[i]["material"].ToString()),
                                category = objGlobaldata.GetDropdownitemById(dsEmmetList.Tables[0].Rows[i]["category"].ToString()),
                                //diameter = objGlobaldata.getRoundbarDiamterById(dsEmmetList.Tables[0].Rows[i]["diameter"].ToString()),                   
                            };
                            if (dsEmmetList.Tables[0].Rows[i]["length"].ToString() != null && dsEmmetList.Tables[0].Rows[i]["length"].ToString() != "")
                            {
                                objModel.length = Convert.ToDecimal(dsEmmetList.Tables[0].Rows[i]["length"].ToString());
                            }
                            if (dsEmmetList.Tables[0].Rows[i]["qty"].ToString() != null && dsEmmetList.Tables[0].Rows[i]["qty"].ToString() != "")
                            {
                                objModel.qty = Convert.ToInt32(dsEmmetList.Tables[0].Rows[i]["qty"].ToString());
                            }
                            if (dsEmmetList.Tables[0].Rows[i]["bal_qty"].ToString() != null && dsEmmetList.Tables[0].Rows[i]["bal_qty"].ToString() != "")
                            {
                                objModel.bal_qty = Convert.ToDecimal(dsEmmetList.Tables[0].Rows[i]["bal_qty"].ToString());  
                            }

                            DateTime dtDocDate;

                            if (dsEmmetList.Tables[0].Rows[i]["expiry_date"].ToString() != ""
                               && DateTime.TryParse(dsEmmetList.Tables[0].Rows[i]["expiry_date"].ToString(), out dtDocDate))
                            {
                                objModel.expiry_date = dtDocDate;
                            }

                            objList.EmmetList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RoundbarMasterList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoundbarMasterList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objList.EmmetList.ToList().ToPagedList(page ?? 1, 1000));
        }

        public JsonResult FunGetStockQtyByLength(string id_stock)
        {
            string sId = "";
            if (id_stock != "")
            {
                sId = objGlobaldata.GetStockQtyByLength(id_stock);

            }
            return Json(sId);
        }
        public JsonResult FunGetStockQtyByExpiryDate(string id_stock)
        {
            string sId = "";
            if (id_stock != "")
            {
                sId = objGlobaldata.GetStockQtyByExpiryDate(id_stock);

            }
            return Json(sId);
        }
        [AllowAnonymous]
        public ActionResult RoundbarIssueDetail()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {


                if (Request.QueryString["id_roundbar_issue"] != null && Request.QueryString["id_roundbar_issue"] != "")
                {
                    string id_roundbar_issue = Request.QueryString["id_roundbar_issue"];

                    string sSqlstmt = "select id_roundbar_issue,issue_date,jobno,po from t_roundbar_issue where id_roundbar_issue = '" + id_roundbar_issue + "'";

                    DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new EmmetModels
                        {
                            id_roundbar_issue = dsEmmetList.Tables[0].Rows[0]["id_roundbar_issue"].ToString(),
                            //material = objGlobaldata.getMaterialById(dsEmmetList.Tables[0].Rows[0]["material"].ToString()),
                            //diameter = objGlobaldata.getRoundbarDiamterById(dsEmmetList.Tables[0].Rows[0]["diameter"].ToString()),
                            jobno = (dsEmmetList.Tables[0].Rows[0]["jobno"].ToString()),
                            po = (dsEmmetList.Tables[0].Rows[0]["po"].ToString()),
                        };

                        DateTime dtDocDate;

                        if (dsEmmetList.Tables[0].Rows[0]["issue_date"].ToString() != ""
                           && DateTime.TryParse(dsEmmetList.Tables[0].Rows[0]["issue_date"].ToString(), out dtDocDate))
                        {
                            objModel.issue_date = dtDocDate;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("RoundbarIssueList");/*Change*/
                    }

                    EmmetModelsList objList = new EmmetModelsList();
                    objList.EmmetList = new List<EmmetModels>();

                    sSqlstmt = "select id_roundbar_issue_trans,id_roundbar_issue,id_stock,heatno,issue_qty,issue_length,category,material from t_roundbar_issue_trans"
                        + " where id_roundbar_issue='" + id_roundbar_issue + "'";

                    DataSet dsTranList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTranList.Tables.Count > 0 && dsTranList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsTranList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EmmetModels objTranModels = new EmmetModels
                                {
                                    issue_qty =Convert.ToInt32(dsTranList.Tables[0].Rows[i]["issue_qty"].ToString()),
                                    issue_length =Convert.ToDecimal(dsTranList.Tables[0].Rows[i]["issue_length"].ToString()),
                                    category = (dsTranList.Tables[0].Rows[i]["category"].ToString()),
                                    material = (dsTranList.Tables[0].Rows[i]["material"].ToString()),
                                    heatno = (dsTranList.Tables[0].Rows[i]["heatno"].ToString()),   
                                };

                                objList.EmmetList.Add(objTranModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in RoundbarIssueDetail: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("RoundbarIssueList");
                            }
                        }
                        ViewBag.objList = objList;
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("RoundbarIssueList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RoundbarIssueDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("RoundbarIssueList");
            }
            return View(objModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddMaterial()
        {
            try
            {
                ViewBag.CatList = objGlobaldata.GetDropdownList("Material Category");
                ViewBag.SubList = objGlobaldata.GetDropdownList("Material Sub-Category");
                ViewBag.TypeList = objGlobaldata.GetDropdownList("Material Type");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddMaterial: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMaterial(EmmetModels objModel, FormCollection form)
        {
            try
            {
                DataSet dsData = objGlobaldata.GenerateMaterialNo(objModel.subcategory, objModel.spec, objModel.material_id, objModel.diameter, objModel.mlength, objModel.material_type);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    objModel.material = (dsData.Tables[0].Rows[0]["material"].ToString());
                }
                if (objGlobaldata.CheckMaterialExists(objModel.material))
                {
                    if (objModel.FunAddMaterial(objModel))
                    {
                        TempData["Successdata"] = "Added successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    TempData["Successdata"] = "Material Already exists";
                    return RedirectToAction("AddMaterial");
                }
             
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddMaterial: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MaterialList");
        }

        [AllowAnonymous]
        public ActionResult MaterialList(FormCollection form, int? page,string chkAll, string category)
        {

            EmmetModelsList objList = new EmmetModelsList();
            objList.EmmetList = new List<EmmetModels>();

            try
            {
                ViewBag.CatList = objGlobaldata.GetDropdownList("Material Category");
                string sSearchtext = "";
                string sSqlstmt = "select id_material_master,category,subcategory,spec,material_id,diameter,mlength,material_type,material,min_qty from t_material_master where active=1";

                if (chkAll == null && chkAll != "All")
                {

                    if (category != "" && category != null)
                    {
                        ViewBag.CatListval = category;
                        sSearchtext = sSearchtext + " and category ='" + category + "'";
                    }
                  
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_material_master asc";

                DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                {
                 
                    for (int i = 0; i < dsEmmetList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmmetModels objModel = new EmmetModels
                            {

                                id_material_master = dsEmmetList.Tables[0].Rows[i]["id_material_master"].ToString(),
                                category = objGlobaldata.GetDropdownitemById(dsEmmetList.Tables[0].Rows[i]["category"].ToString()),
                                subcategory = objGlobaldata.GetDropdownitemById(dsEmmetList.Tables[0].Rows[i]["subcategory"].ToString()),
                                spec = (dsEmmetList.Tables[0].Rows[i]["spec"].ToString()),
                                material_id = (dsEmmetList.Tables[0].Rows[i]["material_id"].ToString()),
                                diameter = (dsEmmetList.Tables[0].Rows[i]["diameter"].ToString()),
                                mlength =(dsEmmetList.Tables[0].Rows[i]["mlength"].ToString()),
                                material_type =objGlobaldata.GetDropdownitemById(dsEmmetList.Tables[0].Rows[i]["material_type"].ToString()),
                                material = (dsEmmetList.Tables[0].Rows[i]["material"].ToString()),                               
                            };
                            if(dsEmmetList.Tables[0].Rows[i]["min_qty"].ToString() != null && dsEmmetList.Tables[0].Rows[i]["min_qty"].ToString() !="")
                            {
                                objModel.min_qty = Convert.ToInt32(dsEmmetList.Tables[0].Rows[i]["min_qty"].ToString());
                            }
                            objList.EmmetList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in MaterialList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MaterialList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objList.EmmetList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult MaterialEdit()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {

                ViewBag.CatList = objGlobaldata.GetDropdownList("Material Category");
                ViewBag.SubList = objGlobaldata.GetDropdownList("Material Sub-Category");
                ViewBag.TypeList = objGlobaldata.GetDropdownList("Material Type");

                if (Request.QueryString["id_material_master"] != null && Request.QueryString["id_material_master"] != "")
                {
                    string id_material_master = Request.QueryString["id_material_master"];

                    string sSqlstmt = "select id_material_master,category,subcategory,spec,material_id,diameter,mlength,material_type,material,min_qty from t_material_master where id_material_master='" + id_material_master + "'";

                    DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new EmmetModels
                        {
                            id_material_master = dsEmmetList.Tables[0].Rows[0]["id_material_master"].ToString(),
                            category = (dsEmmetList.Tables[0].Rows[0]["category"].ToString()),
                            subcategory = (dsEmmetList.Tables[0].Rows[0]["subcategory"].ToString()),
                            spec = (dsEmmetList.Tables[0].Rows[0]["spec"].ToString()),
                            material_id = (dsEmmetList.Tables[0].Rows[0]["material_id"].ToString()),
                            diameter = (dsEmmetList.Tables[0].Rows[0]["diameter"].ToString()),
                            mlength = (dsEmmetList.Tables[0].Rows[0]["mlength"].ToString()),
                            material_type = (dsEmmetList.Tables[0].Rows[0]["material_type"].ToString()),
                            material = (dsEmmetList.Tables[0].Rows[0]["material"].ToString()),
                           
                        };
                        if (dsEmmetList.Tables[0].Rows[0]["min_qty"].ToString() != null && dsEmmetList.Tables[0].Rows[0]["min_qty"].ToString() != "")
                        {
                            objModel.min_qty = Convert.ToInt32(dsEmmetList.Tables[0].Rows[0]["min_qty"].ToString());
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("MaterialList");/*Change*/
                    }        
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("MaterialList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MaterialEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("MaterialList");
            }
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MaterialEdit(EmmetModels objModel, FormCollection form)
        {
            try
            {
                DataSet dsData = objGlobaldata.GenerateMaterialNo(objModel.subcategory, objModel.spec, objModel.material_id, objModel.diameter, objModel.mlength, objModel.material_type);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    objModel.material = (dsData.Tables[0].Rows[0]["material"].ToString());
                }
                if (objModel.FunUpdateMaterial(objModel))
                {
                    TempData["Successdata"] = "Added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MaterialEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("MaterialList");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddStock()
        {
            try
            {
                ViewBag.CatList = objGlobaldata.getMaterialCategoryStockList();
                ViewBag.Suppplier = objGlobaldata.GetSupplierList();
               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddStock: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddStock(EmmetModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> po_upload, HttpPostedFileBase upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                EmmetModelsList objList = new EmmetModelsList();
                objList.EmmetList = new List<EmmetModels>();


                DateTime dateValue;
                if (DateTime.TryParse(form["added_date"], out dateValue) == true)
                {
                    objModel.added_date = dateValue;
                }
                if (upload != null && upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Emmet"), Path.GetFileName(upload.FileName));
                        string sFilename = "Doc" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        upload.SaveAs(sFilepath + "/" + sFilename);
                        objModel.upload = "~/DataUpload/MgmtDocs/Emmet/" + sFilename;
                    
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddStock-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (po_upload != null && files.ContentLength > 0)
                {
                    objModel.po_upload = "";
                    foreach (var file in po_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Emmet"), Path.GetFileName(file.FileName));
                            string sFilename = "PO" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.po_upload = objModel.po_upload + "," + "~/DataUpload/MgmtDocs/Emmet/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddRoundbar-upload: " + ex.ToString());
                        }
                    }
                    objModel.po_upload = objModel.po_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["category" + i] != null && form["material" + i] != null && form["qty" + i] != null && form["price" + i] != null)
                    {
                        EmmetModels objEmmet = new EmmetModels();
                        objEmmet.category = (form["category" + i]);
                        objEmmet.material = (form["material" + i]);
                        objEmmet.qty = Convert.ToInt32(form["qty" + i]);
                        objEmmet.price = Convert.ToDecimal(form["price" + i]);
                        objEmmet.tprice = Convert.ToDecimal(form["tprice" + i]);
                        objList.EmmetList.Add(objEmmet);
                    }
                }

              
                if (objModel.FunAddStock(objModel,objList))
                {
                    TempData["Successdata"] = "Added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddStock: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AddStock");
        }

        [AllowAnonymous]
        public ActionResult StockReceiveList(FormCollection form, int? page, string chkAll, string category)
        {

            EmmetModelsList objList = new EmmetModelsList();
            objList.EmmetList = new List<EmmetModels>();

            try
            {
                ViewBag.CatList = objGlobaldata.getMaterialCategoryStockList();
                string sSearchtext = "";
                string sSqlstmt = "select id_stock_receive,po,po_upload,supplier,added_date from t_stock_receive where active=1";
                if (chkAll == null && chkAll != "All")
                {

                    if (category != "" && category != null)
                    {
                        ViewBag.CatListval = category;
                        sSearchtext = sSearchtext + " and category ='" + category + "'";
                    }

                }
                else
                {
                    ViewBag.chkAll = true;
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_stock_receive asc";

                DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                {
                  
                    for (int i = 0; i < dsEmmetList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmmetModels objModel = new EmmetModels
                            {

                                id_stock_receive = dsEmmetList.Tables[0].Rows[i]["id_stock_receive"].ToString(),
                              
                                po = dsEmmetList.Tables[0].Rows[i]["po"].ToString(),
                                supplier = objGlobaldata.GetSupplierNameById(dsEmmetList.Tables[0].Rows[i]["supplier"].ToString()),
                            
                            };

                            DateTime dtDocDate;

                            if (dsEmmetList.Tables[0].Rows[i]["added_date"].ToString() != ""
                               && DateTime.TryParse(dsEmmetList.Tables[0].Rows[i]["added_date"].ToString(), out dtDocDate))
                            {
                                objModel.added_date = dtDocDate;
                            }

                            objList.EmmetList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in StockReceiveList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in StockReceiveList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objList.EmmetList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult AddStockEdit()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {

                ViewBag.CatList = objGlobaldata.getMaterialCategoryStockList();
                ViewBag.Suppplier = objGlobaldata.GetSupplierList();

                if (Request.QueryString["id_stock_receive"] != null && Request.QueryString["id_stock_receive"] != "")
                {
                    string id_stock_receive = Request.QueryString["id_stock_receive"];

                    string sSqlstmt = "select id_stock_receive,po,po_upload,supplier,added_date from t_stock_receive where id_stock_receive='" + id_stock_receive + "'";
                    DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new EmmetModels
                        {
                            id_stock_receive = dsEmmetList.Tables[0].Rows[0]["id_stock_receive"].ToString(),
                            po = dsEmmetList.Tables[0].Rows[0]["po"].ToString(),
                            supplier = (dsEmmetList.Tables[0].Rows[0]["supplier"].ToString()),
                            po_upload = (dsEmmetList.Tables[0].Rows[0]["po_upload"].ToString()),
                        };
                        DateTime dtDocDate;

                        if (dsEmmetList.Tables[0].Rows[0]["added_date"].ToString() != ""
                           && DateTime.TryParse(dsEmmetList.Tables[0].Rows[0]["added_date"].ToString(), out dtDocDate))
                        {
                            objModel.added_date = dtDocDate;
                        }

                        EmmetModelsList objList = new EmmetModelsList();
                        objList.EmmetList = new List<EmmetModels>();

                        sSqlstmt = "select id_stock_receive_trans,id_stock_receive,category,material,qty,price,expiry_date from t_stock_receive_trans"
                            + " where id_stock_receive='" + id_stock_receive + "'";

                        DataSet dsTranList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsTranList.Tables.Count > 0 && dsTranList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsTranList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EmmetModels objTranModels = new EmmetModels
                                    {
                                        id_stock_receive_trans = dsTranList.Tables[0].Rows[i]["id_stock_receive_trans"].ToString(),
                                        id_stock_receive = dsTranList.Tables[0].Rows[i]["id_stock_receive"].ToString(),
                                        category = dsTranList.Tables[0].Rows[i]["category"].ToString(),
                                        material = dsTranList.Tables[0].Rows[i]["material"].ToString(),
                                        qty = Convert.ToInt32(dsTranList.Tables[0].Rows[i]["qty"].ToString()),
                                        price = Convert.ToDecimal(dsTranList.Tables[0].Rows[i]["price"].ToString()),

                                    };
                                    if (dsTranList.Tables[0].Rows[i]["expiry_date"].ToString() != ""
                                    && DateTime.TryParse(dsTranList.Tables[0].Rows[i]["expiry_date"].ToString(), out dtDocDate))
                                    {
                                        objTranModels.expiry_date = dtDocDate;
                                    }
                                    objList.EmmetList.Add(objTranModels);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddStockEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("StockReceiveList");
                                }
                            }
                            ViewBag.objList = objList;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("StockReceiveList");/*Change*/
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("StockReceiveList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddStockEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("StockReceiveList");
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddStockEdit(EmmetModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> po_upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                EmmetModelsList objList = new EmmetModelsList();
                objList.EmmetList = new List<EmmetModels>();

                string QCDelete = Request.Form["QCDocsValselectall"];

                DateTime dateValue;
                if (DateTime.TryParse(form["added_date"], out dateValue) == true)
                {
                    objModel.added_date = dateValue;
                }
                if (po_upload != null && files.ContentLength > 0)
                {
                    objModel.po_upload = "";
                    foreach (var file in po_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Emmet"), Path.GetFileName(file.FileName));
                            string sFilename = "PO" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.po_upload = objModel.po_upload + "," + "~/DataUpload/MgmtDocs/Emmet/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddRoundbar-upload: " + ex.ToString());
                        }
                    }
                    objModel.po_upload = objModel.po_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.po_upload = objModel.po_upload + "," + form["QCDocsVal"];
                    objModel.po_upload = objModel.po_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objModel.po_upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objModel.po_upload = null;
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["category" + i] != null && form["material" + i] != null && form["qty" + i] != null && form["price" + i] != null)
                    {
                        EmmetModels objEmmet = new EmmetModels();
                        objEmmet.category = (form["category" + i]);
                        objEmmet.material = (form["material" + i]);
                        objEmmet.qty = Convert.ToInt32(form["qty" + i]);
                        objEmmet.price = Convert.ToDecimal(form["price" + i]);
                        if (DateTime.TryParse(form["expiry_date" + i], out dateValue) == true)
                        {
                            objEmmet.expiry_date = dateValue;
                        }
                        objList.EmmetList.Add(objEmmet);
                    }
                }

                if (objModel.category != null && objModel.material != null && objModel.qty != null && objModel.price != null)
                {
                    EmmetModels objEmmet = new EmmetModels();
                    objEmmet.category = form["category"];
                    objEmmet.material = form["material"];
                    objEmmet.qty = Convert.ToInt32(form["qty"]);
                    objEmmet.price = Convert.ToDecimal(form["price"]);
                    if (DateTime.TryParse(form["expiry_date"], out dateValue) == true)
                    {
                        objEmmet.expiry_date = dateValue;
                    }
                    objList.EmmetList.Add(objEmmet);
                }


                if (objModel.FunEditAddStock(objModel, objList))
                {
                    TempData["Successdata"] = "Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddStockEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("StockReceiveList");
        }

        [AllowAnonymous]
        public ActionResult StockReceiveDetail()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {

               
                if (Request.QueryString["id_stock_receive"] != null && Request.QueryString["id_stock_receive"] != "")
                {
                    string id_stock_receive = Request.QueryString["id_stock_receive"];

                    string sSqlstmt = "select id_stock_receive,po,po_upload,supplier,added_date from t_stock_receive where id_stock_receive='" + id_stock_receive + "'";
                    DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new EmmetModels
                        {
                            id_stock_receive = dsEmmetList.Tables[0].Rows[0]["id_stock_receive"].ToString(),  
                            po = dsEmmetList.Tables[0].Rows[0]["po"].ToString(),
                            supplier =objGlobaldata.GetSupplierNameById(dsEmmetList.Tables[0].Rows[0]["supplier"].ToString()),
                            po_upload = (dsEmmetList.Tables[0].Rows[0]["po_upload"].ToString()),                     
                        };
                        DateTime dtDocDate;

                        if (dsEmmetList.Tables[0].Rows[0]["added_date"].ToString() != ""
                           && DateTime.TryParse(dsEmmetList.Tables[0].Rows[0]["added_date"].ToString(), out dtDocDate))
                        {
                            objModel.added_date = dtDocDate;
                        }
                        EmmetModelsList objList = new EmmetModelsList();
                        objList.EmmetList = new List<EmmetModels>();

                        sSqlstmt = "select id_stock_receive_trans,id_stock_receive,category,material,qty,price,expiry_date from t_stock_receive_trans"
                            + " where id_stock_receive='" + id_stock_receive + "'";

                        DataSet dsTranList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsTranList.Tables.Count > 0 && dsTranList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsTranList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EmmetModels objTranModels = new EmmetModels
                                    {
                                        id_stock_receive_trans = dsTranList.Tables[0].Rows[i]["id_stock_receive_trans"].ToString(),
                                        id_stock_receive = dsTranList.Tables[0].Rows[i]["id_stock_receive"].ToString(),
                                        category = dsTranList.Tables[0].Rows[i]["category"].ToString(),
                                        material = dsTranList.Tables[0].Rows[i]["material"].ToString(),
                                        qty = Convert.ToInt32(dsTranList.Tables[0].Rows[i]["qty"].ToString()),
                                        price = Convert.ToDecimal(dsTranList.Tables[0].Rows[i]["price"].ToString()),
                                    };
                                    if (dsTranList.Tables[0].Rows[i]["expiry_date"].ToString() != ""
                                    && DateTime.TryParse(dsTranList.Tables[0].Rows[i]["expiry_date"].ToString(), out dtDocDate))
                                    {
                                        objTranModels.expiry_date = dtDocDate;
                                    }
                                    objList.EmmetList.Add(objTranModels);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddStockEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("StockReceiveList");
                                }
                            }
                            ViewBag.objList = objList;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("StockReceiveList");/*Change*/
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("StockReceiveList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in StockReceiveDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("StockReceiveList");
            }
            return View(objModel);
        }


        [AllowAnonymous]
        public JsonResult MaterialDelete(FormCollection form)
        {
            try
            {
                if (form["id_material_master"] != null && form["id_material_master"] != "")
                {
                    EmmetModels Doc = new EmmetModels();
                    string id_material_master = form["id_material_master"];
                    if (Doc.FunDeleteMaterial(id_material_master))
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MaterialDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult StockReceiveDelete(FormCollection form)
        {
            try
            {
                if (form["id_stock_receive"] != null && form["id_stock_receive"] != "")
                {
                    EmmetModels Doc = new EmmetModels();
                    string id_stock_receive = form["id_stock_receive"];
                    if (Doc.FunDeleteStockReceive(id_stock_receive))
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in StockReceiveDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public ActionResult FunGetMaterialList(string category)
        {

            MultiSelectList lstMaterial = objGlobaldata.getMaterialList(category);
            return Json(lstMaterial);
        }
        public ActionResult FunGetMaterialLengthList(string material)
        {

            MultiSelectList lstMaterial = objGlobaldata.getRoundbarLengthList(material);
            return Json(lstMaterial);
        }
        public ActionResult FunGetHeatNoList(string material)
        {

            MultiSelectList lstMaterial = objGlobaldata.getHeatNoList(material);
            return Json(lstMaterial);
        }
        public ActionResult FunGetMaterialExpiryDateList(string material)
        {

            MultiSelectList lstMaterial = objGlobaldata.getChemicalExpiryList(material);
            return Json(lstMaterial);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult IssueStock()
        {
            try
            {
                ViewBag.category = objGlobaldata.getMaterialCategoryStockList();
                ViewBag.Emplist = objGlobaldata.GetHrEmployeeListbox();

                string sql = "update t_stock_master set dummy_bal='0' where dummy_bal != '0'";
                objGlobaldata.ExecuteQuery(sql);

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueStock: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult IssueStock(EmmetModels objModel, FormCollection form)
        {
            try
            {
          
                EmmetModelsList objList = new EmmetModelsList();
                objList.EmmetList = new List<EmmetModels>();


                DateTime dateValue;
                if (DateTime.TryParse(form["issue_date"], out dateValue) == true)
                {
                    objModel.issue_date = dateValue;
                }
             
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["empid" + i] != null && form["category" + i] != null && form["material" + i] != null && form["issue_qty" + i] != null)
                    {
                        EmmetModels objEmmet = new EmmetModels();

                        objEmmet.empid = form["empid" + i];
                        objEmmet.category = (form["category" + i]);
                        objEmmet.material = (form["material" + i]);
                        objEmmet.issue_qty = Convert.ToInt32(form["issue_qty" + i]);
                        objList.EmmetList.Add(objEmmet);
                    }
                }


                if (objModel.FunIssueStock(objModel, objList))
                {
                    TempData["Successdata"] = "Issued successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueStock: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("IssueStock");
        }

        [AllowAnonymous]
        public ActionResult IssueStockList(FormCollection form, int? page)
        {

            EmmetModelsList objList = new EmmetModelsList();
            objList.EmmetList = new List<EmmetModels>();

            try
            {
                ViewBag.CatList = objGlobaldata.getMaterialCategoryStockList();
                string sSearchtext = "";
                string sSqlstmt = "select id_stock_issue,jobno,issue_date from t_stock_issue where active=1";


                sSqlstmt = sSqlstmt + sSearchtext + " order by id_stock_issue asc";

                DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                {
                    
                    for (int i = 0; i < dsEmmetList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmmetModels objModel = new EmmetModels
                            {

                                id_stock_issue = dsEmmetList.Tables[0].Rows[i]["id_stock_issue"].ToString(),
                                jobno = (dsEmmetList.Tables[0].Rows[i]["jobno"].ToString()),
                            };

                            DateTime dtDocDate;

                            if (dsEmmetList.Tables[0].Rows[i]["issue_date"].ToString() != ""
                               && DateTime.TryParse(dsEmmetList.Tables[0].Rows[i]["issue_date"].ToString(), out dtDocDate))
                            {
                                objModel.issue_date = dtDocDate;
                            }

                            objList.EmmetList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in IssueStockList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueStockList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objList.EmmetList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult ReturnList(FormCollection form, int? page, string chkAll, string empid)
        {

            EmmetModelsList objList = new EmmetModelsList();
            objList.EmmetList = new List<EmmetModels>();

            try
            {
                ViewBag.Emplist = objGlobaldata.GetHrEmployeeListbox();
                string sSearchtext = "";
                string sSqlstmt = "select t.id_stock_issue,id_stock_issue_trans,jobno,issue_date,empid,category,material,issue_qty,return_qty,balance_qty"
                +" from t_stock_issue t,t_stock_issue_trans tt where t.id_stock_issue=tt.id_stock_issue and active=1";

                if (chkAll == null && chkAll != "All")
                {

                    if (empid != "" && empid != null)
                    {
                        ViewBag.EmmetListval = empid;
                        sSearchtext = sSearchtext + " and empid ='" + empid + "'";
                    }

                }
                else
                {
                    ViewBag.chkAll = true;
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by issue_date asc";

                DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                {
                    
                    for (int i = 0; i < dsEmmetList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmmetModels objModel = new EmmetModels
                            {
                                
                                id_stock_issue = dsEmmetList.Tables[0].Rows[i]["id_stock_issue"].ToString(),
                                id_stock_issue_trans = dsEmmetList.Tables[0].Rows[i]["id_stock_issue_trans"].ToString(),
                                jobno = (dsEmmetList.Tables[0].Rows[i]["jobno"].ToString()),
                                empid =objGlobaldata.GetEmpHrNameById(dsEmmetList.Tables[0].Rows[i]["empid"].ToString()),
                                category = objGlobaldata.GetDropdownitemById(dsEmmetList.Tables[0].Rows[i]["category"].ToString()),
                                material =objGlobaldata.getMaterialById(dsEmmetList.Tables[0].Rows[i]["material"].ToString()),
                                issue_qty =Convert.ToInt32(dsEmmetList.Tables[0].Rows[i]["issue_qty"].ToString()),
                                return_qty =Convert.ToInt32(dsEmmetList.Tables[0].Rows[i]["return_qty"].ToString()),
                                balance_qty =Convert.ToInt32(dsEmmetList.Tables[0].Rows[i]["balance_qty"].ToString()),
                            };

                            DateTime dtDocDate;

                            if (dsEmmetList.Tables[0].Rows[i]["issue_date"].ToString() != ""
                               && DateTime.TryParse(dsEmmetList.Tables[0].Rows[i]["issue_date"].ToString(), out dtDocDate))
                            {
                                objModel.issue_date = dtDocDate;
                            }

                            objList.EmmetList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ReturnList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReturnList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objList.EmmetList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult ReturnChemicalList(FormCollection form, int? page, string chkAll)
        {

            EmmetModelsList objList = new EmmetModelsList();
            objList.EmmetList = new List<EmmetModels>();

            try
            {
              
                string sSearchtext = "";
                string sSqlstmt = "select t.id_chemical_issue,id_chemical_issue_trans,jobno,po,empid,issue_date,category,material,issue_qty,return_qty,balance_qty"
                + " from t_chemical_issue t,t_chemical_issue_trans tt where t.id_chemical_issue=tt.id_chemical_issue and active=1";

                //if (chkAll == null && chkAll != "All")
                //{

                //    if (empid != "" && empid != null)
                //    {
                //        ViewBag.EmmetListval = empid;
                //        sSearchtext = sSearchtext + " and empid ='" + empid + "'";
                //    }

                //}
                //else
                //{
                //    ViewBag.chkAll = true;
                //}

                sSqlstmt = sSqlstmt + sSearchtext + " order by issue_date asc";

                DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                {
                  
                    for (int i = 0; i < dsEmmetList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmmetModels objModel = new EmmetModels
                            {

                                id_chemical_issue = dsEmmetList.Tables[0].Rows[i]["id_chemical_issue"].ToString(),
                                id_chemical_issue_trans = dsEmmetList.Tables[0].Rows[i]["id_chemical_issue_trans"].ToString(),
                                jobno = (dsEmmetList.Tables[0].Rows[i]["jobno"].ToString()),
                                po = (dsEmmetList.Tables[0].Rows[i]["po"].ToString()),
                                empid = objGlobaldata.GetEmpHrNameById(dsEmmetList.Tables[0].Rows[i]["empid"].ToString()),
                                category = objGlobaldata.GetDropdownitemById(dsEmmetList.Tables[0].Rows[i]["category"].ToString()),
                                material = objGlobaldata.getMaterialById(dsEmmetList.Tables[0].Rows[i]["material"].ToString()),
                                issue_qty = Convert.ToInt32(dsEmmetList.Tables[0].Rows[i]["issue_qty"].ToString()),
                                return_qty = Convert.ToInt32(dsEmmetList.Tables[0].Rows[i]["return_qty"].ToString()),
                                balance_qty = Convert.ToInt32(dsEmmetList.Tables[0].Rows[i]["balance_qty"].ToString()),
                            };

                            DateTime dtDocDate;

                            if (dsEmmetList.Tables[0].Rows[i]["issue_date"].ToString() != ""
                               && DateTime.TryParse(dsEmmetList.Tables[0].Rows[i]["issue_date"].ToString(), out dtDocDate))
                            {
                                objModel.issue_date = dtDocDate;
                            }

                            objList.EmmetList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ReturnChemicalList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReturnChemicalList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objList.EmmetList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult LogList(FormCollection form, int? page, string material, string category,string op_type, string chkAll)
        {

            EmmetModelsList objList = new EmmetModelsList();
            objList.EmmetList = new List<EmmetModels>();

            try
            {
                ViewBag.CategoryList = objGlobaldata.GetDropdownList("Material Category");
                ViewBag.Type = objGlobaldata.GetConstantValue("OperationType");
                string sSearchtext = "";
                string sSqlstmt = "select id_stock_log,tran_date,category,material,qty,op_type,jobno,po,empid,po_upload,mtc_upload from t_stock_log";

                if (chkAll == null && chkAll != "All")
                {
                    if (op_type != "" && op_type != null)
                    {

                        ViewBag.Typeval = op_type;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and op_type ='" + op_type + "'";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " where op_type ='" + op_type + "'";
                        }
                    }

                    if (material != "" && material != null)
                    {
                        ViewBag.MaterialList = objGlobaldata.getMaterialList(category);
                        ViewBag.MaterialListval = material;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and material ='" + material + "'";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " where material ='" + material + "'";
                        }
                    }
                    if (category != "" && category != null)
                    {
                        ViewBag.CategoryListval = category;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and category ='" + category + "'";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " where category ='" + category + "'";
                        }
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by op_type asc";

                DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                {
                   
                    for (int i = 0; i < dsEmmetList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmmetModels objModel = new EmmetModels
                            {

                                id_stock_log = dsEmmetList.Tables[0].Rows[i]["id_stock_log"].ToString(),
                                category = objGlobaldata.GetDropdownitemById(dsEmmetList.Tables[0].Rows[i]["category"].ToString()),
                                material = objGlobaldata.getMaterialById(dsEmmetList.Tables[0].Rows[i]["material"].ToString()),
                                qty = Convert.ToInt32(dsEmmetList.Tables[0].Rows[i]["qty"].ToString()),
                                op_type = (dsEmmetList.Tables[0].Rows[i]["op_type"].ToString()),
                                jobno = (dsEmmetList.Tables[0].Rows[i]["jobno"].ToString()),
                                po = (dsEmmetList.Tables[0].Rows[i]["po"].ToString()),
                                empid =objGlobaldata.GetEmpHrNameById(dsEmmetList.Tables[0].Rows[i]["empid"].ToString()),
                                po_upload = (dsEmmetList.Tables[0].Rows[i]["po_upload"].ToString()),
                                mtc_upload = (dsEmmetList.Tables[0].Rows[i]["mtc_upload"].ToString()),
                            };

                            DateTime dtDocDate;

                            if (dsEmmetList.Tables[0].Rows[i]["tran_date"].ToString() != ""
                               && DateTime.TryParse(dsEmmetList.Tables[0].Rows[i]["tran_date"].ToString(), out dtDocDate))
                            {
                                objModel.tran_date = dtDocDate;
                            }

                            objList.EmmetList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in LogList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LogList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objList.EmmetList.ToList().ToPagedList(page ?? 1, 1000));
        }

        public JsonResult FunGetStockQty(string category, string material)
        {
            string Qty = "";
            if (category != "" && material != "")
            {
                Qty = objGlobaldata.GetStockQty(category, material);
            }
            return Json(Qty);
        }
        public JsonResult FunSetStockQty(string category, string material, int issue_qty)
        {
            bool Qty = true;
            if (category != "" && material != "")
            {
                Qty = objGlobaldata.SetStockQty(category, material, issue_qty);
            }
            return Json(Qty);
        }
        public JsonResult FunSetRoundbarQty(string material, string length, int issue_qty)
        {
            bool Qty = true;
            if (material != "")
            {
                Qty = objGlobaldata.SetRoundbarQty(material,length, issue_qty);
            }
            return Json(Qty);
        }
        public JsonResult FunSetChemicalQty(string material, string expiry_date, int issue_qty)
        {
            bool Qty = true;
            string sexpdate = "";
            DateTime dateValue=new DateTime();

            if (expiry_date != null && DateTime.TryParse(expiry_date, out dateValue) == true)
            {
              sexpdate=dateValue.ToString("yyyy-MM-dd");
            }
            if (material != "")
            {
                Qty = objGlobaldata.SetChemicalQty(material, sexpdate, issue_qty);
            }
            return Json(Qty);
        }
        public JsonResult FunUpdateStockQty(string category, string material, int issue_qty)
        {
            bool Qty = true;
            if (category != "" && material != "")
            {
                Qty = objGlobaldata.UpdateStockQty(category, material, issue_qty);
            }
            return Json(Qty);
        }
        public JsonResult FunUpdateRoundbarQty(string material,string length, int issue_qty)
        {
            bool Qty = true;
            if (material != "")
            {
                Qty = objGlobaldata.UpdateRoundbarQty(material,length, issue_qty);
            }
            return Json(Qty);
        }
        public JsonResult FunUpdateChemicalQty(string material, string expiry_date, int issue_qty)
        {
            bool Qty = true;
            string sexpiry_date = "";
            if (expiry_date != "")
            {
                sexpiry_date = objGlobaldata.GetExpDateById(expiry_date);
            }
            if (material != "")
            {
                Qty = objGlobaldata.UpdateChemicalQty(material, sexpiry_date, issue_qty);
            }
            return Json(Qty);
        }
        [AllowAnonymous]
        public ActionResult IssueStockEdit()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {

                ViewBag.category = objGlobaldata.getMaterialCategoryStockList();
                ViewBag.Emplist = objGlobaldata.GetHrEmployeeListbox();
                if (Request.QueryString["id_stock_issue"] != null && Request.QueryString["id_stock_issue"] != "")
                {
                    string id_stock_issue = Request.QueryString["id_stock_issue"];

                    string sSqlstmt = "select id_stock_issue,jobno,issue_date from t_stock_issue where id_stock_issue = '" + id_stock_issue + "'";

                    DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new EmmetModels
                        {
                            id_stock_issue = dsEmmetList.Tables[0].Rows[0]["id_stock_issue"].ToString(),
                            jobno = (dsEmmetList.Tables[0].Rows[0]["jobno"].ToString()),

                        };

                        DateTime dtDocDate;

                        if (dsEmmetList.Tables[0].Rows[0]["issue_date"].ToString() != ""
                              && DateTime.TryParse(dsEmmetList.Tables[0].Rows[0]["issue_date"].ToString(), out dtDocDate))
                        {
                            objModel.issue_date = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("IssueStockList");/*Change*/
                    }

                    EmmetModelsList objList = new EmmetModelsList();
                    objList.EmmetList = new List<EmmetModels>();

                    sSqlstmt = "select id_stock_issue_trans,id_stock_issue,empid,category,material,issue_qty from t_stock_issue_trans"
                        + " where id_stock_issue='" + id_stock_issue + "'";

                    DataSet dsTranList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTranList.Tables.Count > 0 && dsTranList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsTranList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EmmetModels objTranModels = new EmmetModels
                                {
                                    id_stock_issue_trans = dsTranList.Tables[0].Rows[i]["id_stock_issue_trans"].ToString(),
                                    id_stock_issue = dsTranList.Tables[0].Rows[i]["id_stock_issue"].ToString(),
                                    empid = dsTranList.Tables[0].Rows[i]["empid"].ToString(),
                                    category = dsTranList.Tables[0].Rows[i]["category"].ToString(),
                                    material = dsTranList.Tables[0].Rows[i]["material"].ToString(),
                                    issue_qty = Convert.ToInt32(dsTranList.Tables[0].Rows[i]["issue_qty"].ToString()),
                                 
                                };

                                objList.EmmetList.Add(objTranModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in IssueStockEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("IssueStockList");
                            }
                        }
                        ViewBag.objList = objList;
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("IssueStockList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueStockEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("IssueStockList");
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult IssueStockEdit(EmmetModels objModel, FormCollection form)
        {
            try
            {
             
                EmmetModelsList objList = new EmmetModelsList();
                objList.EmmetList = new List<EmmetModels>();

             
                DateTime dateValue;
                if (DateTime.TryParse(form["issue_date"], out dateValue) == true)
                {
                    objModel.issue_date = dateValue;
                }


                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["empid" + i] != null && form["category" + i] != null && form["material" + i] != null && form["issue_qty" + i] != null)
                    {
                        EmmetModels objEmmet = new EmmetModels();

                        objEmmet.empid = form["empid" + i];
                        objEmmet.category = (form["category" + i]);
                        objEmmet.material = (form["material" + i]);
                        objEmmet.issue_qty = Convert.ToInt32(form["issue_qty" + i]);
                        objList.EmmetList.Add(objEmmet);
                    }
                }


                if (objModel.empid != null && objModel.category != null && objModel.material != null && objModel.issue_qty != null)
                {
                    EmmetModels objEmmet = new EmmetModels();

                    objEmmet.empid = form["empid"];
                    objEmmet.category = (form["category"]);
                    objEmmet.material = (form["material"]);
                    objEmmet.issue_qty = Convert.ToInt32(form["issue_qty"]);
                    objList.EmmetList.Add(objEmmet);
                }

                if (objModel.FunUpdateIssueStock(objModel, objList))
                {
                    TempData["Successdata"] = "Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueStockEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("IssueStockEdit", new { id_stock_issue = objModel.id_stock_issue });
        }

        [AllowAnonymous]
        public ActionResult IssueStockDetail()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {

               
                if (Request.QueryString["id_stock_issue"] != null && Request.QueryString["id_stock_issue"] != "")
                {
                    string id_stock_issue = Request.QueryString["id_stock_issue"];

                    string sSqlstmt = "select id_stock_issue,jobno,issue_date from t_stock_issue where id_stock_issue = '" + id_stock_issue + "'";

                    DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new EmmetModels
                        {
                            id_stock_issue = dsEmmetList.Tables[0].Rows[0]["id_stock_issue"].ToString(),
                            jobno = (dsEmmetList.Tables[0].Rows[0]["jobno"].ToString()),

                        };

                        DateTime dtDocDate;

                        if (dsEmmetList.Tables[0].Rows[0]["issue_date"].ToString() != ""
                              && DateTime.TryParse(dsEmmetList.Tables[0].Rows[0]["issue_date"].ToString(), out dtDocDate))
                        {
                            objModel.issue_date = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("IssueStockList");/*Change*/
                    }

                    EmmetModelsList objList = new EmmetModelsList();
                    objList.EmmetList = new List<EmmetModels>();

                    sSqlstmt = "select id_stock_issue_trans,id_stock_issue,empid,category,material,issue_qty,return_qty,balance_qty from t_stock_issue_trans"
                        + " where id_stock_issue='" + id_stock_issue + "'";

                    DataSet dsTranList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTranList.Tables.Count > 0 && dsTranList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsTranList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EmmetModels objTranModels = new EmmetModels
                                {
                                    id_stock_issue_trans = dsTranList.Tables[0].Rows[i]["id_stock_issue_trans"].ToString(),
                                    id_stock_issue = dsTranList.Tables[0].Rows[i]["id_stock_issue"].ToString(),
                                    empid = dsTranList.Tables[0].Rows[i]["empid"].ToString(),
                                    category = dsTranList.Tables[0].Rows[i]["category"].ToString(),
                                    material = dsTranList.Tables[0].Rows[i]["material"].ToString(),
                                    issue_qty = Convert.ToInt32(dsTranList.Tables[0].Rows[i]["issue_qty"].ToString()),
                                    return_qty = Convert.ToInt32(dsTranList.Tables[0].Rows[i]["return_qty"].ToString()),
                                    balance_qty = Convert.ToInt32(dsTranList.Tables[0].Rows[i]["balance_qty"].ToString()),
                                };
                                objList.EmmetList.Add(objTranModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in IssueStockDetail: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("IssueStockList");
                            }
                        }
                        ViewBag.objList = objList;
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("IssueStockList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueStockDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("IssueStockList");
            }
            return View(objModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ReturnStock()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {
                ViewBag.Status = objGlobaldata.GetDropdownList("Material Status");
                if (Request.QueryString["id_stock_issue_trans"] != null && Request.QueryString["id_stock_issue_trans"] != "")
                {
                    objModel.id_stock_issue_trans = Request.QueryString["id_stock_issue_trans"];
                    objModel.id_stock_issue = Request.QueryString["id_stock_issue"];

                    string sql = "select balance_qty from t_stock_issue_trans where id_stock_issue_trans='" + objModel.id_stock_issue_trans + "'";
                    DataSet dsData = objGlobaldata.Getdetails(sql);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        objModel.balance_qty =Convert.ToInt32(dsData.Tables[0].Rows[0]["balance_qty"].ToString());
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("IssueStockList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReturnStock: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ReturnStock(EmmetModels objModel, FormCollection form)
        {
            try
            {       
                DateTime dateValue;
                if (DateTime.TryParse(form["return_date"], out dateValue) == true)
                {
                    objModel.return_date = dateValue;
                }

                if (objModel.FunReturnStock(objModel))
                {
                    TempData["Successdata"] = "Returned successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReturnStock: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ReturnList");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ReturnChemical()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {
                ViewBag.Status = objGlobaldata.GetDropdownList("Material Status");
                if (Request.QueryString["id_chemical_issue_trans"] != null && Request.QueryString["id_chemical_issue_trans"] != "")
                {
                    objModel.id_chemical_issue_trans = Request.QueryString["id_chemical_issue_trans"];
                    objModel.id_chemical_issue = Request.QueryString["id_chemical_issue"];

                    string sql = "select balance_qty from t_chemical_issue_trans where id_chemical_issue_trans='" + objModel.id_chemical_issue_trans + "'";
                    DataSet dsData = objGlobaldata.Getdetails(sql);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        objModel.balance_qty = Convert.ToInt32(dsData.Tables[0].Rows[0]["balance_qty"].ToString());
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("IssueStockList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReturnChemical: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ReturnChemical(EmmetModels objModel, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                if (DateTime.TryParse(form["return_date"], out dateValue) == true)
                {
                    objModel.return_date = dateValue;
                }

                if (objModel.FunReturnChemical(objModel))
                {
                    TempData["Successdata"] = "Returned successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReturnChemical: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ReturnChemicalList");
        }

       

        [AllowAnonymous]
        public ActionResult ReturnStockList(FormCollection form, int? page)
        {

            EmmetModelsList objList = new EmmetModelsList();
            objList.EmmetList = new List<EmmetModels>();

            try
            {
                if (Request.QueryString["id_stock_issue_trans"] != null && Request.QueryString["id_stock_issue_trans"] != "")
                {
                    string id_stock_issue_trans = Request.QueryString["id_stock_issue_trans"];
                    string id_stock_issue = Request.QueryString["id_stock_issue"];
                    ViewBag.id_stock_issue = id_stock_issue;
                    string sSearchtext = "";
                    string sSqlstmt = "select id_stock_return,id_stock_issue_trans,return_date,return_qty,return_status from t_stock_return where active=1 and id_stock_issue_trans='" + id_stock_issue_trans + "'";


                    sSqlstmt = sSqlstmt + sSearchtext + " order by id_stock_return asc";

                    DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                    {
                      
                        for (int i = 0; i < dsEmmetList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EmmetModels objModel = new EmmetModels
                                {

                                    id_stock_return = dsEmmetList.Tables[0].Rows[i]["id_stock_return"].ToString(),
                                    id_stock_issue_trans = (dsEmmetList.Tables[0].Rows[i]["id_stock_issue_trans"].ToString()),
                                    return_qty =Convert.ToInt32(dsEmmetList.Tables[0].Rows[i]["return_qty"].ToString()),
                                    return_status =objGlobaldata.GetDropdownitemById(dsEmmetList.Tables[0].Rows[i]["return_status"].ToString()),
                                };

                                DateTime dtDocDate;

                                if (dsEmmetList.Tables[0].Rows[i]["return_date"].ToString() != ""
                                   && DateTime.TryParse(dsEmmetList.Tables[0].Rows[i]["return_date"].ToString(), out dtDocDate))
                                {
                                    objModel.return_date = dtDocDate;
                                }

                                objList.EmmetList.Add(objModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in ReturnStockList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Data Not Exists";
                        return RedirectToAction("IssueStockDetail", new { id_stock_issue = id_stock_issue });
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("IssueStockList");/*Change*/
                }
     
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReturnStockList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objList.EmmetList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult ReturnChemicalTransList(FormCollection form, int? page)
        {

            EmmetModelsList objList = new EmmetModelsList();
            objList.EmmetList = new List<EmmetModels>();

            try
            {
                if (Request.QueryString["id_chemical_issue_trans"] != null && Request.QueryString["id_chemical_issue_trans"] != "")
                {
                    string id_chemical_issue_trans = Request.QueryString["id_chemical_issue_trans"];
                    string id_chemical_issue = Request.QueryString["id_chemical_issue"];
                    ViewBag.id_chemical_issue = id_chemical_issue;
                    string sSearchtext = "";
                    string sSqlstmt = "select id_chemical_return,id_chemical_issue_trans,return_date,return_qty,return_status from t_chemical_return where active=1 and id_chemical_issue_trans='" + id_chemical_issue_trans + "'";


                    sSqlstmt = sSqlstmt + sSearchtext + " order by id_chemical_return asc";

                    DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                    {
                      
                        for (int i = 0; i < dsEmmetList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                EmmetModels objModel = new EmmetModels
                                {

                                    id_chemical_return = dsEmmetList.Tables[0].Rows[i]["id_chemical_return"].ToString(),
                                    id_chemical_issue_trans = (dsEmmetList.Tables[0].Rows[i]["id_chemical_issue_trans"].ToString()),
                                    return_qty = Convert.ToInt32(dsEmmetList.Tables[0].Rows[i]["return_qty"].ToString()),
                                    return_status = objGlobaldata.GetDropdownitemById(dsEmmetList.Tables[0].Rows[i]["return_status"].ToString()),
                                };

                                DateTime dtDocDate;

                                if (dsEmmetList.Tables[0].Rows[i]["return_date"].ToString() != ""
                                   && DateTime.TryParse(dsEmmetList.Tables[0].Rows[i]["return_date"].ToString(), out dtDocDate))
                                {
                                    objModel.return_date = dtDocDate;
                                }

                                objList.EmmetList.Add(objModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in ReturnChemicalReturnList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Data Not Exists";
                        return RedirectToAction("ReturnChemicalList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("ReturnChemicalList");/*Change*/
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReturnChemicalReturnList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objList.EmmetList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult ReturnStockEdit()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {

                ViewBag.Status = objGlobaldata.GetDropdownList("Material Status");
                if (Request.QueryString["id_stock_return"] != null && Request.QueryString["id_stock_return"] != "")
                {
                    string id_stock_return = Request.QueryString["id_stock_return"];

                    string sSqlstmt = "select id_stock_return,id_stock_issue_trans,return_date,return_qty,return_status from t_stock_return where id_stock_return = '" + id_stock_return + "'";

                    DataSet dsEmmetList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsEmmetList.Tables.Count > 0 && dsEmmetList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new EmmetModels
                        {
                            id_stock_return = dsEmmetList.Tables[0].Rows[0]["id_stock_return"].ToString(),
                            id_stock_issue_trans = (dsEmmetList.Tables[0].Rows[0]["id_stock_issue_trans"].ToString()),
                            return_qty = Convert.ToInt32(dsEmmetList.Tables[0].Rows[0]["return_qty"].ToString()),
                            return_status = (dsEmmetList.Tables[0].Rows[0]["return_status"].ToString()),
                        };
                        objModel.id_stock_issue = Request.QueryString["id_stock_issue"];
                        DateTime dtDocDate;

                        if (dsEmmetList.Tables[0].Rows[0]["return_date"].ToString() != ""
                              && DateTime.TryParse(dsEmmetList.Tables[0].Rows[0]["return_date"].ToString(), out dtDocDate))
                        {
                            objModel.return_date = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("IssueStockList");/*Change*/
                    }

                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("IssueStockList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReturnStockEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("IssueStockList");
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ReturnStockEdit(EmmetModels objModel, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                if (DateTime.TryParse(form["return_date"], out dateValue) == true)
                {
                    objModel.return_date = dateValue;
                }

                if (objModel.FunUpdateReturnStock(objModel))
                {
                    TempData["Successdata"] = "Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReturnStockEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ReturnStockList", new { id_stock_issue = objModel.id_stock_issue, id_stock_issue_trans = objModel.id_stock_issue_trans });
        }

        [AllowAnonymous]
        public JsonResult ReturnStockDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_stock_return"] != null && form["id_stock_return"] != "")
                {
                    EmmetModels Doc = new EmmetModels();
                    string id_stock_return = form["id_stock_return"];
                    if (Doc.FunDeleteReturnStockDoc(id_stock_return))
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReturnStockDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult IssueStockDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_stock_issue"] != null && form["id_stock_issue"] != "")
                {
                    EmmetModels Doc = new EmmetModels();
                    string id_stock_issue = form["id_stock_issue"];
                    if (Doc.FunDeleteIssueStockDoc(id_stock_issue))
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueStockDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddChemical()
        {
            try
            {
                ViewBag.CatList = objGlobaldata.getMaterialCategoryChemicalList();
                ViewBag.Suppplier = objGlobaldata.GetSupplierList();

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddChemical: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddChemical(EmmetModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> po_upload, HttpPostedFileBase upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                EmmetModelsList objList = new EmmetModelsList();
                objList.EmmetList = new List<EmmetModels>();


                DateTime dateValue;
                if (DateTime.TryParse(form["added_date"], out dateValue) == true)
                {
                    objModel.added_date = dateValue;
                }
                if (upload != null && upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Emmet"), Path.GetFileName(upload.FileName));
                        string sFilename = "Doc" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        upload.SaveAs(sFilepath + "/" + sFilename);
                        objModel.upload = "~/DataUpload/MgmtDocs/Emmet/" + sFilename;

                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddStock-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (po_upload != null && files.ContentLength > 0)
                {
                    objModel.po_upload = "";
                    foreach (var file in po_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Emmet"), Path.GetFileName(file.FileName));
                            string sFilename = "PO" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.po_upload = objModel.po_upload + "," + "~/DataUpload/MgmtDocs/Emmet/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddRoundbar-upload: " + ex.ToString());
                        }
                    }
                    objModel.po_upload = objModel.po_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["category" + i] != null && form["material" + i] != null && form["qty" + i] != null && form["price" + i] != null)
                    {
                        EmmetModels objEmmet = new EmmetModels();
                        objEmmet.category = (form["category" + i]);
                        objEmmet.material = (form["material" + i]);
                        objEmmet.qty = Convert.ToInt32(form["qty" + i]);
                        objEmmet.price = Convert.ToDecimal(form["price" + i]);
                        objEmmet.tprice = Convert.ToDecimal(form["tprice" + i]);
                        if (DateTime.TryParse(form["expiry_date" + i], out dateValue) == true)
                        {
                            objEmmet.expiry_date = dateValue;
                        }
                        objList.EmmetList.Add(objEmmet);
                    }
                }


                if (objModel.FunAddChemical(objModel, objList))
                {
                    TempData["Successdata"] = "Added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddStock: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AddChemical");
        }



        [HttpGet]
        [AllowAnonymous]
        public ActionResult ChemicalIssue()
        {
            EmmetModels objModel = new EmmetModels();
            try
            {
                ViewBag.Emplist = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.category = objGlobaldata.getMaterialCategoryChemicalList();
                string sql = "update t_stock_master set dummy_bal='0' where dummy_bal != '0'";
                objGlobaldata.ExecuteQuery(sql);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChemicalIssue: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChemicalIssue(EmmetModels objModel, FormCollection form)
        {
            try
            {
                EmmetModelsList objList = new EmmetModelsList();
                objList.EmmetList = new List<EmmetModels>();

                DateTime dateValue;
                if (DateTime.TryParse(form["issue_date"], out dateValue) == true)
                {
                    objModel.issue_date = dateValue;
                }
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["qty" + i] != null && form["expiry_date" + i] != null && form["issue_qty" + i] != null)
                    {
                        EmmetModels objEmmet = new EmmetModels();
                        objEmmet.empid = (form["empid" + i]);
                        objEmmet.category = (form["category" + i]);
                        objEmmet.material = (form["material" + i]);
                        objEmmet.id_stock = (form["id_stock" + i]);
                        objEmmet.issue_qty = Convert.ToInt32(form["issue_qty" + i]);
                        if (DateTime.TryParse(form["expiry_date" + i], out dateValue) == true)
                        {
                            objEmmet.expiry_date = dateValue;
                        }
                        objList.EmmetList.Add(objEmmet);
                    }
                }
                if (objModel.FunAddChemicalIssue(objModel, objList))
                {
                    TempData["Successdata"] = "Stock updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChemicalIssue: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ChemicalIssue");
        }


        [AllowAnonymous]
        public ActionResult YearEndStockReport()
        {     
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YearEndStockReport(FormCollection form)
        {
            try
            {
                DateTime fromdateValue = new DateTime();
                DateTime toDateValue = new DateTime();

                if (DateTime.TryParse(form["FromDate"], out fromdateValue) == true)
                {
                }
                if (DateTime.TryParse(form["ToDate"], out toDateValue) == true)
                {
                }
                DataSet dsStockReport = objGlobaldata.GetYearlyStockReportdetails(fromdateValue.ToString("yyyy/MM/dd"), toDateValue.ToString("yyyy/MM/dd"));
                ViewBag.dsStock = dsStockReport;
                ViewBag.fromdateValue = (fromdateValue).ToString("dd/MM/yyyy");
                ViewBag.toDateValue = (toDateValue).ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in YearEndStockReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
          
        }

        [AllowAnonymous]
        public ActionResult PurchasedStockReport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchasedStockReport(FormCollection form)
        {
            try
            {
                DateTime fromdateValue = new DateTime();
                DateTime toDateValue = new DateTime();

                if (DateTime.TryParse(form["FromDate"], out fromdateValue) == true)
                {
                }
                if (DateTime.TryParse(form["ToDate"], out toDateValue) == true)
                {
                }
                DataSet dsStockReport = objGlobaldata.PurchaseStockReportdetails(fromdateValue.ToString("yyyy/MM/dd"), toDateValue.ToString("yyyy/MM/dd"));
                ViewBag.dsStock = dsStockReport;
                ViewBag.fromdateValue = (fromdateValue).ToString("dd/MM/yyyy");
                ViewBag.toDateValue = (toDateValue).ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PurchasedStockReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();

        }

        [AllowAnonymous]
        public ActionResult SupplierAnalysisReport()
        {
            try
            {
                ViewBag.Supplier = objGlobaldata.GetSupplierList();
            }
            catch(Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierAnalysisReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }


        public JsonResult getSupplierReport(string FromDate, string ToDate, string[] supplier)
        {
            DataSet dsStockReport = new DataSet();
           
            try
            {
                string sup = string.Join(",", supplier);
                DateTime fromdateValue = new DateTime();
                DateTime toDateValue = new DateTime();
                if (DateTime.TryParse(FromDate, out fromdateValue) == true)
                {
                }
                if (DateTime.TryParse(ToDate, out toDateValue) == true)
                {
                }

                dsStockReport = objGlobaldata.SupplierAnalysisReportdetails(fromdateValue.ToString("yyyy/MM/dd"), toDateValue.ToString("yyyy/MM/dd"), sup);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in getSupplierReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            //var json = new JavaScriptSerializer().Serialize(dsStockReport,formatt);
            var json = JsonConvert.SerializeObject(dsStockReport, Formatting.Indented);
            return Json(json);
        }

        [AllowAnonymous]
        public ActionResult UsageReport()
        {
            return View();
        }

        public JsonResult GetusageReport(string FromDate, string ToDate)
        {
            DataSet dsStockReport = new DataSet();

            try
            {
              
                DateTime fromdateValue = new DateTime();
                DateTime toDateValue = new DateTime();
                if (DateTime.TryParse(FromDate, out fromdateValue) == true)
                {
                }
                if (DateTime.TryParse(ToDate, out toDateValue) == true)
                {
                }

                dsStockReport = objGlobaldata.UsageReportdetails(fromdateValue.ToString("yyyy/MM/dd"), toDateValue.ToString("yyyy/MM/dd"));
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UsageReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            //var json = new JavaScriptSerializer().Serialize(dsStockReport,formatt);
            var json = JsonConvert.SerializeObject(dsStockReport, Formatting.Indented);
            return Json(json);
        }

        [AllowAnonymous]
        public ActionResult OutofStockReport()
        {
            return View();
        }

        public JsonResult GetOutOfStockReport(string FromDate, string ToDate)
        {
            DataSet dsStockReport = new DataSet();

            try
            {

                DateTime fromdateValue = new DateTime();
                DateTime toDateValue = new DateTime();
                if (DateTime.TryParse(FromDate, out fromdateValue) == true)
                {
                }
                if (DateTime.TryParse(ToDate, out toDateValue) == true)
                {
                }

                dsStockReport = objGlobaldata.OutOfStockReportdetails(fromdateValue.ToString("yyyy/MM/dd"), toDateValue.ToString("yyyy/MM/dd"));
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetOutOfStockReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
           
            var json = JsonConvert.SerializeObject(dsStockReport, Formatting.Indented);
            return Json(json);
        }

        [AllowAnonymous]
        public ActionResult PriceComparisonReport()
        {
            try
            {
                ViewBag.Supplier = objGlobaldata.GetSupplierList();
                ViewBag.Material = objGlobaldata.getMaterialList("");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PriceComparisonReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        public JsonResult getPriceCompReport(string FromDate, string ToDate, string[] supplier,string material)
        {
            DataSet dsStockReport = new DataSet();

            try
            {
                string sup = string.Join(",", supplier);
                DateTime fromdateValue = new DateTime();
                DateTime toDateValue = new DateTime();
                if (DateTime.TryParse(FromDate, out fromdateValue) == true)
                {
                }
                if (DateTime.TryParse(ToDate, out toDateValue) == true)
                {

                }
                dsStockReport = objGlobaldata.PriceComparisonReportdetails(fromdateValue.ToString("yyyy/MM/dd"), toDateValue.ToString("yyyy/MM/dd"), sup, material);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in getPriceCompReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            //var json = new JavaScriptSerializer().Serialize(dsStockReport,formatt);
            var json = JsonConvert.SerializeObject(dsStockReport, Formatting.Indented);
            return Json(json);
        }
    }
}