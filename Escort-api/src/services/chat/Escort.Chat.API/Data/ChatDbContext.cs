using Escort.Chat.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Escort.Chat.API.Data;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
    }

    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BookingId).IsRequired();
            entity.Property(e => e.SenderId).IsRequired().HasMaxLength(255);
            entity.Property(e => e.MessageText).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            
            // Add index for faster queries by BookingId
            entity.HasIndex(e => e.BookingId);
            entity.HasIndex(e => e.Timestamp);
        });
    }
}

