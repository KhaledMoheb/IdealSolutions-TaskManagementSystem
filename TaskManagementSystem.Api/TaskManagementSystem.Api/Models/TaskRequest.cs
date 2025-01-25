namespace TaskManagementSystem.Api.Models
{
    public class TaskRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // Pending, In Progress, Completed
        public int AssignedUserId { get; set; }

        public DBTask ToDBTask()
        {
            return new DBTask { Id = Id, Title = Title, Description = Description, Status = Status, AssignedUserId = AssignedUserId };
        }
    }
}
