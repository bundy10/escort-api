namespace Escort.Event.API.DTO;

public static class EventMapper
{
    public static EventGetDTO ToDto(this Event.Domain.Models.Event @event)
    {
        return new EventGetDTO()
        {
            Id = @event.Id,
            EventDetails = @event.EventDetails,
            Completed = @event.Completed
        };
    }

    public static Domain.Models.Event ToDomain(this EventPostPutDto eventDto)
    {
        return new Domain.Models.Event()
        {
            EventDetails = eventDto.EventDetails,
            Completed = eventDto.Completed
        };
    }
}