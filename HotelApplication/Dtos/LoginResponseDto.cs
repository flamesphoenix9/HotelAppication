using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos
{
    public class LoginResponseDto
    {
        public string AccessToken { get; init; } = string.Empty;
    }
}