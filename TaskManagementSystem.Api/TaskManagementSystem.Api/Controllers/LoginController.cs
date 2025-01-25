using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Api.Helpers;
using TaskManagementSystem.Api.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace TaskManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<DBUser> _userManager;
        private readonly SignInManager<DBUser> _signInManager;
        private readonly IJwtHelper _jwtHelper;

        public LoginController(
            UserManager<DBUser> userManager,
            SignInManager<DBUser> signInManager,
            IJwtHelper jwtHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token and user information.
        /// </summary>
        /// <param name="request">The login credentials including username and password.</param>
        /// <returns>Returns a JWT token and user information if login is successful.</returns>
        /// <response code="200">Successfully authenticated and JWT token returned.</response>
        /// <response code="401">Invalid credentials provided.</response>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login", Description = "Authenticates a user and returns a JWT token.")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Validate user credentials using SignInManager
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);
            if (!signInResult.Succeeded)
            {
                return Unauthorized("Invalid credentials");
            }

            // Generate JWT Token
            var token = _jwtHelper.GenerateJwtToken(user);

            // Set the token in a secure, HttpOnly cookie
            Response.Cookies.Append("authToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensure HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1) // Token expiration
            });

            // Return the JWT token and user information
            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.UserName,
                    user.Role
                }
            });
        }

        /// <summary>
        /// Logs the user out by clearing the authentication token.
        /// </summary>
        /// <returns>A message confirming the logout success.</returns>
        /// <response code="200">Successfully logged out.</response>
        [HttpPost("logout")]
        [SwaggerOperation(Summary = "Logout", Description = "Logs the user out by clearing the authentication token.")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("authToken");
            return Ok(new { message = "Logged out successfully." });
        }
    }
}
