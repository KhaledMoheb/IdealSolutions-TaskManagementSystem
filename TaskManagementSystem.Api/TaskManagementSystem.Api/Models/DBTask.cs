namespace TaskManagementSystem.Api.Models
{
    /// <summary>
    /// Represents a task entity in the system, typically stored in the database.
    /// </summary>
    public class DBTask
    {
        /// <summary>
        /// Gets or sets the unique identifier for the task.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title or name of the task.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a detailed description of the task.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current status of the task.
        /// Status can be: Pending, In Progress, or Completed.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user assigned to this task.
        /// </summary>
        public int AssignedUserId { get; set; }

        /// <summary>
        /// Gets or sets the user assigned to the task.
        /// </summary>
        public DBUser AssignedUser { get; set; }

        /// <summary>
        /// Converts the DBTask entity to a TaskResponse DTO for API responses.
        /// </summary>
        /// <returns>A TaskResponse containing the relevant task details.</returns>
        public TaskResponse ToTaskResponse()
        {
            return new TaskResponse
            {
                Id = Id,
                Title = Title,
                Description = Description,
                Status = Status,
                AssignedUserId = AssignedUserId
            };
        }
    }
}
