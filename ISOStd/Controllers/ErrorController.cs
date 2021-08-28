using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
       
        public ActionResult Index()
        {
            return View();
        }
         
        public ActionResult FileNotFound()
        {
            //ViewBag.FIleNotFound="Sorry requested file not found";

            return View();
        }

    }
}
