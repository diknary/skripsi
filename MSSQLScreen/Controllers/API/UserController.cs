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
        [HttpGet]
        [Route("api/admin/adminlist")]
        public IEnumerable<AdminAccount> UserList()
        {
            var userInDb = _context.AdminAccounts.ToList();
            return userInDb;
        }

        [APIAuthorize]
        [HttpGet]
        [Route("api/admin/{admin_id}")]
        public IEnumerable<AdminLog> LoginHistory(int id)
        {
            return _context.AdminLogs.Where(c => c.AdminAccountId == id).ToList();
        }

        [APIAuthorize]
        [HttpPost]
        [Route("api/admin/addadmin")]
        public void AddAdmin(AddAdminViewModel admin)
        {
            var usr = _context.UserAccounts.SingleOrDefault(c => c.Username == admin.Username);
            var adm = _context.AdminAccounts.SingleOrDefault(c => c.Username == admin.Username);
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
                    Name = usr.Name,
                    NIP = usr.NIP,
                    Username = usr.Username,
                    Password = usr.Password,
                    Privilege = "ADMIN"
                };
                _context.AdminAccounts.Add(addadmin);
                _context.SaveChanges();
            }

        }

        [APIAuthorize]
        [HttpPut]
        [Route("api/disconnect/{admin_id}")]
        public void Disconnect(int admin_id)
        {
            var admin = _context.AdminAccounts.Single(c => c.Id == admin_id);
            admin.IsOnline = 0;
            _context.SaveChanges();
        }

        [APIAuthorize]
        [HttpPut]
        [Route("api/admin/{admin_id}/{token}")]
        public void Disconnect(int admin_id, string token)
        {
            var admin = _context.AdminAccounts.Single(c => c.Id == admin_id);
            admin.FirebaseToken = token;
            _context.SaveChanges();
        }
    }
}
