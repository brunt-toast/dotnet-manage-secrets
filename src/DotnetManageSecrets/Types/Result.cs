using System;
using System.Collections.Generic;
using System.Text;

namespace Dev.JoshBrunton.DotnetManageSecrets.Types;

internal class Result<T> : Result
{
    private T? _value;
    private int? _err;

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