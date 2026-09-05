using DemoBoard_server.Data;
using DemoBoard_server.Logging;
using DemoBoard_server.Models;
using DemoBoard_server.Repositories;
using DemoBoard_server.Security;
using DemoBoard_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DemoBoard_server;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = LogConfiguration.CreateLogger();
        Log.Information("Starting DemoBoard-server");
        try
        {
            StartApp(args);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Unhandled exception");
            Environment.ExitCode = 1;
        }
        finally
        {
            Log.Information("Closing DemoBoard-server");
            Log.CloseAndFlush();
        }
    }

    private static void StartApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        
        var connectionString = builder.Configuration.GetConnectionString("DemoBoard") ?? "Data Source=DemoBoard.db";
        builder.Services.AddSqlite<DatabaseContext>(connectionString, optionsAction: options =>
            options.UseLazyLoadingProxies());

        ConfigureSecurity(builder);
        
        builder.Services.AddScoped<VacancyRepository>();        // needs an interface
        builder.Services.AddScoped<PersonRepository>();         // needs an interface
        builder.Services.AddScoped<CompanyRepository>();        // needs an interface
        
        builder.Services.AddScoped<VacancyService>();           // needs an interface
        builder.Services.AddScoped<AccountService>();           // needs an interface
        builder.Services.AddScoped<PersonService>();            // needs an interface
        builder.Services.AddScoped<CompanyService>();           // needs an interface

        var app = builder.Build();

        ConfigureRoles(app);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
            app.MapOpenApi();

        app.UseHttpsRedirection();
        
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void ConfigureSecurity(WebApplicationBuilder builder)
    {
        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DatabaseContext>();

        builder.Services.AddAuthorization(options =>
        {
             options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("AccessPolicy", policy =>
                policy.Requirements.Add(new SameAccountRequirement()));

        builder.Services.AddSingleton<IAuthorizationHandler, AccountAuthorizationHandler>();
        builder.Services.AddSingleton<IAuthorizationHandler, CompanyAuthorizationHandler>();
        builder.Services.AddSingleton<IAuthorizationHandler, PersonAuthorizationHandler>();
        builder.Services.AddSingleton<IAuthorizationHandler, VacancyAuthorizationHandler>();
        
        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 10;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            options.Lockout.MaxFailedAccessAttempts = 5;

            // User settings
            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedEmail = false;
        });
        
        builder.Services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(3);

            options.LoginPath = "/api/Account/login";
            //options.AccessDeniedPath = "/api/Account/AccessDenied";
            options.SlidingExpiration = true;
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:5173");
            });
        });
    }

    private static void ConfigureRoles(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new [] { "Admin", "Person", "Company" };
        foreach (var role in roles)
        {
            var roleExists = roleManager.RoleExistsAsync(role).Result;

            if (!roleExists)
                roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}