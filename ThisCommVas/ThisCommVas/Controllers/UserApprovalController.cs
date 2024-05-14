using System.Linq;
using System.Web.Mvc;
using System;
using ThisCommVas;

namespace ThisCommVas.Controllers
{

    public class UserApprovalController : Controller
    {
        private readonly CommVasEntities _dbContext;

        public UserApprovalController()
        {
            _dbContext = new CommVasEntities(); // Initialize the database context
        }

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login"); // Redirect to login page
            }
            // Get the username of the currently logged-in user (replace "User.Identity.Name" with your method of obtaining the username)
            string currentUserName = User.Identity.Name;

            // Get the ID of the currently logged-in user
            int currentUserId = GetUserIdByUsername(currentUserName);

            // Fetch all users from the database excluding the currently logged-in user
            var allUsers = _dbContext.users.Where(u => u.id != currentUserId).ToList();

            ViewBag.AllUsers = allUsers; // Pass all users to the view
            return View();
        }

        private int GetUserIdByUsername(string userName)
        {
            var user = _dbContext.users.FirstOrDefault(u => u.username == userName);
            return user != null ? user.id : -1; // Return -1 if user is not found
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateUserStatus(int userId, int status)
        {
            // Validate userId
            var user = _dbContext.users.Find(userId);
            if (user == null)
            {
                return Json(new { success = false, error = "User not found" });
            }

            try
            {
                user.Status = status == 1 ? 1 : 2; // Set status to 1 if approved, otherwise set to 2
                _dbContext.SaveChanges(); // Save changes to the database

                // Return updated user information
                return Json(new { success = true, status = user.Status == 1 ? "Approved" : (user.Status == 2 ? "Rejected" : "Pending") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUserRole(int userId, int roleId)
        {
            var user = _dbContext.users.Find(userId);
            if (user == null)
            {
                return Json(new { success = false, error = "User not found" });
            }

            try
            {
                user.RoleId = roleId;
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUser(int userId)
        {
            var user = _dbContext.users.Find(userId);
            if (user == null)
            {
                return Json(new { success = false, error = "User not found" });
            }

            try
            {
                // Delete associated records from PersonalInformation table
                var personalInfo = _dbContext.PersonalInformation.FirstOrDefault(pi => pi.UserId == userId);
                if (personalInfo != null)
                {
                    _dbContext.PersonalInformation.Remove(personalInfo);
                }

                // Delete user from users table
                _dbContext.users.Remove(user);

                _dbContext.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

    }
}
