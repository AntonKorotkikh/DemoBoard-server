namespace DemoBoard_server.DTOs;

public class CompanyUpdateDTO
{
    public required string CompanyAccountName { get; set; }
    public string? LogoPath { get; set; }
    public required string BusinessName { get; set; }
}