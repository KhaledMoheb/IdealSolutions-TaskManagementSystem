namespace TaskManagementSystem.Api.Models
{
    /// <summary>
    /// Represents an error related to task management, including an error message, error code, and optional additional details.
    /// </summary>
    public class TaskError
    {
        /// <summary>
        /// The message describing the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// A numeric code representing the type of error (e.g., 404 for "Not Found", 500 for "Internal Server Error").
        /// </summary>
        public TaskErrorCode ErrorCode { get; set; }

        /// <summary>
        /// An optional field for additional details about the error, such as further context or instructions.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskError"/> class with the specified message, error code, and optional details.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        /// <param name="code">The error code representing the type of error.</param>
        /// <param name="details">Optional additional details regarding the error.</param>
        public TaskError(string message, TaskErrorCode code, string details = null)
        {
            Message = message;
            ErrorCode = code;
            Details = details;
        }

        /// <summary>
        /// Enum representing different types of task-related errors.
        /// </summary>
        public enum TaskErrorCode
        {
            /// <summary>
            /// Error code indicating that the user was not found.
            /// </summary>
            UserNotFound = 1,

            /// <summary>
            /// Error code indicating that the requested task or resource was not found.
            /// </summary>
            NotFound = 2,

            /// <summary>
            /// Error code indicating that the user does not have permission to access a specific resource.
            /// </summary>
            ForbiddenAccess = 3,
        }

        /// <summary>
        /// Helper method to create a "User Not Found" error response.
        /// </summary>
        /// <param name="message">The error message for the "User Not Found" error.</param>
        /// <returns>A <see cref="TaskError"/> instance with the "User Not Found" error code.</returns>
        public static TaskError UserNotFound(string message)
        {
            return new TaskError(message, TaskErrorCode.UserNotFound);
        }

        /// <summary>
        /// Helper method to create a "Not Found" error response.
        /// </summary>
        /// <param name="message">The error message for the "Not Found" error.</param>
        /// <returns>A <see cref="TaskError"/> instance with the "Not Found" error code.</returns>
        public static TaskError NotFound(string message)
        {
            return new TaskError(message, TaskErrorCode.NotFound);
        }

        /// <summary>
        /// Helper method to create a "Forbidden Access" error response.
        /// </summary>
        /// <returns>A <see cref="TaskError"/> instance with the "Forbidden Access" error code.</returns>
        public static TaskError ForbiddenAccess()
        {
            return new TaskError("You do not have permission to access this task.", TaskErrorCode.ForbiddenAccess);
        }
    }
}
