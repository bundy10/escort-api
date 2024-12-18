namespace Escort.Event.Application.Services;

public interface IEventService
{
    Task<IEnumerable<Domain.Models.Event>> GetAllAsync();
    Task<Domain.Models.Event> GetByIdAsync(int id);
    Task<Domain.Models.Event> CreateAsync(Domain.Models.Event entity);
    Task<Domain.Models.Event> UpdateAsync(Domain.Models.Event entity);
    Task<Domain.Models.Event> DeleteAsync(int id);
    Task<IEnumerable<Domain.Models.Event>> GetByUserIdAsync(int id);
}