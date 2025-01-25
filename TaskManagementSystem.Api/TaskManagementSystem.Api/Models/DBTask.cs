namespace TaskManagementSystem.Api.Models
{
    public class DBTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // Pending, In Progress, Completed
        public int AssignedUserId { get; set; }
        public DBUser AssignedUser { get; set; }

        public TaskResponse ToTaskResponse()
        {
            return new TaskResponse { Id = Id, Title = Title, Description = Description, Status = Status, AssignedUserId = AssignedUserId };
        }
    }
}
