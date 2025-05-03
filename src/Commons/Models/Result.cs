using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Models
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Message { get; }

        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string? message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static Result Success(string message ="") => new Result(true,message);
        public static Result Failure(string errorMessage) => new Result(false, errorMessage);
    }


    public class Result<T> : Result
    {
        public T Value { get; }

        private Result(T value, string? message = null) : base(true, message)
        {
            Value = value;
        }

        private Result(string errorMessage) : base(false, errorMessage)
        {
            Value = default!;
        }

        public static Result<T> Success(T value, string message = "") => new Result<T>(value, message);
        public static new Result<T> Failure(string errorMessage) => new Result<T>(errorMessage);
        public static Result<T> Success() => new Result<T>(default!);
    }

}
