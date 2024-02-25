using Escort.Listing.Application.Repositories;
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

    public async Task<TEntity> GetAsync(Guid id)
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
    
    public async Task<TEntity> GetByIdAsync(Guid id)
    {
        var entity = await _entities.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id);

        if (entity == null) throw new ModelNotFoundException();

        return entity;
    }
    
    public async Task<TEntity> DeleteAsync(Guid id)
    {
        var entityToDelete = await GetByIdAsync(id);
        _entities.Remove(entityToDelete);
        await _context.SaveChangesAsync();
        return entityToDelete;
    }
}