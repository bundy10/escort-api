namespace Escort.Client.Domain.Models;

public class ClientContactDetails: BaseDomainModel
{
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
}