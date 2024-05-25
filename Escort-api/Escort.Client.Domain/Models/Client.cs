namespace Escort.Client.Domain.Models;

public class Client : BaseDomainModel
{
    public ClientContactDetails ClientContactDetails { get; set; }

    public Client(ClientContactDetails clientContactDetails)
    {
        ClientContactDetails = clientContactDetails;
    }
}