using ISOStd.Filters;
using System.Web.Mvc;

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