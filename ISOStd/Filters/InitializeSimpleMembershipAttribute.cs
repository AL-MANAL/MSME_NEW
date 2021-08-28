using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using ISOStd.Models;
using System.Web.Routing;
using System.Web;

namespace ISOStd.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        //private static SimpleMembershipInitializer _initializer;
        //private static object _initializerLock = new object();
        //private static bool _isInitialized;

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    // Ensure ASP.NET Simple Membership is initialized only once per app start
        //   // LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        //}

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<UsersContext>(null);

                try
                {
                    using (var context = new UsersContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }


    public class CusAuthenticationAttribute : AuthorizeAttribute
    {
        //public string[] AllowRoles { get; set; }
        public string HandleErrorPath { get; set; }
        //public IUserRepository userRepository = new UserRepository();

        private readonly string[] allowedroles;
        public CusAuthenticationAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            clsGlobal objGlobaldata = new clsGlobal();


            var userroles = objGlobaldata.GetMultiRolesNameById(objGlobaldata.GetCurrentUserSession().role);

            var user = (ISOStd.Models.UserCredentials)HttpContext.Current.Session["UserCredentials"];
            //var userroles = "6";/*userRepository.GetUserRoleByUserName(user).Split(',');*/

            foreach (var role in allowedroles)
            {
                if (userroles.Contains(role))
                //if (role == userroles)
                {
                    return true;
                }
            }


            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.Controller.ViewData["AuthorizationError"] = "You don't have rights to take actions";
            filterContext.Result = /*new RedirectResult("~/Account/AccessDenied");*/
                new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "Home" },
                    { "action", "AccessDenied" }
               });
            return;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PreventFromUrl : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer == null ||
     filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
            {
                filterContext.Result = new RedirectToRouteResult(new
                                          RouteValueDictionary(new { controller = "Home", action = "AccessDenied", area = "" }));
            }
        }
    }
}

