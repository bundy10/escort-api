namespace Escort.Driver.Domain.Models;

public class Driver : BaseDomainModel
{
    public DriverContactDetails DriverContactDetails { get; set; }

    public Driver(DriverContactDetails driverContactDetails)
    {
        DriverContactDetails = driverContactDetails;
    }
}