using DemoBoard_server.Models;
using DemoBoard_server.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoBoard_server.Data;

public class DatabaseContext : IdentityDbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    
    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityUser>().Property(user => user.UserName).IsRequired();
        builder.Entity<IdentityUser>().Property(user => user.Email).IsRequired();

        ConfigureCascadeDelete(builder);
    }

    private void ConfigureCascadeDelete(ModelBuilder builder)
    {
        var entityTypes = builder.Model.GetEntityTypes();
        foreach (var entityType in entityTypes)
        {
            var entityClrType = entityType.ClrType;
            if (!entityClrType.GetInterfaces().Contains(typeof(IUserOwnedResource)))
                continue;

            builder.Entity(entityClrType)
                .HasOne(typeof(IdentityUser), "Owner")
                .WithMany()
                .HasForeignKey("OwnerId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}