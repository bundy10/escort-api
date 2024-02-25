using Escort.User.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Escort.User.Infrastructure.DBcontext;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {}
    
    public DbSet<Domain.Models.User> Users { get; set; }
    public DbSet<UserContactDetails> UserContactDetails { get; set; }
    public DbSet<UserVerificationDetails> UserVerificationDetails { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Models.User>()
            .HasOne(u => u.UserContactDetails)
            .WithOne()
            .HasForeignKey<UserContactDetails>(ucd => ucd.Id);

        modelBuilder.Entity<Domain.Models.User>()
            .HasOne(u => u.UserVerificationDetails)
            .WithOne()
            .HasForeignKey<UserVerificationDetails>(uvd => uvd.Id);
    }
}