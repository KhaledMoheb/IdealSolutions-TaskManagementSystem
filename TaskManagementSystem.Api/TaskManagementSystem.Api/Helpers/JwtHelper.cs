using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementSystem.Api.Models;

namespace TaskManagementSystem.Api.Helpers
{
    // The IJwtHelper interface defines a contract for generating JWT tokens for users.
    public interface IJwtHelper
    {
        string GenerateJwtToken(DBUser user); // Method to generate the JWT token.
    }

    // The JwtHelper class implements IJwtHelper and contains logic to create a JWT token.
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration; // Injected configuration to read settings from appsettings.json or other sources.

        // Constructor to initialize JwtHelper with the application configuration.
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Generates a JWT token for a given user.
        public string GenerateJwtToken(DBUser user)
        {
            // Create the claims (user-related information) to embed in the token.
            var claims = new[]
            {
                // The ClaimTypes.NameIdentifier is used for the user's ID (e.g., unique identifier).
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                // The ClaimTypes.Name is used for the user's username.
                new Claim(ClaimTypes.Name, user.UserName),

                // The ClaimTypes.Role is used for the user's role (e.g., Admin, User).
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Create a symmetric security key from the "Jwt:SecretKey" configuration value.
            // The secret key is used for signing the token to ensure it hasn't been tampered with.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            // Define the signing credentials using the security key and the HmacSha256 algorithm.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token, setting the issuer, audience, claims, expiration, and signing credentials.
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // Token issuer (who issued the token).
                audience: _configuration["Jwt:Audience"], // Token audience (who the token is for).
                claims: claims, // Claims to be included in the token.
                expires: DateTime.Now.AddHours(1), // Set the token to expire in 1 hour.
                signingCredentials: creds // The signing credentials used to sign the token.
            );

            // Return the token as a string (JWT token).
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
