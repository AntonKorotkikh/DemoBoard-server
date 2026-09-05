using DemoBoard_server.Models;

namespace DemoBoard_server.DTOs;

public class VacancyDTO
{
    public long Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime TimeAdded { get; set; }
    public required CompanyDTO Company { get; set; }
}