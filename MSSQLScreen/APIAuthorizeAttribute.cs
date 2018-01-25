using System.Web;
using System.Web.Http.Controllers;
using System.Net.Http;

namespace MSSQLScreen
{
    public class APIAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                base.HandleUnauthorizedRequest(actionContext);
            else
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Privilege level must be SUPERADMIN")
                };
        }
    }
}