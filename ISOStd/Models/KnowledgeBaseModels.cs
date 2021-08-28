using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Web.Mvc.Html;
namespace ISOStd.Models
{
    public class KnowledgeBaseModels
    {

        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Knowledge Base Reference Number")]
        public int id_knowledge_base { get; set; }


        [Required]
        [Display(Name = " Topic ")]
        public string topic { get; set; }

        [Required]
        [Display(Name = "Details")]
        public string details { get; set; }


        //[Required]
        [Display(Name = "Attachment")]
        public string Evidence { get; set; }

        

        //[Required]
        [Display(Name = "Knowledge Uploaded By")]
        public string uploaded_by { get; set; }

        internal bool FunAddKnowledge(KnowledgeBaseModels objKnowledgeModel)
        {

            try
            {
                string sSqlstmt = "insert into t_knowledge_base (topic,  details ,uploaded_by ";

                if (objKnowledgeModel.Evidence != null && objKnowledgeModel.Evidence != "")
                {
                    sSqlstmt = sSqlstmt + ", Evidence";
                }


                sSqlstmt = sSqlstmt + ") values('" + objKnowledgeModel.topic
                     + "','" + objKnowledgeModel.details + "','" + objKnowledgeModel.uploaded_by + "'";

                if (objKnowledgeModel.Evidence != null && objKnowledgeModel.Evidence != "")
                {
                    sSqlstmt = sSqlstmt + ", '" + objKnowledgeModel.Evidence + "'";
                }
                sSqlstmt = sSqlstmt + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }

            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddKnowledge: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateKnowledgeBase(KnowledgeBaseModels objKnowledgeBaseModels)
        {
            try
            {

                string sSqlstmt = "update t_knowledge_base set topic ='" + objKnowledgeBaseModels.topic
                    + "', details='" + objKnowledgeBaseModels.details + "'";


                if (objKnowledgeBaseModels.Evidence != null && objKnowledgeBaseModels.Evidence != "")
                {
                    sSqlstmt = sSqlstmt + ", Evidence='" + objKnowledgeBaseModels.Evidence + "'";
                }
                sSqlstmt = sSqlstmt + " where id_knowledge_base='" + objKnowledgeBaseModels.id_knowledge_base + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateKnowledgeBase: " + ex.ToString());
            }
            return false;
        }
    }
    public class KnowledgeModelsList
    {
        public List<KnowledgeBaseModels> KnowledgeMList { get; set; }
    }
}