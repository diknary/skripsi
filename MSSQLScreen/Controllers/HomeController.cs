using MSSQLScreen.Models;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace MSSQLScreen.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }


        [WebAuthorize]
        public ActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claims = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var getadmin = _context.AdminAccounts.Single(c => c.Username == claims);
            var serverlist = _context.ServerLists.Where(c => c.AdminAccountId == getadmin.Id).ToList();
            return View(serverlist);
        }
    }
}