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
        public ActionResult Connect(AddServerViewModel server)
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
                        Session["IPAddress"] = server.IPAddress;
                        return RedirectToAction("MigrateJob", "Job");
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
                        Session["IPAddress"] = server.IPAddress;
                        return RedirectToAction("MigrateJob", "Job");
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
        [Route("server/remove/{server_id}/{admin_id}")]
        public ActionResult Remove(int server_id, int admin_id)
        {
            var getserver = _context.ServerLists.Single(c => c.Id == server_id && c.AdminAccountId == admin_id);
            _context.ServerLists.Remove(getserver);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

    }
}