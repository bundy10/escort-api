using Escort.Driver.Domain.Models;

namespace Escort.Driver.API.DTO;

public class DriverPostPutDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DriverContactDetails DriverContactDetails { get; set; }
}