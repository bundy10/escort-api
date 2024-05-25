using Escort.User.Domain.Models;

namespace Escort.User.Application.Repositories;

public interface IRepository<TEntity> where TEntity : BaseDomainModel
{
    Task<IEnumerable<TEntity>> GetAllAsync(); 
    Task<TEntity> GetByIdAsync(int id);
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync(int id);
}