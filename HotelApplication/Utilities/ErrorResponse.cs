using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Utilities
{
    public class ErrorResponse 
    {
        public bool IsSuccess { get; } = false;
        public IEnumerable<string> Errors { get; }
        public ErrorResponse(IEnumerable<string> errors)
        {
            Errors = errors;
        }

        public ErrorResponse(string error)
        {
            Errors = [error];
        }
    }
}