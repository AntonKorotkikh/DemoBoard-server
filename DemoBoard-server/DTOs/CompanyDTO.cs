using DemoBoard_server.Models;

namespace DemoBoard_server.DTOs;

public class CompanyDTO
{
    public long Id { get; set; }
    public required AccountResponseDTO Account { get; set; }
    public string? LogoPath { get; set; }
    public string BusinessName { get; set; }
}