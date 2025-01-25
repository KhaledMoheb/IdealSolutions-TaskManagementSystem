using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Api.Models
{
    public class DBUser : IdentityUser<int>
    {

        public override string UserName
        {
            get { return base.UserName; }
            set { base.UserName = value ?? throw new ArgumentNullException(nameof(value)); }
        }
        /// <summary>
        /// Admin or User
        /// </summary>
        public string Role { get; set; }

        public UserResponse ToUserResponse()
        {
            return new UserResponse { Id = Id, Email = Email, UserName = UserName, PhoneNumber = PhoneNumber };
        }
    }
}
