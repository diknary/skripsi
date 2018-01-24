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

        [APIAuthorize(Roles = "SUPERADMIN")]
        [HttpGet]
        [Route("api/admin/adminlist")]
        public IEnumerable<AdminAccount> UserList()
        {
            var userInDb = _context.AdminAccounts.ToList();
            return userInDb;
        }

        [APIAuthorize(Roles ="SUPERADMIN")]
        [HttpGet]
        [Route("api/admin/{admin_id}")]
        public IEnumerable<AdminLog> LoginHistory(int admin_id)
        {
            return _context.AdminLogs.Where(c => c.AdminAccountId == admin_id).ToList();
        }

        [APIAuthorize(Roles = "SUPERADMIN")]
        [HttpPost]
        [Route("api/admin/addadmin")]
        public IHttpActionResult AddAdmin(AddAdminViewModel admin)
        {
            var usr = _context.UserAccounts.SingleOrDefault(c => c.Username == admin.Username);
            var adm = _context.AdminAccounts.SingleOrDefault(c => c.UserAccountId == usr.Id);
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            if (usr == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            if (adm != null)
                throw new HttpResponseException(HttpStatusCode.Ambiguous);
            else
            {
                var addadmin = new AdminAccount
                {
                    UserAccountId = usr.Id,
                    Privilege = "ADMIN"
                };
                _context.AdminAccounts.Add(addadmin);
                _context.SaveChanges();
                return Ok();
            }

        }

        [APIAuthorize]
        [HttpPut]
        [Route("api/admin/{admin_id}")]
        public IHttpActionResult FirebaseToken(int admin_id, FirebaseTokenViewModel frbs)
        {
            var admin = _context.AdminAccounts.SingleOrDefault(c => c.Id == admin_id);
            if (admin == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
            {
                admin.FirebaseToken = frbs.Token;
                _context.SaveChanges();
                return Ok();
            }          
        }

        [APIAuthorize]
        [HttpPut]
        [Route("api/login/{admin_id}")]
        public IHttpActionResult IsOnline(int admin_id)
        {
            var admin = _context.AdminAccounts.SingleOrDefault(c => c.Id == admin_id);
            if (admin == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
            {
                admin.IsOnline = 1;
                _context.SaveChanges();
                return Ok();
            }
        }

        [APIAuthorize]
        [HttpPut]
        [Route("api/logout/{admin_id}")]
        public IHttpActionResult IsNotOnline(int admin_id)
        {
            var admin = _context.AdminAccounts.SingleOrDefault(c => c.Id == admin_id);
            if (admin == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
            {
                admin.IsOnline = 0;
                _context.SaveChanges();
                return Ok();
            }
        }
    }
}
