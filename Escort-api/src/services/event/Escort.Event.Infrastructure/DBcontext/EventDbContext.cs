using Microsoft.EntityFrameworkCore;

namespace Escort.Event.Infrastructure.DBcontext;

public class EventDbContext : DbContext
{
    public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
    {
    }
    public DbSet<Domain.Models.Event> Events { get; set; }
    public DbSet<Domain.Models.EventDetails> EventDetails { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Models.Event>()
            .HasOne(e => e.EventDetails)
            .WithOne()
            .HasForeignKey<Domain.Models.EventDetails>(ed => ed.Id);
    }
}