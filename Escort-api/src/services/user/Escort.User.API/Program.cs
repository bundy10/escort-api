using Escort.User.Application.Repositories;
using Escort.User.Application.Services;
using Escort.User.Infrastructure.DBcontext;
using Escort.User.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Escort.User.API
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
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            // DbContext
            builder.Services.AddScoped<DbContext, UserDbContext>();
            builder.Services.AddDbContext<UserDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
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

            app.MapControllers();

            app.Run();
        }
    }
}