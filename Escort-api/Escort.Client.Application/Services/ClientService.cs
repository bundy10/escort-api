using Escort.Client.Application.Repositories;

namespace Escort.Client.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<IEnumerable<Domain.Models.Client>> GetAllClientAsync()
    {
        return await _clientRepository.GetAllAsync();
    }

    public async Task<Domain.Models.Client> GetClientByIdAsync(int id)
    {
        return await _clientRepository.GetByIdAsync(id);
    }

    public async Task<Domain.Models.Client> CreateClientAsync(Domain.Models.ClientContactDetails clientContactDetails)
    {
        var client = new Domain.Models.Client();
        return await _clientRepository.CreateAsync(client);
    }

    public async Task<Domain.Models.Client> UpdateClientAsync(Domain.Models.ClientContactDetails clientContactDetails)
    {
        var client = new Domain.Models.Client();
        return await _clientRepository.UpdateAsync(client);
    }

    public async Task<Domain.Models.Client> DeleteClientAsync(int id)
    {
        return await _clientRepository.DeleteAsync(id);
    }
}