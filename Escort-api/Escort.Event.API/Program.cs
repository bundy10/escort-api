using Escort.Event.Application.Repositories;
using Escort.Event.Application.Services;
using Escort.Event.Infrastructure.DBcontext;
using Escort.Event.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// repo/services
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();

// DbContext
builder.Services.AddScoped<DbContext, EventDbContext>();
builder.Services.AddDbContext<EventDbContext>(options =>
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