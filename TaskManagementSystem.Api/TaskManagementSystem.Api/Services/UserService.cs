using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Services
{
    public class UserService
    {
        private readonly UserManager<DBUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserService(UserManager<DBUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedUsersAsync()
        {
            // Define roles
            var roles = new[] { "Admin", "User" };

            // Ensure roles exist
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

            // Seed admin user
            await CreateUserIfNotExistsAsync("admin", "Admin123!", "Admin");

            // Seed standard user
            await CreateUserIfNotExistsAsync("user1", "User123!", "User");
        }

        private async Task CreateUserIfNotExistsAsync(string userName, string password, string role)
        {
            var existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser != null)
            {
                // Optional: Check if the user already has the correct role
                var isInRole = await _userManager.IsInRoleAsync(existingUser, role);
                if (!isInRole)
                {
                    await _userManager.AddToRoleAsync(existingUser, role);
                }

                return; // User already exists, no further action needed
            }

            // Create user if it doesn't exist
            var newUser = new DBUser { UserName = userName, Role = role, Email = userName + "@gmail.com", PhoneNumber = "123123123" };
            var createResult = await _userManager.CreateAsync(newUser, password);

            if (createResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(newUser, role);
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Failed to assign role '{role}' to user '{userName}'. Errors: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                throw new Exception($"Failed to create user '{userName}'. Errors: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
