using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Utilities
{
    public class Response<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public IEnumerable<string> Errors { get; }

        private Response(bool isSuccess, T? data, IEnumerable<string> errors)
        {
            IsSuccess = isSuccess;
            Data = data;
            Errors = errors;
        }

        public static Response<T> Success(T? data = default) =>
            new(true, data, Array.Empty<string>());
        public static Response<T?> Failure(string error) =>
            new(false, default, new List<string>(){error});
        public static Response<T> Failure(IEnumerable<string> errors) =>
            new(false, default, errors);
    }
}