using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HotelApplication.Dtos.PaymentDtos;

namespace HotelApplication.Services
{
    public class PaystackPaymentService(HttpClient httpClient,
                                        IConfiguration configuration,
                                        ILogger<PaystackPaymentService> logger) : IPaymentService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _secretKey = configuration["Paystack:SecretKey"] ?? throw new InvalidOperationException("PayStack secret key is missing");
        private readonly ILogger<PaystackPaymentService> _logger = logger;

        public async Task<PaymentInitializeResult> InitializePaymentAsync(PaymentInitializeRequest paymentInitializeRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var payload = new
                {
                    email = paymentInitializeRequest.Email,
                    amount = (long)(paymentInitializeRequest.Amount * 100),
                    reference = paymentInitializeRequest.Reference,
                    callback_url = paymentInitializeRequest.CallbackUrl
                };
                var response = await _httpClient.PostAsJsonAsync("transaction/initialize", payload, cancellationToken);
                var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(rawResponse))
                {
                    _logger.LogError("Paystack initialization failed. Status: {StatusCode}, Body: {Body}",
                        response.StatusCode, rawResponse);

                    return new PaymentInitializeResult(
                        IsSuccess: false,
                        AuthorizationUrl: null,
                        AccessCode: null,
                        Reference: paymentInitializeRequest.Reference,
                        ErrorMessage: $"Payment initialization failed (Status: {response.StatusCode}): {rawResponse}"
                    );
                }
                var content = JsonSerializer.Deserialize<PayStackApiResponse<PaystackInitData>>(rawResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (content?.Status == true && content.Data != null)
                {
                    return new PaymentInitializeResult
                    (
                        IsSuccess: true,
                        AuthorizationUrl: content.Data.AuthorizationUrl,
                        AccessCode: content.Data.AccessCode,
                        Reference: content.Data.Reference,
                        ErrorMessage: null
                    );
                }
                return new PaymentInitializeResult(
                    IsSuccess: false,
                    AuthorizationUrl: null,
                    AccessCode: null,
                    Reference: paymentInitializeRequest.Reference,
                    ErrorMessage: content?.Message ?? "Unable to initialize transaction with Paystack."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Initializing Paystack transaction {paymentInitializeRequest.Reference} ");
                return new PaymentInitializeResult(false, null, null, paymentInitializeRequest.Reference, ex.Message);
            }
        }

        public async Task<PaymentVerifyResult> VerifyPaymentAsync(string reference, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<PayStackApiResponse<PaystackVerifyData>>($"transaction/verify/{Uri.EscapeDataString(reference)}", cancellationToken);

                if (response?.Status == true && response.Data?.Status == "success")
                {
                    return new PaymentVerifyResult(IsSuccess: true, Reference: response.Data.Reference, Amount: response.Data.Amount / 100m, Status: response.Data.Status, GatewayResponse: response.Data?.gateway_response);
                }
                return new PaymentVerifyResult(false, reference, 0, response?.Data?.Status ?? "failed", response?.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying Paystack transaction reference{reference}");
                return new PaymentVerifyResult(false, reference, 0, "error", ex.Message);
            }
        }

        public bool VerifyWebhookSignature(string requestBody, string paystackHeaderSignature)
        {
            if (string.IsNullOrWhiteSpace(paystackHeaderSignature) || string.IsNullOrWhiteSpace(requestBody)) return false;
            byte[] keyBytes = Encoding.UTF8.GetBytes(_secretKey);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(requestBody);
            using var hmac = new HMACSHA512(keyBytes);
            byte[] hash = hmac.ComputeHash(bodyBytes);
            string calculatedSignature = Convert.ToHexString(hash).ToLowerInvariant();
            return String.Equals(paystackHeaderSignature, calculatedSignature, StringComparison.OrdinalIgnoreCase);
        }

        private sealed class PayStackApiResponse<T>
        {
            [JsonPropertyName("status")] public bool Status { get; set; }
            [JsonPropertyName("message")] public string? Message { get; set; }
            [JsonPropertyName("data")] public T? Data { get; set; }
        }
        private sealed class PaystackInitData
        {
            [JsonPropertyName("authorization_url")] public string AuthorizationUrl { get; set; } = string.Empty;
            [JsonPropertyName("access_code")] public string AccessCode { get; set; } = string.Empty;
            [JsonPropertyName("reference")] public string Reference { get; set; } = string.Empty;
        }
        private sealed class PaystackVerifyData
        {
            [JsonPropertyName("reference")] public string Reference { get; set; } = string.Empty;
            [JsonPropertyName("amount")] public decimal Amount { get; set; }
            [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
            [JsonPropertyName("gateway_response")] public string? gateway_response { get; set; }
        }
    }
}