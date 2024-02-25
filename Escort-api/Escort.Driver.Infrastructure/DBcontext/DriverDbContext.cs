using Microsoft.EntityFrameworkCore;

namespace Escort.Driver.Infrastructure.DBcontext;

public class DriverDbContext : DbContext
{
    public DriverDbContext(DbContextOptions<DriverDbContext> options) : base(options)
    {
    }
    public DbSet<Domain.Models.Driver> Drivers { get; set; }
    public DbSet<Domain.Models.DriverContactDetails> DriverDetails { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Models.Driver>()
            .HasOne(d => d.DriverContactDetails)
            .WithOne()
            .HasForeignKey<Domain.Models.DriverContactDetails>(dd => dd.Id);
    }
}