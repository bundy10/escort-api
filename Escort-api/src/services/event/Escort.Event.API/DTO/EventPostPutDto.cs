using Escort.Event.Domain.Models;

namespace Escort.Event.API.DTO;

public class EventPostPutDto
{
    public string Status { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime BookingTime { get; set; }
    
}