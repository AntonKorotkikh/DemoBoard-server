namespace DemoBoard_server.DTOs;

public class CompanyResponseDTO
{
    public required long Id { get; set; }
    public required string CompanyAccountName { get; set; }
    public string? LogoPath { get; set; }
    public required string BusinessName { get; set; }
}