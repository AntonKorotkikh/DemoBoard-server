namespace DemoBoard_server.DTOs;

public class PersonAccountCreationDTO
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}