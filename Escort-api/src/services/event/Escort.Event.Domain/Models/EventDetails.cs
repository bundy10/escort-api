namespace Escort.Event.Domain.Models;

public class EventDetails : BaseDomainModel
{
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime BookingTime { get; set; }
    public User.Domain.Models.User User { get; set; }
    public Client.Domain.Models.Client Client { get; set; }
    public Driver.Domain.Models.Driver? Driver { get; set; } 
    public Listing.Domain.Models.Listing Listing { get; set; }
}