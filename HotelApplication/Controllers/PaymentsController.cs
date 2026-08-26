using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HotelApplication.Dtos.PaymentDtos;
using HotelApplication.Dtos.ReservationGroupDtos;
using HotelApplication.Models;
using HotelApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Allows Paystack to reach this endpoint
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IReservationGroupService _reservationGroupService;

    public PaymentsController(
        IPaymentService paymentService,
        IReservationGroupService reservationGroupService)
    {
        _paymentService = paymentService;
        _reservationGroupService = reservationGroupService;
    }

    [HttpPost("paystack-webhook")]
    public async Task<IActionResult> PaystackWebhook(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var requestBody = await reader.ReadToEndAsync(cancellationToken);
        var signature = Request.Headers["x-paystack-signature"].ToString();

        // Cryptographic Gate: Blocks fake/unauthorized webhook calls
        if (!_paymentService.VerifyWebhookSignature(requestBody, signature))
        {
            return Unauthorized();
        }

        var eventPayload = JsonSerializer.Deserialize<PaystackWebhookEvent>(requestBody);
        if (eventPayload?.Event == "charge.success" && eventPayload.Data  is not null && eventPayload?.Data.Status == "success")
        {
            await _reservationGroupService.ConfirmPaymentAsync(
                 eventPayload.Data.Reference, cancellationToken);
        }
        return Ok();
    }
    private sealed class PaystackWebhookEvent
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public PaystackWebhookData? Data { get; set; }
    }

    private sealed class PaystackWebhookData
    {
        [JsonPropertyName("reference")]
        public string Reference { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}