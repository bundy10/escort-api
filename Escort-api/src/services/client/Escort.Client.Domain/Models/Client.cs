namespace Escort.Client.Domain.Models;

public class Client : BaseDomainModel
{
    public ClientContactDetails ClientContactDetails { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}