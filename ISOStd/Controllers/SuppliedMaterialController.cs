using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using Rotativa;

namespace ISOStd.Controllers
{
    public class SuppliedMaterialController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public SuppliedMaterialController()
        {
            ViewBag.SubMenutype = "SuppliedMaterial";
            ViewBag.Menutype = "Suppliers";
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddSuppliedMaterial()
        {
            SuppliedMaterialModels objmaterial = new SuppliedMaterialModels();
            try
            {
                Initilization();

                objmaterial.branch = objGlobaldata.GetCurrentUserSession().division;
                objmaterial.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objmaterial.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(objmaterial.branch, objmaterial.Department, objmaterial.Location);
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objmaterial.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objmaterial.branch);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSuppliedMaterial: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objmaterial);
        }

        public void Initilization()
        {
            ViewBag.Division = objGlobaldata.GetCompanyBranchListbox();            
            ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
            ViewBag.ProviderType = objGlobaldata.GetDropdownList("Provider Type");
            ViewBag.Supplier = objGlobaldata.GetSupplierList();
            ViewBag.Customer = objGlobaldata.GetCustomerListbox();
            ViewBag.OperationType = objGlobaldata.GetConstantValue("MaterialOperationType");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddSuppliedMaterial(FormCollection form, SuppliedMaterialModels objMdl/*, IEnumerable<HttpPostedFileBase> upload*/)
        {
            try
            {
                DateTime dateValue;

                objMdl.provider_type = form["provider_type"];
                objMdl.supplier_name = form["supplier_name"];
                objMdl.Department = form["Department"];
                objMdl.Location = form["Location"];
                objMdl.branch = form["branch"];

                if (form["order_date"] != null && DateTime.TryParse(form["order_date"], out dateValue) == true)
                {
                    objMdl.order_date = dateValue;
                }                

                SuppliedMaterialModelsList objList = new SuppliedMaterialModelsList();
                objList.MaterialList = new List<SuppliedMaterialModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    SuppliedMaterialModels objFireMdl = new SuppliedMaterialModels();
                    if (form["operation_type" + i] != null && form["quantity" + i] != null && form["qty_date" + i] != null)
                    {
                        objFireMdl.operation_type = form["operation_type" + i];
                        objFireMdl.quantity = Convert.ToInt32(form["quantity" + i]);
                        objFireMdl.done_by = form["done_by" + i];                       
                                         
                        if (DateTime.TryParse(form["qty_date" + i], out dateValue) == true)
                        {
                            objFireMdl.qty_date = dateValue;
                        }
                       
                        objList.MaterialList.Add(objFireMdl);
                    }
                }

                if (objMdl.FunAddSuppliedMaterial(objMdl, objList))
                {
                    TempData["Successdata"] = "Added  successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSuppliedMaterial: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SuppliedMaterialList");
        }

        [AllowAnonymous]
        public ActionResult SuppliedMaterialList(FormCollection form, int? page, string branch_name)
        {
            SuppliedMaterialModelsList objList = new SuppliedMaterialModelsList();
            objList.MaterialList = new List<SuppliedMaterialModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_materials,orderno,order_date,provider_type,supplier_name,customer_name,remark,branch,details,Department,Location" +
                    " from t_supplied_materials where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + " order by id_materials desc";

                DataSet dsFireList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsFireList.Tables.Count > 0 && dsFireList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsFireList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            SuppliedMaterialModels objFireMdl = new SuppliedMaterialModels
                            {
                                id_materials = dsFireList.Tables[0].Rows[i]["id_materials"].ToString(),
                                orderno = dsFireList.Tables[0].Rows[i]["orderno"].ToString(),
                                provider_type = objGlobaldata.GetDropdownitemById(dsFireList.Tables[0].Rows[i]["provider_type"].ToString()),
                                supplier_name = objGlobaldata.GetSupplierNameById(dsFireList.Tables[0].Rows[i]["supplier_name"].ToString()),
                                customer_name = objGlobaldata.GetCustomerNameById(dsFireList.Tables[0].Rows[i]["customer_name"].ToString()),
                                remark = (dsFireList.Tables[0].Rows[i]["remark"].ToString()),
                                details = (dsFireList.Tables[0].Rows[i]["details"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsFireList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsFireList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsFireList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsFireList.Tables[0].Rows[i]["order_date"].ToString() != ""
                               && DateTime.TryParse(dsFireList.Tables[0].Rows[i]["order_date"].ToString(), out dtDocDate))
                            {
                                objFireMdl.order_date = dtDocDate;
                            }
                            
                            objList.MaterialList.Add(objFireMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objList.MaterialList.ToList());
        }

        [AllowAnonymous]
        public JsonResult SuppliedMaterialListSearch(FormCollection form, int? page, string branch_name)
        {
            SuppliedMaterialModelsList objList = new SuppliedMaterialModelsList();
            objList.MaterialList = new List<SuppliedMaterialModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_materials,orderno,order_date,provider_type,supplier_name,customer_name,remark,branch,details,Department,Location" +
                    " from t_supplied_materials where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + " order by id_materials desc";

                DataSet dsFireList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsFireList.Tables.Count > 0 && dsFireList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsFireList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            SuppliedMaterialModels objFireMdl = new SuppliedMaterialModels
                            {
                                id_materials = dsFireList.Tables[0].Rows[i]["id_materials"].ToString(),
                                orderno = dsFireList.Tables[0].Rows[i]["orderno"].ToString(),
                                provider_type = objGlobaldata.GetDropdownitemById(dsFireList.Tables[0].Rows[i]["provider_type"].ToString()),
                                supplier_name = objGlobaldata.GetSupplierNameById(dsFireList.Tables[0].Rows[i]["supplier_name"].ToString()),
                                customer_name = objGlobaldata.GetCustomerNameById(dsFireList.Tables[0].Rows[i]["customer_name"].ToString()),
                                remark = (dsFireList.Tables[0].Rows[i]["remark"].ToString()),
                                details = (dsFireList.Tables[0].Rows[i]["details"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsFireList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsFireList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsFireList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsFireList.Tables[0].Rows[i]["order_date"].ToString() != ""
                               && DateTime.TryParse(dsFireList.Tables[0].Rows[i]["order_date"].ToString(), out dtDocDate))
                            {
                                objFireMdl.order_date = dtDocDate;
                            }

                            objList.MaterialList.Add(objFireMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public JsonResult SuppliedMaterialDelete(FormCollection form)
        {
            try
            {
                if (form["id_materials"] != null && form["id_materials"] != "")
                {
                    SuppliedMaterialModels Doc = new SuppliedMaterialModels();
                    string sid_materials = form["id_materials"];
                    if (Doc.FunDeleteSuppliedMaterial(sid_materials))
                    {
                        TempData["Successdata"] = "Supplied Material deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult SuppliedMaterialEdit()
        {
            SuppliedMaterialModelsList objList = new SuppliedMaterialModelsList();
            objList.MaterialList = new List<SuppliedMaterialModels>();

            SuppliedMaterialModels objFireMdl = new SuppliedMaterialModels();
            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            try
            {               

                if (Request.QueryString["id_materials"] != "" && Request.QueryString["id_materials"] != null)
                {
                    string sid_materials = Request.QueryString["id_materials"];
                    string sSqlstmt = "select id_materials,orderno,order_date,provider_type,supplier_name,customer_name,remark,branch,details,Department,Location" +
                        " from t_supplied_materials where id_materials='"+ sid_materials + "'";

                    DataSet dsFireList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsFireList.Tables.Count > 0 && dsFireList.Tables[0].Rows.Count > 0)
                    {
                           try
                            {
                                 objFireMdl = new SuppliedMaterialModels
                                {
                                    id_materials = dsFireList.Tables[0].Rows[0]["id_materials"].ToString(),
                                    orderno = dsFireList.Tables[0].Rows[0]["orderno"].ToString(),
                                    provider_type = (dsFireList.Tables[0].Rows[0]["provider_type"].ToString()),
                                    supplier_name =/* objGlobaldata.GetSupplierNameById*/(dsFireList.Tables[0].Rows[0]["supplier_name"].ToString()),
                                    customer_name =/* objGlobaldata.GetCustomerNameById*/(dsFireList.Tables[0].Rows[0]["customer_name"].ToString()),
                                    remark = (dsFireList.Tables[0].Rows[0]["remark"].ToString()),
                                    details = dsFireList.Tables[0].Rows[0]["details"].ToString(),
                                     branch = (dsFireList.Tables[0].Rows[0]["branch"].ToString()),
                                     Department = (dsFireList.Tables[0].Rows[0]["Department"].ToString()),
                                     Location = (dsFireList.Tables[0].Rows[0]["Location"].ToString()),
                                 };

                                DateTime dtDocDate;
                                if (dsFireList.Tables[0].Rows[0]["order_date"].ToString() != ""
                                   && DateTime.TryParse(dsFireList.Tables[0].Rows[0]["order_date"].ToString(), out dtDocDate))
                                {
                                    objFireMdl.order_date = dtDocDate;
                                }
                           // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsFireList.Tables[0].Rows[0]["branch"].ToString(), dsFireList.Tables[0].Rows[0]["Department"].ToString(), dsFireList.Tables[0].Rows[0]["Location"].ToString());
                            ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                            ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsFireList.Tables[0].Rows[0]["branch"].ToString());
                            ViewBag.Department = objGlobaldata.GetDepartmentList1(dsFireList.Tables[0].Rows[0]["branch"].ToString());

                        }
                        catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }                      
                    }
                    Initilization();

                    SuppliedMaterialModelsList objSuppliedList = new SuppliedMaterialModelsList();
                    objSuppliedList.MaterialList = new List<SuppliedMaterialModels>();

                    int Bal = 0;
                    string sSqlstmt1 = "select id_materials_trans,id_materials,qty_date,operation_type,quantity,done_by from t_supplied_materials_trans where id_materials= '" + sid_materials + "'";
                    DataSet dsMaterial = objGlobaldata.Getdetails(sSqlstmt1);
                    if (dsMaterial.Tables.Count > 0 && dsMaterial.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            for (int i = 0; i < dsMaterial.Tables[0].Rows.Count; i++)
                            {
                                SuppliedMaterialModels objMdl = new SuppliedMaterialModels
                                {
                                    id_materials = dsMaterial.Tables[0].Rows[i]["id_materials"].ToString(),
                                    operation_type = dsMaterial.Tables[0].Rows[i]["operation_type"].ToString(),
                                    quantity = Convert.ToInt32(dsMaterial.Tables[0].Rows[i]["quantity"].ToString()),
                                    done_by = dsMaterial.Tables[0].Rows[i]["done_by"].ToString(),
                                };
                                int qty = Convert.ToInt32(dsMaterial.Tables[0].Rows[i]["quantity"].ToString());
                                string optype = dsMaterial.Tables[0].Rows[i]["operation_type"].ToString();
                                if (optype == "Issued" && optype !="")
                                {
                                    Bal = Bal - qty;
                                }
                                else if (optype == "Received" && optype != "")
                                {
                                    Bal = Bal + qty;
                                }

                               
                                DateTime dtDate;
                                if (dsMaterial.Tables[0].Rows[i]["qty_date"].ToString() != ""
                                   && DateTime.TryParse(dsMaterial.Tables[0].Rows[i]["qty_date"].ToString(), out dtDate))
                                {
                                    objMdl.qty_date = dtDate;
                                }

                                objSuppliedList.MaterialList.Add(objMdl);
                            }
                            objFireMdl.balance = Bal;
                            ViewBag.SuppliedMaterial = objSuppliedList;
                                                       
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }                   
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objFireMdl);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SuppliedMaterialEdit(FormCollection form, SuppliedMaterialModels objMdl/*, IEnumerable<HttpPostedFileBase> upload*/)
        {
            try
            {
                DateTime dateValue;

                objMdl.provider_type = form["provider_type"];
                objMdl.supplier_name = form["supplier_name"];
                objMdl.Department = form["Department"];
                objMdl.Location = form["Location"];
                objMdl.branch = form["branch"];

                if (form["order_date"] != null && DateTime.TryParse(form["order_date"], out dateValue) == true)
                {
                    objMdl.order_date = dateValue;
                }

                SuppliedMaterialModelsList objList = new SuppliedMaterialModelsList();
                objList.MaterialList = new List<SuppliedMaterialModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    SuppliedMaterialModels objFireMdl = new SuppliedMaterialModels();
                    if (form["operation_type " + i] != null && form["quantity " + i] != null && form["qty_date " + i] != null)
                    {
                        objFireMdl.operation_type = form["operation_type " + i];
                        objFireMdl.quantity = Convert.ToInt32(form["quantity " + i]);
                        objFireMdl.done_by = form["done_by " + i];

                        if (DateTime.TryParse(form["qty_date " + i], out dateValue) == true)
                        {
                            objFireMdl.qty_date = dateValue;
                        }

                        objList.MaterialList.Add(objFireMdl);
                    }
                }

                if (objMdl.FunUpdateSuppliedMaterial(objMdl, objList))
                {
                    TempData["Successdata"] = "updated details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SuppliedMaterialList");
        }

        [AllowAnonymous]
        public ActionResult SuppliedMaterialDetails()
        {
            SuppliedMaterialModelsList objList = new SuppliedMaterialModelsList();
            objList.MaterialList = new List<SuppliedMaterialModels>();

            SuppliedMaterialModels objFireMdl = new SuppliedMaterialModels();

            try
            {
                if (Request.QueryString["id_materials"] != "" && Request.QueryString["id_materials"] != null)
                {

                    string sid_materials = Request.QueryString["id_materials"];
                    string sSqlstmt = "select id_materials,orderno,order_date,provider_type,supplier_name,customer_name,remark,branch,details,Department,Location" +
                        " from t_supplied_materials where id_materials='" + sid_materials + "'";

                    DataSet dsFireList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsFireList.Tables.Count > 0 && dsFireList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objFireMdl = new SuppliedMaterialModels
                            {
                                id_materials = dsFireList.Tables[0].Rows[0]["id_materials"].ToString(),
                                orderno = dsFireList.Tables[0].Rows[0]["orderno"].ToString(),
                                provider_type = objGlobaldata.GetDropdownitemById(dsFireList.Tables[0].Rows[0]["provider_type"].ToString()),
                                supplier_name = objGlobaldata.GetSupplierNameById(dsFireList.Tables[0].Rows[0]["supplier_name"].ToString()),
                                customer_name = objGlobaldata.GetCustomerNameById(dsFireList.Tables[0].Rows[0]["customer_name"].ToString()),
                                remark = (dsFireList.Tables[0].Rows[0]["remark"].ToString()),
                                details = dsFireList.Tables[0].Rows[0]["details"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsFireList.Tables[0].Rows[0]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsFireList.Tables[0].Rows[0]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsFireList.Tables[0].Rows[0]["Location"].ToString()),

                            };

                            DateTime dtDocDate;
                            if (dsFireList.Tables[0].Rows[0]["order_date"].ToString() != ""
                               && DateTime.TryParse(dsFireList.Tables[0].Rows[0]["order_date"].ToString(), out dtDocDate))
                            {
                                objFireMdl.order_date = dtDocDate;
                            }
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }                   

                    SuppliedMaterialModelsList objSuppliedList = new SuppliedMaterialModelsList();
                    objSuppliedList.MaterialList = new List<SuppliedMaterialModels>();

                    int Bal = 0;
                    string sSqlstmt1 = "select id_materials_trans,id_materials,qty_date,operation_type,quantity,done_by from t_supplied_materials_trans where id_materials= '" + sid_materials + "'";
                    DataSet dsMaterial = objGlobaldata.Getdetails(sSqlstmt1);
                    if (dsMaterial.Tables.Count > 0 && dsMaterial.Tables[0].Rows.Count > 0)
                    {

                        try
                        {
                            for (int i = 0; i < dsMaterial.Tables[0].Rows.Count; i++)
                            {
                                SuppliedMaterialModels objMdl = new SuppliedMaterialModels
                                {
                                    id_materials = dsMaterial.Tables[0].Rows[i]["id_materials"].ToString(),
                                    operation_type = dsMaterial.Tables[0].Rows[i]["operation_type"].ToString(),
                                    quantity = Convert.ToInt32(dsMaterial.Tables[0].Rows[i]["quantity"].ToString()),
                                    done_by = dsMaterial.Tables[0].Rows[i]["done_by"].ToString(),
                                };
                                int qty = Convert.ToInt32(dsMaterial.Tables[0].Rows[i]["quantity"].ToString());
                                string optype = dsMaterial.Tables[0].Rows[i]["operation_type"].ToString();
                                if (optype == "Issued" && optype != "")
                                {
                                    Bal = Bal - qty;
                                }
                                else if (optype == "Received" && optype != "")
                                {
                                    Bal = Bal + qty;
                                }

                                DateTime dtDate;
                                if (dsMaterial.Tables[0].Rows[i]["qty_date"].ToString() != ""
                                   && DateTime.TryParse(dsMaterial.Tables[0].Rows[i]["qty_date"].ToString(), out dtDate))
                                {
                                    objMdl.qty_date = dtDate;
                                }
                                objSuppliedList.MaterialList.Add(objMdl);
                            }
                            objFireMdl.balance = Bal;
                            ViewBag.SuppliedMaterial = objSuppliedList;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objFireMdl);
        }


        [AllowAnonymous]
        public ActionResult SuppliedMaterialInfo(int id)
        {
            SuppliedMaterialModelsList objList = new SuppliedMaterialModelsList();
            objList.MaterialList = new List<SuppliedMaterialModels>();

            SuppliedMaterialModels objFireMdl = new SuppliedMaterialModels();

            try
            {
                if (id > 0)
                {
                    string sid_materials = Request.QueryString["id_materials"];
                    string sSqlstmt = "select id_materials,orderno,order_date,provider_type,supplier_name,customer_name,remark,branch,details,Department,Location" +
                        " from t_supplied_materials where id_materials='" + id + "'";

                    DataSet dsFireList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsFireList.Tables.Count > 0 && dsFireList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objFireMdl = new SuppliedMaterialModels
                            {
                                id_materials = dsFireList.Tables[0].Rows[0]["id_materials"].ToString(),
                                orderno = dsFireList.Tables[0].Rows[0]["orderno"].ToString(),
                                provider_type = objGlobaldata.GetDropdownitemById(dsFireList.Tables[0].Rows[0]["provider_type"].ToString()),
                                supplier_name = objGlobaldata.GetSupplierNameById(dsFireList.Tables[0].Rows[0]["supplier_name"].ToString()),
                                customer_name = objGlobaldata.GetCustomerNameById(dsFireList.Tables[0].Rows[0]["customer_name"].ToString()),
                                remark = (dsFireList.Tables[0].Rows[0]["remark"].ToString()),
                                details = dsFireList.Tables[0].Rows[0]["details"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsFireList.Tables[0].Rows[0]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsFireList.Tables[0].Rows[0]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsFireList.Tables[0].Rows[0]["Location"].ToString()),

                            };

                            DateTime dtDocDate;
                            if (dsFireList.Tables[0].Rows[0]["order_date"].ToString() != ""
                               && DateTime.TryParse(dsFireList.Tables[0].Rows[0]["order_date"].ToString(), out dtDocDate))
                            {
                                objFireMdl.order_date = dtDocDate;
                            }
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }                   

                    SuppliedMaterialModelsList objSuppliedList = new SuppliedMaterialModelsList();
                    objSuppliedList.MaterialList = new List<SuppliedMaterialModels>();

                    int Bal = 0;
                    string sSqlstmt1 = "select id_materials_trans,id_materials,qty_date,operation_type,quantity,done_by from t_supplied_materials_trans where id_materials= '" + id + "'";
                    DataSet dsMaterial = objGlobaldata.Getdetails(sSqlstmt1);
                    if (dsMaterial.Tables.Count > 0 && dsMaterial.Tables[0].Rows.Count > 0)
                    {

                        try
                        {
                            for (int i = 0; i < dsMaterial.Tables[0].Rows.Count; i++)
                            {
                                SuppliedMaterialModels objMdl = new SuppliedMaterialModels
                                {
                                    id_materials = dsMaterial.Tables[0].Rows[i]["id_materials"].ToString(),
                                    operation_type = dsMaterial.Tables[0].Rows[i]["operation_type"].ToString(),
                                    quantity = Convert.ToInt32(dsMaterial.Tables[0].Rows[i]["quantity"].ToString()),
                                    done_by = dsMaterial.Tables[0].Rows[i]["done_by"].ToString(),
                                };
                                int qty = Convert.ToInt32(dsMaterial.Tables[0].Rows[i]["quantity"].ToString());
                                string optype = dsMaterial.Tables[0].Rows[i]["operation_type"].ToString();
                                if (optype == "Issued" && optype != "")
                                {
                                    Bal = Bal - qty;
                                }
                                else if (optype == "Received" && optype != "")
                                {
                                    Bal = Bal + qty;
                                }

                                DateTime dtDate;
                                if (dsMaterial.Tables[0].Rows[i]["qty_date"].ToString() != ""
                                   && DateTime.TryParse(dsMaterial.Tables[0].Rows[i]["qty_date"].ToString(), out dtDate))
                                {
                                    objMdl.qty_date = dtDate;
                                }
                                objSuppliedList.MaterialList.Add(objMdl);
                            }
                            objFireMdl.balance = Bal;
                            ViewBag.SuppliedMaterial = objSuppliedList;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialInfo: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objFireMdl);
        }


        [AllowAnonymous]
        public ActionResult SuppliedMaterialPDF(FormCollection form)
        {
            SuppliedMaterialModelsList objList = new SuppliedMaterialModelsList();
            objList.MaterialList = new List<SuppliedMaterialModels>();

            SuppliedMaterialModels objFireMdl = new SuppliedMaterialModels();

            try
            {
                if (form["id_materials"] != "" && form["id_materials"] != null)
                {
                    string sid_materials = form["id_materials"];
                    string sSqlstmt = "select id_materials,orderno,order_date,provider_type,supplier_name,customer_name,remark,branch,details,Department,Location" +
                        " from t_supplied_materials where id_materials='" + sid_materials + "'";

                    DataSet dsFireList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsFireList.Tables.Count > 0 && dsFireList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objFireMdl = new SuppliedMaterialModels
                            {
                                id_materials = dsFireList.Tables[0].Rows[0]["id_materials"].ToString(),
                                orderno = dsFireList.Tables[0].Rows[0]["orderno"].ToString(),
                                provider_type = objGlobaldata.GetDropdownitemById(dsFireList.Tables[0].Rows[0]["provider_type"].ToString()),
                                supplier_name = objGlobaldata.GetSupplierNameById(dsFireList.Tables[0].Rows[0]["supplier_name"].ToString()),
                                customer_name = objGlobaldata.GetCustomerNameById(dsFireList.Tables[0].Rows[0]["customer_name"].ToString()),
                                remark = (dsFireList.Tables[0].Rows[0]["remark"].ToString()),
                                details = dsFireList.Tables[0].Rows[0]["details"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsFireList.Tables[0].Rows[0]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsFireList.Tables[0].Rows[0]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsFireList.Tables[0].Rows[0]["Location"].ToString()),

                            };

                            DateTime dtDocDate;
                            if (dsFireList.Tables[0].Rows[0]["order_date"].ToString() != ""
                               && DateTime.TryParse(dsFireList.Tables[0].Rows[0]["order_date"].ToString(), out dtDocDate))
                            {
                                objFireMdl.order_date = dtDocDate;
                            }
                            CompanyModels objCompany = new CompanyModels();
                            dsFireList = objCompany.GetCompanyDetailsForReport(dsFireList);

                            string loggedby = objGlobaldata.GetCurrentUserSession().empid;
                            dsFireList = objGlobaldata.GetReportDetails(dsFireList, objFireMdl.orderno, loggedby, "PURCHASE DETAILS REPORT");
                            ViewBag.CompanyInfo = dsFireList;

                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    ViewBag.SuppliedMat = objFireMdl;

                    SuppliedMaterialModelsList objSuppliedList = new SuppliedMaterialModelsList();
                    objSuppliedList.MaterialList = new List<SuppliedMaterialModels>();

                    string sSqlstmt1 = "select id_materials_trans,id_materials,qty_date,operation_type,quantity,done_by from t_supplied_materials_trans where id_materials= '" + sid_materials + "'";
                    DataSet dsMaterial = objGlobaldata.Getdetails(sSqlstmt1);
                    if (dsMaterial.Tables.Count > 0 && dsMaterial.Tables[0].Rows.Count > 0)
                    {

                        try
                        {
                            for (int i = 0; i < dsMaterial.Tables[0].Rows.Count; i++)
                            {
                                SuppliedMaterialModels objMdl = new SuppliedMaterialModels
                                {
                                    id_materials = dsMaterial.Tables[0].Rows[i]["id_materials"].ToString(),
                                    operation_type = dsMaterial.Tables[0].Rows[i]["operation_type"].ToString(),
                                    quantity = Convert.ToInt32(dsMaterial.Tables[0].Rows[i]["quantity"].ToString()),
                                    done_by = dsMaterial.Tables[0].Rows[i]["done_by"].ToString(),
                                };                                

                                DateTime dtDate;
                                if (dsMaterial.Tables[0].Rows[i]["qty_date"].ToString() != ""
                                   && DateTime.TryParse(dsMaterial.Tables[0].Rows[i]["qty_date"].ToString(), out dtDate))
                                {
                                    objMdl.qty_date = dtDate;
                                }
                                objSuppliedList.MaterialList.Add(objMdl);
                            }                           
                            ViewBag.SuppliedMaterial = objSuppliedList;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialPDF: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppliedMaterialPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("SuppliedMaterialPDF")
            {
                //FileName = "SuppliedMaterialReport.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }
    }    
}