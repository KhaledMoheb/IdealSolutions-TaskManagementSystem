using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Services
{
    /// <summary>
    /// Service responsible for user-related operations, including user seeding and role management.
    /// </summary>
    public class UserService
    {
        private readonly UserManager<DBUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userManager">The manager for user-related operations.</param>
        /// <param name="roleManager">The manager for role-related operations.</param>
        public UserService(UserManager<DBUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Seeds the database with predefined roles and users (Admin, User).
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SeedUsersAsync()
        {
            // Define roles
            var roles = new[] { "Admin", "User" };

            // Ensure roles exist in the system
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole<int>(role));
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception($"Failed to create role: {role}. Errors: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // Seed the Admin user
            await CreateUserIfNotExistsAsync("admin", "Admin123!", "Admin");

            // Seed a standard User
            await CreateUserIfNotExistsAsync("user1", "User123!", "User");
        }

        /// <summary>
        /// Creates a user if they do not already exist and assigns the specified role.
        /// </summary>
        /// <param name="userName">The username of the user to create.</param>
        /// <param name="password">The password for the user.</param>
        /// <param name="role">The role to assign to the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task CreateUserIfNotExistsAsync(string userName, string password, string role)
        {
            // Check if the user already exists
            var existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser != null)
            {
                // Optional: Check if the user already has the correct role
                var isInRole = await _userManager.IsInRoleAsync(existingUser, role);
                if (!isInRole)
                {
                    // Assign the role if the user doesn't already have it
                    await _userManager.AddToRoleAsync(existingUser, role);
                }

                return; // User already exists, no further action needed
            }

            // Create the user if they don't exist
            var newUser = new DBUser
            {
                UserName = userName,
                Role = role,
                Email = userName + "@gmail.com",
                PhoneNumber = "123123123"
            };

            // Create the new user with the specified password
            var createResult = await _userManager.CreateAsync(newUser, password);

            if (createResult.Succeeded)
            {
                // Assign the specified role to the new user
                var roleResult = await _userManager.AddToRoleAsync(newUser, role);
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Failed to assign role '{role}' to user '{userName}'. Errors: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                // Throw an exception if user creation fails
                throw new Exception($"Failed to create user '{userName}'. Errors: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
