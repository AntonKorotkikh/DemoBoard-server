using Microsoft.EntityFrameworkCore;

namespace DemoBoard_server.Models;

[Index(nameof(Id), IsUnique = true)]
public class Vacancy : IEntity
{
    public long Id { get; set; }    // a GUID/UUID would be better?
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime TimeAdded { get; set; }
    public virtual required Company Company { get; set; }
}