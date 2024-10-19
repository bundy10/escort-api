using Escort.Event.Application.Repositories;
using Escort.Event.Application.Services;
using Escort.Event.Infrastructure.DBcontext;
using Escort.Event.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Escort.Event.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    corsPolicyBuilder => corsPolicyBuilder.WithOrigins("http://localhost:8085")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            // repo/services
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IEventService, EventService>();

            // DbContext
            builder.Services.AddScoped<DbContext, EventDbContext>();
            builder.Services.AddDbContext<EventDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Escort.Event.Infrastructure"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Escort.User.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}