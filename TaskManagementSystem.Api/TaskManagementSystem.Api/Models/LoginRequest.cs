namespace TaskManagementSystem.Api.Models
{
    /// <summary>
    /// Represents a request to log in, containing the username and password.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the username of the user attempting to log in.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the user attempting to log in.
        /// </summary>
        public string Password { get; set; }
    }
}
