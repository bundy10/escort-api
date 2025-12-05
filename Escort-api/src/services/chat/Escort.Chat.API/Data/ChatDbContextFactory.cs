using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Escort.Chat.API.Data;

public class ChatDbContextFactory : IDesignTimeDbContextFactory<ChatDbContext>
{
    public ChatDbContext CreateDbContext(string[] args)
    {
        // Load .env file for connection string
        DotNetEnv.Env.Load();

        var optionsBuilder = new DbContextOptionsBuilder<ChatDbContext>();
        
        // Get connection string from environment variable
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback connection string for migration generation
            connectionString = "Host=localhost;Port=5432;Database=companiondb;Username=appuser;Password=123456789";
        }

        optionsBuilder.UseNpgsql(connectionString);

        return new ChatDbContext(optionsBuilder.Options);
    }
}

