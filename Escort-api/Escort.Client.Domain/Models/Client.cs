namespace Escort.Client.Domain.Models;

public class Client : BaseDomainModel
{
    public ClientContactDetails ClientContactDetails { get; set; }

    public Client(ClientContactDetails clientContactDetails)
    {
        ClientContactDetails = clientContactDetails;
    }
    public Client WithId()
    {
        this.Id = Guid.NewGuid();
        return this;
    }
}