namespace TaskManagementSystem.Api.Models
{
    /// <summary>
    /// Represents a request for creating or updating a task.
    /// </summary>
    public class TaskRequest
    {
        /// <summary>
        /// Gets or sets the task identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the task.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the task.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the status of the task. 
        /// Valid values: "Pending", "In Progress", "Completed".
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the user identifier to whom the task is assigned.
        /// </summary>
        public int AssignedUserId { get; set; }

        /// <summary>
        /// Converts the current task request into a <see cref="DBTask"/> entity for database operations.
        /// </summary>
        /// <returns>A <see cref="DBTask"/> instance representing the task in the database.</returns>
        public DBTask ToDBTask()
        {
            return new DBTask { Id = Id, Title = Title, Description = Description, Status = Status, AssignedUserId = AssignedUserId };
        }
    }
}
