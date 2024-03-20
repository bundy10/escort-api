using Escort.Client.API.DTO;
using Escort.Client.Application.Repositories;
using Escort.Client.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Escort.Client.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController : Controller
{
    private readonly IClientRepository _clientRepository;
    
    public ClientController(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
        var clients = await _clientRepository.GetAllAsync();
        return Ok(clients.Select(client => client.ToDto()));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetClientById(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        return Ok(client.ToDto());
    }
    
    [HttpPost]
    public async Task<ActionResult<IEnumerable<ClientGetDTO>>> CreateClient(ClientPostPutDto clientPostPutDto)
    {
        var client = await _clientRepository.CreateAsync(clientPostPutDto.ToDomain());
        return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client.ToDto());
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClientGetDTO>> UpdateClient(Guid id, [FromBody] ClientPostPutDto clientPostPutDto)
    {
        {
            try
            {
                var client = clientPostPutDto.ToDomain();
                client.Id = id;
                await _clientRepository.UpdateAsync(client);
                var clientGetDto = client.ToDto();

                return Ok(clientGetDto);
            }
            catch (ModelNotFoundException)
            {
                return NotFound();
            }
        }
    }
}