using MSSQLScreen.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Http;

namespace MSSQLScreen.Controllers.API
{
    public class ServerController : ApiController
    {
        private ApplicationDbContext _context;

        public ServerController()
        {
            _context = new ApplicationDbContext();
        }

        private bool ValidateConnection(string IP, string userId, string password)
        {
            SqlConnection sql = new SqlConnection("server=" + IP + ";" + "user id=" + userId + ";" + "password=" + password + ";");
            using (sql)
            {
                try
                {
                    sql.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        [APIAuthorize]
        [HttpGet]
        [Route("api/server/{admin_id}")]
        public IEnumerable<ServerList> ServerList(string admin_id)
        {
            int adminId = Int32.Parse(admin_id);
            var serverlist = _context.ServerLists.Where(c => c.AdminAccountId == adminId).ToList();
            return serverlist;
        }

        [APIAuthorize]
        [HttpPost]
        [Route("api/server/connect")]
        public IHttpActionResult Connect(AddServerViewModel server)
        {
            if (ModelState.IsValid)
            {
                if (ValidateConnection(server.IPAddress, server.UserId, server.Password))
                {
                    var identity = (ClaimsIdentity)User.Identity;
                    int adminId = Int32.Parse(((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == "AdminId").Value);
                    var getadmin = _context.AdminAccounts.SingleOrDefault(c => c.Id == adminId);
                    var getserver = _context.ServerLists.SingleOrDefault(c => c.IPAddress == server.IPAddress);
                    if (getserver != null)
                    {
                        getserver.UserId = server.UserId;
                        getserver.Password = server.Password;
                        _context.SaveChanges();
                        return StatusCode(HttpStatusCode.Accepted);
                    }
                    else
                    {
                        var addserver = new ServerList
                        {
                            IPAddress = server.IPAddress,
                            UserId = server.UserId,
                            Password = server.Password,
                            AdminAccountId = getadmin.Id
                        };

                        _context.ServerLists.Add(addserver);
                        _context.SaveChanges();
                        return StatusCode(HttpStatusCode.Accepted);
                    }

                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.NotAcceptable);
                }

            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        [APIAuthorize]
        [HttpDelete]
        [Route("api/server/remove/{server_id}/{admin_id}")]
        public void Remove(int server_id, int admin_id)
        {
            var getserver = _context.ServerLists.Single(c => c.Id == server_id && c.AdminAccountId == admin_id);
            _context.ServerLists.Remove(getserver);
            _context.SaveChanges();
        }
    }
}
