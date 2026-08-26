using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos.PaymentDtos
{
    public record PaymentInitializeRequest(
        string Email,
        decimal Amount,
        string Reference,
        string? CallbackUrl = null
    );
    public record PaymentInitializeResult(
        bool IsSuccess,
        string? AuthorizationUrl,
        string? AccessCode,
        string? Reference,
        string? ErrorMessage
    );
    public record PaymentVerifyResult(
        bool IsSuccess,
        string Reference,
        decimal Amount,
        string Status,
        string? GatewayResponse
    );
}