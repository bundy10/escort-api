namespace Escort.Driver.Domain.Models;

public class DriverContactDetails : BaseDomainModel
{
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
}