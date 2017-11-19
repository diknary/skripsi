using MSSQLScreen.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MSSQLScreen
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private ApplicationDbContext _context;

        public MvcApplication()
        {
            _context = new ApplicationDbContext();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }

        protected void Session_End()
        {
            var identity = (ClaimsIdentity)User.Identity;
            string username = Convert.ToString(identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier));

        }

    }
}
