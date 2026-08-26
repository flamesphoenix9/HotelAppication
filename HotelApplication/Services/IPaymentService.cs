using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Dtos.PaymentDtos;

namespace HotelApplication.Services
{
    public interface IPaymentService
    {
        Task<PaymentInitializeResult> InitializePaymentAsync(PaymentInitializeRequest paymentInitializeRequest, CancellationToken cancellationToken = default);
        Task<PaymentVerifyResult> VerifyPaymentAsync(string reference, CancellationToken cancellationToken = default);
        bool VerifyWebhookSignature(string requestBody, string paystackHeaderSignature); 
    }
}