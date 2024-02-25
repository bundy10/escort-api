using Microsoft.EntityFrameworkCore;

namespace Escort.Listing.Infrastructure.Repositories;

public class ListingDbContext : DbContext
{
    public ListingDbContext(DbContextOptions<ListingDbContext> options) : base(options)
    {
    }
    public DbSet<Domain.Models.Listing> Listings { get; set; }
    public DbSet<Domain.Models.ListingDetails> ListingDetails { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Models.Listing>()
            .HasOne(l => l.ListingDetails)
            .WithOne()
            .HasForeignKey<Domain.Models.ListingDetails>(ld => ld.Id);
    }
    
}