﻿using Escort.Booking.Infrastructure.DBcontext;
using Escort.Worker.Payout.Services;
using Escort.Worker.Payout.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DotNetEnv;

// Load .env file from the root directory (3 levels up from the worker)
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine($"Loaded .env file from: {envPath}");
}
else
{
    Console.WriteLine($".env file not found at: {envPath}");
}

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.AddConsole();

// Add configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure database context
// Priority: 1. Environment variable from .env, 2. appsettings.json
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "DefaultConnection connection string is not configured. " +
        "Please set ConnectionStrings__DefaultConnection in .env file or ConnectionStrings:DefaultConnection in appsettings.json");
}

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add HTTP client for calling Payment API
builder.Services.AddHttpClient();

// Register services
builder.Services.AddScoped<IPayoutProcessingService, PayoutProcessingService>();

// Register the hosted service
builder.Services.AddHostedService<PayoutWorker>();

var host = builder.Build();

// Run the worker
await host.RunAsync();

