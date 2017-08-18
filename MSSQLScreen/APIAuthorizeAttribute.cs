using System.Web;
using System.Web.Http.Controllers;

namespace MSSQLScreen
{
    public class APIAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                base.HandleUnauthorizedRequest(actionContext);
        }
    }
}