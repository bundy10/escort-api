using Escort.Booking.Domain.Enums;

namespace Escort.Booking.API.DTO;

public class BookingGetDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public int? DriverId { get; set; }
    public int ListingId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime BookingTime { get; set; }
    public BookingStatus Status { get; set; }
    public string? PaymentIntentId { get; set; }
}

