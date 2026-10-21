using DemoBoard_server.Models;

namespace DemoBoard_server.DTOs;

public class VacancyUpdateDTO
{
    public required long Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public required string CompanyAccountName { get; set; }
}