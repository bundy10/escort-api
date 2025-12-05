using Escort.Safety.API.Services;
using Stripe;
using Amazon.Rekognition;
using Amazon.Runtime;

namespace Escort.Safety.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load environment variables from .env file
            DotNetEnv.Env.Load();
            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            // Configure Stripe
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
            
            // Configure AWS Rekognition
            var awsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") 
                ?? builder.Configuration["AWS:AccessKeyId"];
            var awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY") 
                ?? builder.Configuration["AWS:SecretAccessKey"];
            var awsRegion = Environment.GetEnvironmentVariable("AWS_REGION") 
                ?? builder.Configuration["AWS:Region"] 
                ?? "us-east-1"; // Default region

            if (!string.IsNullOrEmpty(awsAccessKey) && !string.IsNullOrEmpty(awsSecretKey))
            {
                var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
                builder.Services.AddSingleton<IAmazonRekognition>(
                    new AmazonRekognitionClient(credentials, Amazon.RegionEndpoint.GetBySystemName(awsRegion)));
            }
            else
            {
                // Use default credentials (IAM role, environment variables, or credentials file)
                builder.Services.AddSingleton<IAmazonRekognition>(
                    new AmazonRekognitionClient(Amazon.RegionEndpoint.GetBySystemName(awsRegion)));
            }
            
            // Register services
            builder.Services.AddScoped<IIdentityVerificationService, IdentityVerificationService>();
            builder.Services.AddHttpClient<IUserUpdateService, UserUpdateService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Escort.Safety.API v1"));
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
