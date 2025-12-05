using Escort.Chat.API.Hubs;
using Escort.Chat.API.Services;
using Escort.Chat.API.Data;
using Escort.Chat.API.Repositories;
using Microsoft.EntityFrameworkCore;

// Load environment variables from .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database context
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "DefaultConnection connection string is not configured. " +
        "Please set ConnectionStrings__DefaultConnection in .env file or ConnectionStrings:DefaultConnection in appsettings.json");
}

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add SignalR
builder.Services.AddSignalR();

// Add HttpClient for calling other services
builder.Services.AddHttpClient();

// Register services
builder.Services.AddScoped<IBookingValidationService, BookingValidationService>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        corsPolicyBuilder => corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Escort.Chat.API v1"));
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Map SignalR hub
app.MapHub<ChatHub>("/chatHub");

app.Run();

