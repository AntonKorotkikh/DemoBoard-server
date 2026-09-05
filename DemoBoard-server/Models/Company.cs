using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DemoBoard_server.Models;

[Index(nameof(Id), IsUnique = true)]
public class Company : IEntity
{
    public long Id { get; set; }
    public virtual required IdentityUser Account { get; set; }
    public string? LogoPath { get; set; }
    public string BusinessName { get; set; }
    
    public virtual List<Vacancy> PostedVacancies { get; set; }
}