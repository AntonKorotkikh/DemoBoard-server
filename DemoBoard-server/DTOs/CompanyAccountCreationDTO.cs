namespace DemoBoard_server.DTOs;

public class CompanyAccountCreationDTO
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    // public string? LogoPath { get; set; }        accept binary data instead of this
    public string BusinessName { get; set; }
}