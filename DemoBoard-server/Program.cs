using DemoBoard_server.Data;
using DemoBoard_server.Extensions;
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
        
        builder.AddDatabaseService();
        builder.AddSecurity();
        builder.AddWebServices();

        var app = builder.Build();

        app.ConfigureRoles();

        if (app.Environment.IsDevelopment())
            app.MapOpenApi();

        app.UseHttpsRedirection();
        
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}