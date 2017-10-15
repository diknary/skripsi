using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using MSSQLScreen.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace MSSQLScreen.Controllers
{
    public class ServerController : Controller
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

        [WebAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [WebAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddServer(AddServerViewModel server)
        {
            if (ModelState.IsValid)
            {
                if (ValidateConnection(server.IPAddress, server.UserId, server.Password))
                {
                    var getserver = _context.ServerLists.SingleOrDefault(c => c.IPAddress == server.IPAddress);
                    if (getserver != null)
                    {
                        getserver.UserId = server.UserId;
                        getserver.Password = server.Password;
                        _context.SaveChanges();
                        return RedirectToAction("MigrateJob", "Job", new { IP = server.IPAddress });
                    }
                    else
                    {
                        var addserver = new ServerList
                        {
                            IPAddress = server.IPAddress,
                            UserId = server.UserId,
                            Password = server.Password
                        };

                        _context.ServerLists.Add(addserver);
                        _context.SaveChanges();
                        return RedirectToAction("MigrateJob", "Job", new { IP = server.IPAddress});
                    }  
                    
                }
                else
                {
                    TempData["errmsg"] = "Login failed";
                    return RedirectToAction("Index", "Home");
                }
                    
            }
            else
            {
                TempData["errmsg"] = "Field cannot be blank";
                return RedirectToAction("Index", "Home");
            }
            
        }

        [WebAuthorize]
        public ActionResult Connect(int server_id)
        {
            var getserver = _context.ServerLists.SingleOrDefault(c => c.Id == server_id);
            if (ValidateConnection(getserver.IPAddress, getserver.UserId, getserver.Password))
            {
                return RedirectToAction("MigrateJob", "Job", new { IP = getserver.IPAddress });

            }
            else
            {
                TempData["errmsg"] = "Login failed/User credentials may have been changed";
                return RedirectToAction("Index", "Home");
            }
        }

        [WebAuthorize]
        [Route("server/remove/{server_id}")]
        public ActionResult Remove(int server_id)
        {
            var getserver = _context.ServerLists.Single(c => c.Id == server_id);
            _context.ServerLists.Remove(getserver);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

    }
}