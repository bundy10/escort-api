using Escort.Event.Domain.Models;

namespace Escort.Event.Application.Repositories;

public interface IRepository <TEntity> where TEntity : BaseDomainModel

{
    Task<IEnumerable<TEntity>> GetAllAsync(); 
    Task<TEntity> GetByIdAsync(Guid id);
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync(Guid id);
}