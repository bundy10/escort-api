namespace Escort.Payment.API.Services;

public interface IStripePaymentService
{
    Task<string> AuthorizePaymentAsync(string bookingId, decimal amount, string currency);
    Task<string> CapturePaymentAsync(string paymentIntentId);
}

