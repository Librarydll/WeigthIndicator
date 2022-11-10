using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Domain
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public static Result CreateError(string errorMessage = null) => new Result { ErrorMessage = errorMessage };
    }
    public class Result<T> : Result
    {
        public T Value { get; set; }
        public static Result<T> CreateSuccess(T value) => new Result<T>
        {
            IsSuccess = true,
            Value = value
        };
    }
}
