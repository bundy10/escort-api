namespace Escort.Event.Domain.Models;

public class EventDetails : BaseDomainModel
{
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime BookingTime { get; set; }
    public 
    
 
    public EventDetails WithId()
    {
        this.Id = Guid.NewGuid();
        return this;
    }
}