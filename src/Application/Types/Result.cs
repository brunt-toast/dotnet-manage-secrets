using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

public class Result<T>
{
    private T? _value;
    private ErrorCodes? _err;

    public bool IsOk => _value is not null;

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
            _err = err
        };
    }

    public T Unwrap()
    {
        if (_err is not null)
        {
            Environment.Exit((int)_err);
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
