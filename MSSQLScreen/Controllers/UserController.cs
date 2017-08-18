using System.Web;
using System.Linq;
using System.Web.Mvc;
using MSSQLScreen.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace MSSQLScreen.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext _context;

        public UserController()
        {
            _context = new ApplicationDbContext();
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
                    var getUserRole = _context.UserAccounts.Single(c => c.Username == user.Username);
                    var identity = new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Username),
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Role, getUserRole.Privilege),
                        },
                        DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = user.RememberMe }, identity);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View();
                }
            }
            ModelState.Remove("Password");
            return View();
        }

        [Route("user/register")]
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
        public ActionResult ManageAdmin(UserAccount admin)
        {
            var usr = _context.UserAccounts.SingleOrDefault(c => c.Username == admin.Username);
            if (ModelState.IsValid)
            {
                if (usr == null)
                    ModelState.AddModelError("", "Username is not found");
                else
                {
                    usr.Privilege = admin.Privilege;
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [WebAuthorize]
        [HttpPost]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            //// clear authentication cookie
            //HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            //cookie1.Expires = DateTime.Now.AddYears(-1);
            //Response.Cookies.Add(cookie1);

            //// clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            //SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            //HttpCookie cookie2 = new HttpCookie(sessionStateSection.CookieName, "");
            //cookie2.Expires = DateTime.Now.AddYears(-1);
            //Response.Cookies.Add(cookie2);
            return RedirectToAction("Login", "User");
        }
    }
}