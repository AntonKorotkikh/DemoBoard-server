using DemoBoard_server.Data;
using DemoBoard_server.Repositories;
using DemoBoard_server.Security;
using DemoBoard_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DemoBoard_server.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        
        builder.Services.AddScoped<VacancyRepository>();        // needs an interface
        builder.Services.AddScoped<PersonRepository>();         // needs an interface
        builder.Services.AddScoped<CompanyRepository>();        // needs an interface
        
        builder.Services.AddScoped<VacancyService>();           // needs an interface
        builder.Services.AddScoped<AccountService>();           // needs an interface
        builder.Services.AddScoped<PersonService>();            // needs an interface
        builder.Services.AddScoped<CompanyService>();           // needs an interface
    }

    public static void AddDatabaseService(this IHostApplicationBuilder builder)
    {
        var databasePath = Path.Combine(AppContext.BaseDirectory, "DemoBoard.db");
        var connectionString = builder.Configuration.GetConnectionString("DemoBoard") ?? $"Data Source={databasePath}";
        builder.Services.AddSqlite<DatabaseContext>(connectionString, optionsAction: options =>
            options.UseLazyLoadingProxies());
    }

    public static void AddSecurity(this IHostApplicationBuilder builder)
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
                policy.WithOrigins("http://localhost:5173");    // probably shouldn't hardcode this
            });
        });
    }
}