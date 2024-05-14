using System;
using System.Linq;
using System.Web.Mvc;
using ThisCommVas;

namespace ThisCommVas.Controllers
{
    public class RegisterController : Controller
    {
        private readonly CommVasEntities db = new CommVasEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(string email, string username, string password, int RoleId, string firstName, string lastName, DateTime dob, string gender, string nationality, string phone)
        {
            if (ModelState.IsValid)
            {
                bool isEmailUnique = !db.users.Any(u => u.email == email);
                bool isUsernameUnique = !db.users.Any(u => u.username == username);

                if (isEmailUnique && isUsernameUnique)
                {
                    int defaultStatus = 3; // Assign default status

                    // Add user to users table
                    var newUser = new users
                    {
                        email = email,
                        username = username,
                        password = password,
                        RoleId = RoleId,
                        Status = defaultStatus
                    };
                    db.users.Add(newUser);
                    db.SaveChanges();

                    // Retrieve the user ID of the newly registered user
                    int userId = newUser.id;

                    // Add personal information to PersonalInformation table
                    var personalInfo = new PersonalInformation
                    {
                        UserId = userId,
                        FirstName = firstName,
                        LastName = lastName,
                        DateOfBirth = dob,
                        Gender = gender,
                        Nationality = nationality,
                        Phone = phone
                    };
                    db.PersonalInformation.Add(personalInfo);
                    db.SaveChanges();

                    // Redirect to login page or any other page
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    ViewBag.ErrorMessage = "Email or username already exists.";
                }
            }

            // If ModelState is not valid, return to registration page with errors
            return View("Index");
        }

        [HttpPost]
        public ActionResult CheckUnique(string email, string username)
        {
            bool isEmailUnique = !db.users.Any(u => u.email == email);
            bool isUsernameUnique = !db.users.Any(u => u.username == username);

            return Json(new { unique = isEmailUnique && isUsernameUnique });
        }
    }
}
