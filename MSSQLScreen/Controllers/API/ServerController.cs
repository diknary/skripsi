using MSSQLScreen.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Security.Cryptography;
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

        private static byte[] CreateRandomSalt(int length)
        {

            byte[] randBytes;

            if (length >= 1)
            {
                randBytes = new byte[length];
            }
            else
            {
                randBytes = new byte[1];
            }

            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();

            rand.GetBytes(randBytes);

            return randBytes;
        }

        private static void ClearBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentException("buffer");
            }

            for (int x = 0; x < buffer.Length; x++)
            {
                buffer[x] = 0;
            }
        }

        [APIAuthorize]
        [HttpGet]
        [Route("api/server")]
        public IEnumerable<ServerList> ServerList()
        {
            var serverlist = _context.ServerLists.ToList();
            return serverlist;
        }

        [APIAuthorize]
        [HttpsAuthorize]
        [HttpGet]
        [Route("api/server/{server_id}")]
        public IHttpActionResult Connect(int server_id)
        {
            var getserver = _context.ServerLists.SingleOrDefault(c => c.Id == server_id);
            if (getserver == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
            {
                byte[] dcrpt = Convert.FromBase64String(getserver.Password);
                try
                {
                    if (ValidateConnection(getserver.IPAddress, getserver.UserId, ASCIIEncoding.ASCII.GetString(dcrpt)))
                        return StatusCode(HttpStatusCode.Accepted);
                    else
                        throw new HttpResponseException(HttpStatusCode.NotAcceptable);
                }
                catch (FormatException)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
                
            }
        }

        [APIAuthorize(Roles = "SUPERADMIN")]
        [HttpsAuthorize]
        [HttpPost]
        [Route("api/server/connect")]
        public IHttpActionResult AddServer(AddServerViewModel server)
        {

            if (ModelState.IsValid)
            {
                if (ValidateConnection(server.IPAddress, server.UserId, server.Password))
                {
                    var getserver = _context.ServerLists.SingleOrDefault(c => c.IPAddress == server.IPAddress);
                    if (getserver != null)
                    {
                        byte[] ncrpt = ASCIIEncoding.ASCII.GetBytes(server.Password);

                        getserver.UserId = server.UserId;
                        getserver.Password = Convert.ToBase64String(ncrpt);
                        _context.SaveChanges();

                        return StatusCode(HttpStatusCode.Accepted);
                    }
                    else
                    {
                        byte[] ncrpt = ASCIIEncoding.ASCII.GetBytes(server.Password);

                        var addserver = new ServerList
                        {
                            IPAddress = server.IPAddress,
                            UserId = server.UserId,
                            Password = Convert.ToBase64String(ncrpt),
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

        [APIAuthorize(Roles = "SUPERADMIN")]
        [HttpDelete]
        [Route("api/server/remove/{server_id}")]
        public IHttpActionResult Remove(int server_id)
        {
            var getserver = _context.ServerLists.SingleOrDefault(c => c.Id == server_id);
            if (getserver == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
            {
                _context.ServerLists.Remove(getserver);
                _context.SaveChanges();
                return Ok();
            }          
        }
    }
}
