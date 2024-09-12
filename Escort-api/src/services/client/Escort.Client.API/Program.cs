using Escort.Client.Application.Repositories;
using Escort.Client.Application.Services;
using Escort.Client.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Escort.Client.API
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
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IClientService, ClientService>();

            // DbContext
            builder.Services.AddScoped<DbContext, ClientDbContext>();
            builder.Services.AddDbContext<ClientDbContext>(options =>
            {
                options.UseInMemoryDatabase("EscortDatabase");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Escort.Client.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.MapControllers();

            app.Run();
        }
    }
}