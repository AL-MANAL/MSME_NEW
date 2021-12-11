using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ISOStd
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        static ILog logger = LogManager.GetLogger("MvcApplication");
        protected void Application_Start()
        {
            //var path = new DirectoryInfo(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            Directory.SetCurrentDirectory(AssemblyLoadDirectory);
            FileInfo fileInfo = new FileInfo(Path.Combine( AssemblyLoadDirectory, "log4net.config"));


            log4net.Config.XmlConfigurator.ConfigureAndWatch(fileInfo);
            //Log.Debug("Application loaded.");
            logger.Info(fileInfo.FullName);

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        static public string AssemblyLoadDirectory
        {
            get
            {
                string codeBase = Assembly.GetCallingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

    }
}