namespace Escort.Event.Domain.Models;

public class Event : BaseDomainModel
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime BookingTime { get; set; }
    public string Status { get; set; } = "Pending";
}