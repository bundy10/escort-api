using Escort.User.Application.Repositories;
using Escort.User.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Escort.User.Infrastructure.Repositories;

public class UserRepository : BaseRepository<Domain.Models.User>, IUserRepository
{
    public UserRepository(DbContext context) : base(context)
    {
    }
    public async Task<Domain.Models.User> GetUserWithListingsAsync(Guid userId)
    {
        var user = await _entities.Include(u => u.Listings).SingleOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (user.Listings == null || !user.Listings.Any())
        {
            throw new Exception("User has no listings");
        }

        return user;
    }
}