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
            var serverlist = _context.ServerLists.ToList();
            return View(serverlist);
        }
    }
}