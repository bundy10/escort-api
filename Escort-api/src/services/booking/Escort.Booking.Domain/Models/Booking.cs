using Escort.Booking.Domain.Enums;

namespace Escort.Booking.Domain.Models;

public class Booking : BaseDomainModel
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime BookingTime { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? PaymentIntentId { get; set; }
    public DateTime? PayoutDueAt { get; set; }
    public bool PayoutProcessed { get; set; } = false;
}

