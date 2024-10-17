namespace Escort.Driver.Domain.Models;

public class Driver : BaseDomainModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DriverContactDetails DriverContactDetails { get; set; }
}