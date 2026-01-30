using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

public class Result<T>
{
    private T? _value;

    public bool IsOk => _value is not null;
    public ErrorCodes? Error { get; private set; }

    private Result()
    {
    }

    public static Result<T> Ok(T t)
    {
        return new Result<T>
        {
            _value = t
        };
    }

    public static Result<T> Err(ErrorCodes err)
    {
        return new Result<T>
        {
            Error = err
        };
    }

    public T Unwrap()
    {
        if (Error is not null)
        {
            Environment.Exit((int)Error);
        }

        return _value!;
    }

    public static implicit operator Result<T>(T value)
    {
        return Ok(value);
    }

    public static implicit operator Result<T>(ErrorCodes value)
    {
        return Err(value);
    }
}
