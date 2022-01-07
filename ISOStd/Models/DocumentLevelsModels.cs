using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
namespace ISOStd.Models
{
    public class DocumentLevelsModels
    {

        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "Id")]
        public string id_doc_level { get; set; }

        [Required]
        [Display(Name = "Document Level")]
        [System.Web.Mvc.Remote("doesDocNameExist", "DocumentLevels", HttpMethod = "POST", ErrorMessage = "Document Level already exists. Please enter a different name.")]     
        public string Document_Level { get; set; }

        [Required]
        [Display(Name = "Id")]
        public string id_levels_details { get; set; }

        [Required]
        [Display(Name = "Level Details")]
        public string description { get; set; }

        internal bool CheckForDocNameExists(string sDocName)
        {
            try
            {
                DataSet dsDoc = objGlobalData.Getdetails("select Document_Level from t_document_levels where Document_Level='" + sDocName + "' and active=1");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in DocLevelCheckForDocNameExists: " + ex.ToString());
            }
            return true;
        }
         
        internal bool FunDeleteDocLevelType(string sTypeID)
        {
            try
            {
                string sSqlstmt = "Update t_document_levels set active=0 where id_doc_level=" + sTypeID;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDocLevelType: " + ex.ToString());
            }
            return false;
        }
        
        internal bool FunUpdateDocLevelType(DocumentLevelsModels obj)
        {
            try
            {
                string sSqlstmt = "update t_document_levels set Document_Level ='" + obj.Document_Level +"' where id_doc_level='" + obj.id_doc_level + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDocLevelTypeUpdate: " + ex.ToString());
            }
            return false;
        }
 
        internal bool FunAddDocLevelType(DocumentLevelsModels objDocLevelType)
        {
            try
            {
                string sSqlstmt = "insert into t_document_levels (Document_Level,loggedby)" +
                    " values('" + objDocLevelType.Document_Level + "','" + objGlobalData.GetCurrentUserSession().empid + "')";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDocLevelType: " + ex.ToString());
            }
            return false;
        }

        public DataSet GetDocLevelListbox()
        {
            DataSet dsLevel = new DataSet();
            try
            {
                dsLevel = objGlobalData.Getdetails("select id_doc_level,Document_Level from t_document_levels where active = 1");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDocLevelListbox: " + ex.ToString());
            }
            return dsLevel;
        }

        // Document Level Details

        public MultiSelectList GetDocLeveldetailsListbox(string sId)
        {
            DocumentLevelsModelsList DocumentLevelsModelsList = new DocumentLevelsModelsList();
            DocumentLevelsModelsList.LevelList = new List<DocumentLevelsModels>();
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select id_levels_details,description from t_document_levels_details where id_doc_level='" + sId + "' and active=1");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DocumentLevelsModels emp = new DocumentLevelsModels()
                        {
                            id_doc_level = dsEmp.Tables[0].Rows[i]["id_levels_details"].ToString(),
                            description = dsEmp.Tables[0].Rows[i]["description"].ToString()
                        };

                        DocumentLevelsModelsList.LevelList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDocumentDetailsListbox: " + ex.ToString());
            }
            return new MultiSelectList(DocumentLevelsModelsList.LevelList, "id_doc_level", "description");
        }

        internal bool FunAddDocLevelDetails(DocumentLevelsModels objDocLevel)
        {
            try
            {
                string sSqlstmt = "insert into t_document_levels_details (id_doc_level, description,loggedby) values('" + objDocLevel.id_doc_level + "','" + objDocLevel.description + "','" + objGlobalData.GetCurrentUserSession().empid + "')";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDocLevelDetails: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateDocLevelDetails(DocumentLevelsModels objDocLevel)
        {
            try
            {
                string sSqlstmt = "update t_document_levels_details set description='" + objDocLevel.description + "' where id_levels_details='" + objDocLevel.id_levels_details + "' ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDocLevelDetails: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteDocLevelDetails(DocumentLevelsModels objDocLevel)
        {
            try
            {
                string sSqlstmt = "update t_document_levels_details set active=0 where id_levels_details='" + objDocLevel.id_levels_details + "' ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDocLevelDetails: " + ex.ToString());
            }
            return false;
        }

    }

    public class DocTypeDetails
    {
        public string id { get; set; }
        public string Document_Level { get; set; }
    }

    public class DocumentLevelsModelsList
    {
        public List<DocumentLevelsModels> LevelList { get; set; }
    } 
}

