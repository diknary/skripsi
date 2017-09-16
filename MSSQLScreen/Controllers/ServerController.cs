using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using MSSQLScreen.Models;
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

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddServer(AddServerViewModel server)
        {
            if (ModelState.IsValid)
            {
                if (ValidateConnection(server.IPAddress, server.UserId, server.Password))
                {
                    var identity = (ClaimsIdentity)User.Identity;
                    var claims = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    var getadmin = _context.AdminAccounts.Single(c => c.Username == claims);
                    var getserver = _context.ServerLists.Single(c => c.IPAddress == server.IPAddress);
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

        public ActionResult Connect(int id)
        {
            var getserver = _context.ServerLists.Single(c => c.Id == id);

            if (!ValidateConnection(getserver.IPAddress, getserver.UserId, getserver.Password))
            {
                TempData["errmsg"] = "Server login credentials have been changed";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Session["IPAddress"] = getserver.IPAddress;
                return RedirectToAction("MigrateJob", "Job");
            }
                
        }

        public ActionResult Remove(int id)
        {
            var getserver = _context.ServerLists.Single(c => c.Id == id);
            _context.ServerLists.Remove(getserver);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

    }
}