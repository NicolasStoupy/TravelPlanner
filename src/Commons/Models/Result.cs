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

        private Result(bool isSuccess, string? message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static Result Success(string message ="") => new Result(true,message);
        public static Result Failure(string errorMessage) => new Result(false, errorMessage);
    }

}
