using System.Web;
using System.Linq;
using System.Web.Mvc;
using MSSQLScreen.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;

namespace MSSQLScreen.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext _context;

        public UserController()
        {
            _context = new ApplicationDbContext();
        }

        private void CreateLoginHistory(int id)
        {
            var addloginhis = new AdminLog
            {
                LoginDate = DateTime.Now,
                AdminAccountId = id
            };
            _context.AdminLogs.Add(addloginhis);
            _context.SaveChanges();
        }

        private void GetLastLogin(string username)
        {
            var getUser = _context.AdminAccounts.Single(c => c.Username == username);
            var getLastLogin = _context.AdminLogs.Where(c => c.AdminAccountId == getUser.Id).OrderByDescending(c => c.Id).FirstOrDefault();
            getUser.LastLogin = getLastLogin.LoginDate;
            _context.SaveChanges();
        }

        [WebAuthorize]
        public ActionResult Index()
        {
            var userInDb = _context.AdminAccounts.ToList();
            return View(userInDb);
        }

        // GET: User
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (new ValidationManager().IsValid(user.Username, user.Password))
                {
                    var getUser = _context.AdminAccounts.Single(c => c.Username == user.Username);
                    var identity = new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Username),
                            new Claim(ClaimTypes.Name, getUser.Name),
                            new Claim(ClaimTypes.Role, getUser.Privilege),
                            new Claim("AdminId", getUser.Id.ToString())
                        },
                        DefaultAuthenticationTypes.ApplicationCookie);
                    
                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = user.RememberMe }, identity);

                    CreateLoginHistory(getUser.Id);

                    GetLastLogin(user.Username);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View();
                }
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserAccount account)
        {
            if (ModelState.IsValid)
            {
                _context.UserAccounts.Add(account);
                _context.SaveChanges();
                ModelState.Clear();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdmin(AddAdminViewModel admin)
        {
            var usr = _context.UserAccounts.SingleOrDefault(c => c.Username == admin.Username);
            var adm = _context.AdminAccounts.SingleOrDefault(c => c.Username == admin.Username);
            if (ModelState.IsValid)
            {
                if (usr == null)
                    TempData["errmsg"] = "Username is not found";
                else if (adm != null)              
                    TempData["errmsg"] = "Admin is already exist";               
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
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["errmsg"] = "Field cannot be blank";
                return RedirectToAction("Index", "Home");
            }

        }

        [WebAuthorize]
        public ActionResult LoginHistory(int id)
        {
            var loginhistory = _context.AdminLogs.Where(c => c.AdminAccountId == id).OrderBy(c => c.Id).ToList();
            return View(loginhistory);
        }


        [WebAuthorize]
        [HttpPost]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "User");
        }
    }
}