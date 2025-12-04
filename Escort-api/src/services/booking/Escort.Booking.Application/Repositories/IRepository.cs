using Escort.Booking.Domain.Models;

namespace Escort.Booking.Application.Repositories;

public interface IRepository<T> where T : BaseDomainModel
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

