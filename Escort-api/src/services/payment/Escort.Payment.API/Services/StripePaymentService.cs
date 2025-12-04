using Stripe;

namespace Escort.Payment.API.Services;

public class StripePaymentService : IStripePaymentService
{
    private readonly PaymentIntentService _paymentIntentService;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(ILogger<StripePaymentService> logger)
    {
        _paymentIntentService = new PaymentIntentService();
        _logger = logger;
    }

    public async Task<string> AuthorizePaymentAsync(string bookingId, decimal amount, string currency)
    {
        try
        {
            // Convert amount to cents/smallest currency unit
            var amountInCents = (long)(amount * 100);

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = currency.ToLower(),
                CaptureMethod = "manual", // Authorize only, capture later
                TransferGroup = bookingId,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    { "booking_id", bookingId }
                }
            };

            var paymentIntent = await _paymentIntentService.CreateAsync(options);

            _logger.LogInformation(
                "Created PaymentIntent {PaymentIntentId} for booking {BookingId} with amount {Amount} {Currency}",
                paymentIntent.Id, bookingId, amount, currency);

            return paymentIntent.ClientSecret;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error occurred while creating payment intent for booking {BookingId}", bookingId);
            throw new InvalidOperationException($"Failed to create payment intent: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while creating payment intent for booking {BookingId}", bookingId);
            throw;
        }
    }

    public async Task<string> CapturePaymentAsync(string paymentIntentId)
    {
        try
        {
            _logger.LogInformation("Capturing payment for PaymentIntent {PaymentIntentId}", paymentIntentId);

            // Capture the previously authorized payment
            var paymentIntent = await _paymentIntentService.CaptureAsync(paymentIntentId);

            if (paymentIntent.Status != "succeeded")
            {
                _logger.LogWarning(
                    "Payment capture resulted in unexpected status {Status} for PaymentIntent {PaymentIntentId}",
                    paymentIntent.Status, paymentIntentId);
            }
            else
            {
                _logger.LogInformation(
                    "Successfully captured payment for PaymentIntent {PaymentIntentId}",
                    paymentIntentId);
            }

            return paymentIntent.Id;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error occurred while capturing payment for PaymentIntent {PaymentIntentId}", paymentIntentId);
            throw new InvalidOperationException($"Failed to capture payment: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while capturing payment for PaymentIntent {PaymentIntentId}", paymentIntentId);
            throw;
        }
    }
}

