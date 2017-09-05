using System.Linq;
using System.Web.Http;
using MSSQLScreen.Models;

namespace MSSQLScreen.Controllers.API
{
    public class ResourceController : ApiController
    {
        private ApplicationDbContext _context;

        public ResourceController()
        {
            _context = new ApplicationDbContext();
        }

        [APIAuthorize]
        [HttpGet]
        [Route("api/resource")]
        public IHttpActionResult GetResources()
        {
            var resourcesInDb = _context.ResourceUsages.SingleOrDefault();
            return Ok(resourcesInDb);
        }
    }
}
