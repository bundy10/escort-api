namespace Escort.Event.Domain.Models;

public class EventDetails
{
    public EventDetails(DateTime date, DateTime startTime, DateTime endTime, DateTime bookingTime, User.Domain.Models.User user, Client.Domain.Models.Client client, Driver.Domain.Models.Driver? driver, Listing.Domain.Models.Listing listing)
    {
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        BookingTime = bookingTime;
        User = user;
        Client = client;
        Driver = driver;
        Listing = listing;
    }


    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime BookingTime { get; set; }
    public User.Domain.Models.User User { get; set; }
    public Client.Domain.Models.Client Client { get; set; }
    public Driver.Domain.Models.Driver? Driver { get; set; } 
    public Listing.Domain.Models.Listing Listing { get; set; }
    
 
    public EventDetails WithId()
    {
        this.Id = Guid.NewGuid();
        return this;
    }
}