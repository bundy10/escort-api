using Escort.Driver.Application.Repositories;
using Escort.Driver.Application.Services;
using Escort.Driver.Infrastructure.DBcontext;
using Escort.Driver.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Escort.Driver.API
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

            // repo/services
            builder.Services.AddScoped<IDriverRepository, DriverRepository>();
            builder.Services.AddScoped<IDriverService, DriverService>();

            // DbContext
            builder.Services.AddScoped<DbContext, DriverDbContext>();
            builder.Services.AddDbContext<DriverDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Escort.Driver.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.MapControllers();

            app.Run();
        }
    }
}