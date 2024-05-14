using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Web.Security;
using System.Data.Entity.Infrastructure;


namespace ThisCommVas.Controllers
{
    public class HomeController : Controller
    {
        private readonly CommVasEntities db = new CommVasEntities();
        // GET: Home
        public ActionResult Index(int? category_Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            using (var db = new CommVasEntities())
            {
                IQueryable<products> productsQuery = db.products.Include("product_image");

                if (category_Id.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.category_Id == category_Id.Value);
                }

                ViewBag.Products = productsQuery.ToList();
                ViewBag.Categories = db.categories.ToList();
            }

            return View();
        }


        [HttpPost]
        public ActionResult Search(string searchTerm)
        {
            using (var db = new CommVasEntities())
            {
                var productsQuery = db.SearchProduct(searchTerm)
                             .GroupBy(p => p.product_id)
                             .Select(g => g.First()) // Select only the first product of each group
                             .ToList();

                // Return a partial view with the search results
                return PartialView("_SearchResults", productsQuery);
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EditPersonalInfo()
        {
            // Retrieve the id of the logged-in user
            string username = User.Identity.Name;
            int userId = GetUserIdByUsername(username);

            // Retrieve the personal information for the logged-in user
            var personalInfo = db.PersonalInformation.Where(pi => pi.UserId == userId).FirstOrDefault();
           
            if (personalInfo == null)
            {
                // Handle case where personal information is not found
                return HttpNotFound("Personal information not found");
            }

            // Pass personal information to the view for editing
            return View(personalInfo);
        }

        private int GetUserIdByUsername(string userName)
        {
            using (var db = new CommVasEntities())
            {
                var user = db.users.FirstOrDefault(u => u.username == userName);
                if (user != null)
                {
                    return user.id;
                }
            }

            throw new Exception($"User with username '{userName}' not found in database.");
        }


        // POST: Home/EditPersonalInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPersonalInformation(PersonalInformation model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the id of the logged-in user
                string username = User.Identity.Name;
                int userId = GetUserIdByUsername(username);

                // Update the personal information in the database
                var existingPersonalInfo = db.PersonalInformation.FirstOrDefault(pi => pi.UserId == userId);
                if (existingPersonalInfo != null)
                {
                    existingPersonalInfo.FirstName = model.FirstName;
                    existingPersonalInfo.LastName = model.LastName;
                    // Update other fields as needed

                    db.SaveChanges();

                    // Redirect to the index page after successful editing
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle case where personal information is not found
                    return HttpNotFound("Personal information not found");
                }
            }

            // If ModelState is not valid, return the edit view with errors
            return View(model);
        }
        public ActionResult About()
        {
            return View();
        }

    }
}