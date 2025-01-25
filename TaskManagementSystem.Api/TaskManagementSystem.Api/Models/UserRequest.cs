namespace TaskManagementSystem.Api.Models
{
    public class UserRequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public DBUser ToDBUser()
        {
            return new DBUser { Id = Id, UserName = UserName, Email = Email, PhoneNumber = PhoneNumber };

        }
    }
}
