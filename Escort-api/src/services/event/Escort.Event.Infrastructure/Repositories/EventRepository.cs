using Escort.Event.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Escort.Event.Infrastructure.Repositories;

public class EventRepository : BaseRepository<Domain.Models.Event>, IEventRepository
{
    public EventRepository(DbContext context) : base(context)
    {
    }
}