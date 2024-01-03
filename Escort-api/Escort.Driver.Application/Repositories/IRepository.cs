using Escort.Driver.Domain.Models;

namespace Escort.Driver.Application.Repositories;

public interface IRepository <TEntity> where TEntity : BaseDomainModel

{
    Task<IEnumerable<TEntity>> GetAllAsync(); 
    Task<TEntity> GetByIdAsync(Guid id);
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync(Guid id);
}