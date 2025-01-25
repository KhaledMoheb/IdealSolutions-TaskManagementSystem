namespace TaskManagementSystem.Api.Models
{
    public class TaskError
    {
        // The message describing the error
        public string Message { get; set; }

        // A numeric code that can represent the type of error (e.g., 404 for "Not Found", 500 for "Internal Server Error")
        public TaskErrorCode ErrorCode { get; set; }

        // An optional field for additional details about the error
        public string Details { get; set; }

        // Constructor for TaskError with all fields
        public TaskError(string message, TaskErrorCode code, string details = null)
        {
            Message = message;
            ErrorCode = code;
            Details = details;
        }

        public enum TaskErrorCode
        {
            UserNotFound = 1,
            NotFound = 2,
            ForbiddenAccess = 3,
        }

        // Helper to create error response for UserNotFound
        public static TaskError UserNotFound(string message)
        {
            return new TaskError(message, TaskErrorCode.UserNotFound);
        }

        // Helper to create error response for NotFound
        public static TaskError NotFound(string message)
        {
            return new TaskError(message, TaskErrorCode.NotFound);
        }

        // Helper to create error response for Forbidden access
        public static TaskError ForbiddenAccess()
        {
            return new TaskError("You do not have permission to access this task.", TaskErrorCode.ForbiddenAccess);
        }
    }
}
