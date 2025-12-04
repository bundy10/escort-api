namespace Escort.Worker.Payout.DTOs;

public class ReleaseFundsResponse
{
    public string BookingId { get; set; } = string.Empty;
    public string TransferId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

