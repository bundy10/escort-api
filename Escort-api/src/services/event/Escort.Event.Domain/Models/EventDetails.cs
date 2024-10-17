namespace Escort.Event.Domain.Models;

public class EventDetails : BaseDomainModel
{
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime BookingTime { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public int? DriverId { get; set; } 
    public int ListingId { get; set; }
}