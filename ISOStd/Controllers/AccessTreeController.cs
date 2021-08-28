using ISOStd.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI.WebControls;

namespace ISOStd.Controllers
{
    public class AccessTreeController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        [AllowAnonymous]
        public ActionResult AddAccessTree(int? page,string RoleId, FormCollection form)
        {
            AccessTreeModelsList objList = new AccessTreeModelsList();
            objList.TreeList = new List<AccessTreeModels>();
            try
            {
                AccessTreeModels obj = new AccessTreeModels();
              
                string sSqlstmt = "";
                //  string sSearchtext = "";

                //if (Request.QueryString["appl_branch"] != null && Request.QueryString["appl_branch"] != "" && Request.QueryString["RoleId"] != null && Request.QueryString["RoleId"] != "" && Request.QueryString["branch_id"] != null && Request.QueryString["branch_id"] != "")
                if ( Request.QueryString["RoleId"] != null && Request.QueryString["RoleId"] != "")
                {
                    string appl_branch=Request.QueryString["appl_branch"];
                    string sRoleID = Request.QueryString["RoleId"];
                    string sbranch_id = Request.QueryString["branch_id"];
                    ViewBag.role_id = sRoleID;
                    //sSqlstmt = "Select RoleTree from t_access";
                    //ViewBag.Roleval = sRoleID;
                    //sSearchtext = sSearchtext + " where Id ='" + sRoleID + "'";
                    string role = objGlobaldata.GetRolesNameById(sRoleID);
                    string branch = objGlobaldata.GetCompanyBranchNameById(sbranch_id);
                    //if (appl_branch == "Yes")
                    //{
                    //      ViewBag.Role = String.Concat(role, "-", branch);
                    //}
                    //else{
                          ViewBag.Role = String.Concat(role);
                    //}

                    //sSqlstmt = "SELECT Id_access,a.id,parent_level,BranchName,BranchTree FROM t_company_branch a," +
                    //    " t_access b where active=1 and role_id ='" + sRoleID + "' and branch_id ='" + sbranch_id + "' order by parent_level,a.id asc ";

                    sSqlstmt = "SELECT Id_access,a.id,parent_level,BranchName,BranchTree FROM t_company_branch a," +
                        " t_access b where active=1 and role_id ='" + sRoleID + "' order by parent_level,a.id asc ";
                    DataSet dsTreeList = objGlobaldata.Getdetails(sSqlstmt);

                  
                    if (dsTreeList.Tables.Count > 0 && dsTreeList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsTreeList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                AccessTreeModels objModels = new AccessTreeModels
                                {
                                    Id_access = (dsTreeList.Tables[0].Rows[i]["Id_access"].ToString()),
                                    id = (dsTreeList.Tables[0].Rows[i]["id"].ToString()),
                                    parent_level = (dsTreeList.Tables[0].Rows[i]["parent_level"].ToString()),

                                    BranchName = (dsTreeList.Tables[0].Rows[i]["BranchName"].ToString()),
                                    BranchTree = (dsTreeList.Tables[0].Rows[i]["BranchTree"].ToString()),
                                   // Levels = dsTreeList.Tables[0].Rows[0]["levels"].ToString()
                                };
                                ViewBag.Id = objModels.Id_access;
                                objList.TreeList.Add(objModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AddAccessTree: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAccessTree: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objList.TreeList.ToList().ToPagedList(page ?? 1, 10000));
        }


        [AllowAnonymous]
        public JsonResult UpdateAccessTree(string selected, string Id, string role_id)
        {
            AccessTreeModels objAccessList = new AccessTreeModels();
            bool IssueNo = false;
            //if (selected != null)
            //{
                IssueNo = objAccessList.FunUpdateAccessTree(selected, Id, role_id);
            //}

            return Json(IssueNo);
        }       
    }
    }