using Stripe;

namespace Escort.Payment.API.Services;

public class PayoutService : IPayoutService
{
    private readonly TransferService _transferService;
    private readonly BalanceService _balanceService;
    private readonly ILogger<PayoutService> _logger;

    public PayoutService(ILogger<PayoutService> logger)
    {
        _transferService = new TransferService();
        _balanceService = new BalanceService();
        _logger = logger;
    }

    public async Task<string> ReleaseFundsAsync(string bookingId, string destinationAccountId, long amount)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(bookingId))
            {
                throw new ArgumentException("BookingId cannot be null or empty", nameof(bookingId));
            }

            if (string.IsNullOrWhiteSpace(destinationAccountId))
            {
                throw new ArgumentException("DestinationAccountId cannot be null or empty", nameof(destinationAccountId));
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));
            }

            _logger.LogInformation(
                "Attempting to release funds for booking {BookingId} to account {AccountId} with amount {Amount}",
                bookingId, destinationAccountId, amount);

            // Check platform balance before attempting transfer
            var balance = await _balanceService.GetAsync();
            var availableBalance = balance.Available.FirstOrDefault(b => b.Currency == "usd");

            if (availableBalance == null || availableBalance.Amount < amount)
            {
                var availableAmount = availableBalance?.Amount ?? 0;
                _logger.LogWarning(
                    "Insufficient balance for transfer. Available: {Available}, Required: {Required}",
                    availableAmount, amount);
                
                throw new InvalidOperationException(
                    $"Insufficient platform balance. Available: {availableAmount / 100.0:C}, Required: {amount / 100.0:C}");
            }

            // Create transfer to connected account
            var transferOptions = new TransferCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                Destination = destinationAccountId,
                TransferGroup = bookingId, // Match the transfer_group from PaymentIntent
                Description = $"Payout for booking {bookingId}",
                Metadata = new Dictionary<string, string>
                {
                    { "booking_id", bookingId },
                    { "transfer_type", "service_payout" }
                }
            };

            var transfer = await _transferService.CreateAsync(transferOptions);

            _logger.LogInformation(
                "Successfully created transfer {TransferId} for booking {BookingId} to account {AccountId}",
                transfer.Id, bookingId, destinationAccountId);

            return transfer.Id;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, 
                "Stripe error occurred while releasing funds for booking {BookingId} to account {AccountId}", 
                bookingId, destinationAccountId);

            // Handle specific Stripe error codes
            switch (ex.StripeError?.Code)
            {
                case "insufficient_funds":
                    throw new InvalidOperationException(
                        "Insufficient funds in platform account to complete the transfer", ex);
                
                case "account_invalid":
                    throw new InvalidOperationException(
                        $"Invalid destination account: {destinationAccountId}", ex);
                
                case "transfer_group_invalid":
                    throw new InvalidOperationException(
                        $"Invalid transfer group: {bookingId}", ex);
                
                default:
                    throw new InvalidOperationException(
                        $"Failed to release funds: {ex.StripeError?.Message ?? ex.Message}", ex);
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument provided for fund release");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation during fund release");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Unexpected error occurred while releasing funds for booking {BookingId}", 
                bookingId);
            throw;
        }
    }
}

