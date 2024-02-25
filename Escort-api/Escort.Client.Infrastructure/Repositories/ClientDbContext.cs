using Microsoft.EntityFrameworkCore;

namespace Escort.Client.Infrastructure.Repositories;

public class ClientDbContext : DbContext
{
    public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options)
    {
    }
    public DbSet<Domain.Models.Client> Clients { get; set; }
    public DbSet<Domain.Models.ClientContactDetails> ClientDetails { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Models.Client>()
            .HasOne(c => c.ClientContactDetails)
            .WithOne()
            .HasForeignKey<Domain.Models.ClientContactDetails>(cd => cd.Id);
    }
}