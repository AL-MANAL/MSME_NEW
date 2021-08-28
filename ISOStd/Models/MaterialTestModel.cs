using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISOStd.Models
{
    public class MaterialTestModel
    {
        public string Material { get; set; }
        public int Passed { get; set; }
        public int Performed { get; set; }
        public int Planned { get; set; }
    }
}