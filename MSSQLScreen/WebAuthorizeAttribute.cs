using System.Web.Mvc;
using System.Web.Routing;

namespace MSSQLScreen
{
    public class WebAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "User", action = "Login" }));

        }
    }
}