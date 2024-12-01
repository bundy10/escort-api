using Microsoft.EntityFrameworkCore;

namespace Escort.Event.Infrastructure.DBcontext;

public class EventDbContext : DbContext
{
    public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
    {
    }
    public DbSet<Domain.Models.Event> Events { get; set; }
}