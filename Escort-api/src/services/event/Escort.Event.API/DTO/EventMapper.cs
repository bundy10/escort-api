namespace Escort.Event.API.DTO;

public static class EventMapper
{
    public static EventGetDTO ToDto(this Event.Domain.Models.Event @event)
    {
        return new EventGetDTO()
        {
            Id = @event.Id,
            UserId = @event.UserId,
            ClientId = @event.ClientId,
            DriverId = @event.DriverId,
            ListingId = @event.ListingId,
            Date = @event.Date,
            StartTime = @event.StartTime,
            EndTime = @event.EndTime,
            BookingTime = @event.BookingTime,
            Status = @event.Status
        };
    }

    public static Domain.Models.Event ToDomain(this EventPostPutDto eventDto)
    {
        return new Domain.Models.Event()
        {
            Status = eventDto.Status,
            Date = eventDto.Date,
            StartTime = eventDto.StartTime,
            EndTime = eventDto.EndTime,
            BookingTime = eventDto.BookingTime
        };
    }
}