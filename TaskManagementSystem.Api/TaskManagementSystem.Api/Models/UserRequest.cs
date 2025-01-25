namespace TaskManagementSystem.Api.Models
{
    /// <summary>
    /// Represents a request model for creating or updating a user.
    /// </summary>
    public class UserRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// This is the name the user will use to log in.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Converts the UserRequest instance to a DBUser instance.
        /// This is used to map the request data to the database model.
        /// </summary>
        /// <returns>A DBUser instance populated with the data from the UserRequest.</returns>
        public DBUser ToDBUser()
        {
            return new DBUser { Id = Id, UserName = UserName, Email = Email, PhoneNumber = PhoneNumber };
        }
    }
}
