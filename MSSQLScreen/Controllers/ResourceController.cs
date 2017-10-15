using System.Linq;
using System.Web.Mvc;
using MSSQLScreen.Models;

namespace MSSQLScreen.Controllers
{
    public class ResourceController : Controller
    {
        private ApplicationDbContext _context;

        public ResourceController()
        {
            _context = new ApplicationDbContext();
        }

        [WebAuthorize]
        public ActionResult CPUMemoryUsage()
        {
            return View();
        }
    }
}