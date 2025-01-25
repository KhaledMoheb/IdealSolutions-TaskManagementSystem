namespace TaskManagementSystem.Api.Models
{
    /// <summary>
    /// Represents a response model for a task, typically used for returning task data in API responses.
    /// </summary>
    public class TaskResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the task.
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
        /// Valid values include: "Pending", "In Progress", and "Completed".
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the user identifier to whom the task is assigned.
        /// </summary>
        public int AssignedUserId { get; set; }
    }
}
