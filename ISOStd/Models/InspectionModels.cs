using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;


namespace ISOStd.Models
{

    public class InspectionCategoryModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        public string id_inspection_question { get; set; }
        public string Category { get; set; }

        public string GetInspectionCategoryByID(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Inspection Category' and (item_id='" + item_id + "' or item_desc='" + item_id + "')";
                    DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionCategoryByID: " + ex.ToString());
            }
            return "";
        }

        public string getInspectionCategoryIDByName(string item_id)
        {
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                   + "and header_desc='Inspection Category' and item_desc='" + item_id + "'";
                DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Id"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in getInspectionCategoryIDByName: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetInspectionCategoryList()
        {
            DropdownInspectionList objReportList = new DropdownInspectionList();
            objReportList.lstDropdown = new List<DropdownInspectionItems>();
            try
            {
                string sSsqlstmt = "select distinct category as Id,item_desc as Name from  t_inspection_section t,dropdownitems tt where tt.item_id=t.category";

                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownInspectionItems objReport = new DropdownInspectionItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionCategoryList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public MultiSelectList GetInspectionCategoryListbox()
        {
            DropdownInspectionList objReportList = new DropdownInspectionList();
            objReportList.lstDropdown = new List<DropdownInspectionItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Inspection Category' order by item_desc asc";

                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownInspectionItems objReport = new DropdownInspectionItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionCategoryListbox: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public MultiSelectList GetInspectionSectionListbox(string sCategory, string sBranch)
        {
            DropdownInspectionList objReportList = new DropdownInspectionList();
            objReportList.lstDropdown = new List<DropdownInspectionItems>();
            try
            {
                string sSsqlstmt = "select id_inspection as Id, Section as Name from t_inspection_section where Category='" + sCategory + "' and  branch='" + sBranch + "' and active=1";

                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownInspectionItems objReport = new DropdownInspectionItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionSectionListbox: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public MultiSelectList GetInspSectionRespPersonList(string sCategory, string sBranch)
        {
            DropdownInspectionList objReportList = new DropdownInspectionList();
            objReportList.lstDropdown = new List<DropdownInspectionItems>();
            try
            {
                string sSsqlstmt = "select id_inspection as Id, Section as Name,Resp_person as Resp_person from t_inspection_section where Category='" + sCategory + "' and  branch='" + sBranch + "' and active=1";

                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownInspectionItems objReport = new DropdownInspectionItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString() + "_" + dsReportType.Tables[0].Rows[i]["Resp_person"].ToString() + "_" + objGlobalData.GetMultiHrEmpNameById(dsReportType.Tables[0].Rows[i]["Resp_person"].ToString())
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspSectionRespPersonList: " + ex.ToString());
            }

            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetSectionNamebyId(string Section_id)
        {
            try
            {
                string sSsqlstmt = "select Section as Name from t_inspection_section where id_inspection='" + Section_id + "'";
                DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSectionNamebyId: " + ex.ToString());
            }
            return "";
        }

        //public MultiSelectList GetInspRespPersonList(string sCategory, string sBranch)
        //{
        //    DropdownInspectionList objReportList = new DropdownInspectionList();
        //    objReportList.lstDropdown = new List<DropdownInspectionItems>();
        //    try
        //    {
        //        string sSsqlstmt = "select id_inspection as Id, Resp_person as Name from t_inspection_section where Category='" + sCategory + "' and  branch='" + sBranch + "'";

        //        DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

        //        if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
        //            {
        //                DropdownInspectionItems objReport = new DropdownInspectionItems()
        //                {
        //                    Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
        //                    Name = dsReportType.Tables[0].Rows[i]["Name"].ToString() 
        //                };
        //                objReportList.lstDropdown.Add(objReport);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in GetInspRespPersonList: " + ex.ToString());
        //    }

        //    return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        //}

        //public string newfucntion(string sCategory, string sBranch)
        //{
        //    string name = null;
        //    try
        //    {
        //        string sSsqlstmt = "select Resp_person as Resp_person from t_inspection_section where Category='" + sCategory + "' and  branch='" + sBranch + "'";

        //        DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

        //        if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
        //            {
        //                name = objGlobalData.GetMultiHrEmpNameById(dsReportType.Tables[0].Rows[i]["Resp_person"].ToString());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in GetInspectionSectionListbox: " + ex.ToString());
        //    }

        //    return name;
        //}

    }

    public class InspctionQuestionModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        public string id_inspection_question { get; set; }

        public string id_inspection_rating { get; set; }

        public string id_inspection { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Section")]
        public string Section { get; set; }

        [Display(Name = "Questions")]
        public string InspectionQuestions { get; set; }

        [Display(Name = "Ratings")]
        public string inspection_rating { get; set; }

        [Display(Name = "Responsible Person")]
        public string Resp_person { get; set; }

        public MultiSelectList GetInspectionQuestionsListbox(string sCategory)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where Category='" + sCategory + "' and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            InspectionQuestions = dsQuest.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditElementsListbox: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }

        public MultiSelectList GetInspectionQuestionsList(string sCategory, string sSection)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where Category='" + sCategory + "' and Section='" + sSection + "'and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            InspectionQuestions = dsQuest.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionQuestionsList: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }

        public MultiSelectList GetInspectionQuestionsListboxWithBranch(string sbranch)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where branch='" + sbranch + "' and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            InspectionQuestions = dsQuest.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditElementsListbox: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }

        public MultiSelectList GetInspectionQuestionsListbox(string sCategory, string branchId)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where Category='" + sCategory + "' and branch='" + branchId + "' and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            InspectionQuestions = dsQuest.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionQuestionsListbox: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }

        public MultiSelectList GetInspectionQuestList(string sCategory, string sSection, string branchId)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where Category='" + sCategory + "' and Section='" + sSection + "' and branch='" + branchId + "' and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            InspectionQuestions = dsQuest.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionQuestList: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }

        public MultiSelectList GetInspectionQuestions(string sCategory, string branchId)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                string sUser = objGlobalData.GetCurrentUserSession().empid;
                string id_inspection = "";
                string Section_Id = "";
                DataSet dsQuest;
                dsQuest = objGlobalData.Getdetails("select id_inspection from t_inspection_section where Category='" + sCategory +
                    "' and branch='" + branchId + "' and (find_in_set('" + sUser + "', Resp_person) || Resp_person is NULL || Resp_person ='' ) and Active=1");

                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        id_inspection = dsQuest.Tables[0].Rows[i]["id_inspection"].ToString();
                        Section_Id = string.Concat(Section_Id, id_inspection, ",");
                    }
                    Section_Id = Section_Id.Remove(Section_Id.Length - 1, 1);
                }

                if (Section_Id != "")
                {
                    DataSet dsQuest1 = objGlobalData.Getdetails("select id_inspection_question,InspectionQuestions from t_inspection_questions where Section in (" + Section_Id + ") and Active=1");
                    if (dsQuest1.Tables.Count > 0 && dsQuest1.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsQuest1.Tables[0].Rows.Count; i++)
                        {
                            InspctionQuestionModels Qst = new InspctionQuestionModels()
                            {
                                id_inspection_question = dsQuest1.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                                InspectionQuestions = dsQuest1.Tables[0].Rows[i]["InspectionQuestions"].ToString()
                            };

                            InspQst.InspectionQstList.Add(Qst);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionQuestions" + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "InspectionQuestions");
        }


        public MultiSelectList GetSectionQuestions(string sCategory, string branchId)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            InspectionCategoryModels obj = new InspectionCategoryModels();

            try
            {
                string sUser = objGlobalData.GetCurrentUserSession().empid;
                string id_inspection = "";
                string Section_Id = "";
                DataSet dsQuest;

                dsQuest = objGlobalData.Getdetails("select id_inspection from t_inspection_section where Category='" + sCategory +
                    "' and branch='" + branchId + "' and (find_in_set('" + sUser + "', Resp_person) ||  Resp_person is NULL || Resp_person ='') and Active=1");

                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        id_inspection = dsQuest.Tables[0].Rows[i]["id_inspection"].ToString();
                        Section_Id = string.Concat(Section_Id, id_inspection, ",");
                    }
                    Section_Id = Section_Id.Remove(Section_Id.Length - 1, 1);
                }
                if(Section_Id != "" )
                {                
                DataSet dsQuest1 = objGlobalData.Getdetails("select id_inspection_question,Section from t_inspection_questions where Section in (" + Section_Id + ") and Active=1 order by Section asc");
                if (dsQuest1.Tables.Count > 0 && dsQuest1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest1.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_question = dsQuest1.Tables[0].Rows[i]["id_inspection_question"].ToString(),
                            Section = obj.GetSectionNamebyId(dsQuest1.Tables[0].Rows[i]["Section"].ToString())
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSectionQuestions: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_question", "Section");
        }


        internal bool FunDeleteQuestions(string sid_inspection_question)
        {
            try
            {
                string sSqlstmt = "update t_inspection_questions set Active=0 where id_inspection_question='" + sid_inspection_question + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteQuestions: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInspectionQuestions(int id_inspection_question, string InspectionQuestions)
        {
            try
            {
                string sSqlstmt = "update t_inspection_questions set InspectionQuestions='" + InspectionQuestions + "' where id_inspection_question='" + id_inspection_question + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInspectionQuestions: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddInspectionQuestions(InspctionQuestionModels objInspModels)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_inspection_questions (InspectionQuestions,Category,Section,branch) values('" + objInspModels.InspectionQuestions + "','" + objInspModels.Category + "','" + objInspModels.Section + "','" + sBranch + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspectionQuestions: " + ex.ToString());
            }
            return false;
        }

        public DataSet GetInspectionRating()
        {

            DataSet dsRating = new DataSet();
            try
            {
                dsRating = objGlobalData.Getdetails("select id_inspection_rating, inspection_rating from t_inspection_rating order by id_inspection_rating");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionRating: " + ex.ToString());
            }
            return dsRating;//new MultiSelectList(lstSurvey.lstSurveyRating, "SQ_OptionsId", "RatingOptions");
        }

        public DataSet GetInspectionRating(string Category)
        {

            DataSet dsRating = new DataSet();
            try
            {
                dsRating = objGlobalData.Getdetails("select id_inspection_rating, inspection_rating from t_inspection_rating where Category='" + Category + "' and Active=1 order by id_inspection_rating");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionRating: " + ex.ToString());
            }
            return dsRating;//new MultiSelectList(lstSurvey.lstSurveyRating, "SQ_OptionsId", "RatingOptions");
        }
        //------------------------Rating-------------------------------------

        public MultiSelectList GetInspectionRatingListbox(string sCategory)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection_rating,inspection_rating from t_inspection_rating where Category='" + sCategory + "' and Active=1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection_rating = dsQuest.Tables[0].Rows[i]["id_inspection_rating"].ToString(),
                            inspection_rating = dsQuest.Tables[0].Rows[i]["inspection_rating"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionRatingListbox: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection_rating", "inspection_rating");
        }

        internal bool FunAddInspectionRatings(InspctionQuestionModels objInspModels)
        {
            try
            {
                string sSqlstmt = "insert into t_inspection_rating (inspection_rating,Category) values('" + objInspModels.inspection_rating + "','" + objInspModels.Category + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspectionRatings: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteRatings(string id_inspection_rating)
        {
            try
            {
                string sSqlstmt = "update t_inspection_rating set Active=0 where id_inspection_rating='" + id_inspection_rating + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteRatings: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInspectionRatings(int id_inspection_rating, string inspection_rating)
        {
            try
            {
                string sSqlstmt = "update t_inspection_rating set inspection_rating='" + inspection_rating + "' where id_inspection_rating='" + id_inspection_rating + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInspectionRatings: " + ex.ToString());
            }
            return false;
        }

        //-----------------------Section-----------------------------

        public MultiSelectList GetInspectionSectionListbox(string sCategory, string sbranch)
        {
            InspctionQuestionList InspQst = new InspctionQuestionList();
            InspQst.InspectionQstList = new List<InspctionQuestionModels>();

            try
            {
                DataSet dsQuest = objGlobalData.Getdetails("select id_inspection,Section from t_inspection_section where Category='" + sCategory + "' and branch = '" + sbranch + "' and Active = 1");
                if (dsQuest.Tables.Count > 0 && dsQuest.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest.Tables[0].Rows.Count; i++)
                    {
                        InspctionQuestionModels Qst = new InspctionQuestionModels()
                        {
                            id_inspection = dsQuest.Tables[0].Rows[i]["id_inspection"].ToString(),
                            Section = dsQuest.Tables[0].Rows[i]["Section"].ToString()
                        };

                        InspQst.InspectionQstList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionSectionListbox: " + ex.ToString());
            }
            return new MultiSelectList(InspQst.InspectionQstList, "id_inspection", "Section");
        }

        internal bool FunAddInspectionSection(InspctionQuestionModels objInspModels)
        {
            try
            {
                string sBranch_name = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_inspection_section (Category,Section,Resp_person,branch) values('" + objInspModels.Category + "','" + objInspModels.Section + "','" + objInspModels.Resp_person + "','" + sBranch_name + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspectionSection: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteSection(string sid_inspection)
        {
            try
            {
                string sSqlstmt = "update t_inspection_section set active=0 where id_inspection='" + sid_inspection + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSection: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInspectionSection(int sid_inspection, string sSection, string sResp_person)
        {
            try
            {
                string sSqlstmt = "update t_inspection_section set Section='" + sSection + "' , Resp_person = '" + sResp_person + "' where id_inspection='" + sid_inspection + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInspectionSection: " + ex.ToString());
            }
            return false;
        }


    }

    public class GenerateInspectionChecklistModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "id")]
        public string id_inspection_checklist { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Inspection Date")]
        public DateTime Inspection_date { get; set; }

        [Display(Name = "Inspection By")]
        public string Inspection_by { get; set; }

        [Display(Name = "Location/Site")]
        public string Location { get; set; }

        [Display(Name = "Project")]
        public string Project { get; set; }

        [Display(Name = "Tag No")]
        public string TagNo { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        public string id_inspection { get; set; }
        public string cat_id { get; set; }

        public string id_inspection_question { get; set; }
        public string Section { get; set; }
        public string InspectionQuestions { get; set; }
        public List<GenerateInspectionChecklistModels> InspChk { get; set; }

        internal bool FunAddInspectionChecklist(GenerateInspectionChecklistModels objInspChecklist, InspectionChecklistList objInsp)
        {
            InspectionChecklistModels obj = new InspectionChecklistModels();
            string Inspection_date = objInspChecklist.Inspection_date.ToString("yyyy-MM-dd HH':'mm':'ss");
           // string sBranch = objGlobalData.GetCurrentUserSession().division;

            string sSqlstmt = "insert into t_inspection_checklist (Category,Inspection_date,Inspection_by,Location,Project,TagNo,Comment,branch,Department) values('" + objInspChecklist.Category + "',"
          + "'" + Inspection_date + "','" + objInspChecklist.Inspection_by + "','" + objInspChecklist.Location + "','" + objInspChecklist.Project + "','" + objInspChecklist.TagNo + "','" + objInspChecklist.Comment + "','" + objInspChecklist.branch + "','" + objInspChecklist.Department + "')";

            return obj.FunAddInspectionChecklistDetails(objInsp, objGlobalData.ExecuteQueryReturnId(sSqlstmt));

        }

        internal bool FunUpdateInspectionChecklist(GenerateInspectionChecklistModels objInspChecklist, InspectionChecklistList objInsp)
        {
            try
            {
                string Inspection_date = objInspChecklist.Inspection_date.ToString("yyyy-MM-dd HH':'mm':'ss");


                string sSqlstmt = "update t_inspection_checklist set Category='" + objInspChecklist.Category + "', Inspection_by='" + objInspChecklist.Inspection_by
                   + "', Location='" + objInspChecklist.Location + "', Project='" + objInspChecklist.Project + "', TagNo='" + objInspChecklist.TagNo + "', Comment='" + objInspChecklist.Comment
                   + "', Department='" + objInspChecklist.Department + "', branch='" + objInspChecklist.branch + "'";

                if (objInspChecklist.Inspection_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Inspection_date='" + Inspection_date + "' ";
                }
                sSqlstmt = sSqlstmt + " where id_inspection_checklist='" + objInspChecklist.id_inspection_checklist + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    InspectionChecklistModels objElement = new InspectionChecklistModels();
                    objInsp.lst[0].id_inspection_checklist = objInspChecklist.id_inspection_checklist;
                    objElement.FunUpdateInspectionChecklistDetails(objInsp);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInspectionChecklist: " + ex.ToString());
            }
            return false;
        }

    }

    public class InspectionChecklistModels
    {
        clsGlobal objGlobalData = new clsGlobal();
        [Display(Name = "id")]
        public string id_inspection_checklist_trans { get; set; }

        [Display(Name = "ID")]
        public string id_inspection_checklist { get; set; }

        [Display(Name = "id")]
        public string id_inspection_question { get; set; }

        [Display(Name = "id")]
        public string id_inspection_rating { get; set; }

        [Display(Name = "Action")]
        public string Action { get; set; }

        [Display(Name = "Action By")]
        public string ActionBy { get; set; }

        [Display(Name = "Upload")]
        public string Upload { get; set; }

        public string id_ques { get; set; }

        internal bool FunAddInspectionChecklistDetails(InspectionChecklistList objInsp, int id_inspection_checklist)
        {
            try
            {
                if (objInsp.lst.Count > 0)
                {

                    string sSqlstmt = "delete from t_inspection_checklist_trans where id_inspection_checklist='"
                        + id_inspection_checklist + "'; ";
                    for (int i = 0; i < objInsp.lst.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_inspection_checklist_trans (id_inspection_checklist, id_inspection_question, id_inspection_rating,Action,ActionBy,Upload"
                        + ") values('" + id_inspection_checklist + "','" + objInsp.lst[i].id_inspection_question
                        + "','" + objInsp.lst[i].id_inspection_rating + "','" + objInsp.lst[i].Action + "','" + objInsp.lst[i].ActionBy + "','" + objInsp.lst[i].Upload + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInspectionChecklistDetails: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateInspectionChecklistDetails(InspectionChecklistList objInsp)
        {
            try
            {
                if (objInsp.lst.Count > 0)
                {
                    string sSqlstmt = "";
                    for (int i = 0; i < objInsp.lst.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "update t_inspection_checklist_trans set id_inspection_question='" + objInsp.lst[i].id_inspection_question + "',"
                            + "id_inspection_rating='" + objInsp.lst[i].id_inspection_rating + "',Action='" + objInsp.lst[i].Action + "',ActionBy='" + objInsp.lst[i].ActionBy + "'";
                        if (objInsp.lst[i].Upload != null)
                        {
                            sSqlstmt = sSqlstmt + ", Upload='" + objInsp.lst[i].Upload + "' ";
                        }
                        sSqlstmt = sSqlstmt + " where id_inspection_checklist_trans='" + objInsp.lst[i].id_inspection_checklist_trans + "';";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInspectionChecklistDetails: " + ex.ToString());
            }
            return false;
        }

        public string GetInspectionQuestionNameById(string sid_inspection_question)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("SELECT InspectionQuestions FROM t_inspection_questions where id_inspection_question='" + sid_inspection_question + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["InspectionQuestions"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionQuestionNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetInspectionRatingNameById(string sid_inspection_rating)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("SELECT inspection_rating FROM t_inspection_rating where id_inspection_rating='" + sid_inspection_rating + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["inspection_rating"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionRatingNameById: " + ex.ToString());
            }
            return "";
        }

        internal bool FunDeleteInspectionChecklist(string sid_inspection_checklist)
        {
            try
            {
                string sSqlstmt = "update t_inspection_checklist set Active=0 where id_inspection_checklist='" + sid_inspection_checklist + "' and Active=1";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteInspectionChecklist: " + ex.ToString());
            }
            return false;
        }
    }

    public class InspectionCategoryList
    {
        public List<InspectionCategoryModels> InspectionLst { get; set; }
    }

    public class InspctionQuestionList
    {
        public List<InspctionQuestionModels> InspectionQstList { get; set; }
    }

    public class GenerateInspectionChecklistList
    {
        public List<GenerateInspectionChecklistModels> lstChk { get; set; }
    }
    public class InspectionChecklistList
    {
        public List<InspectionChecklistModels> lst { get; set; }
    }

    public class DropdownInspectionItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownInspectionList
    {
        public List<DropdownInspectionItems> lstDropdown { get; set; }
    }


}