namespace Escort.Payment.API.Services;

public interface IPayoutService
{
    Task<string> ReleaseFundsAsync(string bookingId, string destinationAccountId, long amount);
}

