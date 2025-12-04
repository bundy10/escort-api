namespace Escort.Booking.Domain.Models;

public class BaseDomainModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public int? DriverId { get; set; } 
    public int ListingId { get; set; }
}

