using Escort.User.Application.Repositories;
using Escort.User.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Escort.User.Infrastructure.Repositories;

public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : BaseDomainModel
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _entities;

    public BaseRepository(DbContext context)
    {
        _context = context;
        _entities = context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await Task.Run(() => _entities.AsNoTracking());
    }

    public async Task<TEntity> GetAsync(int id)
    {
        var entity = await _entities.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id);

        if (entity == null) throw new ModelNotFoundException();

        return entity;
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        await _entities.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        await GetAsync(entity.Id);
        _entities.Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }
    
    public async Task<TEntity> GetByIdAsync(int id)
    {
        var entity = await _entities.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id);

        if (entity == null) throw new ModelNotFoundException();

        return entity;
    }
    
    public async Task<TEntity> DeleteAsync(int id)
    {
        var entityToDelete = await GetByIdAsync(id);
        _entities.Remove(entityToDelete);
        await _context.SaveChangesAsync();
        return entityToDelete;
    }

    public async Task<TEntity?> AuthenticateUserLoginAttempt(string username, string password)
    {
        return await _entities
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserName == username && user.Password == password);
    }
}