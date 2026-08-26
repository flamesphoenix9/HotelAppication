using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos.PaymentDtos
{
    public record PaymentInitializeResponseDto
    {
        public string AuthorizationUrl { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
    }
}