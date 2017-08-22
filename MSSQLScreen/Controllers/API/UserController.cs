using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MSSQLScreen.Models;

namespace MSSQLScreen.Controllers.API
{
    public class UserController : ApiController
    {
        private ApplicationDbContext _context;

        public UserController()
        {
            _context = new ApplicationDbContext();
        }

        [APIAuthorize]
        [HttpPost]
        public IHttpActionResult ManageAdmin(UserAccount admin)
        {
            var usr = _context.UserAccounts.SingleOrDefault(c => c.Username == admin.Username);
            if (ModelState.IsValid)
            {
                if (usr == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                else
                {
                    usr.Privilege = admin.Privilege;
                    _context.SaveChanges();
                }
            }
            ModelState.Remove("Password");
            return Ok();
        }
    }
}
