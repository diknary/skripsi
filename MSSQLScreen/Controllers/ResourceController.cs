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
        [Route("home/cpuMemoryUsage")]
        public ActionResult CPUMemoryUsage()
        {
            //var resourceInDB = _context.ResourceUsages.SingleOrDefault();

            //PerformanceCounter CPUcounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            //PerformanceCounter MemoryCounter = new PerformanceCounter("Memory", "Available MBytes");

            //if(resourceInDB == null)
            //{ 
            //    var usage = new ResourceUsage
            //    {
            //        ProcessorUsage = CPUcounter.NextValue(),
            //        AvailableMemory = MemoryCounter.NextValue()
            //    };
            //    _context.ResourceUsages.Add(usage);
            //    _context.SaveChanges();
            //}
            //else
            //{
            //    resourceInDB.ProcessorUsage = CPUcounter.NextValue();
            //    resourceInDB.AvailableMemory = MemoryCounter.NextValue();
            //    _context.SaveChanges();
            //}

            //Response.AddHeader("Refresh", "2");
            var resourceInDb = _context.ResourceUsages.SingleOrDefault();
            return View(resourceInDb);

        }
    }
}