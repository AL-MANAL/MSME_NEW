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
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class TrainingProgramController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        // GET: TrainingProgram

        public TrainingProgramController()
        {
            ViewBag.Menutype = "Training";
            ViewBag.SubMenutype = "TrainingProgram";
        }
        public ActionResult Index()
        {
            return View();
        }

        // GET: TrainingProgram/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
                  

        // GET: TrainingProgram/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingProgram/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingProgram/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrainingProgram/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingProgram/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingProgram/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        //public ActionResult InitilizeTrainingsList()
        //{

        //    try
        //    {
                
        //        ViewBag.Topics = objGlobaldata.getTrainingTopicList();
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in InitilizeAddTrainings: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }



        //    return null;
        //}
        public ActionResult TrainingProgramList(string SearchText, string branch_name)
        {
            TrainingProgramModelsList objAccessList = new TrainingProgramModelsList();
            objAccessList.TrainingProgramList = new List<TrainingProgram>();
            try
            {
                string sSearchtext = "";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "Select * from t_training_program where active=1";
                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + "calyear ='" + SearchText + "' or calyear like '"+ "%" + SearchText + "%'";
                }

                if (sSearchtext != "")
                {
                    sSearchtext = " and " + sSearchtext;
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsProgram = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsProgram.Tables.Count > 0 && i < dsProgram.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                       
                        TrainingProgram objAccess = new TrainingProgram
                        {
                            Id_Training_program = dsProgram.Tables[0].Rows[i]["Id_Training_program"].ToString(),
                            topic_id = objGlobaldata.GetDropdownitemById(dsProgram.Tables[0].Rows[i]["topic_id"].ToString()),
                            Jan = dsProgram.Tables[0].Rows[i]["Jan"].ToString(),
                            Feb = dsProgram.Tables[0].Rows[i]["Feb"].ToString(),
                            Mar = dsProgram.Tables[0].Rows[i]["Mar"].ToString(),
                            Apr = dsProgram.Tables[0].Rows[i]["Apr"].ToString(),
                            May = dsProgram.Tables[0].Rows[i]["May"].ToString(),
                            June = dsProgram.Tables[0].Rows[i]["Jun"].ToString(),
                            Jul = dsProgram.Tables[0].Rows[i]["Jul"].ToString(),
                            Aug = dsProgram.Tables[0].Rows[i]["Aug"].ToString(),
                            Sept = dsProgram.Tables[0].Rows[i]["Sept"].ToString(),
                            Oct = dsProgram.Tables[0].Rows[i]["Oct"].ToString(),
                            Nov = dsProgram.Tables[0].Rows[i]["Nov"].ToString(),
                            Dec = dsProgram.Tables[0].Rows[i]["Dece"].ToString(),
                            Status = dsProgram.Tables[0].Rows[i]["stat"].ToString(),
                            year = dsProgram.Tables[0].Rows[i]["calyear"].ToString(),
                            remark= dsProgram.Tables[0].Rows[i]["remark"].ToString(),
                            //Planned_Date = dsProgram.Tables[0].Rows[i]["planned_date"].ToString(),
                            //Logged_id = dsProgram.Tables[0].Rows[i]["logged_id"].ToString(),


                        };
                        ViewBag.Topics = objGlobaldata.GetDropdownList("Training Topic");
                        objAccessList.TrainingProgramList.Add(objAccess);
                        
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in TrainingProgramList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeCompetenceEvalList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objAccessList.TrainingProgramList);
        }


        public JsonResult TrainingProgramListSearch(string SearchText, string branch_name)
        {
            TrainingProgramModelsList objAccessList = new TrainingProgramModelsList();
            objAccessList.TrainingProgramList = new List<TrainingProgram>();
            try
            {
                string sSearchtext = "";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "Select * from t_training_program where active=1";
                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + "calyear ='" + SearchText + "' or calyear like '" + "%" + SearchText + "%'";
                }

                if (sSearchtext != "")
                {
                    sSearchtext = " and " + sSearchtext;
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsProgram = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsProgram.Tables.Count > 0 && i < dsProgram.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                       
                        TrainingProgram objAccess = new TrainingProgram
                        {
                            Id_Training_program = dsProgram.Tables[0].Rows[i]["Id_Training_program"].ToString(),
                            topic_id = objGlobaldata.GetDropdownitemById(dsProgram.Tables[0].Rows[i]["topic_id"].ToString()),
                            Jan = dsProgram.Tables[0].Rows[i]["Jan"].ToString(),
                            Feb = dsProgram.Tables[0].Rows[i]["Feb"].ToString(),
                            Mar = dsProgram.Tables[0].Rows[i]["Mar"].ToString(),
                            Apr = dsProgram.Tables[0].Rows[i]["Apr"].ToString(),
                            May = dsProgram.Tables[0].Rows[i]["May"].ToString(),
                            June = dsProgram.Tables[0].Rows[i]["Jun"].ToString(),
                            Jul = dsProgram.Tables[0].Rows[i]["Jul"].ToString(),
                            Aug = dsProgram.Tables[0].Rows[i]["Aug"].ToString(),
                            Sept = dsProgram.Tables[0].Rows[i]["Sept"].ToString(),
                            Oct = dsProgram.Tables[0].Rows[i]["Oct"].ToString(),
                            Nov = dsProgram.Tables[0].Rows[i]["Nov"].ToString(),
                            Dec = dsProgram.Tables[0].Rows[i]["Dece"].ToString(),
                            Status = dsProgram.Tables[0].Rows[i]["stat"].ToString(),
                            year = dsProgram.Tables[0].Rows[i]["calyear"].ToString(),
                            remark = dsProgram.Tables[0].Rows[i]["remark"].ToString(),
                            //Planned_Date = dsProgram.Tables[0].Rows[i]["planned_date"].ToString(),
                            //Logged_id = dsProgram.Tables[0].Rows[i]["logged_id"].ToString(),


                        };
                        ViewBag.Topics = objGlobaldata.GetDropdownList("Training Topic");
                        objAccessList.TrainingProgramList.Add(objAccess);

                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in TrainingProgramListSearch: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingProgramListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }



        [AllowAnonymous]
        public JsonResult UpdateTrainingProgram(string selected, int status)
        {
            TrainingProgram objTrainingList = new TrainingProgram();
            bool IssueNo = false;
            if (selected != null)
            {
                IssueNo = objTrainingList.FunUpdateTrainingProgram(selected, status);
            }

            return Json(IssueNo);
        }


        //public JsonResult Update2TrainingProgram(string zzz)
        //{
        //    TrainingProgram objTrainingList = new TrainingProgram();
        //    bool IssueNo = false;
        //    if (zzz != null)
        //    {
        //        IssueNo = objTrainingList.FunUpdate2TrainingProgram(zzz);
        //    }

        //    return Json(IssueNo);
        //}


        public ActionResult TrainingProgramAdd()
       {
           //UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
           ViewBag.Topics = objGlobaldata.GetDropdownList("Training Topic");
          return View();


        }


        public ActionResult TrainingProgramEdit()
        {
            TrainingProgram objTrainingP = new TrainingProgram();
            try
            {
                //UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                ViewBag.Topics = objGlobaldata.GetDropdownList("Training Topic");
                if (Request.QueryString["Id_Training_program"] != null && Request.QueryString["Id_Training_program"] != "")
                {
                    string sId_Training_program = Request.QueryString["Id_Training_program"];

                    string sSqlstmt = "select Id_Training_program,topic_id,remark"
                    + " from t_training_program where Id_Training_program='" + sId_Training_program + "'";

                    DataSet dsTrainingProgramList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsTrainingProgramList.Tables.Count > 0 && dsTrainingProgramList.Tables[0].Rows.Count > 0)
                    {

                        objTrainingP = new TrainingProgram
                        {
                            Id_Training_program = (dsTrainingProgramList.Tables[0].Rows[0]["Id_Training_program"].ToString()),
                            topic_id = (dsTrainingProgramList.Tables[0].Rows[0]["topic_id"].ToString()),
                            remark = (dsTrainingProgramList.Tables[0].Rows[0]["remark"].ToString()),

                        };
                    }
                    else
                    {

                        TempData["alertdata"] = "Id_Training_program cannot be Null or empty";
                        return RedirectToAction("TrainingProgramList");
                    }
                }

                else
                {
                    TempData["alertdata"] = "Id_Training_program cannot be Null or empty";
                    return RedirectToAction("TrainingProgramList");
                }
              }
           
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingProgramEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("TrainingProgramList");
            }
            return View(objTrainingP);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrainingProgramEdit(TrainingProgram objProg, FormCollection form)
        {
            try
            {
                objProg.topic_id = form["Training_Topic"];
                objProg.remark = form["remark"];

               
                if (objProg.FunUpdateTrainingProgram(objProg))
                {
                    TempData["Successdata"] = "Training Program Planner updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingProgramEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("TrainingProgramList");
            }

            return RedirectToAction("TrainingProgramList");
        }
                          
        [HttpPost]       
        public ActionResult TrainingProgramAdd(TrainingProgram objTrainingProg, FormCollection form)
        {
            try
           {
                // UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                DateTime dateValue;
                if (DateTime.TryParse(form["Planned_Date"], out dateValue) == true)
                {
                    objTrainingProg.Planned_Date = dateValue;
                }


                objTrainingProg.year = form["year"];

                objTrainingProg.topic_id = form["Training_Topic"];
                objTrainingProg.Jan = form["Jan"];
                objTrainingProg.Feb = form["Feb"];
                objTrainingProg.Mar = form["Mar"];
                objTrainingProg.Apr = form["Apr"];
                objTrainingProg.May = form["May"];
                objTrainingProg.June = form["June"];
                objTrainingProg.Jul = form["Jul"];
                objTrainingProg.Aug = form["Aug"];
                objTrainingProg.Sept = form["Sept"];
                objTrainingProg.Oct = form["Oct"];
                objTrainingProg.Nov = form["Nov"];
                objTrainingProg.Dec = form["Dec"];
                objTrainingProg.remark = form["remark"];

                if (objTrainingProg.Jan=="Jan")
            {
                objTrainingProg.Jan = "1";

            }
                else
                {
                    objTrainingProg.Jan = "0";
                }
                if (objTrainingProg.Feb == "Feb")
                {
                    objTrainingProg.Feb = "1";

                }
                else
                {
                    objTrainingProg.Feb = "0";
                }



                if (objTrainingProg.Mar == "Mar")
                {
                    objTrainingProg.Mar = "1";

                }
                else
                {
                    objTrainingProg.Mar = "0";
                }



                if (objTrainingProg.Apr == "Apr")
                {
                    objTrainingProg.Apr = "1";

                }
                else
                {
                    objTrainingProg.Apr = "0";
                }



                if (objTrainingProg.May == "May")
                {
                    objTrainingProg.May = "1";

                }
                else
                {
                    objTrainingProg.May = "0";
                }




                if (objTrainingProg.June == "June")
                {
                    objTrainingProg.June = "1";

                }
                else
                {
                    objTrainingProg.June = "0";
                }


                if (objTrainingProg.Jul == "Jul")
                {
                    objTrainingProg.Jul = "1";

                }
                else
                {
                    objTrainingProg.Jul = "0";
                }
                if (objTrainingProg.Aug == "Aug")
                {
                    objTrainingProg.Aug = "1";

                }
                else
                {
                    objTrainingProg.Aug = "0";
                }

                if (objTrainingProg.Sept == "Sept")
                {
                    objTrainingProg.Sept = "1";

                }
                else
                {
                    objTrainingProg.Sept = "0";
                }

                if (objTrainingProg.Oct == "Oct")
                {
                    objTrainingProg.Oct = "1";

                }
                else
                {
                    objTrainingProg.Oct = "0";
                }


                if (objTrainingProg.Nov == "Nov")
                {
                    objTrainingProg.Nov = "1";

                }
                else
                {
                    objTrainingProg.Nov = "0";
                }
                if (objTrainingProg.Dec == "Dec")
                {
                    objTrainingProg.Dec = "1";

                }
                else
                {
                    objTrainingProg.Dec = "0";
                }



           if (objTrainingProg.FunAddProgram(objTrainingProg))
            {
                TempData["Successdata"] = "Added Training Program details successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
        }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainingProgram: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }





            //return View();
            return RedirectToAction("TrainingProgramList");

        }
        public JsonResult TrainingProgramDelete(FormCollection form)
        {
            try
            {
               
                    if (form["Id_Training_Program"] != null && form["Id_Training_Program"] != "")
                    {

                        TrainingProgram Doc = new TrainingProgram();
                        string sid_Id_Training_Program = form["Id_Training_Program"];


                        if (Doc.FunDeleteTrainingProgram(sid_Id_Training_Program))
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
                        TempData["alertdata"] = "sid_Id_Training_Program  cannot be Null or empty";
                        return Json("Failed");
                    }
               

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }



    }



}
