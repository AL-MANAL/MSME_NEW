using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.IO;
using PagedList;
using PagedList.Mvc;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class RecordsRetentionController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public RecordsRetentionController()
        {
            ViewBag.Menutype = "Documents";
            ViewBag.SubMenutype = "Records";
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AddRecords()
        {
            RecordsRetentionModels objrec = new RecordsRetentionModels();

            try
            {

                objrec.branch = objGlobaldata.GetCurrentUserSession().division;
                objrec.Dept_id = objGlobaldata.GetCurrentUserSession().DeptID;
                objrec.Work_Location = objGlobaldata.GetCurrentUserSession().Work_Location;

               // ViewBag.DeptId = objGlobaldata.GetDepartmentListbox();
                //ViewBag.RecordsType = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.RecordsType = objGlobaldata.GetDropdownList("Record Type");
                ViewBag.RecordRetention = objGlobaldata.GetDropdownList("RecordRetention");
                ViewBag.DocList = objGlobaldata.GetDocumentNameList();
                //string Work_Location = objGlobaldata.GetCurrentUserSession().division;
                //ViewBag.WorkLocation = objGlobaldata.GetDivisionLocationList(Work_Location);
                ViewBag.HeaderId = objGlobaldata.GetDropdownHeaderIdByDesc("Record Type");
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.DeptId = objGlobaldata.GetDepartmentListbox(objrec.branch);
                ViewBag.WorkLocation = objGlobaldata.GetDivisionLocationList(objrec.branch);
                ViewBag.CriteriaList = objGlobaldata.GetDropdownList("Recored Retention Criteria");

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRecords: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objrec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRecords(RecordsRetentionModels objRecordsRetentionModels, FormCollection form, HttpPostedFileBase Upload_Path)
        {
            try
            {
                DateTime dateValue;

                objRecordsRetentionModels.branch = form["branch"];
                objRecordsRetentionModels.Work_Location = form["Work_Location"];
                objRecordsRetentionModels.Dept_id = form["Dept_id"];
                if (objRecordsRetentionModels.branch == null || objRecordsRetentionModels.branch == "")
                {
                    objRecordsRetentionModels.branch = objGlobaldata.GetCurrentUserSession().division;
                }

                string sServerMapath = Server.MapPath("~/DataUpload/MgmtDocs/IMSDocs");
                try
                {
                    objRecordsRetentionModels.Upload_Path = "";
                    if (Upload_Path != null)
                    {
                        string spath = Path.Combine(sServerMapath, Path.GetFileName(Upload_Path.FileName));
                        string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                        Upload_Path.SaveAs(sFilepath + "/" + sFilename);
                        objRecordsRetentionModels.Upload_Path = "~/DataUpload/MgmtDocs/IMSDocs/" + sFilename;
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }

                if (form["Generated_On"] != null && DateTime.TryParse(form["Generated_On"], out dateValue) == true)
                {
                    objRecordsRetentionModels.Generated_On = dateValue;
                }

                if (objRecordsRetentionModels.FunAddRecords(objRecordsRetentionModels))
                {
                    TempData["Successdata"] = "Added Record details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRecords: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RecordsList");
        }
        
        [AllowAnonymous]
        public JsonResult RecordDocDelete(FormCollection form)
        {
            try
            {

                  if (form["Records_Id"] != null && form["Records_Id"] != "")
                    {

                        RecordsRetentionModels Doc = new RecordsRetentionModels();
                        string sRecords_Id = form["Records_Id"];


                        if (Doc.FunDeleteRecordDoc(sRecords_Id))
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
                objGlobaldata.AddFunctionalLog("Exception in RecordDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        
        [AllowAnonymous]
        public ActionResult RecordsList(string Record_Title, string branch_name)
        {
            RecordsRetentionModelsList objEmergencyPlanNRecordList = new RecordsRetentionModelsList();
            objEmergencyPlanNRecordList.lstRecordsRetention = new List<RecordsRetentionModels>();
            RecordsRetentionModels objrec = new RecordsRetentionModels();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT Records_Id, Records_Type, Work_Location, Dept_id, Doc_name, Record_Title, Generated_On, Upload_Path, Retention_Period, LoggedBy, LoggedDate,branch,retention_criteria"
                     + " FROM t_records_retention_period where Active=1";

                if (Record_Title != null && Record_Title != "")
                {
                    ViewBag.Record_Title = Record_Title;

                    sSqlstmt = sSqlstmt + " and (Record_Title ='" + Record_Title + "'  or Record_Title like '%" + Record_Title + "%' or Record_Title like '" + Record_Title + "%')";
                }
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Generated_On desc,Record_Title";
                DataSet dsRecordsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRecordsList.Tables.Count > 0)
                {  
                    for (int i = 0; i < dsRecordsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            RecordsRetentionModels objRecordsRetention = new RecordsRetentionModels
                            {
                                Records_Id = dsRecordsList.Tables[0].Rows[i]["Records_Id"].ToString(),
                                Records_Type = dsRecordsList.Tables[0].Rows[i]["Records_Type"].ToString(),
                                Work_Location = objGlobaldata.GetDivisionLocationById(dsRecordsList.Tables[0].Rows[i]["Work_Location"].ToString()),
                                Dept_id = objGlobaldata.GetMultiDeptNameById(dsRecordsList.Tables[0].Rows[i]["Dept_id"].ToString()),
                                Doc_name = objGlobaldata.GetDocNameByID(dsRecordsList.Tables[0].Rows[i]["Doc_name"].ToString()),
                                Record_Title = (dsRecordsList.Tables[0].Rows[i]["Record_Title"].ToString()),
                                Upload_Path = dsRecordsList.Tables[0].Rows[i]["Upload_Path"].ToString(),
                                Retention_Period = dsRecordsList.Tables[0].Rows[i]["Retention_Period"].ToString(),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsRecordsList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsRecordsList.Tables[0].Rows[i]["branch"].ToString())
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[i]["Generated_On"].ToString(), out dateValue))
                            {
                                objRecordsRetention.Generated_On = dateValue;
                            }

                            if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[i]["LoggedDate"].ToString(), out dateValue))
                            {
                                objRecordsRetention.LoggedDate = dateValue;
                            }

                            objEmergencyPlanNRecordList.lstRecordsRetention.Add(objRecordsRetention);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RecordsList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RecordsList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEmergencyPlanNRecordList.lstRecordsRetention.ToList());
        }

        [AllowAnonymous]
        public JsonResult RecordsListSearch(string Record_Title, string branch_name)
        {
            RecordsRetentionModelsList objEmergencyPlanNRecordList = new RecordsRetentionModelsList();
            objEmergencyPlanNRecordList.lstRecordsRetention = new List<RecordsRetentionModels>();
            RecordsRetentionModels objrec = new RecordsRetentionModels();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT Records_Id, Records_Type, Work_Location, Dept_id, Doc_name, Record_Title, Generated_On, Upload_Path, Retention_Period, LoggedBy, LoggedDate,branch"
                     + " FROM t_records_retention_period where Active=1";

                if (Record_Title != null && Record_Title != "")
                {
                    ViewBag.Record_Title = Record_Title;

                    sSqlstmt = sSqlstmt + " and (Record_Title ='" + Record_Title + "'  or Record_Title like '%" + Record_Title + "%' or Record_Title like '" + Record_Title + "%')";
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Generated_On desc,Record_Title";
                DataSet dsRecordsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRecordsList.Tables.Count > 0)
                {


                    for (int i = 0; i < dsRecordsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            RecordsRetentionModels objRecordsRetention = new RecordsRetentionModels
                            {
                                Records_Id = dsRecordsList.Tables[0].Rows[i]["Records_Id"].ToString(),
                                Records_Type = dsRecordsList.Tables[0].Rows[i]["Records_Type"].ToString(),
                                Work_Location = objGlobaldata.GetDivisionLocationById(dsRecordsList.Tables[0].Rows[i]["Work_Location"].ToString()),
                                Dept_id = objGlobaldata.GetMultiDeptNameById(dsRecordsList.Tables[0].Rows[i]["Dept_id"].ToString()),
                                Doc_name = objGlobaldata.GetDocNameByID(dsRecordsList.Tables[0].Rows[i]["Doc_name"].ToString()),
                                Record_Title = (dsRecordsList.Tables[0].Rows[i]["Record_Title"].ToString()),
                                Upload_Path = dsRecordsList.Tables[0].Rows[i]["Upload_Path"].ToString(),
                                Retention_Period = dsRecordsList.Tables[0].Rows[i]["Retention_Period"].ToString(),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsRecordsList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsRecordsList.Tables[0].Rows[i]["branch"].ToString())

                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[i]["Generated_On"].ToString(), out dateValue))
                            {
                                objRecordsRetention.Generated_On = dateValue;
                            }

                            if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[i]["LoggedDate"].ToString(), out dateValue))
                            {
                                objRecordsRetention.LoggedDate = dateValue;
                            }

                            objEmergencyPlanNRecordList.lstRecordsRetention.Add(objRecordsRetention);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RecordsListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RecordsListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
               
        [AllowAnonymous]
        public ActionResult RecordsDetails()
        {
            try
            {
                RecordsRetentionModels objrec = new RecordsRetentionModels();
                ViewBag.WorkLocation = objGlobaldata.GetCompanyBranchListbox();
                if (Request.QueryString["Records_Id"] != null && Request.QueryString["Records_Id"] != "")
                {
                    string sRecords_Id = Request.QueryString["Records_Id"];
                    string sSqlstmt = "SELECT Records_Id, Records_Type, Work_Location, Dept_id, Doc_name, Record_Title, Generated_On, Upload_Path, Retention_Period, LoggedBy, LoggedDate,Jobno,branch,retention_criteria "
                         + " FROM t_records_retention_period where Records_Id='" + sRecords_Id + "'";

                    DataSet dsRecordsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsRecordsList.Tables.Count > 0 && dsRecordsList.Tables[0].Rows.Count > 0)
                    {
                        RecordsRetentionModels objRecordsRetention = new RecordsRetentionModels
                        {
                            Records_Id = dsRecordsList.Tables[0].Rows[0]["Records_Id"].ToString(),
                            Records_Type = dsRecordsList.Tables[0].Rows[0]["Records_Type"].ToString(),
                            Work_Location = objGlobaldata.GetDivisionLocationById(dsRecordsList.Tables[0].Rows[0]["Work_Location"].ToString()),
                            Dept_id = objGlobaldata.GetMultiDeptNameById(dsRecordsList.Tables[0].Rows[0]["Dept_id"].ToString()),
                            Doc_name = objGlobaldata.GetDocNameByID(dsRecordsList.Tables[0].Rows[0]["Doc_name"].ToString()),
                            Record_Title = (dsRecordsList.Tables[0].Rows[0]["Record_Title"].ToString()),
                            Upload_Path = dsRecordsList.Tables[0].Rows[0]["Upload_Path"].ToString(),
                            Retention_Period = dsRecordsList.Tables[0].Rows[0]["Retention_Period"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsRecordsList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Jobno = dsRecordsList.Tables[0].Rows[0]["Jobno"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsRecordsList.Tables[0].Rows[0]["branch"].ToString()),
                            retention_criteria = objGlobaldata.GetDropdownitemById(dsRecordsList.Tables[0].Rows[0]["retention_criteria"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["Generated_On"].ToString(), out dateValue))
                        {
                            objRecordsRetention.Generated_On = dateValue;
                        }

                        if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["LoggedDate"].ToString(), out dateValue))
                        {
                            objRecordsRetention.LoggedDate = dateValue;
                        }

                        return View(objRecordsRetention);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("RecordsList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Records Id cannot be null";
                    return RedirectToAction("RecordsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RecordsDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RecordsList");
        }
        
        [AllowAnonymous]
        public ActionResult RecordsInfo(int id)
        {
            try
            {
                RecordsRetentionModels objrec = new RecordsRetentionModels();
                string sSqlstmt = "SELECT Records_Id, Records_Type, Work_Location, Dept_id, Doc_name, Record_Title, Generated_On, Upload_Path, Retention_Period, LoggedBy, LoggedDate,Jobno,branch,retention_criteria "
                     + " FROM t_records_retention_period where Records_Id='" + id + "'";

                DataSet dsRecordsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRecordsList.Tables.Count > 0 && dsRecordsList.Tables[0].Rows.Count > 0)
                {
                    RecordsRetentionModels objRecordsRetention = new RecordsRetentionModels
                    {
                        Records_Id = dsRecordsList.Tables[0].Rows[0]["Records_Id"].ToString(),
                        Records_Type = dsRecordsList.Tables[0].Rows[0]["Records_Type"].ToString(),
                        Work_Location = objGlobaldata.GetDivisionLocationById(dsRecordsList.Tables[0].Rows[0]["Work_Location"].ToString()),
                        Dept_id = objGlobaldata.GetMultiDeptNameById(dsRecordsList.Tables[0].Rows[0]["Dept_id"].ToString()),
                        Doc_name = objGlobaldata.GetDocNameByID(dsRecordsList.Tables[0].Rows[0]["Doc_name"].ToString()),
                        Record_Title = (dsRecordsList.Tables[0].Rows[0]["Record_Title"].ToString()),
                        Upload_Path = dsRecordsList.Tables[0].Rows[0]["Upload_Path"].ToString(),
                        Retention_Period = dsRecordsList.Tables[0].Rows[0]["Retention_Period"].ToString(),
                        LoggedBy = objGlobaldata.GetEmpHrNameById(dsRecordsList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                        Jobno = dsRecordsList.Tables[0].Rows[0]["Jobno"].ToString(),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsRecordsList.Tables[0].Rows[0]["branch"].ToString()),
                        retention_criteria = objGlobaldata.GetDropdownitemById(dsRecordsList.Tables[0].Rows[0]["retention_criteria"].ToString()),
                    };

                    DateTime dateValue;
                    if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["Generated_On"].ToString(), out dateValue))
                    {
                        objRecordsRetention.Generated_On = dateValue;
                    }

                    if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["LoggedDate"].ToString(), out dateValue))
                    {
                        objRecordsRetention.LoggedDate = dateValue;
                    }

                    return View(objRecordsRetention);
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("RecordsList");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RecordsDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RecordsList");
        }
               
        [AllowAnonymous]
        public ActionResult RecordsEdit()
        {
            try
            {
                RecordsRetentionModels objrec = new RecordsRetentionModels();
                 //string Work_Location = objGlobaldata.GetCurrentUserSession().division;
                //ViewBag.WorkLocation = objGlobaldata.GetDivisionLocationList(Work_Location);

                ViewBag.DocList = objGlobaldata.GetDocumentNameList();
                if (Request.QueryString["Records_Id"] != null && Request.QueryString["Records_Id"] != "")
                {
                    string sRecords_Id = Request.QueryString["Records_Id"];

                    string sSqlstmt = "SELECT Records_Id, Records_Type, Work_Location, Dept_id, Doc_name, Record_Title, Generated_On, Upload_Path, Retention_Period, LoggedBy, LoggedDate,Jobno,branch,retention_criteria"
                         + " FROM t_records_retention_period where Records_Id='" + sRecords_Id + "'";

                    DataSet dsRecordsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsRecordsList.Tables.Count > 0 && dsRecordsList.Tables[0].Rows.Count > 0)
                    {
                        RecordsRetentionModels objRecordsRetention = new RecordsRetentionModels
                        {
                            Records_Id = dsRecordsList.Tables[0].Rows[0]["Records_Id"].ToString(),
                            Records_Type = dsRecordsList.Tables[0].Rows[0]["Records_Type"].ToString(),
                            Work_Location = /*objGlobaldata.GetDivisionLocationById*/(dsRecordsList.Tables[0].Rows[0]["Work_Location"].ToString()),
                            Dept_id = (dsRecordsList.Tables[0].Rows[0]["Dept_id"].ToString()),
                            Doc_name = objGlobaldata.GetDocNameByID(dsRecordsList.Tables[0].Rows[0]["Doc_name"].ToString()),
                            Record_Title = (dsRecordsList.Tables[0].Rows[0]["Record_Title"].ToString()),
                            Upload_Path = dsRecordsList.Tables[0].Rows[0]["Upload_Path"].ToString(),
                            Retention_Period = dsRecordsList.Tables[0].Rows[0]["Retention_Period"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsRecordsList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Jobno = dsRecordsList.Tables[0].Rows[0]["Jobno"].ToString(),
                            branch = dsRecordsList.Tables[0].Rows[0]["branch"].ToString(),
                            retention_criteria = dsRecordsList.Tables[0].Rows[0]["retention_criteria"].ToString(),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["Generated_On"].ToString(), out dateValue))
                        {
                            objRecordsRetention.Generated_On = dateValue;
                        }

                        if (DateTime.TryParse(dsRecordsList.Tables[0].Rows[0]["LoggedDate"].ToString(), out dateValue))
                        {
                            objRecordsRetention.LoggedDate = dateValue;
                        }

                        //ViewBag.DeptId = objGlobaldata.GetDepartmentListbox();
                        //ViewBag.RecordsType = objGlobaldata.GetAllIsoStdListbox();
                        ViewBag.RecordsType = objGlobaldata.GetDropdownList("Record Type");
                        ViewBag.RecordRetention = objGlobaldata.GetDropdownList("RecordRetention");
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.WorkLocation = objGlobaldata.GetLocationbyMultiDivision(dsRecordsList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.DeptId = objGlobaldata.GetDepartmentList1(dsRecordsList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.CriteriaList = objGlobaldata.GetDropdownList("Recored Retention Criteria");

                        return View(objRecordsRetention);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("RecordsList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Records Id cannot be null";
                    return RedirectToAction("RecordsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RecordsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RecordsList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecordsEdit(RecordsRetentionModels objRecordsRetentionModels, FormCollection form, HttpPostedFileBase Upload_Path)
        {
            try
            {
                DateTime dateValue;

                objRecordsRetentionModels.branch = form["branch"];
                objRecordsRetentionModels.Work_Location = form["Work_Location"];
                objRecordsRetentionModels.Dept_id = form["Dept_id"];

                if (objRecordsRetentionModels.branch == null || objRecordsRetentionModels.branch == "")
                {
                    objRecordsRetentionModels.branch = objGlobaldata.GetCurrentUserSession().division;
                }

                string sServerMapath = Server.MapPath("~/DataUpload/MgmtDocs/IMSDocs");
                try
                {
                    objRecordsRetentionModels.Upload_Path = "";
                    if (Upload_Path != null)
                    {
                        string spath = Path.Combine(sServerMapath, Path.GetFileName(Upload_Path.FileName));
                        string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                        Upload_Path.SaveAs(sFilepath + "/" + sFilename);
                        objRecordsRetentionModels.Upload_Path = "~/DataUpload/MgmtDocs/IMSDocs/" + sFilename;
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }

                if (form["Generated_On"] != null && DateTime.TryParse(form["Generated_On"], out dateValue) == true)
                {
                    objRecordsRetentionModels.Generated_On = dateValue;
                }


                if (objRecordsRetentionModels.FunUpdateRecords(objRecordsRetentionModels))
                {
                    TempData["Successdata"] = "Updated Record details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RecordsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("RecordsList");
        }

        [HttpPost]
        public ActionResult FunRetention_PeriodList(string idMgmt)
        {
            RecordsRetentionModels objMgmtDocuments = new RecordsRetentionModels();
            string lstEmp = objMgmtDocuments.GetMultiRetention_PeriodList(idMgmt);
            return Json(lstEmp);
        }
    }
}
