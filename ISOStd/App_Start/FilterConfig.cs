using ISOStd.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace ISOStd
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CheckSessionOutAttribute());
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckSessionOutAttribute : ActionFilterAttribute
    {
        private clsGlobal objglobal = new clsGlobal();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower().Trim();
            string actionName = filterContext.ActionDescriptor.ActionName.ToLower().Trim();

            if (!actionName.StartsWith("login") && !actionName.StartsWith("logoff") && !actionName.StartsWith("forgotpassword"))
            {
                var session = HttpContext.Current.Session["UserCredentials"];
                HttpContext ctx = HttpContext.Current;
                //Redirects user to login screen if session has timed out
                if (session == null)
                {
                    objglobal.ClearUserSession();
                    string redirectTo = "~/Account/Login";
                    base.OnActionExecuting(filterContext);
                    redirectTo = string.Format("~/Account/Login?ReturnUrl={0}", HttpUtility.UrlEncode(ctx.Request.RawUrl));
                    filterContext.Result = new RedirectResult(redirectTo);
                    return;
                }
            }
        }
    }
}