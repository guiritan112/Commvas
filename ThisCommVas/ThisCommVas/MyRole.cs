using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ThisCommVas
{
    public class MyRole : RoleProvider
    {
        public override string ApplicationName { get; set; }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            // Implement logic to add users to roles
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            // Implement logic to create a new role
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            // Implement logic to delete a role
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            // Implement logic to find users in a role
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            // Implement logic to retrieve all roles
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var db = new CommVasEntities()) // Replace with your DbContext or database context
            {
                var user = db.users.FirstOrDefault(u => u.username == username);
                if (user != null)
                {
                    string roleName = GetRoleName(user.RoleId); // Get the role name based on the RoleId
                    return new string[] { roleName }; // Return the role name
                }
            }
            return new string[] { }; // Return empty array if no roles found
        }

        public override string[] GetUsersInRole(string roleName)
        {
            // Implement logic to retrieve users in a role
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            // Implement logic to check if a user is in a role
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            // Implement logic to remove users from roles
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            // Implement logic to check if a role exists
            throw new NotImplementedException();
        }
        private string GetRoleName(int? roleId)
        {
            if (!roleId.HasValue) // Check if roleId is null
            {
                return "Unknown";
            }

            switch (roleId.Value) // Use Value to access the underlying integer
            {
                case 1:
                    return "Admin";
                case 2:
                    return "Artist";
                default:
                    return "Unknown";
            }
        }
    }
}
