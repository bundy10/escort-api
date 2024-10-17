namespace Escort.Client.API.DTO;

public static class ClientMapper
{
    public static ClientGetDTO ToDto(this Domain.Models.Client client)
    {
        return new ClientGetDTO
        {
            Id = client.Id,
            ClientContactDetails = client.ClientContactDetails,
            FirstName = client.FirstName,
            LastName = client.LastName
        };
    }
    
    public static Domain.Models.Client ToDomain(this ClientPostPutDto clientDto)
    {
        return new Domain.Models.Client()
        {
            FirstName = clientDto.FirstName,
            LastName = clientDto.LastName,
            ClientContactDetails = clientDto.ClientContactDetails
        };
    }
}