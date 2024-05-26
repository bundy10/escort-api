using Escort.Listing.Application.Repositories;
using Escort.Listing.Application.Services;
using Escort.Listing.Infrastructure.DBcontext;
using Escort.Listing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// repo/services
builder.Services.AddScoped<IListingRepository, ListingRepository>();
builder.Services.AddScoped<IListingService, ListingService>();

// DbContext
builder.Services.AddScoped<DbContext, ListingDbContext>();
builder.Services.AddDbContext<ListingDbContext>(options =>
{
    options.UseInMemoryDatabase("EscortDatabase");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapControllers();

app.Run();