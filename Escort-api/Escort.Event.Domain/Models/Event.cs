namespace Escort.Event.Domain.Models;

public class Event : BaseDomainModel
{
    public EventDetails EventDetails { get; set; }
    public bool Completed { get; set; }
}