using DemoBoard_server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoBoard_server.Data;

public class DatabaseContext : IdentityDbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityUser>().Property(user => user.UserName).IsRequired();
        builder.Entity<IdentityUser>().Property(user => user.Email).IsRequired();

        // add code for cascading deletion
    }

    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Company> Companies { get; set; }
}