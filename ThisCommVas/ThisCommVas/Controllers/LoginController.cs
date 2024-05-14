using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThisCommVas;
using System.Web.Security;

namespace ThisCommVas.Controllers
{
    public class LoginController : Controller
    {
        private CommVasEntities _context = new CommVasEntities();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = _context.users.SingleOrDefault(u => u.username == username);

            if (user != null)
            {
                if (user.password == password)
                {
                    if (user.Status == 1)
                    {
                        // Create authentication ticket
                        FormsAuthentication.SetAuthCookie(username, false);

                        // Store user role in session
                        Session["UserRole"] = user.Roles.RoleName;

                        return RedirectToAction("Index", "Home");
                    }
                    else if (user.Status == 2)
                    {
                        ViewBag.ErrorMessage = "Your account has been rejected. Please contact support.";
                    }
                    else if (user.Status == 3)
                    {
                        ViewBag.ErrorMessage = "Your account is pending approval. Please wait for approval.";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Your account status is invalid.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid password";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username";
            }

            return View("Index");
        }

    }
}
