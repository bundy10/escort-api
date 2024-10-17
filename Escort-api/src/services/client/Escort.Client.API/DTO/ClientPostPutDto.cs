using Escort.Client.Domain.Models;

namespace Escort.Client.API.DTO;

public class ClientPostPutDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public ClientContactDetails ClientContactDetails { get; set; }
}