using MSSQLScreen.Models;
using System;
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
            int adminId = Int32.Parse(((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == "AdminId").Value);
            var serverlist = _context.ServerLists.Where(c => c.AdminAccountId == adminId).ToList();
            return View(serverlist);
        }
    }
}