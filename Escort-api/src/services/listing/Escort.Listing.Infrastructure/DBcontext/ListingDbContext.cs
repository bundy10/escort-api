using Microsoft.EntityFrameworkCore;

namespace Escort.Listing.Infrastructure.DBcontext;

public class ListingDbContext : DbContext
{
    public ListingDbContext(DbContextOptions<ListingDbContext> options) : base(options)
    {
    }
    public DbSet<Domain.Models.Listing> Listings { get; set; }
    public DbSet<Domain.Models.ListingDetails> ListingDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enable PostGIS extension
        modelBuilder.HasPostgresExtension("postgis");
        
        modelBuilder.Entity<Domain.Models.Listing>()
            .HasOne(l => l.ListingDetails)
            .WithOne()
            .HasForeignKey<Domain.Models.ListingDetails>(ld => ld.ListingId);
        
        // Configure the Location property as a PostGIS geometry column with SRID 4326 (WGS84)
        modelBuilder.Entity<Domain.Models.Listing>()
            .Property(l => l.Location)
            .HasColumnType("geometry (point, 4326)");
        
        // Create a spatial index on the Location column for better query performance
        modelBuilder.Entity<Domain.Models.Listing>()
            .HasIndex(l => l.Location)
            .HasMethod("gist");
    }
}