namespace TaskManagementSystem.Api.Models
{
    /// <summary>
    /// Represents the result of an operation, which can either be successful with a value or fail with an error.
    /// </summary>
    public class Result<TValue, TError>
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the value associated with the successful result.
        /// </summary>
        public TValue Value { get; private set; }

        /// <summary>
        /// Gets the error associated with a failed result.
        /// </summary>
        public TError Err { get; private set; }

        // Private constructor used by Success and Error methods.
        private Result(bool isSuccess, TValue value, TError error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Err = error;
        }

        /// <summary>
        /// Creates a successful result with the given value.
        /// </summary>
        /// <param name="value">The value representing the successful outcome.</param>
        /// <returns>A <see cref="Result{TValue, TError}"/> indicating success.</returns>
        public static Result<TValue, TError> Success(TValue value)
        {
            return new Result<TValue, TError>(true, value, default);
        }

        /// <summary>
        /// Creates a failed result with the given error.
        /// </summary>
        /// <param name="error">The error representing the failed outcome.</param>
        /// <returns>A <see cref="Result{TValue, TError}"/> indicating failure.</returns>
        public static Result<TValue, TError> Error(TError error)
        {
            return new Result<TValue, TError>(false, default, error);
        }

        /// <summary>
        /// Matches the result to either a success or error handler.
        /// </summary>
        /// <typeparam name="T">The type of the return value from the handler functions.</typeparam>
        /// <param name="onSuccess">The function to handle a successful result.</param>
        /// <param name="onError">The function to handle a failed result.</param>
        /// <returns>The result of the matched handler function.</returns>
        public T Match<T>(Func<TValue, T> onSuccess, Func<TError, T> onError)
        {
            return IsSuccess ? onSuccess(Value) : onError(Err);
        }
    }
}
