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
    public class TrainingPlanModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_training_plan { get; set; }

        [Display(Name = "Training topic ")]
        public string topic { get; set; }

        [Display(Name = "Employee names ")]
        public string emp_id { get; set; }

        [Display(Name = "From Date")]
        public string from_date { get; set; }

        [Display(Name = "To Date")]
        public string to_date { get; set; }

        [Display(Name = "Training source")]
        public string source_id { get; set; }

        [Display(Name = "Trainer Name")]
        public string trainer_name { get; set; }

        [Display(Name = "To be reviewed by")]
        public string review_by { get; set; }

        [Display(Name = "To be approved by")]
        public string approve_by { get; set; }

      

    }
}