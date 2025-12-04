namespace Escort.Worker.Payout.Services;

public interface IPayoutProcessingService
{
    Task ProcessPendingPayoutsAsync(CancellationToken cancellationToken);
}

