public class Result<TValue, TError>
{
    public bool IsSuccess { get; }
    public TValue Value { get; private set; }
    public TError Err { get; private set; }

    private Result(bool isSuccess, TValue value, TError error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Err = error;
    }

    public static Result<TValue, TError> Success(TValue value)
    {
        return new Result<TValue, TError>(true, value, default);
    }

    public static Result<TValue, TError> Error(TError error)
    {
        return new Result<TValue, TError>(false, default, error);
    }

    public T Match<T>(Func<TValue, T> onSuccess, Func<TError, T> onError)
    {
        return IsSuccess ? onSuccess(Value) : onError(Err);
    }
}
