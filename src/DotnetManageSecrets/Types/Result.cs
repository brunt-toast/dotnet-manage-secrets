using System;
using System.Collections.Generic;
using System.Text;
using Dev.JoshBrunton.DotnetManageSecrets.Enums.Enums;

namespace Dev.JoshBrunton.DotnetManageSecrets.Types;

internal class Result<T> : Result
{
    private T? _value;
    private int? _err;

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

    public static Result<T> Err(int err)
    {
        return new Result<T>
        {
            _err = err
        };
    }

    public static Result<T> Err(ExitCodes err)
    {
        return new Result<T>
        {
            _err = (int)err
        };
    }

    public T Unwrap()
    {
        if (_err is not null)
        {
            Environment.Exit(_err.Value);
        }

        return _value!;
    }
}

class Result
{

}