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
        [Route("api/resource/{server_id}")]
        public IHttpActionResult GetResources(int server_id)
        {
            var resourcesInDb = _context.ResourceUsages.SingleOrDefault(c => c.ServerListId == server_id);
            return Ok(resourcesInDb);
        }
    }
}
