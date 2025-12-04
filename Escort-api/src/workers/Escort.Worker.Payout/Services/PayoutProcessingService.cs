using System.Text;
using System.Text.Json;
using Escort.Booking.Domain.Enums;
using Escort.Booking.Infrastructure.DBcontext;
using Escort.Worker.Payout.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Escort.Worker.Payout.Services;

public class PayoutProcessingService : IPayoutProcessingService
{
    private readonly BookingDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PayoutProcessingService> _logger;

    public PayoutProcessingService(
        BookingDbContext dbContext,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<PayoutProcessingService> logger)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ProcessPendingPayoutsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting payout processing at {Time}", DateTime.UtcNow);

            // Query for bookings that need payout processing
            var pendingPayouts = await _dbContext.Bookings
                .Where(b => 
                    b.Status == BookingStatus.Completed &&
                    b.PayoutDueAt.HasValue &&
                    b.PayoutDueAt.Value < DateTime.UtcNow &&
                    b.PayoutProcessed == false)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Found {Count} bookings pending payout", pendingPayouts.Count);

            foreach (var booking in pendingPayouts)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Payout processing cancelled");
                    break;
                }

                await ProcessBookingPayoutAsync(booking, cancellationToken);
            }

            _logger.LogInformation("Completed payout processing at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing payouts");
            throw;
        }
    }

    private async Task ProcessBookingPayoutAsync(
        Escort.Booking.Domain.Models.Booking booking, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Processing payout for booking {BookingId} for driver {DriverId}",
                booking.Id, booking.DriverId);

            // Validate booking has required information
            if (!booking.DriverId.HasValue)
            {
                _logger.LogWarning(
                    "Booking {BookingId} has no driver assigned, skipping payout",
                    booking.Id);
                return;
            }

            if (string.IsNullOrEmpty(booking.PaymentIntentId))
            {
                _logger.LogWarning(
                    "Booking {BookingId} has no payment intent, skipping payout",
                    booking.Id);
                return;
            }

            // TODO: Calculate amount - this should come from your business logic
            // For now, using a placeholder. You'll need to implement proper amount calculation
            // based on booking details, pricing, commission, etc.
            var amount = CalculatePayoutAmount(booking);

            // TODO: Get driver's Stripe account ID from your database
            // This is a placeholder - you need to retrieve the actual Stripe account ID
            // from your User/Driver service or database
            var destinationAccountId = $"acct_driver_{booking.DriverId}";

            // Call Payment API to release funds
            var releaseFundsRequest = new ReleaseFundsRequest
            {
                BookingId = booking.Id.ToString(),
                DestinationAccountId = destinationAccountId,
                Amount = amount
            };

            var success = await CallPaymentApiAsync(releaseFundsRequest, cancellationToken);

            if (success)
            {
                // Mark booking as payout processed
                booking.PayoutProcessed = true;
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Successfully processed payout for booking {BookingId}",
                    booking.Id);
            }
            else
            {
                _logger.LogError(
                    "Failed to process payout for booking {BookingId}",
                    booking.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing payout for booking {BookingId}",
                booking.Id);
            // Don't rethrow - continue processing other bookings
        }
    }

    private async Task<bool> CallPaymentApiAsync(
        ReleaseFundsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var baseUrl = _configuration["PaymentAPI:BaseUrl"];
            var endpoint = _configuration["PaymentAPI:ReleaseFundsEndpoint"];
            var fullUrl = $"{baseUrl}{endpoint}";

            _logger.LogInformation(
                "Calling Payment API at {Url} for booking {BookingId}",
                fullUrl, request.BookingId);

            var httpClient = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(fullUrl, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<ReleaseFundsResponse>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation(
                    "Payment API returned success for booking {BookingId}. Transfer ID: {TransferId}",
                    request.BookingId, result?.TransferId);

                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "Payment API returned error {StatusCode} for booking {BookingId}: {Error}",
                    response.StatusCode, request.BookingId, errorContent);

                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception calling Payment API for booking {BookingId}",
                request.BookingId);
            return false;
        }
    }

    private long CalculatePayoutAmount(Escort.Booking.Domain.Models.Booking booking)
    {
        // TODO: Implement proper amount calculation based on your business logic
        // This should consider:
        // - Booking duration (StartTime to EndTime)
        // - Listing price per hour
        // - Platform commission
        // - Any other fees or adjustments
        
        // Placeholder: Return a default amount (e.g., $100.00 = 10000 cents)
        _logger.LogWarning(
            "Using placeholder amount calculation for booking {BookingId}. Implement proper calculation!",
            booking.Id);
        
        return 10000; // $100.00 in cents
    }
}

