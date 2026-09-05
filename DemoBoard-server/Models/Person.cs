using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DemoBoard_server.Models;

[Index(nameof(Id), IsUnique = true)]
public class Person : IEntity
{
    public long Id { get; set; }
    public virtual required IdentityUser Account { get; set; }

    public virtual List<Vacancy> SavedVacancies { get; set; }
}