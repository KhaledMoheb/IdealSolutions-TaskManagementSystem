using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Api.Models
{
    /// <summary>
    /// Represents a user in the system, extending from IdentityUser with an integer ID.
    /// </summary>
    public class DBUser : IdentityUser<int>
    {
        /// <summary>
        /// Gets or sets the username of the user. Throws an exception if null.
        /// </summary>
        public override string UserName
        {
            get { return base.UserName; }
            set { base.UserName = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        /// <summary>
        /// Gets or sets the role of the user. Possible values: "Admin" or "User".
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Converts the DBUser entity to a UserResponse DTO for API responses.
        /// </summary>
        /// <returns>A UserResponse containing the relevant user details.</returns>
        public UserResponse ToUserResponse()
        {
            return new UserResponse
            {
                Id = Id,
                Email = Email,
                UserName = UserName,
                PhoneNumber = PhoneNumber
            };
        }
    }
}
