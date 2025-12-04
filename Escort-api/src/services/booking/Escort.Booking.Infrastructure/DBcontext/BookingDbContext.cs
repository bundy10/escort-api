using Escort.Booking.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Escort.Booking.Infrastructure.DBcontext;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<Booking.Domain.Models.Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking.Domain.Models.Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.PaymentIntentId).HasMaxLength(255);
        });
    }
}

