using Escort.Event.Application.Repositories;
using Escort.Event.Domain.Models;

namespace Escort.Event.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<Domain.Models.Event> CreateAsync(Domain.Models.Event eventT)
    {
        return await _eventRepository.CreateAsync(eventT);
    }
    
    public async Task<IEnumerable<Domain.Models.Event>> GetAllAsync()
    {
        return await _eventRepository.GetAllAsync();
    }
    
    public async Task<Domain.Models.Event> GetByIdAsync(int id)
    {
        return await _eventRepository.GetByIdAsync(id);
    }
    
    public async Task<Domain.Models.Event> UpdateAsync(Domain.Models.Event eventT)
    {
        return await _eventRepository.UpdateAsync(eventT);
    }
    
    public async Task<Domain.Models.Event> DeleteAsync(int id)
    {
        return await _eventRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Domain.Models.Event>> GetByUserIdAsync(int id)
    {
        return await _eventRepository.GetByUserIdAsync(id);
    }
}