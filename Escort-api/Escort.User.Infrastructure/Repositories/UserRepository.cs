using Escort.User.Application.Repositories;
using Escort.User.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Escort.User.Infrastructure.Repositories;

public class UserRepository : BaseRepository<Domain.Models.User>, IUserRepository
{
    public UserRepository(DbContext context) : base(context)
    {
    }
}