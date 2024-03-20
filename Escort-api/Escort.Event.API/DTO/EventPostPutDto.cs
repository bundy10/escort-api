using Escort.Event.Domain.Models;

namespace Escort.Event.API.DTO;

public class EventPostPutDto
{
    public EventDetails EventDetails { get; set; }
    public bool Completed { get; set; }
}