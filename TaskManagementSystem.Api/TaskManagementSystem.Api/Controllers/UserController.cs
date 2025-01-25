using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.ServicesContracts;

namespace TaskManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<DBUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private const string UserRole = "User";

        public UserController(UserManager<DBUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Get all users in the "User" role.
        /// </summary>
        /// <returns>List of users with the "User" role.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all users in the 'User' role", Description = "Retrieve a list of users who are assigned the 'User' role.")]
        [SwaggerResponse(200, "List of users in the 'User' role.", typeof(IEnumerable<DBUser>))]
        [SwaggerResponse(403, "Forbidden access.")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.GetUsersInRoleAsync(UserRole);
            return Ok(users);
        }

        /// <summary>
        /// Get a user by their ID.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>User details if found, 404 if not found.</returns>
        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "Get user by ID", Description = "Retrieve user details by their unique ID.")]
        [SwaggerResponse(200, "User details found.", typeof(DBUser))]
        [SwaggerResponse(404, "User not found.")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="user">User data for the new user.</param>
        /// <param name="password">Password for the new user.</param>
        /// <returns>Created user, or error details if creation fails.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new user", Description = "Create a new user and assign the 'User' role.")]
        [SwaggerResponse(201, "User created successfully.", typeof(DBUser))]
        [SwaggerResponse(400, "Bad request. User creation failed.")]
        [SwaggerResponse(409, "Conflict. User already exists.")]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest user, [FromQuery] string password)
        {
            DBUser dBUser = user.ToDBUser();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userResult = await _userManager.CreateAsync(dBUser, password);
            if (userResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(dBUser, UserRole);
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new { message = "Failed to assign role", errors = roleResult.Errors });
                }

                return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, user);
            }

            return BadRequest(new { message = "User creation failed", errors = userResult.Errors });
        }

        /// <summary>
        /// Update an existing user's details.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="user">Updated user data.</param>
        /// <returns>Updated user, or error details if update fails.</returns>
        [HttpPut("{userId}")]
        [SwaggerOperation(Summary = "Update an existing user", Description = "Update the user details like email, username, and phone number.")]
        [SwaggerResponse(200, "User updated successfully.", typeof(DBUser))]
        [SwaggerResponse(400, "Bad request. User ID mismatch or invalid data.")]
        [SwaggerResponse(404, "User not found.")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] DBUser user)
        {
            if (userId != user.Id)
            {
                return BadRequest(new { message = "User ID mismatch" });
            }

            var existingUser = await _userManager.FindByIdAsync(userId.ToString());
            if (existingUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Update user properties
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;

            // Update user details
            var updateResult = await _userManager.UpdateAsync(existingUser);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new { message = "Failed to update user", errors = updateResult.Errors });
            }

            return Ok(existingUser);
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="userId">The user ID to delete.</param>
        /// <returns>No content if deleted successfully, or error details if deletion fails.</returns>
        [HttpDelete("{userId}")]
        [SwaggerOperation(Summary = "Delete a user", Description = "Delete a user by their unique ID.")]
        [SwaggerResponse(204, "User deleted successfully.")]
        [SwaggerResponse(404, "User not found.")]
        [SwaggerResponse(400, "Bad request. Error deleting user.")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var result = await _userManager.DeleteAsync(existingUser);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(new { message = "Error deleting user", errors = result.Errors });
        }
    }
}
