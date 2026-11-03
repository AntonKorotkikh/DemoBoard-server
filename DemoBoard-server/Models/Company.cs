using DemoBoard_server.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DemoBoard_server.Models;

[Index(nameof(Id), IsUnique = true)]
public class Company : IEntity, IUserOwnedResource
{
    public long Id { get; set; }
    public virtual required IdentityUser Account { get; set; }
    public string? LogoPath { get; set; }
    public required string BusinessName { get; set; }
    public virtual required IdentityUser Owner { get; set; }
    
    public virtual List<Vacancy> PostedVacancies { get; set; }
}