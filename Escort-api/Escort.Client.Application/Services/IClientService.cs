namespace Escort.Client.Application.Services;

public interface IClientService
{
    Task<IEnumerable<Domain.Models.Client>> GetAllClientAsync();
    Task<Domain.Models.Client> GetClientByIdAsync(int id);
    Task<Domain.Models.Client> CreateClientAsync(Domain.Models.ClientContactDetails clientContactDetails);
    Task<Domain.Models.Client> UpdateClientAsync(Domain.Models.ClientContactDetails clientContactDetails);
    Task<Domain.Models.Client> DeleteClientAsync(int id);
}