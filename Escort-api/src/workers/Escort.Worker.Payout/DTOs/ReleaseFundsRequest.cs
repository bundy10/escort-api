namespace Escort.Worker.Payout.DTOs;

public class ReleaseFundsRequest
{
    public string BookingId { get; set; } = string.Empty;
    public string DestinationAccountId { get; set; } = string.Empty;
    public long Amount { get; set; }
}

