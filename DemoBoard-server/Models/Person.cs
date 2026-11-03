using DemoBoard_server.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DemoBoard_server.Models;

[Index(nameof(Id), IsUnique = true)]
public class Person : IEntity, IUserOwnedResource
{
    public long Id { get; set; }
    public virtual required IdentityUser Owner { get; set; }

    public virtual List<Vacancy> SavedVacancies { get; set; }
}