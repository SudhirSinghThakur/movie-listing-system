using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MultipeAuthenticationSupport.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1", new() { Title = "Movie API", Version = "v1" });
            setup.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "JWT only (no 'Bearer ' prefix). Example: `eyJ...`",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            setup.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
            });
        });

        // Versioning
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // EF Core
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("MovieDb"));

        // Health Checks
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("Database");

        builder.Services.AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(15);
            options.AddHealthCheckEndpoint("Movie API", "/health");
            options.SetHeaderText("🎬 Movie Listing System Health");
        }).AddInMemoryStorage();

        // Authentication: Multi-scheme
        builder.Services.AddAuthentication("MultiScheme")
            .AddPolicyScheme("MultiScheme", "Token-based scheme selector", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    if (string.IsNullOrEmpty(token)) return "Local";

                    try
                    {
                        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                        Console.WriteLine($"🔎 Issuer: {jwt.Issuer}");
                        return jwt.Issuer.Contains("auth0.com") ? "SSO" : "Local";
                    }
                    catch
                    {
                        return "Local";
                    }
                };
            })
            .AddJwtBearer("Local", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            })
            .AddJwtBearer("SSO", options =>
            {
                options.Authority = "https://sudhirthakur.eu.auth0.com/";
                options.Audience = "https://moviesystem/api";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "https://sudhirthakur.eu.auth0.com/",
                    ValidAudience = "https://moviesystem/api",
                    ValidateLifetime = true
                };
            });

        builder.Services.AddAuthorization();


        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        var app = builder.Build();

        // Middlewares
        app.UseCors("AllowFrontend");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        // Logging middleware
        app.Use(async (context, next) =>
        {
            Console.WriteLine($"➡️  {context.Request.Method} {context.Request.Path}");
            await next();
        });

        // Routes
        app.MapControllers();

        // Health Check Endpoints
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        error = e.Value.Exception?.Message,
                        duration = e.Value.Duration.ToString()
                    })
                });
                await context.Response.WriteAsync(result);
            }
        });

        app.MapHealthChecksUI();
        app.UseExceptionHandler("/error");
        // Seed data
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DbSeeder.Seed(dbContext);
        }
        app.MapGet("/", () => Results.Ok("🎬 Movie API is up and running!"));
        app.Run();
    }
}