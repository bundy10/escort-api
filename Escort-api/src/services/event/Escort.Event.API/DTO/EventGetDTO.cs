namespace Escort.Event.API.DTO;

public class EventGetDTO : EventPostPutDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public int? DriverId { get; set; }
    
    public int ListingId { get; set; }
    
}