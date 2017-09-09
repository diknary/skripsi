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
        [Route("api/user/userlist")]
        public IEnumerable<UserAccount> UserList()
        {
            var userInDb = _context.UserAccounts.ToList();
            return userInDb;
        }

        [APIAuthorize]
        [HttpPut]
        [Route("api/user/manageadmin")]
        public void ManageAdmin(AddAdminViewModel admin)
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

    }
}
