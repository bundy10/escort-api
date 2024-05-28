using Escort.Driver.Application.Repositories;
using Escort.Driver.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Escort.Driver.Infrastructure.Repositories;

public class DriverRepository: BaseRepository<Domain.Models.Driver>, IDriverRepository
{
    public DriverRepository(DbContext context) : base(context)
    {
    }
}