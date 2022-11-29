using System;
using System.Collections.Generic;

/*
 * By Josef Ottosson, from https://github.com/joseftw/jos.result
 */
namespace RightToAskClient.Models
{
    public abstract class JOSResult
    {
        public bool Success { get; protected set; }
        public bool Failure => !Success;
    }

    public abstract class JOSResult<T> : JOSResult
    {
        private T _data;

        protected JOSResult(T data)
        {
            Data = data;
        }

        public T Data
        {
            get => Success ? _data : throw new Exception($"You can't access .{nameof(Data)} when .{nameof(Success)} is false");
            set => _data = value;
        }
    }

    public class SuccessResult : JOSResult
    {
        public SuccessResult()
        {
            Success = true;
        }
    }

    public class SuccessResult<T> : JOSResult<T>
    {
        public SuccessResult(T data) : base(data)
        {
            Success = true;
        }

        public static implicit operator SuccessResult(SuccessResult<T> successResult)
        {
            return new SuccessResult();
        }
    }

    public class ErrorResult : JOSResult, IErrorResult
    {
        
        public ErrorResult(string message) : this(message, Array.Empty<Error>())
        {
            
        }

        public ErrorResult(string message, IReadOnlyCollection<Error> errors)
        {
            Message = message;
            Success = false;
            Errors = errors ?? Array.Empty<Error>();
        }

        public string Message { get; }
        public IReadOnlyCollection<Error> Errors { get; }

        public virtual ErrorResult<T> ToGeneric<T>()
        {
            return new ErrorResult<T>(Message, Errors);
        }
    }

    public class ErrorResult<T> : JOSResult<T>, IErrorResult
    {
        public ErrorResult(string message) : this(message, Array.Empty<Error>())
        {
        }

        public ErrorResult(string message, IReadOnlyCollection<Error> errors) : base(default)
        {
            Message = message;
            Success = false;
            Errors = errors ?? Array.Empty<Error>();
        }

        public string Message { get; set; }
        public IReadOnlyCollection<Error> Errors { get; }

        public static implicit operator ErrorResult(ErrorResult<T> errorResult)
        {
            return new ErrorResult(errorResult.Message, errorResult.Errors);
        }

        public virtual ErrorResult<TType> ToGeneric<TType>()
        {
            return new ErrorResult<TType>(Message, Errors);
        }
    }

    // TODO It's possible that we should have a variety of context-specific error results, e.g. for Geoscape / RTA server errors.
    public class ServerUnreachableErrorResult : ErrorResult
    {
        public ServerUnreachableErrorResult(string message) : base(message)
        {
        }

        public ServerUnreachableErrorResult(string message, IReadOnlyCollection<Error> errors) : base(message, errors)
        {
        }
    }

    public class ServerUnreachableErrorResult<T> : ErrorResult
    {
        public ServerUnreachableErrorResult(string message) : base(message)
        {
        }
        
        public ServerUnreachableErrorResult(string message, IReadOnlyCollection<Error> errors) : base(message, errors)
        {
        }
    }

    public class Error
    {
        public Error(string details) : this(null, details)
        {
        }

        public Error(string code, string details)
        {
            Code = code;
            Details = details;
        }

        public string Code { get; }
        public string Details { get; }
    }

        
    internal interface IErrorResult
    {
        string Message { get; }
        IReadOnlyCollection<Error> Errors { get; }
    }
}
